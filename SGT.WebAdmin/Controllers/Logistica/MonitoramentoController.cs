using Dominio.Excecoes.Embarcador;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Globalization;


namespace SGT.WebAdmin.Controllers.Logistica
{

    [CustomAuthorize("Logistica/Monitoramento", "TorreControle/AcompanhamentoCarga", "Logistica/MonitoramentoNovo")]
    public class MonitoramentoController : MonitoramentoControllerBase
    {
        #region Construtores

        public MonitoramentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos privados

        private readonly string _urlController = "Logistica/Monitoramento";

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosAlertaResumoVeiculo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaEventos = repMonitoramentoEvento.BuscarTodosAtivos();
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaEventosPrioridade = listaEventos.OrderBy(o => o.Prioridade).ToList();

                Repositorio.Embarcador.Logistica.Monitoramento repMoniroramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                filtrosPesquisa.DescricaoAlerta = string.Empty; // Ignora o filtro por tipo de alerta
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo> resumoAlertas = repMoniroramento.ConsultarTotaisAlertas(filtrosPesquisa, configuracao, configuracaoMonitoramento);

                dynamic listaRetornar = new List<dynamic>();

                foreach (var evento in listaEventosPrioridade)
                {
                    var alerta = resumoAlertas.Where(a => a.TipoAlerta == evento.TipoAlerta && a.MonitoramentoEvento == evento.Codigo).FirstOrDefault();

                    if (alerta == null || alerta.Total == 0)
                        continue;

                    var resumoAlerta =
                    new
                    {
                        label = evento.Descricao, //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(evento.TipoAlerta),
                        value = alerta?.Total ?? 0,
                        backgroundColor = ObterCorAlerta(evento.TipoAlerta, listaEventos)
                    };

                    listaRetornar.Add(resumoAlerta);
                }

                return new JsonpResult(listaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosResumo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosAlertaResumoCargasSituacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                dynamic listaRetornar = new List<dynamic>();

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.ResumoCargaStatus> monitoramentos = repMonitoramento.ObterResumoCargaStatus(filtrosPesquisa, configuracao, configuracaoMonitoramento);

                unitOfWork.CommitChanges();

                var listaAgrupada = monitoramentos.GroupBy(x => new { x.CodigoStatusViagem, x.statusViagem })
                    .Select(obj => new
                    {
                        status = obj.Key.statusViagem,
                        codigoStatus = obj.Key.CodigoStatusViagem,
                        total = obj.Count(),
                        totalAlerta = obj.Sum(x => x.totalAlertas)
                    }).ToList();

                return new JsonpResult(listaAgrupada);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosResumo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterResumoCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.CargaNaoEncontrada);

                return new JsonpResult(ObterGridResumoCarga(carga, unitOfWork));
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosAlertaResumo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus statusMonitoramento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus");
                Repositorio.Embarcador.Logistica.AlertaMonitor alertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.AlertasResumo> resumoAlertas = alertaMonitor.BuscarTotaisAlertasEmAberto(statusMonitoramento);

                var listaRetornar = (
                    from reg in resumoAlertas
                    select new
                    {
                        Descricao = reg.Descricao,
                        Valor = reg.Total
                    }
                ).ToList();


                return new JsonpResult(listaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosResumoAlerta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAlertasEmAberto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int carga = Request.GetIntParam("carga");
                string DescricaoAlerta = Request.GetStringParam("DescricaoAlerta");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterTipoDeAlertaPorDescricao(DescricaoAlerta);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", "Codigo", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoDeAlerta, "Tipo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Veiculo, "Placa", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Data, "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "Valor", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Latitude", false);
                grid.AdicionarCabecalho("Longitude", false);
                grid.AdicionarCabecalho("Veiculo", false);

                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos = repMonitoramentoEvento.BuscarTodos();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorio = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> regs = tipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta ? repositorio.BuscarEmAbertoPorCarga(carga, parametrosConsulta) : repositorio.BuscarEmAbertoPorCargaeTipoAlerta(carga, tipoAlerta, parametrosConsulta);
                int totalRegistros = regs.Count;

