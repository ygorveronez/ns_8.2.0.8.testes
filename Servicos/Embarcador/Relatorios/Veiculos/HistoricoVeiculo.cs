using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Veiculos
{
    public class HistoricoVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico, Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Veiculos.VeiculoHistorico _repositorioVeiculoHistorico;

        #endregion

        #region Construtores

        public HistoricoVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculoHistorico = new Repositorio.Embarcador.Veiculos.VeiculoHistorico(_unitOfWork);
        }

        public HistoricoVeiculo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, 
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioVeiculoHistorico = new Repositorio.Embarcador.Veiculos.VeiculoHistorico(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioVeiculoHistorico.ConsultarRelatorioHistoricoVeiculoAsync(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioVeiculoHistorico.ConsultarRelatorioHistoricoVeiculo(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioVeiculoHistorico.ContarConsultaRelatorioHistoricoVeiculo(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Veiculos/HistoricoVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioVeiculoHistorico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", filtrosPesquisa.CodigoVeiculo));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal));

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