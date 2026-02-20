using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas
{
    public class CategoriaPessoa : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>
    {
        #region Construtores

        public CategoriaPessoa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region MétodosPrivados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> Consultar(string descricao)
        {
            var consultaCategoriaPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaCategoriaPessoa = consultaCategoriaPessoa.Where(o => o.Descricao.Contains(descricao));

            return consultaCategoriaPessoa;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa BuscarPorCodigoIntegracao(string codigoIntegracao, List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> lstCategorriaPessoa = null)
        {
            if (lstCategorriaPessoa != null)
                return lstCategorriaPessoa.Where(o => o.CodigoIntegracao == codigoIntegracao).FirstOrDefault();
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>().Where(o => o.CodigoIntegracao == codigoIntegracao).FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa BuscarPorCodigo(int codigo)
        {
            var consultaCategoriaPessoa = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>()
                .Where(o => o.Codigo == codigo);

            return consultaCategoriaPessoa.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCategoriaPessoa = Consultar(descricao);

            return ObterLista(consultaCategoriaPessoa, parametrosConsulta);
        }

        public int ContarConsulta(string descricao)
        {
            var consultaCategoriaPessoa = Consultar(descricao);

            return consultaCategoriaPessoa.Count();
        }

        #endregion
    }
}
