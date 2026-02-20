using CoreWCF;

namespace SGT.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFilial" in both code and config file together.
    [ServiceContract]
    public interface IPrePlanejamento
    {
        [OperationContract]
        Retorno<bool> BuscarPorCodigoIntegracao(string CodigoIntegracao);

        [OperationContract]
        Retorno<bool> SalvarPrePlanejamento(Dominio.ObjetosDeValor.Embarcador.PrePlanejamento.PrePlanejamento prePlanejamento);

    }
}
