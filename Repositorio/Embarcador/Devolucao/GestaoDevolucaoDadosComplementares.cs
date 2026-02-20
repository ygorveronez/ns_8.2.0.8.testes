using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Devolucao
{
    public class GestaoDevolucaoDadosComplementares : RepositorioBase<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>
    {
        public GestaoDevolucaoDadosComplementares(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GestaoDevolucaoDadosComplementares(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares BuscarPorGestaoDevolucao(long codigo)
        {
            IQueryable<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>();
            query = query.Where(o => o.GestaoDevolucao.Codigo == codigo);
            return query.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>> BuscarPorGestoesDevolucoesAsync(List<long> codigos, int pageSize = 500)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>();

            List<long> dados = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares> resultado = new List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>();

            for (int i = 0; i < dados.Count; i += pageSize)
            {
                List<long> bloco = dados.Skip(i).Take(pageSize).ToList();

                List<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares> encontrados = await this.SessionNHiBernate
                    .Query<Dominio.Entidades.Embarcador.Devolucao.GestaoDevolucaoDadosComplementares>()
                    .Where(o => bloco.Contains(o.GestaoDevolucao.Codigo))
                    .Fetch(x => x.PeriodoDescarregamento)
                    .Distinct()
                    .ToListAsync(CancellationToken);

                resultado.AddRange(encontrados);
            }

            return resultado;
        }
    }
}