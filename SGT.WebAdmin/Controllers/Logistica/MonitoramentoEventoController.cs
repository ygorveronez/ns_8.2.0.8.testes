using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/MonitoramentoEvento")]
    public class MonitoramentoEventoController : BaseController
    {
		#region Construtores

		public MonitoramentoEventoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho monitoramentoEventoGatilho = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho();
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);

                PreencherMonitoramentoEvento(monitoramentoEvento, unitOfWork);
                PreencherMonitoramentoEventoGatilho(monitoramentoEvento, monitoramentoEventoGatilho, unitOfWork);

                //Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento jaExisteMonitormentoEventoAtivo = repMonitoramentoEvento.BuscarAtivo(monitoramentoEvento.TipoAlerta);
                //if (jaExisteMonitormentoEventoAtivo != null)
                //    return new JsonpResult(false, true, "Já existe um Evento de monitoramento ativo para o tipo selecionado"); //#52390 permitir cadastrar mesmo alerta com gatilhos diferentes.

                unitOfWork.Start();

                repMonitoramentoEvento.Inserir(monitoramentoEvento, Auditado);
                new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork).Inserir(monitoramentoEventoGatilho);
                bool restringirHorario = Request.GetBoolParam("RestringirHorario");
                if (restringirHorario)
                {
                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario monitoramentoEventoHorario = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario();
                    PreencherMonitoramentoEventoHorario(monitoramentoEvento, monitoramentoEventoHorario);
                    new Repositorio.Embarcador.Logistica.MonitoramentoEventoHorario(unitOfWork).Inserir(monitoramentoEventoHorario);
                }

                AdicionarOuAtualizarTratativas(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarStatusViagem(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTipoDeCarga(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTipoDeOperacao(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTratativaAutomatica(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarCausa(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarAcaoTratativa(monitoramentoEvento, unitOfWork);
                SalvarTiposIntegracao(monitoramentoEvento, unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
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
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(codigo, auditavel: true);

                Repositorio.Embarcador.Logistica.MonitoramentoEventoHorario repMonitoramentoEventoHorario = new Repositorio.Embarcador.Logistica.MonitoramentoEventoHorario(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario monitoramentoEventoHorario = repMonitoramentoEventoHorario.BuscarPorEvento(monitoramentoEvento);

                if (monitoramentoEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherMonitoramentoEvento(monitoramentoEvento, unitOfWork);

                //Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento jaExisteMonitormentoEventoAtivo = repMonitoramentoEvento.BuscarAtivo(monitoramentoEvento.TipoAlerta);
                //if (jaExisteMonitormentoEventoAtivo != null && jaExisteMonitormentoEventoAtivo.Codigo != monitoramentoEvento.Codigo)
                //    return new JsonpResult(false, true, "Já existe um Evento de monitoramento ativo para o tipo selecionado"); //#52390 permitir cadastrar mesmo alerta com gatilhos diferentes.

                monitoramentoEvento.Gatilho.Initialize();
                PreencherMonitoramentoEventoGatilho(monitoramentoEvento, monitoramentoEvento.Gatilho, unitOfWork);

                unitOfWork.Start();

                List<Dominio.Entidades.Auditoria.HistoricoPropriedade> listaAlteracoes = new List<Dominio.Entidades.Auditoria.HistoricoPropriedade>();

                listaAlteracoes.AddRange(monitoramentoEvento.Gatilho.GetChanges());
                listaAlteracoes.AddRange(monitoramentoEvento.GetChanges());

                repMonitoramentoEvento.Atualizar(monitoramentoEvento);
                new Repositorio.Embarcador.Logistica.MonitoramentoEventoGatilho(unitOfWork).Atualizar(monitoramentoEvento.Gatilho);

                bool restringirHorario = Request.GetBoolParam("RestringirHorario");
                if (restringirHorario)
                {
                    if (monitoramentoEventoHorario == null)
                    {
                        monitoramentoEventoHorario = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario();
                        PreencherMonitoramentoEventoHorario(monitoramentoEvento, monitoramentoEventoHorario);
                        new Repositorio.Embarcador.Logistica.MonitoramentoEventoHorario(unitOfWork).Inserir(monitoramentoEventoHorario);
                    }
                    else
                    {
                        PreencherMonitoramentoEventoHorario(monitoramentoEvento, monitoramentoEventoHorario);
                        monitoramentoEventoHorario.Initialize();
                        listaAlteracoes.AddRange(monitoramentoEventoHorario.GetChanges());
                        new Repositorio.Embarcador.Logistica.MonitoramentoEventoHorario(unitOfWork).Atualizar(monitoramentoEventoHorario);
                    }
                }
                else if (monitoramentoEventoHorario != null)
                {
                    repMonitoramentoEventoHorario.Deletar(monitoramentoEventoHorario);
                }

                AdicionarOuAtualizarTratativas(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarStatusViagem(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTipoDeCarga(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTipoDeOperaca(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarTratativaAutomatica(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarCausa(monitoramentoEvento, unitOfWork);
                AdicionarOuAtualizarAcaoTratativa(monitoramentoEvento, unitOfWork);
                SalvarTiposIntegracao(monitoramentoEvento, unitOfWork);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, listaAlteracoes, "Atualizado", unitOfWork);

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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repositorio = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa repositorioMonitoramentoEventoCausa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorioAlertaTratativaAcao = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (monitoramentoEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao repMonitoramentoEventoTipoIntegracao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao(unitOfWork);
                List<TipoIntegracao> tiposIntegracao = repMonitoramentoEventoTipoIntegracao.BuscarTipoIntegracaoPorMonitoramentoEvento(monitoramentoEvento.Codigo);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> causasAlertas = repositorioMonitoramentoEventoCausa.BuscarPorTipoAlerta(monitoramentoEvento.TipoAlerta);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> acoesTratativa = repositorioAlertaTratativaAcao.BuscarPorTipoDeAlerta(monitoramentoEvento.TipoAlerta, false);

                return new JsonpResult(new
                {
                    Evento = new
                    {
                        monitoramentoEvento.Codigo,
                        monitoramentoEvento.Descricao,
                        Status = monitoramentoEvento.Ativo,
                        monitoramentoEvento.TipoAlerta,
                        monitoramentoEvento.TipoMonitoramentoEvento,
                        monitoramentoEvento.Prioridade,
                        monitoramentoEvento.Cor,
                        monitoramentoEvento.ExibirApp,
                        monitoramentoEvento.NaoGerarParaPreCarga,
                        monitoramentoEvento.GerarAlertaAcompanhamentoCarga,
                        monitoramentoEvento.IntegrarEvento,
                        TipoIntegracao = tiposIntegracao.ToList(),
                        monitoramentoEvento.ExibirDescricaoAlerta,
                        monitoramentoEvento.ExibirDataeHoraGeracaoAlerta,
                        monitoramentoEvento.ConsiderarParaSemaforo,
                        monitoramentoEvento.GerarAtendimento,
                        MotivoChamado = new { Codigo = monitoramentoEvento.MotivoChamado?.Codigo ?? 0, Descricao = monitoramentoEvento.MotivoChamado?.Descricao ?? string.Empty },
                        monitoramentoEvento.ExibirControleEntrega,
                        monitoramentoEvento.QuandoProcessar,
                        VerificarStatusViagem = (monitoramentoEvento.VerificarStatusViagem != VerificarStatusViagem.Todos) ? monitoramentoEvento.VerificarStatusViagem : VerificarStatusViagem.NaoVerificar,
                        VerificarTipoDeCarga = (monitoramentoEvento.VerificarTipoDeCarga != VerificarTipoDeCarga.Todos) ? monitoramentoEvento.VerificarTipoDeCarga : VerificarTipoDeCarga.NaoVerificar,
                        VerificarTipoDeOperacao = (monitoramentoEvento.VerificarTipoDeOperacao != VerificarTipoDeOperacao.Todos) ? monitoramentoEvento.VerificarTipoDeOperacao : VerificarTipoDeOperacao.NaoVerificar
                    },
                    Gatilho = new
                    {
                        monitoramentoEvento.Gatilho.Raio,
                        monitoramentoEvento.Gatilho.Tempo,
                        monitoramentoEvento.Gatilho.Velocidade,
                        monitoramentoEvento.Gatilho.Velocidade2,
                        monitoramentoEvento.Gatilho.TempoEvento,
                        monitoramentoEvento.Gatilho.TempoEvento2,
                        monitoramentoEvento.Gatilho.Quantidade,
                        monitoramentoEvento.Gatilho.DataBase,
                        monitoramentoEvento.Gatilho.DataReferencia,
                        monitoramentoEvento.Gatilho.ConsiderarApenasDataNaReferencia,
                        monitoramentoEvento.Gatilho.TratativaAutomatica,
                        monitoramentoEvento.Gatilho.EventoContinuo,
                        monitoramentoEvento.Gatilho.TempoReferenteaDataCarregamentoCarga,
                        monitoramentoEvento.Gatilho.ValidarApenasCargasNaoIniciadas,
                        PontosDeApoio = monitoramentoEvento.Gatilho.PontosDeApoio.Select(p => new
                        {
                            Codigo = p.Codigo,
                            Descricao = p.Descricao,
                            Filial = p.Filial?.Descricao ?? string.Empty,
                            Tipo = p.Tipo.ObterDescricao()
                        }).ToList(),
                        RaioProximidade = new
                        {
                            Codigo = monitoramentoEvento.Gatilho.RaioProximidade?.Codigo ?? 0,
                            Descricao = monitoramentoEvento.Gatilho.RaioProximidade?.Identificacao ?? string.Empty
                        },
                    },
                    Horario = new
                    {
                        RestringirHorario = (monitoramentoEvento.Horario != null && monitoramentoEvento.Horario != null),
                        HoraInicio = (monitoramentoEvento.Horario != null) ? monitoramentoEvento.Horario.HoraInicio.ToString(@"hh\:mm") : "",
                        HoraFim = (monitoramentoEvento.Horario != null) ? monitoramentoEvento.Horario.HoraFim.ToString(@"hh\:mm") : "",
                    },
                    StatusViagem = (
                        from statusViagem in monitoramentoEvento.StatusViagem
                        select new
                        {
                            statusViagem.Codigo,
                            CodigoMonitoramentoStatusViagem = statusViagem.MonitoramentoStatusViagem.Codigo,
                            OrdemMonitoramentoStatusViagem = statusViagem.MonitoramentoStatusViagem.Ordem,
                            DescricaoMonitoramentoStatusViagem = statusViagem.MonitoramentoStatusViagem.Descricao,
                            DescricaoAtivoMonitoramentoStatusViagem = statusViagem.MonitoramentoStatusViagem.DescricaoAtivo
                        }
                    ).ToList(),
                    TipoDeCarga = (
                        from tipoDeCarga in monitoramentoEvento.TipoDeCarga
                        select new
                        {
                            tipoDeCarga.Codigo,
                            CodigoTipoDeCarga = tipoDeCarga.TipoDeCarga.Codigo,
                            DescricaoTipoDeCarga = tipoDeCarga.TipoDeCarga.Descricao,
                            DescricaoAtivoTipoDeCarga = tipoDeCarga.TipoDeCarga.DescricaoAtivo
                        }
                    ).ToList(),
                    TipoDeOperacao = (
                        from tipoDeOperacao in monitoramentoEvento.TipoDeOperacao
                        select new
                        {
                            tipoDeOperacao.Codigo,
                            CodigoTipoDeOperacao = tipoDeOperacao.TipoDeOperacao.Codigo,
                            DescricaoTipoDeOperacao = tipoDeOperacao.TipoDeOperacao.Descricao,
                            DescricaoAtivoTipoDeOperacao = tipoDeOperacao.TipoDeOperacao.DescricaoAtivo
                        }
                    ).ToList(),
                    Tratativas = (
                        from tratativa in monitoramentoEvento.Tratativas
                        select new
                        {
                            tratativa.Codigo,
                            CodigoCategoriaResponsavel = tratativa.CategoriaResponsavel.Codigo,
                            DescricaoCategoriaResponsavel = tratativa.CategoriaResponsavel.Descricao,
                            EnviarEmail = tratativa.EnvioEmail,
                            EnviarEmailTransportador = tratativa.EnvioEmailTransportador,
                            EnviarEmailCliente = tratativa.EnvioEmailCliente,
                            Sequencia = tratativa.Sequencia,
                            Tempo = tratativa.TempoEmMinutos
                        }
                    ).ToList(),
                    TratativaAutomatica = (
                        from tratativaAutomatica in monitoramentoEvento.TratativaAutomatica
                        select new
                        {
                            Codigo = tratativaAutomatica?.Codigo ?? 0,
                            TratativaAutomatica = true,
                            GatilhoTratativaAutomatica = tratativaAutomatica?.Gatilho ?? 0
                        }
                    ).ToList(),
                    Causa = (
                        from causa in causasAlertas
                        select new
                        {
                            causa.Codigo,
                            causa.Descricao,
                            causa.Ativo,
                            AtivoDescricao = causa.DescricaoAtivo
                        }
                    ).ToList(),
                    AcaoTratativa = (
                        from acao in acoesTratativa
                        select new
                        {
                            acao.Codigo,
                            acao.Descricao,
                            Status = acao.Ativo,
                            StatusDescricao = acao.DescricaoAtivo,
                            AcaoMonitorada = acao.AcaoMonitorada,
                            AcaoMonitoradaDescricao = acao.AcaoMonitorada ? "Sim" : "Não"
                        }
                    ).ToList()
                });
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(codigo, auditavel: true);

                if (monitoramentoEvento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                repMonitoramentoEvento.Deletar(monitoramentoEvento, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover dados.");
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

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
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

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterCausasEAcoesPorTipoAlerta()
        {
            try
            {
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

                TipoAlerta tipoAlerta = Request.GetEnumParam<TipoAlerta>("TipoAlerta");

                Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa repositorioMonitoramentoEventoCausa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorioAlertaTratativaAcao = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> causasAlertas = repositorioMonitoramentoEventoCausa.BuscarPorTipoAlerta(tipoAlerta);
                List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> acoesTratativa = repositorioAlertaTratativaAcao.BuscarPorTipoDeAlerta(tipoAlerta, false);

                return new JsonpResult(new
                {
                    Causa = (
                        from causa in causasAlertas
                        select new
                        {
                            causa.Codigo,
                            causa.Descricao,
                            causa.Ativo,
                            AtivoDescricao = causa.DescricaoAtivo
                        }
                    ).ToList(),
                    AcaoTratativa = (
                        from acao in acoesTratativa
                        select new
                        {
                            acao.Codigo,
                            acao.Descricao,
                            Status = acao.Ativo,
                            StatusDescricao = acao.DescricaoAtivo,
                            AcaoMonitorada = acao.AcaoMonitorada,
                            AcaoMonitoradaDescricao = acao.AcaoMonitorada ? "Sim" : "Não"
                        }
                    ).ToList()
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar causas e ações de tratativas.");
            }
        }

        #endregion

        #region Métodos Privados

        private void AdicionarOuAtualizarTratativas(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tratativas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Tratativas"));

            ExcluirTratativasRemovidas(monitoramentoEvento, tratativas, unitOfWork);
            InserirTratativasAdicionadas(monitoramentoEvento, tratativas, unitOfWork);
        }

        private void AdicionarOuAtualizarStatusViagem(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic statusViagem = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("StatusViagem"));
            if (monitoramentoEvento.VerificarStatusViagem == VerificarStatusViagem.NaoVerificar) statusViagem = new List<dynamic>();
            ExcluirStatusViagemRemovidos(monitoramentoEvento, statusViagem, unitOfWork);
            if (monitoramentoEvento.VerificarStatusViagem != VerificarStatusViagem.NaoVerificar) InserirStatusViagemAdicionados(monitoramentoEvento, statusViagem, unitOfWork);
        }

        private void AdicionarOuAtualizarTipoDeCarga(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tiposDeCarga = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoDeCarga"));
            if (monitoramentoEvento.VerificarTipoDeCarga == VerificarTipoDeCarga.NaoVerificar) tiposDeCarga = new List<dynamic>();
            ExcluirTipoDeCargaRemovidos(monitoramentoEvento, tiposDeCarga, unitOfWork);
            if (monitoramentoEvento.VerificarTipoDeCarga != VerificarTipoDeCarga.NaoVerificar) InserirTipoDeCargaAdicionados(monitoramentoEvento, tiposDeCarga, unitOfWork);
        }
        private void AdicionarOuAtualizarTipoDeOperaca(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tiposDeOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoDeOperacao"));
            if (monitoramentoEvento.VerificarTipoDeOperacao == VerificarTipoDeOperacao.NaoVerificar) tiposDeOperacao = new List<dynamic>();
            ExcluirTipoDeOperacaoRemovidos(monitoramentoEvento, tiposDeOperacao, unitOfWork);
            if (monitoramentoEvento.VerificarTipoDeOperacao != VerificarTipoDeOperacao.NaoVerificar)
                InserirTipoDeOperacaoAdicionados(monitoramentoEvento, tiposDeOperacao, unitOfWork);
        }

        private void AdicionarOuAtualizarTratativaAutomatica(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tratativaAutomatica = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TratativaAutomatica"));
            if (tratativaAutomatica != null)
            {
                bool? temTratativaAutomatica = ((string)tratativaAutomatica.TratativaAutomatica).ToNullableBool();

                Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica repMonitoramentoEventoTratativaAutomatica = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica monitoramentoEventoTratativaAutomatica = null;

                int? codigo = ((string)tratativaAutomatica.Codigo).ToNullableInt();
                if (codigo.HasValue && codigo > 0)
                    monitoramentoEventoTratativaAutomatica = repMonitoramentoEventoTratativaAutomatica.BuscarPorCodigo(codigo.Value);
                else
                    monitoramentoEventoTratativaAutomatica = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativaAutomatica();

                monitoramentoEventoTratativaAutomatica.MonitoramentoEvento = monitoramentoEvento;
                monitoramentoEventoTratativaAutomatica.Gatilho = ((string)tratativaAutomatica.GatilhoTratativaAutomatica).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TratativaAutomaticaMonitoramentoEvento>();

                if (codigo.HasValue && codigo > 0)
                {
                    if (temTratativaAutomatica ?? false)
                        repMonitoramentoEventoTratativaAutomatica.Atualizar(monitoramentoEventoTratativaAutomatica);
                    else
                        repMonitoramentoEventoTratativaAutomatica.Deletar(monitoramentoEventoTratativaAutomatica);
                }
                else if (temTratativaAutomatica ?? false)
                    repMonitoramentoEventoTratativaAutomatica.Inserir(monitoramentoEventoTratativaAutomatica);
            }
        }

        private void AdicionarOuAtualizarCausa(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic causas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Causa"));

            Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa repositorioMonitoramentoEventoCausa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> listaCausas = repositorioMonitoramentoEventoCausa.BuscarPorTipoAlerta(monitoramentoEvento.TipoAlerta);
            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> listaCausasSalvas = new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa>();

            foreach (dynamic causa in causas)
            {
                int? codigo = ((string)causa.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa monitoramentoEventoCausa;

                if (codigo.HasValue)
                    monitoramentoEventoCausa = repositorioMonitoramentoEventoCausa.BuscarPorCodigo(codigo.Value, false);
                else
                    monitoramentoEventoCausa = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa();

                monitoramentoEventoCausa.Descricao = causa.Descricao;
                monitoramentoEventoCausa.Ativo = causa.Ativo;
                monitoramentoEventoCausa.TipoAlerta = monitoramentoEvento.TipoAlerta;

                if (codigo.HasValue)
                    repositorioMonitoramentoEventoCausa.Atualizar(monitoramentoEventoCausa);
                else
                    repositorioMonitoramentoEventoCausa.Inserir(monitoramentoEventoCausa);

                listaCausasSalvas.Add(monitoramentoEventoCausa);
            }

            foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa causa in listaCausas)
            {
                if (!listaCausasSalvas.Exists(c => c.Codigo == causa.Codigo))
                    repositorioMonitoramentoEventoCausa.Deletar(causa);
            }
        }

        private void AdicionarOuAtualizarAcaoTratativa(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic causas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("AcaoTratativa"));

            Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorioAlertaTratativaAcao = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);

            foreach (dynamic causa in causas)
            {
                int? codigo = ((string)causa.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao monitoramentoEventoAcaoTratatica;

                if (codigo.HasValue)
                    monitoramentoEventoAcaoTratatica = repositorioAlertaTratativaAcao.BuscarPorCodigo(codigo.Value, false);
                else
                    monitoramentoEventoAcaoTratatica = new Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao();

                monitoramentoEventoAcaoTratatica.Descricao = causa.Descricao;
                monitoramentoEventoAcaoTratatica.Ativo = causa.Status;
                monitoramentoEventoAcaoTratatica.AcaoMonitorada = causa.AcaoMonitorada;
                monitoramentoEventoAcaoTratatica.TipoAlerta = monitoramentoEvento.TipoAlerta;

                if (codigo.HasValue)
                    repositorioAlertaTratativaAcao.Atualizar(monitoramentoEventoAcaoTratatica);
                else
                    repositorioAlertaTratativaAcao.Inserir(monitoramentoEventoAcaoTratatica);
            }
        }
        private void AdicionarOuAtualizarTipoDeOperacao(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic tiposDeOperacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("TipoDeOperacao"));
            if (monitoramentoEvento.VerificarTipoDeOperacao == VerificarTipoDeOperacao.NaoVerificar) tiposDeOperacao = new List<dynamic>();
            ExcluirTipoDeOperacaoRemovidos(monitoramentoEvento, tiposDeOperacao, unitOfWork);
            if (monitoramentoEvento.VerificarTipoDeOperacao != VerificarTipoDeOperacao.NaoVerificar)
                InserirTipoDeOperacaoAdicionados(monitoramentoEvento, tiposDeOperacao, unitOfWork);
        }



        private void ExcluirTratativasRemovidas(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tratativas, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramentoEvento.Tratativas?.Count > 0)
            {
                Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa repositorioTratativa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa(unitOfWork);
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (dynamic tratativa in tratativas)
                {
                    int? codigo = ((string)tratativa.Codigo).ToNullableInt();

                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa> listaTratativasRemover = (from tratativa in monitoramentoEvento.Tratativas where !listaCodigosAtualizados.Contains(tratativa.Codigo) select tratativa).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa tratativa in listaTratativasRemover)
                {
                    repositorioTratativa.Deletar(tratativa);
                }

                if (listaTratativasRemover.Count > 0)
                {
                    string descricaoAcao = listaTratativasRemover.Count == 1 ? "Tratativa removida" : "Múltiplas tratativas removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirTratativasAdicionadas(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tratativas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa repositorioTratativa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTratativa(unitOfWork);
            Repositorio.Embarcador.Logistica.CategoriaResponsavel repositorioCategoriaResponsavel = new Repositorio.Embarcador.Logistica.CategoriaResponsavel(unitOfWork);
            int totalTratativasAdicionadasOuAtualizadas = 0;

            foreach (dynamic tratativa in tratativas)
            {
                int? codigo = ((string)tratativa.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa monitoramentoEventoTratativa;

                if (codigo.HasValue)
                    monitoramentoEventoTratativa = repositorioTratativa.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException("Tratativa não encontrada");
                else
                    monitoramentoEventoTratativa = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTratativa();

                monitoramentoEventoTratativa.CategoriaResponsavel = repositorioCategoriaResponsavel.BuscarPorCodigo(((string)tratativa.CodigoCategoriaResponsavel).ToInt()) ?? throw new ControllerException("Categoria de responsável não encontrada");
                monitoramentoEventoTratativa.EnvioEmail = ((string)tratativa.EnviarEmail).ToBool();
                monitoramentoEventoTratativa.EnvioEmailTransportador = ((string)tratativa.EnviarEmailTransportador).ToBool();
                monitoramentoEventoTratativa.EnvioEmailCliente = ((string)tratativa.EnviarEmailCliente).ToBool();
                monitoramentoEventoTratativa.MonitoramentoEvento = monitoramentoEvento;
                monitoramentoEventoTratativa.Sequencia = ((string)tratativa.Sequencia).ToInt();
                monitoramentoEventoTratativa.TempoEmMinutos = ((string)tratativa.Tempo).ToInt();

                if (codigo.HasValue)
                    repositorioTratativa.Atualizar(monitoramentoEventoTratativa);
                else
                    repositorioTratativa.Inserir(monitoramentoEventoTratativa);

                totalTratativasAdicionadasOuAtualizadas++;
            }

            if (monitoramentoEvento.IsInitialized() && (totalTratativasAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = totalTratativasAdicionadasOuAtualizadas == 1 ? "Tratativa adicionada ou atualizada" : "Múltiplas tratativas adicionadas ou atualizadas";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void ExcluirStatusViagemRemovidos(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic statusViagem, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramentoEvento.StatusViagem?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();
                foreach (dynamic status in statusViagem)
                {
                    int? codigo = ((string)status.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                Repositorio.Embarcador.Logistica.MonitoramentoEventoStatusViagem repMonitoramentoEventoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoEventoStatusViagem(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem> listaStatusViagemRemover = (from status in monitoramentoEvento.StatusViagem where !listaCodigosAtualizados.Contains(status.Codigo) select status).ToList();
                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem status in listaStatusViagemRemover)
                {
                    repMonitoramentoEventoStatusViagem.Deletar(status);
                }

                if (listaStatusViagemRemover.Count > 0)
                {
                    string descricaoAcao = (listaStatusViagemRemover.Count == 1) ? "Status de viagem removido" : "Múltiplos status de viagem removidos";
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirStatusViagemAdicionados(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic statusViagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEventoStatusViagem repMonitoramentoEventoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoEventoStatusViagem(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem repMonitoramentoStatusViagem = new Repositorio.Embarcador.Logistica.MonitoramentoStatusViagem(unitOfWork);
            int totalStatusViagemAdicionadasOuAtualizadas = 0;

            foreach (dynamic status in statusViagem)
            {
                int? codigo = ((string)status.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem monitoramentoEventoStatusViagem;
                if (codigo.HasValue)
                    monitoramentoEventoStatusViagem = repMonitoramentoEventoStatusViagem.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException("Status de viagem do evento não encontrado");
                else
                    monitoramentoEventoStatusViagem = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoStatusViagem();

                monitoramentoEventoStatusViagem.MonitoramentoStatusViagem = repMonitoramentoStatusViagem.BuscarPorCodigo(((string)status.CodigoMonitoramentoStatusViagem).ToInt(), false) ?? throw new ControllerException("Status de viagem não encontrado");
                monitoramentoEventoStatusViagem.MonitoramentoEvento = monitoramentoEvento;

                if (codigo.HasValue)
                    repMonitoramentoEventoStatusViagem.Atualizar(monitoramentoEventoStatusViagem);
                else
                    repMonitoramentoEventoStatusViagem.Inserir(monitoramentoEventoStatusViagem);

                totalStatusViagemAdicionadasOuAtualizadas++;
            }

            if (monitoramentoEvento.IsInitialized() && (totalStatusViagemAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = (totalStatusViagemAdicionadasOuAtualizadas == 1) ? "Status de viagem adicionado ou atualizado" : "Múltiplos status de viagem adicionados ou atualizados";
                Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void ExcluirTipoDeCargaRemovidos(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tiposDeCarga, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramentoEvento.StatusViagem?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();
                foreach (dynamic tipoDeCarga in tiposDeCarga)
                {
                    int? codigo = ((string)tipoDeCarga.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga repMonitoramentoEventoTipoDeCarga = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga> listaTipoDeCargaRemover = (from obj in monitoramentoEvento.TipoDeCarga where !listaCodigosAtualizados.Contains(obj.Codigo) select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga tipoDeCarga in listaTipoDeCargaRemover)
                {
                    repMonitoramentoEventoTipoDeCarga.Deletar(tipoDeCarga);
                }

                if (listaTipoDeCargaRemover.Count > 0)
                {
                    string descricaoAcao = (listaTipoDeCargaRemover.Count == 1) ? "Tipo de carga removido" : "Múltiplos tipos de cargas removidos";
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirTipoDeCargaAdicionados(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tiposDeCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga repMonitoramentoEventoTipoDeCarga = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            int totalTipoDeCargaAdicionadasOuAtualizadas = 0;

            foreach (dynamic tipoDeCarga in tiposDeCarga)
            {
                int? codigo = ((string)tipoDeCarga.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga monitoramentoEventoTipoDeCarga;
                if (codigo.HasValue)
                    monitoramentoEventoTipoDeCarga = repMonitoramentoEventoTipoDeCarga.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException("Tipo de carga do evento não encontrado");
                else
                    monitoramentoEventoTipoDeCarga = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeCarga();

                monitoramentoEventoTipoDeCarga.TipoDeCarga = repTipoDeCarga.BuscarPorCodigo(((string)tipoDeCarga.CodigoTipoDeCarga).ToInt(), false) ?? throw new ControllerException("Tipo de carga não encontrado");
                monitoramentoEventoTipoDeCarga.MonitoramentoEvento = monitoramentoEvento;

                if (codigo.HasValue)
                    repMonitoramentoEventoTipoDeCarga.Atualizar(monitoramentoEventoTipoDeCarga);
                else
                    repMonitoramentoEventoTipoDeCarga.Inserir(monitoramentoEventoTipoDeCarga);

                totalTipoDeCargaAdicionadasOuAtualizadas++;
            }

            if (monitoramentoEvento.IsInitialized() && (totalTipoDeCargaAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = (totalTipoDeCargaAdicionadasOuAtualizadas == 1) ? "Tipo de carga adicionado ou atualizado" : "Múltiplos tipos de carga adicionados ou atualizados";
                Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }


        private void ExcluirTipoDeOperacaoRemovidos(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tiposDeOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            if (monitoramentoEvento.StatusViagem?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();
                foreach (dynamic tipoDeOperacao in tiposDeOperacao)
                {
                    int? codigo = ((string)tipoDeOperacao.Codigo).ToNullableInt();
                    if (codigo.HasValue)
                        listaCodigosAtualizados.Add(codigo.Value);
                }

                Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao repMonitoramentoEventoTipoDeOperacao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao> listaTipoDeOperacaoRemover = (from obj in monitoramentoEvento.TipoDeOperacao where !listaCodigosAtualizados.Contains(obj.Codigo) select obj).ToList();
                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao tipoDeOperacao in listaTipoDeOperacaoRemover)
                {
                    repMonitoramentoEventoTipoDeOperacao.Deletar(tipoDeOperacao);
                }

                if (listaTipoDeOperacaoRemover.Count > 0)
                {
                    string descricaoAcao = (listaTipoDeOperacaoRemover.Count == 1) ? "Tipo de Operacao removido" : "Múltiplos tipos de Operacaos removidos";
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirTipoDeOperacaoAdicionados(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, dynamic tiposDeOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao repMonitoramentoEventoTipoDeOperacao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoDeOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            int totalTipoDeOperacaoAdicionadasOuAtualizadas = 0;

            foreach (dynamic tipoDeOperacao in tiposDeOperacao)
            {
                int? codigo = ((string)tipoDeOperacao.Codigo).ToNullableInt();
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao monitoramentoEventoTipoDeOperacao;
                if (codigo.HasValue)
                    monitoramentoEventoTipoDeOperacao = repMonitoramentoEventoTipoDeOperacao.BuscarPorCodigo(codigo.Value) ?? throw new ControllerException("Tipo de Operacao do evento não encontrado");
                else
                    monitoramentoEventoTipoDeOperacao = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoDeOperacao();

                monitoramentoEventoTipoDeOperacao.TipoDeOperacao = repTipoDeOperacao.BuscarPorCodigo(((string)tipoDeOperacao.CodigoTipoDeOperacao).ToInt(), false) ?? throw new ControllerException("Tipo de Operacao não encontrado");
                monitoramentoEventoTipoDeOperacao.MonitoramentoEvento = monitoramentoEvento;

                if (codigo.HasValue)
                    repMonitoramentoEventoTipoDeOperacao.Atualizar(monitoramentoEventoTipoDeOperacao);
                else
                    repMonitoramentoEventoTipoDeOperacao.Inserir(monitoramentoEventoTipoDeOperacao);

                totalTipoDeOperacaoAdicionadasOuAtualizadas++;
            }

            if (monitoramentoEvento.IsInitialized() && (totalTipoDeOperacaoAdicionadasOuAtualizadas > 0))
            {
                string descricaoAcao = (totalTipoDeOperacaoAdicionadasOuAtualizadas == 1) ? "Tipo de Operacao adicionado ou atualizado" : "Múltiplos tipos de Operacao adicionados ou atualizados";
                Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }



        private void PreencherMonitoramentoEvento(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);

            monitoramentoEvento.Ativo = Request.GetBoolParam("Status");
            monitoramentoEvento.Descricao = Request.GetNullableStringParam("Descricao") ?? throw new ControllerException("Descrição do evento é obrigatória.");
            monitoramentoEvento.TipoMonitoramentoEvento = Request.GetNullableEnumParam<TipoMonitoramentoEvento>("TipoMonitoramentoEvento") ?? throw new ControllerException("Tipo do evento é obrigatório.");
            monitoramentoEvento.TipoAlerta = monitoramentoEvento.TipoMonitoramentoEvento.ObterTipoAlerta();
            monitoramentoEvento.Prioridade = Request.GetIntParam("Prioridade");
            monitoramentoEvento.Cor = Request.GetStringParam("Cor");
            monitoramentoEvento.ExibirApp = Request.GetBoolParam("ExibirApp");
            monitoramentoEvento.NaoGerarParaPreCarga = Request.GetBoolParam("NaoGerarParaPreCarga");
            monitoramentoEvento.ConsiderarParaSemaforo = Request.GetBoolParam("ConsiderarParaSemaforo");
            monitoramentoEvento.GerarAtendimento = Request.GetBoolParam("GerarAtendimento");
            int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");
            if (codigoMotivoChamado > 0)
                monitoramentoEvento.MotivoChamado = repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado, false);
            else
                monitoramentoEvento.MotivoChamado = null;

            monitoramentoEvento.IntegrarEvento = Request.GetBoolParam("IntegrarEvento");
            monitoramentoEvento.GerarAlertaAcompanhamentoCarga = Request.GetBoolParam("GerarAlertaAcompanhamentoCarga");
            monitoramentoEvento.ExibirDescricaoAlerta = monitoramentoEvento.GerarAlertaAcompanhamentoCarga && Request.GetBoolParam("ExibirDescricaoAlerta");
            monitoramentoEvento.ExibirDataeHoraGeracaoAlerta = monitoramentoEvento.GerarAlertaAcompanhamentoCarga && Request.GetBoolParam("ExibirDataeHoraGeracaoAlerta");
            monitoramentoEvento.ExibirControleEntrega = Request.GetBoolParam("ExibirControleEntrega");
            monitoramentoEvento.QuandoProcessar = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.QuandoProcessarMonitoramento>("QuandoProcessar");
            monitoramentoEvento.VerificarStatusViagem = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarStatusViagem>("VerificarStatusViagem");
            monitoramentoEvento.VerificarTipoDeCarga = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeCarga>("VerificarTipoDeCarga");
            monitoramentoEvento.VerificarTipoDeOperacao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VerificarTipoDeOperacao>("VerificarTipoDeOperacao");
        }

        private void PreencherMonitoramentoEventoGatilho(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoGatilho monitoramentoEventoGatilho, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.Locais repLocais = new Repositorio.Embarcador.Logistica.Locais(unitOfWork);
            Repositorio.Embarcador.Logistica.RaioProximidade repRaioProximidade = new Repositorio.Embarcador.Logistica.RaioProximidade(unitOfWork);
            dynamic dadosGatilho = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Gatilho"));

            monitoramentoEventoGatilho.MonitoramentoEvento = monitoramentoEvento;
            monitoramentoEventoGatilho.Raio = ((string)dadosGatilho.Raio).ToInt();
            monitoramentoEventoGatilho.Tempo = ((string)dadosGatilho.Tempo).ToInt();
            monitoramentoEventoGatilho.Velocidade = ((string)dadosGatilho.Velocidade).ToInt();
            monitoramentoEventoGatilho.Velocidade2 = ((string)dadosGatilho.Velocidade2).ToInt();
            monitoramentoEventoGatilho.TempoEvento = ((string)dadosGatilho.TempoEvento).ToInt();
            monitoramentoEventoGatilho.TempoEvento2 = ((string)dadosGatilho.TempoEvento2).ToInt();
            monitoramentoEventoGatilho.Quantidade = ((string)dadosGatilho.Quantidade).ToInt();

            monitoramentoEventoGatilho.PontosDeApoio = new List<Dominio.Entidades.Embarcador.Logistica.Locais>();
            List<int> pontosDeApoio = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>((string)dadosGatilho.PontosDeApoio);
            foreach (int codigoPontoApoio in pontosDeApoio)
                monitoramentoEventoGatilho.PontosDeApoio.Add(repLocais.BuscarPorCodigo(codigoPontoApoio));

            monitoramentoEventoGatilho.RaioProximidade = repRaioProximidade.BuscarPorCodigo(((string)dadosGatilho.RaioProximidade).ToInt());

            bool.TryParse((string)dadosGatilho.ConsiderarApenasDataNaReferencia, out bool considerarApenasDataNaReferencia);
            monitoramentoEventoGatilho.ConsiderarApenasDataNaReferencia = considerarApenasDataNaReferencia;

            bool.TryParse((string)dadosGatilho.TratativaAutomatica, out bool tratativaAutomatica);
            monitoramentoEventoGatilho.TratativaAutomatica = tratativaAutomatica;

            bool.TryParse((string)dadosGatilho.EventoContinuo, out bool eventoContinuo);
            monitoramentoEventoGatilho.EventoContinuo = eventoContinuo;

            bool.TryParse((string)dadosGatilho.TempoReferenteaDataCarregamentoCarga, out bool tempoReferenteaDataCarregamentoCarga);
            monitoramentoEventoGatilho.TempoReferenteaDataCarregamentoCarga = tempoReferenteaDataCarregamentoCarga;

            bool.TryParse((string)dadosGatilho.ValidarApenasCargasNaoIniciadas, out bool ValidarApenasCargasNaoIniciadas);
            monitoramentoEventoGatilho.ValidarApenasCargasNaoIniciadas = ValidarApenasCargasNaoIniciadas;

            monitoramentoEventoGatilho.DataBase = ((string)dadosGatilho.DataBase).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData>();
            monitoramentoEventoGatilho.DataReferencia = ((string)dadosGatilho.DataReferencia).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.MonitoramentoEventoData>();
            if (!monitoramentoEvento.Ativo)
                monitoramentoEventoGatilho.Posicao = 0;
        }

        private void PreencherMonitoramentoEventoHorario(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoHorario monitoramentoEventoHorario)
        {
            dynamic dadosGatilho = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Horario"));
            monitoramentoEventoHorario.MonitoramentoEvento = monitoramentoEvento;
            monitoramentoEventoHorario.HoraInicio = TimeSpan.ParseExact((string)dadosGatilho.HoraInicio, "g", null, System.Globalization.TimeSpanStyles.None);
            monitoramentoEventoHorario.HoraFim = TimeSpan.ParseExact((string)dadosGatilho.HoraFim, "g", null, System.Globalization.TimeSpanStyles.None);
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento()
            {
                Descricao = Request.GetStringParam("Descricao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo),
                Tipo = Request.GetNullableEnumParam<TipoMonitoramentoEvento>("TipoMonitoramentoEvento")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string descricao = Request.GetStringParam("Descricao");
                SituacaoAtivoPesquisa situacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo", "Tipo", 20, Models.Grid.Align.left, false);

                if (situacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoAtivo", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoEvento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Logistica.MonitoramentoEvento repositorio = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> listaMonitoramentoEvento = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento>();

                var listaMonitoramentoEventoRetornar = (
                    from monitoramentoEvento in listaMonitoramentoEvento
                    select new
                    {
                        monitoramentoEvento.Codigo,
                        monitoramentoEvento.Descricao,
                        monitoramentoEvento.DescricaoAtivo,
                        Tipo = monitoramentoEvento.TipoMonitoramentoEvento.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaMonitoramentoEventoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "DescricaoAtivo")
                return "Ativo";

            return propriedadeOrdenar;
        }

        private void SalvarTiposIntegracao(Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao repMonitoramentoEventoTipoIntegracao = new Repositorio.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            List<TipoIntegracao> tiposIntegracao = Request.GetListEnumParam<TipoIntegracao>("TipoIntegracao");

            List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao> monitoramentoEventoTiposIntegracao = repMonitoramentoEventoTipoIntegracao.BuscarPorMonitoramentoEvento(monitoramentoEvento.Codigo);

            if (monitoramentoEventoTiposIntegracao.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao> deletar = (from obj in monitoramentoEventoTiposIntegracao where !tiposIntegracao.Contains(obj.TipoIntegracao.Tipo) select obj).ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao MonitoramentoEventoTipoIntegracao in deletar)
                {
                    TipoIntegracao tipoIntegracaoDeletar = MonitoramentoEventoTipoIntegracao.TipoIntegracao.Tipo;
                    repMonitoramentoEventoTipoIntegracao.Deletar(MonitoramentoEventoTipoIntegracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.ExcluiuIntegracao, tipoIntegracaoDeletar.ObterDescricao()), unitOfWork);
                }
            }

            foreach (TipoIntegracao enumTipoIntegracao in tiposIntegracao)
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(enumTipoIntegracao);
                Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao MonitoramentoEventoTipoIntegracao = repMonitoramentoEventoTipoIntegracao.BuscarPorMonitoramentoEventoETipoIntegracao(monitoramentoEvento.Codigo, tipoIntegracao.Codigo);

                if (MonitoramentoEventoTipoIntegracao == null)
                {
                    MonitoramentoEventoTipoIntegracao = new Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoTipoIntegracao();
                    MonitoramentoEventoTipoIntegracao.MonitoramentoEvento = monitoramentoEvento;
                    MonitoramentoEventoTipoIntegracao.TipoIntegracao = tipoIntegracao;
                    repMonitoramentoEventoTipoIntegracao.Inserir(MonitoramentoEventoTipoIntegracao);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, monitoramentoEvento, string.Format(Localization.Resources.Ocorrencias.TipoOcorrencia.AdicionouIntegracao, enumTipoIntegracao.ObterDescricao()), unitOfWork);
                }
            }
        }

        #endregion
    }
}
