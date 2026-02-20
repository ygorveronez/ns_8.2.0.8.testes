using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class FiltroPesquisaTipoBaseline
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
    }
}
