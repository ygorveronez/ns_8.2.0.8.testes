using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Integracao
{
    public class IntegracaoProcessamentoEDIFTP : RepositorioBase<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP>
    {
        #region Construtores

        public IntegracaoProcessamentoEDIFTP(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion


        #region Métodos Privados 

        private IQueryable<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP filtrosPesquisa)
        {
            var consultaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP>();

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.DataIntegracao.Date >= filtrosPesquisa.DataInicial.Value);

            if (filtrosPesquisa.DataFinal.HasValue)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.DataIntegracao.Date <= filtrosPesquisa.DataFinal.Value);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NomeArquivo))
                consultaIntegracoes = consultaIntegracoes.Where(o => o.NomeArquivo.Contains(filtrosPesquisa.NomeArquivo));

            if (filtrosPesquisa.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.Todas)
                consultaIntegracoes = consultaIntegracoes.Where(o => o.SituacaoIntegracaoEDIFTP == filtrosPesquisa.Situacao);

            return consultaIntegracoes;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP> BuscarPendenteIntegracao()
        {
            var listaIntegracoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP>()
                .Where(o => o.SituacaoIntegracaoEDIFTP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao)
                .OrderBy(o => o.DataIntegracao)
                .ToList();

            return listaIntegracoes;
        }

        public Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP BuscarProximaIntegracaoPendente()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP>();
            var result = from obj in query where obj.SituacaoIntegracaoEDIFTP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao || obj.SituacaoIntegracaoEDIFTP == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoProcessamentoEDIFTP.EmAndamento select obj;
            return result.OrderBy(obj => obj.DataIntegracao).FirstOrDefault();
        }


        public Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP BuscarPorCodigo(int codigo)
        {
            var integracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP>()
                .Where(o => o.Codigo == codigo)
                .FirstOrDefault();

            return integracao;
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP> Consultar(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP filtrosPesquisa, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return ObterLista(consultaIntegracoes, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }


        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP filtrosPesquisa)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return consultaIntegracoes.Count();
        }

        public List<Dominio.Entidades.Embarcador.Integracao.IntegracaoProcessamentoEDIFTP> ConsultarReenvio(Dominio.ObjetosDeValor.Embarcador.Integracao.FiltroPesquisaProcessamentoEDIFTP filtrosPesquisa)
        {
            var consultaIntegracoes = Consultar(filtrosPesquisa);

            return consultaIntegracoes.ToList();
        }

        #endregion

    }
}
