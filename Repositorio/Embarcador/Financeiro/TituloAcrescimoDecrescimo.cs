using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class TituloAcrescimoDecrescimo : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo>
    {
        public TituloAcrescimoDecrescimo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo> BuscarPorTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloAcrescimoDecrescimo>();
            var result = from obj in query where obj.Titulo.Codigo == codigo select obj;
            return result.ToList();
        }

    }

}
