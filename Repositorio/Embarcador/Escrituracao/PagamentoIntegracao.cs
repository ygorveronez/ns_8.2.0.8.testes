using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using MongoDB.Driver;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class PagamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>
    {
        public PagamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PagamentoIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.DocumentoFaturamento)
                .FirstOrDefault();
        }


        public bool PagamentoOutrarDocumentoRecebeuMiro(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var result = from obj in query where obj.Pagamento.Carga.Codigo == codigoCarga select obj;
            var documentoIntegrado = result.Select(x => x.DocumentoFaturamento).FirstOrDefault();
            return !string.IsNullOrEmpty(documentoIntegrado?.NumeroMiro ?? "");
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorCodigoCte(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.DocumentoFaturamento.CTe.Codigo == codigo && obj.DocumentoFaturamento.Situacao != SituacaoDocumentoFaturamento.Cancelado select obj;
            return resut
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.DocumentoFaturamento)
                .FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorChaveDocumento(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.DocumentoFaturamento.CTe.Chave == chave && obj.DocumentoFaturamento.Situacao != SituacaoDocumentoFaturamento.Cancelado select obj;
            return resut
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.DocumentoFaturamento)
                .FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> BuscarPorPagamentosPendentesDeIntegracao(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query
                        where obj.Pagamento.Codigo == Pagamento
      && obj.SituacaoIntegracao == SituacaoIntegracao.AgIntegracao
      && obj.DataIntegracao <= DateTime.Now
                        select obj;
            return resut
                .Fetch(obj => obj.Pagamento)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Destinatario)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Empresa)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.Serie)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.TomadorPagador)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ThenFetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.Pagamento.Codigo == Pagamento select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorNumeroFolha(string numeroFolha)
        {
            var consultaDocumentoFaturamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>()
                .Where(x => x.DocumentoFaturamento.NumeroFolha == numeroFolha);

            return consultaDocumentoFaturamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> BuscarPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.Pagamento.Codigo == Pagamento select obj;
            return resut.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPrimeiroPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.Pagamento.Codigo == Pagamento select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> BuscarPorPagamento(List<int> pagamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            query = query.Where(obj => pagamentos.Contains(obj.Pagamento.Codigo));

            return query.ToList();
        }

        public bool PagamentoPossuiIntegragracaoDocumentoPendentes(int codigoPagamento, int codigoPagamentoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();
            var resut = from obj in query where obj.Codigo != codigoPagamentoIntegracao && obj.Pagamento.Codigo == codigoPagamento && obj.SituacaoIntegracao != SituacaoIntegracao.Integrado select obj;
            return resut.Any();
        }

        public int ContarPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.Pagamento.Codigo == Pagamento select obj;

            return resut.Count();
        }

        public int ContarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public int ContarPorPagamentoETipoIntegracao(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public int ContarPorPagamentoESituacaoDiff(int codigo, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigo && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>> BuscarAgIntegracaoAsync(CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.Pagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPagamento.AguardandoIntegracao select obj;

            return result.ToListAsync(cancellationToken);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> _Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoPagamento > 0)
                result = result.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoPagamento, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.DocumentoFaturamento)
                .ThenFetch(obj => obj.CTe)
                .ToList();
        }

        public int ContarConsulta(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(codigoPagamento, situacao);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorPagamentoETipoIntegracao(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return resut.FirstOrDefault();
        }

        public bool ExistePorPagamentoETipoIntegracaoETipoDocumento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao, Dominio.Enumeradores.TipoDocumento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.TipoIntegracao.Tipo == tipoIntegracao && obj.TipoDocumento == tipoDocumento select obj;

            return resut.Any();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarPorDocumentoFaturamentoETipoIntegracao(int codigoDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.DocumentoFaturamento.Codigo == codigoDocumento && obj.TipoIntegracao.Tipo == tipoIntegracao select obj;

            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao BuscarIntegracoesPorNumeroMiro(string numeroMiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where obj.DocumentoFaturamento.NumeroMiro == numeroMiro select obj;

            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao> BuscarPorDocumentosFaturamento(List<int> codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoIntegracao>();

            var resut = from obj in query where codigoDocumento.Contains(obj.DocumentoFaturamento.Codigo) select obj;

            return resut.ToList();
        }

        public IList<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento> BuscarParaAPIDePagamentos(List<int> codigosPagamentos)
        {
            StringBuilder sql = new();

            sql.Append($@"SELECT PAG_CODIGO AS CodigoIntPagamento,
                         DFA_CODIGO AS CodigoIntDocumentoPagamento,
                         TPI_CODIGO AS TipoInt,
                         INT_SITUACAO_INTEGRACAO AS SituacaoIntegracaoInt,
                         INT_PROBLEMA_INTEGRACAO AS MensagemRetorno,
                         INT_DATA_INTEGRACAO AS DataEnvioIntegracao
                         FROM T_PAGAMENTO_INTEGRACAO");

            if (!codigosPagamentos.IsNullOrEmpty())
                sql.Append($@" WHERE PAG_CODIGO IN ({string.Join(", ", codigosPagamentos)})");
            else
                sql.Append($@" WHERE PAG_CODIGO = 0");

            var consulta = this.SessionNHiBernate.CreateSQLQuery(sql.ToString());

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento)));

            return consulta.SetTimeout(600).List<Dominio.ObjetosDeValor.WebService.Rest.Escrituracao.BuscarDadosPagamento.IntegracaoPagamento>();
        }
    }
}