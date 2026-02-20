using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Ocorrencias" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Ocorrencias.svc or Ocorrencias.svc.cs at the Solution Explorer and start debugging.
    public class Ocorrencias(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IOcorrencias
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> BuscarTiposOcorrencia(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>();

                    List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposOcorrencia = repTipoOcorrencia.Consultar(true, true, (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repTipoOcorrencia.ContarConsulta(true, true);

                    foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia in tiposOcorrencia)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia dynTipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia();
                        dynTipoOcorrencia.CodigoIntegracao = tipoOcorrencia.CodigoIntegracao;
                        dynTipoOcorrencia.Descricao = tipoOcorrencia.Descricao;
                        retorno.Objeto.Itens.Add(dynTipoOcorrencia);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou tipos de ocorrências", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os tipo de Ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> BuscarMovivosAprovacaoOcorrencia(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>>();
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia repMotivoRejeicaoOcorrencia = new Repositorio.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia(unitOfWork);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>();
                    retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia>();

                    List<Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia> motivosRejeicaoOcorrencia = repMotivoRejeicaoOcorrencia.Consultar("", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AprovacaoRejeicao.Aprovacao, "", "", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repMotivoRejeicaoOcorrencia.ContarConsulta("", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AprovacaoRejeicao.Aprovacao);

                    foreach (Dominio.Entidades.Embarcador.Ocorrencias.MotivoRejeicaoOcorrencia motivo in motivosRejeicaoOcorrencia)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia dynTipoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.TipoOcorrencia();
                        dynTipoOcorrencia.CodigoIntegracao = motivo.Codigo.ToString();
                        dynTipoOcorrencia.Descricao = motivo.Descricao;
                        retorno.Objeto.Itens.Add(dynTipoOcorrencia);
                    }
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou motivos de aprovação de ocorrências", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar motivos de aprovação de Ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }
        
        public Retorno<int> AdicionarOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia ocorrencia)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AdicionarOcorrencia(ocorrencia));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>> BuscarEntregasPedidoPendentesIntegracao(int? inicio, int? limite)
        {
#if DEBUG
#else
            ValidarToken();
#endif

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>>()
            {
                Mensagem = ""
            };

            inicio ??= 0;
            limite ??= 0;

            try
            {
                if (limite <= 100)
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEntregasPendenteIntegracao((int)inicio, (int)limite, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntrega>()
                    {
                        Itens = Servicos.Embarcador.GestaoEntregas.EntregaPedido.PedidoPendentesIntegracao(cargaEntregaPedidos),
                        NumeroTotalDeRegistro = repCargaEntregaPedido.ContarPorCargaEntregasPendenteIntegracao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Entregue)
                    };

                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou entrega de pedido pendentes", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar os pedidos";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarIntegracaoPedidosPendentes(List<Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntregaProtocolo> protocolos)
        {
            Servicos.Log.TratarErro("Numero de Protocolos: " + protocolos.Count.ToString(), "ConfirmarIntegracaoPedidosPendentes");
#if DEBUG
#else
            ValidarToken();
#endif

            Retorno<bool> retorno = new Retorno<bool>();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configControleEntrega = repConfiguracaoControleEntrega.ObterConfiguracaoPadrao();

                foreach (Dominio.ObjetosDeValor.Embarcador.Pedido.PedidoEntregaProtocolo protocolo in protocolos)
                {
                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregaPedidos = repCargaEntregaPedido.BuscarPorCargaEPedidoPedido(protocolo.ProtocoloCarga, protocolo.ProtocoloPedido);

                    List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = (from obj in cargaEntregaPedidos select obj.CargaEntrega).Distinct().ToList();
                    if (cargaEntregas.Count > 0)
                    {
                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                        {
                            cargaEntrega.IntegradoERP = true;
                            repCargaEntrega.Atualizar(cargaEntrega);
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repCargaEntrega, unitOfWork, configControleEntrega);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, "Confirmou integração do pedido.", unitOfWork);
                        }
                        unitOfWork.CommitChanges();
                        retorno.Status = true;
                        retorno.Objeto = true;
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O protocolo informada não existe no Multi Embarcador";
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.Mensagem = "Ocorreu uma falha ao processar a confirmação";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Servicos.Log.TratarErro("Numero de Protocolos: " + protocolos.Count.ToString() + " Retorno: " + retorno.Status.ToString() + " " + retorno.DataRetorno, "ConfirmarIntegracaoPedidosPendentes");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>> BuscarOcorrencia(string cnpjcpfCliente, string cnpjcpfDestinatario, string cnpjcpfRecebedor, string cnpjcpfRemetente, int? numeroCTe, string numeroNota, int? numeroPedido, string numeroPedidoCliente, string numeroSolicitacao, int? serieCTe, int? serieNota, int? inicio, int? limite, string numeroPedidoNoCliente)
        {
            numeroCTe ??= 0;
            numeroPedido ??= 0;
            serieCTe ??= 0;
            serieNota ??= 0;
            inicio ??= 0;
            limite ??= 0;

            try
            {
                Servicos.Log.TratarErro($"BuscarOcorrencia: {" cnpjcpfCliente: " + cnpjcpfCliente + " cnpjcpfDestinatario: " + cnpjcpfDestinatario + " cnpjcpfRemetente: " + cnpjcpfRemetente + " numeroCTe: " + numeroCTe + " numeroNota: " + (numeroNota?.ToString() ?? "") + " numeroPedido: " + numeroPedido.ToString() + " numeroPedidoCliente: " + numeroPedidoCliente + " numeroSolicitacao: " + numeroSolicitacao + " serieCTe: " + serieCTe.ToString() + " serieNota: " + serieNota.ToString() + " inicio: " + inicio.ToString() + " limite: " + limite.ToString()}");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
#if DEBUG
#else
            ValidarToken();
#endif
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>>()
            {
                Mensagem = ""
            };

            try
            {
                if (limite == 0)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "O limite deve ser maior que zero.";
                }
                else if (limite <= 100)
                {
                    Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repPedidoOcorrencia = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);
                    Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                    Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                    double.TryParse(Utilidades.String.OnlyNumbers(cnpjcpfCliente), out double codigoCliente);
                    double.TryParse(Utilidades.String.OnlyNumbers(cnpjcpfDestinatario), out double codigoDestinatario);
                    double.TryParse(Utilidades.String.OnlyNumbers(cnpjcpfRecebedor), out double codigoRecebedor);
                    double.TryParse(Utilidades.String.OnlyNumbers(cnpjcpfRemetente), out double codigoRemetente);

                    List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> ocorrencias = repCargaOcorrencia.ConsultarOcorrenciasDocumento(codigoRecebedor, numeroPedidoCliente, numeroSolicitacao, numeroNota, (int)serieNota, (int)numeroCTe, (int)serieCTe, (int)numeroPedido, codigoCliente, codigoRemetente, codigoDestinatario, (int)inicio, (int)limite, numeroPedidoNoCliente);

                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>()
                    {
                        Itens = ocorrencias == null ? null : srvOcorrencia.RetornoOcorrenciaIntegracao(ocorrencias),
                        NumeroTotalDeRegistro = repCargaOcorrencia.ContarOcorrenciasDocumento(codigoRecebedor, numeroPedidoCliente, numeroSolicitacao, numeroNota, (int)serieNota, (int)numeroCTe, (int)serieCTe, (int)numeroPedido, codigoCliente, codigoRemetente, codigoDestinatario, numeroPedidoNoCliente)
                    };

                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ocorrenciasPedido = repPedidoOcorrencia.ConsultarOcorrenciasDocumento(codigoRecebedor, numeroPedidoCliente, numeroSolicitacao, numeroNota, (int)serieNota, (int)numeroCTe, (int)serieCTe, (int)numeroPedido, codigoCliente, codigoRemetente, codigoDestinatario, (int)inicio, (int)limite, numeroPedidoNoCliente);
                    if (ocorrenciasPedido != null && ocorrenciasPedido.Count > 0)
                    {
                        retorno.Objeto.Itens.AddRange(srvOcorrencia.RetornoOcorrenciaIntegracao(ocorrenciasPedido));
                        retorno.Objeto.NumeroTotalDeRegistro += repPedidoOcorrencia.ContarOcorrenciasDocumento(codigoRecebedor, numeroPedidoCliente, numeroSolicitacao, numeroNota, (int)serieNota, (int)numeroCTe, (int)serieCTe, (int)numeroPedido, codigoCliente, codigoRemetente, codigoDestinatario, numeroPedidoNoCliente);
                    }

                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou ocorrenciass", unitOfWork);
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar as ocorrencias";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> BuscarDocumentosOcorrencia(int? protocoloOcorrencia, int? numeroOcorrencia)
        {
            ValidarToken();

            protocoloOcorrencia ??= 0;
            numeroOcorrencia ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                Servicos.WebService.CTe.CTe serWSCTe = new Servicos.WebService.CTe.CTe(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>();
                retorno.Status = true;
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTeComplementoInfo = null;
                if (protocoloOcorrencia > 0)
                {
                    cargasCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorOcorrencia((int)protocoloOcorrencia);
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaCTeComplementoInfo.ContarPorOcorrencia((int)protocoloOcorrencia);
                }
                else if (numeroOcorrencia > 0)
                {
                    cargasCTeComplementoInfo = repCargaCTeComplementoInfo.BuscarPorNumeroOcorrencia((int)numeroOcorrencia);
                    retorno.Objeto.NumeroTotalDeRegistro = repCargaCTeComplementoInfo.ContarPorNumeroOcorrencia((int)numeroOcorrencia);
                }

                retorno.Objeto.Itens = new List<Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar>();
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTeComplementoInfo)
                {
                    Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar cTeComplementar = new Dominio.ObjetosDeValor.WebService.CTe.CTeComplementar();
                    cTeComplementar.ProtocoloCTeComplementado = cargaCTeComplementoInfo.CargaCTeComplementado.CTe.Codigo;
                    cTeComplementar.CTe = serWSCTe.ConverterObjetoCargaCTeComplementoInfo(cargaCTeComplementoInfo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Todos, configuracao.UtilizarCodificacaoUTF8ConversaoPDF, unitOfWork);
                    if (cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia != null)
                    {
                        cTeComplementar.Ocorrencia = new Dominio.ObjetosDeValor.WebService.Ocorrencia.Ocorrencia
                        {
                            Protocolo = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Codigo,
                            CodigoIntegracao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.CodigoProceda,
                            Descricao = cargaCTeComplementoInfo.CargaOcorrencia.TipoOcorrencia.Descricao
                        };
                    }

                    retorno.Objeto.Itens.Add(cTeComplementar);
                }

                Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou documentos da ocorrência protocolo " + protocoloOcorrencia.ToString(), unitOfWork);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao buscar documentos da ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SolicitarCancelamentoOcorrencia(int protocoloOcorrencia, string motivoCancelamento)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.SolicitarCancelamentoOcorrencia solicitarCancelamentoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.SolicitarCancelamentoOcorrencia
            {
                ProtocoloOcorrencia = protocoloOcorrencia,
                MotivoCancelamento = motivoCancelamento
            };

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).SolicitarCancelamentoOcorrencia(solicitarCancelamentoOcorrencia));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia> BuscarSituacaoOcorrencia(int protocoloOcorrencia)
        {
            Dominio.ObjetosDeValor.Embarcador.Ocorrencia.BuscarSituacaoOcorrencia buscarSituacaoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.BuscarSituacaoOcorrencia
            {
                ProtocoloOcorrencia = protocoloOcorrencia
            };

            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoOcorrencia>.CreateFrom(new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarSituacaoOcorrencia(buscarSituacaoOcorrencia));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoDetalhesOcorrencia> BuscarOcorrenciaPorNumero(int numeroOcorrencia)
        {
            Servicos.Log.TratarErro($"BuscarOcorrenciaPorNumero: numeroOcorrencia: {numeroOcorrencia}");

#if !DEBUG
            ValidarToken();
#endif

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoDetalhesOcorrencia> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoDetalhesOcorrencia>();

            try
            {
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao repCargaOcorrenciaAutorizacao = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao(unitOfWork);

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repCargaOcorrencia.BuscarPorNumero(numeroOcorrencia);

                if (ocorrencia == null)
                {
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Status = false;
                    retorno.Mensagem = "Ocorrência não localizada";
                }
                else
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaAutorizacao aprovacao = repCargaOcorrenciaAutorizacao.BuscarUltimoAprovadorOcorrencia(ocorrencia.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoOcorrencia.EmissaoOcorrencia);

                    retorno.Objeto = new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.RetornoDetalhesOcorrencia()
                    {
                        NumeroOcorrencia = ocorrencia.NumeroOcorrencia,
                        Data = ocorrencia.DataOcorrencia.ToString("dd/MM/yyyy"),
                        Observacao = ocorrencia.Observacao,
                        Situacao = ocorrencia.DescricaoSituacao,
                        ValorOcorrencia = ocorrencia.ValorOcorrencia,
                        CodigoIntegracaoTipoOcorrencia = ocorrencia.TipoOcorrencia.CodigoIntegracao,
                        DescricaoTipoOcorrencia = ocorrencia.TipoOcorrencia.Descricao,
                        NumeroCarga = ocorrencia.Carga?.CodigoCargaEmbarcador,
                        CodigoIntegracaoFilial = ocorrencia.Carga?.Filial?.CodigoFilialEmbarcador,
                        DescricaoFilial = ocorrencia.Carga?.Filial?.Descricao,
                        CPFSolicitante = ocorrencia.Usuario?.CPF,
                        NomeSolicitante = ocorrencia.Usuario?.Nome,
                        CPFAprovador = aprovacao?.Usuario?.CPF,
                        NomeAprovador = aprovacao?.Usuario?.Nome,
                        CodigoMotivoAprovacao = aprovacao?.MotivoRejeicaoOcorrencia?.Codigo.ToString(),
                        DescricaoMotivoAprovacao = aprovacao?.MotivoRejeicaoOcorrencia?.Descricao,
                        CentroCusto = aprovacao?.CentroResultado?.Descricao
                    };

                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Buscou ocorrencias", unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as ocorrencias";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<int> BuscarQuantidadeOcorrenciasColetaEntregaNaoIntegradas(string dataInicial, string dataFinal)
        {
            ValidarToken();

            Retorno<int> retorno = new Retorno<int>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                DateTime? dataInicioFiltro = dataInicial.ToNullableDateTime();
                if (!string.IsNullOrWhiteSpace(dataInicial) && !dataInicioFiltro.HasValue)
                    return Retorno<int>.CriarRetornoDadosInvalidos("A data inicio não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                DateTime? dataFimFiltro = dataFinal.ToNullableDateTime();
                if (!string.IsNullOrWhiteSpace(dataFinal) && !dataFimFiltro.HasValue)
                    return Retorno<int>.CriarRetornoDadosInvalidos("A data inicio não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                int quantidadeRejistros = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarQuantidadeIntegracoesPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, dataInicioFiltro, dataFimFiltro);

                retorno.Mensagem = "Existem " + quantidadeRejistros.ToString() + " registros com falha de integração.";
                retorno.Objeto = quantidadeRejistros;
                retorno.Status = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SolicitarReenvioIntegracaoOcorrenciaColetaEntrega(string dataInicial, string dataFinal, int? quantidade)
        {
            ValidarToken();

            quantidade ??= 0;

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                DateTime? dataInicioFiltro = dataInicial.ToNullableDateTime();
                if (!string.IsNullOrWhiteSpace(dataInicial) && !dataInicioFiltro.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A data inicio não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                DateTime? dataFimFiltro = dataFinal.ToNullableDateTime();
                if (!string.IsNullOrWhiteSpace(dataFinal) && !dataFimFiltro.HasValue)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("A data inicio não esta em um formato correto (dd/MM/yyyy HH:mm:ss)");

                if (quantidade > 1000)
                {
                    retorno.Mensagem = "Quantidade máxima por integração é 1000.";
                    retorno.Objeto = false;
                    retorno.Status = true;
                }
                else
                {
                    if (quantidade <= 0)
                        quantidade = 1000;

                    Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao repPedidoOcorrenciaColetaEntregaIntegracao = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao(unitOfWork);
                    List<int> integracoes = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao, dataInicioFiltro, dataFimFiltro, (int)quantidade);
                    for (int i = 0; i < integracoes.Count; i++)
                    {
                        Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao pedidoOcorrenciaColetaEntregaIntegracao = repPedidoOcorrenciaColetaEntregaIntegracao.BuscarPorCodigo(integracoes[i]);
                        pedidoOcorrenciaColetaEntregaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                        pedidoOcorrenciaColetaEntregaIntegracao.ProblemaIntegracao = string.Empty;
                        repPedidoOcorrenciaColetaEntregaIntegracao.Atualizar(pedidoOcorrenciaColetaEntregaIntegracao);

                        unitOfWork.FlushAndClear();
                    }

                    retorno.Mensagem = "Solicitação realizada com sucesso.";
                    retorno.Objeto = true;
                    retorno.Status = true;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        #endregion

        #region Métodos Globais Integracao Ocorrencia embarcador

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>> BuscarOcorrenciasPorTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Servicos.WebService.Ocorrencia.Ocorrencia serWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                retorno.Objeto = serWSOcorrencia.BuscarOcorrenciasPorTransportador(integradora.Empresa, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento>> BuscarOcorrenciasCanceladasPorTransportador()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento>>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Servicos.WebService.Ocorrencia.Ocorrencia serWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                retorno.Objeto = serWSOcorrencia.BuscarOcorrenciasCanceladasPorTransportador(integradora.Empresa, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> ConfirmarIntegracaoOcorrenciaTransportador(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<string> retorno = new Retorno<string>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(protocolo, true);

                if (cargaOcorrencia.Emitente != null && !cargaOcorrencia.Emitente.CNPJ.StartsWith(integradora.Empresa.CNPJ.Left(8)))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo inválido para este transportador.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                cargaOcorrencia.IntegrouTransportador = true;

                repCargaOcorrencia.Atualizar(cargaOcorrencia, Auditado);

                unitOfWork.CommitChanges();

                retorno.Objeto = "Confirmação realizada com sucesso.";
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da ocorrencia.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> ConfirmarIntegracaoOcorrenciaCanceladaTransportador(int protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<string> retorno = new Retorno<string>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repCargaOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento cargaOcorrenciaCancelamento = repCargaOcorrenciaCancelamento.BuscarPorCodigo(protocolo, true);

                if (cargaOcorrenciaCancelamento.Ocorrencia.Emitente != null && !cargaOcorrenciaCancelamento.Ocorrencia.Emitente.CNPJ.StartsWith(integradora.Empresa.CNPJ.Left(8)))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Protocolo inválido para este transportador.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                unitOfWork.Start();

                cargaOcorrenciaCancelamento.IntegrouTransportador = true;

                repCargaOcorrenciaCancelamento.Atualizar(cargaOcorrenciaCancelamento, Auditado);

                unitOfWork.CommitChanges();

                retorno.Objeto = "Confirmação realizada com sucesso.";
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar a integração da ocorrencia cancelada.";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> BuscarOcorrenciaPorProtocoloETransportador(int protocoloOcorrencia)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora.Empresa == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    return retorno;
                }

                Servicos.WebService.Ocorrencia.Ocorrencia serWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                retorno.Objeto = serWSOcorrencia.BuscarOcorrenciaPorProtocoloETransportador(protocoloOcorrencia, integradora.Empresa, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntrega(Dominio.ObjetosDeValor.WebService.Carga.Protocolos protocolo)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> retorno = new Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>() { Status = true };

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (protocolo == null || (protocolo.protocoloIntegracaoCarga == 0 && protocolo.protocoloIntegracaoPedido == 0))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados inválidos para esta integração.";
                    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    return retorno;
                }

                Servicos.WebService.Ocorrencia.Ocorrencia serWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                retorno.Objeto = serWSOcorrencia.BuscarOcorrenciasEntrega(protocolo, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar a ocorrência";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntregaPorNumeroPedido(string numeroPedidoEmbarcador, string dataInicial, string dataFinal)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                DateTime dataInicioParametro = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(dataInicial))
                    DateTime.TryParseExact(dataInicial, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataInicioParametro);

                DateTime dataFimParametro = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(dataFinal))
                    DateTime.TryParseExact(dataFinal, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataFimParametro);

                if (string.IsNullOrWhiteSpace(numeroPedidoEmbarcador) && (dataInicioParametro <= DateTime.MinValue || dataFimParametro <= DateTime.MinValue))
                    return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoDadosInvalidos("É obrigatório informar o número do pedido ou intervalo de datas.");


                Servicos.WebService.Ocorrencia.Ocorrencia servicoWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> ocorrencias = servicoWSOcorrencia.BuscarOcorrenciasEntregaPorNumeroPedidoEmbarcador(numeroPedidoEmbarcador, dataInicioParametro, dataFimParametro, unitOfWork);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoSucesso(ocorrencias);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as ocorrências.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>> BuscarOcorrenciasEntregaPorNumeroPedidoPaginado(string numeroPedidoEmbarcador, string dataInicial, string dataFinal, int? inicioRegistros, int? limiteRegistros)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            inicioRegistros ??= 0;
            limiteRegistros ??= 0;

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                DateTime dataInicioParametro = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(dataInicial))
                    DateTime.TryParseExact(dataInicial, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataInicioParametro);

                DateTime dataFimParametro = DateTime.MinValue;
                if (!string.IsNullOrWhiteSpace(dataFinal))
                    DateTime.TryParseExact(dataFinal, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataFimParametro);

                if (string.IsNullOrWhiteSpace(numeroPedidoEmbarcador) && (dataInicioParametro <= DateTime.MinValue || dataFimParametro <= DateTime.MinValue))
                    return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoDadosInvalidos("É obrigatório informar o número do pedido ou intervalo de datas.");

                if (limiteRegistros < 1 || limiteRegistros > 100)
                    return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoDadosInvalidos("Limite de registros invalido, informe valor entre 1 e 100.");

                Servicos.WebService.Ocorrencia.Ocorrencia servicoWSOcorrencia = new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork);

                List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega> ocorrencias = servicoWSOcorrencia.BuscarOcorrenciasEntregaPorNumeroPedidoEmbarcador(numeroPedidoEmbarcador, dataInicioParametro, dataFimParametro, unitOfWork, (int)inicioRegistros, (int)limiteRegistros);

                return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoSucesso(ocorrencias);
            }
            catch (System.ServiceModel.FaultException e)
            {
                Servicos.Log.TratarErro(e);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoExcecao(e.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaEntrega>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as ocorrências.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        public Retorno<int> EnviarOcorrencia(Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracao)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<int>.CreateFrom(new Servicos.WebService.Ocorrencia.CargaOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarOcorrencia(ocorrenciaIntegracao));
            });
        }

        public Retorno<bool> EnviarCancelamentoOcorrencia(Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Ocorrencia.CargaOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EnviarCancelamentoOcorrencia(ocorrenciaCancelamento));
            });
        }

        public Retorno<bool> ConfirmarIntegracaoOcorrencia(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoOcorrencia(protocolos));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>> BuscarOcorrenciasPendentesIntegracao(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>>.CreateFrom(new Servicos.WebService.Ocorrencia.Ocorrencia(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarOcorrenciasPendentesIntegracao(quantidade ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>.CreateFrom(retorno.Objeto)
                };
            });
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceOcorrencias;
        }

        #endregion
    }
}
