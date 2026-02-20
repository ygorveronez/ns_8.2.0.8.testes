using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.Entidades.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Patrimonio;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Repositorio;
using Repositorio.Embarcador.Configuracoes;
using Repositorio.Embarcador.Patrimonio;
using Servicos.Embarcador.Configuracoes;
using SGT.WebAdmin.Models.Grid;
using SGTAdmin.Controllers;
using System.Drawing;

namespace SGT.WebAdmin.Controllers.Patrimonio
{
    [CustomAuthorize("Patrimonio/Pet")]
    public class PetController : BaseController
    {
        public PetController(Conexao conexao) : base(conexao) { }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisar()
		{
			UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                FiltroPesquisaPet filtroPesquisaPet = ObterFiltrosPesquisa();

                Grid grid = new(Request);
                grid.header = new List<Head>();
                grid.AdicionarCabecalho(Localization.Resources.Patrimonio.Pet.Codigo, "Codigo", 30, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Patrimonio.Pet.Nome, "Nome", 40, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Patrimonio.Pet.Tutor, "NomeTutor", 40, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Patrimonio.Pet.TelefoneTutor, "TelefoneTutor", 40, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Configuracoes.Especie.DescricaoEspecie, "Especie", 40, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Configuracoes.EspecieRaca.Raca, "Raca", 40, Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Pessoas.Pessoa.Sexo, "Sexo", 20, Align.left, true);

                PetRepositorio petRepositorio = new(workUnit);
                List<Pet> pets = petRepositorio.Consultar(filtroPesquisaPet, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(pets.Count);

                grid.AdicionaRows((from pet in pets
                                   select new
                                   {
                                       pet.Codigo,
                                       pet.Nome,
                                       NomeTutor = pet.Tutor != null ? pet.Tutor?.Nome : string.Empty,
                                       TelefoneTutor = pet.Tutor != null ? (pet.Tutor?.Telefone1 ?? pet.Tutor?.Telefone2).ObterTelefoneFormatado() : string.Empty,
                                       Especie = pet.Especie != null ? pet.Especie?.Descricao : string.Empty,
                                       Raca = pet.Raca != null ? pet.Raca?.Descricao : string.Empty,
                                       Sexo = pet.Sexo == Sexo.NaoInformado ? "Não informado" : (pet.Sexo == Sexo.Masculino ? "Macho" : "Fêmea")
                                   }).ToList());

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao pesquisar.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                if (codigo <= 0)
                    return new JsonpResult(false, "Código é obrigatório");

                PetRepositorio petRepositorio = new(workUnit);
                Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);

                string caminho = ObterCaminhoArquivoFoto(workUnit);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();
                string imagemBase64 = string.Empty;
                if (!string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    byte[] imageArray = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(nomeArquivo);
                    imagemBase64 = Convert.ToBase64String(imageArray); //Utilidades.File.GerarGZIP(imageArray);
                }

                PetAnexoRepositorio petAnexoRepositorio = new(workUnit);
                List<PetAnexo> petAnexos = petAnexoRepositorio.BuscarTodosPorCodigoPet(codigo);

                var dynPet = new
                {
                    pet.Codigo,
                    pet.Ativo,
                    pet.Castrado,
                    pet.Microchip,
                    Tutor = new
                    {
                        Codigo = pet.Tutor != null ? pet.Tutor.Codigo : 0,
                        Descricao = pet.Tutor != null ? pet.Tutor.Nome : string.Empty,
                    },
                    Especie = new
                    {
                        Codigo = pet.Especie != null ? pet.Especie.Codigo : 0,
                        Descricao = pet.Especie != null ? pet.Especie.Descricao : string.Empty,
                    },
                    Raca = new
                    {
                        Codigo = pet.Raca != null ? pet.Raca.Codigo : 0,
                        Descricao = pet.Raca != null ? pet.Raca.Descricao : string.Empty,
                    },
                    Cor = new
                    {
                        Codigo = pet.Cor != null ? pet.Cor.Codigo : 0,
                        Descricao = pet.Cor != null ? pet.Cor.Descricao : string.Empty,
                    },
                    PlanoServico = new
                    {
                        Codigo = pet.PlanoServico != null ? pet.PlanoServico.Codigo : 0,
                        Descricao = pet.PlanoServico != null ? pet.PlanoServico.Descricao : string.Empty,
                    },
                    pet.Sexo,
                    pet.Porte,
                    pet.Pelagem,
                    pet.Comportamento,
                    DataNascimento = pet.DataNascimento.ToString("dd/MM/yyyy HH:mm"),
                    UltimaVisita = pet.UltimaVisita.ToString("dd/MM/yyyy HH:mm"),
                    pet.Nome,
                    pet.Observacao,
                    pet.Peso,
                    Anexos = (from obj in petAnexos
                              select new
                              {
                                  obj.Codigo,
                                  Guid = obj.GuidArquivo,
                                  obj.Descricao,
                                  obj.NomeArquivo,
                              }).ToList(),
                    FotoPet = !string.IsNullOrWhiteSpace(nomeArquivo) ? ("data:image/png;base64," + imagemBase64) : string.Empty,
                };

                return new JsonpResult(dynPet);

            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);
            try
            {
                Pet pet = new();
                PreencherPet(pet, workUnit);
                PetRepositorio petRepositorio = new(workUnit);

                workUnit.Start();
                var id = petRepositorio.Inserir(pet);
                workUnit.CommitChanges();

                return new JsonpResult(id, true, "Pet inserido com sucesso!");
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                if (codigo < 0)
                    return new JsonpResult(false, "Código inválido.");

                PetRepositorio petRepositorio = new(workUnit);
                Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);
                if (pet == null)
                    return new JsonpResult(false, "Ocorreu um erro ao carregar os dados do pet.");

