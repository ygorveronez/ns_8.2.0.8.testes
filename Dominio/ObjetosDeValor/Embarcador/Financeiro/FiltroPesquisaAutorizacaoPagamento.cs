using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaAutorizacaoPagamento
    {
        public int CodigoPagamentoEletronico { get; set; }
        public double Fornecedor { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public int NumeroTitulo { get; set; }
        public string NumeroDocumento { get; set; }
        public SituacaoAutorizacao SituacaoAutorizacao { get; set; }
        public TipoTituloNegociacao TipoTituloNegociacao { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public List<TipoDocumentoPesquisaTitulo> TiposDocumento { get; set; }
        public SituacaoBoletoTitulo SituacaoBoletoTitulo { get; set; }
    }
}
