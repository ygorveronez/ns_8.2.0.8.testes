using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_CARGA_DOCUMENTO", EntityName = "FaturaCargaDocumento", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento", NameType = typeof(FaturaCargaDocumento))]
    public class FaturaCargaDocumento : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Utilizar a propriedade Fatura.")]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaCarga", Column = "FAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaCarga FaturaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoFatura", Column = "FCD_TIPO_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura TipoDocumentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFatura", Column = "FCD_NUMERO_FATURA_VINCULADA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusDocumentoFatura", Column = "FCD_STATUS_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusDocumentoFatura StatusDocumentoFatura { get; set; }

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (this.TipoDocumentoFatura)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento:
                        return "CT-e";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.NotaFiscal:
                        return "NFS-e";
                    default:
                        return "";
                }
            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.ConhecimentoDeTransporteEletronico.Descricao;
            }
        }

        public virtual bool Equals(FaturaCargaDocumento other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
