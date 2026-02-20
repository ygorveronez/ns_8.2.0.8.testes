using System;

namespace Dominio.ObjetosDeValor.Embarcador.Dashboard
{
    public class DetalhesDashboardDocumentacao
    {
        public int CodigoCarga { get; set; }
        public string NomeNavio { get; set; }
        public DateTime? DataEtsPol { get; set; }
        public DateTime? DataFechamento { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }
        public string Regiao { get; set; }
        public string Carga { get; set; }
        public string LinkCarga { get; set; }
        public string Booking { get; set; }
        public string StatusCarga { get; set; }
        public string StatusSvm { get; set; }
        public string StatusMercante { get; set; }
        public string Containeres { get; set; }
        public int QtdCntrCarga { get; set; }
        public string Modal { get; set; }
        public string TipoTomador { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
    }
}
