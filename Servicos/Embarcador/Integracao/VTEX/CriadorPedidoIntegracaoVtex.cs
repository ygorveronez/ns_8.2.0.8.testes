using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;
using System.Text.RegularExpressions;

namespace Servicos.Embarcador.Integracao.VTEX
{
    /// <summary>
    /// Classe que cria os pedidos na integração com a VTEX
    /// </summary>
    class CriadorPedidoIntegracaoVtex
    {
        Repositorio.UnitOfWork unitOfWork;
        AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin;

        public CriadorPedidoIntegracaoVtex(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Repositorio.UnitOfWork unitOfWorkAdmin)
        {
            this.unitOfWorkAdmin = unitOfWorkAdmin;
            this.unitOfWork = unitOfWork;
        }

        public void CriarPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, Dominio.Entidades.Embarcador.Integracao.PedidoAguardandoIntegracao pedidoAguardandoIntegracao, out string informacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas)
        {
            informacao = "";
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);

            TipoPedidoVtex tipoPedidoVtex = ObterTipoPedidoVtex(dadosPedido);

            var pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido();
            pedido.NumeroPedidoEmbarcador = dadosPedido.orderId;
            pedido.NumeroOrdem = dadosPedido.sellerOrderId;
            pedido.DataCriacao = DateTime.Now;
            pedido.Filial = pedidoAguardandoIntegracao.Filial;
            pedido.SituacaoPedido = SituacaoPedido.Aberto;
            //pedido.ValorFreteCobradoCliente = dadosPedido.shippingData.logisticsInfo[0].listPrice;
            pedido.ValorFreteNegociado = dadosPedido.shippingData.logisticsInfo[0].listPrice;
            pedido.ValorFreteInformativo = dadosPedido.shippingData.logisticsInfo[0].listPrice;
            pedido.PesoTotal = ObterPesoPedido(dadosPedido);
            pedido.TipoDeCarga = repTipoDeCarga.BuscarPorCodigoEmbarcador("1");
            pedido.TipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao("ECM_VTEX");
            pedido.PrevisaoEntrega = dadosPedido.shippingData.logisticsInfo.Count > 0 ? dadosPedido.shippingData.logisticsInfo[0].shippingEstimateDate : null;
            pedido.UsarTipoTomadorPedido = true;
            pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Remetente;
            pedido.DataCriacaoPedidoERP = dadosPedido.creationDate;

            PreencherDadosRemetente(pedido, pedidoAguardandoIntegracao.Filial, dadosPedido, tipoPedidoVtex);
            PreencherDadosRecebedor(pedido, dadosPedido, tipoPedidoVtex);
            PreencherDadosDestinatario(pedido, dadosPedido);
            PreencherDadosTransportador(pedido, dadosPedido, tipoPedidoVtex);
            PreencherDadosCanalEntrega(pedido, dadosPedido, tipoPedidoVtex);
            PreencherDadosCanalVenda(pedido, dadosPedido);
            PreencherDadosDoca(pedido, dadosPedido, docas);

            //caso pedido RNLNormal deve ter a filial diferente
            if (tipoPedidoVtex == TipoPedidoVtex.RNLNormal)
            {
                if (dadosPedido.sellers.Count > 0)
                {
                    Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(dadosPedido.sellers[0].id);
                    if (filial != null)
                        pedido.Filial = filial;
                }
            }

            repPedido.Inserir(pedido);
            pedido.Protocolo = pedido.Codigo;
            repPedido.Atualizar(pedido);

            CriarPedidosProduto(pedido, dadosPedido);

            if ((pedido.CanalEntrega?.GerarCargaAutomaticamente) ?? false)
            {
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                pedidos.Add(pedido);

                Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                string mensagemErroCriarCarga = Servicos.Embarcador.Pedido.Pedido.CriarCarga(out Dominio.Entidades.Embarcador.Cargas.Carga carga, pedidos, unitOfWork, tipoServicoMultisoftware, null, configuracao, true, true);

                if (!string.IsNullOrWhiteSpace(mensagemErroCriarCarga))
                {
                    Servicos.Log.TratarErro("falha" + mensagemErroCriarCarga, "CargaCanalEntrega");
                    throw new ServicoException(mensagemErroCriarCarga);
                }


                repositorioPedido.Atualizar(pedido);
            }

