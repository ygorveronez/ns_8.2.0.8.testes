using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamentoSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao>
    {
        #region Construtores

        public CargaJanelaDescarregamentoSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional? situacao)
        {
            var consultaJanelaDescarregamentoSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaJanelaDescarregamentoSituacao = consultaJanelaDescarregamentoSituacao.Where(o => o.Descricao.Contains(descricao));

            if (situacao.HasValue)
                consultaJanelaDescarregamentoSituacao = consultaJanelaDescarregamentoSituacao.Where(o => o.Situacao == situacao.Value);

            return consultaJanelaDescarregamentoSituacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional? situacao, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaJanelaDescarregamentoSituacao = Consultar(descricao, situacao);

            return ObterLista(consultaJanelaDescarregamentoSituacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional? situacao)
        {
            var consultaJanelaDescarregamentoSituacao = Consultar(descricao, situacao);

            return consultaJanelaDescarregamentoSituacao.Count();
        }

        public bool ExistePorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaJanelaDescarregamentoAdicional situacao, int codigoDesconsiderar)
        {
            var consultaJanelaDescarregamentoSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoSituacao>()
                .Where(o => o.Codigo != codigoDesconsiderar && o.Situacao == situacao);

            return consultaJanelaDescarregamentoSituacao.Any();
        }

        #endregion
    }
}
