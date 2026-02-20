using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Enumerador;
using System;
using System.Linq;

namespace SGT.Mobile
{
    public class Motorista : WebServiceBase, IMotorista
    {
        #region Métodos Públicos

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa> AlterarReboque(string token, int usuario, int empresaMultisoftware, string placa)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Transportadores.MotoristaMobile servicoMotoristaMobile = new Servicos.Embarcador.Transportadores.MotoristaMobile(unitOfWork);
                    Dominio.Entidades.Veiculo veiculo = servicoMotoristaMobile.AlterarReboque(motorista, placa);

                    Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa empresaMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa()
                    {
                        Codigo = usuarioMobileCliente.Cliente.Codigo,
                        Descricao = usuarioMobileCliente.Cliente.RazaoSocial,
                        Transportadora = motorista.Empresa != null ? motorista.Empresa.RazaoSocial : usuarioMobileCliente.Cliente.RazaoSocial,
                        Placa = veiculo.Placa,
                        Reboques = (from reboque in veiculo.VeiculosVinculados select reboque.Placa).ToList(),
                        UrlEmbarcador = ObterURLCliente(usuarioMobileCliente),
                        VeiculoTipoManobra = veiculo.TipoManobra
                    };

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoSucesso(empresaMobile);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoExcecao("Ocorreu uma falha ao alterar o reboque");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa> AlterarTracao(string token, int usuario, int empresaMultisoftware, string placa)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                {
                    TipoAuditado = TipoAuditado.Usuario,
                    OrigemAuditado = OrigemAuditado.WebServiceMobile
                };
                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Transportadores.MotoristaMobile servicoMotoristaMobile = new Servicos.Embarcador.Transportadores.MotoristaMobile(unitOfWork, ObterURLCliente(usuarioMobileCliente));
                    Dominio.Entidades.Veiculo veiculo = servicoMotoristaMobile.AlterarTracao(motorista, placa, auditado);                   

                    Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa empresaMobile = new Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa()
                    {
                        Codigo = usuarioMobileCliente.Cliente.Codigo,
                        Descricao = usuarioMobileCliente.Cliente.RazaoSocial,
                        Transportadora = motorista.Empresa != null ? motorista.Empresa.RazaoSocial : usuarioMobileCliente.Cliente.RazaoSocial,
                        Placa = veiculo.Placa,
                        Reboques = (from reboque in veiculo.VeiculosVinculados select reboque.Placa).ToList(),
                        UrlEmbarcador = ObterURLCliente(usuarioMobileCliente),
                        VeiculoTipoManobra = veiculo.TipoManobra
                    };

                    return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoSucesso(empresaMobile);
                }
            }
            catch (WebServiceException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (ServicoException excecao)
            {
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<Dominio.ObjetosDeValor.Embarcador.Mobile.Autenticacao.Empresa>.CriarRetornoExcecao("Ocorreu uma falha ao alterar a tração");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> FinalizarJornada(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Transportadores.MotoristaMobile servicoMotoristaMobile = new Servicos.Embarcador.Transportadores.MotoristaMobile(unitOfWork);

                    servicoMotoristaMobile.FinalizarJornada(motorista, TipoServicoMultisoftware);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao finalizar a jornada");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<bool> IniciarJornada(string token, int usuario, int empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                    Servicos.Embarcador.Transportadores.MotoristaMobile servicoMotoristaMobile = new Servicos.Embarcador.Transportadores.MotoristaMobile(unitOfWork);

                    servicoMotoristaMobile.IniciarJornada(motorista);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao iniciar a jornada");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
