using System;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class Navio
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public DateTime DataHoraDeadLine { get; set; }
        public DateTime DataHoraDeadLCarga { get; set; }
    }
}
