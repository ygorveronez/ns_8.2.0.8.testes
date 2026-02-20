using Dominio.Entidades.Embarcador.Frete;

namespace Dominio.Entidades.Embarcador.Transportadores
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TRANSPORTADOR_COMPONENTE_CTE_IMPORTADO", EntityName = "TransportadorComponenteCTeImportado", Name = "Dominio.Entidades.Embarcador.Transportadores.TransportadorComponenteCTeImportado", NameType = typeof(TransportadorComponenteCTeImportado))]
    public class TransportadorComponenteCTeImportado : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ComponenteFrete", Column = "CFR_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ComponenteFrete ComponenteFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TCI_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 150)]
        public virtual string Descricao { get; set; }
    }
}
