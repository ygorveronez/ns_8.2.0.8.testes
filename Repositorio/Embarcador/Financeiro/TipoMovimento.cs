using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using LinqKit;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Financeiro
{
    public class TipoMovimento : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>
    {
        public TipoMovimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query where obj.Descricao == descricao select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> ConsultarIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query select obj;
            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query select obj;
            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.TipoMovimento BuscarPorCodigoEEmpresa(int codigo, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> Consultar(int codigoEmpresa, string descricao, int centroCusto, int planoDebito, int planoCredito, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento forma, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento finalidadeTipoMovimento, List<int> finalidades, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();

            var result = from obj in query select obj;

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                result = result.Where(obj => obj.Codigo == codigo);
            else if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if ((int)forma > 0)
                result = result.Where(obj => obj.FormaTipoMovimento == forma);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);
            if (centroCusto > 0)
                result = result.Where(obj => (from p in obj.CentrosResultados where p.CentroResultado.Codigo == centroCusto select new { p.Codigo }).Count() > 0);
            if (planoDebito > 0)
                result = result.Where(obj => obj.PlanoDeContaDebito.Codigo == planoDebito);
            if (planoCredito > 0)
                result = result.Where(obj => obj.PlanoDeContaCredito.Codigo == planoCredito);

            //if ((int)finalidadeTipoMovimento > 0)
            //    result = result.Where(obj => obj.FinalidadeTipoMovimento == finalidadeTipoMovimento || obj.FinalidadeTipoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas);

            if (finalidades.Count > 0)
            {
                String[] listFinalidades = finalidades.Select(x => x.ToString()).ToArray();
                var predicate = PredicateBuilder.False<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
                foreach (var finalidade in listFinalidades)
                    predicate = predicate.Or(obj => (";" + obj.Finalidades + ";").Contains(";" + finalidade + ";"));
                result = result.Where(predicate);
            }

            if(!string.IsNullOrWhiteSpace(codigoIntegracao))
            {
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);
            }

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, int centroCusto, int planoDebito, int planoCredito, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTipoMovimento forma, Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento finalidadeTipoMovimento, List<int> finalidades, string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();

            var result = from obj in query select obj;

            var codigo = 0;
            if (int.TryParse(descricao, out codigo))
                result = result.Where(obj => obj.Codigo == codigo);
            else if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if ((int)forma > 0)
                result = result.Where(obj => obj.FormaTipoMovimento == forma);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);
            if (centroCusto > 0)
                result = result.Where(obj => (from p in obj.CentrosResultados where p.CentroResultado.Codigo == centroCusto select new { p.Codigo }).Count() > 0);
            if (planoDebito > 0)
                result = result.Where(obj => obj.PlanoDeContaDebito.Codigo == planoDebito);
            if (planoCredito > 0)
                result = result.Where(obj => obj.PlanoDeContaCredito.Codigo == planoCredito);

            //if ((int)finalidadeTipoMovimento > 0)
            //    result = result.Where(obj => obj.FinalidadeTipoMovimento == finalidadeTipoMovimento || obj.FinalidadeTipoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.FinalidadeTipoMovimento.Todas);

            if (finalidades.Count > 0)
            {
                String[] listFinalidades = finalidades.Select(x => x.ToString()).ToArray();
                var predicate = PredicateBuilder.False<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
                foreach (var finalidade in listFinalidades)
                    predicate = predicate.Or(obj => (";" + obj.Finalidades + ";").Contains(";" + finalidade + ";"));
                result = result.Where(predicate);
            }

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
            {
                result = result.Where(obj => obj.CodigoIntegracao == codigoIntegracao);
            }


            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TipoMovimento> RelatorioTipoMovimento(string codigoPlanoDebito, string codigoPlanoCredito, int codigoCentroResultado, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            var parametros = new List<ParametroSQL>();

            string query = @"SELECT T.TIM_CODIGO Codigo, T.TIM_DESCRICAO Descricao,
                PD.PLA_PLANO PlanoDebito, PD.PLA_DESCRICAO DescricaoPlanoDebito,
                PC.PLA_PLANO PlanoCredito, PC.PLA_DESCRICAO DescricaoPlanoCredito
                FROM T_TIPO_MOVIMENTO T
                JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = T.PLA_CODIGO_DEBITO
                JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = T.PLA_CODIGO_CREDITO
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigoPlanoDebito))
            {
                query += " AND  PD.PLA_PLANO = :PD_PLA_PLANO";
                parametros.Add(new ParametroSQL("PD_PLA_PLANO", codigoPlanoDebito));
            }

            if (!string.IsNullOrWhiteSpace(codigoPlanoCredito))
            {
                query += " AND  PC.PLA_PLANO = :PC_PLA_PLANO";
                parametros.Add(new ParametroSQL("PC_PLA_PLANO", codigoPlanoCredito));

            }

            if (codigoCentroResultado > 0)
                query += " AND T.TIM_CODIGO IN (SELECT MC.TIM_CODIGO FROM T_TIPO_MOVIMENTO_CENTRO_RESULTADO MC WHERE MC.CRE_CODIGO = " + codigoCentroResultado.ToString() + ") "; 

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


            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.TipoMovimento)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.TipoMovimento>();
        }

        public int ContarRelatorioTipoMovimento(string codigoPlanoDebito, string codigoPlanoCredito, int codigoCentroResultado)
        {
            var parametros = new List<ParametroSQL>();

            string query = @"SELECT COUNT(0) as CONTADOR
                FROM T_TIPO_MOVIMENTO T
                JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = T.PLA_CODIGO_DEBITO
                JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = T.PLA_CODIGO_CREDITO
                WHERE 1 = 1";

            if (!string.IsNullOrWhiteSpace(codigoPlanoDebito))
            {
                query += " AND  PD.PLA_PLANO = :PD_PLA_PLANO";
                parametros.Add(new ParametroSQL("PD_PLA_PLANO", codigoPlanoDebito));
            }

            if (!string.IsNullOrWhiteSpace(codigoPlanoCredito))
            {
                query += " AND  PC.PLA_PLANO = :PC_PLA_PLANO";
                parametros.Add(new ParametroSQL("PC_PLA_PLANO", codigoPlanoCredito));

            }

            if (codigoCentroResultado > 0)
                query += " AND T.TIM_CODIGO IN (SELECT MC.TIM_CODIGO FROM T_TIPO_MOVIMENTO_CENTRO_RESULTADO MC WHERE MC.CRE_CODIGO = " + codigoCentroResultado.ToString() + ") "; // SQL-INJECTION-SAFE

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> BuscarTipoMovimentoEmpresa(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento>();
            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.Ativo select obj;
            return result.ToList();
        }
    }
}
