namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SELECAO_FUNCIONARIO", EntityName = "SelecaoFuncionario", Name = "Dominio.Entidades.Embarcador.WMS.SelecaoFuncionario", NameType = typeof(SelecaoFuncionario))]
    public class SelecaoFuncionario : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Selecao", Column = "SEL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Selecao Selecao { get; set; }

        public virtual string Descricao { get { return Codigo.ToString(); } }
    }
}
