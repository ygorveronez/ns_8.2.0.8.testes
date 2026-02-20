using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 180000)]

    public class EnvioDocumentacaoLote : LongRunningProcessBase<EnvioDocumentacaoLote>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.Documentos.EnvioDocumentacaoLote.ProcessarEnvioDocumentacaoLote(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _urlAcesso);
            Servicos.Embarcador.Documentos.EnvioDocumentacaoLote.ProcessarEnvioDocumentacaoFinalizacaoCarga(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _urlAcesso);
            Zeus.Embarcador.ZeusNFe.EnvioDocumentacaoAFRMM.ProcessarEnvioAutomaticoDocumentacao(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _urlAcesso);
        }

        #region Métodos Privados
        private static TimeSpan ObterHorarioExecucao(Repositorio.UnitOfWork unitOfWork)
        {
            string horarioExecucaoConfigurado = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().HorarioExecucaoThreadDiaria;

            if (!string.IsNullOrWhiteSpace(horarioExecucaoConfigurado))
            {
                string[] horarioExecucaoConfiguradoParticionado = horarioExecucaoConfigurado.Split(':');

                if (horarioExecucaoConfiguradoParticionado.Length == 2)
                {
                    int horas = horarioExecucaoConfiguradoParticionado[0].ToInt();
                    int minutos = horarioExecucaoConfiguradoParticionado[1].ToInt();

                    return new TimeSpan(hours: horas, minutes: minutos, seconds: 0);
                }
            }

            return new TimeSpan(hours: 0, minutes: 15, seconds: 0);
        }

        #endregion
    }
}