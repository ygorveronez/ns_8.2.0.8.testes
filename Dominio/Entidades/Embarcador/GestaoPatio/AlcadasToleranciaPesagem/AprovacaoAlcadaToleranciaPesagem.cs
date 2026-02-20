using System;

namespace Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_TOLERANCIA_PESAGEM", EntityName = "AprovacaoAlcadaToleranciaPesagem", Name = "Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem.AprovacaoAlcadaToleranciaPesagem", NameType = typeof(AprovacaoAlcadaToleranciaPesagem))]
    public class AprovacaoAlcadaToleranciaPesagem : RegraAutorizacao.AprovacaoAlcada<Cargas.CargaJanelaCarregamentoGuarita, RegrasAutorizacaoToleranciaPesagem>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaJanelaCarregamentoGuarita", Column = "JCG_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaJanelaCarregamentoGuarita OrigemAprovacao { get; set; }

        [Obsolete("Não utilizar. O campo foi descontinuado", true)]
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoToleranciaPesagem", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegrasAutorizacaoToleranciaPesagem RegraAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_GUID_CARGA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string GuidCarga { get; set; }

        #endregion Propriedades Sobrescritas

        #region Métodos Sobrescritos

        public override bool IsPermitirAprovacaoOuReprovacao(int codigoUsuario)
        {
            return (OrigemAprovacao.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada) && (OrigemAprovacao.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada) && base.IsPermitirAprovacaoOuReprovacao(codigoUsuario);
        }

        #endregion Métodos Sobrescritos
    }
}
