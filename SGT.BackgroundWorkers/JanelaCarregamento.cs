using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class JanelaCarregamento : LongRunningProcessBase<JanelaCarregamento>
    {
        #region Métodos Privados

        private void VerificarCargasDisponibilizarParaTransportadoresPorDataLiberacao(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);

                servicoCargaJanelaCarregamentoTransportador.DisponibilizarParaTransportadoresPorDataLiberacao(_tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarCargasEmCotacaoTempoEsgotado(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao servicoCargaJanelaCarregamentoCotacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoCotacao(unitOfWork);

                servicoCargaJanelaCarregamentoCotacao.VerificarCotacaoComTempoEsgotado(_tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarNaoComparecimentos(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.AutomatizacaoNaoComparecimento servicoAutomatizacaoNaoComparecimento = new Servicos.Embarcador.Logistica.AutomatizacaoNaoComparecimento(unitOfWork);

                servicoAutomatizacaoNaoComparecimento.VerificarNaoComparecimentos();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarTransportadoresEnviarEmailPorPrazoEsgotado(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork);

                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaDisponibilizadaParaTransportadoresPorPrazoEsgotado();

                unitOfWork.CommitChanges();
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarTransportadoresTempoAceiteEncerrado(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);

                servicoCargaJanelaCarregamentoTransportador.RejeitarJanelasCarregamentoTransportadorPorTempoEncerrado(SituacaoCargaJanelaCarregamentoTransportador.AgAceite, _tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }
        private void VerificarTransportadoresTerceirosTempoInteresseEncerrado(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro servicoCargaJanelaCarregamentoTransportadorTerceiro = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportadorTerceiro(unitOfWork);

                servicoCargaJanelaCarregamentoTransportadorTerceiro.RejeitarJanelasCarregamentoTransportadorTerceiroPorTempoEncerrado(SituacaoCargaJanelaCarregamentoTransportador.Disponivel, _tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarTransportadoresTempoInteresseEncerrado(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador servicoCargaJanelaCarregamentoTransportador = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoTransportador(unitOfWork);

                servicoCargaJanelaCarregamentoTransportador.RejeitarJanelasCarregamentoTransportadorPorTempoEncerrado(SituacaoCargaJanelaCarregamentoTransportador.Disponivel, _tipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarTransportadoresTempoAceiteExpirando(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao servicoCargaJanelaCarregamentoNotificacao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoNotificacao(unitOfWork);

                servicoCargaJanelaCarregamentoNotificacao.EnviarEmailCargaTempoAceiteExpirandoParaTransportadores();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarIntegracoesPendentesResultadoLeilao(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem servicoCargaJanelaCarregamentoLeilao = new Servicos.Embarcador.Logistica.CargaJanelaCarregamentoViagem(unitOfWork, _tipoServicoMultisoftware);

                servicoCargaJanelaCarregamentoLeilao.EnviarIntegracaoResultadoLeilao();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarIntegracoesPendentesJanelaCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                new Servicos.Embarcador.Integracao.Klios.IntegracaoKlios(unitOfWork, _tipoServicoMultisoftware).EnviarIntegracoesPendentes();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarIntegracaoAguardandoRetornoJanelaCarregamento(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                new Servicos.Embarcador.Integracao.Klios.IntegracaoKlios(unitOfWork, _tipoServicoMultisoftware).EnviarIntegracoesAguardandoRetorno();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        private void VerificarJanelaDescarregamentoNaoComparecimento(Repositorio.UnitOfWork unitOfWork)
        {
            unitOfWork.FlushAndClear();

            try
            {
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();

                Servicos.Embarcador.Logistica.JanelaDescarga servicoJanelaDescarga = new Servicos.Embarcador.Logistica.JanelaDescarga(unitOfWork, _auditado, _tipoServicoMultisoftware, null, configuracaoTMS);

                servicoJanelaDescarga.SolicitarCancelamentoPorTempoLimiteNaoComparecimento();
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
        }

        #endregion Métodos Privados

        #region Métodos Sobrescritos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            VerificarCargasEmCotacaoTempoEsgotado(unitOfWork);
            VerificarCargasDisponibilizarParaTransportadoresPorDataLiberacao(unitOfWork);
            VerificarTransportadoresTempoAceiteEncerrado(unitOfWork);
            VerificarTransportadoresTempoInteresseEncerrado(unitOfWork);
            VerificarTransportadoresTempoAceiteExpirando(unitOfWork);
            VerificarTransportadoresEnviarEmailPorPrazoEsgotado(unitOfWork);
            VerificarNaoComparecimentos(unitOfWork);
            VerificarIntegracoesPendentesResultadoLeilao(unitOfWork);
            VerificarIntegracoesPendentesJanelaCarregamento(unitOfWork);
            VerificarIntegracaoAguardandoRetornoJanelaCarregamento(unitOfWork);
            VerificarTransportadoresTerceirosTempoInteresseEncerrado(unitOfWork);
            VerificarJanelaDescarregamentoNaoComparecimento(unitOfWork);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        #endregion Métodos Sobrescritos
    }
}
