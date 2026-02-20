using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Acerto
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_ACERTO_VEICULO_RESULTADO", EntityName = "AcertoVeiculoResultado", Name = "Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado", NameType = typeof(AcertoVeiculoResultado))]
    public class AcertoVeiculoResultado : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Acerto.AcertoVeiculoResultado>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "AcertoViagem", Column = "ACV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Acerto.AcertoViagem AcertoViagem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Veiculo", Column = "VEI_CODIGO_TRACAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Veiculo Tracao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "VeiculosVinculados", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.False, Table = "T_ACERTO_VEICULO_RESULTADO_VEICULOS_VINCULADOS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVR_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Veiculo", Column = "VEI_CODIGO")]
        public virtual ICollection<Veiculo> VeiculosVinculados { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFaturamentoBruto", Column = "AVR_FATURAMENTO_BRUTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorFaturamentoBruto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorResultadoLiquido", Column = "AVR_RESULTADO_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorResultadoLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorICMS", Column = "AVR_ICMS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorICMS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCombustivelTracao", Column = "AVR_COMBUSTIVEL_TRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCombustivelTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCombustivelReboques", Column = "AVR_COMBUSTIVEL_REBOQUES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorCombustivelReboques { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorParametroMediaTracao", Column = "AVR_PARAMETRO_MEDIA_TRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorParametroMediaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMediaTracao", Column = "AVR_MEDIA_TRACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMediaTracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "KMTotal", Column = "AVR_KM_TOTAL", TypeType = typeof(int), NotNull = false)]
        public virtual int KMTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaCombustivel", Column = "AVR_DESPESA_COMBUSTIVEL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesaCombustivel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaArla", Column = "AVR_DESPESA_ARLA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesaArla { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaPedagio", Column = "AVR_DESPESA_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaMotorista", Column = "AVR_DESPESA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesaTotal", Column = "AVR_DESPESA_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesaTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaFrete", Column = "AVR_RECEITA_FRETE", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReceitaFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaPedagio", Column = "AVR_RECEITA_PEDAGIO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReceitaPedagio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaOutros", Column = "AVR_RECEITA_OUTROS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReceitaOutros { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaBonificacao", Column = "AVR_RECEITA_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReceitaBonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ReceitaTotal", Column = "AVR_RECEITA_TOTAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ReceitaTotal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoFinal", Column = "AVR_SALDO_FINAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagens", Column = "AVR_NUMERO_VIAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroViagens { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeViagemCompartilhada", Column = "AVR_QTD_VIAGEM_COMPARTILHADA", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeViagemCompartilhada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorViagemCompartilhada", Column = "AVR_VALOR_VIAGEM_COMPARTILHADA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorViagemCompartilhada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FreteLiquido", Column = "AVR_VALOR_FRETE_LIQUIDO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal FreteLiquido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DespesasMotorista", Column = "AVR_VALOR_DESPESA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DespesasMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualComissao", Column = "AVR_PERCENTUAL_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PercentualComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorComissao", Column = "AVR_VALOR_COMISSAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorComissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Bonificacao", Column = "AVR_VALOR_BONIFICACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Bonificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Desconto", Column = "AVR_VALOR_DESCONTO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Desconto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalPagarMotorista", Column = "AVR_VALOR_PAGAR_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal TotalPagarMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "LiquidoMes", Column = "AVR_VALOR_LIQUIDO_MES", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal LiquidoMes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ocorrencias", Column = "AVR_OCORRENCIAS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal Ocorrencias { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AbastecimentoMotorista", Column = "AVR_ABASTECIMENTO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AbastecimentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PedagioMotorista", Column = "AVR_PEDAGIO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal PedagioMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutraDespesaMotorista", Column = "AVR_OUTRA_DESPESA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal OutraDespesaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AdiantamentoMotorista", Column = "AVR_ADIANTAMENTO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AdiantamentoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiariaMotorista", Column = "AVR_DIARIA_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal DiariaMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SaldoMotorista", Column = "AVR_SALDO_MOTORISTA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal SaldoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoConta", Column = "PLA_CODIGO_USUARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Financeiro.PlanoConta PlanoContaUsuario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_FECHAMENTO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario OperadorFechamento { get; set; }

        public virtual bool Equals(AcertoVeiculoResultado other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }
    }
}
