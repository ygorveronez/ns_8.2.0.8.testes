using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_DESCARTE", EntityName = "RegraDescarte", Name = "Dominio.Entidades.Embarcador.WMS.RegraDescarte", NameType = typeof(RegraDescarte))]
    public class RegraDescarte : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_PRODUTO_EMBARCADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_DEPOSITO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorDeposito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_DEPOSITO_RUA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorRua { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_DEPOSITO_BLOCO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorBloco { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_DEPOSITO_POSICAO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorPosicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RED_DEPOSITO_QUANTIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_APROVACAO_DESCARTE_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasProdutoEmbarcador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_PRODUTO_EMBARCADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaProdutoEmbarcador", Column = "RDP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaProdutoEmbarcador> AlcadasProdutoEmbarcador { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasDeposito", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_DEPOSITO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaDeposito", Column = "RDD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaDeposito> AlcadasDeposito { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasRua", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_DEPOSITO_RUA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaRua", Column = "RDR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaRua> AlcadasRua { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasBloco", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_DEPOSITO_BLOCO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaBloco", Column = "RDB_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaBloco> AlcadasBloco { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPosicao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_DEPOSITO_POSICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaPosicao", Column = "RDP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaPosicao> AlcadasPosicao { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasQuantidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_DESCARTE_DEPOSITO_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RED_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadaQuantidade", Column = "RDQ_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.WMS.AlcadaQuantidade> AlcadasQuantidade { get; set; }
        // --------------------------------------
    }
}
