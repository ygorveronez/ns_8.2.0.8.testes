using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class GrupoModeloVeicular : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>
    {
        public GrupoModeloVeicular(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public GrupoModeloVeicular(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }


        public Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>();

            var codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>> BuscarPorDescricoesAsync(List<string> descricoes, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>();

            var descricoesUnicas = descricoes.Select(d => d.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> resultado = new();

            for (int i = 0; i < descricoesUnicas.Count; i += pageSize)
            {
                var bloco = descricoesUnicas.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>()
                    .Where(obj => bloco.Contains(obj.Descricao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>();
            var result = from obj in query where obj.Descricao.Equals(descricao) select obj;
            return result.FirstOrDefault();
        }

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> _Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular>();

            var result = from obj in query select obj;

            // Filtros
            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    result = result.Where(o => o.Ativo == true);
                else
                    result = result.Where(o => o.Ativo == false);
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Cargas.GrupoModeloVeicular> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(descricao, status);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var result = _Consultar(descricao, status);

            return result.Count();
        }
    }

}