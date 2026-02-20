using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_LEILAO", EntityName = "RegraAutorizacaoLeilao", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasLeilao.RegraAutorizacaoLeilao", NameType = typeof(RegraAutorizacaoLeilao))]
    public class RegraAutorizacaoLeilao : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTransportador", Column = "RAT_TRANSPORTADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValor", Column = "RAT_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCentroCarregamento", Column = "RAT_CENTRO_CARREGAMENTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorCentroCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_LEILAO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasLeilao.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_LEILAO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasLeilao.AlcadaValor", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValor> AlcadasValor { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTransportador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_LEILAO_TRANSPORTADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasLeilao.AlcadaTransportador", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTransportador> AlcadasTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasCentroCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_LEILAO_CENTRO_CARREGAMENTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasLeilao.AlcadaCentroCarregamento", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaCentroCarregamento> AlcadasCentroCarregamento { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRAS_AUTORIZACAO_LEILAO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorTransportador || RegraPorFilial || RegraPorValor || RegraPorCentroCarregamento;
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasValor?.Clear();
            AlcadasTransportador?.Clear();
            AlcadasCentroCarregamento?.Clear();
        }

        #endregion
    }
}
