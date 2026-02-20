using System;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    public class GeracaoConciliacaoTransportador : LongRunningProcessBase<GeracaoConciliacaoTransportador>
    {
        public Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador _configuracao;
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador repositorioConfiguracaoConciliacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador(unitOfWork);
            _configuracao = await repositorioConfiguracaoConciliacao.BuscarPrimeiroRegistroAsync();

            if (_configuracao != null && _configuracao.HabilitarGeracaoAutomatica && !_configuracao.FimProcesso.HasValue && !_configuracao.InicioProcesso.HasValue)
                BuscareGerarConciliacoes(unitOfWork);
        }


        private void BuscareGerarConciliacoes(Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Financeiro.ConciliacaoTransportador servicoConciliacao = new Servicos.Embarcador.Financeiro.ConciliacaoTransportador(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador repositorioConfiguracaoConciliacao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoConciliacaoTransportador(unitOfWork);

            var repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            var cargaCtes = repCargaCte.BuscarPorPeriodoEmissaoETransportador(_configuracao.codEmpresa, _configuracao.DataInicialGeracao, _configuracao.DataFinalGeracao);

            _configuracao.InicioProcesso = DateTime.Now;

            repositorioConfiguracaoConciliacao.Atualizar(_configuracao);

            foreach (var cargaCte in cargaCtes)
            {
                try
                {
                    servicoConciliacao.AdicionarCTeEmConciliacaoTransportador(cargaCte.CTe);
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }
            }

            _configuracao.FimProcesso = DateTime.Now;
            repositorioConfiguracaoConciliacao.Atualizar(_configuracao);

            Servicos.Log.TratarErro($" Geração conciliacao transportador Finalizada | Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ");

        }


        public override bool CanRun()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_stringConexao, Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
                return new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Marfrig);
        }


    }
}