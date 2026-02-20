using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Produtos
{
    public class TipoEmbalagem : RepositorioBase<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>
    {
        public TipoEmbalagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propriedadeOrdenar, string dirOrdena, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> query = ObterQueryConsulta(descricao, status);

            if (!string.IsNullOrWhiteSpace(propriedadeOrdenar) && !string.IsNullOrWhiteSpace(dirOrdena))
                query = query.OrderBy(propriedadeOrdenar + " " + dirOrdena);

            if (inicio > 0)
                query = query.Skip(inicio);

            if (limite > 0)
                query = query.Take(limite);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao && obj.Ativo select obj;

            return result.FirstOrDefault();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            return ObterQueryConsulta(descricao, status).Count();
        }

        public List<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> BuscarAtivos()
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();

            query = query.Where(o => o.Ativo);

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> ObterQueryConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            IQueryable<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Produtos.TipoEmbalagem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query;
        }

        #endregion
    }
}
