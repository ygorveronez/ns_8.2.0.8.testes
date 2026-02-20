using Dominio.Interfaces.Database;
using System.Collections.Generic;
using System.Threading;

namespace Servicos.Embarcador.Patrimonio;

public class Bem : ServicoBase
    {        
        public Bem(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }

        public bool CadastrarBem(Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documentoEntrada, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = null;

            if (documentoEntrada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoDocumentoEntrada.Finalizado)
                return true;

            Repositorio.Embarcador.Financeiro.DocumentoEntradaItem repDocumentoEntradaItem = new Repositorio.Embarcador.Financeiro.DocumentoEntradaItem(unidadeTrabalho);
            Repositorio.Embarcador.Patrimonio.Bem repBem = new Repositorio.Embarcador.Patrimonio.Bem(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem> itens = repDocumentoEntradaItem.BuscarPorDocumentoEntrada(documentoEntrada.Codigo);

            foreach (Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaItem item in itens)
            {
                if (item.Produto != null && item.Produto.ProdutoBem != null && item.Produto.ProdutoBem.Value && !repBem.ContemBemCadastradoPeloDocumentoEntrada(item.Codigo))
                {
                    for (int i = 0; i < item.Quantidade; i++)
                    {
                        Dominio.Entidades.Embarcador.Patrimonio.Bem bem = new Dominio.Entidades.Embarcador.Patrimonio.Bem();

                        bem.DataImplantacao = documentoEntrada.DataEntrada;
                        bem.DataAquisicao = documentoEntrada.DataEmissao;
                        bem.DataImplantacao = documentoEntrada.DataEntrada;
                        bem.DataFimGarantia = documentoEntrada.DataEntrada;

                        bem.Descricao = item.Produto.Descricao;
                        bem.DescricaoNota = !string.IsNullOrWhiteSpace(item.DescricaoProdutoFornecedor) ? item.DescricaoProdutoFornecedor : item.Produto.DescricaoNotaFiscal;
                        bem.ValorBem = item.ValorUnitario;

                        bem.Almoxarifado = item.Produto.Almoxarifado;
                        bem.CentroResultado = item.Produto.CentroResultado;
                        bem.GrupoProdutoTMS = item.Produto.GrupoProdutoTMS;
                        bem.Fornecedor = documentoEntrada.Fornecedor;

                        bem.DocumentoEntradaItem = item;
                        bem.Empresa = documentoEntrada.Destinatario;
                        bem.Produto = item.Produto;

                        repBem.Inserir(bem);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, bem, null, "Novo bem inserido.", unidadeTrabalho);
                    }
                }
            }
            return true;
        }
    }

