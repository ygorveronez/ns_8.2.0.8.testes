namespace Dominio.Entidades.Embarcador.Pessoas
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_GRUPO_PESSOA_TIPO_OCORRENCIA", EntityName = "GrupoPessoaTipoOcorrencia", Name = "Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia", NameType = typeof(Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia))]
    public class GrupoPessoaTipoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "TTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "TTO_GRUPO_PESSOA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual GrupoPessoas GrupoPessoa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "TTO_TIPO_OCORRENCIA", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "TTO_CODIGO_INTEGRACAO", TypeType = typeof(string), Length = 50, NotNull = true)]
        public virtual string CodigoIntegracao { get; set; }
    }
}
