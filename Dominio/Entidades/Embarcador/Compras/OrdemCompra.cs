using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.Entidades.Embarcador.Compras
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_COMPRA", EntityName = "OrdemCompra", Name = "Dominio.Entidades.Embarcador.Compras.OrdemCompra", NameType = typeof(OrdemCompra))]
    public class OrdemCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ORC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_NUMERO", TypeType = typeof(int))]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_FORNECEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "ORC_VEICULO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CotacaoCompra", Column = "COT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual CotacaoCompra CotacaoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_DATA_PREVISAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataPrevisaoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_MOTORISTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Motorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_TRANSPORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "MotivoCompra", Column = "MCO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Compras.MotivoCompra MotivoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_CONDICAO_PAGAMENTO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string CondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ORC_SITUACAO_TRATATIVA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoTratativaFluxoCompra SituacaoTratativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BloquearEdicaoOrdemCompraPorAbastecimento", Column = "ORC_BLOQUEAR_EDICAO_ORDEM_COMPRA_ABASTECIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool BloquearEdicaoOrdemCompraPorAbastecimento { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Mercadorias", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ORDEM_COMPRA_MERCADORIA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ORC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemCompraMercadoria", Column = "OCM_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria> Mercadorias { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Autorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_ORDEM_COMPRA")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ORC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaOrdemCompra", Column = "AAA_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.AlcadasOrdemCompra.AprovacaoAlcadaOrdemCompra> Autorizacoes { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "Requisicoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ORDEM_COMPRA_REQUISICAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ORC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "OrdemCompraRequisicao", Column = "OCR_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Compras.OrdemCompraRequisicao> Requisicoes { get; set; }

        public virtual string Descricao
        {
            get { return this.Numero.ToString(); }
        }

        public virtual decimal ValorTotal
        {
            get
            {
                decimal valorTotal = 0;

                if (this.Mercadorias != null)
                    valorTotal = (from o in Mercadorias.ToList() select o.ValorTotal).Sum();

                return valorTotal;
            }
        }

        public virtual string DescricaoSituacao
        {
            get { return Situacao.ObterDescricao(); }
        }
    }
}
