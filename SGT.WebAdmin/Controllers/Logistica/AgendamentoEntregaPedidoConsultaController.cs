using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AgendamentoEntregaPedidoConsulta")]
    public class AgendamentoEntregaPedidoConsultaController : BaseController
    {
        #region Construtores

        public AgendamentoEntregaPedidoConsultaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = "";

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int totalRegistros = repositorioCargaPedido.ContarConsultaAgendamentoEntregaPedidoConsulta(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta> listaGrid = totalRegistros > 0 ? repositorioCargaPedido.ConsultaAgendamentoEntregaPedidoConsulta(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaGrid);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaAuditoria()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisaAuditoria();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = "";

                Repositorio.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria repositorio = new Repositorio.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria(unitOfWork);

                int codigoPedido = Request.GetIntParam("CodigoPedido");

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega configuracaoAgendamentoEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoEntrega(unitOfWork).BuscarConfiguracaoPadrao();
                int codigoCargaEntrega = (configuracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false) ? Request.GetIntParam("CodigoCargaEntrega") : 0;
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaEntregaPedido.BuscarPedidosPorEntrega(codigoCargaEntrega);

                List<int> codigosPedidos = pedidos.Count > 0 ? pedidos.Select(p => p.Codigo).ToList() : new List<int>() { codigoPedido };
                int totalRegistros = repositorio.ContarConsultaPorPedido(codigosPedidos);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria> listaRetorno = totalRegistros > 0 ? repositorio.ConsultaPorPedidos(codigosPedidos, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria>();

                var retorno = (from o in listaRetorno
                               select new
                               {
                                   o.Codigo,
                                   Descricao = $"{o.Empresa.NomeFantasia} exportou os dados do agendamento.",
                                   Data = o.Data.ToString("dd/MM/yyyy HH:mm"),
                                   Transportador = o.Empresa.Descricao
                               });

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a consulta.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                parametrosConsulta.PropriedadeOrdenar = "";
                parametrosConsulta.LimiteRegistros = 0;
                parametrosConsulta.InicioRegistros = 0;

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                int totalRegistros = repositorioCargaPedido.ContarConsultaAgendamentoEntregaPedidoConsulta(filtrosPesquisa);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta> listaGrid = totalRegistros > 0 ? repositorioCargaPedido.ConsultaAgendamentoEntregaPedidoConsulta(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntregaConsulta>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && totalRegistros > 0)
                {
                    List<int> codigosPedidos = listaGrid.Select(obj => obj.CodigoPedido).Distinct().ToList();

                    Repositorio.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria repositorioAgendamentoEntregaAuditoria = new Repositorio.Embarcador.Logistica.AgendamentoEntregaPedidoConsultaAuditoria(unitOfWork);
                    Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                    int registrosAdicionados = 0;
                    int limiteRegistros = 1900;

                    for (int i = 0; i < codigosPedidos.Count; i += limiteRegistros)
                    {
                        List<int> codigosAdicionar = codigosPedidos.Skip(i).Take(limiteRegistros).ToList();
                        registrosAdicionados += repositorioAgendamentoEntregaAuditoria.AdicionarAuditorias(codigosAdicionar, this.Empresa.Codigo);
                    }
                }

                grid.AdicionaRows(listaGrid);

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Estado", "UFDestino", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("CT-e", "CTe", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Agendamento", "SituacaoAgendamentoDescricao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Agendamento", "DataAgendamentoFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Agendamento", "ObservacaoAgendamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Reagendamento", "ObservacaoReagendamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Exige Agendamento", "DescricaoExigeAgendamento", 15, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Volumes", "Volumes", 15, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Metros Cúbicos", "MetrosCubicos", 15, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo De Carga", "TipoCarga", 15, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão de Entrega", "DataPrevisaoEntregaFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Logistica/AgendamentoEntregaPedidoConsulta", "grid-agendamento-entrega-pedido-consulta");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntregaPedidoConsulta()
            {
                CodigoDestino = Request.GetIntParam("Destino"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : 0,
                CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                CTe = Request.GetIntParam("CTe"),
                DataAgendamentoInicial = Request.GetNullableDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetNullableDateTimeParam("DataAgendamentoFinal"),
                Estado = Request.GetStringParam("Estado"),
                NotaFiscal = Request.GetIntParam("NotaFiscal"),
                Situacao = Request.GetNullableEnumParam<SituacaoAgendamentoEntregaPedido>("Situacao")
            };
        }

        private Models.Grid.Grid ObterGridPesquisaAuditoria()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data/Hora", "Data", 8, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        #endregion
    }
}
