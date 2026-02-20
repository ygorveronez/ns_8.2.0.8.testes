using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.NFe
{
    public class EstoqueProdutos : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos, Dominio.Relatorios.Embarcador.DataSource.NFe.Estoque>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.NotaFiscal.NotaFiscal _repositorioEstoqueProdutos;

        #endregion

        #region Construtores

        public EstoqueProdutos(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioEstoqueProdutos = new Repositorio.Embarcador.NotaFiscal.NotaFiscal(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.NFe.Estoque> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioEstoqueProdutos.RelatorioEstoqueProdutos(filtrosPesquisa, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioEstoqueProdutos.ContarRelatorioEstoqueProdutos(filtrosPesquisa);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/NFe/EstoqueProdutos";
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.NFe.FiltroPesquisaRelatorioEstoqueProdutos filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);

            parametros.Add(new Parametro("CodigoProduto", filtrosPesquisa.CodProduto));
            parametros.Add(new Parametro("NCM", filtrosPesquisa.CodigoNCM));

            if (filtrosPesquisa.CodigoProduto > 0)
            {
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto);
                parametros.Add(new Parametro("Produto", "(" + produto.Codigo.ToString() + ") " + produto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Produto", false));

            parametros.Add(new Parametro("Descricao", filtrosPesquisa.Descricao));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Status) && filtrosPesquisa.Status != "T")
            {
                if (filtrosPesquisa.Status == "I")
                    parametros.Add(new Parametro("Status", "Inativo", true));
                else
                    parametros.Add(new Parametro("Status", "Ativo", true));
            }
            else
                parametros.Add(new Parametro("Status", false));

            parametros.Add(new Parametro("Categoria", filtrosPesquisa.Categoria.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa);
                parametros.Add(new Parametro("Empresa", empresa.RazaoSocial, true));
            }
            else
                parametros.Add(new Parametro("Empresa", false));

            if (filtrosPesquisa.CodigoGrupoProduto > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoProduto);
                parametros.Add(new Parametro("GrupoProduto", grupoProduto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("GrupoProduto", false));

            if (filtrosPesquisa.CodigoMarca > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = repMarcaProduto.BuscarPorCodigo(filtrosPesquisa.CodigoMarca, false);
                parametros.Add(new Parametro("Marca", marcaProduto.Descricao, true));
            }
            else
                parametros.Add(new Parametro("Marca", false));

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = repLocalArmazenamentoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoLocalArmazenamento, false);
                parametros.Add(new Parametro("LocalArmazenamento", localArmazenamento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("LocalArmazenamento", false));

            if (filtrosPesquisa.DataPosicaoEstoque > DateTime.MinValue)
                parametros.Add(new Parametro("DataPosicaoEstoque", filtrosPesquisa.DataPosicaoEstoque.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Parametro("DataPosicaoEstoque", false));

            if (filtrosPesquisa.EstoqueReservado == true)
                parametros.Add(new Parametro("EstoqueReservadoFormatado", "Sim", true));
            else
                parametros.Add(new Parametro("EstoqueReservadoFormatado", false));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "DescricaoCategoria")
                return "Categoria";

            if (propriedadeOrdenarOuAgrupar == "QuantidadeEstoqueFormatado")
                return "QuantidadeEstoque";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}