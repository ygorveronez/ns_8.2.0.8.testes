using NHibernate.Linq;
using Repositorio.Embarcador.Veiculos.Consulta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Veiculos
{
    public class Equipamento : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.Equipamento>
    {
        public Equipamento(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Equipamento(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento BuscarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
            var result = from obj in query where obj.Ativo && obj.Descricao.Contains(descricao) select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.Equipamento BuscarPorDescricaoParaAbastecimento(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
            var result = from obj in query where obj.Ativo && obj.EquipamentoAceitaAbastecimento && obj.Descricao.Contains(descricao) select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> BuscarPorCodigo(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> BuscarPorCodigo(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaEquipamento = Consultar(filtrosPesquisa);

            consultaEquipamento = consultaEquipamento
                .Fetch(o => o.MarcaEquipamento)
                .Fetch(o => o.ModeloEquipamento)
                .Fetch(o => o.CentroResultado);

            return ObterLista(consultaEquipamento, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.Equipamento> ConsultarEquipamentosPendentesIntegracao(string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            query = query.Where(obj => obj.Integrado != true);

            return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultarEquipamentosPendentesIntegracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            query = query.Where(obj => obj.Integrado != true);

            return query.Count();
        }

        #endregion

        #region Métodos Públicos - Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento> ConsultarRelatorioEquipamento(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaEquipamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento>();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento>> ConsultarRelatorioEquipamentoAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaEquipamento().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento)));

            return await query.SetTimeout(600).ListAsync<Dominio.Relatorios.Embarcador.DataSource.Veiculos.Equipamento>();
        }

        public int ContarConsultaRelatorioEquipamento(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioEquipamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaEquipamento().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.Equipamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaEquipamento filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.Equipamento>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.Codigo > 0)
                result = result.Where(obj => obj.Codigo == filtrosPesquisa.Codigo);

            if (int.TryParse(filtrosPesquisa.Descricao, out int codigo))
                result = result.Where(obj => obj.Codigo == codigo || obj.Descricao.Contains(filtrosPesquisa.Descricao));
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Numero))
                result = result.Where(obj => obj.Numero.Contains(filtrosPesquisa.Numero));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Chassi))
                result = result.Where(obj => obj.Chassi.Contains(filtrosPesquisa.Chassi));

            if (filtrosPesquisa.CodigoMarcaEquipamento > 0)
                result = result.Where(obj => obj.MarcaEquipamento.Codigo == filtrosPesquisa.CodigoMarcaEquipamento);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Ativo);

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => !obj.Ativo);

            if (filtrosPesquisa.CodigoVeiculo > 0)
            {
                var queryVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Veiculo>();
                queryVeiculo = queryVeiculo.Where(obj => obj.Codigo == filtrosPesquisa.CodigoVeiculo);

                result = result.Where(obj => queryVeiculo.Any(e => e.Equipamentos.Any(o => o.Codigo == obj.Codigo)));
            }

            return result;
        }

        #endregion
    }
}
