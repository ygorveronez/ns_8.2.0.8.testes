namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_RODOCRED", EntityName = "CIOTRodocred", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTRodocred", NameType = typeof(CIOTRodocred))]
    public class CIOTRodocred : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_ID_CLIENTE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string IDCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_LOGIN", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string Login { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_CHAVE_AUTENTICACAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string ChaveAutenticacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRC_URL", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string URL { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
