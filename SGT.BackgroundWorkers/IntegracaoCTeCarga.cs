using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCTeCarga : LongRunningProcessBase<IntegracaoCTeCarga>
    {
        private bool ContemHoraCalculada = false;

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoTMS.GerarCargaDeCTEsNaoVinculadosACargas)
            {
                if (_tempoAguardarProximaExecucao == 0)
                {
                    DateTime dataAtual = DateTime.Now;
                    DateTime dataExecucao = new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, configuracaoTMS.HoraGeracaoCargaDeCTEsNaoVinculadosACargas.Hours, configuracaoTMS.HoraGeracaoCargaDeCTEsNaoVinculadosACargas.Minutes, 0);

                    if (dataExecucao > dataAtual)
                        _tempoAguardarProximaExecucao = (int)(dataExecucao - dataAtual).TotalMilliseconds;
                    else
                    {
                        dataExecucao = dataExecucao.AddDays(1);
                        _tempoAguardarProximaExecucao = (int)(dataExecucao - dataAtual).TotalMilliseconds;
                    }

                    ContemHoraCalculada = true;
                }
                else
                {
                    _tempoAguardarProximaExecucao = 86400000;
                    ContemHoraCalculada = false;
                }

                if (!ContemHoraCalculada)
                {
                    ContemHoraCalculada = true;
                    Servicos.Embarcador.CTe.CTEsImportados.GerarCargaDosCTesDisponiveis(unitOfWork, _tipoServicoMultisoftware);
                }
            }
            else
            {
                _tempoAguardarProximaExecucao = 86400000;
            }
        }       
    }
}