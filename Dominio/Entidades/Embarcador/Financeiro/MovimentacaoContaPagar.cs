using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MOVIMENTACAO_CONTA_PAGAR", EntityName = "MovimentacaoContaPagar", Name = "Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar", NameType = typeof(MovimentacaoContaPagar))]
    public class MovimentacaoContaPagar : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MCP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmpresa", Column = "MCP_CODIGO_EMPRESA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CodigoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Transportador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContaPagar", Column = "CPA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.ContaPagar ContaPagar { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Referencia", Column = "MCP_REFERENCIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Referencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PmntBlock", Column = "MCP_PMNT_BLOCK", TypeType = typeof(string), Length = 1, NotNull = false)]
        public virtual string PmntBlock { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PostedKey", Column = "MCP_POST_KEY", TypeType = typeof(PostedKey), NotNull = false)]
        public virtual PostedKey PostedKey { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTaxa", Column = "MCP_CODIGO_TAXA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoTaxa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "MCP_DATA_DOCUMENOT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DueData", Column = "MCP_DUE_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DueData { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "MCP_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMonetario", Column = "MCP_VALOR_MONETARIO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorMonetario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EspecialGL", Column = "MCP_ESPECIAL_GL", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string EspecialGL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCompensamento", Column = "MCP_DATA_COMPENSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCompensamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClrngDoc", Column = "MCP_CLRNG_DOC", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string ClrngDoc { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Texto", Column = "MCP_TEXTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Texto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "MCP_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeUsuario", Column = "MCP_NOME_USUARIO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string NomeUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "InvoiceRef", Column = "MCP_INVOICE_REF", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string InvoiceRef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPostagem", Column = "MCP_DATA_POSTAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPostagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Atribuicao", Column = "MCP_ATRIBUICAO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Atribuicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DisBase", Column = "MCP_DIS_BASE", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal DisBase { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Moeda", Column = "MCP_MOEDA", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Moeda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TermoPagamento", Column = "MCP_TERMO_PAGAMENTO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string TermoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocTextoCabecalho", Column = "MCP_DOC_TEXTO_CABECALHO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DocTextoCabecalho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocHeaderText", Column = "MCP_DOC_HEADER_TEXT", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DocHeaderText { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveReferencia", Column = "MCP_CHAVE_REFERENCIA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ChaveReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "MCP_VALOR_TOTAL", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoCompra", Column = "MCP_DOCUMENTO_COMPRA", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DocumentoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParceiroComercial", Column = "MCP_PARCEIRO_COMERCIAL", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string ParceiroComercial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GL", Column = "MCP_GL", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string GL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CentroLucro", Column = "MCP_CENTRO_LUCRO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CentroLucro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Segmento", Column = "MCP_SEGMENTO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Segmento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "MCP_ORDEM", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CentroCusto", Column = "MCP_CENTRO_CUSTO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CentroCusto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DebitoCredito", Column = "MCP_DEBITO_CREDITO", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string DebitoCredito { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoVencimento", Column = "MCP_SITUACAO_VENCIMENTO", TypeType = typeof(SituacaoVencimentoMovimentacao), NotNull = false)]
        public virtual SituacaoVencimentoMovimentacao SituacaoVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DebitosCompensados", Column = "MCP_DEBITOS_COMPENSADO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string DebitosCompensados { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_DESTINATARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Destinatario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_RECEBEDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Recebedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensageLog", Column = "MCP_MENSAGE_LOG", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string MensageLog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUpload", Column = "MCP_DATA_UPLOAD", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUpload { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "MCP_PROTOCOLO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataAprovacaoTaxa", Column = "MCP_DATA_APROVACAO_TAXA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataAprovacaoTaxa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCte", Column = "MCP_NUMERO_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieCte", Column = "MCP_SERIE_CTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SerieCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveAcesso", Column = "MCP_CHAVE_ACESSO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChaveAcesso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NetValor", Column = "MCP_NET_VALOR", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal NetValor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRegistro", Column = "MCP_TIPO_REGISTRO", TypeType = typeof(TipoRegistro), NotNull = false)]
        public virtual TipoRegistro TipoRegistro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MiroRecebida", Column = "MCP_MIRO_RECEBIDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MiroRecebida { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoMiro", Column = "MCP_OBSERVACAO_MIRO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ObservacaoMiro { get; set; }   
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "Comments", Column = "MCP_COMMENTS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Comments { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamento", Column = "MCP_SITUACAO_PROCESSAMENTO", TypeType = typeof(SituacaoProcessamento), NotNull = false)]
        public virtual SituacaoProcessamento SituacaoProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TermoQuitacaoFinanceiro", Column = "TQU_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TermoQuitacaoFinanceiro TermoQuitacaoFinanceiro { get; set; }

    }
}
