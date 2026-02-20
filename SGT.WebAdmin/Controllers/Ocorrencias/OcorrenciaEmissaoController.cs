using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/Ocorrencia")]
    public class OcorrenciaEmissaoController : BaseController
    {
		#region Construtores

		public OcorrenciaEmissaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> SalvarOutrosDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoOcorrencia = Request.GetIntParam("CodigoOcorrencia");
                int numero = Request.GetIntParam("Numero");
                int serie = Request.GetIntParam("Serie");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.EmpresaSerie repositorioEmpresaSerie = new Repositorio.EmpresaSerie(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.PendenciaEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCTe.BuscarPorCodigo(codigoCTe);
                if (cte == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao);

                if (cte.Numero == numero && cte.Serie.Codigo == serie)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.NaoEPermitidoSalvarSemAlterarAsInformacoes);

                unitOfWork.Start();

                int numeroAnterior = cte.Numero;
                int serieAnterior = cte.Serie.Numero;
                cte.Numero = numero;
                cte.Serie = repositorioEmpresaSerie.BuscarPorCodigo(serie);
                if (cte.Serie == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                int quantidadeNFSes = repositorioCTe.ContarCTePorChaveUnica(cte.Numero, cte.Serie.Codigo, cte.ModeloDocumentoFiscal.Codigo, cte.Empresa.Codigo, cte.TipoAmbiente);
                cte.TipoControle = quantidadeNFSes + 1;
                cte.Status = "A";

                repositorioCTe.Atualizar(cte);

                string descricaoAuditoria = $"Alterou o número do documento complementar de {numeroAnterior}-{serieAnterior} para {numero}-{cte.Serie.Numero}";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, $"{descricaoAuditoria} via ocorrência", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, descricaoAuditoria, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarOutrosDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCTe = Request.GetIntParam("CodigoCTe");
                int codigoOcorrencia = Request.GetIntParam("CodigoOcorrencia");

                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia);
                if (ocorrencia == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.PendenciaEmissao)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao);

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repositorioCTe.BuscarPorCodigo(codigoCTe);
                if (cte == null)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NaoFoiPossivelEncontrarRegistro);

                if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros)
                    return new JsonpResult(false, true, Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao);

                unitOfWork.Start();

                cte.Status = "A";
                repositorioCTe.Atualizar(cte);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cte, "Autorizou o documento complementar via ocorrência", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, $"Autorizou o documento complementar {cte.Numero}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AutorizarOutrosDocumentosPorOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoOcorrencia = Request.GetIntParam("CodigoOcorrencia");

                Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo repositorioCargaCTeComplementoInfo = new Repositorio.Embarcador.Cargas.CargaCTeComplementoInfo(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.ConhecimentoDeTransporteEletronico repositorioCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> cargasCTesComplementoInfo = repositorioCargaCTeComplementoInfo.BuscarCTesOutrosDocumentosPendentesPorOcorrencia(codigoOcorrencia);
                if (cargasCTesComplementoInfo.Count == 0)
                    return new JsonpResult(false, true, Localization.Resources.Gerais.Geral.NenhumRegistroEncontrado);

                unitOfWork.Start();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo cargaCTeComplementoInfo in cargasCTesComplementoInfo)
                {
                    if (cargaCTeComplementoInfo.CargaCTeComplementado?.Carga?.PossuiPendencia ?? false)
                    {
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.PossuiPendencia = false;
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.problemaCTE = false;
                        cargaCTeComplementoInfo.CargaCTeComplementado.Carga.MotivoPendencia = "";
                        repositorioCarga.Atualizar(cargaCTeComplementoInfo.CargaCTeComplementado.Carga);
                    }

                    if (cargaCTeComplementoInfo.CTe == null)
                        continue;

                    if (cargaCTeComplementoInfo.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.Outros)
                        continue;

                    cargaCTeComplementoInfo.CTe.Status = "A";
                    repositorioCTe.Atualizar(cargaCTeComplementoInfo.CTe);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCTeComplementoInfo.CTe, "Autorizou o documento complementar em lote via ocorrência", unitOfWork);
                }

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = repositorioOcorrencia.BuscarPorCodigo(codigoOcorrencia);

                if (ocorrencia.SituacaoOcorrencia != SituacaoOcorrencia.PendenciaEmissao)
                    throw new ControllerException(Localization.Resources.Ocorrencias.Ocorrencia.SituacaoNaoPermiteEmissao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, ocorrencia, $"Autorizou todos os documentos complementares pendentes", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoSalvar);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
