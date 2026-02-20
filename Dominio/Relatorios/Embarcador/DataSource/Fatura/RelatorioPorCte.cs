using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Fatura
{
    public sealed class RelatorioPorCte
    {
        public int Codigo { get; set; }
        public int Numero { get; set; }
        public int NumeroCte { get; set; }
        public int SerieCte { get; set; }
        public decimal ValorLiquido { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public string NomePessoa { get; set; }
        public string EnderecoPessoa { get; set; }
        public string BairroPessoa { get; set; }
        public string CEPPessoa { get; set; }
        public string NumeroEnderecoPessoa { get; set; }
        public string CidadePessoa { get; set; }
        public string EstadoPessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string IEPessoa { get; set; }
        public string ObservacaoFatura { get; set; }
        public bool ImprimirObservacaoFatura { get; set; }
        public string RazaoEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string IEEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
        public string ComplementoEmpresa { get; set; }
        public string NomeBanco { get; set; }
        public int NumeroBanco { get; set; }
        public string AgenciaBanco { get; set; }
        public string DigitoAgenciaBanco { get; set; }
        public string NumeroContaBanco { get; set; }
        public int TipoContaBanco { get; set; }
        public long NumeroPreFatura { get; set; }
        public DateTime DataEmissao { get; set; }
        public decimal Peso { get; set; }
        public string Notas { get; set; }
        public decimal ValorSemICMS { get; set; }
        public decimal ICMS { get; set; }

        public decimal TotalCobrado
        {
            get { return ValorSemICMS + ICMS; }
        }
    }
}
