using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Contatos
{
    public class SituacaoContato : RepositorioBase<Dominio.Entidades.Embarcador.Contatos.SituacaoContato>
    {
        public SituacaoContato(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Contatos.SituacaoContato BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.SituacaoContato>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Contatos.SituacaoContato> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.SituacaoContato>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Equals(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                query = query.Where(o => o.Ativo == (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Contatos.SituacaoContato> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            var result = _Consultar(descricao, ativo);

            if (inicio > 0)
                result = result.Skip(inicio);

            if (limite > 0)
                result = result.Take(limite);

            result = result.OrderBy(propOrdenar + " " + dirOrdenar);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo)
        {
            var result = _Consultar(descricao, ativo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Contatos.SituacaoContato> BuscarTodos(bool? ativo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.SituacaoContato>();

            if (ativo.HasValue)
                query = query.Where(o => o.Ativo == ativo.Value);

            return query.ToList();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.SituacaoContato>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }
    }
}
