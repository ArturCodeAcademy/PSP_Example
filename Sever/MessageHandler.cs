using Models;
using Newtonsoft.Json;
using System.Text;

namespace Sever
{
	internal class MessageHandler
	{
		private List<Client> _clients;

		public MessageHandler(List<Client> clients)
		{
			_clients = clients;
		}

		public string HandleMessage(string message)
		{
			Message msgModel = JsonConvert.DeserializeObject<Message>(message);
			Message response = new()
			{
				Receiver = msgModel.Sender,
				Sender = new Models.Client {  Name = "Server" } 
			};

			switch (msgModel.Type)
			{
				case ActionType.Quit:
					return "Quit";

				case ActionType.GetClients:
					StringBuilder sb = new();
					foreach (Client client in _clients)
					{
						sb.Append($"{client.ClientModel.Name} {client.ClientModel.ID};");
					}
					response.Content = sb.ToString();
					break;

				case ActionType.Message:
					Client? receiver = _clients.FirstOrDefault(c => c.ClientModel.ID == msgModel.Receiver.ID);
					if (receiver is not null)
					{
						response.Content = msgModel.Content;
						response.Receiver = msgModel.Receiver;
						response.Sender = msgModel.Sender;
						receiver.Writer.WriteLine(JsonConvert.SerializeObject(response));
						receiver.Writer.Flush();

						return "Message sent";
					}
					else
					{
						response.Content = "Receiver not found";
					}

					break;

				default:
					response.Content = "Message not recognized";
					break;
			}

			return JsonConvert.SerializeObject(response);
		}
	}
}
