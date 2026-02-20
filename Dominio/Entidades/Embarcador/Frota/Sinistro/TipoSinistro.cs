namespace Dominio.Entidades.Embarcador.Frota.Sinistro
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_SINISTRO", EntityName = "TipoSinistro", Name = "Dominio.Entidades.Embarcador.Frota.Sinistro.TipoSinistro", NameType = typeof(TipoSinistro))]
   public class TipoSinistro : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TSI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class ="native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "TSI_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "TSI_STATUS", TypeType = typeof(bool), NotNull = true)]
        public virtual bool Status { get; set; }


    }
}
