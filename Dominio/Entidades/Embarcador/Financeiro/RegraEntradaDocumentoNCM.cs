using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ENTRADA_DOCUMENTO_NCM", EntityName = "RegraEntradaDocumentoNCM", Name = "Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM", NameType = typeof(RegraEntradaDocumentoNCM))]
    public class RegraEntradaDocumentoNCM : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "REN_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraEntradaDocumento", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraEntradaDocumento RegraEntradaDocumento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NCM;
            }
        }

        public virtual bool Equals(RegraEntradaDocumentoNCM other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM Clonar()
        {
            return (Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM)this.MemberwiseClone();
        }
    }
}