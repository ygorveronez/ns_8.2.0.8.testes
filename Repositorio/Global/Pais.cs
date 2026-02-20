using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio
{
    public class Pais : RepositorioBase<Dominio.Entidades.Pais>, Dominio.Interfaces.Repositorios.Pais
    {
        public Pais(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Pais(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Pais> BuscarTodos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();
            var result = from obj in query orderby obj.Nome select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Pais BuscarPorCodigo(int codigoPais, List<Dominio.Entidades.Pais> lstPais = null)
        {
            if (lstPais != null)
                return lstPais.Where(obj => obj.Codigo == codigoPais).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Pais>().Where(obj => obj.Codigo == codigoPais).FirstOrDefault();
        }

        public List<Dominio.Entidades.Pais> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();
            query = query.Where(obj => codigos.Contains(obj.Codigo));

            return query.ToList();
        }

        public async Task<List<Dominio.Entidades.Pais>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Pais>();

            var codigosUnicos = codigos.Distinct().ToList();

            List<Dominio.Entidades.Pais> resultado = new List<Dominio.Entidades.Pais>();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Pais>()
                    .Where(p => bloco.Contains(p.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);

                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public Dominio.Entidades.Pais BuscarPorSigla(string siglaPais)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();
            var result = from obj in query where obj.Sigla.Equals(siglaPais) select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Pais>> BuscarPorListaSiglaAsync(List<string> siglas, int pageSize = 2000)
        {
            if (siglas == null || siglas.Count == 0)
                return new List<Dominio.Entidades.Pais>();

            var siglasUnicas = siglas.Select(s => s.Trim().ToUpper()).Distinct().ToList();
            List<Dominio.Entidades.Pais> resultado = new List<Dominio.Entidades.Pais>();

            for (int i = 0; i < siglasUnicas.Count; i += pageSize)
            {
                var bloco = siglasUnicas.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Pais>()
                    .Where(p => bloco.Contains(p.Sigla.ToUpper()));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public Dominio.Entidades.Pais BuscarPorSiglaUF(string siglaPais, List<Dominio.Entidades.Pais> lstPais = null)
        {
            if (lstPais != null)
                return lstPais.Where(obj => obj.Abreviacao.Equals(siglaPais)).FirstOrDefault();

            return this.SessionNHiBernate.Query<Dominio.Entidades.Pais>().Where(obj => obj.Abreviacao.Equals(siglaPais)).FirstOrDefault();
        }

        public Dominio.Entidades.Pais BuscarPorNome(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();
            var result = from obj in query where obj.Nome.Equals(nome) select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Pais>> BuscarPorNomesAsync(List<string> nomes, int pageSize = 2000)
        {
            if (nomes == null || nomes.Count == 0)
                return new List<Dominio.Entidades.Pais>();

            var nomesUnicos = nomes.Select(n => n.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Pais> resultado = new List<Dominio.Entidades.Pais>();

            for (int i = 0; i < nomesUnicos.Count; i += pageSize)
            {
                var bloco = nomesUnicos.Skip(i).Take(pageSize).ToList();

                var query = SessionNHiBernate.Query<Dominio.Entidades.Pais>()
                    .Where(p => bloco.Contains(p.Nome));

                var encontrados = await query.ToListAsync(CancellationToken.None);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public List<Dominio.Entidades.Pais> Consulta(string nome, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));



            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(string nome)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Pais>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(nome))
                result = result.Where(obj => obj.Nome.Contains(nome));


            return result.Count();
        }
    }
}