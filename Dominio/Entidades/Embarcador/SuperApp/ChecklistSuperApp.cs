using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.SuperApp
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CHECKLIST_SUPER_APP", EntityName = "ChecklistSuperApp", Name = "Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp", NameType = typeof(ChecklistSuperApp))]
    public class ChecklistSuperApp : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "CSA_TITULO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdSuperApp", Column = "CSA_ID_SUPER_APP", TypeType = typeof(string), NotNull = false, Length = 24)]
        public virtual string IdSuperApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoFluxo", Column = "CSA_TIPO_FLUXO", TypeType = typeof(TipoFluxoChecklistSuperApp), NotNull = true)]
        public virtual TipoFluxoChecklistSuperApp TipoFluxo { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Titulo;
            }
        }

    }
}