using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AlertaTratativa", "Logistica/Monitoramento", "Cargas/ControleEntrega", "TorreControle/AcompanhamentoCarga", "Cargas/AlertasTransportador", "Logistica/MonitoramentoNovo")]
    public class AlertaMonitoramentoController : BaseController
    {
		#region Construtores

		public AlertaMonitoramentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Metodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor Alerta = new Dominio.Entidades.Embarcador.Logistica.AlertaMonitor();

                try
                {
                    PreencherDados(Alerta, unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                Repositorio.Embarcador.Logistica.AlertaMonitor repAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                repAlerta.Inserir(Alerta);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAlerta = Request.GetIntParam("Codigo");
                bool buscarTodosDaCarga = Request.GetBoolParam("BuscarTodosDaCarga");
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorioAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa repositorioMonitoramentoEventoCausa = new Repositorio.Embarcador.Logistica.MonitoramentoEventoCausa(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativaAcao repositorioTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativaAcao(unitOfWork);
                Repositorio.Embarcador.Logistica.AlertaTratativa repositorioAlertaTratativa = new Repositorio.Embarcador.Logistica.AlertaTratativa(unitOfWork);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = repositorioAlerta.BuscarPorCodigo(codigoAlerta);

                if (alerta == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o alerta.");

                List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEventoCausa> causasAlertas = repositorioMonitoramentoEventoCausa.BuscarPorTipoAlerta(alerta.TipoAlerta).Where(c => c.Ativo).ToList();
                List<Dominio.Entidades.Embarcador.Logistica.AlertaTratativaAcao> tratativas = repositorioTratativa.BuscarPorTipoDeAlerta(alerta.TipoAlerta);
                Dominio.Entidades.Embarcador.Logistica.AlertaTratativa alertaTratativa = repositorioAlertaTratativa.BuscarPorAlerta(alerta.Codigo);

                var retorno = new
                {
                    alerta.Codigo,
                    CodigoAlerta = alerta.Codigo,
                    TipoAlerta = alerta.TipoAlerta.ObterDescricao() ?? "-",
                    Tipo = alerta.TipoAlerta,
                    NumeroAlerta = alerta.Codigo,
                    DescricaoStatus = alerta.Status.ObterDescricao() ?? "-",
                    Status = alerta.Status,
                    ImagemStatus = alerta.Status.ObterImagemStatus(),
                    DataInicio = alerta.Data.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                    DataFim = alerta.DataFim?.ToString("dd/MM/yyyy HH:mm:ss") ?? "-",
                    Responsavel = alerta.Responsavel?.Nome ?? "-",
                    UsuarioResponsavelLogado = (alerta.Responsavel != null && alerta.Responsavel.Codigo == this.Usuario.Codigo),
                    PossuiResponsavel = alerta.Responsavel != null,
                    Latitude = alerta.Latitude?.ToString() ?? string.Empty,
                    Longitude = alerta.Longitude?.ToString() ?? string.Empty,
                    Causas = (from causa in causasAlertas
                              select new
                              {
                                  text = causa.Descricao,
                                  value = causa.Codigo
                              }).ToList(),
                    Tratativas = (from tratativa in tratativas
                                  select new
                                  {
                                      text = tratativa.Descricao,
                                      value = tratativa.Codigo
                                  }).ToList(),
                    Tratativa = alertaTratativa?.AlertaTratativaAcao?.Descricao ?? "-",
                    CodigoTratativa = alertaTratativa?.AlertaTratativaAcao?.Codigo ?? 0,
                    Causa = alertaTratativa?.CausaAlertaTratativa?.Descricao ?? "-",
                    CodigoCausa = alertaTratativa?.CausaAlertaTratativa?.Codigo ?? 0,
                    Observacao = alertaTratativa?.Observacao ?? alerta.Observacao ?? string.Empty,
                    ResolvidoPor = alertaTratativa?.Usuario?.Nome ?? alerta.Responsavel?.Nome ?? string.Empty,
                    ReprogramarAlerta = alerta.AlertaReprogramado,
                    TempoReprogramacaoAlerta = alerta.TempoReprogramado,
                    Gatilho = alerta.AlertaDescricao ?? string.Empty,
                    ListaAlertasCarga = buscarTodosDaCarga ? servicoControleEntrega.ObterAlertasDasCargas(new List<Dominio.Entidades.Embarcador.Cargas.Carga> { new() { Codigo = alerta.Carga?.Codigo ?? 0 } }, unitOfWork, new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta> { alerta.TipoAlerta }) : null
                };

                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> AssumirResponsabilidadeAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoAlerta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorioAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = repositorioAlerta.BuscarPorCodigo(codigoAlerta);

                if (alerta == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o alerta.");

                unitOfWork.Start();

                alerta.Responsavel = this.Usuario;
                alerta.Status = AlertaMonitorStatus.EmTratativa;

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    CodigoAlerta = alerta.Codigo,
                    alerta.Responsavel.Codigo,
                    Responsavel = alerta.Responsavel.Nome,
                    DescricaoStatus = alerta.Status.ObterDescricao() ?? "-",
                    EnumStatus = alerta.Status,
                    ImagemStatus = alerta.Status.ObterImagemStatus(),
                };

                return new JsonpResult(retorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> DeixarAlerta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigoAlerta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlertaMonitor repositorioAlerta = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta = repositorioAlerta.BuscarPorCodigo(codigoAlerta);

                if (alerta == null)
                    return new JsonpResult(false, false, "Não foi possível encontrar o alerta.");

                unitOfWork.Start();

                alerta.Responsavel = null;
                alerta.Status = AlertaMonitorStatus.EmAberto;

                unitOfWork.CommitChanges();

                var retorno = new
                {
                    CodigoAlerta = alerta.Codigo,
                    Responsavel = alerta.Responsavel?.Nome ?? "-",
                    DescricaoStatus = alerta.Status.ObterDescricao() ?? "-",
                    EnumStatus = alerta.Status,
                    ImagemStatus = alerta.Status.ObterImagemStatus(),
                };

                return new JsonpResult(retorno);
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

        #endregion

        #region Metodos Privados


        private void PreencherDados(Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            int codveiculo = Request.GetIntParam("Veiculo");
            int carga = Request.GetIntParam("Carga");
            string descricao = Request.GetStringParam("Descricao");
            DateTime data = Request.GetDateTimeParam("DataAlerta");
            int codcargaEntrega = Request.GetIntParam("CargaEntrega");
            int codEvento = Request.GetIntParam("Evento");

            Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento evento = repMonitoramentoEvento.BuscarPorCodigo(codEvento, false);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega CargaEntrega = repCargaEntrega.BuscarPorCodigo(codcargaEntrega);

            if (evento.ExibirControleEntrega && CargaEntrega == null)
            {
                throw new Exception("Entrega/Coleta é obrigatória para este evento");
            }

            alerta.MonitoramentoEvento = evento;
            alerta.Carga = repCarga.BuscarPorCodigo(carga);
            alerta.TipoAlerta = evento.TipoAlerta;
            alerta.AlertaDescricao = descricao;
            alerta.Veiculo = (codveiculo > 0) ? new Dominio.Entidades.Veiculo { Codigo = codveiculo } : null;
            alerta.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.EmAberto;
            alerta.DataCadastro = DateTime.Now;
            alerta.AlertaManual = true;
            alerta.CargaEntrega = CargaEntrega;
            alerta.Data = data;

        }

        #endregion

    }
}
