using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class AlcadaContratoFreteTerceiroTerceiros : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>
    {
        public AlcadaContratoFreteTerceiroTerceiros(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroTerceiros>();
            var result = from obj in query where obj.RegraContratoFreteTerceiro.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
