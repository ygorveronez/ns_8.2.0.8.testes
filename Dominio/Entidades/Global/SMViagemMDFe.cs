using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_SM_VIAGEM", EntityName = "SMViagemMDFe", Name = "Dominio.Entidades.SMViagemMDFe", NameType = typeof(SMViagemMDFe))]
    public class SMViagemMDFe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MSV_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Integradora", Column = "MSV_INTEGRADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraSM), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraSM Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MSV_TIPO", TypeType = typeof(Enumeradores.TipoIntegracaoSM), NotNull = true)]
        public virtual Enumeradores.TipoIntegracaoSM Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MSV_STATUS", TypeType = typeof(Enumeradores.StatusIntegracaoSM), NotNull = true)]
        public virtual Enumeradores.StatusIntegracaoSM Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "MSV_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracaoViagem", Column = "MSV_CODIGO_INTEGRACAO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIntegracaoViagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "MSV_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        public virtual string DescricaoIntegradora
        {
            get
            {
                switch (this.Integradora)
                {
                    case Enumeradores.IntegradoraSM.Trafegus:
                        return "Trafegus";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoIntegracaoSM.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoIntegracaoSM.Cancelamento:
                        return "Cancelamento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusIntegracaoSM.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusIntegracaoSM.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusIntegracaoSM.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusIntegracaoSM.PendenteCancelamento:
                        return "Pendente Cancelamento";
                    case Enumeradores.StatusIntegracaoSM.Sucesso:
                        return "Sucesso";
                    case Enumeradores.StatusIntegracaoSM.CancelamentoRejeitado:
                        return "Cancelamento Rejeitado";
                    case Enumeradores.StatusIntegracaoSM.Rejeitado:
                        return "Rejeitado";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
