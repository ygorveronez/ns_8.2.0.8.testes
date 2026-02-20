namespace Dominio.Entidades.Embarcador.CIOT
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CIOT_BBC", EntityName = "CIOTBBC", Name = "Dominio.Entidades.Embarcador.CIOT.CIOTBBC", NameType = typeof(CIOTBBC))]
    public class CIOTBBC : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CBB_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConfiguracaoCIOT", Column = "CCT_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT ConfiguracaoCIOT { get; set; }

        public virtual string Descricao
        {
            get
            {
                return Codigo.ToString();
            }
        }
    }
}
