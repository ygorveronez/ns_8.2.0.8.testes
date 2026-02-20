using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Avarias
{
    public class Lote : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.Lote>
    {
        public Lote(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Lote(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Avarias.Lote BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.Lote>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> _ConsultarProdutosAvaria(int lote, int solicitacao, string descricao, string codigoEmbarcador, bool? removidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados>();

            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == solicitacao && obj.SolicitacaoAvaria.Lote.Codigo == lote
                         select obj;

            if (removidos.HasValue)
                result = result.Where(o => o.RemovidoLote == removidos.Value);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.ProdutoEmbarcador.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                result = result.Where(o => o.ProdutoEmbarcador.CodigoProdutoEmbarcador.Contains(codigoEmbarcador));

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.ProdutosAvariados> ConsultarProdutosAvaria(int lote, int solicitacao, string descricao, string codigoEmbarcador, bool? removidos, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, removidos);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result
                .Fetch(obj => obj.ProdutoEmbarcador)
                .ThenFetch(obj => obj.GrupoProduto)
                .ToList();
        }

        public int ContarConsultaProdutosAvaria(int lote, int solicitacao, string descricao, string codigoEmbarcador, bool? removidos)
        {
            var result = _ConsultarProdutosAvaria(lote, solicitacao, descricao, codigoEmbarcador, removidos);

            return result.Count();
        }

        private NHibernate.ISQLQuery _QueryConsultarProdutosLote(int lote, string descricao, string codigoEmbarcador, bool contagem, int inicioRegistros = 0, int maximoRegistros = 0)
        {
            string sqlQuery = "SELECT ";

            string select = @"
	            max(PAV_CODIGO) Codigo,
                _produto.PRO_COD_PRODUTO CodigoProduto,
	            _produtoembarcador.GRP_DESCRICAO ProdutoEmbarcador,
	            _grupoproduto.GRP_DESCRICAO GrupoProduto,
	            sum(PAV_VALOR_AVARIA) ValorAvaria,
	            sum(PAV_UNIDADES_AVARIADAS) UnidadesAvariadas,
	            sum(PAV_CAIXAS_AVARIADAS) CaixasAvariadas";

            string count = "distinct(count(0) over ())";

            if (contagem)
                sqlQuery += count;
            else
                sqlQuery += select;

            List<string> sqlWhere = new List<string>();
            sqlWhere.Add("1 = 1");

            if (lote > 0)
                sqlWhere.Add(@"SAV_CODIGO IN (
	                                SELECT SAV_CODIGO FROM T_SOLICITACAO_AVARIA WHERE LAV_CODIGO = " + (lote.ToString()) + @"
                                )");

            if (!string.IsNullOrWhiteSpace(descricao))
                sqlWhere.Add("_produtoembarcador.GRP_DESCRICAO LIKE :descricao");

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                sqlWhere.Add("_produtoembarcador.PRO_CODIGO_PRODUTO_EMBARCADOR LIKE :codigoEmbarcador");

            sqlQuery += @"
            FROM 
	            T_PRODUTOS_AVARIADOS _produtosavariados
            LEFT JOIN
	            T_PRODUTO_EMBARCADOR _produtoembarcador
            ON
	            _produtoembarcador.PRO_CODIGO = _produtosavariados.PRO_CODIGO 
            LEFT JOIN
	            T_GRUPO_PRODUTO _grupoproduto
            ON
	            _grupoproduto.GPR_CODIGO = _produtoembarcador.GRP_CODIGO 
            LEFT JOIN T_PRODUTO _produto ON _produto.PRO_CODIGO = _produtosavariados.PRO_CODIGO
            WHERE " + (String.Join(" \r\n\tAND ", sqlWhere)) + @" 
            GROUP BY
	            _produtosavariados.PRO_CODIGO,
                _produto.PRO_COD_PRODUTO,
	            _produtoembarcador.GRP_DESCRICAO,
	            _produtoembarcador.PRO_CODIGO_PRODUTO_EMBARCADOR,
	            _grupoproduto.GRP_DESCRICAO
            ORDER BY 1 ASC ";

            if (maximoRegistros > 0)
                sqlQuery += "OFFSET " + (inicioRegistros.ToString()) + " ROWS FETCH NEXT " + (maximoRegistros.ToString()) + " ROWS ONLY";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            if (!string.IsNullOrWhiteSpace(descricao))
                query.SetString("descricao", "%" + descricao + "%");

            if (!string.IsNullOrWhiteSpace(codigoEmbarcador))
                query.SetString("codigoEmbarcador", "%" + codigoEmbarcador + "%");

            return query;
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Avarias.ProdutosAvariados> ConsultarProdutosLote(int lote, string descricao, string codigoEmbarcador, int inicioRegistros, int maximoRegistros)
        {
            var result = _QueryConsultarProdutosLote(lote, descricao, codigoEmbarcador, false, inicioRegistros, maximoRegistros);

            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Avarias.ProdutosAvariados)));

            return result.List<Dominio.ObjetosDeValor.Embarcador.Avarias.ProdutosAvariados>();
        }

        public int ContarConsultaProdutosLote(int lote, string descricao, string codigoEmbarcador)
        {
            var result = _QueryConsultarProdutosLote(lote, descricao, codigoEmbarcador, true);

            return result.UniqueResult<int>();
        }

        public int BuscarProximoNumero()
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.Lote>();

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? (retorno.Value + 1) : 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.Lote> _Consultar(int solicitante, int transportadora, int filial, int motivo, int lote, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.Lote>();
            var result = from obj in query select obj;

            if (solicitante > 0)
                result = result.Where(o => o.Avarias.Any(a => a.Solicitante.Codigo == solicitante));
            //result = result.Where(o => o.Avarias.Select(a => a.Solicitante.Codigo).Contains(solicitante));

            if (transportadora > 0)
                result = result.Where(o => o.Transportador.Codigo == transportadora);

            if (filial > 0)
                result = result.Where(o => o.Avarias.Select(a => a.Carga.Filial.Codigo).Contains(filial));

            if (motivo > 0)
                result = result.Where(o => o.Avarias.Select(a => a.MotivoAvaria.Codigo).Contains(motivo));

            if (lote > 0)
                result = result.Where(o => o.Numero == lote);

            if (dataInicio != DateTime.MinValue && dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataGeracao >= dataInicio && o.DataGeracao < dataFim.AddDays(1));
            else if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataGeracao >= dataInicio);
            else if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataGeracao < dataFim.AddDays(1));

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Todas)
                result = result.Where(o => o.Situacao == situacao);

            if (etapa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Todas)
                result = result.Where(o => o.Etapa == etapa);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.Lote> Consultar(int solicitante, int transportadora, int filial, int motivo, int lote, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(solicitante, transportadora, filial, motivo, lote, dataInicio, dataFim, situacao, etapa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Criador)
                .Fetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public int ContarConsulta(int solicitante, int transportadora, int filial, int motivo, int lote, DateTime dataInicio, DateTime dataFim, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote etapa)
        {
            var result = _Consultar(solicitante, transportadora, filial, motivo, lote, dataInicio, dataFim, situacao, etapa);

            return result.Count();
        }

        public int ContarConsultaLotesPendentes(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa)
        {
            var sql = QueryConsultaLotesPendentes(filtrosPesquisa, null, true);
            return this.SessionNHiBernate.CreateSQLQuery(sql).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Avarias.DadosPesquisaLotesPendentes> ConsultaLotesPendentes(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var sql = QueryConsultaLotesPendentes(filtrosPesquisa, parametroConsulta, false);
            var result = this.SessionNHiBernate.CreateSQLQuery(sql);
            result.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Avarias.DadosPesquisaLotesPendentes)));
            return result.SetTimeout(600).List<Dominio.ObjetosDeValor.Embarcador.Avarias.DadosPesquisaLotesPendentes>();
        }

        public IList<int> ConsultaLotesPendentes(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa)
        {
            var sql = QueryConsultaLotesPendentes(filtrosPesquisa, null, false, true);
            var result = this.SessionNHiBernate.CreateSQLQuery(sql);
            return result.SetTimeout(600).List<int>();
        }

        private string QueryConsultaLotesPendentes(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta,
            bool somenteContarNumeroRegistros,
            bool somenteCodigos = false)
        {
            StringBuilder sql = new StringBuilder();

            if (somenteContarNumeroRegistros)
                sql.Append(@"select distinct(count(0) over ()) ");
            else if (somenteCodigos)
                sql.Append(@"select distinct(loteAvaria.LAV_CODIGO) ");
            else
                sql.Append(@"
                            SELECT
                                loteAvaria.LAV_CODIGO            AS Codigo
                               ,loteAvaria.LAV_NUMERO            AS Numero
                               ,loteAvaria.LAV_DATA_GERACAO      AS DataAbertura
                               ,loteAvaria.LAV_SITUACAO          AS Situacao
                               ,loteAvaria.LAV_ETAPA             AS Etapa
                               ,transportador.EMP_TIPO           AS Tipo
                               ,transportador.EMP_CNPJ           AS CNPJ
                               ,transportador.EMP_CODIGO_EMPRESA AS CodigoEmpresa
                               ,transportador.EMP_RAZAO          AS RazaoSocial
                               ,usuario.FUN_NOME                 AS Criador

                               ,(Select SUM(solicitacaoAvaria.SAV_VALOR_DESCONTO)
                                   From T_SOLICITACAO_AVARIA solicitacaoAvaria
                                  Where solicitacaoAvaria.LAV_CODIGO = loteAvaria.LAV_CODIGO) AS ValorDescontos

                               ,(Select SUM(produtosAvariados.PAV_VALOR_AVARIA)
                                   From T_PRODUTOS_AVARIADOS produtosAvariados
                                   Join T_SOLICITACAO_AVARIA solicitacaoAvaria On solicitacaoAvaria.SAV_CODIGO = produtosAvariados.SAV_CODIGO
                                  Where produtosAvariados.PAV_REMOVIDO_LOTE = 0
                                    And solicitacaoAvaria.LAV_CODIGO = loteAvaria.LAV_CODIGO) As ValorAvarias
                             
                               ,(Select SUM(DATEDIFF(HOUR, tempoEtapaLote.LAV_ENTRADA, tempoEtapaLote.LAV_SAIDA))
                                   From T_TEMPO_ETAPA_LOTE tempoEtapaLote
                                  Where tempoEtapaLote.LAV_CODIGO = loteAvaria.LAV_CODIGO 
                                    And tempoEtapaLote.LAV_ETAPA = loteAvaria.LAV_ETAPA
                                    And tempoEtapaLote.LAV_SAIDA Is Not Null) As TotalHoras

                               ,Substring((Select Distinct ', ' + filiaL.FIL_DESCRICAO
                                             From T_SOLICITACAO_AVARIA solicitacaoAvaria
                                             Join T_CARGA carga On carga.CAR_CODIGO = solicitacaoAvaria.CAR_CODIGO
                                             Join T_FILIAL filial On filial.FIL_CODIGO = carga.FIL_CODIGO      
                                            Where solicitacaoAvaria.LAV_CODIGO = loteAvaria.LAV_CODIGO
                                            For XML Path('')), 3, 1000) AS Filial

                               ,Substring((Select Distinct ', ' + usuario.FUN_NOME
                                             From T_FUNCIONARIO usuario
                                            Where usuario.FUN_CODIGO In (Select funcionario.FUN_CODIGO
                                                                           From T_REPONSAVEL_AVARIA responsavelAvaria
                                                                           Join T_SOLICITACAO_AVARIA solicitacaoAvaria on responsavelAvaria.SAV_CODIGO=solicitacaoAvaria.SAV_CODIGO
                                                                           Join T_LOTE_AVARIA loteAvariaSub on solicitacaoAvaria.LAV_CODIGO=loteAvariaSub.LAV_CODIGO
                                                                      Left Join T_FUNCIONARIO funcionario on responsavelAvaria.FUN_CODIGO=funcionario.FUN_CODIGO
                                                                          Where loteAvariaSub.LAV_CODIGO = loteAvaria.LAV_CODIGO)
                                            For XML Path('')), 3, 1000) As Responsavel
                ");

            sql.Append(" FROM T_LOTE_AVARIA loteAvaria ");
            sql.Append(ObterJoinsLotesPendentes());
            sql.Append(ObterFiltrosLotesPendentes(filtrosPesquisa));

            if (!somenteContarNumeroRegistros && !somenteCodigos && !string.IsNullOrWhiteSpace(parametroConsulta.PropriedadeOrdenar))
            {
                sql.Append($" ORDER BY {parametroConsulta.PropriedadeOrdenar} {parametroConsulta.DirecaoOrdenar}");

                if ((parametroConsulta.InicioRegistros > 0) || (parametroConsulta.LimiteRegistros > 0))
                    sql.Append($" OFFSET {parametroConsulta.InicioRegistros} ROWS FETCH NEXT {parametroConsulta.LimiteRegistros} ROWS ONLY;");
            }

            return sql.ToString();
        }

        private string ObterJoinsLotesPendentes()
        {
            return @"
                LEFT JOIN T_FUNCIONARIO usuario ON usuario.FUN_CODIGO = loteAvaria.FUN_CODIGO
                LEFT JOIN T_EMPRESA transportador On transportador.EMP_CODIGO = loteAvaria.EMP_CODIGO";
        }

        private string ObterFiltrosLotesPendentes(Dominio.ObjetosDeValor.Embarcador.Avarias.FiltroPesquisaLotesPendentes filtrosPesquisa)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(" WHERE 1 = 1 ");

            if (filtrosPesquisa.NumeroLote > 0)
                sql.Append($" AND loteAvaria.LAV_NUMERO = {filtrosPesquisa.NumeroLote}");

            if (filtrosPesquisa.CodigoTransportador > 0)
                sql.Append($" AND loteAvaria.EMP_CODIGO = {filtrosPesquisa.CodigoTransportador}");

            if (filtrosPesquisa.SituacaoLote != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Todas)
                sql.Append($" AND loteAvaria.LAV_SITUACAO = {(int)filtrosPesquisa.SituacaoLote}");

            if (filtrosPesquisa.EtapaLote != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaLote.Todas)
                sql.Append($" AND loteAvaria.LAV_ETAPA = {(int)filtrosPesquisa.EtapaLote}");

            return sql.ToString();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.Lote> _ConsultarAceite(int numeroLote, int transportadora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.Lote>();
            var result = from obj in query select obj;

            if (numeroLote > 0)
                result = result.Where(o => o.Numero == numeroLote);

            if (transportadora > 0)
                result = result.Where(o => o.Transportador.Codigo == transportadora);

            if (situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote.Todas)
                result = result.Where(o => o.Situacao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.Lote> ConsultarAceite(int numeroLote, int transportadora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarAceite(numeroLote, transportadora, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.Criador)
                .Fetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .ToList();
        }

        public int ContarConsultaAceite(int numeroLote, int transportadora, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoLote situacao)
        {
            var result = _ConsultarAceite(numeroLote, transportadora, situacao);

            return result.Count();
        }


        public List<Dominio.Entidades.Usuario> ResponsaveisLote(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaAutorizacaoAvaria etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
            var queryAutorizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.SolicitacaoAvariaAutorizacao>();

            var resultAutorizacao = (from obj in queryAutorizacao
                                     where obj.SolicitacaoAvaria.Lote.Codigo == lote && obj.EtapaAutorizacaoAvaria == etapa
                                     group obj by obj.Usuario.Codigo into g
                                     select g.Key).ToList();

            var result = from obj in query where resultAutorizacao.Contains(obj.Codigo) select obj;

            return result.ToList();
        }
    }
}
