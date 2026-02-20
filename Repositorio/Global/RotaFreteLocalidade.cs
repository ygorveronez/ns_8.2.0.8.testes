using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio
{
    public class RotaFreteLocalidade : RepositorioBase<Dominio.Entidades.RotaFreteLocalidade>
    {
        #region Construtores

        public RotaFreteLocalidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public void DeletarPorRotaFrete(int codigoRotaFrete)
        {
            UnitOfWork.Sessao
                .CreateQuery($"delete RotaFreteLocalidade localidade where localidade.RotaFrete.Codigo = :codigo")
                .SetInt32("codigo", codigoRotaFrete)
                .ExecuteUpdate();
        }

        public bool ExistePorRotaFrete(int codigoRotaFrete, int codigoLocalidade)
        {
            var consultaRotaFreteLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteLocalidade>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete && o.Localidade.Codigo == codigoLocalidade);

            return consultaRotaFreteLocalidade.Any();
        }

        public List<Dominio.Entidades.RotaFreteLocalidade> BuscarPorRotaFrete(int codigoRotaFrete)
        {
            var consultaRotaFreteLocalidade = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFreteLocalidade>()
                .Where(o => o.RotaFrete.Codigo == codigoRotaFrete);

            return consultaRotaFreteLocalidade.ToList();
        }

        #endregion Métodos Públicos
    }
}
