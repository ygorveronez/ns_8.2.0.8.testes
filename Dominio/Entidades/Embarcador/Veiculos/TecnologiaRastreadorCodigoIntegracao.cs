namespace Dominio.Entidades.Embarcador.Veiculos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TECNOLOGIA_RASTREADOR_CODIGO_INTEGRACAO", EntityName = "TecnologiaRastreadorCodigoIntegracao", Name = "Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreadorCodigoIntegracao", NameType = typeof(TecnologiaRastreadorCodigoIntegracao))]
    public class TecnologiaRastreadorCodigoIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TRT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TRT_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TecnologiaRastreador", Column = "TRA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Veiculos.TecnologiaRastreador TecnologiaRastreador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }
    }
}
