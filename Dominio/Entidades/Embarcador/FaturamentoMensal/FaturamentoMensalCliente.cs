using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.FaturamentoMensal
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FATURAMENTO_MENSAL_CLIENTE", EntityName = "FaturamentoMensalCliente", Name = "Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente", NameType = typeof(FaturamentoMensalCliente))]
    public class FaturamentoMensalCliente : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.FaturamentoMensal.FaturamentoMensalCliente>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FMC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ativo", Column = "FMC_ATIVO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Ativo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiaFatura", Column = "FMC_DIA_FATURA", TypeType = typeof(int), NotNull = false)]
        public virtual int DiaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorTotal", Column = "FMC_VALOR_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataUltimaFatura", Column = "FMC_DATA_ULTIMA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataProximaFatura", Column = "FMC_DATA_PROXIMA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataProximaFatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorServicoPrincipal", Column = "FMC_VALOR_SERVICO_PRINCIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorServicoPrincipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorAdesao", Column = "FMC_VALOR_ADESAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorAdesao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoObservacaoFaturamentoMensal", Column = "FMC_TIPO_OBSERVACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal TipoObservacaoFaturamentoMensal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "FMC_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "FaturamentoMensalGrupo", Column = "FMG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual FaturamentoMensalGrupo FaturamentoMensalGrupo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoMovimento", Column = "TIM_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.TipoMovimento TipoMovimento { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Pessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Servico", Column = "SER_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NotaFiscal.Servico Servico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaDaOperacao", Column = "NAT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaDaOperacao NaturezaDaOperacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "BoletoConfiguracao", Column = "BCF_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Financeiro.BoletoConfiguracao BoletoConfiguracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "ServicosExtras", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_FATURAMENTO_MENSAL_SERVICO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "FMC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "FaturamentoMensalServico", Column = "FMS_CODIGO")]
        public virtual IList<FaturamentoMensalServico> ServicosExtras { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoNotaFiscal", Column = "FMC_TIPO_NOTA_FISCAL", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoNota), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoNota TipoNotaFiscal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoCompra", Column = "FMC_NUMERO_PEDIDO_COMPRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPedidoItemCompra", Column = "FMC_NUMERO_PEDIDO_ITEM_COMPRA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroPedidoItemCompra { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataContrato", Column = "FMC_DATA_CONTRATO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataContrato { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "FMC_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamentoAte", Column = "FMC_DATA_LANCAMENTO_ATE", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataLancamentoAte { get; set; }

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

        public virtual string DescricaoAtivo
        {
            get
            {
                switch (this.Ativo)
                {
                    case true:
                        return "Ativo";
                    case false:
                        return "Inativo";
                    default:
                        return "";
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Pessoa?.Descricao ?? string.Empty;
            }
        }

        public virtual bool Equals(FaturamentoMensalCliente other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
