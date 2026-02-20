using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_PROVISAO_PENDENTE", EntityName = "RegraAutorizacaoProvisaoPendente", Name = "Dominio.Entidades.Embarcador.Escrituracao.AlcadasProvisaoPendente.RegraAutorizacaoProvisaoPendente", NameType = typeof(RegraAutorizacaoProvisaoPendente))]
    public class RegraAutorizacaoProvisaoPendente : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorProvisao", Column = "RAT_VALOR_PROVISAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoGeracaoRegraProvisao), NotNull = false)]
        public virtual TipoGeracaoRegraProvisao TipoGeracaoRegraProvisao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PROVISAO_PENDENTE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasProvisaoPendente.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadaValorProvisao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_PROVISAO_PENDENTE_VALOR_PROVISAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasProvisaoPendente.AlcadaValorProvisao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorProvisao> AlcadaValorProvisao { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_PROVISAO_PENDENTE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (RegraPorFilial || RegraPorValorProvisao);
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadaValorProvisao?.Clear();
        }

        #endregion
    }
}