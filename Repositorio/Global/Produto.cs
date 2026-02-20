using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Repositorio.Embarcador.Produtos.Consulta;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Threading;

namespace Repositorio
{
    public class Produto : RepositorioBase<Dominio.Entidades.Produto>, Dominio.Interfaces.Repositorios.Produto
    {
        public Produto(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public Produto(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Produto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

        public List<string> BuscarDescricaoPorCodigo(List<int> codigosProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where codigosProduto.Contains(obj.Codigo) select obj;

            return result.Select(o => o.Descricao).ToList();
        }

        public bool ContemProdutoMesmoCodigo(string codigoProduto, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoProduto == codigoProduto && obj.Status == "A" select obj;

            if (codigo > 0)
                result = result.Where(o => o.Codigo != codigo);

            return result.Any();
        }

        public Dominio.Entidades.Produto BuscarPorCodigoProduto(string codigoProduto, int codigoEmpresa = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoProduto == codigoProduto && obj.Status == "A" select obj;
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarPorCodigoProdutoECategoria(string codigoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaProduto categoriaProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoProduto == codigoProduto && obj.CategoriaProduto == categoriaProduto select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarPorCodigoProdutoELocalArmazenamento(string codigoProduto, int codigoLocalArmazenamento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoProduto == codigoProduto && obj.Status == "A" select obj;

            if (codigoLocalArmazenamento > 0)
                result = result.Where(o => o.LocalArmazenamentoProduto.Codigo == codigoLocalArmazenamento);
            else
                result = result.Where(o => o.LocalArmazenamentoProduto == null);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Produto> BuscarPorCodigo(int[] codigosProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            query = query.Where(o => codigosProdutos.Contains(o.Codigo));

            return query.ToList();
        }

        public List<Dominio.Entidades.Produto> BuscarPorCodigo(List<int> codigosProdutos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            query = query.Where(o => codigosProdutos.Contains(o.Codigo));

            return query.ToList();
        }

        public Dominio.Entidades.Produto BuscarPorCodigoIntegracao(int codigoEmpresa, string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoProduto == codigo && obj.Empresa.Codigo == codigoEmpresa && obj.Status == "A" select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarPorCodigo(int codigoEmpresa, int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Produto BuscarPorNCM(string codigoNCM)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.CodigoNCM == codigoNCM select obj;

            return result.FirstOrDefault();
        }
        public List<Dominio.Entidades.Produto> Consultar(int codigoEmpresa, string descricao, string status, string codigoProduto, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(codigoProduto))
                result = result.Where(o => o.CodigoProduto.Equals(codigoProduto));

            return result.OrderBy(o => o.Descricao).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, string status, string codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.Empresa.Codigo == codigoEmpresa select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(o => o.Status.Equals(status));

            if (!string.IsNullOrWhiteSpace(codigoProduto))
                result = result.Where(o => o.CodigoProduto.Equals(codigoProduto));

            return result.Count();
        }

        public List<Dominio.Entidades.Produto> ConsultarTipoCombustivelTMS(bool somenteAbastecimentos, int codigo, string descricao, TipoAbastecimento? tipoAbastecimento, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = ConsultarTipoCombustivelTMS(somenteAbastecimentos, codigo, descricao, tipoAbastecimento);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTipoCombustivelTMS(bool somenteAbastecimentos, int codigo, string descricao, TipoAbastecimento? tipoAbastecimento)
        {
            var result = ConsultarTipoCombustivelTMS(somenteAbastecimentos, codigo, descricao, tipoAbastecimento);

            return result.Count();
        }

        public List<Dominio.Entidades.Produto> ConsultarTipoCombustivel(int codigo, string descricao, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsultaTipoCombustivel(int codigo, string descricao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> ConsultarEstoqueMinimo(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros, bool somenteEstoqueMinimo, int grupoProdutoTms)
        {
            var result = ConsultarEstoqueMinimo(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador, somenteEstoqueMinimo, grupoProdutoTms);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaEstoqueMinimo(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador, bool somenteEstoqueMinimo, int grupoProdutoTms)
        {
            var result = ConsultarEstoqueMinimo(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador, somenteEstoqueMinimo, grupoProdutoTms);

            return result.Count();
        }

        public List<Dominio.Entidades.Produto> ConsultarProdutoEstoque(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = ConsultarProdutoEstoque(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaProdutoEstoque(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador)
        {
            var result = ConsultarProdutoEstoque(filial, codigoBarrasEAN, descricao, ativo, empresa, codigoProdutoEmbarcador);

            return result.Count();
        }

        public List<Dominio.Entidades.Produto> Consulta(int codigoGrupoImposto, string codigoBarrasEAN, int codigo, string codigoProduto, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento? tipoAbastecimento, int codigoEmpresa, string codigoProdutoEmbarcador, int codigoOrdemCompra, bool somenteComEstoque, bool IsCodigoDeBarras, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, int codigoLocalArmazenamento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();
            var queryOrdemCompraMercadoria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();

            if (codigo > 0 && codigo.ToString() == codigoProduto && codigo.ToString() == codigoBarrasEAN)
            {
                query = query.Where(obj => obj.Codigo == codigo || obj.CodigoProduto == codigoProduto || obj.CodigoBarrasEAN == codigoBarrasEAN);
            }
            else
            {

                if (!string.IsNullOrWhiteSpace(descricao))
                    query = query.Where(obj => obj.Descricao.Contains(descricao));

                if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                    query = query.Where(obj => obj.CodigoBarrasEAN.Contains(codigoBarrasEAN) || obj.CodigoEAN.Contains(codigoBarrasEAN));

                if (!string.IsNullOrWhiteSpace(ncm))
                    query = query.Where(obj => obj.CodigoNCM.Contains(ncm));

                if (!string.IsNullOrWhiteSpace(codigoProduto))
                    query = query.Where(obj => obj.CodigoProduto.Contains(codigoProduto));

                if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                {
                    if (IsCodigoDeBarras)
                        query = query.Where(obj => obj.CodigoProduto.Equals(codigoProdutoEmbarcador));
                    else
                        query = query.Where(obj => obj.CodigoProduto.Contains(codigoProdutoEmbarcador) || obj.Codigo.ToString().Contains(codigoProdutoEmbarcador));
                }
                if (codigo > 0)
                    query = query.Where(obj => obj.Codigo == codigo);

                if (codigoGrupoImposto > 0)
                    query = query.Where(obj => obj.GrupoImposto.Codigo == codigoGrupoImposto);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(obj => obj.Status.Equals("A"));
                else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(obj => obj.Status.Equals("I"));

                if (tipoAbastecimento.HasValue)
                {
                    if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
                        query = query.Where(obj => (obj.CodigoNCM.StartsWith("271121") || obj.CodigoNCM.StartsWith("271019") || obj.CodigoNCM.StartsWith("271012") || obj.CodigoNCM.StartsWith("220710")) && obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true);
                    else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                        query = query.Where(obj => obj.CodigoNCM.StartsWith("310210") && obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true);
                }

                if (codigoOrdemCompra > 0)
                {
                    var resultOrdemCompraMercadoria = from obj in queryOrdemCompraMercadoria where obj.OrdemCompra.Codigo == codigoOrdemCompra select obj;
                    query = query.Where(obj => resultOrdemCompraMercadoria.Any(o => o.Produto.Codigo == obj.Codigo));
                }

                if (somenteComEstoque)
                {
                    var queryEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
                    query = query.Where(obj => queryEstoque.Any(o => o.Produto.Codigo == obj.Codigo && o.Quantidade > 0));
                }
            }

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo.Equals(codigoEmpresa));

            if (codigoLocalArmazenamento > 0)
                query = query.Where(obj => obj.LocalArmazenamentoProduto.Codigo == codigoLocalArmazenamento);

            if (maximoRegistros > 0)
                return query.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
            else
                return query.ToList();
        }

        public int ContaConsulta(int codigoGrupoImposto, string codigoBarrasEAN, int codigo, string codigoProduto, string descricao, string ncm, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento? tipoAbastecimento, int codigoEmpresa, string codigoProdutoEmbarcador, int codigoOrdemCompra, bool somenteComEstoque, bool IsCodigoDeBarras, int codigoLocalArmazenamento = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();
            var queryOrdemCompraMercadoria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria>();

            if (codigo > 0 && codigo.ToString() == codigoProduto && codigo.ToString() == codigoBarrasEAN)
            {
                query = query.Where(obj => obj.Codigo == codigo || obj.CodigoProduto == codigoProduto || obj.CodigoBarrasEAN == codigoBarrasEAN);
            }
            else
            {

                if (!string.IsNullOrWhiteSpace(descricao))
                    query = query.Where(obj => obj.Descricao.Contains(descricao));

                if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                    query = query.Where(obj => obj.CodigoBarrasEAN.Contains(codigoBarrasEAN) || obj.CodigoEAN.Contains(codigoBarrasEAN));

                if (!string.IsNullOrWhiteSpace(ncm))
                    query = query.Where(obj => obj.CodigoNCM.Contains(ncm));

                if (!string.IsNullOrWhiteSpace(codigoProduto))
                    query = query.Where(obj => obj.CodigoProduto.Contains(codigoProduto));

                if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                {
                    if (IsCodigoDeBarras)
                        query = query.Where(obj => obj.CodigoProduto.Equals(codigoProdutoEmbarcador));
                    else
                        query = query.Where(obj => obj.CodigoProduto.Contains(codigoProdutoEmbarcador) || obj.Codigo.ToString().Contains(codigoProdutoEmbarcador));

                }
                if (codigo > 0)
                    query = query.Where(obj => obj.Codigo == codigo);

                if (codigoGrupoImposto > 0)
                    query = query.Where(obj => obj.GrupoImposto.Codigo == codigoGrupoImposto);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(obj => obj.Status.Equals("A"));
                else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                    query = query.Where(obj => obj.Status.Equals("I"));

                if (tipoAbastecimento.HasValue)
                {
                    if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Combustivel)
                        query = query.Where(obj => (obj.CodigoNCM.StartsWith("271121") || obj.CodigoNCM.StartsWith("271019") || obj.CodigoNCM.StartsWith("271012")) && obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true);
                    else if (tipoAbastecimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento.Arla)
                        query = query.Where(obj => obj.CodigoNCM.StartsWith("310210") && obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true);
                }

                if (codigoOrdemCompra > 0)
                {
                    var resultOrdemCompraMercadoria = from obj in queryOrdemCompraMercadoria where obj.OrdemCompra.Codigo == codigoOrdemCompra select obj;
                    query = query.Where(obj => resultOrdemCompraMercadoria.Any(o => o.Produto.Codigo == obj.Codigo));
                }

                if (somenteComEstoque)
                {
                    var queryEstoque = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();
                    query = query.Where(obj => queryEstoque.Any(o => o.Produto.Codigo == obj.Codigo && o.Quantidade > 0));
                }
            }

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo.Equals(codigoEmpresa));

            if (codigoLocalArmazenamento > 0)
                query = query.Where(obj => obj.LocalArmazenamentoProduto.Codigo == codigoLocalArmazenamento);

            return query.Count();
        }

        public Dominio.Entidades.Produto BuscarPorPostoTabelaDeValor(double cnpj, string codigoIntegradao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pessoas.PostoCombustivelTabelaValores>();

            var result = from obj in query where obj.CodigoIntegracao.Equals(codigoIntegradao) && obj.ModalidadeFornecedorPessoas.ModalidadePessoas.Cliente.CPF_CNPJ == cnpj select obj;

            IQueryable<Dominio.Entidades.Produto> queryProduto = result.Select(obj => obj.Produto);
            return queryProduto.FirstOrDefault();
        }

        public List<Dominio.Entidades.Produto> BuscarProdutosDocumentoEntrada(int produtoEmEBS, List<int> modelosDocumento, int codigoFilial, DateTime dataEntradaInicial, DateTime dataEntradaFinal, DateTime dataEmissaoInicial, DateTime dataEmissaoFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem>();

            var result = from obj in query where obj.Produto != null && obj.DocumentoEntrada.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado select obj;

            if (modelosDocumento != null && modelosDocumento.Count > 0)
            {
                IEnumerable<int> modelosDocumentoValidos = modelosDocumento.Where(o => o > 0);

                if (modelosDocumentoValidos.Count() > 0)
                    result = result.Where(obj => modelosDocumentoValidos.Contains(obj.DocumentoEntrada.Modelo.Codigo));
            }

            if (codigoFilial > 0)
                result = result.Where(obj => obj.DocumentoEntrada.Destinatario.Codigo == codigoFilial);

            if (produtoEmEBS > 0)
            {
                if (produtoEmEBS == 1)//nunca gerado
                    result = result.Where(obj => obj.Produto.ProdutoEmEBS == false || obj.Produto.ProdutoEmEBS == null);
                else if (produtoEmEBS == 2)//gerado em algum arquivo
                    result = result.Where(obj => obj.Produto.ProdutoEmEBS == true);
            }

            if (dataEntradaInicial > DateTime.MinValue && dataEntradaFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEntrada >= dataEntradaInicial && obj.DocumentoEntrada.DataEntrada <= dataEntradaFinal.AddDays(1));
            else if (dataEntradaInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEntrada <= dataEntradaInicial);
            else if (dataEntradaFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEntrada >= dataEntradaFinal.AddDays(1));

            if (dataEmissaoInicial > DateTime.MinValue && dataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEmissao >= dataEmissaoInicial && obj.DocumentoEntrada.DataEmissao <= dataEmissaoFinal.AddDays(1));
            else if (dataEmissaoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEmissao <= dataEmissaoInicial);
            else if (dataEmissaoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DocumentoEntrada.DataEmissao >= dataEmissaoFinal.AddDays(1));

            return result.Select(obj => obj.Produto).Distinct().ToList();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida ObterUnidadeMedida(string unidadeMedida, Repositorio.UnitOfWork unidadeDeTrabalho, int codigoEmpresa)
        {
            Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor repUnidadeMedidaFornecedor = new Repositorio.Embarcador.Produtos.UnidadeMedidaFornecedor(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Produtos.UnidadeMedidaFornecedor unidadeMedidaFornecedor = repUnidadeMedidaFornecedor.BuscarPorDescricao(unidadeMedida, codigoEmpresa);
            if (unidadeMedidaFornecedor != null)
                return unidadeMedidaFornecedor.UnidadeDeMedida;

            switch (unidadeMedida.ToLower())
            {
                case "kg":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Quilograma;
                case "lt":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Litros;
                case "mmbtu":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MMBTU;
                case "m3":
                case "m³":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroCubico;
                case "to":
                case "ton":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Tonelada;
                case "un":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
                case "ser":
                case "srv":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Servico;
                case "cx":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Caixa;
                case "m2":
                case "m²":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.MetroQuadrado;
                case "m":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Metro;
                case "pc":
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Peca;

                default:
                    return Dominio.ObjetosDeValor.Embarcador.Enumeradores.UnidadeDeMedida.Unidade;
            }
        }

        public void RemoverVinculoEntidades(int codigoProduto)
        {
            this.SessionNHiBernate.CreateSQLQuery("DELETE FROM T_PRODUTO_ESTOQUE WHERE PRO_CODIGO = :codigoProduto").SetParameter("codigoProduto", codigoProduto).ExecuteUpdate();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque> ConsultarEstoqueMinimo(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador, bool somenteEstoqueMinimo, int grupoProdutoTms)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque>();

            var result = from obj in query
                         where obj.EstoqueMinimo > obj.Quantidade && obj.Empresa.Codigo == filial
                         select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Produto.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                result = result.Where(obj => obj.Produto.CodigoBarrasEAN.Contains(codigoBarrasEAN) || obj.Produto.CodigoEAN.Contains(codigoBarrasEAN));

            if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                result = result.Where(obj => obj.Produto.CodigoProduto.Contains(codigoProdutoEmbarcador) || obj.Produto.Codigo.ToString().Contains(codigoProdutoEmbarcador));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Produto.Status.Equals("A"));
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Produto.Status.Equals("I"));

            if (empresa > 0)
                result = result.Where(obj => obj.Produto.Empresa.Codigo.Equals(empresa));

            if (grupoProdutoTms > 0)
                result = result.Where(obj => obj.Produto.GrupoProdutoTMS.Codigo == grupoProdutoTms);

            if (somenteEstoqueMinimo)
                result = result.Where(obj => obj.EstoqueMinimo >= 1.00m);

            return result;
        }

        private IQueryable<Dominio.Entidades.Produto> ConsultarProdutoEstoque(int filial, string codigoBarrasEAN, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo, int empresa, string codigoProdutoEmbarcador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query select obj;

            if (filial > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filial);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(codigoBarrasEAN))
                result = result.Where(obj => obj.CodigoBarrasEAN.Contains(codigoBarrasEAN) || obj.CodigoEAN.Contains(codigoBarrasEAN));

            if (!string.IsNullOrWhiteSpace(codigoProdutoEmbarcador))
                result = result.Where(obj => obj.CodigoProduto.Contains(codigoProdutoEmbarcador) || obj.Codigo.ToString().Contains(codigoProdutoEmbarcador));

            if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(obj => obj.Status.Equals("A"));
            else if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(obj => obj.Status.Equals("I"));

            if (empresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo.Equals(empresa));

            return result;
        }

        private IQueryable<Dominio.Entidades.Produto> ConsultarTipoCombustivelTMS(bool somenteAbastecimentos, int codigo, string descricao, TipoAbastecimento? tipoAbastecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            var result = from obj in query where obj.ProdutoCombustivel.HasValue && obj.ProdutoCombustivel.Value == true select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(o => o.Descricao.Contains(descricao));

            if (codigo > 0)
                result = result.Where(o => o.Codigo == codigo);

            if (somenteAbastecimentos || tipoAbastecimento.HasValue)
            {
                var queryAbastecimento = this.SessionNHiBernate.Query<Dominio.Entidades.Abastecimento>();
                if (tipoAbastecimento.HasValue)
                    result = result.Where(o => queryAbastecimento.Any(a => a.Produto.Codigo == o.Codigo && a.TipoAbastecimento == tipoAbastecimento));
                else
                    result = result.Where(o => queryAbastecimento.Any(a => a.Produto.Codigo == o.Codigo));
            }

            result = result.Where(o => o.Status == "A");

            return result;
        }

        private IQueryable<Dominio.Entidades.Produto> ConsultarPedidos(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa)
        {
            var consultaVeiculo = this.SessionNHiBernate.Query<Dominio.Entidades.Produto>();

            return Consultar(consultaVeiculo, filtrosPesquisa);
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto> ConsultarRelatorioProduto(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var query = new ConsultaProduto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Produtos.Produto>();
        }

        public int ContarConsultaRelatorioProduto(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var query = new ConsultaProduto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return query.SetTimeout(600).UniqueResult<int>();
        }


        public int ContarConsultaRelatorioProdutos(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa)
        {
            var consulta = ConsultarPedidos(filtrosPesquisa);

            return consulta.Count();
        }

        private IQueryable<Dominio.Entidades.Produto> Consultar(IQueryable<Dominio.Entidades.Produto> consultaProdutos, Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa)
        {
            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoNCM))
                consultaProdutos = consultaProdutos.Where(p => p.CodigoNCM == filtrosPesquisa.CodigoNCM);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoCEST))
                consultaProdutos = consultaProdutos.Where(p => p.CodigoCEST == filtrosPesquisa.CodigoCEST);

            if (!string.IsNullOrEmpty(filtrosPesquisa.CodigoBarrasEAN))
                consultaProdutos = consultaProdutos.Where(p => p.CodigoBarrasEAN == filtrosPesquisa.CodigoBarrasEAN);

            if (filtrosPesquisa.CodigoProduto > 0)
                consultaProdutos = consultaProdutos.Where(p => p.Codigo == filtrosPesquisa.CodigoProduto);

            if (filtrosPesquisa.CodigoGrupo > 0)
                consultaProdutos = consultaProdutos.Where(p => p.GrupoProdutoTMS.Codigo == filtrosPesquisa.CodigoGrupo);

            if (filtrosPesquisa.CodigoMarca > 0)
                consultaProdutos = consultaProdutos.Where(p => p.MarcaProduto.Codigo == filtrosPesquisa.CodigoMarca);

            if (filtrosPesquisa.CodigoLocalArmazenamento > 0)
                consultaProdutos = consultaProdutos.Where(p => p.LocalArmazenamentoProduto.Codigo == filtrosPesquisa.CodigoLocalArmazenamento);

            if (filtrosPesquisa.CodigoGrupoImposto > 0)
                consultaProdutos = consultaProdutos.Where(p => p.GrupoImposto.Codigo == filtrosPesquisa.CodigoGrupoImposto);

            if (filtrosPesquisa.CodigoEmpresa > 0)
                consultaProdutos = consultaProdutos.Where(p => p.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Ativo)
                consultaProdutos = consultaProdutos.Where(p => p.Status == "A");

            if (filtrosPesquisa.Status == SituacaoAtivoPesquisa.Inativo)
                consultaProdutos = consultaProdutos.Where(p => p.Status == "I");

            //if(filtrosPesquisa.CategoriaProduto != null)

            //    consultaProdutos = consultaProdutos.Where(p => p.CategoriaProduto == CategoriaProduto.);

            return consultaProdutos;
        }

        public List<Dominio.Entidades.Produto> ConsultarProdutos(Dominio.ObjetosDeValor.Embarcador.Produtos.FiltroPesquisaRelatorioProduto filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var consultaPedidos = ConsultarPedidos(filtrosPesquisa);
            return ObterLista(consultaPedidos, parametroConsulta);
        }

        #endregion
    }
}
