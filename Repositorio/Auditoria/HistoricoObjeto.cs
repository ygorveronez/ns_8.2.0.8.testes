using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;

namespace Repositorio.Auditoria
{
    public class HistoricoObjeto : RepositorioBase<Dominio.Entidades.Auditoria.HistoricoObjeto>
    {
        #region Construtores

        public HistoricoObjeto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public HistoricoObjeto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Auditoria.HistoricoObjeto> Consultar(int codigoEmpresa, long codigo, string entidade)
        {
            var consultaHistoricoObjeto = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>()
                .Where(obj => obj.Objeto == entidade);

            if (codigo > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(obj => obj.CodigoObjeto == codigo);

            if (codigoEmpresa > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return consultaHistoricoObjeto;
        }

        private IQueryable<Dominio.Entidades.Auditoria.HistoricoObjeto> ConsultarIntegracao(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaHistoricoObjeto = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>();

            if (filtrosPesquisa.CodigoIntegradora > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Integradora.Codigo == filtrosPesquisa.CodigoIntegradora);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.OrigemAuditado.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.OrigemAuditado == filtrosPesquisa.OrigemAuditado.Value);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaHistoricoObjeto.Timeout(120);
        }

        private string ObterSqlConsultaIntegracaoCarga(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            StringBuilder sql = new StringBuilder();

            if (parametrosConsulta == null)
                sql.Append("select distinct(count(0) over ()) ");
            else
            {
                sql.Append("select Historico.HIO_CODIGO as Codigo, ");
                sql.Append("       convert(varchar(10), Historico.HIO_DATA, 103) + ' ' + convert(varchar(5), Historico.HIO_DATA, 108) as Data, ");
                sql.Append("       Historico.HIO_DESCRICAO_ACAO as Acao, ");
                sql.Append("       Historico.HIO_IP as IP, ");
                sql.Append("       Carga.CAR_CODIGO_CARGA_EMBARCADOR as Carga, ");
                sql.Append("	   Integradora.INT_DESCRICAO as Integradora ");
            }

            sql.Append("  from T_HISTORICO_OBJETO Historico ");
            sql.Append("  left join T_CARGA Carga on Carga.CAR_CODIGO = Historico.HIO_CODIGO_OBJETO and Historico.HIO_OBJETO = 'Carga' ");
            sql.Append("  left join T_INTEGRADORA Integradora on Integradora.INT_CODIGO = Historico.INT_CODIGO ");
            sql.Append(" where Historico.HIO_ORIGEM_AUDITADO = 2 ");

            if (filtrosPesquisa.CodigoIntegradora > 0)
                sql.Append($" and Historico.INT_CODIGO = {filtrosPesquisa.CodigoIntegradora} ");

            if (filtrosPesquisa.DataInicio.HasValue)
                sql.Append($" and Historico.HIO_DATA >= '{filtrosPesquisa.DataInicio.Value.Date.ToString("yyyyMMdd HH:mm:ss")}' ");

            if (filtrosPesquisa.DataLimite.HasValue)
                sql.Append($" and Historico.HIO_DATA <= '{filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay).ToString("yyyyMMdd HH:mm:ss")}' ");

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga))
                sql.Append($" and Carga.CAR_CODIGO_CARGA_EMBARCADOR = '{filtrosPesquisa.NumeroCarga}' ");

            if (filtrosPesquisa.NumeroCte > 0)
            {
                sql.Append(" and exists ( ");
                sql.Append("     select top 1 CargaCTe.CON_CODIGO ");
                sql.Append("       from T_CARGA_CTE CargaCTe ");
                sql.Append("       join T_CTE Cte ON Cte.CON_CODIGO = CargaCTe.CON_CODIGO ");
                sql.Append("      where CargaCTe.CAR_CODIGO = Carga.CAR_CODIGO ");
                sql.Append($"       and Cte.CON_NUM = {filtrosPesquisa.NumeroCte} ");
                sql.Append(" ) ");
            }

            if (parametrosConsulta != null)
            {
                if (parametrosConsulta.LimiteRegistros > 0)
                    sql.Append($" order by Historico.HIO_DATA desc offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
                else
                    sql.Append($" order by Historico.HIO_DATA desc;");
            }


            return sql.ToString();
        }

        private string ObterSqlConsultaRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
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

            if (filtrosPesquisa.Menus != null)
                sql.Append($" and Historico.HIO_OBJETO in (:P_MENUS)");

            if (!string.IsNullOrEmpty(filtrosPesquisa.AcaoRealizada))
                sql.Append($" and Historico.HIO_DESCRICAO_ACAO like :P_ACAO_REALIZADA");