                var listaRetornar = (
                    from reg in regs
                    select new
                    {
                        Codigo = reg.Codigo,
                        Tipo = ObterDescricaoMonitoramentoEvento(monitoramentoEventos, reg.TipoAlerta),
                        Data = reg.Data,
                        Latitude = reg.Latitude,
                        Longitude = reg.Longitude,
                        Valor = reg.AlertaDescricao,
                        Placa = reg.Veiculo.Placa,
                        Veiculo = reg.Veiculo.Codigo
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAlertas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int carga = Request.GetIntParam("carga");
                string DescricaoAlerta = Request.GetStringParam("DescricaoAlerta");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterTipoDeAlertaPorDescricao(DescricaoAlerta);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "Carga", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Alertas, "Alerta", 20, Models.Grid.Align.left, false, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoDeAlerta, "Tipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Veiculo, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Data, "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "Valor", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "StatusDescricao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "Latitude", 10, Models.Grid.Align.right, false, false).NumberFormat("n10"); ;
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "Longitude", 10, Models.Grid.Align.right, false, false).NumberFormat("n10"); ;
                grid.AdicionarCabecalho("Veiculo", false);

                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos = repMonitoramentoEvento.BuscarTodos();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorio = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> regs = (tipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta) ?
                    repositorio.BuscarPorCarga(carga, parametrosConsulta) : repositorio.BuscarPorCargaETipoAlerta(carga, tipoAlerta, parametrosConsulta);

                int totalRegistros = (tipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta.SemAlerta) ? repositorio.ContarPorCarga(carga) : repositorio.ContarPorCargaETipoAlerta(carga, tipoAlerta);

                var listaRetornar = (
                    from reg in regs
                    select new
                    {
                        Codigo = reg.Codigo,
                        Carga = reg.Carga.CodigoCargaEmbarcador,
                        Alerta = reg.MonitoramentoEvento?.Descricao ?? "",
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaHelper.ObterDescricao(reg.TipoAlerta),
                        Data = reg.Data,
                        Latitude = reg.Latitude,
                        Longitude = reg.Longitude,
                        Valor = reg.AlertaDescricao,
                        Placa = reg.Veiculo.Placa,
                        Veiculo = reg.Veiculo.Codigo,
                        Status = (int)reg.Status,
                        StatusDescricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatusHelper.ObterDescricao(reg.Status)
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaConsultarAlertas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCargasCanceladasCompativeis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int carga = Request.GetIntParam("carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "Carga", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.TipoOperacao, "TipoOperacao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Veiculo, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataCriacao, "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Destinos, "Destinos", 10, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Cargas.Carga repositorio = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.Carga cargaExemplo = repositorio.BuscarPorCodigoFetch(carga);

                if (cargaExemplo == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.CargaNaoEncontrada);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosExemplo = repositorioCargaPedido.BuscarPorCarga(cargaExemplo.Codigo);

                //List<int> localidadeOrigens = cargaPedidosExemplo.Select(x => x.Pedido.Origem.Codigo).ToList();
                //List<int> localidadeDestino = cargaPedidosExemplo.Select(x => x.Pedido.Destino.Codigo).ToList();

                List<double> remetentes = cargaPedidosExemplo.Select(x => x.Pedido.Remetente.CPF_CNPJ).ToList();
                List<double> destinatarios = cargaPedidosExemplo.Select(x => x.Pedido.Destinatario.CPF_CNPJ).ToList();
                int veiculo = cargaExemplo.Veiculo.Codigo;
                int transportador = cargaExemplo.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Entidades.Embarcador.Cargas.Carga> regs = repositorio.ConsultaCargasCanceladasCompativeis(cargaExemplo, remetentes, destinatarios, transportador, veiculo, parametrosConsulta);

                int totalRegistros = regs.Count;

                var listaRetornar = (
                    from reg in regs
                    select new
                    {
                        Codigo = reg.Codigo,
                        Carga = reg.CodigoCargaEmbarcador,
                        TipoOperacao = reg.TipoOperacao?.Descricao ?? "",
                        Data = reg.DataCriacaoCarga.ToString("dd/MM/yyyy HH:mm:ss"),
                        Placa = reg.Veiculo.Placa,
                        Destinos = reg.DadosSumarizados.Destinos
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoConsultarCargasCompativeis);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaMonitor = repAlertaMonitor.BuscarPorCodigo(codigo);
                if (alertaMonitor == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.AlertaNaoEncontrado);

                Repositorio.Embarcador.Logistica.AlertaTratativa repAlertaTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativa alertaTratativa = repAlertaTratativa.BuscarPorAlerta(codigo);

                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos = repMonitoramentoEvento.BuscarTodos();

                dynamic alerta = new
                {
                    alertaMonitor.Codigo,
                    DataCadastro = alertaMonitor.DataCadastro.ToString(),
                    Data = alertaMonitor.Data.ToString(),
                    Tipo = ObterDescricaoMonitoramentoEvento(monitoramentoEventos, alertaMonitor.TipoAlerta),
                    alertaMonitor.Descricao,
                    alertaMonitor.AlertaDescricao,
                    alertaMonitor.Latitude,
                    alertaMonitor.Longitude,
                    Carga = alertaMonitor.Carga.CodigoCargaEmbarcador,
                    Veiculo = alertaMonitor.Veiculo.Placa,
                    alertaMonitor.Status,
                    StatusDescricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatusHelper.ObterDescricao(alertaMonitor.Status),
                    DataFim = (alertaMonitor.DataFim != null) ? alertaMonitor.DataFim.ToString() : string.Empty,
                    Usuario = alertaTratativa?.Usuario?.Nome ?? string.Empty,
                    Tratativa = alertaTratativa?.AlertaTratativaAcao?.Descricao ?? string.Empty,
                    Observacao = alertaTratativa?.Observacao ?? string.Empty
                };
                return new JsonpResult(alerta);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDetalhesAlerta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaTorreMonitoramento()
        {
            try
            {
                var grid = ObterGridPesquisaTorreMonitoramento();
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaTorreMonitoramento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaTorreMonitoramento());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAlertasTorre()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfigAlertaCarga = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> listaEventosAtivos = repConfigAlertaCarga.BuscarTodosAtivos();

                Repositorio.Embarcador.Logistica.Monitoramento repMoniroramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisaTorre(unitOfWork);
                filtrosPesquisa.DescricaoAlerta = string.Empty; // Ignora o filtro por tipo de alerta

                IList<Dominio.ObjetosDeValor.Embarcador.Carga.AlertaCarga.AlertaCargaResumo> resumoAlertas = repMoniroramento.ConsultarTotaisAlertasTorre(filtrosPesquisa, configuracao);
                dynamic listaCabecalhoRetornar = new List<dynamic>();
                dynamic listaDadosRetornarTotais = new List<dynamic>();
                dynamic listaDadosRetornarPendentes = new List<dynamic>();
                dynamic listaDadosRetornarCargas = new List<dynamic>();

                foreach (var eventoAtivo in listaEventosAtivos)
                {
                    var alerta = resumoAlertas.Where(a => a.TipoAlerta == eventoAtivo.TipoCargaAlerta).FirstOrDefault();
                    if (alerta == null || alerta.Total == 0)
                        continue;

                    var cabecalhoAlerta =
                    new
                    {
                        DescricaoAlerta = eventoAtivo.Descricao,
                        TipoAlerta = eventoAtivo.TipoCargaAlerta,
                        backgroundColor = ObterCorAlertaTorre(eventoAtivo.TipoCargaAlerta, listaEventosAtivos),
                    };

                    listaCabecalhoRetornar.Add(cabecalhoAlerta);

                    var dadosAlertaTotais =
                    new
                    {
                        Totais = alerta?.Total ?? 0,
                        backgroundColor = ObterCorAlertaTorre(eventoAtivo.TipoCargaAlerta, listaEventosAtivos),
                    };

                    listaDadosRetornarTotais.Add(dadosAlertaTotais);

                    var dadosAlertaCargas =
                    new
                    {
                        Cargas = alerta?.Cargas ?? 0,
                        backgroundColor = ObterCorAlertaTorre(eventoAtivo.TipoCargaAlerta, listaEventosAtivos),
                    };

                    listaDadosRetornarCargas.Add(dadosAlertaCargas);

                    var dadosAlertaPendentes =
                    new
                    {
                        Pendentes = alerta?.Pendentes ?? 0,
                        backgroundColor = ObterCorAlertaTorre(eventoAtivo.TipoCargaAlerta, listaEventosAtivos),
                    };

                    listaDadosRetornarPendentes.Add(dadosAlertaPendentes);

                }

                var ObjetoRetorno =
                    new
                    {
                        ListaCabecalho = listaCabecalhoRetornar,
                        ListaDadosTotais = listaDadosRetornarTotais,
                        ListaDadosCargas = listaDadosRetornarCargas,
                        ListaDadosPendentes = listaDadosRetornarPendentes
                    };

                return new JsonpResult(ObjetoRetorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosResumo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterAlertasTorreGrid()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int carga = Request.GetIntParam("carga");
                string DescricaoAlerta = Request.GetStringParam("DescricaoAlerta");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga tipoAlerta = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterTipoDeAlertaPorDescricao(DescricaoAlerta);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "Carga", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoDoAlerta, "Tipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Data, "Data", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Valor, "Valor", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "StatusDescricao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", false);

                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repConfigEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> Eventos = repConfigEvento.BuscarTodos();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repositorioAlerta = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento> regs = (tipoAlerta == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga.SemAlerta) ?
                    repositorioAlerta.BuscarPorCarga(carga) : repositorioAlerta.BuscarPorCargaETipoAlerta(carga, tipoAlerta, parametrosConsulta);
                int totalRegistros = regs.Count;

                var listaRetornar = (
                    from reg in regs
                    select new
                    {
                        Codigo = reg.Codigo,
                        Carga = reg.Carga.CodigoCargaEmbarcador,
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(reg.TipoAlerta),
                        Data = reg.DataEvento,
                        Valor = reg.AlertaDescricao,
                        Status = (int)reg.Status,
                        StatusDescricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatusHelper.ObterDescricao(reg.Status)
                    }
                ).ToList();

                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaConsultarAlertas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesAlertaTorre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento alerta = repCargaEvento.BuscarPorCodigo(codigo);
                if (alerta == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.AlertaNaoEncontrado);

                dynamic alertaevento = new
                {
                    alerta.Codigo,
                    DataCadastro = alerta.DataCadastro.ToString(),
                    Data = alerta.DataEvento.ToString(),
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCargaHelper.ObterDescricao(alerta.TipoAlerta),
                    alerta.Descricao,
                    alerta.AlertaDescricao,
                    Carga = alerta.Carga.CodigoCargaEmbarcador,
                    alerta.Status,
                    StatusDescricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatusHelper.ObterDescricao(alerta.Status),
                    Observacao = alerta?.Observacao ?? string.Empty
                };
                return new JsonpResult(alertaevento);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDetalhesAlerta);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarTratativaAlertaCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAlerta = Request.GetIntParam("CodigoAlerta");
                var observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.CargaEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.CargaEvento alertaSelecionado = repCargaEvento.BuscarPorCodigo(codigoAlerta);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                if (alertaSelecionado == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.AlertaNaoEncontrado);
                if (alertaSelecionado.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado) return new JsonpResult(false, "Alerta já finalizado.");

                unitOfWork.Start();
                try
                {
                    alertaSelecionado.Observacao = observacao;
                    alertaSelecionado.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                    repCargaEvento.Atualizar(alertaSelecionado);
                    servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(null, alertaSelecionado);
                    unitOfWork.CommitChanges();
                }
                catch
                {
                    unitOfWork.Rollback();
                    throw;
                }


                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoAdicionarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosMapaHistoricoPosicao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                Repositorio.Embarcador.Logistica.Posicao repositorioPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Posicao posicaoPreTrip = new Dominio.Entidades.Embarcador.Logistica.Posicao();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                DateTime? dataInicial = Request.GetNullableDateTimeParam("DataInicial");
                DateTime? dataFinal = Request.GetNullableDateTimeParam("DataFinal");
                DateTime dataAtual = DateTime.Now;

                bool posicoesMobile = Request.GetBoolParam("PosicaoMobile");
                bool posicoesTecnologia = Request.GetBoolParam("PosicaoTecnologia");

                if (monitoramento.Carga.Filial != null && monitoramento.Carga.Filial.HabilitarPreViagemTrizy && monitoramento.Carga.Filial.TipoOperacoesTrizy.Contains(monitoramento.Carga.TipoOperacao))
                    posicaoPreTrip = repositorioPosicao.BuscarPrimeiraPosicao(monitoramento, monitoramento.Veiculo.Codigo);


                if (!dataInicial.HasValue)
                    dataInicial = monitoramento.DataCriacao;

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                if (dataInicial != null && dataFinal != null)
                {
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    posicoesVeiculo = repPosicao.BuscarWaypointsPorMonitoramentoVeiculo(monitoramento.Codigo, null, dataInicial, dataFinal, false, posicoesMobile, posicoesTecnologia);
                }
                var listaPosicao = (
                   from posicao in posicoesVeiculo
                   where posicao.Latitude >= -90 && posicao.Latitude <= 90 && posicao.Longitude >= -180 && posicao.Longitude <= 180
                   select new
                   {
                       Data = posicao.DataVeiculo.ToString(MASCARA_DATA_HMS),
                       posicao.Placa,
                       posicao.Descricao,
                       posicao.Latitude,
                       posicao.Longitude,
                       online = ((DateTime.Now - posicao.DataVeiculo).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString()
                   }
                );


                dynamic dadosVeiculo = ObterDadosVeiculoParaMapa(unitOfWork, monitoramento);
                dynamic dadosEntregas = ObterDadosEntregasParaMapa(unitOfWork, monitoramento);
                dynamic areas = ObterAreas(unitOfWork, monitoramento.Carga, configuracao);
                dynamic paradas = ObterParadas(unitOfWork, monitoramento, configuracao);
                dynamic monitoramentoVeiculos = ObterPolilinhaRealizadaVeiculos(unitOfWork, monitoramento);
                var retorno = (
                    new
                    {
                        Veiculo = dadosVeiculo,
                        Entregas = dadosEntregas,
                        PolilinhaPrevista = monitoramento.PolilinhaPrevista,
                        DistanciaPrevista = String.Format("{0:n0}Km", monitoramento.DistanciaPrevista),
                        PolilinhaRealizada = monitoramento.PolilinhaRealizada,
                        DistanciaRealizada = String.Format("{0:n0}Km", monitoramento.DistanciaRealizada),
                        PolilinhaAteOrigem = monitoramento.PolilinhaAteOrigem,
                        DistanciaAteOrigem = String.Format("{0:n0}Km", monitoramento.DistanciaAteOrigem),
                        PolilinhaAteDestino = monitoramento.PolilinhaAteDestino,
                        DistanciaAteDestino = String.Format("{0:n0}Km", monitoramento.DistanciaAteDestino),
                        PontosPrevistos = monitoramento.PontosPrevistos,
                        Posicoes = listaPosicao,
                        Areas = areas,
                        Paradas = paradas,
                        MonitoramentoVeiculos = monitoramentoVeiculos,
                        PosicaoInicioPreTrip = (new { Latitude = posicaoPreTrip?.Latitude ?? 0, Longitude = posicaoPreTrip?.Longitude ?? 0 })
                    }
                );

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosMapa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosMapaHistoricoPosicaoTelaAcompanhamentoCarga()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                DateTime? dataInicial = monitoramento.DataInicio;
                DateTime? dataFinal = dataInicial = monitoramento.DataFim;

                if (!dataInicial.HasValue)
                    dataInicial = monitoramento.DataCriacao;

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
                if (dataInicial != null && dataFinal != null)
                {
                    Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                    posicoesVeiculo = repPosicao.BuscarWaypointsPorMonitoramentoVeiculo(monitoramento.Codigo, null, dataInicial, dataFinal);
                }

                //var listaPosicao = (
                //   from posicao in posicoesVeiculo
                //   where posicao.Latitude >= -90 && posicao.Latitude <= 90 && posicao.Longitude >= -180 && posicao.Longitude <= 180
                //   select new
                //   {
                //       Data = posicao.DataVeiculo.ToString(MASCARA_DATA_HMS),
                //       posicao.Placa,
                //       posicao.Descricao,
                //       posicao.Latitude,
                //       posicao.Longitude,
                //       online = ((DateTime.Now - posicao.DataVeiculo).TotalMinutes <= configuracao.TempoSemPosicaoParaVeiculoPerderSinal).ToString()
                //   }
                //);

                //dynamic dadosVeiculo = ObterDadosVeiculoParaMapa(unitOfWork, monitoramento);
                dynamic dadosEntregas = ObterDadosEntregasParaMapa(unitOfWork, monitoramento);
                //dynamic areas = ObterAreas(unitOfWork, monitoramento.Carga, configuracao);
                //dynamic paradas = ObterParadas(unitOfWork, monitoramento, configuracao);
                //dynamic monitoramentoVeiculos = ObterPolilinhaRealizadaVeiculos(unitOfWork, monitoramento);
                var retorno = (
                    new
                    {
                        //Veiculo = dadosVeiculo,
                        Entregas = dadosEntregas,
                        //PolilinhaPrevista = monitoramento.PolilinhaPrevista,
                        DistanciaPrevista = String.Format("{0:n0}Km", monitoramento.DistanciaPrevista),
                        //PolilinhaRealizada = monitoramento.PolilinhaRealizada,
                        DistanciaRealizada = String.Format("{0:n0}Km", monitoramento.DistanciaRealizada),
                        //PolilinhaAteOrigem = monitoramento.PolilinhaAteOrigem,
                        DistanciaAteOrigem = String.Format("{0:n0}Km", monitoramento.DistanciaAteOrigem),
                        //PolilinhaAteDestino = monitoramento.PolilinhaAteDestino,
                        DistanciaAteDestino = String.Format("{0:n0}Km", monitoramento.DistanciaAteDestino),
                        PontosPrevistos = monitoramento.PontosPrevistos,
                        wayPointsPrevistos = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento.PolilinhaPrevista),
                        wayPointsRealizados = Servicos.Embarcador.Logistica.Polilinha.Decodificar(monitoramento.PolilinhaRealizada),
                        //Posicoes = listaPosicao,
                        //Areas = areas,
                        //Paradas = paradas,
                        //MonitoramentoVeiculos = monitoramentoVeiculos,
                    }
                );

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosMapa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramento()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("codigo");

                NumberFormatInfo nfi = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalDigits = 0 };
                NumberFormatInfo nfid = new NumberFormatInfo { NumberGroupSeparator = ".", NumberDecimalSeparator = ",", NumberDecimalDigits = 1 };

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Logistica.MonitoramentoDadosSumarizados repMonitoramentoDadosSumarizados = new Repositorio.Embarcador.Logistica.MonitoramentoDadosSumarizados(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoDadosSumarizados> monitoramentoDadosSumarizados = repMonitoramentoDadosSumarizados.BuscarPorMonitoramento(codigo);

                if (monitoramento == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                List<Dominio.Entidades.Veiculo> reboques = monitoramento.Carga?.VeiculosVinculados?.ToList() ?? new List<Dominio.Entidades.Veiculo>();
                List<string> motoristasNome = new List<string>();
                List<string> motoristasCPF = new List<string>();
                List<string> motoristasTelefone = new List<string>();
                int totalMotoristas = monitoramento.Carga?.Motoristas?.Count ?? 0;

                for (int i = 0; i < totalMotoristas; i++)
                {
                    motoristasNome.Add(monitoramento.Carga.Motoristas[i].Nome);
                    motoristasCPF.Add(monitoramento.Carga.Motoristas[i].CPF_CNPJ_Formatado_Com_Asterisco);
                    motoristasTelefone.Add(monitoramento.Carga.Motoristas[i].Telefone);
                }

                string na = "N/A";
                return new JsonpResult(new
                {
                    Codigo = monitoramento.Codigo,
                    CodigoCarga = monitoramento.Carga?.Codigo ?? 0,
                    CodigoCargaEmbarcador = monitoramento.Carga?.CodigoCargaEmbarcador ?? "",
                    Data = monitoramento.DataCriacao?.ToString(MASCARA_DATA_HM),
                    DataInicio = monitoramento.DataInicio?.ToString(MASCARA_DATA_HM),
                    DataFim = monitoramento.DataFim?.ToString(MASCARA_DATA_HM),
                    DistanciaPrevista = (monitoramento.DistanciaPrevista != null) ? String.Format("{0:n1} Km", monitoramento.DistanciaPrevista) : na,
                    DistanciaRealizada = String.Format("{0:n1} Km", monitoramento.DistanciaRealizada),
                    DistanciaAteOrigem = (monitoramento.DistanciaAteOrigem != null) ? String.Format("{0:n1} Km", monitoramento.DistanciaAteOrigem) : na,
                    DistanciaAteDestino = (monitoramento.DistanciaAteDestino != null) ? String.Format("{0:n1} Km", monitoramento.DistanciaAteDestino) : na,
                    MotoristaNome = String.Join(", ", motoristasNome.ToArray()),
                    MotoristaCPF = String.Join(", ", motoristasCPF.ToArray()),
                    MotoristaTelefone = String.Join(", ", motoristasTelefone.ToArray()),
                    CavaloPlaca = monitoramento.Veiculo.Placa,
                    CavaloTara = monitoramento.Veiculo.Tara.ToString("n", nfi),
                    CavaloCapacidade = monitoramento.Veiculo.CapacidadeKG.ToString("n", nfi),
                    ReboquePlaca = string.Join(", ", reboques.Select(reboque => reboque.Placa)),
                    ReboqueTara = string.Join(", ", reboques.Select(reboque => reboque.Tara.ToString("n", nfi))),
                    ReboqueCapacidade = string.Join(", ", reboques.Select(reboque => reboque.CapacidadeKG.ToString("n", nfi))),
                    PosicaoAtualData = (monitoramento.UltimaPosicao != null) ? monitoramento.UltimaPosicao.DataVeiculo.ToString() : na,
                    PosicaoAtualLatitude = (monitoramento.UltimaPosicao != null) ? monitoramento.UltimaPosicao.Latitude.ToString("f6").Replace(",", ".") : na,
                    PosicaoAtualLongitude = (monitoramento.UltimaPosicao != null) ? monitoramento.UltimaPosicao.Longitude.ToString("f6").Replace(",", ".") : na,
                    Velocidade = (monitoramento.UltimaPosicao != null) ? monitoramento.UltimaPosicao.Velocidade.ToString() + " Km/h" : na,
                    Temperatura = (monitoramento.UltimaPosicao != null && monitoramento.UltimaPosicao.Temperatura != null && monitoramento.UltimaPosicao.Temperatura != 0) ? monitoramento.UltimaPosicao.Temperatura.Value.ToString(nfid) + "º" : na,
                    Ignicao = (monitoramento.UltimaPosicao != null && monitoramento.UltimaPosicao?.Ignicao > 0),
                    Tecnologia = monitoramento.Veiculo?.TecnologiaRastreador?.Descricao,
                    NomeConta = monitoramento.Veiculo?.TecnologiaRastreador?.NomeConta,
                    NumeroEquipamento = monitoramento.Veiculo?.NumeroEquipamentoRastreador,
                    Rastreador = (monitoramento.UltimaPosicao != null && monitoramento.UltimaPosicao.DataVeiculo > DateTime.Now.AddMinutes(-configuracao.TempoSemPosicaoParaVeiculoPerderSinal)),
                    DescricaoRastreador = monitoramento.Carga?.Veiculo?.TecnologiaRastreador != null ? monitoramento.Carga.Veiculo.TecnologiaRastreador.Descricao : monitoramento.UltimaPosicao != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterDescricao(monitoramento.UltimaPosicao.Rastreador) : "",
                    StatusCodigo = (int)monitoramento.Status,
                    StatusDescricao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatusHelper.ObterDescricao(monitoramento.Status),
                    StatusViagemTipoRegra = monitoramento.StatusViagem?.TipoRegra,
                    StatusViagemCodigo = monitoramento.StatusViagem?.Codigo,
                    StatusViagemDescricao = monitoramento.StatusViagem?.Descricao.ToString() ?? na,
                    ChegadaAgendada = (monitoramento.Carga?.DataInicioViagemReprogramada != null) ? monitoramento.Carga?.DataInicioViagemReprogramada?.ToString(MASCARA_DATA_HM) : (monitoramento.Carga?.DataInicioViagemPrevista != null) ? monitoramento.Carga?.DataInicioViagemPrevista?.ToString(MASCARA_DATA_HM) : na,
                    TempoEstimado = (monitoramento.Carga != null && monitoramento.Carga?.DataFimViagemPrevista != null && monitoramento.Carga?.DataInicioViagemPrevista != null) ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(monitoramento.Carga.DataFimViagemPrevista.Value - monitoramento.Carga.DataInicioViagemPrevista.Value) : na,
                    TempoRealizado = (monitoramento.Carga != null && monitoramento.Carga?.DataFimViagem != null && monitoramento.Carga?.DataInicioViagem != null) ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(monitoramento.Carga.DataFimViagem.Value - monitoramento.Carga.DataInicioViagem.Value) : na,
                    ChegadaPrevistaInicial = monitoramento.Carga?.DataFimViagemPrevista?.ToString(MASCARA_DATA_HM) ?? na,
                    ChegadaPrevistaAtual = monitoramento.Carga?.DataFimViagemReprogramada?.ToString(MASCARA_DATA_HM) ?? na,
                    ChegadaReal = monitoramento.Carga?.DataFimViagem?.ToString(MASCARA_DATA_HM) ?? na,
                    Critico = monitoramento.Critico ? true : false,
                    NumeroMonitoramentoCarga = repMonitoramento.ContarMonitoramentosPorCarga(monitoramento.Carga?.Codigo ?? 0),
                    Observacao = monitoramento.Observacao,
                    MonitoramentoQualidade = (
                            from row in monitoramentoDadosSumarizados
                            select new
                            {
                                Regra = row.RegraQualidadeMonitoramento.Descricao,
                                row.Resultado
                            }).ToList()
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDetalhesMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoDestinos()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                dynamic rows = new List<dynamic>();
                if (carga != null)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
                    rows = (
                    from entrega in cargaEntregas
                    select new
                    {
                        Codigo = entrega?.Codigo,
                        Cliente = entrega?.Cliente?.Descricao,
                        Localizacao = !string.IsNullOrEmpty(entrega?.Cliente?.Latitude) && !string.IsNullOrEmpty(entrega?.Cliente?.Longitude) ? entrega?.Cliente?.Latitude.Replace(",", ".") + "," + entrega?.Cliente?.Longitude.Replace(",", ".") : "",
                        EntregaColeta = (entrega.Coleta) ? "C" : "E",
                        Ordem = entrega.Ordem,
                        OrdemRealizada = (entrega.DataConfirmacao != null) ? entrega.OrdemRealizada : -1,
                        Cidade = entrega?.Cliente?.Localidade?.Descricao + "/" + entrega?.Cliente?.Localidade?.Estado?.Sigla,
                        DistanciaAteDestino = (entrega?.DistanciaAteDestino != null) ? String.Format("{0:n1} Km", entrega.DistanciaAteDestino) : "",
                        DataPrevisaoChegada = entrega?.DataPrevista?.ToString(MASCARA_DATA_HM) ?? "",
                        DataEntregaReprogramada = entrega?.DataReprogramada?.ToString(MASCARA_DATA_HM) ?? "",
                        DataEntrada = entrega?.DataEntradaRaio?.ToString(MASCARA_DATA_HM) ?? "",
                        DataSaida = entrega?.DataSaidaRaio?.ToString(MASCARA_DATA_HM) ?? "",
                        EntregaNoPrazo = entrega.DescricaoEntregaNoPrazo
                    }).ToList();
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ordem, "Ordem", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.OrdemRealizada, "OrdemRealizada", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EntregaColeta, "EntregaColeta", 4, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Cliente, "Cliente", 23, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Localizacao, "Localizacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Cidade, "Cidade", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegada, "DataPrevisaoChegada", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegadaReprogramada, "DataEntregaReprogramada", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Distancia, "DistanciaAteDestino", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Entrada, "DataEntrada", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Saida, "DataSaida", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EntregaRealizadaNoPrazo, "EntregaNoPrazo", 4, Models.Grid.Align.center, false);
                grid.AdicionaRows(rows);
                grid.setarQuantidadeTotal(rows.Count);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDestinosDetalheMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoPedidos()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("carga");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.PedidosCargaMonitoramento>();
                if (carga != null)
                {
                    Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                    pedidos = repMonitoramento.ConsultarPedidos(carga.Codigo);
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Pedido, "Pedido", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Cliente, "NomeCliente", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NotaFiscal, "NotaFiscal", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Serie, "Serie", 10, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Chave, "Chave", 10, Models.Grid.Align.left, false);
                grid.AdicionaRows(pedidos);
                grid.setarQuantidadeTotal(pedidos.Count);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterPedidosDetalheMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoMonitoramento = Request.GetIntParam("Monitoramento");
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento;

                if (codigoMonitoramento > 0)
                    monitoramento = repositorioMonitoramento.BuscarPorCodigo(codigoMonitoramento);
                else
                    monitoramento = repositorioMonitoramento.BuscarUltimoPorCarga(codigoCarga);

                if (monitoramento == null)
                    return new JsonpResult(false, true, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                List<Dominio.Entidades.Veiculo> reboques = monitoramento.Carga?.VeiculosVinculados?.ToList() ?? new List<Dominio.Entidades.Veiculo>();
                Dominio.Entidades.Veiculo veiculo = monitoramento.Carga?.Veiculo;

                return new JsonpResult(new
                {
                    Codigo = monitoramento.Codigo,
                    Carga = monitoramento.Carga?.Codigo ?? 0,
                    CargaEmbarcador = monitoramento.Carga?.CodigoCargaEmbarcador ?? "",
                    DataInicioMonitoramento = monitoramento.DataInicio.HasValue ? monitoramento.DataInicio.Value.AddMinutes(-5).ToString("dd/MM/yyyy HH:mm") : "",
                    DataFimMonitoramento = monitoramento.DataFim.HasValue ? monitoramento.DataFim.Value.AddMinutes(5).ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                    Status = monitoramento.Status,
                    Tracao = monitoramento.Veiculo?.Placa,
                    Tecnologia = veiculo?.TecnologiaRastreador != null ? veiculo.TecnologiaRastreador.Descricao : monitoramento.UltimaPosicao != null ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreadorHelper.ObterDescricao(monitoramento.UltimaPosicao.Rastreador) : "",
                    Reboques = string.Join(", ", reboques.Select(reboque => reboque.Placa))
                }); ;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterHistoricoTemperatura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoHistoricoStatusViagem()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("codigo");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                dynamic rows = new List<dynamic>();
                if (monitoramento != null)
                {
                    Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem> historicos = repMonitoramentoHistoricoStatusViagem.BuscarPorMonitoramento(monitoramento);
                    if (historicos.Count > 0)
                    {
                        int codigoUltimo = historicos.Last().Codigo;
                        rows = (
                        from row in historicos
                        select new
                        {
                            row.Codigo,
                            CodigoMonitoramento = codigo,
                            Ultimo = row.Codigo == codigoUltimo,
                            TipoRegra = row.StatusViagem.TipoRegra,
                            Status = row.StatusViagem.Descricao,
                            Veiculo = row.Veiculo.Placa,
                            ClienteAlvo = row.Cliente?.Descricao,
                            SubareaAlvo = row.SubareaCliente?.Descricao,
                            DataInicio = row.DataInicio.ToString(MASCARA_DATA_HM),
                            DataFim = row.DataFim?.ToString(MASCARA_DATA_HM) ?? "",
                            DataPrevistaChegadaPlanta = monitoramento.Carga?.DataPrevisaoChegadaOrigem?.ToString(MASCARA_DATA_HM) ?? "",
                            Tempo = (row.TempoSegundos != null) ? row.Tempo.ToString() : ""
                        }).ToList();
                    }
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMonitoramento", false);
                grid.AdicionarCabecalho("Ultimo", false);
                grid.AdicionarCabecalho("TipoRegra", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "Status", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Veiculo, "Veiculo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataInicio, "DataInicio", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataFim, "DataFim", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tempo, "Tempo", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataPrevistaChegadaPlanta, "DataPrevistaChegadaPlanta", 10, Models.Grid.Align.left, false, false);
                grid.AdicionaRows(rows);
                grid.setarQuantidadeTotal(rows.Count);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterHistoricoStatusViagemMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoPermanenciaCliente()
        {

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPermanenciaCliente(unitOfWork);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuFalhaObterPermanenciasClientes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarGridPermanenciaCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPermanenciaCliente(unitOfWork);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhesMonitoramentoPermanenciaSubarea()
        {

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("carga");
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                dynamic rows = new List<dynamic>();
                if (carga != null)
                {
                    Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea> permanenciaSubareas = repPermanenciaSubarea.BuscarPorCarga(carga.Codigo);

                    rows = (
                    from row in permanenciaSubareas
                    select new
                    {
                        Codigo = row.Codigo,
                        Cliente = row.Subarea.Cliente?.Descricao ?? string.Empty,
                        Subarea = row.Subarea.Descricao,
                        TipoSubarea = row.Subarea.TipoSubarea.Descricao,
                        DataEntrada = row.DataInicio.ToString(MASCARA_DATA_HM),
                        DataSaida = row.DataFim?.ToString(MASCARA_DATA_HM),
                        Tempo = (row.DataFim != null) ? row.Tempo.ToString() : null
                    }).ToList();
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Cliente, "Cliente", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Subarea, "Subarea", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoDaSubarea, "TipoSubarea", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataEntrada, "DataEntrada", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataSaida, "DataSaida", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tempo, "Tempo", 15, Models.Grid.Align.left, false);
                grid.AdicionaRows(rows);
                grid.setarQuantidadeTotal(rows.Count);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterPermanenciasSubareas);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarStatusViagemSubareas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigo = Request.GetIntParam("codigo");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                dynamic rows = new List<dynamic>();
                if (monitoramento != null)
                {

                    Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem> statusViagem = repMonitoramentoStatusViagem.BuscarAtivos();

                    List<double> codigosClientes = ExtrairCodigosClientes(monitoramento.Carga);
                    Repositorio.Embarcador.Logistica.SubareaCliente repSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Logistica.SubareaCliente> subareas = repSubareaCliente.BuscarPorClientes(codigosClientes);

                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> entregasDestinos = repCargaEntrega.BuscarDestinosEntregasPorCarga(monitoramento.Carga.Codigo);

                    return new JsonpResult(new
                    {
                        StatusViagem = (
                            from row in statusViagem
                            select new
                            {
                                row.Codigo,
                                row.Descricao,
                                TiposSubareas = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.BuscarTiposSubareasRelacionadosAoStatus(row.TipoRegra),
                                Destino = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.StatusEstaRelacionadoAoDestino(row.TipoRegra),
                                EmPlanta = Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.StatusRelacionadoAPlanta(row.TipoRegra)
                            }),

                        Subarea = (
                            from row in subareas
                            select new
                            {
                                row.Codigo,
                                Descricao = row.Cliente != null ? row.Cliente.NomeCNPJ + ": " + row.Descricao + " (" + row.TipoSubarea.Descricao + ")" : string.Empty,
                                Tipo = (int)row.TipoSubarea.Tipo
                            }),

                        EntregaDestino = (
                            from row in entregasDestinos
                            select new
                            {
                                row.Codigo,
                                Descricao = row.Cliente.NomeCNPJ + " (" + row.Ordem + ")",
                            }),
                    });
                }
                else
                {
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                }

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterHistoricoTemperatura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarStatusViagem()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigo = Request.GetIntParam("Codigo");
                int codigoStatusViagem = Request.GetIntParam("StatusViagem");
                DateTime dataInicioStatus = Request.GetDateTimeParam("DataInicioStatus");
                bool indicarEntradaEmSubarea = Request.GetBoolParam("IndicarEntradaEmSubarea");
                int codigoEntregaDestino = Request.GetIntParam("EntregaDestino");
                int codigoSubarea = Request.GetIntParam("Subarea");
                DateTime? dataInicioSubarea = Request.GetNullableDateTimeParam("DataInicioSubarea");
                DateTime? dataFimSubarea = Request.GetNullableDateTimeParam("DataFimSubarea");

                DateTime? dataPrevisaoChegaPlanta = Request.GetNullableDateTimeParam("DataPrevisaoChegadaPlanta");

                if (dataInicioStatus == DateTime.MinValue) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.DataInicialNovoStatusNaoInformada);
                if (dataInicioStatus > DateTime.Now) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.DataInicialStatusNaoPodeSerMaiorDataHoraAtuais);

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado) return new JsonpResult(false, "Não é possível alterar um monitoramento finalizado ou cancelado.");

                Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoStatusViagem statusViagem = repMonitoramentoStatusViagem.BuscarPorCodigo(codigoStatusViagem, false);
                if (statusViagem == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.StatusViagemNaoEcontrado);

                DateTime? dataInicial = null;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = null;
                Dominio.Entidades.Embarcador.Logistica.SubareaCliente subarea = null;
                if (indicarEntradaEmSubarea)
                {
                    if (dataInicioSubarea == null || (dataFimSubarea != null && dataInicioSubarea > dataFimSubarea))
                        return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.DevemSerInformadasSubareasDataInicialFinalCorretamente);

                    dataInicial = dataInicioSubarea;

                    List<double> codigosClientes = ExtrairCodigosClientes(monitoramento.Carga);
                    Repositorio.Embarcador.Logistica.SubareaCliente repSubareaCliente = new Repositorio.Embarcador.Logistica.SubareaCliente(unitOfWork);
                    subarea = repSubareaCliente.BuscarPorCodigo(codigoSubarea, false);
                    if (subarea == null || !codigosClientes.Contains(subarea.Cliente.CPF_CNPJ))
                        return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.SubareaNaoEncontrada);

                    cargaEntrega = (from row in monitoramento.Carga.Entregas where row.Cliente.CPF_CNPJ == subarea.Cliente.CPF_CNPJ select row).FirstOrDefault();
                }

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();

                unitOfWork.Start();
                try
                {
                    bool alterado = false;
                    // Altera o status do monitoramento
                    try
                    {
                        Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.AlterarStatusViagem(unitOfWork, monitoramento, statusViagem, dataInicioStatus, base.Auditado, configuracao, TipoServicoMultisoftware, Cliente, configuracaoControleEntrega);
                        alterado = true;
                    }
                    catch (Exception e)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, e.Message);
                    }

                    if (dataPrevisaoChegaPlanta != null && dataPrevisaoChegaPlanta != DateTime.MinValue)
                    {
                        monitoramento.Carga.DataPrevisaoChegadaOrigem = dataPrevisaoChegaPlanta;
                        repCarga.Atualizar(monitoramento.Carga);

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramento.Carga, Localization.Resources.Logistica.Monitoramento.AlterouDataPrevisaoChegadaPlantaMonitoramentoAlterarStatusViagem, unitOfWork);
                    }


                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                    Repositorio.Embarcador.Logistica.PermanenciaSubarea repPermanenciaSubarea = new Repositorio.Embarcador.Logistica.PermanenciaSubarea(unitOfWork);

                    // Encerra permanência em subárea em aberto
                    Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea permanenciaSubarea = repPermanenciaSubarea.BuscarAbertaPorCarga(monitoramento.Carga);
                    if (permanenciaSubarea != null)
                    {
                        if (!dataInicial.HasValue) dataInicial = dataInicioStatus;
                        permanenciaSubarea.DataFim = (permanenciaSubarea.DataInicio < dataInicial.Value) ? dataInicial.Value : permanenciaSubarea.DataInicio;
                        permanenciaSubarea.TempoSegundos = (int)(permanenciaSubarea.DataFim.Value - permanenciaSubarea.DataInicio).TotalSeconds;
                        repPermanenciaSubarea.Atualizar(permanenciaSubarea);
                    }

                    if (indicarEntradaEmSubarea)
                    {
                        string descricaoAuditoria = "Indicada entrada";

                        // Inclusão de nova permanência na subárea
                        Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea novaPermanenciaSubarea = new Dominio.Entidades.Embarcador.Logistica.PermanenciaSubarea();
                        novaPermanenciaSubarea.Initialize();
                        novaPermanenciaSubarea.CargaEntrega = cargaEntrega;
                        novaPermanenciaSubarea.Subarea = subarea;
                        novaPermanenciaSubarea.DataInicio = dataInicioSubarea.Value;
                        if (dataFimSubarea != null)
                        {
                            descricaoAuditoria += " e saída ";
                            novaPermanenciaSubarea.DataFim = dataFimSubarea;
                            novaPermanenciaSubarea.TempoSegundos = (int)(dataFimSubarea.Value - dataInicioSubarea.Value).TotalSeconds;
                        }
                        repPermanenciaSubarea.Inserir(novaPermanenciaSubarea);

                        Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente = repPermanenciaCliente.BuscarAbertaPorCarga(monitoramento.Carga);
                        if (permanenciaCliente == null || permanenciaCliente.Cliente.Codigo != subarea?.Cliente?.Codigo)
                        {
                            Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente novaPermanenciaCliente = new Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente();
                            novaPermanenciaCliente.CargaEntrega = cargaEntrega;
                            novaPermanenciaCliente.Cliente = subarea.Cliente;
                            novaPermanenciaCliente.DataInicio = dataInicioSubarea.Value;
                            repPermanenciaCliente.Inserir(novaPermanenciaCliente);
                        }
                        if (permanenciaCliente != null && permanenciaCliente.Cliente.Codigo != subarea?.Cliente?.Codigo)
                        {
                            permanenciaCliente.DataFim = dataInicioSubarea.Value;
                            permanenciaCliente.TempoSegundos = (int)(permanenciaCliente.DataFim.Value - permanenciaCliente.DataInicio).TotalSeconds;
                            repPermanenciaCliente.Atualizar(permanenciaCliente);
                        }

                        // Auditoria
                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                            Usuario = base.Usuario
                        };
                        Servicos.Auditoria.Auditoria.Auditar(auditado, monitoramento, novaPermanenciaSubarea.GetChanges(), descricaoAuditoria + Localization.Resources.Logistica.Monitoramento.deSubarea, unitOfWork);

                        alterado = true;
                    }

                    if (codigoEntregaDestino > 0)
                    {
                        cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoEntregaDestino);
                        if (cargaEntrega != null && cargaEntrega.Carga.Codigo == monitoramento.Carga.Codigo)
                        {
                            if (!cargaEntrega.DataInicio.HasValue)
                            {
                                cargaEntrega.DataInicio = dataInicioStatus;
                                if (!cargaEntrega.DataEntradaRaio.HasValue)
                                {
                                    cargaEntrega.DataEntradaRaio = dataInicioStatus;
                                    cargaEntrega.DataLimitePermanenciaRaio = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.CalcularPermanenciaMaxima(cargaEntrega, unitOfWork);
                                }
                                cargaEntrega.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.EmCliente;
                                repCargaEntrega.Atualizar(cargaEntrega);
                                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                                Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaCliente = repPermanenciaCliente.BuscarAbertaPorCarga(monitoramento.Carga);
                                if (permanenciaCliente == null || permanenciaCliente.Cliente.Codigo != subarea?.Cliente?.Codigo)
                                {
                                    Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente novaPermanenciaCliente = new Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente();
                                    novaPermanenciaCliente.CargaEntrega = cargaEntrega;
                                    novaPermanenciaCliente.Cliente = cargaEntrega.Cliente;
                                    novaPermanenciaCliente.DataInicio = dataInicioStatus;
                                    repPermanenciaCliente.Inserir(novaPermanenciaCliente);
                                }
                                if (permanenciaCliente != null && permanenciaCliente.Cliente.Codigo != subarea?.Cliente?.Codigo)
                                {
                                    permanenciaCliente.DataFim = dataInicioStatus;
                                    permanenciaCliente.TempoSegundos = (int)(permanenciaCliente.DataFim.Value - permanenciaCliente.DataInicio).TotalSeconds;
                                    repPermanenciaCliente.Atualizar(permanenciaCliente);
                                }
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.Entregainvalida);
                        }
                    }

                    if (alterado)
                    {
                        unitOfWork.CommitChanges();
                        return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.StatusViagemAlteradoComSucesso);
                    }
                    else
                    {
                        return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.OStatusFicouIgualAoAnterior);
                    }

                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                    unitOfWork.Rollback();
                    throw;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarStatusViagemMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarHistoricoStatusViagem()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas(_urlController);
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento))
                return new JsonpResult(false, true, Localization.Resources.Logistica.Monitoramento.VoceNaoPossuiPermissaoParaExecutarEssaAcao);

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {

                int codigoMonitoramento = Request.GetIntParam("CodigoMonitoramento");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                int codigoHistoricoStatusViagem = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorCodigo(codigoHistoricoStatusViagem, false);

                DateTime dataInicioStatus = Request.GetDateTimeParam("DataInicioStatus");

                DateTime? dataPrevisaoChegaPlanta = Request.GetNullableDateTimeParam("DataPrevisaoChegadaPlanta");

                try
                {
                    Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.AlterarHistoricoStatusViagem(unitOfWork, monitoramento, historicoStatusViagem, dataInicioStatus, base.Auditado);
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, excecao.Message);
                }

