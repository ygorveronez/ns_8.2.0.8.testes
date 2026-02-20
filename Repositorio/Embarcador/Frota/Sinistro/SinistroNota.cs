using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroNota : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroNota>
    {
        public SinistroNota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroNota> BuscarPorFluxoSinistro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroNota>()
                .Where(obj => obj.Sinistro.Codigo == codigo);

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Frota.SinistroNota BuscarPorFluxoSinistroEDocumentoEntrada(int codigo, int codigoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroNota>()
                .Where(obj => obj.Sinistro.Codigo == codigo && obj.DocumentoEntrada.Codigo == codigoDocumento);

            return query.FirstOrDefault();
        }
    }
}
