using Microsoft.AspNetCore.SignalR;

namespace ReinasApiPrueba
{
    public class NotificationHub : Hub
    {
        // Método para agregar un usuario a un grupo específico
        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the group {groupName}.");
        }

        // Método para eliminar un usuario de un grupo específico
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} has left the group {groupName}.");
        }

        // Método para enviar un mensaje a todos los miembros de un grupo
        public async Task SendMessage(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }
    }

    public class SomeService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SomeService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyGroupAsync(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message);
        }
    }


}
