using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class CargaIntegracaoHUBOfertas : LongRunningProcessBase<CargaIntegracaoHUBOfertas>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            try
            {
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).BuscarPorTipo(TipoIntegracao.HUB);

                if (tipoIntegracao == null) return;

                var repositorioConfiguracaoHUB = new Repositorio.Embarcador.Configuracoes.IntegracaoHUB(unitOfWork);
                var configuracaoHUB = await repositorioConfiguracaoHUB.BuscarPrimeiroRegistroAsync();

                if (configuracaoHUB == null || string.IsNullOrEmpty(configuracaoHUB.ConexaoServiceBUS)) return;

                await new Servicos.Embarcador.Integracao.HUB.IntegracaoHUBOfertas(unitOfWork, _tipoServicoMultisoftware).GerarIntegracoes();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro no worker CargaIntegracaoHUBOfertas: {ex.Message}", "CargaIntegracaoHUBOfertas");
            }

        }

    }
}
