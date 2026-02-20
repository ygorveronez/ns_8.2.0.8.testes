using System.Collections.Generic;
using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFilial" in both code and config file together.
    [ServiceContract]
    public interface IFilial
    {
        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliais(int? inicio, int? limite);

        [OperationContract]
        Retorno<bool> SalvarFilial(Dominio.ObjetosDeValor.Embarcador.Filial.Filial filialIntegracao);

        [OperationContract]
        Retorno<bool> ConfirmarIntegracaoFilial(List<int> protocolos);

        [OperationContract]
        Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliaisPendentesIntegracao(int? quantidade);

        [OperationContract]
        Retorno<bool> InformarVolumesTanques(Dominio.ObjetosDeValor.WebService.Filial.FilialTanque filialTanque);
    }
}
