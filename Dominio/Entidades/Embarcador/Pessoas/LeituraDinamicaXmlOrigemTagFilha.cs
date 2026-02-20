namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEITURA_DINAMICA_XML_ORIGEM_TAG_FILHA", EntityName = "LeituraDinamicaXmlOrigemTagFilha", Name = "Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigemTagFilha", NameType = typeof(LeituraDinamicaXmlOrigemTagFilha))]
    public class LeituraDinamicaXmlOrigemTagFilha : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LDF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LDF_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LeituraDinamicaXmlOrigem", Column = "LDO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LeituraDinamicaXmlOrigem LeituraDinamicaXmlOrigem { get; set; }
    }
}