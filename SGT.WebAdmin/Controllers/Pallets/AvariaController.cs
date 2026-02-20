using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize(new string[] { "ExportarPesquisa", "Pesquisa" }, "Pallets/Avaria")]
    public class AvariaController : BaseController
    {
		#region Construtores

		public AvariaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarDadosAvaria()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                var repositorioAvaria = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var avaria = new Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet();
                List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade> quantidadesAvariadas;

                try
                {
                    PreencherDadosAvariaAdicionar(unitOfWork, avaria, repositorioAvaria);

                    quantidadesAvariadas = ObterQuantidadesAvariadasAdicionar(unitOfWork, avaria);
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, false, excecao.Message);
                }
                
                var repositorioQuantidadesAvariadas = new Repositorio.Embarcador.Pallets.AvariaPalletQuantidade(unitOfWork);

                repositorioAvaria.Inserir(avaria);

                foreach (var quantidadeAvariada in quantidadesAvariadas)
                {
                    repositorioQuantidadesAvariadas.Inserir(quantidadeAvariada);
                }

                var servicoAvaria = new Servicos.Embarcador.Pallets.Avaria(unitOfWork);

                servicoAvaria.EtapaAprovacao(avaria, TipoServicoMultisoftware);

                repositorioAvaria.Atualizar(avaria);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, avaria, null, "Adicionado", unitOfWork, Dominio.ObjetosDeValor.Enumerador.AcaoBancoDados.Insert);

                unitOfWork.CommitChanges();

                return new JsonpResult(avaria.Codigo);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao adicionar os dados da avaria.");
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
                var repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var avaria = repositorio.BuscarPorCodigo(codigo);

                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                return new JsonpResult(new
                {
                    avaria.Codigo,
                    avaria.Situacao,
                    DadosAvaria = ObterDadosAvaria(avaria),
                    QuantidadesAvariadas = ObterQuantidadesAvariadas(avaria),
                    Anexos = ObterDadosAvariaAnexos(avaria),
                    ResumoAprovacao = ObterResumoAprovacao(unitOfWork, avaria)
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

        public async Task<IActionResult> BuscarSituacoes()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var repositorio = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);
                var situacoes = repositorio.BuscarAtivosPorSituacaoPalletAvariado();

                return new JsonpResult(
                    (
                        from situacao in situacoes
                        select new
                        {
                            situacao.Codigo,
                            situacao.Descricao
                        }
                    ).ToList()
                );
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao obter os dados das situações de devolução de pallets.");
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
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria(unitOfWork);
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
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria(unitOfWork);
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
                var repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var avaria = repositorio.BuscarPorCodigo(codigo);

                if (avaria == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (avaria.Situacao != SituacaoAvariaPallet.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                var servicoAvaria = new Servicos.Embarcador.Pallets.Avaria(unitOfWork);

                servicoAvaria.EtapaAprovacao(avaria, TipoServicoMultisoftware);

                repositorio.Atualizar(avaria);

                unitOfWork.CommitChanges();

                return new JsonpResult(avaria.Situacao != SituacaoAvariaPallet.SemRegraAprovacao);
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

        private dynamic ObterDadosAvaria(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            return new
            {
                avaria.Codigo,
                avaria.Observacao,
                Filial = new { Codigo = avaria.Filial?.Codigo ?? 0, Descricao = avaria.Filial?.Descricao ?? "" },
                Transportador = new { Codigo = avaria.Transportador?.Codigo ?? 0, Descricao = avaria.Transportador?.Descricao ?? "" },
                MotivoAvaria = new { avaria.MotivoAvaria.Codigo, avaria.MotivoAvaria.Descricao },
                avaria.Numero,
                Setor = avaria.Setor == null ? null : new { avaria.Setor.Codigo, avaria.Setor.Descricao }
            };
        }

        private dynamic ObterDadosAvariaAnexos(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            return (
                from anexo in avaria.Anexos
                select new
                {
                    anexo.Codigo,
                    anexo.Descricao,
                    anexo.NomeArquivo
                }
            ).ToList();
        }

        private Dominio.Entidades.Embarcador.Filiais.Filial ObterFilial(Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            var codigoFilial = Request.GetIntParam("Filial");

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

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    grid.AdicionarCabecalho(descricao: "Empresa/Filial", propriedade: "Transportador", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                else
                    grid.AdicionarCabecalho(descricao: "Filial", propriedade: "Filial", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                grid.AdicionarCabecalho(descricao: "Setor", propriedade: "Setor", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);
                grid.AdicionarCabecalho(descricao: "Motivo Avaria", propriedade: "MotivoAvaria", tamanho: 15, alinhamento: Models.Grid.Align.left, permiteOrdenacao: true);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaAvaria()
                {
                    Numero = Request.GetIntParam("Numero"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoSetor = Request.GetIntParam("Setor"),
                    CodigoMotivoAvaria = Request.GetIntParam("MotivoAvaria"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                    DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                    Situacao = Request.GetEnumParam<SituacaoAvariaPallet>("Situacao")
                };

                var repositorio = new Repositorio.Embarcador.Pallets.AvariaPallet(unitOfWork);
                var avarias = repositorio.Consultar(filtrosPesquisa, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                var totalRegistros = repositorio.ContarConsulta(filtrosPesquisa);
                var lista = (from avaria in avarias
                    select new
                    {
                        avaria.Codigo,
                        Data = avaria.Data.ToString("dd/MM/yyyy"),
                        Filial = avaria.Filial?.Descricao,
                        Transportador = avaria.Transportador?.Descricao,
                        MotivoAvaria = avaria.MotivoAvaria.Descricao,
                        avaria.Numero,
                        Setor = avaria.Setor?.Descricao,
                        Situacao = avaria.Situacao.ObterDescricao()
                    }
                ).ToList();

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

        private Dominio.Entidades.Embarcador.Pallets.MotivoAvariaPallet ObterMotivoAvaria(Repositorio.UnitOfWork unitOfWork)
        {
            var codigoMotivoAvaria = Request.GetIntParam("MotivoAvaria");

            if (codigoMotivoAvaria <= 0)
                throw new Exception("Motivo da avaria não informado");

            var repositorio = new Repositorio.Embarcador.Pallets.MotivoAvariaPallet(unitOfWork);
            var motivoAvaria = repositorio.BuscarPorCodigo(codigoMotivoAvaria);

            if (motivoAvaria == null)
                throw new Exception("Motivo da avaria não encontrado");

            return motivoAvaria;
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Filial")
                return "Filial.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "MotivoAvaria")
                return "MotivoAvaria.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Setor")
                return "Setor.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "Transportador.RazaoSocial";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private dynamic ObterQuantidadesAvariadas(Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            return (
                from quantidadeAvariada in avaria.QuantidadesAvariadas
                select new {
                    quantidadeAvariada.SituacaoDevolucaoPallet.Codigo,
                    quantidadeAvariada.SituacaoDevolucaoPallet.Descricao,
                    quantidadeAvariada.Quantidade
                }
            ).ToList();
        }

        private List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade> ObterQuantidadesAvariadasAdicionar(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            var listaQuantidadesAvariadas = new List<Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade>();
            var quantidadesAvariadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("QuantidadesAvariadas"));
            var repositorioSituacaoDevolucao = new Repositorio.Embarcador.Pallets.SituacaoDevolucaoPallet(unitOfWork);

            foreach (var quantidadeAvariada in quantidadesAvariadas)
            {
                var quantidade = ((string)quantidadeAvariada.Quantidade).ToInt();
                var situacaoDevolucao = repositorioSituacaoDevolucao.BuscarPorCodigo(((string)quantidadeAvariada.Codigo).ToInt());

                if (situacaoDevolucao == null)
                    throw new Exception("Situação de devolução de pallet não encontrada");

                var avariaPalletQuantidade = new Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPalletQuantidade()
                {
                    AvariaPallet = avaria,
                    Quantidade = quantidade,
                    SituacaoDevolucaoPallet = situacaoDevolucao
                };

                listaQuantidadesAvariadas.Add(avariaPalletQuantidade);
            }

            if (listaQuantidadesAvariadas.Count == 0)
                throw new Exception("Nenhuma quantidade avariada informada");

            return listaQuantidadesAvariadas;
        } 

        private dynamic ObterResumoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasAvaria.AprovacaoAlcadaAvaria(unitOfWork);
            var aprovacoes = repositorioAprovacao.ContarAprovacoes(avaria.Codigo);
            var aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(avaria.Codigo);
            var reprovacoes = repositorioAprovacao.ContarReprovacoes(avaria.Codigo);

            return new
            {
                Solicitante = avaria.Solicitante.Descricao,
                DataSolicitacao = avaria.Data.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = avaria.Situacao.ObterDescricao(),
            };
        }

        private Dominio.Entidades.Setor ObterSetor(Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            var codigoSetor = Request.GetIntParam("Setor");

            if (codigoSetor <= 0)
                return null;

            var repositorio = new Repositorio.Setor(unitOfWork);
            var setor = repositorio.BuscarPorCodigo(codigoSetor);

            if (setor == null)
                throw new Exception("Setor não encontrado");

            return setor;
        }

        private Dominio.Entidades.Empresa ObterTransportador(Repositorio.UnitOfWork unitOfWork)
        {
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return null;

            int codigoTransportador = Request.GetIntParam("Transportador");

            if (codigoTransportador <= 0)
                throw new Exception("Empresa/Filial não informada");

            Repositorio.Empresa repositorio = new Repositorio.Empresa(unitOfWork);
            Dominio.Entidades.Empresa transportador = repositorio.BuscarPorCodigo(codigoTransportador);

            if (transportador == null)
                throw new Exception("Empresa/Filial não encontrada");

            return transportador;
        }

        private void PreencherDadosAvariaAdicionar(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.Avaria.AvariaPallet avaria, Repositorio.Embarcador.Pallets.AvariaPallet repositorioAvaria)
        {
            var observacao = Request.Params("Observacao");

            avaria.Data = DateTime.Now;
            avaria.Transportador = ObterTransportador(unitOfWork);
            avaria.Filial = ObterFilial(unitOfWork);
            avaria.MotivoAvaria = ObterMotivoAvaria(unitOfWork);
            avaria.Numero = repositorioAvaria.BuscarProximoNumero();
            avaria.Observacao = String.IsNullOrWhiteSpace(observacao) ? string.Empty : observacao;
            avaria.Setor = ObterSetor(unitOfWork);
            avaria.Situacao = SituacaoAvariaPallet.AguardandoAprovacao;
            avaria.Solicitante = this.Usuario;
        }

        #endregion
    }
}
