using System;

namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_PNEU_RETORNO_REFORMA_PRODUTO", EntityName = "Frota.PneuRetornoReformaProduto", Name = "Dominio.Entidades.Embarcador.Frota.PneuRetornoReformaProduto", NameType = typeof(PneuRetornoReformaProduto))]
    public class PneuRetornoReformaProduto : EntidadeBase, IEquatable<PneuRetornoReformaProduto>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "PRP_Quantidade", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PRP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FinalidadeProdutoOrdemServico", Column = "FPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FinalidadeProdutoOrdemServico FinalidadeProduto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Frota.PneuRetornoReforma", Column = "PRR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PneuRetornoReforma PneuRetornoReforma { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        public virtual bool Equals(PneuRetornoReformaProduto other)
        {
            return (other.Codigo == this.Codigo);
        }
    }
}
