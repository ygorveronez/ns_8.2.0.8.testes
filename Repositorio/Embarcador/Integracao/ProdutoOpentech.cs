using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class ProdutoOpentech : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>
    {
        public ProdutoOpentech(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ProdutoOpentech(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return query;
        }

        public Task<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.Codigo == codigo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech BuscarPorCodigoProdutoOpentTech(int codigoProdutoOpentech)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.CodigoProdutoOpentech == codigoProdutoOpentech && o.Ativo)
                .FirstOrDefault();

            return query;
        }

        public Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech BuscarPorCodigoProdutoOpentTechETipoOperacao(int codigoProdutoOpentech, int codigoOperacao, int tipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.CodigoProdutoOpentech == codigoProdutoOpentech && o.TipoOperacao.Codigo == codigoOperacao && o.Ativo && o.TipoDeCarga.Codigo == tipoCarga);

            var result = query.FirstOrDefault();

            if (result == null)
            {
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.CodigoProdutoOpentech == codigoProdutoOpentech && o.TipoOperacao.Codigo == codigoOperacao && o.Ativo && o.TipoDeCarga == null);

                result = query.FirstOrDefault();
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> BuscarPorOperacaoEstadoApolice(int codigoOperacao, string ufDestino, List<int> codigosApolice, int codigoTipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.Ativo &&
                            o.TipoOperacao.Codigo == codigoOperacao &&
                            o.Estados.Any(e => e.Sigla == ufDestino) &&
                            codigosApolice.Contains(o.ApoliceSeguro.Codigo) &&
                            o.TipoDeCarga.Codigo == codigoTipoCarga
                            );

            var result = query.ToList();

            if (result.Count == 0)
            {
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                    .Where(o => o.Ativo &&
                                o.TipoOperacao.Codigo == codigoOperacao &&
                                o.Estados.Any(e => e.Sigla == ufDestino) &&
                                codigosApolice.Contains(o.ApoliceSeguro.Codigo) &&
                                o.TipoDeCarga == null);
                result = query.ToList();
            }

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> BuscarPorOperacaoEstadoSemApolice(int codigoOperacao, string ufDestino, int tipoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.Ativo &&
                            o.TipoOperacao.Codigo == codigoOperacao &&
                            o.Estados.Any(e => e.Sigla == ufDestino) &&
                            o.ApoliceSeguro == null &&
                            o.TipoDeCarga.Codigo == tipoCarga);
            var result = query.ToList();

            if (result == null || result.Count == 0)
            {
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
               .Where(o => o.Ativo &&
                            o.TipoOperacao.Codigo == codigoOperacao &&
                            o.Estados.Any(e => e.Sigla == ufDestino) &&
                            o.ApoliceSeguro == null &&
                            o.TipoDeCarga == null);

                result = query.ToList();
            }
            return result;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> BuscarPorOperacaoEstadoELocalidadeApolice(
            int codigoOperacao,
            string ufDestino,
            List<int> codigosApolice,
            int codigoTipoCarga,
            int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                .Where(o => o.Ativo &&
                            o.TipoOperacao.Codigo == codigoOperacao &&
                            o.Estados.Any(e => e.Sigla == ufDestino) &&
                            (
                                o.Localidades.Any(e => e.Codigo == codigoLocalidade) ||
                                !o.Localidades.Any()
                            ) &&
                            codigosApolice.Contains(o.ApoliceSeguro.Codigo) &&
                            o.TipoDeCarga.Codigo == codigoTipoCarga);

            var result = query.ToList();

            if (result.Count == 0)
            {
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                    .Where(o => o.Ativo &&
                                o.TipoOperacao.Codigo == codigoOperacao &&
                                o.Estados.Any(e => e.Sigla == ufDestino) &&
                                (
                                    o.Localidades.Any(e => e.Codigo == codigoLocalidade) ||
                                    !o.Localidades.Any()
                                ) &&
                                codigosApolice.Contains(o.ApoliceSeguro.Codigo) &&
                                o.TipoDeCarga == null);
                result = query.ToList();
            }
            return result;
        }


        public List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> BuscarPorOperacaoEstadoELocalidadeSemApolice(
            int codigoOperacao,
            string ufDestino,
            int tipoCarga,
            int codigoLocalidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
                         .Where(o => o.Ativo &&
                           o.TipoOperacao.Codigo == codigoOperacao &&
                           o.Estados.Any(e => e.Sigla == ufDestino) &&
                           (codigoLocalidade == 0 || o.Localidades.Any(e => e.Codigo == codigoLocalidade) || !o.Localidades.Any()) &&
                           o.ApoliceSeguro == null &&
                           o.TipoDeCarga.Codigo == tipoCarga);
            var result = query.ToList();

            if (result == null || result.Count == 0)
            {
                query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>()
               .Where(o => o.Ativo &&
                            o.TipoOperacao.Codigo == codigoOperacao &&
                            o.Estados.Any(e => e.Sigla == ufDestino) &&
                            (codigoLocalidade == 0 || o.Localidades.Any(e => e.Codigo == codigoLocalidade) || !o.Localidades.Any()) &&
                            o.ApoliceSeguro == null &&
                            o.TipoDeCarga == null);

                result = query.ToList();
            }
            return result;
        }

        public Task<List<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>> ConsultarAsync(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            result = result.OrderBy(parametrosConsulta.PropriedadeOrdenar + (parametrosConsulta.DirecaoOrdenar == "asc" ? " ascending" : " descending"));

            if (parametrosConsulta.InicioRegistros > 0)
                result = result.Skip(parametrosConsulta.InicioRegistros);

            if (parametrosConsulta.LimiteRegistros > 0)
                result = result.Take(parametrosConsulta.LimiteRegistros);

            return result.ToListAsync(CancellationToken);
        }

        public Task<int> ContarConsultaAsync(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.CountAsync(CancellationToken);
        }

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech> Consultar(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaProdutoOpentech filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.ProdutoOpentech>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                result = result.Where(obj => (obj.Ativo == true || obj.Ativo == false));
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo == true);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Ativo == false);

            if (filtrosPesquisa.CodigoOperacao > 0)
                result = result.Where(obj => obj.TipoOperacao.Codigo == filtrosPesquisa.CodigoOperacao);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.UfDestino))
                result = result.Where(obj => obj.Estados.Any(o => o.Sigla == filtrosPesquisa.UfDestino));

            if (filtrosPesquisa.CodigoApolice > 0)
                result = result.Where(obj => obj.ApoliceSeguro.Codigo == filtrosPesquisa.CodigoApolice);

            if (filtrosPesquisa.TipoCarga != null && filtrosPesquisa.TipoCarga.Count > 0)
                result = result.Where(obj => filtrosPesquisa.TipoCarga.Contains(obj.TipoDeCarga.Codigo));

            return result;
        }
    }
}
