namespace Models;

public class Message
{
	public Client Sender { get; set; }
    public Client Receiver { get; set; }
	public string Content { get; set; }
	public ActionType Type { get; set; }
}
