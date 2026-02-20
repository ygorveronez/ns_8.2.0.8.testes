using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { }, "Pallets/Transferencia")]
    public class TransferenciaController : BaseController
    {
		#region Construtores

		public TransferenciaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarEnvio()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (transferencia.Situacao != SituacaoTransferenciaPallet.AguardandoEnvio)
                    return new JsonpResult(false, true, "A situação da transferência não permite realizar o envio.");

                try
                {
                    transferencia.Envio = ObterEnvioAdicionar(unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                AtualizarAprovacao(unitOfWork, transferencia);

                repositorio.Atualizar(transferencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, transferencia, $"Adicionado o envio", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao enviar a transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarRecebimento()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (transferencia.Situacao != SituacaoTransferenciaPallet.AguardandoRecebimento)
                    return new JsonpResult(false, true, "A situação da transferência não permite realizar a finalização.");

                try
                {
                    transferencia.Situacao = SituacaoTransferenciaPallet.Finalizada;
                    transferencia.Recebimento = ObterRecebimentoAdicionar(unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                repositorio.Atualizar(transferencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, transferencia, $"Adicionado o recebimento", unitOfWork);

                InserirMovimentacaoEstoqueRecebimento(transferencia, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao finalizar a transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarSolicitacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = new Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet() {
                    Numero = repositorio.BuscarProximoNumero(),
                    Situacao = SituacaoTransferenciaPallet.AguardandoEnvio,
                    DataTransferencia = DateTime.Now
                };
                
                try
                {
                    transferencia.Solicitacao = ObterSolicitacaoAdicionar(unitOfWork);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }

                repositorio.Inserir(transferencia, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar a transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    transferencia.Codigo,
                    transferencia.Situacao,
                    Envio = ObterEnvio(transferencia),
                    Solicitacao = ObterSolicitacao(transferencia),
                    Recebimento = ObterRecebimento(transferencia),
                    Resumo = ObterResumo(transferencia),
                    ResumoAprovacao = ObterResumoAprovacao(unitOfWork, transferencia)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarPorCodigo()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (transferencia.Situacao != SituacaoTransferenciaPallet.AguardandoEnvio)
                    return new JsonpResult(false, true, "A situação da transferência não permite realizar o cancelamento.");

                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                if ((configuracaoTMS?.PrazoCancelamentoTransferenciaPallets > 0) && (transferencia.Solicitacao.Data.AddDays(configuracaoTMS.PrazoCancelamentoTransferenciaPallets).Date < DateTime.Now.Date))
                    return new JsonpResult(false, true, $"Prazo de cancelamento de {configuracaoTMS.PrazoCancelamentoTransferenciaPallets} {(configuracaoTMS.PrazoCancelamentoTransferenciaPallets > 1 ? "dias" : "dia")} encerrado.");

                transferencia.Situacao = SituacaoTransferenciaPallet.EnvioCancelado;

                repositorio.Atualizar(transferencia);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, transferencia, $"Transferência cancelada", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao cancelar a transferência.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets.AprovacaoAlcadaTransferenciaPallet(unitOfWork);
                var autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (autorizacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    autorizacao.Codigo,
                    Regra = autorizacao.Descricao,
                    Situacao = autorizacao.Situacao.ObterDescricao(),
                    Usuario = autorizacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = autorizacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = autorizacao.Data.HasValue ? autorizacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", false);
                grid.AdicionarCabecalho("Data", false);
                grid.AdicionarCabecalho("Motivo", false);
                grid.AdicionarCabecalho("Usuário", "Usuario", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 5, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);

                var codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets.AprovacaoAlcadaTransferenciaPallet(unitOfWork);
                var listaAutorizacao = repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta);
                var totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigo);

                var lista = (
                    from autorizacao in listaAutorizacao
                    select new
                    {
                        autorizacao.Codigo,
                        PrioridadeAprovacao = autorizacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = autorizacao.Situacao.ObterDescricao(),
                        Usuario = autorizacao.Usuario?.Nome,
                        Regra = autorizacao.Descricao,
                        Data = autorizacao.Data.HasValue ? autorizacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(autorizacao.Motivo) ? string.Empty : autorizacao.Motivo,
                        DT_RowColor = autorizacao.ObterCorGrid()
                    }
                ).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprocessarRegras()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var codigo = Request.GetIntParam("Codigo");
                var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
                var transferencia = repositorio.BuscarPorCodigo(codigo);

                if (transferencia == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (transferencia.Situacao != SituacaoTransferenciaPallet.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                AtualizarAprovacao(unitOfWork, transferencia);

                repositorio.Atualizar(transferencia);

                unitOfWork.CommitChanges();

                return new JsonpResult(transferencia.Situacao != SituacaoTransferenciaPallet.SemRegraAprovacao);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao reprocessar as regras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void AtualizarAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            var servicoTransferencia = new Servicos.Embarcador.Pallets.Transferencia(unitOfWork);

            servicoTransferencia.EtapaAprovacao(transferencia, TipoServicoMultisoftware);
        }

        private void InserirMovimentacaoEstoqueRecebimento(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet movimentacaoEntrada = new Dominio.ObjetosDeValor.Embarcador.Pallets.DadosMovimentacaoEstoquePallet()
            {
                CodigoFilial = transferencia.Solicitacao.Filial.Codigo,
                CodigoSetor = transferencia.Solicitacao.Setor.Codigo,
                Quantidade = transferencia.Recebimento.Quantidade,
                TipoLancamento = TipoLancamento.Automatico,
                TipoOperacaoMovimentacao = TipoOperacaoMovimentacaoEstoquePallet.FilialEntrada,
            };

            new Servicos.Embarcador.Pallets.EstoquePallet(unitOfWork).InserirMovimentacao(movimentacaoEntrada);
        }

        private dynamic ObterEnvio(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            if (transferencia.Envio == null)
                return null;

            return new
            {
                transferencia.Envio.Codigo,
                transferencia.Envio.Quantidade,
                transferencia.Envio.Remetente,
                transferencia.Envio.Responsavel,
                Filial = new { transferencia.Envio.Filial.Codigo, transferencia.Envio.Filial.Descricao },
                Setor = new { transferencia.Envio.Setor.Codigo, transferencia.Envio.Setor.Descricao },
                Turno = new { transferencia.Envio.Turno.Codigo, transferencia.Envio.Turno.Descricao }
            };
        }

        private Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletEnvio ObterEnvioAdicionar(Repositorio.UnitOfWork unitOfWork)
        {
            var dadosEnvio = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Envio"));
            var quantidade = ((string)dadosEnvio.Quantidade).ToInt();
            var remetente = (string)dadosEnvio.Remetente;
            var responsavel = (string)dadosEnvio.Responsavel;

            if (quantidade <= 0)
                throw new Exception("Quantidade não informada");

            if (String.IsNullOrWhiteSpace(remetente))
                throw new Exception("Remetente não informado");

            if (String.IsNullOrWhiteSpace(responsavel))
                throw new Exception("Responsável não informado");

            return new Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletEnvio()
            {
                Filial = ObterFilial(unitOfWork, ((string)dadosEnvio.Filial).ToInt()),
                Quantidade = quantidade,
                Remetente = remetente,
                Responsavel = responsavel,
                Setor = ObterSetor(unitOfWork, ((string)dadosEnvio.Setor).ToInt()),
                Turno = ObterTurno(unitOfWork, ((string)dadosEnvio.Turno).ToInt())
            };
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Repositorio.UnitOfWork unitOfWork, int codigoFilial)
        {
            if (codigoFilial <= 0)
                throw new Exception("Filial não informada");

            var repositorio = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            var filial = repositorio.BuscarPorCodigo(codigoFilial);

            if (filial == null)
                throw new Exception("Filial não encontrada");

            return filial;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho(propriedade: "Codigo", visivel: false);
                grid.AdicionarCabecalho(descricao: "Número", propriedade: "Numero", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Data", propriedade: "Data", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Situacao", propriedade: "Situacao", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Setor", propriedade: "Setor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Turno", propriedade: "Turno", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Quantidade", propriedade: "Quantidade", tamanho: 15, alinhamento: Models.Grid.Align.center, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Solicitante", propriedade: "Solicitante", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPallet()
                {
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoSetor = Request.GetIntParam("Setor"),
                    CodigoTurno = Request.GetIntParam("Turno"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    Numero = Request.GetIntParam("Numero"),
                    Situacao = Request.GetEnumParam<SituacaoTransferenciaPallet>("Situacao")
                };

                var lista = Pesquisar(filtrosPesquisa, ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Data")
                return "Solicitacao.Data";

            if (grid.header[grid.indiceColunaOrdena].data == "Filial")
                return "Solicitacao.Filial.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Quantidade")
                return "Solicitacao.Quantidade";

            if (grid.header[grid.indiceColunaOrdena].data == "Setor")
                return "Solicitacao.Setor.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Solicitante")
                return "Solicitacao.Solicitante";

            if (grid.header[grid.indiceColunaOrdena].data == "Turno")
                return "Solicitacao.Turno.Descricao";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private dynamic ObterRecebimento(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            if (transferencia.Recebimento == null)
                return null;

            return new
            {
                transferencia.Recebimento.Codigo,
                transferencia.Recebimento.Quantidade,
                transferencia.Recebimento.Recebedor,
                Data = transferencia.Recebimento.Data.ToString("dd/MM/yyyy")
            };
        }

        private Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletRecebimento ObterRecebimentoAdicionar(Repositorio.UnitOfWork unitOfWork)
        {
            var dadosEnvio = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Recebimento"));
            var data = ((string)dadosEnvio.Data).ToNullableDateTime();
            var quantidade = ((string)dadosEnvio.Quantidade).ToInt();
            var recebedor = (string)dadosEnvio.Recebedor;

            if (!data.HasValue)
                throw new Exception("Data não informada");

            if (quantidade <= 0)
                throw new Exception("Quantidade não informada");

            if (String.IsNullOrWhiteSpace(recebedor))
                throw new Exception("Recebedor não informado");

            return new Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletRecebimento()
            {
                Data = data.Value,
                Quantidade = quantidade,
                Recebedor = recebedor
            };
        }

        private dynamic ObterResumo(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            return new
            {
                transferencia.Solicitacao.Quantidade,
                transferencia.Solicitacao.Solicitante,
                Data = transferencia.Solicitacao.Data.ToString("dd/MM/yyyy"),
                transferencia.Numero,
                Filial = transferencia.Solicitacao.Filial.Descricao,
                Setor = transferencia.Solicitacao.Setor.Descricao,
                Situacao = transferencia.Situacao.ObterDescricao(),
                Turno = transferencia.Solicitacao.Turno.Descricao
            };
        }

        private dynamic ObterResumoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasTransferenciaPallets.AprovacaoAlcadaTransferenciaPallet(unitOfWork);
            var aprovacoes = repositorioAprovacao.ContarAprovacoes(transferencia.Codigo);
            var aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(transferencia.Codigo);
            var reprovacoes = repositorioAprovacao.ContarReprovacoes(transferencia.Codigo);

            return new
            {
                transferencia.Solicitacao.Solicitante,
                DataSolicitacao = transferencia.Solicitacao.Data.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = transferencia.Situacao.ObterDescricao(),
            };
        }

        private Dominio.Entidades.Setor ObterSetor(Repositorio.UnitOfWork unitOfWork, int codigoSetor)
        {
            if (codigoSetor <= 0)
                throw new Exception("Setor não informado");

            var repositorio = new Repositorio.Setor(unitOfWork);
            var setor = repositorio.BuscarPorCodigo(codigoSetor);

            if (setor == null)
                throw new Exception("Setor não encontrado");

            return setor;
        }

        private dynamic ObterSolicitacao(Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPallet transferencia)
        {
            return new
            {
                transferencia.Solicitacao.Codigo,
                transferencia.Solicitacao.Data,
                transferencia.Solicitacao.Quantidade,
                transferencia.Solicitacao.Solicitante,
                transferencia.Numero,
                Filial = new { transferencia.Solicitacao.Filial.Codigo, transferencia.Solicitacao.Filial.Descricao },
                Setor = new { transferencia.Solicitacao.Setor.Codigo, transferencia.Solicitacao.Setor.Descricao },
                Turno = new { transferencia.Solicitacao.Turno.Codigo, transferencia.Solicitacao.Turno.Descricao }
            };
        }

        private Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletSolicitacao ObterSolicitacaoAdicionar(Repositorio.UnitOfWork unitOfWork)
        {
            var quantidade = Request.GetIntParam("Quantidade");
            var solicitante = Request.Params("Solicitante");

            if (quantidade <= 0)
                throw new Exception("Quantidade não informada");

            if (String.IsNullOrWhiteSpace(solicitante))
                throw new Exception("Solicitante não informado");

            return new Dominio.Entidades.Embarcador.Pallets.Transferencia.TransferenciaPalletSolicitacao()
            {
                Data = DateTime.Now,
                Filial = ObterFilial(unitOfWork, Request.GetIntParam("Filial")),
                Quantidade = quantidade,
                Setor = ObterSetor(unitOfWork, Request.GetIntParam("Setor")),
                Solicitante = solicitante.Trim(),
                Turno = ObterTurno(unitOfWork, Request.GetIntParam("Turno"))
            };
        }

        private Dominio.Entidades.Embarcador.Filiais.Turno ObterTurno(Repositorio.UnitOfWork unitOfWork, int codigoTurno)
        {
            if (codigoTurno <= 0)
                throw new Exception("Turno não informado");

            var repositorio = new Repositorio.Embarcador.Filiais.Turno(unitOfWork);
            var turno = repositorio.BuscarPorCodigo(codigoTurno);

            if (turno == null)
                throw new Exception("Turno não encontrado");

            return turno;
        }

        private dynamic Pesquisar(Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaTransferenciaPallet filtrosPesquisa, ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorio = new Repositorio.Embarcador.Pallets.TransferenciaPallet(unitOfWork);
            var listaTransferenciaPallet = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);

            return (from transferenciaPallet in listaTransferenciaPallet
                select new
                {
                    transferenciaPallet.Codigo,
                    transferenciaPallet.Solicitacao.Quantidade,
                    transferenciaPallet.Solicitacao.Solicitante,
                    Data = transferenciaPallet.Solicitacao.Data.ToString("dd/MM/yyyy"),
                    transferenciaPallet.Numero,
                    Filial = transferenciaPallet.Solicitacao.Filial.Descricao,
                    Setor = transferenciaPallet.Solicitacao.Setor.Descricao,
                    Situacao = transferenciaPallet.Situacao.ObterDescricao(),
                    Turno = transferenciaPallet.Solicitacao.Turno.Descricao
                }
            ).ToList();
        }

        #endregion
    }
}
