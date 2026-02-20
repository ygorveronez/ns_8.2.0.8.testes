using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoReagendamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento>
    {

        #region Construtores

        public MotivoReagendamento(UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public List<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento> BuscarTodosAtivos(int limite = 0)
        {
            var consultaMotivoReagendamentoAtivo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento>();

            consultaMotivoReagendamentoAtivo = consultaMotivoReagendamentoAtivo.Where(o => o.Ativo);

            if (limite > 0)
                consultaMotivoReagendamentoAtivo = consultaMotivoReagendamentoAtivo.Take(limite);

            return consultaMotivoReagendamentoAtivo.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoReagendamento filtrosPesquisa)
        {
            var consultaMotivoReagendamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoReagendamento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMotivoReagendamento = consultaMotivoReagendamento.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoReagendamento = consultaMotivoReagendamento.Where(o => o.Ativo);
            else if (filtrosPesquisa.Ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoReagendamento = consultaMotivoReagendamento.Where(o => !o.Ativo);

            return consultaMotivoReagendamento;
        }

        #endregion

    }
}
