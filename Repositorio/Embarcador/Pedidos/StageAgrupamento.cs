using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class StageAgrupamento : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>
    {
        public StageAgrupamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public StageAgrupamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> BuscarPorCargaDt(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaDT.Codigo == codigoCargaDt select obj;
            return query.Fetch(x => x.CargaDT).ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>> BuscarPorCargaDtAsync(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaDT.Codigo == codigoCargaDt select obj;
            return await query.Fetch(x => x.CargaDT).ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento BuscarPorCodigoFetch(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.Codigo == codigo select obj;
            return query.Fetch(x => x.CargaDT)
                  .Fetch(x => x.Veiculo)
                  .Fetch(x => x.SegundoReboque)
                  .Fetch(x => x.Reboque)
                  .Fetch(x => x.Motorista)
                  .Fetch(x => x.Expedidor)
                  .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> BuscarPorCodigosFetch(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return query.Fetch(x => x.CargaDT)
                  .Fetch(x => x.CargaGerada)
                  .Fetch(x => x.Veiculo)
                  .Fetch(x => x.SegundoReboque)
                  .Fetch(x => x.Reboque)
                  .Fetch(x => x.Motorista)
                  .Fetch(x => x.Expedidor).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> BuscarPorCargaGerada(int codigoCargaFilho)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaGerada.Codigo == codigoCargaFilho select obj;
            return query.Fetch(x => x.CargaGerada).Fetch(x => x.CargaDT).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento BuscarPrimeiroPorCargaGerada(int codigoCargaFilho)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaGerada.Codigo == codigoCargaFilho select obj;
            return query.Fetch(x => x.CargaGerada).Fetch(x => x.CargaDT).FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> BuscarPrimeiroPorCargaGeradaAsync(int codigoCargaFilho, CancellationToken cancellationToken)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaGerada.Codigo == codigoCargaFilho select obj;
            return await query.Fetch(x => x.CargaGerada).Fetch(x => x.CargaDT).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarPrimeiraCargaDTPorCargaGerada(int codigoCargaFilho)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> consultaAgrupamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(agrupamento => agrupamento.CargaGerada.Codigo == codigoCargaFilho);

            return consultaAgrupamento.Select(agrupamento => agrupamento.CargaDT).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.Stage BuscarStagePorCargaMaeENumeroStage(int cargaDt, string numeroStage)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.Stage> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.Stage>();
            query = from obj in query where obj.CargaDT.Codigo == cargaDt && obj.NumeroStage == numeroStage select obj;
            return query.FirstOrDefault();
        }

        public List<int> BuscarPorCargaDtCodigos(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaDT.Codigo == codigoCargaDt select obj;
            return query.Select(c => c.Codigo).ToList();
        }


        public List<int> BuscarCodigosStageAgrupadaAProcessar(int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(obj => obj.Processado == true && obj.CargaDT.StagesGeradas == false && ((bool?)obj.ProcessadoPorPrechekin ?? false) == false).OrderBy(x => x.CargaDT.Codigo).Take(numeroRegistrosPorVez);

            return query.Select(x => x.Codigo)
                .Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento BuscarPrimeiroStageAgrupadaAProcessarPorPreCheking()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(obj => obj.Processado == true && obj.CargaDT.StagesGeradas == false && ((bool?)obj.ProcessadoPorPrechekin ?? false) == true).OrderBy(x => x.CargaDT.Codigo);

            return query.FirstOrDefault();
        }

        public List<int> BuscarCodigosStageAgrupadaAProcessarPorPreChekingPorCargaDT(int codigoDT)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(obj => obj.Processado == true && obj.CargaDT.StagesGeradas == false && ((bool?)obj.ProcessadoPorPrechekin ?? false) == true && obj.CargaDT.Codigo == codigoDT && obj.CargaGerada == null);

            return query.Select(x => x.Codigo).ToList();
        }

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento BuscarPorCargaENumeroVeiculo(int codigoCarga, int numeroVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(obj => obj.NumeroVeiculo == numeroVeiculo && obj.CargaDT.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento BuscarPrimerAgrupamento(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(obj => obj.Veiculo != null && obj.Reboque != null && obj.Motorista != null && obj.CargaDT.Codigo == codigoCarga);
            return query.FirstOrDefault();
        }


        //Autoriza a geracao das cargas do agrupamento das stages (thread Unilever processa)
        public void AutorizarGeracaoCarga(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE StageAgrupamento SET Processado = :processando WHERE Veiculo is not null and Motorista is not null and  CargaDT.Codigo = :codigoCarga")
                .SetInt32("codigoCarga", codigoCarga)
                .SetBoolean("processando", true)
                .ExecuteUpdate();
        }

        //Autoriza a geracao das cargas do agrupamento das stages PARA CARGAS DE PRE-CHEKIN (thread Unilever processa)
        public void AutorizarGeracaoCargaPrecheckin(int codigoCarga)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE StageAgrupamento SET Processado = :processando, ProcessadoPorPrechekin = :processandoPrechekin  WHERE CargaDT.Codigo = :codigoCarga and CargaGerada is null")
                .SetInt32("codigoCarga", codigoCarga)
                .SetBoolean("processando", true)
                .SetBoolean("processandoPrechekin", true)
                .ExecuteUpdate();
        }

        public bool ExisteAgrupamentoDaCargaComVeiculo(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(obj => obj.CargaDT.Codigo == codigoCargaDt);

            return query.Any(o => o.Veiculo != null);
        }

        public bool ExisteAgrupamentoDaCarga(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(obj => obj.CargaDT.Codigo == codigoCargaDt);

            return query.Any();
        }

        public bool ExisteAgrupamentoDaCargaDTJaGerado(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(obj => obj.CargaDT.Codigo == codigoCargaDt && obj.CargaGerada != null);

            return query.Any();
        }

        public List<int> BuscarCodigoMotoristaPorAgrupamento(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(obj => obj.CargaDT.Codigo == codigoCargaDt);

            return query.Select(x => x.Motorista.Codigo).ToList();
        }
        public async Task<List<int>> BuscarCodigoMotoristaPorAgrupamentoAsync(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>()
                .Where(obj => obj.CargaDT.Codigo == codigoCargaDt);

            return await query.Select(x => x.Motorista.Codigo).ToListAsync();
        }


        public bool ExisteAgrupamentoDaCargaDTAGerar(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaDT.Codigo == codigoCargaDt && obj.Processado == true select obj;
            return query.Any();
        }

        public bool ExisteAgrupamentoComFreteCalculado(int codigoCargaDt)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = from obj in query where obj.CargaDT.Codigo == codigoCargaDt && (obj.CargaGerada == null || (obj.CargaGerada.ValorFrete == 0)) select obj;
            return query.Any();
        }
        public bool EstaCargaFoiGeradoPorUmAgrupamento(int codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.StageAgrupamento>();
            query = query.Where(a => a.CargaGerada.Codigo == codigo);
            return query.Any();
        }

        public void RemoverVinculoStagePorAgrupamento(int codigoAgrupamentoStage)
        {
            UnitOfWork.Sessao.CreateSQLQuery("UPDATE T_STAGE SET STG_CODIGO = NULL WHERE STG_CODIGO = :codigoAgrupamentoStage;").SetInt32("codigoAgrupamentoStage", codigoAgrupamentoStage).ExecuteUpdate();
        }

        #endregion
    }
}
