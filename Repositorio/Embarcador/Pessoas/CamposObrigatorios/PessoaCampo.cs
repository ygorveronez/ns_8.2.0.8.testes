using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pessoas.CamposObrigatorios
{
    public class PessoaCampo : RepositorioBase<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo>
    {
        public PessoaCampo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> Consultar(string descricao, string propOrdenar, string dirOrdenar, int inicio, int limite)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> query = ObterQueryConsulta(descricao, propOrdenar, dirOrdenar, inicio, limite);

            return query.ToList();
        }

        public int ContarConsulta(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> query = ObterQueryConsulta(descricao);

            return query.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> ObterQueryConsulta(string descricao, string propOrdenar = "", string dirOrdenar = "", int inicio = 0, int limite = 0)
        {
            IQueryable<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.CamposObrigatorios.PessoaCampo>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(propOrdenar) && !string.IsNullOrWhiteSpace(dirOrdenar))
                query = query.OrderBy(propOrdenar + " " + dirOrdenar);

            if (inicio > 0 || limite > 0)
                query = query.Skip(inicio).Take(limite);

            return query;
        }

        #endregion
    }
}
