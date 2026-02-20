using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Integracao.SaintGobain
{
    public sealed class FTPSaintGobain
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;
        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain _configuracaoFTP;

        #endregion

        #region Construtores

        public FTPSaintGobain(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();
        }

        #endregion

        #region Métodos Privados

        private void GerarRegistroEDI(List<string> arquivos)
        {
            Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI repositorioControleIntegracaoCargaEDI = new Repositorio.Embarcador.Integracao.ControleIntegracaoCargaEDI(_unitOfWork);

            foreach (string nomeArquivo in arquivos)
            {
                try
                {
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(nomeArquivo))
                        continue;

                    _unitOfWork.Start();

                    Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI controleIntegracaoCargaEDI = new Dominio.Entidades.Embarcador.Integracao.ControleIntegracaoCargaEDI();
                    controleIntegracaoCargaEDI.Data = DateTime.Now;
                    controleIntegracaoCargaEDI.MensagemRetorno = "";
                    controleIntegracaoCargaEDI.NumeroDT = "";
                    controleIntegracaoCargaEDI.NomeArquivo = Path.GetFileName(nomeArquivo);
                    controleIntegracaoCargaEDI.GuidArquivo = Guid.NewGuid().ToString() + Path.GetExtension(nomeArquivo);
                    controleIntegracaoCargaEDI.NumeroTentativas = 0;
                    controleIntegracaoCargaEDI.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.AgIntegracao;

                    repositorioControleIntegracaoCargaEDI.Inserir(controleIntegracaoCargaEDI);

                    _unitOfWork.CommitChanges();

                    try
                    {
                        MoverArquivoPastaProcessados(nomeArquivo);
                    }
                    catch (Exception excecao)
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

        private Dominio.Entidades.Embarcador.Pedidos.Pedido GerarPedido(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery xmlDesserializado)
        {
            Repositorio.Embarcador.Pedidos.Pedido repositorioPedido = new Repositorio.Embarcador.Pedidos.Pedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoProduto repositorioPedidoProduto = new Repositorio.Embarcador.Pedidos.PedidoProduto(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoEndereco repositorioPedidoEndereco = new Repositorio.Embarcador.Pedidos.PedidoEndereco(_unitOfWork);
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(_unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repositorioModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(_unitOfWork);

            Pedido.Pedido servicoPedido = new Pedido.Pedido();

            string numeroPedido = xmlDesserializado.Dados.DadosCarga.Produtos.FirstOrDefault().NumeroPedido;
            decimal pesoTotal = xmlDesserializado.Dados.DadosCarga.Produtos.Sum(obj => obj.PesoBrutoProduto);
            int quantidadeVolumes = Decimal.ToInt32(xmlDesserializado.Dados.DadosCarga.Produtos.Sum(obj => obj.Volumes));

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = new Dominio.Entidades.Embarcador.Pedidos.Pedido()
            {
                Destinatario = ObterDestinatario(xmlDesserializado.Dados.DadosCarga.Clientes.Where(obj => obj.TipoCliente == "WE").Select(obj => obj).FirstOrDefault()),
                NumeroPedidoEmbarcador = numeroPedido,
                PesoTotal = pesoTotal,
                QtVolumes = quantidadeVolumes,
                Remetente = ObterRemetente(xmlDesserializado.Dados.DadosCarga.CodigoRemetenteFilial),
                Filial = ObterFilial(xmlDesserializado.Dados.DadosCarga.CodigoRemetenteFilial),
                SituacaoPedido = SituacaoPedido.Aberto,
                Destino = ObterDestinatario(xmlDesserializado.Dados.DadosCarga.Clientes.Where(obj => obj.TipoCliente == "WE").Select(obj => obj).FirstOrDefault())?.Localidade,
                TipoOperacao = ObterTipoOperacao(xmlDesserializado.Dados.DadosCarga.DadosTipoOperacao),
                CodigoCargaEmbarcador = xmlDesserializado.Dados.DadosCarga.Numero,
                TipoDeCarga = string.IsNullOrEmpty(xmlDesserializado.Dados.DadosComplementares.TipoDeCarga) ? null : repositorioTipoDeCarga.BuscarPorCodigoEmbarcador(xmlDesserializado.Dados.DadosComplementares.TipoDeCarga),
                ModeloVeicularCarga = string.IsNullOrEmpty(xmlDesserializado.Dados.DadosCarga.ModeloVeicularCarga) ? null : repositorioModeloVeicularCarga.buscarPorCodigoIntegracao(xmlDesserializado.Dados.DadosCarga.ModeloVeicularCarga)
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

            foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.DadosPedidoProduto produto in xmlDesserializado.Dados.DadosCarga.Produtos)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoProduto pedidoProduto = new Dominio.Entidades.Embarcador.Pedidos.PedidoProduto()
                {
                    Pedido = pedido,
                    Produto = ObterProduto(produto),
                    PesoTotalEmbalagem = 0,
                    PesoUnitario = produto.PesoBrutoProduto / produto.Volumes,
                    Quantidade = produto.Volumes
                };

                repositorioPedidoProduto.Inserir(pedidoProduto);
            }

            return pedido;
        }

        private void IniciarGeracoesCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery xmlDesserializado)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Servicos.Embarcador.Carga.Carga serCarga = new Embarcador.Carga.Carga(_unitOfWork);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = GerarPedido(xmlDesserializado);

            if (!string.IsNullOrWhiteSpace(pedido.CodigoCargaEmbarcador))
            {
                Dominio.Entidades.Embarcador.Cargas.Carga cargaExiste = repCarga.BuscarPorCodigoCargaEmbarcador(pedido.CodigoCargaEmbarcador, pedido.Filial?.Codigo ?? 0, null);
                if (cargaExiste != null)
                {
                    if (!serCarga.VerificarSeCargaEstaNaLogistica(cargaExiste, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) || cargaExiste.DataInicioGeracaoCTes.HasValue || cargaExiste.CalculandoFrete)
                        throw new ServicoException(string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeMaisSerAtualizadaCancelarMesma, pedido.CodigoCargaEmbarcador));
                    else
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
                        {
                            Carga = cargaExiste,
                            GerarIntegracoes = false,
                            MotivoCancelamento = Localization.Resources.Cargas.ControleGeracaoEDI.ViagemAtualizada,
                            TipoServicoMultisoftware = AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                        };

                        Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, _configuracaoEmbarcador, _unitOfWork);
                        Servicos.Embarcador.Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);

                        if (cargaCancelamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCancelamentoCarga.Cancelada)
                            throw new ServicoException(string.Format(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoPodeMaisSerAtualizadaCancelarManualmente, pedido.CodigoCargaEmbarcador));
                    }
                }
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = null;

            string mensagemRetornoCarga = Pedido.Pedido.CriarCarga(out carga, new List<Dominio.Entidades.Embarcador.Pedidos.Pedido> { pedido }, _unitOfWork, _tipoServicoMultisoftware, null, _configuracaoEmbarcador, true, false, false, false);

            if (!string.IsNullOrWhiteSpace(mensagemRetornoCarga))
                throw new ServicoException(mensagemRetornoCarga);
        }

        private void SolicitarCancelamentoCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery xmlDesserializado)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);

            int codigoFilial = ObterFilial(xmlDesserializado.Dados.DadosCarga.CodigoRemetenteFilial)?.Codigo ?? 0;
            string numeroCarga = xmlDesserializado.Dados.DadosCarga.Numero;

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigoCargaEmbarcador(numeroCarga, codigoFilial);

            if (carga == null)
                throw new ServicoException(Localization.Resources.Cargas.ControleGeracaoEDI.CargaNaoEncontrada);

            Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar cargaCancelamentoAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.CargaCancelamentoAdicionar()
            {
                Carga = carga,
                MotivoCancelamento = Localization.Resources.Cargas.ControleGeracaoEDI.CancelamentoPorFTP,
                TipoServicoMultisoftware = _tipoServicoMultisoftware
            };

            Dominio.Entidades.Embarcador.Cargas.CargaCancelamento cargaCancelamento = Servicos.Embarcador.Carga.Cancelamento.GerarCargaCancelamento(cargaCancelamentoAdicionar, _configuracaoEmbarcador, _unitOfWork);
            Carga.Cancelamento.SolicitarCancelamentoCarga(ref cargaCancelamento, _unitOfWork, _unitOfWork.StringConexao, _tipoServicoMultisoftware);

            if (cargaCancelamento.Situacao == SituacaoCancelamentoCarga.RejeicaoCancelamento)
                throw new ServicoException(cargaCancelamento.MensagemRejeicaoCancelamento, errorCode: Dominio.ObjetosDeValor.Enumerador.CodigoExcecao.RejeicaoCancelamentoCarga);
        }

        private void LiberarEmissaoCarga(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery xmlDesserializado)
        {
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);

            Dominio.Entidades.Embarcador.Filiais.Filial filial = ObterFilial(xmlDesserializado.Dados.DadosCarga.CodigoRemetenteFilial);

            if (filial == null)
                throw new ServicoException(string.Format(Localization.Resources.Servicos.Integracoes.SaintGobain.FTP.FilialNaoEncontrada, xmlDesserializado.Dados.DadosCarga.CodigoRemetenteFilial));
            //Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = GerarPedido(xmlDesserializado);

            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigoCargaEmbarcador(xmlDesserializado.Dados.DadosCarga.Numero, filial.Codigo);

            if (carga == null)
                throw new ServicoException(string.Format(Localization.Resources.Servicos.Integracoes.SaintGobain.FTP.CargaNaoEntrada, xmlDesserializado.Dados.DadosCarga.Numero));
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaPedido.BuscarPorCarga(carga.Codigo);

            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(_unitOfWork);
            servicoCarga.LiberarCargaSemNFe(carga, cargaPedidos, _configuracaoEmbarcador, _unitOfWork, _tipoServicoMultisoftware);

        }

        private void MoverArquivoPastaProcessados(string arquivo)
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain configuracaoFTP = ObterConfiguracaoFTP();

            string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, "Processados");
            string nomeArquivo = Path.GetFileName(arquivo);

            if (Utilidades.IO.FileStorageService.Storage.Exists(Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo)))
            {
                int numeroArquivosComMesmoNome = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho).Where(x => x.Contains(nomeArquivo)).Count();
                nomeArquivo = $"{Path.GetFileNameWithoutExtension(arquivo)}_{numeroArquivosComMesmoNome}{Path.GetExtension(arquivo)}";
            }

            Utilidades.IO.FileStorageService.Storage.Move(arquivo, Utilidades.IO.FileStorageService.Storage.Combine(caminho, nomeArquivo));
        }

        private List<string> ObterArquivos()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain configuracaoFTP = ObterConfiguracaoFTP();

            if (configuracaoFTP == null)
                return null;

            string diretorioDownload = Utilidades.IO.FileStorageService.Storage.Combine(Configuracoes.ConfigurationInstance.GetInstance(_unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivosIntegracaoEDI, configuracaoFTP.DiretorioDownload);
            string erro = "";
            
            Servicos.FTP.DownloadArquivosPasta(configuracaoFTP.EnderecoFTP, configuracaoFTP.Porta, configuracaoFTP.Diretorio, configuracaoFTP.Usuario, configuracaoFTP.Senha, configuracaoFTP.Passivo, configuracaoFTP.SSL, diretorioDownload, out erro, configuracaoFTP.UtilizarSFTP, false, "", true, false, false, true);

            if (!string.IsNullOrWhiteSpace(erro))
            {
                Log.TratarErro(erro);

                return null;
            }

            return Utilidades.IO.FileStorageService.Storage.GetFiles(diretorioDownload, "*.xml", SearchOption.TopDirectoryOnly).ToList();
        }

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain ObterConfiguracaoFTP()
        {
            if (_configuracaoFTP != null)
                return _configuracaoFTP;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain repositorio = new Repositorio.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain(_unitOfWork);

            _configuracaoFTP = repositorio.BuscarPrimeiroRegistro();

            return _configuracaoFTP;
        }

        private Dominio.Entidades.Cliente ObterDestinatario(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.DadosCliente dadosCliente)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repositorioLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfiguracaoTMS.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Cliente destinatario = repositorioCliente.BuscarPorCodigoIntegracao(dadosCliente.Codigo);
            Dominio.Entidades.Localidade localidade = null;

            if (dadosCliente.Estado.Length == 3)
                localidade = repositorioLocalidade.BuscarPorDescricaoAbreviacaoUF(Utilidades.String.RemoveAccents(dadosCliente.Cidade), dadosCliente.Estado);
            else
                localidade = repositorioLocalidade.BuscarPorDescricaoEUF(Utilidades.String.RemoveAccents(dadosCliente.Cidade), dadosCliente.Estado);

            if (localidade == null)
                throw new ServicoException(Localization.Resources.Cargas.ControleGeracaoEDI.LocalidadeNaoEncontradaLocalidadeIncorreta);

            if (destinatario != null)
            {
                destinatario.Nome = dadosCliente.Nome;
                destinatario.Localidade = localidade;
                destinatario.EnderecoDigitado = true;
                destinatario.Endereco = dadosCliente.Endereco;
                destinatario.Telefone1 = dadosCliente.Telefone.Left(20);
                destinatario.Telefone2 = dadosCliente.TelefoneSecundario.Left(20);
                destinatario.Email = dadosCliente.DadosComplementaresCliente?.EmailPrincipal ?? string.Empty;
                repositorioCliente.Atualizar(destinatario);

                return destinatario;
            }

            destinatario = new Dominio.Entidades.Cliente()
            {
                Nome = dadosCliente.Nome,
                EnderecoDigitado = true,
                Endereco = dadosCliente.Endereco,
                Localidade = localidade,
                CodigoIntegracao = dadosCliente.Codigo,
                CEP = "00000000",
                Bairro = Localization.Resources.Cargas.ControleGeracaoEDI.Centro,
                CPF_CNPJ = Double.Parse(dadosCliente.Codigo),
                Email = dadosCliente.DadosComplementaresCliente?.EmailPrincipal ?? string.Empty,
                Telefone1 = dadosCliente.Telefone.Left(20),
                Telefone2 = dadosCliente.TelefoneSecundario.Left(20),
                Atividade = new Repositorio.Atividade(_unitOfWork).BuscarPrimeiraAtividade(),
                Ativo = true,
                Tipo = config.Pais == TipoPais.Exterior ? "J" : "E"
            };

            repositorioCliente.Inserir(destinatario);

            return destinatario;
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(string codigoIntegracao)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                return null;

            return repositorioFilial.buscarPorCodigoEmbarcador(codigoIntegracao);
        }

        private Dominio.Entidades.Cliente ObterRemetente(string codigoIntegracao)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            if (string.IsNullOrWhiteSpace(codigoIntegracao))
                return null;

            return repositorioCliente.BuscarPorCodigoIntegracao(codigoIntegracao);
        }

        private Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador ObterProduto(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.DadosPedidoProduto dadosProduto)
        {
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(_unitOfWork);

            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repositorioProdutoEmbarcador.buscarPorCodigoEmbarcador(dadosProduto.CodigoProduto);

            if (produto != null)
                return produto;

            produto = new Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador()
            {
                CodigoProdutoEmbarcador = dadosProduto.CodigoProduto,
                Descricao = dadosProduto.Descricao,
                Ativo = true,
                PesoUnitario = dadosProduto.PesoBrutoProduto / dadosProduto.Volumes,
                Integrado = false
            };

            repositorioProdutoEmbarcador.Inserir(produto);

            return produto;
        }

        private Dominio.Entidades.Embarcador.Pedidos.TipoOperacao ObterTipoOperacao(Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.DadosTipoOperacao dadosTipoOperacao)
        {
            Repositorio.Embarcador.Pedidos.TipoOperacao repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);

            if (string.IsNullOrWhiteSpace(dadosTipoOperacao?.CodigoIntegracaoTipoOperacao))
                return null;

            return repositorioTipoOperacao.BuscarPorCodigoIntegracao(dadosTipoOperacao.CodigoIntegracaoTipoOperacao);
        }

        private void ProcessarArquivos()
        {
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoFTPSaintGobain configuracaoFTP = ObterConfiguracaoFTP();
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
                
                using (StreamReader reader = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(caminhoCompleto), Encoding.UTF8))//Encoding.GetEncoding("ISO-8859-1")
                {
                    string arquivoXml = reader.ReadToEnd();
                    Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery xmlDeserializado = Utilidades.XML.Deserializar<Dominio.ObjetosDeValor.Embarcador.Integracao.SaintGobain.Delivery>(arquivoXml);

                    if (xmlDeserializado?.Dados?.DadosCarga == null)
                        continue;

                    bool sucesso = true;

                    try
                    {
                        switch (xmlDeserializado.Dados.DadosComplementares.TipoAcao)
                        {
                            case "WHSORD":
                                if (xmlDeserializado.Dados.DadosCarga.DadosInformacaoDeletar.Tag == "DEL")
                                    controle.TipoArquivo = TipoArquivoCargaIntegracaoEDI.Deletion;
                                else
                                    controle.TipoArquivo = TipoArquivoCargaIntegracaoEDI.Creacion;
                                break;
                            case "DESADV":
                                controle.TipoArquivo = TipoArquivoCargaIntegracaoEDI.GoodsIssue;
                                break;
                        }

                        controle.CodigoIntegracaoCliente = xmlDeserializado.Dados.DadosCarga.Clientes.Where(c => c.TipoCliente == "WE").FirstOrDefault().Codigo;

                        repositorioControleIntegracao.Atualizar(controle);

                        _unitOfWork.Start();

                        switch (xmlDeserializado.Dados.DadosComplementares.TipoAcao)
                        {
                            case "WHSORD":
                                if (xmlDeserializado.Dados.DadosCarga.DadosInformacaoDeletar.Tag == "DEL")
                                {
                                    SolicitarCancelamentoCarga(xmlDeserializado);
                                    controle.SituacaoCarga = SituacaoCargaIntegracaoEDI.Cancelada;
                                }
                                else
                                {
                                    IniciarGeracoesCarga(xmlDeserializado);
                                    controle.SituacaoCarga = SituacaoCargaIntegracaoEDI.AguardandoEmissao;
                                }
                                break;
                            case "DESADV":
                                LiberarEmissaoCarga(xmlDeserializado);
                                break;
                        }

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
                        controle.MensagemRetorno = excecao.Message;
                    }

                    controle.NumeroDT = xmlDeserializado.Dados.DadosCarga.Numero;

                    if (!sucesso)
                        controle.SituacaoIntegracaoCargaEDI = SituacaoIntegracaoCargaEDI.Falha;
                    else
                    {
                        controle.SituacaoIntegracaoCargaEDI = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoCargaEDI.Integrado;
                        controle.MensagemRetorno = "";
                    }
                }

                repositorioControleIntegracao.Atualizar(controle);
            }
        }

        #endregion

        #region Métodos Públicos

        public void IniciarProcessamento()
        {
            List<string> arquivos = ObterArquivos();

            if (arquivos?.Count > 0)
                GerarRegistroEDI(arquivos);

            ProcessarArquivos();
        }

        public void GerarRegistroEDI(string arquivo)
        {
            GerarRegistroEDI(new List<string> { arquivo });
        }

        #endregion
    }
}
