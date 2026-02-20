using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RelatorioFrancesinha
    {
        public string NossoNumero { get; set; }
        public int NumeroDocumento { get; set; }
        public string NomePessoa { get; set; }
        public double CNPJPessoa { get; set; }
        public string BancoAgenciaConta { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal Valor { get; set; }
        public int Remessa { get; set; }

        public string FantasiaEmpresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string FoneEmpresa { get; set; }
        public string EnderecoEmpresa { get; set; }
        public string BairroEmpresa { get; set; }
        public string CEPEmpresa { get; set; }
        public string NumeroEnderecoEmpresa { get; set; }
        public string CidadeEmpresa { get; set; }
        public string EstadoEmpresa { get; set; }
    }
}