using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class SubcontratacaoController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult ConsultarAdmin()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unidadeDeTrabalho);

                DateTime.TryParseExact(Request.Params["DataEmissaoInicial"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(Request.Params["DataEmissaoFinal"], "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);
                int.TryParse(Request.Params["inicioRegistros"], out int inicioRegistros);
                int.TryParse(Request.Params["Empresa"], out int empresa);

                string status = Request.Params["Status"];

                List<Dominio.Entidades.Subcontratacao> listaDocumentos = repSubcontratacao.Consultar(empresa, dataInicial, dataFinal, status, inicioRegistros, 50);
                int countDocumentos = repSubcontratacao.ContarConsulta(empresa, dataInicial, dataFinal, status);

                var retorno = (from obj in listaDocumentos
                               select new
                               {
                                   obj.Codigo,
                                   CodigoCTe = obj.DocumentoSubcontratacao?.Codigo ?? 0,
                                   EmpresaCTe = obj.DocumentoSubcontratacao?.Empresa.Codigo ?? 0,
                                   CodigoTransporte = obj.CodigoProcessoTransporte,
                                   DataImportacao = obj.DataImportacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                   ContratanteCNPJ = obj.Empresa?.Codigo > 3 ? obj.Empresa?.CNPJ_Formatado : string.Empty,
                                   Contratante = obj.Empresa?.Codigo > 3 ? obj.Empresa?.Descricao : string.Empty,
                                   Status = obj.Situacao == Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento ? obj.DescricaoSituacao : obj.DocumentoSubcontratacao == null ? obj.DescricaoSituacao : obj.DocumentoSubcontratacao.DescricaoStatus,
                                   Contratado = obj.EmpresaSubcontratada != null ? obj.EmpresaSubcontratada?.Descricao : string.Empty,
                                   Numero = obj.DocumentoSubcontratacao?.Numero.ToString() ?? string.Empty,
                                   Valor = obj.ValorFrete.ToString("n2") ?? "0,00",
                                   Mensagem = obj.DescricaoFalha
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "CodigoCTe", "EmpresaCTe", "Cod. Transporte|10", "Data Emissão|10", "CNPJ Contr.|10", "Contratante|10", "Status|10", "Contratado|15", "Numero|10", "Valor|10", "Mensagem|17" }, countDocumentos);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar documentos.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Reprocessar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.Subcontratacao repSubcontratacao = new Repositorio.Subcontratacao(unidadeDeTrabalho);
                Dominio.Entidades.Subcontratacao subcontratacao = repSubcontratacao.BuscarPorCodigo(codigo);

                if (subcontratacao != null)
                {
                    if (subcontratacao.Situacao != Dominio.Enumeradores.SituacaoSubcontratacao.FalhaProcessamento)
                        return Json<bool>(true, false, "Situação não permite reprocessar.");

                    subcontratacao.Situacao = Dominio.Enumeradores.SituacaoSubcontratacao.AgProcessamento;
                    repSubcontratacao.Atualizar(subcontratacao);                    
                }
                else
                    return Json<bool>(false, false, "Registro não encontrada.");

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                unidadeDeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha.");
            }
        }
    }
}