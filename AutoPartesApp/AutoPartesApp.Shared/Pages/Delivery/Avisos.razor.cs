using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Shared.Pages.Delivery
{
    public partial class Avisos : ComponentBase
    {
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        // State
        private string selectedTab = "pending";
        private int pendingCount => pendingNotifications.Count;

        // Data
        private List<NotificationItem> pendingNotifications = new();
        private List<NotificationItem> olderNotifications = new();
        private List<NotificationItem> readNotifications = new();

        protected override void OnInitialized()
        {
            LoadNotifications();
        }

        private void LoadNotifications()
        {
            // Notificaciones pendientes recientes
            pendingNotifications = new List<NotificationItem>
            {
                new NotificationItem
                {
                    Id = 1,
                    Type = "order",
                    Title = "Nuevo Pedido #3920",
                    Time = "Hace 5 min",
                    Message = "Tienes un nuevo pedido disponible a 1.2km de tu ubicación actual.",
                    IsRead = false,
                    HasAction = false
                },
                new NotificationItem
                {
                    Id = 2,
                    Type = "admin",
                    Title = "Mensaje Admin",
                    Time = "Hoy, 10:15 AM",
                    Message = "Recuerda actualizar tus documentos de circulación antes del viernes.",
                    IsRead = false,
                    HasAction = true
                },
                new NotificationItem
                {
                    Id = 3,
                    Type = "delivery",
                    Title = "Nuevo Pedido #3915",
                    Time = "Hace 1 hora",
                    Message = "Pedido listo para recolección en 'Restaurante Central'.",
                    IsRead = false,
                    HasAction = false
                }
            };

            // Notificaciones antiguas
            olderNotifications = new List<NotificationItem>
            {
                new NotificationItem
                {
                    Id = 4,
                    Type = "payment",
                    Title = "Pago Procesado",
                    Time = "Ayer, 18:30",
                    Message = "Tu resumen semanal ha sido generado satisfactoriamente.",
                    IsRead = true,
                    HasAction = false
                }
            };

            // Notificaciones leídas (inicialmente vacío)
            readNotifications = new List<NotificationItem>();
        }

        // Tab Selection
        private void SelectTab(string tab)
        {
            selectedTab = tab;
            StateHasChanged();
        }

        // Mark single notification as read
        private void MarkAsRead(int notificationId)
        {
            var notification = pendingNotifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                readNotifications.Add(notification);
                pendingNotifications.Remove(notification);

                Console.WriteLine($"✅ Notificación {notificationId} marcada como leída");
                StateHasChanged();
            }
        }

        // Mark all as read
        private void MarkAllAsRead()
        {
            Console.WriteLine("📬 Marcando todas las notificaciones como leídas...");

            // Mover todas las pendientes a leídas
            foreach (var notification in pendingNotifications.ToList())
            {
                notification.IsRead = true;
                readNotifications.Add(notification);
            }

            foreach (var notification in olderNotifications.ToList())
            {
                notification.IsRead = true;
                readNotifications.Add(notification);
            }

            pendingNotifications.Clear();
            olderNotifications.Clear();

            Console.WriteLine($"✅ {readNotifications.Count} notificaciones marcadas como leídas");
            StateHasChanged();
        }

        // Toggle notification state
        private void ToggleNotification(int notificationId)
        {
            var notification = pendingNotifications.FirstOrDefault(n => n.Id == notificationId);

            if (notification != null)
            {
                MarkAsRead(notificationId);
            }
        }

        // Helper Methods
        private string GetIconBgClass(string type)
        {
            return type switch
            {
                "order" => "bg-primary",
                "admin" => "bg-slate-700",
                "delivery" => "bg-slate-700",
                "payment" => "bg-slate-700",
                _ => "bg-slate-700"
            };
        }

        private string GetIconName(string type)
        {
            return type switch
            {
                "order" => "shopping_bag",
                "admin" => "info",
                "delivery" => "local_shipping",
                "payment" => "payments",
                _ => "notifications"
            };
        }

        // Data Model
        private class NotificationItem
        {
            public int Id { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Time { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public bool IsRead { get; set; }
            public bool HasAction { get; set; }
        }
    }
}
