using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class JanelaAgendamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento, Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.AgendamentoColeta _repositorioAgendamentoColeta;

        #endregion

        #region Construtores

        public JanelaAgendamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(_unitOfWork);
        }
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.JanelaAgendamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAgendamentoColeta.ConsultarRelatorioJanelaAgendamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAgendamentoColeta.ContarConsultaRelatorioJanelaAgendamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/JanelaAgendamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioJanelaAgendamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Logistica.CentroDescarregamento repositorioCentroDescarregamento = new Repositorio.Embarcador.Logistica.CentroDescarregamento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = filtrosPesquisa.CodigosFilial?.Count > 0 ? repositorioFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : null;

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>
            {
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor)?.Descricao ?? ""),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDeCarga", repositorioTipoDeCarga.BuscarPorCodigo(filtrosPesquisa.TipoDeCarga)?.Descricao ?? ""),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("JanelaExcedente", filtrosPesquisa.JanelaExcedente == OpcaoSimNaoPesquisa.Todos ? "" : filtrosPesquisa.JanelaExcedente.ObterDescricao()),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoAgendamento", string.Join(", ", filtrosPesquisa.SituacaoAgendamento.Select(o => o.ObterDescricao()).ToList())),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filiais?.Count > 0 ? string.Join(", ", filiais.Select(o => o.Descricao)) : string.Empty),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa.NumeroCarga),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", filtrosPesquisa.NumeroPedido),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Senha", filtrosPesquisa.Senha),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RaizCnpjFornecedor", filtrosPesquisa.RaizCnpjFornecedor),
                new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CentroDescarregamento", repositorioCentroDescarregamento.BuscarPorCodigo(filtrosPesquisa.CentroDescarregamento)?.Descricao ?? "")
            };

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
