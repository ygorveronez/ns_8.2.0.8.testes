using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Documentos
{
    [CustomAuthorize(new string[] { "ObterDetalhes", "RegrasAprovacao" }, "Documentos/ControleDocumento")]
    public class ControleDocumentoController : BaseController
    {
        #region Construtores

        public ControleDocumentoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisa());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            try
            {
                var grid = ObterGridPesquisa();

                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

                return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = repositorioControleDocumento.BuscarPorCodigo(codigoControleDocumento);
                Servicos.Embarcador.CTe.ComponenteFrete servicoComponenteFrete = new Servicos.Embarcador.CTe.ComponenteFrete(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(codigoControleDocumento);

                if (controleDocumento == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");
                if (controleDocumento.CargaCTe.PreCTe == null)
                    return new JsonpResult(false, "Não foi possível encontrar o Pré-CTe");

                string notasCTe = string.Join(",", (from obj in controleDocumento.CTe.Documentos orderby obj.Numero select obj.Numero).ToList()).Left(47);
                string notasPreCTe = string.Join(",", (from obj in controleDocumento.CargaCTe.PreCTe.Documentos orderby obj.Numero select obj.Numero).ToList()).Left(47);

                var retorno = new
                {
                    CTe = new
                    {
                        controleDocumento.CTe.Codigo,
                        CodigoEmpresa = controleDocumento.CTe.Empresa.Codigo,
                        ValorAReceber = controleDocumento.CTe.ValorAReceber.ToString("n2"),
                        AliquotaICMS = controleDocumento.CTe.AliquotaICMS.ToString("n2"),
                        BaseCalculoICMS = controleDocumento.CTe.BaseCalculoICMS.ToString("n2"),
                        ValorICMS = controleDocumento.CTe.ValorICMS.ToString("n2"),
                        ValorFrete = controleDocumento.CTe.ValorFrete.ToString("n2"),
                        CST = controleDocumento.CTe?.CST ?? string.Empty,
                        CFOP = controleDocumento.CTe.CFOP?.CodigoCFOP ?? 0,
                        Tomador = controleDocumento.CTe.TomadorPagador?.Cliente?.Descricao ?? string.Empty,
                        Remetente = controleDocumento.CTe.Remetente?.Cliente?.Descricao ?? string.Empty,
                        Destinatario = controleDocumento.CTe.Destinatario?.Cliente?.Descricao ?? string.Empty,
                        Expedidor = controleDocumento.CTe.Expedidor?.Cliente.Descricao ?? "",
                        Recebedor = controleDocumento.CTe.Recebedor?.Cliente.Descricao ?? "",
                        Origem = controleDocumento.CTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Destino = controleDocumento.CTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Emissor = controleDocumento.CTe.Empresa?.Descricao ?? string.Empty,
                        Complemento = controleDocumento.CTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento,
                        Documentos = notasCTe,
                        NumeroCTe = controleDocumento.CTe?.Numero ?? 0,
                        Rota = controleDocumento.Carga?.Rota?.Descricao ?? string.Empty,
                    },
                    PreCTe = new
                    {
                        controleDocumento.CargaCTe.PreCTe.Codigo,
                        CodigoEmpresa = controleDocumento.CargaCTe.PreCTe.Empresa.Codigo,
                        ValorDesconto = /*controleDocumento.ValorDesconto.ToString("n2"),*/ "0,00",
                        ValorAReceber = controleDocumento.CargaCTe.PreCTe.ValorAReceber.ToString("n2"),
                        AliquotaICMS = controleDocumento.CargaCTe.PreCTe.AliquotaICMS.ToString("n2"),
                        BaseCalculoICMS = controleDocumento.CargaCTe.PreCTe.BaseCalculoICMS.ToString("n2"),
                        ValorICMS = controleDocumento.CargaCTe.PreCTe.ValorICMS.ToString("n2"),
                        CST = controleDocumento.CargaCTe.PreCTe.CST ?? string.Empty,
                        CFOP = controleDocumento.CargaCTe.PreCTe.CFOP?.CodigoCFOP ?? 0,
                        Tomador = controleDocumento.CargaCTe.PreCTe.Tomador?.Cliente?.Descricao ?? string.Empty,
                        Remetente = controleDocumento.CargaCTe.PreCTe.Remetente?.Cliente?.Descricao ?? string.Empty,
                        TipoAmbiente = controleDocumento.CargaCTe.PreCTe.Empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "Homologação" : "Produção",
                        Destinatario = controleDocumento.CargaCTe.PreCTe.Destinatario?.Cliente?.Descricao ?? string.Empty,
                        Expedidor = controleDocumento.CargaCTe.PreCTe.Expedidor?.Cliente?.Descricao ?? "",
                        Recebedor = controleDocumento.CargaCTe.PreCTe.Recebedor?.Cliente?.Descricao ?? "",
                        Origem = controleDocumento.CargaCTe.PreCTe.LocalidadeInicioPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Destino = controleDocumento.CargaCTe.PreCTe.LocalidadeTerminoPrestacao?.DescricaoCidadeEstado ?? string.Empty,
                        Emissor = controleDocumento.CargaCTe.PreCTe.Empresa?.Descricao ?? string.Empty,
                        Rota = controleDocumento.Carga?.Rota?.Descricao ?? string.Empty,
                        ValorFrete = controleDocumento.CargaCTe.PreCTe.ValorFrete.ToString("n2"),
                        Complemento = controleDocumento.CargaCTe.PreCTe.TipoCTE == Dominio.Enumeradores.TipoCTE.Complemento,
                        Documentos = notasPreCTe,
                        NumeroCTe = controleDocumento.CTe?.Numero ?? 0
                    },

                    ComponentesFrete = servicoComponenteFrete.ObterInformacoesComponentesFrete(controleDocumento.CTe, controleDocumento.CargaCTe?.PreCTe)
                    //CTesAnteriores = ObterCTesAnteriores
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarDadosParqueamento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Documentos.ControleDocumento controleDocumento = repositorioControleDocumento.BuscarPorCodigo(codigoControleDocumento);

                if (controleDocumento == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    controleDocumento.Codigo,
                    Motivo = controleDocumento.MotivoParqueamento,
                    MotivoTransportador = controleDocumento.MotivoTransportador ?? string.Empty,
                    Rejeitado = controleDocumento.SituacaoControleDocumento == SituacaoControleDocumento.RejeitadoPeloTransportador,
                    Aprovado = controleDocumento.SituacaoControleDocumento == SituacaoControleDocumento.ParqueadoManualmente
                };

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao obter detalhes.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ParquearDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(true, false, "Somente o embarcador pode enviar registros para aprovação.");

                List<int> codigosParqueados = Request.GetListParam<int>("Codigos");
                string motivoParqueamento = Request.GetStringParam("MotivoParqueamento");

                foreach (int codigo in codigosParqueados)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao realizar o parqueamento.");

                    if (documento.SituacaoControleDocumento == SituacaoControleDocumento.AguardandoAprovacao)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} já está pendente de aprovação.");

                    if (documento.SituacaoControleDocumento == SituacaoControleDocumento.ParqueadoManualmente)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} já está parqueado.");

                    if ((documento.CTe.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? Dominio.Enumeradores.TipoDocumento.CTe) == Dominio.Enumeradores.TipoDocumento.CTe && documento.CTe.DataEmissao >= DateTime.Today.AddDays(-45))
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} foi emitido há menos de 45 dias, e não pode ser parqueado.");

                    List<(int codigo, int codigoPedido)> dadosMiro = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork).BuscarDadosMiro(new List<int> { documento.CTe.Codigo });

                    if (dadosMiro.Count > 0 || dadosMiro.FirstOrDefault().codigo > 0)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} possui MIRO, e não pode ser parqueado.");

                    documento.SituacaoControleDocumento = SituacaoControleDocumento.AguardandoAprovacao;
                    documento.MotivoParqueamento = !string.IsNullOrWhiteSpace(motivoParqueamento) ? motivoParqueamento : string.Empty;
                    documento.DataEnvioAprovacao = DateTime.Now;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Enviou para Aprovação", unitOfWork);

                    repControleDocumento.Atualizar(documento);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o parqueamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DesparquearDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade svcIrregularidade = new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Documentos/ControleDocumento");

                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.ControleDocumento_PermiteDesparquearDocumentos))
                    return new JsonpResult(true, false, "Usuário não tem permissão para desparquear documentos.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(true, false, "Somente o embarcador pode desparquear um documento.");

                List<int> codigosParqueados = Request.GetListParam<int>("Codigos");

                foreach (int codigo in codigosParqueados)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao realizar o desparqueamento.");

                    if (documento.SituacaoControleDocumento == SituacaoControleDocumento.Desparqueado)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} já desparqueado.");

                    if (documento.SituacaoControleDocumento != SituacaoControleDocumento.ParqueadoManualmente && documento.SituacaoControleDocumento != SituacaoControleDocumento.ParqueadoAutomaticamente)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} não pode ser desparqueado.");

                    unitOfWork.Start();

                    documento.SituacaoControleDocumento = SituacaoControleDocumento.Desparqueado;
                    documento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Documento Desparqueado", unitOfWork);

                    repControleDocumento.Atualizar(documento);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o desparqueamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(true, false, "Somente o transportador pode aprovar ou rejeitar registros.");

                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);
                Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                Servicos.Embarcador.Escrituracao.Provisao servicoProvisao = new Servicos.Embarcador.Escrituracao.Provisao(unitOfWork);
                Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa svcDocumentoDestinadoEmpresa = new Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

                List<int> codigosParqueados = Request.GetListParam<int>("Codigos");
                string motivoAprovacao = Request.GetStringParam("MotivoAprovacao");

                foreach (int codigo in codigosParqueados)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao realizar o parqueamento.");

                    if (documento.SituacaoControleDocumento != SituacaoControleDocumento.AguardandoAprovacao)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} não pode ser aprovado.");

                    if (string.IsNullOrWhiteSpace(motivoAprovacao))
                        return new JsonpResult(false, true, "É obrigatório adicionar um motivo para a aprovação.");

                    var documentoDestinado = repDocumentoDestinadoEmpresa.BuscarPorChaveAguardandoDesacordo(documento.CTe?.Chave ?? string.Empty);

                    string retornoAprovacao = documentoDestinado != null ? Servicos.Embarcador.Documentos.DocumentoDestinadoEmpresa.AprovarEmissaoDesacordo(documentoDestinado, documento.MotivoDesacordo, unitOfWork, TipoServicoMultisoftware, Auditado, documento) : string.Empty;

                    if (!string.IsNullOrWhiteSpace(retornoAprovacao))
                        return new JsonpResult(false, true, retornoAprovacao);

                    documento.SituacaoControleDocumento = SituacaoControleDocumento.ParqueadoManualmente;
                    documento.MotivoTransportador = !string.IsNullOrWhiteSpace(motivoAprovacao) ? motivoAprovacao : string.Empty;

                    //isso precisa ser validado com a mudanca da miro.
                    //Dominio.Entidades.Embarcador.Financeiro.DocumentoFaturamento documentoFaturamento = repositorioDocumentoFaturamento.BuscarDocumentoAtivoPorCTe(documento.CTe.Codigo)?.FirstOrDefault();
                    //servicoProvisao.GerarIntegracaoLoteProvisao(documentoFaturamento);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Aprovou o Parqueamento", unitOfWork);

                    repControleDocumento.Atualizar(documento);
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar o parqueamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(true, false, "Somente o transportador pode aprovar ou rejeitar registros.");

                List<int> codigosParqueados = Request.GetListParam<int>("Codigos");
                string motivoRejeicao = Request.GetStringParam("MotivoRejeicao");

                foreach (int codigo in codigosParqueados)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao realizar o parqueamento.");

                    if (documento.SituacaoControleDocumento != SituacaoControleDocumento.AguardandoAprovacao)
                        return new JsonpResult(false, true, $"O documento {documento.CTe.Numero} não pode ser rejeitado.");

                    if (string.IsNullOrWhiteSpace(motivoRejeicao))
                        return new JsonpResult(false, true, "É obrigatório adicionar um motivo para a rejeição.");

                    documento.SituacaoControleDocumento = SituacaoControleDocumento.RejeitadoPeloTransportador;
                    documento.MotivoTransportador = !string.IsNullOrWhiteSpace(motivoRejeicao) ? motivoRejeicao : string.Empty;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Rejeitou o parqueamento", unitOfWork);

                    repControleDocumento.Atualizar(documento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeitar o parqueamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarAnaliseDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(true, false, "Somente o transportador pode informar análise registros.");

                List<int> codigos = Request.GetListParam<int>("Codigos");
                string analise = Request.GetStringParam("Analise");

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                    if (string.IsNullOrWhiteSpace(analise))
                        return new JsonpResult(false, true, "É obrigatório adicionar a análise.");

                    documento.Analise = analise;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Informou análise", unitOfWork);

                    repControleDocumento.Atualizar(documento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar análise de documentos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarMotivoDocumentos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    return new JsonpResult(true, false, "Somente o transportador pode informar análise registros.");

                List<int> codigos = Request.GetListParam<int>("Codigos");
                int motivo = Request.GetIntParam("Motivo");

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                    if (motivo == 0)
                        return new JsonpResult(false, true, "É obrigatório adicionar o motivo de irregularidade.");

                    Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade motivoIrregularidade = new Repositorio.Embarcador.GerenciamentoIrregularidades.MotivoIrregularidade(unitOfWork).BuscarPorCodigo(motivo);

                    if (motivoIrregularidade == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o motivo de irregularidade.");

                    Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, ServicoResponsavel.Transporador);

                    if (historicoIrregularidade != null)
                    {
                        historicoIrregularidade.Observacao = string.IsNullOrEmpty(historicoIrregularidade.Observacao) ? "Informado o motivo de irregularidade " + motivoIrregularidade.Descricao + "." : Utilidades.String.Left(historicoIrregularidade.Observacao + ". Informado o motivo de irregularidade " + motivoIrregularidade.Descricao + ".", 500);
                    }

                    documento.MotivoIrregularidade = motivoIrregularidade;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Informou o motivo de irregularidade", unitOfWork);

                    repControleDocumento.Atualizar(documento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao informar o motivo de irregularidade.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RespostaResponsabilidade()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositoriohistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);

            try
            {
                unitOfWork.Start();

                List<int> codigos = Request.GetListParam<int>("Codigos");
                string responsavel = Request.GetStringParam("Responsavel");
                ServicoResponsavel servicoResponsavel = responsavel == "Embarcador" ? ServicoResponsavel.Embarcador : ServicoResponsavel.Transporador;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador && responsavel == "Transportador")
                    return new JsonpResult(true, false, "Somente o embarcador pode enviar documentos ao transportador.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe && responsavel == "Embarcador")
                    return new JsonpResult(true, false, "Somente o transportador pode enviar documentos ao embarcador.");

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                    if (documento.ServicoResponsavel == ServicoResponsavel.Embarcador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        return new JsonpResult(false, true, $"O documento só pode ser alterado pelo embarcador.");

                    if (documento.ServicoResponsavel == ServicoResponsavel.Transporador && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                        return new JsonpResult(false, true, $"O documento só pode ser alterado pelo transportador.");

                    if (servicoResponsavel == ServicoResponsavel.Embarcador)
                    {
                        Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, ServicoResponsavel.Transporador);

                        switch (historicoIrregularidade.Irregularidade.GatilhoIrregularidade)
                        {
                            case GatilhoIrregularidade.SemLink:
                            case GatilhoIrregularidade.CTeCancelado:
                            case GatilhoIrregularidade.AliquotaICMSValorICMS:
                            case GatilhoIrregularidade.CNPJTransportadora:

                            case GatilhoIrregularidade.TomadorFreteUnilever:
                            case GatilhoIrregularidade.MunicipioPrestacaoServico:
                            case GatilhoIrregularidade.CFOP:
                            case GatilhoIrregularidade.Participantes:
                            case GatilhoIrregularidade.AliquotaISSValorISS:
                                if (string.IsNullOrEmpty(documento.Analise))
                                    return new JsonpResult(false, true, $"Para transferir a responsabilidade do documento para o embarcador é necessário que o documento tenha análise informada.");
                                break;
                            case GatilhoIrregularidade.CSTICMS:
                            case GatilhoIrregularidade.NFeVinculadaAoFrete:
                                if (string.IsNullOrEmpty(documento.Analise))
                                    return new JsonpResult(false, true, $"Para transferir a responsabilidade do documento para o embarcador é necessário que o documento tenha análise informada.");

                                if (documento.AcaoTratativa == AcaoTratativaIrregularidade.NecessarioCartaCorrecao && !VerificarCCesAnexadasNaoConfirmadas(documento.Codigo, unitOfWork))
                                    return new JsonpResult(false, true, $"Para transferir a responsabilidade do documento para o embarcador é necessário informar carta de correção.");

                                break;
                            case GatilhoIrregularidade.ValorPrestacaoServico:
                            case GatilhoIrregularidade.ValorTotalReceber:
                                if (documento.MotivoIrregularidade == null)
                                    return new JsonpResult(false, true, $"Para transferir a responsabilidade do documento para o embarcador é necessário que o documento tenha motivo informado.");
                                break;
                        }
                    }

                    new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork).MovimentarTratativaDocumento(
                        new Dominio.ObjetosDeValor.Embarcador.GerenciamentoIrregularidades.MovimentarIrregularidade()
                        {
                            ControleDocumento = documento,
                            Responsavel = servicoResponsavel
                        }, true);

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Transferiu a responsabilidade para o " + responsavel, unitOfWork);

                    repControleDocumento.Atualizar(documento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar a troca de responsabilidade do documento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DelegarSetor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                List<int> codigos = Request.GetListParam<int>("Codigos");
                int setor = Request.GetIntParam("Setor");

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        return new JsonpResult(false, true, $"Somente o embarcador pode delegar setores.");

                    if (setor > 0)
                    {
                        Dominio.Entidades.Setor setorDelegado = new Repositorio.Setor(unitOfWork).BuscarPorCodigo(setor);

                        if (setorDelegado == null)
                            return new JsonpResult(false, true, "Não foi possível encontrar o setor escolhido.");

                        new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                        {
                            ControleDocumento = documento,
                            Responsavel = ServicoResponsavel.Embarcador,
                            Observacao = $@"Delegado para o setor {setorDelegado.Descricao}."
                        }, false, setorDelegado);
                    }
                    else
                    {
                        new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork).MovimentarTratativaDocumento(new MovimentarIrregularidade()
                        {
                            ControleDocumento = documento,
                            Responsavel = ServicoResponsavel.Embarcador,
                            Observacao = $@"Delegado para o setor da tratativa seguinte."
                        });
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Delegou o setor", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao delegar o setor.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarAnexos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoControleDocumento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(codigoControleDocumento);

                return new JsonpResult(new
                {
                    Anexos = (
                                from anexo in anexos
                                select new
                                {
                                    anexo.Codigo,
                                    anexo.Descricao,
                                    anexo.NomeArquivo,
                                    CodigoIrregularidade = anexo?.CodigoIrregularidade ?? 0
                                }
                            ).ToList(),
                });
            }

            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar Anexos.");
            }

            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> InformarAcaoTratativa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                List<int> codigos = Request.GetListParam<int>("Codigos");
                int acao = Request.GetIntParam("Acao");

                foreach (int codigo in codigos)
                {
                    Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                    if (documento == null)
                        return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                    if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                        return new JsonpResult(false, true, $"Somente o embarcador pode informar ação para as tratativas.");

                    documento.Initialize();

                    documento.AcaoTratativa = (AcaoTratativaIrregularidade)acao;

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Informou ação de tratativa", unitOfWork);

                    repControleDocumento.Atualizar(documento, Auditado);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao realizar o parqueamento.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PagarConformeFRS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<int> codigos = Request.GetListParam<int>("Codigo");

                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> documentos = repositorioControleDocumento.BuscarPorCodigo(codigos);

                if (documentos.Count == 0)
                    return new JsonpResult(false, "Nenhum Registro encontrado para processar");


                foreach (var documento in documentos)
                {
                    Dominio.Entidades.Embarcador.Escrituracao.Provisao provisaoDocumento = BuscarProvisao(unitOfWork, documento);

                    if (provisaoDocumento == null)
                        continue;

                    int diasEmissaoDocumento = (documento.CTe.DataEmissao.Value - DateTime.Now).Days;

                    Dominio.Enumeradores.TipoDocumento tipoDocumento = documento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao;

                    if (documento.CTe.ValorAReceber > provisaoDocumento.ValorProvisao && diasEmissaoDocumento > 45 && tipoDocumento == Dominio.Enumeradores.TipoDocumento.CTe)
                    {
                        decimal diferencaEntreValores = documento.CTe.ValorAReceber - provisaoDocumento.ValorProvisao;
                        Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarDocumentoFaturamentoPorControleDocumento(documento, diferencaEntreValores, unitOfWork, true);
                        LiberarHistorico(documento, unitOfWork);
                        repositorioControleDocumento.Atualizar(documento);

                        continue;
                    }

                    if (tipoDocumento == Dominio.Enumeradores.TipoDocumento.NFS && documento.CTe.ValorPrestacaoServico > provisaoDocumento.ValorProvisao)
                        return new JsonpResult(new { Parqueamento = true }, true, $"Direcione Parqueamento para autorização do transportador do documento (Nº{documento.CTe.Numero})");


                    return new JsonpResult(new { GerarOcorrencia = true, CodigoDocumento = documento.Codigo, CodigCarga = documento.Carga.Codigo }, true, $"Precisa gerar uma Ocorrência  para o documento (Nº {documento.CTe.Numero})");
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Falha ao tentar Pagar conforme FRS");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PagarConformeOutroValor()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                decimal outroValorPagar = Request.GetDecimalParam("NovoValor");
                int codigoControleDocumento = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

                Dominio.Entidades.Embarcador.Documentos.ControleDocumento existeControleDocmento = repositorioControleDocumento.BuscarPorCodigo(codigoControleDocumento);

                if (existeControleDocmento == null)
                    throw new ControllerException("Controle de documento não encontrado para pagar");

                if (outroValorPagar == 0)
                    throw new ControllerException("Valor informado não é valido");

                int diasEmissaoDocumento = (existeControleDocmento.CTe.DataEmissao.Value - DateTime.Now).Days;

                if (existeControleDocmento.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.CTe && diasEmissaoDocumento <= 45)
                    throw new ControllerException("Documento foi emitiado a menos de 45 dias. Precisa ser direcionado para a area de Desacordo");

                Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = BuscarProvisao(unitOfWork, existeControleDocmento);

                if (provisao == null)
                    throw new ControllerException("Este Documento não possui FRS (Provisão)");

                decimal valorCte = existeControleDocmento.CTe.ValorAReceber;
                decimal valorProvisao = provisao.ValorProvisao;
                decimal diferencaEntreValores = outroValorPagar - valorCte;

                /*
                 Se outro valor a pagar é maior que o valor do CT-e dever gerar um complemento, senão deve gerar um desconto
                 */

                Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarDocumentoFaturamentoPorControleDocumento(existeControleDocmento, diferencaEntreValores, unitOfWork);
                LiberarHistorico(existeControleDocmento, unitOfWork);
                existeControleDocmento.SituacaoControleDocumento = SituacaoControleDocumento.Liberado;

                return new JsonpResult(true, "Documento enviado para pagamento conforme outro valor");
            }
            catch (BaseException ex)
            {
                return new JsonpResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Error ao tentar pagar conforme outro valor");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PagarConformeDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                List<int> codigos = Request.GetListParam<int>("Codigo");
                Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                Repositorio.Embarcador.Escrituracao.Provisao repositorioProvisao = new Repositorio.Embarcador.Escrituracao.Provisao(unitOfWork);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repositorioDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unitOfWork);

                List<Dominio.Entidades.Embarcador.Documentos.ControleDocumento> documentos = repositorioControleDocumento.BuscarPorCodigo(codigos);

                if (documentos.Count == 0)
                    return new JsonpResult(false, "Nenhum Registro encontrado para processar");

                foreach (var documento in documentos)
                {
                    LiberarHistorico(documento, unitOfWork);
                    Servicos.Embarcador.Fatura.FaturamentoDocumento.GerarDocumentoFaturamentoPorControleDocumento(documento, documento.CTe.ValorAReceber, unitOfWork);
                }

                return new JsonpResult(true, "Pagamento feito Conforme documento processado com sucesso");
            }
            catch (Exception exe)
            {
                Servicos.Log.TratarErro(exe);
                return new JsonpResult(false, "Falha ao tentar Pagar conforme FRS");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarCCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                if (documento == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, true, $"Somente o embarcador pode aprovar cartas de correção.");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(codigo);

                if (anexos == null || anexos.Count() == 0 || !VerificarCCesAnexadasNaoConfirmadas(documento.Codigo, unitOfWork))
                    return new JsonpResult(false, true, $"Não há nenhum arquivo de carta de correção informado para a irregularidade atual.");

                documento.Initialize();

                documento.SituacaoAprovacaoCartaDeCorrecao = SituacaoAprovacaoCartaDeCorrecao.Aprovada;
                documento.MotivoRejeicaoCCe = string.Empty;
                Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, 0);

                foreach (var anexo in anexos)
                {
                    anexo.Initialize();
                    anexo.CodigoIrregularidade = historicoIrregularidade.Irregularidade.Codigo;
                    repAnexosCartaCorrecao.Atualizar(anexo);
                }

                new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork).ValidarIrregularidadeControleDocumento(documento);

                repControleDocumento.Atualizar(documento, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Aprovou carta de correção da irregularidade " + historicoIrregularidade.Irregularidade.Descricao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar carta de correção.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RejeitarCCe()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                string motivo = Request.GetStringParam("Motivo");

                Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                if (documento == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    return new JsonpResult(false, true, $"Somente o embarcador pode rejeitar cartas de correção.");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(unitOfWork);
                List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(codigo);

                if (anexos == null || anexos.Count() == 0 || !VerificarCCesAnexadasNaoConfirmadas(documento.Codigo, unitOfWork))
                    return new JsonpResult(false, true, $"Não há nenhum arquivo de carta de correção informado para a irregularidade atual.");

                documento.Initialize();

                documento.SituacaoAprovacaoCartaDeCorrecao = SituacaoAprovacaoCartaDeCorrecao.Rejeitada;
                documento.MotivoRejeicaoCCe = motivo;
                Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork).BuscarUltimoPorDocumentoResponsavel(documento.Codigo, 0);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, documento, null, "Rejeitou carta de correção da irregularidade " + historicoIrregularidade.Irregularidade.Descricao, unitOfWork);

                repControleDocumento.Atualizar(documento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeição da carta de correção.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarPreCtesLikarDocumento()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaPreCte());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LinkarDocumento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");
                int codigoCargaCTe = Request.GetIntParam("CargaCTe");

                Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = repControleDocumento.BuscarPorCodigo(codigo);

                if (documento == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o documento.");

                Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);

                if (cargaCTe == null)
                    return new JsonpResult(false, true, "Ocorreu uma falha ao buscar o pré-cte para vínculo.");

                new Servicos.Embarcador.Carga.PreCTe(unitOfWork).LinkarCTeComPreCTe(cargaCTe, documento.CTe, cargaCTe.PreCTe, TipoServicoMultisoftware, ConfiguracaoEmbarcador, unitOfWork);

                documento.Initialize();
                documento.SituacaoVerificacao = SituacaoVerificacaoControleDocumento.AgVerificacao;

                repControleDocumento.Atualizar(documento, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao rejeição da carta de correção.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento ObterFiltrosPesquisa()
        {
            bool transportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe;

            Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                SituacaoControleDocumento = transportador ? new List<SituacaoControleDocumento>() { SituacaoControleDocumento.AguardandoAprovacao, SituacaoControleDocumento.Inconsistente, SituacaoControleDocumento.InconsistenteSemTratativa } : Request.GetListEnumParam<SituacaoControleDocumento>("Situacao"),
                Numero = Request.GetIntParam("NumeroDocumento"),
                Serie = Request.GetIntParam("Serie"),
                CodigoModeloDocumentoFiscal = Request.GetIntParam("ModeloDocumentoFiscal"),
                CodigoCarga = Request.GetIntParam("Carga"),
                NFe = Request.GetIntParam("NotaFiscal"),
                CodigoTransportador = transportador ? this.Empresa?.Codigo ?? 0 : Request.GetIntParam("Empresa"),
                CodigosPortfolio = Request.GetListParam<int>("Portfolio"),
                CodigosIrregularidade = Request.GetListParam<int>("Irregularidade"),
                CodigosSetor = Request.GetListParam<int>("Setor"),
                CodigoUsuario = Request.GetIntParam("Usuario"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataGeracaoIrregularidadeInicial = Request.GetDateTimeParam("DataGeracaoIrregularidadeInicial"),
                DataGeracaoIrregularidadeFinal = Request.GetDateTimeParam("DataGeracaoIrregularidadeFinal"),
                Empresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Empresa.Codigo : 0,
                TransportadorLogado = transportador
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCargaCTe", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("MotivoParqueamento", false);
                grid.AdicionarCabecalho("Analise", false);
                grid.AdicionarCabecalho("CodigoIrregularidade", false);
                grid.AdicionarCabecalho("CodigoEntidadeIrregularidade", false);
                grid.AdicionarCabecalho("PossuiPreCTe", false);
                grid.AdicionarCabecalho("Número", "Numero", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Série", "Serie", 5, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Modelo Documento Fiscal", "ModeloDocumentoFiscal", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Carga", "Carga", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("NF-es", "NFes", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "SituacaoControleDocumento", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Dias em Aprovação", "DiasEmAprovacao", 8, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Transportador", false);
                else
                    grid.AdicionarCabecalho("Transportador", "Transportador", 12, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Portfólio", "Portfolio", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Irregularidade", "Irregularidade", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data da Irregularidade", "DataGeracaoIrregularidade", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Setor", "Setor", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Responsável pela Irregularidade", "ResponsavelPelaIrregularidade", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tratativas", false);
                grid.AdicionarCabecalho("DentroDoMes", false);
                grid.AdicionarCabecalho("TipoDocumentoEmissao", false);
                grid.AdicionarCabecalho("CCeRejeitada", false);
                grid.AdicionarCabecalho("MotivoRejeicaoCCe", false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "ControleDocumento/Pesquisa", "grid-controle-documento");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));

                Repositorio.Embarcador.Documentos.ControleDocumento repControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
                Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorioHistoricoIrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Documentos.FiltroPesquisaControleDocumento filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repControleDocumento.ContarConsultaControleDocumento(filtrosPesquisa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                IList<Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento> controleDocumentos = (totalRegistros > 0) ? repControleDocumento.ConsultarControleDocumento(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento>();
                List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicoIrregularidade = totalRegistros > 0 ? repositorioHistoricoIrregularidade.BuscarPorCodigoControleDocumento(controleDocumentos.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade>();

                var lista = (from obj in controleDocumentos
                             select RetornarControleDocumentoComHistorico(obj, historicoIrregularidade)).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridPesquisaPreCte()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Carga", "Carga", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Remetente", "Remetente", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Expedidor", "Expedidor", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Recebedor", "Recebedor", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("CodigoPreCTe", false);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.Documentos.ControleDocumento documento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork).BuscarPorCodigo(codigo);

                if (documento == null)
                    throw new ControllerException("Documento não encontrado.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTes = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork).BuscarPreCteSemCTeAtivoPorTransportadoraETipoDocumento(documento.CTe?.Empresa?.Codigo ?? 0, documento.CTe?.ModeloDocumentoFiscal?.TipoDocumentoEmissao ?? 0, grid.inicio, grid.limite);

                var lista = (from obj in cargaCTes
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 Carga = obj.Carga.CodigoCargaEmbarcador,
                                 Remetente = obj?.PreCTe?.Remetente?.Nome ?? string.Empty,
                                 Expedidor = obj?.PreCTe?.Expedidor?.Nome ?? string.Empty,
                                 Destinatario = obj?.PreCTe?.Destinatario?.Nome ?? string.Empty,
                                 Recebedor = obj?.PreCTe?.Recebedor?.Nome ?? string.Empty,
                                 Tomador = obj?.PreCTe?.Tomador?.Nome ?? string.Empty,
                                 Valor = obj?.PreCTe?.ValorAReceber ?? 0,
                                 CodigoPreCTe = obj?.PreCTe?.Codigo ?? 0
                             }
                ).OrderByDescending(o => o.Codigo).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(lista.Count);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DataGeracaoIrregularidadeFormatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            if (propriedadeOrdenar.Equals("SituacaoControleDocumento"))
                return "Situacao";

            if (propriedadeOrdenar.Equals("ResponsavelPelaIrregularidade"))
                return "ServicoResponsavel";

            return propriedadeOrdenar;
        }

        private Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento RetornarControleDocumentoComHistorico(Dominio.ObjetosDeValor.Embarcador.Documentos.ControleDocumento obj, List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicoIrregularidade)
        {
            Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade historicoIrregularidadePrincipal;
            bool retornaTratativas = true;
            List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicos = historicoIrregularidade.Where(x => x.ControleDocumento.Codigo == obj.Codigo && x.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao).ToList() ?? null;
            historicoIrregularidadePrincipal = historicos.OrderBy(x => x.Irregularidade?.Sequencia).ThenByDescending(x => x.Codigo).FirstOrDefault();

            if (historicoIrregularidadePrincipal == null)
            {
                historicoIrregularidadePrincipal = historicoIrregularidade.Where(x => x.ControleDocumento.Codigo == obj.Codigo && x.SituacaoIrregularidade != SituacaoIrregularidade.AguardandoAprovacao && x.SituacaoIrregularidade != SituacaoIrregularidade.Aprovada)
                    .OrderByDescending(x => x.Irregularidade.Sequencia)
                    .ThenByDescending(x => x.Codigo)
                    .FirstOrDefault();
                retornaTratativas = false;
            }

            if (obj.Situacao == 7)
            {
                historicoIrregularidadePrincipal = null;
                retornaTratativas = false;
            }

            obj.Portfolio = historicoIrregularidadePrincipal != null ? historicoIrregularidadePrincipal?.Porfolio?.Descricao ?? "" : "";
            obj.CodigoIrregularidade = historicoIrregularidadePrincipal != null ? (int)(historicoIrregularidadePrincipal?.Irregularidade?.GatilhoIrregularidade ?? 0) : 0;
            obj.CodigoEntidadeIrregularidade = historicoIrregularidadePrincipal != null ? (historicoIrregularidadePrincipal?.Irregularidade?.Codigo ?? 0) : 0;
            obj.Irregularidade = historicoIrregularidadePrincipal != null ? historicoIrregularidadePrincipal?.Irregularidade?.Descricao ?? "" : string.Empty;
            obj.DataGeracaoIrregularidade = historicoIrregularidadePrincipal != null ? historicoIrregularidadePrincipal?.DataIrregularidade.Value : null;
            obj.ResponsavelPelaIrregularidade = historicoIrregularidadePrincipal != null ? !string.IsNullOrEmpty(historicoIrregularidadePrincipal?.Setor?.Descricao) ? historicoIrregularidadePrincipal.Setor.Descricao : historicoIrregularidadePrincipal.ServicoResponsavel.ObterDescricao() : "";
            obj.Setor = historicoIrregularidadePrincipal?.Setor?.Descricao ?? string.Empty;
            obj.Tratativas = retornaTratativas ? BuscarTratativasIrregularidadeSetor(historicoIrregularidadePrincipal?.SequenciaTrataviva ?? 0, historicoIrregularidadePrincipal?.Irregularidade?.Codigo ?? 0, obj?.CodigoCargaCTe ?? 0, (ServicoResponsavel)obj.ServicoResponsavel) : string.Empty;

            return obj;
        }

        private string BuscarTratativasIrregularidadeSetor(int sequencia, int codigoIrregularidade, int codigoCargaCte, ServicoResponsavel servico)
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                if (servico == ServicoResponsavel.Transporador)
                    return string.Empty;

                int codigoGrupoOperacao = 0;
                if (codigoCargaCte > 0)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork).BuscarPorCodigo(codigoCargaCte);
                    if (cargaCTe != null)
                        codigoGrupoOperacao = cargaCTe.Carga?.TipoOperacao?.GrupoTipoOperacao?.Codigo ?? 0;
                }

                Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade repositorioTratativa = new Repositorio.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade(unitOfWork);
                Dominio.Entidades.Embarcador.GerenciamentoIrregularidades.TratativaIrregularidade tratativa = repositorioTratativa.BuscarPorDefinicaoTratativasIrregularidade(codigoIrregularidade, sequencia, codigoGrupoOperacao, null);

                if (tratativa == null)
                    return string.Empty;
                else
                {
                    string retorno = string.Empty;
                    foreach (AcaoTratativaIrregularidade acao in tratativa.Acoes)
                        retorno += (int)acao + "|";

                    return retorno.TrimEnd('|');
                }
            }
        }

        private Dominio.Entidades.Embarcador.Escrituracao.Provisao BuscarProvisao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Documentos.ControleDocumento existeControleDocmento)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repositorioCargaPedidoCte = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            Repositorio.Embarcador.Escrituracao.DocumentoProvisao repositorioDocumentoProvisao = new Repositorio.Embarcador.Escrituracao.DocumentoProvisao(unitOfWork);
            List<int> codigoStage = repositorioCargaPedidoCte.BuscarStagePorCodigoCTe(new List<int> { existeControleDocmento.CTe.Codigo }, existeControleDocmento.Carga.Codigo);

            List<Dominio.Entidades.Embarcador.Escrituracao.DocumentoProvisao> documentoFrs = repositorioDocumentoProvisao.BuscarPorStage(codigoStage);
            Dominio.Entidades.Embarcador.Escrituracao.Provisao provisao = documentoFrs.Where(x => x.Provisao != null).Select(x => x.Provisao).Distinct().OrderByDescending(x => x.Codigo).FirstOrDefault();
            return provisao;
        }

        private void LiberarHistorico(Dominio.Entidades.Embarcador.Documentos.ControleDocumento existeControleDocmento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Documentos.ControleDocumento repositorioControleDocumento = new Repositorio.Embarcador.Documentos.ControleDocumento(unitOfWork);
            Repositorio.Embarcador.Documentos.HistoricoIrregularidade repositorHistorioIrrregularidade = new Repositorio.Embarcador.Documentos.HistoricoIrregularidade(unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> historicos = repositorHistorioIrrregularidade.BuscarPorControleDocumento(existeControleDocmento.Codigo);
            var historicoIrregularidadePrincipal = historicos.Where(X => X.SituacaoIrregularidade == SituacaoIrregularidade.AguardandoAprovacao).OrderBy(x => x.Irregularidade.Sequencia).ThenByDescending(x => x.Codigo).FirstOrDefault();

            if (historicoIrregularidadePrincipal == null)
            {
                existeControleDocmento.SituacaoControleDocumento = SituacaoControleDocumento.Liberado;
                repositorioControleDocumento.Atualizar(existeControleDocmento);
                return;
            };

            new Servicos.Embarcador.GerenciamentoIrregularidades.Irregularidade(unitOfWork).AprovarHistoricoIrregularidade(new List<Dominio.Entidades.Embarcador.Documentos.HistoricoIrregularidade> { historicoIrregularidadePrincipal }, AprovadorIrregularidade.Usuario);
            existeControleDocmento.SituacaoControleDocumento = SituacaoControleDocumento.Liberado;
            repositorioControleDocumento.Atualizar(existeControleDocmento);
        }

        private bool VerificarCCesAnexadasNaoConfirmadas(int documento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento> repAnexosCartaCorrecao = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo, Dominio.Entidades.Embarcador.Documentos.ControleDocumento>(unitOfWork);
            List<Dominio.Entidades.Embarcador.Documentos.CartaCorrecaoAnexo> anexos = repAnexosCartaCorrecao.BuscarPorEntidade(documento);

            return anexos != null && anexos.Any(x => (x?.CodigoIrregularidade ?? 0) == 0 && x.ExtensaoArquivo.ToLower().Equals("xml"));
        }

        #endregion Métodos Privados
    }
}
