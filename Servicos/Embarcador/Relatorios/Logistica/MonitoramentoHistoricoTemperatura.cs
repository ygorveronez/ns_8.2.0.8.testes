using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Carga;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Logistica;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Logistica
{
    public class MonitoramentoHistoricoTemperatura : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura, Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Logistica.Monitoramento _repositorioMonitoramento;

        #endregion

        #region Construtores

        public MonitoramentoHistoricoTemperatura(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(_unitOfWork);
        }

        #endregion
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoHistoricoTemperatura> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioMonitoramento.ConsultarRelatorioHistoricoTemperatura(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioMonitoramento.ContarConsultaRelatorioHistoricoTemperatura(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Logistica/MonitoramentoHistoricoTemperatura";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioMonitoramentoHistoricoTemperatura filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.FaixaTemperatura repFaixaTemperatura = new Repositorio.Embarcador.Cargas.FaixaTemperatura(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Filiais.Filial filial = filtrosPesquisa.CodigoFilial > 0 ? repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Embarcador.Cargas.FaixaTemperatura faixaTemperatura = filtrosPesquisa.CodigoFaixaTemperatura > 0 ? repFaixaTemperatura.BuscarPorCodigo(filtrosPesquisa.CodigoFaixaTemperatura) : null;

            parametros.Add(new Parametro("DataInicial", filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy HH:mm") : string.Empty));
            parametros.Add(new Parametro("DataFinal", filtrosPesquisa.DataFinal != DateTime.MinValue ? filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy HH:mm") : string.Empty));
            parametros.Add(new Parametro("StatusMonitoramento", filtrosPesquisa.StatusMonitoramento.ObterDescricao()));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("ForaFaixa", filtrosPesquisa.ForaFaixa.ObterDescricao()));
            parametros.Add(new Parametro("Transportador", empresa != null ? empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial : null));
            parametros.Add(new Parametro("Filial", filial?.Descricao));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Parametro("FaixaTemperatura", faixaTemperatura?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar));
            parametros.Add(new Parametro("DataCriacaoCarga", filtrosPesquisa.DataCriacaoCargaInicial, filtrosPesquisa.DataCriacaoCargaFinal));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEventoFormatada")
                return "DataEvento";
            else if (propriedadeOrdenarOuAgrupar == "DataChegadaDestinoFormatada")
                return "DataChegadaDestino";
            else if (propriedadeOrdenarOuAgrupar == "DataSaidaDestinoFormatada")
                return "DataSaidaDestino";
            else if (propriedadeOrdenarOuAgrupar == "DataCriacaoCargaFormatada")
                return "DataCriacaoCarga";

            return propriedadeOrdenarOuAgrupar;
        }

    }
}
