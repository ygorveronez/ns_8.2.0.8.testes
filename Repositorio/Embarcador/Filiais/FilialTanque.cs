using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Tanques
{
    public class FilialTanque : RepositorioBase<Dominio.Entidades.Embarcador.Filiais.FilialTanque>
    {
        public FilialTanque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Filiais.FilialTanque BuscarPorFilialETanque(int filial, int tanque)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialTanque>();

            var result = from obj in query select obj;
            result = result.Where(o => o.Filial.Codigo == filial && o.Tanque.Codigo == tanque) ;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Filiais.FilialTanque BuscarPorFilialETanque(string filial, string tanque)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialTanque>();

            var result = from obj in query select obj;
            result = result.Where(o => (o.Filial.CodigoFilialEmbarcador == filial || o.Filial.OutrosCodigosIntegracao.Contains(filial)) && o.Filial.Ativo && o.Tanque.ID == tanque);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Filiais.FilialTanque> BuscarPorFilial(int filial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Filiais.FilialTanque>();

            var result = from obj in query select obj;
            result = result.Where(o => o.Filial.Codigo == filial);

            return result.ToList();
        }

    }

}

