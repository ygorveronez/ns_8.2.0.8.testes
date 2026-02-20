namespace Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_SOLICITACAO_GAS_DATA", EntityName = "AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData", Name = "Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AlcadaSolicitacaoGasData", NameType = typeof(AlcadaSolicitacaoGasData))]
    public class AlcadaSolicitacaoGasData : RegraAutorizacao.Alcada<RegraAprovacaoSolicitacaoGas, int>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Property(0, Name = "PropriedadeAlcada", Column = "ALC_MINUTOS", TypeType = typeof(int), NotNull = true)]
        public override int PropriedadeAlcada { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAprovacaoSolicitacaoGas", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAprovacaoSolicitacaoGas RegrasAutorizacao { get; set; }

        public override string Descricao {
            get
            {
                return $"{PropriedadeAlcada}";
            }
        }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada;
        }

        #endregion
    }
}
