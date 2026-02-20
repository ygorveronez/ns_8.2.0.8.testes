using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CARGA_CANCELAMENTO", EntityName = "AprovacaoAlcadaCargaCancelamento", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.AprovacaoAlcadaCargaCancelamento", NameType = typeof(AprovacaoAlcadaCargaCancelamento))]
    public class AprovacaoAlcadaCargaCancelamento : RegraAutorizacao.AprovacaoAlcada<CargaCancelamentoSolicitacao, RegraAutorizacaoCargaCancelamento>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCancelamentoSolicitacao", Column = "CCS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override CargaCancelamentoSolicitacao OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCargaCancelamento", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCargaCancelamento RegraAutorizacao { get; set; }

        #endregion
    }
}
