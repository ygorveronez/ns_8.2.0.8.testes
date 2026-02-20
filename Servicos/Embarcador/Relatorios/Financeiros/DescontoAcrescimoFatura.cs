using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class DescontoAcrescimoFatura : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura, Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.Titulo _repositorioDescontoAcrescimoFatura;

        #endregion

        #region Construtores

        public DescontoAcrescimoFatura(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioDescontoAcrescimoFatura = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DescontoAcrescimoFatura> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioDescontoAcrescimoFatura.RelatorioDescontoAcrescimoFatura(filtrosPesquisa.DataInicialQuitacao, filtrosPesquisa.DataFinalQuitacao, filtrosPesquisa.GruposPessoas, filtrosPesquisa.Pessoa, filtrosPesquisa.ConhecimentoDeTransporte, filtrosPesquisa.Fatura, filtrosPesquisa.GrupoPessoa, filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioDescontoAcrescimoFatura.ContarDescontoAcrescimoFatura(filtrosPesquisa.DataInicialQuitacao, filtrosPesquisa.DataFinalQuitacao, filtrosPesquisa.GruposPessoas, filtrosPesquisa.Pessoa, filtrosPesquisa.ConhecimentoDeTransporte, filtrosPesquisa.Fatura, filtrosPesquisa.GrupoPessoa, filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/DescontoAcrescimoFatura";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDescontoAcrescimoFatura filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(_unitOfWork);

            Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();

            identificacaoCamposRPT.PrefixoCamposSum = "";
            identificacaoCamposRPT.IndiceSumGroup = "3";
            identificacaoCamposRPT.IndiceSumReport = "4";

            if (filtrosPesquisa.ConhecimentoDeTransporte > 0)
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(filtrosPesquisa.ConhecimentoDeTransporte);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", "(" + cte.Numero + ") " + cte.Chave, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", false));

            if (filtrosPesquisa.Fatura > 0)
            {
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
                Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(filtrosPesquisa.Fatura);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", "(" + fatura.Codigo + ") " + fatura.Numero, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

            if (filtrosPesquisa.GruposPessoas != null && filtrosPesquisa.GruposPessoas.Count > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", "Multiplos grupos", true));
            else if (filtrosPesquisa.GrupoPessoa > 0)
            {
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(filtrosPesquisa.GrupoPessoa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

            if (filtrosPesquisa.Pessoa > 0)
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Pessoa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

            if (filtrosPesquisa.DataInicialEmissao > DateTime.MinValue && filtrosPesquisa.DataFinalEmissao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + filtrosPesquisa.DataInicialEmissao?.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalEmissao?.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialEmissao > DateTime.MinValue && filtrosPesquisa.DataFinalEmissao == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + filtrosPesquisa.DataInicialEmissao?.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialEmissao == DateTime.MinValue && filtrosPesquisa.DataFinalEmissao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Até " + filtrosPesquisa.DataFinalEmissao?.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Todos", true));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.DataInicialQuitacao > DateTime.MinValue && filtrosPesquisa.DataFinalQuitacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + filtrosPesquisa.DataInicialQuitacao?.ToString("dd/MM/yyyy") + " até " + filtrosPesquisa.DataFinalQuitacao?.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialQuitacao > DateTime.MinValue && filtrosPesquisa.DataFinalQuitacao == DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + filtrosPesquisa.DataInicialQuitacao?.ToString("dd/MM/yyyy"), true));
            else if (filtrosPesquisa.DataInicialQuitacao == DateTime.MinValue && filtrosPesquisa.DataFinalQuitacao > DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Até " + filtrosPesquisa.DataFinalQuitacao?.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Todos", true));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "NumeroFatura")
                return "NumeroFatura";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}