using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using Orleans;
using Orleans.Runtime;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Streams;

using GrainInterfaces.Game;
using GrainInterfaces.Game.Messages;
using GrainInterfaces.Game.Messages.Actions;
using GrainInterfaces.Player;

using Client.Observers;

// Client sample taken from
// https://github.com/dotnet/orleans/blob/master/Samples/2.0/HelloWorld/src/OrleansClient/Program.cs
// Temporary test client before moving to an ASP.Net Core Web Frontend
namespace Client
{
	class Program
	{
		static int Main(string[] args)
		{
			return RunMainAsync().Result;
		}

		private static async Task<int> RunMainAsync()
		{
			try
			{
				using (var client = await StartClientWithRetries())
				{
					await DoClientWork(client);
					Console.ReadKey();
				}

				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Console.ReadKey();
				return 1;
			}
		}

		private static async Task<IClusterClient> StartClientWithRetries(int initializeAttemptsBeforeFailing = 5)
		{
			int attempt = 0;
			IClusterClient client;
			while (true)
			{
				try
				{
					client = new ClientBuilder()
						.UseLocalhostClustering()
						.Configure<ClusterOptions>(options =>
						{
							options.ClusterId = "dev";
							options.ServiceId = "game";
						})
						.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IGame).Assembly).WithReferences())
						.ConfigureLogging(logging => logging.AddConsole())
						.AddSimpleMessageStreamProvider("GameStream")
						.Build();

					await client.Connect();

					Console.WriteLine("Client successfully connect to silo host");

					break;
				}
				catch (SiloUnavailableException)
				{
					attempt++;
					Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
					if (attempt > initializeAttemptsBeforeFailing)
					{
						throw;
					}
					await Task.Delay(TimeSpan.FromSeconds(4));
				}
			}

			return client;
		}

		private static async Task DoClientWork(IClusterClient client)
		{
			// Create a player
			string playerId = "nullorvoid";
			IPlayer player = client.GetGrain<IPlayer>(playerId + Guid.NewGuid().ToString());

			// Connect to a game
			// TODO: Write a game creation grain for setting this up
			Guid gameId = Guid.Empty;
			IGame game = client.GetGrain<IGame>(gameId);

			// Register to the game stream using the game id
			// Streams are identified by stream IDs, which are just logical names comprised of GUIDs and strings.
			IStreamProvider streamProvider = client.GetStreamProvider("GameStream");
			IAsyncStream<GameMessage> gameStream = streamProvider.GetStream<GameMessage>(gameId, "game");
			StreamSubscriptionHandle<GameMessage> handleGame = await gameStream.SubscribeAsync(new GameStreamObserver(client.ServiceProvider.GetService<ILoggerFactory>().CreateLogger("GameStreamObserver")));

			IAsyncStream<GameMessage> actionStream = streamProvider.GetStream<GameMessage>(gameId, "actions");
			StreamSubscriptionHandle<GameMessage> handleActions = await actionStream.SubscribeAsync(new GameStreamActionObserver(client.ServiceProvider.GetService<ILoggerFactory>().CreateLogger("GameStreamActionObserver")));

			// For testing we're going to throw it all in a giant try catch >.<
			// TODO: put in a testing framework.
			try
			{
				await player.SetName("Rob Towell");
				await game.Join(player);
				await Task.Delay(2000);
				await game.ProcessActionMessage(player, new MoveMessage() { PlayerId = player.GetPrimaryKeyString(), Direction = "left" });
				await Task.Delay(2000);
				await game.Leave(player);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}

			// unsubscribe from the stream to clean up
			await handleGame.UnsubscribeAsync();
			await handleActions.UnsubscribeAsync();

			Console.WriteLine("Work Completed");
		}
	}
}
