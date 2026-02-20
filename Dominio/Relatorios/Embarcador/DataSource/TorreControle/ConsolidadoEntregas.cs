using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.TorreControle
{
    public class ConsolidadoEntregas
    {
        #region Propriedades

        public int Codigo { get; set; }
        public string CodigoCargaEmbarcador { get; set; }
        public string NumeroPedido { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public decimal PesoTotalPedido { get; set; }
        public string DataCriacaoCarga { get; set; }
        public string DataInicioViagemPrevista { get; set; }
        public string DataInicioViagemRealizada { get; set; }
        public string DataPrevisaoEntrega { get; set; }
        public string DataEntregaReprogramada { get; set; }
        public string DataInicioEntrega { get; set; }
        public string DataEntradaRaio { get; set; }
        public string DataSaidaRaio { get; set; }
        public string DataConfirmacao { get; set; }
        public string DataPrevisaoEntregaAjustada { get; set; }
        private StatusPrazoEntrega StatusPrazoEntrega { get; set; }
        public int TempoAtraso { get; set; }
        public string MotivoUltimaOcorrencia { get; set; }
        public string DataUltimaOcorrencia { get; set; }
        public string DataInicioViagem { get; set; }
        public string DataFimEntrega { get; set; }
        public string UsuarioCanhoto { get; set; }
        public string DataCanhoto { get; set; }
        public string Transportador { get; set; }
        public string Motoristas { get; set; }
        private string PlacaTracao { get; set; }
        public string PlacasReboques { get; set; }
        public string DataUltimaPosicao { get; set; }
        private StatusPosicao StatusPosicaoVeiculo { get; set; }
        public string CoordenadasLocalizacaoAtual { get; set; }
        private TipoInteracaoEntrega TipoInteracaoInicioViagem { get; set; }
        private TipoInteracaoEntrega TipoInteracaoChegadaViagem { get; set; }
        private TipoInteracaoEntrega TipoInteracaoFimEntrega { get; set; }
        public decimal PercentualViagem { get; set; }
        public string CodigoFilial { get; set; }
        public string NomeFilial { get; set; }
        public string Origem { get; set; }
        public string CodigoOrigem { get; set; }
        public string CidadeUFOrigem { get; set; }
        public string EnderecoOrigem { get; set; }
        public string CoordenadasOrigem { get; set; }
        public string Cliente { get; set; }
        public string CidadeUFCliente { get; set; }
        public string EnderecoCliente { get; set; }
        public string CoordenadasCliente { get; set; }
        public int OrdemEntregaPrevista { get; set; }
        public int OrdemEntregaRealizada { get; set; }
        public decimal DistanciaPrevista { get; set; }
        public decimal DistanciaAteDestino { get; set; }
        public int PercentualCargaEntregue { get; set; }
        public string CodigoTipoOperacao { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProduto { get; set; }
        public string CodigoModeloVeicular { get; set; }
        public string DescricaoModeloVeicular { get; set; }
        public string TipoOcorrencia { get; set; }
        public string PossuiCanhoto { get; set; }
        public string EscritorioVenda { get; set; }
        public string EquipeVendas { get; set; }
        public string TipoMercado { get; set; }
        public string CanalVenda { get; set; }
        public string CanalEntrega { get; set; }
        public CargaCritica CargaCritica { get; set; }
        public PedidoCritico PedidoCritico { get; set; }

        private StatusViagemControleEntrega SituacaoViagem { get; set; }

        #endregion

        #region Propriedades com Regras

        public string PedidoCriticoFormatado
        {
            get { return this.PedidoCritico.ObterPedidoCriticoFormatado(); }
        }

        public string CargaCriticaFormatado
        {
            get { return this.CargaCritica.ObterCargaCriticaFormatado(); }
        }

        public string StatusPrazoEntregaFormatado
        {
            get { return this.StatusPrazoEntrega.ObterDescricao(); }
        }

        public string StatusPosicaoVeiculoFormatado
        {
            get { return this.StatusPosicaoVeiculo.ObterDescricao(); }
        }

        public string PlacaTracaoFormatada
        {
            get { return this.PlacaTracao.ObterPlacaFormatada(); }
        }

        public string TipoInteracaoInicioViagemFormatado
        {
            get { return this.TipoInteracaoInicioViagem.ObterDescricao(); }
        }

        public string TipoInteracaoChegadaViagemFormatado
        {
            get { return this.TipoInteracaoChegadaViagem.ObterDescricao(); }
        }

        public string TipoInteracaoFimEntregaFormatado
        {
            get { return this.TipoInteracaoFimEntrega.ObterDescricao(); }
        }

        public string SituacaoViagemFormatada
        {
            get { return this.SituacaoViagem.ObterDescricao(); }
        }

        #endregion
    }
}
