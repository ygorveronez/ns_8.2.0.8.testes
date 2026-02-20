using Dominio.Excecoes.Embarcador;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class OrdemEmbarque(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IOrdemEmbarque
    {
        #region Métodos Globais

        public Retorno<bool> AtualizarOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueRetorno ordemEmbarqueRetorno)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            ValidarToken();
            ArmazenarLogIntegracao(ordemEmbarqueRetorno, unitOfWork);
            

            try
            {
                unitOfWork.Start();

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).AtualizarIntegracaoCargaOrdemEmbarque(ordemEmbarqueRetorno);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a ordem de embarque");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> AtualizarSituacaoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.OrdemEmbarqueSituacaoRetorno ordemEmbarqueSituacaoRetorno)
        {
            ValidarToken();

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                unitOfWork.Start();

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).AtualizarSituacaoOrdemEmbarque(ordemEmbarqueSituacaoRetorno);
                
                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao atualizar a situação da ordem de embarque");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarCancelamentoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoOrdemEmbarqueRequest request)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            ArmazenarLogIntegracao(request, unitOfWork);

            try
            {
                unitOfWork.Start();

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).AtualizarIntegracaoCancelamentoOrdemEmbarque(request);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar o cancelamento da ordem de embarque");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> ConfirmarTrocaPedidoOrdemEmbarque(Dominio.ObjetosDeValor.WebService.OrdemEmbarque.ConfirmacaoTrocaPedidoOrdemEmbarqueRequest request)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            ArmazenarLogIntegracao(request, unitOfWork);


            try
            {
                unitOfWork.Start();

                new Servicos.Embarcador.Integracao.Marfrig.IntegracaoOrdemEmbarqueMarfrig(unitOfWork).AtualizarIntegracaoTrocaPedido(request, TipoServicoMultisoftware, Auditado);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoDadosInvalidos(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao confirmar a troca de pedidos da ordem de embarque");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override string ObterCaminhoArquivoLog(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid() + "_integracaoOrdemEmbarque.txt");
        }

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceOrdemEmbarque;
        }

        #endregion
    }
}
