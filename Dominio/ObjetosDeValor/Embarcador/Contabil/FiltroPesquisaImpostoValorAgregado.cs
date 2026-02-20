using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Contabil
{
    public sealed class FiltroPesquisaImpostoValorAgregado
    {
        public int CodigoDesconsiderar{ get; set; }

        public string CodigoIVA { get; set; }

        public int CodigoModeloDocumentoFiscal { get; set; }

        public bool? DestinatarioExterior { get; set; }

        public bool? ImpostoMaiorQueZero { get; set; }

        public bool? PermitirInformarManualmente { get; set; }

        public UsoMaterial? UsoMaterial { get; set; }
    }
}
