using System;
using Microsoft.AspNetCore.Http;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Rota(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IRota
    {
        #region Métodos Públicos

        public Retorno<int> AdicionarRota(Dominio.ObjetosDeValor.WebService.Rota.Rota rotaIntegracao)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                unitOfWork.Start();

                if (!Servicos.Embarcador.Carga.RotaFrete.AdicionarRotaFrete(out string mensagemErro, out Dominio.Entidades.RotaFrete rotaFrete, integradora, rotaIntegracao, Auditado, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return Retorno<int>.CriarRetornoDadosInvalidos(mensagemErro);
                }

                unitOfWork.CommitChanges();

                return Retorno<int>.CriarRetornoSucesso(rotaFrete.Codigo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unitOfWork.Rollback();

                return Retorno<int>.CriarRetornoExcecao($"Ocorreu uma falha ao adicionar a rota.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceCargas;
        }

        #endregion
    }
}
