using System.Collections.Generic;
using System.ServiceModel;

namespace EmissaoCTe.Integracao
{
    [ServiceContract]
    public interface IIntegracaoMDFe
    {
        [OperationContract]
        Retorno<object> Alterar(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.StatusIntegracao statusIntegracao, string token);

        [OperationContract]
        Retorno<List<RetornoMDFe>> Buscar(Dominio.Enumeradores.StatusIntegracao statusIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<RetornoMDFe> BuscarPorCodigoMDFe(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token, string codificarUTF8);

        [OperationContract]
        Retorno<RetornoMDFe> BuscarPorCodigoMDFeAverbado(int codigoMDFe, Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<List<RetornoMDFe>> BuscarPorTipoDeIntegracao(Dominio.Enumeradores.TipoIntegracaoMDFe tipoIntegracao, int codigoEmpresaPai, int numeroCarga, int numeroUnidade, Dominio.Enumeradores.TipoRetornoIntegracao tipoRetorno, string token);

        [OperationContract]
        Retorno<RetornoImpressora> IncluirImpressora(int numeroUnidade, string nomeImpressora, string token);

        [OperationContract]
        Retorno<List<RetornoImpressora>> ConsultarImpressora(int numeroUnidade, string status, string nomeImpressora, string token);

        [OperationContract]
        Retorno<RetornoImpressora> AlterarImpressora(int codigo, string numeroUnidade, string nomeImpressora, string status, string token);

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.Embarcador.EmissorDocumento.RetornoStatusMDFePorChave> BuscarStatusMDFePorChave(Dominio.ObjetosDeValor.MDFe.ConsultaStatusMDFePorChave consultaStatusMDFePorChave);
    }

}
