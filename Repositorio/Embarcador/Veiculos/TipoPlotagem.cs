using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class TipoPlotagem : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem>
    {
        public TipoPlotagem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public TipoPlotagem(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Task<List<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem>> ConsultarAsync(string descricao, SituacaoAtivoPesquisa situacao, string propOrdenar, string dirOrdena, int inicio, int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.OrderBy(propOrdenar + " " + dirOrdena).Skip(inicio).Take(limite).ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(string descricao, SituacaoAtivoPesquisa situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                query = query.Where(o => o.Descricao.Contains(descricao));

            if (situacao == SituacaoAtivoPesquisa.Ativo)
                query = query.Where(o => o.Ativo);
            else if (situacao == SituacaoAtivoPesquisa.Inativo)
                query = query.Where(o => !o.Ativo);

            return query.CountAsync(CancellationToken);
        }

        public Task<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.TipoPlotagem>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefaultAsync(CancellationToken);
        }
    }
}
