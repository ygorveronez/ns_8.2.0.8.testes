using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "BuscarPorCarga" }, "Cargas/ControleEntrega", "Logistica/Monitoramento")]
    public class ControleEntregaInicioViagemController : BaseController
    {
		#region Construtores

		public ControleEntregaInicioViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Carga"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                string dataPreViageminicio = string.Empty;
                string dataPreViagemFim = string.Empty;

                if (!carga.DataPreViagemInicio.HasValue)
                    dataPreViageminicio = carga.DataPreViagemFim.HasValue ? "Início de Pré Trip não realizado" : "";
                else
                    dataPreViageminicio = Localization.Resources.Cargas.ControleEntrega.PreTripIniciadaEm + " " + carga.DataPreViagemInicio.Value.ToString("dd/MM/yyyy HH:mm");

                if (carga.DataPreViagemFim.HasValue)
                    dataPreViagemFim = Localization.Resources.Cargas.ControleEntrega.PreTripFinalizadaEm + " " + carga.DataPreViagemFim.Value.ToString("dd/MM/yyyy HH:mm");

                // Formata retorno
                var retorno = new
                {
                    Carga = carga.Codigo,
                    ViagemAberta = !carga?.DataInicioViagem.HasValue ?? false,
                    DataInicioViagem = carga?.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataInicioViagemSugerida = (!carga.DataInicioViagem.HasValue) ? carga.DadosSumarizados.DataPrevisaoInicioViagem.HasValue ? carga.DadosSumarizados.DataPrevisaoInicioViagem.Value.ToString("dd/MM/yyyy HH:mm") : DateTime.Now.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    PermiteAlterarDataInicioCarregamento = carga?.TipoOperacao?.PermiteAlterarDataInicioCarregamentoNoControleEntrega ?? false,
                    ViagemFinalizada = carga?.DataFimViagem.HasValue ?? false,
                    LatitudeInicioViagem = carga?.LatitudeInicioViagem,
                    LongitudeInicioViagem = carga?.LongitudeInicioViagem,
                    DataFimViagem = carga?.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    PermiteTransportadorConfirmarRejeitarEntrega = carga?.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false,
                    DataPrevisaoInicioViagem = carga.DadosSumarizados.DataPrevisaoInicioViagem?.ToString("dd/MM/yyyy HH:mm"),
                    DataPreViagemInicio = dataPreViageminicio,
                    DataPreViagemFim = dataPreViagemFim
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInicioViagem()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repConfiguracaoTipoOperacaoControle = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int codigoCargaComMonitoramentoAtivo = Request.GetIntParam("CodigoCargaComMonitoramentoAtivo");
                DateTime dataInicioViagem = Request.GetDateTimeParam("DataInicioViagemInformada");
                List<SituacaoCarga> situacoesCargaNaoEmitida = SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repConfiguracaoTipoOperacaoControle.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

                bool permiteDataAnteriorCarregamento = carga.TipoOperacao != null ? carga.TipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento : false;

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente) && !(carga.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (carga.DataInicioViagem.HasValue)
                    return new JsonpResult(false, true, "O início da viagem já foi registrado.");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe &&
                    ((configuracaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false) || (configuracaoTipoOperacaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false)) &&
                    situacoesCargaNaoEmitida.Contains(carga.SituacaoCarga))
                    return new JsonpResult(false, true, "A situação da Carga não permite informar início da viagem.");
                if (dataInicioViagem == null || dataInicioViagem == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data de início da viagem.");
                if (dataInicioViagem > DateTime.Now)
                    return new JsonpResult(false, true, "A data informada de início de viagem deve ser menor que a data atual.");
                if (dataInicioViagem < carga.DataCarregamentoCarga && !permiteDataAnteriorCarregamento)
                    return new JsonpResult(false, true, $"A data informada de início de viagem deve ser maior que a data de carregamento da carga, {carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm")}");

                unitOfWork.Start();

                try
                {
                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(codigoCarga, dataInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, null, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unitOfWork))
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Início de viagem informado manualmente", unitOfWork);
                        if (codigoCargaComMonitoramentoAtivo > 0)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga cargaComMonitoramentoAtivo = repCarga.BuscarPorCodigo(codigoCargaComMonitoramentoAtivo);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaComMonitoramentoAtivo, $"Monitoramento encerrado na carga {cargaComMonitoramentoAtivo.CodigoCargaEmbarcador}, visto que foi iniciada viagem na carga {carga.CodigoCargaEmbarcador}.", unitOfWork);
                        }
                    }

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao informar inicio de viagem.");
                }

                // Retorna informacoes

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar ação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInicioPreTrip()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                DateTime dataInicioPreTrip = Request.GetDateTimeParam("DataInicioPreTrip");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramento = repositorioMonitoramento.BuscarPorCodigoCarga(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimPreTrip))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                if (carga.DataPreViagemInicio.HasValue)
                    return new JsonpResult(false, true, "O início da viagem já foi registrado.");
                if (dataInicioPreTrip == null || dataInicioPreTrip == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data de início da pré-trip.");
                if (carga.DataInicioViagem.HasValue)
                    return new JsonpResult(false, true, "Não é possível iniciar a pré-trip pois a viagem da carga já foi iniciada.");
                if (monitoramento?.Status == MonitoramentoStatus.Finalizado)
                    return new JsonpResult(false, true, "Não é possível iniciar a pré-trip pois o monitoramento da carga já foi finalizado.");

                unitOfWork.Start();

                if (!carga.DataPreViagemInicio.HasValue && configuracaoTMS.QuandoIniciarMonitoramento == QuandoIniciarMonitoramento.EstouIndoAoIniciarViagem)
                {
                    carga.DataPreViagemInicio = dataInicioPreTrip;
                    Servicos.Embarcador.Monitoramento.Monitoramento.GerarMonitoramentoEIniciar(carga, configuracaoTMS, Auditado, "Motorista sinalizou que está indo (PreTrip)", unitOfWork);
                    repCarga.Atualizar(carga);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Início de pré trip informado manualmente", unitOfWork);
                }

                unitOfWork.CommitChanges();

                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(carga);

                return new JsonpResult(true, true, "Início de pré trip informado manualmente.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar ação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInicioViagemAlterar()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                DateTime dataInicioViagemAlterar = Request.GetDateTimeParam("DataInicioViagemInformadaAlterar");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                bool permiteDataAnteriorCarregamento = carga.TipoOperacao != null ? carga.TipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento : false;
                bool possuiPermissaoAlterarDataInicioViagemLivre = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem);

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (carga.DataFimViagem.HasValue && !possuiPermissaoAlterarDataInicioViagemLivre)
                    return new JsonpResult(false, true, "A viagem já foi finalizada.");
                if (dataInicioViagemAlterar == null || dataInicioViagemAlterar == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data de início da viagem.");
                if (dataInicioViagemAlterar > DateTime.Now)
                    return new JsonpResult(false, true, "A data informada deve ser menor que a data atual.");
                if (dataInicioViagemAlterar < carga.DataCarregamentoCarga && !permiteDataAnteriorCarregamento)
                    return new JsonpResult(false, true, $"A data informada de início de viagem deve ser maior que a data de carregamento da carga, {carga.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm")}");

                unitOfWork.Start();
                try
                {
                    carga.Initialize();

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarInicioViagem(carga.Codigo, dataInicioViagemAlterar, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Início da viagem alterado manualmente", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repCarga.Atualizar(carga, Auditado);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao alterar o início de viagem.");
                }

                // Retorna informacoes

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar ação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExisteMonitoramentoAtivoParaVeiculoPlaca()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios.
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.Monitoramento repMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);

                // Parametros.
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                // Busca informacões.
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                if (carga.Veiculo == null)
                    return new JsonpResult(true);

                List<Dominio.Entidades.Embarcador.Logistica.Monitoramento> listMonitoramento = repMonitoramento.BuscarMonitoramentoEmAbertoPorVeiculoPlaca(carga.Veiculo.Placa);

                // Valida se existe monitoramento para VeiculoPlaca.
                if (listMonitoramento.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Logistica.Monitoramento monitoramentoCarga = listMonitoramento.Find(p => p.Carga.CodigoCargaEmbarcador != carga.CodigoCargaEmbarcador);
                    if (monitoramentoCarga != null)
                    {
                        var retorno = new { CodigoCargaComMonitoramentoAtivo = monitoramentoCarga.Carga.Codigo };
                        return new JsonpResult(retorno, true, Localization.Resources.Cargas.ControleEntrega.ExisteUmMonitoramentoEmAbertoParaVeiculoPlaca);
                    }
                    else
                        return new JsonpResult(true);
                }
                else
                    return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar ação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
    #endregion
}


