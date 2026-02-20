using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Servicos.Embarcador.Integracao.FTPAmazon
{
    public sealed class FTPAmazon
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon _configuracaoFTP;

        #endregion

        #region Construtores

        public FTPAmazon(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();
        }

        #endregion

        #region Métodos Públicos

        public void IniciarProcessamento()
        {
            IEnumerable<string> arquivos = ObterArquivos();

            if (arquivos?.Any() ?? false)
                GerarRegistroEDI(arquivos);

            ProcessarArquivos();
        }

        public void GerarRegistroEDI(string arquivo)
        {
            GerarRegistroEDI(new List<string> { arquivo });
        }

        #endregion

        #region Métodos Privados - Geração de Documentos

        private Dominio.Entidades.Embarcador.Cargas.Carga IniciarGeracoesCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xmlDesserializado, string nomeArquivo)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = GerarPedido(xmlDesserializado, nomeArquivo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            string mensagemRetornoCarga = Pedido.Pedido.CriarCarga(out carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido> { pedido }, _unitOfWork, _tipoServicoMultisoftware, null, _configuracaoEmbarcador, true, false, false, false);

            Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorPedido(pedido.Codigo).FirstOrDefault();

            if (nomeArquivo.Contains("Manifesto - Transhipment"))
            {
                cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.SubContratada;
                InserirCTeTerceiro(ObterCTeTerceiro(xmlDesserializado, cargaPedido), cargaPedido);
            }

            else if (nomeArquivo.Contains("Manifesto - AMZL"))
            {
                cargaPedido.TipoContratacaoCarga = TipoContratacaoCarga.RedespachoIntermediario;
                InserirCTeTerceiro(ObterCTeTerceiro(xmlDesserializado, cargaPedido, utilizaPacotes: true), cargaPedido);
            }

            else if (nomeArquivo.Contains("Manifesto - Direct Injection"))
                InserirNotasFiscais(ObterNotasFiscais(xmlDesserializado, utilizaPacotes: true, cargaPedido), cargaPedido);


            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                throw new ServicoException(mensagemRetornoCarga);

            return carga;
        }

        private Dominio.Entidades.Embarcador.Pedidos.Pedido GerarPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xml, string nomeArquivo)
        {
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);


            Dominio.Entidades.Veiculo veiculo = ObterVeiculo(xml.AmazonManifest?.ManifestHeader?.TrailerName.Right(7) ?? string.Empty);
            Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacao = new Dominio.Entidades.Embarcador.Pedidos.TipoOperacao() { };

            if (nomeArquivo.Contains("Manifesto - Transhipment"))
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao("transhipment");

            else if (nomeArquivo.Contains("Manifesto - AMZL"))
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao("injecao-amzl");

            else if (nomeArquivo.Contains("Manifesto - Direct Injection"))
                tipoOperacao = repTipoOperacao.BuscarPorCodigoIntegracao("direct-injection");

            if (veiculo == null)
                throw new ServicoException($"Veículo placa: {xml.AmazonManifest?.ManifestHeader?.TrailerName.Right(7)} não encontrado");

            if (tipoOperacao == null)
                throw new ServicoException("Tipo de Operação não encontrado");

            Pedido.Pedido servicoPedido = new Pedido.Pedido();

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                CodigoCargaEmbarcador = xml.AmazonManifest?.ManifestHeader?.LoadReferenceID,
                Veiculos = new List<Dominio.Entidades.Veiculo>() { veiculo },
                Empresa = ObterTransportador(xml.AmazonManifest?.ManifestHeader?.CarrierInternalID ?? string.Empty),
                Filial = ObterFilial(xml),
                Remetente = repCliente.BuscarPorNomeVisaoBI(xml.AmazonManifest?.ManifestHeader?.WarehouseLocationID ?? string.Empty),
                Destinatario = repCliente.BuscarPorNomeVisaoBI(xml.AmazonManifest?.ShipmentDetail?.FirstOrDefault()?.DestinationWarehouseLocationID ?? string.Empty),
                Origem = repLocalidade.BuscarPorCEP((xml.AmazonManifest?.ManifestHeader?.ShipFromAddress?.PostalCode ?? string.Empty).Replace("-", "")),
                ModeloVeicularCarga = veiculo?.ModeloVeicularCarga,
                Motoristas = veiculo?.Motoristas?.Select(o => o.Motorista)?.ToList(),
                TipoDeCarga = veiculo?.ModeloVeicularCarga?.TiposCarga?.FirstOrDefault()?.TipoDeCarga,
                TipoOperacao = tipoOperacao,
                Numero = repositorioPedido.BuscarProximoNumero(),
                NumeroPedidoEmbarcador = xml.AmazonManifest?.ManifestHeader?.LoadReferenceID
            };

            if (pedido.Remetente != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoOrigem = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoOrigem, pedido.Remetente);

                if (pedidoEnderecoOrigem.Localidade != null)
                {
                    repositorioPedidoEndereco.Inserir(pedidoEnderecoOrigem);
                    pedido.EnderecoOrigem = pedidoEnderecoOrigem;
                    pedido.Origem = pedidoEnderecoOrigem.Localidade;
                }
            }

            if (pedido.Destinatario != null)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco pedidoEnderecoDestino = new Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco();
                servicoPedido.PreecherEnderecoPedido(ref pedidoEnderecoDestino, pedido.Destinatario);

                if (pedidoEnderecoDestino.Localidade != null)
                {
                    repositorioPedidoEndereco.Inserir(pedidoEnderecoDestino);
                    pedido.EnderecoDestino = pedidoEnderecoDestino;
                    pedido.Destino = pedidoEnderecoDestino.Localidade;
                }
            }

            repositorioPedido.Inserir(pedido);

            pedido.Protocolo = pedido.Codigo;

            repositorioPedido.Atualizar(pedido);

            return pedido;
        }

        #endregion

        #region Métodos Privados - Buscar Dados
        private Dominio.Entidades.Veiculo ObterVeiculo(string placa)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            return repVeiculo.BuscarPorPlaca(placa);
        }

        private Dominio.Entidades.Empresa ObterTransportador(string codigo)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            return repEmpresa.BuscarPorCodigoIntegracao(codigo);
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xml)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigoIntegracao(xml.AmazonManifest?.ManifestHeader?.WarehouseLocationID ?? string.Empty);

            return filial;
        }

        private Dominio.Entidades.ParticipanteCTe ObterParticipanteCTe(double cpfCnpj)
        {
            Repositorio.Cliente cliente = new Repositorio.Cliente(_unitOfWork);

            return new Servicos.Cliente(_unitOfWork.StringConexao).ConverterClienteParaParticipanteCTe(cliente.BuscarPorCPFCNPJ(cpfCnpj)); ;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> ObterNotasFiscais(Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xml, bool utilizaPacotes, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNFe = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNFes = new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPacotes = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            if (xml?.AmazonManifest?.ShipmentDetail == null || xml.AmazonManifest.ShipmentDetail.Count == 0)
                return new List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal>();

            foreach (var obj in xml?.AmazonManifest?.ShipmentDetail)
            {
                if (obj == null || obj?.BrNFes == null || obj.BrNFes.Count == 0)
                    continue;

                foreach (var nfe in obj?.BrNFes)
                {

                    Dominio.Entidades.Cliente emitente = repCliente.BuscarPorCPFCNPJ(nfe?.SellerCNPJ ?? 0);
                    Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(nfe?.CustomerCNPJCPF ?? 0);

                    if (emitente == null)
                        throw new ServicoException($"Emitente da NF-e: número {nfe.NFeNumber} não encontrado");

                    if (destinatario == null)
                        throw new ServicoException($"Destinatário da NF-e: número {nfe.NFeNumber} não encontrado");

                    listaNFes.Add(new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal()
                    {
                        Emitente = repCliente.BuscarPorCPFCNPJ(nfe?.SellerCNPJ ?? 0),
                        Destinatario = repCliente.BuscarPorCPFCNPJ(nfe?.CustomerCNPJCPF ?? 0),
                        Numero = nfe.NFeNumber,
                        Serie = nfe?.NFeSeries,
                        DataEmissao = DateTime.Parse(nfe.NFeIssuanceDate).ToLocalTime(),
                        Chave = nfe.NFeAccessCode,
                        ValorST = nfe.NFeICMSSTAmount,
                        ValorICMS = nfe.NFeICMSAmount,
                        Valor = nfe.NFeTotalValue,
                        ValorTotalProdutos = nfe.NFeProductsTotalValue,
                        CNPJTranposrtador = nfe?.SellerCNPJ.ToString(),
                        PlacaVeiculoNotaFiscal = xml.AmazonManifest?.ManifestHeader?.TrailerName.Split(' ')[1],
                        XML = string.Empty,
                    });
                }

                Dominio.Entidades.Embarcador.Cargas.Pacote pacote = new Dominio.Entidades.Embarcador.Cargas.Pacote()
                {
                    LogKey = obj.ShipmentPackageInfo?.CartonID?.TrackingID ?? string.Empty,
                    Peso = obj.ShipmentPackageInfo?.ShipmentPackageDeclaredGrossWeight?.WeightValue ?? 0,
                    Origem = cargaPedido.Pedido.Remetente,
                    Destino = cargaPedido.Pedido.Destinatario,
                    Contratante = cargaPedido.Pedido.Tomador,
                    DataRecebimento = DateTime.Now
                };
                repPacote.Inserir(pacote);

                repCargaPedidoPacote.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote()
                {
                    CargaPedido = cargaPedido,
                    Pacote = pacote
                });
            }

            return listaNFes;
        }

        private List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> ObterCTeTerceiro(Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xml, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido, bool utilizaPacotes = false)
        {
            Repositorio.Embarcador.Cargas.Pacote repPacote = new Repositorio.Embarcador.Cargas.Pacote(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repCTe = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoPacote repCargaPedidoPacote = new Repositorio.Embarcador.Cargas.CargaPedidoPacote(_unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(_unitOfWork);

            List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> listaCTes = new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote> cargaPacotes = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote>();

            if (xml?.AmazonManifest?.ShipmentDetail == null || xml.AmazonManifest.ShipmentDetail.Count == 0)
                return new List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro>();

            foreach (var obj in xml?.AmazonManifest?.ShipmentDetail)
            {
                if (obj?.BrNFes == null || obj?.BrNFes.Count == 0)
                    continue;

                BrNFe primeiraNFe = obj.BrNFes[0];

                if (primeiraNFe.NFeSeries == null)
                    continue;

                listaCTes.Add(new Dominio.Entidades.Embarcador.CTe.CTeTerceiro()
                {
                    Numero = utilizaPacotes ? obj.BrCTe.CTeNumber : primeiraNFe.NFeNumber,
                    Serie = primeiraNFe.NFeSeries,
                    DataEmissao = utilizaPacotes ? DateTime.Parse(obj.BrCTe.CTeIssuanceDate).ToLocalTime() : DateTime.Parse(primeiraNFe.NFeIssuanceDate).ToLocalTime(),
                    ChaveAcesso = utilizaPacotes ? obj.BrCTe.CTeAccessCode : primeiraNFe.NFeAccessCode,
                    TransportadorTerceiro = repCliente.BuscarPorCPFCNPJ(primeiraNFe?.SellerCNPJ ?? 0),
                    Remetente = ObterParticipanteCTe(primeiraNFe?.SellerCNPJ ?? 0),
                    Destinatario = ObterParticipanteCTe(primeiraNFe?.CustomerCNPJCPF ?? 0),
                    ValorTotalMercadoria = primeiraNFe?.NFeProductsTotalValue ?? 0,
                    CFOP = repCFOP.BuscarPorCodigo(6),
                    IdentifacaoPacote = utilizaPacotes ? obj.ShipmentPackageInfo?.CartonID?.TrackingID ?? null : null,
                });

                if (!utilizaPacotes)
                    continue;

                Dominio.Entidades.Embarcador.Cargas.Pacote pacote = new Dominio.Entidades.Embarcador.Cargas.Pacote()
                {
                    LogKey = obj.ShipmentPackageInfo?.CartonID?.TrackingID ?? string.Empty,
                    Peso = obj.ShipmentPackageInfo?.ShipmentPackageDeclaredGrossWeight?.WeightValue ?? 0,
                    Origem = cargaPedido.Pedido.Remetente,
                    Destino = cargaPedido.Pedido.Destinatario,
                    Contratante = cargaPedido.Pedido.Tomador,
                    DataRecebimento = DateTime.Now
                };
                repPacote.Inserir(pacote);

                repCargaPedidoPacote.Inserir(new Dominio.Entidades.Embarcador.Cargas.CargaPedidoPacote()
                {
                    CargaPedido = cargaPedido,
                    Pacote = pacote
                });
            }

            return listaCTes;
        }

        #endregion

        #region Métodos Privados

        private void GerarRegistroEDI(IEnumerable<string> arquivos)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork);

            foreach (string nomeArquivo in arquivos)
            {
                string fileName = Path.GetFileName(nomeArquivo);
                string fileExtension = Path.GetExtension(nomeArquivo);

                try
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                        continue;

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();
                    controleIntegracaoCargaEDI.Data = DateTime.Now;
                    controleIntegracaoCargaEDI.MensagemRetorno = "";
                    controleIntegracaoCargaEDI.NumeroDT = "";
                    controleIntegracaoCargaEDI.NomeArquivo = fileName;
                    controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + fileExtension;
                    controleIntegracaoCargaEDI.NumeroTentativas = 0;
                    controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao;

                    repositorioControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);

                    _unitOfWork.CommitChanges();

                    try
                    {
                        MoverArquivoPastaProcessados(nomeArquivo);
                    }
                    catch (Exception ex)
                    {
                        repositorioControleIntegracaoCargaEDI.Deletar(controleIntegracaoCargaEDI);
                    }
                }
                catch (Exception excecao)
                {
                    Log.TratarErro(excecao);
                    _unitOfWork.Rollback();
                    continue;
                }
            }
        }

        private void MoverArquivoPastaProcessados(string arquivo)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon configuracaoFTP = ObterConfiguracaoFTP();

            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");
            string nomeArquivo = Path.GetFileName(arquivo);
            string extensaoArquivo = Path.GetExtension(arquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo)))
            {
                int numeroArquivosComMesmoNome = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho).Where(x => x.Contains(nomeArquivo)).Count();
                nomeArquivo = $"{Path.GetFileNameWithoutExtension(nomeArquivo)}_{numeroArquivosComMesmoNome}{extensaoArquivo}";
            }

            Utilidades.IO.FileStorageService.Storage.Move(arquivo, Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo));
        }

        private IEnumerable<string> ObterArquivos()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon configuracaoFTP = ObterConfiguracaoFTP();

            if (configuracaoFTP == null)
                return null;

            string diretorioDownload = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, configuracaoFTP.DiretorioDownload);

            string erro = "";
            Servicos.FTP.DownloadArquivosPasta(configuracaoFTP.EnderecoFTP, configuracaoFTP.Porta, configuracaoFTP.Diretorio, configuracaoFTP.Usuario, configuracaoFTP.Senha, configuracaoFTP.Passivo, configuracaoFTP.SSL, diretorioDownload, out erro, configuracaoFTP.UtilizarSFTP, false, "", true, false, false, true, nomeArquivoComEspaco: true);

            if (!string.IsNullOrWhiteSpace(erro))
            {
                Log.TratarErro(erro);

                return null;
            }

            return Utilidades.IO.FileStorageService.Storage.GetFiles(diretorioDownload, "*.xml", SearchOption.TopDirectoryOnly);
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon ObterConfiguracaoFTP()
        {
            if (_configuracaoFTP != null)
                return _configuracaoFTP;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPAmazon repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPAmazon(_unitOfWork);

            _configuracaoFTP = repositorio.BuscarPrimeiroRegistro();

            return _configuracaoFTP;
        }

        private void ProcessarArquivos()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPAmazon configuracaoFTP = ObterConfiguracaoFTP();
            if (configuracaoFTP == null)
                return;

            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracao = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI> controles = repositorioControleIntegracao.BuscarPendenteIntegracao(0, 5);

            string caminhoRaiz = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");

            foreach (Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controle in controles)
            {

                _unitOfWork.Start();

                controle.NumeroTentativas++;
                repositorioControleIntegracao.Atualizar(controle);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

                _unitOfWork.CommitChanges();

                string arquivo = controle.NomeArquivo;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, arquivo)))
                {
                    controle.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                    controle.MensagemRetorno = Localization.Resources.Cargas.ControleGeracaoEDI.ArquivoNaoEncontrado;

                    repositorioControleIntegracao.Atualizar(controle);

                    continue;
                }

                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoRaiz, arquivo);
                System.IO.MemoryStream stream = new MemoryStream(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCompleto));

                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1")))//Encoding.GetEncoding("ISO-8859-1")
                {
                    string arquivoXml = reader.ReadToEnd();

                    arquivoXml = RemoverTags(arquivoXml);

                    bool sucesso = false;

                    Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message xmlDeserializado = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.Integracao.FTPAmazon.Message>(arquivoXml);

                    if (xmlDeserializado == null)
                        continue;

                    xmlDeserializado = ObterXmlDeserializadoCompleto(arquivoXml, xmlDeserializado, arquivo);

                    try
                    {
                        _unitOfWork.Start();

                        carga = IniciarGeracoesCarga(xmlDeserializado, arquivo);

                        sucesso = true;

                        _unitOfWork.CommitChanges();
                    }
                    catch (ServicoException serexcecao)
                    {
                        _unitOfWork.Rollback();
                        Log.TratarErro(serexcecao);
                        sucesso = false;
                        controle.MensagemRetorno = serexcecao.Message;
                    }
                    catch (BaseException excecao) when (excecao.ErrorCode == Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RejeicaoCancelamentoCarga)
                    {
                        sucesso = true;
                        controle.MensagemRetorno = excecao.Message;

                        _unitOfWork.CommitChanges();
                    }
                    catch (BaseException excecao)
                    {
                        _unitOfWork.Rollback();
                        Log.TratarErro(excecao);
                        sucesso = false;
                        controle.MensagemRetorno = excecao.Message;
                    }

                    catch (Exception excecao)
                    {
                        _unitOfWork.Rollback();
                        Log.TratarErro(excecao);
                        sucesso = false;
                        controle.MensagemRetorno = "Ocorreu um erro ao realizar a operação";
                    }
                    if (!sucesso)
                        controle.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                    else
                    {
                        controle.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                        controle.MensagemRetorno = "Integrado com sucesso";
                        controle.Transportador = carga.Empresa;
                        controle.Placa = carga.Veiculo.Placa_Formatada;
                    }
                }

                repositorioControleIntegracao.Atualizar(controle);
            }
        }

        private static string RemoverTags(string xml)
        {
            int indexFinal = xml.IndexOf('>', 50);

            if (!string.IsNullOrEmpty(xml) && xml.Contains("xmlns:xsi"))
            {
                xml = xml.Substring(indexFinal + 1);
                xml = xml.Replace("</transmission>", "");
            }
            return xml;
        }

        private static string ObterShipment(string xml)
        {
            return string.Concat("<shipmentDetail>", xml, "</shipmentDetail>");
        }

        private static string ObterNFe(string xml)
        {
            return string.Concat("<brNFe>", xml, "</brNFe>");
        }

        private static string ObterCTe(string xml)
        {
            return string.Concat("<brCTe>", xml, "</brCTe>");
        }

        private static string ObterShipmentPackage(string xml)
        {
            return string.Concat("<shipmentPackageInfo>", xml, "</shipmentPackageInfo>");
        }

        private Message ObterXmlDeserializadoCompleto(string arquivoXml, Message xml, string nomeArquivo)
        {
            if (xml?.AmazonManifest == null)
                return xml;

            List<ShipmentDetail> detalhes = new List<ShipmentDetail>();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(arquivoXml);
            XmlNodeList listaShipment = doc.SelectNodes("message/amazonManifest/manifestDetail/shipmentDetail");

            foreach (XmlNode shipment in listaShipment)
            {
                string strNode = ObterShipment(shipment.InnerXml);
                List<BrNFe> NFes = new List<BrNFe>();
                ShipmentDetail novo = Utilidades.XML.Deserializar<ShipmentDetail>(strNode);

                if (nomeArquivo.Contains("Manifesto - Transhipment"))
                {
                    string strNodeNfe = ObterNFe(shipment.SelectSingleNode("brNfe")?.InnerXml);
                    NFes.Add(Utilidades.XML.Deserializar<BrNFe>(strNodeNfe));
                }
                else if (nomeArquivo.Contains("Manifesto - AMZL"))
                {
                    string strNodeNfe = ObterNFe(shipment.SelectSingleNode("brNFe")?.InnerXml);
                    NFes.Add(Utilidades.XML.Deserializar<BrNFe>(strNodeNfe));
                }
                else
                {
                    XmlNodeList XMLNFes = shipment.SelectNodes("brNFes/brNFe");

                    foreach (XmlNode nfe in XMLNFes)
                    {
                        string strNodeNfe = ObterNFe(nfe.InnerXml);
                        NFes.Add(Utilidades.XML.Deserializar<BrNFe>(strNodeNfe));
                    }
                }

                novo.BrNFes = NFes;

                detalhes.Add(novo);
            }

            xml.AmazonManifest.ShipmentDetail = detalhes;

            return xml;
        }

        private void InserirNotasFiscais(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repPedidoNFe = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);

            if (notasFiscais == null || cargaPedido == null)
                return;

            bool alteradoTipoDeCarga = false;

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal nfe in notasFiscais)
            {
                if (nfe == null)
                    continue;

                nfe.nfAtiva = true;

                repNotaFiscal.Inserir(nfe);

                Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal pedidoNFe = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork).InserirNotaCargaPedido(nfe, cargaPedido, _tipoServicoMultisoftware, TipoNotaFiscal.Remessa, _configuracaoEmbarcador, false, out alteradoTipoDeCarga);

                repPedidoNFe.Inserir(pedidoNFe);
            }

            return;
        }

        private void InserirCTeTerceiro(List<Dominio.Entidades.Embarcador.CTe.CTeTerceiro> listaCTes, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido)
        {
            Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao repPedidoCTe = new Repositorio.Embarcador.Pedidos.PedidoCTeParaSubContratacao(_unitOfWork);
            Repositorio.Embarcador.CTe.CTeTerceiro repNotaFiscal = new Repositorio.Embarcador.CTe.CTeTerceiro(_unitOfWork);

            if (listaCTes == null || cargaPedido == null)
                return;

            foreach (Dominio.Entidades.Embarcador.CTe.CTeTerceiro cte in listaCTes)
            {
                if (cte == null)
                    continue;

                cte.Ativo = true;

                repNotaFiscal.Inserir(cte);

                Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao pedidoCTe = new Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao()
                {
                    CTeTerceiro = cte,
                    CargaPedido = cargaPedido,
                    CFOP = cte.CFOP,
                    ValorICMS = cte.ValorICMS
                };


                repPedidoCTe.Inserir(pedidoCTe);
            }

            return;
        }

        #endregion


    }
}
