
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class CFOP : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP, Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP>
    {
        #region Atributos

        private readonly Repositorio.CFOP _repositorioCFOP;

        #endregion

        #region Construtores

        public CFOP(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioCFOP = new Repositorio.CFOP(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.CFOP> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioCFOP.RelatorioCFOP(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioCFOP.ContarCFOP(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/CFOP";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioCFOP filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);

            Dominio.Entidades.CFOP cfop = filtrosPesquisa.CodigoCFOP > 0 ? repCFOP.BuscarPorCodigo(filtrosPesquisa.CodigoCFOP, false) : null;

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CFOP", cfop?.CFOPComExtensao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Extensao", filtrosPesquisa.Extensao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Descricao", filtrosPesquisa.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Status", filtrosPesquisa.Status == "A" ? "Ativo" : filtrosPesquisa.Status == "I" ? "Inativo" : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GerarEstoque", filtrosPesquisa.GerarEstoque ? "Sim" : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("RealizaRateioDespesaVeiculo", filtrosPesquisa.RealizaRateioDespesaVeiculo ? "Sim" : ""));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tipo", filtrosPesquisa.TipoCFOP?.ObterDescricao()));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "GeraEstoqueFormatado")
                return "GeraEstoque";

            if (propriedadeOrdenarOuAgrupar == "RealizarRateioDespesaVeiculoFormatado")
                return "RealizarRateioDespesaVeiculo";

            if (propriedadeOrdenarOuAgrupar == "CSTICMSFormatado")
                return "CSTICMS";

            if (propriedadeOrdenarOuAgrupar == "BloqueioDocumentoEntradaFormatado")
                return "BloqueioDocumentoEntrada";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}