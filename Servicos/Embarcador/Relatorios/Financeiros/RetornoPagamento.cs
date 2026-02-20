using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class RetornoPagamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento, Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno _repositorioRetornoPagamento;

        #endregion

        #region Construtores

        public RetornoPagamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRetornoPagamento = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoRetorno(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoPagamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRetornoPagamento.RelatorioRetornoPagamento(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRetornoPagamento.ContarRetornoPagamento(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/RetornoPagamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoPagamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Banco repositorioBanco = new Repositorio.Banco(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repositorioBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
            identificacaoCamposRPT.PrefixoCamposSum = "";
            identificacaoCamposRPT.IndiceSumGroup = "3";
            identificacaoCamposRPT.IndiceSumReport = "4";

            List<Dominio.Entidades.Banco> bancosPessoa = filtrosPesquisa.CodigosBancoPessoa.Count > 0 ? repositorioBanco.BuscarPorCodigos(filtrosPesquisa.CodigosBancoPessoa) : new List<Dominio.Entidades.Banco>();
            Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = filtrosPesquisa.CodigoConfiguracaoBoleto > 0 ? repositorioBoletoConfiguracao.BuscarPorCodigo(filtrosPesquisa.CodigoConfiguracaoBoleto) : null;


            if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "De " + filtrosPesquisa.DataInicialImportacao.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalImportacao.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "De " + filtrosPesquisa.DataInicialImportacao.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialImportacao == DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "Até " + filtrosPesquisa.DataFinalImportacao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "Todos", true));

            if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "De " + filtrosPesquisa.DataInicialPagamento.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalPagamento.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialPagamento > DateTime.MinValue && filtrosPesquisa.DataFinalPagamento == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "De " + filtrosPesquisa.DataInicialPagamento.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialPagamento == DateTime.MinValue && filtrosPesquisa.DataFinalPagamento > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "Até " + filtrosPesquisa.DataFinalPagamento.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPagamento", "Todos", true));

            if (filtrosPesquisa.CodigoTitulo > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", filtrosPesquisa.CodigoTitulo.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Titulo", false));

            if (filtrosPesquisa.CnpjFornecedor > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjFornecedor).Nome, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BancoPessoa", bancosPessoa.Select(o => o.Descricao)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BoletoConfiguracao", boletoConfiguracao?.Descricao ?? string.Empty));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoAgendamento")
                return "Agendamento";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}