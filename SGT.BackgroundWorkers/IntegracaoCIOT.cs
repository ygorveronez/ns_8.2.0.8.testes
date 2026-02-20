using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class IntegracaoCIOT : LongRunningProcessBase<IntegracaoCIOT>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Servicos.Embarcador.CIOT.CIOT srvCIOT = new Servicos.Embarcador.CIOT.CIOT();

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;

            srvCIOT.IntegrarMotoristasPendentesIntegracao(numeroTentativas, minutosACadaTentativa, unitOfWork);
            srvCIOT.IntegrarVeiculosPendentesIntegracao(numeroTentativas, minutosACadaTentativa, unitOfWork);
            VerificarContratoFretePendenteEncerramentoCIOT(_tipoServicoMultisoftware, unitOfWork);

            Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto servicoContratoFreteAcrescimoDesconto = new Servicos.Embarcador.Terceiros.ContratoFreteAcrescimoDesconto(unitOfWork);
            servicoContratoFreteAcrescimoDesconto.VerificarIntegracoesPendentes(_tipoServicoMultisoftware);
            servicoContratoFreteAcrescimoDesconto.GerarIntegracoes(_tipoServicoMultisoftware);

            TestarDisponibilidadeIntegracoes(unitOfWork);
        }

        #region Métodos Privados

        private void VerificarContratoFretePendenteEncerramentoCIOT(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Terceiros.ContratoFrete repContratoFrete = new Repositorio.Embarcador.Terceiros.ContratoFrete(unidadeDeTrabalho);
            Servicos.Embarcador.Terceiros.ContratoFrete serContratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Terceiros.ContratoFrete> contratosFrete = repContratoFrete.BuscarContratoPendenteEncerramentoCIOT();
            foreach (Dominio.Entidades.Embarcador.Terceiros.ContratoFrete contrato in contratosFrete)
            {
                string mensagem = "";
                Dominio.Entidades.Embarcador.Documentos.CIOT ciotEncerrado = serContratoFrete.SolicitarFinalizacaoPorCIOT(contrato, tipoServicoMultisoftware, unidadeDeTrabalho, out mensagem);

                if (ciotEncerrado != null)
                    serContratoFrete.EncerramentoContratoViaCIOT(contrato, ciotEncerrado, tipoServicoMultisoftware, unidadeDeTrabalho, DateTime.Now);
                else
                {
                    contrato.EmEncerramentoCIOT = false;
                    repContratoFrete.Atualizar(contrato);
                    Servicos.Log.TratarErro("Encerramento CIOT Rejeitado ", mensagem);
                }
            }
        }

        private void TestarDisponibilidadeIntegracoes(Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Integracao.ControleIntegracao servicoControleIntegracao = new Servicos.Embarcador.Integracao.ControleIntegracao(unidadeTrabalho);

            servicoControleIntegracao.TestarDisponibilidadeIntegracoes();
        }

        #endregion
    }
}