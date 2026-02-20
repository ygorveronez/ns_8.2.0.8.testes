using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Global
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_INFORMACOES_BANCARIAS", EntityName = "PedidoInformacoesBancarias", Name = "Dominio.Entidades.PedidoInformacoesBancarias", NameType = typeof(PedidoInformacoesBancarias))]
    public class PedidoInformacoesBancarias : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PIB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        public virtual string Descricao { get { return $"{Pedido.NumeroPedidoEmbarcador} - {TipoInformacaoBancaria?.ObterDescricao() ?? ""}"; } }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChavePIX", Column = "PIB_CHAVE_PIX", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string ChavePIX { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Conta", Column = "PIB_CONTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Conta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Agencia", Column = "PIB_AGENCIA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Agencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ipef", Column = "PIB_IPEF", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Ipef { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInformacaoBancaria", Column = "PIB_TIPO_INFORMACAO_BANCARIA", TypeType = typeof(TipoPagamentoMDFe), Length = 100, NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe? TipoInformacaoBancaria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPagamento", Column = "PIB_TIPO_PAGAMENTO", TypeType = typeof(FormasPagamento), Length = 100, NotNull = false)]
        public virtual FormasPagamento? TipoPagamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdiantamento", Column = "PIB_VALOR_ADIANTAMENTO", TypeType = typeof(decimal), NotNull = false)]
        public virtual decimal ValorAdiantamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IndicadorAltoDesempenho", Column = "PIB_INDICADOR_ALTO_DESEMPENHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IndicadorAltoDesempenho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "PIB_VALOR_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataVencimentoCIOT", Column = "PIB_DATA_VENCIMENTO_CIOT", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataVencimentoCIOT { get; set; }
    }
}
