using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamentoComposicaoHorario : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario>
    {
        #region Construtores

        public CargaJanelaDescarregamentoComposicaoHorario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario> BuscarPorCargaJanelaDescarregamento(int codigoCargaJanelaDescarregamento)
        {
            var consultaCargaJanelaDescarregamentoComposicaoHorario = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorario>()
                .Where(o => o.CargaJanelaDescarregamento.Codigo == codigoCargaJanelaDescarregamento);

            return consultaCargaJanelaDescarregamentoComposicaoHorario
                .OrderByDescending(o => o.Codigo)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
