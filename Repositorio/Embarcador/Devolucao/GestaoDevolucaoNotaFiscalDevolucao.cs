using System.Collections.Generic;
using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoNotaFiscalDevolucao : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>
    {
        public GestaoDevolucaoNotaFiscalDevolucao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>> BuscarPorCodigoGestaoDevolucaoAsync(long codigoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigoDevolucao);
            return await query.ToListAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>> BuscarPorCodigoGestaoDevolucaoAsync(List<long> codigoDevolucao, int pageSize = 500)
        {
            if (codigoDevolucao == null || codigoDevolucao.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();

            List<long> notas = codigoDevolucao.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> resultado = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();

            for (int i = 0; i < notas.Count; i += pageSize)
            {
                List<long> bloco = notas.Skip(i).Take(pageSize).ToList();

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> encontrados = await this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>()
                    .Where(o => codigoDevolucao.Contains(o.GestaoDevolucao.Codigo))
                    .Fetch(x => x.XMLNotaFiscal)
                    .ThenFetch(c => c.Canhoto)
                    .ToListAsync();

                resultado.AddRange(encontrados.Distinct());
            }
            return resultado;
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> BuscarPorChaveNotaEDevolucaoAsync(string chaveNFD, long codigoGestaoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalDevolucao>();
            query = query.Where(o => o.XMLNotaFiscal.Chave == chaveNFD && o.GestaoDevolucao.Codigo == codigoGestaoDevolucao);
            return await query.FirstOrDefaultAsync();
        }
    }
}
