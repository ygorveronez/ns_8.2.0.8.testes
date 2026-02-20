using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Importacao;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/FilaCarregamento")]
    public class FilaCarregamentoController : BaseController
    {
        #region Construtores

        public FilaCarregamentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> AceitarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AceitarCargaManualmente(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao aceitar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AceitarPreCarga(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aceitar o pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AceitarSolicitacaoSaida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                string justificativa = Request.GetStringParam("Justificativa");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AceitarSaida(codigoFilaCarregamentoVeiculo, justificativa, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao aceitar a saída da fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFila()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                DateTime? dataProgramada = Request.GetNullableDateTimeParam("DataProgramada");

                if (dataProgramada.HasValue && (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe))
                {
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();

                    if (configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo > 0)
                    {
                        DateTime dataLimiteAdicionarFilaCarregamentoVeiculo = DateTime.Now.AddDays(configuracaoPreCarga.DiasParaTransportadorAdicionarFilaCarregamentoVeiculo);

                        if (dataProgramada.Value > dataLimiteAdicionarFilaCarregamentoVeiculo)
                            throw new ControllerException($"A data de previsão de chegada informada excede o limite configurado ({dataLimiteAdicionarFilaCarregamentoVeiculo.ToDateTimeString()})");
                    }
                }

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    CodigoTipoRetornoCarga = Request.GetIntParam("Tipo"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    CodigosDestino = Request.GetListParam<int>("Destinos"),
                    CodigosRegiaoDestino = Request.GetListParam<int>("RegioesDestino"),
                    CodigosTipoCarga = Request.GetListParam<int>("TiposCarga"),
                    DataProgramada = Request.GetNullableDateTimeParam("DataProgramada"),
                    SiglasEstadoDestino = Request.GetListParam<string>("EstadosDestino"),
                    CodigoAreaVeiculo = Request.GetIntParam("CodigoAreaVeiculo"),
                    CodigoEquipamento = Request.GetIntParam("Equipamento")
                };

                servicoFilaCarregamentoVeiculo.Adicionar(filaCarregamentoVeiculoAdicionar, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro na fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFilaEmTransicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar filaCarregamentoVeiculoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoVeiculoAdicionar()
                {
                    CodigoMotorista = Request.GetIntParam("Motorista"),
                    CodigoTipoRetornoCarga = Request.GetIntParam("Tipo"),
                    CodigoVeiculo = Request.GetIntParam("Veiculo"),
                    EmTransicao = true
                };

                servicoFilaCarregamentoVeiculo.Adicionar(filaCarregamentoVeiculoAdicionar, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro a fila Em Transição.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFilaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
            int codigoMotorista = Request.GetIntParam("Motorista");
            int codigoVeiculo = Request.GetIntParam("Veiculo");

            try
            {
                Servicos.Embarcador.Logistica.FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new Servicos.Embarcador.Logistica.FilaCarregamentoMotorista(unitOfWork, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoMotorista.Adicionar(codigoMotorista, codigoCentroCarregamento, codigoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlocarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                int codigoCarga = Request.GetIntParam("Carga");
                string observacao = Request.GetStringParam("Observacao");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());
                Servicos.Embarcador.Carga.HistoricoVinculo serHistorico = new Servicos.Embarcador.Carga.HistoricoVinculo(unitOfWork);

                servicoFilaCarregamentoVeiculo.AlocarCargaManualmente(codigoFilaCarregamentoVeiculo, codigoCarga, observacao, TipoServicoMultisoftware);

                try
                {
                    Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigoFilaCarregamentoVeiculo) ?? throw new ServicoException("Fila de carregamento não encontrada.");

                    string erros = string.Empty;
                    serHistorico.InserirHistoricoVinculo(unitOfWork, ref erros, LocalVinculo.FilaCarregamento, carga.Veiculo, carga.VeiculosVinculados, carga.Motoristas, DateTime.Now, null, carga.Pedidos?.FirstOrDefault()?.Pedido, carga, filaCarregamentoVeiculo);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alocar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarPrimeiraPosicaoFila()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/FilaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FilaCarregamento_PermitirAlterarPrimeiraPosicao))
                    throw new ControllerException("Você não possui permissão para alterar a posição na fila.");

                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AlterarPrimeiraPosicao(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a posição na fila.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarCentroCarregamento()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamento = Request.GetIntParam("Codigo");
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AlterarCentroCarregamento(codigoFilaCarregamento, codigoCentroCarregamento, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao alterar o centro de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarSituacaoFila()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoFilaCarregamento = Request.GetIntParam("Codigo");
                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoMotivo = Request.GetIntParam("Motivo");
                string observacao = Request.GetStringParam("Observacao");
                SituacaoFilaCarregamentoVeiculo situacao = Request.GetEnumParam<SituacaoFilaCarregamentoVeiculo>("Situacao");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AlterarSituacao(codigoFilaCarregamento, codigoCentroCarregamento, situacao, codigoMotivo, TipoServicoMultisoftware, observacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a situação da fila.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AlterarUltimaPosicaoFila()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/FilaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FilaCarregamento_PermitirAlterarUltimaPosicao))
                    throw new ControllerException("Você não possui permissão para alterar a posição na fila.");

                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.AlterarUltimaPosicao(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a posição na fila.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarChegadaVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamento.ConfirmarChegadaVeiculo(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao onfirmar a chegada de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesatrelarTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamento.DesatrelarVeiculo(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao onfirmar a chegada de veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("GET")]
        public async Task<IActionResult> DispararNotificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("FilaCarregamentoVeiculo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);

                servicoFilaCarregamentoVeiculo.NotificarAlteracao(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao disparar a notificação de fila de carregamento alterada.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarNotificacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                string mensagem = Request.GetStringParam("Mensagem");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);

                servicoFilaCarregamentoVeiculo.EnviarNotificacao(codigoFilaCarregamentoVeiculo, mensagem);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a notificação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        public async Task<IActionResult> EnviarNotificacaoMotoristaSMS()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                int codigoNotificacao = Request.GetIntParam("Mensagem");


                Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS repNotificacaoMotoristaSMS = new Repositorio.Embarcador.Configuracoes.NotificacaoMotoristaSMS(unitOfWork);
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Configuracoes.NotificacaoMotoristaSMS notificacaoMotoristaSMS = repNotificacaoMotoristaSMS.BuscarPorCodigo(codigoNotificacao);

                if (filaCarregamentoVeiculo.ConjuntoMotorista.Motorista == null)
                    return new JsonpResult(true, false, "Obrigatório informar um motorista");

                var isTerceiro = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);


                Servicos.SMS srvSMS = new Servicos.SMS(unitOfWork);
                string msgErro;
                string cpfMotorista = filaCarregamentoVeiculo?.ConjuntoMotorista?.Motorista?.CPF_Formatado ?? "";
                string nomeMotorista = filaCarregamentoVeiculo.ConjuntoMotorista?.Motorista?.Nome ?? "";
                string numeroCarga = filaCarregamentoVeiculo.Carga?.CodigoCargaEmbarcador ?? "";
                string placaVeiculo = filaCarregamentoVeiculo.ConjuntoVeiculo?.Tracao?.Placa_Formatada ?? "";
                string nomeTransportadora = (isTerceiro)
                                        ? filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Proprietario?.Nome
                                        : filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Empresa?.RazaoSocial;
                string cnpjTransportadora = (isTerceiro)
                                        ? filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Proprietario?.CPF_CNPJ_Formatado
                                        : filaCarregamentoVeiculo.ConjuntoVeiculo.Tracao?.Empresa?.CNPJ_Formatado;
                string doca = filaCarregamentoVeiculo.Carga?.NumeroDoca ?? "";

                string mensagem = notificacaoMotoristaSMS.Mensagem.Replace("#CpfMotorista", cpfMotorista)
                                                                  .Replace("#NomeMotorista", nomeMotorista)
                                                                  .Replace("#NumeroCarga", numeroCarga)
                                                                  .Replace("#PlacaVeiculo", placaVeiculo)
                                                                  .Replace("#NomeTransportadora", nomeTransportadora)
                                                                  .Replace("#CnpjTransportadora", cnpjTransportadora)
                                                                  .Replace("#Doca", doca)
                                                                  .Replace("#DataMensagem", DateTime.Now.ToString());

                if (!srvSMS.EnviarSMS(ConfiguracaoEmbarcador.TokenSMS, ConfiguracaoEmbarcador.SenderSMS, filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Celular, mensagem, unitOfWork, out msgErro))
                {
                    return new JsonpResult(false, "Falha ao enviar SMS");
                }

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao enviar a notificação.");
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

        public async Task<IActionResult> InformarConjuntoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                int codigoMotorista = Request.GetIntParam("Motorista");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.InformarMotorista(codigoFilaCarregamentoVeiculo, codigoMotorista, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao informar o conjunto do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarEquipamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                int codigoEquipamento = Request.GetIntParam("Equipamento");

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

                bool equipamentoEmUso = repositorioFilaCarregamentoVeiculo.ExisteEquipamentoEmUsoFilaCarregamento(codigoEquipamento);

                if (equipamentoEmUso)
                    return new JsonpResult(false, true, "O equipamento já está sendo utilizado em outro veículo");

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.InserirEquipamento(codigoFilaCarregamentoVeiculo, codigoEquipamento, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao informar o equipamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarConjuntoVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                int codigoTracao = Request.GetIntParam("Tracao");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.InformarTracao(codigoFilaCarregamentoVeiculo, codigoTracao, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao informar o conjunto do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Liberar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamento.Liberar(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao liberar.");
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
                Dominio.Entidades.Empresa transportador = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) ? Usuario.Empresa : null;

                return new JsonpResult(new
                {
                    Legendas = servicoConfiguracaoLegenda.ObterPorGrupoCodigoControle(GrupoCodigoControleLegenda.FilaCarregamento),
                    Transportador = new { Codigo = transportador?.Codigo ?? 0, Descricao = transportador?.Descricao ?? "" }
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

        public async Task<IActionResult> ObterDadosResumo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();

                int codigoCentroCarregamento = Request.GetIntParam("CentroCarregamento");
                int codigoFilial = Request.GetIntParam("Filial");
                int codigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular");
                bool exibirResumoSomentePorModeloVeicularCarga = configuracao?.ExibirResumoFilaCarregamentoSomentePorModeloVeicularCarga ?? false;
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Logistica.GraficoFilaCarregamentoResumo> listaGraficoFilaCarregamentoResumo = repositorioFilaCarregamentoVeiculo.BuscarResumo(codigoCentroCarregamento, codigoFilial, codigoGrupoModeloVeicularCarga, exibirResumoSomentePorModeloVeicularCarga);
                List<dynamic> retorno = new List<dynamic>();

                retorno.AddRange((from grafico in listaGraficoFilaCarregamentoResumo select new { Descricao = grafico.DescricaoGrupoOuModeloVeicular, Valor = grafico.TotalRegistros }).ToList());

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os dados do resumo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigo);

                if (filaCarregamentoVeiculo == null)
                    return new JsonpResult(false, true, "Fila de carregamento não encontrada.");

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico repositorioFilaCarregamentoVeiculoHistorico = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico> listaHistorico = repositorioFilaCarregamentoVeiculoHistorico.BuscarPorFilaCarregamentoVeiculo(codigo);

                var detalhes = new
                {
                    filaCarregamentoVeiculo.Codigo,
                    LocalAtual = filaCarregamentoVeiculo.ConjuntoVeiculo.ObterLocalAtual()?.DescricaoAcao ?? "",
                    Historicos = (
                        from historico in listaHistorico
                        select new
                        {
                            DataOrdenar = historico.Data.ToString("yyyyMMddHHmm"),
                            Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                            historico.Descricao,
                            Posicao = historico.ObterPosicao(),
                            Usuario = historico.Usuario?.Descricao,
                            OrigemAlteracao = historico.OrigemAlteracao.ObterDescricao()
                        }
                    ).ToList()
                };

                return new JsonpResult(detalhes);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesMotorista()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPorCodigo(codigo);

                if (filaCarregamentoVeiculo == null)
                    return new JsonpResult(false, true, "Fila de carregamento não encontrada.");

                if (!filaCarregamentoVeiculo.ConjuntoMotorista.IsCompleto())
                    return new JsonpResult(false, true, "Fila de carregamento não possui motorista informado.");

                var detalhesMotorista = new
                {
                    Cpf = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.CPF_Formatado,
                    filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Nome,
                    filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Telefone,
                    Transportador = filaCarregamentoVeiculo.ConjuntoMotorista.Motorista.Empresa?.Descricao
                };

                return new JsonpResult(detalhesMotorista);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes do motorista da fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterDetalhesSolicitacaoSaida()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico servicoHistorico = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico(unitOfWork, Usuario);
                Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoHistorico ultimoHistorico = servicoHistorico.ObterUltimo(codigoFilaCarregamentoVeiculo);

                if (ultimoHistorico == null)
                    return new JsonpResult(false, true, "Fila de carregamento não encontrada.");

                if (ultimoHistorico.FilaCarregamentoVeiculo.Situacao != SituacaoFilaCarregamentoVeiculo.EmRemocao)
                    return new JsonpResult(false, true, "Situação da fila de carregamento não possui solicitação de saída em aberto.");

                var detalhesSolicitacaoSaida = new
                {
                    ultimoHistorico.FilaCarregamentoVeiculo.Codigo,
                    Motivo = ultimoHistorico.MotivoRetiradaFilaCarregamento?.Descricao
                };

                return new JsonpResult(detalhesSolicitacaoSaida);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da solicitação de saída da fila de carregamento.");
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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular"),
                    CodigosCarga = Request.GetListParam<int>("Carga"),
                    CodigosConfiguracaoProgramacaoCarga = Request.GetListParam<int>("ConfiguracaoProgramacaoCarga"),
                    CodigosDestino = Request.GetListParam<int>("Destino"),
                    CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicular"),
                    CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino"),
                    CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                    CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                    CodigosTransportador = Request.GetListParam<int>("Transportador"),
                    CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                    DataProgramadaInicial = Request.GetNullableDateTimeParam("DataProgramadaInicial"),
                    DataProgramadaFinal = Request.GetNullableDateTimeParam("DataProgramadaFinal"),
                    SiglasEstadoDestino = Request.GetListParam<string>("EstadoDestino"),
                };

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                    filtrosPesquisa.CodigoTransportador = new Repositorio.Empresa(unitOfWork).BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato)?.Codigo ?? 0;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                {
                    List<int> lstClienteTerceiro = new List<int>();
                    foreach (var item in new Repositorio.Veiculo(unitOfWork).BuscarPorProprietario((double)(this.Usuario?.ClienteTerceiro?.CPF_CNPJ ?? 0)))
                        lstClienteTerceiro.Add(item.Codigo);

                    filtrosPesquisa.CodigosVeiculo = lstClienteTerceiro;
                }

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoTotalizador filaCarregamentoTotalizador = repositorioFilaCarregamentoVeiculo.BuscarTotalizadorPorSituacao(filtrosPesquisa);

                return new JsonpResult(filaCarregamentoTotalizador ?? new Dominio.ObjetosDeValor.Embarcador.Logistica.FilaCarregamentoTotalizador());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os totalizadores por situação da fila de carregamento.");
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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaEmTransicao()
        {
            try
            {
                if (IsExibirTodasFilasCarregamento())
                    return new JsonpResult(ObterGridPesquisaEmTransicao());

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaFluxoPatio()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaFluxoPatio());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaMotorista()
        {
            try
            {
                if (IsExibirTodasFilasCarregamento())
                    return new JsonpResult(ObterGridPesquisaMotorista());

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPreCarga()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPreCarga());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPreCargaMotorista()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPreCargaMotorista());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> RecusarCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.RecusarCargaManualmente(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao recusar a carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecusarPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.RecusarPreCarga(codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao recusar o pré planejamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RecusarSolicitacaoSaida()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                string justificativa = Request.GetStringParam("Justificativa");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.RecusarSaida(codigoFilaCarregamentoVeiculo, justificativa);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao recusar a saída da fila de carregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverConjuntoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamento = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.RemoverConjuntoMotorista(codigoFilaCarregamento);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o conjunto do motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverFilaCarregamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoMotorista = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoMotorista servicoFilaCarregamentoMotorista = new Servicos.Embarcador.Logistica.FilaCarregamentoMotorista(unitOfWork, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoMotorista.Remover(codigoFilaCarregamentoMotorista);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover o motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverReversa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamento = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamento.RemoverReversa(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao remover o motorista da reversa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverTracao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.RemoverTracao(codigoFilaCarregamentoVeiculo);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a tração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Reposicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/FilaCarregamento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.FilaCarregamento_PermitirReposicionar))
                    throw new ControllerException("Você não possui permissão para alterar a posição na fila.");

                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("Codigo");
                int codigoMotivo = Request.GetIntParam("Motivo");
                int novaPosicao = Request.GetIntParam("Posicao");
                string observacao = Request.GetStringParam("Observacao");

                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.Reposicionar(codigoFilaCarregamentoVeiculo, novaPosicao, codigoMotivo, TipoServicoMultisoftware, observacao);

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar a posição na fila.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VincularFilaCarregamentoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoFilaCarregamentoMotorista = Request.GetIntParam("FilaCarregamentoMotorista");
                int codigoFilaCarregamentoVeiculo = Request.GetIntParam("FilaCarregamentoVeiculo");
                Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, Auditado.Usuario, ObterOrigemAlteracaoFilaCarregamento());

                servicoFilaCarregamentoVeiculo.VincularFilaCarregamentoMotorista(codigoFilaCarregamentoMotorista, codigoFilaCarregamentoVeiculo, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao vincular o motorista.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracaoImportacao()
        {
            List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoFilaCarregamento();

            return new JsonpResult(configuracoes.ToList());
        }

        public async Task<IActionResult> Importar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                List<ConfiguracaoImportacao> configuracoes = ConfiguracaoImportacaoFilaCarregamento();

                dynamic parametro = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Parametro"));

                RetornoImportacao retorno = Servicos.Embarcador.Importacao.Importacao.PreencherImportacaoManual(Request, ((dados) =>
                {
                    Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoImportacao servicoVeiculoImportar = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculoImportacao(unitOfWork, TipoServicoMultisoftware, dados, configuracao, parametro);

                    return servicoVeiculoImportar.ObterFilaCarregamentoVeiculoImportar(TipoServicoMultisoftware);
                }));

                if (retorno == null)
                    return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoImportarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private bool IsExibirTodasFilasCarregamento()
        {
            return (
                (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe) &&
                (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
            );
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento repositorioConfiguracaoFilaCarregamento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento(unitOfWork);
                Repositorio.Embarcador.Configuracoes.Integracao repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFilaCarregamento configuracaoFilaCarregamento = repositorioConfiguracaoFilaCarregamento.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repConfiguracaoIntegracao.Buscar();

                bool permitirOrdenar = configuracaoGeralCarga.UtilizarProgramacaoCarga;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular"),
                    CodigosCarga = Request.GetListParam<int>("Carga"),
                    CodigosConfiguracaoProgramacaoCarga = Request.GetListParam<int>("ConfiguracaoProgramacaoCarga"),
                    CodigosDestino = Request.GetListParam<int>("Destino"),
                    CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicular"),
                    CodigosRegiaoDestino = Request.GetListParam<int>("RegiaoDestino"),
                    CodigosTipoCarga = Request.GetListParam<int>("TipoCarga"),
                    CodigosTipoOperacao = Request.GetListParam<int>("TipoOperacao"),
                    CodigosTransportador = Request.GetListParam<int>("Transportador"),
                    CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                    DataProgramadaInicial = Request.GetNullableDateTimeParam("DataProgramadaInicial"),
                    DataProgramadaFinal = Request.GetNullableDateTimeParam("DataProgramadaFinal"),
                    SiglasEstadoDestino = Request.GetListParam<string>("EstadoDestino"),
                    SituacaoPesquisa = Request.GetListEnumParam<SituacaoFilaCarregamentoVeiculoPesquisa>("Situacao"),
                    Situacoes = SituacaoFilaCarregamentoVeiculoHelper.ObterSituacoesNaFila(),
                    Tipo = Request.GetNullableEnumParam<TipoFilaCarregamentoVeiculo>("Tipo")
                };

                if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                    filtrosPesquisa.Situacoes.Add(SituacaoFilaCarregamentoVeiculo.EmViagem);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    filtrosPesquisa.CodigoTransportador = this.Empresa.Codigo;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Terceiros)
                    filtrosPesquisa.CodigoTransportador = new Repositorio.Empresa(unitOfWork).BuscarPorCNPJ(Usuario.ClienteTerceiro.CPF_CNPJ_SemFormato)?.Codigo ?? 0;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro)
                    filtrosPesquisa.CodigoProprietarioVeiculo = this.Usuario?.ClienteTerceiro?.CPF_CNPJ ?? -1;

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CargaEmbarcador", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CPFMotoristas", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PermiteEnviarNotificacaoSuperApp", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoPreCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoModeloVeicularCarga", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigosTransportador", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTracao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "ConjuntoMotoristaCompleto", visivel: false);
                grid.AdicionarCabecalho(propriedade: "ConjuntoVeiculoCompleto", visivel: false);
                grid.AdicionarCabecalho(propriedade: "EmRemocao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Situacao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "InformarEquipamento", visivel: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 10, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (configuracao?.UtilizarControleHigienizacao ?? false)
                    grid.AdicionarCabecalho(descricao: "Higienizado", propriedade: "Higienizado", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                if (filtrosPesquisa.CodigoTransportador == 0)
                    grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 20, alinhamento: Models.Grid.Align.left, permitirOrdenar);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    grid.AdicionarCabecalho(descricao: "Dedicado", propriedade: "VeiculoDedicado", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data de Previsão de Chegada", propriedade: "DataProgramada", tamanho: 15, alinhamento: Models.Grid.Align.center, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Rastreador", propriedade: "Rastreador", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data da Última Posição", propriedade: "DataUltimaPosicao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Última posição do Veículo", propriedade: "UltimaPosicaoDescricao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                }
                else if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 20, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Configuração de Pré Planejamento", propriedade: "ConfiguracaoProgramacaoCarga", tamanho: 20, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Cidades de Destino", propriedade: "Destinos", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Estados de Destino", propriedade: "EstadosDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Regiões de Destino", propriedade: "RegioesDestino", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Tipos de Carga", propriedade: "TiposCarga", tamanho: 20, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Dedicado", propriedade: "VeiculoDedicado", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Extra", propriedade: "Extra", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Status GR", propriedade: "StatusGr", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Teste Frio", propriedade: "MensagemLicenca", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data de Previsão de Chegada", propriedade: "DataProgramada", tamanho: 15, alinhamento: Models.Grid.Align.center, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Rastreador", propriedade: "Rastreador", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data da Última Posição", propriedade: "DataUltimaPosicao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Número do Pré Planejamento", propriedade: "NumeroPreCarga", tamanho: 10, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Data do Pré Planejamento", propriedade: "DataPrevisaoEntrega", tamanho: 15, alinhamento: Models.Grid.Align.center, permitirOrdenar);
                    grid.AdicionarCabecalho(descricao: "Número da Carga", propriedade: "NumeroCargaEmbarcador", tamanho: 10, alinhamento: Models.Grid.Align.left, permitirOrdenar);
                }

                grid.AdicionarCabecalho(descricao: "Tempo na Fila", propriedade: "TempoFila", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false, visible: !configuracaoGeralCarga.UtilizarProgramacaoCarga);
                grid.AdicionarCabecalho(descricao: "Posição", propriedade: "Posicao", tamanho: 10, alinhamento: Models.Grid.Align.center, permitirOrdenar);

                grid.AdicionarCabecalho(descricao: "Nº Equipamento", propriedade: "Equipamento", tamanho: 10, alinhamento: Models.Grid.Align.left);
                grid.AdicionarCabecalho(descricao: "Área do CD", propriedade: "AreaVeiculo", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Data do Registro", propriedade: "DataEntradaFila", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "FilaCarregamento/Pesquisa", "grid-fila-carregamento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta((propriedadeOrdenar) =>
                {
                    if (propriedadeOrdenar == "ModeloVeicularCarga")
                        return "ConjuntoVeiculo.ModeloVeicularCarga.Descricao";

                    if (propriedadeOrdenar == "Tracao")
                        return "ConjuntoVeiculo.Tracao.Placa";

                    if (propriedadeOrdenar == "Motorista")
                        return "ConjuntoMotorista.Motorista.Nome";

                    if (propriedadeOrdenar == "Filial")
                        return "CentroCarregamento.Filial.Descricao, Filial.Descricao";

                    if (propriedadeOrdenar == "NumeroPreCarga")
                        return "PreCarga.NumeroPreCarga";

                    if (propriedadeOrdenar == "NumeroCargaEmbarcador")
                        return "Carga.CodigoCargaEmbarcador";

                    if (propriedadeOrdenar == "ConfiguracaoProgramacaoCarga")
                        return "PreCarga.ConfiguracaoProgramacaoCarga.Descricao";

                    if (propriedadeOrdenar == "DataPrevisaoEntrega")
                        return "PreCarga.DataPrevisaoEntrega";

                    if (propriedadeOrdenar == "Equipamento")
                        return "Equipamento.Numero";

                    return "Posicao";
                });

                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                Servicos.Embarcador.GestaoPatio.Higienizacao servicoHigienizacao = new Servicos.Embarcador.GestaoPatio.Higienizacao(unitOfWork);
                List<(int CodigoFilaCarregamentoVeiculo, string Destino)> listaDestino = new List<(int CodigoFilaCarregamentoVeiculo, string Destino)>();
                List<(int CodigoFilaCarregamentoVeiculo, string Estado)> listaEstadoDestino = new List<(int CodigoFilaCarregamentoVeiculo, string Estado)>();
                List<(int CodigoFilaCarregamentoVeiculo, string Regiao)> listaRegiaoDestino = new List<(int CodigoFilaCarregamentoVeiculo, string Regiao)>();
                List<(int CodigoFilaCarregamentoVeiculo, string TipoCarga)> listaTipoCarga = new List<(int CodigoFilaCarregamentoVeiculo, string TipoCarga)>();
                List<(int CodigoCarga, string Mensagem)> listaMensagemLicenca = new List<(int CodigoCarga, string Mensagem)>();
                List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador, string UltimaPosicaoDescricao)> listaPosicaoVeiculos = new List<(int CodigoVeiculo, DateTime DataPosicaoAtual, bool Rastreador, string UltimaPosicaoDescricao)>();
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

                if (totalRegistros > 0)
                {
                    listaFilaCarregamentoVeiculo = repositorio.Consultar(filtrosPesquisa, parametrosConsulta);

                    if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                    {
                        Repositorio.Embarcador.Cargas.CargaLicenca repositorioCargaLicenca = new Repositorio.Embarcador.Cargas.CargaLicenca(unitOfWork);
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino repositorioDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoDestino(unitOfWork);
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino repositorioEstadoDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoEstadoDestino(unitOfWork);
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino repositorioRegiaoDestino = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoRegiaoDestino(unitOfWork);
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga repositorioTipoCarga = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculoTipoCarga(unitOfWork);
                        Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                        List<int> codigosFilaCarregamentoVeiculo = listaFilaCarregamentoVeiculo.Select(fila => fila.Codigo).ToList();
                        List<int> codigosCarga = listaFilaCarregamentoVeiculo.Where(fila => fila.Carga != null).Select(fila => fila.Carga.Codigo).ToList();
                        List<int> codigosVeiculo = listaFilaCarregamentoVeiculo.SelectMany(fila => fila.ConjuntoVeiculo.ObterCodigos()).Distinct().ToList();

                        listaDestino = repositorioDestino.BuscarPorFilasCarregamentoVeiculo(codigosFilaCarregamentoVeiculo);
                        listaEstadoDestino = repositorioEstadoDestino.BuscarPorFilasCarregamentoVeiculo(codigosFilaCarregamentoVeiculo);
                        listaRegiaoDestino = repositorioRegiaoDestino.BuscarPorFilasCarregamentoVeiculo(codigosFilaCarregamentoVeiculo);
                        listaTipoCarga = repositorioTipoCarga.BuscarPorFilasCarregamentoVeiculo(codigosFilaCarregamentoVeiculo);
                        listaMensagemLicenca = repositorioCargaLicenca.BuscarMensagemPorCargas(codigosCarga);
                    }
                    else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    {
                        Repositorio.Embarcador.Logistica.PosicaoAtual repositorioPosicaoAtual = new Repositorio.Embarcador.Logistica.PosicaoAtual(unitOfWork);
                        List<int> codigosVeiculo = listaFilaCarregamentoVeiculo.SelectMany(fila => fila.ConjuntoVeiculo.ObterCodigos()).Distinct().ToList();

                        listaPosicaoVeiculos = repositorioPosicaoAtual.BuscarPosicaoPorVeiculos(codigosVeiculo);
                    }
                }

                var isTerceiro = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.TransportadorTerceiro || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoVeiculo
                    select new
                    {
                        fila.Codigo,
                        fila.Situacao,
                        CodigoCarga = fila.Carga?.Codigo ?? 0,
                        CargaEmbarcador = fila.Carga?.CodigoCargaEmbarcador ?? "",
                        CodigoPreCarga = fila.PreCarga?.Codigo ?? 0,
                        CodigoModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga.Codigo,
                        CodigoTracao = fila.ConjuntoVeiculo.Tracao?.Codigo ?? 0,
                        DataEntradaFila = fila.DataEntrada,
                        ConjuntoMotoristaCompleto = fila.ConjuntoMotorista.IsCompleto(),
                        ConjuntoVeiculoCompleto = fila.ConjuntoVeiculo.IsCompleto(),
                        EmRemocao = fila.Situacao == SituacaoFilaCarregamentoVeiculo.EmRemocao,
                        DataProgramada = fila.DataProgramada?.ToDateTimeString() ?? "",
                        Motorista = fila.ConjuntoMotorista.Motorista?.Descricao,
                        CPFMotoristas = fila.ConjuntoMotorista.Motorista?.CPF ?? "",
                        ModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga.Descricao,
                        Equipamento = fila.Equipamento?.Numero?.ToString() ?? "",
                        Transportador = (isTerceiro)
                            ? fila.ConjuntoVeiculo.Tracao?.Proprietario?.Descricao
                            : fila.ConjuntoVeiculo.Tracao?.Empresa?.Descricao
                        ?? string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques where reboque.Empresa != null select reboque.Empresa.Descricao).Distinct().ToList()),
                        CodigosTransportador = ((isTerceiro)
                            ? fila.ConjuntoVeiculo.Tracao?.Proprietario?.Codigo
                            : fila.ConjuntoVeiculo.Tracao?.Empresa?.Codigo)?.ToString()
                        ?? string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques where reboque.Empresa != null select reboque.Empresa.Codigo).Distinct().ToList()),
                        Tracao = fila.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                        TempoFila = (DateTime.Now - fila.DataEntrada).ToTimeString(showSeconds: true),
                        Posicao = fila.ObterPosicao(),
                        Filial = fila.CentroCarregamento?.Filial?.Descricao ?? fila.Filial?.Descricao ?? "",
                        InformarEquipamento = fila.Filial?.InformarEquipamentoFluxoPatio,
                        Higienizado = servicoHigienizacao.IsVeiculosHigienizados(fila.ConjuntoVeiculo) ? "Sim" : "Não",
                        Destinos = string.Join(", ", listaDestino.Where(destino => destino.CodigoFilaCarregamentoVeiculo == fila.Codigo).Select(destino => destino.Destino.Trim())),
                        EstadosDestino = string.Join(", ", listaEstadoDestino.Where(estadoDestino => estadoDestino.CodigoFilaCarregamentoVeiculo == fila.Codigo).Select(estadoDestino => estadoDestino.Estado.Trim())),
                        RegioesDestino = string.Join(", ", listaRegiaoDestino.Where(regiaoDestino => regiaoDestino.CodigoFilaCarregamentoVeiculo == fila.Codigo).Select(regiaoDestino => regiaoDestino.Regiao.Trim())),
                        TiposCarga = string.Join(", ", listaTipoCarga.Where(tipoCarga => tipoCarga.CodigoFilaCarregamentoVeiculo == fila.Codigo).Select(tipoCarga => tipoCarga.TipoCarga)),
                        NumeroPreCarga = fila.PreCarga?.NumeroPreCarga ?? "",
                        ConfiguracaoProgramacaoCarga = fila.PreCarga?.ConfiguracaoProgramacaoCarga?.Descricao ?? "",
                        DataPrevisaoEntrega = fila.PreCarga?.DataPrevisaoEntrega?.ToDateTimeString() ?? "",
                        NumeroCargaEmbarcador = fila.Carga?.CodigoCargaEmbarcador ?? "",
                        StatusGr = (fila.Carga != null) ? fila.Carga.ProblemaIntegracaoGrMotoristaVeiculo ? "Não OK" : "OK" : "",
                        MensagemLicenca = listaMensagemLicenca.Where(licenca => licenca.CodigoCarga == (fila.Carga?.Codigo ?? 0)).Select(licenca => licenca.Mensagem).FirstOrDefault(),
                        VeiculoDedicado = isTerceiro ? fila.ConjuntoVeiculo?.Tracao?.CentroResultado != null ? "Sim" : "Não" : fila.ConjuntoVeiculoDedicado.ObterDescricao(),
                        Rastreador = (listaPosicaoVeiculos.Where(posicaoVeiculo => fila.ConjuntoVeiculo.ObterCodigos().Contains(posicaoVeiculo.CodigoVeiculo) && posicaoVeiculo.Rastreador).Count() > 0) ? "Sim" : "Não",
                        DataUltimaPosicao = listaPosicaoVeiculos.Where(posicaoVeiculo => fila.ConjuntoVeiculo.ObterCodigos().Contains(posicaoVeiculo.CodigoVeiculo)).Min(posicaoVeiculo => (DateTime?)posicaoVeiculo.DataPosicaoAtual)?.ToDateTimeString() ?? "",
                        Extra = fila.DataProgramadaAlteradaAutomaticamente.ObterDescricao(),
                        DT_FontColor = "#212529",
                        DT_RowColor = fila.ObterCorLinha(),
                        AreaVeiculo = fila.AreaVeiculo?.Descricao ?? "",
                        PermiteEnviarNotificacaoSuperApp = (configuracaoIntegracao?.PossuiIntegracaoTrizy ?? false) && !string.IsNullOrEmpty(fila.ConjuntoMotorista.Motorista?.CPF),
                        UltimaPosicaoDescricao = listaPosicaoVeiculos.Where(posicaoVeiculo => fila.ConjuntoVeiculo.ObterCodigos().Contains(posicaoVeiculo.CodigoVeiculo)).Min(posicaoVeiculo => (string)posicaoVeiculo.UltimaPosicaoDescricao)?.ToString() ?? "",

                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaEmTransicao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculo()
                {
                    CodigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular"),
                    CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicular"),
                    Situacoes = new List<SituacaoFilaCarregamentoVeiculo>() { SituacaoFilaCarregamentoVeiculo.EmTransicao },
                    Tipo = null
                };

                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = grid.inicio,
                    LimiteRegistros = grid.limite,
                    PropriedadeOrdenar = "DataEntrada"
                };
                Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoVeiculo
                    select new
                    {
                        fila.Codigo,
                        ModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga.Descricao,
                        Tracao = fila.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                        DT_FontColor = "#212529",
                        DT_RowColor = fila.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaFluxoPatio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "PrimeiroNaFila", visivel: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 18, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 18, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 27, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Posição", propriedade: "Posicao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Nº Equipamento", propriedade: "Equipamento", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                List<int> listaCodigoPrimeiroDisponivelNaFilaCarregamento = new List<int>();
                int totalRegistros = 0;

                int codigoFilial = Request.GetIntParam("Filial");
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

                if ((codigoFilial > 0) && !configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(codigoFilial);
                }

                if ((configuracaoGeralCarga.UtilizarProgramacaoCarga && (codigoFilial > 0)) || (centroCarregamento != null))
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoDisponivel()
                    {
                        CodigoCentroCarregamento = centroCarregamento?.Codigo ?? 0,
                        CodigoFilial = codigoFilial,
                        CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicular"),
                        CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : 0,
                    };

                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "asc",
                        InicioRegistros = grid.inicio,
                        LimiteRegistros = grid.limite,
                        PropriedadeOrdenar = "Posicao"
                    };

                    Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

                    totalRegistros = repositorioFilaCarregamentoVeiculo.ContarConsultaDisponivelPorAgrupamento(filtrosPesquisa);

                    if (totalRegistros > 0)
                        listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.ConsultarDisponivelPorAgrupamento(filtrosPesquisa, parametrosConsulta);

                    List<int> codigosModelosVeicularesCarga;

                    if (filtrosPesquisa.CodigoModeloVeicularCarga > 0)
                        codigosModelosVeicularesCarga = new List<int>() { filtrosPesquisa.CodigoModeloVeicularCarga };
                    else
                        codigosModelosVeicularesCarga = (from filaCarregamento in listaFilaCarregamentoVeiculo select filaCarregamento.ConjuntoVeiculo.ModeloVeicularCarga.Codigo).Distinct().ToList();

                    foreach (var codigoModeloVeicular in codigosModelosVeicularesCarga)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel filtrosPesquisaPrimeiroDisponivel = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPrimeiroDisponivel()
                        {
                            CodigoCentroCarregamento = filtrosPesquisa.CodigoCentroCarregamento,
                            CodigoFilial = codigoFilial,
                            CodigoModeloVeicularCarga = codigoModeloVeicular
                        };

                        Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo filaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.BuscarPrimeiroDisponivelNaFila(filtrosPesquisaPrimeiroDisponivel);

                        if (filaCarregamentoVeiculo != null)
                            listaCodigoPrimeiroDisponivelNaFilaCarregamento.Add(filaCarregamentoVeiculo.Codigo);
                    }
                }

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoVeiculo
                    select new
                    {
                        fila.Codigo,
                        Motorista = fila.ConjuntoMotorista.Motorista?.Descricao,
                        ModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga.Descricao,
                        Tracao = fila.ConjuntoVeiculo.Tracao?.Placa_Formatada,
                        Reboques = string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques select reboque.Placa_Formatada).ToList()),
                        Posicao = fila.ObterPosicao(),
                        Equipamento = fila.Equipamento?.Numero ?? string.Empty,
                        PrimeiroNaFila = listaCodigoPrimeiroDisponivelNaFilaCarregamento.Contains(fila.Codigo)
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista()
                {
                    CodigoCentroCarregamento = Request.GetIntParam("CentroCarregamento"),
                    CodigoGrupoModeloVeicularCarga = Request.GetIntParam("GrupoModeloVeicular"),
                    CodigosModeloVeicularCarga = Request.GetListParam<int>("ModeloVeicular"),
                    RetornarMotoristaComReboqueAtrelado = true
                };

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "Situacao", visivel: false);
                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 10, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = "desc",
                    InicioRegistros = grid.inicio,
                    LimiteRegistros = grid.limite,
                    PropriedadeOrdenar = "DataEntrada"
                };
                Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorio = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaSemFilaCarregamentoVeiculo(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista> listaFilaCarregamentoMotorista = totalRegistros > 0 ? repositorio.ConsultarSemFilaCarregamentoVeiculo(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>();

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoMotorista
                    select new
                    {
                        fila.Codigo,
                        ModeloVeicularCarga = fila.ConjuntoVeiculo?.ModeloVeicularCarga?.Descricao,
                        Motorista = fila.Motorista.Nome,
                        Tracao = fila.ConjuntoVeiculo?.Tracao?.Placa_Formatada,
                        Situacao = fila.Situacao,
                        DT_FontColor = fila.ObterCorFonte(),
                        DT_RowColor = fila.ObterCorLinha()
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaPreCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga repositorioConfiguracaoPreCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPreCarga(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga repositorioConfiguracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPreCarga configuracaoPreCarga = repositorioConfiguracaoPreCarga.BuscarPrimeiroRegistro();
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = repositorioConfiguracaoGeralCarga.BuscarPrimeiroRegistro();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresa", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaMotorista", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaReboque", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaSegundoReboque", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaTracao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoMotorista", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoReboque", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoSegundoReboque", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTracao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DescricaoEmpresa", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DescricaoReboque", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DescricaoSegundoReboque", visivel: false);
                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 18, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Reboques", propriedade: "Reboques", tamanho: 18, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Transportador", propriedade: "Transportador", tamanho: 27, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 27, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Nº Equipamento", propriedade: "Equipamento", tamanho: 27, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (this.ConfiguracaoEmbarcador.UtilizarControleJornadaMotorista)
                    grid.AdicionarCabecalho(descricao: "Jornada", propriedade: "Jornada", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                if (configuracaoGeralCarga.UtilizarProgramacaoCarga)
                {
                    grid.AdicionarCabecalho(descricao: "Dedicado", propriedade: "VeiculoDedicado", tamanho: 10, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                    grid.AdicionarCabecalho(descricao: "Data de Chegada", propriedade: "DataProgramada", tamanho: 20, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);
                }

                grid.AdicionarCabecalho(descricao: "Posição", propriedade: "Posicao", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo> listaFilaCarregamentoVeiculo = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculo>();
                int totalRegistros = 0;

                int codigoPreCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga);

                if (preCarga?.Filial != null)
                {
                    Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

                    if (!configuracaoGeralCarga.UtilizarProgramacaoCarga)
                    {
                        Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                        centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(preCarga.Filial.Codigo);
                    }

                    if (configuracaoGeralCarga.UtilizarProgramacaoCarga || (centroCarregamento != null))
                    {
                        Repositorio.Embarcador.PreCargas.PreCargaDestino repositorioPreCargaDestino = new Repositorio.Embarcador.PreCargas.PreCargaDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino repositorioPreCargaEstadoDestino = new Repositorio.Embarcador.PreCargas.PreCargaEstadoDestino(unitOfWork);
                        Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino repositorioPreCargaRegiaoDestino = new Repositorio.Embarcador.PreCargas.PreCargaRegiaoDestino(unitOfWork);

                        List<int> codigosDestinos = repositorioPreCargaDestino.BuscarCodigosDestinosPorPreCarga(preCarga.Codigo);
                        List<int> codigosRegioesDestino = repositorioPreCargaRegiaoDestino.BuscarCodigosRegioesDestinoPorPreCarga(preCarga.Codigo);
                        List<string> siglasEstadosDestino = repositorioPreCargaEstadoDestino.BuscarSiglasEstadosDestinoPorPreCarga(preCarga.Codigo);

                        Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPreCarga filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoVeiculoPreCarga()
                        {
                            CodigoCentroCarregamento = centroCarregamento?.Codigo ?? 0,
                            CodigoFilial = preCarga.Filial.Codigo,
                            CodigoModeloVeicularCarga = !configuracaoPreCarga.VincularPrePlanoSemValidarModeloVeicularCarga ? preCarga.ModeloVeicularCarga?.Codigo ?? 0 : 0,
                            CodigoPreCarga = preCarga.Codigo,
                            CodigosDestinos = codigosDestinos,
                            CodigosRegioesDestino = codigosRegioesDestino,
                            DataProgramada = preCarga.DataPrevisaoEntrega,
                            SiglasEstadosDestino = siglasEstadosDestino
                        };
                        Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                        {
                            DirecaoOrdenar = "asc",
                            InicioRegistros = grid.inicio,
                            LimiteRegistros = grid.limite,
                            PropriedadeOrdenar = "Posicao"
                        };
                        Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo repositorioFilaCarregamentoVeiculo = new Repositorio.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork);

                        totalRegistros = repositorioFilaCarregamentoVeiculo.ContarConsultaDisponivelParaPreCarga(filtrosPesquisa);

                        if (totalRegistros > 0)
                            listaFilaCarregamentoVeiculo = repositorioFilaCarregamentoVeiculo.ConsultarDisponivelParaPreCarga(filtrosPesquisa, parametrosConsulta);
                    }
                }

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoVeiculo
                    select new
                    {
                        fila.Codigo,
                        CodigoEmpresa = fila.ObterTransportador()?.Codigo ?? 0,
                        CodigoEmpresaMotorista = fila.ConjuntoMotorista.Motorista?.Empresa?.Codigo ?? 0,
                        CodigoEmpresaReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(0)?.Empresa?.Codigo ?? 0,
                        CodigoEmpresaSegundoReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(1)?.Empresa?.Codigo ?? 0,
                        CodigoEmpresaTracao = fila.ConjuntoVeiculo.Tracao?.Empresa?.Codigo ?? 0,
                        CodigoMotorista = fila.ConjuntoMotorista.Motorista?.Codigo,
                        CodigoReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(0)?.Codigo,
                        CodigoSegundoReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(1)?.Codigo,
                        CodigoTracao = fila.ConjuntoVeiculo.Tracao?.Codigo,
                        DescricaoEmpresa = fila.ObterTransportador()?.Descricao ?? "",
                        DescricaoReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(0)?.Placa ?? "",
                        DescricaoSegundoReboque = fila.ConjuntoVeiculo.Reboques?.ElementAtOrDefault(1)?.Placa ?? "",
                        Transportador = fila.ConjuntoVeiculo.Tracao?.Empresa?.Descricao ?? string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques where reboque.Empresa != null select reboque.Empresa.Descricao).Distinct().ToList()),
                        Motorista = fila.ConjuntoMotorista.Motorista?.Descricao,
                        ModeloVeicularCarga = fila.ConjuntoVeiculo.ModeloVeicularCarga.Descricao,
                        Tracao = fila.ConjuntoVeiculo.Tracao?.Placa,
                        Reboques = string.Join(", ", (from reboque in fila.ConjuntoVeiculo.Reboques select reboque.Placa).ToList()),
                        VeiculoDedicado = fila.ConjuntoVeiculoDedicado.ObterDescricao(),
                        DataProgramada = fila.DataProgramada?.ToDateTimeString() ?? "",
                        Posicao = fila.ObterPosicao(),
                        Equipamento = fila.Equipamento?.ToString() ?? string.Empty,
                        Jornada = ObterJornadaMotorista(fila.ConjuntoMotorista.Motorista, unitOfWork)
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Models.Grid.Grid ObterGridPesquisaPreCargaMotorista()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaMotorista", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoEmpresaTracao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoMotorista", visivel: false);
                grid.AdicionarCabecalho(propriedade: "CodigoTracao", visivel: false);
                grid.AdicionarCabecalho(propriedade: "DescricaoEmpresaMotorista", visivel: false);
                grid.AdicionarCabecalho(descricao: "Motorista", propriedade: "Motorista", tamanho: 27, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                if (this.ConfiguracaoEmbarcador.UtilizarControleJornadaMotorista)
                    grid.AdicionarCabecalho(descricao: "Jornada", propriedade: "Jornada", tamanho: 18, alinhamento: Models.Grid.Align.center, permiteOrdenacao: false);

                grid.AdicionarCabecalho(descricao: "Modelo Veicular", propriedade: "ModeloVeicularCarga", tamanho: 22, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);
                grid.AdicionarCabecalho(descricao: "Tração", propriedade: "Tracao", tamanho: 18, alinhamento: Models.Grid.Align.left, permiteOrdenacao: false);

                List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista> listaFilaCarregamentoMotorista = new List<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoMotorista>();
                int totalRegistros = 0;

                int codigoPreCarga = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.PreCargas.PreCarga repositorioPreCarga = new Repositorio.Embarcador.PreCargas.PreCarga(unitOfWork);
                Dominio.Entidades.Embarcador.PreCargas.PreCarga preCarga = repositorioPreCarga.BuscarPorCodigo(codigoPreCarga);
                Dominio.Entidades.Embarcador.Logistica.CentroCarregamento centroCarregamento = null;

                if (preCarga?.Filial != null)
                {
                    Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
                    centroCarregamento = repositorioCentroCarregamento.BuscarPorFilial(preCarga.Filial.Codigo);
                }

                if (centroCarregamento != null)
                {
                    Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaFilaCarregamentoMotorista()
                    {
                        CodigoCentroCarregamento = centroCarregamento.Codigo
                    };
                    Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                    {
                        DirecaoOrdenar = "asc",
                        InicioRegistros = grid.inicio,
                        LimiteRegistros = grid.limite,
                        PropriedadeOrdenar = "Posicao"
                    };
                    Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista repositorioFilaCarregamentoMotorista = new Repositorio.Embarcador.Logistica.FilaCarregamentoMotorista(unitOfWork);

                    totalRegistros = repositorioFilaCarregamentoMotorista.ContarConsultaSemFilaCarregamentoVeiculo(filtrosPesquisa);

                    if (totalRegistros > 0)
                        listaFilaCarregamentoMotorista = repositorioFilaCarregamentoMotorista.ConsultarSemFilaCarregamentoVeiculo(filtrosPesquisa, parametrosConsulta);
                }

                var listaFilaCarregamentoRetornar = (
                    from fila in listaFilaCarregamentoMotorista
                    select new
                    {
                        fila.Codigo,
                        CodigoEmpresaMotorista = fila.Motorista.Empresa?.Codigo ?? 0,
                        CodigoEmpresaTracao = fila.ConjuntoVeiculo?.Tracao?.Empresa?.Codigo ?? 0,
                        CodigoMotorista = fila.Motorista.Codigo,
                        CodigoTracao = fila.ConjuntoVeiculo?.Tracao?.Codigo,
                        DescricaoEmpresaMotorista = fila.Motorista.Empresa?.Descricao ?? "",
                        Motorista = fila.Motorista.Descricao,
                        ModeloVeicularCarga = fila.ConjuntoVeiculo?.ModeloVeicularCarga.Descricao,
                        Tracao = fila.ConjuntoVeiculo?.Tracao?.Placa,
                        Jornada = ObterJornadaMotorista(fila.Motorista, unitOfWork)
                    }
                ).ToList();

                grid.AdicionaRows(listaFilaCarregamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterJornadaMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Transportadores.MotoristaJornada repositorioMotoristaJornada = new Servicos.Embarcador.Transportadores.MotoristaJornada(unitOfWork, this.ConfiguracaoEmbarcador);

            return repositorioMotoristaJornada.ObterJornadaMotorista(motorista);
        }

        private OrigemAlteracaoFilaCarregamento ObterOrigemAlteracaoFilaCarregamento()
        {
            return Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo.ObterOrigemAlteracaoFilaCarregamento(TipoServicoMultisoftware);
        }

        private List<ConfiguracaoImportacao> ConfiguracaoImportacaoFilaCarregamento()
        {
            var configuracoes = new List<ConfiguracaoImportacao>();

            configuracoes.Add(new ConfiguracaoImportacao() { Id = 1, Descricao = "Veículo", Propriedade = "Placa", Tamanho = 150, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 2, Descricao = "Motorista", Propriedade = "CPFMotorista", Tamanho = 150, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 3, Descricao = "Previsao de chegada", Propriedade = "DataPrevisaoChegada", Tamanho = 150, CampoInformacao = true, Regras = new List<string> { "required" } });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 4, Descricao = "Estado", Propriedade = "Estado", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 5, Descricao = "Cidade", Propriedade = "Cidade", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 6, Descricao = "Região", Propriedade = "Regiao", Tamanho = 150, CampoInformacao = true });
            configuracoes.Add(new ConfiguracaoImportacao() { Id = 7, Descricao = "Tipo de Carga", Propriedade = "TipoDeCarga", Tamanho = 150, CampoInformacao = true });

            return configuracoes;
        }

        #endregion
    }
}
