using System;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NOTA_FISCAL_OBSERVACAO_FISCAL", EntityName = "NotaFiscalObservacaoFiscal", Name = "Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal", NameType = typeof(NotaFiscalObservacaoFiscal))]
    public class NotaFiscalObservacaoFiscal : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NOF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscal", Column = "NFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscal NotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ObservacaoFiscal", Column = "OBF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ObservacaoFiscal ObservacaoFiscal { get; set; }

        public virtual bool Equals(NotaFiscalObservacaoFiscal other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal Clonar()
        {
            return (Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscalObservacaoFiscal)this.MemberwiseClone();
        }
    }
}
