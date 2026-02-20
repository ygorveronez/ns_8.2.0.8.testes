using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE_AVERBACAO", EntityName = "AverbacaoNFSe", Name = "Dominio.Entidades.AverbacaoNFSe", NameType = typeof(AverbacaoNFSe))]
    public class AverbacaoNFSe : EntidadeBase
    {
        public AverbacaoNFSe() { }

        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "AVN_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NFSe", Column = "NFSE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NFSe NFSe { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Averbacao", Column = "AVN_AVERBACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Averbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Protocolo", Column = "AVN_PROTOCOLO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Protocolo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRetorno", Column = "AVN_CODIGO_RETORNO", TypeType = typeof(string), Length = 10, NotNull = false)]
        public virtual string CodigoRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemRetorno", Column = "AVN_MENSAGEM_RETORNO", TypeType = typeof(string), Length = 2000, NotNull = false)]
        public virtual string MensagemRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIntegracao", Column = "AVN_CODIGO_INTEGRACAO", TypeType = typeof(int), NotNull = false)]
        public virtual int CodigoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataRetorno", Column = "AVN_DATA_RETORNO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetorno { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tipo", Column = "AVN_TIPO", TypeType = typeof(Enumeradores.TipoAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.TipoAverbacaoCTe Tipo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "AVN_STATUS", TypeType = typeof(Enumeradores.StatusAverbacaoCTe), NotNull = true)]
        public virtual Enumeradores.StatusAverbacaoCTe Status { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegradoraAverbacao", Column = "AVN_INTEGRADORA", TypeType = typeof(Dominio.Enumeradores.IntegradoraAverbacao), NotNull = false)]
        public virtual Dominio.Enumeradores.IntegradoraAverbacao IntegradoraAverbacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoUsuario", Column = "AVN_CODIGO_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoUsuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Usuario", Column = "AVN_USUARIO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Senha", Column = "AVN_SENHA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Senha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Token", Column = "AVN_TOKEN", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string Token { get; set; }


        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVERBACAO_NFSE_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AverbacaoIntegracaoArquivo", Column = "AIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacaoCancelamento", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_AVERBACAO_NFSE_INTEGRACAO_ARQUIVO_CANCELAMENTO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "AVN_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "AverbacaoIntegracaoArquivo", Column = "AIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo> ArquivosTransacaoCancelamento { get; set; }

        public virtual string DescricaoTipo
        {
            get
            {
                switch (this.Tipo)
                {
                    case Enumeradores.TipoAverbacaoCTe.Autorizacao:
                        return "Autorização";
                    case Enumeradores.TipoAverbacaoCTe.Cancelamento:
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
                    case Enumeradores.StatusAverbacaoCTe.Rejeicao:
                        return "Rejeição";
                    case Enumeradores.StatusAverbacaoCTe.Sucesso:
                        return "Averbado";
                    case Enumeradores.StatusAverbacaoCTe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusAverbacaoCTe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusAverbacaoCTe.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusAverbacaoCTe.AgEmissao:
                        return "Ag. Emissão";
                    case Enumeradores.StatusAverbacaoCTe.AgCancelamento:
                        return "Ag. Cancelamento";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string DescricaoSeguradoraAverbacao
        {
            get
            {
                switch (this.IntegradoraAverbacao)
                {
                    case Enumeradores.IntegradoraAverbacao.NaoDefinido:
                        return "Não Definido";
                    case Enumeradores.IntegradoraAverbacao.ATM:
                        return "ATM";
                    case Enumeradores.IntegradoraAverbacao.Quorum:
                        return "Quoruom";
                    case Enumeradores.IntegradoraAverbacao.PortoSeguro:
                        return "Porto Seguro";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
