using System;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_ELECTROLUX_CONSULTA_LOG", EntityName = "IntegracaoElectroluxConsultaLog", Name = "Dominio.Entidades.Embarcador.Cargas.IntegracaoElectroluxConsultaLog", NameType = typeof(IntegracaoElectroluxConsultaLog))]
    public class IntegracaoElectroluxConsultaLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "INE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroIndentificador", Column = "INE_PARAMETRO_IDENTIFICADOR", TypeType = typeof(long), NotNull = false)]
        public virtual long? ParametroIndentificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataInicial", Column = "INE_PARAMETRO_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? ParametroDataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataFinal", Column = "INE_PARAMETRO_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? ParametroDataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoIdentificadorDocumento", Column = "INE_RETORNO_IDENTIFICADOR_DOCUMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string RetornoIdentificadorDocumento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "RetornoXML", Column = "INE_RETORNO_XML", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string RetornoXML { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "INA_RETORNO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "INE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoElectrolux), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoElectrolux Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "INE_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoElectrolux Tipo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "INE_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataConsulta { get; set; }

    }
}
