using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_FRIMESA", EntityName = "CargaFrimesa", Name = "Dominio.Entidades.CargaFrimesa", NameType = typeof(CargaFrimesa))]
    public class CargaFrimesa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CAF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RotaFrete", Column = "ROT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RotaFrete Rota { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoVeiculo", Column = "VTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoVeiculo TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarga", Column = "CAF_DATA_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "CAF_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTransportadora", Column = "CAF_TRANSPORTADORA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoTransportadora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoVeiculo", Column = "CAF_VEICULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRota", Column = "CAF_ROTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoTipo", Column = "CAF_TIPO_VEICULO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoTipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "FRF_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdicionalPeso", Column = "FRF_VALOR_ADICIONAL_PESO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdicionalPeso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "CAF_TIPO_DOCUMENTO", TypeType = typeof(Enumeradores.TipoDocumento), NotNull = false)]
        public virtual Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFretePlanilha", Column = "CAF_VALOR_FRETE_PLANILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ValorFretePlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoCarga", Column = "CAF_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_FRIMESA_DOCUMENTOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "CAF_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaFrimesaDocumentos", Column = "CFD_CODIGO")]
        public virtual IList<Dominio.Entidades.CargaFrimesaDocumentos> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosCTes", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(cte.CON_NUM AS NVARCHAR(20))
                                                                                    FROM T_CARGA_FRIMESA_DOCUMENTOS cargaFrimesaDocumentos
	                                                                                inner join T_CTE cte ON cte.CON_CODIGO = cargaFrimesaDocumentos.CON_CODIGO
                                                                                    WHERE cargaFrimesaDocumentos.CAF_CODIGO = CAF_CODIGO and cargaFrimesaDocumentos.CON_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumerosNFSes", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(nfse.NFSE_NUMERO AS NVARCHAR(20))
                                                                                    FROM T_CARGA_FRIMESA_DOCUMENTOS cargaFrimesaDocumentos
	                                                                                inner join T_NFSE nfse ON nfse.NFSE_CODIGO = cargaFrimesaDocumentos.NFSE_CODIGO
                                                                                    WHERE cargaFrimesaDocumentos.CAF_CODIGO = CAF_CODIGO and cargaFrimesaDocumentos.NFSE_CODIGO IS NOT NULL FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumerosNFSes { get; set; }

    }
}