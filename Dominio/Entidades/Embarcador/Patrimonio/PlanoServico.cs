namespace Dominio.Entidades.Embarcador.Patrimonio
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_PLANO_SERVICO", EntityName = "PlanoServico", Name = "Dominio.Entidades.Embarcador.Patrimonio.PlanoServico", NameType = typeof(PlanoServico))]
	public class PlanoServico : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PLS_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PLS_ATIVO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool Ativo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "PLS_DESCRICAO", TypeType = typeof(string), Length = 250, NotNull = false)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Empresa Empresa { get; set; }
	}
}
