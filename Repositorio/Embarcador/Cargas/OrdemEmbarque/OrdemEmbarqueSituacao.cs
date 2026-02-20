using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas.OrdemEmbarque
{
    public class OrdemEmbarqueSituacao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao>
    {

        #region Atributos privados

        private List<string> codigosIntegracaoSituacaoOrdemCancelada = new List<string> { "2", "8005" };
        private List<string> codigosIntegracaoSituacaoOrdemEmCancelamento = new List<string> { "999999" };

        #endregion

        #region Construtores

        public OrdemEmbarqueSituacao(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public OrdemEmbarqueSituacao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaOrdemEmbarqueSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao>();

            if (!string.IsNullOrWhiteSpace(descricao))
                consultaOrdemEmbarqueSituacao = consultaOrdemEmbarqueSituacao.Where(o => o.Descricao.Contains(descricao));

            if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                consultaOrdemEmbarqueSituacao = consultaOrdemEmbarqueSituacao.Where(o => o.Ativo);
            else if (situacaoAtivo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                consultaOrdemEmbarqueSituacao = consultaOrdemEmbarqueSituacao.Where(o => !o.Ativo);

            return consultaOrdemEmbarqueSituacao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao BuscarPorCodigoIntegracao(string codigoIntegracao)
        {
            var consultaOrdemEmbarqueSituacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao>()
                .Where(o => o.CodigoIntegracao == codigoIntegracao);

            return consultaOrdemEmbarqueSituacao.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao> Consultar(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOrdemEmbarqueSituacao = Consultar(descricao, situacaoAtivo);

            return ObterLista(consultaOrdemEmbarqueSituacao, parametrosConsulta);
        }

        public int ContarConsulta(string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa situacaoAtivo)
        {
            var consultaOrdemEmbarqueSituacao = Consultar(descricao, situacaoAtivo);

            return consultaOrdemEmbarqueSituacao.Count();
        }

        public List<string> GetCodigosIntegracaoSituacaoOrdemCancelada()
        {
            return codigosIntegracaoSituacaoOrdemCancelada;
        }

        public List<string> GetCodigosIntegracaoSituacaoOrdemEmCancelamento()
        {
            return codigosIntegracaoSituacaoOrdemEmCancelamento;
        }

        public bool SituacaoEhCancelada(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao ordemEmbarqueSituacao)
        {
            return CodigoIntegracaoSituacaoEhCancelada(ordemEmbarqueSituacao != null ? ordemEmbarqueSituacao.CodigoIntegracao : "");
        }

        public bool SituacaoEhEmCancelamento(Dominio.Entidades.Embarcador.Cargas.OrdemEmbarque.OrdemEmbarqueSituacao ordemEmbarqueSituacao)
        {
            return CodigoIntegracaoSituacaoEhEmCancelamento(ordemEmbarqueSituacao != null ? ordemEmbarqueSituacao.CodigoIntegracao : "");
        }

        public bool CodigoIntegracaoSituacaoEhCancelada(string codigoIntegracao = "")
        {
            return !string.IsNullOrWhiteSpace(codigoIntegracao) && codigosIntegracaoSituacaoOrdemCancelada.Contains(codigoIntegracao);
        }

        public bool CodigoIntegracaoSituacaoEhEmCancelamento(string codigoIntegracao = "")
        {
            return !string.IsNullOrWhiteSpace(codigoIntegracao) && codigosIntegracaoSituacaoOrdemEmCancelamento.Contains(codigoIntegracao);
        }

        #endregion
    }
}
