using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CONTROLE_REAJUSTE_FRETE_PLANILHA", EntityName = "RegraControleReajusteFretePlanilha", Name = "Dominio.Entidades.Embarcador.Frete.RegraControleReajusteFretePlanilha", NameType = typeof(RegraControleReajusteFretePlanilha))]
    public class RegraControleReajusteFretePlanilha : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRP_POR_TIPO_OPERACAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTipoOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRP_POR_FILIAL", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFilial { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_APROVADORES_REGRA_CONTROLE_REAJUSTE_FRETE_PLANILHA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTipoOperacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_REAJUSTE_FRETE_TIPO_OPERACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaReajusteFreteTipoOperacao", Column = "ART_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteTipoOperacao> AlcadasTipoOperacao { get; set; }
        // --------------------------------------


        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFilial", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_REAJUSTE_FRETE_FILIAL")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRP_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaReajusteFreteFilial", Column = "ARF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Frete.AlcadaReajusteFreteFilial> AlcadasFilial { get; set; }
        // --------------------------------------
    }
}