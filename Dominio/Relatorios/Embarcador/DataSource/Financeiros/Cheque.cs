using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public sealed class Cheque
    {
        private string _cnpjEmpresa;
        public string CnpjEmpresa
        {
            get { return _cnpjEmpresa.ObterCnpjFormatado(); }
            set { _cnpjEmpresa = value; }
        }

        public double CpfCnpjPessoa { get; set; }

        public string CpfCnpjPessoaFormatado
        {
            get { return CpfCnpjPessoa > 0 ? CpfCnpjPessoa.ToString().ObterCpfOuCnpjFormatado(TipoPessoa) : ""; }
        }

        public int Codigo { get; set; }

        public DateTime DataCadastro { get; set; }

        public string DataCadastroFormatada
        {
            get { return DataCadastro.ToString("dd/MM/yyyy HH:mm"); }
        }

        public DateTime DataCompensacao { get; set; }

        public string DataCompensacaoFormatada
        {
            get { return DataCompensacao != DateTime.MinValue ? DataCompensacao.ToString("dd/MM/yyyy") : ""; }
        }

        public DateTime DataTransacao { get; set; }

        public string DataTransacaoFormatada
        {
            get { return DataTransacao.ToString("dd/MM/yyyy"); }
        }

        public DateTime DataVencimento { get; set; }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento.ToString("dd/MM/yyyy"); }
        }

        public string DescricaoBanco { get; set; }

        public string DigitoAgencia { get; set; }

        public string NomePessoa { get; set; }

        public string NumeroAgencia { get; set; }

        public string NumeroCheque { get; set; }

        public string NumeroConta { get; set; }

        public string NumeroTitulo { get; set; }

        public string Observacao { get; set; }

        public string RazaoSocialEmpresa { get; set; }

        public StatusCheque Status { get; set; }

        public string StatusDescricao
        {
            get { return Status.ObterDescricao(); }
        }

        public TipoCheque Tipo { get; set; }

        public string TipoDescricao
        {
            get { return Tipo.ObterDescricao(); }
        }

        public string TipoPessoa { get; set; }

        public decimal Valor { get; set; }
    }
}
