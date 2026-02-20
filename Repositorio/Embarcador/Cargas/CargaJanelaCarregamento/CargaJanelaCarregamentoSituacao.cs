using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaCarregamentoSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao>
    {
        #region Construtores

        public CargaJanelaCarregamentoSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional? situacao)
        {
            var consultaJanelaCarregamentoSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaJanelaCarregamentoSituacao = consultaJanelaCarregamentoSituacao.Where(o => o.Descricao.Contains(descricao));

            if (situacao.HasValue)
                consultaJanelaCarregamentoSituacao = consultaJanelaCarregamentoSituacao.Where(o => o.Situacao == situacao.Value);

            return consultaJanelaCarregamentoSituacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJanelaCarregamentoSituacao = Consultar(descricao, situacao);

            return ObterLista(consultaJanelaCarregamentoSituacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional? situacao)
        {
            var consultaJanelaCarregamentoSituacao = Consultar(descricao, situacao);

            return consultaJanelaCarregamentoSituacao.Count();
        }

        public bool ExistePorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaCarregamentoAdicional situacao, int codigoDesconsiderar)
        {
            var consultaJanelaCarregamentoSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaCarregamentoSituacao>()
                .Where(o => o.Codigo != codigoDesconsiderar && o.Situacao == situacao);

            return consultaJanelaCarregamentoSituacao.Any();
        }

        #endregion
    }
}
