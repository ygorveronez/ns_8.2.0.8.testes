using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class RemessaPagamento
    {
        public string Empresa { get; set; }
        public string CNPJEmpresa { get; set; }
        public string NumeroBanco { get; set; }
        public string DigitoBanco { get; set; }
        public string Banco { get; set; }
        public string NumeroAgencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroConta { get; set; }
        public string DigitoConta { get; set; }
        public string NumeroConvenio { get; set; }
        public int CodigoTitulo { get; set; }
        public int Parcela { get; set; }
        public string NumeroBoleto { get; set; }
        public string NumeroDocumento { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataVencimento { get; set; }
        public string Observacao { get; set; }
        public string Operador { get; set; }
        public DateTime DataGeracao { get; set; }
        public DateTime DataPagamento { get; set; }
        public FinalidadePagamentoEletronico Finalidade { get; set; }
        public ModalidadePagamentoEletronico Modalidade { get; set; }
        public int Numero { get; set; }
        public int QtdTitulos { get; set; }
        public TipoContaPagamentoEletronico TipoConta { get; set; }
        public decimal ValorTotal { get; set; }
        public string Fornecedor { get; set; }
        public string CNPJFornecedor { get; set; }

        public string DescricaoDataVencimento
        {
            get
            {
                if (DataVencimento != DateTime.MinValue)
                    return DataVencimento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataGeracao
        {
            get
            {
                if (DataGeracao != DateTime.MinValue)
                    return DataGeracao.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoDataPagamento
        {
            get
            {
                if (DataPagamento != DateTime.MinValue)
                    return DataPagamento.ToString("dd/MM/yyyy");
                else
                    return string.Empty;
            }
        }

        public string DescricaoFinalidade
        {
            get { return Finalidade.ObterDescricao(); }
        }

        public string DescricaoModalidade
        {
            get { return Modalidade.ObterDescricao(); }
        }

        public string DescricaoTipoConta
        {
            get { return TipoConta.ObterDescricao(); }
        }
    }
}
