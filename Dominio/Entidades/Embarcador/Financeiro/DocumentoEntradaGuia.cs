using System;

namespace Dominio.Entidades.Embarcador.Financeiro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TMS_DOCUMENTO_ENTRADA_GUIA", EntityName = "DocumentoEntradaGuia", Name = "Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaGuia", NameType = typeof(DocumentoEntradaGuia))]
    public class DocumentoEntradaGuia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TDG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "DocumentoEntradaTMS", Column = "TDE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual DocumentoEntradaTMS DocumentoEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "TDG_NUMERO", TypeType = typeof(string), Length = 60, NotNull = true)]
        public virtual string Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "TDG_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimento", Column = "TDG_DATA_VENCIMENTO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataVencimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamento", Column = "TDG_DATA_PAGAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Pago", Column = "TDG_PAGO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Pago { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "TDG_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_GERACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoGeracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO_REVERSAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoMovimento TipoMovimentoReversao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoTitulo", Formula = @"SUBSTRING((SELECT DISTINCT ', ' + CAST(T.TIT_CODIGO AS NVARCHAR(20)) FROM T_TITULO T
                                                                                        WHERE T.TIT_STATUS <> 4 AND T.TDG_CODIGO = TDG_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string CodigoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusTitulo", Formula = @"SUBSTRING((SELECT DISTINCT ', ' +CASE WHEN T.TIT_STATUS = 1 THEN 'Aberto' ELSE 'Pago' END FROM T_TITULO T
                                                                                        WHERE T.TIT_STATUS <> 4 AND T.TDG_CODIGO = TDG_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string StatusTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPagamentoTitulo", Formula = @"(SELECT TOP(1) T.TIT_DATA_LIQUIDACAO FROM T_TITULO T
                                                                                        WHERE T.TIT_STATUS <> 4 AND T.TDG_CODIGO = TDG_CODIGO)", TypeType = typeof(DateTime), Lazy = true)]
        public virtual DateTime DataPagamentoTitulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTitulo", Formula = @"(SELECT TOP(1) T.TIT_VALOR_PENDENTE FROM T_TITULO T
                                                                                        WHERE T.TIT_STATUS <> 4 AND T.TDG_CODIGO = TDG_CODIGO)", TypeType = typeof(decimal), Lazy = true)]
        public virtual decimal ValorTitulo { get; set; }

        public virtual string Descricao
        {
            get { return Codigo.ToString(); }
        }
    }
}
