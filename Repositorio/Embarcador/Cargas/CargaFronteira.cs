using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaFronteira : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>
    {
        public CargaFronteira(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>();

            var result = from obj in query where obj.Carga.Codigo == carga select obj;

            return result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>();
            var result = from obj in query where cargas.Contains(obj.Carga.Codigo) select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Cliente> BuscarFronteirasPorCarga(int codigoCarga)
        {
            return this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaFronteira>()
                .Where(obj => obj.Carga.Codigo == codigoCarga)
                .Select(obj => obj.Fronteira)
                .ToList();
        }

        public void CopiarFronteirasParaCarga(List<Dominio.Entidades.Embarcador.Cargas.CargaFronteira> fronteiras, Dominio.Entidades.Embarcador.Cargas.Carga cargaNova)
        {
            foreach (var fronteira in fronteiras)
            {
                this.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaFronteira
                {
                    Carga = cargaNova,
                    Fronteira = fronteira.Fronteira,
                });
            }
        }

        public void DeletarPorCarga(int codigoCarga)
        {
            if (UnitOfWork.IsActiveTransaction())
            {
                UnitOfWork.Sessao
                    .CreateQuery($"delete CargaFronteira where Carga.Codigo = :codigoCarga")
                    .SetInt32("codigoCarga", codigoCarga)
                    .ExecuteUpdate();
            }
            else
            {
                try
                {
                    UnitOfWork.Start();

                    UnitOfWork.Sessao
                        .CreateQuery($"delete CargaFronteira where Carga.Codigo = :codigoCarga")
                        .SetInt32("codigoCarga", codigoCarga)
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
