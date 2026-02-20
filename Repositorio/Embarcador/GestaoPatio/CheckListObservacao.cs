using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class CheckListObservacao : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao>
    {

        public CheckListObservacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao BuscarPorCategoria(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList categoria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao>();
            var result = from obj in query where obj.Categoria == categoria && obj.Ativo == true select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao>();

            var result = from obj in query select obj;

            // Filtros
            if(!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Ativo == ativo);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListObservacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status,  string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }
}
