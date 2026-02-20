using System;

namespace Dominio.ObjetosDeValor.Embarcador.Dashboard
{
    public class DashboardDocumentacao
    {
        public int NavioAberto { get; set; }
        public string StatusNavio { get; set; }
        public string NomeNavio { get; set; }
        public DateTime? DataEtsPol { get; set; }
        public Embarques Embarques { get; set; }
        public string Regiao { get; set; }
        public string Mdfe { get; set; }
        public DateTime? DataFechamento { get; set; }
        public int? Bookings { get; set; }
        public int? TotalCargaPedentes { get; set; }
        public int? TotalCargaEmissao { get; set; }
        public int? TotalCargaErro { get; set; }
        public int? TotalCargaGerada { get; set; }
        public int? TotalSvmPendente { get; set; }
        public int? TotalSvmErro { get; set; }
        public int? TotalSvmGerado { get; set; }
        public int? TotalMercantePendente { get; set; }
        public int? TotalMercanteRetornado { get; set; }
        public int? TotalPendenteMdfe { get; set; }
        public int? TotalCarga { get; set; }
        public int? TotalSvm { get; set; }
        public int? TotalMercante { get; set; }
    }

    public class Embarques
    {
        public int Codigo { get; set; }
        public string Descricao { get; set; }
    } 
}
