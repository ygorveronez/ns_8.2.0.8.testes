namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_FILTRO_PERIODO", EntityName = "FiltroPeriodo", Name = "Dominio.Entidades.Embarcador.Ocorrencias.FiltroPeriodo", NameType = typeof(FiltroPeriodo))]
    public class FiltroPeriodo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OFP_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_REMENTENTE", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Remetente { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_DESTINATARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Destinatario { get; set; }
    }
}
