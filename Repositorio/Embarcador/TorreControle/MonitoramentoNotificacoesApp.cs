using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.TorreControle
{
    public class MonitoramentoNotificacoesApp : RepositorioBase<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>
    {
        public MonitoramentoNotificacoesApp(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp BuscarPorChamadoETipo(int codigoChamado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp tipoNotificacaoApp)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.Chamado.Codigo == codigoChamado && obj.TipoNotificacaoApp == tipoNotificacaoApp);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp BuscarPorCargaETipo(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacaoApp tipoNotificacaoApp)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.Carga.Codigo == codigoCarga && obj.TipoNotificacaoApp == tipoNotificacaoApp);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp> BuscarIntegracoesPendentes()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                                        (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < 3));
            return result.ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaMonitoramentoNotificacoesApp> Consultar(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = QueryMonitorNotificacaoApp(filtrosPesquisa, false, parametroConsulta);

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaMonitoramentoNotificacoesApp)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.TorreControle.ConsultaMonitoramentoNotificacoesApp>();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            string sql = QueryMonitorNotificacaoApp(filtrosPesquisa, true, parametroConsulta);

            NHibernate.ISQLQuery consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        private string QueryMonitorNotificacaoApp(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaConsultaMonitorNotificacaoApp filtrosPesquisa, bool somenteContarNumeroRegistros, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            StringBuilder query = new StringBuilder();

            if (somenteContarNumeroRegistros)
                query.Append(@"SELECT DISTINCT(COUNT(0) OVER ()) ");
            else
                query.Append($@"SELECT
                                    Monitoramento.MNA_CODIGO AS Codigo,
                                    Funcionario.FUN_NOME AS NomeMotorista,
                                    Funcionario.FUN_CPF AS CPFMotorista,
                                    Empresa.EMP_FANTASIA AS DescricaoTransportador,
                                    Localidades.LOC_IBGE AS CodigoIBGE,
                                    Localidades.LOC_DESCRICAO AS DescricaoLocalidade,
                                    Localidades.PAI_CODIGO AS Pais,
                                    Pais.PAI_NOME AS PaisNome,
                                    Pais.PAI_ABREVIACAO AS PaisAbreviacao,
                                    Localidades.UF_SIGLA AS EstadoSigla,
	                                Monitoramento.MNA_TIPO_NOTIFICACAO_APP AS TipoNotificacao,
                                    Carga.CAR_CODIGO_CARGA_EMBARCADOR AS CodigoCargaEmbarcador,
                                    Chamados.CHA_NUMERO AS Chamado,
                                    Monitoramento.INT_SITUACAO_INTEGRACAO AS SituacaoIntegracao,
                                    Monitoramento.INT_DATA_INTEGRACAO AS DataEnvio,
                                    Monitoramento.INT_NUMERO_TENTATIVAS AS NumeroTentativas,
                                    Monitoramento.INT_PROBLEMA_INTEGRACAO AS Retorno");

            query.Append($@" FROM
                                T_MONITORAMENTO_NOTIFICACOES_APP AS Monitoramento 
                            JOIN T_CARGA Carga ON Monitoramento.CAR_CODIGO = Carga.CAR_CODIGO 
                            LEFT JOIN T_FUNCIONARIO AS Funcionario ON Monitoramento.FUN_CODIGO = Funcionario.FUN_CODIGO
                            LEFT JOIN T_EMPRESA AS Empresa ON Carga.EMP_CODIGO = Empresa.EMP_CODIGO
                            LEFT JOIN T_LOCALIDADES AS Localidades ON Empresa.LOC_CODIGO = Localidades.LOC_CODIGO
                            LEFT JOIN T_PAIS AS Pais ON Pais.PAI_CODIGO = Localidades.PAI_CODIGO
                            LEFT JOIN T_UF AS UF ON UF.UF_SIGLA = Localidades.UF_SIGLA
                            LEFT JOIN T_CHAMADOS AS Chamados ON Chamados.CHA_CODIGO = Monitoramento.CHA_CODIGO");

            query.Append($@" WHERE 1 = 1");

            if (filtrosPesquisa.CodigoCarga > 0)
                query.Append($@" AND Monitoramento.CAR_CODIGO = {filtrosPesquisa.CodigoCarga}");

            if (filtrosPesquisa.CodigoMotorista > 0)
                query.Append($@" AND Monitoramento.FUN_CODIGO = {filtrosPesquisa.CodigoMotorista}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                query.Append($@" AND Carga.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.DataInicioEnvio != DateTime.MinValue)
                query.Append($@" AND Monitoramento.INT_DATA_INTEGRACAO >= '{filtrosPesquisa.DataInicioEnvio:yyyy-MM-dd HH:mm:ss}'");

            if (filtrosPesquisa.DataFimEnvio != DateTime.MinValue)
                query.Append($" AND Monitoramento.INT_DATA_INTEGRACAO < '{filtrosPesquisa.DataFimEnvio.Value.AddDays(1).Date:yyyyMMdd HH:mm:ss}'");

            if (filtrosPesquisa.SituacaoIntegracao.HasValue)
                query.Append($@" AND Monitoramento.INT_SITUACAO_INTEGRACAO = {(int)filtrosPesquisa.SituacaoIntegracao.Value}");

            if (filtrosPesquisa.TipoNotificacaoApp.HasValue)
                query.Append($@" AND Monitoramento.MNA_TIPO_NOTIFICACAO_APP = {(int)filtrosPesquisa.TipoNotificacaoApp.Value}");

            if (filtrosPesquisa.CodigoChamado > 0)
                query.Append($@" AND Chamados.CHA_CODIGO = {filtrosPesquisa.CodigoChamado}");

            if (!somenteContarNumeroRegistros && !string.IsNullOrWhiteSpace(parametroConsulta?.PropriedadeOrdenar))
            {
                query.Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    query.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return query.ToString();
        }

        public Task<List<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.TorreControle.MonitoramentoNotificacoesApp>();
            var result = from obj in query where obj.Chamado.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            
            return result.ToListAsync();
        }
    }
}
