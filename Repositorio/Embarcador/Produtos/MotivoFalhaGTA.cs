using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class MotivoFalhaGTA : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA>
    {

        public MotivoFalhaGTA(UnitOfWork unitOfWork) : base(unitOfWork) { }

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA>();

            var result = from obj in query select obj;

            // Filtros
            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Ativo == ativo);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.MotivoFalhaGTA>();
            query = query.Where(o => o.Ativo);
            return query.ToList();
        }

    }
}
