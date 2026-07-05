namespace PruebaTécnicaWebConLogin.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int? IntentosFallidos { get; set; }
        public bool? Bloqueado { get; set; }
        public int RoleId { get; set; }
        public DateTime? FechaBloqueo { get; set; }
    }

}
