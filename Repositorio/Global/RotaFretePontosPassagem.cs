using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class RotaFretePontosPassagem : RepositorioBase<Dominio.Entidades.RotaFretePontosPassagem>
    {
        #region Construtores
        public RotaFretePontosPassagem(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public RotaFretePontosPassagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        public Dominio.Entidades.RotaFretePontosPassagem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFretePontosPassagem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.RotaFretePontosPassagem> BuscarPorRotaFrete(int rotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFretePontosPassagem>();
            var result = from obj in query where obj.RotaFrete.Codigo == rotaFrete select obj;
            return result.OrderBy(obj => obj.Ordem).ToList();
        }
        public async Task<List<Dominio.Entidades.RotaFretePontosPassagem>> BuscarPorRotaFreteAsync(int rotaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RotaFretePontosPassagem>();
            var result = from obj in query where obj.RotaFrete.Codigo == rotaFrete select obj;
            return await result.OrderBy(obj => obj.Ordem).ToListAsync();
        }

        public void DeletarPorRotaFrete(int rotaFrete)
        {
            UnitOfWork.Sessao.CreateQuery("DELETE RotaFretePontosPassagem obj WHERE obj.RotaFrete.Codigo = :codigoRotaFrete ")
                              .SetInt32("codigoRotaFrete", rotaFrete)
                              .ExecuteUpdate();

        }

    }
}
