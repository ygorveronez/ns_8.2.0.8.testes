using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Entidades.Embarcador.Integracao
{
    /// <summary>
    /// Entidade que guarda dados de um pedido esperando intergração. Criado inicialmente para a integraçao com a VTEX, 
    /// mas pode ser usado para outras se necessário com adaptações.
    /// </summary>
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_AGUARDADO_INTEGRACAO", EntityName = "PedidoAguardandoIntegracao", Name = "Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao", NameType = typeof(PedidoAguardandoIntegracao))]
    public class PedidoAguardandoIntegracao : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int64", Column = "PAI_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual long Codigo { get; set; }

        /// <summary>
        /// De qual filial o pedido é
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Filiais.Filial Filial { get; set; }

        /// <summary>
        /// Identificador para integração. Na VTEX o campo se chama orderId.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "IdIntegracao", Column = "PAI_ORDER_ID", TypeType = typeof(string), Length = 300, NotNull = false)]
        public virtual string IdIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoIntegracao", Column = "PAI_SITUACAO_INTEGRACAO", TypeType = typeof(SituacaoPedidoAguardandoIntegracao), NotNull = true)]
        public virtual SituacaoPedidoAguardandoIntegracao SituacaoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracao", Column = "PAI_TIPO_INTEGRACAO", TypeType = typeof(TipoIntegracao), NotNull = false)]
        public virtual TipoIntegracao? TipoIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroTentativas", Column = "PAI_NUMERO_TENTATIVAS", TypeType = typeof(int), NotNull = false)]
        public virtual int NumeroTentativas { get; set; }

        /// <summary>
        /// Informação complementar do pedido. Antigamente era o MotivoRejeicao, mas agora estou usando também para dar informações de outras situações
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Informacao", Column = "PAI_MOTIVO_REJEICAO", TypeType = typeof(string), Length = 500, NotNull = false)]
        public virtual string Informacao { get; set; }

        /// <summary>
        /// A data da criação do pedido que vem da integração
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAI_DATA_CRIACAO_PEDIDO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataCriacaoPedido { get; set; }

        /// <summary>
        /// Data que essa entidade foi criada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAI_CREATED_AT", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime CreatedAt { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroCarga", Column = "PAI_NUMERO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ObjetoPedido", Column = "PAI_OBJETO_PEDIDO", TypeType = typeof(string), Length = 10000, NotNull = false)]
        public virtual string ObjetoPedido { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "ArquivosTransacao", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_PEDIDO_AGUARDADO_INTEGRACAO_ARQUIVO_ARQUIVO")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "PAI_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "CargaCTeIntegracaoArquivo", Column = "CCA_CODIGO")]
        public virtual ICollection<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> ArquivosTransacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoIntegracaoEmillenium", Column = "PAI_TIPO_INTEGRACAO_EMILLENIUM", TypeType = typeof(Enumeradores.TipoIntegracaoEmillenium), NotNull = false)]
        public virtual Enumeradores.TipoIntegracaoEmillenium? TipoIntegracaoEmillenium { get; set; }

        /// <summary>
        /// Data que essa entidade foi pesquisada e retornada na lista de pedidos
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAI_DATA_PESQUISA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPesquisa { get; set; }

        /// <summary>
        /// Data de embarque do pedido retornado na lista
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAI_DATA_EMBARQUE_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataEmbarquePedido { get; set; }

        /// <summary>
        /// Ultima Data de embarque da lista de pedidos retornada pela data pesquisada
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Column = "PAI_ULTIMA_DATA_EMBARQUE_LISTA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? UltimaDataEmbarqueLista { get; set; }
    }
}
