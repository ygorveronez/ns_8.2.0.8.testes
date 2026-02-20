namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_TRANSPORTADOR_INTEGRACAO", EntityName = "GrupoTransportadorIntegracao", Name = "Dominio.Entidades.Embarcador.Transportadores.GrupoTransportadorIntegracao", NameType = typeof(GrupoTransportadorIntegracao))]
    public class GrupoTransportadorIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GTI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoTransportador", Column = "GRT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoTransportador GrupoTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "GTI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao Tipo { get; set; }
    }
}
