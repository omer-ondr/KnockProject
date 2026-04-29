using Microsoft.AspNetCore.SignalR;

namespace KnockProject.API.Hubs;

public class ProgressHub : Hub
{
    // Cihaz bağlandığında herhangi bir özel işlem yapmamıza gerek yok
    // Client sadece ConnectionId'sini alarak HTTP isteğine ekleyecek.
    public string GetConnectionId() => Context.ConnectionId;
}
