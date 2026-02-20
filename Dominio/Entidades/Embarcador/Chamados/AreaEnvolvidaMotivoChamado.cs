namespace Dominio.Entidades.Embarcador.Chamados
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_AREA_ENVOLVIDA_MOTIVO_CHAMADO", EntityName = "AreaEnvolvidaMotivoChamado", Name = "Dominio.Entidades.Embarcador.Chamados.AreaEnvolvidaMotivoChamado", NameType = typeof(AreaEnvolvidaMotivoChamado))]
	public class AreaEnvolvidaMotivoChamado : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AEM_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "AEM_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "AEM_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string CodigoIntegracao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Column = "AEM_STATUS", TypeType = typeof(bool))]
		public virtual bool Status { get; set; }

		#region Propriedades com Regras

		public virtual string DescricaoStatus
		{
			get { return Status ? "Ativo" : "Inativo"; }
		}

		#endregion
	}
}
