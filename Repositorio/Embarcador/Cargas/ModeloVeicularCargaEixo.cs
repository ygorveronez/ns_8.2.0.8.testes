using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas
{
    public class ModeloVeicularCargaEixo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo>
    {
        public ModeloVeicularCargaEixo(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public List<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo> BuscarPorModeloVeicular(int codigoModeloVeicular)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo>();
            var result = from obj in query where obj.ModeloVeicularCarga.Codigo == codigoModeloVeicular select obj;
            return result.ToList();
        }

        public bool ContemEixoConfigurado(int codigoModeloVeicular, int numeroEixo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCargaEixo>();
            var result = from obj in query where obj.ModeloVeicularCarga.Codigo == codigoModeloVeicular && obj.Numero == numeroEixo select obj;
            return result.Count() > 0;
        }
    }
}
