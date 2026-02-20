using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class DestinoPrioritarioCalculoFreteLocalidade : RepositorioBase<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade>
    {

        public DestinoPrioritarioCalculoFreteLocalidade(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade BuscarPorLocalidadeEConfiguracao(int codigo, int localidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade>();
            var result = from obj in query where obj.DestinoPrioritarioCalculoFrete.Codigo == codigo && obj.Localidade.Codigo == localidade select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade> BuscarLocalidadeParaRemover(int codigo, List<int> localidadesRemover)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade>();
            var result = from obj in query where obj.DestinoPrioritarioCalculoFrete.Codigo == codigo && !localidadesRemover.Contains(obj.Localidade.Codigo) select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade ValidarPorTabelaFreteELocalidades(int tabelaFrete, List<int> localidades)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.DestinoPrioritarioCalculoFreteLocalidade>();
            var result = from obj in query
                         where 
                             obj.DestinoPrioritarioCalculoFrete.TabelasFrete.Any(a => a.Codigo == tabelaFrete) 
                             && obj.DestinoPrioritarioCalculoFrete.Ativo == true
                             && obj.Ativo == true
                             && localidades.Contains(obj.Localidade.Codigo)
                         orderby obj.Ordem ascending select obj;

            return result.FirstOrDefault();
        }
    }
}
