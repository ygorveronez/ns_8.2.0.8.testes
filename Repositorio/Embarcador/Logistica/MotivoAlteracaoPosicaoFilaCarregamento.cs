using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class MotivoAlteracaoPosicaoFilaCarregamento : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento>
    {
        #region Construtores

        public MotivoAlteracaoPosicaoFilaCarregamento(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoAlteracaoPosicaoFilaCarregamento filtrosPesquisa)
        {
            var consultaMotivoAlteracaoPosicaoFilaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaMotivoAlteracaoPosicaoFilaCarregamento = consultaMotivoAlteracaoPosicaoFilaCarregamento.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoAlteracaoPosicaoFilaCarregamento = consultaMotivoAlteracaoPosicaoFilaCarregamento.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoAlteracaoPosicaoFilaCarregamento = consultaMotivoAlteracaoPosicaoFilaCarregamento.Where(o => !o.Ativo);

            return consultaMotivoAlteracaoPosicaoFilaCarregamento;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento BuscarPorCodigo(int codigo)
        {
            var consultaMotivoAlteracaoPosicaoFilaCarregamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento>()
                .Where(o => o.Codigo == codigo);

            return consultaMotivoAlteracaoPosicaoFilaCarregamento.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoAlteracaoPosicaoFilaCarregamento> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoAlteracaoPosicaoFilaCarregamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoAlteracaoPosicaoFilaCarregamento = Consultar(filtrosPesquisa);

            return ObterLista(consultaMotivoAlteracaoPosicaoFilaCarregamento, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMotivoAlteracaoPosicaoFilaCarregamento filtrosPesquisa)
        {
            var consultaMotivoAlteracaoPosicaoFilaCarregamento = Consultar(filtrosPesquisa);

            return consultaMotivoAlteracaoPosicaoFilaCarregamento.Count();
        }

        #endregion
    }
}
