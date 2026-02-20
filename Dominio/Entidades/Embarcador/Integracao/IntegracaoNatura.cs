using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Integracao
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_NATURA", EntityName = "IntegracaoNatura", Name = "Dominio.Entidades.Embarcador.Cargas.IntegracaoNatura", NameType = typeof(IntegracaoNatura))]
    public class IntegracaoNatura : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "INA_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConsulta", Column = "INA_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroNumero", Column = "INA_PARAMETRO_NUMERO", TypeType = typeof(long), NotNull = false)]
        public virtual long? ParametroNumero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataInicial", Column = "INA_PARAMETRO_DATA_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? ParametroDataInicial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ParametroDataFinal", Column = "INA_PARAMETRO_DATA_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? ParametroDataFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "INA_PROTOCOLO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_REQUISICAO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoRequisicao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ArquivoIntegracao", Column = "ARI_CODIGO_RESPOSTA", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao ArquivoResposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "INA_RETORNO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "INA_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoNatura), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoNatura Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "INA_TIPO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoNatura Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "SubIntegracoes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_INTEGRACAO_NATURA_SUBINTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "INA_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "IntegracaoNatura", ForeignKey = "INA_CODIGO", Column = "INA_CODIGO_SUBINTEGRACAO")]
        public virtual ICollection<IntegracaoNatura> SubIntegracoes { get; set; }
    }
}
