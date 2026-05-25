using System;
using System.Collections.Generic;
using System.Text;

namespace Social_Network.Core.Models
{
    internal class UserSettings
    {
        // Property
        public int Id { get; set; }
        public bool IsPrivateAccount { get; set; } = false;
        public bool NotificatonsEnabled { get; set; } = true;
        // Foreing key and navigaton property for User
        public int UserId { get; set; }
        public User? User { get; set; } 
    }
}
