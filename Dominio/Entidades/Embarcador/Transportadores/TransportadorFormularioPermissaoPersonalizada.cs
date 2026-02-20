namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_FORMULARIO_PERMISSAO_PERSONALIZADA", EntityName = "TransportadorFormularioPermissaoPersonalizada", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorFormularioPermissaoPersonalizada", NameType = typeof(TransportadorFormularioPermissaoPersonalizada))]
    public class TransportadorFormularioPermissaoPersonalizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "TransportadorFormulario", Column = "EFM_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Transportadores.TransportadorFormulario TransportadorFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Column = "PPP_CODIGO_PERMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoPermissao { get; set; }
    }
}
