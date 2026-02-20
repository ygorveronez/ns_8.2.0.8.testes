using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile;
using Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Monitoramento" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Monitoramento.svc or Monitoramento.svc.cs at the Solution Explorer and start debugging.
    public class Monitoramento : WebServiceBase, IMonitoramento
    {
        public void DoWork()
        {
            throw new NotImplementedException();
        }

        #region Métodos Públicos

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> ObterParada(string codigoCargaEntrega, string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSessaoExpirada();


                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedidoProduto repCargaPedidoProduto = new Repositorio.Embarcador.Cargas.CargaPedidoProduto(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade repCargaPedidoProdutoDivisaoCapacidade = new Repositorio.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade(unitOfWork);
                        Repositorio.Embarcador.Cargas.CargaRotaFrete repCargaRotaFrete = new Repositorio.Embarcador.Cargas.CargaRotaFrete(unitOfWork);
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                        Repositorio.Embarcador.Logistica.PeriodoDescarregamento repPeriodoDescarregamento = new Repositorio.Embarcador.Logistica.PeriodoDescarregamento(unitOfWork);
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto repCargaEntregaProduto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto(unitOfWork);
                        Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                        Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        int.TryParse(codigoCargaEntrega, out int codigoCargaEntregaCod);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntregaCod);

                        if (cargaEntrega != null)
                        {
                            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaProduto> cargaEntregaProdutos = repCargaEntregaProduto.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> cargaPedidoProdutos = repCargaPedidoProduto.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProdutoDivisaoCapacidade> cargaCargaPedidoProdutoDivisaoCapacidade = repCargaPedidoProdutoDivisaoCapacidade.BuscarPorCarga(cargaEntrega.Carga.Codigo);
                            List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarAtendimentosPorEntrega(cargaEntrega.Codigo);
                            List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarAnalisesAtendimentosPorEntrega(cargaEntrega.Codigo);
                            List<Dominio.Entidades.Embarcador.Canhotos.Canhoto> canhotos = repCanhoto.BuscarPorCarga(cargaEntrega.Carga.Codigo);

                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                            Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada parada = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterParada(cargaEntrega, cargaEntregaNotasFiscais, cargaEntregaPedidos, cargaEntregaProdutos, cargaPedidoProdutos, cargaCargaPedidoProdutoDivisaoCapacidade, chamados, chamadoAnalises, canhotos, configuracao, unitOfWork);
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSucesso(parada);
                        }
                        else
                        {
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoDadosInvalidos("Não foi localizada uma parada para o código informado");
                        }

                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os dados da parada.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>> ObterCargas(string dataUltimaVerificacao, string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoSessaoExpirada();

                DateTime data;
                if (!DateTime.TryParseExact(dataUltimaVerificacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data))
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoDadosInvalidos("A data de ultima verificação não esta em um formato correto (ddMMyyyyHHmmss);");
                else
                {
                    AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                    int.TryParse(clienteMultisoftware, out int codigoCliente);
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        try
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga> cargas = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterControleEntregaCargaMobile(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork);
                            return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoSucesso(cargas);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os dados das cargas.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>> ObterAtendimentos(string codigoCarga, string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        int.TryParse(codigoCarga, out int codigoCargaParsed);
                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> paradasAtendimento = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterAtendimentosCargaMobile(codigoCargaParsed, unitOfWork);
                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>>.CriarRetornoSucesso(paradasAtendimento);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os atendimentos da carga.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada> ObterEntrega(string codigoEntrega, string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                int.TryParse(codigoEntrega, out int codigoEntregaMobile);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoEntregaMobile);

                        if (cargaEntrega == null)
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoExcecao("Não foi possível encontrar a entrega.");

                        Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga cargaMobile = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ConverterCargaControleEntrega(cargaEntrega.Carga, cargaEntrega.Codigo, unitOfWork);

                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSucesso(cargaMobile.Paradas.FirstOrDefault());
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Parada>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os dados das cargas.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga> ObterCarga(string codigoCarga, string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>.CriarRetornoSessaoExpirada();


                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);

                int.TryParse(codigoCarga, out int codigoCargaMobile);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoFetch(codigoCargaMobile);
                        Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga cargaMobile = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ConverterCargaControleEntrega(carga, unitOfWork);
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>.CriarRetornoSucesso(cargaMobile);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os dados das cargas.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarViagem(int clienteMultisoftware, int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                    {
                        try
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                            DateTime data;
                            if (!DateTime.TryParseExact(coordenada.dataCoordenada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data))
                                return Retorno<bool>.CriarRetornoDadosInvalidos("A data de coordenada não esta em um formato correto (ddMMyyyyHHmmss);");
                            else
                            {
                                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                                Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                                if (carga.DataInicioViagem == null)
                                {
                                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                                    Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                                    if (!(carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                                    {
                                        wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                        {
                                            Latitude = coordenada.latitude,
                                            Longitude = coordenada.longitude
                                        };
                                    }

                                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                                    {
                                        TipoAuditado = TipoAuditado.Usuario,
                                        OrigemAuditado = OrigemAuditado.WebServiceMobile
                                    };

                                    if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, data, OrigemSituacaoEntrega.App, wayPoint, configuracaoEmbarcador, TipoServicoMultisoftware, usuarioMobileCliente?.Cliente, auditado, unitOfWork))
                                    {
                                        if (motorista != null)
                                            Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Início de viagem informado pelo motorista {motorista.Descricao} via app", unitOfWork);

                                    }
                                }

                                // Notifica para outros motoristas da carga que uma ação foi realizada
                                if (motorista != null)
                                {
                                    Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                                    serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(carga, null, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.InicioViagem);
                                }

                                return Retorno<bool>.CriarRetornoSucesso(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar viagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> Confirmar(Dominio.ObjetosDeValor.Embarcador.Mobile.Request.Confirmar parameters)
        {
            Servicos.Log.TratarErro($"Iniciando Confirmar - {Newtonsoft.Json.JsonConvert.SerializeObject(parameters)}"); // Pelo amor de deus não remove isso até o app antigo for desativado

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);

                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, parameters.clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                    {
                        try
                        {
                            DateTime.TryParseExact(parameters.terminoColetaEntrega, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataTerminoColetaEntrega);
                            DateTime.TryParseExact(parameters.inicioColetaEntrega, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioColetaEntrega);
                            DateTime.TryParseExact(parameters.dataConfirmacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataConfirmacao);


                            DateTime.TryParseExact(parameters.dataConfirmacaoChegada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataConfirmacaoChegada);

                            if (dataInicioColetaEntrega == DateTime.MinValue && dataTerminoColetaEntrega >= DateTime.MinValue)
                                dataInicioColetaEntrega = dataTerminoColetaEntrega;

                            if (dataTerminoColetaEntrega == DateTime.MinValue && dataInicioColetaEntrega >= DateTime.MinValue)
                                dataTerminoColetaEntrega = dataInicioColetaEntrega;

                            if (dataInicioColetaEntrega == DateTime.MinValue)
                                dataInicioColetaEntrega = DateTime.Now;

                            if (dataTerminoColetaEntrega == DateTime.MinValue)
                                dataTerminoColetaEntrega = DateTime.Now;

                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                            Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(parameters.codigoCargaEntrega);

                            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

                            List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
                            if (!listaSituacaoEmAberto.Contains(cargaEntrega.Situacao))
                            {
                                Servicos.Log.TratarErro($"ConfirmarEntrega - CargaEntrega finalizada");
                                return Retorno<bool>.CriarRetornoSucesso(true);
                            }

                            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                            {
                                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile,
                                Usuario = motorista
                            };

                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDescarga = null;
                            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;

                            if (!(cargaEntrega?.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                            {
                                wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                {
                                    Latitude = parameters.coordenada.latitude,
                                    Longitude = parameters.coordenada.longitude
                                };

                                if (parameters.coordenadaDescarga != null)
                                {
                                    wayPointDescarga = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                    {
                                        Latitude = parameters.coordenadaDescarga.latitude,
                                        Longitude = parameters.coordenadaDescarga.longitude
                                    };
                                }

                                if (parameters.coordenadaConfirmacaoChegada != null)
                                {
                                    wayPointConfirmacaoChegada = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                    {
                                        Latitude = parameters.coordenadaConfirmacaoChegada.latitude,
                                        Longitude = parameters.coordenadaConfirmacaoChegada.longitude
                                    };
                                }
                            }

                            unitOfWork.Start();

                            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros parametrosFinalizarEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FinalizarEntregaParametros
                            {
                                cargaEntrega = cargaEntrega,
                                dataInicioEntrega = dataInicioColetaEntrega,
                                dataTerminoEntrega = dataTerminoColetaEntrega,
                                dataConfirmacao = dataConfirmacao,

                                dataSaidaRaio = null,
                                wayPoint = wayPoint,

                                pedidos = parameters.pedidos,
                                motivoRetificacao = parameters.motivoRetificacao,
                                justificativaEntregaForaRaio = parameters.justificativaEntregaForaRaio,
                                motivoFalhaGTA = parameters.motivoFalhaGTA,
                                configuracaoEmbarcador = configuracao,
                                tipoServicoMultisoftware = TipoServicoMultisoftware,
                                sistemaOrigem = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                                dadosRecebedor = parameters.dadosRecebedor,
                                wayPointDescarga = wayPointDescarga,

                                // Confirmação de chegada
                                dataConfirmacaoChegada = dataConfirmacaoChegada,
                                wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,

                                // Documentos
                                handlingUnitIds = parameters.handlingUnitIds,
                                chavesNFe = parameters.chavesNFe,

                                motorista = motorista,

                                OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                                auditado = auditado,
                                configuracaoControleEntrega = configuracaoControleEntrega,
                                tipoOperacaoParametro = tipoOperacaoParametro,
                                TornarFinalizacaoDeEntregasAssincrona = configuracaoControleEntrega.TornarFinalizacaoDeEntregasAssincrona
                            };

                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(parametrosFinalizarEntrega, unitOfWork);

                            if (cargaEntrega?.Carga != null)
                            {
                                Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(unitOfWork);

                                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoGatilhoTracking tipoAplicacaoGatilhoTracking = cargaEntrega.Coleta ? TipoAplicacaoGatilhoTracking.Coleta : TipoAplicacaoGatilhoTracking.Entrega;

                                if (cargaEntrega.Fronteira)
                                    servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaFronteira, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                                else if (cargaEntrega.Parqueamento)
                                    servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, cargaEntrega.Cliente.CPF_CNPJ, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.SaidaParqueamento, dataConfirmacaoChegada, dataConfirmacao, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                                else
                                    servicoOcorrenciaAutomaticaPorPeriodo.GerarOcorrenciaPorTracking(cargaEntrega.Carga, codigoFronteiraParqueamento: 0d, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GatilhoFinalTraking.FimEntrega, dataInicioColetaEntrega, dataTerminoColetaEntrega, TipoServicoMultisoftware, Cliente, tipoAplicacaoGatilhoTracking, cargaEntrega.DataAgendamento);
                            }

                            unitOfWork.CommitChanges();

                            // Notifica para outros motoristas da carga que uma ação foi realizada
                            if (motorista != null)
                            {
                                Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                                serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(cargaEntrega?.Carga, cargaEntrega, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.FinalizacaoEntregaColeta);
                            }

                            Servicos.Log.TratarErro($"Finalizando Confirmar - codigoCargaEntrega: {parameters.codigoCargaEntrega}");
                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        catch (ServicoException excecao)
                        {
                            throw;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Erro ao Confirmar => " + Newtonsoft.Json.JsonConvert.SerializeObject(parameters));
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (ServicoException excecao)
            {
                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RegistroCanceladoOuAnulado)
                    return Retorno<bool>.CriarRetornoRegistroIndisponivel(excecao.Message);

                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                Servicos.Log.TratarErro("Erro ao Confirmar => " + Newtonsoft.Json.JsonConvert.SerializeObject(parameters));
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> Rejeitar(Dominio.ObjetosDeValor.Embarcador.Mobile.Request.Rejeitar parameters)
        {
            Servicos.Log.TratarErro($"Iniciando Rejeitar - codigoCargaEntrega: {parameters.codigoCargaEntrega}");
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, parameters.clienteMultisoftware);

                if (usuarioMobileCliente == null)
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                DateTime.TryParseExact(parameters.coordenada.dataCoordenada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataDevolucao);
                DateTime.TryParseExact(parameters.inicioCarregamento, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioCarregamento);
                DateTime.TryParseExact(parameters.terminoCarregamento, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataTerminoCarregamento);
                DateTime.TryParseExact(parameters.inicioDescarga, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicioDescarga);
                DateTime.TryParseExact(parameters.terminoDescarga, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataTerminoDescarga);
                DateTime.TryParseExact(parameters.dataConfirmacaoChegada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataConfirmacaoChegada);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    try
                    {
                        unitOfWork.Start();
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceMobile
                        };
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(parameters.codigoCargaEntrega);
                        Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                        auditado.Usuario = motorista;

                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointDescarga = null;
                        Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPointConfirmacaoChegada = null;
                        if (!(cargaEntrega?.Carga.TipoOperacao?.ConfiguracaoMobile?.BloquearRastreamento ?? false))
                        {
                            wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                            {
                                Latitude = parameters.coordenada.latitude,
                                Longitude = parameters.coordenada.longitude
                            };

                            if (parameters.coordenadaDescarga != null)
                            {
                                wayPointDescarga = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                {
                                    Latitude = parameters.coordenadaDescarga.latitude,
                                    Longitude = parameters.coordenadaDescarga.longitude
                                };
                            }

                            if (parameters.coordenadaConfirmacaoChegada != null)
                            {
                                wayPointConfirmacaoChegada = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint
                                {
                                    Latitude = parameters.coordenadaConfirmacaoChegada.latitude,
                                    Longitude = parameters.coordenadaConfirmacaoChegada.longitude
                                };
                            }
                        }

                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
                        {
                            codigoCargaEntrega = parameters.codigoCargaEntrega,
                            codigoMotivo = parameters.codigoMotivo,
                            data = dataDevolucao,
                            dataInicioCarregamento = dataInicioCarregamento,
                            dataTerminoCarregamento = dataTerminoCarregamento,
                            dataInicioDescarga = dataInicioDescarga,
                            dataTerminoDescarga = dataTerminoDescarga,
                            wayPoint = wayPoint,
                            wayPointDescarga = wayPointDescarga,
                            usuario = null,
                            motivoRetificacao = parameters.motivoRetificacao,
                            tipoServicoMultisoftware = TipoServicoMultisoftware,
                            observacao = parameters.observacao,
                            configuracao = configuracao,
                            devolucaoParcial = parameters.devolucaoParcial,
                            produtos = parameters.produtos,
                            motivoFalhaGTA = parameters.motivoFalhaGTA,
                            apenasRegistrar = false,
                            dadosRecebedor = parameters.dadosRecebedor,
                            permitirEntregarMaisTarde = false,
                            dataConfirmacaoChegada = dataConfirmacaoChegada,
                            wayPointConfirmacaoChegada = wayPointConfirmacaoChegada,
                            atendimentoRegistradoPeloMotorista = true,
                            OrigemSituacaoEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App,
                            clienteMultisoftware = usuarioMobileCliente.Cliente
                        };

                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, unitOfWork, out chamado, TipoServicoMultisoftware);

                        if (chamado != null)
                        {
                            if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                            {
                                int codigoChamado = chamado.Codigo;
                                string stringConexao = unitOfWork.StringConexao;
                                Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                            }
                            Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);
                        }

                        unitOfWork.CommitChanges();

                        if (chamado != null)
                        {
                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
                            Repositorio.Embarcador.Chamados.ChamadoAnexo repChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);

                            if (repCargaEntregaFoto.PossuiImagemEnviadaAposData(parameters.codigoCargaEntrega, DateTime.Now.AddMinutes(-2)) && !repChamadoAnexo.PossuiAnexo(chamado.Codigo))
                            {
                                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto imagemEntrega = repCargaEntregaFoto.BuscarUltimaPorCargaEntrega(parameters.codigoCargaEntrega);

                                string extensao = ".jpg";
                                string tokenImagem = imagemEntrega.GuidArquivo;
                                string caminhoEntrega = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" }), tokenImagem + extensao);

                                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoEntrega))
                                {
                                    string caminhoChamado = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Chamados" }), tokenImagem + extensao);

                                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
                                        Utilidades.IO.FileStorageService.Storage.Copy(caminhoEntrega, caminhoChamado);

                                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoChamado))
                                    {
                                        Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo()
                                        {
                                            Chamado = chamado,
                                            Descricao = string.Empty,
                                            GuidArquivo = tokenImagem,
                                            NomeArquivo = imagemEntrega.NomeArquivo
                                        };

                                        repChamadoAnexo.Inserir(chamadoAnexo);
                                        if (auditado != null)
                                            Servicos.Auditoria.Auditoria.Auditar(auditado, chamado, $"Copiou o anexo {imagemEntrega.NomeArquivo} do controle de entrega", unitOfWork);
                                    }
                                    else
                                        Servicos.Log.TratarErro("Rejeitar - Imagem não foi salva para o Atendimento " + chamado.Numero);
                                }
                                else
                                    Servicos.Log.TratarErro("Rejeitar - Imagem do controle de entrega não foi encontrada para copiar ao Atendimento " + chamado.Numero);
                            }

                            new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                            return Retorno<bool>.CriarRetornoSucesso(false);
                        }

                        // Notifica para outros motoristas da carga que uma ação foi realizada
                        if (motorista != null)
                        {
                            Servicos.Embarcador.Chamado.NotificacaoMobile serNotificaoMobile = new Servicos.Embarcador.Chamado.NotificacaoMobile(unitOfWork, 0);
                            serNotificaoMobile.NotificarCargaAtualizadaPorOutroMotorista(cargaEntrega?.Carga, cargaEntrega, motorista, TipoEventoAlteracaoCargaPorOutroMotorista.RejeicaoEntregaColeta);
                        }

                        Servicos.Log.TratarErro($"Finalizando Rejeitar - codigoCargaEntrega: {parameters.codigoCargaEntrega}");
                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ObterTipoServico(usuarioMobileCliente).ToString());
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro("Erro ao Rejeitar => " + Newtonsoft.Json.JsonConvert.SerializeObject(parameters));
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                    }
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar Rejeitar");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> AtualizarDadosPosicionamento(int clienteMultisoftware, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada> coordenadas, int codigoCarga = 0)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
                        Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                        if (motorista != null)
                        {
                            unitOfWork.Start();

                            Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramentoOuMotorista(unitOfWork, codigoCarga, motorista?.Codigo ?? 0);

                            Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(unitOfWork);
                            //try
                            //{
                            //    Servicos.Log.TratarErro("AtualizarDadosPosicionamento:coordenadas => " + Newtonsoft.Json.JsonConvert.SerializeObject(coordenadas));
                            //}
                            //catch (Exception ex)
                            //{
                            //    Servicos.Log.TratarErro("AtualizarDadosPosicionamento:ex => " + ex.ToString());
                            //}

                            if (veiculo != null)
                            {
                                for (int i = 0; i < coordenadas.Count; i++)
                                {
                                    Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada = coordenadas[i];

                                    DateTime dataPosicao;
                                    DateTime.TryParseExact(coordenada.dataCoordenada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out dataPosicao);

                                    Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao()
                                    {
                                        ID = coordenada.id,
                                        Data = dataPosicao,
                                        DataVeiculo = dataPosicao,
                                        DataCadastro = DateTime.Now,
                                        IDEquipamento = motorista.Codigo.ToString(),
                                        Veiculo = veiculo,
                                        Ignicao = 1,
                                        Latitude = coordenada.latitude,
                                        Longitude = coordenada.longitude,
                                        Velocidade = coordenada.Velocidade.HasValue ? (int)coordenada.Velocidade.Value : 0,
                                        Temperatura = 0,
                                        Descricao = $"{coordenada.latitude.ToString()}, {coordenada.longitude.ToString()} (M)",
                                        NivelBateria = coordenada.nivelBateria,
                                        NivelSinalGPS = coordenada.nivelSinalGPS
                                    };
                                    repPosicaoPendenteIntegracao.Inserir(posicaoPendenteIntegracao);
                                }
                            }

                            //VAMOS ATUALIZAR A LISTA DE MOTORISTAS QUE ESTAO NESTE CPF na qual estao com versao do APP diferente;
                            if (!string.IsNullOrEmpty(usuarioMobile.VersaoAPP))
                                AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF, usuarioMobile.VersaoAPP);

                            ////if (motorista.VersaoAPP != usuarioMobile.VersaoAPP)
                            ////{
                            ////    motorista.VersaoAPP = usuarioMobile.VersaoAPP;
                            ////    repositorioMotorista.Atualizar(motorista);
                            ////}


                            unitOfWork.CommitChanges();

                        }
                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar as coordenadas.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> AtualizarCheckList(int clienteMultisoftware, int codigoCargaEntrega, List<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RespostaCheckList> respostas)
        {
            Servicos.Log.TratarErro($"Iniciando AtualizarCheckList - codigoCargaEntrega: {codigoCargaEntrega}, respostas: {Newtonsoft.Json.JsonConvert.SerializeObject(respostas)}"); // Pelo amor de deus não remove isso até o app antigo for desativado

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                    {
                        try
                        {
                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList repCargaEntregaCheckList = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList(unitOfWork);

                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                            TipoCheckList tipoChecklist = cargaEntrega.Coleta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckList.Entrega;
                            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckList checkList = repCargaEntregaCheckList.BuscarPorCargaEntrega(cargaEntrega?.Codigo ?? 0, tipoChecklist);

                            if (checkList == null)
                                return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi possível encontrar CheckList da Coleta");

                            unitOfWork.Start();

                            new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaCheckList(unitOfWork).SalvarRespostasCheckList(checkList, respostas);

                            unitOfWork.CommitChanges();
                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> JustificarParadaNaoProgramada(int clienteMultisoftware, List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaParadaNaoProgramada> listaJustificativas)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                    {
                        try
                        {
                            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                            foreach (JustificativaParadaNaoProgramada justificativa in listaJustificativas)
                            {
                                Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alertaParadaNaoProgramada = repAlertaMonitor.BuscarPorCodigo(justificativa.codigoParadaNaoProgramada);

                                if (alertaParadaNaoProgramada != null)
                                {
                                    alertaParadaNaoProgramada.Observacao = justificativa.justificativa.Length > 300 ? justificativa.justificativa.Substring(0, 300) : justificativa.justificativa;
                                    repAlertaMonitor.Atualizar(alertaParadaNaoProgramada);
                                }
                            }

                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>> ObterMotivosRejeicaoEntrega(string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);


                if (usuarioMobileCliente != null)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao> motivoDevolucaos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>();

                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                    try
                    {
                        Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                        List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega, true);

                        foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia in tiposDeOcorrenciaDeCTe)
                        {
                            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao> listaMotivo = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>();
                            Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao motivo = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao
                            {
                                Codigo = tipoOcorrencia.Codigo,
                                Motivo = tipoOcorrencia.Descricao,
                                AguardarTratativa = tipoOcorrencia.MotivoChamado != null ? true : false,
                                ObrigarFoto = tipoOcorrencia.AnexoObrigatorio,
                                Devolucao = tipoOcorrencia.MotivoChamado != null ? tipoOcorrencia.MotivoChamado.TipoMotivoAtendimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoAtendimento.Devolucao : false
                            };
                            motivoDevolucaos.Add(motivo);
                        }

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>>.CriarRetornoSucesso(motivoDevolucaos);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os motivos de rejeição de entrega.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>> ObterMotivosRejeicaoColeta(string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);

                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                        List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta, true);

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta> motivoRejeicaos =
                        (from obj in tiposDeOcorrenciaDeCTe
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta()
                         {
                             Codigo = obj.Codigo,
                             Descricao = obj.Descricao,
                             CodigoTipoOperacaoColeta = obj.TipoOperacaoColeta?.Codigo ?? 0,
                             Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColetaProduto>()
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>>.CriarRetornoSucesso(motivoRejeicaos);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os motivos de rejeição de coleta.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>> ObterMotivosRetificacaoColeta(string clienteMultisoftware)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta repMotivoRetificacaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta(unitOfWork);

                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRetificacaoColeta> listaMotivosRetificacaoColeta = repMotivoRetificacaoColeta.BuscarAtivos(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta);

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta> motivoRetificacao =
                        (from obj in listaMotivosRetificacaoColeta
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta()
                         {
                             Codigo = obj.Codigo,
                             Descricao = obj.Descricao,
                             TipoOperacao = obj.TipoOperacao != null ?
                             (
                                 new Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.TipoOperacao()
                                 {
                                     Codigo = obj.TipoOperacao.Codigo,
                                     Descricao = obj.TipoOperacao.Descricao
                                 }
                             ) : null,
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>>.CriarRetornoSucesso(motivoRetificacao);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRetificacaoColeta>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os motivos de retificação de coleta.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>> ObterMotivosFalhaGTA(string clienteMultisoftware)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Produtos.MotivoFalhaGTA repMotivoFalhaGTA = new Repositorio.Embarcador.Produtos.MotivoFalhaGTA(unitOfWork);

                        List<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> listaMotivosFalhaGTA = repMotivoFalhaGTA.BuscarAtivos();

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA> motivoFalhaGTA =
                        (from obj in listaMotivosFalhaGTA
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA()
                         {
                             Codigo = obj.Codigo,
                             Descricao = obj.Descricao
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>>.CriarRetornoSucesso(motivoFalhaGTA);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoFalhaGTA>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os motivos de falhas do no GTA.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>> ObterJustificativasTemperatura(string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura repJustificativaTemperatura = new Repositorio.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura(unitOfWork);

                        List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.JustificativaTemperatura> listaJustificativasTemperatura = repJustificativaTemperatura.BuscarAtivos();

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura> justificativasTemperatura =
                        (from obj in listaJustificativasTemperatura
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura()
                         {
                             Codigo = obj.Codigo,
                             Descricao = obj.Descricao
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>>.CriarRetornoSucesso(justificativasTemperatura);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.JustificativaTemperatura>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as justificativas de temperatura.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>> ObterRegrasReconhecimentoCanhoto(string clienteMultisoftware)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto repConfiguracaoReconhecimentoCanhoto = new Repositorio.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto(unitOfWork);

                        List<Dominio.Entidades.Embarcador.Canhotos.ConfiguracaoReconhecimentoCanhoto> configuracoes = repConfiguracaoReconhecimentoCanhoto.BuscarAtivos();

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto> listaRegras =
                        (from obj in configuracoes
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto()
                         {
                             Codigo = obj.Codigo,
                             PalavrasChaves = obj.PalavrasChaves.Split(';').Where(p => p.Length > 0).ToList()
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>>.CriarRetornoSucesso(listaRegras);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.ConfiguracaoReconhecimentoCanhoto>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as regras de reconhecimento de canhotos.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> EnviarChaveNFeDevolucao(int clienteMultisoftware, int codigoCargaEntrega, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Nota nfeOrigem, string chaveNFe, string observacaoMotorista, string imagem)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {

                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao repCargaEntregaNFeDevolucao = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao(unitOfWork);

                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao cargaEntregaNFeDevolucao = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNFeDevolucao();
                        cargaEntregaNFeDevolucao.CargaEntrega = cargaEntrega;
                        cargaEntregaNFeDevolucao.ChaveNFe = chaveNFe;
                        cargaEntregaNFeDevolucao.ObservacaoMotorista = observacaoMotorista;
                        if (nfeOrigem != null && nfeOrigem.Codigo > 0)
                            cargaEntregaNFeDevolucao.XMLNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal() { Codigo = nfeOrigem.Codigo };

                        if (!string.IsNullOrWhiteSpace(imagem))
                        {
                            byte[] data = System.Convert.FromBase64String(imagem);

                            MemoryStream ms = new MemoryStream(data);

                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemNotaDevolucao(ms, unitOfWork, out string tokenImagem);

                            if (!string.IsNullOrWhiteSpace(tokenImagem))
                                cargaEntregaNFeDevolucao.GuidArquivo = tokenImagem;
                        }


                        repCargaEntregaNFeDevolucao.Inserir(cargaEntregaNFeDevolucao);

                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a chave da nf-e de devolução.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> NotaFiscalColetaEntrega(int clienteMultisoftware, int codigoCargaEntrega, string foto, string dataEnvio)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);
                if (usuarioMobileCliente == null) return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    if (!DateTime.TryParseExact(dataEnvio, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                        return Retorno<bool>.CriarRetornoDadosInvalidos("A data de envio não esta em um formato correto (ddMMyyyyHHmmss);");

                    byte[] buffer = System.Convert.FromBase64String(foto);
                    MemoryStream ms = new MemoryStream(buffer);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemNotaFiscal(ms, unitOfWork, out string guid);
                    if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemNotaFiscalColetaEntrega(clienteMultisoftware, codigoCargaEntrega, guid, data, unitOfWork, out string mensagemErro))
                        return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem da nota fiscal da coleta/entrega.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> AssinaturaProdutor(int clienteMultisoftware, int codigoCargaEntrega, string imagem, string dataEnvio)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);
                if (usuarioMobileCliente == null) return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    if (!DateTime.TryParseExact(dataEnvio, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                        return Retorno<bool>.CriarRetornoDadosInvalidos("A data de envio não esta em um formato correto (ddMMyyyyHHmmss);");

                    byte[] buffer = System.Convert.FromBase64String(imagem);
                    MemoryStream ms = new MemoryStream(buffer);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemAssinatura(ms, unitOfWork, out string guid);
                    if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarAssinaturaProdutorColetaEntrega(clienteMultisoftware, codigoCargaEntrega, guid, data, unitOfWork, out string mensagemErro))
                        return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a assinatura do produtor.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> FotoRecebedor(int clienteMultisoftware, int codigoCargaEntrega, string imagem, string dataEnvio)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();


                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);
                if (usuarioMobileCliente == null) return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                    if (cargaEntrega == null)
                    {
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Carga entrega não encontrada");
                    }

                    if (!DateTime.TryParseExact(dataEnvio, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                        return Retorno<bool>.CriarRetornoDadosInvalidos("A data de envio não esta em um formato correto (ddMMyyyyHHmmss);");

                    byte[] buffer = System.Convert.FromBase64String(imagem);
                    MemoryStream ms = new MemoryStream(buffer);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemRecebedor(ms, cargaEntrega, unitOfWork);
                    repCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a a foto do recebedor.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> GuiaTransporteAnimalColetaEntrega(int clienteMultisoftware, int codigoCargaEntrega, string codigoBarras, string numeroNF, string serie, string uf, int quantidade)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);
                if (usuarioMobileCliente == null) return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                try
                {
                    if (!Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarGTAColetaEntrega(codigoCargaEntrega, codigoBarras, numeroNF, serie, uf, quantidade, unitOfWork, out string mensagemErro))
                    {
                        return Retorno<bool>.CriarRetornoDadosInvalidos(mensagemErro);
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    throw;
                }
                finally
                {
                    unitOfWork.Dispose();
                    unitOfWork = null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem do monitoramento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> EnviarImagem(int clienteMultisoftware, int codigoCargaEntrega, string imagem, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                {
                    Servicos.Log.TratarErro("EnviarImagem: Sessão expirada");
                    return Retorno<bool>.CriarRetornoSessaoExpirada();
                }

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente == null)
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    DateTime.TryParseExact(coordenada?.dataCoordenada, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

                    byte[] buffer = System.Convert.FromBase64String(imagem);
                    MemoryStream ms = new MemoryStream(buffer);

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                        Usuario = ObterUsuario(unitOfWork, usuarioMobile.CPF)
                    };

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(ms, unitOfWork, out string tokenImagem);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(clienteMultisoftware, codigoCargaEntrega, tokenImagem, unitOfWork, data, coordenada?.latitude, coordenada?.longitude, OrigemSituacaoEntrega.App, true, auditado);

                    //Servicos.Log.TratarErro("EnviarImagem: Imagem salva com sucesso");
                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                Servicos.Log.TratarErro("EnviarImagem => " + Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    clienteMultisoftware,
                    codigoCargaEntrega,
                    imagem,
                    coordenada
                }));
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem do monitoramento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>> ObterTiposEventos(string clienteMultisoftware)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                int.TryParse(clienteMultisoftware, out int codigoCliente);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, codigoCliente);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                        List<Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento> monitoramentoEventos = repMonitoramentoEvento.BuscarTodosMobile();

                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento> tiposEvento =
                        (from obj in monitoramentoEventos
                         select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento()
                         {
                             Codigo = obj.Codigo,
                             Descricao = obj.Descricao
                         }).ToList();

                        return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>>.CriarRetornoSucesso(tiposEvento);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.TipoEvento>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar os motivos de rejeição de coleta.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarEvento(int codigoCarga, int codigoTipoEvento, int clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Coordenada coordenada, string observacao)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(codigoTipoEvento, false);

                        if (monitoramentoEvento != null)
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                            DateTime dataEvento = DateTime.Now.AddMilliseconds(-coordenada.diferencaEmMilissegundos);

                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarAlerta(monitoramentoEvento.TipoAlerta, monitoramentoEvento, (decimal)coordenada.latitude, (decimal)coordenada.longitude, dataEvento, observacao, carga, unitOfWork);

                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        else
                            return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um tipo de evento para o código informado.");

                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar o inicio do evento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> FinalizarEvento(int codigoCarga, int codigoTipoEvento, int clienteMultisoftware, long milisegundosEvento, string observacao)
        {
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<bool>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);


                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    try
                    {
                        Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                        Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarPorCodigo(codigoTipoEvento, false);

                        if (monitoramentoEvento != null)
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                            Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);
                            Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCarga(unitOfWork);
                            List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarEmAbertoPorCarga(carga.Codigo, monitoramentoEvento.TipoAlerta);

                            unitOfWork.Start();


                            foreach (Dominio.Entidades.Embarcador.Logistica.AlertaMonitor alerta in alertas)
                            {
                                alerta.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.AlertaMonitorStatus.Finalizado;
                                alerta.DataFim = alerta.Data.AddMilliseconds(milisegundosEvento);
                                if (!string.IsNullOrWhiteSpace(observacao))
                                    alerta.Observacao = observacao;

                                repAlertaMonitor.Atualizar(alerta);
                                servAlertaAcompanhamentoCarga.AtualizarTratativaAlertaAcompanhamentoCarga(alerta, null);
                            }

                            unitOfWork.CommitChanges();
                            return Retorno<bool>.CriarRetornoSucesso(true);
                        }
                        else
                            return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado um tipo de evento para o código informado.");

                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        throw;
                    }
                    finally
                    {
                        unitOfWork.Dispose();
                        unitOfWork = null;
                    }
                }
                else
                    return Retorno<bool>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao informar o inicio do evento.");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<RecebedoresAutorizadosPorDestinatario>> ObterDadosReconhecimentoFacialRecebedor(int clienteMultisoftware, int codigoCarga)
        {

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);
                if (usuarioMobile == null)
                    return Retorno<List<RecebedoresAutorizadosPorDestinatario>>.CriarRetornoSessaoExpirada();

                AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente repUsuarioMobileCliente = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobileCliente(unitOfWorkAdmin);
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = repUsuarioMobileCliente.BuscarPorUsuarioECliente(usuarioMobile.Codigo, clienteMultisoftware);

                if (usuarioMobileCliente != null)
                {
                    using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                    {
                        try
                        {
                            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                            Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado repPessoaRecebedorAutorizado = new Repositorio.Embarcador.Pessoas.PessoaRecebedorAutorizado(unitOfWork);
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                            if (carga == null)
                            {
                                throw new WebServiceException("Carga não encontrada");
                            }

                            // Obtém todos os destinatários
                            List<Dominio.Entidades.Cliente> listaDestinatarios = new List<Dominio.Entidades.Cliente> { };
                            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repCargaEntrega.BuscarPorCarga(carga.Codigo);
                            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
                            {
                                if (!cargaEntrega.Coleta)
                                {
                                    listaDestinatarios.Add(cargaEntrega.Cliente);
                                }
                            }

                            List<RecebedoresAutorizadosPorDestinatario> listaDestinatariosComRecebedores = new List<RecebedoresAutorizadosPorDestinatario> { };

                            // Para cada destinatário que tem nas cargaEntregas do tipo !coleta, criar um objeto com a lista dos recebedores.
                            foreach (Dominio.Entidades.Cliente destinatario in listaDestinatarios)
                            {
                                List<Dominio.Entidades.Embarcador.Pessoas.PessoaRecebedorAutorizado> recebedoresAutorizados = repPessoaRecebedorAutorizado.BuscarPorPessoa(destinatario.CPF_CNPJ);

                                listaDestinatariosComRecebedores.Add(new RecebedoresAutorizadosPorDestinatario
                                {
                                    CodigoDestinatario = destinatario.Codigo,
                                    NomeDestinatario = destinatario.Nome,
                                    RecebedoresAutorizados = (from o in recebedoresAutorizados
                                                              select new RecebedorAutorizado
                                                              {
                                                                  Nome = o.Nome,
                                                                  CPF = o.CPF,
                                                                  Foto = ObterBase64Imagem(Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Cliente", "FotoRecebedor" }), o.GuidFoto + ".jpg"))
                                                              }).ToList()
                                });
                            }

                            return Retorno<List<RecebedoresAutorizadosPorDestinatario>>.CriarRetornoSucesso(listaDestinatariosComRecebedores);
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {
                            unitOfWork.Dispose();
                        }
                    }
                }
                else
                    return Retorno<List<RecebedoresAutorizadosPorDestinatario>>.CriarRetornoSessaoNaoVinculadaAoClienteMultisoftware();
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<RecebedoresAutorizadosPorDestinatario>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<RecebedoresAutorizadosPorDestinatario>>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar viagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterBase64Imagem(string caminho)
        {
            if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminho);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }
        }

        #endregion
    }
}
