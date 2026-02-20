using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Escrituracao
{
    public class IntegracaoLotePagamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento, Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Escrituracao.Pagamento _repositorioEscrituracaoPagamento;
        private readonly int _limitePergustasRespostas = 60;

        #endregion

        #region Construtores

        public IntegracaoLotePagamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioEscrituracaoPagamento = new Repositorio.Embarcador.Escrituracao.Pagamento(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Escrituracao.IntegracaoLotePagamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioEscrituracaoPagamento.ConsultarRelatorioIntegracaoLotePagamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioEscrituracaoPagamento.ContarConsultaRelatorioIntegracaoLotePagamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Escrituracao/IntegracaoLotePagamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Escrituracao.FiltroPesquisaIntegracaoLotePagamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.Carga cargas = filtrosPesquisa.CodigoCarga > 0 ? repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga) : null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = filtrosPesquisa.CodigoCTe > 0 ? repositorioCTe.BuscarPorCodigo(filtrosPesquisa.CodigoCTe) : null;
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cteNumPagamento = filtrosPesquisa.NumeroPagamento > 0 ? repositorioCTe.BuscarPorCodigo(filtrosPesquisa.NumeroPagamento) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPagamento", cteNumPagamento?.Numero ?? 0));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", cargas?.CodigoCargaEmbarcador ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", cte?.Descricao ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoIntegracao", filtrosPesquisa.SituacaoIntegracao?.ObterDescricao() ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoPagamento", filtrosPesquisa.SituacaoPagamento?.ObterDescricao() ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCarga?.ObterDescricao() ?? string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado", filtrosPesquisa.ExibirUltimoRegistroQuandoExistirProtocoloCTeDuplicado ? "Sim" : ""));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
