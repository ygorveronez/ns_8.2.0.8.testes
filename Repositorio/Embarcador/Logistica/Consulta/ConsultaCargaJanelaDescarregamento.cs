using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaCargaJanelaDescarregamento : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento>
    {
        #region Construtores

        public ConsultaCargaJanelaDescarregamento() : base(tabela: "T_CARGA_JANELA_DESCARREGAMENTO CargaJanelaDescarregamento") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsAgendamentoColeta(StringBuilder joins)
        {
            if (!joins.Contains(" AgendamentoColeta "))
                joins.Append("left join T_AGENDAMENTO_COLETA AgendamentoColeta on AgendamentoColeta.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsCategoria(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            if (!joins.Contains(" CategoriaAgendamentoColeta "))
                joins.Append("left join T_CATEGORIA CategoriaAgendamentoColeta on CategoriaAgendamentoColeta.CTG_CODIGO = AgendamentoColeta.CTG_CODIGO ");
        }

        private void SetarJoinsAgendamentoPallet(StringBuilder joins)
        {
            if (!joins.Contains(" AgendamentoPallet "))
                joins.Append("left join T_AGENDAMENTO_PALLET AgendamentoPallet on AgendamentoPallet.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsAgendamentoColetaPedido(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" AgendamentoColetaPedido "))
                joins.Append("left join T_AGENDAMENTO_COLETA_PEDIDO AgendamentoColetaPedido on AgendamentoColetaPedido.ACO_CODIGO = AgendamentoColeta.ACO_CODIGO and AgendamentoColetaPedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsAgendamentoColetaRecebedor(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);

            if (!joins.Contains("RecebedorAgendamentoColeta"))
                joins.Append(" left join T_CLIENTE RecebedorAgendamentoColeta on RecebedorAgendamentoColeta.CLI_CGCCPF = AgendamentoColeta.CLI_RECEBEDOR ");
        }

        private void SetarJoinsAgendamentoEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" AgendamentEntrega "))
                joins.Append("left join T_AGENDAMENTO_ENTREGA AgendamentoEntrega on AgendamentoEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            if (!joins.Contains(" TipoOperacao "))
                joins.Append("left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            if (!joins.Contains(" ModeloVeicular "))
                joins.Append("left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CargaEntrega "))
                joins.Append("left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO and CargaEntrega.CLI_CODIGO_ENTREGA = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO and CargaEntrega.CEN_COLETA = 0 ");
        }

        private void SetarJoinsCargaJanelaDescarregamentoPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" CargaJanelaDescarregamentoPedido "))
                joins.Append("left join T_CARGA_JANELA_DESCARREGAMENTO_PEDIDO CargaJanelaDescarregamentoPedido on CargaJanelaDescarregamentoPedido.CJD_CODIGO = CargaJanelaDescarregamento.CJD_CODIGO and CargaJanelaDescarregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append($"left join T_CARGA_PEDIDO CargaPedido on CargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO and CargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota} ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append("left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            if (!joins.Contains(" Filial "))
                joins.Append("left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsFornecedor(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);

            if (!joins.Contains(" Fornecedor "))
                joins.Append("left join T_CLIENTE Fornecedor on Fornecedor.CLI_CGCCPF = AgendamentoColeta.REM_CODIGO ");
        }

        private void SetarJoinsMonitoramento(StringBuilder joins)
        {
            if (!joins.Contains(" Monitoramento "))
            {
                joins.Append($@"
                    left join T_MONITORAMENTO Monitoramento on Monitoramento.MON_CODIGO in (
                        select max(_monitoramento.MON_CODIGO)
                          from T_MONITORAMENTO _monitoramento
                         where _monitoramento.CAR_CODIGO = Carga.CAR_CODIGO
                           and _monitoramento.VEI_CODIGO = Carga.CAR_VEICULO
                           and _monitoramento.MON_STATUS in ({(int)MonitoramentoStatus.Iniciado}, {(int)MonitoramentoStatus.Finalizado})
                    ) "
                );
            }
        }

        private void SetarJoinsMonitoramentoStatusViagem(StringBuilder joins)
        {
            SetarJoinsMonitoramento(joins);

            if (!joins.Contains(" MonitoramentoStatusViagem "))
                joins.Append("left join T_MONITORAMENTO_STATUS_VIAGEM MonitoramentoStatusViagem on MonitoramentoStatusViagem.MSV_CODIGO = Monitoramento.MSV_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append("left join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsPosicaoAtual(StringBuilder joins)
        {
            if (!joins.Contains(" PosicaoAtual "))
                joins.Append("left join T_POSICAO_ATUAL PosicaoAtual on PosicaoAtual.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsProdutoPrincipal(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" ProdutoPrincipal "))
                joins.Append("left join T_PRODUTO_EMBARCADOR ProdutoPrincipal on ProdutoPrincipal.PRO_CODIGO = Pedido.PRO_CODIGO ");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins)
        {
            if (!joins.Contains(" TipoCarga "))
                joins.Append("left join T_TIPO_DE_CARGA TipoCarga on TipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
        }

        private void SetarJoinsTipoCargaAgendamentoColeta(StringBuilder joins)
        {
            SetarJoinsAgendamentoColeta(joins);

            if (!joins.Contains(" TipoCargaAgendamentoColeta "))
                joins.Append("left join T_TIPO_DE_CARGA TipoCargaAgendamentoColeta on TipoCargaAgendamentoColeta.TCG_CODIGO = AgendamentoColeta.TCC_CODIGO ");
        }

        private void SetarJoinsTipoCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" TipoCargaPedido "))
                joins.Append("left join T_TIPO_DE_CARGA TipoCargaPedido on TipoCargaPedido.TCG_CODIGO = Pedido.TCG_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            if (!joins.Contains(" Transportador "))
                joins.Append("left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsSituacaoCadastrada(StringBuilder joins)
        {
            if (!joins.Contains(" SituacaoCadastrada "))
                joins.Append("left join T_JANELA_DESCARREGAMENTO_SITUACAO SituacaoCadastrada on SituacaoCadastrada.JDS_CODIGO = CargaJanelaDescarregamento.JDS_CODIGO ");
        }

        private void SetarJoinsVeiculo(StringBuilder joins)
        {
            if (!joins.Contains(" Veiculo "))
                joins.Append("left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = Carga.CAR_VEICULO ");
        }

        private void SetarJoinsCargaDadosSumarizados(StringBuilder joins)
        {
            if (!joins.Contains(" CargaDadosSumarizados "))
                joins.Append("left join T_CARGA_DADOS_SUMARIZADOS CargaDadosSumarizados on CargaDadosSumarizados.CDS_CODIGO = Carga.CDS_CODIGO ");
        }

        private void SetarJoinsFaixaTemperatura(StringBuilder joins)
        {
            if (!joins.Contains(" FaixaTemperatura "))
                joins.Append("LEFT JOIN T_FAIXA_TEMPERATURA FaixaTemperatura ON FaixaTemperatura.FTE_CODIGO = Carga.FTE_CODIGO ");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento filtroPesquisa)
        {
            switch (propriedade)
            {
                case "AgendaExtra":
                case "AgendaExtraDescricao":
                    if (!select.Contains(" AgendaExtra,"))
                        select.AppendLine("Carga.CAR_AGENDA_EXTRA as AgendaExtra, ");
                    break;

                case "Categoria":
                    if (!select.Contains(" Categoria,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("COALESCE(CategoriaAgendamentoColeta.CTG_DESCRICAO, ProdutoPrincipal.GRP_DESCRICAO) as Categoria, ");
                            SetarJoinsProdutoPrincipal(joins);

                            SetarJoinsCategoria(joins);
                        }
                        else
                        {
                            select.AppendLine("CategoriaAgendamentoColeta.CTG_DESCRICAO as Categoria, ");
                            SetarJoinsCategoria(joins);
                        }
                    }
                    break;

                case "CentroDescarregamento":
                    if (!select.Contains(" CentroDescarregamento,"))
                        select.AppendLine("CentroDescarregamento.CED_DESCRICAO as CentroDescarregamento, ");
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                        select.AppendLine("CargaJanelaDescarregamento.CJD_CODIGO as Codigo, ");
                    break;

                case "CodigoAgendamentoColeta":
                    if (!select.Contains(" CodigoAgendamentoColeta,"))
                    {
                        select.AppendLine("AgendamentoColeta.ACO_CODIGO as CodigoAgendamentoColeta, ");
                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;
                case "CodigoAgendamentoPallet":
                    if (!select.Contains(" CodigoAgendamentoPallet,"))
                    {
                        select.AppendLine("AgendamentoPallet.ACP_CODIGO as CodigoAgendamentoPallet, ");
                        SetarJoinsAgendamentoPallet(joins);
                    }
                    break;

                case "CodigoAgendamentoColetaPedido":
                    if (!select.Contains(" CodigoAgendamentoColetaPedido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("AgendamentoColetaPedido.ACP_CODIGO as CodigoAgendamentoColetaPedido, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                            select.AppendLine("0 as CodigoAgendamentoColetaPedido, ");
                    }
                    break;

                case "CodigoCarga":
                    if (!select.Contains(" CodigoCarga,"))
                        select.AppendLine("CargaJanelaDescarregamento.CAR_CODIGO as CodigoCarga, ");
                    break;

                case "CodigoCargaEmbarcador":
                    if (!select.Contains(" CodigoCargaEmbarcador,"))
                        select.AppendLine("Carga.CAR_CODIGO_CARGA_EMBARCADOR as CodigoCargaEmbarcador, ");
                    break;

                case "CodigoMonitoramento":
                    if (!select.Contains(" CodigoMonitoramento,"))
                    {
                        select.AppendLine("Monitoramento.MON_CODIGO as CodigoMonitoramento, ");
                        SetarJoinsMonitoramento(joins);
                    }
                    break;

                case "CodigoPedido":
                    if (!select.Contains(" CodigoPedido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("Pedido.PED_CODIGO as CodigoPedido, ");
                            SetarJoinsPedido(joins);
                        }
                        else
                            select.AppendLine("0 as CodigoPedido, ");
                    }
                    break;

                case "CPFCNPJDestinatario":
                    if (!select.Contains(" CPFCNPJDestinatario,"))
                    {
                        select.AppendLine("Destinatario.CLI_CGCCPF CPFCNPJDestinatario, ");
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ExibirJanelaDescargaPorPedido":
                    if (!select.Contains(" ExibirJanelaDescargaPorPedido,"))
                    {
                        select.AppendLine("CentroDescarregamento.CED_EXIBIR_JANELA_DESCARGA_POR_PEDIDO ExibirJanelaDescargaPorPedido, ");
                    }
                    break;

                case "DataAgendamentoColeta":
                case "DataAgendamento":
                case "HoraAgendamento":
                    if (!select.Contains(" DataAgendamentoColeta,"))
                    {
                        select.AppendLine("AgendamentoColeta.ACO_DATA_AGENDAMENTO as DataAgendamentoColeta, ");
                        SetarJoinsAgendamentoColeta(joins);
                    }

                    SetarSelect("DataAgendamentoPallet", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);

                    break;
                case "DataAgendamentoPallet":
                    if (!select.Contains(" DataAgendamentoPallet,"))
                    {
                        select.AppendLine("AgendamentoPallet.ACP_DATA_AGENDAMENTO as DataAgendamentoPallet, ");
                        SetarJoinsAgendamentoPallet(joins);
                    }
                    break;

                case "DataDescarregamento":
                case "InicioDescarregamento":
                    if (!select.Contains(" InicioDescarregamento,"))
                        select.AppendLine("CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO as InicioDescarregamento, ");
                    break;

                case "DataEmissaoCTe":
                case "DataEmissaoCTeFormatado":
                    if (!select.Contains(" DataEmissaoCTe,"))
                    {
                        select.AppendLine("(\tSelect\tTop 1 CTE.CON_DATAHORAEMISSAO\r\n\t\t\tFrom\tT_CARGA_CTE CargaCTe\r\n\t\t\t\t\tLeft Join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO \r\n\t\t\tWhere\tCargaCTe.CAR_CODIGO = Carga.CAR_CODIGO\r\n\t\t\tOrder By 1 Desc\r\n\t\t) as DataEmissaoCTe, ");
                    }
                    break;

                case "DataEntradaRaio":
                case "DataEntradaRaioFormatada":
                    if (!select.Contains(" DataEntradaRaio,"))
                    {
                        select.AppendLine("CargaEntrega.CEN_DATA_ENTRADA_RAIO as DataEntradaRaio, ");
                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataFimJanela":
                case "DataValidadePedido":
                    if (!select.Contains(" DataValidadePedido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("Pedido.PED_DATA_VALIDADE as DataValidadePedido, ");
                            SetarJoinsPedido(joins);
                        }
                        else
                            select.AppendLine("null as DataValidadePedido, ");
                    }
                    break;

                case "DataLancamento":
                case "DataLancamentoDescricao":
                    if (!select.Contains(" DataLancamento,"))
                        select.AppendLine("Carga.CAR_DATA_CRIACAO as DataLancamento, ");
                    break;

                case "DataProximaEntregaReprogramada":
                case "DataProximaEntregaReprogramadaFormatada":
                    if (!select.Contains(" DataProximaEntregaReprogramada,"))
                    {
                        select.AppendLine($@"
                            (
                                select top(1) case
			                               when CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA > CargaEntrega.CEN_DATA_ENTREGA_PREVISTA then CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA
					                       else CargaEntrega.CEN_DATA_ENTREGA_PREVISTA
                                       end 
                                  from T_CARGA_ENTREGA _cargaEntrega
                                 where _cargaEntrega.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                   and _cargaEntrega.CEN_SITUACAO != {(int)SituacaoEntrega.Entregue}
                                 order by 1
                            ) DataProximaEntregaReprogramada, "
                        );
                    }
                    break;

                case "DescricaoSituacao":
                case "Situacao":
                    if (!select.Contains(" Situacao,"))
                        select.AppendLine("CargaJanelaDescarregamento.CJD_SITUACAO as Situacao, ");
                    break;

                case "SituacaoCarga":
                    if (!select.Contains(" SituacaoCarga,"))
                        select.AppendLine("Carga.CAR_SITUACAO as SituacaoCarga, ");
                    break;

                case "DescricaoSituacaoCadastrada":
                    if (!select.Contains(" DescricaoSituacaoCadastrada,"))
                    {
                        select.AppendLine("SituacaoCadastrada.JDS_DESCRICAO as DescricaoSituacaoCadastrada, ");
                        SetarJoinsSituacaoCadastrada(joins);
                    }
                    break;

                case "CodigoSituacaoCadastrada":
                    if (!select.Contains(" CodigoSituacaoCadastrada,"))
                        select.AppendLine("CargaJanelaDescarregamento.JDS_CODIGO as CodigoSituacaoCadastrada, ");
                    break;

                case "Destinatario":
                    if (!select.Contains(" DestinatarioNome,"))
                    {
                        select.AppendLine(@"
                            Destinatario.CLI_NOME as DestinatarioNome,
                            Destinatario.CLI_NOMEFANTASIA DestinatarioNomeFantasia,
                            Destinatario.CLI_CODIGO_INTEGRACAO DestinatarioCodigoIntegracao,
                            Destinatario.CLI_FISJUR DestinatarioTipo,
                            Destinatario.CLI_PONTO_TRANSBORDO DestinatarioPontoTransbordo, "
                        );

                        SetarSelect("CPFCNPJDestinatario", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "Excedente":
                case "SemHorario":
                    if (!select.Contains(" SemHorario,"))
                        select.AppendLine("CargaJanelaDescarregamento.CJD_EXCEDENTE as SemHorario, ");
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.AppendLine("Filial.FIL_DESCRICAO as Filial, ");
                        SetarJoinsFilial(joins);
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" FornecedorNome,"))
                    {
                        select.AppendLine(@"
                            Fornecedor.CLI_NOME as FornecedorNome,
                            Fornecedor.CLI_NOMEFANTASIA FornecedorNomeFantasia,
                            Fornecedor.CLI_CODIGO_INTEGRACAO FornecedorCodigoIntegracao,
                            Fornecedor.CLI_FISJUR FornecedorTipo,
                            Fornecedor.CLI_CGCCPF CPFCNPJFornecedor,
                            Fornecedor.CLI_PONTO_TRANSBORDO FornecedorPontoTransbordo, "
                        );

                        SetarJoinsFornecedor(joins);
                    }
                    break;

                case "HoraDescarregamento":
                    SetarSelect("InicioDescarregamento", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("SemHorario", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    SetarSelect("ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                    break;

                case "KmAteDestino":
                case "KmAteDestinoFormatado":
                    if (!select.Contains(" KmAteDestino,"))
                    {
                        select.AppendLine("CargaEntrega.CEN_DISTANCIA_ATE_DESTINO as KmAteDestino, ");
                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "Modalidade":
                    if (!select.Contains(" Modalidade,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("TipoCargaPedido.TCG_DESCRICAO as Modalidade, ");
                            SetarJoinsTipoCargaPedido(joins);
                        }
                        else
                            select.AppendLine("null as Modalidade, ");
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select.AppendLine("ModeloVeicular.MVC_DESCRICAO as ModeloVeicular, ");
                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista,"))
                    {
                        select.AppendLine(@"
                            substring((
                                select distinct ', ' + _motorista.FUN_NOME
                                  from T_CARGA_MOTORISTA _cargaMotorista
                                  join T_FUNCIONARIO _motorista on _motorista.FUN_CODIGO = _cargaMotorista.CAR_MOTORISTA
                                 where _cargaMotorista.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                   for xml path('')
                            ), 3, 1000) as Motorista, "
                        );
                    }
                    break;

                case "NotasFiscaisEntrega":
                    if (!select.Contains(" NotasFiscaisEntrega,"))
                    {
                        select.AppendLine(@"
                            substring((
                                select distinct ', ' + convert(varchar(20), _notaFiscal.NF_NUMERO)
                                  from T_CARGA_ENTREGA_NOTA_FISCAL _entregaNotaFiscal
                                  join T_PEDIDO_XML_NOTA_FISCAL _pedidoNotaFiscal on _pedidoNotaFiscal.PNF_CODIGO = _entregaNotaFiscal.PNF_CODIGO
                                  join T_XML_NOTA_FISCAL _notaFiscal on _notaFiscal.NFX_CODIGO = _pedidoNotaFiscal.NFX_CODIGO
                                 where _entregaNotaFiscal.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                   for xml path('')
                            ), 3, 1000) as NotasFiscaisEntrega, "
                        );

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR as NumeroPedido, ");
                            SetarJoinsPedido(joins);
                        }
                        else
                        {
                            select.AppendLine($@"
                                substring((
                                    select distinct ', ' + _pedido.PED_NUMERO_PEDIDO_EMBARCADOR
                                      from T_CARGA_PEDIDO _cargaPedido
                                      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                       and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                       for xml path('')
                                ), 3, 1000) as NumeroPedido, "
                            );
                        }
                    }
                    break;

                case "ObservacaoPedido":
                    if (!select.Contains(" ObservacaoPedido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("Pedido.PED_OBSERVACAO as ObservacaoPedido, ");
                            SetarJoinsPedido(joins);
                        }
                        else
                        {
                            select.AppendLine($@"
                                substring((
                                    select distinct ', ' + _pedido.PED_OBSERVACAO
                                      from T_CARGA_PEDIDO _cargaPedido
                                      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                       and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                       for xml path('')
                                ), 3, 1000) as ObservacaoPedido, "
                            );
                        }
                    }
                    break;

                case "OrigemCarga":
                    if (!select.Contains(" OrigemCarga,"))
                    {
                        select.AppendLine($@"
                            (
                                select top(1) _localidade.LOC_DESCRICAO + ' - ' +
                                       case
                                           when (_localidade.LOC_IBGE <> 9999999 and _localidade.PAI_CODIGO is null) then isnull(_localidade.UF_SIGLA, '')
                                           when (_localidadePais.PAI_ABREVIACAO is null) then isnull(_localidadePais.PAI_NOME, '')
                                           else isnull(_localidadePais.PAI_ABREVIACAO, '')
                                       end
                                  from T_CARGA_PEDIDO _cargaPedido
                                  join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                  left join T_CLIENTE _expedidor on _expedidor.CLI_CGCCPF = _cargaPedido.CLI_CODIGO_EXPEDIDOR
                                  left join T_LOCALIDADES _localidade on _localidade.LOC_CODIGO = isnull(_expedidor.LOC_CODIGO, _cargaPedido.LOC_CODIGO_ORIGEM)
                                  left join T_UF _localidadeEstado on _localidadeEstado.UF_SIGLA = _localidade.UF_SIGLA
                                  left join T_PAIS _localidadePais on _localidadePais.PAI_CODIGO = _localidade.PAI_CODIGO
                                 where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                   and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                   and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                 order by _cargaPedido.PED_ORDEM_COLETA
                            ) as OrigemCarga, "
                        );
                    }
                    break;

                case "PermiteInformarAcaoParcial":
                    if (!select.Contains(" PermiteInformarAcaoParcial,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("cast((case when CargaJanelaDescarregamentoPedido.JDP_CODIGO is null then 1 else 0 end) as bit) as PermiteInformarAcaoParcial, ");
                            SetarJoinsCargaJanelaDescarregamentoPedido(joins);
                        }
                        else
                            select.AppendLine("cast(0 as bit) as PermiteInformarAcaoParcial, ");
                    }
                    break;

                case "Peso":
                case "PesoFormatado":
                    if (!select.Contains(" Peso,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("CargaPedido.PED_PESO as Peso, ");
                            SetarJoinsCargaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine($@"
                                (
                                    select sum(_cargaPedido.PED_PESO)
                                      from T_CARGA_PEDIDO _cargaPedido
                                      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                       and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                ) as Peso, "
                            );
                        }
                    }
                    break;

                case "PesoLiquido":
                case "PesoLiquidoFormatado":
                    if (!select.Contains(" PesoLiquido,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("CargaPedido.PED_PESO_LIQUIDO as PesoLiquido, ");
                            SetarJoinsCargaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine($@"
                                (
                                    select sum(_cargaPedido.PED_PESO_LIQUIDO)
                                      from T_CARGA_PEDIDO _cargaPedido
                                      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                       and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                ) as PesoLiquido, "
                            );
                        }
                    }
                    break;

                case "PrevisaoEntrega":
                    if (!select.Contains(" PrevisaoEntrega,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("convert(varchar(10), Pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + convert(varchar(5), Pedido.PED_PREVISAO_ENTREGA, 108) as PrevisaoEntrega, ");
                            SetarJoinsPedido(joins);
                        }
                        else
                        {
                            select.AppendLine($@"
                                substring((
                                    select distinct ', ' + convert(varchar(10), _pedido.PED_PREVISAO_ENTREGA, 103) + ' ' + convert(varchar(5), _pedido.PED_PREVISAO_ENTREGA, 108)
                                      from T_CARGA_PEDIDO _cargaPedido
                                      join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO
                                     where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota}
                                       and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO
                                       for xml path('')
                                ), 3, 1000) as PrevisaoEntrega, "
                            );
                        }
                    }
                    break;

                case "StatusBuscaSenhaAutomatica":
                    if (!select.Contains(" StatusBuscaSenhaAutomatica,"))
                    {
                        select.AppendLine("(case when isnull(AgendamentoColeta.ACO_SENHA, '') = '' then AgendamentoColeta.ACO_ERRO_BUSCAR_SENHA_AUTOMATICAMENTE else '' end) as StatusBuscaSenhaAutomatica, ");
                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;
                case "SenhaAgendamento":
                    if (!select.Contains(" SenhaAgendamentoColeta,"))
                    {
                        SetarSelect("StatusBuscaSenhaAutomatica", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);
                        select.AppendLine("AgendamentoColeta.ACO_SENHA as SenhaAgendamentoColeta, ");
                    }

                    SetarSelect("SenhaAgendamentoPallet", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtroPesquisa);

                    break;

                case "SenhaAgendamentoPallet":
                    if (!select.Contains(" SenhaAgendamentoPallet,"))
                    {
                        select.AppendLine("AgendamentoPallet.ACP_SENHA as SenhaAgendamentoPallet, ");
                        SetarJoinsAgendamentoPallet(joins);
                    }
                    break;

                case "StatusViagem":
                    if (!select.Contains(" StatusViagem,"))
                    {
                        select.AppendLine("MonitoramentoStatusViagem.MSV_DESCRICAO as StatusViagem, ");
                        SetarJoinsMonitoramentoStatusViagem(joins);
                    }
                    break;

                case "QtdItens":
                    if (!select.Contains(" QtdItens,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("AgendamentoColetaPedido.ACP_SKU as QtdItens, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine(@"
                                (
                                    select sum(_agendamentoColetaPedido.ACP_SKU)
                                      from T_AGENDAMENTO_COLETA_PEDIDO _agendamentoColetaPedido
                                     where _agendamentoColetaPedido.ACO_CODIGO = AgendamentoColeta.ACO_CODIGO
                                ) as QtdItens, "
                            );

                            SetarJoinsAgendamentoColeta(joins);
                        }
                    }
                    break;

                case "TerminoDescarregamento":
                    if (!select.Contains(" TerminoDescarregamento,"))
                        select.AppendLine("CargaJanelaDescarregamento.CJD_TERMINO_DESCARREGAMENTO as TerminoDescarregamento, ");
                    break;

                case "TipoAcaoParcial":
                    if (!select.Contains(" TipoAcaoParcial,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("CargaJanelaDescarregamentoPedido.JDP_TIPO_ACAO_PARCIAL as TipoAcaoParcial, ");
                            SetarJoinsCargaJanelaDescarregamentoPedido(joins);
                        }
                        else
                            select.AppendLine("null as TipoAcaoParcial, ");
                    }
                    break;

                case "TipoDeCarga":
                    if (!select.Contains(" TipoDeCarga,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("isnull(TipoCargaAgendamentoColeta.TCG_DESCRICAO, TipoCargaPedido.TCG_DESCRICAO) as TipoDeCarga, ");
                            SetarJoinsTipoCargaAgendamentoColeta(joins);
                            SetarJoinsTipoCargaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine("isnull(TipoCargaAgendamentoColeta.TCG_DESCRICAO, TipoCarga.TCG_DESCRICAO) as TipoDeCarga, ");
                            SetarJoinsTipoCargaAgendamentoColeta(joins);
                            SetarJoinsTipoCarga(joins);
                        }
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.AppendLine("TipoOperacao.TOP_DESCRICAO as TipoOperacao, ");
                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.AppendLine(@"(CASE WHEN (TRIM(AgendamentoColeta.ACO_TRANSPORTADOR_MANUAL) IS NOT NULL AND TRIM(AgendamentoColeta.ACO_TRANSPORTADOR_MANUAL) <> '') THEN AgendamentoColeta.ACO_TRANSPORTADOR_MANUAL 
                                                ELSE Transportador.EMP_RAZAO END) as Transportador, ");

                        SetarJoinsTransportador(joins);
                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;

                case "Veiculo":
                    if (!select.Contains(" Veiculo,"))
                    {
                        select.AppendLine(@"
                            (
                                Veiculo.VEI_PLACA +
                                isnull((
                                    select ', ' + _reboque.VEI_PLACA
                                      from T_CARGA_VEICULOS_VINCULADOS _veiculoVinculado
                                      join T_VEICULO _reboque on _reboque.VEI_CODIGO = _veiculoVinculado.VEI_CODIGO
                                     where _veiculoVinculado.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                                       for xml path('')
                                ), '')
                            ) Veiculo, "
                        );

                        SetarJoinsVeiculo(joins);
                    }
                    break;

                case "VolumeEmCx":
                    if (!select.Contains(" VolumeEmCx,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine(@"(CASE
			                                        WHEN ISNULL(CentroDescarregamento.CED_USAR_LAYOUT_AGENDAMENTO_POR_CAIXA_ITEM, 0) = 0
                                                        THEN CONVERT( INT, ISNULL(
                                                            (SELECT SUM( CEILING(
                                                                            AgendamentoColetaPedidoProduto.APP_QUANTIDADE /
                                                                            CAST( IIF( ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA > 0,
                                                                                ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA,
                                                                                1) AS DECIMAL) ))
					                                        FROM T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO AgendamentoColetaPedidoProduto
					                                            JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.PRP_CODIGO = AgendamentoColetaPedidoProduto.PRP_CODIGO
					                                            JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador ON ProdutoEmbarcador.PRO_CODIGO = PedidoProduto.PRO_CODIGO
					                                        WHERE AgendamentoColetaPedidoProduto.ACP_CODIGO = AgendamentoColetaPedido.ACP_CODIGO), AgendamentoColeta.ACO_VOLUMES))

                                                    WHEN (SELECT CEM_CONTROLAR_AGENDAMENTO_SKU FROM T_CONFIGURACAO_EMBARCADOR) = 1
			                                            THEN AgendamentoColetaPedido.ACP_VOLUMES_ENVIAR

			                                        ELSE AgendamentoColeta.ACO_VOLUMES
		                                        END) AS VolumeEmCx, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine("AgendamentoColeta.ACO_VOLUMES as VolumeEmCx, ");
                            SetarJoinsAgendamentoColeta(joins);
                        }
                    }
                    break;

                case "QtdProdutos":
                    if (!select.Contains(" QtdProdutos,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine(" (SELECT SUM(APP_QUANTIDADE) FROM T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO AgendamentoColetaPedidoProduto WHERE AgendamentoColetaPedidoProduto.ACP_CODIGO = AgendamentoColetaPedido.ACP_CODIGO) as QtdProdutos, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine("AgendamentoColeta.ACO_VOLUMES as QtdProdutos, ");
                            SetarJoinsAgendamentoColeta(joins);
                        }
                    }
                    break;

                case "QuantidadeArquivoIntegracao":
                    if (!select.Contains(" QuantidadeArquivoIntegracao, "))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine("(SELECT COUNT(AgendamentoColetaPedidoIntegracaoArquivo.CCA_CODIGO) FROM T_AGENDAMENTO_COLETA_PEDIDO_INTEGRACAO_ARQUIVO AgendamentoColetaPedidoIntegracaoArquivo WHERE AgendamentoColetaPedidoIntegracaoArquivo.ACP_CODIGO = AgendamentoColetaPedido.ACP_CODIGO) QuantidadeArquivoIntegracao, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                            select.AppendLine("0 as QuantidadeArquivoIntegracao, ");
                    }
                    break;

                case "RastreadorOnline":
                    if (!select.Contains(" RastreadorOnline,"))
                    {
                        select.AppendLine($"cast((case when (PosicaoAtual.POA_DATA_VEICULO is not null and dateadd(minute, {filtroPesquisa.TempoSemPosicaoParaVeiculoPerderSinal}, PosicaoAtual.POA_DATA_VEICULO) < getdate()) then 1 else 0 end) as bit) as RastreadorOnline, ");

                        SetarJoinsPosicaoAtual(joins);
                    }
                    break;

                case "ObservacaoFluxoPatio":
                    if (!select.Contains(" ObservacaoFluxoPatio, "))
                    {
                        select.AppendLine("CargaJanelaDescarregamento.CJD_OBSERVACAO_FLUXO_PATIO as ObservacaoFluxoPatio, ");
                        select.AppendLine(@"(SELECT TOP 1 SGP_CODIGO
                                           FROM T_SEQUENCIA_GESTAO_PATIO SequenciaGestaoPatioDestino
                                           WHERE SequenciaGestaoPatioDestino.FIL_CODIGO = Carga.FIL_CODIGO
                                           AND SequenciaGestaoPatioDestino.SGP_TIPO = 2
                                          ) as CodigoSequenciaGestaoPatioDestino, ");
                    }
                    break;

                case "DisponibilidadeVeiculo":
                    if (!select.Contains(" DataPrevisaoChegada, "))
                    {
                        select.AppendLine("CargaJanelaDescarregamento.CJD_DATA_PREVISAO_CHEGADA as DataPrevisaoChegada, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente, "))
                    {
                        select.AppendLine("CargaDadosSumarizados.CDS_REMETENTES Remetente, ");

                        SetarJoinsCargaDadosSumarizados(joins);
                    }

                    break;
                case "SetPointTransp":
                    if (!select.Contains(" SetPointTransp, "))
                    {
                        select.AppendLine("Carga.CAR_SET_POINT_VEICULO SetPointTransp, ");
                    }
                    break;

                case "RangeTempTransp":
                    if (!select.Contains(" RangeTempTransp, "))
                    {
                        select.AppendLine("CONCAT(FaixaTemperatura.FTE_FAIXA_FINAL, (CASE WHEN FaixaTemperatura.FTE_FAIXA_FINAL is not null or FaixaTemperatura.FTE_FAIXA_INICIAL is not null then ' até ' end), FaixaTemperatura.FTE_FAIXA_FINAL) RangeTempTransp, ");

                        SetarJoinsFaixaTemperatura(joins);
                    }
                    break;

                case "TipoCargaTaura":
                    if (!select.Contains(" TipoCargaTaura, "))
                    {
                        select.AppendLine("Carga.CAR_CATEGORIA_CARGA_EMBARCADOR TipoCargaTaura, ");
                    }
                    break;

                case "Valor":
                    if (!select.Contains(" Valor,"))
                    {
                        if (filtroPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.AppendLine(@"(CASE
                                           WHEN ISNULL(CentroDescarregamento.CED_USAR_LAYOUT_AGENDAMENTO_POR_CAIXA_ITEM, 0) = 0
                                                        THEN CONVERT( DECIMAL, ISNULL(
                                                            (SELECT SUM( CEILING(
                                                                            AgendamentoColetaPedidoProduto.APP_QUANTIDADE /
                                                                            CAST( IIF( ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA > 0,
                                                                                ProdutoEmbarcador.PRO_QUANTIDADE_CAIXA,
                                                                                1) AS DECIMAL) * (PedidoProduto.PRP_VALOR_PRODUTO / (CASE WHEN PedidoProduto.PRP_QUANTIDADE = 0 THEN 1 ELSE PedidoProduto.PRP_QUANTIDADE END))))
                                             FROM T_AGENDAMENTO_COLETA_PEDIDO_PRODUTO AgendamentoColetaPedidoProduto
                                                 JOIN T_PEDIDO_PRODUTO PedidoProduto ON PedidoProduto.PRP_CODIGO = AgendamentoColetaPedidoProduto.PRP_CODIGO
                                                 JOIN T_PRODUTO_EMBARCADOR ProdutoEmbarcador ON ProdutoEmbarcador.PRO_CODIGO = PedidoProduto.PRO_CODIGO
                                             WHERE AgendamentoColetaPedidoProduto.ACP_CODIGO = AgendamentoColetaPedido.ACP_CODIGO), AgendamentoColeta.ACO_VALOR_TOTAL_VOLUMES))

                                                    WHEN (SELECT CEM_CONTROLAR_AGENDAMENTO_SKU FROM T_CONFIGURACAO_EMBARCADOR) = 1
                                               THEN AgendamentoColetaPedido.ACP_VALOR_VOLUMES_ENVIAR

                                           ELSE AgendamentoColeta.ACO_VALOR_TOTAL_VOLUMES
                                          END) AS Valor, ");
                            SetarJoinsAgendamentoColetaPedido(joins);
                        }
                        else
                        {
                            select.AppendLine("AgendamentoColeta.ACO_VALOR_TOTAL_VOLUMES as Valor, ");
                            SetarJoinsAgendamentoColeta(joins);
                        }
                    }
                    break;

                case "ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga":
                    if (!select.Contains("ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga"))
                    {
                        select.AppendLine("(select CJC_EXIBIR_HORA_AGENDADA_PARA_CARGAS_EXCEDENTES_JANELA_DESCARGA from T_CONFIGURACAO_JANELA_CARREGAMENTO) AS ExibirHoraAgendadaParaCargasExcedentesJanelaDescarga, ");
                    }
                    break;
                case "UnidadeMedidaAgendamento":
                    if (!select.Contains(" UnidadeMedidaAgendamento,"))
                    {
                        select.AppendLine("AgendamentoColeta.ACO_UNIDADE as UnidadeMedidaAgendamento, ");
                        SetarJoinsAgendamentoColeta(joins);
                    }
                    break;
                case "QuantidadeNotas":
                    if (!select.Contains(" QuantidadeNotas,"))
                    {
                        select.AppendLine(@"(SELECT COUNT(_pedido_xml.NFX_CODIGO)
		                                    FROM VIEW_PEDIDO_XML _pedido_xml
			                                    JOIN T_CARGA_PEDIDO cargaPedido ON cargaPedido.PED_CODIGO = _pedido_xml.PED_CODIGO
		                                    AND cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                        ) AS QuantidadeNotas, ");
                    }
                    break;
                case "QuantidadeNotasAVIPED":
                    if (!select.Contains(" QuantidadeNotasAVIPED,"))
                    {
                        select.AppendLine(@"(select count(*)
                                            from
                                                T_INTEGRACAO_AVIPED integracaoAVIPED
                                            inner join
                                                T_CARGA_PEDIDO cargaPedido 
                                                    on integracaoAVIPED.CPE_CODIGO=cargaPedido.CPE_CODIGO 
                                            inner join
                                                T_CARGA carga 
                                                    on cargaPedido.CAR_CODIGO=carga.CAR_CODIGO 
                                            left outer join
                                                T_PEDIDO pedido
                                                    on cargaPedido.PED_CODIGO=pedido.PED_CODIGO 
                                            where
                                                carga.CAR_CODIGO=CargaJanelaDescarregamento.CAR_CODIGO
		                                        AND LEN(AVI_NUM_AVISO_RECEBIMENTO) > 0) as QuantidadeNotasAVIPED,
                        ");
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor,"))
                    {
                        select.AppendLine("RecebedorAgendamentoColeta.CLI_NOME Recebedor, ");

                        SetarJoinsAgendamentoColetaRecebedor(joins);
                    }
                    break;

                case "Motivo":
                    if (!select.Contains(" Motivo,"))
                    {
                        select.AppendLine("CargaJanelaDescarregamento.CJD_MOTIVO_REAGENDAMENTO Motivo, ");
                    }
                    break;

                case "QuantidadeNaoComparecimento":
                    if (!select.Contains(" QuantidadeNaoComparecimento,"))
                    {
                        select.AppendLine("CargaJanelaDescarregamento.CJD_QUANTIDADE_NAO_COMPARECIMENTO QuantidadeNaoComparecimento, ");
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            joins.Insert(0, @"
                join T_CARGA Carga on Carga.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO
                join T_CENTRO_DESCARREGAMENTO CentroDescarregamento on CentroDescarregamento.CED_CODIGO = CargaJanelaDescarregamento.CED_CODIGO "
            );

            where.Append($@"
                and isnull(CargaJanelaDescarregamento.CJD_CANCELADA, 0) = 0 
                and CargaJanelaDescarregamento.CED_CODIGO = {filtrosPesquisa.CodigoCentroDescarregamento} 
                and (
                      (Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Cancelada} and Carga.CAR_SITUACAO <> {(int)SituacaoCarga.Anulada}) or
                      (
                          CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.CargaDevolvida} or
                          CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.NaoComparecimento} or
                          CargaJanelaDescarregamento.CJD_SITUACAO = {(int)SituacaoCargaJanelaDescarregamento.NaoComparecimentoConfirmadoPeloFornecedor}
                      ) and CargaJanelaDescarregamento.CJD_SITUACAO <> {(int)SituacaoCargaJanelaDescarregamento.Cancelado}
                  )");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            else if (filtrosPesquisa.DataDescarregamento != DateTime.MinValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO between '{filtrosPesquisa.DataDescarregamento.Date.ToString("yyyyMMdd HH:mm:ss")}' and '{filtrosPesquisa.DataDescarregamento.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataDescarregamentoInicial != DateTime.MinValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO >= '{filtrosPesquisa.DataDescarregamentoInicial.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataDescarregamentoFinal != DateTime.MinValue)
                where.Append($" and CargaJanelaDescarregamento.CJD_INICIO_DESCARREGAMENTO <= '{filtrosPesquisa.DataDescarregamentoFinal.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.Situacao.Count > 0)
                where.Append($" and CargaJanelaDescarregamento.CJD_SITUACAO IN({string.Join(",", filtrosPesquisa.Situacao.Select(x => (int)x))}) ");

            if (filtrosPesquisa.SituacaoAgendamentoPallet.HasValue)
            {
                where.Append($" and AgendamentoPallet.ACP_SITUACAO = {(int)filtrosPesquisa.SituacaoAgendamentoPallet.Value} ");
                SetarJoinsAgendamentoPallet(joins);
            }

            if (filtrosPesquisa.CodigoSituacao > 0)
                where.Append($" and CargaJanelaDescarregamento.JDS_CODIGO = {filtrosPesquisa.CodigoSituacao} ");

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                where.Append($" and Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}) ");

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
                where.Append($" and ((Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})) OR (Carga.FIL_CODIGO_DESTINO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)}))) ");

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                where.Append($" and Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}) ");

            if (filtrosPesquisa.DataLancamento != DateTime.MinValue)
                where.Append($" and cast(Carga.CAR_DATA_CRIACAO as date) = '{filtrosPesquisa.DataLancamento.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.SenhaAgendamento))
            {
                where.Append($@" and (
                    AgendamentoColeta.ACO_SENHA = '{filtrosPesquisa.SenhaAgendamento}' or
                    AgendamentoEntrega.AGE_SENHA = '{filtrosPesquisa.SenhaAgendamento}' or
                    AgendamentoPallet.ACP_SENHA = '{filtrosPesquisa.SenhaAgendamento}') 
                ");
                SetarJoinsAgendamentoColeta(joins);
                SetarJoinsAgendamentoEntrega(joins);
                SetarJoinsAgendamentoPallet(joins);
            }

            if (filtrosPesquisa.CodigoFornecedor > 0)
            {
                where.Append($" and AgendamentoColeta.REM_CODIGO = {filtrosPesquisa.CodigoFornecedor} ");
                SetarJoinsAgendamentoColeta(joins);
            }

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" and Veiculo.VEI_CODIGO = {filtrosPesquisa.CodigoVeiculo} ");
                SetarJoinsVeiculo(joins);
            }

            if (filtrosPesquisa.CodigoTipoCarga > 0)
            {
                where.Append($" and TipoCarga.TCG_CODIGO = {filtrosPesquisa.CodigoTipoCarga} ");
                SetarJoinsTipoCarga(joins);
            }

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                where.Append($" and Transportador.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");
                SetarJoinsTransportador(joins);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroLacre))
            {
                where.Append($" and Carga.CAR_CODIGO in (SELECT CAR_CODIGO  FROM T_CARGA_LACRE WHERE CLA_NUMERO = :CLA_NUMERO)");
                parametros.Add(new Consulta.ParametroSQL("CLA_NUMERO", filtrosPesquisa.NumeroLacre));
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCTe))
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("      SELECT CAR_CODIGO FROM T_CARGA_CTE CargaCTe");
                where.Append("          JOIN T_CTE CTe on CargaCTe.CON_CODIGO = CTe.CON_CODIGO");
                where.Append($"     WHERE CTe.CON_NUM = '{filtrosPesquisa.NumeroCTe}'");
                where.Append("      )");
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroNF))
            {
                where.Append(" and Carga.CAR_CODIGO in (");
                where.Append("      SELECT CAR_CODIGO FROM T_CARGA_PEDIDO CargaPedido");
                where.Append("          JOIN T_PEDIDO_XML_NOTA_FISCAL XMLPedidoNotaFiscal on CargaPedido.CPE_CODIGO = XMLPedidoNotaFiscal.CPE_CODIGO");
                where.Append("          JOIN T_XML_NOTA_FISCAL XMLNotaFiscal ON XMLPedidoNotaFiscal.NFX_CODIGO = XMLNotaFiscal.NFX_CODIGO");
                where.Append($"     WHERE XMLNotaFiscal.NF_NUMERO = '{filtrosPesquisa.NumeroNF}'");
                where.Append("      )");
            }

            if (filtrosPesquisa.ExcedenteDescarregamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao.Todos)
                where.Append($" and CargaJanelaDescarregamento.CJD_EXCEDENTE = {(int)filtrosPesquisa.ExcedenteDescarregamento}");

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
            {
                SetarJoinsPedido(joins);

                where.Append($" and isnull(CargaPedido.CLI_CODIGO_RECEBEDOR, Pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO ");

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    where.Append($" and Pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($" and Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}' ");
            }
            else
            {
                where.Append(" and exists ( ");
                where.Append("         select top(1) 1 ");
                where.Append("           from T_CARGA_PEDIDO _cargaPedido ");
                where.Append("           join T_PEDIDO _pedido on _pedido.PED_CODIGO = _cargaPedido.PED_CODIGO ");
                where.Append("          where _cargaPedido.CAR_CODIGO = CargaJanelaDescarregamento.CAR_CODIGO ");
                where.Append($"           and _cargaPedido.PED_TIPO_CARREGAMENTO_PEDIDO <> {(int)TipoCarregamentoPedido.TrocaNota} ");
                where.Append("            and isnull(_cargaPedido.CLI_CODIGO_RECEBEDOR, _pedido.CLI_CODIGO) = CentroDescarregamento.CLI_CGCCPF_DESTINATARIO ");

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    where.Append($"       and _pedido.FIL_CODIGO_VENDA in ({string.Join(", ", filtrosPesquisa.CodigosFilialVenda)}) ");

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                    where.Append($"       and _pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}' ");

                where.Append("     ) ");
            }
        }

        #endregion
    }
}
