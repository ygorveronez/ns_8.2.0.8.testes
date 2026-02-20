using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Containers
{
    public class ControleContainer : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer, Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer.RelatorioControleContainer>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Pedidos.ColetaContainer _repositorioColetaContainer;

        #endregion

        #region Construtores

        public ControleContainer(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioColetaContainer = new Repositorio.Embarcador.Pedidos.ColetaContainer(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Containers.ControleContainer.RelatorioControleContainer> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioColetaContainer.ConsultarRelatorioControleContainer(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioColetaContainer.ContarConsultaRelatorioControleContainer(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Containers/ControleContainer";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Container.FiltroPesquisaRelatorioControleContainer filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Pedidos.ContainerTipo repContainerTipo = new Repositorio.Embarcador.Pedidos.ContainerTipo(_unitOfWork);


            Dominio.Entidades.Cliente cliente = filtrosPesquisa.LocalEsperaVazio > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.LocalEsperaVazio) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.Filial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.Filial) : null;
            Dominio.Entidades.Embarcador.Pedidos.ContainerTipo tipoContainer = filtrosPesquisa.TipoContainer > 0 ? repContainerTipo.BuscarPorCodigo(filtrosPesquisa.TipoContainer) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", filtrosPesquisa?.CodigoCargaEmbarcador)); ;
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalColeta", cliente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalAtual", cliente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("LocalEsperaVazio", cliente?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataMovimentacao", filtrosPesquisa?.DataMovimentacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPorto", filtrosPesquisa?.DataPorto));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataColeta", filtrosPesquisa?.DataInicialColeta, filtrosPesquisa?.DataFinalColeta));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoContainer", filtrosPesquisa.SituacaoContainer.HasValue ? filtrosPesquisa.SituacaoContainer.Value.ObterDescricao() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pedido", filtrosPesquisa?.NumeroPedido));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroBooking", filtrosPesquisa?.NumeroBooking));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroEXP", filtrosPesquisa?.NumeroEXP));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroContainer", filtrosPesquisa?.NumeroContainer));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DiasPosse", filtrosPesquisa?.DiasPosseInicial.ToString(), filtrosPesquisa?.DiasPosseFinal.ToString()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoContainer", tipoContainer?.Descricao));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataMovimentacaoFormatada")
                return "DataMovimentacao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}