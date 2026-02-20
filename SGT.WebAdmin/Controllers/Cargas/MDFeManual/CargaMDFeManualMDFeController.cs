using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualMDFeController : BaseController
    {
		#region Construtores

		public CargaMDFeManualMDFeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        public async Task<IActionResult> ConsultarCargaMDFe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);

                int status = -1;
                if (!string.IsNullOrWhiteSpace(Request.Params("Status")))
                    int.TryParse(Request.Params("Status"), out status);

                int cargaMDFeManual = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeManual")))
                    int.TryParse(Request.Params("CargaMDFeManual"), out cargaMDFeManual);
                else if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeAquaviario")))
                    int.TryParse(Request.Params("CargaMDFeAquaviario"), out cargaMDFeManual);

                Dominio.Enumeradores.StatusMDFe statusMDFe = (Dominio.Enumeradores.StatusMDFe)status;


                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMDFE", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Status", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Serie", "Serie", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Data da Emissão", "Emissao", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarga", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDesgarga", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Status", "DescricaoStatus", 8, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Retorno Sefaz", "RetornoSefaz", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Importado", false);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                propOrdenacao = "MDFe." + propOrdenacao;


                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe> cargaMDFes = repCargaMDFeManualMDFe.ConsultarMDFe(cargaMDFeManual, statusMDFe, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCargaMDFeManualMDFe.ContarConsultaMDFe(cargaMDFeManual, statusMDFe));
                var lista = (from obj in cargaMDFes
                             select new
                             {
                                 obj.Codigo,
                                 CodigoMDFE = obj.MDFe.Codigo,
                                 CodigoEmpresa = obj.MDFe.Empresa.Codigo,
                                 obj.MDFe.Numero,
                                 Serie = obj.MDFe.Serie.Numero,
                                 Emissao = obj.MDFe.DataEmissao.HasValue ? obj.MDFe.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                                 UFCarga = obj.MDFe.EstadoCarregamento.Nome,
                                 UFDesgarga = obj.MDFe.EstadoDescarregamento.Nome,
                                 DescricaoStatus = obj.MDFe.DescricaoStatus,
                                 Status = obj.MDFe.Status,
                                 RetornoSefaz = obj.MDFe.MensagemStatus != null ? obj.MDFe.MensagemStatus.MensagemDoErro : obj.MDFe.MensagemRetornoSefaz,
                                 obj.MDFe.Importado,
                                 DT_RowColor = obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado ? "#dff0d8" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao ? "rgba(193, 101, 101, 1)" : obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado ? "#777" : "",
                                 DT_FontColor = (obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || obj.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Cancelado) ? "#FFFFFF" : ""
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EmitirNovamente()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoMDFE, codigoEmpresa = 0;
                int.TryParse(Request.Params("CodigoMDFE"), out codigoMDFE);
                int.TryParse(Request.Params("CodigoEmpresa"), out codigoEmpresa);

                if (codigoMDFE > 0)
                {
                    unitOfWork.Start();

                    Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe repCargaMDFeManualMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualMDFe(unitOfWork);
                    Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unitOfWork);

                    Servicos.MDFe svcMDFe = new Servicos.MDFe(unitOfWork);

                    Servicos.Embarcador.Carga.MDFe svcCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unitOfWork);
                    Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFE, codigoEmpresa);

                    if (mdfe != null)
                    {
                        if (mdfe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || mdfe.Status == Dominio.Enumeradores.StatusMDFe.EmDigitacao)
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaCargaMDFeManualMDFe = repCargaMDFeManualMDFe.BuscarPorMDFe(mdfe.Codigo);
                            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(cargaCargaMDFeManualMDFe.CargaMDFeManual.Codigo);

                            if (cargaMDFeManual != null)
                            {
                                if (!string.IsNullOrWhiteSpace(Request.Params("Percurso")))
                                {
                                    dynamic percursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Percurso"));
                                    if (percursos != null)
                                        SalvarPercurso(ref cargaMDFeManual, unitOfWork);
                                }

                                if (cargaCargaMDFeManualMDFe != null)
                                {

                                    if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                                    {
                                        TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                                        mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                                    }

                                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                                    repCargaMDFeManual.Atualizar(cargaMDFeManual);

                                    AtualizarPercursoMDFe(ref mdfe, cargaMDFeManual, unitOfWork);
                                    mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmDigitacao;

                                    svcCargaMDFe.AtualizarANTT(ref mdfe, TipoServicoMultisoftware, unitOfWork);

                                    repMDFe.Atualizar(mdfe);

                                    unitOfWork.CommitChanges();

                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCargaMDFeManualMDFe, null, "Solicitou emissão do MDF-e", unitOfWork);
                                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaCargaMDFeManualMDFe.MDFe, null, "Solicitou emissão do MDF-e", unitOfWork);

                                    bool sucesso = svcMDFe.Emitir(mdfe, unitOfWork);
                                    if (!sucesso)
                                    {
                                        cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.Rejeicao;
                                        repCargaMDFeManual.Atualizar(cargaMDFeManual);

                                        return new JsonpResult(false, true, "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");
                                    }
                                }
                                else
                                {
                                    unitOfWork.Rollback();
                                    return new JsonpResult(false, true, "O MDF-e informado não pertence a uma carga.");
                                }
                            }
                            else
                            {
                                unitOfWork.Rollback();
                                return new JsonpResult(false, true, "O MDF-e informado não pertence a uma carga.");
                            }
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, "A atual situação do MDF-e (" + mdfe.DescricaoStatus + ") não permite sua emissão.");
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, "O MDF-e informado não foi localizado");
                    }
                }
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar emitir o MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReemitirMDFesRejeitados()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCargaMDFeManual = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeManual")))
                    int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);
                else if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeAquaviario")))
                    int.TryParse(Request.Params("CargaMDFeAquaviario"), out codigoCargaMDFeManual);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);
                Servicos.Embarcador.Carga.MDFe svcCargaMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);
                //Servicos.Embarcador.Hubs.Carga svcHubCarga = new Servicos.Embarcador.Hubs.Carga();

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);

                if (!string.IsNullOrWhiteSpace(Request.Params("Percurso")))
                {
                    dynamic percursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Percurso"));
                    if (percursos != null)
                        SalvarPercurso(ref cargaMDFeManual, unidadeTrabalho);
                }

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe in cargaMDFeManual.MDFeManualMDFes.ToList())
                {
                    if (cargaMDFeManualMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Rejeicao || cargaMDFeManualMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmDigitacao)
                    {
                        Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(cargaMDFeManualMDFe.MDFe.Codigo);
                        if (mdfe != null)
                        {
                            if (mdfe.DataEmissao < DateTime.Now.AddDays(-1))
                            {
                                TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(mdfe.Empresa.FusoHorario);
                                mdfe.DataEmissao = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, fusoHorarioEmpresa);
                            }

                            AtualizarPercursoMDFe(ref mdfe, cargaMDFeManual, unidadeTrabalho);
                            mdfe.Status = Dominio.Enumeradores.StatusMDFe.EmDigitacao;
                            svcCargaMDFe.AtualizarANTT(ref mdfe, TipoServicoMultisoftware, unidadeTrabalho);
                            repMDFe.Atualizar(mdfe);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualMDFe, null, "Solicitou emissão do MDF-e", unidadeTrabalho);
                            Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualMDFe.MDFe, null, "Solicitou emissão do MDF-e", unidadeTrabalho);

                            if (!svcMDFe.Emitir(mdfe, unidadeTrabalho))
                                return new JsonpResult(false, true, "O MDF-e nº " + mdfe.Numero.ToString() + " da empresa " + mdfe.Empresa.CNPJ + " foi salvo, porém, ocorreu uma falha ao emiti-lo.");

                        }
                    }
                }

                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);

                //svcHubCarga.InformarCargaAtualizada(carga.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAcaoCarga.Alterada, _conexao.StringConexao);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao tentar reemitir os MDF-es.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosParaEncerramentoPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Request.Params("codigo"));

                Dominio.ObjetosDeValor.Embarcador.Carga.DadosEncerramentoMDFe dadosEncerramentoMDFe = Servicos.Embarcador.Carga.MDFe.ObterDadosEncerramentoMDFe(codigo, unidadeTrabalho);

                var encerramentoMDF = new
                {
                    dadosEncerramentoMDFe.Codigo,
                    Estado = dadosEncerramentoMDFe.Estado.Nome.Trim() + " - " + dadosEncerramentoMDFe.Estado.Sigla,
                    DataEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("dd/MM/yyyy"),
                    HoraEncerramento = dadosEncerramentoMDFe.DataEncerramento.ToString("HH:mm"),
                    Localidades = (from obj in dadosEncerramentoMDFe.Localidades
                                   select new
                                   {
                                       Codigo = obj.Codigo,
                                       Descricao = obj.Descricao
                                   }).ToList()
                };

                return new JsonpResult(encerramentoMDF);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados para encerramento do MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> EncerrarMDFe()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Repositorio.Embarcador.Configuracoes.IntegracaoIntercab repIntegracaoIntercab = new Repositorio.Embarcador.Configuracoes.IntegracaoIntercab(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoIntercab integracaoIntercab = repIntegracaoIntercab.BuscarIntegracao();
                int.TryParse(Request.Params("Codigo"), out int codigo);                
                int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);

                int codigoCargaMDFeManual = 0;
                if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeManual")))
                    int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);
                else if (!string.IsNullOrWhiteSpace(Request.Params("CargaMDFeAquaviario")))
                    int.TryParse(Request.Params("CargaMDFeAquaviario"), out codigoCargaMDFeManual);

                DateTime.TryParseExact(Request.Params("DataEncerramento") + " " + Request.Params("HoraEncerramento"), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataEncerramento);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);

                if ((mdfe.Importado || cargaMDFeManual.MDFeRecebidoDeIntegracao) && (integracaoIntercab?.AtivarIntegracaoMDFeAquaviario ?? false))
                    return new JsonpResult(false, true, "Não é possível executar esta operação para MDF-e recebido da integração.");

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Solicitou Encerramento do MDF-e", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, null, "Solicitou Encerramento do MDF-e " + mdfe.Descricao, unidadeTrabalho);

                if (!Servicos.Embarcador.Carga.MDFe.EncerrarMDFe(out string erro, codigo, codigoLocalidade, dataEncerramento, WebServiceConsultaCTe, this.Usuario, unidadeTrabalho, _conexao.StringConexao, Auditado))
                    return new JsonpResult(false, true, erro);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao encerrar o MDF-e.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarPercurso(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeTrabalho);

            dynamic percursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Percurso"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursosExistentes = repPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (percursosExistentes.Count > 0)
            {
                List<string> estados = new List<string>();

                foreach (var percurso in percursos)
                    estados.Add((string)percurso.Estado);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursosDeletar = (from obj in percursosExistentes where !estados.Contains(obj.Estado.Sigla) select obj).ToList();

                for (var i = 0; i < percursosDeletar.Count; i++)
                    repPercurso.Deletar(percursosDeletar[i]);
            }

            foreach (var percurso in percursos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso perc = repPercurso.BuscarPorEstadoECargaMDFeManual(cargaMDFeManual.Codigo, (string)percurso.Sigla);

                if (perc == null)
                    perc = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso();

                perc.CargaMDFeManual = cargaMDFeManual;
                perc.Estado = repEstado.BuscarPorSigla((string)percurso.Sigla);
                perc.Ordem = (int)percurso.Posicao;

                if (perc.Codigo > 0)
                    repPercurso.Atualizar(perc);
                else
                    repPercurso.Inserir(perc);
            }
        }

        private bool AtualizarPercursoMDFe(ref Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            Repositorio.PercursoMDFe repPercursoMDFe = new Repositorio.PercursoMDFe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repCargaMDFeManualPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);

            List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem> passagens = new List<Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem>();

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percuros = repCargaMDFeManualPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso percurso in percuros)
            {
                if (percurso.Estado.Sigla == mdfe.EstadoDescarregamento.Sigla)
                    break;

                passagens.Add(new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = percurso.Ordem, Sigla = percurso.Estado.Sigla });
            }

            List<Dominio.Entidades.PercursoMDFe> percursosMDFe = repPercursoMDFe.BuscarPorMDFe(mdfe.Codigo);
            for (var i = 0; i < percursosMDFe.Count; i++)
                repPercursoMDFe.Deletar(percursosMDFe[i]);

            Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);
            svcMDFe.GerarPercursos(mdfe, unidadeTrabalho, null, passagens, configuracaoTMS);

            return true;
        }


        #endregion
    }
}
