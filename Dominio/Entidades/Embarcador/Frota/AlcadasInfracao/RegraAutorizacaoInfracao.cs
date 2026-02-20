using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frota.AlcadasInfracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_INFRACAO", EntityName = "RegraAutorizacaoInfracao", Name = "Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao", NameType = typeof(RegraAutorizacaoInfracao))]
    public class RegraAutorizacaoInfracao : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoInfracao", Column = "RAT_TIPO_INFRACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoInfracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoInfracao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_INFRACAO_TIPO_INFRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasInfracao.AlcadaTipoInfracao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoInfracao> AlcadasTipoInfracao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_INFRACAO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasInfracao.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_INFRACAO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorTipoInfracao || RegraPorValor;
        }

        public override void LimparAlcadas()
        {
            AlcadasTipoInfracao?.Clear();
            AlcadasValor?.Clear();
        }

        #endregion
    }
}
