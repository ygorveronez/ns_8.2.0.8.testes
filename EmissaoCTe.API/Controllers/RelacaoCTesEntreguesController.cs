using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web.Mvc;
using Servicos;
using Microsoft.Reporting.WebForms;

namespace EmissaoCTe.API.Controllers
{
    public class RelacaoCTesEntreguesController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao(string arquivo = null)
        {
            if (string.IsNullOrWhiteSpace(arquivo))
                arquivo = "relacaoctesentregues.aspx";
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals(arquivo) select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Filtros
                int.TryParse(Request.Params["NumeroInicial"], out int numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out int numeroFinal);
                int.TryParse(Request.Params["NumeroCTe"], out int numeroCTe);

                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                double.TryParse(Request.Params["Cliente"], out double cliente);

                Dominio.Enumeradores.StatusRelacaoCTesEntregues? status = null;
                if (Enum.TryParse(Request.Params["Status"], out Dominio.Enumeradores.StatusRelacaoCTesEntregues statusAux))
                    status = statusAux;

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string numeroControle = Request.Params["NumeroControle"] ?? string.Empty;

                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);

                Repositorio.RelacaoCTesEntregues repRelacaoCTesEntregues = new Repositorio.RelacaoCTesEntregues(unitOfWork);

                List<Dominio.Entidades.RelacaoCTesEntregues> lista = repRelacaoCTesEntregues.Consultar(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, cliente, status, numeroCTe, descricao, numeroControle, inicioRegistros, 50);
                int totalRegistro = repRelacaoCTesEntregues.ContarConsulta(this.EmpresaUsuario.Codigo, numeroInicial, numeroFinal, dataInicial, dataFinal, cliente, status, numeroCTe, descricao, numeroControle);

                var retorno = (from obj in lista select
                              new {
                                  obj.Codigo,
                                  obj.Numero,
                                  DataCriacao = obj.DataBipagem.ToString("dd/MM/yyyy"),
                                  Status = obj.DescricaoStatus,
                                  Cliente = obj.Cliente.Nome
                              }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|15", "Data Criação|15", "Status|15", "Cliente|45" }, totalRegistro);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as relações de entregas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorio
                Repositorio.RelacaoCTesEntregues repRelacaoCTesEntregues = new Repositorio.RelacaoCTesEntregues(unitOfWork);
                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);

                // Covnerte parametro
                int.TryParse(Request.Params["Codigo"], out int codigo);

                // Busca dados
                Dominio.Entidades.RelacaoCTesEntregues relacao = repRelacaoCTesEntregues.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                // Valida
                if (relacao == null)
                    return Json<bool>(false, false, "Ocorreu uma falha ao buscar dados.");

                double emissor = 0;
                //if (relacao.CTes != null && relacao.CTes.Count > 0)
                //    double.TryParse(relacao.CTes.FirstOrDefault().CTe.Intermediario.CPF_CNPJ, out emissor);

                Dominio.Entidades.CalculoRelacaoCTesEntregues calculoRelacaoCTesEntregues = repCalculoRelacaoCTesEntregues.BuscarPorEmpresaECliente(this.EmpresaUsuario.Codigo, relacao.Cliente.CPF_CNPJ, emissor);

