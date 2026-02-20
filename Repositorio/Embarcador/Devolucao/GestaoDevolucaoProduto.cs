using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>
    {
        public GestaoDevolucaoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GestaoDevolucaoProduto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> BuscarPorGestaoDevolucao(long codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigo);
            return query.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>> BuscarPorGestoesDevolucoesAsync(List<long> codigos, int pageSize = 500)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>();

            var produtos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> resultado = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>();

            for (int i = 0; i < produtos.Count; i += pageSize)
            {
                List<long> bloco = produtos.Skip(i).Take(pageSize).ToList();

                IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> query = this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto>()
                    .Where(o => bloco.Contains(o.GestaoDevolucao.Codigo));

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoProduto> encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }
    }
}