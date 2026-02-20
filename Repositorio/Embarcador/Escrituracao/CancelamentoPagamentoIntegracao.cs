using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoPagamentoIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>
    {
        public CancelamentoPagamentoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> BuscarPorCancelamentoPagamento(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.CancelamentoPagamento.Codigo == codigo select obj;
            return resut.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTiposPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.CancelamentoPagamento.Codigo == Pagamento select obj.TipoIntegracao.Tipo;
            return resut.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> BuscarPorCarga(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.CancelamentoPagamento.Codigo == Pagamento select obj;
            return resut.ToList();
        }

        public int ContarPorPagamento(int Pagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();

            var resut = from obj in query where obj.CancelamentoPagamento.Codigo == Pagamento select obj;

            return resut.Count();
        }

        public int ContarPorPagamento(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoPagamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public int ContarPorPagamentoETipoIntegracao(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao tipoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();

            var resut = from obj in query where obj.CancelamentoPagamento.Codigo == codigoPagamento && obj.TipoIntegracao.Tipo == tipoIntegracao select obj.Codigo;

            return resut.Count();
        }

        public int ContarPorPagamentoESituacaoDiff(int codigo, SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigo && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;
            
            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao BuscarPorNumeroMiro(string numeroMiro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.DocumentoFaturamento.NumeroMiro == numeroMiro && obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno select obj;
            return resut.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao BuscarPorChaveDocumento(string chave)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();
            var resut = from obj in query where obj.DocumentoFaturamento.CTe.Chave == chave && obj.SituacaoIntegracao == SituacaoIntegracao.AgRetorno select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento> BuscarAgIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamento>();
            var result = from obj in query where obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoPagamento.AgIntegracao select obj;

            return result.ToList();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> _Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoPagamento > 0)
                result = result.Where(obj => obj.CancelamentoPagamento.Codigo == codigoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoIntegracao> Consultar(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
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
                .ToList();
        }

        public int ContarConsulta(int codigoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(codigoPagamento, situacao);

            return result.Count();
        }
    }
}
