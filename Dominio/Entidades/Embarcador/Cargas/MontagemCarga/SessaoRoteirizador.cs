using System;

/// <summary>
/// Um pedido não pode pertencer a mais de uma sessão exceto se a mesma está com a situação cancelada.
/// </summary>
namespace Dominio.Entidades.Embarcador.Cargas.MontagemCarga
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_SESSAO_ROTEIRIZADOR", EntityName = "SessaoRoteirizador", Name = "Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador", NameType = typeof(SessaoRoteirizador))]
    public class SessaoRoteirizador : EntidadeBase, IEquatable<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SessaoRoteirizador>
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "SRO_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        /// <summary>
        /// Usuário responsável pela sessão de roteirização.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_USUARIO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        /// <summary>
        /// Filial de carregamento dos pedidos da sessão de roteirização.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        /// <summary>
        /// Data inicio dos pedidos para pesquisar e relacionar a sessão de roteirização.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicial", Column = "CAR_DATA_CARREGAMENTO_PEDIDO_INICIAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicial { get; set; }

        /// <summary>
        /// Data final dos pedidos para pesquisar e relacionar a sessão de roteirização.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFinal", Column = "CAR_DATA_CARREGAMENTO_PEDIDO_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFinal { get; set; }

        /// <summary>
        /// Contem a data de inicio da sessão de roteirização
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Inicio", Column = "SRO_DATA_INICIO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime Inicio { get; set; }
        /// <summary>
        /// Identifica o horário final da sessão de roteirização pelo usuário.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "Fim", Column = "SRO_DATA_FIM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? Fim { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SituacaoSessaoRoteirizador", Column = "SRO_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoSessaoRoteirizador SituacaoSessaoRoteirizador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Descricao", Column = "SRO_DESCRICAO", TypeType = typeof(string), Length = 4000, NotNull = false)]
        public virtual string Descricao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MontagemCarregamentoPedidoProduto", Column = "SRO_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool MontagemCarregamentoPedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoMontagemCarregamentoPedidoProduto", Column = "SRO_TIPO_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoMontagemCarregamentoPedidoProduto TipoMontagemCarregamentoPedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeMontagemCarregamentoPedidoProduto", Column = "SRO_PRIORIDADE_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeMontagemCarregamentoPedidoProduto PrioridadeMontagemCarregamentoPedidoProduto { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PrioridadeMontagemCarregamentoPedido", Column = "SRO_PRIORIDADE_MONTAGEM_CARREGAMENTO_PEDIDO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.PrioridadeMontagemCarregamentoPedido), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.PrioridadeMontagemCarregamentoPedido PrioridadeMontagemCarregamentoPedido { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoStatusEstoqueMontagemCarregamentoPedidoProduto", Column = "SRO_TIPO_STATUS_ESTOQUE_MONTAGEM_CARREGAMENTO_PEDIDO_PRODUTO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoStatusEstoqueMontagemCarregamentoPedidoProduto TipoStatusEstoqueMontagemCarregamentoPedidoProduto { get; set; }

        /// <summary>
        /// Último usuário mexendo na sessão.
        /// </summary>
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO_USUARIO_ATUAL", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario UsuarioAtual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Parametros", Column = "SRO_PARAMETROS", TypeType = typeof(string), Length = 20000, NotNull = false)]
        public virtual string Parametros { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "ReservaNumeroCarregamentoMontagem", Column = "SRO_RESERVA_NRO_CARREGAMENTO_MONTAGEM", TypeType = typeof(int), NotNull = false)]
        public virtual int ReservaNumeroCarregamentoMontagem { get; set; }

        ///// <summary>
        ///// Modelo Milk Run, TELHANORTE... gera o carregamento com várias coletas e seus respectivos destinatários...
        ///// </summary>
        //[NHibernate.Mapping.Attributes.Property(0, Name = "MontagemCarregamentoColetaEntrega", Column = "SRO_MONTAGEM_CARREGAMENTO_COLETA_ENTREGA", TypeType = typeof(bool), NotNull = false)]
        //public virtual bool MontagemCarregamentoColetaEntrega { get; set; }

        /// <summary>
        /// Quando é uma sessão secundária, sendo a origem não a filial e sim um expedidor sendo este o "Recebedor" do pedido de uma carga já gerada.
        /// </summary>
        [NHibernate.Mapping.Attributes.Property(0, Name = "RoteirizacaoRedespacho", Column = "SRO_ROTEIRIZACAO_REDESPACHO", TypeType = typeof(bool), NotNull = false)]
        public virtual bool RoteirizacaoRedespacho { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_EXPEDIDOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Expedidor { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoRoteirizacaoColetaEntrega", Column = "SRO_TIPO_ROTEIRIZACAO_COLETA_ENTREGA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoRoteirizacaoColetaEntrega), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoRoteirizacaoColetaEntrega TipoRoteirizacaoColetaEntrega { get; set; }

        public virtual bool Equals(SessaoRoteirizador other)
        {
            return (this.Codigo == other.Codigo);
        }

        public virtual string DescricaoSituacao
        {
            get
            {
                return this.SituacaoSessaoRoteirizador.ToString();
            }
        }
    }
}
