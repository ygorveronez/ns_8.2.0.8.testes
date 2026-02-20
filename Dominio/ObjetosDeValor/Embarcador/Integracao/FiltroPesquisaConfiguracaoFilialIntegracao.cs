using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class FiltroPesquisaConfiguracaoFilialIntegracao
    {
        public int CodigoTipoOperacao { get; set; }

        public int CodigoFilial { get; set; }

        public SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
