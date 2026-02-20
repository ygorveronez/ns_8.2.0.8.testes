using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 10000)]

    public class FilaCarregamento : LongRunningProcessBase<FilaCarregamento>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo servicoFilaCarregamentoVeiculo = new Servicos.Embarcador.Logistica.FilaCarregamentoVeiculo(unitOfWork, OrigemAlteracaoFilaCarregamento.Sistema);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();

            if (configuracaoEmbarcador.UtilizarFilaCarregamento)
            {
                servicoFilaCarregamentoVeiculo.DesvincularCargasPrazoAceitacaoEsgotado();
                servicoFilaCarregamentoVeiculo.VincularCargasSemTransportadorFilaCarregamentoDisponiveisPorTransportadorExclusivo(_tipoServicoMultisoftware);
                servicoFilaCarregamentoVeiculo.VincularCargasSemTransportadorFilaCarregamentoDisponiveis(_tipoServicoMultisoftware);
                servicoFilaCarregamentoVeiculo.VincularPreCargasSemTransportadorFilaCarregamentoDisponiveis(_tipoServicoMultisoftware);
            }
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }
    }
}
