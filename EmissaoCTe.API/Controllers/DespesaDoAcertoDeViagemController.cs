using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DespesaDoAcertoDeViagemController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoAcertoViagem = 0;
                int.TryParse(Request.Params["CodigoAcertoViagem"], out codigoAcertoViagem);
                Repositorio.DespesaDoAcertoDeViagem repDespesaAcertoViagem = new Repositorio.DespesaDoAcertoDeViagem(unitOfWork);
                List<Dominio.Entidades.DespesaDoAcertoDeViagem> listaDespesas = repDespesaAcertoViagem.BuscarPorAcertoDeViagem(codigoAcertoViagem);
                var retorno = from obj in listaDespesas
                              select new Dominio.ObjetosDeValor.DespesaAcertoDeViagem
                              {
                                  Codigo = obj.Codigo,
                                  CodigoFornecedor = obj.Fornecedor != null ? obj.Fornecedor.CPF_CNPJ_Formatado : string.Empty,
                                  CodigoTipoDespesa = obj.TipoDespesa != null ? obj.TipoDespesa.Codigo : 0,
                                  Data = obj.Data != null ? obj.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  Descricao = obj.Descricao,
                                  DescricaoFornecedor = obj.Fornecedor != null ? string.Concat(obj.Fornecedor.CPF_CNPJ_Formatado, " - ", obj.Fornecedor.Nome) : obj.NomeFornecedor,
                                  DescricaoTipoDespesa = obj.TipoDespesa != null ? string.Concat(obj.TipoDespesa.Codigo, " - ", obj.TipoDespesa.Descricao) : string.Empty,
                                  Excluir = false,
                                  Observacao = obj.Observacao,
                                  Paga = obj.Paga,
                                  Quantidade = obj.Quantidade.ToString("n2"),
                                  ValorUnitario = obj.ValorUnitario.ToString("n2")
                              };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as despesas do acerto de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
