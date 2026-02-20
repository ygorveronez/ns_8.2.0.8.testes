using Dominio.ObjetosDeValor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ManifestoEletronicoDeDocumentosFiscaisController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("emissaomdfe.aspx") select obj).FirstOrDefault();
        }

        #endregion

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int serie, numeroInicial, numeroFinal, inicioRegistros, fimRegistros, codigoMDFe = 0;
                int.TryParse(Request.Params["Serie"], out serie);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["NumeroCTe"], out int numeroCTe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);
                int.TryParse(Request.Params["fimRegistros"], out fimRegistros);
                if (!string.IsNullOrWhiteSpace(Request.Params["CodigoMDFe"]))
                    int.TryParse(Servicos.Criptografia.Descriptografar(Request.Params["CodigoMDFe"], "CT3##MULT1@#$S0FTW4R3"), out codigoMDFe);

                if (fimRegistros == 0)
                    fimRegistros = 20;

                DateTime dataEmissaoInicial, dataEmissaoFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.StatusMDFe statusAux;
                Dominio.Enumeradores.StatusMDFe? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusMDFe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string placaVeiculo = Request.Params["Placa"];
                string placaReboque = Request.Params["Reboque"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];
                string nomeUsuario = Request.Params["NomeUsuario"];

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.Consultar(this.EmpresaUsuario.Codigo, codigoMDFe, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), numeroCTe, inicioRegistros, fimRegistros, nomeUsuario);
                int countCTes = repMDFe.ContarConsulta(this.EmpresaUsuario.Codigo, codigoMDFe, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, placaReboque, cpfMotorista, nomeMotorista, this.Usuario.Series.Where(s => s.Tipo == Dominio.Enumeradores.TipoSerie.MDFe).Select(o => o.Codigo).ToArray(), numeroCTe, nomeUsuario);

                List<Dominio.Entidades.VeiculoMDFe> veiculos = repVeiculoMDFe.BuscarPorMDFes((from obj in mdfes select obj.Codigo).ToArray());

                var retorno = from obj in mdfes
                              select new
                              {
                                  obj.Codigo,
                                  obj.Status,
                                  obj.Numero,
                                  Serie = obj.Serie.Numero,
                                  DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                  Placa = (from veic in veiculos where veic.MDFe.Codigo == obj.Codigo select veic.Placa).FirstOrDefault(),
                                  EstadoCarregamento = string.Concat(obj.EstadoCarregamento.Sigla, " - ", obj.EstadoCarregamento.Nome),
                                  EstadoDescarregamento = string.Concat(obj.EstadoDescarregamento.Sigla, " - ", obj.EstadoDescarregamento.Nome),
                                  obj.DescricaoStatus,
                                  MensagemSefaz = (obj.MensagemStatus == null ? (obj.MensagemRetornoSefaz != null ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : string.Empty) : obj.MensagemStatus.MensagemDoErro)
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Número|8", "Série|6", "Emissão|10", "Placa|8", "UF Carga|15", "UF Descarga|15", "Status|10", "Retorno Sefaz|18" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
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
                int numeroCTe = 0;

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                string ufCarregamento = string.Empty;
                string ufDescarregamento = string.Empty;
                string placaVeiculo = string.Empty;
                string nomeMotorista = string.Empty;
                string cpfMotorista = string.Empty;
                string nomeUsuario = string.Empty;

                // Repositorio
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                // Filtros
                DateTime dataEmissaoInicial, dataEmissaoFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissaoFinal);

                Dominio.Enumeradores.StatusMDFe statusAux;
                Dominio.Enumeradores.StatusMDFe? status = null;
                if (Enum.TryParse<Dominio.Enumeradores.StatusMDFe>(Request.Params["Status"], out statusAux))
                    status = statusAux;

                int empresa = 0;
                int.TryParse(Request.Params["Empresa"], out empresa);

                string numeroCarga = Request.Params["NumeroCarga"];
                placaVeiculo = Utilidades.String.OnlyNumbersAndChars(Request.Params["Placa"]);
                int.TryParse(Request.Params["NumeroMDFe"], out int numeroMDFe);
                numeroInicial = numeroMDFe;
                numeroFinal = numeroMDFe;

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.Consultar(empresa, 0, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, series, numeroCTe, inicioRegistros, 50, nomeUsuario, numeroCarga, "", this.EmpresaUsuario.Codigo);
                int countMDFes = repMDFe.ContarConsulta(empresa, 0, this.EmpresaUsuario.TipoAmbiente, serie, numeroInicial, numeroFinal, dataEmissaoInicial, dataEmissaoFinal, status, ufCarregamento, ufDescarregamento, placaVeiculo, string.Empty, cpfMotorista, nomeMotorista, series, numeroCTe, nomeUsuario, numeroCarga, "", this.EmpresaUsuario.Codigo);
                List<Dominio.Entidades.VeiculoMDFe> veiculos = repVeiculoMDFe.BuscarPorMDFes((from obj in mdfes select obj.Codigo).ToArray());

                var retorno = (from obj in mdfes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Status,
                                   Empresa = obj.Empresa.Codigo,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   NomeEmpresa = obj.Empresa.RazaoSocial + " (" + obj.Empresa.CNPJ + ")",
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   Placa = (from veic in veiculos where veic.MDFe.Codigo == obj.Codigo select veic.Placa).FirstOrDefault(),
                                   EstadoCarregamento = string.Concat(obj.EstadoCarregamento.Sigla, " - ", obj.EstadoCarregamento.Nome),
                                   EstadoDescarregamento = string.Concat(obj.EstadoDescarregamento.Sigla, " - ", obj.EstadoDescarregamento.Nome),
                                   obj.DescricaoStatus,
                                   MensagemSefaz = (obj.MensagemStatus == null ? (obj.MensagemRetornoSefaz != null ? System.Web.HttpUtility.HtmlEncode(obj.MensagemRetornoSefaz) : string.Empty) : obj.MensagemStatus.MensagemDoErro)
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Status", "Empresa", "Número|8", "Série|6", "Empresa|10", "Emissão|10", "Placa|8", "UF Carga|15", "UF Descarga|15", "Status|10", "Retorno Sefaz|18" }, countMDFes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
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
                int codigo = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigo);

                Repositorio.MDFeRetornoSefaz repRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeRetornoSefaz> listaRetornosSefaz = repRetornoSefaz.BuscarPorMDFe(codigo);

                var retorno = (from retornoSefaz in listaRetornosSefaz
                               select new
                               {
                                   Codigo = retornoSefaz.Codigo,
                                   Data = retornoSefaz.DataHora.ToString("dd/MM/yyyy HH:mm"),
                                   DescricaoTipo = retornoSefaz.DescricaoTipo,
                                   RetornoSefaz = !string.IsNullOrWhiteSpace(Utilidades.String.ReplaceInvalidCharacters(retornoSefaz.MensagemRetorno)) ? System.Web.HttpUtility.HtmlEncode(Utilidades.String.ReplaceInvalidCharacters(retornoSefaz.MensagemRetorno)) : retornoSefaz.ErroSefaz != null ? retornoSefaz.ErroSefaz.CodigoDoErro.ToString() + " - " + retornoSefaz.ErroSefaz.MensagemDoErro : string.Empty
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
        public ActionResult ConsultarSumarizado()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int numeroInicial, numeroFinal, inicioRegistros = 0;
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.Consultar(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, numeroInicial, numeroFinal, inicioRegistros, 50);
                int countMDFes = repMDFe.ContarConsulta(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, numeroInicial, numeroFinal);

                var retorno = (from obj in mdfes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   EstadoCarregamento = string.Concat(obj.EstadoCarregamento.Sigla, " - ", obj.EstadoCarregamento.Nome),
                                   EstadoDescarregamento = string.Concat(obj.EstadoDescarregamento.Sigla, " - ", obj.EstadoDescarregamento.Nome),
                                   obj.DescricaoStatus
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Número|10", "Série|10", "Emissão|10", "UF Carga|25", "UF Descarga|25", "Status|10" }, countMDFes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarNaoEncerrados()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Usuario.TipoAcesso != Dominio.Enumeradores.TipoAcesso.Admin)
                    return Json<bool>(false, false, "Acesso negado.");

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.ConsultarNaoEncerrados(this.EmpresaUsuario.Codigo, DateTime.Now.AddDays(-25), new List<int>(), new List<double>(), inicioRegistros, 50);
                int countMDFes = repMDFe.ContarConsultaNaoEncerrados(this.EmpresaUsuario.Codigo, DateTime.Now.AddDays(-25));

                var retorno = (from obj in mdfes
                               select new
                               {
                                   obj.Codigo,
                                   Empresa = obj.Empresa.CNPJ + " - " + obj.Empresa.NomeFantasia,
                                   Numero = obj.Numero + " - " + obj.Serie.Numero,
                                   DataAutorizacao = obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   obj.DescricaoStatus,
                                   obj.MensagemRetornoSefaz
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Empresa|20", "Número|10", "Data Autorização|10", "Status|10", "Retorno Sefaz|40" }, countMDFes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarPendentesDeEncerramento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.EmpresaUsuario.Configuracao != null && !this.EmpresaUsuario.Configuracao.ExibirHomeMDFesPendenteEncerramento)
                    return Json<bool>(false, false, "MDFes pendentes de encerramento sem configuração para exibição na pagina inicial.");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.ConsultarPendentesDeEncerramento(this.EmpresaUsuario.Codigo, DateTime.Now.AddDays(-1), this.EmpresaUsuario.TipoAmbiente, 0, 500);

                var retorno = (from obj in mdfes
                               select new
                               {
                                   CodigoCriptografado = Servicos.Criptografia.Criptografar(obj.Codigo.ToString(), "CT3##MULT1@#$S0FTW4R3"),
                                   obj.Codigo,
                                   Empresa = obj.Empresa.CNPJ + " - " + obj.Empresa.NomeFantasia,
                                   Numero = obj.Numero,
                                   Serie = obj.Serie.Numero,
                                   Chave = obj.Chave,
                                   DataAutorizacao = obj.DataAutorizacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   Veiculo = obj.Veiculos != null && obj.Veiculos.Count() > 0 ? obj.Veiculos.FirstOrDefault().Placa : string.Empty,
                                   obj.DescricaoStatus,
                                   obj.MensagemRetornoSefaz
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os MDF-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ConsultarCTesParaEmissao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repCTeMDFe = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                int codigoMunicipio, numeroInicial, numeroFinal, inicioRegistros, codigoCTe = 0;
                int.TryParse(Request.Params["CodigoMunicipio"], out codigoMunicipio);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string ufCarregamento = string.Empty;// Request.Params["UFCarregamento"]; Alterado para permitir selecionar CTes de UF Origens diferentes
                string placaVeiculo = Request.Params["Placa"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];

                bool ctesSemMdfe = false;
                bool ignorarLocalidades = false;
                bool.TryParse(Request.Params["CTesSemMDFe"], out ctesSemMdfe);
                bool.TryParse(Request.Params["IgnorarOrigem"], out ignorarLocalidades);

                List<int> numerosSeries = (from o in this.Usuario.Series where o.Tipo == Dominio.Enumeradores.TipoSerie.CTe && o.Status == "A" select o.Numero).ToList();

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["TipoServico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                Dominio.Enumeradores.TipoCTE? tipoCTe = null;
                Dominio.Enumeradores.TipoCTE tipoCTeAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["TipoCTe"], out tipoCTeAux))
                    tipoCTe = tipoCTeAux;

                if (ignorarLocalidades)
                {
                    ufCarregamento = string.Empty;
                    ufDescarregamento = string.Empty;
                    codigoMunicipio = 0;
                }


                List<Dominio.ObjetosDeValor.ConsultaCTeParaMDFe> ctes = repCTe.ConsultarParaEmissaoDeMDFe(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, codigoMunicipio, ufCarregamento, ufDescarregamento, numeroInicial, numeroFinal, dataInicial, dataFinal, placaVeiculo, nomeMotorista, cpfMotorista, tipoServico, tipoCTe, numerosSeries, inicioRegistros, 50, codigoCTe);
                List<Dominio.ObjetosDeValor.ConsultaCTeParaMDFe> ctesSemMDFe = new List<Dominio.ObjetosDeValor.ConsultaCTeParaMDFe>();

                int countCTes = repCTe.ContarConsultaParaEmissaoDeMDFe(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, codigoMunicipio, ufCarregamento, ufDescarregamento, numeroInicial, numeroFinal, dataInicial, dataFinal, placaVeiculo, nomeMotorista, cpfMotorista, tipoServico, tipoCTe, numerosSeries, codigoCTe);

                if (!ctesSemMdfe)
                    ctesSemMDFe = ctes;
                else
                {
                    for (var i = 0; i < countCTes; i++)
                    {
                        if (i < 50)
                        {
                            if (repCTeMDFe.VerificaCTePendenteMDFe(ctes[i].Codigo))
                                ctesSemMDFe.Add(ctes[i]);
                        }
                    }
                }

                var retorno = (from obj in ctesSemMDFe
                               select new
                               {
                                   obj.Codigo,
                                   obj.UFDescarregamento,
                                   obj.UFCarregamento,
                                   obj.ValorTotalMercadoria,
                                   obj.PesoTotal,
                                   obj.PesoKgTotal,
                                   Numero = string.Concat(obj.Numero, " - ", obj.Serie),
                                   DataEmissao = obj.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   LocalidadeInicioPrestacao = string.Concat(obj.UFInicioPrestacao, " / ", obj.LocalidadeInicioPrestacao),
                                   LocalidadeTerminoPrestacao = string.Concat(obj.UFTerminoPrestacao, " / ", obj.LocalidadeTerminoPrestacao),
                                   ValorFrete = obj.ValorFrete.ToString("n2"),
                                   obj.Averbado
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "UFDescarregamento", "UFCarregamento", "ValorTotalMercadoria", "PesoTotal", "PesoKgTotal", "Número|10", "Data Emissão|16", "Inicio Prestação|21", "Fim Prestação|21", "Valor Frete|10", "Averb.|8" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterTodosOsCTesParaEmissao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMunicipio, numeroInicial, numeroFinal;
                int.TryParse(Request.Params["CodigoMunicipio"], out codigoMunicipio);
                int.TryParse(Request.Params["NumeroInicial"], out numeroInicial);
                int.TryParse(Request.Params["NumeroFinal"], out numeroFinal);

                DateTime dataInicial, dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string ufCarregamento = string.Empty;// Request.Params["UFCarregamento"]; Alterado para permitir selecionar CTes de UF Origens diferentes
                string placaVeiculo = Request.Params["Placa"];
                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Request.Params["CPFMotorista"];

                Dominio.Enumeradores.TipoServico? tipoServico = null;
                Dominio.Enumeradores.TipoServico tipoServicoAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoServico>(Request.Params["TipoServico"], out tipoServicoAux))
                    tipoServico = tipoServicoAux;

                Dominio.Enumeradores.TipoCTE? tipoCTe = null;
                Dominio.Enumeradores.TipoCTE tipoCTeAux;
                if (Enum.TryParse<Dominio.Enumeradores.TipoCTE>(Request.Params["TipoCTe"], out tipoCTeAux))
                    tipoCTe = tipoCTeAux;

                List<int> numerosSeries = (from o in this.Usuario.Series where o.Tipo == Dominio.Enumeradores.TipoSerie.CTe && o.Status == "A" select o.Numero).ToList();

                //if (string.IsNullOrWhiteSpace(ufCarregamento))
                //    return Json<bool>(false, false, "Estado de carregamento inválido para selecionar todos.");

                if (string.IsNullOrWhiteSpace(ufDescarregamento))
                    return Json<bool>(false, false, "Estado de carregamento inválido para selecionar todos.");

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ConsultaCTeParaMDFe> ctes = repCTe.ConsultarParaEmissaoDeMDFe(this.EmpresaUsuario.Codigo, this.EmpresaUsuario.TipoAmbiente, codigoMunicipio, ufCarregamento, ufDescarregamento, numeroInicial, numeroFinal, dataInicial, dataFinal, placaVeiculo, nomeMotorista, cpfMotorista, tipoServico, tipoCTe, numerosSeries, 0, 4000);

                Repositorio.InformacaoCargaCTE repInfoCargaCTe = new Repositorio.InformacaoCargaCTE(unidadeDeTrabalho);

                var retorno = (from obj in ctes
                               select new
                               {
                                   obj.Codigo,
                                   obj.Numero,
                                   ValorFrete = obj.ValorFrete.ToString("n2")
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
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
                int codigo = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigo);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);
                Repositorio.MDFeCIOT repMDFeCIOT = new Repositorio.MDFeCIOT(unidadeDeTrabalho);
                Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento repositorioConfiguracaoIntegracaoEmissorDocumento = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento(unidadeDeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoEmissorDocumento configuracaoIntegracaoEmissorDocumento = repositorioConfiguracaoIntegracaoEmissorDocumento.BuscarConfiguracaoPadrao();

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                if (mdfe != null)
                {
                    List<Dominio.Entidades.MDFeSeguro> listaSeguros = repMDFeSeguro.BuscarPorMDFe(mdfe.Codigo);
                    IList<Dominio.ObjetosDeValor.ContratantesMDFe> listaContratantes = repMDFeContratante.BuscarPorMDFeRelacionado(mdfe.Codigo);
                    List<Dominio.Entidades.MDFeCIOT> listaCIOTs = repMDFeCIOT.BuscarPorMDFe(mdfe.Codigo);

                    Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(unidadeDeTrabalho).BuscarPorMDFe(mdfe.Codigo);
                    List<Dominio.Entidades.MDFePagamentoParcela> listaParcelaMDFe = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(unidadeDeTrabalho).BuscarPorInformacoesBancarias(informacoesBancarias?.Codigo ?? 0);
                    List<Dominio.Entidades.MDFePagamentoComponente> listaPagamentoComponenteMDFe = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(unidadeDeTrabalho).BuscarPorInformacoesBancarias(informacoesBancarias?.Codigo ?? 0);

                    var retorno = new
                    {
                        mdfe.Codigo,
                        mdfe.Chave,
                        mdfe.CIOT,
                        DataEmissao = mdfe.DataEmissao.Value.ToString("dd/MM/yyyy"),
                        HoraEmissao = mdfe.DataEmissao.Value.ToString("HH:mm"),
                        UFCarregamento = mdfe.EstadoCarregamento.Sigla,
                        UFDescarregamento = mdfe.EstadoDescarregamento.Sigla,
                        mdfe.Log,
                        Modal = mdfe.Modal.Codigo,
                        mdfe.Numero,
                        mdfe.ObservacaoContribuinte,
                        mdfe.ObservacaoFisco,
                        mdfe.PesoBrutoMercadoria,
                        mdfe.Protocolo,
                        mdfe.ProtocoloCancelamento,
                        mdfe.ProtocoloEncerramento,
                        mdfe.RNTRC,
                        Serie = mdfe.Serie.Codigo,
                        mdfe.UnidadeMedidaMercadoria,
                        mdfe.ValorTotalMercadoria,
                        mdfe.TipoAmbiente,
                        mdfe.TipoEmissao,
                        mdfe.TipoEmitente,
                        mdfe.Status,
                        mdfe.Versao,
                        TipoCarga = mdfe.TipoCargaMDFe,
                        mdfe.ProdutoPredominanteDescricao,
                        mdfe.ProdutoPredominanteCEAN,
                        mdfe.ProdutoPredominanteNCM,
                        mdfe.CEPCarregamentoLotacao,
                        LatitudeCarregamentoLotacao = mdfe.LatitudeCarregamentoLotacao.HasValue ? mdfe.LatitudeCarregamentoLotacao.Value.ToString("n10") : string.Empty,
                        LongitudeCarregamentoLotacao = mdfe.LongitudeCarregamentoLotacao.HasValue ? mdfe.LongitudeCarregamentoLotacao.Value.ToString("n10") : string.Empty,
                        mdfe.CEPDescarregamentoLotacao,
                        LatitudeDescarregamentoLotacao = mdfe.LatitudeDescarregamentoLotacao.HasValue ? mdfe.LatitudeDescarregamentoLotacao.Value.ToString("n10") : string.Empty,
                        LongitudeDescarregamentoLotacao = mdfe.LongitudeDescarregamentoLotacao.HasValue ? mdfe.LongitudeDescarregamentoLotacao.Value.ToString("n10") : string.Empty,
                        Seguros = (from obj in listaSeguros
                                   select new
                                   {
                                       Id = obj.Codigo,
                                       Seguradora = obj.NomeSeguradora,
                                       obj.NumeroApolice,
                                       obj.NumeroAverbacao,
                                       obj.CNPJSeguradora,
                                       obj.Responsavel,
                                       Tipo = obj.TipoResponsavel,
                                       DescricaoTipo = obj.TipoResponsavel.ToString("G"),
                                   }).ToList(),
                        Contratantes = (from obj in listaContratantes
                                        select new
                                        {
                                            obj.CPF_CNPJ,
                                            Nome = !string.IsNullOrEmpty(obj.Nome) ? obj.Nome : string.Empty,
                                            obj.Id,
                                        }).ToList(),
                        CIOTs = (from obj in listaCIOTs
                                 select new
                                 {
                                     Id = obj.Codigo,
                                     CIOT = obj.NumeroCIOT,
                                     CPF_CNPJ = obj.Responsavel
                                 }).ToList(),
                        DataCancelamento = mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? mdfe.DataCancelamento.ToString() : string.Empty,
                        Justificativa = mdfe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? mdfe.JustificativaCancelamento : string.Empty,

                        // Informações Bancárias no topo
                        TipoPagamento = informacoesBancarias?.TipoPagamento ?? null,
                        ValorAdiantamento = informacoesBancarias?.ValorAdiantamento ?? 0,

                        TipoInformacaoBancaria = informacoesBancarias?.TipoInformacaoBancaria ?? null,
                        Banco = informacoesBancarias?.Conta ?? string.Empty,
                        Agencia = informacoesBancarias?.Agencia ?? string.Empty,
                        ChavePIX = informacoesBancarias?.ChavePIX ?? string.Empty,
                        Ipef = informacoesBancarias?.Ipef ?? string.Empty,

                        ParcelasPagamento = listaParcelaMDFe != null ? (from obj in listaParcelaMDFe
                                                                        select new ParcelaPagamentoCIOT()
                                                                        {
                                                                            Id = obj.Codigo,
                                                                            NumeroParcela = obj.NumeroParcela ?? 1,
                                                                            DataVencimento = obj.DataVencimentoParcela != null && obj.DataVencimentoParcela != DateTime.MinValue ? obj.DataVencimentoParcela.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                                            ValorParcela = $"{obj.ValorParcela ?? 0:F2}",
                                                                        }).ToList() : null,

                        ComponentesPagamento = listaPagamentoComponenteMDFe != null ? (from obj in listaPagamentoComponenteMDFe
                                                                                       select new ComponentePagamentoCIOT()
                                                                                       {
                                                                                           Id = obj.Codigo,
                                                                                           ValorComponente = $"{obj.ValorComponente ?? 0:F2}",
                                                                                           TipoComponente = (int)(obj.TipoComponente ?? Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento.FreteValor),
                                                                                       }).ToList() : null,
                        configuracaoIntegracaoEmissorDocumento.TipoEmissorDocumentoMDFe
                    };

                    return Json(retorno, true);
                }

                return Json<bool>(false, false, "MDF-e não econtrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterCodigoPorNumeroESerie()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int numero, codigoSerie = 0;
                int.TryParse(Request.Params["Numero"], out numero);
                int.TryParse(Request.Params["Serie"], out codigoSerie);

                if (numero == 0 || codigoSerie == 0)
                    return Json<bool>(false, false, "Número ou Série inválido.");

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                Dominio.Entidades.EmpresaSerie serie = repEmpresaSerie.BuscarPorSerie(this.EmpresaUsuario.Codigo, codigoSerie, Dominio.Enumeradores.TipoSerie.MDFe);

                if (serie == null)
                    return Json<bool>(false, false, "Série não cadastrada.");

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorNumeroESerie(this.EmpresaUsuario.Codigo, numero, serie);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrada.");

                var retorno = new
                {
                    mdfe.Codigo,
                    mdfe.Status
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes do MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesCTes()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                List<Dominio.ObjetosDeValor.CTeMDFe> ctesMDFe = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.CTeMDFe>>(Request.Params["CTes"]);
                List<int> codigosCTes = (from obj in ctesMDFe select obj.Codigo).ToList();
                int quantidade = codigosCTes.Count / 1;
                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                for (var i = 0; i < quantidade; i++)
                {
                    ctes.AddRange(repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigosCTes.Skip(i * 1).Take(1).ToArray()));
                }

                Repositorio.VeiculoCTE repVeiculoCTe = new Repositorio.VeiculoCTE(unidadeDeTrabalho);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeDeTrabalho);

                Servicos.CTe svcCTe = new Servicos.CTe(unidadeDeTrabalho);
                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                List<Dominio.Entidades.VeiculoCTE> veiculosCTes = repVeiculoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, ctes.FirstOrDefault().Codigo); // (from obj in ctesMDFe select obj.Codigo).ToArray()
                List<Dominio.Entidades.Veiculo> veiculos = repVeiculo.BuscarPorPlaca(this.EmpresaUsuario.Codigo, (from obj in veiculosCTes select obj.Placa).Distinct().ToArray());
                //List<Dominio.ObjetosDeValor.InformacaoSeguro> listaSeguros = svcCTe.ObterSegurosDistintosPorCTes(ctes);
                List<Dominio.ObjetosDeValor.InformacaoSeguro> listaSeguros = svcCTe.ObterSegurosPorCTes(ctes);

                var listaTomadores = (from obj in ctes
                                      group obj by obj.TomadorPagador.CPF_CNPJ into tomador
                                      select tomador.FirstOrDefault()).ToList();
                if (ctes.FirstOrDefault().TomadorPagador.Exterior)
                {
                    if (ctes.FirstOrDefault().TomadorPagador == ctes.FirstOrDefault().Remetente)
                        listaTomadores = (from obj in ctes
                                          group obj by obj.Destinatario.CPF_CNPJ into tomador
                                          select tomador.FirstOrDefault()).ToList();
                    else
                        listaTomadores = (from obj in ctes
                                          group obj by obj.Remetente.CPF_CNPJ into tomador
                                          select tomador.FirstOrDefault()).ToList();
                }

                var listaCIOTs = (from obj in ctes
                                  where !string.IsNullOrEmpty(obj.CIOT)
                                  group obj by obj.CIOT into g
                                  select new
                                  {
                                      g.FirstOrDefault().CIOT,
                                      g.FirstOrDefault().Empresa.CNPJ,
                                  }).ToList();

                var retorno = new
                {
                    CIOT = (from obj in ctes where !string.IsNullOrWhiteSpace(obj.CIOT) select obj.CIOT).FirstOrDefault(),
                    MunicipiosCarregamento = this.ObterMunicipiosCarregamentoCTes(ctes, (from obj in ctes where obj.Codigo == codigosCTes[0] select obj.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault()),
                    MunicipiosDescarregamento = this.ObterMunicipiosDescarregamentoCTes(ctes, unidadeDeTrabalho),
                    UFCarregamento = (from obj in ctes where obj.Codigo == codigosCTes[0] select obj.LocalidadeInicioPrestacao.Estado.Sigla).FirstOrDefault(),
                    UFDescarregamento = (from obj in ctes select obj.LocalidadeTerminoPrestacao.Estado.Sigla).FirstOrDefault(),
                    Motoristas = this.ObterMotoristasCTes(ctes, unidadeDeTrabalho),
                    Veiculo = this.ObterVeiculoCTes(veiculos),
                    Reboques = this.ObterReboquesCTes(veiculos),
                    Seguros = (from obj in listaSeguros
                               select new
                               {
                                   CNPJSeguradora = !string.IsNullOrEmpty(obj.CNPJSeguradora) ? obj.CNPJSeguradora : string.Empty,
                                   DescricaoResponsavel = !string.IsNullOrEmpty(obj.DescricaoResponsavel) ? obj.DescricaoResponsavel : string.Empty,
                                   CNPJResponsavel = !string.IsNullOrEmpty(obj.CNPJResponsavel) ? obj.CNPJResponsavel : string.Empty,
                                   NumeroApolice = !string.IsNullOrEmpty(obj.NumeroApolice) ? obj.NumeroApolice : string.Empty,
                                   NumeroAverberacao = !string.IsNullOrEmpty(obj.NumeroAverberacao) ? obj.NumeroAverberacao : svcMDFe.BuscarAverbacaoCTe(obj.Id, this.EmpresaUsuario.Codigo, null),
                                   obj.Responsavel,
                                   Seguradora = !string.IsNullOrEmpty(obj.Seguradora) ? obj.Seguradora : string.Empty
                               }).ToList(),
                    Contratantes = (from obj in listaTomadores
                                    select new
                                    {
                                        Nome = !string.IsNullOrEmpty(obj.Tomador.Nome) ? obj.Tomador.Nome : string.Empty,
                                        CPF_CNPJ = !string.IsNullOrEmpty(obj.Tomador.CPF_CNPJ) ? obj.Tomador.CPF_CNPJ : string.Empty
                                    }).ToList(),
                    CIOTs = (from obj in listaCIOTs
                             select new
                             {
                                 CIOT = !string.IsNullOrEmpty(obj.CIOT) ? obj.CIOT : string.Empty,
                                 CNPJ = !string.IsNullOrEmpty(obj.CNPJ) ? obj.CNPJ : string.Empty
                             }).ToList()
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes dos CT-es.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesEncerramento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe != null)
                {
                    Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                    List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipiosDescarregamento = repMunicipioDescarregamento.BuscarPorMDFe(mdfe.Codigo);

                    if (municipiosDescarregamento.Count() > 0)
                    {
                        var retorno = new
                        {
                            SiglaUF = mdfe.EstadoDescarregamento.Sigla,
                            DescricaoUF = mdfe.EstadoDescarregamento.Nome,
                            Municipios = (from obj in municipiosDescarregamento select new { obj.Municipio.Descricao, obj.Municipio.Codigo }).ToList()
                        };

                        return Json(retorno, true);
                    }
                    else
                    {
                        var retorno = new
                        {
                            SiglaUF = municipiosDescarregamento.Count() > 0 ? mdfe.EstadoDescarregamento.Sigla : mdfe.Empresa.Localidade.Estado.Sigla,
                            DescricaoUF = municipiosDescarregamento.Count() > 0 ? mdfe.EstadoDescarregamento.Nome : mdfe.Empresa.Localidade.Estado.Nome,
                            Municipios = new { Descricao = mdfe.Empresa.Localidade.Descricao, Codigo = mdfe.Empresa.Localidade.Codigo }
                        };

                        return Json(retorno, true);
                    }
                }

                return Json<bool>(false, false, "MDF-e não encontrado.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os dados para encerramento.");
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
                var ptBR = CultureInfo.GetCultureInfo("pt-BR");
                int codigo, codigoSerie, codigoModal = 0;
                int tipoPagamento = 0;

                int.TryParse(Request.Params["Codigo"], out codigo);
                int.TryParse(Request.Params["Serie"], out codigoSerie);
                int.TryParse(Request.Params["Modal"], out codigoModal);

                DateTime dataEmissao = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataEmissao"] + " " + Request.Params["HoraEmissao"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEmissao);

                string ufCarregamento = Request.Params["UFCarregamento"];
                string ufDescarregamento = Request.Params["UFDescarregamento"];
                string rntrc = Request.Params["RNTRC"];
                string ciot = Request.Params["CIOT"];
                string observacaoFisco = Request.Params["ObservacaoFisco"];
                string observacaoContribuinte = Request.Params["ObservacaoContribuinte"];
                string produtoPredominanteDescricao = Request.Params["ProdutoPredominanteDescricao"];
                string produtoPredominanteCEAN = Request.Params["ProdutoPredominanteCEAN"];
                string produtoPredominanteNCM = Request.Params["ProdutoPredominanteNCM"];

                string cepCarregamento = Request.Params["CEPCarregamento"];
                decimal.TryParse(Request.Params["LatitudeCarregamento"], out decimal latitudeCarregamento);
                decimal.TryParse(Request.Params["LongitudeCarregamento"], out decimal longitudeCarregamento);

                string cepDescarregamento = Request.Params["CEPDescarregamento"];
                decimal.TryParse(Request.Params["LatitudeDescarregamento"], out decimal latitudeDescarregamento);
                decimal.TryParse(Request.Params["LongitudeDescarregamento"], out decimal longitudeDescarregamento);

                decimal pesoBruto, valorTotalMercadoria = 0m;
                decimal.TryParse(Request.Params["PesoBruto"], out pesoBruto);
                decimal.TryParse(Request.Params["ValorTotalMercadoria"], out valorTotalMercadoria);

                Dominio.Enumeradores.UnidadeMedidaMDFe unidadeMedida;
                Enum.TryParse<Dominio.Enumeradores.UnidadeMedidaMDFe>(Request.Params["UnidadeMedida"], out unidadeMedida);

                Dominio.Enumeradores.TipoEmitenteMDFe tipoEmitente;
                Enum.TryParse<Dominio.Enumeradores.TipoEmitenteMDFe>(Request.Params["TipoEmitente"], out tipoEmitente);

                Dominio.Enumeradores.TipoCargaMDFe tipoCargaMDFe;
                Enum.TryParse<Dominio.Enumeradores.TipoCargaMDFe>(Request.Params["TipoCarga"], out tipoCargaMDFe);

                int.TryParse(Request.Params["TipoPagamento"], out tipoPagamento);

                decimal? valorComponente = VerificarDecimal(Request.Params, "ValorComponente", ptBR);
                decimal? valorAdiantamento = VerificarDecimal(Request.Params, "ValorAdiantamento", ptBR);
                int? tipoInformacaoBancaria = VerificarInt(Request.Params, "TipoInformacaoBancaria");
                int? numeroParcela = VerificarInt(Request.Params, "NumeroParcela");
                DateTime? dataVencimentoParcela = VerificarData(Request.Params, "DataVencimentoParcela", "dd/MM/yyyy", ptBR);
                decimal? valorParcela = VerificarDecimal(Request.Params, "ValorParcela", ptBR);
                string banco = VerificarString(Request.Params, "Banco");
                string agencia = VerificarString(Request.Params, "Agencia");
                string chavePIX = VerificarString(Request.Params, "ChavePIX");
                string ipef = VerificarString(Request.Params, "Ipef");

                bool emitir = false;
                bool.TryParse(Request.Params["Emitir"], out emitir);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = null;

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                if (codigo > 0)
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                        return Json<bool>(false, false, "Permissão para alteração do MDF-e negada!");

                    mdfe = repMDFe.BuscarPorCodigo(codigo, this.EmpresaUsuario.Codigo);

                    if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.AguardandoCompraValePedagio)
                        return Json<bool>(false, false, "O status do MDF-e não permite a alteração do mesmo.");

                    if (this.UsuarioAdministrativo != null)
                        mdfe.Log = string.Concat(mdfe.Log, " / Alterado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        mdfe.Log = string.Concat(mdfe.Log, " / Alterado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                }
                else
                {
                    if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                        return Json<bool>(false, false, "Permissão para inclusão do MDF-e negada!");

                    mdfe = new Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais();
                    mdfe.Empresa = this.EmpresaUsuario;

                    if (this.UsuarioAdministrativo != null)
                        mdfe.Log = string.Concat("Criado por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                    else
                        mdfe.Log = string.Concat("Criado por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                }

                if (ConfigurationManager.AppSettings["TransacaoCTe"] == "Serializable")
                    unidadeDeTrabalho.Start(System.Data.IsolationLevel.Serializable);
                else
                    unidadeDeTrabalho.Start();

                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                Repositorio.ModalTransporte repModal = new Repositorio.ModalTransporte(unidadeDeTrabalho);
                Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                Repositorio.ModeloDocumentoFiscal repModelo = new Repositorio.ModeloDocumentoFiscal(unidadeDeTrabalho);
                Repositorio.VeiculoMDFe repVeiculoMDFe = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias repositorioMDFeInformacoesBancarias = new Repositorio.Embarcador.MDFE.MDFeInformacoesBancarias(unidadeDeTrabalho);

                Dominio.Entidades.EmpresaSerie serieMDFe = repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerie);

                mdfe.CIOT = ciot;
                mdfe.DataEmissao = dataEmissao;
                mdfe.EstadoCarregamento = repEstado.BuscarPorSigla(ufCarregamento);
                mdfe.EstadoDescarregamento = repEstado.BuscarPorSigla(ufDescarregamento);
                mdfe.Modal = repModal.BuscarPorCodigo(codigoModal, false);
                mdfe.UnidadeMedidaMercadoria = unidadeMedida;
                mdfe.ValorTotalMercadoria = valorTotalMercadoria;
                mdfe.PesoBrutoMercadoria = pesoBruto;
                mdfe.ObservacaoContribuinte = observacaoContribuinte;
                mdfe.ObservacaoFisco = observacaoFisco;
                mdfe.RNTRC = rntrc;
                mdfe.Modelo = repModelo.BuscarPorModelo("58");
                mdfe.TipoAmbiente = this.EmpresaUsuario.TipoAmbiente;
                mdfe.TipoEmissao = Dominio.Enumeradores.TipoEmissaoMDFe.Normal;
                mdfe.TipoEmitente = tipoEmitente;
                mdfe.ProdutoPredominanteDescricao = !string.IsNullOrWhiteSpace(produtoPredominanteDescricao) ? produtoPredominanteDescricao : "DIVERSOS";
                mdfe.ProdutoPredominanteCEAN = produtoPredominanteCEAN;
                mdfe.ProdutoPredominanteNCM = produtoPredominanteNCM;
                mdfe.TipoCargaMDFe = tipoCargaMDFe;

                mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(cepCarregamento);
                mdfe.LatitudeCarregamentoLotacao = latitudeCarregamento;
                mdfe.LongitudeCarregamentoLotacao = longitudeCarregamento;

                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(cepDescarregamento);
                mdfe.LatitudeDescarregamentoLotacao = latitudeDescarregamento;
                mdfe.LongitudeDescarregamentoLotacao = longitudeDescarregamento;

                if (mdfe.Codigo <= 0)
                {
                    if (mdfe.Empresa.PrimeiroNumeroMDFe > 0 && repMDFe.ContarPorEmpresa(this.EmpresaUsuario.Codigo) == 0)
                        mdfe.Numero = mdfe.Empresa.PrimeiroNumeroMDFe;
                    else
                        mdfe.Numero = repMDFe.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, codigoSerie, this.EmpresaUsuario.TipoAmbiente) + 1;

                    mdfe.Serie = serieMDFe;
                    mdfe.Versao = this.VersaoMDFe();

                    repMDFe.Inserir(mdfe);
                }
                else
                {
                    if (mdfe.Serie.Codigo != serieMDFe.Codigo)
                    {
                        mdfe.Numero = repMDFe.BuscarUltimoNumero(this.EmpresaUsuario.Codigo, codigoSerie, this.EmpresaUsuario.TipoAmbiente) + 1;
                        mdfe.Serie = serieMDFe;
                    }

                    repMDFe.Atualizar(mdfe);
                }

                Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias = repositorioMDFeInformacoesBancarias.BuscarPorMDFe(mdfe.Codigo);
                informacoesBancarias ??= new Dominio.Entidades.MDFeInformacoesBancarias() { MDFe = mdfe };
                informacoesBancarias.TipoInformacaoBancaria = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe?)tipoInformacaoBancaria;
                informacoesBancarias.ValorAdiantamento = valorAdiantamento;
                informacoesBancarias.TipoPagamento = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento?)tipoPagamento;
                informacoesBancarias.Conta = banco;
                informacoesBancarias.Agencia = agencia;
                informacoesBancarias.ChavePIX = chavePIX;
                informacoesBancarias.Ipef = ipef;

                if (informacoesBancarias.Codigo > 0)
                    repositorioMDFeInformacoesBancarias.Atualizar(informacoesBancarias);
                else
                    repositorioMDFeInformacoesBancarias.Inserir(informacoesBancarias);

                SalvarComponentesPagamento(informacoesBancarias, unidadeDeTrabalho);
                SalvarParcelasPagamento(informacoesBancarias, unidadeDeTrabalho);
                this.SalvarLacres(mdfe, unidadeDeTrabalho);
                this.SalvarMunicipiosCarregamento(ref mdfe, unidadeDeTrabalho);
                this.SalvarMunicipiosDescarregamento(ref mdfe, unidadeDeTrabalho);
                this.SalvarPercursos(mdfe, unidadeDeTrabalho);
                this.SalvarVeiculo(mdfe, unidadeDeTrabalho);
                this.SalvarReboques(mdfe, unidadeDeTrabalho);
                this.SalvarMotoristas(mdfe, unidadeDeTrabalho);
                this.SalvarContratantes(mdfe, unidadeDeTrabalho);
                this.SalvarCIOTs(mdfe, unidadeDeTrabalho);
                this.SalvarSeguros(mdfe, unidadeDeTrabalho);
                this.SalvarValesPedagio(mdfe, unidadeDeTrabalho);
                this.SalvarPercursoEntreEstados(mdfe, unidadeDeTrabalho);
                if (codigo == 0) //Apenas quando esta inserindo
                    svcMDFe.GerarObservacoesVeiculos(mdfe, unidadeDeTrabalho);

                Dominio.Enumeradores.StatusMDFe statusAnterior = mdfe.Status;

                unidadeDeTrabalho.CommitChanges();

                if (emitir)
                {
                    Dominio.Entidades.VeiculoMDFe veiculoMDFe = repVeiculoMDFe.BuscarPorMDFe(mdfe.Codigo);

                    if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca && veiculoMDFe != null)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repMDFe.BuscarPorPlacaEStatus(this.EmpresaUsuario.CNPJ, veiculoMDFe.Placa, Dominio.Enumeradores.StatusMDFe.Autorizado);

                        if (mdfeAnterior != null)
                            // return Json((mdfe == null ? null : new { Codigo = mdfe.Codigo }), false, "Existe o MDF-e " + mdfeAnterior.Chave + " autorizado com a placa " + veiculoMDFe.Placa + ". É necessário encerra-lo para emitir novo MDF-e.");
                            return Json((mdfe == null ? null : new { Codigo = mdfe.Codigo }), false, "Existe o MDF-e " + mdfeAnterior.Numero + "/" + mdfeAnterior.Serie.Numero + " Emissor " + mdfeAnterior.Empresa.CNPJ + " Chave " + mdfeAnterior.Chave + " autorizado com a placa " + veiculoMDFe.Placa + ". É necessário encerra-lo para emitir novo MDF-e.");
                    }

                    if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.AguardarAverbacaoCTeParaEmitirMDFe)
                    {
                        Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                        List<int> listaCTEs = repDocumento.BuscarCodigosCTesPorMDFe(mdfe.Codigo);

                        Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                        for (var i = 0; i < listaCTEs.Count; i++)
                        {
                            var codigoCTe = listaCTEs[i];
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

                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmDigitacao; //Dominio.Enumeradores.StatusMDFe.Pendente

                    svcMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);

                    if (svcMDFe.Emitir(mdfe, unidadeDeTrabalho))
                    {
                        if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                        if (statusAnterior == Dominio.Enumeradores.StatusMDFe.Rejeicao)
                            svcMDFe.RemoverPendenciaMDFeCarga(mdfe, Auditado, unidadeDeTrabalho);
                    }
                    else
                        return Json((mdfe == null ? null : new { Codigo = mdfe.Codigo }), false, "O MDF-e foi salvo, porém, ocorreram problemas ao enviar para o sefaz.");

                }

                return Json(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar os dados do MDF-e.");
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

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Enviado)
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(5, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    return Json<bool>(true, true, "Aguarde, MDFe-e será consultado.");
                }

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca && mdfe.Veiculos != null && mdfe.Veiculos.Count() > 0)
                {
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfeAnterior = repMDFe.BuscarPorPlacaEStatus(this.EmpresaUsuario.CNPJ, mdfe.Veiculos.FirstOrDefault().Placa, Dominio.Enumeradores.StatusMDFe.Autorizado);

                    if (mdfeAnterior != null)
                        return Json<bool>(false, false, "Existe o MDF-e " + mdfeAnterior.Chave + " autorizado com a placa " + mdfe.Veiculos.FirstOrDefault().Placa + ". É necessário encerra-lo para emitir novo MDF-e.");
                }

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                    return Json<bool>(false, false, "O status do MDF-e não permite a emissão do mesmo.");

                if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.AguardarAverbacaoCTeParaEmitirMDFe)
                {
                    Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);

                    List<int> listaCTEs = repDocumento.BuscarCodigosCTesPorMDFe(mdfe.Codigo);

                    Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unidadeDeTrabalho);
                    for (var i = 0; i < listaCTEs.Count; i++)
                    {
                        var codigoCTe = listaCTEs[i];
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
                    mdfe.Log = string.Concat(mdfe.Log, " / Emitido por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                else
                    mdfe.Log = string.Concat(mdfe.Log, " / Emitido por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                mdfe.Versao = string.IsNullOrWhiteSpace(Request.Params["Versao"]) ? this.VersaoMDFe() : Request.Params["Versao"];
                repMDFe.Atualizar(mdfe);

                svcMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);

                if (svcMDFe.Emitir(mdfe, unidadeDeTrabalho))
                {
                    if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["WebServiceConsultaCTe"]))
                        svcMDFe.AdicionarMDFeNaFilaDeConsulta(mdfe, unidadeDeTrabalho);
                    else if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    svcMDFe.RemoverPendenciaMDFeCarga(mdfe, Auditado, unidadeDeTrabalho);

                    return Json<bool>(true, true);
                }

                return Json<bool>(false, false, "O MDF-e foi salvo, porém, ocorreram problemas ao enviar para o sefaz.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao emitir o MDF-e.");
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
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                string justificativa = Request.Params["Justificativa"];
                string cobrarCancelamento = Request.Params["CobrarCancelamento"];

                DateTime dataCancelamento;
                DateTime.TryParseExact(Request.Params["DataCancelamento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataCancelamento);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento)
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    return Json<bool>(true, true, "Aguarde, Consultando Cancelamento do MDF-e.");
                }

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return Json<bool>(false, false, "O status do MDF-e não permite o cancelamento do mesmo.");

                int limiteCancelamentoMDFe = this.TempoMaximoCancelarMDFe(mdfe);
                bool permiteCancelamento = (DateTime.Now - mdfe.DataAutorizacao.Value).TotalHours < limiteCancelamentoMDFe;

                if (limiteCancelamentoMDFe > 0 && !permiteCancelamento)
                    return Json<bool>(false, false, "O MDF-e só pode ser cancelado até " + limiteCancelamentoMDFe.ToString() + " hora" + (limiteCancelamentoMDFe > 1 ? "s" : "") + " após a emissão.");

                if (justificativa.Length <= 20 || justificativa.Length >= 255)
                    return Json<bool>(false, false, "A justificativa deve conter no mínimo 20 e no máximo 255 caracteres.");

                if (this.UsuarioAdministrativo != null)
                    mdfe.Log += string.Concat(" / Tentativa de cancelamento por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                else
                    mdfe.Log += string.Concat(" / Tentativa de cancelamento por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                mdfe.JustificativaCancelamento = justificativa;
                repMDFe.Atualizar(mdfe);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).CancelarMdfe(mdfe.Codigo, this.EmpresaUsuario.Codigo, justificativa, unidadeDeTrabalho, dataCancelamento, cobrarCancelamento))
                {
                    svcMDFe.AtualizarIntegracaoRetornoMDFe(mdfe, unidadeDeTrabalho);

                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                    return Json(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Não foi possível cancelar o MDF-e.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao cancelar o MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Encerrar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe, codigoMunicipio = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);
                int.TryParse(Request.Params["CodigoMunicipio"], out codigoMunicipio);

                DateTime dataEncerramento, dataEvento;
                DateTime.TryParseExact(Request.Params["DataEncerramento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEncerramento);
                DateTime.TryParseExact(Request.Params["DataEvento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEvento);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);
                Dominio.Entidades.Localidade municipioEncerramento = repLocalidade.BuscarPorCodigo(codigoMunicipio);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (municipioEncerramento == null)
                    return Json<bool>(false, false, "Município de encerramento não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    return Json<bool>(true, true, "Aguarde, Consultando Encerramento do MDF-e.");
                }

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return Json<bool>(false, false, "O status do MDF-e não permite o encerramento do mesmo.");

                mdfe.MunicipioEncerramento = municipioEncerramento;

                string logEncerramento = string.Empty;
                if (this.UsuarioAdministrativo != null)
                    logEncerramento = string.Concat(" / Tentativa de encerramento por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                else
                    logEncerramento = string.Concat(" / Tentativa de encerramento por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                mdfe.Log += logEncerramento;

                repMDFe.Atualizar(mdfe);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, this.EmpresaUsuario.Codigo, dataEncerramento, unidadeDeTrabalho, dataEvento))
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                    svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, logEncerramento, unidadeDeTrabalho);
                    return Json(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Não foi possível encerrar o MDF-e.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult EncerrarAdmin()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, 0);
                Dominio.Entidades.Localidade municipioEncerramento = null;

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                municipioEncerramento = mdfe.Empresa.Localidade;

                if (municipioEncerramento == null)
                    return Json<bool>(false, false, "Município de encerramento não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento)
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    return Json<bool>(true, true, "Aguarde, Consultando Encerramento do MDF-e.");
                }

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return Json<bool>(false, false, "O status do MDF-e não permite o encerramento do mesmo.");

                mdfe.MunicipioEncerramento = municipioEncerramento;

                string logEncerramento = string.Empty;
                if (this.UsuarioAdministrativo != null)
                    logEncerramento = string.Concat(" / Tentativa de encerramento por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                else
                    logEncerramento = string.Concat(" / Tentativa de encerramento por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                mdfe.Log += logEncerramento;

                repMDFe.Atualizar(mdfe);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                DateTime dataEncerramento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                if (fusoHorarioEmpresa.BaseUtcOffset != TimeZoneInfo.Local.BaseUtcOffset)
                    dataEncerramento = TimeZoneInfo.ConvertTime(dataEncerramento, TimeZoneInfo.Local, fusoHorarioEmpresa);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).EncerrarMdfe(mdfe.Codigo, mdfe.Empresa.Codigo, dataEncerramento, unidadeDeTrabalho, dataEncerramento))
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                    svcMDFe.SalvarLogEncerramentoMDFe(mdfe.Chave, mdfe.Protocolo, dataEncerramento, mdfe.Empresa, mdfe.Empresa.Localidade, logEncerramento, unidadeDeTrabalho);
                    return Json(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Não foi possível encerrar o MDF-e.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult IncluirMotorista()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                string nomeMotorista = Request.Params["NomeMotorista"];
                string cpfMotorista = Utilidades.String.OnlyNumbers(Request.Params["CPFMotorista"]);

                DateTime dataEvento;
                DateTime.TryParseExact(Request.Params["DataEvento"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataEvento);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EventoInclusaoMotoristaEnviado)
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                    return Json<bool>(true, true, "Aguarde, Consultando Encerramento do MDF-e.");
                }

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
                    return Json<bool>(false, false, "O status do MDF-e não permite incluir motorista.");

                if (!Utilidades.Validate.ValidarCPF(cpfMotorista))
                    return Json<bool>(false, false, "CPF inválido.");

                if (string.IsNullOrWhiteSpace(nomeMotorista) || nomeMotorista.Length <= 2)
                    return Json<bool>(false, false, "Nome do motorista inválido.");

                Dominio.Entidades.MotoristaMDFe motoristaMDFe = repMotoristaMDFe.BuscarPorCpf(mdfe.Codigo, cpfMotorista, Dominio.Enumeradores.TipoMotoristaMDFe.Normal);
                if (motoristaMDFe != null)
                    return Json<bool>(false, false, "Motorista já existente no MDFe.");

                string logEncerramento = string.Empty;
                if (this.UsuarioAdministrativo != null)
                    logEncerramento = string.Concat(" / Tentativa de inclusão de motorista por ", this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");
                else
                    logEncerramento = string.Concat(" / Tentativa de inclusão de motorista por ", this.Usuario.CPF, " - ", this.Usuario.Nome, " em ", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ".");

                mdfe.Log += logEncerramento;

                motoristaMDFe = new Dominio.Entidades.MotoristaMDFe();
                motoristaMDFe.CPF = cpfMotorista;
                motoristaMDFe.Nome = nomeMotorista.Length > 60 ? nomeMotorista.Substring(0, 60) : nomeMotorista;
                motoristaMDFe.MDFe = mdfe;
                motoristaMDFe.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.SolicitadoEventoInclusao;
                repMotoristaMDFe.Inserir(motoristaMDFe);

                repMDFe.Atualizar(mdfe);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).IncluirMotorista(mdfe.Codigo, this.EmpresaUsuario.Codigo, motoristaMDFe.CPF, motoristaMDFe.Nome, unidadeDeTrabalho, dataEvento))
                {
                    if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                        FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);
                    return Json(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Não foi possível incluir motorista no MDF-e.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao incluir motorista no MDF-e.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadDAMDFE()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                bool contingencia = false;
                bool.TryParse(Request.Params["Contingencia"], out contingencia);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado &&
                    !contingencia)
                    return Json<bool>(false, false, "O MDF-e deve estar autorizado para o download do DAMDFE.");

                if (contingencia && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Rejeicao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmDigitacao && mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmitidoContingencia)
                    return Json<bool>(false, false, "O MDF-e não pode ter sido autorizado para baixar o DAMDFE em contingência.");

                if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoRelatorios"]))
                    return Json<bool>(false, false, "O caminho para os download da DAMDFE não está disponível. Contate o suporte técnico.");

                string caminhoPDF = string.Empty;

                if (!contingencia)
                    caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoRelatorios"], mdfe.Empresa.CNPJ, mdfe.Chave) + ".pdf";
                else
                {
                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                    if (Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(mdfe.SistemaEmissor).SolicitarEmissaoContingencia(mdfe.Codigo, unidadeDeTrabalho))
                    {
                        mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmitidoContingencia;
                        repMDFe.Atualizar(mdfe);

                        string obsContingencia = this.UsuarioAdministrativo != null ? string.Concat(this.UsuarioAdministrativo.CPF, " - ", this.UsuarioAdministrativo.Nome) : string.Concat(this.Usuario.CPF, " - ", this.Usuario.Nome);
                        svcMDFe.SalvarRetornoSefaz(mdfe, "O", 0, 0, string.Concat("MDFe emitido em contingência pelo usuário ", obsContingencia), unidadeDeTrabalho);

                        if (mdfe.SistemaEmissor == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.Integrador)
                            FilaConsultaCTe.GetInstance().QueueItem(2, mdfe.Codigo, Dominio.Enumeradores.TipoObjetoConsulta.MDFe, Conexao.StringConexao);

                        caminhoPDF = Utilidades.IO.FileStorageService.Storage.Combine(ConfigurationManager.AppSettings["CaminhoRelatorios"], mdfe.Empresa.CNPJ, mdfe.Numero.ToString()) + "_CONTINGENCIA_.pdf";
                    }
                    else
                        return Json<bool>(false, false, "Não foi possível solicitar a emissão em Contingência. Contate o suporte técnico.");
                }

                byte[] arquivo = null;

                if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoPDF))
                {
                    if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["CaminhoGeradorRelatorios"]))
                        return Json<bool>(false, false, "O gerador da DAMDFE não está disponível. Contate o suporte técnico.");

                    Servicos.DAMDFE svcDAMDFE = new Servicos.DAMDFE(unidadeDeTrabalho);

                    arquivo = svcDAMDFE.Gerar(mdfe.Codigo, contingencia);
                }
                else
                {
                    arquivo = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoPDF);
                }

                if (arquivo != null)
                {
                    if (!contingencia)
                        return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Chave, ".pdf"));
                    else
                        return Arquivo(arquivo, "application/pdf", string.Concat(mdfe.Numero, "_CONTINGENCIA_.pdf"));
                }
                else
                    return Json<bool>(false, false, "Não foi possível gerar o DAMDFE, atualize a página e tente novamente.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do DAMDFE.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLAutorizacao()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, 0);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmCancelamento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.EmEncerramento &&
                    mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "O MDF-e deve estar autorizado para o download do XML de autorização.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLAutorizacao(mdfe, unidadeDeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, ".xml"));
                else
                    return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML de autorização.");
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
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Cancelado)
                    return Json<bool>(false, false, "O MDF-e deve estar cancelado para o download do XML de cancelamento.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLCancelamento(mdfe, unidadeDeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, "-evCancMDFe.xml"));
                else
                    return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML de cancelamento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLEncerramento()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "O MDF-e deve estar encerrado para o download do XML de encerramento.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe();

                byte[] arquivo = svcMDFe.ObterXMLEncerramento(mdfe, unidadeDeTrabalho);

                if (arquivo != null)
                    return Arquivo(arquivo, "text/xml", string.Concat(mdfe.Chave, "-evEncMDFe.xml"));
                else
                    return Json<bool>(false, false, "XML não encontrado, atualize a página e tente novamente.");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao realizar o download do XML de encerramento.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadXMLInclusaoMotorista()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);
                Repositorio.MDFeRetornoSefaz repMDFeRetornoSefaz = new Repositorio.MDFeRetornoSefaz(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "O MDF-e deve estar encerrado para o download do XML de Inclusão de Motorista.");

                if (!repMDFeRetornoSefaz.ExisteEventoInclusaoMotorista(mdfe.Codigo))
                    return Json<bool>(false, false, "O MDF-e deve não possui evento de inclusão de motorista autorizado.");

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeDeTrabalho);

                return Arquivo(svcMDFe.ObterXMLInclusaoMotorista(mdfe, unidadeDeTrabalho), "application/zip", "LoteXML_evIncCondutorMDFe.zip");
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
        public ActionResult DownloadEDIFiscal()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe, lacre = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);
                int.TryParse(Request.Params["Lacre"], out lacre);

                Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe, this.EmpresaUsuario.Codigo);

                if (mdfe == null)
                    return Json<bool>(false, false, "MDF-e não encontrado.");

                if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Encerrado)
                    return Json<bool>(false, false, "O MDF-e deve estar autorizado/encerrado para a geração do EDI Fiscal.");

                if (mdfe.Lacres.Count <= 0 && lacre == 0)
                    return Json<bool>(false, false, "O MDF-e deve possuir ao menos um lacre para a geração do EDI Fiscal.");

                if (mdfe.Lacres.Count <= 0 && lacre >= 0)
                {
                    Repositorio.LacreMDFe repLacreMDFe = new Repositorio.LacreMDFe(unidadeDeTrabalho);
                    Dominio.Entidades.LacreMDFe lacreMDFe = new Dominio.Entidades.LacreMDFe();
                    lacreMDFe.MDFe = mdfe;
                    lacreMDFe.Numero = lacre.ToString();
                    repLacreMDFe.Inserir(lacreMDFe);

                    mdfe.Lacres.Add(lacreMDFe);
                }

                Dominio.Entidades.LayoutEDI layout = this.EmpresaUsuario.LayoutsEDI.Where(o => o.Tipo == Dominio.Enumeradores.TipoLayoutEDI.FISCAL).FirstOrDefault();

                if (layout == null)
                    return Json<bool>(false, false, "Layout do EDI Fiscal não encontrado.");

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unidadeDeTrabalho, new Dominio.ObjetosDeValor.MDFe.EDIMDFe(mdfe), layout);

                MemoryStream arquivo = svcEDI.GerarArquivoMDFe();

                return Arquivo(arquivo, "text/plain", string.Concat(mdfe.Chave, " - EDI Fiscal.txt"));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao gerar EDI Fiscal.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDadosXMLNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            string fileName = string.Empty;

            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        var nfe = MultiSoftware.NFe.Servicos.Leitura.Ler(file.InputStream);

                        if (nfe.GetType() == typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc))
                            return Json(this.ObterDadosDaNotaParaMDFe((MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork), true);
                        if (nfe.GetType() == typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc))
                            return Json(this.ObterDadosDaNotaParaMDFe((MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork), true);
                        else if (nfe.GetType() == typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc))
                            return Json(this.ObterDadosDaNotaParaMDFe((MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)nfe, unitOfWork), true);
                        else
                            return Json(false, false, "A versão da NF-e não é suportada ou a NF-e não está processada.");
                    }
                    else
                    {
                        return Json<bool>(false, false, "A extensão do arquivo " + file.FileName + " é inválida. Somente a extensão XML é aceita.");
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

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao ler o arquivo xml.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult VerificarSeJaUtilizouNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string chaveNFe = Request.Params["ChaveNFe"];
                if (string.IsNullOrWhiteSpace(chaveNFe) || chaveNFe.Length != 44)
                    return Json<bool>(false, false, "Chave da NF-e inválida.");
                int codigoMunicipioDescarregamento;
                int.TryParse(Request.Params["CodigoMunicipioDescarregamento"], out codigoMunicipioDescarregamento);

                Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
                if (!serDocumento.ValidarChave(chaveNFe))
                    return Json<bool>(false, false, "Chave da NF-e informada está inválida.");

                Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);

                var retorno = new
                {
                    MDFes = repNFeMDFe.ObterNumerosMDFesPorChaveNFe(this.EmpresaUsuario.Codigo, chaveNFe, codigoMunicipioDescarregamento)
                };

                return Json(retorno, true);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha genérica ao buscar MDFe NFe.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }



        [AcceptVerbs("POST")]
        public ActionResult IntegrareMDFeOracle()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigooEmpresa = 0;
                int.TryParse(Request.Params["CodigoEmpresa"], out codigooEmpresa);

                DateTime dataInicio, dataFim;
                DateTime.TryParseExact(Request.Params["DataInicio"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicio);
                DateTime.TryParseExact(Request.Params["DataFim"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFim);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                //List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> listaMDFes = repMDFe.BuscarPorStatusEPeriodo(codigooEmpresa, Dominio.Enumeradores.StatusMDFe.Autorizado, dataInicio, dataFim);
                List<int> listaMDFes = repMDFe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, Dominio.Enumeradores.StatusMDFe.Autorizado, dataInicio, dataFim);

                if (listaMDFes.Count > 0)
                {
                    for (var i = 0; i < listaMDFes.Count(); i++)
                    {
                        svcMDFe.IntegrareMDFeOracleAsync(null, listaMDFes[i]).GetAwaiter().GetResult();

                        unitOfWork.FlushAndClear();
                    }
                }

                listaMDFes = repMDFe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, Dominio.Enumeradores.StatusMDFe.Encerrado, dataInicio, dataFim);

                if (listaMDFes.Count > 0)
                {
                    for (var i = 0; i < listaMDFes.Count(); i++)
                    {
                        svcMDFe.IntegrareMDFeOracleAsync(null, listaMDFes[i]).GetAwaiter().GetResult();

                        unitOfWork.FlushAndClear();
                    }
                }

                listaMDFes = repMDFe.BuscarCodigosPorStatusEPeriodo(codigooEmpresa, Dominio.Enumeradores.StatusMDFe.Cancelado, dataInicio, dataFim);

                if (listaMDFes.Count > 0)
                {
                    for (var i = 0; i < listaMDFes.Count(); i++)
                    {
                        svcMDFe.IntegrareMDFeOracleAsync(null, listaMDFes[i]).GetAwaiter().GetResult();

                        unitOfWork.FlushAndClear();
                    }
                }

                return Json<bool>(true, true, "MDFes enviados com sucesso.");

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao enviar MDFes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult GerarMDFeAnterior()
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (System.IO.Path.GetExtension(file.FileName).ToLower().Equals(".xml"))
                    {
                        string path = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoUploadXMLNotasFiscais"], this.EmpresaUsuario.Codigo.ToString());

                        path = Utilidades.IO.FileStorageService.Storage.Combine(path, "MDFe Anterior");

                        Utilidades.IO.FileStorageService.Storage.SaveStream(Utilidades.IO.FileStorageService.Storage.Combine(path, file.FileName), file.InputStream);

                        Servicos.MDFe svcMDFe = new Servicos.MDFe();

                        object retorno = svcMDFe.GerarMDFeAnteriorAsync(file.InputStream, this.EmpresaUsuario.Codigo);

                        if (retorno != null)
                        {
                            if (retorno.GetType() == typeof(string))
                            {
                                return Json<bool>(false, false, (string)retorno);
                            }
                            else if (retorno.GetType() == typeof(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais))
                            {
                                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = (Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais)retorno;
                                return Json(new { mdfe.Numero, Serie = mdfe.Serie.Numero, mdfe.Chave }, true);
                            }
                            else
                            {
                                return Json<bool>(false, false, "MDFe inválido.");
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
        }

        #endregion

        #region Métodos Privados

        private object ObterDadosDaNotaParaMDFe(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

            Dominio.Entidades.Localidade localidadeCarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.emit.enderEmit.cMun));
            Dominio.Entidades.Localidade localidadeDescarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.dest.enderDest.cMun));

            var retorno = new
            {
                MDFes = repNFeMDFe.ObterNumerosMDFesPorChaveNFe(this.EmpresaUsuario.Codigo, nfe.protNFe.infProt.chNFe),
                Chave = nfe.protNFe.infProt.chNFe,
                PesoBrutoMercadoria = svcNFe.ObterPeso(nfe.NFe.infNFe.transp),
                ValorTotalMercadoria = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                CodigoMunicipioCarregamento = localidadeCarregamento.Codigo,
                UFCarregamento = localidadeCarregamento.Estado.Sigla,
                CodigoIBGEMunicipioCarregamento = localidadeCarregamento.CodigoIBGE,
                DescricaoMunicipioCarregamento = localidadeCarregamento.Descricao,
                CodigoMunicipioDescarregamento = localidadeDescarregamento.Codigo,
                UFDescarregamento = localidadeDescarregamento.Estado.Sigla,
                CodigoIBGEMunicipioDescarregamento = localidadeDescarregamento.CodigoIBGE,
                DescricaoMunicipioDescarregamento = localidadeDescarregamento.Descricao,
                PlacaVeiculo = nfe.NFe.infNFe.transp != null && nfe.NFe.infNFe.transp.Items != null ? (from obj in nfe.NFe.infNFe.transp.Items where obj.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TVeiculo) select ((MultiSoftware.NFe.NotaFiscal.TVeiculo)obj).placa).FirstOrDefault() : string.Empty
            };

            return retorno;
        }

        private object ObterDadosDaNotaParaMDFe(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

            Dominio.Entidades.Localidade localidadeCarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.emit.enderEmit.cMun));
            Dominio.Entidades.Localidade localidadeDescarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.dest.enderDest.cMun));

            var retorno = new
            {
                MDFes = repNFeMDFe.ObterNumerosMDFesPorChaveNFe(this.EmpresaUsuario.Codigo, nfe.protNFe.infProt.chNFe),
                Chave = nfe.protNFe.infProt.chNFe,
                PesoBrutoMercadoria = svcNFe.ObterPeso(nfe.NFe.infNFe.transp, unitOfWork),
                ValorTotalMercadoria = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                CodigoMunicipioCarregamento = localidadeCarregamento.Codigo,
                UFCarregamento = localidadeCarregamento.Estado.Sigla,
                CodigoIBGEMunicipioCarregamento = localidadeCarregamento.CodigoIBGE,
                DescricaoMunicipioCarregamento = localidadeCarregamento.Descricao,
                CodigoMunicipioDescarregamento = localidadeDescarregamento.Codigo,
                UFDescarregamento = localidadeDescarregamento.Estado.Sigla,
                CodigoIBGEMunicipioDescarregamento = localidadeDescarregamento.CodigoIBGE,
                DescricaoMunicipioDescarregamento = localidadeDescarregamento.Descricao,
                PlacaVeiculo = nfe.NFe.infNFe.transp != null && nfe.NFe.infNFe.transp.Items != null ? (from obj in nfe.NFe.infNFe.transp.Items where obj.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TVeiculo) select ((MultiSoftware.NFe.NotaFiscal.TVeiculo)obj).placa).FirstOrDefault() : string.Empty
            };

            return retorno;
        }

        private object ObterDadosDaNotaParaMDFe(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc nfe, Repositorio.UnitOfWork unitOfWork)
        {
            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.NotaFiscalEletronicaMDFe repNFeMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
            Servicos.NFe svcNFe = new Servicos.NFe(unitOfWork);

            Dominio.Entidades.Localidade localidadeCarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.emit.enderEmit.cMun));
            Dominio.Entidades.Localidade localidadeDescarregamento = repLocalidade.BuscarPorCodigoIBGE(int.Parse(nfe.NFe.infNFe.dest.enderDest.cMun));

            var retorno = new
            {
                MDFes = repNFeMDFe.ObterNumerosMDFesPorChaveNFe(this.EmpresaUsuario.Codigo, nfe.protNFe.infProt.chNFe),
                Chave = nfe.protNFe.infProt.chNFe,
                PesoBrutoMercadoria = svcNFe.ObterPeso(nfe.NFe.infNFe.transp),
                ValorTotalMercadoria = decimal.Parse(nfe.NFe.infNFe.total.ICMSTot.vNF, cultura),
                CodigoMunicipioCarregamento = localidadeCarregamento.Codigo,
                UFCarregamento = localidadeCarregamento.Estado.Sigla,
                CodigoIBGEMunicipioCarregamento = localidadeCarregamento.CodigoIBGE,
                DescricaoMunicipioCarregamento = localidadeCarregamento.Descricao,
                CodigoMunicipioDescarregamento = localidadeDescarregamento.Codigo,
                UFDescarregamento = localidadeDescarregamento.Estado.Sigla,
                CodigoIBGEMunicipioDescarregamento = localidadeDescarregamento.CodigoIBGE,
                DescricaoMunicipioDescarregamento = localidadeDescarregamento.Descricao,
                PlacaVeiculo = nfe.NFe.infNFe.transp != null && nfe.NFe.infNFe.transp.Items != null ? (from obj in nfe.NFe.infNFe.transp.Items where obj.GetType() == typeof(MultiSoftware.NFe.NotaFiscal.TVeiculo) select ((MultiSoftware.NFe.NotaFiscal.TVeiculo)obj).placa).FirstOrDefault() : string.Empty
            };

            return retorno;
        }

        private List<Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe> ObterMunicipiosCarregamentoCTes(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, string siglaEstado)
        {
            List<Dominio.Entidades.Localidade> listaLocalidadesInicioPrestacao = (from obj in ctes select obj.LocalidadeInicioPrestacao).Distinct().ToList();

            List<Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe> municipiosCarregamento = new List<Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe>();

            for (var i = 0; i < listaLocalidadesInicioPrestacao.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(siglaEstado) || listaLocalidadesInicioPrestacao[i].Estado.Sigla == siglaEstado)
                {
                    municipiosCarregamento.Add(new Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe()
                    {
                        Codigo = -(i + 1),
                        CodigoMunicipio = listaLocalidadesInicioPrestacao[i].Codigo,
                        DescricaoMunicipio = listaLocalidadesInicioPrestacao[i].Descricao,
                        Excluir = false
                    });
                }
            }

            return municipiosCarregamento;
        }

        private List<Dominio.ObjetosDeValor.VeiculoMDFe> ObterReboquesCTes(List<Dominio.Entidades.Veiculo> veiculos)
        {
            List<Dominio.ObjetosDeValor.VeiculoMDFe> reboquesMDFe = new List<Dominio.ObjetosDeValor.VeiculoMDFe>();

            for (var i = 0; i < veiculos.Count() && reboquesMDFe.Count() <= 3; i++)
            {
                if (veiculos[i].TipoVeiculo == "1")
                {
                    reboquesMDFe.Add(new Dominio.ObjetosDeValor.VeiculoMDFe()
                    {
                        CapacidadeKG = veiculos[i].CapacidadeKG,
                        CapacidadeM3 = veiculos[i].CapacidadeM3,
                        Codigo = -(i + 1),
                        Excluir = false,
                        Placa = veiculos[i].Placa,
                        RNTRC = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].RNTRC.ToString("D8") : string.Empty,
                        Tara = veiculos[i].Tara,
                        CPFCNPJ = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].Proprietario.CPF_CNPJ_SemFormato : string.Empty,
                        IE = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].Proprietario.IE_RG : string.Empty,
                        Nome = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].Proprietario.Nome.Length > 60 ? veiculos[i].Proprietario.Nome.Substring(0, 60) : veiculos[i].Proprietario.Nome : string.Empty,
                        TipoCarroceria = veiculos[i].TipoCarroceria,
                        TipoProprietario = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].TipoProprietario.ToString("D") : string.Empty,
                        TipoRodado = veiculos[i].TipoRodado,
                        UF = veiculos[i].Estado.Sigla,
                        UFProprietario = veiculos[i].Proprietario != null && veiculos[i].Tipo == "T" ? veiculos[i].Proprietario.Localidade.Estado.Sigla : string.Empty,
                        RENAVAM = veiculos[i].Renavam
                    });
                }
            }

            return reboquesMDFe;
        }

        private Dominio.ObjetosDeValor.VeiculoMDFe ObterVeiculoCTes(List<Dominio.Entidades.Veiculo> veiculos)
        {
            Dominio.ObjetosDeValor.VeiculoMDFe veiculoMDFe = (from obj in veiculos
                                                              where obj.TipoVeiculo == "0"
                                                              select new Dominio.ObjetosDeValor.VeiculoMDFe()
                                                              {
                                                                  CapacidadeKG = obj.CapacidadeKG,
                                                                  CapacidadeM3 = obj.CapacidadeM3,
                                                                  Codigo = -1,
                                                                  Excluir = false,
                                                                  Placa = obj.Placa,
                                                                  RENAVAM = obj.Renavam,
                                                                  RNTRC = obj.Proprietario != null && obj.Tipo == "T" ? obj.RNTRC.ToString("D8") : string.Empty,
                                                                  Tara = obj.Tara,
                                                                  CPFCNPJ = obj.Proprietario != null && obj.Tipo == "T" ? obj.Proprietario.CPF_CNPJ_SemFormato : string.Empty,
                                                                  IE = obj.Proprietario != null && obj.Tipo == "T" ? obj.Proprietario.IE_RG : string.Empty,
                                                                  Nome = obj.Proprietario != null && obj.Tipo == "T" ? obj.Proprietario.Nome.Length > 60 ? obj.Proprietario.Nome.Substring(0, 60) : obj.Proprietario.Nome : string.Empty,
                                                                  TipoCarroceria = obj.TipoCarroceria,
                                                                  TipoProprietario = obj.Proprietario != null && obj.Tipo == "T" ? obj.TipoProprietario.ToString("D") : string.Empty,
                                                                  TipoRodado = obj.TipoRodado,
                                                                  UF = obj.Estado.Sigla,
                                                                  UFProprietario = obj.Proprietario != null && obj.Tipo == "T" ? obj.Proprietario.Localidade.Estado.Sigla : string.Empty,
                                                                  CNPJFornecedorValePedagio = obj.FornecedorValePedagio?.CPF_CNPJ_SemFormato ?? string.Empty,
                                                                  CNPJResponsavelValePedagio = obj.ResponsavelValePedagio?.CPF_CNPJ_SemFormato ?? string.Empty,
                                                                  NumeroCompraValePedagio = obj.NumeroCompraValePedagio,
                                                                  ValorValePedagio = obj.ValorValePedagio
                                                              }).FirstOrDefault();
            return veiculoMDFe;
        }

        private List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe> ObterMunicipiosDescarregamentoCTes(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.InformacaoCargaCTE repInfoCargaCTe = new Repositorio.InformacaoCargaCTE(unitOfWork);
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdutosPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unitOfWork);

            List<int> codigosCidades = (from obj in ctes select obj.LocalidadeTerminoPrestacao.Codigo).Distinct().ToList();

            List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe> municipiosDescarregamento = new List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe>();

            for (var i = 0; i < codigosCidades.Count(); i++)
            {
                Dominio.Entidades.Localidade cidade = repLocalidade.BuscarPorCodigo(codigosCidades[i]);

                Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe municipioDescarregamento = new Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe();

                municipioDescarregamento.Codigo = -(i + 1);
                municipioDescarregamento.CodigoMunicipio = cidade.Codigo;
                municipioDescarregamento.DescricaoMunicipio = cidade.Descricao;
                municipioDescarregamento.Excluir = false;
                municipioDescarregamento.Documentos = new List<Dominio.ObjetosDeValor.DocumentoMunicipioDescarregamentoMDFe>();

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctesDoMunicipio = (from obj in ctes where obj.LocalidadeTerminoPrestacao.Codigo == codigosCidades[i] select obj).ToList();

                for (var j = 0; j < ctesDoMunicipio.Count(); j++)
                {
                    decimal pesoTotal = repInfoCargaCTe.ObterPesoKg(ctesDoMunicipio[j].Codigo);
                    if (pesoTotal == 0)
                        pesoTotal = repInfoCargaCTe.ObterPesoTon(ctesDoMunicipio[j].Codigo);
                    if (pesoTotal == 0)
                        pesoTotal = repInfoCargaCTe.ObterPesoTotal(ctesDoMunicipio[j].Codigo);

                    municipioDescarregamento.Documentos.Add(new Dominio.ObjetosDeValor.DocumentoMunicipioDescarregamentoMDFe()
                    {
                        Codigo = j,
                        Excluir = false,
                        CTe = new Dominio.ObjetosDeValor.CTeMDFe()
                        {
                            Codigo = ctesDoMunicipio[j].Codigo,
                            DataEmissao = ctesDoMunicipio[j].DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                            LocalidadeInicioPrestacao = string.Concat(ctesDoMunicipio[j].LocalidadeInicioPrestacao.Estado.Sigla, " / ", ctesDoMunicipio[j].LocalidadeInicioPrestacao.Descricao),
                            Numero = ctesDoMunicipio[j].Numero.ToString() + " - " + ctesDoMunicipio[j].Serie.Numero,
                            UFDescarregamento = ctesDoMunicipio[j].LocalidadeTerminoPrestacao.Estado.Sigla,
                            ValorFrete = ctesDoMunicipio[j].ValorFrete,
                            ValorTotalMercadoria = ctesDoMunicipio[j].ValorTotalMercadoria,
                            PesoTotal = pesoTotal,
                            PesoKgTotal = pesoTotal,
                            ProdutosPerigosos = repProdutosPerigosos.BuscarPorDocumentoParaMDFe(ctesDoMunicipio[j].Codigo)

                        },
                    });
                }

                municipiosDescarregamento.Add(municipioDescarregamento);
            }

            return municipiosDescarregamento;
        }

        private List<Dominio.ObjetosDeValor.MotoristaMDFe> ObterMotoristasCTes(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.MotoristaCTE repMotorista = new Repositorio.MotoristaCTE(unitOfWork);

            List<Dominio.Entidades.MotoristaCTE> motoristasCTes = repMotorista.BuscarPorCTe(this.EmpresaUsuario.Codigo, ctes.FirstOrDefault().Codigo);//(from obj in ctes select obj.Codigo).ToArray()
            motoristasCTes = motoristasCTes.GroupBy(o => new { o.NomeMotorista, o.CPFMotorista }).Select(g => g.First()).ToList();

            List<Dominio.ObjetosDeValor.MotoristaMDFe> motoristasMDFe = new List<Dominio.ObjetosDeValor.MotoristaMDFe>();

            for (var i = 0; i < motoristasCTes.Count(); i++)
            {
                motoristasMDFe.Add(new Dominio.ObjetosDeValor.MotoristaMDFe()
                {
                    Codigo = -(i + 1),
                    CPF = motoristasCTes[i].CPFMotorista,
                    Excluir = false,
                    Nome = motoristasCTes[i].NomeMotorista
                });
            }

            return motoristasMDFe;
        }

        private void SalvarLacres(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Lacres"]))
            {
                Repositorio.LacreMDFe repLacre = new Repositorio.LacreMDFe(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.LacreMDFe> lacres = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.LacreMDFe>>(Request.Params["Lacres"]);

                if (lacres != null)
                {
                    for (var i = 0; i < lacres.Count; i++)
                    {
                        Dominio.Entidades.LacreMDFe lacre = repLacre.BuscarPorCodigo(lacres[i].Codigo, mdfe.Codigo);

                        if (!lacres[i].Excluir)
                        {
                            if (lacre == null)
                                lacre = new Dominio.Entidades.LacreMDFe();

                            lacre.MDFe = mdfe;
                            lacre.Numero = lacres[i].Numero;

                            if (lacre.Codigo > 0)
                                repLacre.Atualizar(lacre);
                            else
                                repLacre.Inserir(lacre);

                        }
                        else if (lacre != null && lacre.Codigo > 0)
                        {
                            repLacre.Deletar(lacre);
                        }
                    }
                }
            }
        }

        private void SalvarMotoristas(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Motoristas"]))
            {
                Repositorio.MotoristaMDFe repMotoristaMDFe = new Repositorio.MotoristaMDFe(unidadeDeTrabalho);
                List<Dominio.ObjetosDeValor.MotoristaMDFe> motoristas = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MotoristaMDFe>>(Request.Params["Motoristas"]);

                if (motoristas != null)
                {
                    for (var i = 0; i < motoristas.Count; i++)
                    {
                        Dominio.Entidades.MotoristaMDFe motorista = repMotoristaMDFe.BuscarPorCodigo(motoristas[i].Codigo, mdfe.Codigo);

                        if (!motoristas[i].Excluir)
                        {
                            if (motorista == null)
                                motorista = new Dominio.Entidades.MotoristaMDFe();

                            motorista.MDFe = mdfe;
                            motorista.Nome = motoristas[i].Nome.Length > 60 ? motoristas[i].Nome.Substring(0, 60) : motoristas[i].Nome;
                            motorista.CPF = motoristas[i].CPF;
                            motorista.Tipo = Dominio.Enumeradores.TipoMotoristaMDFe.Normal;

                            if (motorista.Codigo > 0)
                                repMotoristaMDFe.Atualizar(motorista);
                            else
                                repMotoristaMDFe.Inserir(motorista);

                        }
                        else if (motorista != null && motorista.Codigo > 0)
                        {
                            repMotoristaMDFe.Deletar(motorista);
                        }
                    }
                }
            }
        }

        private void SalvarMunicipiosCarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["MunicipiosCarregamento"]))
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.MunicipioCarregamentoMDFe repMunicipioCarregamentoMDFe = new Repositorio.MunicipioCarregamentoMDFe(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe> municipiosCarregamento = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MunicipioCarregamentoMDFe>>(Request.Params["MunicipiosCarregamento"]);

                if (municipiosCarregamento != null)
                {
                    for (var i = 0; i < municipiosCarregamento.Count; i++)
                    {
                        Dominio.Entidades.MunicipioCarregamentoMDFe municipioCarregamento = repMunicipioCarregamentoMDFe.BuscarPorCodigo(municipiosCarregamento[i].Codigo, mdfe.Codigo);

                        if (!municipiosCarregamento[i].Excluir)
                        {
                            if (municipioCarregamento == null)
                                municipioCarregamento = new Dominio.Entidades.MunicipioCarregamentoMDFe();

                            municipioCarregamento.MDFe = mdfe;
                            municipioCarregamento.Municipio = repLocalidade.BuscarPorCodigo(municipiosCarregamento[i].CodigoMunicipio);

                            if (municipioCarregamento.Codigo > 0)
                                repMunicipioCarregamentoMDFe.Atualizar(municipioCarregamento);
                            else
                                repMunicipioCarregamentoMDFe.Inserir(municipioCarregamento);

                            if (String.IsNullOrWhiteSpace(mdfe.CEPCarregamentoLotacao))
                                mdfe.CEPCarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioCarregamento.Municipio.CEP);
                            if (mdfe.LatitudeCarregamentoLotacao == 0)
                                mdfe.LatitudeCarregamentoLotacao = municipioCarregamento.Municipio.Latitude;
                            if (mdfe.LongitudeCarregamentoLotacao == 0)
                                mdfe.LongitudeCarregamentoLotacao = municipioCarregamento.Municipio.Longitude;
                        }
                        else if (municipioCarregamento != null && municipioCarregamento.Codigo > 0)
                        {
                            repMunicipioCarregamentoMDFe.Deletar(municipioCarregamento);
                        }
                    }
                }
            }
        }

        private void SalvarMunicipiosDescarregamento(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["MunicipiosDescarregamento"]))
            {
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipio = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.NotaFiscalEletronicaMDFe repNFeMunicipio = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);
                Repositorio.CTeMDFe repCTeMDFe = new Repositorio.CTeMDFe(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe> municipiosDescarregamento = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe>>(Request.Params["MunicipiosDescarregamento"]);

                if (municipiosDescarregamento != null)
                {
                    for (var i = 0; i < municipiosDescarregamento.Count; i++)
                    {
                        Dominio.Entidades.MunicipioDescarregamentoMDFe municipioDescarregamento = repMunicipioDescarregamento.BuscarPorCodigo(municipiosDescarregamento[i].Codigo, mdfe.Codigo);

                        if (!municipiosDescarregamento[i].Excluir)
                        {
                            if (municipioDescarregamento == null)
                                municipioDescarregamento = new Dominio.Entidades.MunicipioDescarregamentoMDFe();

                            municipioDescarregamento.MDFe = mdfe;
                            municipioDescarregamento.Municipio = repLocalidade.BuscarPorCodigo(municipiosDescarregamento[i].CodigoMunicipio);

                            if (municipioDescarregamento.Codigo > 0)
                                repMunicipioDescarregamento.Atualizar(municipioDescarregamento);
                            else
                                repMunicipioDescarregamento.Inserir(municipioDescarregamento);

                            if (string.IsNullOrWhiteSpace(mdfe.CEPDescarregamentoLotacao))
                                mdfe.CEPDescarregamentoLotacao = Utilidades.String.OnlyNumbers(municipioDescarregamento.Municipio.CEP);
                            if (mdfe.LatitudeDescarregamentoLotacao == 0)
                                mdfe.LatitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Latitude;
                            if (mdfe.LongitudeDescarregamentoLotacao == 0)
                                mdfe.LongitudeDescarregamentoLotacao = municipioDescarregamento.Municipio.Longitude;

                            if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporte)
                                this.SalvarDocumentosMunicipioDescarregamento(mdfe, municipiosDescarregamento[i].Documentos, municipioDescarregamento, unidadeDeTrabalho);
                            else
                                repDocumentoMunicipio.DeletarPorMunicipio(municipioDescarregamento.Codigo);

                            if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte)
                                this.SalvarNFesMunicipioDescarregamento(municipiosDescarregamento[i].NFes, municipioDescarregamento, unidadeDeTrabalho);
                            else if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.TransporteCTeGlobalizado)
                                this.SalvarNFesMunicipioDescarregamento(municipiosDescarregamento[i].NFes, municipioDescarregamento, unidadeDeTrabalho);
                            else
                                repNFeMunicipio.DeletarPorMunicipio(municipioDescarregamento.Codigo);

                            if (mdfe.TipoEmitente == Dominio.Enumeradores.TipoEmitenteMDFe.PrestadorDeServicoDeTransporteApenasChaveCTe)
                                this.SalvarChaveCTesMunicipioDescarregamento(municipiosDescarregamento[i].ChaveCTes, municipioDescarregamento, unidadeDeTrabalho);
                            else
                                repCTeMDFe.DeletarPorMunicipio(municipioDescarregamento.Codigo);

                        }
                        else if (municipioDescarregamento != null && municipioDescarregamento.Codigo > 0)
                        {
                            this.DeletarProdutosPerigosos(municipioDescarregamento.Codigo, unidadeDeTrabalho);
                            repNFeMunicipio.DeletarPorMunicipio(municipioDescarregamento.Codigo);
                            repDocumentoMunicipio.DeletarPorMunicipio(municipioDescarregamento.Codigo);
                            repMunicipioDescarregamento.Deletar(municipioDescarregamento);
                        }
                    }
                }
            }
        }

        private void DeletarProdutosPerigosos(int codigoMunicipio, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumentoMunicipio = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unidadeDeTrabalho);

            List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = repDocumentoMunicipio.BuscarPorMunicipio(codigoMunicipio);

            for (var i = 0; i < documentos.Count(); i++)
                repProdPerigosos.DeletarPorDocumento(documentos[i].Codigo);
        }

        private void SalvarProdutosPerigososDocumentosMunicipioDescarregamento(List<Dominio.ObjetosDeValor.MDFeProdutosPerigosos> produtosPerigosos, Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unidadeDeTrabalho);

            for (var i = 0; i < produtosPerigosos.Count(); i++)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos prodPerigosos = repProdPerigosos.BuscarPorCodigo(produtosPerigosos[i].Id);

                if (prodPerigosos == null)
                    prodPerigosos = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFeProdPerigosos();

                prodPerigosos.ClasseRisco = produtosPerigosos[i].ClasseRisco;
                prodPerigosos.GrupoEmbalagem = produtosPerigosos[i].GrupoEmbalagem;
                prodPerigosos.Nome = produtosPerigosos[i].NomeApropriado;
                prodPerigosos.NumeroONU = produtosPerigosos[i].NumeroONU;
                prodPerigosos.QuantidadeTipoVolumes = produtosPerigosos[i].QuantidadeETipo;
                prodPerigosos.QuantidadeTotalProduto = produtosPerigosos[i].QuantidadeTotal;
                prodPerigosos.DocumentoMunicipioDescarregamentoMDFe = documento;

                if (prodPerigosos.Codigo > 0)
                {
                    if (produtosPerigosos[i].Excluir)
                        repProdPerigosos.Deletar(prodPerigosos);
                    else
                        repProdPerigosos.Atualizar(prodPerigosos);

                }
                else
                    repProdPerigosos.Inserir(prodPerigosos);

            }

        }

        private void SalvarDocumentosMunicipioDescarregamento(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, List<Dominio.ObjetosDeValor.DocumentoMunicipioDescarregamentoMDFe> documentos, Dominio.Entidades.MunicipioDescarregamentoMDFe municipio, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (documentos != null)
            {
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

                for (var i = 0; i < documentos.Count(); i++)
                {
                    Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento = repDocumento.BuscarPorCodigo(documentos[i].Codigo, municipio.Codigo);

                    if (!documentos[i].Excluir)
                    {
                        if (documento == null)
                            documento = new Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe();

                        documento.MunicipioDescarregamento = municipio;
                        documento.CTe = repCTe.BuscarPorCodigo(municipio.MDFe.Empresa.Codigo, documentos[i].CTe.Codigo); //, "A"

                        if (documento.Codigo > 0)
                            repDocumento.Atualizar(documento);
                        else
                            repDocumento.Inserir(documento);

                        if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00" && documentos[i].CTe.ProdutosPerigosos != null)
                            this.SalvarProdutosPerigososDocumentosMunicipioDescarregamento(documentos[i].CTe.ProdutosPerigosos, documento, unidadeDeTrabalho);
                    }
                    else if (documento != null && documento.Codigo > 0)
                    {
                        repDocumento.Deletar(documento);
                    }
                }
            }
        }

        private void SalvarNFesMunicipioDescarregamento(List<Dominio.ObjetosDeValor.NotaFiscalEletronicaMDFe> notasFiscais, Dominio.Entidades.MunicipioDescarregamentoMDFe municipio, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (notasFiscais != null)
            {
                Repositorio.NotaFiscalEletronicaMDFe repNotaFiscalMDFe = new Repositorio.NotaFiscalEletronicaMDFe(unidadeDeTrabalho);

                for (var i = 0; i < notasFiscais.Count(); i++)
                {
                    Dominio.Entidades.NotaFiscalEletronicaMDFe notaFiscal = repNotaFiscalMDFe.Buscar(notasFiscais[i].Codigo, municipio.Codigo);

                    if (!notasFiscais[i].Excluir)
                    {
                        if (notaFiscal == null)
                            notaFiscal = new Dominio.Entidades.NotaFiscalEletronicaMDFe();

                        notaFiscal.MunicipioDescarregamento = municipio;
                        notaFiscal.Chave = Utilidades.String.OnlyNumbers(notasFiscais[i].Chave);
                        notaFiscal.SegundoCodigoDeBarra = Utilidades.String.OnlyNumbers(notasFiscais[i].SegundoCodigoDeBarra);

                        if (notaFiscal.Codigo > 0)
                            repNotaFiscalMDFe.Atualizar(notaFiscal);
                        else
                            repNotaFiscalMDFe.Inserir(notaFiscal);
                    }
                    else if (notaFiscal != null && notaFiscal.Codigo > 0)
                    {
                        repNotaFiscalMDFe.Deletar(notaFiscal);
                    }
                }
            }
        }

        private void SalvarChaveCTesMunicipioDescarregamento(List<Dominio.ObjetosDeValor.ChaveCTes> chavesCTes, Dominio.Entidades.MunicipioDescarregamentoMDFe municipio, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (chavesCTes != null)
            {
                Repositorio.CTeMDFe repCTeMDFe = new Repositorio.CTeMDFe(unidadeDeTrabalho);

                for (var i = 0; i < chavesCTes.Count(); i++)
                {
                    Dominio.Entidades.CTeMDFe chaveCTeMDFe = repCTeMDFe.Buscar(chavesCTes[i].Codigo, municipio.Codigo);

                    if (!chavesCTes[i].Excluir)
                    {
                        if (chaveCTeMDFe == null)
                            chaveCTeMDFe = new Dominio.Entidades.CTeMDFe();

                        chaveCTeMDFe.MunicipioDescarregamento = municipio;
                        chaveCTeMDFe.Chave = Utilidades.String.OnlyNumbers(chavesCTes[i].Chave);

                        if (chaveCTeMDFe.Codigo > 0)
                            repCTeMDFe.Atualizar(chaveCTeMDFe);
                        else
                            repCTeMDFe.Inserir(chaveCTeMDFe);
                    }
                    else if (chaveCTeMDFe != null && chaveCTeMDFe.Codigo > 0)
                    {
                        repCTeMDFe.Deletar(chaveCTeMDFe);
                    }
                }
            }
        }

        private void SalvarPercursos(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Percursos"]))
            {
                Repositorio.PercursoMDFe repPercurso = new Repositorio.PercursoMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.PercursoMDFe> percursos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.PercursoMDFe>>(Request.Params["Percursos"]);

                if (percursos != null)
                {
                    for (var i = 0; i < percursos.Count; i++)
                    {
                        Dominio.Entidades.PercursoMDFe percurso = repPercurso.BuscarPorCodigo(percursos[i].Codigo, mdfe.Codigo);

                        if (!percursos[i].Excluir)
                        {
                            if (percurso == null)
                                percurso = new Dominio.Entidades.PercursoMDFe();

                            DateTime data;
                            DateTime.TryParseExact(percursos[i].Data + " " + percursos[i].Hora, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data);

                            if (data != DateTime.MinValue)
                                percurso.DataInicioViagem = data;
                            else
                                percurso.DataInicioViagem = null;

                            percurso.MDFe = mdfe;
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
            }
        }

        private void SalvarPercursoEntreEstados(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.PercursoEstado repPercurso = new Repositorio.PercursoEstado(unidadeDeTrabalho);
            Repositorio.PassagemPercursoEstado repPassagem = new Repositorio.PassagemPercursoEstado(unidadeDeTrabalho);
            Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeDeTrabalho);

            Dominio.Entidades.PercursoEstado percurso = repPercurso.BuscarPorOrigemEDestino(this.EmpresaUsuario.Codigo, mdfe.EstadoCarregamento.Sigla, mdfe.EstadoDescarregamento.Sigla);

            if (percurso == null)
            {
                percurso = new Dominio.Entidades.PercursoEstado();

                percurso.Empresa = this.EmpresaUsuario;
                percurso.EstadoDestino = mdfe.EstadoDescarregamento;
                percurso.EstadoOrigem = mdfe.EstadoCarregamento;

                repPercurso.Inserir(percurso);
            }

            List<Dominio.Entidades.PassagemPercursoEstado> passagens = repPassagem.Buscar(percurso.Codigo);
            List<Dominio.Entidades.PercursoMDFe> percursosMDFe = repPercursoMDFe.BuscarPorMDFe(mdfe.Codigo);

            if (!(from obj in passagens orderby obj.EstadoDePassagem.Sigla select obj.EstadoDePassagem.Sigla).SequenceEqual(from obj in percursosMDFe orderby obj.Estado.Sigla select obj.Estado.Sigla))
            {
                if (passagens.Count > 0)
                    foreach (Dominio.Entidades.PassagemPercursoEstado passagem in passagens)
                        repPassagem.Deletar(passagem);

                for (var i = 0; i < percursosMDFe.Count; i++)
                {
                    Dominio.Entidades.PassagemPercursoEstado passagem = new Dominio.Entidades.PassagemPercursoEstado();

                    passagem.EstadoDePassagem = percursosMDFe[i].Estado;
                    passagem.Ordem = i + 1;
                    passagem.Percurso = percurso;

                    repPassagem.Inserir(passagem);
                }
            }
        }

        private void SalvarReboques(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Reboques"]))
            {
                Repositorio.ReboqueMDFe repReboque = new Repositorio.ReboqueMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.VeiculoMDFe> reboques = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.VeiculoMDFe>>(Request.Params["Reboques"]);

                if (reboques != null)
                {
                    for (var i = 0; i < reboques.Count; i++)
                    {
                        Dominio.Entidades.ReboqueMDFe reboque = repReboque.BuscarPorCodigo(reboques[i].Codigo, mdfe.Codigo);

                        if (!reboques[i].Excluir)
                        {
                            if (reboque == null)
                                reboque = new Dominio.Entidades.ReboqueMDFe();

                            reboque.CPFCNPJProprietario = reboques[i].CPFCNPJ;
                            reboque.IEProprietario = reboques[i].IE;
                            reboque.NomeProprietario = reboques[i].Nome != null && reboques[i].Nome.Length > 60 ? reboques[i].Nome.Substring(0, 60) : reboques[i].Nome;
                            reboque.TipoCarroceria = reboques[i].TipoCarroceria;
                            reboque.TipoProprietario = reboques[i].TipoProprietario;
                            reboque.UF = repEstado.BuscarPorSigla(reboques[i].UF);
                            reboque.UFProprietario = repEstado.BuscarPorSigla(reboques[i].UFProprietario);
                            reboque.MDFe = mdfe;
                            reboque.CapacidadeKG = reboques[i].CapacidadeKG;
                            reboque.CapacidadeM3 = reboques[i].CapacidadeM3;
                            reboque.Placa = reboques[i].Placa;
                            reboque.RENAVAM = reboques[i].RENAVAM;
                            reboque.RNTRC = reboques[i].RNTRC;
                            reboque.Tara = reboques[i].Tara;

                            if (reboque.Codigo > 0)
                                repReboque.Atualizar(reboque);
                            else
                                repReboque.Inserir(reboque);
                        }
                        else if (reboque != null && reboque.Codigo > 0)
                        {
                            repReboque.Deletar(reboque);
                        }
                    }
                }
            }
        }

        private void SalvarVeiculo(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Veiculo"]))
            {
                Dominio.ObjetosDeValor.VeiculoMDFe veiculo = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.VeiculoMDFe>(Request.Params["Veiculo"]);

                Repositorio.VeiculoMDFe repVeiculo = new Repositorio.VeiculoMDFe(unidadeDeTrabalho);
                Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                Dominio.Entidades.VeiculoMDFe veic = repVeiculo.BuscarPorMDFe(mdfe.Codigo);

                if (veic == null)
                    veic = new Dominio.Entidades.VeiculoMDFe();

                veic.CPFCNPJProprietario = veiculo.CPFCNPJ;
                veic.IEProprietario = veiculo.IE;
                veic.NomeProprietario = veiculo.Nome != null && veiculo.Nome.Length > 60 ? veiculo.Nome.Substring(0, 60) : veiculo.Nome;
                veic.TipoCarroceria = veiculo.TipoCarroceria;
                veic.TipoRodado = veiculo.TipoRodado;
                veic.TipoProprietario = veiculo.TipoProprietario;
                veic.UF = repEstado.BuscarPorSigla(veiculo.UF);
                veic.UFProprietario = repEstado.BuscarPorSigla(veiculo.UFProprietario);
                veic.MDFe = mdfe;
                veic.CapacidadeKG = veiculo.CapacidadeKG != null ? veiculo.CapacidadeKG : 0;
                veic.CapacidadeM3 = veiculo.CapacidadeM3 != null ? veiculo.CapacidadeM3 : 0;
                veic.Placa = veiculo.Placa;
                veic.RENAVAM = veiculo.RENAVAM;
                veic.RNTRC = veiculo.RNTRC;
                veic.Tara = veiculo.Tara;

                if (veic.Codigo > 0)
                    repVeiculo.Atualizar(veic);
                else
                    repVeiculo.Inserir(veic);
            }
        }

        private void SalvarSeguros(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
            {
                bool inseriuSeguro = false;

                Repositorio.MDFeContratante repContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);
                List<Dominio.Entidades.MDFeContratante> contratantes = repContratante.BuscarPorMDFe(mdfe.Codigo);

                if (!string.IsNullOrWhiteSpace(Request.Params["Seguros"]))
                {
                    List<Dominio.ObjetosDeValor.InformacaoSeguroMDFe> seguros = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.InformacaoSeguroMDFe>>(Request.Params["Seguros"]);
                    Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);

                    if (seguros != null)
                    {
                        for (var i = 0; i < seguros.Count; i++)
                        {
                            Dominio.Entidades.MDFeSeguro seguro = repMDFeSeguro.BuscarPorCodigo(seguros[i].Id, mdfe.Codigo);

                            if (!seguros[i].Excluir)
                            {
                                if (seguro == null)
                                    seguro = new Dominio.Entidades.MDFeSeguro();

                                seguro.TipoResponsavel = seguros[i].Tipo;
                                seguro.Responsavel = !string.IsNullOrWhiteSpace(seguros[i].Responsavel) ? seguros[i].Responsavel : seguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : contratantes != null && contratantes.Count > 0 ? contratantes.FirstOrDefault().Contratante : string.Empty;

                                seguro.CNPJSeguradora = !string.IsNullOrWhiteSpace(seguros[i].CNPJSeguradora) ? seguros[i].CNPJSeguradora :
                                                        mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;
                                seguro.NomeSeguradora = !string.IsNullOrWhiteSpace(seguros[i].Seguradora) ? seguros[i].Seguradora :
                                                        mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;
                                seguro.NumeroApolice = !string.IsNullOrWhiteSpace(seguros[i].NumeroApolice) ? seguros[i].NumeroApolice :
                                                       mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                                       mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;
                                seguro.NumeroAverbacao = !string.IsNullOrWhiteSpace(seguros[i].NumeroAverbacao) ? seguros[i].NumeroAverbacao :
                                                         mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                         mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                        mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(seguro.NumeroApolice) ? seguro.NumeroApolice : string.Empty;
                                seguro.MDFe = mdfe;

                                if (seguro.Codigo > 0)
                                    repMDFeSeguro.Atualizar(seguro);
                                else
                                    repMDFeSeguro.Inserir(seguro);

                                inseriuSeguro = true;
                            }
                            else if (seguro != null && seguro.Codigo > 0)
                            {
                                repMDFeSeguro.Deletar(seguro);
                            }
                        }
                    }
                }

                if (!inseriuSeguro && mdfe.Empresa.Configuracao != null && mdfe.TipoEmitente != Dominio.Enumeradores.TipoEmitenteMDFe.NaoPrestadorDeServicoDeTransporte)
                {
                    Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeDeTrabalho);
                    Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

                    mdfeSeguro.MDFe = mdfe;
                    mdfeSeguro.TipoResponsavel = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                 mdfe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
                                                 mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
                                                 mdfe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

                    mdfeSeguro.Responsavel = mdfeSeguro.TipoResponsavel == Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente ? mdfe.Empresa.CNPJ : contratantes != null && contratantes.Count > 0 ? contratantes.FirstOrDefault().Contratante : mdfe.Empresa.CNPJ;

                    mdfeSeguro.CNPJSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CNPJSeguro) ? mdfe.Empresa.Configuracao.CNPJSeguro :
                                                mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
                                                mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.CNPJ : string.Empty;

                    mdfeSeguro.NomeSeguradora = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NomeSeguro) ? mdfe.Empresa.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.Configuracao.NomeSeguro :
                                                mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : mdfe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
                                                mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? mdfe.Empresa.RazaoSocial.Length > 30 ? mdfe.Empresa.RazaoSocial.Substring(0, 30) : mdfe.Empresa.RazaoSocial : string.Empty;

                    mdfeSeguro.NumeroApolice = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.Configuracao.NumeroApoliceSeguro :
                                               mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 20 ? mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

                    mdfeSeguro.NumeroAverbacao = mdfe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.Configuracao.AverbacaoSeguro :
                                                 mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : mdfe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
                                                 mdfe.Empresa.Configuracao != null && !mdfe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

                    repMDFeSeguro.Inserir(mdfeSeguro);
                }
            }
        }

        private void SalvarContratantes(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
            {
                if (!string.IsNullOrWhiteSpace(Request.Params["Contratantes"]))
                {
                    List<Dominio.ObjetosDeValor.ContratantesMDFe> contratantes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ContratantesMDFe>>(Request.Params["Contratantes"]);
                    Repositorio.MDFeContratante repMDFeContratante = new Repositorio.MDFeContratante(unidadeDeTrabalho);

                    if (contratantes != null)
                    {
                        for (var i = 0; i < contratantes.Count; i++)
                        {
                            Dominio.Entidades.MDFeContratante contratante = repMDFeContratante.BuscarPorCodigo(contratantes[i].Id, mdfe.Codigo);

                            if (!contratantes[i].Excluir)
                            {
                                if (contratante == null)
                                    contratante = new Dominio.Entidades.MDFeContratante();

                                contratante.Contratante = Utilidades.String.OnlyNumbers(contratantes[i].CPF_CNPJ);
                                contratante.NomeContratante = !string.IsNullOrWhiteSpace(contratantes[i].Nome) ? contratantes[i].Nome : string.Empty;
                                contratante.IDEstrangeiro = !string.IsNullOrWhiteSpace(contratantes[i].IDEstrangeiro) ? contratantes[i].IDEstrangeiro : string.Empty;
                                contratante.MDFe = mdfe;

                                if (contratante.Codigo > 0)
                                    repMDFeContratante.Atualizar(contratante);
                                else
                                    repMDFeContratante.Inserir(contratante);
                            }
                            else if (contratante != null && contratante.Codigo > 0)
                            {
                                repMDFeContratante.Deletar(contratante);
                            }
                        }
                    }
                }
            }
        }

        private void SalvarCIOTs(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(mdfe.Versao) && mdfe.Versao == "3.00")
            {
                if (!string.IsNullOrWhiteSpace(Request.Params["CIOTs"]))
                {
                    List<Dominio.ObjetosDeValor.MDFeCIOT> CIOTS = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.MDFeCIOT>>(Request.Params["CIOTs"]);
                    Repositorio.MDFeCIOT repMDFeCIOT = new Repositorio.MDFeCIOT(unidadeDeTrabalho);

                    if (CIOTS != null)
                    {
                        for (var i = 0; i < CIOTS.Count; i++)
                        {
                            Dominio.Entidades.MDFeCIOT CIOT = repMDFeCIOT.BuscarPorCodigo(CIOTS[i].Id, mdfe.Codigo);

                            if (!CIOTS[i].Excluir)
                            {
                                if (CIOT == null)
                                    CIOT = new Dominio.Entidades.MDFeCIOT();

                                CIOT.NumeroCIOT = CIOTS[i].CIOT;
                                CIOT.Responsavel = Utilidades.String.OnlyNumbers(CIOTS[i].CPF_CNPJ);
                                CIOT.MDFe = mdfe;

                                if (CIOT.Codigo > 0)
                                    repMDFeCIOT.Atualizar(CIOT);
                                else
                                    repMDFeCIOT.Inserir(CIOT);
                            }
                            else if (CIOT != null && CIOT.Codigo > 0)
                            {
                                repMDFeCIOT.Deletar(CIOT);
                            }
                        }
                    }
                }
            }
        }

        private void SalvarComponentesPagamento(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Componentes"]))
            {
                List<Dominio.ObjetosDeValor.ComponentePagamentoCIOT> listaComponente = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ComponentePagamentoCIOT>>(Request.Params["Componentes"]);

                if (listaComponente != null)
                {
                    Repositorio.Embarcador.MDFE.MDFePagamentoComponente repositorioMDFePagamentoComponente = new Repositorio.Embarcador.MDFE.MDFePagamentoComponente(unidadeDeTrabalho);

                    for (var i = 0; i < listaComponente.Count; i++)
                    {
                        Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento tipoComponente = (Dominio.ObjetosDeValor.Enumerador.TipoComponentePagamento)listaComponente[i].TipoComponente;
                        Dominio.Entidades.MDFePagamentoComponente componente = repositorioMDFePagamentoComponente.BuscarPorInformacoesBancariasETipoComponente(informacoesBancarias.Codigo, tipoComponente);

                        if (!listaComponente[i].Excluir)
                        {
                            componente ??= new Dominio.Entidades.MDFePagamentoComponente();
                            componente.InformacoesBancarias = informacoesBancarias;
                            componente.TipoComponente = tipoComponente;
                            componente.ValorComponente = listaComponente[i].ValorComponente.ToDecimal();

                            if (componente.Codigo > 0)
                                repositorioMDFePagamentoComponente.Atualizar(componente);
                            else
                                repositorioMDFePagamentoComponente.Inserir(componente);
                        }
                        else if (componente != null && componente.Codigo > 0)
                            repositorioMDFePagamentoComponente.Deletar(componente);
                    }
                }
            }
        }

        private void SalvarParcelasPagamento(Dominio.Entidades.MDFeInformacoesBancarias informacoesBancarias, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["Parcelas"]))
            {
                List<Dominio.ObjetosDeValor.ParcelaPagamentoCIOT> listaParcela = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ParcelaPagamentoCIOT>>(Request.Params["Parcelas"]);

                if (listaParcela != null)
                {
                    Repositorio.Embarcador.MDFE.MDFePagamentoParcela repositorioMDFePagamentoParcela = new Repositorio.Embarcador.MDFE.MDFePagamentoParcela(unidadeDeTrabalho);

                    for (var i = 0; i < listaParcela.Count; i++)
                    {
                        Dominio.Entidades.MDFePagamentoParcela parcela = repositorioMDFePagamentoParcela.BuscarPorInformacoesBancariasENumeroParcela(informacoesBancarias.Codigo, listaParcela[i].NumeroParcela);
                        decimal valorParcela = listaParcela[i].ValorParcela.ToDecimal();

                        if (!listaParcela[i].Excluir && valorParcela > 0)
                        {
                            parcela ??= new Dominio.Entidades.MDFePagamentoParcela();
                            parcela.InformacoesBancarias = informacoesBancarias;
                            parcela.NumeroParcela = listaParcela[i].NumeroParcela;
                            parcela.DataVencimentoParcela = Convert.ToDateTime(listaParcela[i].DataVencimento);
                            parcela.ValorParcela = valorParcela;

                            if (parcela.Codigo > 0)
                                repositorioMDFePagamentoParcela.Atualizar(parcela);
                            else
                                repositorioMDFePagamentoParcela.Inserir(parcela);
                        }
                        else if (parcela != null && parcela.Codigo > 0)
                            repositorioMDFePagamentoParcela.Deletar(parcela);
                    }
                }
            }
        }

        private void SalvarValesPedagio(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ValesPedagio"]))
            {
                Repositorio.ValePedagioMDFe repValePedagio = new Repositorio.ValePedagioMDFe(unidadeDeTrabalho);

                List<Dominio.ObjetosDeValor.ValePedagioMDFe> valesPedagio = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ValePedagioMDFe>>(Request.Params["ValesPedagio"]);

                if (valesPedagio != null)
                {
                    for (var i = 0; i < valesPedagio.Count; i++)
                    {
                        Dominio.Entidades.ValePedagioMDFe valePedagio = repValePedagio.BuscarPorCodigo(valesPedagio[i].Codigo, mdfe.Codigo);

                        if (!valesPedagio[i].Excluir)
                        {
                            if (valePedagio == null)
                                valePedagio = new Dominio.Entidades.ValePedagioMDFe();

                            valePedagio.MDFe = mdfe;
                            valePedagio.CNPJFornecedor = valesPedagio[i].CNPJFornecedor;
                            valePedagio.CNPJResponsavel = valesPedagio[i].CNPJResponsavel;
                            valePedagio.NumeroComprovante = valesPedagio[i].NumeroComprovante;
                            valePedagio.CodigoAgendamentoPorto = valesPedagio[i].CodigoAgendamentoPorto;
                            valePedagio.ValorValePedagio = valesPedagio[i].ValorValePedagio;
                            valePedagio.QuantidadeEixos = valesPedagio[i].QuantidadeEixos.HasValue ? valesPedagio[i].QuantidadeEixos.Value : 2;
                            valePedagio.TipoCompra = valesPedagio[i].TipoCompra.HasValue ? valesPedagio[i].TipoCompra.Value : Dominio.Enumeradores.TipoCompraValePedagio.Tag;

                            if (valePedagio.Codigo > 0)
                                repValePedagio.Atualizar(valePedagio);
                            else
                                repValePedagio.Inserir(valePedagio);
                        }
                        else if (valePedagio != null && valePedagio.Codigo > 0)
                        {
                            repValePedagio.Deletar(valePedagio);
                        }
                    }
                }
            }
        }

        private int TempoMaximoCancelarMDFe(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe)
        {
            int limiteConfigurado = 0;

            if (this.EmpresaUsuario.Configuracao != null && this.EmpresaUsuario.Configuracao.PrazoCancelamentoMDFe > 0)
                limiteConfigurado = this.EmpresaUsuario.Configuracao.PrazoCancelamentoMDFe;
            else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && this.EmpresaUsuario.EmpresaPai.Configuracao.PrazoCancelamentoMDFe > 0)
                limiteConfigurado = this.EmpresaUsuario.EmpresaPai.Configuracao.PrazoCancelamentoMDFe;
            else
                return 0;

            return limiteConfigurado;
        }

        private string VersaoMDFe()
        {
            string versao = "3.00";

            if (this.EmpresaUsuario.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.Configuracao.VersaoMDFe))
                versao = this.EmpresaUsuario.Configuracao.VersaoMDFe;
            else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoMDFe))
                versao = this.EmpresaUsuario.EmpresaPai.Configuracao.VersaoMDFe;

            return versao;
        }

        static T? VerificarPreenchimento<T>(NameValueCollection p, string key, Func<string, T> parse) where T : struct
        {
            var s = p[key];
            if (string.IsNullOrWhiteSpace(s)) return null;
            try { return parse(s); } catch { return null; }
        }

        static string VerificarString(NameValueCollection p, string key)
        {
            var s = p[key];
            return string.IsNullOrWhiteSpace(s) ? (string)null : s;
        }

        static decimal? VerificarDecimal(NameValueCollection p, string key, IFormatProvider culture, NumberStyles styles = NumberStyles.Number)
            => VerificarPreenchimento<decimal>(p, key, s => decimal.Parse(s, styles, culture));

        static int? VerificarInt(NameValueCollection p, string key)
            => VerificarPreenchimento<int>(p, key, s => int.Parse(s, CultureInfo.InvariantCulture));

        static DateTime? VerificarData(NameValueCollection p, string key, string formato, IFormatProvider culture)
            => VerificarPreenchimento<DateTime>(p, key, s => DateTime.ParseExact(s, formato, culture));

        #endregion
    }
}
