namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_REPOM", EntityName = "CIOTRepom", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTRepom", NameType = typeof(CIOTRepom))]
    public class CIOTRepom : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CRP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CODIGO_CLIENTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_ASSINATURA_DIGITAL", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string AssinaturaDigital { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CNPJ_INTEGRADOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJIntegrador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CODIGO_MOVIMENTO_INSS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CodigoMovimentoINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CODIGO_MOVIMENTO_SEST", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CodigoMovimentoSEST { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CODIGO_MOVIMENTO_SENAT", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CodigoMovimentoSENAT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CRP_CODIGO_MOVIMENTO_IR", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CodigoMovimentoIR { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
