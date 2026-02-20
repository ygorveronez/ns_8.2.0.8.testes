using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioFaturamentoPorCTe
    {
        public DateTime DataInicialEmissao { get; set; }
        public DateTime DataFinalEmissao { get; set; }
        public DateTime DataInicialFatura { get; set; }
        public DateTime DataFinalFatura { get; set; }
        public DateTime DataInicialVencimentoFatura { get; set; }
        public DateTime DataFinalVencimentoFatura { get; set; }
        public int NumeroInicial { get; set; }
        public int NumeroFinal { get; set; }
        public int NumeroFatura { get; set; }
        public int NumeroTitulo { get; set; }
        public string NumeroBoleto { get; set; }
        public string NFe { get; set; }
        public SituacaoFatura? SituacaoFatura { get; set; }
        public List<StatusTitulo> StatusTitulo { get; set; }
        public List<string> StatusCTe { get; set; }
        public List<TipoServicoMultimodal> TipoServico { get; set; }
        public List<int> TipoTomador { get; set; }
        public double CpfCnpjTomador { get; set; }
        public int CodigoCarga { get; set; }
        public int CodigoGrupoPessoas { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public SituacaoCarga SituacaoCarga { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }
        public SituacaoFaturamentoCTe ?SituacaoFaturamentoCTe { get; set; }
        public TipoPropostaMultimodal ?TipoProposta { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa VeioPorImportacao { get; set; }
        public bool SomenteCTeSubstituido { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoViagem { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public List<Dominio.Enumeradores.TipoCTE> TiposCTe { get; set; }
        public DateTime DataInicialPrevisaoSaidaNavio { get; set; }
        public DateTime DataFinalPrevisaoSaidaNavio { get; set; }
    }
}
