using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_LSTRANSLOG", EntityName = "IntegracaoLsTranslog", Name = "Dominio.Entidades.IntegracaoLsTranslog", NameType = typeof(IntegracaoLsTranslog))]
    public class IntegracaoLsTranslog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILS_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ConhecimentoDeTransporteEletronico", Column = "CON_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ConhecimentoDeTransporteEletronico CTe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "XMLNotaFiscalEletronica", Column = "XML_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual XMLNotaFiscalEletronica NFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ILS_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEnvio", Column = "ILS_STATUS_ENVIO", TypeType = typeof(ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog StatusEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoEnvio", Column = "ILS_OBSERVACAO_ENVIO", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusConsulta", Column = "ILS_STATUS_CONSULTA", TypeType = typeof(ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog StatusConsulta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObservacaoRetorno", Column = "ILS_OBSERVACAO_RERTORNO", Type = "StringClob", NotNull = false)]
        public virtual string ObservacaoRetorno { get; set; }

        public virtual ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog TipoDocumento
        {
            get
            {
                if (this.CTe != null)
                    return ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.CTe;
                else if (this.NFSe != null)
                    return ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFSe;
                else
                    return ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFe;
            }
        }

        public virtual string DescricaoTipoDocumento
        {
            get
            {
                switch (TipoDocumento)
                {
                    case ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.CTe:
                        return "CTe";
                    case ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFSe:
                        return "NFSe";
                    case ObjetosDeValor.Enumerador.TipoDocumentoLsTranslog.NFe:
                        return "NFe";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoStatusEnvio
        {
            get
            {
                switch (StatusEnvio)
                {
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro:
                        return "Erro";
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso:
                        return "Sucesso";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoStatusConsulta
        {
            get
            {
                switch (StatusConsulta)
                {
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Erro:
                        return "Erro";
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Pendente:
                        return "Pendente";
                    case ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog.Sucesso:
                        return "Sucesso";
                    default:
                        return "";
                }
            }
        }
    }
}
