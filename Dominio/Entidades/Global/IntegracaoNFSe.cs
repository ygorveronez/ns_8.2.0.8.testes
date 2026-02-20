namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_NFSE", EntityName = "IntegracaoNFSe", Name = "Dominio.Entidades.IntegracaoNFSe", NameType = typeof(IntegracaoNFSe))]
    public class IntegracaoNFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "INF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "INF_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Arquivo", Column = "INF_ARQUIVO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string Arquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "INF_STATUS", TypeType = typeof(Enumeradores.StatusIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusIntegracao Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArquivo", Column = "INF_TIPO_ARQUIVO", TypeType = typeof(Enumeradores.TipoArquivoIntegracao), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoArquivoIntegracao TipoArquivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "INF_TIPO", TypeType = typeof(Enumeradores.TipoIntegracaoNFSe), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoIntegracaoNFSe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaCarga", Column = "INF_NUMERO_CARGA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDaCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDaUnidade", Column = "INF_NUMERO_UNIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroDaUnidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTipoOperacao", Column = "INF_CODIGO_TIPO_OPERACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Romaneio", Column = "INF_ROMANEIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Romaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "INF_TIPO_VEICULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCalculo", Column = "INF_TIPO_CALCULO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoCalculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDespesa", Column = "INF_VALOR_DESPESA", TypeType = typeof(decimal), NotNull = false, Scale = 2, Precision = 18)]
        public virtual decimal ValorDespesa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GerouCargaEmbarcador", Column = "INF_GEROU_CARGA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GerouCargaEmbarcador { get; set; }
    }
}
