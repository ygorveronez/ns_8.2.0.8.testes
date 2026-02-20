using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_DOCUMENTO_ENTRADA", EntityName = "DocumentoEntrada", Name = "Dominio.Entidades.DocumentoEntrada", NameType = typeof(DocumentoEntrada))]
    public class DocumentoEntrada : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "DOE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EspecieDocumentoFiscal", Column = "EDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EspecieDocumentoFiscal Especie { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ModeloDocumentoFiscal", Column = "MOD_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ModeloDocumentoFiscal Modelo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoDeConta", Column = "PLA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual PlanoDeConta PlanoDeConta { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Fornecedor { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLancamento", Column = "DOE_NUMERO_LANCAMENTO", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEntrada", Column = "DOE_DATA_ENTRADA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "DOE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "DOE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Serie", Column = "DOE_SERIE", TypeType = typeof(string), Length = 10, NotNull = true)]
        public virtual string Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorProdutos", Column = "DOE_VALOR_PRODUTOS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorProdutos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "DOE_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMS", Column = "DOE_BASE_CALCULO_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMS", Column = "DOE_VALOR_TOTAL_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoICMSST", Column = "DOE_BASE_CALCULO_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal BaseCalculoICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalICMSST", Column = "DOE_VALOR_TOTAL_ICMS_ST", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalICMSST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalIPI", Column = "DOE_VALOR_TOTAL_IPI", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalIPI { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalPIS", Column = "DOE_VALOR_TOTAL_PIS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalCOFINS", Column = "DOE_VALOR_TOTAL_COFINS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalDesconto", Column = "DOE_VALOR_TOTAL_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalDesconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalOutrasDespesas", Column = "DOE_VALOR_TOTAL_OUTRAS_DESPESAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalOutrasDespesas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotalFrete", Column = "DOE_VALOR_TOTAL_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = true)]
        public virtual decimal ValorTotalFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "DOE_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusDocumentoEntrada), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusDocumentoEntrada Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorPagamento", Column = "DOE_INDICADOR_PAGAMENTO", TypeType = typeof(Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada), NotNull = true)]
        public virtual Dominio.Enumeradores.IndicadorPagamentoDocumentoEntrada IndicadorPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Chave", Column = "DOE_CHAVE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Chave { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Itens", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_DOCUMENTO_ENTRADA_ITEM")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "DOE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ItemDocumentoEntrada", Column = "DEI_CODIGO")]
        public virtual IList<Dominio.Entidades.ItemDocumentoEntrada> Itens { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Dominio.Enumeradores.StatusDocumentoEntrada.Aberto:
                        return "Aberto";
                    case Dominio.Enumeradores.StatusDocumentoEntrada.Finalizado:
                        return "Finalizado";
                    case Dominio.Enumeradores.StatusDocumentoEntrada.Cancelado:
                        return "Cancelado";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
