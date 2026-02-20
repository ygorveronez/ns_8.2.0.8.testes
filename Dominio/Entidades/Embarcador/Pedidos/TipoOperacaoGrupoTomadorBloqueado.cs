namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_OPERACAO_GRUPO_TOMADOR_BLOQUEADO", EntityName = "TipoOperacaoGrupoTomadorBloqueado", Name = "Dominio.Entidades.Embarcador.Pedidos.TipoOperacaoGrupoTomadorBloqueado", NameType = typeof(TipoOperacaoGrupoTomadorBloqueado))]
    public class TipoOperacaoGrupoTomadorBloqueado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TGB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoOperacao", Column = "TOP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoOperacao TipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pessoas.GrupoPessoas GrupoPessoas { get; set; }
    }
}
