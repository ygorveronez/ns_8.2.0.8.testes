using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FAIXA_EMISSAO_CTE", EntityName = "FaixaEmissaoCTe", Name = "Dominio.Entidades.FaixaEmissaoCTe", NameType = typeof(FaixaEmissaoCTe))]
    public class FaixaEmissaoCTe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FEC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoEmissaoCTe", Column = "PEC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.PlanoEmissaoCTe Plano { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FEC_QUANTIDADE", TypeType = typeof(int), NotNull = true)]
        public virtual int Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "FEC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual Decimal Valor { get; set; }
    }
}
