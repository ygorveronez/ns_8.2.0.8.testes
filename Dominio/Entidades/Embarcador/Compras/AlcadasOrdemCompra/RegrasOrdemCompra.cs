using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_ORDEM_COMPRA", EntityName = "RegrasOrdemCompra", Name = "Dominio.Entidades.Embarcador.Compras.RegrasOrdemCompra", NameType = typeof(RegrasOrdemCompra))]
    public class RegrasOrdemCompra : Alcada.RegraAprovacao
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_FORNECEDOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_OPERADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_SETOROPERADOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorSetorOperador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_PRODUTO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_VALOR", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_QUANTIDADE", TypeType = typeof(bool), NotNull = true)]
        public virtual bool RegraPorQuantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_GRUPO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorGrupoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "RRC_ATIVO_PERCENTUAL_DIFERENCA_VALOR_CUSTO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RegraPorPercentualDiferencaValorCustoProduto { get; set; }



        [NHibernate.Mapping.Attributes.Set(0, Name = "Aprovadores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_REGRA_ORDEM_COMPRA_FUNCIONARIOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Usuario", Column = "FUN_CODIGO")]
        public virtual ICollection<Usuario> Aprovadores { get; set; }



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasFornecedor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaFornecedor", Column = "ARF_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaFornecedor> AlcadasFornecedor { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasOperador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_OPERADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaOperador", Column = "ARO_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaOperador> AlcadasOperador { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasSetorOperador", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_SETOR_OPERADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaSetorOperador", Column = "ARS_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaSetorOperador> AlcadasSetorOperador { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaProduto", Column = "ARP_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaProduto> AlcadasProduto { get; set; }
        // --------------------------------------



        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasValor", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_VALOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaValor", Column = "ARV_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaValor> AlcadasValor { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasQuantidade", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_QUANTIDADE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaQuantidade", Column = "ARQ_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaQuantidade> AlcadasQuantidade { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasGrupoProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_GRUPO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaGrupoProduto", Column = "ARG_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaGrupoProduto> AlcadasGrupoProduto { get; set; }
        // --------------------------------------

        // --------------------------------------
        [NHibernate.Mapping.Attributes.Bag(0, Name = "AlcadasPercentualDiferencaValorCustoProduto", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ALCADA_ORDEM_COMPRA_PERCENTUAL_DIFERENCA_VALOR_CUSTO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "RRC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto", Column = "APD_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AlcadaPercentualDiferencaValorCustoProduto> AlcadasPercentualDiferencaValorCustoProduto { get; set; }
        // --------------------------------------

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }
    }
}
