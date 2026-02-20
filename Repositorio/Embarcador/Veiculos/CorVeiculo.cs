using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Veiculos
{
    public class CorVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo>
    {
        public CorVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Veiculos.CorVeiculo BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo>();

            var result = from obj in query select obj;
            result = result.Where(ent => ent.Codigo == codigo);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Veiculos.CorVeiculo ConsultarPorDescricao(string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo>();
            query = query.Where(obj => obj.Descricao.Contains(descricao));
            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = Consultar(filtrosPesquisa);

            return ObterLista(consulta, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo filtrosPesquisa)
        {
            var consulta = Consultar(filtrosPesquisa);

            return consulta.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo> Consultar(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaCorVeiculo filtrosPesquisa)
        {
            var consulta = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.CorVeiculo>();
            consulta = from obj in consulta select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consulta = consulta.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consulta = consulta.Where(obj => !obj.Ativo);          

            else if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consulta = consulta.Where(obj => obj.Ativo);

            return consulta;
        }

        #endregion
    }
}
