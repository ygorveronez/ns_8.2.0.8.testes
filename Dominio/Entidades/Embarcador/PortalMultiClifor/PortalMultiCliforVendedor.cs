namespace Dominio.Entidades.Embarcador.PortalMultiClifor
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PORTAL_MULTI_CLIFOR_VENDEDOR", EntityName = "PortalMultiCliforVendedor", Name = "Dominio.Entidades.Embarcador.PortalMultiClifor.PortalMultiCliforVendedor", NameType = typeof(PortalMultiCliforVendedor))]
    public class PortalMultiCliforVendedor : EntidadeBase
    {
        
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMCV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Vendedor", Column = "PMCV_VENDEDOR", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Vendedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioAcessoPortal", Column = "PMCV_USUARIO_ACESSO_PORTAL", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioAcessoPortal { get; set; }

    }
}
