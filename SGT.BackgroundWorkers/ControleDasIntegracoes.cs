using AdminMultisoftware.Repositorio;
using SGT.BackgroundWorkers.Utils;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 14400000)]
    public class ControleDasIntegracoes : LongRunningProcessBase<ControleDasIntegracoes>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarArquivosForaDoPrazo(unitOfWork);
        }

        private void VerificarArquivosForaDoPrazo(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Integracao.ControleDasIntegracoes repositorioControleDasIntegracaoes = new Repositorio.Embarcador.Integracao.ControleDasIntegracoes(unitOfWork);
            Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo repositorioControleDasIntegracaoesAnexo = new Repositorio.Embarcador.Integracao.ControleDasIntegracoesAnexo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repositorioConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repositorioConfiguracaoArquivo.BuscarPrimeiroRegistro();

            List<long> controlesDeIntegracaoForaDoPrazo = repositorioControleDasIntegracaoes.BuscarCodigoIntegracaoesVencidos();

            foreach (long codigoControleFora in controlesDeIntegracaoForaDoPrazo)
            {
                List<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo> existeControleIntegracaoPrazo = repositorioControleDasIntegracaoesAnexo.BuscarPorControleIntegracao(codigoControleFora);
                if (existeControleIntegracaoPrazo == null || existeControleIntegracaoPrazo.Count == 0)
                    continue;

                foreach (var controleAnexo in existeControleIntegracaoPrazo)
                {
                    string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "ControleDasIntegracoes", controleAnexo.NomeArquivo);
                    Utilidades.File.RemoverArquivo(caminhoArquivo);
                    repositorioControleDasIntegracaoesAnexo.Deletar(controleAnexo);
                }

                Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoes controleProcessado = repositorioControleDasIntegracaoes.BuscarPorCodigo(codigoControleFora);
                controleProcessado.Expiou = true;
                repositorioControleDasIntegracaoes.Atualizar(controleProcessado);
            }
        }
    }
}