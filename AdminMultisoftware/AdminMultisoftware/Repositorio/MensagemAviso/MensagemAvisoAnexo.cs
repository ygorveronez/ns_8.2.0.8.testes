using System.Linq;

namespace AdminMultisoftware.Repositorio.MensagemAviso
{
    public class MensagemAvisoAnexo : RepositorioBase<Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo>
    {
        public MensagemAvisoAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo BuscarPorGuid(string guidArquivo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.MensagemAviso.MensagemAvisoAnexo>();
            query = query.Where(o => o.GuidArquivo == guidArquivo);

            return query.FirstOrDefault();
        }

    }
}
