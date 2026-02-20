namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_TRUCKPAD", EntityName = "CIOTTruckPad", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTTruckPad", NameType = typeof(CIOTTruckPad))]
    public class CIOTTruckPad : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_URL_TOKEN", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLTruckPadToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_URL", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URLTruckPad { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_USUARIO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string UsuarioTruckPad { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_SENHA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string SenhaTruckPad { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTP_OFFICE_ID", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string OfficeID { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
