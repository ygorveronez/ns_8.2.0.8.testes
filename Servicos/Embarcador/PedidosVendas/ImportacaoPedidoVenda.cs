using System;
using System.Collections.Generic;
using Dominio.Excecoes.Embarcador;


namespace Servicos.Embarcador.PedidosVendas
{
    public sealed class ImportacaoPedidoVenda
    {
        #region Atributos Privados Somente Leitura

        private readonly Dictionary<string, dynamic> _dados;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.Entidades.Empresa _empresa;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private readonly AdminMultisoftware.Repositorio.UnitOfWork _unitOfWorkAdmin;

        #endregion Atributos Privados Somente Leitura

        #region Construtores

        public ImportacaoPedidoVenda(Dictionary<string, dynamic> dados, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador,
            Dominio.Entidades.Empresa empresa, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            _dados = dados;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _unitOfWork = unitOfWork;
            _empresa = empresa;
            _unitOfWorkAdmin = unitOfWorkAdmin;
            _configuracaoEmbarcador = configuracaoEmbarcador;

            if (!_unitOfWorkAdmin.IsActiveTransaction())
            {
                _unitOfWorkAdmin = new AdminMultisoftware.Repositorio.UnitOfWork(unitOfWorkAdmin.StringConexao);
                _unitOfWorkAdmin.Start();
            }
        }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda ObterPedidoVendaImportar()
        {
            Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda pedidoVenda = new Dominio.ObjetosDeValor.Embarcador.PedidosVendas.ImportacaoPedidoVenda();
            pedidoVenda.Cliente = ObterCliente();
            pedidoVenda.NumeroOrcamento = ObterNumeroOrcamento();
            pedidoVenda.DataEmissao = ObterDataEmissao();
            pedidoVenda.DataPrevisao = ObterDataPrevisao();
            pedidoVenda.Observacao = ObterObservacao();
            pedidoVenda.Referencia = ObterReferencia();
            pedidoVenda.FormaPagamento = ObterFormaPagamento();
            pedidoVenda.Produto = ObterProduto();
            pedidoVenda.DescricaoItem = ObterDescricaotem();
            pedidoVenda.Quantidade = ObterQuantidadeItem();
            pedidoVenda.ValorUnitario = ObterValorUnitario();
            pedidoVenda.ValorTotal = ObterValorTotal();
            pedidoVenda.NumeroOrdemCompra = ObterNumeroOrdemCompra();
            pedidoVenda.NumeroItemOrdemCompra = ObterNumeroItemOrdemCompra();

            return pedidoVenda;
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.Entidades.Cliente ObterCliente()
        {
            var CNPJBuscar = string.Empty;

            if (_dados.TryGetValue("CNPJ", out var cnpj))
                CNPJBuscar = (string)cnpj;

            if (_configuracaoEmbarcador.Pais == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPais.Brasil)
            {
                if (string.IsNullOrWhiteSpace(CNPJBuscar))
                    throw new ImportacaoException("CNPJ não informado");                
            }

            CNPJBuscar = Utilidades.String.OnlyNumbers(CNPJBuscar);

            Repositorio.Cliente repositorio = new Repositorio.Cliente(_unitOfWork);
            Dominio.Entidades.Cliente cliente = repositorio.BuscarPorCPFCNPJ(CNPJBuscar.ToDouble());

            if (cliente == null)
                throw new ImportacaoException("Cliente não encontrado na base de dados!");

            cliente.Initialize();

            return cliente;
        }

        private string ObterNumeroOrcamento()
        {
            var numOSRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroOrcamento", out var numeroOS))
                numOSRetornar = ((string)numeroOS).Trim();

            return numOSRetornar;
        }

        private DateTime ObterDataEmissao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataEmissao", out var dataEmissao))
                data = ((string)dataEmissao).ToNullableDateTime();

            return data ?? DateTime.Now;
        }

        private DateTime ObterDataPrevisao()
        {
            DateTime? data = null;
            if (_dados.TryGetValue("DataPrevisao", out var dataEmissao))
                data = ((string)dataEmissao).ToNullableDateTime();

            return data ?? DateTime.Now;
        }

        private string ObterObservacao()
        {
            var observacaoRetornar = string.Empty;

            if (_dados.TryGetValue("Observacao", out var observacao))
                observacaoRetornar = ((string)observacao).Trim();

            return observacaoRetornar;
        }

        private string ObterReferencia()
        {
            var referenciaRetornar = string.Empty;

            if (_dados.TryGetValue("Referencia", out var referencia))
                referenciaRetornar = ((string)referencia).Trim();

            return referenciaRetornar;
        }

        private string ObterFormaPagamento()
        {
            var formaPagamentoRetornar = string.Empty;

            if (_dados.TryGetValue("FormaPagamento", out var formaPagamento))
                formaPagamentoRetornar = ((string)formaPagamento).Trim();

            return formaPagamentoRetornar;
        }

        private Dominio.Entidades.Produto ObterProduto()
        {
            var codigoProdutoBuscar = string.Empty;

            if (_dados.TryGetValue("CodigoProduto", out var Produto))
                codigoProdutoBuscar = (string)Produto.Trim();

            if (string.IsNullOrWhiteSpace(codigoProdutoBuscar))
                throw new ImportacaoException("Código do produto não informado!");

            Repositorio.Produto repProduto = new Repositorio.Produto(_unitOfWork);
            Dominio.Entidades.Produto produto = repProduto.BuscarPorCodigoProduto(codigoProdutoBuscar, _empresa?.Codigo ?? 0);

            if (produto == null)
                throw new ImportacaoException("Produto não encontrado na base de dados!");

            produto.Initialize();

            return produto;
        }

        private string ObterNumeroOrdemCompra()
        {
            var descricaoItemRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroOrdemCompra", out var descricaoItem))
                descricaoItemRetornar = ((string)descricaoItem).Trim();

            return descricaoItemRetornar;
        }


        private string ObterNumeroItemOrdemCompra()
        {
            var descricaoItemRetornar = string.Empty;

            if (_dados.TryGetValue("NumeroItemOrdemCompra", out var descricaoItem))
                descricaoItemRetornar = ((string)descricaoItem).Trim();

            return descricaoItemRetornar;
        }


        private string ObterDescricaotem()
        {
            var descricaoItemRetornar = string.Empty;

            if (_dados.TryGetValue("DescricaoItem", out var descricaoItem))
                descricaoItemRetornar = ((string)descricaoItem).Trim();

            return descricaoItemRetornar;
        }

        private decimal ObterQuantidadeItem()
        {
            decimal quantidadeItemRetornar = 0;

            if (_dados.TryGetValue("Quantidade", out var quantidadeItem))
                quantidadeItemRetornar = ((string)quantidadeItem).ToDecimal();

            return quantidadeItemRetornar;
        }

        private decimal ObterValorUnitario()
        {
            decimal valorUnitarioRetornar = 0;

            if (_dados.TryGetValue("ValorUnitario", out var valorUnitario))
                valorUnitarioRetornar = ((string)valorUnitario).ToDecimal();

            return valorUnitarioRetornar;
        }

        private decimal ObterValorTotal()
        {
            decimal valorTotalRetornar = 0;

            if (_dados.TryGetValue("ValorTotal", out var valorTotal))
                valorTotalRetornar = ((string)valorTotal).ToDecimal();

            return valorTotalRetornar;
        }

        #endregion  Métodos Privados
    }
}
