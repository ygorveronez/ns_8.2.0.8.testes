using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Avarias
{
    public class LoteEDIIntegracao : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>
    {
        public LoteEDIIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public LoteEDIIntegracao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> _Consultar(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();
            var result = from obj in query select obj;

            if (lote > 0)
                result = result.Where(obj => obj.Lote.Codigo == lote);

            if (situacao != null)
                result = result.Where(o => o.SituacaoIntegracao == situacao);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> Consultar(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(lote, situacao);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var result = _Consultar(lote, situacao);

            return result.Count();
        }



        public List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> BuscarIntegracoesPendentes(double minutosACadaTentativa, string propOrdenacao, string dirOrdenacao, int numeroRegistrosPorVez)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            var result = from obj in query
                         where obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao ||
                               (obj.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao &&
                                obj.NumeroTentativas < obj.LayoutEDI.NumeroTentativasAutomaticasIntegracao && obj.DataIntegracao <= DateTime.Now.AddMinutes(-minutosACadaTentativa))
                         select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(0).Take(numeroRegistrosPorVez).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao> BuscarPorLote(int lote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == lote select obj;

            if (situacao.HasValue)
                result = result.Where(o => o.SituacaoIntegracao == situacao.Value);

            return result.ToList();
        }

        public Task<int> ContarPorLoteAsync(int codigoLote, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.LoteEDIIntegracao>();

            var result = from obj in query where obj.Lote.Codigo == codigoLote && obj.SituacaoIntegracao == situacao select obj.Codigo;

            return result.CountAsync(CancellationToken);
        }
    }
}
