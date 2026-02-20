using System;

namespace Servicos.Embarcador.Produto
{
    public class Estoque
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Estoque(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool ValidarProdutoComEstoque(out string erro, Dominio.Entidades.Produto produto, decimal quantidade, Dominio.Entidades.Cliente oficina, Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento, bool validarEstoquePosicao = false, DateTime? dataPosicaoEstoque= null)
        {
            erro = "";

            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(_unitOfWork);

            if (oficina != null && empresa == null)
                empresa = ObterEmpresaOficina(oficina);

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = null;
            decimal quantidadeEstoque = 0;
            if (validarEstoquePosicao)
            {
                quantidadeEstoque = repProdutoEstoque.BuscarEstoquePosicao(produto.Codigo, empresa?.Codigo ?? 0, dataPosicaoEstoque ?? DateTime.Now);
            }
            else if (configuracao.UtilizaMultiplosLocaisArmazenamento)
            {
                estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, empresa?.Codigo, localArmazenamento?.Codigo);
                quantidadeEstoque = estoque?.Quantidade ?? 0;
            }
            else
            {
                estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, empresa?.Codigo ?? 0);
                if (estoque == null)
                    estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo);

                quantidadeEstoque = estoque?.Quantidade ?? 0;
            }

            if (quantidadeEstoque < quantidade || quantidadeEstoque < 0)
            {
                erro = "O produto " + produto.Descricao + " não possui o estoque suficiente para realizar a movimentação. Estoque atual: " + (quantidadeEstoque.ToString("n4")) + " Estoque necessário: " + quantidade.ToString("n4");
                return false;
            }

            return true;
        }

        public bool MovimentarEstoque(out string erro, Dominio.Entidades.Produto produto, decimal quantidade, Dominio.Enumeradores.TipoMovimento tipoMovimento, string tipoDocumento, string documento, decimal custo, Dominio.Entidades.Empresa empresa, DateTime dataMovimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Cliente oficina = null, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = null)
        {
            if (oficina != null && empresa == null)
                empresa = ObterEmpresaOficina(oficina);

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = ObterEstoque(produto, empresa, dataMovimento, tipoServicoMultisoftware, localArmazenamento);

            MovimentarEstoque(estoque, quantidade, tipoMovimento, tipoDocumento, documento, custo, empresa, dataMovimento);

            erro = string.Empty;
            return true;
        }

        public void MovimentarEstoque(Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque, decimal quantidade, Dominio.Enumeradores.TipoMovimento tipoMovimento, string tipoDocumento, string documento, decimal custo, Dominio.Entidades.Empresa empresa, DateTime dataMovimento)
        {
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(_unitOfWork);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico repProdutoEstoqueHistorico = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoqueHistorico(_unitOfWork);

            DateTime dataEstoque = DateTime.Now;
            if (dataMovimento > DateTime.MinValue)
            {
                estoque.Data = dataMovimento;
                dataEstoque = dataMovimento;
            }
            else
                estoque.Data = dataEstoque;

            if (tipoMovimento == Dominio.Enumeradores.TipoMovimento.Entrada)
                estoque.Quantidade += quantidade;
            else
                estoque.Quantidade -= quantidade;

            repProdutoEstoque.Atualizar(estoque);

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico historico = new Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoqueHistorico
            {
                Data = dataEstoque,
                Documento = documento,
                Quantidade = quantidade,
                Tipo = tipoMovimento,
                TipoDocumento = tipoDocumento,
                Custo = custo,
                Empresa = empresa != null ? empresa : estoque.Empresa,
                Produto = estoque.Produto,
                LocalArmazenamento = estoque.LocalArmazenamento,
                ProdutoEstoque = estoque
            };

            repProdutoEstoqueHistorico.Inserir(historico);
        }

        public Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque AdicionarEstoque(Dominio.Entidades.Produto produto, Dominio.Entidades.Empresa empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = null, DateTime? dataMovimento = null)
        {
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(_unitOfWork);
            Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto repLocalArmazenamentoProduto = new Repositorio.Embarcador.Produtos.LocalArmazenamentoProduto(_unitOfWork);

            int codigoEmpresa = empresa?.Codigo ?? 0;

            if (localArmazenamento == null && (configuracao.UtilizaMultiplosLocaisArmazenamento || tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
                localArmazenamento = produto.LocalArmazenamentoProduto != null ? produto.LocalArmazenamentoProduto : codigoEmpresa > 0 ? repLocalArmazenamentoProduto.BuscarPrimeiroPorEmpresa(codigoEmpresa) : null;

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = new Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque();
            estoque.Produto = produto;
            estoque.Empresa = empresa;
            estoque.LocalArmazenamento = localArmazenamento;
            estoque.Data = dataMovimento.HasValue ? dataMovimento.Value : DateTime.Now;
            estoque.Quantidade = 0;
            estoque.CustoMedio = 0;
            estoque.UltimoCusto = 0;
            estoque.EstoqueMaximo = 0;
            estoque.EstoqueMinimo = 0;

            repProdutoEstoque.Inserir(estoque);

            return estoque;
        }

        public bool MovimentarEstoqueReserva(out string erro, Dominio.Entidades.Produto produto, decimal quantidade, Dominio.Enumeradores.TipoMovimento tipoMovimento, Dominio.Entidades.Empresa empresa, DateTime dataMovimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Cliente oficina = null, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento = null)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto repConfiguracaoProduto = new Repositorio.Embarcador.Configuracoes.ConfiguracaoProduto(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoProduto configuracaoProduto = repConfiguracaoProduto.BuscarConfiguracaoPadrao();

            if (!configuracaoProduto.ControlarEstoqueReserva)
            {
                erro = string.Empty;
                return true;
            }

            if (oficina != null && empresa == null)
                empresa = ObterEmpresaOficina(oficina);

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = ObterEstoque(produto, empresa, dataMovimento, tipoServicoMultisoftware, localArmazenamento);

            decimal quantidadeLogicaEstoque = ((estoque.Quantidade) - (estoque.QuantidadeEstoqueReservada));
            if (tipoMovimento == Dominio.Enumeradores.TipoMovimento.Entrada)
                if (quantidadeLogicaEstoque < quantidade)
                {
                    erro = "Estoque do produto em reserva e indisponível para lançamento!";
                    return false;
                }

            MovimentarEstoqueReserva(estoque, quantidade, tipoMovimento);

            erro = string.Empty;
            return true;
        }

        private void MovimentarEstoqueReserva(Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque, decimal quantidade, Dominio.Enumeradores.TipoMovimento tipoMovimento)
        {
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(_unitOfWork);

            if (tipoMovimento == Dominio.Enumeradores.TipoMovimento.Entrada)
                estoque.QuantidadeEstoqueReservada += quantidade;
            else
                estoque.QuantidadeEstoqueReservada -= quantidade;

            repProdutoEstoque.Atualizar(estoque);
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Empresa ObterEmpresaOficina(Dominio.Entidades.Cliente oficina)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedorPessoas = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(_unitOfWork);

            Dominio.Entidades.Empresa empresaOficina = repModalidadeFornecedorPessoas.BuscarEmpresaOficinaPorCliente(oficina.CPF_CNPJ);
            if (empresaOficina != null)
                return empresaOficina;

            return repEmpresa.BuscarPorCNPJ(oficina.CPF_CNPJ_SemFormato);
        }

        private Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque ObterEstoque(Dominio.Entidades.Produto produto, Dominio.Entidades.Empresa empresa, DateTime dataMovimento, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Produtos.LocalArmazenamentoProduto localArmazenamento)
        {
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repConfiguracao.BuscarConfiguracaoPadrao();

            Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque;
            if (configuracao.UtilizaMultiplosLocaisArmazenamento)
                estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, empresa?.Codigo, localArmazenamento?.Codigo);
            else
            {
                localArmazenamento = null;
                estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo, empresa?.Codigo ?? 0);
                if (estoque == null)
                    estoque = repProdutoEstoque.BuscarPorProduto(produto.Codigo);
            }

            if (estoque == null)
                estoque = AdicionarEstoque(produto, empresa, tipoServicoMultisoftware, configuracao, localArmazenamento, dataMovimento);
            if (estoque != null && (empresa != null || localArmazenamento != null))
            {
                if (empresa != null)
                    estoque.Empresa = empresa;
                if (localArmazenamento != null)
                    estoque.LocalArmazenamento = localArmazenamento;

                repProdutoEstoque.Atualizar(estoque);
            }

            return estoque;
        }

        #endregion
    }
}
