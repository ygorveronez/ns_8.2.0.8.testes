using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaLocaisPrestacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>
    {
        public CargaLocaisPrestacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaLocaisPrestacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => cargas.Contains(p.Carga.Codigo));
            return result.ToList();
        }


        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == carga);
            return result
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado)
                .ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == carga)
                .Fetch(obj => obj.LocalidadeInicioPrestacao)
                .ThenFetch(obj => obj.Estado)
                .Fetch(obj => obj.LocalidadeTerminoPrestacao)
                .ThenFetch(obj => obj.Estado);

            return result.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao BuscarPorOrigemDestinoECarga(int carga, int inicioPrestacao, int fimPrestacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == carga && p.LocalidadeInicioPrestacao.Codigo == inicioPrestacao && p.LocalidadeTerminoPrestacao.Codigo == fimPrestacao);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> BuscarPorCargaUFOrigemEUFDestino(int carga, string ufOrigem, string UFDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == carga && p.LocalidadeInicioPrestacao.Estado.Sigla == ufOrigem && p.LocalidadeTerminoPrestacao.Estado.Sigla == UFDestino);
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao BuscarPrimeiroPorCargaUFOrigemEUFDestino(int carga, string ufOrigem, string UFDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>()
                .Where(obj => obj.Carga.Codigo == carga && obj.LocalidadeInicioPrestacao.Estado.Sigla == ufOrigem && obj.LocalidadeTerminoPrestacao.Estado.Sigla == UFDestino);

            return query.FirstOrDefault();
        }


        public List<Dominio.Entidades.Localidade> BuscarEstadosDestinoPrestacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.LocalidadeTerminoPrestacao;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Localidade BuscarEstadoOrigemPrestacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.LocalidadeInicioPrestacao;

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Localidade> BuscarEstadosOrigemPrestacao(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            var result = from obj in query where obj.Carga.Codigo == codigoCarga select obj.LocalidadeInicioPrestacao;

            return result.Distinct().ToList();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao BuscarPrimeiroRegistroPorCarga(int carga)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();

            query = query.Where(p => p.Carga.Codigo == carga);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao> BuscarPorCargaEUFDestino(int carga, string ufDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaLocaisPrestacao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == carga && p.LocalidadeTerminoPrestacao.Estado.Sigla == ufDestino);
            return result.ToList();
        }
    }
}
