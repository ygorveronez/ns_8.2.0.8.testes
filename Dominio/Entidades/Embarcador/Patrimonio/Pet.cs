using Dominio.Entidades.Embarcador.Configuracoes;
using System;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_PET", EntityName = "Pet", Name = "Dominio.Entidades.Embarcador.Patrimonio.Pet", NameType = typeof(Pet))]
	public class Pet : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PET_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }
		
		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Empresa Empresa { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Cliente Tutor { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PET_NOME", TypeType = typeof(string), Length = 250, NotNull = false)]
		public virtual string Nome { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Especie", Column = "ESP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Especie Especie { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EspecieRaca", Column = "ESR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual EspecieRaca Raca { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PET_ATIVO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Porte", Column = "PET_PORTE", TypeType = typeof(ObjetosDeValor.Enumerador.Porte), NotNull = false)]
		public virtual ObjetosDeValor.Enumerador.Porte Porte { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Sexo", Column = "PET_SEXO", TypeType = typeof(ObjetosDeValor.Enumerador.Sexo), NotNull = false)]
		public virtual ObjetosDeValor.Enumerador.Sexo Sexo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Pelagem", Column = "PET_PELAGEM", TypeType = typeof(ObjetosDeValor.Enumerador.Pelagem), NotNull = false)]
		public virtual ObjetosDeValor.Enumerador.Pelagem Pelagem { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CorAnimal", Column = "COR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual CorAnimal Cor { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PET_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
		public virtual decimal Peso { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "DataNascimento", Column = "PET_DATA_NASCIMENTO", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime DataNascimento { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Comportamento", Column = "PET_COMPORTAMENTO", TypeType = typeof(ObjetosDeValor.Enumerador.Comportamento), NotNull = false)]
		public virtual ObjetosDeValor.Enumerador.Comportamento Comportamento { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PlanoServico", Column = "PLS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual PlanoServico PlanoServico { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PET_CASTRADO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Castrado { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PET_MICROCHIP", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Microchip { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "UltimaVisita", Column = "PET_ULTIMA_VISITA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime UltimaVisita { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "PET_OBSERVACAO", TypeType = typeof(string), Length = 350, NotNull = false)]
		public virtual string Observacao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoFoto", Column = "PET_CAMINHO_FOTO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string CaminhoFoto { get; set; }
	}
}
