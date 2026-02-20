namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_AJUSTE_SIMULACAO_ITEM", EntityName = "AjusteTabelaFreteSimulacaoItem", Name = "Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItem", NameType = typeof(AjusteTabelaFreteSimulacaoItem))]
    public class AjusteTabelaFreteSimulacaoItem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AjusteTabelaFreteSimulacao", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual AjusteTabelaFreteSimulacao Simulacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente TabelaAjuste { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaTabelaFreteCliente", Column = "CTC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaTabelaFreteCliente TabelaCarga { get; set; }

        /// <summary>
        /// Valor do frete sem o valor dos componentes.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "TSI_VALOR_FRETE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        /// <summary>
        /// Valor do frete mais o valor dos componentes
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFreteTotal", Column = "TSI_VALOR_FRETE_TOTAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorFreteTotal { get; set; }

        /// <summary>
        /// Valor aproximado do ICMS (pode variar em ambiente real)
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "TSI_VALOR_ICMS", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorICMS { get; set; }        
    }
}
