using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_NAO_CONFORMIDADE", EntityName = "AprovacaoAlcadaNaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.AprovacaoAlcadaNaoConformidade", NameType = typeof(AprovacaoAlcadaNaoConformidade))]
    public class AprovacaoAlcadaNaoConformidade : RegraAutorizacao.AprovacaoAlcada<NaoConformidade, RegraAutorizacaoNaoConformidade>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaoConformidade", Column = "NCF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override NaoConformidade OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoNaoConformidade", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoNaoConformidade RegraAutorizacao { get; set; }

        #endregion
    }
}
