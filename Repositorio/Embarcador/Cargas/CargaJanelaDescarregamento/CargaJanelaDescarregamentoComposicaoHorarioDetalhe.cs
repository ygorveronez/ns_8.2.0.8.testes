using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamentoComposicaoHorarioDetalhe : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe>
    {
        #region Construtores

        public CargaJanelaDescarregamentoComposicaoHorarioDetalhe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe> BuscarPorComposicaoHorario(List<int> codigosComposicoesHorario)
        {
            var consultaCargaJanelaDescarregamentoComposicaoHorarioDetalhe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoComposicaoHorarioDetalhe>()
                .Where(o => codigosComposicoesHorario.Contains(o.ComposicaoHorario.Codigo));

            return consultaCargaJanelaDescarregamentoComposicaoHorarioDetalhe
                .OrderBy(o => o.Ordem)
                .ToList();
        }

        #endregion Métodos Públicos
    }
}
