using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_CTE_INTEGRACAO_RETORNO", EntityName = "CTeIntegracaoRetorno", Name = "Dominio.Entidades.CTeIntegracaoRetorno", NameType = typeof(CTeIntegracaoRetorno))]
    public class CTeIntegracaoRetorno : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CIR_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "CIR_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "CIR_SITUACAO", TypeType = typeof(Enumeradores.SituacaoCTeIntegracaoRetorno), NotNull = false)]
        public virtual Enumeradores.SituacaoCTeIntegracaoRetorno SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "CIR_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProblemaIntegracao", Column = "CIR_PROBLEMA_INTEGRACAO", Type = "StringClob", NotNull = false)]
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
