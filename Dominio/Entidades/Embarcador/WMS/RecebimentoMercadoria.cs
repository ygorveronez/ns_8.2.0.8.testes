using System;

namespace Dominio.Entidades.Embarcador.WMS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_RECEBIMENTO_MERCADORIA", EntityName = "RecebimentoMercadoria", Name = "Dominio.Entidades.Embarcador.WMS.RecebimentoMercadoria", NameType = typeof(RecebimentoMercadoria))]
    public class RecebimentoMercadoria : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "REM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "REM_DESCRICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLote", Column = "REM_NUMERO_LOTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCliente", Column = "REM_CNPJ_CLIETE", TypeType = typeof(double), Scale = 6, Precision = 18, NotNull = false)]
        public virtual double CNPJCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NCM", Column = "REM_NCM", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string NCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFe", Column = "REM_CHAVE_NFE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificacao", Column = "REM_IDENTIFICACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Identificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoBarras", Column = "REM_CODIGO_BARRAS", Type = "StringClob", NotNull = false)]
        public virtual string CodigoBarras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "REM_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime ?DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "REM_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLote", Column = "REM_QUANTIDADE_LOTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeLote { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Comprimento", Column = "REM_COMPRIMENTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Comprimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Altura", Column = "REM_ALTURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Altura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Largura", Column = "REM_LARGURA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Largura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetroCubico", Column = "REM_METRO_CUBICO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal MetroCubico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "REM_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadePalet", Column = "REM_QUANTIDADE_PALET", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadePalet { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DepositoPosicao", Column = "DPO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DepositoPosicao DepositoPosicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Recebimento", Column = "RME_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Recebimento Recebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ProdutoEmbarcador", Column = "PRO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produtos.ProdutoEmbarcador ProdutoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Produto", Column = "PRO_CODIGO_INTERNO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Produto Produto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRecebimentoMercadoria", Column = "REM_TIPO_RECEBIMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRecebimentoMercadoria ?TipoRecebimentoMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "REM_SERIE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeConferida", Column = "REM_QUANTIDADE_CONFERIDA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeConferida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeFaltante", Column = "REM_QUANTIDADE_FALTANTE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal QuantidadeFaltante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CPF_CNPJ_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissaoNF", Column = "REM_DATA_EMISSAO_NF", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime ?DataEmissaoNF { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_EMISSORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaEmissora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO_DISTRIBUIDORA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa EmpresaDistribuidora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoBruto", Column = "REM_PESO_BRUTO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoLiquido", Column = "REM_PESO_LIQUIDO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal PesoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNF", Column = "REM_NUMERO_NF", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieNF", Column = "REM_SERIE_NF", TypeType = typeof(int), NotNull = false)]
        public virtual int SerieNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoria", Column = "REM_VALOR_MERCADORIA", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorNF", Column = "REM_VALOR_NF", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal ValorNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "VolumeNF", Column = "REM_VOLUME_NF", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string VolumeNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoUnidade", Column = "REM_TIPO_UNIDADE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string TipoUnidade { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.XMLNotaFiscal XMLNotaFiscal { get; set; }
    }

}
