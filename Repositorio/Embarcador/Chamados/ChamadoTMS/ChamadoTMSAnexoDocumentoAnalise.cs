using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class ChamadoTMSAnexoDocumentoAnalise : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise>
    {
        public ChamadoTMSAnexoDocumentoAnalise(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise> BuscarPorChamado(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoDocumentoAnalise>();
            var result = from obj in query where obj.EntidadeAnexo.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}
