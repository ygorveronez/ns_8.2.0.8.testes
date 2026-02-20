using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorQuantidadeCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga>
    {
        public PontuacaoPorQuantidadeCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga> BuscarTodas(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga>();

            return query.OrderBy(obj => obj.QuantidadeInicio).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga>();

            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga VerificarExisteEntrePeriodo(int quantidadeInicio, int quantidadeFim)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorQuantidadeCarga>();

            query = query.Where(obj => (quantidadeInicio >= obj.QuantidadeInicio && quantidadeInicio <= obj.QuantidadeFim) ||
                (quantidadeFim >= obj.QuantidadeInicio && quantidadeFim <= obj.QuantidadeFim) ||
                (obj.QuantidadeInicio >= quantidadeInicio && obj.QuantidadeInicio <= quantidadeFim) ||
                (obj.QuantidadeFim >= quantidadeInicio && obj.QuantidadeFim <= quantidadeFim));

            return query.FirstOrDefault();
        }
    }
}
