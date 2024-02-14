using System.Net.Sockets;

class Client
{
	public Models.Client ClientModel;
	public TcpClient TcpClient;
	public NetworkStream Stream;
	public StreamReader Reader;
	public StreamWriter Writer;

	public Client(TcpClient tcpClient)
	{
		ClientModel = new();
		ClientModel.ID = Guid.NewGuid();
		TcpClient = tcpClient;
		Stream = tcpClient.GetStream();
		Reader = new(Stream);
		Writer = new(Stream);
	}
}