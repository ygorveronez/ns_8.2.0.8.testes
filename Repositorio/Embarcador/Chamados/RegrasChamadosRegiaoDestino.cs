using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class RegrasChamadosRegiaoDestino : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>
    {
        public RegrasChamadosRegiaoDestino(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino> BuscarPorRegras(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.RegrasChamadosRegiaoDestino>();
            var result = from obj in query where obj.RegrasAnaliseChamados.Codigo == codigo select obj;

            return result.OrderBy("Ordem ascending").ToList();
        }

    }
}