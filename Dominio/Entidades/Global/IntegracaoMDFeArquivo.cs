namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_MDFE_ARQUIVO", EntityName = "IntegracaoMDFeArquivo", Name = "Dominio.Entidades.IntegracaoMDFeArquivo", NameType = typeof(IntegracaoMDFeArquivo))]
    public class IntegracaoMDFeArquivo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IMA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "IMA_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = true)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaCarga", Column = "IMA_NUMERO_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "IMA_NUMERO_UNIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroDaUnidade { get; set; }
    }
}
