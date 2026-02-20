using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaAgendamentoEntregaPedido : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega>
    {
        #region Construtores

        public ConsultaAgendamentoEntregaPedido() : base(tabela: "T_CARGA_PEDIDO as CargaPedido") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCarga(StringBuilder joins)
        {
            if (!joins.Contains(" Carga "))
                joins.Append(" INNER JOIN T_CARGA Carga ON Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            if (!joins.Contains(" Pedido "))
                joins.Append(" INNER JOIN T_PEDIDO Pedido ON Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" INNER JOIN T_TIPO_OPERACAO TipoOperacao ON TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            SetarJoinsPedido(joins);
            if (!joins.Contains(" Destinatario "))
                joins.Append(" INNER JOIN T_CLIENTE Destinatario ON Destinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsLocalidade(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);
            if (!joins.Contains(" Localidade "))
                joins.Append(" INNER JOIN T_LOCALIDADES Localidade ON Localidade.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        private void SetarJoinsEmpresa(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" Empresa "))
                joins.Append(" INNER JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsJanelaCarregamento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);
            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" INNER JOIN T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento ON CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtroPesquisa)
        {
            switch (propriedade)
            {
                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");
                    }
                    SetarJoinsCarga(joins);

                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");
                    }
                    SetarJoinsTipoOperacao(joins);

                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Destinatario.CLI_NOME as Cliente, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");
                    }
                    SetarJoinsDestinatario(joins);

                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("Localidade.LOC_DESCRICAO as Destino, ");
                        groupBy.Append("Localidade.LOC_DESCRICAO, ");
                    }
                    SetarJoinsLocalidade(joins);

                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Empresa.EMP_RAZAO as Transportador, ");
                        groupBy.Append("Empresa.EMP_RAZAO, ");
                    }
                    SetarJoinsEmpresa(joins);

                    break;

                case "ObservacaoReagendamento":
                    if (!select.Contains(" ObservacaoReagendamento, "))
                    {
                        select.Append("Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO as ObservacaoReagendamento, ");
                        groupBy.Append("Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "QtdVolumes":
                    if (!select.Contains(" QtdVolumes, "))
                    {
                        select.Append("Pedido.PED_QUANTIDADE_VOLUMES as QtdVolumes, ");
                        groupBy.Append("Pedido.PED_QUANTIDADE_VOLUMES, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "QtdMetrosCubicos":
                    if (!select.Contains(" QtdMetrosCubicos, "))
                    {
                        select.Append("Pedido.PED_CUBAGEM_TOTAL as QtdMetrosCubicos, ");
                        groupBy.Append("Pedido.PED_CUBAGEM_TOTAL, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataSugestaoEntregaFormatada":
                    if (!select.Contains(" DataSugestaoEntrega, "))
                    {
                        select.Append("Carga.CAR_DATA_SUGESTAO_ENTREGA as DataSugestaoEntrega, ");
                        groupBy.Append("Carga.CAR_DATA_SUGESTAO_ENTREGA, ");
                    }
                    SetarJoinsCarga(joins);

                    break;

                case "TipoAgendamentoEntrega":
                    if (!select.Contains(" TipoAgendamentoEntrega, "))
                    {
                        select.Append("Pedido.PED_TIPO_AGENDAMENTO_ENTREGA as TipoAgendamentoEntrega, ");
                        groupBy.Append("Pedido.PED_TIPO_AGENDAMENTO_ENTREGA, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataCarregamentoInicialFormatada":
                    if (!select.Contains(" DataCarregamentoInicial, "))
                    {
                        select.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO as DataCarregamentoInicial, ");
                        groupBy.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataCarregamentoFinalFormatada":
                    if (!select.Contains(" DataCarregamentoFinal, "))
                    {
                        select.Append("Pedido.PED_DATA_TERMINO_CARREGAMENTO as DataCarregamentoFinal, ");
                        groupBy.Append("Pedido.PED_DATA_TERMINO_CARREGAMENTO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataAgendamentoFormatada":
                    if (!select.Contains(" DataAgendamento, "))
                    {
                        select.Append("Pedido.PED_DATA_AGENDAMENTO as DataAgendamento, ");
                        groupBy.Append("Pedido.PED_DATA_AGENDAMENTO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataPrevisaoEntregaFormatada":
                    if (!select.Contains(" DataPrevisaoEntrega, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA as DataPrevisaoEntrega, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "DataCriacaoPedidoFormatada":
                    if (!select.Contains(" DataCriacaoPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO as DataCriacaoPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "SituacaoCargaFormatada":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO as SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");
                    }
                    SetarJoinsCarga(joins);

                    break;

                case "SituacaoAgendamentoEntregaPedidoFormatada":
                    if (!select.Contains(" SituacaoAgendamentoEntregaPedido, "))
                    {
                        select.Append("Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO as SituacaoAgendamentoEntregaPedido, ");
                        groupBy.Append("Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO, ");
                    }
                    SetarJoinsPedido(joins);

                    break;

                case "ContatoCliente":
                    if (!select.Contains(" ContatoCliente, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _anexoCliente.ANX_DESCRICAO 
                                  from T_AGENDAMENTO_ENTREGA_PEDIDO_CLIENTE_ANEXOS _anexoCliente 
                                 where _anexoCliente.PED_CODIGO = CargaPedido.PED_CODIGO
                                   for xml path('')
                            ), 3, 1000) ContatoCliente, "
                        );
                        groupBy.Append("CargaPedido.PED_CODIGO, ");
                    }

                    break;

                case "ContatoTransportador":
                    if (!select.Contains(" ContatoTransportador, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select distinct ', ' + _anexoTransportador.ANX_DESCRICAO 
                                  from T_AGENDAMENTO_ENTREGA_PEDIDO_TRANSPORTADOR_ANEXOS _anexoTransportador 
                                 where _anexoTransportador.PED_CODIGO = CargaPedido.PED_CODIGO
                                   for xml path('')
                            ), 3, 1000) ContatoTransportador, "
                        );
                        groupBy.Append("CargaPedido.PED_CODIGO, ");
                    }

                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais, "))
                    {
                        select.Append(
                            @" SUBSTRING(
                                (select distinct CAST(XmlNotaFiscal.NF_NUMERO AS varchar) + ', ' + (CASE when PedidoNotaFiscalParcial.CNP_NUMERO IS NULL then '' else + CAST(PedidoNotaFiscalParcial.CNP_NUMERO AS varchar) END)
                                FROM T_PEDIDO_XML_NOTA_FISCAL PedidoXmlNotaFiscal
                                left JOIN T_XML_NOTA_FISCAL XmlNotaFiscal ON XmlNotaFiscal.NFX_CODIGO = PedidoXmlNotaFiscal.NFX_CODIGO
                                left JOIN T_CARGA_PEDIDO _cargaPedido ON _cargaPedido.CPE_CODIGO = PedidoXmlNotaFiscal.CPE_CODIGO
                                left JOIN T_PEDIDO_NOTA_FISCAL_PARCIAL PedidoNotaFiscalParcial ON PedidoNotaFiscalParcial.PED_CODIGO = _cargaPedido.PED_CODIGO
                                WHERE _cargaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                    FOR XML path('')), 0, 1000) NotasFiscais, "
                                        );
                        groupBy.Append("CargaPedido.CPE_CODIGO, CargaPedido.PED_CODIGO, ");
                    }

                    break;

                default:
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendamentoEntrega filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            where.Append($" AND Carga.CAR_SITUACAO NOT IN (13, 18) ");
            SetarJoinsCarga(joins);

            where.Append($" AND Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO > 0 ");
            SetarJoinsPedido(joins);

            if (filtrosPesquisa.SituacaoAgendamento.HasValue)
            {
                where.Append($" AND Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO = {(int)filtrosPesquisa.SituacaoAgendamento} ");
                SetarJoinsPedido(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Carga))
            {
                where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.Carga}' ");
                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.CodigoCliente > 0)
            {
                where.Append($" AND Destinatario.CLI_CGCCPF = {filtrosPesquisa.CodigoCliente} ");
                SetarJoinsDestinatario(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" AND Empresa.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
                SetarJoinsEmpresa(joins);
            }

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                where.Append($" AND TipoOperacao.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");
                SetarJoinsTipoOperacao(joins);
            }

            if (filtrosPesquisa.DataCarregamentoInicial != DateTime.MinValue)
            {
                where.Append($" AND CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO >= '{filtrosPesquisa.DataCarregamentoInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsJanelaCarregamento(joins);
            }
            if (filtrosPesquisa.DataCarregamentoFinal != DateTime.MinValue)
            {
                where.Append($" AND CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO <= '{filtrosPesquisa.DataCarregamentoInicial.ToString(pattern)} 23:59:59' ");
                SetarJoinsJanelaCarregamento(joins);
            }


            if (filtrosPesquisa.DataAgendamentoInicial != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO >= '{filtrosPesquisa.DataAgendamentoInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsPedido(joins);
            }
            if (filtrosPesquisa.DataAgendamentoFinal != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_AGENDAMENTO <= '{filtrosPesquisa.DataAgendamentoFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.DataPrevisaoEntregaInicial != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsPedido(joins);
            }
            if (filtrosPesquisa.DataPrevisaoEntregaFinal != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_PREVISAO_ENTREGA <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.DataCriacaoPedidoInicial != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_CRIACAO >= '{filtrosPesquisa.DataCriacaoPedidoInicial.ToString(pattern)} 00:00:00' ");
                SetarJoinsPedido(joins);
            }
            if (filtrosPesquisa.DataCriacaoPedidoFinal != DateTime.MinValue)
            {
                where.Append($" AND Pedido.PED_DATA_CRIACAO <= '{filtrosPesquisa.DataCriacaoPedidoFinal.ToString(pattern)} 23:59:59' ");
                SetarJoinsPedido(joins);
            }


            if (filtrosPesquisa.DataInicialSugestaoEntrega != DateTime.MinValue)
            {
                where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA >= '{filtrosPesquisa.DataInicialSugestaoEntrega.ToString(pattern)} 00:00:00' ");
                SetarJoinsCarga(joins);
            }
            if (filtrosPesquisa.DataFinalSugestaoEntrega != DateTime.MinValue)
            {
                where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA <= '{filtrosPesquisa.DataFinalSugestaoEntrega.ToString(pattern)} 23:59:59' ");
                SetarJoinsCarga(joins);
            }


            if (filtrosPesquisa.PossuiDataSugestaoEntrega.HasValue)
            {
                if (filtrosPesquisa.PossuiDataSugestaoEntrega.Value)
                    where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA IS NOT NULL");
                else
                    where.Append($" AND Carga.CAR_DATA_SUGESTAO_ENTREGA IS NULL");

                SetarJoinsCarga(joins);
            }

            if (filtrosPesquisa.PossuiDataTerminoCarregamento.HasValue)
            {
                if (filtrosPesquisa.PossuiDataTerminoCarregamento.Value)
                    where.Append($" AND Pedido.PED_DATA_TERMINO_CARREGAMENTO IS NOT NULL");
                else
                    where.Append($" AND Pedido.PED_DATA_TERMINO_CARREGAMENTO IS NULL");

                SetarJoinsPedido(joins);
            }

            if (filtrosPesquisa.ExibirCargasAgrupadas)
                where.Append(" and Carga.CAR_CARGA_AGRUPADA = 1 AND Carga.CAR_CARGA_FECHADA = 1 ");

        }

        #endregion
    }
}
