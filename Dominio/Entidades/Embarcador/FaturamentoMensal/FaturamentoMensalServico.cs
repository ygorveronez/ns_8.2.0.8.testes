using System;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_MENSAL_SERVICO", EntityName = "FaturamentoMensalServico", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico", NameType = typeof(FaturamentoMensalServico))]
    public class FaturamentoMensalServico : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalServico>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Quantidade", Column = "FMS_QUANTIDADE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Quantidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorUnitario", Column = "FMS_VALOR_UNITARIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorUnitario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "FMS_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "FMS_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamentoAte", Column = "FMS_DATA_LANCAMENTO_ATE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamentoAte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObservacaoFaturamentoMensal", Column = "FMS_TIPO_OBSERVACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal TipoObservacaoFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FMS_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoMensalCliente", Column = "FMC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoMensalCliente FaturamentoMensalCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoCompra", Column = "FMS_NUMERO_PEDIDO_COMPRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoItemCompra", Column = "FMS_NUMERO_PEDIDO_ITEM_COMPRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoItemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Historico", Column = "FMS_HISTORICO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Historico { get; set; }

        public virtual string DescricaoTipoObservacaoFaturamentoMensal
        {
            get
            {
                switch (this.TipoObservacaoFaturamentoMensal)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Boleto:
                        return "Usar em BOLETO";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum:
                        return "Nenhum";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscal:
                        return "Usar em NF";
                    case ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.NotaFiscalBoleto:
                        return "Usar em NF e BOLETO";
                    default:
                        return "";
                }
            }
        }
        public virtual string Descricao
        {
            get
            {
                return this.Servico?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(FaturamentoMensalServico other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
