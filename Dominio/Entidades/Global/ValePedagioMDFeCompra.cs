using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_MDFE_VALE_PEDAGIO_COMPRA", EntityName = "ValePedagioMDFeCompra", Name = "Dominio.Entidades.ValePedagioMDFeCompra", NameType = typeof(ValePedagioMDFeCompra))]
    public class ValePedagioMDFeCompra : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "MVC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ManifestoEletronicoDeDocumentosFiscais", Column = "MDF_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ManifestoEletronicoDeDocumentosFiscais MDFe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IBGEInicio", Column = "MVC_IBGE_INICIO", TypeType = typeof(int), NotNull = false)]
        public virtual int IBGEInicio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IBGEFim", Column = "MVC_IBGE_FIM", TypeType = typeof(int), NotNull = false)]
        public virtual int IBGEFim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJFornecedor", Column = "MVC_CNPJ_FORNECEDOR", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJFornecedor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CNPJResponsavel", Column = "MVC_CNPJ_RESPONSAVEL", TypeType = typeof(string), Length = 14, NotNull = false)]
        public virtual string CNPJResponsavel { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroComprovante", Column = "MVC_NUMERO_COMPROVANTE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string NumeroComprovante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Valor", Column = "MVC_VALOR", TypeType = typeof(decimal), Scale = 2, Precision = 13, NotNull = false)]
        public virtual decimal Valor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCompra", Column = "MVC_TIPO_COMPRA", TypeType = typeof(Enumeradores.TipoCompraValePedagio), NotNull = false)]
        public virtual Enumeradores.TipoCompraValePedagio TipoCompra { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "Integradora", Column = "MVC_INTEGRADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraValePedagio Integradora { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoUsuario", Column = "MVC_INTEGRACAO_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IntegracaoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoSenha", Column = "MVC_INTEGRACAO_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IntegracaoSenha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoToken", Column = "MVC_INTEGRACAO_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IntegracaoToken { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "MVC_TIPO", TypeType = typeof(Enumeradores.TipoIntegracaoValePedagio), NotNull = true)]
        public virtual Enumeradores.TipoIntegracaoValePedagio Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "MVC_STATUS", TypeType = typeof(Enumeradores.StatusIntegracaoValePedagio), NotNull = true)]
        public virtual Enumeradores.StatusIntegracaoValePedagio Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "MVC_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ComprarRetorno", Column = "MVC_COMPRAR_RETORNO", TypeType = typeof(Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Enumeradores.OpcaoSimNao ComprarRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "MVC_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TentativaReenvio", Column = "MVC_TENTATIVA_REENVIO", TypeType = typeof(int), NotNull = false)]
        public virtual int TentativaReenvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeEixos", Column = "MVC_QUANTIDADE_EIXOS", TypeType = typeof(int), NotNull = false)]
        public virtual int QuantidadeEixos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoRota", Column = "MVC_DESCRICAO_ROTA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string DescricaoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UrlIntegracaoRest", Column = "MVC_URL_INTEGRACAO_REST", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string UrlIntegracaoRest { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEmissaoValePedagioANTT", Column = "MVC_CODIGO_EMISSAO_VALE_PEDAGIO_ANTT", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string CodigoEmissaoValePedagioANTT { get; set; }

        public virtual string DescricaoIntegradora
        {
            get
            {
                switch (this.Integradora)
                {
                    case Enumeradores.IntegradoraValePedagio.Target:
                        return "Target";
                    case Enumeradores.IntegradoraValePedagio.SemParar:
                        return "Sem Parar";
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
                    case Enumeradores.TipoIntegracaoValePedagio.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoIntegracaoValePedagio.Cancelamento:
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
                    case Enumeradores.StatusIntegracaoValePedagio.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusIntegracaoValePedagio.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusIntegracaoValePedagio.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusIntegracaoValePedagio.PendenteCancelamento:
                        return "Pendente Cancelamento";
                    case Enumeradores.StatusIntegracaoValePedagio.RejeicaoCancelamento:
                        return "Rejeição Cancelamento";
                    case Enumeradores.StatusIntegracaoValePedagio.RejeicaoCompra:
                        return "Rejeição Compra";
                    case Enumeradores.StatusIntegracaoValePedagio.RotaSemCusto:
                        return "Rota sem Custo";
                    case Enumeradores.StatusIntegracaoValePedagio.SemRota:
                        return "Sem rota";
                    case Enumeradores.StatusIntegracaoValePedagio.Sucesso:
                        return "Sucesso";
                    case Enumeradores.StatusIntegracaoValePedagio.ProblemaNaIntegracao:
                        return "Problema na integração";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
