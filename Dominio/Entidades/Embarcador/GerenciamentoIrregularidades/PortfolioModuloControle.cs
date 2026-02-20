namespace Dominio.Entidades.Embarcador.GerenciamentoIrregularidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PORTFOLIO_MODULO_CONTROLE", EntityName = "PortfolioModuloControle", Name = "Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.PortfolioModuloControle", NameType = typeof(PortfolioModuloControle))]
    public class PortfolioModuloControle : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PMC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }        
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "PMC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}
