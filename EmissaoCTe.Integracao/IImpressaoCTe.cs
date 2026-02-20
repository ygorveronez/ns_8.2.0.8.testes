using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IImpressaoCTe
    {
        [OperationContract]
        Retorno<int> SolicitarImpressao(int[] codigosCTes, string cnpjEmpresaPai);

        [OperationContract]
        Retorno<int> Alterar(int codigoImpressao, Dominio.Enumeradores.StatusImpressaoCTe status);

        [OperationContract]
        Retorno<List<RetornoImpressao>> ObterImpressoesPendentes(int codigoEmpresaPai, int numeroUnidade);
    }
}
