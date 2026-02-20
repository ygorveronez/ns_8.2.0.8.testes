using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class MovimentoFinanceiroArquivoContabil
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string Documento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento TipoDocumentoMovimento { get; set; }
        public string Observacao { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoContaDebito { get; set; }
        public int CodigoContaCredito { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoFuncionario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoGeracaoMovimento TipoGeracaoMovimento { get; set; }
        public DateTime DataGeracao { get; set; }
        public int CodigoTitulo { get; set; }
        public DateTime DataBaseSistema { get; set; }
        public int CodigoGrupo { get; set; }
        public double CNPJPessoa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public string PlanoDeContaDebito { get; set; }
        public string PlanoDeContaDebitoContabil { get; set; }
        public string PlanoDeContaCredito { get; set; }
        public string PlanoDeContaCreditoContabil { get; set; }
        public double CNPJPessoaTitulo { get; set; }
        public string NomePessoa { get; set; }
        public string NumeroDocumento { get; set; }
        public string CodigoHistoricoMovimentoFinanceiro { get; set; }
    }
}
