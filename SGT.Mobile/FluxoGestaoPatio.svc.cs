using Dominio.Excecoes.Embarcador;
using System;

namespace SGT.Mobile
{
    public class FluxoGestaoPatio : WebServiceBase, IFluxoGestaoPatio
    {
        #region Métodos Públicos

        public Retorno<bool> AvancarEtapa(string token, int usuario, int empresaMultisoftware, string qrCode)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    try
                    {
                        unitOfWork.Start();

                        Dominio.Entidades.Usuario motorista = ObterMotorista(unitOfWork, usuarioMobileCliente.UsuarioMobile.CPF);
                        Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(unitOfWork);

                        servicoFluxoGestaoPatio.AvancarEtapa(motorista, qrCode);

                        unitOfWork.CommitChanges();

                        return Retorno<bool>.CriarRetornoSucesso(true);
                    }
                    catch (Exception)
                    {
                        unitOfWork.Rollback();
                        throw;
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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao avançar a etapa de pátio");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
