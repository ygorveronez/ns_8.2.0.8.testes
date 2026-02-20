using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorFilial : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>
    {
        public ContratoFreteTransportadorFilial(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarFiliaisNaoPesentesNaLista(int contrato, List<int> codigosFiliais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigosFiliais.Contains(obj.Filial.Codigo)
                         select obj.Filial.Codigo;

            return result.ToList();
        }

        public List<int> BuscarCodigosFiliaisPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                         select obj.Filial.Codigo;

            return result.ToList();

        }
        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial BuscarPorContratoEFilial(int contrato, int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorFilial>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.Filial.Codigo == filial
                         select obj;

            return result.FirstOrDefault();
        }
    }
}
