namespace Dominio.Relatorios.Embarcador.DataSource.Fretes
{
    public class DocumentoHavanModeloVeicular
    {
        public int Codigo { get; set; }
        public string CodigoIntegracao { get; set; }

        public decimal BITREM { get; set; }
        public decimal CARRETA2E { get; set; }
        public decimal CARRETA3E { get; set; }
        public decimal TOCO { get; set; }
        public decimal TRUCK { get; set; }
        public decimal CARRETA6EIXOSLS { get; set; }
        public decimal CARRETA6EIXOSVANDERLEIA { get; set; }
        public decimal CARRETARODOTREM { get; set; }
        public decimal RODOTREM { get; set; }

        public decimal PEDAGIOBITREM { get; set; }
        public decimal PEDAGIOCARRETA2E { get; set; }
        public decimal PEDAGIOCARRETA3E { get; set; }
        public decimal PEDAGIOTOCO { get; set; }
        public decimal PEDAGIOTRUCK { get; set; }
        public decimal PEDAGIOCARRETA6EIXOSLS { get; set; }
        public decimal PEDAGIOCARRETA6EIXOSVANDERLEIA { get; set; }
        public decimal PEDAGIOCARRETARODOTREM { get; set; }
        public decimal PEDAGIORODOTREM { get; set; }

        public decimal BITREMHAVAN { get; set; }
        public decimal CARRETAHAVAN { get; set; }
        public decimal CARRETARODOTREMHAVAN { get; set; }
        public decimal RODOTREMHAVAN { get; set; }

        public decimal PEDAGIOBITREMHAVAN { get; set; }
        public decimal PEDAGIOCARRETAHAVAN { get; set; }
        public decimal PEDAGIOCARRETARODOTREMHAVAN { get; set; }
        public decimal PEDAGIORODOTREMHAVAN { get; set; }

        public string BairroOrigem { get; set; }
        public string CidadeOrigem { get; set; }
        public string UFOrigem { get; set; }
        public string BairroDestino { get; set; }
        public string CidadeDestino { get; set; }
        public string UFDestino { get; set; }
        public int Quilometragem { get; set; }
    }
}
