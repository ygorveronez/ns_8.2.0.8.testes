using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IClientes" in both code and config file together.
    [ServiceContract]
    public interface IClientes
    {
        [OperationContract]
        Retorno<int> EnviarCNPJConsulta(string cnpj, string estado);

        [OperationContract]
        Retorno<RetornoConsultaCNPJ> ConsultarCNPJConsulta(string cnpj, string estado);
    }
}
