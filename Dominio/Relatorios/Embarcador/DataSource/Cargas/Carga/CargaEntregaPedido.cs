using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga
{
    public class CargaEntregaPedido
    {
        #region Propriedades
        public int Codigo { get; set; }
        public string Filial { get; set; }
        public string GradeCarga { get; set; }
        private double CNPJCliente { get; set; }
        private string TipoCliente { get; set; }
        public string NomeCliente { get; set; }
        private DateTime DataInicioCarregamento { get; set; }
        public string NumeroCarga { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroEXP { get; set; }
        public string NumeroRedespacho { get; set; }
        private DateTime PrevisaoEntrega { get; set; }
        private DateTime DataChegadaCliente { get; set; }
        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus MonitoramentoStatus { get; set; }
        private bool EntregaRealizadaNoPrazo { get; set; }
        private DateTime DataAgendamento { get; set; }

        #endregion

        #region PropriedadesComRegras
        public string DataInicioCarregamentoFormatada
        {
            get { return DataInicioCarregamento != DateTime.MinValue ? DataInicioCarregamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataAgendamentoFormatada
        {
            get { return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }
        public string PrevisaoEntregaFormatada
        {
            get { return PrevisaoEntrega != DateTime.MinValue ? PrevisaoEntrega.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string DataChegadaClienteFormatada
        {
            get { return DataChegadaCliente != DateTime.MinValue ? DataChegadaCliente.ToString("dd/MM/yyyy HH:mm") : string.Empty; }
        }

        public string CNPJClienteFormatado
        {
            get { return this.CNPJCliente > 0 ? this.CNPJCliente.ToString().ObterCpfOuCnpjFormatado(this.TipoCliente) : ""; }
        }

        public string MonitoramentoStatusDescricao
        {
            get { return Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(this.MonitoramentoStatus); }
        }

        public string EntregaRealizadaNoPrazoDescricao
        {
            get { return this.EntregaRealizadaNoPrazo == false ? "Não" : "Sim" ?? ""; }
        }

        #endregion
    }
}
