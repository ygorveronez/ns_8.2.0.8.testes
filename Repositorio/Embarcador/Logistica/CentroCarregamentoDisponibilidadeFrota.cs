using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class CentroCarregamentoDisponibilidadeFrota : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>
    {
        public CentroCarregamentoDisponibilidadeFrota(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CentroCarregamentoDisponibilidadeFrota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }


        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> BuscarPorCentrosDeCarregamentoEDia(List<int> codigoCentrosCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigoCentrosCarregamento.Contains(ent.CentroCarregamento.Codigo) && ent.Dia == dia);

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>> BuscarPorCentrosDeCarregamentoEDiaAsync(List<int> codigoCentrosCarregamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DiaSemana dia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();

            var result = from obj in query select obj;
            result = result.Where(ent => codigoCentrosCarregamento.Contains(ent.CentroCarregamento.Codigo) && ent.Dia == dia);

            return result.ToListAsync(CancellationToken);
        }


        public List<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.CentroCarregamentoDisponibilidadeFrota>();

            var result = from obj in query select obj;

            return result.ToList();

        }


    }

}
