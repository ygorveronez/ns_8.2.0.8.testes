using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmissaoCTe.API.Controllers
{
    public class DOCCOBController : ApiController
    {
        #region Variáveis Globais

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("doccob.aspx") select obj).FirstOrDefault();
        }

        #endregion

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]);

                int codigoDuplicata = 0;
                int.TryParse(Request.Params["CodigoDuplicata"], out codigoDuplicata);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                List<Dominio.ObjetosDeValor.ConsultaCTe> ctes = repCTe.ConsultarPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, string.Empty, string.Empty, cpfCnpjRemetente, cpfCnpjDestinatario, new string[] { "A" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, inicioRegistros, 50, codigoDuplicata, true);
                int countCTes = repCTe.ContarConsultaPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, string.Empty, string.Empty, cpfCnpjRemetente, cpfCnpjDestinatario, new string[] { "A" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, codigoDuplicata, true);

                var retorno = (from cte in ctes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   Serie = cte.Serie.ToString(),
                                   Documento = cte.Documento,
                                   Remetente = cte.Remetente != null ? cte.Remetente.Nome : string.Empty,
                                   LocalidadeRemetente = cte.Remetente != null ? cte.Remetente.Localidade != null ? cte.Remetente.Localidade.Estado.Sigla + " / " + cte.Remetente.Localidade.Descricao : cte.Remetente.Cidade : string.Empty,
                                   Destinatario = cte.Destinatario != null ? cte.Destinatario.Nome : string.Empty,
                                   LocalidadeDestinatario = cte.Destinatario != null ? cte.Destinatario.Localidade != null ? cte.Destinatario.Localidade.Estado.Sigla + " / " + cte.Destinatario.Localidade.Descricao : cte.Remetente.Cidade : string.Empty,
                                   ValorFrete = cte.Valor.ToString("n2")
                               }).ToList();

                return Json(retorno, true, "", new string[] { "Codigo", "Número|10", "Série|8", "Doc.|8", "Remetente|12", "Cidade Rem.|10", "Destinatário|15", "Cidade Dest.|15", "Valor Frete|12" }, countCTes);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult SelecionarTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                DateTime dataInicial, dataFinal;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]);

                int codigoDuplicata = 0;
                int.TryParse(Request.Params["CodigoDuplicata"], out codigoDuplicata);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int countCTes = repCTe.ContarConsultaPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, string.Empty, string.Empty, cpfCnpjRemetente, cpfCnpjDestinatario, new string[] { "A" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, codigoDuplicata);
                List<Dominio.ObjetosDeValor.ConsultaCTe> ctes = repCTe.ConsultarPorDuplicata(this.EmpresaUsuario.Codigo, dataInicial, dataFinal, 0, 0, string.Empty, string.Empty, cpfCnpjRemetente, cpfCnpjDestinatario, new string[] { "A" }, Dominio.Enumeradores.TipoCTE.Todos, this.EmpresaUsuario.TipoAmbiente, this.Usuario.Series.Where(o => o.Tipo == Dominio.Enumeradores.TipoSerie.CTe).Select(o => o.Codigo).ToArray(), 0, string.Empty, string.Empty, 0, countCTes, codigoDuplicata);

                var retorno = (from cte in ctes
                               select new
                               {
                                   cte.Codigo,
                                   Numero = cte.Numero.ToString(),
                                   ValorFrete = cte.Valor.ToString("n2")
                               }).ToList();

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao selecionar os CT-es.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult PreparaGerarao()
        {
            /* Devido a um problema na selecao de muitos CTes, o tamanho do GET excedia o limite
             * 
             * Para solução do problema:
             * - A requisição do download é enviada via POST
             * - Informações são salvas numa sessão
             * - Retorna o nome da sessão
             * - Método do download envia essa sessão
             */
            string idsRequisicao = Request.Params["CTes"];
            string nomeSessao = "DOCCOB" + DateTime.Now.ToString("HHmmss");

            Session[nomeSessao] = idsRequisicao;

            return Json(nomeSessao, true);
        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult Gerar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (this.Permissao() == null || this.Permissao().PermissaoDeAcesso != "A")
                    return Json<bool>(false, false, "Permissão negada para acessar este recurso!");

                DateTime dataInicial = DateTime.MinValue;
                DateTime dataFinal = DateTime.MinValue;
                DateTime.TryParseExact(Request.Params["DataInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataInicial);
                DateTime.TryParseExact(Request.Params["DataFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataFinal);

                string cpfCnpjRemetente = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJRemetente"]);
                string cpfCnpjDestinatario = Utilidades.String.OnlyNumbers(Request.Params["CPFCNPJDestinatario"]);
                string nomeSessao = Request.Params["Sessao"];

                int codigoLayout = 0;
                int.TryParse(Request.Params["Versao"], out codigoLayout);

                int codigoDuplicata = 0;
                int.TryParse(Request.Params["CodigoDuplicata"], out codigoDuplicata);

                List<int> codigosCTes = JsonConvert.DeserializeObject<List<int>>(Session[nomeSessao].ToString());

                Session.Remove(nomeSessao);

                Repositorio.LayoutEDI repLayout = new Repositorio.LayoutEDI(unitOfWork);
                Dominio.Entidades.LayoutEDI layout = repLayout.BuscarPorCodigoETipo(codigoLayout, Dominio.Enumeradores.TipoLayoutEDI.DOCCOB);

                if (layout == null)
                    return Json<bool>(false, false, "Layout do arquivo não encontrado.");

                Repositorio.Duplicata repDuplciata = new Repositorio.Duplicata(unitOfWork);
                Dominio.Entidades.Duplicata duplicata = repDuplciata.BuscaPorCodigo(0, codigoDuplicata);

                System.IO.MemoryStream arquivo = this.GerarDOCCOB(cpfCnpjRemetente, cpfCnpjDestinatario, dataInicial, dataFinal, 0, layout, codigosCTes, codigoDuplicata, unitOfWork);

                Servicos.GeracaoEDI svcEDI = new Servicos.GeracaoEDI(unitOfWork);
                string nomeArquivo = string.Empty;
                if (!string.IsNullOrWhiteSpace(layout.Nomenclatura))
                    nomeArquivo = svcEDI.ObterNomenclaturaLayoutEDI(layout.Nomenclatura, this.EmpresaUsuario, duplicata != null ? duplicata.Pessoa : null, duplicata != null ? duplicata.Numero.ToString() : string.Empty, DateTime.Now);

                if (string.IsNullOrWhiteSpace(nomeArquivo))
                {
                    if (codigoDuplicata > 0)
                        return Arquivo(arquivo, "text/plain", string.Concat("DOCCOB_", duplicata.Numero, "_", duplicata.Pessoa.CPF_CNPJ_SemFormato, ".txt"));
                    else
                        nomeArquivo = string.Concat("DOCCOB_", dataInicial.ToString("ddMMyy"), "-", dataFinal.ToString("ddMMyy"));
                }

                if (codigoDuplicata == 0)
                    return Arquivo(arquivo, "application/zip", string.Concat(nomeArquivo, ".zip"));
                else
                    return Arquivo(arquivo, "text/plain", string.Concat(nomeArquivo, ".txt"));

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao gerar o arquivo DOCCOB.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public System.IO.MemoryStream GerarDOCCOB(string cpfCnpjRemetente, string cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, Dominio.Entidades.LayoutEDI layout, List<int> codigosCTes, int codigoDuplicata, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.GeracaoEDI svcEDI = null;

            if (codigoDuplicata == 0)
            {
                svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, dataInicial, dataFinal, codigoVeiculo, false, codigosCTes, codigoDuplicata, null, null, cpfCnpjRemetente, cpfCnpjDestinatario);
                return svcEDI.GerarLote(true);
            }
            else
            {
                svcEDI = new Servicos.GeracaoEDI(unitOfWork, layout, this.EmpresaUsuario, cpfCnpjRemetente, cpfCnpjDestinatario, dataInicial, dataFinal, codigoVeiculo, false, codigosCTes, codigoDuplicata, null, null);
                return svcEDI.GerarArquivo();
            }
        }

    }
}
