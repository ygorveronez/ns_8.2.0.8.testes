using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Pessoas
{
    public class PerfilAcesso : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso, Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso>
    {
        #region Atributos

        private readonly Usuario _servicoUsuario;
        private List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> reportResult;

        #endregion

        #region Construtores

        public PerfilAcesso(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _servicoUsuario = new Usuario(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override List<Dominio.Relatorios.Embarcador.DataSource.Usuarios.PerfilAcesso> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            if (reportResult?.Count > 0)
                return reportResult;

            reportResult = _servicoUsuario.RelatorioPerfilAcesso(propriedadesAgrupamento, filtrosPesquisa, _unitOfWork);

            return _servicoUsuario.AgrupaRelatorioPerfilAcesso(reportResult, parametrosConsulta);
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            if (reportResult?.Count > 0)
                return reportResult.Count();

            reportResult = _servicoUsuario.RelatorioPerfilAcesso(propriedadesAgrupamento, filtrosPesquisa, _unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta();

            reportResult = _servicoUsuario.AgrupaRelatorioPerfilAcesso(reportResult, parametrosConsulta);

            return reportResult.Count();
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Pessoas/PerfilAcesso";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Pessoas.FiltroPesquisaRelatorioPerfilAcesso filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Embarcador.Usuarios.PerfilAcesso repPerfilAcesso = new Repositorio.Embarcador.Usuarios.PerfilAcesso(_unitOfWork);

            Dominio.Entidades.Embarcador.Usuarios.PerfilAcesso perfil = filtrosPesquisa.CodigoPerfil > 0 ? repPerfilAcesso.BuscarPorCodigo(filtrosPesquisa.CodigoPerfil) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ativo", filtrosPesquisa.Ativo.HasValue ? filtrosPesquisa.Ativo.Value ? "Ativo" : "Inativo" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PerfilAcesso", perfil?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
