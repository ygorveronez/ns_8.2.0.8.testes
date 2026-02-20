using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ContratoPrestacaoServico")]
    public class ContratoPrestacaoServicoController : BaseController
    {
		#region Construtores

		public ContratoPrestacaoServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = new Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico()
                {
                    Situacao = SituacaoContratoPrestacaoServico.SemRegraAprovacao
                };

                PreencherContratoPrestacaoServico(contratoPrestacaoServico);

                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);

                unitOfWork.Start();

                AdicionarOuAtualizarTransportadores(contratoPrestacaoServico, unitOfWork);
                AdicionarOuAtualizarFiliais(contratoPrestacaoServico, unitOfWork);

                repositorio.Inserir(contratoPrestacaoServico, Auditado);

                AtualizarAprovacao(contratoPrestacaoServico, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = repositorio.BuscarPorCodigo(codigo, auditavel: true);

                if (contratoPrestacaoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (!contratoPrestacaoServico.Situacao.IsPermiteAtualizar())
                    return new JsonpResult(false, true, "A situação do contrato de prestação de serviço não permite atualizar.");

                PreencherContratoPrestacaoServico(contratoPrestacaoServico);

                unitOfWork.Start();

                AdicionarOuAtualizarTransportadores(contratoPrestacaoServico, unitOfWork);
                AdicionarOuAtualizarFiliais(contratoPrestacaoServico, unitOfWork);
                AtualizarAprovacao(contratoPrestacaoServico, unitOfWork);

                repositorio.Atualizar(contratoPrestacaoServico, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os dados.");
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
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (contratoPrestacaoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    DadosContratoPrestacaoServico = new
                    {
                        contratoPrestacaoServico.Codigo,
                        DataFinal = contratoPrestacaoServico.DataFinal.ToString("dd/MM/yyyy"),
                        DataInicial = contratoPrestacaoServico.DataInicial.ToString("dd/MM/yyyy"),
                        contratoPrestacaoServico.Descricao,
                        contratoPrestacaoServico.Situacao,
                        Status = contratoPrestacaoServico.Ativo,
                        contratoPrestacaoServico.ValorTeto
                    },
                    Filiais = (
                        from filial in contratoPrestacaoServico.Filiais
                        select new
                        {
                            filial.Codigo,
                            filial.Descricao
                        }
                    ).ToList(),
                    Transportadores = (
                        from transportador in contratoPrestacaoServico.Transportadores
                        select new
                        {
                            transportador.Codigo,
                            transportador.Descricao
                        }
                    ).ToList(),
                    ResumoAprovacao = ObterResumoAprovacao(contratoPrestacaoServico, unitOfWork)
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico autorizacao = repositorioAprovacao.BuscarPorCodigo(codigo);

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

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaPorAtivo()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaAtivo());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> PesquisaAutorizacoes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
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

                int codigo = Request.GetIntParam("Codigo");
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico> listaAutorizacao = repositorioAprovacao.ConsultarAutorizacoes(codigo, parametrosConsulta);
                int totalRegistros = repositorioAprovacao.ContarAutorizacoes(codigo);

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
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico = repositorio.BuscarPorCodigo(codigo, auditavel: false);

                if (contratoPrestacaoServico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (contratoPrestacaoServico.Situacao != SituacaoContratoPrestacaoServico.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                unitOfWork.Start();

                AtualizarAprovacao(contratoPrestacaoServico, unitOfWork);
                repositorio.Atualizar(contratoPrestacaoServico);

                unitOfWork.CommitChanges();

                return new JsonpResult(contratoPrestacaoServico.Situacao != SituacaoContratoPrestacaoServico.SemRegraAprovacao);
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

        private void AdicionarOuAtualizarFiliais(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic filiais = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Filiais"));

            ExcluirFiliaisRemovidas(contratoPrestacaoServico, filiais, unitOfWork);
            InserirFiliaisAdicionadas(contratoPrestacaoServico, filiais, unitOfWork);
        }

        private void AdicionarOuAtualizarTransportadores(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            dynamic transportadores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Transportadores"));

            ExcluirTransportadoresRemovidos(contratoPrestacaoServico, transportadores, unitOfWork);
            InserirTransportadoresAdicionados(contratoPrestacaoServico, transportadores, unitOfWork);
        }

        private void AtualizarAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Frete.ContratoPrestacaoServico servicoContratoPrestacaoServico = new Servicos.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);

            servicoContratoPrestacaoServico.CriarAprovacao(contratoPrestacaoServico, TipoServicoMultisoftware);
        }

        private void ExcluirFiliaisRemovidas(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, dynamic filiais, Repositorio.UnitOfWork unitOfWork)
        {
            if (contratoPrestacaoServico.Filiais?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var filial in filiais)
                    listaCodigosAtualizados.Add(((string)filial.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Filiais.Filial> listaFilialRemover = (from filial in contratoPrestacaoServico.Filiais where !listaCodigosAtualizados.Contains(filial.Codigo) select filial).ToList();

                foreach (Dominio.Entidades.Embarcador.Filiais.Filial filial in listaFilialRemover)
                    contratoPrestacaoServico.Filiais.Remove(filial);

                if (listaFilialRemover.Count > 0)
                {
                    string descricaoAcao = listaFilialRemover.Count == 1 ? "Filial removida" : "Múltiplas filiais removidas";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoPrestacaoServico, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void ExcluirTransportadoresRemovidos(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, dynamic transportadores, Repositorio.UnitOfWork unitOfWork)
        {
            if (contratoPrestacaoServico.Transportadores?.Count > 0)
            {
                List<int> listaCodigosAtualizados = new List<int>();

                foreach (var transportador in transportadores)
                    listaCodigosAtualizados.Add(((string)transportador.Codigo).ToInt());

                List<Dominio.Entidades.Empresa> listaTransportadorRemover = (from transportador in contratoPrestacaoServico.Transportadores where !listaCodigosAtualizados.Contains(transportador.Codigo) select transportador).ToList();

                foreach (Dominio.Entidades.Empresa transportador in listaTransportadorRemover)
                    contratoPrestacaoServico.Transportadores.Remove(transportador);

                if (listaTransportadorRemover.Count > 0)
                {
                    string descricaoAcao = listaTransportadorRemover.Count == 1 ? "Transportador removido" : "Múltiplos transportadores removidos";

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoPrestacaoServico, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
                }
            }
        }

        private void InserirFiliaisAdicionadas(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, dynamic filiais, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            int totalFiliaisAdicionadas = 0;

            if (contratoPrestacaoServico.Filiais == null)
                contratoPrestacaoServico.Filiais = new List<Dominio.Entidades.Embarcador.Filiais.Filial>();

            foreach (var filial in filiais)
            {
                int codigo = ((string)filial.Codigo).ToInt();
                Dominio.Entidades.Embarcador.Filiais.Filial filialAdicionar = repositorioFilial.BuscarPorCodigo(codigo) ?? throw new ControllerException("Filial não encontrada");

                if (!contratoPrestacaoServico.Filiais.Contains(filialAdicionar))
                {
                    contratoPrestacaoServico.Filiais.Add(filialAdicionar);

                    totalFiliaisAdicionadas++;
                }
            }

            if (contratoPrestacaoServico.IsInitialized() && (totalFiliaisAdicionadas > 0))
            {
                string descricaoAcao = totalFiliaisAdicionadas == 1 ? "Filial adicionada" : "Múltiplas filiais adicionadas";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoPrestacaoServico, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private void InserirTransportadoresAdicionados(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, dynamic transportadores, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            int totalTransportadoresAdicionados = 0;

            if (contratoPrestacaoServico.Transportadores == null)
                contratoPrestacaoServico.Transportadores = new List<Dominio.Entidades.Empresa>();

            foreach (var transportador in transportadores)
            {
                int codigo = ((string)transportador.Codigo).ToInt();
                Dominio.Entidades.Empresa transportadorAdicionar = repositorioEmpresa.BuscarPorCodigo(codigo) ?? throw new ControllerException("Transportador não encontrado");

                if (!contratoPrestacaoServico.Transportadores.Contains(transportadorAdicionar))
                {
                    contratoPrestacaoServico.Transportadores.Add(transportadorAdicionar);

                    totalTransportadoresAdicionados++;
                }
            }

            if (contratoPrestacaoServico.IsInitialized() && (totalTransportadoresAdicionados > 0))
            {
                string descricaoAcao = totalTransportadoresAdicionados == 1 ? "Transportador adicionado" : "Múltiplos transportadores adicionados";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, contratoPrestacaoServico, null, descricaoAcao, unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Registro);
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetNullableEnumParam<SituacaoContratoPrestacaoServico>("Situacao"),
                SituacaoAtivo = Request.GetEnumParam("Status", SituacaoAtivoPesquisa.Ativo)
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Teto", "ValorTeto", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação da Aprovação", "Situacao", 15, Models.Grid.Align.center, true);

                if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 15, Models.Grid.Align.center, false);
                
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta  = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                int totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> listaContratoPrestacaoServico = totalRegistros > 0 ? repositorio.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();

                var listaContratoPrestacaoServicoRetornar = (
                    from contratoPrestacaoServico in listaContratoPrestacaoServico
                    select new
                    {
                        contratoPrestacaoServico.Codigo,
                        contratoPrestacaoServico.Descricao,
                        contratoPrestacaoServico.DescricaoAtivo,
                        DataFinal = contratoPrestacaoServico.DataFinal.ToString("dd/MM/yyyy"),
                        DataInicial = contratoPrestacaoServico.DataInicial.ToString("dd/MM/yyyy"),
                        ValorTeto = contratoPrestacaoServico.ValorTeto.ToString("N2"),
                        Situacao = contratoPrestacaoServico.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaContratoPrestacaoServicoRetornar);
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

        private Models.Grid.Grid ObterGridPesquisaAtivo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 25, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Inicial", "DataInicial", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Data Final", "DataFinal", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Valor Teto", "ValorTeto", 10, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Frete.ContratoPrestacaoServico repositorio = new Repositorio.Embarcador.Frete.ContratoPrestacaoServico(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaAtivo(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> listaContratoPrestacaoServico = totalRegistros > 0 ? repositorio.ConsultarAtivo(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();

                var listaContratoPrestacaoServicoRetornar = (
                    from contratoPrestacaoServico in listaContratoPrestacaoServico
                    select new
                    {
                        contratoPrestacaoServico.Codigo,
                        contratoPrestacaoServico.Descricao,
                        DataFinal = contratoPrestacaoServico.DataFinal.ToString("dd/MM/yyyy"),
                        DataInicial = contratoPrestacaoServico.DataInicial.ToString("dd/MM/yyyy"),
                        ValorTeto = contratoPrestacaoServico.ValorTeto.ToString("N2")
                    }
                ).ToList();

                grid.AdicionaRows(listaContratoPrestacaoServicoRetornar);
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

        private dynamic ObterResumoAprovacao(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico repositorioAprovacao = new Repositorio.Embarcador.Frete.AprovacaoAlcadaContratoPrestacaoServico(unitOfWork);
            int aprovacoes = repositorioAprovacao.ContarAprovacoes(contratoPrestacaoServico.Codigo);
            int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(contratoPrestacaoServico.Codigo);
            int reprovacoes = repositorioAprovacao.ContarReprovacoes(contratoPrestacaoServico.Codigo);

            return new
            {
                Codigo = contratoPrestacaoServico.Codigo,
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                PossuiRegras = (contratoPrestacaoServico.Situacao != SituacaoContratoPrestacaoServico.SemRegraAprovacao),
                Reprovacoes = reprovacoes
            };
        }

        private void PreencherContratoPrestacaoServico(Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico contratoPrestacaoServico)
        {
            contratoPrestacaoServico.Ativo = Request.GetBoolParam("Status");
            contratoPrestacaoServico.DataFinal = Request.GetDateTimeParam("DataFinal");
            contratoPrestacaoServico.DataInicial = Request.GetDateTimeParam("DataInicial");
            contratoPrestacaoServico.Descricao = Request.GetStringParam("Descricao");
            contratoPrestacaoServico.ValorTeto = Request.GetDecimalParam("ValorTeto");
        }

        #endregion
    }
}
