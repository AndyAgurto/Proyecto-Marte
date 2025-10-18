namespace Marte.Application.Models
{
    public class LoginRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
    }
}