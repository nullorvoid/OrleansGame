using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;


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
			silo = new SiloHostBuilder()
				.UseLocalhostClustering()
				.Configure<ClusterOptions>(options =>
				{
					options.ClusterId = "dev";
					options.ServiceId = "game";
				})
				.Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
				.ConfigureLogging(logging => logging.AddConsole())
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
