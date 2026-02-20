using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_BLOQUEIO_EMISSAO_POR_NAO_CONFORMIDADE", EntityName = "BloqueioEmissaoPorNaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.BloqueioEmissaoPorNaoConformidade", NameType = typeof(BloqueioEmissaoPorNaoConformidade))]
    public class BloqueioEmissaoPorNaoConformidade : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "BEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [Obsolete("Não utilizar, será removido")]
        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "BEC_DESCRICAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "BEC_SITUACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ItemNaoConformidade", Column = "INC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.ItemNaoConformidade TipoNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "TiposOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_BLOQUEIO_EMISSAO_POR_NAO_CONFORMIDADE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "BEC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TipoOperacao", Column = "TOP_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.TipoOperacao> TiposOperacao { get; set; }
    }
}
