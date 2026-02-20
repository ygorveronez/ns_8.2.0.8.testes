using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoTipoCriticidadeAtendimento : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>
    {
        #region Construtores

        public MotivoChamadoTipoCriticidadeAtendimento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public MotivoChamadoTipoCriticidadeAtendimento(UnitOfWork unitOfWork, CancellationToken cancellationToken)
            : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Assíncronos

        /// <summary>
        /// Busca um tipo de criticidade por código (async)
        /// </summary>
        public Task<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento> BuscarPorCodigoAsync(int codigo)
        {
            if (codigo <= 0)
                return null;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();
            var result = query.Where(t => t.Codigo == codigo);
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        /// <summary>
        /// Busca tipos de criticidade por motivo de chamado (async)
        /// </summary>
        public Task<List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>> BuscarPorMotivoChamadoAsync(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();
            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado);
            return query.ToListAsync(CancellationToken);
        }

        /// <summary>
        /// Busca tipos de criticidade por motivo de chamado e tipo específico (async)
        /// </summary>
        public Task<List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>> BuscarPorMotivoChamadoETipoAsync(int codigoMotivoChamado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroCriticidade tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();
            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado && g.Tipo == tipo);
            return query.ToListAsync(CancellationToken);
        }

        /// <summary>
        /// Busca tipos de criticidade por código do motivo do chamado (async)
        /// </summary> 
        public Task<List<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>> BuscarPorCodigoMotivoChamado(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.TipoCriticidadeAtendimento>();
            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado);
            return query.ToListAsync(CancellationToken);
        }
        #endregion
    }

}