using Dominio.Interfaces.Embarcador.Integracao;
using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_INTEGRACAO", EntityName = "PedidoIntegracao", Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao", NameType = typeof(PedidoIntegracao))]
    public class PedidoIntegracao : EntidadeBase, IIntegracaoComArquivo<PedidoIntegracaoArquivo>, IEquatable<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracao>
    {
        public PedidoIntegracao() { }
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "PEI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeArquivo", Column = "PEI_NOME_ARQUIVO", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string NomeArquivo { get; set; }

        /// <summary>
        /// Indica que inciou a tentativa de envio externa (FTP, E-mail, etc).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IniciouConexaoExterna", Column = "PEI_INICIOU_CONEXAO_EXTERNA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IniciouConexaoExterna { get; set; }

        /// <summary>
        /// Se tiver o tempo informado faz o reenvio automaticamente da integração após o tempo determinado (somente quando a tentativa anterior estiver integrada).
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "ReenviarAutomaticamenteOutraVezAposMinutos", Column = "PEI_REENVIAR_AUTOMATICAMENTE_APOS_MINUTOS", TypeType = typeof(int), NotNull = false)]
        public virtual int ReenviarAutomaticamenteOutraVezAposMinutos { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "LayoutEDI", Column = "LAY_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual LayoutEDI LayoutEDI { get; set; }

        /// <summary>
        /// Se o edi é do transportador armazena aqui pois pode usar esse dado como parametro para algumas regras.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "PEI_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoIntegracao", Column = "TPI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cargas.TipoIntegracao TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Tentativas", Column = "PEI_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int Tentativas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEnvio", Column = "PEI_DATA_ENVIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEI_MENSAGEM_RETORNO", Type = "StringClob", NotNull = false)]
        public virtual string ProblemaIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "PEI_CODIGO_INTEGRACAO_INTEGRADORA", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoIntegracaoIntegradora { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_INTEGRACAO_PEDIDO_INTEGRACAO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PEI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "PedidoIntegracaoArquivo", Column = "PIA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Pedidos.PedidoIntegracaoArquivo> ArquivosTransacao { get; set; }

        /// <summary>
        /// Indica que é uma integracao para cancelamento.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IntegracaoCancelamento", Column = "PEI_INTEGRACAO_CANCELAMENTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool IntegracaoCancelamento { get; set; } = false;

        public virtual string Descricao
        {
            get
            {
                return this.LayoutEDI?.Descricao ?? DescricaoTipoIntegracao;
            }
        }
        public virtual string DescricaoSituacaoIntegracao
        {
            get
            {
                switch (this.SituacaoIntegracao)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao:
                        return "Aguardando Integração";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgRetorno:
                        return "Aguardando Retorno";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado:
                        return "Integrado";
                    case ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao:
                        return "Problemas com a Integração";
                    default:
                        return "";
                }
            }
        }

        public virtual string DescricaoTipoIntegracao
        {
            get
            {
                return TipoIntegracao?.DescricaoTipo ?? string.Empty;
            }
        }

        public virtual bool Equals(PedidoIntegracao other)
        {
            if (other.Codigo == this.Codigo)
                return true;
            else
                return false;
        }

        public virtual DateTime DataIntegracao
        {
            get
            {
                return this.DataEnvio ?? DateTime.Now;
            }
            set
            {
                this.DataEnvio = value;
            }
        }
    }
}
