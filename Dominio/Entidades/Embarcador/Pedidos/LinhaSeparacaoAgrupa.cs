namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LINHA_SEPARACAO_AGRUPA", EntityName = "LinhaSeparacaoAgrupa", Name = "Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacaoAgrupa", NameType = typeof(LinhaSeparacaoAgrupa))]
    public class LinhaSeparacaoAgrupa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CLA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LinhaSeparacao", Column = "CLS_CODIGO_UM", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacaoUm { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LinhaSeparacao", Column = "CLS_CODIGO_DOIS", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.LinhaSeparacao LinhaSeparacaoDois { get; set; }
    }
}