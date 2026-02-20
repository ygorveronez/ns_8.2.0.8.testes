using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA_ITEM_ORDEM_SERVICO", EntityName = "DocumentoEntradaItemOrdemServico", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItemOrdemServico", NameType = typeof(DocumentoEntradaItemOrdemServico))]
    public class DocumentoEntradaItemOrdemServico : EntidadeBase, IEquatable<DocumentoEntradaItemOrdemServico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Frota.OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaItem", Column = "TDI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaItem DocumentoEntradaItem { get; set; }

        public virtual bool Equals(DocumentoEntradaItemOrdemServico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
