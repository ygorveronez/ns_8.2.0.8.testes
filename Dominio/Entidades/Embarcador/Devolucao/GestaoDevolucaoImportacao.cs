using System;

namespace Dominio.Entidades.Embarcador.Devolucao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DEVOLUCAO_IMPORTACAO", EntityName = "GestaoDevolucaoImportacao", Name = "Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoImportacao", NameType = typeof(GestaoDevolucaoImportacao))]

    public class GestaoDevolucaoImportacao : EntidadeBase
    {
        #region Atributos
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "GDI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OrigemRecebimento", Column = "GDI_ORIGEM_RECEBIMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao), NotNull = false)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemGestaoDevolucao OrigemRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_DEVOLUCAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal NotaFiscalDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDevolucao", Column = "GDV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao GestaoDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "GDI_TIPO_DOCUMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumento", Column = "GDI_NUMERO_DOCUMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDocumento", Column = "GDI_DATA_DOCUMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "GDI_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrega", Column = "GDI_DATA_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Perda", Column = "GDI_PERDA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Perda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiBloqueioFinanceiro", Column = "GDI_POSSUI_BLOQUEIO_FINANCEIRO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiBloqueioFinanceiro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EmDevolucao", Column = "GDI_EM_DEVOLUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EmDevolucao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasAtraso", Column = "GDI_DIAS_ATRASO", TypeType = typeof(int), NotNull = false)]
        public virtual int DiasAtraso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "GDI_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CondicaoPagamento", Column = "GDI_CONDICAO_PAGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CondicaoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Item", Column = "GDI_ITEM", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string Item { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Referencia", Column = "GDI_REFERENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Referencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdentificadorCaso", Column = "GDI_IDENTIFICADOR_CASO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string IdentificadorCaso { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Motivo", Column = "GDI_MOTIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Motivo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReferenciaFatura", Column = "GDI_REFERENCIA_FATURA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ReferenciaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesGerais", Column = "GDI_OBSERVACOES_GERAIS", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacoesGerais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesDocumento", Column = "GDI_OBSERVACOES_DOCUMENTO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacoesDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacoesItem", Column = "GDI_OBSERVACOES_ITEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacoesItem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EquipeVendas", Column = "GDI_EQUIPE_VENDAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EquipeVendas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "EscritorioVendas", Column = "GDI_ESCRITORIO_VENDAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string EscritorioVendas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "GDI_STATUS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFeReferencia", Column = "GDI_CHAVE_NFE_REFERENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFeReferencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveNFD", Column = "GDI_CHAVE_NFD", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveNFD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UsuarioModificouOrdem", Column = "GDI_USUARIO_MODIFICOU_ORDEM", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string UsuarioModificouOrdem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "XML_NFD", Column = "GDI_XML_NFD", Type = "StringClob", NotNull = false)]
        public virtual string XML_NFD { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "GDI_CFOP", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Fornecimento", Column = "GDI_FORNECIMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Fornecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DocumentoFaturamento", Column = "GDI_DOCUMENTO_FATURAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DocumentoFaturamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComPendenciaFinanceira", Column = "GDI_COM_PENDENCIA_FINANCEIRA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ComPendenciaFinanceira { get; set; }

        #endregion

        #region Atributos Virtuais
        public virtual string Descricao
        {
            get
            {
                return NotaFiscalOrigem?.Descricao ?? NumeroDocumento ?? string.Empty;
            }
        }

        public virtual string DeovlucaoGeradaDescricao
        {
            get
            {
                return EmDevolucao ? "Sim" : "Não";
            }
        }

        public virtual string ComPendenciaFinanceiraDescricao
        {
            get
            {
                return ComPendenciaFinanceira ? "Sim" : "Não";
            }
        }
        #endregion
    }
}
