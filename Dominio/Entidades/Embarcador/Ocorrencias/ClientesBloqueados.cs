namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CLIENTES_BLOQUEADOS", EntityName = "ClientesBloqueados", Name = "Dominio.Entidades.Embarcador.Ocorrencias.ClientesBloqueados", NameType = typeof(ClientesBloqueados))]
    public class ClientesBloqueados : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCliente", Column = "OCB_TIPO_CLIENTE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TomadorTipoOcorrencia), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoTomador TipoCliente { get; set; }
    }
}
