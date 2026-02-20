using System.Linq;

namespace Repositorio
{
    public class NFSePreNFSItupeva : RepositorioBase<Dominio.Entidades.NFSePreNFSItupeva>, Dominio.Interfaces.Repositorios.NFSePreNFSItupeva
    {
        public NFSePreNFSItupeva(UnitOfWork unitOfWork) : base(unitOfWork) { }


        public bool RegistroJaLancado(string numeroValidacao, string numeroBloco, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSePreNFSItupeva>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.NumeroValidacao == numeroValidacao && obj.NumeroBloco == numeroBloco select obj;

            return result.Count() > 0;
        }

        public Dominio.Entidades.NFSePreNFSItupeva BuscarPendente(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSePreNFSItupeva>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa && obj.RPSNFSe == null select obj;

            return result.OrderBy(o => int.Parse(o.NumeroBloco)).ThenBy(o => int.Parse(o.NumeroSequencia)).FirstOrDefault();
        }

        public Dominio.Entidades.NFSePreNFSItupeva BuscarPreNFSePorRPS(int codigoRPS)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSePreNFSItupeva>();

            var result = from obj in query where obj.RPSNFSe.Codigo == codigoRPS select obj;

            return result.FirstOrDefault();
        }
    }
}
