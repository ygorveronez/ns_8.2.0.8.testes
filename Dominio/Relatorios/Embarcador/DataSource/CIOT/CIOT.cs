using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.CIOT
{
    public class CIOT
    {
        public string NumeroCIOT { get; set; }
        public string ProtocoloAutorizacao { get; set; }
        public string DataEmissao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT Operadora { get; set; }
        public string NomeContratante { get; set; }
        public string CNPJContratante { get; set; }
        public string EnderecoContratante { get; set; }
        public string TelefoneContratante { get; set; }
        public string CidadeContratante { get; set; }
        public string NomeContratado { get; set; }
        public string RNTRCContratado { get; set; }
        public string RGContratado { get; set; }
        public string PISContratado { get; set; }
        public string CPFCNPJContratado { get; set; }
        public string EnderecoContratado { get; set; }
        public string TelefoneContratado { get; set; }
        public string Motorista { get; set; }
        public string CPFMotorista { get; set; }
        public string CNHMotorista { get; set; }
        public string RGMotorista { get; set; }
        public string PISMotorista { get; set; }
        public string EnderecoMotorista { get; set; }
        public string NumeroCartaoMotorista { get; set; }
        public string TelefoneMotorista { get; set; }
        public string PlacaVeiculo { get; set; }
        public string EstadoVeiculo { get; set; }
        public int AnoVeiculo { get; set; }
        public string Origem { get; set; }
        public string Destino { get; set; }
        public decimal ValorTotalFreteContratado { get; set; }
        public decimal INSS { get; set; }
        public decimal IR { get; set; }
        public decimal SEST { get; set; }
        public decimal SENAT { get; set; }
        public decimal Descontos { get; set; }
        public decimal Adiantamento { get; set; }
        public decimal Abastecimento { get; set; }
        public decimal Acrescimos { get; set; }

        public decimal Impostos
        {
            get
            {
                return INSS + IR + SEST + SENAT;
            }
        }

        public decimal Saldo { get; set; }

        public string DescricaoOperadora
        {
            get
            {
                return Operadora.ObterDescricao();

                //switch (this.Operadora)
                //{
                //    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.OperadoraCIOT.eFrete:
                //        return "IPC Administração LTDA";
                //    default:
                //        return string.Empty;
                //}
            }
        }
    }
}
