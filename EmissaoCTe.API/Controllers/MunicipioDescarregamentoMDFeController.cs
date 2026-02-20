using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class MunicipioDescarregamentoMDFeController : ApiController
    {
        [AcceptVerbs("POST")]
        public ActionResult BuscarPorMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoMDFe = 0;
                int.TryParse(Request.Params["CodigoMDFe"], out codigoMDFe);

                Repositorio.MunicipioDescarregamentoMDFe repMunicipioDescarregamento = new Repositorio.MunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.InformacaoCargaCTE repInfoCarga = new Repositorio.InformacaoCargaCTE(unitOfWork);
                Repositorio.DocumentoMunicipioDescarregamentoMDFe repDocumento = new Repositorio.DocumentoMunicipioDescarregamentoMDFe(unitOfWork);
                Repositorio.NotaFiscalEletronicaMDFe repNFe = new Repositorio.NotaFiscalEletronicaMDFe(unitOfWork);
                Repositorio.CTeMDFe repCTeMDFe = new Repositorio.CTeMDFe(unitOfWork);

                List<Dominio.Entidades.MunicipioDescarregamentoMDFe> municipiosDescarregamento = repMunicipioDescarregamento.BuscarPorMDFe(codigoMDFe);

                var retorno = new List<Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe>();

                foreach (Dominio.Entidades.MunicipioDescarregamentoMDFe municipio in municipiosDescarregamento)
                {
                    List<Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe> documentos = repDocumento.BuscarPorMunicipio(municipio.Codigo);
                    List<Dominio.Entidades.NotaFiscalEletronicaMDFe> notasFiscais = repNFe.BuscarPorMunicipio(municipio.Codigo);
                    List<Dominio.Entidades.CTeMDFe> chaveCTesMDFe = repCTeMDFe.BuscarPorMunicipio(municipio.Codigo);

                    retorno.Add(new Dominio.ObjetosDeValor.MunicipioDescarregamentoMDFe()
                    {
                        Codigo = municipio.Codigo,
                        CodigoMunicipio = municipio.Municipio.Codigo,
                        DescricaoMunicipio = municipio.Municipio.Descricao,
                        Excluir = false,
                        Documentos = (from obj in documentos
                                      select new Dominio.ObjetosDeValor.DocumentoMunicipioDescarregamentoMDFe()
                                      {
                                          Codigo = obj.Codigo,
                                          Excluir = false,
                                          CTe = new Dominio.ObjetosDeValor.CTeMDFe()
                                          {
                                              Codigo = obj.CTe.Codigo,
                                              DataEmissao = obj.CTe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm"),
                                              LocalidadeInicioPrestacao = string.Concat(obj.CTe.LocalidadeInicioPrestacao.Estado.Sigla, " / ", obj.CTe.LocalidadeInicioPrestacao.Descricao),
                                              Numero = string.Concat(obj.CTe.Numero, " - ", obj.CTe.Serie.Numero),
                                              PesoTotal = repInfoCarga.ObterPesoTotal(obj.CTe.Codigo),
                                              ValorFrete = obj.CTe.ValorFrete,
                                              ValorTotalMercadoria = obj.CTe.ValorTotalMercadoria,
                                              ProdutosPerigosos = this.ProdutosPerigososPorDocumento(obj, unitOfWork)
                                          }
                                      }).ToList(),
                        NFes = (from obj in notasFiscais
                                select new Dominio.ObjetosDeValor.NotaFiscalEletronicaMDFe()
                                {
                                    Chave = obj.Chave,
                                    Codigo = obj.Codigo,
                                    SegundoCodigoDeBarra = !string.IsNullOrWhiteSpace(obj.SegundoCodigoDeBarra) ? obj.SegundoCodigoDeBarra : string.Empty,
                                    Excluir = false
                                }).ToList(),
                        ChaveCTes = (from obj in chaveCTesMDFe
                                     select new Dominio.ObjetosDeValor.ChaveCTes()
                                     {
                                         Chave = obj.Chave,
                                         Codigo = obj.Codigo,
                                         Excluir = false
                                     }).ToList()
                    });
                }

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os municípios de descarregamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private List<Dominio.ObjetosDeValor.MDFeProdutosPerigosos> ProdutosPerigososPorDocumento(Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos repProdPerigosos = new Repositorio.DocumentoMunicipioDescarregamentoMDFeProdPerigosos(unitOfWork);

            var produtos = from obj in (repProdPerigosos.BuscarPorDocumento(documento.Codigo))
                           select new Dominio.ObjetosDeValor.MDFeProdutosPerigosos()
                           {
                               Id = obj.Codigo,
                               NumeroONU = obj.NumeroONU,
                               NomeApropriado = obj.Nome,
                               ClasseRisco = obj.ClasseRisco,
                               GrupoEmbalagem = obj.GrupoEmbalagem,
                               QuantidadeTotal = obj.QuantidadeTotalProduto,
                               QuantidadeETipo = obj.QuantidadeTipoVolumes,
                               Excluir = false,
                           };

            return produtos.ToList();
        }

    }
}
