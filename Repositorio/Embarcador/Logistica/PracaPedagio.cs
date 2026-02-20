using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Logistica
{
    public class PracaPedagio : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>
    {
        #region Construtores

        public PracaPedagio(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public PracaPedagio(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> Consultar(string descricao, string concessionaria, string rodovia, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(concessionaria))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Concessionaria.Contains(concessionaria));

            if (!string.IsNullOrWhiteSpace(rodovia))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Rodovia.Contains(rodovia));

            if (!string.IsNullOrWhiteSpace(codigoIntegracao))
                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.CodigoIntegracao == codigoIntegracao);

            if (situacaoAtivo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                var ativo = situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo ? true : false;

                consultaMotivoPunicaoVeiculo = consultaMotivoPunicaoVeiculo.Where(o => o.Ativo == ativo);
            }

            return consultaMotivoPunicaoVeiculo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagio BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var motivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao && o.Ativo)
                .FirstOrDefault();

            return motivoPunicaoVeiculo;
        }

        public Dominio.Entidades.Embarcador.Logistica.PracaPedagio BuscarPorCodigo(int codigo)
        {
            var motivoPunicaoVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoPunicaoVeiculo;
        }
        public async Task<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> BuscarPorCodigoAsync(int codigo)
        {
            return await this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefaultAsync();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> BuscarTodosAtivas()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>()
                .Where(o => o.Ativo == true);
            return query.ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>> BuscarTodosAtivasAsync()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.PracaPedagio>()
                .Where(o => o.Ativo);
            return query.ToListAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Logistica.PracaPedagio> Consultar(string descricao, string concessionaria, string rodovia, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, concessionaria, concessionaria, rodovia, situacaoAtivo);

            return ObterLista(consultaMotivoPunicaoVeiculo, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(string descricao, string concessionaria, string rodovia, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoPunicaoVeiculo = Consultar(descricao, concessionaria, concessionaria, rodovia, situacaoAtivo);

            return consultaMotivoPunicaoVeiculo.Count();
        }
        public IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.PracaPedagio> ConsultarRelatorioPracaPedagio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioPracaPedagio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new Consulta.ConsultaPracaPedagio().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Logistica.PracaPedagio)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Logistica.PracaPedagio>();
        }

        public int ContarConsultaRelatorioPracaPedagio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioPracaPedagio filtrosPesquisa, List<PropriedadeAgrupamento> propriedades)
        {
            var query = new Consulta.ConsultaPracaPedagio().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion
    }
}
