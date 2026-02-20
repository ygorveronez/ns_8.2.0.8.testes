using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class MarcaVeiculo : RepositorioBase<Dominio.Entidades.MarcaVeiculo>, Dominio.Interfaces.Repositorios.MarcaVeiculo
    {
        public MarcaVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.MarcaVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.MarcaVeiculo> BuscarPorCodigo(List<int> codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();
            var result = from obj in query where codigo.Contains(obj.Codigo) select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.ToList();
        }

        public Dominio.Entidades.MarcaVeiculo BuscarPorCodigoIntegracao(string codigoIntegracao, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.MarcaVeiculo BuscarPorDescricao(string descricao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.MarcaVeiculo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.MarcaVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.AddOrder(NHibernate.Criterion.Order.Asc("Descricao"));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.MarcaVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.MarcaVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.MarcaVeiculo> Consulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoModelo, string tipoVeiculo, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();

            var result = from obj in query select obj;

            if (codigoModelo > 0)
                result = result.Where(obj => obj.Modelos.Any(modelo => modelo.Codigo == codigoModelo));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
            {
                if (tipoVeiculo == "0")
                    result = result.Where(obj => obj.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao);
                else if (tipoVeiculo == "1")
                    result = result.Where(obj => obj.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoModelo, string tipoVeiculo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MarcaVeiculo>();

            var result = from obj in query select obj;

            if (codigoModelo > 0)
                result = result.Where(obj => obj.Modelos.Any(modelo => modelo.Codigo == codigoModelo));

            if (!string.IsNullOrWhiteSpace(tipoVeiculo))
            {
                if (tipoVeiculo == "0")
                    result = result.Where(obj => obj.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Tracao);
                else if (tipoVeiculo == "1")
                    result = result.Where(obj => obj.TipoVeiculo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoVeiculo.Reboque);
            }

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        #endregion
    }
}
