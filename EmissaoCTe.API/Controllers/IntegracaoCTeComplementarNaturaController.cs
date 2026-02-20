using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class IntegracaoCTeComplementarNaturaController : ApiController
    {

        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("integracaoctecomplementarnatyra.aspx") select obj).FirstOrDefault();
        }


        public ActionResult AdicionarCTeAoDocumentoDeTransporte()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoCTe, codigoDt;
                int.TryParse(Request.Params["CodigoCTe"], out codigoCTe);
                int.TryParse(Request.Params["CodigoDt"], out codigoDt);

                decimal valorTotalFrete = 0;

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.DocumentoTransporteNatura repDocumentoTransporte = new Repositorio.DocumentoTransporteNatura(unitOfWork);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoCTe);
                Dominio.Entidades.DocumentoTransporteNatura documentoTransporte = repDocumentoTransporte.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoDt);

                if (cte == null)
                    return Json<bool>(false, false, "O CT-e não foi encontrado.");

                if (documentoTransporte == null)
                    return Json<bool>(false, false, "Documento de Transporte não foi encontrado.");

                //if (cte.Status != "A")
                //    return Json<bool>(false, false, "O status do CT-e não permite que ele seja vinculado à um Documento de Transporte.");

                Repositorio.NotaFiscalDocumentoTransporteNatura repNotasDt = new Repositorio.NotaFiscalDocumentoTransporteNatura(unitOfWork);
                Dominio.Entidades.NotaFiscalDocumentoTransporteNatura notasDt = repNotasDt.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);

                if (notasDt != null)
                    return Json<bool>(false, false, "O CT-e já está vinculado ao documento de transporte " + notasDt.DocumentoTransporte.NumeroDT + ".");

                List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura> documentosDt = new List<Dominio.Entidades.NotaFiscalDocumentoTransporteNatura>();
                documentosDt = repNotasDt.BuscarPorDocumentoTransporte(codigoDt);

                for (var i = 0; i < documentosDt.Count; i++)
                {
                    if (documentosDt[i].CTe == null && documentosDt[i].NFSe == null)
                        repNotasDt.Deletar(documentosDt[i]);
                    else
                        valorTotalFrete += documentosDt[i].ValorFrete;
                }

                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.DocumentosCTE repDocumentoCTe = new Repositorio.DocumentosCTE(unitOfWork);
                List<Dominio.Entidades.DocumentosCTE> documentosCTe = null;

                if (cte.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento)
                    documentosCTe = repDocumentoCTe.BuscarPorChaveCTe(this.EmpresaUsuario.Codigo, cte.ChaveCTESubComp);
                else
                    documentosCTe = repDocumentoCTe.BuscarPorCTe(this.EmpresaUsuario.Codigo, cte.Codigo);

                if (documentosCTe != null)
                {
                    for (var i = 0; i < documentosCTe.Count; i++)
                    {
                        //Solicitado pela Juliana a remoção desta validação
                        if (documentosCTe[i].ChaveNFE == null || documentosCTe[i].ChaveNFE == "")
                            return Json<bool>(false, false, "CT-e não possui nota fiscal eletrônica para vincular ao Documento de Transporte.");

                        notasDt = new Dominio.Entidades.NotaFiscalDocumentoTransporteNatura();

                        notasDt.CTe = cte;
                        notasDt.XML = string.Empty;
                        notasDt.DocumentoTransporte = documentoTransporte;
                        notasDt.Numero = int.Parse(documentosCTe[i].Numero);
                        notasDt.Peso = documentosCTe[i].Peso;
                        notasDt.Quantidade = documentosCTe[i].Volume;
                        notasDt.Serie = documentosCTe[i].Serie != null ? int.Parse(documentosCTe[i].Serie) : documentosCTe[i].ChaveNFE != null ? int.Parse(documentosCTe[i].ChaveNFE.Substring(22, 3)) : 0;
                        notasDt.Valor = documentosCTe[i].Valor;
                        notasDt.Chave = documentosCTe[i].ChaveNFE != null ? documentosCTe[i].ChaveNFE : "0";
                        notasDt.DataEmissao = documentosCTe[i].DataEmissao;
                        notasDt.ValorFrete = documentosCTe.Count == 1 ? cte.ValorFrete : cte.ValorFrete / documentosCTe.Count;
                        valorTotalFrete += documentosCTe.Count == 1 ? cte.ValorFrete : cte.ValorFrete / documentosCTe.Count;
                        notasDt.Status = Dominio.ObjetosDeValor.Enumerador.StatusNotaFiscalNatura.Emitido;
                        notasDt.Emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Remetente.CPF_CNPJ));
                        notasDt.Destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(cte.Destinatario.CPF_CNPJ));
                        notasDt.TipoPagamento = cte.TipoPagamento;

                        repNotasDt.Inserir(notasDt);
                    }
                }
                else
                    return Json<bool>(false, false, "CT-e não possui nota fiscal para vincular ao Documento de Transporte.");

                documentoTransporte.Status = Dominio.ObjetosDeValor.Enumerador.StatusDocumentoTransporteNatura.Emitido;
                documentoTransporte.Motorista = cte.Motoristas.Count > 0 && cte.Motoristas[0] != null ? repMotorista.BuscarMotoristaPorCPF(cte.Motoristas[0].CPFMotorista) : null;
                documentoTransporte.Veiculo = cte.Veiculos.Count > 0 && cte.Veiculos[0] != null ? repVeiculo.BuscarPorPlaca(cte.Veiculos[0].Placa) : null;
                documentoTransporte.ValorFrete = valorTotalFrete;

                repDocumentoTransporte.Atualizar(documentoTransporte);

                return Json<bool>(true, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao vincular o CT-e ao documento de transporte.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}