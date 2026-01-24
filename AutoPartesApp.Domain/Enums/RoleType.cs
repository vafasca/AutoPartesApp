using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPartesApp.Domain.Enums
{
    public enum RoleType
    {
        Admin = 1,
        Delivery = 2,
        Client = 3
    }

    public static class RoleTypeExtensions
    {
        public static string ToFriendlyString(this RoleType role)
        {
            return role switch
            {
                RoleType.Admin => "Administrador",
                RoleType.Delivery => "Repartidor",
                RoleType.Client => "Cliente",
                _ => "Desconocido"
            };
        }

        public static RoleType FromString(string role)
        {
            return role.ToLower() switch
            {
                "admin" or "administrador" => RoleType.Admin,
                "delivery" or "repartidor" => RoleType.Delivery,
                "client" or "cliente" => RoleType.Client,
                _ => RoleType.Client
            };
        }
    }
}
