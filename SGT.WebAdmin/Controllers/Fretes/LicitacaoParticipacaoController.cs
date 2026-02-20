using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/LicitacaoParticipacao")]
    public class LicitacaoParticipacaoController : BaseController
    {
		#region Construtores

		public LicitacaoParticipacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");
                Repositorio.Embarcador.Frete.TabelaFrete repositorioTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete = repositorioTabelaFrete.BuscarPorCodigo(codigoTabelaFrete);

                if (tabelaFrete == null)
                    return new JsonpResult(false, "Tabela de frete não encontrada.");

                Models.Grid.Grid grid = ObterGridOferta(tabelaFrete);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = new Dominio.Entidades.Embarcador.Relatorios.Relatorio() { Colunas = new List<Dominio.Entidades.Embarcador.Relatorios.RelatorioColuna>() };
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = new Models.Grid.Relatorio().RetornoGridPadraoRelatorio(grid, relatorio);

                return new JsonpResult(retorno);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorLicitacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    return new JsonpResult(false, true, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigoLicitacao = Request.GetIntParam("CodigoLicitacao");
                Repositorio.Embarcador.Frete.Licitacao repositorioLicitacao = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = repositorioLicitacao.BuscarPorCodigo(codigoLicitacao, auditavel: false);

                if (licitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a licitação.");

                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorLicitacaoETransportador(licitacao.Codigo, this.Empresa.Codigo);

                return new JsonpResult(new
                {
                    Inscricao = ObterInscricao(licitacaoParticipacao),
                    Oferta = ObterOferta(licitacaoParticipacao),
                    Resumo = ObterResumo(licitacao, licitacaoParticipacao),
                    RetornoOferta = ObterRetornoOferta(licitacaoParticipacao),
                    Anexos = ObterAnexos(licitacao)
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar os dados de participação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    return new JsonpResult(false, true, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar o registro de participação.");

                unitOfWork.Start();

                licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.Cancelada;

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                //string propOrdena = "";

                //Models.Grid.Grid grid = ObterGridOferta(licitacaoParticipacao.Licitacao.TabelaFrete);
                //List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);
                //Servicos.Embarcador.Frete.Licitacao.ReordenarRankingLicitacao(licitacaoParticipacao.Licitacao, agrupamentos, unitOfWork);
                
                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Participação da licitação cancelada", unitOfWork);

                unitOfWork.CommitChanges();

                try
                {
                    EnviarNotificacaoUsuarioLicitacao(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacao.UsuarioDeixouLicitacao, licitacaoParticipacao.Usuario.Nome, licitacaoParticipacao.Licitacao.Numero));
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dadosda participação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> EnviarOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    return new JsonpResult(false, true, "Não é possível realizar esta operação para este tipo de empresa.");
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                int codigo = Request.GetIntParam("CodigoLicitacaoParticipacao");
                int codigoTabelaFreteCliente = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar o registro de participação.");

                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoTabelaFreteCliente);

                if (tabelaFreteCliente == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar o registro da tabela frete.");

                unitOfWork.Start();

                licitacaoParticipacao.DataEnvioOferta = DateTime.Now;
                licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.AguardandoRetornoOferta;
                licitacaoParticipacao.TabelaFreteCliente = tabelaFreteCliente;

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                //string propOrdena = "";

                //Models.Grid.Grid grid = ObterGridOferta(licitacaoParticipacao.Licitacao.TabelaFrete);
                //List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, ref propOrdena, string.Empty);

                //Servicos.Embarcador.Frete.Licitacao.ReordenarRankingLicitacao(licitacaoParticipacao.Licitacao, agrupamentos, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores parametrosValores = PreencherParametrosValores();
                parametrosValores.TabelaFreteCliente = tabelaFreteCliente;
                servicoTabelaFreteCliente.SalvarValores(parametrosValores, Auditado);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Oferta para a licitação enviada", unitOfWork);

                unitOfWork.CommitChanges();

                try
                {
                    EnviarNotificacaoUsuarioLicitacao(unitOfWork, licitacaoParticipacao, string.Format(Localization.Resources.Fretes.LicitacaoParticipacao.UsuarioEnviouOfertaLicitacao, licitacaoParticipacao.Usuario.Nome, licitacaoParticipacao.Licitacao.Numero));
                }
                catch (Exception excecao)
                {
                    Servicos.Log.TratarErro(excecao);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dadosda participação.");
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

        public async Task<IActionResult> Participar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    return new JsonpResult(false, true, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigoLicitacao = Request.GetIntParam("CodigoLicitacao");
                Repositorio.Embarcador.Frete.Licitacao repositorioLicitacao = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.Licitacao licitacao = repositorioLicitacao.BuscarPorCodigo(codigoLicitacao, auditavel: false);

                if (licitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar a licitação.");

                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacaoExistente = repositorioLicitacaoParticipacao.BuscarPorLicitacaoETransportador(licitacao.Codigo, this.Empresa.Codigo);

                if (licitacaoParticipacaoExistente != null)
                    return new JsonpResult(false, true, $"Você já está participando desta licitação com o número de proposta {licitacaoParticipacaoExistente.Numero}.");

                unitOfWork.Start();

                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = new Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao()
                {
                    Licitacao = licitacao,
                    Numero = repositorioLicitacaoParticipacao.BuscarProximoNumero(),
                    Observacao = Request.GetNullableStringParam("Observacao"),
                    Situacao = SituacaoLicitacaoParticipacao.AguardandoOferta,
                    Transportador = this.Empresa,
                    Usuario = this.Usuario
                };

                repositorioLicitacaoParticipacao.Inserir(licitacaoParticipacao);

                GerarOferta(unitOfWork, licitacaoParticipacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Adicionada participação", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados da participação.");
            }
            finally
            {
                unitOfWork.Dispose();
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

        public async Task<IActionResult> PesquisaOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLicitacaoParticipacao = Request.GetIntParam("Codigo");
                int codigoTabelaFrete = Request.GetIntParam("TabelaFrete");

                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                List<(int Codigo, string Descricao)> listaRetorno = repositorioTabelaFreteCliente.BuscarOpcoesPorLicitacaoParticipacao(codigoLicitacaoParticipacao, codigoTabelaFrete);

                return new JsonpResult(new
                {
                    ListaOferta = (from o in listaRetorno
                                   select new
                                   {
                                       o.Codigo,
                                       o.Descricao
                                   }).ToList()
                });
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

        public async Task<IActionResult> RefazerOferta()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                //if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                //    return new JsonpResult(false, true, "Não é possível realizar esta operação para este tipo de empresa.");

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.LicitacaoParticipacao repositorioLicitacaoParticipacao = new Repositorio.Embarcador.Frete.LicitacaoParticipacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao = repositorioLicitacaoParticipacao.BuscarPorCodigo(codigo, auditavel: false);

                if (licitacaoParticipacao == null)
                    return new JsonpResult(false, true, $"Não foi possível encontrar o registro de participação.");

                unitOfWork.Start();

                licitacaoParticipacao.DataEnvioOferta = null;
                licitacaoParticipacao.ObservacaoRetorno = string.Empty;
                licitacaoParticipacao.Situacao = SituacaoLicitacaoParticipacao.AguardandoOferta;

                repositorioLicitacaoParticipacao.Atualizar(licitacaoParticipacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, licitacaoParticipacao, null, "Solicitado realizar oferta para licitação novamente", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dadosda participação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValorFixo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoParametro = Request.GetIntParam("CodigoItem");
                Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
                Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete repositorioParametro = new Repositorio.Embarcador.Frete.ParametroBaseCalculoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ParametroBaseCalculoTabelaFrete parametro = repositorioParametro.BuscarPorCodigo(codigoParametro);
                Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFrete = repositorioTabelaFreteCliente.BuscarPorCodigo(codigoParametro);

                if (parametro == null && tabelaFrete == null)
                    return new JsonpResult(false, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                if (Request.GetStringParam("Info") == "ValorBase")
                {
                    parametro.ValorBase = Request.GetDecimalParam("ValorItem");

                    repositorioParametro.Atualizar(parametro);
                }

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o valor do item.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarValorItem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoItem = Request.GetIntParam("CodigoItem");
                Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete repositorioItem = new Repositorio.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ItemParametroBaseCalculoTabelaFrete item = repositorioItem.BuscarPorCodigo(codigoItem);

                if (item == null)
                    return new JsonpResult(false, "O item não foi encontrado para edição. Atualize a página e tente novamente.");

                item.Valor = Request.GetDecimalParam("ValorItem");

                repositorioItem.Atualizar(item);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o valor do item.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void EnviarNotificacaoUsuarioLicitacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao, string mensagem)
        {
            var servicoNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente: Cliente, tipoServicoMultisoftware: TipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacao(
                usuario: licitacaoParticipacao.Licitacao.Usuario,
                usuarioGerouNotificacao: licitacaoParticipacao.Usuario,
                codigoObjeto: licitacaoParticipacao.Codigo,
                URLPagina: "Fretes/LicitacaoParticipacaoAvaliacao",
                nota: mensagem,
                icone: IconesNotificacao.sucesso,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: TipoServicoMultisoftware,
                unitOfWork: unitOfWork
            );
        }

        private void GerarOferta(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            Repositorio.Embarcador.Frete.TabelaFreteCliente repositorioTabelaFreteCliente = new Repositorio.Embarcador.Frete.TabelaFreteCliente(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente> tabelasFreteCliente = repositorioTabelaFreteCliente.BuscarTodasPorTabela(licitacaoParticipacao.Licitacao.TabelaFrete.Codigo);
            Servicos.Embarcador.Frete.TabelaFreteCliente servicoTabelaFreteCliente = new Servicos.Embarcador.Frete.TabelaFreteCliente(unitOfWork);

            if (licitacaoParticipacao.Licitacao.TabelasFreteCliente.Count > 0)
            {
                tabelasFreteCliente.Clear();
                tabelasFreteCliente.AddRange(licitacaoParticipacao.Licitacao.TabelasFreteCliente);
            }

            foreach (Dominio.Entidades.Embarcador.Frete.TabelaFreteCliente tabelaFreteCliente in tabelasFreteCliente)
            {
                if (tabelaFreteCliente.LicitacaoParticipacao == null)
                    servicoTabelaFreteCliente.DuplicarParaLicitacaoParticipacao(tabelaFreteCliente, licitacaoParticipacao);
            }
        }

        private dynamic ObterAnexos(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao)
        {
            return (
                from anexo in licitacao.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo,
                }
            ).ToList();
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao()
            {
                CodigoTabelaFrete = Request.GetIntParam("TabelaFrete"),
                CodigoTransportador = this.Empresa?.Codigo ?? 0,
                Descricao = Request.GetNullableStringParam("Descricao"),
                Numero = Request.GetIntParam("Numero")
            };
        }

        private Models.Grid.Grid ObterGridOferta(Dominio.Entidades.Embarcador.Frete.TabelaFrete tabelaFrete)
        {
            int NumeroMaximoComplementos = 15;
            int UltimaColunaDinamica = 1;
            decimal TamanhoColunasValores = 1.75m;
            decimal TamanhoColunasParticipantes = 5.50m;
            decimal TamanhoColunasEnderecoParticipantes = 3m;

            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            bool visibleTMS = true;

            grid.AdicionarCabecalho("Código", "CodigoIntegracao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tabela de Frete", "TabelaFrete", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, false, false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
            {
                visibleTMS = false;
                grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Rota Frete Origem", "RotaFreteOrigem", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
                grid.AdicionarCabecalho("Rota Frete Destino", "RotaFrete", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            }

            grid.AdicionarCabecalho("Remetente", "DescricaoRemetente", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, visibleTMS);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Estado Origem", "EstadoOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Região Origem", "RegiaoOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Origem", "CEPOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Origem", "PaisOrigem", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Destinatário", "DescricaoDestinatario", TamanhoColunasParticipantes, Models.Grid.Align.left, false, false, false, true, visibleTMS);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Região Destino", "RegiaoDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Estado Destino", "EstadoDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CEP Destino", "CEPDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("País Destino", "PaisDestino", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, false, false, false, true, false);

            grid.AdicionarCabecalho("Tomador", "Tomador", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            if (tabelaFrete.ParametroBase.HasValue)
                grid.AdicionarCabecalho("Base (" + tabelaFrete.ParametroBase.Value.ObterDescricao() + ")", "ParametroBase", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);

            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Inicio Vigência", "DataInicial", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Fim Vigência", "DataFinal", TamanhoColunasEnderecoParticipantes, Models.Grid.Align.left, true, false, false, false, false);

            grid.AdicionarCabecalho("Tipo Pagamento", "DescricaoTipoPagamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);

            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, true, false);

            if (tabelaFrete.NumeroEntregas.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.NumeroEntrega))
            {
                grid.AdicionarCabecalho("Entrega", "NumeroEntrega", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Entrega", "DescricaoValorEntrega", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TipoEmbalagens.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoEmbalagem))
            {
                grid.AdicionarCabecalho("Tipo de Embalagem", "TipoEmbalagem", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo de Embalagem", "DescricaoValorTipoEmbalagem", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.PesosTransportados.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Peso))
            {
                grid.AdicionarCabecalho("Peso", "DescricaoPeso", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Peso", "DescricaoValorPeso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Distancias.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Distancia))
            {
                grid.AdicionarCabecalho("Distância", "DescricaoDistancia", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Distância", "DescricaoValorDistancia", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.TiposCarga.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.TipoCarga))
            {
                grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tipo Carga", "DescricaoValorTipoCarga", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosReboque.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloReboque))
            {
                grid.AdicionarCabecalho("Reboque", "ModeloReboque", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Reboque", "DescricaoValorModeloReboque", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.ModelosTracao.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.ModeloTracao))
            {
                grid.AdicionarCabecalho("Tração", "ModeloTracao", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tração", "DescricaoValorModeloTracao", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Ajudantes.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Ajudante))
            {
                grid.AdicionarCabecalho("Ajudante", "NumeroAjudante", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Ajudante", "DescricaoValorAjudante", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Pallets.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Pallets))
            {
                grid.AdicionarCabecalho("Pallet", "NumeroPallets", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Pallet", "DescricaoValorPallets", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            if (tabelaFrete.Tempos.Count > 0 && (!tabelaFrete.ParametroBase.HasValue || tabelaFrete.ParametroBase.Value != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoParametroBaseTabelaFrete.Tempo))
            {
                grid.AdicionarCabecalho("Tempo", "HoraTempo", TamanhoColunasParticipantes, Models.Grid.Align.left, true, false, false, false, true);
                grid.AdicionarCabecalho("Valor Tempo", "DescricaoValorTempo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            }

            for (int i = 0; i < tabelaFrete.Componentes.Count; i++)
            {
                if (i < NumeroMaximoComplementos)
                {
                    grid.AdicionarCabecalho(tabelaFrete.Componentes[i].ComponenteFrete.Descricao, "DescricaoValorComponente" + UltimaColunaDinamica.ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.nenhum, tabelaFrete.Componentes[i].Codigo);
                    UltimaColunaDinamica++;
                }
                else
                    break;
            }

            grid.AdicionarCabecalho("Valor Total", "ValorTotal", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, true);

            if (tabelaFrete.PossuiMinimoGarantido)
                grid.AdicionarCabecalho("Valor Mínimo", "ValorMinimo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorMaximo)
                grid.AdicionarCabecalho("Valor Máximo", "ValorMaximo", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            if (tabelaFrete.PossuiValorBase)
                grid.AdicionarCabecalho("Valor Base", "DescricaoValorBase", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Início", "DataInicio", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Fim", "DataFim", 10, Models.Grid.Align.center, true);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaLicitacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.Licitacao repositorio = new Repositorio.Embarcador.Frete.Licitacao(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.Licitacao> listaLicitacao = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.Licitacao>();

                var listaLicitacaoRetornar = (
                    from licitacao in listaLicitacao
                    select new
                    {
                        licitacao.Codigo,
                        licitacao.Descricao,
                        DataFim = licitacao.DataFim.ToString("dd/MM/yyyy"),
                        DataInicio = licitacao.DataInicio.ToString("dd/MM/yyyy"),
                        licitacao.Numero
                    }
                ).ToList();

                grid.AdicionaRows(listaLicitacaoRetornar);
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

        private dynamic ObterInscricao(Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            if (licitacaoParticipacao == null)
                return null;

            return new
            {
                licitacaoParticipacao.Codigo,
                licitacaoParticipacao.Numero,
                licitacaoParticipacao.Observacao,
                licitacaoParticipacao.Situacao
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores PreencherParametrosValores()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.ParametrosValores()
            {
                Valores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Valores")),
                Observacoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Observacoes")),
                ValoresMinimosGarantidos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresMinimosGarantidos")),
                ValoresMaximos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresMaximos")),
                ValoresBases = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresBases")),
                ValoresExcedentes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValoresExcedentes")),
                PercentuaisPagamentoAgregados = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("PercentuaisPagamentoAgregados"))
            };
        }

        private dynamic ObterOferta(Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            if (licitacaoParticipacao == null)
                return null;

            return new
            {
                licitacaoParticipacao.Codigo,
                TabelaFrete = licitacaoParticipacao.Licitacao.TabelaFrete.Codigo,
                OpcaoOferta = licitacaoParticipacao.TabelaFreteCliente?.Codigo ?? 0
            };
        }

        private dynamic ObterResumo(Dominio.Entidades.Embarcador.Frete.Licitacao licitacao, Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            return new
            {
                licitacao.Numero,
                licitacao.Descricao,
                Validade = $"{licitacao.DataInicio.ToString("dd/MM/yyyy")} até {licitacao.DataFim.ToString("dd/MM/yyyy")}",
                Situacao = licitacaoParticipacao?.Situacao.ObterDescricao() ?? ""
            };
        }

        private dynamic ObterRetornoOferta(Dominio.Entidades.Embarcador.Frete.LicitacaoParticipacao licitacaoParticipacao)
        {
            if (licitacaoParticipacao == null)
                return null;

            return new
            {
                ClasseCor = licitacaoParticipacao.Situacao.ObterClasseCor(),
                Mensagem = licitacaoParticipacao.Situacao.ObterDescricao(),
                Observacao = licitacaoParticipacao.ObservacaoRetorno
            };
        }

        #endregion
    }
}
