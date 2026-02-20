using System;
using System.Collections.Generic;

namespace Dominio.Entidades.Embarcador.Cargas.RolagemContainer
{
    [NHibernate.Mapping.Attributes.Class(0, DynamicUpdate = true, Table = "T_ROLAGEM_CONTAINER", EntityName = "RolagemContainer", Name = "Dominio.Entidades.Embarcador.Cargas.RolagemContainer.RolagemContainer", NameType = typeof(RolagemContainer))]
    public class RolagemContainer : EntidadeBase, Interfaces.Embarcador.Entidade.IEntidade
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "ROC_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Usuario", Column = "FUN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Usuario Usuario { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Planilha", Column = "ROC_PLANILHA", TypeType = typeof(string), Length = 200, NotNull = true)]
        public virtual string Planilha { get; set; }
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "CaminhoPlanilha", Column = "ROC_CAMINHO_PLANILHA", Type = "StringClob", NotNull = false)]
        public virtual string CaminhoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "QuantidadeLinhas", Column = "ROC_QTDE_LINHAS", TypeType = typeof(int), NotNull = true)]
        public virtual int QuantidadeLinhas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataImportacao", Column = "ROC_DATA_IMPORTACAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataImportacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Situacao", Column = "ROC_SITUACAO", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido), NotNull = true)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoImportacaoPedido Situacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Mensagem", Column = "ROC_MENSAGEM", TypeType = typeof(string), Length = 1000, NotNull = false)]
        public virtual string Mensagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataInicioProcessamento", Column = "ROC_DATA_INICIO_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataInicioProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataFimProcessamento", Column = "ROC_DATA_FIM_PROCESSAMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataFimProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TotalSegundosProcessamento", Column = "ROC_TOTAL_SEGUNDOS_PROCESSAMENTO", TypeType = typeof(int), NotNull = false)]
        public virtual int? TotalSegundosProcessamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoPlanilha", Column = "ROC_TIPO_PLANILHA", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoPlanilhaFeeder TipoPlanilha { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CGCCPF_CLIENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Cliente Cliente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEquipamento", Column = "ROC_TIPO_EQUIPAMENTO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string TipoEquipamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BookingNovo", Column = "ROC_NUMERO_BOOKING_NOVO", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string BookingNovo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BookingAnterior", Column = "ROC_NUMERO_BOOKING_ANTERIOR", TypeType = typeof(string), NotNull = true, Length = 200)]
        public virtual string BookingAnterior { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroOS", Column = "ROC_NUMERO_OS", TypeType = typeof(string), NotNull = false, Length = 200)]
        public virtual string NumeroOS { get; set; }
  
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_MUNICIPIO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade MunicipioOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_MUNICIPIO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade MunicipioDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFOrigem", Column = "ROC_UF_ORIGEM", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "UFDestino", Column = "ROC_UF_DESTINO", TypeType = typeof(string), Length = 2, NotNull = false)]
        public virtual string UFDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.Porto PortoDestino { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO_1", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo1 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_TRANSBORDO_1", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo1 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_TRANSBORDO_1", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalTransbordo1 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_VIAGEM_1", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavioTransbordo1 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO_2", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo2 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_TRANSBORDO_2", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo2 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_TRANSBORDO_2", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalTransbordo2 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_VIAGEM_2", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavioTransbordo2 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO_3", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_TRANSBORDO_3", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_TRANSBORDO_3", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalTransbordo3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_VIAGEM_3", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavioTransbordo3 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO_4", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo4 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_TRANSBORDO_4", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo4 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_TRANSBORDO_4", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalTransbordo4 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_VIAGEM_4", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavioTransbordo4 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Porto", Column = "POT_PORTO_TRANSBORDO_5", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Porto PortoTransbordo5 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_TRANSBORDO_5", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.Navio NavioTransbordo5 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoTerminalImportacao", Column = "TTI_TERMINAL_TRANSBORDO_5", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedidos.TipoTerminalImportacao TerminalTransbordo5 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "PedidoViagemNavio", Column = "PVN_VIAGEM_5", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Pedidos.PedidoViagemNavio PedidoViagemNavioTransbordo5 { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO_ANTIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedidoAntigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "CargaPedido", Column = "CPE_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.CargaPedido CargaPedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "ROC_ENTIDADE_PAI", TypeType = typeof(bool), NotNull = false)]
        public virtual bool EntidadePai { get; set; }

        [NHibernate.Mapping.Attributes.Set(0, Name = "RolagemContainers", Cascade = "all", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_ROLAGEM_CONTAINER_ROLAGEM_CONTAINER")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "ROC_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "RolagemContainer", Column = "ROC_CODIGO_FILHO")]
        public virtual ICollection<RolagemContainer> RolagemContainers { get; set; }

        public virtual TimeSpan? Tempo()
        {
            if (TotalSegundosProcessamento != null)
                return TimeSpan.FromSeconds(TotalSegundosProcessamento.Value);
            else
                return null;
        }

        public virtual string Descricao
        {
            get
            {
                return Planilha;
            }
        }
    }
}
