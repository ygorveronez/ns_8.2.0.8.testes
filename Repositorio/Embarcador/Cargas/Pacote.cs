using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class Pacote : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.Pacote>
    {
        public Pacote(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Pacote(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.Pacote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<bool> ExistePacoteIgualAsync(string logKey, CancellationToken cancellationToken)
        {
            var consultaPacote = this.SessionNHiBernate
                .CreateSQLQuery($@"select count(*) from T_PACOTE with(nolock) where PCT_LOG_KEY = :logKey")
                .SetString("logKey", logKey);

            return (await consultaPacote.UniqueResultAsync<int>(cancellationToken)) > 0;
        }

        public bool ExistePacote()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>();

            return query.FirstOrDefault() != null;
        }
        public async Task<bool> ExistePacoteAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>();

            return await query.FirstOrDefaultAsync() != null;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> BuscarPacotesPendentesIntegracaoPasso2()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Fetch(p => p.Pacote)
                //.ThenFetch(p => p.CTeTerceiroXML)
                .Fetch(c => c.CargaPedido)
                //.ThenFetch(c => c.Carga)
                //.ThenFetch(c => c.TipoOperacao)
                .Where(p =>
                p.Pacote.CTeTerceiroXML != null && p.Pacote.TipoIntegracao != null &&
                (p.Pacote.CTeTerceiroXML.CTeTerceiro == null || p.Pacote.CTeTerceiroXML.Pacote.Codigo == null) &&
                (p.Pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                 p.Pacote.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao));
            return query.Take(1000).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Pacote> BuscarPacotesPendentesIntegracaoPasso1()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>()
                .Fetch(p => p.CTeTerceiroXML)
                .Where(p => p.CTeTerceiroXML == null &&
                            p.TipoIntegracao != null &&
                            (p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                             p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao));
            return query.Take(1000).ToList();
        }

        public List<int> BuscarPacotesPendentesIntegracaoPasso1Download()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>()
                .Fetch(p => p.CTeTerceiroXML)
                .Where(p => p.CTeTerceiroXML == null &&
                            p.TipoIntegracao != null &&
                            p.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao);
            return query.Select(o => o.Codigo).Take(500).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Pacote> BuscarPacotesPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga).Select(o => o.Pacote);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.Pacote BuscarPacotePorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido.Carga.Codigo == codigoCarga).Select(o => o.Pacote);

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.Pacote BuscarPacoteCompativelComCargaPedidoOld(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            var consultaPacote = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.Pacote>()
                .Where(o =>
                    (o.Origem.CPF_CNPJ == pedido.Remetente.CPF_CNPJ || o.Contratante.CPF_CNPJ == pedido.Remetente.CPF_CNPJ)
                    && o.Destino.CPF_CNPJ == pedido.Destinatario.CPF_CNPJ
                );

            return consultaPacote.OrderByDescending(o => o.DataRecebimento).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Carga.Pacote.Pacote BuscarPacoteCompativelComCargaPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            string sql = string.Empty;

            sql += @"SELECT TOP 1
	                    pacote.PCT_CODIGO as CodigoPacote,
	                    pacote.PCT_LOG_KEY as LogKey
                     FROM 
                        T_PACOTE pacote 
                        JOIN T_CTE_TERCEIRO cteTerceiro on cteTerceiro.CPS_IDENTIFICACAO_PACOTE = pacote.PCT_LOG_KEY
                     WHERE 
                        cteTerceiro.CPS_DATAHORAEMISSAO > :DataLimiteCTeTerceiro 
                        AND pacote.CLI_CGCCPF_DESTINO = :CPFCNPJDestino
                        AND (pacote.CLI_CGCCPF_ORIGEM = :CPFCNPJOrigem OR pacote.CLI_CGCCPF_CONTRATANTE = :CPFCNPJOrigem)
                     ORDER BY PCT_DATA_RECEBIMENTO DESC";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetParameter("DataLimiteCTeTerceiro", DateTime.Today.AddMonths(-5));
            query.SetParameter("CPFCNPJDestino", pedido.Destinatario.CPF_CNPJ);
            query.SetParameter("CPFCNPJOrigem", pedido.Remetente.CPF_CNPJ);
            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.Pacote.Pacote)));

            return query.SetTimeout(300).UniqueResult<Dominio.ObjetosDeValor.Embarcador.Carga.Pacote.Pacote>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Pacote> BuscarPacotesAvulsos(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaPacotes filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>()
                .Where(o => o.CargaPedido == null).Select(o => o.Pacote);

            if (!string.IsNullOrWhiteSpace(filtroPesquisa.NumeroPacote))
                query = query.Where(o => o.LogKey.Contains(filtroPesquisa.NumeroPacote));

            return ObterLista(query, parametroConsulta);
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes> ConsultarRelatorioPacotes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaPacotes = new Repositorio.Embarcador.Cargas.ConsultaPacotes().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPacotes.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes)));

            return consultaPacotes.SetTimeout(1200).List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes>> ConsultarRelatorioPacotesAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, CancellationToken cancellationToken)
        {
            var consultaPacotes = new Repositorio.Embarcador.Cargas.ConsultaPacotes().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaPacotes.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes)));

            return await consultaPacotes.SetTimeout(1200).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.RelatorioPacotes>(cancellationToken);
        }

        public int ContarConsultaRelatorioPacotes(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioPacotes filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaPacotes = new Repositorio.Embarcador.Cargas.ConsultaPacotes().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaPacotes.SetTimeout(1200).UniqueResult<int>();
        }

        public void InserirImportacaoPacotes(List<Dominio.ObjetosDeValor.Embarcador.Carga.CargaPedidoPacoteImportacao> pacotes, int primeiroPedido, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            string parameros = $"( :CLI_CGCCPF_ORIGEM_[X], :CLI_CGCCPF_DESTINO_[X], :PCT_DATA_RECEBIMENTO_[X], :PCT_LOG_KEY_[X], :PED_CODIGO_[X],:TPI_CODIGO_[X],:PCT_SITUACAO_INTEGRACAO_[X] )";
            string sqlQuery = @"
                        insert into T_PACOTE (CLI_CGCCPF_ORIGEM, CLI_CGCCPF_DESTINO, PCT_DATA_RECEBIMENTO, PCT_LOG_KEY, PED_CODIGO,TPI_CODIGO, PCT_SITUACAO_INTEGRACAO) values " + parameros.Replace("[X]", "0");

            for (int i = 1; i < pacotes.Count; i++)
                sqlQuery += ", " + parameros.Replace("[X]", i.ToString());

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);
            for (int i = 0; i < pacotes.Count; i++)
            {
                query.SetParameter("CLI_CGCCPF_ORIGEM_" + i.ToString(), pacotes[i].Origem);
                query.SetParameter("CLI_CGCCPF_DESTINO_" + i.ToString(), pacotes[i].Destino);
                query.SetParameter("PCT_DATA_RECEBIMENTO_" + i.ToString(), pacotes[i].DataRecebimento);
                query.SetParameter("PCT_LOG_KEY_" + i.ToString(), pacotes[i].LogKey);
                query.SetParameter("PED_CODIGO_" + i.ToString(), primeiroPedido);
                query.SetParameter("TPI_CODIGO_" + i.ToString(), (int)tipoIntegracao);
                query.SetParameter("PCT_SITUACAO_INTEGRACAO_" + i.ToString(), 0);
            }
            query.ExecuteUpdate();
        }

        public void InserirImportacaoCargaPedidoPacote(Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            string parameros = "";
            string sqlQuery = "";

            if (cargaPedido?.Pedido?.Codigo == null)
                return;

            var queryContarPacotes = $"select count(1) from T_PACOTE where PED_CODIGO = {cargaPedido.Pedido.Codigo}"; // SQL-INJECTION-SAFE
            var consultaContarPacotes = this.SessionNHiBernate.CreateSQLQuery(queryContarPacotes);
            int count = consultaContarPacotes.SetTimeout(600).UniqueResult<int>();

            for (int i = 0; i < count; i += 1000)
            {
                var queryPacote = $"select PCT_CODIGO from T_PACOTE where PED_CODIGO = {cargaPedido.Pedido.Codigo} order by PCT_CODIGO OFFSET {i} ROWS FETCH FIRST 1000 ROWS ONLY "; // SQL-INJECTION-SAFE
                var consulta = this.SessionNHiBernate.CreateSQLQuery(queryPacote);
                IList<int> codigoPacotes = consulta.SetTimeout(600).List<int>();

                parameros = "( :CPE_CODIGO_[X], :PCT_CODIGO_[X] )";
                sqlQuery = @"
                        insert into T_CARGA_PEDIDO_PACOTE (CPE_CODIGO, PCT_CODIGO) values " + parameros.Replace("[X]", "0");


                for (int x = 1; x < codigoPacotes.Count; x++)
                    sqlQuery += ", " + parameros.Replace("[X]", x.ToString());

                var query2 = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

                for (int x = 0; x < codigoPacotes.Count; x++)
                {
                    query2.SetParameter("CPE_CODIGO_" + x.ToString(), cargaPedido.Codigo);
                    query2.SetParameter("PCT_CODIGO_" + x.ToString(), codigoPacotes[x]);
                }

                query2.ExecuteUpdate();
            }
        }
    }
}


