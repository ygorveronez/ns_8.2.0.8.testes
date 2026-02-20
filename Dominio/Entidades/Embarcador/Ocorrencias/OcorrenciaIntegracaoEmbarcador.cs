using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_INTEGRACAO_EMBARCADOR", EntityName = "OcorrenciaIntegracaoEmbarcador", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador", NameType = typeof(OcorrenciaIntegracaoEmbarcador))]
    public class OcorrenciaIntegracaoEmbarcador : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "OIE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_DESCRICAO_OCORRENCIA_EMBARCADOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaOcorrencia", Column = "COC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia CargaOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "GrupoPessoas", Column = "GRP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_OCORRENCIA_INTEGRACAO_EMBARCADOR_CTE")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OIE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO")]
        public virtual ICollection<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OcorrenciaCancelamento", Column = "CAO_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento OcorrenciaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_DATA_CONSULTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_PROTOCOLO", TypeType = typeof(int), NotNull = true)]
        public virtual int Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_PROTOCOLO_CARGA", TypeType = typeof(int), NotNull = true)]
        public virtual int ProtocoloCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_NUMERO_OCORRENCIA_EMBARCADOR", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroOcorrenciaEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_DATA_CORRENCIA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_OBSERVACAO_EMBARCADOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string ObservacaoEmbarcador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_CODIGO_INTEGRACAO_TIPOOCORRENCIA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoTipoOcorrencia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_SITUACAO", TypeType = typeof(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador), NotNull = true)]
        public virtual Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_MENSAGEM", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_PROTOCOLO_CANCELAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? ProtocoloCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_DATA_CANCELAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_MOTIVO_CANCELAMENTO", TypeType = typeof(string), Length = 250, NotNull = false)]
        public virtual string MotivoCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OIE_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Cancelamento { get; set; }



    }
}
