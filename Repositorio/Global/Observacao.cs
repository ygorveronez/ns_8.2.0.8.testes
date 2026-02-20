using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class Observacao : RepositorioBase<Dominio.Entidades.Observacao>, Dominio.Interfaces.Repositorios.Observacao
    {
        public Observacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Observacao BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query where obj.Codigo == codigo && obj.Empresa.Codigo == codigoEmpresa select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Observacao> BuscarAutomaticasPorEmpresa(int codigoEmpresa, int codigoEmpresaPai, Dominio.Enumeradores.TipoObservacao tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query where (obj.Empresa.Codigo == codigoEmpresa || obj.Empresa.Codigo == codigoEmpresaPai) && obj.Tipo == tipo && obj.Status.Equals("A") && obj.Automatica select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Observacao> BuscarPorUFeCST(int codigoEmpresa, Dominio.Enumeradores.TipoObservacao tipo, Dominio.Enumeradores.TipoCTE tipoCTe, string cst, string ufInicio, string ufFim, bool automatica, string status = "A")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo && obj.Status == status && obj.TipoCTe == tipoCTe && obj.Automatica == automatica select obj;

            if (!string.IsNullOrWhiteSpace(cst))
                result = result.Where(o => o.CST.Equals(cst));
            else
                result = result.Where(o => (o.CST == "" || o.CST == null));

            if (!string.IsNullOrWhiteSpace(ufInicio))
                result = result.Where(o => o.UF.Equals(ufInicio));
            else
                result = result.Where(o => (o.UF == "" || o.UF == null));

            if (!string.IsNullOrWhiteSpace(ufFim))
                result = result.Where(o => o.UFDestino.Equals(ufFim));
            else
                result = result.Where(o => (o.UFDestino == "" || o.UFDestino == null));

            return result.ToList();
        }

        public List<Dominio.Entidades.Observacao> BuscarPorOperacaoICMS(int codigoEmpresa, Dominio.Enumeradores.TipoObservacao tipo, Dominio.Enumeradores.TipoCTE tipoCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao operacao, string cst, bool automatica, string status = "A")
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Tipo == tipo && obj.Status == status && obj.Operacao == operacao && obj.CST.Equals(cst) && obj.TipoCTe == tipoCTe && obj.Automatica == automatica select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Observacao> Consultar(int codigoEmpresa, int codigoEmpresaPai, string descricao, int inicioRegistros, int maximoRegistros, bool? automatica, Dominio.Enumeradores.TipoObservacao tipo = Dominio.Enumeradores.TipoObservacao.Todos)
        {
            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Observacao>();

            //if (!string.IsNullOrWhiteSpace(descricao))
            //    criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            //if (tipo != Dominio.Enumeradores.TipoObservacao.Todos)
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("Tipo", tipo));
            //if (codigoEmpresaPai > 0)
            //    criteria.Add(new NHibernate.Criterion.OrExpression(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa), NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresaPai)));
            //else
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));

            //criteria.SetFirstResult(inicioRegistros);
            //criteria.SetMaxResults(maximoRegistros);
            //criteria.AddOrder(NHibernate.Criterion.Order.Asc("Codigo"));
            //return criteria.List<Dominio.Entidades.Observacao>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query select obj;

            if (tipo != Dominio.Enumeradores.TipoObservacao.Todos)
                result = result.Where(o => o.Tipo == tipo);

            if (codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo == codigoEmpresaPai || o.Empresa.Codigo == codigoEmpresa));
            else
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (automatica != null)
                result = result.Where(o => o.Automatica == automatica);

            return result.Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int codigoEmpresaPai, string descricao, bool? automatica, Dominio.Enumeradores.TipoObservacao tipo = Dominio.Enumeradores.TipoObservacao.Todos)
        {
            //var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.Observacao>();
            //if (!string.IsNullOrWhiteSpace(descricao))
            //    criteria.Add(NHibernate.Criterion.Expression.InsensitiveLike("Descricao", descricao, NHibernate.Criterion.MatchMode.Anywhere));
            //if (tipo != Dominio.Enumeradores.TipoObservacao.Todos)
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("Tipo", tipo));
            //if (codigoEmpresaPai > 0)
            //    criteria.Add(new NHibernate.Criterion.OrExpression(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa), NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresaPai)));
            //else
            //    criteria.Add(NHibernate.Criterion.Expression.Eq("Empresa.Codigo", codigoEmpresa));
            //criteria.SetProjection(NHibernate.Criterion.Projections.RowCount());
            //return criteria.UniqueResult<int>();

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Observacao>();
            var result = from obj in query select obj;

            if (tipo != Dominio.Enumeradores.TipoObservacao.Todos)
                result = result.Where(o => o.Tipo == tipo);

            if (codigoEmpresaPai > 0)
                result = result.Where(o => (o.Empresa.Codigo == codigoEmpresaPai || o.Empresa.Codigo == codigoEmpresa));
            else
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (automatica != null)
                result = result.Where(o => o.Automatica == automatica);

            return result.Count();
        }
    }
}
