using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaCargaOrganizacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao>
    {
        public CargaCargaOrganizacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao> BuscarPorCarga(int codigoCarga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao> queryCargaCargaOrganizacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCargaOrganizacao>();

            return queryCargaCargaOrganizacao.Where(cco => cco.Carga.Codigo == codigoCarga).ToList();
        }
    }
}
