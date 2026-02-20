using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>
    {
        public ContratoFreteTransportadorModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarCodigosNaoPesentesNaLista(int contrato, List<int> codigosModelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigosModelo.Contains(obj.ModeloVeicular.Codigo)
                         select obj.ModeloVeicular.Codigo;

            return result.ToList();
        }

        public List<int> BuscarCodigoModelosVecularesPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                         select obj.ModeloVeicular.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular BuscarPorContratoEModelo(int contrato, int modelo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.ModeloVeicular.Codigo == modelo
                         select obj;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular> BuscarPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorModeloVeicular>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                         select obj;

            return result.ToList();
        }
    }
}
