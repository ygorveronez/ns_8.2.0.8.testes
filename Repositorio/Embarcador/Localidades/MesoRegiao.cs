using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Localidades
{
    public class MesoRegiao : RepositorioBase<Dominio.Entidades.Embarcador.Localidades.MesoRegiao>
    {
        #region Constructores
        public MesoRegiao(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicos
        public Dominio.Entidades.Embarcador.Localidades.MesoRegiao BuscaPorCodigoIntegracao (string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.MesoRegiao>();
            query = query.Where(m => m.CodigoIntegracao == codigoIntegracao);
            return query.FirstOrDefault();
        }

        public int ContarConsulta(string descricao, bool situacao, string codigoIntegracao)
        {
            var consulta = Consultar(descricao, situacao, codigoIntegracao);

            return consulta.Count();
        }

        public List<Dominio.Entidades.Embarcador.Localidades.MesoRegiao> Consultar(string descricao, bool situacao,string codigoIntegracao, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consulta = Consultar(descricao, situacao,codigoIntegracao);

            return ObterLista(consulta, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Localidades.MesoRegiao> Consultar(string descricao,bool situacao,string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Localidades.MesoRegiao>();

            query = query.Where(o => o.Situacao == situacao);

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if(!string.IsNullOrEmpty(codigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao == codigoIntegracao);

            return query;
        }

        #endregion
    }
}
