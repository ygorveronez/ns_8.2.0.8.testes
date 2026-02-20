using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManualCancelamento", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualCancelamentoController : BaseController
    {
		#region Construtores

		public CargaMDFeManualCancelamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, codigoCarga, numeroMDFe, codigoTransportador;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("CTe"), out numeroCTe);
                int.TryParse(Request.Params("MDFe"), out numeroMDFe);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoTransportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 12, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenar != "Situacao")
                    propOrdenar = "CargaMDFeManual." + propOrdenar;

                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento> cargasCancelamento = repCargaMDFeManualCancelamento.Consultar(codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repCargaMDFeManualCancelamento.ContarConsulta(codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao));

                var retorno = (from obj in cargasCancelamento
                               select new
                               {
                                   obj.Codigo,
                                   Transportador = obj.CargaMDFeManual.Empresa?.Descricao ?? "",
                                   MDFe = string.Join(", ", obj.CargaMDFeManual.MDFeManualMDFes.Select(o => o.MDFe.Numero)),
                                   Origem = obj.CargaMDFeManual.Origem?.DescricaoCidadeEstado ?? "",
                                   Destino = !obj.CargaMDFeManual.UsarListaDestinos() ? obj.CargaMDFeManual.Destino?.DescricaoCidadeEstado ?? "" : obj.CargaMDFeManual.Destinos != null && obj.CargaMDFeManual.Destinos.Count > 0 ? string.Join(",", (from det in obj.CargaMDFeManual.Destinos orderby det.Ordem select det.Localidade.DescricaoCidadeEstado).ToList()) : "",
                                   Motorista = obj.CargaMDFeManual.Motoristas != null && obj.CargaMDFeManual.Motoristas.Count > 0 ? string.Join(", ", obj.CargaMDFeManual.Motoristas.Select(o => o.Descricao)) : "",
                                   Veiculo = obj.CargaMDFeManual.Veiculo != null ? obj.CargaMDFeManual.Veiculo.Placa + (obj.CargaMDFeManual.Reboques.Count > 0 ? (", " + string.Join(", ", obj.CargaMDFeManual.Reboques.Select(o => o.Placa))) : string.Empty) : "",
                                   Situacao = obj.DescricaoSituacao
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
                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);

                if (cargaMDFeManual == null)
                    return new JsonpResult(false, true, "MDF-e Manual não encontrado.");

                bool mdfePermite = true;
                string mensagem = string.Empty;

                if (cargaMDFeManual.MDFeManualMDFes.Any(cargaMDFe => cargaMDFe.MDFe != null && ((cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && cargaMDFe.MDFe.DataAutorizacao < DateTime.Now.AddDays(-1)) || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)))
                {
                    mdfePermite = false;
                }

                return new JsonpResult(new
                {
                    MDFePermiteCancelamento = mdfePermite
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

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfigEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unidadeTrabalho);
                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                int codigoCargaMDFeManual;
                int.TryParse(Request.Params("CargaMDFeManual"), out codigoCargaMDFeManual);

                string motivo = Request.Params("Motivo");
                if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length <= 20)
                    return new JsonpResult(false, true, "O motivo deve possuir mais de 20 caracteres.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS config = repConfigEmbarcador.BuscarConfiguracaoPadrao();
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoCargaMDFeManual);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento = repCargaMDFeManualCancelamento.BuscarPorCargaMDFeManual(codigoCargaMDFeManual);


                if (cargaMDFeManual.MDFeManualMDFes.Any(o => o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmEncerramento || o.MDFe.Status == Dominio.Enumeradores.StatusMDFe.EmCancelamento))
                    return new JsonpResult(false, true, "Não é possível adicionar um cancelamento para este MDF-e manual pois há um MDF-e em encerramento/cancelamento para a mesma.");

                if (cargaMDFeManual.MDFeManualMDFes.Any(cargaMDFe => cargaMDFe.MDFe != null && ((cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado && cargaMDFe.MDFe.DataAutorizacao < DateTime.Now.AddDays(-1)) || cargaMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Encerrado)))
                    return new JsonpResult(false, true, "Não é possível adicionar um cancelamento para este MDF-e manual pois há um MDF-e já encerrado ou com prazo de cancelamento esgotado.");

                if (cargaMDFeManualCancelamento != null)
                    return new JsonpResult(false, true, "Já existe uma solicitação de cancelamento para este MDF-e Manual.");

                if (cargaMDFeManual.MDFeRecebidoDeIntegracao)
                    return new JsonpResult(false, true, "Não é possível realizar o cancelamento de um MDF-e recebido via integração.");

                unidadeTrabalho.Start();

                cargaMDFeManualCancelamento = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento();

                cargaMDFeManualCancelamento.CargaMDFeManual = cargaMDFeManual;
                cargaMDFeManualCancelamento.DataCancelamento = DateTime.Now;
                cargaMDFeManualCancelamento.MotivoCancelamento = motivo;
                cargaMDFeManualCancelamento.Usuario = Usuario;
                cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.EmCancelamento;

                repCargaMDFeManualCancelamento.Inserir(cargaMDFeManualCancelamento, Auditado);

                //GerarLogCancelamento(cargaMDFeManualCancelamento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoLogCargaMDFeManualCancelamento.Emissao, unidadeTrabalho);

                unidadeTrabalho.CommitChanges();

                EnviarMDFesParaCancelamento(cargaMDFeManualCancelamento, unidadeTrabalho);

                return new JsonpResult(cargaMDFeManualCancelamento.Codigo);
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

                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento = repCargaMDFeManualCancelamento.BuscarPorCodigo(codigoCancelamento);

                if (cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.CancelamentoRejeitado)
                    return new JsonpResult(false, true, "A situação do cancelamento não permite o reenvio do mesmo.");

                unidadeTrabalho.Start();

                cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManualCancelamento.EmCancelamento;

                repCargaMDFeManualCancelamento.Atualizar(cargaMDFeManualCancelamento);

                unidadeTrabalho.CommitChanges();

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualCancelamento.CargaMDFeManual, null, "Reenviou integração do cancelamento dos MDF-es", unidadeTrabalho);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualCancelamento, null, "Reenviou integração do cancelamento dos MDF-es", unidadeTrabalho);
                EnviarMDFesParaCancelamento(cargaMDFeManualCancelamento, unidadeTrabalho);

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

                Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento repCargaMDFeManualCancelamento = new Repositorio.Embarcador.Cargas.CargaMDFeManualCancelamento(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento = repCargaMDFeManualCancelamento.BuscarPorCodigo(codigo);

                if (cargaMDFeManualCancelamento == null)
                    return new JsonpResult(false, true, "Cancelamento não encontrado. Atualize a página e tente novamente.");

                var retorno = new
                {
                    Codigo = cargaMDFeManualCancelamento.Codigo,
                    CargaMDFeManual = new
                    {
                        Codigo = cargaMDFeManualCancelamento.CargaMDFeManual.Codigo,
                        Descricao = string.Join(", ", cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes.Select(o => o.MDFe.Numero)) + " - " + cargaMDFeManualCancelamento.CargaMDFeManual.Origem.DescricaoCidadeEstado + " até " + (!cargaMDFeManualCancelamento.CargaMDFeManual.UsarListaDestinos() ? cargaMDFeManualCancelamento.CargaMDFeManual.Destino?.DescricaoCidadeEstado ?? "" : cargaMDFeManualCancelamento.CargaMDFeManual.Destinos != null && cargaMDFeManualCancelamento.CargaMDFeManual.Destinos.Count > 0 ? string.Join(",", (from obj in cargaMDFeManualCancelamento.CargaMDFeManual.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList()) : ""),
                        Empresa = cargaMDFeManualCancelamento.CargaMDFeManual.Empresa?.Descricao ?? "",
                        MDFe = string.Join(", ", cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes.Select(o => o.MDFe.Numero)),
                        Origem = cargaMDFeManualCancelamento.CargaMDFeManual.Origem?.DescricaoCidadeEstado ?? "",
                        Destino = !cargaMDFeManualCancelamento.CargaMDFeManual.UsarListaDestinos() ? cargaMDFeManualCancelamento.CargaMDFeManual.Destino?.DescricaoCidadeEstado ?? "" : cargaMDFeManualCancelamento.CargaMDFeManual.Destinos != null && cargaMDFeManualCancelamento.CargaMDFeManual.Destinos.Count > 0 ? string.Join(",", (from det in cargaMDFeManualCancelamento.CargaMDFeManual.Destinos orderby det.Ordem select det.Localidade.DescricaoCidadeEstado).ToList()) : "",
                        Motorista = cargaMDFeManualCancelamento.CargaMDFeManual.Motoristas != null && cargaMDFeManualCancelamento.CargaMDFeManual.Motoristas.Count > 0 ? string.Join(", ", cargaMDFeManualCancelamento.CargaMDFeManual.Motoristas.Select(o => o.Descricao)) : "",
                        Veiculo = cargaMDFeManualCancelamento.CargaMDFeManual.Veiculo != null ? cargaMDFeManualCancelamento.CargaMDFeManual.Veiculo.Placa + (cargaMDFeManualCancelamento.CargaMDFeManual.Reboques.Count > 0 ? (", " + string.Join(", ", cargaMDFeManualCancelamento.CargaMDFeManual.Reboques.Select(o => o.Placa))) : string.Empty) : "",
                    },
                    UsuarioSolicitou = cargaMDFeManualCancelamento.Usuario?.Nome,
                    Motivo = cargaMDFeManualCancelamento.MotivoCancelamento,
                    cargaMDFeManualCancelamento.DescricaoSituacao,
                    Situacao = cargaMDFeManualCancelamento.SituacaoMDFeManualCancelamento,
                    MensagemRejeicaoCancelamento = cargaMDFeManualCancelamento.MotivoRejeicaoCancelamento,
                    DataCancelamento = cargaMDFeManualCancelamento.DataCancelamento.Value.ToString("dd/MM/yyyy")
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

        /// <summary>
        /// Quando uma carga é anulada e existem MDFes não encerrados o sistema faz a solicitação para encerramento dos mesmos.
        /// </summary>
        /// <param name="cargaMDFEsEncerramento"></param>
        /// <param name="carga"></param>
        private void EnviarMDFesParaCancelamento(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCancelamento cargaMDFeManualCancelamento, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.MDFe svcMDFe = new Servicos.MDFe(unidadeTrabalho);
            foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualMDFe cargaMDFeManualMDFe in cargaMDFeManualCancelamento.CargaMDFeManual.MDFeManualMDFes)
            {
                if (cargaMDFeManualMDFe.MDFe.Status == Dominio.Enumeradores.StatusMDFe.Autorizado)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManualMDFe.MDFe, null, "Solicitou cancelamento do MDF-es", unidadeTrabalho);
                    Servicos.Embarcador.EmissorDocumento.EmissorDocumentoService.GetEmissorDocumentoMDFe(cargaMDFeManualMDFe.MDFe.SistemaEmissor).CancelarMdfe(cargaMDFeManualMDFe.MDFe.Codigo, cargaMDFeManualMDFe.MDFe.Empresa.Codigo, cargaMDFeManualCancelamento.MotivoCancelamento, unidadeTrabalho);
                }
            }
        }


        #endregion
    }
}
