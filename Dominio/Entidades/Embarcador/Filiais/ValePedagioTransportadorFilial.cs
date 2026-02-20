namespace Dominio.Entidades.Embarcador.Filiais
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_VALE_PEDAGIO_TRANSPORTADOR_FILIAL", EntityName = "ValePedagioTransportadorFilial", Name = "Dominio.Entidades.Embarcador.Filiais.ValePedagioTransportadorFilial", NameType = typeof(ValePedagioTransportadorFilial))]
    public class ValePedagioTransportadorFilial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "VPT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "VPT_COMPRAR_VALE_PEDAGIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ComprarValePedagio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "VPT_CODIGO_VALE_PEDAGIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracaoValePedagio { get; set; }
    }
}
