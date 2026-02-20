using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Filiais
{
    public class FilialBalanca : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialBalanca>
    {
        public FilialBalanca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> BuscarPorFilial(int codigoFilial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialBalanca>();
            var result = from obj in query where obj.Filial.Codigo == codigoFilial select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Filiais.FilialBalanca> Consultar(Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilialBalanca filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialBalanca>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.ModeloBalanca.Contains(filtrosPesquisa.Descricao));

            result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            return result;
        }

        #endregion
    }
}
