using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class EmissaoDocumentosEtapaFreteLeve : LongRunningProcessBase<EmissaoDocumentosEtapaFreteLeve>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFrete(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware, 0, 500, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteLeve);
        }

        #endregion Métodos Protegidos
    }
}