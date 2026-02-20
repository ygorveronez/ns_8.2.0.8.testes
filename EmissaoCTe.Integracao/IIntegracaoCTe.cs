using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IIntegracaoCTe
    {
        [OperationContract]
        Retorno<List<RetornoCTe>> Buscar(Dominio.Enumeradores.StatusIntegracao statusIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<object> Alterar(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.StatusIntegracao statusIntegracao, string token);

        [OperationContract]
        Retorno<RetornoCTe> BuscarPorCodigoCTe(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8);

        [OperationContract]
        Retorno<List<RetornoCTe>> BuscarPorCodigoMDFe(int codigoMDFe, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<RetornoCTe> BuscarPorNumeroDaNota(string cnpjEmpresaEmitente, string numeroNotaFiscal, string serieNotaFiscal, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<RetornoCTeMDFe> BuscarCTeMDFePorCodigoCTe(int codigoCTe, Dominio.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8);

        [OperationContract]
        Retorno<List<RetornoImpressora>> ConsultarImpressora(int numeroUnidade, string status, string nomeImpressora, string token);

        [OperationContract]
        Retorno<List<RetornoDetalhesCTe>> BuscarDetalhesCTePorPeriodo(string dataInicial, string dataFinal, string token);

        [OperationContract]
        Retorno<List<RetornoNFe>> BuscarNFeImpressaoPorCTe(int codigoEmpresaPai, int codigoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoImpressao situacao, string token);
    }
}
