using Dominio.Entidades.Embarcador.GestaoEntregas;
using Dominio.ObjetosDeValor.Embarcador.GestaoEntregas;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.GestaoEntregas
{
    [CustomAuthorize(new string[] { "ObterEtapasFluxo", "ConfiguracoesGestaoEntrega", "ObterFluxoEntrega", "BuscarDetalhesEntrega" }, "GestaoEntregas/FluxoEntrega")]
    public class FluxoEntregaController : BaseController
    {
		#region Construtores

		public FluxoEntregaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        public async Task<IActionResult> ObterEtapasFluxo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao>()
                {
                    servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.InicioViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem),
                    servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Posicao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem),
                    servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Entregas, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem),
                    servicoFluxoGestaoPatioConfiguracaoEtapa.ObterDescricaoEtapa(Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.FimViagem, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem),
                };

                return new JsonpResult(situacoes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfiguracoesGestaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega repConfiguracaoGestaoPatio = new Repositorio.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega configuracao = repConfiguracaoGestaoPatio.BuscarConfiguracao();

                if (configuracao == null)
                {
                    configuracao = new Dominio.Entidades.Embarcador.GestaoEntregas.ConfiguracaoGestaoEntrega();
                    repConfiguracaoGestaoPatio.Inserir(configuracao);
                }

                return new JsonpResult(new
                {
                    configuracao.OcultarFluxoCarga,
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterFluxoEntregaPorcarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(unitOfWork);
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = repConfiguracaoGestaoPatio.BuscarConfiguracao();
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                int carga = Request.GetIntParam("Carga");

                Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntrega = repFluxoGestaoEntrega.BuscarPorCarga(carga);


                if (fluxoGestaoEntrega == null)
                    return new JsonpResult(false, "Não existem entregas para esta carga.");

                var retorno = new { Fluxo = ObterDetalhesFluxoEntrega(fluxoGestaoEntrega, configuracao, configuracaoEmbarcador, unitOfWork) };


                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterFluxoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(unitOfWork);
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(unitOfWork);

                int codigoFilial = Request.GetIntParam("Filial");
                double destinatario = Request.GetDoubleParam("Destinatario");

                int.TryParse(Request.Params("Transportador"), out int codigoTransportador);

                DateTime dataInicial = Utilidades.Conversor.ExtraiDateTime(Request.Params("DataInicial"));
                DateTime dataFinal = Utilidades.Conversor.ExtraiDateTime(Request.Params("DataFinal"));

                string numeroCarga = Request.Params("CodigoCargaEmbarcador");
                string placa = Request.Params("Placa");
                string pedido = Request.Params("Pedido");
                string ordenacao = Request.Params("Ordenacao");
                int numeroPedido = Request.GetIntParam("Pedido");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    pedido = "";
                else
                    numeroPedido = 0;

                int inicio = int.Parse(Request.Params("inicio"));
                int limite = int.Parse(Request.Params("limite"));

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega? situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega>("Situacao");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio etapaFluxoGestaoPatio = Request.GetEnumParam("EtapaFluxoGestaoEntrega", Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Todas);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = repConfiguracaoGestaoPatio.BuscarConfiguracao();
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(codigoFilial);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);
                bool existeSequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ExisteSequenciaGestaoPatio(codigoFilial, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoFluxoGestaoPatio.Origem);

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS && !existeSequenciaGestaoPatio)
                    return new JsonpResult(false, true, "A filial não possui uma sequencia para gestão de pátio.");

                int total = repFluxoGestaoEntrega.ContarConsulta(situacao, etapaFluxoGestaoPatio, numeroCarga, dataInicial, dataFinal, codigoFilial, destinatario, placa, pedido, numeroPedido, codigoTransportador);
                List<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega> fluxoGestaoEntrega = total > 0 ? repFluxoGestaoEntrega.Consultar(situacao, etapaFluxoGestaoPatio, numeroCarga, dataInicial, dataFinal, codigoFilial, destinatario, placa, pedido, numeroPedido, codigoTransportador, "DataCriacao", ordenacao, inicio, limite) : new List<Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega>();

                List<dynamic> lista = (from obj in fluxoGestaoEntrega select ObterDetalhesFluxoEntrega(obj, configuracao, configuracaoEmbarcador, unitOfWork)).ToList();


                return new JsonpResult(lista, total);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o fluxo de pátio.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio repConfiguracaoGestaoPatio = new Repositorio.Embarcador.Filiais.ConfiguracaoGestaoPatio(unitOfWork);
                Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao = repConfiguracaoGestaoPatio.BuscarConfiguracao();
                Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntrega = repFluxoGestaoEntrega.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = ObterConfiguracaoEmbarcador(unitOfWork);

                var retorno = ObterDetalhesFluxoEntrega(fluxoGestaoEntrega, configuracao, configuracaoEmbarcador, unitOfWork);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por codigo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Carga"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntregas = repFluxoGestaoEntrega.BuscarPorCarga(codigo);

                // Valida
                if (fluxoGestaoEntregas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    fluxoGestaoEntregas.Carga.Codigo,
                    ViagemAberta = !fluxoGestaoEntregas.DataInicioViagem.HasValue,
                    DataInicioViagem = fluxoGestaoEntregas.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarInicioViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega repFluxoGestaoEntrega = new Repositorio.Embarcador.GestaoEntregas.FluxoGestaoEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntregas = repFluxoGestaoEntrega.BuscarPorCarga(codigo);

                // Valida
                if (fluxoGestaoEntregas == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();

                if (ConfiguracaoEmbarcador.PossuiMonitoramento)
                    Servicos.Embarcador.Monitoramento.Monitoramento.IniciarMonitoramento(fluxoGestaoEntregas.Carga, fluxoGestaoEntregas.DataInicioViagem, ConfiguracaoEmbarcador, base.Auditado, unitOfWork);

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao executar ação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDetalhesEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.EntregaPedido repEntregaPedido = new Repositorio.Embarcador.GestaoEntregas.EntregaPedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido entregaPedido = repEntregaPedido.BuscarPorCodigo(codigo);

                // Valida
                if (entregaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a entega.");


                // Formata retorno
                var retorno = new
                {
                    entregaPedido.Codigo,
                    Numero = entregaPedido.Pedido.NumeroPedidoEmbarcador,
                    Destinatario = entregaPedido.Pedido.Destinatario?.Descricao ?? string.Empty,
                    Localidade = entregaPedido.Pedido.Destinatario?.Localidade.Descricao ?? string.Empty,
                    Situacao = entregaPedido.DescricaoSituacao,
                    EnumSituacao = entregaPedido.Situacao,
                    DataPrevisaoEntrega = entregaPedido.DataPrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataEntrega = entregaPedido.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    DataRejeitado = entregaPedido.DataRejeitado?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    entregaPedido.Avaliacao,
                    Observacao = (entregaPedido.Observacao ?? "").Replace("\n", "<br />"),
                    entregaPedido.NomeRecebedor,
                    entregaPedido.DocumentoRecebedor,
                    JanelaDescarga = string.Join(", ", entregaPedido?.Pedido?.Destinatario?.ClienteDescargas.Select(o => o.HoraInicioDescarga + " - " + o.HoraLimiteDescarga)),
                    //FluxoAberto = entregaPedido.Etapa.FluxoGestaoEntrega.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega.Aguardando,

                    LocalEntrega = new
                    {
                        entregaPedido.Latitude,
                        entregaPedido.Longitude,
                    },

                    LocalCliente = new
                    {
                        Latitude = (entregaPedido.Pedido.Destinatario.Latitude ?? "").Replace(".", ",").ToDecimal(),
                        Longitude = (entregaPedido.Pedido.Destinatario.Longitude ?? "").Replace(".", ",").ToDecimal(),
                    },

                    Imagens = (from o in entregaPedido.Fotos
                               select new
                               {
                                   o.Codigo,
                                   Miniatura = Base64Imagem(o, unitOfWork)
                               }).ToList(),
                };

                // Retorna informacoes
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ConfirmarPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.EntregaPedido repEntregaPedido = new Repositorio.Embarcador.GestaoEntregas.EntregaPedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido entregaPedido = repEntregaPedido.BuscarPorCodigo(codigo);

                // Valida
                if (entregaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a entega.");

                if (entregaPedido.Etapa.FluxoGestaoEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega.Aguardando)
                    return new JsonpResult(false, true, "Não é possível alterar a entrega após o fluxo de sido finalizado.");

                unitOfWork.Start();

                entregaPedido.Initialize();

                entregaPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.Entregue;
                entregaPedido.DataEntrega = DateTime.Now;
                entregaPedido.Pedido.DataEntrega = entregaPedido?.Pedido != null ? entregaPedido.DataEntrega : null;

                repEntregaPedido.Atualizar(entregaPedido, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por codigo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarPedido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.EntregaPedido repEntregaPedido = new Repositorio.Embarcador.GestaoEntregas.EntregaPedido(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedido entregaPedido = repEntregaPedido.BuscarPorCodigo(codigo);

                // Valida
                if (entregaPedido == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a entega.");

                if (entregaPedido.Etapa.FluxoGestaoEntrega.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEtapaFluxoGestaoEntrega.Aguardando)
                    return new JsonpResult(false, true, "Não é possível alterar a entrega após o fluxo de sido finalizado.");

                unitOfWork.Start();

                entregaPedido.Initialize();

                entregaPedido.PendenteIntegracao = true;
                entregaPedido.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.Rejeitado;
                entregaPedido.DataRejeitado = DateTime.Now;
                entregaPedido.DataEntrega = null;
                if (entregaPedido.Pedido != null)
                    entregaPedido.Pedido.DataEntrega = null;

                repEntregaPedido.Atualizar(entregaPedido, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por codigo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExibirAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoEntregas.EntregaPedidoFoto repEntregaPedidoFoto = new Repositorio.Embarcador.GestaoEntregas.EntregaPedidoFoto(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoEntregas.EntregaPedidoFoto anexo = repEntregaPedidoFoto.BuscarPorCodigo(codigo);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a entega.");

                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
                string extensao = Path.GetExtension(anexo.NomeArquivo);
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensao);

                return File(arquivo, "image/jpeg");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS ObterConfiguracaoEmbarcador(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

            return repositorioConfiguracaoTMS.BuscarConfiguracaoPadrao();
        }

        private string ConcatenarCodigosIntegradoes(IEnumerable<string> codigos)
        {
            return String.Join(" - ", (from o in codigos where !string.IsNullOrEmpty(o) select o));
        }

        private string ConcatenarDestinatarios(List<Dominio.Entidades.Embarcador.Cargas.CargaPedido> cargaPedidos)
        {
            return ConcatenarCodigosIntegradoes(from o in cargaPedidos select o.Pedido.Destinatario.CodigoIntegracao);
        }

        private bool ObterPrevisaoNaJanelaDescarga(Dominio.Entidades.Cliente cliente, DateTime? dataPrevisao)
        {
            if ((cliente == null) || (dataPrevisao == null))
                return true;

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> listaDescarga = (from obj in cliente?.ClienteDescargas
                                 where string.IsNullOrEmpty(obj?.HoraInicioDescarga) == false &&
                                       string.IsNullOrEmpty(obj?.HoraLimiteDescarga) == false       
                                 select obj).ToList();

            if (listaDescarga.Count == 0) 
                return true;

            DateTime? horaPrevisao = Convert.ToDateTime(dataPrevisao?.ToString("HH:mm:ss"));

            List<Dominio.Entidades.Embarcador.Pessoas.ClienteDescarga> descargasNoHorario = (from obj in cliente.ClienteDescargas
                                                                                             where Convert.ToDateTime(obj.HoraInicioDescarga) < horaPrevisao && Convert.ToDateTime(obj.HoraLimiteDescarga) > horaPrevisao
                                                                                             select obj).ToList();
            return descargasNoHorario.Count > 0;

        }

        private dynamic ObterDetalhesFluxoEntrega(Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntrega, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracao, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Carga.Carga servicoCarga = new Servicos.Embarcador.Carga.Carga(unitOfWork);
            Repositorio.Embarcador.Pessoas.ClienteDescarga repClienteDescarga = new Repositorio.Embarcador.Pessoas.ClienteDescarga(unitOfWork);

            var retorno = new
            {
                fluxoGestaoEntrega.Codigo,
                CargaCancelada = fluxoGestaoEntrega.Carga != null && (fluxoGestaoEntrega.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Cancelada || fluxoGestaoEntrega.Carga.SituacaoCarga == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCarga.Anulada),
                Carga = fluxoGestaoEntrega.Carga?.Codigo ?? 0,
                NumeroCarregamento = servicoCarga.ObterNumeroCarga(fluxoGestaoEntrega.Carga, configuracaoEmbarcador),
                fluxoGestaoEntrega.Situacao,
                fluxoGestaoEntrega.EtapaAtual,
                fluxoGestaoEntrega.IndexEtapa,

                GuaritaSaidaDescricao = configuracao?.GuaritaSaidaDescricao ?? string.Empty,
                PosicaoDescricao = configuracao?.PosicaoDescricao ?? string.Empty,
                EntregasDescricao = configuracao?.EntregasDescricao ?? string.Empty,
                FimViagemDescricao = configuracao?.FimViagemDescricao ?? string.Empty,

                DataInicioViagem = fluxoGestaoEntrega.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemPrevista = fluxoGestaoEntrega.DataInicioViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataInicioViagemReprogramada = fluxoGestaoEntrega.DataInicioViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoEntrega.DiferencaInicioViagem,

                DataPosicao = fluxoGestaoEntrega.DataPosicao?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagem = fluxoGestaoEntrega.DataFimViagem?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemPrevista = fluxoGestaoEntrega.DataFimViagemPrevista?.ToString("dd/MM/yyyy HH:mm") ?? "",
                DataFimViagemReprogramada = fluxoGestaoEntrega.DataFimViagemReprogramada?.ToString("dd/MM/yyyy HH:mm") ?? "",
                fluxoGestaoEntrega.DiferencaFimViagem,
                DataEntrega = fluxoGestaoEntrega.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? "",

                Placas = fluxoGestaoEntrega.Carga != null ? ObterPlacas(fluxoGestaoEntrega.Carga.Veiculo, fluxoGestaoEntrega.Carga.VeiculosVinculados) : string.Empty,
                DataCarga = fluxoGestaoEntrega.Carga?.DataCarregamentoCarga?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                Remetente = fluxoGestaoEntrega.Carga?.DadosSumarizados.CodigoIntegracaoRemetentes ?? string.Empty,
                Transportador = fluxoGestaoEntrega.Carga != null ? fluxoGestaoEntrega.Carga.Empresa?.NomeFantasia : string.Empty,
                TipoOperacao = fluxoGestaoEntrega.Carga != null ? fluxoGestaoEntrega.Carga.TipoOperacao?.Descricao : string.Empty,
                Destinatario = fluxoGestaoEntrega.Carga != null ? ConcatenarDestinatarios(fluxoGestaoEntrega.Carga.Pedidos.ToList()) : string.Empty,

                Etapas = (from etapa in fluxoGestaoEntrega.GetEtapas()
                          select new EtapaFluxoEntrega
                          {
                              Etapa = etapa.Etapa,
                              EtapaLiberada = etapa.EtapaLiberada,
                              Pedido = new
                              {
                                  Codigo = etapa.EntregaPedido?.Codigo ?? 0,
                                  Numero = ObterNumero(fluxoGestaoEntrega, etapa.EntregaPedido, configuracaoEmbarcador, unitOfWork),
                                  Situacao = etapa.EntregaPedido?.Situacao ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoEntregaPedido.NaoEntregue,
                                  DataPrevisaoEntrega = etapa.EntregaPedido?.Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? etapa.EntregaPedido?.Pedido.PrevisaoEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                  DataEntrega = etapa.EntregaPedido?.Pedido.DataEntrega?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                  DataRejeitado = etapa.EntregaPedido?.DataRejeitado?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                  DataPrevisaoEntregaReprogramada = etapa.EntregaPedido?.Pedido?.PrevisaoEntregaAtual?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                                  etapa.EntregaPedido?.DiferencaEntrega,
                                  PrivisaoEntergaNaJanela = ObterPrevisaoNaJanelaDescarga(etapa.EntregaPedido?.Pedido?.Destinatario, etapa.EntregaPedido?.Pedido?.PrevisaoEntregaAtual?? etapa.EntregaPedido?.Pedido.PrevisaoEntrega)
                              }
                          }).ToList(),
            };
            return retorno;
        }

        private string ObterNumero(Dominio.Entidades.Embarcador.GestaoEntregas.FluxoGestaoEntrega fluxoGestaoEntrega, EntregaPedido entregaPedido, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            if (entregaPedido == null)
                return string.Empty;

            if (configuracaoEmbarcador.UtilizarNumeroNotaFluxoEntregas)
            {
                Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repositorioCargaPedido.BuscarPorCargaEPedido(fluxoGestaoEntrega.Carga.Codigo, entregaPedido.Pedido.Codigo);

                if (cargaPedido != null && !cargaPedido.PedidoSemNFe)
                    return string.Join(", ", (from notaFiscal in cargaPedido.NotasFiscais where cargaPedido.NotasFiscais != null select notaFiscal.XMLNotaFiscal.Numero));
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return entregaPedido.Pedido.Numero.ToString();

            return entregaPedido.Pedido.NumeroPedidoEmbarcador;
        }

        private List<EtapaFluxoEntrega> EtapasFluxoComPedidos(FluxoGestaoEntrega fluxoGestaoEntrega)
        {
            List<EtapaFluxoEntrega> etapas = new List<EtapaFluxoEntrega>();

            foreach (var etapa in fluxoGestaoEntrega.GetEtapas())
            {
                if (etapa.Etapa != Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio.Entregas)
                {
                    etapas.Add(new EtapaFluxoEntrega
                    {
                        Etapa = etapa.Etapa,
                        EtapaLiberada = etapa.EtapaLiberada,
                    });
                }
                else
                {
                    foreach (var pedido in fluxoGestaoEntrega.Carga.Pedidos)
                    {
                        etapas.Add(new EtapaFluxoEntrega
                        {
                            Etapa = etapa.Etapa,
                            EtapaLiberada = etapa.EtapaLiberada,
                            Pedido = new EtapaFluxoEntregaPedido
                            {
                                Codigo = pedido.Pedido.Codigo,
                                Numero = pedido.Pedido.NumeroPedidoEmbarcador,
                            }
                        });
                    }
                }
            }

            return etapas;
        }

        private string ObterPlacas(Dominio.Entidades.Veiculo veiculo, IEnumerable<Dominio.Entidades.Veiculo> veiculosVinculados)
        {
            if (veiculo != null)
            {
                List<string> placas = new List<string>() { veiculo.Placa };
                placas.AddRange(veiculosVinculados.Select(o => o.Placa));

                return string.Join(", ", placas);
            }
            else
                return "";

        }

        private string Base64Imagem(EntregaPedidoFoto foto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });
            string extensao = Path.GetExtension(foto.NomeArquivo);
            string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, foto.GuidArquivo + "-miniatura" + extensao);

            if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
            {
                byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                return base64ImageRepresentation;
            }
            else
            {
                return "";
            }

        }

        #endregion
    }
}
