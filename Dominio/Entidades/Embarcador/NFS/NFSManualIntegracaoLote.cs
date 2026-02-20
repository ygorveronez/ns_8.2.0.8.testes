using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.NFS
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFS_MANUAL_INTEGRACAO_LOTE", EntityName = "NFSManualIntegracaoLote", Name = "Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoLote", NameType = typeof(NFSManualIntegracaoLote))]
    public class NFSManualIntegracaoLote : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NIL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "NIL_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRecebimento", Column = "NIL_DATA_RECEBIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRecebimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NIL_DATA_ULTIMA_CONSULTA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataUltimaConsultaRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "NIL_SITUACAO_INTEGRACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SistemaEmissor), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "NIL_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "NIL_PROBLEMA_INTEGRACAO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoConsultaIntegradora", Column = "NIL_CODIGO_CONSULTA_INTEGRADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoConsultaIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSManualIntegracaoNatura", Column = "NIN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.NFS.NFSManualIntegracaoNatura IntegracaoNatura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "CTes", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFS_MANUAL_CTE_INTEGRACAO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NIL_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "NFsManualCTeIntegracao", Column = "CCI_CODIGO")]
        public virtual ICollection<NFSManualCTeIntegracao> CTes { get; set; }

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
