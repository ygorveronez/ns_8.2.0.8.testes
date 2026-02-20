using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class MotivoAvaliacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>
    {
        public MotivoAvaliacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MotivoAvaliacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            if(inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query.OrderBy(propriedadeOrdenar + " " + dirOrdena).ToList();
        }
        
        public List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> BuscarMotivosAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>();

            query = query.Where(o => o.Ativo == true);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> BuscarPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>> BuscarPorCodigosAsync(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return await query.ToListAsync().ConfigureAwait(false);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.Count();
        }



    }
}
