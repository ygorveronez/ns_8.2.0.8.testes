using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Escrituracao
{
    public class LoteEscrituracaoEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>
    {
        public LoteEscrituracaoEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteEscrituracaoEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        public Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> Consultar(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoLoteEscrituracao > 0)
                result = result.Where(obj => obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros)
                .Fetch(obj => obj.TipoIntegracao)
                .Fetch(obj => obj.LayoutEDI)
                .ToList();
        }

        public int ContarConsulta(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query select obj;

            if (codigoLoteEscrituracao > 0)
                result = result.Where(obj => obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result.Count();
        }

        public Task<List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>> BuscarIntegracoesPendentesAsync(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez, CancellationToken cancellationToken)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query
                         where obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoInformada &&
                               obj.TipoIntegracao.Tipo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.NaoPossuiIntegracao &&
                               //!obj.LoteEscrituracao.GerandoIntegracoes &&
                               obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result
                .Fetch(obj => obj.LayoutEDI)
                .Fetch(obj => obj.LoteEscrituracao)
                .Fetch(obj => obj.Empresa)
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToListAsync(cancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> BuscarPorLoteEscrituracao(int codigoLoteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao select obj;

            return result.ToList();
        }
        
        public int ContarPorLoteEscrituracaoESituacaoDiff(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacaoDiff)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.SituacaoIntegracao != situacaoDiff select obj.Codigo;

            return result.Count();
        }

        public int ContarPorLoteEscrituracao(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao> BuscarPorLoteEscrituracao(int codigoLoteEscrituracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao> BuscarTipoIntegracaoPorLoteEscrituracao(int codigoLoteEscrituracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao select obj.TipoIntegracao.Tipo;

            return result.Distinct().ToList();
        }

        public bool VerificarSeExistePorLoteEscrituracao(int codigoLoteEscrituracao, int codigoTipoIntegracao, int codigoLayoutEDI, double tomador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Escrituracao.LoteEscrituracaoEDIIntegracao>();

            var result = from obj in query
                         where obj.LoteEscrituracao.Codigo == codigoLoteEscrituracao && obj.TipoIntegracao.Codigo == codigoTipoIntegracao && obj.LayoutEDI.Codigo == codigoLayoutEDI
                         select obj.Codigo;


            return result.Any();
        }
    }
}
