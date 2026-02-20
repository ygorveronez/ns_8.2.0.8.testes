using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.TorreControle
{
    public class ConsultaPorNotaFiscal : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal, Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.TorreControle.ConsultaPorNotaFiscal _repositorioConsultaPorNotaFiscal;

        #endregion

        #region Construtores

        public ConsultaPorNotaFiscal(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioConsultaPorNotaFiscal = new Repositorio.Embarcador.TorreControle.ConsultaPorNotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.RelatorioConsultaPorNotaFiscal> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioConsultaPorNotaFiscal.ConsultarRelatorioConsultaPorNotaFiscal(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioConsultaPorNotaFiscal.ContarConsultaRelatorioConsultaPorNotaFiscal(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/TorreControle/ConsultaPorNotaFiscal";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsultaPorNotaFiscal filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            Dominio.Entidades.Cliente cliente = filtrosPesquisa.CnpjCpfCliente > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjCpfCliente) : null;
            Dominio.Entidades.Empresa transportador = filtrosPesquisa.CodigoTransportador > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador) : null;
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;

            parametros.Add(new Parametro("DataCarregamento", filtrosPesquisa.DataCarregamentoInicial, filtrosPesquisa.DataCarregamentoFinal));
            parametros.Add(new Parametro("DataPrevisaoEntrega", filtrosPesquisa.DataPrevisaoEntregaInicial, filtrosPesquisa.DataPrevisaoEntregaFinal));
            parametros.Add(new Parametro("DataAgendamento", filtrosPesquisa.DataAgendamentoInicial, filtrosPesquisa.DataAgendamentoFinal));
            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga));
            parametros.Add(new Parametro("NumeroNota", filtrosPesquisa.NumeroNota));
            parametros.Add(new Parametro("SituacaoAgendamento", filtrosPesquisa.SituacaoAgendamento?.ObterDescricao()));
            parametros.Add(new Parametro("Cliente", cliente?.Descricao));
            parametros.Add(new Parametro("TipoOperacao", tipoOperacao?.Descricao));
            parametros.Add(new Parametro("Transportador", transportador?.Descricao));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar.Equals("ClienteDescricao"))
                return "Cliente";

            if (propriedadeOrdenarOuAgrupar.Equals("TransportadorDescricao"))
                return "Transportador";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}
