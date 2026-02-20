using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaRelatorioTransportador
    {
        public int Localidade { get; set; }
        public string Estado { get; set; }
        public bool CertificadosVencidos { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public DateTime PrazoValidade { get; set; }
        public bool? LiberacaoParaPagamentoAutomatico { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa OptanteSimplesNacional { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa EmiteEmbarcador { get; set; }
        public Dominio.Enumeradores.OpcaoSimNaoPesquisa ConfiguracaoNFSe { get; set; }
        public bool? Bloqueado { get; set; }
        public DateTime DataInicioVencimentoCertificado { get; set; }
        public DateTime DataFinalVencimentoCertificado { get; set; }
        public DateTime? DataCriacaoInicial { get; set; }
        public DateTime? DataCriacaoFinal { get; set; }
        public DateTime? DataAlteracaoInicial { get; set; }
        public DateTime? DataAlteracaoFinal { get; set; }
    }
}
