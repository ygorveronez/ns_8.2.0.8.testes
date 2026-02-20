using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ControleEntrega" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ControleEntrega.svc or ControleEntrega.svc.cs at the Solution Explorer and start debugging.
    public class ControleEntrega : WebServiceBase, IControleEntrega
    {
        #region Métodos Públicos

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> ObterDadosEntrega(string usuario, string dataUltimaVerificacao, string token)
        {

            ValidarToken(token);

            DateTime dataInicioRequisicao = DateTime.Now;


            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);


            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

                if (usuarioMobile != null)
                {
                    if (!DateTime.TryParseExact(dataUltimaVerificacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de ultima verificação não esta em um formato correto (ddMMyyyyHHmmss); ";

                    }
                    else
                    {

                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Documentos.Documento>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    var dados = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterDocumentosMobile(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork);

                                    retorno.Objeto.AddRange(dados);
                                }

                            }
                            finally
                            {
                                unitOfWork.Dispose();
                                unitOfWork = null;
                            }
                        }
                    }

                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os canhotos alterados";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>> ObterDadosCargas(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = ValidarSessao(unitOfWorkAdmin);

            //if(usuarioMobile == null)
            //    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>.CriarRetornoSessaoExpirada();
            //todo: remover validacao abaixo apos atualização de segurança por sessão (descomentar trecho acima).

            if (usuarioMobile == null)
            {
                AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
                usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));
            }

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>>();
            retorno.Status = true;
            try
            {


                if (usuarioMobile != null)
                {
                    DateTime data;
                    if (!DateTime.TryParseExact(dataUltimaVerificacao, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data))
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A data de ultima verificação não esta em um formato correto (ddMMyyyyHHmmss); ";

                    }
                    else
                    {

                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Carga>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    var dados = Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ObterControleEntregaCargaMobile(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork);

                                    retorno.Objeto.AddRange(dados);
                                }

                            }
                            catch
                            {
                                throw;
                            }
                            finally
                            {
                                unitOfWork.Dispose();
                                unitOfWork = null;
                            }
                        }
                    }

                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os dados das cargas.";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> ConfirmarEntrega(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string token, string latitude, string longitude, string data, string observacao)
        {
            Servicos.Log.TratarErro($"ConfirmarEntrega data: {data} latitude: {latitude} longitude: {longitude}");

            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                double lat = double.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                double lng = double.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                DateTime.TryParseExact(data, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataConfirmacao);


                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService confiWebService = repConfiguracaoWebService.BuscarConfiguracaoPadrao();

                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

                    var listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();
                    if (!listaSituacaoEmAberto.Contains(cargaEntrega.Situacao))
                    {
                        Servicos.Log.TratarErro($"ConfirmarEntrega - CargaEntrega finalizada");
                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }

                    if (confiWebService?.NaoPermitirConfirmarEntregaSituacaoCargaEmAndamento ?? false)
                    {
                        var listaSituacaoCargaEmAberto = SituacaoCargaHelper.ObterSituacoesCargaPermiteAtualizar();
                        if (listaSituacaoCargaEmAberto.Contains(cargaEntrega.Carga.SituacaoCarga))
                        {
                            Servicos.Log.TratarErro($"ConfirmarEntrega - Carga pendente de avanço da etapa de frete para confirmação da entrega");
                            return Retorno<bool>.CriarRetornoDadosInvalidos("Carga pendente de avanço da etapa de frete para confirmação da entrega");
                        }
                    }

                    Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                    Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);

                    unitOfWork.Start();
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntrega, dataConfirmacao, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = lat, Longitude = lng }, null, 0, "", configuracao, TipoServicoMultisoftware, null, Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemSituacaoEntrega.App, usuarioMobileCliente?.Cliente, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                    unitOfWork.CommitChanges();
                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar imagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        //Para debugar; http://localhost:52952/ControleEntrega.svc/ObterMotivosDevolucao/token/20
        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>> ObterMotivosDevolucao(string usuario, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario.ToInt());

                if (usuarioMobile != null)
                {
                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao>();

                    foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        try
                        {
                            Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                            List<Dominio.Entidades.TipoDeOcorrenciaDeCTe> tiposDeOcorrenciaDeCTe = repositorioTipoDeOcorrenciaDeCTe.BuscarOcorrenciasMobile(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega, true);

                            foreach (Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaDeCTe in tiposDeOcorrenciaDeCTe)
                            {
                                var motivo = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoDevolucao { Codigo = tipoDeOcorrenciaDeCTe.Codigo, Motivo = tipoDeOcorrenciaDeCTe.Descricao };
                                retorno.Objeto.Add(motivo);
                            }
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
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os motivos da devolução";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        //Para debugar; http://localhost:52952/ControleEntrega.svc/ObterMotivosRejeicaoColeta/token/20
        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>> ObterMotivosRejeicaoColeta(string usuario, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario.ToInt());

                if (usuarioMobile != null)
                {
                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta>();

                    foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                        try
                        {
                            Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta repMotivoRejeicaoColeta = new Repositorio.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta(unitOfWork);

                            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.MotivoRejeicaoColeta> listaMotivosRejeicaoColeta = repMotivoRejeicaoColeta.BuscarAtivos();

                            retorno.Objeto.AddRange((from obj in listaMotivosRejeicaoColeta
                                                     select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColeta()
                                                     {
                                                         Codigo = obj.Codigo,
                                                         Descricao = obj.Descricao,
                                                         Produtos = (from produto in obj.Produtos
                                                                     select new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.MotivoRejeicaoColetaProduto()
                                                                     {
                                                                         Codigo = produto.Codigo,
                                                                         Descricao = produto.Descricao
                                                                     }).ToList()
                                                     }));
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
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar os motivos da devolução";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;


        }

        public Retorno<bool> RejeitarEntrega(int usuario, int codigoCargaEntrega, int codigoMotivoDevolucao, int empresaMultisoftware, string token, string latitude, string longitude, string data, string observacao)
        {
            Servicos.Log.TratarErro($"RejeitarEntrega data: {data} latitude: {latitude} longitude: {longitude}");

            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                double lat = double.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                double lng = double.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                DateTime.TryParseExact(data, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dataDevolucao);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado = null;
                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

                    Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                    {
                        TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Usuario,
                        OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceControleEntrega
                    };

                    Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros()
                    {
                        codigoCargaEntrega = codigoCargaEntrega,
                        codigoMotivo = codigoMotivoDevolucao,
                        data = dataDevolucao,
                        wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = lat, Longitude = lng },
                        tipoServicoMultisoftware = TipoServicoMultisoftware,
                        observacao = observacao,
                        configuracao = configuracao,
                        clienteMultisoftware = usuarioMobileCliente?.Cliente ?? null
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
                        new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                        return Retorno<bool>.CriarRetornoSucesso(false);
                    }
                    else
                        return Retorno<bool>.CriarRetornoSucesso(true);
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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar imagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado> VerificarEntregaPendente(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                if (usuarioMobileCliente == null)
                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoDadosInvalidos("Usuário inválido");

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);
                    Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                    Repositorio.Embarcador.Chamados.ChamadoAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoAnalise(unitOfWork);
                    if (cargaEntrega != null)
                    {
                        Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repChamado.BuscarPorCargaEntrega(cargaEntrega.Codigo);
                        List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise> chamadoAnalises = repChamadoAnalise.BuscarPorChamado(chamado.Codigo);
                        Dominio.Entidades.Embarcador.Chamados.ChamadoAnalise chamadoAnalise = chamadoAnalises.FirstOrDefault();
                        if (chamado != null)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado objChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado();
                            objChamado.NumeroChamado = chamado.Numero;
                            objChamado.Retorno = chamadoAnalise?.Observacao ?? "";
                            objChamado.Situacao = chamado.Situacao;

                            if (objChamado.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Finalizado && cargaEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntrega.Rejeitado)
                                objChamado.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoChamado.Cancelada;

                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoSucesso(objChamado);
                        }
                        else
                            return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoDadosInvalidos("Não existe nenhuma pendência para essa entrega");
                    }
                    else
                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoDadosInvalidos("Entrega não localizada");
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Chamado.Chamado>.CriarRetornoExcecao("Ocorreu uma falha ao enviar imagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarViagem(int usuario, int codigoCarga, int empresaMultisoftware, string token, string latitude, string longitude, string dataInicio)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            DateTime.TryParseExact(dataInicio, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

            try
            {
                double lat = double.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                double lng = double.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    if (carga.DataInicioViagem == null)
                    {
                        Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                        var configuracaoEmbarcador = repConfiguracao.BuscarConfiguracaoPadrao();

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado();
                        auditado.TipoAuditado = TipoAuditado.Usuario;
                        auditado.OrigemAuditado = OrigemAuditado.WebServiceMobile;

                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga.Codigo, data, OrigemSituacaoEntrega.App, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = lat, Longitude = lng }, configuracaoEmbarcador, TipoServicoMultisoftware, usuarioMobileCliente.Cliente, auditado, unitOfWork))
                        {
                            Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                            if (motorista != null)
                                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Início de viagem informado pelo motorista {motorista.Descricao} via app", unitOfWork);

                        }
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
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
        public Retorno<bool> IniciarEvento(int usuario, int codigoCarga, int codigoTipoAlerta, int empresaMultisoftware, string token, string latitude, string longitude, string dataEvento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            DateTime.TryParseExact(dataEvento, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

            try
            {
                decimal lat = decimal.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                decimal lng = decimal.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta)codigoTipoAlerta;

                    Repositorio.Embarcador.Logistica.MonitoramentoEvento repMonitoramentoEvento = new Repositorio.Embarcador.Logistica.MonitoramentoEvento(unitOfWork);
                    Dominio.Entidades.Embarcador.Logistica.MonitoramentoEvento monitoramentoEvento = repMonitoramentoEvento.BuscarAtivo(tipoAlerta);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.GerarAlerta(tipoAlerta, monitoramentoEvento, lat, lng, data, "", carga, unitOfWork);

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar evento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }
        public Retorno<bool> FinalizarEvento(int usuario, int codigoCarga, int codigoTipoAlerta, int empresaMultisoftware, string token, string latitude, string longitude, string dataEvento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                decimal lat = decimal.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                decimal lng = decimal.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta tipoAlerta = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAlerta)codigoTipoAlerta;

                    Repositorio.Embarcador.Logistica.AlertaMonitor repAlertaMonitor = new Repositorio.Embarcador.Logistica.AlertaMonitor(unitOfWork);

                    List<Dominio.Entidades.Embarcador.Logistica.AlertaMonitor> alertas = repAlertaMonitor.BuscarEmAbertoPorCarga(carga.Codigo, tipoAlerta);


                    DateTime.TryParseExact(dataEvento, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data);

                    unitOfWork.Start();
                    try
                    {

                        foreach (var alerta in alertas)
                        {
                            alerta.DataFim = data;
                            repAlertaMonitor.Atualizar(alerta);
                        }

                        unitOfWork.CommitChanges();
                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro(excecao);
                        unitOfWork.Rollback();
                        return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao finalizar evento");
                    }
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma finalizar evento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }
        public Retorno<bool> EnviarImagemEntrega(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string imagem, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    byte[] data = System.Convert.FromBase64String(imagem);

                    MemoryStream ms = new MemoryStream(data);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(ms, unitOfWork, out string tokenImagem);

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagemEntrega(empresaMultisoftware, codigoCargaEntrega, tokenImagem, unitOfWork, DateTime.Now, 0, 0, OrigemSituacaoEntrega.App) ;

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (BaseException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar imagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> EnviarImagemCanhoto(int usuario, int codigoCargaEntrega, int empresaMultisoftware, string imagem, string token)
        {
            ValidarToken(token);


            Retorno<bool> retorno = new Retorno<bool>();

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    byte[] data = System.Convert.FromBase64String(imagem);

                    MemoryStream ms = new MemoryStream(data);

                    Servicos.Embarcador.Mobile.Canhotos.Canhotos serCanhoto = new Servicos.Embarcador.Mobile.Canhotos.Canhotos();
                    serCanhoto.SalvarImagemCanhoto(ms, out string tokenImagem, unitOfWork);

                    tokenImagem = ObterTokenFormatado(codigoCargaEntrega, tokenImagem, unitOfWork);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                    Servicos.Embarcador.Canhotos.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR(unitOfWork);
                    retorno.Mensagem = serCanhoto.EnviarImagemCanhotoLeituraOCR(tokenImagem, motorista.Codigo, empresaMultisoftware, unitOfWork);

                    if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                    {
                        retorno.Status = false;
                        retorno.Objeto = false;
                    }
                    else
                    {
                        string caminhoRaiz = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoRaiz;
                        string apilink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRLink;
                        string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().APIOCRKey;

                        string[] nomeSplit = tokenImagem.Split('_');

                        Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto controleLeituraImagemCanhoto = new Dominio.Entidades.Embarcador.Canhotos.ControleLeituraImagemCanhoto
                        {
                            Data = DateTime.Now,
                            GuidArquivo = nomeSplit[0] + "_" + nomeSplit[1] + "_" + nomeSplit[2] + ".jpg",
                            NumeroDocumento = "",
                            MensagemRetorno = "",
                            NomeArquivo = nomeSplit[2] + ".jpg",
                            SituacaoleituraImagemCanhoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.AgProcessamento,
                            Extensao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.ExtensaoArquivo.JPG
                        };

                        srvLeitorOCR.DefinirAPI(apilink, apiKey);
                        srvLeitorOCR.InterpretarCanhotoPendente(ref controleLeituraImagemCanhoto, caminhoRaiz, unitOfWork, TipoServicoMultisoftware);

                        if (controleLeituraImagemCanhoto.SituacaoleituraImagemCanhoto != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoleituraImagemCanhoto.CanhotoVinculado)
                        {
                            retorno.Mensagem = controleLeituraImagemCanhoto.MensagemRetorno;
                            retorno.Status = false;
                            retorno.Objeto = false;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(retorno.Mensagem))
                        return Retorno<bool>.CriarRetornoSucesso(true);
                    else
                        return Retorno<bool>.CriarRetornoDadosInvalidos(retorno.Mensagem);

                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao enviar imagem");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }
        public Retorno<string> EnviarByteImagemEntrega(Stream imagem)
        {
            Retorno<string> retorno = new Retorno<string>();
            retorno.Status = true;
            try
            {
                string tokenImagem = "";
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SalvarImagem(imagem, null, out tokenImagem);
                retorno.Objeto = tokenImagem;
            }
            catch (ServicoException excecao)
            {
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem da Entrega";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> AtualizarDadosPosicionamento(string token, int usuario, int empresaMultisoftware, string data, string latitude, string longitude, int codigoCarga)
        {

            ValidarToken(token);
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    if (motorista != null)
                    {
                        Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramentoOuMotorista(unitOfWork, codigoCarga, motorista.Codigo);
                        if (veiculo != null)
                        {
                            Servicos.Log.TratarErro("data posição " + data);
                            DateTime dataPosicao = DateTime.Now;
                            DateTime.TryParseExact(data, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out dataPosicao);
                            if (dataPosicao == DateTime.MinValue) dataPosicao = DateTime.Now;

                            Double lat = double.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            Double lng = double.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                            Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao()
                            {
                                ID = long.Parse(data),
                                Data = dataPosicao,
                                DataVeiculo = dataPosicao,
                                DataCadastro = DateTime.Now,
                                IDEquipamento = usuario.ToString(),
                                Veiculo = veiculo,
                                Ignicao = 1,
                                Latitude = lat,
                                Longitude = lng,
                                Velocidade = 0,
                                Temperatura = 0,
                                Descricao = $"{lat}, {lng} (CE)"
                            };

                            Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao repPosicaoPendenteIntegracao = new Repositorio.Embarcador.Logistica.PosicaoPendenteIntegracao(unitOfWork);
                            repPosicaoPendenteIntegracao.Inserir(posicaoPendenteIntegracao);
                        }

                        //VAMOS ATUALIZAR A LISTA DE MOTORISTAS QUE ESTAO NESTE CPF na qual estao com versao do APP diferente;
                        if (!string.IsNullOrEmpty(usuarioMobileCliente.UsuarioMobile?.VersaoAPP ?? ""))
                            AtualizarVersaoAppMotoristasPorCPFEVersaoDiferente(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF, usuarioMobileCliente.UsuarioMobile.VersaoAPP);
                    }

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao alterar a tração");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }


        #endregion

        #region Métodos Privados

        private string ObterTokenFormatado(int codigoCargaEntrega, string token, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            return cargaEntrega.Carga.Empresa.CNPJ_SemFormato + "_E_" + token;
        }

        #endregion
    }
}