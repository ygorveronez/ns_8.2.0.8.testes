using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class HistoricoEstoque : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque, Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioHistoricoEstoque;

        #endregion

        #region Construtores

        public HistoricoEstoque(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioHistoricoEstoque = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.HistoricoEstoque> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioHistoricoEstoque.RelatorioHistoricoEstoque(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioHistoricoEstoque.ContarRelatorioHistoricoEstoque(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/HistoricoEstoque";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioHistoricoEstoque filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);

            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;
            Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = filtrosPesquisa.CodigoGrupoProduto > 0 ? repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoProduto) : null;
            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = filtrosPesquisa.CodigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoLocalArmazenamento) : null;

            IdentificacaoCamposRPT identificacaoCamposRPT = new IdentificacaoCamposRPT();
            identificacaoCamposRPT.PrefixoCamposSum = "";
            identificacaoCamposRPT.IndiceSumGroup = "3";
            identificacaoCamposRPT.IndiceSumReport = "4";

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto);
                parametros.Add(new Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Produto", false));

            parametros.Add(new Parametro("DataMovimento", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status))
            {
                if (filtrosPesquisa.Status == "I")
                    parametros.Add(new Parametro("Status", "Inativo", true));
                else if (filtrosPesquisa.Status == "A")
                    parametros.Add(new Parametro("Status", "Ativo", true));
                else if (filtrosPesquisa.Status == "T")
                    parametros.Add(new Parametro("Status", "Todos", true));
            }
            else
                parametros.Add(new Parametro("Status", false));

            parametros.Add(new Parametro("Categoria", filtrosPesquisa.Categoria.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));
            parametros.Add(new Parametro("Empresa", empresa?.RazaoSocial));
            parametros.Add(new Parametro("GrupoProduto", grupoProduto?.Descricao));
            parametros.Add(new Parametro("LocalArmazenamento", localArmazenamento?.Descricao));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DataFormatada")
                return "Data";

            if (propriedadeOrdenarOuAgrupar == "QuantidadeFormatado")
                return "Quantidade";

            if (propriedadeOrdenarOuAgrupar == "QuantidadeEstoqueFormatado")
                return "QuantidadeEstoque";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}