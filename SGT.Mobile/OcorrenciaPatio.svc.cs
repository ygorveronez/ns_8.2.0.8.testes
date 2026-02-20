using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace SGT.Mobile
{
    public class OcorrenciaPatio : WebServiceBase, IOcorrenciaPatio
    {
        #region Métodos Públicos

        public Retorno<bool> Adicionar(string token, int usuario, int empresaMultisoftware, int codigoCentroCarregamento, int codigoVeiculo, int codigoTipo, string descricao)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario, empresaMultisoftware, unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);
                    Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioDados dados = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.OcorrenciaPatioDados()
                    {
                        CodigoCentroCarregamento = codigoCentroCarregamento,
                        CodigoTipo = codigoTipo,
                        CodigoVeiculo = codigoVeiculo,
                        Descricao = descricao,
                        TipoLancamento = TipoLancamento.Manual
                    };

                    servicoOcorrenciaPatio.Adicionar(dados);

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
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao adicionar a ocorrência de pátio");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>> ObterTipos(string token, string usuario, string empresaMultisoftware)
        {
            ValidarToken(token);

            AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.AdminStringConexao);

            try
            {
                AdminMultisoftware.Dominio.Entidades.Mobile.UsuarioMobileCliente usuarioMobileCliente = ObterUsuarioMobileCliente(usuario.ToInt(), empresaMultisoftware.ToInt(), unitOfWorkAdmin);

                using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao(usuarioMobileCliente)))
                {
                    Servicos.Embarcador.GestaoPatio.OcorrenciaPatio servicoOcorrenciaPatio = new Servicos.Embarcador.GestaoPatio.OcorrenciaPatio(unitOfWork);
                    List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo> listaOcorrenciaPatioTipo = servicoOcorrenciaPatio.ObterTiposOcorrencia();

                    return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>>.CriarRetornoSucesso(listaOcorrenciaPatioTipo);
                }
            }
            catch (ServicoException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (WebServiceException excecao)
            {
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return Retorno<List<Dominio.ObjetosDeValor.Embarcador.Mobile.GestaoPatio.OcorrenciaPatioTipo>>.CriarRetornoExcecao("Ocorreu uma falha ao obter os tipos de ocorrência de pátio");
            }
            finally
            {
                unitOfWorkAdmin.Dispose();
            }
        }

        #endregion
    }
}
