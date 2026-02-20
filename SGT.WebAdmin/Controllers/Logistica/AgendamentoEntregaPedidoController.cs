using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AgendamentoEntregaPedido")]
    public class AgendamentoEntregaPedidoController : BaseController
    {
        #region Construtores

        public AgendamentoEntregaPedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = PreencherGrid(ObterGridPesquisa(unitOfWork), unitOfWork, false);

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

        public async Task<IActionResult> AgendarDescarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

                List<int> codigos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos")).Distinct().ToList();
                List<int> codigosCargaEntrega = new();
                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                if (agruparTelaDeAgendamentoPorEntrega)
                {
                    codigosCargaEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCargaEntrega")).Distinct().ToList();
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                }

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repositorioResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);

                int codigoResponsavel = Request.GetIntParam("ResponsavelMotivoReagendamento");
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(codigos, codigosCargaEntrega);
                codigos = pedidos.Select(p => p.Codigo).ToList();
                Dominio.ObjetosDeValor.Embarcador.Logistica.InformacoesAgendamentoEntregaPedido informacoesAgendamentoEntrega = new Dominio.ObjetosDeValor.Embarcador.Logistica.InformacoesAgendamentoEntregaPedido()
                {
                    DataAgendamento = Request.GetNullableDateTimeParam("Data"),
                    Observacoes = Request.GetStringParam("Observacoes"),
                    Pedidos = pedidos,
                    SalvarComDataRetroativa = Request.GetBoolParam("SalvarComDataRetroativa"),
                    Reagendamento = Request.GetBoolParam("Reagendamento"),
                    ObservacaoReagendamento = Request.GetStringParam("ObservacaoReagendamento"),
                    CodigoMotivoReagendamento = Request.GetIntParam("MotivoReagendamento"),
                    ResponsavelMotivoReagendamentoPedidos = codigoResponsavel > 0 && Request.GetBoolParam("Reagendamento") ? repositorioResponsavelAtrasoEntrega.BuscarPorCodigo(codigoResponsavel, false) : null,
                    ExigeSenhaAgendamento = Request.GetBoolParam("ExigeSenhaAgendamento"),
                    SenhaEntregaAgendamento = Request.GetStringParam("SenhaEntregaAgendamento"),
                };

                servicoAgendamentoEntregaPedido.SalvarHorarioAgendamento(informacoesAgendamentoEntrega, Cliente);

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = new Repositorio.Embarcador.Cargas.Carga(unitOfWork).BuscarCargasPorPedidos(codigos);

                if (configuracaoGeral?.PermitirAgendamentoPedidosSemCarga ?? false)
                {
                    Servicos.Embarcador.Carga.Carga serCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);

                    foreach (Dominio.Entidades.Embarcador.Cargas.Carga carga in cargas)
                        serCarga.AdicionarCargaJanelaCarregamento(carga, ConfiguracaoEmbarcador, TipoServicoMultisoftware, unitOfWork);
                }

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta configuracaoAgendamentoColeta = new Repositorio.Embarcador.Configuracoes.ConfiguracaoAgendamentoColeta(unitOfWork).BuscarPrimeiroRegistro();
                if ((configuracaoAgendamentoColeta?.EnviarEmailDeNotificacaoAutomaticamenteAoTransportadorDaCarga ?? false) && configuracaoAgendamentoColeta.ModeloEmail != null)
                {
                    if (cargas.TrueForAll(carga => carga.Empresa != null))
                    {
                        List<string> emailTransportador = cargas.Select(carga => carga.Empresa.Email).Distinct().ToList();

                        List<string> emails = new List<string>();
                        foreach (var email in emailTransportador)
                        {
                            if (email.Contains(";"))
                                emails.AddRange(email.Split(';'));
                            else
                                emails.Add(email);
                        }

                        string corpoEmailAgendamento = servicoAgendamentoEntregaPedido.BuscarCorpoEmailPorModelo(pedidos, configuracaoAgendamentoColeta.ModeloEmail, cargas, unitOfWork);

                        Dominio.ObjetosDeValor.Email.Mensagem mensagem = new Dominio.ObjetosDeValor.Email.Mensagem()
                        {
                            Destinatarios = emails,
                            Assunto = !string.IsNullOrEmpty(configuracaoAgendamentoColeta.ModeloEmail?.Assunto) ? configuracaoAgendamentoColeta.ModeloEmail.Assunto : $"Notificação de Agendamento - Carga(s) {string.Join(", ", cargas.Select(c => c.CodigoCargaEmbarcador))}",
                            Corpo = corpoEmailAgendamento
                        };
                        servicoAgendamentoEntregaPedido.EnviarEmailAgendamento(mensagem);

                        servicoAgendamentoEntregaPedido.GravarDataEnvioEmailNotificacaoTransportador(pedidos, DateTime.Now, unitOfWork, Auditado);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.DataCarregamentoRetroativa)
                    return new JsonpResult(new
                    {
                        Mensagem = excecao.Message,
                        DataRetroativa = true
                    });

                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao agendar a descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AguardandoReagendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigo = Request.GetIntParam("Codigo");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega = new List<int>() { Request.GetIntParam("CodigoCargaEntrega") };

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int> { codigo }, codigosCargaEntrega);
                servicoAgendamentoEntregaPedido.SetarAguardandoReagendamento(pedidos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao agendar a descarga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AguardandoRetornoCliente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int> { codigo }, codigosCargaEntrega);

                if (pedidos.Count == 0)
                    throw new ControllerException("Pedido( não encontrado.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    servicoAgendamentoEntregaPedido.AguardandoRetornoCliente(pedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu um erro ao alterar a situação do agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SolicitarAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                List<int> codigosPedidos = Request.GetListParam<int>("Pedidos");
                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                List<int> codigosCargaEntrega = agruparTelaDeAgendamentoPorEntrega ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCargaEntrega")).Distinct().ToList() : new();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(codigosPedidos, codigosCargaEntrega);

                servicoAgendamentoEntregaPedido.SolicitarAgendamento(pedidos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SolicitarReagendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigoCarga = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork).BuscarPorCarga(codigoCarga);

                if (!pedidos.All(obj => obj.DataAgendamento.HasValue))
                    throw new ControllerException("Para solicitar o reagendamento, é necessário que todos os pedidos da carga já estejam agendados.");

                servicoAgendamentoEntregaPedido.SolicitarReagendamento(Request.GetStringParam("Observacao"), pedidos);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao solicitar o reagendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPeriodosDestinatario()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                List<Dominio.ObjetosDeValor.Embarcador.Logistica.PeriodosAgendamentoEntregaPedido> periodos = servicoAgendamentoEntregaPedido.ObterPeriodosDescargaDestinatario(Request.GetDoubleParam("CPFCnpjDestinatario"));

                return new JsonpResult(new
                {
                    PeriodoDia = (from o in periodos
                                  select new
                                  {
                                      Dia = o.Dia,
                                      Periodos = (from p in o.Periodos
                                                  select new
                                                  {
                                                      p.Inicio,
                                                      p.Fim
                                                  }).ToList()
                                  }).ToList()
                });
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os períodos disponíveis.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarRota()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigo = Request.GetIntParam("Codigo");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int> { codigo }, codigosCargaEntrega);

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    servicoAgendamentoEntregaPedido.SetarRotaPedido(pedido);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao buscar a rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarSituacaoNotaFiscal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoCargaPedido = Request.GetIntParam("CodigoCargaPedido");
                List<int> codigosCargaPedidos = new List<int>() { codigoCargaPedido };
                DateTime data = Request.GetDateTimeParam("Data");
                int codigoNotaFiscalSituacao = Request.GetIntParam("NotaFiscalSituacao");

                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                int codigoEntrega = agruparTelaDeAgendamentoPorEntrega ? Request.GetIntParam("CodigoCargaEntrega") : 0;
                if (codigoEntrega > 0)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(codigoEntrega);
                    codigosCargaPedidos = cargaPedidos.Select(p => p.Codigo).ToList();
                }

                foreach (int codigo in codigosCargaPedidos)
                    servicoAgendamentoEntregaPedido.SalvarSituacaoNotaFiscal(codigo, data, codigoNotaFiscalSituacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a data sugerida para a entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarSugestaoDataEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                DateTime dataSugerida = Request.GetDateTimeParam("DataSugestaoEntrega");

                servicoAgendamentoEntregaPedido.SalvarDataSugestaoEntrega(dataSugerida, codigoCarga);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a data sugerida para a entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarSugestaoReagendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);

                List<int> codigos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("Codigos")).Distinct().ToList();
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                {
                    codigosCargaEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCargaEntrega")).Distinct().ToList();
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                }

                Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega repositorioResponsavelAtrasoEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.TipoResponsavelAtrasoEntrega(unitOfWork);

                int codigoResponsavel = Request.GetIntParam("ResponsavelMotivoReagendamento");

                Dominio.ObjetosDeValor.Embarcador.Logistica.InformacoesAgendamentoEntregaPedido informacoesAgendamentoEntrega = new Dominio.ObjetosDeValor.Embarcador.Logistica.InformacoesAgendamentoEntregaPedido();
                informacoesAgendamentoEntrega.DataAgendamento = Request.GetNullableDateTimeParam("Data");
                informacoesAgendamentoEntrega.Observacoes = Request.GetStringParam("Observacoes");
                informacoesAgendamentoEntrega.Pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(codigos, codigosCargaEntrega);
                informacoesAgendamentoEntrega.SalvarComDataRetroativa = Request.GetBoolParam("SalvarComDataRetroativa");
                informacoesAgendamentoEntrega.Reagendamento = Request.GetBoolParam("Reagendamento");
                informacoesAgendamentoEntrega.ObservacaoReagendamento = Request.GetStringParam("ObservacaoReagendamento");
                informacoesAgendamentoEntrega.CodigoMotivoReagendamento = Request.GetIntParam("MotivoReagendamento");
                informacoesAgendamentoEntrega.ResponsavelMotivoReagendamentoPedidos = codigoResponsavel > 0 && informacoesAgendamentoEntrega.Reagendamento ? repositorioResponsavelAtrasoEntrega.BuscarPorCodigo(codigoResponsavel, false) : null;

                servicoAgendamentoEntregaPedido.SugerirDataReagendamento(informacoesAgendamentoEntrega);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a data sugerida para a entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarInformacaoContatos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                List<int> codigosPedidos = Request.GetListParam<int>("Codigos");
                string informacaoContato = Request.GetStringParam("InformacaoContato");

                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                List<int> codigosCargaEntrega = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("CodigosCargaEntrega")).Distinct().ToList();
                if (agruparTelaDeAgendamentoPorEntrega && codigosCargaEntrega.Count > 0)
                {
                    Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(codigosPedidos, codigosCargaEntrega);
                    codigosPedidos = pedidos.Select(p => p.Codigo).ToList();
                }

                if (codigosPedidos?.Count == 0)
                    throw new ControllerException("Selecione ao menos 1 registro.");

                if (string.IsNullOrWhiteSpace(informacaoContato))
                    throw new ControllerException("Informe uma descrição.");

                repositorioPedido.AdicionarInformacaoContatoCliente(codigosPedidos, this.Usuario.Nome, informacaoContato);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar as informações.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AssumirAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoPedido = Request.GetIntParam("Pedido");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { codigoPedido }, codigosCargaEntrega);

                if (pedidos.Count == 0)
                    throw new ServicoException("Pedido não encontrado para assumir agendamento.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    servicoAgendamentoEntregaPedido.AssumirAgendamento(pedido, Usuario, DateTime.Now);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarDadosAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoPedido = Request.GetIntParam("Pedido");
                string nomeResponsavel = Request.GetStringParam("NomeResponsavel");
                string senha = Request.GetStringParam("SenhaAgendamento");
                DateTime data = Request.GetDateTimeParam("DataAgendamento");
                string protocolo = Request.GetStringParam("ProtocoloAgendamento");

                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { codigoPedido }, codigosCargaEntrega);

                if (pedidos.Count == 0)
                    throw new ServicoException("Pedido não encontrado para informar os dados do atendimento.");

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                    servicoAgendamentoEntregaPedido.InformarDadosAgendamento(pedido, nomeResponsavel, senha, data, protocolo);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailAgendamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoPedido = Request.GetIntParam("codigoPedido");
                List<int> codigosPedidos = new List<int>() { codigoPedido };
                int codigoCarga = Request.GetIntParam("CodigoCarga");
                List<int> codigosCargas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.GetStringParam("CodigosCargas")).Distinct().ToList();
                if (codigosCargas.Count == 0)
                    codigosCargas = new List<int>() { codigoCarga };

                double codigoCliente = Request.GetDoubleParam("CPFCNPJCliente");
                bool enviarEmailParaTodosOsPedidosDoMesmoDestinoECarga = Request.GetBoolParam("EnviarEmailParaTodosOsPedidosDoMesmoDestinoECarga");
                bool multiplasCargas = Request.GetBoolParam("MultiplasCargas");
                int codigoModeloEmail = Request.GetIntParam("ModeloEmail");

                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                int codigoEntrega = agruparTelaDeAgendamentoPorEntrega ? Request.GetIntParam("CodigoCargaEntrega") : 0;
                if (!multiplasCargas && codigoEntrega > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDaEntrega = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { }, new List<int>() { codigoEntrega });
                    codigosPedidos = pedidosDaEntrega.Select(p => p.Codigo).ToList();
                }

                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = repCarga.BuscarPorCodigos(codigosCargas);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPorCargasFetchBasico(codigosCargas).Select(cp => cp.Pedido).ToList();

                if (enviarEmailParaTodosOsPedidosDoMesmoDestinoECarga || multiplasCargas)
                    pedidos = pedidos.Where(pedido => pedido.Destinatario.Codigo == codigoCliente).ToList();
                else
                    pedidos = pedidos.Where(pedido => codigosPedidos.Contains(pedido.Codigo)).ToList();

                Dominio.Entidades.Cliente cliente = new Repositorio.Cliente(unitOfWork).BuscarPorCPFCNPJ(codigoCliente);

                if (pedidos == null || pedidos.Count == 0)
                    throw new ServicoException("Pedido não encontrado.");

                if (cliente == null)
                    throw new ServicoException("Cliente não encontrado.");

                List<string> emailsCliente = cliente.Emails.Where(o => o.TipoEmail == TipoEmail.Agendamento).Select(o => o.Email).ToList();

                if (emailsCliente.Count == 0)
                    throw new ServicoException("Cliente não possui e-mail do tipo Agendamento cadastrado.");

                List<string> emails = new List<string>();
                foreach (var email in emailsCliente)
                {
                    if (email.Contains(";"))
                        emails.AddRange(email.Split(';'));
                    else
                        emails.Add(email);
                }
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail = repConfiguracaoModeloEmail.BuscarPorCodigo(codigoModeloEmail, false);

                string corpoEmailAgendamento = servicoAgendamentoEntregaPedido.BuscarCorpoEmailPorModelo(pedidos, modeloEmail, cargas, unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = pedidos[0];
                Dominio.ObjetosDeValor.Email.Mensagem mensagem = new Dominio.ObjetosDeValor.Email.Mensagem()
                {
                    Destinatarios = emails,
                    Assunto = !string.IsNullOrEmpty(modeloEmail?.Assunto) ? modeloEmail.Assunto : $"Agendamento Pedido " + pedido.NumeroPedidoEmbarcador,
                    Corpo = corpoEmailAgendamento
                };

                servicoAgendamentoEntregaPedido.EnviarEmailAgendamento(mensagem);

                DateTime dataEnvioEmail = DateTime.Now;
                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido ped in pedidos)
                {
                    ped.DataeHoraEnvioEmailAgendamento = dataEnvioEmail;
                    repositorioPedido.Atualizar(ped);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, ped, $"Envio de email de Agendamento" + (enviarEmailParaTodosOsPedidosDoMesmoDestinoECarga ? " (Enviou para todos do mesmo Destino/Carga)" : ""), unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = PreencherGrid(ObterGridPesquisa(unitOfWork), unitOfWork, true);

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

        public async Task<IActionResult> BuscarOpcoesModeloEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);

                List<dynamic> listaOpcoesModeloEmail = new List<dynamic>();

                List<Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail> modelosEmail = repConfiguracaoModeloEmail.BuscarPorTipoModeloEmail(TipoModeloEmail.AgendamentoEntrega);
                foreach (Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modelo in modelosEmail)
                    listaOpcoesModeloEmail.Add(new { text = modelo.Descricao, value = modelo.Codigo });

                listaOpcoesModeloEmail.Add(new { text = "Modelo Padrão", value = 0 }); //Sempre retorna opção Padrão.
                return new JsonpResult(listaOpcoesModeloEmail);

            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarHistoricoAgendamentoEntregaPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoPedido = Request.GetIntParam("CodigoPedido");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { codigoPedido }, codigosCargaEntrega);

                Repositorio.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido repositorioHistoricoAgendamentoEntregaPedido = new Repositorio.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Pedido", "Pedido", 8, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Ação", "Tipo", 12, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data da Ação", "DataHoraRegistro", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Data da Agenda", "DataHoraAgenda", 10, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Observação", "Observacao", 30, Models.Grid.Align.left, false, false, false, false, true);
                grid.AdicionarCabecalho("Motivo", "MotivoReagendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Responsável", "Responsavel", 10, Models.Grid.Align.center, false, false, false, false, true);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false, false, false, false, true);

                List<Dominio.Entidades.Embarcador.Logistica.HistoricoAgendamentoEntregaPedido> historicos = repositorioHistoricoAgendamentoEntregaPedido.BuscarPorPedidos(pedidos.Select(p => p.Codigo).ToList());
                dynamic retorno = (from his in historicos
                                   select new
                                   {
                                       Codigo = his.Codigo,
                                       Tipo = his.Tipo.ObterDescricao(),
                                       DataHoraRegistro = his.DataHoraRegistro.ToString(),
                                       DataHoraAgenda = his.DataHoraAgenda?.ToString() ?? string.Empty,
                                       Observacao = his.Observacao ?? string.Empty,
                                       MotivoReagendamento = his.MotivoReagendamento?.Descricao ?? string.Empty,
                                       Responsavel = his.ResponsavelMotivoReagendamento?.Descricao ?? string.Empty,
                                       Pedido = his.Pedido?.Descricao ?? string.Empty,
                                       Usuario = his.Usuario?.Descricao ?? string.Empty,
                                   }
                    ).ToList();
                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(retorno.Count);
                return new JsonpResult(grid);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarEmailNotificacaoTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail repConfiguracaoModeloEmail = new Repositorio.Embarcador.Configuracoes.ConfiguracaoModeloEmail(unitOfWork);
                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoCarga = Request.GetIntParam("CodigoCarga");
                int codigoModeloEmail = Request.GetIntParam("ModeloEmail");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigoCarga);
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = repCargaPedido.BuscarPedidosPorCarga(codigoCarga);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoModeloEmail modeloEmail = repConfiguracaoModeloEmail.BuscarPorCodigo(codigoModeloEmail, false);
                Dominio.Entidades.Empresa transportador = carga.Empresa;

                bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
                int codigoEntrega = agruparTelaDeAgendamentoPorEntrega ? Request.GetIntParam("CodigoCargaEntrega") : 0;
                if (codigoEntrega > 0)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosDaEntrega = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { }, new List<int>() { codigoEntrega });
                    List<int> codigosPedidos = pedidosDaEntrega.Select(p => p.Codigo).ToList();
                    pedidos = pedidos.Where(p => codigosPedidos.Contains(p.Codigo)).ToList();
                }

                if (pedidos == null || pedidos.Count == 0)
                    throw new ServicoException("Pedido não encontrado.");

                if (transportador == null)
                    throw new ServicoException("Transportador da carga não encontrado.");

                if (string.IsNullOrEmpty(transportador.Email))
                    throw new ServicoException("Cliente não possui e-mail do tipo Agendamento cadastrado.");

                if (modeloEmail == null || string.IsNullOrEmpty(modeloEmail.Corpo))
                    throw new ServicoException("Modelo de Email não encontrado ou com corpo inválido, verifique o cadastro de layout do modelo de email.");

                List<string> emailTransportador = new List<string>() { transportador.Email };
                List<string> emails = new List<string>();
                foreach (var email in emailTransportador)
                {
                    if (email.Contains(";"))
                        emails.AddRange(email.Split(';'));
                    else
                        emails.Add(email);
                }

                unitOfWork.Start();

                string corpoEmailAgendamento = servicoAgendamentoEntregaPedido.BuscarCorpoEmailPorModelo(pedidos, modeloEmail, new List<Dominio.Entidades.Embarcador.Cargas.Carga>() { carga }, unitOfWork);
                string assunto = !string.IsNullOrEmpty(modeloEmail.Assunto) ? modeloEmail.Assunto : $"Notificação de Agendamento - Carga {carga.CodigoCargaEmbarcador}";

                Dominio.ObjetosDeValor.Email.Mensagem mensagem = new Dominio.ObjetosDeValor.Email.Mensagem()
                {
                    Destinatarios = emails,
                    Assunto = assunto,
                    Corpo = corpoEmailAgendamento
                };
                servicoAgendamentoEntregaPedido.EnviarEmailAgendamento(mensagem);

                servicoAgendamentoEntregaPedido.GravarDataEnvioEmailNotificacaoTransportador(pedidos, DateTime.Now, unitOfWork, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GravarSenhaAgendamentoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                Servicos.Embarcador.Logistica.AgendamentoEntregaPedido servicoAgendamentoEntregaPedido = new Servicos.Embarcador.Logistica.AgendamentoEntregaPedido(unitOfWork, ConfiguracaoEmbarcador, TipoServicoMultisoftware, Auditado, Usuario);

                int codigoPedido = Request.GetIntParam("CodigoPedido");
                List<int> codigosCargaEntrega = new();
                if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                    codigosCargaEntrega.Add(Request.GetIntParam("CodigoCargaEntrega"));

                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = servicoAgendamentoEntregaPedido.ObterPedidosAgendamento(new List<int>() { codigoPedido }, codigosCargaEntrega);

                if (pedidos.Count == 0)
                    throw new ServicoException("Pedido não encontrado para gravar a senha de agendamento.");

                string senhaEntregaAgendamento = Request.GetStringParam("SenhaEntregaAgendamento");

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarPorPedidos(pedidos.Select(obj => obj.Codigo).ToList());
                if (cargasEntrega.Count == 0)
                    throw new ServicoException("Entrega não encontrado para gravar a senha de agendamento.");

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
                {
                    cargaEntrega.SenhaEntrega = senhaEntregaAgendamento;
                    await repositorioCargaEntrega.AtualizarAsync(cargaEntrega);
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
                {
                    pedido.SenhaAgendamento = senhaEntregaAgendamento;
                    await repPedido.AtualizarAsync(pedido);

                }

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o agendamento.");
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
            grid.AdicionarCabecalho("CodigoCargaPedido", false);
            grid.AdicionarCabecalho("CodigoPedido", false);
            grid.AdicionarCabecalho("CodigoCarga", false);
            grid.AdicionarCabecalho("SituacaoAgendamento", false);
            grid.AdicionarCabecalho("SituacaoViagem", false);
            grid.AdicionarCabecalho("CPFCNPJCliente", false);
            grid.AdicionarCabecalho("PermiteAgendarEntrega", false);
            grid.AdicionarCabecalho("CodigoTransportador", false);
            grid.AdicionarCabecalho("UltimaConsultaTransportador", false);
            grid.AdicionarCabecalho("PermitirAgendarDescargaAposDataEntregaSugerida", false);
            grid.AdicionarCabecalho("CodigoCargaEntrega", false);
            grid.AdicionarCabecalho("NF-e", "NFe", 15, Models.Grid.Align.left, false, false, false, false, true);

            if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                grid.AdicionarCabecalho("Status Recebimento NF-e", "StatusRecebimentoNotaFiscalAgrupado", 15, Models.Grid.Align.center, false, false, false, false, true);
            else
                grid.AdicionarCabecalho("Status Recebimento NF-e", "StatusRecebimentoNotaFiscal", 15, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação do Agendamento", "SituacaoAgendamentoDescricao", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Reagendamento", "ObservacaoReagendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Cliente", "ClienteDecricao", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", 10, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Sugestão de Entrega", "DataSugestaoEntregaFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Agendamento Entrega", "DescricaoTipoAgendamentoEntrega", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Início de Carregamento", "DataCarregamentoInicialFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Término de Carregamento", "DataCarregamentoFinalFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data de Agendamento", "DataAgendamentoFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação da Viagem", "SituacaoViagemDescricao", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão de Entrega", "DataPrevisaoEntregaFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);

            if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                grid.AdicionarCabecalho("Data Criação Pedido", "DataCriacaoPedidoAgrupado", 15, Models.Grid.Align.center, false, false, false, false, true);
            else
                grid.AdicionarCabecalho("Data Criação Pedido", "DataCriacaoPedidoFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);

            grid.AdicionarCabecalho("Exige Agendamento", "DescricaoExigeAgendamento", 10, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Qtd. Volumes", "QuantidadeVolumes", 10, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Qtd. M³", "QuantidadeMetrosCubicosFormatado", 10, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação NF-e", "SituacaoNotaFiscal", 10, Models.Grid.Align.left, false, false, false, false, true);

            if (ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false)
                grid.AdicionarCabecalho("Observação do Pedido", "ObservacaoPedidoAgrupado", 10, Models.Grid.Align.left, false, false, false, false, true);
            else
                grid.AdicionarCabecalho("Observação do Pedido", "ObservacaoPedido", 10, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho("Data/Hora Lib Agendamento", "DataLiberadoAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Usuário Agendamento", "UsuarioAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data/Hora Usuario Assumiu Agendamento", "DataUsuarioAssumiuAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Previsão Entrega", "DataPrevisaoEntrega", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Forma Agendamento", "FormaAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Telefone Cliente", "TelefoneCliente", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Link Agendamento", "LinkAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("E-mail Agendamento", "EmailAgendamento", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Tela Agendamento", "DataTelaAgendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Número pedido cliente", "NumeroPedidoEmbarcador", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Número Ordem Pedido", "NumeroOrdemPedido", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data previsão saída", "DataPrevisaoSaidaFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tempo de agendamento", "TempoAgendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tempo de Percurso", "TempoPercurso", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tempo para chegar na entrega", "TempoChegarNaEntrega", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Enviou Email Agendamento?", "IconeEmailEnviado", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Envio Email Agendamento", "DataeHoraEnvioEmailAgendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Criação Carga", "DataCriacaoCargaFormatada", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Sugestão Reagendamento", "DataHoraSugestaoReagendamentoFormatado", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Senha do Agendamento", "SenhaEntregaAgendamento", 15, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("ExigeSenhaAgendamento", false);

            Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "AgendamentoEntregaPedido/Pesquisa", "grid-agendamento-entrega");
            grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

            return grid;
        }

        private Models.Grid.Grid PreencherGrid(Models.Grid.Grid grid, Repositorio.UnitOfWork unitOfWork, bool exportar)
        {
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
            parametrosConsulta.PropriedadeOrdenar = "";

            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            bool agruparTelaDeAgendamentoPorEntrega = ConfiguracaoAgendamentoEntrega?.VisualizarTelaDeAgendamentoPorEntrega ?? false;
            int totalRegistros = agruparTelaDeAgendamentoPorEntrega ?
                repositorioCargaPedido.ContarConsultaAgendamentoEntregaAgrupado(filtrosPesquisa) :
                repositorioCargaPedido.ContarConsultaAgendamentoEntrega(filtrosPesquisa);

            if (exportar)
                parametrosConsulta.LimiteRegistros = totalRegistros;

            IList<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega> listaGrid = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.RetornoPesquisaAgendamentoEntrega>();
            if (totalRegistros > 0)
            {
                if (agruparTelaDeAgendamentoPorEntrega)
                    listaGrid = repositorioCargaPedido.ConsultaAgendamentoEntregaAgrupado(filtrosPesquisa, parametrosConsulta);
                else
                    listaGrid = repositorioCargaPedido.ConsultaAgendamentoEntrega(filtrosPesquisa, parametrosConsulta);
            }

            grid.setarQuantidadeTotal(totalRegistros);
            grid.AdicionaRows(listaGrid);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaAgendamentoEntrega()
            {
                Carga = Request.GetStringParam("Carga"),
                CodigosClientes = Request.GetListParam<double>("Cliente"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigosTransportadores = Request.GetListParam<int>("Transportador"),
                DataAgendamentoInicial = Request.GetNullableDateTimeParam("DataAgendamentoInicial"),
                DataAgendamentoFinal = Request.GetNullableDateTimeParam("DataAgendamentoFinal"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                SituacoesAgendamento = Request.GetListEnumParam<SituacaoAgendamentoEntregaPedido>("SituacaoAgendamento"),
                NFe = Request.GetIntParam("NFe"),
                DataPrevisaoEntregaInicial = Request.GetNullableDateTimeParam("DataPrevisaoEntregaInicial"),
                DataPrevisaoEntregaFinal = Request.GetNullableDateTimeParam("DataPrevisaoEntregaFinal"),
                DataCriacaoPedidoInicial = Request.GetNullableDateTimeParam("DataCriacaoPedidoInicial"),
                DataCriacaoPedidoFinal = Request.GetNullableDateTimeParam("DataCriacaoPedidoFinal"),
                PossuiDataTerminoCarregamento = Request.GetNullableBoolParam("PossuiDataTermioCarregamento"),
                PossuiDataSugestaoEntrega = Request.GetNullableBoolParam("PossuiDataSugestaoEntrega"),
                PossuiNotaFiscalVinculada = Request.GetNullableEnumParam<SimNao>("PossuiNotaFiscalVinculada"),
                DataInicialSugestaoEntrega = Request.GetNullableDateTimeParam("DataInicialSugestaoEntrega"),
                DataFinalSugestaoEntrega = Request.GetNullableDateTimeParam("DataFinalSugestaoEntrega"),
                CodigosSituacaoNotaFiscal = Request.GetListParam<int>("SituacaoNotaFiscal"),
                SomenteCargasFinalizadas = Request.GetBoolParam("StatusCarga"),
                CanalEntrega = Request.GetNullableListParam<int>("CanalEntrega"),
                ObrigaCarga = !configuracaoGeral.PermitirAgendamentoPedidosSemCarga,
                NumeroOrdem = Request.GetStringParam("NumeroOrdem"),
                DataInicialCriacaoDaCarga = Request.GetNullableDateTimeParam("DataInicialCriacaoDaCarga"),
                DataFinalCriacaoDaCarga = Request.GetNullableDateTimeParam("DataFinalCriacaoDaCarga"),
                SenhaEntregaAgendamento = Request.GetStringParam("SenhaEntregaAgendamento"),
                EntegasComSenhaDeAgendamento = Request.GetNullableBoolParam("EntegasComSenhaDeAgendamento"),
                SiglasUFDestino = Request.GetListParam<string>("UFDestino"),
            };
        }
        #endregion
    }
}