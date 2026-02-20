using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CRM
{
    public class AgendaTarefas : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas, Dominio.Relatorios.Embarcador.DataSource.CRM.AgendaTarefas>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Agenda.AgendaTarefa _repositorioAgendaTarefas;

        #endregion

        #region Construtores

        public AgendaTarefas(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAgendaTarefas = new Repositorio.Embarcador.Agenda.AgendaTarefa(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CRM.AgendaTarefas> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAgendaTarefas.ConsultarRelatorioAgendaTarefas(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAgendaTarefas.ContarConsultaRelatorioAgendaTarefas(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CRM/AgendaTarefas";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CRM.FiltroPesquisaRelatorioAgendaTarefas filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CodigoCliente > 0 ? repCliente.BuscarPorCodigo(filtrosPesquisa.CodigoCliente, false) : null;
            Dominio.Entidades.Usuario usuario = filtrosPesquisa.CodigoUsuario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoUsuario) : null;

            DateTime? dataInicial = filtrosPesquisa?.DataInicial != DateTime.MinValue ? filtrosPesquisa?.DataInicial : null;
            DateTime? dataFinal = filtrosPesquisa?.DataFinal != DateTime.MinValue ? filtrosPesquisa?.DataFinal : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Observacao", filtrosPesquisa?.Observacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa?.Status?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", dataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", dataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Cliente", cliente?.Nome));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Colaborador", usuario?.Nome));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}