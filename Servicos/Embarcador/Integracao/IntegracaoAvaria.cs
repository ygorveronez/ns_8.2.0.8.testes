using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao
{
    public class IntegracaoAvaria : ServicoBase
    {
        public IntegracaoAvaria(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken)
        {
        }

        public async Task VerificarIntegracoesPendentesAsync()
        {
            Repositorio.Embarcador.Avarias.LoteEDIIntegracao repositorioLoteEDIIntegracao = new Repositorio.Embarcador.Avarias.LoteEDIIntegracao(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Avarias.Lote repositorioLote = new Repositorio.Embarcador.Avarias.Lote(_unitOfWork, _cancellationToken);

            IntegracaoEDI servicoIntegracaoEdi = new IntegracaoEDI(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Avarias.Lote> lotes = await servicoIntegracaoEdi.VerificarIntegracoesPendentesLotesAsync();

            foreach (Dominio.Entidades.Embarcador.Avarias.Lote lote in lotes)
            {
                if (lote.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.EmIntegracao)
                {
                    if (await repositorioLoteEDIIntegracao.ContarPorLoteAsync(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) > 0)
                    {
                        lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.FalhaIntegracao;
                        await repositorioLote.AtualizarAsync(lote);
                        continue;
                    }
                    else if (await repositorioLoteEDIIntegracao.ContarPorLoteAsync(lote.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao) > 0)
                    {
                        continue;
                    }
                    else
                    {
                        lote.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Finalizada;
                        await repositorioLote.AtualizarAsync(lote);
                    }
                    Servicos.Embarcador.Hubs.Avaria hubAvaria = new Hubs.Avaria();
                    hubAvaria.InformarLoteAtualizado(lote.Codigo);
                }
            }
        }

    }
}
