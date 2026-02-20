using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoNotaFiscalOrigem : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>
    {
        public GestaoDevolucaoNotaFiscalOrigem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>> BuscarPorCodigoGestaoDevolucaoAsync(long codigoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigoDevolucao);
            return await query.ToListAsync();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>> BuscarPorCodigoGestaoDevolucaoAsync(List<long> codigoOrigem, int pageSize = 500)
        {
            if (codigoOrigem == null || codigoOrigem.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();

            List<long> notas = codigoOrigem.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> resultado = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();

            for (int i = 0; i < notas.Count; i += pageSize)
            {
                List<long> bloco = notas.Skip(i).Take(pageSize).ToList();

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> encontrados = await this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>()
                    .Where(o => codigoOrigem.Contains(o.GestaoDevolucao.Codigo))
                    .Fetch(x => x.XMLNotaFiscal)
                    .ToListAsync();

                resultado.AddRange(encontrados.Distinct());
            }
            return resultado;
        }

        public async Task<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> BuscarPorChaveNotaEDevolucaoAsync(string chaveNFD, long codigoGestaoDevolucao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoNotaFiscalOrigem>();
            query = query.Where(o => o.XMLNotaFiscal.Chave == chaveNFD && o.GestaoDevolucao.Codigo == codigoGestaoDevolucao);
            return await query.FirstOrDefaultAsync();
        }
    }
}
