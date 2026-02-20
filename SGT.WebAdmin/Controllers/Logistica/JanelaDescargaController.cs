using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Dominio.ObjetosDeValor.Relatorios;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Servicos.Extensions;
using SGTAdmin.Controllers;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize(new string[] { "BuscarSenhaAgendamento", "ExportarTabelaDescarregamento", "ObterCapacidadeDescarregamentoDados", "ObterInformacoesCentroDescarregamento", "ObterComposicaoHorarioDescarregamento" }, "Logistica/JanelaDescarga")]
    public class JanelaDescargaController : BaseController
    {
        #region Construtores

        public JanelaDescargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento, false) ?? throw new ControllerException("Não foi possível encontrar o centro de descarregamento.");

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException("Não foi possível encontrar a carga.");

                DateTime dataDescarregamento = Request.GetDateTimeParam("DataDescarregamento");
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);

                servicoCargaJanelaDescarregamento.AdicionarManualmente(carga, centroDescarregamento, dataDescarregamento);

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a carga na janela de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDescarregamentoPorPeriodo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/MontagemCarga");
            bool usuarioPossuiPermissaoSobreporRegrasCarregamento = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_SobreporRegras);

            try
            {
                unitOfWork.Start();

                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento, false) ?? throw new ControllerException("Não foi possível encontrar o centro de descarregamento.");

                int codigoCarga = Request.GetIntParam("Carga");
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga) ?? throw new ControllerException("Não foi possível encontrar a carga.");

                int codigoPeriodoDescarregamento = Request.GetIntParam("PeriodoDescarregamento");
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento configuracoesDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDescarregamento()
                {
                    PermitirHorarioDescarregamentoComLimiteAtingido = Request.GetBoolParam("PermitirHorarioDescarregamentoComLimiteAtingido"),
                    PermitirHorarioDescarregamentoInferiorAoAtual = Request.GetBoolParam("PermitirHorarioDescarregamentoInferiorAoAtual")
                };
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCodigo(codigoPeriodoDescarregamento) ?? throw new ControllerException("O período de descarregamento informado não foi encontrado.");

                DateTime dataDescarregamento = Request.GetDateTimeParam("DataDescarregamento");

                new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, configuracoesDescarregamento).AdicionarManualmente(carga, centroDescarregamento, dataDescarregamento, periodoDescarregamento);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (usuarioPossuiPermissaoSobreporRegrasCarregamento)
                {
                    if (excecao.ErrorCode == CodigoExcecao.HorarioDescarregamentoInferiorAtual)
                        return new JsonpResult(new { HorarioDescarregamentoInferiorAtual = true }, true, excecao.Message);

                    if (excecao.ErrorCode == CodigoExcecao.HorarioLimiteDescarregamentoAtingido)
                        return new JsonpResult(new { HorarioLimiteDescarregamentoAtingido = true }, true, excecao.Message);
                }

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a carga na janela de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarHorarioDescarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);


            bool solicitouAgendaExtra = Request.GetBoolParam("SolicitouAgendaExtra");
            bool solicitouAgendaAcimaDoLimite = Request.GetBoolParam("SolicitouAgendaAcimaDoLimite");
            bool solicitouMotivo = Request.GetBoolParam("SolicitouMotivo");

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/JanelaDescarga");

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_PermiteAlocarJanelaExtra) && solicitouAgendaExtra)
                    throw new ControllerException("Você não tem permissão para executar essa ação.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigoAsync(codigo, false);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = await repositorioAgendamentoColeta.BuscarPorCargaAsync(cargaJanelaDescarregamento.Carga.Codigo);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                DateTime novoHorario = Request.GetDateTimeParam("NovoHorario");
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(unitOfWork);

                cargaJanelaDescarregamento.MotivoReagendamento = Request.GetStringParam("MotivoReagendamento");

                if (!solicitouAgendaExtra && !solicitouAgendaAcimaDoLimite)
                    servicoDisponibilidadeDescarregamento.AlterarHorarioDescarregamento(cargaJanelaDescarregamento, novoHorario);
                else
                    servicoDisponibilidadeDescarregamento.AlterarHorarioDescarregamentoSemVerificarDisponibilidade(cargaJanelaDescarregamento, novoHorario, solicitouAgendaExtra);

                EnviarEmailAlteracaoHorario(agendamentoColeta, novoHorario, unitOfWork, Cliente);

                string mensagemAuditoria = solicitouMotivo ? "Reagendou o descarregamento" : "Alterou o horário de descarregamento";

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaJanelaDescarregamento, $"{mensagemAuditoria} para {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork, cancellationToken);

                if (solicitouMotivo)
                {
                    if (agendamentoColeta != null)
                    {
                        agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Agendado;

                        await repositorioAgendamentoColeta.AtualizarAsync(agendamentoColeta);

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, agendamentoColeta, $"{mensagemAuditoria} para {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork, cancellationToken);
                    }

                    servicoJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
                }
                else { 
                    servicoJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, unitOfWork);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                if (solicitouMotivo)
                    Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.ReagendarAgendamento));
                else
                    Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.AlterarHorario));

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (
                    !solicitouAgendaExtra &&
                    permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_PermiteAlocarJanelaExtra) &&
                    (
                        (excecao.ErrorCode == CodigoExcecao.HorarioDescarregamentoIndisponivel) ||
                        (excecao.ErrorCode == CodigoExcecao.HorarioLimiteDescarregamentoAtingido) ||
                        (excecao.ErrorCode == CodigoExcecao.ToleranciaAlocacaoHorarioDescarregamentoAtingida)
                    )
                )
                    return new JsonpResult(new { SolicitouAgendaExtra = true });

                if (excecao.ErrorCode == CodigoExcecao.CapacidadeGrupoPessoaAtigida && !solicitouAgendaAcimaDoLimite)
                    return new JsonpResult(new { SolicitouAgendaAcimaDoLimite = true });

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o horário de descarregamento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarHorarioDescarregamentoPorPeriodo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/MontagemCarga");
            bool usuarioPossuiPermissaoSobreporRegrasCarregamento = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.JanelaDescarga_SobreporRegras);
            bool solicitouMotivo = Request.GetBoolParam("SolicitouMotivo");
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);


            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigoAsync(codigo, false);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                int codigoPeriodoDescarregamento = Request.GetIntParam("PeriodoDescarregamento");
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = await repositorioPeriodoDescarregamento.BuscarPorCodigoAsync(codigoPeriodoDescarregamento);

                if (periodoDescarregamento == null)
                    throw new ControllerException("O período de descarregamento informado não foi encontrado.");

                DateTime dataDescarregamento = Request.GetDateTimeParam("DataDescarregamento");
                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeDescarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento()
                {
                    PermitirHorarioDescarregamentoComLimiteAtingido = Request.GetBoolParam("PermitirHorarioDescarregamentoComLimiteAtingido"),
                    PermitirHorarioDescarregamentoInferiorAoAtual = Request.GetBoolParam("PermitirHorarioDescarregamentoInferiorAoAtual")
                };
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(unitOfWork, configuracaoDisponibilidadeDescarregamento);

                cargaJanelaDescarregamento.MotivoReagendamento = Request.GetStringParam("MotivoReagendamento");

                servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamentoPorPeriodo(cargaJanelaDescarregamento, periodoDescarregamento, dataDescarregamento);
                string mensagemAuditoria = solicitouMotivo ? "Reagendou o descarregamento" : "Alterou o horário de descarregamento";

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaJanelaDescarregamento, $"{mensagemAuditoria} para {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = await repositorioAgendamentoColeta.BuscarPorCargaAsync(cargaJanelaDescarregamento.Carga.Codigo);

                if (solicitouMotivo)
                {

                    if (agendamentoColeta != null)
                    {
                        agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Agendado;

                        await repositorioAgendamentoColeta.AtualizarAsync(agendamentoColeta);

                        await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, agendamentoColeta, $"{mensagemAuditoria} para {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork, cancellationToken);
                    }

                    servicoJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
                }
                else
                {
                    servicoJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, unitOfWork);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);

                if (solicitouMotivo)
                    Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.ReagendarAgendamento));
                else
                    Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.AlterarHorario));

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                if (usuarioPossuiPermissaoSobreporRegrasCarregamento)
                {
                    if (excecao.ErrorCode == CodigoExcecao.HorarioDescarregamentoInferiorAtual)
                        return new JsonpResult(new { HorarioDescarregamentoInferiorAtual = true }, true, excecao.Message);

                    if (excecao.ErrorCode == CodigoExcecao.HorarioLimiteDescarregamentoAtingido)
                        return new JsonpResult(new { HorarioLimiteDescarregamentoAtingido = true }, true, excecao.Message);
                }

                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o horário de descarregamento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> AlterarSituacaoCadastrada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCodigo(codigo, auditavel: false);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                SituacaoCargaJanelaDescarregamentoCadastrada situacao = Request.GetEnumParam<SituacaoCargaJanelaDescarregamentoCadastrada>("Situacao");
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);

                servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, situacao);

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a situação do descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarSenhaAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<int> codigos = Request.GetListParam<int>("Codigos");

                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentosColetaPedidos = repositorioAgendamentoColetaPedido.BuscarPedidosMesmoAgendamentoPorCodigo(codigos);

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);
                Servicos.Embarcador.Integracao.SAD.IntegracaoSAD servicoIntegracaoSad = new Servicos.Embarcador.Integracao.SAD.IntegracaoSAD(unitOfWork, TipoServicoMultisoftware);

                List<Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta> retornos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColeta>();

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = agendamentosColetaPedidos.Select(obj => obj.AgendamentoColeta).Distinct().ToList();

                foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta in agendamentosColeta)
                {
                    List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> pedidosAgendamento = agendamentosColetaPedidos.Where(obj => obj.AgendamentoColeta.Codigo == agendamentoColeta.Codigo).ToList();

                    retornos.Add(servicoIntegracaoSad.ObterSenhaAgendamento(pedidosAgendamento));

                    foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SAD.RetornoIntegracaoSADAgendamentoColetaPedido retorno in retornos.LastOrDefault().Pedidos)
                    {
                        if (!retorno.Sucesso)
                            continue;

                        Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido = pedidosAgendamento.Where(obj => obj.Pedido.Codigo == retorno.CodigoPedido).FirstOrDefault();

                        agendamentoColetaPedido.AgendamentoColeta.Senha = retorno.Senha;

                        if (agendamentoColeta.Situacao == SituacaoAgendamentoColeta.AguardandoGeracaoSenha)
                        {
                            agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Agendado;

                            Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCarga(agendamentoColeta.Carga.Codigo);

                            servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento);
                        }

                        repositorioAgendamentoColeta.Atualizar(agendamentoColetaPedido.AgendamentoColeta);
                    }
                }

                return new JsonpResult(new
                {
                    Retorno = (from o in retornos
                               select new
                               {
                                   o.NumeroAgenda,
                                   o.NumeroPedidos,
                                   o.Mensagem,
                                   Pedidos = (from p in o.Pedidos
                                              select new
                                              {
                                                  DT_FontColor = p.CorFonte,
                                                  DT_RowColor = p.CorLinha,
                                                  p.Mensagem,
                                                  Pedido = p.NumeroPedido,
                                                  p.Senha
                                              }).ToList(),
                                   DT_RowColor = o.CorLinha,
                                   DT_FontColor = o.CorFonte
                               }).ToList()
                });
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu um erro ao buscar a(s) senha(s) do agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarAgendamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                List<int> codigos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

                if (codigos.Count == 0)
                    throw new ControllerException("Nenhum registro selecionado.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigosAsync(codigos);

                if (cargasJanelaDescarregamento.Count == 0)
                    throw new ControllerException("Não foi possível encontrar o(s) registro(s).");

                if (cargasJanelaDescarregamento.Any(obj =>
                    obj.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoConfirmacaoAgendamento &&
                    obj.Situacao != SituacaoCargaJanelaDescarregamento.Nucleo &&
                    obj.Situacao != SituacaoCargaJanelaDescarregamento.ValidacaoFiscal))
                {
                    throw new ControllerException("Um ou mais agendamentos selecionados já foram confirmados.");
                }

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);

                List<int> codigosCargas = cargasJanelaDescarregamento.Select(obj => obj.Carga.Codigo).ToList();

                bool centroExigeConfirmacao = cargasJanelaDescarregamento.TrueForAll(obj => obj.CentroDescarregamento.ExigeAprovacaoCargaParaDescarregamento);

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = repositorioAgendamentoColeta.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> agendamentosEntrega = await repositorioAgendamentoEntrega.BuscarPorCargasAsync(codigosCargas);
                List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> agendamentosPallet = await repositorioAgendamentoPallet.BuscarPorCargasAsync(codigosCargas);

                if (agendamentosEntrega.Count == 0 && agendamentosColeta.Count == 0 && agendamentosPallet.Count == 0 && !centroExigeConfirmacao)
                    throw new ControllerException("Não foi possível encontrar o(s) agendamento(s).");

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, null, Auditado, null);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                    servicoJanelaDescarregamento.ConfirmarAgendamento(cargaJanelaDescarregamento,
                                                                      agendamentosColeta.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                                                      agendamentosEntrega.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                                                      agendamentosPallet.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                                                      Usuario,
                                                                      TipoServicoMultisoftware);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamento in agendamentosColeta)
                {
                    Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado).EnviarEmails(agendamento, TipoGatilhoNotificacao.ConfirmarAgendamento));
                }

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar o agendamento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> CancelarAgendamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                List<int> codigos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));
                string motivoCancelamento = Request.GetStringParam("Motivo");

                if (codigos.Count == 0)
                    throw new ControllerException("Nenhum registro selecionado.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigosAsync(codigos);

                if (cargasJanelaDescarregamento.Count == 0)
                    throw new ControllerException("Não foi possível encontrar o(s) registro(s).");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);

                if (cargasJanelaDescarregamento.Exists(o => o.Situacao == SituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado))
                    throw new ControllerException("Um ou mais agendamentos selecionados já foram finalizados.");

                List<int> codigosCargas = cargasJanelaDescarregamento.Select(obj => obj.Carga.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = repositorioAgendamentoColeta.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> agendamentosPallets = await repositorioAgendamentoPallet.BuscarPorCargasAsync(codigosCargas);

                if (agendamentosColeta.Exists(o => o.Cancelado) || agendamentosPallets.Exists(o => o.Situacao == SituacaoAgendamentoPallet.Cancelado))
                    throw new ControllerException("Um ou mais agendamentos selecionados já foram cancelados.");

                StringBuilder mensagemRetorno = new StringBuilder();
                Repositorio.Embarcador.Cargas.CargaCancelamento repositorioCargaCancelamento = new Repositorio.Embarcador.Cargas.CargaCancelamento(unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
                Servicos.Embarcador.Logistica.JanelaDescarga servicoJanelaDescarga = new Servicos.Embarcador.Logistica.JanelaDescarga(unitOfWork, Auditado, TipoServicoMultisoftware, this.Usuario, ConfiguracaoEmbarcador);

                bool possuiIntegracaoSAD = await repositorioTipoIntegracao.ExistePorTipoAsync(TipoIntegracao.SAD);
                List<(string URL, int CodigoCentroDescarregamento)> urlsSad = new List<(string URL, int CodigoCentroDescarregamento)>();

                if (possuiIntegracaoSAD)
                {
                    Repositorio.Embarcador.Configuracoes.IntegracaoSAD repositorioSAD = new Repositorio.Embarcador.Configuracoes.IntegracaoSAD(unitOfWork);
                    List<int> codigosCentrosDescarregamento = cargasJanelaDescarregamento.Select(obj => obj.CentroDescarregamento.Codigo).Distinct().ToList();
                    urlsSad = repositorioSAD.BuscarURLsCancelarAgendaPorCentrosDescarregamento(codigosCentrosDescarregamento);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = (from o in agendamentosColeta where o.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo select o).FirstOrDefault();
                    Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet = (from o in agendamentosPallets where o.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo select o).FirstOrDefault();

                    bool gerarIntegracoes = !possuiIntegracaoSAD;
                    string urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == cargaJanelaDescarregamento.CentroDescarregamento.Codigo select obj.URL).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                        urlSADCentroDescarregamento = (from obj in urlsSad where obj.CodigoCentroDescarregamento == 0 select obj.URL).FirstOrDefault();

                    if (agendamentoColeta != null && !string.IsNullOrWhiteSpace(urlSADCentroDescarregamento))
                        gerarIntegracoes = true;

                    Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = servicoJanelaDescarga.CancelarAgendamento(agendamentoColeta, agendamentoPallet, cargaJanelaDescarregamento.Carga, gerarIntegracoes, motivoCancelamento);

                    if (agendamentoColeta != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta, "Agendamento de coleta cancelado via janela de descarregamento.", unitOfWork);
                        mensagemRetorno.AppendLine($"Agendamento de coleta{(string.IsNullOrWhiteSpace(agendamentoColeta.Senha) ? "" : $" {agendamentoColeta.Senha}")}{(agendamentoColeta.Carga == null ? "" : $" da carga {agendamentoColeta.Carga.CodigoCargaEmbarcador}")} cancelado com sucesso.");
                    }

                    if (agendamentoPallet != null)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoPallet, "Agendamento de pallet cancelado via janela de descarregamento.", unitOfWork);
                        mensagemRetorno.AppendLine($"Agendamento de pallet{(string.IsNullOrWhiteSpace(agendamentoPallet.Senha) ? "" : $" {agendamentoPallet.Senha}")}{(agendamentoPallet.Carga == null ? "" : $" da carga {agendamentoPallet.Carga.CodigoCargaEmbarcador}")} cancelado com sucesso.");

                        agendamentoPallet.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAgendamentoPallet.Cancelado;
                        agendamentoPallet.DataCancelamento = DateTime.Now;

                        await repositorioAgendamentoPallet.AtualizarAsync(agendamentoPallet);
                    }

                    if (cargaCancelamento != null)
                    {
                        if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                            mensagemRetorno.AppendLine($"Carga {cargaCancelamento.Carga.CodigoCargaEmbarcador} não pode ser desagendada. Motivo: {cargaCancelamento.MensagemRejeicaoCancelamento}.");
                        else
                        {
                            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.AgConfirmacao)
                            {
                                cargaCancelamento.Situacao = SituacaoCancelamentoCarga.EmCancelamento;
                                await repositorioCargaCancelamento.AtualizarAsync(cargaCancelamento);
                            }

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCancelamento.Carga, (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada ? $"Carga desagendada via janela de descarregamento" : "Desagendamento da carga solicitado via janela de descarregamento"), unitOfWork);
                            mensagemRetorno.AppendLine(cargaCancelamento.Situacao == SituacaoCancelamentoCarga.Cancelada ? $"Carga {cargaCancelamento.Carga.CodigoCargaEmbarcador} desagendada da fila de descarregamento com sucesso." : $"Desagendamento solicitado com sucesso. A carga {cargaCancelamento.Carga.CodigoCargaEmbarcador} ficará aguardando o cancelamento ser finalizado.");
                            EnviarEmailCancelamentoAgendamento(agendamentoColeta, motivoCancelamento, unitOfWork, Cliente);
                        }
                    }

                    Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);
                    servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.Cancelado);

                    await repositorioCargaJanelaDescarregamento.AtualizarAsync(cargaJanelaDescarregamento);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = (from o in agendamentosColeta where o.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo select o).FirstOrDefault();
                    Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.CancelarAgendamento));
                }


                return new JsonpResult(true, true, mensagemRetorno.ToString());
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o agendamento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExportarTabelaDescarregamento()
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

        public async Task<IActionResult> FinalizarDescarregamento(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                List<int> codigos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos"));

                if (codigos.Count == 0)
                    throw new ControllerException("Nenhum registro selecionado.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigosAsync(codigos);

                if (cargasJanelaDescarregamento.Count == 0)
                    throw new ControllerException("Não foi possível encontrar o(s) registro(s).");

                if (cargasJanelaDescarregamento.Exists(obj => obj.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento && obj.Situacao != SituacaoCargaJanelaDescarregamento.ChegadaConfirmada))
                    throw new ControllerException("Um ou mais agendamentos selecionados não estão aguardando descarregamento.");

                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork, cancellationToken);

                List<int> codigosCargas = cargasJanelaDescarregamento.Select(obj => obj.Carga.Codigo).ToList();

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta> agendamentosColeta = repositorioAgendamentoColeta.BuscarPorCargas(codigosCargas);
                List<Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet> agendamentosPallet = await repositorioAgendamentoPallet.BuscarPorCargasAsync(codigosCargas);
                List<Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega> agendamentosEntrega = await repositorioAgendamentoEntrega.BuscarPorCargasAsync(codigosCargas);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                    FinalizarDescarregamento(cargaJanelaDescarregamento,
                                             agendamentosColeta.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                             agendamentosPallet.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                             agendamentosEntrega.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo),
                                             unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                {
                    Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = agendamentosColeta.FirstOrDefault(obj => obj.Carga.Codigo == cargaJanelaDescarregamento.Carga.Codigo);
                    Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.DescarregamentoFinalizado));
                }

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao finalizar o descarregamento.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> InformarAcaoParcial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoPedido = Request.GetIntParam("CodigoPedido");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                bool entrega = Request.GetBoolParam("Entrega");
                int quantidade = Request.GetIntParam("Quantidade");
                dynamic dynProdutos = JsonConvert.DeserializeObject<dynamic>(Request.Params("Produtos"));

                unitOfWork.Start();


                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedido repositorioCargaJanelaDescarregamentoPedido = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto repositorioCargaJanelaDescarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamento = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);

                if (repositorioCargaJanelaDescarregamentoPedido.VerificarExistenciaPorPedidoCargaJanelaDescarregamento(codigoPedido, codigo))
                    throw new ControllerException("Esse pedido e essa janela já sofreram uma ação parcial.");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCodigo(codigo, false);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

                if (cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.AguardandoDescarregamento && cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.ChegadaConfirmada)
                    throw new ControllerException("A situação atual do descarregamento não permite a ação.");

                string mensagem = entrega ? "entregue parcialmente" : "devolvido parcialmente";

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorCodigo(codigoPedido);

                if (pedido == null)
                    throw new ControllerException("Não foi possível encontrar o pedido.");

                //Verifica se houve alteração na quantidade de produtos no agendamento.
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto> listaCargaJanelaDescarregamentoPedidoProduto = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>();
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = repAgendamentoColetaPedidoProduto.BuscarPorCodigoAgendamentoColeta(agendamentoColeta.Codigo);
                listAgendamentoColetaPedidoProduto = listAgendamentoColetaPedidoProduto.FindAll(x => x.PedidoProduto.Pedido.Codigo == codigoPedido);

                foreach (var produto in dynProdutos)
                {
                    Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = repositorioPedidoProduto.BuscarPorPedidoProdutoEmbarcador(codigoPedido, (string)produto.CodigoEmbarcador);
                    if (pedidoProduto == null)
                        throw new ControllerException("Não foi possível encontrar o produto para atualizar o saldo.");

                    int quantidadeEntrgada = 0;

                    if (produto.Quantidade > 0)
                    {
                        if (listAgendamentoColetaPedidoProduto?.Count > 0)
                        {
                            Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto alteracaoPedidoProdutoAgendamento = listAgendamentoColetaPedidoProduto.Find(x => x.PedidoProduto.Produto.Codigo == (int)produto.CodigoProduto);
                            if (alteracaoPedidoProdutoAgendamento != null)
                            {
                                quantidadeEntrgada = alteracaoPedidoProdutoAgendamento.Quantidade -= (int)produto.Quantidade;
                                alteracaoPedidoProdutoAgendamento.QuantidadeDeCaixas = (int)Math.Ceiling((decimal)alteracaoPedidoProdutoAgendamento.Quantidade / Math.Max(alteracaoPedidoProdutoAgendamento.PedidoProduto.Produto.QuantidadeCaixa, 1));
                                repAgendamentoColetaPedidoProduto.Atualizar(alteracaoPedidoProdutoAgendamento);
                                quantidadeEntrgada = alteracaoPedidoProdutoAgendamento.Quantidade;
                            }
                        }

                    }

                    listaCargaJanelaDescarregamentoPedidoProduto.Add(new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto()
                    {
                        CargaJanelaDescarregamentoPedido = null,
                        PedidoProduto = pedidoProduto,
                        Quantidade = quantidadeEntrgada,
                        QuantidadeAgendada = (decimal)produto.QuantidadeOriginal
                    });
                }

                int saldoProdutosDevolvidos = (int)listaCargaJanelaDescarregamentoPedidoProduto.Sum(x => x.Quantidade);
                if (saldoProdutosDevolvidos > 0)
                    quantidade = saldoProdutosDevolvidos;

                servicoAgendamento.SubtrairSaldoVolumesPendentesPedidos(agendamentoColeta, unitOfWork);

                pedido.PedidoTotalmenteCarregado = false;
                pedido.SituacaoPedido = SituacaoPedido.Aberto;
                //TODO: PPC - Adicionado log temporário para identificar problema de retorno de saldo de pedido.
                Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Liberou saldo pedido {pedido.PesoSaldoRestante} - Peso Total.: {pedido.PesoTotal} - Totalmente carregado.: {pedido.PedidoTotalmenteCarregado}. JanelaDescargaController.InformarAcaoParcial", "SaldoPedido");

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido cargaJanelaDescarregamentoPedido = new Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedido()
                {
                    CargaJanelaDescarregamento = cargaJanelaDescarregamento,
                    Pedido = pedido,
                    TipoAcaoParcial = entrega ? TipoAcaoParcial.EntregueParcialmente : TipoAcaoParcial.DevolvidaParcialmente,
                    Quantidade = quantidade
                };

                repositorioCargaJanelaDescarregamentoPedido.Inserir(cargaJanelaDescarregamentoPedido);

                foreach (var cargaJanelaDescarregamentoPedidoProduto in listaCargaJanelaDescarregamentoPedidoProduto)
                {
                    cargaJanelaDescarregamentoPedidoProduto.CargaJanelaDescarregamentoPedido = cargaJanelaDescarregamentoPedido;
                    repositorioCargaJanelaDescarregamentoPedidoProduto.Inserir(cargaJanelaDescarregamentoPedidoProduto);
                }

                repositorioPedido.Atualizar(pedido);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, pedido, null, $"Pedido {mensagem}.", unitOfWork);

                List<int> codigosPedidos = agendamentoColeta != null ? agendamentoColeta.Pedidos.Select(obj => obj.Pedido.Codigo).ToList() : new List<int>();
                List<int> codigosPedidosComAcaoParcial = repositorioCargaJanelaDescarregamentoPedido.BuscarCodigosPedidosPorCargaJanelaDescarregamento(cargaJanelaDescarregamento.Codigo);

                Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega = repositorioAgendamentoEntrega.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

                if (codigosPedidos.All(obj => codigosPedidosComAcaoParcial.Contains(obj)))
                    FinalizarDescarregamento(cargaJanelaDescarregamento, agendamentoColeta, null, agendamentoEntrega, unitOfWork);


                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);

                SituacaoCargaJanelaDescarregamento novaSituacao = entrega ? SituacaoCargaJanelaDescarregamento.EntregaParcialmente : SituacaoCargaJanelaDescarregamento.CargaDevolvidaParcialmente;
                servicoCargaJanelaDescarregamento.InserirHistoricoCargaJanelaDescarregamento(cargaJanelaDescarregamento, novaSituacao);
                cargaJanelaDescarregamento.Situacao = novaSituacao; 
                
                TipoGatilhoNotificacao tipoGatilhoNotificacao = entrega ? TipoGatilhoNotificacao.CargaEntregueParcial : TipoGatilhoNotificacao.CargaDevolvidaParcial;

                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento);

                servicoJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, unitOfWork);


                unitOfWork.CommitChanges();

                if (codigosPedidos.All(obj => codigosPedidosComAcaoParcial.Contains(obj)))
                    System.Threading.Tasks.Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, tipoGatilhoNotificacao));

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o status da janela.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarNaoComparecimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.JanelaDescarga servicoJanelaDescarga = new Servicos.Embarcador.Logistica.JanelaDescarga(unitOfWork, Auditado, TipoServicoMultisoftware, this.Usuario, ConfiguracaoEmbarcador);
                return new JsonpResult(true, true, servicoJanelaDescarga.NaoComparecimentoCargaDevolvida(JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos")), TipoGatilhoNotificacao.NoShowNaoComparecimento, true));
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
                return new JsonpResult(false, "Ocorreu uma falha ao informar o não comparecimento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarCargaDevolvida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.JanelaDescarga servicoJanelaDescarga = new Servicos.Embarcador.Logistica.JanelaDescarga(unitOfWork, Auditado, TipoServicoMultisoftware, this.Usuario, ConfiguracaoEmbarcador);
                return new JsonpResult(true, true, servicoJanelaDescarga.NaoComparecimentoCargaDevolvida(JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos")), TipoGatilhoNotificacao.CargaDevolvida));
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
                return new JsonpResult(false, "Ocorreu uma falha ao informar a devolução.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterCapacidadeDescarregamentoDados()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                DateTime? dataDescarregamento = Request.GetNullableDateTimeParam("DataDescarregamento");

                if (!dataDescarregamento.HasValue)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);

                if ((centroDescarregamento == null) || !centroDescarregamento.UtilizarCapacidadeDescarregamentoPorPeso)
                    return new JsonpResult(false);

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional repositorioCapacidadeDescarregamentoAdicional = new Repositorio.Embarcador.Logistica.CapacidadeDescarregamentoAdicional(unitOfWork);
                Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
                DiaSemana diaSemana = DiaSemanaHelper.ObterDiaSemana(dataDescarregamento.Value);
                List<dynamic> listaCapacidadeDescarregamentoPeriodo = new List<dynamic>();
                Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoCapacidadeDescarregamento = repositorioExcecaoCapacidadeDescarregamento.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo, dataDescarregamento.Value.Date, dataDescarregamento.Value.Date);

                if (centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.DiaSemana)
                {
                    int capacidadeDescarregamento = excecaoCapacidadeDescarregamento?.CapacidadeDescarregamento ?? centroDescarregamento.ObterCapacidadeDescarregamento(diaSemana);
                    int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamento(centroDescarregamento.Codigo, dataDescarregamento.Value);
                    int capacidadeDescarregamentoTotal = capacidadeDescarregamento + capacidadeDescarregamentoAdicional;
                    int capacidadeUtilizada = (int)repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoDia(0, centroDescarregamento.Codigo, dataDescarregamento.Value, centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido);
                    int capacidadeDisponivel = capacidadeDescarregamentoTotal - capacidadeUtilizada;

                    listaCapacidadeDescarregamentoPeriodo.Add(new
                    {
                        CapacidadeAdicional = capacidadeDescarregamentoAdicional.ToString("n0"),
                        CapacidadeDescarregamento = capacidadeDescarregamento.ToString("n0"),
                        CapacidadeDisponivel = capacidadeDisponivel.ToString("n0"),
                        CapacidadeUtilizada = capacidadeUtilizada.ToString("n0"),
                        Periodo = "Capacidade Diária",
                        PeriodoDescricao = "",
                        PeriodoAtivo = true
                    });
                }
                else if (centroDescarregamento.TipoCapacidadeDescarregamentoPorPeso == TipoCapacidadeDescarregamentoPorPeso.PeriodoDescarregamento)
                {
                    Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
                    ICollection<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = null;
                    int capacidadeDiariaDescarregamento = 0;
                    int capacidadeDiariaDescarregamentoAdicional = 0;
                    int capacidadeDiariaUtilizada = 0;

                    if (excecaoCapacidadeDescarregamento != null)
                        periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecaoCapacidadeDescarregamento.Codigo);
                    else if (centroDescarregamento.CapacidadeDescaregamentoPorDia)
                        periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, dataDescarregamento.Value.Day, dataDescarregamento.Value.Month);
                    else
                        periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCentroDescarregamentoEDia(centroDescarregamento.Codigo, diaSemana);

                    foreach (Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento in periodosDescarregamento)
                    {
                        List<int> codigosCanaisVenda = periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Codigo).ToList();
                        int capacidadeDescarregamento = periodoDescarregamento.CapacidadeDescarregamento;
                        int capacidadeDescarregamentoAdicional = repositorioCapacidadeDescarregamentoAdicional.BuscarCapacidadeDescarregamentoPorPeriodo(centroDescarregamento.Codigo, dataDescarregamento.Value, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, codigosCanaisVenda);
                        int capacidadeDescarregamentoTotal = capacidadeDescarregamento + capacidadeDescarregamentoAdicional;
                        int capacidadeUtilizada = (int)repositorioCargaJanelaDescarregamento.BuscarPesoTotalDescarregamentoPeriodo(0, centroDescarregamento.Codigo, dataDescarregamento.Value, periodoDescarregamento.HoraInicio, periodoDescarregamento.HoraTermino, centroDescarregamento.UtilizarCapacidadeDescarregamentoPesoLiquido, codigosCanaisVenda);
                        int capacidadeDisponivel = capacidadeDescarregamentoTotal - capacidadeUtilizada;
                        bool periodoAtivo = (periodosDescarregamento.Count == 1);

                        listaCapacidadeDescarregamentoPeriodo.Add(new
                        {
                            CapacidadeAdicional = capacidadeDescarregamentoAdicional.ToString("n0"),
                            CapacidadeDescarregamento = capacidadeDescarregamento.ToString("n0"),
                            CapacidadeDisponivel = capacidadeDisponivel.ToString("n0"),
                            CapacidadeUtilizada = capacidadeUtilizada.ToString("n0"),
                            Periodo = $"{periodoDescarregamento.HoraInicio.ToString(@"hh\:mm")} a {periodoDescarregamento.HoraTermino.ToString(@"hh\:mm")}",
                            PeriodoDescricao = (periodoDescarregamento.CanaisVenda.Count > 0) ? $"(Canais de Venda: {string.Join(", ", periodoDescarregamento.CanaisVenda.Select(canalVenda => canalVenda.CanalVenda.Descricao))})" : "",
                            PeriodoAtivo = periodoAtivo
                        });

                        capacidadeDiariaDescarregamento += capacidadeDescarregamento;
                        capacidadeDiariaDescarregamentoAdicional += capacidadeDescarregamentoAdicional;
                        capacidadeDiariaUtilizada += capacidadeUtilizada;
                    }

                    if (periodosDescarregamento.Count > 1)
                    {
                        int capacidadeDiariaDescarregamentoTotal = capacidadeDiariaDescarregamento + capacidadeDiariaDescarregamentoAdicional;
                        int capacidadeDiariaDisponivel = capacidadeDiariaDescarregamentoTotal - capacidadeDiariaUtilizada;

                        listaCapacidadeDescarregamentoPeriodo.Add(new
                        {
                            CapacidadeAdicional = capacidadeDiariaDescarregamentoAdicional.ToString("n0"),
                            CapacidadeDescarregamento = capacidadeDiariaDescarregamento.ToString("n0"),
                            CapacidadeDisponivel = capacidadeDiariaDisponivel.ToString("n0"),
                            CapacidadeUtilizada = capacidadeDiariaUtilizada.ToString("n0"),
                            Periodo = "Capacidade Diária",
                            PeriodoDescricao = "",
                            PeriodoAtivo = true
                        });
                    }
                }

                return new JsonpResult(listaCapacidadeDescarregamentoPeriodo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar aos dados de capacidade de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterConfiguracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Configuracoes.ConfiguracaoLegenda servicoConfiguracaoLegenda = new Servicos.Embarcador.Configuracoes.ConfiguracaoLegenda(unitOfWork);
                Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao repositorioJanelaDescarregamentoSituacao = new Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorioCargaJanelaDescarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao> situacoes = repositorioJanelaDescarregamentoSituacao.BuscarTodos();
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> situacoesAdicionais = repositorioCargaJanelaDescarregamentoSituacao.BuscarTodos();

                var legendasDinamicas = (
                    from situacao in situacoesAdicionais
                    select new
                    {
                        situacao.Descricao,
                        Cores = new
                        {
                            Fonte = Utilidades.Cores.ObterCorPorPencentual(situacao.Cor, percentual: 40),
                            Fundo = situacao.Cor
                        }
                    }
                ).ToList();

                var legendasPorSituacao = (
                    from situacao in situacoes
                    select new
                    {
                        situacao.Codigo,
                        situacao.Descricao,
                        situacao.Situacao,
                        Cores = new
                        {
                            Fonte = Utilidades.Cores.ObterCorPorPencentual(situacao.Cor, percentual: 40),
                            Fundo = situacao.Cor
                        }
                    }
                ).ToList();

                return new JsonpResult(new
                {
                    Legendas = servicoConfiguracaoLegenda.ObterPorGrupoCodigoControle(GrupoCodigoControleLegenda.JanelaDescarregamento),
                    LegendasDinamicas = legendasDinamicas,
                    LegendasPorSituacao = legendasPorSituacao
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

        public async Task<IActionResult> ObterInformacoesCentroDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);

                var informacoesCentroDescarregamento = new
                {
                    centroDescarregamento.BloquearJanelaDescarregamentoExcedente,
                    centroDescarregamento.PermitirGerarDescargaArmazemExterno
                };

                return new JsonpResult(informacoesCentroDescarregamento);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter as informações do centro de descarregamento.");
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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> DesagendarCarga(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork, cancellationToken);

                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamento = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repositorioCargaJanelaDescarregamento.BuscarPorCodigoAsync(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = await repositorioAgendamentoColeta.BuscarPorCargaAsync(cargaJanelaDescarregamento.Carga.Codigo);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (agendamentoColeta != null)
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Cancelado;
                    foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedido in agendamentoColeta.Pedidos)
                    {
                        agendamentoColetaPedido.Pedido.PedidoTotalmenteCarregado = false;
                        await repositorioPedido.AtualizarAsync(agendamentoColetaPedido.Pedido);
                    }

                    await repositorioAgendamentoColeta.AtualizarAsync(agendamentoColeta);

                    servicoAgendamento.SubtrairSaldoVolumesPendentesPedidos(agendamentoColeta, unitOfWork);

                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, agendamentoColeta, "Carga desagendada.", unitOfWork, cancellationToken);
                }

                cargaJanelaDescarregamento.Cancelada = true;
                await repositorioCargaJanelaDescarregamento.AtualizarAsync(cargaJanelaDescarregamento);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaJanelaDescarregamento, "Carga desagendada.", unitOfWork, cancellationToken);

                EnviarEmailDesagendamentoCarga(agendamentoColeta, unitOfWork, Cliente);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.Desagendar));

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao desagendar a carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterComposicaoHorarioDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaJanelaDescarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario repositorioComposicaoHorario = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario> composicoesHorarioDescarregamento = repositorioComposicaoHorario.BuscarPorCargaJanelaDescarregamento(codigoCargaJanelaDescarregamento);

                if (composicoesHorarioDescarregamento.Count == 0)
                    return new JsonpResult(false, true, "Nenhuma composição de horário encontrada para a janela de descarregamento.");

                List<int> codigosComposicoesHorario = composicoesHorarioDescarregamento.Select(o => o.Codigo).ToList();
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe repositorioComposicaoHorarioDetalhe = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe> composicoesHorarioDescarregamentoDetalhes = repositorioComposicaoHorarioDetalhe.BuscarPorComposicaoHorario(codigosComposicoesHorario);

                var composicoesHorarioDescarregamentoRetornar = (
                    from composicao in composicoesHorarioDescarregamento
                    select ObterComposicaoHorarioDescarregamento(composicao, composicoesHorarioDescarregamentoDetalhes)
                ).ToList();

                return new JsonpResult(composicoesHorarioDescarregamentoRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar as composições de horário de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarHistoricoIntegracaoAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);

                List<int> codigos = Request.GetListParam<int>("Codigos");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data", "Data", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tipo", "DescricaoTipo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Retorno", "Mensagem", 40, Models.Grid.Align.left, false);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> integracoes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorCodigos(codigos);

                foreach (var integracao in agendamentoColetaPedido)
                    integracoes.AddRange(integracao.ArquivosTransacao.Select(o => o));

                grid.setarQuantidadeTotal(integracoes.Count());

                var retorno = (from obj in integracoes.OrderByDescending(o => o.Data).Skip(grid.inicio).Take(grid.limite)
                               select new
                               {
                                   obj.Codigo,
                                   Data = obj.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                                   obj.DescricaoTipo,
                                   obj.Mensagem
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar o histórico de integração do agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadArquivosHistoricoIntegracaoAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido agendamentoColetaPedidoIntegracao = repositorioAgendamentoColetaPedido.BuscarPorCodigoArquivo(codigo);

                if (agendamentoColetaPedidoIntegracao == null)
                    return new JsonpResult(true, false, "Histórico de integração do agendamento não encontrado.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = agendamentoColetaPedidoIntegracao.ArquivosTransacao.Where(o => o.Codigo == codigo).FirstOrDefault();

                if (arquivoIntegracao == null || (arquivoIntegracao.ArquivoRequisicao == null && arquivoIntegracao.ArquivoResposta == null))
                    return new JsonpResult(true, false, "Não há registros de arquivos salvos para este histórico de integração.");

                byte[] arquivo = Servicos.Embarcador.Integracao.ArquivoIntegracao.CriarZip(new List<Dominio.Entidades.Embarcador.Integracao.ArquivoIntegracao>() { arquivoIntegracao.ArquivoRequisicao, arquivoIntegracao.ArquivoResposta });

                return Arquivo(arquivo, "application/zip", "Arquivos Integracao Agendamento Pedido " + agendamentoColetaPedidoIntegracao.Pedido.NumeroPedidoEmbarcador + ".zip");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao realizar o download dos arquivos de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarObservacaoFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamentoReferencia = repositorioCargaJanelaDescarregamento.BuscarPorCodigo(codigo, auditavel: false);

                if (cargaJanelaDescarregamentoReferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                string observacaoFluxoPatio = Request.GetNullableStringParam("Observacao");

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoConsulta servicoCargaJanelaDescarregamentoConsulta = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoConsulta(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = servicoCargaJanelaDescarregamentoConsulta.ObterCargasJanelaCarregamentoComCargasAgrupadas(cargaJanelaDescarregamentoReferencia);

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                {
                    cargaJanelaDescarregamento.Initialize();
                    cargaJanelaDescarregamento.ObservacaoFluxoPatio = observacaoFluxoPatio;

                    repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar a observação do fluxo de pátio");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarDataPrevisaoChegada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCodigo(codigo, auditavel: false);

                if (cargaJanelaDescarregamento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                DateTime dataPrevisaoChegada = Request.GetDateTimeParam("DataPrevisaoChegada");

                if (dataPrevisaoChegada == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data Obrigatória");

                cargaJanelaDescarregamento.DataPrevisaoChegada = dataPrevisaoChegada;

                repositorioCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamento, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a data de previsão de chegada");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarMultiplosHorariosDescarregamentosPorPeriodo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPeriodoDescarregamento = Request.GetIntParam("PeriodoDescarregamento");
                List<int> codigos = Request.GetListParam<int>("Codigos");

                if (codigos.Count == 0)
                    throw new ControllerException("Nenhum registro selecionado.");

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoDisponibilidadeDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(unitOfWork);

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargasJanelaDescarregamento = repositorioCargaJanelaDescarregamento.BuscarPorCodigos(codigos);
                Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCodigo(codigoPeriodoDescarregamento);

                if (cargasJanelaDescarregamento.Count < 0)
                    throw new ControllerException("Não foi possível encontrar os registros!");

                if (periodoDescarregamento == null)
                    throw new ControllerException("O período de descarregamento informado não foi encontrado.");

                DateTime dataDescarregamento = Request.GetDateTimeParam("DataDescarregamento");

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento in cargasJanelaDescarregamento)
                {
                    servicoDisponibilidadeDescarregamento.DefinirHorarioDescarregamentoPorPeriodo(cargaJanelaDescarregamento, periodoDescarregamento, dataDescarregamento);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaDescarregamento, null, $"Alterou o horário de descarregamento para {cargaJanelaDescarregamento.InicioDescarregamento.ToString("dd/MM/yyyy HH:mm")}.", unitOfWork);
                }

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar o horário de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarChegada(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork, cancellationToken);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);
                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);

                int codigo = Request.GetIntParam("CodigoJanelaDescarregamento");
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoMotorista = Request.GetIntParam("Motorista");
                int codigoTracao = Request.GetIntParam("Veiculo");
                int codigoReboque = Request.GetIntParam("Reboque");
                int codigoSegundoReboque = Request.GetIntParam("SegundoReboque");
                string rg = Request.GetStringParam("RG");
                string telefone = Request.GetStringParam("Telefone");


                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repCargaJanelaDescarregamento.BuscarPorCodigoAsync(codigo, auditavel: false);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = await repCarga.BuscarPorCodigoAsync(codigoCarga);
                Dominio.Entidades.Usuario motorista = await repUsuario.BuscarMotoristaPorCodigoAsync(codigoMotorista);
                Dominio.Entidades.Veiculo tracao = await repVeiculo.BuscarPorCodigoAsync(codigoTracao);
                Dominio.Entidades.Veiculo reboque = await repVeiculo.BuscarPorCodigoAsync(codigoReboque);
                Dominio.Entidades.Veiculo segundoReboque = await repVeiculo.BuscarPorCodigoAsync(codigoSegundoReboque);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                await unitOfWork.StartAsync(cancellationToken);

                if (tracao == null)
                    throw new ControllerException("O Veículo não pode estar vazio.");

                if (motorista == null)
                    throw new ControllerException("O Motorista não pode estar vazio.");

                carga.Veiculo = tracao;

                if (carga.VeiculosVinculados == null)
                    carga.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();
                carga.VeiculosVinculados.Clear();

                if (reboque != null)
                    carga.VeiculosVinculados.Add(reboque);
                if (segundoReboque != null)
                    carga.VeiculosVinculados.Add(segundoReboque);

                motorista.RG = rg;
                motorista.Telefone = telefone;
                await repUsuario.AtualizarAsync(motorista);

                if (carga.Motoristas == null)
                    carga.Motoristas = new List<Dominio.Entidades.Usuario>();
                carga.Motoristas.Clear();
                carga.Motoristas.Add(motorista);

                if (cargaJanelaDescarregamento.Situacao != SituacaoCargaJanelaDescarregamento.ChegadaConfirmada)
                    servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.ChegadaConfirmada);
                else
                    await repCargaJanelaDescarregamento.AtualizarAsync(cargaJanelaDescarregamento);

                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaJanelaDescarregamento, "Chegada Confirmada.", unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = await repositorioAgendamentoColeta.BuscarPorCargaAsync(carga.Codigo);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.ChegadaConfirmada));

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao desagendar a carga.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ConfirmarSaidaVeiculo(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaDescarregamento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork, cancellationToken);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork, cancellationToken);
                
                Servicos.Embarcador.Email.ConfiguracaoModeloEmail servicoConfiguracaoModeloEmail = new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(_conexao.StringConexao, Usuario, Auditado);
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = await repCargaJanelaDescarregamento.BuscarPorCodigoAsync(codigoJanelaDescarregamento, auditavel: false);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repAgendamentoColeta.BuscarAgendamentoAbertoPorCarga(cargaJanelaDescarregamento.Carga.Codigo);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                await unitOfWork.StartAsync(cancellationToken);

                await servicoCargaJanelaDescarregamento.InserirHistoricoCargaJanelaDescarregamentoAsync(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.SaidaVeiculoConfirmada);

                cargaJanelaDescarregamento.Situacao = SituacaoCargaJanelaDescarregamento.SaidaVeiculoConfirmada;

                servicoCargaJanelaDescarregamento.AdicionarIntegracaoComAtualizacao(cargaJanelaDescarregamento.Carga, unitOfWork);

                await repCargaJanelaDescarregamento.AtualizarAsync(cargaJanelaDescarregamento);
                await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, cargaJanelaDescarregamento, "Saída do Veículo Confirmada.", unitOfWork, cancellationToken);

                if (agendamentoColeta != null)
                {
                    agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Finalizado;
                    await repAgendamentoColeta.AtualizarAsync(agendamentoColeta);
                    await Servicos.Auditoria.Auditoria.AuditarAsync(Auditado, agendamentoColeta, "Saída do Veículo Confirmada.", unitOfWork, cancellationToken);
                }

                await unitOfWork.CommitChangesAsync(cancellationToken);

                if (agendamentoColeta != null)
                    Task.Factory.StartNew(() => servicoConfiguracaoModeloEmail.EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.SaidaConfirmada));

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao confirmar saída do veículo");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ObterDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                int codigo = Request.GetIntParam("CodigoCarga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigo);

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");


                var retorno = new
                {
                    VeiculoCodigo = carga.Veiculo?.Codigo ?? 0,
                    VeiculoDescricao = carga.Veiculo?.Placa ?? string.Empty,
                    PrimeiroReboqueCodigo = carga.VeiculosVinculados?.FirstOrDefault()?.Codigo ?? 0,
                    SegundoReboqueCodigo = carga.VeiculosVinculados.ElementAtOrDefault(1)?.Codigo ?? 0,
                    PrimeiroReboqueDescricao = carga.VeiculosVinculados?.FirstOrDefault()?.Placa ?? string.Empty,
                    SegundoReboqueDescricao = carga.VeiculosVinculados?.ElementAtOrDefault(1)?.Placa ?? string.Empty,
                    MotoristaCodigo = carga.Motoristas?.FirstOrDefault()?.Codigo ?? 0,
                    MotoristaDescricao = carga.Motoristas?.FirstOrDefault()?.Nome ?? string.Empty,
                    MotoristaRG = carga.Motoristas?.FirstOrDefault()?.RG,
                    MotoristaTelefone = carga.Motoristas?.FirstOrDefault()?.Telefone_Formatado,
                };

                return new JsonpResult(retorno);
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
                return new JsonpResult(false, "Ocorreu uma falha ao desagendar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterHorariosDisponiveis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Descricao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DescricaoDataDescarga", visivel: false);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Dia da Semana", propriedade: "DiaSemana", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Horário", propriedade: "Horario", tamanho: 30, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                DateTime dia = Request.GetDateTimeParam("DataAgendamentoDisponibilidade");

                if (dia == DateTime.MinValue)
                    return new JsonpResult(false, Localization.Resources.Cargas.Carga.DataObrigatoria);

                Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeDescarregamento = PreencherConfiguracaoDisponibilidadeDescarregamento();

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade servicoJanelaDescarregamentoDisponibilidade = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamentoDisponibilidade(unitOfWork, ConfiguracaoEmbarcador, configuracaoDisponibilidadeDescarregamento);

                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = BuscarCentroDescarregamentoPorParametro(unitOfWork);

                if (centroDescarregamento == null)
                {
                    grid.AdicionaRows(new List<dynamic> { });
                    return new JsonpResult(grid);
                }

                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> horarios = servicoJanelaDescarregamentoDisponibilidade.ObterPeriodosDescarregamentoDisponiveis(centroDescarregamento, dia);

                grid.setarQuantidadeTotal(horarios.Count);
                grid.AdicionaRows((from periodo in horarios
                                   select new
                                   {
                                       periodo.Codigo,
                                       Data = dia.ToDateString(),
                                       Descricao = $"{periodo.HoraInicio:hh\\:mm} até {periodo.HoraTermino:hh\\:mm}",
                                       DiaSemana = periodo.Dia.ObterDescricaoResumida(),
                                       Horario = $"{periodo.HoraInicio:hh\\:mm} - {periodo.HoraTermino:hh\\:mm}",
                                       DescricaoDataDescarga = $"{dia:dd/MM} - {periodo.HoraInicio.ToString(@"hh\:mm")} até {periodo.HoraTermino.ToString(@"hh\:mm")}",
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, Localization.Resources.Cargas.Carga.OcorreuUmaFalhaAoObterAsInformacoesDoCentroDeCarregamento);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> ComprovanteCargaInformada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                bool descarga = Request.GetBoolParam("Descarga");
                string senhaAgendamento = Request.GetStringParam("SenhaAgendamento");
                int codigoJanelaDescarregamento = Request.GetIntParam("CodigoJanelaDescarregamento");

                var pdf = ReportRequest.WithType(ReportType.ComprovanteCargaInformadaJanelaDescarga)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo.ToString())
                    .AddExtraData("descarga", descarga.ToString())
                    .AddExtraData("senhaAgendamento", senhaAgendamento.ToString())
                    .AddExtraData("codigoJanelaDescarregamento", codigoJanelaDescarregamento.ToString())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", Localization.Resources.GestaoPatio.FluxoPatio.ComprovanteDeCargaInformada + ".pdf");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPedidoProdutosDescargaParcial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto repAgendamentoColetaPedidoProduto = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedidoProduto(unitOfWork);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                int codigoPedido = Request.GetIntParam("CodigoPedido");

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                List<dynamic> retorno = new List<dynamic>();

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repAgendamentoColeta.BuscarPorCarga(codigoCarga);
                List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto> listAgendamentoColetaPedidoProduto = repAgendamentoColetaPedidoProduto.BuscarPorCodigoAgendamentoColeta(agendamentoColeta.Codigo);

                if (listAgendamentoColetaPedidoProduto != null)
                {
                    foreach (Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedidoProduto agendamentoColetaPedidoProduto in listAgendamentoColetaPedidoProduto.FindAll(x => x.PedidoProduto.Pedido.Codigo == codigoPedido))
                    {
                        retorno.Add(new
                        {
                            Codigo = agendamentoColetaPedidoProduto.PedidoProduto.Codigo,
                            CodigoProduto = agendamentoColetaPedidoProduto.PedidoProduto.Produto.Codigo,
                            CodigoEmbarcador = agendamentoColetaPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                            Descricao = agendamentoColetaPedidoProduto.PedidoProduto.Produto.Descricao ?? string.Empty,
                            Setor = agendamentoColetaPedidoProduto.PedidoProduto.Produto.GrupoProduto.Descricao ?? string.Empty,
                            Quantidade = 0,
                            QuantidadeOriginal = agendamentoColetaPedidoProduto.Quantidade,
                            Removido = false,
                            DT_Enable = true
                        });
                    }
                }

                return new JsonpResult(retorno);
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
                return new JsonpResult(false, "Ocorreu uma falha ao obter produtos da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VisualizarPedidoProdutosDescargaParcial()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto repositorioCargaJanelaDescarregamentoPedidoProduto = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto(unitOfWork);

                int quantidadeNaoEntregue = 0;
                bool entregueParcial = false;
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (carga == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                List<dynamic> retorno = new List<dynamic>();

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto> listaCargaJanelaDescarregamentoPedidoProduto = repositorioCargaJanelaDescarregamentoPedidoProduto.BuscarPorCarga(codigoCarga);

                if (listaCargaJanelaDescarregamentoPedidoProduto != null && listaCargaJanelaDescarregamentoPedidoProduto.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto cargaJanelaDescarregamentoPedidoProduto in listaCargaJanelaDescarregamentoPedidoProduto)
                    {
                        entregueParcial = cargaJanelaDescarregamentoPedidoProduto.CargaJanelaDescarregamentoPedido.TipoAcaoParcial == TipoAcaoParcial.EntregueParcialmente;
                        quantidadeNaoEntregue = cargaJanelaDescarregamentoPedidoProduto.CargaJanelaDescarregamentoPedido.Quantidade;
                        retorno.Add(new
                        {
                            Codigo = cargaJanelaDescarregamentoPedidoProduto.PedidoProduto.Codigo,
                            CodigoProduto = cargaJanelaDescarregamentoPedidoProduto.PedidoProduto.Produto.Codigo,
                            CodigoEmbarcador = cargaJanelaDescarregamentoPedidoProduto.PedidoProduto.Produto.CodigoProdutoEmbarcador ?? string.Empty,
                            Descricao = cargaJanelaDescarregamentoPedidoProduto.PedidoProduto.Produto.Descricao ?? string.Empty,
                            Setor = cargaJanelaDescarregamentoPedidoProduto.PedidoProduto.Produto.GrupoProduto.Descricao ?? string.Empty,
                            Quantidade = cargaJanelaDescarregamentoPedidoProduto.Quantidade,
                            QuantidadeOriginal = cargaJanelaDescarregamentoPedidoProduto.QuantidadeAgendada,
                            Removido = false,
                            DT_Enable = false
                        });
                    }
                }

                return new JsonpResult(new { Produtos = retorno, EntregueParcial = entregueParcial, QuantidadeNaoEntregue = quantidadeNaoEntregue });
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
                return new JsonpResult(false, "Ocorreu uma falha ao obter produtos da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarDescargaArmazemExterno()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoJanelaDescarregamento = Request.GetIntParam("CodigoJanelaDescarregamento");
                int codigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento");
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.Embarcador.Cargas.Redespacho repRedespacho = new Repositorio.Embarcador.Cargas.Redespacho(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento = repCargaJanelaDescarregamento.BuscarPorCodigo(codigoJanelaDescarregamento, auditavel: false);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repCentroDescarregamento.BuscarPorCodigo(codigoCentroDescarregamento);
                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao);

                if (cargaJanelaDescarregamento == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (centroDescarregamento == null || tipoOperacao == null)
                    throw new ControllerException("Centro de Descarregamento ou Tipo de Operação não encontrados.");

                if (cargaJanelaDescarregamento.CentroDescarregamento == centroDescarregamento)
                    throw new ControllerException("O Centro de Descarregamento Externo não pode ser igual ao atual.");

                if (!tipoOperacao.PermitirAdicionarNaJanelaDescarregamento)
                    throw new ControllerException("O tipo de operação da carga não permite adicionar na janela de descarrregamento.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Cargas.Redespacho redespacho = new Dominio.Entidades.Embarcador.Cargas.Redespacho()
                {
                    DataRedespacho = DateTime.Now,
                    Carga = cargaJanelaDescarregamento.Carga,
                    NumeroRedespacho = repRedespacho.BuscarProximoCodigo(),
                    TipoOperacao = tipoOperacao,
                    Expedidor = cargaJanelaDescarregamento.CentroDescarregamento.Destinatario,
                };

                repRedespacho.Inserir(redespacho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarCargaPedidoPorCargaEDestinatario(cargaJanelaDescarregamento.Carga.Codigo, cargaJanelaDescarregamento.CentroDescarregamento.Destinatario.CPF_CNPJ);

                if (cargaPedidos.Count <= 0)
                    throw new ControllerException("Não foram encontrados pedidos compatíveis para adicionar na Carga de Redespacho.");

                Dominio.Entidades.Usuario motorista = cargaJanelaDescarregamento.Carga.Motoristas.FirstOrDefault();
                List<Dominio.Entidades.Veiculo> veiculosVinculador = cargaJanelaDescarregamento.Carga.VeiculosVinculados.ToList();

                Dominio.Entidades.Embarcador.Cargas.Carga cargaGerada = Servicos.Embarcador.Carga.CargaDistribuidor.GerarCargaProximoTrecho(cargaJanelaDescarregamento.Carga, tipoOperacao, 0, true, cargaJanelaDescarregamento.CentroDescarregamento.Destinatario, cargaPedidos, cargaJanelaDescarregamento.Carga.Empresa, ConfiguracaoEmbarcador, true, redespacho, cargaJanelaDescarregamento.Carga.Veiculo, TipoServicoMultisoftware, unitOfWork, false, cargaJanelaDescarregamento.Carga.ModeloVeicularCarga, centroDescarregamento.Destinatario, motorista, veiculosVinculador);

                if (cargaGerada == null)
                    throw new ControllerException("Ocorreu uma falha ao gerar a Carga de Redespacho");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaGerada, "Carga criada via adição de Descarregamento de Armazém Externo", unitOfWork);

                redespacho.CargaGerada = cargaGerada;

                repRedespacho.Atualizar(redespacho);

                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);
                servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.Cancelado);

                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamentoNovo = repCargaJanelaDescarregamento.BuscarPorCargaECentroDescarregamento(cargaGerada.Codigo, centroDescarregamento.Codigo);

                if (cargaJanelaDescarregamentoNovo == null)
                    cargaJanelaDescarregamentoNovo = repCargaJanelaDescarregamento.BuscarPorCarga(cargaGerada.Codigo);

                Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                servicoAgendamentoColeta.DuplicarAgendamentoColeta(cargaJanelaDescarregamentoNovo, cargaJanelaDescarregamento.Carga, unitOfWork);

                cargaJanelaDescarregamentoNovo.InicioDescarregamento = cargaJanelaDescarregamento.InicioDescarregamento;

                repCargaJanelaDescarregamento.Atualizar(cargaJanelaDescarregamentoNovo);

                unitOfWork.CommitChanges();

                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta = repositorioAgendamentoColeta.BuscarPorCarga(cargaJanelaDescarregamento.Carga.Codigo);
                Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColetaNovaCarga = repositorioAgendamentoColeta.BuscarPorCarga(cargaGerada.Codigo);
                System.Threading.Tasks.Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(unitOfWork.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColetaNovaCarga, TipoGatilhoNotificacao.DescargaArmazemExterno));
                System.Threading.Tasks.Task.Factory.StartNew(() => new Servicos.Embarcador.Email.ConfiguracaoModeloEmail(unitOfWork.StringConexao, Usuario, Auditado).EnviarEmails(agendamentoColeta, TipoGatilhoNotificacao.CancelarAgendamento));

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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a Descarga em Armazém Externo");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSituacaoCargaJanelaDescarregamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                List<int> codigos = Request.GetListParam<int>("Codigos");

                if (codigos.Count == 0)
                    throw new ControllerException("Nenhum registro selecionado.");

                Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento> cargaJanelaDescarregamentos = repositorioCargaJanelaDescarregamento.BuscarPorCodigos(codigos, auditavel: false);

                string erroMsg = codigos.Count == 1 ? "Não foi possível encontrar o registro" : "Não foi possível encontrar os registros.";

                if (cargaJanelaDescarregamentos.Count == 0)
                    throw new ControllerException(erroMsg);

                SituacaoCargaJanelaDescarregamento situacao = Request.GetEnumParam<SituacaoCargaJanelaDescarregamento>("Situacao");
                Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, Auditado);

                foreach (var cargaJanelaDescarregamento in cargaJanelaDescarregamentos)
                    servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, situacao);

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
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a situação do descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterComposicaoHorarioDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario composicaoHorarioDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe> composicoesHorarioDescarregamentoDetalhes)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe> detalhes = (from o in composicoesHorarioDescarregamentoDetalhes where o.ComposicaoHorario.Codigo == composicaoHorarioDescarregamento.Codigo select o).ToList();

            return new
            {
                DataCriacao = composicaoHorarioDescarregamento.DataCriacao.ToDateTimeString(),
                Descricoes = (
                    from detalhe in detalhes
                    select new
                    {
                        Ordem = detalhe.Ordem,
                        Descricao = detalhe.Descricao
                    }
                ).ToList()
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento()
            {
                CodigoCentroDescarregamento = Request.GetIntParam("CentroDescarregamento"),
                CodigoSituacao = Request.GetIntParam("SituacaoCadastrada"),
                DataDescarregamento = ConfiguracaoEmbarcador.ExibirJanelaDescargaPorPeriodo ? DateTime.MinValue : Request.GetDateTimeParam("DataDescarregamento"),
                DataDescarregamentoInicial = ConfiguracaoEmbarcador.ExibirJanelaDescargaPorPeriodo ? Request.GetDateTimeParam("DataDescarregamentoInicial") : DateTime.MinValue,
                DataDescarregamentoFinal = ConfiguracaoEmbarcador.ExibirJanelaDescargaPorPeriodo ? Request.GetDateTimeParam("DataDescarregamentoFinal") : DateTime.MinValue,
                SenhaAgendamento = Request.GetStringParam("SenhaAgendamento"),
                Situacao = Request.GetListEnumParam<SituacaoCargaJanelaDescarregamento>("Situacao"),
                SituacaoAgendamentoPallet = Request.GetNullableEnumParam<SituacaoAgendamentoPallet>("SituacaoAgendamentoPallet"),
                CodigoFornecedor = Request.GetDoubleParam("Fornecedor"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                DataLancamento = Request.GetDateTimeParam("DataLancamento"),
                CodigosFilial = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosFilialVenda = ObterListaCodigoFilialVendaPermitidasOperadorLogistica(unitOfWork),
                CodigosTipoCarga = ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork),
                CodigosTipoOperacao = ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork),
                NumeroNF = Request.GetStringParam("NumeroNF"),
                NumeroCTe = Request.GetStringParam("NumeroCTe"),
                NumeroLacre = Request.GetStringParam("NumeroLacre"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                ExcedenteDescarregamento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("ExcedenteDescarregamento"),
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaCargaJanelaDescarregamento filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao repositorioJanelaDescarregamentoSituacao = new Repositorio.Embarcador.Cargas.JanelaDescarregamentoSituacao(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento = repositorioCentroDescarregamento.BuscarPorCodigo(filtrosPesquisa.CodigoCentroDescarregamento);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                List<Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao> situacoesCadastradas = repositorioJanelaDescarregamentoSituacao.BuscarTodos();

                filtrosPesquisa.UtilizarDadosDosPedidos = centroDescarregamento?.ExibirJanelaDescargaPorPedido ?? false;
                filtrosPesquisa.TempoSemPosicaoParaVeiculoPerderSinal = configuracaoEmbarcador.TempoSemPosicaoParaVeiculoPerderSinal;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("CodigoPedido", false);
                grid.AdicionarCabecalho("CPFCNPJDestinatario", false);
                grid.AdicionarCabecalho("Destinatario", false);
                grid.AdicionarCabecalho("Situacao", false);
                grid.AdicionarCabecalho("SituacaoCarga", false);
                grid.AdicionarCabecalho("PermiteInformarAcaoParcial", false);
                grid.AdicionarCabecalho("PermitirInformarObservacaoFluxoPatio", false);
                grid.AdicionarCabecalho("CodigoAgendamentoColeta", false);
                grid.AdicionarCabecalho("CodigoAgendamentoPallet", false);
                grid.AdicionarCabecalho("CodigoAgendamentoColetaPedido", false);
                grid.AdicionarCabecalho("QuantidadeArquivoIntegracao", false);
                grid.AdicionarCabecalho("CodigoMonitoramento", false);
                grid.AdicionarCabecalho("CodigoSituacaoCadastrada", false);
                grid.AdicionarCabecalho("ExibirJanelaDescargaPorPedido", false);
                grid.AdicionarCabecalho("ExcedenteDescarregamento", false);
                grid.AdicionarCabecalho("Data Lançamento", "DataLancamentoDescricao", 8, Models.Grid.Align.center, true, false, false, true, true);
                grid.AdicionarCabecalho("Data Agendada", "DataDescarregamento", 8, Models.Grid.Align.center, !filtrosPesquisa.UtilizarDadosDosPedidos, false, false, true, configuracaoEmbarcador.ExibirJanelaDescargaPorPeriodo);
                grid.AdicionarCabecalho("Hora Agendada", "HoraDescarregamento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Senha do Agendamento", "SenhaAgendamento", 8, Models.Grid.Align.center, false, configuracaoEmbarcador.UtilizarSituacaoNaJanelaDescarregamento);
                grid.AdicionarCabecalho("Status Busca de Senha Automática", "StatusBuscaSenhaAutomatica", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true, !filtrosPesquisa.UtilizarDadosDosPedidos);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 10, Models.Grid.Align.center, false, !filtrosPesquisa.UtilizarDadosDosPedidos);
                grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, false, !filtrosPesquisa.UtilizarDadosDosPedidos);
                grid.AdicionarCabecalho("Número do Pedido", "NumeroPedido", 10, Models.Grid.Align.left, false, configuracaoEmbarcador.ExibirNumeroPedidoJanelaCarregamentoEDescarregamento);
                grid.AdicionarCabecalho("Categoria", "Categoria", 10, Models.Grid.Align.left, false, configuracaoEmbarcador.ControlarAgendamentoSKU);
                grid.AdicionarCabecalho("Centro de Descarregamento", "CentroDescarregamento", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Excedente", "Excedente", 10, Models.Grid.Align.center, false, true);

                if (situacoesCadastradas.Count == 0)
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 15, Models.Grid.Align.center, !filtrosPesquisa.UtilizarDadosDosPedidos, configuracaoEmbarcador.UtilizarSituacaoNaJanelaDescarregamento);
                else
                    grid.AdicionarCabecalho("Situação", "DescricaoSituacaoCadastrada", 15, Models.Grid.Align.center, !filtrosPesquisa.UtilizarDadosDosPedidos, configuracaoEmbarcador.UtilizarSituacaoNaJanelaDescarregamento);

                grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Solicitada", "DataAgendamento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Hora Solicitada", "HoraAgendamento", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 8, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Modalidade", "Modalidade", 8, Models.Grid.Align.left, false, filtrosPesquisa.UtilizarDadosDosPedidos);
                grid.AdicionarCabecalho("Tipo de Carga", "TipoDeCarga", 8, Models.Grid.Align.left, false, filtrosPesquisa.UtilizarDadosDosPedidos);
                grid.AdicionarCabecalho("Qtd. Caixas", "VolumeEmCx", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. Itens", "QtdItens", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Qtd. Produtos", "QtdProdutos", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data Fim Janela", "DataFimJanela", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Previsão de Entrega", "PrevisaoEntrega", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Observação do Pedido", "ObservacaoPedido", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Origem da Carga", "OrigemCarga", 10, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Peso", "PesoFormatado", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Peso Líquido", "PesoLiquidoFormatado", 8, Models.Grid.Align.center, false, false);

                if (configuracaoEmbarcador.PossuiMonitoramento)
                {
                    grid.AdicionarCabecalho("Status da Viagem", "StatusViagem", 8, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("KM até o Destino", "KmAteDestinoFormatado", 8, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("Previsão de Entrega Reprogramada", "DataProximaEntregaReprogramadaFormatada", 8, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("Data Entrada no Raio", "DataEntradaRaioFormatada", 8, Models.Grid.Align.center, false, false);
                    grid.AdicionarCabecalho("Notas Fiscais da Entrega", "NotasFiscaisEntrega", 8, Models.Grid.Align.left, false, false);
                    grid.AdicionarCabecalho("Rastreador", "RastreadorOnline", 8, Models.Grid.Align.center, false, false);
                }

                grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Remetente", "Remetente", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Observação Fluxo Pátio", "ObservacaoFluxoPatio", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Disponibilidade do Veículo", "DisponibilidadeVeiculo", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("SetPoint Transp.", "SetPointTransp", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Tipo de Carga Taura", "TipoCargaTaura", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Range Temp. Transp.", "RangeTempTransp", 8, Models.Grid.Align.left, false, false);
                grid.AdicionarCabecalho("Valor", "Valor", 8, Models.Grid.Align.center, false, true);
                grid.AdicionarCabecalho("Agenda Extra", "AgendaExtraDescricao", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Qtd. Notas", "QuantidadeNotas", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Qtd. AVIPED", "QuantidadeNotasAVIPED", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Unidade de Medida", "UnidadeMedidaAgendamento", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Motivo Reagendamento", "Motivo", 8, Models.Grid.Align.center, false, false);
                grid.AdicionarCabecalho("Data Emissão CT-e", "DataEmissaoCTeFormatado", 10, Models.Grid.Align.center, false, true);
                grid.AdicionarCabecalho("QTD. Não Comparecimento", "QuantidadeNaoComparecimento", 10, Models.Grid.Align.center, false, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "JanelaDescarga/Pesquisa", "grid-tabela-descarregamento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> situacoesAdicionais = new List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao>();
                List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento = new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga> listaCargaJanelaDescarregamentoRetornar = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga>();
                int totalRegistros = 0;

                if (centroDescarregamento != null)
                {
                    List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = (
                        from o in grid.header
                        where o.visible
                        select new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento()
                        {
                            Propriedade = o.name
                        }
                    ).ToList();

                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "Codigo" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoCarga" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoPedido" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoSituacaoCadastrada" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CPFCNPJDestinatario" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "Destinatario" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "Situacao" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "SituacaoCarga" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "PermiteInformarAcaoParcial" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoAgendamentoColeta" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoAgendamentoPallet" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoAgendamentoColetaPedido" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "AgendaExtra" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "TipoAcaoParcial" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "QuantidadeArquivoIntegracao" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "ObservacaoFluxoPatio" });
                    propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "ExibirJanelaDescargaPorPedido" });

                    if (configuracaoEmbarcador.PossuiMonitoramento)
                        propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "CodigoMonitoramento" });

                    if (!propriedades.Any(o => o.Propriedade == "InicioDescarregamento"))
                        propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "InicioDescarregamento" });

                    if (!propriedades.Any(o => o.Propriedade == "TerminoDescarregamento"))
                        propriedades.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento() { Propriedade = "TerminoDescarregamento" });

                    Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento repositorioCargaJanelaDescarregamento = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamento(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao repositorioCargaJanelaDescarregamentoSituacao = new Repositorio.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao(unitOfWork);
                    totalRegistros = repositorioCargaJanelaDescarregamento.ContarConsulta(filtrosPesquisa, propriedades);

                    if (totalRegistros > 0)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                        listaCargaJanelaDescarregamentoRetornar = repositorioCargaJanelaDescarregamento.Consultar(filtrosPesquisa, propriedades, parametrosConsulta);
                        situacoesAdicionais = repositorioCargaJanelaDescarregamentoSituacao.BuscarTodos();
                        periodosDescarregamento = filtrosPesquisa.UtilizarDadosDosPedidos ? new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>() : ObterPeriodosDescarregamentoPorCentro(centroDescarregamento, filtrosPesquisa.DataDescarregamento, unitOfWork);
                    }
                }

                foreach (Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga retornoJanelaDescarga in listaCargaJanelaDescarregamentoRetornar)
                {
                    retornoJanelaDescarga.DT_RowColor = ObterCorLinha(retornoJanelaDescarga, periodosDescarregamento, situacoesCadastradas, situacoesAdicionais);
                    retornoJanelaDescarga.DT_FontColor = ObterCorFonte(retornoJanelaDescarga, periodosDescarregamento, situacoesCadastradas, situacoesAdicionais);
                }

                grid.AdicionaRows(listaCargaJanelaDescarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> ObterPeriodosDescarregamentoPorCentro(Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento centroDescarregamento, DateTime? data, Repositorio.UnitOfWork unitOfWork)
        {
            if ((centroDescarregamento == null) || (data == DateTime.MinValue))
                return new List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento>();

            Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento repositorioExcecaoCapacidadeDescarregamento = new Repositorio.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.PeriodoDescarregamento repositorioPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
            Dominio.Entidades.Embarcador.Logistica.ExcecaoCapacidadeDescarregamento excecaoCapacidadeDescarregamento = repositorioExcecaoCapacidadeDescarregamento.BuscarPorCentroDescarregamento(centroDescarregamento.Codigo, data.Value.Date, data.Value.Date);
            List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento;

            if (excecaoCapacidadeDescarregamento != null)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorExcecao(excecaoCapacidadeDescarregamento.Codigo);
            else if (centroDescarregamento.CapacidadeDescaregamentoPorDia)
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorDiaMes(centroDescarregamento.Codigo, data.Value.Day, data.Value.Month);
            else
                periodosDescarregamento = repositorioPeriodoDescarregamento.BuscarPorCentroDescarregamentoEDia(centroDescarregamento.Codigo, DiaSemanaHelper.ObterDiaSemana(data.Value));

            return periodosDescarregamento;
        }

        private void FinalizarDescarregamento(Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamento cargaJanelaDescarregamento, Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Dominio.Entidades.Embarcador.GestaoPallet.AgendamentoPallet agendamentoPallet, Dominio.Entidades.Embarcador.Cargas.AgendamentoEntrega agendamentoEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Logistica.CargaJanelaDescarregamento servicoCargaJanelaDescarregamento = new Servicos.Embarcador.Logistica.CargaJanelaDescarregamento(unitOfWork, ConfiguracaoEmbarcador, Auditado);

            servicoCargaJanelaDescarregamento.AtualizarSituacao(cargaJanelaDescarregamento, SituacaoCargaJanelaDescarregamento.DescarregamentoFinalizado);

            if (agendamentoColeta != null)
            {
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

                agendamentoColeta.Situacao = SituacaoAgendamentoColeta.Finalizado;
                agendamentoColeta.EtapaAgendamentoColeta = EtapaAgendamentoColeta.Emissao;

                repositorioAgendamentoColeta.Atualizar(agendamentoColeta);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoColeta, null, $"Finalizou o agendamento.", unitOfWork);
            }

            if (agendamentoPallet != null)
            {
                Repositorio.Embarcador.GestaoPallet.AgendamentoPallet repositorioAgendamentoPallet = new Repositorio.Embarcador.GestaoPallet.AgendamentoPallet(unitOfWork);
                Servicos.Embarcador.GestaoPallet.MovimentacaoPallet servicoMovimentacaoPallet = new Servicos.Embarcador.GestaoPallet.MovimentacaoPallet(unitOfWork, Auditado);

                servicoMovimentacaoPallet.AdicionarMovimentacaoAgendamentoPallet(agendamentoPallet);

                agendamentoPallet.Situacao = SituacaoAgendamentoPallet.Finalizado;

                repositorioAgendamentoPallet.Atualizar(agendamentoPallet);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoPallet, null, $"Finalizou o agendamento.", unitOfWork);
            }

            if (agendamentoEntrega != null)
            {
                Repositorio.Embarcador.Cargas.AgendamentoEntrega repositorioAgendamentoEntrega = new Repositorio.Embarcador.Cargas.AgendamentoEntrega(unitOfWork);

                agendamentoEntrega.Situacao = SituacaoAgendamentoEntrega.Finalizado;

                repositorioAgendamentoEntrega.Atualizar(agendamentoEntrega);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, agendamentoEntrega, null, $"Finalizou o agendamento.", unitOfWork);
            }

            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaJanelaDescarregamento, null, $"Descarregamento finalizado.", unitOfWork);
        }

        private string ObterCorLinha(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga retornoJanelaDescarga, List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao> situacoes, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> situacoesAdicionais)
        {
            if (situacoes.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao janelaDescarregamentoSituacao = situacoes.Where(situacao => situacao.Codigo == retornoJanelaDescarga.CodigoSituacaoCadastrada).FirstOrDefault();
                return (janelaDescarregamentoSituacao != null) ? janelaDescarregamentoSituacao.Cor : string.Empty;
            }

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao situacaoAdicional = ObterSituacaoAdicional(retornoJanelaDescarga, periodosDescarregamento, situacoesAdicionais);

            if (situacaoAdicional != null)
                return situacaoAdicional.Cor;

            if (retornoJanelaDescarga.TipoAcaoParcial != 0)
                return retornoJanelaDescarga.TipoAcaoParcial.ObterCorLinha();

            return retornoJanelaDescarga.Situacao.ObterCorLinha();
        }

        private string ObterCorFonte(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga retornoJanelaDescarga, List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao> situacoes, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> situacoesAdicionais)
        {
            if (situacoes.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.JanelaDescarregamentoSituacao janelaDescarregamentoSituacao = situacoes.Where(situacao => situacao.Codigo == retornoJanelaDescarga.CodigoSituacaoCadastrada).FirstOrDefault();
                return (janelaDescarregamentoSituacao != null) ? Utilidades.Cores.ObterCorPorPencentual(janelaDescarregamentoSituacao.Cor, 40m) : "#666";
            }

            Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao situacaoAdicional = ObterSituacaoAdicional(retornoJanelaDescarga, periodosDescarregamento, situacoesAdicionais);

            if (situacaoAdicional != null)
                return Utilidades.Cores.ObterCorPorPencentual(situacaoAdicional.Cor, 40m);

            if (retornoJanelaDescarga.AgendaExtra)
                return "#ff0000";

            if (retornoJanelaDescarga.TipoAcaoParcial != 0)
                return retornoJanelaDescarga.TipoAcaoParcial.ObterCorFonte();

            if (retornoJanelaDescarga.Situacao == SituacaoCargaJanelaDescarregamento.AguardandoGeracaoSenha)
                return "#000000";

            return "#666";
        }

        private Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao ObterSituacaoAdicional(Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoJanelaDescarga retornoJanelaDescarga, List<Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento> periodosDescarregamento, List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> situacoesAdicionais)
        {
            if (situacoesAdicionais.Count == 0)
                return null;

            if ((retornoJanelaDescarga.InicioDescarregamento.Date >= DateTime.Now.Date) && (periodosDescarregamento.Count > 0))
            {
                Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao situacaoForaPeriodo = situacoesAdicionais.Where(o => o.Situacao == SituacaoCargaJanelaDescarregamentoAdicional.ForaPeriodoDescarregamento).FirstOrDefault();

                if (situacaoForaPeriodo != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.PeriodoDescarregamento periodoDescarregamento = (
                        from o in periodosDescarregamento
                        where (retornoJanelaDescarga.InicioDescarregamento.TimeOfDay >= o.HoraInicio && retornoJanelaDescarga.TerminoDescarregamento.TimeOfDay <= o.HoraTermino)
                        select o
                    ).FirstOrDefault();

                    if (periodoDescarregamento == null)
                        return situacaoForaPeriodo;
                }
            }

            return null;
        }

        private void EnviarEmailDesagendamentoCarga(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (agendamentoColeta == null)
                return;

            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailDesagendamento(agendamentoColeta, emails, unitOfWork, cliente);
        }

        private void EnviarEmailAlteracaoHorario(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, DateTime novoHorario, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (agendamentoColeta == null)
                return;

            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailAlteracaoHorario(agendamentoColeta, novoHorario, emails, cliente);
        }

        private void EnviarEmailNaoComparecimento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (agendamentoColeta == null)
                return;

            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailNaoComparecimento(agendamentoColeta, emails, cliente);
        }

        private void EnviarEmailCancelamentoAgendamento(Dominio.Entidades.Embarcador.Logistica.AgendamentoColeta agendamentoColeta, string motivoCancelamento, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente cliente)
        {
            if (agendamentoColeta == null)
                return;


            //alterar consulta para buscar apenas os emails do remetente.. (criar metodo unico no servico)
            Repositorio.Embarcador.Logistica.AgendamentoColetaPedido repositorioAgendamentoColetaPedido = new Repositorio.Embarcador.Logistica.AgendamentoColetaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Logistica.AgendamentoColetaPedido> agendamentoColetaPedido = repositorioAgendamentoColetaPedido.BuscarPorAgendamentoColeta(agendamentoColeta.Codigo);

            if (agendamentoColetaPedido.Count == 0)
                return;

            List<string> emails = new List<string>();

            foreach (string email in agendamentoColetaPedido.Select(o => o.Pedido.Remetente.Email).Distinct())
                emails.Add(email);

            Servicos.Embarcador.Logistica.AgendamentoColeta servicoAgendamentoColeta = new Servicos.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
            servicoAgendamentoColeta.EnviarEmailCancelamentoAgendamento(agendamentoColeta, motivoCancelamento, emails, cliente);
        }

        private Dominio.Entidades.Embarcador.Logistica.CentroDescarregamento BuscarCentroDescarregamentoPorParametro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(unitOfWork);

            double codigoDestinatario = Request.GetDoubleParam("Destinatario");

            return repositorioCentroDescarregamento.BuscarPorDestinatario(codigoDestinatario);
        }

        private Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento PreencherConfiguracaoDisponibilidadeDescarregamento()
        {
            Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento configuracaoDisponibilidadeCarregamento = new Dominio.ObjetosDeValor.Embarcador.JanelaCarregamento.ConfiguracaoDisponibilidadeDescarregamento()
            {
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoRemetente = Request.GetDoubleParam("Remetente"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                configuracaoDisponibilidadeCarregamento.CodigoRemetente = Usuario.Empresa.Codigo;
            else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                configuracaoDisponibilidadeCarregamento.CodigoRemetente = Usuario.Cliente.Codigo;

            return configuracaoDisponibilidadeCarregamento;
        }

        #endregion
    }
}
