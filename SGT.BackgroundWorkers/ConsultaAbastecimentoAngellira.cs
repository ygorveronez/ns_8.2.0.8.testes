using SGT.BackgroundWorkers.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 14400000)]
    public class ConsultaAbastecimentoAngellira : LongRunningProcessBase<ConsultaAbastecimentoAngellira>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.SistemaImportacao
            };

            Servicos.Embarcador.Integracao.AngelLira.ConsultaAbastecimento.ConsultarAbastecimentosPendentes(unitOfWork, _stringConexao, _tipoServicoMultisoftware, auditado);
            Servicos.Embarcador.Integracao.AngelLira.ConsultaAbastecimento.ProcessarAbastecimentosPendentes(unitOfWork, _stringConexao, _tipoServicoMultisoftware, auditado);
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