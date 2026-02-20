namespace Dominio.Entidades.Embarcador.Configuracoes
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_ESPECIE_RACA", EntityName = "EspecieRaca", Name = "Dominio.Entidades.Embarcador.Configuracoes.EspecieRaca", NameType = typeof(EspecieRaca))]
	public class EspecieRaca : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ESR_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ESR_ATIVO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "ESR_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
		public virtual string Descricao{ get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Especie", Column = "ESP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Especie Especie { get; set; }
	}
}
