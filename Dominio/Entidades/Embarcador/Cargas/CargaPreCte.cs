
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Cargas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CARGA_PRE_CTE", EntityName = "CargaPreCte", Name = "Dominio.Entidades.Embarcador.Cargas.CargaPreCte", NameType = typeof(CargaPreCte))]
    public class CargaPreCte : EntidadeBase
    {

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CPC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Carga", Column = "CAR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Carga Carga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCte", Column = "ADT_NUMERO_CTE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ChaveCte", Column = "ADT_CHAVE_CTE", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ChaveCte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusDocumento", Column = "ADT_STATUS", TypeType = typeof(StatusDocumento), NotNull = false)]
        public virtual StatusDocumento StatusDocumento { get; set; }
    }
}
