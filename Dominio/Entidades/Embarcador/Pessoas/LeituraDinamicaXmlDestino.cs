namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEITURA_DINAMICA_XML_DESTINO", EntityName = "LeituraDinamicaXmlDestino", Name = "Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlDestino", NameType = typeof(LeituraDinamicaXmlDestino))]
    public class LeituraDinamicaXmlDestino : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LDD_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeCampo", Column = "LDD_NOME_CAMPO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NomeCampo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LDD_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }
    }
}