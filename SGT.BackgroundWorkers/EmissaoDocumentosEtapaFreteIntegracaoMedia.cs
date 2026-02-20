using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class EmissaoDocumentosEtapaFreteIntegracaoMedia : LongRunningProcessBase<EmissaoDocumentosEtapaFreteIntegracaoMedia>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware, 500, 1500, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracaoMedia);
        }

        #endregion Métodos Protegidos
    }
}