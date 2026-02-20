using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoProcessamentoEDIFTP : LongRunningProcessBase<IntegracaoProcessamentoEDIFTP>
    {
        #region Métodos protegidos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            LocalizarProcessarIntegracaoEDIFTPPendente(unitOfWork, unitOfWorkAdmin);
        }

        #endregion

        #region Métodos privados

        private void LocalizarProcessarIntegracaoEDIFTPPendente(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {

            Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repintegracao = new Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);

            // Busca a proxima EDI pendente
            Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoPendente = repintegracao.BuscarProximaIntegracaoPendente();

            if (integracaoProcessamentoPendente != null)
            {

                // Registra o início do processamento da EDI, de "Pendente" para "Processando"
                IndicarInicioDoProcessamento(unitOfWork, repintegracao, integracaoProcessamentoPendente);

                // Processa a planilha importada
                ProcessarImportacaoEDIFTP(unitOfWork, unitOfWorkAdmin, repintegracao, integracaoProcessamentoPendente);

            }
        }

        public void ProcessarImportacaoEDIFTP(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repintegracao, Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoPendente)
        {
            try
            {
                //Integra o arquivo EDI
                Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP servicoIntegracaoEDI = new Servicos.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoProcessamentoIntegracaoEDIFTP retorno = servicoIntegracaoEDI.IntegrarEDIFTP(integracaoProcessamentoPendente, _tipoServicoMultisoftware, unitOfWork, unitOfWorkAdmin);

                string mensagem;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP situacao;
                if (retorno.Sucesso)
                {
                    mensagem = "Registros integrados com sucesso!";
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.Integrado;
                }
                else
                {
                    mensagem = retorno.MensagemAviso;
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.Falha;
                }

                // Registra o fim do processamento da planilha com erro ou sucesso
                IndicarFimDoProcessamento(unitOfWork, repintegracao, integracaoProcessamentoPendente, situacao, mensagem);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                unitOfWork.FlushAndClear();

                IndicarFimDoProcessamento(unitOfWork, repintegracao, integracaoProcessamentoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.Falha, ex.Message);
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private void IndicarInicioDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repintegracao, Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoPendente)
        {
            // Registra o início do processamento, de "Pendente" para "em andamento"
            unitOfWork.Start();
            try
            {
                integracaoProcessamentoPendente.DataIntegracao = DateTime.Now;
                integracaoProcessamentoPendente.MensagemRetorno = "";
                integracaoProcessamentoPendente.NumeroTentativas += 1;
                integracaoProcessamentoPendente.SituacaoIntegracaoEDIFTP = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.EmAndamento;
                repintegracao.Atualizar(integracaoProcessamentoPendente);
                unitOfWork.CommitChanges();
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                throw;
            }
        }

        private void IndicarFimDoProcessamento(Repositorio.UnitOfWork unitOfWork, Repositorio.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP repintegracao, Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP integracaoProcessamentoPendente, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP SituacaoIntegracaoEDIFTP, string mensagem)
        {
            // Registra o fim do processamento da planilha, de "Processando" para "Sucesso" ou "Erro"
            unitOfWork.Start();
            integracaoProcessamentoPendente.SituacaoIntegracaoEDIFTP = SituacaoIntegracaoEDIFTP;
            integracaoProcessamentoPendente.MensagemRetorno = mensagem;

            repintegracao.Atualizar(integracaoProcessamentoPendente);
            unitOfWork.CommitChanges();
        }

        #endregion
    }
}