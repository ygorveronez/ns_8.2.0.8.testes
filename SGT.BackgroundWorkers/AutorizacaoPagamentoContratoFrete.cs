using AdminMultisoftware.Repositorio;
using SGT.BackgroundWorkers.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.BackgroundWorkers
{
    [RunningConfig(DuracaoPadrao = 60000)]

    public class AutorizacaoPagamentoContratoFrete : LongRunningProcessBase<AutorizacaoPagamentoContratoFrete>
    {
        protected override async Task ExecuteInternalAsync(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin, CancellationToken cancellationToken)
        {
            ProcessarPagamentoCIOT(_tipoServicoMultisoftware, unitOfWork);
            ProcessarPagamentoIntegracao(_tipoServicoMultisoftware, unitOfWork);
        }

        public override bool CanRun()
        {
            return _tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS;
        }

        private void ProcessarPagamentoCIOT(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao repPagamentoCIOTIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoCIOTIntegracao(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete contratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            try
            {
                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoCIOTIntegracao> integracoesPendentes = repPagamentoCIOTIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

                //Agrupar Registros por Autorização de Pagamento Contrato Frete
                var queryGroup = integracoesPendentes.GroupBy(x => new { x.AutorizacaoPagamentoContratoFrete }).Select(y => new { AutorizacaoPagamentoContratoFrete = y.Key.AutorizacaoPagamentoContratoFrete, PagamentoCIOTIntegracao = y });

                foreach (var integracaoPendente in queryGroup)
                    contratoFrete.AutorizarPagamentoContratoFretePagamentoCIOT(integracaoPendente.AutorizacaoPagamentoContratoFrete, integracaoPendente.PagamentoCIOTIntegracao.ToList(), tipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void ProcessarPagamentoIntegracao(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao repPagamentoContratoIntegracao = new Repositorio.Embarcador.Terceiros.PagamentoContratoIntegracao(unitOfWork);
            Servicos.Embarcador.Terceiros.ContratoFrete contratoFrete = new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork);

            int numeroTentativas = 2;
            double minutosACadaTentativa = 5;
            int numeroRegistrosPorVez = 15;

            try
            {
                List<Dominio.Entidades.Embarcador.Terceiros.PagamentoContratoIntegracao> integracoesPendentes = repPagamentoContratoIntegracao.BuscarIntegracoesPendentes(numeroTentativas, minutosACadaTentativa, "Codigo", "asc", numeroRegistrosPorVez);

                //Agrupar Registros por Autorização de Pagamento Contrato Frete
                var queryGroup = integracoesPendentes.GroupBy(x => new { x.AutorizacaoPagamentoContratoFrete }).Select(y => new { AutorizacaoPagamentoContratoFrete = y.Key.AutorizacaoPagamentoContratoFrete, PagamentoCIOTIntegracao = y });

                foreach (var integracaoPendente in queryGroup)
                    contratoFrete.AutorizarPagamentoContratoFretePagamentoIntegracao(integracaoPendente.AutorizacaoPagamentoContratoFrete, integracaoPendente.PagamentoCIOTIntegracao.ToList(), tipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }
    }
}