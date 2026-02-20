using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Ocorrencia
{
    public sealed class FiltroPesquisaGrupoTipoDeOcorrenciaDeCTe
    {
        public SituacaoAtivaPesquisa SituacaoAtivo { get; set; }
        public string CodigoIntegracao { get; set; }
        public string Descricao { get; set; }
    }
}
