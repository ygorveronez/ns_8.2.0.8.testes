using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class AgendamentoEntregaPedido
    {
        #region Propriedades 
        public int Codigo { get; set; }

        public string Carga { get; set; }
        public string TipoOperacao { get; set; }
        public string Cliente { get; set; }
        public string Destino { get; set; }
        public string Transportador { get; set; }
        public string ObservacaoReagendamento { get; set; }
        public string ContatoCliente { get; set; }
        public string ContatoTransportador { get; set; }
        public string NotasFiscais { get; set; }


        public int QtdVolumes { get; set; }
        public decimal QtdMetrosCubicos { get; set; }

        private SituacaoAgendamentoEntregaPedido SituacaoAgendamentoEntregaPedido { get; set; }
        private SituacaoCarga SituacaoCarga { get; set; }

        private DateTime DataSugestaoEntrega { get; set; }
        private DateTime DataCarregamentoInicial { get; set; }
        private DateTime DataCarregamentoFinal { get; set; }
        private DateTime DataAgendamento { get; set; }
        private DateTime DataPrevisaoEntrega { get; set; }
        private DateTime DataCriacaoPedido { get; set; }
        #endregion

        #region Propriedades com Regras
        public string DataSugestaoEntregaFormatada
        {
            get { return FormataData(DataSugestaoEntrega); }
        }
        public string DataCarregamentoInicialFormatada
        {
            get { return FormataData(DataCarregamentoInicial); }
        }
        public string DataCarregamentoFinalFormatada
        {
            get { return FormataData(DataCarregamentoFinal); }
        }
        public string DataAgendamentoFormatada
        {
            get { return FormataData(DataAgendamento); }
        }
        public string DataPrevisaoEntregaFormatada
        {
            get { return FormataData(DataPrevisaoEntrega); }
        }
        public string DataCriacaoPedidoFormatada
        {
            get { return FormataData(DataCriacaoPedido); }
        }

        public string SituacaoCargaFormatada
        {
            get { return SituacaoCarga.ObterDescricao(); }
        }
        public string SituacaoAgendamentoEntregaPedidoFormatada
        {
            get { return SituacaoAgendamentoEntregaPedido.ObterDescricao(); }
        }
        #endregion

        #region Métdos Privados
        private string FormataData(DateTime dataParametro)
        {
            return dataParametro != DateTime.MinValue ? dataParametro.ToString("dd/MM/yyyy HH:mm") : string.Empty;
        }
        #endregion

    }
}