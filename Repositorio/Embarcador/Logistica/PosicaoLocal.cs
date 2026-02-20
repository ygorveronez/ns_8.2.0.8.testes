using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica
{
    public class PosicaoLocal : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal>
    {
        #region Métodos públicos
        public PosicaoLocal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Logistica.PosicaoLocal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal> BuscarPorPosicao(long codigoPosicao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PosicaoLocal>();
            var result = query.Where(ent => ent.Posicao.Codigo == codigoPosicao);
            return result.Fetch(obj => obj.Local).ToList();
        }


        #endregion

    }

}
