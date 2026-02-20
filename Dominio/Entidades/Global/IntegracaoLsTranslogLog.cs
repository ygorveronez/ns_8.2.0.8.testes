using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_INTEGRACAO_LSTRANSLOG_LOG", EntityName = "IntegracaoLsTranslogLog", Name = "Dominio.Entidades.IntegracaoLsTranslogLog", NameType = typeof(IntegracaoLsTranslogLog))]
    public class IntegracaoLsTranslogLog : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ILL_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "IntegracaoLsTranslog", Column = "ILS_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual IntegracaoLsTranslog IntegracaoLsTranslog { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroNFe", Column = "ILL_NUMERO_NFE", TypeType = typeof(int), NotNull = true)]
        public virtual int NumeroNFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Identificador", Column = "ILL_IDENTIFICADOR", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Identificador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "ILL_DATA", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "ILL_TIPO", TypeType = typeof(ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Envio", Column = "ILL_ENVIO", Type = "StringClob", NotNull = false)]
        public virtual string Envio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Retorno", Column = "ILL_RETORNO", Type = "StringClob", NotNull = false)]
        public virtual string Retorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "ILL_STATUS", TypeType = typeof(ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog), NotNull = true)]
        public virtual ObjetosDeValor.Enumerador.StatusIntegracaoLsTranslog Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ILL_MENSAGEM", Type = "StringClob", NotNull = false)]        
        public virtual string Mensagem { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (Tipo)
                {
                    case ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Consulta:
                        return "Consulta";
                    case ObjetosDeValor.Enumerador.TipoIntegracaoLsTranslog.Envio:
                        return "Envio";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoStatus
        {
            get
            {
                switch (Status)
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
