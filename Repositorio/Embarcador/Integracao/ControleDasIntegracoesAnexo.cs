using System.Linq.Dynamic.Core;
using System.Linq;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Integracao
{
    public class ControleDasIntegracoesAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo>
    {
        #region Constructores
        public ControleDasIntegracoesAnexo(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        #endregion
        #region Metodos Publicos
        public List<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo> BuscarPorControleIntegracao (long codigoControle)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ControleDasIntegracoesAnexo>();
            query = query.Where(c => c.EntidadeAnexo.Codigo == codigoControle);
            return query.ToList();
        }
        #endregion
    }
}
