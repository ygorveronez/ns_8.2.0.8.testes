using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/ExcecaoCapacidadeCarregamento")]
    public class ExcecaoCapacidadeCarregamentoController : BaseController
    {
		#region Construtores

		public ExcecaoCapacidadeCarregamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = new Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento();

                PreencherExcecaoCapacidadeCarregamento(excecao, unitOfWork);

                repositorio.Inserir(excecao, Auditado);

                AtualizarPeriodosCarregamento(excecao, null, unitOfWork);
                AtualizarPrevisoesCarregamento(excecao, null, unitOfWork);
                ValidarExcecaoCapacidadeCarregamento(excecao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                PreencherExcecaoCapacidadeCarregamento(excecao, unitOfWork);

                Dominio.Entidades.Auditoria.HistoricoObjeto historico = repositorio.Atualizar(excecao, Auditado);

                AtualizarPeriodosCarregamento(excecao, historico, unitOfWork);
                AtualizarPrevisoesCarregamento(excecao, historico, unitOfWork);
                ValidarExcecaoCapacidadeCarregamento(excecao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAtualizar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                return new JsonpResult(new
                {
                    excecao.Codigo,
                    CentroCarregamento = new
                    {
                        excecao.CentroCarregamento.Codigo,
                        excecao.CentroCarregamento.Descricao,
                        excecao.CentroCarregamento.TipoCapacidadeCarregamentoPorPeso,
                        excecao.CentroCarregamento.TipoCapacidadeCarregamento
                    },
                    DataInicial = excecao.Data.ToString("dd/MM/yyyy"),
                    DataFinal = excecao.DataFinal?.ToDateString() ?? string.Empty,
                    excecao.Descricao,
                    excecao.TipoAbrangencia,
                    Segunda = excecao.DisponivelSegunda,
                    Terca = excecao.DisponivelTerca,
                    Quarta = excecao.DisponivelQuarta,
                    Quinta = excecao.DisponivelQuinta,
                    Sexta = excecao.DisponivelSexta,
                    Sabado = excecao.DisponivelSabado,
                    Domingo = excecao.DisponivelDomingo,
                    CapacidadeCarregamento = excecao.CapacidadeCarregamento > 0 ? excecao.CapacidadeCarregamento.ToString("n0") : "",
                    PeriodosCarregamento = (
                        from periodo in excecao.PeriodosCarregamento
                        select new
                        {
                            periodo.Codigo,
                            DiaSemana = 0,
                            HoraInicio = periodo.HoraInicio.ToString(@"hh\:mm"),
                            HoraTermino = periodo.HoraTermino.ToString(@"hh\:mm"),
                            periodo.ToleranciaExcessoTempo,
                            periodo.CapacidadeCarregamentoSimultaneo,
                            CapacidadeCarregamentoVolume = (periodo.CapacidadeCarregamentoVolume > 0) ? periodo.CapacidadeCarregamentoVolume.ToString("n0") : "",
                            ContainerTipoOperacaoSimultaneo = (
                                from obj in periodo.TipoOperacaoSimultaneo
                                select new
                                {
                                    TipoOperacaoSimultaneo = new
                                    {
                                        obj.TipoOperacao.Codigo,
                                        obj.TipoOperacao.Descricao,
                                    },
                                    CapacidadeSimultaneoTipoOperacao = obj.CapacidadeCarregamentoSimultaneo
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    PrevisoesCarregamento = (
                        from previsao in excecao.PrevisoesCarregamento
                        select new
                        {
                            previsao.Codigo,
                            Descricao = excecao.Descricao ?? string.Empty,
                            DiaSemana = 0,
                            previsao.QuantidadeCargas,
                            previsao.QuantidadeCargasExcedentes,
                            Rota = new { previsao.Rota.Codigo, previsao.Rota.Descricao },
                            ModelosVeiculos = (
                                from modelo in previsao.ModelosVeiculos
                                select new
                                {
                                    modelo.Codigo,
                                    modelo.Descricao
                                }
                            ).ToList()
                        }
                    ).ToList(),
                    excecao.CapacidadeCarregamentoVolume,
                    excecao.CapacidadeCarregamentoCubagem,
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PrevisaoCarregamento repPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamento repPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (excecao == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                unitOfWork.Start();

                repositorio.DeletarPorCodigo(excecao.Codigo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, excecao, Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.ExcluiuRegistro, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRemover);
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

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
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

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarPeriodosCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, dynamic periodosCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo repPeriodoCarregamentoTipoOperacaoSimultaneo = new Repositorio.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);

            if (excecao.PeriodosCarregamento == null)
                excecao.PeriodosCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento>();

            foreach (var periodoCarregamento in periodosCarregamento)
            {
                int? codigo = ((string)periodoCarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodo;

                if (codigo.HasValue)
                    periodo = repositorioPeriodoCarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.NaoFoiPossivelEncontrarPeriodoCarregamento);
                else
                    periodo = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento();

                periodo.CentroCarregamento = excecao.CentroCarregamento;
                periodo.ExcecaoCapacidadeCarregamento = excecao;
                periodo.CapacidadeCarregamentoVolume = ((string)periodoCarregamento.CapacidadeCarregamentoVolume).ToInt();
                periodo.CapacidadeCarregamentoSimultaneo = ((string)periodoCarregamento.CapacidadeCarregamentoSimultaneo).ToInt();
                periodo.Dia = ((string)periodoCarregamento.DiaSemana).ToEnum<DiaSemana>();
                periodo.HoraInicio = ((string)periodoCarregamento.HoraInicio).ToTime();
                periodo.HoraTermino = ((string)periodoCarregamento.HoraTermino).ToTime();
                periodo.ToleranciaExcessoTempo = ((string)periodoCarregamento.ToleranciaExcessoTempo).ToInt();

                if (periodo.TipoOperacaoSimultaneo == null)
                    periodo.TipoOperacaoSimultaneo = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo>();

                if (periodo.Codigo > 0)
                    repositorioPeriodoCarregamento.Atualizar(periodo, historico != null ? Auditado : null, historico);
                else
                    repositorioPeriodoCarregamento.Inserir(periodo, historico != null ? Auditado : null, historico);

                List<int> codigosTipoOperacaoAdicionadoOuAtualizado = new List<int>();
                foreach (var periodoCarregamentoTipoOperacaoSimultaneo in periodoCarregamento.ContainerTipoOperacaoSimultaneo)
                {
                    int codigoTipoOperacao = ((string)periodoCarregamentoTipoOperacaoSimultaneo.TipoOperacaoSimultaneo.Codigo).ToInt();
                    if (codigoTipoOperacao == 0)
                        throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.NaoFoiPossivelEncontrarPeriodoCarregamentoTipoOperacaoSimultaneo);

                    Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo tipoOperacaoSimultaneo = repPeriodoCarregamentoTipoOperacaoSimultaneo.BuscarPorCodigoPeriodoETipoOperacao(periodo.Codigo, codigoTipoOperacao);
                    if (tipoOperacaoSimultaneo == null)
                        tipoOperacaoSimultaneo = new Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo();
                    else
                        tipoOperacaoSimultaneo.Initialize();

                    tipoOperacaoSimultaneo.PeriodoCarregamento = periodo;
                    tipoOperacaoSimultaneo.TipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);
                    tipoOperacaoSimultaneo.CapacidadeCarregamentoSimultaneo = ((string)periodoCarregamentoTipoOperacaoSimultaneo.CapacidadeSimultaneoTipoOperacao).ToInt();

                    if (tipoOperacaoSimultaneo.Codigo > 0)
                        repPeriodoCarregamentoTipoOperacaoSimultaneo.Atualizar(tipoOperacaoSimultaneo, historico != null ? Auditado : null, historico);
                    else
                        repPeriodoCarregamentoTipoOperacaoSimultaneo.Inserir(tipoOperacaoSimultaneo, historico != null ? Auditado : null, historico);

                    codigosTipoOperacaoAdicionadoOuAtualizado.Add(tipoOperacaoSimultaneo.Codigo);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamentoTipoOperacaoSimultaneo> registrosParaDeletar = (from o in periodo.TipoOperacaoSimultaneo
                                                                                                                               where !codigosTipoOperacaoAdicionadoOuAtualizado.Contains(o.Codigo)
                                                                                                                               select o).ToList();

                foreach (var registro in registrosParaDeletar)
                    repPeriodoCarregamentoTipoOperacaoSimultaneo.Deletar(registro, historico != null ? Auditado : null, historico);
            }
        }

        private void AdicionarOuAtualizarPrevisoesCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, dynamic previsoesCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.RotaFrete repositorioRotaFrete = new Repositorio.RotaFrete(unitOfWork);

            if (excecao.PrevisoesCarregamento == null)
                excecao.PrevisoesCarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento>();

            foreach (var previsaoCarregamento in previsoesCarregamento)
            {
                int? codigo = ((string)previsaoCarregamento.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsao;

                if (codigo.HasValue)
                    previsao = repositorioPrevisaoCarregamento.BuscarPorCodigo(codigo.Value, auditavel: true) ?? throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.NaoFoiPossivelEncontrarPrevisaoCarregamento);
                else
                    previsao = new Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento();

                previsao.CentroCarregamento = excecao.CentroCarregamento;
                previsao.ExcecaoCapacidadeCarregamento = excecao;
                previsao.QuantidadeCargas = ((string)previsaoCarregamento.QuantidadeCargas).ToInt();
                previsao.QuantidadeCargasExcedentes = ((string)previsaoCarregamento.QuantidadeCargasExcedentes).ToInt();
                previsao.Dia = ((string)previsaoCarregamento.DiaSemana).ToEnum<DiaSemana>();
                previsao.Descricao = (string)previsaoCarregamento.Descricao;
                previsao.Rota = repositorioRotaFrete.BuscarPorCodigo(((string)previsaoCarregamento.Rota.Codigo).ToInt()) ?? throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.NaoPossivelEncontrarRota);
                previsao.ModelosVeiculos = new List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga>();

                var modelosVeiculos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(previsaoCarregamento.ModelosVeiculos.ToString());

                foreach (var modeloVeiculo in modelosVeiculos)
                    previsao.ModelosVeiculos.Add(repositorioModeloVeicularCarga.BuscarPorCodigo(((string)modeloVeiculo.Codigo).ToInt()) ?? throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.NaoPossivelEncontrarModeloVeicularCarga));

                if (previsao.Codigo > 0)
                    repositorioPrevisaoCarregamento.Atualizar(previsao, historico != null ? Auditado : null, historico);
                else
                    repositorioPrevisaoCarregamento.Inserir(previsao, historico != null ? Auditado : null, historico);
            }
        }

        private void ValidarExcecaoCapacidadeCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repExcecaoCapacidadeCarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);

            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecaoInvalida = repExcecaoCapacidadeCarregamento.BuscarExcecaoIncompativel(excecao.Codigo, excecao.CentroCarregamento.Codigo, excecao.TipoAbrangencia, excecao.Data, excecao.DataFinal, excecao.DisponivelSegunda, excecao.DisponivelTerca, excecao.DisponivelQuarta, excecao.DisponivelQuinta, excecao.DisponivelSexta, excecao.DisponivelSabado, excecao.DisponivelDomingo);

            if (excecaoInvalida != null)
                throw new ControllerException($"Já existe uma exceção ({excecaoInvalida.Descricao}) para {(excecaoInvalida.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Dia ? "a mesma data" : "o mesmo período")}.");
        }

        private void AtualizarPeriodosCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic periodosCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PeriodosCarregamento"));

            ExcluirPeriodosCarregamentoRemovidos(excecao, periodosCarregamento, historico, unitOfWork);
            AdicionarOuAtualizarPeriodosCarregamento(excecao, periodosCarregamento, historico, unitOfWork);
        }

        private void AtualizarPrevisoesCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic previsoesCarregamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PrevisoesCarregamento"));

            ExcluirPrevisoesCarregamentoRemovidos(excecao, previsoesCarregamento, historico, unitOfWork);
            AdicionarOuAtualizarPrevisoesCarregamento(excecao, previsoesCarregamento, historico, unitOfWork);
        }

        private void ExcluirPeriodosCarregamentoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, dynamic periodosCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PeriodoCarregamento repositorioPeriodoCarregamento = new Repositorio.Embarcador.Logistica.PeriodoCarregamento(unitOfWork);

            if (excecao.PeriodosCarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var periodoCarregamento in periodosCarregamento)
                {
                    int? codigo = ((string)periodoCarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento> periodosCarregamentoDeletar = (from o in excecao.PeriodosCarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoCarregamento periodoCarregamento in periodosCarregamentoDeletar)
                    repositorioPeriodoCarregamento.Deletar(periodoCarregamento, historico != null ? Auditado : null, historico);
            }
        }

        private void ExcluirPrevisoesCarregamentoRemovidos(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, dynamic periodosCarregamento, Dominio.Entidades.Auditoria.HistoricoObjeto historico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.PrevisaoCarregamento repositorioPrevisaoCarregamento = new Repositorio.Embarcador.Logistica.PrevisaoCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa repositorioReservaCargaGrupoPessoa = new Repositorio.Embarcador.Logistica.ReservaCargaGrupoPessoa(unitOfWork);

            if (excecao.PrevisoesCarregamento?.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var previsaoCarregamento in periodosCarregamento)
                {
                    int? codigo = ((string)previsaoCarregamento.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        codigos.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento> previsoesCarregamentoDeletar = (from o in excecao.PrevisoesCarregamento where !codigos.Contains(o.Codigo) select o).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.PrevisaoCarregamento previsaoCarregamento in previsoesCarregamentoDeletar)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa> reservasCargaGrupoPessoa = repositorioReservaCargaGrupoPessoa.BuscarPorPrevisao(previsaoCarregamento.Codigo);

                    foreach (Dominio.Entidades.Embarcador.Logistica.ReservaCargaGrupoPessoa reservaCargaGrupoPessoa in reservasCargaGrupoPessoa)
                        repositorioReservaCargaGrupoPessoa.Deletar(reservaCargaGrupoPessoa);

                    repositorioPrevisaoCarregamento.Deletar(previsaoCarregamento, historico != null ? Auditado : null, historico);
                }
            }
        }

        private void PreencherExcecaoCapacidadeCarregamento(Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento excecao, Repositorio.UnitOfWork unitOfWork)
        {
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = repositorioCentroCarregamento.BuscarPorCodigo(codigoCentroCarregamento) ?? throw new ControllerException(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.DataFinalObrigatoria);

            excecao.CentroCarregamento = centroCarregamento;
            excecao.TipoAbrangencia = Request.GetEnumParam<TipoAbrangenciaExcecaoCapacidadeCarregamento>("TipoAbrangencia");
            bool excecaoPorPeriodo = excecao.TipoAbrangencia == TipoAbrangenciaExcecaoCapacidadeCarregamento.Periodo;
            excecao.Data = Request.GetDateTimeParam("DataInicial");
            excecao.DataFinal = null;
            if (excecaoPorPeriodo)
                excecao.DataFinal = Request.GetNullableDateTimeParam("DataFinal") ?? throw new ControllerException("Data Final Obrigatória.");
            excecao.Descricao = Request.GetStringParam("Descricao");
            excecao.CapacidadeCarregamento = Request.GetIntParam("CapacidadeCarregamento");
            excecao.DisponivelSegunda = excecaoPorPeriodo ? Request.GetBoolParam("Segunda") : false;
            excecao.DisponivelTerca = excecaoPorPeriodo ? Request.GetBoolParam("Terca") : false;
            excecao.DisponivelQuarta = excecaoPorPeriodo ? Request.GetBoolParam("Quarta") : false;
            excecao.DisponivelQuinta = excecaoPorPeriodo ? Request.GetBoolParam("Quinta") : false;
            excecao.DisponivelSexta = excecaoPorPeriodo ? Request.GetBoolParam("Sexta") : false;
            excecao.DisponivelSabado = excecaoPorPeriodo ? Request.GetBoolParam("Sabado") : false;
            excecao.DisponivelDomingo = excecaoPorPeriodo ? Request.GetBoolParam("Domingo") : false;
            excecao.CapacidadeCarregamentoVolume = Request.GetIntParam("CapacidadeCarregamentoVolume");
            excecao.CapacidadeCarregamentoCubagem = Request.GetIntParam("CapacidadeCarregamentoCubagem");

            if (excecao.Codigo == 0)
                excecao.Usuario = Usuario;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento()
            {
                CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.Data, "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.DiaSemana, "DiaSemana", 20, Models.Grid.Align.left, true).Ord(false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Logistica.ExcecaoCapacidadeCarregamento.CentroCarregamento, "CentroCarregamento", 20, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaExcecaoCapacidadeCarregamento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento repositorio = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeCarregamento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento> listaExcecaoCapacidadeCarregamento = (totalRegistros > 0) ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeCarregamento>();

                var listaExcecaoCapacidadeCarregamentoRetornar = (
                    from excecao in listaExcecaoCapacidadeCarregamento
                    select new
                    {
                        excecao.Codigo,
                        Data = excecao.DescricaoData,
                        excecao.Descricao,
                        DiaSemana = excecao.DescricaoDiaSemana,
                        CentroCarregamento = excecao.CentroCarregamento.Descricao
                    }
                ).ToList();

                grid.AdicionaRows(listaExcecaoCapacidadeCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "CentroCarregamento")
                return "CentroCarregamento.Descricao";

            return propriedadeOrdenar;
        }

        #endregion
    }
}
