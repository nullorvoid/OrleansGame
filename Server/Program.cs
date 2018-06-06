using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Clustering.AdoNet;

using GrainImplementations;

// Editor Silo configuration and start up from
// https://github.com/dotnet/orleans/blob/master/Samples/2.0/docker-aspnet-core/Silo/Program.cs
namespace Server
{
	class Program
	{
		private static ISiloHost silo;
		private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

		static void Main(string[] args)
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(Environment.GetEnvironmentVariable("DB_SERVICE"));

			IPAddress ip = null;
			if (hostEntry.AddressList.Length > 0)
			{
				ip = hostEntry.AddressList[0];
			}

			string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
			string password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
			string database = Environment.GetEnvironmentVariable("POSTGRES_DB");

			string connectionString = $"Server={ip};Port=5432;Database={database};User Id={user};Password={password};";

			silo = new SiloHostBuilder()
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = "dev";
					options.ServiceId = "game";
				})
				.UseAdoNetClustering(options =>
				{
				options.ConnectionString = connectionString;
				options.Invariant = "Npgsql";
				})
				.ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
				.ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
				// PubSubStore is a special store that manages pub sub of streams
				// For development memory is used however it should be moved to a persistent
				// data store in production.
				// Orleans also Gcs resources from unused streams, this will probably be applicable
				// as we will use it for game streams that should be cleaned up after a game has finished
				.AddMemoryGrainStorage("PubSubStore")
				.AddSimpleMessageStreamProvider("GameStream")
				.Build();

			Task.Run(StartSilo);

			AssemblyLoadContext.Default.Unloading += context =>
			{
				Task.Run(StopSilo);
				siloStopped.WaitOne();
			};

			siloStopped.WaitOne();
		}

		private static async Task StartSilo()
		{
			await silo.StartAsync();
			Console.WriteLine("Silo started");
		}

		private static async Task StopSilo()
		{
			await silo.StopAsync();
			Console.WriteLine("Silo stopped");
			siloStopped.Set();
		}
	}
}
