using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Compras
{
    public class CotacaoCompra : ServicoBase
    {        
        public CotacaoCompra(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        #region Métodos Públicos

        public bool GerarOrdemCompra(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unidadeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string stringConexao, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado, bool finalizar)
        {
            try
            {
                Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria repCotacaoReq = new Repositorio.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria(unidadeTrabalho);
                Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unidadeTrabalho);
                Repositorio.Embarcador.Compras.OrdemCompra repOrdemCompra = new Repositorio.Embarcador.Compras.OrdemCompra(unidadeTrabalho);
                Repositorio.Embarcador.Compras.OrdemCompraMercadoria repOrdemCompraMercadoria = new Repositorio.Embarcador.Compras.OrdemCompraMercadoria(unidadeTrabalho);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarRetornosGanhadores(cotacao.Codigo);
                List<Dominio.Entidades.Embarcador.Compras.CotacaoCompraRequisicaoMercadoria> ListaCotacaoReq = repCotacaoReq.BuscarPorCodigoCotacao(cotacao.Codigo);

                int codigoEmpresa = 0;
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = cotacao.Empresa.Codigo;

                List<double> cnpjFornecedores = new List<double>();
                foreach (var retorno in retornos)
                {
                    if (!cnpjFornecedores.Contains(retorno.CotacaoFornecedor.Fornecedor.CPF_CNPJ))
                        cnpjFornecedores.Add(retorno.CotacaoFornecedor.Fornecedor.CPF_CNPJ);
                }

                for (int i = 0; i < cnpjFornecedores.Count; i++)
                {
                    List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornosFornecedor = repRetorno.BuscarRetornosGanhadores(cotacao.Codigo, cnpjFornecedores[i]);
                    Dominio.Entidades.Embarcador.Compras.OrdemCompra ordem = new Dominio.Entidades.Embarcador.Compras.OrdemCompra();
                    ordem.Data = DateTime.Now.Date;
                    ordem.DataPrevisaoRetorno = cotacao.DataPrevisao.Value;
                    ordem.Fornecedor = retornosFornecedor[0].CotacaoFornecedor.Fornecedor;
                    ordem.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemCompra.Aberta;
                    ordem.Numero = repOrdemCompra.BuscarProximoNumero(codigoEmpresa);
                    ordem.Usuario = cotacao.Usuario;
                    ordem.CotacaoCompra = cotacao;
                    ordem.MotivoCompra = cotacao.Requisicoes != null && cotacao.Requisicoes.Count > 0 ? cotacao.Requisicoes.Select(o => o.RequisicaoMercadoria.MotivoCompra)?.FirstOrDefault() ?? null : null;
                    ordem.Empresa = cotacao.Empresa;
                    ordem.Observacao = cotacao.Observacao == null ? ListaCotacaoReq?.FirstOrDefault()?.RequisicaoMercadoria?.Observacao ?? "" : cotacao.Observacao;
                    repOrdemCompra.Inserir(ordem);

                    foreach (var ret in retornosFornecedor)
                    {
                        Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria produto = new Dominio.Entidades.Embarcador.Compras.OrdemCompraMercadoria();
                        produto.OrdemCompra = ordem;
                        produto.Produto = ret.CotacaoProduto.Produto;
                        produto.Quantidade = ret.QuantidadeRetorno;
                        produto.ValorUnitario = ret.ValorUnitarioRetorno;

                        repOrdemCompraMercadoria.Inserir(produto);
                    }

                    if (finalizar)
                    {
                        Dominio.Entidades.Embarcador.Compras.OrdemCompra ordemCompra = repOrdemCompra.BuscarPorCodigo(ordem.Codigo);
                        Servicos.Embarcador.Compras.OrdemCompra.EtapaAprovacao(ref ordemCompra, unidadeTrabalho, tipoServicoMultisoftware, stringConexao, Auditado, codigoEmpresa);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        public static void AtualizaCotacaoVencedora(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repCotacaoRetornoFornecedorProduto = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Compras.CotacaoProduto produto in cotacao.Produtos)
            {
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> cotacoesProduto = repCotacaoRetornoFornecedorProduto.BuscarPorCotacaoEProduto(cotacao.Codigo, produto.Produto.Codigo);

                // Filtra os retornos que alcanaram o mínimo
                List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> cotacoesValidas = (from o in cotacoesProduto
                                                                                                              where
                                                                                                                o.QuantidadeRetorno >= o.CotacaoProduto.Quantidade
                                                                                                              orderby o.ValorUnitarioRetorno
                                                                                                              ascending
                                                                                                              select o).ToList();

                // So seta como vencedor caso tenha algum valor mínimo 
                if (cotacoesValidas.Count() > 0)
                {
                    foreach (Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto retorno in cotacoesProduto)
                    {
                        retorno.GerarOrdemCompra = false;
                        repCotacaoRetornoFornecedorProduto.Atualizar(retorno);
                    }

                    Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto vencedor = cotacoesValidas.FirstOrDefault();
                    vencedor.GerarOrdemCompra = true;
                    repCotacaoRetornoFornecedorProduto.Atualizar(vencedor);
                }
            }

        }

        public void EnviarEmailNovaCotacaoParaFornecedores(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, List<Dominio.Entidades.Cliente> fornecedores)
        {
            Repositorio.Embarcador.Compras.CotacaoFornecedor repCotacaoFornecedor = new Repositorio.Embarcador.Compras.CotacaoFornecedor(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            bool tipoMultiNFe = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(tipoMultiNFe ? cotacao.Empresa.Codigo : 0);

            if (email == null)
                return;

            if (fornecedores == null)
                fornecedores = repCotacaoFornecedor.BuscarFornecedoresPorCotacao(cotacao.Codigo);

            Dominio.Entidades.Empresa empresa = tipoMultiNFe ? cotacao.Empresa : repEmpresa.BuscarPrincipalEmissoraTMS();
            if (empresa == null)
            {
                Log.TratarErro("Empresa não encontrada para envio de email da cotação do fluxo de compras.");
                return;
            }

            string linkCotacao = Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoAmbiente().LinkCotacaoCompra;

            foreach (Dominio.Entidades.Cliente fornecedor in fornecedores)
            {
                string assunto = "Cotação de Compra nº " + cotacao.Numero.ToString("n0") + " de " + cotacao.DataEmissao.Value.ToDateString() + " da " + empresa.RazaoSocial;
                string mensagemEmail = "Olá amigo " + fornecedor.Nome + "<br/>";
                mensagemEmail += "Por gentileza, cotar os produtos até 24 horas (não esquecer de indicar a marca do seu produto).<br/>";
                mensagemEmail += "Segue link para acessar o sistema com seu Login e Senha.<br/>";
                mensagemEmail += "Link: " + linkCotacao;

                mensagemEmail += "<br/><br/>Pela atenção, obrigado!<br/>";

                mensagemEmail += "<br/><br/>E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(fornecedor.Email))
                    emails.AddRange(fornecedor.Email.Split(';').ToList());

                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in fornecedor.Emails)
                {
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                        emails.Add(outroEmail.Email);
                }

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, tipoMultiNFe ? cotacao.Empresa.Codigo : 0);
                    if (!sucesso)
                        Log.TratarErro($"Problemas ao enviar nova cotação por e-mail ao fornecedor { fornecedor.Nome }: " + mensagemErro);
                }
                else
                    Log.TratarErro($"Fornecedor { fornecedor.Nome } não possui e-mail cadastrado para envio da nova cotação.");
            }
        }

        public void EnviarEmailAvisoParaVencedoresPerdedores(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto repRetorno = new Repositorio.Embarcador.Compras.CotacaoRetornoFornecedorProduto(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornos = repRetorno.BuscarRetornosGanhadores(cotacao.Codigo);
            List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> retornosPerdedores = repRetorno.BuscarRetornosPerdedores(cotacao.Codigo);

            List<Dominio.Entidades.Cliente> fornecedoresGanhadores = retornos.Select(o => o.CotacaoFornecedor.Fornecedor).Distinct().ToList();
            List<Dominio.Entidades.Cliente> fornecedoresPerdedores = retornosPerdedores.Select(o => o.CotacaoFornecedor.Fornecedor).Distinct().ToList();

            bool tipoMultiNFe = tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe;

            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(tipoMultiNFe ? cotacao.Empresa.Codigo : 0);

            if (email == null)
                return;

            Dominio.Entidades.Empresa empresa = tipoMultiNFe ? cotacao.Empresa : repEmpresa.BuscarPrincipalEmissoraTMS();
            if (empresa == null)
            {
                Log.TratarErro("Empresa não encontrada para envio de email da cotação do fluxo de compras.");
                return;
            }

            EfetuarEnvioEmail(cotacao, unitOfWork, email, fornecedoresGanhadores, retornos, false, empresa, tipoServicoMultisoftware);
            EfetuarEnvioEmail(cotacao, unitOfWork, email, fornecedoresPerdedores.Where(o => !fornecedoresGanhadores.Any(g => g.CPF_CNPJ == o.CPF_CNPJ)).ToList(), null, true, empresa, tipoServicoMultisoftware);
        }

        #endregion

        #region Métodos Privados

        private void EfetuarEnvioEmail(Dominio.Entidades.Embarcador.Compras.CotacaoCompra cotacao, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email, List<Dominio.Entidades.Cliente> fornecedores, List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> produtosVencedores, bool perdedores, Dominio.Entidades.Empresa empresa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            foreach (Dominio.Entidades.Cliente fornecedor in fornecedores)
            {
                string assunto = "Finalizado Cotação de Compra nº " + cotacao.Numero.ToString("n0") + " de " + cotacao.DataEmissao.Value.ToDateString();
                string mensagemEmail;

                if (perdedores)
                {
                    mensagemEmail = $"Olá amigo { fornecedor.Nome }, a { empresa.RazaoSocial } agradece o seu retorno, porém não foi desta vez, quem sabe em uma próxima.";
                }
                else
                {
                    mensagemEmail = "Olá amigo " + fornecedor.Nome + ", sua cotação foi a vencedora para o(s) item(ns) abaixo. Está autorizado a prosseguir com o faturamento.<br/>";
                    mensagemEmail += "Por gentileza, não esquecer de mencionar no campo observação o número da cotação.";

                    mensagemEmail += "<br/><br/><div>";
                    mensagemEmail += "<table align='left' width='700' cellpadding='0' cellspacing='0' border='1'>";
                    mensagemEmail += "<tbody>";
                    mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                    mensagemEmail += "<td align='center' colspan='4'><span><font face='Arial' color='#000000' size='2'><b>Relação de Itens</b></font></span></td>";
                    mensagemEmail += "</tr>";
                    mensagemEmail += "<tr bgcolor='#D3D3D3'>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Produto</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Quantidade</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Valor Unitário</b></font></span></td>";
                    mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'><b>Valor Total</b></font></span></td>";
                    mensagemEmail += "</tr>";

                    string quebraLinha = "";
                    List<Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto> produtosFornecedor = produtosVencedores.Where(o => o.CotacaoFornecedor.Fornecedor.CPF_CNPJ == fornecedor.CPF_CNPJ).ToList();
                    foreach (Dominio.Entidades.Embarcador.Compras.CotacaoRetornoFornecedorProduto produto in produtosFornecedor)
                    {
                        mensagemEmail += "<tr>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + produto.CotacaoProduto.Produto.Descricao + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>" + produto.QuantidadeRetorno.ToString("n2") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>R$ " + produto.ValorUnitarioRetorno.ToString("n2") + "</font></span></td>";
                        mensagemEmail += "<td align='center'><span><font face='Arial' color='#000000' size='2'>R$ " + produto.ValorTotalRetorno.ToString("n2") + "</font></span></td>";
                        mensagemEmail += "</tr>";
                        quebraLinha += "<br/>";
                    }

                    mensagemEmail += "</tbody>";
                    mensagemEmail += "</table>";
                    mensagemEmail += "</div><br/>";
                    mensagemEmail += quebraLinha;
                }

                mensagemEmail += "<br/><br/>Pela atenção, obrigado!<br/><br/>";
                mensagemEmail += "Att. " + empresa.RazaoSocial;

                mensagemEmail += "<br/><br/>E-mail enviado automaticamente. Por favor, não responda.";
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                string mensagemErro = "Erro ao enviar e-mail";

                List<string> emails = new List<string>();
                if (!string.IsNullOrWhiteSpace(fornecedor.Email))
                    emails.AddRange(fornecedor.Email.Split(';').ToList());

                foreach (Dominio.Entidades.Embarcador.Pessoas.ClienteOutroEmail outroEmail in fornecedor.Emails)
                {
                    if (!string.IsNullOrWhiteSpace(outroEmail.Email) && outroEmail.EmailStatus == "A")
                        emails.Add(outroEmail.Email);
                }

                emails = emails.Distinct().ToList();
                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork, tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? cotacao.Empresa.Codigo : 0);
                    if (!sucesso)
                        Log.TratarErro($"Problemas ao enviar o aviso da cotação por e-mail ao fornecedor { fornecedor.Nome }: " + mensagemErro);
                }
                else
                    Log.TratarErro($"Fornecedor { fornecedor.Nome } não possui e-mail cadastrado para envio da cotação.");
            }
        }

        #endregion
    }
}