                var retorno = new
                {
                    relacao.Codigo,
                    relacao.Numero,
                    DataRelacao = relacao.DataBipagem.ToString("dd/MM/yyyy"),
                    DataEntrega = relacao.DataEntrega.ToString("dd/MM/yyyy"),
                    Cliente = new { relacao.Cliente.Codigo, Descricao = relacao.Cliente.Nome },
                    relacao.KmInicial,
                    relacao.KmFinal,
                    relacao.Descricao,
                    relacao.Observacao,
                    relacao.NumeroControle,
                    relacao.Status,

                    Diaria = relacao.Diaria.ToString("n2"),
                    ValorAcrescimos = relacao.ValorAcrescimos.ToString("n2"),
                    ValorDescontos = relacao.ValorDescontos.ToString("n2"),
                    ValorTotal = relacao.ValorTotal.ToString("n2"),
                    relacao.TipoDiaria,

                    CalculoRelacaoCTesEntregues = calculoRelacaoCTesEntregues != null ? calculoRelacaoCTesEntregues.ObjetoCalculo() : null,

                    CTes = (from cte in relacao.CTes select FormatoCTe(cte.CTe, cte.Codigo, cte.Ordem, unitOfWork)).ToList(),
                    Coletas = (from coleta in relacao.Coletas select new {
                        Id = coleta.Codigo,
                        Coleta = coleta.Descricao,
                        coleta.Peso,
                        coleta.ValorEvento,
                        coleta.ValorFracao,
                        Excluir = false
                    }).ToList(),
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da relação.");
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
                // Repositorios
                Repositorio.RelacaoCTesEntregues repRelacaoCTesEntregues = new Repositorio.RelacaoCTesEntregues(unitOfWork);
                Repositorio.CalculoRelacaoCTesEntregues repCalculoRelacaoCTesEntregues = new Repositorio.CalculoRelacaoCTesEntregues(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                int.TryParse(Request.Params["Codigo"], out int codigo);
                int.TryParse(Request.Params["KmInicial"], out int kmInicial);
                int.TryParse(Request.Params["KmFinal"], out int kmFinal);

                bool.TryParse(Request.Params["Finalizar"], out bool finalizar);

                double.TryParse(Request.Params["Cliente"], out double cpfcnpjCliente);

                decimal.TryParse(Request.Params["Diaria"], out decimal diaria);
                decimal.TryParse(Request.Params["ValorAcrescimos"], out decimal valorAcrescimos);
                decimal.TryParse(Request.Params["ValorDescontos"], out decimal valorDescontos);
                decimal.TryParse(Request.Params["ValorTotal"], out decimal valorTotal);

                Enum.TryParse(Request.Params["TipoDiaria"], out Dominio.Enumeradores.TipoDiariaRelacaoCTesEntregues tipoDiaria);

                DateTime.TryParseExact(Request.Params["DataRelacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataRelacao);
                DateTime.TryParseExact(Request.Params["DataEntrega"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEntrega);

                string descricao = Request.Params["Descricao"] ?? string.Empty;
                string observacao = Request.Params["Observacao"] ?? string.Empty;
                string numeroControle = Request.Params["NumeroControle"] ?? string.Empty;

                if (dataRelacao == DateTime.MinValue)
                    return Json<bool>(false, false, "Data Bipagem inválida.");

                if (dataEntrega == DateTime.MinValue)
                    return Json<bool>(false, false, "Data Entrega inválida.");
                
                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfcnpjCliente);
                if (cliente == null)
                    return Json<bool>(false, false, "Cliente é obrigatório.");

                // Inicia transition
                unitOfWork.Start();

                // Busca entidade
                Dominio.Entidades.RelacaoCTesEntregues relacao = repRelacaoCTesEntregues.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (relacao == null)
                {
                    relacao = new Dominio.Entidades.RelacaoCTesEntregues
                    {
                        Empresa = this.EmpresaUsuario,
                        Status = Dominio.Enumeradores.StatusRelacaoCTesEntregues.Aberto,
                        Numero = repRelacaoCTesEntregues.UltimoNumero(this.EmpresaUsuario.Codigo) + 1
                    };
                }
                else
                {
                    if (relacao.Status == Dominio.Enumeradores.StatusRelacaoCTesEntregues.Fechado)
                        return Json<bool>(false, false, "O status não permite alterações.");
                }

                // Preenche a entidade
                relacao.KmInicial = kmInicial;
                relacao.KmFinal = kmFinal;
                relacao.DataBipagem = dataRelacao;
                relacao.DataEntrega = dataEntrega;
                relacao.Cliente = cliente;
                relacao.Descricao = descricao;
                relacao.Observacao = observacao;
                relacao.NumeroControle = numeroControle;

                relacao.Diaria = diaria;
                relacao.ValorAcrescimos = valorAcrescimos;
                relacao.ValorDescontos = valorDescontos;
                relacao.ValorTotal = valorTotal;
                relacao.TipoDiaria = tipoDiaria;

                if (finalizar && relacao.Codigo > 0)
                    relacao.Status = Dominio.Enumeradores.StatusRelacaoCTesEntregues.Fechado;

                if (relacao.Codigo > 0)
                    repRelacaoCTesEntregues.Atualizar(relacao);
                else
                    repRelacaoCTesEntregues.Inserir(relacao);

                this.SalvarCTes(ref relacao, unitOfWork);
                double emissor = 0;
                //if (relacao.CTes != null && relacao.CTes.Count > 0)
                //    double.TryParse(relacao.CTes.FirstOrDefault().CTe.Intermediario.CPF_CNPJ, out emissor);

                Dominio.Entidades.CalculoRelacaoCTesEntregues calculo = repCalculoRelacaoCTesEntregues.BuscarPorEmpresaECliente(this.EmpresaUsuario.Codigo, cpfcnpjCliente, emissor);
                this.SalvarColetas(ref relacao, calculo, unitOfWork);
                //this.CalcularTotal(ref relacao, calculo, unitOfWork);

                unitOfWork.CommitChanges();

                return Json(new
                {
                    Codigo = relacao.Codigo,
                    Numero = relacao.Numero
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a relação de CT-es emitido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarDadosPorChave()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                // Repositorios e servicos
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);

                // Chave do CTe
                string chave = Request.Params["Chave"] ?? string.Empty;

                // Valida
                if (!serDocumento.ValidarChave(chave))
                    return Json<bool>(false, false, "A chave do CT-e é inválida.");

                // Busca por chave
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorChave(this.EmpresaUsuario.Codigo, chave);

                return RetornoConsultaCTe(cte, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTeeSefaz()
        {
            try
            {
                // Servico
                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento();

                // Converte parametros
                string chave = Request.Params["Chave"] ?? string.Empty;

                // Valida chave
                if (serDocumento.ValidarChave(chave))
                {
                    ConsultaCTe.CosultaCTeClient consultaCTe = new ConsultaCTe.CosultaCTeClient();
                    OperationContextScope scope = new OperationContextScope(consultaCTe.InnerChannel);
                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                    Servicos.ServicoConsultaCTe.RetornoOfRequisicaoSefazpIzbOyUQ requisicaoSefaz = consultaCTe.SolicitarRequisicaoSefaz();

                    if (requisicaoSefaz.Status)
                    {
                        var retorno = new
                        {
                            TipoConsultaPortalFazenda = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConsultaPortalFazenda.CTe,
                            DadosConsultar = new
                            {
                                VIEWSTATE = requisicaoSefaz.Objeto.VIEWSTATE,
                                EVENTVALIDATION = requisicaoSefaz.Objeto.EVENTVALIDATION,
                                imgCaptcha = requisicaoSefaz.Objeto.Captcha,
                                token = requisicaoSefaz.Objeto.TokenCaptcha,
                                SessionID = requisicaoSefaz.Objeto.SessionID
                            }
                        };
                        return Json(retorno, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, requisicaoSefaz.Mensagem);
                    }
                }
                else
                {
                    return Json<bool>(false, false, "A chave do CT-e é inválida.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar a NF-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InformarCaptchaCTeSefaz()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                // Prepara requisição para o web service
                ConsultaCTe.CosultaCTeClient consultaCTe = new ConsultaCTe.CosultaCTeClient();
                OperationContextScope scope = new OperationContextScope(consultaCTe.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                // Converte dados do estado da requisição
                string VIEWSTATE = Request.Params["VIEWSTATE"];
                string EVENTVALIDATION = Request.Params["EVENTVALIDATION"];
                string TokenCaptcha = Request.Params["token"];
                string SessionID = Request.Params["SessionID"];
                string chave = Request.Params["Chave"];
                string captcha = Request.Params["Captcha"];

                // Converte dados de retorno
                Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz()
                {
                    VIEWSTATE = VIEWSTATE,
                    EVENTVALIDATION = EVENTVALIDATION,
                    TokenCaptcha = TokenCaptcha,
                    SessionID = SessionID
                };

                // Pega informacoes do portal
                Servicos.ServicoConsultaCTe.RetornoOfConsultaSefazpIzbOyUQ consultaSefaz = consultaCTe.ConsultarSefaz(requisicaoSefaz, chave, captcha);

                if (consultaSefaz.Status && consultaSefaz.Objeto.ConsultaValida)
                {
                    // CT-e
                    CTe srvCTe = new CTe(unitOfWork);

                    // Inicia transition
                    //unitOfWork.Start();

                    // Busca serie ou cria
                    Dominio.Entidades.EmpresaSerie serie = null;
                    if (consultaSefaz.Objeto.CTe.Emitente.CNPJ != this.EmpresaUsuario.CNPJ)
                    {
                        serie = repEmpresaSerie.BuscarPorSerie(this.EmpresaUsuario.Codigo, int.Parse(consultaSefaz.Objeto.CTe.Serie), Dominio.Enumeradores.TipoSerie.CTeRec);
                        if (serie == null)
                        {
                            serie = new Dominio.Entidades.EmpresaSerie()
                            {
                                Empresa = this.EmpresaUsuario,
                                Numero = int.Parse(consultaSefaz.Objeto.CTe.Serie),
                                Status = "A",
                                Tipo = Dominio.Enumeradores.TipoSerie.CTeRec
                            };
                            repEmpresaSerie.Inserir(serie);
                        }
                    }
                    else
                    {
                        serie = repEmpresaSerie.BuscarPorSerie(this.EmpresaUsuario.Codigo, int.Parse(consultaSefaz.Objeto.CTe.Serie), Dominio.Enumeradores.TipoSerie.CTe);
                        if (serie == null)
                        {
                            serie = new Dominio.Entidades.EmpresaSerie()
                            {
                                Empresa = this.EmpresaUsuario,
                                Numero = int.Parse(consultaSefaz.Objeto.CTe.Serie),
                                Status = "A",
                                Tipo = Dominio.Enumeradores.TipoSerie.CTe
                            };
                            repEmpresaSerie.Inserir(serie);
                        }
                    }

                    // Busca modelo documento 
                    Dominio.Entidades.ModeloDocumentoFiscal modeloDocumentoFiscal = null;
                    if (consultaSefaz.Objeto.CTe.Emitente.CNPJ != this.EmpresaUsuario.CNPJ)
                        modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("957");
                    else
                        modeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("57");

                    // Define o tipo controle
                    long? tipoControle = null;
                    if (consultaSefaz.Objeto.CTe.Emitente.CNPJ != this.EmpresaUsuario.CNPJ)
                    {
                        tipoControle = repCTe.BuscarUltimoTipoControle() + 1;
                    }

                    // Tomador Pagador
                    bool? empresaComoTomadorPagador = null;
                    if (consultaSefaz.Objeto.CTe.Emitente.CNPJ != this.EmpresaUsuario.CNPJ)
                        empresaComoTomadorPagador = true;


                    // Cria o cte
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = srvCTe.PreencherCTePorObjeto(consultaSefaz.Objeto.CTe, this.EmpresaUsuario, unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, modeloDocumentoFiscal, serie, tipoControle, empresaComoTomadorPagador);

                    unitOfWork.CommitChanges();

                    return RetornoConsultaCTe(cte, unitOfWork);
                }
                else if (consultaSefaz.Status)
                {
                    return Json(false, false, "Falha: " + (!string.IsNullOrWhiteSpace(consultaSefaz.Objeto.MensagemSefaz) ? consultaSefaz.Objeto.MensagemSefaz : "Não foi possível processar os dados."));
                }
                else
                {
                    return Json(false, false, "Falha: " + (!string.IsNullOrWhiteSpace(consultaSefaz.Mensagem) ? consultaSefaz.Mensagem : "Ocorreu uma falha ao consultar os dados."));
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao processar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult RelatorioRelacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorio
                Repositorio.RelacaoCTesEntreguesCTes repRelacaoCTesEntreguesCTes = new Repositorio.RelacaoCTesEntreguesCTes(unitOfWork);
                Repositorio.RelacaoCTesEntregues repRelacaoCTesEntregues = new Repositorio.RelacaoCTesEntregues(unitOfWork);
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                // Covnerte parametro
                int.TryParse(Request.Params["Codigo"], out int codigo);

                // Busca dados
                Dominio.Entidades.RelacaoCTesEntregues relacao = repRelacaoCTesEntregues.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = repRelacaoCTesEntreguesCTes.BuscarCTesOrdenados(codigo, this.EmpresaUsuario.Codigo);

                // Valida
                if (relacao == null)
                    return Json<bool>(false, false, "Ocorreu uma falha ao buscar dados.");

                // Monta o objeto de valor para o relatorio
                List<Dominio.ObjetosDeValor.Relatorios.RelacaoCTesEntregues> dsRelacao = repRelacaoCTesEntregues.InstanciaDataSource();

                string dataBipagemFormatada = relacao.DataBipagem.ToString("dd/MM/yyyy");
                string dataEntregaFormatada = relacao.DataEntrega.ToString("dd/MM/yyyy");
                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in ctes)
                {
                    List<Dominio.Entidades.InformacaoCargaCTE> infos = repInformacaoCargaCTE.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                    dsRelacao.Add(new Dominio.ObjetosDeValor.Relatorios.RelacaoCTesEntregues() {
                        // Relacao
                        Numero = relacao.Numero.ToString(),
                        Data = dataBipagemFormatada,
                        Cliente = relacao.Cliente.Nome,
                        ClienteCPF_CNPJ = relacao.Cliente.CPF_CNPJ_Formatado,
                        DataEntrega = dataEntregaFormatada,
                        KmInicial = relacao.KmInicial.ToString(),
                        KmFinal = relacao.KmFinal.ToString(),
                        DiferencaKm = relacao.DiferencaKm.ToString(),
                        Descricao = relacao.Descricao,
                        Observacao = relacao.Observacao,
                        NumeroControle = relacao.NumeroControle,
                        Status = relacao.DescricaoStatus,

                        // CT-e
                        Emitente = cte.Intermediario?.Nome ?? cte.Empresa.RazaoSocial,
                        EmitenteCPF_CNPJ = cte.Intermediario?.CPF_CNPJ_Formatado ??  cte.Empresa.CNPJ_Formatado,
                        DataEmissao = cte.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Origem = LocalidadePrestacaoRelatorio("LOCINICIO", cte.Codigo, unitOfWork),
                        Destino = LocalidadePrestacaoRelatorio("LOCTERMINO", cte.Codigo, unitOfWork),
                        NumeroCTe = cte.Numero.ToString(),
                        OTS = cte.DataPrevistaEntrega?.ToString("dd/MM/yyyy") ?? string.Empty,
                        RemetenteNome = cte.Remetente.Nome,
                        RemetenteEndereco = cte.Remetente.Endereco,
                        RemetenteCidade = cte.Remetente.Localidade.Descricao,
                        RemetenteUF = cte.Remetente.Localidade.Estado.Sigla,
                        RemetenteCPF_CNPJ = cte.Remetente.CPF_CNPJ_Formatado,
                        DestinatarioNome = cte.Destinatario.Nome,
                        DestinatarioEndereco = cte.Destinatario.Endereco,
                        DestinatarioCidade = cte.Destinatario.Localidade.Descricao,
                        DestinatarioUF = cte.Destinatario.Localidade.Estado.Sigla,
                        DestinatarioCPF_CNPJ = cte.Destinatario.CPF_CNPJ_Formatado,
                        TomadorNome = cte.Tomador.Nome,
                        TomadorEndereco = cte.Tomador.Endereco,
                        TomadorCPF_CNPJ = cte.Tomador.CPF_CNPJ_Formatado,
                        TomadorCidade = cte.Tomador.Localidade.Descricao,
                        TomadorUF = cte.Tomador.Localidade.Estado.Sigla,
                        ValorMercadoria = cte.ValorTotalMercadoria.ToString("n2"),
                        PesoBruto = PesoRelatorio(infos, "01", "PESO BRUTO"),
                        PesoDeclarado = PesoRelatorio(infos, "01", "PESO DECLARADO"),
                        PesoCubado = PesoRelatorio(infos, "01", "PESO CUBADO"),
                        PesoAferido = PesoRelatorio(infos, "01", "PESO AFERIDO"),
                        PesoBaseCalculo = PesoRelatorio(infos, "01", "PESO BASE DE CALCULO"),
                        PesoFaturado = PesoRelatorio(infos, "01", "PESO FATURADO"),
                        QuantidadeVolumes = PesoRelatorio(infos, "03"),
                        ValorAReceber = cte.ValorAReceber.ToString("n2"),
                        ValorICMS = cte.ValorICMS.ToString("n2"),
                        NumeroNFes = String.Join(", ", (from d in cte.Documentos select d.Numero).ToArray())
                    });
                }

                // DataSource
                List<ReportDataSource> dataSources = new List<ReportDataSource>
                {
                    new ReportDataSource("CTes", dsRelacao)
                };

                // Gera arquivo 
                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/RelacaoCTesEntregues.rdlc", "Excel", null, dataSources);

                // Retorna o arquivo
                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "RelacaoCTesEntregues-" + relacao.Numero.ToString() + "." + arquivo.FileNameExtension);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da relação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
        #endregion

        #region Metodos Privados
        private string PesoRelatorio(List<Dominio.Entidades.InformacaoCargaCTE> infos, string unMedida, string descricao = null)
        {
            var aux = from obj in infos
                      where obj.UnidadeMedida == unMedida
                      select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                aux = aux.Where(o => o.Tipo == descricao);

            decimal peso = (aux.Sum(x => (decimal?)x.Quantidade) ?? 0m);

            return peso.ToString("n4");
        }

        private string LocalidadePrestacaoRelatorio(string descricaoObs, int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
            Dominio.Entidades.ObservacaoContribuinteCTE obs = repObservacaoContribuinteCTE.BuscarPorCTeEDescricao(this.EmpresaUsuario.Codigo, codigoCTe, descricaoObs);

            return obs?.Descricao ?? string.Empty;
        }

        private dynamic RetornoConsultaCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork)
        {
            // Se o CTe nao existir, retorna apenas a informacao
            if(cte == null)
            {
                return Json<dynamic>(new
                {
                    ChaveExistente = false
                }, true);
            }

            // Caso o CTe seja valido, retorna no padrao necessario para visualizao
            return Json<dynamic>(new
            {
                ChaveExistente = true,
                CTe = FormatoCTe(cte, 0, 0, unitOfWork)
            }, true);
        }

        private dynamic FormatoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, int codigo, int ordem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unitOfWork);
            
            List<Dominio.Entidades.InformacaoCargaCTE> quatidadeCargas = repInformacaoCargaCTE.BuscarPorCTe(cte.Codigo);

            //decimal pesoCTe = quatidadeCargas != null ? (from o in quatidadeCargas where o.UnidadeMedida == "01" && o.DescricaoUnidadeMedida.Equals("PESO BASE DE CALCULO") select o.Quantidade).Sum() : 0;
            //if (pesoCTe == 0)
            //    pesoCTe = quatidadeCargas != null ? (from o in quatidadeCargas where o.UnidadeMedida == "01" && o.DescricaoUnidadeMedida.Equals("PESO FATURADO") select o.Quantidade).Sum() : 0;
            decimal pesoCTe = quatidadeCargas != null ? (from o in quatidadeCargas where o.UnidadeMedida == "01" && o.DescricaoUnidadeMedida.Equals("PESO DECLARADO") select o.Quantidade).Sum() : 0;
            if (pesoCTe == 0)
                pesoCTe = quatidadeCargas != null ? (from o in quatidadeCargas where o.UnidadeMedida == "01" && o.DescricaoUnidadeMedida.Equals("PESO BRUTO") select o.Quantidade).Sum() : 0;
            if (pesoCTe == 0)
                pesoCTe = quatidadeCargas != null ? (from o in quatidadeCargas where o.UnidadeMedida == "01" select o.Quantidade).FirstOrDefault() : 0;

            return new
            {
                Id = codigo,
                Ordem = ordem,
                CTe = cte.Codigo,
                Chave = cte.Chave,
                CNPJEmitente = cte.ModeloDocumentoFiscal.Numero == "57" ? cte.Empresa.CNPJ : cte.Intermediario.CPF_CNPJ_SemFormato,
                Emitente = cte.ModeloDocumentoFiscal.Numero == "57" ? cte.Empresa.RazaoSocial : cte.Intermediario.Nome,
                Numero = cte.Numero,
                DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                TerminoPrestacao = LocalidadePrestacaoRelatorio("LOCTERMINO", cte.Codigo, unitOfWork),
                Destinatario = cte.Destinatario.CPF_CNPJ_SemFormato,
                Cidade = cte.LocalidadeTerminoPrestacao != null ? cte.LocalidadeTerminoPrestacao.Codigo : cte.Destinatario.Localidade.Codigo,
                ValorAReceber = cte.ValorAReceber,
                Peso = pesoCTe,
                Excluir = false
            };
        }

        private void SalvarColetas(ref Dominio.Entidades.RelacaoCTesEntregues relacao, Dominio.Entidades.CalculoRelacaoCTesEntregues calculo, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RelacaoCTesEntreguesColeta repRelacaoCTesEntreguesColeta = new Repositorio.RelacaoCTesEntreguesColeta(unitOfWork);

            // Converte paramentros
            List<dynamic> relacoes = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params["Coletas"]);

            foreach (dynamic rel in relacoes)
            {
                int codigo = (int)rel.Id;
                Dominio.Entidades.RelacaoCTesEntreguesColeta relacaoColeta = repRelacaoCTesEntreguesColeta.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if ((bool)rel.Excluir)
                    repRelacaoCTesEntreguesColeta.Deletar(relacaoColeta);
                else
                {
                    if (relacaoColeta == null)
                        relacaoColeta = new Dominio.Entidades.RelacaoCTesEntreguesColeta();

                    relacaoColeta.RelacaoCTesEntregues = relacao;
                    relacaoColeta.Descricao = (string)rel.Coleta;
                    relacaoColeta.Peso = (decimal)rel.Peso;
                    relacaoColeta.ValorEvento = 0;
                    relacaoColeta.ValorFracao = 0;

                    if(calculo != null)
                    {
                        relacaoColeta.ValorEvento = calculo.ColetaValorPorEvento;
                        relacaoColeta.ValorFracao = Math.Floor(relacaoColeta.Peso / calculo.ColetaFracao) * calculo.ColetaValorPorFracao;
                    }

                    if (codigo > 0)
                        repRelacaoCTesEntreguesColeta.Atualizar(relacaoColeta);
                    else
                        repRelacaoCTesEntreguesColeta.Inserir(relacaoColeta);
                }
            }
        }

        private void SalvarCTes(ref Dominio.Entidades.RelacaoCTesEntregues relacao, Repositorio.UnitOfWork unitOfWork)
        {
            // Repositorios
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.RelacaoCTesEntreguesCTes repRelacaoCTesEntreguesCTes = new Repositorio.RelacaoCTesEntreguesCTes(unitOfWork);

            // Converte paramentros
            List<dynamic> relacoes = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params["CTes"]);

            foreach (dynamic rel in relacoes)
            {
                int codigo = (int)rel.Id;
                Dominio.Entidades.RelacaoCTesEntreguesCTes relacaoCTe = repRelacaoCTesEntreguesCTes.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if ((bool)rel.Excluir)
                    repRelacaoCTesEntreguesCTes.Deletar(relacaoCTe);
                else if(codigo <= 0)
                {
                    int codigoCTe = (int)rel.CTe;
                    int ordemRelacao = (int)rel.Ordem;

                    relacaoCTe = new Dominio.Entidades.RelacaoCTesEntreguesCTes() {
                        CTe = repCTe.BuscarPorCodigo(codigoCTe),
                        RelacaoCTesEntregues = relacao,
                        Ordem = ordemRelacao
                    };
                    repRelacaoCTesEntreguesCTes.Inserir(relacaoCTe);
                } else if(relacaoCTe != null)
                {
                    int ordemRelacao = (int)rel.Ordem;
                    relacaoCTe.Ordem = ordemRelacao;
                    repRelacaoCTesEntreguesCTes.Atualizar(relacaoCTe);
                }
            }
        }

        //private void CalcularTotal(ref Dominio.Entidades.RelacaoCTesEntregues relacao, Dominio.Entidades.CalculoRelacaoCTesEntregues calculo, Repositorio.UnitOfWork unitOfWork)
        //{
        //    Repositorio.RelacaoCTesEntreguesColeta repRelacaoCTesEntreguesColeta = new Repositorio.RelacaoCTesEntreguesColeta(unitOfWork);
        //    Repositorio.RelacaoCTesEntreguesCTes repRelacaoCTesEntreguesCTes = new Repositorio.RelacaoCTesEntreguesCTes(unitOfWork);

        //    if (calculo != null)
        //    {
        //        // Diaria
        //        decimal valorDiaria = relacao.Diaria;

        //        switch (relacao.TipoDiaria)
        //        {
        //            case Dominio.Enumeradores.TipoDiariaRelacaoCTesEntregues.Diaria:
        //                if (calculo.ValorDiaria > 0)
        //                    valorDiaria = calculo.ValorDiaria;
        //                break;

        //            case Dominio.Enumeradores.TipoDiariaRelacaoCTesEntregues.MeiaDiaria:
        //                if (calculo.ValorMeiaDiaria > 0)
        //                    valorDiaria = calculo.ValorMeiaDiaria;
        //                break;
        //        }

        //        // Acrescimento e Desconto
        //        decimal acrescimento = relacao.ValorAcrescimos;
        //        decimal desconto = relacao.ValorDescontos;

        //        // CTes
        //        List<Dominio.Entidades.RelacaoCTesEntreguesCTes> ctes = repRelacaoCTesEntreguesCTes.BuscarPorRelacao(relacao.Codigo);
        //        ctes
        //    }
        //}
        #endregion
    }
}
