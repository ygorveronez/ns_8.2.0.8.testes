using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Estado : RepositorioBase<Dominio.Entidades.Estado>, Dominio.Interfaces.Repositorios.Estado
    {
        private CancellationToken _cancellationToken;
        public Estado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Estado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork) { this._cancellationToken = cancellationToken; }

        #region Métodos Públicos

        public List<Dominio.Entidades.Estado> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Estado BuscarPorSigla(string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query where obj.Sigla.Equals(sigla) select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Estado> BuscarPorSiglaAsync(string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query where obj.Sigla.Equals(sigla) select obj;
            return await result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Estado BuscarPorAbreviacao(string abreviacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            query = from obj in query where obj.Abreviacao.Equals(abreviacao) select obj;
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Estado> BuscarPorSiglas(List<string> siglas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query where siglas.Contains(obj.Sigla) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Estado>> BuscarPorSiglasAsync(List<string> siglas, int pageSize = 2000)
        {
            if (siglas == null || siglas.Count == 0)
                return new List<Dominio.Entidades.Estado>();

            List<string> codigosUnicos = siglas.Select(c => c.Trim().ToUpper()).Distinct().ToList();
            List<Dominio.Entidades.Estado> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>()
                    .Where(t => bloco.Contains(t.Sigla));

                var encontrados = await query.ToListAsync(CancellationToken);

                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Estado BuscarPorIBGE(int ibge)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query where obj.CodigoIBGE == ibge select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Estado BuscarPorNomeEstado(string nomeEstado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();
            var result = from obj in query where obj.Nome == nomeEstado select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Estado> Consultar(string descricao, int regiao, string propOrdena, string dirOrdena, int inicio, int limite, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Nome.Contains(descricao));

            if (regiao > 0)
                result = result.Where(o => o.RegiaoBrasil.Codigo == regiao);

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(o => o.Sigla.Equals(sigla));

            return result.OrderBy(propOrdena + " " + dirOrdena).Skip(inicio).Take(limite).ToList();
        }

        public int ContarConsulta(string descricao, int regiao, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();

            var result = from obj in query where obj.Nome.Contains(descricao) select obj;

            if (regiao > 0)
                result = result.Where(o => o.RegiaoBrasil.Codigo == regiao);

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(o => o.Sigla.Equals(sigla));

            return result.Count();
        }

        public int ContarConsultaCadastro(string descricao, int regiao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();

            var result = from obj in query where obj.Nome.Contains(descricao) select obj;

            if (regiao > 0)
                result = result.Where(o => o.RegiaoBrasil.Codigo == regiao);

            return result.Count();
        }

        public List<Dominio.Entidades.Estado> ConsultarContingencia(string nome, string sigla, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(nome, sigla);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaContingencia(string nome, string sigla)
        {
            var result = Consultar(nome, sigla);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Estado> Consultar(string nome, string sigla)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Estado>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(sigla))
                result = result.Where(obj => obj.Sigla.Equals(sigla));

            return result;
        }

        #endregion
    }
}
