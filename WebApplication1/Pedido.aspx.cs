using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WebApplication1
{
    public partial class Pedido : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ServicoNFeNovo.NFeClient svcNFe = new ServicoNFeNovo.NFeClient();

            svcNFe.IntegrarNotasFiscais(new Dominio.ObjetosDeValor.WebService.Carga.Protocolos()
            {
                protocoloIntegracaoCarga = 84456,
                protocoloIntegracaoPedido = 84408
            },
            new Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF[] {
                new Dominio.ObjetosDeValor.Embarcador.NFe.TokenNF() {
                    Token = "42170383011247002346558080000003601952624176",
                    TipoNotaFiscalIntegrada = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscalIntegrada.Faturamento
                }
            });
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {

            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();

            Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido pedido = MontarCargaPedido();

            ServiceReference5.RetornoOfRetornoMetodoEntrada4PBTRYa8 retorno = WSpedido.AdicionarPedidoMultiEmbarcador(pedido);

            if (retorno.Status)
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = "";
                lblNumeroCarga.Text = retorno.Objeto.CodigoCargaMultiEmbarcador.ToString();
                lblProtocolo.Text = retorno.Objeto.Protocolo.ToString();
            }
            else
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = retorno.Mensagem;
                if (retorno.Objeto == null)
                {
                    lblNumeroCarga.Text = "0";
                    lblProtocolo.Text = "0";
                }
                else
                {
                    lblNumeroCarga.Text = retorno.Objeto.CodigoCargaMultiEmbarcador.ToString();
                    lblProtocolo.Text = retorno.Objeto.Protocolo.ToString(); ;
                }

            }

        }


        protected void btnAtualizarPedido_Click(object sender, EventArgs e)
        {
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();

            Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido pedido = MontarCargaPedido();

            ServiceReference5.RetornoOfRetornoMetodoEntrada4PBTRYa8 retorno = WSpedido.AtualizarPedidoMultiEmbarcador(pedido);

            if (retorno.Status)
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = "";
                lblNumeroCarga.Text = retorno.Objeto.CodigoCargaMultiEmbarcador.ToString();
                lblProtocolo.Text = retorno.Objeto.Protocolo.ToString();
            }
            else
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = retorno.Mensagem;
                if (retorno.Objeto == null)
                {
                    lblNumeroCarga.Text = "0";
                    lblProtocolo.Text = "0";
                }
                else
                {
                    lblNumeroCarga.Text = retorno.Objeto.CodigoCargaMultiEmbarcador.ToString();
                    lblProtocolo.Text = retorno.Objeto.Protocolo.ToString(); ;
                }

            }
        }

        private SGTCarga.CargaIntegracao MontarCargaNovoWS()
        {
            List<SGTCarga.Produto> listaProduto = new List<SGTCarga.Produto>();
            if (ckbLeiteIntegral.Checked)
            {
                SGTCarga.Produto LeiteIntegral = new SGTCarga.Produto();
                LeiteIntegral.CodigoProduto = "2357";
                LeiteIntegral.DescricaoProduto = "Leite Integral";
                LeiteIntegral.CodigoGrupoProduto = "1";
                LeiteIntegral.ValorUnitario = 1;
                LeiteIntegral.DescricaoGrupoProduto = "Longa Vida";
                LeiteIntegral.PesoUnitario = decimal.Parse(txtPesoUnitLeiteIntegral.Text);
                LeiteIntegral.Quantidade = decimal.Parse(txtQuantidadeLeiteIntegral.Text);
                LeiteIntegral.QuantidadeEmbalagem = (decimal)5;
                LeiteIntegral.PesoTotalEmbalagem = (decimal)58.66;
                listaProduto.Add(LeiteIntegral);
            }

            if (ckbLeiteEmPo.Checked)
            {
                SGTCarga.Produto leiteEmPo = new SGTCarga.Produto();
                leiteEmPo.CodigoProduto = "555";
                leiteEmPo.DescricaoProduto = "Leite em pó";
                leiteEmPo.CodigoGrupoProduto = "1";
                leiteEmPo.ValorUnitario = (decimal)1.23;
                leiteEmPo.DescricaoGrupoProduto = "Longa Vida";
                leiteEmPo.PesoUnitario = decimal.Parse(txtPesoUnitLeiteEmPo.Text);
                leiteEmPo.Quantidade = decimal.Parse(txtQuantidadeLeiteEmPo.Text);
                leiteEmPo.QuantidadeEmbalagem = (decimal)7;
                leiteEmPo.PesoTotalEmbalagem = (decimal)7.66;
                listaProduto.Add(leiteEmPo);
            }

            if (ckbQueijo.Checked)
            {
                SGTCarga.Produto queijo = new SGTCarga.Produto();
                queijo.CodigoProduto = "4587";
                queijo.DescricaoProduto = "Queijo Prata";
                queijo.CodigoGrupoProduto = "2";
                queijo.DescricaoGrupoProduto = "Queijos";
                queijo.ValorUnitario = (decimal)5.12;
                queijo.PesoUnitario = decimal.Parse(txtPesoUnitQueijo.Text);
                queijo.Quantidade = decimal.Parse(txtQuantidadeQueijo.Text);
                queijo.QuantidadeEmbalagem = (decimal)10;
                queijo.PesoTotalEmbalagem = (decimal)17.66;
                listaProduto.Add(queijo);
            }

            if (ckbIogurte.Checked)
            {

                SGTCarga.Produto iogurte = new SGTCarga.Produto();
                iogurte.CodigoProduto = "1569";
                iogurte.DescricaoProduto = "Iogurte Morango";
                iogurte.CodigoGrupoProduto = "3";
                iogurte.DescricaoGrupoProduto = "Resfriados";
                iogurte.ValorUnitario = (decimal)0.99;
                iogurte.PesoUnitario = decimal.Parse(txtPesoUnitIogurte.Text);
                iogurte.Quantidade = decimal.Parse(txtQuantidadeIogurte.Text);
                iogurte.QuantidadeEmbalagem = (decimal)10;
                iogurte.PesoTotalEmbalagem = (decimal)12.58;
                listaProduto.Add(iogurte);
            }

            if (ckbProdutoProsyst.Checked)
            {
                SGTCarga.Produto produtoProsyst = new SGTCarga.Produto();
                produtoProsyst.CodigoProduto = "2064";
                produtoProsyst.DescricaoProduto = "BEBIDA LACTEA FERM.MORANGO TIROL 900G";
                produtoProsyst.CodigoGrupoProduto = "4";
                produtoProsyst.DescricaoGrupoProduto = "Grupo Genérico";
                produtoProsyst.ValorUnitario = (decimal)1.9;
                produtoProsyst.PesoUnitario = decimal.Parse(txtPesoUnitProdutoProsyst.Text);
                produtoProsyst.Quantidade = decimal.Parse(txtQuantidadeProdutoProsyst.Text);
                produtoProsyst.QuantidadeEmbalagem = (decimal)19;
                produtoProsyst.PesoTotalEmbalagem = (decimal)12.985;
                listaProduto.Add(produtoProsyst);
            }



            SGTCarga.CargaIntegracao pedido = new SGTCarga.CargaIntegracao();

            pedido.Remetente = retornoRemetente();
            pedido.Destinatario = retornaDestinatario();



            pedido.NumeroPedidoEmbarcador = this.txtCodigoPedido.Text;
            pedido.NumeroCarga = this.txtCodigoCarga.Text;
            pedido.Filial = new SGTCarga.Filial() { CodigoIntegracao = "1" };



            //pedido.CodigoModeloVeicularEmbarcador = "1";
            //pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";

            //pedido.NumeroPaletes = 10;
            //pedido.PesoTotalPaletes = 315;
            //pedido.ValorTotalPaletes = 100;

            //pedido.Produtos = listaProduto;
            if (listaProduto.Count > 0)
            {
                pedido.ProdutoPredominante = listaProduto[0];
            }
            pedido.PlacaVeiculo = "FQR1782";
            pedido.FecharCargaAutomaticamente = true;

            return pedido;
        }

        private Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido MontarCargaPedido()
        {
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos> listaProduto = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos>();
            if (ckbLeiteIntegral.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos LeiteIntegral = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                LeiteIntegral.CodigoProduto = "2357";
                LeiteIntegral.DescricaoProduto = "Leite Integral";
                LeiteIntegral.CodigoGrupoProduto = "1";
                LeiteIntegral.ValorUnitario = 1;
                LeiteIntegral.DescricaoGrupoProduto = "Longa Vida";
                LeiteIntegral.PesoUnitario = decimal.Parse(txtPesoUnitLeiteIntegral.Text);
                LeiteIntegral.Quantidade = decimal.Parse(txtQuantidadeLeiteIntegral.Text);
                LeiteIntegral.QuantidadeEmbalagem = (decimal)5;
                LeiteIntegral.PesoTotalEmbalagem = (decimal)58.66;
                listaProduto.Add(LeiteIntegral);
            }

            if (ckbLeiteEmPo.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos leiteEmPo = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                leiteEmPo.CodigoProduto = "555";
                leiteEmPo.DescricaoProduto = "Leite em pó";
                leiteEmPo.CodigoGrupoProduto = "1";
                leiteEmPo.ValorUnitario = (decimal)1.23;
                leiteEmPo.DescricaoGrupoProduto = "Longa Vida";
                leiteEmPo.PesoUnitario = decimal.Parse(txtPesoUnitLeiteEmPo.Text);
                leiteEmPo.Quantidade = decimal.Parse(txtQuantidadeLeiteEmPo.Text);
                leiteEmPo.QuantidadeEmbalagem = (decimal)7;
                leiteEmPo.PesoTotalEmbalagem = (decimal)7.66;
                listaProduto.Add(leiteEmPo);
            }

            if (ckbQueijo.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos queijo = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                queijo.CodigoProduto = "4587";
                queijo.DescricaoProduto = "Queijo Prata";
                queijo.CodigoGrupoProduto = "2";
                queijo.DescricaoGrupoProduto = "Queijos";
                queijo.ValorUnitario = (decimal)5.12;
                queijo.PesoUnitario = decimal.Parse(txtPesoUnitQueijo.Text);
                queijo.Quantidade = decimal.Parse(txtQuantidadeQueijo.Text);
                queijo.QuantidadeEmbalagem = (decimal)10;
                queijo.PesoTotalEmbalagem = (decimal)17.66;
                listaProduto.Add(queijo);
            }

            if (ckbIogurte.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos iogurte = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                iogurte.CodigoProduto = "1569";
                iogurte.DescricaoProduto = "Iogurte Morango";
                iogurte.CodigoGrupoProduto = "3";
                iogurte.DescricaoGrupoProduto = "Resfriados";
                iogurte.ValorUnitario = (decimal)0.99;
                iogurte.PesoUnitario = decimal.Parse(txtPesoUnitIogurte.Text);
                iogurte.Quantidade = decimal.Parse(txtQuantidadeIogurte.Text);
                iogurte.QuantidadeEmbalagem = (decimal)10;
                iogurte.PesoTotalEmbalagem = (decimal)12.58;
                listaProduto.Add(iogurte);
            }

            if (ckbProdutoProsyst.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos produtoProsyst = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                produtoProsyst.CodigoProduto = "2064";
                produtoProsyst.DescricaoProduto = "BEBIDA LACTEA FERM.MORANGO TIROL 900G";
                produtoProsyst.CodigoGrupoProduto = "4";
                produtoProsyst.DescricaoGrupoProduto = "Grupo Genérico";
                produtoProsyst.ValorUnitario = (decimal)1.9;
                produtoProsyst.PesoUnitario = decimal.Parse(txtPesoUnitProdutoProsyst.Text);
                produtoProsyst.Quantidade = decimal.Parse(txtQuantidadeProdutoProsyst.Text);
                produtoProsyst.QuantidadeEmbalagem = (decimal)19;
                produtoProsyst.PesoTotalEmbalagem = (decimal)12.985;
                listaProduto.Add(produtoProsyst);
            }



            Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido pedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido();

            if (rdbCuiaba.Checked)
            {
                pedido.Cliente = retornaClienteCuiaba();
            }
            else if (rdbCuritiba.Checked)
            {
                pedido.Cliente = retornaClienteCuritiba();
            }
            else if (rdbJoinvile.Checked)
            {
                pedido.Cliente = retornaClienteJoinville();
            }
            else if (rdbPontaGrossa.Checked)
            {
                pedido.Cliente = retornaClientePontaGrossa();
            }
            else if (rdbSaoPaulo.Checked)
            {
                pedido.Cliente = retornaClienteSaoPaulo();
            }
            else if (rdbSaoPauloPotenza.Checked)
            {
                pedido.Cliente = retornaClientePotenza();
            }
            else if (rbdSaoGoncalo.Checked)
            {
                pedido.Cliente = retornaClienteSaoGoncalo();
            }
            else if (rdbCascavel.Checked)
            {
                pedido.Cliente = retornaClienteCascavel();
            }
            else if (rdbToledo.Checked)
            {
                pedido.Cliente = retornaClienteToledo();
            }

            if (!string.IsNullOrWhiteSpace(txtCodigoCargaSubistituicao.Text))
            {
                pedido.CodigoCargaMultiEmbarcadorOrigemCancelada = int.Parse(txtCodigoCargaSubistituicao.Text);
            }


            pedido.CodigoIBGEOrigem = 4204202;
            pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;

            if (ddlOperacao.SelectedItem.Value == "1")
            {
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;
            }
            if (ddlOperacao.SelectedItem.Value == "2")
            {
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaComRedespacho;
                pedido.Recebedor = retornaOperador();
                pedido.Expedidor = retornaOperador();
            }
            if (ddlOperacao.SelectedItem.Value == "3")
            {
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.EntregaArmazem;
                pedido.Recebedor = retornaOperador();
            }

            if (ddlOperacao.SelectedItem.Value == "4")
            {
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaArmazemCliente;
                pedido.Expedidor = retornaOperador();
                pedido.CodigoIBGEOrigem = 3550308;
            }


            if (rbdClienteTesteProsyst.Checked)
            {
                pedido.Cliente = retornaClienteTesteProsyst();
                pedido.CodigoIBGEOrigem = 4218509;
                pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";
            }

            if (rbdClienteTesteProsystErro.Checked)
            {
                pedido.Cliente = retornaClienteTesteProsyst();
                pedido.CodigoIBGEOrigem = 4218509;
                pedido.CodigoIBGEDestino = 8218509;
                pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";
            }
            else
            {
                pedido.CodigoIBGEDestino = pedido.Cliente.CodigoIBGECidade;
            }

            pedido.CodigoPedido = this.txtCodigoPedido.Text;
            pedido.CodigoCargaEmbarcador = this.txtCodigoCarga.Text;
            pedido.CodigoFilial = "1";
            pedido.CodigoFilialTomadora = "1";

            pedido.CodigoModeloVeicularEmbarcador = "";


            //pedido.CodigoModeloVeicularEmbarcador = "1";
            //pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";

            pedido.NumeroPaletes = 10;
            pedido.PesoTotalPaletes = 315;
            pedido.ValorTotalPaletes = 100;
            pedido.DataCarregamento = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy") + " 04:00:00";

            pedido.ProdutosDoPedido = listaProduto;

            return pedido;
        }


        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteCuiaba()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "CUIABA",
                CodigoIBGECidade = 5103403,
                Complemento = "Teste",
                CPFCNPJ = "71056183000105",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "teste",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADO CUIABA",
                Numero = "1178",
                RazaoSocial = "EMPRESA COMERCIO CUIABA LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }


        private SGTCarga.Pessoa retornoRemetente()
        {
            SGTCarga.Pessoa cliente = new SGTCarga.Pessoa()
            {
                Endereco = new SGTCarga.Endereco()
                {
                    Bairro = "centro",
                    CEP = "89801141",
                    Cidade = new SGTCarga.Localidade()
                    {
                        IBGE = 3550308,
                        Descricao = "SAO PAULO"
                    },
                    Numero = "1178",
                    Complemento = "Teste",
                    DDDTelefone = "49",
                    Telefone = "3323-3902",
                    Logradouro = "teste"
                },
                CodigoAtividade = 3,
                CPFCNPJ = "31555584000195",
                Email = "willian@multisoftware.com.br",
                ClienteExterior = false,
                NomeFantasia = "SUPER MERCADO SAO PAULO",
                RazaoSocial = "EMPRESA COMERCIO SAO PAULO LTDA",
                RGIE = ""
            };

            return cliente;
        }


        private SGTCarga.Pessoa retornaDestinatario()
        {

            SGTCarga.Pessoa cliente = new SGTCarga.Pessoa()
            {
                Endereco = new SGTCarga.Endereco()
                {
                    Bairro = "centro",
                    CEP = "89801141",
                    Cidade = new SGTCarga.Localidade()
                    {
                        IBGE = 5103403,
                        Descricao = "CUIABA"
                    },
                    Numero = "1178",
                    Complemento = "Teste",
                    DDDTelefone = "49",
                    Telefone = "3323-3902",
                    Logradouro = "teste"
                },
                CodigoAtividade = 3,
                CPFCNPJ = "71056183000105",
                Email = "willian@multisoftware.com.br",
                ClienteExterior = false,
                NomeFantasia = "SUPER MERCADO CUIABA",
                RazaoSocial = "EMPRESA COMERCIO CUIABA LTDA",
                RGIE = ""
            };

            return cliente;
        }


        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteSaoPaulo()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "SAO PAULO",
                CodigoIBGECidade = 3550308,
                Complemento = "Teste",
                CPFCNPJ = "31555584000195",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "teste",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADO SAO PAULO",
                Numero = "1178",
                RazaoSocial = "EMPRESA COMERCIO SAO PAULO LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }


        private Dominio.ObjetosDeValor.CTe.Cliente retornaClientePontaGrossa()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "PONTA GROSSA",
                CodigoIBGECidade = 4119905,
                Complemento = "Teste",
                CPFCNPJ = "89073784000191",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "teste",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADO PONTA GROSSA",
                Numero = "1178",
                RazaoSocial = "EMPRESA COMERCIO PONTA GROSSA LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteJoinville()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "Chapeco",
                CodigoIBGECidade = 4209102,
                Complemento = "Teste",
                CPFCNPJ = "66725387000151",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADO JOINVILLE",
                Numero = "1178",
                RazaoSocial = "EMPRESA COMERCIO JOINVILLE LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteTesteProsyst()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "BATERIAS",
                CEP = "83648000",
                CodigoAtividade = 3,
                Cidade = "CAMPO LARGO",
                CodigoIBGECidade = 4104204, //4104204,
                Complemento = "Teste",
                CodigoPais = "1",
                CPFCNPJ = "00.297.455/0001-10",
                Endereco = "BAR E MERCEARIA",
                RazaoSocial = "BAR E MERCEARIA"
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteCascavel()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "Cascavel",
                CodigoIBGECidade = 4104808,
                Complemento = "Teste",
                CPFCNPJ = "75864728000160",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "SUPERMERCADOS IRANI LTDA",
                Numero = "1178",
                RazaoSocial = "SUPERMERCADOS IRANI LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteToledo()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Centro",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "Toledo",
                CodigoIBGECidade = 4127700,
                Complemento = "Teste",
                CPFCNPJ = "21523052000112",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "UNIAO E ALIANCA SUPERMERCAD LTDA ME",
                Numero = "1178",
                RazaoSocial = "UNIAO E ALIANCA SUPERMERCAD LTDA ME",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteCuritiba()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Presidente Médici",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "Chapeco",
                CodigoIBGECidade = 4106902,
                Complemento = "Teste",
                CPFCNPJ = "73336133000107",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADO CURITIBA",
                Numero = "1178",
                RazaoSocial = "EMPRESA COMERCIO CURITIBA LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "84876550",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClientePotenza()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Presidente Médici",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "Chapeco",
                CodigoIBGECidade = 3550308,
                Complemento = "Teste",
                CPFCNPJ = "06128376000162",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "POTENZA SUPER MERCADOS",
                Numero = "1178",
                RazaoSocial = "POTENZA COSMETICOS INSDUSTRIA E COMERCIO LTDA",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaClienteSaoGoncalo()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "Presidente Médici",
                CEP = "89801141",
                CodigoAtividade = 3,
                Cidade = "SÃO GONÇALO",
                CodigoIBGECidade = 3304904,
                Complemento = "Teste",
                CPFCNPJ = "22287551000110",
                Emails = "willian@multisoftware.com.br",
                EmailsContador = "willian@multisoftware.com.br",
                EmailsContato = "willian@multisofware.com.br",
                Endereco = "Rua 7 de Setembro",
                Exportacao = false,
                NomeFantasia = "SUPER MERCADOS SÃO GONÇALO",
                Numero = "1178",
                RazaoSocial = "SUPER MERCADO E DISTRIBUIDORAS SÃO GONÇALO",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "49-9992-1865",
                Telefone2 = "49*3323*3902",
                RGIE = "",
            };

            return cliente;
        }

        private Dominio.ObjetosDeValor.CTe.Cliente retornaOperador()
        {
            Dominio.ObjetosDeValor.CTe.Cliente cliente = new Dominio.ObjetosDeValor.CTe.Cliente()
            {
                Bairro = "CAXINGUI",
                CEP = "05533000",
                CodigoAtividade = 3,
                Cidade = "SAO PAULO",
                CodigoIBGECidade = 3550308,
                Complemento = "FUNDOS SALA 01",
                CPFCNPJ = "18119902000107",
                Emails = "infra@multisoftware.com.br",
                EmailsContador = "infra@multisoftware.com.br",
                EmailsContato = "infra@multisoftware.com.br",
                Endereco = "AVENIDA ELISEU DE ALMEIDA",
                Exportacao = false,
                NomeFantasia = "TRANS HAYAKU ",
                Numero = "1187",
                RazaoSocial = "TRANS HAYAKU TRANSPORTES LTDA - EPP",
                StatusEmails = true,
                StatusEmailsContador = true,
                StatusEmailsContato = true,
                Telefone1 = "(11) 4786-2759",
                Telefone2 = "(11) 4786-2759",
                RGIE = "142392097112",
            };

            return cliente;
        }



        protected void btnBuscarPedido_Click(object sender, EventArgs e)
        {
            lblRetornoPedido.Text = "";
            int numeroProtocolo = int.Parse(this.txtNumeroProtocoloPedido.Text);
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            ServiceReference5.RetornoOfArrayOfRetornoCargaPedido4PBTRYa8 retorno = WSpedido.BuscarPorProtocoloPedido(numeroProtocolo);
            if (retorno.Objeto != null)
            {
                foreach (ServiceReference5.RetornoCargaPedido pedidocarga in retorno.Objeto)
                {
                    lblRetornoPedido.Text += "Carga :" + pedidocarga.CodigoCargaMultiEmbarcador.ToString() + " (" + pedidocarga.SituacaoNF.ToString() + ") CNPJ Transportador:" + pedidocarga.CNPJTransportador;
                    lblRetornoPedido.Text += " Valor Total : " + pedidocarga.ValorTotalAReceber.ToString();
                    lblRetornoPedido.Text += " ValorFrete : " + pedidocarga.ValorFrete.ToString();
                    lblRetornoPedido.Text += " ValorICMS : " + pedidocarga.ValorICMS.ToString();
                    lblRetornoPedido.Text += " ValorComplementoFrete : " + pedidocarga.ValorComplementoFrete.ToString();
                    lblRetornoPedido.Text += " ValorComplementoICMS : " + pedidocarga.ValorComplementoICMS.ToString();
                    lblRetornoPedido.Text += " ValorDescarga : " + pedidocarga.ValorDescarga.ToString();
                    lblRetornoPedido.Text += " ValorADValorem : " + pedidocarga.ValorADValorem.ToString();
                    lblRetornoPedido.Text += " ValorPedagio : " + pedidocarga.ValorPedagio.ToString();
                    lblRetornoPedido.Text += " ValorOutros : " + pedidocarga.ValorOutros.ToString();
                }
            }
            else
            {
                lblRetornoPedido.Text = retorno.Mensagem;
            }

        }

        protected void btnNumeroCarga_Click(object sender, EventArgs e)
        {
            lblNumeroCarga.Text = "";
            int codigoCarga = int.Parse(this.txtNumeroCarga.Text);
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            ServiceReference5.RetornoOfArrayOfRetornoCargaPedido4PBTRYa8 retorno = WSpedido.BuscarPorCodigoCargaMultiEmbarcador(codigoCarga);
            foreach (ServiceReference5.RetornoCargaPedido pedidocarga in retorno.Objeto)
            {
                lblCarga.Text += "Pedido :" + pedidocarga.ProtocoloPedido + " (" + pedidocarga.SituacaoNF.ToString() + ") ";
            }
        }

        protected void btnBuscarPendentes_Click(object sender, EventArgs e)
        {
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            ServiceReference5.RetornoOfArrayOfRetornoCargaPedido4PBTRYa8 retorno = WSpedido.BuscarPorNotasPendentes();
            foreach (ServiceReference5.RetornoCargaPedido pedidocarga in retorno.Objeto)
            {
                lblPendentesNF.Text += "  Pedido :" + pedidocarga.ProtocoloPedido + " (" + pedidocarga.SituacaoNF.ToString() + ") " + " carga: " + pedidocarga.CodigoCargaEmbarcador;
            }
        }

        protected void btnEnviarNF_Click(object sender, EventArgs e)
        {
            //FileStream fs = new FileStream(this.fleNF.Value,FileMode.Open);
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            ServiceReference5.RetornoOfstring retorno = WSpedido.EnviarXMLNFeParaIntegracao(this.fleNF.PostedFile.InputStream);
            lblNumeroNF.Text += " " + retorno.Objeto;
            //fs.Dispose();
            //fs.Close();
        }

        protected void bntEnviarNFs_Click(object sender, EventArgs e)
        {
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal> listaNF = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal>();
            string[] nf = this.guidNFs.Text.Split('/');
            int not = 0;
            if (nf.Length > 0)
            {
                foreach (string item in nf)
                {
                    not++;
                    Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal xmlNF = new Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal();
                    xmlNF.TokenXMLNF = item;
                    if (not == 2)
                        xmlNF.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Remessa;
                    else
                        xmlNF.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                    listaNF.Add(xmlNF);
                }
            }
            else
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal xmlNF = new Dominio.ObjetosDeValor.Embarcador.Pedido.XMLNotaFiscal();
                xmlNF.TokenXMLNF = nf[0];
                xmlNF.TipoNotaFiscal = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotaFiscal.Venda;
                listaNF.Add(xmlNF);
            }

            ServiceReference5.RetornoOfboolean retorno = WSpedido.EnviarNotasFiscaisPedido(int.Parse(this.protocoloPedidoNF.Text), int.Parse(numeroCargaNF.Text), false, listaNF);
            if (retorno.Status)
            {
                lblRetornoEnvioNF.Text = "Sucesso";
            }
            else
            {
                lblRetornoEnvioNF.Text = retorno.Mensagem;
            }
        }

        protected void btnRefaturar_Click(object sender, EventArgs e)
        {
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();

            List<Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos> listaProduto = new List<Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos>();
            if (ckbLeiteIntegral.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos LeiteIntegral = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                LeiteIntegral.CodigoProduto = "2357";
                LeiteIntegral.DescricaoProduto = "Leite Integral";
                LeiteIntegral.CodigoGrupoProduto = "1";
                LeiteIntegral.ValorUnitario = 1;
                LeiteIntegral.DescricaoGrupoProduto = "Longa Vida";
                LeiteIntegral.PesoUnitario = decimal.Parse(txtPesoUnitLeiteIntegral.Text);
                LeiteIntegral.Quantidade = decimal.Parse(txtQuantidadeLeiteIntegral.Text);
                listaProduto.Add(LeiteIntegral);
            }

            if (ckbLeiteEmPo.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos leiteEmPo = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                leiteEmPo.CodigoProduto = "555";
                leiteEmPo.DescricaoProduto = "Leite em pó";
                leiteEmPo.CodigoGrupoProduto = "1";
                leiteEmPo.ValorUnitario = (decimal)1.23;
                leiteEmPo.DescricaoGrupoProduto = "Longa Vida";
                leiteEmPo.PesoUnitario = decimal.Parse(txtPesoUnitLeiteEmPo.Text);
                leiteEmPo.Quantidade = decimal.Parse(txtQuantidadeLeiteEmPo.Text);
                listaProduto.Add(leiteEmPo);
            }

            if (ckbQueijo.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos queijo = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                queijo.CodigoProduto = "4587";
                queijo.DescricaoProduto = "Queijo Prata";
                queijo.CodigoGrupoProduto = "2";
                queijo.DescricaoGrupoProduto = "Queijos";
                queijo.ValorUnitario = (decimal)5.12;
                queijo.PesoUnitario = decimal.Parse(txtPesoUnitQueijo.Text);
                queijo.Quantidade = decimal.Parse(txtQuantidadeQueijo.Text);
                listaProduto.Add(queijo);
            }

            if (ckbIogurte.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos iogurte = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                iogurte.CodigoProduto = "1569";
                iogurte.DescricaoProduto = "Iogurte Morango";
                iogurte.CodigoGrupoProduto = "3";
                iogurte.DescricaoGrupoProduto = "Resfriados";
                iogurte.ValorUnitario = (decimal)0.99;
                iogurte.PesoUnitario = decimal.Parse(txtPesoUnitIogurte.Text);
                iogurte.Quantidade = decimal.Parse(txtQuantidadeIogurte.Text);
                listaProduto.Add(iogurte);
            }

            if (ckbProdutoProsyst.Checked)
            {
                Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos produtoProsyst = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoProdutos();
                produtoProsyst.CodigoProduto = "2064";
                produtoProsyst.DescricaoProduto = "BEBIDA LACTEA FERM.MORANGO TIROL 900G";
                produtoProsyst.CodigoGrupoProduto = "4";
                produtoProsyst.DescricaoGrupoProduto = "Grupo Genérico";
                produtoProsyst.ValorUnitario = (decimal)1.9;
                produtoProsyst.PesoUnitario = decimal.Parse(txtPesoUnitProdutoProsyst.Text);
                produtoProsyst.Quantidade = decimal.Parse(txtQuantidadeProdutoProsyst.Text);
                listaProduto.Add(produtoProsyst);
            }



            Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido pedido = new Dominio.ObjetosDeValor.Embarcador.Pedido.IntegracaoPedido();

            if (rdbCuiaba.Checked)
            {
                pedido.Cliente = retornaClienteCuiaba();
            }

            if (rdbCuritiba.Checked)
            {
                pedido.Cliente = retornaClienteCuritiba();
            }
            if (rdbJoinvile.Checked)
            {
                pedido.Cliente = retornaClienteJoinville();
            }
            if (rdbPontaGrossa.Checked)
            {
                pedido.Cliente = retornaClientePontaGrossa();
            }
            if (rdbSaoPaulo.Checked)
            {
                pedido.Cliente = retornaClienteSaoPaulo();
            }
            if (rdbSaoPauloPotenza.Checked)
            {
                pedido.Cliente = retornaClientePotenza();
            }
            if (rbdSaoGoncalo.Checked)
            {
                pedido.Cliente = retornaClienteSaoGoncalo();
            }

            if (!string.IsNullOrWhiteSpace(txtCodigoCargaSubistituicao.Text))
            {
                pedido.CodigoCargaMultiEmbarcadorOrigemCancelada = int.Parse(txtCodigoCargaSubistituicao.Text);
            }


            pedido.CodigoIBGEOrigem = 4204202;
            pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.VendaNormal;

            if (rbdClienteTesteProsyst.Checked)
            {
                pedido.Cliente = retornaClienteTesteProsyst();
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.EntregaArmazem;
                pedido.CodigoIBGEOrigem = 4218509;
                pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";
            }

            if (rbdClienteTesteProsystErro.Checked)
            {
                pedido.Cliente = retornaClienteTesteProsyst();
                pedido.TipoOperacaoEmissao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao.EntregaArmazem;
                pedido.CodigoIBGEOrigem = 4218509;
                pedido.CodigoIBGEDestino = 8218509;
                pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";
            }
            else
            {
                pedido.CodigoIBGEDestino = pedido.Cliente.CodigoIBGECidade;
            }

            pedido.CodigoPedido = this.txtCodigoPedido.Text;
            pedido.CodigoCargaEmbarcador = this.txtCodigoCarga.Text;
            pedido.CodigoFilial = "1";
            pedido.CodigoFilialTomadora = "1";

            pedido.CodigoModeloVeicularEmbarcador = "";


            //pedido.CodigoModeloVeicularEmbarcador = "1";
            //pedido.DataPrevisaoEntrega = "04/12/2014 13:15:05";

            pedido.NumeroPaletes = 14;

            pedido.Recebedor = retornaOperador();

            pedido.ProdutosDoPedido = listaProduto;

            ServiceReference5.RetornoOfRetornoMetodoEntrada4PBTRYa8 retorno = WSpedido.AdicionarPedidoRefatoramentoMultiEmbarcador(pedido, int.Parse(this.txtNumeroCargaRefatura.Text), int.Parse(this.txtProtocoloRefatura.Text));

            if (retorno.Status)
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = "";
                lblNumeroCarga.Text = retorno.Objeto.CodigoCargaMultiEmbarcador.ToString();
                lblProtocolo.Text = retorno.Objeto.Protocolo.ToString();
            }
            else
            {
                lblData.Text = retorno.DataRetorno;
                lblMensagem.Text = retorno.Mensagem;
                lblNumeroCarga.Text = "0";
                lblProtocolo.Text = "0";
            }

        }

        protected void btnBuscarRateio_Click(object sender, EventArgs e)
        {
            int numeroProtocolo = int.Parse(this.txtProdutoParaRateio.Text);
            ServiceReference5.PedidosClient WSpedido = new ServiceReference5.PedidosClient();
            ServiceReference5.RetornoOfArrayOfRetornoRateioProduto4PBTRYa8 retorno = WSpedido.BuscarRateioPorPedido(numeroProtocolo);
            divRetornoRateio.InnerHtml = "";
            if (retorno.Status == true)
            {
                foreach (ServiceReference5.RetornoRateioProduto rateio in retorno.Objeto)
                {
                    divRetornoRateio.InnerHtml += "Carga :" + rateio.CodigoCargaMultiEmbarcador.ToString() + " <br/> Produto: " +
                        rateio.CodigoProdutoEmbarcador.ToString() + " <br/> Valor Frete:" + rateio.ValorTotalRateado.ToString("n2") + "<br/> Valor Unitário: " +
                        rateio.ValorUnitarioRateado + "<br/> Valor Unitário Produto: " + rateio.ValorUnitarioProduto + "<br/> Pedágio:" + rateio.ValorPedagio +
                        "<br/> Valor ICMS:" + rateio.ValorICMS + "<br/> Valor ICMS ST: " + rateio.ValorICMSST + " <br/> Peso:" + rateio.Peso + " <br/>" +
                        "Valor ADValorem Rateado: " + rateio.ValorADValoremRateado + "<br/> Valor Complemento de Frete Rateado :" + rateio.ValorComplementoFreteRateado +
                        "<br/> Valor Complemento ICMS Rateado: " + rateio.ValorComplementoICMSRateado + "<br/> Valor Descarga Rateado :" + rateio.ValorDescargaRateado +
                        "<br/> Valor Outros Rateado: " + rateio.ValorOutrosRateado + "<br/> Valor Pedagio Rateado :" + rateio.ValorPedagioRateado +
                        "<br/> Protocolo CTe: " + rateio.NumeroProtocoloCTe + "<br/> Protocolo NFs:" + rateio.NumeroProtocoloNFS +
                        "<br/><hr/>";
                }
            }
            else
            {
                divRetornoRateio.InnerHtml = retorno.Mensagem;
            }

        }

        protected void btnEnviarNovoWS_Click(object sender, EventArgs e)
        {

            string userToken = "4ed60154d2f04201ab8b57ed4198da3a";

            SGTCarga.CargasClient wsCarga = new SGTCarga.CargasClient();
            Servicos.InspectorBehavior inspector = new Servicos.InspectorBehavior();
            wsCarga.Endpoint.EndpointBehaviors.Add(inspector);
            OperationContextScope scope = new OperationContextScope(wsCarga.InnerChannel);
            MessageHeader header = MessageHeader.CreateHeader("Token", "Token", userToken);
            OperationContext.Current.OutgoingMessageHeaders.Add(header);

            SGTCarga.CargaIntegracao cargaIntegracao = MontarCargaNovoWS();

            SGTCarga.RetornoOfProtocolosVQbIXuKl retornoCarga = wsCarga.AdicionarCarga(cargaIntegracao);

            if (retornoCarga.Status)
            {
                this.lblMensagem.Text = retornoCarga.Objeto.protocoloIntegracaoCarga.ToString();
            }
            else
            {
                this.lblMensagem.Text = retornoCarga.Mensagem;
            }
        }
    }
}