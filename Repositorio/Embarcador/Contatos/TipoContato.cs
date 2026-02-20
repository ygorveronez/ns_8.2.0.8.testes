using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Contatos
{
    public class TipoContato : RepositorioBase<Dominio.Entidades.Embarcador.Contatos.TipoContato>
    {
        public TipoContato(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Contatos.TipoContato BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.TipoContato>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Contatos.TipoContato> BuscarTodos(bool? ativo = null)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.TipoContato>();

            if (ativo.HasValue)
                query = query.Where(o => o.Ativo == ativo.Value);

            return query.ToList();
        }

        public IQueryable<Dominio.Entidades.Embarcador.Contatos.TipoContato> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.TipoContato>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Equals(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                query = query.Where(o => o.Ativo == (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            return query;
        }

        public List<Dominio.Entidades.Embarcador.Contatos.TipoContato> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, string propOrdenar, string dirOrdenar, int inicio, int limite)
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

        public List<string> BuscarDescricaoPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.TipoContato>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Embarcador.Contatos.TipoContato BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Contatos.TipoContato>();

            query = query.Where(o => o.Descricao.Equals(descricao));

            return query.FirstOrDefault();
        }
    }
}
