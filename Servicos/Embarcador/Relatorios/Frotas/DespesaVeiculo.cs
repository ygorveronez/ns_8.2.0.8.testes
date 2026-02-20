using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Relatorios.Frotas
{
    public class DespesaVeiculo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioDespesaVeiculo, Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS _repositorioFrotas;

        #endregion

        #region Construtores

        public DespesaVeiculo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioFrotas = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
        }

        public DespesaVeiculo(
            Repositorio.UnitOfWork unitOfWork,
            AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            CancellationToken cancellationToken) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioFrotas = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork, cancellationToken);
        }

        #endregion

        #region métodos assíncronos
        public async Task<IList<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo>> ConsultarRegistrosAsync(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return await _repositorioFrotas.RelatorioDespesaVeiculoAsync(filtrosPesquisa.Empresa, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.Veiculo, filtrosPesquisa.Produto, filtrosPesquisa.GrupoProduto, filtrosPesquisa.GrupoProdutoPai, filtrosPesquisa.Fornecedor, filtrosPesquisa.NaturezaOperacao, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false);
        }
        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Frota.DespesaVeiculo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioFrotas.RelatorioDespesaVeiculo(filtrosPesquisa.Empresa, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.Veiculo, filtrosPesquisa.Produto, filtrosPesquisa.GrupoProduto, filtrosPesquisa.GrupoProdutoPai, filtrosPesquisa.Fornecedor, filtrosPesquisa.NaturezaOperacao, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros, false).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioDespesaVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioFrotas.ContarRelatorioDespesaVeiculo(filtrosPesquisa.Empresa, filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal, filtrosPesquisa.Veiculo, filtrosPesquisa.Produto, filtrosPesquisa.GrupoProduto, filtrosPesquisa.GrupoProdutoPai, filtrosPesquisa.Fornecedor, filtrosPesquisa.NaturezaOperacao);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Frotas/DespesaVeiculo";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaRelatorioDespesaVeiculo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repFornecedor = new Repositorio.Cliente(_unitOfWork);
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Repositorio.Embarcador.Produtos.GrupoProdutoTMS repGrupoProduto = new Repositorio.Embarcador.Produtos.GrupoProdutoTMS(_unitOfWork);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue || filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                string data = "";
                data += filtrosPesquisa.DataInicial != DateTime.MinValue ? filtrosPesquisa.DataInicial.ToString("dd/MM/yyyy") + " " : "";
                data += filtrosPesquisa.DataFinal != DateTime.MinValue ? "até " + filtrosPesquisa.DataFinal.ToString("dd/MM/yyyy") : "";
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", data, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Data", false));

            if (filtrosPesquisa.Veiculo > 0)
            {
                Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(filtrosPesquisa.Veiculo);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo.Placa, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", false));

            if (filtrosPesquisa.Produto > 0)
            {
                Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigo(filtrosPesquisa.Produto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", produto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Produto", false));

            if (filtrosPesquisa.GrupoProduto > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.GrupoProduto);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", grupoProduto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProduto", false));

            if (filtrosPesquisa.GrupoProdutoPai > 0)
            {
                Dominio.Entidades.Embarcador.Produtos.GrupoProdutoTMS grupoProduto = repGrupoProduto.BuscarPorCodigo(filtrosPesquisa.GrupoProdutoPai);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProdutoPai", grupoProduto.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoProdutoPai", false));

            if (filtrosPesquisa.Fornecedor > 0)
            {
                Dominio.Entidades.Cliente fornecedor = repFornecedor.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", fornecedor.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fornecedor", false));

            if (filtrosPesquisa.NaturezaOperacao != null && filtrosPesquisa.NaturezaOperacao.Count > 1)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NaturezaOperacao", "Múltiplas Naturezas de Operações", true));
            else if (filtrosPesquisa.NaturezaOperacao != null && filtrosPesquisa.NaturezaOperacao.Count == 1)
            {
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(filtrosPesquisa.NaturezaOperacao[0]);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NaturezaOperacao", naturezaDaOperacao.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NaturezaOperacao", false));

            if (filtrosPesquisa.Empresa > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.Empresa);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", empresa.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Empresa", false));

            if (!string.IsNullOrWhiteSpace(parametrosConsulta?.PropriedadeAgrupar))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta?.PropriedadeAgrupar, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}