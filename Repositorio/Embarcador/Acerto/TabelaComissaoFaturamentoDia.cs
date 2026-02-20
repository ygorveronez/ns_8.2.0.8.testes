using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Acerto
{
    public class TabelaComissaoFaturamentoDia : RepositorioBase<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia>
    {
        public TabelaComissaoFaturamentoDia(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia> BuscarPorTabela(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.TabelaComissaoFaturamentoDia>();
            var result = from obj in query where obj.TabelaComissaoMotorista.Codigo == codigo select obj;
            return result.ToList();
        }
    }
}