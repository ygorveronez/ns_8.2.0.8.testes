using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 100000)]
    public class AbastecimentoInterno : LongRunningProcessBase<AbastecimentoInterno>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;

            ProcessarTransferenciaTanqueAbastecimento();
            SolicitarIntegracaoAbastecimentoAutomatizado();
            ProcessarIntegracoesPendentesReservaAbastecimento();
            ProcessarIntegracoesPendentesAutorizacaoAbastecimento();
            ValidarIntegracoesPendentesAbastecimento();
        }

        #endregion 

        #region Metodos Privados

        private void ProcessarTransferenciaTanqueAbastecimento()
        {
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia repLocalArmazenamentoProdutoTransferencia = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProdutoTransferencia> transferenciasPendentes = repLocalArmazenamentoProdutoTransferencia.BuscarTransferenciasPendentesData();

            foreach (var transferencia in transferenciasPendentes)
                Servicos.Embarcador.Abastecimento.AbastecimentoInterno.ProcessarTransferenciaTanque(transferencia , _unitOfWork);
            
        }
        private void SolicitarIntegracaoAbastecimentoAutomatizado()
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado(_unitOfWork);
            Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao serAbastecimentoInternoIntegracao = new Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao(_unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizado> listaLiberacaoAbastecimentoAutomatizado = repLiberacaoAbastecimentoAutomatizado.BuscarLiberacoesPendentes();

                foreach (var liberacaoAbastecimentoAutomatizado in listaLiberacaoAbastecimentoAutomatizado)
                {
                    serAbastecimentoInternoIntegracao.VerificarIntegracoesAbastecimentoAutomatizado(liberacaoAbastecimentoAutomatizado, _unitOfWork);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarIntegracoesPendentesReservaAbastecimento()
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao serAbastecimentoInternoIntegracao = new Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao(_unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> integracoesPendentes = repLiberacaoAbastecimentoAutomatizado.BuscarIntegracoesPendentesPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.PendenteReserva);

                foreach (var integracao in integracoesPendentes)
                    serAbastecimentoInternoIntegracao.ProcessarIntegracaoPendenteReservaAbastecimento(integracao, _unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

        }

        private void ProcessarIntegracoesPendentesAutorizacaoAbastecimento()
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao serAbastecimentoInternoIntegracao = new Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao(_unitOfWork);
            try
            {
                List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> integracoesPendentes = repLiberacaoAbastecimentoAutomatizado.BuscarIntegracoesPendentesPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.PendenteAutorizacao);

                foreach (var integracao in integracoesPendentes)
                    serAbastecimentoInternoIntegracao.ProcessarIntegracaoPendenteAutorizacaoAbastecimento(integracao, _unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

        }

        private void ValidarIntegracoesPendentesAbastecimento()
        {
            Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao repLiberacaoAbastecimentoAutomatizado = new Repositorio.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao(_unitOfWork);
            Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao serAbastecimentoInternoIntegracao = new Servicos.Embarcador.Abastecimento.AbastecimentoInternoIntegracao(_unitOfWork);
            try
            {
                int minutos = 2;

                List<Dominio.Entidades.Embarcador.AbastecimentoInterno.LiberacaoAbastecimentoAutomatizadoIntegracao> integracoesPendentes = repLiberacaoAbastecimentoAutomatizado.BuscarIntegracoesPendentesDIFFSituacoesOcioso( 
                    new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento> { 
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Finalizado, 
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Pendente, 
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.PendenteReserva,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.PendenteAutorizacao,
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.Autorizado, 
                        Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoAbastecimento.AgRetornoAbastecimento}, minutos);

                foreach (var integracao in integracoesPendentes)
                    serAbastecimentoInternoIntegracao.CancelarAbastecimentoOcioso(integracao, _unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

        }
        #endregion
    }
}