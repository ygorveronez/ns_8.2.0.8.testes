using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public sealed class FiltroPesquisaRelatorioNotasDetalhadas
    {
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int OperadorLancamentoEntrada { get; set; }
        public int OperadorFinalizaEntrada { get; set; }
        public int Serie { get; set; }
        public List<int> CodigosNaturezaOperacao { get; set; }
        public int CodigoProduto { get; set; }
        public int CodigoServico { get; set; }
        public int CodigoModelo { get; set; }
        public int CodigoEmpresaFilial { get; set; }
        public int CodigoVeiculo { get; set; }
        public List<int> CodigosGrupoProduto { get; set; }
        public int CodigoSegmento { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public List<int> CodigosModeloDocumentoFiscal { get; set; }
        public List<int> CodigosTipoMovimento { get; set; }
        public SituacaoDocumentoEntrada StatusNotaEntrada { get; set; }
        public StatusTitulo SituacaoFinanceiraNotaEntrada { get; set; }
        public double CnpjPessoa { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public DateTime DataEntradaInicial { get; set; }
        public DateTime DataEntradaFinal { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public TipoEntradaSaida TipoMovimento { get; set; }
        public string Chave { get; set; }
        public string EstadoEmitente { get; set; }
        public int CodigoEmpresa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public string NumeroModeloNF { get; set; }
        public int CodigoEquipamento { get; set; }
        public bool NotasComDiferencaDeValorTabelaFornecedor { get; set; }

        public DateTime DataFinalizacaoInicial { get; set; }
        public DateTime DataFinalizacaoFinal { get; set; }
    }
}
