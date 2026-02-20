using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class CCeNFe
    {
        public int CodigoNota { get; set; }
        public string NomeEmitente { get; set; }
        public string CNPJEmitente { get; set; }
        public string FoneEmitente { get; set; }
        public string TipoEmitente { get; set; }
        public int TipoAmbiente { get; set; }
        public string NomeDestinatario { get; set; }
        public double CNPJDestinatario { get; set; }
        public string TipoDestinatario { get; set; }
        public int NumeroNota { get; set; }
        public int SerieNota { get; set; }
        public string NumeroLote { get; set; }
        public string ChaveNota { get; set; }
        public string NumeroProtocolo { get; set; }
        public DateTime DataProcessamento { get; set; }
        public string Motivo { get; set; }

    }
}
