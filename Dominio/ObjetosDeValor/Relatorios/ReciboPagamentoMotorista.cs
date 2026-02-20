using System;

namespace Dominio.ObjetosDeValor.Relatorios
{
    public class ReciboPagamentoMotorista
    {
        public byte[] Logomarca { get; set; }

        public string NomeEmpresa { get; set; }

        public int Numero { get; set; }

        public string CNPJEmpresa { get; set; }

        public string IEEmpresa { get; set; }

        public string EnderecoEmpresa { get; set; }

        public string BairroEmpresa { get; set; }

        public string NumeroEmpresa { get; set; }

        public string ComplementoEmpresa { get; set; }

        public string CEPEmpresa { get; set; }

        public string CidadeEmpresa { get; set; }

        public string UFEmpresa { get; set; }

        public string FoneEmpresa { get; set; }

        public DateTime DataEmissaoCTe { get; set; }

        public string PlacaVeiculo { get; set; }

        public string UFVeiculo { get; set; }

        public string DescricaoUFVeiculo { get; set; }

        public string NumeroCTe { get; set; }

        public string SerieCTe { get; set; }

        public string NomeProprietarioVeiculo { get; set; }

        public string CPFCNPJProprietarioVeiculo { get; set; }

        public string IERGProprietarioVeiculo { get; set; }

        public string EnderecoProprietarioVeiculo { get; set; }

        public string BairroProprietarioVeiculo { get; set; }

        public string CidadeProprietarioVeiculo { get; set; }

        public string CEPProprietarioVeiculo { get; set; }

        public string UFProprietarioVeiculo { get; set; }

        public string NomeMotorista { get; set; }

        public string CPFMotorista { get; set; }

        public string RGMotorista { get; set; }

        public string DataNascimentoMotorista { get; set; }

        public string CNHMotorista { get; set; }

        public DateTime? NascimentoMotorista { get; set; }

        public string EnderecoMotorista { get; set; }

        public string CEPMotorista { get; set; }

        public string CidadeMotorista { get; set; }

        public string UFMotorista { get; set; }

        public string PisPasep { get; set; }

        public string PisPasepProprietario { get; set; }

        public string CidadeInicioPrestacao { get; set; }

        public string UFInicioPrestacao { get; set; }

        public string CidadeTerminoPrestacao { get; set; }

        public string UFTerminoPrestacao { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorINSS { get; set; }

        public decimal ValorIR { get; set; }

        public decimal ValorSESTSENAT { get; set; }

        public decimal ValorAdiantamento { get; set; }

        public decimal SaldoAPagar { get; set; }

        public decimal ValorLiquido { get; set; }

        public string SaldoAPagarDescricao { get; set; }

        public string ValorLiquidoDescricao { get; set; }

        public string DescricaoDocumento { get; set; }

        public string AbreviaturaDescricaoDocumento { get; set; }

        public string Observacao { get; set; }

        public decimal ValorOutros { get; set; }

        public string Status { get; set; }

        public decimal ValorIRRF { get; set; }

        public decimal ValorSEST { get; set; }

        public decimal ValorSENAT { get; set; }

        public decimal ValorSeguro { get; set; }

        public decimal ValorPedagio { get; set; }

        public decimal ValorOperacao { get; set; }

        public decimal ValorQuitacao { get; set; }

        public string ValorOperacaoDescricao { get; set; }

        public string ValorQuitacaoDescricao { get; set; }

        public decimal SalarioMotorista { get; set; }
    }
}
