using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Compras
{
    public class FiltroPesquisaFluxoCompra
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public SituacaoTratativaFluxoCompra SituacaoTratativa { get; set; }
        public SituacaoFluxoCompra Situacao { get; set; }
        public EtapaFluxoCompra EtapaAtual { get; set; }
        public int CodigoOrdemCompra { get; set; }
        public int CodigoCotacao { get; set; }
        public int CodigoRequisicaoMercadoria { get; set; }
        public int CodigoUsuario { get; set; }
        public double Fornecedor { get; set; }
        public int Produto { get; set; }
    }
}
