using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class FilaCarregamentoConjuntoVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo>
    {
        #region Construtores

        public FilaCarregamentoConjuntoVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion
        #region Métodos Publicos
        public Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo BuscarUltimoConjuntoPorVeiculo(int codigoVeiculo)
        {
            var consultaFilaConjuntoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.FilaCarregamentoConjuntoVeiculo>();

            consultaFilaConjuntoVeiculo = consultaFilaConjuntoVeiculo
                .Where(o => o.Tracao.Codigo == codigoVeiculo ||  o.Reboques.Where(c => c.Codigo == codigoVeiculo).Any()).OrderByDescending(o => o.Codigo);

            return consultaFilaConjuntoVeiculo.FirstOrDefault();
        }

        #endregion
    }
}
