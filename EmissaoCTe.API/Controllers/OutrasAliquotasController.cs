using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OutrasAliquotasController : ApiController
    {

        public class BuscaDeAliquota
        {
            public decimal? Aliquota { get; set; }
            public decimal? Reducao { get; set; }
            public decimal? Efetiva { get; set; }
            public decimal? Valor { get; set; }

            public static BuscaDeAliquota Retorno(decimal aliquota, decimal reducao, decimal efetiva, decimal valorBaseCalculo) =>
                new BuscaDeAliquota { Aliquota = aliquota, Reducao = reducao, Efetiva = efetiva, Valor = ArredondarNumero(valorBaseCalculo, 2) };
        }

        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult BuscarCstClassificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                string cst = Request.Params["CST"];
                List<Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas> listaOutrasAliquotas = new Repositorio.Embarcador.OutrasAliquotas.OutrasAliquotas(unitOfWork).BuscarTodosOuPorCST(cst);

                var resultado = from obj in listaOutrasAliquotas
                                select new
                                {
                                    obj.CST,
                                    obj.CodigoClassificacaoTributaria,
                                    obj.Descricao,
                                    obj.Codigo
                                };

                return Json(resultado, true, null, new string[] { "CST|10", "ClassTrib|20", "Descrição|60", "OutrasAliquotas|0" }, resultado.Count());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as CST/ClassTrib. Tente novamente!");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarAliquotasPorClassificacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            Servicos.Embarcador.Imposto.ImpostoIBSCBS servicoImpostoIBSCBS = new Servicos.Embarcador.Imposto.ImpostoIBSCBS(unitOfWork);

            try
            {
                decimal baseCalculo = 0;
                int.TryParse(Request.Params["codiMunicipioDestino"], out int codigoMunicipioDestino);
                int.TryParse(Request.Params["codigoOutrasAliquotas"], out int codigoOutrasAliquotas);
                decimal.TryParse(Request.Params["baseCalculo"], NumberStyles.Number, CultureInfo.InvariantCulture, out baseCalculo);
                bool.TryParse(Request.Params["naoReduzirPisCofins"], out bool naoReduzirPisCofins);

                Dominio.Entidades.Localidade municipioDestino = new Repositorio.Localidade(unitOfWork).BuscarPorCodigo(codigoMunicipioDestino);
                Dominio.ObjetosDeValor.Embarcador.Imposto.RetornoImpostoIBSCBS impostoIBSCBSTotal = servicoImpostoIBSCBS.ObterImpostoIBSCBS(new Dominio.ObjetosDeValor.Embarcador.Imposto.ParametroCalculoIBSCBS
                {
                    BaseCalculo = baseCalculo,
                    CodigoLocalidade = municipioDestino.Codigo,
                    SiglaUF = municipioDestino.Estado.Sigla,
                    CodigoTipoOperacao = 0,
                    Empresa = this.EmpresaUsuario,
                    CodigoOutrasAliquotas = codigoOutrasAliquotas,
                    NaoReduzirPisCofins = naoReduzirPisCofins
                });

                if (impostoIBSCBSTotal == null || String.IsNullOrWhiteSpace(impostoIBSCBSTotal.CST) || String.IsNullOrWhiteSpace(impostoIBSCBSTotal.ClassificacaoTributaria))
                    return Json<bool>(false, false, "Não foi possível localizar as alíquotas de IBS/CBS.");

                decimal aliquotaEfetivaIbsMunicipal = impostoIBSCBSTotal.AliquotaIBSMunicipal - (impostoIBSCBSTotal.AliquotaIBSMunicipal * (impostoIBSCBSTotal.PercentualReducaoIBSMunicipal / 100));
                decimal aliquotaEfetivaIbsEstadual = impostoIBSCBSTotal.AliquotaIBSEstadual - (impostoIBSCBSTotal.AliquotaIBSEstadual * (impostoIBSCBSTotal.PercentualReducaoIBSEstadual / 100));
                decimal aliquotaEfetivaCbs = impostoIBSCBSTotal.AliquotaCBS - (impostoIBSCBSTotal.AliquotaCBS * (impostoIBSCBSTotal.PercentualReducaoCBS / 100));

                var resultado = new
                {
                    baseCalculo = impostoIBSCBSTotal.BaseCalculo,
                    ibs = new
                    {
                        municipal = BuscaDeAliquota.Retorno(impostoIBSCBSTotal.AliquotaIBSMunicipal, impostoIBSCBSTotal.PercentualReducaoIBSMunicipal, aliquotaEfetivaIbsMunicipal, impostoIBSCBSTotal.ValorIBSMunicipal),
                        estadual = BuscaDeAliquota.Retorno(impostoIBSCBSTotal.AliquotaIBSEstadual, impostoIBSCBSTotal.PercentualReducaoIBSEstadual, aliquotaEfetivaIbsEstadual, impostoIBSCBSTotal.ValorIBSEstadual)
                    },
                    cbs = BuscaDeAliquota.Retorno(impostoIBSCBSTotal.AliquotaCBS, impostoIBSCBSTotal.PercentualReducaoCBS, aliquotaEfetivaCbs, impostoIBSCBSTotal.ValorCBS)
                };

                return Json(resultado, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as alíquotas de IBS e CBS.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos privados

        public static decimal ArredondarNumero(decimal numero, int casasDecimais, string ceilOrFloor = "round")
        {
            decimal fator = (decimal)Math.Pow(10, casasDecimais);

            return ceilOrFloor.ToLower() switch
            {
                "ceil" => Math.Ceiling(numero * fator) / fator,
                "floor" => Math.Floor(numero * fator) / fator,
                _ => Math.Round(numero, casasDecimais, MidpointRounding.AwayFromZero)
            };
        }

        #endregion
    }
}