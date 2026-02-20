namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_STAGE_AGRUPAMENTO_COMPONENTE_FRETE", EntityName = "StageAgrupamentoComponenteFrete", Name = "Dominio.Entidades.Embarcador.Pedidos.StageAgrupamentoComponenteFrete", NameType = typeof(StageAgrupamentoComponenteFrete))]

    public class StageAgrupamentoComponenteFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "StageAgrupamento", Column = "STG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento StageAgrupamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComponente", Column = "SAC_VALOR_COMPONENTE", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal ValorComponente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Percentual", Column = "SAC_PERCENTUAL_NOTA_FISCAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = true)]
        public virtual decimal Percentual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoComponenteFrete", Column = "SAC_TIPO_COMPONENTE_FRETE", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete TipoComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.ComponenteFrete ComponenteFrete { get; set; }

        //novas

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoValor", Column = "SAC_TIPO_VALOR", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarValorTotalAReceber", Column = "SAC_DESCONTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AcrescentaValorTotalAReceber", Column = "SAC_ACRESCENTAR_VALOR_TOTAL_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AcrescentaValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalAReceber", Column = "SAC_NAO_SOMAR_VALOR_TOTA_A_RECEBER", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalAReceber { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NaoSomarValorTotalPrestacao", Column = "SAC_NAO_SOMAR_VALOR_TOTAL_PRESTACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool NaoSomarValorTotalPrestacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirBaseCalculoICMS", Column = "SAC_INCLUIR_BC_ICMS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirBaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IncluirIntegralmenteContratoFreteTerceiro", Column = "SAC_INCLUIR_INTEGRALMENTE_CONTRATO_FRETE_TERCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IncluirIntegralmenteContratoFreteTerceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescontarComponenteFreteLiquido", Column = "SAC_DESCONTAR_COMPONENTE_FRETE_LIQUIDO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool DescontarComponenteFreteLiquido { get; set; }


    }
}
