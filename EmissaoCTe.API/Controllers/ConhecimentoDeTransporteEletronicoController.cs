using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using Repositorio;
using Servicos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EmissaoCTe.API.Controllers
{
    public class ConhecimentoDeTransporteEletronicoController : ApiController
    {
        #region Variaveis Globais
        public static bool ThreadExecutada = false;
        public static bool Sucesso = false;
        public static Dominio.Entidades.LayoutEDI LayoutEDI = null;
        public static Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis NOTFIS = null;
        public static Stream ArquivoEDI = null;

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("emissaocte.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Metodos Publicos

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros, serie, numeroInicial, numeroFinal = 0, fimRegistros;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out fimRegistros);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out serie);

                if (fimRegistros == 0)
                    fimRegistros = 20;

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                bool contem = false;
                bool.TryParse(Request.Params["Contem"], out contem);

                string placa = Request.Params["Placa"];
                string motorista = Request.Params["Motorista"];
                string status = Request.Params["Status"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNF = Request.Params["NumeroNF"];
                if (!string.IsNullOrWhiteSpace(numeroNF))
                    numeroNF = Utilidades.String.OnlyNumbers(numeroNF);

                Dominio.Enumeradores.TipoCTE tipoCTe;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = null;
                if (Enum.TryParse(Request.Params["AverbacaoCTe"], out Dominio.Enumeradores.FiltroAverbacaoCTe averbacaoCTeAux))
                    averbacaoCTe = averbacaoCTeAux;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, inicioRegistros, fimRegistros);
                int countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe);

                unidadeDeTrabalho.CommitChanges();

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(cte.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   cte.Codigo,
                                   cte.Status,
                                   cte.Numero,
                                   Serie = cte.Serie,
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   cte.DescricaoTipoServico,
                                   Placa = cte.Placa ?? string.Empty,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais != null ? cte.Remetente.Pais.Nome : "EXTERIOR") : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais != null ? cte.Destinatario.Pais.Nome : "EXTERIOR") : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : cte.TerminoPrestacao,
                                   Valor = string.Format("{0:n2}", cte.Valor),
                                   cte.DescricaoStatus,
                                   MensagemRetornoSefaz = cte.Status == "E"
                                        ? "CT-e em processamento."
                                        : !string.IsNullOrWhiteSpace(cte.MensagemRetornoSefaz)
                                            ? HttpUtility.HtmlEncode(ExtrairMensagemDeJson(cte.MensagemRetornoSefaz))
                                            : string.Empty
                               }).ToList();

                return Json(retorno, true, null, new string[] { "CodigoCriptografado", "Codigo", "Status", "Núm.|5", "Série|4", "Emissão|6", "Tipo|6", "Placa|8", "Remetente|11", "Loc. Remet.|9", "Destinatário|11", "Loc. Destin.|9", "Valor|5", "Status|8", "Retorno Sefaz|11" }, countCTes);
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
        public ActionResult ConsultarAdmin()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                // Reutilizado o mesmo metodo do consultar, so foi mantido as variaveis para não ficar confuso
                // Mesmo que elas estejam apenas inicializadas
                int serie = 0, numeroInicial = 0, numeroFinal = 0;
                int[] series = new int[] { };

                bool contem = false;
                string cpfCnpjRemetente = string.Empty;
                string cpfCnpjDestinatario = string.Empty;
                string placa = string.Empty;
                string motorista = string.Empty;
                string tipoOcorrencia = string.Empty;
                string numeroNF = string.Empty;

                Dominio.Enumeradores.TipoCTE tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                // Repositorio
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                // Filtros
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int empresa = 0;
                int.TryParse(Request.Params["Empresa"], out empresa);

                int numeroCarga = 0;
                int.TryParse(Request.Params["NumeroCarga"], out numeroCarga);

                string status = Request.Params["Status"] ?? string.Empty;

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = null;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = repCTe.ConsultarAdmin(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, series, serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, inicioRegistros, 50, this.EmpresaUsuario.Codigo, numeroCarga);
                int countCTes = repCTe.ContarConsultaAdmin(empresa, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, series, serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, this.EmpresaUsuario.Codigo, numeroCarga);

                unidadeDeTrabalho.CommitChanges();

                var retorno = (from cte in listaCTes
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(cte.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   cte.Codigo,
                                   cte.Status,
                                   cte.Empresa,
                                   cte.Numero,
                                   Serie = cte.Serie,
                                   NomeEmpresa = cte.NomeEmpresa,
                                   DataEmissao = cte.DataEmissao.HasValue ? cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   cte.DescricaoTipoServico,
                                   Placa = cte.Placa ?? string.Empty,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Exterior ? string.Concat(cte.Remetente.Cidade, " / ", cte.Remetente.Pais != null ? cte.Remetente.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Remetente.Localidade.Estado.Sigla, " / ", cte.Remetente.Localidade.Descricao) : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Exterior ? string.Concat(cte.Destinatario.Cidade, " / ", cte.Destinatario.Pais != null ? cte.Destinatario.Pais.Nome : "EXPORTACAO") : string.Concat(cte.Destinatario.Localidade.Estado.Sigla, " / ", cte.Destinatario.Localidade.Descricao) : cte.TerminoPrestacao,
                                   Valor = string.Format("{0:n2}", cte.Valor),
                                   DescricaoStatus = cte.StatusIntegracao == "5" && cte.Status == "P" ? "Aguardando confirmação embarcador para processar protocolo " + cte.Codigo.ToString() : cte.DescricaoStatus,
                                   MensagemRetornoSefaz = cte.Status == "E" ? "CT-e em processamento." : cte.MensagemStatus == null ? string.IsNullOrEmpty(cte.MensagemRetornoSefaz) ? string.Empty : System.Web.HttpUtility.HtmlEncode(cte.MensagemRetornoSefaz) : cte.MensagemStatus.MensagemDoErro
                               }).ToList();

                return Json(retorno, true, null, new string[] { "CodigoCriptografado", "Codigo", "Status", "Empresa", "Núm.|5", "Série|4", "Empresa|10", "Emissão|6", "Tipo|6", "Placa|8", "Remetente|11", "Loc. Remet.|9", "Destinatário|11", "Loc. Destin.|9", "Valor|5", "Status|8", "Retorno Sefaz|11" }, countCTes);
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
        public ActionResult ConsultarRetornosSefaz()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.CTeRetornoSefaz repRetornoSefaz = new Repositorio.CTeRetornoSefaz(unidadeDeTrabalho);
                List<Dominio.Entidades.CTeRetornoSefaz> listaRetornosSefaz = repRetornoSefaz.BuscarPorCTe(codigoCTe);

                var retorno = (from retornoSefaz in listaRetornosSefaz
                               select new
                               {
                                   Codigo = retornoSefaz.Codigo,
                                   Data = retornoSefaz.DataHora.ToString("dd/MM/yyyy HH:mm"),
                                   DescricaoTipo = retornoSefaz.DescricaoTipo,
                                   RetornoSefaz = retornoSefaz.ErroSefaz != null ? retornoSefaz.ErroSefaz.CodigoDoErro.ToString() + " - " + retornoSefaz.ErroSefaz.MensagemDoErro : System.Web.HttpUtility.HtmlEncode(Utilidades.String.ReplaceInvalidCharacters(retornoSefaz.MensagemRetorno))
                               }).OrderByDescending(o => o.Codigo).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Tipo|10", "Retorno Sefaz|50" }, listaRetornosSefaz.Count());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as retornos sefaz.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarMDFeListaCTes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                List<string> listaCTes = JsonConvert.DeserializeObject<List<string>>(Request.Params["ListaCTes"]);

                string usuario = "";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    var codigoCTe = 0;
                    int.TryParse(listaCTes[i], out codigoCTe);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    if (cte != null)
                    {
                        ctes.Add(cte);

                    }
                }

                if (ctes == null || ctes.Count == 0 || ctes.Count != listaCTes.Count)
                    return Json<bool>(false, false, "Não foi possível carregar todos CT-es para geração do MDF-e.");
                if (ctes[0].Veiculos == null || ctes[0].Veiculos.Count == 0)
                    return Json<bool>(false, false, "CT-e selecionado não possui veículo para geração do MDF-e.");
                if (ctes[0].Motoristas == null || ctes[0].Motoristas.Count == 0)
                    return Json<bool>(false, false, "CT-e selecionado não possui motorista para geração do MDF-e.");

                var veiculo = (from obj in ctes[0].Veiculos where obj.Veiculo.TipoVeiculo == "0" select obj.Veiculo).FirstOrDefault();
                if (veiculo == null)
                    return Json<bool>(false, false, "CT-e selecionado não possui veículo do tipo Tração para geração do MDF-e.");

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repMDFe.BuscarPorPlacaEStatus(this.EmpresaUsuario.CNPJ, veiculo.Placa, Dominio.Enumeradores.StatusMDFe.Autorizado);

                    if (mdfeAnterior != null)
                        return Json<bool>(false, false, "Existe o MDF-e " + mdfeAnterior.Numero + "/" + mdfeAnterior.Serie.Numero + " Emissor " + mdfeAnterior.Empresa.CNPJ + " Chave " + mdfeAnterior.Chave + " autorizado com a placa " + veiculo.Placa + ". É necessário encerra-lo para emitir novo MDF-e.");
                }


                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.AguardarAverbacaoCTeParaEmitirMDFe)
                {
                    Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
                    for (var i = 0; i < listaCTes.Count; i++)
                    {
                        var codigoCTe = 0;
                        int.TryParse(listaCTes[i], out codigoCTe);
                        List<Dominio.Entidades.AverbacaoCTe> listaAverbacoesPendentes = repAverbacaoCTe.BuscarPorCTesAutorizadosESituacao(this.EmpresaUsuario.Codigo, codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso);
                        if (listaAverbacoesPendentes == null || listaAverbacoesPendentes.Count <= 0)
                        {
                            listaAverbacoesPendentes = repAverbacaoCTe.BuscarPorCTesAutorizadosESituacao(this.EmpresaUsuario.Codigo, codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe.Pendente);
                            if (listaAverbacoesPendentes != null && listaAverbacoesPendentes.Count > 0)
                                return Json<bool>(false, false, "Existem CT-es pendente de retorno da averbação. Por favor, aguarde e tente novamente.");
                            listaAverbacoesPendentes = repAverbacaoCTe.BuscarPorCTesAutorizadosESituacao(this.EmpresaUsuario.Codigo, codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe.Enviado);
                            if (listaAverbacoesPendentes != null && listaAverbacoesPendentes.Count > 0)
                                return Json<bool>(false, false, "Existem CT-es pendente de retorno da averbação. Por favor, aguarde e tente novamente.");
                            listaAverbacoesPendentes = repAverbacaoCTe.BuscarPorCTesAutorizadosESituacao(this.EmpresaUsuario.Codigo, codigoCTe, Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao);
                            if (listaAverbacoesPendentes != null && listaAverbacoesPendentes.Count > 0)
                                return Json<bool>(false, false, "Existem CT-es com averbação rejeitadas. Por favor, verifique e tente novamente.");

                            return Json<bool>(false, false, "Não foi encontrado registro de averbação para alguns CT-es. Por favor, verifique e tente novamente.");
                        }
                    }
                }

                if (this.UsuarioAdministrativo != null)
                    usuario = string.Concat(this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome);
                else
                    usuario = string.Concat(this.Usuario.CPF, " - ", this.Usuario.Nome);

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unitOfWork.Start(System.Data.IsolationLevel.Serializable);
                else
                    unitOfWork.Start();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = svcMDFe.GerarMDFePorCTes(this.EmpresaUsuario, ctes, unitOfWork, null, null, usuario, null, null, null, this.Usuario.Codigo);

                var validarInformacoesDePagamento = ctes != null && ctes.Count == 1;

                if (validarInformacoesDePagamento)
                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmDigitacao;

                unitOfWork.CommitChanges();

                if (!validarInformacoesDePagamento)
                {
                    if (svcMDFe.Emitir(mdfe, unitOfWork))
                    {
                        if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        {
                            FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, "Ocorreu uma falha e o MDF-e não pode ser emitido. Tente novamente.");
                    }
                }

                return Json(new { docImportadoAoIntegrador = !validarInformacoesDePagamento }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarMDFesEmitidos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoCTe"]), out codigoCTe);

                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTeMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                List<int> listaCodigoMDFe = repCTeMDFe.BuscarCodigoDeMDFesPorCTe(codigoCTe);
                listaCodigoMDFe = listaCodigoMDFe.OrderByDescending(p => p).ToList();

                int countDocumentos = listaCodigoMDFe.Count();

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = new List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais>();

                for (var i = 0; i < countDocumentos; i++)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais manifesto = repMDFe.BuscarPorCodigo(listaCodigoMDFe[i]);
                    if (manifesto != null)
                        mdfes.Add(manifesto);
                }

                var retorno = (from mdfe in mdfes
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(mdfe.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   CodigoEmpresa = mdfe.Empresa.Codigo,
                                   mdfe.Codigo,
                                   mdfe.Status,
                                   mdfe.Numero,
                                   Serie = mdfe.Serie.Numero,
                                   DataEmissao = mdfe.DataEmissao.HasValue ? mdfe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   mdfe.DescricaoStatus,
                                   MensagemRetornoSefaz = mdfe.MensagemStatus == null ? string.IsNullOrEmpty(mdfe.MensagemRetornoSefaz) ? string.Empty : System.Web.HttpUtility.HtmlEncode(mdfe.MensagemRetornoSefaz) : mdfe.MensagemStatus.MensagemDoErro
                               }).ToList();

                return Json(retorno, true, null, new string[] { "CodigoCriptografado", "CodigoEmpresa", "Codigo", "Status", "Número|15", "Série|10", "Emissão|15", "Status|15", "Retorno Sefaz|30" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es emitidos para o CTe.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPorTipo()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                int numero = 0;
                int.TryParse(Request.Params["Numero"], out numero);

                DateTime dataEmissao;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                Dominio.Enumeradores.TipoCTE? tipoCTe = null;
                Dominio.Enumeradores.TipoCTE tipoCTeAux;
                if (Enum.TryParse(Request.Params["TipoCTe"], out tipoCTeAux))
                    tipoCTe = tipoCTeAux;

                string numeroDocumento = Request.Params["NumeroDocumento"];

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarPorTipo(this.EmpresaUsuario.Codigo, dataEmissao, numero, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento, inicioRegistros, 50, "");
                int countCTes = repCTe.ContarConsultaPorTipo(this.EmpresaUsuario.Codigo, dataEmissao, numero, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Select(o => o.Codigo).ToArray(), numeroDocumento);

                unidadeDeTrabalho.CommitChanges();

                var retorno = (from obj in listaCTes
                               select new
                               {
                                   obj.ValorFrete,
                                   obj.Codigo,
                                   obj.Chave,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                   obj.DescricaoTipoServico,
                                   obj.DescricaoTipoCTE,
                                   Destinatario = obj.Destinatario != null ? obj.Destinatario.Nome : string.Empty,
                                   obj.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Valor do Frete", "Codigo", "Chave", "Núm.|10", "Série|10", "Emissão|10", "Tipo|15", "Finalid.|15", "Destinatário|20", "Status|10" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                unidadeDeTrabalho.Rollback();

                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarImportadosPortal()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                // Filtros
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["Numero"], out int numero);

                string cnpjEmitente = Request.Params["CNPJEmitente"] ?? string.Empty;

                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissao);

                // Consultar
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarImportadosPortal(this.EmpresaUsuario.Codigo, numero, cnpjEmitente, dataEmissao, inicioRegistros, 50);
                int countCTes = repCTe.ContarConsultaImportadosPortal(this.EmpresaUsuario.Codigo, numero, cnpjEmitente, dataEmissao);

                // Formata
                var retorno = (from obj in listaCTes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Chave,
                                   Numero = obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : string.Empty,
                                   Emitente = obj.ModeloDocumentoFiscal.Numero == "57" ? obj.Empresa.RazaoSocial : obj.Intermediario?.Nome,
                                   TerminoPrestacao = LocalidadePrestacaoRelatorio("LOCTERMINO", obj.Codigo, unitOfWork),
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Chave", "Número|15", "Série|10", "Emissão|15", "Emitente|25", "Destino|25" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDacte()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : this.EmpresaUsuario.Codigo, codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não encontrado, atualize a página e tente novamente.");

                if (cte.Status != "A" && cte.Status != "C" && cte.Status != "K" && cte.Status != "F" && cte.Status != "Q")
                    return Json<bool>(false, false, "O status do CT-e não permite a geração do DACTE.");

                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatorios"]))
                    return Json<bool>(false, false, "O caminho para os download da DACTE não está disponível. Contate o suporte técnico.");

                string caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoRelatorios"], cte.Empresa.CNPJ, cte.Chave);

                string gerarDacteCancelamento = ConfigurationManager.AppSettings["GerarDACTECancelamento"];

                if (cte.Status == "A" || cte.Status == "C" || cte.Status == "K")
                    caminhoPDF = caminhoPDF + ".pdf";
                else if (!string.IsNullOrWhiteSpace(cte.ChaveContingencia) && cte.TipoEmissao == "5") //FSDA
                    caminhoPDF = caminhoPDF + "_FSDA.pdf";
                else if (!string.IsNullOrWhiteSpace(cte.ChaveContingencia) && cte.TipoEmissao == "4") //EPEC
                    caminhoPDF = caminhoPDF + "_EPEC.pdf";

                byte[] dacte = null;

                long tamanhoPDF = 0;
                if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    FileInfo file = new FileInfo(caminhoPDF);
                    tamanhoPDF = file.Length;
                }

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) || tamanhoPDF <= 0 || (cte.Status == "C" && gerarDacteCancelamento == "SIM"))
                {
                    ////Buscar DACTE do Oracle
                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) && ConfigurationManager.AppSettings["RegerarDACTEOracle"] != "NAO" && cte.CodigoCTeIntegrador > 0)
                    {
                        Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);
                        servicoCTe.ObterESalvarDACTE(cte, cte.Empresa.Codigo, null, unidadeDeTrabalho);

                        if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                        {
                            FileInfo file = new FileInfo(caminhoPDF);
                            tamanhoPDF = file.Length;
                        }
                    }

                    if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF) || tamanhoPDF <= 0 || (cte.Status == "C" && gerarDacteCancelamento == "SIM"))
                    {
                        if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoGeradorRelatorios"]))
                            return Json<bool>(false, false, "O gerador da DACTE não está disponível. Contate o suporte técnico.");

                        Servicos.DACTE svcDACTE = new Servicos.DACTE(unidadeDeTrabalho);

                        dacte = svcDACTE.GerarPorProcesso(cte.Codigo);
                    }
                    else
                        dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }
                else
                {
                    dacte = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                var nomePDF = "";
                if (EmpresaUsuario.Configuracao != null && EmpresaUsuario.Configuracao.NomePDFCTe != null && EmpresaUsuario.Configuracao.NomePDFCTe != "")
                {
                    var tamanhoTag = 0;
                    var stringTag = "";

                    nomePDF = EmpresaUsuario.Configuracao.NomePDFCTe;
                    if (nomePDF.IndexOf("#CNPJTransportador##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#CNPJTransportador##TamanhoTag#"), "#CNPJTransportador##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#CNPJTransportador##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#CNPJTransportador##TamanhoTag#") + 19, 14);

                        nomePDF = nomePDF.Replace("#CNPJTransportador#", (cte.Empresa != null ? cte.Empresa.CNPJ.Length > tamanhoTag ? cte.Empresa.CNPJ.Substring(0, tamanhoTag) : cte.Empresa.CNPJ : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#CNPJTransportador#", (cte.Empresa != null ? cte.Empresa.CNPJ : string.Empty));

                    if (nomePDF.IndexOf("#NomeTransportador##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#NomeTransportador##TamanhoTag#"), "#NomeTransportador##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#NomeTransportador##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#NomeTransportador##TamanhoTag#") + 19, 14);

                        nomePDF = nomePDF.Replace("#NomeTransportador#", (cte.Empresa != null ? cte.Empresa.NomeFantasia.Length > tamanhoTag ? cte.Empresa.NomeFantasia.Substring(0, tamanhoTag) : cte.Empresa.NomeFantasia : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#NomeTransportador#", (cte.Empresa != null ? cte.Empresa.NomeFantasia : string.Empty));

                    nomePDF = nomePDF.Replace("#NumeroCTe#", (cte != null ? cte.Numero.ToString() : string.Empty));
                    nomePDF = nomePDF.Replace("#SerieCTe#", (cte.Serie != null ? cte.Serie.Numero.ToString() : string.Empty));
                    nomePDF = nomePDF.Replace("#ChaveCTe#", (cte != null ? cte.Chave : string.Empty));
                    nomePDF = nomePDF.Replace("#PlacaVeiculo#", (cte.Veiculos.FirstOrDefault() != null ? cte.Veiculos.FirstOrDefault().Placa : string.Empty));

                    if (nomePDF.IndexOf("#NomeMotorista##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#NomeMotorista##TamanhoTag#"), "#NomeMotorista##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#NomeMotorista##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#NomeMotorista##TamanhoTag#") + 15, 14);

                        nomePDF = nomePDF.Replace("#NomeMotorista#", (cte.Motoristas.FirstOrDefault() != null ? cte.Motoristas.FirstOrDefault().NomeMotorista.Length > tamanhoTag ? cte.Motoristas.FirstOrDefault().NomeMotorista.Substring(0, tamanhoTag) : cte.Motoristas.FirstOrDefault().NomeMotorista : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#NomeMotorista#", (cte.Motoristas.FirstOrDefault() != null ? cte.Motoristas.FirstOrDefault().NomeMotorista : string.Empty));

                    if (nomePDF.IndexOf("#ClienteRemetente##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#ClienteRemetente##TamanhoTag#"), "#ClienteRemetente##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#ClienteRemetente##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#ClienteRemetente##TamanhoTag#") + 18, 14);

                        nomePDF = nomePDF.Replace("#ClienteRemetente#", (cte.Remetente != null ? cte.Remetente.Nome.Length > tamanhoTag ? cte.Remetente.Nome.Substring(0, tamanhoTag) : cte.Remetente.Nome : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#ClienteRemetente#", (cte.Remetente != null ? cte.Remetente.Nome : string.Empty));

                    if (nomePDF.IndexOf("#ClienteDestinatario##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#ClienteDestinatario##TamanhoTag#"), "#ClienteDestinatario##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#ClienteDestinatario##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#ClienteDestinatario##TamanhoTag#") + 21, 14);

                        nomePDF = nomePDF.Replace("#ClienteDestinatario#", (cte.Destinatario != null ? cte.Destinatario.Nome.Length > tamanhoTag ? cte.Destinatario.Nome.Substring(0, tamanhoTag) : cte.Destinatario.Nome : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#ClienteDestinatario#", (cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty));

                    if (nomePDF.IndexOf("#CidadeOrigem##TamanhoTag#") > 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#CidadeOrigem##TamanhoTag#"), "#CidadeOrigem##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#CidadeOrigem##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#CidadeOrigem##TamanhoTag#") + 14, 14);

                        nomePDF = nomePDF.Replace("#CidadeOrigem#", (cte.LocalidadeInicioPrestacao != null ? cte.LocalidadeInicioPrestacao.Descricao.Length > tamanhoTag ? cte.LocalidadeInicioPrestacao.Descricao.Substring(0, tamanhoTag) : cte.LocalidadeInicioPrestacao.Descricao : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#CidadeOrigem#", (cte.LocalidadeInicioPrestacao != null ? cte.LocalidadeInicioPrestacao.Descricao : string.Empty));
                    nomePDF = nomePDF.Replace("#UFOrigem#", (cte.LocalidadeInicioPrestacao != null ? cte.LocalidadeInicioPrestacao.Estado.Sigla : string.Empty));

                    if (nomePDF.IndexOf("#CidadeDestino##TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#CidadeDestino##TamanhoTag#"), "#CidadeDestino##TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#CidadeDestino##TamanhoTag#".Length, 1));
                        nomePDF = nomePDF.Remove(nomePDF.IndexOf("#CidadeDestino##TamanhoTag#") + 15, 14);

                        nomePDF = nomePDF.Replace("#CidadeDestino#", (cte.LocalidadeTerminoPrestacao != null ? cte.LocalidadeTerminoPrestacao.Descricao.Length > tamanhoTag ? cte.LocalidadeTerminoPrestacao.Descricao.Substring(0, tamanhoTag) : cte.LocalidadeTerminoPrestacao.Descricao : string.Empty));
                    }
                    else
                        nomePDF = nomePDF.Replace("#CidadeDestino#", (cte.LocalidadeTerminoPrestacao != null ? cte.LocalidadeTerminoPrestacao.Descricao : string.Empty));
                    nomePDF = nomePDF.Replace("#UFDestino#", (cte.LocalidadeTerminoPrestacao != null ? cte.LocalidadeTerminoPrestacao.Estado.Sigla : string.Empty));

                    while (nomePDF.IndexOf("#TamanhoTag#") >= 0)
                    {
                        stringTag = nomePDF.Substring(nomePDF.IndexOf("#TamanhoTag#"), "#TamanhoTag#".Length + 1);
                        tamanhoTag = int.Parse(stringTag.Substring("#TamanhoTag#".Length, 1));
                        stringTag = "#TamanhoTag#" + tamanhoTag.ToString() + "#";
                        nomePDF = nomePDF.Replace(stringTag, string.Empty);
                    }
                    if (nomePDF == "")
                        nomePDF = System.IO.Path.GetFileName(caminhoPDF);
                    else nomePDF = nomePDF + ".pdf";
                }
                else nomePDF = System.IO.Path.GetFileName(caminhoPDF);

                if (dacte != null)
                {
                    try
                    {
                        return Arquivo(dacte, "application/pdf", nomePDF);
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(ex);
                        nomePDF = System.IO.Path.GetFileName(caminhoPDF);
                        return Arquivo(dacte, "application/pdf", nomePDF);
                    }
                }
                else
                    return Json<bool>(false, false, "Não foi possível gerar o DACTE, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download da DACTE.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXML()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : this.EmpresaUsuario.Codigo, codigoCTe);

                    if (cte != null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                        byte[] data = svcCTe.ObterXMLAutorizacao(cte);

                        if (data != null)
                        {
                            return Arquivo(data, "text/xml", string.Concat(cte.Chave, ".xml"));
                        }
                    }
                }

                return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLCancelamento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe, codigoEmpresa = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoEmpresa"], out codigoEmpresa);

                if (codigoCTe > 0)
                {

                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoEmpresa > 0 ? codigoEmpresa : this.EmpresaUsuario.Codigo, codigoCTe);

                    if (cte != null)
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                        byte[] data = svcCTe.ObterXMLCancelamento(cte, unidadeDeTrabalho);

                        if (data != null)
                        {
                            if (cte.Status == "I")
                                return Arquivo(data, "text/xml", string.Concat(cte.Numero, "_", cte.Serie.Numero, "_procInutCTe.xml"));
                            else
                                return Arquivo(data, "text/xml", string.Concat(cte.Chave, "_procCancCTe.xml"));
                        }
                    }

                }

                return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SalvarDados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Dominio.Entidades.ConhecimentoDeTransporteEletronico CTE = null;
            try
            {
                List<string> listaErros = new List<string>();

                int codigoCTE, tipoEmissaoCTe, serie, modelo, naturezaDaOperacao, modalTransporte, cfop,
                    municipioLocalEmissaoCTe, municipioInicioPrestacao, municipioTerminoPrestacao, codigoColeta, tipoEnvio = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTE);
                int.TryParse(Request.Params["TipoEmissaoCTe"], out tipoEmissaoCTe);
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NaturezaDaOperacao"], out naturezaDaOperacao);
                int.TryParse(Request.Params["CFOP"], out cfop);
                int.TryParse(Request.Params["ModalTransporte"], out modalTransporte);
                int.TryParse(Request.Params["MunicipioLocalEmissaoCTe"], out municipioLocalEmissaoCTe);
                int.TryParse(Request.Params["MunicipioInicioPrestacao"], out municipioInicioPrestacao);
                int.TryParse(Request.Params["MunicipioTerminoPrestacao"], out municipioTerminoPrestacao);
                int.TryParse(Request.Params["Modelo"], out modelo);
                int.TryParse(Request.Params["CodigoColeta"], out codigoColeta);
                int.TryParse(Request.Params["TipoEnvio"], out tipoEnvio);
                int.TryParse(Request.Params["CodigoCTEReferenciado"], out int codigoCTEReferenciado);
                int.TryParse(Request.Params["SubstituicaoTomador"], out int substituicaoTomador);

                Dominio.Enumeradores.TipoPagamento tipoPagamento;
                Dominio.Enumeradores.TipoServico tipoServico;
                Dominio.Enumeradores.TipoCTE tipoCTE;
                Dominio.Enumeradores.TipoImpressao tipoImpressao;
                Dominio.Enumeradores.TipoTomador tipoTomador;
                Dominio.Enumeradores.TipoCOFINS CSTCOFINS;
                Dominio.Enumeradores.TipoPIS CSTPIS;
                Dominio.Enumeradores.TipoICMS ICMS;
                Dominio.Enumeradores.OpcaoSimNao indicadorGlobalizado;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE indicadorIETomador;

                Enum.TryParse<Dominio.Enumeradores.TipoPagamento>(Request.Params["FormaPagamento"], out tipoPagamento);
                Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["TipoCTE"], out tipoCTE);
                Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["TipoServico"], out tipoServico);
                Enum.TryParse<Dominio.Enumeradores.TipoImpressao>(Request.Params["FormaImpressao"], out tipoImpressao);
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TomadorServico"], out tipoTomador);
                Enum.TryParse<Dominio.Enumeradores.TipoICMS>(Request.Params["ICMS"], out ICMS);
                Enum.TryParse<Dominio.Enumeradores.TipoPIS>(Request.Params["CSTPIS"], out CSTPIS);
                Enum.TryParse<Dominio.Enumeradores.TipoCOFINS>(Request.Params["CSTCOFINS"], out CSTCOFINS);
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IndicadorGlobalizado"], out indicadorGlobalizado);
                Enum.TryParse<Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE>(Request.Params["IndicadorIETomador"], out indicadorIETomador);
                Enum.TryParse<Dominio.Enumeradores.TipoFretamento>(Request.Params["TipoFretamento"], out Dominio.Enumeradores.TipoFretamento tipoFretamento);
                Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IndNegociavel"], out Dominio.Enumeradores.OpcaoSimNao indNegociavel);

                string detalhesRetiradaRecebedor = Request.Params["DetalhesRetiradaRecebedor"];
                string CIOT = Request.Params["CIOT"];
                string numeroCTRB = Request.Params["NumeroCTRB"];
                string serieCTRB = Request.Params["SerieCTRB"];
                string RNTRC = Request.Params["RNTRC"];
                string numeroLacre = Request.Params["NumeroLacre"];
                string conteiner = Request.Params["Conteiner"];
                string outrasCaracteristicasCarga = Request.Params["OutrasCaracteristicasCarga"];
                string produtoPredominante = Request.Params["ProdutoPredominante"];
                string observacoesGerais = Request.Params["ObservacoesGerais"];
                string formaEmissao = Request.Params["FormaEmissao"];
                string observacaoDigitacao = Request.Params["ObservacaoDigitacao"];
                string informacaoAdicionalFisco = Request.Params["InformacaoAdicionalFisco"];
                string caracteristicaAdicionalTransporte = Request.Params["CaracteristicaAdicionalTransporte"];
                string caracteristicaAdicionalServico = Request.Params["CaracteristicaAdicionalServico"];
                string descricaoComplemento = Request.Params["DescricaoComplemento"];
                string codigoBeneficio = Request.Params["CodigoBeneficio"];


                bool indicadorLotacao, recebedorRetiraDestino, incluirICMSNoFrete, exibeICMSNaDacte, duplicado;
                bool.TryParse(Request.Params["RecebedorRetiraDestino"], out recebedorRetiraDestino);
                bool.TryParse(Request.Params["IndicadorLotacao"], out indicadorLotacao);
                bool.TryParse(Request.Params["IncluirICMSNoFrete"], out incluirICMSNoFrete);
                bool.TryParse(Request.Params["ExibeICMSNaDacte"], out exibeICMSNaDacte);
                bool.TryParse(Request.Params["Duplicado"], out duplicado);

                decimal valorTotalCarga, valorCargaAverbacao, valorCOFINS, aliquotaCOFINS, valorBaseCalculoCOFINS, valorPIS, aliquotaPIS, valorBaseCalculoPIS, valorCreditoICMS,
                        valorICMS, aliquotaICMS, valorBaseCalculoICMS, reducaoBaseCalculoICMS, percentualICMSRecolhido, valorAReceber, valorTotalPrestacaoServico,
                        valorFreteContratado, valorINSS, valorIR, valorCSLL;
                decimal.TryParse(Request.Params["AliquotaICMS"], out aliquotaICMS);
                decimal.TryParse(Request.Params["ValorAReceber"], out valorAReceber);
                decimal.TryParse(Request.Params["ValorICMS"], out valorICMS);
                decimal.TryParse(Request.Params["ValorTotalPrestacaoServico"], out valorTotalPrestacaoServico);
                decimal.TryParse(Request.Params["ValorBaseCalculoICMS"], out valorBaseCalculoICMS);
                decimal.TryParse(Request.Params["ValorTotalCarga"], out valorTotalCarga);
                decimal.TryParse(Request.Params["ValorCargaAverbacao"], out valorCargaAverbacao);
                decimal.TryParse(Request.Params["ValorCreditoICMS"], out valorCreditoICMS);
                decimal.TryParse(Request.Params["ValorBaseCalculoPIS"], out valorBaseCalculoPIS);
                decimal.TryParse(Request.Params["AliquotaPIS"], out aliquotaPIS);
                decimal.TryParse(Request.Params["ValorPIS"], out valorPIS);
                decimal.TryParse(Request.Params["ValorBaseCalculoCOFINS"], out valorBaseCalculoCOFINS);
                decimal.TryParse(Request.Params["ReducaoBaseCalculoICMS"], out reducaoBaseCalculoICMS);
                decimal.TryParse(Request.Params["AliquotaCOFINS"], out aliquotaCOFINS);
                decimal.TryParse(Request.Params["ValorCOFINS"], out valorCOFINS);
                decimal.TryParse(Request.Params["PercentualICMSRecolhido"], out percentualICMSRecolhido);
                decimal.TryParse(Request.Params["ValorFreteContratado"], out valorFreteContratado);

                decimal.TryParse(Request.Params["ValorINSS"], out valorINSS);
                decimal.TryParse(Request.Params["ValorIR"], out valorIR);
                decimal.TryParse(Request.Params["ValorCSLL"], out valorCSLL);

                decimal.TryParse(Request.Params["ValorBaseCalculoIR"], out decimal valorBaseCalculoIR);
                decimal.TryParse(Request.Params["AliquotaIR"], out decimal aliquotaIR);
                decimal.TryParse(Request.Params["ValorBaseCalculoINSS"], out decimal valorBaseCalculoINSS);
                decimal.TryParse(Request.Params["AliquotaINSS"], out decimal aliquotaINSS);
                decimal.TryParse(Request.Params["ValorBaseCalculoCSLL"], out decimal valorBaseCalculoCSLL);
                decimal.TryParse(Request.Params["AliquotaCSLL"], out decimal aliquotaCSLL);
                decimal.TryParse(Request.Params["ValorICMSDesoneracao"], out decimal valorICMSDesoneracao);

                int.TryParse(Request.Params["CodigoOutrasAliquotas"], out int codigoOutrasAliquotas);
                string cstIBSCBS = Request.Params["CSTIBSCBS"];
                string classificacaoTributariaIBSCBS = Request.Params["ClassificacaoTributariaIBSCBS"];

                decimal.TryParse(Request.Params["BaseCalculoIBSCBS"], out decimal baseCalculoIBSCBS);

                decimal.TryParse(Request.Params["AliquotaIBSEstadual"], out decimal aliquotaIBSEstadual);
                decimal.TryParse(Request.Params["PercentualReducaoIBSEstadual"], out decimal percentualReducaoIBSEstadual);
                decimal.TryParse(Request.Params["AliquotaIBSEstadualEfetiva"], out decimal aliquotaIBSEstadualEfetiva);
                decimal.TryParse(Request.Params["ValorIBSEstadual"], out decimal valorIBSEstadual);

                decimal.TryParse(Request.Params["AliquotaIBSMunicipal"], out decimal aliquotaIBSMunicipal);
                decimal.TryParse(Request.Params["PercentualReducaoIBSMunicipal"], out decimal percentualReducaoIBSMunicipal);
                decimal.TryParse(Request.Params["AliquotaIBSMunicipalEfetiva"], out decimal aliquotaIBSMunicipalEfetiva);
                decimal.TryParse(Request.Params["ValorIBSMunicipal"], out decimal valorIBSMunicipal);

                decimal.TryParse(Request.Params["AliquotaCBS"], out decimal aliquotaCBS);
                decimal.TryParse(Request.Params["PercentualReducaoCBS"], out decimal percentualReducaoCBS);
                decimal.TryParse(Request.Params["AliquotaCBSEfetiva"], out decimal aliquotaCBSEfetiva);
                decimal.TryParse(Request.Params["ValorCBS"], out decimal valorCBS);

                DateTime dataPrevistaEntregaCargaRecebedor, dataPrevistaEntregaConteiner;
                DateTime dataHoraEmissao = DateTime.Now;

                if (!string.IsNullOrWhiteSpace(Request.Params["DataEmissao"]) && !string.IsNullOrWhiteSpace(Request.Params["HoraEmissao"]))
                    DateTime.TryParseExact(string.Concat(Request.Params["DataEmissao"], " ", Request.Params["HoraEmissao"]), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataHoraEmissao);

                if (!string.IsNullOrWhiteSpace(Request.Params["DataPrevistaEntregaCargaRecebedor"]))
                    DateTime.TryParseExact(Request.Params["DataPrevistaEntregaCargaRecebedor"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntregaCargaRecebedor);
                else
                    dataPrevistaEntregaCargaRecebedor = DateTime.Today.AddDays(1);
                DateTime.TryParseExact(Request.Params["DataPrevistaEntregaConteiner"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataPrevistaEntregaConteiner);
                DateTime.TryParseExact(Request.Params["DataHoraViagem"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataDataHoraViagem);

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                else
                    unidadeDeTrabalho.Start();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                if (codigoCTE > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração do CT-e negada!");

                    CTE = repCTe.BuscarPorId(codigoCTE, this.EmpresaUsuario.Codigo);

                    if (CTE == null)
                    {
                        codigoCTE = 0;

                        if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        {
                            unidadeDeTrabalho.Rollback();
                            return Json<bool>(false, false, "Permissão para inclusão do CT-e negada!");
                        }

                        CTE = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();

                    }
                    else if (CTE.Status != "S" && CTE.Status != "R")
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "O status do CT-e não permite a edição do mesmo.");
                    }
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    {
                        unidadeDeTrabalho.Rollback();
                        return Json<bool>(false, false, "Permissão para inclusão do CT-e negada!");
                    }

                    CTE = new Dominio.Entidades.ConhecimentoDeTransporteEletronico();
                }

                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.ModalTransporte repModalTransporte = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModeloDocumento = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.DocumentosAnulacaoCTE repDocAnulacaoCTe = new Repositorio.DocumentosAnulacaoCTE(unidadeDeTrabalho);
                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);

                Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = repDocAnulacaoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, CTE.Codigo);

                if (serie == 0)
                    return Json<bool>(false, false, "Não foi informada Série para emissão do CT-e.");

                if (tipoCTE != Dominio.Enumeradores.TipoCTE.Normal && tipoCTE != Dominio.Enumeradores.TipoCTE.Simplificado)
                {
                    CTE.ChaveCTESubComp = string.IsNullOrWhiteSpace(CTE.ChaveCTESubComp) ? Request.Params["ChaveCTeOriginal"] : CTE.ChaveCTESubComp;
                    if (!string.IsNullOrWhiteSpace(CTE.ChaveCTESubComp))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = repCTe.BuscarPorChave(CTE.ChaveCTESubComp, this.EmpresaUsuario.Codigo);
                        if (cteOriginal != null)
                        {
                            if (cteOriginal.TipoCTE != Dominio.Enumeradores.TipoCTE.Normal && cteOriginal.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && cteOriginal.TipoCTE != Dominio.Enumeradores.TipoCTE.Substituto)
                            {
                                listaErros.Add("O tipo do CT-e é inválido para este tipo de emissão.");
                            }
                            else
                            {
                                if (cteOriginal.Status != "A" && cteOriginal.Status != "Z")
                                {
                                    listaErros.Add("O status do CT-e é inválido para este tipo de emissão.");
                                }
                                else
                                {
                                    if (tipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                                    {
                                        DateTime dt = cteOriginal.DataEmissao != null ? cteOriginal.DataEmissao.Value : DateTime.Now;
                                        dt = dt.AddDays(this.ObterConfiguracao() != null ? this.ObterConfiguracao().DiasParaEmissaoDeCTeComplementar : 1);
                                        if (DateTime.Now > dt)
                                        {
                                            listaErros.Add("A data de emissão do CT-e não permite a emissão de um CT-e complementar.");
                                        }
                                        if (valorBaseCalculoICMS <= 0 && valorFreteContratado <= 0)
                                        {
                                            listaErros.Add("É necessário que a base de cálculo do ICMS ou o valor do frete contratado seja maior do que zero para emitir um CT-e Complementar.");
                                        }
                                    }
                                    else if (tipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao)
                                    {
                                        DateTime dataAnulacao;
                                        DateTime.TryParseExact(Request.Params["DataAnulacao"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataAnulacao);
                                        if (dataAnulacao == DateTime.MinValue)
                                            listaErros.Add("A data do documento de anulação é inválida.");
                                        else
                                            CTE.DataAnulacao = dataAnulacao;
                                    }
                                    else if (tipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                                    {
                                        bool tomadorContribuinte;
                                        bool.TryParse(Request.Params["TomadorContribuinte"], out tomadorContribuinte);
                                        string chaveAcessoCTeAnulacao = Utilidades.String.OnlyNumbers(Request.Params["ChaveAcessoCTeAnulacao"]);
                                        Dominio.Enumeradores.TipoDocumentoAnulacao tipoDocumentoSubstituicao;
                                        Enum.TryParse<Dominio.Enumeradores.TipoDocumentoAnulacao>(Request.Params["TipoDocumentoSubstituicao"], out tipoDocumentoSubstituicao);
                                        string chaveAcessoCTeEmitidoTomador = Utilidades.String.OnlyNumbers(Request.Params["ChaveAcessoCTeEmitidoTomador"]);
                                        string chaveAcessoNFeEmitidaTomador = Utilidades.String.OnlyNumbers(Request.Params["ChaveAcessoNFeEmitidaTomador"]);
                                        double cnpjEmitenteDocumentoEmitidoTomador;
                                        double.TryParse(Request.Params["CNPJDocumentoEmitidoTomador"], out cnpjEmitenteDocumentoEmitidoTomador);
                                        int modeloDocumentoEmitidoTomador;
                                        int.TryParse(Request.Params["ModeloDocumentoEmitidoTomador"], out modeloDocumentoEmitidoTomador);
                                        string serieDocumentoEmitidoTomador = Request.Params["SerieDocumentoEmitidoTomador"];
                                        string subserieDocumentoEmitidoTomador = Request.Params["SubserieDocumentoEmitidoTomador"];
                                        string numeroDocumentoEmitidoTomador = Utilidades.String.OnlyNumbers(Request.Params["NumeroDocumentoEmitidoTomador"]);
                                        decimal valorDocumentoEmitidoTomador;
                                        decimal.TryParse(Request.Params["ValorDocumentoEmitidoTomador"], out valorDocumentoEmitidoTomador);
                                        DateTime dataEmissaoDocumentoEmitidoTomador;
                                        DateTime.TryParseExact(Request.Params["DataEmissaoDocumentoEmitidoTomador"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoDocumentoEmitidoTomador);

                                        if (documentoAnulacao == null)
                                        {
                                            documentoAnulacao = new Dominio.Entidades.DocumentosAnulacaoCTE();
                                            documentoAnulacao.CTE = CTE;
                                        }
                                        else
                                        {
                                            documentoAnulacao.DataEmissao = null;
                                            documentoAnulacao.Emitente = null;
                                            documentoAnulacao.Numero = string.Empty;
                                            documentoAnulacao.Serie = string.Empty;
                                            documentoAnulacao.Subserie = string.Empty;
                                            documentoAnulacao.Tipo = 0;
                                            documentoAnulacao.Valor = 0m;
                                        }

                                        documentoAnulacao.Tipo = tipoDocumentoSubstituicao;
                                        documentoAnulacao.ContribuinteICMS = tomadorContribuinte ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;

                                        if (tomadorContribuinte)
                                        {
                                            if (tipoDocumentoSubstituicao == Dominio.Enumeradores.TipoDocumentoAnulacao.CTe)
                                            {
                                                documentoAnulacao.Chave = chaveAcessoCTeEmitidoTomador;
                                                documentoAnulacao.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo("57");
                                                if (documentoAnulacao.Chave.Length != 44)
                                                    listaErros.Add("Chave do CT-e do documento de anulação inválido.");
                                            }
                                            else if (tipoDocumentoSubstituicao == Dominio.Enumeradores.TipoDocumentoAnulacao.NFe)
                                            {
                                                documentoAnulacao.Chave = chaveAcessoNFeEmitidaTomador;
                                                documentoAnulacao.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo("55");
                                                if (documentoAnulacao.Chave.Length != 44)
                                                    listaErros.Add("Chave da NF-e do documento de anulação inválida.");
                                            }
                                            else if (tipoDocumentoSubstituicao == Dominio.Enumeradores.TipoDocumentoAnulacao.CTouNF)
                                            {
                                                documentoAnulacao.DataEmissao = dataEmissaoDocumentoEmitidoTomador;
                                                documentoAnulacao.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorId(modeloDocumentoEmitidoTomador); ;
                                                documentoAnulacao.Numero = numeroDocumentoEmitidoTomador;
                                                documentoAnulacao.Serie = serieDocumentoEmitidoTomador;
                                                documentoAnulacao.Subserie = subserieDocumentoEmitidoTomador;
                                                documentoAnulacao.Valor = valorDocumentoEmitidoTomador;
                                                documentoAnulacao.Emitente = repCliente.BuscarPorCPFCNPJ(cnpjEmitenteDocumentoEmitidoTomador); ;
                                                if (documentoAnulacao.DataEmissao == DateTime.MinValue)
                                                    listaErros.Add("Data de emissão do documento de anulação inválida.");
                                                if (documentoAnulacao.Emitente == null)
                                                    listaErros.Add("Emitente do documento de anulação não encontrado.");
                                                if (documentoAnulacao.ModeloDocumentoFiscal == null)
                                                    listaErros.Add("Modelo do documento de anulação não encontrado.");
                                                if (documentoAnulacao.Numero.Length <= 0)
                                                    listaErros.Add("Número do documento de anulação inválido.");
                                                if (documentoAnulacao.Serie.Length <= 0)
                                                    listaErros.Add("Série do documento de anulação inválida.");
                                                if (documentoAnulacao.Valor <= 0m)
                                                    listaErros.Add("Valor do documento de anulação inválido.");
                                            }
                                        }
                                        else
                                        {
                                            documentoAnulacao.Tipo = Dominio.Enumeradores.TipoDocumentoAnulacao.CTe;
                                            documentoAnulacao.Chave = chaveAcessoCTeAnulacao;
                                            documentoAnulacao.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo("57");
                                            if (string.IsNullOrWhiteSpace(documentoAnulacao.Chave) || documentoAnulacao.Chave.Length != 44)
                                                listaErros.Add("Chave do CT-e de substituição inválida.");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            listaErros.Add("CT-e original não encontrado.");
                        }
                    }
                    else
                    {
                        listaErros.Add("Chave do CT-e original é inválida.");
                    }
                }

                CTE.AliquotaICMS = aliquotaICMS;
                CTE.BaseCalculoICMS = valorBaseCalculoICMS;
                CTE.PercentualReducaoBaseCalculoICMS = reducaoBaseCalculoICMS;
                CTE.ValorICMS = valorICMS;
                CTE.ValorPresumido = valorCreditoICMS;
                CTE.PercentualICMSIncluirNoFrete = percentualICMSRecolhido;
                CTE.IncluirICMSNoFrete = incluirICMSNoFrete ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                CTE.SubstituicaoTomador = substituicaoTomador == 1;

                CTE.AliquotaPIS = aliquotaPIS;
                CTE.BasePIS = valorBaseCalculoPIS;
                CTE.ValorPIS = valorPIS;

                CTE.AliquotaCOFINS = aliquotaCOFINS;
                CTE.BaseCOFINS = valorBaseCalculoCOFINS;
                CTE.ValorCOFINS = valorCOFINS;

                CTE.CFOP = repCFOP.BuscarPorId(cfop);
                CTE.IndicadorGlobalizado = indicadorGlobalizado;
                CTE.IndicadorIETomador = indicadorIETomador;
                CTE.CIOT = CIOT;
                CTE.Container = conteiner;
                CTE.DataColeta = DateTime.Now;
                CTE.DataEmissao = dataHoraEmissao;

                if (dataPrevistaEntregaConteiner.ToString("ddMMyyyy") != "01010001")
                    CTE.DataPrevistaContainer = dataPrevistaEntregaConteiner;
                else
                    CTE.DataPrevistaContainer = null;

                CTE.DataPrevistaEntrega = dataPrevistaEntregaCargaRecebedor;
                CTE.DetalhesRetira = detalhesRetiradaRecebedor;
                CTE.Empresa = this.EmpresaUsuario;
                CTE.LacreContainer = numeroLacre;

                CTE.LocalidadeEmissao = repLocalidade.BuscarPorCodigo(municipioLocalEmissaoCTe);
                CTE.LocalidadeInicioPrestacao = repLocalidade.BuscarPorCodigo(municipioInicioPrestacao);
                CTE.LocalidadeTerminoPrestacao = repLocalidade.BuscarPorCodigo(municipioTerminoPrestacao);

                if (CTE.Veiculos != null && CTE.Veiculos.Count > 0 && CTE.Motoristas != null && CTE.Motoristas.Count > 0)
                    CTE.Lotacao = Dominio.Enumeradores.OpcaoSimNao.Sim;
                else
                    CTE.Lotacao = indicadorLotacao ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                CTE.ModalTransporte = repModalTransporte.BuscarPorCodigo(modalTransporte, false);
                CTE.ModeloDocumentoFiscal = repModeloDocumento.BuscarPorModelo(modelo.ToString());
                CTE.NaturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(naturezaDaOperacao);
                CTE.NumeroColeta = numeroCTRB;
                CTE.ProdutoPredominante = produtoPredominante;
                CTE.Retira = recebedorRetiraDestino ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao;
                CTE.RNTRC = RNTRC;
                CTE.OutrasCaracteristicasDaCarga = outrasCaracteristicasCarga;

                CTE.TipoAmbiente = this.EmpresaUsuario.TipoAmbiente;
                CTE.TipoCTE = tipoCTE;
                if (CTE.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    CTE.DescricaoComplemento = !string.IsNullOrWhiteSpace(descricaoComplemento) && descricaoComplemento.Length > 15 ? descricaoComplemento.Substring(0, 15) : descricaoComplemento;
                CTE.TipoEmissao = CTE.TipoEmissao != "4" && CTE.TipoEmissao != "5" ? formaEmissao : CTE.TipoEmissao;
                CTE.TipoImpressao = tipoImpressao;
                CTE.TipoPagamento = tipoPagamento;
                CTE.TipoServico = tipoServico;
                CTE.TipoTomador = tipoTomador;
                CTE.ValorAReceber = valorAReceber;
                CTE.ValorPrestacaoServico = valorTotalPrestacaoServico;
                CTE.ValorFrete = valorFreteContratado;
                CTE.ValorTotalMercadoria = valorTotalCarga;
                CTE.ValorCarbaAverbacao = valorCargaAverbacao;
                string versao = "4.00";
                if (this.EmpresaUsuario.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.VersaoCTe))
                    versao = this.EmpresaUsuario.Configuracao.VersaoCTe;
                else if (this.EmpresaUsuario.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoCTe))
                    versao = this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoCTe;
                CTE.Versao = versao;
                CTE.ObservacoesGerais = observacoesGerais;
                CTE.ObservacoesDigitacao = observacaoDigitacao;
                CTE.InformacaoAdicionalFisco = informacaoAdicionalFisco;
                CTE.CaracteristicaServico = caracteristicaAdicionalServico;
                CTE.CaracteristicaTransporte = caracteristicaAdicionalTransporte;
                CTE.TipoEnvio = tipoEnvio;
                CTE.ObservacoesAvancadas = string.Empty; //Limpa as observações avançadas que são informadas toda vez ao emitir
                CTE.TipoFretamento = tipoFretamento;
                if (dataDataHoraViagem > DateTime.MinValue)
                    CTE.DataHoraViagem = dataDataHoraViagem;

                CTE.ValorBaseCalculoIR = valorBaseCalculoIR;
                CTE.AliquotaIR = aliquotaIR;
                CTE.ValorIR = valorIR;
                CTE.ValorBaseCalculoINSS = valorBaseCalculoINSS;
                CTE.AliquotaINSS = aliquotaINSS;
                CTE.ValorINSS = valorINSS;
                CTE.ValorBaseCalculoCSLL = valorBaseCalculoCSLL;
                CTE.AliquotaCSLL = aliquotaCSLL;
                CTE.ValorCSLL = valorCSLL;
                CTE.CTeReferencia = codigoCTEReferenciado >= 0 ? codigoCTEReferenciado : 0;
                CTE.IndicadorNegociavel = indNegociavel;
                CTE.ValorICMSDesoneracao = valorICMSDesoneracao;
                CTE.CodigoBeneficio = codigoBeneficio;

                CTE.SetarRegraOutraAliquota(codigoOutrasAliquotas);

                CTE.CSTIBSCBS = cstIBSCBS;
                CTE.ClassificacaoTributariaIBSCBS = classificacaoTributariaIBSCBS;
                CTE.BaseCalculoIBSCBS = baseCalculoIBSCBS;

                CTE.AliquotaIBSEstadual = aliquotaIBSEstadual;
                CTE.PercentualReducaoIBSEstadual = percentualReducaoIBSEstadual;
                CTE.AliquotaIBSEstadualEfetiva = aliquotaIBSEstadualEfetiva;
                CTE.ValorIBSEstadual = valorIBSEstadual;

                CTE.AliquotaIBSMunicipal = aliquotaIBSMunicipal;
                CTE.PercentualReducaoIBSMunicipal = percentualReducaoIBSMunicipal;
                CTE.AliquotaIBSMunicipalEfetiva = aliquotaIBSMunicipalEfetiva;
                CTE.ValorIBSMunicipal = valorIBSMunicipal;

                CTE.AliquotaCBS = aliquotaCBS;
                CTE.PercentualReducaoCBS = percentualReducaoCBS;
                CTE.AliquotaCBSEfetiva = aliquotaCBSEfetiva;
                CTE.ValorCBS = valorCBS;

                CTE.Cancelado = "N";

                if (ICMS == Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90 || ICMS == Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90)
                    CTE.ExibeICMSNaDACTE = exibeICMSNaDacte;
                else
                    CTE.ExibeICMSNaDACTE = true;

                //if (tipoEmissaoCTe == 0) //Salvar
                CTE.Status = "S";
                //else //Emitir
                //    CTE.Status = "P";

                this.ObterCSTDoICMS(ICMS, ref CTE);
                this.ObterCSTDoPIS(CSTPIS, ref CTE);
                this.ObterCSTDoCOFINS(CSTCOFINS, ref CTE);

                //if (!CTE.Empresa.OptanteSimplesNacional && CTE.SimplesNacional == Dominio.Enumeradores.OpcaoSimNao.Sim)
                //{
                //    unidadeDeTrabalho.Rollback();
                //    return Json<bool>(false, false, "Transportador não é optante pelo Simples Naciona, verifique a tributação selecionada.");
                //}

                if (CTE.CFOP == null)
                {
                    unidadeDeTrabalho.Rollback();
                    return Json<bool>(false, false, "Selecione uma CFOP para salvar/emitir o CT-e.");
                }

                Dominio.Entidades.EmpresaSerie serieCTe = repSerie.BuscarPorCodigo(serie);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                if (codigoCTE > 0)
                {
                    if (this.UsuarioAdministrativo != null)
                        CTE.Log = string.Concat(CTE.Log, " / Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        CTE.Log = string.Concat(CTE.Log, " / Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    if (codigoCTEReferenciado == -1)
                    {
                        if (this.UsuarioAdministrativo != null)
                            CTE.Log = string.Concat(CTE.Log, " / Liberada Edição por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                        else
                            CTE.Log = string.Concat(CTE.Log, " / Liberada Edição por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    }

                    if (CTE.Serie.Codigo != serieCTe.Codigo)
                    {
                        CTE.Numero = 0;
                        CTE.TipoControle = 1;
                        CTE.Serie = serieCTe;
                        CTE.Numero = svcCTe.ObterProximoNumero(CTE, repCTe);
                    }

                    repCTe.Atualizar(CTE);
                }
                else
                {
                    if ((this.EmpresaUsuario.RegimeEspecial == Dominio.Enumeradores.RegimeEspecialEmpresa.LucroPresumido) && (this.EmpresaUsuario.PercentualCredito > 0))
                    {
                        if ((CTE.CST == "90" || CTE.CST == "91") && CTE.PercentualReducaoBaseCalculoICMS > 0)
                            CTE.ObservacoesGerais += (string.IsNullOrWhiteSpace(CTE.ObservacoesGerais) ? "" : " - ") + "ICMS RECOLHIDO COM CREDITO PRESUMIDO " + this.EmpresaUsuario.PercentualCredito.ToString("n2") + "% CFE. CONVENIO ICMS 106/96";
                        else if ((CTE.CST == "60") && CTE.ValorPresumido > 0)
                            CTE.ObservacoesGerais += (string.IsNullOrWhiteSpace(CTE.ObservacoesGerais) ? "" : " - ") + this.EmpresaUsuario.PercentualCredito.ToString("n2") + "% CREDITO PRESUMIDO R$" + CTE.ValorPresumido.ToString("n2");
                    }

                    if (duplicado)
                        CTE.Log = "Duplicado - ";

                    if (this.UsuarioAdministrativo != null)
                        CTE.Log += string.Concat("Criado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        CTE.Log += string.Concat("Criado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                    if (codigoCTEReferenciado == -1)
                    {
                        if (this.UsuarioAdministrativo != null)
                            CTE.Log = string.Concat(CTE.Log, " / Liberada Edição por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                        else
                            CTE.Log = string.Concat(CTE.Log, " / Liberada Edição por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    }

                    CTE.TipoControle = 1;
                    CTE.Serie = serieCTe;
                    CTE.Numero = svcCTe.ObterProximoNumero(CTE, repCTe);

                    repCTe.Inserir(CTE);
                }

                listaErros.AddRange(this.SalvarCliente("Remetente", ref CTE, Dominio.Enumeradores.TipoTomador.Remetente, unidadeDeTrabalho));
                listaErros.AddRange(this.SalvarCliente("Expedidor", ref CTE, Dominio.Enumeradores.TipoTomador.Expedidor, unidadeDeTrabalho));
                listaErros.AddRange(this.SalvarCliente("Recebedor", ref CTE, Dominio.Enumeradores.TipoTomador.Recebedor, unidadeDeTrabalho));
                listaErros.AddRange(this.SalvarCliente("Destinatario", ref CTE, Dominio.Enumeradores.TipoTomador.Destinatario, unidadeDeTrabalho));

                listaErros.AddRange(this.SalvarLocalEntrega(ref CTE, unidadeDeTrabalho));

                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    listaErros.AddRange(this.SalvarCliente("Tomador", ref CTE, Dominio.Enumeradores.TipoTomador.Outros, unidadeDeTrabalho));
                else
                    CTE.OutrosTomador = null;

                if (codigoCTE == 0 && CTE.Remetente != null && CTE.CST == "60" && CTE.ValorICMS > 0)
                {
                    double cnpjRemetente = 0;
                    double.TryParse(CTE.Remetente.CPF_CNPJ, out cnpjRemetente);
                    Dominio.Entidades.DadosCliente dadosRemetente = repDadosCliente.Buscar(CTE.Empresa.Codigo, cnpjRemetente);
                    if (dadosRemetente != null && CTE.BaseCalculoICMS > 0 && dadosRemetente.PercentualRetencaoICMSST > 0)
                    {
                        //CTE.ObservacoesGerais += (string.IsNullOrWhiteSpace(CTE.ObservacoesGerais) ? "" : " - ") + "R$" + CTE.ValorICMS.ToString("n2") + " ICMS retido e recolhido por substituição tributária, conf. art. 110, III, do Decreto nº 20.686/99. Crédito presumido R$" + CTE.ValorPresumido.ToString("n2");
                        CTE.ObservacoesGerais += (string.IsNullOrWhiteSpace(CTE.ObservacoesGerais) ? "" : " - ") + "ICMS Substituição Tributária (ST) parte do tomador VALOR ICMS ST R$" + (CTE.BaseCalculoICMS * (dadosRemetente.PercentualRetencaoICMSST / 100)).ToString("n2") + ".";
                    }
                }

                Servicos.CTe.SetarTomadorPagadorCTe(ref CTE);

                this.SalvarNFEsDoRemetente(CTE, unidadeDeTrabalho);

                svcCTe.SetarPartilhaICMS(ref CTE, unidadeDeTrabalho);

                if (CTE.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                {
                    if (documentoAnulacao.Codigo > 0)
                        repDocAnulacaoCTe.Atualizar(documentoAnulacao);
                    else
                        repDocAnulacaoCTe.Inserir(documentoAnulacao);
                }
                else if (documentoAnulacao != null)
                {
                    repDocAnulacaoCTe.Deletar(documentoAnulacao);
                }

                this.SalvarComponentesDaPrestacao(CTE, unidadeDeTrabalho);
                this.SalvarInformacoesDeSeguro(CTE, unidadeDeTrabalho);
                this.SalvarInformacoesDeQuantidadeDaCarga(CTE, unidadeDeTrabalho);
                this.SalvarNotasFiscaisDoRemetente(CTE, unidadeDeTrabalho);
                this.SalvarOutrosDocumentosDoRemetente(CTE, unidadeDeTrabalho);
                this.SalvarVeiculos(CTE, unidadeDeTrabalho);
                this.SalvarMotoristas(CTE, unidadeDeTrabalho);
                this.SalvarObservacoesContribuinte(CTE, unidadeDeTrabalho);
                this.SalvarObservacoesFisco(CTE, unidadeDeTrabalho);
                this.SalvarDocumentosDeTransporteAnterioresEletronicos(CTE, unidadeDeTrabalho);
                this.SalvarDocumentosDeTransporteAnterioresPapel(CTE, unidadeDeTrabalho);
                this.SalvarProdutosPerigosos(CTE, unidadeDeTrabalho);
                this.SalvarDadosCobranca(CTE, unidadeDeTrabalho);
                this.SalvarPercursos(CTE, unidadeDeTrabalho);

                this.FinalizarColeta(codigoColeta, unidadeDeTrabalho);

                svcCTe.SetarObservacoesAvancadas(ref CTE, unidadeDeTrabalho);
                svcCTe.SetarObservacaoAvancadaPorRegraICMS(ref CTE, unidadeDeTrabalho, null, true);
                svcCTe.SetarXCampoVeiculo(CTE, unidadeDeTrabalho);
                svcCTe.AdicionarResponsavelSeguroObsContribuinte(CTE, unidadeDeTrabalho);

                repCTe.Atualizar(CTE);

                listaErros.AddRange(this.ValidarCTeParaEmissao(CTE, unidadeDeTrabalho, tipoEmissaoCTe == 1));

                if (listaErros.Count > 0)
                {
                    System.Text.StringBuilder htmlMensagemRetorno = new System.Text.StringBuilder();

                    htmlMensagemRetorno.Append("Foram encontradas as seguintes inconsistências ao salvar o CT-e: <br/><b>");

                    foreach (string mensagem in listaErros)
                    {
                        htmlMensagemRetorno.Append("<br/>&bull; ").Append(mensagem);
                    }

                    htmlMensagemRetorno.Append("</b><br/><br/>Corrija-as e tente emitir novamente.");

                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, htmlMensagemRetorno.ToString());
                }

                string statusAnterior = CTE.Status;
                unidadeDeTrabalho.CommitChanges();

                if (tipoEmissaoCTe == 1) //emitir //if (CTE.Status == "P")
                {
                    if (this.UsuarioAdministrativo != null)
                        CTE.Usuario = this.UsuarioAdministrativo;
                    else
                        CTE.Usuario = this.Usuario;

                    //if (svcCTe.Emitir(CTE.Codigo, CTE.Empresa.Codigo, unidadeDeTrabalho))
                    if (svcCTe.Emitir(ref CTE, unidadeDeTrabalho))
                    {
                        if (CTE.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, CTE.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);

                        svcCTe.AtualizarIntegracaoRetornoCTe(CTE, unidadeDeTrabalho);
                        try
                        {
                            if (statusAnterior == "R")
                            {
                                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(CTE.Codigo);
                                if (cargaCte != null)
                                {
                                    if (cargaCte.Carga.PossuiPendencia)
                                    {
                                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                                        cargaCte.Carga.PossuiPendencia = false;
                                        cargaCte.Carga.problemaCTE = false;
                                        cargaCte.Carga.MotivoPendencia = "";
                                        repCarga.Atualizar(cargaCte.Carga);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Falha ao atualizar carga CT-e: " + ex);
                        }
                    }
                    else
                        return Json((CTE == null ? null : new { Codigo = CTE.Codigo }), CTE == null ? false : true, "O CT-e foi salvo, porém ocorreram erros ao enviar para o sefaz. Atualize a página e verifique os erros.");
                }

                return Json(new { CodigoCTE = CTE.Codigo }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao salvar CT-e: " + ex);
                unidadeDeTrabalho.Rollback();
                Repositorio.ConhecimentoDeTransporteEletronico repCte = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteGravado = null;
                if (CTE != null)
                {
                    cteGravado = repCte.BuscarPorCodigo(CTE.Codigo);
                    if (cteGravado != null)
                    {
                        cteGravado.Status = "R";
                        cteGravado.MensagemRetornoSefaz = string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), " - Erro: Sefaz indisponível no momento. Tente novamente.");

                        repCte.Atualizar(cteGravado);
                    }
                }

                return Json((cteGravado == null ? null : new { Codigo = cteGravado.Codigo }), cteGravado == null ? false : true, "Ocorreu uma falha ao salvar os dados do CTe. Atualize a página e tente novamente!");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Emitir()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            Servicos.CTe svcCTe = new CTe(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            try
            {
                List<string> listaErros = new List<string>();
                int codigoCTE = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTE);
                string formaEmissao = Request.Params["FormaEmissao"];
                bool ajustarGlobalizado = !string.IsNullOrWhiteSpace(Request.Params["AjustarGlobalizado"]) && Request.Params["AjustarGlobalizado"] == "SIM";

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Emissao)
                    cte = repCTe.BuscarPorId(codigoCTE, this.EmpresaUsuario.Codigo);
                else if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                    cte = repCTe.BuscarPorCodigo(codigoCTE);

                if (cte.Empresa.Status != "A")
                    return Json<bool>(true, false, "Empresa não está ativa para emissão de CT-e.");

                if (cte.Empresa.StatusFinanceiro == "B")
                    return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                if (cte.Status == "E")
                {
                    //svcCTe.AdicionarCTeNaFilaDeConsulta(cte); //FilaConsultaCTe.GetInstance().QueueItem(cte.Empresa.Codigo, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                    svcCTe.Consultar(ref cte, unidadeDeTrabalho);

                    if (cte.Status == "A")
                    {
                        bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                        if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cte.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
                        {
                            Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);
                            if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unidadeDeTrabalho))
                                FilaConsultaCTe.GetInstance().QueueItem(5, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, Conexao.StringConexao);
                        }

                        Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unidadeDeTrabalho);
                        svcLsTranslog.SalvarCTeParaIntegracao(cte.Codigo, cte.Empresa.Codigo, unidadeDeTrabalho);

                        Servicos.CIOT svcCIOT = new Servicos.CIOT(unidadeDeTrabalho);
                        svcCIOT.VincularCTeCIOTEFrete(cte.Codigo, unidadeDeTrabalho);
                    }

                    return Json<bool>(true, false, "Aguarde, Consultando status de emissão do CT-e.");
                }

                if (cte.Status != "S" && cte.Status != "R" && cte.Status != "P" && cte.Status != "F" && cte.Status != "G" && cte.Status != "Q" && cte.Status != "W" && !string.IsNullOrWhiteSpace(cte.Status))
                    return Json<bool>(false, false, "O status do CT-e não permite a emissão do mesmo.");

                if (cte.Empresa.CertificadoA3 && (formaEmissao == "5" || formaEmissao == "4"))
                    return Json<bool>(false, false, "Contingência EPEC e FSDA disponível apenas para Certificado A1.");

                Repositorio.DocumentosAnulacaoCTE repDocAnulacaoCTe = new Repositorio.DocumentosAnulacaoCTE(unidadeDeTrabalho);
                Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = repDocAnulacaoCTe.BuscarPorCTe(cte.Empresa.Codigo, cte.Codigo);

                listaErros.AddRange(this.ValidarCTeParaEmissao(cte, unidadeDeTrabalho, true, false));

                if (listaErros.Count > 0)
                {
                    System.Text.StringBuilder htmlMensagemRetorno = new System.Text.StringBuilder();

                    htmlMensagemRetorno.Append("Foram encontradas as seguintes inconsistências ao tentar emitir o CT-e: <br/><b>");

                    foreach (string mensagem in listaErros)
                    {
                        htmlMensagemRetorno.Append("<br/>&bull; ").Append(mensagem);
                    }

                    htmlMensagemRetorno.Append("</b><br/><br/>Corrija-as e tente emitir novamente.");

                    return Json<bool>(false, false, htmlMensagemRetorno.ToString());
                }
                else
                {
                    string statusAnterior = cte.Status;

                    cte.TipoEmissao = cte.TipoEmissao != "4" && cte.TipoEmissao != "5" ? formaEmissao : cte.TipoEmissao;
                    cte.Status = "S"; //cte.Status = "P";                    

                    if (this.UsuarioAdministrativo != null)
                        cte.Usuario = this.UsuarioAdministrativo;
                    else
                        cte.Usuario = this.Usuario;

                    cte.ObservacoesAvancadas = string.Empty; //Limpa as observações avançadas que são informadas toda vez ao emitir

                    if (cte.IndicadorGlobalizado == Dominio.Enumeradores.OpcaoSimNao.Sim && ajustarGlobalizado && cte.Versao == "3.00")
                    {
                        if (cte.Documentos == null || cte.Documentos.Count < 5)
                        {
                            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                            cte.IndicadorGlobalizado = Dominio.Enumeradores.OpcaoSimNao.Nao;
                            cte.SetarDestinatarioDiversos(cte.Empresa, repAtividade.BuscarPorCodigo(4), "DIVERSO");
                        }
                    }

                    svcCTe.SetarObservacoesAvancadas(ref cte, unidadeDeTrabalho);
                    svcCTe.SetarObservacaoAvancadaPorRegraICMS(ref cte, unidadeDeTrabalho, null, true);
                    svcCTe.SetarXCampoVeiculo(cte, unidadeDeTrabalho);
                    svcCTe.AdicionarResponsavelSeguroObsContribuinte(cte, unidadeDeTrabalho);

                    repCTe.Atualizar(cte);

                    //if (svcCTe.Emitir(cte.Codigo, cte.Empresa.Codigo))
                    if (svcCTe.Emitir(ref cte, unidadeDeTrabalho))
                    {
                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                        svcCTe.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);
                        try
                        {
                            if (statusAnterior == "R")
                            {
                                Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeDeTrabalho);
                                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCte = repCargaCTe.BuscarPorCTe(cte.Codigo);
                                if (cargaCte != null)
                                {
                                    if (cargaCte.Carga.PossuiPendencia)
                                    {
                                        Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeDeTrabalho);
                                        cargaCte.Carga.PossuiPendencia = false;
                                        cargaCte.Carga.problemaCTE = false;
                                        cargaCte.Carga.MotivoPendencia = "";
                                        repCarga.Atualizar(cargaCte.Carga);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro("Falha ao atualizar carga CT-e: " + ex);
                        }
                    }
                    else
                    {
                        return Json<bool>(true, false, "O CT-e foi emitido, porém ocorreram erros ao enviar para o sefaz. Atualize a página e verifique os erros.");
                    }
                }

                return Json(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao emitir CT-e: " + ex);
                if (cte != null)
                    svcCTe.SalvarRetornoSefaz(cte, "A", 0, 8888, "ERRO: Sefaz indisponível no momento. Tente novamente.", unidadeDeTrabalho);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir o CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EmitirTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out serie);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string configAdicionarCTesFilaConsulta = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"]) ? ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"] : "SIM";

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                bool contem = false;
                bool.TryParse(Request.Params["Contem"], out contem);

                string placa = Request.Params["Placa"];
                string motorista = Request.Params["Motorista"];
                string status = Request.Params["Status"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNF = Request.Params["NumeroNF"];
                if (!string.IsNullOrWhiteSpace(numeroNF))
                    numeroNF = Utilidades.String.OnlyNumbers(numeroNF);

                string tipoEnvio = System.Configuration.ConfigurationManager.AppSettings["TipoEnvioParaEmitirTodosCTes"];

                Dominio.Enumeradores.TipoCTE tipoCTe;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = null;
                if (Enum.TryParse(Request.Params["AverbacaoCTe"], out Dominio.Enumeradores.FiltroAverbacaoCTe averbacaoCTeAux))
                    averbacaoCTe = averbacaoCTeAux;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                int countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe);
                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, 0, countCTes);

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    if (listaCTes[i].Valor > 0)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i].Codigo);

                        if (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.BloquearEmissaoCTeParaCargaMunicipal && cte.LocalidadeInicioPrestacao.Codigo == cte.LocalidadeTerminoPrestacao.Codigo)
                            return Json<bool>(false, false, "CTe " + cte.Numero + " não emitido, empresa está configurada para não permitir emitir CTe com município de início e término igual (cargas municipais).");

                        if (!string.IsNullOrWhiteSpace(tipoEnvio))
                            cte.TipoEnvio = int.Parse(tipoEnvio);
                        svcCTe.SetarObservacoesAvancadas(ref cte, unidadeDeTrabalho);
                        svcCTe.SetarObservacaoAvancadaPorRegraICMS(ref cte, unidadeDeTrabalho, null, true);
                        svcCTe.SetarXCampoVeiculo(cte, unidadeDeTrabalho);
                        svcCTe.AdicionarResponsavelSeguroObsContribuinte(cte, unidadeDeTrabalho);
                        repCTe.Atualizar(cte);

                        if (svcCTe.Emitir(ref cte, unidadeDeTrabalho))
                        {
                            if (configAdicionarCTesFilaConsulta.Equals("SIM") && cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                                FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, unidadeDeTrabalho.StringConexao);
                            svcCTe.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);
                        }

                        unidadeDeTrabalho.FlushAndClear();
                    }
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificarDigest()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            Servicos.CTe svcCTe = new CTe(unidadeDeTrabalho);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
            try
            {
                List<string> listaErros = new List<string>();
                int codigoCTE = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTE);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
                {
                    if (!this.Usuario.Callcenter)
                        return Json<bool>(true, false, "Não disponível.");

                    cte = repCTe.BuscarPorCodigo(codigoCTE);
                }

                if (cte == null)
                    return Json<bool>(true, false, "CTe não localizado.");

                if (cte.Empresa.Status != "A")
                    return Json<bool>(true, false, "Empresa não está ativa para emissão de CT-e.");

                if (cte.Empresa.StatusFinanceiro == "B")
                    return Json<bool>(true, false, "Empresa está com pendências, contate o setor de cadastros para maiores informações.");

                if (cte.Status == "R" && cte.MensagemRetornoSefaz.Contains("Digest") && cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                {
                    svcCTe.ConsultarDigestCTe(ref cte, unidadeDeTrabalho);

                    if (cte.Status == "E")
                    {
                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao); //svcCTe.AdicionarCTeNaFilaDeConsulta(cte);

                        return Json<bool>(true, false, "Aguarde, Consultando status de emissão do CT-e.");
                    }
                    else if (cte.Status == "A")
                    {
                        return Json<bool>(true, false, "Aguarde, Consultando status de emissão do CT-e.");
                    }
                    else
                        return Json<bool>(false, false, "Não foi possível consultar digest.");

                }
                else
                    return Json<bool>(false, false, "O status/retorno do CT-e deve ser Digest.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Falha ao consultar digest CT-e: " + ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar digest o CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ReenviarAverbacaoCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int serie, numeroInicial, numeroFinal = 0;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["Serie"], out serie);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["Remetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["Destinatario"]);
                string configAdicionarCTesFilaConsulta = !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"]) ? ConfigurationManager.AppSettings["AdicionarCTesFilaConsulta"] : "SIM";

                DateTime dataEmissaoInicial, dataEmissaoFinal;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                bool contem = false;
                bool.TryParse(Request.Params["Contem"], out contem);

                string placa = Request.Params["Placa"];
                string motorista = Request.Params["Motorista"];
                string status = Request.Params["Status"];
                string tipoOcorrencia = Request.Params["TipoOcorrencia"];
                string numeroNF = Request.Params["NumeroNF"];
                if (!string.IsNullOrWhiteSpace(numeroNF))
                    numeroNF = Utilidades.String.OnlyNumbers(numeroNF);

                Dominio.Enumeradores.TipoCTE tipoCTe;
                if (!Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["Finalidade"], out tipoCTe))
                    tipoCTe = Dominio.Enumeradores.TipoCTE.Todos;

                Dominio.Enumeradores.FiltroAverbacaoCTe? averbacaoCTe = Dominio.Enumeradores.FiltroAverbacaoCTe.NaoAverbados;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unidadeDeTrabalho);

                int countCTes = repCTe.ContarConsulta(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe);
                List<Dominio.ObjetosDeValor.ConsultaCTe> listaCTes = repCTe.Consultar(this.EmpresaUsuario.Codigo, dataEmissaoInicial, dataEmissaoFinal, numeroInicial, numeroFinal, placa, motorista, cpfCnpjRemetente, cpfCnpjDestinatario, status, tipoCTe, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), serie, tipoOcorrencia, numeroNF, contem, averbacaoCTe, 0, countCTes);

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    if (svcAverbacao.VerificaAverbacao(listaCTes[i].Codigo, listaCTes[i].Status == "A" ? Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao : Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento, unidadeDeTrabalho))
                        FilaConsultaCTe.GetInstance().QueueItem(1, listaCTes[i].Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, Conexao.StringConexao);

                    unidadeDeTrabalho.FlushAndClear();
                }

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar os conhecimentos de transporte.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico CTE = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);
                    if (CTE != null)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOriginal = CTE.TipoCTE != Dominio.Enumeradores.TipoCTE.Normal ? repCTe.BuscarPorChave(CTE.ChaveCTESubComp, this.EmpresaUsuario.Codigo) : null;
                        Dominio.Entidades.DocumentosAnulacaoCTE documentoAnulacao = null;
                        if (CTE.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                        {
                            Repositorio.DocumentosAnulacaoCTE repDocsAnulacao = new Repositorio.DocumentosAnulacaoCTE(unidadeDeTrabalho);
                            documentoAnulacao = repDocsAnulacao.BuscarPorCTe(this.EmpresaUsuario.Codigo, CTE.Codigo);
                        }
                        object cteJS = new
                        {
                            CTE.ExibeICMSNaDACTE,
                            CTE.AliquotaCOFINS,
                            CTE.AliquotaICMS,
                            CTE.AliquotaPIS,
                            CTE.BaseCalculoICMS,
                            CTE.BaseCOFINS,
                            CTE.BasePIS,
                            CFOP = CTE.CFOP.Codigo,
                            CTE.IndicadorGlobalizado,
                            IndicadorIETomador = CTE.IndicadorIETomador != null ? CTE.IndicadorIETomador : Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS,
                            NaturezaDaOperacao = CTE.NaturezaDaOperacao.Codigo,
                            CTE.Chave,
                            CTE.ChaveCTEReferenciado,
                            CTE.ChaveCTESubComp,
                            AliquotaICMSCTeSubComp = cteOriginal != null ? cteOriginal.AliquotaICMS : 0m,
                            CTE.Codigo,
                            CodigoCriptografado = Servicos.Criptografia.Criptografar(CTE.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                            CTE.Container,
                            CST = this.ObterCSTDoICMS(CTE),
                            CSTPIS = this.ObterCSTDoPIS(CTE.CSTPIS),
                            CSTCOFINS = this.ObterCSTDoCOFINS(CTE.CSTCOFINS),
                            DataAnulacao = CTE.DataAnulacao != null ? CTE.DataAnulacao.Value.ToString("dd/MM/yyyy") : "",
                            DataColeta = CTE.DataColeta != DateTime.MinValue ? string.Format("{0:dd/MM/yyyy}", CTE.DataColeta) : "",
                            DataEmissao = string.Format("{0:dd/MM/yyyy}", CTE.DataEmissao),
                            HoraEmissao = string.Format("{0:HH:mm}", CTE.DataEmissao),
                            DataPrevistaContainer = CTE.DataPrevistaContainer != DateTime.MinValue ? string.Format("{0:dd/MM/yyyy}", CTE.DataPrevistaContainer) : "",
                            DataPrevistaEntrega = CTE.DataPrevistaEntrega != DateTime.MinValue ? string.Format("{0:dd/MM/yyyy}", CTE.DataPrevistaEntrega) : "",
                            Destinatario = this.ObterParticipanteDoCTe(CTE, Dominio.Enumeradores.TipoTomador.Destinatario, unidadeDeTrabalho),
                            CTE.DetalhesRetira,
                            Expedidor = this.ObterParticipanteDoCTe(CTE, Dominio.Enumeradores.TipoTomador.Expedidor, unidadeDeTrabalho),
                            CTE.LacreContainer,
                            UFEmissao = CTE.LocalidadeEmissao.Estado.Sigla,
                            LocalidadeEmissao = CTE.LocalidadeEmissao.Codigo,
                            UFInicioPrestacao = CTE.LocalidadeInicioPrestacao.Estado.Sigla,
                            LocalidadeInicioPrestacao = CTE.LocalidadeInicioPrestacao.Codigo,
                            UFTerminoPrestacao = CTE.LocalidadeTerminoPrestacao.Estado.Sigla,
                            LocalidadeTerminoPrestacao = CTE.LocalidadeTerminoPrestacao.Codigo,
                            CTE.Lotacao,
                            ModalTransporte = CTE.ModalTransporte.Codigo,
                            ModeloDocumentoFiscal = CTE.ModeloDocumentoFiscal.Numero,
                            CTE.Numero,
                            CTE.NumeroColeta,
                            CTE.ObservacaoDaCarga,
                            CTE.ObservacoesGerais,
                            CTE.ObservacaoCancelamento,
                            CTE.ObservacoesDigitacao,
                            CTE.Protocolo,
                            CTE.ProtocoloCancelamentoInutilizacao,
                            Tomador = this.ObterParticipanteDoCTe(CTE, Dominio.Enumeradores.TipoTomador.Outros, unidadeDeTrabalho),
                            CTE.PercentualReducaoBaseCalculoICMS,
                            CTE.ProdutoPredominante,
                            CTE.OutrasCaracteristicasDaCarga,
                            CTE.CaracteristicaServico,
                            CTE.CaracteristicaTransporte,
                            Recebedor = this.ObterParticipanteDoCTe(CTE, Dominio.Enumeradores.TipoTomador.Recebedor, unidadeDeTrabalho),
                            Remetente = this.ObterParticipanteDoCTe(CTE, Dominio.Enumeradores.TipoTomador.Remetente, unidadeDeTrabalho),
                            CPF_CNPJ_Cliente_Entrega = CTE.ClienteEntrega != null ? CTE.ClienteEntrega.CPF_CNPJ_Formatado : "",
                            CTE.Retira,
                            CTE.RNTRC,
                            Serie = CTE.Serie.Codigo,
                            CTE.SerieColeta,
                            CTE.SimplesNacional,
                            CTE.Status,
                            CTE.TipoCTE,
                            CTE.TipoEmissao,
                            CTE.TipoImpressao,
                            CTE.TipoPagamento,
                            CTE.TipoServico,
                            CTE.TipoTomador,
                            CTE.ValorAReceber,
                            CTE.ValorCOFINS,
                            CTE.ValorICMS,
                            CTE.ValorICMSDevido,
                            CTE.ValorPIS,
                            CTE.ValorPrestacaoServico,
                            CTE.ValorPresumido,
                            CTE.ValorTotalMercadoria,
                            ValorCargaAverbacao = CTE.ValorCarbaAverbacao,
                            CTE.IncluirICMSNoFrete,
                            CTE.PercentualICMSIncluirNoFrete,
                            CTE.ValorFrete,
                            ValorFreteOriginal = cteOriginal != null ? cteOriginal.ValorFrete : 0,
                            PossuiCTeAnulacao = repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, CTE.Chave, Dominio.Enumeradores.TipoCTE.Anulacao) > 0 ? true : false,
                            PossuiCTeSubstituicao = repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, CTE.Chave, Dominio.Enumeradores.TipoCTE.Substituto) > 0 ? true : false,
                            DocumentoAnulacao = documentoAnulacao != null ? new { documentoAnulacao.Chave, documentoAnulacao.ContribuinteICMS, DataEmissao = documentoAnulacao.DataEmissao != null ? documentoAnulacao.DataEmissao.Value.ToString("dd/MM/yyyy") : "", CNPJ = documentoAnulacao.Emitente != null ? documentoAnulacao.Emitente.CPF_CNPJ.ToString("n0") : "", Modelo = documentoAnulacao.ModeloDocumentoFiscal.Codigo, documentoAnulacao.Numero, documentoAnulacao.Serie, documentoAnulacao.Subserie, documentoAnulacao.Tipo, documentoAnulacao.Valor } : null,
                            ChaveCTeAnulacao = cteOriginal == null ? repCTe.BuscarChavePorTipoEChaveCTeOriginal(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoCTE.Anulacao, CTE.Chave) : repCTe.BuscarChavePorTipoEChaveCTeOriginal(this.EmpresaUsuario.Codigo, Dominio.Enumeradores.TipoCTE.Anulacao, cteOriginal.Chave),
                            CTE.Log,
                            CTE.InformacaoAdicionalFisco,
                            CTE.CIOT,
                            CodigoRetornoSefaz = CTE.MensagemStatus != null ? CTE.MensagemStatus.CodigoDoErro : 0,
                            CTE.Versao,
                            CTE.ValorINSS,
                            CTE.ValorIR,
                            CTE.ValorCSLL,
                            DescricaoComplemento = CTE.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento ? CTE.DescricaoComplemento : string.Empty,
                            TipoFretamento = CTE.TipoFretamento != null ? CTE.TipoFretamento : Dominio.Enumeradores.TipoFretamento.Eventual,
                            DataHoraViagem = CTE.DataHoraViagem != DateTime.MinValue ? string.Format("{0:dd/MM/yyyy HH:mm}", CTE.DataHoraViagem) : "",
                            CTE.ValorBaseCalculoIR,
                            CTE.AliquotaIR,
                            CTE.ValorBaseCalculoINSS,
                            CTE.AliquotaINSS,
                            CTE.ValorBaseCalculoCSLL,
                            CTE.AliquotaCSLL,
                            CTE.CTeReferencia,
                            IndNegociavel = CTE.IndicadorNegociavel != null ? CTE.IndicadorNegociavel : Dominio.Enumeradores.OpcaoSimNao.Nao,
                            SubstituicaoTomador = CTE.SubstituicaoTomador ? "1" : "0",
                            CTE.ValorICMSDesoneracao,
                            CTE.CodigoBeneficio,
                            CodigoOutrasAliquotas = CTE.OutrasAliquotas?.Codigo ?? 0,
                            CTE.CSTIBSCBS,
                            CTE.ClassificacaoTributariaIBSCBS,
                            CTE.BaseCalculoIBSCBS,
                            CTE.AliquotaIBSEstadual,
                            CTE.PercentualReducaoIBSEstadual,
                            CTE.ValorIBSEstadual,
                            CTE.AliquotaIBSMunicipal,
                            CTE.PercentualReducaoIBSMunicipal,
                            CTE.ValorIBSMunicipal,
                            CTE.AliquotaCBS,
                            CTE.PercentualReducaoCBS,
                            CTE.ValorCBS
                        };
                        return Json(cteJS, true, null);
                    }
                    else
                    {
                        return Json<bool>(false, false, "CT-e não encontrado.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Código de CT-e inválido.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do CT-e. Atualize a página e tente novamente!");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Cancelar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para cancelamento do CT-e negada!");

                int codigoCTe = 0;
                if (!int.TryParse(Request.Params["CodigoCTe"], out codigoCTe) && codigoCTe <= 0)
                    return Json<bool>(false, false, "Código do CT-e inválido.");

                string justificativa = Request.Params["Justificativa"];
                string cobrarCancelamento = Request.Params["CobrarCancelamento"];

                if (justificativa.Length <= 20 || justificativa.Length >= 255)
                    return Json<bool>(false, false, "A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.");

                DateTime dataCancelamento;
                DateTime.TryParseExact(Request.Params["DataCancelamento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataCancelamento);

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);

                    if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Anulacao || cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Substituto)
                    {
                        return Json<bool>(false, false, "Não é possível cancelar um CT-e com esta finalidade.");
                    }
                    else
                    {
                        if (repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, cte.Chave, Dominio.Enumeradores.TipoCTE.Anulacao) > 0)
                            return Json<bool>(false, false, "Não é possível cancelar este CT-e pois ele já foi anulado.");

                        else if (repCTe.ContarPorChaveDoCTeOriginalETipo(this.EmpresaUsuario.Codigo, cte.Chave, Dominio.Enumeradores.TipoCTE.Substituto) > 0)
                            return Json<bool>(false, false, "Não é possível cancelar este CT-e pois ele já foi substituído.");
                    }


                    if (cte.Status != "A")
                        return Json<bool>(false, false, "O status do CT-e não permite o cancelamento do mesmo!");

                    if (cte.StatusIntegrador == "Z")
                        return Json<bool>(false, false, "Não é possível alterar um CT-e que foi importado!");

                    int limiteCancelamentoCTe = this.TempoMaximoCancelarCTe(cte);
                    bool permiteCancelamento = (DateTime.Now - cte.DataRetornoSefaz.Value).TotalHours >= limiteCancelamentoCTe;
                    if (limiteCancelamentoCTe > 0 && permiteCancelamento)
                        return Json<bool>(false, false, "A data de emissão do CT-e não permite o cancelamento do mesmo. O período máximo para cancelamento é de " + limiteCancelamentoCTe.ToString() + " hora" + (limiteCancelamentoCTe > 1 ? "s" : "") + "!");

                    Servicos.CTe svcCancelamento = new Servicos.CTe(unidadeDeTrabalho);
                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(codigoCTe, this.EmpresaUsuario.Codigo, justificativa, unidadeDeTrabalho, dataCancelamento, true, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo : this.Usuario, cobrarCancelamento))
                    {
                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                        svcCancelamento.AtualizarIntegracaoRetornoCTe(cte, unidadeDeTrabalho);

                        return Json<bool>(true, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "Não foi possível cancelar o CT-e.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar o CT-e");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Inutilizar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeDelecao != "A")
                    return Json<bool>(false, false, "Permissão para inutilização do CT-e negada!");

                int codigoCTe = 0;
                if (!int.TryParse(Request.Params["CodigoCTe"], out codigoCTe) && codigoCTe <= 0)
                    return Json<bool>(false, false, "Código do CT-e inválido.");
                string justificativa = Request.Params["Justificativa"];
                if (justificativa.Length <= 20 || justificativa.Length >= 255)
                    return Json<bool>(false, false, "A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.");
                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);

                    if (cte.Status != "R" && cte.Status != "S")
                        return Json<bool>(false, false, "O status do CT-e não permite a inutilização do mesmo!");

                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                    if (svcCTe.Inutilizar(codigoCTe, this.EmpresaUsuario.Codigo, justificativa, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe, null, true, this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo : this.Usuario))
                    {
                        if (cte.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(1, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.CTe, Conexao.StringConexao);
                        return Json<bool>(true, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, "Não foi possível inutilizar o CT-e.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao inutilizar o CT-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarPorEmail()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                string emails = Request.Params["Emails"];
                if (string.IsNullOrWhiteSpace(Request.Params["Emails"]))
                    return Json<bool>(false, false, "E-mails inválidos.");
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);
                if (cte != null)
                {
                    if (!(cte.Status.Equals("E") || cte.Status.Equals("R")))
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                        if (svcCTe.EnviarEmail(cte.Codigo, this.EmpresaUsuario.Codigo, emails, unidadeDeTrabalho))
                            return Json<bool>(true, true);
                        else
                            return Json<bool>(false, false, "Não foi possível enviar o e-mail. Atualize a página e tente novamente!");
                    }
                    else
                    {
                        return Json<bool>(false, false, "O status do CT-e é inválido para o envio do e-mail.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar o envio do e-mail.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EnviarEmailParaTodos()
        {
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);
                if (cte != null)
                {
                    if (!(cte.Status.Equals("E") || cte.Status.Equals("R")))
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        if (svcCTe.EnviarEmail(cte.Codigo, this.EmpresaUsuario.Codigo, unitOfWork))
                            return Json<bool>(true, true);
                        else
                            return Json<bool>(false, false, "Não foi possível enviar o e-mail. Atualize a página e tente novamente!");
                    }
                    else
                    {
                        return Json<bool>(false, false, "O status do CT-e é inválido para o envio do e-mail.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar o envio do e-mail.");
            }
        }

        [AcceptVerbs("POST")]
        public async Task<ActionResult> GerarPreCTe()
        {
            UnitOfWork unitOfWork = new UnitOfWork(Conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params["CodigoCTe"], out int codigoCTe);

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        string path = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoUploadXMLNotasFiscais"], this.EmpresaUsuario.Codigo.ToString());

                        path = Utilidades.IO.FileStorageService.Storage.Combine(path, "CTe");

                        Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(path, file.FileName), file.InputStream);

                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTePorPreCTe(file.InputStream, this.EmpresaUsuario.Codigo, this.Usuario.Codigo, true, codigoCTe);

                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                return Json<bool>(false, false, (string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico) || retorno.GetType().Name == "ConhecimentoDeTransporteEletronicoProxy" || retorno.GetType().Name == "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor")
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                return Json(new { cte.Codigo, cte.Numero, Serie = cte.Serie.Numero, cte.Chave }, true);
                            }
                            else
                            {
                                return Json<bool>(false, false, "Conhecimento de transporte inválido.");
                            }
                        }
                        else
                        {
                            return Json<bool>(false, false, "Ocorreu uma falha genérica ao gerar o CT-e.");
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, "A extensão do arquivo é inválida. Somente a extensão XML é aceita.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler/salvar o arquivo XML.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarCTePorListaNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.NFeAdmin> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NFeAdmin>>(Request.Params["Documentos"]);

                DateTime dataEmissao = DateTime.Now;
                DateTime.TryParseExact(Request.Params["DataEmissao"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                decimal valorFrete, valorTotalMercadoria = 0;
                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorTotalMercadoria"], out valorTotalMercadoria);

                string observacao = Request.Params["Observacao"];

                return this.GerarCTePorListaNFe(documentos, valorFrete, valorTotalMercadoria, observacao, dataEmissao, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarCTePorNOTFIS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                if (this.EmpresaUsuario.Configuracao == null)
                    return Json<bool>(false, false, "A empresa emissora não possui as configurações necessárias para a importação de NOTFIS.");

                if (this.EmpresaUsuario.Configuracao.SerieInterestadual == null || this.EmpresaUsuario.Configuracao.SerieIntraestadual == null)
                    return Json<bool>(false, false, "Não há uma série configurada para a importação de NOTFIS.");

                int codigoLayout = 0;
                int codigoTabelaFreteValor = 0;
                int codigoVeiculoTracao = 0;
                int codigoVeiculoReboque = 0;
                int codigoMotorista = 0;
                decimal valorFrete = 0;
                decimal valorPedagio = 0;
                decimal percentualGris = 0;
                decimal percentualAdvalorem = 0;

                int.TryParse(Request.Params["CodigoLayout"], out codigoLayout);
                int.TryParse(Request.Params["CodigoTabelaFreteValor"], out codigoTabelaFreteValor);
                int.TryParse(Request.Params["CodigoVeiculoTracao"], out codigoVeiculoTracao);
                int.TryParse(Request.Params["CodigoVeiculoReboque"], out codigoVeiculoReboque);
                int.TryParse(Request.Params["CodigoMotorista"], out codigoMotorista);

                decimal.TryParse(Request.Params["ValorFrete"], out valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out valorPedagio);
                decimal.TryParse(Request.Params["PercentualGris"], out percentualGris);
                decimal.TryParse(Request.Params["PercentualAdValorem"], out percentualAdvalorem);

                bool manterCTeDigitacao = true;
                bool incluirICMS = true;
                bool.TryParse(Request.Params["ManterDigitacao"], out manterCTeDigitacao);
                bool.TryParse(Request.Params["IncluirICMS"], out incluirICMS);

                Repositorio.FretePorValor repFretePorValor = new Repositorio.FretePorValor(unitOfWork);
                if (codigoTabelaFreteValor > 0)
                {
                    Dominio.Entidades.FretePorValor fretePorValor = repFretePorValor.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoTabelaFreteValor);
                    if (fretePorValor != null)
                    {
                        if (fretePorValor.TipoRateio == Dominio.Enumeradores.TipoRateioTabelaFreteValor.Peso)
                            return Json<bool>(false, false, "Tabela de frete com rateio por Peso não disponível para importação de NOTFIS.");
                    }
                }

                Dominio.Entidades.LayoutEDI layout = null;
                if (EmpresaUsuario.Configuracao != null && EmpresaUsuario.Configuracao.UtilizaNovaImportacaoEDI)
                    layout = (from obj in this.EmpresaUsuario.LayoutsEDI where obj.Codigo == codigoLayout && obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS_NOVA_IMPORTACAO select obj).FirstOrDefault();
                else
                    layout = (from obj in this.EmpresaUsuario.LayoutsEDI where obj.Codigo == codigoLayout && obj.Tipo == Dominio.Enumeradores.TipoLayoutEDI.NOTFIS select obj).FirstOrDefault();

                if (layout != null)
                {
                    if (EmpresaUsuario.Configuracao != null && EmpresaUsuario.Configuracao.UtilizaNovaImportacaoEDI)
                    {

                        if (layout.CamposPorIndices)
                        {
                            Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, layout, Request.Files[0].InputStream, unitOfWork, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8); //Encoding.GetEncoding("iso-8859-1")

                            List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notasFiscais = leituraEDI.LerNotasFiscais(this.EmpresaUsuario.Configuracao.NaoImportarNotaDuplicadaEDINovaImportacao);

                            object documento = GerarDocumentosListaNotas(EmpresaUsuario.Codigo, notasFiscais, unitOfWork);

                            return Json(documento, true);
                        }
                        else
                        {
                            ThreadExecutada = false;
                            Sucesso = false;
                            LayoutEDI = layout;
                            ArquivoEDI = Request.Files[0].InputStream;

                            int executionCount = 0;

                            Thread thread = new Thread(GerarNOTFIS, 8000000);                            
                            thread.Start();

                            while (!ThreadExecutada)
                            {
                                executionCount++;

                                if (executionCount == 40)
                                {
                                    thread.Abort();
                                    return Json<bool>(false, false, "Ocorreu uma falha ao ler o NOTFIS. Tempo de execução muito longo.");
                                }

                                System.Threading.Thread.Sleep(500);

                                if (ThreadExecutada)
                                {
                                    if (Sucesso)
                                    {
                                        object documento = GerarDocumentosListaNOTFIS(EmpresaUsuario.Codigo, NOTFIS, this.EmpresaUsuario.Configuracao.NaoImportarNotaDuplicadaEDINovaImportacao, unitOfWork);

                                        return Json(documento, true);
                                    }
                                    else
                                        return Json<bool>(false, false, "Ocorreu uma falha ao ler o NOTFIS.");
                                }
                            }

                            return Json<bool>(false, false, "Arquivo EDI não importado.");

                            //Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis notfis = leituraEDI.GerarNotasFis();

                            //object documento = GerarDocumentosListaNOTFIS(EmpresaUsuario.Codigo, notfis, unitOfWork);

                            //return Json(documento, true);
                        }

                    }
                    else
                    {
                        Servicos.LeituraEDI svcLeituraEDI = new Servicos.LeituraEDI(this.EmpresaUsuario, layout, Request.Files[0].InputStream, unitOfWork, codigoTabelaFreteValor, valorFrete, valorPedagio, percentualGris, percentualAdvalorem, codigoVeiculoTracao, codigoVeiculoReboque, codigoMotorista, manterCTeDigitacao, incluirICMS, Encoding.UTF8, this.Usuario);

                        svcLeituraEDI.GerarCTes();

                        return Json<bool>(true, true);
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Layout de EDI não encontrado. Atualize a página e tente novamente.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler o arquivo NOTFIS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult SalvarCTePorXMLNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                bool agruparRemetente, agruparDestinatario, agruparUFDestino, manterDigitacao = true;
                bool.TryParse(Request.Params["AgruparRemetente"], out agruparRemetente);
                bool.TryParse(Request.Params["AgruparDestinatario"], out agruparDestinatario);
                bool.TryParse(Request.Params["AgruparUFDestino"], out agruparUFDestino);
                bool.TryParse(Request.Params["AgruparDT"], out bool agruparDT);

                bool.TryParse(Request.Params["ManterDigitacao"], out manterDigitacao);

                int.TryParse(Request.Params["CodigoTabelaFreteValor"], out int codigoTabelaFreteValor);
                int.TryParse(Request.Params["CodigoVeiculo"], out int codigoTracao);
                int.TryParse(Request.Params["CodigoReboque"], out int codigoReboque);
                int.TryParse(Request.Params["CodigoMotorista"], out int codigoMotorista);
                int.TryParse(Request.Params["CodigoSeguro"], out int codigoSeguro);

                string observacaoCTe = Request.Params["ObservacaoCTe"];
                string expedidorCTe = Request.Params["ExpedidorCTe"];
                string recebedorCTe = Request.Params["RecebedorCTe"];

                decimal.TryParse(Request.Params["ValorFrete"], out decimal valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out decimal valorPedagio);
                decimal.TryParse(Request.Params["ValorAdicionalEntrega"], out decimal valorAdicionalEntrega);
                Enum.TryParse(Request.Params["TipoRateio"], out Dominio.Enumeradores.TipoRateioTabelaFreteValor tipoRateio);

                Dominio.Enumeradores.TipoTomador? tipoTomador = null;
                if (Enum.TryParse(Request.Params["TipoTomador"], out Dominio.Enumeradores.TipoTomador tipoTomadorAux))
                    tipoTomador = tipoTomadorAux;

                string usuario = this.UsuarioAdministrativo != null ? string.Concat(this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome) : string.Concat(this.Usuario.CPF, " - ", this.Usuario.Nome);
                string agrupamento = agruparRemetente && agruparDestinatario ? "Remetente e Destinatario" : agruparRemetente ? "Remetente" : agruparDestinatario ? "Destinatario" : agruparUFDestino ? "UF Destino" : "Um CTe por XML NFe";
                Servicos.Log.TratarErro("Usuário " + usuario + " / Empresa: " + this.EmpresaUsuario.CNPJ + " / Agrupamento: " + agrupamento, "ImportacaoNFe");

                List<Dominio.ObjetosDeValor.XMLNFe> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.XMLNFe>>(Request.Params["NFes"]);

                documentos = documentos.OrderBy(obj => obj.Numero).ToList();

                List<Dominio.ObjetosDeValor.XMLNFe> documentosImportar = new List<Dominio.ObjetosDeValor.XMLNFe>();

                foreach (Dominio.ObjetosDeValor.XMLNFe documento in documentos)
                {
                    bool importarNota = true;
                    if (this.EmpresaUsuario.Configuracao.NaoImportarNotaDuplicadaEDINovaImportacao)
                        importarNota = string.IsNullOrWhiteSpace(documento.Chave) || (from obj in documentosImportar where obj.Chave == documento.Chave select obj).Count() == 0;

                    if (importarNota)
                    {
                        documento.Adicionada = false;
                        documentosImportar.Add(documento);
                    }
                }

                Servicos.CTe svcCTe = new CTe(unitOfWork);
                bool notaSalva = false;
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = svcCTe.SalvarCTePorObjetoNFe(this.EmpresaUsuario, this.UsuarioAdministrativo, this.Usuario, agruparRemetente, agruparDestinatario, agruparUFDestino, agruparDT, documentosImportar, unitOfWork, codigoTabelaFreteValor, codigoTracao, codigoReboque, codigoMotorista, manterDigitacao, valorFrete, valorPedagio, 0, tipoRateio, tipoTomador, codigoSeguro, observacaoCTe, recebedorCTe, expedidorCTe, valorAdicionalEntrega, ref notaSalva);

                if ((listaCTes != null && listaCTes.Count > 0) || notaSalva)
                    return Json<bool>(true, true);
                else
                    return Json<bool>(false, false, "Ocorreu uma falha ao salvar CTes.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao salvar CTes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult GerarCTeMercadoLivre()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                string tokenMercadoLivre = Servicos.MercadoLivre.ObterToken(this.EmpresaUsuario);

                if (string.IsNullOrWhiteSpace(tokenMercadoLivre))
                    return Json<bool>(false, false, "Não foi possível gerar token de acesso no Mercado Livre, contate o suporte para verificação!");

                List<Dominio.ObjetosDeValor.MercadoLivre.Barras> listaCodigosBarras = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MercadoLivre.Barras>>(Request.Params["CodigosBarras"]);

                if (listaCodigosBarras == null || listaCodigosBarras.Count <= 0)
                    return Json<bool>(false, false, "Nenhum código de barras informado.");

                string tomadorCTe = Request.Params["Tomador"];
                string tomadorCTe2 = Request.Params["Tomador2"];
                string expedidorCTe = Request.Params["Expedidor"];
                string recebedorCTe = Request.Params["Recebedor"];
                string observacaoCTe = Request.Params["ObservacaoCTe"];

                int.TryParse(Request.Params["CodigoVeiculo"], out int codigoTracao);
                int.TryParse(Request.Params["CodigoReboque"], out int codigoReboque);
                int.TryParse(Request.Params["CodigoMotorista"], out int codigoMotorista);

                decimal.TryParse(Request.Params["ValorFrete"], out decimal valorFrete);
                decimal.TryParse(Request.Params["ValorPedagio"], out decimal valorPedagio);
                decimal.TryParse(Request.Params["ValorOutros"], out decimal valorOutros);
                decimal.TryParse(Request.Params["PercentualGris"], out decimal percentualGris);

                Dominio.Entidades.Cliente tomador = !string.IsNullOrWhiteSpace(tomadorCTe) && tomadorCTe != "0" ? repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(tomadorCTe))) : null;
                Dominio.Entidades.Cliente tomador2 = !string.IsNullOrWhiteSpace(tomadorCTe2) && tomadorCTe != "0" ? repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(tomadorCTe2))) : null;
                Dominio.Entidades.Cliente expedidor = !string.IsNullOrWhiteSpace(expedidorCTe) && expedidorCTe != "0" ? repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(expedidorCTe))) : null;
                Dominio.Entidades.Cliente recebedor = !string.IsNullOrWhiteSpace(recebedorCTe) && recebedorCTe != "0" ? repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(recebedorCTe))) : null;
                Dominio.Entidades.Veiculo tracao = codigoTracao > 0 ? repVeiculo.BuscarPorCodigo(codigoTracao) : null;
                Dominio.Entidades.Veiculo reboque = codigoReboque > 0 ? repVeiculo.BuscarPorCodigo(codigoReboque) : null;
                Dominio.Entidades.Usuario motorista = codigoMotorista > 0 ? repMotorista.BuscarMotoristaPorCodigo(codigoMotorista) : null;

                if (tomador == null)
                    return Json<bool>(false, false, "Obrigatório informar um tomador.");

                if (tomador2 == null)
                    return Json<bool>(false, false, "Obrigatório informar um tomador 2.");

                if (expedidor == null)
                    return Json<bool>(false, false, "Obrigatório informar um expedidor.");

                if (recebedor == null)
                    return Json<bool>(false, false, "Obrigatório informar um recebedor.");

                if (valorFrete <= 0)
                    return Json<bool>(false, false, "Obrigatório informar um valor de frete.");

                if (tomador.RaizCnpj == tomador2.RaizCnpj)
                    return Json<bool>(false, false, "Obrigatório informar tomadores com Raiz de CNPJ diferente.");

                List<Dominio.ObjetosDeValor.MercadoLivre.HandlingUnit> listaHandlingUnit = Servicos.MercadoLivre.ObterShipmentID(this.EmpresaUsuario, tokenMercadoLivre, listaCodigosBarras);
                string mensagemRetorno = Servicos.MercadoLivre.SalvarCTePorHandlingUnit(this.EmpresaUsuario, tomador, tomador2, expedidor, recebedor, tracao, reboque, motorista, valorFrete, valorPedagio, valorOutros, percentualGris, observacaoCTe, this.UsuarioAdministrativo, this.Usuario, listaHandlingUnit, tokenMercadoLivre, unitOfWork);

                return Json<bool>(true, true, mensagemRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public async Task<ActionResult> GerarCTeAnterior()
        {
            UnitOfWork unitOfWork = new UnitOfWork(Conexao.StringConexao);
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        string path = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoUploadXMLNotasFiscais"], this.EmpresaUsuario.Codigo.ToString());

                        path = Utilidades.IO.FileStorageService.Storage.Combine(path, "CTe Anterior");

                        Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(path, file.FileName), file.InputStream);

                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        object retorno = svcCTe.GerarCTeAnterior(file.InputStream, this.EmpresaUsuario.Codigo, string.Empty, string.Empty);

                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                return Json<bool>(false, false, (string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                return Json(new { cte.Numero, Serie = cte.Serie.Numero, cte.Chave }, true);
                            }
                            else if (retorno.GetType().ToString() == "ConhecimentoDeTransporteEletronicoProxy" || retorno.GetType().ToString() == "ConhecimentoDeTransporteEletronicoProxyForFieldInterceptor")
                            {
                                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retorno;
                                return Json(new { cte.Numero, Serie = cte.Serie.Numero, cte.Chave }, true);
                            }
                            else
                            {
                                return Json<bool>(false, false, "Conhecimento de transporte inválido.");
                            }
                        }
                        else
                        {
                            return Json(true, true);
                        }
                    }
                    else
                    {
                        return Json<bool>(false, false, string.Concat("A extensão do arquivo '", file.FileName, "' é inválida. Somente a extensão XML é aceita."));
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler/salvar o arquivo XML.");
            }
            finally 
            {
                await unitOfWork.DisposeAsync();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterInformacoesReferencia()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                double cpfCnpjRemetente, cpfCnpjDestinatario, cpfCnpjExpedidor, cpfCnpjRecebedor = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]), out cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]), out cpfCnpjDestinatario);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJExpedidor"]), out cpfCnpjExpedidor);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRecebedor"]), out cpfCnpjRecebedor);

                int.TryParse(Request.Params["LocalidadeInicioPrestacao"], out int codigoOrigem);
                int.TryParse(Request.Params["LocalidadeTerminoPrestacao"], out int codigoDestino);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
                Repositorio.ObservacaoFiscoCTE repObsFisco = new Repositorio.ObservacaoFiscoCTE(unidadeDeTrabalho);
                Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
                Repositorio.SeguroCTE repSeguro = new Repositorio.SeguroCTE(unidadeDeTrabalho);

                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);
                Dominio.Entidades.Cliente expedidor = cpfCnpjExpedidor > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjExpedidor) : null;
                Dominio.Entidades.Cliente recebedor = cpfCnpjRecebedor > 0 ? repCliente.BuscarPorCPFCNPJ(cpfCnpjRecebedor) : null;

                Dominio.Entidades.Estado estadoInicioPrestacao = repEstado.BuscarPorSigla(Request.Params["EstadoInicioPrestacao"]);
                Dominio.Entidades.Estado estadoFimPrestacao = repEstado.BuscarPorSigla(Request.Params["EstadoFimPrestacao"]);

                if (estadoInicioPrestacao == null || estadoFimPrestacao == null || remetente == null || destinatario == null)
                    return Json<bool>(false, false, "Inicio da prestação, fim da prestação, remetente ou destinatário inválidos.");

                Dominio.ObjetosDeValor.InformacaoSeguro seguroPadrao = null;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.EmpresaSerie serie = svcCTe.ObterSerie(this.EmpresaUsuario, this.EmpresaUsuario.Localidade.Estado.Sigla, estadoInicioPrestacao.Sigla, estadoFimPrestacao.Sigla, remetente?.CPF_CNPJ_SemFormato ?? string.Empty, destinatario?.CPF_CNPJ_SemFormato ?? string.Empty, string.Empty, string.Empty, string.Empty, unidadeDeTrabalho);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cteReferencia = null;

                if (System.Configuration.ConfigurationManager.AppSettings["IdentificacaoAmbiente"] == "Sintravir")
                    cteReferencia = repCTe.BuscarReferenciaSintravir(this.EmpresaUsuario.Codigo, remetente.CPF_CNPJ_SemFormato, destinatario?.CPF_CNPJ_SemFormato, expedidor?.CPF_CNPJ_SemFormato, recebedor?.CPF_CNPJ_SemFormato, codigoOrigem, codigoDestino, Dominio.Enumeradores.TipoServico.Normal);
                else
                    cteReferencia = repCTe.BuscarReferenciaPorClienteOrigemDestino(this.EmpresaUsuario.Codigo, remetente.CPF_CNPJ_SemFormato, estadoInicioPrestacao.Sigla, estadoFimPrestacao.Sigla, Dominio.Enumeradores.TipoServico.Normal);

                if (cteReferencia == null)
                {
                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, "Nenhum CT-e referencia encontrado.");
                }

                bool copiarValoresCTeAnterior = !(this.EmpresaUsuario.Configuracao?.NaoCopiarValoresCTeAnterior ?? false);

                List<Dominio.Entidades.ObservacaoContribuinteCTE> observacoesContribuinte = new List<Dominio.Entidades.ObservacaoContribuinteCTE>();
                List<Dominio.Entidades.ObservacaoFiscoCTE> observacoesFisco = new List<Dominio.Entidades.ObservacaoFiscoCTE>(); ;
                List<Dominio.Entidades.ComponentePrestacaoCTE> componentesDaPrestacao = repComponentePrestacao.BuscarPorCTe(this.EmpresaUsuario.Codigo, cteReferencia.Codigo);
                List<Dominio.Entidades.SeguroCTE> seguros = repSeguro.BuscarPorCTe(this.EmpresaUsuario.Codigo, cteReferencia.Codigo);

                if ((seguros == null || seguros.Count() == 0))
                    svcCTe.BuscarApolicesSeguro(cteReferencia, this.EmpresaUsuario, unidadeDeTrabalho, out seguroPadrao);

                var retorno = new
                {
                    NumeroCTe = cteReferencia.Numero,
                    Remetente = cteReferencia.Remetente != null ? cteReferencia.Remetente.Nome : "",
                    Destinatario = cteReferencia.Destinatario != null ? cteReferencia.Destinatario.Nome : "",
                    CodigoNatureza = cteReferencia.NaturezaDaOperacao.Codigo,
                    CodigoCFOP = cteReferencia.CFOP.Codigo,
                    AliquotaICMS = cteReferencia.AliquotaICMS,
                    CSTICMS = this.ObterCSTDoICMS(cteReferencia),
                    IncluirICMS = cteReferencia.IncluirICMSNoFrete,
                    PercentualIncluirICMS = cteReferencia.PercentualICMSIncluirNoFrete,
                    cteReferencia.ExibeICMSNaDACTE,
                    ValorFrete = copiarValoresCTeAnterior ? cteReferencia.ValorFrete : 0,
                    cteReferencia.TipoPagamento,
                    TipoTomador = cteReferencia.TipoTomador,
                    cteReferencia.ObservacoesGerais,
                    ObservacoesFisco = from obj in observacoesFisco select new Dominio.ObjetosDeValor.ObservacaoCTE { Identificador = obj.Identificador, Descricao = obj.Descricao, Codigo = -obj.Codigo, Excluir = false },
                    ObservacoesContribuinte = from obj in observacoesContribuinte select new Dominio.ObjetosDeValor.ObservacaoCTE { Identificador = obj.Identificador, Descricao = obj.Descricao, Codigo = -obj.Codigo, Excluir = false },
                    ComponentesPrestacao = copiarValoresCTeAnterior ? from obj in componentesDaPrestacao select new Dominio.ObjetosDeValor.ComponenteDaPrestacao { Descricao = obj.Nome, Excluir = false, Id = -obj.Codigo, IncluiBaseCalculoICMS = obj.IncluiNaBaseDeCalculoDoICMS, IncluiValorAReceber = obj.IncluiNoTotalAReceber, Valor = obj.Valor } : new Dominio.ObjetosDeValor.ComponenteDaPrestacao[] { },
                    InformacoesSeguro = seguros != null && seguros.Count() > 0 ? (from obj in seguros select new Dominio.ObjetosDeValor.InformacaoSeguro { DescricaoResponsavel = obj.DescricaoTipo, Excluir = false, Id = -obj.Codigo, NumeroApolice = obj.NumeroApolice, NumeroAverberacao = string.Empty, Responsavel = obj.Tipo, Seguradora = obj.NomeSeguradora, CNPJSeguradora = obj.CNPJSeguradora, ValorMercadoria = 0 }) : new Dominio.ObjetosDeValor.InformacaoSeguro[] { seguroPadrao },
                    ClientesIguais = true,
                    cteReferencia.TipoServico,
                    cteReferencia.ProdutoPredominante,
                    Serie = serie != null ? serie.Codigo : 0,
                    CodigoCTEReferenciado = cteReferencia.Codigo
                };

                unidadeDeTrabalho.CommitChanges();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter as informações de CT-e Referencia.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterInformacoesSemelhantes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                double cpfCnpjRemetente, cpfCnpjDestinatario = 0f;
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]), out cpfCnpjRemetente);
                double.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]), out cpfCnpjDestinatario);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);

                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
                Repositorio.ObservacaoFiscoCTE repObsFisco = new Repositorio.ObservacaoFiscoCTE(unidadeDeTrabalho);
                Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
                Repositorio.SeguroCTE repSeguro = new Repositorio.SeguroCTE(unidadeDeTrabalho);

                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(cpfCnpjRemetente);
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(cpfCnpjDestinatario);

                Dominio.Entidades.Estado estadoInicioPrestacao = repEstado.BuscarPorSigla(Request.Params["EstadoInicioPrestacao"]);
                Dominio.Entidades.Estado estadoFimPrestacao = repEstado.BuscarPorSigla(Request.Params["EstadoFimPrestacao"]);

                if (estadoInicioPrestacao == null || estadoFimPrestacao == null || remetente == null || destinatario == null)
                    return Json<bool>(false, false, "Inicio da prestação, fim da prestação, remetente ou destinatário inválidos.");

                bool clientesIguais = false;
                Dominio.ObjetosDeValor.InformacaoSeguro seguroPadrao = null;

                unidadeDeTrabalho.Start(System.Data.IsolationLevel.ReadUncommitted);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Dominio.Entidades.EmpresaSerie serie = svcCTe.ObterSerie(this.EmpresaUsuario, this.EmpresaUsuario.Localidade.Estado.Sigla, estadoInicioPrestacao.Sigla, estadoFimPrestacao.Sigla, remetente?.CPF_CNPJ_SemFormato ?? string.Empty, destinatario?.CPF_CNPJ_SemFormato ?? string.Empty, string.Empty, string.Empty, string.Empty, unidadeDeTrabalho);



                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorRemetenteEDestinatario(this.EmpresaUsuario.Codigo, remetente, destinatario, this.EmpresaUsuario.TipoAmbiente);

                if (cte == null)
                    cte = repCTe.BuscarPorUFInicioPrestacaoEUFFimPrestacao(this.EmpresaUsuario.Codigo, estadoInicioPrestacao, estadoFimPrestacao, this.EmpresaUsuario.TipoAmbiente);
                else
                    clientesIguais = true;

                if (cte == null)
                {
                    unidadeDeTrabalho.Rollback();

                    return Json<bool>(false, false, "Nenhum CT-e semelhante encontrado.");
                }

                bool copiaImpostosCTeAnterior = !(this.EmpresaUsuario.Configuracao?.NaoCopiarImpostosCTeAnterior ?? false);
                bool copiaSeguroCTeAnterior = !(cte.Empresa.Configuracao?.NaoCopiarSeguroCTeAnterior ?? false);
                bool naoCopiarTomadorCTeAnterior = this.UsuarioAdministrativo == null ? false : this.UsuarioAdministrativo.Empresa.Configuracao != null ? this.UsuarioAdministrativo.Empresa.Configuracao.NaoCarregarTomadorCTes : false;
                bool copiarObservacaoFiscoContribuinteCTeAnterior = cte.Empresa.Configuracao?.CopiarObservacaoFiscoContribuinteCTeAnterior ?? false;
                bool copiarValoresCTeAnterior = !(this.EmpresaUsuario.Configuracao?.NaoCopiarValoresCTeAnterior ?? false);

                List<Dominio.Entidades.ObservacaoContribuinteCTE> observacoesContribuinte = copiarObservacaoFiscoContribuinteCTeAnterior ? repObsContribuinte.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo) : new List<Dominio.Entidades.ObservacaoContribuinteCTE>();
                List<Dominio.Entidades.ObservacaoFiscoCTE> observacoesFisco = copiarObservacaoFiscoContribuinteCTeAnterior ? repObsFisco.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo) : new List<Dominio.Entidades.ObservacaoFiscoCTE>(); ;
                List<Dominio.Entidades.ComponentePrestacaoCTE> componentesDaPrestacao = repComponentePrestacao.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.SeguroCTE> seguros = new List<Dominio.Entidades.SeguroCTE>();
                if (copiaSeguroCTeAnterior)
                    seguros = repSeguro.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);

                if ((seguros == null || seguros.Count() == 0))
                    svcCTe.BuscarApolicesSeguro(cte, this.EmpresaUsuario, unidadeDeTrabalho, out seguroPadrao);

                var retorno = new
                {
                    NumeroCTe = cte.Numero,
                    Remetente = cte.Remetente != null ? cte.Remetente.Nome : "",
                    Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : "",
                    CodigoNatureza = copiaImpostosCTeAnterior ? cte.NaturezaDaOperacao.Codigo : 0,
                    CodigoCFOP = copiaImpostosCTeAnterior ? cte.CFOP.Codigo : 0,
                    AliquotaICMS = copiaImpostosCTeAnterior ? cte.AliquotaICMS : 0,
                    CSTICMS = copiaImpostosCTeAnterior ? this.ObterCSTDoICMS(cte) : 0,
                    IncluirICMS = cte.IncluirICMSNoFrete,
                    PercentualIncluirICMS = cte.PercentualICMSIncluirNoFrete,
                    cte.ExibeICMSNaDACTE,
                    ValorFrete = copiarValoresCTeAnterior ? cte.ValorFrete : 0,
                    cte.TipoPagamento,
                    TipoTomador = naoCopiarTomadorCTeAnterior ? Dominio.Enumeradores.TipoTomador.NaoInformado : cte.TipoTomador,
                    cte.ObservacoesGerais,
                    ObservacoesFisco = from obj in observacoesFisco select new Dominio.ObjetosDeValor.ObservacaoCTE { Identificador = obj.Identificador, Descricao = obj.Descricao, Codigo = -obj.Codigo, Excluir = false },
                    ObservacoesContribuinte = from obj in observacoesContribuinte select new Dominio.ObjetosDeValor.ObservacaoCTE { Identificador = obj.Identificador, Descricao = obj.Descricao, Codigo = -obj.Codigo, Excluir = false },
                    ComponentesPrestacao = copiarValoresCTeAnterior ? from obj in componentesDaPrestacao select new Dominio.ObjetosDeValor.ComponenteDaPrestacao { Descricao = obj.Nome, Excluir = false, Id = -obj.Codigo, IncluiBaseCalculoICMS = obj.IncluiNaBaseDeCalculoDoICMS, IncluiValorAReceber = obj.IncluiNoTotalAReceber, Valor = obj.Valor } : new Dominio.ObjetosDeValor.ComponenteDaPrestacao[] { },
                    InformacoesSeguro = seguros != null && seguros.Count() > 0 ? (from obj in seguros select new Dominio.ObjetosDeValor.InformacaoSeguro { DescricaoResponsavel = obj.DescricaoTipo, Excluir = false, Id = -obj.Codigo, NumeroApolice = obj.NumeroApolice, NumeroAverberacao = string.Empty, Responsavel = obj.Tipo, Seguradora = obj.NomeSeguradora, CNPJSeguradora = obj.CNPJSeguradora, ValorMercadoria = 0 }) : new Dominio.ObjetosDeValor.InformacaoSeguro[] { seguroPadrao },
                    ClientesIguais = clientesIguais,
                    cte.TipoServico,
                    cte.ProdutoPredominante,
                    Serie = serie != null ? serie.Codigo : 0,
                    CodigoCTEReferenciado = 0
                };

                unidadeDeTrabalho.CommitChanges();

                return Json(retorno, true);
            }

            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter as informações de CT-e semelhante.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarDadosImpostos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Dominio.Enumeradores.TipoTomador tipoTomador;
                Enum.TryParse<Dominio.Enumeradores.TipoTomador>(Request.Params["TomadorServico"], out tipoTomador);

                Dominio.Enumeradores.TipoServico tipoServico;
                Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["TipoServico"], out tipoServico);

                Servicos.Embarcador.Carga.ICMS svcICMS = new Servicos.Embarcador.Carga.ICMS(unidadeDeTrabalho);

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);

                Dominio.ObjetosDeValor.Cliente clienteJS = new Dominio.ObjetosDeValor.Cliente();
                clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["Remetente"]);
                Dominio.Entidades.Cliente remetente = null;
                if (clienteJS != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)))
                    remetente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)));

                clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["Destinatario"]);
                Dominio.Entidades.Cliente destinatario = null;
                if (clienteJS != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)))
                    destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)));
                bool destinatarioExportacao = clienteJS?.Exportacao ?? false;

                Dominio.Entidades.Cliente tomador = null;
                if (tipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    tomador = remetente;
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    tomador = destinatario;
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                {
                    clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["Expedidor"]);
                    if (clienteJS != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)))
                        tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)));
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                {
                    clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["Recebedor"]);
                    if (clienteJS != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)))
                        tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)));
                }
                else if (tipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                {
                    clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["Tomador"]);
                    if (clienteJS != null && !string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)))
                        tomador = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ)));
                }
                int.TryParse(Request.Params["MunicipioInicioPrestacao"], out int municipioInicioPrestacao);
                int.TryParse(Request.Params["MunicipioTerminoPrestacao"], out int municipioTerminoPrestacao);

                Dominio.Entidades.Localidade localidadeInicioPrestacao = repLocalidade.BuscarPorCodigo(municipioInicioPrestacao);
                Dominio.Entidades.Localidade localidadeFimPrestacao = repLocalidade.BuscarPorCodigo(municipioTerminoPrestacao);
                Dominio.Entidades.Localidade localidadeExportacao = repLocalidade.BuscarPorEstado("EX");

                //if (destinatarioExportacao && localidadeExportacao != null)
                //    localidadeFimPrestacao = localidadeExportacao;

                Dominio.Enumeradores.TipoICMS ICMS;
                Enum.TryParse<Dominio.Enumeradores.TipoICMS>(Request.Params["ICMS"], out ICMS);

                if (localidadeInicioPrestacao == null || localidadeFimPrestacao == null) //|| remetente == null || destinatario == null
                    return Json<bool>(false, false, "Início da prestação, término da prestação, remetente ou destinatário não foram informados.");

                bool incluirICMS = false;
                decimal percentualInclusaoICMS = 100;
                decimal valorBaseICMS = 0;
                //Alterado para enviar a informação do SimpesNacional conforme configuração da empresa, anteriormente enviava conforma CST do CTe ICMS == Dominio.Enumeradores.TipoICMS.Simples_Nacional ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao
                Dominio.ObjetosDeValor.Embarcador.ICMS.RegraICMS regraICMS = svcICMS.BuscarRegraICMSMultiCTe(this.EmpresaUsuario, this.EmpresaUsuario.OptanteSimplesNacional ? Dominio.Enumeradores.OpcaoSimNao.Sim : Dominio.Enumeradores.OpcaoSimNao.Nao, remetente, destinatario, tomador, localidadeInicioPrestacao, localidadeFimPrestacao, (destinatarioExportacao && localidadeExportacao != null), ref incluirICMS, ref percentualInclusaoICMS, valorBaseICMS, tipoServico, string.Empty, unidadeDeTrabalho);

                if (regraICMS == null)
                    return Json<bool>(false, false, "Nenhuma regra de icms encontrada.");

                Dominio.Entidades.CFOP cfop = null;
                if (regraICMS.CFOP > 0)
                    cfop = repCFOP.BuscarPorCFOP(regraICMS.CFOP, Dominio.Enumeradores.TipoCFOP.Saida);

                var retorno = new
                {
                    CodigoNatureza = cfop != null ? cfop.NaturezaDaOperacao.Codigo : 0,
                    CodigoCFOP = cfop != null ? cfop.Codigo : 0,
                    AliquotaICMS = regraICMS.ValorBaseCalculoICMS == 1 ? regraICMS.Aliquota : 0,
                    CSTICMS = !string.IsNullOrWhiteSpace(regraICMS.CST) ? this.ObterCSTDoICMS(regraICMS.CST) : 0,
                    ZerarBaseICMS = regraICMS.ValorBaseCalculoICMS == 0 ? true : false,
                    PercentualReducaoBC = regraICMS.PercentualReducaoBC
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao obter as informações de impostos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterValoresDoMes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeGraficosEmissoes)
                    return Json<bool>(false, false, "Grafico emissões sem configuração para exibição na pagina inicial.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.ObjetosDeValor.ValoresCTe valores = repCTe.BuscarValores(this.EmpresaUsuario.Codigo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), this.EmpresaUsuario.TipoAmbiente);
                return Json(valores, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(false, false, "Ocorreu uma falha ao obter os valores de CT-es do mês.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterQuantidadesDoMes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeGraficosEmissoes)
                    return Json<bool>(false, false, "Grafico emissões sem configuração para exibição na pagina inicial.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                List<Dominio.ObjetosDeValor.QuantidadesCTe> quantidades = repCTe.BuscarQuantidades(this.EmpresaUsuario.Codigo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), this.EmpresaUsuario.TipoAmbiente);
                return Json(quantidades, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json(false, false, "Ocorreu uma falha ao obter os valores de CT-es do mês.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterQuantidadeDeCTesPendentesDeEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                if ((this.UsuarioAdministrativo != null) || (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomePendenciasEntrega))
                    return Json(new { Total = 0 }, true);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                int ctesPendentes = repCTe.ContarCTesPendentesDeEntrega(this.EmpresaUsuario.Codigo, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), this.EmpresaUsuario.TipoAmbiente);

                return Json(new { Total = ctesPendentes }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter a quantidade de CT-es pendentes de entrega.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult DownloadPreCTe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoCTe);

                if (cte == null)
                    return Json<bool>(false, false, "CT-e não encontrado. Atualize a página e tente novamente.");

                Repositorio.MotoristaCTE repMotorista = new Repositorio.MotoristaCTE(unitOfWork);
                Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
                Repositorio.SeguroCTE repSeguro = new Repositorio.SeguroCTE(unitOfWork);
                Repositorio.InformacaoCargaCTE repQuantidade = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.DocumentosCTE repDocumento = new Repositorio.DocumentosCTE(unitOfWork);
                Repositorio.ImpostoIBPT repImpostoIBPT = new Repositorio.ImpostoIBPT(unitOfWork);
                Repositorio.VeiculoCTE repVeiculo = new Repositorio.VeiculoCTE(unitOfWork);

                List<Dominio.Entidades.MotoristaCTE> motoristas = repMotorista.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.ComponentePrestacaoCTE> componentes = repComponentePrestacao.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.SeguroCTE> seguros = repSeguro.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.InformacaoCargaCTE> quantidades = repQuantidade.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.DocumentosCTE> documentos = repDocumento.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.Entidades.VeiculoCTE> veiculos = repVeiculo.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                Dominio.Entidades.ImpostoIBPT imposto = repImpostoIBPT.BuscarPorEstado(cte.Empresa.Localidade.Estado.Sigla);

                Dominio.ObjetosDeValor.Relatorios.DACTE dados = new Dominio.ObjetosDeValor.Relatorios.DACTE()
                {
                    AliquotaICMS = cte.AliquotaICMS,
                    Ambiente = cte.TipoAmbiente,
                    BairroEmitente = cte.Empresa.Bairro,
                    BaseCalculoICMS = cte.BaseCalculoICMS,
                    CEPEmitente = cte.Empresa.CEP,
                    CidadeEmitente = cte.Empresa.Localidade.Descricao,
                    CNPJEmitente = cte.Empresa.CNPJ,
                    CidadeOrigemPrestacao = cte.LocalidadeInicioPrestacao.Descricao,
                    CidadeDestinoPrestacao = cte.LocalidadeTerminoPrestacao.Descricao,
                    Codigo = cte.Codigo,
                    CFOP = cte.CFOP != null ? cte.CFOP.CodigoCFOP.ToString() : string.Empty,
                    CPFMotorista = motoristas.Select(o => o.CPFMotorista).FirstOrDefault(),
                    CSTICMS = cte.CST,
                    DataEmissao = cte.DataEmissao,
                    DescricaoComponentePrestacao1 = componentes.Count > 0 ? componentes[0].Nome : string.Empty,
                    DescricaoComponentePrestacao2 = componentes.Count > 1 ? componentes[1].Nome : string.Empty,
                    DescricaoComponentePrestacao3 = componentes.Count > 2 ? componentes[2].Nome : string.Empty,
                    DescricaoComponentePrestacao4 = componentes.Count > 3 ? componentes[3].Nome : string.Empty,
                    DescricaoComponentePrestacao5 = componentes.Count > 4 ? componentes[4].Nome : string.Empty,
                    DescricaoComponentePrestacao6 = componentes.Count > 5 ? componentes[5].Nome : string.Empty,
                    DescricaoComponentePrestacao7 = componentes.Count > 6 ? componentes[6].Nome : string.Empty,
                    DescricaoComponentePrestacao8 = componentes.Count > 7 ? componentes[7].Nome : string.Empty,
                    DescricaoComponentePrestacao9 = componentes.Count > 8 ? componentes[8].Nome : string.Empty,
                    IEEmitente = cte.Empresa.InscricaoEstadual,
                    Logo = Servicos.Imagem.GetFromPath(cte.Empresa.CaminhoLogoDacte, System.Drawing.Imaging.ImageFormat.Bmp),
                    LogradouroEmitente = cte.Empresa.Endereco,
                    Lotacao = cte.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim ? "SIM" : "NÃO",
                    NomeEmitente = cte.Empresa.RazaoSocial,
                    NomeMotorista = motoristas.Select(o => o.NomeMotorista).FirstOrDefault(),
                    Numero = cte.Numero,
                    NumeroApoliceSeguro = seguros.Select(o => o.NumeroApolice).FirstOrDefault(),
                    NumeroAverbacaoSeguro = seguros.Select(o => o.NumeroAverbacao).FirstOrDefault(),
                    NumeroEmitente = cte.Empresa.Numero,
                    NomeSeguradora = seguros.Select(o => o.NomeSeguradora).FirstOrDefault(),
                    OutrasCaracteristicasCarga = cte.OutrasCaracteristicasDaCargaCTe,
                    Observacoes = cte.ObservacoesGerais,
                    PercentualReducaoBaseCalculoICMS = cte.PercentualReducaoBaseCalculoICMS,
                    PercentualTributosEstadual = imposto.PercentualEstadual,
                    PercentualTributosInternacional = imposto.PercentualFederalInternacional,
                    PercentualTributosMunicipal = imposto.PercentualMunicipal,
                    PercentualTributosNacional = imposto.PercentualFederalNacional,
                    ProdutoPredominante = cte.ProdutoPredominanteCTe,
                    QuantidadeCarga1 = quantidades.Count > 0 ? quantidades[0].Quantidade : 0m,
                    QuantidadeCarga2 = quantidades.Count > 1 ? quantidades[1].Quantidade : 0m,
                    QuantidadeCarga3 = quantidades.Count > 2 ? quantidades[2].Quantidade : 0m,
                    QuantidadeCarga4 = quantidades.Count > 3 ? quantidades[3].Quantidade : 0m,
                    ResponsavelSeguro = seguros.Select(o => o.Tipo).FirstOrDefault(),
                    RNTRCEmitente = cte.Empresa.RegistroANTT,
                    Serie = cte.Serie.Numero,
                    Status = cte.Status,
                    SuprimirImpostos = cte.ExibeICMSNaDACTE,
                    TelefoneEmitente = cte.Empresa.Telefone,
                    TipoCTe = cte.TipoCTE,
                    TipoImpressao = cte.TipoImpressao,
                    TipoPagamento = cte.TipoPagamento,
                    TipoServico = cte.TipoServico,
                    TipoTomador = cte.TipoTomador,
                    TotalDocumentos = documentos.Count,
                    UFEmitente = cte.Empresa.Localidade.Estado.Sigla,
                    UnidadeMedida1 = quantidades.Count > 0 ? quantidades[0].DescricaoUnidadeMedida : string.Empty,
                    UnidadeMedida2 = quantidades.Count > 1 ? quantidades[1].DescricaoUnidadeMedida : string.Empty,
                    UnidadeMedida3 = quantidades.Count > 2 ? quantidades[2].DescricaoUnidadeMedida : string.Empty,
                    UnidadeMedida4 = quantidades.Count > 3 ? quantidades[3].DescricaoUnidadeMedida : string.Empty,
                    UFDestinoPrestacao = cte.LocalidadeTerminoPrestacao.Estado.Sigla,
                    UFOrigemPrestacao = cte.LocalidadeInicioPrestacao.Estado.Sigla,
                    ValorComponentePrestacao1 = componentes.Count > 0 ? componentes[0].Valor : 0m,
                    ValorComponentePrestacao2 = componentes.Count > 1 ? componentes[1].Valor : 0m,
                    ValorComponentePrestacao3 = componentes.Count > 2 ? componentes[2].Valor : 0m,
                    ValorComponentePrestacao4 = componentes.Count > 3 ? componentes[3].Valor : 0m,
                    ValorComponentePrestacao5 = componentes.Count > 4 ? componentes[4].Valor : 0m,
                    ValorComponentePrestacao6 = componentes.Count > 5 ? componentes[5].Valor : 0m,
                    ValorComponentePrestacao7 = componentes.Count > 6 ? componentes[6].Valor : 0m,
                    ValorComponentePrestacao8 = componentes.Count > 7 ? componentes[7].Valor : 0m,
                    ValorComponentePrestacao9 = componentes.Count > 8 ? componentes[8].Valor : 0m,
                    ValorICMS = cte.CST != null && cte.CST != "60" ? cte.ValorICMS : 0m,
                    ValorICMSST = cte.CST != null && cte.CST == "60" ? cte.ValorICMS : 0m,
                    ValorTotalMercadoria = cte.ValorTotalMercadoria,
                    ValorTotalReceber = cte.ValorAReceber,
                    ValorTotalServico = cte.ValorPrestacaoServico,
                };

                dados.MarcaDagua = Servicos.Imagem.DrawText("DOCUMENTO SEM VALOR FISCAL", new Font("Arial", 20f, FontStyle.Bold), Color.LightGray, Color.White, 45);

                if (cte.Destinatario != null)
                {
                    dados.CEPDestinatario = cte.Destinatario.CEP;
                    dados.CPFCNPJDestinatario = cte.Destinatario.CPF_CNPJ;
                    dados.IEDestinatario = cte.Destinatario.IE_RG;
                    dados.LogradouroDestinatario = cte.Destinatario.Endereco;
                    dados.NomeDestinatario = cte.Destinatario.Nome;
                    dados.NumeroDestinatario = cte.Destinatario.Numero;
                    dados.TelefoneDestinatario = cte.Destinatario.Telefone1;

                    if (cte.Destinatario.Exterior)
                    {
                        dados.CidadeDestinatario = cte.Destinatario.Cidade;
                        dados.UFDestinatario = "EX";
                        dados.PaisDestinatario = cte.Destinatario.Pais != null ? cte.Destinatario.Pais.Nome : "EXPORTACAO";
                    }
                    else
                    {
                        dados.CidadeDestinatario = cte.Destinatario.Localidade.Descricao;
                        dados.UFDestinatario = cte.Destinatario.Localidade.Estado.Sigla;
                        dados.PaisDestinatario = cte.Destinatario.Localidade.Estado.Pais.Nome;
                    }
                }

                if (cte.Expedidor != null)
                {
                    dados.CEPExpedidor = cte.Expedidor.CEP;
                    dados.CPFCNPJExpedidor = cte.Expedidor.CPF_CNPJ;
                    dados.IEExpedidor = cte.Expedidor.IE_RG;
                    dados.LogradouroExpedidor = cte.Expedidor.Endereco;
                    dados.NomeExpedidor = cte.Expedidor.Nome;
                    dados.NumeroExpedidor = cte.Expedidor.Numero;
                    dados.TelefoneExpedidor = cte.Expedidor.Telefone1;

                    if (cte.Expedidor.Exterior)
                    {
                        dados.CidadeExpedidor = cte.Expedidor.Cidade;
                        dados.UFExpedidor = "EX";
                        dados.PaisExpedidor = cte.Expedidor.Pais != null ? cte.Expedidor.Pais.Nome : "EXPORTACAO";
                    }
                    else
                    {
                        dados.CidadeExpedidor = cte.Expedidor.Localidade.Descricao;
                        dados.UFExpedidor = cte.Expedidor.Localidade.Estado.Sigla;
                        dados.PaisExpedidor = cte.Expedidor.Localidade.Estado.Pais.Nome;
                    }
                }

                if (cte.Recebedor != null)
                {
                    dados.CEPRecebedor = cte.Recebedor.CEP;
                    dados.CPFCNPJRecebedor = cte.Recebedor.CPF_CNPJ;
                    dados.IERecebedor = cte.Recebedor.IE_RG;
                    dados.LogradouroRecebedor = cte.Recebedor.Endereco;
                    dados.NomeRecebedor = cte.Recebedor.Nome;
                    dados.NumeroRecebedor = cte.Recebedor.Numero;
                    dados.TelefoneRecebedor = cte.Recebedor.Telefone1;

                    if (cte.Recebedor.Exterior)
                    {
                        dados.CidadeRecebedor = cte.Recebedor.Cidade;
                        dados.UFRecebedor = "EX";
                        dados.PaisRecebedor = cte.Recebedor.Pais != null ? cte.Recebedor.Pais.Nome : "EXPORTACAO";
                    }
                    else
                    {
                        dados.CidadeRecebedor = cte.Recebedor.Localidade.Descricao;
                        dados.UFRecebedor = cte.Recebedor.Localidade.Estado.Sigla;
                        dados.PaisRecebedor = cte.Recebedor.Localidade.Estado.Pais.Nome;
                    }
                }

                if (cte.Remetente != null)
                {
                    dados.CEPRemetente = cte.Remetente.CEP;
                    dados.CPFCNPJRemetente = cte.Remetente.CPF_CNPJ;
                    dados.IERemetente = cte.Remetente.IE_RG;
                    dados.LogradouroRemetente = cte.Remetente.Endereco;
                    dados.NomeRemetente = cte.Remetente.Nome;
                    dados.NumeroRemetente = cte.Remetente.Numero;
                    dados.TelefoneRemetente = cte.Remetente.Telefone1;

                    if (cte.Remetente.Exterior)
                    {
                        dados.CidadeRemetente = cte.Remetente.Cidade;
                        dados.UFRemetente = "EX";
                        dados.PaisRemetente = cte.Remetente.Pais != null ? cte.Remetente.Pais.Nome : "EXPORTACAO";
                    }
                    else
                    {
                        dados.CidadeRemetente = cte.Remetente.Localidade.Descricao;
                        dados.UFRemetente = cte.Remetente.Localidade.Estado.Sigla;
                        dados.PaisRemetente = cte.Remetente.Localidade.Estado.Pais.Nome;
                    }
                }

                if (cte.Tomador != null)
                {
                    dados.CEPTomador = cte.Tomador.CEP;
                    dados.CPFCNPJTomador = cte.Tomador.CPF_CNPJ;
                    dados.IETomador = cte.Tomador.IE_RG;
                    dados.LogradouroTomador = cte.Tomador.Endereco;
                    dados.NomeTomador = cte.Tomador.Nome;
                    dados.NumeroTomador = cte.Tomador.Numero;
                    dados.TelefoneTomador = cte.Tomador.Telefone1;

                    if (cte.Tomador.Exterior)
                    {
                        dados.CidadeTomador = cte.Tomador.Cidade;
                        dados.UFTomador = "EX";
                        dados.PaisTomador = cte.Tomador.Pais != null ? cte.Tomador.Pais.Nome : "EXPORTACAO";
                    }
                    else
                    {
                        dados.CidadeTomador = cte.Tomador.Localidade.Descricao;
                        dados.UFTomador = cte.Tomador.Localidade.Estado.Sigla;
                        dados.PaisTomador = cte.Tomador.Localidade.Estado.Pais.Nome;
                    }
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>();
                dataSources.Add(new ReportDataSource("DACTE", new List<Dominio.ObjetosDeValor.Relatorios.DACTE>() { dados }));
                dataSources.Add(new ReportDataSource("Veiculos", veiculos));
                dataSources.Add(new ReportDataSource("DocumentosFiscais1", documentos.Take(12)));
                dataSources.Add(new ReportDataSource("DocumentosFiscais2", documentos.Skip(12).Take(12)));

                Servicos.Relatorio svcRelatorio = new Servicos.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Relatorios.Relatorio arquivo = svcRelatorio.GerarWeb("Relatorios/PreCTe.rdlc", "PDF", null, dataSources);

                return Arquivo(arquivo.Arquivo, arquivo.MimeType, "PreCTe_" + cte.Numero.ToString() + "." + arquivo.FileNameExtension.ToLower());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o pré CT-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ConsultarNFeSefaz()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.BloquearConsultaNFeSefaz)
                    return Json<bool>(false, false, "Sefaz não está disponível para consultar NFe, favor fazer importação via XML.");

                Repositorio.WebServicesConsultaNFe repWebServicesConsultaNFe = new Repositorio.WebServicesConsultaNFe(unitOfWork);
                List<Dominio.Entidades.WebServicesConsultaNFe> listaWS = repWebServicesConsultaNFe.BuscarNaoBloqueadas(15);
                Dominio.Entidades.WebServicesConsultaNFe webServicesConsultaNFe = null;
                if (listaWS != null && listaWS.Count > 0)
                    webServicesConsultaNFe = listaWS.FirstOrDefault();
                else if (listaWS.Count() == 0)
                {
                    listaWS = repWebServicesConsultaNFe.BuscarPorNumeroDeConsultas(15);
                    if (listaWS.Count > 0)
                    {
                        foreach (Dominio.Entidades.WebServicesConsultaNFe ws in listaWS)
                        {
                            ws.Consultas = 0;
                            repWebServicesConsultaNFe.Atualizar(ws);
                        }
                    }

                    listaWS = repWebServicesConsultaNFe.BuscarBloqueadasPorData(DateTime.Now.AddHours(-24));
                    if (listaWS.Count > 0)
                    {
                        foreach (Dominio.Entidades.WebServicesConsultaNFe ws in listaWS)
                        {
                            ws.DataBloqueio = null;
                            repWebServicesConsultaNFe.Atualizar(ws);
                        }
                    }

                    listaWS = repWebServicesConsultaNFe.BuscarNaoBloqueadas(15);
                    if (listaWS != null && listaWS.Count > 0)
                        webServicesConsultaNFe = listaWS.FirstOrDefault();
                }

                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                string chave = Request.Params["ChaveNFe"].Replace(" ", "");
                if (serDocumento.ValidarChave(chave))
                {
                    ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();

                    if (webServicesConsultaNFe != null)
                        consultaNFe.Endpoint.Address = new EndpointAddress(webServicesConsultaNFe.WebService);

                    OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
                    MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                    OperationContext.Current.OutgoingMessageHeaders.Add(header);

                    ConsultaNFe.RetornoOfRequisicaoSefazgj5B5PAD requisicaoSefaz = consultaNFe.SolicitarRequisicaoSefaz();

                    if (requisicaoSefaz.Status)
                    {
                        Servicos.Log.TratarErro(this.EmpresaUsuario.CNPJ + ": " + chave, "NFeSefaz");

                        var retorno = new
                        {
                            NotaAdicionada = false, //todo: quando incluir o quebra captcha já executar toda a consulta aqui (ver regra quebra de captcha)
                            DadosConsultar = new
                            {
                                VIEWSTATE = requisicaoSefaz.Objeto.VIEWSTATE,
                                EVENTVALIDATION = requisicaoSefaz.Objeto.EVENTVALIDATION,
                                imgCaptcha = requisicaoSefaz.Objeto.Captcha,
                                token = requisicaoSefaz.Objeto.TokenCaptcha,
                                SessionID = requisicaoSefaz.Objeto.SessionID
                            }
                        };

                        if (webServicesConsultaNFe != null)
                        {
                            webServicesConsultaNFe.Consultas += 1;
                            repWebServicesConsultaNFe.Atualizar(webServicesConsultaNFe);
                        }

                        return Json(retorno, true);
                    }
                    else
                    {
                        Servicos.Log.TratarErro(this.EmpresaUsuario.CNPJ + ": " + chave + " - Retorno: " + requisicaoSefaz.Mensagem, "NFeSefaz");

                        if (webServicesConsultaNFe != null && requisicaoSefaz.Status == false)
                        {
                            webServicesConsultaNFe.DataBloqueio = DateTime.Now;
                            repWebServicesConsultaNFe.Atualizar(webServicesConsultaNFe);
                        }

                        return Json<bool>(false, false, requisicaoSefaz.Mensagem);
                    }
                }
                else
                {
                    return Json<bool>(false, false, "A chave informada é inválida, por favor, verifique e tente novamente.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "NFeSefaz");
                return Json<bool>(false, false, "Sefaz não está disponível para consulta de NFe, favor fazer importação via XML.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult InformarCaptchaNFeSefaz()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string caminhoDiretorioConsultasSefaz = System.Configuration.ConfigurationManager.AppSettings["DiretorioConsultasSefaz"];

                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                Repositorio.WebServicesConsultaNFe repWebServicesConsultaNFe = new Repositorio.WebServicesConsultaNFe(unitOfWork);

                Servicos.CTe svcCTe = new CTe(unitOfWork);

                List<Dominio.Entidades.WebServicesConsultaNFe> listaWS = repWebServicesConsultaNFe.BuscarNaoBloqueadas(15);
                Dominio.Entidades.WebServicesConsultaNFe webServicesConsultaNFe = null;
                if (listaWS != null && listaWS.Count > 0)
                    webServicesConsultaNFe = listaWS.FirstOrDefault();

                Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz = new Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz();
                requisicaoSefaz.VIEWSTATE = Request.Params["VIEWSTATE"];
                requisicaoSefaz.EVENTVALIDATION = Request.Params["EVENTVALIDATION"];
                requisicaoSefaz.TokenCaptcha = Request.Params["token"];
                requisicaoSefaz.SessionID = Request.Params["SessionID"];
                string chave = Request.Params["ChaveNFe"].Replace(" ", "");
                string captcha = Request.Params["Captcha"];

                ConsultaNFe.ConsultaNFeClient consultaNFe = new ConsultaNFe.ConsultaNFeClient();

                if (webServicesConsultaNFe != null)
                    consultaNFe.Endpoint.Address = new EndpointAddress(webServicesConsultaNFe.WebService);

                OperationContextScope scope = new OperationContextScope(consultaNFe.InnerChannel);
                MessageHeader header = MessageHeader.CreateHeader("Token", "Token", "4ed60154d2f04201ab8b57ed4198da32");
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                ConsultaNFe.RetornoOfConsultaSefazgj5B5PAD consultaSefaz = consultaNFe.ConsultarSefaz(requisicaoSefaz, chave, captcha);

                if (consultaSefaz.Status)
                {
                    if (consultaSefaz.Objeto.ConsultaValida)
                    {
                        if (!string.IsNullOrWhiteSpace(caminhoDiretorioConsultasSefaz))
                        {
                            string arquivoConsultaSefaz = Newtonsoft.Json.JsonConvert.SerializeObject(consultaSefaz);
                            if (!string.IsNullOrWhiteSpace(arquivoConsultaSefaz))
                                svcCTe.SalvarArquivoEmTxt(caminhoDiretorioConsultasSefaz, consultaSefaz.Objeto.NotaFiscal.Chave, arquivoConsultaSefaz);
                        }

                        bool naoCopiarTomadorCTeAnterior = this.UsuarioAdministrativo == null ? false : this.UsuarioAdministrativo.Empresa.Configuracao != null ? this.UsuarioAdministrativo.Empresa.Configuracao.NaoCarregarTomadorCTes : false;

                        string chaveNFe = !string.IsNullOrWhiteSpace(consultaSefaz.Objeto.NotaFiscal.Chave) ? Utilidades.String.OnlyNumbers(consultaSefaz.Objeto.NotaFiscal.Chave) : string.Empty;

                        var retorno = new
                        {
                            Chave = chaveNFe.Length > 44 ? chaveNFe.Substring(0, 44) : chaveNFe,
                            ValorTotal = consultaSefaz.Objeto.NotaFiscal.Valor,
                            DataEmissao = consultaSefaz.Objeto.NotaFiscal.DataEmissao.Substring(0, 10),

                            Remetente = consultaSefaz.Objeto.NotaFiscal.Emitente.CPFCNPJ,
                            RemetenteIE = consultaSefaz.Objeto.NotaFiscal.Emitente.RGIE,
                            RemetenteNome = consultaSefaz.Objeto.NotaFiscal.Emitente.RazaoSocial,
                            RemetenteLogradouro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Logradouro,
                            RemetenteCEP = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.CEP,
                            RemetenteBairro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Bairro,
                            RemetenteNumero = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Numero,
                            RemetenteCidade = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.IBGE,
                            RemetenteUF = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.SiglaUF,

                            Destinatario = consultaSefaz.Objeto.NotaFiscal.Destinatario.CPFCNPJ,
                            DestinatarioIE = consultaSefaz.Objeto.NotaFiscal.Destinatario.RGIE,
                            DestinatarioNome = consultaSefaz.Objeto.NotaFiscal.Destinatario.RazaoSocial,
                            DestinatarioLogradouro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Logradouro,
                            DestinatarioCEP = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.CEP,
                            DestinatarioBairro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Bairro,
                            DestinatarioNumero = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Numero,
                            DestinatarioCidade = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE,
                            DestinatarioNomeCidadeUF = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.Descricao + " / " + consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF,
                            DestinatarioExportacao = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE == 9999999 ? this.ObterDadosDestinatarioExportacao(consultaSefaz.Objeto.NotaFiscal.Destinatario, unitOfWork) : null,
                            DestinatarioUF = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF,

                            Numero = consultaSefaz.Objeto.NotaFiscal.Numero,
                            Peso = consultaSefaz.Objeto.NotaFiscal.PesoBruto,
                            PesoLiquido = consultaSefaz.Objeto.NotaFiscal.PesoLiquido,
                            UtilizarPesoLiquido = this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Liquido ? "1" : "0",
                            FormaPagamento = naoCopiarTomadorCTeAnterior ? null : consultaSefaz.Objeto.NotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? "1" : "0",
                            Placa = consultaSefaz.Objeto.NotaFiscal.Veiculo != null ? consultaSefaz.Objeto.NotaFiscal.Veiculo.Placa : string.Empty,
                            NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, consultaSefaz.Objeto.NotaFiscal.Chave), //repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, consultaSefaz.Objeto.NotaFiscal.Chave),
                            Serie = svcCTe.ObterCodigoSerie(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.Localidade.Estado.Sigla, consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.SiglaUF, consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF, unitOfWork),
                            Observacao = consultaSefaz.Objeto.NotaFiscal.InformacoesComplementares,
                            Volume = consultaSefaz.Objeto.NotaFiscal.VolumesTotal,
                            Excluir = false
                        };

                        Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                        Dominio.Entidades.Cliente remetente = ObterCliente(retorno.Remetente, retorno.RemetenteIE, retorno.RemetenteNome, retorno.RemetenteLogradouro, retorno.RemetenteCEP, retorno.RemetenteBairro, retorno.RemetenteNumero, retorno.RemetenteCidade.ToString(), unitOfWork);
                        Dominio.Entidades.Cliente destinatario = ObterCliente(retorno.Destinatario, retorno.DestinatarioIE, retorno.DestinatarioNome, retorno.DestinatarioLogradouro, retorno.DestinatarioCEP, retorno.DestinatarioBairro, retorno.DestinatarioNumero, retorno.DestinatarioCidade.ToString(), unitOfWork);

                        return Json(retorno, true);
                    }
                    else
                    {
                        return Json<bool>(false, false, consultaSefaz.Objeto.MensagemSefaz);
                    }
                }
                else
                {
                    return Json<bool>(false, false, consultaSefaz.Mensagem);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao carregar NF-e Sefaz.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("GET", "POST")]
        public ActionResult ConsultarNFeSefazSalva()
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string caminhoDiretorioConsultasSefaz = System.Configuration.ConfigurationManager.AppSettings["DiretorioConsultasSefaz"];

                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
                Servicos.CTe svcCTe = new CTe(unitOfWork);

                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                string chave = Request.Params["ChaveNFe"].Replace(" ", "");
                if (!string.IsNullOrWhiteSpace(caminhoDiretorioConsultasSefaz) && serDocumento.ValidarChave(chave))
                {
                    string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoDiretorioConsultasSefaz, chave + ".txt");
                    if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                    {
                        string mensagem = string.Empty;
                        List<string> mensagemLinha = new List<string>();

                        using (StreamReader texto = new StreamReader(Utilidades.IO.FileStorageService.Storage.OpenRead(arquivo)))
                        {
                            while ((mensagem = texto.ReadLine()) != null)
                            {
                                mensagemLinha.Add(mensagem);
                            }
                        }

                        bool naoCopiarTomadorCTeAnterior = this.UsuarioAdministrativo == null ? false : this.UsuarioAdministrativo.Empresa.Configuracao != null ? this.UsuarioAdministrativo.Empresa.Configuracao.NaoCarregarTomadorCTes : false;

                        dynamic consultaSefaz = Newtonsoft.Json.JsonConvert.DeserializeObject(mensagemLinha[0]);

                        var retorno = new
                        {
                            Chave = consultaSefaz.Objeto.NotaFiscal.Chave,
                            ValorTotal = consultaSefaz.Objeto.NotaFiscal.Valor,
                            DataEmissao = consultaSefaz.Objeto.NotaFiscal.DataEmissao.ToString().Substring(0, 10),

                            Remetente = consultaSefaz.Objeto.NotaFiscal.Emitente.CPFCNPJ,
                            RemetenteIE = consultaSefaz.Objeto.NotaFiscal.Emitente.RGIE,
                            RemetenteNome = consultaSefaz.Objeto.NotaFiscal.Emitente.RazaoSocial,
                            RemetenteLogradouro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Logradouro,
                            RemetenteCEP = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.CEP,
                            RemetenteBairro = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Bairro,
                            RemetenteNumero = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Numero,
                            RemetenteCidade = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.IBGE,
                            RemetenteUF = consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.SiglaUF,

                            Destinatario = consultaSefaz.Objeto.NotaFiscal.Destinatario.CPFCNPJ,
                            DestinatarioIE = consultaSefaz.Objeto.NotaFiscal.Destinatario.RGIE,
                            DestinatarioNome = consultaSefaz.Objeto.NotaFiscal.Destinatario.RazaoSocial,
                            DestinatarioLogradouro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Logradouro,
                            DestinatarioCEP = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.CEP,
                            DestinatarioBairro = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Bairro,
                            DestinatarioNumero = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Numero,
                            DestinatarioCidade = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE,
                            DestinatarioNomeCidadeUF = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.Descricao + " / " + consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF,
                            DestinatarioUF = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF,
                            DestinatarioExportacao = consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.IBGE == 9999999 ? this.ObterDadosDestinatarioExportacao(consultaSefaz.Objeto.NotaFiscal.Destinatario, unitOfWork) : null,

                            Numero = consultaSefaz.Objeto.NotaFiscal.Numero,
                            Peso = consultaSefaz.Objeto.NotaFiscal.PesoBruto,
                            PesoLiquido = consultaSefaz.Objeto.NotaFiscal.PesoLiquido,
                            UtilizarPesoLiquido = this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Liquido ? "1" : "0",
                            FormaPagamento = naoCopiarTomadorCTeAnterior ? null : consultaSefaz.Objeto.NotaFiscal.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? "1" : "0",
                            Placa = consultaSefaz.Objeto.NotaFiscal.Veiculo != null ? consultaSefaz.Objeto.NotaFiscal.Veiculo.Placa : string.Empty,
                            NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, consultaSefaz.Objeto.NotaFiscal.Chave.ToString()), //repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, consultaSefaz.Objeto.NotaFiscal.Chave.ToString()),
                            Serie = svcCTe.ObterCodigoSerie(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.Localidade.Estado.Sigla.ToString(), consultaSefaz.Objeto.NotaFiscal.Emitente.Endereco.Cidade.SiglaUF.ToString(), consultaSefaz.Objeto.NotaFiscal.Destinatario.Endereco.Cidade.SiglaUF.ToString(), unitOfWork),
                            Observacao = consultaSefaz.Objeto.NotaFiscal.InformacoesComplementares,
                            Volume = consultaSefaz.Objeto.NotaFiscal.VolumesTotal,
                            Excluir = false
                        };

                        return Json(retorno, true);
                    }
                    else
                        return Json<bool>(false, false, "Arquivo Consulta NFe não salvo.");
                }
                else
                    return Json<bool>(false, false, "A chave informada é inválida, por favor, verifique e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao carregar consulta NF-e salva.");
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ConsultarPendentesCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Pagina repPagina = new Repositorio.Pagina(unitOfWork);
                Repositorio.PaginaUsuario repPaginaUsuario = new Repositorio.PaginaUsuario(unitOfWork);

                Dominio.Entidades.Pagina pagina = repPagina.BuscarPorFormulario("empresas.aspx");
                Dominio.Entidades.PaginaUsuario paginaUsuario = null;
                if (pagina != null)
                    paginaUsuario = repPaginaUsuario.BuscarPorPaginaEUsuario(pagina.Codigo, this.Usuario.Codigo);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
                int countCTes = 0;

                if (paginaUsuario != null && paginaUsuario.PermissaoDeAcesso == "A")
                {
                    listaCTes = repCTe.ConsultarPendentesCancelamento(this.EmpresaUsuario.Codigo, inicioRegistros, 50);
                    countCTes = repCTe.ContarConsultarPendentesCancelamento(this.EmpresaUsuario.Codigo);
                }

                dynamic[] retorno = (from obj in listaCTes
                              select new
                              {
                                  CodigoCriptografado = HttpUtility.UrlEncode(Servicos.Criptografia.Criptografar(obj.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3")),
                                  CodigoEmpresa = obj.Empresa.Codigo,
                                  obj.Empresa.CNPJ,
                                  obj.Empresa.RazaoSocial,
                                  CTe = obj.Numero + " / " + obj.Serie.Numero,
                                  Data = obj.DataEmissao != null ? obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                  DataSefaz = obj.DataRetornoSefaz != null ? obj.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty,
                                  RetornoSefaz = obj.MensagemRetornoSefaz,
                                  Log = obj.Log != null ? obj.Log : string.Empty
                              }).ToArray();

                return Json(retorno, true, null, new string[] { "Codigo", "Empresa", "CNPJ|10", "Razão Social|20", "CTe / Série|10", "Data Emissão|10", "Data Sefaz|10", "Retorno Sefaz|10", "Log|20" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados das empresas!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult ImportarNFeCSV()
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
                        object listaNFe = null;
                        listaNFe = this.CarregarDadosCSV(file.InputStream, unitOfWork);

                        return Json(listaNFe, true);
                        // return Json<bool>(true, true);
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
        public ActionResult ImportarHTMLPortalNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para geração de CT-e negada!");

                Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);

                Servicos.CTe svcCTe = new CTe(unitOfWork);
                Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(unitOfWork);

                bool naoCopiarTomadorCTeAnterior = this.UsuarioAdministrativo == null ? false : this.UsuarioAdministrativo.Empresa.Configuracao != null ? this.UsuarioAdministrativo.Empresa.Configuracao.NaoCarregarTomadorCTes : false;

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".html")
                    {
                        Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal notaFiscalHtml = serConsultaReceita.ProcessarHTMLRetorno(file.InputStream, "tbody/"); //responseReceita.GetResponseStream()

                        if (notaFiscalHtml != null)
                        {
                            var retorno = new
                            {
                                Chave = notaFiscalHtml.Chave.Length > 44 ? notaFiscalHtml.Chave.Substring(0, 44) : notaFiscalHtml.Chave,
                                ValorTotal = notaFiscalHtml.Valor,
                                DataEmissao = notaFiscalHtml.DataEmissao.Substring(0, 10),

                                Remetente = notaFiscalHtml.Emitente.CPFCNPJ,
                                RemetenteIE = notaFiscalHtml.Emitente.RGIE,
                                RemetenteNome = notaFiscalHtml.Emitente.RazaoSocial,
                                RemetenteLogradouro = notaFiscalHtml.Emitente.Endereco.Logradouro,
                                RemetenteCEP = notaFiscalHtml.Emitente.Endereco.CEP,
                                RemetenteBairro = notaFiscalHtml.Emitente.Endereco.Bairro,
                                RemetenteNumero = notaFiscalHtml.Emitente.Endereco.Numero,
                                RemetenteCidade = notaFiscalHtml.Emitente.Endereco.Cidade.IBGE,
                                RemetenteUF = notaFiscalHtml.Emitente.Endereco.Cidade.SiglaUF,

                                Destinatario = notaFiscalHtml.Destinatario.CPFCNPJ,
                                DestinatarioIE = notaFiscalHtml.Destinatario.RGIE,
                                DestinatarioNome = notaFiscalHtml.Destinatario.RazaoSocial,
                                DestinatarioLogradouro = notaFiscalHtml.Destinatario.Endereco.Logradouro,
                                DestinatarioCEP = notaFiscalHtml.Destinatario.Endereco.CEP,
                                DestinatarioBairro = notaFiscalHtml.Destinatario.Endereco.Bairro,
                                DestinatarioNumero = notaFiscalHtml.Destinatario.Endereco.Numero,
                                DestinatarioCidade = notaFiscalHtml.Destinatario.Endereco.Cidade.IBGE,
                                DestinatarioNomeCidadeUF = notaFiscalHtml.Destinatario.Endereco.Cidade.Descricao + " / " + notaFiscalHtml.Destinatario.Endereco.Cidade.SiglaUF,
                                DestinatarioExportacao = notaFiscalHtml.Destinatario.Endereco.Cidade.IBGE == 9999999 ? this.ObterDadosDestinatarioExportacao(notaFiscalHtml.Destinatario, unitOfWork) : null,
                                DestinatarioUF = notaFiscalHtml.Destinatario.Endereco.Cidade.SiglaUF,

                                Numero = notaFiscalHtml.Numero,
                                Peso = notaFiscalHtml.PesoBruto,
                                PesoLiquido = notaFiscalHtml.PesoLiquido,
                                UtilizarPesoLiquido = this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Liquido ? "1" : "0",
                                FormaPagamento = naoCopiarTomadorCTeAnterior ? null : notaFiscalHtml.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.A_Pagar ? "1" : "0",
                                Placa = notaFiscalHtml.Veiculo != null ? notaFiscalHtml.Veiculo.Placa : string.Empty,
                                NumeroDosCTesUtilizados = repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(this.EmpresaUsuario.Codigo, notaFiscalHtml.Chave),
                                Serie = svcCTe.ObterCodigoSerie(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.Localidade.Estado.Sigla, notaFiscalHtml.Emitente.Endereco.Cidade.SiglaUF, notaFiscalHtml.Destinatario.Endereco.Cidade.SiglaUF, unitOfWork),
                                Observacao = notaFiscalHtml.InformacoesComplementares,
                                Volume = notaFiscalHtml.VolumesTotal,
                                Excluir = false
                            };

                            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                            Dominio.Entidades.Cliente remetente = ObterCliente(retorno.Remetente, retorno.RemetenteIE, retorno.RemetenteNome, retorno.RemetenteLogradouro, retorno.RemetenteCEP, retorno.RemetenteBairro, retorno.RemetenteNumero, retorno.RemetenteCidade.ToString(), unitOfWork);
                            Dominio.Entidades.Cliente destinatario = ObterCliente(retorno.Destinatario, retorno.DestinatarioIE, retorno.DestinatarioNome, retorno.DestinatarioLogradouro, retorno.DestinatarioCEP, retorno.DestinatarioBairro, retorno.DestinatarioNumero, retorno.DestinatarioCidade.ToString(), unitOfWork);

                            return Json(retorno, true);
                        }
                        else
                            return Json<bool>(false, false, "Nenhuma Nota importada.");
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

        [AcceptVerbs("POST")]
        public ActionResult ObterJustificativaCancelamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico CTE = repCTe.BuscarPorId(codigoCTe, this.EmpresaUsuario.Codigo);
                    if (CTE != null)
                    {
                        var retorno = new
                        {
                            Justificativa = CTE.ObservacaoCancelamento
                        };

                        return Json(retorno, true, null);
                    }
                    else
                    {
                        return Json<bool>(false, false, "CT-e não encontrado.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Código de CT-e inválido.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados do CT-e. Atualize a página e tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult RegerarDACTE()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoCTe = 0;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                if (cte != null)
                {
                    if (cte.Status.Equals("A"))
                    {
                        Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
                        string retorno = svcCTe.RegerarDACTE(cte.Codigo, this.EmpresaUsuario.Codigo, unitOfWork);

                        if (!string.IsNullOrWhiteSpace(retorno) && retorno.Contains("Protocolo interno informado não encontrado"))
                        {
                            svcCTe.IntegrareCTeOracle(cte.Empresa, codigoCTe, unitOfWork);
                            retorno = svcCTe.RegerarDACTE(cte.Codigo, this.EmpresaUsuario.Codigo, unitOfWork);

                            if (string.IsNullOrWhiteSpace(retorno))
                                return Json<bool>(true, true);
                            else
                                return Json<bool>(false, false, "Não foi possível enviar solicitação: " + retorno);
                        }
                        else if (string.IsNullOrWhiteSpace(retorno))
                            return Json<bool>(true, true);
                        else
                            return Json<bool>(false, false, "Não foi possível enviar solicitação: " + retorno);
                    }
                    else
                    {
                        return Json<bool>(false, false, "O status do CT-e é inválido para solicitação.");
                    }
                }
                else
                {
                    return Json<bool>(false, false, "CT-e não encontrado.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao solicitar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult IntegrareCTeOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);

                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params["DataInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params["DataFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                List<int> listaCTes = null;
                for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {
                    listaCTes = repCTe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, new string[] { "K", "A", "C" }, data, data);
                    Servicos.Log.TratarErro("Dia: " + data.ToString() + ": CTes: " + listaCTes.Count().ToString());
                    if (listaCTes.Count > 0)
                    {
                        for (var i = 0; i < listaCTes.Count(); i++)
                        {
                            svCTe.IntegrareCTeOracle(null, listaCTes[i], unitOfWork);

                            unitOfWork.FlushAndClear();
                        }
                    }
                    unitOfWork.FlushAndClear();
                }

                return Json<bool>(true, true, "CTe enviados com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTeOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {

                Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                var listaCTes = repCTe.BuscarCodigosPorStatus("E");

                for (var i = 0; i < listaCTes.Count; i++)
                {
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i]);
                    if ((cte.ModeloDocumentoFiscal.Numero == "67" || cte.ModeloDocumentoFiscal.Numero == "57"))
                    {
                        if (cte.Status == "E")
                        {
                            servicoCTe.Consultar(ref cte, unitOfWork);

                            if (cte.Status == "A")
                            {
                                bool averbaCTe = (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.AverbaAutomaticoATM == 1) || (cte.Empresa.Configuracao != null && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.AverbaAutomaticoATM == 1);

                                if (averbaCTe && cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cte.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao)
                                {
                                    Servicos.Averbacao svcAverbacao = new Servicos.Averbacao(unitOfWork);
                                    if (svcAverbacao.VerificaAverbacao(cte.Codigo, Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao, unitOfWork))
                                        FilaConsultaCTe.GetInstance().QueueItem(5, cte.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.Averbacao, Conexao.StringConexao);
                                }

                                servicoCTe.AtualizarIntegracaoRetornoCTe(cte, unitOfWork);

                                Servicos.LsTranslog svcLsTranslog = new Servicos.LsTranslog(unitOfWork);
                                svcLsTranslog.SalvarCTeParaIntegracao(cte.Codigo, cte.Empresa.Codigo, unitOfWork);

                                Servicos.CIOT svcCIOT = new Servicos.CIOT(unitOfWork);
                                svcCIOT.VincularCTeCIOTEFrete(cte.Codigo, unitOfWork);
                            }
                        }
                    }
                    unitOfWork.FlushAndClear();
                }



                return Json<bool>(true, true, "CTe enviados com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult IntegrareCTeCanceladoOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);

                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params["DataInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params["DataFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                //List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>(); // repCTe.BuscarPorStatusEPeriodo(codigooEmpresa, new string[] { "C" }, dataInicio, dataFim);
                List<int> listaCTes = null;
                for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {
                    //listaCTes = repCTe.BuscarPorStatusEPeriodo(codigooEmpresa, new string[] { "C" }, data, data);
                    listaCTes = repCTe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, new string[] { "C" }, data, data);
                    Servicos.Log.TratarErro("Dia: " + data.ToString() + ": Cancelados: " + listaCTes.Count().ToString());
                    if (listaCTes.Count > 0)
                    {
                        for (var i = 0; i < listaCTes.Count(); i++)
                        {
                            svCTe.IntegrareCTeCanceladoOracle(null, listaCTes[i], unitOfWork);

                            unitOfWork.FlushAndClear();
                        }
                    }
                    unitOfWork.FlushAndClear();
                }

                return Json<bool>(true, true, "CTe enviados com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar CTe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult ExtrairXMLParaDiretorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);

                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params["DataInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params["DataFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                List<int> listaCTes = null;
                for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {
                    listaCTes = repCTe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, new string[] { "A", "C", "I" }, data, data);
                    Servicos.Log.TratarErro("Dia: " + data.ToString() + ": CTes: " + listaCTes.Count().ToString(), "ExtrairXMLParaDiretorio");
                    if (listaCTes.Count > 0)
                    {
                        for (var i = 0; i < listaCTes.Count(); i++)
                        {
                            List<Dominio.Entidades.XMLCTe> listaXMLCTe = repXMLCTe.BuscarPorCodigoCTe(listaCTes[i]);

                            foreach (var xmlCTe in listaXMLCTe)
                            {
                                Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave, "ExtrairXMLParaDiretorio");
                                if (!xmlCTe.XMLArmazenadoEmArquivo)
                                {
                                    if (!string.IsNullOrWhiteSpace(xmlCTe.XML))
                                    {
                                        string arquivo = svCTe.CriarERetornarCaminhoXMLCTe(xmlCTe.CTe, xmlCTe.Tipo == Dominio.Enumeradores.TipoXMLCTe.Autorizacao ? "A" : xmlCTe.CTe.Status, unitOfWork);
                                        Utilidades.IO.FileStorageService.Storage.WriteAllText(arquivo, xmlCTe.XML);
                                        if (Utilidades.IO.FileStorageService.Storage.Exists(arquivo))
                                        {
                                            xmlCTe.XMLArmazenadoEmArquivo = true;
                                            xmlCTe.XML = "";
                                            repXMLCTe.Atualizar(xmlCTe);

                                            Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave + " XML salvo.", "ExtrairXMLParaDiretorio");
                                        }
                                        else
                                            Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave + " não foi possivel salvar XML.", "ExtrairXMLParaDiretorio");
                                    }
                                    else
                                        Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave + " sem XML.", "ExtrairXMLParaDiretorio");
                                }
                                else
                                    Servicos.Log.TratarErro("CTe " + xmlCTe.CTe.Chave + " ja salvo em arquivo.", "ExtrairXMLParaDiretorio");
                            }

                            unitOfWork.FlushAndClear();
                        }
                    }
                    unitOfWork.FlushAndClear();
                }


                return Json<bool>(true, true, "Processo finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao extrair XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        [AcceptVerbs("POST")]
        public ActionResult CancelarCTesPorCSV()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);


                Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                    if (fileExtension.ToLower() == ".csv")
                    {
                        List<Dominio.ObjetosDeValor.XMLNFe> lista = this.CarregarDadosCSVCTe(file.InputStream, unitOfWork);

                        if (lista != null)
                        {
                            for (var i = 0; i < lista.Count(); i++)
                            {
                                try
                                {
                                    Servicos.Log.TratarErro("Cancelando CTe: " + lista[i].Chave, "CancelarCTesPorCSV");

                                    var cte = repCTe.BuscarPorChave(lista[i].Chave);
                                    if (cte == null)
                                        Servicos.Log.TratarErro("CTe não localizado: " + lista[i].Chave, "CancelarCTesPorCSV");
                                    else
                                    {
                                        if (cte.Status == "A")
                                        {
                                            if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoCTe(cte.SistemaEmissor).CancelarCte(cte.Codigo, cte.Empresa.Codigo, "Cancelamento solicitado por Thais Costa", unitOfWork))
                                                Servicos.Log.TratarErro("Solicitado cancelamento do CTe: " + lista[i].Chave, "CancelarCTesPorCSV");
                                            else
                                                Servicos.Log.TratarErro("Não foi possível cancelar CTe: " + lista[i].Chave, "CancelarCTesPorCSV");
                                        }
                                        else
                                            Servicos.Log.TratarErro("Status (" + cte.DescricaoStatus + ") não permite cancelamento. " + lista[i].Chave, "CancelarCTesPorCSV");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Servicos.Log.TratarErro(lista[i].Chave + ": " + ex, "CancelarCTesPorCSV");
                                    return Json<bool>(false, false, "Ocorreu uma falha ao cancelar CTes.");
                                }

                                unitOfWork.FlushAndClear();
                            }
                        }
                    }
                    else
                        return Json<bool>(false, false, "Arquivo com formato invalido.");
                }
                else
                {
                    return Json<bool>(false, false, "Contagem de arquivos inválida.");
                }

                return Json<bool>(true, true, "Processo finalizado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar CTes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarXMLOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);

                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params["DataInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params["DataFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Servicos.CTe svCTe = new Servicos.CTe(unitOfWork);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                List<int> listaCTes = null;
                for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
                {
                    listaCTes = repCTe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, new string[] { "A", "C" }, data, data);
                    Servicos.Log.TratarErro("Dia: " + data.ToString() + ": CTes: " + listaCTes.Count().ToString(), "BuscarXMLOracle");
                    if (listaCTes.Count > 0)
                    {
                        for (var i = 0; i < listaCTes.Count(); i++)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(listaCTes[i]);
                            if (cte != null)
                            {
                                Servicos.Log.TratarErro("CTe chave " + cte.Chave, "BuscarXMLOracle");
                                Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(listaCTes[i], Dominio.Enumeradores.TipoXMLCTe.Autorizacao);
                                if (xmlCTe == null || string.IsNullOrEmpty(xmlCTe.XML))
                                    svCTe.ObterESalvarXMLAutorizacaoOracle(cte, false, null, unitOfWork);

                                if (cte.Status == "C")
                                {
                                    Dominio.Entidades.XMLCTe xmlCTeCanc = repXMLCTe.BuscarPorCTe(listaCTes[i], Dominio.Enumeradores.TipoXMLCTe.Cancelamento);
                                    if (xmlCTeCanc == null || string.IsNullOrEmpty(xmlCTeCanc.XML))
                                        svCTe.ObterESalvarXMLCancelamento(cte, 0, false, null, unitOfWork);
                                }
                            }
                            else
                                Servicos.Log.TratarErro("CTe codigo " + cte.Codigo + " não encontrado.", "BuscarXMLOracle");


                            unitOfWork.FlushAndClear();
                        }
                    }
                    unitOfWork.FlushAndClear();
                }


                return Json<bool>(true, true, "Processo finalizado com sucesso.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao extrair XMLs.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        #endregion

        #region Metodos Privados

        private static void GerarNOTFIS()
        {
            try
            {
                Servicos.LeituraEDI leituraEDI = new Servicos.LeituraEDI(null, LayoutEDI, ArquivoEDI, 0, 0, 0, 0, 0, 0, 0, 0, true, true, Encoding.UTF8);

                NOTFIS = leituraEDI.GerarNotasFis();

                Sucesso = true;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }

            ThreadExecutada = true;
        }

        private string LocalidadePrestacaoRelatorio(string descricaoObs, int codigoCTe, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unitOfWork);
            Dominio.Entidades.ObservacaoContribuinteCTE obs = repObservacaoContribuinteCTE.BuscarPorCTeEDescricao(this.EmpresaUsuario.Codigo, codigoCTe, descricaoObs);

            return obs?.Descricao ?? string.Empty;
        }

        private object CarregarDadosCSV(Stream stream, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var cellValue = "";
                string chaveNFe = "";
                int linha = 0;
                List<Dominio.ObjetosDeValor.XMLNFe> listaNFe = new List<Dominio.ObjetosDeValor.XMLNFe>();

                StreamReader streamReader = new StreamReader(stream);

                while ((cellValue = streamReader.ReadLine()) != null)
                {
                    string[] linhaSeparada = cellValue.Split(';');
                    //if (linha > 0)
                    //{
                    chaveNFe = linhaSeparada[0]; //3
                    if (chaveNFe != "")
                    {
                        Dominio.ObjetosDeValor.XMLNFe nfe = new Dominio.ObjetosDeValor.XMLNFe();
                        nfe.Chave = chaveNFe;
                        nfe.Adicionada = false;
                        listaNFe.Add(nfe);
                    }
                    //}
                    linha = linha + 1;
                }

                return listaNFe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return false;
            }
        }

        private List<Dominio.ObjetosDeValor.XMLNFe> CarregarDadosCSVCTe(Stream stream, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                var cellValue = "";
                string chaveNFe = "";
                int linha = 0;
                List<Dominio.ObjetosDeValor.XMLNFe> listaNFe = new List<Dominio.ObjetosDeValor.XMLNFe>();

                StreamReader streamReader = new StreamReader(stream);

                while ((cellValue = streamReader.ReadLine()) != null)
                {
                    string[] linhaSeparada = cellValue.Split(';');
                    //if (linha > 0)
                    //{
                    chaveNFe = linhaSeparada[0]; //3
                    if (chaveNFe != "")
                    {
                        Dominio.ObjetosDeValor.XMLNFe nfe = new Dominio.ObjetosDeValor.XMLNFe();
                        nfe.Chave = chaveNFe;
                        nfe.Adicionada = false;
                        listaNFe.Add(nfe);
                    }
                    //}
                    linha = linha + 1;
                }

                return listaNFe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return null;
            }
        }

        private void FinalizarColeta(int codigoColeta, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unidadeDeTrabalho);

            Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoColeta);

            if (pedido != null)
            {
                pedido.SituacaoPedido = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Finalizado;

                repPedido.Atualizar(pedido);
            }
        }

        private ActionResult GerarCTePorListaNFe(List<Dominio.ObjetosDeValor.NFeAdmin> documentos, decimal valorFrete, decimal valorTotalMercadoria, string observacao, DateTime dataEmissao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorEmpresaPai(this.EmpresaUsuario.Codigo, documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.transporta.Item : documentos[0].NFe3.NFe.infNFe.transp.transporta.Item);

            if (empresa == null)
                return Json<bool>(false, false, "Empresa emissora não encontrada.");

            if (empresa.Configuracao == null)
                return Json<bool>(false, false, "Configurações da empresa emissora não encontradas.");

            Repositorio.DocumentosCTE repDocumentos = new Repositorio.DocumentosCTE(unitOfWork);

            foreach (var documento in documentos)
            {
                string chaveNFe = "";

                if (documento.NFe2 != null)
                    chaveNFe = documento.NFe2.protNFe.infProt.chNFe;
                else
                    chaveNFe = documento.NFe3.protNFe.infProt.chNFe;

                List<int> numerosCTEsUtilizados = repDocumentos.BuscarNumeroDosCTes(empresa.Codigo, chaveNFe, new string[] { "A", "E", "P" });

                if (numerosCTEsUtilizados.Count() > 0)
                    return Json<bool>(false, false, "A NF-e " + chaveNFe + " já foi utilizada no CT-e número " + numerosCTEsUtilizados[0].ToString());
            }

            if (string.IsNullOrWhiteSpace(observacao))
                observacao = empresa.Configuracao.ObservacaoCTeNormal;
            else if (!string.IsNullOrWhiteSpace(empresa.Configuracao.ObservacaoCTeNormal))
                observacao = string.Concat(observacao, " / ", empresa.Configuracao.ObservacaoCTeNormal);

            List<Dominio.Entidades.Veiculo> veiculos = new List<Dominio.Entidades.Veiculo>();

            if ((documentos[0].NFe2 != null && documentos[0].NFe2.NFe.infNFe.transp.Items != null) || (documentos[0].NFe3 != null && documentos[0].NFe3.NFe.infNFe.transp.Items != null))
            {
                object[] items = documentos[0].NFe2 != null ? documentos[0].NFe2.NFe.infNFe.transp.Items : documentos[0].NFe3.NFe.infNFe.transp.Items;

                string[] placas = (from item in items select ((Newtonsoft.Json.Linq.JObject)item).Property("placa").Value.ToString()).Distinct().ToArray();

                observacao += " - Veículos: " + string.Join(", ", placas);

                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                veiculos = repVeiculo.BuscarPorPlaca(empresa.Codigo, placas);
            }

            Servicos.CTe servicoCTe = new Servicos.CTe(unitOfWork);

            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = servicoCTe.GerarCTePorListaNFe(documentos, empresa.Codigo, dataEmissao, valorFrete, valorTotalMercadoria, observacao, true, this.Usuario, veiculos);

            if (this.Usuario.TipoAcesso == Dominio.Enumeradores.TipoAcesso.Admin)
            {
                Repositorio.UsuarioCTe repUsuarioCTe = new Repositorio.UsuarioCTe(unitOfWork);

                Dominio.Entidades.UsuarioCTe usuarioCTe = new Dominio.Entidades.UsuarioCTe();

                usuarioCTe.CTe = cte;
                usuarioCTe.Usuario = this.Usuario;

                repUsuarioCTe.Inserir(usuarioCTe);
            }

            if (!servicoCTe.Emitir(cte.Codigo, empresa.Codigo))
                return Json<bool>(false, false, "O CT-e foi salvo, porém, ocorreu uma falha ao emiti-lo.");

            if (!this.AdicionarCTeNaFilaDeConsulta(cte))
                return Json<bool>(false, false, "Não foi possível adicionar o CT-e na fila de consulta.");

            servicoCTe.AtualizarIntegracaoRetornoCTe(cte, unitOfWork);

            return Json<bool>(true, true);
        }

        private object ObterParticipanteDoCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.TipoTomador tipo, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.ParticipanteCTe participante = cte.ObterParticipante(tipo);

            if (participante != null)
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                var retorno = new
                {
                    participante.Bairro,
                    participante.CEP,
                    participante.Complemento,
                    CPF_CNPJ = participante.CPF_CNPJ_Formatado,
                    CodigoAtividade = participante.Atividade != null ? participante.Atividade.Codigo : 0,
                    DescricaoAtividade = participante.Atividade != null ? participante.Atividade.Descricao : string.Empty,
                    participante.Email,
                    participante.EmailStatus,
                    participante.EmailContador,
                    participante.EmailContadorStatus,
                    participante.EmailContato,
                    participante.EmailContatoStatus,
                    participante.Endereco,
                    participante.IE_RG,
                    Cidades = participante.Exterior ? null : from obj in repLocalidade.BuscarPorUF(participante.Localidade.Estado.Sigla, this.EmpresaUsuario.Codigo) select new { obj.Codigo, obj.Descricao },
                    participante.Cidade,
                    SiglaPais = participante.Exterior && participante.Pais != null ? participante.Pais.Sigla : string.Empty,
                    participante.Nome,
                    participante.NomeFantasia,
                    participante.Numero,
                    CodigoLocalidade = participante.Exterior ? 0 : participante.Localidade.Codigo,
                    UF = participante.Exterior ? string.Empty : participante.Localidade.Estado.Sigla,
                    CodigoIBGE = participante.Exterior ? 0 : participante.Localidade.CodigoIBGE,
                    Localidade = participante.Exterior ? string.Empty : participante.Localidade.Descricao,
                    participante.SalvarEndereco,
                    participante.Exterior,
                    participante.Telefone1,
                    participante.Telefone2,
                    participante.Tipo,
                    participante.EmailTransportador,
                    participante.EmailTransportadorStatus,
                    participante.InscricaoSuframa
                };

                return retorno;
            }
            else
            {
                return null;
            }
        }

        private List<string> SalvarCliente(string param, ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.TipoTomador tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<string> erros = new List<string>();

            try
            {
                if (Request[param] != "null")
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
                                Repositorio.DadosCliente repDadosCliente = new Repositorio.DadosCliente(unidadeDeTrabalho);
                                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);

                                bool inserir = false, salvarEndereco = true;

                                Dominio.ObjetosDeValor.Endereco endereco = null;

                                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                                Dominio.Entidades.DadosCliente dadosCliente = null;

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


                                endereco = new Dominio.ObjetosDeValor.Endereco();
                                endereco.NomeFantasia = clienteJS.NomeFantasia;
                                if (!salvarEndereco)
                                {
                                    endereco.Bairro = !string.IsNullOrWhiteSpace(clienteJS.Bairro) && clienteJS.Bairro.Length > 40 ? clienteJS.Bairro.Substring(0, 40) : clienteJS.Bairro;
                                    endereco.CEP = clienteJS.CEP;
                                    endereco.Complemento = clienteJS.Complemento;
                                    endereco.Logradouro = clienteJS.Endereco;
                                    endereco.Cidade = repLocalidade.BuscarPorCodigo(clienteJS.Localidade);
                                    endereco.Numero = clienteJS.Numero;
                                    endereco.Telefone = clienteJS.Telefone1;
                                }
                                else
                                {
                                    cliente.Bairro = !string.IsNullOrWhiteSpace(clienteJS.Bairro) && clienteJS.Bairro.Length > 40 ? clienteJS.Bairro.Substring(0, 40) : clienteJS.Bairro;
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
                                cliente.InscricaoSuframa = clienteJS.InscricaoSuframa;
                                cliente.Nome = clienteJS.RazaoSocial;
                                if (inserir)
                                    cliente.NomeFantasia = clienteJS.NomeFantasia;
                                cliente.Telefone2 = clienteJS.Telefone2;
                                cliente.Tipo = Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? "J" : "F";

                                if (cliente.CPF_CNPJ == 0f || !(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? Utilidades.Validate.ValidarCNPJ(clienteJS.CPFCNPJ) : Utilidades.Validate.ValidarCPF(clienteJS.CPFCNPJ)))
                                    erros.Add(string.Concat("CPF/CNPJ do ", tipo.ToString("G"), " inválida."));

                                if (cliente.Atividade == null)
                                    erros.Add(string.Concat("Atividade do ", tipo.ToString("G"), " inválida."));

                                //if (string.IsNullOrWhiteSpace(cliente.IE_RG))
                                //    erros.Add(string.Concat("IE do ", tipo.ToString("G"), " inválida."));

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

                                dadosCliente = repDadosCliente.Buscar(cte.Empresa.Codigo, cpfCnpj);
                                if (!string.IsNullOrWhiteSpace(clienteJS.EmailsTransportador))
                                {
                                    if (dadosCliente == null)
                                    {
                                        dadosCliente = new Dominio.Entidades.DadosCliente();
                                        dadosCliente.Empresa = cte.Empresa;
                                        dadosCliente.Cliente = cliente;
                                        dadosCliente.Email = clienteJS.EmailsTransportador;
                                        dadosCliente.EmailStatus = clienteJS.StatusEmailsTransportador ? "A" : "I";
                                        repDadosCliente.Inserir(dadosCliente);
                                    }
                                    else
                                    {
                                        dadosCliente.Email = clienteJS.EmailsTransportador;
                                        dadosCliente.EmailStatus = clienteJS.StatusEmailsTransportador ? "A" : "I";
                                        repDadosCliente.Atualizar(dadosCliente);
                                    }
                                }

                                cte.SetarParticipante(cliente, tipo, endereco, dadosCliente);
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

                        //if (pais == null)
                        //    erros.Add(string.Concat("País do ", tipo.ToString("G"), " inválido."));

                        if (!string.IsNullOrWhiteSpace(clienteJS.Emails))
                        {
                            var emails = clienteJS.Emails.Split(';');
                            foreach (string email in emails)
                                if (!Utilidades.Validate.ValidarEmail(email.Trim()))
                                    erros.Add(string.Concat("E-mail (", email, ") do ", tipo.ToString("G"), " inválido."));
                        }

                        if (erros.Count > 0)
                            return erros;

                        cte.SetarParticipanteExportacao(clienteJS, tipo, pais);
                    }
                }
                else
                {
                    Dominio.Entidades.ParticipanteCTe part = cte.ObterParticipante(tipo);

                    if (part != null)
                    {
                        Repositorio.ParticipanteCTe repParticipante = new Repositorio.ParticipanteCTe(unidadeDeTrabalho);

                        cte.SetarParticipante(null, tipo);

                        //repParticipante.Deletar(part);
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

        private List<string> SalvarLocalEntrega(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            List<string> erros = new List<string>();
            try
            {
                if (Request["LocalEntregaDiferenteDestinatario"] != "null")
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                    Dominio.ObjetosDeValor.Cliente clienteJS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Cliente>(Request.Params["LocalEntregaDiferenteDestinatario"]);

                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                    double cpfCnpj = 0f;
                    if (double.TryParse(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ), out cpfCnpj))
                    {
                        bool inserir = false;
                        Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);
                        if (cliente == null)
                        {
                            cliente = new Dominio.Entidades.Cliente();
                            inserir = true;
                        }
                        cliente.Atividade = repAtividade.BuscarPorCodigo(clienteJS.CodigoAtividade);
                        cliente.Bairro = clienteJS.Bairro;
                        cliente.CEP = clienteJS.CEP;
                        cliente.Complemento = clienteJS.Complemento;
                        cliente.CPF_CNPJ = cpfCnpj;
                        cliente.Endereco = clienteJS.Endereco;
                        cliente.IE_RG = clienteJS.RGIE;
                        cliente.Localidade = repLocalidade.BuscarPorCodigo(clienteJS.Localidade);
                        cliente.Nome = clienteJS.RazaoSocial;
                        cliente.NomeFantasia = clienteJS.NomeFantasia;
                        cliente.Numero = clienteJS.Numero;
                        cliente.Telefone1 = clienteJS.Telefone1;
                        cliente.Telefone2 = clienteJS.Telefone2;
                        cliente.Tipo = Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? "J" : "F";

                        if (cliente.CPF_CNPJ == 0f || !(Utilidades.String.OnlyNumbers(clienteJS.CPFCNPJ).Length == 14 ? Utilidades.Validate.ValidarCNPJ(clienteJS.CPFCNPJ) : Utilidades.Validate.ValidarCPF(clienteJS.CPFCNPJ)))
                            erros.Add(string.Concat("CPF/CNPJ do local de entrega diferente do local do destinatário inválida."));
                        if (cliente.Atividade == null)
                            erros.Add(string.Concat("Atividade do local de entrega diferente do local do destinatário inválida."));
                        if (string.IsNullOrWhiteSpace(cliente.Bairro))
                            erros.Add(string.Concat("Bairro do local de entrega diferente do local do destinatário inválido."));
                        if (string.IsNullOrWhiteSpace(cliente.CEP) || Utilidades.String.OnlyNumbers(cliente.CEP).Length != 8)
                            erros.Add(string.Concat("CEP do local de entrega diferente do local do destinatário inválido."));
                        if (string.IsNullOrWhiteSpace(cliente.Endereco))
                            erros.Add(string.Concat("Endereço do local de entrega diferente do local do destinatário inválido."));
                        if (string.IsNullOrWhiteSpace(cliente.Nome))
                            erros.Add(string.Concat("Nome/Razão Social do local de entrega diferente do local do destinatário inválido."));
                        if (cliente.Localidade == null)
                            erros.Add(string.Concat("Localidade do local de entrega diferente do local do destinatário inválida."));

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

                        cte.ClienteEntrega = cliente;
                        return erros;
                    }
                    else
                    {
                        cte.ClienteEntrega = null;
                    }
                }
                else
                {
                    cte.ClienteEntrega = null;
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                erros.Add(string.Concat("Inconsistência nos dados do local de entrega diferente do local do destinatário."));
            }
            return erros;
        }

        private List<string> ValidarCTeParaEmissao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho, bool emitir, bool editando = true)
        {
            List<string> listaErros = new List<string>();

            if (cte.ModeloDocumentoFiscal != null && cte.ModeloDocumentoFiscal.Numero == "67")
            {
                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.NaoInformado)
                    listaErros.Add("Tomador não informado.");

                if (cte.TipoServico != Dominio.Enumeradores.TipoServico.TransporteDePessoas && cte.TipoServico != Dominio.Enumeradores.TipoServico.TransporteDeValores && cte.TipoServico != Dominio.Enumeradores.TipoServico.ExcessoDeBagagem)
                    listaErros.Add("Tipo serviço informado não disponível para CT-e OS.");

                if (cte.DataEmissao == DateTime.MinValue)
                    listaErros.Add("Data de emissão inválida.");

                if (cte.LocalidadeEmissao == null)
                    listaErros.Add("Localidade de emissão do CT-e inválida.");

                if (cte.LocalidadeInicioPrestacao == null)
                    listaErros.Add("Localidade de início da prestação inválida.");

                if (cte.LocalidadeTerminoPrestacao == null)
                    listaErros.Add("Localidade de término da prestação inválida.");

                if (cte.Remetente == null && emitir)
                    listaErros.Add("Remetente inválido.");

                if (string.IsNullOrWhiteSpace(cte.CaracteristicaServico) && emitir)
                    listaErros.Add("Características do Serviço não informada.");

                if (cte.CST == null && emitir)
                    listaErros.Add("CST do ICMS inválido.");

                if (string.IsNullOrWhiteSpace(cte.Empresa.TAF) && emitir)
                    listaErros.Add("Transportadora sem TAF (Termo de Autorização de Fretamento) cadastrada.");

                if (string.IsNullOrWhiteSpace(cte.Empresa.NroRegEstadual) && emitir)
                    listaErros.Add("Transportadora sem Número do Registro Estadual.");

                if (emitir && cte.Veiculos != null && cte.Veiculos.Count > 0 && cte.Veiculos.FirstOrDefault().Veiculo != null && cte.Veiculos.FirstOrDefault().Veiculo.Tipo == "T" && cte.Veiculos.FirstOrDefault().Proprietario != null && string.IsNullOrWhiteSpace(cte.Veiculos.FirstOrDefault().Veiculo.TAF))
                    listaErros.Add("Proprietário do veículo sem TAF (Termo de Autorização de Fretamento) cadastrado.");

                if (emitir && cte.Veiculos != null && cte.Veiculos.Count > 0 && cte.Veiculos.FirstOrDefault().Veiculo != null && cte.Veiculos.FirstOrDefault().Veiculo.Tipo == "T" && cte.Veiculos.FirstOrDefault().Proprietario != null && string.IsNullOrWhiteSpace(cte.Veiculos.FirstOrDefault().Veiculo.NroRegEstadual))
                    listaErros.Add("Proprietário do veículo sem sem Número do Registro Estadual.");
            }
            else
            {
                if (emitir)//if (cte.Status != "S")
                {
                    if (cte.Versao == "3.00")
                    {
                        if (cte.IndicadorIETomador == 0)
                            listaErros.Add("Indicação do tomador não informada.");
                        else if (cte.IndicadorIETomador == Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE.ContribuinteICMS)
                        {
                            if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && cte.Remetente != null && string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cte.Remetente.IE_RG)))
                                listaErros.Add("Quando Indicação do Tomador for Contribuinte ICMS deve-se informar uma Inscrição estadual valida da IE para o Remetente.");
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && cte.Destinatario != null && string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cte.Destinatario.IE_RG)))
                                listaErros.Add("Quando Indicação do Tomador for Contribuinte ICMS deve-se informar uma Inscrição estadual valida da IE para o Destinatário.");
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && cte.Expedidor != null && string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cte.Expedidor.IE_RG)))
                                listaErros.Add("Quando Indicação do Tomador for Contribuinte ICMS deve-se informar uma Inscrição estadual valida da IE para o Expedidor.");
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && cte.Recebedor != null && string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cte.Recebedor.IE_RG)))
                                listaErros.Add("Quando Indicação do Tomador for Contribuinte ICMS deve-se informar uma Inscrição estadual valida da IE para o Recebedor.");
                            else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cte.OutrosTomador != null && string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cte.OutrosTomador.IE_RG)))
                                listaErros.Add("Quando Indicação do Tomador for Contribuinte ICMS deve-se informar uma Inscrição estadual valida da IE para o Tomador.");
                        }

                    }

                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.NaoInformado)
                        listaErros.Add("Tomador não informado.");

                    if (cte.Serie == null)
                    {
                        listaErros.Add("Não foi informada Série para emissão do CT-e.");
                    }

                    //if (cte.CST != null && cte.CST == "90")
                    //{
                    //    if (cte.LocalidadeEmissao.Estado.Sigla == cte.LocalidadeInicioPrestacao.Estado.Sigla)
                    //        listaErros.Add("A CST 90 deve ser utilizada quando a UF de Emissão é Diferente da UF de Início da Prestação.");
                    //}

                    if (cte.CFOP != null)
                    {
                        //if (!editando)
                        //{
                        //    if (cte.TipoServico != Dominio.Enumeradores.TipoServico.ServVinculadoMultimodal)
                        //    {
                        //        if (cte.LocalidadeInicioPrestacao.Estado.Sigla != cte.LocalidadeTerminoPrestacao.Estado.Sigla)
                        //        {
                        //            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao)
                        //            {
                        //                if (cte.CFOP.CodigoCFOP >= 7000 && cte.CFOP.CodigoCFOP <= 7999)
                        //                {
                        //                    if (cte.Destinatario == null || !cte.Destinatario.Exterior)
                        //                        listaErros.Add("A CFOP utilizada é para exportação. O destinatário deve ser do tipo exportação.");
                        //                }
                        //                else if (cte.CFOP.CodigoCFOP < 6000 || cte.CFOP.CodigoCFOP > 6999)
                        //                {
                        //                    listaErros.Add("A CFOP para operações entre estados diferentes deve iniciar com 6.");
                        //                }
                        //                else if (cte.LocalidadeEmissao.Estado.Sigla == cte.LocalidadeInicioPrestacao.Estado.Sigla)
                        //                {
                        //                    if (cte.CFOP.CodigoCFOP < 6300 || cte.CFOP.CodigoCFOP > 6399)
                        //                    {
                        //                        listaErros.Add("A CFOP para operações entre estados diferentes com estado início igual ao estado de emissão deve iniciar com 63.");
                        //                    }
                        //                }
                        //                else if (cte.LocalidadeEmissao.Estado.Sigla != cte.LocalidadeInicioPrestacao.Estado.Sigla)
                        //                {
                        //                    if (cte.CFOP.CodigoCFOP < 6900 || cte.CFOP.CodigoCFOP > 6999)
                        //                    {
                        //                        listaErros.Add("A CFOP para operações entre estados diferentes com estado início diferente do estado de emissão deve iniciar com 69.");
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (cte.CFOP.CodigoCFOP < 2000 || cte.CFOP.CodigoCFOP > 2999)
                        //                    listaErros.Add("A CFOP para operações entre estados diferentes deve iniciar com 2.");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao)
                        //            {
                        //                if (cte.CFOP.CodigoCFOP >= 7000 && cte.CFOP.CodigoCFOP <= 7999)
                        //                {
                        //                    if (cte.Destinatario == null || !cte.Destinatario.Exterior)
                        //                        listaErros.Add("A CFOP utilizada é para exportação. O destinatário deve ser do tipo exportação.");
                        //                }
                        //                else if (cte.CFOP.CodigoCFOP < 5000 || cte.CFOP.CodigoCFOP > 5999)
                        //                {
                        //                    listaErros.Add("A CFOP para operações dentro do estado deve iniciar com 5.");
                        //                }
                        //            }
                        //            else
                        //            {
                        //                if (cte.CFOP.CodigoCFOP < 1000 || cte.CFOP.CodigoCFOP > 1999)
                        //                    listaErros.Add("A CFOP para operações dentro do estado deve iniciar com 1.");
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    else
                    {
                        listaErros.Add("CFOP inválida.");
                    }


                    if (cte.CST == null)
                        listaErros.Add("CST do ICMS inválido.");

                    if (cte.DataEmissao == DateTime.MinValue)
                        listaErros.Add("Data de emissão inválida.");

                    if (cte.DataPrevistaEntrega != null && cte.DataPrevistaEntrega.Value == DateTime.MinValue && cte.Versao == "2.00")
                        listaErros.Add("Data prevista de entrega inválida.");

                    if (cte.Remetente == null && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
                        listaErros.Add("Remetente inválido.");

                    if (cte.Destinatario == null && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
                        listaErros.Add("Destinatário inválido.");

                    if (cte.LocalidadeEmissao == null)
                        listaErros.Add("Localidade de emissão do CT-e inválida.");

                    if (cte.LocalidadeInicioPrestacao == null)
                        listaErros.Add("Localidade de início da prestação inválida.");

                    if (cte.LocalidadeTerminoPrestacao == null)
                        listaErros.Add("Localidade de término da prestação inválida.");

                    if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && cte.OutrosTomador == null)
                        listaErros.Add("Tomador inválido.");

                    if (cte.ValorFrete <= 0 && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                        listaErros.Add("Valor do frete inválido.");

                    if (cte.ValorPrestacaoServico <= 0 && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                        listaErros.Add("Valor da prestação de serviços inválido.");

                    if (cte.ValorAReceber <= 0 && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento)
                        listaErros.Add("Valor a receber inválido.");

                    if (cte.IncluirICMSNoFrete == Dominio.Enumeradores.OpcaoSimNao.Sim)
                        if (cte.PercentualICMSIncluirNoFrete <= 0 || cte.PercentualICMSIncluirNoFrete > 100)
                            listaErros.Add("Percentual de ICMS a incluir no valor do frete inválido.");

                    if ((cte.ValorTotalMercadoria <= 0 && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao) && !this.EmitirSemValorDaCarga())
                        listaErros.Add("Valor total da carga inválido.");

                    if (string.IsNullOrWhiteSpace(cte.ProdutoPredominante))
                        listaErros.Add("Produto predominante inválido.");

                    if (string.IsNullOrWhiteSpace(cte.RNTRC))
                        listaErros.Add("RNTRC inválido.");

                    if (editando)
                    {

                        if (cte.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.Versao == "2.00")
                        {
                            if (string.IsNullOrWhiteSpace(Request.Params["Veiculos"]))
                                listaErros.Add("Nenhum veículo informado.");
                            //if (string.IsNullOrWhiteSpace(Request.Params["Motoristas"]))
                            //    listaErros.Add("Nenhum motorista informado.");
                        }

                        if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && string.IsNullOrWhiteSpace(Request.Params["NotasFiscaisRemetente"]) && string.IsNullOrWhiteSpace(Request.Params["NFERemetente"]) && string.IsNullOrWhiteSpace(Request.Params["OutrosDocumentosRemetente"]) && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
                            listaErros.Add("Nenhum documento fiscal do remetente informado.");

                        if (string.IsNullOrWhiteSpace(Request.Params["InformacoesQuantidadeCarga"]))
                            listaErros.Add("Nenhuma informação de quantidade da carga informada.");

                        if (string.IsNullOrWhiteSpace(Request.Params["InformacoesSeguro"]) && cte.Versao == "2.00")
                            listaErros.Add("Nenhum seguro informado.");
                    }
                    else
                    {
                        if (cte.Lotacao == Dominio.Enumeradores.OpcaoSimNao.Sim && cte.Versao == "2.00")
                        {
                            Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                            Repositorio.MotoristaCTE repMotoristaCTe = new Repositorio.MotoristaCTE(unidadeDeTrabalho);

                            if (repVeiculoCTe.ContarPorCTe(cte.Empresa.Codigo, cte.Codigo) <= 0)
                                listaErros.Add("Nenhum veículo informado.");

                            //if (repMotoristaCTe.ContarPorCTe(cte.Empresa.Codigo, cte.Codigo) <= 0)
                            //    listaErros.Add("Nenhum motorista informado.");
                        }

                        Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unidadeDeTrabalho);

                        if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && repDocumentosCTe.ContarPorCTe(cte.Empresa.Codigo, cte.Codigo) <= 0 && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario)
                            listaErros.Add("Nenhum documento fiscal do remetente informado.");

                        Repositorio.InformacaoCargaCTE repInfoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

                        if (repInfoCargaCTe.ContarPorCTe(cte.Empresa.Codigo, cte.Codigo) <= 0)
                            listaErros.Add("Nenhuma informação de quantidade da carga informada.");

                        Repositorio.SeguroCTE repSeguroCTe = new Repositorio.SeguroCTE(unidadeDeTrabalho);

                        if (repSeguroCTe.ContarPorCTe(cte.Empresa.Codigo, cte.Codigo) <= 0 && cte.Versao == "2.00")
                            listaErros.Add("Nenhum seguro informado.");

                    }

                    // Bloqueio Diversos Destinos
                    bool bloquarEmissaoCTeComUFDestinosDiferentes = cte.Empresa.Configuracao?.BloquearEmissaoCTeComUFDestinosDiferentes ?? false;
                    if (bloquarEmissaoCTeComUFDestinosDiferentes && !ValidarDocumentosDistindos(cte, unidadeDeTrabalho, out string multiplosDestinos))
                        listaErros.Add(multiplosDestinos);

                    //Bloqueio por UF
                    Repositorio.EstadosBloqueadosEmissao repEstadosBloqueadosEmissao = new Repositorio.EstadosBloqueadosEmissao(unidadeDeTrabalho);
                    var listaEstados = repEstadosBloqueadosEmissao.BuscarPorConfiguracao(cte.Empresa.Configuracao?.Codigo ?? 0);
                    if (listaEstados != null && listaEstados.Count > 0 && listaEstados.Any(o => o.Estado.Sigla == cte.LocalidadeInicioPrestacao.Estado.Sigla))
                        listaErros.Add("Empresa está configurada para não permitir emitir CTe com estado destino " + cte.LocalidadeInicioPrestacao.Estado.Sigla);

                    if (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.BloquearEmissaoCTeParaCargaMunicipal && cte.LocalidadeInicioPrestacao.Codigo == cte.LocalidadeTerminoPrestacao.Codigo)
                        listaErros.Add("Empresa está configurada para não permitir emitir CTe com município de início e término igual (cargas municipais).");

                    //Validação para emitir por valor
                    if (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.ValorLimiteFrete > 0 && cte.ValorAReceber > cte.Empresa.Configuracao.ValorLimiteFrete)
                        listaErros.Add("Empresa está configurada para não permitir emitir CTe com valor maior que R$" + cte.Empresa.Configuracao.ValorLimiteFrete.ToString("n2") + ".");
                    else if (cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.ValorLimiteFrete == 0 && cte.Empresa.EmpresaPai != null && cte.Empresa.EmpresaPai.Configuracao != null && cte.Empresa.EmpresaPai.Configuracao.ValorLimiteFrete > 0 && cte.ValorAReceber > cte.Empresa.EmpresaPai.Configuracao.ValorLimiteFrete)
                        listaErros.Add("Não é possível emitir CTe com valor maior que R$" + cte.Empresa.EmpresaPai.Configuracao.ValorLimiteFrete.ToString("n2") + ".");

                    if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Normal && cte.Empresa.Configuracao != null && cte.Empresa.Configuracao.ExigirObservacaoContribuinteValorContainer)
                    {
                        Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                        Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
                        var veiculosCTe = repVeiculoCTe.BuscarPorCTe(cte.Codigo);
                        if (veiculosCTe != null && veiculosCTe.Count > 0)
                        {
                            int veiculosReboquePortaContainers = veiculosCTe.Where(o => o.TipoCarroceria == "04" && o.TipoVeiculo == "1").Count();

                            if (veiculosReboquePortaContainers > 0)
                            {
                                var observacoesContribuinte = repObservacaoContribuinteCTE.BuscarPorCTe(cte.Codigo);
                                bool possuiObsContainerValorFisco = observacoesContribuinte.Where(o => o.Identificador == "ValorContainer").Any();
                                if (!possuiObsContainerValorFisco)
                                    listaErros.Add("Empresa está configurada para exigir observação de contribuinte com identificador 'ValorContainer' quando tiver veiculos do tipo Porta Container..");
                            }

                        }

                    }


                }
            }

            if (cte.CFOP == null)
                listaErros.Add("CFOP inválida");

            if (cte.Serie == null)
                listaErros.Add("Série inválida.");
            else
            {
                Repositorio.ConfiguracaoEmpresaClienteSerie repConfiguracaoEmpresaClienteSerie = new Repositorio.ConfiguracaoEmpresaClienteSerie(unidadeDeTrabalho);
                Dominio.Entidades.ConfiguracaoEmpresaClienteSerie configuracaoEmpresaClienteSerie = null;

                string cnpjTomador = string.Empty;
                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente)
                    cnpjTomador = cte.Remetente?.CPF_CNPJ ?? string.Empty;
                else if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario)
                    cnpjTomador = cte.Destinatario?.CPF_CNPJ ?? string.Empty;
                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor)
                    cnpjTomador = cte.Expedidor?.CPF_CNPJ ?? string.Empty;
                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor)
                    cnpjTomador = cte.Recebedor?.CPF_CNPJ ?? string.Empty;
                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                    cnpjTomador = cte.OutrosTomador?.CPF_CNPJ ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(cnpjTomador))
                    configuracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorCliente(cte.Empresa.Configuracao.Codigo, Utilidades.String.OnlyNumbers(cnpjTomador), Dominio.Enumeradores.TipoTomador.Outros);
                if (configuracaoEmpresaClienteSerie == null && cte.Remetente != null && !string.IsNullOrWhiteSpace(cte.Remetente.CPF_CNPJ))
                    configuracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorCliente(cte.Empresa.Configuracao.Codigo, Utilidades.String.OnlyNumbers(cte.Remetente.CPF_CNPJ), Dominio.Enumeradores.TipoTomador.Remetente);
                if (configuracaoEmpresaClienteSerie == null && cte.Destinatario != null && !string.IsNullOrWhiteSpace(cte.Destinatario.CPF_CNPJ))
                    configuracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorCliente(cte.Empresa.Configuracao.Codigo, Utilidades.String.OnlyNumbers(cte.Destinatario.CPF_CNPJ), Dominio.Enumeradores.TipoTomador.Destinatario);
                if (configuracaoEmpresaClienteSerie == null && cte.Expedidor != null && !string.IsNullOrWhiteSpace(cte.Expedidor.CPF_CNPJ))
                    configuracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorCliente(cte.Empresa.Configuracao.Codigo, Utilidades.String.OnlyNumbers(cte.Expedidor.CPF_CNPJ), Dominio.Enumeradores.TipoTomador.Expedidor);
                if (configuracaoEmpresaClienteSerie == null && cte.Recebedor != null && !string.IsNullOrWhiteSpace(cte.Recebedor.CPF_CNPJ))
                    configuracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorCliente(cte.Empresa.Configuracao.Codigo, Utilidades.String.OnlyNumbers(cte.Recebedor.CPF_CNPJ), Dominio.Enumeradores.TipoTomador.Recebedor);

                if (configuracaoEmpresaClienteSerie != null && configuracaoEmpresaClienteSerie.Serie.Numero != cte.Serie.Numero)
                    listaErros.Add("Cliente possui série " + configuracaoEmpresaClienteSerie.Serie.Numero.ToString() + " configurada para emissão de CTe, não é possível utilizar outra série.");
            }

            if (cte.ModeloDocumentoFiscal == null)
                listaErros.Add("Modelo do documento fiscal inválido.");

            DateTime dataLiberacaoImpostos = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unidadeDeTrabalho).BuscarConfiguracaoPadrao().DataLiberacaoImpostos;

            if (DateTime.Now >= dataLiberacaoImpostos && cte.Empresa.RegimeTributarioCTe == Dominio.ObjetosDeValor.Embarcador.Enumeradores.RegimeTributarioCTe.RegimeNormal && cte.OutrasAliquotas == null)
                listaErros.Add("Dados de IBS e CBS não informados.");

            // Valida os componentes de prestação
            if (!string.IsNullOrWhiteSpace(Request.Params["ComponentesDaPrestacao"]))
            {
                List<string> componentesInseridos = new List<string>();
                List<Dominio.ObjetosDeValor.ComponenteDaPrestacao> componentesDaPrestacao = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ComponenteDaPrestacao>>(Request.Params["ComponentesDaPrestacao"]);
                for (var i = 0; i < componentesDaPrestacao.Count; i++)
                {
                    if (!componentesDaPrestacao[i].Excluir)
                    {
                        string descComp = componentesDaPrestacao[i].Descricao.ToUpper();

                        if (componentesInseridos.Contains(descComp))
                        {
                            listaErros.Add("Existem componentes de prestação duplicados.");
                            break;
                        }
                        else if (cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Complemento && cte.TipoCTE != Dominio.Enumeradores.TipoCTE.Anulacao && componentesDaPrestacao[i].Valor == 0)
                        {
                            listaErros.Add("Existem componentes com valor zerado.");
                            break;
                        }
                        else
                            componentesInseridos.Add(descComp);
                    }
                }
            }



            return listaErros;
        }

        private bool ValidarDocumentosDistindos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, out string erro)
        {
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unitOfWork);

            List<Dominio.Entidades.DocumentosCTE> documentos = repDocumentosCTE.BuscarPorCTe(cte.Codigo);
            erro = string.Empty;

            List<string> destinatarios = new List<string>();

            foreach (Dominio.Entidades.DocumentosCTE documento in documentos)
            {
                if (!string.IsNullOrWhiteSpace(documento.DestinatarioUF) && !destinatarios.Contains(documento.DestinatarioUF))
                    destinatarios.Add(documento.DestinatarioUF);
            }

            if (destinatarios.Count() > 1)
            {
                erro = "CT-e possui notas fiscais com estados destinos diferentes, não é possível emitir.";
                return false;
            }

            return true;
        }

        private void ObterCSTDoICMS(Dominio.Enumeradores.TipoICMS icms, ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            switch (icms)
            {
                case Dominio.Enumeradores.TipoICMS.ICMS_Diferido_51:
                    cte.CST = "51";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Isencao_40:
                    cte.CST = "40";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Nao_Tributado_41:
                    cte.CST = "41";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Normal_00:
                    cte.CST = "00";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90:
                    cte.CST = "91";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90:
                    cte.CST = "90";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60:
                    cte.CST = "60";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20:
                    cte.CST = "20";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Nao;
                    break;
                case Dominio.Enumeradores.TipoICMS.Simples_Nacional:
                    cte.CST = "";
                    cte.SimplesNacional = Dominio.Enumeradores.OpcaoSimNao.Sim;
                    break;
            }
        }

        private void ObterCSTDoPIS(Dominio.Enumeradores.TipoPIS pis, ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            switch (pis)
            {
                case Dominio.Enumeradores.TipoPIS.Operacao_Com_Suspensao_Da_Contribuicao_09:
                    cte.CSTPIS = "09";
                    break;
                case Dominio.Enumeradores.TipoPIS.Operacao_Isenta_Da_Contribuicao_07:
                    cte.CSTPIS = "07";
                    break;
                case Dominio.Enumeradores.TipoPIS.Operacao_Sem_Incidencia_Da_Contribuicao_08:
                    cte.CSTPIS = "08";
                    break;
                case Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_A_Aliquota_Zero_06:
                    cte.CSTPIS = "06";
                    break;
                case Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_Com_Aliquota_Basica_01:
                    cte.CSTPIS = "01";
                    break;
                case Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_Com_Aliquota_Diferenciada_02:
                    cte.CSTPIS = "02";
                    break;
                case Dominio.Enumeradores.TipoPIS.Outras_Operacoes_99:
                    cte.CSTPIS = "99";
                    break;
                case Dominio.Enumeradores.TipoPIS.Outras_Operacoes_De_Saida_49:
                    cte.CSTPIS = "49";
                    break;
            }
        }

        private void ObterCSTDoCOFINS(Dominio.Enumeradores.TipoCOFINS cofins, ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            switch (cofins)
            {
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Com_Suspensao_Da_Contribuicao_09:
                    cte.CSTCOFINS = "09";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Isenta_Da_Contribuicao_07:
                    cte.CSTCOFINS = "07";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Sem_Incidencia_Da_Contribuicao_08:
                    cte.CSTCOFINS = "08";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_A_Aliquota_Zero_06:
                    cte.CSTCOFINS = "06";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_Com_Aliquota_Basica_01:
                    cte.CSTCOFINS = "01";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_Com_Aliquota_Diferenciada_02:
                    cte.CSTCOFINS = "02";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Outras_Operacoes_99:
                    cte.CSTCOFINS = "99";
                    break;
                case Dominio.Enumeradores.TipoCOFINS.Outras_Operacoes_De_Saida_49:
                    cte.CSTCOFINS = "49";
                    break;
            }
        }

        private Dominio.Enumeradores.TipoICMS ObterCSTDoICMS(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            switch (cte.CST)
            {
                case "91":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90;
                case "90":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90;
                case "51":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Diferido_51;
                case "40":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Isencao_40;
                case "41":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Nao_Tributado_41;
                case "00":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Normal_00;
                case "60":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60;
                case "20":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20;
                case "":
                    return Dominio.Enumeradores.TipoICMS.Simples_Nacional;
                default:
                    return 0;
            }
        }

        private Dominio.Enumeradores.TipoICMS ObterCSTDoICMS(string cst)
        {
            switch (cst)
            {
                case "91":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Outras_Situacoes_90;
                case "90":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Devido_A_UF_Origem_Prestação_Quando_Diferente_UF_Emitente_90;
                case "51":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Diferido_51;
                case "40":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Isencao_40;
                case "41":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Nao_Tributado_41;
                case "00":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Normal_00;
                case "60":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Pagto_Atr_Tomador_3o_Previsto_Para_ST_60;
                case "20":
                    return Dominio.Enumeradores.TipoICMS.ICMS_Reducao_Base_Calculo_20;
                case "":
                    return Dominio.Enumeradores.TipoICMS.Simples_Nacional;
                case "SN":
                    return Dominio.Enumeradores.TipoICMS.Simples_Nacional;
                default:
                    return 0;
            }
        }

        private Dominio.Enumeradores.TipoPIS ObterCSTDoPIS(string pis)
        {
            switch (pis)
            {
                case "09":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Com_Suspensao_Da_Contribuicao_09;
                case "07":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Isenta_Da_Contribuicao_07;
                case "08":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Sem_Incidencia_Da_Contribuicao_08;
                case "06":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_A_Aliquota_Zero_06;
                case "01":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_Com_Aliquota_Basica_01;
                case "02":
                    return Dominio.Enumeradores.TipoPIS.Operacao_Tributavel_Com_Aliquota_Diferenciada_02;
                case "99":
                    return Dominio.Enumeradores.TipoPIS.Outras_Operacoes_99;
                case "49":
                    return Dominio.Enumeradores.TipoPIS.Outras_Operacoes_De_Saida_49;
                default:
                    return Dominio.Enumeradores.TipoPIS.Outras_Operacoes_99;
            }
        }

        private Dominio.Enumeradores.TipoCOFINS ObterCSTDoCOFINS(string cofins)
        {
            switch (cofins)
            {
                case "09":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Com_Suspensao_Da_Contribuicao_09;
                case "07":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Isenta_Da_Contribuicao_07;
                case "08":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Sem_Incidencia_Da_Contribuicao_08;
                case "06":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_A_Aliquota_Zero_06;
                case "01":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_Com_Aliquota_Basica_01;
                case "02":
                    return Dominio.Enumeradores.TipoCOFINS.Operacao_Tributavel_Com_Aliquota_Diferenciada_02;
                case "99":
                    return Dominio.Enumeradores.TipoCOFINS.Outras_Operacoes_99;
                case "49":
                    return Dominio.Enumeradores.TipoCOFINS.Outras_Operacoes_De_Saida_49;
                default:
                    return Dominio.Enumeradores.TipoCOFINS.Outras_Operacoes_99;
            }
        }

        private void SalvarComponentesDaPrestacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ComponentesDaPrestacao"]))
            {
                Repositorio.ComponentePrestacaoCTE repComponentesDaPrestacao = new Repositorio.ComponentePrestacaoCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.ComponenteDaPrestacao> componentesDaPrestacao = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ComponenteDaPrestacao>>(Request.Params["ComponentesDaPrestacao"]);
                for (var i = 0; i < componentesDaPrestacao.Count; i++)
                {
                    Dominio.Entidades.ComponentePrestacaoCTE componente = repComponentesDaPrestacao.BuscarPorCodigoECTe(cte.Codigo, componentesDaPrestacao[i].Id);
                    if (!componentesDaPrestacao[i].Excluir)
                    {
                        if (componente == null)
                            componente = new Dominio.Entidades.ComponentePrestacaoCTE();
                        componente.CTE = cte;
                        componente.Nome = componentesDaPrestacao[i].Descricao;
                        componente.Valor = componentesDaPrestacao[i].Valor;
                        componente.IncluiNaBaseDeCalculoDoICMS = componentesDaPrestacao[i].IncluiBaseCalculoICMS;
                        componente.IncluiNoTotalAReceber = componentesDaPrestacao[i].IncluiValorAReceber;
                        if (componente.Codigo > 0)
                            repComponentesDaPrestacao.Atualizar(componente);
                        else
                            repComponentesDaPrestacao.Inserir(componente);
                    }
                    else if (componente != null && componente.Codigo > 0)
                    {
                        repComponentesDaPrestacao.Deletar(componente);
                    }
                }
            }
        }

        private void SalvarInformacoesDeSeguro(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["InformacoesSeguro"]))
            {
                Repositorio.SeguroCTE repSeguroCTE = new Repositorio.SeguroCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.InformacaoSeguro> informacoesSeguro = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.InformacaoSeguro>>(Request.Params["InformacoesSeguro"]);
                for (var i = 0; i < informacoesSeguro.Count; i++)
                {
                    Dominio.Entidades.SeguroCTE informacaoSeguro = repSeguroCTE.BuscarPorCTeECodigo(cte.Codigo, informacoesSeguro[i].Id);
                    if (!informacoesSeguro[i].Excluir)
                    {
                        if (informacaoSeguro == null)
                            informacaoSeguro = new Dominio.Entidades.SeguroCTE();
                        informacaoSeguro.NomeSeguradora = informacoesSeguro[i].Seguradora;
                        informacaoSeguro.NumeroApolice = informacoesSeguro[i].NumeroApolice;
                        informacaoSeguro.CNPJSeguradora = informacoesSeguro[i].CNPJSeguradora;
                        informacaoSeguro.NumeroAverbacao = informacoesSeguro[i].NumeroAverberacao;
                        informacaoSeguro.Tipo = informacoesSeguro[i].Responsavel;
                        informacaoSeguro.Valor = informacoesSeguro[i].ValorMercadoria;
                        informacaoSeguro.CTE = cte;
                        if (informacaoSeguro.Codigo > 0)
                            repSeguroCTE.Atualizar(informacaoSeguro);
                        else
                            repSeguroCTE.Inserir(informacaoSeguro);
                    }
                    else if (informacaoSeguro != null && informacaoSeguro.Codigo > 0)
                    {
                        repSeguroCTE.Deletar(informacaoSeguro);
                    }
                }
            }
        }

        private void SalvarInformacoesDeQuantidadeDaCarga(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["InformacoesQuantidadeCarga"]))
            {
                Repositorio.InformacaoCargaCTE repInformacaoCargaCTE = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.InformacaoCarga> informacoesCarga = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.InformacaoCarga>>(Request.Params["InformacoesQuantidadeCarga"]);
                for (var i = 0; i < informacoesCarga.Count; i++)
                {
                    Dominio.Entidades.InformacaoCargaCTE informacao = repInformacaoCargaCTE.BuscarPorCTeECodigo(cte.Codigo, informacoesCarga[i].Id);
                    if (!informacoesCarga[i].Excluir)
                    {
                        if (informacao == null)
                            informacao = new Dominio.Entidades.InformacaoCargaCTE();
                        informacao.CTE = cte;
                        informacao.Quantidade = Math.Round(informacoesCarga[i].Quantidade, 4, MidpointRounding.ToEven);
                        informacao.Tipo = informacoesCarga[i].TipoUnidade.Length > 20 ? informacoesCarga[i].TipoUnidade.Substring(0, 20) : informacoesCarga[i].TipoUnidade;
                        informacao.UnidadeMedida = int.Parse(informacoesCarga[i].UnidadeMedida.ToString("D")).ToString("00");
                        if (informacao.Codigo > 0)
                            repInformacaoCargaCTE.Atualizar(informacao);
                        else
                            repInformacaoCargaCTE.Inserir(informacao);
                    }
                    else if (informacao != null && informacao.Codigo > 0)
                    {
                        repInformacaoCargaCTE.Deletar(informacao);
                    }
                }
            }
        }

        private void SalvarNFEsDoRemetente(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            if (!string.IsNullOrWhiteSpace(Request.Params["NFERemetente"]) && Request.Params["TipoDocumentoRemetente"] == "1")
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.NotasRemetente> nfesRemetente = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NotasRemetente>>(Request.Params["NFERemetente"]);

                for (var i = 0; i < nfesRemetente.Count; i++)
                {
                    Dominio.Entidades.DocumentosCTE documento = nfesRemetente[i].Codigo > 0 ? repDocumentosCTE.BuscarPorCodigoECTe(nfesRemetente[i].Codigo, cte.Codigo) : new Dominio.Entidades.DocumentosCTE();

                    if (!nfesRemetente[i].Excluir)
                    {
                        DateTime.TryParseExact(nfesRemetente[i].DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataEmissaoNFeRemetente);

                        documento.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorModelo("55");
                        documento.Numero = nfesRemetente[i].Numero;
                        documento.RemetenteUF = nfesRemetente[i].RemetenteUF;
                        documento.DestinatarioUF = nfesRemetente[i].DestinatarioUF;
                        documento.Numero = nfesRemetente[i].Numero;
                        documento.ChaveNFE = nfesRemetente[i].Chave.Replace(" ", "");
                        documento.CTE = cte;
                        documento.DataEmissao = dataEmissaoNFeRemetente > DateTime.MinValue ? dataEmissaoNFeRemetente : DateTime.Today;
                        documento.Valor = nfesRemetente[i].ValorTotal;
                        documento.Peso = nfesRemetente[i].Peso;
                        if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                            documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);


                        if (documento.Codigo > 0)
                            repDocumentosCTE.Atualizar(documento);
                        else
                            repDocumentosCTE.Inserir(documento);
                    }
                    else if (documento != null)
                    {
                        repDocumentosCTE.Deletar(documento);
                    }
                }
            }
            else
            {
                List<string> modelos = new List<string>();
                modelos.Add("55");
                var listaDocumentosCTe = repDocumentosCTE.BuscarPorCTeEModelos(cte.Codigo, modelos);
                foreach (Dominio.Entidades.DocumentosCTE documento in listaDocumentosCTe)
                    repDocumentosCTE.Deletar(documento);
            }
        }

        private void SalvarNotasFiscaisDoRemetente(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            if (!string.IsNullOrWhiteSpace(Request.Params["NotasFiscaisRemetente"]) && Request.Params["TipoDocumentoRemetente"] == "2")
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.NotasRemetente> notasFiscaisRemetente = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NotasRemetente>>(Request.Params["NotasFiscaisRemetente"]);
                for (var i = 0; i < notasFiscaisRemetente.Count; i++)
                {
                    Dominio.Entidades.DocumentosCTE documento = repDocumentosCTE.BuscarPorCodigoECTe(notasFiscaisRemetente[i].Codigo, cte.Codigo);
                    if (!notasFiscaisRemetente[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.DocumentosCTE();
                        documento.Numero = notasFiscaisRemetente[i].Numero;
                        documento.BaseCalculoICMS = notasFiscaisRemetente[i].BaseCalculoICMS;
                        documento.BaseCalculoICMSST = notasFiscaisRemetente[i].BaseCalculoICMSST;
                        documento.CFOP = notasFiscaisRemetente[i].CFOP;
                        documento.Peso = notasFiscaisRemetente[i].Peso;
                        documento.PINSuframa = notasFiscaisRemetente[i].PIN;
                        documento.ValorICMS = notasFiscaisRemetente[i].ValorICMS;
                        documento.ValorICMSST = notasFiscaisRemetente[i].ValorICMSST;
                        documento.ValorProdutos = notasFiscaisRemetente[i].ValorProdutos;
                        documento.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(notasFiscaisRemetente[i].Modelo);
                        documento.Serie = notasFiscaisRemetente[i].Serie;
                        documento.CTE = cte;
                        DateTime dataEmissaoNotaFiscalRemetente;
                        DateTime.TryParseExact(notasFiscaisRemetente[i].DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoNotaFiscalRemetente);
                        documento.DataEmissao = dataEmissaoNotaFiscalRemetente;
                        documento.Valor = notasFiscaisRemetente[i].ValorTotal;
                        if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                            documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);
                        if (documento.Codigo > 0)
                            repDocumentosCTE.Atualizar(documento);
                        else
                            repDocumentosCTE.Inserir(documento);
                    }
                    else if (documento != null)
                    {
                        repDocumentosCTE.Deletar(documento);
                    }
                }
            }
            else
            {
                List<string> modelos = new List<string>();
                modelos.Add("01");
                modelos.Add("04");
                var listaDocumentosCTe = repDocumentosCTE.BuscarPorCTeEModelos(cte.Codigo, modelos);
                foreach (Dominio.Entidades.DocumentosCTE documento in listaDocumentosCTe)
                    repDocumentosCTE.Deletar(documento);
            }
        }

        private void SalvarOutrosDocumentosDoRemetente(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXMLNotaFiscalEletronica = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unidadeDeTrabalho);
            Repositorio.DocumentosCTE repDocumentosCTE = new Repositorio.DocumentosCTE(unidadeDeTrabalho);
            if (!string.IsNullOrWhiteSpace(Request.Params["OutrosDocumentosRemetente"]) && Request.Params["TipoDocumentoRemetente"] == "3")
            {
                Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.NotasRemetente> outrosDocumentosRemetente = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.NotasRemetente>>(Request.Params["OutrosDocumentosRemetente"]);
                for (var i = 0; i < outrosDocumentosRemetente.Count; i++)
                {
                    Dominio.Entidades.DocumentosCTE documento = repDocumentosCTE.BuscarPorCodigoECTe(outrosDocumentosRemetente[i].Codigo, cte.Codigo);
                    if (!outrosDocumentosRemetente[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.DocumentosCTE();
                        documento.Numero = outrosDocumentosRemetente[i].Numero;
                        documento.ModeloDocumentoFiscal = repModeloDocumentoFiscal.BuscarPorId(outrosDocumentosRemetente[i].Modelo);
                        documento.CTE = cte;
                        DateTime dataEmissaoOutroDocumentoRemetente;
                        DateTime.TryParseExact(outrosDocumentosRemetente[i].DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoOutroDocumentoRemetente);
                        documento.DataEmissao = dataEmissaoOutroDocumentoRemetente;
                        documento.Valor = outrosDocumentosRemetente[i].ValorTotal;
                        documento.Descricao = outrosDocumentosRemetente[i].Descricao;
                        if (!string.IsNullOrWhiteSpace(documento.ChaveNFE))
                            documento.XMLNotaFiscal = repXMLNotaFiscalEletronica.BuscarPorChave(documento.ChaveNFE);
                        if (documento.Codigo > 0)
                            repDocumentosCTE.Atualizar(documento);
                        else
                            repDocumentosCTE.Inserir(documento);
                    }
                    else if (documento != null)
                    {
                        repDocumentosCTE.Deletar(documento);
                    }
                }
            }
            else
            {
                List<string> modelos = new List<string>();
                modelos.Add("00");
                modelos.Add("99");
                var listaDocumentosCTe = repDocumentosCTE.BuscarPorCTeEModelos(cte.Codigo, modelos);
                foreach (Dominio.Entidades.DocumentosCTE documento in listaDocumentosCTe)
                    repDocumentosCTE.Deletar(documento);
            }
        }

        private void SalvarVeiculos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Veiculos"]))
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);
                Repositorio.VeiculoCTE repVeiculoCTE = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                Repositorio.ProprietarioVeiculoCTe repProprietario = new Repositorio.ProprietarioVeiculoCTe(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.Veiculo> veiculos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Veiculo>>(Request.Params["Veiculos"]);

                for (var i = 0; i < veiculos.Count; i++)
                {
                    Dominio.Entidades.VeiculoCTE veiculo = repVeiculoCTE.BuscarPorCodigoECTe(veiculos[i].Id, cte.Codigo);

                    if (!veiculos[i].Excluir)
                    {
                        if (veiculo == null)
                            veiculo = new Dominio.Entidades.VeiculoCTE();

                        veiculo.CTE = cte;
                        veiculo.Veiculo = repVeiculo.BuscarPorCodigo(veiculos[i].Codigo);
                        veiculo.SetarDadosVeiculo(veiculo.Veiculo);

                        if (veiculo.TipoPropriedade != "T" && veiculo.Proprietario != null)
                        {
                            if (veiculo.Proprietario.Codigo > 0)
                            {
                                Dominio.Entidades.ProprietarioVeiculoCTe prop = repProprietario.BuscarPorCodigo(veiculo.Proprietario.Codigo);

                                veiculo.Proprietario = null;

                                repProprietario.Deletar(prop);
                            }
                            else
                            {
                                veiculo.Proprietario = null;
                            }
                        }

                        if (veiculo.Codigo > 0)
                            repVeiculoCTE.Atualizar(veiculo);
                        else
                            repVeiculoCTE.Inserir(veiculo);

                    }
                    else if (veiculo != null)
                    {
                        if (veiculo.Proprietario != null)
                        {
                            Dominio.Entidades.ProprietarioVeiculoCTe prop = repProprietario.BuscarPorCodigo(veiculo.Proprietario.Codigo);

                            veiculo.Proprietario = null;

                            repProprietario.Deletar(prop);
                        }

                        repVeiculoCTE.Deletar(veiculo);
                    }
                }
            }
        }

        private void SalvarMotoristas(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Motoristas"]))
            {
                Repositorio.MotoristaCTE repMotoristaCTE = new Repositorio.MotoristaCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.Motorista> motoristas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Motorista>>(Request.Params["Motoristas"]);
                for (var i = 0; i < motoristas.Count; i++)
                {
                    Dominio.Entidades.MotoristaCTE motorista = repMotoristaCTE.BuscarPorCodigoECTe(motoristas[i].Codigo, cte.Codigo);
                    if (!motoristas[i].Excluir)
                    {
                        if (motorista == null)
                            motorista = new Dominio.Entidades.MotoristaCTE();
                        motorista.CPFMotorista = Utilidades.String.RemoveDiacritics(motoristas[i].CPF);
                        motorista.CTE = cte;
                        motorista.NomeMotorista = Utilidades.String.RemoveDiacritics(motoristas[i].Nome.Length > 200 ? motoristas[i].Nome.Substring(0, 200) : motoristas[i].Nome);
                        if (motorista.Codigo > 0)
                            repMotoristaCTE.Atualizar(motorista);
                        else
                            repMotoristaCTE.Inserir(motorista);

                        this.CadastrarMotorista(cte.Empresa, motorista, unidadeDeTrabalho);
                    }
                    else if (motorista != null)
                    {
                        repMotoristaCTE.Deletar(motorista);
                    }
                }
            }
        }

        private void CadastrarMotorista(Dominio.Entidades.Empresa empresa, Dominio.Entidades.MotoristaCTE motorista, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["CadastrarMotoristaEmissao"] == "NAO")
                return;

            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unidadeDeTrabalho);
            Repositorio.Setor repSetor = new Repositorio.Setor(unidadeDeTrabalho);
            string cpf = Utilidades.String.OnlyNumbers(motorista.CPFMotorista);
            Dominio.Entidades.Usuario cadastrarMotorista = repMotorista.BuscarMotoristaPorCPF(empresa.Codigo, cpf);

            if (cadastrarMotorista == null)
            {
                cadastrarMotorista = new Dominio.Entidades.Usuario();
                cadastrarMotorista.CPF = cpf;
                cadastrarMotorista.Nome = Utilidades.String.Left(motorista.NomeMotorista, 80);
                cadastrarMotorista.Empresa = empresa;
                cadastrarMotorista.Localidade = empresa.Localidade;
                cadastrarMotorista.Setor = repSetor.BuscarPorCodigo(1);
                cadastrarMotorista.Status = "A";
                cadastrarMotorista.Tipo = "M";

                repMotorista.Inserir(cadastrarMotorista);
            }
        }

        private void SalvarObservacoesContribuinte(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ObservacoesContribuinte"]))
            {
                Repositorio.ObservacaoContribuinteCTE repObservacaoContribuinteCTE = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.ObservacaoCTE> observacoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ObservacaoCTE>>(Request.Params["ObservacoesContribuinte"]);
                for (var i = 0; i < observacoes.Count; i++)
                {
                    Dominio.Entidades.ObservacaoContribuinteCTE observacao = repObservacaoContribuinteCTE.BuscarPorCodigoECTe(observacoes[i].Codigo, cte.Codigo);
                    if (!observacoes[i].Excluir)
                    {
                        if (observacao == null)
                            observacao = new Dominio.Entidades.ObservacaoContribuinteCTE();
                        observacao.CTE = cte;
                        observacao.Descricao = observacoes[i].Descricao;
                        observacao.Identificador = observacoes[i].Identificador;
                        if (observacao.Codigo > 0)
                            repObservacaoContribuinteCTE.Atualizar(observacao);
                        else
                            repObservacaoContribuinteCTE.Inserir(observacao);
                    }
                    else if (observacao != null)
                    {
                        repObservacaoContribuinteCTE.Deletar(observacao);
                    }
                }

                Servicos.CTe svcCTe = new CTe(unidadeDeTrabalho);
                if (repObservacaoContribuinteCTE.BuscarPorCTeEIdentificacao(cte.Codigo, "ObsContAutomatica") == null)
                {
                    var obsPadraoContribuinte = svcCTe.BuscarObservacaoPadrao(cte, Dominio.Enumeradores.TipoObservacao.Contribuinte, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla, cte.CST, cte.ValorICMS, unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                    if (!string.IsNullOrWhiteSpace(obsPadraoContribuinte))
                    {
                        Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);

                        Dominio.Entidades.ObservacaoContribuinteCTE observacao = new Dominio.Entidades.ObservacaoContribuinteCTE();
                        observacao.CTE = cte;
                        observacao.Descricao = obsPadraoContribuinte;
                        observacao.Identificador = "ObsContAutomatica";

                        repObsContribuinte.Inserir(observacao);
                    }
                }

            }
        }

        private void SalvarObservacoesFisco(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ObservacoesFisco"]))
            {
                Repositorio.ObservacaoFiscoCTE repObservacaoFiscoCTE = new Repositorio.ObservacaoFiscoCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.ObservacaoCTE> observacoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ObservacaoCTE>>(Request.Params["ObservacoesFisco"]);
                for (var i = 0; i < observacoes.Count; i++)
                {
                    Dominio.Entidades.ObservacaoFiscoCTE observacao = repObservacaoFiscoCTE.BuscarPorCodigoECTe(observacoes[i].Codigo, cte.Codigo);
                    if (!observacoes[i].Excluir)
                    {
                        if (observacao == null)
                            observacao = new Dominio.Entidades.ObservacaoFiscoCTE();
                        observacao.CTE = cte;
                        observacao.Descricao = observacoes[i].Descricao;
                        observacao.Identificador = observacoes[i].Identificador;
                        if (observacao.Codigo > 0)
                            repObservacaoFiscoCTE.Atualizar(observacao);
                        else
                            repObservacaoFiscoCTE.Inserir(observacao);
                    }
                    else if (observacao != null)
                    {
                        repObservacaoFiscoCTE.Deletar(observacao);
                    }
                }

                Servicos.CTe svcCTe = new CTe(unidadeDeTrabalho);
                if (repObservacaoFiscoCTE.BuscarPorCTeEIdentificacao(cte.Codigo, "ObsContAutomatica") == null)
                {
                    var obsPadraoContribuinte = svcCTe.BuscarObservacaoPadrao(cte, Dominio.Enumeradores.TipoObservacao.Fisco, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla, cte.CST, cte.ValorICMS, unidadeDeTrabalho, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
                    if (!string.IsNullOrWhiteSpace(obsPadraoContribuinte))
                    {
                        Repositorio.ObservacaoContribuinteCTE repObsContribuinte = new Repositorio.ObservacaoContribuinteCTE(unidadeDeTrabalho);

                        Dominio.Entidades.ObservacaoContribuinteCTE observacao = new Dominio.Entidades.ObservacaoContribuinteCTE();
                        observacao.CTE = cte;
                        observacao.Descricao = obsPadraoContribuinte;
                        observacao.Identificador = "ObsContAutomatica";

                        repObsContribuinte.Inserir(observacao);
                    }
                }
            }
        }

        private void SalvarDocumentosDeTransporteAnterioresEletronicos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["DocumentosDeTransporteAnterioresEletronicos"]))
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentosAnterioresCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior> documentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior>>(Request.Params["DocumentosDeTransporteAnterioresEletronicos"]);
                for (var i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = repDocumentosAnterioresCTe.BuscarPorCodigoECTe(documentos[i].Codigo, cte.Codigo);
                    if (!documentos[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();
                        double cpfCnpjEmissor = double.Parse(Utilidades.String.OnlyNumbers(documentos[i].Emissor));
                        documento.CTe = cte;
                        documento.Emissor = repCliente.BuscarPorCPFCNPJ(cpfCnpjEmissor);
                        documento.Chave = Utilidades.String.OnlyNumbers(documentos[i].Chave);
                        if (documento.Codigo > 0)
                            repDocumentosAnterioresCTe.Atualizar(documento);
                        else
                            repDocumentosAnterioresCTe.Inserir(documento);
                    }
                    else if (documento != null)
                    {
                        repDocumentosAnterioresCTe.Deletar(documento);
                    }
                }
            }
        }

        private void SalvarDocumentosDeTransporteAnterioresPapel(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["DocumentosDeTransporteAnterioresPapel"]))
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.DocumentoDeTransporteAnteriorCTe repDocumentosAnterioresCTe = new Repositorio.DocumentoDeTransporteAnteriorCTe(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior> documentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.DocumentoDeTransporteAnterior>>(Request.Params["DocumentosDeTransporteAnterioresPapel"]);
                for (var i = 0; i < documentos.Count; i++)
                {
                    Dominio.Entidades.DocumentoDeTransporteAnteriorCTe documento = repDocumentosAnterioresCTe.BuscarPorCodigoECTe(documentos[i].Codigo, cte.Codigo);
                    if (!documentos[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.DocumentoDeTransporteAnteriorCTe();
                        double cpfCnpjEmissor = double.Parse(Utilidades.String.OnlyNumbers(documentos[i].Emissor));
                        documento.CTe = cte;
                        documento.Emissor = repCliente.BuscarPorCPFCNPJ(cpfCnpjEmissor);
                        documento.DataEmissao = DateTime.ParseExact(documentos[i].DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);
                        documento.Numero = documentos[i].Numero;
                        documento.Serie = documentos[i].Serie;
                        documento.Tipo = documentos[i].Tipo;
                        if (documento.Codigo > 0)
                            repDocumentosAnterioresCTe.Atualizar(documento);
                        else
                            repDocumentosAnterioresCTe.Inserir(documento);
                    }
                    else if (documento != null)
                    {
                        repDocumentosAnterioresCTe.Deletar(documento);
                    }
                }
            }
        }

        private void SalvarProdutosPerigosos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ProdutosPerigosos"]))
            {
                Repositorio.ProdutoPerigosoCTE repProdutosPerigososCTe = new Repositorio.ProdutoPerigosoCTE(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.ProdutoPerigoso> produtos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ProdutoPerigoso>>(Request.Params["ProdutosPerigosos"]);
                for (var i = 0; i < produtos.Count; i++)
                {
                    Dominio.Entidades.ProdutoPerigosoCTE produto = repProdutosPerigososCTe.BuscarPorCodigoECTe(produtos[i].Codigo, cte.Codigo);
                    if (!produtos[i].Excluir)
                    {
                        if (produto == null)
                            produto = new Dominio.Entidades.ProdutoPerigosoCTE();
                        produto.CTE = cte;
                        produto.ClasseRisco = produtos[i].ClasseRisco;
                        produto.Grupo = produtos[i].GrupoEmbalagem;
                        produto.NomeApropriado = produtos[i].NomeApropriado;
                        produto.NumeroONU = produtos[i].NumeroONU;
                        produto.PontoFulgor = produtos[i].PontoDeFulgor;
                        produto.Quantidade = produtos[i].QuantidadeTotal;
                        produto.Volumes = produtos[i].QuantidadeETipo;
                        if (produto.Codigo > 0)
                            repProdutosPerigososCTe.Atualizar(produto);
                        else
                            repProdutosPerigososCTe.Inserir(produto);
                    }
                    else if (produto != null)
                    {
                        repProdutosPerigososCTe.Deletar(produto);
                    }
                }
            }
        }

        private void SalvarPercursos(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Percursos"]))
            {
                Repositorio.PercursoCTe repPercurso = new Repositorio.PercursoCTe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.PercursoCTe> percursos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PercursoCTe>>(Request.Params["Percursos"]);

                if (percursos != null && percursos.Count > 0)
                {
                    for (var i = 0; i < percursos.Count; i++)
                    {
                        Dominio.Entidades.PercursoCTe percurso = repPercurso.BuscarPorCodigo(percursos[i].Codigo, cte.Codigo);

                        if (!percursos[i].Excluir)
                        {
                            if (percurso == null)
                                percurso = new Dominio.Entidades.PercursoCTe();

                            percurso.CTe = cte;
                            percurso.Estado = repEstado.BuscarPorSigla(percursos[i].Sigla);

                            if (percurso.Codigo > 0)
                                repPercurso.Atualizar(percurso);
                            else
                                repPercurso.Inserir(percurso);




                        }
                        else if (percurso != null && percurso.Codigo > 0)
                        {
                            repPercurso.Deletar(percurso);
                        }
                    }
                }
                else
                {
                    Repositorio.PercursoEstado repPercursoEstado = new Repositorio.PercursoEstado(unidadeDeTrabalho);
                    Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unidadeDeTrabalho);

                    Dominio.Entidades.PercursoEstado percurso = repPercursoEstado.Buscar(cte.Empresa.Codigo, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla);
                    if (percurso == null && cte.Empresa.EmpresaPai != null)
                        percurso = repPercursoEstado.Buscar(cte.Empresa.EmpresaPai.Codigo, cte.LocalidadeInicioPrestacao.Estado.Sigla, cte.LocalidadeTerminoPrestacao.Estado.Sigla);
                    if (percurso != null)
                    {
                        List<Dominio.Entidades.PassagemPercursoEstado> passagemPercursoEstado = repPassagem.Buscar(percurso.Codigo);

                        if (cte.LocalidadeInicioPrestacao.Estado.Sigla == "EX")
                        {
                            Dominio.Entidades.PercursoCTe percursoCTe = new Dominio.Entidades.PercursoCTe();
                            percursoCTe.Estado = percurso.EstadoOrigem;
                            percursoCTe.CTe = cte;
                            repPercurso.Inserir(percursoCTe);
                        }

                        foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagemPercursoEstado)
                        {
                            Dominio.Entidades.PercursoCTe percursoCTe = new Dominio.Entidades.PercursoCTe();

                            percursoCTe.Estado = passagem.EstadoDePassagem;
                            percursoCTe.CTe = cte;
                            repPercurso.Inserir(percursoCTe);
                        }

                        if (cte.LocalidadeTerminoPrestacao.Estado.Sigla == "EX")
                        {
                            Dominio.Entidades.PercursoCTe percursoCTe = new Dominio.Entidades.PercursoCTe();
                            percursoCTe.Estado = percurso.EstadoDestino;
                            percursoCTe.CTe = cte;
                            repPercurso.Inserir(percursoCTe);
                        }
                    }

                }
            }
        }

        private void SalvarDadosCobranca(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["DadosCobranca"]))
            {
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                Repositorio.CobrancaCTe repCobranca = new Repositorio.CobrancaCTe(unidadeDeTrabalho);
                Repositorio.ParcelaCobrancaCTe repParcelaCobranca = new Repositorio.ParcelaCobrancaCTe(unidadeDeTrabalho);
                Dominio.Entidades.CobrancaCTe cobranca = repCobranca.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);
                List<Dominio.ObjetosDeValor.Cobranca> parcelasCobranca = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.Cobranca>>(Request.Params["DadosCobranca"]);

                if (parcelasCobranca.Count > 0 && cte.Tomador != null && !string.IsNullOrWhiteSpace(cte.Tomador.CPF_CNPJ))
                {
                    if (cobranca == null)
                    {
                        cobranca = new Dominio.Entidades.CobrancaCTe();
                        cobranca.Numero = repCobranca.BuscarUltimoNumero(this.EmpresaUsuario.Codigo) + 1;
                    }

                    cobranca.Valor = cte.ValorAReceber;
                    cobranca.ValorDesconto = 0;
                    cobranca.ValorLiquido = cte.ValorFrete;
                    cobranca.CTe = cte;
                    if (cte.Tomador.CPF_CNPJ != null)
                        cobranca.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Tomador.CPF_CNPJ));

                    if (cobranca.Codigo > 0)
                        repCobranca.Atualizar(cobranca);
                    else
                        repCobranca.Inserir(cobranca);

                    for (var i = 0; i < parcelasCobranca.Count; i++)
                    {
                        Dominio.Entidades.ParcelaCobrancaCTe parcela = repParcelaCobranca.BuscarPorCodigoECobranca(parcelasCobranca[i].Codigo, cobranca.Codigo);

                        if (!parcelasCobranca[i].Excluir)
                        {
                            if (parcela == null)
                                parcela = new Dominio.Entidades.ParcelaCobrancaCTe();

                            parcela.Cobranca = cobranca;
                            parcela.DataVencimento = DateTime.ParseExact(parcelasCobranca[i].DataVencimento, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None);
                            parcela.Numero = parcelasCobranca[i].Parcela;
                            parcela.Valor = parcelasCobranca[i].Valor;
                            if ((parcelasCobranca.Count == 1 && parcela.Valor == 0) || (parcelasCobranca.Count == 1 && parcela.Valor != cte.ValorAReceber))
                                parcela.Valor = cte.ValorAReceber;

                            if (parcela.Codigo > 0)
                                repParcelaCobranca.Atualizar(parcela);
                            else
                                repParcelaCobranca.Inserir(parcela);
                        }
                        else if (parcela != null)
                        {
                            repParcelaCobranca.Deletar(parcela);
                        }
                    }

                    if (repParcelaCobranca.ContarPorCobranca(cobranca.Codigo) <= 0)
                        repCobranca.Deletar(cobranca);
                }
                else if (cobranca != null)
                {
                    repCobranca.Deletar(cobranca);
                }
                else
                {
                    Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                    svcCTe.SalvarDadosCobrancaAutomatico(this.EmpresaUsuario, ref cte, unidadeDeTrabalho);
                }
            }
            else
            {
                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                svcCTe.SalvarDadosCobrancaAutomatico(this.EmpresaUsuario, ref cte, unidadeDeTrabalho);
            }
        }

        private Dominio.Entidades.ConfiguracaoEmpresa ObterConfiguracao()
        {
            Dominio.Entidades.ConfiguracaoEmpresa configuracao = this.EmpresaUsuario.Configuracao;
            if (this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai != null)
                configuracao = this.EmpresaUsuario.EmpresaPai.Configuracao;
            return configuracao;
        }

        private bool AdicionarCTeNaFilaDeConsulta(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            try
            {
                if (cte.SistemaEmissor != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                    return true;

                string postData = "CodigoCTe=" + cte.Codigo;
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(WebConfigurationManager.AppSettings["UriSistemaEmissaoCTe"], "/IntegracaoCTe/AdicionarNaFilaDeConsulta"));

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bytes.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);

                WebResponse response = request.GetResponse();

                Stream stream = response.GetResponseStream();

                StreamReader reader = new StreamReader(stream);
                var result = reader.ReadToEnd();

                stream.Dispose();
                reader.Dispose();

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var retorno = (System.Collections.Generic.Dictionary<string, object>)serializer.DeserializeObject(result);

                return (bool)retorno["Sucesso"];
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }

        private void SalvarValoresFrete(ref Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Enumeradores.IncluiICMSFrete incluirICMS, decimal valorFrete, decimal valorAdicional, decimal valorPedagio, decimal valorGris, decimal valorTAS, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unitOfWork);

            cte.ValorFrete = Math.Round(valorFrete, 2, MidpointRounding.ToEven);
            cte.ValorPrestacaoServico = Math.Round(cte.ValorFrete + valorPedagio + valorAdicional + valorGris, 2, MidpointRounding.ToEven);
            cte.ValorAReceber = Math.Round(cte.ValorFrete + valorPedagio + valorAdicional + valorGris, 2, MidpointRounding.ToEven);

            Dominio.Entidades.ComponentePrestacaoCTE componenteFreteValorCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
            componenteFreteValorCTe.CTE = cte;
            componenteFreteValorCTe.IncluiNaBaseDeCalculoDoICMS = false;
            componenteFreteValorCTe.IncluiNoTotalAReceber = true;
            componenteFreteValorCTe.Nome = "FRETE VALOR";
            componenteFreteValorCTe.Valor = cte.ValorFrete;

            repComponentePrestacao.Inserir(componenteFreteValorCTe);

            if (valorPedagio > 0)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componentePedagioCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                componentePedagioCTe.CTE = cte;
                componentePedagioCTe.IncluiNaBaseDeCalculoDoICMS = cte.LocalidadeInicioPrestacao.Estado.Sigla == "PR" ? false : true;
                componentePedagioCTe.IncluiNoTotalAReceber = true;
                componentePedagioCTe.Nome = "PEDAGIO";
                componentePedagioCTe.Valor = Math.Round(valorPedagio, 2, MidpointRounding.ToEven);

                repComponentePrestacao.Inserir(componentePedagioCTe);
            }

            if (valorAdicional > 0)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteAdicionalValorCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                componenteAdicionalValorCTe.CTE = cte;
                componenteAdicionalValorCTe.IncluiNaBaseDeCalculoDoICMS = true;
                componenteAdicionalValorCTe.IncluiNoTotalAReceber = true;
                componenteAdicionalValorCTe.Nome = "AD VALOREM";
                componenteAdicionalValorCTe.Valor = Math.Round(valorAdicional, 2, MidpointRounding.ToEven);

                repComponentePrestacao.Inserir(componenteAdicionalValorCTe);
            }

            if (valorGris > 0)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteGris = new Dominio.Entidades.ComponentePrestacaoCTE();
                componenteGris.CTE = cte;
                componenteGris.IncluiNaBaseDeCalculoDoICMS = true;
                componenteGris.IncluiNoTotalAReceber = true;
                componenteGris.Nome = "GRIS";
                componenteGris.Valor = Math.Round(valorGris, 2, MidpointRounding.ToEven);

                repComponentePrestacao.Inserir(componenteGris);
            }


            if (valorTAS > 0)
            {
                Dominio.Entidades.ComponentePrestacaoCTE componenteTAS = new Dominio.Entidades.ComponentePrestacaoCTE();
                componenteTAS.CTE = cte;
                componenteTAS.IncluiNaBaseDeCalculoDoICMS = true;
                componenteTAS.IncluiNoTotalAReceber = true;
                componenteTAS.Nome = "TAS";
                componenteTAS.Valor = Math.Round(valorTAS, 2, MidpointRounding.ToEven);

                repComponentePrestacao.Inserir(componenteTAS);
            }

            if (cte.AliquotaICMS > 0)
            {
                cte.BaseCalculoICMS = cte.ValorAReceber;
                cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                if (incluirICMS == Dominio.Enumeradores.IncluiICMSFrete.Sim)
                {
                    cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Sim;
                    cte.PercentualICMSIncluirNoFrete = 100;

                    cte.BaseCalculoICMS = Math.Round(cte.ValorAReceber / (1 - (cte.AliquotaICMS / 100)), 2, MidpointRounding.ToEven);
                    cte.ValorICMS = Math.Round(cte.BaseCalculoICMS * (cte.AliquotaICMS / 100), 2, MidpointRounding.ToEven);

                    decimal valorImpostoIncluso = Math.Round(cte.BaseCalculoICMS - cte.ValorPrestacaoServico, 2, MidpointRounding.ToEven);
                    if (valorImpostoIncluso > 0)
                    {
                        Dominio.Entidades.ComponentePrestacaoCTE componenteImpostoCTe = new Dominio.Entidades.ComponentePrestacaoCTE();
                        componenteImpostoCTe.CTE = cte;
                        componenteImpostoCTe.IncluiNaBaseDeCalculoDoICMS = false;
                        componenteImpostoCTe.IncluiNoTotalAReceber = false;
                        componenteImpostoCTe.Nome = "IMPOSTOS";
                        componenteImpostoCTe.Valor = Math.Round(valorImpostoIncluso, 2, MidpointRounding.ToEven);

                        repComponentePrestacao.Inserir(componenteImpostoCTe);
                    }

                    cte.ValorPrestacaoServico = cte.BaseCalculoICMS;
                    cte.ValorAReceber = cte.BaseCalculoICMS;
                }
                else
                    cte.IncluirICMSNoFrete = Dominio.Enumeradores.OpcaoSimNao.Nao;
            }
        }

        public Dominio.Entidades.Cliente ObterCliente(string cnpj, string ie, string nome, string rua, string cep, string bairro, string numero, string cidade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

            double cpfCnpj = 0;
            double.TryParse(Utilidades.String.OnlyNumbers(cnpj), out cpfCnpj);

            if (cpfCnpj > 0)
            {

                bool inserir = false;

                Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cpfCnpj);

                if (cliente == null)
                {
                    cliente = new Dominio.Entidades.Cliente();
                    inserir = true;
                }

                cliente.Bairro = bairro;
                cliente.CEP = cep;
                cliente.Complemento = cliente == null ? string.Empty : cliente.Complemento;
                cliente.CPF_CNPJ = cpfCnpj;
                cliente.Endereco = rua;
                if (ie != null && ie != "")
                    cliente.IE_RG = ie;
                else
                {
                    cliente.IE_RG = "ISENTO";
                }
                cliente.InscricaoMunicipal = cliente == null ? string.Empty : cliente.InscricaoMunicipal;
                cliente.Localidade = repLocalidade.BuscarPorCodigoIBGE(int.Parse(cidade));

                string nomeAjustado = Utilidades.String.RemoveAllSpecialCharacters(nome);
                if (string.IsNullOrWhiteSpace(cliente.Nome))
                    cliente.Nome = nomeAjustado;
                else if (nomeAjustado == nome)
                    cliente.Nome = nome;

                cliente.NomeFantasia = nome;
                cliente.Numero = numero;
                cliente.Telefone1 = cliente == null ? string.Empty : cliente.Telefone1;
                cliente.Tipo = Utilidades.String.OnlyNumbers(cnpj).Length == 14 ? "J" : "F";

                if (cliente.Atividade == null)
                    cliente.Atividade = Servicos.Atividade.ObterAtividade(this.EmpresaUsuario.Codigo, cliente.Tipo, Conexao.StringConexao);

                if (cliente.Tipo == "F" && cliente.Atividade.Codigo == 7 && string.IsNullOrWhiteSpace(cliente.IE_RG))
                    cliente.IE_RG = "ISENTO";

                if (inserir)
                {
                    if (cliente.Tipo == "J" && cliente.GrupoPessoas == null)
                    {
                        Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
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

                return cliente;
            }
            else
                return null;
        }

        private void SalvarInformacoesComponentesDaPrestacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico conhecimento, string descricao, decimal valor, bool incluiICMS, bool incluiTotal, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ComponentePrestacaoCTE repComponentePrestacao = new Repositorio.ComponentePrestacaoCTE(unitOfWork);
            Dominio.Entidades.ComponentePrestacaoCTE componente = new Dominio.Entidades.ComponentePrestacaoCTE();

            componente.CTE = conhecimento;
            componente.IncluiNaBaseDeCalculoDoICMS = incluiICMS;
            componente.IncluiNoTotalAReceber = incluiTotal;
            componente.Nome = descricao;
            componente.Valor = valor;

            repComponentePrestacao.Inserir(componente);

        }

        private Dominio.ObjetosDeValor.Cliente ObterDadosDestinatarioExportacao(Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa destinatario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);

            Dominio.ObjetosDeValor.Cliente clienteExportacao = new Dominio.ObjetosDeValor.Cliente();
            clienteExportacao.RazaoSocial = destinatario.RazaoSocial;
            clienteExportacao.NomeFantasia = destinatario.NomeFantasia;
            clienteExportacao.Nome = destinatario.RazaoSocial;
            clienteExportacao.Endereco = destinatario.Endereco.Logradouro;
            clienteExportacao.Bairro = destinatario.Endereco.Bairro;
            clienteExportacao.Cidade = destinatario.Endereco.Cidade.Descricao;
            clienteExportacao.Complemento = destinatario.Endereco.Complemento;
            clienteExportacao.Emails = string.Empty;
            clienteExportacao.Numero = "S/N";
            clienteExportacao.SiglaPais = repPais.BuscarPorCodigo(destinatario.Endereco.Cidade.Pais.CodigoPais).Sigla;
            clienteExportacao.Exportacao = true;

            return clienteExportacao;
        }

        private int TempoMaximoCancelarCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            int limiteConfigurado = 0;

            if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.PrazoCancelamentoCTe > 0)
                limiteConfigurado = this.EmpresaUsuario.Configuracao.PrazoCancelamentoCTe;
            else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && this.EmpresaUsuario.EmpresaPai.Configuracao.PrazoCancelamentoCTe > 0)
                limiteConfigurado = this.EmpresaUsuario.EmpresaPai.Configuracao.PrazoCancelamentoCTe;
            else
                return 0;

            return limiteConfigurado;
        }

        private bool EmitirSemValorDaCarga()
        {
            return this.EmpresaUsuario.Configuracao.EmitirSemValorDaCarga != null ? this.EmpresaUsuario.Configuracao.EmitirSemValorDaCarga : false;
        }

        private object GerarDocumentosListaNOTFIS(int codigoEmpresa, Dominio.ObjetosDeValor.EDI.Notfis.EDINotFis EDI, bool naoInserirNotaDuplicada, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Servicos.Cliente svcCliente = new Servicos.Cliente(Conexao.StringConexao);

            List<Dominio.ObjetosDeValor.XMLNFe> listaNotasRetorno = new List<Dominio.ObjetosDeValor.XMLNFe>();

            foreach (Dominio.ObjetosDeValor.EDI.Notfis.Embarcador embarcador in EDI.CabecalhoDocumento.Embarcadores)
            {
                for (var i = 0; i < embarcador.Destinatarios.Count; i++)
                {
                    for (var k = 0; k < embarcador.Destinatarios[i].NotasFiscais.Count; k++)
                    {
                        embarcador.Pessoa.AtualizarEnderecoPessoa = true;
                        embarcador.Destinatarios[i].Pessoa.AtualizarEnderecoPessoa = true;
                        svcCliente.ConverterObjetoValorPessoa(embarcador.Pessoa, string.Empty, unitOfWork, codigoEmpresa);
                        svcCliente.ConverterObjetoValorPessoa(embarcador.Destinatarios[i].Pessoa, string.Empty, unitOfWork, codigoEmpresa);
                        if (embarcador.Destinatarios[i].NotasFiscais[k].ResponsavelFrete != null)
                            svcCliente.ConverterObjetoValorPessoa(embarcador.Destinatarios[i].NotasFiscais[k].ResponsavelFrete.Pessoa, string.Empty, unitOfWork, codigoEmpresa);

                        bool inserirNota = true;
                        if (naoInserirNotaDuplicada)
                        {
                            //List<Dominio.ObjetosDeValor.XMLNFe> documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.XMLNFe>>(listaNotasRetorno);                            

                            inserirNota = string.IsNullOrWhiteSpace(embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave) || (from obj in listaNotasRetorno where obj.Chave == embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave select obj).Count() == 0;
                        }

                        if (inserirNota)
                        {
                            Dominio.ObjetosDeValor.XMLNFe nota = new Dominio.ObjetosDeValor.XMLNFe()
                            {
                                Chave = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave,
                                ValorTotal = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Valor,
                                DataEmissao = DateTime.Today.ToString("dd/MM/yyyy"),
                                Remetente = embarcador.Destinatarios[i].NotasFiscais[k].ResponsavelFrete != null && embarcador.Destinatarios[i].NotasFiscais[k].ResponsavelFrete.Pessoa != null ? embarcador.Destinatarios[i].NotasFiscais[k].ResponsavelFrete.Pessoa.CPFCNPJ : embarcador.Pessoa.CPFCNPJ,
                                Destinatario = embarcador.Destinatarios[i].Pessoa.CPFCNPJ,
                                Numero = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Numero.ToString(),
                                Peso = embarcador.Destinatarios[i].NotasFiscais[k].NFe.PesoBruto,
                                FormaPagamento = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ModalidadeFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete.Pago ? "0" : "1",
                                Placa = embarcador.Destinatarios[i].NotasFiscais[k].Placa != null ? embarcador.Destinatarios[i].NotasFiscais[k].Placa.Replace(" ", "") : string.Empty,
                                NumeroDosCTesUtilizados = !string.IsNullOrWhiteSpace(embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave) ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave) : repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, embarcador.Destinatarios[i].NotasFiscais[k].NFe.Numero.ToString()), //!string.IsNullOrWhiteSpace(embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave) ? repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, embarcador.Destinatarios[i].NotasFiscais[k].NFe.Chave) : repDocumentosCTe.BuscarNumeroDoCTePorOutrosDocumentosEEmpresa(codigoEmpresa, embarcador.Destinatarios[i].NotasFiscais[k].NFe.Numero.ToString()),
                                Serie = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Serie,
                                Observacao = string.Empty,
                                ObsTransporte = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Observacao,
                                Volume = embarcador.Destinatarios[i].NotasFiscais[k].NFe.VolumesTotal,
                                ValorFrete = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ValorFrete,
                                TipoRaterio = Dominio.Enumeradores.TipoRateioTabelaFreteValor.ValorNotaFiscal,
                                NumeroRomaneio = embarcador.Destinatarios[i].NotasFiscais[k].NumeroRomaneio,
                                NumeroCTeAnterior = embarcador.Destinatarios[i].NotasFiscais[k].CTe != null && embarcador.Destinatarios[i].NotasFiscais[k].CTe.Numero > 0 ? embarcador.Destinatarios[i].NotasFiscais[k].CTe.Numero.ToString() : string.Empty,
                                ChaveCTeAnterior = embarcador.Destinatarios[i].NotasFiscais[k].CTe != null && !string.IsNullOrWhiteSpace(embarcador.Destinatarios[i].NotasFiscais[k].CTe.Chave) ? embarcador.Destinatarios[i].NotasFiscais[k].CTe.Chave : string.Empty,
                                EmissorCTeAnterior = embarcador.Destinatarios[i].NotasFiscais[k].CTe != null && !string.IsNullOrWhiteSpace(embarcador.Destinatarios[i].NotasFiscais[k].CTe.Chave) ? embarcador.Destinatarios[i].NotasFiscais[k].CTe.Chave.Substring(6, 14) : string.Empty,
                                ValorPedagio = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ValorComponentePedagio,
                                ValorDescarga = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ValorComponenteDescarga,
                                ValorAdValorem = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ValorComponenteAdValorem,
                                ValorAdicionalEntrega = embarcador.Destinatarios[i].NotasFiscais[k].NFe.ValorComponenteAdicionalEntrega,
                                Cubagem = embarcador.Destinatarios[i].NotasFiscais[k].NFe.Cubagem
                            };

                            listaNotasRetorno.Add(nota);
                        }
                    }
                }
            }

            return listaNotasRetorno;

        }

        private object GerarDocumentosListaNotas(int codigoEmpresa, List<Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal> notas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Servicos.Cliente svcCliente = new Servicos.Cliente(Conexao.StringConexao);

            List<object> listaNotasRetorno = new List<object>();

            foreach (Dominio.ObjetosDeValor.Embarcador.NFe.NotaFiscal nota in notas)
            {
                nota.Emitente.AtualizarEnderecoPessoa = false;
                nota.Destinatario.AtualizarEnderecoPessoa = false;
                svcCliente.ConverterObjetoValorPessoa(nota.Emitente, string.Empty, unitOfWork, codigoEmpresa);
                svcCliente.ConverterObjetoValorPessoa(nota.Destinatario, string.Empty, unitOfWork, codigoEmpresa);

                var notaImportacao = new
                {
                    Chave = nota.Chave,
                    ValorTotal = nota.Valor,
                    DataEmissao = !string.IsNullOrWhiteSpace(nota.DataEmissao) ? nota.DataEmissao.Length > 10 ? nota.DataEmissao.Substring(0, 10) : nota.DataEmissao : DateTime.Today.ToString("dd/MM/yyyy"),
                    Remetente = nota.Emitente.CPFCNPJ,
                    Destinatario = nota.Destinatario.CPFCNPJ,
                    Numero = nota.Numero,
                    Peso = nota.PesoBruto,
                    FormaPagamento = nota.ModalidadeFrete,
                    Placa = nota.Veiculo != null && !string.IsNullOrWhiteSpace(nota.Veiculo.Placa) ? nota.Veiculo.Placa.Replace(" ", "").Replace("-", "") : string.Empty,
                    NumeroDosCTesUtilizados = !string.IsNullOrWhiteSpace(nota.Chave) ? repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nota.Chave) : repDocumentosCTe.BuscarNumeroStatusDoCTePorChaveEEmpresa(codigoEmpresa, nota.Numero.ToString()),// !string.IsNullOrWhiteSpace(nota.Chave) ? repDocumentosCTe.BuscarNumeroDoCTePorChaveEEmpresa(codigoEmpresa, nota.Chave) : repDocumentosCTe.BuscarNumeroDoCTePorOutrosDocumentosEEmpresa(codigoEmpresa, nota.Numero.ToString()),
                    Serie = nota.Serie,
                    Observacao = string.Empty,
                    Volume = nota.VolumesTotal,
                    ValorFrete = nota.ValorFrete,
                    TipoRaterio = Dominio.Enumeradores.TipoRateioTabelaFreteValor.ValorNotaFiscal,
                    NumeroRomaneio = nota.NumeroRomaneio,
                    BaseCalculoICMS = nota.BaseCalculoICMS,
                    ValorICMS = nota.ValorICMS,
                    BaseCalculoST = nota.BaseCalculoST,
                    ValorST = nota.ValorST,
                    AliquotaICMS = nota.AliquotaICMS,
                    ObservacaoCTe = string.Concat(!string.IsNullOrWhiteSpace(nota.NumeroDT) ? "PRE-CTE: " + nota.NumeroDT + " - " : string.Empty, nota.Observacao),
                    ValorPedagio = nota.ValorComponentePedagio,
                    ValorDescarga = nota.ValorComponenteDescarga,
                    ValorAdValorem = nota.ValorComponenteAdValorem,
                    ValorAdicionalEntrega = nota.ValorComponenteAdicionalEntrega,
                    NumeroDT = nota.NumeroDT,
                    nota.ObsPlaca,
                    nota.ObsTransporte,
                    Cubagem = nota.Cubagem
                };
                listaNotasRetorno.Add(notaImportacao);
            }

            return listaNotasRetorno;

        }

        public static string ExtrairMensagemDeJson(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            int jsonStart = texto.IndexOf('{');
            if (jsonStart < 0)
                return texto;

            string jsonPart = texto.Substring(jsonStart);

            try
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonPart);
                return obj?.message != null ? (string)obj.message : texto;
            }
            catch
            {

            }

            return texto;
        }



        #endregion
    }
}
