using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Servicos.DTO;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Guias
{
    [CustomAuthorize("Guias/VincularGuia")]
    public class VincularGuiaController : BaseController
    {
		#region Construtores

		public VincularGuiaController(Conexao conexao) : base(conexao) { }

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
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = repGuiaAnexo.BuscarPorCodigo(codigo, false);

                if (guiaAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Servicos.Embarcador.Guias.LeitorOCR servcoLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                var retorno = new
                {
                    guiaAnexo.Codigo,
                    Situacao = guiaAnexo.EntidadeAnexo?.Situacao.ObterDescricao() ?? string.Empty,
                    Extensao = guiaAnexo.ExtensaoArquivo,
                    Guia = guiaAnexo.EntidadeAnexo != null ? new { guiaAnexo.EntidadeAnexo.Codigo, guiaAnexo.EntidadeAnexo.Descricao } : null,
                    Imagem = guiaAnexo.ExtensaoArquivo != ExtensaoArquivo.PDF.ToString().ToLower() ? servcoLeitorOCR.ObterBase64DaImagem(guiaAnexo, unitOfWork) : null,
                    TipoAnexo = guiaAnexo.TipoAnexo ?? TipoAnexoGuiaRecolhimento.Guia
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
                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repositorioCanhoto = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = repGuiaAnexo.BuscarPorCodigo(codigo, false);
                Servicos.Embarcador.Guias.LeitorOCR servicoLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);


                if (guiaAnexo != null && guiaAnexo.EntidadeAnexo?.Codigo > 0)
                {
                    nome = guiaAnexo.NomeArquivo;
                    stream = servicoLeitorOCR.ObterStremingPDF(guiaAnexo, unitOfWork);
                }
                else
                {
                    nome = guiaAnexo.NomeArquivo;
                    stream = servicoLeitorOCR.ObterStreamingPDFProcessados(guiaAnexo);
                }

                if ((stream == null || stream.Length <= 0) && guiaAnexo.EntidadeAnexo?.Codigo > 0)
                {
                    Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repositorioCanhoto.BuscarPorCodigo(guiaAnexo.EntidadeAnexo.Codigo, false);

                    if (guia != null)
                    {
                        Servicos.Embarcador.Guias.LeitorOCR servicoCanhoto = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);
                        nome = guiaAnexo.NomeArquivo;
                        stream = servicoCanhoto.ObterStremingPDFGuia(guiaAnexo, unitOfWork);
                    }
                }

                if (stream == null)
                    return new JsonpResult(false, "Arquivo não localizado no sistema");

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

        public async Task<IActionResult> UploadImagens()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Guias.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                IEnumerable<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Imagem");
                List<string> erros = new List<string>();
                List<string> extensoesValidas = new List<string>() {
                    ".jpg", ".tif", ".pdf"
                };
                int adicionados = 0;

                if (!arquivos.Any())
                    return new JsonpResult(false, true, "Nenhum arquivo selecionado para envio.");

                for (var i = 0; i < arquivos.Count(); i++)
                {
                    Servicos.DTO.CustomFile file = arquivos.ElementAt(i);
                    var extensaoArquivo = System.IO.Path.GetExtension(file.FileName).ToLower();

                    if (!extensoesValidas.Contains(extensaoArquivo))
                    {
                        erros.Add("Extensão " + extensaoArquivo + " não permitida.");
                        continue;
                    }
                    try
                    {
                        string caminhoRaiz = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos;

                        srvLeitorOCR.AdicionarArquivoNaPastaProcessados(unitOfWork, file, caminhoRaiz);

                        adicionados++;
                    }
                    catch (Exception e)
                    {
                        erros.Add("Erro ao processar arquivo " + file.FileName + ".");
                        Servicos.Log.TratarErro(e);
                    }
                }

                return new JsonpResult(new
                {
                    Adicionados = adicionados,
                    Erros = erros
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo a ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Confirmar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual repGuia = new Repositorio.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEtadual(unitOfWork);
                Servicos.Embarcador.Guias.LeitorOCR srvLeitorOCR = new Servicos.Embarcador.Guias.LeitorOCR(unitOfWork);

                // Parametros
                int codigo = Request.GetIntParam("Codigo");
                int codigoGuia = Request.GetIntParam("Guia");
                TipoAnexoGuiaRecolhimento tipoAnexo = Request.GetEnumParam<TipoAnexoGuiaRecolhimento>("TipoAnexo");

                // Busca informacoes
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo guiaAnexo = repGuiaAnexo.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(codigoGuia, false);

                // Valida
                if (guiaAnexo == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (guia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (tipoAnexo == TipoAnexoGuiaRecolhimento.Guia && guia.SituacaoDigitalizacaoGuiaRecolhimento == SituacaoDigitalizacaoGuiaRecolhimento.Digitalizado)
                    return new JsonpResult(false, true, "Já exise um arquivo 'Guia' vinculado à essa Guia de Recolhimento.");

                if (tipoAnexo == TipoAnexoGuiaRecolhimento.Comprovante && guia.SituacaoDigitalizacaoComprovante == SituacaoDigitalizacaoGuiaComprovante.Digitalizado)
                    return new JsonpResult(false, true, "Já exise um arquivo 'Comprovante' vinculado à essa Guia de Recolhimento.");

                // Persiste dados
                unitOfWork.Start();

                if (tipoAnexo > 0)
                    guiaAnexo.TipoAnexo = tipoAnexo;

                string caminhoNaoVinculados = guiaAnexo.EntidadeAnexo == null ? srvLeitorOCR.ObterCaminhoArquivosNaoVinculados(unitOfWork) : srvLeitorOCR.ObterCaminhoArquivosVinculados(unitOfWork);
                string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminhoNaoVinculados, guiaAnexo.GuidArquivo);

                srvLeitorOCR.VincularAnexoGuia(guia, caminhoCompleto, guiaAnexo, unitOfWork);

                repGuia.Atualizar(guia, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar dados.");
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
                Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo anexo = repGuiaAnexo.BuscarPorCodigo(codigo, false);
                Dominio.Entidades.Embarcador.CTe.GuiaNacionalRecolhimentoTributoEstadual guia = repGuia.BuscarPorCodigo(anexo.EntidadeAnexo?.Codigo ?? 0, false);


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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome Arquivo", "NomeArquivo", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data", "Data", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação da Guia", "Situacao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tipo do Anexo", "TipoArquivoDescricao", 30, Models.Grid.Align.left, true);

                Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo repGuiaAnexo = new Repositorio.Embarcador.Guias.GuiaRecolhimentoAnexo(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repGuiaAnexo.ContarConsulta(filtrosPesquisa);

                List<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo> listaAnexo = totalRegistros > 0 ? repGuiaAnexo.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Guias.GuiaRecolhimentoAnexo>();

                var lista = (from obj in listaAnexo
                             select new
                             {
                                 obj.Codigo,
                                 CodigoGuia = obj.EntidadeAnexo?.Codigo ?? 0,
                                 NumeroDocumento = obj.EntidadeAnexo?.NroGuia ?? string.Empty,
                                 NomeArquivo = obj.NomeArquivo ?? string.Empty,
                                 Data = obj.DataAnexo?.ToString("d") ?? string.Empty,
                                 Situacao = obj.EntidadeAnexo?.Situacao.ObterDescricao() ?? string.Empty,
                                 TipoArquivoDescricao = obj.TipoAnexo?.ObterDescricao() ?? string.Empty,
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.VincularGuia.FiltroPesquisaVincularGuia()
            {
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                CodigoCarga = Request.GetIntParam("Carga"),
                Status = Request.GetListEnumParam<SituacaoGuia>("Situacao"),
                DataInicial = Request.GetDateTimeParam("DataInicio"),
                DataFinal = Request.GetDateTimeParam("DataFim"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}
