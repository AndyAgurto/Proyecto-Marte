using System;

namespace Marte.Domain.Entities
{
    public class Asistencia
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid AsistenteId { get; set; }
        public Asistente Asistente { get; set; } = null!;

        public DateTime Fecha { get; set; } = DateTime.Today;
        public DateTime HoraIngreso { get; set; }
        public DateTime? HoraSalida { get; set; }

        public bool HastaCierre { get; set; } = false;
        public string? Observacion { get; set; }
    }
}
