using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pessoas
{
    public sealed class FiltroPesquisaRelatorioPessoa
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TipoPessoaCadastro TipoPessoa { get; set; }
        public List<TipoModalidade> ModalidadePessoa { get; set; }
        public List<string> Estado { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public int CodigoLocalidade { get; set; }
        public int CodigoAtividade { get; set; }
        public bool? Situacao { get; set; }
        public bool? SomenteSemCodigoIntegracao { get; set; }
        public bool? ExibeSomenteComCodigoIntegracao { get; set; }
        public OpcaoSimNaoPesquisa Bloqueado { get; set; }
        public OpcaoSimNaoPesquisa AguardandoConferenciaInformacao { get; set; }
        public OpcaoSimNaoPesquisa ComGeolocalizacao { get; set; }
        public bool? SomenteSemContaContabil { get; set; }
        public int CodigoCategoria { get; set; }
        public int CodigoEmpresa { get; set; }
        public string Vendedor { get; set; }
        //public DateTime DataIntegracaoInicial { get; set; }
        //public DateTime DataIntegracaoFinal { get; set; }
    }
}
