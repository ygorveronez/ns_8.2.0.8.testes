using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.GestaoPatio
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_CHECKLIST_IMPRESSAO", EntityName = "TipoChecklistImpressao", Name = "Dominio.Entidades.Embarcador.GestaoPatio.TipoChecklistImpressao", NameType = typeof(TipoChecklistImpressao))]
    public class TipoChecklistImpressao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TCI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Caminho", Column = "TCI_CAMINHO", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Caminho { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TCI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Column = "TCI_LAYOUT_CHECKLIST", TypeType = typeof(LayoutCheckList), NotNull = false)]
        public virtual LayoutCheckList LayoutCheckList { get; set; }
    }
}
