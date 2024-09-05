using Microsoft.AspNet.SignalR;

public class AuthHub : Hub
{
    public void SendAuthSuccess(string connectionId)
    {

        Clients.Client(connectionId).authSuccess();
    }
}