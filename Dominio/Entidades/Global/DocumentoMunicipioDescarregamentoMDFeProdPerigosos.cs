namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_MUNICIPIO_DESCARREGAMENTO_DOC_PROD_PERIGOSOS", EntityName = "DocumentoMunicipioDescarregamentoMDFeProdPerigosos", Name = "Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos", NameType = typeof(DocumentoMunicipioDescarregamentoMDFeProdPerigosos))]
    public class DocumentoMunicipioDescarregamentoMDFeProdPerigosos : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MPP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoMunicipioDescarregamentoMDFe", Column = "MDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoMunicipioDescarregamentoMDFe DocumentoMunicipioDescarregamentoMDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroONU", Column = "MPP_NUMERO_ONU", TypeType = typeof(string), Length = 4, NotNull = false)]
        public virtual string NumeroONU { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "MPP_NOME", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClasseRisco", Column = "MPP_CLASSE_RISCO", TypeType = typeof(string), Length = 40, NotNull = false)]
        public virtual string ClasseRisco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GrupoEmbalagem", Column = "MPP_GRUPO_EMBALAGEM", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string GrupoEmbalagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTotalProduto", Column = "MPP_QTD_TOTAL_PRODUTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string QuantidadeTotalProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeTipoVolumes", Column = "MPP_QTD_TIPO_VOLUMES", TypeType = typeof(string), Length = 60, NotNull = false)]
        public virtual string QuantidadeTipoVolumes { get; set; }
    }
}
