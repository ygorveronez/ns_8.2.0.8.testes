using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class LiberacaoPagamentoProvedor : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor, Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.PagamentoProvedor _repositorioPagamentoProvedor;

        #endregion

        #region Construtores

        public LiberacaoPagamentoProvedor(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioPagamentoProvedor = new Repositorio.Embarcador.Financeiro.PagamentoProvedor(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioLiberacaoPagamentoProvedor> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioPagamentoProvedor.ConsultarRelatorioLiberacaoPagamentoProvedor(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioPagamentoProvedor.ContarConsultaRelatorioLiberacaoPagamentoProvedor(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/LiberacaoPagamentoProvedor";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioLiberacaoPagamentoProvedor filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

  
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = filtrosPesquisa.CodigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao) : null;
            Dominio.Entidades.Cliente provedor = filtrosPesquisa.CodigoProvedor > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoProvedor) : null;
            Dominio.Entidades.Cliente tomador = filtrosPesquisa.CodigoTomador > 0d ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CodigoTomador) : null;
            Dominio.Entidades.Empresa filialEmissora = filtrosPesquisa.CodigoFilialEmissora > 0d ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoFilialEmissora) : null;


            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCargaInicial", filtrosPesquisa.DataCargaInicial != System.DateTime.MinValue ? filtrosPesquisa.DataCargaInicial.ToString("d") : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCargaFinal", filtrosPesquisa.DataCargaFinal != System.DateTime.MinValue ? filtrosPesquisa.DataCargaFinal.ToString("d") : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroCarga", string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroCarga) ? filtrosPesquisa.NumeroCarga : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOS", string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS) ? filtrosPesquisa.NumeroOS : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOperacao", tipoOperacao != null ? tipoOperacao.Descricao : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("FilialEmissora", filialEmissora != null ? filialEmissora.RazaoSocial : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Provedor", provedor != null ? provedor.Nome : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomador != null ? tomador.Nome : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoLiberacaoPagamentoProvedor", filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor != SituacaoLiberacaoPagamentoProvedor.Todos ? filtrosPesquisa.SituacaoLiberacaoPagamentoProvedor.ObterDescricao() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoDocumento", filtrosPesquisa.TipoDocumentoProvedor != TipoDocumentoProvedor.Nenhum ? filtrosPesquisa.TipoDocumentoProvedor.ToString() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("IndicacaoLiberacaoOK", filtrosPesquisa.IndicacaoLiberacaoOK != Dominio.Enumeradores.OpcaoSimNaoPesquisa.Todos ? filtrosPesquisa.IndicacaoLiberacaoOK.ToString() : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EtapaLiberacaoPagamentoProvedor", filtrosPesquisa.EtapaLiberacaoPagamentoProvedor != EtapaLiberacaoPagamentoProvedor.Todos ? filtrosPesquisa.EtapaLiberacaoPagamentoProvedor.ObterDescricao() : string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataCargaInicial")
                return "DataCargaInicial";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}