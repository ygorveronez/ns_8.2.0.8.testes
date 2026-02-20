using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.CheckListsUsuario
{
    public class CheckListUsuario : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario, Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario.CheckListUsuario>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Checklist.CheckListResposta _repositorioChecklist;

        #endregion

        #region Construtores

        public CheckListUsuario(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioChecklist = new Repositorio.Embarcador.Checklist.CheckListResposta(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CheckListsUsuario.CheckListUsuario> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioChecklist.ConsultarRelatorio(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioChecklist.ContarConsultaRelatorio(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CheckListsUsuario/CheckListUsuario";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CheckListsUsuario.FiltroPesquisaCheckListUsuario filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.Checklist.Checklist repTipoGROT = new Repositorio.Embarcador.Checklist.Checklist(_unitOfWork);

            Dominio.Entidades.Embarcador.Checklist.Checklist tipoGROT = filtrosPesquisa.CodigoTipoGROT > 0 ? repTipoGROT.BuscarPorCodigo(filtrosPesquisa.CodigoTipoGROT, false) : null;
            Dominio.Entidades.Usuario usuario = filtrosPesquisa.CodigoUsuario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoUsuario, false) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoGROT", tipoGROT?.Descricao ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Usuario", usuario?.Nome ?? ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPreenchimento", filtrosPesquisa.DataPreenchimentoInicial, filtrosPesquisa.DataPreenchimentoFinal, false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CnpjTransportadorFormatado")
                return "CnpjTransportador";

            if (propriedadeOrdenarOuAgrupar == "DataAtualizacaoFormatada")
                return "DataAtualizacao";

            if (propriedadeOrdenarOuAgrupar == "DataValidadeGerenciadoraRiscoFormatada")
                return "DataValidadeGerenciadoraRisco";

            if (propriedadeOrdenarOuAgrupar == "DataValidadeLiberacaoSeguradoraFormatada")
                return "DataValidadeLiberacaoSeguradora";

            if (propriedadeOrdenarOuAgrupar == "DataAquisicaoFormatada")
                return "DataAquisicao";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}