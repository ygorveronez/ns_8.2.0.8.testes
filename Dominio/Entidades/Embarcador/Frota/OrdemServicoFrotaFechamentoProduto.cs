namespace Dominio.Entidades.Embarcador.Frota
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FROTA_ORDEM_SERVICO_FECHAMENTO_PRODUTO", EntityName = "OrdemServicoFrotaFechamentoProduto", Name = "Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaFechamentoProduto", NameType = typeof(OrdemServicoFrotaFechamentoProduto))]
    public class OrdemServicoFrotaFechamentoProduto : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OrdemServicoFrota", Column = "OSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual OrdemServicoFrota OrdemServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FinalidadeProdutoOrdemServico", Column = "FPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FinalidadeProdutoOrdemServico FinalidadeProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOrcado", Column = "OFP_VALOR_ORCADO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorOrcado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDocumento", Column = "OFP_VALOR_DOCUMENTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal ValorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "OFP_VALOR_UNITARIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeOrcada", Column = "OFP_QUANTIDADE_ORCADA", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal QuantidadeOrcada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeDocumento", Column = "OFP_QUANTIDADE_DOCUMENTO", TypeType = typeof(decimal), NotNull = true)]
        public virtual decimal QuantidadeDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Garantia", Column = "OFP_GARANTIA", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Garantia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Autorizado", Column = "OFP_AUTORIZADO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Autorizado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "OFP_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OFP_ORIGEM", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLancamento Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LocalArmazenamentoProduto", Column = "LAP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.LocalArmazenamentoProduto LocalArmazenamento { get; set; }

        public virtual string CorSituacao
        {
            get
            {
                switch (Situacao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota.NaoOrcado:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Vermelho;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota.DiferenteOrcado:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Amarelo;
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoProdutoFechamentoOrdemServicoFrota.ConformeOrcado:
                        return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Verde;
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Produto.Descricao;
            }
        }
    }
}
