using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Administrativo
{
    public class LogEnvioEmail : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail, Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioEmail>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Email.LogEnvioEmail _repositorioLogEnvioEmail;

        #endregion

        #region Construtores

        public LogEnvioEmail(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioLogEnvioEmail = new Repositorio.Embarcador.Email.LogEnvioEmail(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Administrativo.LogEnvioEmail> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioLogEnvioEmail.RelatorioLogsEnvioEmail(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioLogEnvioEmail.ContarRelatorioLogsEnvioEmail(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Administrativo/LogEnvioEmail";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Administrativo.FiltroPesquisaRelatorioLogEnvioEmail filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}