using System.Linq;

namespace Repositorio.Embarcador.Documentos
{
    public class EventoDesacordoServico : RepositorioBase<Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico>
    {
        public EventoDesacordoServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public long BuscarUltimoIdLote(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico>();

            query = query.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return query.Max(o => (long?)o.IdLote) ?? 0L;
        }

        public Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico BuscarPorChaveStatus(string chaveCTe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusEventoDesacordoServico status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Documentos.EventoDesacordoServico>();

            query = query.Where(o => o.ChaveCTe == chaveCTe && o.Status == status);

            return query.FirstOrDefault();
        }
    }
}
