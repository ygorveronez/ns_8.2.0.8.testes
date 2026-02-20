using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas.ExtratoValePedagio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_EXTRATOS_VALE_PEDAGIO", EntityName = "ExtratosValePedagio", Name = "Dominio.Entidades.Embarcador.Cargas.ExtratosValePedagio", NameType = typeof(ExtratosValePedagio))]
	public class ExtratosValePedagio : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "EVP_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "EVP_NUMERO_VIAGEM", TypeType = typeof(long), NotNull = false)]
		public virtual long NumeroViagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Fatura", Column = "EVP_FATURA", TypeType = typeof(string), NotNull = false)]
		public virtual string Fatura { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataFatura", Column = "EVP_DATA_FATURA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataFatura { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "TemCredito", Column = "EVP_TEM_CREDITO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool TemCredito { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoExtrato", Column = "EVP_SITUACAO_EXTRATO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio), NotNull = false)]
		public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoExtratoValePedagio SituacaoExtrato { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoValePedagio", Column = "CVP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual ValePedagio.CargaIntegracaoValePedagio ValePedagio { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataCriacao", Column = "EVP_DATA_CRIACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime DataCriacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataAtualizacao", Column = "EVP_DATA_ATUALIZACAO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataAtualizacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoProcessamento", Column = "EVP_SITUACAO_PROCESSAMENTO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio), NotNull = false)]
		public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamentoExtratoValePedagio SituacaoProcessamento { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataProcessamento", Column = "EVP_DATA_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataProcessamento { get; set; }

		[NHibernate.Mapping.Attributes.Set(0, Name = "Extratos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_EXTRATOS_VALE_PEDAGIO_EXTRATO")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "EVP_CODIGO")]
		[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ExtratoCreditoValePedagioIntegracao", Column = "EVI_CODIGO")]
		public virtual ICollection<ExtratoCreditoValePedagioIntegracao> Extratos { get; set; }

		public virtual string DescricaoSituacaoValePedagio
		{
			get { return ValePedagio != null ? ValePedagio.SituacaoValePedagio.ObterDescricao() : string.Empty; }
		}

		public virtual string DescricaoSituacaoExtrato
		{
			get { return SituacaoExtrato.ObterDescricao(); }
		}
	}
}
