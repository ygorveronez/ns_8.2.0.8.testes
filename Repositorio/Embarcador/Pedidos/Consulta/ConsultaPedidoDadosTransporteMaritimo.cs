using Dominio.ObjetosDeValor.Embarcador.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Pedidos
{
    sealed class ConsultaPedidoDadosTransporteMaritimo : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Pedido.FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo>
    {
        #region Construtores

        public ConsultaPedidoDadosTransporteMaritimo() : base(tabela: "T_PEDIDO_DADOS_TRANSPORTE_MARITIMO PedidoDadoTransporteMaritimo ") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append(" LEFT JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = PedidoDadoTransporteMaritimo.PED_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" LEFT JOIN T_CARGA_PEDIDO CargaPedido ON CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" LEFT JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsRemetentePedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" RemetentePedido "))
                joins.Append(" LEFT JOIN T_CLIENTE RemetentePedido ON RemetentePedido.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsRecebedorPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" RecebedorPedido "))
                joins.Append(" LEFT JOIN T_CLIENTE RecebedorPedido ON RecebedorPedido.CLI_CGCCPF = Pedido.CLI_CODIGO_RECEBEDOR ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append(" LEFT JOIN T_ACERTO_TIPO_CARGA TipoCarga ON TipoCarga.ATC_CODIGO = PedidoDadoTransporteMaritimo.ATC_CODIGO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append(" LEFT JOIN T_FILIAL Filial ON Filial.FIL_CODIGO = PedidoDadoTransporteMaritimo.FIL_CODIGO ");
        }

        private void SetarJoinsImportador(StringBuilder joins)
        {
            if (!joins.Contains(" Importador "))
                joins.Append(" LEFT JOIN T_CLIENTE Importador ON Importador.CLI_CGCCPF = PedidoDadoTransporteMaritimo.CLI_CODIGO_IMPORTADOR "); 
        }

        private void SetarJoinsNavio(StringBuilder joins)
        {
            if (!joins.Contains(" Navio "))
                joins.Append(" LEFT JOIN T_NAVIO Navio ON Navio.NAV_CODIGO = PedidoDadoTransporteMaritimo.NAV_CODIGO ");
        }

        private void SetarJoinsTipoContainer(StringBuilder joins)
        {
            if (!joins.Contains(" TipoContainer "))
                joins.Append(" LEFT JOIN T_CONTAINER_TIPO TipoContainer ON TipoContainer.CTI_CODIGO = PedidoDadoTransporteMaritimo.CTI_CODIGO ");
        }

        private void SetarJoinsViaTransporte(StringBuilder joins)
        {
            if (!joins.Contains(" ViaTransporte "))
                joins.Append(" LEFT JOIN T_VIA_TRANSPORTE ViaTransporte ON ViaTransporte.TVT_CODIGO = PedidoDadoTransporteMaritimo.TVT_CODIGO ");
        }

        private void SetarJoinsArmador(StringBuilder joins)
        {
            if (!joins.Contains(" Armador "))
                joins.Append(" LEFT JOIN T_CLIENTE Armador ON Armador.CLI_CGCCPF = PedidoDadoTransporteMaritimo.CLI_CODIGO_ARMADOR ");
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" Remetente "))
                joins.Append(" LEFT JOIN T_CLIENTE Remetente ON Remetente.CLI_CGCCPF = PedidoDadoTransporteMaritimo.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsContainer(StringBuilder joins)
        {
            if (!joins.Contains(" Container "))
                joins.Append(" LEFT JOIN T_CONTAINER Container ON Container.CTR_CODIGO = PedidoDadoTransporteMaritimo.CTR_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO Codigo, ");
                groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO, ");
            }

            switch (propriedade)
            {
                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CodigoIdentificacaoCarga":
                    if (!select.Contains(" CodigoIdentificacaoCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_IDENTIFICACAO_CARGA CodigoIdentificacaoCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_IDENTIFICACAO_CARGA, ");
                    }
                    break;

                case "DescricaoIdentificacaoCarga":
                    if (!select.Contains(" DescricaoIdentificacaoCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_IDENTIFICACAO_CARGA DescricaoIdentificacaoCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_IDENTIFICACAO_CARGA, ");
                    }
                    break;

                case "CodigoNCM":
                    if (!select.Contains(" CodigoNCM, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_NCM CodigoNCM, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_NCM, ");
                    }
                    break;

                case "MetragemCarga":
                    if (!select.Contains(" MetragemCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_METRAGEM_CARGA MetragemCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_METRAGEM_CARGA, ");
                    }
                    break;

                case "Incoterm":
                    if (!select.Contains(" Incoterm, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_INCOTERM Incoterm, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_INCOTERM, ");
                    }
                    break;

                case "Transbordo":
                    if (!select.Contains(" Transbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TRANSBORDO Transbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TRANSBORDO, ");
                    }
                    break;

                case "MensagemTransbordo":
                    if (!select.Contains(" MensagemTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_MENSAGEM_TRANSBORDO MensagemTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_MENSAGEM_TRANSBORDO, ");
                    }
                    break;

                case "CodigoArmador":
                    if (!select.Contains(" CodigoArmador, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ARMADOR CodigoArmador, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ARMADOR, ");
                    }
                    break;

                case "CodigoRota":
                    if (!select.Contains(" CodigoRota, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ROTA CodigoRota, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ROTA, ");
                    }
                    break;

                case "DataBookingFormatada":
                    if (!select.Contains(" DataBooking, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_BOOKING DataBooking, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_BOOKING, ");
                    }
                    break;

                case "DataDeadLineCargaFormatada":
                    if (!select.Contains(" DataDeadLineCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEAD_LINE_CARGA DataDeadLineCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEAD_LINE_CARGA, ");
                    }
                    break;

                case "DataDeadLineDrafFormatada":
                    if (!select.Contains(" DataDeadLineDraf, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEAD_LINE_DRAF DataDeadLineDraf, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEAD_LINE_DRAF, ");
                    }
                    break;

                case "DataDepositoContainerFormatada":
                    if (!select.Contains(" DataDepositoContainer, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEPOSITO_CONTAINER DataDepositoContainer, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEPOSITO_CONTAINER, ");
                    }
                    break;

                case "DataETADestinoFormatada":
                    if (!select.Contains(" DataETADestino, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_DESTINO DataETADestino, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_DESTINO, ");
                    }
                    break;

                case "DataETADestinoFinalFormatada":
                    if (!select.Contains(" DataETADestinoFinal, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_DESTINO_FINAL DataETADestinoFinal, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_DESTINO_FINAL, ");
                    }
                    break;

                case "DataETASegundaOrigemFormatada":
                    if (!select.Contains(" DataETASegundaOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_SEGUNDA_ORIGEM DataETASegundaOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_SEGUNDA_ORIGEM, ");
                    }
                    break;

                case "DataETASegundoDestinoFormatada":
                    if (!select.Contains(" DataETASegundoDestino, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_SEGUNDO_DESTINO DataETASegundoDestino, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_SEGUNDO_DESTINO, ");
                    }
                    break;

                case "DataETAOrigemFormatada":
                    if (!select.Contains(" DataETAOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_ORIGEM DataETAOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_ORIGEM, ");
                    }
                    break;

                case "DataETAOrigemFinalFormatada":
                    if (!select.Contains(" DataETAOrigemFinal, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_ORIGEM_FINAL DataETAOrigemFinal, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_ORIGEM_FINAL, ");
                    }
                    break;

                case "DataETATransbordoFormatada":
                    if (!select.Contains(" DataETATransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_TRANSBORDO DataETATransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA_TRANSBORDO, ");
                    }
                    break;

                case "DataETSFormatada":
                    if (!select.Contains(" DataETS, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETS DataETS, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETS, ");
                    }
                    break;

                case "DataETSTransbordoFormatada":
                    if (!select.Contains(" DataETSTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETS_TRANSBORDO DataETSTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETS_TRANSBORDO, ");
                    }
                    break;

                case "DataRetiradaContainerFormatada":
                    if (!select.Contains(" DataRetiradaContainer, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_CONTAINER DataRetiradaContainer, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_CONTAINER, ");
                    }
                    break;

                case "DataRetiradaContainerDestinoFormatada":
                    if (!select.Contains(" DataRetiradaContainerDestino, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_CONTAINER_DESTINO DataRetiradaContainerDestino, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_CONTAINER_DESTINO, ");
                    }
                    break;

                case "DataRetiradaVazioFormatada":
                    if (!select.Contains(" DataRetiradaVazio, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_VAZIO DataRetiradaVazio, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETIRADA_VAZIO, ");
                    }
                    break;

                case "DataRetornoVazioFormatada":
                    if (!select.Contains(" DataRetornoVazio, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETORNO_VAZIO DataRetornoVazio, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RETORNO_VAZIO, ");
                    }
                    break;

                case "CodigoPortoCarregamento":
                    if (!select.Contains(" CodigoPortoCarregamento, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_CARREGAMENTO CodigoPortoCarregamento, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_CARREGAMENTO, ");
                    }
                    break;

                case "DescricaoPortoOrigem":
                    if (!select.Contains(" DescricaoPortoOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_DESCRICAO DescricaoPortoOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_DESCRICAO, ");
                    }
                    break;

                case "PaisPortoOrigem":
                    if (!select.Contains(" PaisPortoOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_PAIS PaisPortoOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_PAIS, ");
                    }
                    break;

                case "SiglaPaisPortoOrigem":
                    if (!select.Contains(" SiglaPaisPortoOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_SIGLA_PAIS SiglaPaisPortoOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_ORIGEM_SIGLA_PAIS, ");
                    }
                    break;

                case "CodigoPortoCarregamentoTransbordo":
                    if (!select.Contains(" CodigoPortoCarregamentoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_CARREGAMENTO_TRANSBORDO CodigoPortoCarregamentoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_CARREGAMENTO_TRANSBORDO, ");
                    }
                    break;

                case "DescricaoPortoCarregamentoTransbordo":
                    if (!select.Contains(" DescricaoPortoCarregamentoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_PORTO_CARREGAMENTO_TRANSBORDO DescricaoPortoCarregamentoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_PORTO_CARREGAMENTO_TRANSBORDO, ");
                    }
                    break;

                case "CodigoPortoDestinoTransbordo":
                    if (!select.Contains(" CodigoPortoDestinoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_DESTINO_TRANSBORDO CodigoPortoDestinoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PORTO_DESTINO_TRANSBORDO, ");
                    }
                    break;

                case "DescricaoPortoDestinoTransbordo":
                    if (!select.Contains(" DescricaoPortoDestinoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_PORTO_DESTINO_TRANSBORDO DescricaoPortoDestinoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DESCRICAO_PORTO_DESTINO_TRANSBORDO, ");
                    }
                    break;

                case "PaisPortoDestinoTransbordo":
                    if (!select.Contains(" PaisPortoDestinoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_DESTINO_PAIS PaisPortoDestinoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_DESTINO_PAIS, ");
                    }
                    break;

                case "SiglaPaisPortoDestinoTransbordo":
                    if (!select.Contains(" SiglaPaisPortoDestinoTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_DESTINO_SIGLA_PAIS SiglaPaisPortoDestinoTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PORTO_DESTINO_SIGLA_PAIS, ");
                    }
                    break;

                case "ModoTransporte":
                    if (!select.Contains(" ModoTransporte, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_MODO_TRANSPORTE ModoTransporte, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_MODO_TRANSPORTE, ");
                    }
                    break;

                case "NomeNavio":
                    if (!select.Contains(" NomeNavio, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NOME_NAVIO NomeNavio, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NOME_NAVIO, ");
                    }
                    break;

                case "NomeNavioTransbordo":
                    if (!select.Contains(" NomeNavioTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NOME_NAVIO_TRANSBORDO NomeNavioTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NOME_NAVIO_TRANSBORDO, ");
                    }
                    break;

                case "NumeroBLFormatado":
                    SetarSelect("NumeroBL", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("NumeroBLPedido", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "NumeroBL":
                    if (!select.Contains(" NumeroBL, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_BL NumeroBL, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_BL, ");
                    }
                    break;

                case "NumeroBLPedido":
                    if (!select.Contains(" NumeroBLPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_BL NumeroBLPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_BL, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains(" NumeroContainer, "))
                    {
                        //select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_CONTAINER NumeroContainer, ");
                        //groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_CONTAINER, ");

                        select.Append("Container.CTR_NUMERO NumeroContainer, ");
                        groupBy.Append("Container.CTR_NUMERO, ");

                        SetarJoinsContainer(joins);
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains(" NumeroLacre, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_LACRE NumeroLacre, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_LACRE, ");
                    }
                    break;

                case "NumeroViagem":
                    if (!select.Contains(" NumeroViagem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_VIAGEM NumeroViagem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_VIAGEM, ");
                    }
                    break;

                case "NumeroViagemTransbordo":
                    if (!select.Contains(" NumeroViagemTransbordo, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_VIAGEM_TRANSBORDO NumeroViagemTransbordo, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_VIAGEM_TRANSBORDO, ");
                    }
                    break;

                case "TerminalContainer":
                    if (!select.Contains(" TerminalContainer, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TERMINAL_CONTAINER TerminalContainer, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TERMINAL_CONTAINER, ");
                    }
                    break;

                case "TerminalOrigem":
                    if (!select.Contains(" TerminalOrigem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TERMINAL_ORIGEM TerminalOrigem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TERMINAL_ORIGEM, ");
                    }
                    break;

                case "TipoTransporte":
                    if (!select.Contains(" TipoTransporte, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_TRANSPORTE TipoTransporte, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_TRANSPORTE, ");
                    }
                    break;

                case "TipoEnvioDescricao":
                    if (!select.Contains(" TipoEnvio, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_ENVIO TipoEnvio, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_ENVIO, ");
                    }
                    break;

                case "StatusDescricao":
                    if (!select.Contains(" Status, "))
                    {
                        select.Append("CONVERT(INT,PedidoDadoTransporteMaritimo.CTM_STATUS) Status, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_STATUS, ");
                    }
                    break;

                case "DescricaoFilial":
                    if (!select.Contains(" DescricaoFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO DescricaoFilial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NumeroBookingFormatado":
                    SetarSelect("NumeroBooking", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("NumeroBookingPedido", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_BOOKING NumeroBooking, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroBookingPedido":
                    if (!select.Contains(" NumeroBookingPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_BOOKING NumeroBookingPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_BOOKING, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "NumeroEXPFormatado":
                    SetarSelect("NumeroEXP", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("NumeroEXPPedido", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "NumeroEXP":
                    if (!select.Contains(" NumeroEXP, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_EXP NumeroEXP, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_NUMERO_EXP, ");
                    }
                    break;

                case "NumeroEXPPedido":
                    if (!select.Contains(" NumeroEXPPedido, "))
                    {
                        select.Append("Pedido.PED_NUMERO_EXP NumeroEXPPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_EXP, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CodigoCargaEmbarcadorFormatado":
                    SetarSelect("CodigoCargaEmbarcador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    SetarSelect("PedidoCodigoCargaEmbarcador", 0, select, joins, groupBy, false, filtrosPesquisa);
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_CARGA_EMBARCADOR CodigoCargaEmbarcador, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "PedidoCodigoCargaEmbarcador":
                    if (!select.Contains(" PedidoCodigoCargaEmbarcador, "))
                    {
                        select.Append("Pedido.PED_CODIGO_CARGA_EMBARCADOR PedidoCodigoCargaEmbarcador, ");
                        groupBy.Append("Pedido.PED_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    break;

                case "DescricaoTipoCarga":
                    if (!select.Contains(" DescricaoTipoCarga, "))
                    {
                        select.Append("TipoCarga.ACE_DESCRICAO DescricaoTipoCarga, ");
                        groupBy.Append("TipoCarga.ACE_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins);
                    }
                    break;

                case "PossuiGensetFormatado":
                    if (!select.Contains(" PossuiGenset, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_POSSUI_GENSET PossuiGenset, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_POSSUI_GENSET, ");
                    }
                    break;

                case "CodigoDespachante":
                    if (!select.Contains(" CodigoDespachante, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DESPACHANTE_CODIGO CodigoDespachante, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DESPACHANTE_CODIGO, ");
                    }
                    break;

                case "DescricaoDespachante":
                    if (!select.Contains(" DescricaoDespachante, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DESPACHANTE_DESCRICAO DescricaoDespachante, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DESPACHANTE_DESCRICAO, ");
                    }
                    break;

                case "HalalFormatado":
                    if (!select.Contains(" Halal, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_HALAL Halal, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_HALAL, ");
                    }
                    break;

                case "DataETAFormatada":
                    if (!select.Contains(" DataETA, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA DataETA, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_ETA, ");
                    }
                    break;

                case "NomeImportador":
                    if (!select.Contains(" NomeImportador, "))
                    {
                        select.Append("Importador.CLI_NOME NomeImportador, ");
                        groupBy.Append("Importador.CLI_NOME, ");

                        SetarJoinsImportador(joins);
                    }
                    break;

                case "DescricaoNavio":
                    if (!select.Contains(" DescricaoNavio, "))
                    {
                        select.Append("Navio.NAV_DESCRICAO DescricaoNavio, ");
                        groupBy.Append("Navio.NAV_DESCRICAO, ");

                        SetarJoinsNavio(joins);
                    }
                    break;

                case "DescricaoTipoContainer":
                    if (!select.Contains(" DescricaoTipoContainer, "))
                    {
                        select.Append("TipoContainer.CTI_DESCRICAO DescricaoTipoContainer, ");
                        groupBy.Append("TipoContainer.CTI_DESCRICAO, ");

                        SetarJoinsTipoContainer(joins);
                    }
                    break;

                case "TipoProbeDescricao":
                    if (!select.Contains(" TipoProbe, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_PROBE TipoProbe, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_PROBE, ");
                    }
                    break;

                case "DescricaoViaTransporte":
                    if (!select.Contains(" DescricaoViaTransporte, "))
                    {
                        select.Append("ViaTransporte.TVT_DESCRICAO DescricaoViaTransporte, ");
                        groupBy.Append("ViaTransporte.TVT_DESCRICAO, ");

                        SetarJoinsViaTransporte(joins);
                    }
                    break;

                case "NomeArmador":
                    if (!select.Contains(" NomeArmador, "))
                    {
                        select.Append("Armador.CLI_NOME NomeArmador, ");
                        groupBy.Append("Armador.CLI_NOME, ");

                        SetarJoinsArmador(joins);
                    }
                    break;

                case "CodigoEspecie":
                    if (!select.Contains(" CodigoEspecie, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_ESPECIE_CODIGO CodigoEspecie, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_ESPECIE_CODIGO, ");
                    }
                    break;

                case "DescricaoEspecie":
                    if (!select.Contains(" DescricaoEspecie, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_ESPECIE_DESCRICAO DescricaoEspecie, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_ESPECIE_DESCRICAO, ");
                    }
                    break;

                case "StatusEXPDescricao":
                    if (!select.Contains(" StatusEXP, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_STATUS_EXP StatusEXP, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_STATUS_EXP, ");
                    }
                    break;

                case "FretePrepaidDescricao":
                    if (!select.Contains(" FretePrepaid, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_FRETE_PREPAID FretePrepaid, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TIPO_FRETE_PREPAID, ");
                    }
                    break;

                case "CargaPaletizadaFormatado":
                    if (!select.Contains(" CargaPaletizada, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CARGA_PALETIZADA CargaPaletizada, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CARGA_PALETIZADA, ");
                    }
                    break;

                case "Temperatura":
                    if (!select.Contains(" Temperatura, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_TEMPERATURA Temperatura, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_TEMPERATURA, ");
                    }
                    break;

                case "DataCarregamentoPedidoFormatada":
                    if (!select.Contains(" DataCarregamentoPedido, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_CARREGAMENTO_PEDIDO DataCarregamentoPedido, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_CARREGAMENTO_PEDIDO, ");
                    }
                    break;

                case "NomeRemetente":
                    if (!select.Contains(" NomeRemetente, "))
                    {
                        select.Append("Remetente.CLI_NOME NomeRemetente, ");
                        groupBy.Append("Remetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CodigoInLand":
                    if (!select.Contains(" CodigoInLand, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_INLAND_CODIGO CodigoInLand, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_INLAND_CODIGO, ");
                    }
                    break;

                case "DescricaoInLand":
                    if (!select.Contains(" DescricaoInLand, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_INLAND_DESCRICAO DescricaoInLand, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_INLAND_DESCRICAO, ");
                    }
                    break;

                case "DataDeadLinePedidoFormatada":
                    if (!select.Contains(" DataDeadLinePedido, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEADLINE_PEDIDO DataDeadLinePedido, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_DEADLINE_PEDIDO, ");
                    }
                    break;

                case "DataReservaFormatada":
                    if (!select.Contains(" DataReserva, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RESERVA DataReserva, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_RESERVA, ");
                    }
                    break;

                case "SegundaDataDeadLineCargaFormatada":
                    if (!select.Contains(" SegundaDataDeadLineCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_SEGUNDA_DATA_DEAD_LINE_CARGA SegundaDataDeadLineCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_SEGUNDA_DATA_DEAD_LINE_CARGA, ");
                    }
                    break;

                case "SegundaDataDeadLineDrafFormatada":
                    if (!select.Contains(" SegundaDataDeadLineDraf, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_SEGUNDA_DATA_DEAD_LINE_DRAF SegundaDataDeadLineDraf, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_SEGUNDA_DATA_DEAD_LINE_DRAF, ");
                    }
                    break;

                case "ValorCapatazia":
                    if (!select.Contains(" ValorCapatazia, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_VALOR_CAPATAZIA ValorCapatazia, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_VALOR_CAPATAZIA, ");
                    }
                    break;

                case "MoedaCapatazia":
                    if (!select.Contains(" MoedaCapatazia, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_MOEDA_CAPATAZIA MoedaCapatazia, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_MOEDA_CAPATAZIA, ");
                    }
                    break;

                case "ValorFrete":
                    if (!select.Contains(" ValorFrete, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_VALOR_FRETE ValorFrete, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_VALOR_FRETE, ");
                    }
                    break;

                case "CodigoContratoFOB":
                    if (!select.Contains(" CodigoContratoFOB, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_CONTRATO_FOB CodigoContratoFOB, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_CONTRATO_FOB, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_OBSERVACAO Observacao, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_OBSERVACAO, ");
                    }
                    break;

                case "JustificativaCancelamento":
                    if (!select.Contains(" JustificativaCancelamento, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_JUSTIFICATIVA_CANCELAMENTO JustificativaCancelamento, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_JUSTIFICATIVA_CANCELAMENTO, ");
                    }
                    break;

                case "DataPrevisaoEntregaFormatada":
                    if (!select.Contains(" DataPrevisaoEntrega, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PREVISAO_ENTREGA DataPrevisaoEntrega, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PREVISAO_ENTREGA, ");
                    }
                    break;

                case "ProtocoloCarga":
                    if (!select.Contains(" ProtocoloCarga, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PROTOCOLO_CARGA ProtocoloCarga, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_PROTOCOLO_CARGA, ");
                    }
                    break;

                case "DataPrevisaoEstufagemFormatada":
                    if (!select.Contains(" DataPrevisaoEstufagem, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_PREVISAO_ESTUFAGEM DataPrevisaoEstufagem, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_PREVISAO_ESTUFAGEM, ");
                    }
                    break;

                case "DataConhecimentoFormatada":
                    if (!select.Contains(" DataConhecimento, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_DATA_CONHECIMENTO DataConhecimento, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_DATA_CONHECIMENTO, ");
                    }
                    break;

                case "CodigoOriginal":
                    if (!select.Contains(" CodigoOriginal, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ORIGINAL CodigoOriginal, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_CODIGO_ORIGINAL, ");
                    }
                    break;

                case "BookingTemporarioFormatado":
                    if (!select.Contains(" BookingTemporario, "))
                    {
                        select.Append("PedidoDadoTransporteMaritimo.CTM_BOOKING_TEMPORARIO BookingTemporario, ");
                        groupBy.Append("PedidoDadoTransporteMaritimo.CTM_BOOKING_TEMPORARIO, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(FiltroPesquisaRelatorioPedidoDadosTransporteMaritimo filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            where.Append(" AND CTM_CODIGO_ORIGINAL = 0 ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCargaEmbarcador))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCargaEmbarcador}' ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.Origem > 0)
            {
                where.Append($" AND RemetentePedido.LOC_CODIGO = {filtrosPesquisa.Origem} ");
                SetarJoinsRemetentePedido(joins);
            }

            if (filtrosPesquisa.Destino > 0)
            {
                where.Append($" AND RecebedorPedido.LOC_CODIGO = {filtrosPesquisa.Destino} ");
                SetarJoinsRecebedorPedido(joins);
            }

            if (filtrosPesquisa.Filial > 0)
                where.Append($" AND PedidoDadoTransporteMaritimo.FIL_CODIGO = {filtrosPesquisa.Filial} ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroEXP))
            {
                where.Append($" AND Pedido.PED_NUMERO_EXP = '{filtrosPesquisa.NumeroEXP}' ");
                SetarJoinsPedido(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
            {
                where.Append($" AND Pedido.PED_NUMERO_BOOKING = '{filtrosPesquisa.NumeroBooking}' ");
                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.DataInicial.HasValue && filtrosPesquisa.DataInicial.Value != DateTime.MinValue)
                where.Append($" AND PedidoDadoTransporteMaritimo.CTM_DATA_BOOKING >= '{filtrosPesquisa.DataInicial.Value.ToString("yyyyMMdd")}' ");


            if (filtrosPesquisa.DataFim.HasValue && filtrosPesquisa.DataFim.Value != DateTime.MinValue)
                where.Append($" AND PedidoDadoTransporteMaritimo.CTM_DATA_BOOKING < '{filtrosPesquisa.DataFim.Value.AddDays(1).ToString("yyyyMMdd")}' ");

            if (filtrosPesquisa.Status.HasValue)
                where.Append($" AND PedidoDadoTransporteMaritimo.CTM_STATUS = {(int)filtrosPesquisa.Status.Value} ");
    }

        #endregion
    }
}
