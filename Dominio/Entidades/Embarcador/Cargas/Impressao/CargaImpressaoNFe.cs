namespace Dominio.Entidades.Embarcador.Cargas.Impressao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_IMPRESSAO_NFE", EntityName = "CargaImpressaoNFe", Name = "Dominio.Entidades.Embarcador.Cargas.CargaImpressaoNFe", NameType = typeof(CargaImpressaoNFe))]
    public class CargaImpressaoNFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Carga Carga { get; set; }        

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML", Column = "NF_XML", Type = "StringClob", NotNull = true)]
        public virtual string XML { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoImpressao", Column = "CIN_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao SituacaoImpressao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "CIN_CHAVE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Chave { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "CIN_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "CIN_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPDF", Column = "CIN_CAMINHO_PDF", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string CaminhoPDF { get; set; }
    }
}
