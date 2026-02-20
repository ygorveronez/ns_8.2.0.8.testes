using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Configuracoes
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_ESPECIE", EntityName = "Especie", Name = "Dominio.Entidades.Embarcador.Configuracoes.Especie", NameType = typeof(Especie))]
	public class Especie : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESP_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ESP_ATIVO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ESP_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
		public virtual string Descricao{ get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Empresa Empresa { get; set; }

		[NHibernate.Mapping.Attributes.Bag(0, Name = "Racas", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ESPECIE_RACA")]
		[NHibernate.Mapping.Attributes.Key(1, Column = "ESP_CODIGO")]
		[NHibernate.Mapping.Attributes.OneToMany(2, Class = "EspecieRaca")]
		public virtual ICollection<EspecieRaca> Racas { get; set; }
	}
}
