using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class ManifestacaoDestinatario : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario>
    {
        public ManifestacaoDestinatario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public long BuscarUltimoIdLote(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Max(o => (long?)o.IdLote) ?? 0L;
        }
    }
}
