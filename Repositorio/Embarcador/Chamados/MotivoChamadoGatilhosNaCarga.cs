using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Chamados
{
    public class MotivoChamadoGatilhosNaCarga : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga>
    {

        #region Construtor

        public MotivoChamadoGatilhosNaCarga(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Metodos Publicos

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga> BuscarPorMotivoChamado(int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga>();

            query = query.Where(g => g.MotivoChamado.Codigo == codigoMotivoChamado);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga> BuscarPorTipoGatilhoNaCarga(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMotivoChamadoGatilhoNaCarga tipoGatilho)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.MotivoChamadoGatilhosNaCarga>();

            query = query.Where(g => g.Gatilho == tipoGatilho);

            return query.ToList();
        }

        public void DeletarPorMotivoChamado(int codigoMotivoChamado)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE FROM MotivoChamadoGatilhosNaCarga WHERE MCH_CODIGO = :codigoMotivoChamado").SetInt32("codigoMotivoChamado", codigoMotivoChamado).ExecuteUpdate();
        }
        #endregion
    }
}
