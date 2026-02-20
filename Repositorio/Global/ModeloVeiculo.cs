using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class ModeloVeiculo : RepositorioBase<Dominio.Entidades.ModeloVeiculo>, Dominio.Interfaces.Repositorios.ModeloVeiculo
    {
        public ModeloVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.ModeloVeiculo BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.ModeloVeiculo> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.ModeloVeiculo> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.ModeloVeiculo BuscarPorDescricao(string descricao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.ModeloVeiculo BuscarPorCodigoIntegracao(string codigoIntegracao, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();
            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegracao) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.ModeloVeiculo> Consultar(int codigoEmpresa, string descricao, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ModeloVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.ModeloVeiculo>();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.ModeloVeiculo>();
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(descricao))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.ModeloVeiculo> Consulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoMarca, int codigoEmpresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();

            var result = from obj in query select obj;

            if (codigoMarca > 0)
                result = result.Where(obj => obj.MarcaVeiculo.Codigo == codigoMarca);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (maximoRegistros > 0)
                result = result.Skip(inicioRegistros).Take(maximoRegistros);

            return result.Fetch(o => o.MarcaVeiculo).ToList();
        }

        public int ContaConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int codigoMarca, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ModeloVeiculo>();

            var result = from obj in query select obj;

            if (codigoMarca > 0)
                result = result.Where(obj => obj.MarcaVeiculo.Codigo == codigoMarca);

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

        public bool ContemVeiculoComModelo(Dominio.Entidades.ModeloVeiculo modeloVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            var result = from obj in query where obj.Modelo.Codigo == modeloVeiculo.Codigo select obj;

            return result.Count() > 0;
        }

        #endregion
    }
}
