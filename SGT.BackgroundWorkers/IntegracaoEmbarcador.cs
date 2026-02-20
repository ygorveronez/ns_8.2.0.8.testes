using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoEmbarcador : LongRunningProcessBase<IntegracaoEmbarcador>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = await repositorioConfiguracaoTMS.BuscarConfiguracaoPadraoAsync();

            if (configuracaoTMS.ImportarCargasMultiEmbarcador)
            {
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCargasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCTesCargasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork, configuracaoTMS);
                await new Servicos.Embarcador.Carga.CargaImportacaoEmbarcador(unitOfWork, _tipoServicoMultisoftware, cancellationToken).ImportarMDFesCargasEmbarcadorAsync();
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.GerarCargasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCancelamentosEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.ImportarCTesCanceladosEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                Servicos.Embarcador.Carga.CargaImportacaoEmbarcador.GerarCancelamentosEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);

                if (_tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.ImportarOcorrenciasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.ImportarCTesOcorrenciasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork, configuracaoTMS);
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.GerarOcorrenciasImportadasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.ImportarOcorrenciasCanceladasEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.ImportarCTesCanceladosEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                    Servicos.Embarcador.Ocorrencia.OcorrenciaImportacaoEmbarcador.GerarCancelamentosOcorrenciaEmbarcador(out _, _tipoServicoMultisoftware, unitOfWork);
                }
            }

            if (configuracaoTMS.GerarCargaDeMDFesNaoVinculadosACargas)
                Servicos.Embarcador.CTe.CTEsImportados.GerarCargaDosMDFesDisponiveis(unitOfWork, _tipoServicoMultisoftware);
        }
    }
}