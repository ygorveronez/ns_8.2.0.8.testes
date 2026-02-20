namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PRE_CTE_COMP_PREST", EntityName = "ComponentePrestacaoPreCTE", Name = "Dominio.Entidades.ComponentePrestacaoPreCTE", NameType = typeof(ComponentePrestacaoPreCTE))]
    public class ComponentePrestacaoPreCTE : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PreConhecimentoDeTransporteEletronico", Column = "PCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PreConhecimentoDeTransporteEletronico PreCTE { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PCP_NOME", TypeType = typeof(string), Length = 15, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "PCP_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNaBaseDeCalculoDoICMS", Column = "PCP_INCLUI_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNaBaseDeCalculoDoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluiNoTotalAReceber", Column = "PCP_INCLUI_TOTAL_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluiNoTotalAReceber { get; set; }
    }
}
