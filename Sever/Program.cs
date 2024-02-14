using Newtonsoft.Json;
using Sever;
using System.Net;
using System.Net.Sockets;

internal class Program
{
	private static List<Client> _clients = new();

	private static void Main(string[] args)
	{
		const int PORT = 12345;
		TcpListener listener = new (IPAddress.Any, PORT);
		listener.Start();

        Console.WriteLine("Server is running on port " + PORT);

		while (true)
		{
			TcpClient client = listener.AcceptTcpClient();

			Thread clientThread = new (HandleClient);
			clientThread.Start(client);
		}
    }

	private static async void HandleClient(object? clientObj)
	{
		if (clientObj is not TcpClient tcpClient)
			return;

		Client client = new(tcpClient);
		_clients.Add(client);


		NetworkStream stream = tcpClient.GetStream();
		StreamReader reader = new (stream);
		StreamWriter writer = new (stream);

		string clientName = await reader.ReadLineAsync();
		client.ClientModel.Name = clientName;

		await Console.Out.WriteLineAsync($"Client {clientName} ({client.ClientModel.ID}) connected");

        string jsonClientModel = JsonConvert.SerializeObject(client.ClientModel);
        await writer.WriteAsync(jsonClientModel + '\n');
		await writer.FlushAsync();

		MessageHandler messageHandler = new(_clients);

        while (true)
		{
			string message = reader.ReadLine();
			Console.WriteLine($"Received from {clientName}: {message}");

			string response = messageHandler.HandleMessage(message);

			if (response == "Quit")
				break;

			writer.WriteLine(response + "\n");
			writer.Flush();
		}

		tcpClient.Close();
		_clients.Remove(client);
        Console.WriteLine($"Client {clientName} ({client.ClientModel.ID}) disconnected");
    }
}
