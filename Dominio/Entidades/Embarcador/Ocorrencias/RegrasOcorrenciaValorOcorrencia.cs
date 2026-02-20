namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_OCORRENCIA_VALOR_OCORRENCIA", EntityName = "RegrasOcorrenciaValorOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaValorOcorrencia", NameType = typeof(RegrasOcorrenciaValorOcorrencia))]
    public class RegrasOcorrenciaValorOcorrencia : RegraAutorizacao.AlcadaSemPadronizacao
    {
        #region Propriedades Públicas

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RVO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoOcorrencia", Column = "RAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoOcorrencia RegrasAutorizacaoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RVO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RVO_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RVO_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "RVO_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 15, NotNull = true)]
        public virtual decimal Valor { get; set; }

        #endregion
        
        #region Métodos Públicos Sobrescritos

        public override bool IsCondicaoVerdadeira(object valor)
        {
            return IsCondicaoVerdadeiraValor(Valor, valor, (ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizao)Condicao);
        }

        public override bool IsJuncaoTodasVerdadeiras()
        {
            return Juncao == ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia.E;
        }

        #endregion
    }
}
