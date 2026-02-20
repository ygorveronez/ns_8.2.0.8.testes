using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class TipoBaseline : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>
    {
        public TipoBaseline(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoBaseline(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> BuscarPorCodigosIntegracao(List<string> codigosIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var result = from obj in query where codigosIntegracao.Contains(obj.CodigoIntegracao) select obj;

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.TipoBaseline BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var result = from obj in query where obj.CodigoIntegracao == codigoIntegracao select obj;

            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>> BuscarPorCodigosIntegracaoAsync(List<string> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var codigosUnicos = codigos.Select(c => c.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>()
                    .Where(obj => bloco.Contains(obj.CodigoIntegracao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }

        public async Task<List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>> BuscarPorCodigosAsync(List<int> codigos, int pageSize = 2000)
        {
            if (codigos == null || codigos.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var codigosUnicos = codigos.Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> resultado = new();

            for (int i = 0; i < codigosUnicos.Count; i += pageSize)
            {
                var bloco = codigosUnicos.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>()
                    .Where(obj => bloco.Contains(obj.Codigo));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        public bool ExisteDuplicado(Dominio.Entidades.Embarcador.Bidding.TipoBaseline tipoBaseline)
        {
            var consultaTipoBaseline = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            consultaTipoBaseline = consultaTipoBaseline.Where(obj => obj.Codigo != tipoBaseline.Codigo);

            consultaTipoBaseline = consultaTipoBaseline.Where(obj => obj.Descricao.Equals(tipoBaseline.Descricao) || obj.CodigoIntegracao.Equals(tipoBaseline.CodigoIntegracao));
            //consultaTipoBaseline = consultaTipoBaseline.Where(obj => obj.CodigoIntegracao.Equals(tipoBaseline.CodigoIntegracao));

            return consultaTipoBaseline.Any();
        }

        public Dominio.Entidades.Embarcador.Bidding.TipoBaseline BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var result = from obj in query where obj.Descricao.Contains(descricao) select obj;

            return result.FirstOrDefault();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>> BuscarPorDescricoesAsync(List<string> descricoes, int pageSize = 2000)
        {
            if (descricoes == null || descricoes.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var descricoesUnicas = descricoes.Select(d => d.Trim()).Distinct().ToList();
            List<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> resultado = new();

            for (int i = 0; i < descricoesUnicas.Count; i += pageSize)
            {
                var bloco = descricoesUnicas.Skip(i).Take(pageSize).ToList();

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>()
                    .Where(obj => bloco.Contains(obj.Descricao));

                var encontrados = await query.ToListAsync(CancellationToken);
                resultado.AddRange(encontrados);
            }

            return resultado;
        }


        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.TipoBaseline> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaTipoBaseline filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.TipoBaseline>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                result = result.Where(obj => obj.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Status);
            else if (filtrosPesquisa.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Status);

            return result;
        }

        #endregion Métodos Privados
    }
}
