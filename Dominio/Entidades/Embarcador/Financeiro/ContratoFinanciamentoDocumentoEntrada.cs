using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONTRATO_FINANCIAMENTO_DOCUMENTO_ENTRADA", EntityName = "ContratoFinanciamentoDocumentoEntrada", Name = "Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada", NameType = typeof(ContratoFinanciamentoDocumentoEntrada))]
    public class ContratoFinanciamentoDocumentoEntrada : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Financeiro.ContratoFinanciamentoDocumentoEntrada>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CDE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntradaTMS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContratoFinanciamento", Column = "CFI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContratoFinanciamento ContratoFinanciamento { get; set; }

        public virtual bool Equals(ContratoFinanciamentoDocumentoEntrada other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

    }
}
