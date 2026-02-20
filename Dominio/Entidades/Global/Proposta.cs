using System;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PROPOSTA", EntityName = "Proposta", Name = "Dominio.Entidades.Proposta", NameType = typeof(Proposta))]
    public class Proposta : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "PRO_NUMERO", TypeType = typeof(int), NotNull = false)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataLancamento", Column = "PRO_DATA_LANCAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime DataLancamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Data", Column = "PRO_DATA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime Data { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Nome", Column = "PRO_NOME", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Nome { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Email", Column = "PRO_EMAIL", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Email { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Telefone", Column = "PRO_FONE", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string Telefone { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoColeta", Column = "TPC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoColeta TipoColeta { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ModalProposta", Column = "PRO_MODAL", TypeType = typeof(Dominio.Enumeradores.ModalProposta), NotNull = false)]
        public virtual Dominio.Enumeradores.ModalProposta ModalProposta { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Peso", Column = "PRO_PESO", TypeType = typeof(decimal), Scale = 6, Precision = 18, NotNull = false)]
        public virtual decimal Peso { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - TRUCK
        /// 02 - TOCO
        /// 03 - CAVALO
        /// 04 - VAN
        /// 05 - UTILITARIO
        /// 07 - VUC
        /// 08 - 3/4
        /// 09 - CARRETA
        /// 06 - OUTROS
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoVeiculo", Column = "PRO_TIPO_VEICULO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoVeiculo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCarga", Column = "ATC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual TipoCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Volumes", Column = "PRO_VOLUMES", TypeType = typeof(int), NotNull = false)]
        public virtual int Volumes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Dimensoes", Column = "PRO_DIMENSOES", TypeType = typeof(string), Length = 30, NotNull = false)]
        public virtual string Dimensoes { get; set; }

        /// <summary>
        /// 00 - NAO APLICADO
        /// 01 - ABERTA
        /// 02 - FECHADA / BAU
        /// 03 - GRANEL
        /// 04 - PORTA CONTAINER
        /// 05 - SIDER
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoCarroceria", Column = "PRO_TIPO_CARROCERIA", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string TipoCarroceria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Rastreador", Column = "PRO_RASTREADOR", TypeType = typeof(Dominio.Enumeradores.OpcaoSimNao), NotNull = false)]
        public virtual Dominio.Enumeradores.OpcaoSimNao Rastreador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "PRO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Origem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "PRO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade Destino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "PRO_CLIENTE_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "PRO_CLIENTE_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente ClienteDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DiasValidade", Column = "PRO_DIAS_VALIDADE", TypeType = typeof(int), NotNull = false)]
        public virtual int? DiasValidade { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoCustosAdicionais", Column = "PRO_CUSTOS_ADICIONAIS", Type = "StringClob", NotNull = false)]
        public virtual string TextoCustosAdicionais { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoFormaCobranca", Column = "PRO_FORMA_COBRANCA", Type = "StringClob", NotNull = false)]
        public virtual string TextoFormaCobranca { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TextoCTRN", Column = "PRO_CTRN", Type = "StringClob", NotNull = false)]
        public virtual string TextoCTRN { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacoes", Column = "PRO_OBSERVACOES", TypeType = typeof(string), Length = 3000, NotNull = false)]
        public virtual string Observacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorMercadoria", Column = "PRO_VALOR_MERCADORIA", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal ValorMercadoria { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UnidadeMonetaria", Column = "PRO_UNIDADE_MONETARIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string UnidadeMonetaria { get; set; }
        public virtual string DescricaoTipoVeiculo
        {
            get
            {
                switch (TipoVeiculo)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Truck";
                    case "02":
                        return "Toco";
                    case "03":
                        return "Cavalo";
                    case "04":
                        return "Van";
                    case "05":
                        return "Utilitário";
                    case "06":
                        return "Outros";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoTipoCarroceria
        {
            get
            {
                switch (TipoCarroceria)
                {
                    case "00":
                        return "Não Aplicado";
                    case "01":
                        return "Aberta";
                    case "02":
                        return "Fechada/Baú";
                    case "03":
                        return "Granel";
                    case "04":
                        return "Porta Container";
                    case "05":
                        return "Sider";
                    default:
                        return "";
                }
            }
        }
        public virtual string DescricaoModalProposta
        {
            get
            {
                switch (ModalProposta)
                {
                    case Enumeradores.ModalProposta.Rodoviario:
                        return "Rodoviário";
                    case Enumeradores.ModalProposta.Rodoaereo:
                        return "Rodoaereo";
                    case Enumeradores.ModalProposta.Outros:
                        return "Outros";
                    default:
                        return "";
                }
            }
        }
    }
}