using System;

namespace Dominio.ObjetosDeValor.WebService.Rest.Unilever.Carga.DocumentoTransporte
{
    public class EventosDT
    {
        public string Qualificador { get; set; }
        public DateTime? DataInicioPrevisto { get; set; }
        public DateTime? DataFimPrevisto { get; set; }
        public DateTime? DataInicioReal { get; set; }
        public DateTime? DataFimReal { get; set; }
    }
}