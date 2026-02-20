using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Frete
{
    public class TipoContratoFrete : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete>
    {
        public TipoContratoFrete(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoContratoFrete(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Frete.TipoContratoFrete BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete>()
                .Where(obj => obj.Codigo == codigo).FirstOrDefaultAsync();
        }

        public Dominio.Entidades.Embarcador.Frete.TipoContratoFrete BuscarPorCodigoAditivo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete>();
            var result = from obj in query where obj.ContratoFreteAditivo.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool aditivos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (ativo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(o => o.Ativo == (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo));

            if (aditivos)
                result = result.Where(o => o.TipoAditivo == true);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Frete.TipoContratoFrete> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool aditivos, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, ativo, aditivos);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, bool aditivos)
        {
            var result = _Consultar(descricao, ativo, aditivos);

            return result.Count();
        }
    }
}
