using System;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Autenticacao(IServiceProvider _serviceProvider) : IAutenticacao
	{
		public Retorno<string> Autenticar(Dominio.ObjetosDeValor.WebService.Autenticacao.Credenciais credenciais)
		{
			if (credenciais == null || string.IsNullOrWhiteSpace(credenciais.Usuairo) || string.IsNullOrWhiteSpace(credenciais.Senha))
				return Retorno<string>.CriarRetornoDadosInvalidos("É necessário informar corretamente as credenciais para autenticação.");

			Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
			Retorno<string> retorno = null;
			try
			{
				Repositorio.WebService.Integradora repositorioIntegradora = new Repositorio.WebService.Integradora(unitOfWork);

				Dominio.Entidades.WebService.Integradora integradora = repositorioIntegradora.BuscarPorUsuarioESenha(credenciais.Usuairo, credenciais.Senha);

				if (integradora != null )
				{
					unitOfWork.Start();

					integradora.TokenTemporario = Guid.NewGuid().ToString().Replace("-", "");
					integradora.DataExpiracao = DateTime.Now.AddMinutes(integradora.TempoExpiracao);

					repositorioIntegradora.Atualizar(integradora);

					unitOfWork.CommitChanges();

					retorno = Retorno<string>.CriarRetornoSucesso(integradora.TokenTemporario, string.Empty);
				} else { 
					retorno = Retorno<string>.CriarRetornoDadosInvalidos("Não foi possível realizar a autenticação para as credenciais informadas.");
				}
			}
			catch (Exception ex)
			{
				unitOfWork.Rollback();
				Servicos.Log.TratarErro(ex, "Autenticacao");
				retorno = Retorno<string>.CriarRetornoExcecao("Ocorreu uma falha o realizar a autenticação.");
			}
			finally 
			{ 
				unitOfWork.Dispose(); 
			}

			return retorno;
		}
	}
}
