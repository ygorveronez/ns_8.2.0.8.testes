using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Escrituracao
{
    public class PagamentoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>
    {
        public PagamentoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public PagamentoEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoPagamento > 0)
                result = result.Where(obj => obj.Pagamento.Codigo == codigoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query
                         where /*obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&*/
                               //!obj.Pagamento.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao BuscarUltimoPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao BuscarPorPagamentoELayout(int codigoPagamento, int layout)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.LayoutEDI.Codigo == layout select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> BuscarPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> BuscarPorPagamentos(List<int> codigosPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where codigosPagamento.Contains(obj.Pagamento.Codigo) select obj;

            return result.ToList();
        }

        public int ContarPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento select obj.Codigo;

            return result.Count();
        }

        public int ContarPorPagamentoESituacaoDiff(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public int ContarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao> BuscarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorPagamento(int codigoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query where obj.Pagamento.Codigo == codigoPagamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public bool VerificarSeExistePorPagamento(int codigoPagamento, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.PagamentoEDIIntegracao>();

            var result = from obj in query
                         where obj.Pagamento.Codigo == codigoPagamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }
    }
}
