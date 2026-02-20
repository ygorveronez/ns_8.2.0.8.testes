using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;
using Utilidades.Extensions;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize("GestaoPatio/ControleVisita")]
    public class ControleVisitaController : BaseController
    {
        #region Construtores

        public ControleVisitaController(Conexao conexao) : base(conexao) { }

        #endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita = repControleVisita.BuscarPorCodigo(codigo);

                // Valida
                if (controleVisita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Formata retorno
                var retorno = new
                {
                    controleVisita.Codigo,
                    CodigoControleVisitaPessoa = controleVisita.ControleVisitaPessoa?.Codigo ?? 0,
                    CodigoControleVisitaPessoaFoto = controleVisita.ControleVisitaPessoa != null && controleVisita.ControleVisitaPessoa.Foto != null && controleVisita.ControleVisitaPessoa.Foto.Count > 0 ? controleVisita.ControleVisitaPessoa.Foto.Where(o => o.Status == true).FirstOrDefault()?.Codigo ?? 0 : 0,
                    Numero = controleVisita.Numero.ToString("n0"),
                    DataHoraEntrada = controleVisita.DataHoraEntrada.HasValue ? controleVisita.DataHoraEntrada.Value.ToString("dd/MM/yyyy HH:MM") : string.Empty,
                    DataHoraPrevisaoSaida = controleVisita.DataHoraPrevisaoSaida.HasValue ? controleVisita.DataHoraPrevisaoSaida.Value.ToString("dd/MM/yyyy HH:MM") : string.Empty,
                    CPF = controleVisita.CPF,
                    Nome = controleVisita.Nome,
                    DataNascimento = controleVisita.DataNascimento.HasValue ? controleVisita.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Identidade = controleVisita.Identidade,
                    OrgaoEmissor = controleVisita.OrgaoEmissor,
                    Estado = new { Codigo = controleVisita.Estado?.Sigla ?? string.Empty, Descricao = controleVisita.Estado?.Descricao ?? string.Empty },
                    Empresa = controleVisita.Empresa,
                    Autorizador = new { Codigo = controleVisita.Autorizador?.Codigo ?? 0, Descricao = controleVisita.Autorizador?.Descricao ?? string.Empty },
                    Setor = new { Codigo = controleVisita.Setor?.Codigo ?? 0, Descricao = controleVisita.Setor?.Descricao ?? string.Empty },
                    PlacaVeiculo = controleVisita.PlacaVeiculo,
                    ModeloVeiculo = controleVisita.ModeloVeiculo,
                    Observacao = controleVisita.Observacao,
                    DataHoraSaida = controleVisita.DataHoraSaida.HasValue ? controleVisita.DataHoraSaida.Value.ToString("dd/MM/yyyy HH:MM") : string.Empty,
                    Arquivo = string.Empty,
                    SituacaoControleVisita = controleVisita.Situacao
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

        public async Task<IActionResult> BuscarDadosCPF()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoa repControleVisitaPessoa = new Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoa(unitOfWork);

                // Parametros
                string cpf = Utilidades.String.OnlyNumbers(Request.Params("CPF"));

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita = repControleVisita.BuscarVisitaAbertaCPF(cpf);
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa pessoa = repControleVisitaPessoa.BuscarPorCPF(cpf);

                // Valida
                if (controleVisita == null && pessoa == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro de controle de pessoa e nem de visita.");

                // Formata retorno
                var retorno = new
                {
                    Codigo = controleVisita?.Codigo ?? 0,
                    CodigoControleVisitaPessoa = controleVisita != null ? controleVisita.ControleVisitaPessoa?.Codigo ?? 0 : pessoa?.Codigo ?? 0,
                    CodigoControleVisitaPessoaFoto = pessoa != null && pessoa.Foto != null && pessoa.Foto.Count > 0 ? pessoa.Foto.Where(o => o.Status == true).FirstOrDefault()?.Codigo ?? 0 : 0,
                    Numero = controleVisita?.Numero.ToString("n0") ?? string.Empty,
                    DataHoraEntrada = controleVisita != null ? controleVisita.DataHoraEntrada.HasValue ? controleVisita.DataHoraEntrada.Value.ToString("dd/MM/yyyy hh:MM") : string.Empty : string.Empty,
                    DataHoraPrevisaoSaida = controleVisita != null ? controleVisita.DataHoraPrevisaoSaida.HasValue ? controleVisita.DataHoraPrevisaoSaida.Value.ToString("dd/MM/yyyy hh:MM") : string.Empty : string.Empty,
                    CPF = controleVisita?.CPF ?? pessoa?.CPF ?? string.Empty,
                    Nome = controleVisita?.Nome ?? pessoa?.Nome ?? string.Empty,
                    DataNascimento = controleVisita != null && controleVisita.DataNascimento.HasValue ? controleVisita.DataNascimento.Value.ToString("dd/MM/yyyy") : pessoa != null && pessoa.DataNascimento.HasValue ? pessoa.DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Identidade = controleVisita?.Identidade ?? pessoa?.Identidade ?? string.Empty,
                    OrgaoEmissor = controleVisita?.OrgaoEmissor ?? pessoa?.OrgaoEmissor ?? string.Empty,
                    Estado = controleVisita != null ? new { Codigo = controleVisita.Estado?.Sigla ?? string.Empty, Descricao = controleVisita.Estado?.Descricao ?? string.Empty } : pessoa != null ? new { Codigo = pessoa.Estado?.Sigla ?? string.Empty, Descricao = pessoa.Estado?.Descricao ?? string.Empty } : null,
                    Empresa = controleVisita?.Empresa ?? pessoa?.Empresa ?? string.Empty,
                    Autorizador = controleVisita != null ? new { Codigo = controleVisita.Autorizador?.Codigo ?? 0, Descricao = controleVisita.Autorizador?.Descricao ?? string.Empty } : null,
                    Setor = controleVisita != null ? new { Codigo = controleVisita.Setor?.Codigo ?? 0, Descricao = controleVisita.Setor?.Descricao ?? string.Empty } : null,
                    PlacaVeiculo = controleVisita?.PlacaVeiculo ?? pessoa?.PlacaVeiculo ?? string.Empty,
                    ModeloVeiculo = controleVisita?.ModeloVeiculo ?? pessoa?.ModeloVeiculo ?? string.Empty,
                    Observacao = controleVisita?.Observacao ?? string.Empty,
                    DataHoraSaida = controleVisita != null ? controleVisita.DataHoraSaida.HasValue ? controleVisita.DataHoraSaida.Value.ToString("dd/MM/yyyy hh:MM") : string.Empty : string.Empty,
                    Arquivo = string.Empty,
                    SituacaoControleVisita = controleVisita != null ? controleVisita.Situacao : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Aberto
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita = new Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita();

                // Preenche entidade com dados
                PreencheEntidade(ref controleVisita, unitOfWork, true);
                PreencherEntidadePessoa(ref controleVisita, unitOfWork);

                controleVisita.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Aberto;

                // Valida entidade
                if (!ValidaEntidade(controleVisita, out string erro))
                    return new JsonpResult(false, true, erro);

                // Persiste dados
                repControleVisita.Inserir(controleVisita, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                var dynRetorno = new
                {
                    controleVisita.Codigo
                };

                return new JsonpResult(dynRetorno);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Inicia transacao
                unitOfWork.Start();

                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita = repControleVisita.BuscarPorCodigo(codigo, true);

                // Valida
                if (controleVisita == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                // Preenche entidade com dados
                PreencheEntidade(ref controleVisita, unitOfWork, false);
                PreencherEntidadePessoa(ref controleVisita, unitOfWork);

                // Valida entidade
                if (!ValidaEntidade(controleVisita, out string erro))
                    return new JsonpResult(false, true, erro);

                if (controleVisita.DataHoraSaida.HasValue)
                    controleVisita.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita.Fechado;

                // Persiste dados
                repControleVisita.Atualizar(controleVisita, Auditado);
                unitOfWork.CommitChanges();

                // Retorna sucesso
                var dynRetorno = new
                {
                    controleVisita.Codigo
                };

                return new JsonpResult(dynRetorno);
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

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosControleVisita()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);
                Dominio.Entidades.Usuario usuarioLogado = this.Usuario;

                var dadosUsuarioLogado = new
                {
                    usuarioLogado.Codigo,
                    usuarioLogado.Nome,
                    ProximoNumero = repControleVisita.BuscarProximoNumero()
                };

                return new JsonpResult(dadosUsuarioLogado);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do usuário.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarFoto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoaFoto repControleVisitaPessoaFoto = new Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoaFoto(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

                int codigoControleVisita = 0;
                int.TryParse(Request.Params("CodigoControleVisita"), out codigoControleVisita);

                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita = repControleVisita.BuscarPorCodigo(codigoControleVisita);
                unitOfWork.Start();

                if (controleVisita.ControleVisitaPessoa != null)
                {
                    List<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto> fotosAtivas = repControleVisitaPessoaFoto.BuscarFotosAtivas(controleVisita.ControleVisitaPessoa.Codigo);
                    foreach (var foto in fotosAtivas)
                    {
                        foto.Status = false;
                        repControleVisitaPessoaFoto.Atualizar(foto);
                    }
                }

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "FotosControleVisita");
                
                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto controleVisitaPessoaFoto = new Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto();

                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminhoSave, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    controleVisitaPessoaFoto.CaminhoArquivo = caminho;
                    controleVisitaPessoaFoto.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));
                    controleVisitaPessoaFoto.Status = true;
                    controleVisitaPessoaFoto.ControleVisitaPessoa = controleVisita.ControleVisitaPessoa;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, controleVisita, null, "Adicionou a foto " + controleVisitaPessoaFoto.CaminhoArquivo + ".", unitOfWork);

                    repControleVisitaPessoaFoto.Inserir(controleVisitaPessoaFoto);
                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterImagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita visita = repControleVisita.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    Imagens = (from foto in visita.ControleVisitaPessoa.Foto.Where(o => o.Status == true)
                               select new
                               {
                                   foto.Codigo,
                                   Numero = visita.Numero,
                                   Miniatura = ObterMiniatura(foto)
                               }).ToList()
                });
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

        public async Task<IActionResult> ImprimirEtiqueta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string cpf = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF"));
                if (string.IsNullOrWhiteSpace(cpf))
                    return new JsonpResult(false, "Favor digite ao menos o CPF para a geração da etiqueta.");

                Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.EtiquetaControleVisita etiquetaControleVisita =
                    new Dominio.Relatorios.Embarcador.DataSource.GestaoPatio.EtiquetaControleVisita()
                    {
                        Autorizador = Request.GetStringParam("Autorizador"),
                        CPF = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF")),
                        Empresa = Request.GetStringParam("Empresa"),
                        ModeloVeiculo = Request.GetStringParam("ModeloVeiculo"),
                        Nome = Request.GetStringParam("Nome"),
                        PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                        Setor = Request.GetStringParam("Setor"),
                        CodigoBarras = null
                    };

                byte[] pdf = ReportRequest.WithType(ReportType.EtiquetaControleVisita)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("EtiquetaControleVisitaDs", etiquetaControleVisita.ToJson())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", "Etiqueta de Visita.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        public string ObterMiniatura(Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoaFoto foto)
        {
            string filepath = foto.CaminhoArquivo;

            if (Path.GetExtension(filepath).ToLower() == ".tif")
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(filepath)))
                {
                    if (System.Drawing.Imaging.ImageFormat.Tiff.Equals(image.RawFormat))
                    {
                        string tmp = Path.GetTempFileName();
                        Utilidades.IO.FileStorageService.Storage.SaveImage(tmp, image, System.Drawing.Imaging.ImageFormat.Png);
                        filepath = tmp;
                    }
                }
            }

            string newTemp = Path.GetTempFileName();
            using (Image imgPhoto = Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(filepath)))
            using (Bitmap newImage = ResizeImage(imgPhoto, 850))
                Utilidades.IO.FileStorageService.Storage.SaveImage(newTemp, newImage);

            byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(newTemp);
            return Convert.ToBase64String(imageArray);
        }

        private Bitmap ResizeImage(Image image, int newWidth)
        {
            int newHeight = (image.Height * newWidth) / image.Width;
            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /* GridPesquisa
         * Retorna o model de Grid para a o módulo
         */
        private Models.Grid.Grid GridPesquisa()
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            // Cabecalhos grid
            grid.Prop("Codigo");
            grid.Prop("Nome").Nome("Nome").Tamanho(35).Align(Models.Grid.Align.left);
            grid.Prop("CPF").Nome("CPF").Tamanho(15).Align(Models.Grid.Align.left);
            grid.Prop("Empresa").Nome("Empresa").Tamanho(20).Align(Models.Grid.Align.left);
            grid.Prop("SituacaoControleVisita").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

            return grid;
        }

        /* ExecutaPesquisa
         * Converte os dados recebidos e executa a busca
         * Retorna um dynamic pronto para adicionar ao grid
         */
        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

            // Dados do filtro
            Enum.TryParse(Request.Params("SituacaoControleVisita"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoControleVisita situacaoControleVisita);

            string cpf = Request.Params("CPF");
            string nome = Request.Params("Nome");
            string empresa = Request.Params("Empresa");

            // Consulta
            List<Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita> listaGrid = repControleVisita.Consultar(cpf, nome, empresa, situacaoControleVisita, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repControleVisita.ContarConsulta(cpf, nome, empresa, situacaoControleVisita);

            var lista = from obj in listaGrid
                        select new
                        {
                            obj.Codigo,
                            obj.Nome,
                            obj.CPF,
                            obj.Empresa,
                            SituacaoControleVisita = obj.DescricaoSituacaoControleVisita
                        };

            return lista.ToList();
        }

        /* PreencheEntidade
         * Recebe uma instancia da entidade
         * Converte parametros recebido por request
         * Atribui a entidade
         */
        private void PreencherEntidadePessoa(ref Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoa repControleVisitaPessoa = new Repositorio.Embarcador.GestaoPatio.ControleVisitaPessoa(unitOfWork);
            Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa pessoa = repControleVisitaPessoa.BuscarPorCPF(controleVisita.CPF);
            if (pessoa != null)
            {
                pessoa.CPF = controleVisita.CPF;
                pessoa.DataNascimento = controleVisita.DataNascimento;
                pessoa.Empresa = controleVisita.Empresa;
                pessoa.Estado = controleVisita.Estado;
                pessoa.Identidade = controleVisita.Identidade;
                pessoa.ModeloVeiculo = controleVisita.ModeloVeiculo;
                pessoa.Nome = controleVisita.Nome;
                pessoa.OrgaoEmissor = controleVisita.OrgaoEmissor;
                pessoa.PlacaVeiculo = controleVisita.PlacaVeiculo;

                repControleVisitaPessoa.Atualizar(pessoa);
                controleVisita.ControleVisitaPessoa = pessoa;
            }
            else
            {
                pessoa = new Dominio.Entidades.Embarcador.GestaoPatio.ControleVisitaPessoa()
                {
                    CPF = controleVisita.CPF,
                    DataNascimento = controleVisita.DataNascimento,
                    Empresa = controleVisita.Empresa,
                    Estado = controleVisita.Estado,
                    Identidade = controleVisita.Identidade,
                    ModeloVeiculo = controleVisita.ModeloVeiculo,
                    Nome = controleVisita.Nome,
                    OrgaoEmissor = controleVisita.OrgaoEmissor,
                    PlacaVeiculo = controleVisita.PlacaVeiculo
                };

                repControleVisitaPessoa.Inserir(pessoa);
                controleVisita.ControleVisitaPessoa = pessoa;
            }

        }
        private void PreencheEntidade(ref Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita, Repositorio.UnitOfWork unitOfWork, bool inserindo)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Setor repSetor = new Repositorio.Setor(unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.ControleVisita repControleVisita = new Repositorio.Embarcador.GestaoPatio.ControleVisita(unitOfWork);

            // Vincula dados
            controleVisita.Autorizador = Request.GetIntParam("Autorizador") > 0 ? repUsuario.BuscarPorCodigo(Request.GetIntParam("Autorizador")) : null;
            controleVisita.CPF = Utilidades.String.OnlyNumbers(Request.GetStringParam("CPF"));
            controleVisita.DataHoraEntrada = Request.GetNullableDateTimeParam("DataHoraEntrada");
            controleVisita.DataHoraPrevisaoSaida = Request.GetNullableDateTimeParam("DataHoraPrevisaoSaida");
            controleVisita.DataHoraSaida = Request.GetNullableDateTimeParam("DataHoraSaida");
            controleVisita.DataNascimento = Request.GetNullableDateTimeParam("DataNascimento");
            controleVisita.Empresa = Request.GetStringParam("Empresa");
            controleVisita.Estado = !string.IsNullOrWhiteSpace(Request.GetStringParam("Estado")) ? repEstado.BuscarPorSigla(Request.GetStringParam("Estado")) : null;
            controleVisita.Identidade = Request.GetStringParam("Identidade");
            controleVisita.ModeloVeiculo = Request.GetStringParam("ModeloVeiculo");
            controleVisita.Nome = Request.GetStringParam("Nome");
            controleVisita.ControleVisitaPessoa = null;
            if (inserindo)
                controleVisita.Numero = repControleVisita.BuscarProximoNumero();
            controleVisita.Observacao = Request.GetStringParam("Observacao");
            controleVisita.OrgaoEmissor = Request.GetStringParam("OrgaoEmissor");
            controleVisita.PlacaVeiculo = Request.GetStringParam("PlacaVeiculo");
            controleVisita.Setor = Request.GetIntParam("Setor") > 0 ? repSetor.BuscarPorCodigo(Request.GetIntParam("Setor")) : null;
        }

        /* ValidaEntidade
         * Recebe uma instancia da entidade
         * Valida informacoes
         * Retorna de entidade e valida ou nao e retorna erro (se tiver)
         */
        private bool ValidaEntidade(Dominio.Entidades.Embarcador.GestaoPatio.ControleVisita controleVisita, out string msgErro)
        {
            msgErro = "";

            if (string.IsNullOrWhiteSpace(controleVisita.CPF))
            {
                msgErro = "CPF é obrigatória.";
                return false;
            }

            return true;
        }

        /* PropOrdena
         * Recebe o campo ordenado na grid
         * Retorna o elemento especifico da entidade para ordenacao
         */
        private void PropOrdena(ref string propOrdenar)
        {
        }
        #endregion
    }
}
