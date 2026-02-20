namespace Dominio.ObjetosDeValor.Embarcador.CIOT.BBC
{
    public class IntegrarPagamentoViagem
    {
        public int viagemExternoId { get; set; }

        public string tipo { get; set; }

        public string formaPagamento { get; set; }

        public string cpfCnpjContratado { get; set; }

        public string nomeContratado { get; set; }

        public string cpfMotorista { get; set; }

        public string nomeMotorista { get; set; }

        public decimal valor { get; set; }

        public string tipoBanco { get; set; }

        public int pagamentoExternoId { get; set; }

        public int? agencia { get; set; }

        public string conta { get; set; }

        public string tipoConta { get; set; }

        public int ibgeOrigem { get; set; }

        public int ibgeDestino { get; set; }

        public string chavePix { get; set; }

        public string hashValidacao { get; set; }
        public string filialId { get; set; }
    }
}
