using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Carga.GestaoDadosColeta
{
    public class GestaoDadosColeta
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS _configuracaoEmbarcador;

        #endregion Atributos

        #region Construtores

        public GestaoDadosColeta(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null) { }

        public GestaoDadosColeta(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            _unitOfWork = unitOfWork;
            _auditado = auditado;
        }

        #endregion Construtores

        #region Metodos Privados

        private void AdicionarDadosTransporteMotoristas(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte dadosTransporte, Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosTransporteAdicionar dadosTransporteAdicionar)
        {
            if (dadosTransporteAdicionar.CodigosMotoristas?.Count > 0)
            {
                Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(_unitOfWork);

                dadosTransporte.Motoristas = repositorioMotorista.BuscarPorCodigos(dadosTransporteAdicionar.CodigosMotoristas);
            }
            else
                dadosTransporte.Motoristas?.Clear();
        }

        private void AdicionarDadosTransporteVeiculos(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte dadosTransporte, Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosTransporteAdicionar dadosTransporteAdicionar)
        {
            Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = dadosTransporte.GestaoDadosColeta.CargaEntrega.Carga;
            Dominio.Entidades.Veiculo veiculo = null;

            if (dadosTransporteAdicionar.CodigoTracao > 0)
            {
                veiculo = repositorioVeiculo.BuscarPorCodigo(dadosTransporteAdicionar.CodigoTracao);

                if (veiculo != null)
                {
                    if ((carga.TipoOperacao?.ExigirVeiculoComRastreador ?? false) && !veiculo.PossuiRastreador)
                        throw new ServicoException($"Para o tipo de operação '{carga.TipoOperacao.Descricao}' é obrigatório informar um veiculo com o rastreador cadastrado.");

                    if ((veiculo.TipoVeiculo == "1") && (veiculo.VeiculosTracao != null) && (veiculo.VeiculosTracao.Count > 0))
                    {
                        Dominio.Entidades.Veiculo tracao = (from obj in veiculo.VeiculosTracao where obj.Ativo select obj).FirstOrDefault();

                        if (tracao != null)
                            veiculo = tracao;
                    }
                }
            }

            dadosTransporte.Veiculo = veiculo;

            if (dadosTransporte.VeiculosVinculados == null)
                dadosTransporte.VeiculosVinculados = new List<Dominio.Entidades.Veiculo>();

            if (carga.TipoOperacao?.ExigePlacaTracao ?? false)
            {
                dadosTransporte.VeiculosVinculados.Clear();

                if (dadosTransporteAdicionar.CodigoReboque > 0)
                {
                    Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorCodigo(dadosTransporteAdicionar.CodigoReboque);

                    if (reboque != null && !dadosTransporte.VeiculosVinculados.Contains(reboque))
                        dadosTransporte.VeiculosVinculados.Add(reboque);
                }

                if (dadosTransporteAdicionar.CodigoSegundoReboque > 0)
                {
                    Dominio.Entidades.Veiculo reboque = repositorioVeiculo.BuscarPorCodigo(dadosTransporteAdicionar.CodigoSegundoReboque);

                    if (reboque != null && !dadosTransporte.VeiculosVinculados.Contains(reboque))
                        dadosTransporte.VeiculosVinculados.Add(reboque);
                }
            }
            else if (dadosTransporte.Veiculo != null)
            {
                dadosTransporte.VeiculosVinculados.Clear();

                foreach (Dominio.Entidades.Veiculo veiculoVinculado in dadosTransporte.Veiculo.VeiculosVinculados)
                    dadosTransporte.VeiculosVinculados.Add(veiculoVinculado);
            }
            else
                dadosTransporte.VeiculosVinculados.Clear();
        }

        private void AdicionarNFeCarga(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal repositorioPedidoXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.PedidoXMLNotaFiscal(_unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(_unitOfWork);
            Servicos.Embarcador.Pedido.NotaFiscal servicoCargaNotaFiscal = new Servicos.Embarcador.Pedido.NotaFiscal(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            Dominio.Entidades.Embarcador.Cargas.Carga carga = gestaoDadosColetaDadosNFe.GestaoDadosColeta.CargaEntrega.Carga;
            Dominio.Entidades.Embarcador.Cargas.CargaPedido primeiroCargaPedido = repositorioCargaEntregaPedido.BuscarPrimeiroCargaPedidoPorCargaEntrega(gestaoDadosColetaDadosNFe.GestaoDadosColeta.CargaEntrega.Codigo);
            Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal = string.IsNullOrWhiteSpace(gestaoDadosColetaDadosNFe.Chave) ? null : repositorioXMLNotaFiscal.BuscarPorChave(gestaoDadosColetaDadosNFe.Chave);

            if (primeiroCargaPedido == null)
                primeiroCargaPedido = repositorioCargaPedido.BuscarPrimeiroPorCargaSemFetch(carga.Codigo);

            if (primeiroCargaPedido == null)
                throw new ServicoException("A coleta não possui pedidos.");

            if (xmlNotaFiscal == null)
            {
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
                xmlNotaFiscal.Chave = gestaoDadosColetaDadosNFe.Chave;
                xmlNotaFiscal.DataEmissao = gestaoDadosColetaDadosNFe.DataEmissao.Value;
                xmlNotaFiscal.Emitente = gestaoDadosColetaDadosNFe.Emitente;
                xmlNotaFiscal.Destinatario = gestaoDadosColetaDadosNFe.Destinatario;
                xmlNotaFiscal.Peso = gestaoDadosColetaDadosNFe.Peso;
                xmlNotaFiscal.PesoBaseParaCalculo = gestaoDadosColetaDadosNFe.Peso;
                xmlNotaFiscal.PesoLiquido = gestaoDadosColetaDadosNFe.Peso;
                xmlNotaFiscal.Volumes = gestaoDadosColetaDadosNFe.Volumes;
                xmlNotaFiscal.Valor = gestaoDadosColetaDadosNFe.Valor;
                xmlNotaFiscal.TipoOperacaoNotaFiscal = TipoOperacaoNotaFiscal.Saida;
                xmlNotaFiscal.Filial = carga.Filial;
                xmlNotaFiscal.Empresa = carga.Empresa;
                xmlNotaFiscal.nfAtiva = true;
                xmlNotaFiscal.DataRecebimento = DateTime.Now;
                xmlNotaFiscal.CNPJTranposrtador = string.Empty;
                xmlNotaFiscal.PlacaVeiculoNotaFiscal = string.Empty;
                xmlNotaFiscal.XML = string.Empty;

                if (string.IsNullOrWhiteSpace(gestaoDadosColetaDadosNFe.Chave))
                {
                    xmlNotaFiscal.Descricao = "Outros";
                    xmlNotaFiscal.Modelo = "99";
                    xmlNotaFiscal.Numero = gestaoDadosColetaDadosNFe.Numero;
                    xmlNotaFiscal.Serie = gestaoDadosColetaDadosNFe.Serie;
                    xmlNotaFiscal.TipoDocumento = TipoDocumento.Outros;
                    xmlNotaFiscal.TipoEmissao = TipoEmissaoNotaFiscal.NaoEletronica;
                }
                else
                {
                    xmlNotaFiscal.Modelo = "55";
                    xmlNotaFiscal.Numero = Utilidades.Chave.ObterNumero(gestaoDadosColetaDadosNFe.Chave);
                    xmlNotaFiscal.Serie = Utilidades.Chave.ObterSerie(gestaoDadosColetaDadosNFe.Chave);
                    xmlNotaFiscal.TipoDocumento = TipoDocumento.NFe;
                    xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<TipoEmissaoNotaFiscal>();
                }

                repositorioXMLNotaFiscal.Inserir(xmlNotaFiscal);
            }

            servicoCargaNotaFiscal.InserirNotaCargaPedido(xmlNotaFiscal, primeiroCargaPedido, tipoServicoMultisoftware, TipoNotaFiscal.Venda, configuracaoEmbarcador, false, out bool alteradoTipoDeCarga, _auditado);
            Servicos.Auditoria.Auditoria.Auditar(_auditado, xmlNotaFiscal, null, "Adicionada via gestão de dados de coleta", _unitOfWork);

            decimal pesoNaNFs = repositorioPedidoXMLNotaFiscal.BuscarPesoPorCarga(carga.Codigo);
            int volumes = repositorioPedidoXMLNotaFiscal.BuscarVolumesPorCarga(carga.Codigo);

            Servicos.WebService.NFe.NotaFiscal.FinalizarEnvioDasNotas(ref primeiroCargaPedido, pesoNaNFs, volumes, null, null, null, null, null, configuracaoEmbarcador, tipoServicoMultisoftware, _auditado, null, _unitOfWork);
        }

        private async Task AtualizarAprovacaoAsync(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta, SituacaoGestaoDadosColeta situacao)
        {
            await AtualizarAprovacaoAsync(gestaoDadosColeta, situacao, null);
        }

        private async Task AtualizarAprovacaoAsync(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta, SituacaoGestaoDadosColeta situacao, Dominio.Entidades.Embarcador.Configuracoes.Motivo motivoRejeicao)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repositorioGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(_unitOfWork);

            gestaoDadosColeta.DataAprovacao = DateTime.Now;
            gestaoDadosColeta.UsuarioAprovacao = _auditado?.Usuario;
            gestaoDadosColeta.Situacao = situacao;
            gestaoDadosColeta.Motivo = motivoRejeicao;

            await repositorioGestaoDadosColeta.AtualizarAsync(gestaoDadosColeta);
        }


        private void AtualizarDadosTransporteCarga(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = gestaoDadosColetaDadosTransporte.GestaoDadosColeta.CargaEntrega.Carga;

            carga.Veiculo = gestaoDadosColetaDadosTransporte.Veiculo;
            carga.VeiculosVinculados = gestaoDadosColetaDadosTransporte.VeiculosVinculados?.ToList();
            carga.Motoristas = gestaoDadosColetaDadosTransporte.Motoristas?.ToList();

            repositorioCarga.Atualizar(carga);
        }

        private void GerarIntegracoes(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta)
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.ArcelorMittal);

            bool naoGerarIntegracao = gestaoDadosColeta.CargaEntrega.Carga.TipoOperacao?.ConfiguracaoIntegracao?.NaoGerarIntegracaoRetornoConfirmacaoColeta ?? false;

            if (tipoIntegracao == null || naoGerarIntegracao)
                return;

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao repositorioGestaoDadosColetaIntegracao = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao gestaoDadosColetaIntegracao = new Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao()
            {
                GestaoDadosColeta = gestaoDadosColeta,
                TipoIntegracao = tipoIntegracao,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                ProblemaIntegracao = "",
                DataIntegracao = DateTime.Now,
            };

            repositorioGestaoDadosColetaIntegracao.Inserir(gestaoDadosColetaIntegracao);
        }

        private Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ObterCargaEntrega(int codigoCargaEntrega)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoCargaEntrega);

            if (cargaEntrega == null)
                throw new ServicoException("Não foi possível encontrar a coleta informada.");

            return cargaEntrega;
        }

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador()
        {
            if (_configuracaoEmbarcador == null)
                _configuracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(_unitOfWork).BuscarConfiguracaoPadrao();

            return _configuracaoEmbarcador;
        }

        private void RecomprarValePedagio(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio> valesPedagios = repositorioCargaValePedagio.BuscarPorCarga(carga.Codigo, SituacaoIntegracao.Integrado);

            if (valesPedagios.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio valePedagio in valesPedagios)
            {
                valePedagio.SituacaoValePedagio = SituacaoValePedagio.EmCancelamento;
                repositorioCargaValePedagio.Atualizar(valePedagio);
            }

            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador();
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorCarga(carga.Codigo);

            ValePedagio.CargaValePedagioRota.CriarCargaValePedagioPorRotaFrete(carga, cargaPedidos, configuracaoEmbarcador, _unitOfWork, tipoServicoMultisoftware);
        }

        private void GerarIntegracoesNotificacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, TipoNotificacaoApp tipoNotificacao)
        {
            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(_unitOfWork).GerarIntegracaoNotificacao(carga, tipoNotificacao);
        }

        #endregion Metodos Privados

        #region Metodos Publicos

        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe Adicionar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAdicionar dadosNFeAdicionar)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repositorioGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = ObterCargaEntrega(dadosNFeAdicionar.CodigoCargaEntrega);

            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta = new Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta()
            {
                CargaEntrega = cargaEntrega,
                Empresa = cargaEntrega.Carga.Empresa,
                Latitude = dadosNFeAdicionar.Latitude,
                Longitude = dadosNFeAdicionar.Longitude,
                Origem = dadosNFeAdicionar.Origem,
                Tipo = TipoGestaoDadosColeta.DadosNfe,
                UsuarioCriacao = _auditado?.Usuario
            };

            repositorioGestaoDadosColeta.Inserir(gestaoDadosColeta);

            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe = new Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe()
            {
                GestaoDadosColeta = gestaoDadosColeta,
                GuidArquivo = dadosNFeAdicionar.GuidArquivo,
                OrigemFoto = dadosNFeAdicionar.OrigemFoto
            };

            repositorioGestaoDadosColetaNFe.Inserir(gestaoDadosColetaDadosNFe);

            return gestaoDadosColetaDadosNFe;
        }

        public Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte Adicionar(Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosTransporteAdicionar dadosTransporteAdicionar)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repositorioGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(_unitOfWork);
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repositorioGestaoDadosColetaDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = ObterCargaEntrega(dadosTransporteAdicionar.CodigoCargaEntrega);

            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta = new Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta()
            {
                CargaEntrega = cargaEntrega,
                Empresa = cargaEntrega.Carga.Empresa,
                Origem = dadosTransporteAdicionar.Origem,
                Tipo = TipoGestaoDadosColeta.DadosTransporte,
                UsuarioCriacao = _auditado?.Usuario
            };

            repositorioGestaoDadosColeta.Inserir(gestaoDadosColeta);

            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte = new Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte()
            {
                GestaoDadosColeta = gestaoDadosColeta
            };

            AdicionarDadosTransporteVeiculos(gestaoDadosColetaDadosTransporte, dadosTransporteAdicionar);
            AdicionarDadosTransporteMotoristas(gestaoDadosColetaDadosTransporte, dadosTransporteAdicionar);

            repositorioGestaoDadosColetaDadosTransporte.Inserir(gestaoDadosColetaDadosTransporte);

            return gestaoDadosColetaDadosTransporte;
        }

        public async Task AprovarAsync(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (gestaoDadosColetaDadosNFe.Numero <= 0)
                throw new ServicoException("O número deve ser informado.");

            if (string.IsNullOrWhiteSpace(gestaoDadosColetaDadosNFe.Serie))
                throw new ServicoException("A série deve ser informada.");

            if (gestaoDadosColetaDadosNFe.Emitente == null)
                throw new ServicoException("O emitente deve ser informado.");

            if (gestaoDadosColetaDadosNFe.Destinatario == null)
                throw new ServicoException("O destinatário deve ser informado.");

            if (!gestaoDadosColetaDadosNFe.DataEmissao.HasValue)
                throw new ServicoException("A data de emissão deve ser informada.");

            await AtualizarAprovacaoAsync(gestaoDadosColetaDadosNFe.GestaoDadosColeta, SituacaoGestaoDadosColeta.Aprovado);
            AdicionarNFeCarga(gestaoDadosColetaDadosNFe, tipoServicoMultisoftware);
            GerarIntegracoes(gestaoDadosColetaDadosNFe.GestaoDadosColeta);
            GerarIntegracoesNotificacao(gestaoDadosColetaDadosNFe.GestaoDadosColeta.CargaEntrega.Carga, TipoNotificacaoApp.CarregamentoValidadoAguardandoCompraValePedagio);
        }

        public async Task AprovarAsync(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            await AtualizarAprovacaoAsync(gestaoDadosColetaDadosTransporte.GestaoDadosColeta, SituacaoGestaoDadosColeta.Aprovado);
            AtualizarDadosTransporteCarga(gestaoDadosColetaDadosTransporte);
            GerarIntegracoes(gestaoDadosColetaDadosTransporte.GestaoDadosColeta);
            RecomprarValePedagio(gestaoDadosColetaDadosTransporte.GestaoDadosColeta.CargaEntrega.Carga, tipoServicoMultisoftware);
        }

        public void AtualizarDadosAprovacao(Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe, Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAprovacao dadosNFeAprovacao)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(_unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(_unitOfWork);

            gestaoDadosColetaDadosNFe.DataEmissao = (dadosNFeAprovacao.DataEmissao != DateTime.MinValue) ? dadosNFeAprovacao.DataEmissao : null;
            gestaoDadosColetaDadosNFe.Emitente = (dadosNFeAprovacao.CpfCnpjEmitente > 0d) ? repositorioCliente.BuscarPorCPFCNPJ(dadosNFeAprovacao.CpfCnpjEmitente) : null;
            gestaoDadosColetaDadosNFe.Destinatario = (dadosNFeAprovacao.CpfCnpjDestinatario > 0d) ? repositorioCliente.BuscarPorCPFCNPJ(dadosNFeAprovacao.CpfCnpjDestinatario) : null;
            gestaoDadosColetaDadosNFe.Numero = dadosNFeAprovacao.Numero;
            gestaoDadosColetaDadosNFe.Serie = dadosNFeAprovacao.Serie;
            gestaoDadosColetaDadosNFe.Peso = dadosNFeAprovacao.Peso;
            gestaoDadosColetaDadosNFe.Volumes = dadosNFeAprovacao.Volumes;
            gestaoDadosColetaDadosNFe.Valor = dadosNFeAprovacao.Valor;

            if (string.IsNullOrWhiteSpace(dadosNFeAprovacao.Chave))
                gestaoDadosColetaDadosNFe.Chave = string.Empty;
            else
            {
                gestaoDadosColetaDadosNFe.Chave = dadosNFeAprovacao.Chave.ObterSomenteNumeros();

                if (!Utilidades.Validate.ValidarChaveNFe(dadosNFeAprovacao.Chave))
                    throw new ServicoException("A chave informada é inválida.");
            }

            repositorioGestaoDadosColetaNFe.Atualizar(gestaoDadosColetaDadosNFe);
        }

        public async Task ReprovarDadosNFeAsync(int codigo, Dominio.Entidades.Embarcador.Configuracoes.Motivo motivoRejeicao, int codigoCarga)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe = repositorioGestaoDadosColetaNFe.BuscarPorCodigo(codigo);

            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = await repositorioCarga.BuscarPorCodigoAsync(codigoCarga);

            if (gestaoDadosColetaDadosNFe == null)
                throw new ServicoException("Não foi possível encontrar os dados da NF-e.");

            await AtualizarAprovacaoAsync(gestaoDadosColetaDadosNFe.GestaoDadosColeta, SituacaoGestaoDadosColeta.Reprovado, motivoRejeicao);

            Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao parametrosIntegracaoNotificacao = new Dominio.ObjetosDeValor.Embarcador.TorreControle.IntegracaoNotificacao()
            {
                Carga = carga,
                TipoNotificacaoApp = TipoNotificacaoApp.RejeicaoDadosNFeColeta,
                GestaoDadosColetaDadosNFe = gestaoDadosColetaDadosNFe,
                MotivoRejeicao = motivoRejeicao,
            };

            new Servicos.Embarcador.SuperApp.IntegracaoNotificacaoApp(_unitOfWork).GerarIntegracaoNotificacao(parametrosIntegracaoNotificacao);
        }

        public async Task ReprovarDadosTransporteAsync(int codigo)
        {
            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repositorioGestaoDadosColetaDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(_unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte = repositorioGestaoDadosColetaDadosTransporte.BuscarPorCodigo(codigo);

            if (gestaoDadosColetaDadosTransporte == null)
                throw new ServicoException("Não foi possível encontrar os dados de transporte.");

            await AtualizarAprovacaoAsync(gestaoDadosColetaDadosTransporte.GestaoDadosColeta, SituacaoGestaoDadosColeta.Reprovado);
        }

        public void VerificarGestaoDadosColetaIntegracoesPendentes(TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware == TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao repositorioGestaoDadosColetaIntegracao = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao> gestoesDadosColetaIntegracoes = repositorioGestaoDadosColetaIntegracao.BuscarIntegracaoPendente(5, 5, "Codigo", "asc", 20, TipoEnvioIntegracao.Individual);

            foreach (Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaIntegracao gestaoDadosColetaIntegracao in gestoesDadosColetaIntegracoes)
            {
                if (gestaoDadosColetaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.ArcelorMittal)
                {
                    if (gestaoDadosColetaIntegracao.GestaoDadosColeta.Tipo == TipoGestaoDadosColeta.DadosNfe)
                        new Servicos.Embarcador.Integracao.ArcelorMittal.IntegracaoArcelorMittal(_unitOfWork).IntegrarTransporteFornecimento(gestaoDadosColetaIntegracao);
                    else
                        new Servicos.Embarcador.Integracao.ArcelorMittal.IntegracaoArcelorMittal(_unitOfWork).IntegrarDadosCarga(gestaoDadosColetaIntegracao);
                }
                else
                {
                    gestaoDadosColetaIntegracao.DataIntegracao = DateTime.Now;
                    gestaoDadosColetaIntegracao.NumeroTentativas++;
                    gestaoDadosColetaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    gestaoDadosColetaIntegracao.ProblemaIntegracao = "Tipo de integração não implementada";

                    repositorioGestaoDadosColetaIntegracao.Atualizar(gestaoDadosColetaIntegracao);
                }
            }
        }

        #endregion
    }
}
