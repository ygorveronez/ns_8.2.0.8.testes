using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Financeiro;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Consulta;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class PosicaoContasPagar : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaPosicaoContasPagar, Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasPagar>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.Titulo _repositorioTitulo;

        #endregion

        #region Construtores

        public PosicaoContasPagar(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PosicaoContasPagar> ConsultarRegistros(FiltroPesquisaPosicaoContasPagar filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, ParametroConsulta parametrosConsulta)
        {
            return _repositorioTitulo.RelatorioPosicaoContasPagar(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(FiltroPesquisaPosicaoContasPagar filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTitulo.ContarRelatorioPosicaoContasPagar(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/PosicaoContasPagar";
        }

        protected override List<Parametro> ObterParametros(FiltroPesquisaPosicaoContasPagar filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataPosicao", filtrosPesquisa.DataPosicao));
            if(filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy")));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy"), false));

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy")));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy"), false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataEmissaoFormatada")
                return "DataEmissao";

            if (propriedadeOrdenarOuAgrupar == "DataPagamentoFormatada")
                return "DataPagamento";

            if (propriedadeOrdenarOuAgrupar == "DataVencimentoFormatada")
                return "DataVencimento";

            if (propriedadeOrdenarOuAgrupar == "DataBaseFormatada")
                return "DataBaseBaixa";

            if (propriedadeOrdenarOuAgrupar == "CPFCNPJFornecedorFormatada")
                return "CPFCNPJFornecedor";

            if (propriedadeOrdenarOuAgrupar == "TipoFormatada")
                return "Tipo";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}
