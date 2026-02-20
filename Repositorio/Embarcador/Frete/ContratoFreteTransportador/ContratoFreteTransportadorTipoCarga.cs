using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorTipoCarga : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>
    {
        public ContratoFreteTransportadorTipoCarga(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarTipoCargasNaoPesentesNaLista(int contrato, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigos.Contains(obj.TipoDeCarga.Codigo)
                         select obj.TipoDeCarga.Codigo;

            return result.ToList();
        }
        
        public List<int> BuscarCodigosTipoCargasPorContrato(int contrato)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                         select obj.TipoDeCarga.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga BuscarPorContratoETipoCarga(int contrato, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.TipoDeCarga.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoDeCarga BuscarPrimeiroTipoCargaPorContrato(int contrato)
        {
            var consultaContratoFreteTransportadorTipoCarga = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoCarga>()
                .Where(contratoTipoCarga => contratoTipoCarga.ContratoFrete.Codigo == contrato);

            return consultaContratoFreteTransportadorTipoCarga.Select(contratoTipoCarga => contratoTipoCarga.TipoDeCarga).FirstOrDefault();
        }
    }
}
