using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class RotaFreteClassificacao : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao>
    {
        #region Construtores

        public RotaFreteClassificacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao> Consultar(string descricao, RotaFreteClasse? classe)
        {
            var consultaRotaFreteClassificacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaRotaFreteClassificacao = consultaRotaFreteClassificacao.Where(o => o.Descricao.Contains(descricao));

            if (classe.HasValue)
                consultaRotaFreteClassificacao = consultaRotaFreteClassificacao.Where(o => o.Classe == classe.Value);

            return consultaRotaFreteClassificacao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.RotaFreteClassificacao> Consultar(string descricao, RotaFreteClasse? classe, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaRotaFreteClassificacao = Consultar(descricao, classe);

            return ObterLista(consultaRotaFreteClassificacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, RotaFreteClasse? classe)
        {
            var consultaRotaFreteClassificacao = Consultar(descricao, classe);

            return consultaRotaFreteClassificacao.Count();
        }

        #endregion
    }
}
