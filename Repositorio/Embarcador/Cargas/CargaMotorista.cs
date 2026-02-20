using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaMotorista : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>
    {
        public CargaMotorista(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CargaMotorista(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken)
        {
        }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result
                .Fetch(obj => obj.Motorista)
                .ToList();
        }
        public Task<List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>> BuscarPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result
                .Fetch(obj => obj.Motorista)
                .ToListAsync();
        }

        public Task<int> BuscarPrimeiroCodigoPorCargaAsync(int codigoCarga)
        {
            string query = @$"select top 1 CAR_MOTORISTA from t_carga_motorista where car_codigo = {codigoCarga}";
            var result = this.SessionNHiBernate.CreateSQLQuery(query);

            return result.SetTimeout(600).UniqueResultAsync<int>();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaMotorista BuscarPorCargaMotorista(int codigoCarga, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga)
                .Where(q => q.Motorista.Codigo == codigoMotorista);

            return result
                .Fetch(obj => obj.Motorista)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> BuscarPorCargas(List<int> cargas)
        {
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> result = new List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();

            int take = 1000;
            int start = 0;
            while (start < cargas?.Count)
            {
                List<int> tmp = cargas.Skip(start).Take(take).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
                var filter = from obj in query
                             where tmp.Contains(obj.Carga.Codigo)
                             select obj;

                result.AddRange(filter.Fetch(obj => obj.Motorista)
                                       .ThenFetch(obj => obj.Localidade)
                                       .ThenFetch(obj => obj.Pais)
                                      .ToList());

                start += take;
            }

            return result;


        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista).ToList();
        }

        public Task<List<Dominio.Entidades.Usuario>> BuscarMotoristasPorCargaAsnyc(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista).ToListAsync();
        }

        public List<string> BuscarNomeMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista.Nome).ToList();
        }

        public List<int> BuscarCodigoMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista.Codigo).ToList();
        }
        public async Task<List<int>> BuscarCodigoMotoristasPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return await result.Select(obj => obj.Motorista.Codigo).ToListAsync();
        }

        public List<string> BuscarCPFMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista.CPF).ToList();
        }

        public string BuscarPrimeiroCPFMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>()
                .Where(p => p.Carga.Codigo == codigoCarga);

            return query.Select(obj => obj.Motorista.CPF).FirstOrDefault();
        }
        public async Task<string> BuscarPrimeiroCPFMotoristasPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>()
                .Where(p => p.Carga.Codigo == codigoCarga);

            return await query.Select(obj => obj.Motorista.CPF).FirstOrDefaultAsync();
        }

        public List<string> BuscarCNHMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista.NumeroHabilitacao).ToList();
        }

        public List<string> BuscarRGMotoristasPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista.RG).ToList();
        }

        public Dominio.Entidades.Usuario BuscarPrimeiroMotoristaPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Carga.Codigo == codigoCarga);
            return result.Select(obj => obj.Motorista).FirstOrDefault();
        }

        public Task<Dominio.Entidades.Usuario> BuscarPrimeiroMotoristaPorCargaAsync(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>()
                .Where(p => p.Carga.Codigo == codigoCarga);

            return query.Select(obj => obj.Motorista).FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Cargas.Carga BuscarUltimaCargaMotorista(string cpfMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Motorista.CPF == cpfMotorista && p.Carga.CargaFechada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);
            return result.Select(obj => obj.Carga).OrderByDescending(c => c.Codigo).FirstOrDefault();
        }

        public Dominio.Entidades.Veiculo BuscarVeiculoUltimaCargaMotorista(int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Motorista.Codigo == codigoMotorista && p.Carga.Veiculo != null && p.Carga.CargaFechada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada && p.Carga.SituacaoCarga != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada);
            return result.OrderByDescending(c => c.Carga.Codigo).Select(obj => obj.Carga.Veiculo).FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> BuscarPorCodigoMobilePendentesAtualizacao(int codigoUsuarioMobile)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Motorista.Codigo == codigoUsuarioMobile && p.NotificacaoAtualizacaoCargaPendente);
            return result
                .Fetch(obj => obj.Motorista)
                .Fetch(obj => obj.Carga)
                .ToList();
        }

        public List<Dominio.Entidades.Usuario> ConsultarMotoristasCarga(int codigoCarga, string nome, string cpf, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> result = _ConsultarMotoristasCarga(codigoCarga, nome, cpf);

            IQueryable<Dominio.Entidades.Usuario> resultMotorista = result.Select(obj => obj.Motorista);

            return ObterLista(resultMotorista, parametroConsulta);
        }

        public int ContarMotoristasCarga(int codigoCarga, string nome, string cpf)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> result = _ConsultarMotoristasCarga(codigoCarga, nome, cpf);

            IQueryable<Dominio.Entidades.Usuario> resultMotorista = result.Select(obj => obj.Motorista);

            return resultMotorista.Count();
        }

        public bool ExistePorCarga(int codigoCarga)
        {
            var consultaCargaMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>()
                .Where(cargaMotorista => cargaMotorista.Carga.Codigo == codigoCarga);

            return consultaCargaMotorista.Count() > 0;
        }

        public bool ExistePorMotorista(double codigoMotorista)
        {
            var consultaCargaMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>()
                .Where(cargaMotorista => cargaMotorista.Carga.Terceiro.CPF_CNPJ == codigoMotorista || cargaMotorista.Carga.Motoristas.Any(x => x.Cliente.CPF_CNPJ == codigoMotorista));

            return consultaCargaMotorista.Any();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> _ConsultarMotoristasCarga(int codigoCarga, string nome, string cpf)
        {
            var result = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaMotorista>();

            result = from obj in result where obj.Carga.Codigo == codigoCarga select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(o => o.Motorista.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(cpf))
                result = result.Where(o => o.Motorista.CPF == cpf);

            return result;
        }

        #endregion
    }
}
