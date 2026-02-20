using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class SimulacaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>
    {
        public SimulacaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public SimulacaoFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result.FirstOrDefault();
        }

        public bool PossuiSimulacaoFrete(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>()
                .Where(obj => obj.Carregamento.Codigo == codigoCarregamento);

            return query.Any();
        }


        public IList<Dominio.ObjetosDeValor.Embarcador.Frete.SimulacaoDeFrete> BuscarSimulacaoFrete(List<int> codigosCarregamentos)
        {
            if (codigosCarregamentos.Count() <= 0)
                return new List<Dominio.ObjetosDeValor.Embarcador.Frete.SimulacaoDeFrete>();

            string sql = $@" select CRG_CODIGO CodigoCarregamento, COUNT(*) QuantidadeSimulacoes from T_SIMULACAO_FRETE_CARREGAMENTO 
                      WHERE CRG_CODIGO in ({string.Join(",", codigosCarregamentos.ToList())}) 
                      GROUP BY CRG_CODIGO ";
            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);
            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Frete.SimulacaoDeFrete)));
            return consulta.List<Dominio.ObjetosDeValor.Embarcador.Frete.SimulacaoDeFrete>();
        }





        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete> BuscarPorCarregamentos(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>();
            var resut = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;
            return resut.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>> BuscarPorCarregamentosAsync(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.SimulacaoFrete>();
            var resut = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;
            return resut.ToListAsync(CancellationToken);
        }
    }
}