using Dominio.Entidades.Embarcador.Filiais;
using System;

namespace Dominio.Entidades.Embarcador.Produtos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ORDEM_DE_COMPRA_HISTORIAL", DynamicUpdate = true, EntityName = "OrdemCompraHistorial", Name = "Dominio.Entidades.Embarcador.Produtos.OrdemCompraHistorial", NameType = typeof(OrdemCompraHistorial))]
    public class OrdemCompraHistorial : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OPH_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ControleIntegracaoEmbarcador", Column = "OPH_CONTROLE_INTEGRACAO_EMBARCADOR", TypeType = typeof(int), NotNull = false)]
        public virtual int ControleIntegracaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "OPH_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroItemDocumento", Column = "OPH_NUMERO_ITEM_DOCUMENTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NumeroItemDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoProduto", Column = "OPH_NUMERO_DOCUMENTO_PRODUTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NumeroDocumentoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ItemNoDocumento", Column = "OPH_ITEM_NO_DOCUMENTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string ItemNoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoDocumento", Column = "OPH_ANO_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SequencialClassificacaoContabil", Column = "OPH_SEQUENCIAL_CLASSIFICACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int SequencialClassificacaoContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransacaoHistoricoOrdem", Column = "OPH_TIPO_TRANSACAO_HISTORICO_ORDEM", TypeType = typeof(string), NotNull = false)]
        public virtual string TipoTransacaoHistoricoOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CategoriaHistorico", Column = "OPH_CATEGORIA_HISTORICO", TypeType = typeof(string), NotNull = false)]
        public virtual string CategoriaHistorico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMovimento", Column = "OPH_TIPO_MOVIMENTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamentoDocumento", Column = "OPH_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamentoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntradaDocContabil", Column = "OPH_DATA_ENTRADA_DOC_CONTABIL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntradaDocContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "OPH_DATA_DOCUMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HoraEntradaDoccontabil", Column = "OPH_HORA_ENTRADA_DOC_CONTABIL", TypeType = typeof(TimeSpan), NotNull = false)]
        public virtual TimeSpan? HoraEntradaDoccontabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "OPH_QUANTIDADE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeUnidPrecoOrdem", Column = "OPH_QUANTIDADE_UNID_PRECO_ORDEM", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QuantidadeUnidPrecoOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "OPH_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMoedaDocumento", Column = "OPH_VALOR_MOEDA_DOCUMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMoedaDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QtdeUnidadeNota", Column = "OPH_QUANTIDADE_UNIDADE_NOTA", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal QtdeUnidadeNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Moeda", Column = "OPH_MOEDA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MoedaSecundaria", Column = "OPH_MOEDA_SECUNDADIA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string MoedaSecundaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeNota", Column = "OPH_UNIDADE_NOTA", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string UnidadeNota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoProdutoSecundario", Column = "OPH_CODIGO_PRODUTO_SECUNDARIO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoProdutoSecundario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Operacao", Column = "OPH_OPERACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string Operacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EntregaConcluida", Column = "OPH_ENTREGA_CONCLUIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntregaConcluida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoReferencia", Column = "OPH_DOCUMENTO_REFERENCIA", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string DocumentoReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentoDocref", Column = "OPH_NUMERO_DOCUMENTO_REF", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string NumeroDocumentoDocref { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AnoFiscalDocumentoRef", Column = "OPH_ANO_FISCAL_DOCUMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int AnoFiscalDocumentoRef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoImposto", Column = "OPH_CODIGO_IMPOSTO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string CodigoImposto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "OPH_USUARIO", TypeType = typeof(string), Length = 25, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ProdutoEmbarcador Produto { get; set; }
    }
}
