using System.Linq;

namespace Repositorio
{
    public class RPSNFSe : RepositorioBase<Dominio.Entidades.RPSNFSe>, Dominio.Interfaces.Repositorios.RPSNFSe
    {
        public RPSNFSe(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public int ObterUltimoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.RPSNFSe>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            int? retorno = result.Max(o => (int?)o.Numero);

            return retorno.HasValue ? retorno.Value : 0;
        }

        public Dominio.Entidades.RPSNFSe BuscarPorNFSe(int codigoNFSe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.NFSe>();

            var result = from obj in query where obj.Codigo == codigoNFSe && obj.RPS != null select obj.RPS;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.RPSNFSe BuscarPorNFSeCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            var result = from obj in query where obj.Codigo == codigoCTe && obj.RPS != null select obj.RPS;

            return result.FirstOrDefault();
        }
    }
}
