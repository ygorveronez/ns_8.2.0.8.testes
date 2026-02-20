namespace Dominio.Entidades.Embarcador.Frota.Sinistro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRAVIDADE_SINISTRO", EntityName = "GravidadeSinistro", Name = "Dominio.Entidades.Embarcador.Frota.Sinistro.GravidadeSinistro", NameType = typeof(GravidadeSinistro))]
    public class GravidadeSinistro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TGS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TGS_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TGS_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }
    }
}
