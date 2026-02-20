using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCartaCorrecao : LongRunningProcessBase<IntegracaoCartaCorrecao>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {            
            VerificarIntegracoesCartaCorrecaoCTePendentes(unitOfWork);
        }

        private void VerificarIntegracoesCartaCorrecaoCTePendentes(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao repositorioCartaCorrecaoIntegracao = new Repositorio.Embarcador.Cargas.CartaCorrecaoIntegracao(unitOfWork);

            int numeroTentativas = 3;
            double minutosACadaTentativa = 5;

            List<Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao> integracoes = repositorioCartaCorrecaoIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa);

            foreach (Dominio.Entidades.Embarcador.Cargas.CartaCorrecaoIntegracao integracao in integracoes)
            {
                switch (integracao.TipoIntegracao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Intercab:
                        new Servicos.Embarcador.Integracao.Intercab.IntegracaoIntercab(unitOfWork).IntegrarCartaCorrecaoCTe(integracao);
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.EMP:
                        new Servicos.Embarcador.Integracao.EMP.IntegracaoEMP(unitOfWork).IntegrarCartaCorrecaoCTe(integracao, _clienteUrlAcesso?.URLAcesso ?? "");
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NFTP:
                        new Servicos.Embarcador.Integracao.NFTP.IntegracaoNFTP(unitOfWork).IntegrarCartaCorrecaoCTe(integracao, _clienteUrlAcesso?.URLAcesso ?? "");
                        break;


                    default:
                        break;
                }
            }
        }
    }
}