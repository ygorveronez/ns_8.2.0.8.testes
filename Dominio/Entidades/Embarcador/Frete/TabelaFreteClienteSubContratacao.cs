using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO", EntityName = "TabelaFreteClienteSubContratacao", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacao", NameType = typeof(TabelaFreteClienteSubContratacao))]
    public class TabelaFreteClienteSubContratacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteCliente", Column = "TFC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteCliente TabelaFreteCliente { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }
        
        /// <summary>
        /// Valor Percentual que desconta do CT-e quando Subcontrata o Terceiro
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "TCS_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualDesconto { get; set; }

        /// <summary>
        /// Quando uma carga possuir CT-es normais e de subcontratação é possível informar um valor fixo para os de Subcontratação no rateio dos documentos, utilizando essa propriedade.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFixoSubContratacaoParcial", Column = "TCS_VALOR_FIXO_SUBCONTRATACAO_PARCIAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorFixoSubContratacaoParcial { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Valores", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO_ACRESCIMO_DESCONTO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "TCS_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "TabelaFreteClienteSubContratacaoAcrescimoDesconto", Column = "SAD_CODIGO")]
        public virtual IList<TabelaFreteClienteSubContratacaoAcrescimoDesconto> Valores { get; set; }

        public virtual TabelaFreteClienteSubContratacao Clonar()
        {
            return (TabelaFreteClienteSubContratacao)this.MemberwiseClone();
        }
    }
}
