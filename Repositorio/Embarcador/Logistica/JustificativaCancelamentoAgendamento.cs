using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class JustificativaCancelamentoAgendamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>
    {
        #region Construtores

        public JustificativaCancelamentoAgendamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento filtrosPesquisa)
        {
            var consultaJustificativaCancelamentoAgendamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>();

            if (!string.IsNullOrEmpty(filtrosPesquisa.Descricao))
                consultaJustificativaCancelamentoAgendamento = consultaJustificativaCancelamentoAgendamento.Where(obj => obj.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao != SituacaoAtivaPesquisa.Todos)
            {
                if (filtrosPesquisa.Situacao == SituacaoAtivaPesquisa.Ativa)
                    consultaJustificativaCancelamentoAgendamento = consultaJustificativaCancelamentoAgendamento.Where(obj => obj.Ativa == true);
                else
                    consultaJustificativaCancelamentoAgendamento = consultaJustificativaCancelamentoAgendamento.Where(obj => obj.Ativa == false);
            }

            return consultaJustificativaCancelamentoAgendamento;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento BuscarPorCodigo(int codigo)
        {
            var consultaJustificativaCancelamentoAgendamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>()
                .Where(obj => obj.Codigo == codigo);

            return consultaJustificativaCancelamentoAgendamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento> BuscarPorCodigos(List<int> codigos)
        {
            var consultaJustificativaCancelamentoAgendamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>()
                .Where(obj => codigos.Contains(obj.Codigo));

            return consultaJustificativaCancelamentoAgendamento.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJustificativaCancelamentoAgendamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaJustificativaCancelamentoAgendamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaJustificativaCancelamentoAgendamento filtrosPesquisa)
        {
            var consultaJustificativaCancelamentoAgendamento = Consultar(filtrosPesquisa);

            return consultaJustificativaCancelamentoAgendamento.Count();
        }

        public bool ExisteDuplicidade(Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento JustificativaCancelamentoAgendamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.JustificativaCancelamentoAgendamento>();
            query = query.Where(i => i.Codigo != JustificativaCancelamentoAgendamento.Codigo);
            query = query.Where(i => i.Ativa == JustificativaCancelamentoAgendamento.Ativa &&
                                     i.Descricao == JustificativaCancelamentoAgendamento.Descricao &&
                                     i.Observacao == JustificativaCancelamentoAgendamento.Observacao);

            return query.Any();
        }

        #endregion Métodos Públicos
    }
}
