using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Servicos.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Carga.ControleEntrega
{
    public class OcorrenciaEntrega
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion


        #region Construtores

        public OcorrenciaEntrega(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public OcorrenciaEntrega(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }

        #endregion

        #region Métodos Privados

        private static void GerarOcorrenciaPedidoOcorrencia(List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, decimal? latitude, decimal? longitude, string observacaoOcorrencia, decimal valor, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            bool retornarPreCtes = (carga.SituacaoCarga == SituacaoCarga.PendeciaDocumentos && carga.AgImportacaoCTe);
            Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repositorioCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosGerarOcorrencia = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<int> codigosTipoOcorrencia = pedidosOcorrenciaColetaEntregas?.Select(obj => obj.TipoDeOcorrencia.Codigo).Distinct().ToList() ?? new List<int>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega in pedidosOcorrenciaColetaEntregas)
            {
                Dominio.Entidades.Cliente tomador = pedidoOcorrenciaColetaEntrega.Pedido.ObterTomador();

                if (tomador?.GrupoPessoas?.GerarOcorrenciaControleEntrega ?? false)
                    pedidosGerarOcorrencia.Add(pedidoOcorrenciaColetaEntrega.Pedido);
            }

            if (pedidosGerarOcorrencia.Count == 0)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesDaCarga = repositorioCargaCTe.BuscarPorCarga(carga.Codigo, false, true, 0, 0, retornarPreCtes);
            if (configuracaoEmbarcador.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTesDaCarga)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaFinalizadora = repositorioCargaOcorrenciaDocumento.BuscarOcorrenciaFinalPorDocumento(cargaCTe.Codigo);
                    if (cargaOcorrenciaFinalizadora != null)
                    {
                        if (cargaCTe.CTe != null)
                            throw new ServicoException("Já foi gerada uma ocorrencia (" + cargaOcorrenciaFinalizadora.CargaOcorrencia.NumeroOcorrencia + ") final para o CT-e " + cargaCTe.CTe.Numero + "");
                        else
                            throw new ServicoException("Já foi gerada uma ocorrencia (" + cargaOcorrenciaFinalizadora.CargaOcorrencia.NumeroOcorrencia + ") final para as notas fiscais (" + string.Join(", ", (from nf in cargaCTe.NotasFiscais select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()) + ") do Pré CT-e selecionado");
                    }
                }

                foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosGerarOcorrencia)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ocorrenciasFinalizadoras = repositorioPedidoOcorrenciaColetaEntrega.BuscarOcorrenciasFinalizadorasPorPedido(pedido.Codigo);

                    if (ocorrenciasFinalizadoras != null)
                    {
                        if (ocorrenciasFinalizadoras.Where(o => o.Pedido.Codigo != pedido.Codigo).Count() > 0)
                        {
                            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrenciaFinalizadora = ocorrenciasFinalizadoras.Where(o => o.Pedido.Codigo != pedido.Codigo).FirstOrDefault();
                            throw new ServicoException($"Já foi gerada a ocorrencia ({ocorrenciaFinalizadora.Descricao}) final para o pedido {ocorrenciaFinalizadora.Pedido.NumeroPedidoEmbarcador}");
                        }
                    }
                }
            }

            if (configuracaoEmbarcador.NaoPermitirLancarOcorrenciasEmDuplicidadeNaSequencia)
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe in cargaCTesDaCarga)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento ultimaOcorrencia = repositorioCargaOcorrenciaDocumento.BuscarUltimaOcorrenciaPorCargaCTe(cargaCTe.Codigo);

                    if (ultimaOcorrencia != null && codigosTipoOcorrencia.Contains(ultimaOcorrencia.CargaOcorrencia.TipoOcorrencia.Codigo))
                    {
                        if (cargaCTe.CTe != null)
                            throw new ServicoException("Não é possivel gerar ocorrências na sequencia com o mesmo tipo de ocorrência para o CT-e  " + cargaCTe.CTe.Numero + " (esse tipo de ocorrência já foi gerado na ocorrência " + ultimaOcorrencia.CargaOcorrencia.NumeroOcorrencia + " anteriormente). ");
                        else
                            throw new ServicoException("Não é possivel gerar ocorrências na sequencia com o mesmo tipo de ocorrência para as notas fiscais (" + string.Join(", ", (from nf in cargaCTe.NotasFiscais select nf.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero).Distinct().ToList()) + ") do Pré CT-e selecionado (esse tipo de ocorrência já foi gerado na ocorrência " + ultimaOcorrencia.CargaOcorrencia.NumeroOcorrencia + " anteriormente). ");
                    }
                }
            }


            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repositorioCargaPedido.BuscarPorPedidos((from o in pedidosGerarOcorrencia select o.Codigo).ToList());
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosGerarOcorrencia = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidosGerarOcorrencia)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = (from obj in cargaPedidos where obj.Pedido.Codigo == pedido.Codigo && obj.Carga.Codigo == carga.Codigo select obj).FirstOrDefault();

                if (cargaPedido != null)
                {
                    if (cargaPedido.PedidoEncaixado)//FEITO PARA DANONE.. TEM Q SER DA PRIMEIRA CARGA, sem comparar pela variavel de parametro
                        cargaPedido = (from obj in cargaPedidos where obj.Pedido.Codigo == pedido.Codigo && obj.ObterTomador().CPF_CNPJ == pedido.ObterTomador().Codigo select obj).FirstOrDefault();

                    if (cargaPedido != null)
                        cargaPedidosGerarOcorrencia.Add(cargaPedido);
                }
            }

            if (cargaPedidos.Count == 0)
                return;

            Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia repositorioGrupoPessoaTipoOcorrencia = new Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia(unitOfWork);
            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega primeiroPedidoOcorrenciaColetaEntrega = pedidosOcorrenciaColetaEntregas.FirstOrDefault();
            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia grupoPessoaTipoOcorrencia = repositorioGrupoPessoaTipoOcorrencia.BuscarPorTipoOcorrencia(primeiroPedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.Codigo, primeiroPedidoOcorrenciaColetaEntrega.Pedido.ObterTomador()?.GrupoPessoas?.Codigo ?? 0);

            if (grupoPessoaTipoOcorrencia == null && primeiroPedidoOcorrenciaColetaEntrega.TipoDeOcorrencia == null)
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.Carga> cargas = (from obj in cargaPedidosGerarOcorrencia select obj.CargaOrigem).Distinct().ToList();

            foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaOrigem in cargas)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosCarga = (from obj in cargaPedidos where obj.CargaOrigem.Codigo == cargaOrigem.Codigo select obj).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCargaCTesPorCargaPedidos((from obj in cargaPedidosCarga select obj.Codigo).ToList());

                if (cargaCTes.Count == 0)
                    continue;

                GerarOcorrencia(cargaOrigem, grupoPessoaTipoOcorrencia, primeiroPedidoOcorrenciaColetaEntrega.TipoDeOcorrencia, componenteFrete, chamado, cargaCTes, primeiroPedidoOcorrenciaColetaEntrega.DataOcorrencia, latitude, longitude, observacaoOcorrencia, valor, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);

            }
        }

        private static void GerarNotificacaoDePedidoOcorrenciaPortalCliente(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedido == null)
                return;

            Servicos.Embarcador.Notificacao.Notificacao serNotificacaoPortal = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, null, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, string.Empty);

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            Dominio.Entidades.Usuario usuarioDestinatario = repUsuario.BuscarPorCPF(pedido.Destinatario?.CPF_CNPJ_SemFormato ?? "");
            Dominio.Entidades.Usuario usuarioRemetente = repUsuario.BuscarPorCPF(pedido.Remetente?.CPF_CNPJ_SemFormato ?? "");

            string nota = string.Format(Localization.Resources.Cargas.ControleEntrega.PedidoTemUmaOcorrenciaPendente, pedido.NumeroPedidoEmbarcador);

            if (usuarioDestinatario != null)
                serNotificacaoPortal.GerarNotificacao(usuarioDestinatario, codigoClienteMultisoftware: clienteMultisoftware?.Codigo ?? 0, codigoObjeto: pedido.Codigo, URLPagina: "", nota: nota, icone: IconesNotificacao.ocorrenciaPedido, tipoNotificacao: TipoNotificacao.ocorrenciaPedido, tipoServicoMultisoftwareNotificar: AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, unitOfWork: unitOfWork);

            if (usuarioRemetente != null)
                serNotificacaoPortal.GerarNotificacao(usuarioRemetente, codigoClienteMultisoftware: clienteMultisoftware?.Codigo ?? 0, codigoObjeto: pedido.Codigo, URLPagina: "", nota: nota, icone: IconesNotificacao.ocorrenciaPedido, tipoNotificacao: TipoNotificacao.ocorrenciaPedido, tipoServicoMultisoftwareNotificar: AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor, unitOfWork: unitOfWork);

        }

        private static void GravarOcorrenciaGeradaEntrega(
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            DateTime DataEvento,
            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia,
            string pacote,
            int volumes,
            string observacao,
            Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenadas,
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado,
            List<string> imagens = null,
            IEnumerable<CustomFile> anexosOcorrencia = null,
            Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = null,
            List<int> notasFiscais = null,
            TipoServicoMultisoftware? tipoServicoMultisoftware = null,
            OrigemSituacaoEntrega? origemSituacaoEntrega = null
        )
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaColetaEntregaAnexo svcOcorrenciaAnexo = new Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);

            bool existeOcorrenciaEntrega = repositorioOcorrenciaColetaEntrega.ExisteOcorrenciaPorCargaEntregaETipoOcorrencia(cargaEntrega.Codigo, DataEvento, tipoDeOcorrencia.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemCriacaoOcorrencia origemOcorrencia = ObterOrigemOcorrencia(origemSituacaoEntrega ?? cargaEntrega.OrigemSituacao, tipoServicoMultisoftware);

            if (existeOcorrenciaEntrega)
                return;

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaColetaEntrega = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega()
            {
                DataOcorrencia = DataEvento,
                CargaEntrega = cargaEntrega,
                TipoDeOcorrencia = tipoDeOcorrencia,
                TiposCausadoresOcorrencia = tiposCausadoresOcorrencia,
                Pacote = pacote,
                Latitude = configOcorrenciaCoordenadas?.Latitude ?? null,
                Longitude = configOcorrenciaCoordenadas?.Longitude ?? null,
                DataPosicao = configOcorrenciaCoordenadas?.DataPosicao ?? null,
                DataPrevisaoRecalculada = configOcorrenciaCoordenadas?.DataPrevisaoRecalculada ?? null,
                DistanciaAteDestino = configOcorrenciaCoordenadas?.DistanciaAteDestino ?? 0,
                TempoPercurso = configOcorrenciaCoordenadas?.TempoPercurso,
                Volumes = volumes,
                PendenteEnvioEmail = cargaEntrega.Carga.TipoOperacao?.EnviarLinkAcompanhamentoParaClienteEntrega ?? false,
                ObservacaoOcorrencia = observacao,
                OrigemOcorrencia = origemOcorrencia,
            };

            repositorioOcorrenciaColetaEntrega.Inserir(ocorrenciaColetaEntrega, auditado);
            svcOcorrenciaAnexo.AdicionarAnexos(ocorrenciaColetaEntrega, anexosOcorrencia, unitOfWork);
            SalvarNotasFiscais(ocorrenciaColetaEntrega, unitOfWork, notasFiscais);

            AdicionarImagensOcorrencia(ocorrenciaColetaEntrega, imagens, unitOfWork);
        }

        private static void AdicionarImagensOcorrencia(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia, List<string> imagens, Repositorio.UnitOfWork unitOfWork)
        {
            if (imagens == null || imagens.Count == 0)
                return;

            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo repAnexo = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo(unitOfWork);

            string extensao = ".jpg";

            foreach (string imagem in imagens)
            {
                // Salva imagem no disco
                byte[] buffer = System.Convert.FromBase64String(imagem);
                System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.ArmazenarArquivoFisico(ms, Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), out string tokenImagem);

                // Salva no banco
                string caminhoOcorrencia = Utilidades.IO.FileStorageService.Storage.Combine(Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Ocorrencias" }), tokenImagem + extensao);
                int numeroAnexo = repAnexo.BuscarProximoNumero(ocorrencia.Codigo);

                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoOcorrencia))
                {
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo ocorrenciaAnexo = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaAnexo()
                    {
                        EntidadeAnexo = ocorrencia,
                        Descricao = string.Empty,
                        GuidArquivo = tokenImagem,
                        NomeArquivo = $"{ocorrencia.TipoDeOcorrencia?.Descricao ?? string.Empty} - {numeroAnexo}" + extensao,
                        Numero = numeroAnexo
                    };

                    repAnexo.Inserir(ocorrenciaAnexo);

                }
            }
        }

        private static bool VerificarPermiteSituacaoEmTransporte(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido situacaoAcompanhamentoPedido)
        {
            if (situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.Entregue &&
                         situacaoAcompanhamentoPedido != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.EmTransporte &&
                         situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.SaiuParaEntrega &&
                         situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.EntregaParcial &&
                         situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.EntregaRejeitada)
                return true;
            else
                return false;
        }

        private static bool VerificarPermiteSituacaoSaiuEntrega(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido situacaoAcompanhamentoPedido)
        {
            if (situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.Entregue &&
                         situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.EntregaParcial &&
                         situacaoAcompanhamentoPedido != SituacaoAcompanhamentoPedido.EntregaRejeitada)
                return true;
            else
                return false;
        }

        private static List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> ProcessarOcorrenciaEntrega(
            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos,
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            decimal? latitude,
            decimal? longitude,
            DateTime DataEvento,
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega,
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas,
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao,
            AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware,
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado,
            TipoServicoMultisoftware tipoServicoMultisoftware,
            OrigemSituacaoEntrega? origemSituacaoEntrega)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosEmTransporte = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosSaiuParaEntrega = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidosFinalizados = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEncaixados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

            if (cargaPedidos.Any(obj => obj.PedidoEncaixado))
                cargaPedidosEncaixados = repCargaPedido.BuscarPorPedidos((from o in cargaPedidos where o.PedidoEncaixado select o.Pedido.Codigo).ToList());

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
            configOcorrenciaCoordenada.Latitude = latitude;
            configOcorrenciaCoordenada.Longitude = longitude;
            configOcorrenciaCoordenada.DataExecucao = DataEvento;
            configOcorrenciaCoordenada.DataPosicao = DataEvento;
            configOcorrenciaCoordenada.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
            configOcorrenciaCoordenada.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
            configOcorrenciaCoordenada.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntrega;
                Dominio.Entidades.Cliente clientePedido;
                if (cargaEntrega.Coleta)
                {
                    tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta;
                    clientePedido = pedido.Remetente;

                    if (clientePedido == null)
                        clientePedido = pedido.Expedidor;
                }
                else
                {
                    clientePedido = pedido.Destinatario;
                    tipoAplicacaoColetaEntrega = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega;

                    if (clientePedido == null)
                        clientePedido = pedido.Recebedor;
                }

                if (clientePedido == null)
                    return pedidosOcorrenciaColetaEntregas;

                bool clienteAlvo = cargaEntrega.Cliente == clientePedido;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntregaPedido = null;
                if (cargaEntrega.Reentrega)
                {
                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega && obj.Reentrega == true select obj).FirstOrDefault();

                    if (configuracaoOcorrenciaEntregaPedido == null) // caso nao exista de reentrega continua como estava..
                        configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();
                }
                else
                    configuracaoOcorrenciaEntregaPedido = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == clienteAlvo && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntrega select obj).FirstOrDefault();

                if (configuracaoOcorrenciaEntregaPedido != null)
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntrega.Carga;

                    if (cargaPedido.PedidoEncaixado)
                        carga = (from obj in cargaPedidosEncaixados where obj.Pedido.Codigo == pedido.Codigo && obj.ObterTomador().CPF_CNPJ == pedido.ObterTomador().Codigo select obj).FirstOrDefault()?.Carga ?? null;

                    pedidosOcorrenciaColetaEntregas.Add(GerarPedidoOcorrenciaColetaEntrega(cargaEntrega.Cliente, pedido, carga, configuracaoOcorrenciaEntregaPedido.TipoDeOcorrencia, configuracaoPortalCliente, DataEvento, eventoColetaEntrega, configuracao, clienteMultisoftware, configOcorrenciaCoordenada, unitOfWork, false));
                }


                if (eventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Confirma)
                {
                    if (cargaEntrega.Coleta)
                    {
                        if (VerificarPermiteSituacaoEmTransporte(pedido.SituacaoAcompanhamentoPedido))
                            pedidosEmTransporte.Add(pedido);
                    }
                    else
                    {
                        if (clientePedido.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ)
                            pedidosFinalizados.Add(pedido);
                        else if (VerificarPermiteSituacaoEmTransporte(pedido.SituacaoAcompanhamentoPedido))
                            pedidosEmTransporte.Add(pedido);
                    }
                }

                if (eventoColetaEntrega == Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem)
                {
                    if (VerificarPermiteSituacaoSaiuEntrega(pedido.SituacaoAcompanhamentoPedido))
                    {
                        if (!cargaEntrega.Coleta && clientePedido.CPF_CNPJ == cargaEntrega.Cliente?.CPF_CNPJ)
                            pedidosSaiuParaEntrega.Add(pedido);
                    }
                }
            }

            Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega tipoAplicacaoColetaEntregaCarga = cargaEntrega.Coleta ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Coleta : Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAplicacaoColetaEntrega.Entrega;
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = (from obj in configuracaoOcorrenciaEntregas where obj.AlvoDoPedido == true && obj.TipoAplicacaoColetaEntrega == tipoAplicacaoColetaEntregaCarga select obj).FirstOrDefault();
            if (configuracaoOcorrenciaEntrega != null)
                GravarOcorrenciaGeradaEntrega(
                    cargaEntrega,
                    DataEvento,
                    configuracaoOcorrenciaEntrega.TipoDeOcorrencia,
                    "",
                    0,
                    string.Empty,
                    configOcorrenciaCoordenada,
                    unitOfWork,
                    auditado,
                    tipoServicoMultisoftware: tipoServicoMultisoftware,
                    origemSituacaoEntrega: origemSituacaoEntrega
                );

            if (pedidosFinalizados.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosFinalizados select obj.Codigo).ToList(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.Entregue);

            if (pedidosEmTransporte.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosEmTransporte select obj.Codigo).ToList(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.EmTransporte);

            if (pedidosSaiuParaEntrega.Count > 0)
                repPedido.SetarSituacaoAcompanhamentoPorPedidos((from obj in pedidosSaiuParaEntrega select obj.Codigo).ToList(), Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcompanhamentoPedido.SaiuParaEntrega);

            return pedidosOcorrenciaColetaEntregas;
        }

        //Nao torne esse metodo publico, use os outros acima e trate os parametros.
        private static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, string observacao, DateTime dataOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega? EventoColetaEntrega, string pacote, int volumes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenadas, Repositorio.UnitOfWork unitOfWork, bool gerarNotificacao = true, bool gerarIntegracao = false, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = null, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntregaIntegracao = null)
        {
            Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

            if (configuracao.NaoPermitirLancarOcorrenciasDepoisDeOcorrenciaFinalGerada)
            {
                Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega ocorrenciaFinalizadora = repositorioPedidoOcorrenciaColetaEntrega.BuscarOcorrenciaFinalizadoraPorPedido(pedido.Codigo);
                if (ocorrenciaFinalizadora != null)
                    throw new ServicoException($"Já foi gerada a ocorrencia ({ocorrenciaFinalizadora.Descricao}) final para o pedido {pedido.NumeroPedidoEmbarcador}");
            }

            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = new Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega()
            {
                Alvo = alvo,
                DataOcorrencia = dataOcorrencia,
                Observacao = observacao,
                Pedido = pedido,
                Carga = carga,
                Pacote = pacote,
                Volumes = volumes,
                TempoPercurso = configOcorrenciaCoordenadas?.TempoPercurso ?? "",
                Latitude = configOcorrenciaCoordenadas?.Latitude ?? null,
                Longitude = configOcorrenciaCoordenadas?.Longitude ?? null,
                DataPosicao = configOcorrenciaCoordenadas?.DataPosicao ?? null,
                DataPrevisaoRecalculada = configOcorrenciaCoordenadas?.DataPrevisaoRecalculada ?? null,
                DistanciaAteDestino = configOcorrenciaCoordenadas?.DistanciaAteDestino ?? 0,
                PendenteEnvioSMS = configuracaoPortalCliente?.EnviarSMS ?? false,
                PendenteEnvioEmail = (configuracaoPortalCliente?.EnviarEmail ?? false) && (tipoDeOcorrencia == null || (tipoDeOcorrencia?.NaoIndicarAoCliente != true)),
                PendenteEnvioWhatsApp = configuracaoPortalCliente?.EnviarMensagemWhatsApp ?? false,
                PendenteIntegracaoERP = alvo?.GrupoPessoas?.PermitirConsultarOcorrenciaControleEntregaWebService ?? false,
                TipoDeOcorrencia = tipoDeOcorrencia,
                EventoColetaEntrega = EventoColetaEntrega != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Todos ? EventoColetaEntrega : null,
                Natureza = pedidoOcorrenciaColetaEntregaIntegracao?.Natureza ?? string.Empty,
                Razao = pedidoOcorrenciaColetaEntregaIntegracao?.Razao ?? string.Empty,
                SolicitacaoCliente = pedidoOcorrenciaColetaEntregaIntegracao?.SolicitacaoCliente ?? string.Empty,
                NotaFiscalDevolucao = pedidoOcorrenciaColetaEntregaIntegracao?.NotafiscalDevolucao ?? 0,
                GrupoOcorrencia = pedidoOcorrenciaColetaEntregaIntegracao?.GrupoOcorrencia ?? string.Empty,
            };

            if (tipoDeOcorrencia?.DisponibilizarPedidoParaNovaIntegracao ?? false)
            {
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Log.TratarErro($"Pedido {pedido.NumeroPedidoEmbarcador} - Saldo Restante de.: {pedido.PesoSaldoRestante} para {pedido.PesoTotal}. OcorrenciaEntrega.GerarPedidoOcorrenciaColetaEntrega", "SaldoPedido");
                pedido.AguardandoIntegracao = true;
                pedido.PedidoTotalmenteCarregado = false;
                pedido.PesoSaldoRestante = pedido.PesoTotal;
                pedido.PalletSaldoRestante = pedido.TotalPallets;
                pedido.SituacaoPedido = SituacaoPedido.Aberto;
                repPedido.Atualizar(pedido);
            }

            repositorioPedidoOcorrenciaColetaEntrega.Inserir(pedidoOcorrenciaColetaEntrega);


            if (gerarIntegracao || (pedido.ObterTomador()?.GrupoPessoas?.GerarOcorrenciaControleEntrega ?? false))
                Servicos.Embarcador.Carga.ControleEntrega.PedidoOcorrenciaColetaEntregaIntegracao.GerarIntegracoes(pedidoOcorrenciaColetaEntrega, configuracao, unitOfWork);

            if ((tipoDeOcorrencia?.GerarAtendimentoAutomaticamente ?? false) && carga != null)
            {
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                if (!(configuracaoOcorrencia?.NaoGerarAtendimentoDuplicadoParaMesmaOcorrencia ?? false) || !repChamado.ExistePorCargaMotivoChamadoCliente(carga.Codigo, tipoDeOcorrencia.MotivoChamadoGeracaoAutomatica.Codigo, alvo.CPF_CNPJ))
                    new Servicos.Embarcador.Chamado.Chamado(unitOfWork).GerarAtendimentoAutomaticamente(carga, alvo, tipoDeOcorrencia, carga.Motoristas?.FirstOrDefault(), unitOfWork);
            }

            if (gerarNotificacao)
                GerarNotificacaoDePedidoOcorrenciaPortalCliente(pedido, clienteMultisoftware, unitOfWork);

            EnviarNotificacaoPorEmail(pedidoOcorrenciaColetaEntrega, unitOfWork);

            return pedidoOcorrenciaColetaEntrega;
        }

        //private static bool ExisteOcorrenciaPedidoPorTipoOcorrenTempo(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, int tempo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega repositorioPedidoOcorrenciaColetaEntrega = new Repositorio.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega(unitOfWork);

        //    return repositorioPedidoOcorrenciaColetaEntrega.ExistePorPedidoTipoOcorrenciaEmTempo(tipoDeOcorrencia.Codigo, tempo, pedido.Codigo);
        //}

        //private static bool ExisteOcorrenciaEntregaPorTipo(Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, int tempo, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);

        //    return repositorioOcorrenciaColetaEntrega.ExistePorEntregaTipoOcorrenciaEmTempo(tipoDeOcorrencia.Codigo, tempo, cargaEntrega.Codigo);
        //}

        private static Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento GerarObjetoCargaEntregaEnvento(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataEvento, EventoColetaEntrega eventoColetaEntrega, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, decimal? latitude, decimal? longitude, OrigemSituacaoEntrega origemSituacao)
        {

            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento()
            {
                Carga = cargaEntrega.CargaOrigem != null ? cargaEntrega.CargaOrigem : cargaEntrega.Carga,
                CargaEntrega = cargaEntrega,
                DataOcorrencia = dataEvento,
                DataPosicao = dataEvento,
                DataPrevisaoRecalculada = cargaEntrega.DataReprogramada,
                EventoColetaEntrega = eventoColetaEntrega,
                TipoDeOcorrencia = tipoDeOcorrencia,
                Latitude = latitude,
                Longitude = longitude,
                Origem = origemSituacao,
            };
        }

        private static void EnviarNotificacaoPorEmail(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            if (pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.TipoOcorrenciaControleEntrega && (pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.NotificarClientePorEmail ?? false))
                NotificarClientePorEmail(pedidoOcorrenciaColetaEntrega, unitOfWork);
        }

        public static void NotificarClientePorEmail(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Logistica.CentroCarregamento repositorioCentroCarregamento = new Repositorio.Embarcador.Logistica.CentroCarregamento(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

            if (string.IsNullOrWhiteSpace(pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.AssuntoEmailNotificacao) || string.IsNullOrWhiteSpace(pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CorpoEmailNotificacao) || string.IsNullOrWhiteSpace(pedidoOcorrenciaColetaEntrega.Alvo.Email))
                return;

            EnviarEmailClientePedidoOcorrencia(pedidoOcorrenciaColetaEntrega, unitOfWork);
        }

        private static void EnviarEmailClientePedidoOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork).BuscarEmailEnviaDocumentoAtivo();

                if (configuracaoEmail == null)
                    return;

                string de = configuracaoEmail.Email;
                string login = configuracaoEmail.Email;
                string senha = configuracaoEmail.Senha;
                string[] copiaOcultaPara = new string[] { };
                string[] copiaPara = new string[] { };
                string corpo = ObterBodyEmailNotificacao(pedidoOcorrenciaColetaEntrega);
                string assunto = RetornarTextoEmail(pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.AssuntoEmailNotificacao, pedidoOcorrenciaColetaEntrega);
                string servidorSMTP = configuracaoEmail.Smtp;
                List<System.Net.Mail.Attachment> anexos = null;
                string assinatura = "";
                bool possuiSSL = configuracaoEmail.RequerAutenticacaoSmtp;
                string responderPara = "";
                int porta = configuracaoEmail.PortaSmtp;
                string para = pedidoOcorrenciaColetaEntrega.Alvo.Email;

                if (!Servicos.Email.EnviarEmail(de, login, senha, para, copiaOcultaPara, copiaPara, assunto, corpo, servidorSMTP, out string erro, configuracaoEmail.DisplayEmail, anexos, assinatura, possuiSSL, responderPara, porta, unitOfWork))
                    Log.TratarErro(erro, "EnviarEmailClientePedidoOcorrencia");
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "EnviarEmailClientePedidoOcorrencia");
            }
        }

        private static string ObterBodyEmailNotificacao(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            System.Text.StringBuilder body = new System.Text.StringBuilder();

            body.AppendLine(@"<div style=""font-family: Arial;"">");
            body.AppendLine($@"    <p style=""margin:0px"">{RetornarTextoEmail(pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CorpoEmailNotificacao, pedidoOcorrenciaColetaEntrega)}</p>");
            body.AppendLine($@"    <p style=""font-size: 12px; margin:0px"">{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}</p>");
            body.AppendLine("    <p></p>");
            body.AppendLine(@"    <p style=""font-size: 12px; margin:0px"">Esse e-mail foi enviado automaticamente pela MultiSoftware. Por favor, não responder.</p>");
            body.AppendLine("</div>");

            return body.ToString();
        }

        private static void ValidarTipoOcorrenciaRecalculoPrevisoes(List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas, Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntregaAtual, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega primeiroPedidoOcorrenciaColetaEntrega = pedidosOcorrenciaColetaEntregas.FirstOrDefault();

            if (primeiroPedidoOcorrenciaColetaEntrega != null && (primeiroPedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.RecalcularPrevisaoEntregaPendentes ?? false))
                //devemos recalculas as previsoes da carga e ProcessarEventoEntrega
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(cargaEntregaAtual.Carga, configuracaoEmbarcador, unitOfWork, tipoServicoMultisoftware);
        }

        private static Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento GerarObjetoCargaEntregaEnventoCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataEvento, EventoColetaEntrega eventoColetaEntrega, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, decimal? latitude, decimal? longitude, OrigemSituacaoEntrega origemSituacao)
        {

            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento()
            {
                Carga = carga,
                CargaEntrega = null,
                DataOcorrencia = dataEvento,
                DataPosicao = dataEvento,
                EventoColetaEntrega = eventoColetaEntrega,
                TipoDeOcorrencia = tipoDeOcorrencia,
                Latitude = latitude,
                Longitude = longitude,
                Origem = origemSituacao,
            };
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public static string RetornarTextoEmail(string textoEmail, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            return textoEmail
                .Replace("#NumeroOcorrencia", "***")
                .Replace("#RazaoTransportador", pedidoOcorrenciaColetaEntrega.Carga?.Empresa?.RazaoSocial ?? "***")
                .Replace("#CNPJTransportador", pedidoOcorrenciaColetaEntrega.Carga?.Empresa?.CNPJ_Formatado ?? "***")
                .Replace("#TipoOcorrencia", pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? "***")
                .Replace("#ValorOcorrencia", "***")
                .Replace("#NumeroCarga", pedidoOcorrenciaColetaEntrega.Carga?.CodigoCargaEmbarcador ?? "***")
                .Replace("#NumeroPedido", pedidoOcorrenciaColetaEntrega.Pedido?.NumeroPedidoEmbarcador ?? "***")
                .Replace("#NumeroOrdem", pedidoOcorrenciaColetaEntrega.Pedido?.NumeroOrdem ?? "***");
        }

        public static void GerarOcorrencia(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia grupoPessoaTipoOcorrencia, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes, DateTime dataOcorrencia, decimal? latitude, decimal? longitude, string observacaoOcorrencia, decimal valor, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.CargaOcorrencia.Ocorrencia servicoOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoOcorrencia repositorioChamadoOcorrencia = new Repositorio.Embarcador.Chamados.ChamadoOcorrencia(unitOfWork);

            Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();

            cargaOcorrencia.DataOcorrencia = dataOcorrencia;
            cargaOcorrencia.DataAlteracao = DateTime.Now;
            cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia = DateTime.Now;
            cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
            cargaOcorrencia.ValorOcorrencia = valor;
            cargaOcorrencia.ValorOcorrenciaOriginal = valor;
            cargaOcorrencia.Observacao = observacaoOcorrencia;
            cargaOcorrencia.ObservacaoCTe = "";
            cargaOcorrencia.ObservacaoCTes = "";
            cargaOcorrencia.Carga = carga;
            cargaOcorrencia.TipoOcorrencia = grupoPessoaTipoOcorrencia != null ? grupoPessoaTipoOcorrencia.TipoOcorrencia : tipoDeOcorrencia;
            cargaOcorrencia.CodigoTipoOcorrenciaParaIntegracao = grupoPessoaTipoOcorrencia != null ? grupoPessoaTipoOcorrencia.CodigoIntegracao : tipoDeOcorrencia?.CodigoIntegracao ?? string.Empty;
            cargaOcorrencia.OrigemOcorrencia = cargaOcorrencia.TipoOcorrencia.OrigemOcorrencia;
            cargaOcorrencia.ComponenteFrete = componenteFrete;

            if (latitude != null)
                cargaOcorrencia.Latitude = latitude.ToString().Replace(",", ".");

            if (longitude != null)
                cargaOcorrencia.Longitude = longitude.ToString().Replace(",", ".");

            repositorioCargaOcorrencia.Inserir(cargaOcorrencia);

            if (chamado != null)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia chamadoOcorrencia = new Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia()
                {
                    Chamado = chamado,
                    CargaOcorrencia = cargaOcorrencia
                };

                repositorioChamadoOcorrencia.Inserir(chamadoOcorrencia);
            }

            Integracao.IntegracaoOcorrencia.AdicionarIntegracoesOcorrencia(cargaOcorrencia, cargaCTes, unitOfWork);
            string mensagemRetorno = null;
            servicoOcorrencia.FluxoGeralOcorrencia(ref cargaOcorrencia, cargaCTes, null, ref mensagemRetorno, unitOfWork, tipoServicoMultisoftware, null, configuracaoEmbarcador, clienteMultisoftware, "", false);
        }

        public static bool GerarEventoCargaEntrega(Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar eventoCargaEntregaAdicionar, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, OrigemSituacaoEntrega origemSituacaoEntrega, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, string pacote, int volumes)
        {
            if (!eventoCargaEntregaAdicionar.TipoOcorrencia.TipoOcorrenciaControleEntrega)
                return false;

            bool retorno = false;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repositorioXMLNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repositorioCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repositorioConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntrega = repositorioConfiguracaoOcorrenciaEntrega.BuscarPorTipoOcorrencia(eventoCargaEntregaAdicionar.TipoOcorrencia.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repositorioCargaEntregaPedido.BuscarPorCargaPedido(eventoCargaEntregaAdicionar.CodigosCargaPedidos);
            Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint wayPoint = null;

            if (!string.IsNullOrEmpty(eventoCargaEntregaAdicionar.Latitude) && !string.IsNullOrWhiteSpace(eventoCargaEntregaAdicionar.Longitude))
                wayPoint = new Dominio.ObjetosDeValor.Embarcador.Logistica.WayPoint(eventoCargaEntregaAdicionar.Latitude, eventoCargaEntregaAdicionar.Longitude);

            if (configuracaoOcorrenciaEntrega.Count > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrencia = configuracaoOcorrenciaEntrega.FirstOrDefault();

                switch (configuracaoOcorrencia.EventoColetaEntrega)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.Confirma:
                        Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork).BuscarPrimeiroRegistro();
                        Dominio.Entidades.Embarcador.Pedidos.TipoOperacao tipoOperacaoParametro = null;

                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                        {
                            bool podeFinalizar = true;
                            if (volumes > 0)
                            {
                                int volumesTotais = repOcorrenciaColetaEntrega.ObterVolumesTotais(cargaEntrega.Codigo, eventoCargaEntregaAdicionar.TipoOcorrencia.Codigo);
                                volumesTotais += volumes;
                                int volumesPedidos = repositorioCargaEntregaPedido.ObterVolumesTotais(cargaEntrega.Codigo);
                                if (volumesTotais < volumesPedidos)
                                    podeFinalizar = false;

                                Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();

                                configOcorrenciaCoordenada.Latitude = (decimal)wayPoint.Latitude;
                                configOcorrenciaCoordenada.Longitude = (decimal)wayPoint.Longitude;
                                configOcorrenciaCoordenada.DataExecucao = DateTime.Now;
                                configOcorrenciaCoordenada.DataPosicao = DateTime.Now;
                                configOcorrenciaCoordenada.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
                                configOcorrenciaCoordenada.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
                                configOcorrenciaCoordenada.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";
                                GravarOcorrenciaGeradaEntrega(
                                    cargaEntrega,
                                    eventoCargaEntregaAdicionar.DataOcorrencia,
                                    eventoCargaEntregaAdicionar.TipoOcorrencia,
                                    "",
                                    0,
                                    string.Empty,
                                    configOcorrenciaCoordenada,
                                    unitOfWork,
                                    auditado,
                                    tipoServicoMultisoftware: tipoServicoMultisoftware,
                                    origemSituacaoEntrega: origemSituacaoEntrega
                                );
                            }
                            if (podeFinalizar)
                            {
                                tipoOperacaoParametro ??= new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork).BuscarPorCodigoFetch(cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0);
                                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.FinalizarEntrega(cargaEntrega, eventoCargaEntregaAdicionar.DataOcorrencia, wayPoint, null, 0, "", configuracao, tipoServicoMultisoftware, null, origemSituacaoEntrega, clienteMultisoftware, unitOfWork, false, configuracaoControleEntrega, tipoOperacaoParametro);
                                retorno = true;
                            }
                            if (origemSituacaoEntrega == OrigemSituacaoEntrega.ArquivoEDI)
                                Servicos.Auditoria.Auditoria.Auditar(auditado, cargaEntrega, "Entrega confirmada ao gerar evento de Carga Entrega via Envio de OCOREN", unitOfWork);
                        }
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.ChegadaNoAlvo:
                        foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                            Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarEntradaNoRaio(cargaEntrega, eventoCargaEntregaAdicionar.DataOcorrencia, unitOfWork);
                        break;

                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.IniciaViagem:
                        if (!eventoCargaEntregaAdicionar.Carga.DataInicioViagem.HasValue)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditadoAutomatico = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
                            {
                                TipoAuditado = TipoAuditado.Sistema,
                                OrigemAuditado = OrigemAuditado.Sistema
                            };

                            if (Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.IniciarViagem(eventoCargaEntregaAdicionar.Carga.Codigo, eventoCargaEntregaAdicionar.DataOcorrencia, OrigemSituacaoEntrega.UsuarioMultiEmbarcador, wayPoint, configuracao, tipoServicoMultisoftware, clienteMultisoftware, auditadoAutomatico, unitOfWork))
                                Servicos.Auditoria.Auditoria.Auditar(auditadoAutomatico, eventoCargaEntregaAdicionar.Carga, $"Início de viagem informado automaticamente ao gerar Evento de Carga Entrega na Ocorrência", unitOfWork);
                        }

                        break;
                    //case Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.EventosViagem:
                    //    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    //    {
                    //        Dominio.Entidades.Embarcador.Chamados.Chamado chamado = new Dominio.Entidades.Embarcador.Chamados.Chamado();
                    //        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(cargaEntrega.Codigo, cargaOcorrencia.TipoOcorrencia.Codigo, cargaOcorrencia.DataOcorrencia, wayPoint, cargaOcorrencia.Usuario, 0, tipoServicoMultisoftware, null, unitOfWork, out chamado, "", configuracao, false, new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>(), true);
                    //    }
                    //    break;

                    default:
                        break;
                }
            }
            else if (eventoCargaEntregaAdicionar.TipoOcorrencia.UsadoParaMotivoRejeicaoColetaEntrega)
            {
                Chamado.Chamado servicoChamado = new Chamado.Chamado(unitOfWork);
                List<SituacaoNotaFiscal> situacoesNotasFiscaisFinalizada = new List<SituacaoNotaFiscal>()
                {
                    SituacaoNotaFiscal.Devolvida,
                    SituacaoNotaFiscal.DevolvidaParcial,
                    SituacaoNotaFiscal.Entregue
                };

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                {
                    bool podeFinalizar = true;
                    if (volumes > 0)
                    {
                        int volumesTotais = repOcorrenciaColetaEntrega.ObterVolumesTotais(cargaEntrega.Codigo, eventoCargaEntregaAdicionar.TipoOcorrencia.Codigo);
                        volumesTotais += volumes;
                        int volumesPedidos = repositorioCargaEntregaPedido.ObterVolumesTotais(cargaEntrega.Codigo);
                        if (volumesTotais < volumesPedidos)
                            podeFinalizar = false;

                        Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();

                        configOcorrenciaCoordenada.Latitude = (decimal)wayPoint.Latitude;
                        configOcorrenciaCoordenada.Longitude = (decimal)wayPoint.Longitude;
                        configOcorrenciaCoordenada.DataExecucao = DateTime.Now;
                        configOcorrenciaCoordenada.DataPosicao = DateTime.Now;
                        configOcorrenciaCoordenada.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
                        configOcorrenciaCoordenada.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
                        configOcorrenciaCoordenada.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";

                        GravarOcorrenciaGeradaEntrega(
                            cargaEntrega,
                            eventoCargaEntregaAdicionar.DataOcorrencia,
                            eventoCargaEntregaAdicionar.TipoOcorrencia,
                            "",
                            0,
                            string.Empty,
                            configOcorrenciaCoordenada,
                            unitOfWork,
                            auditado,
                            tipoServicoMultisoftware: tipoServicoMultisoftware,
                            origemSituacaoEntrega: origemSituacaoEntrega
                        );
                    }

                    if (podeFinalizar)
                    {
                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros()
                        {
                            codigoCargaEntrega = cargaEntrega.Codigo,
                            codigoMotivo = eventoCargaEntregaAdicionar.TipoOcorrencia.Codigo,
                            configuracao = configuracao,
                            data = eventoCargaEntregaAdicionar.DataOcorrencia,
                            observacao = "",
                            tipoServicoMultisoftware = tipoServicoMultisoftware,
                            usuario = eventoCargaEntregaAdicionar.Usuario,
                            wayPoint = wayPoint,
                            OrigemSituacaoEntrega = origemSituacaoEntrega,
                            clienteMultisoftware = clienteMultisoftware
                        };

                        if (eventoCargaEntregaAdicionar.ValidarNotasFiscaisFinalizadas)
                        {
                            if (!repositorioXMLNotaFiscal.ExisteNotasFiscaisPorEntrega(cargaEntrega.Codigo, situacoesNotasFiscaisFinalizada, situacaoNotaFiscal: null))
                                continue;

                            parametros.situacoesNotasFiscaisNaoAtualizar = new List<SituacaoNotaFiscal>()
                        {
                            SituacaoNotaFiscal.Devolvida,
                            SituacaoNotaFiscal.DevolvidaParcial,
                            SituacaoNotaFiscal.Entregue
                        };
                        }

                        Dominio.Entidades.Embarcador.Chamados.Chamado chamado;
                        Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, auditado, unitOfWork, out chamado, tipoServicoMultisoftware);
                        servicoChamado.EnviarEmailChamadoAberto(chamado, unitOfWork);
                        retorno = true;
                    }
                }
            }
            return retorno;
        }

        public static void GerarEventoCargaEntregaPorOcorrencia(List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTEs, Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            if ((cargaOcorrencia.Carga == null) || (cargaCTEs.Count == 0))
                return;

            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<int> codigosCargaPedidos = repositorioCargaPedidoXMLNotaFiscalCTe.BuscarCodigosCargaPedidoPorCargaCTes((from o in cargaCTEs select o.Codigo).ToList());

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar eventoCargaEntregaAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.EventoCargaEntregaAdicionar()
            {
                Carga = cargaOcorrencia.Carga,
                CodigosCargaPedidos = codigosCargaPedidos,
                DataOcorrencia = cargaOcorrencia.DataOcorrencia,
                Latitude = cargaOcorrencia.Latitude,
                Longitude = cargaOcorrencia.Longitude,
                TipoOcorrencia = cargaOcorrencia.TipoOcorrencia,
                Usuario = cargaOcorrencia.Usuario
            };

            OrigemSituacaoEntrega origemSituacaoEntrega = (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;
            GerarEventoCargaEntrega(eventoCargaEntregaAdicionar, configuracao, origemSituacaoEntrega, tipoServicoMultisoftware, auditado, unitOfWork, clienteMultisoftware, "", 0);
        }

        //chamado pelo Iniciar viagem e finalizar Viagem
        public static void GerarOcorrenciaCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga, DateTime dataEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, decimal? latitude, decimal? longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origemSituacaoEntrega, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(eventoColetaEntrega);

            if (configuracaoOcorrenciaEntregas.Count <= 0)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> cargaEntregasPedido = repCargaEntregaPedido.BuscarPorCarga(carga.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();

            //OrigemSituacaoEntrega origemSituacaoEntrega = origem.ObterOrigemSituacaoEntrega();

            foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
            {
                if (cargaEntrega.Carga.CargaDePreCarga)
                    continue;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = (from obj in cargaEntregasPedido where obj.CargaEntrega.Codigo == cargaEntrega.Codigo select obj.CargaPedido).Distinct().ToList();
                pedidosOcorrenciaColetaEntregas.AddRange(ProcessarOcorrenciaEntrega(
                    cargaPedidos,
                    cargaEntrega,
                    latitude,
                    longitude,
                    dataEvento,
                    eventoColetaEntrega,
                    configuracaoOcorrenciaEntregas,
                    configuracaoEmbarcador,
                    clienteMultisoftware,
                    unitOfWork,
                    auditado,
                    tipoServicoMultisoftware: tipoServicoMultisoftware,
                    origemSituacaoEntrega: origemSituacaoEntrega));

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, eventoColetaEntrega, tipoDeOcorrencia: null, latitude, longitude, origemSituacaoEntrega);
                servicoCargaEntregaEvento.ProcessarEventoEntrega(cargaEntregaEvento, cargaPedidos, configuracaoOcorrenciaEntregas, configuracaoControleEntrega);
            }

            GerarOcorrenciaPedidoOcorrencia(pedidosOcorrenciaColetaEntregas, carga, null, null, latitude, longitude, string.Empty, 0m, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
        }

        //chamado pelo Iniciar e finalizar Entrega
        public void GerarOcorrenciaEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataEvento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega, decimal? latitude, decimal? longitude, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origemSituacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(_unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(eventoColetaEntrega);

            if (configuracaoOcorrenciaEntregas.Count <= 0)
                return;

            if (cargaEntrega.Carga.CargaDePreCarga)
                return;

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas = ProcessarOcorrenciaEntrega(
                cargaPedidos,
                cargaEntrega,
                latitude,
                longitude,
                dataEvento,
                eventoColetaEntrega,
                configuracaoOcorrenciaEntregas,
                configuracaoEmbarcador,
                clienteMultisoftware,
                _unitOfWork,
                auditado,
                tipoServicoMultisoftware,
                origemSituacao
            );
            GerarOcorrenciaPedidoOcorrencia(pedidosOcorrenciaColetaEntregas, cargaEntrega.Carga, null, null, latitude, longitude, string.Empty, 0m, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, _unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, eventoColetaEntrega, tipoDeOcorrencia: null, latitude, longitude, origemSituacao);
            servicoCargaEntregaEvento.ProcessarEventoEntrega(cargaEntregaEvento, cargaPedidos, configuracaoOcorrenciaEntregas, configuracaoControleEntrega);

            ValidarTipoOcorrenciaRecalculoPrevisoes(pedidosOcorrenciaColetaEntregas, configuracaoOcorrenciaEntregas, cargaEntrega, configuracaoEmbarcador, tipoServicoMultisoftware, _unitOfWork);
        }

        public static void GerarOcorrenciaRejeicao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime DataEvento, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, decimal? latitude, decimal? longitude, string observacao, decimal valor, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemSituacaoEntrega origemSituacao, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete, Dominio.Entidades.Embarcador.Chamados.Chamado chamado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega eventoColetaEntrega = EventoColetaEntrega.Todos, List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = null, List<string> imagensOcorrencia = null, IEnumerable<CustomFile> anexosOcorrencia = null, Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = null, List<int> notasFiscais = null)
        {
            if (tipoDeOcorrencia == null || (cargaEntrega.ChamadoEmAberto && auditado.TipoAuditado != TipoAuditado.Usuario && !tipoDeOcorrencia.GerarOcorrenciaAutomaticamenteRejeicaoMobile && !tipoDeOcorrencia.SalvarCausadorDaOcorrenciaNaGestaoDeOcorrencias))
                return;

            Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia repConfiguracaoOcorrencia = new Repositorio.Embarcador.Configuracoes.ConfiguracaoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega> pedidosOcorrenciaColetaEntregas = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega>();
            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = repConfiguracaoOcorrencia.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidosEncaixados = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
            if (cargaPedidos.Any(obj => obj.PedidoEncaixado))
                cargaPedidosEncaixados = repCargaPedido.BuscarPorPedidos((from o in cargaPedidos where o.PedidoEncaixado select o.Pedido.Codigo).ToList());

            bool verificarTipoOcorrenciaPorNotaFiscal = !configuracaoOcorrencia.IgnorarSituacaoDasNotasAoGerarOcorrencia && cargaEntregaNotasFiscais?.Count > 0;
            List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega> configuracaoOcorrenciaEntregas = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega>();
            if (verificarTipoOcorrenciaPorNotaFiscal)
                configuracaoOcorrenciaEntregas = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.Confirma);

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configPedidoOcorrencia = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
            if (latitude != null && longitude != null)
            {
                configPedidoOcorrencia.Latitude = latitude;
                configPedidoOcorrencia.Longitude = longitude;
                configPedidoOcorrencia.DataExecucao = DateTime.Now;
                configPedidoOcorrencia.DataPosicao = DateTime.Now;
                configPedidoOcorrencia.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
                configPedidoOcorrencia.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
                configPedidoOcorrencia.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntrega.Carga;

                if (cargaPedido.PedidoEncaixado)
                    carga = (from obj in cargaPedidosEncaixados where obj.Pedido.Codigo == pedido.Codigo && obj.ObterTomador().CPF_CNPJ == pedido.ObterTomador().Codigo select obj).FirstOrDefault()?.Carga ?? null;

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrenciaColetaEntrega = tipoDeOcorrencia;
                if (verificarTipoOcorrenciaPorNotaFiscal && !configuracaoOcorrencia.IgnorarSituacaoDasNotasAoGerarOcorrencia)
                {
                    List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaNotasFiscaisCargaEntrega = cargaEntregaNotasFiscais.Where(o => o.PedidoXMLNotaFiscal.CargaPedido.Codigo == cargaPedido.Codigo).Select(obj => obj.PedidoXMLNotaFiscal.XMLNotaFiscal).ToList();
                    if (listaNotasFiscaisCargaEntrega.Count > 0 && listaNotasFiscaisCargaEntrega.Any(o => o.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Entregue))
                    {
                        Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configuracaoOcorrenciaEntrega = configuracaoOcorrenciaEntregas.Where(o => o.TipoAplicacaoColetaEntrega == TipoAplicacaoColetaEntrega.Entrega).FirstOrDefault();
                        if (configuracaoOcorrenciaEntrega != null)
                            tipoDeOcorrenciaColetaEntrega = configuracaoOcorrenciaEntrega.TipoDeOcorrencia;
                    }
                }

                pedidosOcorrenciaColetaEntregas.Add(GerarPedidoOcorrenciaColetaEntrega(cargaEntrega.Cliente, pedido, carga, tipoDeOcorrenciaColetaEntrega, configuracaoPortalCliente, DataEvento, observacao, eventoColetaEntrega, configuracaoEmbarcador, clienteMultisoftware, configPedidoOcorrencia, unitOfWork, configuracaoOcorrencia: configuracaoOcorrencia));
            }

            GravarOcorrenciaGeradaEntrega(
                cargaEntrega,
                DataEvento,
                tipoDeOcorrencia,
                "",
                0,
                observacao,
                configPedidoOcorrencia,
                unitOfWork,
                auditado,
                imagensOcorrencia,
                anexosOcorrencia,
                tiposCausadoresOcorrencia,
                notasFiscais,
                tipoServicoMultisoftware: tipoServicoMultisoftware,
                origemSituacao
            );
            GerarOcorrenciaPedidoOcorrencia(pedidosOcorrenciaColetaEntregas, cargaEntrega.Carga, componenteFrete, chamado, latitude, longitude, observacao, valor, configuracaoEmbarcador, tipoServicoMultisoftware, clienteMultisoftware, unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnvento(cargaEntrega, DataEvento, eventoColetaEntrega, tipoDeOcorrencia: tipoDeOcorrencia, latitude, longitude, origemSituacao);
            if (configuracaoOcorrenciaEntregas?.Count > 0)
                servicoCargaEntregaEvento.ProcessarEventoEntrega(cargaEntregaEvento, cargaPedidos, configuracaoOcorrenciaEntregas, configuracaoControleEntrega);
            else
                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega);
        }

        public static void GerarOcorrenciaEntregaRecalculoPrevisao(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataEvento, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configPedidoOcorrencia, OrigemSituacaoEntrega origemSituacao, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido repCargaEntregaPedido = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido(unitOfWork);

            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);

            Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente = GestaoEntregas.ConfiguracaoPortalCliente.ObterConfiguracao(unitOfWork);

            List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos = repCargaEntregaPedido.BuscarPedidosPorCargaEntrega(cargaEntrega.Codigo);

            foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido in cargaPedidos)
            {
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = cargaPedido.Pedido;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaEntrega.Carga;

                GerarPedidoOcorrenciaColetaEntrega(cargaEntrega.Cliente, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, dataEvento, EventoColetaEntrega.RecalculoPrevisao, configuracaoEmbarcador, clienteMultisoftware, configPedidoOcorrencia, unitOfWork);
            }

            GravarOcorrenciaGeradaEntrega(
                cargaEntrega,
                dataEvento,
                tipoDeOcorrencia,
                "",
                0,
                string.Empty,
                configPedidoOcorrencia,
                unitOfWork,
                auditado,
                tipoServicoMultisoftware: tipoServicoMultisoftware,
                origemSituacaoEntrega: origemSituacao
            );

            Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, EventoColetaEntrega.RecalculoPrevisao, tipoDeOcorrencia, configPedidoOcorrencia.Latitude, configPedidoOcorrencia.Longitude, origemSituacao);
            servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, string observacao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao, dataOcorrencia: DateTime.Now, EventoColetaEntrega: null, pacote: "", volumes: 0, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: null, unitOfWork: unitOfWork);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, DateTime dataOcorrencia, string observacao, string pacotes, int volumes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao: observacao, dataOcorrencia: dataOcorrencia, EventoColetaEntrega: null, pacote: pacotes, volumes: volumes, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: null, unitOfWork: unitOfWork);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, DateTime dataOcorrencia, string observacao, string pacotes, int volumes, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Ocorrencia.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega = null)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao: observacao, dataOcorrencia: dataOcorrencia, EventoColetaEntrega: null, pacote: pacotes, volumes: volumes, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: null, unitOfWork: unitOfWork, pedidoOcorrenciaColetaEntregaIntegracao: pedidoOcorrenciaColetaEntrega);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, DateTime dataOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega EventoColetaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenadas, Repositorio.UnitOfWork unitOfWork, bool gerarNotificacao = true)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao: string.Empty, dataOcorrencia: dataOcorrencia, EventoColetaEntrega: EventoColetaEntrega, pacote: "", volumes: 0, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: configOcorrenciaCoordenadas, unitOfWork: unitOfWork, gerarNotificacao);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, DateTime dataOcorrencia, string observacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega EventoColetaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenadas, Repositorio.UnitOfWork unitOfWork, bool gerarNotificacao = true, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoOcorrencia configuracaoOcorrencia = null)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao: observacao, dataOcorrencia: dataOcorrencia, EventoColetaEntrega: EventoColetaEntrega, pacote: "", volumes: 0, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: configOcorrenciaCoordenadas, unitOfWork: unitOfWork, gerarNotificacao, configuracaoOcorrencia: configuracaoOcorrencia);
        }

        public static Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega GerarPedidoOcorrenciaColetaEntrega(Dominio.Entidades.Cliente alvo, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia, Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoPortalCliente configuracaoPortalCliente, string observacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega EventoColetaEntrega, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Repositorio.UnitOfWork unitOfWork, bool gerarIntegracao)
        {
            return GerarPedidoOcorrenciaColetaEntrega(alvo, pedido, carga, tipoDeOcorrencia, configuracaoPortalCliente, observacao, dataOcorrencia: DateTime.Now, EventoColetaEntrega: EventoColetaEntrega, pacote: "", volumes: 0, configuracao: configuracao, clienteMultisoftware: clienteMultisoftware, configOcorrenciaCoordenadas: null, unitOfWork: unitOfWork, gerarIntegracao: gerarIntegracao);
        }

        //atualização do inicio e fim viagem geramos eventos de integracao.
        public static void GerarEventosColetaEntregaCargaAtualizacaoInicioFimViagem(Dominio.Entidades.Embarcador.Cargas.Carga carga, EventoColetaEntrega evento, DateTime dataEvento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemAuditado origem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            if (evento == EventoColetaEntrega.IniciaViagem)
            {
                // 1 deve gerar evento de inicio viagem
                // 2 caso ainda nao tem entregas, atualizar previsoes dinamicas e enviar evento de previsao dinamica.

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> cargaEntregas = repCargaEntrega.BuscarPorCarga(carga.Codigo);

                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configInicioViagem = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.IniciaViagem)?.FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configRecalculo = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EventoColetaEntrega.RecalculoPrevisao)?.FirstOrDefault();

                if (configInicioViagem == null)
                    return;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnventoCarga(carga, dataEvento, evento, configInicioViagem.TipoDeOcorrencia, carga.LatitudeInicioViagem, carga.LongitudeInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);

                if (configRecalculo != null && !cargaEntregas.Any(obj => SituacaoEntregaHelper.ObterSituacaoEntregaFinalizada(obj.Situacao)))
                {
                    foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in cargaEntregas)
                    {
                        if (cargaEntrega.Carga.CargaDePreCarga)
                            continue;

                        Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEventoPorEntrega = new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento()
                        {
                            Carga = carga,
                            CargaEntrega = cargaEntrega,
                            DataOcorrencia = dataEvento,
                            DataPosicao = dataEvento,
                            EventoColetaEntrega = EventoColetaEntrega.RecalculoPrevisao,
                            DataPrevisaoRecalculada = cargaEntrega.DataReprogramada,
                            TipoDeOcorrencia = configRecalculo.TipoDeOcorrencia,
                            Latitude = carga.LatitudeInicioViagem,
                            Longitude = carga.LongitudeInicioViagem,
                            Origem = OrigemSituacaoEntrega.UsuarioMultiEmbarcador,
                        };

                        servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEventoPorEntrega, configuracaoControleEntrega, true);
                    }
                }
            }
            else if (evento == EventoColetaEntrega.FimViagem)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configFimViagem = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.FimViagem)?.FirstOrDefault();

                if (configFimViagem == null)
                    return;

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnventoCarga(carga, dataEvento, evento, configFimViagem.TipoDeOcorrencia, carga.LatitudeInicioViagem, carga.LongitudeInicioViagem, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);
            }
        }

        //atualização das datas de entrega integracao.
        public static void GerarEventosColetaEntregaAtualizacaoDatasEntrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, DateTime dataEvento, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, OrigemAuditado origem, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega, Repositorio.UnitOfWork unitOfWork)
        {

            Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega repConfiguracaoOcorrenciaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega(unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

            //1 - Se a entrega com a data atualizada for a última que foi entregue na carga, integrar o evento de saída do cliente(004 / 009) e recalcular a previsão estática das entregas subsequentes, integrando novamente essas previsões.
            //2 - Se a entrega com a data atualizada não for a última entregue, e também não é a última da carga, apenas integrar o evento de saída do cliente(004 / 009)
            //3 - Se a entrega com a data atualizada for a último da carga, integrar o evento de saída de último cliente(005 / 009) e também atualizar o fim de viagem, integrando o evento 006

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configConfirma = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.Confirma)?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configUltimaConfirmacao = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.UltimaConfirmacao)?.FirstOrDefault();
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.ConfiguracaoOcorrenciaEntrega configFimViagem = repConfiguracaoOcorrenciaEntrega.BuscarRegrasPorEvento(EventoColetaEntrega.FimViagem)?.FirstOrDefault();

            //Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimacargaEntregaFinalizadaPorData = repCargaEntrega.BuscarUltimaCargaEntregaRealizada(cargaEntrega.Carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimacargaEntregaFinalizadaPorOrdem = repCargaEntrega.BuscarUltimaCargaEntregaRealizadaPorOrdem(cargaEntrega.Carga.Codigo);

            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega ultimaCargaEntrega = repCargaEntrega.BuscarUltimaCargaEntrega(cargaEntrega.Carga.Codigo);

            if (cargaEntrega.Codigo == ultimacargaEntregaFinalizadaPorOrdem.Codigo && cargaEntrega.Codigo != ultimaCargaEntrega.Codigo && configConfirma != null)
            {
                //é ultimo entregue mas nao igual ao ultimo da carga enviar evendo de saida do cliente (tem q recalcular previsoes entrega para entregas em aberto)
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEvento = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, EventoColetaEntrega.Confirma, configConfirma.TipoDeOcorrencia, cargaEntrega.LatitudeConfirmacaoChegada, cargaEntrega.LongitudeConfirmacaoChegada, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);
                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEvento, configuracaoControleEntrega, true);

                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.AtualizarPrevisaoCargaEntrega(cargaEntrega.Carga, configuracaoEmbarcador, unitOfWork, DateTime.MinValue, tipoServicoMultisoftware);
            }
            else if (cargaEntrega.Codigo == ultimaCargaEntrega.Codigo && configUltimaConfirmacao != null && configFimViagem != null)
            {
                //é a ultima entrega da carga envia o evento de confirmacao e envia o evento de fim de viagem
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEventoConfirmacao = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, EventoColetaEntrega.UltimaConfirmacao, configUltimaConfirmacao.TipoDeOcorrencia, cargaEntrega.LatitudeConfirmacaoChegada, cargaEntrega.LongitudeConfirmacaoChegada, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEventoConfirmacao, configuracaoControleEntrega, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEventoFimViagem = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, EventoColetaEntrega.FimViagem, configFimViagem.TipoDeOcorrencia, cargaEntrega.LatitudeConfirmacaoChegada, cargaEntrega.LongitudeConfirmacaoChegada, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEventoFimViagem, configuracaoControleEntrega, true);
            }
            if (cargaEntrega.Codigo != ultimacargaEntregaFinalizadaPorOrdem.Codigo && cargaEntrega.Codigo != ultimaCargaEntrega.Codigo && configConfirma != null)
            {
                //evento de saida do cliente
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEventoConfirmacao = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, EventoColetaEntrega.Confirma, configConfirma.TipoDeOcorrencia, cargaEntrega.LatitudeConfirmacaoChegada, cargaEntrega.LongitudeConfirmacaoChegada, OrigemSituacaoEntrega.UsuarioMultiEmbarcador);

                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEventoConfirmacao, configuracaoControleEntrega, true);
            }

        }

        public void GerarOcorrenciaEntregaPorGatilho(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, TipoDataAlteracaoGatilho tipoDataAlteracaoGatilho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo servicoOcorrenciaAutomaticaPorPeriodo = new Servicos.Embarcador.CargaOcorrencia.OcorrenciaAutomaticaPorPeriodo(_unitOfWork);

            int codigoFilial = cargaEntrega.Carga.Filial?.Codigo ?? 0;
            int codigoEmpresa = cargaEntrega.Carga.Empresa?.Codigo ?? 0;
            int codigoTipoOperacao = cargaEntrega.Carga.TipoOperacao?.Codigo ?? 0;

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = servicoOcorrenciaAutomaticaPorPeriodo.ObterListaGatilhos(tipoDataAlteracaoGatilho, codigoFilial, codigoEmpresa, codigoTipoOperacao);

            List<SituacaoEntrega> listaSituacaoEmAberto = SituacaoEntregaHelper.ObterListaSituacaoEntregaEmAberto();

            if ((listaGatilhoGeracaoAutomatica == null || listaGatilhoGeracaoAutomatica.Count == 0) || !listaSituacaoEmAberto.Contains(cargaEntrega.Situacao))
                return;

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomaticaOcorrencia in listaGatilhoGeracaoAutomatica)
            {
                ConfigurarEGerarOcorrenciaEntrega(cargaEntrega, gatilhoGeracaoAutomaticaOcorrencia, configuracaoControleEntrega, tipoServicoMultisoftware);
            }
        }

        public void GerarOcorrenciaEntregaPorGatilhoAutomatico(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia repositorioGatilhoGeracaoAutomaticaOcorrencia = new Repositorio.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia(_unitOfWork);
            Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega repConfiguracaoControleEntrega = new Repositorio.Embarcador.Configuracoes.ConfiguracaoControleEntrega(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega = repConfiguracaoControleEntrega.BuscarPrimeiroRegistro();

            List<Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia> listaGatilhoGeracaoAutomatica = repositorioGatilhoGeracaoAutomaticaOcorrencia.BuscarPorGatilhoAutomaticoAtingirData();

            if (listaGatilhoGeracaoAutomatica == null || listaGatilhoGeracaoAutomatica.Count == 0)
                return;

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomaticaOcorrencia in listaGatilhoGeracaoAutomatica)
            {
                List<int> codigosFilial = gatilhoGeracaoAutomaticaOcorrencia.Filiais.Select(obj => obj.Codigo).ToList();
                List<int> codigosEmpresa = gatilhoGeracaoAutomaticaOcorrencia.Transportadores.Select(obj => obj.Codigo).ToList();
                List<int> codigosTipoOperacao = gatilhoGeracaoAutomaticaOcorrencia.TiposOperacoes.Select(obj => obj.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega> listaCargaEntrega = repositorioCargaEntrega.ObterEntregasAtingiramData(codigosFilial, codigosTipoOperacao, codigosEmpresa, gatilhoGeracaoAutomaticaOcorrencia.TipoOcorrencia.Codigo, gatilhoGeracaoAutomaticaOcorrencia.TipoCobrancaMultimodal, limiteRegistros: 10);

                foreach (Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega in listaCargaEntrega)
                {
                    try
                    {
                        ConfigurarEGerarOcorrenciaEntrega(cargaEntrega, gatilhoGeracaoAutomaticaOcorrencia, configuracaoControleEntrega, tipoServicoMultisoftware);
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex, "ThreadOcorrenciaEntrega");
                    }

                    _unitOfWork.FlushAndClear();
                }
            }
        }

        public void ConfigurarEGerarOcorrenciaEntrega(
            Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega,
            Dominio.Entidades.Embarcador.Ocorrencias.GatilhoGeracaoAutomaticaOcorrencia gatilhoGeracaoAutomaticaOcorrencia,
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoControleEntrega configuracaoControleEntrega,
            TipoServicoMultisoftware tipoServicoMultisoftware
        )
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao repOcorrenciaTipoIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao(_unitOfWork);
            Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento servicoCargaEntregaEvento = new Servicos.Embarcador.Carga.ControleEntrega.CargaEntregaEvento(_unitOfWork);

            Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoDeOcorrencia = gatilhoGeracaoAutomaticaOcorrencia.TipoOcorrencia;
            string observacao = gatilhoGeracaoAutomaticaOcorrencia.ObterObservacaoOcorrencia();
            DateTime dataEvento = DateTime.Now;

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas configOcorrenciaCoordenada = new Dominio.ObjetosDeValor.Embarcador.Pedido.ConfiguracaoOcorrenciaCoordenadas();
            configOcorrenciaCoordenada.Latitude = 0;
            configOcorrenciaCoordenada.Longitude = 0;
            configOcorrenciaCoordenada.DataExecucao = dataEvento;
            configOcorrenciaCoordenada.DataPosicao = dataEvento;
            configOcorrenciaCoordenada.DistanciaAteDestino = cargaEntrega.DistanciaAteDestino > 0 ? cargaEntrega.DistanciaAteDestino : cargaEntrega.Distancia;
            configOcorrenciaCoordenada.DataPrevisaoRecalculada = cargaEntrega.DataReprogramada ?? null;
            configOcorrenciaCoordenada.TempoPercurso = cargaEntrega.DataReprogramada.HasValue ? Servicos.Embarcador.Monitoramento.AlertaMonitor.FormatarTempo(DateTime.Now - cargaEntrega.DataReprogramada.Value) : "";

            GravarOcorrenciaGeradaEntrega(
                cargaEntrega,
                dataEvento,
                tipoDeOcorrencia,
                "",
                0,
                observacao,
                configOcorrenciaCoordenada,
                _unitOfWork,
                new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado(),
                tipoServicoMultisoftware: tipoServicoMultisoftware
            );

            if (!tipoDeOcorrencia.TipoOcorrenciaControleEntrega) return;

            List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaTipoIntegracao> ocorrenciaTiposIntegracao = repOcorrenciaTipoIntegracao.BuscarPorTipoOcorrencia(tipoDeOcorrencia.Codigo);
            if (ocorrenciaTiposIntegracao.Count > 0)
            {
                EventoColetaEntrega eventoColetaEntrega = EventoColetaEntrega.AlteracaoDataAgendamentoEntregaTransportador;
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.CargaEntregaEvento cargaEntregaEventoConfirmacao = GerarObjetoCargaEntregaEnvento(cargaEntrega, dataEvento, eventoColetaEntrega, tipoDeOcorrencia, 0, 0, OrigemSituacaoEntrega.Automatico);
                servicoCargaEntregaEvento.GerarCargaEntregaEvento(cargaEntregaEventoConfirmacao, configuracaoControleEntrega);
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static OrigemCriacaoOcorrencia ObterOrigemOcorrencia(OrigemSituacaoEntrega? origemSituacaoEntrega, TipoServicoMultisoftware? tipoServico = null)
        {
            if (!origemSituacaoEntrega.HasValue)
            {
                if (tipoServico == TipoServicoMultisoftware.MultiEmbarcador)
                {
                    return OrigemCriacaoOcorrencia.ManualEmbarcador;
                }
                else if (tipoServico == TipoServicoMultisoftware.MultiCTe)
                {
                    return OrigemCriacaoOcorrencia.ManualTransportador;
                }
                else
                {
                    return OrigemCriacaoOcorrencia.Manual;
                }
            }

            switch (origemSituacaoEntrega.Value)
            {
                case OrigemSituacaoEntrega.MonitoramentoAutomaticamente: return OrigemCriacaoOcorrencia.Monitoramento;
                case OrigemSituacaoEntrega.WebService: return OrigemCriacaoOcorrencia.Integracao;
                case OrigemSituacaoEntrega.App: return OrigemCriacaoOcorrencia.MultiMobile;
                case OrigemSituacaoEntrega.UsuarioMultiEmbarcador: return OrigemCriacaoOcorrencia.ManualEmbarcador;
                case OrigemSituacaoEntrega.UsuarioPortalTransportador: return OrigemCriacaoOcorrencia.ManualTransportador;
                case OrigemSituacaoEntrega.LiberacaoCanhoto: return OrigemCriacaoOcorrencia.Integracao;
                case OrigemSituacaoEntrega.AlertaMonitoramento: return OrigemCriacaoOcorrencia.Monitoramento;
                case OrigemSituacaoEntrega.ArquivoEDI: return OrigemCriacaoOcorrencia.Integracao;
                case OrigemSituacaoEntrega.Correios: return OrigemCriacaoOcorrencia.Integracao;
                case OrigemSituacaoEntrega.Automatico: return OrigemCriacaoOcorrencia.Monitoramento;
                default: return OrigemCriacaoOcorrencia.Manual;
            }
        }

        private static void SalvarNotasFiscais(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrenciaColetaEntrega, Repositorio.UnitOfWork unitOfWork, List<int> codigosNotasFiscais)
        {
            if (codigosNotasFiscais == null || codigosNotasFiscais.Count == 0) return;

            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal repOcorrenciaColetaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal(unitOfWork);
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);

            foreach (int codigoNotaFiscal in codigosNotasFiscais)
            {
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal ocorrenciaColetaEntregaNotaFiscal = new Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal();

                ocorrenciaColetaEntregaNotaFiscal.OcorrenciaColetaEntrega = ocorrenciaColetaEntrega;
                ocorrenciaColetaEntregaNotaFiscal.XMLNotaFiscal = repNotaFiscal.BuscarPorCodigo(codigoNotaFiscal);

                repOcorrenciaColetaEntregaNotaFiscal.Inserir(ocorrenciaColetaEntregaNotaFiscal);
            }
        }

        #endregion
    }
}