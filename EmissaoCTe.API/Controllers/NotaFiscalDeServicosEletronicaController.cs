using Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class NotaFiscalDeServicosEletronicaController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("emissaonfse.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult ObterConfiguracoes()
        {
            try
            {
                Dominio.Entidades.Empresa empresa = this.EmpresaUsuario;

                var retorno = new
                {
                    CodigoEmpresa = empresa.Codigo,
                    Cidade = empresa.Localidade.Codigo,
                    Estado = empresa.Localidade.Estado.Sigla,
                    Pais = empresa.Localidade.Estado.Pais.Sigla,
                    ObservacaoPadrao = empresa.Configuracao?.ObservacaoPadraoNFSe ?? string.Empty,
                    DicasEmissao = empresa.Configuracao?.DicasEmissaoCTe ?? string.Empty,
                    PermitirImportarCSV = empresa.Configuracao?.GerarNFSeImportacoes ?? false,
                    EmiteNFSeForaEmbarcador = empresa.Configuracao?.EmiteNFSeForaEmbarcador ?? false,
                    PermiteImportarXMLNFSe = empresa.Configuracao?.PermiteImportarXMLNFSe ?? false,
                    PermitirImportarNFeSalvas = empresa.Configuracao?.ArmazenaNotasParaGerarPorPeriodo ?? false
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações para emissão de NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal, serie, inicioRegistros;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["NumeroRPS"], out int numeroRPS);

                string numeroDocumento = Request.Params["NumeroDocumento"];
                string cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);

                Dominio.Enumeradores.StatusNFSe? status = null;
                Dominio.Enumeradores.StatusNFSe statusAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.Consultar(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), 0, numeroRPS, numeroDocumento, cnpjTomador, inicioRegistros, 50);
                int countNFSe = repNFSe.ContarConsulta(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), 0, numeroRPS, numeroDocumento, cnpjTomador);

                var retorno = (from obj in listaNFSe
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                   RPS = obj.RPS != null ? obj.RPS.Numero : 0,
                                   Tomador = obj.Tomador != null ? (obj.Tomador.Exterior ? obj.Tomador.NumeroDocumentoExterior : obj.Tomador.CPF_CNPJ_Formatado) + " - " + obj.Tomador.Nome : string.Empty,
                                   LocalidadePrestacao = obj.LocalidadePrestacaoServico != null ? obj.LocalidadePrestacaoServico.Estado.Sigla + " / " + obj.LocalidadePrestacaoServico.Descricao : string.Empty,
                                   Valor = obj.ValorServicos.ToString("n2"),
                                   DescricaoStatus = !string.IsNullOrWhiteSpace(obj.DescricaoStatus) ? obj.DescricaoStatus : string.Empty,
                                   MensagemRetorno = obj.RPS != null && !string.IsNullOrWhiteSpace(obj.RPS.MensagemRetorno) ? System.Web.HttpUtility.HtmlEncode(Utilidades.String.ReplaceInvalidCharacters(obj.RPS.MensagemRetorno)) : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|7", "Série|5", "Data Emissão|10", "RPS|7", "Tomador|13", "Loc. Prest.|12", "Valor|8", "Status|10", "Retorno|21" }, countNFSe);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as NFS-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarAdmin()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.IntegracaoNFSe repIntegracaoNFSe = new Repositorio.IntegracaoNFSe(unidadeDeTrabalho);

                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                int numeroInicial = 0;
                int numeroFinal = 0;
                int serie = 0;
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["Empresa"], out int empresa);

                int numeroCarga = 0;
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);

                List<int> series = new List<int>();
                Dominio.Enumeradores.StatusNFSe? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out Dominio.Enumeradores.StatusNFSe statusAux))
                    status = statusAux;

                List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.Consultar(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, series, numeroCarga, 0, "", "", inicioRegistros, 50);
                int countNFSe = repNFSe.ContarConsulta(empresa, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, series, numeroCarga, 0, "", "");

                var retorno = (from obj in listaNFSe
                               select new
                               {
                                   obj.Codigo,
                                   obj.Status,
                                   obj.Numero,
                                   DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy"),
                                   Serie = obj.Serie.Numero,
                                   Empresa = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.CNPJ + ")" : string.Empty,
                                   Tomador = obj.Tomador != null ? (obj.Tomador.Exterior ? obj.Tomador.NumeroDocumentoExterior : obj.Tomador.CPF_CNPJ_Formatado) + " - " + obj.Tomador.Nome : string.Empty,
                                   LocalidadePrestacao = obj.LocalidadePrestacaoServico != null ? obj.LocalidadePrestacaoServico.Estado.Sigla + " / " + obj.LocalidadePrestacaoServico.Descricao : string.Empty,
                                   Valor = obj.ValorServicos.ToString("n2"),
                                   DescricaoStatus = repIntegracaoNFSe.BuscarStatusIntegracaoEmissao(obj.Codigo) == "AguardandoConfirmacao" && obj.Status == Dominio.Enumeradores.StatusNFSe.Pendente ? "Aguardando confirmação embarcador para processar protocolo " + obj.Codigo.ToString() : !string.IsNullOrWhiteSpace(obj.DescricaoStatus) ? obj.DescricaoStatus : string.Empty,
                                   MensagemRetorno = obj.RPS != null && !string.IsNullOrWhiteSpace(obj.RPS.MensagemRetorno) ? System.Web.HttpUtility.HtmlEncode(Utilidades.String.ReplaceInvalidCharacters(obj.RPS.MensagemRetorno)) : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Número|8", "Data Emissão|10", "Série|6", "Empresa|10", "Tomador|15", "Loc. Prest.|15", "Valor|8", "Status|10", "Retorno|21" }, countNFSe);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo, codigoSerie, codigoNatureza, numeroSubstituicao, localidadePrestacaoServico;
                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Numero"], out int numero);
                int.TryParse(Request.Params["Serie"], out codigoSerie);
                int.TryParse(Request.Params["NumeroSubstituicao"], out numeroSubstituicao);
                int.TryParse(Request.Params["LocalidadePrestacaoServico"], out localidadePrestacaoServico);
                int.TryParse(Request.Params["Natureza"], out codigoNatureza);
                int.TryParse(Request.Params["NumeroRPS"], out int numeroRPS);

                decimal valorServicos, valorDeducoes, valorPIS, valorCOFINS, valorINSS, valorIR, valorCSLL, valorISSRetido, valorOutrasOperacoes, valorDescontoCondicionado, valorDescontoIncondicionado, aliquotaISS, baseCalculoISS, valorISS;
                decimal.TryParse(Request.Params["ValorServicos"], out valorServicos);
                decimal.TryParse(Request.Params["ValorDeducoes"], out valorDeducoes);
                decimal.TryParse(Request.Params["ValorPIS"], out valorPIS);
                decimal.TryParse(Request.Params["ValorCOFINS"], out valorCOFINS);
                decimal.TryParse(Request.Params["ValorINSS"], out valorINSS);
                decimal.TryParse(Request.Params["ValorIR"], out valorIR);
                decimal.TryParse(Request.Params["ValorCSLL"], out valorCSLL);
                decimal.TryParse(Request.Params["ValorISSRetido"], out valorISSRetido);
                decimal.TryParse(Request.Params["ValorOutrasOperacoes"], out valorOutrasOperacoes);
                decimal.TryParse(Request.Params["ValorDescontoIncondicionado"], out valorDescontoIncondicionado);
                decimal.TryParse(Request.Params["ValorDescontoCondicionado"], out valorDescontoCondicionado);
                decimal.TryParse(Request.Params["AliquotaISS"], out aliquotaISS);
                decimal.TryParse(Request.Params["BaseCalculoISS"], out baseCalculoISS);
                decimal.TryParse(Request.Params["ValorISS"], out valorISS);

                decimal valorIBSEstadual = Request["ValorIBSEstadual"].ToDecimal(); 
                decimal valorIBSMunicipal = Request["ValorIBSMunicipal"].ToDecimal(); 
                decimal valorCBS = Request["ValorCBS"].ToDecimal();


                bool issRetido, emitir, issIncluso;
                bool.TryParse(Request.Params["ISSRetido"], out issRetido);
                bool.TryParse(Request.Params["Emitir"], out emitir);
                bool.TryParse(Request.Params["ISSIncluso"], out issIncluso);

                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params["DataEmissao"] + " " + Request.Params["HoraEmissao"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                string serieSubstituicao = Request.Params["SerieSubstituicao"];
                string outrasInformacoes = Request.Params["OutrasInformacoes"];

                if (string.IsNullOrWhiteSpace(this.EmpresaUsuario.InscricaoMunicipal))
                    return Json<bool>(false, false, "Inscrição Municipal do emitente é inválida.");

                Repositorio.NotaFiscalServico repNFS = new Repositorio.NotaFiscalServico(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.NaturezaNFSe repNatureza = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.NFSe nfse = null;

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração negada.");

                    nfse = repNFSe.BuscarPorCodigo(codigo);

                    if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Rejeicao && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmDigitacao)
                        return Json<bool>(false, false, "O status da NFS-e não permite a alteração/emissão da mesma.");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão negada.");

                    Dominio.Entidades.NotaFiscalServico nfs = new Dominio.Entidades.NotaFiscalServico();

                    nfs.TipoNotaFiscalServico = Dominio.Enumeradores.TipoNotaFiscalServico.Eletronica;

                    repNFS.Inserir(nfs);

                    nfse = new Dominio.Entidades.NFSe();
                    nfse.Empresa = this.EmpresaUsuario;
                    nfse.Ambiente = this.EmpresaUsuario.TipoAmbiente;
                    nfse.NFS = nfs;
                }

                nfse.AliquotaISS = aliquotaISS;
                nfse.BaseCalculoISS = baseCalculoISS;
                nfse.DataEmissao = dataEmissao;
                nfse.DataIntegracao = DateTime.Now;
                nfse.ISSRetido = issRetido;
                nfse.LocalidadePrestacaoServico = repLocalidade.BuscarPorCodigo(localidadePrestacaoServico);
                nfse.Natureza = repNatureza.BuscarPorCodigo(codigoNatureza);
                nfse.NumeroSubstituicao = numeroSubstituicao;
                nfse.OutrasInformacoes = outrasInformacoes;
                nfse.Serie = repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerie);
                nfse.SerieSubstituicao = serieSubstituicao;
                nfse.ValorCOFINS = valorCOFINS;
                nfse.ValorCSLL = valorCSLL;
                nfse.ValorDeducoes = valorDeducoes;
                nfse.ValorDescontoCondicionado = valorDescontoCondicionado;
                nfse.ValorDescontoIncondicionado = valorDescontoIncondicionado;
                nfse.ValorINSS = valorINSS;
                nfse.ValorIR = valorIR;
                nfse.ValorISS = valorISS;
                nfse.ValorISSRetido = valorISSRetido;
                nfse.ValorOutrasRetencoes = valorOutrasOperacoes;
                nfse.ValorPIS = valorPIS;
                nfse.ValorServicos = valorServicos;
                nfse.Status = Dominio.Enumeradores.StatusNFSe.EmDigitacao; 
                nfse.ValorIBSEstadual = valorIBSEstadual; 
                nfse.ValorIBSMunicipal = valorIBSMunicipal; 
                nfse.ValorCBS = valorCBS;


                bool emiteNFSfora = nfse.Empresa?.Configuracao?.EmiteNFSeForaEmbarcador ?? false;
                List<string> erros = new List<string>();

                erros.AddRange(this.SalvarCliente("Intermediario", ref nfse, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Intermediario, unidadeDeTrabalho));
                erros.AddRange(this.SalvarCliente("Tomador", ref nfse, Dominio.Enumeradores.TipoClienteNotaFiscalServico.Tomador, unidadeDeTrabalho));

                if (nfse.LocalidadePrestacaoServico == null)
                    erros.Add("Localidade de prestação do serviço inválida.");

                if (emiteNFSfora && numero <= 0)
                    erros.Add("Número da nota não informado");

                if (erros.Count > 0)
                {
                    System.Text.StringBuilder htmlMensagemRetorno = new System.Text.StringBuilder();

                    htmlMensagemRetorno.Append("Foram encontradas as seguintes inconsistências ao salvar a NFS-e: <br/><b>");

                    foreach (string mensagem in erros)
                    {
                        htmlMensagemRetorno.Append("<br/>&bull; ").Append(mensagem);
                    }

                    htmlMensagemRetorno.Append("</b><br/><br/>Corrija-as e tente emitir novamente.");

                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, htmlMensagemRetorno.ToString());
                }

                if (nfse.Codigo > 0)
                {
                    if (this.UsuarioAdministrativo != null)
                        nfse.Log = string.Concat(nfse.Log, " / Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        nfse.Log = string.Concat(nfse.Log, " / Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    repNFSe.Atualizar(nfse);

                    if (numeroRPS > 0)
                    {
                        Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unidadeDeTrabalho);
                        Dominio.Entidades.RPSNFSe rps = repRPS.BuscarPorNFSe(nfse.Codigo);
                        if (rps != null)
                        {
                            rps.Numero = numeroRPS;

                            repRPS.Atualizar(rps);
                        }
                    }

                }
                else
                {
                    if (emiteNFSfora)
                        nfse.Numero = numero;
                    else
                        nfse.Numero = repNFSe.ObterUltimoNumero(nfse.Empresa.Codigo, nfse.Serie.Codigo) + 1;

                    if (this.UsuarioAdministrativo != null)
                        nfse.Log += string.Concat("Criado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        nfse.Log += string.Concat("Criado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    repNFSe.Inserir(nfse);
                }

                string msg = string.Empty;

                this.SalvarItens(nfse, unidadeDeTrabalho);
                this.SalvarDocumentos(nfse, unidadeDeTrabalho, out msg);

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    unidadeDeTrabalho.Rollback();
                    return Json<bool>(false, false, msg);
                }

                Dominio.Enumeradores.StatusNFSe statusAnterior = nfse.Status;

                unidadeDeTrabalho.CommitChanges();

                if (emitir)
                {
                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    unidadeDeTrabalho.Start();

                    if (emiteNFSfora)
                    {
                        Repositorio.RPSNFSe repRPS = new Repositorio.RPSNFSe(unidadeDeTrabalho);

                        Dominio.Entidades.RPSNFSe rps = new Dominio.Entidades.RPSNFSe();
                        rps.Status = "A";
                        rps.MensagemRetorno = "NFSe lançada já autorizada";
                        rps.Data = DateTime.Now;
                        rps.Empresa = nfse.Empresa;
                        repRPS.Inserir(rps);

                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Autorizado;
                        nfse.RPS = rps;
                        repNFSe.Atualizar(nfse);

                        svcNFSe.ConverterNFSeEmCTe(nfse, unidadeDeTrabalho, false);

                        unidadeDeTrabalho.CommitChanges();
                    }
                    else if (svcNFSe.Emitir(nfse, unidadeDeTrabalho))
                    {
                        nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;

                        repNFSe.Atualizar(nfse);

                        if (statusAnterior == Dominio.Enumeradores.StatusNFSe.Rejeicao)
                        {
                            Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unidadeDeTrabalho);
                            Dominio.Entidades.Embarcador.Cargas.CargaNFS cargaNFS = repCargaNFS.BuscarPorCodigoNFSe(nfse.Codigo);
                            if (cargaNFS != null)
                            {
                                if (cargaNFS.Carga.PossuiPendencia)
                                {
                                    Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                                    cargaNFS.Carga.PossuiPendencia = false;
                                    cargaNFS.Carga.problemaNFS = false;
                                    cargaNFS.Carga.MotivoPendencia = "";
                                    repCarga.Atualizar(cargaNFS.Carga);
                                }
                            }
                        }


                        unidadeDeTrabalho.CommitChanges();

                        FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);
                    }
                    else
                    {
                        unidadeDeTrabalho.CommitChanges();

                        return Json<bool>(true, true, "A NFS-e foi salva, mas ocorreu uma falha ao emitir. Atualize a página e tente novamente.");
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Emitir()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigo);

                if (nfse != null)
                {
                    if (nfse.Empresa.Status != "A")
                        return Json<bool>(true, false, "Empresa não está ativa para emissão de NFS-e.");

                    if (nfse.Empresa.StatusFinanceiro == "B")
                        return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                    Dominio.Enumeradores.StatusNFSe statusAnterior = nfse.Status;

                    if (nfse.Status == Dominio.Enumeradores.StatusNFSe.EmDigitacao || nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao)
                    {
                        unidadeDeTrabalho.Start();

                        if (svcNFSe.Emitir(nfse, unidadeDeTrabalho))
                        {
                            nfse.Status = Dominio.Enumeradores.StatusNFSe.Enviado;

                            repNFSe.Atualizar(nfse);

                            if (statusAnterior == Dominio.Enumeradores.StatusNFSe.Rejeicao)
                            {
                                Repositorio.Embarcador.Cargas.CargaNFS repCargaNFS = new Repositorio.Embarcador.Cargas.CargaNFS(unidadeDeTrabalho);
                                Dominio.Entidades.Embarcador.Cargas.CargaNFS cargaNFS = repCargaNFS.BuscarPorCodigoNFSe(nfse.Codigo);
                                if (cargaNFS != null)
                                {
                                    if (cargaNFS.Carga.PossuiPendencia)
                                    {
                                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                                        cargaNFS.Carga.PossuiPendencia = false;
                                        cargaNFS.Carga.problemaNFS = false;
                                        cargaNFS.Carga.MotivoPendencia = "";
                                        repCarga.Atualizar(cargaNFS.Carga);
                                    }
                                }
                            }

                            unidadeDeTrabalho.CommitChanges();

                            FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);
                        }
                        else
                        {
                            unidadeDeTrabalho.Rollback();

                            return Json<bool>(false, false, "A NFS-e foi salva, mas ocorreu uma falha ao emitir. Atualize a página e tente novamente.");
                        }
                    }
                    else
                        return Json<bool>(false, false, "Status da NFS-e não permite emitir novamente.");
                }
                else
                    return Json<bool>(false, false, "A NFS-e não encontrada para emissão.");

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir a NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirTodas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                int numeroInicial, numeroFinal, serie, inicioRegistros;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NumeroRPS"], out int numeroRPS);

                string numeroDocumento = Request.Params["NumeroDocumento"];
                string cnpjTomador = Utilidades.String.OnlyNumbers(Request.Params["Tomador"]);

                Dominio.Enumeradores.StatusNFSe? status = null;
                Dominio.Enumeradores.StatusNFSe statusAux;
                if (Enum.TryParse<Dominio.Enumeradores.StatusNFSe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.ConsultarTodas(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), 0, numeroRPS, numeroDocumento, cnpjTomador, 0, 500);
                int countNFSe = repNFSe.ContarConsulta(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, numeroInicial, numeroFinal, serie, status, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.NFSe).Select(o => o.Codigo).ToList(), 0, 0, numeroDocumento, cnpjTomador);

                for (var i = 0; i < listaNFSe.Count; i++)
                {
                    if (listaNFSe[i].ValorServicos > 0)
                    {
                        Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(listaNFSe[i].Codigo);

                        if (svcNFSe.Emitir(nfse, unidadeDeTrabalho))
                            svcNFSe.AdicionarNFSeNaFilaDeConsulta(nfse, unidadeDeTrabalho);

                        unidadeDeTrabalho.FlushAndClear();
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as NFS-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult GerarNFSeNotasImportadas()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.XMLNotaFiscalEletronica repXMLNotaFiscalEletronica = new Repositorio.XMLNotaFiscalEletronica(unidadeDeTrabalho);

                List<long> codigoNotasImportadas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<long>>(Request.Params["NotasImportadas"]);
                List<Dominio.Entidades.XMLNotaFiscalEletronica> documentosSelecionados = repXMLNotaFiscalEletronica.BuscarPorCodigos(codigoNotasImportadas);

                if (documentosSelecionados == null || documentosSelecionados.Count == 0)
                    return Json<bool>(true, false, "Notas não localizadas para gerar NFS-e.");

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                unidadeDeTrabalho.Start();

                Dominio.Entidades.NFSe nfse = svcNFSe.GerarNFSePorXMLNotaFiscalEletronica(documentosSelecionados, this.EmpresaUsuario.Codigo, unidadeDeTrabalho);

                if (nfse == null)
                    throw new Exception("NFS-e não foi gerada");

                foreach (Dominio.Entidades.XMLNotaFiscalEletronica nota in documentosSelecionados)
                {
                    nota.DataImportacao = DateTime.Now;
                    if (this.UsuarioAdministrativo != null)
                        nota.Log = string.Concat(nota.Log, " / Gerada NFSe codigo " + nfse.Codigo.ToString() + " por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome);
                    else
                        nota.Log = string.Concat(nota.Log, " / Gerada NFSe codigo " + nfse.Codigo.ToString() + " por ", this.Usuario.CPF, " - ", this.Usuario.Nome);
                    nota.GeradoDocumento = true;
                    repXMLNotaFiscalEletronica.Atualizar(nota);
                }

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir a NFS-e.");
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada. Atualize a página e tente novamente.");

                Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unitOfWork);

                List<Dominio.Entidades.ItemNFSe> itens = repItem.BuscarPorNFSe(nfse.Codigo);

                var retorno = new
                {
                    nfse.Codigo,
                    nfse.AliquotaISS,
                    nfse.Ambiente,
                    nfse.BaseCalculoISS,
                    nfse.CodigoVerificacao,
                    DataEmissao = nfse.DataEmissao.ToString("dd/MM/yyyy"),
                    HoraEmissao = nfse.DataEmissao.ToString("HH:mm"),
                    nfse.ISSRetido,
                    EstadoPrestacaoServico = nfse.LocalidadePrestacaoServico.Estado.Sigla,
                    LocalidadePrestacaoServico = nfse.LocalidadePrestacaoServico.Codigo,
                    Natureza = nfse.Natureza.Codigo,
                    nfse.Numero,
                    nfse.NumeroSubstituicao,
                    nfse.OutrasInformacoes,
                    Serie = nfse.Serie.Codigo,
                    nfse.SerieSubstituicao,
                    nfse.Status,
                    nfse.ValorCOFINS,
                    nfse.ValorCSLL,
                    nfse.ValorDeducoes,
                    nfse.ValorDescontoCondicionado,
                    nfse.ValorDescontoIncondicionado,
                    nfse.ValorINSS,
                    nfse.ValorIR,
                    nfse.ValorISS,
                    nfse.ValorISSRetido,
                    nfse.ValorOutrasRetencoes,
                    nfse.ValorPIS,
                    nfse.ValorServicos,
                    nfse.ValorCBS,
                    nfse.ValorIBSEstadual,
                    nfse.ValorIBSMunicipal,
                    nfse.Log,
                    NumeroRPS = nfse.RPS != null ? nfse.RPS.Numero : 0,
                    Tomador = nfse.Tomador != null ? new
                    {
                        CodigoAtividade = nfse.Tomador.Atividade.Codigo,
                        Bairro = nfse.Tomador.Bairro,
                        CEP = nfse.Tomador.CEP,
                        Cidade = nfse.Tomador.Cidade,
                        Complemento = nfse.Tomador.Complemento,
                        CPF_CNPJ = nfse.Tomador.CPF_CNPJ_Formatado,
                        DescricaoAtividade = nfse.Tomador.Atividade.Descricao,
                        Email = nfse.Tomador.Email,
                        EmailContador = nfse.Tomador.EmailContador,
                        EmailContato = nfse.Tomador.EmailContato,
                        Endereco = nfse.Tomador.Endereco,
                        Exportacao = nfse.Tomador.Exterior,
                        InscricaoMunicipal = nfse.Tomador.InscricaoMunicipal,
                        CodigoLocalidade = nfse.Tomador.Exterior ? 0 : nfse.Tomador.Localidade.Codigo,
                        NomeFantasia = nfse.Tomador.NomeFantasia,
                        Numero = nfse.Tomador.Numero,
                        NumeroDocumentoExportacao = nfse.Tomador.NumeroDocumentoExterior,
                        Nome = nfse.Tomador.Nome,
                        IE_RG = nfse.Tomador.IE_RG,
                        SalvarEndereco = nfse.Tomador.SalvarEndereco,
                        SiglaPais = nfse.Tomador.Exterior ? nfse.Tomador.Pais.Sigla : nfse.Tomador.Localidade.Estado.Pais.Sigla,
                        EmailStatus = nfse.Tomador.EmailStatus,
                        EmailContadorStatus = nfse.Tomador.EmailContadorStatus,
                        EmailContatoStatus = nfse.Tomador.EmailContatoStatus,
                        Telefone1 = nfse.Tomador.Telefone1,
                        Telefone2 = nfse.Tomador.Telefone2,
                        UF = nfse.Tomador.Exterior ? "EX" : nfse.Tomador.Localidade.Estado.Sigla
                    } : null,
                    Intermediario = nfse.Intermediario != null ? new
                    {
                        CodigoAtividade = nfse.Intermediario.Atividade.Codigo,
                        Bairro = nfse.Intermediario.Bairro,
                        CEP = nfse.Intermediario.CEP,
                        Cidade = nfse.Intermediario.Cidade,
                        Complemento = nfse.Intermediario.Complemento,
                        CPF_CNPJ = nfse.Intermediario.CPF_CNPJ_Formatado,
                        DescricaoAtividade = nfse.Intermediario.Atividade.Descricao,
                        Email = nfse.Intermediario.Email,
                        EmailContador = nfse.Intermediario.EmailContador,
                        EmailContato = nfse.Intermediario.EmailContato,
                        Endereco = nfse.Intermediario.Endereco,
                        Exportacao = nfse.Intermediario.Exterior,
                        InscricaoMunicipal = nfse.Intermediario.InscricaoMunicipal,
                        CodigoLocalidade = nfse.Intermediario.Exterior ? 0 : nfse.Intermediario.Localidade.Codigo,
                        NomeFantasia = nfse.Intermediario.NomeFantasia,
                        Numero = nfse.Intermediario.Numero,
                        NumeroDocumentoExportacao = nfse.Intermediario.NumeroDocumentoExterior,
                        Nome = nfse.Intermediario.Nome,
                        IE_RG = nfse.Intermediario.IE_RG,
                        SalvarEndereco = nfse.Intermediario.SalvarEndereco,
                        SiglaPais = nfse.Intermediario.Exterior ? nfse.Intermediario.Pais.Sigla : nfse.Intermediario.Localidade.Estado.Pais.Sigla,
                        EmailStatus = nfse.Intermediario.EmailStatus,
                        EmailContadorStatus = nfse.Intermediario.EmailContadorStatus,
                        EmailContatoStatus = nfse.Intermediario.EmailContatoStatus,
                        Telefone1 = nfse.Intermediario.Telefone1,
                        Telefone2 = nfse.Intermediario.Telefone2,
                        UF = nfse.Intermediario.Exterior ? "EX" : nfse.Intermediario.Localidade.Estado.Sigla
                    } : null,
                    Itens = (from obj in itens
                             select new
                             {
                                 obj.AliquotaISS,
                                 obj.BaseCalculoISS,
                                 obj.Codigo,
                                 obj.Discriminacao,
                                 obj.ExigibilidadeISS,
                                 Localidade = obj.Municipio.Codigo,
                                 Estado = obj.Municipio.Estado.Sigla,
                                 LocalidadeIncidencia = obj.MunicipioIncidencia.Codigo,
                                 EstadoIncidencia = obj.MunicipioIncidencia.Estado.Sigla,
                                 Pais = obj.PaisPrestacaoServico.Sigla,
                                 obj.Quantidade,
                                 CodigoServico = obj.Servico.Codigo,
                                 DescricaoServico = obj.Servico.Numero + " - " + obj.Servico.Descricao,
                                 ServicoPrestadoPais = obj.ServicoPrestadoNoPais,
                                 obj.ValorDeducoes,
                                 obj.ValorDescontoCondicionado,
                                 obj.ValorDescontoIncondicionado,
                                 obj.ValorISS,
                                 obj.ValorServico,
                                 obj.ValorTotal,
                                 obj.IncluirISSNoFrete,
                                 obj.NBS,
                                 obj.CodigoIndicadorOperacao,
                                 obj.CSTIBSCBS,
                                 obj.ClassificacaoTributariaIBSCBS,
                                 obj.BaseCalculoIBSCBS,
                                 obj.AliquotaIBSEstadual,
                                 obj.PercentualReducaoIBSEstadual,
                                 obj.ValorIBSEstadual,
                                 obj.AliquotaIBSMunicipal,
                                 obj.PercentualReducaoIBSMunicipal,
                                 obj.ValorIBSMunicipal,
                                 obj.AliquotaCBS,
                                 obj.PercentualReducaoCBS,
                                 obj.ValorCBS,
                                 Excluir = false
                             }).ToList(),
                    Documentos = (from obj in nfse.Documentos
                                  select new
                                  {
                                      Id = obj.Codigo,
                                      obj.Chave,
                                      obj.Numero,
                                      obj.Serie,
                                      Valor = obj.Valor,
                                      Peso = obj.Peso,
                                      DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                      Excluir = false
                                  }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados da NFS-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDANFSE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

                int.TryParse(Request.Params["CodigoNFSe"], out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return Json<bool>(false, false, "O status da NFS-e não permite a geração do DANFSE.");

                byte[] danfse = null;

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento = repLancamentoNFSManual.BuscarPorNFSe(nfse.Codigo);
                    if (lancamento != null && !string.IsNullOrWhiteSpace(lancamento.DadosNFS.ImagemNFS))
                        danfse = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(lancamento.DadosNFS.ImagemNFS);
                }
                else
                {
                    danfse = svcNFSe.ObterDANFSE(nfse.Codigo);
                }

                if (danfse != null)
                    return Arquivo(danfse, "application/pdf", "NFSe_" + nfse.Numero.ToString() + ".pdf");
                else
                    return Json<bool>(false, false, "Não foi possível gerar o DANFSE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do DANFSE.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unitOfWork);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unitOfWork);
                Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);

                int.TryParse(Request.Params["CodigoNFSe"], out int codigoNFSe);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada, atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmCancelamento
                    && nfse.Status != Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                    return Json<bool>(false, false, "O status da NFS-e não permite a geração do XML.");

                byte[] xml = null;

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.NFSeManualGerada)
                {
                    Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamento = repLancamentoNFSManual.BuscarPorNFSe(nfse.Codigo);
                    if (lancamento != null && !string.IsNullOrWhiteSpace(lancamento.DadosNFS.XML))
                        xml = System.Text.Encoding.Default.GetBytes(lancamento.DadosNFS.XML);
                }
                else
                {
                    xml = svcNFSe.ObterXML(codigoNFSe, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao);
                }

                if (xml != null)
                    return Arquivo(xml, "text/xml", string.Concat("NFSe_", nfse.Numero, ".xml"));

                return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Cancelar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                string justificativa = Request.Params["Justificativa"];

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada. Atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado)
                    return Json<bool>(false, false, "O status da NFS-e não permite o cancelamento da mesma.");

                if (justificativa.Length < 20 || justificativa.Length > 255)
                    return Json<bool>(false, false, "A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.");

                nfse.JustificativaCancelamento = justificativa;

                repNFSe.Atualizar(nfse);

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                if (!svcNFSe.Cancelar(nfse.Codigo, unidadeDeTrabalho, true, this.UsuarioAdministrativo ?? this.Usuario))
                {
                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, "Ocorreu uma falha ao enviar a solicitação de cancelamento.");
                }

                FilaConsultaCTe.GetInstance().QueueItem(3, nfse.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.NFSe, Conexao.StringConexao);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar a NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InformarCancelamentoPrefeitura()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                string justificativa = Request.Params["Justificativa"];

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.RPSNFSe repRPSNFSe = new Repositorio.RPSNFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada. Atualize a página e tente novamente.");

                if (justificativa.Length < 20 || justificativa.Length > 255)
                    return Json<bool>(false, false, "A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.");

                if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado)
                {
                    nfse.JustificativaCancelamento = justificativa;
                    string logCancelamento = string.Empty;
                    if (this.UsuarioAdministrativo != null)
                        logCancelamento = string.Concat("Cancelamento na prefeitura informado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        logCancelamento = string.Concat("Cancelamento na prefeitura informado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    nfse.Log = !string.IsNullOrWhiteSpace(nfse.Log) ? string.Concat(nfse.Log, " / ", logCancelamento) : logCancelamento;

                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Cancelado;
                    repNFSe.Atualizar(nfse);

                    nfse.RPS.Status = "C";
                    nfse.RPS.MensagemRetorno = logCancelamento;
                    repRPSNFSe.Atualizar(nfse.RPS);

                    bool averbarNFSe = (nfse.Empresa.Configuracao != null && nfse.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (nfse.Empresa.Configuracao != null && nfse.Empresa.EmpresaPai != null && nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);
                    if (averbarNFSe)
                    {
                        Servicos.AverbacaoNFSe svcAverbacaoNFSe = new Servicos.AverbacaoNFSe(unidadeDeTrabalho);
                        svcAverbacaoNFSe.Averbar(nfse, Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho);
                    }

                }
                else if (nfse.Status == Dominio.Enumeradores.StatusNFSe.Rejeicao)
                {
                    Dominio.Entidades.RPSNFSe rpsNFSe = repRPSNFSe.BuscarPorNFSe(nfse.Codigo);

                    nfse.JustificativaCancelamento = justificativa;
                    string logCancelamento = string.Empty;
                    if (this.UsuarioAdministrativo != null)
                        logCancelamento = string.Concat("Inutilização informada por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        logCancelamento = string.Concat("Inutilização informada por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    nfse.Log = !string.IsNullOrWhiteSpace(nfse.Log) ? string.Concat(nfse.Log, " / ", logCancelamento) : logCancelamento;

                    nfse.Numero = 0;
                    nfse.Status = Dominio.Enumeradores.StatusNFSe.Inutilizada;
                    if (nfse.Empresa.Localidade.CodigoIBGE != 3524006) //Itupeva não exclui o RPS pois possui o bloco 
                        nfse.RPS = null;
                    repNFSe.Atualizar(nfse);

                    if (nfse.Empresa.Localidade.CodigoIBGE != 3524006 && rpsNFSe != null) //Itupeva não exclui o RPS pois possui o bloco 
                        repRPSNFSe.Deletar(rpsNFSe);
                }
                else
                    return Json<bool>(false, false, "O status da NFS-e não permite a inutilização.");


                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar a NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Excluir()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFSe;
                int.TryParse(Request.Params["CodigoNFSe"], out codigoNFSe);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada. Atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Rejeicao && nfse.Status != Dominio.Enumeradores.StatusNFSe.EmDigitacao)
                    return Json<bool>(false, false, "O status da NFS-e não permite a exclusão da mesma.");

                unidadeDeTrabalho.Start();

                Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                svcNFSe.Deletar(nfse.Codigo, unidadeDeTrabalho);

                unidadeDeTrabalho.CommitChanges();

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao excluir a NFS-e.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["CodigoNFSe"], out int codigoNFSe);

                Repositorio.AverbacaoNFSe repAverbacao = new Repositorio.AverbacaoNFSe(unidadeDeTrabalho);

                List<Dominio.Entidades.AverbacaoNFSe> averbacoes = repAverbacao.BuscarPorNFSe(this.EmpresaUsuario.Codigo, codigoNFSe);

                var retorno = (from obj in averbacoes
                               orderby obj.CodigoIntegracao descending
                               select new
                               {
                                   Data = obj.DataRetorno != null ? obj.DataRetorno.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty,
                                   Tipo = obj.DescricaoTipo,
                                   Protocolo = obj.Status == Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso && !string.IsNullOrWhiteSpace(obj.Averbacao) ? obj.Averbacao : obj.Protocolo ?? string.Empty,
                                   Status = obj.DescricaoStatus,
                                   Mensagem = obj.CodigoRetorno + " - " + obj.MensagemRetorno
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Data|20", "Tipo|15", "Protocolo|15", "Status|15", "Mensagem|35" }, retorno.Count);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados de averbação do MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarAverbacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params["CodigoNFSe"], out int codigoNFSe);

                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.NFSe nfse = repNFSe.BuscarPorCodigo(codigoNFSe);

                if (nfse == null)
                    return Json<bool>(false, false, "NFS-e não encontrada. Atualize a página e tente novamente.");

                if (nfse.Status != Dominio.Enumeradores.StatusNFSe.Autorizado && nfse.Status != Dominio.Enumeradores.StatusNFSe.Cancelado)
                    return Json<bool>(false, false, "O status da NFS-e não permite a solicitação de averbação.");

                Repositorio.AverbacaoNFSe repAverbacao = new Repositorio.AverbacaoNFSe(unidadeDeTrabalho);

                if (repAverbacao.ContarPorNFSeTipoEStatus(nfse.Codigo, nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, new Dominio.Enumeradores.StatusAverbacaoCTe[] { Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso, Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado }) > 0)
                    return Json<bool>(false, false, "Já existe uma averbação para esta NFS-e.");

                Servicos.AverbacaoNFSe svcAverbacao = new Servicos.AverbacaoNFSe(unidadeDeTrabalho);

                svcAverbacao.Averbar(nfse, nfse.Status == Dominio.Enumeradores.StatusNFSe.Autorizado ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar a averbação.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ImportarNFSeCSV()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".csv")
                    {
                        List<Dominio.ObjetosDeValor.NFSeCSV> listaNFSe = this.CarregarDadosCSV(file.InputStream, unitOfWork);

                        if (listaNFSe == null || listaNFSe.Count == 0)
                            return Json<bool>(false, false, "Nenhuma NFSe importada.");

                        string validarClientes = this.ValidarClientesCSV(listaNFSe, unitOfWork);
                        if (!string.IsNullOrWhiteSpace(validarClientes))
                            return Json<bool>(false, false, validarClientes);

                        if (!this.GerarNFSePorCSV(this.Usuario.Empresa, listaNFSe, unitOfWork))
                            return Json<bool>(false, false, "Não foi possível importar.");
                        else
                            return Json<bool>(true, true);
                    }
                    else
                        return Json<bool>(false, false, "Arquivo com formato invalido.");
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao importar CSV.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ImportarNFSeXMLAutorizado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".xml")
                    {
                        Servicos.NFSe svcNFSe = new Servicos.NFSe(unitOfWork);
                        object retorno = svcNFSe.ObterDocumentoPorXMLNFSeAutorizada(file.InputStream, this.EmpresaUsuario);

                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                                return Json<bool>(false, false, (string)retorno);
                            else
                                return Json<bool>(false, false, "Nota de Serviço inválida.");
                        }
                        else
                            return Json<bool>(true, true);
                    }
                    else
                        return Json<bool>(false, false, "Arquivo com formato inválido.");
                }
                else
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao importar XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarDocumentos(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unidadeDeTrabalho, out string msg)
        {
            msg = string.Empty;
            Repositorio.DocumentosNFSe repDocumentosNFSe = new Repositorio.DocumentosNFSe(unidadeDeTrabalho);

            List<Dominio.ObjetosDeValor.DocumentoNFSe> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DocumentoNFSe>>(Request.Params["Documentos"]);

            foreach (Dominio.ObjetosDeValor.DocumentoNFSe documento in documentos)
            {
                Dominio.Entidades.DocumentosNFSe item = repDocumentosNFSe.BuscarPorCodigo(documento.Id) ?? new Dominio.Entidades.DocumentosNFSe();

                if (!documento.Excluir)
                {
                    DateTime dataEmissaoAux;
                    DateTime? dataEmissao = null;
                    if (DateTime.TryParseExact(documento.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoAux))
                        dataEmissao = dataEmissaoAux;

                    item.Chave = documento.Chave;
                    item.Numero = documento.Numero;
                    item.Serie = documento.Serie;
                    item.Valor = documento.Valor;
                    item.DataEmissao = dataEmissao;
                    item.Peso = documento.Peso;
                    item.NFSe = nfse;

                    bool valido = true;

                    if (string.IsNullOrWhiteSpace(item.Numero))
                        valido = false;

                    if (string.IsNullOrWhiteSpace(item.Serie))
                        valido = false;

                    if (valido)
                    {
                        if (item.Codigo > 0)
                            repDocumentosNFSe.Atualizar(item);
                        else
                            repDocumentosNFSe.Inserir(item);
                    }
                    else
                    {
                        msg = "Número e série do Documento é obrigatório.";
                    }
                }
                else if (item != null && item.Codigo > 0)
                {
                    repDocumentosNFSe.Deletar(item);
                }
            }
        }

        private void SalvarItens(Dominio.Entidades.NFSe nfse, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ItemNFSe repItem = new Repositorio.ItemNFSe(unidadeDeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Pais repPais = new Repositorio.Pais(unidadeDeTrabalho);
            Repositorio.ServicoNFSe repServico = new Repositorio.ServicoNFSe(unidadeDeTrabalho);

            dynamic itens = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params["Itens"]);

            foreach (var itemEditar in itens)
            {
                Dominio.Entidades.ItemNFSe item = (int)itemEditar.Codigo > 0 ? repItem.BuscarPorCodigo((int)itemEditar.Codigo) : null;

                if (!(bool)itemEditar.Excluir)
                {
                    if (item == null)
                    {
                        item = new Dominio.Entidades.ItemNFSe();
                        item.NFSe = nfse;
                    }

                    item.AliquotaISS = (decimal)itemEditar.AliquotaISS;
                    item.BaseCalculoISS = (decimal)itemEditar.BaseCalculoISS;
                    item.Discriminacao = (string)itemEditar.Discriminacao;
                    item.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)itemEditar.ExigibilidadeISS;
                    item.Municipio = repLocalidade.BuscarPorCodigo((int)itemEditar.Localidade);
                    item.MunicipioIncidencia = repLocalidade.BuscarPorCodigo((int)itemEditar.LocalidadeIncidencia);
                    item.PaisPrestacaoServico = repPais.BuscarPorSigla((string)itemEditar.Pais);
                    item.Quantidade = (decimal)itemEditar.Quantidade;
                    item.Servico = repServico.BuscarPorCodigo((int)itemEditar.CodigoServico);
                    item.ServicoPrestadoNoPais = (bool)itemEditar.ServicoPrestadoPais;
                    item.ValorDeducoes = (decimal)itemEditar.ValorDeducoes;
                    item.ValorDescontoCondicionado = (decimal)itemEditar.ValorDescontoCondicionado;
                    item.ValorDescontoIncondicionado = (decimal)itemEditar.ValorDescontoIncondicionado;
                    item.ValorISS = (decimal)itemEditar.ValorISS;
                    item.ValorServico = (decimal)itemEditar.ValorServico;
                    item.ValorTotal = (decimal)itemEditar.ValorTotal;
                    item.IncluirISSNoFrete = (bool)itemEditar.IncluirISSNoFrete ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                    item.NBS = itemEditar?.NBS ?? "";
                    item.CodigoIndicadorOperacao =  itemEditar?.CodigoIndicadorOperacao ?? "";
                    item.CSTIBSCBS =  itemEditar?.CSTIBSCBS ?? "";
                    item.ClassificacaoTributariaIBSCBS = itemEditar?.ClassificacaoTributariaIBSCBS ?? "";
                    item.BaseCalculoIBSCBS = (decimal?)itemEditar.BaseCalculoIBSCBS ?? 0;
                    item.AliquotaIBSEstadual = (decimal?)itemEditar.AliquotaIBSEstadual ?? 0;
                    item.PercentualReducaoIBSEstadual = (decimal?)itemEditar.PercentualReducaoIBSEstadual ?? 0;
                    item.ValorIBSEstadual = (decimal?)itemEditar.ValorIBSEstadual ?? 0;
                    item.AliquotaIBSMunicipal = (decimal?)itemEditar.AliquotaIBSMunicipal ?? 0;
                    item.PercentualReducaoIBSMunicipal = (decimal?)itemEditar.PercentualReducaoIBSMunicipal ?? 0;
                    item.ValorIBSMunicipal = (decimal?)itemEditar.ValorIBSMunicipal ?? 0;
                    item.AliquotaCBS = (decimal?)itemEditar.AliquotaCBS ?? 0;
                    item.PercentualReducaoCBS = (decimal?)itemEditar.PercentualReducaoCBS ?? 0;
                    item.ValorCBS = (decimal?)itemEditar.ValorCBS ?? 0;

                    if (item.Codigo > 0)
                        repItem.Atualizar(item);
                    else
                        repItem.Inserir(item);
                }
                else if (item != null && item.Codigo > 0)
                {
                    repItem.Deletar(item);
                }
            }
        }

        private List<string> SalvarCliente(string param, ref Dominio.Entidades.NFSe nfse, Dominio.Enumeradores.TipoClienteNotaFiscalServico tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<string> erros = new List<string>();

            try
            {
                if (Request.Params[param] != "null" && Request.Params[param] != null)
                {
                    Dominio.ObjetosDeValor.Cliente clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params[param]);

                    if (!clienteJS.Exportacao)
                    {
                        if (!string.IsNullOrWhiteSpace(clienteJS.CPFCNPJ))
                        {
                            double cpfCnpj = 0f;
                            clienteJS.CPFCNPJ = Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ);

                            clienteJS.CPFCNPJ = string.IsNullOrWhiteSpace(clienteJS.CPFCNPJ) ? "0" : clienteJS.CPFCNPJ;

                            if (double.TryParse(clienteJS.CPFCNPJ, out cpfCnpj) && (clienteJS.CPFCNPJ.Length == 11 || clienteJS.CPFCNPJ.Length == 14))
                            {
                                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

                                bool inserir = false, salvarEndereco = true;

                                Dominio.ObjetosDeValor.Endereco endereco = null;

                                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                                if (cliente == null)
                                {
                                    cliente = new Dominio.Entidades.Cliente();
                                    inserir = true;
                                }
                                else
                                {
                                    salvarEndereco = clienteJS.SalvarEndereco;
                                }

                                cliente.Atividade = repAtividade.BuscarPorCodigo(clienteJS.CodigoAtividade);

                                if (!salvarEndereco)
                                {
                                    endereco = new Dominio.ObjetosDeValor.Endereco();
                                    endereco.Bairro = clienteJS.Bairro;
                                    endereco.CEP = clienteJS.CEP;
                                    endereco.Complemento = clienteJS.Complemento;
                                    endereco.Logradouro = clienteJS.Endereco;
                                    endereco.Cidade = repLocalidade.BuscarPorCodigo(clienteJS.Localidade);
                                    endereco.Numero = clienteJS.Numero;
                                    endereco.Telefone = clienteJS.Telefone1;
                                }
                                else
                                {
                                    cliente.Bairro = clienteJS.Bairro;
                                    cliente.CEP = clienteJS.CEP;
                                    cliente.Complemento = clienteJS.Complemento;
                                    cliente.Endereco = clienteJS.Endereco;
                                    cliente.Localidade = repLocalidade.BuscarPorCodigo(clienteJS.Localidade);
                                    cliente.Numero = clienteJS.Numero;
                                    cliente.Telefone1 = clienteJS.Telefone1;
                                }

                                cliente.CPF_CNPJ = cpfCnpj;
                                cliente.Email = clienteJS.Emails;
                                cliente.EmailContador = clienteJS.EmailsContador;
                                cliente.EmailContato = clienteJS.EmailsContato;
                                cliente.EmailContadorStatus = clienteJS.StatusEmailsContador ? "A" : "I";
                                cliente.EmailContatoStatus = clienteJS.StatusEmailsContato ? "A" : "I";
                                cliente.EmailStatus = clienteJS.StatusEmails ? "A" : "I";
                                cliente.IE_RG = clienteJS.RGIE.ToUpper() == "ISENTO" ? "ISENTO" : Utilidades.String.OnlyNumbers(clienteJS.RGIE);
                                cliente.InscricaoMunicipal = Utilidades.String.OnlyNumbers(clienteJS.IM);
                                cliente.Nome = clienteJS.RazaoSocial;
                                cliente.NomeFantasia = clienteJS.NomeFantasia;
                                cliente.Telefone2 = clienteJS.Telefone2;
                                cliente.Tipo = Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? "J" : "F";

                                if (cliente.CPF_CNPJ == 0f || !(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? Utilidades.Validate.ValidarCNPJ(clienteJS.CPFCNPJ) : Utilidades.Validate.ValidarCPF(clienteJS.CPFCNPJ)))
                                    erros.Add(string.Concat("CPF/CNPJ do ", tipo.ToString("G"), " inválida."));

                                if (cliente.Atividade == null)
                                    erros.Add(string.Concat("Atividade do ", tipo.ToString("G"), " inválida."));

                                if (string.IsNullOrWhiteSpace(cliente.IE_RG))
                                    erros.Add(string.Concat("IE do ", tipo.ToString("G"), " inválida."));

                                if (string.IsNullOrWhiteSpace(clienteJS.Bairro))
                                    erros.Add(string.Concat("Bairro do ", tipo.ToString("G"), " inválido."));

                                if (string.IsNullOrWhiteSpace(clienteJS.Endereco))
                                    erros.Add(string.Concat("Endereço do ", tipo.ToString("G"), " inválido."));

                                if (string.IsNullOrWhiteSpace(clienteJS.RazaoSocial))
                                    erros.Add(string.Concat("Nome/Razão Social do ", tipo.ToString("G"), " inválido."));

                                if (cliente.Localidade == null)
                                    erros.Add(string.Concat("Localidade do ", tipo.ToString("G"), " inválida."));

                                if (!string.IsNullOrWhiteSpace(cliente.Email))
                                {
                                    var emails = cliente.Email.Split(';');
                                    foreach (string email in emails)
                                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                            erros.Add(string.Concat("E-mail (", email, ") do ", tipo.ToString("G"), " inválido."));
                                }

                                if (!string.IsNullOrWhiteSpace(cliente.EmailContador))
                                {
                                    var emails = cliente.EmailContador.Split(';');
                                    foreach (string email in emails)
                                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                            erros.Add(string.Concat("E-mail do contador (", email, ") do ", tipo.ToString("G"), " inválido."));
                                }

                                if (!string.IsNullOrWhiteSpace(cliente.EmailContato))
                                {
                                    var emails = cliente.EmailContato.Split(';');
                                    foreach (string email in emails)
                                        if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                            erros.Add(string.Concat("E-mail do contato (", email, ") do ", tipo.ToString("G"), " inválido."));
                                }

                                if (erros.Count > 0)
                                    return erros;
                                else
                                {
                                    if (inserir)
                                    {
                                        if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                                        {
                                            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unidadeDeTrabalho);
                                            Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoas = repGrupoPessoas.BuscarPorRaizCNPJ(Utilidades.String.OnlyNumbers(cliente.CPF_CNPJ_Formatado).Remove(8, 6));
                                            if (grupoPessoas != null)
                                            {
                                                cliente.GrupoPessoas = grupoPessoas;
                                            }
                                        }
                                        cliente.DataCadastro = DateTime.Now;
                                        cliente.Ativo = true;
                                        repCliente.Inserir(cliente);
                                    }
                                    else
                                    {
                                        cliente.DataUltimaAtualizacao = DateTime.Now;
                                        cliente.Integrado = false;
                                        repCliente.Atualizar(cliente);
                                    }
                                }

                                nfse.SetarParticipante(cliente, tipo, endereco);
                            }
                            else
                            {
                                erros.Add(string.Concat("Falha ao obter CPF/CNPJ do ", tipo.ToString("G"), "."));
                            }
                        }
                    }
                    else
                    {
                        Repositorio.Pais repPais = new Repositorio.Pais(unidadeDeTrabalho);

                        Dominio.Entidades.Pais pais = repPais.BuscarPorSigla(clienteJS.SiglaPais);

                        if (string.IsNullOrWhiteSpace(clienteJS.Bairro))
                            erros.Add(string.Concat("Bairro do ", tipo.ToString("G"), " inválido."));

                        if (string.IsNullOrWhiteSpace(clienteJS.Cidade))
                            erros.Add(string.Concat("Cidade do ", tipo.ToString("G"), " inválido."));

                        if (string.IsNullOrWhiteSpace(clienteJS.Endereco))
                            erros.Add(string.Concat("Endereço do ", tipo.ToString("G"), " inválido."));

                        if (string.IsNullOrWhiteSpace(clienteJS.RazaoSocial))
                            erros.Add(string.Concat("Nome do ", tipo.ToString("G"), " inválido."));

                        if (string.IsNullOrWhiteSpace(clienteJS.Numero))
                            erros.Add(string.Concat("Número do ", tipo.ToString("G"), " inválido."));

                        if (pais == null)
                            erros.Add(string.Concat("País do ", tipo.ToString("G"), " inválido."));

                        if (!string.IsNullOrWhiteSpace(clienteJS.Emails))
                        {
                            var emails = clienteJS.Emails.Split(';');
                            foreach (string email in emails)
                                if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                    erros.Add(string.Concat("E-mail (", email, ") do ", tipo.ToString("G"), " inválido."));
                        }

                        if (erros.Count > 0)
                            return erros;

                        nfse.SetarParticipanteExportacao(clienteJS, tipo, pais);
                    }
                }
                else
                {
                    Dominio.Entidades.ParticipanteNFSe part = nfse.ObterParticipante(tipo);

                    if (part != null)
                    {
                        Repositorio.ParticipanteNFSe repParticipante = new Repositorio.ParticipanteNFSe(unidadeDeTrabalho);

                        nfse.SetarParticipante(null, tipo);

                        repParticipante.Deletar(part);
                    }
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                erros.Add(string.Concat("Inconsistência nos dados do ", tipo.ToString("G"), "."));
            }

            return erros;
        }

        private List<Dominio.ObjetosDeValor.NFSeCSV> CarregarDadosCSV(Stream stream, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var cellValue = "";
                string cnpjTomador = "";
                decimal valorServico = 0;
                int linha = 0;
                List<Dominio.ObjetosDeValor.NFSeCSV> listaNFSe = new List<Dominio.ObjetosDeValor.NFSeCSV>();

                StreamReader streamReader = new StreamReader(stream);

                while ((cellValue = streamReader.ReadLine()) != null)
                {
                    string[] linhaSeparada = cellValue.Split(';');
                    cnpjTomador = linhaSeparada[0];
                    if (cnpjTomador != "")
                    {
                        Dominio.ObjetosDeValor.NFSeCSV nfe = new Dominio.ObjetosDeValor.NFSeCSV();
                        nfe.CNPJTomador = cnpjTomador;
                        nfe.Servico = Utilidades.String.RemoveAllSpecialCharacters(linhaSeparada[1]);
                        decimal.TryParse(linhaSeparada[2], out valorServico);
                        nfe.Valor = valorServico;

                        listaNFSe.Add(nfe);
                    }
                    linha = linha + 1;
                }

                return listaNFSe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return null;
            }
        }

        private string ValidarClientesCSV(List<Dominio.ObjetosDeValor.NFSeCSV> listaNFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
            foreach (Dominio.ObjetosDeValor.NFSeCSV nfseCSV in listaNFSe)
            {
                double.TryParse(Utilidades.String.OnlyNumbers(nfseCSV.CNPJTomador), out double cnpjTomador);
                Dominio.Entidades.Cliente clienteTomador = repCliente.BuscarPorCPFCNPJ(cnpjTomador);
                if (cnpjTomador == 0 || clienteTomador == null)
                {
                    Servicos.Log.TratarErro("CSV NFSe: Cliente CNPJ " + nfseCSV.CNPJTomador + " não cadastrado.");
                    return "Cliente CNPJ " + nfseCSV.CNPJTomador + " não cadastrado.";
                }
            }

            return string.Empty;
        }


        private bool GerarNFSePorCSV(Dominio.Entidades.Empresa empresa, List<Dominio.ObjetosDeValor.NFSeCSV> listaNFSe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            foreach (Dominio.ObjetosDeValor.NFSeCSV nfseCSV in listaNFSe)
            {
                Dominio.ObjetosDeValor.NFSe.NFSe nfse = new Dominio.ObjetosDeValor.NFSe.NFSe();

                double.TryParse(Utilidades.String.OnlyNumbers(nfseCSV.CNPJTomador), out double cnpjTomador);

                Dominio.Entidades.Cliente clienteTomador = repCliente.BuscarPorCPFCNPJ(cnpjTomador);

                if (cnpjTomador > 0 && clienteTomador != null)
                {
                    nfse.CodigoIBGECidadePrestacaoServico = clienteTomador.Localidade?.CodigoIBGE ?? 0;
                    nfse.ISSRetido = false;
                    nfse.OutrasInformacoes = nfseCSV.Servico;

                    nfse.Emitente = new Dominio.ObjetosDeValor.CTe.Empresa()
                    {
                        CNPJ = empresa.CNPJ,
                        Atualizar = false
                    };

                    nfse.Tomador = new Dominio.ObjetosDeValor.CTe.Cliente()
                    {
                        Bairro = clienteTomador.Bairro,
                        CEP = clienteTomador.CEP,
                        CodigoAtividade = clienteTomador.Atividade.Codigo,
                        CodigoIBGECidade = clienteTomador.Localidade.CodigoIBGE,
                        CodigoPais = clienteTomador.Localidade.Estado.Pais.Codigo.ToString(),
                        Complemento = clienteTomador.Complemento,
                        CPFCNPJ = clienteTomador.CPF_CNPJ_SemFormato,
                        Endereco = clienteTomador.Endereco,
                        Exportacao = false,
                        NomeFantasia = clienteTomador.NomeFantasia,
                        RazaoSocial = clienteTomador.Nome,
                        RGIE = clienteTomador.IE_RG,
                        Numero = !string.IsNullOrWhiteSpace(clienteTomador.Numero) && clienteTomador.Numero.Length > 2 ? clienteTomador.Numero : "S/N",
                        Telefone1 = clienteTomador.Telefone1,
                        Telefone2 = clienteTomador.Telefone2
                    };

                    nfse.Itens = new List<Dominio.ObjetosDeValor.NFSe.Item>();

                    Servicos.NFSe servicoNFSe = new Servicos.NFSe(unidadeDeTrabalho);
                    Dominio.Entidades.ServicoNFSe servicoMultiCTe = servicoNFSe.ObterServicoNFSe(empresa, empresa.Localidade.CodigoIBGE != nfse.CodigoIBGECidadePrestacaoServico, nfse.CodigoIBGECidadePrestacaoServico, unidadeDeTrabalho);

                    Dominio.ObjetosDeValor.NFSe.Servico servico = null;
                    if (servicoMultiCTe != null)
                    {
                        servico = new Dominio.ObjetosDeValor.NFSe.Servico();
                        servico.Numero = servicoMultiCTe.Numero;
                        servico.Descricao = !string.IsNullOrWhiteSpace(nfseCSV.Servico) ? Utilidades.String.Left(nfseCSV.Servico, 100) : servicoMultiCTe.Descricao;
                        servico.Aliquota = servicoMultiCTe.Aliquota;
                        servico.CNAE = servicoMultiCTe.CNAE;
                        servico.CodigoTributacao = servicoMultiCTe.CodigoTributacao;
                    }

                    nfse.Itens.Add(new Dominio.ObjetosDeValor.NFSe.Item()
                    {
                        CodigoIBGECidade = nfse.CodigoIBGECidadePrestacaoServico,
                        CodigoIBGECidadeIncidencia = nfse.CodigoIBGECidadePrestacaoServico,
                        Discriminacao = "",
                        CodigoPaisPrestacaoServico = int.Parse(clienteTomador.Localidade.Pais.Sigla),
                        Quantidade = 1,
                        ServicoPrestadoNoPais = true,
                        ExigibilidadeISS = 1,
                        ValorServico = nfseCSV.Valor,
                        BaseCalculoISS = nfseCSV.Valor,
                        AliquotaISS = servicoMultiCTe?.Aliquota ?? 0,
                        ValorISS = servicoMultiCTe != null ? empresa.OptanteSimplesNacional && empresa.Localidade.CodigoIBGE == 3518800 ? 0 : nfseCSV.Valor * servicoMultiCTe.Aliquota / 100 : 0, //Guarulhos deve enviar valor zerado quando transp for do simples
                        ValorTotal = nfseCSV.Valor,
                        Servico = servico
                    });

                    Servicos.NFSe svcNFSe = new Servicos.NFSe(unidadeDeTrabalho);

                    Dominio.Entidades.NFSe nfseIntegrada = svcNFSe.GerarNFSePorObjeto(nfse, unidadeDeTrabalho, Dominio.Enumeradores.StatusNFSe.EmDigitacao);
                }
                else
                {
                    Servicos.Log.TratarErro("Importação CSV NFSe: Cliente CNPJ " + nfseCSV.CNPJTomador + " não cadastrado.");
                }

            }

            return true;
        }

        #endregion
    }
}
