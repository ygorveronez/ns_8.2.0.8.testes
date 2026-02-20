using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Financeiro
{
    public class PlanoConta : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>
    {
        public PlanoConta(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PlanoConta(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarPorPlanoEmpresa(string plano, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Plano.Equals(plano) select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.PlanoContabilidade.Equals(codigoIntegracao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> BuscarPlanoEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Ativo select obj;
            return result.ToList();
        }

        public bool ContemPlanoConta(int codigo, string plano, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Codigo != codigo && obj.Plano.Equals(plano) select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarPlanoPai(string plano, int codigoEmpresa)
        {
            if (plano.IndexOf(".") >= 0)
            {
                int posicaoPonto = plano.LastIndexOf(".");
                string planoPai = string.Empty;
                if (posicaoPonto == 3)
                    planoPai = plano.Substring(0, plano.Length - posicaoPonto);
                else if (posicaoPonto >= 6)
                    planoPai = plano.Substring(0, posicaoPonto);
                else
                    planoPai = plano.Substring(0, plano.Length - posicaoPonto - 1);
                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
                var result = from obj in query where obj.Plano.Equals(planoPai) select obj;
                if (codigoEmpresa > 0)
                    result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
                return result.FirstOrDefault();
            }
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarPrimeiroAnalitico(string plano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Plano.StartsWith(plano) && obj.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> BuscarProximoPlanoAnalitico(string plano, int tamanhoPlano, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Plano.StartsWith(plano) && obj.Plano.Length == tamanhoPlano select obj;
            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            result = result.OrderBy("Plano descending");
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarUltimoPlano(string plano, int tamanhoPlano, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();

            query = query.Where(o => o.Plano.StartsWith(plano) && o.Plano.Length == tamanhoPlano);
            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return query.OrderByDescending(o => o.Plano).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PlanoConta> Consultar(string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa receitaDespesa, int codigo, int codigoEmpresa, string descricao, string plano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado grupoDeResultado, string comGrupoResultado, int planoContaDebito, int planoContaCredito, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if ((int)receitaDespesa > 0)
                result = result.Where(obj => obj.ReceitaDespesa == receitaDespesa);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.PlanoContabilidade.Equals(codigoIntegracao));

            if (!string.IsNullOrWhiteSpace(plano))
                result = result.Where(obj => obj.Plano.StartsWith(plano));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if ((int)tipo > 0)
                result = result.Where(obj => obj.AnaliticoSintetico == tipo);

            if ((int)grupoDeResultado > 0)
                result = result.Where(obj => obj.GrupoDeResultado == grupoDeResultado);

            if (comGrupoResultado == "S")
                result = result.Where(obj => obj.GrupoDeResultado > 0);

            if (planoContaDebito > 0 || planoContaCredito > 0)
            {
                var queryTipoMovimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
                if (planoContaDebito > 0)
                    queryTipoMovimento = queryTipoMovimento.Where(o => o.PlanoDeContaDebito.Codigo == planoContaDebito);

                if (planoContaCredito > 0)
                    queryTipoMovimento = queryTipoMovimento.Where(o => o.PlanoDeContaCredito.Codigo == planoContaCredito);

                result = result.Where(obj => queryTipoMovimento.Any(p => p.PlanoDeContaDebito.Codigo == obj.Codigo));
            }



            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));
            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();

        }

        public int ContarConsulta(string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.ReceitaDespesa receitaDespesa, int codigo, int codigoEmpresa, string descricao, string plano, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.GrupoDeResultado grupoDeResultado, string comGrupoResultado, int planoContaDebito, int planoContaCredito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();

            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if ((int)receitaDespesa > 0)
                result = result.Where(obj => obj.ReceitaDespesa == receitaDespesa);

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                result = result.Where(obj => obj.PlanoContabilidade.Equals(codigoIntegracao));

            if (!string.IsNullOrWhiteSpace(plano))
                result = result.Where(obj => obj.Plano.StartsWith(plano));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if ((int)tipo > 0)
                result = result.Where(obj => obj.AnaliticoSintetico == tipo);

            if ((int)grupoDeResultado > 0)
                result = result.Where(obj => obj.GrupoDeResultado == grupoDeResultado);

            if (comGrupoResultado == "S")
                result = result.Where(obj => obj.GrupoDeResultado > 0);

            if (planoContaDebito > 0 || planoContaCredito > 0)
            {
                var queryTipoMovimento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
                if (planoContaDebito > 0)
                    queryTipoMovimento = queryTipoMovimento.Where(o => o.PlanoDeContaDebito.Codigo == planoContaDebito);

                if (planoContaCredito > 0)
                    queryTipoMovimento = queryTipoMovimento.Where(o => o.PlanoDeContaCredito.Codigo == planoContaCredito);

                result = result.Where(obj => queryTipoMovimento.Any(p => p.PlanoDeContaDebito.Codigo == obj.Codigo));
            }

            return result.Count();
        }

        public string BuscarPlanoPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.Select(o => o.Plano).FirstOrDefault();
        }

        public List<string> BuscarPlanosPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoConta>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.Select(o => o.Plano).ToList();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoConta> RelatorioPlanoConta(int codigoEmpresa, string planoContaSintetica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = @"SELECT PLA_CODIGO Codigo, " +
                " PLA_PLANO Plano, " +
                " PLA_DESCRICAO Descricao, " +
                " CASE	 " +
                "    WHEN PLA_TIPO = 2 THEN 'SINTÉTICO' " +
                "   ELSE 'ANALITICO' " +
                " END DescricaoAnaliticoSintetico, " +
                " PLA_PLANO_CONTABILIDADE PlanoContabilidade " +
                " FROM T_PLANO_DE_CONTA " +
                " WHERE PLA_ATIVO = 1";

            if ((int)tipo > 0)
            {
                if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico)
                    query += " AND PLA_TIPO = 1";
                else
                    query += " AND PLA_TIPO = 2";
            }
            if (!string.IsNullOrWhiteSpace(planoContaSintetica))
                query += " AND PLA_PLANO LIKE '" + planoContaSintetica + "%'";
            if (codigoEmpresa > 0)
                query += " AND (EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

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

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoConta)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoConta>();
        }

        public int ContarRelatorioPlanoConta(int codigoEmpresa, string planoContaSintetica, Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico tipo)
        {
            string query = @"SELECT COUNT(0) as CONTADOR " +
                " FROM T_PLANO_DE_CONTA " +
                " WHERE PLA_ATIVO = 1";

            if ((int)tipo > 0)
            {
                if (tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico)
                    query += " AND PLA_TIPO = 1";
                else
                    query += " AND PLA_TIPO = 2";
            }
            if (!string.IsNullOrWhiteSpace(planoContaSintetica))
                query += " AND PLA_PLANO LIKE '" + planoContaSintetica + "%'";
            if (codigoEmpresa > 0)
                query += " AND (EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        #endregion
    }
}
