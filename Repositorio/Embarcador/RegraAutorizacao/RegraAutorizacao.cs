using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.RegraAutorizacao
{
    public class RegraAutorizacao<TRegra> : RepositorioBase<TRegra> where TRegra : Dominio.Entidades.Embarcador.RegraAutorizacao.RegraAutorizacao
    {
        #region Construtores

        public RegraAutorizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos

        public TRegra BuscarPorCodigo(int codigo)
        {
            var regraAutorizacao = this.SessionNHiBernate.Query<TRegra>()
                .Where(regra => regra.Codigo == codigo)
                .FirstOrDefault();

            return regraAutorizacao;
        }

        #endregion
    }
}
