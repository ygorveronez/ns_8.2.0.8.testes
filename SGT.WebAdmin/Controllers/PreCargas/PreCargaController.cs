using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.PreCargas
{
    [CustomAuthorize("PreCargas/PreCarga", "Logistica/FilaCarregamento")]
    public class PreCargaController : BaseController
    {
		#region Construtores

		public PreCargaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento repositorioConfiguracaoJanelaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoJanelaCarregamento configuracaoJanelaCarregamento = repositorioConfiguracaoJanelaCarregamento.BuscarPrimeiroRegistro();

                return new JsonpResult(new
                {
                    Configuracoes = new
                    {
                        configuracaoEmbarcador.DadosTransporteObrigatorioPreCarga,
                        configuracaoEmbarcador.PermiteAdicionarPreCargaManual,
                        configuracaoEmbarcador.TransportadorObrigatorioPreCarga,
                        configuracaoGeralCarga.UtilizarProgramacaoCarga,
                        configuracaoFilaCarregamento.DiasFiltrarDataProgramada,
                        configuracaoJanelaCarregamento.DisponibilizarCargaParaTransportadoresPorPrioridade
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterConfiguracoes);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracaoDadosParaTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadasPreCarga = ObterPermissoesPersonalizadas("PreCargas/PreCarga");

                return new JsonpResult(new
                {
                    Configuracoes = new
                    {
                        configuracaoEmbarcador.DadosTransporteObrigatorioPreCarga,
                        configuracaoEmbarcador.LocalCarregamentoObrigatorioPreCarga,
                        configuracaoEmbarcador.TransportadorObrigatorioPreCarga,
                        configuracaoGeralCarga.UtilizarProgramacaoCarga
                    },
                    PermissoesPersonalizadas = permissoesPersonalizadasPreCarga
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsConfiguracoesDosDadosDeTransporte);
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
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Numero, "Descricao", 20, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Filial, "Filial", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicularCarga", 25, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    NumeroPreCarga = Request.GetStringParam("PreCarga"),
                    SemCarregamento = Request.GetBoolParam("SemCarregamento"),
                    SemCarga = Request.GetBoolParam("SemCarga"),
                    SomentePreCargaAtiva = true
                };

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> listaPreCarga = repositorioPreCarga.Consultar(filtrosPesquisa, parametrosConsulta);
                int totalRegistros = repositorioPreCarga.ContarConsulta(filtrosPesquisa);

                var listaPreCargaRetornar = (
                    from preCarga in listaPreCarga
                    select new
                    {
                        preCarga.Codigo,
                        preCarga.Descricao,
                        Filial = preCarga.Filial?.Descricao,
                        ModeloVeicularCarga = preCarga.ModeloVeicularCarga?.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaPreCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaCargasParaVinculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaEmbarcador", false);
                grid.AdicionarCabecalho("CodigoTransportador", false);
                grid.AdicionarCabecalho("CodigoEmpresaReboque", false);
                grid.AdicionarCabecalho("CodigoEmpresaSegundoReboque", false);
                grid.AdicionarCabecalho("CodigoEmpresaTracao", false);
                grid.AdicionarCabecalho("CodigoReboque", false);
                grid.AdicionarCabecalho("CodigoSegundoReboque", false);
                grid.AdicionarCabecalho("CodigoTracao", false);
                grid.AdicionarCabecalho("DescricaoReboque", false);
                grid.AdicionarCabecalho("DescricaoSegundoReboque", false);
                grid.AdicionarCabecalho("TipoVeiculoTracao", false);
                grid.AdicionarCabecalho("CodigoFaixaTemperatura", false);
                grid.AdicionarCabecalho("CodigoMotorista", false);
                grid.AdicionarCabecalho("DescricaoMotorista", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.NumeroDaCarga, "Descricao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.TipoDaCarga, "TipoCarga", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.ModeloVeicular, "ModeloVeicular", 15, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Empresa/Filial", "Transportador", 15, Models.Grid.Align.left, false);
                else
                    grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Transportador, "Transportador", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Veiculo, "DescricaoTracao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Motorista, "Motorista", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.DataCarregamento, "DataCarregamento", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Carga.Situacao, "Situacao", 10, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "") propOrdenar = "";

                string numeroCarga = Request.Params("CodigoCargaEmbarcador");
                int.TryParse(Request.Params("Filial"), out int filial);
                int origem = 0;
                int destino = 0;
                int.TryParse(Request.Params("Transportador"), out int transportador);

                List<double> remetentes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Remetentes"));
                List<double> destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Destinatarios"));

                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime dataFinal);

                //List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.Consultar(dataInicial, dataFinal, numeroCarga, filial, origem, destino, transportador, remetentes, destinatarios, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.ConsultarParaPreCarga(dataInicial, dataFinal, numeroCarga, filial, origem, destino, transportador, remetentes, destinatarios, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int quantidade = repCarga.ContarConsultaParaPreCarga(dataInicial, dataFinal, numeroCarga, filial, origem, destino, transportador, remetentes, destinatarios);

                var listaCarga = (
                    from obj in cargas
                    select new
                    {
                        obj.Codigo,
                        obj.CodigoCargaEmbarcador,
                        CodigoTransportador = obj.Empresa?.Codigo ?? 0,
                        CodigoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(0)?.Codigo ?? 0,
                        CodigoSegundoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(1)?.Codigo ?? 0,
                        CodigoTracao = obj.Veiculo?.Codigo ?? 0,
                        DescricaoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(0)?.Placa ?? string.Empty,
                        DescricaoSegundoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? string.Empty,
                        DescricaoTracao = obj.Veiculo?.Placa ?? string.Empty,
                        CodigoEmpresaReboque = obj.VeiculosVinculados?.ElementAtOrDefault(0)?.Empresa?.Codigo ?? 0,
                        CodigoEmpresaSegundoReboque = obj.VeiculosVinculados?.ElementAtOrDefault(1)?.Empresa?.Codigo ?? 0,
                        CodigoEmpresaTracao = obj.Veiculo?.Empresa?.Codigo ?? 0,
                        Descricao = obj.CodigoCargaEmbarcador,
                        TipoCarga = obj.TipoDeCarga?.Descricao ?? string.Empty,
                        TipoVeiculoTracao = obj.Veiculo?.TipoVeiculo ?? "",
                        ModeloVeicular = obj.ModeloVeicularCarga?.Descricao ?? string.Empty,
                        Transportador = obj.Empresa?.Descricao ?? string.Empty,
                        Motorista = obj.NomeMotoristas ?? string.Empty,
                        DataCarregamento = obj.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Situacao = obj.DescricaoSituacaoCarga,
                        CodigoFaixaTemperatura = obj.FaixaTemperatura?.Codigo ?? 0,
                        CodigoMotorista = obj.Motoristas?.ElementAtOrDefault(0)?.Codigo ?? 0,
                        DescricaoMotorista = obj.Motoristas?.ElementAtOrDefault(0)?.Nome ?? string.Empty,
                    }
                ).ToList();

                grid.setarQuantidadeTotal(quantidade);
                grid.AdicionaRows(listaCarga);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoExportar);
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ValidaCargaSelecionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Carga");
                List<double> remetentes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Remetentes"));
                List<double> destinatarios = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(Request.Params("Destinatarios"));
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(true, false, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarCarga);

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                if (!configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    bool remetentesIncompativel = (from o in carga.DadosSumarizados.ClientesRemetentes where !remetentes.Contains(o.CPF_CNPJ) select o).Count() > 0;
                    bool quantidadeRemetenteIncompativel = (from o in carga.DadosSumarizados.ClientesRemetentes select o.CPF_CNPJ).Distinct().Count() == remetentes.Count;
                    bool destinatariosIncompativel = (from o in carga.DadosSumarizados.ClientesDestinatarios where !destinatarios.Contains(o.CPF_CNPJ) select o).Count() > 0;
                    bool quantidadeDestinatarioIncompativel = (from o in carga.DadosSumarizados.ClientesDestinatarios select o.CPF_CNPJ).Distinct().Count() == destinatarios.Count;

                    if (remetentesIncompativel || !quantidadeRemetenteIncompativel)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.OsRemetentesdoPrePlanejamentoNaoSaoCompativeisComACargaSelecionada);

                    if (destinatariosIncompativel || !quantidadeDestinatarioIncompativel)
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.OsDestinatariosdoPrePlanejamentoNaoSaoCompativeisComACargaSelecionada);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoValidarACarga);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarPreCargaManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS RepositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = RepositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = ObterPreCargaAdicionar(unitOfWork, configuracaoEmbarcador);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);

                repositorioPreCarga.Inserir(preCarga);

                AdicionarDestinosPreCargaManual(preCarga, unitOfWork);
                new Servicos.Embarcador.Carga.CargaDadosSumarizados(unitOfWork).AlterarDadosSumarizadosPreCarga(preCarga, unitOfWork, TipoServicoMultisoftware);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork).CriarCargaJanelaCarregamento(preCarga);
                new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork).Adicionar(preCarga, TipoServicoMultisoftware, cargaJanelaCarregamento);

                if (configuracaoPreCarga.VincularFilaCarregamentoVeiculoAutomaticamente)
                {
                    try
                    {
                        new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento()).AlocarParaPrimeiroDaFila(preCarga, TipoServicoMultisoftware);
                    }
                    catch
                    {
                        new Servicos.Embarcador.PreCarga.PreCargaOfertaTransportador(unitOfWork).DisponibilizarParaTransportadorPorRota(preCarga);
                    }
                }

                unitOfWork.CommitChanges();

                if (cargaJanelaCarregamento != null)
                    new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os pedidos da Rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosParaTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigo);

                if (preCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, ObterOrigemAlteracaoFilaCarregamento());
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.ObterFilaCarregamentoVeiculoPorPreCarga(preCarga.Codigo);
                Dominio.Entidades.Embarcador.Logistica.ProcedimentoEmbarque procedimentoEmbarque = Servicos.Embarcador.Logistica.ProcedimentoEmbarque.ObterProcedimentoEmbarqueCarga(preCarga.TipoOperacao, preCarga.Filial, unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura> faixasTemperatura = new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();
                bool cargaPodeSerModificada = PermitirAlterarDadostransporte(preCarga.Carga);
                bool exibirFilaCarregamentoMotorista = cargaPodeSerModificada && configuracaoEmbarcador.UtilizarFilaCarregamento;
                bool exibirFilaCarregamentoVeiculo = cargaPodeSerModificada && (configuracaoEmbarcador.UtilizarFilaCarregamento || configuracaoGeralCarga.UtilizarProgramacaoCarga);
                bool exigirConfirmacaoTracao = (preCarga.TipoOperacao?.ExigePlacaTracao ?? false) || configuracaoGeralCarga.UtilizarProgramacaoCarga;

                if (procedimentoEmbarque != null)
                    faixasTemperatura = repositorioFaixaTemperatura.BuscarPorProcedimento(procedimentoEmbarque.Codigo);
                else
                    faixasTemperatura = preCarga.TipoOperacao != null ? repositorioFaixaTemperatura.BuscarPorTipoOperacao(preCarga.TipoOperacao.Codigo) : new List<Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura>();

                List<(int CodigoPreCarga, string Destino)> listaDestino = repositorioPreCargaDestino.BuscarPorPreCarga(preCarga.Codigo);
                List<(int CodigoPreCarga, string Estado)> listaEstadoDestino = repositorioPreCargaEstadoDestino.BuscarPorPreCarga(preCarga.Codigo);
                List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarPorPreCarga(preCarga.Codigo);
                Dominio.Entidades.Veiculo reboque = preCarga.VeiculosVinculados?.ElementAtOrDefault(0);
                Dominio.Entidades.Veiculo segundoReboque = preCarga.VeiculosVinculados?.ElementAtOrDefault(1);

                var retorno = new
                {
                    preCarga.Codigo,
                    PossuiCarga = preCarga.Carga != null,
                    CargaPodeSerModificada = cargaPodeSerModificada,
                    ExibirFilaCarregamentoMotorista = exibirFilaCarregamentoMotorista,
                    ExibirFilaCarregamentoVeiculo = exibirFilaCarregamentoVeiculo,
                    DataImportacao = preCarga.DataImportacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataAtualizacaoImportacao = preCarga.DataAtualizacaoImportacao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    CargaRetorno = preCarga.CargaRetorno ?? string.Empty,
                    preCarga.DocaCarregamento,
                    preCarga.NumeroPreCarga,
                    Pedidos = String.Join(", ", from p in preCarga.Pedidos select p.NumeroPedidoEmbarcador),
                    Carga = preCarga.Carga != null ? new { preCarga.Carga.Codigo, Descricao = preCarga.Carga.CodigoCargaEmbarcador } : null,
                    ModeloVeicular = preCarga.ModeloVeicularCarga != null ? new { preCarga.ModeloVeicularCarga.Codigo, preCarga.ModeloVeicularCarga.Descricao } : null,
                    TipoCarga = preCarga.TipoDeCarga?.Descricao ?? string.Empty,
                    TipoOperacao = preCarga.TipoOperacao?.Descricao ?? string.Empty,
                    Remetente = PedidosPreCargaRemetentes(preCarga.Pedidos),
                    Destinatario = PedidosPreCargaDestinatarios(preCarga.Pedidos),
                    ConfiguracaoProgramacaoCarga = preCarga.ConfiguracaoProgramacaoCarga?.Descricao ?? "",
                    Origem = ObjetoOrigemDestino(preCarga.Pedidos?.FirstOrDefault()?.Origem),
                    Destino = ObjetoOrigemDestino(preCarga.Pedidos?.FirstOrDefault()?.Destino),
                    CidadesDestino = string.Join(", ", listaDestino.Select(destino => destino.Destino.Trim())),
                    EstadosDestino = string.Join(", ", listaEstadoDestino.Select(estadoDestino => estadoDestino.Estado.Trim())),
                    RegioesDestino = string.Join(", ", listaRegiaoDestino.Select(regiaoDestino => regiaoDestino.Regiao.Trim())),
                    Peso = preCarga.Pedidos.Sum(p => p.PesoTotal).ToString("n2"),
                    Entregas = preCarga.Pedidos?.Count() ?? 0,
                    RotaFrete = new { Codigo = preCarga.Rota?.Codigo ?? 0, Descricao = preCarga.Rota?.Descricao ?? string.Empty },
                    DataPrevisaoEntrega = preCarga.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataPrevisaoEntregaManual = preCarga.DataPrevisaoEntregaManual?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataInicioViagem = preCarga.DataPrevisaoInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    PrevisaoChegadaDoca = preCarga.PrevisaoChegadaDoca?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    BuscaRemetentes = PedidosPreCargaCodigosRemetentes(preCarga.Pedidos),
                    BuscaDestinatarios = PedidosPreCargaCodigosDestinatarios(preCarga.Pedidos),
                    Filial = new { Codigo = preCarga.Filial?.Codigo ?? 0, Descricao = preCarga.Filial?.Descricao ?? "" },
                    Transportador = new { Codigo = preCarga.Empresa?.Codigo ?? 0, Descricao = preCarga.Empresa?.Descricao ?? "" },
                    Tracao = new
                    {
                        Codigo = preCarga.Veiculo?.Codigo ?? 0,
                        Descricao = exigirConfirmacaoTracao ? preCarga.Veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(preCarga.Veiculo),
                        TipoVeiculo = preCarga.Veiculo?.TipoVeiculo ?? ""
                    },
                    MotivoAlteracaoData = preCarga?.MotivoAlteracaoData ?? string.Empty,
                    MotivoCancelamento = preCarga?.MotivoCancelamento ?? string.Empty,
                    ObservacaoPreCarga = preCarga?.Observacao ?? string.Empty,
                    DataInclusao = preCarga?.DataCriacaoPreCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    PesoPlanejamento = preCarga?.Peso.ToString("n2") ?? string.Empty,
                    QuantidadePallets = preCarga?.QuantidadePallet.ToString("n2") ?? string.Empty,
                    EmpresaTracao = preCarga.Veiculo?.Empresa?.Codigo ?? 0,
                    Reboque = new { Codigo = reboque?.Codigo ?? 0, Descricao = reboque?.Descricao ?? "" },
                    EmpresaReboque = reboque?.Empresa?.Codigo ?? 0,
                    SegundoReboque = new { Codigo = segundoReboque?.Codigo ?? 0, Descricao = segundoReboque?.Descricao ?? "" },
                    EmpresaSegundoReboque = segundoReboque?.Empresa?.Codigo ?? 0,
                    Motorista = preCarga.Motoristas?.Count() > 0 ? new { preCarga.Motoristas.FirstOrDefault().Codigo, preCarga.Motoristas.FirstOrDefault().Descricao } : null,
                    EmpresaMotorista = preCarga.Motoristas?.FirstOrDefault()?.Empresa?.Codigo ?? 0,
                    FilaCarregamento = filaCarregamentoVeiculo?.Codigo ?? 0,
                    FilaCarregamentoAnterior = filaCarregamentoVeiculo?.Codigo ?? 0,
                    LocalCarregamento = new { Codigo = preCarga.LocalCarregamento?.Codigo ?? 0, Descricao = preCarga.LocalCarregamento?.DescricaoAcao ?? "" },
                    ExigirConfirmacaoTracao = exigirConfirmacaoTracao,
                    NumeroReboques = preCarga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                    FaixaTemperatura = new { Codigo = preCarga.FaixaTemperatura?.Codigo ?? 0, Descricao = preCarga.FaixaTemperatura?.Descricao ?? "" },
                    ProblemaVincularCarga = preCarga.ProblemaVincularCarga ?? "",
                    FaixasTemperatura = (
                        from faixaTemperatura in faixasTemperatura
                        select new
                        {
                            text = faixaTemperatura.Descricao,
                            value = faixaTemperatura.Codigo.ToString()
                        }
                    ).ToList()
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarDadosParaTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigo, true);

                if (preCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarRegistro);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga != null)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCargaComCargaVinculada = repPreCarga.BuscarPorCargaVinculadaOutraPreCarga(preCarga.Codigo, carga.Codigo);

                    if (preCargaComCargaVinculada != null)
                        return new JsonpResult(false, true, string.Format(Localization.Resources.Cargas.Carga.ACargaSelecionadaJaEstaVinculadaAoPrePlanejamento, preCargaComCargaVinculada.NumeroPreCarga));

                    if (!carga.SituacaoCarga.IsSituacaoCargaNaoEmitida())
                        return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.ASituacaoDaCargaSelecionadaNaoPermiteVincularAoPrePlanejamento);
                }

                bool adicionarCarga = ((preCarga.Carga == null) && (carga != null));
                bool removerCarga = ((preCarga.Carga != null) && (carga == null));
                bool trocarCarga = ((carga != null) && (preCarga.Carga != null) && (carga.Codigo != preCarga.Carga.Codigo));

                unitOfWork.Start();

                if (trocarCarga || removerCarga)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, null, $"Removeu a Carga {servicoCarga.ObterNumeroCarga(preCarga.Carga, configuracaoEmbarcador)}", unitOfWork);

                    if (!servicoPreCarga.RemoverCargaDaPreCarga(preCarga, out string erroVinculo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erroVinculo);
                    }
                }

                PreencheEntidade(preCarga, unitOfWork);

                if (trocarCarga || adicionarCarga)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, null, $"Vinculou a Carga {servicoCarga.ObterNumeroCarga(carga, configuracaoEmbarcador)}", unitOfWork);

                    List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

                    if (!servicoPreCarga.VincularPreCargaACarga(preCarga, carga, cargaPedidos, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, out string erroVinculo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erroVinculo);
                    }
                }

                repPreCarga.Atualizar(preCarga, Auditado);

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> filasCarregamentoAlteradas = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                int codigoFilaCarregamento = Request.GetIntParam("FilaCarregamento");

                if (configuracaoEmbarcador.UtilizarFilaCarregamento || configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    if ((codigoFilaCarregamento <= 0) && ((preCarga.Veiculo != null) || (preCarga.VeiculosVinculados.Count > 0)))
                    {
                        Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo conjuntoVeiculo = new Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo()
                        {
                            Tracao = preCarga.Veiculo,
                            Reboques = preCarga.VeiculosVinculados.ToList(),
                            ModeloVeicularCarga = preCarga.ModeloVeicularCarga ?? preCarga.VeiculosVinculados.FirstOrDefault()?.ModeloVeicularCarga ?? preCarga.Veiculo?.ModeloVeicularCarga
                        };

                        Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
                        {
                            CodigoFilial = preCarga.Filial.Codigo,
                            CodigoMotorista = preCarga.Motoristas.FirstOrDefault()?.Codigo ?? 0,
                            CodigosDestino = repositorioPreCargaDestino.BuscarCodigosDestinosPorPreCarga(preCarga.Codigo),
                            CodigosRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarCodigosRegioesDestinoPorPreCarga(preCarga.Codigo),
                            ConjuntoVeiculo = conjuntoVeiculo,
                            DataProgramada = preCarga.DataPrevisaoEntrega,
                            SiglasEstadoDestino = repositorioPreCargaEstadoDestino.BuscarSiglasEstadosDestinoPorPreCarga(preCarga.Codigo)
                        };

                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamento.Adicionar(filaCarregamentoVeiculoAdicionar, TipoServicoMultisoftware);
                        codigoFilaCarregamento = filaCarregamentoVeiculo.Codigo;
                    }

                    filasCarregamentoAlteradas.AddRange(servicoFilaCarregamento.AlocarPreCargaManualmente(preCarga, codigoFilaCarregamento, Request.GetIntParam("FilaCarregamentoMotorista"), TipoServicoMultisoftware));
                }

                if ((codigoFilaCarregamento > 0) || preCarga.IsChangedByPropertyName("Empresa"))
                {
                    Repositorio.Embarcador.PreCargas.PreCargaOferta repositorioPreCargaOferta = new Repositorio.Embarcador.PreCargas.PreCargaOferta(unitOfWork);
                    Dominio.Entidades.Embarcador.PreCargas.PreCargaOferta preCargaOferta = repositorioPreCargaOferta.BuscarPorPreCarga(preCarga.Codigo);

                    if (preCargaOferta != null)
                    {
                        Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioPreCargaOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);

                        preCargaOferta.Situacao = SituacaoPreCargaOferta.Finalizada;

                        repositorioPreCargaOferta.Atualizar(preCargaOferta);
                        repositorioPreCargaOfertaTransportador.RejeitarTodas(preCargaOferta.Codigo);
                    }
                }

                unitOfWork.CommitChanges();

                servicoFilaCarregamento.NotificarAlteracoes(filasCarregamentoAlteradas);
                NotificarJanelaCarregamentoAtualizada(preCarga, unitOfWork);

                return new JsonpResult(FormataObjetoPreCarga(preCarga));
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == CodigoExcecao.JornadaTrabalhoExcedida)
                    return new JsonpResult(new
                    {
                        JornadaExcedida = true,
                        Mensagem = excecao.Message
                    });

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarOperacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PreCargas/PreCarga");

                // Instancia repositorios
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                string justificativa = Request.GetStringParam("Justificativa");
                string motivo = Request.GetStringParam("Motivo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigo, true);

                // Valida
                if (preCarga == null)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarORegistro);

                if (justificativa.Length < 20)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.JustificativaDeveConterNoMinimoVinteCaracteres);

                if (!ValidaPossibilidadeCancelamento(preCarga) && !permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PreCarga_Supervisor))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoEPossivelCancelarUmaPrePlanejamentoComDadosJaInformados);

                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PreCarga_Supervisor) && !ValidaPossibilidadeCancelamentoComCarga(preCarga))
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.NaoEPossivelCancelarUmaPrePlanejamentoComACargaNaAtualSituacao);

                // Persiste dados
                unitOfWork.Start();

                if (preCarga.Carga != null)
                {
                    if (!servicoPreCarga.RemoverCargaDaPreCarga(preCarga, out string erroVinculo))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erroVinculo);
                    }
                }
                preCarga.SituacaoPreCarga = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPreCarga.Cancelada;
                preCarga.JustificativaCancelamento = justificativa;
                preCarga.UsuarioCancelamento = this.Usuario;
                preCarga.MotivoCancelamento = motivo;
                Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), Localization.Resources.Cargas.Carga.CancelamentoPrePlanejamento, unitOfWork);

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoDisponibilizada = servicoFilaCarregamentoVeiculo.DisponibilizarPorPreCargaCancelada(preCarga, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                if (filaCarregamentoDisponibilizada != null)
                    servicoFilaCarregamentoVeiculo.NotificarAlteracao(filaCarregamentoDisponibilizada);

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelamentoMassivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string justificativa = Request.GetStringParam("Justificativa");
                string motivo = Request.GetStringParam("Motivo");

                if (justificativa.Length < 20)
                    return new JsonpResult(false, true, Localization.Resources.Cargas.Carga.JustificativaDeveConterNoMinimoVinteCaracteres);

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                Servicos.Embarcador.PreCarga.PreCarga servicoPreCarga = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("PreCargas/PreCarga");
                bool supervisor = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.PreCarga_Supervisor);
                List<int> codigosPreCargasSelecionadas = ObterCodigosPreCargasSelecionadas(unitOfWork);
                List<string> numerosPreCargasComErro = new List<string>();

                foreach (int codigoPreCarga in codigosPreCargasSelecionadas)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga, true);

                    if (!ValidaPossibilidadeCancelamento(preCarga) && !supervisor)
                        continue;

                    unitOfWork.Start();

                    if (preCarga.Carga != null)
                    {
                        if (!servicoPreCarga.RemoverCargaDaPreCarga(preCarga, out string erroVinculo))
                        {
                            unitOfWork.Rollback();
                            numerosPreCargasComErro.Add(preCarga.NumeroPreCarga);
                            Servicos.Log.TratarErro(erroVinculo, "PRECARGA");
                            continue;
                        }
                    }

                    preCarga.SituacaoPreCarga = SituacaoPreCarga.Cancelada;
                    preCarga.JustificativaCancelamento = justificativa;
                    preCarga.UsuarioCancelamento = this.Usuario;
                    preCarga.MotivoCancelamento = motivo;

                    repositorioPreCarga.Atualizar(preCarga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, preCarga, preCarga.GetChanges(), Localization.Resources.Cargas.Carga.CancelamentoPrePlanejamento, unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoDisponibilizada = servicoFilaCarregamentoVeiculo.DisponibilizarPorPreCargaCancelada(preCarga, TipoServicoMultisoftware);

                    unitOfWork.CommitChanges();

                    if (filaCarregamentoDisponibilizada != null)
                        servicoFilaCarregamentoVeiculo.NotificarAlteracao(filaCarregamentoDisponibilizada);
                }

                return new JsonpResult(new
                {
                    PreCargas = numerosPreCargasComErro
                });
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoCancelarOsPrePlanejamentosSelecionados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedido(unitOfWork);

            unitOfWork.Dispose();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> ConfirmarImportar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedido(unitOfWork);
                dynamic registrosAlterados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("RegistrosAlterados"));
                string retorno = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork).ConfirmarImportarPedido(registrosAlterados, this.Usuario, Auditado);

                if (string.IsNullOrWhiteSpace(retorno))
                    return new JsonpResult(true);
                else
                    return new JsonpResult(false, true, retorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoPedido(unitOfWork);

                string dados = Request.Params("Dados");

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork).ImportarPedido(dados, this.Usuario, TipoServicoMultisoftware, Cliente, Auditado);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTotalizadoresPorSituacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigosConfiguracaoProgramacaoCarga = Request.GetListParam<int>("ConfiguracaoProgramacaoCarga"),
                    CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicularCarga"),
                    CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                    CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                    DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                    NumeroPreCarga = Request.GetStringParam("PreCarga"),
                    SomenteProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga
                };

                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaTotalizador preCargaTotalizador = repositorioPreCarga.BuscarTotalizadorPorSituacao(filtrosPesquisa);

                return new JsonpResult(preCargaTotalizador ?? new Dominio.ObjetosDeValor.Embarcador.PreCarga.PreCargaTotalizador());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterOsTotalizadoresPorSituacaoDoPrePlanejamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Transportador, propriedade: "Descricao", tamanho: 30, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Situacao, propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                int codigoPreCarga = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador repositorioOfertaTransportador = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportador(unitOfWork);
                int totalRegistros = repositorioOfertaTransportador.ContarConsultaPorPreCarga(codigoPreCarga);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> listaTransportadoresOfertadosSemOrdenacao = (totalRegistros > 0) ? repositorioOfertaTransportador.ConsultarPorPreCarga(codigoPreCarga, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador>();
                List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportador> listaTransportadoresOfertados = listaTransportadoresOfertadosSemOrdenacao.OrderByDescending(o => o.Tipo).ThenByDescending(o => o.Bloqueada).ThenBy(o => o.Codigo).ToList();

                var listaTransportadoresOfertadosRetornar = (
                    from o in listaTransportadoresOfertados
                    select new
                    {
                        o.Codigo,
                        o.Transportador.Descricao,
                        Situacao = o.Situacao.ObterDescricao(),
                        DT_FontColor = "#666",
                        DT_RowColor = o.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaTransportadoresOfertadosRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarOsTransportadoresOfertados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertadosHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoOfertaTransportador = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico repositorioHistorico = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico(unitOfWork);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistorico> historicos = repositorioHistorico.BuscarPorOfertaTransportador(codigoOfertaTransportador);

                var historicosRetornar = (
                    from historico in historicos
                    select new
                    {
                        historico.Codigo,
                        historico.Tipo,
                        Data = historico.Data.ToDateTimeString(),
                        historico.Descricao,
                        Usuario = historico.Usuario?.Descricao ?? ""
                    }
                ).ToList();

                return new JsonpResult(historicosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarOsTransportadoresOfertados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterTransportadoresOfertadosHistoricoOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoHistorico = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta repositorioHistoricoOferta = new Repositorio.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta(unitOfWork);
                List<Dominio.Entidades.Embarcador.PreCargas.PreCargaOfertaTransportadorHistoricoOferta> historicosOferta = repositorioHistoricoOferta.BuscarPorOfertaTransportadorHistorico(codigoHistorico);

                var historicosRetornar = (
                    from historicoOferta in historicosOferta
                    select new
                    {
                        historicoOferta.Codigo,
                        historicoOferta.Ordem,
                        historicoOferta.Descricao,
                        Empresa = historicoOferta.Empresa.Descricao,
                        PercentualCargas = historicoOferta.PercentualCargas.ToString("n2"),
                        PercentualConfigurado = historicoOferta.PercentualConfigurado.ToString("n2"),
                        Prioridade = historicoOferta.Prioridade.ToString("n0"),
                        DT_FontColor = historicoOferta.Tipo.ObterCorFonte(),
                        DT_RowColor = historicoOferta.Tipo.ObterCorLinha()
                    }
                ).ToList();

                return new JsonpResult(historicosRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoBuscarHistoricoDoTransportadorOfertado);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularFilaCarregamentoMassivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                List<int> codigosPreCargasSelecionadas = ObterCodigosPreCargasSelecionadas(unitOfWork);
                List<(string NumeroPreCarga, string MensagemRetorno, bool Sucesso)> retornos = new List<(string NumeroPreCarga, string MensagemRetorno, bool Sucesso)>();

                foreach (int codigoPreCarga in codigosPreCargasSelecionadas)
                {
                    string numeroPreCarga = string.Empty;

                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga, true);

                        numeroPreCarga = preCarga.NumeroPreCarga;

                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = servicoFilaCarregamentoVeiculo.AlocarParaPrimeiroDaFila(preCarga, TipoServicoMultisoftware);

                        unitOfWork.CommitChanges();

                        retornos.Add(ValueTuple.Create(numeroPreCarga, Localization.Resources.Cargas.Carga.VeiculoDaFilaDeCarregamentoVinculadoComSucesso, true));

                        servicoFilaCarregamentoVeiculo.NotificarAlteracao(filaCarregamentoVeiculo);
                    }
                    catch (BaseException excecao)
                    {
                        unitOfWork.Rollback();
                        retornos.Add(ValueTuple.Create(numeroPreCarga, excecao.Message, false));
                    }
                    catch
                    {
                        unitOfWork.Rollback();
                        retornos.Add(ValueTuple.Create(numeroPreCarga, Localization.Resources.Cargas.Carga.OcorreuFalhaAoVincularVeiculoDaFilaDeCarregamento, false));
                    }
                }

                return new JsonpResult((
                    from retorno in retornos
                    select new
                    {
                        NumeroPreCarga = retorno.NumeroPreCarga,
                        MensagemRetorno = retorno.MensagemRetorno,
                        DT_RowColor = retorno.Sucesso ? "#99ff99" : "#ffb3b3"
                    }
                ).ToList());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ImportarPrePlanejamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string dados = Request.Params("Dados");
                List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha> linhas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Importacao.DadosLinha>>(dados);

                Dominio.ObjetosDeValor.Embarcador.Importacao.RetornoImportacao retornoImportacao = new Servicos.Embarcador.PreCarga.PreCarga(unitOfWork).ImportarPrePlanejamento(linhas, Usuario, unitOfWork, Auditado, TipoServicoMultisoftware);

                return new JsonpResult(retornoImportacao);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacaoPrePlanejamento()

        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>();

            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Cargas.Carga.FilialDescricao, Propriedade = "FilialDescricao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = Localization.Resources.Cargas.Carga.FilialCodigo, Propriedade = "FilialCodigo", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = Localization.Resources.Cargas.Carga.TipoDeCarga, Propriedade = "TipoCarga", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = Localization.Resources.Cargas.Carga.TipoDeOperacao, Propriedade = "TipoOperacao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = Localization.Resources.Cargas.Carga.ModeloVeicular, Propriedade = "ModeloVeicularCarga", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = Localization.Resources.Cargas.Carga.DataCarregamento, Propriedade = "Data", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = Localization.Resources.Cargas.Carga.DestCidOpcional, Propriedade = "Cidade", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = Localization.Resources.Cargas.Carga.DestUfObrigatorio, Propriedade = "Estado", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = Localization.Resources.Cargas.Carga.DataPrevisaoDeEntrega, Propriedade = "DataPrevisaoEntrega", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Rota (Descrição)", Propriedade = "RotaDescricao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Rota (Código Integração)", Propriedade = "RotaCodigo", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Peso", Propriedade = "Peso", Tamanho = 200 });
            configuracoes.Add(new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Qtde de Paletes", Propriedade = "QuantidadePallets", Tamanho = 200 });

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> AdicionarObservacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigo);

                if (preCarga == null) throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarORegistro);

                unitOfWork.Start();

                preCarga.Observacao = observacao;
                repPreCarga.Atualizar(preCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarDataPlanejamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.PreCargas.PreCarga repPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                int codigo = Request.GetIntParam("Codigo");
                DateTime data = Request.GetDateTimeParam("Data");
                DateTime? dataPrevisaoEntrega = Request.GetNullableDateTimeParam("DataPrevisaoEntrega");
                string motivo = Request.GetStringParam("Motivo");

                if (data <= DateTime.Now) throw new ControllerException(Localization.Resources.Cargas.Carga.ADataNaoPodeSerAnteriorAoDiaAtual);
                if (string.IsNullOrWhiteSpace(motivo) || motivo.Length < 10) throw new ControllerException(Localization.Resources.Cargas.Carga.OMotivoNaoPodeTerMenosDeDezCaracteres);

                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repPreCarga.BuscarPorCodigo(codigo);

                if (preCarga == null) throw new ControllerException(Localization.Resources.Cargas.Carga.NaoFoiPossivelEncontrarORegistro);
                if (preCarga.SituacaoPreCarga != SituacaoPreCarga.Nova) throw new ControllerException(Localization.Resources.Cargas.Carga.SomenteEPossivelAlterarADataDeUmRegistroNovo);


                unitOfWork.Start();

                preCarga.DataPrevisaoEntrega = data;
                preCarga.DataPrevisaoEntregaManual = dataPrevisaoEntrega;
                preCarga.MotivoAlteracaoData = motivo;
                repPreCarga.Atualizar(preCarga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoAtualizarDados);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Métodos Privados

        private void NotificarJanelaCarregamentoAtualizada(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamento cargaJanelaCarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaCarregamento(unitOfWork).BuscarPorPreCarga(preCarga.Codigo);

            if (cargaJanelaCarregamento != null)
                new Servicos.Embarcador.Hubs.JanelaCarregamento().InformarJanelaCarregamentoAtualizada(cargaJanelaCarregamento);
        }

        private List<int> ObterCodigosPreCargasSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            if (!Request.GetBoolParam("SelecionarTodos"))
            {
                List<int> codigosPreCargasSelecionadas = Request.GetListParam<int>("ItensSelecionados");

                return codigosPreCargasSelecionadas;
            }

            List<int> codigosPreCargasNaoSelecionadas = Request.GetListParam<int>("ItensNaoSelecionados");
            Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            List<int> codigosPreCargas = repositorioPreCarga.ConsultarCodigos(filtrosPesquisa);

            foreach (var codigoPreCarga in codigosPreCargasNaoSelecionadas)
                codigosPreCargas.Remove(codigoPreCarga);

            return codigosPreCargas;
        }

        private Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

            return new Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigosConfiguracaoProgramacaoCarga = Request.GetListParam<int>("ConfiguracaoProgramacaoCarga"),
                CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicularCarga"),
                CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario"),
                CpfCnpjRemetente = Request.GetDoubleParam("Remetente"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataViagemFinal = Request.GetNullableDateTimeParam("DataViagemFinal"),
                DataViagemInicial = Request.GetNullableDateTimeParam("DataViagemInicial"),
                NumeroCarga = Request.GetStringParam("Carga"),
                NumeroPedido = Request.GetStringParam("Pedido"),
                NumeroPreCarga = Request.GetStringParam("PreCarga"),
                Situacao = Request.GetListEnumParam<SituacaoPreCarga>("Situacao"),
                Status = Request.GetEnumParam<FiltroPreCarga>("Status"),
                SomenteProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga,
                CodigosCidadesDestino = Request.GetListParam<int>("CidadesDestino"),
                CodigosRotaFrete = Request.GetListParam<int>("RotasFrete"),
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {

                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.ObjetosDeValor.Embarcador.PreCarga.FiltroPesquisaPreCarga filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PermitirCancelar", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PermitirCancelarViaSupervisor", visivel: false);
                grid.AdicionarCabecalho(propriedade: "ExibirTransportadoresOfertados", visivel: false);
                grid.AdicionarCabecalho(propriedade: "ObservacaoDescricao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "MotivoAlteracaoData", visivel: false);
                grid.AdicionarCabecalho(propriedade: "MotivoCancelamento", visivel: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Numero, propriedade: "NumeroPreCarga", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);

                if (filtrosPesquisa.SomenteProgramacaoCarga)
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Data, propriedade: "DataPrevisaoEntrega", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                else
                {
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Doca, propriedade: "PrevisaoChegadaDoca", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Gerais.Geral.Viagem, propriedade: "DataPrevisaoInicioViagem", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Gerais.Geral.Pedidos, propriedade: "Pedidos", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.FaixaDeTemperatura, propriedade: "FaixaTemperatura", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Origem, propriedade: "Remetente", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Destino, propriedade: "Destinatario", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Peso, propriedade: "Peso", tamanho: 10, alinhamento: Models.Grid.Align.right, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Entregas, propriedade: "Entregas", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.LocalCarregamento, propriedade: "DocaCarregamento", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Filial, propriedade: "Filial", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Veiculo, propriedade: "Veiculos", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Transportador, propriedade: "Empresa", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ModeloVeicular, propriedade: "ModeloVeicularCarga", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeCarga, propriedade: "TipoCarga", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TipoDeOperacao, propriedade: "TipoOperacao", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Motorista, propriedade: "Motoristas", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.NumeroDaCarga, propriedade: "CodigoCargaEmbarcador", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (filtrosPesquisa.SomenteProgramacaoCarga)
                {
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.RegioesDestino, propriedade: "RegioesDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.CidadesDestino, propriedade: "CidadesDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.EstadoDestino, propriedade: "EstadosDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.ConfiguracaoProgramacaoCarga, propriedade: "ConfiguracaoProgramacaoCarga", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.VeiculosDedicado, propriedade: "VeiculoDedicado", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.StatusGR, propriedade: "StatusGr", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.TesteFrio, propriedade: "MensagemLicenca", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: Localization.Resources.Cargas.Carga.Rastreador, propriedade: "Rastreador", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: "Observação", propriedade: "Observacao", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Status do Pré Planejamento", propriedade: "StatusPrePlanejamento", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data de Inclusão do PP", propriedade: "DataCriacaoPreCarga", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Peso do Planejamento", propriedade: "PesoPreCarga", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Quantidade de Pallets", propriedade: "QuantidadePallets", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data Previsão Entrega", propriedade: "DataPrevisaoEntregaManual", 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "PreCarga/Pesquisa", "grid-pre-carga");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                List<(int CodigoCarga, string Mensagem)> listaMensagemLicenca = new List<(int CodigoCarga, string Mensagem)>();
                List<(int CodigoVeiculo, string NumeroContrato)> listaVeiculoContrato = new List<(int CodigoVeiculo, string NumeroContrato)>();
                List<(int CodigoPreCarga, string Regiao)> listaRegiaoDestino = new List<(int CodigoPreCarga, string Regiao)>();
                List<(int CodigoPreCarga, string Destino)> listaDestino = new List<(int CodigoConfiguracaoProgramacaoCarga, string Destino)>();
                List<(int CodigoPreCarga, string Estado)> listaEstadoDestino = new List<(int CodigoConfiguracaoProgramacaoCarga, string Estado)>();
                List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador)> listaPosicaoVeiculos = new List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador)>();
                List<Dominio.Entidades.Embarcador.PreCargas.PreCarga> listaPreCarga = new List<Dominio.Entidades.Embarcador.PreCargas.PreCarga>();
                int totalRegistros = repositorioPreCarga.ContarConsulta(filtrosPesquisa);

                if (totalRegistros > 0)
                {
                    listaPreCarga = repositorioPreCarga.Consultar(filtrosPesquisa, parametrosConsulta);

                    if (filtrosPesquisa.SomenteProgramacaoCarga)
                    {
                        Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork);
                        Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo repositorioContratoFreteTransportadorVeiculo = new Repositorio.Embarcador.Frete.ContratoFreteTransportadorVeiculo(unitOfWork);
                        Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);

                        List<int> codigosPreCarga = listaPreCarga.Select(preCarga => preCarga.Codigo).ToList();
                        List<int> codigosCarga = listaPreCarga.Where(preCarga => preCarga.Carga != null).Select(preCarga => preCarga.Carga.Codigo).ToList();
                        List<int> codigosVeiculo = listaPreCarga.SelectMany(preCarga => preCarga.RetornarCodigos).Distinct().ToList();
                        List<int> codigosConfiguracaoProgramacaoCarga = listaPreCarga.Where(preCarga => preCarga.ConfiguracaoProgramacaoCarga != null).Select(preCarga => preCarga.ConfiguracaoProgramacaoCarga.Codigo).ToList();

                        listaMensagemLicenca = repositorioCargaLicenca.BuscarMensagemPorCargas(codigosCarga);
                        listaVeiculoContrato = repositorioContratoFreteTransportadorVeiculo.BuscarContratosAtivosPorVeiculos(codigosVeiculo);
                        listaDestino = repositorioPreCargaDestino.BuscarPorPreCargas(codigosPreCarga);
                        listaEstadoDestino = repositorioPreCargaEstadoDestino.BuscarPorPreCargas(codigosPreCarga);
                        listaRegiaoDestino = repositorioPreCargaRegiaoDestino.BuscarPorPreCargas(codigosPreCarga);
                        listaPosicaoVeiculos = repositorioPosicaoAtual.BuscarDadosPosicaoPorVeiculos(codigosVeiculo);
                    }
                }

                var listaPreCargaRetornar = (
                    from preCarga in listaPreCarga
                    select new
                    {
                        preCarga.Codigo,
                        CodigoCarga = preCarga.Carga?.Codigo ?? 0,
                        preCarga.NumeroPreCarga,
                        Observacao = preCarga.Observacao ?? string.Empty,
                        CodigoCargaEmbarcador = preCarga.Carga?.CodigoCargaEmbarcador ?? "",
                        DataPrevisaoEntrega = preCarga.DataPrevisaoEntrega?.ToDateTimeString(),
                        DataPrevisaoEntregaManual = preCarga.DataPrevisaoEntregaManual?.ToDateTimeString(),
                        PrevisaoChegadaDoca = preCarga.PrevisaoChegadaDoca?.ToDateTimeString(),
                        DataPrevisaoInicioViagem = preCarga.DataPrevisaoInicioViagem?.ToDateTimeString(),
                        Pedidos = String.Join(", ", from pedido in preCarga.Pedidos select pedido.NumeroPedidoEmbarcador),
                        FaixaTemperatura = preCarga.FaixaTemperatura?.Descricao ?? string.Empty,
                        Remetente = PedidosPreCargaRemetentes(preCarga.Pedidos),
                        Destinatario = PedidosPreCargaDestinatarios(preCarga.Pedidos),
                        Peso = preCarga.Pedidos.Sum(p => p.PesoTotal).ToString("n2"),
                        Entregas = preCarga.Pedidos?.Count() ?? 0,
                        preCarga.DocaCarregamento,
                        PermitirCancelar = ValidaPossibilidadeCancelamento(preCarga),
                        PermitirCancelarViaSupervisor = ValidaPossibilidadeCancelamentoComCarga(preCarga),
                        Filial = preCarga.Filial?.Descricao,
                        Empresa = preCarga.Empresa?.Descricao,
                        Veiculos = preCarga.RetornarPlacas,
                        RotaFrete = preCarga.Rota?.Descricao ?? string.Empty,
                        PrevisaoEntrega = preCarga.DataPrevisaoEntregaManual?.ToDateTimeString(),
                        ObservacaoDescricao = preCarga.Observacao,
                        MotivoAlteracaoData = preCarga.MotivoAlteracaoData,
                        MotivoCancelamento = preCarga.MotivoCancelamento,
                        VeiculoDedicado = (preCarga.RetornarCodigos.Count > 0) ? (listaVeiculoContrato.Where(veiculoContrato => preCarga.RetornarCodigos.Contains(veiculoContrato.CodigoVeiculo)).Count() > 0) ? "Sim" : "Não" : "",
                        Rastreador = (listaPosicaoVeiculos.Where(posicaoVeiculo => preCarga.RetornarCodigos.Contains(posicaoVeiculo.CodigoVeiculo) && posicaoVeiculo.Rastreador).Count() > 0) ? "Sim" : "Não",
                        ModeloVeicularCarga = preCarga.ModeloVeicularCarga?.Descricao ?? "",
                        TipoCarga = preCarga.TipoDeCarga?.Descricao ?? "",
                        TipoOperacao = preCarga.TipoOperacao?.Descricao ?? "",
                        Motoristas = preCarga.RetornarDescricaoMotoristas,
                        ConfiguracaoProgramacaoCarga = preCarga.ConfiguracaoProgramacaoCarga?.Descricao ?? "",
                        StatusGr = (preCarga.Carga != null) ? preCarga.Carga.ProblemaIntegracaoGrMotoristaVeiculo ? "Não OK" : "OK" : "",
                        MensagemLicenca = listaMensagemLicenca.Where(licenca => licenca.CodigoCarga == (preCarga.Carga?.Codigo ?? 0)).Select(licenca => licenca.Mensagem).FirstOrDefault(),
                        CidadesDestino = string.Join(", ", listaDestino.Where(destino => destino.CodigoPreCarga == preCarga.Codigo).Select(regiaoDestino => regiaoDestino.Destino.Trim())),
                        EstadosDestino = string.Join(", ", listaEstadoDestino.Where(estadoDestino => estadoDestino.CodigoPreCarga == preCarga.Codigo).Select(regiaoDestino => regiaoDestino.Estado.Trim())),
                        RegioesDestino = string.Join(", ", listaRegiaoDestino.Where(regiaoDestino => regiaoDestino.CodigoPreCarga == preCarga.Codigo).Select(regiaoDestino => regiaoDestino.Regiao.Trim())),
                        ExibirTransportadoresOfertados = configuracaoGeralCarga.UtilizarProgramacaoCarga,
                        StatusPrePlanejamento = preCarga.SituacaoPreCarga.ObterDescricao(),
                        DataCriacaoPreCarga = preCarga?.DataCriacaoPreCarga.ToString() ?? string.Empty,
                        PesoPreCarga = preCarga?.Peso.ToString("n2") ?? string.Empty,
                        QuantidadePallets = preCarga?.QuantidadePallet.ToString("n2") ?? string.Empty,
                        DT_FontColor = (!configuracaoGeralCarga.UtilizarProgramacaoCarga && !string.IsNullOrWhiteSpace(preCarga.ProblemaVincularCarga)) ? "#212529" : preCarga.SituacaoPreCarga.ObterCorFonte(),
                        DT_RowColor = (!configuracaoGeralCarga.UtilizarProgramacaoCarga && !string.IsNullOrWhiteSpace(preCarga.ProblemaVincularCarga)) ? "#c18ff0" : preCarga.SituacaoPreCarga.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaPreCargaRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Descricao")
                return "NumeroPreCarga";

            return propriedadeOrdenar;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> ConfiguracaoImportacaoPedido(Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao> configuracoes = new List<Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao>
            {
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 1, Descricao = "Cód. Intg. Remetente", Propriedade = "CodigoRemetente", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 2, Descricao = "Cód. Intg. Destinatário", Propriedade = "CodigoDestinatario", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 3, Descricao = "CNPJ Transportadora", Propriedade = "CNPJTransportadora", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 4, Descricao = "Tipo Operação", Propriedade = "TipoOperacao", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 5, Descricao = "Modelo Veicular", Propriedade = "ModeloVeicularCarga", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 6, Descricao = "Tipo de Carga", Propriedade = "TipoCarga", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 7, Descricao = "Pré Planejamento", Propriedade = "NumeroPreCarga", Tamanho = 200, Obrigatorio = true, Regras = new List<string> { "required" } },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 12, Descricao = "Número Pedido", Propriedade = "NumeroPedido", Tamanho = 200 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 13, Descricao = "Peso", Propriedade = "PesoPedido", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 14, Descricao = "Carga (Romaneio)", Propriedade = "Carga", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 16, Descricao = "Doca Carregamento", Propriedade = "DocaCarregamento", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 15, Descricao = "Prev. Chegada na Doca", Propriedade = "PrevisaoChegadaDoca", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 8, Descricao = "Início Viagem", Propriedade = "InicioViagem", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 9, Descricao = "Chegada Destinatário", Propriedade = "ChegadaDestinatario", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 10, Descricao = "Saída Destinatário", Propriedade = "SaidaDestinatario", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Fim Viagem", Propriedade = "FimViagem", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 11, Descricao = "Reconhecer Retorno", Propriedade = "CargaRetorno", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 17, Descricao = "Faixa de Temperatura", Propriedade = "FaixaTemperatura", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 18, Descricao = "Observação", Propriedade = "Observacao", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 19, Descricao = "Peso do Planejamento", Propriedade = "PesoPlanejamento", Tamanho = 120 },
                new Dominio.ObjetosDeValor.Embarcador.Importacao.ConfiguracaoImportacao() { Id = 20, Descricao = "Quantidade de Pallets", Propriedade = "QuantidadePallets", Tamanho = 120 },
                
                //17
            };

            return configuracoes;
        }

        private dynamic ObjetoOrigemDestino(Dominio.Entidades.Localidade localidade)
        {
            if (localidade == null) return null;

            return new
            {
                localidade.Codigo,
                localidade.Descricao
            };
        }

        private dynamic FormataObjetoPreCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            return new
            {
                preCarga.Codigo,
                preCarga.NumeroPreCarga,
                preCarga.DocaCarregamento,
                NumeroCarga = preCarga.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                Pedidos = String.Join(", ", from p in preCarga.Pedidos select p.NumeroPedidoEmbarcador),
                Transportador = preCarga.Empresa?.Descricao ?? string.Empty,
                DataInicioViagem = preCarga.DataPrevisaoInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                PrevisaoChegadaDoca = preCarga.PrevisaoChegadaDoca?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                TempoAtraso = TempoAtraso(preCarga),
                ModeloVeicular = preCarga.ModeloVeicularCarga?.Descricao ?? string.Empty,
                Remetente = PedidosPreCargaRemetentes(preCarga.Pedidos),
                Destinatario = PedidosPreCargaDestinatarios(preCarga.Pedidos),
                ConfiguracaoProgramacaoCarga = preCarga.ConfiguracaoProgramacaoCarga?.Descricao ?? "",
                Peso = preCarga.Pedidos.Sum(p => p.PesoTotal).ToString("n2"),
                Entregas = preCarga.Pedidos?.Count() ?? 0,
                Tracao = preCarga.Veiculo?.Placa ?? string.Empty,
                Reboque = ObterPlacas(null, preCarga.VeiculosVinculados),
                Motorista = preCarga.Motoristas?.FirstOrDefault()?.Nome ?? string.Empty,
                TipoOperacao = preCarga.TipoOperacao?.Descricao ?? string.Empty,
                FaixaTemperatura = preCarga.FaixaTemperatura?.Descricao ?? string.Empty,
                TipoCarga = preCarga.TipoDeCarga?.Descricao ?? string.Empty,
                PossuiCarga = preCarga.Carga != null,
                PreCargaCancelada = preCarga.SituacaoPreCarga == SituacaoPreCarga.Cancelada,
                ProblemaVincularCarga = !string.IsNullOrWhiteSpace(preCarga.ProblemaVincularCarga),
                DadosInformados = preCarga.Carga == null ? (
                    preCarga.Empresa != null ||
                    preCarga.Veiculo != null ||
                    (preCarga.VeiculosVinculados != null && preCarga.VeiculosVinculados.Count() > 0) ||
                    (preCarga.Motoristas != null && preCarga.Motoristas.Count() > 0)
                ) : false
            };
        }

        private int TempoAtraso(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            DateTime emRelacaoA = preCarga.DataCarretaInformada ?? DateTime.Now;
            // doca  - confirmacao
            // 06:00 - 05:45 = 15minutos
            return (preCarga.PrevisaoChegadaDoca.HasValue && preCarga.PrevisaoChegadaDoca.Value.Date <= DateTime.Today.AddDays(1)) ? (int)(preCarga.PrevisaoChegadaDoca.Value - emRelacaoA).TotalMinutes : 0;
        }

        private string PedidosPreCargaRemetentes(ICollection<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<string> codigosIntregacoes = (from p in pedidos select p.Remetente.CodigoIntegracao).Distinct().ToList();

            if (pedidos == null)
                return "";

            if (codigosIntregacoes.Count() > 1)
                return String.Join(", ", codigosIntregacoes);

            return pedidos.FirstOrDefault()?.Remetente?.Descricao ?? string.Empty;
        }

        private string PedidosPreCargaCodigosRemetentes(ICollection<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<double> codigos = (from p in pedidos select p.Remetente.CPF_CNPJ).Distinct().ToList();

            if (pedidos == null)
                return "[]";

            return "[" + String.Join(", ", codigos) + "]";
        }

        private string PedidosPreCargaDestinatarios(ICollection<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<string> codigosIntregacoes = (from p in pedidos select p.Destinatario.CodigoIntegracao).Distinct().ToList();

            if (pedidos == null)
                return "";

            if (codigosIntregacoes.Count() > 1)
                return String.Join(", ", codigosIntregacoes);

            return pedidos.FirstOrDefault()?.Destinatario?.Descricao ?? string.Empty;
        }

        private string PedidosPreCargaCodigosDestinatarios(ICollection<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos)
        {
            List<double> codigos = (from p in pedidos select p.Destinatario.CPF_CNPJ).Distinct().ToList();

            if (pedidos == null)
                return "[]";

            return "[" + String.Join(", ", codigos) + "]";
        }

        private void PreencheEntidade(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Logistica.AreaVeiculoPosicao repositorioAreaVeiculoPosicao = new Repositorio.Embarcador.Logistica.AreaVeiculoPosicao(unitOfWork);
            Repositorio.Embarcador.Cargas.FaixaTemperatura repositorioFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repositorioFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

            int codigoTransportador = Request.GetIntParam("Transportador");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoLocalCarregamento = Request.GetIntParam("LocalCarregamento");
            int codigoFaixaTemperatura = Request.GetIntParam("FaixaTemperatura");

            preCarga.LocalCarregamento = codigoLocalCarregamento > 0 ? repositorioAreaVeiculoPosicao.BuscarPorCodigo(codigoLocalCarregamento) : null;
            preCarga.DocaCarregamento = preCarga.LocalCarregamento?.DescricaoAcao.Left(tamanho: 20);
            preCarga.Empresa = repositorioEmpresa.BuscarPorCodigo(codigoTransportador);
            preCarga.FaixaTemperatura = codigoFaixaTemperatura > 0 ? repositorioFaixaTemperatura.BuscarPorCodigo(codigoFaixaTemperatura) : null;

            Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repositorioUsuario.BuscarPorCodigo(codigoMotorista) : null;
            bool dadosTransporteObrigatorioPreCarga = configuracaoEmbarcador?.DadosTransporteObrigatorioPreCarga ?? false;
            bool transportadorObrigatorioPreCarga = configuracaoEmbarcador?.TransportadorObrigatorioPreCarga ?? false;

            if ((dadosTransporteObrigatorioPreCarga || transportadorObrigatorioPreCarga) && (preCarga.Empresa == null))
                throw new ControllerException("O transportador é obrigatório.");

            if (dadosTransporteObrigatorioPreCarga)
            {
                if (motorista == null)
                    throw new ControllerException("O motorista é obrigatório");
                else if ((motorista?.Empresa != null) && (motorista.Empresa.Codigo != preCarga.Empresa.Codigo))
                    throw new ControllerException("Motorista selecionado não pertence ao transportador selecionado.");
            }

            if (preCarga.Motoristas == null)
                preCarga.Motoristas = new List<Dominio.Entidades.Usuario>();
            else
                preCarga.Motoristas.Clear();

            if (motorista != null)
            {
                if (preCarga.DataPrevisaoInicioViagem.HasValue)
                {
                    Dominio.Entidades.Embarcador.PreCargas.PreCarga preCargaConflitoHorario = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork).ValidarMotoristaPorHorario(preCarga.Codigo, motorista.Codigo, preCarga.DataPrevisaoInicioViagem.Value);

                    if (preCargaConflitoHorario != null)
                        throw new ControllerException($"Já existe o pré planejamento ({preCargaConflitoHorario.NumeroPreCarga}) para esse motorista com previsão de início dia {preCargaConflitoHorario.DataPrevisaoInicioViagem.Value.ToString("dd/MM")} às {preCargaConflitoHorario.DataPrevisaoInicioViagem.Value.ToString("HH:mm")}.");
                }

                Servicos.Embarcador.Transportadores.MotoristaJornada servicoMotoristaJornada = new Servicos.Embarcador.Transportadores.MotoristaJornada(unitOfWork, configuracaoEmbarcador);
                servicoMotoristaJornada.ValidarJornadaTrabalhoExcedida(motorista, Request.GetBoolParam("UtilizarMotoristaJornadaExcedida"), preCarga.Rota?.TempoDeViagemEmMinutos ?? 0);

                preCarga.Motoristas.Add(motorista);
            }

            PreencherVeiculos(preCarga, unitOfWork, configuracaoEmbarcador);

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatioDoVeiculo = repositorioFluxoGestaoPatio.BuscarPorVeiculoDisponibilidade(preCarga.Codigo, preCarga.Veiculo?.Codigo ?? 0);

            if ((fluxoGestaoPatioDoVeiculo != null) && fluxoGestaoPatioDoVeiculo.DataFimViagemPrevista.HasValue)
            {
                if (fluxoGestaoPatioDoVeiculo.DataFimViagemPrevista.Value >= preCarga.DataPrevisaoInicioViagem.Value)
                    throw new ControllerException($"O veículo {preCarga.Veiculo.Placa} não possui previsão de disponibilidade para o inicío da viagem.");
            }

            if (preCarga.Carga != null)
            {
                preCarga.Carga.Empresa = preCarga.Empresa;
                preCarga.Carga.Veiculo = preCarga.Veiculo;
                preCarga.Carga.VeiculosVinculados.Clear();
                preCarga.Carga.Motoristas.Clear();

                foreach (var reboquePreCarga in preCarga.VeiculosVinculados)
                    preCarga.Carga.VeiculosVinculados.Add(reboquePreCarga);

                foreach (var motoristaPreCarga in preCarga.Motoristas)
                    preCarga.Carga.Motoristas.Add(motoristaPreCarga);

                repositorioCarga.Atualizar(preCarga.Carga);
            }
        }

        private void PreencherVeiculos(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
            int codigoVeiculo = Request.GetIntParam("Tracao");
            int codigoReboque = Request.GetIntParam("Reboque");
            int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");

            preCarga.Veiculo = codigoVeiculo > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoVeiculo) : null;
            Dominio.Entidades.Veiculo reboque = codigoReboque > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoReboque) : null;
            Dominio.Entidades.Veiculo segundoReboque = codigoSegundoReboque > 0 ? repositorioVeiculo.BuscarPorCodigo(codigoSegundoReboque) : null;

            if (configuracaoEmbarcador.DadosTransporteObrigatorioPreCarga)
            {
                int numeroReboques = preCarga.ModeloVeicularCarga?.NumeroReboques ?? 0;

                if (preCarga.Veiculo == null)
                {
                    if (numeroReboques > 0)
                        throw new ControllerException("A tração (cavalo) é obrigatória");
                    else
                        throw new ControllerException("O veículo é obrigatório");
                }

                if ((numeroReboques > 0) && (reboque == null))
                    throw new ControllerException($"O reboque (carreta{(numeroReboques > 1 ? " 1" : "")}) é obrigatório");

                if ((numeroReboques > 1) && (segundoReboque == null))
                    throw new ControllerException("O reboque (carreta 2) é obrigatório");
            }

            if (preCarga.VeiculosVinculados == null)
                preCarga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
            else
                preCarga.VeiculosVinculados.Clear();

            if ((reboque != null) || (segundoReboque != null))
            {
                preCarga.DataCarretaInformada = DateTime.Now;

                if (reboque != null)
                {
                    if ((reboque?.Empresa != null) && (preCarga.Empresa != null) && (reboque.Empresa.Codigo != preCarga.Empresa.Codigo))
                        throw new ControllerException($"Reboque (carreta {(segundoReboque == null ? "" : " 1")}) selecionado não pertence ao transportador selecionado.");

                    preCarga.VeiculosVinculados.Add(reboque);
                }

                if (segundoReboque != null)
                {
                    if ((segundoReboque?.Empresa != null) && (preCarga.Empresa != null) && (segundoReboque.Empresa.Codigo != preCarga.Empresa.Codigo))
                        throw new ControllerException("Reboque (carreta 2) selecionado não pertence ao transportador selecionado.");

                    preCarga.VeiculosVinculados.Add(segundoReboque);
                }
            }
            else
                preCarga.DataCarretaInformada = null;
        }

        private bool PermitirAlterarDadostransporte(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                return false;

            return carga?.SituacaoCarga.IsSituacaoCargaNaoEmitida() ?? true;
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            List<string> placas = new List<string>() { };

            if (veiculo != null)
                placas.Add(veiculo.Placa);

            if (veiculosVinculados != null)
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

            return string.Join(", ", placas);
        }

        private bool ValidaPossibilidadeCancelamento(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            return preCarga.Empresa == null && preCarga.Veiculo == null && preCarga.VeiculosVinculados.Count == 0 && preCarga.Carga == null;
        }

        private bool ValidaPossibilidadeCancelamentoComCarga(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga)
        {
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] situacoes = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga[] {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte
            };

            if (preCarga.Carga == null)
                return true;

            return !situacoes.Contains(preCarga.Carga.SituacaoCarga);
        }

        #endregion

        #region Métodos Privados Adicionar Pré Carga Manual

        private void AdicionarDestinosPreCargaManual(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            List<int> codigosDestinos = Request.GetListParam<int>("Destinos");
            List<string> siglasEstadosDestino = Request.GetListParam<string>("EstadosDestino");
            List<int> codigosRegioesDestino = Request.GetListParam<int>("RegioesDestino");
            int totalDestinosAdicionados = 0;

            foreach (int codigoDestino in codigosDestinos)
            {
                repositorioPreCargaDestino.Inserir(new Dominio.Entidades.Embarcador.PreCargas.PreCargaDestino()
                {
                    PreCarga = preCarga,
                    Localidade = new Dominio.Entidades.Localidade() { Codigo = codigoDestino }
                });

                totalDestinosAdicionados++;
            }

            foreach (string siglaEstadoDestino in siglasEstadosDestino)
            {
                repositorioPreCargaEstadoDestino.Inserir(new Dominio.Entidades.Embarcador.PreCargas.PreCargaEstadoDestino()
                {
                    PreCarga = preCarga,
                    Estado = new Dominio.Entidades.Estado() { Sigla = siglaEstadoDestino }
                });

                totalDestinosAdicionados++;
            }

            foreach (int codigoRegiaoDestino in codigosRegioesDestino)
            {
                repositorioPreCargaRegiaoDestino.Inserir(new Dominio.Entidades.Embarcador.PreCargas.PreCargaRegiaoDestino()
                {
                    PreCarga = preCarga,
                    Regiao = new Dominio.Entidades.Embarcador.Localidades.Regiao() { Codigo = codigoRegiaoDestino }
                });

                totalDestinosAdicionados++;
            }

            if (configuracaoGeralCarga.UtilizarProgramacaoCarga && (totalDestinosAdicionados == 0))
                throw new ControllerException("Nenhum destino foi informado.");
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoFilial = Request.GetIntParam("Filial");

            if (codigoFilial <= 0)
                throw new ControllerException("Filial deve ser informada.");

            Repositorio.Embarcador.Filiais.Filial repositorio = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoFilial) ?? throw new ControllerException("Filial não encontrada.");
        }

        private Dominio.Entidades.RotaFrete ObterRotaFrete(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoRotaFrete = Request.GetIntParam("RotaFrete");

            if (codigoRotaFrete <= 0)
                return null;

            Repositorio.RotaFrete repositorio = new Repositorio.RotaFrete(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoRotaFrete);
        }

        private Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga ObterModeloVeicularCarga(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga");

            if (codigoModeloVeicularCarga <= 0)
                return null;

            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorio = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoModeloVeicularCarga) ?? throw new ControllerException("Modelo veicular não encontrado.");
        }

        private OrigemAlteracaoFilaCarregamento ObterOrigemAlteracaoFilaCarregamento()
        {
            return Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(TipoServicoMultisoftware);
        }

        private Dominio.Entidades.Embarcador.PreCargas.PreCarga ObterPreCargaAdicionar(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
            Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
            Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = new Dominio.Entidades.Embarcador.PreCargas.PreCarga();

            DateTime? dataPrevisaoEntrega = Request.GetNullableDateTimeParam("DataPrevisaoEntrega");

            preCarga.AdicionadaManualmente = true;
            preCarga.ProgramacaoCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga;
            preCarga.DataPrevisaoEntrega = configuracaoGeralCarga.UtilizarProgramacaoCarga ? Request.GetDateTimeParam("Data") : DateTime.Now;
            preCarga.DataPrevisaoEntregaManual = dataPrevisaoEntrega == DateTime.MinValue ? null : dataPrevisaoEntrega;
            preCarga.Empresa = ObterTransportador(unitOfWork, configuracaoEmbarcador);
            preCarga.Filial = ObterFilial(unitOfWork);
            preCarga.Rota = ObterRotaFrete(unitOfWork);
            preCarga.ModeloVeicularCarga = ObterModeloVeicularCarga(unitOfWork);
            preCarga.NumeroPreCarga = repositorioPreCarga.ObterProximoCodigo(preCarga.Filial?.Codigo ?? 0).ToString();
            preCarga.SituacaoPreCarga = configuracaoGeralCarga.UtilizarProgramacaoCarga ? SituacaoPreCarga.Nova : SituacaoPreCarga.AguardandoGeracaoCarga;
            preCarga.DataCriacaoPreCarga = DateTime.Now;
            preCarga.Operador = Usuario;
            preCarga.TipoDeCarga = ObterTipoCarga(unitOfWork);
            preCarga.TipoOperacao = ObterTipoOperacao(unitOfWork);
            preCarga.Observacao = Request.GetStringParam("Observacao");
            preCarga.Peso = Request.GetDecimalParam("Peso");
            preCarga.QuantidadePallet = Request.GetDecimalParam("QuantidadePallets");

            if (preCarga.DataPrevisaoEntrega < DateTime.Today)
                throw new ControllerException("A data de pré planejamento deve ser maior ou igual a data atual");

            PreencherMotoristasPreCargaAdicionar(preCarga, unitOfWork, configuracaoEmbarcador);
            PreencherVeiculos(preCarga, unitOfWork, configuracaoEmbarcador);

            return preCarga;
        }

        private Dominio.Entidades.Embarcador.Cargas.TipoDeCarga ObterTipoCarga(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoCarga = Request.GetIntParam("TipoCarga");

            if (codigoTipoCarga <= 0)
                return null;

            Repositorio.Embarcador.Cargas.TipoDeCarga repositorio = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoCarga) ?? throw new ControllerException("Tipo de carga não encontrado.");
        }

        private Dominio.Entidades.Embarcador.Pedidos.TipoOperacao ObterTipoOperacao(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

            if (codigoTipoOperacao <= 0)
                return null;

            Repositorio.Embarcador.Pedidos.TipoOperacao repositorio = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTipoOperacao) ?? throw new ControllerException("Tipo de operação não encontrado.");
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            int codigoTransportador = Request.GetIntParam("Transportador");

            if (codigoTransportador <= 0)
            {
                if (configuracaoEmbarcador.TransportadorObrigatorioPreCarga || configuracaoEmbarcador.DadosTransporteObrigatorioPreCarga)
                    throw new ControllerException("Transportador deve ser informado.");

                return null;
            }

            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);

            return repositorio.BuscarPorCodigo(codigoTransportador) ?? throw new ControllerException("Transportador não encontrado.");
        }

        private void PreencherMotoristasPreCargaAdicionar(Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador)
        {
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
            List<Dominio.Entidades.Usuario> listaMotoristaRetornar = new List<Dominio.Entidades.Usuario>();
            List<int> codigosMotoristas = Request.GetListParam<int>("Motoristas");

            foreach (int codigoMotorista in codigosMotoristas)
            {
                Dominio.Entidades.Usuario motoristaRetornar = repositorioMotorista.BuscarPorCodigo(codigoMotorista) ?? throw new ControllerException("Motorista não encontrado");

                if ((preCarga.Empresa != null) && (motoristaRetornar.Empresa != null) && (preCarga.Empresa.Codigo != motoristaRetornar.Empresa.Codigo))
                    throw new ControllerException($"Motorista {motoristaRetornar.Descricao} não pertence ao transportador selecionado.");

                listaMotoristaRetornar.Add(motoristaRetornar);
            }

            if ((listaMotoristaRetornar.Count == 0) && configuracaoEmbarcador.DadosTransporteObrigatorioPreCarga)
                throw new ControllerException($"Informe pelo menos um motorista.");

            preCarga.Motoristas = listaMotoristaRetornar;
        }

        #endregion
    }
}
