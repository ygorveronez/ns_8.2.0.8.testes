using Dominio.Excecoes.Embarcador;
using System;
using System.Collections.Generic;

namespace SGT.Mobile
{
    public class Manobra : WebServiceBase, IManobra
    {
        #region Métodos Privados

        private Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>> ObterManobras(string token, string usuario, string empresaMultisoftware, bool retornarManobrasPorMotoristaTracaoManobra)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra> manobras = retornarManobrasPorMotoristaTracaoManobra ? servicoManobraMobile.ObterManobrasPorMotoristaTracaoManobra(motorista) : servicoManobraMobile.ObterManobrasPorMotorista(motorista);

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>>.CriarRetornoSucesso(manobras);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>>.CriarRetornoExcecao("Ocorreu uma falha ao consultar as manobras");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion

        #region Métodos Públicos

        public Retorno<bool> FinalizarIntervalo(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.FinalizarIntervalo(motorista);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao finalizar o intervalo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao> FinalizarManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra, string QRCodeLocal)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Embarcador.Logistica.AreaVeiculoPosicao local = ObterLocal(unitOfWork, QRCodeLocal);

                    if (local == null)
                    {
                        List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao> locais = ObterLocais(unitOfWork, QRCodeLocal);

                        return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoSucesso(
                            new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao() { Finalizada = false, Locais = locais }
                        );
                    }

                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.FinalizarManobra(codigoManobra, local);

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoSucesso(
                        new Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao() { Finalizada = true, Locais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.AreaVeiculoPosicao>() }
                    );
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.FinalizarAcaoPosicao>.CriarRetornoExcecao("Ocorreu uma falha ao finalizar a manobra");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarIntervalo(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.IniciarIntervalo(motorista);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar o intervalo");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>> ObterManobras(string token, string usuario, string empresaMultisoftware)
        {
            return ObterManobras(token, usuario, empresaMultisoftware, retornarManobrasPorMotoristaTracaoManobra: true);
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.Logistica.Manobra>> ObterManobrasPorMotorista(string token, string usuario, string empresaMultisoftware)
        {
            return ObterManobras(token, usuario, empresaMultisoftware, retornarManobrasPorMotoristaTracaoManobra: false);
        }

        public Retorno<bool> RemoverManobraTracaoVinculada(string token, int usuario, int empresaMultisoftware, int codigoManobra)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.RemoverManobraTracaoVinculada(codigoManobra);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover a manobra vinculada");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> RemoverReservaManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.RemoverReservaManobra(motorista, codigoManobra);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover a reserva da manobra");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> ReservarManobra(string token, int usuario, int empresaMultisoftware, int codigoManobra)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.ReservarManobra(motorista, codigoManobra);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao remover a reserva da manobra");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> VincularManobraTracao(string token, int usuario, int empresaMultisoftware, int codigoManobra, string placa)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Logistica.ManobraMobile servicoManobraMobile = new Servicos.Embarcador.Logistica.ManobraMobile(unitOfWork);

                    servicoManobraMobile.VincularManobraTracao(motorista, codigoManobra, placa);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao vincular a manobra");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
