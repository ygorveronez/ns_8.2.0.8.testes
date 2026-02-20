using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dominio.Entidades.Embarcador.Atendimento;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    [CustomAuthorize("Atendimentos/Chamado")]
    public class ChamadoController : BaseController
    {
		#region Construtores

		public ChamadoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);

                int numeroInicial = 0, numeroFinal = 0, codigoSistema = 0, codigoModulo = 0, codigoTela = 0;
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int.TryParse(Request.Params("Sistema"), out codigoSistema);
                int.TryParse(Request.Params("Modulo"), out codigoModulo);
                int.TryParse(Request.Params("Tela"), out codigoTela);
                int.TryParse(Request.Params("Solicitante"), out int codigoSolicitante);

                string motivo = Request.Params("MotivoProblema");
                string titulo = Request.Params("Titulo");

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusAberto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusCancelado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa statusFinalizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade;
                Enum.TryParse(Request.Params("Prioridade"), out prioridade);

                List<int> status = JsonConvert.DeserializeObject<List<int>>(Request.Params("Status"));
                for (int i = 0; i < status.Count(); i++)
                {
                    if (status[i] == 1)
                        statusAberto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Aberto;
                    else if (status[i] == 2)
                        statusCancelado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado;
                    else if (status[i] == 3)
                        statusFinalizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Finalizado;
                }

                int empresa = 0;
                int empresaPai = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    empresaPai = this.Usuario.Empresa.Codigo;
                else if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    empresa = this.Usuario.Empresa.Codigo;
                    empresaPai = this.Usuario.Empresa.EmpresaPai != null ? this.Usuario.Empresa.EmpresaPai.Codigo : 0;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Data Abertura", "Data", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Motivo / Problema", "MotivoProblema", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Sistema", false);
                grid.AdicionarCabecalho("Título", "Titulo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Módulo", "Modulo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Tela", "Tela", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "Prioridade", 10, Models.Grid.Align.center, true);

                List<Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa> listaChamado = repChamado.Consultar(TipoServicoMultisoftware, motivo, statusAberto, statusCancelado, statusFinalizado, codigoSistema, codigoModulo, codigoTela, empresa, empresaPai, numeroInicial, numeroFinal, titulo, dataInicial, dataFinal, prioridade, codigoSolicitante, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repChamado.ContarConsulta(TipoServicoMultisoftware, motivo, statusAberto, statusCancelado, statusFinalizado, codigoSistema, codigoModulo, codigoTela, empresa, empresaPai, numeroInicial, numeroFinal, titulo, dataInicial, dataFinal, prioridade, codigoSolicitante));
                var lista = (from p in listaChamado
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Atendimento != null ? p.Atendimento.Numero : 0,
                                 Data = p.Data.Value.ToString("dd/MM/yyyy HH:mm"),
                                 p.MotivoProblema,
                                 Sistema = p.AtendimentoSistema != null ? p.AtendimentoSistema.Descricao : string.Empty,
                                 p.Titulo,
                                 Modulo = p.AtendimentoModulo != null ? p.AtendimentoModulo.Descricao : string.Empty,
                                 Tela = p.AtendimentoTela != null ? p.AtendimentoTela.Descricao : string.Empty,
                                 Prioridade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimentoHelper.ObterDescricao(p.Prioridade)
                             }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa();

                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

                int codigoSistema = 0, codigoModulo = 0, codigoTela = 0, codigoEmpresa = 0, codigoTipoAtendimento = 0;
                int.TryParse(Request.Params("Sistema"), out codigoSistema);
                int.TryParse(Request.Params("Modulo"), out codigoModulo);
                int.TryParse(Request.Params("Tela"), out codigoTela);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("TipoAtendimento"), out codigoTipoAtendimento);
                int.TryParse(Request.Params("Solicitante"), out int codigoSolicitante);

                string titulo = Request.Params("Titulo");
                string motivoProblema = Request.Params("MotivoProblema");
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa status;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade;
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("Prioridade"), out prioridade);

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = new Dominio.Entidades.Embarcador.Atendimento.Atendimento();

                atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;
                atendimento.TipoContato = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoContatoAtendimento.Outros;
                atendimento.TipoAcessoRemoto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcessoRemoto.Nenhum;
                atendimento.Prioridade = prioridade;
                atendimento.ContatoAtendimento = this.Usuario?.Nome;
                if (codigoEmpresa == 0)
                {
                    atendimento.Empresa = this.Usuario?.Empresa?.EmpresaPai;
                    atendimento.EmpresaFilho = this.Usuario?.Empresa;
                }
                else
                {
                    atendimento.Empresa = this.Usuario?.Empresa;
                    atendimento.EmpresaFilho = repEmpresa.BuscarPorCodigo(codigoEmpresa);
                }
                atendimento.Numero = repAtendimento.BuscarUltimoNumero(TipoServicoMultisoftware, this.Usuario?.Empresa?.EmpresaPai != null ? this.Usuario.Empresa.EmpresaPai.Codigo : 0);

                repAtendimento.Inserir(atendimento);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Adicionou o chamado {chamado.Descricao}.", unitOfWork);
                
                InserirLogAtendimento(atendimento, chamado, unitOfWork);

                AdicionarRespostas(unitOfWork, atendimento, chamado);

                chamado.Atendimento = atendimento;
                chamado.Prioridade = prioridade;
                chamado.Status = status;
                chamado.MotivoProblema = motivoProblema;
                chamado.JustificativaObservacao = observacao;
                chamado.Data = DateTime.Now;
                chamado.AtendimentoTela = repTela.BuscarPorCodigo(codigoTela);
                chamado.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                chamado.AtendimentoModulo = repModulo.BuscarPorCodigo(codigoModulo);
                chamado.Titulo = titulo;

                chamado.AtendimentoTipo = codigoTipoAtendimento > 0 ? repTipo.BuscarPorCodigo(codigoTipoAtendimento) : null;
                chamado.Solicitante = codigoSolicitante > 0 ? repUsuario.BuscarPorCodigo(codigoSolicitante) : null;

                EnviarChamadoAoRedMine(chamado, unitOfWork);

                repChamado.Inserir(chamado);

                InserirLogChamado(chamado, atendimento, unitOfWork);
                if (chamado.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Aberto)
                    EnviarEmailChamado(chamado, unitOfWork, true);

                unitOfWork.CommitChanges();

                object retorno = new
                {
                    chamado.Codigo
                };

                return new JsonpResult(retorno, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                unitOfWork.Start();
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado = repChamado.BuscarPorCodigo(int.Parse(Request.Params("Codigo")));

                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTela repTela = new Repositorio.Embarcador.Atendimento.AtendimentoTela(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoSistema repSistema = new Repositorio.Embarcador.Atendimento.AtendimentoSistema(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoModulo repModulo = new Repositorio.Embarcador.Atendimento.AtendimentoModulo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                int codigoSistema = 0, codigoModulo = 0, codigoTipo = 0, codigoTela = 0, codigoTipoAtendimento = 0;
                int.TryParse(Request.Params("Sistema"), out codigoSistema);
                int.TryParse(Request.Params("Modulo"), out codigoModulo);
                int.TryParse(Request.Params("Tipo"), out codigoTipo);
                int.TryParse(Request.Params("Tela"), out codigoTela);
                int.TryParse(Request.Params("TipoAtendimento"), out codigoTipoAtendimento);
                int.TryParse(Request.Params("Solicitante"), out int codigoSolicitante);

                string titulo = Request.Params("Titulo");
                string motivoProblema = Request.Params("MotivoProblema");
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa status;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade;
                Enum.TryParse(Request.Params("Status"), out status);
                Enum.TryParse(Request.Params("Prioridade"), out prioridade);

                chamado.Prioridade = prioridade;
                chamado.Status = status;
                chamado.MotivoProblema = motivoProblema;
                chamado.JustificativaObservacao = observacao;
                chamado.AtendimentoTela = repTela.BuscarPorCodigo(codigoTela);
                chamado.AtendimentoSistema = repSistema.BuscarPorCodigo(codigoSistema);
                chamado.AtendimentoModulo = repModulo.BuscarPorCodigo(codigoModulo);
                chamado.Titulo = titulo;

                chamado.AtendimentoTipo = codigoTipoAtendimento > 0 ? repTipo.BuscarPorCodigo(codigoTipoAtendimento) : null;
                chamado.Solicitante = codigoSolicitante > 0 ? repUsuario.BuscarPorCodigo(codigoSolicitante) : null;

                InativarAnexos(unitOfWork, chamado);
                EnviarChamadoAoRedMine(chamado, unitOfWork);

                repChamado.Atualizar(chamado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Atualizou o chamado {chamado.Descricao}.", unitOfWork);


                InserirLogChamado(chamado, chamado.Atendimento, unitOfWork);
                if (chamado.Atendimento != null)
                {
                    AtualizarChamadoNoAtendimento(chamado, unitOfWork);
                    AdicionarRespostas(unitOfWork, chamado.Atendimento, chamado);
                    InativarRespostas(unitOfWork, chamado);
                    if (chamado.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado)
                        EnviarEmailChamado(chamado, unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado = repChamado.BuscarPorCodigo(codigo, auditavel: true);

                object retorno = null;

                if (chamado != null)
                {
                    retorno = new
                    {
                        chamado.Codigo,
                        chamado.Prioridade,
                        chamado.Status,
                        chamado.MotivoProblema,
                        Observacao = chamado.JustificativaObservacao,
                        Data = chamado.Data.Value.ToString("dd/MM/yyyy HH:mm"),
                        chamado.HoraInicial,
                        chamado.HoraFinal,
                        chamado.Duracao,
                        Empresa = chamado.Atendimento != null ? new { Codigo = chamado.Atendimento.EmpresaFilho.Codigo, Descricao = chamado.Atendimento.EmpresaFilho.RazaoSocial } : null,
                        Atendimento = chamado.Atendimento != null ? new { Codigo = chamado.Atendimento.Codigo, Descricao = chamado.Atendimento.Numero } : null,
                        Tela = chamado.AtendimentoTela != null ? new { Codigo = chamado.AtendimentoTela.Codigo, Descricao = chamado.AtendimentoTela.Descricao } : null,
                        TipoAtendimento = chamado.AtendimentoTipo != null ? new { Codigo = chamado.AtendimentoTipo.Codigo, Descricao = chamado.AtendimentoTipo.Descricao } : null,
                        Sistema = chamado.AtendimentoSistema != null ? new { Codigo = chamado.AtendimentoSistema.Codigo, Descricao = chamado.AtendimentoSistema.Descricao } : null,
                        Modulo = chamado.AtendimentoModulo != null ? new { Codigo = chamado.AtendimentoModulo.Codigo, Descricao = chamado.AtendimentoModulo.Descricao } : null,
                        ListaAnexos = chamado.Anexos != null ? (from obj in chamado.Anexos
                                                                where obj.Status == true
                                                                select new
                                                                {
                                                                    obj.Codigo,
                                                                    DescricaoAnexo = obj.Descricao,
                                                                    Arquivo = obj.NomeArquivo
                                                                }).ToList() : null,
                        chamado.Titulo,
                        Numero = chamado.Atendimento != null ? chamado.Atendimento.Numero : 0,
                        Solicitante = chamado.Solicitante != null ? new { chamado.Solicitante.Codigo, Descricao = chamado.Solicitante.Nome } : null,
                        ListaRespostas = chamado.Atendimento.Respostas != null ? (from obj in chamado.Atendimento.Respostas
                                                                                  where obj.Status == true
                                                                                  select new
                                                                                  {
                                                                                      obj.Codigo,
                                                                                      obj.Resposta,
                                                                                      CodigoFuncionario = obj.Funcionario != null ? obj.Funcionario.Codigo : 0,
                                                                                      Funcionario = obj.Funcionario != null ? obj.Funcionario.Nome : string.Empty,
                                                                                      DataHora = obj.DataHora.Value.ToString("dd/MM/yyyy HH:mm:ss")
                                                                                  }).ToList() : null
                    };

                    return new JsonpResult(retorno);
                }
                else
                    return new JsonpResult(false, "Chamado não encontrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<Servicos.DTO.CustomFile> files = HttpContext.GetFiles();

                if (files.Count <= 0)
                    return new JsonpResult(false, "Selecione um arquivo para envio.");

                Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo repChamadoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);

                int codigoChamado = 0;
                int.TryParse(Request.Params("CodigoChamado"), out codigoChamado);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado = repChamado.BuscarPorCodigo(codigoChamado);

                string caminhoSave = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().Anexos, "AnexoChamado");
                
                for (var i = 0; i < files.Count; i++)
                {
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo chamadoAnexo = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo();

                    string descricao = Request.Params("DescricaoAnexo");
                    Servicos.DTO.CustomFile file = files[i];

                    var nomeArquivo = file.FileName;
                    var guidArquivo = Guid.NewGuid().ToString().Replace("-", "");
                    var extensaoArquivo = System.IO.Path.GetExtension(nomeArquivo).ToLower();
                    string caminho = caminhoSave;

                    caminho = Utilidades.IO.FileStorageService.Storage.Combine(caminho, guidArquivo + extensaoArquivo);
                    file.SaveAs(caminho);

                    chamadoAnexo.CaminhoArquivo = caminho;
                    chamadoAnexo.NomeArquivo = Utilidades.String.RemoveAllSpecialCharacters(Utilidades.String.RemoveDiacritics(System.IO.Path.GetFileName(nomeArquivo)));
                    chamadoAnexo.Descricao = descricao;
                    chamadoAnexo.Status = true;
                    chamadoAnexo.AtendimentoTarefa = repChamado.BuscarPorCodigo(codigoChamado);

                    repChamadoAnexo.Inserir(chamadoAnexo);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Adicionou o anexo {chamadoAnexo.Descricao}.", unitOfWork);


                    unitOfWork.CommitChanges();
                   
                    //EnviarAnexosRedMine(chamadoAnexo);

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

        [AllowAuthenticate]
        public async Task<IActionResult> DownloadAnexo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAnexo = 0;
                int.TryParse(Request.Params("CodigoAnexo"), out codigoAnexo);

                Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo repChamadoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo chamadoAnexo = repChamadoAnexo.BuscarPorCodigo(codigoAnexo);

                if (chamadoAnexo == null)
                    return new JsonpResult(false, "Anexo não encontrado no Banco de Dados.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(chamadoAnexo.CaminhoArquivo))
                    return new JsonpResult(false, "Anexo não encontrado no Servidor.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(chamadoAnexo.CaminhoArquivo), "image/jpeg", chamadoAnexo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao obter o Anexo do Chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodosStatus()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<dynamic> retorno = new List<dynamic>();

                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Aberto, text = "Aberto" });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado, text = "Cancelado" });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Finalizado, text = "Finalizado" });

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void InativarAnexos(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo repChamadoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo(unidadeDeTrabalho);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {

                foreach (var anexo in listaAnexos)
                {

                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo chamadoAnexo = repChamadoAnexo.BuscarPorCodigo((int)anexo.Codigo);

                    chamadoAnexo.Status = false;
                    repChamadoAnexo.Atualizar(chamadoAnexo);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Removeu o anexo {chamadoAnexo.Descricao}.", unidadeDeTrabalho);
                }
                
            }
        }

        private void AdicionarRespostas(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento, Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoResposta repAtendimentoResposta = new Repositorio.Embarcador.Atendimento.AtendimentoResposta(unidadeDeTrabalho);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unidadeDeTrabalho);

            dynamic listaRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRespostasNovas"));
            if (listaRespostas.Count > 0)
            {
               
                foreach (var resposta in listaRespostas)
                {
                   
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta respostaAtendimento = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta();

                    respostaAtendimento.Resposta = (string)resposta.Resposta;
                    DateTime dataResposta;
                    DateTime.TryParse((string)resposta.DataHora, out dataResposta);
                    if (dataResposta > DateTime.MinValue)
                        respostaAtendimento.DataHora = dataResposta;
                    else
                        respostaAtendimento.DataHora = DateTime.Now;
                    respostaAtendimento.Status = true;
                    respostaAtendimento.Atendimento = atendimento;
                    respostaAtendimento.Funcionario = repFuncionario.BuscarPorCodigo((int)resposta.CodigoFuncionario);
                    repAtendimentoResposta.Inserir(respostaAtendimento);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Adicionou a resposta {respostaAtendimento.Resposta}.", unidadeDeTrabalho);

                }
            }
        }

        private void InativarRespostas(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoResposta repAtendimentoResposta = new Repositorio.Embarcador.Atendimento.AtendimentoResposta(unidadeDeTrabalho);

            dynamic listaRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRespostasExcluidas"));
            if (listaRespostas.Count > 0)
            {
                foreach (var resposta in listaRespostas)
                {
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta respostaAtendimento = repAtendimentoResposta.BuscarPorCodigo((int)resposta.Codigo);

                    respostaAtendimento.Status = false;
                    repAtendimentoResposta.Atualizar(respostaAtendimento);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, $"Removeu a resposta '{respostaAtendimento.Resposta}'.", unidadeDeTrabalho);
                }
            }
        }

        private void AtualizarChamadoNoAtendimento(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigo(chamado.Atendimento.Codigo);

            if (chamado.Status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado)
                atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado;
            else
                atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;
            atendimento.Prioridade = chamado.Prioridade;

            repAtendimento.Atualizar(atendimento);
            InserirLogAtendimento(atendimento, chamado, unidadeDeTrabalho);
        }

        private void InserirLogChamado(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado, Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoTarefaLog repChamadoLog = new Repositorio.Embarcador.Atendimento.AtendimentoTarefaLog(unidadeDeTrabalho);
            Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaLog chamadoLog = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaLog();

            chamadoLog.DataHora = DateTime.Now;
            chamadoLog.Status = chamado.Status;
            chamadoLog.Observacao = "Registro gerado ao Salvar o Chamado pelo Usuário: " + this.Usuario.Nome;
            chamadoLog.AtendimentoTarefa = repChamado.BuscarPorCodigo(chamado.Codigo);
            chamadoLog.EmpresaFilho = this.Usuario.Empresa;

            repChamadoLog.Inserir(chamadoLog);
        }

        private void InserirLogAtendimento(Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento, Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoLog repAtendimentoLog = new Repositorio.Embarcador.Atendimento.AtendimentoLog(unidadeDeTrabalho);
            Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Atendimento.AtendimentoLog atendimentoLog = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoLog();

            atendimentoLog.DataHora = DateTime.Now;
            atendimentoLog.Status = atendimento.Status;
            atendimentoLog.Observacao = "Registro gerado ao Salvar o Chamado de código: " + chamado.Codigo + " pelo Usuário: " + this.Usuario.Nome;
            atendimentoLog.Atendimento = repAtendimento.BuscarPorCodigo(atendimento.Codigo);
            atendimentoLog.EmpresaFilho = this.Usuario.Empresa;

            repAtendimentoLog.Inserir(atendimentoLog);
        }

        private void EnviarEmailChamado(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado, Repositorio.UnitOfWork unidadeDeTrabalho, bool insert = false)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoTarefa repChamado = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unidadeDeTrabalho);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unidadeDeTrabalho);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ||
                TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo(codigoEmpresa);

            string assunto = "";
            string mensagemEmail = "";
            string mensagemErro = "Erro ao enviar e-mail";

            if ((chamado.Atendimento.EmpresaFilho != null) || (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS))
            {
                if (email == null)
                    throw new Exception("Não há um e-mail configurado para realizar o envio.");

                if (insert)
                {
                    assunto = "Novo Chamado da Empresa " + chamado.Atendimento.EmpresaFilho.RazaoSocial + " Nº " + chamado.Atendimento.Numero.ToString("n0");
                    mensagemEmail = "Olá,<br/><br/>Novo Chamado aberto pela Empresa: " + chamado.Atendimento.EmpresaFilho.RazaoSocial + ".";
                    mensagemEmail += "<br/>Número do Chamado: " + chamado.Atendimento.Numero.ToString("n0");
                    mensagemEmail += "<br/>Usuário que abriu o chamado: " + this.Usuario.Nome + ".";
                    if (chamado.Solicitante != null)
                        mensagemEmail += "<br/>Solicitante do chamado: " + chamado.Solicitante.Nome + ".";
                    mensagemEmail += "<br/>Título/Descrição do Chamado: " + chamado.Titulo + ".";
                    mensagemEmail += "<br/><br/>Tela: " + chamado.AtendimentoTela?.Descricao + ".";
                    mensagemEmail += "<br/>Motivo/Problema Relatado: " + chamado.MotivoProblema + ".";
                    mensagemEmail += "<br/>Prioridade: " + Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimentoHelper.ObterDescricao(chamado.Prioridade) + ".";
                    mensagemEmail += "<br/><br/>Este chamado deve ser Concluído/Finalizado na tela de Atendimento.";
                }
                else
                {
                    assunto = "Chamado Cancelado da Empresa: " + chamado.Atendimento.EmpresaFilho.RazaoSocial;
                    mensagemEmail = "Olá,<br/><br/>Chamado de Motivo: " + chamado.MotivoProblema + " da Empresa: " + chamado.Atendimento.EmpresaFilho.RazaoSocial + " foi Cancelado.";
                }
                mensagemEmail += "<br/><br/>E-mail enviado automaticamente. Por favor, não responda.";

                List<string> emails = new List<string>();
                List<string> emailsCC = new List<string>();
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    //emailsCC.Add("rodrigo@multisoftware.com.br");
                    //emailsCC.Add("cesar@multisoftware.com.br");
                    //emailsCC.Add("infra@multisoftware.com.br");
                    emailsCC.Add("willian@multisoftware.com.br");
                    emailsCC.Add("daniel@multisoftware.com.br");
                    emailsCC.Add("suportetms@multisoftware.com.br");
                    if (!string.IsNullOrWhiteSpace(this.Usuario.Email))
                        emails.Add(this.Usuario.Email);
                    if (chamado.Atendimento.Funcionario != null)
                    {
                        if (!string.IsNullOrWhiteSpace(chamado.Atendimento.Funcionario.Email))
                            emails.Add(chamado.Atendimento.Funcionario.Email);
                        if (chamado.Atendimento.Funcionario.Empresa != null)
                            if (!string.IsNullOrWhiteSpace(chamado.Atendimento.Funcionario.Empresa.Email))
                                emails.Add(chamado.Atendimento.Funcionario.Empresa.Email);
                    }
                    else if (this.Empresa?.Email != null)
                    {
                        if (!string.IsNullOrWhiteSpace(this.Empresa.Email))
                            emails.Add(this.Empresa.Email);
                    }

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                        mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");

                    if (chamado.Atendimento.Empresa != null)
                    {
                        if (!string.IsNullOrWhiteSpace(chamado.Atendimento.Empresa.Email))
                            emails.Add(chamado.Atendimento.Empresa.Email);
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(chamado.Atendimento.EmpresaFilho.Email))
                            emails.Add(chamado.Atendimento.EmpresaFilho.Email);
                    }
                }

                if (emails.Count > 0)
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, null, emailsCC.ToArray(), emails.ToArray(), assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, null, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unidadeDeTrabalho, codigoEmpresa);
                    if (!sucesso)
                        throw new Exception("Problemas ao enviar o chamado por e-mail: " + mensagemErro);
                }
                else
                    throw new Exception("Empresa Pai que presta suporte não possui e-mail cadastrado.");
            }
        }

        private void EnviarChamadoAoRedMine(AtendimentoTarefa chamado, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine repConfiguracaoRedMine = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine configuracaoRedMine = repConfiguracaoRedMine.BuscarConfiguracaoPadrao();

            if (configuracaoRedMine != null && configuracaoRedMine.CodigoUsuarioDestino > 0 && !string.IsNullOrWhiteSpace(configuracaoRedMine.ClienteRedMine))
            {
                Servicos.Embarcador.Atendimento.RedMine.IntegracaoRedMine servicoIntegracaoRedMine = new Servicos.Embarcador.Atendimento.RedMine.IntegracaoRedMine(configuracaoRedMine, this.Usuario.Empresa.TipoAmbiente, unidadeDeTrabalho);

                if (chamado.Codigo > 0)
                    servicoIntegracaoRedMine.AtualizarTarefaRedMine(chamado);
                else
                    chamado.NumeroChamadoRedMine = servicoIntegracaoRedMine.CriarTarefaRedMine(chamado);
            }

        }

        private void EnviarAnexosRedMine(AtendimentoTarefaAnexo chamadoAnexo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine repConfiguracaoRedMine = new Repositorio.Embarcador.Configuracoes.ConfiguracaoRedMine(unidadeDeTrabalho);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoRedMine configuracaoRedMine = repConfiguracaoRedMine.BuscarConfiguracaoPadrao();

            if (configuracaoRedMine != null && configuracaoRedMine.CodigoUsuarioDestino > 0 && !string.IsNullOrWhiteSpace(configuracaoRedMine.ClienteRedMine))
            {
                Servicos.Embarcador.Atendimento.RedMine.IntegracaoRedMine servicoIntegracaoRedMine = new Servicos.Embarcador.Atendimento.RedMine.IntegracaoRedMine(configuracaoRedMine, this.Usuario.Empresa.TipoAmbiente, unidadeDeTrabalho);

                if (chamadoAnexo.AtendimentoTarefa.Codigo > 0)
                    servicoIntegracaoRedMine.EnviarAnexoTarefaRedMine(chamadoAnexo);
            }

        }

        #endregion
    }
}
