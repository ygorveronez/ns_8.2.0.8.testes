namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_CLIENTE_SUB_CONTRATACAO_ACRESCIMO_DESCONTO", EntityName = "TabelaFreteClienteSubContratacaoAcrescimoDesconto", Name = "Dominio.Entidades.Embarcador.Frete.TabelaFreteClienteSubContratacaoAcrescimoDesconto", NameType = typeof(TabelaFreteClienteSubContratacaoAcrescimoDesconto))]
    public class TabelaFreteClienteSubContratacaoAcrescimoDesconto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SAD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFreteClienteSubContratacao", Column = "TCS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFreteClienteSubContratacao TabelaFreteClienteSubContratacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Justificativa", Column = "JUS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Fatura.Justificativa Justificativa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "SAD_VALOR", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal Valor { get; set; }
    }
}
