using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_INTEGRACAO_RETORNO", EntityName = "NFSeIntegracaoRetorno", Name = "Dominio.Entidades.NFSeIntegracaoRetorno", NameType = typeof(NFSeIntegracaoRetorno))]
    public class NFSeIntegracaoRetorno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "NIR_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "NIR_SITUACAO", TypeType = typeof(Enumeradores.SituacaoCTeIntegracaoRetorno), NotNull = false)]
        public virtual Enumeradores.SituacaoCTeIntegracaoRetorno SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "NIR_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "NIR_PROBLEMA_INTEGRACAO", Type = "StringClob", NotNull = false)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoIntegracao TipoIntegracao { get; set; }

        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual NFSe NFSe { get; set; }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.SituacaoIntegracao)
                {
                    case Enumeradores.SituacaoCTeIntegracaoRetorno.Aguardando:
                        return "Aguardando";
                    case Enumeradores.SituacaoCTeIntegracaoRetorno.Falha:
                        return "Falha";
                    case Enumeradores.SituacaoCTeIntegracaoRetorno.Sucesso:
                        return "Sucesso";
                    default:
                        return string.Empty;
                }
            }
        }


    }
}
