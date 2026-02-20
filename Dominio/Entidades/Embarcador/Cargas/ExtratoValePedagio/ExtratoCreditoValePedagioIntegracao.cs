using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXTRATO_CREDITO_VALE_PEDAGIO_INTEGRACAO", EntityName = "ExtratoCreditoValePedagioIntegracao", Name = "Dominio.Entidades.Embarcador.Cargas.ExtratoCreditoValePedagioIntegracao", NameType = typeof(ExtratoCreditoValePedagioIntegracao))]
	public class ExtratoCreditoValePedagioIntegracao : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EVI_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "EVI_NUMERO", TypeType = typeof(long), NotNull = false)]
		public virtual long Numero { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataOperacao", Column = "EVI_DATA_OPERACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataOperacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataCompra", Column = "EVI_DATA_COMPRA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataCompra { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Acao", Column = "EVI_ACAO", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string Acao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "EVI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorOperacao", Column = "EVI_VALOR_OPERACAO", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
		public virtual decimal? ValorOperacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "EVI_NUMERO_VIAGEM", TypeType = typeof(long), NotNull = false)]
		public virtual long NumeroViagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioVigencia", Column = "EVI_DATA_INICIO_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataInicioVigencia { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataFimVigencia", Column = "EVI_DATA_FIM_VIGENCIA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataFimVigencia { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataPassagem", Column = "EVI_DATA_PASSAGEM", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataPassagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CNPJCPFTransp", Column = "EVI_CNPJ_CPF_TRANSP", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string CNPJCPFTransp { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomeTransp", Column = "EVI_NOME_TRANSP", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NomeTransp { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Tag", Column = "EVI_TAG", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string Tag { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Placa", Column = "EVI_PLACA", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string Placa { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomeRota", Column = "EVI_NOME_ROTA", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NomeRota { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ItemFinanceiro1", Column = "EVI_ITEM_FINANCEIRO_1", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string ItemFinanceiro1 { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ItemFinanceiro2", Column = "EVI_ITEM_FINANCEIRO_2", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string ItemFinanceiro2 { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ItemFinanceiro3", Column = "EVI_ITEM_FINANCEIRO_3", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string ItemFinanceiro3 { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "SaldoDia", Column = "EVI_SALDO_DIA", TypeType = typeof(decimal), Scale = 2, Precision = 11, NotNull = false)]
		public virtual decimal? SaldoDia { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomePraca", Column = "EVI_NOME_PRACA", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NomePraca { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomeRodovia", Column = "EVI_NOME_RODOVIA", TypeType = typeof(string), Length = 50, NotNull = false)]
		public virtual string NomeRodovia { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomeConcessionaria", Column = "EVI_NOME_CONCESSIONARIA", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NomeConcessionaria { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Fatura", Column = "EVI_FATURA", TypeType = typeof(string), NotNull = false)]
		public virtual string Fatura { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataFatura", Column = "EVI_DATA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataFatura { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TipoVP", Column = "EVI_TIPO_VP", TypeType = typeof(int), NotNull = false)]
		public virtual int TipoVP { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "EVI_STATUS", TypeType = typeof(int), NotNull = false)]
		public virtual int Status { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamentoExtratoCredito", Column = "EVI_SITUACAO_PROCESSAMENTO_EXTRATO_CREDITO", TypeType = typeof(SituacaoProcessamentoExtratoValePedagio), NotNull = false)]
		public virtual SituacaoProcessamentoExtratoValePedagio SituacaoProcessamentoExtratoCredito { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "EVI_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataCriacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "EVI_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataAtualizacao { get; set; }
	}
}
