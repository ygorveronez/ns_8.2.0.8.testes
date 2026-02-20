using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class ContratoFreteTransportadorTipoOperacao : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao>
    {
        public ContratoFreteTransportadorTipoOperacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<int> BuscarTipoOperacoesNaoPesentesNaLista(int contrato, List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && !codigos.Contains(obj.TipoOperacao.Codigo)
                         select obj.TipoOperacao.Codigo;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao BuscarPorContratoETipoOperacao(int contrato, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoFreteTransportadorTipoOperacao>();
            var result = from obj in query
                         where
                            obj.ContratoFrete.Codigo == contrato
                            && obj.TipoOperacao.Codigo == codigo
                         select obj;

            return result.FirstOrDefault();
        }
    }
}
