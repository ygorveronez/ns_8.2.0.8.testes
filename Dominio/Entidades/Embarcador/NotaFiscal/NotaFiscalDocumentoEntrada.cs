using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_DOCUMENTO_ENTRADA", EntityName = "NotaFiscalDocumentoEntrada", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada", NameType = typeof(NotaFiscalDocumentoEntrada))]
    public class NotaFiscalDocumentoEntrada : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalDocumentoEntrada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS DocumentoEntrada { get; set; }

        public virtual bool Equals(NotaFiscalDocumentoEntrada other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
