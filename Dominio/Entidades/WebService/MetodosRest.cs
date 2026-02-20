
namespace Dominio.Entidades.WebService
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_METODO_REST", EntityName = "MetodosRest", Name = "Dominio.Entidades.WebService.MetodosRest", NameType = typeof(MetodosRest))]
    public class MetodosRest : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MER_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeMetodo", Column = "MER_NOME_METODO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NomeMetodo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "GeraLog", Column = "MER_GERA_LOG", TypeType = typeof(bool), NotNull = false)]
        public virtual bool GeraLog { get; set; }


    }
}
