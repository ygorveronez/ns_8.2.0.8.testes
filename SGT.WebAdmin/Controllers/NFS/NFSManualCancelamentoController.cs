using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/NFSManualCancelamento")]
    public class NFSManualCancelamentoController : BaseController
    {
		#region Construtores

		public NFSManualCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double codigoTomador;

                int codigoLocalidadePrestacao, codigoFilial, numeroNFS, codigoCarga, numeroDOC, codigoTransportador;
                int.TryParse(Request.Params("LocalidadePrestacao"), out codigoLocalidadePrestacao);
                int.TryParse(Request.Params("Filial"), out codigoFilial);
                double.TryParse(Request.Params("Tomador"), out codigoTomador);
                int.TryParse(Request.Params("Numero"), out numeroNFS);
                int.TryParse(Request.Params("numeroDOC"), out numeroDOC);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    codigoTransportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Empresa", 20, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho("Empresa/Filial", "Empresa", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Local da Prestacao", "LocalidadePrestacao", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 15, Models.Grid.Align.left, true);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.center, true);

                grid.AdicionarCabecalho("Valor", "ValorFrete", 10, Models.Grid.Align.right, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar != "Situacao")
                    propOrdenar = "LancamentoNFSManual." + propOrdenar;

                if (propOrdenar == "Numero")
                    propOrdenar = "LancamentoNFSManual.LancamentoNFSManual." + propOrdenar;

                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento> cargasCancelamento = repNFSManualCancelamento.Consultar(codigoLocalidadePrestacao, codigoFilial, codigoTomador, numeroNFS, numeroDOC, codigoCarga, codigoTransportador, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNFSManualCancelamento.ContarConsulta(codigoLocalidadePrestacao, codigoFilial, codigoTomador, numeroNFS, numeroDOC, codigoCarga, codigoTransportador, situacao));

                var retorno = (from obj in cargasCancelamento
                               select new
                               {
                                   obj.Codigo,
                                   Numero = obj.LancamentoNFSManual?.DadosNFS.Numero.ToString() ?? "0",
                                   Empresa = obj.LancamentoNFSManual.Transportador?.Descricao ?? string.Empty,
                                   LocalidadePrestacao = obj.LancamentoNFSManual.LocalidadePrestacao.DescricaoCidadeEstado,
                                   Tomador = obj.LancamentoNFSManual.Tomador?.Descricao ?? string.Empty,
                                   Situacao = obj.DescricaoSituacao,
                                   ValorFrete = obj.LancamentoNFSManual?.DadosNFS.ValorFrete.ToString("n2") ?? "0,00"
                               }).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ValidarCarga()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLancamentoNFSManual;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);

                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeTrabalho);

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual);

                if (lancamentoNFSManual == null)
                    return new JsonpResult(false, true, "NFS Manual não encontrado.");

                bool nfsPermite = true;
                string mensagem = string.Empty;

                //todo: ver para criar regra por prefeiura na questão do prazo de cancelamento, quando a nfse for emtida pelo sistema.
                //if (lancamentoNFSManual.CTe != null && lancamentoNFSManual.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe && (lancamentoNFSManual.CTe.DataEmissao.Value.AddDays(7) > DateTime.Now) && lancamentoNFSManual.CTe.Status == "A")
                //{
                //    mdfePermite = false;
                //}

                return new JsonpResult(new
                {
                    NFSPermiteCancelamento = nfsPermite
                });
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, true, "Ocorreu uma falha ao validar os dados da carga para cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfigEmbarcador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unidadeTrabalho);
                Repositorio.Embarcador.Financeiro.DocumentoFaturamento repDocumentoFaturamento = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(unidadeTrabalho);
                Repositorio.Embarcador.NFS.LancamentoNFSManual repLancamentoNFSManual = new Repositorio.Embarcador.NFS.LancamentoNFSManual(unidadeTrabalho);
                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral config = repConfigEmbarcador.BuscarConfiguracaoPadrao();

                int codigoLancamentoNFSManual;
                int.TryParse(Request.Params("LancamentoNFSManual"), out codigoLancamentoNFSManual);

                string motivo = Request.Params("Motivo");
                if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20)
                    return new JsonpResult(false, true, "O motivo deve possuir mais de 20 caracteres.");

                Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual = repLancamentoNFSManual.BuscarPorCodigo(codigoLancamentoNFSManual);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = repNFSManualCancelamento.BuscarPorLancamentoNFSManual(codigoLancamentoNFSManual);

                if (nfsManualCancelamento != null)
                    return new JsonpResult(false, true, "Já existe uma solicitação de cancelamento para esta NFS Manual.");

                bool liquidada = repDocumentoFaturamento.ExisteDocumentoPagoPorLancamentoNFSManual(lancamentoNFSManual.Codigo);
                if (liquidada)
                    return new JsonpResult(false, true, "Não é possível cancelar a carga pois seus documentos já foram pagos.");

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamento = new List<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

                if (!ValidarSeEstaVinculadoAAlgumDocumento(out string erro, lancamentoNFSManual, titulosParaCancelamento, unidadeTrabalho))
                    return new JsonpResult(false, true, erro);

                Dominio.Entidades.Embarcador.Escrituracao.Pagamento pagamento = repDocumentoFaturamento.ExistePagamentoEmFechamento(lancamentoNFSManual.Codigo);
                if (pagamento != null)
                    return new JsonpResult(false, true, "Não é possível cancelar a NFS pois ela está em um lote de pagamento que está em aberto, Lote " + pagamento.Numero.ToString() + ", primeiro cancele o lote de pagamento, depois tente cancelar a NFS.");

                if ((config?.NaoPermitirCancelamentoNFSManualSeHouverIntegracao ?? false) && (lancamentoNFSManual.CTe.NFsManualIntegrada ?? false))
                    return new JsonpResult(false, true, "Não é possível cancelar a NFS pois ela tem uma integração confirmada.");

                unidadeTrabalho.Start();

                Servicos.Embarcador.Financeiro.Titulo.GerarCancelamentoAutomaticoTitulosEmAberto(titulosParaCancelamento, "Cancelamento do título gerado automaticamente à partir do cancelamento da NFS Manual " + (lancamentoNFSManual.DadosNFS?.Numero.ToString() ?? string.Empty) + ".", TipoServicoMultisoftware, unidadeTrabalho);

                nfsManualCancelamento = new Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento();

                nfsManualCancelamento.LancamentoNFSManual = lancamentoNFSManual;
                nfsManualCancelamento.DataCancelamento = DateTime.Now;
                nfsManualCancelamento.MotivoCancelamento = motivo;
                nfsManualCancelamento.Usuario = Usuario;
                nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.EmCancelamento;
                repNFSManualCancelamento.Inserir(nfsManualCancelamento, Auditado);

                unidadeTrabalho.CommitChanges();

                EnviarNFSeParaCancelamento(nfsManualCancelamento, unidadeTrabalho);

                return new JsonpResult(nfsManualCancelamento.Codigo);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao gerar o cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }


        public async Task<IActionResult> Reenviar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento;
                int.TryParse(Request.Params("Codigo"), out codigoCancelamento);

                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (nfsManualCancelamento.SituacaoNFSManualCancelamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.CancelamentoRejeitado)
                    return new JsonpResult(false, true, "A situação do cancelamento não permite o reenvio do mesmo.");

                unidadeTrabalho.Start();

                nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.EmCancelamento;

                repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManualCancelamento, null, "Reenviou o Cancelamento.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                EnviarNFSeParaCancelamento(nfsManualCancelamento, unidadeTrabalho);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao reenviar o cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> InformarCancelamentoPrefeitura()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoCancelamento;
                int.TryParse(Request.Params("Codigo"), out codigoCancelamento);

                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (nfsManualCancelamento.SituacaoNFSManualCancelamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.CancelamentoRejeitado)
                    return new JsonpResult(false, true, "A situação do cancelamento não permite o reenvio do mesmo.");

                unidadeTrabalho.Start();

                nfsManualCancelamento.SituacaoNFSManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFSManualCancelamento.EmCancelamento;
                nfsManualCancelamento.CancelouDocumentos = true;

                repNFSManualCancelamento.Atualizar(nfsManualCancelamento);

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                nfsManualCancelamento.LancamentoNFSManual.CTe.Status = "C";
                nfsManualCancelamento.LancamentoNFSManual.CTe.DataCancelamento = DateTime.Now;
                repCTe.Atualizar(nfsManualCancelamento.LancamentoNFSManual.CTe);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, nfsManualCancelamento, null, "Informou o cancelamento da NFSe na prefeitura.", unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao reenviar o cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> ObterDadosGerais()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var retorno = new
                {
                    Usuario = Usuario.Nome
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados gerais para cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.NFS.NFSManualCancelamento repNFSManualCancelamento = new Repositorio.Embarcador.NFS.NFSManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento = repNFSManualCancelamento.BuscarPorCodigo(codigo);

                if (nfsManualCancelamento == null)
                    return new JsonpResult(false, true, "Cancelamento não encontrado. Atualize a página e tente novamente.");

                var retorno = new
                {
                    Codigo = nfsManualCancelamento.Codigo,
                    LancamentoNFSManual = new
                    {
                        Codigo = nfsManualCancelamento.LancamentoNFSManual.Codigo,
                        Descricao = ((nfsManualCancelamento.LancamentoNFSManual.DadosNFS?.Numero.ToString() + " - ") ?? string.Empty) + (nfsManualCancelamento.LancamentoNFSManual.Transportador?.Descricao ?? string.Empty),
                        Empresa = nfsManualCancelamento.LancamentoNFSManual.Transportador?.Descricao ?? string.Empty,
                        NFS = nfsManualCancelamento.LancamentoNFSManual?.DadosNFS.Numero.ToString() ?? "0",
                        LocalidadePrestacao = nfsManualCancelamento.LancamentoNFSManual.LocalidadePrestacao.DescricaoCidadeEstado,
                        Tomador = nfsManualCancelamento.LancamentoNFSManual.Tomador?.Descricao ?? string.Empty,
                        Filial = nfsManualCancelamento.LancamentoNFSManual.Filial?.Descricao ?? string.Empty
                    },
                    UsuarioSolicitou = nfsManualCancelamento.Usuario?.Nome,
                    Motivo = nfsManualCancelamento.MotivoCancelamento,
                    nfsManualCancelamento.DescricaoSituacao,
                    Situacao = nfsManualCancelamento.SituacaoNFSManualCancelamento,
                    MensagemRejeicaoCancelamento = nfsManualCancelamento.MotivoRejeicaoCancelamento,
                    DataCancelamento = nfsManualCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy")
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os detalhes do cancelamento.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados 

        private void EnviarNFSeParaCancelamento(Dominio.Entidades.Embarcador.NFS.NFSManualCancelamento nfsManualCancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);

            if (nfsManualCancelamento.LancamentoNFSManual.CTe != null && nfsManualCancelamento.LancamentoNFSManual.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
            {
                Servicos.NFSe serNFSe = new Servicos.NFSe(unidadeTrabalho);
                serNFSe.CancelarNFSe(nfsManualCancelamento.LancamentoNFSManual.CTe.Codigo, unidadeTrabalho);
            }
            else if (nfsManualCancelamento.LancamentoNFSManual.CTe != null && (nfsManualCancelamento.LancamentoNFSManual.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFS || nfsManualCancelamento.LancamentoNFSManual.CTe.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.Outros))
            {
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeTrabalho);
                nfsManualCancelamento.LancamentoNFSManual.CTe.Status = "C";
                nfsManualCancelamento.LancamentoNFSManual.CTe.DataCancelamento = DateTime.Now;
                repCTe.Atualizar(nfsManualCancelamento.LancamentoNFSManual.CTe);

            }
        }

        private bool ValidarSeEstaVinculadoAAlgumDocumento(out string erro, Dominio.Entidades.Embarcador.NFS.LancamentoNFSManual lancamentoNFSManual, List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamento, Repositorio.UnitOfWork unitOfWork)
        {
            if(lancamentoNFSManual.CTe == null)
            {
                erro = string.Empty;
                return true;
            }

            Repositorio.Embarcador.Fatura.FaturaDocumento repFaturaDocumento = new Repositorio.Embarcador.Fatura.FaturaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.TituloDocumento repTituloDocumento = new Repositorio.Embarcador.Financeiro.TituloDocumento(unitOfWork);

            List<int> numerosFaturasNova = repFaturaDocumento.BuscarNumeroFaturaPorCTe(lancamentoNFSManual.CTe.Codigo);
            if (numerosFaturasNova.Count > 0)
            {
                erro = "A NFS " + lancamentoNFSManual.CTe.Numero + " está vinculada à(s) fatura(s) nº " + string.Join(", ", numerosFaturasNova) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> numerosTitulos = repTituloDocumento.BuscarNumeroTituloPorCTe(lancamentoNFSManual.CTe.Codigo);
            if (numerosTitulos.Count > 0)
            {
                erro = "A NFS " + lancamentoNFSManual.CTe.Numero + " está vinculada ao(s) título(s) nº " + string.Join(", ", numerosTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<int> nossoNumeroBoletoTitulos = repTituloDocumento.BuscarNumeroBoletoTituloPorCTe(lancamentoNFSManual.CTe.Codigo);
            if (nossoNumeroBoletoTitulos.Count > 0)
            {
                erro = "A NFS " + lancamentoNFSManual.CTe.Numero + " está vinculada ao(s) boleto(s) no(s) título(s) nº " + string.Join(", ", nossoNumeroBoletoTitulos) + ", não sendo possível realizar o cancelamento/anulação.";
                return false;
            }

            List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulosParaCancelamentoCTe = repTituloDocumento.BuscarTitulosEmAbertoPorCTe(lancamentoNFSManual.CTe.Codigo);

            if (titulosParaCancelamentoCTe.Count > 0)
                titulosParaCancelamento.AddRange(titulosParaCancelamentoCTe);

            erro = string.Empty;
            return true;
        }


        #endregion
    }
}
