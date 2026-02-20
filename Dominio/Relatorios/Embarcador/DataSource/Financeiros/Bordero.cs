using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class Bordero
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public decimal ValorACobrar { get; set; }
        public decimal ValorTotalAcrescimo { get; set; }
        public decimal ValorTotalDesconto { get; set; }
        public decimal ValorTotalACobrar { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public string ValorPorExtenso { get; set; }
        public bool ImprimirObservacao { get; set; }
        public string Observacao { get; set; }
        public string NomeEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string TelefoneEmpresa { get; set; }
        public string NomeTomador { get; set; }
        public string CNPJTomador { get; set; }
        public string IETomador { get; set; }
        public string EnderecoTomador { get; set; }
        public string CidadeTomador { get; set; }
        public string CEPTomador { get; set; }
        public string TelefoneTomador { get; set; }
        public string Agencia { get; set; }
        public string Banco { get; set; }
        public string NumeroConta { get; set; }
        public string TipoConta { get; set; }
    }
}
