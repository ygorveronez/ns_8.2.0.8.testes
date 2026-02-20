using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Pedidos
{
    public class OrdemColetaExclusiva
    {
        public string Remetente { get; set; }
        public string CidadeRemetente { get; set; }

        public string Destinatario { get; set; }
        public string CidadeDestino { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }
        private DateTime DataAgendamento { get; set; }
        private DateTime DataColeta { get; set; }

        public int CodigoMotorista { get; set; }
        public string Motorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public string RGMotorista { get; set; }
        public string CPFMotorista { get; set; }
        public string CNHMotorista { get; set; }
        public string NumeroRegistroCNHMotorista { get; set; }

        public string Veiculo { get; set; }
        public int QuantidadeEixoVeiculo { get; set; }
        public string Reboque { get; set; }
        public int QuantidadeEixoReboque { get; set; }

        public string Observacao { get; set; }

        #region Propriedades com Regras

        public string DataAgendamentoFormatada
        {
            get { return DataAgendamento != DateTime.MinValue ? DataAgendamento.ToDateTimeString() : string.Empty; }
        }

        public string DataColetaFormatada
        {
            get { return DataColeta != DateTime.MinValue ? DataColeta.ToDateTimeString() : string.Empty; }
        }

        #endregion
    }
}