            informacao = $"Pedido {dadosPedido.orderId} criado com sucesso. Tipo de pedido: {tipoPedidoVtex.ObterDescricao()}";
        }

        private void PreencherDadosDoca(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido PedidoVtex, List<Dominio.Entidades.Embarcador.Integracao.IntegracaoDocas> docas)
        {
            if (PedidoVtex.shippingData != null)
                foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.LogisticsInfo logisticsInfo in PedidoVtex.shippingData.logisticsInfo)
                    if (logisticsInfo.slas != null)
                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Sla slas in logisticsInfo.slas)
                            pedido.NumeroDoca = slas.pickupStoreInfo?.dockId ?? string.Empty;

            if (docas != null)            
                pedido.TempoDeDoca = docas.Where(X => X.CodigoIntegracao == pedido.NumeroDoca)?.FirstOrDefault()?.TempoMedioCarregamento ?? 0;
        }

        private TipoPedidoVtex ObterTipoPedidoVtex(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido)
        {
            string deliveryChannel = dadosPedido.shippingData.logisticsInfo[0].deliveryChannel;
            string selectedSla = dadosPedido.shippingData.logisticsInfo[0].selectedSla;
            string seller = dadosPedido.items[0].seller;
            string affiliateId = dadosPedido.affiliateId;

            // Ship from stores
            if ((deliveryChannel != "pickup-in-point" && seller.Contains("decathlon")) || selectedSla == "Ship From stores" || selectedSla == "Rápida")
            {
                return TipoPedidoVtex.ShipFromStores;
            }

            // RNL Normal
            if (deliveryChannel == "pickup-in-point" && seller == "1" && dadosPedido.shippingData.logisticsInfo[0].deliveryCompany != "Clique Retire")
            {
                return TipoPedidoVtex.RNLNormal;
            }

            // Clique Retire
            if (deliveryChannel == "pickup-in-point" && seller == "1" && dadosPedido.shippingData.logisticsInfo[0].deliveryCompany == "Clique Retire")
            {
                return TipoPedidoVtex.CliqueRetire;
            }

            // RNL Express
            if (deliveryChannel == "pickup-in-point" && seller.Contains("decatlhon"))
            {
                return TipoPedidoVtex.RNLExpress;
            }

            // Market Place IN
            if (seller != "1" && !seller.ToLower().Contains("decatlhon"))
            {
                return TipoPedidoVtex.MarketPlaceIn;
            }

            // Market Place OUT
            if (!string.IsNullOrEmpty(affiliateId))
            {
                return TipoPedidoVtex.MarketPlaceOut;
            }

            // E-commerce normal
            return TipoPedidoVtex.ECommerceNormal;
        }

        private decimal ObterPesoPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido)
        {
            return (from o in dadosPedido.items select o.additionalInfo?.dimension?.weight ?? 0).Sum();
        }

        private void PreencherDadosRemetente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Filiais.Filial filial, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, TipoPedidoVtex tipoPedidoVtex)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);

            Dominio.Entidades.Cliente Filial = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(filial.CNPJ));
            Dominio.Entidades.Cliente remetente = Filial;

            if (tipoPedidoVtex == TipoPedidoVtex.ShipFromStores)
            {
                var codigo = dadosPedido.sellers[0]?.id;

                // Testando para ver se não tem um UUID concatenado no código. Se tiver, tem que tirar ele;
                Regex re = new Regex(@"(.+) \(.+\)");
                var match = re.Match(codigo);
                if (match.Success)
                {
                    codigo = match.Groups[1].Value;
                }

                remetente = repositorioCliente.BuscarPorCodigoIntegracaoVtex(codigo);

                if (remetente == null)
                {
                    throw new ServicoException($"Não foi possível encontrar o remetente do pedido no campo sellers (Id {codigo})");
                }

                pedido.Remetente = remetente;
                pedido.Origem = remetente.Localidade;

                pedido.Tomador = Filial;
                pedido.TipoTomador = Dominio.Enumeradores.TipoTomador.Outros;
            }
            else
            {
                if (remetente == null)
                {
                    throw new ServicoException($"Não foi possível encontrar o remetente do pedido (CNPJ {filial.CNPJ_Formatado})");
                }

                pedido.Remetente = remetente;
                pedido.Origem = remetente.Localidade;
            }
        }

        private void PreencherDadosRecebedor(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, TipoPedidoVtex tipoPedidoVtex)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Cliente recebedor = null;

            if (tipoPedidoVtex == TipoPedidoVtex.RNLNormal || tipoPedidoVtex == TipoPedidoVtex.CliqueRetire)
            {
                // Se for RNL Normal, o destinatário é uma filial da Decatlhon
                var codigo = dadosPedido.shippingData.logisticsInfo[0].selectedSla;

                // Testando para ver se não tem um UUID concatenado no código. Se tiver, tem que tirar ele;
                Regex re = new Regex(@"(.+) \(.+\)");
                var match = re.Match(codigo);
                if (match.Success)
                {
                    codigo = match.Groups[1].Value;
                }

                recebedor = repositorioCliente.BuscarPorCodigoIntegracaoVtex(codigo);

                if (recebedor == null) //#48189 busca pelo selectedSla se nao achar pega pelo deliveryCompany
                {
                    var codigodelivery = dadosPedido.shippingData.logisticsInfo[0].deliveryCompany;
                    var matchdelivery = re.Match(codigodelivery);
                    if (matchdelivery.Success)
                    {
                        codigodelivery = match.Groups[1].Value;
                    }

                    recebedor = repositorioCliente.BuscarPorCodigoIntegracaoVtex(codigodelivery);
                }
            }

            pedido.Recebedor = recebedor;
        }

        private void PreencherDadosDestinatario(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            try
            {
                Dominio.Entidades.Cliente destinatario;

                if (dadosPedido.clientProfileData.isCorporate)
                    destinatario = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(dadosPedido.clientProfileData.corporateDocument));
                else
                    destinatario = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(dadosPedido.clientProfileData.document));

                if (destinatario == null)
                {
                    //FEITO PARA GARANTIR Q VAI BUSCAR OU PELO corporateDocument ou document
                    if (!string.IsNullOrEmpty(dadosPedido.clientProfileData.corporateDocument))
                        destinatario = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(dadosPedido.clientProfileData.corporateDocument));
                    else if (!string.IsNullOrEmpty(dadosPedido.clientProfileData.document))
                        destinatario = repositorioCliente.BuscarPorCPFCNPJ(double.Parse(dadosPedido.clientProfileData.document));
                }

                destinatario = CriarDestinatario(destinatario, dadosPedido.clientProfileData, dadosPedido.shippingData.address);

                pedido.Destinatario = destinatario;
                pedido.Destino = destinatario.Localidade;
            }
            catch (ServicoException ex)
            {
                throw new ServicoException(ex.Message);
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex.Message);
                throw new ServicoException($"Não foi possível encontrar o destinatario do pedido (CNPJ {dadosPedido.clientProfileData?.document})");
            }

        }

        private void PreencherDadosTransportador(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, TipoPedidoVtex tipoPedidoVtex)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);

            Dominio.Entidades.Empresa transportador = null;

            if (tipoPedidoVtex == TipoPedidoVtex.RNLNormal || tipoPedidoVtex == TipoPedidoVtex.RNLExpress)
            {
                transportador = null;
            }
            else
            {
                transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(dadosPedido.shippingData.logisticsInfo[0].deliveryCompany);

                if (transportador == null || dadosPedido.shippingData.logisticsInfo[0].deliveryCompany == null)
                {
                    transportador = repositorioEmpresa.BuscarPorCodigoIntegracao(dadosPedido.shippingData.logisticsInfo[0].selectedSla);

                    if (transportador == null)
                    {
                        throw new ServicoException($"Não foi possível encontrar o transportador do pedido - deliveryCompany \"{dadosPedido.shippingData.logisticsInfo[0].deliveryCompany}\" ou selectedSla \" {dadosPedido.shippingData.logisticsInfo[0].selectedSla} \" no codigo integração");
                    }
                }
            }

            pedido.Empresa = transportador;
        }

        private void PreencherDadosCanalEntrega(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido, TipoPedidoVtex tipoPedidoVtex)
        {
            Repositorio.Embarcador.Pedidos.CanalEntrega repCanalEntrega = new Repositorio.Embarcador.Pedidos.CanalEntrega(unitOfWork);

            switch (tipoPedidoVtex)
            {
                case TipoPedidoVtex.ShipFromStores:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Ship_from_stores");
                    break;
                case TipoPedidoVtex.RNLNormal:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("RNL_Normal");
                    break;
                case TipoPedidoVtex.RNLExpress:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("RNL_Express");
                    break;
                case TipoPedidoVtex.MarketPlaceIn:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Market_Place_IN");
                    break;
                case TipoPedidoVtex.MarketPlaceOut:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Market_Place_OUT");
                    break;
                case TipoPedidoVtex.ECommerceNormal:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Commerce_Normal");
                    break;
                case TipoPedidoVtex.CliqueRetire:
                    pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Clique_Retire");
                    break;
            }

            string deliveryCompany = dadosPedido.shippingData?.logisticsInfo[0]?.deliveryCompany ?? "";
            if (deliveryCompany == "Clique Retire")
                pedido.CanalEntrega = repCanalEntrega.BuscarPorCodigoIntegracao("Clique_Retire");

        }

        private void PreencherDadosCanalVenda(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido)
        {
            Repositorio.Embarcador.Pedidos.CanalVenda repositorioCanalVenda = new Repositorio.Embarcador.Pedidos.CanalVenda(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.CanalVenda canalVenda = repositorioCanalVenda.BuscarPorCodigoIntegracao(dadosPedido?.salesChannel ?? string.Empty);

            if (canalVenda != null)
                pedido.CanalVenda = canalVenda;
        }

        private Dominio.Entidades.Cliente CriarDestinatario(Dominio.Entidades.Cliente destinatario, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.ClientProfileData dadosDestinatario, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Address address)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

            bool inserir = false;
            if (destinatario == null)
            {
                destinatario = new Dominio.Entidades.Cliente();
                destinatario.CPF_CNPJ = dadosDestinatario.isCorporate ? double.Parse(dadosDestinatario.corporateDocument.Replace(".", "").Replace("-", "")) : double.Parse(dadosDestinatario.document.Replace(".", "").Replace("-", ""));
                inserir = true;
            }

            destinatario.Nome = dadosDestinatario.firstName + " " + dadosDestinatario.lastName;

            if (destinatario.Nome.Length > 80)
                destinatario.Nome = destinatario.Nome.Substring(0, 79);

            destinatario.Telefone1 = dadosDestinatario.phone;
            destinatario.Email = dadosDestinatario.email;
            destinatario.Tipo = dadosDestinatario.isCorporate ? "J" : "F";
            destinatario.Ativo = true;

            // Criar dados de localização do destinatario
            destinatario.CEP = address.postalCode.Replace("-", "").Replace(".", "");

            var localidade = repLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveDiacritics(address.city.ToUpper()), address.state.ToUpper());

            if (localidade == null)
                localidade = repLocalidade.BuscarPorDescricaoEEstado(Utilidades.String.RemoveDiacritics(address.city.ToUpper()), address.state.ToUpper());

            if (localidade == null)
                localidade = repLocalidade.BuscarPorCEP(destinatario.CEP);

            if (localidade == null)
            {
                Servicos.Embarcador.Localidades.Localidade svcLocalidade = new Servicos.Embarcador.Localidades.Localidade(unitOfWork);
                dynamic endereco = svcLocalidade.BuscarEnderecoPorCEP(Utilidades.String.OnlyNumbers(destinatario.CEP), unitOfWork, unitOfWorkAdmin);
                if (endereco != null)
                    localidade = repLocalidade.BuscarPorCodigo(endereco.CodigoCidade);
            }


            if (localidade == null)
            {
                throw new ServicoException($"Não foi possível encontrar a localidade do destinatário {Utilidades.String.RemoveDiacritics(address.city.ToUpper())} - {address.state.ToUpper()} - CEP: {address.postalCode}");
            }
            else
                destinatario.Localidade = localidade;

            destinatario.Endereco = address.street;
            destinatario.Numero = address.number;
            destinatario.Bairro = address.neighborhood;
            destinatario.Complemento = address.complement?.ToString();

            if (destinatario.Endereco?.Length > 120)
            {
                destinatario.Endereco = destinatario.Endereco.Substring(0, 120);
            }

            if (destinatario.Complemento?.Length > 60)
            {
                destinatario.Complemento = destinatario.Complemento.Substring(0, 60);
            }

            if (destinatario.Bairro?.Length > 40)
            {
                destinatario.Bairro = destinatario.Bairro.Substring(0, 40);
            }

            if (destinatario.Numero?.Length > 40)
            {
                destinatario.Numero = destinatario.Numero.Substring(0, 40);
            }

            destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
            if (address.geoCoordinates.Count == 2)
            {
                destinatario.Latitude = address.geoCoordinates[0].ToString();
                destinatario.Longitude = address.geoCoordinates[1].ToString();
            }

            if (inserir)
                repositorioCliente.Inserir(destinatario);
            else
                repositorioCliente.Atualizar(destinatario);

            return destinatario;
        }

        private void CriarPedidosProduto(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Pedido dadosPedido)
        {
            Repositorio.Embarcador.Pedidos.PedidoProduto repPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            foreach (var item in dadosPedido.items)
            {
                var produto = repProdutoEmbarcador.buscarPorCodigoEmbarcador(item.productId);

                if (produto == null)
                {
                    produto = CriarProduto(item);
                }

                var pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto();
                pedidoProduto.Produto = produto;
                pedidoProduto.Pedido = pedido;
                pedidoProduto.ValorProduto = item.sellingPrice;
                pedidoProduto.Quantidade = item.quantity;
                pedidoProduto.PesoUnitario = (item.additionalInfo?.dimension?.weight ?? 0);

                repPedidoProduto.Inserir(pedidoProduto);
            }
        }

        private Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador CriarProduto(Dominio.ObjetosDeValor.Embarcador.Integracao.VTEX.Item item)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            var produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador
            {
                CodigoProdutoEmbarcador = !string.IsNullOrEmpty(item.refId) ? item.refId : item.productId,
                Descricao = item.name,
                Integrado = false
            };

            repProdutoEmbarcador.Inserir(produto);

            return produto;
        }
    }
}