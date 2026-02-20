using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public class ControleVisitaPessoaFoto : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto>
    {
        public ControleVisitaPessoaFoto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto> BuscarFotosAtivas(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto>();
            var result = from obj in query where obj.ControleVisitaPessoa.Codigo == codigo && obj.Status == true select obj;
            return result.ToList();
        }
    }
}
