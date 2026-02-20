using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 3000)]

    public class ProcessarDadosGestaoDevolucao : LongRunningProcessBase<ProcessarDadosGestaoDevolucao>
    {

        #region Atributos

        public Repositorio.UnitOfWork _unitOfWork;
        public AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;


        #endregion Atributos

        #region Metodos Publicos

        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            _unitOfWork = unitOfWork;
            _unitOfWorkAdmin = unitOfWorkAdmin;

            AlterarDevolucoesAutomaticamenteAoEsgotarTempo(_auditado, _clienteMultisoftware);
        }

        #endregion

        #region Metodos Privados

        private void AlterarDevolucoesAutomaticamenteAoEsgotarTempo(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao servicoGestaoDevolucao = new Servicos.Embarcador.GestaoDevolucao.GestaoDevolucao(_unitOfWork, auditado, clienteMultisoftware);
            Repositorio.Embarcador.Devolucao.GestaoDevolucao repositorioGestaoDevolucao = new Repositorio.Embarcador.Devolucao.GestaoDevolucao(_unitOfWork);
            Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa repositorioGestaoDevolucaoEtapa = new Repositorio.Embarcador.Devolucao.GestaoDevolucaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPaletes configuracaoPaletes = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPaletes(_unitOfWork).BuscarPrimeiroRegistro();
            try
            {
                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao> DevolucoesAterarTipoAutomaticamento = repositorioGestaoDevolucao.ConsultaDevolucaoComTempoEsgotadoSemTipo(72, configuracaoPaletes.LimiteDiasParaDevolucaoDePallet);

                foreach (Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucao gestaoDevolucao in DevolucoesAterarTipoAutomaticamento)
                {
                    _unitOfWork.Start();

                    gestaoDevolucao.Initialize();

                    servicoGestaoDevolucao.DefinirTipoGestaoDevolucao(gestaoDevolucao, TipoGestaoDevolucao.Permuta);
                    servicoGestaoDevolucao.DefinirEtapaAtualGestaoDevolucao(gestaoDevolucao, EtapaGestaoDevolucao.OrdemeRemessa);//ja esta avançando para a 3 etapa da permuta.
                    servicoGestaoDevolucao.FinalizarEtapasAnteriores(gestaoDevolucao, gestaoDevolucao.EtapaAtual);
                    Servicos.Auditoria.Auditoria.Auditar(_auditado, gestaoDevolucao, gestaoDevolucao.GetChanges(), "Avançou etapa automaticamente ao exceder o tempo limite do transportador", _unitOfWork);

                    _unitOfWork.CommitChanges();
                }
            }
            catch (Exception excecao)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                _unitOfWork.Dispose();
            }

        }


        #endregion
    }
}