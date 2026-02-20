namespace Dominio.ObjetosDeValor.Embarcador.NotaFiscal
{
    public sealed class FiltroPesquisaRelatorioItemNaoConformidade
    {
        public string Descricao { get; set; }
        public bool? Situacao { get; set; }
        public Enumeradores.GrupoNC? Grupo { get; set; }
        public Enumeradores.SubGrupoNC? SubGrupo { get; set; }
        public Enumeradores.AreaNC? Area { get; set; }
        public bool? IrrelevanteParaNC { get; set; }
        public bool? PermiteContingencia { get; set; }
    }
}
