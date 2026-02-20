using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class Rota : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.Rota>
    {
        public Rota(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Rota(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Logistica.Rota BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;
            result = result.Where(rot => rot.Codigo == codigo);

            return result.FirstOrDefault();

        }

        public async Task<Dominio.Entidades.Embarcador.Logistica.Rota> BuscarPorCodigoAsync(int codigo, CancellationToken cancellationToken)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>()
                .Where(x => x.Codigo == codigo).FirstOrDefaultAsync(cancellationToken);
        }

        public Dominio.Entidades.Embarcador.Logistica.Rota BuscarRotaPorOrigemDestino(int codigoOrigem, int codigoDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;
            result = result.Where(rot => rot.Origem.Codigo == codigoOrigem && rot.Destino.Codigo == codigoDestino && rot.Destinatario == null && rot.Remetente == null);
            result = result.Where(rot => rot.Ativo);

            return result.FirstOrDefault();

        }

        public Task<Dominio.Entidades.Embarcador.Logistica.Rota> BuscarRotaPorOrigemDestinoAsync(int codigoOrigem, int codigoDestino)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;
            result = result.Where(rot => rot.Origem.Codigo == codigoOrigem && rot.Destino.Codigo == codigoDestino && rot.Destinatario == null && rot.Remetente == null);
            result = result.Where(rot => rot.Ativo);

            return result.FirstOrDefaultAsync(CancellationToken);

        }

        public Dominio.Entidades.Embarcador.Logistica.Rota BuscarRotaPorRemetenteDestino(double remetente, double destinatario)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;
            result = result.Where(rot => rot.Remetente.CPF_CNPJ == remetente && rot.Destinatario.CPF_CNPJ == destinatario);
            result = result.Where(rot => rot.Ativo == true);

            return result.FirstOrDefault();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Rota> Consultar(int? codigoOrigem, int? codigoDestino, double remetente, double destinatario, bool somenteNaoIntegradaSemParar, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;

            if (codigoOrigem > 0)
                result = result.Where(rot => rot.Origem.Codigo == codigoOrigem.Value);

            if (codigoDestino > 0)
                result = result.Where(rot => rot.Destino.Codigo == codigoDestino.Value);

            if (remetente > 0)
                result = result.Where(rot => rot.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(rot => rot.Destinatario.CPF_CNPJ == destinatario);

            if (somenteNaoIntegradaSemParar)
                result = result.Where(rot => rot.IntegradaSemParar == false);

            result = result.Where(rot => rot.Ativo == true);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int? codigoOrigem, int? codigoDestino, double remetente, double destinatario, bool somenteNaoIntegradaSemParar)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();

            var result = from obj in query select obj;

            if (codigoOrigem > 0)
                result = result.Where(rot => rot.Origem.Codigo == codigoOrigem.Value);

            if (codigoDestino > 0)
                result = result.Where(rot => rot.Destino.Codigo == codigoDestino.Value);

            if (remetente > 0)
                result = result.Where(rot => rot.Remetente.CPF_CNPJ == remetente);

            if (destinatario > 0)
                result = result.Where(rot => rot.Destinatario.CPF_CNPJ == destinatario);

            if (somenteNaoIntegradaSemParar)
                result = result.Where(rot => rot.IntegradaSemParar == false);

            result = result.Where(rot => rot.Ativo == true);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.Rota> BuscarRotasProcessamento()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();
            var result = from obj in query
                         where
                             obj.Remetente != null
                             && obj.Destinatario != null
                         //&& obj.Origem == null
                         //&& obj.Destino == null

                         select obj;

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Logistica.Rota> buscarTodasRotas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.Rota>();
            var result = from obj in query select obj;

            return result.ToList();

        }
    }
}
