using System;

namespace Dominio.Entidades.Embarcador.Fatura
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURA_CARGA_DOCUMENTO_EXCLUIDO", EntityName = "FaturaCargaDocumentoExcluido", Name = "Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumentoExcluido", NameType = typeof(FaturaCargaDocumentoExcluido))]
    public class FaturaCargaDocumentoExcluido : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Fatura.FaturaCargaDocumento>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FCE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturaCarga", Column = "FAC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Fatura.FaturaCarga FaturaCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico ConhecimentoDeTransporteEletronico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumentoFatura", Column = "FCE_TIPO_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura TipoDocumentoFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroFatura", Column = "FCE_NUMERO_FATURA_VINCULADA", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroFatura { get; set; }

        public virtual string Descricao
        {
            get
            {
                switch (this.TipoDocumentoFatura)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento:
                        return this.ConhecimentoDeTransporteEletronico.Descricao;
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.NotaFiscal:
                        return this.NFSe.Descricao;
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (this.TipoDocumentoFatura)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.Conhecimento:
                        return "Conhecimento";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoFatura.NotaFiscal:
                        return "Nota de Serviço";
                    default:
                        return "";
                }
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
