using System.Collections.Generic;
using System.Linq;


namespace Repositorio
{
    public class PalavrasChaveNFe : RepositorioBase<Dominio.Entidades.PalavrasChaveNFe>, Dominio.Interfaces.Repositorios.PalavrasChaveNFe
    {
        public PalavrasChaveNFe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.PalavrasChaveNFe BuscaPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PalavrasChaveNFe>();

            var result = from obj in query where obj.Codigo == codigo select obj;
            
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.PalavrasChaveNFe BuscaPorPalavra(int codigoEmpresa, string palavra)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PalavrasChaveNFe>();

            var result = from obj in query where obj.Palavra.Equals(palavra) select obj;

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.PalavrasChaveNFe> ConsultarTodas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.PalavrasChaveNFe>();

            var result = from obj in query select obj;

            return result.ToList();
        }

    }
}
