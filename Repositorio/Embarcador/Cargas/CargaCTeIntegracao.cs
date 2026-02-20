using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCTeIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>
    {

        CancellationToken _cancellationToken;
        public CargaCTeIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaCTeIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public bool ExisteIntegracaoParaCargaCte(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Codigo == codigo select obj;

            return result.Any();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao BuscarPorCodigoArquivo(int codigoArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.ArquivosTransacao.Any(o => o.Codigo == codigoArquivo) select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.CTe.Codigo == codigoCTe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarPorCargaCTeETipoIntegracao(int codigoCargaCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Codigo == codigoCargaCTe && obj.TipoIntegracao.Tipo == tipo select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarPorCarga(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo.Value);

            return result.Fetch(o => o.CargaCTe).ThenFetch(o => o.CTe).ThenFetch(o => o.Serie).ToList();
        }

        public Task<List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>> BuscarTipoIntegracaoPorCargaAsync(int codigoCarga, bool integracaoFilialEmissora)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && obj.IntegracaoFilialEmissora == integracaoFilialEmissora select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToListAsync(CancellationToken);
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>> BuscarCTeIntegracaoPendenteAsync(int tentativasLimite, double tempoProximaTentativaMinutos, string propOrdenacao, string dirOrdenacao, int maximoRegistros, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where !obj.CargaCTe.Carga.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == tipoEnvio && (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao && obj.NumeroTentativas < tentativasLimite && obj.DataIntegracao <= DateTime.Now.AddMinutes(-tempoProximaTentativaMinutos))) select obj;

            return result.WithOptions(o=> o.SetTimeout(120)).OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToListAsync(cancellationToken);
        }

        public int ContarPorCargaETipoIntegracao(int codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            query = query.Where(o => o.CargaCTe.Carga.Codigo == codigoCarga && o.TipoIntegracao.Tipo == tipoIntegracao);

            return query.Count();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> BuscarCTeIntegracaoSemLote(string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where !obj.CargaCTe.Carga.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.Lote == null select obj;

            return result.Fetch(o => o.TipoIntegracao)
                         .Fetch(o => o.CargaCTe)
                         .ThenFetch(o => o.Carga)
                         .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoSemLote(string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            query = query.Where(obj => !obj.CargaCTe.Carga.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.Lote == null);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Select(o => o.TipoIntegracao).Distinct().Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.Carga> BuscarCargaDeCTeIntegracaoSemLote(int codigoTipoIntegracao, string propOrdenacao, string dirOrdenacao, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            query = query.Where(obj => obj.TipoIntegracao.Codigo == codigoTipoIntegracao && !obj.CargaCTe.Carga.GerandoIntegracoes && obj.TipoIntegracao.TipoEnvio == TipoEnvioIntegracao.Lote && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao && obj.Lote == null);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Select(o => o.CargaCTe.Carga).Distinct().Skip(0).Take(maximoRegistros).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> BuscarTipoIntegracaoPendente(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEnvioIntegracao tipoEnvio)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.TipoIntegracao.TipoEnvio == tipoEnvio && obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao || (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao) select obj.TipoIntegracao;

            return result.Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao> Consultar(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoCarga > 0)
                result = result.Where(o => o.CargaCTe.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending")).Skip(inicio).Take(limite)
                .Fetch(obj => obj.CargaCTe).ThenFetch(obj => obj.CTe).ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.TipoIntegracao)
                .ToList();
        }

        public int ContarConsulta(int codigoCarga, SituacaoIntegracao? situacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao? tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query select obj;

            if (codigoCarga > 0)
                result = result.Where(o => o.CargaCTe.Carga.Codigo == codigoCarga);

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            if (tipo.HasValue)
                result = result.Where(o => o.TipoIntegracao.Tipo == tipo);

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga select obj;

            return result.Count();
        }

        public int ContarPorCargaESituacaoDiff(int codigoCarga, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao != situacaoDiff select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public int ContarPorCarga(int codigoCarga, SituacaoIntegracao[] situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.CargaCTe.Carga.Codigo == codigoCarga && situacao.Contains(obj.SituacaoIntegracao) select obj;

            return result.Count();
        }

        public int ContarPorLote(int codigoLote, SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote && obj.SituacaoIntegracao == situacao select obj;

            return result.Count();
        }

        public void SetarLotePorCodigos(List<int> codigos, int codigoLote)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE CargaCTeIntegracao SET Lote = :codigoLote WHERE Codigo IN (:codigos)")
                .SetParameterList("codigos", codigos)
                .SetInt32("codigoLote", codigoLote)
                .ExecuteUpdate();
        }

        public void SetarRetornoPorLote(int codigoLote, int numeroTentativas, string problemaIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao, DateTime dataIntegracao)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE CargaCTeIntegracao SET DataIntegracao = :dataIntegracao, SituacaoIntegracao = :situacao, ProblemaIntegracao = :problemaIntegracao, NumeroTentativas = :numeroTentativas WHERE Codigo IN (SELECT c.Codigo FROM CargaCTeIntegracao c WHERE c.Lote.Codigo = :codigoLote)")
                .SetInt32("codigoLote", codigoLote)
                .SetInt32("numeroTentativas", numeroTentativas)
                .SetString("problemaIntegracao", problemaIntegracao)
                .SetEnum("situacao", situacao)
                .SetDateTime("dataIntegracao", dataIntegracao)
                .ExecuteUpdate();
        }

        public void SetarArquivoIntegracaoPorLote(int codigoLote, int codigoArquivoIntegracao)
        {
            UnitOfWork.Sessao.CreateSQLQuery($"INSERT INTO T_CARGA_CTE_INTEGRACAO_ARQUIVO_ARQUIVO (CCI_CODIGO, CCA_CODIGO) SELECT CCI_CODIGO, {codigoArquivoIntegracao} FROM T_CARGA_CTE_INTEGRACAO WHERE CCL_CODIGO = {codigoLote}").ExecuteUpdate(); // SQL-INJECTION-SAFE
        }

        #region Relatório Integrações Rejeitadas

        public IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao> ConsultarRelatorioIntegracao(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataIntegracaoInicial, DateTime dataIntegracaoFinal, int codigoGrupoPessoas, int codigoTipoIntegracao, int codigoCarga, int codigoCTe, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioIntegracao(false, propriedades, dataEmissaoInicial, dataEmissaoFinal, dataIntegracaoInicial, dataIntegracaoFinal, codigoGrupoPessoas, codigoTipoIntegracao, codigoCarga, codigoCTe, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao)));

            return query.SetTimeout(300).List<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao>> ConsultarRelatorioIntegracaoAsync(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataIntegracaoInicial, DateTime dataIntegracaoFinal, int codigoGrupoPessoas, int codigoTipoIntegracao, int codigoCarga, int codigoCTe, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioIntegracao(false, propriedades, dataEmissaoInicial, dataEmissaoFinal, dataIntegracaoInicial, dataIntegracaoFinal, codigoGrupoPessoas, codigoTipoIntegracao, codigoCarga, codigoCTe, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite));

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao)));

            return await query.SetTimeout(300).ListAsync<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao>();
        }

        public int ContarConsultaRelatorioIntegracao(List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataIntegracaoInicial, DateTime dataIntegracaoFinal, int codigoGrupoPessoas, int codigoTipoIntegracao, int codigoCarga, int codigoCTe)
        {
            var query = this.SessionNHiBernate.CreateSQLQuery(ObterSelectConsultaRelatorioIntegracao(true, propriedades, dataEmissaoInicial, dataEmissaoFinal, dataIntegracaoInicial, dataIntegracaoFinal, codigoGrupoPessoas, codigoTipoIntegracao, codigoCarga, codigoCTe, "", "", "", "", 0, 0));

            return query.SetTimeout(300).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioIntegracao(bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataIntegracaoInicial, DateTime dataIntegracaoFinal, int codigoGrupoPessoas, int codigoTipoIntegracao, int codigoCarga, int codigoCTe, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioIntegracao(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioIntegracao(ref where, ref joins, dataEmissaoInicial, dataEmissaoFinal, dataIntegracaoInicial, dataIntegracaoFinal, codigoGrupoPessoas, codigoCarga, codigoCTe, codigoTipoIntegracao);

            if (!string.IsNullOrWhiteSpace(propAgrupa))
            {
                SetarSelectRelatorioIntegracao(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                orderBy = " order by " + propAgrupa + " " + dirAgrupa;
            }

            if (!string.IsNullOrWhiteSpace(propOrdena))
            {
                if (propOrdena != propAgrupa)
                    orderBy += (orderBy.Length <= 0 ? " order by " : ", ") + propOrdena + " " + dirOrdena;
            }

            return (count ? "select distinct(count(0) over ())" : "select CargaCTeIntegracao.CCI_CODIGO Codigo, " + (select.Length > 0 ? select.Substring(0, select.Length - 2) : string.Empty)) +
                   " from T_CARGA_CTE_INTEGRACAO CargaCTeIntegracao inner join T_CARGA_CTE CargaCTe on CargaCTeIntegracao.CCT_CODIGO = CargaCTe.CCT_CODIGO " + joins +
                   " where CargaCTeIntegracao.INT_SITUACAO_INTEGRACAO = 2 " + where +
                   " group by CargaCTeIntegracao.CCI_CODIGO" + (groupBy.Length > 0 ? ", " + groupBy.Substring(0, groupBy.Length - 2) : string.Empty) + (count ? string.Empty : (orderBy.Length > 0 ? orderBy : " order by 1 asc ")) +
                   (count || (inicio <= 0 && limite <= 0) ? "" : " offset " + inicio.ToString() + " rows fetch next " + limite.ToString() + " rows only;");
        }

        private void SetarWhereRelatorioIntegracao(ref string where, ref string joins, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal, DateTime dataIntegracaoInicial, DateTime dataIntegracaoFinal, int codigoGrupoPessoas, int codigoCarga, int codigoCTe, int codigoTipoIntegracao)
        {
            if (dataEmissaoInicial != DateTime.MinValue)
            {
                where += " AND CTe.CON_DATAHORAEMISSAO >= '" + dataEmissaoInicial.ToString("yyyy-MM-dd") + "'";

                if (!joins.Contains(" CTe "))
                    joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
            }

            if (dataEmissaoFinal != DateTime.MinValue)
            {
                where += " AND CTe.CON_DATAHORAEMISSAO < '" + dataEmissaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

                if (!joins.Contains(" CTe "))
                    joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
            }

            if (dataIntegracaoInicial != DateTime.MinValue)
                where += " AND CargaCTeIntegracao.INT_DATA_INTEGRACAO >= '" + dataIntegracaoInicial.ToString("yyyy-MM-dd") + "'";

            if (dataIntegracaoFinal != DateTime.MinValue)
                where += " AND CargaCTeIntegracao.INT_DATA_INTEGRACAO < '" + dataIntegracaoFinal.AddDays(1).ToString("yyyy-MM-dd") + "'";

            if (codigoGrupoPessoas > 0)
            {
                where += " AND Carga.GRP_CODIGO = " + codigoGrupoPessoas.ToString();

                if (!joins.Contains(" Carga "))
                    joins += " inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO";
            }

            if (codigoCarga > 0)
                where += " AND CargaCTe.CAR_CODIGO = " + codigoCarga.ToString();

            if (codigoCTe > 0)
                where += " AND CargaCTe.CON_CODIGO = " + codigoCTe.ToString();

            if (codigoTipoIntegracao > 0)
                where += " AND CargaCTeIntegracao.TPI_CODIGO = " + codigoTipoIntegracao.ToString();
        }

        private void SetarSelectRelatorioIntegracao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "GrupoPessoas":
                    if (!select.Contains("GrupoPessoas"))
                    {
                        select += "GrupoPessoas.GRP_DESCRICAO GrupoPessoas, ";
                        groupBy += "GrupoPessoas.GRP_DESCRICAO, ";

                        if (!joins.Contains(" Carga "))
                            joins += " inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO";

                        if (!joins.Contains(" GrupoPessoas "))
                            joins += " left join T_GRUPO_PESSOAS GrupoPessoas on GrupoPessoas.GRP_CODIGO = Carga.GRP_CODIGO";
                    }
                    break;
                case "NumeroCTe":
                    if (!select.Contains("NumeroCTe"))
                    {
                        select += "CTe.CON_NUM NumeroCTe, ";
                        groupBy += "CTe.CON_NUM, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                case "SerieCTe":
                    if (!select.Contains("SerieCTe"))
                    {
                        select += "SerieCTe.ESE_NUMERO SerieCTe, ";
                        groupBy += "SerieCTe.ESE_NUMERO, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";

                        if (!joins.Contains(" SerieCTe "))
                            joins += " inner join T_EMPRESA_SERIE SerieCTe on SerieCTe.ESE_CODIGO = CTe.CON_SERIE";
                    }
                    break;
                case "NumeroCarga":
                    if (!select.Contains(" NumeroCarga,"))
                    {
                        select += "Carga.CAR_CODIGO_CARGA_EMBARCADOR NumeroCarga, ";
                        groupBy += "Carga.CAR_CODIGO_CARGA_EMBARCADOR, ";

                        if (!joins.Contains(" Carga "))
                            joins += " inner join T_CARGA Carga on Carga.CAR_CODIGO = CargaCTe.CAR_CODIGO";
                    }
                    break;
                case "DataEmissao":
                    if (!select.Contains("DataEmissao"))
                    {
                        select += "CTe.CON_DATAHORAEMISSAO DataEmissao, ";
                        groupBy += "CTe.CON_DATAHORAEMISSAO, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                case "DataIntegracao":
                    if (!select.Contains("DataIntegracao"))
                    {
                        select += "CargaCTeIntegracao.INT_DATA_INTEGRACAO DataIntegracao, ";
                        groupBy += "CargaCTeIntegracao.INT_DATA_INTEGRACAO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao,"))
                    {
                        select += "CargaCTeIntegracao.INT_SITUACAO_INTEGRACAO Situacao, ";
                        groupBy += "CargaCTeIntegracao.INT_SITUACAO_INTEGRACAO, ";
                    }
                    break;
                case "Tentativas":
                    if (!select.Contains("Tentativas"))
                    {
                        select += "CargaCTeIntegracao.INT_NUMERO_TENTATIVAS Tentativas, ";
                        groupBy += "CargaCTeIntegracao.INT_NUMERO_TENTATIVAS, ";
                    }
                    break;
                case "SituacaoIntegracao":
                    if (!select.Contains("SituacaoIntegracao"))
                    {
                        select += "CargaCTeIntegracao.INT_SITUACAO_INTEGRACAO SituacaoIntegracao, ";
                        groupBy += "CargaCTeIntegracao.INT_SITUACAO_INTEGRACAO, ";
                    }
                    break;
                case "Mensagem":
                    if (!select.Contains("Mensagem"))
                    {
                        select += "CargaCTeIntegracao.INT_PROBLEMA_INTEGRACAO Mensagem, ";
                        groupBy += "CargaCTeIntegracao.INT_PROBLEMA_INTEGRACAO, ";
                    }
                    break;
                case "TipoIntegracao":
                    if (!select.Contains("TipoIntegracao"))
                    {
                        select += "TipoIntegracao.TPI_DESCRICAO TipoIntegracao, ";
                        groupBy += "TipoIntegracao.TPI_DESCRICAO, ";

                        if (!joins.Contains(" TipoIntegracao "))
                            joins += " left join T_TIPO_INTEGRACAO TipoIntegracao on TipoIntegracao.TPI_CODIGO = CargaCTeIntegracao.TPI_CODIGO";
                    }
                    break;
                case "AliquotaICMS":
                    if (!select.Contains("AliquotaICMS"))
                    {
                        select += "CTe.CON_ALIQ_ICMS AliquotaICMS, ";
                        groupBy += "CTe.CON_ALIQ_ICMS, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                case "ValorFrete":
                    if (!count && !select.Contains("ValorFrete"))
                    {
                        select += "SUM(CTe.CON_VALOR_FRETE) ValorFrete, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                case "ValorAReceber":
                    if (!count && !select.Contains("ValorAReceber"))
                    {
                        select += "SUM(CTe.CON_VALOR_RECEBER) ValorAReceber, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                case "ValorICMS":
                    if (!count && !select.Contains("ValorICMS"))
                    {
                        select += "SUM(CTe.CON_VAL_ICMS) ValorICMS, ";

                        if (!joins.Contains(" CTe "))
                            joins += " inner join T_CTE CTe on CTe.CON_CODIGO = CargaCTe.CON_CODIGO";
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
