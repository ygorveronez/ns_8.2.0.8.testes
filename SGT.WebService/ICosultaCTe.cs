using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICosultaCTe" in both code and config file together.
    [ServiceContract]
    public interface ICosultaCTe
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz> SolicitarRequisicaoSefaz();

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.CTe.ConsultaSefaz> ConsultarSefaz(Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz requisicaoSefaz, string ChaveCTe, string Capcha);
    }
}
