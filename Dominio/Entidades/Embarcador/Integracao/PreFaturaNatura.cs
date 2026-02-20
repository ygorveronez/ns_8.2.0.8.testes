using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_NATURA_PRE_FATURA", EntityName = "PreFaturaNatura", Name = "Dominio.Entidades.Embarcardor.Integracao.PreFaturaNatura", NameType = typeof(PreFaturaNatura))]
    public class PreFaturaNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "IPN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Fatura", Column = "FAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Embarcador.Fatura.Fatura Fatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPN_NUMERO_PRE_FATURA", TypeType = typeof(long), NotNull = true)]
        public virtual long NumeroPreFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPN_DATA_PRE_FATURA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataPreFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPN_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = true)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPN_MENSAGEM_SITUACAO", TypeType = typeof(string), Length = 300, NotNull = true)]
        public virtual string MensagemSituacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "IPN_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreFaturaNatura Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(COALESCE((SELECT SUM(item.IPI_VALOR_DESCONTO)
                                                                FROM T_INTEGRACAO_NATURA_PRE_FATURA_ITEM item
                                                                WHERE item.IPN_CODIGO = IPN_CODIGO), 0))", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTotalDescontoItens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(COALESCE((select SUM(cte.CON_VALOR_RECEBER) from T_INTEGRACAO_NATURA_PRE_FATURA_ITEM item
                                                                INNER JOIN T_CARGA_CTE cargaCTe on item.CCT_CODIGO = cargaCTe.CCT_CODIGO
                                                                INNER JOIN T_CTE cte on cte.CON_CODIGO = cargaCTe.CON_CODIGO
                                                                where item.IPN_CODIGO = IPN_CODIGO), 0))", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTotalReceberCTes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Formula = @"(COALESCE((SELECT COUNT(item.IPI_CODIGO)
                                                                FROM T_INTEGRACAO_NATURA_PRE_FATURA_ITEM item
                                                                WHERE item.IPN_CODIGO = IPN_CODIGO), 0))", TypeType = typeof(int), Lazy = true)]
        public virtual int QuantidadeItens { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_NATURA_PRE_FATURA_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IDT_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegracaoNatura", Column = "INA_CODIGO")]
        public virtual ICollection<IntegracaoNatura> Integracoes { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_NATURA_PRE_FATURA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "IPN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemPreFaturaNatura", Column = "IPI_CODIGO")]
        public virtual IList<Dominio.Entidades.Embarcador.Integracao.ItemPreFaturaNatura> Itens { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.NumeroPreFatura.ToString();
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return Situacao.ObterDescricao();
            }
        }
    }
}
