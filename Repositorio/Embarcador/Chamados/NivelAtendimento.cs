using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class NivelAtendimento : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>
    {
        #region Constructor
        public NivelAtendimento(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public NivelAtendimento(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion


        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> BuscarPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Codigo == codigoChamado);
            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Chamados.NivelAtendimento BuscarPrimeiroPorChamado(int codigoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Codigo == codigoChamado);
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Carga.Codigo == codigoCarga);
            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> BuscarNiveisAtendimentosExpirados(int skip, int quantidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>()
                        .Where(n => n.Chamado.Situacao != SituacaoChamado.Finalizado
                                    && !n.FoiNotificado)
                        .Skip(skip)
                        .Take(quantidade);

            return query.ToList();
        }

        public int CountBuscarNiveisAtendimentosExpirados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Situacao != SituacaoChamado.Finalizado &&
                                !n.FoiNotificado);
            return query.Count();
        }

        public Dominio.Entidades.Embarcador.Chamados.NivelAtendimento BuscarSeChamadoComNivelAtual(int codigoChamado, EscalationList nivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Codigo == codigoChamado && n.Nivel == nivel);
            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento> BuscarSeChamadoComNivelAtualAsync(int codigoChamado, EscalationList nivel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.NivelAtendimento>();
            query = query.Where(n => n.Chamado.Codigo == codigoChamado && n.Nivel == nivel);
            return query.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion
    }
}
