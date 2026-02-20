namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_VALOR_FRETE_MOTIVO_REAJUSTE", EntityName = "RegrasMotivoReajuste", Name = "Dominio.Entidades.Embarcador.Frete.RegrasMotivoReajuste", NameType = typeof(RegrasMotivoReajuste))]
    public class RegrasMotivoReajuste : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RFM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoTabelaFrete", Column = "RAF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoTabelaFrete RegrasAutorizacaoTabelaFrete { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RFM_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RFM_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoValorFrete Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RFM_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoValorFrete Juncao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoReajuste", Column = "MRE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual MotivoReajuste MotivoReajuste { get; set; }

    }

}