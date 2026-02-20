using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Cargas.ControleEntrega
{
    public class CargaEntregaCheckListAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa>
    {
        public CargaEntregaCheckListAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> BuscarPorCargaEntregaChecklist(int codigoCargaEntregaChecklist)
        {
            var consultaAlternativas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa>()
                .Where(o => o.CargaEntregaCheckListPergunta.CargaEntregaCheckList.Codigo == codigoCargaEntregaChecklist);

            return consultaAlternativas.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa> BuscarPorPergunta(int codigoPergunta)
        {
            var consultaAlternativas = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaCheckListAlternativa>()
                .Where(o => o.CargaEntregaCheckListPergunta.Codigo == codigoPergunta);

            return consultaAlternativas.ToList();
        }
    }
}

