namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LEITURA_DINAMICA_XML_ORIGEM", EntityName = "LeituraDinamicaXmlOrigem", Name = "Dominio.Entidades.Embarcador.Pessoas.LeituraDinamicaXmlOrigem", NameType = typeof(LeituraDinamicaXmlOrigem))]
    public class LeituraDinamicaXmlOrigem : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LDO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDocumento", Column = "LDO_TIPO_DOCUMENTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento TipoDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LDO_DESCRICAO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "LDO_HABILITAR_TAG_FILHA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitarTagFilha { get; set; }
    }
}