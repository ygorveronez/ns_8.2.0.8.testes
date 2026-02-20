namespace Dominio.Entidades.Embarcador.Logistica.ConfiguracaoRotafrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_TRANSPORTADORES_HUB_OFERTAS", EntityName = "GrupoTransportadoresHUBOfertas", Name = "Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadoresHUBOfertas", NameType = typeof(GrupoTransportadoresHUBOfertas))]
    public class GrupoTransportadoresHUBOfertas : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoRotaFrete", Column = "CRF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoRotaFrete ConfiguracaoRotaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "GTH_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequenciaOferta", Column = "GTH_SEQUENCIA_OFERTA", TypeType = typeof(int), NotNull = false)]
        public virtual int SequenciaOferta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoDeixarDeOfertarAntesDoCarregamento", Column = "GTH_TEMPO_DEIXAR_DE_OFERTAR_ANTES_DO_CARREGAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoDeixarDeOfertarAntesDoCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TempoOfertarExclusivamenteParaGrupo", Column = "GTH_TEMPO_OFERTAR_EXCLUSIVAMENTE_PARA_GRUPO", TypeType = typeof(int), NotNull = false)]
        public virtual int TempoOfertarExclusivamenteParaGrupo { get; set; }

    }
}
