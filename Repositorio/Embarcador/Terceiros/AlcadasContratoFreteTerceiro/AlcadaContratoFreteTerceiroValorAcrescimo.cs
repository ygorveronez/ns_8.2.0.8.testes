using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Terceiros
{
    public class AlcadaContratoFreteTerceiroValorAcrescimo : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>
    {
        public AlcadaContratoFreteTerceiroValorAcrescimo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo> BuscarPorRegra(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.AlcadaContratoFreteTerceiroValorAcrescimo>();
            var result = from obj in query where obj.RegraContratoFreteTerceiro.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}
