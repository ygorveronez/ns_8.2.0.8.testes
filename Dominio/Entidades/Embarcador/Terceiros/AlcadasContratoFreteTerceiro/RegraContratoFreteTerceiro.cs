using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Terceiros
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_CONTRATO_FRETE_TERCEIRO", EntityName = "RegraContratoFreteTerceiro", Name = "Dominio.Entidades.Embarcador.Terceiros.RegraContratoFreteTerceiro", NameType = typeof(RegraContratoFreteTerceiro))]
    public class RegraContratoFreteTerceiro : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCT_POR_VALOR_CONTRATO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCT_POR_VALOR_ACRESCIMO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorAcrescimo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCT_POR_VALOR_DESCONTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValorDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RCT_POR_TERCEIROS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorTerceiros { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_APROVADORES_REGRA_CONTRATO_FRETE_TERCEIRO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorContrato", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaContratoFreteTerceiroValorContrato", Column = "ARV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorContrato> AlcadasValorContrato { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorAcrescimo", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_ACRESCIMO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaContratoFreteTerceiroValorAcrescimo", Column = "ARA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> AlcadasValorAcrescimo { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValorDesconto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaContratoFreteTerceiroValorDesconto", Column = "ARD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorDesconto> AlcadasValorDesconto { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasTerceiros", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_CONTRATO_FRETE_TERCEIRO_TERCEIROS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RCT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaContratoFreteTerceiroTerceiros", Column = "ART_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> AlcadasTerceiros { get; set; }
        // --------------------------------------
    }
}