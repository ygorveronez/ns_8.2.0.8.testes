using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaDocumentoEntrada
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataEntradaInicial { get; set; }
        public DateTime DataEntradaFinal { get; set; }
        public decimal ValorInicial { get; set; }
        public decimal ValorFinal { get; set; }
        public int NumeroTitulo { get; set; }
        public double CpfCnpjFornecedor { get; set; }
        public int NumeroLancamentoInicial { get; set; }
        public int NumeroLancamentoFinal { get; set; }
        public Int64 NumeroDocumentoInicial { get; set; }
        public Int64 NumeroDocumentoFinal { get; set; }
        public int CodigoNaturezaOperacao { get; set; }
        public int CodigoCFOP { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoVeiculo { get; set; }
        public int CodigoDestinatario { get; set; }
        public SituacaoDocumentoEntrada? Situacao { get; set; }
        public List<int> ModelosDocumento { get; set; }
        public int TipoDocumentoEmEBS { get; set; }
        public string Chave { get; set; }
        public StatusFinanceiroDocumentoEntrada? StatusFinanceiro { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoStatusLancamento { get; set; }
        public int NumeroFogo { get; set; }
    }
}
