using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.SuperApp
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_CHECKLIST_SUPER_APP_ETAPA", EntityName = "ChecklistSuperAppEtapa", Name = "Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperAppEtapa", NameType = typeof(ChecklistSuperAppEtapa))]
    public class ChecklistSuperAppEtapa : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "CSE_TIPO", TypeType = typeof(TipoEtapaChecklistSuperApp), NotNull = true)]
        public virtual TipoEtapaChecklistSuperApp Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEvidencia", Column = "CSE_TIPO_EVIDENCIA", TypeType = typeof(TipoEtapaChecklistSuperApp), NotNull = false)]
        public virtual TipoEvidenciaSuperApp TipoEvidencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Titulo", Column = "CSE_TITULO", TypeType = typeof(string), NotNull = true, Length = 100)]
        public virtual string Titulo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "CSE_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "CSE_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Obrigatorio", Column = "CSE_OBRIGATORIO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Obrigatorio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IdSuperApp", Column = "CSE_ID_SUPER_APP", TypeType = typeof(string), NotNull = false, Length = 24)]
        public virtual string IdSuperApp { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Configuracoes", Column = "CSE_CONFIGURACOES", Type = "StringClob", NotNull = false)]
        public virtual string Configuracoes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ChecklistSuperApp", Column = "CSA_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.SuperApp.ChecklistSuperApp ChecklistSuperApp { get; set; }





    }
}