                PreencherPet(pet, workUnit);

                workUnit.Start();
                petRepositorio.Atualizar(pet);
                workUnit.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> Excluir()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("Codigo");
                if (codigo < 0)
                    return new JsonpResult(false, "Código inválido.");

                workUnit.Start();

                // (O deletar da foto é feito pelo javascript chamando outro endpoint)

                PetAnexoRepositorio petAnexoRepositorio = new(workUnit);

                // Obtenho todos os anexos
                List<PetAnexo> petAnexos = petAnexoRepositorio.BuscarTodosPorCodigoPet(codigo);

                // Deleto os anexos na pasta
                foreach (PetAnexo petAnexo in petAnexos)
                {
                    string caminho = CaminhoArquivos(workUnit);
                    var extensaoArquivo = System.IO.Path.GetExtension(petAnexo.NomeArquivo).ToLower();
                    string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, petAnexo.GuidArquivo + extensaoArquivo);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                        Utilidades.IO.FileStorageService.Storage.Delete(arquivo);
                }

                // Deleto o registro dos anexos no banco de dados
                petAnexoRepositorio.Deletar(petAnexos);

                PetRepositorio petRepositorio = new(workUnit);
                Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);
                if (pet == null)
                    return new JsonpResult(false, "Ocorreu um erro ao carregar os dados do pet.");

                petRepositorio.Deletar(pet);
                workUnit.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarFoto()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                if (codigo <= 0)
                    return new JsonpResult(false, "Falha ao inserir imagem do pet");

                PetRepositorio petRepositorio = new(workUnit);
                Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);
                if (pet == null)
                    return new JsonpResult(false, "Falha ao inserir imagem do pet");

                IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("ArquivoFoto");
                if (arquivos.Count <= 0)
                    return new JsonpResult(false, true, "Foto do pet nao encontrada");

                Servicos.DTO.CustomFile arquivoFoto = arquivos[0];
                string extensaoArquivo = System.IO.Path.GetExtension(arquivoFoto.FileName).ToLower();
                string caminho = ObterCaminhoArquivoFoto(workUnit);
                string nomeArquivoFotoExistente = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(nomeArquivoFotoExistente))
                    Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivoFotoExistente);

                using (System.Drawing.Image t = System.Drawing.Image.FromStream(HttpContext.GetFile().InputStream))
                {
                    using (System.Drawing.Image novaImagem = Utilidades.Image.RedimensionarImagem(t, new Size(1300, 1300)))
                    {
                        Utilidades.IO.FileStorageService.Storage.SaveImage(Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}{extensaoArquivo}"), novaImagem);
                    }
                }

                pet.CaminhoFoto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{codigo}{extensaoArquivo}");
                workUnit.Start();
                petRepositorio.Atualizar(pet);
                workUnit.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a foto.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> RemoverFoto()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);
                if (codigo < 0)
                    return new JsonpResult(false, "Código inválido.");

                PetRepositorio petRepositorio = new(workUnit);
                Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);
                if (pet == null)
                    return new JsonpResult(false, "Ocorreu um erro ao carregar os dados da espécie.");

                string caminho = ObterCaminhoArquivoFoto(workUnit);
                string nomeArquivo = Utilidades.IO.FileStorageService.Storage.GetFiles(caminho, $"{codigo}.*").FirstOrDefault();

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                    return new JsonpResult(false, true, "Foto não encontrada");

                Utilidades.IO.FileStorageService.Storage.Delete(nomeArquivo);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        public async Task<IActionResult> AnexarArquivos()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Pet");

                if (!this.InsereArquivos(codigo, workUnit, out string erro))
                    return new JsonpResult(false, erro);

                PetAnexoRepositorio petAnexoRepositorio = new(workUnit);
                List<PetAnexo> anexos = petAnexoRepositorio.BuscarTodosPorCodigoPet(codigo);

                var dynAnexos = (from obj in anexos
                                 select new
                                 {
                                     obj.Codigo,
                                     Guid = obj.GuidArquivo,
                                     obj.Descricao,
                                     obj.NomeArquivo,
                                 }).ToList();

                return new JsonpResult(new
                {
                    Anexos = dynAnexos
                });
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao anexar arquivo.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                PetAnexoRepositorio petAnexoRepositorio = new(workUnit);
                PetAnexo petAnexo = petAnexoRepositorio.BuscarPorCodigo(codigo, false);
                if (petAnexo == null)
                    return new JsonpResult(false, "Ocorreu um erro ao buscar dados do anexo.");

                string caminho = this.CaminhoArquivos(workUnit);
                string extencao = System.IO.Path.GetExtension(petAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, petAnexo.GuidArquivo + extencao);
                byte[] bArquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo);

                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", petAnexo.NomeArquivo);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar o anexo");
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o anexo");
            }
            finally
            {
                workUnit.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirAnexo()
        {
            UnitOfWork workUnit = new(_conexao.StringConexao);

            try
            {
                List<PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Patrimonio/Pet");

                int codigo = Request.GetIntParam("Codigo");
                PetAnexoRepositorio petAnexoRepositorio = new(workUnit);
                PetAnexo petAnexo = petAnexoRepositorio.BuscarPorCodigo(codigo, false);

                if (petAnexo == null)
                    return new JsonpResult(false, "Erro ao buscar o anexo para excluir.");

                string caminho = CaminhoArquivos(workUnit);
                var extensaoArquivo = System.IO.Path.GetExtension(petAnexo.NomeArquivo).ToLower();
                string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, petAnexo.GuidArquivo + extensaoArquivo);

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    return new JsonpResult(false, "Erro ao deletar o anexo.");
                else
                    Utilidades.IO.FileStorageService.Storage.Delete(arquivo);

                petAnexoRepositorio.Deletar(petAnexo);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                workUnit.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao fazer deletar o anexo.");
            }
            finally
            {
                workUnit.Dispose();
            }
        }

        private void PreencherPet(Pet pet, UnitOfWork workUnit)
        {
            FiltroPesquisaPet parametros = ObterParametros();
            Cliente clienteRepositorio = new(workUnit);
            EspecieRepositorio especieRepositorio = new(workUnit);
            EspecieRacaRepositorio especieRacaRepositorio = new(workUnit);
            CorAnimalRepositorio corAnimalRepositorio = new(workUnit);
            PlanoServicoRepositorio planoServicoRepositorio = new(workUnit);

            pet.Ativo = parametros.Ativo;
            pet.Castrado = parametros.Castrado;
            pet.Microchip = parametros.Microchip;
            pet.Tutor = parametros.TutorCodigo > 0 ? clienteRepositorio.BuscarPorCPFCNPJSemFetch(parametros.TutorCodigo) : null;
            pet.Especie = parametros.EspecieCodigo > 0 ? especieRepositorio.BuscarPorCodigo(parametros.EspecieCodigo, false) : null;
            pet.Raca = parametros.RacaCodigo > 0 ? especieRacaRepositorio.BuscarPorCodigo(parametros.RacaCodigo, false) : null;
            pet.Cor = parametros.CorCodigo > 0 ? corAnimalRepositorio.BuscarPorCodigo(parametros.CorCodigo, false) : null;
            pet.PlanoServico = parametros.PlanoServicoCodigo > 0 ? planoServicoRepositorio.BuscarPorCodigo(parametros.PlanoServicoCodigo, false) : null;
            pet.Sexo = parametros.Sexo;
            pet.Porte = parametros.Porte;
            pet.Pelagem = parametros.Pelagem;
            pet.Comportamento = parametros.Comportamento;
            pet.DataNascimento = parametros.DataNascimento;
            pet.UltimaVisita = parametros.UltimaVisita;
            pet.Nome = parametros.Nome;
            pet.Observacao = parametros.Observacao;
            pet.CaminhoFoto = parametros.CaminhoFoto;
            pet.Peso = parametros.Peso;

            pet.Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa : null;

        }

        private FiltroPesquisaPet ObterParametros()
        {
            FiltroPesquisaPet parametros = new()
            {
                Ativo = Request.GetBoolParam("Ativo"),
                Castrado = Request.GetBoolParam("Castrado"),
                Microchip = Request.GetBoolParam("Microchip"),
                TutorCodigo = Request.GetDoubleParam("Tutor"),
                EspecieCodigo = Request.GetIntParam("Especie"),
                RacaCodigo = Request.GetIntParam("Raca"),
                CorCodigo = Request.GetIntParam("Cor"),
                PlanoServicoCodigo = Request.GetIntParam("PlanoServico"),
                Sexo = Request.GetEnumParam<Sexo>("Sexo"),
                Porte = Request.GetEnumParam<Porte>("Porte"),
                Pelagem = Request.GetEnumParam<Pelagem>("Pelagem"),
                Comportamento = Request.GetEnumParam<Comportamento>("Comportamento"),
                DataNascimento = Request.GetDateTimeParam("DataNascimento"),
                UltimaVisita = Request.GetDateTimeParam("UltimaVisita"),
                Nome = Request.GetStringParam("Nome"),
                Observacao = Request.GetStringParam("Observacao"),
                CaminhoFoto = Request.GetStringParam("CaminhoFoto"),
                Peso = Request.GetDecimalParam("Peso")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                parametros.EmpresaCodigo = Usuario.Empresa.Codigo;

            return parametros;
        }

        private FiltroPesquisaPet ObterFiltrosPesquisa()
        {
            FiltroPesquisaPet filtrosPesquisa = new()
            {
                TutorCodigo = Request.GetDoubleParam("Tutor"),
                EspecieCodigo = Request.GetIntParam("Especie"),
                RacaCodigo = Request.GetIntParam("Raca"),
                CorCodigo = Request.GetIntParam("Cor"),
                Nome = Request.GetStringParam("Nome"),
                SituacaoAtivo = Request.GetEnumParam("Ativo", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos),
                Porte = Request.GetEnumParam("Porte", Porte.Todos),
                Sexo = Request.GetEnumParam("Sexo", Sexo.NaoInformado),
                Pelagem = Request.GetEnumParam("Pelagem", Pelagem.Todas)
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.EmpresaCodigo = Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        private string ObterCaminhoArquivoFoto(UnitOfWork workUnit)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { ConfigurationInstance.GetInstance(workUnit)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Foto", "Pet" });
        }

        private dynamic InsereArquivos(int codigo, UnitOfWork workUnit, out string erro)
        {
            erro = string.Empty;

            PetRepositorio petRepositorio = new(workUnit);
            Pet pet = petRepositorio.BuscarPorCodigo(codigo, false);
            if (pet == null)
            {
                erro = "Pet não localizado para anexar arquivo.";
                return false;
            }

            IList<Servicos.DTO.CustomFile> arquivos = HttpContext.GetFiles("Arquivo");
            if (arquivos.Count <= 0)
            {
                erro = "Nenhum arquivo selecionado para envio.";
                return false;
            }

            string[] descricoes = Request.Form["Descricao"].ToArray();
            string[] guids = Request.Form["Guid"].ToArray();

            List<PetAnexo> petAnexos = new List<PetAnexo>();
            for (var i = 0; i < arquivos.Count(); i++)
            {
                // Extrai dados
                Servicos.DTO.CustomFile file = arquivos[i];
                var nomeArquivo = file.FileName;
                var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                var guidArquivo = i < guids.Length ? guids[i] : string.Empty; //Guid.NewGuid().ToString().Replace("-", "");
                string caminho = this.CaminhoArquivos(workUnit);

                // Salva na pasta
                file.SaveAs(caminho + guidArquivo + extensaoArquivo);

                // Insere no banco
                PetAnexo petAnexo = new()
                {
                    Pet = pet,
                    Descricao = i < descricoes.Length ? descricoes[i] : string.Empty,
                    GuidArquivo = guidArquivo,
                    NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)))
                };
                petAnexos.Add(petAnexo);
            }

            PetAnexoRepositorio petAnexoRepositorio = new(workUnit);
            workUnit.Start();
            petAnexoRepositorio.Inserir(petAnexos);
            workUnit.CommitChanges();

            return true;
        }

        private string CaminhoArquivos(Repositorio.UnitOfWork workUnit)
        {
            return Utilidades.Directory.CriarCaminhoArquivos(new string[] { Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(workUnit)?.ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "Pet" });
        }
    }
}
