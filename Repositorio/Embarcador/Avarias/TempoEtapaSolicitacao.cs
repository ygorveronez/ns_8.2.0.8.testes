using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Avarias
{
    public class TempoEtapaSolicitacao : RepositorioBase<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>
    {
        public TempoEtapaSolicitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao BuscarUltimaEtapa(int codigoSolicitacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>();
            var result = from obj in query
                         where obj.SolicitacaoAvaria.Codigo == codigoSolicitacaoAvaria && obj.Saida == null
                         select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao> BuscarPorSolicitacao(int codigoSolicitacaoAvaria)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>();
            var result = from obj in query where obj.SolicitacaoAvaria.Codigo == codigoSolicitacaoAvaria select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao> TemposNaEtapa(List<int> codigosSolicitacaoAvaria, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaSolicitacao etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Avarias.TempoEtapaSolicitacao>();

            var result = query.Where(obj =>
                            codigosSolicitacaoAvaria.Contains(obj.SolicitacaoAvaria.Codigo) &&
                            obj.Etapa == etapa &&
                            obj.Saida != null);

            return result.ToList();

        }


    }
}