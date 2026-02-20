using CoreWCF;
using Dominio.Excecoes.Embarcador;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Fretes(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IFretes
    {
        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete> CalcularFrete(Dominio.ObjetosDeValor.WebService.Frete.DadosCalculoFrete dadosCalculoFrete)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {

                ValidarToken();

                return new Servicos.Embarcador.Frete.CalculoFrete(unitOfWork, TipoServicoMultisoftware).CalcularFrete(dadosCalculoFrete);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Dominio.ObjetosDeValor.WebService.Retorno<Dominio.ObjetosDeValor.WebService.Frete.RetornoCalculoFrete>.CriarRetornoExcecao("Ocorreu uma falha genérica ao realizar o cálculo do frete.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>>> ObterCotacao(Dominio.ObjetosDeValor.WebService.Pedido.Cotacao cotacao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {

                ValidarToken();

                List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao> cotacoes = new Servicos.Embarcador.Pedido.Cotacao(unitOfWork).ObterCotacoes(cotacao, Conexao.createInstance(_serviceProvider).AdminStringConexao, TipoServicoMultisoftware);

                return await Task.FromResult(Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>>.CriarRetornoSucesso(cotacoes));
            }
            catch (ServicoException ex)
            {
                Servicos.Log.TratarErro(ex);

                return await Task.FromResult(Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>>.CriarRetornoDadosInvalidos(ex.Message));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return await Task.FromResult(Dominio.ObjetosDeValor.WebService.Retorno<List<Dominio.ObjetosDeValor.WebService.Pedido.RetornoCotacao>>.CriarRetornoExcecao("Ocorreu uma falha genérica ao realizar o cálculo do frete."));
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<bool> SolicitarRecalculoFrete(Dominio.ObjetosDeValor.WebService.Frete.SolicitacaoRecalculoFrete solicitacaoRecalculoFrete)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            ValidarToken();

            try
            {
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorProtocolo(solicitacaoRecalculoFrete.ProtocoloIntegracaoCarga);

                if (carga == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("O protocolo da carga informada não existe.");

                unitOfWork.Start();

                Servicos.Embarcador.Carga.Frete servicoFrete = new Servicos.Embarcador.Carga.Frete(unitOfWork);
                servicoFrete.SolicitarRecalculoFrete(carga, null, TipoServicoMultisoftware, Auditado, false, unitOfWork);

                unitOfWork.CommitChanges();

                return Retorno<bool>.CriarRetornoSucesso(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return Retorno<bool>.CriarRetornoExcecao(excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return Retorno<bool>.CriarRetornoExcecao("Ocorreu uma falha ao solicitar o recálculo do frete!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceFretes;
        }

        #endregion
    }
}
