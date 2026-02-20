using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Financeiro
{
    public class MovimentacaoContaPagar : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>
    {

        #region Constructores 
        public MovimentacaoContaPagar(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #endregion

        #region Metodos Publicoss

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> Pesquisar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtroPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametro)
        {
            var cosulta = _Consultar(filtroPesquisa);
            return ObterLista(cosulta, parametro);
        }

        public int ContarPesquisa(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtroPesquisa)
        {
            var cosulta = _Consultar(filtroPesquisa);
            return cosulta.Count();
        }


        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarPendentesProcessamento(int quantidade = 25)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>();

            query = query.Where(x => x.SituacaoProcessamento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoProcessamento.AguardandoProcessamento);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarPorTermoQuitacao(int codigoTermoQuitacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>();

            query = query.Where(x => x.TermoQuitacaoFinanceiro.Codigo == codigoTermoQuitacao);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarMovimentacaoFinanceiraContaPagar(DateTime? dataInicial, DateTime? dataFinal, List<int> codigoTransportador, bool possuiTermoQuitacao = false)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>()
                .Where(x => codigoTransportador.Contains(x.Transportador.Codigo));

            if (dataInicial.HasValue)
                query = query.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento ? x.DataUpload >= dataInicial : x.DataDocumento >= dataInicial);

            if (dataFinal.HasValue)
                query = query.Where(x => x.TipoRegistro == TipoRegistro.NotasCompensadasXAdiantamento ? x.DataUpload < dataFinal.Value.AddDays(1) : x.DataDocumento < dataFinal.Value.AddDays(1));

            if (!possuiTermoQuitacao)
                query = query.Where(x => x.TermoQuitacaoFinanceiro == null);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarPorCodigos(List<int> codigosMovimentacoes)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>();

            query = query.Where(x => codigosMovimentacoes.Contains(x.Codigo));

            return query.ToList();
        }

        public bool ExisteRegistroPendentesEmAbertoParaTransportador(int codigoTransportador, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>()
                .Where(x => x.Transportador.Codigo == codigoTransportador && x.TipoRegistro == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro.PendentesemAberto);

            if (dataInicial.HasValue)
                query = query.Where(x => x.DataDocumento >= dataInicial);

            if (dataFinal.HasValue)
                query = query.Where(x => x.DataDocumento <= dataFinal);

            return query.Any();
        }

        public void ReprocessarContasPagarSemMiro(DateTime? dataInicio, DateTime? dataFim)
        {
            //0 = Aguardando Processamento

            string sql = $"UPDATE T_MOVIMENTACAO_CONTA_PAGAR SET MCP_SITUACAO_PROCESSAMENTO = 0, MCP_OBSERVACAO_MIRO = null  WHERE CON_CODIGO IS NULL ";

            if (dataInicio.HasValue)
                sql += $" AND MCP_DATA_DOCUMENOT >= '{dataInicio.Value.ToString("yyyy-MM-dd")}'";

            if (dataFim.HasValue)
                sql += $" AND MCP_DATA_DOCUMENOT < '{dataFim.Value.AddDays(1).ToString("yyyy-MM-dd")}'";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetTimeout(6000).UniqueResult();
        }

        public int RemoverVinculoTermoQuitacao(List<int> movimentacoes)
        {
            string query = $"update T_MOVIMENTACAO_CONTA_PAGAR set TQU_CODIGO = null  where MCP_CODIGO in ({string.Join(",", movimentacoes)})"; // SQL-INJECTION-SAFE
            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public int AdicionarVinculoTermoQuitacao(int codigoTermo, List<int> movimentacoes)
        {
            string query = $"update T_MOVIMENTACAO_CONTA_PAGAR set TQU_CODIGO = {codigoTermo}  where MCP_CODIGO in ({string.Join(",", movimentacoes)})"; // SQL-INJECTION-SAFE
            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarPorTransportador(int transportador, DateTime? dataInicial, DateTime? dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>()
                                                                        .Where(x => x.TermoQuitacaoFinanceiro == null);

            query = from obj in query where obj.Transportador.Codigo == transportador select obj;

            if (dataFinal.HasValue)
                query = from obj in query where obj.DataDocumento.Value <= dataFinal.Value select obj;

            if (dataInicial.HasValue)
                query = from obj in query where obj.DataDocumento.Value >= dataInicial.Value select obj;

            return query.ToList();
        }

        public bool ExisteMovimentacaoPorNumeroMiro(List<string> numeroMiro)
        {
            var sql = $"SELECT TOP 1 MCP_CODIGO FROM T_MOVIMENTACAO_CONTA_PAGAR WHERE MCP_DATA_COMPENSAMENTO IS NOT NULL";

            if (numeroMiro.Count > 0)
                sql = sql + " AND (";

            foreach (var numero in numeroMiro)
                sql = sql + $" MCP_CHAVE_REFERENCIA LIKE '{numero}%' OR";

            if (sql.EndsWith("OR"))
                sql = sql.Substring(0, sql.Length - 2) + " )";

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql);

            return consulta.SetTimeout(600).UniqueResult<int>() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> BuscarMovimentacaoesAprovarManualmente(List<int> codigosSelecionados)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>().Where(x => x.SituacaoProcessamento == SituacaoProcessamento.AguardandoProcessamento);

            if (codigosSelecionados.Count > 0)
                query = query.Where(x => codigosSelecionados.Contains(x.Codigo));
            return query.ToList();
        }
        #endregion

        #region Metodos Privados
        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> _Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentacaoContaPagar filtroPesquisa)
        {

            IQueryable<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentacaoContaPagar>();

            if (filtroPesquisa.Transportador > 0)
            {
                if (filtroPesquisa.TodasFiliaisTransportador)
                {
                    filtroPesquisa.CodigosFiliaisTransportador.Add(filtroPesquisa.Transportador);
                    query = query.Where(x => filtroPesquisa.CodigosFiliaisTransportador.Contains(x.Transportador.Codigo));
                }
                else
                    query = query.Where(x => x.Transportador.Codigo == filtroPesquisa.Transportador);
            }

            if (filtroPesquisa.DataCompensacaoFinal.HasValue)
            {
                if (filtroPesquisa.DataCompensacaoFinal.HasValue && filtroPesquisa.DataCompensacaoInicial.Value == filtroPesquisa.DataCompensacaoFinal.Value)
                    query = query.Where(x => x.DataCompensamento.Value < filtroPesquisa.DataCompensacaoFinal.Value.AddDays(1));
                else
                    query = query.Where(x => x.DataCompensamento <= filtroPesquisa.DataCompensacaoFinal.Value);

            }

            if (filtroPesquisa.DataCompensacaoInicial.HasValue)
                query = query.Where(x => x.DataCompensamento.Value >= filtroPesquisa.DataCompensacaoInicial.Value);

            if (filtroPesquisa.DataDocInicial.HasValue)
                query = query.Where(x => x.DataDocumento >= filtroPesquisa.DataDocInicial);

            if (filtroPesquisa.DataDocFinal.HasValue)
                if (filtroPesquisa.DataDocInicial.HasValue && filtroPesquisa.DataDocInicial.Value == filtroPesquisa.DataDocFinal.Value)
                    query = query.Where(x => x.DataDocumento < filtroPesquisa.DataDocFinal.Value.AddDays(1));
                else
                    query = query.Where(x => x.DataDocumento <= filtroPesquisa.DataDocFinal.Value);

            if (filtroPesquisa.TipoArquivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoRegistro.SemTipo)
                query = query.Where(x => x.TipoRegistro == filtroPesquisa.TipoArquivo);

            if (filtroPesquisa.SituacaoDocumentoMovimentacao.HasValue && filtroPesquisa.SituacaoDocumentoMovimentacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoMovimentacao.Todos)
            {
                if (filtroPesquisa.SituacaoDocumentoMovimentacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoMovimentacao.ComDocumento)
                    query = query.Where(x => x.MiroRecebida);
                else
                    query = query.Where(x => !x.MiroRecebida);
            }

            if (!string.IsNullOrEmpty(filtroPesquisa.DocumentoCompensacao))
                query = query.Where(x => x.ClrngDoc == filtroPesquisa.DocumentoCompensacao);

            if (filtroPesquisa.SituacaoProcessamento.HasValue)
                query = query.Where(x => x.SituacaoProcessamento == filtroPesquisa.SituacaoProcessamento.Value);

            if (filtroPesquisa.CodigoNumeroDocumento > 0)
                query = query.Where(x => x.CTe.Numero == filtroPesquisa.CodigoNumeroDocumento);

            if (filtroPesquisa.NumeroTermoQuitacao > 0)
                query = query.Where(x => x.TermoQuitacaoFinanceiro.NumeroTermo == filtroPesquisa.NumeroTermoQuitacao);

            return query;
        }



        #endregion
    }
}
