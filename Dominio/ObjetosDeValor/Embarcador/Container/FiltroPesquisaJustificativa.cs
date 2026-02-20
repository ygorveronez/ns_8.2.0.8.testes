using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Container
{
    public class FiltroPesquisaJustificativa
    {
        public string Descricao { get; set; }
        public SituacaoAtivoPesquisa Ativo { get; set; }
        public StatusColetaContainer? StatusContainer { get; set; }
    }
}