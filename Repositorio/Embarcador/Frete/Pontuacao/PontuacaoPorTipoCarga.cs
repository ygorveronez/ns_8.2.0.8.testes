using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga>
    {
        public PontuacaoPorTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga> BuscarTodas(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga>();

            return query.OrderBy(obj => obj.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga>();

            return query.Count();
        }


        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga VerificarExistePorTipoCarga(int TipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoCarga>();

            query = query.Where(obj => obj.TipoCarga.Codigo == TipoCarga);

            return query.FirstOrDefault();
        }
    }
}
