namespace Dominio.Entidades.Embarcador.Cotacao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_COTACAO_VALOR_MERCADORIA", EntityName = "RegrasValorMercadoria", Name = "Dominio.Entidades.Embarcador.Cotacao.RegrasValorMercadoria", NameType = typeof(RegrasValorMercadoria))]
    public class RegrasValorMercadoria : RegraAutorizacao.AlcadaSemPadronizacao
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RVM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraCotacao", Column = "RCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegraCotacao RegrasCotacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RVM_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RVM_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RVM_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao Juncao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RVM_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal Valor { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsCondicaoVerdadeira(object valor)
        {
            return IsCondicaoVerdadeiraValor(Valor, valor, (ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao)Condicao);
        }

        public override bool IsJuncaoTodasVerdadeiras()
        {
            return Juncao == ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizao.E;
        }

        #endregion
    }
}