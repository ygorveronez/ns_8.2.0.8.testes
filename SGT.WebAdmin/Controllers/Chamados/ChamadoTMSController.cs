using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize("Chamados/ChamadoTMS")]
    public class ChamadoTMSController : BaseController
    {
		#region Construtores

		public ChamadoTMSController(Conexao conexao) : base(conexao) { }

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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMS();

                PreencherChamadoTMS(chamado, unitOfWork);
                SalvarFormaPagamento(chamado, unitOfWork);

                repChamado.Inserir(chamado, Auditado);

                SalvarConhecimentos(chamado, unitOfWork);
                SalvarCustosAdicionais(chamado, unitOfWork);
                SalvarChapas(chamado, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(chamado.Codigo);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                PreencherChamadoTMS(chamado, unitOfWork);
                SalvarFormaPagamento(chamado, unitOfWork);

                repChamado.Atualizar(chamado, Auditado);

                SalvarConhecimentos(chamado, unitOfWork);
                SalvarCustosAdicionais(chamado, unitOfWork);
                SalvarChapas(chamado, unitOfWork);

                unitOfWork.CommitChanges();
                return new JsonpResult(chamado.Codigo);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    //ETAPA 1
                    chamado.Codigo,
                    MotivoChamado = chamado.MotivoChamado.Codigo,
                    chamado.Situacao,
                    CodigoResponsavel = chamado.Responsavel?.Codigo ?? 0,
                    CodigoAutor = chamado.Autor?.Codigo ?? 0,
                    Responsavel = new { Codigo = chamado.Responsavel?.Codigo ?? 0, Descricao = chamado.Responsavel?.Descricao ?? string.Empty },
                    PodeEditar = (chamado.Responsavel?.Codigo ?? 0) == this.Usuario.Codigo,
                    PossuiResponsavel = chamado.Responsavel != null,
                    PermiteAssumirChamadoMesmoSetor = chamado?.Responsavel?.Setor?.PermitirAssumirChamadosDoMesmoSetor ?? false,
                    Abertura = new
                    {
                        chamado.Numero,
                        Carga = new { chamado.Carga.Codigo, Descricao = chamado.Carga.CodigoCargaEmbarcador },
                        MotivoChamado = new { chamado.MotivoChamado.Codigo, chamado.MotivoChamado.Descricao },
                        Motorista = new { Codigo = chamado.Motorista?.Codigo ?? 0, Descricao = chamado.Motorista?.Nome ?? "" },
                        chamado.FormaCobranca,
                        QuantidadeFormaCobranca = chamado.QuantidadeFormaCobranca.ToString("n2"),
                        ValorUnitario = chamado.ValorUnitario.ToString("n2"),
                        ValorTotal = chamado.ValorTotal.ToString("n2"),
                        chamado.NumeroOrdemColeta,
                        chamado.CelularMotorista,
                        chamado.Observacao,

                        CustosAdicionais = (
                            from obj in chamado.CustosAdicional
                            select new
                            {
                                obj.Codigo,
                                PedidoTipoPagamento = new { obj.PedidoTipoPagamento.Codigo, obj.PedidoTipoPagamento.Descricao },
                                QuantidadeCustoExtra = obj.QuantidadeCustoExtra.ToString("n2"),
                                ValorUnitario = obj.ValorUnitario.ToString("n2"),
                                ValorTotal = obj.ValorTotal.ToString("n2"),
                            }
                        ).ToList(),
                        Chapas = (
                            from obj in chamado.Chapas
                            select new
                            {
                                obj.Codigo,
                                obj.Nome,
                                CPF = obj.CPF.ObterCpfFormatado(),
                                obj.Telefone
                            }
                        ).ToList(),
                    },
                    FormaPagamento = new
                    {
                        chamado.MotoristaPagouSemAutorizacao,
                        chamado.GestorLogisticaAutorizouPagamento,
                        chamado.FormaPagamento,
                        ValorAdiantamento = chamado.ValorAdiantamento.ToString("n2"),
                        OutroMotorista = new { Codigo = chamado.OutroMotorista?.Codigo ?? 0, Descricao = chamado.OutroMotorista?.Nome ?? "" },
                        chamado.NomeTerceiro,
                        chamado.CnpjCpfTerceiro,
                        chamado.TipoContaBanco,
                        chamado.Agencia,
                        chamado.NumeroConta,
                        Banco = new { Codigo = chamado.Banco?.Codigo ?? 0, Descricao = chamado.Banco?.Descricao ?? "" }
                    },
                    Anexos = (
                        from obj in chamado.Anexos
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),

                    //ETAPA 2
                    AutorizacaoCliente = new
                    {
                        chamado.Codigo,
                        chamado.NumeroOcorrencia,
                        DataRetornoAutorizacaoCliente = chamado.DataRetornoAutorizacaoCliente.HasValue ? chamado.DataRetornoAutorizacaoCliente.Value.ToString("dd/MM/yyyy") : string.Empty,
                        chamado.SituacaoAutorizacaoCliente,
                        ValorAprovacaoParcial = chamado.ValorAprovacaoParcial.ToString("n2"),
                        chamado.ObservacaoAutorizacaoCliente,
                        ContatoGrupoPessoa = new { Codigo = chamado.ContatoGrupoPessoa?.Codigo ?? 0, Descricao = chamado.ContatoGrupoPessoa?.Descricao ?? "" },
                        chamado.AssuntoEmail,
                        chamado.CorpoEmail,
                        AdiantamentosMotorista = (
                            from obj in chamado.AdiantamentosMotorista
                            select new
                            {
                                obj.Codigo,
                                PagamentoMotoristaTipo = new { Codigo = obj.PagamentoMotoristaTipo?.Codigo ?? 0, Descricao = obj.PagamentoMotoristaTipo?.Descricao ?? string.Empty },
                                DataPagamento = obj.DataPagamento.ToString("dd/MM/yyyy"),
                                Valor = obj.Valor.ToString("n2"),
                                obj.Observacao
                            }
                        ).ToList(),
                    },
                    AnexosEmail = (
                        from obj in chamado.AnexosEmail
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                    AnexosAdiantamentoMotorista = (
                        from obj in chamado.AnexosAdiantamentoMotorista
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList(),
                    OrientacaoMotorista = new
                    {
                        chamado.Codigo,
                        chamado.MensagemOrientacaoMotorista,
                        DataUltimoEnvioMensagemOrientacaoMotorista = chamado.DataUltimoEnvioMensagemOrientacaoMotorista.HasValue ? chamado.DataUltimoEnvioMensagemOrientacaoMotorista.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
                    },

                    //ETAPA 3
                    DocumentoAnalise = new
                    {
                        chamado.Codigo,
                        DataDocumentoRecebido = chamado.DataDocumentoRecebido.HasValue ? chamado.DataDocumentoRecebido.Value.ToString("dd/MM/yyyy") : string.Empty,
                        ValorRecibo = chamado.ValorRecibo > 0 ? chamado.ValorRecibo.ToString("n2") : string.Empty,
                        chamado.NumeroDocumento,
                        chamado.ObservacaoDocumento
                    },
                    AutorizacaoAnalise = new
                    {
                        chamado.Codigo,
                        NovoValorAutorizado = chamado.NovoValorAutorizado > 0 ? chamado.NovoValorAutorizado.ToString("n2") : string.Empty,
                        chamado.FormaAutorizacaoPagamento,
                        chamado.JustificativaAutorizacao
                    },
                    AnexosDocumentoAnalise = (
                        from obj in chamado.AnexosDocumentoAnalise
                        select new
                        {
                            obj.Codigo,
                            obj.Descricao,
                            obj.NomeArquivo,
                        }
                    ).ToList()
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> ReabrirChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Situacao != SituacaoChamadoTMS.Cancelado)
                    return new JsonpResult(false, true, "Só é possível reabrir um chamado cancelado.");

                chamado.Situacao = SituacaoChamadoTMS.Aberto;
                repChamado.Atualizar(chamado, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reabrir o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AssumirChamado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.ChamadoTMS repositorioChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repositorioChamado.BuscarPorCodigo(codigo, false);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Responsavel != null)
                {
                    if (chamado.Responsavel.Codigo == this.Usuario.Codigo)
                        return new JsonpResult(false, true, "Você já é o responsável por esse chamado.");

                    if (!(ConfiguracaoEmbarcador?.PermitirAssumirChamadoDeOutroResponsavel ?? false) && !(this.Usuario?.Setor?.PermitirAssumirChamadosDoMesmoSetor ?? false))
                        return new JsonpResult(false, true, "Já existe um responsável para esse chamado.");

                    if (this.Usuario?.Setor != null && this.Usuario.Setor.PermitirAssumirChamadosDoMesmoSetor)
                    {
                        if (chamado.Responsavel.Setor != this.Usuario.Setor)
                            return new JsonpResult(false, true, "Você só pode assumir chamados de responsáveis do mesmo setor.");
                    }
                }

                unitOfWork.Start();

                chamado.Responsavel = this.Usuario;

                repositorioChamado.Atualizar(chamado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, "Assumiu a responsabilidade do Chamado.", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(chamado.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao assumir o chamado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PesquisaConhecimentosCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMSCTe repChamadoTMSCTe = new Repositorio.Embarcador.Chamados.ChamadoTMSCTe(unitOfWork);

                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                Models.Grid.EditableCell editableValorDescarga = null;
                editableValorDescarga = new Models.Grid.EditableCell(TipoColunaGrid.aDecimal, 9);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCTe", false);
                grid.AdicionarCabecalho("CTe", "Numero", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.center, false);

                grid.AdicionarCabecalho("Remetente", "Remetente", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Início Prestação", "InicioPrestacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Fim Prestação", "FimPrestacao", 10, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Qtd. Caixas", "Caixas", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Peso", "Peso", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Comp. Descarga", "ComponenteDescarga", 8, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("Valor Descarga", "ValorDescarga", 8, Models.Grid.Align.right, false, false, false, false, true, editableValorDescarga);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                if (codigo > 0)
                {
                    List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe> listaConhecimentos = repChamadoTMSCTe.ConsultarPorChamado(codigo, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repChamadoTMSCTe.ContarConsultarPorChamado(codigo));

                    var lista = (from p in listaConhecimentos
                                 select new
                                 {
                                     p.Codigo,
                                     CodigoCTe = p.CTe.Codigo,
                                     p.CTe.Numero,
                                     DataEmissao = p.CTe.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                     Remetente = p.CTe.Remetente.Nome,
                                     Destinatario = p.CTe.Destinatario.Nome,
                                     InicioPrestacao = p.CTe.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                     FimPrestacao = p.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                     Caixas = p.CTe.Volumes.ToString("n2"),
                                     Peso = p.CTe.Peso.ToString("n3"),
                                     ComponenteDescarga = p.CTe.ComponentesPrestacao?.Sum(s => s.ValorDescarga).ToString("n2"),
                                     ValorDescarga = p.ValorDescarga.ToString("n2")
                                 }).ToList();

                    grid.AdicionaRows(lista);
                }
                else
                {
                    List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaConhecimentos = repChamadoTMSCTe.ConsultarCTesPorCarga(codigoCarga, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                    grid.setarQuantidadeTotal(repChamadoTMSCTe.ContarConsultarCTesPorCarga(codigoCarga));

                    var lista = (from p in listaConhecimentos
                                 select new
                                 {
                                     Codigo = 0,
                                     CodigoCTe = p.Codigo,
                                     p.Numero,
                                     DataEmissao = p.DataEmissao.Value.ToString("dd/MM/yyyy"),
                                     Remetente = p.Remetente.Nome,
                                     Destinatario = p.Destinatario.Nome,
                                     InicioPrestacao = p.LocalidadeInicioPrestacao.DescricaoCidadeEstado,
                                     FimPrestacao = p.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                                     Caixas = p.Volumes.ToString("n2"),
                                     Peso = p.Peso.ToString("n3"),
                                     ComponenteDescarga = p.ComponentesPrestacao?.Sum(s => s.ValorDescarga).ToString("n2"),
                                     ValorDescarga = 0.ToString("n2")
                                 }).ToList();

                    grid.AdicionaRows(lista);
                }

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar os CTes da carga.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarEtapa2()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                SalvarAutorizacaoCliente(chamado, unitOfWork);
                SalvarOrientacaoMotorista(chamado, unitOfWork);
                SalvarAdiantamentosMotorista(chamado, unitOfWork);

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarEtapaAnalise()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.ChamadoTMS repositorioChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnalise repChamadoAnalise = new Repositorio.Embarcador.Chamados.ChamadoTMSAnalise(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repositorioChamado.BuscarPorCodigo(codigo, false);

                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (chamado.Situacao != SituacaoChamadoTMS.Aberto)
                    return new JsonpResult(false, true, "Situação do chamado não permite iniciar a análise.");

                unitOfWork.Start();

                chamado.Situacao = SituacaoChamadoTMS.EmAnalise;

                repositorioChamado.Atualizar(chamado);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, chamado, null, "Iniciou a análise do Chamado.", unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise abertura = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnalise()
                {
                    Chamado = chamado,
                    Autor = chamado.Autor,
                    DataCriacao = DateTime.Now
                };
                abertura.Observacao = $"Chamado aberto por { abertura.Autor.Nome}, dia {chamado.DataCriacao.ToString("dd/MM/yyyy")} às {chamado.DataCriacao.ToString("HH:mm")}.\nMotivo: {chamado.MotivoChamado.Descricao}.";
                repChamadoAnalise.Inserir(abertura);

                unitOfWork.CommitChanges();
                return new JsonpResult(chamado.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar a análise.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarPorEmail()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail repChamadoTMSAnexoEmail = new Repositorio.Embarcador.Chamados.ChamadoTMSAnexoEmail(unitOfWork);
                Repositorio.Embarcador.Email.ConfigEmailDocTransporte repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte email = repConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

                if (email == null)
                    throw new Exception("Não há um e-mail configurado para realizar o envio.");

                int codigo = Request.GetIntParam("Codigo");
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoArquivos, "Anexos", "ChamadoTMS");

                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, false);
                if (chamado == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                List<System.Net.Mail.Attachment> attachments = new List<System.Net.Mail.Attachment>();
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail> anexosEmail = repChamadoTMSAnexoEmail.BuscarPorChamado(chamado.Codigo);
                foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAnexoEmail anexo in anexosEmail)
                {
                    string extencao = System.IO.Path.GetExtension(anexo.NomeArquivo).ToLower();
                    string arquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminho, anexo.GuidArquivo + extencao);
                    attachments.Add(new System.Net.Mail.Attachment(arquivo));
                }

                string assunto = PreencherTags(chamado, chamado.AssuntoEmail, unitOfWork);
                string mensagemEmail = PreencherTags(chamado, chamado.CorpoEmail, unitOfWork);
                if (!string.IsNullOrWhiteSpace(email.MensagemRodape))
                    mensagemEmail += "<br/>" + "<br/>" + "<br/>" + email.MensagemRodape.Replace("#qLinha#", "<br/>");
                string mensagemErro = "Erro ao enviar e-mail";

                List<string> emails = new List<string>();
                foreach (Dominio.Entidades.Embarcador.Pessoas.ContatoGrupoPessoaDado contato in chamado.ContatoGrupoPessoa.Contatos)
                {
                    if (!string.IsNullOrWhiteSpace(contato.Email))
                        emails.AddRange(contato.Email.Split(';').ToList());
                }

                emails = emails.Distinct().ToList();
                if (emails.Count == 0)
                    return new JsonpResult(false, true, "Não foi possível encontrar e-mails para o envio.");
                else
                {
                    bool sucesso = Servicos.Email.EnviarEmail(email.Email, email.Email, email.Senha, "", emails.ToArray(), null, assunto, mensagemEmail, email.Smtp, out mensagemErro, email.DisplayEmail, attachments, "", email.RequerAutenticacaoSmtp, "", email.PortaSmtp, unitOfWork);
                    if (!sucesso)
                        return new JsonpResult(false, false, "Problemas ao enviar a ordem de serviço por e-mail: " + mensagemErro);
                }

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar por e-mail. " + ex.Message);
            }
        }

        public async Task<IActionResult> RetornaMensagemOrientacaoMotorista()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado = repChamado.BuscarPorCodigo(codigo, true);

                chamado.MensagemOrientacaoMotorista = Request.GetStringParam("MensagemOrientacaoMotorista");
                chamado.DataUltimoEnvioMensagemOrientacaoMotorista = DateTime.Now;

                repChamado.Atualizar(chamado, Auditado);

                unitOfWork.CommitChanges();

                string retorno = PreencherTags(chamado, chamado.MensagemOrientacaoMotorista, unitOfWork);
                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao montar mensagem.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PreencherChamadoTMS(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoTMS repChamado = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Chamados.MotivoChamado repMotivoChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            int codigoCarga = Request.GetIntParam("Carga");
            int codigoMotivoChamado = Request.GetIntParam("MotivoChamado");
            int codigoMotorista = Request.GetIntParam("Motorista");

            chamado.Carga = repCarga.BuscarPorCodigo(codigoCarga);
            chamado.MotivoChamado = repMotivoChamado.BuscarPorCodigo(codigoMotivoChamado);
            chamado.Motorista = repUsuario.BuscarPorCodigo(codigoMotorista);

            if (chamado.Carga == null)
                throw new ControllerException("Carga é obrigatória.");

            if (chamado.MotivoChamado == null)
                throw new ControllerException("Motivo é obrigatório.");

            if (chamado.Motorista == null)
                throw new ControllerException("Motorista é obrigatório.");

            if (chamado.Codigo == 0)
            {
                chamado.Numero = repChamado.BuscarProximoNumero();
                chamado.Situacao = SituacaoChamadoTMS.Aberto;
                chamado.DataCriacao = DateTime.Now;
                chamado.Autor = Usuario;
                chamado.Responsavel = Usuario;
            }

            chamado.DataFinalizacao = null;
            chamado.CelularMotorista = Utilidades.String.OnlyNumbers(Request.GetStringParam("CelularMotorista"));
            chamado.NumeroOrdemColeta = Request.GetStringParam("NumeroOrdemColeta");
            chamado.FormaCobranca = Request.GetEnumParam<FormaCobrancaChamado>("FormaCobranca");
            chamado.QuantidadeFormaCobranca = Request.GetDecimalParam("QuantidadeFormaCobranca");
            chamado.ValorUnitario = Request.GetDecimalParam("ValorUnitario");
            chamado.ValorTotal = Request.GetDecimalParam("ValorTotal");
            chamado.Observacao = Request.GetStringParam("Observacao");
        }

        private void SalvarFormaPagamento(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Banco repBanco = new Repositorio.Banco(unitOfWork);

            dynamic formaPagamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("FormaPagamento"));

            int outroMotorista = ((string)formaPagamento.OutroMotorista).ToInt();
            int banco = ((string)formaPagamento.Banco).ToInt();

            chamado.MotoristaPagouSemAutorizacao = ((string)formaPagamento.MotoristaPagouSemAutorizacao).ToBool();
            chamado.GestorLogisticaAutorizouPagamento = ((string)formaPagamento.GestorLogisticaAutorizouPagamento).ToBool();

            chamado.FormaPagamento = ((string)formaPagamento.FormaPagamento).ToEnum<FormaPagamentoChamado>();
            chamado.TipoContaBanco = ((string)formaPagamento.TipoContaBanco).ToEnum<TipoContaBanco>();

            chamado.ValorAdiantamento = ((string)formaPagamento.ValorAdiantamento).ToDecimal();

            chamado.NomeTerceiro = (string)formaPagamento.NomeTerceiro;
            chamado.CnpjCpfTerceiro = Utilidades.String.OnlyNumbers((string)formaPagamento.CnpjCpfTerceiro);
            chamado.Agencia = (string)formaPagamento.Agencia;
            chamado.NumeroConta = (string)formaPagamento.NumeroConta;

            chamado.OutroMotorista = outroMotorista > 0 ? repUsuario.BuscarPorCodigo(outroMotorista) : null;
            chamado.Banco = outroMotorista > 0 ? repBanco.BuscarPorCodigo(banco) : null;
        }

        private void SalvarCustosAdicionais(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoTMSCustoAdicional repCustoAdicional = new Repositorio.Embarcador.Chamados.ChamadoTMSCustoAdicional(unitOfWork);

            dynamic dynCustosAdicionais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("CustosAdicionais"));

            if (chamado.CustosAdicional != null && chamado.CustosAdicional.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var custoAdicional in dynCustosAdicionais)
                    if (custoAdicional.Codigo != null)
                        codigos.Add((int)custoAdicional.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCustoAdicional> custoAdicionalDeletar = (from obj in chamado.CustosAdicional where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < custoAdicionalDeletar.Count; i++)
                    repCustoAdicional.Deletar(custoAdicionalDeletar[i], Auditado);
            }
            else
                chamado.CustosAdicional = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCustoAdicional>();

            foreach (var custoAdicional in dynCustosAdicionais)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCustoAdicional chamadoTMSCustoAdicional = custoAdicional.Codigo != null ? repCustoAdicional.BuscarPorCodigo((int)custoAdicional.Codigo, true) : null;
                if (chamadoTMSCustoAdicional == null)
                    chamadoTMSCustoAdicional = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCustoAdicional();

                int codigoPedidoTipoPagamento = ((string)custoAdicional.PedidoTipoPagamento.Codigo).ToInt();

                chamadoTMSCustoAdicional.QuantidadeCustoExtra = Utilidades.Decimal.Converter((string)custoAdicional.QuantidadeCustoExtra);
                chamadoTMSCustoAdicional.ValorUnitario = Utilidades.Decimal.Converter((string)custoAdicional.ValorUnitario);
                chamadoTMSCustoAdicional.ValorTotal = Utilidades.Decimal.Converter((string)custoAdicional.ValorTotal);

                chamadoTMSCustoAdicional.PedidoTipoPagamento = codigoPedidoTipoPagamento > 0 ? repPedidoTipoPagamento.BuscarPorCodigo(codigoPedidoTipoPagamento) : null;
                chamadoTMSCustoAdicional.Chamado = chamado;

                if (chamadoTMSCustoAdicional.Codigo > 0)
                    repCustoAdicional.Atualizar(chamadoTMSCustoAdicional, Auditado);
                else
                    repCustoAdicional.Inserir(chamadoTMSCustoAdicional, Auditado);
            }
        }

        private void SalvarChapas(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoTipoPagamento repPedidoTipoPagamento = new Repositorio.Embarcador.Pedidos.PedidoTipoPagamento(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoTMSChapa repChapa = new Repositorio.Embarcador.Chamados.ChamadoTMSChapa(unitOfWork);

            dynamic dynChapas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("Chapas"));

            if (chamado.Chapas != null && chamado.Chapas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var chapa in dynChapas)
                    if (chapa.Codigo != null)
                        codigos.Add((int)chapa.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSChapa> chapaDeletar = (from obj in chamado.Chapas where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < chapaDeletar.Count; i++)
                    repChapa.Deletar(chapaDeletar[i], Auditado);
            }
            else
                chamado.Chapas = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSChapa>();

            foreach (var chapa in dynChapas)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSChapa chamadoTMSChapa = chapa.Codigo != null ? repChapa.BuscarPorCodigo((int)chapa.Codigo, true) : null;
                if (chamadoTMSChapa == null)
                    chamadoTMSChapa = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSChapa();

                chamadoTMSChapa.Nome = (string)chapa.Nome;
                chamadoTMSChapa.CPF = Utilidades.String.OnlyNumbers((string)chapa.CPF);
                chamadoTMSChapa.Telefone = Utilidades.String.OnlyNumbers((string)chapa.Telefone);

                chamadoTMSChapa.Chamado = chamado;

                if (chamadoTMSChapa.Codigo > 0)
                    repChapa.Atualizar(chamadoTMSChapa, Auditado);
                else
                    repChapa.Inserir(chamadoTMSChapa, Auditado);
            }
        }

        private void SalvarConhecimentos(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoTMSCTe repChamadoCTe = new Repositorio.Embarcador.Chamados.ChamadoTMSCTe(unitOfWork);

            dynamic dynCTes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ListaSelecaoConhecimentos"));

            if (chamado.CTes != null && chamado.CTes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var cte in dynCTes)
                    if (cte.Codigo != null)
                        codigos.Add((int)cte.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe> cteDeletar = (from obj in chamado.CTes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < cteDeletar.Count; i++)
                    repChamadoCTe.Deletar(cteDeletar[i], Auditado);
            }
            else
                chamado.CTes = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe>();

            foreach (var cte in dynCTes)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe chamadoTMSCTe = cte.Codigo != null ? repChamadoCTe.BuscarPorCodigo((int)cte.Codigo, true) : null;
                if (chamadoTMSCTe == null)
                    chamadoTMSCTe = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe();

                int codigoCTe = ((string)cte.CodigoCTe).ToInt();

                chamadoTMSCTe.ValorDescarga = Utilidades.Decimal.Converter((string)cte.ValorDescarga);

                chamadoTMSCTe.CTe = codigoCTe > 0 ? repCTe.BuscarPorCodigo(codigoCTe, false) : null;
                chamadoTMSCTe.Chamado = chamado;

                if (chamadoTMSCTe.Codigo > 0)
                    repChamadoCTe.Atualizar(chamadoTMSCTe, Auditado);
                else
                    repChamadoCTe.Inserir(chamadoTMSCTe, Auditado);
            }
        }

        private void SalvarAutorizacaoCliente(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa repContatoGrupoPessoa = new Repositorio.Embarcador.Pessoas.ContatoGrupoPessoa(unitOfWork);

            int codigoContatoGrupoPessoa = Request.GetIntParam("ContatoGrupoPessoa");

            chamado.NumeroOcorrencia = Request.GetStringParam("NumeroOcorrencia");
            chamado.DataRetornoAutorizacaoCliente = Request.GetNullableDateTimeParam("DataRetornoAutorizacaoCliente");
            chamado.SituacaoAutorizacaoCliente = Request.GetEnumParam<SituacaoAutorizacaoClienteChamado>("SituacaoAutorizacaoCliente");
            chamado.ValorAprovacaoParcial = Request.GetDecimalParam("ValorAprovacaoParcial");
            chamado.ObservacaoAutorizacaoCliente = Request.GetStringParam("ObservacaoAutorizacaoCliente");

            chamado.AssuntoEmail = Request.GetStringParam("AssuntoEmail");
            chamado.CorpoEmail = Request.GetStringParam("CorpoEmail");
            chamado.ContatoGrupoPessoa = codigoContatoGrupoPessoa > 0 ? repContatoGrupoPessoa.BuscarPorCodigo(codigoContatoGrupoPessoa, false) : null;
        }

        private void SalvarOrientacaoMotorista(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic orientacaoMotorista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("OrientacaoMotorista"));

            chamado.MensagemOrientacaoMotorista = (string)orientacaoMotorista.MensagemOrientacaoMotorista;
        }

        private void SalvarAdiantamentosMotorista(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(unitOfWork);
            Repositorio.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista repAdiantamentoMotorista = new Repositorio.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista(unitOfWork);

            dynamic dynAdiantamentosMotorista = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("AdiantamentosMotorista"));

            if (chamado.AdiantamentosMotorista != null && chamado.AdiantamentosMotorista.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var adiantamentoMotorista in dynAdiantamentosMotorista)
                    if (adiantamentoMotorista.Codigo != null)
                        codigos.Add((int)adiantamentoMotorista.Codigo);

                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista> adiantamentoMotoristaDeletar = (from obj in chamado.AdiantamentosMotorista where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < adiantamentoMotoristaDeletar.Count; i++)
                    repAdiantamentoMotorista.Deletar(adiantamentoMotoristaDeletar[i], Auditado);
            }
            else
                chamado.AdiantamentosMotorista = new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista>();

            foreach (var adiantamentoMotorista in dynAdiantamentosMotorista)
            {
                Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista chamadoTMSAdiantamentoMotorista = adiantamentoMotorista.Codigo != null ? repAdiantamentoMotorista.BuscarPorCodigo((int)adiantamentoMotorista.Codigo, true) : null;
                if (chamadoTMSAdiantamentoMotorista == null)
                    chamadoTMSAdiantamentoMotorista = new Dominio.Entidades.Embarcador.Chamados.ChamadoTMSAdiantamentoMotorista();

                int codigoPagamentoMotoristaTipo = ((string)adiantamentoMotorista.PagamentoMotoristaTipo.Codigo).ToInt();

                chamadoTMSAdiantamentoMotorista.DataPagamento = ((string)adiantamentoMotorista.DataPagamento).ToDateTime();
                chamadoTMSAdiantamentoMotorista.Valor = Utilidades.Decimal.Converter((string)adiantamentoMotorista.Valor);
                chamadoTMSAdiantamentoMotorista.Observacao = (string)adiantamentoMotorista.Observacao;

                chamadoTMSAdiantamentoMotorista.PagamentoMotoristaTipo = codigoPagamentoMotoristaTipo > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigo(codigoPagamentoMotoristaTipo) : null;
                chamadoTMSAdiantamentoMotorista.Chamado = chamado;

                if (chamadoTMSAdiantamentoMotorista.Codigo > 0)
                    repAdiantamentoMotorista.Atualizar(chamadoTMSAdiantamentoMotorista, Auditado);
                else
                    repAdiantamentoMotorista.Inserir(chamadoTMSAdiantamentoMotorista, Auditado);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                CodigoMotivoChamado = Request.GetIntParam("MotivoChamado"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                SituacaoChamado = Request.GetEnumParam("SituacaoChamado", SituacaoChamadoTMS.Todos)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoCarga", false);
                grid.AdicionarCabecalho("Número", "Numero", 3, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Carga", "Carga", 4, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motivo Chamado", "MotivoChamado", 6, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 9, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 6, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Tempo Chamado", "TempoChamado", 5, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaChamadoTMS filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                Repositorio.Embarcador.Chamados.ChamadoTMS repChamadoTMS = new Repositorio.Embarcador.Chamados.ChamadoTMS(unitOfWork);
                int totalRegistros = repChamadoTMS.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS> listaChamado = (totalRegistros > 0) ? repChamadoTMS.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMS>();

                var listaChamadoRetornar = (
                    from obj in listaChamado
                    select new
                    {
                        obj.Codigo,
                        obj.Numero,
                        Carga = obj.Carga.CodigoCargaEmbarcador,
                        CodigoCarga = obj.Carga.Codigo,
                        Motorista = obj.Motorista.Nome,
                        MotivoChamado = obj.MotivoChamado.Descricao,
                        DataCriacao = obj.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                        TempoChamado = this.ObterTempoChamado(obj),
                        Situacao = obj.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaChamadoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "Carga")
                return "Carga.CodigoCargaEmbarcador";

            if (propriedadeOrdenar == "Motorista")
                return "Motorista.Nome";

            return propriedadeOrdenar;
        }

        private string ObterTempoChamado(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado)
        {
            DateTime dataFinalizacao = DateTime.Now;

            if (chamado.Situacao == SituacaoChamadoTMS.Finalizado && chamado.DataFinalizacao.HasValue)
                dataFinalizacao = chamado.DataFinalizacao.Value;

            TimeSpan tempoTotal = (dataFinalizacao - chamado.DataCriacao);

            int dias = tempoTotal.Days;
            string tempo = "";

            if (dias > 0)
                tempo += $"{dias} dia{(dias > 1 ? "s" : "")} ";

            tempo += tempoTotal.ToString(@"hh\:mm");

            return tempo.Trim();
        }

        private string PreencherTags(Dominio.Entidades.Embarcador.Chamados.ChamadoTMS chamado, string propriedade, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.ChamadoTMSCTe repChamadoTMSCTe = new Repositorio.Embarcador.Chamados.ChamadoTMSCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe> ctesChamado = repChamadoTMSCTe.BuscarPorChamado(chamado.Codigo);

            string numeroSerieCTe = "";
            string inicioPrestacao = "";
            string remetente = "";
            string destinatario = "";
            string fimPrestacao = "";
            string dataEmissao = "";
            decimal valorFrete = 0;
            decimal valorIcmsIncluso = 0;
            string notaFiscal = "";

            foreach (Dominio.Entidades.Embarcador.Chamados.ChamadoTMSCTe chamadoCTe in ctesChamado)
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = chamadoCTe.CTe;
                if (string.IsNullOrWhiteSpace(numeroSerieCTe))
                {
                    numeroSerieCTe = cte.Descricao;
                    inicioPrestacao = cte.LocalidadeInicioPrestacao.DescricaoCidadeEstado;
                    remetente = cte.Remetente.Nome;
                    destinatario = cte.Destinatario.Nome;
                    fimPrestacao = cte.LocalidadeTerminoPrestacao.DescricaoCidadeEstado;
                    dataEmissao = cte.DataEmissao?.ToString("dd/MM/yyyy");
                    valorFrete = cte.ValorFrete;
                    valorIcmsIncluso = cte.ValorICMSIncluso;
                    notaFiscal = cte.NumeroNotas;
                }
                else
                {
                    numeroSerieCTe += ", " + cte.Descricao;
                    inicioPrestacao += ", " + cte.LocalidadeInicioPrestacao.DescricaoCidadeEstado;
                    remetente += ", " + cte.Remetente.Nome;
                    destinatario += ", " + cte.Destinatario.Nome;
                    fimPrestacao += ", " + cte.LocalidadeTerminoPrestacao.DescricaoCidadeEstado;
                    dataEmissao += ", " + cte.DataEmissao?.ToString("dd/MM/yyyy");
                    valorFrete += cte.ValorFrete;
                    valorIcmsIncluso += cte.ValorICMSIncluso;
                    notaFiscal += ", " + cte.NumeroNotas;
                }
            }

            propriedade = propriedade.Replace("#NumeroChamado", chamado.Numero.ToString());
            propriedade = propriedade.Replace("#DataEmissaoViagem", dataEmissao);
            propriedade = propriedade.Replace("#NumeroSerieCTe", numeroSerieCTe);
            propriedade = propriedade.Replace("#NumeroCarga", chamado.Carga.CodigoCargaEmbarcador);
            propriedade = propriedade.Replace("#NumeroPedidoEmbarcador", string.Join(", ", chamado.Carga.Pedidos.Select(p => p.Pedido.NumeroPedidoEmbarcador)));
            propriedade = propriedade.Replace("#InicioPrestacao", inicioPrestacao);
            propriedade = propriedade.Replace("#Remetente", remetente);
            propriedade = propriedade.Replace("#Destinatario", destinatario);
            propriedade = propriedade.Replace("#Frota", chamado.Carga.NumeroFrotasVeiculos);
            propriedade = propriedade.Replace("#Veiculo", chamado.Carga.PlacasVeiculos);
            propriedade = propriedade.Replace("#ValorTotal", valorFrete.ToString("n2"));
            propriedade = propriedade.Replace("#FimPrestacao", fimPrestacao);
            propriedade = propriedade.Replace("#ModeloVeicular", chamado.Carga.ModeloVeicularCarga?.Descricao);
            propriedade = propriedade.Replace("#FormaCobranca", chamado.FormaCobranca.ObterDescricao());
            propriedade = propriedade.Replace("#ValorUnitario", chamado.ValorUnitario.ToString("n2"));
            propriedade = propriedade.Replace("#Motorista", chamado.Motorista.Nome);
            propriedade = propriedade.Replace("#ValorCobrar", valorFrete.ToString("n2"));
            propriedade = propriedade.Replace("#QuantidadeFormaCobranca", chamado.QuantidadeFormaCobranca.ToString("n2"));
            propriedade = propriedade.Replace("#CustosAdicionais", chamado.CustosAdicional.Sum(c => c.ValorTotal).ToString("n2"));
            propriedade = propriedade.Replace("#ValorInclusoFrete", valorIcmsIncluso.ToString("n2"));
            propriedade = propriedade.Replace("#NotaFiscal", notaFiscal);

            return propriedade.Trim();
        }

        #endregion
    }
}
