using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class MotivoSolicitacaoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>
    {

        public MotivoSolicitacaoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public MotivoSolicitacaoFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete> BuscarPorCodigoAsync(int codigo, CancellationToken cancellation)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return await result.FirstOrDefaultAsync(cancellation);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                bool ativo = status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                result = result.Where(o => o.Ativo == ativo);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.MotivoSolicitacaoFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }
}
