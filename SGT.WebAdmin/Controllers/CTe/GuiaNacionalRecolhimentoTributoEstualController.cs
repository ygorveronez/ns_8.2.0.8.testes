using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.CTe
{
    [CustomAuthorize("CTe/GuiaNacionalRecolhimentoTributoEstual", "Guias/GuiasRecolhimento")]
    public class GuiaNacionalRecolhimentoTributoEstualController : BaseController
    {
        #region Construtores

        public GuiaNacionalRecolhimentoTributoEstualController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Metodos Publicos 

        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoGNREIntegracao = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guiaNacionalRecolhimentoTributoEstadual = repositorioGNRE.BuscarPorCodigo(codigoGNREIntegracao, false);

                if (guiaNacionalRecolhimentoTributoEstadual == null)
                    return new JsonpResult($"Não há registro existente para a guia com o codigo {codigoGNREIntegracao}");

                guiaNacionalRecolhimentoTributoEstadual.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                guiaNacionalRecolhimentoTributoEstadual.Situacao = SituacaoGuia.AguardandoEnvio;

                if (guiaNacionalRecolhimentoTributoEstadual.NumeroTentativas == 3)
                    guiaNacionalRecolhimentoTributoEstadual.NumeroTentativas = 1;

                repositorioGNRE.Atualizar(guiaNacionalRecolhimentoTributoEstadual);

                return new JsonpResult(true, "Reenvio Feito com sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        public async Task<IActionResult> ReenviarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoCarga = Request.GetIntParam("Carga");

                if (codigoCarga == 0)
                    return new JsonpResult($"Precisa informar uma carga");

                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasComProblemas = repositorioGNRE.BuscarPorCarga(codigoCarga);
                foreach (Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia in guiasComProblemas)
                {
                    guia.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    guia.Situacao = SituacaoGuia.AguardandoEnvio;

                    if (guia.NumeroTentativas == 3)
                        guia.NumeroTentativas = 1;

                    repositorioGNRE.Atualizar(guia);
                }

                return new JsonpResult(true, "Integrações reenviadas com sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        public async Task<IActionResult> ReenviarTodosPendentes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasComProblemas = repositorioGNRE.BuscarPendentes();
                foreach (Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia in guiasComProblemas)
                {
                    guia.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                    guia.Situacao = SituacaoGuia.AguardandoEnvio;

                    if (guia.NumeroTentativas == 3)
                        guia.NumeroTentativas = 1;

                    repositorioGNRE.Atualizar(guia);
                }

                return new JsonpResult(true, "Integrações reenviadas com sucesso");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        public async Task<IActionResult> Download()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoGNRE = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);


                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoGNRE = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guiaNacionalRecolhimentoTributoEstadual = repositorioGNRE.BuscarPorCodigo(codigoGNRE, false);

                if (guiaNacionalRecolhimentoTributoEstadual == null)
                    return new JsonpResult($"Não há registro existente para a guia com o codigo {codigoGNRE}");

                guiaNacionalRecolhimentoTributoEstadual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuia.Cancelada;
                repositorioGNRE.Atualizar(guiaNacionalRecolhimentoTributoEstadual);

                return new JsonpResult(true, "Guia Cancelada com sucesso!");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar processar a requisição");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                int numeroCte = Request.GetIntParam("NroCte");
                int carga = Request.GetIntParam("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nro da guia", "NumeroGuia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nro CT-e", "NumeroCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasNacionaisTributosEstaduais = repositorioGNRE.BuscarPorCarga(carga, numeroCte);

                List<dynamic> retorno = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia in guiasNacionaisTributosEstaduais)
                    retorno.Add(new
                    {
                        Codigo = guia.Codigo,
                        DataEmissao = guia.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                        NumeroGuia = guia.NroGuia,
                        NumeroCte = guia?.Cte?.Numero ?? 0,
                        Valor = guia.Valor,
                        Status = guia.Situacao.ObterDescricao(),
                        SituacaoComprovante = guia?.SituacaoDigitalizacaoComprovante ?? 0,
                        SituacaoGuia = guia?.SituacaoDigitalizacaoGuiaRecolhimento ?? 0

                    });

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(retorno.Count);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ConsultarGuias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repositorioConfigGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unitOfWork);
                var configGeral = repositorioConfigGeral.BuscarConfiguracaoPadrao();

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeralTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                var configGeralTMS = repConfigGeralTMS.BuscarConfiguracaoPadrao();

                bool visualizarGNRESemValidacaoDocumentos = configGeral.VisualizarGNRESemValidacaoDocumentos;

                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("GuiaAnexada", false);
                grid.AdicionarCabecalho("ComprovanteAnexado", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("GuiaValidadaManualmente", false);
                grid.AdicionarCabecalho("ComprovanteValidadoManualmente", false);
                grid.AdicionarCabecalho("ValidouTodasInformacoesManualmente", false);
                if (!visualizarGNRESemValidacaoDocumentos)
                    grid.AdicionarCabecalho("Nro da guia", "NumeroGuia", 15, Models.Grid.Align.center);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.center, true, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                grid.AdicionarCabecalho("Veiculo", "Veiculo", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave CTe", "ChaveCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nro CT-e", "NumeroCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série CTe", "SerieCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true);
                if (!visualizarGNRESemValidacaoDocumentos)
                {
                    grid.AdicionarCabecalho("OCR Guia", "OCRGuiaValidado", 15, Models.Grid.Align.left);
                    grid.AdicionarCabecalho("OCR Comprovante", "OCRComprovanteValidado", 15, Models.Grid.Align.left);
                    grid.AdicionarCabecalho("Guia x Comprovante x Sistema", "TodosDadosValidados", 20, Models.Grid.Align.left);
                }

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento filtroPesquisa = ObterFiltrosPesquisa(unitOfWork, configGeralTMS);
                List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasNacionaisTributosEstaduais = repositorioGNRE.Consultar(filtroPesquisa, parametrosConsulta);

                List<dynamic> retorno = new List<dynamic>();
                /* TO-DO
                * QUANDO ESTEJA FEITO A VALIDAÇÃO DA GUIA VIA OCR DEVE VALIDAR
                * COM OS CAMPOS DO COMPROVANTE CASO NÃO SEJAM IGUAIS VALIDAR SE FOI
                *  VALIDADO MANUALMENTE
                */

                foreach (Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia in guiasNacionaisTributosEstaduais)
                    retorno.Add(new
                    {
                        Codigo = guia.Codigo,
                        NumeroGuia = guia.NroGuia,
                        Valor = guia.Valor,
                        DataEmissao = guia.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Carga = guia.Carga?.CodigoCargaEmbarcador ?? "",
                        Transportador = guia.Cte?.Empresa?.NomeCNPJ ?? "",
                        Veiculo = guia.Cte?.Veiculos?.FirstOrDefault().Placa_Formatada ?? "",
                        Motorista = guia.Cte?.Motoristas.FirstOrDefault().NomeMotorista ?? "",
                        ChaveCte = guia.Cte?.ChaveAcesso ?? "",
                        NumeroCte = guia.Cte?.Numero.ToString() ?? "",
                        SerieCte = guia.Cte?.Serie?.Numero.ToString() ?? "",
                        Status = guia.Situacao.ObterDescricao(),
                        GuiaAnexada = (guia?.SituacaoDigitalizacaoGuiaRecolhimento ?? SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado) == SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado,
                        ComprovanteAnexado = (guia?.SituacaoDigitalizacaoComprovante ?? SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado) == SituacaoDigitalizacaoGuiaComprovante.Digitalizado,
                        OCRGuiaValidado = guia.GuiaValidadaManualmente.HasValue && guia.GuiaValidadaManualmente.Value ? SituacaoLeituraOCR.Validado.ObterDescricao() : guia.SituacaoLeituraOCRGuia.ObterDescricao(),
                        OCRComprovanteValidado = guia.ComprovanteValidadoManualmente.HasValue && guia.ComprovanteValidadoManualmente.Value ? SituacaoLeituraOCR.Validado.ObterDescricao() : guia?.SituacaoLeituraOCRComprovante.ObterDescricao(),
                        TodosDadosValidados = guia.TodosOsDadosValidadosOCR,
                        guia.GuiaValidadaManualmente,
                        guia.ComprovanteValidadoManualmente,
                        guia.ValidouTodasInformacoesManualmente,
                        Observacao = guia.Observacao
                    });

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(retorno.Count);

                return new JsonpResult(grid);
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarGuias()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaCTe repositorioCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigGeralTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                var configGeralTMS = repConfigGeralTMS.BuscarConfiguracaoPadrao();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Nro da guia", "NumeroGuia", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor", "Valor", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Transportador", "Transportador", 15, Models.Grid.Align.center, true, TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                grid.AdicionarCabecalho("Veiculo", "Veiculo", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Chave CTe", "ChaveCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Nro CT-e", "NumeroCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Série CTe", "SerieCte", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "Status", 15, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento filtroPesquisa = ObterFiltrosPesquisa(unitOfWork, configGeralTMS);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual> guiasNacionaisTributosEstaduais = repositorioGNRE.Consultar(filtroPesquisa, parametrosConsulta);

                List<dynamic> retorno = new List<dynamic>();

                foreach (Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia in guiasNacionaisTributosEstaduais)
                    retorno.Add(new
                    {
                        Codigo = guia.Codigo,
                        NumeroGuia = guia.NroGuia,
                        Valor = guia.Valor,
                        DataEmissao = guia.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Carga = guia.Carga?.Codigo.ToString() ?? "",
                        Transportador = guia.Cte?.Empresa?.NomeCNPJ ?? "",
                        Veiculo = guia.Cte?.Veiculos?.FirstOrDefault().Placa_Formatada ?? "",
                        Motorista = guia.Cte?.Motoristas.FirstOrDefault().NomeMotorista ?? "",
                        ChaveCte = guia.Cte?.ChaveAcesso ?? "",
                        NumeroCte = guia.Cte?.Numero.ToString() ?? "",
                        SerieCte = guia.Cte?.Serie?.Numero.ToString() ?? "",
                        Status = guia.Situacao.ObterDescricao(),
                    });

                grid.AdicionaRows(retorno);
                grid.setarQuantidadeTotal(repositorioGNRE.ContarConsulta(filtroPesquisa));

                byte[] bArquivo = grid.GerarExcel();
                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao exportar.");

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

        public async Task<IActionResult> BuscarHistorico()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositoriorecolhimentoTributoEtadualIntegracao = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                int codigoGuia = Request.GetIntParam("GuiaRecolhimento");

                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual integracao = repositoriorecolhimentoTributoEtadualIntegracao.BuscarPorCodigo(codigoGuia, false);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Mensagem", "Mensagem", 15, Models.Grid.Align.center, true);

                var lista = (from obj in integracao.ArquivosTransacao
                             select new
                             {
                                 obj.Data,
                                 obj.Descricao,
                                 obj.Mensagem
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count());

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> LiberarComProblemaGNRE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga");
            if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_PermiteAvancarCargaComRejeitacaoGNRE) && !Usuario.UsuarioAdministrador)
                return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

            try
            {
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(false, true, "Você não possui permissão para executar essa ação.");

                if (carga == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                unitOfWork.Start();
                if (carga.PossuiPendencia)
                {
                    carga.LiberadoComProblemaGNRE = true;
                    carga.PossuiPendencia = false;
                    carga.MotivoPendencia = "";

                    repCarga.Atualizar(carga);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, carga, null, "Avançou Etapa com GNRE Rejeitados.", unitOfWork);
                }
                unitOfWork.CommitChanges();

                svcHubCarga.InformarCargaAtualizada(codigoCarga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar etapa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DigitalizarGuia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(codigo, true);

                if ((guia?.SituacaoDigitalizacaoGuiaRecolhimento ?? SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado) == SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado)
                {
                    unitOfWork.Start();

                    guia.SituacaoDigitalizacaoGuiaRecolhimento = SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado;

                    repGuia.Atualizar(guia);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DigitalizarComprovante()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(codigo, true);

                if ((guia?.SituacaoDigitalizacaoComprovante ?? SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado) == SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado)
                {
                    unitOfWork.Start();

                    guia.SituacaoDigitalizacaoComprovante = SituacaoDigitalizacaoGuiaComprovante.Digitalizado;

                    repGuia.Atualizar(guia);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                TipoAnexoGuiaRecolhimento tipoAnexo = Request.GetEnumParam<TipoAnexoGuiaRecolhimento>("TipoAnexo");

                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = repGuiaAnexo.BuscarAnexoPorGuia(codigo, tipoAnexo);

                if (guiaAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Guias.LeitorOCR servcoLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                var retorno = new
                {
                    guiaAnexo.Codigo,
                    Situacao = guiaAnexo.EntidadeAnexo?.Situacao.ObterDescricao() ?? string.Empty,
                    Extensao = guiaAnexo.ExtensaoArquivo,
                    Guia = guiaAnexo.EntidadeAnexo != null ? new { guiaAnexo.EntidadeAnexo.Codigo, guiaAnexo.EntidadeAnexo.Descricao } : null,
                    Imagem = guiaAnexo.ExtensaoArquivo != ExtensaoArquivo.PDF.ToString().ToLower() ? servcoLeitorOCR.ObterBase64DaImagem(guiaAnexo, unitOfWork) : null
                };
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Renderizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            MemoryStream stream = new MemoryStream();
            string nome = "Guia.pdf";

            try
            {

                int codigo = Request.GetIntParam("Codigo");
                int codigoGuia = (codigo == 0) ? Request.GetIntParam("Guia") : 0;
                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioCanhoto = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = repGuiaAnexo.BuscarPorCodigo(codigo, false);

                if (guiaAnexo != null)
                {
                    Servicos.Embarcador.Guias.LeitorOCR servicoLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);
                    nome = guiaAnexo.NomeArquivo;
                    stream = servicoLeitorOCR.ObterStremingPDF(guiaAnexo, unitOfWork);
                }

                if ((stream == null || stream.Length <= 0) && codigoGuia > 0)
                {
                    Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repositorioCanhoto.BuscarPorCodigo(codigoGuia, false);

                    if (guia != null)
                    {
                        Servicos.Embarcador.Guias.LeitorOCR servicoCanhoto = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);
                        nome = guiaAnexo.NomeArquivo;
                        stream = servicoCanhoto.ObterStremingPDFGuia(guiaAnexo, unitOfWork);
                    }
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return File(stream, "application/pdf", nome);
        }

        public async Task<IActionResult> UploadImagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                TipoAnexoGuiaRecolhimento tipoAnexo = Request.GetEnumParam<TipoAnexoGuiaRecolhimento>("TipoAnexo");

                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(codigo, auditavel: false);

                Servicos.Embarcador.Guias.LeitorOCR svcLeitor = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                if (guia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!PermitirAdicionarImagem(guia, tipoAnexo))
                    return new JsonpResult(false, true, "Situação não permite adicionar arquivos.");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
                string[] descricoes = Request.TryGetArrayParam<string>("Descricao");

                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repositorioAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                string caminho = svcLeitor.ObterCaminhoArquivosVinculados(unitOfWork);

                for (int i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile arquivo = arquivos[i];
                    string extensaoArquivo = System.IO.Path.GetExtension(arquivo.FileName).ToLower();
                    string guidArquivo = Guid.NewGuid().ToString().Replace("_", "");
                    string nomeArquivo = arquivo.FileName;
                    double tamanhoPermitido = 1000000;

                    if (arquivo.Length > tamanhoPermitido)
                        throw new ControllerException("Tamanho do arquivo é maior que o permitido (1MB)");

                    Servicos.Embarcador.Guias.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(new Servicos.Global.ServicoOCR(unitOfWork), unitOfWork);

                    arquivo.SaveAs(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{guidArquivo}{extensaoArquivo}"));

                    Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo anexo = new Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo()
                    {
                        EntidadeAnexo = guia,
                        Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                        GuidArquivo = guidArquivo,
                        NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(arquivo.FileName)))
                    };

                    anexo.TipoAnexo = tipoAnexo;
                    anexo.DataAnexo = System.DateTime.Now;

                    if (tipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                        guia.SituacaoDigitalizacaoComprovante = SituacaoDigitalizacaoGuiaComprovante.Digitalizado;
                    else if (tipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                        guia.SituacaoDigitalizacaoGuiaRecolhimento = SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado;

                    repGuia.Atualizar(guia);
                    repositorioAnexo.Inserir(anexo, Auditado);


                    if (tipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                        srvLeitorOCR.ProcessarTextoComprovante(Utilidades.File.ReadToEnd(arquivo.InputStream), guia);
                    else if (tipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                        srvLeitorOCR.ProcessarTextoGuia(Utilidades.File.ReadToEnd(arquivo.InputStream), guia);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, guia, null, $"Adicionou o arquivo {nomeArquivo}.", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar o(s) arquivo(s).");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Descartar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia instancia
                unitOfWork.Start();

                // Repositorios
                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);

                // Busca Anexo
                int codigo = Request.GetIntParam("Codigo");
                TipoAnexoGuiaRecolhimento tipoAnexo = Request.GetEnumParam<TipoAnexoGuiaRecolhimento>("TipoAnexo");

                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(codigo, false);

                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo anexo = repGuiaAnexo.BuscarAnexoPorGuia(codigo, tipoAnexo);

                Servicos.Embarcador.Guias.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                // Valida
                if (anexo == null)
                    return new JsonpResult(false, false, "Erro ao buscar os dados.");

                // Monta apontamento ao arquivo
                string caminho = anexo.EntidadeAnexo?.Codigo > 0 ? srvLeitorOCR.ObterCaminhoArquivosVinculados(unitOfWork) : srvLeitorOCR.ObterCaminhoArquivosNaoVinculados(unitOfWork);
                var extensaoArquivo = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extensaoArquivo);

                // Verifica se arquivo exise
                if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                // Remove do banco
                if (guia != null)
                {
                    if (anexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                        guia.SituacaoDigitalizacaoGuiaRecolhimento = SituacaoDigitalizacaoGuiaRecolhimento.NaoDigitalizado;
                    if (anexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                        guia.SituacaoDigitalizacaoComprovante = SituacaoDigitalizacaoGuiaComprovante.NaoDigitalizado;

                    if (anexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Guia)
                        guia.SituacaoLeituraOCRGuia = SituacaoLeituraOCR.NaoValidado;

                    if (anexo.TipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante)
                        guia.SituacaoLeituraOCRComprovante = SituacaoLeituraOCR.NaoValidado;

                    repGuia.Atualizar(guia);
                }

                repGuiaAnexo.Deletar(anexo);

                // Commita
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao fazer download dos dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidacaoManual()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoGNRE = Request.GetIntParam("Codigo");
                bool? validadoOCRGuiaManual = Request.GetNullableBoolParam("OCRGuiaValidacaoManual");
                bool? validadoOCRComprovanteValidacaoManual = Request.GetNullableBoolParam("OCRComprovanteValidacaoManual");
                bool? ValidadaTodasInformacoesManualmente = Request.GetNullableBoolParam("ValidadaTodasInformacoesManualmente");
                string observacao = Request.GetStringParam("Observacao");

                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioGNRE = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual existeGNRE = repositorioGNRE.BuscarPorCodigo(codigoGNRE, false);
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Guias/GuiasRecolhimento");
                if (existeGNRE == null)
                    return new JsonpResult(false, "Não encontrado registro de GNRE");

                bool possuiPermisao = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Guias_PermiteReverterAprovacaoOCRGuias);
                if ((existeGNRE.ValidouTodasInformacoesManualmente.HasValue && existeGNRE.ValidouTodasInformacoesManualmente.Value) && (ValidadaTodasInformacoesManualmente.HasValue && !ValidadaTodasInformacoesManualmente.Value))
                    if (!possuiPermisao && !Usuario.UsuarioAdministrador)
                        return new JsonpResult(false, "Você não possui permisão para realizar esta ação!");

                if ((existeGNRE.GuiaValidadaManualmente.HasValue && existeGNRE.GuiaValidadaManualmente.Value) && (validadoOCRGuiaManual.HasValue && !validadoOCRGuiaManual.Value))
                    if (!possuiPermisao && !Usuario.UsuarioAdministrador)
                        return new JsonpResult(false, "Você não possui permisão para realizar esta ação!");

                if ((existeGNRE.ComprovanteValidadoManualmente.HasValue && existeGNRE.ComprovanteValidadoManualmente.Value) && (validadoOCRComprovanteValidacaoManual.HasValue && !validadoOCRComprovanteValidacaoManual.Value))
                    if (!possuiPermisao && !Usuario.UsuarioAdministrador)
                        return new JsonpResult(false, "Você não possui permisão para realizar esta ação!");

                existeGNRE.ValidouTodasInformacoesManualmente = ValidadaTodasInformacoesManualmente;
                existeGNRE.ComprovanteValidadoManualmente = validadoOCRComprovanteValidacaoManual;
                existeGNRE.GuiaValidadaManualmente = validadoOCRGuiaManual;
                existeGNRE.SituacaoLeituraOCRGuia = validadoOCRGuiaManual.HasValue && validadoOCRGuiaManual.Value ? SituacaoLeituraOCR.Validado : SituacaoLeituraOCR.Rejeitado;
                existeGNRE.SituacaoLeituraOCRComprovante = validadoOCRComprovanteValidacaoManual.HasValue && validadoOCRComprovanteValidacaoManual.Value ? SituacaoLeituraOCR.Validado : SituacaoLeituraOCR.Rejeitado;
                existeGNRE.SituacaoTodasAsInformacoes = ValidadaTodasInformacoesManualmente.HasValue && ValidadaTodasInformacoesManualmente.Value ? SituacaoLeituraOCR.Validado : SituacaoLeituraOCR.Rejeitado;

                existeGNRE.Observacao = Utilidades.String.Left(observacao, 250);

                repositorioGNRE.Atualizar(existeGNRE);

                return new JsonpResult(true, true, "GNRE validado manualmente com sucesso!");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao validar os dos dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Metodos Privados

        private Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configGeralTMS)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            int codmotorista = Request.GetIntParam("Motorista");

            Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.GuiaRecolhimento.FiltroPesquisaGuiaRecolhimento()
            {
                NumeroCte = Request.GetIntParam("NroCte"),
                NumeroCarga = Request.GetStringParam("Carga"),
                ChaveCte = Request.GetStringParam("ChaveCte"),
                SerieCte = Request.GetIntParam("SerieCte"),
                Codigotransportador = Request.GetIntParam("Transportador"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                Status = Request.GetListEnumParam<SituacaoGuia>("Status"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal")
            };

            if (codmotorista > 0)
            {
                Dominio.Entidades.Usuario motorista = repUsuario.BuscarPorCodigo(codmotorista);
                filtrosPesquisa.CPFMotorista = motorista.CPF;
            }

            //portal do transportador, deve apresentar apenas os registros do transportador logado
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                filtrosPesquisa.Codigotransportador = this.Usuario.Empresa.Codigo;
            }

            if (configGeralTMS.FiltrarCargasPorParteDoNumero)
            {
                filtrosPesquisa.FiltrarCargasPorParteDoNumero = true;
            }

            return filtrosPesquisa;
        }

        private bool PermitirAdicionarImagem(Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia, TipoAnexoGuiaRecolhimento tipoAnexo)
        {
            if (tipoAnexo == TipoAnexoGuiaRecolhimento.Guia && guia.SituacaoDigitalizacaoGuiaRecolhimento == SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado)
                return false;

            if (tipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante && guia.SituacaoDigitalizacaoComprovante == SituacaoDigitalizacaoGuiaComprovante.Digitalizado)
                return false;

            return true;

        }

        #endregion
    }
}
