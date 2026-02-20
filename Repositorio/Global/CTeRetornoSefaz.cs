using System.Collections.Generic;
using System.Linq;

namespace Repositorio
{
    public class CTeRetornoSefaz : RepositorioBase<Dominio.Entidades.CTeRetornoSefaz>, Dominio.Interfaces.Repositorios.CTeRetornoSefaz
    {
        public CTeRetornoSefaz(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.CTeRetornoSefaz> BuscarPorCTe(int codigoCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeRetornoSefaz>();
            var result = from obj in query where obj.CTe.Codigo == codigoCTe select obj;
            return result.ToList();
        }

        public bool ExisteVariasTentativasIntegracao(int codigoCte, int numeroMinimoTentativas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.CTeRetornoSefaz>();
            var result = query.Where(x => x.ErroSefaz.CodigoDoErro == 8888).Select(x => x.CTe.Codigo).Where(x => x == codigoCte);

            return result.Count() > numeroMinimoTentativas;
        }
    }
}
