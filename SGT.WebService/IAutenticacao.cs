using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAutenticacao" in both code and config file together.
    [ServiceContract]
	public interface IAutenticacao
	{
		[OperationContract]
		Retorno<string> Autenticar(Dominio.ObjetosDeValor.WebService.Autenticacao.Credenciais credenciais);
	}
}
