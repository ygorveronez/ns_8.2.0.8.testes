using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class HistoricoVinculoKmReboque : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque>
    {
        public HistoricoVinculoKmReboque(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque BuscarUltimoKmVinculo(int veiculo, int reboque, TipoMovimentoKmReboque tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque>();
            var result = from obj in query where obj.Veiculo != null && obj.Reboque != null && obj.Veiculo.Codigo == veiculo && obj.Reboque.Codigo == reboque select obj;

            result = result.Where(obj => obj.TipoMovimento == tipo);

            if (result.Count() > 0)
                return result.OrderBy("DataCriacao desc, KMAtual desc").FirstOrDefault();
            else
                return null;
        }

        public bool BuscarDesvinculoDepoisVinculo(int veiculo, int reboque, DateTime data)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque>();
            var result = from obj in query where obj.Veiculo != null 
                                              && obj.Reboque != null 
                                              && obj.Veiculo.Codigo == veiculo 
                                              && obj.Reboque.Codigo == reboque 
                                              && obj.TipoMovimento == TipoMovimentoKmReboque.Desvinculo 
                                              select obj;

            if (data != DateTime.MinValue)
                result = result.Where(o => o.DataCriacao >= data);

            if (result.Count() > 0)
                return result.OrderBy("DataCriacao desc, KMAtual desc").Any();
            else
                return false;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque> Consulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque>();

            var result = from obj in query select obj;

            if (filtrosPesquisa.DataCriacaoInicial.HasValue)
                result = result.Where(obj => obj.DataCriacao.Value.Date >= filtrosPesquisa.DataCriacaoInicial.Value.Date);

            if (filtrosPesquisa.DataCriacaoFinal.HasValue)
                result = result.Where(obj => obj.DataCriacao.Value.Date <= filtrosPesquisa.DataCriacaoFinal.Value.Date);

            if (filtrosPesquisa.DataAlteracaoInicial.HasValue)
                result = result.Where(obj => obj.DataAlteracao.Value.Date >= filtrosPesquisa.DataAlteracaoInicial.Value.Date);

            if (filtrosPesquisa.DataAlteracaoFinal.HasValue)
                result = result.Where(obj => obj.DataAlteracao.Value.Date <= filtrosPesquisa.DataAlteracaoFinal.Value.Date);

            if (filtrosPesquisa.Veiculo > 0)
                result = result.Where(obj => obj.Veiculo != null && obj.Veiculo.Codigo == filtrosPesquisa.Veiculo);

            if (filtrosPesquisa.Reboque > 0)
                result = result.Where(obj => obj.Reboque != null && obj.Reboque.Codigo == filtrosPesquisa.Reboque);
            
            if (filtrosPesquisa.TipoMovimentoKmReboque > 0)
                result = result.Where(obj => obj.TipoMovimento == filtrosPesquisa.TipoMovimentoKmReboque);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque> Consulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque> result = Consulta(filtrosPesquisa);

            result
                .Fetch(o => o.Veiculo)
                .Fetch(o => o.Reboque);

            return ObterLista(result, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContaConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaHistoricoVinculoKmReboque filtrosPesquisa)
        {
            IQueryable<Dominio.Entidades.Embarcador.Veiculos.HistoricoVinculoKmReboque> result = Consulta(filtrosPesquisa);

            return result.Count();
        }
    }
}
