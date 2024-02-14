using Models;
using Newtonsoft.Json;
using System;

class MessageCreator
{
	private readonly Client _client;

	public MessageCreator(Client client)
	{
		_client = client;
	}

	public async Task<string> CreateMessage()
	{
		Message message = new()
		{
			Sender = _client,
		};
		
		// Simulate a delay to fix console output
		await Task.Delay(1000);

		Console.WriteLine("1. Quit");
        Console.WriteLine("2. Get clients");
		Console.WriteLine("3. Send message");
		Console.Write("Choose an option: ");

		string input = Console.ReadLine();
		int option;
		while (int.TryParse(input, out option) && !(option >= 1 && option <= 3))
		{
            Console.WriteLine("Invalid option");
			Console.Write("Choose an option: ");
			input = Console.ReadLine();
        }

		switch (option)
		{
			case 1:
				message.Type = ActionType.Quit;
				break;

			case 2:
				message.Type = ActionType.GetClients;
				break;

			case 3:
				message.Type = ActionType.Message;
				Console.Write("Write the receiver's id: ");
				string receiverId = Console.ReadLine();
				Guid id;
				while (!Guid.TryParse(receiverId, out id))
				{
					receiverId = Console.ReadLine();
				}
				message.Receiver = new() { ID = id };

				Console.Write("Write the message: ");
				message.Content = Console.ReadLine();
				break;
		}

        return JsonConvert.SerializeObject(message);
	}
}