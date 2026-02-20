using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Logistica
{
    public class MotivoSelecaoMotoristaForaOrdem : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem>
    {
        #region Construtores

        public MotivoSelecaoMotoristaForaOrdem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoSelecaoMotoristaForaOrdem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaMotivoSelecaoMotoristaForaOrdem = consultaMotivoSelecaoMotoristaForaOrdem.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaMotivoSelecaoMotoristaForaOrdem = consultaMotivoSelecaoMotoristaForaOrdem.Where(o => o.Ativo);

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaMotivoSelecaoMotoristaForaOrdem = consultaMotivoSelecaoMotoristaForaOrdem.Where(o => !o.Ativo);

            return consultaMotivoSelecaoMotoristaForaOrdem;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem BuscarPorCodigo(int codigo)
        {
            var motivoSelecaoMotoristaForaOrdem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return motivoSelecaoMotoristaForaOrdem;
        }

        public List<Dominio.Entidades.Embarcador.Logistica.MotivoSelecaoMotoristaForaOrdem> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaMotivoSelecaoMotoristaForaOrdem = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaMotivoSelecaoMotoristaForaOrdem, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaMotivoSelecaoMotoristaForaOrdem = Consultar(descricao, situacaoAtivo);

            return consultaMotivoSelecaoMotoristaForaOrdem.Count();
        }

        #endregion
    }
}
