namespace Dominio.Entidades.Embarcador.Contabeis
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CODIGO_INTEGRACAO_CFOP_CST", EntityName = "CodigoIntegracaoCFOPCST", Name = "Dominio.Entidades.Embarcador.Contabeis.CodigoIntegracaoCFOPCST", NameType = typeof(CodigoIntegracaoCFOPCST))]
    public class CodigoIntegracaoCFOPCST : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CCC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "CCC_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CST", Column = "CCC_CST", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "CCC_CFOP", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CFOP { get; set; }

        public virtual string Descricao
        {
            get { return CodigoIntegracao + " - " + CST + " - " + CFOP; }
        }
    }
}
