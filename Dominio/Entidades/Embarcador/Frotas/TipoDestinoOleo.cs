namespace Dominio.Entidades.Embarcador.Frotas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_TIPO_DESTINO_OLEO_ABASTECIMENTO", EntityName = "TipoDestinoOleo", Name = "Dominio.Entidades.Embarcador.Frotas.TipoDestinoOleo", NameType = typeof(TipoDestinoOleo))]
    public class TipoDestinoOleo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "DOA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "DOA_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = true)]
        public virtual string Descricao { get; set; }
    }
}