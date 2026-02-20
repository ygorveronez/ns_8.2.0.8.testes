using System;

namespace Dominio.ObjetosDeValor.Embarcador.Escrituracao
{
    public class FiltroRelatorioCompetencia
    {
        public int CodigoFilial { get; set; }

        public int CodigoTransportador { get; set; }

        public double CnpjCpfTomador { get; set; }

        public int CodigoTipoOperacao { get; set; }

        public DateTime DataEmissaoInicial { get; set; }

        public DateTime DataEmissaoFinal { get; set; }

        public DateTime DataCargaInicial { get; set; }

        public DateTime DataCargaFinal { get; set; }

        public DateTime DataEmissaoCTeInicial { get; set; }

        public DateTime DataEmissaoCTeFinal { get; set; }

        public DateTime DataEmissaoNotaInicial { get; set; }

        public DateTime DataEmissaoNotaFinal { get; set; }

        public bool VisualizarTambemDocumentosAguardandoProvisao { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string NumeroValePedagio { get; set; }
    }
}
