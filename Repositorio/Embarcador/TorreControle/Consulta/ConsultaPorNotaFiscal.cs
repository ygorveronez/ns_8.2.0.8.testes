using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.TorreControle.Consulta
{
    sealed class ConsultaPorNotaFiscal : Embarcador.Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal>
    {
        #region Construtores

        public ConsultaPorNotaFiscal() : base(tabela: "T_XML_NOTA_FISCAL as NotaFiscal") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsPedidoNotaFiscal(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoNotaFiscal "))
                joins.Append(" join T_PEDIDO_XML_NOTA_FISCAL PedidoNotaFiscal on PedidoNotaFiscal.NFX_CODIGO = NotaFiscal.NFX_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            SetarJoinsPedidoNotaFiscal(joins);

            if (!joins.Contains(" CargaPedido "))
                joins.Append(" join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = PedidoNotaFiscal.CPE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Carga "))
                joins.Append(" join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CargaJanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO CargaJanelaCarregamento on CargaJanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" TipoOperacao "))
                joins.Append(" left join T_TIPO_OPERACAO TipoOperacao on TipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
        }

        private void SetarJoinsPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" Pedido "))
                joins.Append(" join T_PEDIDO Pedido on Pedido.PED_CODIGO = CargaPedido.PED_CODIGO ");
        }

        private void SetarJoinsResponsavel(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" Responsavel "))
                joins.Append(" left join T_FUNCIONARIO Responsavel on Pedido.FUN_CODIGO = Responsavel.FUN_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" Destinatario "))
                joins.Append(" left join T_CLIENTE Destinatario on Destinatario.CLI_CGCCPF = NotaFiscal.CLI_CODIGO_DESTINATARIO ");
        }

        private void SetarJoinsGrupoPessoaDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" GrupoPessoaDestinatario "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaDestinatario on Destinatario.GRP_CODIGO = GrupoPessoaDestinatario.GRP_CODIGO ");
        }

        private void SetarJoinsCategoriaDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" CategoriaDestinatario "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaDestinatario on Destinatario.CTP_CODIGO = CategoriaDestinatario.CTP_CODIGO ");
        }

        private void SetarJoinsTipoResponsavel(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" TipoResponsavel "))
                joins.Append(" left join T_TIPO_RESPONSAVEL_ATRASO_ENTREGA TipoResponsavel on Pedido.PED_RESPONSAVEL_MOTIVO_REAGENDAMENTO = TipoResponsavel.TRA_CODIGO ");
        }

        private void SetarJoinsDestino(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" Destino "))
                joins.Append(" left join T_LOCALIDADES Destino on Destino.LOC_CODIGO = Destinatario.LOC_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Transportador "))
                joins.Append(" left join T_EMPRESA Transportador on Transportador.EMP_CODIGO = Carga.EMP_CODIGO ");
        }

        private void SetarJoinsCargaEntregaPedido(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CargaEntregaPedido "))
                joins.Append(" left join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEP_CODIGO = (select top 1 CEP_CODIGO from T_CARGA_ENTREGA_PEDIDO where CPE_CODIGO = CargaPedido.CPE_CODIGO order by CEP_CODIGO desc) ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins)
        {
            SetarJoinsCargaEntregaPedido(joins);

            if (!joins.Contains(" CargaEntrega "))
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CEN_CODIGO = CargaEntregaPedido.CEN_CODIGO ");
        }

        private void SetarJoinsViewTorreControle(StringBuilder joins)
        {
            SetarJoinsCargaEntrega(joins);

            if (!joins.Contains(" ViewTorreControle "))
                joins.Append(" left join V_TORRE_CONTROLE_CARGAS ViewTorreControle on ViewTorreControle.CEN_CODIGO = CargaEntrega.CEN_CODIGO ");
        }

        private void SetarJoinsPedidoAdicional(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" PedidoAdicional "))
                joins.Append(" left join T_PEDIDO_ADICIONAL PedidoAdicional ON PedidoAdicional.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsMotivoReagendamento(StringBuilder joins)
        {
            SetarJoinsPedido(joins);

            if (!joins.Contains(" MotivoReagendamento "))
                joins.Append(" left join T_MOTIVO_REAGENDAMENTO MotivoReagendamento ON Pedido.MRE_CODIGO = MotivoReagendamento.MRE_CODIGO");
        }

        private void SetarJoinsModeloVeicular(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" ModeloVeicular "))
                joins.Append(" join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
        }

        private void SetarJoinsTipoTrecho(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Trecho "))
                joins.Append(" left join T_TIPO_TRECHO Trecho on Trecho.TTR_CODIGO = Carga.TTR_CODIGO ");
        }

        private void SetarJoinsFilialEmissora(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CNPJFilialEmissora "))
                joins.Append(" left join T_EMPRESA FilialEmissora on FilialEmissora.EMP_CODIGO = Carga.EMP_CODIGO_FILIAL_EMISSORA ");
        }

        private void SetarJoinsFilial(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Filial "))
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
        }

        private void SetarJoinsExpedidor(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" Expedidor "))
                joins.Append(" left join T_CLIENTE Expedidor on Expedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
        }

        private void SetarJoinsCategoriaExpedidor(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CategoriaExpedidor "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaExpedidor on CategoriaExpedidor.CTP_CODIGO = Expedidor.CTP_CODIGO ");
        }

        private void SetarJoinsCidadeOrigemPedido(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" CidadeOrigemPedido "))
                joins.Append(" left join T_LOCALIDADES AS CidadeOrigemPedido ON Pedido.LOC_CODIGO_ORIGEM = CidadeOrigemPedido.LOC_CODIGO");
        }

        private void SetarJoinsVeiculos(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" PlacaVeiculo "))
                joins.Append(" left join T_CARGA_VEICULOS_VINCULADOS CargaVeiculo on CargaVeiculo.CAR_CODIGO = Carga.CAR_CODIGO");
            joins.Append(" left join T_VEICULO Veiculo on Veiculo.VEI_CODIGO = CargaVeiculo.VEI_CODIGO");
        }

        private void SetarJoinsFuncionario(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" AnalistaResponsavelMonitoramento "))
                joins.Append(" left join T_FUNCIONARIO AnalistaResponsavelMonitoramento on AnalistaResponsavelMonitoramento.FUN_CODIGO = Carga.CAR_ANALISTA_RESPONSAVEL_MONITORAMENTO");
        }

        private void SetarJoinsRotaFrete(StringBuilder joins)
        {
            SetarJoinsCarga(joins);

            if (!joins.Contains(" RotaFrete "))
                joins.Append(" left join T_ROTA_FRETE RotaFrete on RotaFrete.ROF_CODIGO = Carga.ROF_CODIGO");
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa)
        {
            if (!select.Contains(" Codigo, "))
            {
                select.Append("NotaFiscal.NFX_CODIGO Codigo, ");

                if (!groupBy.Contains("NotaFiscal.NFX_CODIGO"))
                    groupBy.Append("NotaFiscal.NFX_CODIGO, ");
            }

            switch (propriedade)
            {
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select.Append("NotaFiscal.NF_NUMERO Numero, ");
                        groupBy.Append("NotaFiscal.NF_NUMERO, ");
                    }
                    break;

                case "Carga":
                    if (!select.Contains(" Carga, "))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR Carga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "SituacaoNotaFiscal":
                    if (!select.Contains(" SituacaoNotaFiscal, "))
                    {
                        select.Append(@"(SELECT TOP 1 _notaFiscalSituacao.NFS_DESCRICAO
						 		         FROM T_NOTA_FISCAL_SITUACAO _notaFiscalSituacao
						 		         WHERE NotaFiscal.NFS_CODIGO = _notaFiscalSituacao.NFS_CODIGO) SituacaoNotaFiscal, ");
                        groupBy.Append("NotaFiscal.NFS_CODIGO, ");
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao, "))
                    {
                        select.Append("TipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("TipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins);
                    }
                    break;

                case "SituacaoAgendamentoDescricao":
                    if (!select.Contains(" SituacaoAgendamento, "))
                    {
                        select.Append("Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO SituacaoAgendamento, ");
                        groupBy.Append("Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "ObservacaoReagendamento":
                    if (!select.Contains(" ObservacaoReagendamento, "))
                    {
                        select.Append("Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO ObservacaoReagendamento, ");
                        groupBy.Append("Pedido.PED_OBSERVACAO_SOLICITACAO_REAGENDAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Cliente":
                    if (!select.Contains(" Cliente, "))
                    {
                        select.Append("Destinatario.CLI_NOME Cliente, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ClienteCnpj":
                    if (!select.Contains(" ClienteCnpj, "))
                    {
                        select.Append("Destinatario.CLI_CGCCPF as ClienteCnpj, ");
                        groupBy.Append("Destinatario.CLI_CGCCPF, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ClienteDescricao":
                    SetarSelect("Cliente", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("ClienteCnpj", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "Destino":
                    if (!select.Contains(" Destino, "))
                    {
                        select.Append("Destino.LOC_DESCRICAO as Destino, ");
                        groupBy.Append("Destino.LOC_DESCRICAO, ");

                        SetarJoinsDestino(joins);
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador, "))
                    {
                        select.Append("Transportador.EMP_FANTASIA as Transportador, ");
                        groupBy.Append("Transportador.EMP_FANTASIA, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "TransportadorCnpj":
                    if (!select.Contains(" TransportadorCnpj, "))
                    {
                        select.Append("Transportador.EMP_CNPJ TransportadorCnpj, ");
                        groupBy.Append("Transportador.EMP_CNPJ, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "TransportadorDescricao":
                    SetarSelect("Transportador", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("TransportadorCnpj", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "SugestaoDataEntregaFormatada":
                    if (!select.Contains(" SugestaoDataEntrega, "))
                    {
                        select.Append("Carga.CAR_DATA_SUGESTAO_ENTREGA SugestaoDataEntrega, ");
                        groupBy.Append("Carga.CAR_DATA_SUGESTAO_ENTREGA, ");

                        SetarJoinsCarga(joins);
                    }
                    break;

                case "DataReagendamentoFormatada":
                    if (!select.Contains(" DataReagendamento, "))
                    {
                        select.Append(@"(CASE WHEN Pedido.PED_DATA_AGENDAMENTO <> Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO 
									        THEN Pedido.PED_DATA_AGENDAMENTO
								        ELSE NULL END) DataReagendamento, ");

                        if (!groupBy.Contains("Pedido.PED_DATA_AGENDAMENTO"))
                            groupBy.Append("Pedido.PED_DATA_AGENDAMENTO, ");

                        if (!groupBy.Contains("Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO"))
                            groupBy.Append("Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataAgendamentoFormatada":
                    if (!select.Contains(" DataAgendamento, "))
                    {
                        select.Append("Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO DataAgendamento, ");

                        if (!groupBy.Contains("Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO"))
                            groupBy.Append("Pedido.PED_DATA_PRIMEIRO_AGENDAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataTerminoCarregamentoFormatada":
                    if (!select.Contains(" DataTerminoCarregamento, "))
                    {
                        select.Append("Pedido.PED_DATA_TERMINO_CARREGAMENTO DataTerminoCarregamento, ");
                        groupBy.Append("Pedido.PED_DATA_TERMINO_CARREGAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "DataInicioEntregaFormatada":
                    if (!select.Contains(" DataInicioEntrega, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA DataInicioEntrega, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_INICIO_ENTREGA"))
                            groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "DataFimEntregaFormatada":
                    if (!select.Contains(" DataFimEntrega, "))
                    {
                        select.Append(@"(CASE WHEN NotaFiscal.NF_SITUACAO_ENTREGA = 3
									        THEN NULL
								        ELSE CargaEntrega.CEN_DATA_ENTREGA END) DataFimEntrega, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    SetarSelect("SituacaoViagem", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    SetarSelect("SituacaoEntregaNotaFiscal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "DataPrevisaoEntregaFormatada":
                    if (!select.Contains(" DataPrevisaoEntrega, "))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA DataPrevisaoEntrega, ");

                        if (!groupBy.Contains("Pedido.PED_PREVISAO_ENTREGA"))
                            groupBy.Append("Pedido.PED_PREVISAO_ENTREGA , ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "SituacaoViagem":
                    if (!select.Contains(" SituacaoViagem, "))
                    {
                        select.Append("ViewTorreControle.STATUS SituacaoViagem, ");
                        groupBy.Append("ViewTorreControle.STATUS, ");

                        SetarJoinsViewTorreControle(joins);
                    }
                    break;

                case "SituacaoEntregaNotaFiscalDescricao":
                    SetarSelect("SituacaoEntregaNotaFiscal", codigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                    break;

                case "SituacaoEntregaNotaFiscal":
                    if (!select.Contains(" SituacaoEntregaNotaFiscal, "))
                    {
                        select.Append("NotaFiscal.NF_SITUACAO_ENTREGA SituacaoEntregaNotaFiscal, ");
                        groupBy.Append("NotaFiscal.NF_SITUACAO_ENTREGA, ");
                    }
                    break;

                case "ContatoCliente":
                    if (!select.Contains(" ContatoCliente, "))
                    {
                        select.Append(@"SUBSTRING(
						 				(
						 					SELECT ', ' + _contatoCliente.ANX_DESCRICAO
						 					  FROM T_AGENDAMENTO_ENTREGA_PEDIDO_CLIENTE_ANEXOS AS _contatoCliente
						 					 WHERE _contatoCliente.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS ContatoCliente, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "ContatoTransportador":
                    if (!select.Contains(" ContatoTransportador, "))
                    {
                        select.Append(@"SUBSTRING(
						 				(
						 					SELECT ', ' + _contatoTransportador.ANX_DESCRICAO
						 					  FROM T_AGENDAMENTO_ENTREGA_PEDIDO_TRANSPORTADOR_ANEXOS AS _contatoTransportador
						 					 WHERE _contatoTransportador.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS ContatoTransportador, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Ocorrencia":
                    if (!select.Contains(" Ocorrencia, "))
                    {
                        select.Append(@"SUBSTRING(
						 				(
						 					SELECT ', ' + _tipoOcorrencia.OCO_DESCRICAO
						 					  FROM T_OCORRENCIA AS _tipoOcorrencia 
						 					  JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA AS _ocorrencia
						 						ON _tipoOcorrencia.OCO_CODIGO = _ocorrencia.OCO_CODIGO
						 					 WHERE _ocorrencia.PED_CODIGO = Pedido.PED_CODIGO
						 					   FOR XML PATH('')
						 			     ), 3, 1000
						 			  ) AS Ocorrencia, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO"))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "SituacaoEntregaDescricao":
                    if (!select.Contains(" SituacaoEntregaAgendamento, "))
                    {
                        select.Append(@"(  CASE WHEN Pedido.PED_DATA_AGENDAMENTO IS NULL AND CargaEntrega.CEN_DATA_INICIO_ENTREGA IS NULL AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME IS NULL
						 			                THEN 0 
						 			                ELSE
						 		            CASE 
                                                WHEN Pedido.PED_DATA_AGENDAMENTO >= CargaEntrega.CEN_DATA_INICIO_ENTREGA 
						 			            THEN 1
                                                WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA > Pedido.PED_DATA_AGENDAMENTO 
						 			            THEN 2
                                                WHEN Pedido.PED_DATA_AGENDAMENTO IS NULL
                                                THEN 1
                                                WHEN Pedido.PED_DATA_AGENDAMENTO >= CargaEntrega.CEN_DATA_INICIO_ENTREGA AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME = 1
                                                THEN 1
                                                WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA > Pedido.PED_DATA_AGENDAMENTO AND MotivoReagendamento.MRE_CONSIDERAR_ON_TIME = 1
						 		                THEN 2
                                        END END
						 	            ) AS SituacaoEntregaAgendamento, ");

                        if (!groupBy.Contains("Pedido.PED_DATA_AGENDAMENTO"))
                            groupBy.Append("Pedido.PED_DATA_AGENDAMENTO, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_INICIO_ENTREGA"))
                            groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, ");

                        if (!groupBy.Contains(" MotivoReagendamento.MRE_CONSIDERAR_ON_TIME"))
                            groupBy.Append(" MotivoReagendamento.MRE_CONSIDERAR_ON_TIME, ");

                        SetarJoinsPedido(joins);
                        SetarJoinsCargaEntrega(joins);
                        SetarJoinsMotivoReagendamento(joins);
                    }
                    break;

                case "ResponsavelAgenda":
                    if (!select.Contains(" ResponsavelAgenda, "))
                    {
                        select.Append("Responsavel.FUN_NOME ResponsavelAgenda, ");
                        groupBy.Append("Responsavel.FUN_NOME, ");

                        SetarJoinsResponsavel(joins);
                    }
                    break;

                case "Holding":
                    if (!select.Contains(" Holding, "))
                    {
                        select.Append("GrupoPessoaDestinatario.GRP_DESCRICAO Holding, ");
                        groupBy.Append("GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaDestinatario(joins);
                    }
                    break;

                case "Categoria":
                    if (!select.Contains(" Categoria, "))
                    {
                        select.Append("CategoriaDestinatario.CTP_DESCRICAO Categoria, ");
                        groupBy.Append("CategoriaDestinatario.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaDestinatario(joins);
                    }
                    break;

                case "MotivoReagendamento":
                    if (!select.Contains(" MotivoReagendamento, "))
                    {
                        select.Append("MotivoReagendamento.MRE_DESCRICAO MotivoReagendamento, ");
                        groupBy.Append("MotivoReagendamento.MRE_DESCRICAO, ");

                        SetarJoinsMotivoReagendamento(joins);
                    }
                    break;

                case "ISISReturn":
                    if (!select.Contains(" ISISReturn, "))
                    {
                        select.Append("PedidoAdicional.PAD_ISIS_RETURN ISISReturn, ");
                        groupBy.Append("PedidoAdicional.PAD_ISIS_RETURN, ");

                        SetarJoinsPedidoAdicional(joins);
                    }
                    break;

                case "ResponsavelReagendamento":
                    if (!select.Contains(" ResponsavelReagendamento, "))
                    {
                        select.Append("TipoResponsavel.TRA_DESCRICAO ResponsavelReagendamento, ");
                        groupBy.Append("TipoResponsavel.TRA_DESCRICAO, ");

                        SetarJoinsTipoResponsavel(joins);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloVeicular(joins);
                    }
                    break;

                case "ObservacaoMotivoReagendamento":
                    if (!select.Contains(" ObservacaoMotivoReagendamento, "))
                    {
                        select.Append("Pedido.PED_MOTIVO_REAGENDAMENTO ObservacaoMotivoReagendamento, ");
                        groupBy.Append("Pedido.PED_MOTIVO_REAGENDAMENTO, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "Trecho":
                    if (!select.Contains(" Trecho, "))
                    {
                        select.Append("Trecho.TTR_DESCRICAO Trecho, ");
                        groupBy.Append("Trecho.TTR_DESCRICAO, ");

                        SetarJoinsTipoTrecho(joins);
                    }
                    break;

                case "CNPJFilialEmissoraFormatado":
                    if (!select.Contains(" CNPJFilialEmissora, "))
                    {
                        select.Append("FilialEmissora.EMP_CNPJ CNPJFilialEmissora, ");
                        groupBy.Append("FilialEmissora.EMP_CNPJ, ");

                        SetarJoinsFilialEmissora(joins);
                    }
                    break;

                case "CNPJFilialFormatado":
                    if (!select.Contains(" CNPJFilial, "))
                    {
                        select.Append("Filial.FIL_CNPJ CNPJFilial, ");
                        groupBy.Append("Filial.FIL_CNPJ, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NomeFilial":
                    if (!select.Contains("NomeFilial, "))
                    {
                        select.Append("Filial.FIL_DESCRICAO NomeFilial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins);
                    }
                    break;

                case "NomeFantasia":
                    if (!select.Contains(" NomeFantasia, "))
                    {
                        select.Append("Expedidor.CLI_NOMEFANTASIA NomeFantasia, ");
                        groupBy.Append("Expedidor.CLI_NOMEFANTASIA, ");

                        SetarJoinsExpedidor(joins);
                    }
                    break;

                case "CategoriaExpedidor":
                    if (!select.Contains(" CategoriaExpedidor, "))
                    {
                        select.Append("CategoriaExpedidor.CTP_DESCRICAO CategoriaExpedidor, ");
                        groupBy.Append("CategoriaExpedidor.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaExpedidor(joins);
                    }
                    break;

                case "NumeroPedidoEmbarcador":
                    if (!select.Contains(" NumeroPedidoEmbarcador, "))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedidoEmbarcador, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");

                        SetarJoinsPedido(joins);
                    }
                    break;

                case "CidadeOrigemPedido":
                    if (!select.Contains(" CidadeOrigemPedido, "))
                    {
                        select.Append("CidadeOrigemPedido.LOC_DESCRICAO CidadeOrigemPedido, ");
                        groupBy.Append("CidadeOrigemPedido.LOC_DESCRICAO, ");

                        SetarJoinsCidadeOrigemPedido(joins);
                    }
                    break;

                case "CodigoCliente":
                    if (!select.Contains(" CodigoCliente, "))
                    {
                        select.Append("Destinatario.CLI_CODIGO_INTEGRACAO CodigoCliente, ");
                        groupBy.Append("Destinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NomeCliente":
                    if (!select.Contains(" NomeCliente, "))
                    {
                        select.Append("Destinatario.CLI_NOME  NomeCliente, ");
                        groupBy.Append("Destinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CidadeDestinatario":
                    if (!select.Contains(" CidadeDestinatario, "))
                    {
                        select.Append("Destinatario.CLI_CIDADE  CidadeDestinatario, ");
                        groupBy.Append("Destinatario.CLI_CIDADE, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ClienteEndereco":
                    if (!select.Contains(" ClienteEndereco, "))
                    {
                        select.Append("Destinatario.CLI_ENDERECO ClienteEndereco, ");
                        groupBy.Append("Destinatario.CLI_ENDERECO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "PlacaVeiculo":
                    if (!select.Contains(" PlacaVeiculo, "))
                    {
                        select.Append("Veiculo.VEI_PLACA PlacaVeiculo, ");
                        groupBy.Append("Veiculo.VEI_PLACA, ");

                        SetarJoinsVeiculos(joins);
                    }
                    break;

                case "TransportadorCodigoIntegracao":
                    if (!select.Contains(" TransportadorCodigoIntegracao, "))
                    {
                        select.Append("Transportador.EMP_CODIGO_INTEGRACAO TransportadorCodigoIntegracao, ");
                        groupBy.Append("Transportador.EMP_CODIGO_INTEGRACAO, ");

                        SetarJoinsTransportador(joins);
                    }
                    break;

                case "DataEntregaPrevistaFormatada":
                    if (!select.Contains(" DataEntregaPrevistaFormatada, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA DataEntregaPrevista, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_PREVISTA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "OrdemPrevista":
                    if (!select.Contains(" OrdemPrevista, "))
                    {
                        select.Append("CargaEntrega.CEN_ORDEM OrdemPrevista, ");
                        groupBy.Append("CargaEntrega.CEN_ORDEM, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "AnalistaResponsavelMonitoramento":
                    if (!select.Contains(" AnalistaResponsavelMonitoramento, "))
                    {
                        select.Append("AnalistaResponsavelMonitoramento.FUN_NOME AnalistaResponsavelMonitoramento, ");
                        groupBy.Append("AnalistaResponsavelMonitoramento.FUN_NOME, ");

                        SetarJoinsFuncionario(joins);
                    }
                    break;

                case "SituacaoEntregaPrevisaoDescricao":
                    if (!select.Contains(" SituacaoEntregaPrevisao, "))
                    {
                        select.Append(@" (  CASE WHEN CargaEntrega.CEN_DATA_ENTREGA_PREVISTA IS NULL 
									  OR CargaEntrega.CEN_DATA_INICIO_ENTREGA IS NULL
						 			                THEN 0 
						 			                ELSE
						 		            CASE 
                                                WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA > CargaEntrega.CEN_DATA_ENTREGA_PREVISTA
						 			            THEN 2
                                                WHEN CargaEntrega.CEN_DATA_INICIO_ENTREGA < CargaEntrega.CEN_DATA_ENTREGA_PREVISTA
						 			            THEN 1
                                        END END
						 	            ) AS SituacaoEntregaPrevisao, ");

                        if (!groupBy.Contains("CargaEntrega.CEN_DATA_INICIO_ENTREGA"))
                            groupBy.Append("CargaEntrega.CEN_DATA_INICIO_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins);
                    }
                    break;

                case "PesoCarga":
                    if (!select.Contains(" PesoCarga, "))
                    {
                        select.Append("(select sum(CargaPedido.PED_PESO)) PesoCarga, ");


                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "SequenciaEntrega":
                    if (!select.Contains(" SequenciaEntrega, "))
                    {
                        select.Append("CargaPedido.PED_ORDEM_ENTREGA SequenciaEntrega, ");
                        groupBy.Append("CargaPedido.PED_ORDEM_ENTREGA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "RotaFreteDescricao":
                    if (!select.Contains(" RotaFreteDescricao, "))
                    {
                        select.Append("RotaFrete.ROF_DESCRICAO RotaFreteDescricao, ");
                        groupBy.Append("RotaFrete.ROF_DESCRICAO, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;

                case "RotaFreteCodigoIntegracao":
                    if (!select.Contains(" RotaFreteCodigoIntegracao, "))
                    {
                        select.Append("RotaFrete.ROF_CODIGO_INTEGRACAO RotaFreteCodigoIntegracao, ");
                        groupBy.Append("RotaFrete.ROF_CODIGO_INTEGRACAO, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;
            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string pattern = "yyyy-MM-dd";

            SetarJoinsCarga(joins);
            SetarJoinsPedido(joins);

            where.Append(" and NotaFiscal.NF_ATIVA = 1 and Carga.CAR_SITUACAO NOT IN (13, 18) and Pedido.PED_SITUACAO IN (1, 3)");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                where.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.CnpjCpfCliente > 0)
            {
                where.Append($" and Destinatario.CLI_CGCCPF = {filtrosPesquisa.CnpjCpfCliente} ");
                SetarJoinsDestinatario(joins);
            }

            if (filtrosPesquisa.NumeroNota > 0)
                where.Append($" and NotaFiscal.NF_NUMERO = {filtrosPesquisa.NumeroNota} ");

            if (filtrosPesquisa.SituacaoAgendamento.HasValue)
                where.Append($" and Pedido.PED_SITUACAO_AGENDAMENTO_ENTREGA_PEDIDO = {(int)filtrosPesquisa.SituacaoAgendamento.Value} ");

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
                where.Append($" and Carga.TOP_CODIGO = {filtrosPesquisa.CodigoTipoOperacao} ");

            if (filtrosPesquisa.CodigoTransportador > 0)
                where.Append($" and Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador} ");

            if (filtrosPesquisa.DataAgendamentoInicial.HasValue)
                where.Append($" and CAST(Pedido.PED_DATA_AGENDAMENTO AS DATE) >= '{filtrosPesquisa.DataAgendamentoInicial.Value.ToString(pattern)}' ");

            if (filtrosPesquisa.DataAgendamentoFinal.HasValue)
                where.Append($" and CAST(Pedido.PED_DATA_AGENDAMENTO AS DATE) <= '{filtrosPesquisa.DataAgendamentoFinal.Value.ToString(pattern)}' ");

            if (filtrosPesquisa.DataCarregamentoInicial.HasValue)
            {
                where.Append($" and CAST(ISNULL(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, Carga.CAR_DATA_CARREGAMENTO) AS DATE) >= '{filtrosPesquisa.DataCarregamentoInicial.Value.ToString(pattern)}' ");
                SetarJoinsCargaJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.DataCarregamentoFinal.HasValue)
            {
                where.Append($" and CAST(ISNULL(CargaJanelaCarregamento.CJC_INICIO_CARREGAMENTO, Carga.CAR_DATA_CARREGAMENTO) AS DATE) <= '{filtrosPesquisa.DataCarregamentoFinal.Value.ToString(pattern)}' ");
                SetarJoinsCargaJanelaCarregamento(joins);
            }

            if (filtrosPesquisa.DataPrevisaoEntregaInicial.HasValue)
            {
                where.Append($" and CAST(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA AS DATE) >= '{filtrosPesquisa.DataPrevisaoEntregaInicial.Value.ToString(pattern)}' ");
                SetarJoinsCargaEntrega(joins);
            }

            if (filtrosPesquisa.DataPrevisaoEntregaFinal.HasValue)
            {
                where.Append($" and CAST(CargaEntrega.CEN_DATA_ENTREGA_PREVISTA AS DATE) <= '{filtrosPesquisa.DataPrevisaoEntregaFinal.Value.ToString(pattern)}' ");
                SetarJoinsCargaEntrega(joins);
            }

            if (filtrosPesquisa.Filial > 0)
                where.Append($" and Carga.FIL_CODIGO = {filtrosPesquisa.Filial} ");

            if (filtrosPesquisa.TipoTrecho > 0)
                where.Append($" and Carga.TTR_CODIGO = {filtrosPesquisa.TipoTrecho} ");

            if (filtrosPesquisa.Expedidor > 0)
                where.Append($" and CargaPedido.CLI_CODIGO_EXPEDIDOR = {filtrosPesquisa.Expedidor} ");

            if (filtrosPesquisa.Recebedor > 0)
                where.Append($" and CargaPedido.CLI_CODIGO_RECEBEDOR = {filtrosPesquisa.Expedidor} ");
        }

        #endregion
    }
}
