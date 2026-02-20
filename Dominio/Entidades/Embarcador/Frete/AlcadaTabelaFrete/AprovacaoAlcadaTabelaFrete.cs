using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_TABELA_FRETE_ALTERACAO", EntityName = "AprovacaoAlcadaTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.AlcadasTabelaFrete.AprovacaoAlcadaTabelaFrete", NameType = typeof(AprovacaoAlcadaTabelaFrete))]
    public class AprovacaoAlcadaTabelaFrete : RegraAutorizacao.AprovacaoAlcada<TabelaFreteAlteracao, RegraAutorizacaoTabelaFrete>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteAlteracao", Column = "TFA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override TabelaFreteAlteracao OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoTabelaFrete", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoTabelaFrete RegraAutorizacao { get; set; }

        #endregion
    }
}
