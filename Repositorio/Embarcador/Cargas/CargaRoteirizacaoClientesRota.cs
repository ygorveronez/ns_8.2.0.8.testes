using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaRoteirizacaoClientesRota : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota>
    {
        public CargaRoteirizacaoClientesRota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota> BuscarPorCargaRoteirizacao(int cargaRoteirizacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaRoteirizacaoClientesRota>();
            var resut = from obj in query where obj.CargaRoteirizacao.Codigo == cargaRoteirizacao select obj;
            return resut
                .Fetch(obj => obj.Cliente)
                .OrderBy(obj => obj.Ordem).ToList();
        }
    }
}
