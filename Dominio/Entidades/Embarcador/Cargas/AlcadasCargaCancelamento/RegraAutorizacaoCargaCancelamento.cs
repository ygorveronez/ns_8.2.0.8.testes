using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_CARGA_CANCELAMENTO", EntityName = "RegraAutorizacaoCargaCancelamento", Name = "Dominio.Entidades.Embarcador.Cargas.AlcadasCargaCancelamento.RegraAutorizacaoCargaCancelamento", NameType = typeof(RegraAutorizacaoCargaCancelamento))]
    public class RegraAutorizacaoCargaCancelamento : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorValorFrete", Column = "RAT_VALOR_FRETE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_CANCELAMENTO_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCargaCancelamento.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_CANCELAMENTO_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCargaCancelamento.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorFrete", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CARGA_CANCELAMENTO_VALOR_FRETE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasCargaCancelamento.AlcadaValorFrete", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaValorFrete> AlcadasValorFrete { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_CARGA_CANCELAMENTO_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return (RegraPorFilial || RegraPorTipoOperacao || RegraPorValorFrete);
        }

        public override void LimparAlcadas()
        {
            AlcadasFilial?.Clear();
            AlcadasTipoOperacao?.Clear();
            AlcadasValorFrete?.Clear();
        }

        #endregion
    }
}
