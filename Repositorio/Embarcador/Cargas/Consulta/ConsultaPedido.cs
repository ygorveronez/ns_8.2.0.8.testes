using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio.Embarcador.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repositorio.Embarcador.Cargas
{
    sealed class ConsultaPedido : Consulta.Consulta<Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido>
    {
        #region Construtores

        public ConsultaPedido() : base(tabela: "T_PEDIDO as Pedido") { }

        #endregion

        #region Métodos Privados

        private void SetarJoinsCanalEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" CanalEntrega "))
                joins.Append(" left join T_CANAL_ENTREGA CanalEntrega on CanalEntrega.CNE_CODIGO = Pedido.CNE_CODIGO ");
        }

        private void SetarJoinsCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCargaPedido(joins);

            if (joins.Contains(" Carga "))
                return;

            if (filtrosPesquisa.UtilizarDadosDasCargasAgrupadas)
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
            else
                joins.Append(" left join T_CARGA Carga on Carga.CAR_CODIGO = CargaPedido.CAR_CODIGO_ORIGEM ");
        }

        private void SetarJoinsCargaCte(StringBuilder joins)
        {
            SetarJoinsCargaPedido(joins);

            if (!joins.Contains(" CargaCTe "))
                joins.Append(" left join T_CARGA_CTE CargaCTe on CargaCTe.CAR_CODIGO = CargaPedido.CAR_CODIGO_ORIGEM ");
        }

        private void SetarJoinsMotivoCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoCancelamento "))
                joins.Append(" left join T_MOTIVO_CANCELAMENTO_PEDIDO MotivoCancelamento on MotivoCancelamento.MCP_CODIGO = Pedido.MCP_CODIGO ");
        }

        private void SetarJoinsUsuarioCancelamento(StringBuilder joins)
        {
            if (!joins.Contains(" UsuarioCancelamento "))
                joins.Append(" left join T_FUNCIONARIO UsuarioCancelamento on UsuarioCancelamento.FUN_CODIGO = Pedido.FUN_CANCELAMENTO_CODIGO ");
        }

        private void SetarJoinsCanalVenda(StringBuilder joins)
        {
            if (!joins.Contains(" canalvenda "))
                joins.Append(" left join T_CANAL_VENDA canalvenda on canalvenda.CNV_CODIGO = Pedido.CNV_CODIGO ");
        }

        private void SetarJoinsCargaEntrega(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCargaPedido(joins);

            if (joins.Contains(" CargaEntrega "))
                return;

            if (filtrosPesquisa.UtilizarDadosDasCargasAgrupadas)
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
            else
                joins.Append(" left join T_CARGA_ENTREGA CargaEntrega on CargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO_ORIGEM ");
        }

        private void SetarJoinsCargaGrupoPessoa(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" CGrupoPessoa "))
                joins.Append(" left join T_GRUPO_PESSOAS CGrupoPessoa on CGrupoPessoa.GRP_CODIGO = Carga.GRP_CODIGO ");
        }

        private void SetarJoinsCargaJanelaCarregamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" JanelaCarregamento "))
                joins.Append(" left join T_CARGA_JANELA_CARREGAMENTO JanelaCarregamento on JanelaCarregamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPedido(StringBuilder joins)
        {
            if (!joins.Contains(" CargaPedido "))
                joins.Append(" left join T_CARGA_PEDIDO CargaPedido on CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsCargaPreAgrupamento(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" CargaPreAgrupamento "))
                joins.Append(" left join T_CARGA_PRE_AGRUPAMENTO CargaPreAgrupamento on CargaPreAgrupamento.CAR_CODIGO = Carga.CAR_CODIGO ");
        }

        private void SetarJoinsCargaPreAgrupamentoAgrupador(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquis)
        {
            SetarJoinsCargaPreAgrupamento(joins, filtrosPesquis);

            if (!joins.Contains(" CargaPreAgrupamentoAgrupador "))
                joins.Append(" left join T_CARGA_PRE_AGRUPAMENTO_AGRUPADOR CargaPreAgrupamentoAgrupador on CargaPreAgrupamentoAgrupador.PAA_CODIGO = CargaPreAgrupamento.PAA_CODIGO ");
        }

        private void SetarJoinsCategoriaDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" CategoriaDestinatario "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaDestinatario on CategoriaDestinatario.CTP_CODIGO = PedidoDestinatario.CTP_CODIGO ");
        }

        private void SetarJoinsCategoriaRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" CategoriaRemetente "))
                joins.Append(" left join T_CATEGORIA_PESSOA CategoriaRemetente on CategoriaRemetente.CTP_CODIGO = PedidoRemetente.CTP_CODIGO ");
        }

        private void SetarJoinsDadosTransporteMaritimo(StringBuilder joins)
        {
            if (!joins.Contains(" DadosTransporteMaritimo "))
                joins.Append(" left join T_PEDIDO_DADOS_TRANSPORTE_MARITIMO DadosTransporteMaritimo on DadosTransporteMaritimo.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsDestinatario(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoDestinatario "))
                joins.Append(" left join T_CLIENTE PedidoDestinatario on PedidoDestinatario.CLI_CGCCPF = Pedido.CLI_CODIGO ");
        }

        private void SetarJoinsDestino(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CPDestino "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_LOCALIDADES CPDestino on CPDestino.LOC_CODIGO = Pedido.LOC_CODIGO_DESTINO ");
            else
            {
                SetarJoinsCargaPedido(joins);

                joins.Append(" left join T_LOCALIDADES CPDestino on CPDestino.LOC_CODIGO = CargaPedido.LOC_CODIGO_DESTINO ");
            }
        }

        private void SetarJoinsExpedidor(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CPExpedidor "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_CLIENTE CPExpedidor on CPExpedidor.CLI_CGCCPF = Pedido.CLI_CODIGO_EXPEDIDOR ");
            else
            {
                SetarJoinsCargaPedido(joins);

                joins.Append(" left join T_CLIENTE CPExpedidor on CPExpedidor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_EXPEDIDOR ");
            }
        }

        private void SetarJoinsFilial(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" Filial "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Pedido.FIL_CODIGO ");
            else
            {
                SetarJoinsCarga(joins, filtrosPesquisa);

                joins.Append(" left join T_FILIAL Filial on Filial.FIL_CODIGO = Carga.FIL_CODIGO ");
            }
        }

        private void SetarJoinsFuncionarioGerente(StringBuilder joins)
        {
            if (!joins.Contains(" FuncionarioGerente "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioGerente on FuncionarioGerente.FUN_CODIGO = Pedido.FUN_CODIGO_GERENTE ");
        }

        private void SetarJoinsFuncionarioGerenteRegional(StringBuilder joins)
        {
            SetarJoinsFuncionarioGerente(joins);

            if (!joins.Contains(" FuncionarioGerenteRegional "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioGerenteRegional on FuncionarioGerenteRegional.FUN_CODIGO = FuncionarioGerente.FUN_GERENTE ");
        }

        private void SetarJoinsFuncionarioGerenteRegionalSupervisor(StringBuilder joins)
        {
            SetarJoinsFuncionarioGerenteSupervisor(joins);

            if (!joins.Contains(" FuncionarioGerenteRegionalSupervisor "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioGerenteRegionalSupervisor on FuncionarioGerenteRegionalSupervisor.FUN_CODIGO = FuncionarioGerenteSupervisor.FUN_GERENTE ");

        }

        private void SetarJoinsFuncionarioGerenteSupervisor(StringBuilder joins)
        {
            SetarJoinsFuncionarioSupervisor(joins);

            if (!joins.Contains(" FuncionarioGerenteSupervisor "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioGerenteSupervisor on FuncionarioGerenteSupervisor.FUN_CODIGO = FuncionarioSupervisor.FUN_GERENTE ");
        }

        private void SetarJoinsFuncionarioSupervisor(StringBuilder joins)
        {
            if (!joins.Contains(" FuncionarioSupervisor "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioSupervisor on FuncionarioSupervisor.FUN_CODIGO = Pedido.FUN_CODIGO_SUPERVISOR ");
        }

        private void SetarJoinsFuncionarioVendedor(StringBuilder joins)
        {
            if (!joins.Contains(" FuncionarioVendedor "))
                joins.Append(" left join T_FUNCIONARIO FuncionarioVendedor on FuncionarioVendedor.FUN_CODIGO = Pedido.FUN_CODIGO_VENDEDOR ");
        }

        private void SetarJoinsGrupoPessoaDestinatario(StringBuilder joins)
        {
            SetarJoinsDestinatario(joins);

            if (!joins.Contains(" GrupoPessoaDestinatario "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaDestinatario on GrupoPessoaDestinatario.GRP_CODIGO = PedidoDestinatario.GRP_CODIGO ");
        }

        private void SetarJoinsGrupoPessoaRemetente(StringBuilder joins)
        {
            SetarJoinsRemetente(joins);

            if (!joins.Contains(" GrupoPessoaRemetente "))
                joins.Append(" left join T_GRUPO_PESSOAS GrupoPessoaRemetente on GrupoPessoaRemetente.GRP_CODIGO = PedidoRemetente.GRP_CODIGO ");
        }

        private void SetarJoinsModeloveicularCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" ModeloVeicular "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Pedido.MVC_CODIGO ");
            else
            {
                SetarJoinsCarga(joins, filtrosPesquisa);

                joins.Append(" left join T_MODELO_VEICULAR_CARGA ModeloVeicular on ModeloVeicular.MVC_CODIGO = Carga.MVC_CODIGO ");
            }
        }

        private void SetarJoinsMontagemCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" MontagemCarga "))
                joins.Append(" left join T_CARREGAMENTO MontagemCarga on MontagemCarga.CRG_CODIGO = Carga.CRG_CODIGO ");
        }

        private void SetarJoinsOcorrenciaCTeIntegracao(StringBuilder joins)
        {
            SetarJoinsCargaCte(joins);

            if (!joins.Contains(" OcorrenciaCTeIntegracao "))
                joins.Append("LEFT JOIN T_OCORRENCIA_CTE_INTEGRACAO OcorrenciaCTeIntegracao ON OcorrenciaCTeIntegracao.CCT_CODIGO = CargaCTe.CCT_CODIGO ");
        }

        private void SetarJoinsOrigem(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CPOrigem "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_LOCALIDADES CPOrigem on CPOrigem.LOC_CODIGO = Pedido.LOC_CODIGO_ORIGEM ");
            else
            {
                SetarJoinsCargaPedido(joins);

                joins.Append(" left join T_LOCALIDADES CPOrigem on CPOrigem.LOC_CODIGO = CargaPedido.LOC_CODIGO_ORIGEM ");
            }
        }

        private void SetarJoinsPaisDestino(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsDestino(joins, filtrosPesquisa);

            if (!joins.Contains(" CPPaisDestino "))
                joins.Append(" left join T_PAIS CPPaisDestino on CPPaisDestino.PAI_CODIGO = CPDestino.PAI_CODIGO ");
        }

        private void SetarJoinsPaisOrigem(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsOrigem(joins, filtrosPesquisa);

            if (!joins.Contains(" CPPaisOrigem "))
                joins.Append(" left join T_PAIS CPPaisOrigem on CPPaisOrigem.PAI_CODIGO = CPOrigem.PAI_CODIGO ");
        }

        private void SetarJoinsRecebedor(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CPRecebedor "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_CLIENTE CPRecebedor on CPRecebedor.CLI_CGCCPF = Pedido.CLI_CODIGO_RECEBEDOR ");
            else
            {
                SetarJoinsCargaPedido(joins);

                joins.Append(" left join T_CLIENTE CPRecebedor on CPRecebedor.CLI_CGCCPF = CargaPedido.CLI_CODIGO_RECEBEDOR ");
            }
        }

        private void SetarJoinsRemetente(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoRemetente "))
                joins.Append(" left join T_CLIENTE PedidoRemetente on PedidoRemetente.CLI_CGCCPF = Pedido.CLI_CODIGO_REMETENTE ");
        }

        private void SetarJoinsRotaFrete(StringBuilder joins)
        {
            if (!joins.Contains(" RotaFrete "))
                joins.Append(" left join T_ROTA_FRETE RotaFrete on RotaFrete.ROF_CODIGO = Pedido.ROF_CODIGO ");
        }

        private void SetarJoinsPedidoAutorizacao(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoAutorizacao "))
                joins.Append(@" OUTER APPLY (SELECT TOP 1 [FUN_CODIGO] as Autorizador, PedidoAutorizacao.PEA_MOTIVO as MotivoAutorizacaoPedido
                                             FROM T_PEDIDO_AUTORIZACAO PedidoAutorizacao
                                             WHERE Pedido.PED_CODIGO = PedidoAutorizacao.PED_CODIGO
	                                         AND PedidoAutorizacao.PEA_SITUACAO = 1
                                            ) AS PedidoAutorizacao ");
        }

        private void SetarJoinsPedidoMotivo(StringBuilder joins)
        {
            if (!joins.Contains(" MotivoPedido "))
                joins.Append(@" LEFT JOIN T_PEDIDO_MOTIVO MotivoPedido ON Pedido.PM_CODIGO = MotivoPedido.PM_CODIGO");
        }

        private void SetarJoinsPedidoOcorrenciaEntrega(StringBuilder joins)
        {
            if (!joins.Contains(" OcorrenciaColetaEntrega "))
                joins.Append(@" LEFT JOIN T_PEDIDO_OCORRENCIA_COLETA_ENTREGA OcorrenciaColetaEntrega ON Pedido.PED_CODIGO = OcorrenciaColetaEntrega.PED_CODIGO");
        }

        private void SetarJoinsTipoCarga(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CTipoCarga "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_TIPO_DE_CARGA CTipoCarga on CTipoCarga.TCG_CODIGO = Pedido.TCG_CODIGO ");
            else
            {
                SetarJoinsCarga(joins, filtrosPesquisa);

                joins.Append(" left join T_TIPO_DE_CARGA CTipoCarga on CTipoCarga.TCG_CODIGO = Carga.TCG_CODIGO ");
            }
        }

        private void SetarJoinsTipoOperacao(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CTipoOperacao "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_TIPO_OPERACAO CTipoOperacao on CTipoOperacao.TOP_CODIGO = Pedido.TOP_CODIGO ");
            else
            {
                SetarJoinsCarga(joins, filtrosPesquisa);

                joins.Append(" left join T_TIPO_OPERACAO CTipoOperacao on CTipoOperacao.TOP_CODIGO = Carga.TOP_CODIGO ");
            }
        }

        private void SetarJoinsTipoSeparacao(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquis)
        {
            SetarJoinsMontagemCarga(joins, filtrosPesquis);

            if (!joins.Contains(" TipoSeparacao "))
                joins.Append(" left join T_TIPO_SEPARACAO TipoSeparacao on TipoSeparacao.TSE_CODIGO = MontagemCarga.TSE_CODIGO ");
        }

        private void SetarJoinsTransportador(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" CEmpresa "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_EMPRESA CEmpresa on CEmpresa.EMP_CODIGO = Pedido.EMP_CODIGO ");
            else
            {
                SetarJoinsCarga(joins, filtrosPesquisa);

                joins.Append(" left join T_EMPRESA CEmpresa on CEmpresa.EMP_CODIGO = Carga.EMP_CODIGO ");
            }
        }

        private void SetarJoinsViaTransporte(StringBuilder joins)
        {
            if (!joins.Contains(" ViaTransporte "))
                joins.Append(" left join T_VIA_TRANSPORTE ViaTransporte on ViaTransporte.TVT_CODIGO = Pedido.TVT_CODIGO ");
        }

        private void SetarJoinsPedidoViagemNavio(StringBuilder joins)
        {
            if (!joins.Contains(" PedidoViagemNavio "))
                joins.Append(" left join T_PEDIDO_VIAGEM_NAVIO PedidoViagemNavio on PedidoViagemNavio.PVN_CODIGO = Pedido.PVN_CODIGO ");
        }

        private void SetarJoinsPortoDestino(StringBuilder joins)
        {
            if (!joins.Contains(" PortoDestino "))
                joins.Append(" left join T_PORTO PortoDestino on PortoDestino.POT_CODIGO = Pedido.POT_CODIGO_DESTINO ");
        }

        private void SetarJoinsPortoOrigem(StringBuilder joins)
        {
            if (!joins.Contains(" PortoOrigem "))
                joins.Append(" left join T_PORTO PortoOrigem on PortoOrigem.POT_CODIGO = Pedido.POT_CODIGO_ORIGEM ");
        }

        private void SetarJoinsCte(StringBuilder joins)
        {
            SetarJoinsCargaCte(joins);

            if (!joins.Contains(" CTe "))
                joins.Append(" left join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO ");
        }
        private void SetarJoinsPedidoAdicional(StringBuilder joins)
        {

            if (!joins.Contains(" PedidoAdicional "))
                joins.Append(" left join T_PEDIDO_ADICIONAL PedidoAdicional on PedidoAdicional.PED_CODIGO = Pedido.PED_CODIGO ");
        }

        private void SetarJoinsTomador(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (joins.Contains(" Tomador "))
                return;

            if (filtrosPesquisa.UtilizarDadosDosPedidos)
                joins.Append(" left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = Pedido.CLI_CODIGO_TOMADOR ");
            else
            {
                SetarJoinsCargaPedido(joins);

                joins.Append(" left join T_CLIENTE Tomador on Tomador.CLI_CGCCPF = CargaPedido.CLI_CODIGO_TOMADOR ");
            }
        }

        private void SetarJoinsRedespacho(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" Redespacho "))
                joins.Append(" left join T_REDESPACHO Redespacho on Redespacho.RED_CODIGO = Carga.RED_CODIGO ");
        }

        private void SetarJoinsFuncionarioAutor(StringBuilder joins)
        {
            if (!joins.Contains(" FuncionarioAutor "))
                joins.Append(" left outer join T_FUNCIONARIO FuncionarioAutor on FuncionarioAutor.FUN_CODIGO = Pedido.FUN_CODIGO_AUTOR ");
        }
        private void SetarJoinsCentroResultado(StringBuilder joins)
        {
            if (!joins.Contains(" CentroResultado "))
                joins.Append("left outer join T_CENTRO_RESULTADO CentroResultado on CentroResultado.CRE_CODIGO = Pedido.CRE_CODIGO ");
        }
        private void SetarJoinsEmailSolicitante(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" EmailSolicitante "))
                joins.Append(" left join T_AGENDAMENTO_COLETA EmailSolicitante on EmailSolicitante.CAR_CODIGO = Carga.CAR_CODIGO");
        }

        private void SetarJoinCentroDeCustoViagem(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            SetarJoinsCarga(joins, filtrosPesquisa);

            if (!joins.Contains(" CentroDeCustoViagemCodigo "))
                joins.Append(" LEFT JOIN T_CENTRO_CUSTO_VIAGEM CentroDeCustoViagem ON CentroDeCustoViagem.CCV_CODIGO = Pedido.CCV_CODIGO");
        }

        private void SetarJoinSituacoesEntrega(StringBuilder joins, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            if (!joins.ToString().Contains("AS SituacoesEntrega"))
            {
                joins.Append(@"
                                LEFT JOIN (
                                    SELECT 
                                        CargaEntrega.CAR_CODIGO,
                                        CargaEntregaPedido.CPE_CODIGO,
                                        CargaEntrega.CEN_SITUACAO   
                                    FROM 
                                        T_CARGA_ENTREGA CargaEntrega
                                        JOIN T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido 
                                            ON CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO
                                    WHERE 
                                        CargaEntrega.CEN_COLETA = 0
                                ) AS SituacoesEntrega
                                    ON SituacoesEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO ");
            }
        }

        #endregion Métodos Privados

        #region Métodos Protegidos Sobrescritos

        protected override SQLDinamico ObterSql(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            if (filtrosPesquisa.ExibirProdutos && !propriedades.Any(o => o.Propriedade == "Codigo"))
                propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "Codigo" });

            return base.ObterSql(filtrosPesquisa, parametrosConsulta, propriedades, somenteContarNumeroRegistros);
        }

        protected override void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo,"))
                    {
                        select.Append("Pedido.PED_CODIGO Codigo, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "Companhia":
                    if (!select.Contains(" Companhia,"))
                    {
                        select.Append("Pedido.PED_COMPANHIA Companhia, ");
                        groupBy.Append("Pedido.PED_COMPANHIA, ");
                    }
                    break;

                case "DataEmbarque":
                case "DataEmbarqueFormatada":
                case "DiaSemana":
                    if (!select.Contains("DataEmbarque,"))
                    {
                        select.Append("(case when (isnull(JanelaCarregamento.CJC_EXCEDENTE, 1) = 0 and JanelaCarregamento.CEC_CODIGO is not null) then JanelaCarregamento.CJC_INICIO_CARREGAMENTO else null end) DataEmbarque, ");
                        groupBy.Append("JanelaCarregamento.CJC_EXCEDENTE, JanelaCarregamento.CJC_INICIO_CARREGAMENTO, JanelaCarregamento.CEC_CODIGO, ");

                        SetarJoinsCargaJanelaCarregamento(joins, filtrosPesquisa);
                    }
                    break;

                case "DataAgendamento":
                    if (!select.Contains(" DataAgendamento,"))
                    {
                        select.Append("(case when Pedido.PED_DATA_AGENDAMENTO is null then '' else convert(varchar(10), Pedido.PED_DATA_AGENDAMENTO, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_AGENDAMENTO, 108) end) DataAgendamento, ");
                        groupBy.Append("Pedido.PED_DATA_AGENDAMENTO, ");
                    }
                    break;

                case "PrevisaoEntregaTransportador":
                    if (!select.Contains(" PrevisaoEntregaTransportador,"))
                    {
                        select.Append("(case when Pedido.PED_PREVISAO_ENTREGA_TRANSPORTADOR is null then '' else convert(varchar(10), Pedido.PED_PREVISAO_ENTREGA_TRANSPORTADOR, 103) + ' ' + convert(varchar(5), Pedido.PED_PREVISAO_ENTREGA_TRANSPORTADOR, 108) end) PrevisaoEntregaTransportador, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA_TRANSPORTADOR, ");
                    }
                    break;

                case "ValorCustoFrete":
                    if (!select.Contains(" ValorCustoFrete,"))
                    {
                        select.Append("Pedido.PED_VALOR_CUSTO_FRETE ValorCustoFrete,");
                        groupBy.Append("Pedido.PED_VALOR_CUSTO_FRETE, ");
                    }
                    break;
                case "CustoFrete":
                    if (!select.Contains(" CustoFrete,"))
                    {
                        select.Append("Pedido.PED_CUSTO_FRETE CustoFrete,");
                        groupBy.Append("Pedido.PED_CUSTO_FRETE, ");
                    }
                    break;
                case "DataTerminoCarregamento":
                    if (!select.Contains(" DataTerminoCarregamento,"))
                    {
                        select.Append("(case when Pedido.PED_DATA_TERMINO_CARREGAMENTO is null then '' else convert(varchar(10), Pedido.PED_DATA_TERMINO_CARREGAMENTO, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_TERMINO_CARREGAMENTO, 108) end) DataTerminoCarregamento, ");
                        groupBy.Append("Pedido.PED_DATA_TERMINO_CARREGAMENTO, ");
                    }
                    break;

                case "DataETA":
                    if (!select.Contains(" DataETA,"))
                    {
                        select.Append("(case when Pedido.PED_DATA_ETA is null then '' else convert(varchar(10), Pedido.PED_DATA_ETA, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_ETA, 108) end) DataETA, ");
                        groupBy.Append("Pedido.PED_DATA_ETA, ");
                    }
                    break;

                case "DataInclusaoBooking":
                    if (!select.Contains(" DataInclusaoBooking,"))
                    {
                        select.Append("(case when Pedido.PED_DATA_INCLUSAO_BOOKING is null then '' else convert(varchar(10), Pedido.PED_DATA_INCLUSAO_BOOKING, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_INCLUSAO_BOOKING, 108) end) DataInclusaoBooking, ");
                        groupBy.Append("Pedido.PED_DATA_INCLUSAO_BOOKING, ");
                    }
                    break;

                case "DataInclusaoPCP":
                    if (!select.Contains(" DataInclusaoPCP,"))
                    {
                        select.Append("(case when Pedido.PED_DATA_INCLUSAO_PCP is null then '' else convert(varchar(10), Pedido.PED_DATA_INCLUSAO_PCP, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_INCLUSAO_PCP, 108) end) DataInclusaoPCP, ");
                        groupBy.Append("Pedido.PED_DATA_INCLUSAO_PCP, ");
                    }
                    break;

                case "DataRetiradaCtrn":
                    if (!select.Contains(" DataRetiradaCtrn,"))
                    {
                        select.Append(@"
                            case
                                when Carga.CAR_DATA_RETIRADA_CTRN is null then
                                    substring((
                                        select distinct ', ' + convert(varchar(10), _cargaVeiculoContainer.CVC_DATA_RETIRADA_CTRN, 103)
                                          from T_CARGA_VEICULO_CONTAINER _cargaVeiculoContainer
                                         where _cargaVeiculoContainer.CAR_CODIGO = Carga.CAR_CODIGO
                                           and _cargaVeiculoContainer.CVC_DATA_RETIRADA_CTRN is not null
                                           for xml path('')
                                    ), 3, 1000)
                                else convert(varchar(10), Carga.CAR_DATA_RETIRADA_CTRN, 103)
                            end DataRetiradaCtrn, ");

                        groupBy.Append("Carga.CAR_DATA_RETIRADA_CTRN, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DeliveryTerm":
                    if (!select.Contains(" DeliveryTerm,"))
                    {
                        select.Append("Pedido.PED_DELIVERY_TERM DeliveryTerm, ");
                        groupBy.Append("Pedido.PED_DELIVERY_TERM, ");
                    }
                    break;

                case "IdAutorizacao":
                    if (!select.Contains(" IdAutorizacao,"))
                    {
                        select.Append("Pedido.PED_ID_AUTORIZACAO IdAutorizacao, ");
                        groupBy.Append("Pedido.PED_ID_AUTORIZACAO, ");
                    }
                    break;

                case "NumeroBooking":
                    if (!select.Contains(" NumeroBooking,"))
                    {
                        select.Append("Pedido.PED_NUMERO_BOOKING NumeroBooking, ");
                        groupBy.Append("Pedido.PED_NUMERO_BOOKING, ");
                    }
                    break;

                case "NumeroNavio":
                    if (!select.Contains(" NumeroNavio,"))
                    {
                        select.Append("Pedido.PED_NUMERO_NAVIO NumeroNavio, ");
                        groupBy.Append("Pedido.PED_NUMERO_NAVIO, ");
                    }
                    break;

                case "Ordem":
                    if (!select.Contains(" Ordem,"))
                    {
                        select.Append("Pedido.PED_ORDEM Ordem, ");
                        groupBy.Append("Pedido.PED_ORDEM, ");
                    }
                    break;

                case "PortoChegada":
                    if (!select.Contains(" PortoChegada,"))
                    {
                        select.Append("Pedido.PED_PORTO_CHEGADA PortoChegada, ");
                        groupBy.Append("Pedido.PED_PORTO_CHEGADA, ");
                    }
                    break;

                case "PortoSaida":
                    if (!select.Contains(" PortoSaida,"))
                    {
                        select.Append("Pedido.PED_PORTO_SAIDA PortoSaida, ");
                        groupBy.Append("Pedido.PED_PORTO_SAIDA, ");
                    }
                    break;

                case "Reserva":
                    if (!select.Contains(" Reserva,"))
                    {
                        select.Append("Pedido.PED_RESERVA Reserva, ");
                        groupBy.Append("Pedido.PED_RESERVA, ");
                    }
                    break;

                case "Resumo":
                    if (!select.Contains(" Resumo,"))
                    {
                        select.Append("Pedido.PED_RESUMO Resumo, ");
                        groupBy.Append("Pedido.PED_RESUMO, ");
                    }
                    break;

                case "Temperatura":
                    if (!select.Contains(" Temperatura,"))
                    {
                        select.Append("Pedido.PED_TEMPERATURA Temperatura, ");
                        groupBy.Append("Pedido.PED_TEMPERATURA, ");
                    }
                    break;

                case "TipoEmbarque":
                    if (!select.Contains(" TipoEmbarque,"))
                    {
                        select.Append("Pedido.PED_TIPO_EMBARQUE TipoEmbarque, ");
                        groupBy.Append("Pedido.PED_TIPO_EMBARQUE, ");
                    }
                    break;

                case "DataCadastro":
                    if (!select.Contains(" DataCadastro,"))
                    {
                        select.Append("CONVERT(nvarchar(8), Pedido.PED_DATA_CADASTRO, 3) + ' ' + CONVERT(nvarchar(5), Pedido.PED_DATA_CADASTRO, 8) DataCadastro, ");
                        groupBy.Append("Pedido.PED_DATA_CADASTRO, ");
                    }
                    break;

                case "DataCriacaoCarga":
                    if (!select.Contains(" DataCriacaoCarga,"))
                    {
                        select.Append("convert(varchar(10), Pedido.PED_DATA_CRIACAO, 103) + ' ' + convert(varchar(5), Pedido.PED_DATA_CRIACAO, 108) DataCriacaoCarga, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "NumeroPedido":
                    if (!select.Contains(" NumeroPedido,"))
                    {
                        select.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR NumeroPedido, ");
                        groupBy.Append("Pedido.PED_NUMERO_PEDIDO_EMBARCADOR, ");
                    }
                    break;

                case "NumeroPedidoInterno":
                    if (!select.Contains(" NumeroPedidoInterno,"))
                    {
                        select.Append("Convert(NVARCHAR(15), Pedido.PED_NUMERO) NumeroPedidoInterno, ");
                        groupBy.Append("Pedido.PED_NUMERO, ");
                    }
                    break;

                case "Transbordo":
                    if (!select.Contains(" Transbordo,"))
                    {
                        select.Append("(CASE Carga.CAR_CARGA_TRANSBORDO WHEN 1 THEN 'Sim' ELSE 'Não' END) Transbordo, ");
                        groupBy.Append("Carga.CAR_CARGA_TRANSBORDO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "SituacaoPedido":
                case "DescricaoSituacaoPedido":
                    if (!select.Contains(" SituacaoPedido,"))
                    {
                        select.Append("Pedido.PED_SITUACAO SituacaoPedido, Pedido.PED_CANCELADO_APOS_VINCULO_COM_CARGA CanceladoAposVinculoCarga, ");
                        groupBy.Append("Pedido.PED_SITUACAO, Pedido.PED_CANCELADO_APOS_VINCULO_COM_CARGA, ");
                    }
                    break;

                case "DataChipFormatada":
                    if (!select.Contains(" DataChip,"))
                    {
                        select.Append("Pedido.PED_DATA_CHIP DataChip, ");
                        groupBy.Append("Pedido.PED_DATA_CHIP, ");
                    }
                    break;

                case "DataCancelFormatada":
                    if (!select.Contains(" DataCancel,"))
                    {
                        select.Append("Pedido.PED_DATA_CANCEL DataCancel, ");
                        groupBy.Append("Pedido.PED_DATA_CANCEL, ");
                    }
                    break;

                case "ObservacaoCTe":
                    if (!select.Contains(" ObservacaoCTe,"))
                    {
                        select.Append("Pedido.PED_OBSERVACAO_CTE ObservacaoCTe, ");
                        groupBy.Append("Pedido.PED_OBSERVACAO_CTE, ");
                    }
                    break;

                case "EscritorioVenda":
                    if (!select.Contains(" EscritorioVenda,"))
                    {
                        select.Append("Pedido.PED_ESCRITORIO_VENDA EscritorioVenda, ");

                        if (!groupBy.Contains("Pedido.PED_ESCRITORIO_VENDA"))
                            groupBy.Append("Pedido.PED_ESCRITORIO_VENDA, ");
                    }
                    break;

                case "EquipeVendas":
                    if (!select.Contains(" EquipeVendas,"))
                    {
                        select.Append("Pedido.PED_EQUIPE_VENDAS EquipeVendas, ");

                        if (!groupBy.Contains("Pedido.PED_EQUIPE_VENDAS"))
                            groupBy.Append("Pedido.PED_EQUIPE_VENDAS, ");
                    }
                    break;

                case "TipoMercado":
                    if (!select.Contains(" TipoMercado,"))
                    {
                        select.Append("Pedido.PED_TIPO_MERCADORIA TipoMercado, ");

                        if (!groupBy.Contains("Pedido.PED_TIPO_MERCADORIA"))
                            groupBy.Append("Pedido.PED_TIPO_MERCADORIA, ");
                    }
                    break;

                case "CanalVenda":
                    if (!select.Contains(" CanalVenda,"))
                    {
                        select.Append("canalvenda.CNV_DESCRICAO CanalVenda, ");

                        if (!groupBy.Contains("canalvenda.CNV_DESCRICAO"))
                            groupBy.Append("canalvenda.CNV_DESCRICAO, ");

                        SetarJoinsCanalVenda(joins);
                    }
                    break;

                case "CargaCritica":
                    if (!select.Contains(" CargaCritica,"))
                    {
                        select.Append("Carga.CAR_CARGA_CRITICA CargaCritica, ");

                        if (!groupBy.Contains("Carga.CAR_CARGA_CRITICA"))
                            groupBy.Append("Carga.CAR_CARGA_CRITICA, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "CanalEntrega":
                    if (!select.Contains(" CanalEntrega,"))
                    {
                        select.Append("CanalEntrega.CNE_DESCRICAO CanalEntrega, ");
                        groupBy.Append("CanalEntrega.CNE_DESCRICAO, ");

                        SetarJoinsCanalEntrega(joins);
                    }
                    break;

                case "Filial":
                    if (!select.Contains(" Filial,"))
                    {
                        select.Append("Filial.FIL_DESCRICAO Filial, ");
                        groupBy.Append("Filial.FIL_DESCRICAO, ");

                        SetarJoinsFilial(joins, filtrosPesquisa);
                    }
                    break;

                case "ModeloVeicular":
                    if (!select.Contains(" ModeloVeicular,"))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO ModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloveicularCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "CapacidadePesoModeloVeicular":
                    if (!select.Contains(" CapacidadePesoModeloVeicular,"))
                    {
                        select.Append("ModeloVeicular.MVC_CAPACIDADE_PESO_TRANSPORTE CapacidadePesoModeloVeicular, ");
                        groupBy.Append("ModeloVeicular.MVC_CAPACIDADE_PESO_TRANSPORTE, ");

                        SetarJoinsModeloveicularCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "CodigoPedidoCliente":
                    if (!select.Contains(" CodigoPedidoCliente,"))
                    {
                        select.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE CodigoPedidoCliente, ");
                        groupBy.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE, ");
                    }
                    break;

                case "Remetente":
                    if (!select.Contains(" Remetente,"))
                    {
                        select.Append("PedidoRemetente.CLI_NOME Remetente, ");
                        groupBy.Append("PedidoRemetente.CLI_NOME, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CNPJRemetenteFormatado":
                    if (!select.Contains(" CNPJRemetente,"))
                    {
                        select.Append("PedidoRemetente.CLI_CGCCPF CNPJRemetente, PedidoRemetente.CLI_FISJUR TipoRemetente, ");
                        groupBy.Append("PedidoRemetente.CLI_CGCCPF, PedidoRemetente.CLI_FISJUR, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "GrupoRemetente":
                    if (!select.Contains(" GrupoRemetente,"))
                    {
                        select.Append("GrupoPessoaRemetente.GRP_DESCRICAO GrupoRemetente, ");
                        groupBy.Append("GrupoPessoaRemetente.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaRemetente(joins);
                    }
                    break;

                case "CategoriaRemetente":
                    if (!select.Contains(" CategoriaRemetente,"))
                    {
                        select.Append("CategoriaRemetente.CTP_DESCRICAO CategoriaRemetente, ");
                        groupBy.Append("CategoriaRemetente.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaRemetente(joins);
                    }
                    break;

                case "Destinatario":
                    if (!select.Contains(" Destinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_NOME Destinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_NOME, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "EnderecoPrincipalDestinatario":
                    if (!select.Contains(" EnderecoPrincipalDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_ENDERECO EnderecoPrincipalDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_ENDERECO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "BairroDestinatario":
                    if (!select.Contains(" BairroDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_BAIRRO BairroDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_BAIRRO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NumeroDestinatario":
                    if (!select.Contains(" NumeroDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_NUMERO NumeroDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_NUMERO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "ComplementoDestinatario":
                    if (!select.Contains(" ComplementoDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_COMPLEMENTO ComplementoDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_COMPLEMENTO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CNPJDestinatarioFormatado":
                    if (!select.Contains(" CNPJDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_CGCCPF CNPJDestinatario, PedidoDestinatario.CLI_FISJUR TipoDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_CGCCPF, PedidoDestinatario.CLI_FISJUR, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "GrupoDestinatario":
                    if (!select.Contains(" GrupoDestinatario,"))
                    {
                        select.Append("GrupoPessoaDestinatario.GRP_DESCRICAO GrupoDestinatario, ");
                        groupBy.Append("GrupoPessoaDestinatario.GRP_DESCRICAO, ");

                        SetarJoinsGrupoPessoaDestinatario(joins);
                    }
                    break;

                case "CategoriaDestinatario":
                    if (!select.Contains(" CategoriaDestinatario,"))
                    {
                        select.Append("CategoriaDestinatario.CTP_DESCRICAO CategoriaDestinatario, ");
                        groupBy.Append("CategoriaDestinatario.CTP_DESCRICAO, ");

                        SetarJoinsCategoriaDestinatario(joins);
                    }
                    break;

                case "PrevisaoEntregaPedidoFormatada":
                    if (!select.Contains(" PrevisaoEntregaPedido,"))
                    {
                        select.Append("Pedido.PED_PREVISAO_ENTREGA PrevisaoEntregaPedido, ");
                        groupBy.Append("Pedido.PED_PREVISAO_ENTREGA, ");
                    }
                    break;

                case "DataPrevistaFormatada":
                    if (!select.Contains(" DataPrevista,"))
                    {
                        select.Append(@"(select TOP 1 cargaEntrega.CEN_DATA_ENTREGA_PREVISTA from T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido left join T_CARGA_ENTREGA cargaEntrega on cargaEntrega.CEN_CODIGO = cargaEntregaPedido.CEN_CODIGO WHERE cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and cargaEntrega.CEN_COLETA = 0 AND cargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO) DataPrevista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "PrevisaoEntregaAnteriorString":
                    if (!select.Contains(" PrevisaoEntregaAnterior,"))
                    {
                        select.Append("(SELECT TOP 1 PHD_DATA_ANTERIOR FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 2 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoEntregaAnterior, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "PrevisaoEntregaAnteriorResponsavel":
                case "PrevisaoEntregaAnteriorResponsavelDescricao":
                    if (!select.Contains(" PrevisaoEntregaAnteriorResponsavel,"))
                    {
                        select.Append("(SELECT TOP 1 PDH_RESPONSAVEL FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 2 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoEntregaAnteriorResponsavel, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "PrevisaoEntregaAnteriorObservacao":
                    if (!select.Contains(" PrevisaoEntregaAnteriorObservacao,"))
                    {
                        select.Append("(SELECT TOP 1 PHD_OBSERVACAO FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 2 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoEntregaAnteriorObservacao, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "PrevisaoSaidaString":
                    if (!select.Contains(" PrevisaoSaida,"))
                    {
                        select.Append("Pedido.PED_DATA_PREVISAO_SAIDA PrevisaoSaida, ");
                        groupBy.Append("Pedido.PED_DATA_PREVISAO_SAIDA, ");
                    }
                    break;

                case "PrevisaoSaidaAnteriorString":
                    if (!select.Contains(" PrevisaoSaidaAnterior,"))
                    {
                        select.Append("(SELECT TOP 1 PHD_DATA_ANTERIOR FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 1 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoSaidaAnterior, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "PrevisaoSaidaAnteriorResponsavel":
                case "PrevisaoSaidaAnteriorResponsavelDescricao":
                    if (!select.Contains(" PrevisaoSaidaAnteriorResponsavel,"))
                    {
                        select.Append("(SELECT TOP 1 PDH_RESPONSAVEL FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 1 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoSaidaAnteriorResponsavel, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "PrevisaoSaidaAnteriorObservacao":
                    if (!select.Contains(" PrevisaoSaidaAnteriorObservacao,"))
                    {
                        select.Append("(SELECT TOP 1 PHD_OBSERVACAO FROM T_PEDIDO_HISTORICO_ALTERACAO_DATA WHERE PED_CODIGO = Pedido.PED_CODIGO AND PDH_TIPO_DATA = 1 ORDER BY PHD_DATA_ALTERACAO DESC) PrevisaoSaidaAnteriorObservacao, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "Paletes":
                    if (!select.Contains(" Paletes,"))
                    {
                        select.Append("(Pedido.PED_NUMERO_PALETES + Pedido.PED_NUMERO_PALETES_FRACIONADO) Paletes, ");
                        groupBy.Append("Pedido.PED_NUMERO_PALETES, Pedido.PED_NUMERO_PALETES_FRACIONADO, ");
                    }
                    break;

                case "ICMS":
                    if (!select.Contains(" ICMS,"))
                    {
                        select.Append("CargaPedido.PED_VALOR_ICMS ICMS, ");
                        groupBy.Append("CargaPedido.PED_VALOR_ICMS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "CSTIBSCBS":
                    if (!select.Contains(" CSTIBSCBS, "))
                    {
                       
                        select.Append("CargaPedido.CPE_CST_IBSCBS CSTIBSCBS, ");
                        groupBy.Append("CargaPedido.CPE_CST_IBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ClassificacaoTributariaIBSCBS":
                    if (!select.Contains(" ClassificacaoTributariaIBSCBS, "))
                    {
                        select.Append("CargaPedido.CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS ClassificacaoTributariaIBSCBS, ");
                        groupBy.Append("CargaPedido.CPE_CLASSIFICACAO_TRIBUTARIA_IBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "BaseCalculoIBSCBS":
                    if (!select.Contains(" BaseCalculoIBSCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_BASE_CALCULO_IBSCBS) BaseCalculoIBSCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaIBSEstadual":
                    if (!select.Contains(" AliquotaIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_IBS_ESTADUAL) AliquotaIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoIBSEstadual":
                    if (!select.Contains(" PercentualReducaoIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_IBS_ESTADUAL) PercentualReducaoIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorIBSEstadual":
                    if (!select.Contains(" ValorIBSEstadual, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_IBS_ESTADUAL) ValorIBSEstadual, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaIBSMunicipal":
                    if (!select.Contains(" AliquotaIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_IBS_MUNICIPAL) AliquotaIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoIBSMunicipal":
                    if (!select.Contains(" PercentualReducaoIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_IBS_MUNICIPAL) PercentualReducaoIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorIBSMunicipal":
                    if (!select.Contains(" ValorIBSMunicipal, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_IBS_MUNICIPAL) ValorIBSMunicipal, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "AliquotaCBS":
                    if (!select.Contains(" AliquotaCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_ALIQUOTA_CBS) AliquotaCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "PercentualReducaoCBS":
                    if (!select.Contains(" PercentualReducaoCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_PERCENTUAL_REDUCAO_CBS) PercentualReducaoCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ValorCBS":
                    if (!select.Contains(" ValorCBS, "))
                    {
                        select.Append("min(CargaPedido.CPE_VALOR_CBS) ValorCBS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;


                case "FreteLiquido":
                    if (!select.Contains(" FreteLiquido,"))
                    {
                        select.Append("CargaPedido.PED_VALOR_FRETE FreteLiquido, ");
                        groupBy.Append("CargaPedido.PED_VALOR_FRETE, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "TotalReceber":
                    if (!select.Contains(" TotalReceber,"))
                    {
                        select.Append("CargaPedido.PED_VALOR_FRETE_PAGAR TotalReceber, ");
                        groupBy.Append("CargaPedido.PED_VALOR_FRETE_PAGAR, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "Volume":
                    if (!select.Contains(" Volume,"))
                    {
                        select.Append("Pedido.PED_QUANTIDADE_VOLUMES Volume, ");
                        groupBy.Append("Pedido.PED_QUANTIDADE_VOLUMES, ");
                    }
                    break;

                case "ValorMercadoria":
                    if (!select.Contains(" ValorMercadoria,"))
                    {
                        select.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorMercadoria, ");
                        groupBy.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, ");
                    }
                    break;

                case "ISS":
                    if (!select.Contains(" ISS,"))
                    {
                        select.Append("CargaPedido.PED_VALOR_ISS ISS, ");
                        groupBy.Append("CargaPedido.PED_VALOR_ISS, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "Origem":
                    if (!select.Contains(" Origem,"))
                    {
                        select.Append("CPOrigem.LOC_DESCRICAO Origem, ");
                        groupBy.Append("CPOrigem.LOC_DESCRICAO, ");

                        SetarJoinsOrigem(joins, filtrosPesquisa);
                    }
                    break;

                case "PaisOrigem":
                    if (!select.Contains(" PaisOrigem,"))
                    {
                        select.Append("CPPaisOrigem.PAI_NOME PaisOrigem, ");
                        groupBy.Append("CPPaisOrigem.PAI_NOME, ");

                        SetarJoinsPaisOrigem(joins, filtrosPesquisa);
                    }
                    break;

                case "UFOrigem":
                    if (!select.Contains(" UFOrigem,"))
                    {
                        select.Append("CPOrigem.UF_SIGLA UFOrigem, ");
                        groupBy.Append("CPOrigem.UF_SIGLA, ");

                        SetarJoinsOrigem(joins, filtrosPesquisa);
                    }
                    break;

                case "Destino":
                    if (!select.Contains(" Destino,"))
                    {
                        select.Append("CPDestino.LOC_DESCRICAO Destino, ");
                        groupBy.Append("CPDestino.LOC_DESCRICAO, ");

                        SetarJoinsDestino(joins, filtrosPesquisa);
                    }
                    break;

                case "PaisDestino":
                    if (!select.Contains(" PaisDestino,"))
                    {
                        select.Append("CPPaisDestino.PAI_NOME PaisDestino, ");
                        groupBy.Append("CPPaisDestino.PAI_NOME, ");

                        SetarJoinsPaisDestino(joins, filtrosPesquisa);
                    }
                    break;

                case "UFDestino":
                    if (!select.Contains(" UFDestino,"))
                    {
                        select.Append("CPDestino.UF_SIGLA UFDestino, ");
                        groupBy.Append("CPDestino.UF_SIGLA, ");

                        SetarJoinsDestino(joins, filtrosPesquisa);
                    }
                    break;

                case "Expedidor":
                    if (!select.Contains(" Expedidor,"))
                    {
                        select.Append("CPExpedidor.CLI_NOME Expedidor, ");
                        groupBy.Append("CPExpedidor.CLI_NOME, ");

                        SetarJoinsExpedidor(joins, filtrosPesquisa);
                    }
                    break;

                case "CNPJExpedidorFormatado":
                    if (!select.Contains(" CNPJExpedidor,"))
                    {
                        select.Append("CPExpedidor.CLI_CGCCPF CNPJExpedidor, CPExpedidor.CLI_FISJUR TipoExpedidor, ");
                        groupBy.Append("CPExpedidor.CLI_CGCCPF, CPExpedidor.CLI_FISJUR, ");

                        SetarJoinsExpedidor(joins, filtrosPesquisa);
                    }
                    break;

                case "Recebedor":
                    if (!select.Contains(" Recebedor,"))
                    {
                        select.Append("CPRecebedor.CLI_NOME Recebedor, ");
                        groupBy.Append("CPRecebedor.CLI_NOME, ");

                        SetarJoinsRecebedor(joins, filtrosPesquisa);
                    }
                    break;

                case "CNPJRecebedorFormatado":
                    if (!select.Contains(" CNPJRecebedor,"))
                    {
                        select.Append("CPRecebedor.CLI_CGCCPF CNPJRecebedor, CPRecebedor.CLI_FISJUR TipoRecebedor, ");
                        groupBy.Append("CPRecebedor.CLI_CGCCPF, CPRecebedor.CLI_FISJUR, ");

                        SetarJoinsRecebedor(joins, filtrosPesquisa);
                    }
                    break;

                case "Tomador":
                    if (!select.Contains(" TipoTomadorPagador,"))
                    {
                        SetarSelect("Destinatario", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("Expedidor", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("Recebedor", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("Remetente", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("TomadorOutros", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("CNPJDestinatarioFormatado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("CNPJExpedidorFormatado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("CNPJRecebedorFormatado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("CNPJRemetenteFormatado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
                        SetarSelect("CNPJTomadorOutrosFormatado", 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append("Pedido.PED_TIPO_TOMADOR TipoTomadorPagador, ");
                            groupBy.Append("Pedido.PED_TIPO_TOMADOR, ");
                        }
                        else
                        {
                            select.Append("CargaPedido.PED_TIPO_TOMADOR TipoTomadorPagador, ");
                            groupBy.Append("CargaPedido.PED_TIPO_TOMADOR, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "Peso":
                    if (!select.Contains(" Peso,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append("Pedido.PED_PESO_TOTAL_CARGA Peso, ");
                            groupBy.Append("Pedido.PED_PESO_TOTAL_CARGA, ");
                        }
                        else
                        {
                            select.Append("CargaPedido.PED_PESO Peso, ");
                            groupBy.Append("CargaPedido.PED_PESO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "CubagemPedido":
                    if (!select.Contains(" CubagemPedido,"))
                    {
                        select.Append("Pedido.PED_CUBAGEM_TOTAL CubagemPedido, ");
                        groupBy.Append("Pedido.PED_CUBAGEM_TOTAL, ");
                    }
                    break;

                case "PesoPedido":
                    if (!select.Contains(" PesoPedido,"))
                    {
                        select.Append("Pedido.PED_PESO_TOTAL_CARGA PesoPedido, ");
                        groupBy.Append("Pedido.PED_PESO_TOTAL_CARGA, ");
                    }
                    break;

                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ");
                        groupBy.Append("Carga.CAR_CODIGO_CARGA_EMBARCADOR, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataCarregamentoString":
                    if (!select.Contains(" DataCarregamento,"))
                    {
                        select.Append("Carga.CAR_DATA_CARREGAMENTO DataCarregamento, ");
                        groupBy.Append("Carga.CAR_DATA_CARREGAMENTO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataColetaString":
                    if (!select.Contains(" DataColeta,"))
                    {
                        select.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO DataColeta, ");
                        groupBy.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, ");
                    }
                    break;

                case "Transportador":
                    if (!select.Contains(" Transportador,"))
                    {
                        select.Append("CEmpresa.EMP_RAZAO Transportador, ");
                        groupBy.Append("CEmpresa.EMP_RAZAO, ");

                        SetarJoinsTransportador(joins, filtrosPesquisa);
                    }
                    break;

                case "TipoCarga":
                    if (!select.Contains(" TipoCarga,"))
                    {
                        select.Append("CTipoCarga.TCG_DESCRICAO TipoCarga, ");
                        groupBy.Append("CTipoCarga.TCG_DESCRICAO, ");

                        SetarJoinsTipoCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "TipoOperacao":
                    if (!select.Contains(" TipoOperacao,"))
                    {
                        select.Append("CTipoOperacao.TOP_DESCRICAO TipoOperacao, ");
                        groupBy.Append("CTipoOperacao.TOP_DESCRICAO, ");

                        SetarJoinsTipoOperacao(joins, filtrosPesquisa);
                    }
                    break;

                case "RotaFrete":
                    if (!select.Contains(" RotaFrete,"))
                    {
                        select.Append(
                            @"CASE 
                                WHEN LEN(RotaFrete.ROF_CODIGO_INTEGRACAO) = 0 OR RotaFrete.ROF_CODIGO_INTEGRACAO IS NULL THEN RotaFrete.ROF_DESCRICAO 
		                        ELSE CONCAT(RotaFrete.ROF_DESCRICAO, ' - ', RotaFrete.ROF_CODIGO_INTEGRACAO) 
	                        END RotaFrete, "
                        );

                        groupBy.Append("RotaFrete.ROF_DESCRICAO, RotaFrete.ROF_CODIGO_INTEGRACAO, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;

                case "Restricoes":
                    if (!select.Contains(" Restricoes,"))
                    {
                        select.Append(
                            @"SUBSTRING((
                                select ', ' + _restricaoEntrega.REE_DESCRICAO
                                  from T_CLIENTE_DESCARGA _clienteDescarga
                                  join T_CLIENTE_RESTRICAO_DESCARGA _clienteRestricaoDescarga on _clienteRestricaoDescarga.CLD_CODIGO = _clienteDescarga.CLD_CODIGO
                                  join T_RESTRICAO_ENTREGA _restricaoEntrega on _restricaoEntrega.REE_CODIGO = _clienteRestricaoDescarga.REE_CODIGO
                                 where _clienteDescarga.CLI_CGCCPF = Pedido.CLI_CODIGO
                                   for xml path('')
                            ), 3, 1000) Restricoes, "
                        );

                        if (!groupBy.Contains("Pedido.CLI_CODIGO, "))
                            groupBy.Append("Pedido.CLI_CODIGO, ");
                    }
                    break;

                case "GrupoPessoas":
                    if (!select.Contains(" GrupoPessoas,"))
                    {
                        select.Append("CGrupoPessoa.GRP_DESCRICAO GrupoPessoas, ");
                        groupBy.Append("CGrupoPessoa.GRP_DESCRICAO, ");

                        SetarJoinsCargaGrupoPessoa(joins, filtrosPesquisa);
                    }
                    break;

                case "Veiculos":
                    if (!select.Contains(" Veiculos,"))
                    {
                        select.Append(
                            @"((
		                        SELECT Vei.VEI_PLACA 
		                          FROM T_VEICULO Vei 
		                         WHERE Vei.VEI_CODIGO = Carga.CAR_VEICULO
	                        ) + ISNULL((
		                        SELECT ', ' + VeiculoFrota.VEI_PLACA 
		                          FROM T_CARGA_VEICULOS_VINCULADOS VeiculoVinculadoFrota INNER JOIN T_VEICULO VeiculoFrota ON VeiculoVinculadoFrota.VEI_CODIGO = VeiculoFrota.VEI_CODIGO 
		                         WHERE VeiculoVinculadoFrota.CAR_CODIGO = Carga.CAR_CODIGO FOR XML PATH('')), ''
                            )) Veiculos, ");

                        groupBy.Append("Carga.CAR_VEICULO, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "NotasFiscais":
                    if (!select.Contains(" NotasFiscais,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"substring((
		                            select ', ' + convert(nvarchar(50), _xmlnotafiscal.NF_NUMERO)
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
		                             where _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                       for xml path('')
                                ), 3, 1000) NotasFiscais, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"substring((
		                            select ', ' + convert(nvarchar(50), _xmlnotafiscal.NF_NUMERO) 
		                              from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO 
		                             where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                       for xml path('')
                                ), 3, 1000) NotasFiscais, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "VolumeNF":
                    if (!select.Contains(" VolumeNF,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"(
                                    select sum(_xmlnotafiscal.NF_VOLUMES)
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
                                     where _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                ) VolumeNF, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"(
                                    select sum(_xmlnotafiscal.NF_VOLUMES)
                                      from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO
                                     where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                ) VolumeNF, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "QuantidadeTotalProduto":
                    if (!select.Contains(" QuantidadeTotalProduto,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"(
		                            select sum(_xmlProduto.XFP_QUANTIDADE)
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL_PRODUTO _xmlProduto on _xmlProduto.NFX_CODIGO = _pedidoXml.NFX_CODIGO
                                     where _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                ) QuantidadeTotalProduto, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"(
		                            select sum(_xmlProduto.XFP_QUANTIDADE) 
		                              from T_PEDIDO_XML_NOTA_FISCAL _cargaPedidoXmlNotaFiscal
                                      join T_XML_NOTA_FISCAL_PRODUTO _xmlProduto on _xmlProduto.NFX_CODIGO = _cargaPedidoXmlNotaFiscal.NFX_CODIGO
		                             where _cargaPedidoXmlNotaFiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                ) QuantidadeTotalProduto, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "CTes":
                    if (!select.Contains(" CTes,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"substring((
		                            select ', ' + CONVERT(NVARCHAR(50), _cte.CON_NUM) 
                                      from VIEW_PEDIDO_CTE _pedidoCte
                                      join T_CTE _cte on _cte.CON_CODIGO = _pedidoCte.CON_CODIGO
		                             where _pedidoCte.PED_CODIGO = Pedido.PED_CODIGO
	                                   for xml path('')
                                ), 3, 1000) CTes, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"substring((
		                            select ', ' + CONVERT(NVARCHAR(50), _cte.CON_NUM) 
		                              from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal
		                              left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargapedidoxmlnotafiscalcte on _cargapedidoxmlnotafiscalcte.PNF_CODIGO = _cargapedidoxmlnotafiscal.PNF_CODIGO
		                              left join T_CARGA_CTE _cargacte on _cargacte.CCT_CODIGO = _cargapedidoxmlnotafiscalcte.CCT_CODIGO
		                              left join T_CTE _cte on _cte.CON_CODIGO = _cargacte.CON_CODIGO
		                             where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                       and _cargacte.CCC_CODIGO is null
		                             group BY _cte.CON_NUM
	                                   for xml path('')
                                ), 3, 1000) CTes, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "Fatura":
                    if (!select.Contains(" Fatura,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"substring((
		                            select ', ' + convert(nvarchar(50), _fatura.FAT_NUMERO) 
		                              from VIEW_PEDIDO_CTE _pedidoCte
                                      join T_CTE _cte on _cte.CON_CODIGO = _pedidoCte.CON_CODIGO
		                              join T_DOCUMENTO_FATURAMENTO _documentoFataramento on _documentoFataramento.CON_CODIGO = _cte.CON_CODIGO
		                              join t_fatura_documento _faturaDocumento on _faturaDocumento.DFA_CODIGO = _documentoFataramento.DFA_CODIGO
		                              join T_FATURA _fatura on _fatura.FAT_CODIGO = _faturaDocumento.FAT_CODIGO 
		                              join T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                             where _pedidoCte.PED_CODIGO = Pedido.PED_CODIGO
		                             group by _fatura.FAT_NUMERO
	                                   for xml path('')
                                ), 3, 1000) Fatura, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"substrinG((
		                            select ', ' + convert(nvarchar(50), _fatura.FAT_NUMERO) 
		                              from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal
		                              left join T_XML_NOTA_FISCAL xmlnotafiscal on xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO
		                              left join T_CARGA_PEDIDO_XML_NOTA_FISCAL_CTE _cargapedidoxmlnotafiscalcte on _cargapedidoxmlnotafiscalcte.PNF_CODIGO = _cargapedidoxmlnotafiscal.PNF_CODIGO
		                              left join T_CARGA_CTE _cargacte on _cargacte.CCT_CODIGO = _cargapedidoxmlnotafiscalcte.CCT_CODIGO
		                              left join T_CTE _cte on _cte.CON_CODIGO = _cargacte.CON_CODIGO
		                              join T_DOCUMENTO_FATURAMENTO _documentoFataramento on _documentoFataramento.CON_CODIGO = _cte.CON_CODIGO
		                              join t_fatura_documento _faturaDocumento on _faturaDocumento.DFA_CODIGO = _documentoFataramento.DFA_CODIGO
		                              join T_FATURA _fatura on _fatura.FAT_CODIGO = _faturaDocumento.FAT_CODIGO 
		                              join T_CTE_XML_NOTAS_FISCAIS _ctexmlnotafiscal on _ctexmlnotafiscal.CON_CODIGO = _cte.CON_CODIGO 
		                             where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                       and _cargacte.CCC_CODIGO is null
		                             group BY _fatura.FAT_NUMERO
	                                   for xml path('')
                                ), 3, 1000) Fatura, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "DataInicioEmissaoDocumentos":
                case "DataInicioEmissaoDocumentosFormatada":
                    if (!select.Contains("DataInicioEmissaoDocumentos, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_GERACAO_CTES DataInicioEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_GERACAO_CTES, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataFimEmissaoDocumentos":
                    if (!select.Contains("DataFimEmissaoDocumentos, "))
                    {
                        select.Append("CONVERT(VARCHAR, Carga.CAR_DATA_FINALIZACAO_EMISSAO, 103) DataFimEmissaoDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_FINALIZACAO_EMISSAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "Motoristas":
                    if (!select.Contains(" Motoristas,"))
                    {
                        select.Append(@"SUBSTRING((
			                            SELECT ISNULL(PedMotorista.PedMotoristaNome, ' ') + ', ' + Motorista.FUN_NOME
			                            FROM T_CARGA_MOTORISTA CargaMotorista
			                            INNER JOIN T_FUNCIONARIO Motorista ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
			                            OUTER APPLY (
			                            		SELECT ', ' + Motorista.FUN_NOME AS PedMotoristaNome
			                            		FROM T_PEDIDO_MOTORISTA pedMotorista
			                            		INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = pedMotorista.FUN_CODIGO
                                                AND Motorista.FUN_CODIGO != CargaMotorista.CAR_MOTORISTA			                            		
                                                WHERE pedMotorista.PED_CODIGO = Pedido.PED_CODIGO
			                            	) AS PedMotorista
			                            WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
			                            FOR XML PATH('')
			                            ), 3, 1000) Motoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO,"))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "TipoSeparacao":
                    if (!select.Contains(" TipoSeparacao, "))
                    {
                        select.Append("TipoSeparacao.TSE_DESCRICAO TipoSeparacao, ");
                        groupBy.Append("TipoSeparacao.TSE_DESCRICAO, ");

                        SetarJoinsTipoSeparacao(joins, filtrosPesquisa);
                    }
                    break;

                case "SenhaAgendamento":
                    if (!select.Contains(" SenhaAgendamento, "))
                    {
                        select.Append("Pedido.PED_SENHA_AGENDAMENTO SenhaAgendamento, ");
                        groupBy.Append("Pedido.PED_SENHA_AGENDAMENTO, ");
                    }
                    break;

                case "SenhaAgendamentoCliente":
                    if (!select.Contains(" SenhaAgendamentoCliente, "))
                    {
                        select.Append("Pedido.PED_SENHA_AGENDAMENTO_CLIENTE SenhaAgendamentoCliente, ");
                        groupBy.Append("Pedido.PED_SENHA_AGENDAMENTO_CLIENTE, ");
                    }
                    break;

                case "QtdeItensProdutos":
                    if (!select.Contains(" QtdeItensProdutos, "))
                    {
                        select.Append(
                            @"(
                                select sum(_cargaPedidoProduto.CPP_QUANTIDADE)
                                  from T_CARGA_PEDIDO_PRODUTO _cargaPedidoProduto
                                 where _cargaPedidoProduto.CPE_CODIGO = CargaPedido.CPE_CODIGO
                            ) QtdeItensProdutos, "
                        );

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "ObservacaoInterna":
                    if (!select.Contains("ObservacaoInterna, "))
                    {
                        select.Append("Pedido.PED_OBSERVACAO_INTERNA ObservacaoInterna, ");
                        groupBy.Append("Pedido.PED_OBSERVACAO_INTERNA, ");
                    }
                    break;

                case "ObservacaoCarga":
                    if (!select.Contains(" ObservacaoCarga, "))
                    {
                        select.Append("MontagemCarga.CRG_OBSERVACAO ObservacaoCarga, ");
                        groupBy.Append("MontagemCarga.CRG_OBSERVACAO, ");

                        SetarJoinsMontagemCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DescricaoSituacaoEntrega":
                    if (!select.Contains(" SituacaoEntrega, "))
                    {
                        select.Append(" SituacoesEntrega.CEN_SITUACAO AS SituacaoEntrega, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CAR_CODIGO_ORIGEM, "))
                            groupBy.Append("CargaPedido.CAR_CODIGO_ORIGEM, ");

                        if (!groupBy.Contains("CargaPedido.CAR_CODIGO, "))
                            groupBy.Append("CargaPedido.CAR_CODIGO, ");

                        if (!groupBy.Contains("SituacoesEntrega.CEN_SITUACAO, "))
                            groupBy.Append("SituacoesEntrega.CEN_SITUACAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                        SetarJoinSituacoesEntrega(joins, filtrosPesquisa);
                    }
                    break;

                case "DescricaoSituacaoCarga":
                    if (!select.Contains(" SituacaoCarga, "))
                    {
                        select.Append("Carga.CAR_SITUACAO SituacaoCarga, ");
                        groupBy.Append("Carga.CAR_SITUACAO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataInicioViagemFormatada":
                    if (!select.Contains(" DataInicioViagem, "))
                    {
                        select.Append("Carga.CAR_DATA_INICIO_VIAGEM DataInicioViagem, ");
                        groupBy.Append("Carga.CAR_DATA_INICIO_VIAGEM, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataEntregaFormatada":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        select.Append(@"(select TOP 1 cargaEntrega.CEN_DATA_FIM_ENTREGA from T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido left join T_CARGA_ENTREGA cargaEntrega on cargaEntrega.CEN_CODIGO = cargaEntregaPedido.CEN_CODIGO WHERE cargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO and cargaEntrega.CEN_COLETA = 0 AND cargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO) DataEntrega, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "Saldo":
                    if (!select.Contains(" Saldo, "))
                    {
                        select.Append("Pedido.PED_SALDO_VOLUMES_RESTANTE Saldo, ");
                        groupBy.Append("Pedido.PED_SALDO_VOLUMES_RESTANTE, ");
                    }
                    break;

                case "PedidoComAgenda":
                    if (!select.Contains(" PedidoComAgenda, "))
                    {
                        select.Append("(CASE WHEN EXISTS (SELECT _AgendamendoPedido.ACP_CODIGO FROM T_AGENDAMENTO_COLETA_PEDIDO _AgendamendoPedido WHERE _AgendamendoPedido.PED_CODIGO = Pedido.PED_CODIGO) THEN 'Sim' ELSE 'Não' END) PedidoComAgenda, ");
                        groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "DataFimJanela":
                case "DataFimJanelaFormatada":
                    if (!select.Contains(" DataFimJanela, "))
                    {
                        select.Append("Pedido.PED_DATA_VALIDADE DataFimJanela, ");
                        groupBy.Append("Pedido.PED_DATA_VALIDADE, ");
                    }
                    break;

                case "Gerente":
                    if (!select.Contains(" Gerente, "))
                    {
                        select.Append("COALESCE(FuncionarioGerente.FUN_NOME, FuncionarioGerenteSupervisor.FUN_NOME) AS Gerente, ");
                        groupBy.Append("FuncionarioGerente.FUN_NOME, FuncionarioGerenteSupervisor.FUN_NOME, ");

                        SetarJoinsFuncionarioGerente(joins);
                        SetarJoinsFuncionarioGerenteSupervisor(joins);
                    }
                    break;

                case "GerenteRegional":
                    if (!select.Contains("GerenteRegional, "))
                    {
                        select.Append("COALESCE(FuncionarioGerenteRegional.FUN_NOME, FuncionarioGerenteRegionalSupervisor.FUN_NOME) AS GerenteRegional, ");
                        groupBy.Append("FuncionarioGerenteRegional.FUN_NOME, FuncionarioGerenteRegionalSupervisor.FUN_NOME, ");

                        SetarJoinsFuncionarioGerenteRegional(joins);
                        SetarJoinsFuncionarioGerenteRegionalSupervisor(joins);
                    }
                    break;

                case "Vendedor":
                    if (!select.Contains(" Vendedor, "))
                    {
                        select.Append("FuncionarioVendedor.FUN_NOME Vendedor, ");
                        groupBy.Append("FuncionarioVendedor.FUN_NOME, ");

                        SetarJoinsFuncionarioVendedor(joins);
                    }
                    break;

                case "Supervisor":
                    if (!select.Contains(" Supervisor, "))
                    {
                        select.Append("FuncionarioSupervisor.FUN_NOME Supervisor, ");
                        groupBy.Append("FuncionarioSupervisor.FUN_NOME, ");

                        SetarJoinsFuncionarioSupervisor(joins);
                    }
                    break;

                case "NumeroPedidoCliente":
                    if (!select.Contains(" NumeroPedidoCliente, "))
                    {
                        select.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE NumeroPedidoCliente, ");
                        groupBy.Append("Pedido.PED_CODIGO_PEDIDO_CLIENTE, ");
                    }
                    break;

                case "GrossSales":
                    if (!select.Contains(" GrossSales, "))
                    {
                        select.Append("Pedido.PED_GROSS_SALES GrossSales, ");
                        groupBy.Append("Pedido.PED_GROSS_SALES, ");
                    }
                    break;

                case "PossuiIscaFormatada":
                    if (!select.Contains(" PossuiIsca, "))
                    {
                        select.Append("Pedido.PED_POSSUI_ISCA PossuiIsca, ");
                        groupBy.Append("Pedido.PED_POSSUI_ISCA, ");
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select.Append("Pedido.PED_OBSERVACAO Observacao, ");
                        groupBy.Append("Pedido.PED_OBSERVACAO, ");
                    }
                    break;

                case "NumeroOrdem":
                    if (!select.Contains(" NumeroOrdem, "))
                    {
                        select.Append("Pedido.PED_NUMERO_ORDEM NumeroOrdem, ");
                        groupBy.Append("Pedido.PED_NUMERO_ORDEM, ");
                    }
                    break;

                case "PossuiEtiquetagemFormatada":
                    if (!select.Contains(" PossuiEtiquetagem, "))
                    {
                        select.Append("Pedido.PED_POSSUI_ETIQUETAGEM PossuiEtiquetagem, ");
                        groupBy.Append("Pedido.PED_POSSUI_ETIQUETAGEM, ");
                    }
                    break;

                case "DataAlocacaoPedidoFormatada":
                    if (!select.Contains(" DataAlocacaoPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_ALOCACAO_PEDIDO DataAlocacaoPedido, ");
                        groupBy.Append("Pedido.PED_DATA_ALOCACAO_PEDIDO, ");
                    }
                    break;

                case "DataAlocacaoISISFormatada":
                    if (!select.Contains(" DataAlocacaoISIS,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"(
                                    select top 1 _xmlnotafiscal.NF_DATA_HORA_CRIACAO_EMBARCADOR
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
                                     where _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                ) DataAlocacaoISIS, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"(
                                    select top 1 _xmlnotafiscal.NF_DATA_HORA_CRIACAO_EMBARCADOR
                                      from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal 
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO 
                                     where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                ) DataAlocacaoISIS, "
                                );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "ValorNota":
                    if (!select.Contains(" ValorNota,"))
                    {
                        if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        {
                            select.Append(
                                @"(
                                    select sum(_xmlnotafiscal.NF_VALOR) 
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
                                     WHERE _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                ) ValorNota, "
                            );

                            if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                                groupBy.Append("Pedido.PED_CODIGO, ");
                        }
                        else
                        {
                            select.Append(
                                @"(
                                    select sum(_xmlnotafiscal.NF_VALOR) 
                                      from T_PEDIDO_XML_NOTA_FISCAL _cargapedidoxmlnotafiscal 
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _cargapedidoxmlnotafiscal.NFX_CODIGO 
                                     where _cargapedidoxmlnotafiscal.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                ) ValorNota, "
                            );

                            if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                                groupBy.Append("CargaPedido.CPE_CODIGO, ");

                            SetarJoinsCargaPedido(joins);
                        }
                    }
                    break;

                case "OrdemCarregamento":
                    if (!select.Contains(" OrdemCarregamento,"))
                    {
                        select.Append(
                            $@"(
                                select top(1) _carregamentoPedido.CRP_ORDEM
                                  from T_CARREGAMENTO _carregamento
                                  left join T_CARREGAMENTO_PEDIDO _carregamentoPedido on _carregamentoPedido.CRG_CODIGO = _carregamento.CRG_CODIGO
                                 where _carregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO
                                   and _carregamento.CRG_SITUACAO <> {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado}
                            ) OrdemCarregamento, "
                        );

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "Carregamento":
                    if (!select.Contains(" Carregamento,"))
                    {
                        select.Append(
                            $@"(
                                select top(1) _carregamento.CRG_AUTO_SEQUENCIA_NUMERO
                                  from T_CARREGAMENTO _carregamento
                                  left join T_CARREGAMENTO_PEDIDO _carregamentoPedido on _carregamentoPedido.CRG_CODIGO = _carregamento.CRG_CODIGO
                                 where _carregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO
                                   and _carregamento.CRG_SITUACAO <> {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado}
                            ) Carregamento, "
                        );

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "CodIntegracaoDestinatarioPedido":
                    if (!select.Contains(" CodIntegracaoDestinatarioPedido,"))
                    {
                        select.Append("PedidoDestinatario.CLI_CODIGO_INTEGRACAO CodIntegracaoDestinatarioPedido, ");
                        groupBy.Append("PedidoDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "PesoProdutoPedidoFormatada":
                    if (!select.Contains(" PesoProdutoPedido, "))
                    {
                        select.Append("(SELECT SUM(_produtoPedido.PRP_PESO_UNITARIO) FROM T_PEDIDO_PRODUTO _produtoPedido WHERE _produtoPedido.PED_CODIGO = Pedido.PED_CODIGO) PesoProdutoPedido, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "QtdProdutoPedidoFormatada":
                    if (!select.Contains(" QtdProdutoPedido, "))
                    {
                        select.Append("(SELECT SUM(_produtoPedido.PRP_QUANTIDADE) FROM T_PEDIDO_PRODUTO _produtoPedido WHERE _produtoPedido.PED_CODIGO = Pedido.PED_CODIGO) QtdProdutoPedido, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "CodIntegracaoProdutoEmbarcadorPedido":
                    if (!select.Contains(" CodIntegracaoProdutoEmbarcadorPedido, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                SELECT ', ' + _ProdutoEmbarcador.PRO_CODIGO_PRODUTO_EMBARCADOR
			                      FROM T_PEDIDO_PRODUTO _produtoPedido
			                      LEFT JOIN T_PRODUTO_EMBARCADOR _ProdutoEmbarcador ON _ProdutoEmbarcador.PRO_CODIGO = _produtoPedido.PRO_CODIGO
			                     where _produtoPedido.PED_CODIGO = Pedido.PED_CODIGO
			                       FOR XML PATH('')
                            ), 3, 1000) CodIntegracaoProdutoEmbarcadorPedido, "
                        );

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "DescricaoProdutoEmbarcadorPedido":
                    if (!select.Contains(" DescricaoProdutoEmbarcadorPedido, "))
                    {
                        select.Append(
                            @"SUBSTRING((
                                 SELECT ', ' + _ProdutoEmbarcador.GRP_DESCRICAO
			                       FROM T_PEDIDO_PRODUTO _produtoPedido
			                       LEFT JOIN T_PRODUTO_EMBARCADOR _ProdutoEmbarcador ON _ProdutoEmbarcador.PRO_CODIGO = _produtoPedido.PRO_CODIGO
			                      where _produtoPedido.PED_CODIGO = Pedido.PED_CODIGO
			                        FOR XML PATH('')
                            ), 3, 1000) DescricaoProdutoEmbarcadorPedido, "
                        );

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "CodIntegracaoTipoCargaPedido":
                    if (!select.Contains(" CodIntegracaoTipoCargaPedido,"))
                    {
                        select.Append("CTipoCarga.TCG_CODIGO_TIPO_CARGA_EMBARCADOR CodIntegracaoTipoCargaPedido, ");
                        groupBy.Append("CTipoCarga.TCG_CODIGO_TIPO_CARGA_EMBARCADOR, ");

                        SetarJoinsTipoCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "ValorFreteNegociado":
                    if (!select.Contains(" ValorFreteNegociado, "))
                    {
                        select.Append("Pedido.PED_VALOR_FRETE_NEGOCIADO ValorFreteNegociado, ");
                        groupBy.Append("Pedido.PED_VALOR_FRETE_NEGOCIADO, ");
                    }
                    break;

                case "ValorFreteTransportadorTerceiro":
                    if (!select.Contains(" ValorFreteTransportadorTerceiro, "))
                    {
                        select.Append("Pedido.PED_VALOR_FRETE_TRANSPORTADOR_TERCEIRO ValorFreteTransportadorTerceiro, ");
                        groupBy.Append("Pedido.PED_VALOR_FRETE_TRANSPORTADOR_TERCEIRO, ");
                    }
                    break;

                case "ValorFreteToneladaTerceiro":
                    if (!select.Contains(" ValorFreteToneladaTerceiro, "))
                    {
                        select.Append("Pedido.PED_VALOR_FRETE_TONELADA_TERCEIRO ValorFreteToneladaTerceiro, ");
                        groupBy.Append("Pedido.PED_VALOR_FRETE_TONELADA_TERCEIRO, ");
                    }
                    break;

                case "ValorFreteToneladaNegociado":
                    if (!select.Contains(" ValorFreteToneladaNegociado, "))
                    {
                        select.Append("Pedido.PED_VALOR_FRETE_TONELADA_NEGOCIADO ValorFreteToneladaNegociado, ");
                        groupBy.Append("Pedido.PED_VALOR_FRETE_TONELADA_NEGOCIADO, ");
                    }
                    break;

                case "ValorPedagioRota":
                    if (!select.Contains(" ValorPedagioRota, "))
                    {
                        select.Append("Pedido.PED_VALOR_PEDAGIO_ROTA ValorPedagioRota, ");
                        groupBy.Append("Pedido.PED_VALOR_PEDAGIO_ROTA, ");
                    }
                    break;

                case "ValorTotalPedido":
                    if (!select.Contains(" ValorTotalPedido, "))
                    {
                        select.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS ValorTotalPedido, ");
                        groupBy.Append("Pedido.PED_VALOR_TOTAL_NOTAS_FISCAIS, ");
                    }
                    break;

                case "DataSalvamentoDadosTransporte":
                case "      DataSalvamentoDadosTransporteFormatada":
                    if (!select.Contains("DataSalvamentoDadosTransporte, "))
                    {
                        select.Append("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE DataSalvamentoDadosTransporte, ");
                        groupBy.Append("Carga.CAR_DATA_SALVAMENTO_DADOS_TRANSPORTE, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataConfirmacaoEnvioDocumentos":
                case "DataConfirmacaoEnvioDocumentosFormatada":
                    if (!select.Contains("DataConfirmacaoEnvioDocumentos, "))
                    {
                        select.Append("Carga.CAR_DATA_CONFIRMACAO_DOCUMENTOS_FISCAIS DataConfirmacaoEnvioDocumentos, ");
                        groupBy.Append("Carga.CAR_DATA_CONFIRMACAO_DOCUMENTOS_FISCAIS, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataConfirmacaoValorFrete":
                case "DataConfirmacaoValorFreteFormatada":
                    if (!select.Contains("DataConfirmacaoValorFrete, "))
                    {
                        select.Append("Carga.CAR_DATA_CONFIRMACAO_VALOR_FRETE DataConfirmacaoValorFrete, ");
                        groupBy.Append("Carga.CAR_DATA_CONFIRMACAO_VALOR_FRETE, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "DataEnvioCTeOcorrencia":
                case "DataEnvioCTeOcorrenciaFormatada":
                    if (!select.Contains("DataEnvioCTeOcorrencia, "))
                    {
                        select.Append("OcorrenciaCTeIntegracao.INT_DATA_INTEGRACAO DataEnvioCTeOcorrencia, ");
                        groupBy.Append("OcorrenciaCTeIntegracao.INT_DATA_INTEGRACAO, ");

                        SetarJoinsOcorrenciaCTeIntegracao(joins);
                    }
                    break;

                case "PrevisaoEntregaReprogramada":
                case "PrevisaoEntregaReprogramadaFormatada":
                    if (!select.Contains("PrevisaoEntregaReprogramada, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA PrevisaoEntregaReprogramada, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA_REPROGRAMADA, ");

                        SetarJoinsCargaEntrega(joins, filtrosPesquisa);
                    }
                    break;

                case "DataEntradaNoRaio":
                    if (!select.Contains("DataEntradaNoRaio, "))
                    {
                        select.Append("CargaEntrega.CEN_DATA_ENTREGA DataEntradaNoRaio, ");
                        groupBy.Append("CargaEntrega.CEN_DATA_ENTREGA, ");

                        SetarJoinsCargaEntrega(joins, filtrosPesquisa);
                    }
                    break;

                case "IdAgrupador":
                case "IdAgrupadorDescricao":
                    if (!select.Contains("IdAgrupador, "))
                    {
                        select.Append("CargaPreAgrupamentoAgrupador.PAA_CODIGO_AGRUPAMENTO IdAgrupador, ");
                        groupBy.Append("CargaPreAgrupamentoAgrupador.PAA_CODIGO_AGRUPAMENTO, ");

                        SetarJoinsCargaPreAgrupamentoAgrupador(joins, filtrosPesquisa);
                    }
                    break;

                case "CodigoSAP":
                    if (!select.Contains("CodigoSAP, "))
                    {
                        select.Append("PedidoDestinatario.CLI_CODIGO_SAP CodigoSAP, ");
                        groupBy.Append("PedidoDestinatario.CLI_CODIGO_SAP, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "CodigoArmador":
                    if (!select.Contains("CodigoArmador, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_CODIGO_ARMADOR CodigoArmador, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_CODIGO_ARMADOR, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "DataDeadLineCarga":
                case "DataDeadLineCargaFormatada":
                    if (!select.Contains("DataDeadLineCarga, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_DATA_DEAD_LINE_CARGA DataDeadLineCarga, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_DATA_DEAD_LINE_CARGA, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "DataDeadLineDraf":
                case "DataDeadLineDrafFormatada":
                    if (!select.Contains("DataDeadLineDraf, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_DATA_DEAD_LINE_DRAF DataDeadLineDraf, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_DATA_DEAD_LINE_DRAF, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "DataEstufagem":
                case "DataEstufagemFormatada":
                case "SemanaEstufagem":
                    if (!select.Contains(" DataEstufagem, "))
                    {
                        select.Append("Pedido.PED_DATA_ESTUFAGEM DataEstufagem, ");
                        groupBy.Append("Pedido.PED_DATA_ESTUFAGEM, ");
                    }
                    break;

                case "DataETATransbordo":
                case "DataETATransbordoFormatada":
                    if (!select.Contains("DataETATransbordo, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_DATA_ETA_TRANSBORDO DataETATransbordo, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_DATA_ETA_TRANSBORDO, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "DataDeCriacaoPedidoERP":
                case "DataDeCriacaoPedidoERPFormatada":
                    if (!select.Contains("DataDeCriacaoPedidoERP, "))
                    {
                        select.Append("Pedido.PED_DATA_DE_CRIACAO_PEDIDO_ERP DataDeCriacaoPedidoERP, ");
                        groupBy.Append("Pedido.PED_DATA_DE_CRIACAO_PEDIDO_ERP, ");

                    }
                    break;

                case "DataETS":
                case "DataETSFormatada":
                    if (!select.Contains("DataETS, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_DATA_ETS DataETS, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_DATA_ETS, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "Despachante":
                    if (!select.Contains(" CodigoDespachante, "))
                    {
                        select.Append("Pedido.PED_DESPACHANTE_CODIGO CodigoDespachante, Pedido.PED_DESPACHANTE_DESCRICAO DescricaoDespachante, ");
                        groupBy.Append("Pedido.PED_DESPACHANTE_CODIGO, Pedido.PED_DESPACHANTE_DESCRICAO, ");
                    }
                    break;

                case "Incoterm":
                    if (!select.Contains("Incoterm, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_INCOTERM Incoterm, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_INCOTERM, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "InLand":
                    if (!select.Contains(" CodigoInLand, "))
                    {
                        select.Append("Pedido.PED_INLAND_CODIGO CodigoInLand, Pedido.PED_INLAND_DESCRICAO DescricaoInLand, ");
                        groupBy.Append("Pedido.PED_INLAND_CODIGO, Pedido.PED_INLAND_DESCRICAO, ");
                    }
                    break;

                case "NomeNavio":
                    if (!select.Contains("NomeNavio, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_NOME_NAVIO NomeNavio, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_NOME_NAVIO, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "NomeNavioTransbordo":
                    if (!select.Contains("NomeNavioTransbordo, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_NOME_NAVIO_TRANSBORDO NomeNavioTransbordo, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_NOME_NAVIO_TRANSBORDO, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "NumeroContainer":
                    if (!select.Contains("NumeroContainer, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_NUMERO_CONTAINER NumeroContainer, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_NUMERO_CONTAINER, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "NumeroEXP":
                    if (!select.Contains("NumeroEXP, "))
                    {
                        select.Append("Pedido.PED_NUMERO_EXP NumeroEXP, ");
                        groupBy.Append("Pedido.PED_NUMERO_EXP, ");
                    }
                    break;

                case "NumeroLacre":
                    if (!select.Contains("NumeroLacre, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_NUMERO_LACRE NumeroLacre, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_NUMERO_LACRE, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "OrdemEmbarque":
                    if (!select.Contains("OrdemEmbarque, "))
                    {
                        select.Append(
                            @"substring((
                                 select ', ' + _ordemEmbarque.OEM_NUMERO
			                       from T_ORDEM_EMBARQUE _ordemEmbarque
			                      where _ordemEmbarque.CAR_CODIGO = Carga.CAR_CODIGO
			                        for xml path('')
                            ), 3, 1000) OrdemEmbarque, "
                        ); ;

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "PagamentoMaritimo":
                case "PagamentoMaritimoDescricao":
                    if (!select.Contains("PagamentoMaritimo, "))
                    {
                        select.Append("Pedido.PED_PAGAMENTO_MARITIMO PagamentoMaritimo, ");
                        groupBy.Append("Pedido.PED_PAGAMENTO_MARITIMO, ");
                    }
                    break;

                case "PaisPortoViagemDestino":
                    if (!select.Contains(" PaisPortoDestino, "))
                    {
                        select.Append("Pedido.PED_PORTO_DESTINO_PAIS PaisPortoDestino, Pedido.PED_PORTO_DESTINO_SIGLA_PAIS SiglaPaisPortoDestino, ");
                        groupBy.Append("Pedido.PED_PORTO_DESTINO_PAIS, Pedido.PED_PORTO_DESTINO_SIGLA_PAIS, ");
                    }
                    break;

                case "PaisPortoViagemOrigem":
                    if (!select.Contains(" PaisPortoOrigem, "))
                    {
                        select.Append("Pedido.PED_PORTO_ORIGEM_PAIS PaisPortoOrigem, Pedido.PED_PORTO_ORIGEM_SIGLA_PAIS SiglaPaisPortoOrigem, ");
                        groupBy.Append("Pedido.PED_PORTO_ORIGEM_PAIS, Pedido.PED_PORTO_ORIGEM_SIGLA_PAIS, ");
                    }
                    break;

                case "PortoViagemDestino":
                    if (!select.Contains(" CodigoPortoDestino, "))
                    {
                        select.Append("Pedido.PED_PORTO_DESTINO_CODIGO CodigoPortoDestino, Pedido.PED_PORTO_DESTINO_DESCRICAO DescricaoPortoDestino, ");
                        groupBy.Append("Pedido.PED_PORTO_DESTINO_CODIGO, Pedido.PED_PORTO_DESTINO_DESCRICAO, ");
                    }
                    break;

                case "PortoViagemOrigem":
                    if (!select.Contains(" CodigoPortoOrigem, "))
                    {
                        select.Append("Pedido.PED_PORTO_ORIGEM_CODIGO CodigoPortoOrigem, Pedido.PED_PORTO_ORIGEM_DESCRICAO DescricaoPortoOrigem, ");
                        groupBy.Append("Pedido.PED_PORTO_ORIGEM_CODIGO, Pedido.PED_PORTO_ORIGEM_DESCRICAO, ");
                    }
                    break;

                case "PossuiGenset":
                case "PossuiGensetDescricao":
                    if (!select.Contains("PossuiGenset, "))
                    {
                        select.Append("Pedido.PED_POSSUI_GENSET PossuiGenset, ");
                        groupBy.Append("Pedido.PED_POSSUI_GENSET, ");
                    }
                    break;

                case "TerminalOrigem":
                    if (!select.Contains("TerminalOrigem, "))
                    {
                        select.Append("DadosTransporteMaritimo.CTM_TERMINAL_ORIGEM TerminalOrigem, ");
                        groupBy.Append("DadosTransporteMaritimo.CTM_TERMINAL_ORIGEM, ");

                        SetarJoinsDadosTransporteMaritimo(joins);
                    }
                    break;

                case "TipoContainer":
                    if (!select.Contains("TipoContainer, "))
                    {
                        select.Append("ModeloVeicular.MVC_DESCRICAO TipoContainer, ");
                        groupBy.Append("ModeloVeicular.MVC_DESCRICAO, ");

                        SetarJoinsModeloveicularCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "ViaTransporte":
                    if (!select.Contains("ViaTransporte, "))
                    {
                        select.Append("ViaTransporte.TVT_DESCRICAO ViaTransporte, ");
                        groupBy.Append("ViaTransporte.TVT_DESCRICAO, ");

                        SetarJoinsViaTransporte(joins);
                    }
                    break;

                case "ExpedidorNomeFantasia":
                    if (!select.Contains("ExpedidorNomeFantasia, "))
                    {
                        select.Append("CPExpedidor.CLI_NOMEFANTASIA ExpedidorNomeFantasia, ");
                        groupBy.Append("CPExpedidor.CLI_NOMEFANTASIA, ");

                        SetarJoinsExpedidor(joins, filtrosPesquisa);
                    }
                    break;

                case "CPFMotoristasFormatado":
                    if (!select.Contains(" CPFMotoristas, "))
                    {
                        select.Append(@"SUBSTRING((
			                                    SELECT ISNULL(PedMotorista.PedMotoristaCPF, '') +', ' + Motorista.FUN_CPF
			                                    FROM T_CARGA_MOTORISTA CargaMotorista
			                                    INNER JOIN T_FUNCIONARIO Motorista ON CargaMotorista.CAR_MOTORISTA = Motorista.FUN_CODIGO
			                                    OUTER APPLY (
					                                    SELECT ', ' + Motorista.FUN_CPF AS PedMotoristaCPF
					                                    FROM T_PEDIDO_MOTORISTA pedMotorista
					                                    INNER JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = pedMotorista.FUN_CODIGO
                                                AND Motorista.FUN_CODIGO != CargaMotorista.CAR_MOTORISTA					                                    
                                                WHERE pedMotorista.PED_CODIGO = Pedido.PED_CODIGO
				                                    ) AS PedMotorista
			                                    WHERE CargaMotorista.CAR_CODIGO = Carga.CAR_CODIGO
			                                    FOR XML PATH('')
			                                    ), 3, 1000) CPFMotoristas, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO,"))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "ViagemNavio":
                    if (!select.Contains(" ViagemNavio,"))
                    {
                        select.Append("PedidoViagemNavio.PVN_DESCRICAO ViagemNavio, ");
                        groupBy.Append("PedidoViagemNavio.PVN_DESCRICAO, ");

                        SetarJoinsPedidoViagemNavio(joins);
                    }
                    break;

                case "PortoDestino":
                    if (!select.Contains("PortoDestino, "))
                    {
                        select.Append("PortoDestino.POT_DESCRICAO PortoDestino, ");
                        groupBy.Append("PortoDestino.POT_DESCRICAO, ");

                        SetarJoinsPortoDestino(joins);
                    }
                    break;

                case "PortoOrigem":
                    if (!select.Contains("PortoOrigem, "))
                    {
                        select.Append("PortoOrigem.POT_DESCRICAO PortoOrigem, ");
                        groupBy.Append("PortoOrigem.POT_DESCRICAO, ");

                        SetarJoinsPortoOrigem(joins);
                    }
                    break;

                case "DataETAPortoOrigemFormatada":
                    if (!select.Contains("DataETAPortoOrigem, "))
                    {
                        select.Append($@"(
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO 
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM) DataETAPortoOrigem, "
                        );

                        if (!groupBy.Contains("Pedido.PVN_CODIGO, "))
                            groupBy.Append("Pedido.PVN_CODIGO, ");

                        if (!groupBy.Contains("Pedido.POT_CODIGO_ORIGEM, "))
                            groupBy.Append("Pedido.POT_CODIGO_ORIGEM, ");

                        if (!groupBy.Contains("Pedido.TTI_CODIGO_ORIGEM, "))
                            groupBy.Append("Pedido.TTI_CODIGO_ORIGEM, ");
                    }
                    break;

                case "DataETSPortoOrigemFormatada":
                    if (!select.Contains("DataETSPortoOrigem, "))
                    {
                        select.Append($@"(
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM) DataETSPortoOrigem, "
                        );

                        if (!groupBy.Contains("Pedido.PVN_CODIGO, "))
                            groupBy.Append("Pedido.PVN_CODIGO, ");

                        if (!groupBy.Contains("Pedido.POT_CODIGO_ORIGEM, "))
                            groupBy.Append("Pedido.POT_CODIGO_ORIGEM, ");

                        if (!groupBy.Contains("Pedido.TTI_CODIGO_ORIGEM, "))
                            groupBy.Append("Pedido.TTI_CODIGO_ORIGEM, ");
                    }
                    break;

                case "DataETAPortoDestinoFormatada":
                    if (!select.Contains("DataETAPortoDestino, "))
                    {
                        select.Append($@"(
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO) DataETAPortoDestino, "
                        );

                        if (!groupBy.Contains("Pedido.PVN_CODIGO, "))
                            groupBy.Append("Pedido.PVN_CODIGO, ");

                        if (!groupBy.Contains("Pedido.POT_CODIGO_DESTINO, "))
                            groupBy.Append("Pedido.POT_CODIGO_DESTINO, ");

                        if (!groupBy.Contains("Pedido.TTI_CODIGO_DESTINO, "))
                            groupBy.Append("Pedido.TTI_CODIGO_DESTINO, ");
                    }
                    break;

                case "DataETSPortoDestinoFormatada":
                    if (!select.Contains("DataETSPortoDestino, "))
                    {
                        select.Append($@"(
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO) DataETSPortoDestino, "
                        );

                        if (!groupBy.Contains("Pedido.PVN_CODIGO, "))
                            groupBy.Append("Pedido.PVN_CODIGO, ");

                        if (!groupBy.Contains("Pedido.POT_CODIGO_DESTINO, "))
                            groupBy.Append("Pedido.POT_CODIGO_DESTINO, ");

                        if (!groupBy.Contains("Pedido.TTI_CODIGO_DESTINO, "))
                            groupBy.Append("Pedido.TTI_CODIGO_DESTINO, ");
                    }
                    break;


                case "DataInclusaoPedidoFormatada":
                    if (!select.Contains("DataInclusaoPedido, "))
                    {
                        select.Append("Pedido.PED_DATA_CRIACAO DataInclusaoPedido, ");
                        groupBy.Append("Pedido.PED_DATA_CRIACAO, ");
                    }
                    break;

                case "TipoServicoPedido":
                    if (!select.Contains("TipoServicoPedido, "))
                    {
                        select.Append("PedidoAdicional.PAD_TIPO_SERVICO  TipoServicoPedido, ");
                        groupBy.Append("PedidoAdicional.PAD_TIPO_SERVICO , ");
                        SetarJoinsPedidoAdicional(joins);
                    }
                    break;

                case "TipoPropostaMultiModalDescricao":
                    if (!select.Contains("TipoPropostaMultiModal, "))
                    {
                        select.Append("CTipoOperacao.TOP_TIPO_PROPOSTA_MULTIMODAL TipoPropostaMultiModal, ");
                        groupBy.Append("CTipoOperacao.TOP_TIPO_PROPOSTA_MULTIMODAL, ");

                        SetarJoinsTipoOperacao(joins, filtrosPesquisa);
                    }
                    break;

                case "TomadorOutros":
                    if (!select.Contains(" TomadorOutros,"))
                    {
                        select.Append("Tomador.CLI_NOME TomadorOutros, ");
                        groupBy.Append("Tomador.CLI_NOME, ");

                        SetarJoinsTomador(joins, filtrosPesquisa);
                    }
                    break;

                case "CNPJTomadorOutrosFormatado":
                    if (!select.Contains(" CNPJTomadorOutros,"))
                    {
                        select.Append("Tomador.CLI_CGCCPF CNPJTomadorOutros, Tomador.CLI_FISJUR TipoTomadorOutros, ");
                        groupBy.Append("Tomador.CLI_CGCCPF, Tomador.CLI_FISJUR, ");

                        SetarJoinsTomador(joins, filtrosPesquisa);
                    }
                    break;

                case "NumeroRedespachoFormatado":
                    if (!select.Contains(" NumeroRedespacho, "))
                    {
                        select.Append("CAST(Redespacho.RED_NUMERO AS VARCHAR(20)) NumeroRedespacho, ");
                        groupBy.Append("Redespacho.RED_NUMERO, ");

                        SetarJoinsRedespacho(joins, filtrosPesquisa);
                    }
                    break;

                case "OperadorPedido":
                    if (!select.Contains(" OperadorPedido, "))
                    {
                        select.Append("FuncionarioAutor.FUN_NOME OperadorPedido, ");
                        groupBy.Append("FuncionarioAutor.FUN_NOME, ");

                        SetarJoinsFuncionarioAutor(joins);
                    }
                    break;
                case "EmailSolicitante":
                    if (!select.Contains(" EmailSolicitante,"))
                    {
                        select.Append("EmailSolicitante.ACO_SOLICITANTE EmailSolicitante, ");
                        groupBy.Append("EmailSolicitante.ACO_SOLICITANTE, ");

                        SetarJoinsEmailSolicitante(joins, filtrosPesquisa);

                    }
                    break;

                case "CodIntegracaoRemetente":
                    if (!select.Contains(" CodIntegracaoRemetente,"))
                    {
                        select.Append("PedidoRemetente.CLI_CODIGO_INTEGRACAO CodIntegracaoRemetente, ");
                        groupBy.Append("PedidoRemetente.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsRemetente(joins);
                    }
                    break;

                case "CodIntegracaoDestinatario":
                    if (!select.Contains(" CodIntegracaoDestinatario,"))
                    {
                        select.Append("PedidoDestinatario.CLI_CODIGO_INTEGRACAO CodIntegracaoDestinatario, ");
                        groupBy.Append("PedidoDestinatario.CLI_CODIGO_INTEGRACAO, ");

                        SetarJoinsDestinatario(joins);
                    }
                    break;

                case "NotasFiscaisPedido":
                    if (!select.Contains(" NotasFiscaisPedido, "))
                    {
                        select.Append(
                                @"SUBSTRING((
		                            SELECT ', ' + convert(nvarchar(50), _xmlnotafiscal.NF_NUMERO) 
		                                FROM T_PEDIDO_NOTAS_FISCAIS _notasFiscais
                                            JOIN T_XML_NOTA_FISCAL _xmlNotaFiscal ON _xmlNotaFiscal.NFX_CODIGO = _notasFiscais.NFX_CODIGO 
		                                WHERE _notasFiscais.PED_CODIGO = Pedido.PED_CODIGO
                                       FOR XML PATH('')
                                ), 3, 1000) NotasFiscaisPedido, "
                            );

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                default:
                    if (!somenteContarNumeroRegistros && propriedade.Contains("ValorComponente"))
                    {
                        SetarJoinsCargaPedido(joins);

                        select.Append($"(SELECT SUM(CCF_VALOR_COMPONENTE) FROM T_CARGA_PEDIDO_COMPONENTES_FRETE Componente WHERE Componente.CPE_CODIGO = CargaPedido.CPE_CODIGO AND Componente.CFR_CODIGO = {codigoDinamico}) {propriedade}, "); 

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO, "))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        //select.Append($"SUM({propriedade}.CCF_VALOR_COMPONENTE) {propriedade}, ");
                        //joins.Append($" left join T_CARGA_PEDIDO_COMPONENTES_FRETE {propriedade} on CargaPedido.CPE_CODIGO = {propriedade}.CPE_CODIGO and {propriedade}.CFR_CODIGO = {codigoDinamico} ");
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        select.Append("CentroResultado.CRE_DESCRICAO CentroResultado, ");
                        groupBy.Append("CentroResultado.CRE_DESCRICAO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "DataInicioJanelaFormatada":
                    if (!select.Contains(" DataInicioJanela, "))
                    {
                        select.Append("Pedido.PED_DATA_INICIO_JANELA_DESCARGA DataInicioJanela, ");
                        groupBy.Append("Pedido.PED_DATA_INICIO_JANELA_DESCARGA, ");

                        SetarJoinsCargaPedido(joins);
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        select.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR CodigoIntegracao, ");
                        groupBy.Append("Filial.FIL_CODIGO_FILIAL_EMBARCADOR, ");

                        SetarJoinsFilial(joins, filtrosPesquisa);
                    }
                    break;

                case "NaturezaOP":
                    if (!select.Contains(" NaturezaOP, "))
                    {
                        select.Append(
                                @"(
                                    select top 1 _xmlnotafiscal.NF_NATUREZA_OP
                                      from VIEW_PEDIDO_XML _pedidoXml
                                      join T_XML_NOTA_FISCAL _xmlnotafiscal on _xmlnotafiscal.NFX_CODIGO = _pedidoXml.NFX_CODIGO
		                             where _pedidoXml.PED_CODIGO = Pedido.PED_CODIGO
                                ) NaturezaOP, "
                            );

                        //groupBy.Append("_pedidoXml.NFX_CODIGO, ");

                        SetarJoinsCentroResultado(joins);
                    }
                    break;

                case "NumeroCarregamento":
                    if (!select.Contains(" NumeroCarregamento, "))
                    {
                        select.Append("Pedido.PED_NUMERO_CARREGAMENTO NumeroCarregamento, ");
                        groupBy.Append("Pedido.PED_NUMERO_CARREGAMENTO, ");
                    }
                    break;

                case "SubstituicaoDescricao":
                    if (!select.Contains(" Substituicao, "))
                    {
                        select.Append("Pedido.PED_SUBSTITUICAO Substituicao, ");
                        groupBy.Append("Pedido.PED_SUBSTITUICAO, ");
                    }
                    break;

                case "AguardandoIntegracaoDescricao":
                    if (!select.Contains(" AguardandoIntegracao, "))
                    {
                        select.Append("Pedido.PED_AG_INTEGRACAO AguardandoIntegracao, ");
                        groupBy.Append("Pedido.PED_AG_INTEGRACAO, ");

                    }
                    break;

                case "QtdPalletsCarregado":
                    if (!select.Contains(" QtdPalletsCarregado, "))
                    {
                        select.Append("(SELECT SUM(isnull(CP.CRP_PALLET, 0)) FROM T_CARREGAMENTO_PEDIDO CP WHERE CP.PED_CODIGO = Pedido.PED_CODIGO AND Carga.CRG_CODIGO= CP.CRG_CODIGO) QtdPalletsCarregado, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");

                        if (!groupBy.Contains("Carga.CRG_CODIGO, "))
                            groupBy.Append("Carga.CRG_CODIGO, ");
                    }
                    break;

                case "CargaPossuiAnexosFormatada":
                    if (!select.Contains(" CargaPossuiAnexos, "))
                    {
                        select.Append("(select count(1)  from T_CARGA_NFE_ANEXO CargaNFeAnexo where CargaNFeAnexo.CAR_CODIGO = Carga.CAR_CODIGO) CargaPossuiAnexos, ");
                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;
                case "LinhaSeparacao":
                    if (!select.Contains(" LinhaSeparacao, "))
                    {
                        select.Append(@"(SELECT SUBSTRING((
                              SELECT ', ' + CONVERT(NVARCHAR(50), T_LINHA_SEPARACAO.CLS_DESCRICAO)
                              FROM T_LINHA_SEPARACAO
                              INNER JOIN T_PEDIDO_PRODUTO ON T_LINHA_SEPARACAO.CLS_CODIGO = T_PEDIDO_PRODUTO.CLS_CODIGO
                              WHERE T_PEDIDO_PRODUTO.PED_CODIGO = Pedido.PED_CODIGO
                              FOR XML PATH('')), 3, 1000)) AS LinhaSeparacao, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO, "))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "QuantidadePacotes":
                    if (!select.Contains(" QuantidadePacotes, "))
                    {
                        select.Append("( ");
                        select.Append("    select COUNT(1) from T_CARGA_PEDIDO_PACOTE CargaPedidoPacote ");
                        select.Append("    inner join T_CARGA_PEDIDO CargaPedido on CargaPedido.CPE_CODIGO = CargaPedidoPacote.CPE_CODIGO ");
                        select.Append("    where CargaPedido.PED_CODIGO = Pedido.PED_CODIGO ");
                        select.Append(") QuantidadePacotes, ");

                        if (!groupBy.Contains("Pedido.PED_CODIGO,"))
                            groupBy.Append("Pedido.PED_CODIGO, ");
                    }
                    break;

                case "QuantidadePacotesColetados":
                    if (!select.Contains(" QuantidadePacotesColetados, "))
                    {
                        select.Append("( ");
                        select.Append("    select Sum(CargaEntrega.CEN_QUANTIDADE_PACOTES_COLETADOS) from T_CARGA_ENTREGA CargaEntrega ");
                        select.Append("    inner join T_CARGA_ENTREGA_PEDIDO CargaEntregaPedido on CargaEntregaPedido.CEN_CODIGO = CargaEntrega.CEN_CODIGO");
                        select.Append("     and CargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO ");
                        select.Append("    where CargaEntrega.CAR_CODIGO = Carga.CAR_CODIGO ");
                        select.Append("     and CargaEntrega.CEN_COLETA = 1 ");
                        select.Append(") QuantidadePacotesColetados, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO,"))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("CargaPedido.CPE_CODIGO,"))
                            groupBy.Append("CargaPedido.CPE_CODIGO, ");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                    break;

                case "KmRota":
                    if (!select.Contains(" KmRota, "))
                    {
                        select.Append("RotaFrete.ROF_QUILOMETROS AS KmRota, ");

                        if (!groupBy.Contains("RotaFrete.ROF_QUILOMETROS, "))
                            groupBy.Append("RotaFrete.ROF_QUILOMETROS, ");

                        SetarJoinsRotaFrete(joins);
                    }
                    break;

                case "Autorizado":
                    if (!select.Contains(" Autorizado, "))
                    {
                        select.Append("CASE WHEN PedidoAutorizacao.Autorizador IS NOT NULL THEN 'Sim' ELSE 'Não' END as Autorizado, ");
                        SetarJoinsPedidoAutorizacao(joins);
                    }
                    break;

                case "Autorizador":
                    if (!select.Contains(" Autorizador, "))
                    {
                        select.Append("PedidoAutorizacao.Autorizador, ");
                        SetarJoinsPedidoAutorizacao(joins);
                        if (!groupBy.Contains("PedidoAutorizacao.Autorizador, "))
                            groupBy.Append("PedidoAutorizacao.Autorizador, ");
                    }
                    break;

                case "MotivoAutorizacaoPedido":
                    if (!select.Contains(" MotivoAutorizacaoPedido, "))
                    {
                        select.Append("PedidoAutorizacao.MotivoAutorizacaoPedido, ");
                        SetarJoinsPedidoAutorizacao(joins);
                        if (!groupBy.Contains("PedidoAutorizacao.MotivoAutorizacaoPedido, "))
                            groupBy.Append("PedidoAutorizacao.MotivoAutorizacaoPedido, ");
                    }
                    break;

                case "MotivoPedido":
                    if (!select.Contains(" MotivoPedido, "))
                    {
                        select.Append("MotivoPedido.PM_DESCRICAO as MotivoPedido, ");
                        SetarJoinsPedidoMotivo(joins);
                        if (!groupBy.Contains("MotivoPedido.PM_DESCRICAO, "))
                            groupBy.Append("MotivoPedido.PM_DESCRICAO, ");
                    }
                    break;

                case "UsuarioCancelamento":
                    if (!select.Contains(" UsuarioCancelamento, "))
                    {
                        select.Append("UsuarioCancelamento.FUN_NOME as UsuarioCancelamento, ");
                        if (!groupBy.Contains("UsuarioCancelamento.FUN_NOME, "))
                            groupBy.Append("UsuarioCancelamento.FUN_NOME, ");
                        SetarJoinsUsuarioCancelamento(joins);
                    }
                    break;
                case "MotivoCancelamento":
                    if (!select.Contains(" MotivoCancelamento, "))
                    {
                        select.Append("coalesce(MotivoCancelamento.MCP_DESCRICAO, Pedido.PED_MOTIVO_CANCELAMENTO) as MotivoCancelamento, ");

                        if (!groupBy.Contains("MotivoCancelamento.MCP_DESCRICAO, "))
                            groupBy.Append("MotivoCancelamento.MCP_DESCRICAO, ");

                        if (!groupBy.Contains("Pedido.PED_MOTIVO_CANCELAMENTO, "))
                            groupBy.Append("Pedido.PED_MOTIVO_CANCELAMENTO, ");

                        SetarJoinsMotivoCancelamento(joins);
                    }
                    break;
                case "DataEntregaOcorrenciaPedidoFormatada":
                    if (!select.Contains(" DataEntregaOcorrenciaPedidoFormatada, "))
                    {
                        select.Append("OcorrenciaColetaEntrega.POC_DATA_OCORRENCIA as DataEntregaOcorrenciaPedido, ");

                        SetarJoinsPedidoOcorrenciaEntrega(joins);

                        if (!groupBy.Contains("OcorrenciaColetaEntrega.POC_DATA_OCORRENCIA, "))
                            groupBy.Append("OcorrenciaColetaEntrega.POC_DATA_OCORRENCIA, ");
                    }
                    break;
                case "Protocolo":
                    if (!select.Contains(" Protocolo, "))
                    {
                        select.Append("Pedido.PED_PROTOCOLO as Protocolo, ");

                        if (!groupBy.Contains("Pedido.PED_PROTOCOLO, "))
                            groupBy.Append("Pedido.PED_PROTOCOLO, ");
                    }
                    break;

                case "CentroDeCustoViagemCodigo":
                case "CentroDeCustoViagemDescricao":
                    if (!select.Contains("CentroDeCustoViagemCodigo, "))
                    {
                        select.Append("Pedido.CCV_CODIGO CentroDeCustoViagemCodigo, ");
                        select.Append("CentroDeCustoViagem.CCV_DESCRICAO CentroDeCustoViagemDescricao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Pedido.CCV_CODIGO, "))
                            groupBy.Append("Pedido.CCV_CODIGO, ");

                        if (!groupBy.Contains("CentroDeCustoViagem.CCV_DESCRICAO, "))
                            groupBy.Append("CentroDeCustoViagem.CCV_DESCRICAO, ");

                        SetarJoinCentroDeCustoViagem(joins, filtrosPesquisa);
                    }
                    break;

                case "DescricaoPedidoBloqueado":
                case "PedidoBloqueado":
                    if (!select.Contains(" PedidoBloqueado, "))
                    {
                        select.Append("Pedido.PED_BLOQUEADO as PedidoBloqueado, ");

                        if (!groupBy.Contains("Pedido.PED_BLOQUEADO, "))
                            groupBy.Append("Pedido.PED_BLOQUEADO, ");
                    }
                    break;
                case "NovaDataAgendamento":
                    if (!select.Contains(" NovaDataAgendamento, "))
                    {
                        select.Append("(case when Pedido.CAR_DATA_CARREGAMENTO_PEDIDO is null then '' else convert(varchar(10), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 103) + ' ' + convert(varchar(5), Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, 108) end) NovaDataAgendamento, ");
                        groupBy.Append("Pedido.CAR_DATA_CARREGAMENTO_PEDIDO, ");
                    }
                    break;

                case "DataVinculoTracao":
                case "DataVinculoTracaoFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoTracao,"))
                    {
                        select.Append(" (select max(HistoricoVinculoTracao.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoTracao " +
                                        " where HistoricoVinculoTracao.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HistoricoVinculoTracao.VEI_CODIGO_TRACAO = Carga.CAR_VEICULO) DataVinculoTracao, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");

                        if (!groupBy.Contains("Carga.CAR_VEICULO, "))
                            groupBy.Append("Carga.CAR_VEICULO, ");
                    }
                    break;

                case "DataVinculoReboque":
                case "DataVinculoReboqueFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoReboque,"))
                    {
                        select.Append(" (select max(HistoricoVinculoReboque.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoReboque " +
                                        " inner join T_HISTORICO_VINCULO_REBOQUES HVReboques on HVReboques.THV_CODIGO = HistoricoVinculoReboque.THV_CODIGO " +
                                        " where HistoricoVinculoReboque.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HVReboques.VEI_CODIGO in (select CReboques.VEI_CODIGO from T_CARGA_VEICULOS_VINCULADOS CReboques where CReboques.CAR_CODIGO = Carga.CAR_CODIGO) " +
                                       ") DataVinculoReboque, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "DataVinculoMotorista":
                case "DataVinculoMotoristaFormatada":
                    if (!somenteContarNumeroRegistros && !select.Contains(" DataVinculoMotorista,"))
                    {
                        select.Append(" (select max(HistoricoVinculoMotorista.THV_DATA_HORA_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculoMotorista " +
                                        " inner join T_HISTORICO_VINCULO_MOTORISTAS HVMotoristas on HVMotoristas.THV_CODIGO = HistoricoVinculoMotorista.THV_CODIGO " +
                                        " where HistoricoVinculoMotorista.CAR_CODIGO = Carga.CAR_CODIGO " +
                                          " and HVMotoristas.FUN_CODIGO in (select CMotoristas.CAR_MOTORISTA from T_CARGA_MOTORISTA CMotoristas where CMotoristas.CAR_CODIGO = Carga.CAR_CODIGO) " +
                                       " ) DataVinculoMotorista, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

                case "LocalVinculo":
                case "DescricaoLocalVinculo":
                    if (!somenteContarNumeroRegistros && !select.Contains(" LocalVinculo,"))
                    {
                        select.Append(" (select max(HistoricoVinculo.THV_LOCAL_VINCULO) " +
                                         " from T_HISTORICO_VINCULO HistoricoVinculo " +
                                        " where HistoricoVinculo.CAR_CODIGO = Carga.CAR_CODIGO " +
                                       " ) LocalVinculo, ");

                        if (!groupBy.Contains("Carga.CAR_CODIGO, "))
                            groupBy.Append("Carga.CAR_CODIGO, ");
                    }
                    break;

            }
        }

        protected override void SetarWhere(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null)
        {
            string datePattern = "yyyy-MM-dd HH:mm";
            //where.Append(" AND Pedido.PED_PEDIDO_DE_PRE_CARGA = 0 ");

            if (filtrosPesquisa.DataCarregamentoInicial != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.CAR_DATA_CARREGAMENTO_PEDIDO AS DATE) >= '{filtrosPesquisa.DataCarregamentoInicial.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataCarregamentoFinal != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.CAR_DATA_CARREGAMENTO_PEDIDO AS DATE) <= '{filtrosPesquisa.DataCarregamentoFinal.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataCriacaoPedidoInicial != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataCriacaoPedidoInicial.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataCriacaoPedidoFinal != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataCriacaoPedidoFinal.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInclusaoBookingInicial.HasValue)
                where.Append($" AND CAST(Pedido.PED_DATA_INCLUSAO_BOOKING AS DATE) >= '{filtrosPesquisa.DataInclusaoBookingInicial.Value.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInclusaoBookingLimite.HasValue)
                where.Append($" AND CAST(Pedido.PED_DATA_INCLUSAO_BOOKING AS DATE) <= '{filtrosPesquisa.DataInclusaoBookingLimite.Value.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInclusaoPCPInicial.HasValue)
                where.Append($" AND CAST(Pedido.PED_DATA_INCLUSAO_PCP AS DATE) >= '{filtrosPesquisa.DataInclusaoPCPInicial.Value.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInclusaoPCPLimite.HasValue)
                where.Append($" AND CAST(Pedido.PED_DATA_INCLUSAO_PCP AS DATE) <= '{filtrosPesquisa.DataInclusaoPCPLimite.Value.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInicioJanela != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_DATA_INICIO_JANELA_DESCARGA AS DATE) >= '{filtrosPesquisa.DataInicioJanela.Date.ToString(datePattern)}'");

            if (filtrosPesquisa.PrevisaoDataInicial.HasValue)
            {
                where.Append(
                    $@" AND EXISTS (
                        select TOP 1 cargaEntrega.CEN_DATA_ENTREGA_PREVISTA
                          from T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido 
                          left join T_CARGA_ENTREGA cargaEntrega on cargaEntrega.CEN_CODIGO = cargaEntregaPedido.CEN_CODIGO
					     WHERE cargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO
                           and cargaEntrega.CEN_COLETA = 0
                           AND cargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO 
                           AND cargaEntrega.CEN_DATA_ENTREGA_PREVISTA >= '{filtrosPesquisa.PrevisaoDataInicial.Value.Date.ToString(datePattern)}'
                    ) "
                );
            }

            if (filtrosPesquisa.PrevisaoDataFinal.HasValue)
            {
                where.Append(
                    $@" AND EXISTS (
                        select TOP 1 cargaEntrega.CEN_DATA_ENTREGA_PREVISTA
                          from T_CARGA_ENTREGA_PEDIDO cargaEntregaPedido 
                          left join T_CARGA_ENTREGA cargaEntrega on cargaEntrega.CEN_CODIGO = cargaEntregaPedido.CEN_CODIGO
					     WHERE cargaEntrega.CAR_CODIGO = CargaPedido.CAR_CODIGO
                           and cargaEntrega.CEN_COLETA = 0 
                           AND cargaEntregaPedido.CPE_CODIGO = CargaPedido.CPE_CODIGO 
                           AND cargaEntrega.CEN_DATA_ENTREGA_PREVISTA <= '{filtrosPesquisa.PrevisaoDataFinal.Value.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'
                    ) "
                );
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DeliveryTerm))
                where.Append($" AND Pedido.PED_DELIVERY_TERM = '{filtrosPesquisa.DeliveryTerm}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.IdAutorizacao))
                where.Append($" AND Pedido.PED_ID_AUTORIZACAO = '{filtrosPesquisa.IdAutorizacao}'");

            if (filtrosPesquisa.SituacoesPedido.Any())
                where.Append($" AND Pedido.PED_SITUACAO IN ({string.Join(", ", filtrosPesquisa.SituacoesPedido.Select(o => o.ToString("D")))}) ");

            if (filtrosPesquisa.SomenteComReserva)
                where.Append(" AND ISNULL(Pedido.PED_RESERVA, '') <> '' ");

            if (filtrosPesquisa.SomentePedidosCanceladosAposVincularCarga)
                where.Append(" AND Pedido.PED_CANCELADO_APOS_VINCULO_COM_CARGA = 1 ");

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO >= '{filtrosPesquisa.DataInicial.Date.ToString(datePattern)}'");
                if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                    where.Append($" AND Carga.CAR_DATA_CRIACAO < '{filtrosPesquisa.DataFinal.Date.AddDays(1).ToString(datePattern)}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTransportadores?.Count > 0)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                    where.Append($" AND Pedido.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");
                else
                    where.Append($" AND Carga.EMP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTransportadores)})");

                SetarJoinsTransportador(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosFilial?.Count > 0)
            {
                if (filtrosPesquisa.CodigosFilial.Any(o => o == -1))
                {
                    if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        where.Append($" AND (Pedido.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
                    else
                    {
                        where.Append($" AND (Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }

                    where.Append($@" OR EXISTS (   SELECT _cargaPedidoRecebedor.CAR_CODIGO 
                                                            FROM T_CARGA_PEDIDO _cargaPedidoRecebedor
                                                            LEFT JOIN T_PEDIDO _pedido ON _pedido.PED_CODIGO = _cargaPedidoRecebedor.PED_CODIGO
                                                            WHERE Carga.CAR_CODIGO = _cargaPedidoRecebedor.CAR_CODIGO
                                                            AND _pedido.CLI_CODIGO_RECEBEDOR IN({string.Join(",", filtrosPesquisa.CodigosRecebedores)})))");
                }
                else
                {
                    if (filtrosPesquisa.UtilizarDadosDosPedidos)
                        where.Append($" AND Pedido.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");
                    else
                    {
                        where.Append($" AND Carga.FIL_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosFilial)})");

                        SetarJoinsCarga(joins, filtrosPesquisa);
                    }
                }
            }

            if (filtrosPesquisa.CodigosOrigem.Count > 0)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                    where.Append($" AND Pedido.LOC_CODIGO_ORIGEM IN ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})");
                else
                    where.Append($" AND CargaPedido.LOC_CODIGO_ORIGEM IN ({string.Join(", ", filtrosPesquisa.CodigosOrigem)})");
            }

            if (filtrosPesquisa.SiglasOrigem.Count > 0)
            {
                where.Append($" AND CPOrigem.UF_SIGLA IN ({string.Join(", ", from o in filtrosPesquisa.SiglasOrigem select $"'{o}'")}) ");

                SetarJoinsOrigem(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosDestino.Count > 0)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                    where.Append($" AND Pedido.LOC_CODIGO_DESTINO IN ({string.Join(", ", filtrosPesquisa.CodigosDestino)})");
                else
                    where.Append($" AND CargaPedido.LOC_CODIGO_DESTINO IN ({string.Join(", ", filtrosPesquisa.CodigosDestino)})");
            }

            if (filtrosPesquisa.SiglasDestino.Count > 0)
            {
                where.Append($" AND CPDestino.UF_SIGLA IN ({string.Join(", ", from o in filtrosPesquisa.SiglasDestino select $"'{o}'")})");

                SetarJoinsDestino(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                    where.Append($" AND (Pedido.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");
                else
                {
                    where.Append($" AND (Carga.TCG_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoCarga)}){(filtrosPesquisa.CodigosTipoCarga.Contains(-1) ? " or Carga.TCG_CODIGO is null" : "")})");

                    SetarJoinsCarga(joins, filtrosPesquisa);
                }
            }

            if (filtrosPesquisa.CodigosModelosVeiculos?.Count > 0)
            {
                where.Append($" AND Carga.MVC_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosModelosVeiculos)})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CpfCnpjsRemetente.Count > 0)
                where.Append($" AND Pedido.CLI_CODIGO_REMETENTE IN ({string.Join(", ", filtrosPesquisa.CpfCnpjsRemetente)})");

            if (filtrosPesquisa.CodigosRestricoes.Count > 0)
            {
                where.Append(
                    $@" and exists (
                                select top(1) 1
                                  from T_CLIENTE_DESCARGA _clienteDescarga
                                  join T_CLIENTE_RESTRICAO_DESCARGA _clienteRestricaoDescarga on _clienteRestricaoDescarga.CLD_CODIGO = _clienteDescarga.CLD_CODIGO
                                 where _clienteDescarga.CLI_CGCCPF = Pedido.CLI_CODIGO
                                   and _clienteRestricaoDescarga.REE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosRestricoes)})
                            ) "
                );
            }

            if (filtrosPesquisa.CpfCnpjsDestinatario.Count > 0)
                where.Append($" AND Pedido.CLI_CODIGO IN ({string.Join(", ", filtrosPesquisa.CpfCnpjsDestinatario)})");

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                where.Append($" AND Carga.CAR_VEICULO = {filtrosPesquisa.CodigoVeiculo}");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                where.Append(
                    $@" AND Carga.CAR_CODIGO IN (
                        SELECT _cargamotorista.CAR_CODIGO
                          FROM T_CARGA_MOTORISTA _cargamotorista
                         INNER JOIN T_FUNCIONARIO _motoristas ON _cargamotorista.CAR_MOTORISTA = _motoristas.FUN_CODIGO 
                         WHERE _cargamotorista.CAR_CODIGO = CAR_CODIGO AND _motoristas.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}
                    )"
                );

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosGruposPessoas?.Count > 0)
            {
                where.Append($" AND Carga.GRP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosGruposPessoas)})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.Situacoes?.Count() > 0)
            {
                where.Append($" AND Carga.CAR_SITUACAO IN ({string.Join(", ", (from o in filtrosPesquisa.Situacoes select (int)o))})");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }
            if (filtrosPesquisa.SituacoesCargaMercante != null && filtrosPesquisa.SituacoesCargaMercante.Count > 0)
            {
                where.Append($" and (1 = 0 ");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Cancelada))
                    where.Append($" or Carga.CAR_SITUACAO = 13");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Anulada))
                    where.Append($" or Carga.CAR_SITUACAO = 18");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.AguardandoEmissao))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is not null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteEmissaoCTe))
                    where.Append($" or (Carga.CAR_SITUACAO = 5 and Carga.CAR_DATA_RECEBIMENTO_ULTIMA_NFE is null)");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMDFe))
                    where.Append($" or (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteMercante))
                    where.Append($" or (Carga.CAR_TODOS_CTES_COM_MERCANTE != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 5))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteFaturamento))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoCTe))
                    where.Append($" or Carga.CAR_SITUACAO = 15");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteIntegracaoFatura))
                    where.Append($" or (Carga.CAR_TODOS_CTES_FATURADOS_INTEGRADOS != 1 and Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3))");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.PendenteSVM))
                {
                    where.Append($" or (Carga.CAR_SITUACAO != 13 and Carga.CAR_SITUACAO != 18 and Carga.CAR_CODIGO in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3) and ");
                    where.Append($@" exists (
                    select
                        cargacte5_.CCT_CODIGO 
                    from
                        T_CARGA_CTE cargactes4_,
                        T_CARGA_CTE cargacte5_ 
                    left outer join
                        T_CTE conhecimen6_ 
                            on cargacte5_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                    where
                        Carga.CAR_CODIGO=cargactes4_.CAR_CODIGO 
                        and cargactes4_.CCT_CODIGO=cargacte5_.CCT_CODIGO 
                        and  not (exists (select
                            ctesvmmult7_.CSM_CODIGO 
                        from
                            T_CTE_SVM_MULTIMODAL ctesvmmult7_ 
                        inner join
                            T_CTE conhecimen8_ 
                                on ctesvmmult7_.CON_CODIGO_SVM=conhecimen8_.CON_CODIGO 
                        inner join
                            T_CTE conhecimen9_ 
                                on ctesvmmult7_.CON_CODIGO_MULTIMODAL=conhecimen9_.CON_CODIGO 
                        where
                            conhecimen8_.CON_STATUS='A'
                            and conhecimen9_.CON_TIPO_CTE=0
                            and (conhecimen9_.CON_CODIGO=conhecimen6_.CON_CODIGO 
                            or (conhecimen9_.CON_CODIGO is null) 
                            and (conhecimen6_.CON_CODIGO is null)))))
                    )");

                }
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.ComErro))
                    where.Append($" or Carga.CAR_SITUACAO = 15 or Carga.CAR_SITUACAO = 6 or Carga.CAR_PROBLEMA_CTE = 1");
                if (filtrosPesquisa.SituacoesCargaMercante.Any(o => o == SituacaoCargaMercante.Finalizada))
                {
                    where.Append($" or (Carga.CAR_SITUACAO = 11 and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MERCANTE = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_COM_MANIFESTO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_SERVICO_MULTIMODAL = 4)) and ");
                    where.Append($" (Carga.CAR_TODOS_CTES_FATURADOS = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 1 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 3 or _cargaPedido.TBF_MODAL_PROPOSTA_MULTIMODAL = 4 or _cargaPedido.TBF_TIPO_PROPOSTA_MULTIMODAL = 3)) and ");
                    where.Append($" (Carga.CAR_MDFE_AQUAVIARIO_VINCULADO = 1 or Carga.CAR_CODIGO not in (select DISTINCT _cargaPedido.CAR_CODIGO from T_CARGA_PEDIDO _cargaPedido where _cargaPedido.TBF_TIPO_COBRANCA_MULTIMODAL = 5))) ");
                }
                where.Append($" )");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                    where.Append($" AND (Pedido.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Pedido.TOP_CODIGO is null" : "")})");
                else
                {
                    where.Append($" AND (Carga.TOP_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigosTipoOperacao)}){(filtrosPesquisa.CodigosTipoOperacao.Contains(-1) ? " or Carga.TOP_CODIGO is null" : "")})");

                    SetarJoinsCarga(joins, filtrosPesquisa);
                }
            }

            if (filtrosPesquisa.TipoLocalPrestacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.todos)
            {
                if (filtrosPesquisa.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.intraMunicipal)
                    where.Append(" AND CargaPedido.LOC_CODIGO_ORIGEM = CargaPedido.LOC_CODIGO_DESTINO");
                else if (filtrosPesquisa.TipoLocalPrestacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLocalPrestacao.interMunicipal)
                    where.Append(" AND CargaPedido.LOC_CODIGO_ORIGEM <> CargaPedido.LOC_CODIGO_DESTINO");
            }

            if (filtrosPesquisa.CodigosRotaFrete.Count > 0)
                where.Append($" AND Pedido.ROF_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosRotaFrete)})");

            if (filtrosPesquisa.CodigosPedido.Count > 0)
                where.Append($" AND Pedido.PED_CODIGO IN ({string.Join(", ", filtrosPesquisa.CodigosPedido)})");

            if (filtrosPesquisa.PedidosSemCarga)
            {
                SetarJoinsCarga(joins, filtrosPesquisa);
                where.Append(
                    $@" and Pedido.PED_SITUACAO = {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto}
                        and CargaPedido.PED_CODIGO is null
                        and not exists (
                                select top(1) 1
                                  from T_CARREGAMENTO_PEDIDO _carregamentoPedido
                                  join T_CARREGAMENTO _carregamento ON _carregamento.CRG_CODIGO = _carregamentoPedido.CRG_CODIGO
                                 where _carregamentoPedido.PED_CODIGO = Pedido.PED_CODIGO
                                   and _carregamento.CRG_SITUACAO <> {(int)Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarregamento.Cancelado}
                            )"
                );
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
            {
                if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                    where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR like '%{filtrosPesquisa.NumeroCarga}%'");
                else
                    where.Append($" AND Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigoGerente > 0)
                where.Append($" AND Pedido.FUN_CODIGO_GERENTE = {filtrosPesquisa.CodigoGerente} ");

            if (filtrosPesquisa.CodigoSupervisor > 0)
                where.Append($" AND Pedido.FUN_CODIGO_SUPERVISOR = {filtrosPesquisa.CodigoSupervisor} ");

            if (filtrosPesquisa.CodigoVendedor > 0)
                where.Append($" AND Pedido.FUN_CODIGO_VENDEDOR = {filtrosPesquisa.CodigoVendedor} ");

            if (filtrosPesquisa.DataInicioViagemInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(Carga.CAR_DATA_INICIO_VIAGEM AS DATE) >= '{filtrosPesquisa.DataInicioViagemInicial.ToString(datePattern)}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataInicioViagemFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(Carga.CAR_DATA_INICIO_VIAGEM AS DATE) <= '{filtrosPesquisa.DataInicioViagemFinal.ToString(datePattern)}'");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataEntregaInicial != DateTime.MinValue)
            {
                where.Append($" AND CAST(CargaEntrega.CEN_DATA_FIM_ENTREGA AS DATE) >= '{filtrosPesquisa.DataEntregaInicial.ToString(datePattern)}'");

                SetarJoinsCargaEntrega(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataEntregaFinal != DateTime.MinValue)
            {
                where.Append($" AND CAST(CargaEntrega.CEN_DATA_FIM_ENTREGA AS DATE) <= '{filtrosPesquisa.DataEntregaFinal.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'");

                SetarJoinsCargaEntrega(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.PrevisaoEntregaPedidoDataInicial != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_PREVISAO_ENTREGA AS DATE) >= '{filtrosPesquisa.PrevisaoEntregaPedidoDataInicial.ToString(datePattern)}'");

            if (filtrosPesquisa.PrevisaoEntregaPedidoDataFinal != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_PREVISAO_ENTREGA AS DATE) <= '{filtrosPesquisa.PrevisaoEntregaPedidoDataFinal.ToString(datePattern)}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                where.Append($" AND Pedido.PED_CODIGO_PEDIDO_CLIENTE = '{filtrosPesquisa.NumeroPedidoCliente}'");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                where.Append($" AND Pedido.PED_NUMERO_PEDIDO_EMBARCADOR = '{filtrosPesquisa.NumeroPedido}'");

            if (filtrosPesquisa.CpfCnpjsExpedidor?.Count > 0)
            {
                where.Append($" AND CPExpedidor.CLI_CGCCPF in ({string.Join(", ", filtrosPesquisa.CpfCnpjsExpedidor)})");

                SetarJoinsExpedidor(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.SituacoesEntrega?.Count > 0)
            {
                where.Append($@" AND SituacoesEntrega.CPE_CODIGO = CargaPedido.CPE_CODIGO
                                 AND SituacoesEntrega.CEN_SITUACAO IN ({string.Join(", ", filtrosPesquisa.SituacoesEntrega.Select(x => (int)x))})");

                SetarJoinsCarga(joins, filtrosPesquisa);
                SetarJoinSituacoesEntrega(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.PossuiExpedidor.HasValue)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                {
                    if (filtrosPesquisa.PossuiExpedidor.Value)
                        where.Append(" AND Pedido.CLI_CODIGO_EXPEDIDOR > 0");
                    else
                        where.Append(" AND (Pedido.CLI_CODIGO_EXPEDIDOR = 0 OR Pedido.CLI_CODIGO_EXPEDIDOR IS NULL)");
                }
                else
                {
                    if (filtrosPesquisa.PossuiExpedidor.Value)
                        where.Append(" AND CargaPedido.CLI_CODIGO_EXPEDIDOR > 0");
                    else
                        where.Append(" AND (CargaPedido.CLI_CODIGO_EXPEDIDOR = 0 OR CargaPedido.CLI_CODIGO_EXPEDIDOR IS NULL)");
                }
            }

            if (filtrosPesquisa.PossuiRecebedor.HasValue)
            {
                if (filtrosPesquisa.UtilizarDadosDosPedidos)
                {
                    if (filtrosPesquisa.PossuiRecebedor.Value)
                        where.Append(" AND Pedido.CLI_CODIGO_RECEBEDOR > 0");
                    else
                        where.Append(" AND (Pedido.CLI_CODIGO_RECEBEDOR = 0 OR Pedido.CLI_CODIGO_RECEBEDOR IS NULL)");
                }
                else
                {
                    if (filtrosPesquisa.PossuiRecebedor.Value)
                        where.Append(" AND CargaPedido.CLI_CODIGO_RECEBEDOR > 0");
                    else
                        where.Append(" AND (CargaPedido.CLI_CODIGO_RECEBEDOR = 0 OR CargaPedido.CLI_CODIGO_RECEBEDOR IS NULL)");
                }
            }

            if (filtrosPesquisa.ExibirCargasAgrupadas)
            {
                where.Append(" and ((Carga.CAR_CARGA_AGRUPADA = 1 AND Carga.CAR_CARGA_FECHADA = 1) OR Carga.CAR_CODIGO_AGRUPAMENTO is not null) ");

                SetarJoinsCarga(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.CodigoNumeroViagemNavio > 0)
            {
                where.Append($" AND PedidoViagemNavio.PVN_CODIGO = {filtrosPesquisa.CodigoNumeroViagemNavio} ");

                SetarJoinsPedidoViagemNavio(joins);
            }

            if (filtrosPesquisa.CodigoPortoOrigem > 0)
            {
                where.Append($" AND PortoOrigem.POT_CODIGO = {filtrosPesquisa.CodigoPortoOrigem} ");

                SetarJoinsPortoOrigem(joins);
            }

            if (filtrosPesquisa.CodigoPortoDestino > 0)
            {
                where.Append($" AND PortoDestino.POT_CODIGO = {filtrosPesquisa.CodigoPortoDestino} ");

                SetarJoinsPortoDestino(joins);
            }

            if (filtrosPesquisa.TipoPropostaMultiModal.Any())
            {
                where.Append($" AND CTipoOperacao.TOP_TIPO_PROPOSTA_MULTIMODAL IN ({string.Join(", ", filtrosPesquisa.TipoPropostaMultiModal.Select(o => o.ToString("D")))}) ");

                SetarJoinsTipoOperacao(joins, filtrosPesquisa);
            }

            if (filtrosPesquisa.DataETAPortoOrigemInicial != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO 
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM
                           AND Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO >= '{filtrosPesquisa.DataETAPortoOrigemInicial.Date.ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETAPortoOrigemFinal != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO 
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM
                           AND Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO <= '{filtrosPesquisa.DataETAPortoOrigemFinal.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETSPortoOrigemInicial != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM
                           AND Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO >= '{filtrosPesquisa.DataETSPortoOrigemInicial.Date.ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETSPortoOrigemFinal != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_ORIGEM
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_ORIGEM
                           AND Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO <= '{filtrosPesquisa.DataETSPortoOrigemFinal.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETAPortoDestinoInicial != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO
                           AND Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO >= '{filtrosPesquisa.DataETAPortoDestinoInicial.Date.ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETAPortoDestinoFinal != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO
                           AND Schedule.PVS_DATA_PREVISAO_CHEGADA_NAVIO <= '{filtrosPesquisa.DataETAPortoDestinoFinal.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETSPortoDestinoInicial != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO
                           AND Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO >= '{filtrosPesquisa.DataETSPortoDestinoInicial.Date.ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataETSPortoDestinoFinal != DateTime.MinValue)
            {
                where.Append(
                    $@" AND EXISTS (
                                SELECT TOP 1 Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO
                                FROM T_PEDIDO_VIAGEM_NAVIO_SCHEDULE Schedule
                                WHERE Schedule.PVN_CODIGO = Pedido.PVN_CODIGO
                                AND Schedule.POT_CODIGO_ATRACACAO = Pedido.POT_CODIGO_DESTINO
                                AND Schedule.TTI_CODIGO_ATRACACAO = Pedido.TTI_CODIGO_DESTINO
                           AND Schedule.PVS_DATA_PREVISAO_SAIDA_NAVIO <= '{filtrosPesquisa.DataETSPortoDestinoFinal.AddDays(1).AddMilliseconds(-1).ToString(datePattern)}'
                    ) "
                );

                SetarJoinsCte(joins);
            }

            if (filtrosPesquisa.DataInclusaoPedidoInicial != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_DATA_CRIACAO AS DATE) >= '{filtrosPesquisa.DataInclusaoPedidoInicial.ToString(datePattern)}'");

            if (filtrosPesquisa.DataInclusaoPedidoFinal != DateTime.MinValue)
                where.Append($" AND CAST(Pedido.PED_DATA_CRIACAO AS DATE) <= '{filtrosPesquisa.DataInclusaoPedidoFinal.ToString(datePattern)}'");

            if (filtrosPesquisa.CodigoOperadorPedido?.Count > 0)
            {
                where.Append($" AND Pedido.FUN_CODIGO_AUTOR in ({string.Join(", ", filtrosPesquisa.CodigoOperadorPedido)})");
            }

            if (filtrosPesquisa.CodigoCentroResultado?.Count > 0)
            {
                where.Append($" AND Pedido.CRE_CODIGO in ({string.Join(", ", filtrosPesquisa.CodigoCentroResultado)})");
            }

            if (filtrosPesquisa.AguardandoIntegracao.HasValue)
            {
                where.Append($" AND Pedido.PED_AG_INTEGRACAO = {filtrosPesquisa.AguardandoIntegracao.GetHashCode()}");
            }

            if (filtrosPesquisa.SomentePedidosDeIntegracao)
                where.Append(" AND (Pedido.PED_PROTOCOLO > 0 AND Pedido.PED_PEDIDO_INTEGRADO_EMBARCADOR = 1 AND Pedido.PED_ADICIONADA_MANUALMENTE = 0 AND Pedido.PED_PEDIDO_IMPORTADO_POR_PLANILHA = 0)");

            if (filtrosPesquisa.CentroDeCustoViagemDescricao is not null && filtrosPesquisa.CentroDeCustoViagemCodigo > 0)
            {
                where.Append($" and Pedido.CCV_CODIGO IN ({(int)filtrosPesquisa.CentroDeCustoViagemCodigo})");
            }

        }

        #endregion Métodos Protegidos Sobrescritos

        #region Métodos Públicos

        public string ObterSqlPesquisaProdutos(Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();

            SetarWhere(filtrosPesquisa, where, joins, groupBy);

            sql.Append("SELECT Pedido.PED_CODIGO CodigoPedido, Produto.PRO_CODIGO Codigo, Produto.GRP_DESCRICAO Descricao ");
            sql.Append("  FROM T_PEDIDO Pedido ");
            sql.Append("  JOIN T_PEDIDO_PRODUTO PedidoProduto ON Pedido.PED_CODIGO = PedidoProduto.PED_CODIGO ");
            sql.Append("  JOIN T_PRODUTO_EMBARCADOR Produto ON PedidoProduto.PRO_CODIGO = Produto.PRO_CODIGO ");
            sql.Append(joins.ToString());

            if (where.Length > 0)
                sql.Append($" where {where.ToString().Trim().Substring(3)} ");

            return sql.ToString();
        }

        #endregion Métodos Públicos
    }
}
