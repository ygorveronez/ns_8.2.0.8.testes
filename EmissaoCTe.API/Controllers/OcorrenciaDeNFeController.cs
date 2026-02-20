using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class OcorrenciaDeNFeController : ApiController
    {
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("ocorrenciasdenfe.aspx") select obj).FirstOrDefault();
        }


        #region Métodos Globais

        [AcceptVerbs("POST")]
        public ActionResult Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                string descricaoTipoOcorrencia = Request.Params["DescricaoTipoOcorrencia"];
                string observacaoOcorrencia = Request.Params["ObservacaoOcorrencia"];
                string numeroNFe = Request.Params["NumeroNFe"];

                int inicioRegistros = 0;
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);


                Repositorio.OcorrenciaDeNFe repOcorrenciaDeNFe = new Repositorio.OcorrenciaDeNFe(unitOfWork);

                List<Dominio.Entidades.OcorrenciaDeNFe> itens = repOcorrenciaDeNFe.Consultar(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNFe, inicioRegistros, 50);
                int totalRegistros = repOcorrenciaDeNFe.ContarConsulta(this.EmpresaUsuario.Codigo, descricaoTipoOcorrencia, observacaoOcorrencia, numeroNFe);

                var retorno = (from obj in itens
                               select
            new
            {
                obj.Codigo,
                DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                NFe = string.Concat(obj.NFe.Numero),
                DescricaoOcorrencia = obj.Ocorrencia.Descricao,
                obj.Ocorrencia.DescricaoTipo
            }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "NF-e|15", "Ocorrência|35", "Tipo da Ocorrência|20" }, totalRegistros);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao consultar as ocorrências.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.OcorrenciaDeNFe repOcorrenciaDeNFe = new Repositorio.OcorrenciaDeNFe(unitOfWork);

                int codigo = 0;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Dominio.Entidades.OcorrenciaDeNFe ocorrencia = repOcorrenciaDeNFe.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigo);

                if (ocorrencia == null)
                    return Json<bool>(false, false, "Ocorrência não encontrada.");

                var retorno = new
                {
                    ocorrencia.Codigo,
                    CodigoNFe = ocorrencia.NFe.Codigo,
                    DescricaoNFe = string.Concat(ocorrencia.NFe.Numero),
                    CodigoTipoOcorrencia = ocorrencia.Ocorrencia.Codigo,
                    DescricaoTipoOcorrencia = ocorrencia.Ocorrencia.Descricao,
                    DataDaOcorrencia = ocorrencia.DataDaOcorrencia.ToString("dd/MM/yyyy HH:mm"),
                    ocorrencia.Observacao
                };
                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter os detalhes da ocorrência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.OcorrenciaDeNFe repOcorrenciaNFe = new Repositorio.OcorrenciaDeNFe(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.XMLNotaFiscalEletronica repNFe = new Repositorio.XMLNotaFiscalEletronica(unitOfWork);

                if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                    return Json<bool>(false, false, "Permissão para inclusão negada.");

                int codigoNFe, codigoTipoOcorrencia = 0;
                int.TryParse(Request.Params["CodigoNFe"], out codigoNFe);
                int.TryParse(Request.Params["CodigoTipoOcorrencia"], out codigoTipoOcorrencia);

                DateTime dataDaOcorrencia;
                DateTime.TryParseExact(Request.Params["DataDaOcorrencia"], "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out dataDaOcorrencia);
                string observacao = Request.Params["Observacao"];

                if (dataDaOcorrencia == DateTime.MinValue)
                    return Json<bool>(false, false, "Data da ocorrência inválida.");

                Dominio.Entidades.XMLNotaFiscalEletronica NFe = repNFe.BuscarPorCodigo(codigoNFe);
                if (NFe == null)
                    return Json<bool>(false, false, "NF-e inválido.");

                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);
                if (tipoOcorrencia == null)
                    return Json<bool>(false, false, "Ocorrência inválida.");

                Dominio.Entidades.OcorrenciaDeNFe ocorrenciaDeNFe = new Dominio.Entidades.OcorrenciaDeNFe();

                ocorrenciaDeNFe.NFe = NFe;
                ocorrenciaDeNFe.DataDaOcorrencia = dataDaOcorrencia;
                ocorrenciaDeNFe.DataDeCadastro = DateTime.Now;
                ocorrenciaDeNFe.Observacao = observacao;
                ocorrenciaDeNFe.Ocorrencia = tipoOcorrencia;

                repOcorrenciaNFe.Inserir(ocorrenciaDeNFe);

                return Json(new
                {
                    Codigo = ocorrenciaDeNFe.Codigo
                }, true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar a ocorrência de NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarPorNFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoNFe, inicioRegistros = 0;
                int.TryParse(Request.Params["CodigoNFe"], out codigoNFe);
                int.TryParse(Request.Params["inicioRegistros"], out inicioRegistros);

                Repositorio.OcorrenciaDeNFe repOcorrenciaDeNFe = new Repositorio.OcorrenciaDeNFe(unitOfWork);
                List<Dominio.Entidades.OcorrenciaDeNFe> listaOcorrencias = repOcorrenciaDeNFe.ConsultarPorNFe(this.EmpresaUsuario.Codigo, codigoNFe, inicioRegistros, 50);
                int countOcorrencias = repOcorrenciaDeNFe.ContarConsultaPorNFe(this.EmpresaUsuario.Codigo, codigoNFe);

                var retorno = from obj in listaOcorrencias
                              select new
                              {
                                  obj.Codigo,
                                  DataDaOcorrencia = obj.DataDaOcorrencia.ToString("dd/MM/yyyy"),
                                  DescricaoOcorrencia = obj.Ocorrencia.Descricao,
                                  DescricaoTipoOcorrencia = obj.Ocorrencia.DescricaoTipo,
                                  obj.Observacao,
                              };

                return Json(retorno, true, null, new string[] { "Codigo", "Data|15", "Ocorrência|30", "Tipo|15", "Observação|40" }, countOcorrencias);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao buscar as ocorrências para esta NF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        
        


        #endregion

    }
}