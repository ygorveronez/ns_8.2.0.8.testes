using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class FilaCarregamentoVeiculoAtrelado : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado>
    {
        #region Construtores

        public FilaCarregamentoVeiculoAtrelado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado BuscarPorFilaCarregamentoVeiculo(int codigoFilaCarregamentoVeiculo)
        {
            var consultaFilaCarregamentoVeiculoAtrelado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado>()
                .Where(o => o.FilaCarregamentoVeiculo.Codigo == codigoFilaCarregamentoVeiculo);

            return consultaFilaCarregamentoVeiculoAtrelado.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado BuscarPorTracao(int codigoTracao)
        {
            var consultaFilaCarregamentoVeiculoAtrelado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoVeiculoAtrelado>()
                .Where(o => o.FilaCarregamentoMotorista.ConjuntoVeiculo.Tracao.Codigo == codigoTracao);

            return consultaFilaCarregamentoVeiculoAtrelado.FirstOrDefault();
        }

        #endregion
    }
}
