namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PERFIL_TRANSPORTADOR_FORMULARIO_PERMISSAO_PERSONALIZADA", EntityName = "PerfilTransportadorFormularioPermissaoPersonalizada", Name = "Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormularioPermissaoPersonalizada", NameType = typeof(PerfilTransportadorFormularioPermissaoPersonalizada))]
    public class PerfilTransportadorFormularioPermissaoPersonalizada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "PerfilTransportadorFormulario", Column = "PTF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Transportadores.PerfilTransportadorFormulario PerfilTransportadorFormulario { get; set; }

        [NHibernate.Mapping.Attributes.Property(Name = "CodigoPermissao", Column = "PPS_CODIGO_PERMISSAO", TypeType = typeof(int), NotNull = true)]
        public virtual int CodigoPermissao { get; set; }

        public virtual string Descricao
        {
            get { return PerfilTransportadorFormulario?.Descricao ?? string.Empty + CodigoPermissao; }
        }
    }
}
