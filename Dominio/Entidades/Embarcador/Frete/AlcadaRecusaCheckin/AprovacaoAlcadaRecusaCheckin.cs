using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AUTORIZACAO_ALCADA_RECUSA_CHECKIN", EntityName = "AprovacaoAlcadaRecusaCheckin", Name = "Dominio.Entidades.Embarcador.Frete.AlcadaRecusaCheckin.AprovacaoAlcadaRecusaCheckin", NameType = typeof(AprovacaoAlcadaRecusaCheckin))]
    public class AprovacaoAlcadaRecusaCheckin : RegraAutorizacao.AprovacaoAlcada<Cargas.CargaCTe, RegraAutorizacaoRecusaCheckin>
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "AAL_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaCTe", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override Cargas.CargaCTe OrigemAprovacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegraAutorizacaoRecusaCheckin", Column = "RAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public override RegraAutorizacaoRecusaCheckin RegraAutorizacao { get; set; }

        #endregion
    }
}
