using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Entregas(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IEntregas
    {
        #region Métodos Globais

        public Retorno<bool> ConfirmarEntregaPedido(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo, string dataEntrega, Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint pontoEntrega)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            retorno.Status = true;
            try
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCargaEPedido(protocolo.protocoloIntegracaoCarga, protocolo.protocoloIntegracaoPedido);
                if (cargaPedido != null)
                {

                    DateTime data;
                    if (!DateTime.TryParseExact(dataEntrega, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de entrega não está em um formato correto (dd/MM/yyyy HH:mm:ss); ";
                    }
                    else
                    {
                        Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido cargaEntregaPedido = repCargaEntregaPedido.BuscarPorCargaPedido(cargaPedido.Codigo);
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                        Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService confiWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                        if (cargaEntregaPedido != null)
                        {
                            if (cargaEntregaPedido.CargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                            {
                                if (confiWebService?.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento ?? false)
                                {
                                    List<SituacaoCarga> listaSituacaoCargaEmAberto = SituacaoCargaHelper.ObterSituacoesCargaPermiteAtualizar();
                                    if (listaSituacaoCargaEmAberto.Contains(cargaEntregaPedido.CargaEntrega.Carga.SituacaoCarga))
                                    {
                                        Servicos.Log.TratarErro($"ConfirmarEntregaPedido - Carga pendente de avanço da etapa de frete para confirmação da entrega");
                                        return Retorno<bool>.CriarRetornoDadosInvalidos("Carga pendente de avanço da etapa de frete para confirmação da entrega");
                                    }
                                }

                                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                                Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntregaPedido.CargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                                unitOfWork.Start();
                                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntregaPedido.CargaEntrega, data, pontoEntrega, null, 0, "", configuracao, TipoServicoMultisoftware, Auditado, OrigemSituacaoEntrega.WebService, this.Cliente, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntregaPedido.CargaEntrega, "Confirmou a entrega.", unitOfWork);
                                unitOfWork.CommitChanges();
                            }
                            else
                            {
                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DuplicidadeDaRequisicao;
                            }
                        }
                        else
                        {
                            retorno.Status = false;
                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                            retorno.Mensagem = "Não foi gerado o controle de entrega para o pedido " + cargaPedido.Pedido.Numero + " na carga (" + cargaPedido.Carga.CodigoCargaEmbarcador + ")";
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Nenhum pedido foi encontrado para os protocolos informados";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "A carga informada não existe no Multi Embarcador";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            retorno.Objeto = retorno.Status;
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> BuscarEntregasRealizadasPendentesIntegracao(int? inicio, int? limite)
        {
            Servicos.Log.TratarErro($"BuscarEntregasRealizadasPendentesIntegracao Inicio", "BuscarEntregasRealizadasPendentesIntegracao");

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Servicos.Log.TratarErro($"Token ok", "BuscarEntregasRealizadasPendentesIntegracao");
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto repositorioConfiguracaoCanhoto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoCanhoto(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoCanhoto configuracaoCanhoto = repositorioConfiguracaoCanhoto.BuscarConfiguracaoPadrao();

                    Servicos.WebService.Entrega.Entrega servicioEntrega = new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "");

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega = new List<SituacaoEntrega>();
                    situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue);
                    if (configuracaoWebService?.RetornarEntregasRejeitadas ?? false)
                    {
                        situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado);
                        situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue);
                    }

                    Servicos.Log.TratarErro($"Buscando Entregas Pendentes.. ", "BuscarEntregasRealizadasPendentesIntegracao");

                    bool retornarSomenteCanhotoDigitalizado = configuracaoCanhoto.RetornarMetodoBuscarEntregasRealizadasPendentesIntegracaoSomenteCanhotoDigitalizado;

                    IList<int> codigosCargaEntregas = new List<int>();

                    if (retornarSomenteCanhotoDigitalizado)
                    {
                        codigosCargaEntregas = repCargaEntrega.BuscarCodigosEntregasPendentesIntegracaoSomenteDigitalizados(situacoesEntrega);
                        codigosCargaEntregas = codigosCargaEntregas.Skip((int)inicio).Take((int)limite).ToList();
                    }
                    else
                        codigosCargaEntregas = repCargaEntrega.BuscarCodigosEntregasPendentesIntegracao((int)inicio, (int)limite, situacoesEntrega);


                    Servicos.Log.TratarErro($"Entregas Encontradas: {codigosCargaEntregas.Count} ", "BuscarEntregasRealizadasPendentesIntegracao");

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas(codigosCargaEntregas);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>()
                    {
                        Itens = new List<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>(),
                        NumeroTotalDeRegistro = retornarSomenteCanhotoDigitalizado ? repCargaEntrega.ContarCodigosEntregasPendentesIntegracaoSomenteDigitalizados(situacoesEntrega) : repCargaEntrega.ContarPendentesIntegracao(situacoesEntrega)
                    };

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargasEntregas(codigosCargaEntregas);

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCodigos(codigosCargaEntregas);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> correnciasColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCodigosEntregas((from obj in cargaEntregas select obj.Codigo).ToList());

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                        retorno.Objeto.Itens.Add(servicioEntrega.ObterEntrega(cargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos, correnciasColetaEntrega, unitOfWork));

                    Servicos.Log.TratarErro($"Retorno: {Newtonsoft.Json.JsonConvert.SerializeObject(retorno.Objeto)} ", "BuscarEntregasRealizadasPendentesIntegracao");

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entregas pendentes de integração", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as entregas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarEntregaRealizadasPendentesIntegracao(int ProtocoloEntrega)
        {
            ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
            retorno.Status = true;
            retorno.Objeto = true;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(ProtocoloEntrega);
                if (cargaEntrega != null)
                {
                    if (!cargaEntrega.IntegradoERP)
                    {
                        cargaEntrega.IntegradoERP = true;
                        repCargaEntrega.Atualizar(cargaEntrega);
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, "Confirmou integração da entrega.", unitOfWork);
                    }
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O protocolo informado não existe no Multi Embarcador";
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao processar a confirmação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.EDI.AGRO.AGRO> BuscarDadosColetaAGRO(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.EDI.AGRO.AGRO> retorno = new Retorno<Dominio.ObjetosDeValor.EDI.AGRO.AGRO>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);
                if (carga == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "não foi encontrada nenhuma carga para o protocolo informado.";
                }
                else if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "a carga informada ainda não está finalizada.";
                }
                else
                {
                    Servicos.Embarcador.Integracao.EDI.AGRO serAgro = new Servicos.Embarcador.Integracao.EDI.AGRO();
                    Dominio.ObjetosDeValor.EDI.AGRO.AGRO agro = serAgro.ConverterCargaEDIIntegracaoParaAGRO(carga, unitOfWork);
                    retorno.Objeto = agro;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entregas pendentes de integração", unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os dados AGRO";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.EDI.GTA.SUINO> BuscarDadosColetaSUINO(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.EDI.GTA.SUINO> retorno = new Retorno<Dominio.ObjetosDeValor.EDI.GTA.SUINO>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);
                if (carga == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "não foi encontrada nenhuma carga para o protocolo informado.";
                }
                else if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada && carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.EmTransporte)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "a carga informada ainda não está finalizada.";
                }
                else
                {
                    Servicos.Embarcador.Integracao.EDI.GTA serGta = new Servicos.Embarcador.Integracao.EDI.GTA();
                    Dominio.ObjetosDeValor.EDI.GTA.SUINO agro = serGta.ConverterCargaEDIIntegracaoParaSUINO(carga, unitOfWork);
                    retorno.Objeto = agro;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entregas pendentes de integração", unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os dados de suínos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.EDI.GTA.AVES> BuscarDadosColetaAVES(int protocoloCarga)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.EDI.GTA.AVES> retorno = new Retorno<Dominio.ObjetosDeValor.EDI.GTA.AVES>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };
            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(protocoloCarga);
                if (carga == null)
                {
                    throw new WebServiceException("Não foi encontrada nenhuma carga para o protocolo informado.");
                }
                else if (carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Encerrada)
                {
                    throw new WebServiceException("A carga informada ainda não está finalizada");
                }
                else
                {
                    Servicos.Embarcador.Integracao.EDI.GTA serGta = new Servicos.Embarcador.Integracao.EDI.GTA();
                    Dominio.ObjetosDeValor.EDI.GTA.AVES aves = serGta.ConverterCargaEDIIntegracaoParaAVES(carga, unitOfWork);
                    retorno.Objeto = aves;
                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entregas pendentes de integração", unitOfWork);
                }
            }
            catch (WebServiceException ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os dados de Aves";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>> BuscarOcorrenciasColetaEntregaPendentesIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (limite > 100)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>>.CriarRetornoDadosInvalidos("O limite não pode ser superior a 100");

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                int totalRegistros = repositorioPedidoOcorrenciaColetaEntrega.ContarConsultaPendentesIntegracao();
                List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega> ocorrenciasRetornar = new List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>();

                if (totalRegistros > 0)
                {
                    Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ocorrenciasColetaEntrega = repositorioPedidoOcorrenciaColetaEntrega.ConsultarPendentesIntegracao((int)inicio, (int)limite);
                    List<int> codigosPedido = (from o in ocorrenciasColetaEntrega select o.Pedido.Codigo).Distinct().ToList();
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> notasFiscaisPedidos = repositorioPedidoXMLNotaFiscal.BuscarPorPedidos(codigosPedido);

                    foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrenciaColetaEntrega in ocorrenciasColetaEntrega)
                    {
                        Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega ocorrenciaColetaEntregaRetornar = new Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega()
                        {
                            CpfCnpjCliente = ocorrenciaColetaEntrega.Alvo?.CPF_CNPJ_SemFormato ?? "",
                            Data = ocorrenciaColetaEntrega.DataOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                            NotasFiscais = new List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntregaNotaFiscal>(),
                            Protocolo = ocorrenciaColetaEntrega.Codigo,
                            Tipo = ocorrenciaColetaEntrega.TipoDeOcorrencia.CodigoProceda ?? ""
                        };

                        List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais = (from o in notasFiscaisPedidos where o.CargaPedido.Pedido.Codigo == ocorrenciaColetaEntrega.Pedido.Codigo select o.XMLNotaFiscal).ToList();

                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                        {
                            ocorrenciaColetaEntregaRetornar.NotasFiscais.Add(new Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntregaNotaFiscal()
                            {
                                Chave = notaFiscal.Chave
                            });
                        }

                        ocorrenciasRetornar.Add(ocorrenciaColetaEntregaRetornar);
                    }
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega> paginacaoOcorrenciasColetaEntrega = new Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>()
                {
                    NumeroTotalDeRegistro = totalRegistros,
                    Itens = ocorrenciasRetornar
                };

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>>.CriarRetornoSucesso(paginacaoOcorrenciasColetaEntrega);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaColetaEntrega>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as ocorrências de coleta/entrega");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarOcorrenciaColetaEntregaPendenteIntegracao(int protocoloOcorrenciaColetaEntrega)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                if (protocoloOcorrenciaColetaEntrega <= 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo da ocorrência de coleta/entrega deve ser informado");

                unitOfWork.Start();

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = repositorioPedidoOcorrenciaColetaEntrega.BuscarPorCodigo(protocoloOcorrenciaColetaEntrega, auditavel: false);

                if (pedidoOcorrenciaColetaEntrega == null)
                    throw new WebServiceException("Não foi possível encontrar a ocorrência de coleta/entrega com o protocolo informado");

                if (pedidoOcorrenciaColetaEntrega.PendenteIntegracaoERP)
                {
                    pedidoOcorrenciaColetaEntrega.PendenteIntegracaoERP = false;
                    repositorioPedidoOcorrenciaColetaEntrega.Atualizar(pedidoOcorrenciaColetaEntrega);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, pedidoOcorrenciaColetaEntrega, "Confirmou a ocorrência de coleta/entrega.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a ocorrência de coleta/entrega");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga> ConsultaSituacaoNotaFiscal(Dominio.ObjetosDeValor.WebService.Entrega.ConsultaSituacaoNotaFiscal consultaSituacaoNota)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            AdminMultisoftware.Repositorio.UnitOfWork adminUnitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);

            Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true,
                DataRetorno = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")
            };

            try
            {
                if (consultaSituacaoNota.NumeroNota <= 0 || string.IsNullOrEmpty(consultaSituacaoNota.SerieNota))
                    return Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga>.CriarRetornoDadosInvalidos("Número Nota e Série são Obrigatórios");

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

                double cpfcnpj = Utilidades.String.OnlyNumbers(consultaSituacaoNota.CNPJEmissor)?.ToDouble() ?? 0;
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> listaCargaEntregaNota = repositorioCargaEntregaNotaFiscal.BuscarPorNumeroNotaFiscalESerie(consultaSituacaoNota.NumeroNota, consultaSituacaoNota.SerieNota, cpfcnpj);

                if (listaCargaEntregaNota != null && listaCargaEntregaNota.Count > 0)
                {
                    //Ultima carga da nota
                    Dominio.Entidades.Embarcador.Cargas.Carga ultimaCargaNota = listaCargaEntregaNota.Select(obj => obj.CargaEntrega.Carga).OrderBy(obj => obj.DataCriacaoCarga).LastOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = listaCargaEntregaNota.Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).FirstOrDefault();
                    Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido = listaCargaEntregaNota.Select(obj => obj.PedidoXMLNotaFiscal.CargaPedido.Pedido).OrderBy(obj => obj.Codigo).LastOrDefault();

                    Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga situacaoNotaFiscalCarga = ConverterCargaEmRetornoNotaFiscalCarga(ultimaCargaNota, xmlNotaFiscal, Pedido, unitOfWork, adminUnitOfWork, Conexao.createInstance(_serviceProvider).StringConexao);
                    retorno.Objeto = situacaoNotaFiscalCarga;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou SituacaoNotaFiscalCarga Numero: " + consultaSituacaoNota.NumeroNota + " Serie: " + consultaSituacaoNota.SerieNota + " CNPJ: " + consultaSituacaoNota.CNPJEmissor, unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.RegistroIndisponivel;
                    retorno.Status = false;
                    retorno.Mensagem = "Não foram encontradas referências a este Número de Nota e série.";
                    return retorno;
                }
            }
            catch (WebServiceException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga>.CriarRetornoExcecao("Ocorreu uma falha ao buscar situação da Nota Fiscal");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga>.CriarRetornoExcecao("Ocorreu uma falha ao buscar situação da Nota Fiscal");
            }
            finally
            {
                adminUnitOfWork.Dispose();
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloCarga(int protocoloCarga)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> retorno = new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConsultarDetalhesEntregaPorProtocoloCarga(protocoloCarga);
                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>.CreateFrom(retorno.Objeto)
                };
            });

        }

        public Retorno<bool> EnviarMensagemChatEntrega(Dominio.ObjetosDeValor.WebService.Entrega.MensagemChat mensagemChat)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<bool> retorno = new Retorno<bool>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true,
                Objeto = true,
            };
            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens servAlertaAcompanhamentoCarga = new Servicos.Embarcador.TorreControle.AlertaAcompanhamentoCargaMensagens();

                if (mensagemChat.NotaFiscal?.Numero == null)
                {
                    throw new WebServiceException("É necessário enviar o número da nota fiscal");
                }

                if (mensagemChat.NotaFiscal?.Serie == null)
                {
                    throw new WebServiceException("É necessário enviar a série da nota fiscal");
                }

                double cpfCnpjDouble = 0;
                if (mensagemChat.NotaFiscal?.Emitente?.CPFCNPJ == null || !double.TryParse(mensagemChat.NotaFiscal?.Emitente?.CPFCNPJ, out cpfCnpjDouble))
                {
                    throw new WebServiceException("É necessário enviar o CPF ou CNPJ do emitente da nota fiscal");
                }

                Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal = repXmlNotaFiscal.BuscarPorNumeroSerieEmitente(mensagemChat.NotaFiscal.Numero, mensagemChat.NotaFiscal.Serie, cpfCnpjDouble);

                if (notaFiscal == null)
                {
                    throw new WebServiceException("Nota fiscal não encontrada");
                }

                // Procura a CargaEntrega
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorNotaFiscal(notaFiscal.Codigo);
                List<SituacaoEntrega> situacoesPodeMandarMensagem = new List<SituacaoEntrega> { SituacaoEntrega.NaoEntregue };
                if (cargaEntrega == null || !situacoesPodeMandarMensagem.Contains(cargaEntrega.Situacao))
                {
                    throw new WebServiceException("Nenhuma entrega ativa para essa nota fiscal");
                }

                Dominio.Entidades.Usuario usuario = null;
                // Procura o usuário
                string cpf = mensagemChat.Usuario.CPF.Replace(".", "").Replace("-", "");
                if (!string.IsNullOrWhiteSpace(cpf))
                    usuario = repUsuario.BuscarPorCPF(cpf);

                if (!string.IsNullOrWhiteSpace(mensagemChat.Usuario.CodigoIntegracao))
                    usuario = repUsuario.BuscarPorCodigoIntegracao(mensagemChat.Usuario.CodigoIntegracao);

                if (usuario == null)
                {
                    throw new WebServiceException("Usuário não encontrado");
                }

                // Mandando a mensagem
                Servicos.Embarcador.Chat.ChatMensagem.EnviarMensagemChat(
                                cargaEntrega.Carga,
                                usuario,
                                DateTime.Now,
                                cargaEntrega.Carga.Motoristas.ToList(),
                                mensagemChat.Mensagem,
                                ClienteAcesso.Cliente.Codigo,
                                unitOfWork, null
                );

                servAlertaAcompanhamentoCarga.informarAtualizacaoMensagensCard();

            }
            catch (WebServiceException ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Objeto = false;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os dados AGRO";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>> BuscarProtocolosEntregasPendentesIntegracao(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega> situacoesEntrega = new List<SituacaoEntrega>();
                    situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue);
                    if (configuracaoWebService?.RetornarEntregasRejeitadas ?? false)
                    {
                        situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado);
                        situacoesEntrega.Add(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Reentergue);
                    }

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPendentesIntegracao((int)inicio, (int)limite, situacoesEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas((from obj in cargaEntregas select obj.Codigo).ToList());

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>()
                    {
                        Itens = new List<Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega>(),
                        NumeroTotalDeRegistro = repCargaEntrega.ContarPendentesIntegracao(situacoesEntrega)
                    };

                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega entrega = new Dominio.ObjetosDeValor.WebService.Carga.CargaEntrega();

                        entrega.ProtocoloEntrega = cargaEntrega.Codigo;
                        entrega.ProtocoloCarga = cargaEntrega.Carga.Protocolo;
                        entrega.CodigoFilialCarga = cargaEntrega.Carga.Filial?.CodigoFilialEmbarcador;

                        retorno.Objeto.Itens.Add(entrega);
                    }

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou protocolos entregas pendentes de integração", unitOfWork);
                }
                else
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as entregas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> ConsultarDetalhesEntregaPorProtocoloEntrega(int protocoloEntrega)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>>()
            {
                Mensagem = "",
                CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso,
                Status = true
            };

            try
            {

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
                Servicos.WebService.Entrega.Entrega servicioEntrega = new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "");
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(protocoloEntrega);

                if (cargaEntrega == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "Não encontrado entregas para protocolo " + protocoloEntrega.ToString();
                }
                else
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>()
                    {
                        Itens = new List<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.Entrega>(),
                        NumeroTotalDeRegistro = 1
                    };

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargaEntrega(protocoloEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntrega(protocoloEntrega);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> correnciasColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCodigosEntregas(new List<int> { protocoloEntrega });

                    retorno.Objeto.Itens.Add(servicioEntrega.ObterEntrega(cargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos, correnciasColetaEntrega, unitOfWork));

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entregas pendentes de integração", unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as entregas";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.OcorrenciaEntrega ocorrencia)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {

                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if ((ocorrencia.TipoOcorrencia == null) || string.IsNullOrWhiteSpace(ocorrencia.TipoOcorrencia.CodigoIntegracao))
                    throw new WebServiceException("É obrigatório informar o tipo da Ocorrência.");

                Repositorio.TipoDeOcorrenciaDeCTe repOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
                Repositorio.Global.EmpresaIntelipostIntegracao repEmpresaIntelipostIntegracao = new Repositorio.Global.EmpresaIntelipostIntegracao(unitOfWork);
                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Servicos.Embarcador.CargaOcorrencia.Ocorrencia serOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia();
                Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService confiWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorUsuarioMultisoftware();

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = null;

                Dominio.Entidades.Empresa empresa = integradora.Empresa;

                if (empresa == null)
                    throw new WebServiceException("O token informado não pertence a nenhum transportador");

                Repositorio.Global.EmpresaIntelipostTipoOcorrencia repEmpresaIntelipostTipoOcorrencia = new Repositorio.Global.EmpresaIntelipostTipoOcorrencia(unitOfWork);
                Dominio.Entidades.EmpresaIntelipostTipoOcorrencia empresaIntelipostTipoOcorrencia = repEmpresaIntelipostTipoOcorrencia.BuscarPorEmpresaECodigoIntegracao(empresa.Codigo, ocorrencia.TipoOcorrencia.CodigoIntegracao);
                tipoDeOcorrencia = empresaIntelipostTipoOcorrencia?.TipoOcorrencia;

                if (tipoDeOcorrencia == null)
                    tipoDeOcorrencia = repOcorrenciaDeCTe.BuscarPorCodigoIntegracao(ocorrencia.TipoOcorrencia.CodigoIntegracao);

                if (tipoDeOcorrencia == null)
                    throw new WebServiceException($"Não existe um tipo de ocorrência cadastrado ou ativo na base Multisoftware para o código de integração {ocorrencia.TipoOcorrencia.CodigoIntegracao}.");

                if (tipoDeOcorrencia.OrigemOcorrencia != OrigemOcorrencia.PorCarga)
                    throw new WebServiceException("O tipo da ocorrência informada não é uma ocorrência por carga e não pode ser criado via WebService.");

                if (ocorrencia.NotaFiscal == null)
                    throw new WebServiceException("É obrigatório informar a nota fiscal");

                DateTime data;

                if (!string.IsNullOrWhiteSpace(ocorrencia.DataOcorrencia))
                {
                    if (!DateTime.TryParseExact(ocorrencia.DataOcorrencia, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out data))
                        throw new WebServiceException("A data da ocorrência não está em um formato correto(dd/MM/yyyy HH:mm:ss)");
                }
                else
                    throw new WebServiceException("É obrigatório informar a data da ocorrência.");

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = null;

                if (!string.IsNullOrWhiteSpace(ocorrencia.NotaFiscal.Chave))
                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorChaveTransportador(ocorrencia.NotaFiscal.Chave, empresa.Codigo);
                else
                {
                    if (ocorrencia.NotaFiscal.Emitente == null || ocorrencia.NotaFiscal.Destinatario == null)
                        throw new WebServiceException("É obrigatório informar o emitente e o destinatário da nota fiscal");

                    double cnpjRemetente = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.NotaFiscal.Emitente.CPFCNPJ), out cnpjRemetente);

                    double cnpjDestinatario = 0;
                    double.TryParse(Utilidades.String.OnlyNumbers(ocorrencia.NotaFiscal.Destinatario.CPFCNPJ), out cnpjDestinatario);

                    pedidoXMLNotaFiscal = repPedidoXMLNotaFiscal.BuscarPorNumeroNotaEmitenteTransportador(ocorrencia.NotaFiscal.Numero, empresa.Codigo, cnpjRemetente, cnpjDestinatario);
                }

                if (pedidoXMLNotaFiscal == null)
                    throw new WebServiceException("Não foi localizada uma nota fiscal compatível com os dados informados para gerar a ocorrência");

                Dominio.Entidades.Embarcador.Cargas.Carga carga = pedidoXMLNotaFiscal.CargaPedido.Carga;
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> pedidoXMLNotaFiscaisCarga = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>() { pedidoXMLNotaFiscal };
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if (confiWebService?.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento ?? false)
                {
                    List<SituacaoCarga> listaSituacaoCargaEmAberto = SituacaoCargaHelper.ObterSituacoesCargaPermiteAtualizar();
                    if (listaSituacaoCargaEmAberto.Contains(carga.SituacaoCarga))
                    {
                        Servicos.Log.TratarErro($"AdicionarOcorrencia - Carga pendente de avanço da etapa de frete para confirmação da entrega");
                        return Retorno<bool>.CriarRetornoDadosInvalidos("Carga pendente de avanço da etapa de frete para confirmação da entrega");
                    }
                }

                unitOfWork.Start();

                bool retorno = serOcorrencia.GerarPedidoOcorrenciaNotas(pedidoXMLNotaFiscaisCarga, tipoDeOcorrencia, carga, data, ocorrencia.Observacao, ocorrencia.Latitude, ocorrencia.Longitude, OrigemSituacaoEntrega.WebService, ocorrencia.Pacote?.NumeroPacote ?? "", ocorrencia.Volumes, TipoServicoMultisoftware, configuracaoEmbarcador, usuario, Cliente, unitOfWork, Auditado);

                if (!retorno)
                    throw new WebServiceException("Ocorrência enviada anteriormente para esse pacote", errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();

                if (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RequisicaoDuplicada)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao(excecao.Message);

                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar a Ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> EnviarImagensEntrega(Dominio.ObjetosDeValor.WebService.Entrega.EnvioImagemEntrega envioImagemEntrega)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            try
            {
                if (string.IsNullOrWhiteSpace(envioImagemEntrega.NotaFiscal?.Chave))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar a nota fiscal");

                if (string.IsNullOrWhiteSpace(envioImagemEntrega.Imagem))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("É obrigatório informar a imagem");

                Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoXMLNotaFiscal = repositorioPedidoXMLNotaFiscal.BuscarPorChaveTransportador(envioImagemEntrega.NotaFiscal.Chave, integradora?.Empresa?.Codigo ?? 0);
                if (pedidoXMLNotaFiscal == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizada uma nota fiscal compatível com os dados informados.");

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarEntregaPorCargaPedido(pedidoXMLNotaFiscal.CargaPedido.Codigo);
                if (cargaEntrega == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Entrega não foi localizada.");

                byte[] data = System.Convert.FromBase64String(envioImagemEntrega.Imagem);
                MemoryStream arquivo = new MemoryStream(data);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(arquivo, unitOfWork, out string tokenImagem);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(1, cargaEntrega.Codigo, tokenImagem, unitOfWork, DateTime.Now, 0, 0, OrigemSituacaoEntrega.WebService, false, Auditado);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao salvar a imagem da entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>> BuscarImagensEntregaPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite > 10)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 10.");

                List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega> listaImagensEntrega = new List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);

                int totalRegistros = repositorioCargaEntregaFoto.ContarConsultaImagensEntregaPendentesIntegracao();

                if (totalRegistros > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> cargaEntregaFotos = repositorioCargaEntregaFoto.ConsultarImagensEntregaPendentesIntegracao("Codigo", "asc", (int)inicio, (int)limite);
                    listaImagensEntrega.AddRange(ObterImagensEntrega(cargaEntregaFotos, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>()
                {
                    Itens = listaImagensEntrega,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Imagens Entregas Pendentes de Integração", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar as imagens das entregas pendentes de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoImagemEntrega(int protocoloImagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto repositorioCargaEntregaFoto = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto = repositorioCargaEntregaFoto.BuscarPorCodigo(protocoloImagem);

                if (cargaEntregaFoto == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizada uma imagem de entrega para o protocolo informado.");

                if (cargaEntregaFoto.Integrado)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Imagem da entrega já foi integrada.");

                cargaEntregaFoto.Integrado = true;
                repositorioCargaEntregaFoto.Atualizar(cargaEntregaFoto);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração da imagem da entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>> BuscarImagensOcorrenciaPendentesIntegracao(int? inicio, int? limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite > 10)
                    return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>>.CriarRetornoDadosInvalidos("O limite não pode ser maior que 10.");

                List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia> listaRetorno = new List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>();

                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);

                int totalRegistros = repAnexo.ContarConsultaImagensOcorrenciaPendentesIntegracao();

                if (totalRegistros > 0)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> listaAnexo = repAnexo.ConsultarImagensOcorrenciaPendentesIntegracao("Codigo", "asc", (int)inicio, (int)limite);
                    listaRetorno.AddRange(ObterImagensOcorrencia(listaAnexo, unitOfWork));
                }

                Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>()
                {
                    Itens = listaRetorno,
                    NumeroTotalDeRegistro = totalRegistros
                };

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou Imagens Entregas Pendentes de Integração", unitOfWork);

                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>>.CriarRetornoSucesso(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>>.CriarRetornoExcecao("Ocorreu uma falha ao buscar as imagens das entregas pendentes de integração.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarIntegracaoImagemOcorrencia(int protocoloImagem)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo = repAnexo.BuscarPorCodigo(protocoloImagem);

                if (anexo == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizada uma imagem de ocorrência para o protocolo informado.");

                if (anexo.Integrado)
                    return Retorno<bool>.CriarRetornoDuplicidadeRequisicao("Imagem da ocorrência já foi integrada.");

                anexo.Integrado = true;
                repAnexo.Atualizar(anexo);

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a integração da imagem da ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>> ObterEntregasPorPeriodo(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodo obterEntregasPorPeriodo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>>.CreateFrom(new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ObterEntregasPorPeriodo(obterEntregasPorPeriodo));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.ControleEntrega.ObterEntregasPorPeriodoResponse>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> AtualizarPrevisaoEntrega(Dominio.ObjetosDeValor.Embarcador.ControleEntrega.AtualizarPrevisaoEntrega atualizarPrevisaoEntrega)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Usuario repositorioUsuario = new Repositorio.Usuario(unitOfWork);

                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repositorioPedido.BuscarPorProtocolo(atualizarPrevisaoEntrega.protolocoPedido);

                Servicos.Embarcador.Notificacao.NotificacaoMTrack servicoNotificacaoMTrack = new Servicos.Embarcador.Notificacao.NotificacaoMTrack(unitOfWork);

                if (string.IsNullOrWhiteSpace(atualizarPrevisaoEntrega.dataPrevisaoEntrega))
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data de Previsão Entrega inválida ou não informada.");

                DateTime.TryParse(atualizarPrevisaoEntrega.dataPrevisaoEntrega, out DateTime dataPrevisaoEntrega);

                if (dataPrevisaoEntrega == DateTime.MinValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Data de Previsão Entrega inválida ou não informada.");


                if (pedido == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Protocolo do Pedido inválido ou não informado.");

                unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargasEntrega = repositorioCargaEntrega.BuscarCargaEntregaLiberadaPorPedido(pedido.Codigo);

                List<Dominio.Entidades.Usuario> motoristas = repositorioUsuario.BuscarTodosMotoristasAtivos(0, 50);

                pedido.PrevisaoEntrega = dataPrevisaoEntrega;

              


                repositorioPedido.Atualizar(pedido);

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargasEntrega)
                {
                    cargaEntrega.DataPrevista = dataPrevisaoEntrega;

                    repositorioCargaEntrega.Atualizar(cargaEntrega);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, null, string.Format(Localization.Resources.Cargas.Carga.AlterouDataPrevisaoEntrega, dataPrevisaoEntrega.ToDateTimeString()), unitOfWork);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega.Carga, null, string.Format(Localization.Resources.Cargas.Carga.AlterouDataPrevisaoEntregaCarga, dataPrevisaoEntrega.ToDateTimeString(), cargaEntrega.Cliente?.CPF_CNPJ_Formatado ?? "-"), unitOfWork);

                    servicoNotificacaoMTrack.NotificarMudancaCargaEntrega(cargaEntrega, motoristas, AdminMultisoftware.Dominio.Enumeradores.MobileHubs.EntregaPrevisaoAtualizada, true, Cliente.Codigo);
                }

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a data previsão de entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServicePedidos;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga ConverterCargaEmRetornoNotaFiscalCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notafiscal, Dominio.Entidades.Embarcador.Pedidos.Pedido Pedido, Repositorio.UnitOfWork unitOFWork, AdminMultisoftware.Repositorio.UnitOfWork UnitOfWorkAdmin, string stringConexao)
        {
            Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga ObjetoRetorno = new Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalCarga();

            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrencia = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOFWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOFWork);
            Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOFWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto repXMLNotaFiscalProduto = new Repositorio.Embarcador.Pedidos.XMLNotaFiscalProduto(unitOFWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregapedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOFWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOFWork);
            Servicos.WebService.Pessoas.Pessoa serPessoa = new Servicos.WebService.Pessoas.Pessoa(unitOFWork);

            //ocorrencias carga
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> OcorrenciaPedidoNota = repositorioPedidoOcorrencia.BuscarPorPedido(Pedido?.Codigo ?? 0);
            //Notas fiscais da Carga
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> NotasFiscaisDaCarga = repositorioNotaFiscal.BuscarPorCarga(carga.Codigo);
            //Monitoramento da Carga
            Dominio.Entidades.Embarcador.Logistica.Monitoramento MonitoramentoCarga = repositorioMonitoramento.BuscarUltimoPorCarga(carga.Codigo);

            ObjetoRetorno.CargaCodigoCargaEmbarcador = carga.CodigoCargaEmbarcador;
            ObjetoRetorno.CargaDescricaoOperacao = carga.TipoOperacao.Descricao;
            ObjetoRetorno.CargaDescricaoRota = carga.Rota?.Descricao ?? "";
            ObjetoRetorno.CargaProtocoloCarga = carga.Protocolo;
            ObjetoRetorno.CargaPeso = (decimal)carga.DadosSumarizados?.PesoTotal;
            ObjetoRetorno.CargaDataCriacao = carga.DataCriacaoCarga;
            ObjetoRetorno.CargaDataCarregamento = carga.DataCarregamentoCarga ?? null;
            ObjetoRetorno.CargaDataInicioViagem = carga.DataInicioViagem ?? null;
            ObjetoRetorno.CargaDataFimViagem = carga.DataFimViagem ?? null;
            ObjetoRetorno.CargaDataPrevisaoChegadaDestinatario = carga.DataPrevisaoChegadaOrigem ?? null;

            //VEICULO
            ObjetoRetorno.VeiculoPlaca = carga.Veiculo?.Placa ?? "";
            ObjetoRetorno.VeiculoTaraKg = carga.Veiculo?.Tara ?? 0;
            ObjetoRetorno.VeiculoCapacidadeKG = carga.Veiculo?.CapacidadeKG ?? 0;
            ObjetoRetorno.VeiculoTransportadora = $"{carga.Empresa.RazaoSocial} ({carga.Empresa.Localidade.DescricaoCidadeEstado})";

            //MOTORISTA
            Dominio.Entidades.Usuario Motorista = carga.Motoristas.LastOrDefault(); //ver qual pegar
            ObjetoRetorno.MotoristaCPF = Motorista?.CPF ?? "";
            ObjetoRetorno.MotoristaNome = Motorista?.Nome ?? "";
            ObjetoRetorno.MotoristaTelefone = Motorista?.Telefone_Formatado ?? "";

            //POSICAO
            ObjetoRetorno.PosicaoData = MonitoramentoCarga?.UltimaPosicao?.DataVeiculo ?? null;
            ObjetoRetorno.PosicaoLatitude = MonitoramentoCarga?.UltimaPosicao?.Latitude ?? 0D;
            ObjetoRetorno.PosicaoLongitude = MonitoramentoCarga?.UltimaPosicao?.Longitude ?? 0D;

            //OPERADOR
            ObjetoRetorno.OperadorNome = carga.Operador?.Nome ?? "";
            ObjetoRetorno.OperadorEmail = carga.Operador?.Email ?? "";
            ObjetoRetorno.OperadorLogin = carga.Operador?.Login ?? "";

            ObjetoRetorno.Remetente = serPessoa.ConverterObjetoPessoa(notafiscal.Emitente);
            ObjetoRetorno.Destinatario = serPessoa.ConverterObjetoPessoa(notafiscal.Destinatario);

            string linkOcorrencia = "";
            if (Pedido != null && !string.IsNullOrEmpty(Pedido.CodigoRastreamento))
            {
                string urlBase = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLBase(Cliente.Codigo, TipoServicoMultisoftware, UnitOfWorkAdmin, Conexao.createInstance(_serviceProvider).AdminStringConexao, unitOFWork);
                linkOcorrencia = Servicos.Embarcador.GestaoEntregas.NotificaoEntrega.ObterURLRastreamentoPedido(Pedido.CodigoRastreamento, urlBase);
            }

            ObjetoRetorno.LinkRastreio = linkOcorrencia;

            ObjetoRetorno.Notas = new List<Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalNotasCarga>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNota in NotasFiscaisDaCarga)
            {

                Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalNotasCarga notaCargaRetorno = new Dominio.ObjetosDeValor.WebService.Entrega.SituacaoNotaFiscalNotasCarga();
                notaCargaRetorno.Chave = xmlNota.Chave;
                notaCargaRetorno.CNPJEmitente = xmlNota.Emitente.CPF_CNPJ_SemFormato;
                notaCargaRetorno.CNPJRecebedor = xmlNota.Destinatario.CPF_CNPJ_SemFormato;
                notaCargaRetorno.DataEmissao = xmlNota.DataEmissao;
                notaCargaRetorno.Emitente = xmlNota.Emitente.NomeFantasia;
                notaCargaRetorno.Recebedor = xmlNota.NomeDestinatario;
                notaCargaRetorno.Numero = xmlNota.Numero;
                notaCargaRetorno.Serie = xmlNota.Serie;
                notaCargaRetorno.SituacaoNota = notafiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Nenhum ? SituacaoNotaFiscal.AgEntrega : notafiscal.SituacaoEntregaNotaFiscal;

                notaCargaRetorno.ProtocoloPedido = !string.IsNullOrEmpty(xmlNota.NumeroControlePedido) ? xmlNota.NumeroControlePedido : "0";
                notaCargaRetorno.NumeroPedidoEmbarcador = !string.IsNullOrEmpty(xmlNota.NumeroPedidoEmbarcador) ? xmlNota.NumeroPedidoEmbarcador : "0";

                List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto> produtosNota = repXMLNotaFiscalProduto.BuscarPorNotaFiscal(xmlNota.Codigo);
                notaCargaRetorno.Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto>();
                if (produtosNota.Count > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscalProduto produtoNota in produtosNota)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Pedido.Produto produto = new Dominio.ObjetosDeValor.Embarcador.Pedido.Produto();
                        produto.CodigoProduto = produtoNota.Produto?.CodigoProdutoEmbarcador ?? "";
                        produto.DescricaoProduto = produtoNota.Produto?.Descricao ?? "";
                        produto.CodigoGrupoProduto = produtoNota.Produto?.GrupoProduto?.CodigoGrupoProdutoEmbarcador ?? "";
                        produto.DescricaoGrupoProduto = produtoNota.Produto?.GrupoProduto?.Descricao ?? "";
                        produto.Quantidade = produtoNota.Quantidade;
                        notaCargaRetorno.Produtos.Add(produto);
                    }
                }

                ObjetoRetorno.Notas.Add(notaCargaRetorno);
            }

            ObjetoRetorno.Ocorrencias = new List<Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes>();
            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrencia in OcorrenciaPedidoNota)
            {
                Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes ocorrenciaRetorno = new Dominio.ObjetosDeValor.WebService.Entrega.OcorrenciaDetalhes();
                ocorrenciaRetorno.Data = ocorrencia.DataOcorrencia;
                ocorrenciaRetorno.Observacao = "";
                ocorrenciaRetorno.Tipo = ocorrencia.TipoDeOcorrencia.Descricao;
                ocorrenciaRetorno.Situacao = notafiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Nenhum ? SituacaoNotaFiscal.AgEntrega.ObterDescricao() : notafiscal.SituacaoEntregaNotaFiscal.ObterDescricao();

                ObjetoRetorno.Ocorrencias.Add(ocorrenciaRetorno);
            }

            return ObjetoRetorno;

        }

        private List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega> ObterImagensEntrega(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto> cargaEntregaFotos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
            Servicos.WebService.Entrega.Entrega servicioEntrega = new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "");
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

            List<int> codigosEntrega = cargaEntregaFotos.Select(o => o.CargaEntrega.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargasEntregas(codigosEntrega);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas(codigosEntrega);

            List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega> listaImagensEntrega = new List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega>();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> correnciasColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCodigosEntregas(codigosEntrega);

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaFoto cargaEntregaFoto in cargaEntregaFotos)
            {
                Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega imagemEntrega = new Dominio.ObjetosDeValor.WebService.Entrega.ImagemEntrega()
                {
                    ProtocoloImagem = cargaEntregaFoto.Codigo,
                    Imagem = servicoControleEntrega.ObterBase64ImagemEntrega(cargaEntregaFoto, unitOfWork),
                    Entrega = servicioEntrega.ObterEntrega(cargaEntregaFoto.CargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos, correnciasColetaEntrega, unitOfWork)
                };

                listaImagensEntrega.Add(imagemEntrega);
            }

            return listaImagensEntrega;
        }

        private List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia> ObterImagensOcorrencia(List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo> listaAnexo, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe repCargaEntregaChaveNfe = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            Servicos.WebService.Entrega.Entrega servicioEntrega = new Servicos.WebService.Entrega.Entrega(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, "");
            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega servicoControleEntrega = new Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega(unitOfWork);

            List<int> codigosEntrega = listaAnexo.Select(o => o.EntidadeAnexo.CargaEntrega.Codigo).ToList();
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaChaveNfe> listaCargaEntregaChaveNfe = repCargaEntregaChaveNfe.BuscarPorCargasEntregas(codigosEntrega);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregas(codigosEntrega);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> correnciasColetaEntrega = repOcorrenciaColetaEntrega.BuscarPorCodigosEntregas(codigosEntrega);
            List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia> listaImagensEntrega = new List<Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia>();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo anexo in listaAnexo)
            {
                Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia imagemEntrega = new Dominio.ObjetosDeValor.WebService.Entrega.ImagemOcorrencia()
                {
                    ProtocoloImagem = anexo.Codigo,
                    Imagem = servicoControleEntrega.ObterBase64ImagemOcorrencia(anexo, unitOfWork),
                    Entrega = servicioEntrega.ObterEntrega(anexo.EntidadeAnexo.CargaEntrega, listaCargaEntregaChaveNfe, cargaEntregaPedidos, correnciasColetaEntrega, unitOfWork)
                };

                listaImagensEntrega.Add(imagemEntrega);
            }

            return listaImagensEntrega;
        }

        #endregion
    }
}
