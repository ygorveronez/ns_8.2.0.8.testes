using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Simulacoes
{
    public class RegrasSimulacaoTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>
    {
        public RegrasSimulacaoTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Simulacoes.RegrasSimulacaoTransportador>();
            var result = from obj in query where obj.RegrasAutorizacaoSimulacao.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }
    }
}


