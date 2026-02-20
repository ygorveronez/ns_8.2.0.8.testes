using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Auditoria
{
    public class AuditoriaUsuario : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria, Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario>
    {
        #region Atributos

        private readonly Repositorio.Auditoria.HistoricoObjeto _repositorioHistoricoObjeto;

        #endregion

        #region Construtores

        public AuditoriaUsuario(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList, metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioHistoricoObjeto.ConsultarRelatorioAuditoriaUsuario(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioHistoricoObjeto.ContarConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Auditoria/Auditoria";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            List<Dominio.Entidades.Usuario> Usuarios = (filtrosPesquisa.CodigosUsuario != null && filtrosPesquisa.CodigosUsuario.Count > 0) ? repUsuario.BuscarUsuariosPorCodigos(filtrosPesquisa.CodigosUsuario.ToArray(), string.Empty) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial?.ToString("dd/MM/yyyy"), true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal?.ToString("dd/MM/yyyy"), true));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Usuarios", Usuarios != null && Usuarios.Count > 0 ? string.Join(", ", Usuarios.Select(o => o.Nome)) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Menus", filtrosPesquisa.Menus?.Count() > 0 ? string.Join(", ", filtrosPesquisa.Menus) : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("AcaoRealizada", filtrosPesquisa.AcaoRealizada));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}