using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_COTACAO", EntityName = "CotacaoCompra", Name = "Dominio.Entidades.Embarcador.Compras.CotacaoCompra", NameType = typeof(CotacaoCompra))]
    public class CotacaoCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_NUMERO", TypeType = typeof(int))]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_DATA_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_DESCRICAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "COT_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Produtos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_PRODUTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoProduto", Column = "COP_CODIGO")]
        public virtual IList<CotacaoProduto> Produtos { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Fornecedores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_FORNECEDOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoFornecedor", Column = "COF_CODIGO")]
        public virtual IList<CotacaoFornecedor> Fornecedores { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Requisicoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_COTACAO_REQUISICAO_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "COT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CotacaoCompraRequisicaoMercadoria", Column = "CRM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria> Requisicoes { get; set; }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Aberto:
                        return "Aberto";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.AguardandoRetorno:
                        return "Aguardando Retorno";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Cancelado:
                        return "Cancelado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Finalizado:
                        return "Finalizado";
                    default:
                        return "";
                }
            }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                decimal valorTotal = 0;

                if (this.Produtos != null)
                    valorTotal = (from o in Produtos.ToList() select o.ValorTotal).Sum();

                return valorTotal;
            }
        }
    }
}