using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_INTEGRACAO_GERENCIAR_LIBERACAO_TRANSPORTADORES", EntityName = "IntegracaoGerenciarLiberacaoTransportadores", Name = "Dominio.Entidades.Embarcador.Logistica.IntegracaoGerenciarLiberacaoTransportadores", NameType = typeof(IntegracaoGerenciarLiberacaoTransportadores))]
	public class IntegracaoGerenciarLiberacaoTransportadores : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILT_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NumeroShipment", Column = "ILT_NUMERO_SHIPMENT", TypeType = typeof(string), Length = 100, NotNull = false)]
		public virtual string NumeroShipment { get; set; }

		[NHibernate.Mapping.Attributes.Set(0, Name = "Cargas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_GERENCIAR_LIBERACAO_TRANSPORTADORES_CARGAS")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "ILT_CODIGO")]
		[NHibernate.Mapping.Attributes.ManyToMany(2, Class = "Carga", Column = "CAR_CODIGO")]
		public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.Carga> Cargas { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Acao", Column = "ILT_ACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores), NotNull = false)]
		public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.AcaoLiberacaoTransportadores? Acao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Divisoria", Column = "ILT_DIVISORIA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Divisoria { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ContemCargaPerigosa", Column = "ILT_CONTEM_CARGA_PERIGOSA", TypeType = typeof(bool), NotNull = false)]
		public virtual bool ContemCargaPerigosa { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CustoPlanejado", Column = "ILT_CUSTO_PLANEJADO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal CustoPlanejado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CustoAtual", Column = "ILT_CUSTO_ATUAL", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal CustoAtual { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataInicio", Column = "ILT_DATA_INICIO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime DataInicio { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Razao", Column = "ILT_RAZAO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string Razao { get; set; }
		
		[NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "ILT_OBSERVACAO", TypeType = typeof(string), Length = 2000, NotNull = false)]

		public virtual string Observacao { get; set; }
	}
}
