namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_OFERTA_INTEGRACAO", EntityName = "CargaOfertaIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.CargaOfertaIntegracao", NameType = typeof(CargaOfertaIntegracao))]
    public class CargaOfertaIntegracao : Integracao.Integracao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "COI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "COI_TIPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoOfertaCarga Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOferta", Column = "CAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaOferta CargaOferta { get; set; }
    }
}