using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.MontagemCarga
{
    public class CarregamentoApolice : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>
    {
        public CarregamentoApolice(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CarregamentoApolice(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice> BuscarPorCarregamento(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento select obj;

            return result.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>> BuscarPorCarregamentoAsync(int codigoCarregamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>();

            var result = from obj in query where obj.Carregamento.Codigo == codigoCarregamento select obj;

            return result.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro> BuscarApolicesPorCarregamento(int codigoCarregamento)
        {
            var consultaCarregamentoApolice = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoApolice
                .Select(o => o.ApoliceSeguro)
                .ToList();
        }

        public bool ExistePorCarregamento(int codigoCarregamento)
        {
            var consultaCarregamentoApolice = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoApolice.Any();
        }

        public Task<bool> ExistePorCarregamentoAsync(int codigoCarregamento)
        {
            var consultaCarregamentoApolice = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MontagemCarga.CarregamentoApolice>()
                .Where(o => o.Carregamento.Codigo == codigoCarregamento);

            return consultaCarregamentoApolice.AnyAsync();
        }
    }
}
