using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Email
{
    public class LogEnvioEmail : RepositorioBase<Dominio.Entidades.Embarcador.Email.LogEnvioEmail>
    {
        public LogEnvioEmail(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioEmail> RelatorioLogsEnvioEmail(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            string query = @"SELECT L.LEE_DATA Data, 
                                    L.LEE_EMAIL_REMETENTE EmailRemetente, 
                                    L.LEE_EMAIL_DESTINATARIO EmailDestinatario,
                                    L.LEE_EMAIL_RESPOSTA EmailResposta,
                                    L.LEE_EMAIL_COPIA EmailCopia,
                                    L.LEE_EMAIL_COPIA_OCULTA EmailCopiaOculta,
                                    L.LEE_DESCRICAO_ANEXO DescricaoAnexo,
                                    L.LEE_ASSUNTO Assunto,
                                    L.LEE_MENSAGEM Mensagem
                            FROM T_LOG_ENVIO_EMAIL L 
                            WHERE 1 = 1";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND L.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(L.LEE_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(L.LEE_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                agrup = true;
                query += " order by " + parametrosConsulta.PropriedadeAgrupar + " " + parametrosConsulta.DirecaoAgrupar;
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && parametrosConsulta.PropriedadeAgrupar != parametrosConsulta.PropriedadeOrdenar)
            {
                if (agrup)
                {
                    query += ", " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
                else
                {
                    query += " order by " + parametrosConsulta.PropriedadeOrdenar + " " + parametrosConsulta.DirecaoOrdenar;
                }
            }

            if (parametrosConsulta.LimiteRegistros > 0)
                query += " OFFSET " + parametrosConsulta.InicioRegistros + " ROWS FETCH FIRST " + parametrosConsulta.LimiteRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioEmail)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioEmail>();
        }

        public int ContarRelatorioLogsEnvioEmail(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail filtrosPesquisa)
        {
            string query = @"SELECT COUNT(0) as CONTADOR 
                            FROM T_LOG_ENVIO_EMAIL L 
                            WHERE 1 = 1";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                query += " AND L.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                query += " AND CAST(L.LEE_DATA AS DATE) >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "'";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                query += " AND CAST(L.LEE_DATA AS DATE) <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }
    }
}