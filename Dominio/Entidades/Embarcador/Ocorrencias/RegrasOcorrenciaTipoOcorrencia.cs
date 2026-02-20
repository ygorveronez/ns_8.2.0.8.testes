namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_REGRA_OCORRENCIA_TIPO_OCORRENCIA", EntityName = "RegrasOcorrenciaTipoOcorrencia", Name = "Dominio.Entidades.Embarcador.Ocorrencias.RegrasOcorrenciaTipoOcorrencia", NameType = typeof(RegrasOcorrenciaTipoOcorrencia))]
    public class RegrasOcorrenciaTipoOcorrencia : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "RTO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RegrasAutorizacaoOcorrencia", Column = "RAO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual RegrasAutorizacaoOcorrencia RegrasAutorizacaoOcorrencia { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Ordem", Column = "RTO_ORDEM", TypeType = typeof(int), NotNull = true)]
        public virtual int Ordem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Condicao", Column = "RTO_CONDICAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.CondicaoAutorizaoOcorrencia Condicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Juncao", Column = "RTO_JUNCAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.JuncaoAutorizaoOcorrencia Juncao { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeOcorrenciaDeCTe", Column = "OCO_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoDeOcorrenciaDeCTe TipoDeOcorrencia { get; set; }

    }

}

