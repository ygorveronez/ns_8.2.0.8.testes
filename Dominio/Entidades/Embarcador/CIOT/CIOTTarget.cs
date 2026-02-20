namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_TARGET", EntityName = "CIOTTarget", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTTarget", NameType = typeof(CIOTTarget))]
    public class CIOTTarget : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_USUARIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_SENHA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_TOKEN", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Token { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_URL_WEB_SERVICE", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string URLWebService { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_ASSOCIAR_CARTAO_MOTORISTA_TRANSPORTADOR", TypeType = typeof(bool), NotNull = false)]
        public virtual bool AssociarCartaoMotoristaTransportador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_CONSULTAR_CARTAO_MOTORISTA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool ConsultarCartaoMotorista { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTA_UTILIZAR_CIOT_TARGET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool UtilizarCiotTarget { get; set; }

        public virtual string Descricao
        {
            get { return Usuario; }
        }
    }
}
