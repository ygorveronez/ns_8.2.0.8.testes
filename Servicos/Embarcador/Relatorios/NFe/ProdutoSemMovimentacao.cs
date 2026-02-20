using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class ProdutoSemMovimentacao : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioProdutoSemMovimentacao, Dominio.Relatorios.Embarcador.DataSource.NFe.ProdutoSemMovimentacao>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioProdutoSemMovimentacao;

        #endregion

        #region Construtores

        public ProdutoSemMovimentacao(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioProdutoSemMovimentacao = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.ProdutoSemMovimentacao> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioProdutoSemMovimentacao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioProdutoSemMovimentacao.RelatorioProdutoSemMovimentacao(filtrosPesquisa.Empresa,filtrosPesquisa.Produto,filtrosPesquisa.EstoqueDiferenteZero,filtrosPesquisa.DataInicial,filtrosPesquisa.DataFinal,filtrosPesquisa.TipoAmbiente,filtrosPesquisa.GrupoProduto, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioProdutoSemMovimentacao filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioProdutoSemMovimentacao.ContarRelatorioProdutoSemMovimentacao(filtrosPesquisa.Empresa, filtrosPesquisa.Produto, filtrosPesquisa.EstoqueDiferenteZero, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.TipoAmbiente, filtrosPesquisa.GrupoProduto);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/ProdutoSemMovimentacao";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioProdutoSemMovimentacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);

            if (filtrosPesquisa.EstoqueDiferenteZero)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstoqueDiferenteZero", "Sim", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstoqueDiferenteZero", "Não", true));

            if (filtrosPesquisa.Produto > 0)
            {
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(filtrosPesquisa.Produto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            if (filtrosPesquisa.Empresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

            if (filtrosPesquisa.GrupoProduto > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.GrupoProduto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "EstoqueFormatado")
                return "Estoque";

            if (propriedadeOrdenarOuAgrupar == "DataUltimaCompraFormatada")
                return "DataUltimaCompra";

            if (propriedadeOrdenarOuAgrupar == "DataUltimaVendaFormatada")
                return "DataUltimaVenda";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}