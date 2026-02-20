using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.PedidosVendas
{
    public class VendaDireta : ServicoBase
    {        
        public VendaDireta(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Globais

        public bool AtualizarTitulos(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, Dominio.Entidades.Usuario usuario, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado)
        {
            erro = string.Empty;

            Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela repVendaDiretaParcela = new Repositorio.Embarcador.PedidoVenda.VendaDiretaParcela(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixa repTituloBaixa = new Repositorio.Embarcador.Financeiro.TituloBaixa(unidadeTrabalho);
            Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado repTituloBaixaAgrupado = new Repositorio.Embarcador.Financeiro.TituloBaixaAgrupado(unidadeTrabalho);
            Financeiro.ProcessoMovimento svcProcessoMovimento = new Financeiro.ProcessoMovimento(StringConexao);

            if (usuario.Empresa?.TipoMovimento == null || usuario.Empresa?.TipoPagamentoRecebimento == null)
            {
                erro = "Favor configurar o Tipo de Movimento e o Tipo de Pagamento Recebimento padrão no Cadastro da sua Empresa antes de prosseguir!";
                return false;
            }

            if (vendaDireta.BoletoConfiguracao == null && vendaDireta.TipoCobranca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca.Banco)
            {
                erro = "Não informado a configuração do boleto para a geração, favor ajustar a venda!";
                return false;
            }

            if (repVendaDiretaParcela.ContarPorVendaDireta(vendaDireta.Codigo) > 0)
            {
                erro = "Já existem títulos em negociação para esta venda, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (repVendaDiretaParcela.ContarPorStatusEVendaDireta(vendaDireta.Codigo) > 0)
            {
                erro = "Já existem títulos quitados dessa venda, não sendo possível atualizar os mesmos.";
                return false;
            }

            if (repVendaDiretaParcela.ContarPorBoletoStatusEVendaDireta(vendaDireta.Codigo) > 0)
            {
                erro = "Já existem Boletos gerados dessa venda, fazer cancelar os mesmos antes da venda.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela> parcelas = repVendaDiretaParcela.BuscarPorVendaDireta(vendaDireta.Codigo);

            foreach (Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaParcela parcela in parcelas)
            {
                if (vendaDireta.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = new Dominio.Entidades.Embarcador.Financeiro.Titulo();

                    titulo.TipoTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                    titulo.DataEmissao = DateTime.Now;
                    titulo.DataVencimento = parcela.DataVencimento;
                    titulo.DataProgramacaoPagamento = parcela.DataVencimento;
                    titulo.GrupoPessoas = vendaDireta.Cliente.GrupoPessoas;
                    titulo.Pessoa = vendaDireta.Cliente;
                    titulo.Sequencia = parcela.Sequencia;
                    titulo.ValorOriginal = parcela.Valor;
                    titulo.ValorPendente = parcela.Valor;
                    titulo.Desconto = parcela.Desconto;
                    titulo.Acrescimo = 0;
                    titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto;
                    titulo.DataAlteracao = DateTime.Now;
                    titulo.Observacao = !string.IsNullOrEmpty(vendaDireta.Observacao) ? "Venda Direta - " + vendaDireta.Observacao : string.Concat("Referente à parcela " + parcela.Sequencia + " da venda direta nº " + vendaDireta.Numero.ToString() + ".");
                    titulo.Empresa = vendaDireta.Empresa;
                    titulo.ValorTituloOriginal = titulo.ValorOriginal;
                    titulo.TipoDocumentoTituloOriginal = "Venda Direta";
                    titulo.NumeroDocumentoTituloOriginal = vendaDireta.Numero.ToString();
                    titulo.TipoMovimento = usuario.Empresa.TipoMovimento;
                    titulo.Provisao = false;
                    titulo.DataLancamento = DateTime.Now;
                    titulo.Usuario = usuario;

                    if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                        titulo.TipoAmbiente = tipoAmbiente;

                    if (vendaDireta.TipoCobranca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca.Banco)
                    {
                        titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Boleto;
                        titulo.BoletoConfiguracao = vendaDireta.BoletoConfiguracao;
                        titulo.BoletoStatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.BoletoStatusTitulo.Emitido;
                    }
                    else if (vendaDireta.TipoCobranca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca.Carteira && parcela.DataVencimento.Date == DateTime.Now.Date)
                    {
                        titulo.FormaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaTitulo.Dinheiro;
                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Quitada;
                        titulo.DataLiquidacao = titulo.DataEmissao;
                        titulo.DataBaseLiquidacao = titulo.DataEmissao;
                        titulo.ValorPago = titulo.ValorPendente;
                        titulo.ValorPendente = 0;
                    }
                    else
                        titulo.FormaTitulo = parcela.Forma;

                    repTitulo.Inserir(titulo, Auditado);

                    parcela.Titulo = titulo;
                    repVendaDiretaParcela.Atualizar(parcela);

                    if (!svcProcessoMovimento.GerarMovimentacao(out erro, titulo.TipoMovimento, titulo.DataEmissao.Value, titulo.ValorOriginal, vendaDireta.Numero.ToString(),
                        titulo.Observacao, unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, null, null, titulo.Codigo, null, null, null, titulo.DataEmissao))
                        return false;

                    if ((vendaDireta.TipoCobranca == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCobranca.Carteira && parcela.DataVencimento.Date == DateTime.Now.Date) || vendaDireta.TipoCobrancaVendaDireta == TipoCobrancaVendaDireta.PIX || vendaDireta.TipoCobrancaVendaDireta == TipoCobrancaVendaDireta.Avista)//Quita o título
                    {
                        if (!Servicos.Embarcador.Financeiro.Titulo.QuitarTitulo(out erro, titulo, DateTime.Now, DateTime.Now, unidadeTrabalho, titulo.Pessoa, null, usuario, tipoServicoMultisoftware, "COMANDO DE BAIXA AUTOMÁTICA VIA VENDA DIRETA", 0m, true, 0m, true, Auditado))                        
                            return false;                        

                        //Dominio.Entidades.Embarcador.Financeiro.TituloBaixa tituloBaixa = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixa();
                        //tituloBaixa.DataBaixa = titulo.DataEmissao;
                        //tituloBaixa.DataBase = titulo.DataEmissao;
                        //tituloBaixa.DataOperacao = DateTime.Now;
                        //tituloBaixa.Numero = 1;
                        //tituloBaixa.Observacao = "Gerado automaticamente pela Venda Direta de nº: " + vendaDireta.Numero.ToString();
                        //tituloBaixa.SituacaoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBaixaTitulo.Finalizada;
                        //tituloBaixa.Sequencia = 1;
                        //tituloBaixa.Valor = titulo.ValorOriginal;
                        //tituloBaixa.TipoBaixaTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo.Receber;
                        //tituloBaixa.Pessoa = vendaDireta.Cliente;
                        //tituloBaixa.Titulo = titulo;
                        //tituloBaixa.ModeloAntigo = true;
                        //tituloBaixa.TipoPagamentoRecebimento = usuario.Empresa.TipoPagamentoRecebimento;
                        //tituloBaixa.Usuario = usuario;
                        //repTituloBaixa.Inserir(tituloBaixa, Auditado);

                        //Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado tituloBaixaAgrupado = new Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado();
                        //tituloBaixaAgrupado.Titulo = titulo;
                        //tituloBaixaAgrupado.TituloBaixa = tituloBaixa;
                        //tituloBaixaAgrupado.DataBaixa = tituloBaixa.DataOperacao.Value;
                        //tituloBaixaAgrupado.DataBase = tituloBaixa.DataBase.Value;
                        //repTituloBaixaAgrupado.Inserir(tituloBaixaAgrupado, Auditado);

                        //svcProcessoMovimento.GerarMovimentacao(null, titulo.DataLiquidacao.Value, titulo.ValorPago, tituloBaixa.Codigo.ToString(),
                        //    "BAIXA DO TÍTULO A RECEBER AUTOMÁTICA VIA VENDA DIRETA", unidadeTrabalho, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento,
                        //    tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, usuario.Empresa.TipoPagamentoRecebimento.PlanoConta, 0, null, titulo.Pessoa, titulo.Pessoa.GrupoPessoas);
                    }
                }
                else if (vendaDireta.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta.Finalizado)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repVendaDiretaParcela.BuscarPorVendaDiretaParcela(parcela.Codigo);
                    if (titulo != null)
                    {
                        if (!svcProcessoMovimento.GerarMovimentacao(out erro, null, titulo.DataEmissao.Value, titulo.ValorOriginal, vendaDireta.Numero.ToString(), "REVERSÃO DO TÍTULO AUTOMÁTICO PELA VENDA DIRETA - " + titulo.Observacao, unidadeTrabalho,
                            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Outros, tipoServicoMultisoftware, 0, titulo.TipoMovimento.PlanoDeContaDebito, titulo.TipoMovimento.PlanoDeContaCredito,
                            titulo.Codigo, null, null, null, titulo.DataEmissao.Value))
                            return false;

                        titulo.StatusTitulo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado;
                        titulo.DataAlteracao = DateTime.Now;
                        titulo.DataCancelamento = DateTime.Now;

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Cancelado título.", unidadeTrabalho);
                        repTitulo.Atualizar(titulo);

                        parcela.Titulo = null;
                        repVendaDiretaParcela.Atualizar(parcela);
                    }
                }
            }

            return true;
        }

        public bool MovimentarEstoque(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            erro = null;

            Repositorio.Embarcador.PedidoVenda.VendaDiretaItem repVendaDiretaItem = new Repositorio.Embarcador.PedidoVenda.VendaDiretaItem(unidadeTrabalho);
            Repositorio.Embarcador.NotaFiscal.ProdutoEstoque repProdutoEstoque = new Repositorio.Embarcador.NotaFiscal.ProdutoEstoque(unidadeTrabalho);
            Servicos.Embarcador.Produto.Estoque servicoEstoque = new Servicos.Embarcador.Produto.Estoque(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem> itens = repVendaDiretaItem.BuscarPorVendaDireta(vendaDireta.Codigo);

            foreach (Dominio.Entidades.Embarcador.PedidoVenda.VendaDiretaItem item in itens)
            {
                if (item.Produto != null)
                {
                    if (vendaDireta.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta.Finalizado)
                    {
                        Dominio.Entidades.Embarcador.NotaFiscal.ProdutoEstoque estoque = repProdutoEstoque.BuscarPorProduto(item.Produto.Codigo, item.Produto.Empresa?.Codigo ?? 0);
                        if (estoque == null)
                            estoque = repProdutoEstoque.BuscarPorProduto(item.Produto.Codigo);
                        if (estoque == null || estoque.Quantidade < item.Quantidade)
                        {
                            erro = "Produto " + item.Produto.Descricao + " não possui estoque para a venda, favor dar entrada nele antes de finalizar a venda";
                            return false;
                        }
                        else if (item.Produto.Empresa != null)
                        {
                            estoque.Empresa = item.Produto.Empresa;
                            repProdutoEstoque.Atualizar(estoque);
                        }

                        if (!servicoEstoque.MovimentarEstoque(out erro, item.Produto, item.Quantidade, Dominio.Enumeradores.TipoMovimento.Saida, "VEDIR", vendaDireta.Numero.ToString(), 0, item.Produto?.Empresa, DateTime.Now, tipoServicoMultisoftware))
                            return false;
                    }
                    //else if (vendaDireta.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusVendaDireta.Finalizado)
                    //{
                    //    if (!Servicos.Embarcador.Produto.Estoque.MovimentarEstoque(out erro, item.Produto, item.Quantidade, Dominio.Enumeradores.TipoMovimento.Entrada, "VEDIR", vendaDireta.Numero.ToString(), unidadeTrabalho, 0, item.Produto?.Empresa, DateTime.Now))
                    //        return false;
                    //}
                }
            }

            return true;
        }

        public void GerarNotificacaoVenda(Dominio.Entidades.Embarcador.PedidoVenda.VendaDireta vendaDireta, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, out string erro)
        {
            erro = "";

            if (vendaDireta.ProdutoServico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ProdutoServico.Servico)
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Notificacao.Notificacao();

                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unidadeTrabalho);

                List<Dominio.Entidades.Usuario> usuariosEmpresa = repUsuario.BuscarPorEmpresa(vendaDireta.Empresa.Codigo, "U");
                if (usuariosEmpresa?.Count > 0)
                {
                    foreach (var usuario in usuariosEmpresa)
                    {
                        serNotificacao.GerarNotificacao(usuario, vendaDireta.Codigo, "PedidosVendas/VendaDireta", "Nova venda direta de serviço criada. Número: " + vendaDireta.Numero, IconesNotificacao.atencao, TipoNotificacao.vendaDireta, tipoServicoMultisoftware, unidadeTrabalho);
                    }
                }
            }
        }

        #endregion
    }
}
