using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Containers
{
    public class HistoricoMovimentacaoContainers : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer, Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.ColetaContainer _repositorioColetaContainer;

        #endregion

        #region Construtores

        public HistoricoMovimentacaoContainers(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Containers.HistoricoMovimentacaoContainers.HistoricoMovimentacaoContainers> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioColetaContainer.ConsultarRelatorioHistoricoMovimentacaoContainer(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioColetaContainer.ContarConsultaRelatorioHistoricoMovimentacaoContainer(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Containers/HistoricoMovimentacaoContainers";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioHistoricoMovimentacaoContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repositorioTipoContainer = new Repositorio.Embarcador.Pedidos.ContainerTipo(_unitOfWork);
            Dominio.Entidades.Cliente localAtual = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.LocalAtual);
            Dominio.Entidades.Cliente localColeta = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.LocalColeta);
            Dominio.Entidades.Cliente localEsperaVazio = repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.LocalEsperaVazio);
            Dominio.Entidades.Embarcador.Filiais.Filial filial = repositorioFilial.BuscarPorCodigo(filtrosPesquisa.Filial);
            Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = repositorioTipoContainer.BuscarPorCodigo(filtrosPesquisa.TipoContainer);

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa?.Carga));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Container", filtrosPesquisa?.Container));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoContainer", filtrosPesquisa?.SituacaoContainer?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialColeta", filtrosPesquisa?.DataInicialColeta > DateTime.MinValue ? filtrosPesquisa?.DataInicialColeta.ToString("dd/MM/yyyy") : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalColeta", filtrosPesquisa?.DataFinalColeta > DateTime.MinValue ? filtrosPesquisa?.DataFinalColeta.ToString("dd/MM/yyyy") : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalAtual", localAtual?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalColeta", localColeta?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalEsperaVazio", localEsperaVazio?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasPosseInicial", filtrosPesquisa?.DiasPosseInicial > 0 ? filtrosPesquisa?.DiasPosseInicial : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasPosseFinal", filtrosPesquisa?.DiasPosseFinal > 0 ? filtrosPesquisa?.DiasPosseFinal : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPorto", filtrosPesquisa?.DataPorto > DateTime.MinValue ? filtrosPesquisa?.DataPorto.ToString("dd/MM/yyyy") : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimentacao", filtrosPesquisa?.DataMovimentacao > DateTime.MinValue ? filtrosPesquisa?.DataMovimentacao.ToString("dd/MM/yyyy") : null));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa?.NumeroBooking));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedido", filtrosPesquisa?.NumeroPedido));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEXP", filtrosPesquisa?.NumeroEXP));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContainer", tipoContainer?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            propriedadeOrdenarOuAgrupar.Replace("Formatado", "");
            propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}