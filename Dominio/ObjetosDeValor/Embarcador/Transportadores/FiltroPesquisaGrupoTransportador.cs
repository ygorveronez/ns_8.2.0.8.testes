using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Transportadores
{
    public sealed class FiltroPesquisaGrupoTransportador
    {
        public string Descricao { get; set; }
        public string CodigoIntegracao { get; set; }
        public SituacaoAtivoPesquisa Situacao { get; set; }
    }
}
