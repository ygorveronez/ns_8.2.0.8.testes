using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class RetornoBoleto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto, Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.BoletoRetorno _repositorioRetornoBoleto;

        #endregion

        #region Construtores

        public RetornoBoleto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioRetornoBoleto = new Repositorio.Embarcador.Financeiro.BoletoRetorno(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RetornoBoleto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioRetornoBoleto.RelatorioRetornoBoleto(filtrosPesquisa.CodigoEmpresa, filtrosPesquisa.DataInicialImportacao, filtrosPesquisa.DataFinalImportacao, filtrosPesquisa.DataInicialOcorrencia, filtrosPesquisa.DataFinalOcorrencia, filtrosPesquisa.BoletoConfiguracao, filtrosPesquisa.BoletoComando, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, true).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioRetornoBoleto.ContarRetornoBoleto(filtrosPesquisa.CodigoEmpresa, filtrosPesquisa.DataInicialImportacao, filtrosPesquisa.DataFinalImportacao, filtrosPesquisa.DataInicialOcorrencia, filtrosPesquisa.DataFinalOcorrencia, filtrosPesquisa.BoletoConfiguracao, filtrosPesquisa.BoletoComando);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/RetornoBoleto";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioRetornoBoleto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
            identificacaoCamposRPT.PrefixoCamposSum = "";
            identificacaoCamposRPT.IndiceSumGroup = "3";
            identificacaoCamposRPT.IndiceSumReport = "4";

            Repositorio.Embarcador.Financeiro.BoletoConfiguracao repBoletoConfiguracao = new Repositorio.Embarcador.Financeiro.BoletoConfiguracao(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoRetornoComando repBoletoRetornoComando = new Repositorio.Embarcador.Financeiro.BoletoRetornoComando(_unitOfWork);

            if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "De " + filtrosPesquisa.DataInicialImportacao.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalImportacao.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialImportacao > DateTime.MinValue && filtrosPesquisa.DataFinalImportacao == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "De " + filtrosPesquisa.DataInicialImportacao.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialImportacao == DateTime.MinValue && filtrosPesquisa.DataFinalImportacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "Até " + filtrosPesquisa.DataFinalImportacao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataImportacao", "Todos", true));

            if (filtrosPesquisa.DataInicialOcorrencia > DateTime.MinValue && filtrosPesquisa.DataFinalOcorrencia > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrencia", "De " + filtrosPesquisa.DataInicialOcorrencia.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalOcorrencia.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialOcorrencia > DateTime.MinValue && filtrosPesquisa.DataFinalOcorrencia == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrencia", "De " + filtrosPesquisa.DataInicialOcorrencia.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialOcorrencia == DateTime.MinValue && filtrosPesquisa.DataFinalOcorrencia > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrencia", "Até " + filtrosPesquisa.DataFinalOcorrencia.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataOcorrencia", "Todos", true));

            if (filtrosPesquisa.BoletoConfiguracao > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.BoletoConfiguracao boletoConfiguracao = repBoletoConfiguracao.BuscarPorCodigo(filtrosPesquisa.BoletoConfiguracao);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BoletoConfiguracao", boletoConfiguracao.DescricaoBanco, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BoletoConfiguracao", false));

            if (filtrosPesquisa.BoletoComando > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando boletoRetornoComando = repBoletoRetornoComando.BuscarPorCodigo(filtrosPesquisa.BoletoComando);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BoletoRetornoComando", boletoRetornoComando.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("BoletoRetornoComando", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));


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