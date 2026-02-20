using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.CTes
{
    public class AFRMMControl : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl, Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl>
    {
        #region Atributos

        private readonly Repositorio.ConhecimentoDeTransporteEletronico _repositorioAFRMControl;

        #endregion

        #region Construtores

        public AFRMMControl(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAFRMControl = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
        }

        public AFRMMControl(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioAFRMControl = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork, cancellationToken);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioAFRMControl.ConsultarRelatorioAFRMMControl(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioAFRMControl.ConsultarRelatorioAFRMMControlAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros);
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioAFRMControl.ContarConsultaRelatorioAFRMMControl(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/CTe/AFRMMControl";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "FilialFormatada")
                return "Filial";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}