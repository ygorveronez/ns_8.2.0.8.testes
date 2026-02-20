using Dominio.ObjetosDeValor.Embarcador.Consulta;
using Dominio.ObjetosDeValor.Embarcador.RH;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.RH
{
    public class FolhaLancamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.RH.FiltroPesquisaRelatorioFolhaLancamento, Dominio.Relatorios.Embarcador.DataSource.RH.FolhaLancamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.RH.FolhaLancamento _repFolhaLancamento;

        #endregion

        #region Construtores

        public FolhaLancamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repFolhaLancamento = new Repositorio.Embarcador.RH.FolhaLancamento(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.RH.FolhaLancamento> ConsultarRegistros(FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repFolhaLancamento.ConsultarRelatorioFolhaLancamento(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repFolhaLancamento.ContarConsultaRelatorioFolhaLancamento(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/RH/FolhaLancamento";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaRelatorioFolhaLancamento filtrosPesquisa, ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);
            Repositorio.Embarcador.RH.FolhaInformacao repFolhaInformacao = new Repositorio.Embarcador.RH.FolhaInformacao(_unitOfWork);
            
            Dominio.Entidades.Usuario funcionario = filtrosPesquisa.CodigoFuncionario > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoFuncionario) : null;
            Dominio.Entidades.Embarcador.RH.FolhaInformacao folhaInformacao = filtrosPesquisa.CodigoInformacaoFolha > 0 ? repFolhaInformacao.BuscarPorCodigo(filtrosPesquisa.CodigoInformacaoFolha) : null;

            string situacaoFuncionario = "";
            if (!string.IsNullOrEmpty(filtrosPesquisa.SituacaoFuncionario))
                situacaoFuncionario = filtrosPesquisa.SituacaoFuncionario == "A" ? "Ativo" : "Inativo";

            parametros.Add(new Parametro("Funcionario", funcionario?.Descricao));
            parametros.Add(new Parametro("InformacaoFolha", folhaInformacao?.Descricao));
            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("DataCompetencia", filtrosPesquisa.DataCompetenciaInicial, filtrosPesquisa.DataCompetenciaFinal));
            parametros.Add(new Parametro("SituacaoFuncionario", situacaoFuncionario));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Descricao"))
                return propriedadeOrdenarOuAgrupar.Replace("Descricao", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
