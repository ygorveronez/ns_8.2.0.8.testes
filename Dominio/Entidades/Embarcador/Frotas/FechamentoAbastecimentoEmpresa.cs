namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FECHAMENTO_ABASTECIMENTO_EMPRESA", EntityName = "FechamentoAbastecimentoEmpresa", Name = "Dominio.Entidades.Embarcador.Frotas.FechamentoAbastecimentoEmpresa", NameType = typeof(FechamentoAbastecimentoEmpresa))]
    public class FechamentoAbastecimentoEmpresa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FAE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FechamentoAbastecimento", Column = "FAB_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FechamentoAbastecimento FechamentoAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        public virtual string Descricao 
        {
            get { return "Fechamento Abastecimento Empresa"; } 
        }
    }
}
