using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmissaoNFe" in both code and config file together.
    [ServiceContract]
    public interface IEmissaoNFe
    {
        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoAssinatura(string cnpjEmpresa);

        [OperationContract]
        Retorno<string> SalvarRetornoEnvioNFe(Dominio.ObjetosDeValor.Embarcador.NotaFiscal.RetornoEnvioNFe ret, int codigoNFe);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoInutilizacao(string cnpjEmpresa);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoCancelamento(string cnpjEmpresa);

        [OperationContract]
        Retorno<List<Dominio.ObjetosDeValor.WebService.EmissaoNFe.EmissaoNFe>> BuscarNotasAguardandoCartaCorrecao(string cnpjEmpresa);
    }
}
