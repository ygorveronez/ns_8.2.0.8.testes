using Dominio.Entidades.Embarcador.Pedidos;
using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_DOCS", EntityName = "DocumentosCTE", Name = "Dominio.Entidades.DocumentosCTE", NameType = typeof(DocumentosCTE))]
    public class DocumentosCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NFC_NUMERO", TypeType = typeof(string), NotNull = false)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "NFC_SERIE", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "NFC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal ModeloDocumentoFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFC_DATAEMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volume", Column = "NFC_VOLUME", TypeType = typeof(int), NotNull = false)]
        public virtual int Volume { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "NFC_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "NFC_BCICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "NFC_VALORICMS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "NFC_BCICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMSST", Column = "NFC_VALORICMSST", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "NFC_VALORPRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "NFC_CFOP", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemPrincipal", Column = "NFC_ITEMPRINCIPAL", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ItemPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFE", Column = "NFC_CHAVENFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFE { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloNFe", Column = "NFC_PROTOCOLO_NFE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ProtocoloNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PINSuframa", Column = "NFC_PIN_SUFRAMA", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string PINSuframa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJRemetente", Column = "NFC_CNPJ_REMETENTE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string CNPJRemetente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RemetenteUF", Column = "NFC_UF_REMETENTE", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string RemetenteUF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DestinatarioUF", Column = "NFC_UF_DESTINATARIO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string DestinatarioUF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubSerie", Column = "NFC_SUB_SERIE", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string SubSerie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoCTe", Column = "NFC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoCTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NUMERO_ROMANEIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroRomaneio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NUMERO_PEDIDO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NUMERO_REFERENCIA_EDI", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroReferenciaEDI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NCM_PREDOMINANTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NCMPredominante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NUMERO_CONTROLE_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroControleCliente { get; set; }

        /// <summary>
        /// coluna criada erronamente, não usar
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscalEletronica", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscalEletronica XMLNotaFiscalEletronica { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO_CORRETA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscal XMLNotaFiscal { get; set; }

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
                if (!string.IsNullOrWhiteSpace(this.ChaveNFE) && ChaveNFE.Length >= 44)
                {
                    int serie = 0;

                    if (int.TryParse(this.ChaveNFE.Substring(22, 3), out serie))
                        return serie.ToString();
                    else
                        return this.ChaveNFE.Substring(22, 3);
                }
                else if (!string.IsNullOrWhiteSpace(this.Serie))
                {
                    return this.Serie;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual string NumeroOuNumeroDaChave
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ChaveNFE))
                {
                    int numero = 0;

                    if (int.TryParse(this.ChaveNFE.Substring(25, 9), out numero))
                        return numero.ToString();
                    else
                        return this.ChaveNFE.Substring(25, 9);
                }
                else if (!string.IsNullOrWhiteSpace(this.Numero))
                {
                    return this.Numero;
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

        public virtual int VolumeMaiorQueZero
        {
            get
            {
                return Volume > 0 ? Volume : 1;
            }
        }

        public virtual decimal ValorMaiorQueZero
        {
            get
            {
                return Valor > 0 ? Valor : 0.01m;
            }
        }

        public virtual string Descricao
        {
            get
            {
                return DescricaoCTe != null ? DescricaoCTe.ToUpper() : string.Empty;
            }
            set
            {
                DescricaoCTe = value != null ? value.ToUpper() : value;
            }
        }

        public virtual DocumentosCTE Clonar()
        {
            return (DocumentosCTE)this.MemberwiseClone();
        }
    }
}
