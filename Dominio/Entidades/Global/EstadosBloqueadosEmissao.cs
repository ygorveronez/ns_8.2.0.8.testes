namespace Dominio.Entidades
{

    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EMPRESA_CONFIG_ESTADOS_BLOQUEADOS_EMISSAO", EntityName = "EstadosBloqueadosEmissao", Name = "Dominio.Entidades.EstadosBloqueadosEmissao", NameType = typeof(EstadosBloqueadosEmissao))]
    public class EstadosBloqueadosEmissao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EBE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoEmpresa", Column = "COF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConfiguracaoEmpresa ConfiguracaoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }
    }
}
