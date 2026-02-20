using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NHibernate.Criterion;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class NaturezaDaOperacao : RepositorioBase<Dominio.Entidades.NaturezaDaOperacao>, Dominio.Interfaces.Repositorios.NaturezaDaOperacao
    {
        public NaturezaDaOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.NaturezaDaOperacao> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query orderby obj.Descricao select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.NaturezaDaOperacao> BuscarTodosPorTipoCFOP(Dominio.Enumeradores.TipoCFOP tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CFOP>();
            var result = (from obj in query where obj.Tipo == tipo select obj.NaturezaDaOperacao).Distinct();
            return result.ToList();
        }

        public Dominio.Entidades.NaturezaDaOperacao BuscarTodosPorTipoEstado(bool devolucao, bool dentroEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Devolucao == devolucao && obj.DentroEstado == dentroEstado select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NaturezaDaOperacao BuscarPorId(int idNatureza)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Codigo == idNatureza select obj;
            return result.FirstOrDefault();
        }
        public Dominio.Entidades.NaturezaDaOperacao BuscarPorNumero(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Numero == numero select obj;
            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.NaturezaDaOperacao> BuscarPorIds(int[] ids)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            query = query.Where(o => ids.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.NaturezaDaOperacao> BuscarPorIds(List<int> ids)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            query = query.Where(o => ids.Contains(o.Codigo));

            return query.ToList();
        }

        public IList<Dominio.Entidades.NaturezaDaOperacao> Consultar(string descricao, int inicioRegistros, int maximoRegistros)
        {
            var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.NaturezaDaOperacao>();
            int codigo = 0;
            if (int.TryParse(descricao, out codigo))
                criteria.Add(Expression.Or(Expression.Eq("Codigo", codigo), Expression.InsensitiveLike("Descricao", descricao, MatchMode.Anywhere)));
            else
                criteria.Add(Expression.InsensitiveLike("Descricao", descricao, MatchMode.Anywhere));
            criteria.SetFirstResult(inicioRegistros);
            criteria.SetMaxResults(maximoRegistros);
            return criteria.List<Dominio.Entidades.NaturezaDaOperacao>();
        }

        //public int ContarConsulta(string descricao)
        //{
        //    var criteria = this.SessionNHiBernate.CreateCriteria<Dominio.Entidades.NaturezaDaOperacao>();
        //    int codigo = 0;
        //    if (int.TryParse(descricao, out codigo))
        //        criteria.Add(Expression.Or(Expression.Eq("Codigo", codigo), Expression.InsensitiveLike("Descricao", descricao, MatchMode.Anywhere)));
        //    else
        //        criteria.Add(Expression.InsensitiveLike("Descricao", descricao, MatchMode.Anywhere));
        //    criteria.SetProjection(Projections.RowCount());
        //    return criteria.UniqueResult<int>();
        //}

        public Dominio.Entidades.NaturezaDaOperacao BuscarPrimeiroRegistro()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NaturezaDaOperacao> BuscarNaturezaEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status.Equals("A") select obj;
            return result.ToList();
        }

        public Dominio.Entidades.NaturezaDaOperacao BuscarNaturezaNFe(string descricao, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Descricao.Like(descricao) && obj.Empresa.Codigo.Equals(codigoEmpresa) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.NaturezaDaOperacao BuscarPrimeiroRegistro(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.NaturezaDaOperacao> Consultar(string descricao, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.OrderBy(propriedadeOrdenacao + (direcaoOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            return result.Count();
        }

        public List<Dominio.Entidades.NaturezaDaOperacao> Consultar(string descricao, int empresa, string status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, bool? dentroEstado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                result = result.Where(obj => obj.Codigo == codigo);
            else if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(obj => obj.Status.Equals(status));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            if (dentroEstado != null)
                result = result.Where(obj => obj.DentroEstado == dentroEstado);

            if (tipoEntradaSaida > 0)
                result = result.Where(obj => obj.Tipo == tipoEntradaSaida || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(string descricao, int empresa, string status, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida, bool? dentroEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();

            var result = from obj in query select obj;

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                result = result.Where(obj => obj.Codigo == codigo);
            else if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(obj => obj.Status.Equals(status));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            if (dentroEstado != null)
                result = result.Where(obj => obj.DentroEstado == dentroEstado);

            if (tipoEntradaSaida > 0)
                result = result.Where(obj => obj.Tipo == tipoEntradaSaida || obj.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos);

            return result.Count();
        }

        public Dominio.Entidades.NaturezaDaOperacao BuscarPorIdNFSe(int idNaturezaNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.NaturezaNFSe.Codigo == idNaturezaNFSe select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.NaturezaDaOperacao> RelatorioNaturezaDaOperacao(int codigoEmpresa, int codigoNaturezaDaOperacao, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string query = @"SELECT N.NAT_CODIGO Codigo, N.NAT_DESCRICAO Descricao, CF.CFO_CFOP CFOP, CF.CFO_EXTENSAO Extensao
                FROM T_NATUREZAOPERACAO N
                LEFT OUTER JOIN T_NATUREZAOPERACAO_CFOP C ON C.NAT_CODIGO = N.NAT_CODIGO
                LEFT OUTER JOIN T_CFOP CF ON CF.CFO_CODIGO = C.CFO_CODIGO";

            query += " WHERE 1 = 1 ";

            if (codigoNaturezaDaOperacao > 0)
                query += " AND N.NAT_CODIGO = " + codigoNaturezaDaOperacao.ToString();

            if (codigoEmpresa > 0)
                query += " AND N.EMP_CODIGO = " + codigoEmpresa.ToString();

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.NaturezaDaOperacao)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.NaturezaDaOperacao>();
        }

        public int ContarNaturezaDaOperacao(int codigoEmpresa, int codigoNaturezaDaOperacao)
        {
            string query = @"SELECT COUNT(0) as CONTADOR FROM T_NATUREZAOPERACAO N";

            query += " WHERE 1 = 1 ";

            if (codigoNaturezaDaOperacao > 0)
                query += " AND N.NAT_CODIGO = " + codigoNaturezaDaOperacao.ToString();

            if (codigoEmpresa > 0)
                query += " AND N.EMP_CODIGO = " + codigoEmpresa.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public bool PossuiTipoMovimentoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Codigo == codigo && obj.TipoMovimento == null && obj.GeraTitulo == true select obj;
            return result.Any();
        }

        public bool ObrigarParcelasNFe(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NaturezaDaOperacao>();
            var result = from obj in query where obj.Codigo == codigo && obj.TipoMovimento != null && obj.GeraTitulo == true select obj;
            return result.Any();
        }

        #endregion
    }
}
