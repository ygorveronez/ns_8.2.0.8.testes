using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoFronteira : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira>
    {
        public CarregamentoFronteira(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoFronteira(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira> BuscarPorCarregamento(int carregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira>();

            var result = from obj in query where obj.Carregamento.Codigo == carregamento select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira>> BuscarPorCarregamentosAsync(List<int> carregamentos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoFronteira>();

            var result = from obj in query where carregamentos.Contains(obj.Carregamento.Codigo) select obj;

            return result.ToListAsync(CancellationToken);
        }

        public void DeletarPorCarregamento(int codigoCarregamento)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete CarregamentoFronteira where Carregamento.Codigo = :codigoCarregamento")
                    .SetInt32("codigoCarregamento", codigoCarregamento)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery($"delete CarregamentoFronteira where Carregamento.Codigo = :codigoCarregamento")
                        .SetInt32("codigoCarregamento", codigoCarregamento)
                        .ExecuteUpdate();

                    UnitOfWork.CommitChanges();
                }
                catch
                {
                    UnitOfWork.Rollback();
                    throw;
                }
            }
        }
    }
}
