using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Financeiro
{
    public class CentroResultado : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>
    {
        public CentroResultado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public CentroResultado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>> BuscarPorCodigosAsync(List<int> codigos)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>()
                .Where(x => codigos.Contains(x.Codigo)).ToListAsync();
        }

        public List<string> BuscarDescricaoPorCodigos(List<int> codigos)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.Select(o => o.Descricao).ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorPlano(string planoContabilidade)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.PlanoContabilidade == planoContabilidade select obj;
            return result.FirstOrDefault();
        }

        public bool ContemCentroResultado(int codigo, string plano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.Codigo != codigo && obj.Plano.Equals(plano) select obj;
            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> BuscarProximoPlanoAnalitico(string plano, int tamanhoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.Plano.StartsWith(plano) && obj.Plano.Length == tamanhoPlano select obj;
            result = result.OrderBy("Plano descending");
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> BuscarPlanoFilho(string plano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.Plano.StartsWith(plano + ".") select obj;
            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPlanoPai(string plano)
        {
            if (plano.IndexOf(".") >= 0)
            {
                int posicaoPonto = plano.LastIndexOf(".");
                string planoPai = string.Empty;
                if (posicaoPonto == 3)
                    planoPai = plano.Substring(0, plano.Length - posicaoPonto);
                else if (posicaoPonto >= 6)
                    planoPai = plano.Substring(0, posicaoPonto);
                else
                    planoPai = plano.Substring(0, plano.Length - posicaoPonto - 1);

                var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
                var result = from obj in query where obj.Plano.Equals(planoPai) select obj;
                return result.FirstOrDefault();
            }
            else
                return null;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> result = Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorTipoOperacao(Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            query = query.Where(obj => obj.Ativo && obj.TiposOperacao.Any(o => o == tipoOperacao));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorTipoOperacao(int codigoTipoOperacao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            query = query.Where(obj => obj.Ativo && obj.TiposOperacao.Any(o => o.Codigo == codigoTipoOperacao));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorPlanoEEmpresa(string planoContabilidade, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.PlanoContabilidade == planoContabilidade select obj;

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorDescricao(string descricao)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            query = query.Where(o => o.Descricao == descricao);

            return query.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Financeiro.CentroResultado BuscarPorNumeroPlano(string plano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();
            var result = from obj in query where obj.Plano == plano select obj;
            return result.FirstOrDefault();
        }

        public int ContarMotoristasPorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado);

            return query.Count();
        }

        public List<Dominio.Entidades.Usuario> BuscarMotoristasPorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado);

            return query.ToList();
        }

        public int ContarTracaoPorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado && o.TipoVeiculo.Equals("0"));

            return query.Count();
        }

        public List<Dominio.Entidades.Veiculo> BuscarTracaoPorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado && o.TipoVeiculo.Equals("0"));

            return query.ToList();
        }

        public int ContarReboquePorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado && o.TipoVeiculo.Equals("1"));

            return query.Count();
        }

        public List<Dominio.Entidades.Veiculo> BuscarReboquePorCentroResultado(int codigoCentroResultado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();

            query = query.Where(o => o.CentroResultado.Codigo == codigoCentroResultado && o.TipoVeiculo.Equals("1"));

            return query.ToList();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.CentroResultado> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCentroResultado filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.CentroResultado>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Plano))
                result = result.Where(obj => obj.Plano.StartsWith(filtrosPesquisa.Plano));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
            {
                var resultTipo = result.Where(obj => (from p in obj.TiposMovimentos where p.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento select new { p.Codigo }).Count() > 0);
                if (resultTipo.Count() > 0)
                    result = result.Where(obj => (from p in obj.TiposMovimentos where p.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento select new { p.Codigo }).Count() > 0);
            }

            if ((int)filtrosPesquisa.Tipo > 0)
                result = result.Where(obj => obj.AnaliticoSintetico == filtrosPesquisa.Tipo);

            if (filtrosPesquisa.CodigoUsuario > 0)
            {
                var queryUsuario = this.SessionNHiBernate.Query<Dominio.Entidades.Usuario>();
                var resultQueryUsuario = from obj in queryUsuario where obj.Codigo == filtrosPesquisa.CodigoUsuario select obj;

                result = result.Where(o => resultQueryUsuario.Where(a => a.CentrosResultado.Any(c => c.Codigo == o.Codigo)).Any());
            }

            return result;
        }

        #endregion
    }
}
