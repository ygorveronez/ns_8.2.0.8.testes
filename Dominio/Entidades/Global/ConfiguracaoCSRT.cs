namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CONFIGURACAO_CSRT", EntityName = "ConfiguracaoCSRT", Name = "Dominio.Entidades.ConfiguracaoCSRT", NameType = typeof(ConfiguracaoCSRT))]
    public class ConfiguracaoCSRT : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CSR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Estado", Column = "UF_SIGLA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Estado Estado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "idCSRT", Column = "CSR_ID_CSRT", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string idCSRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSRT", Column = "CSR_CSRT", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CSRT { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitaProducao", Column = "CSR_HABILITA_PRODUCAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitaProducao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "HabilitaHomologacao", Column = "CSR_HABILITA_HOMOLOGACAO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool HabilitaHomologacao { get; set; }
    }
}
