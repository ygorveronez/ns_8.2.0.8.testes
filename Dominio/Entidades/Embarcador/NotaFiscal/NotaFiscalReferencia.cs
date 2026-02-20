using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_REFERENCIA", EntityName = "NotaFiscalReferencia", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia", NameType = typeof(NotaFiscalReferencia))]
    public class NotaFiscalReferencia : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalReferencia>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "NFR_TIPO_DOCUMENTO", TypeType = typeof(Dominio.Enumeradores.TipoDocumentoReferenciaNFe), NotNull = false)]
        public virtual Dominio.Enumeradores.TipoDocumentoReferenciaNFe TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "NFR_CHAVE", TypeType = typeof(string), Length = 44, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UF", Column = "NFR_UF", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFR_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJEmitente", Column = "NFR_CNPJ_EMITENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NFR_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NFR_NUMERO", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CPFEmitente", Column = "NFR_CPF_EMITENTE", TypeType = typeof(string), Length = 11, NotNull = false)]
        public virtual string CPFEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Modelo", Column = "NFR_MODELO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string Modelo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IEEmitente", Column = "NFR_IE_EMITENTE", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string IEEmitente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroECF", Column = "NFR_NUMERO_ECF", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string NumeroECF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "COO", Column = "NFR_COO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string COO { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        public virtual bool Equals(NotaFiscalReferencia other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case Enumeradores.TipoDocumentoReferenciaNFe.NF:
                        return "NF-e/NFC-e";
                    case Enumeradores.TipoDocumentoReferenciaNFe.NFModelo1:
                        return "NF Modelo 1";
                    case Enumeradores.TipoDocumentoReferenciaNFe.NFProdutorRural:
                        return "NF Produtor Rural";
                    case Enumeradores.TipoDocumentoReferenciaNFe.CTe:
                        return "CT-e";
                    case Enumeradores.TipoDocumentoReferenciaNFe.CupomFiscal:
                        return "Cupom Fiscal";
                    default:
                        return "";
                }
            }
        }
    }
}
