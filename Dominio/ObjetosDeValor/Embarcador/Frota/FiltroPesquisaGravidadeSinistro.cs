using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Frota
{
    public sealed class FiltroPesquisaGravidadeSinistro
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Status { get; set; }
    }
}
