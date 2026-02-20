using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo>
    {
        public ContratoFreteTransportadorAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> BuscarPorContrato(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo>();
            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigo select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo> Consultar(int codigoContrato, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo>();
            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigoContrato select obj;

            return result
                .OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"))
                .Skip(inicioRegistros)
                .Take(maximoRegistros)
                .ToList();
        }

        public int ContarConsulta(int codigoContrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorAnexo>();
            var result = from obj in query where obj.ContratoFreteTransportador.Codigo == codigoContrato select obj;

            return result.Count();
        }


    }
}
