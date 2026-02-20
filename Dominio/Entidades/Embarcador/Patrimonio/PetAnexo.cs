using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.Entidades.Embarcador.Patrimonio
{
	[NHibernate.Mapping.Attributes.Class(0, Table = "T_PET_ANEXO", EntityName = "PetAnexo", Name = "Dominio.Entidades.Embarcador.Patrimonio.Pet", NameType = typeof(PetAnexo))]
	public class PetAnexo : EntidadeBase
	{
		[NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PAN_CODIGO")]
		[NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
		public virtual int Codigo { get; set; }

		[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pet", Column = "PET_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
		public virtual Pet Pet { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "PAN_DESCRICAO", TypeType = typeof(string), Length = 1000, NotNull = false)]
		public virtual string Descricao { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "PAN_NOME_ARQUIVO", TypeType = typeof(string), Length = 500, NotNull = false)]
		public virtual string NomeArquivo { get; set; }

		[NHibernate.Mapping.Attributes.Property(0, Name = "GuidArquivo", Column = "PAN_GUID_ARQUIVO", TypeType = typeof(string), Length = 40, NotNull = false)]
		public virtual string GuidArquivo { get; set; }
	}
}
