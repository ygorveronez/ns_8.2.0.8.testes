using System.Collections.Generic;
using System.Linq;
using Dominio.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Produtos
{
    public class Produto : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto, Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto>
    {
        #region Atributos

        private readonly Repositorio.Produto _repositorioProduto;

        #endregion

        #region Construtores

        public Produto(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioProduto = new Repositorio.Produto(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioProduto.ConsultarRelatorioProduto(filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioProduto.ContarConsultaRelatorioProduto(filtrosPesquisa, propriedadesAgrupamento);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Produtos/Produto";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);
            Repositorio.Embarcador.Produtos.MarcaProduto repMarcaProduto = new Repositorio.Embarcador.Produtos.MarcaProduto(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);
            Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(_unitOfWork);

            Dominio.Entidades.Produto produto = filtrosPesquisa.CodigoProduto > 0 ? repProduto.BuscarPorCodigo(filtrosPesquisa.CodigoProduto) : null;
            Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = filtrosPesquisa.CodigoGrupo > 0 ? repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoGrupo) : null;
            Dominio.Entidades.Embarcador.Produtos.MarcaProduto marcaProduto = filtrosPesquisa.CodigoMarca > 0 ? repMarcaProduto.BuscarPorCodigo(filtrosPesquisa.CodigoMarca, false) : null;
            Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = filtrosPesquisa.CodigoLocalArmazenamento > 0 ? repLocalArmazenamentoProduto.BuscarPorCodigo(filtrosPesquisa.CodigoLocalArmazenamento, false) : null;
            Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto = filtrosPesquisa.CodigoGrupoImposto > 0 ? repGrupoImposto.BuscarPorCodigo(filtrosPesquisa.CodigoGrupoImposto) : null;
            Dominio.Entidades.Empresa empresa = filtrosPesquisa.CodigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoEmpresa) : null;

            parametros.Add(new Parametro("NCM", filtrosPesquisa.CodigoNCM));
            parametros.Add(new Parametro("CEST", filtrosPesquisa.CodigoCEST));
            parametros.Add(new Parametro("CodigoBarrasEAN", filtrosPesquisa.CodigoBarrasEAN));
            parametros.Add(new Parametro("Produto", produto?.Descricao));
            parametros.Add(new Parametro("Grupo", grupoProduto?.Descricao));
            parametros.Add(new Parametro("Marca", marcaProduto?.Descricao));
            parametros.Add(new Parametro("LocalArmazenamento", localArmazenamento?.Descricao));
            parametros.Add(new Parametro("GrupoImposto", grupoImposto?.Descricao));
            parametros.Add(new Parametro("Empresa", empresa?.Descricao));
            parametros.Add(new Parametro("Status", filtrosPesquisa.Status.ObterDescricao()));
            parametros.Add(new Parametro("CategoriaProduto", filtrosPesquisa.CategoriaProduto?.ObterDescricao()));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar));


            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "UnidadeMedidaFormatada")
                return "UnidadeDeMedida";

            if (propriedadeOrdenarOuAgrupar == "CategoriaFormatada")
                return "Categoria";

            if (propriedadeOrdenarOuAgrupar == "OrigemMercadoriaFormatada")
                return "OrigemMercadoria";

            if (propriedadeOrdenarOuAgrupar == "GeneroProdutoFormatado")
                return "GeneroProduto";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion
    }
}