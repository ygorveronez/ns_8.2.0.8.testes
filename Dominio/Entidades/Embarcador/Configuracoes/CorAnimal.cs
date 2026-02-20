namespace Dominio.Entidades.Embarcador.Configuracoes
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_COR_ANIMAL", EntityName = "CorAnimal", Name = "Dominio.Entidades.Embarcador.Configuracoes.CorAnimal", NameType = typeof(CorAnimal))]
	public class CorAnimal : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "COR_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "COR_ATIVO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "COR_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Empresa Empresa { get; set; }
	}
}
