using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Frete.Pontuacao
{
    public class PontuacaoPorTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao>
    {
        public PontuacaoPorTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao>();

            query = query.Where(o => o.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao> BuscarTodas(int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao>();

            return query.OrderBy(obj => obj.Codigo).Skip(inicio).Take(limite).ToList();
        }

        public int ContarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao>();

            return query.Count();
        }


        public Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao VerificarExistePorTipoOperacao(int tipoOperacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.Pontuacao.PontuacaoPorTipoOperacao>();

            query = query.Where(obj => obj.TipoOperacao.Codigo == tipoOperacao);

            return query.FirstOrDefault();
        }
    }
}
