using System.Linq;
using System.Linq.Dynamic.Core;


namespace Repositorio.Embarcador.Terceiros
{
    public class ContratoFreteIntegracaoArquivo : RepositorioBase<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo>
    {
        public ContratoFreteIntegracaoArquivo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Terceiros.ContratoFreteIntegracaoArquivo>();
            var resut = from obj in query where obj.Codigo == codigo select obj;
            return resut.FirstOrDefault();
        }

    }
}
