using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Ocorrencias
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_OCORRENCIA_CTE_INTEGRACAO_LOTE", EntityName = "OcorrenciaCTeIntegracaoLote", Name = "Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoLote", NameType = typeof(OcorrenciaCTeIntegracaoLote))]
    public class OcorrenciaCTeIntegracaoLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "OCL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "OCL_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "OCL_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "OCL_DATA_ULTIMA_CONSULTA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "OCL_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "OCL_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "OCL_PROBLEMA_INTEGRACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoConsultaIntegradora", Column = "OCL_CODIGO_CONSULTA_INTEGRADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoConsultaIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaIntegracaoNatura", Column = "CNA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura IntegracaoNatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_CARGA_CTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "OCL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracao", Column = "CCI_CODIGO")]
        public virtual ICollection<OcorrenciaCTeIntegracao> CTes { get; set; }

        public virtual string DescricaoSituacaoIntegracao
        {
            get
            {
                switch (SituacaoIntegracao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao:
                        return "Aguardando Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado:
                        return "Integrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao:
                        return "Falha ao Integrar";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
