namespace Dominio.Entidades.Embarcador.Logistica
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_LOCAIS", EntityName = "Locais", Name = "Dominio.Entidades.Embarcador.Logistica.Locais", NameType = typeof(Locais))]
    public class Locais : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "LOC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "LOC_DESCRICAO", TypeType = typeof(string), Length = 100, NotNull = true)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoLocal), Column = "LOC_TIPO", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoLocal Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoArea", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoArea), Column = "LOC_TIPO_AREA", NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoArea TipoArea { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Area", Column = "LOC_AREA", Type = "StringClob", NotNull = false)]
        public virtual string Area { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "LOC_OBSERVACAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }
    }
}
