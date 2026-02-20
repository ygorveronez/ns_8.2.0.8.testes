namespace Dominio.Entidades.Global
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_FILTRO_PESQUISA_MODELO", EntityName = "FiltroPesquisaModelo", Name = "Dominio.Entidades.FiltroPesquisaModelo", NameType = typeof(FiltroPesquisaModelo))]
	public class FiltroPesquisaModelo : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "FPM_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "FPM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ModeloPadrao", Column = "FPM_MODELO_PADRAO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool ModeloPadrao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "ModeloExclusivoUsuario", Column = "FPM_MODELO_EXCLUSIVO_USUARIO", TypeType = typeof(bool), NotNull = false)]
		public virtual bool ModeloExclusivoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AvancarDatasAutomaticamente", Column = "FPM_AVANCAR_DATAS_AUTOMATICAMENTE", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AvancarDatasAutomaticamente { get; set; }
    }
}
