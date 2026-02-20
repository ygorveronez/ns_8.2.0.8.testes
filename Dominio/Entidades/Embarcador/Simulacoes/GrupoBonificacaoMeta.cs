namespace Dominio.Entidades.Embarcador.Simulacoes
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_BONIFICACAO_META", EntityName = "GrupoBonificacaoMeta", Name = "Dominio.Entidades.Embarcador.Simulacoes.GrupoBonificacaoMeta", NameType = typeof(GrupoBonificacaoMeta))]
    public class GrupoBonificacaoMeta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GBM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoBonificacao", Column = "GRB_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Simulacoes.GrupoBonificacao GrupoBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Regiao", Column = "REG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Localidades.Regiao Regiao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasIdaPrevista", Column = "GBM_QTD_CARGAS_IDA_PREVISTA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargasIdaPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasIdaRealizada", Column = "GBM_QTD_CARGAS_IDA_REALIZADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargasIdaRealizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasRetornoPrevista", Column = "GBM_QTD_CARGAS_RETORNO_PREVISTA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargasRetornoPrevista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeCargasRetornoRealizada", Column = "GBM_QTD_CARGAS_RETORNO_REALIZADA", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeCargasRetornoRealizada { get; set; }
    }
}
