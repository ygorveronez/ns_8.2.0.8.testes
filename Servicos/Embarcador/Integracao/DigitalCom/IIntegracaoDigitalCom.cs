namespace Servicos.Embarcador.Integracao.DigitalCom
{
    public interface IIntegracaoDigitalCom
    {
        void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao);

        bool PermitirGerarIntegracao();
    }
}
