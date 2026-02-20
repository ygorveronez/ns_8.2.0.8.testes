using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.CTe
{
    public sealed class FiltroPesquisaRelatorioContainer
    {
        public DateTime DataEmissaoInicial { get; set; }
        public DateTime DataEmissaoFinal { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string NumeroControle { get; set; }
        public int NumeroCTe { get; set; }
        public int NumeroNota { get; set; }
        public int NumeroSerie { get; set; }
        public List<SituacaoCarga> SituacaoCarga { get; set; }
        public List<SituacaoCargaMercante> SituacoesCargaMercante { get; set; }
        public List<TipoPropostaMultimodal> TipoProposta { get; set; }
        public List<TipoModal> TipoModal { get; set; }
        public List<TipoServicoMultimodal> TipoServico { get; set; }
        public List<string> SituacaoCTe { get; set; }
        public int CodigoPortoOrigem { get; set; }
        public int CodigoPortoDestino { get; set; }
        public int CodigoViagem { get; set; }
        public int CodigoContainer { get; set; }
        public int CodigoTerminalOrigem { get; set; }
        public int CodigoTerminalDestino { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public int CodigoTipoOperacao { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa VeioPorImportacao { get; set; }
        public bool SomenteCTeSubstituido { get; set; }
        public List<Dominio.Enumeradores.TipoCTE> TiposCTe { get; set; }
        public int CodigoViagemTransbordo { get; set; }
        public int CodigoPortoTransbordo { get; set; }
        public int CodigoBalsa { get; set; }
    }
}