            if (parametrosConsulta != null)
            {
                sql.Append($" ORDER BY {parametrosConsulta.PropriedadeOrdenar} ");

                if (parametrosConsulta.LimiteRegistros > 0)

                    sql.Append($" desc offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return sql.ToString();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaObjeto> ConsultaMenu(string descricao, bool contarConsulta, string propOrdenacao = null, string dirOrdenacao = null, int? inicioRegistros = null, int? maximoRegistros = null)
        {
            string query = @"SELECT DISTINCT HIO_OBJETO Codigo, HIO_OBJETO Descricao
                             FROM
                                 T_HISTORICO_OBJETO";

            if (!string.IsNullOrEmpty(descricao))
            {
                query = $@"SELECT DISTINCT HIO_OBJETO as Codigo, HIO_OBJETO Descricao
                             FROM
                                 T_HISTORICO_OBJETO
                             WHERE
                                 HIO_OBJETO LIKE '%{descricao}%'";
            }
            if (!contarConsulta && !string.IsNullOrWhiteSpace(propOrdenacao))
            {
                query += $" order by {propOrdenacao} {dirOrdenacao}";

                if ((inicioRegistros > 0) || (maximoRegistros > 0))
                    query += $" offset {inicioRegistros} rows fetch next {maximoRegistros} rows only;";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaObjeto)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaObjeto>();

        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Auditoria.HistoricoObjeto BuscarPorCodigo(long codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoObjeto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoHistorico> BuscarPropriedadesObjetos(long codigoEntidade, string entidade, List<string> propriedades)
        {
            string sql = @" select Objeto.HIO_CODIGO as Codigo,
						    Usuario.FUN_NOME as Usuario,	
						    Objeto.HIO_DATA as DataHora,
							HistoricoPropriedade.HIP_PROPRIEDADE as Propriedade,
							HistoricoPropriedade.HIP_DE as ValorAnterior,
							HistoricoPropriedade.HIP_PARA as ValorAtual
						from T_HISTORICO_OBJETO Objeto
                    left join T_FUNCIONARIO Usuario on Usuario.FUN_CODIGO = Objeto.FUN_CODIGO
					left join T_HISTORICO_OBJETO_PROPRIEDADE Propriedade on Propriedade.HIO_CODIGO = Objeto.HIO_CODIGO
					left join T_HISTORICO_PROPRIEDADE HistoricoPropriedade on HistoricoPropriedade.HIP_CODIGO = Propriedade.HIP_CODIGO
                    where Objeto.HIO_OBJETO = :entidade 
                    and Objeto.HIO_CODIGO_OBJETO = :codigoEntidade 
                    and HistoricoPropriedade.HIP_PROPRIEDADE in (:propriedades)";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                                .SetParameter("entidade", entidade)
                                .SetParameter("codigoEntidade", codigoEntidade)
                                .SetParameterList("propriedades", propriedades);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoHistorico)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Pedidos.PedidoHistorico>();
        }


        public List<Dominio.Entidades.Auditoria.HistoricoObjeto> Consultar(int codigoEmpresa, long codigo, string entidade, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaHistoricoObjeto = Consultar(codigoEmpresa, codigo, entidade);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                consultaHistoricoObjeto = consultaHistoricoObjeto.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                consultaHistoricoObjeto = consultaHistoricoObjeto.Take(maximoRegistros);

            return consultaHistoricoObjeto
                .Fetch(o => o.Usuario)
                .Fetch(o => o.Empresa)
                .Fetch(o => o.Integradora)
                .Timeout(120)
                .ToList();
        }

        public List<Dominio.Entidades.Auditoria.HistoricoObjeto> ConsultarIntegracao(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaIntegracao = ConsultarIntegracao(filtrosPesquisa);

            return ObterLista(consultaIntegracao, parametrosConsulta);
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Auditoria.HistoricoObjetoCarga> ConsultarIntegracaoCarga(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSqlConsultaIntegracaoCarga(filtrosPesquisa, parametrosConsulta);
            var consultaIntegracaoCarga = this.SessionNHiBernate.CreateSQLQuery(sql);

            consultaIntegracaoCarga.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Auditoria.HistoricoObjetoCarga)));

            return consultaIntegracaoCarga.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Auditoria.HistoricoObjetoCarga>();
        }

        public int ContarConsulta(int codigoEmpresa, long codigo, string entidade)
        {
            var consultaHistoricoObjeto = Consultar(codigoEmpresa, codigo, entidade);

            return consultaHistoricoObjeto.Count();
        }

        public int ContarConsultaIntegracao(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaIntegracao = ConsultarIntegracao(filtrosPesquisa);

            return consultaIntegracao.Count();
        }

        public int ContarConsultaIntegracaoCarga(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            string sql = ObterSqlConsultaIntegracaoCarga(filtrosPesquisa, parametrosConsulta: null);
            var consultaIntegracaoCarga = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consultaIntegracaoCarga.SetTimeout(600).UniqueResult<int>();
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

        public int ContarConsultaRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSqlConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, parametrosConsulta: null);
            var consultaRelatorioAuditoria = this.SessionNHiBernate.CreateSQLQuery(sql);
            AdicionarParametrosConsultaRelatorioAuditoriaUsuario(consultaRelatorioAuditoria, filtrosPesquisa);

            return consultaRelatorioAuditoria.SetTimeout(600).UniqueResult<int>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario> ConsultarRelatorioAuditoriaUsuario(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string sql = ObterSqlConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, parametrosConsulta);
            var consultarelatorio = this.SessionNHiBernate.CreateSQLQuery(sql);
            AdicionarParametrosConsultaRelatorioAuditoriaUsuario(consultarelatorio, filtrosPesquisa);

            consultarelatorio.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario)));

            return consultarelatorio.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario>();

        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaObjeto> ConsultarMenus(string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var criteria = ConsultaMenu(descricao, false, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);

            return criteria;
        }

        public int ContarConsultaMenus(string descricao)
        {
            var criteria = ConsultaMenu(descricao, true);

            return criteria.Count();
        }

        private void AdicionarParametrosConsultaRelatorioAuditoriaUsuario(ISQLQuery query, Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa)
        {
            if (filtrosPesquisa.Menus != null)
                query.SetParameterList("P_MENUS", filtrosPesquisa.Menus);

            if (!string.IsNullOrEmpty(filtrosPesquisa.AcaoRealizada))
                query.SetParameter("P_ACAO_REALIZADA", $"%{filtrosPesquisa.AcaoRealizada}%");
        }

        #endregion
    }
}
