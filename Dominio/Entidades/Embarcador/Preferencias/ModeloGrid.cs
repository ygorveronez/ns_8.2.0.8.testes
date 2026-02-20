using System;

namespace Dominio.Entidades.Embarcador.Preferencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MODELO_GRID", EntityName = "ModeloGrid", Name = "Dominio.Entidades.Embarcador.Preferencias.ModeloGrid", NameType = typeof(ModeloGrid))]
	public class ModeloGrid : EntidadeBase
	{
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MDG_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "MDG_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModeloPadrao", Column = "MDG_MODELO_PADRAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ModeloPadrao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioCadastro { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCadastro", Column = "MDG_DATA_CADASTRO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCadastro { get; set; }
    }
}
