using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorTempoAtividade : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade>
    {
        public PontuacaoPorTempoAtividade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade> BuscarTodas(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade>();

            return query.OrderBy(obj => obj.AnoInicio).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade>();

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade VerificarExisteEntrePeriodo(int anoIncio, int anoFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTempoAtividade>();
     
            query = query.Where(obj => (anoIncio >= obj.AnoInicio && anoIncio <= obj.AnoFim) ||
                (anoFim >= obj.AnoInicio && anoFim <= obj.AnoFim) ||
                (obj.AnoInicio >= anoIncio && obj.AnoInicio <= anoFim) ||
                (obj.AnoFim >= anoIncio && obj.AnoFim <= anoFim));

            return query.FirstOrDefault();
        }
    }
}
