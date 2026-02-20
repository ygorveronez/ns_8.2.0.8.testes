using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public sealed class CargaProduto
    {
        #region Propriedades

        public int Codigo { get; set; }

        public string CargaProdutoDescricao { get; set; }

        public string CnpjTransportador { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string CodigoProdutoEmbarcador { get; set; }

        public string DescricaoProduto { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public decimal PesoUnitario { get; set; }

        public decimal Quantidade { get; set; }

        public string TipoFrete { get; set; }

        public string RazaoSocialTransportador { get; set; }

        private double CPFCNPJDestinatario { get; set; }

        private string TipoDestinatario { get; set; }

        public string Destinatario { get; set; }

        public string LocalidadeDestinatario { get; set; }

        public SituacaoCargaJanelaCarregamento SituacaoJanelaCarregamento { get; set; }

        private SituacaoCarga SituacaoCarga { get; set; }

        public DateTime DataCarregamentoCarga { get; set; }

        public decimal TotalPallets { get; set; }

        public string UnidadeMedida { get; set; }

        public int SequenciaRoteirizacao { get; set; }

        public string Bloco { get; set; }

        public int OrdemCarregamento { get; set; }

        public int OrdemEntrega { get; set; }

        public string DescricaoGrupoProduto { get; set; }

        public decimal CubagemPedido { get; set; }

        private double CPFCNPJRecebedor { get; set; }

        public string RecebedorDescricao { get; set; }

        public string LocalidadeRecebedor { get; set; }

        private DateTime DataCriacao { get; set; }

        public string Filial { get; set; }

        private double CPFCNPJRemetente { get; set; }

        private string TipoRemetente { get; set; }

        public string Remetente { get; set; }

        public decimal ValorMercadoria { get; set; }

        public int Saldo { get; set; }

        private DateTime DataInicioJanela { get; set; }

        private DateTime DataFimJanela { get; set; }

        public string CodigoGrupoProduto { get; set; }

        public string PedidoComAgenda { get; set; }

        public string PlacaVeiculos { get; set; }

        public string Motoristas { get; set; }


        #endregion

        #region Propriedades com Regras

        public decimal PesoTotal
        {
            get { return Quantidade * PesoUnitario; }
        }

        public string CPFCNPJDestinatarioFormatado
        {
            get
            {
                if (TipoDestinatario == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoDestinatario == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJDestinatario) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJDestinatario);
            }
        }

        public string CPFCNPJRecebedorFormatado
        {
            get
            {
                return CPFCNPJRecebedor > 0 ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJRecebedor) : "";
            }
        }

        public string DescricaoSituacaoJanelaCarregamento
        {
            get { return SituacaoCarga.IsSituacaoCargaFaturada() ? "Faturada" : SituacaoJanelaCarregamento.ObterDescricao(); }
        }

        public string DataCarregamentoFormatada
        {
            get { return DataCarregamentoCarga != DateTime.MinValue ? DataCarregamentoCarga.ToString("dd/MM/yyyy") : ""; }
        }

        public string Barcode
        {
            get
            {
                return $"*{CodigoCargaEmbarcador}*";
            }
        }

        public string DataCriacaoFormatada
        {
            get { return DataCriacao != DateTime.MinValue ? DataCriacao.ToString("dd/MM/yyyy") : ""; }
        }

        public string CPFCNPJRemetenteFormatado
        {
            get
            {
                if (TipoRemetente == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoRemetente == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJRemetente) : string.Format(@"{0:000\.000\.000\-00}", CPFCNPJRemetente);
            }
        }

        public string DataInicioJanelaFormatada
        {
            get { return DataInicioJanela != DateTime.MinValue ? DataInicioJanela.ToString("dd/MM/yyyy") : ""; }
        }

        public string DataFimJanelaFormatada
        {
            get { return DataFimJanela != DateTime.MinValue ? DataFimJanela.ToString("dd/MM/yyyy") : ""; }
        }

        #endregion
    }
}
