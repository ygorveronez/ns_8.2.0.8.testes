using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_XML_NOTA_FISCAL_CONTABILIZACAO", EntityName = "XMLNotaFiscalContabilizacao", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalContabilizacao", NameType = typeof(XMLNotaFiscalContabilizacao))]
    public class XMLNotaFiscalContabilizacao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscal", Column = "NFX_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscal XMLNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CODIGO_EMPRESA", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NOME_EMPRESA", TypeType = typeof(string), NotNull = false)]
        public virtual string NomeEmpresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_NUMERO_RECEBIMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CFOP_ENTRADA", TypeType = typeof(string), NotNull = false, Length = 10)]
        public virtual string CFOPEntrada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DATA_CONTABILIZACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContabilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DATA_RECEBIMENTO_FISICO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimentoFisico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CODIGO_TRANSACAO_RECEBIMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoTransacaoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_TRANSACAO_RECEBIMENTO", TypeType = typeof(string), NotNull = false)]
        public virtual string TransacaoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_REVERSAO_RECEBIMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ReversaoRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CALCULAR_PIS_COFINS", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CalcularPisCofins { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CONTA_TRANSACAO", TypeType = typeof(string), NotNull = false)]
        public virtual string ContaTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_UC", TypeType = typeof(string), NotNull = false)]
        public virtual string UC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_ORDEM_VENDA", TypeType = typeof(string), NotNull = false)]
        public virtual string OrdemVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DATA_ORDEM_VENDA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataOrdemVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CODIGO_UNICO_NF", TypeType = typeof(string), NotNull = false)]
        public virtual string CodigoUnicoNF { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_ESTRUTURA_VENDA", TypeType = typeof(string), NotNull = false)]
        public virtual string EstruturaVenda { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_ESPECIE", TypeType = typeof(string), NotNull = false)]
        public virtual string Especie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_ITEM_FRETE", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string ItemFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CONTA_CONTABIL", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string ContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_MERCADO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Mercado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DIRETORIA", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string Diretoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DESCRICAO_UC", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string DescricaoUC { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_PEDAGIO", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string Pedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_CIA", TypeType = typeof(string), NotNull = false, Length = 5)]
        public virtual string CIA { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_COD_CONTA_CONTABIL", TypeType = typeof(string), NotNull = false, Length = 100)]
        public virtual string CodContaContabil { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFC_DESCRICAO_TRANSACAO", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string DescricaoTransacao { get; set; }
    }
}
