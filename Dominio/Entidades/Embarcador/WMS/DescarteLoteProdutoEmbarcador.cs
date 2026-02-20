using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DESCARTE_LOTE_PRODUTO_EMBARCADOR", EntityName = "DescarteLoteProdutoEmbarcador", Name = "Dominio.Entidades.Embarcador.WMS.DescarteLoteProdutoEmbarcador", NameType = typeof(DescarteLoteProdutoEmbarcador))]
    public class DescarteLoteProdutoEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DPE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPE_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPE_MOTIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPE_QUANTIDADE", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPE_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoEmail), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "ProdutoEmbarcadorLote", Column = "PEL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcadorLote Lote { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO_INTERNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario Usuario { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(Class = "Usuario", Column = "FUN_APROVADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Usuario UsuarioAprovador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DPE_DATA_APROVACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacao { get; set; }


        [NHibernate.Mapping.Attributes.Bag(0, Name = "DescartesAutorizacoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AUTORIZACAO_ALCADA_DESCARTE_LOTE_PRODUTO_EMBARCADOR")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DPE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AprovacaoAlcadaDescarteLoteProduto", Column = "AAD_CODIGO")]
        public virtual ICollection<AprovacaoAlcadaDescarteLoteProduto> DescartesAutorizacoes { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Lote?.Descricao ?? string.Empty;
            }
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                switch (this.Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.AgAprovacao:
                        return "Ag. Aprovação";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Finalizado:
                        return "Finalizado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.Rejeitada:
                        return "Rejeitado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoDescarteLoteProdutoEmbarcador.SemRegra:
                        return "Sem Regra";
                    default:
                        return "";
                }
            }
        }
    }
}
