using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class CancelamentoProvisaoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>
    {
        public CancelamentoProvisaoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CancelamentoProvisaoEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao> Consultar(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoCancelamentoProvisao > 0)
                result = result.Where(obj => obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoCancelamentoProvisao > 0)
                result = result.Where(obj => obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao> BuscarIntegracoesPendentes(int numeroTentativas, double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query
                         where /*obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&*/
                               //!obj.CancelamentoProvisao.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < numeroTentativas && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao BuscarUltimoPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao orderby obj.Codigo descending select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao> BuscarPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao BuscarPorCancelamentoProvisaoELayoutEDI(int codigoCancelamentoProvisao, int layoutEDI)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao && obj.LayoutEDI.Codigo == layoutEDI select obj;

            return result.FirstOrDefault();
        }

        public int ContarPorProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao select obj.Codigo;

            return result.Count();
        }

        public int ContarPorCancelamentoProvisaoESituacaoDiff(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public int ContarPorCancelamentoProvisao(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao> BuscarPorCancelamentoProvisao(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>> BuscarPorCancelamentoProvisaoAsync(int codigoCancelamentoProvisao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToListAsync(cancellationToken);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorCancelamentoProvisao(int codigoCancelamentoProvisao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public bool VerificarSeExistePorProvisao(int codigoCancelamentoProvisao, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.CancelamentoProvisaoEDIIntegracao>();

            var result = from obj in query
                         where obj.CancelamentoProvisao.Codigo == codigoCancelamentoProvisao && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }
    }
}
