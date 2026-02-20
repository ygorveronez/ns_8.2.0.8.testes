using System;
using System.Linq;

namespace Repositorio.Embarcador.Login
{
    public class TentativaLoginMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Login.TentativaLoginMotorista>
    {
        public TentativaLoginMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public int BuscarQuantidadeTentativasPorDataCargaMotorista(DateTime data, int codigoCarga, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Login.TentativaLoginMotorista>()
                .Where(x => x.CodigoMotorista == codigoMotorista && x.CodigoCarga == codigoCarga && x.Data > data.AddMinutes(-30) && x.Data  < data.AddMinutes(30));

            return query.Count();
        }
    }
}
