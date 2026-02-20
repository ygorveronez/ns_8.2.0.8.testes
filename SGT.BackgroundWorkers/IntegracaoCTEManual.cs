using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCTEManual : LongRunningProcessBase<IntegracaoCTEManual>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarIntegracoesCargaCTeManualPendentes(unitOfWork);
        }

        private void VerificarIntegracoesCargaCTeManualPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao repCargaCTeManualIntegracao = new Repositorio.Embarcador.Cargas.CargaCTeManualIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao> integracoes = repCargaCTeManualIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeManualIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarCargaCTeManual(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCargaCTeManual(integracao, _clienteUrlAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                        new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarCargaCTeManual(integracao, _clienteUrlAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.SAP:
                        new Servicos.Embarcador.Integracao.SAP.IntegracaoSAP(unitOfWork).IntegrarCTE(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.PortalCabotagem:
                        new Servicos.Embarcador.Integracao.PortalCabotagem.IntegracaoPortalCabotagem(unitOfWork).Integrar(integracao.CTe.Codigo);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.KMM:
                        if (integracao.Status == Dominio.Enumeradores.StatusIntegracaoCTeManual.CancelarCTeManual)
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCancelamentoCargaCTeManual(integracao);
                        else
                            new Servicos.Embarcador.Integracao.KMM.IntegracaoKMM(unitOfWork).IntegrarCargaCTeManual(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.CTePagamentoLoggi:
                        new Servicos.Embarcador.Integracao.Loggi.IntegracaoLoggi(unitOfWork, null).IntegrarCargaCTeManual(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Globus:
                        if (integracao.Status == Dominio.Enumeradores.StatusIntegracaoCTeManual.CancelarCTeManual)
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCancelamentoCargaCTeManual(integracao);
                        else
                            new Servicos.Embarcador.Integracao.Globus.IntegracaoGlobus(unitOfWork).IntegrarCargaCTeManual(integracao);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}