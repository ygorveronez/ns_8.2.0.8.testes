using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRAS_AUTORIZACAO_NAO_CONFORMIDADE", EntityName = "RegraAutorizacaoNaoConformidade", Name = "Dominio.Entidades.Embarcador.NotaFiscal.AlcadasNaoConformidade.RegraAutorizacaoNaoConformidade", NameType = typeof(RegraAutorizacaoNaoConformidade))]
    public class RegraAutorizacaoNaoConformidade : RegraAutorizacao.RegraAutorizacao
    {
        #region Propriedades

        [NHibernate.Mapping.Attributes.Property(0, Column = "RAT_TIPO_APROVADOR_REGRA", TypeType = typeof(TipoAprovadorRegra), NotNull = false)]
        public virtual TipoAprovadorRegra TipoAprovadorRegra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorFilial", Column = "RAT_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorTipoOperacao", Column = "RAT_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorCFOP", Column = "RAT_CFOP", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorCFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RegraPorNaoConformidade", Column = "RAT_NAO_CONFORMIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorNaoConformidade{ get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Grupo", Column = "RAT_GRUPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.GrupoNC), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.GrupoNC Grupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SubGrupo", Column = "RAT_SUBGRUPO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SubGrupoNC), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SubGrupoNC SubGrupo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "RAT_AREA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.AreaNC), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.AreaNC Area { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_NAO_CONFORMIDADE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasNaoConformidade.AlcadaFilial", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaFilial> AlcadasFilial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasCFOP", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_NAO_CONFORMIDADE_CFOP")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasNaoConformidade.AlcadaCFOP", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaCFOP> AlcadasCFOP { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasNaoConformidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_NAO_CONFORMIDADE_NAO_CONFORMIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasNaoConformidade.AlcadaNaoConformidade", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaNaoConformidade> AlcadasNaoConformidade { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_NAO_CONFORMIDADE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasNaoConformidade.AlcadaTipoOperacao", Column = "ALC_CODIGO")]
        public virtual IList<AlcadaTipoOperacao> AlcadasTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Setores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NAO_CONFORMIDADE_SETORES")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Setor", Column = "SET_CODIGO")]
        public virtual ICollection<Setor> Setores { get; set; }

        #endregion

        #region Propriedades Sobrescritas

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_NAO_CONFORMIDADE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RAT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public override ICollection<Usuario> Aprovadores { get; set; }

        #endregion

        #region Métodos Públicos Sobrescritos

        public override bool IsAlcadaAtiva()
        {
            return RegraPorFilial || RegraPorTipoOperacao || RegraPorCFOP || RegraPorNaoConformidade;
        }

        public override void LimparAprovadores()
        {
            Aprovadores?.Clear();
            Setores?.Clear();
        }

        public override void LimparAlcadas()
        {
            AlcadasTipoOperacao?.Clear();
            AlcadasNaoConformidade?.Clear();
            AlcadasCFOP?.Clear();
            AlcadasFilial?.Clear();
        }

        #endregion
    }
}
