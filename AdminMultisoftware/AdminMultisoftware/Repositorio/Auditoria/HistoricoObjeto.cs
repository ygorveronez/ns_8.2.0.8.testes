using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace AdminMultisoftware.Repositorio.Auditoria
{
    public class HistoricoObjeto : RepositorioBase<Dominio.Entidades.Auditoria.HistoricoObjeto>
    {
        #region Construtores

        public HistoricoObjeto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Auditoria.HistoricoObjeto> Consultar(long codigo, string entidade)
        {
            var consultaHistoricoObjeto = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>()
                .Where(obj => obj.Objeto == entidade);

            if (codigo > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(obj => obj.CodigoObjeto == codigo);

            return consultaHistoricoObjeto;
        }

        private IQueryable<Dominio.Entidades.Auditoria.HistoricoObjeto> ConsultarIntegracao(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaHistoricoObjeto = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>();

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.OrigemAuditado.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.OrigemAuditado == filtrosPesquisa.OrigemAuditado.Value);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaHistoricoObjeto;
        }

        private string ObterSqlConsultaRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, Dominio.ObjetosDeValor.Auditoria.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (parametrosConsulta == null)
                sql.Append("select distinct(count(0) over ()) ");
            else
            {
                sql.Append("select Historico.HIO_CODIGO as Codigo, funcionario.FUN_NOME as Usuario, Historico.HIO_OBJETO as Menu, ");
                sql.Append("Historico.HIO_DESCRICAO_ACAO as Acao, Historico.HIO_DESCRICAO Descricao, Historico.HIO_DATA as Data, propriedade.HIP_PROPRIEDADE as Propriedade, propriedade.HIP_DE ValorAntigo, propriedade.HIP_PARA ValorNovo");
            }

            sql.Append("  from T_HISTORICO_OBJETO Historico ");
            sql.Append("  inner join T_FUNCIONARIO funcionario on funcionario.FUN_CODIGO = Historico.FUN_CODIGO ");
            sql.Append("  left join T_HISTORICO_OBJETO_PROPRIEDADE objetoPropriedade on objetoPropriedade.HIO_CODIGO = Historico.HIO_CODIGO ");
            sql.Append("  left join T_HISTORICO_PROPRIEDADE propriedade on propriedade.HIP_CODIGO = objetoPropriedade.HIP_CODIGO ");
            sql.Append("  where 1 = 1 ");

            if (filtrosPesquisa.CodigosUsuario != null && filtrosPesquisa.CodigosUsuario.Count() > 0)
                sql.Append($" and funcionario.FUN_CODIGO in ( {string.Join(",", filtrosPesquisa.CodigosUsuario)} )");

            if (filtrosPesquisa.DataInicial.HasValue)
                sql.Append($" and Historico.HIO_DATA >= '{filtrosPesquisa.DataInicial.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataFinal.HasValue)
                sql.Append($" and Historico.HIO_DATA <= '{filtrosPesquisa.DataFinal.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            if (parametrosConsulta != null)
            {
                sql.Append($" ORDER BY { parametrosConsulta.PropriedadeOrdenar} ");

                if (parametrosConsulta.LimiteRegistros > 0)
                    sql.Append($" desc offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return sql.ToString();
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Auditoria.HistoricoObjeto BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Auditoria.HistoricoObjeto> Consultar(long codigo, string entidade, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaHistoricoObjeto = Consultar(codigo, entidade);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                consultaHistoricoObjeto = consultaHistoricoObjeto.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Take(maximoRegistros);

            return consultaHistoricoObjeto
                .Fetch(o => o.Usuario)
                .Timeout(120)
                .ToList();
        }

        public List<Dominio.Entidades.Auditoria.HistoricoObjeto> ConsultarIntegracao(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Auditoria.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracao = ConsultarIntegracao(filtrosPesquisa);

            return ObterLista(consultaIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(long codigo, string entidade)
        {
            var consultaHistoricoObjeto = Consultar(codigo, entidade);

            return consultaHistoricoObjeto.Count();
        }

        public int ContarConsultaIntegracao(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaIntegracao = ConsultarIntegracao(filtrosPesquisa);

            return consultaIntegracao.Count();
        }

        public void TrocarAuditoria(string nomeEntidade, long codigoEntidadeAtual, long codigoNovaEntidade)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery("update HistoricoObjeto set CodigoObjeto = :codigoNovo where CodigoObjeto = :codigoAtual and Objeto = :nomeEntidade")
                    .SetString("nomeEntidade", nomeEntidade)
                    .SetInt64("codigoAtual", codigoEntidadeAtual)
                    .SetInt64("codigoNovo", codigoNovaEntidade)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery("update HistoricoObjeto set CodigoObjeto = :codigoNovo where CodigoObjeto = :codigoAtual and Objeto = :nomeEntidade")
                        .SetString("nomeEntidade", nomeEntidade)
                        .SetInt64("codigoAtual", codigoEntidadeAtual)
                        .SetInt64("codigoNovo", codigoNovaEntidade)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }

        public int ContarConsultaRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa)
        {
            string sql = ObterSqlConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, parametrosConsulta: null);
            var consultaRelatorioAuditoria = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaRelatorioAuditoria.SetTimeout(600).UniqueResult<int>();

        }

        public List<Dominio.ObjetosDeValor.Auditoria.AuditoriaUsuario> ConsultarRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, Dominio.ObjetosDeValor.Auditoria.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSqlConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, parametrosConsulta);
            var consultarelatorio = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultarelatorio.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Auditoria.AuditoriaUsuario)));

            return consultarelatorio.SetTimeout(600).List<Dominio.ObjetosDeValor.Auditoria.AuditoriaUsuario>().ToList();

        }

        #endregion
    }
}