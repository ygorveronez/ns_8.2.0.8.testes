using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 180000)]

    public class EnvioDocumentacaoAFRMM : LongRunningProcessBase<EnvioDocumentacaoAFRMM>
    {
        
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            var horarioExecucao = ObterHorarioExecucao(unitOfWork);
            DateTime dataBaseProximaExcecucao = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, horarioExecucao.Hours, horarioExecucao.Minutes, horarioExecucao.Seconds);
            DateTime dataProximaExecucao = dataBaseProximaExcecucao;

            if ((dataProximaExecucao - DateTime.Now).TotalMilliseconds < 0)
                dataProximaExecucao = dataProximaExecucao.AddDays(1);

            DateTime dataMinimaExcecucao = dataBaseProximaExcecucao.AddMinutes(-5);
            DateTime dataMaximaExcecucao = dataBaseProximaExcecucao.AddMinutes(25);
            bool dataAtualDentroIntervaloExecucao = (DateTime.Now >= dataMinimaExcecucao) && (DateTime.Now <= dataMaximaExcecucao);

            if (dataAtualDentroIntervaloExecucao)                            
                Zeus.Embarcador.ZeusNFe.EnvioDocumentacaoAFRMM.ProcessarEnvioDocumentacaoAFRMM(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _urlAcesso, true);                            

            Zeus.Embarcador.ZeusNFe.EnvioDocumentacaoAFRMM.ProcessarEnvioDocumentacaoAFRMM(unitOfWork, _stringConexao, _tipoServicoMultisoftware, _stringConexaoAdmin, _urlAcesso, false);
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