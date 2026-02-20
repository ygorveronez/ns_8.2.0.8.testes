using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
using System.Linq.Dynamic.Core;

namespace SGT.WebAdmin.Controllers.Cargas.GestaoDadosColeta
{
    [CustomAuthorize(new string[] { "BuscarDadosNfePorCodigo", "BuscarDadosTransportePorCodigo" }, "Cargas/GestaoDadosColeta")]
    public class GestaoDadosColetaController : BaseController
    {
        #region Construtores

        public GestaoDadosColetaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
        }

        public async Task<IActionResult> AdicionarGestaoDadosColetaDadosNfe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta servicoGestaoDadosColeta = new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAprovacao dadosNFeAprovacao = ObterDadosNFeAprovacaoSalvar();
                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAdicionar dadosNFeAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAdicionar()
                {
                    CodigoCargaEntrega = Request.GetIntParam("Coleta"),
                    GuidArquivo = Request.GetStringParam("GuidArquivo"),
                    Origem = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? OrigemGestaoDadosColeta.Embarcador : OrigemGestaoDadosColeta.Transportador,
                    OrigemFoto = OrigemFotoDadosNFEGestaoDadosColeta.GestaoDadosColeta
                };

                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe = servicoGestaoDadosColeta.Adicionar(dadosNFeAdicionar);

                servicoGestaoDadosColeta.AtualizarDadosAprovacao(gestaoDadosColetaDadosNFe, dadosNFeAprovacao);

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                    await servicoGestaoDadosColeta.AprovarAsync(gestaoDadosColetaDadosNFe, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarGestaoDadosColetaDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta servicoGestaoDadosColeta = new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosTransporteAdicionar dadosTransporteAdicionar = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosTransporteAdicionar()
                {
                    CodigoCargaEntrega = Request.GetIntParam("Coleta"),
                    Origem = TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador ? OrigemGestaoDadosColeta.Embarcador : OrigemGestaoDadosColeta.Transportador,
                    CodigoTracao = Request.GetIntParam("Veiculo"),
                    CodigoReboque = Request.GetIntParam("Reboque"),
                    CodigoSegundoReboque = Request.GetIntParam("SegundoReboque"),
                    CodigosMotoristas = Request.GetListParam<int>("Motoristas")
                };

                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte = servicoGestaoDadosColeta.Adicionar(dadosTransporteAdicionar);

                if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiEmbarcador)
                    await servicoGestaoDadosColeta.AprovarAsync(gestaoDadosColetaDadosTransporte, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult(Localization.Resources.Gerais.Geral.AdicionadaComSucesso);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoAdicionar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovacaoGestaoDadosColetaDadosNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaDadosNFe = repositorioGestaoDadosColetaNFe.BuscarPorCodigo(codigo);

                if (gestaoDadosColetaDadosNFe == null)
                    throw new ControllerException("Não foi possível encontrar os dados da NF-e.");

                Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta servicoGestaoDadosColeta = new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAprovacao dadosNFeAprovacao = ObterDadosNFeAprovacaoSalvar();

                servicoGestaoDadosColeta.AtualizarDadosAprovacao(gestaoDadosColetaDadosNFe, dadosNFeAprovacao);
                await servicoGestaoDadosColeta.AprovarAsync(gestaoDadosColetaDadosNFe, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult("Dados da NF-e aprovados com sucesso.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os dados da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovacaoGestaoDadosColetaDadosTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repositorioGestaoDadosColetaDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaDadosTransporte = repositorioGestaoDadosColetaDadosTransporte.BuscarPorCodigo(codigo);

                if (gestaoDadosColetaDadosTransporte == null)
                    throw new ControllerException("Não foi possível encontrar os dados de transporte.");

                await new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado).AprovarAsync(gestaoDadosColetaDadosTransporte, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult("Dados de transporte aprovados com sucesso.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os dados de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarGestaoDadosColetaDadosNFe()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                int codigoMotivo = Request.GetIntParam("CodigoMotivoRejeicao");
                int codigoCarga = Request.GetIntParam("CodigoCarga");


                Repositorio.Embarcador.Configuracoes.Motivo repositorioMotivo = new(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.Motivo motivoRejeicao = await repositorioMotivo.BuscarPorCodigoAsync(codigoMotivo, false);

                if (motivoRejeicao == null)
                    throw new ControllerException("Motivo rejeição não encontrado");

                Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta servicoGestaoDadosColeta = new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado);

                await servicoGestaoDadosColeta.ReprovarDadosNFeAsync(codigo, motivoRejeicao, codigoCarga);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult("Dados da NF-e reprovados com sucesso.");
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();

                if (excecao is BaseException)
                    return new JsonpResult(false, true, excecao.Message);
                else
                {
                    Servicos.Log.TratarErro(excecao);
                    return new JsonpResult(false, "Ocorreu uma falha ao reprovar os dados da NF-e.");
                }
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> RejeitarGestaoDadosColetaDadosTransporte()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync();

                int codigo = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta servicoGestaoDadosColeta = new Servicos.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColeta(unitOfWork, Auditado);

                await servicoGestaoDadosColeta.ReprovarDadosTransporteAsync(codigo);

                await unitOfWork.CommitChangesAsync();

                return new JsonpResult("Dados de transporte reprovados com sucesso.");
            }
            catch (BaseException excecao)
            {
                await unitOfWork.RollbackAsync();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao reprovar os dados de transporte.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"GestaoDadosColeta.{grid.extensaoCSV}");

                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoExportar);
            }
        }

