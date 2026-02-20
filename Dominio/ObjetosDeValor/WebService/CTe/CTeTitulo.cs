using System;

namespace Dominio.ObjetosDeValor.WebService.CTe
{
    public class CTeTitulo
    {
        public int Codigo { get; set; }
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public int Sequencia { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo StatusTitulo { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo TipoTitulo { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal ValorPendente { get; set; }
        public decimal ValorTituloOriginal { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa GrupoPessoas { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Pessoa { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }
        public string TipoDocumentoTituloOriginal { get; set; }
        public string NumeroDocumentoTituloOriginal { get; set; }
        public Dominio.ObjetosDeValor.Empresa Empresa { get; set; }
        public DateTime? DataLancamento { get; set; }
        public string Usuario { get; set; }
        public string NossoNumero { get; set; }
        public DateTime? DataProgramacaoPagamento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo FormaTitulo { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo BoletoStatusTitulo { get; set; }
        public string BoletoConfiguracao { get; set; }
        public bool BoletoEnviadoPorEmail { get; set; }
        public bool BoletoGeradoAutomaticamente { get; set; }
        public bool EnviarDocumentacaoFaturamentoCTe { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }
        public decimal ValorOriginalMoedaEstrangeira { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public DateTime? DataBaseCRT { get; set; }
        public bool GerarFaturamentoAVista { get; set; }
    }
}
