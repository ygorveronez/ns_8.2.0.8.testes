using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frota
{
    public class SinistroDocumentacaoEnvolvidos : RepositorioBase<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos>
    {
        public SinistroDocumentacaoEnvolvidos(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos> BuscarPorFluxoSinistro(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos>()
                .Where(obj => obj.SinistroDados.Codigo == codigo);

            return query.ToList();
        }
    }
}
