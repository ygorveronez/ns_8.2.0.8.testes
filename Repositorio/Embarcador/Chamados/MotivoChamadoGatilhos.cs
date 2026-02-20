using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoGatilhos : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>
    {

        #region Construtor

        public MotivoChamadoGatilhos(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MotivoChamadoGatilhos(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> BuscarPorMotivoChamado(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado);

            return query.ToList();
        }
        public Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList BuscarNivelPorMotivoChamado(int codigoMotivoChamado, EscalationList nivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado && g.Nivel == nivel);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> BuscarNivelPorMotivoChamadoAsync(int codigoMotivoChamado, EscalationList nivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado && g.Nivel == nivel);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> BuscarPorNiveisEMotivosChamado(List<int> codigosMotivoChamado, List<EscalationList> niveis)
        {
            List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList> result = new List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

            int maxParameters = 1000;

            // Divide `codigosMotivoChamado` em lotes
            int startMotivos = 0;
            while (startMotivos < codigosMotivoChamado.Count)
            {
                List<int> motivosLote = codigosMotivoChamado.Skip(startMotivos).Take(maxParameters).ToList();

                // Para cada lote de `codigosMotivoChamado`, divide `niveis` também em lotes
                int startNiveis = 0;
                while (startNiveis < niveis.Count)
                {
                    List<EscalationList> niveisLote = niveis.Skip(startNiveis).Take(maxParameters).ToList();

                    var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosTempoList>();

                    query = query.Where(g => motivosLote.Contains(g.MotivoChamado.Codigo)
                                          && niveisLote.Contains(g.Nivel)
                                          && g.Nivel != EscalationList.SemNivel);

                    result.AddRange(query.ToList());

                    startNiveis += maxParameters;
                }

                startMotivos += maxParameters;
            }

            return result;
        }

        #endregion
    }
}
