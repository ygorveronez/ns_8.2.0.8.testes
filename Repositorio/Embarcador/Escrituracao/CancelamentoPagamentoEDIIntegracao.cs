using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoPagamentoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>
    {
        public CancelamentoPagamentoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao> Consultar(int codigoCancelamentoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoCancelamentoPagamento > 0)
                result = result.Where(obj => obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoCancelamentoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoCancelamentoPagamento > 0)
                result = result.Where(obj => obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query
                         where /*obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&*/
                               //!obj.CancelamentoPagamento.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao BuscarUltimoPorCancelamentoPagamento(int codigoCancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao> BuscarPorCancelamentoPagamento(int codigoCancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao BuscarPorCancelamentoPagamentoELayoutEDI(int codigoCancelamentoPagamento, int layoutEDI)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento && obj.LayoutEDI.Codigo == layoutEDI select obj;

            return result.FirstOrDefault();
        }



        public int ContarPorCancelamentoPagamento(int codigoCancelamentoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao> BuscarPorCancelamentoPagamento(int codigoCancelamentoPagamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCancelamentoPagamento(int codigoCancelamentoPagamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public bool VerificarSeExistePorProvisao(int codigoCancelamentoPagamento, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoPagamentoEDIIntegracao>();

            var result = from obj in query
                         where obj.CancelamentoPagamento.Codigo == codigoCancelamentoPagamento && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }
    }
}
