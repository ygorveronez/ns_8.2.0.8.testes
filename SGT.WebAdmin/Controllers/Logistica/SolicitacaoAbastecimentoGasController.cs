using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Logistica
{
    [CustomAuthorize("Logistica/AbastecimentoGas")]
    public class SolicitacaoAbastecimentoGasController : BaseController
    {
		#region Construtores

		public SolicitacaoAbastecimentoGasController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data/Hora Criação", "DataHoraCriacao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data/Hora Aprovação/Recusa", "DataHoraAprovacao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nome usúario Aprovação/Recusa", "NomeUsuarioAprovacao", 35, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 35, Models.Grid.Align.left, true);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoAbastecimentoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                int totalRegistros = repositorioSolicitacaoAbastecimentoGas.ContarConsulta(filtrosPesquisa, parametrosConsulta);
                List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas> abastecimentos = totalRegistros > 0 ? repositorioSolicitacaoAbastecimentoGas.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas>();

                var abastecimentosRetornar = (
                    from abastecimento in abastecimentos
                    select new
                    {
                        abastecimento.Codigo,
                        abastecimento.Descricao,
                        DataHoraCriacao = abastecimento.DataCriacao,
                        DataHoraAprovacao = abastecimento.Situacao == SituacaoAprovacaoSolicitacaoGas.Aprovada || abastecimento.Situacao == SituacaoAprovacaoSolicitacaoGas.Reprovada ? abastecimento.DataUltimaAlteracao.ToString("dd/MM/yyyy HH:mm") : ""  ,
                        NomeUsuarioAprovacao = abastecimento.Situacao == SituacaoAprovacaoSolicitacaoGas.Aprovada || abastecimento.Situacao == SituacaoAprovacaoSolicitacaoGas.Reprovada ? abastecimento.Usuario.Nome: "",
                        Situacao = abastecimento.Situacao.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(abastecimentosRetornar);
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

        public async Task<IActionResult> BuscarResumoAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacao = repositorioSolicitacaoGas.BuscarPorCodigo(codigo);

                if (solicitacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                if (solicitacao.Situacao == SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao)
                    return new JsonpResult(new
                    {
                        solicitacao.Codigo,
                        DescricaoSituacao = solicitacao.Situacao.ObterDescricao(),
                        PossuiAlcada = true,
                        PossuiRegras = false
                    });

                Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork);
                int aprovacoes = repositorioAprovacao.ContarAprovacoes(solicitacao.Codigo);
                int aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(solicitacao.Codigo);
                int reprovacoes = repositorioAprovacao.ContarReprovacoes(solicitacao.Codigo);

                return new JsonpResult(new
                {
                    solicitacao.Codigo,
                    AprovacoesNecessarias = aprovacoesNecessarias,
                    Aprovacoes = aprovacoes,
                    Reprovacoes = reprovacoes,
                    DescricaoSituacao = solicitacao.Situacao.ObterDescricao(),
                    PossuiAlcada = true,
                    PossuiRegras = solicitacao.Situacao != SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao
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

        public async Task<IActionResult> DetalhesAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas aprovacao = repositorioAprovacao.BuscarPorCodigo(codigo);

                if (aprovacao == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    aprovacao.Codigo,
                    Regra = aprovacao.Descricao,
                    Situacao = aprovacao.Situacao.ObterDescricao(),
                    Usuario = aprovacao.Usuario?.Nome ?? string.Empty,
                    PodeAprovar = aprovacao.IsPermitirAprovacaoOuReprovacao(this.Usuario.Codigo),
                    Data = aprovacao.Data.HasValue ? aprovacao.Data.Value.ToString("dd/MM/yyyy") : string.Empty,
                    Motivo = string.IsNullOrWhiteSpace(aprovacao.Motivo) ? string.Empty : aprovacao.Motivo,
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

        public async Task<IActionResult> PesquisaAprovacoes()
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
                grid.AdicionarCabecalho("Usuário", "Usuario", 40, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", 20, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 20, Models.Grid.Align.center, false);

                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacao = repositorioSolicitacaoGas.BuscarPorCodigo(codigo);
                int totalRegistros = 0;

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas repositorioAprovacao = new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork);
                totalRegistros = solicitacao != null ?
                    repositorioAprovacao.ContarAutorizacoes(solicitacao.Codigo) :
                    0;

                List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas> listaAprovacao = totalRegistros > 0 ?
                    new Repositorio.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas(unitOfWork).BuscarPorOrigem(solicitacao.Codigo) :
                    new List<Dominio.Entidades.Embarcador.Logistica.AlcadasSolicitacaoGas.AprovacaoAlcadaSolicitacaoGas>();

                var lista = (
                    from aprovacao in listaAprovacao
                    select new
                    {
                        aprovacao.Codigo,
                        PrioridadeAprovacao = aprovacao.RegraAutorizacao?.PrioridadeAprovacao ?? 0,
                        Situacao = aprovacao.Situacao.ObterDescricao(),
                        Usuario = aprovacao.Usuario?.Nome,
                        Regra = aprovacao.Descricao,
                        Data = aprovacao.Data.HasValue ? aprovacao.Data.ToString() : string.Empty,
                        Motivo = string.IsNullOrWhiteSpace(aprovacao.Motivo) ? string.Empty : aprovacao.Motivo,
                        DT_RowColor = aprovacao.ObterCorGrid()
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
                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorio = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacao = repositorio.BuscarPorCodigo(codigo);

                if (solicitacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (solicitacao.Situacao != SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                unitOfWork.Start();

                new Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork).CriarAprovacao(solicitacao, TipoServicoMultisoftware);
                repositorio.Atualizar(solicitacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(solicitacao.Situacao != SituacaoAprovacaoSolicitacaoGas.SemRegraAprovacao);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                double codigoBase = Request.GetDoubleParam("Base");
                int codigoProduto = Request.GetIntParam("Produto");
                DateTime data = DateTime.Now.Date.AddDays(Request.GetIntParam("DataMedicao"));

                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacao = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProduto = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacao = repositorioSolicitacao.BuscarPorDataMedicaoFilialBaseProduto(data, codigoBase, codigoProduto);
                Dominio.Entidades.Cliente clienteBase = repositorioCliente.BuscarPorCPFCNPJ(codigoBase);
                Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produto = repositorioProduto.BuscarPorCodigo(codigoProduto);

                if (solicitacao != null)
                    throw new ControllerException($"Já existe uma solicitação de {produto.Descricao} para o dia {data.ToString("dd/MM/yyyy")} para a base {clienteBase.Descricao}.");

                solicitacao = new Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas();
                
                PreencherEntidade(solicitacao, unitOfWork);

                repositorioSolicitacao.Inserir(solicitacao, Auditado);

                Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas servicoSolicitacaoAbastecimentoGas = new Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                servicoSolicitacaoAbastecimentoGas.CriarAprovacao(solicitacao, TipoServicoMultisoftware);

                repositorioSolicitacao.Atualizar(solicitacao);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacao, "Adicionou solicitação de gás", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new
                {
                    solicitacao.Situacao
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/AbastecimentoGas");

                int codigo = Request.GetIntParam("Codigo");
                bool duplicar = Request.GetBoolParam("Duplicar");

                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacao = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);

                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas = repositorioSolicitacao.BuscarPorCodigo(codigo);

                if (solicitacaoAbastecimentoGas == null)
                    return new JsonpResult(false, "Registro não encontrado.");

                bool dataIgualOuPosterior = solicitacaoAbastecimentoGas.DataMedicao.Date >= DateTime.Now.Date;

                dynamic objetoRetorno = new System.Dynamic.ExpandoObject();

                objetoRetorno.Codigo = duplicar ? 0 : solicitacaoAbastecimentoGas.Codigo;
                objetoRetorno.Abertura = solicitacaoAbastecimentoGas.Abertura.ToString("n2");
                objetoRetorno.DataMedicaoEntidade = solicitacaoAbastecimentoGas.DataMedicao.ToString("dd/MM/yyyy");
                objetoRetorno.Base = new { Codigo = solicitacaoAbastecimentoGas.ClienteBase?.CPF_CNPJ ?? 0, Descricao = solicitacaoAbastecimentoGas.ClienteBase?.Descricao ?? "" };
                objetoRetorno.Justificativa = new { CodigoJustificativa = solicitacaoAbastecimentoGas.Justificativa?.Codigo ?? 0, Justificativa = solicitacaoAbastecimentoGas.Justificativa?.Justificativa ?? "" };
                objetoRetorno.PrevisaoBombeio = solicitacaoAbastecimentoGas.PrevisaoBombeio.ToString("n2");
                objetoRetorno.PrevisaoTransferenciaRecebida = solicitacaoAbastecimentoGas.PrevisaoTransferenciaRecebida.ToString("n2");
                objetoRetorno.PrevisaoDemandaDomiciliar = solicitacaoAbastecimentoGas.PrevisaoDemandaDomiciliar.ToString("n2");
                objetoRetorno.PrevisaoDemandaEmpresarial = solicitacaoAbastecimentoGas.PrevisaoDemandaEmpresarial.ToString("n2");
                objetoRetorno.EstoqueUltrasystem = solicitacaoAbastecimentoGas.EstoqueUltrasystem.ToString("n2");
                objetoRetorno.PrevisaoTransferenciaEnviada = solicitacaoAbastecimentoGas.PrevisaoTransferenciaEnviada.ToString("n2");
                objetoRetorno.DensidadeAberturaDia = solicitacaoAbastecimentoGas.DensidadeAberturaDia.ToString("n2");
                objetoRetorno.PrevisaoFechamento = solicitacaoAbastecimentoGas.PrevisaoFechamento.ToString("n2").Replace(".", "").Replace(",", ".");
                objetoRetorno.VolumeRodoviarioCarregamentoProximoDia = solicitacaoAbastecimentoGas.VolumeRodoviarioCarregamentoProximoDiaOriginal.ToString("n2");
                objetoRetorno.PrevisaoBombeioProximoDia = solicitacaoAbastecimentoGas.PrevisaoBombeioProximoDia.ToString("n2");
                objetoRetorno.DisponibilidadeTransferenciaProximoDia = solicitacaoAbastecimentoGas.DisponibilidadeTransferenciaProximoDia.ToString("n2");
                objetoRetorno.Produto = new { Codigo = solicitacaoAbastecimentoGas.Produto?.Codigo ?? 0, Descricao = solicitacaoAbastecimentoGas.Produto?.Descricao ?? "" };
                objetoRetorno.PermiteAdicionarVolumeExtra = permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas) && !duplicar && dataIgualOuPosterior;
                objetoRetorno.AdicionalVolumeRodoviarioCarregamentoProximoDia = solicitacaoAbastecimentoGas.AdicionalVolumeRodoviarioCarregamentoProximoDia.ToString("n2");
                objetoRetorno.AdicionalDisponibilidadeTransferenciaProximoDia = solicitacaoAbastecimentoGas.AdicionalDisponibilidadeTransferenciaProximoDia.ToString("n2");

                if (duplicar && dataIgualOuPosterior)
                    objetoRetorno.DataMedicao = solicitacaoAbastecimentoGas.DataMedicao.ToString("dd/MM/yyyy");

                return new JsonpResult(objetoRetorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarAbastecimentoGasDuplicado()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = Request.GetIntParam("AbastecimentoDuplicado");

                if (codigo == 0)
                    return new JsonpResult(true);

                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacao = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas = repositorioSolicitacao.BuscarPorCodigo(codigo);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoDuplicada = new Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas();

                PreencherEntidade(solicitacaoDuplicada, unitOfWork);

                if (
                    solicitacaoAbastecimentoGas.ClienteBase?.CPF_CNPJ == solicitacaoDuplicada.ClienteBase?.CPF_CNPJ &&
                    solicitacaoAbastecimentoGas.DataMedicao == solicitacaoDuplicada.DataMedicao &&
                    solicitacaoAbastecimentoGas.Abertura == solicitacaoDuplicada.Abertura &&
                    solicitacaoAbastecimentoGas.PrevisaoBombeio == solicitacaoDuplicada.PrevisaoBombeio &&
                    solicitacaoAbastecimentoGas.PrevisaoTransferenciaRecebida == solicitacaoDuplicada.PrevisaoTransferenciaRecebida &&
                    solicitacaoAbastecimentoGas.PrevisaoDemandaDomiciliar == solicitacaoDuplicada.PrevisaoDemandaDomiciliar &&
                    solicitacaoAbastecimentoGas.PrevisaoDemandaEmpresarial == solicitacaoDuplicada.PrevisaoDemandaEmpresarial &&
                    solicitacaoAbastecimentoGas.EstoqueUltrasystem == solicitacaoDuplicada.EstoqueUltrasystem &&
                    solicitacaoAbastecimentoGas.PrevisaoTransferenciaEnviada == solicitacaoDuplicada.PrevisaoTransferenciaEnviada &&
                    solicitacaoAbastecimentoGas.VolumeRodoviarioCarregamentoProximoDia == solicitacaoDuplicada.VolumeRodoviarioCarregamentoProximoDia &&
                    solicitacaoAbastecimentoGas.PrevisaoBombeioProximoDia == solicitacaoDuplicada.PrevisaoBombeioProximoDia &&
                    solicitacaoAbastecimentoGas.DisponibilidadeTransferenciaProximoDia == solicitacaoDuplicada.DisponibilidadeTransferenciaProximoDia &&
                    solicitacaoAbastecimentoGas.Produto.Codigo == solicitacaoDuplicada.Produto.Codigo &&
                    solicitacaoAbastecimentoGas.PrevisaoFechamento == solicitacaoDuplicada.PrevisaoFechamento
                )
                    return new JsonpResult(false, "Abastecimento de Gás não pode ser igual ao registro duplicado.");

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao verificar registro duplicado.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarQuantidadeAdicional()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Logistica/AbastecimentoGas");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.AbastecimentoGas_PermiteAdicionarVolumeExtraSolicitacaoGas))
                    return new JsonpResult(false, true, "Você não possui as permissões necessárias para realizar essa ação.");

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas repositorioSolicitacaoGas = new Repositorio.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoGas = repositorioSolicitacaoGas.BuscarPorCodigo(codigo);

                if (solicitacaoGas == null)
                    return new JsonpResult(false, "O registro não foi encontrado.");

                solicitacaoGas.Initialize();

                solicitacaoGas.AdicionalDisponibilidadeTransferenciaProximoDia = Request.GetDecimalParam("AdicionalDisponibilidadeTransferenciaProximoDia");
                solicitacaoGas.AdicionalVolumeRodoviarioCarregamentoProximoDia = Request.GetDecimalParam("AdicionalVolumeRodoviarioCarregamentoProximoDia");
                solicitacaoGas.DataAdicaoQuantidade = DateTime.Now;
                solicitacaoGas.UsuarioAdicaoQuantidade = Usuario;
                
                if (!ValidarSolicitacao(solicitacaoGas, unitOfWork))
                    throw new ControllerException("Não é possível lançar a solicitação pois a quantidade de disponibilidade informada é menor que a quantidade já utilizada.");

                Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas servicoSolicitacaoAbastecimentoGas = new Servicos.Embarcador.Logistica.SolicitacaoAbastecimentoGas(unitOfWork);
                servicoSolicitacaoAbastecimentoGas.CriarAprovacao(solicitacaoGas, TipoServicoMultisoftware);

                repositorioSolicitacaoGas.Atualizar(solicitacaoGas);

                string mensagem = string.Empty;

                if (solicitacaoGas.Situacao != SituacaoAprovacaoSolicitacaoGas.Aprovada)
                    mensagem = "A solicitação de abastecimento foi atualizada e está aguardando aprovação.";
                else
                    mensagem = "A solicitação de abastecimento foi atualizada com sucesso.";

                Servicos.Auditoria.Auditoria.Auditar(Auditado, solicitacaoGas, solicitacaoGas.GetChanges(), "Salvou quantidade adicional", unitOfWork);

                return new JsonpResult(true, true, mensagem);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                unitOfWork.Rollback();
                return new JsonpResult(false, "Ocorreu uma falha ao salvar a quantidade adicional.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaSolicitacaoAbastecimentoGas
            {
                DataSolicitacaoInicial = Request.GetNullableDateTimeParam("DataSolicitacaoInicial"),
                DataSolicitacaoFinal = Request.GetNullableDateTimeParam("DataSolicitacaoFinal"),
                CodigosBasesSatelite = Request.GetDoubleParam("Base") > 0 ? new List<double> { Request.GetDoubleParam("Base") } : new List<double>(),
                Situacao = Request.GetNullableEnumParam<SituacaoAprovacaoSolicitacaoGas>("Situacao")
            };
        }

        private bool ValidarSolicitacao(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas, Repositorio.UnitOfWork unitOfWork)
        {
            if (solicitacaoAbastecimentoGas.DisponibilidadeTransferenciaProximoDiaTotal <= 0)
                return true;

            Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas repositorioConsolidacao = new Repositorio.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas(unitOfWork);

            List<Dominio.Entidades.Embarcador.Logistica.ConsolidacaoSolicitacaoAbastecimentoGas> consolidacoes = repositorioConsolidacao.BuscarPorDataMedicaoProduto(solicitacaoAbastecimentoGas.DataMedicao.Date, solicitacaoAbastecimentoGas.Produto?.Codigo ?? 0);
            consolidacoes = consolidacoes.Where(obj => obj.Carga.SituacaoCarga != SituacaoCarga.Cancelada && obj.Carga.SituacaoCarga != SituacaoCarga.Anulada).ToList();

            return solicitacaoAbastecimentoGas.DisponibilidadeTransferenciaProximoDiaTotal >= consolidacoes.Where(obj => obj.ClienteBaseSupridora?.CPF_CNPJ == solicitacaoAbastecimentoGas.ClienteBase.CPF_CNPJ).Sum(c => c.QuantidadeCarga);
        }

        private void PreencherEntidade(Dominio.Entidades.Embarcador.Logistica.SolicitacaoAbastecimentoGas solicitacaoAbastecimentoGas, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Produtos.ProdutoEmbarcador repositorioProdutoEmbarcador = new Repositorio.Embarcador.Produtos.ProdutoEmbarcador(unitOfWork);

            Dominio.Entidades.Cliente cliente = repositorioCliente.BuscarPorCPFCNPJ(Request.GetDoubleParam("Base"));
            Dominio.Entidades.Embarcador.Produtos.ProdutoEmbarcador produtoEmbarcador = repositorioProdutoEmbarcador.BuscarPorCodigo(Request.GetIntParam("Produto"));

            solicitacaoAbastecimentoGas.ClienteBase = cliente;
            solicitacaoAbastecimentoGas.DataMedicao = DateTime.Now.Date.AddDays(Request.GetIntParam("DataMedicao"));
            solicitacaoAbastecimentoGas.Abertura = Request.GetDecimalParam("Abertura");
            solicitacaoAbastecimentoGas.PrevisaoBombeio = Request.GetDecimalParam("PrevisaoBombeio");
            solicitacaoAbastecimentoGas.PrevisaoTransferenciaRecebida = Request.GetDecimalParam("PrevisaoTransferenciaRecebida");
            solicitacaoAbastecimentoGas.PrevisaoDemandaDomiciliar = Request.GetDecimalParam("PrevisaoDemandaDomiciliar");
            solicitacaoAbastecimentoGas.PrevisaoDemandaEmpresarial = Request.GetDecimalParam("PrevisaoDemandaEmpresarial");
            solicitacaoAbastecimentoGas.EstoqueUltrasystem = Request.GetDecimalParam("EstoqueUltrasystem");
            solicitacaoAbastecimentoGas.PrevisaoTransferenciaEnviada = Request.GetDecimalParam("PrevisaoTransferenciaEnviada");
            solicitacaoAbastecimentoGas.DensidadeAberturaDia = Request.GetDecimalParam("DensidadeAberturaDia");
            solicitacaoAbastecimentoGas.VolumeRodoviarioCarregamentoProximoDia = Request.GetDecimalParam("VolumeRodoviarioCarregamentoProximoDia");
            solicitacaoAbastecimentoGas.VolumeRodoviarioCarregamentoProximoDiaOriginal = solicitacaoAbastecimentoGas.VolumeRodoviarioCarregamentoProximoDia;
            solicitacaoAbastecimentoGas.PrevisaoBombeioProximoDia = Request.GetDecimalParam("PrevisaoBombeioProximoDia");
            solicitacaoAbastecimentoGas.DisponibilidadeTransferenciaProximoDia = Request.GetDecimalParam("DisponibilidadeTransferenciaProximoDia");
            solicitacaoAbastecimentoGas.SaldoRestante = Request.GetDecimalParam("DisponibilidadeTransferenciaProximoDia");
            solicitacaoAbastecimentoGas.Usuario = this.Usuario;
            solicitacaoAbastecimentoGas.DataCriacao = DateTime.Now;
            solicitacaoAbastecimentoGas.DataUltimaAlteracao = DateTime.Now;
            solicitacaoAbastecimentoGas.Produto = produtoEmbarcador;
            solicitacaoAbastecimentoGas.PrevisaoFechamento = (solicitacaoAbastecimentoGas.Abertura + solicitacaoAbastecimentoGas.PrevisaoBombeio + solicitacaoAbastecimentoGas.PrevisaoTransferenciaRecebida) - (solicitacaoAbastecimentoGas.PrevisaoDemandaDomiciliar + solicitacaoAbastecimentoGas.PrevisaoDemandaEmpresarial + solicitacaoAbastecimentoGas.EstoqueUltrasystem + solicitacaoAbastecimentoGas.PrevisaoTransferenciaEnviada);
        }

        #endregion
    }
}
