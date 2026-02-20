using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Cargas.ControleEntrega
{
    [CustomAuthorize(new string[] { "BuscarPorCarga" }, "Cargas/ControleEntrega", "Logistica/Monitoramento")]
    public class ControleEntregaFimViagemController : BaseController
    {
        #region Construtores

        public ControleEntregaFimViagemController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
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

                dynamic retorno = new
                {
                    Carga = carga.Codigo,
                    ViagemAberta = !carga?.DataFimViagem.HasValue ?? true,
                    DataFimViagem = (carga.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty),
                    DataInicioViagem = (carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty),
                    ObservacaoEtapa = "",
                    DataFimViagemSugerida = (!carga.DataFimViagem.HasValue) ? DateTime.Now.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    PermiteTransportadorConfirmarRejeitarEntrega = carga?.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false,
                    HabilitarCobrancaEstadiaAutomaticaPeloTracking = carga?.TipoOperacao?.HabilitarCobrancaEstadiaAutomaticaPeloTracking ?? false,
                    ExigirJustificativaParaEncerramentoManualViagem = carga?.TipoOperacao?.ExigirJustificativaParaEncerramentoManualViagem ?? false,
                    MotivoEncerramentoManual = carga?.EncerramentoManualViagem?.Descricao ?? string.Empty,
                    ObservacaoEncerramentoManual = carga?.ObservacaoEncerramentoManualViagem ?? string.Empty
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarResumoRoteiro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisaResumoRoteiro();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdenaResumoRoteiro(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisaResumoRoteiro(ref totalRegistros, Request.GetIntParam("Carga"), propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> GerarOcorrenciaEstadia()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteGerarOcorrenciaEstadia))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro da carga.");
                if (!(carga.TipoOperacao?.HabilitarCobrancaEstadiaAutomaticaPeloTracking ?? false))
                    return new JsonpResult(false, true, "A configuração da carga não está habilitada para gerar a cobrança de estadia.");

                unitOfWork.Start();
                try
                {
                    carga.Initialize();

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarOcorrenciaEstadia(unitOfWork, carga, TipoServicoMultisoftware, Cliente, Auditado);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar as ocorrências de estadia.");
                }

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

        public async Task<IActionResult> InformarFimViagem()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega repConfiguracaoTipoOperacaoControle = new Repositorio.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega(unitOfWork);

                // Parametros
                int codigoCarga = Request.GetIntParam("Carga");

                DateTime dataFimViagem = Request.GetDateTimeParam("DataFimViagemInformada");
                DateTime dataInicioViagem = Request.GetDateTimeParam("DataInicioViagemInformada");
                List<SituacaoCarga> situacoesCargaNaoEmitida = SituacaoCargaHelper.ObterSituacoesCargaNaoEmitida();

                List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.DataTerminoEntrega> entregasDatasFimEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.DataTerminoEntrega>>(Request.GetStringParam("Entregas"));

                // Busca informacoes
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Pedidos.ConfiguracaoTipoOperacaoControleEntrega configuracaoTipoOperacaoControleEntrega = repConfiguracaoTipoOperacaoControle.BuscarPorTipoOperacao(carga.TipoOperacao?.Codigo ?? 0);

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente) && !(carga.TipoOperacao?.PermitirTransportadorConfirmarRejeitarEntrega ?? false))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repCargaEntrega.BuscarPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList = repCargaEntregaCheckList.BuscarPrimeiroPorCarga(codigoCarga, TipoCheckList.Desembarque);

                // Valida
                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (carga.DataFimViagem.HasValue)
                    return new JsonpResult(false, true, "O fim da viagem já foi registrado.");
                if (carga.DataFimViagem.HasValue)
                    return new JsonpResult(false, true, "Não é possível finalizar a viagem pois ainda não foi iniciada.");
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe &&
                    ((configuracaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false) || (configuracaoTipoOperacaoControleEntrega?.BloquearInicioeFimDeViagemPeloTransportadorEmCargaNaoEmitida ?? false)) &&
                    situacoesCargaNaoEmitida.Contains(carga.SituacaoCarga))
                    return new JsonpResult(false, true, "A situação da Carga não permite informar final da viagem.");
                if (dataInicioViagem == null || dataInicioViagem == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data inicial da viagem.");
                if (dataFimViagem == null || dataFimViagem == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data final da viagem.");
                if (dataFimViagem > DateTime.Now)
                    return new JsonpResult(false, true, "A data informada de fim de viagem deve ser menor que a data atual.");
                if (dataFimViagem < carga.DataInicioViagem || (dataFimViagem < dataInicioViagem)) 
                    return new JsonpResult(false, true, $"A data informada de fim de viagem deve ser maior que a data de início de viagem, {carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm")}");
                if ((carga.TipoOperacao?.ConfiguracaoControleEntrega?.EnviarBoletimViagemAoFinalizarViagem ?? false) && checkList == null)
                    return new JsonpResult(false, true, "A pesquisa de desembarque deve ser respondida antes da finalização da viagem.");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    if (repCargaEntrega.ContemControleNaoFinalizado(codigoCarga))
                        return new JsonpResult(false, true, "Existe coleta/entrega ainda não finalizada, favor verifique antes de finalizar a viagem.");
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    if (configuracaoGeral.NaoPermitirFinalizarViagemDetalhesFimViagem)
                        return new JsonpResult(false, true, "Não foi possivel finalizar a viagem.");

                unitOfWork.Start();

                string descricaoAuditoria = "Finalizou o controle de entregas da carga.";

                if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarViagem(codigoCarga, dataFimViagem, dataInicioViagem, Auditado, descricaoAuditoria, Usuario, TipoServicoMultisoftware, Cliente, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, unitOfWork, entregasDatasFimEntrega))
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Fim de viagem informado manualmente", unitOfWork);
                else
                    descricaoAuditoria += " Viagem não finalizada";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, descricaoAuditoria, unitOfWork);

                Servicos.Embarcador.Monitoramento.Monitoramento.FinalizarMonitoramento(carga, dataFimViagem, ConfiguracaoEmbarcador, Auditado, descricaoAuditoria, unitOfWork, MotivoFinalizacaoMonitoramento.FinalizouControleEntregaCarga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao informar fim da viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarFimPreTrip()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                DateTime dataFimPreTrip = Request.GetDateTimeParam("DataFimPreTrip");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimPreTrip))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");
                if (carga.DataPreViagemFim.HasValue)
                    return new JsonpResult(false, true, "O fim da pré trip já foi registrado.");
                if (dataFimPreTrip == null || dataFimPreTrip == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data de fim da pré trip.");

                unitOfWork.Start();

                carga.DataPreViagemFim = dataFimPreTrip;

                repCarga.Atualizar(carga);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, "Fim de pré trip informado manualmente", unitOfWork);
                unitOfWork.CommitChanges();

                servAlertaAcompanhamentoCarga.informarAtualizacaoCardCargaAcompanhamento(carga);

                return new JsonpResult(true, true, "Fim de pré trip informado com sucesso.");

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

        public async Task<IActionResult> InformarFimViagemAlterar()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Logistica/Monitoramento");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteIniciarFinalizarViagensEntregasManualmente))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repositorioConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);

                Servicos.Embarcador.Carga.AlertaCarga.FimViagem alertaFimViagem = new Servicos.Embarcador.Carga.AlertaCarga.FimViagem(unitOfWork, unitOfWork.StringConexao);

                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                DateTime dataFimViagemAlterar = Request.GetDateTimeParam("DataFimViagemInformada");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                bool permiteDataAnteriorCarregamento = carga.TipoOperacao != null ? carga.TipoOperacao.PermitirDataInicioViagemAnteriorDataCarregamento : false;
                bool possuiPermissaoAlterarDataInicioViagemLivre = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.CargaEntrega_PermiteAlterarDataInicioFimViagem);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega UltimacargaEntregaFinalizada = repCargaEntrega.BuscarUltimaCargaEntregaRealizada(carga.Codigo);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork).BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repositorioConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");
                if (carga.DataFimViagem.HasValue && !possuiPermissaoAlterarDataInicioViagemLivre)
                    return new JsonpResult(false, true, "A viagem já foi finalizada.");
                if (dataFimViagemAlterar == null || dataFimViagemAlterar == DateTime.MinValue)
                    return new JsonpResult(false, true, "Deve ser informada a data de fim da viagem.");
                if (dataFimViagemAlterar > DateTime.Now)
                    return new JsonpResult(false, true, "A data informada deve ser menor que a data atual.");
                if (UltimacargaEntregaFinalizada != null && UltimacargaEntregaFinalizada.DataFim.HasValue && dataFimViagemAlterar < UltimacargaEntregaFinalizada.DataFim.Value)
                    return new JsonpResult(false, true, $"A data informada deve ser maior ou igual a data de confirmação da ultima entrega {UltimacargaEntregaFinalizada.DataFim.Value.ToString("dd/MM/yyyy HH:mm")}");

                unitOfWork.Start();

                try
                {
                    carga.Initialize();

                    carga.DataFimViagem = dataFimViagemAlterar;

                    alertaFimViagem.ProcessarEvento(carga);

                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarEventosColetaEntregaCargaAtualizacaoInicioFimViagem(carga, EventoColetaEntrega.FimViagem, dataFimViagemAlterar, configuracaoEmbarcador, TipoServicoMultisoftware, Cliente, Auditado?.OrigemAuditado ?? OrigemAuditado.Sistema, configuracaoControleEntrega, unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, carga.GetChanges(), "Fim da viagem alterado manualmente", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Update);

                    repCarga.Atualizar(carga, Auditado);

                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao alterar o fim de viagem.");
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

        public async Task<IActionResult> ReabrirCarga()
        {
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/ControleEntrega", "Cargas/Carga");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteReabrirCargaFinalizada))
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(true, "Você não pode executar essa ação.");

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.Retornos.RetornoCarga repRetornoCarga = new Repositorio.Embarcador.Cargas.Retornos.RetornoCarga(unitOfWork);

                int codigoCarga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga, true);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Cargas.Retornos.RetornoCarga retornoCarga = repRetornoCarga.BuscarPorCarga(carga.Codigo);

                if (retornoCarga != null)
                    return new JsonpResult(false, true, "Não é possível reabrir a carga porque ela já gerou retorno.");

                carga.DataFimViagem = null;

                repositorioCarga.Atualizar(carga, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarJustificativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoEncerramento = Request.GetIntParam("CodigoEncerramento");

                Repositorio.Embarcador.Justificativas.EncerramentoManualViagem repEncerramentoManualViagem = new Repositorio.Embarcador.Justificativas.EncerramentoManualViagem(unitOfWork);
                Dominio.Entidades.Embarcador.Justificativas.EncerramentoManualViagem justificativaEncerramentoManual = repEncerramentoManualViagem.BuscarPorCodigo(codigoEncerramento);

                if (justificativaEncerramentoManual == null)
                    return new JsonpResult(false, true, "Justificativa não foi encontrada.");

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a carga.");

                carga.EncerramentoManualViagem = justificativaEncerramentoManual;
                carga.ObservacaoEncerramentoManualViagem = Request.GetStringParam("Observacao");

                repCarga.Atualizar(carga, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPesquisaResumoRoteiro()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Etapa, "Etapa", 20, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Inicio, "Inicio", 12, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Fim, "Fim", 12, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Total, "TotalHorasFormatado", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Freetime", "FreetimeHorasFormatado", 10, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.ControleEntrega.Exedente, "ExedenteHorasFormatado", 10, Models.Grid.Align.center, false);

            return grid;
        }

        private void PropOrdenaResumoRoteiro(ref string propOrdena)
        {
            //if (propOrdena == "DescricaoTipoCanhoto")
            //    propOrdena = "TipoCanhoto";

            //if (propOrdena == "DescricaoDigitalizacao")
            //    propOrdena = "SituacaoDigitalizacaoCanhoto";

            //if (propOrdena == "DescricaoSituacao")
            //    propOrdena = "SituacaoCanhoto";

            //else if (propOrdena == "Emitente")
            //    propOrdena = "XMLNotaFiscal.Emitente.Nome";

            //else if (propOrdena == "Empresa")
            //    propOrdena += ".RazaoSocial";

            //else if (propOrdena == "DataNotaFiscal")
            //    propOrdena = "XMLNotaFiscal.DataEmissao";

            //else if (propOrdena == "DataDigitalizacao")
            //    propOrdena = "DataEnvioCanhoto";
        }

        private IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro> ExecutaPesquisaResumoRoteiro(ref int totalRegistros, int codigoCarga, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            totalRegistros = repCargaEntrega.ContarBuscarResumoRoteiro(codigoCarga);
            IList<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro> listaGrid = (totalRegistros > 0) ? repCargaEntrega.BuscarResumoRoteiro(codigoCarga, propOrdenar, dirOrdena, inicio, limite) : new List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.ResumoRoteiro>();

            return listaGrid;
        }

        #endregion
    }
}
