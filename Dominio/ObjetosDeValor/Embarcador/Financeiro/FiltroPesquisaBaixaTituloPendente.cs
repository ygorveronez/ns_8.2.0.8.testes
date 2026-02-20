using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaBaixaTituloPendente
    {
        public DateTime DataProgramacaoPagamentoInicial { get; set; }
        public DateTime DataProgramacaoPagamentoFinal { get; set; }
        public DateTime DataAutorizacaoInicial { get; set; }
        public DateTime DataAutorizacaoFinal { get; set; }
        public int NumeroTitulo { get; set; }
        public DateTime DataVencimentoInicial { get; set; }
        public DateTime DataVencimentoFinal { get; set; }
        public decimal ValorInicial { get; set; }
        public decimal ValorFinal { get; set; }
        public Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao SomenteTitulosDeNegociacao { get; set; }
        public string NumeroPedido { get; set; }
        public string NumeroOcorrencia { get; set; }
        public int CodigoFatura { get; set; }
        public int CodigoConhecimento { get; set; }
        public int CodigoCarga { get; set; }
        public TipoTitulo TipoTitulo { get; set; }
        public double CnpjPessoa { get; set; }
        public int CodigoEmpresa { get; set; }
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware TipoServico { get; set; }
        public int CodigoBaixa { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public FormaTitulo FormaTitulo { get; set; }
        public string NumeroDocumento { get; set; }
        public SituacaoBoletoTitulo? SituacaoBoletoTitulo { get; set; }
        public SituacaoPagamentoEletronico? SituacaoPagamentoEletronico { get; set; }
        public int CodigoPagamentoEletronico { get; set; }
        public int CodigoNaturezaOperacaoEntrada { get; set; }
        public decimal ValorTitulo { get; set; }
        public int CodigoBanco { get; set; }
        public string RaizCnpjPessoa { get; set; }
        public double CnpjCpfPortador { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public List<TipoDocumentoPesquisaTitulo> TiposDocumento { get; set; }
    }
}
