using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class OcorrenciaDeFuncionario : RepositorioBase<Dominio.Entidades.OcorrenciaDeFuncionario>, Dominio.Interfaces.Repositorios.OcorrenciaDeFuncionario
    {
        public OcorrenciaDeFuncionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.OcorrenciaDeFuncionario BuscarPorCodigo(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeFuncionario>();
            var result = from obj in query where obj.Codigo == codigo && obj.Funcionario.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Entidades.OcorrenciaDeFuncionario> Consultar(int codigoEmpresa, string nomeFuncionario, string descricaoTipoOcorrencia, string status, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.OcorrenciaDeFuncionario>();
            criteria.CreateAlias("Funcionario", "funcionario");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("funcionario.Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(nomeFuncionario))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("funcionario.Nome", nomeFuncionario, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
            {
                criteria.CreateAlias("TipoDeOcorrencia", "tipoDeOcorrencia");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("tipoDeOcorrencia.Descricao", descricaoTipoOcorrencia, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Status", status));
            criteria.SetMaxResults(maximoRegistros);
            criteria.SetFirstResult(inicioRegistros);
            return criteria.List<Dominio.Entidades.OcorrenciaDeFuncionario>();
        }

        public int ContarConsulta(int codigoEmpresa, string nomeFuncionario, string descricaoTipoOcorrencia, string status)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.OcorrenciaDeFuncionario>();
            criteria.CreateAlias("Funcionario", "funcionario");
            criteria.Add(NHibernate.Criterion.Restrictions.Eq("funcionario.Empresa.Codigo", codigoEmpresa));
            if (!string.IsNullOrWhiteSpace(nomeFuncionario))
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("funcionario.Nome", nomeFuncionario, NHibernate.Criterion.MatchMode.Anywhere));
            if (!string.IsNullOrWhiteSpace(descricaoTipoOcorrencia))
            {
                criteria.CreateAlias("TipoDeOcorrencia", "tipoDeOcorrencia");
                criteria.Add(NHibernate.Criterion.Restrictions.InsensitiveLike("tipoDeOcorrencia.Descricao", descricaoTipoOcorrencia, NHibernate.Criterion.MatchMode.Anywhere));
            }
            if (!string.IsNullOrWhiteSpace(status))
                criteria.Add(NHibernate.Criterion.Restrictions.Eq("Status", status));
            criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            return criteria.UniqueResult<int>();
        }

        public List<Dominio.Entidades.OcorrenciaDeFuncionario> Relatorio(int codigoEmpresa, int codigoFuncionario, int codigoVeiculo, int codigoTipoOcorrencia, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.OcorrenciaDeFuncionario>();

            var result = from obj in query where obj.Funcionario.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;

            if (codigoFuncionario > 0)
                result = result.Where(o => o.Funcionario.Codigo == codigoFuncionario);

            if (codigoVeiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == codigoVeiculo);

            if (codigoTipoOcorrencia > 0)
                result = result.Where(o => o.TipoDeOcorrencia.Codigo == codigoTipoOcorrencia);

            if (dataInicial != DateTime.MinValue)
                result = result.Where(o => o.DataDaOcorrencia >= dataInicial.Date);

            if (dataFinal != DateTime.MinValue)
                result = result.Where(o => o.DataDaOcorrencia < dataFinal.AddDays(1).Date);

            return result.ToList();
        }
    }
}
