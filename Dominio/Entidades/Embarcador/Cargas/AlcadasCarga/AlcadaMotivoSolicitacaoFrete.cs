namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ALCADA_CARGA_MOTIVO_SOLICITACAO_FRETE", EntityName = "AlcadasCarga.AlcadaMotivoSolicitacaoFrete", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AlcadaMotivoSolicitacaoFrete", NameType = typeof(AlcadaMotivoSolicitacaoFrete))]
    public class AlcadaMotivoSolicitacaoFrete : RegraAutorizacao.Alcada<RegraAutorizacaoCarga, MotivoSolicitacaoFrete>
    {
        #region Propriedades Sobrescritas

        public override string Descricao
        {
            get { return PropriedadeAlcada.Descricao; }
        }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoSolicitacaoFrete", Column = "MSF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override MotivoSolicitacaoFrete PropriedadeAlcada { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarga", Column = "RAT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarga RegrasAutorizacao { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override object ObterValorPropriedadeAlcada()
        {
            return PropriedadeAlcada.Codigo;
        }

        #endregion
    }
}
