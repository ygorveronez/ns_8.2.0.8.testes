using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class BemDepreciacao : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao>
    {
        public BemDepreciacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao BuscarPorBem(int codigoBem, int mes, int ano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.BemDepreciacao>();
            var result = from obj in query where obj.Bem.Codigo == codigoBem && obj.Mes == mes && obj.Ano == ano select obj;
            return result.FirstOrDefault();
        }
    }
}
