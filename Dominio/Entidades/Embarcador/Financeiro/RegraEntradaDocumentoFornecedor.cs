using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ENTRADA_DOCUMENTO_FORNECEDOR", EntityName = "RegraEntradaDocumentoFornecedor", Name = "Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor", NameType = typeof(RegraEntradaDocumentoFornecedor))]
    public class RegraEntradaDocumentoFornecedor : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraEntradaDocumento", Column = "RED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraEntradaDocumento RegraEntradaDocumento { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Pessoa?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(RegraEntradaDocumentoFornecedor other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor Clonar()
        {
            return (Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor)this.MemberwiseClone();
        }
    }
}