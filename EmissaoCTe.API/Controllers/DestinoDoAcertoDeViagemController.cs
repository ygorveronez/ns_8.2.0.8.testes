using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class DestinoDoAcertoDeViagemController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorAcertoDeViagem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoAcertoViagem = 0;
                int.TryParse(Request.Params["CodigoAcertoViagem"], out codigoAcertoViagem);
                Repositorio.DestinoDoAcertoDeViagem repDestinoAcertoViagem = new Repositorio.DestinoDoAcertoDeViagem(unitOfWork);
                List<Dominio.Entidades.DestinoDoAcertoDeViagem> listaDestinos = repDestinoAcertoViagem.BuscarPorAcertoDeViagem(codigoAcertoViagem);
                var retorno = from obj in listaDestinos
                              select new Dominio.ObjetosDeValor.DestinoAcertoDeViagem
                              {
                                  Codigo = obj.Codigo,
                                  CodigoCTe = obj.CTe != null ? obj.CTe.Codigo : 0,
                                  CodigoCliente = obj.Cliente != null ? obj.Cliente.CPF_CNPJ_SemFormato : "",
                                  CodigoTipoCarga = obj.TipoCarga != null ? obj.TipoCarga.Codigo : 0,
                                  DataFinal = obj.DataFinal != null ? obj.DataFinal.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DataInicial = obj.DataInicial != null ? obj.DataInicial.Value.ToString("dd/MM/yyyy") : string.Empty,
                                  DescricaoCTe = obj.CTe != null ? string.Concat(obj.CTe.Numero, " - ", obj.CTe.Serie.Numero) : string.Empty,
                                  DescricaoCliente = obj.Cliente != null ? obj.Cliente.Nome : string.Empty,
                                  DescricaoDestino = obj.Destino != null ? string.Concat(obj.Destino.Descricao, " / ", obj.Destino.Estado.Sigla) : string.Empty,
                                  DescricaoOrigem = obj.Origem != null ? string.Concat(obj.Origem.Descricao, " / ", obj.Origem.Estado.Sigla) : string.Empty,
                                  UFDestino = obj.Destino != null ? obj.Destino.Estado.Sigla : string.Empty,
                                  UFOrigem = obj.Origem != null ? obj.Origem.Estado.Sigla : string.Empty,
                                  DescricaoTipoCarga = obj.TipoCarga != null ? string.Concat(obj.TipoCarga.Codigo, " - ", obj.TipoCarga.Descricao) : string.Empty,
                                  Excluir = false,
                                  KMFinal = obj.KilometragemFinal,
                                  KMInicial = obj.KilometragemInicial,
                                  MunicipioDestino = obj.Destino != null ? obj.Destino.Codigo : 0,
                                  MunicipioOrigem = obj.Origem != null ? obj.Origem.Codigo : 0,
                                  Peso = obj.PesoCarga.ToString("n4"),
                                  Observacao = obj.Observacao ?? string.Empty,
                                  ValorFrete = obj.ValorFrete.ToString("n2"),
                                  ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                  OutrosDescontos = obj.OutrosDescontos.ToString("n2")
                              };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os destinos do acerto de viagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
