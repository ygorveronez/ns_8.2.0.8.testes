using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace SGT.Mobile
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Cargas" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Cargas.svc or Cargas.svc.cs at the Solution Explorer and start debugging.
    public class Cargas : WebServiceBase, ICargas
    {
        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargaAgNotaPorUsuario(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();

            //Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(Conexao.StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

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
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serCarga.BuscarCargasAgNotas(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargaPorUsuario(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();

            //Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(Conexao.StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

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
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serCarga.BuscarCargas(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargasCanceladas(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();

            //Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(Conexao.StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

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
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serCarga.BuscarCargasCanceladas(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> BuscarCargasFinalizadas(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Cargas.Carga serCarga = new Servicos.Embarcador.Mobile.Cargas.Carga();

            //Servicos.WebService.Carga.Carga serWSCarga = new Servicos.WebService.Carga.Carga(Conexao.StringConexao);

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

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
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Cargas.Carga>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serCarga.BuscarCargasEncerradas(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>> BuscarMotivosOcorrencia(string usuario, string dataUltimaVerificacao, string token)
        {
            ValidarToken(token);
            DateTime dataInicioRequisicao = DateTime.Now;
            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(int.Parse(usuario));

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
                        retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Ocorrencias.MotivoOcorrencia>();
                        foreach (AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente in usuarioMobile.Clientes.ToList())
                        {
                            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                            try
                            {
                                if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                    usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                                {
                                    retorno.Objeto.AddRange(serOcorrencia.BuscarMotivosOcorrencia(data, usuarioMobile.Codigo, usuarioMobileCliente.Cliente, unitOfWork));
                                }
                                else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                                {
                                    //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = dataInicioRequisicao.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<int> EnviarOcorrencia(int usuario, int ocorrencia, int empresaMultisoftware, string dataOcorrencia, int motivo, string observacao, string latitude, string longitude, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<int> retorno = new Retorno<int>();
            retorno.Status = true;
            retorno.Objeto = 0;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (motorista != null)
                                {
                                    DateTime data;
                                    if (!DateTime.TryParseExact(dataOcorrencia, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out data) || motivo == 0 || string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Dados inválidos, data, motivo, latitude ou longitude não informado";
                                    }
                                    else
                                    {
                                        int codigoOcorrencia = serOcorrencia.EnviarOcorrencia(usuario, ocorrencia, empresaMultisoftware, data, motivo, observacao, latitude, longitude, unitOfWork);
                                        if (codigoOcorrencia > 0)
                                        {
                                            retorno.Objeto = codigoOcorrencia;
                                            retorno.Status = true;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                            retorno.Mensagem = "Sucesso";
                                        }
                                        else
                                        {
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                            retorno.Mensagem = "Dados inválidos, data, motivo, latitude ou longitude não informado";
                                        }
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
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
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da ocorrência";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> BuscarGrupoClientesDigitalizacaoCanhoto(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            int _usuario = usuario.ToInt();
            int _empresaMultisoftware = empresaMultisoftware.ToInt();

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Servicos.Embarcador.Mobile.Pessoas.Pessoa srvPessoa = new Servicos.Embarcador.Mobile.Pessoas.Pessoa();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>>
            {
                Status = true
            };

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(_usuario, _empresaMultisoftware, unitOfWorkAdmin);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.GrupoPessoa>();

                    try
                    {
                        if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                            usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            retorno.Objeto = srvPessoa.BuscarGrupoClientesDigitalizamCanhoto(unitOfWork);
                        }
                        else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        {
                            //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                        unitOfWork = null;
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            return retorno;
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> BuscarClientesDigitalizacaoCanhoto(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);
            Servicos.Log.TratarErro("Token: " + token);
            Servicos.Log.TratarErro("Usuário: " + usuario);
            Servicos.Log.TratarErro("Empresa: " + empresaMultisoftware);

            int _usuario = usuario.ToInt();
            int _empresaMultisoftware = empresaMultisoftware.ToInt();

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Servicos.Embarcador.Mobile.Pessoas.Pessoa srvPessoa = new Servicos.Embarcador.Mobile.Pessoas.Pessoa();

            Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>> retorno = new Retorno<List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>>
            {
                Status = true
            };

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(_usuario, _empresaMultisoftware, unitOfWorkAdmin);

                if (usuarioMobileCliente != null)
                {
                    Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                    retorno.Objeto = new List<Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa>();

                    try
                    {
                        if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                            usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                        {
                            retorno.Objeto = srvPessoa.BuscarClientesDigitalizamCanhoto(unitOfWork);
                        }
                        else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        {
                            //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                        unitOfWork = null;
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
                retorno.Mensagem = "Ocorreu uma falha ao consultar a Carga";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> EnviarDocumentoOcorrencia(int usuario, int ocorrencia, int documento, int empresaMultisoftware, string token, string numeroDocumentoRecebedor, string nomeRecebedor)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);


                                if (motorista != null)
                                {
                                    bool retornoSalvar = serOcorrencia.EnviarDocumentoOcorrencia(ocorrencia, documento, numeroDocumentoRecebedor, nomeRecebedor, usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware, unitOfWork, usuarioMobileCliente.Cliente);
                                    if (retornoSalvar)
                                    {
                                        retorno.Objeto = true;
                                        retorno.Status = true;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                        retorno.Mensagem = "Sucesso";
                                    }
                                    else
                                    {
                                        retorno.Objeto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Dados inválidos, documento ou carga não localizados";
                                    }
                                }
                                else
                                {
                                    retorno.Objeto = false;
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Objeto = false;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (ServicoException excecao)
            {
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem do Canhoto";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarImagemOcorrencia(int usuario, int ocorrencia, int empresaMultisoftware, string tokenImagem, string token, int codigoCargaEntrega)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        string caminhoTemp = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao;
                        string fileLocationTemp = Utilidades.IO.FileStorageService.Storage.Combine(caminhoTemp, tokenImagem);

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (motorista != null)
                                {
                                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = repCargaOcorrencia.BuscarPorCodigo(ocorrencia);

                                    if (cargaOcorrencia != null)
                                    {
                                        if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                        {
                                            string url = ObterURLCliente(usuarioMobileCliente);
                                            if (!string.IsNullOrWhiteSpace(url))
                                            {
                                                url = $"{url}/SGT.WebService/NFe.svc";


                                                System.IO.StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(fileLocationTemp));
                                                try
                                                {
                                                    System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);
                                                    System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                                                    binding.MaxReceivedMessageSize = int.MaxValue;
                                                    binding.ReceiveTimeout = new TimeSpan(0, 5, 0);

                                                    if (url.StartsWith("https"))
                                                        binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;


                                                    NFe.NFeClient wsNFe = new NFe.NFeClient(binding, endpointAddress);
                                                    wsNFe.Endpoint.Address = new EndpointAddress(url);

                                                    Servicos.InspectorBehavior inspector = new Servicos.InspectorBehavior();
                                                    wsNFe.Endpoint.EndpointBehaviors.Add(inspector);
                                                    OperationContextScope scope = new OperationContextScope(wsNFe.InnerChannel);
                                                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "9834cdd53573429da6f4b3fbf9b50f4e");
                                                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                                                    NFe.RetornoOfstring retornoToken = wsNFe.EnviarStreamImagemOcorrencia(reader.BaseStream);
                                                    if (retornoToken.Status)
                                                    {
                                                        NFe.RetornoOfboolean retornoEnvio = wsNFe.EnviarImagemOcorrencia(motorista.Codigo, cargaOcorrencia.Codigo, retornoToken.Objeto);
                                                        if (!retornoEnvio.Status)
                                                        {
                                                            retorno.Status = false;
                                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                            retorno.Mensagem = retornoEnvio.Mensagem;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        retorno.Status = false;
                                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                        retorno.Mensagem = retornoToken.Mensagem;
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    throw;
                                                }
                                                finally
                                                {
                                                    reader.Dispose();
                                                }
                                            }
                                            else
                                            {
                                                retorno.Status = false;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                retorno.Mensagem = "A integração de imagens de ocorrências com este embarcador não está habilitada.";
                                            }
                                        }
                                        else
                                        {
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                            retorno.Mensagem = "A imagem não foi enviada para o servidor.";
                                        }
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "O codigo informado não pertence a uma ocorrência válida";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);
                            throw;
                        }
                        finally
                        {

                            if (Utilidades.IO.FileStorageService.Storage.Exists(fileLocationTemp))
                                Utilidades.IO.FileStorageService.Storage.Delete(fileLocationTemp);

                            unitOfWork.Dispose();
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
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
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o envio da imagem da Ocorrência";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> InformarEtapaFluxoPatio(int usuario, int empresaMultisoftware, int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo, string dataEtapa, string token, string latitude, string longitude)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario usuarioEmbarcador = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (usuarioEmbarcador != null)
                                {
                                    if (!DateTime.TryParseExact(dataEtapa, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime data))
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Dados inválidos, data não está no formato correto ddMMyyyyHHmmss";
                                    }


                                    Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                                    Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                                    if ((configuracaoTMS.AppUtilizaControleColetaEntrega) && (etapaFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem))
                                    {
                                        double lat = double.Parse(latitude, System.Globalization.CultureInfo.InvariantCulture);
                                        double lng = double.Parse(longitude, System.Globalization.CultureInfo.InvariantCulture);

                                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                                        {
                                            TipoAuditado = TipoAuditado.Usuario,
                                            OrigemAuditado = OrigemAuditado.WebServiceMobile
                                        };

                                        if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(carga, DateTime.Now, OrigemSituacaoEntrega.App, new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint { Latitude = lat, Longitude = lng }, configuracaoTMS, TipoServicoMultisoftware, this.Cliente, auditado, unitOfWork))
                                        {
                                            Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);

                                            if (motorista != null)
                                                Servicos.Auditoria.Auditoria.Auditar(auditado, carga, $"Início de viagem informado pelo motorista {motorista.Descricao} via app", unitOfWork);
                                        }
                                    }
                                    else
                                        FluxoPatio(carga, etapaFluxo, latitude, longitude, retorno, unitOfWork, data);
                                }
                                else
                                {
                                    retorno.Objeto = false;
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Objeto = false;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (ServicoException excecao)
            {
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Status = false;
                retorno.Mensagem = excecao.Message;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao confirmar o fluxo de pátio.";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        private Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio FluxoPatio(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxo, string latitude, string longitude, Retorno<bool> retorno, Repositorio.UnitOfWork unitOfWork, DateTime data)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCargaETipo(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);

            if (fluxoGestaoPatio != null)
            {
                if (etapaFluxo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao)
                {
                    if (etapaFluxo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Todas)
                        etapaFluxo = fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual;

                    if (etapaFluxo == fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual)
                    {

                        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado
                        {
                            TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                            OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas
                        };

                        new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork, auditado, Cliente).LiberarProximaEtapa(fluxoGestaoPatio, etapaFluxo, data);
                    }
                    else
                    {
                        retorno.Objeto = false;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "A atual etapa do fluxo não é a informada";
                    }
                }

                if (!string.IsNullOrEmpty(latitude) && !string.IsNullOrEmpty(longitude))
                {
                    Repositorio.Embarcador.GestaoPatio.RastreamentoCarga repRastreamentoCarga = new Repositorio.Embarcador.GestaoPatio.RastreamentoCarga(unitOfWork);
                    Dominio.Entidades.Embarcador.GestaoPatio.RastreamentoCarga rastreamentoCarga = repRastreamentoCarga.BuscarPorFluxoGestaoPatio(fluxoGestaoPatio.Codigo);
                    if (rastreamentoCarga != null)
                    {
                        rastreamentoCarga.UltimaAtualizacao = DateTime.Now;
                        rastreamentoCarga.SetLatitude(latitude);
                        rastreamentoCarga.SetLongitude(longitude);
                        repRastreamentoCarga.Atualizar(rastreamentoCarga);
                    }
                }
            }
            else
            {
                retorno.Objeto = false;
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                retorno.Mensagem = "A carga informada não possui fluxo de pátio.";
            }

            return etapaFluxo;
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatio> ObterEtapaFluxoPatio(int usuario, int empresaMultisoftware, int carga, string token, int filial, string placa)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Retorno<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatio> retorno = new Retorno<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatio>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario usuarioEmbarcador = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (usuarioEmbarcador != null)
                                {
                                    Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio repFluxoGestaoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);
                                    Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);
                                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                                    Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = null;
                                    if (carga > 0)
                                        fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorCargaETipo(carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);
                                    else if (filial > 0 && !string.IsNullOrWhiteSpace(placa))
                                    {
                                        Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorPlaca(placa);
                                        fluxoGestaoPatio = repFluxoGestaoPatio.BuscarPorVeiculoFilialMotorista(veiculo.Codigo, filial, usuarioEmbarcador.Codigo);
                                    }

                                    if (fluxoGestaoPatio != null)
                                    {
                                        retorno.Objeto = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatio();
                                        retorno.Objeto.Carga = new Dominio.ObjetosDeValor.WebService.Carga.CargaIntegracao();
                                        retorno.Objeto.Carga.NumeroCarga = fluxoGestaoPatio.Carga?.CodigoCargaEmbarcador ?? "";
                                        retorno.Objeto.Carga.ProtocoloCarga = fluxoGestaoPatio.Carga?.Codigo ?? 0;
                                        retorno.Objeto.EtapaAtualLiberada = fluxoGestaoPatio.EtapaAtualLiberada;
                                        retorno.Objeto.IndiceEtapaAtual = fluxoGestaoPatio.EtapaAtual;
                                        retorno.Objeto.Etapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao>();

                                        foreach (var etapa in fluxoGestaoPatio.GetEtapas())
                                        {
                                            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao etapaDescricao = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(etapa.EtapaFluxoGestaoPatio, fluxoGestaoPatio.Tipo);
                                            DateTime? dataEtapa = servicoFluxoGestaoPatio.ObterDataEtapa(fluxoGestaoPatio, etapa.EtapaFluxoGestaoPatio);

                                            if (dataEtapa != null)
                                                etapaDescricao.DataInformada = dataEtapa.Value.ToString("ddMMyyyyHHmmss");

                                            retorno.Objeto.Etapas.Add(etapaDescricao);
                                        }

                                        retorno.Status = true;
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "A carga informada não possui fluxo de pátio.";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
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
                retorno.Mensagem = "Ocorreu uma falha ao buscar o fluxo de pátio.";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> EnviarByteImagemOcorrencia(Stream imagem)
        {
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<string> retorno = new Retorno<string>();
            retorno.Status = true;
            try
            {
                string tokenImagem = "";
                retorno.Mensagem = serOcorrencia.SalvarImagem(imagem, out tokenImagem, null);
                if (!string.IsNullOrWhiteSpace(retorno.Mensagem))
                {
                    retorno.Status = false;
                    retorno.Objeto = "";
                }
                else
                {
                    retorno.Objeto = tokenImagem;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a imagem (stream) da Ocorrencia";
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;

        }

        public Retorno<bool> EnviarOcorrenciaIntegracao(int usuario, int ocorrencia, int empresaMultisoftware, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);
            Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia serOcorrencia = new Servicos.Embarcador.Mobile.Ocorrencias.Ocorrencia();
            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));

                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                                Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigoMobile(usuarioMobile.Codigo);

                                if (motorista != null)
                                {
                                    bool retornoSalvar = serOcorrencia.EnviarOcorrenciaIntegracao(ocorrencia, unitOfWork, usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware, usuarioMobileCliente.Cliente);
                                    if (retornoSalvar)
                                    {
                                        retorno.Objeto = true;
                                        retorno.Status = true;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                        retorno.Mensagem = "Sucesso";
                                    }
                                    else
                                    {
                                        retorno.Objeto = false;
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Dados inválidos, ocorrência não localizada";
                                    }
                                }
                                else
                                {
                                    retorno.Objeto = false;
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "O usuário cadastrado não está cadastrado na Empresa";
                                }

                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Objeto = false;
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
                    }
                }
                else
                {
                    retorno.Objeto = false;
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O usuário informado é inválido";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Objeto = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao enviar a ocorrência para a integração";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<bool> EnviarChaveNotaParcial(string chaveNFe, int usuario, int codigoCarga, int empresaMultisoftware, string token)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);
            AdminMultisoftware.Repositorio.Mobile.UsuarioMobile repUsuarioMobile = new AdminMultisoftware.Repositorio.Mobile.UsuarioMobile(unitOfWorkAdmin);

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.Status = true;
            retorno.Objeto = true;
            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobile usuarioMobile = repUsuarioMobile.BuscarPorCodigo(usuario);

                if (usuarioMobile != null)
                {
                    AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = (from obj in usuarioMobile.Clientes where obj.Cliente.Codigo == empresaMultisoftware select obj).FirstOrDefault();
                    if (usuarioMobileCliente != null)
                    {
                        Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente));
                        try
                        {
                            if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ||
                                usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                            {
                                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);

                                Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial repCargaPedidoXMLNotaFiscalParcial = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial(unitOfWork);

                                if (chaveNFe.Length == 44 && serDocumento.ValidarChave(chaveNFe))
                                {
                                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                                    if (carga != null)
                                    {
                                        if (carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.AgNFe)
                                        {
                                            Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial cargaPedidoXMLNotaFiscalParcial = repCargaPedidoXMLNotaFiscalParcial.ConsultarPorChave(carga.Pedidos.FirstOrDefault().Codigo, chaveNFe);

                                            if (cargaPedidoXMLNotaFiscalParcial == null)
                                            {
                                                cargaPedidoXMLNotaFiscalParcial = new Dominio.Entidades.Embarcador.Cargas.CargaPedidoXMLNotaFiscalParcial();
                                                cargaPedidoXMLNotaFiscalParcial.CargaPedido = carga.Pedidos.FirstOrDefault();
                                                cargaPedidoXMLNotaFiscalParcial.Chave = chaveNFe;

                                                int numero = 0;

                                                int.TryParse(chaveNFe.Substring(25, 9), out numero);
                                                cargaPedidoXMLNotaFiscalParcial.Numero = numero;
                                                repCargaPedidoXMLNotaFiscalParcial.Inserir(cargaPedidoXMLNotaFiscalParcial);

                                                retorno.Status = true;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.Sucesso;
                                                retorno.Mensagem = "Sucesso";
                                            }
                                            else
                                            {
                                                retorno.Status = false;
                                                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                                retorno.Mensagem = "A chave desta nota já foi vinculada anteriormente a carga.";
                                            }
                                        }
                                        else
                                        {
                                            retorno.Status = false;
                                            retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                            retorno.Mensagem = "A atual situação da carga (" + carga.DescricaoSituacaoCarga + ")  não permite o envio das chaves.";
                                        }
                                    }
                                    else
                                    {
                                        retorno.Status = false;
                                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                        retorno.Mensagem = "Não foi localizada uma carga para esse protocolo na base Multisoftware";
                                    }
                                }
                                else
                                {
                                    retorno.Status = false;
                                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                                    retorno.Mensagem = "A chave informada não é uma chave de NF-e válida";
                                }
                            }
                            else if (usuarioMobileCliente.Cliente.ClienteConfiguracao.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                            {
                                //todo: implementar aqui a variação quando não for multiEmbarcador.
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
                            unitOfWork = null;
                        }
                    }
                    else
                    {
                        retorno.Status = false;
                        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                        retorno.Mensagem = "O usuário não possui acesso para a empresa informada";
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
                retorno.Mensagem = "Ocorreu uma falha ao enviar a chave da NF-e";
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }

            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            return retorno;
        }

        public Retorno<string> EnviarByteImagemNFe(Stream imagem)
        {
            try
            {
                string apiLink = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().APIOCRLink;
                string apiKey = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance()?.ObterConfiguracaoAmbiente().APIOCRKey;
                Servicos.Embarcador.Canhotos.LeitorOCR servicoLeitorOCR = new Servicos.Embarcador.Canhotos.LeitorOCR();

                servicoLeitorOCR.DefinirAPI(apiLink, apiKey);

                string chaveNFe = servicoLeitorOCR.ObterChaveNFe(imagem);

                return Retorno<string>.CriarRetornoSucesso(chaveNFe);
            }
            catch (ServicoException excecao)
            {
                return Retorno<string>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha ao enviar a imagem do Canhoto");
            }
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
                    if (!CargaEmMonitoramento(unitOfWork, codigoCarga))
                        return Retorno<bool>.CriarRetornoSucesso(true);

                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    if (motorista != null)
                    {
                        Dominio.Entidades.Veiculo veiculo = ObterVeiculoPorCargaNoMonitoramentoOuMotorista(unitOfWork, codigoCarga, motorista.Codigo);
                        if (veiculo != null)
                        {
                            DateTime DataPosicao = DateTime.Now;
                            DateTime.TryParseExact(data, "ddMMyyyyHHmmss", null, System.Globalization.DateTimeStyles.None, out DataPosicao);

                            Double lat = double.Parse(((string)latitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
                            Double lng = double.Parse(((string)longitude).Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);

                            Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao posicaoPendenteIntegracao = new Dominio.Entidades.Embarcador.Logistica.PosicaoPendenteIntegracao()
                            {
                                ID = long.Parse(data),
                                Data = DataPosicao,
                                DataVeiculo = DataPosicao,
                                DataCadastro = DateTime.Now,
                                IDEquipamento = usuario.ToString(),
                                Veiculo = veiculo,
                                Ignicao = 1,
                                Latitude = lat,
                                Longitude = lng,
                                Velocidade = 0,
                                Temperatura = 0,
                                Descricao = $"{latitude},{longitude}"
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
    }
}
