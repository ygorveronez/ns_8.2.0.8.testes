using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class PagamentoEletronicoTitulo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>
    {
        public PagamentoEletronicoTitulo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> BuscarPorPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            var result = from obj in query where obj.PagamentoEletronico.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> BuscarPorTitulosRemessa(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            var result = from obj in query where obj.PagamentoEletronico.Codigo == codigo select obj;
            return result.Select(o => o.Titulo).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo BuscarPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            query = query.Where(o => o.Titulo.Codigo == codigo);

            return query
                .FirstOrDefault();
        }

        public List<int> BuscarTodosPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            query = query.Where(o => o.Titulo.Codigo == codigo);

            return query.Select(c => c.Codigo).ToList();
        }

        public bool ContemRemessaPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo>();
            query = query.Where(o => o.Titulo.Codigo == codigo);

            return query.Any();
        }

    }
}