        public async Task<IActionResult> BuscarDadosNfePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDadosColeta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaNFe = repositorioGestaoDadosColetaNFe.BuscarPorCodigoGestaoDadosColeta(codigoGestaoDadosColeta);

                if (gestaoDadosColetaNFe == null)
                    return new JsonpResult(null);

                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta = gestaoDadosColetaNFe.GestaoDadosColeta;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = gestaoDadosColeta.CargaEntrega;

                var gestaoDadosColetaRetornar = new
                {
                    GestaoDadosColeta = new
                    {
                        Carga = cargaEntrega.Carga.CodigoCargaEmbarcador ?? string.Empty,
                        CodigoCarga = cargaEntrega.Carga.Codigo,
                        Endereco = $"{(cargaEntrega.Cliente?.CodigoIntegracao ?? string.Empty)} - {(cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty)}",
                        Cliente = cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty,
                        Pedidos = string.Join(",", cargaEntrega.Pedidos.Select(pedido => pedido.CargaPedido.Pedido.NumeroPedidoEmbarcador)),
                        Transportador = cargaEntrega.Carga.Empresa.Descricao ?? string.Empty,
                        Origem = cargaEntrega.Carga.DadosSumarizados.Origens ?? string.Empty,
                        Destino = cargaEntrega.Carga.DadosSumarizados?.Destinos ?? string.Empty,
                    },
                    Geolocalizacao = new
                    {
                        PossuiGeolocalizacao = gestaoDadosColeta.Latitude.HasValue && gestaoDadosColeta.Longitude.HasValue,
                        gestaoDadosColeta.Latitude,
                        gestaoDadosColeta.Longitude,
                        PossuiLocalizacaoCliente = (!string.IsNullOrEmpty(cargaEntrega.Cliente?.Latitude)) && !string.IsNullOrEmpty(cargaEntrega.Cliente?.Longitude) && (cargaEntrega.Cliente?.RaioEmMetros.HasValue ?? false),
                        LocalizacaoCliente = new { Latitude = cargaEntrega.Cliente?.Latitude, Longitude = cargaEntrega.Cliente?.Longitude, RaioCliente = cargaEntrega.Cliente?.RaioEmMetros }
                    },
                    DadosAprovacaoNFe = new
                    {
                        gestaoDadosColetaNFe.Codigo,
                        gestaoDadosColeta.Situacao,
                        gestaoDadosColetaNFe.GuidArquivo,
                        gestaoDadosColetaNFe.Chave,
                        Numero = (gestaoDadosColetaNFe.Numero > 0) ? gestaoDadosColetaNFe.Numero.ToString() : string.Empty,
                        gestaoDadosColetaNFe.Serie,
                        FotoNotaFiscal = ObterFotoNFeBase64(gestaoDadosColetaNFe.GuidArquivo, gestaoDadosColetaNFe.OrigemFoto, unitOfWork),
                        DataEmissao = gestaoDadosColetaNFe.DataEmissao?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                        Peso = (gestaoDadosColetaNFe.Peso > 0m) ? gestaoDadosColetaNFe.Peso.ToString("n2") : string.Empty,
                        Volumes = (gestaoDadosColetaNFe.Volumes > 0) ? gestaoDadosColetaNFe.Volumes.ToString("n2") : string.Empty,
                        Valor = (gestaoDadosColetaNFe.Valor > 0m) ? gestaoDadosColetaNFe.Valor.ToString("n2") : string.Empty,
                        Emitente = new { Codigo = gestaoDadosColetaNFe.Emitente?.Codigo ?? 0, Descricao = gestaoDadosColetaNFe.Emitente?.NomeCNPJ ?? "" },
                        Destinatario = new { Codigo = gestaoDadosColetaNFe.Destinatario?.Codigo ?? 0, Descricao = gestaoDadosColetaNFe.Destinatario?.NomeCNPJ ?? "" },
                        gestaoDadosColeta.ErroRetornoConfirmacaoColeta,
                        DataRetornoConfirmacaoColeta = gestaoDadosColeta.DataRetornoConfirmacaoColeta?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                        gestaoDadosColeta.OperacaoRetornoConfirmacaoColeta,
                        gestaoDadosColeta.IdExternoRetornoConfirmacaoColeta,
                    }
                };

