using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace SGT.Mobile
{
    public class ControleCarregamento : WebServiceBase, IControleCarregamento
    {
        #region Métodos Públicos

        public Retorno<bool> FinalizarCarregamento(string token, int usuario, int empresaMultisoftware, int codigoControleCarregamento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);
                    servicoControleCarregamento.Finalizar(codigoControleCarregamento);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao finalizar o carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarCarregamento(string token, int usuario, int empresaMultisoftware, int codigoControleCarregamento)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);
                    servicoControleCarregamento.Iniciar(codigoControleCarregamento);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar o carregamento");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>> ObterTodosEmCarregamento(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> listaControleCarregamento = servicoControleCarregamento.ObterTodosEmCarregamento();

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoSucesso(listaControleCarregamento);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoExcecao("Ocorreu uma falha ao obter todos os carregamentos");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>> ObterTodosEmDoca(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.ControleCarregamento servicoControleCarregamento = new Servicos.Embarcador.Logistica.ControleCarregamento(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento> listaControleCarregamento = servicoControleCarregamento.ObterTodosEmDoca();

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoSucesso(listaControleCarregamento);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.ControleCarregamento>>.CriarRetornoExcecao("Ocorreu uma falha ao obter todos os carregamentos em doca");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
