using Models;
using Newtonsoft.Json;
using System.Net.Sockets;

internal class Program
{
	private static Models.Client _clientModel;

	private static async Task Main(string[] args)
	{
		const int PORT = 12345;
		const string IP = "localhost";
		TcpClient client = new();

		Console.Write("Write your name: ");
		string name = Console.ReadLine();
		Console.WriteLine("Connecting to server...");

		try
		{
			client.Connect(IP, PORT);
			Console.WriteLine("Connected to server");

			NetworkStream stream = client.GetStream();
			StreamReader reader = new(stream);
			StreamWriter writer = new(stream);

			writer.WriteLine(name);
			await writer.FlushAsync();
            string jsonClientModel = await reader.ReadLineAsync();
			_clientModel = JsonConvert.DeserializeObject<Models.Client>(jsonClientModel);
            await Console.Out.WriteLineAsync($"Client Id: {_clientModel.ID}");

			Task readTask = ReadAsync(reader);
			Task writeTask = WriteAsync(writer);

			while (client.Connected) ;
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
			await Console.Out.WriteLineAsync("Failed to connect to server");
		}

		await Console.Out.WriteLineAsync("Disconnected from server");
	}

	private static async Task ReadAsync(StreamReader reader)
	{
		while (true)
		{
			string message = await reader.ReadLineAsync();
			Message msgModel = JsonConvert.DeserializeObject<Message>(message);

			string response = $"Received from [{msgModel?.Sender.Name}]:\n" +
				$"{msgModel?.Content.Replace(';', '\n')}";
			await Console.Out.WriteLineAsync(response);
		}
	}

	private static async Task WriteAsync(StreamWriter writer)
	{
		MessageCreator messageCreator = new(_clientModel);

		while (true)
		{
            if (!Console.KeyAvailable)
			{
				string message = await messageCreator.CreateMessage();
				await writer.WriteLineAsync(message);
				await writer.FlushAsync();
			}
		}
	}
}