                return new JsonpResult(gestaoDadosColetaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> BuscarDadosTransportePorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDadosColeta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte repositorioGestaoDadosTransporte = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosTransporte gestaoDadosColetaTransporte = repositorioGestaoDadosTransporte.BuscarPorCodigoGestaoDadosColeta(codigoGestaoDadosColeta);

                if (gestaoDadosColetaTransporte == null)
                    return new JsonpResult(null);

                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta gestaoDadosColeta = gestaoDadosColetaTransporte.GestaoDadosColeta;
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = gestaoDadosColeta.CargaEntrega;
                Dominio.Entidades.Embarcador.Cargas.Carga carga = gestaoDadosColeta.CargaEntrega.Carga;
                Dominio.Entidades.Veiculo reboque = gestaoDadosColetaTransporte.VeiculosVinculados?.ElementAtOrDefault(0);
                Dominio.Entidades.Veiculo segundoReboque = gestaoDadosColetaTransporte.VeiculosVinculados?.ElementAtOrDefault(1);
                bool exigirConfirmacaoTracao = (carga.TipoOperacao?.ExigePlacaTracao ?? false);

                var gestaoDadosColetaRetornar = new
                {
                    GestaoDadosColeta = new
                    {
                        Carga = carga.CodigoCargaEmbarcador ?? string.Empty,
                        Endereco = $"{(cargaEntrega.Cliente?.CodigoIntegracao ?? string.Empty)} - {(cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty)}",
                        Cliente = cargaEntrega.Cliente?.Nome ?? cargaEntrega.Localidade?.Descricao ?? string.Empty,
                        Pedidos = string.Join(",", cargaEntrega.Pedidos.Select(pedido => pedido.CargaPedido.Pedido.NumeroPedidoEmbarcador)),
                        Transportador = gestaoDadosColeta.Empresa.Descricao ?? string.Empty,
                        Origem = carga.DadosSumarizados?.Origens ?? string.Empty,
                        Destino = carga.DadosSumarizados?.Destinos ?? string.Empty,
                    },
                    DadosTransporte = new
                    {
                        gestaoDadosColetaTransporte.Codigo,
                        gestaoDadosColeta.Situacao,
                        ExigirConfirmacaoTracao = exigirConfirmacaoTracao,
                        NumeroReboques = carga.ModeloVeicularCarga?.NumeroReboques ?? 0,
                        TipoVeiculo = exigirConfirmacaoTracao ? "0" : "",
                        Veiculo = new { Codigo = gestaoDadosColetaTransporte.Veiculo?.Codigo ?? 0, Descricao = exigirConfirmacaoTracao ? gestaoDadosColetaTransporte.Veiculo?.Descricao ?? "" : Servicos.Embarcador.Veiculo.Veiculo.ObterDescricaoPlacas(gestaoDadosColetaTransporte.Veiculo) },
                        Reboque = new { Codigo = reboque?.Codigo ?? 0, Descricao = reboque?.Descricao ?? "" },
                        SegundoReboque = new { Codigo = segundoReboque?.Codigo ?? 0, Descricao = segundoReboque?.Descricao ?? "" },
                        Transportador = new { Codigo = gestaoDadosColeta.Empresa?.Codigo ?? 0, Descricao = gestaoDadosColeta.Empresa.Descricao ?? "" },
                        Motoristas = (
                            from motorista in gestaoDadosColetaTransporte.Motoristas
                            select new
                            {
                                motorista.Codigo,
                                motorista.Descricao,
                            }
                        ).ToList(),
                    }
                };

                return new JsonpResult(gestaoDadosColetaRetornar);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFoto()
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhuma foto selecionada para adicionar.");

                Servicos.DTO.CustomFile foto = arquivos[0];
                string extensaoArquivo = Path.GetExtension(foto.FileName).ToLower();
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "NotaFiscal" });
                string guidArquivo = Guid.NewGuid().ToString();
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}");

                foto.SaveAs(nomeArquivo);

                return new JsonpResult(new
                {
                    FotoNotaFiscal = ObterFotoNFeBase64(guidArquivo, OrigemFotoDadosNFEGestaoDadosColeta.GestaoDadosColeta, unitOfWork),
                    GuidArquivo = guidArquivo
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a imagem da NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string guidArquivo = Request.GetStringParam("GuidArquivo");
                string caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "NotaFiscal" });
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{guidArquivo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, "Não foi possível encontrar a imagem da NF-e");

                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao remover a imagem da NF-e");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        [AcceptVerbs("POST", "GET")]
        public async Task<IActionResult> DownloadFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoGestaoDadosColeta = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe repositorioGestaoDadosColetaNFe = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColetaDadosNFe gestaoDadosColetaNFe = repositorioGestaoDadosColetaNFe.BuscarPorCodigo(codigoGestaoDadosColeta);

                if (gestaoDadosColetaNFe == null)
                    throw new ControllerException("Não foi possível encontrar os dados da NF-e.");

                string nomeCompletoArquivo = ObterNomeCompletoArquivoFotoNFe(gestaoDadosColetaNFe.GuidArquivo, gestaoDadosColetaNFe.OrigemFoto, unitOfWork);

                if (string.IsNullOrWhiteSpace(nomeCompletoArquivo))
                    return new JsonpResult(false, true, "Não foi possível encontrar a imagem da NF-e");

                byte[] imagemBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
                string nomeArquivo = Path.GetFileName(nomeCompletoArquivo);
                string extensaoArquivo = Path.GetExtension(nomeArquivo);

                return Arquivo(imagemBinario, $"application/{extensaoArquivo}", nomeArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoRealizarDownload);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAprovacao ObterDadosNFeAprovacaoSalvar()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.GestaoDadosColetaDadosNFeAprovacao()
            {
                Chave = Request.GetStringParam("Chave"),
                Numero = Request.GetIntParam("Numero"),
                Serie = Request.GetStringParam("Serie"),
                DataEmissao = Request.GetDateTimeParam("DataEmissao"),
                Peso = Request.GetDecimalParam("Peso"),
                Volumes = Request.GetIntParam("Volumes"),
                Valor = Request.GetDecimalParam("Valor"),
                CpfCnpjEmitente = Request.GetDoubleParam("Emitente"),
                CpfCnpjDestinatario = Request.GetDoubleParam("Destinatario")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Tipo", false);
                grid.AdicionarCabecalho("Carga", "CodigoCargaEmbarcador", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Fornecedor", "Cliente", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Data de Criação da Carga", "DataCriacaoCargaFormatada", 7, Models.Grid.Align.left, false, true, true);
                grid.AdicionarCabecalho("Tipo", "TipoDescricao", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "SituacaoAprovacao", 7, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data da Aprovação", "DataAprovacaoFormatada", 7, Models.Grid.Align.center, true, true, true);
                grid.AdicionarCabecalho("Aprovador", "UsuarioAprovacao", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Filial", "Filial", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Origem", "OrigemCarga", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Destino", "DestinoCarga", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Data Criação", "DataInicial", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Criado Por", "OrigemDescricao", 7, Models.Grid.Align.left, true, true, true);
                grid.AdicionarCabecalho("Duração", "Duracao", 7, Models.Grid.Align.left, false, true, true);

                Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenarOuAgrupar);
                Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta repositorioGestaoDadosColeta = new Repositorio.Embarcador.Cargas.GestaoDadosColeta.GestaoDadosColeta(unitOfWork);
                int totalRegistros = repositorioGestaoDadosColeta.ContarConsulta(filtrosPesquisa, parametrosConsulta);

                if (totalRegistros == 0)
                {
                    grid.AdicionaRows(new List<object> { });
                    return grid;
                }

                IList<Dominio.Relatorios.Embarcador.DataSource.Cargas.GestaoDadosColeta.GestaoDadosColeta> listaGestaoDadosColeta = repositorioGestaoDadosColeta.Consultar(filtrosPesquisa, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaGestaoDadosColeta);

                return grid;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.GestaoDadosColeta.FiltroPesquisaGestaoDadosColeta()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                DataInicialCriacaoCarga = Request.GetDateTimeParam("DataInicial"),
                DataFinalCriacaoCarga = Request.GetDateTimeParam("DataFinal"),
                SituacaoGestaoDadosColeta = Request.GetNullableEnumParam<SituacaoGestaoDadosColeta>("Situacao"),
                CodigoFilialEmbarcador = Request.GetIntParam("Filial"),
                CodigoCliente = Request.GetDoubleParam("Cliente"),
                OrigemCarga = Request.GetIntParam("OrigemCarga"),
                DestinoCarga = Request.GetIntParam("DestinoCarga"),
                RetornoConfirmacao = Request.GetNullableEnumParam<SituacaoGestaoDadosColetaRetornoConfirmacao>("RetornoConfirmacao")
            };

            if (TipoServicoMultisoftware == TipoServicoMultisoftware.MultiCTe)
                filtrosPesquisa.CodigoTransportador = Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private string ObterFotoNFeBase64(string guidArquivo, OrigemFotoDadosNFEGestaoDadosColeta origemFoto, Repositorio.UnitOfWork unitOfWork)
        {
            string nomeCompletoArquivo = ObterNomeCompletoArquivoFotoNFe(guidArquivo, origemFoto, unitOfWork);

            if (string.IsNullOrWhiteSpace(nomeCompletoArquivo))
                return string.Empty;

            byte[] imagemBinario = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeCompletoArquivo);
            string imagemBase64 = Convert.ToBase64String(imagemBinario);

            return imagemBase64;
        }

        private string ObterNomeCompletoArquivoFotoNFe(string guidArquivo, OrigemFotoDadosNFEGestaoDadosColeta origemFoto, Repositorio.UnitOfWork unitOfWork)
        {
            string caminho = string.Empty;

            if (origemFoto == OrigemFotoDadosNFEGestaoDadosColeta.GestaoDadosColeta)
                caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "NotaFiscal" });
            else if (origemFoto == OrigemFotoDadosNFEGestaoDadosColeta.ControleDeEntrega)
                caminho = Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "EntregaPedido" });

            return Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{guidArquivo}.*").FirstOrDefault();
        }

        private string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.EndsWith("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            if (propriedadeOrdenarOuAgrupar == "Cliente")
                return "NomeCliente";

            if (propriedadeOrdenarOuAgrupar == "TipoDescricao")
                return "Tipo";

            if (propriedadeOrdenarOuAgrupar == "SituacaoAprovacao")
                return "Situacao";

            if (propriedadeOrdenarOuAgrupar == "OrigemDescricao")
                return "Origem";

            if (propriedadeOrdenarOuAgrupar == "UsuarioAprovacao")
                return "NomeUsuario";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion Métodos Privados
    }
}
