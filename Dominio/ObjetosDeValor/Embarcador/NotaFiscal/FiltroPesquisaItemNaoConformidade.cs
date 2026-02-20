using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public class FiltroPesquisaItemNaoConformidade
    {
        public string Descricao { get; set; }
        public bool? Status { get; set; }
        public GrupoNC? Grupo { get; set; }
        public SubGrupoNC? SubGrupo { get; set; }
        public AreaNC? Area { get; set; }
        public bool? IrrelevanteParaNC { get; set; }
        public bool? PermiteContingencia { get; set; }
    }
}

