using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_CARGA", EntityName = "AprovacaoAlcadaCarga", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCarga.AprovacaoAlcadaCarga", NameType = typeof(AprovacaoAlcadaCarga))]
    public class AprovacaoAlcadaCarga : RegraAutorizacao.AprovacaoAlcada<Carga, RegraAutorizacaoCarga>
    {
        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Carga OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoCarga", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoCarga RegraAutorizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAA_GUID_CARGA", TypeType = typeof(string), Length = 5000, NotNull = false)]
        public virtual string GuidCarga { get; set; }

        #endregion

        #region Métodos Sobrescritos

        public override bool IsPermitirAprovacaoOuReprovacao(int codigoUsuario)
        {
            return (OrigemAprovacao.SituacaoCarga != SituacaoCarga.Cancelada) && (OrigemAprovacao.SituacaoCarga != SituacaoCarga.Anulada) && base.IsPermitirAprovacaoOuReprovacao(codigoUsuario);
        }

        #endregion
    }
}
