using AdminMultisoftware.Repositorio;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoMercante : LongRunningProcessBase<IntegracaoMercante>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarIntegracoesPendentes(unitOfWork);
        }

        private void VerificarIntegracoesPendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao repositorioArquivoMercanteIntegracao = new Repositorio.Embarcador.Documentos.ArquivoMercanteIntegracao(unitOfWork);

            Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab servicoIntegracaoIntercab = new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork);
            Servicos.Embarcador.Integracao.EMP.IntegracaoEMP servicoIntegracaoEMP = new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao> integracoes = repositorioArquivoMercanteIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Documentos.ArquivoMercanteIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        servicoIntegracaoIntercab.IntegrarArquivoMercanteIntegracao(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        servicoIntegracaoEMP.IntegrarArquivoMercanteIntegracao(integracao);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}