using System.Collections.Generic;
using Dominio.Entidades.Embarcador.GestaoPatio.AlcadasToleranciaPesagem;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_TOLERANCIA_PESAGEM", EntityName = "RegrasAutorizacaoToleranciaPesagem", Name = "Dominio.Entidades.Embarcador.GestaoPatio.RegrasAutorizacaoToleranciaPesagem", NameType = typeof(RegrasAutorizacaoToleranciaPesagem))]
    public class RegrasAutorizacaoToleranciaPesagem : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_REGRA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRegraAutorizacaoToleranciaPesagem TipoRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorModeloVeicularCarga", Column = "RAT_MODELO_VEICULAR_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoCarga", Column = "RAT_TIPO_CARGA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TOLERANCIA_PESAGEM_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasToleranciaPesagem.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasModeloVeicularCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TOLERANCIA_PESAGEM_MODELO_VEICULAR_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasToleranciaPesagem.AlcadaModeloVeicularCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaModeloVeicularCarga> AlcadasModeloVeicularCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoCarga", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TOLERANCIA_PESAGEM_TIPO_CARGA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasToleranciaPesagem.AlcadaTipoCarga", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoCarga> AlcadasTipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_TOLERANCIA_PESAGEM_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasToleranciaPesagem.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_TOLERANCIA_PESAGEM_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (
                RegraPorFilial ||
                RegraPorModeloVeicularCarga ||
                RegraPorTipoCarga ||
                RegraPorTipoOperacao
            );
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasModeloVeicularCarga?.Clear();
            AlcadasTipoCarga?.Clear();
            AlcadasTipoOperacao?.Clear();
        }

        #endregion
    }
}
