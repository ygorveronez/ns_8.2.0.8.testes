using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio
{
    public class StatusConsultaImpressaoUnidade : RepositorioBase<Dominio.Entidades.StatusConsultaImpressaoUnidade>
    {
        public StatusConsultaImpressaoUnidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.StatusConsultaImpressaoUnidade BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.StatusConsultaImpressaoUnidade>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.StatusConsultaImpressaoUnidade BuscarPorUnidadeDocumento(int numeroUnidade, Dominio.Enumeradores.TipoObjetoConsulta documento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.StatusConsultaImpressaoUnidade>();
            var result = from obj in query where obj.NumeroDaUnidade == numeroUnidade && obj.Documento == documento select obj;
            return result.FirstOrDefault();
        }
    }
}


