using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ObservacaoController : ApiController
    {

        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("observacoes.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult ConsultarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Dominio.Enumeradores.TipoObservacao tipo;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoObservacao>(Request.Params["Tipo"], out tipo))
                    tipo = Dominio.Enumeradores.TipoObservacao.Todos;
                var descricao = Request.Params["Descricao"];
                Repositorio.Observacao repObservacao = new Repositorio.Observacao(unitOfWork);

                var listaObservacao = repObservacao.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, descricao, inicioRegistros, 50, false, tipo);
                int countObservacao = repObservacao.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, descricao, false, tipo);

                var retorno = from obj in listaObservacao
                              select new
                              {
                                  obj.Tipo,
                                  obj.Log,
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.DescricaoTipoObservacao,
                                  UF = obj.UF != null ? obj.UF : string.Empty,
                                  UFFim = obj.UFDestino != null ? obj.UFDestino : string.Empty,
                                  obj.Status,
                              };

                return Json(retorno, true, null, new string[] { "Tipo", "Log", "Código|10", "Descrição|35", "Tipo|20", "UF|5",  "UF Destino|10",  "Status|10" }, countObservacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as observações! Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                Dominio.Enumeradores.TipoObservacao tipo;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoObservacao>(Request.Params["Tipo"], out tipo))
                    tipo = Dominio.Enumeradores.TipoObservacao.Todos;
                var descricao = Request.Params["Descricao"];
                Repositorio.Observacao repObservacao = new Repositorio.Observacao(unitOfWork);

                var listaObservacao = repObservacao.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, descricao, inicioRegistros, 50, null, tipo);
                int countObservacao = repObservacao.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.EmpresaPai != null ? this.EmpresaUsuario.EmpresaPai.Codigo : 0, descricao, null, tipo);

                var retorno = from obj in listaObservacao
                              select new
                              {
                                  obj.Tipo,
                                  obj.Log,
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.DescricaoTipoObservacao,
                                  UF = obj.UF != null ? obj.UF : string.Empty,
                                  UFFim = obj.UFDestino != null ? obj.UFDestino : string.Empty,
                                  CST = obj.CST != null ? obj.CST : string.Empty,
                                  obj.TipoCTe,
                                  obj.Operacao,
                                  obj.Status,
                                  obj.Automatica
                              };

                return Json(retorno, true, null, new string[] { "Tipo", "Log", "Código|10", "Descrição|35", "Tipo|20", "UF|5", "UF Destino|10", "CST", "Tipo CTe", "Operacao", "Status|10", "Automatica" }, countObservacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as observações! Atualize a página e tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string descricao = Request.Params["Descricao"];
                if (string.IsNullOrWhiteSpace(descricao))
                    return Json<bool>(false, false, "Descrição inválida!");
                Dominio.Enumeradores.TipoObservacao tipo;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoObservacao>(Request.Params["Tipo"], out tipo))
                    return Json<bool>(false, false, "Tipo de observação inválido!");
                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Dominio.Enumeradores.TipoCTE tipoCTe;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["TipoCTe"], out tipoCTe))
                    return Json<bool>(false, false, "Tipo de CTe inválido!");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao operacao;
                if (!Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao>(Request.Params["Operacao"], out operacao))
                    return Json<bool>(false, false, "Tipo de observação inválido!");

                string status = Request.Params["Status"];
                string cst = Request.Params["CST"];
                string ufInicio = Request.Params["UfInicio"];
                string ufFim = Request.Params["UfFim"];

                bool.TryParse(Request.Params["Automatica"], out bool automatica);

                Repositorio.Observacao repObservacao = new Repositorio.Observacao(unitOfWork);
                Dominio.Entidades.Observacao observacao;

                string log = "";

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração de observação negada!");
                    observacao = repObservacao.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);
                    if (observacao == null)
                        return Json<bool>(true, false, "Observação não encontrada! Atualize a página e tente novamente.");

                    if (operacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao.Todos)
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorOperacaoICMS(this.Usuario.Empresa.Codigo, tipo, tipoCTe, operacao, cst, automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para para esta Operação e Tributação.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufInicio) && !string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, ufInicio, ufFim, automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para estes Estados e esta Tributação.");
                        }
                    }
                    else if(!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufInicio))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, ufInicio, "", automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Origem e esta Tributação.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, "", ufFim, automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Destino e esta Tributação.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(ufInicio) && !string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", ufInicio, ufFim, automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para estes Estados.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(cst))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, "","", automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para esta Tributação.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(ufInicio))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", ufInicio, "", automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Origem.");
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", "", ufFim, automatica);
                        foreach (Dominio.Entidades.Observacao obsValidar in listaObs)
                        {
                            if (obsValidar.Codigo != codigo)
                                return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Destino.");
                        }
                    }

                    if (observacao.Status != status)
                    {
                        if (status == "I")
                            log = "Status Inativo.";
                        else
                            log = "Status Ativo.";
                    }

                    if (observacao.Tipo != tipo)
                    {
                        string descricaoTipo = "";

                        if (tipo == Dominio.Enumeradores.TipoObservacao.Fisco)
                            descricaoTipo = "Fisco";
                        else if (tipo == Dominio.Enumeradores.TipoObservacao.Contribuinte)
                            descricaoTipo = "Contribuinte";
                        else if (tipo == Dominio.Enumeradores.TipoObservacao.Geral)
                            descricaoTipo = "Geral";

                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "Tipo Obs. de " + observacao.DescricaoTipoObservacao + " para " + descricaoTipo + ". ");
                        else
                            log = "Tipo Obs. de " + observacao.DescricaoTipoObservacao + " para " + descricaoTipo + ". ";
                    }

                    if (observacao.TipoCTe != tipoCTe)
                    {
                        string descricaoTipoCTe = "";

                        if (tipoCTe == Dominio.Enumeradores.TipoCTE.Normal)
                            descricaoTipoCTe = "Normal";
                        else if (tipoCTe == Dominio.Enumeradores.TipoCTE.Complemento)
                            descricaoTipoCTe = "Complemento";
                        else if (tipoCTe == Dominio.Enumeradores.TipoCTE.Anulacao)
                            descricaoTipoCTe = "Anulação";
                        else if (tipoCTe == Dominio.Enumeradores.TipoCTE.Substituto)
                            descricaoTipoCTe = "Substituto";

                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "Tipo CTe de " + observacao.DescricaoTipoCTe + " para " + descricaoTipoCTe + ". ");
                        else
                            log = "Tipo CTe de " + observacao.DescricaoTipoObservacao + " para " + descricaoTipoCTe + ". ";
                    }

                    if (observacao.Operacao != operacao)
                    {
                        string descricaoOperacao = string.Empty;

                        if (operacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao.Todos)
                            descricaoOperacao = "Todos";
                        else if (operacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao.Interestadual)
                            descricaoOperacao = "Interestadual";
                        else if (operacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.Operacao.Intraestadual)
                            descricaoOperacao = "Intraestadual";

                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "Observação de " + observacao.DescricaoOperacao + " para " + descricaoOperacao + ". ");
                        else
                            log = "Observação de " + observacao.DescricaoOperacao + " para " + descricaoOperacao + ". ";
                    }

                    var cstAnterior = observacao.CST != null ? observacao.CST : "";
                    if (cstAnterior != cst)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "CST de " + (string.IsNullOrWhiteSpace(observacao.CST) ? "Todas" : observacao.CST) + " para " + cst + ". ");
                        else
                            log = "CST de " + (string.IsNullOrWhiteSpace(observacao.CST) ? "Todas" : observacao.CST) + " para " + cst + ". ";
                    }

                    var ufAnterior = observacao.UF != null ? observacao.UF : "";
                    if (ufAnterior != ufInicio)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "Estado de " + (string.IsNullOrWhiteSpace(observacao.UF) ? "Todos" : observacao.UF) + " para " + ufInicio + ". ");
                        else
                            log = "Estado de " + (string.IsNullOrWhiteSpace(observacao.UF) ? "Todos" : observacao.UF) + " para " + ufInicio + ". ";
                    }

                    if (observacao.Descricao != descricao)
                    {
                        if (!string.IsNullOrWhiteSpace(log))
                            log = string.Concat(log, " ", "Mensagem de " + observacao.Descricao + " para " + descricao + ". ");
                        else
                            log = "Mensagem de " + observacao.Descricao + " para " + descricao + ". ";
                    }

                    if (!string.IsNullOrWhiteSpace(log))
                    {
                        if (this.UsuarioAdministrativo != null)
                            log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, ": ", log);
                        else
                            log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, ": ", log);
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufInicio) && !string.IsNullOrWhiteSpace(ufFim) )
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, ufInicio, ufFim, automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para estes Estados e esta CST.");
                    }
                    else if (!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufInicio))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, ufInicio, "", automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Origem e esta CST.");
                    }
                    else if (!string.IsNullOrWhiteSpace(cst) && !string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, "", ufFim, automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Destino e esta CST.");
                    }
                    else if (!string.IsNullOrWhiteSpace(ufInicio) && !string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", ufInicio, ufFim, automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para estes Estados.");
                    }
                    else if (!string.IsNullOrWhiteSpace(cst))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, cst, "","", automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para esta CST.");
                    }
                    else if (!string.IsNullOrWhiteSpace(ufInicio))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", ufInicio, "", automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado.");
                    }
                    else if (!string.IsNullOrWhiteSpace(ufFim))
                    {
                        List<Dominio.Entidades.Observacao> listaObs = new List<Dominio.Entidades.Observacao>();
                        listaObs = repObservacao.BuscarPorUFeCST(this.Usuario.Empresa.Codigo, tipo, tipoCTe, "", "", ufFim, automatica);
                        if (listaObs.Count() > 0)
                            return Json<bool>(true, false, "Já existe observação deste Tipo para este Estado Destino.");
                    }

                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão de observação negada!");
                    observacao = new Dominio.Entidades.Observacao();
                    observacao.DataCadastro = DateTime.Now;

                    if (this.UsuarioAdministrativo != null)
                        log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, ".");
                    else
                        log = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Inserido por ", this.Usuario.CPF, " - ", this.Usuario.Nome, ".");
                }

                observacao.Descricao = descricao;
                observacao.Status = status;
                observacao.Tipo = tipo;
                observacao.Operacao = operacao;
                observacao.TipoCTe = tipoCTe;
                observacao.Automatica = automatica;
                observacao.Empresa = this.EmpresaUsuario;
                if (!string.IsNullOrWhiteSpace(cst))
                    observacao.CST = cst;
                else
                    observacao.CST = null;
                if (!string.IsNullOrWhiteSpace(ufInicio))
                    observacao.UF = ufInicio;
                else
                    observacao.UF = null;
                if (!string.IsNullOrWhiteSpace(ufFim))
                    observacao.UFDestino = ufFim;
                else
                    observacao.UFDestino = null;

                if (!string.IsNullOrWhiteSpace(log))
                {
                    if (string.IsNullOrWhiteSpace(observacao.Log))
                        observacao.Log = log;
                    else
                        observacao.Log = string.Concat(observacao.Log, "\n", log);
                }

                if (codigo > 0)
                    repObservacao.Atualizar(observacao);
                else
                    repObservacao.Inserir(observacao);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a observação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para exclusão de observação negada!");

                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Observacao repObservacao = new Repositorio.Observacao(unitOfWork);
                Dominio.Entidades.Observacao observacao = repObservacao.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (observacao == null)
                    return Json<bool>(true, false, "Observação não encontrada! Atualize a página e tente novamente.");
                else
                    repObservacao.Deletar(observacao);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao excluir a observação!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
