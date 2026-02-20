using System;

namespace Dominio.Entidades.Embarcador.Documentos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GESTAO_DOCUMENTO_HISTORICO_CTE", EntityName = "GestaoDocumentoHistoricoCTe", Name = "Dominio.Entidades.Embarcador.Documentos.GestaoDocumentoHistoricoCTe", NameType = typeof(GestaoDocumentoHistoricoCTe))]
	public class GestaoDocumentoHistoricoCTe : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "GHC_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GestaoDocumento", Column = "GED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual GestaoDocumento GestaoDocumento { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.ConhecimentoDeTransporteEletronico CTe { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "GHC_DATA", TypeType = typeof(DateTime), NotNull = false)]
		public virtual DateTime Data { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Dominio.Entidades.Usuario Usuario { get; set; }

		public virtual string Descricao
		{
			get { return CTe?.Numero.ToString() ?? string.Empty; }
		}
	}
}