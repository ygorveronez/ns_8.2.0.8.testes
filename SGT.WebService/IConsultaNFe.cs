using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IConsultaNFe" in both code and config file together.
    [ServiceContract]
    public interface IConsultaNFe
    {
        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz> SolicitarRequisicaoSefaz();

        [OperationContract]
        Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz> ConsultarSefaz(Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz, string ChaveNFe, string Captcha);
    }
}
