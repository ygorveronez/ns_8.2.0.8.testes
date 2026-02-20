namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SEFAZ", EntityName = "Sefaz", Name = "Dominio.Entidades.Sefaz", NameType = typeof(Sefaz))]
    public class Sefaz : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SEF_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SEF_DESCRICAO", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoSefaz", Column = "SEF_TIPO", TypeType = typeof(Dominio.Enumeradores.TipoSefaz), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoSefaz TipoSefaz { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoAmbiente", Column = "SEF_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoSefaz), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRecepcao", Column = "SEF_URL_RECEPCAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlRecepcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRetRecepcao", Column = "SEF_URL_RET_RECEPCAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlRetRecepcao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlInutilizacao", Column = "SEF_URL_INUTILIZACAO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlInutilizacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlConsultaProtocolo", Column = "SEF_URL_CONSULTA_PROTOCOLO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlConsultaProtocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlStatusServico", Column = "SEF_URL_STATUS_SERVICO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlStatusServico { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRecepcaoEvento", Column = "SEF_URL_RECEPCAO_EVENTO", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlRecepcaoEvento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlRecepcaoOS", Column = "SEF_URL_RECEPCAO_OS", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlRecepcaoOS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlDistribuicaoDFe", Column = "SEF_URL_DISTRIBUICAO_DFE", TypeType = typeof(string), NotNull = false, Length = 1000)]
        public virtual string UrlDistribuicaoDFe { get; set; }
    }
}
