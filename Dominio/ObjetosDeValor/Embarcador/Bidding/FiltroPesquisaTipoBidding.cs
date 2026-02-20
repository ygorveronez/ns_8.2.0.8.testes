using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Bidding
{
    public class FiltroPesquisaTipoBidding
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
        public int CodigoEmpresa { get; set; }
    }
}
