using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using WebServer.HubControllers;

using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

using GrainInterfaces.Player;
using System.Net;
using Orleans.Clustering.AdoNet;

namespace WebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddSignalR();
			services.AddSingleton<IClusterClient>(CreateClusterClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();
			app.UseSignalR(routes =>
            {
                routes.MapHub<Lobby>("/lobby");
            });
        }

		private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
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

            var client = new ClientBuilder()
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
						.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IPlayer).Assembly).WithReferences())
						.ConfigureLogging(logging => logging.AddConsole())
						.AddSimpleMessageStreamProvider("GameStream")
						.Build();

            StartClientWithRetries(client).Wait();
            return client;
        }

        private static async Task StartClientWithRetries(IClusterClient client)
        {
            for (var i=0; i<5; i++)
            {
                try
                {
                    await client.Connect();
                    return;
                }
                catch(Exception)
                { }
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}
