using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Carga
{
    public class CargaEntregaPedido : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido, Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaEntregaPedido>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Cargas.Carga repositorioCarga;

        #endregion

        #region Construtores

        public CargaEntregaPedido(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.CargaEntregaPedido> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return repositorioCarga.ConsultarRelatorioCargaEntregaPedido(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return repositorioCarga.ContarConsultaRelatorioCargaEntregaPedido(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Cargas/CargaEntregaPedido";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaEntregaPedido filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            List<Dominio.Entidades.Empresa> listaEmpresa = filtrosPesquisa.CodigosTransportadora?.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosTransportadora) : new List<Dominio.Entidades.Empresa>();
            List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFilial = filtrosPesquisa.CodigosFilial?.Count > 0 ? repFilial.BuscarPorCodigos(filtrosPesquisa.CodigosFilial) : new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            parametros.Add(new Parametro("Transportador", listaEmpresa != null ? string.Join(", ", listaEmpresa.Select(o => o.NomeFantasia)) : string.Empty));
            parametros.Add(new Parametro("Filial", listaFilial != null ? string.Join(", ", listaFilial.Select(o => o.Descricao)) : string.Empty));
            parametros.Add(new Parametro("DataCriacao", filtrosPesquisa.DataInicialCriacao, filtrosPesquisa.DataFinalCriacao));
            parametros.Add(new Parametro("DataEntrega", filtrosPesquisa.DataInicialEntrega, filtrosPesquisa.DataFinalEntrega));
            parametros.Add(new Parametro("SituacaoCarga", filtrosPesquisa.SituacaoCargas.Select(o => o.ObterDescricao())));
            parametros.Add(new Parametro("CargaAgendada", filtrosPesquisa.CargaAgendada.HasValue ? filtrosPesquisa.CargaAgendada.Value ? "Sim" : "Não" : ""));
            parametros.Add(new Parametro("TipoDestino", filtrosPesquisa.TipoDestino != "T" ? filtrosPesquisa.TipoDestino == "C" ? "Coleta" : "Entrega" : "Todos"));
            parametros.Add(new Parametro("StatusMonitoramento", string.Join(", ", filtrosPesquisa.StatusMonitoramento.Select(O => O.ObterDescricao()))));
            parametros.Add(new Parametro("PrevisaoEntregaPlanejadaInicio", filtrosPesquisa.PrevisaoEntregaPlanejadaInicio, filtrosPesquisa.PrevisaoEntregaPlanejadaInicio));
            parametros.Add(new Parametro("PrevisaoEntregaPlanejadaFinal", filtrosPesquisa.PrevisaoEntregaPlanejadaFinal, filtrosPesquisa.PrevisaoEntregaPlanejadaFinal));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CNPJClienteFormatado")
                return "CNPJCliente";

            if (propriedadeOrdenarOuAgrupar == "DataInicioCarregamentoFormatada")
                return "DataInicioCarregamento";

            if (propriedadeOrdenarOuAgrupar == "DataAgendamentoFormatada")
                return "DataAgendamento";

            return propriedadeOrdenarOuAgrupar;
        }
    }
}