                if (dataPrevisaoChegaPlanta != null && dataPrevisaoChegaPlanta != DateTime.MinValue)
                {
                    monitoramento.Carga.DataPrevisaoChegadaOrigem = dataPrevisaoChegaPlanta;
                    repCarga.Atualizar(monitoramento.Carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramento.Carga, Localization.Resources.Logistica.Monitoramento.AlterouDataPrevisaoChegadaPlantaMonitoramentoEditarStatusViagem, unitOfWork);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.HistoricoStatusViagemAlteradoComSucesso);

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarStatusViagemMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarEntradaSaidaRaioCliente()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas(_urlController);
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Monitoramento_InformarDataEntradaSaidaRaio))
                return new JsonpResult(false, true, Localization.Resources.Logistica.Monitoramento.VoceNaoPossuiPermissaoParaExecutarEssaAcao);

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                DateTime DataEntradaRaio = Request.GetDateTimeParam("DataEntradaRaio");
                DateTime? DataSaidaRaio = Request.GetNullableDateTimeParam("DataSaidaRaio");

                if (DataSaidaRaio != null && DataSaidaRaio < DataEntradaRaio)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoMensagemErroHorarioInvalido);

                int codigoEntrega = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoEntrega, false);
                Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente permanenciaClienteCadastrada = null;

                List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasCliente = repPermanenciaCliente.BuscarPorCargaEntrega(cargaEntrega);
                if (permanenciasCliente != null && permanenciasCliente.Count > 0)
                    permanenciaClienteCadastrada = permanenciasCliente.Last();

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.DestinoEntregaNaoEncontrado);

                try
                {

                    if (permanenciaClienteCadastrada == null)
                    {
                        permanenciaClienteCadastrada = new Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente();
                        permanenciaClienteCadastrada.CargaEntrega = cargaEntrega;
                        permanenciaClienteCadastrada.Cliente = cargaEntrega.Cliente;
                    }

                    permanenciaClienteCadastrada.DataFim = DataSaidaRaio;
                    permanenciaClienteCadastrada.DataInicio = DataEntradaRaio;
                    permanenciaClienteCadastrada.TempoSegundos = DataSaidaRaio != null ? (int?)(permanenciaClienteCadastrada.DataFim.Value - permanenciaClienteCadastrada.DataInicio).TotalSeconds : (int?)null;

                    cargaEntrega.DataEntradaRaio = DataEntradaRaio;
                    cargaEntrega.DataSaidaRaio = DataSaidaRaio;
                    cargaEntrega.Carga.DataAtualizacaoCarga = DateTime.Now;

                    repositorioCarga.Atualizar(cargaEntrega.Carga);
                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                    if (permanenciaClienteCadastrada.Codigo > 0)
                        repPermanenciaCliente.Atualizar(permanenciaClienteCadastrada);
                    else
                        repPermanenciaCliente.Inserir(permanenciaClienteCadastrada);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, Localization.Resources.Logistica.Monitoramento.AlterouDataEntradaRaioSaidaRaioMonitoramento, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, permanenciaClienteCadastrada, Localization.Resources.Logistica.Monitoramento.AlterouDataEntradaRaioSaidaRaioMonitoramento, unitOfWork);

                    new Servicos.Embarcador.Carga.ControleEntrega.ControleEntregaQualidade(unitOfWork, null).ProcessarRegrasDeQualidadeDeEntrega(cargaEntrega);
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, excecao.Message);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.HistoricoPermanenciaClienteAlteradoComSucesso);

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarPermanenciaClienteMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarNumeroRastreador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);

                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
                string novoNumeroRastreador = Request.GetStringParam("NumeroRastreador");

                Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                if (veiculo == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.VeiculoNaoEncontrado);

                veiculo.NumeroEquipamentoRastreador = novoNumeroRastreador;

                repositorioVeiculo.Atualizar(veiculo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, veiculo, string.Format(Localization.Resources.Logistica.Monitoramento.AlterouNumeroRastreadorVeiculo, veiculo.Placa, veiculo.NumeroEquipamentoRastreador), unitOfWork);

                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.NumeroRastreadorAlteradoComSucesso);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarNumeroRastreador);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarDatasPrevisoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    throw new ControllerException(Localization.Resources.Pedidos.Pedido.AcaoNaoEPermitida);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                int CodigoProximaEntrega = Request.GetIntParam("CodigoProximaEntrega");
                int CodigoEntrega = Request.GetIntParam("CodigoEntrega");
                DateTime novaDataPrevisaoReprogramada = Request.GetDateTimeParam("DataPrevisaoReprogramada");
                DateTime novaDataPrevisaoPlanejada = Request.GetDateTimeParam("DataPrevisaoPlanejada");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(CodigoEntrega > 0 ? CodigoEntrega : CodigoProximaEntrega);

                if (cargaEntrega == null)
                    return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.CargaNaoEncontrada);

                if (novaDataPrevisaoReprogramada != DateTime.MinValue)
                    cargaEntrega.DataReprogramada = novaDataPrevisaoReprogramada;

                if (novaDataPrevisaoPlanejada != DateTime.MinValue)
                    cargaEntrega.DataPrevista = novaDataPrevisaoPlanejada;

                repositorioCargaEntrega.Atualizar(cargaEntrega);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);


                if (novaDataPrevisaoReprogramada != DateTime.MinValue)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, "Data Previsao Entrega Reprogramada alterada Manualmente: " + cargaEntrega.DataReprogramada.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);

                if (novaDataPrevisaoPlanejada != DateTime.MinValue)
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, "Data Previsao Entrega Planejada alterada Manualmente: " + cargaEntrega.DataPrevista.Value.ToString("dd/MM/yyyy HH:mm:ss"), unitOfWork);

                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.AlterouDataPrevisoesComSucesso);
            }
            catch (ControllerException excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarDataPrevisoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExcluirHistoricoStatusViagem()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas(_urlController);
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Monitoramento_AlterarOuExcluirHistoricoDeStatusDoMonitoramento))
                return new JsonpResult(false, true, Localization.Resources.Logistica.Monitoramento.VoceNaoPossuiPermissaoParaExecutarEssaAcao);

            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {

                int codigoMonitoramento = Request.GetIntParam("CodigoMonitoramento");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                int codigoHistoricoStatusViagem = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem repMonitoramentoHistoricoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoHistoricoStatusViagem historicoStatusViagem = repMonitoramentoHistoricoStatusViagem.BuscarPorCodigo(codigoHistoricoStatusViagem, false);

                try
                {
                    Servicos.Embarcador.Monitoramento.MonitoramentoStatusViagem.ExcluirHistoricoStatusViagem(unitOfWork, monitoramento, historicoStatusViagem, base.Auditado);
                }
                catch (Exception excecao)
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, excecao.Message);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.HistoricoStatusViagemExcluidoSucesso);

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarStatusViagemMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> MonitoramentoCritico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigoMonitoramento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);
                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado) return new JsonpResult(false, "Monitoramento já encerrado");

                bool critico = Request.GetBoolParam("Critico");
                string mensagem = (critico) ? Localization.Resources.Logistica.Monitoramento.MonitoramentoMarcadoComoCritico : Localization.Resources.Logistica.Monitoramento.RemovidaCriticidadeDoMonitoramento;

                monitoramento.Initialize();
                monitoramento.Critico = critico;
                repMonitoramento.Atualizar(monitoramento);
                Servicos.Auditoria.Auditoria.Auditar(base.Auditado, monitoramento, monitoramento.GetChanges(), mensagem, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, mensagem);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarStatusViagemMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> SalvarObservacaoMonitoramento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {
                int codigoMonitoramento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);
                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado) return new JsonpResult(false, "Monitoramento já encerrado");

                string Observacao = Request.GetStringParam("Observacao");

                monitoramento.Initialize();
                monitoramento.Observacao = Observacao;
                repMonitoramento.Atualizar(monitoramento);
                Servicos.Auditoria.Auditoria.Auditar(base.Auditado, monitoramento, monitoramento.GetChanges(), "Atualizado observação monitoramento", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.ObservacaoAlteradaComSucesso);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAlterarObservacaoMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AllowAuthenticate]
        public async Task<IActionResult> FinalizarMonitoramentoManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            unitOfWork.Start();
            try
            {

                int codigoMonitoramento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigoMonitoramento);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                if (monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Cancelado || monitoramento.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado) return new JsonpResult(false, "Monitoramento já encerrado");

                string msg = "Monitoramento finalizado manualmente pela pagina de Monitoramento";
                Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(monitoramento, DateTime.Now, configuracaoTMS, base.Auditado, msg, unitOfWork, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MotivoFinalizacaoMonitoramento.FinalizacaoManual);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, Localization.Resources.Logistica.Monitoramento.FinalizadoComSucesso);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoFinalizarMonitoramento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDadosMapa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoMonitoramento = Request.GetIntParam("codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                int codigoVeiculo = Request.GetIntParam("Veiculo");
                return base.DadosMapa(unitOfWork, codigoMonitoramento, codigoCarga, codigoVeiculo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterDadosMapa);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoPosicao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridHistoricoPosicao(unitOfWork);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoObterHistoricoPosicoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoVeiculos()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                if (monitoramento == null) return null;

                Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo> monitoramentoVeiculo = repMonitoramentoVeiculo.BuscarTodosPorMonitoramento(monitoramento.Codigo);

                dynamic monitoramentoVeiculos = (
                    from r in monitoramentoVeiculo
                    select new
                    {
                        r.Codigo,
                        r.Veiculo.Placa,
                        DataInicio = r.DataInicio.ToString(MASCARA_DATA_HMS),
                        DataFim = (r.DataFim != null) ? r.DataFim.Value.ToString(MASCARA_DATA_HMS) : "",
                        Distancia = String.Format("{0:n1} Km", (r.Distancia != null) ? r.Distancia : 0),
                    }
                ).ToList();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Placa, "Placa", 25, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataIncial, "DataInicio", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataFinal, "DataFim", 25, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Distancia, "Distancia", 25, Models.Grid.Align.right, false);
                grid.AdicionaRows(monitoramentoVeiculos);
                grid.setarQuantidadeTotal(monitoramentoVeiculos.Count);
                return new JsonpResult(grid);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterHistoricoVeiculos);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarHistoricoPosicao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridHistoricoPosicao(unitOfWork);
                if (grid == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaGerarArquivoPosicoes);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaExportarHistoricoPosicoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterParadas()
        {
            try
            {
                return new JsonpResult(ObterGridParadas());
            }
            catch
            {
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterParadas);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarParadas()
        {
            try
            {
                var grid = ObterGridParadas();
                byte[] arquivoBinario = grid.GerarExcel();
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaGerarArquivoParadas);
            }
            catch
            {
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterParadas);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoTemperatura()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool PosicoesComTemperatura = Request.GetBoolParam("PosicoesValidas");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = ConsultarPosicoes(unitOfWork, monitoramento, PosicoesComTemperatura);
                dynamic temperaturas = (
                    from posicao in posicoesVeiculo
                    select new
                    {
                        Data = posicao.DataVeiculo.ToString(MASCARA_DATA_HMS),
                        posicao.Temperatura,
                        posicao.Placa,
                        TemperaturaDescricao = (posicao.Temperatura > 0) ? posicao.Temperatura + "°C" : ""
                    }
                ).ToList();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(monitoramento.Carga.Codigo);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = carga?.TipoDeCarga?.FaixaDeTemperatura;

                return new JsonpResult(new
                {
                    Temperaturas = temperaturas,
                    FaixaInicial = (faixaTemperatura != null) ? faixaTemperatura?.FaixaInicial : null,
                    FaixaFinal = (faixaTemperatura != null) ? faixaTemperatura?.FaixaFinal : null,
                });

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterHistoricoTemperatura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHistoricoVelocidade()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                if (monitoramento == null) return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.MonitoramentoNaoEncontrado);

                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = ConsultarPosicoes(unitOfWork, monitoramento);
                dynamic velocidades = (
                    from posicao in posicoesVeiculo
                    select new
                    {
                        Data = posicao.DataVeiculo.ToString(MASCARA_DATA_HMS),
                        posicao.Velocidade,
                        posicao.Placa,
                        VelocidadeDescricao = (posicao.Velocidade > 0) ? posicao.Temperatura + "Km/h" : ""
                    }
                ).ToList();

                return new JsonpResult(velocidades);

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaObterHistoricoTemperatura);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> TrocarMonitoramentoCargaCancelada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            try
            {
                int codigoCargaCancelada = Request.GetIntParam("CargaCancelada"); //deve herdar o monitoramento dessa carga para a carga de destino.
                int codigoCargaDestino = Request.GetIntParam("CargaDestino");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga cargaCancelada = repCarga.BuscarPorCodigoFetch(codigoCargaCancelada);
                Dominio.Entidades.Embarcador.Cargas.Carga cargaNova = repCarga.BuscarPorCodigoFetch(codigoCargaDestino);

                Servicos.Embarcador.Monitoramento.Monitoramento.TrocarCarga(cargaCancelada, cargaNova, Localization.Resources.Logistica.Monitoramento.TrocaDeCargaManuamenteLogisticaMonitoramento, configuracao, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaNova, Localization.Resources.Logistica.Monitoramento.TrocaMonitoramentoCargaManualmenteLogisticaMonitoramento, unitOfWork);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Logistica.Monitoramento.OcorreuUmaFalhaAoVincularMonitoramentoCarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarPosicaoManualmente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            unitOfWork.Start();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracoesControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracoesControleEntrega.ObterConfiguracaoPadrao();

                int codigoVeiculo = Request.GetIntParam("CodigoVeiculo");
                int codigo = Request.GetIntParam("Codigo");
                DateTime DataPosicao = Request.GetDateTimeParam("Data");
                double Latitude = Request.GetDoubleParam("Latitude");
                double Longitude = Request.GetDoubleParam("Longitude");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao repMonitoramentoVeiculoPosicao = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculoPosicao(unitOfWork);
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                Repositorio.Veiculo repveiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Logistica.MonitoramentoVeiculo repMonitoramentoVeiculo = new Repositorio.Embarcador.Logistica.MonitoramentoVeiculo(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoVeiculo monitoramentoVeiculo = repMonitoramentoVeiculo.BuscarAbertoPorMonitoramento(monitoramento.Codigo);

                Dominio.Entidades.Veiculo veiculo = repveiculo.BuscarPorCodigo(codigoVeiculo);
                Dominio.Entidades.Embarcador.Logistica.Posicao novaPosicao = new Dominio.Entidades.Embarcador.Logistica.Posicao
                {
                    Data = DataPosicao,
                    DataVeiculo = DataPosicao,
                    IDEquipamento = veiculo.Codigo.ToString(),
                    Veiculo = veiculo,
                    DataCadastro = DateTime.Now,
                    Processar = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProcessarPosicao.Pendente,
                    EmAlvo = false,
                    Descricao = $"{Latitude} - {Longitude}",
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Rastreador = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EnumTecnologiaRastreador.Manual
                };

                repPosicao.Inserir(novaPosicao);

                CalcularPrevisoesEntregas(unitOfWork, monitoramento, novaPosicao, configuracao, configuracaoControleEntrega);
                Servicos.Embarcador.Monitoramento.Carga.DesvincularCavaloNaCargaComStatusEmParqueamento(monitoramento, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(base.Auditado, monitoramento, monitoramento.GetChanges(), $"Adicionou nova posição ao Monitoramento Veículo: {novaPosicao.Veiculo.Placa} - Data: {novaPosicao.Data}", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Posição adicionada com sucesso");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao adicionar nova posição");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterFaixasTemperatura()
        {
            try
            {

                return new JsonpResult(ObterGridPesquisaFaixasTemperatura());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AlterarFaixaTemperatura()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            unitOfWork.Start();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracoesControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracoesControleEntrega.ObterConfiguracaoPadrao();

                int codigo = Request.GetIntParam("Codigo");
                int codigoMonitoramento = Request.GetIntParam("CodigoMonitoramento");
                int codigoFaixaTemperatura = Request.GetIntParam("FaixaTemperatura");

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = repMonitoramento.BuscarFaixaTemperaturaPorCodigo(codigoFaixaTemperatura);

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> alteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();
                alteracoes.Add(new Dominio.Entidades.Auditoria.HistoricoPropriedade("FaixaTemperatura", monitoramento.Carga.FaixaTemperatura?.Descricao, faixaTemperatura.Descricao));

                monitoramento.Carga.FaixaTemperatura = faixaTemperatura;

                repMonitoramento.Atualizar(monitoramento);

                Servicos.Auditoria.Auditoria.Auditar(base.Auditado, monitoramento, alteracoes, "Monitoramento atualizado. Alterou a faixa de temperatura", unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(true, "Faixa de temperatura alterada com sucesso");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Falha ao alterar faixa de temperatura");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados
        private Models.Grid.Grid ObterGridPermanenciaCliente(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCarga = Request.GetIntParam("carga");
            int codigoEntrega = Request.GetIntParam("entrega");
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
            dynamic rows = new List<dynamic>();
            if (carga != null)
            {
                Repositorio.Embarcador.Logistica.PermanenciaCliente repPermanenciaCliente = new Repositorio.Embarcador.Logistica.PermanenciaCliente(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.PermanenciaCliente> permanenciasClientes = repPermanenciaCliente.BuscarPorCarga(carga.Codigo);
                if (codigoEntrega > 0)
                    permanenciasClientes = permanenciasClientes.Where(p => p.CargaEntrega.Codigo == codigoEntrega).ToList();

                rows = (
                    from row in permanenciasClientes
                    select new
                    {
                        Codigo = row.Codigo,
                        Cliente = row.Cliente.Descricao,
                        DataEntrada = row.DataInicio.ToString(MASCARA_DATA_HM),
                        DataSaida = row.DataFim?.ToString(MASCARA_DATA_HM),
                        Tempo = (row.DataFim != null) ? row.Tempo.ToString() : null
                    }
                ).ToList();
            }

            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Cliente, "Cliente", 55, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataEntrada, "DataEntrada", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataSaida, "DataSaida", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tempo, "Tempo", 15, Models.Grid.Align.left, false);
            grid.AdicionaRows(rows);
            grid.setarQuantidadeTotal(rows.Count);

            return grid;
        }
        private void CalcularPrevisoesEntregas(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, Dominio.Entidades.Embarcador.Logistica.Posicao posicao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega)
        {
            Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega servicoPrevisaoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.PrevisaoControleEntrega(unitOfWork, configuracao);
            Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

            Dominio.Entidades.Embarcador.Cargas.CargaRotaFrete cargaRotaFrete = repCargaRotaFrete.BuscarPorCarga(monitoramento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> CargaEntregas = repCargaEntrega.BuscarPorCarga(monitoramento.Carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas((from o in CargaEntregas select o.Codigo).ToList());

            List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntrega> previsoesCargaEntrega = servicoPrevisaoControleEntrega.CalcularPrevisoesEntregasComPosicao(monitoramento.Carga, cargaRotaFrete, CargaEntregas, cargaEntregaPedidos, posicao, configuracaoIntegracao, configuracaoControleEntrega);

            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarPrevisaoCargaEntrega(previsoesCargaEntrega, cargaEntregaPedidos, monitoramento.Carga, CargaEntregas, false, true, configuracao, unitOfWork, TipoServicoMultisoftware);
        }

        private Models.Grid.Grid ObterGridResumoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ordem, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Carga, "Codigo", 5, Models.Grid.Align.left, true);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Filial, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Filial, "Filial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Placa, "Placa", 4, Models.Grid.Align.left, false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Transportador, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Transportador, "Transportador", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tipo, "Tipo", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TipoBau, "TipoBau", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Cliente, "Cliente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.EntregaColeta, "EntregaColeta", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.InicioViagem, "DataInicioViagem", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegada, "DataPrevisaoChegada", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.PrevisaoChegadaReprogramada, "DataEntregaReprogramada", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Entrada, "DataEntrada", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Saida, "DataSaida", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.NumeroPedidoEmbarcador, "NumeroPedidoEmbarcador", 9, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarResumoEntregas);

                var totalRegistros = repCargaEntrega.ContarConsultaResumoEntregas(carga.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = totalRegistros > 0 ? repCargaEntrega.ConsultarResumoEntregas(carga.Codigo, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega>();

                var listaRetornar = (
                from cargaEntrega in cargaEntregas
                select new
                {
                    cargaEntrega.Ordem,
                    Codigo = carga.CodigoCargaEmbarcador,
                    Filial = carga.Filial?.Descricao,
                    Placa = carga?.Veiculo?.Placa ?? "",
                    Transportador = carga?.Empresa != null ? carga?.Empresa?.Descricao : string.Empty,
                    Tipo = carga?.Veiculo?.ModeloVeicularCarga?.Descricao ?? "",
                    TipoBau = carga?.Veiculo?.ModeloCarroceria?.Descricao,
                    Cliente = cargaEntrega?.Cliente?.Descricao,
                    EntregaColeta = (cargaEntrega.Coleta) ? Localization.Resources.Logistica.Monitoramento.Coleta : Localization.Resources.Logistica.Monitoramento.Entrega,
                    DataInicioViagem = carga.DataInicioViagem?.ToString(MASCARA_DATA_HM) ?? "",
                    DataPrevisaoChegada = cargaEntrega.DataPrevista.HasValue ? cargaEntrega.DataPrevista.Value.ToString(MASCARA_DATA_HM) : BuscarDataPrevistaPedido(cargaEntrega.Codigo, unitOfWork),
                    DataEntregaReprogramada = cargaEntrega.DataReprogramada.HasValue ? cargaEntrega.DataReprogramada.Value.ToString(MASCARA_DATA_HM) : BuscarDataPrevistaPedido(cargaEntrega.Codigo, unitOfWork),
                    DataEntrada = cargaEntrega?.DataEntradaRaio?.ToString(MASCARA_DATA_HM) ?? "",
                    DataSaida = cargaEntrega?.DataSaidaRaio?.ToString(MASCARA_DATA_HM) ?? "",
                    NumeroPedidoEmbarcador = BuscarNumerOPedido(cargaEntrega.Codigo, unitOfWork),
                }).ToList();


                grid.AdicionaRows(listaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        private string BuscarDataPrevistaPedido(int cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(cargaEntrega);

            DateTime dataPrevisao = cargaPedidos.Select(x => x.Pedido.PrevisaoEntrega.HasValue ? x.Pedido.PrevisaoEntrega.Value : DateTime.MinValue).FirstOrDefault();

            if (dataPrevisao != DateTime.MinValue)
                return dataPrevisao.ToString(MASCARA_DATA_HM);
            else
                return "";
        }

        private string BuscarNumerOPedido(int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarCargaPedidoPorCargaEntrega(codigoCarga);

            return cargaPedidos.Select(o => o.Pedido?.NumeroPedidoEmbarcador).FirstOrDefault() ?? "";
        }

        private string ObterDescricaoMonitoramentoEvento(List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta)
        {
            int total = monitoramentoEventos.Count;
            for (int i = 0; i < total; i++)
            {
                if (monitoramentoEventos[i].TipoAlerta == tipoAlerta)
                {
                    return monitoramentoEventos[i].Descricao;
                }
            }
            return string.Empty;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento repConfiguracaoMonitoramento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMonitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMonitoramento configuracaoMonitoramento = repConfiguracaoMonitoramento.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                unitOfWork.Start(System.Data.IsolationLevel.ReadUncommitted);

                GridMonitoramento gridMonitoramento = new GridMonitoramento();
                Models.Grid.Grid grid = gridMonitoramento.ObterGridComPesquisa(Request, this.Usuario.Codigo, filtrosPesquisa, configuracao, configuracaoMonitoramento, unitOfWork, this.TipoServicoMultisoftware);

                //unitOfWork.CommitChanges();

                //Repositorio.Embarcador.Preferencias.PreferenciaGrid repPreferenciaGrid = new Repositorio.Embarcador.Preferencias.PreferenciaGrid(unitOfWork);
                //Dominio.Entidades.Embarcador.Preferencias.PreferenciaGrid preferenciaGrid = repPreferenciaGrid.BuscarPorUsuarioUrlGrid(this.Usuario.Codigo, url: "Monitoramento/Pesquisa", grid: "grid-monitoramento");
                //grid.AplicarPreferenciasGrid(preferenciaGrid);
                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(8485);
                //Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(carga, ConfiguracaoEmbarcador, unitOfWork, TipoServicoMultisoftware);

                //Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                //Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);

                //Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(11362);
                //List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);
                //List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> cargaPedidosXMLs = repPedidoXmlNotaFiscal.BuscarPorCarga(carga.Codigo);
                //Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarControleEntregaSemRota(carga, cargaPedidos, cargaPedidosXMLs, true, configuracao, TipoServicoMultisoftware, unitOfWork);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.OcultarCabecalhos(new string[] { "Filial", "PossuiContratoFrete", "Transportador", "NumeroEXP" });

                return grid;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaTorreMonitoramento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = ObterFiltrosPesquisaTorre(unitOfWork);

                GridTorreMonitoramento gridMonitoramento = new GridTorreMonitoramento();
                Models.Grid.Grid grid = gridMonitoramento.ObterGridComPesquisa(Request, this.Usuario.Codigo, filtrosPesquisa, configuracao, unitOfWork);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "Monitoramento/PesquisaTorreMonitoramento", "grid-torre-monitoramento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.OcultarCabecalhos(new string[] { "Filial", "PossuiContratoFrete", "Transportador", "NumeroEXP" });

                return grid;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                DescricaoAlerta = Request.GetStringParam("DescricaoAlerta"),
                Status = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                CodigosStatusViagem = Request.GetListParam<int>("StatusViagem"),
                CodigosGrupoTipoOperacao = Request.GetListParam<int>("GrupoTipoOperacao"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                SomenteRastreados = Request.GetBoolParam("SomenteRastreados"),
                SomenteUltimoPorCarga = Request.GetBoolParam("SomenteUltimoPorCarga"),
                FiltroCliente = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente>("FiltroCliente"),
                Cliente = Request.GetDoubleParam("Cliente"),
                CodigoCategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                DataEntregaPedidoInicio = Request.GetDateTimeParam("DataEntregaPedidoInicio"),
                DataEntregaPedidoFinal = Request.GetDateTimeParam("DataEntregaPedidoFinal"),
                PrevisaoEntregaInicio = Request.GetDateTimeParam("PrevisaoEntregaInicio"),
                PrevisaoEntregaFinal = Request.GetDateTimeParam("PrevisaoEntregaFinal"),
                CodigosExpedidores = Request.GetListParam<long>("Expedidor"),
                VeiculosComContratoDeFrete = Request.GetBoolParam("VeiculosComContratoDeFrete"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestinos = Request.GetListParam<int>("Destino"),
                CodigoClienteDestino = Request.GetListParam<double>("ClienteDestino"),
                CodigoClienteOrigem = Request.GetListParam<double>("ClienteOrigem"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                DataEmissaoNFeFim = Request.GetDateTimeParam("DataEmissaoNFeFim"),
                DataEmissaoNFeInicio = Request.GetDateTimeParam("DataEmissaoNFeInicio"),
                CodigosResponsavelVeiculo = Request.GetListParam<int>("ResponsavelVeiculo"),
                CodigosCentroResultado = Request.GetListParam<int>("CentroResultado"),
                CodigosFronteiraRotaFrete = Request.GetListParam<double>("FronteiraRotaFrete"),
                CodigosPaisDestino = Request.GetListParam<int>("PaisDestino"),
                CodigosPaisOrigem = Request.GetListParam<int>("PaisOrigem"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                ApenasMonitoramentosCriticos = Request.GetBoolParam("ApenasMonitoramentosCriticos"),
                VeiculosEmLocaisTracking = Request.GetBoolParam("VeiculosEmLocaisTracking"),
                LocaisTracking = Request.GetListParam<int>("LocaisTracking"),
                DataInicioCarregamento = Request.GetDateTimeParam("DataInicioCarregamento"),
                DataFimCarregamento = Request.GetDateTimeParam("DataFimCarregamento"),
                PossuiRecebedor = Request.GetNullableBoolParam("PossuiRecebedor"),
                PossuiExpedidor = Request.GetNullableBoolParam("PossuiExpedidor"),
                Destinatario = Request.GetListParam<double>("Destinatario"),
                Recebedores = Request.GetListParam<double>("Recebedores"),
                CodigoCargaEmbarcadorMulti = Request.GetListParam<int>("CodigoCargaEmbarcadorMulti"),
                CodigosTiposTrecho = Request.GetListParam<int>("TipoTrecho"),
                InicioViagemPrevistaInicial = Request.GetDateTimeParam("InicioViagemPrevistaInicial"),
                InicioViagemPrevistaFinal = Request.GetDateTimeParam("InicioViagemPrevistaFinal"),
                CodigoMotorista = Request.GetIntParam("CodigoMotorista"),
                Remetente = Request.GetListParam<double>("Remetente"),
                ModalTransporte = Request.GetEnumParam("TipoCobrancaMultimodal", Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal.Nenhum),
            };

            if (this.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor && Usuario.ClienteFornecedor != null)
            {
                //para mostrar no portal cliente.
                filtrosPesquisa.CodigoClienteDestino = new List<double>();
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Dominio.Entidades.Cliente clienteFornecedor = repositorioCliente.BuscarPorCPFCNPJ(Usuario.ClienteFornecedor.CPF_CNPJ);
                List<Dominio.Entidades.Cliente> clientesCnpjRaiz = repositorioCliente.BuscarPorRaizCNPJ(clienteFornecedor.CPF_CNPJ_SemFormato.Substring(0, 8));
                filtrosPesquisa.FiltroCliente = Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente.ComColetaOuEntrega;

                if (clientesCnpjRaiz != null && clientesCnpjRaiz.Count > 1)
                {
                    foreach (Dominio.Entidades.Cliente cliente in clientesCnpjRaiz)
                        filtrosPesquisa.CodigoClienteDestino.Add(cliente.CPF_CNPJ);
                }
                else
                    filtrosPesquisa.CodigoClienteDestino.Add(Usuario.ClienteFornecedor.CPF_CNPJ);
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoAlerta))
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarAtivo(filtrosPesquisa.DescricaoAlerta);
                if (monitoramentoEvento != null)
                {
                    filtrosPesquisa.TipoAlerta = monitoramentoEvento.TipoAlerta;
                    filtrosPesquisa.CodigoEvento = monitoramentoEvento.Codigo;
                }
            }

            List<int> codigosFilial = Request.GetListParam<int>("Filial");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork);

            if (filtrosPesquisa.CodigosTipoOperacao == null || filtrosPesquisa.CodigosTipoOperacao.Count == 0)
                filtrosPesquisa.CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork); //codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            if (filtrosPesquisa.VeiculosEmLocaisTracking)
            {
                Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);

                if (filtrosPesquisa.LocaisTracking.Count > 0)
                    filtrosPesquisa.locais = repLocais.BuscarPorCodigos(filtrosPesquisa.LocaisTracking);
                else
                    filtrosPesquisa.locais = repLocais.BuscarTodos();
            }

            return filtrosPesquisa;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento ObterFiltrosPesquisaTorre(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramento()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                DescricaoAlerta = Request.GetStringParam("DescricaoAlerta"),
                Status = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus>("MonitoramentoStatus"),
                CodigosStatusViagem = Request.GetListParam<int>("StatusViagem"),
                CodigosGrupoTipoOperacao = Request.GetListParam<int>("GrupoTipoOperacao"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigoGrupoPessoa = Request.GetIntParam("GrupoPessoa"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                NumeroNotaFiscal = Request.GetIntParam("NumeroNotaFiscal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                SomenteRastreados = Request.GetBoolParam("SomenteRastreados"),
                FiltroCliente = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoFiltroCliente>("FiltroCliente"),
                Cliente = Request.GetDoubleParam("Cliente"),
                CodigoCategoriaPessoa = Request.GetIntParam("CategoriaPessoa"),
                CodigoFuncionarioVendedor = Request.GetIntParam("FuncionarioVendedor"),
                NumeroEXP = Request.GetStringParam("NumeroEXP"),
                DataEntregaPedidoInicio = Request.GetDateTimeParam("DataEntregaPedidoInicio"),
                DataEntregaPedidoFinal = Request.GetDateTimeParam("DataEntregaPedidoFinal"),
                PrevisaoEntregaInicio = Request.GetDateTimeParam("PrevisaoEntregaInicio"),
                PrevisaoEntregaFinal = Request.GetDateTimeParam("PrevisaoEntregaFinal"),
                CodigosExpedidores = Request.GetListParam<long>("Expedidor"),
                VeiculosComContratoDeFrete = Request.GetBoolParam("VeiculosComContratoDeFrete"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestinos = Request.GetListParam<int>("Destino"),
                CodigoClienteDestino = Request.GetListParam<double>("ClienteDestino"),
                CodigoClienteOrigem = Request.GetListParam<double>("ClienteOrigem"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                EstadosOrigem = Request.GetListParam<string>("EstadoOrigem"),
                EstadosDestino = Request.GetListParam<string>("EstadoDestino"),
                DataEmissaoNFeFim = Request.GetDateTimeParam("DataEmissaoNFeFim"),
                DataEmissaoNFeInicio = Request.GetDateTimeParam("DataEmissaoNFeInicio"),
                //DataInicioCarregamento = Request.GetDateTimeParam("DataInicioCarregamento"),
                //DataFimCarregamento = Request.GetDateTimeParam("DataFimCarregamento"),
            };

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.DescricaoAlerta))
            {
                Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga repCargaEvento = new Repositorio.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga configuracaoAlerta = repCargaEvento.BuscarAtivo(filtrosPesquisa.DescricaoAlerta);
                if (configuracaoAlerta != null)
                    filtrosPesquisa.TipoAlertaCarga = configuracaoAlerta.TipoCargaAlerta;
            }

            List<int> codigosFilial = Request.GetListParam<int>("Filial");
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            filtrosPesquisa.CodigosFilial = codigosFilial.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFilial;
            filtrosPesquisa.CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork);
            filtrosPesquisa.CodigosTipoOperacao = codigoTipoOperacao == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : new List<int>() { codigoTipoOperacao };

            return filtrosPesquisa;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private string ObterPropriedadeOrdenarResumoEntregas(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DataEntrada")
                return "DataInicioEntrega";

            if (propriedadeOrdenar == "DataSaida")
                return "DataEntrega";

            return propriedadeOrdenar;
        }

        private int PercentualViagem(string polilinhaPrevista, string polilinhaRealizada, decimal distanciaPrevista, Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus status)
        {
            decimal percentual = 0;

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoStatus.Finalizado)
            {
                percentual = 100;
            }
            else if (!string.IsNullOrWhiteSpace(polilinhaPrevista) && !string.IsNullOrWhiteSpace(polilinhaRealizada) && distanciaPrevista > 0)
            {

                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsPrevistos = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinhaPrevista);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint> wayPointsRealizados = Servicos.Embarcador.Logistica.Polilinha.Decodificar(polilinhaRealizada);
                if (wayPointsPrevistos.Count > 0 && wayPointsRealizados.Count > 0)
                {
                    decimal distanciaAproximadaRealizada = (decimal)Servicos.Embarcador.Logistica.Polilinha.CalculaDistanciaRealizadaAproximada(wayPointsPrevistos, wayPointsRealizados.Last());
                    distanciaAproximadaRealizada /= 1000; // m para km
                    if (distanciaAproximadaRealizada <= distanciaPrevista)
                    {
                        percentual = distanciaAproximadaRealizada * 100 / distanciaPrevista;
                    }
                    else
                    {
                        percentual = 99;
                    }
                }

            }

            return (int)Math.Truncate(percentual);

        }

        private Models.Grid.Grid GridHistoricoTemperatura()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Data, "Data", 15, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Localizacao, "Descricao", 40, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "Latitude", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "Longitude", 15, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Temperatura, "TemperaturaDescricao", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Temperatura", false);
            return grid;
        }

        private List<double> ExtrairCodigosClientes(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            List<double> codigosClientes = new List<double>();
            if (carga != null)
            {
                try
                {
                    codigosClientes = (from row in carga.Entregas select row.Cliente.CPF_CNPJ).ToList();
                }
                catch
                {
                }
            }
            return codigosClientes;
        }

        private string ObterCorAlerta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta, List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaEventos)
        {
            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = listaEventos.Where(o => o.TipoAlerta == tipoAlerta).FirstOrDefault();

            var cor = "#4da6ff";//Azul
            if (monitoramentoEvento != null && !string.IsNullOrEmpty(monitoramentoEvento.Cor))
                cor = monitoramentoEvento.Cor;

            return cor;
        }

        private string ObterCorAlertaTorre(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlertaCarga tipoAlerta, List<Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga> listaEventos)
        {
            Dominio.Entidades.Embarcador.Cargas.AlertaCarga.ConfiguracaoAlertaCarga eventoCarga = listaEventos.Where(o => o.TipoCargaAlerta == tipoAlerta).FirstOrDefault();

            var cor = "#4da6ff";//Azul
            if (eventoCarga != null && !string.IsNullOrEmpty(eventoCarga.Cor))
                cor = eventoCarga.Cor;

            return cor;
        }

        private Models.Grid.Grid ObterGridParadas()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Tipo, "Tipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Veiculo, "Placa", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Data, "Data", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "Latitude", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "Longitude", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Duracao, "Tempo", 10, Models.Grid.Align.left, false);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                if (monitoramento != null)
                {
                    dynamic paradas = base.ObterParadas(unitOfWork, monitoramento, configuracao);
                    grid.AdicionaRows(paradas);
                    grid.setarQuantidadeTotal(paradas.Count);
                }
                else
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada> paradas = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Parada>();
                    grid.AdicionaRows(paradas);
                    grid.setarQuantidadeTotal(0);
                }

                return grid;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> ConsultarPosicoes(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento, bool comTemperaturaValida = false, bool ApenasPosicoesMobile = false, bool ApenasPosicoesTecnologia = false)
        {
            // Datas limites da consulta. Restringe o período para, no máximo, o período do monitoramento
            DateTime? dataInicial = RestringirDataInicial(Request.GetNullableDateTimeParam("DataInicial"), monitoramento);
            DateTime? dataFinal = RestringirDataFinal(Request.GetNullableDateTimeParam("DataFinal"), monitoramento);

            bool posicoesMobile = Request.GetBoolParam("PosicaoMobile");
            bool posicoesTecnologia = Request.GetBoolParam("PosicaoTecnologia");

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao>();
            if (dataInicial != null)
            {
                Repositorio.Embarcador.Logistica.Posicao repPosicao = new Repositorio.Embarcador.Logistica.Posicao(unitOfWork);
                posicoesVeiculo = repPosicao.BuscarWaypointsPorMonitoramentoVeiculo(monitoramento.Codigo, null, dataInicial, dataFinal, comTemperaturaValida, posicoesMobile, posicoesTecnologia);
            }
            return posicoesVeiculo;
        }

        private Models.Grid.Grid ObterGridHistoricoPosicao(Repositorio.UnitOfWork unitOfWork)
        {
            int codigo = Request.GetIntParam("Codigo");

            bool posicoesMobile = Request.GetBoolParam("PosicaoMobile");
            bool posicoesTecnologia = Request.GetBoolParam("PosicaoTecnologia");

            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repMonitoramento.BuscarPorCodigo(codigo);
            if (monitoramento == null) return null;

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.Posicao> posicoesVeiculo = ConsultarPosicoes(unitOfWork, monitoramento, false, posicoesMobile, posicoesTecnologia);
            dynamic posicoes = (
                from posicao in posicoesVeiculo
                select new
                {
                    Data = posicao.DataVeiculo.ToString(MASCARA_DATA_HMS),
                    posicao.CodigoVeiculo,
                    posicao.Placa,
                    posicao.Descricao,
                    posicao.DataCadastro,
                    Latitude = !string.IsNullOrEmpty(posicao.Latitude.ToString()) ? posicao.Latitude.ToString("f6").Replace(",", ".") : "",
                    Longitude = !string.IsNullOrEmpty(posicao.Longitude.ToString()) ? posicao.Longitude.ToString("f6").Replace(",", ".") : "",
                    Ignicao = (posicao.Ignicao > 0) ? "Ligada" : "Desligada",
                    posicao.Velocidade,
                    VelocidadeDescricao = posicao.Velocidade + "Km/h",
                    posicao.Temperatura,
                    TemperaturaDescricao = (posicao.Temperatura != null && (posicao.SensorTemperatura.HasValue && posicao.SensorTemperatura.Value)) ? posicao.Temperatura + "°C" : "",
                }
            ).ToList();

            Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataDaPosicao, "Data", 15, Models.Grid.Align.left, false);

            if (Usuario.UsuarioMultisoftware || Usuario.UsuarioAtendimento)
                grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.DataIntegracao, "DataCadastro", 15, Models.Grid.Align.left, false);

            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Placa, "Placa", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Latitude, "Latitude", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Longitude, "Longitude", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.Ignicao, "Ignicao", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Velocidade", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.VelocidadeDescricao, "VelocidadeDescricao", 10, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("Temperatura", false);
            grid.AdicionarCabecalho(Localization.Resources.Logistica.Monitoramento.TemperaturaDescricao, "TemperaturaDescricao", 10, Models.Grid.Align.right, false);
            grid.AdicionaRows(posicoes);
            grid.setarQuantidadeTotal(posicoes.Count);
            return grid;

        }

        private DateTime? RestringirDataInicial(DateTime? dataInicial, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (dataInicial != null)
            {
                if (monitoramento.DataInicio != null)
                {
                    if (monitoramento.DataInicio > dataInicial) dataInicial = monitoramento.DataInicio.Value;
                }
                else
                {
                    if (monitoramento.DataCriacao > dataInicial) dataInicial = monitoramento.DataCriacao.Value;
                }
            }
            return dataInicial;
        }

        private DateTime? RestringirDataFinal(DateTime? dataFinal, Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento)
        {
            if (dataFinal != null)
            {
                if (monitoramento.DataFim != null && monitoramento.DataFim < dataFinal) dataFinal = monitoramento.DataFim.Value;
            }
            else
            {
                dataFinal = DateTime.Now;
            }
            return dataFinal;
        }

        private Grid ObterGridPesquisaFaixasTemperatura()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            string descricao = Request.Params("Descricao");

            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Descrição", "Descricao", 50, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Faixa de temperatura", "DescricaoVariancia", 25, Models.Grid.Align.left, true);
            Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = repMonitoramento.BuscarFaixasTemperatura(descricao);
            int totalRegistros = repMonitoramento.ContarConsulta(descricao);

            if (!string.IsNullOrEmpty(descricao) && totalRegistros <= 5)
            {
                List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> listaExata = (from p in faixasTemperatura where p.Descricao.ToUpper() == descricao.ToUpper() select p).ToList();
                if (listaExata.Count == 1)
                {
                    faixasTemperatura = listaExata;
                    totalRegistros = 1;
                }
            }

            grid.setarQuantidadeTotal(totalRegistros);

            dynamic lista = (from p in faixasTemperatura
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoVariancia
                             }).ToList();
            grid.AdicionaRows(lista);
            return grid;
        }
        #endregion
    }
}