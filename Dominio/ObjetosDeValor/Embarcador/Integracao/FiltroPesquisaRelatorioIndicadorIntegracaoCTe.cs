using Dominio.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public sealed class FiltroPesquisaRelatorioIndicadorIntegracaoCTe
    {
        public string CodigoCargaEmbarcador { get; set; }

        public int CodigoFilial { get; set; }

        public int CodigoIntegradora1 { get; set; }

        public int CodigoIntegradora2 { get; set; }

        public int CodigoIntegradora3 { get; set; }

        public int CodigoIntegradora4 { get; set; }

        public int CodigoIntegradora5 { get; set; }

        public int CodigoTransportador { get; set; }

        public DateTime? DataEmissaoInicio { get; set; }

        public DateTime? DataEmissaoLimite { get; set; }

        public string DescricaoIntegradora1 { get; set; }

        public string DescricaoIntegradora2 { get; set; }

        public string DescricaoIntegradora3 { get; set; }

        public string DescricaoIntegradora4 { get; set; }

        public string DescricaoIntegradora5 { get; set; }

        public int NumeroCTe { get; set; }

        public OpcaoSimNaoPesquisa Integrado1 { get; set; } = OpcaoSimNaoPesquisa.Todos;

        public OpcaoSimNaoPesquisa Integrado2 { get; set; } = OpcaoSimNaoPesquisa.Todos;

        public OpcaoSimNaoPesquisa Integrado3 { get; set; } = OpcaoSimNaoPesquisa.Todos;

        public OpcaoSimNaoPesquisa Integrado4 { get; set; } = OpcaoSimNaoPesquisa.Todos;

        public OpcaoSimNaoPesquisa Integrado5 { get; set; } = OpcaoSimNaoPesquisa.Todos;
    }
}
