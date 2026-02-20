using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]
    public class EmissaoDocumentosEtapaFreteIntegracao : LongRunningProcessBase<EmissaoDocumentosEtapaFreteIntegracao>
    {
        #region Métodos Protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            new Servicos.Embarcador.Carga.Carga(unitOfWork).SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao(unitOfWork, _stringConexao, _webServiceConsultaCTe, _tipoServicoMultisoftware, 0, 0, IdentificadorControlePosicaoThread.SolicitarEmissaoDocumentosAutorizadosNaEtapaDeFreteIntegracao);

        }

        #endregion Métodos Protegidos
    }
}