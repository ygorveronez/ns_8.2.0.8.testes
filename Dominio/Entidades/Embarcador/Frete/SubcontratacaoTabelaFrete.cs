namespace Dominio.Entidades.Embarcador.Frete
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TABELA_FRETE_SUBCONTRATACAO", EntityName = "SubcontratacaoTabelaFrete", Name = "Dominio.Entidades.Embarcador.Frete.SubcontratacaoTabelaFrete", NameType = typeof(SubcontratacaoTabelaFrete))]
    public class SubcontratacaoTabelaFrete : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TFS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TabelaFrete", Column = "TBF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TabelaFrete TabelaFrete { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        /// <summary>
        /// Valor Percentual que desconta do CT-e quando Subcontrata o Terceiro
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualDesconto", Column = "TFS_PERCENTUAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualDesconto { get; set; }
        
        /// <summary>
        /// Valor Percentual que cobra sobre o valor do CT-e quando é subcontratado pelo Terceiro
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualCobranca", Column = "TFS_PERCENTUAL_COBRANCA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal PercentualCobranca { get; set; }

        public virtual string Descricao
        {
            get
            {
                return this.Pessoa.Descricao;
            }
        }
    }
}
