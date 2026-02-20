using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SGT.WebAdmin.Controllers.Atendimentos
{
    [CustomAuthorize("Atendimentos/Atendimento")]
    public class AtendimentoController : BaseController
    {
		#region Construtores

		public AtendimentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);

                int codigoFuncionario, numeroInicial, numeroFinal;
                int.TryParse(Request.Params("Funcionario"), out codigoFuncionario);
                int.TryParse(Request.Params("NumeroInicial"), out numeroInicial);
                int.TryParse(Request.Params("NumeroFinal"), out numeroFinal);
                int empresaFilhio;
                int.TryParse(Request.Params("Empresa"), out empresaFilhio);

                string motivo = Request.Params("MotivoProblema");
                string titulo = Request.Params("Titulo");

                DateTime dataInicial, dataFinal;
                DateTime.TryParse(Request.Params("DataInicial"), out dataInicial);
                DateTime.TryParse(Request.Params("DataFinal"), out dataFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusAberto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusEmAndamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusCancelado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento statusFinalizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Todos;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade;
                Enum.TryParse(Request.Params("Prioridade"), out prioridade);

                List<int> status = JsonConvert.DeserializeObject<List<int>>(Request.Params("Status"));
                for (int i = 0; i < status.Count(); i++)
                {
                    if (status[i] == 1)
                        statusAberto = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;
                    else if (status[i] == 2)
                        statusEmAndamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento;
                    else if (status[i] == 3)
                        statusCancelado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado;
                    else if (status[i] == 4)
                        statusFinalizado = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado;
                }

                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    empresa = this.Usuario.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Empresa/Cliente", "Cliente", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Prioridade", "DescricaoPrioridade", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Título", "Titulos", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Telas", "Telas", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Módulos", "Modulos", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motivos", "Motivos", 30, Models.Grid.Align.left, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "DescricaoPrioridade")
                    propOrdenar = "Prioridade";
                else if (propOrdenar == "DescricaoStatus")
                    propOrdenar = "Status";
                //status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto;

                List<Dominio.Entidades.Embarcador.Atendimento.Atendimento> listaAtendimento = repAtendimento.Consultar(TipoServicoMultisoftware, numeroInicial, numeroFinal, dataInicial, dataFinal, statusAberto, statusEmAndamento, statusCancelado, statusFinalizado, prioridade, empresaFilhio, codigoFuncionario, empresa, motivo, titulo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repAtendimento.ContarConsulta(TipoServicoMultisoftware, numeroInicial, numeroFinal, dataInicial, dataFinal, statusAberto, statusEmAndamento, statusCancelado, statusFinalizado, prioridade, empresaFilhio, codigoFuncionario, empresa, motivo, titulo));
                var lista = (from p in listaAtendimento
                             select new
                             {
                                 p.Codigo,
                                 p.Numero,
                                 Cliente = p.EmpresaFilho != null ? p.EmpresaFilho.RazaoSocial : string.Empty,
                                 DescricaoPrioridade = Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimentoHelper.ObterDescricao(p.Prioridade),
                                 p.DescricaoStatus,
                                 p.Titulos,
                                 p.Telas,//Telas = p.Telas != null ? p.Telas : string.Empty,
                                 p.Modulos,//Modulos = p.Modulos != null ? p.Modulos : string.Empty,
                                 p.Motivos//Motivos = p.Motivos != null ? p.Motivos : string.Empty
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

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repAtendimentoTarefa = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTipo repAtendimentoTipo = new Repositorio.Embarcador.Atendimento.AtendimentoTipo(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                int codigo, codigoFuncionario, codigoEmpresa, codigoTarefa, codigoTela, codigoModulo, codigoSistema, codigoTipoAtendimento = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);
                int.TryParse(Request.Params("Funcionario"), out codigoFuncionario);
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("CodigoTarefa"), out codigoTarefa);
                int.TryParse(Request.Params("Tela"), out codigoTela);
                int.TryParse(Request.Params("Modulo"), out codigoModulo);
                int.TryParse(Request.Params("Sistema"), out codigoSistema);
                int.TryParse(Request.Params("TipoAtendimento"), out codigoTipoAtendimento);

                DateTime dataChamado, dataAtendimento;
                DateTime.TryParse(Request.Params("DataAberturaChamado"), out dataChamado);
                DateTime.TryParse(Request.Params("DataAtendimento"), out dataAtendimento);

                string pessoaContato = Request.Params("PessoaContato");
                string obseracaoSuporte = Request.Params("ObservacaoSuporte");
                string motivoChamado = Request.Params("MotivoChamado");
                string solucaoChamado = Request.Params("SolucaoChamado");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa status;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento prioridade;
                Enum.TryParse(Request.Params("StatusChamado"), out status);
                Enum.TryParse(Request.Params("PrioridadeChamado"), out prioridade);

                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa tarefa = repAtendimentoTarefa.BuscarPorCodigo(codigoTarefa);

                if (tarefa == null || atendimento == null)
                    return new JsonpResult(false, "Favor selecione um chamado/atendimento para atualizar.");

                atendimento.DataInicial = dataAtendimento;
                atendimento.Funcionario = repUsuario.BuscarPorCodigo(codigoFuncionario);
                atendimento.ObservacaoSuporte = obseracaoSuporte;
                atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento;
                atendimento.ContatoAtendimento = pessoaContato;

                tarefa.Status = status;
                tarefa.AtendimentoTipo = repAtendimentoTipo.BuscarPorCodigo(codigoTipoAtendimento);
                if (!string.IsNullOrWhiteSpace(tarefa.JustificativaObservacao))
                    tarefa.JustificativaObservacao += " " + solucaoChamado;
                else
                    tarefa.JustificativaObservacao = solucaoChamado;

                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Finalizado)
                    atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado;
                else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Cancelado)
                    atendimento.Status = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado;

                InativarAnexos(unitOfWork);
                InativarRespostas(unitOfWork);
                AdicionarRespostas(unitOfWork, atendimento);

                EnviarEmailChamado(tarefa, unitOfWork);

                repAtendimento.Atualizar(atendimento);
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
                Repositorio.Embarcador.Atendimento.Atendimento repAtendimento = new Repositorio.Embarcador.Atendimento.Atendimento(unitOfWork);
                Repositorio.Embarcador.Atendimento.AtendimentoTarefa repAtendimentoTarefa = new Repositorio.Embarcador.Atendimento.AtendimentoTarefa(unitOfWork);
                Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento = repAtendimento.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa tarefa = repAtendimentoTarefa.BuscarPorAtendimento(codigo);
                var dynProcessoMovimento = new
                {
                    atendimento.Codigo,
                    DataAberturaChamado = tarefa != null ? tarefa.Data.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    DataAtendimento = atendimento.DataInicial != null ? atendimento.DataInicial.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                    atendimento.Numero,
                    Funcionario = atendimento.Funcionario != null ? new { Codigo = atendimento.Funcionario.Codigo, Descricao = atendimento.Funcionario.Nome } : null,
                    Empresa = atendimento.EmpresaFilho != null ? new { Codigo = atendimento.EmpresaFilho.Codigo, Descricao = atendimento.EmpresaFilho.RazaoSocial } : null,
                    PessoaContato = atendimento.ContatoAtendimento,
                    atendimento.ObservacaoSuporte,
                    CodigoTarefa = tarefa != null ? tarefa.Codigo : 0,
                    Tela = tarefa != null && tarefa.AtendimentoTela != null ? tarefa.AtendimentoTela != null ? new { Codigo = tarefa.AtendimentoTela.Codigo, Descricao = tarefa.AtendimentoTela.Descricao } : null : null,
                    Modulo = tarefa != null && tarefa.AtendimentoModulo != null ? tarefa.AtendimentoModulo != null ? new { Codigo = tarefa.AtendimentoModulo.Codigo, Descricao = tarefa.AtendimentoModulo.Descricao } : null : null,
                    Sistema = tarefa != null && tarefa.AtendimentoSistema != null ? tarefa.AtendimentoSistema != null ? new { Codigo = tarefa.AtendimentoSistema.Codigo, Descricao = tarefa.AtendimentoSistema.Descricao } : null : null,
                    TituloChamado = tarefa != null ? tarefa.Titulo : string.Empty,
                    TipoAtendimento = tarefa != null && tarefa.AtendimentoTipo != null ? tarefa.AtendimentoTipo != null ? new { Codigo = tarefa.AtendimentoTipo.Codigo, Descricao = tarefa.AtendimentoTipo.Descricao } : null : null,
                    StatusChamado = tarefa != null ? tarefa.Status : Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimentoTarefa.Aberto,
                    PrioridadeChamado = tarefa != null ? tarefa.Prioridade : Dominio.ObjetosDeValor.Embarcador.Enumeradores.PrioridadeAtendimento.Normal,
                    MotivoChamado = tarefa != null ? tarefa.MotivoProblema : string.Empty,
                    SolucaoChamado = tarefa != null ? tarefa.JustificativaObservacao : string.Empty,
                    Solicitante = tarefa != null && tarefa.Solicitante != null ? tarefa.Solicitante != null ? new { tarefa.Solicitante.Codigo, Descricao = tarefa.Solicitante.Nome } : null : null,
                    ListaAnexos = tarefa != null && tarefa.Anexos != null ? (from obj in tarefa.Anexos
                                                                             where obj.Status == true
                                                                             select new
                                                                             {
                                                                                 obj.Codigo,
                                                                                 DescricaoAnexo = obj.Descricao,
                                                                                 Arquivo = obj.NomeArquivo
                                                                             }).ToList() : null,
                    ListaRespostas = atendimento != null && atendimento.Respostas != null ? (from obj in atendimento.Respostas
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
                return new JsonpResult(dynProcessoMovimento);
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

                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Aberto, text = "Aberto" });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.EmAndamento, text = "Em Andamento" });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Cancelado, text = "Cancelado" });
                retorno.Insert(0, new { value = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusAtendimento.Finalizado, text = "Finalizado" });

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

        private void InativarAnexos(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo repChamadoAnexo = new Repositorio.Embarcador.Atendimento.AtendimentoTarefaAnexo(unidadeDeTrabalho);

            dynamic listaAnexos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaAnexosExcluidos"));
            if (listaAnexos.Count > 0)
            {
                foreach (var anexo in listaAnexos)
                {
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefaAnexo chamadoAnexo = repChamadoAnexo.BuscarPorCodigo((int)anexo.Codigo.val);

                    chamadoAnexo.Status = false;
                    repChamadoAnexo.Atualizar(chamadoAnexo);
                }
            }
        }

        private void AdicionarRespostas(Repositorio.UnitOfWork unidadeDeTrabalho, Dominio.Entidades.Embarcador.Atendimento.Atendimento atendimento)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoResposta repAtendimentoResposta = new Repositorio.Embarcador.Atendimento.AtendimentoResposta(unidadeDeTrabalho);
            Repositorio.Usuario repFuncionario = new Repositorio.Usuario(unidadeDeTrabalho);

            dynamic listaRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRespostasNovas"));
            if (listaRespostas.Count > 0)
            {
                foreach (var resposta in listaRespostas)
                {
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta respostaAtendimento = new Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta();

                    respostaAtendimento.Resposta = (string)resposta.Resposta.val;
                    DateTime dataResposta;
                    DateTime.TryParse((string)resposta.DataHora.val, out dataResposta);
                    if (dataResposta > DateTime.MinValue)
                        respostaAtendimento.DataHora = dataResposta;
                    else
                        respostaAtendimento.DataHora = DateTime.Now;
                    respostaAtendimento.Status = true;
                    respostaAtendimento.Atendimento = atendimento;
                    respostaAtendimento.Funcionario = repFuncionario.BuscarPorCodigo((int)resposta.CodigoFuncionario.val);
                    repAtendimentoResposta.Inserir(respostaAtendimento);
                }
            }
        }

        private void InativarRespostas(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Atendimento.AtendimentoResposta repAtendimentoResposta = new Repositorio.Embarcador.Atendimento.AtendimentoResposta(unidadeDeTrabalho);

            dynamic listaRespostas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaRespostasExcluidas"));
            if (listaRespostas.Count > 0)
            {
                foreach (var resposta in listaRespostas)
                {
                    Dominio.Entidades.Embarcador.Atendimento.AtendimentoResposta respostaAtendimento = repAtendimentoResposta.BuscarPorCodigo((int)resposta.Codigo.val);

                    respostaAtendimento.Status = false;
                    repAtendimentoResposta.Atualizar(respostaAtendimento);
                }
            }
        }

        private void EnviarEmailChamado(Dominio.Entidades.Embarcador.Atendimento.AtendimentoTarefa chamado, Repositorio.UnitOfWork unidadeDeTrabalho)
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

                assunto = "Atualização do Chamado: " + chamado.Atendimento.EmpresaFilho.RazaoSocial + " Nº " + chamado.Atendimento.Numero.ToString("n0");
                mensagemEmail = "Olá,<br/><br/>Chamado de Motivo: " + chamado.MotivoProblema + " da Empresa: " + chamado.Atendimento.EmpresaFilho.RazaoSocial;
                mensagemEmail += "<br/>Número do Chamado: " + chamado.Atendimento.Numero.ToString("n0");
                mensagemEmail += "<br/>Usuário que respondeu o chamado: " + this.Usuario.Nome + ".";
                if (chamado.Solicitante != null)
                    mensagemEmail += "<br/>Solicitante do chamado: " + chamado.Solicitante.Nome + ".";
                mensagemEmail += "<br/>Tela: " + chamado.AtendimentoTela?.Descricao + ".";
                if (!string.IsNullOrWhiteSpace(chamado.JustificativaObservacao))
                    mensagemEmail += "<br/>Solução aplicada: " + chamado.JustificativaObservacao;
                if (!string.IsNullOrWhiteSpace(chamado.Atendimento.ObservacaoSuporte))
                    mensagemEmail += "<br/>Observação do suporte: " + chamado.Atendimento.ObservacaoSuporte;
                mensagemEmail += "<br/>Status do chamado: " + chamado.DescricaoStatus;
                mensagemEmail += "<br/>Status do atendimento: " + chamado.Atendimento.DescricaoStatus;

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
                    else if (!string.IsNullOrWhiteSpace(this.Empresa.Email))
                    {
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

        #endregion
    }
}
