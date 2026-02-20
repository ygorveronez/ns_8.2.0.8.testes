using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Entidades.Embarcador.Pedidos
{
    //um pedido pode ter vaarios bookings (dados transporte maritimo) mas apenas um ativo.
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_PEDIDO_DADOS_TRANSPORTE_MARITIMO", EntityName = "PedidoDadosTransporteMaritimo", DynamicUpdate = true, Name = "Dominio.Entidades.Embarcador.Pedidos.PedidoDadosTransporteMaritimo", NameType = typeof(PedidoDadosTransporteMaritimo))]
    public class PedidoDadosTransporteMaritimo : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "CTM_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Pedido", Column = "PED_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Pedido Pedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoIdentificacaoCarga", Column = "CTM_CODIGO_IDENTIFICACAO_CARGA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoIdentificacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoIdentificacaoCarga", Column = "CTM_DESCRICAO_IDENTIFICACAO_CARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoIdentificacaoCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoNCM", Column = "CTM_CODIGO_NCM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoNCM { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MetragemCarga", Column = "CTM_METRAGEM_CARGA", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MetragemCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Incoterm", Column = "CTM_INCOTERM", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Incoterm { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Transbordo", Column = "CTM_TRANSBORDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string Transbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "MensagemTransbordo", Column = "CTM_MENSAGEM_TRANSBORDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string MensagemTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoArmador", Column = "CTM_CODIGO_ARMADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoArmador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoRota", Column = "CTM_CODIGO_ROTA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoRota { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_BOOKING", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_DEAD_LINE_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLineCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_DEAD_LINE_DRAF", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLineDraf { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_DEPOSITO_CONTAINER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDepositoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_DESTINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETADestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_DESTINO_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETADestinoFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_SEGUNDA_ORIGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETASegundaOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_SEGUNDO_DESTINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETASegundoDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_ORIGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETAOrigem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_ORIGEM_FINAL", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETAOrigemFinal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETA_TRANSBORDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETATransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETS", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_ETS_TRANSBORDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETSTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_RETIRADA_CONTAINER", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetiradaContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_RETIRADA_CONTAINER_DESTINO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetiradaContainerDestino { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_RETIRADA_VAZIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetiradaVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_DATA_RETORNO_VAZIO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataRetornoVazio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoCarregamentoTransbordo", Column = "CTM_CODIGO_PORTO_CARREGAMENTO_TRANSBORDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortoCarregamentoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPortoCarregamentoTransbordo", Column = "CTM_DESCRICAO_PORTO_CARREGAMENTO_TRANSBORDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoPortoCarregamentoTransbordo { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_PORTO_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PortoOrigem { get; set; }

        [ObsoleteAttribute("INATIVADA foi transformado em campo PortoOrigem", false)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoCarregamento", Column = "CTM_CODIGO_PORTO_CARREGAMENTO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortoCarregamento { get; set; }

        [ObsoleteAttribute("INATIVADA foi transformado em campo PortoOrigem", false)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPortoOrigem", Column = "CTM_PORTO_ORIGEM_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoPortoOrigem { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo PortoOrigem", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "PaisPortoOrigem", Column = "CTM_PORTO_ORIGEM_PAIS", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string PaisPortoOrigem { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo PortoOrigem", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisPortoOrigem", Column = "CTM_PORTO_ORIGEM_SIGLA_PAIS", TypeType = typeof(string), Length = 10, NotNull = false)]
        //public virtual string SiglaPaisPortoOrigem { get; set; }


        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_PORTO_DESTINO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente PortoDestino { get; set; }

        [ObsoleteAttribute("INATIVADA foi transformado em campo PortoDestino", false)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoPortoDestinoTransbordo", Column = "CTM_CODIGO_PORTO_DESTINO_TRANSBORDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoPortoDestinoTransbordo { get; set; }

        [ObsoleteAttribute("INATIVADA foi transformado em campo PortoDestino", false)]
        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoPortoDestinoTransbordo", Column = "CTM_DESCRICAO_PORTO_DESTINO_TRANSBORDO", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string DescricaoPortoDestinoTransbordo { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo PortoDestino", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "PaisPortoDestinoTransbordo", Column = "CTM_PORTO_DESTINO_PAIS", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string PaisPortoDestinoTransbordo { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo PortoDestino", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "SiglaPaisPortoDestinoTransbordo", Column = "CTM_PORTO_DESTINO_SIGLA_PAIS", TypeType = typeof(string), Length = 10, NotNull = false)]
        //public virtual string SiglaPaisPortoDestinoTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ModoTransporte", Column = "CTM_MODO_TRANSPORTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string ModoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NomeNavio", Column = "CTM_NOME_NAVIO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NomeNavio { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "NomeNavioTransbordo", Column = "CTM_NOME_NAVIO_TRANSBORDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string NomeNavioTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO_TRANSBORDO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Pedidos.Navio NavioTransbordo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroBL", Column = "CTM_NUMERO_BL", TypeType = typeof(string), Length = 200, NotNull = false)]
        public virtual string NumeroBL { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo Container", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "NumeroContainer", Column = "CTM_NUMERO_CONTAINER", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string NumeroContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Container", Column = "CTR_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Embarcador.Pedidos.Container Container { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroLacre", Column = "CTM_NUMERO_LACRE", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroLacre { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagem", Column = "CTM_NUMERO_VIAGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroViagem { get; set; } //TEM O CODIGO VIAGEM

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroViagemTransbordo", Column = "CTM_NUMERO_VIAGEM_TRANSBORDO", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string NumeroViagemTransbordo { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo cliente LocalTerminalContainer.", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "TerminalContainer", Column = "CTM_TERMINAL_CONTAINER", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string TerminalContainer { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_TERMINAL_CONTAINER", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Cliente LocalTerminalContainer { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformado em campo cliente LocalTerminalOrigem.", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "TerminalOrigem", Column = "CTM_TERMINAL_ORIGEM", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string TerminalOrigem { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_LOCAL_TERMINAL_ORIGEM", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Entidades.Cliente LocalTerminalOrigem { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformada em enumerador TipoDeTransporte.", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "TipoTransporte", Column = "CTM_TIPO_TRANSPORTE", TypeType = typeof(string), Length = 50, NotNull = false)]
        //public virtual string TipoTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoDeTransporte", Column = "CTM_TIPO_DE_TRANSPORTE", TypeType = typeof(TipoTransporteDadosMaritimos), NotNull = false)]
        public virtual TipoTransporteDadosMaritimos TipoDeTransporte { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoEnvio", Column = "CTM_TIPO_ENVIO", TypeType = typeof(TipoEnvioTransporteMaritimo), NotNull = false)]
        public virtual TipoEnvioTransporteMaritimo? TipoEnvio { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "CTM_STATUS", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo Status { get; set; }

        // *** CAMPOS NOVOS.. ** \\
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Filial", Column = "FIL_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Filiais.Filial Filial { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_NUMERO_BOOKING", TypeType = typeof(string), NotNull = false, Length = 150)]
        public virtual string NumeroBooking { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroEXP", Column = "CTM_NUMERO_EXP", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string NumeroEXP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoCargaEmbarcador", Column = "CTM_CODIGO_CARGA_EMBARCADOR", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string CodigoCargaEmbarcador { get; set; }

        //[ObsoleteAttribute("INATIVADA foi trocada para TipoDeCarga.", false)]
        //[NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoCarga", Column = "ATC_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        //public virtual Dominio.Entidades.TipoCarga TipoCarga { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "TipoDeCarga", Column = "TCG_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.TipoDeCarga TipoDeCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PossuiGenset", Column = "CTM_POSSUI_GENSET", TypeType = typeof(bool), NotNull = false)]
        public virtual bool PossuiGenset { get; set; }

        //[ObsoleteAttribute("INATIVADA foi trocada para Entidade Cliente Despachante", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoDespachante", Column = "CTM_DESPACHANTE_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string CodigoDespachante { get; set; }

        //[ObsoleteAttribute("INATIVADA foi trocada para Entidade Cliente Despachante", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoDespachante", Column = "CTM_DESPACHANTE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string DescricaoDespachante { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_DESPACHANTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Despachante { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Halal", Column = "CTM_HALAL", TypeType = typeof(bool), NotNull = false)]
        public virtual bool Halal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataETA", Column = "CTM_DATA_ETA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataETA { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_IMPORTADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Importador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Navio", Column = "NAV_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Navio Navio { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ContainerTipo", Column = "CTI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual ContainerTipo TipoContainer { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoProbe", Column = "CTM_TIPO_PROBE", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoProbe), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoProbe? TipoProbe { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ViaTransporte", Column = "TVT_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Cargas.ViaTransporte ViaTransporte { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_ARMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Armador { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoEspecie", Column = "CTM_ESPECIE_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoEspecie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoEspecie", Column = "CTM_ESPECIE_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string DescricaoEspecie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "StatusEXP", Column = "CTM_STATUS_EXP", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.StatusEXP), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.StatusEXP? StatusEXP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "FretePrepaid", Column = "CTM_TIPO_FRETE_PREPAID", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.FretePrepaid), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.FretePrepaid FretePrepaid { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CargaPaletizada", Column = "CTM_CARGA_PALETIZADA", TypeType = typeof(bool), NotNull = false)]
        public virtual bool CargaPaletizada { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Temperatura", Column = "CTM_TEMPERATURA", TypeType = typeof(string), Length = 50, NotNull = false)]
        public virtual string Temperatura { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataCarregamentoPedido", Column = "CTM_DATA_CARREGAMENTO_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataCarregamentoPedido { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Cliente", Column = "CLI_CODIGO_REMETENTE", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Cliente Remetente { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformada em enumerador TipoInLand.", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "CodigoInLand", Column = "CTM_INLAND_CODIGO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string CodigoInLand { get; set; }

        //[ObsoleteAttribute("INATIVADA foi transformada em enumerador TipoInLand.", false)]
        //[NHibernate.Mapping.Attributes.Property(0, Name = "DescricaoInLand", Column = "CTM_INLAND_DESCRICAO", TypeType = typeof(string), Length = 150, NotNull = false)]
        //public virtual string DescricaoInLand { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "TipoInLand", Column = "CTM_INLAND", TypeType = typeof(ObjetosDeValor.Embarcador.Enumeradores.TipoInland), NotNull = false)]
        public virtual ObjetosDeValor.Embarcador.Enumeradores.TipoInland TipoInLand { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataDeadLinePedido", Column = "CTM_DATA_DEADLINE_PEDIDO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataDeadLinePedido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataReserva", Column = "CTM_DATA_RESERVA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataReserva { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_SEGUNDA_DATA_DEAD_LINE_CARGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? SegundaDataDeadLineCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "CTM_SEGUNDA_DATA_DEAD_LINE_DRAF", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? SegundaDataDeadLineDraf { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCapatazia", Column = "CTM_VALOR_CAPATAZIA", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorCapatazia { get; set; }

        //[NHibernate.Mapping.Attributes.Property(0, Name = "MoedaCapatazia", Column = "CTM_MOEDA_CAPATAZIA", TypeType = typeof(string), Length = 20, NotNull = false)]
        //public virtual string MoedaCapatazia { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Moeda", Column = "MDA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Moedas.Moeda MoedaCapatazia { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorFrete", Column = "CTM_VALOR_FRETE", TypeType = typeof(decimal), NotNull = false, Scale = 4, Precision = 18)]
        public virtual decimal ValorFrete { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoContratoFOB", Column = "CTM_CODIGO_CONTRATO_FOB", TypeType = typeof(string), Length = 150, NotNull = false)]
        public virtual string CodigoContratoFOB { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Observacao", Column = "CTM_OBSERVACAO", TypeType = typeof(string), Length = 800, NotNull = false)]
        public virtual string Observacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaCancelamento", Column = "CTM_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 400, NotNull = false)]
        public virtual string JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEntrega", Column = "CTM_PREVISAO_ENTREGA", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEntrega { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ProtocoloCarga", Column = "CTM_CODIGO_PROTOCOLO_CARGA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string ProtocoloCarga { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataPrevisaoEstufagem", Column = "CTM_PREVISAO_ESTUFAGEM", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataPrevisaoEstufagem { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataConhecimento", Column = "CTM_DATA_CONHECIMENTO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataConhecimento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoOriginal", TypeType = typeof(int), Column = "CTM_CODIGO_ORIGINAL", NotNull = false)]
        public virtual int CodigoOriginal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BookingTemporario", TypeType = typeof(bool), Column = "CTM_BOOKING_TEMPORARIO", NotNull = false)]
        public virtual bool BookingTemporario { get; set; }

        // *** FIM DOS CAMPOS NOVOS.. ** \\


        public virtual PedidoDadosTransporteMaritimo Clonar()
        {
            PedidoDadosTransporteMaritimo pedidoDadosTransporteMaritimoClonado = (PedidoDadosTransporteMaritimo)this.MemberwiseClone();
            return pedidoDadosTransporteMaritimoClonado;
        }

        public virtual string DescricaoStatus
        {
            get
            {

                switch (this.Status)
                {
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Ativo:
                        return "Ativo";
                    case ObjetosDeValor.Embarcador.Enumeradores.StatusControleMaritimo.Cancelado:
                        return "Cancelado";
                    default:
                        return "";
                }
            }
        }

    }
}
