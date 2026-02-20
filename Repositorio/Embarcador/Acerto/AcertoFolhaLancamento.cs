using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Acerto
{
    public class AcertoFolhaLancamento : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento>
    {
        public AcertoFolhaLancamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento> BuscarPorAcerto(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoFolhaLancamento>();
            var result = from obj in query where obj.AcertoViagem.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
