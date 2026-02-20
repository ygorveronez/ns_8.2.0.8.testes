using AdminMultisoftware.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMDFe : LongRunningProcessBase<IntegracaoMDFe>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarIntegracoesPendentes(unitOfWork);
            VerificarIntegracoesCancelamentoPendentes(unitOfWork);
        }

        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao repositorioCargaMDFeAquaviarioManualIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao> integracoes = repositorioCargaMDFeAquaviarioManualIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeAquaviarioManualIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        {
                            if (integracao.IntegrarEncerramento)
                                servicoIntegracaoIntercab.IntegrarCargaMDFeEncerramentoManual(integracao);
                            else
                                servicoIntegracaoIntercab.IntegrarCargaMDFeAquaviarioManual(integracao);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void VerificarIntegracoesCancelamentoPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao repCargaMDFeManualCancelamentoIntegracao = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao> integracoes = repCargaMDFeManualCancelamentoIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamentoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarCargaMDFeCancelamentoManual(integracao);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}