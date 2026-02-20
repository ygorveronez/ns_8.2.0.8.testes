using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class ContratoTransporteRodoviario
    {
        public DateTime DataEmissao { get; set; }

        public string NumeroCIOT { get; set; }

        public int NumeroViagem { get; set; }

        public string NumeroContrato { get; set; }

        public string NomeContratante { get; set; }

        public string CNPJContratante { get; set; }

        public string EnderecoContratante { get; set; }

        public string NumeroContratante { get; set; }

        public string CidadeContratante { get; set; }

        public string UFContratante { get; set; }

        public string TelefoneContratante { get; set; }

        public string NomeTransportador { get; set; }

        public int RNTRCTransportador { get; set; }

        public string RNTRCTransportadorFormatado
        {
            get
            {
                return string.Format("{0:00000000}", this.RNTRCTransportador);
            }
        }

        public double CPFCNPJTransportador { get; set; }

        public string TipoTransportador { get; set; }

        public virtual string CPFCNPJTransportadorFormatado
        {
            get
            {
                return this.TipoTransportador.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJTransportador) : String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJTransportador);
            }
        }

        public string EnderecoTransportador { get; set; }

        public string NumeroTransportador { get; set; }

        public string CidadeTransportador { get; set; }

        public string UFTransportador { get; set; }

        public string TelefoneTransportador { get; set; }

        public string NomeMotorista { get; set; }

        public string CPFMotorista { get; set; }

        public string RGMotorista { get; set; }

        public string DataNascimentoMotorista { get; set; }

        public string PISMotorista { get; set; }

        public string CNHMotorista { get; set; }

        public string EnderecoMotorista { get; set; }

        public string CidadeMotorista { get; set; }

        public string UFMotorista { get; set; }

        public string TelefoneMotorista { get; set; }

        public string NumeroCartaoMotorista { get; set; }

        public string PlacaVeiculo { get; set; }

        public string UFVeiculo { get; set; }

        public string DescricaoUFVeiculo { get; set; }

        public int AnoVeiculo { get; set; }

        public string CidadeOrigem { get; set; }

        public string UFOrigem { get; set; }

        public string CidadeDestino { get; set; }

        public string UFDestino { get; set; }

        public string CodigoNaturezaCarga { get; set; }

        public decimal ValorAdiantamento { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal ValorSENAT { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorOperacao { get; set; }

        public decimal ValorQuitacao { get; set; }

        public decimal ValorLiquido{ get; set; }

        public decimal ValorBruto { get; set; }

        public Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT TipoIntegradora { get; set; }
    }
}
