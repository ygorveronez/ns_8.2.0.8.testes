using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_JANELA_CARREGAMENTO_VIAGEM", EntityName = "CargaJanelaCarregamentoViagem", Name = "Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoViagem", NameType = typeof(CargaJanelaCarregamentoViagem))]
	public class CargaJanelaCarregamentoViagem : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "JCV_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Set(0, Name = "CargasJanelaCarregamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_VIAGEM_JANELA_CARREGAMENTO")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "JCV_CODIGO")]
		[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaJanelaCarregamento", Column = "CJC_CODIGO")]
		public virtual ICollection<CargaJanelaCarregamento> CargasJanelaCarregamento { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CargasVinculada", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_JANELA_CARREGAMENTO_VINCULADAS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "JCV_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaVinculada", Column = "VIN_CODIGO")]
        public virtual ICollection<CargaVinculada> CargasVinculada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "JCV_NUMERO_VIAGEM", TypeType = typeof(string), Length = 200, NotNull = false)]
		public virtual string NumeroViagem { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Acao", Column = "JCV_ACAO", TypeType = typeof(AcaoLiberacaoTransportadores), NotNull = false)]
		public virtual AcaoLiberacaoTransportadores Acao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Divisoria", Column = "JCV_DIVISORIA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Divisoria { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CargaPerigosa", Column = "JCV_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool CargaPerigosa { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CustoPlanejado", Column = "JCV_CUSTO_PLANEJADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal CustoPlanejado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CustoAtual", Column = "JCV_CUSTO_ATUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal CustoAtual { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "JCV_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime? DataInicio { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Razao", Column = "JCV_RAZAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
		public virtual string Razao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "JCV_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]
		public virtual string Observacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "StatusLeilao", Column = "JCV_STATUS_LEILAO", TypeType = typeof(StatusLeilaoIntegracaoJanelaCarregamento), NotNull = false)]
		public virtual StatusLeilaoIntegracaoJanelaCarregamento StatusLeilao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "JCV_VALOR_FRETE", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal ValorFrete { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Empresa Transportador { get; set; }

		public virtual string Descricao
		{
			get { return NumeroViagem; }
		}

		public virtual string AcaoDescricao
		{
			get
			{
				return AcaoLiberacaoTransportadoresHelper.ObterDescricao(this.Acao);
			}
		}
	}
}
