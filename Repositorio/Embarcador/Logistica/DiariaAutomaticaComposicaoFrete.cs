using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class DiariaAutomaticaComposicaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete>
    {
        public DiariaAutomaticaComposicaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete> BuscarPorDiariaAutomatica(int codigoDiariaAutomatica)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.DiariaAutomaticaComposicaoFrete>();
            var result = from obj in query where obj.DiariaAutomatica.Codigo == codigoDiariaAutomatica select obj;
            return result.ToList();
        }

    }
}
