using System;


namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_DOCS", EntityName = "DocumentosPreCTE", Name = "Dominio.Entidades.DocumentosPreCTE", NameType = typeof(DocumentosPreCTE))]
    public class DocumentosPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PNF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PNF_NUMERO", TypeType = typeof(string), NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "PNF_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PNF_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "PNF_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "PNF_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PNF_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "PNF_BCICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "PNF_VALORICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "PNF_BCICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "PNF_VALORICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "PNF_VALORPRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "PNF_CFOP", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemPrincipal", Column = "PNF_ITEMPRINCIPAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ItemPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFE", Column = "PNF_CHAVENFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PINSuframa", Column = "PNF_PIN_SUFRAMA", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string PINSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJRemetente", Column = "PNF_CNPJ_REMETENTE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string CNPJRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubSerie", Column = "PNF_SUB_SERIE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string SubSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PNF_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCMPredominante", Column = "PNF_NCM_PREDOMINANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NCMPredominante { get; set; }

        public virtual string NumeroModelo
        {
            get
            {
                if (this.ModeloDocumentoFiscal != null)
                    return this.ModeloDocumentoFiscal.Numero;
                else
                    return string.Empty;
            }
        }

        public virtual string SerieOuSerieDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Serie))
                {
                    return this.Serie;
                }
                else if (!string.IsNullOrWhiteSpace(this.ChaveNFE))
                {
                    int serie = 0;

                    if (int.TryParse(this.ChaveNFE.Substring(23, 2), out serie))
                        return serie.ToString();
                    else
                        return this.ChaveNFE.Substring(23, 2);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual string IncidenciaDeICMS
        {
            get
            {
                return this.ValorICMS > 0 || this.ValorICMSST > 0 ? "S" : "N";
            }
        }
    }
}
