using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pallets
{
    [CustomAuthorize("Pallets/DevolucaoValePallet")]
    public class DevolucaoValePalletController : BaseController
    {
		#region Construtores

		public DevolucaoValePalletController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Baixar()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Pallets/DevolucaoValePallet");

                var codigo = Request.GetIntParam("CodigoValePallet");
                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var valePallet = repositorioValePallet.BuscarPorCodigo(codigo);

                DateTime dataDevolucao = DateTime.Today;
                if (permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Pallets_PermiteDataRetroativa_DevolucaoPallet))
                    dataDevolucao = Request.GetDateTimeParam("DataDevolucao");

                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (dataDevolucao == DateTime.MinValue)
                    return new JsonpResult(false, true, "Data de devolução é obrigatória.");

                Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet;

                try
                {
                    devolucaoValePallet = new Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet()
                    {
                        Data = dataDevolucao,
                        Filial = ObterFilial(unitOfWork, Request.GetIntParam("Filial")),
                        Numero = repositorioValePallet.BuscarProximoNumero(),
                        Observacao = ObterObservacao(),
                        QuantidadePallets = valePallet.Quantidade,
                        Setor = ObterSetor(unitOfWork, Request.GetIntParam("Setor")),
                        Situacao = SituacaoDevolucaoValePallet.AguardandoAprovacao,
                        ValePallet = valePallet,
                    };
                }
                catch (Exception excecao)
                {
                    return new JsonpResult(false, true, excecao.Message);
                }

                unitOfWork.Start();

                var repositorioDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);

                repositorioDevolucaoValePallet.Inserir(devolucaoValePallet, Auditado);

                var servicoDevolucaoValePallet = new Servicos.Embarcador.Pallets.DevolucaoValePallet(unitOfWork, Auditado);

                servicoDevolucaoValePallet.EtapaAprovacao(devolucaoValePallet, TipoServicoMultisoftware);

                repositorioDevolucaoValePallet.Atualizar(devolucaoValePallet);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {

                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, excecao.Message);

            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, false, "Ocorreu uma falha ao solicitar a baixa do vale pallets.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigoValePallet()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioValePallet = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
                var valePallet = repositorioValePallet.BuscarPorCodigo(codigo);

                if (valePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var repositorioDevolucaoValePallet = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);
                var devolucaoValePallet = repositorioDevolucaoValePallet.BuscarPorValePallet(codigo);

                if (devolucaoValePallet == null)
                {
                    devolucaoValePallet = new Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet()
                    {
                        Data = DateTime.Now,
                        Filial = this.Usuario.Filial,
                        QuantidadePallets = valePallet.Quantidade,
                        Setor = this.Usuario.Setor,
                        ValePallet = valePallet
                    };
                }

                return new JsonpResult(new
                {
                    DadosDevolucao = ObterDadosDevolucao(devolucaoValePallet),
                    ResumoAprovacao = ObterResumoAprovacao(unitOfWork, devolucaoValePallet)
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

        public async Task<IActionResult> DetalhesAutorizacao()
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                var codigo = Request.GetIntParam("Codigo");
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet(unitOfWork);
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
                var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet(unitOfWork);
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
                var repositorio = new Repositorio.Embarcador.Pallets.DevolucaoValePallet(unitOfWork);
                var devolucaoValePallet = repositorio.BuscarPorCodigo(codigo);

                if (devolucaoValePallet == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (devolucaoValePallet.Situacao != SituacaoDevolucaoValePallet.SemRegraAprovacao)
                    return new JsonpResult(false, true, "A situação não permite esta operação.");

                var servicoDevolucaoValePallet = new Servicos.Embarcador.Pallets.DevolucaoValePallet(unitOfWork, Auditado);

                servicoDevolucaoValePallet.EtapaAprovacao(devolucaoValePallet, TipoServicoMultisoftware);

                repositorio.Atualizar(devolucaoValePallet);

                unitOfWork.CommitChanges();

                return new JsonpResult(devolucaoValePallet.Situacao != SituacaoDevolucaoValePallet.SemRegraAprovacao);
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

        private dynamic ObterDadosDevolucao(Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet)
        {
            return new JsonpResult(new
            {
                devolucaoValePallet.Codigo,
                CodigoValePallet = devolucaoValePallet.ValePallet.Codigo,
                devolucaoValePallet.Data,
                Filial = devolucaoValePallet.Filial == null ? null : new { devolucaoValePallet.Filial.Codigo, devolucaoValePallet.Filial.Descricao },
                Numero = devolucaoValePallet.Numero > 0 ? devolucaoValePallet.Numero.ToString() : "",
                NumeroValePallet = devolucaoValePallet.ValePallet.Numero,
                devolucaoValePallet.Observacao,
                Quantidade = devolucaoValePallet.QuantidadePallets,
                Setor = devolucaoValePallet.Setor == null ? null : new { devolucaoValePallet.Setor.Codigo, devolucaoValePallet.Setor.Descricao },
                devolucaoValePallet.Situacao
            });
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

                grid.Prop("Codigo");
                grid.Prop("Numero").Nome("Número").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("Data").Nome("Data").Tamanho(10).Align(Models.Grid.Align.center);
                grid.Prop("NFe").Nome("NF-e").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("Transportador").Nome("Transportador").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("Motorista").Nome("Motorista").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("Cliente").Nome("Cliente").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("Quantidade").Nome("Quantidade").Tamanho(7).Align(Models.Grid.Align.center);
                grid.Prop("Cidade").Nome("Cidade").Tamanho(15).Align(Models.Grid.Align.left);
                grid.Prop("Representante").Nome("Representante").Tamanho(10).Align(Models.Grid.Align.left);
                grid.Prop("Situacao").Nome("Situação").Tamanho(10).Align(Models.Grid.Align.left);

                var propriedadeOrdenar = ObterPropriedadeOrdenar(grid);
                int totalRegistros = 0;

                var lista = Pesquisar(ref totalRegistros, propriedadeOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

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

        private string ObterObservacao()
        {
            var observacao = Request.Params("Observacao");

            return String.IsNullOrWhiteSpace(observacao) ? String.Empty : observacao.Trim();
        }

        private string ObterPropriedadeOrdenar(Models.Grid.Grid grid)
        {
            if (grid.header[grid.indiceColunaOrdena].data == "Data")
                return "DataLancamento";

            if (grid.header[grid.indiceColunaOrdena].data == "NFe")
                return "Devolucao.XMLNotaFiscal.Numero";

            if (grid.header[grid.indiceColunaOrdena].data == "Transportador")
                return "Devolucao.Transportador.RazaoSocial";

            if (grid.header[grid.indiceColunaOrdena].data == "Motorista")
                return "Devolucao.CargaPedido.Carga.NomeMotoristas";

            if (grid.header[grid.indiceColunaOrdena].data == "Cliente")
                return "Devolucao.XMLNotaFiscal.Destinatario.Nome";

            if (grid.header[grid.indiceColunaOrdena].data == "Cidade")
                return "Devolucao.Filial.Localidade.Descricao";

            if (grid.header[grid.indiceColunaOrdena].data == "Representante")
                return "Representante.Descricao";

            return grid.header[grid.indiceColunaOrdena].data;
        }

        private dynamic ObterResumoAprovacao(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Pallets.DevolucaoValePallet devolucaoValePallet)
        {
            var repositorioAprovacao = new Repositorio.Embarcador.Pallets.AlcadasDevolucaoValePallet.AprovacaoAlcadaDevolucaoValePallet(unitOfWork);
            var aprovacoes = repositorioAprovacao.ContarAprovacoes(devolucaoValePallet.Codigo);
            var aprovacoesNecessarias = repositorioAprovacao.ContarAprovacoesNecessarias(devolucaoValePallet.Codigo);
            var reprovacoes = repositorioAprovacao.ContarReprovacoes(devolucaoValePallet.Codigo);

            return new
            {
                Solicitante = "Sem Solicitante",
                DataSolicitacao = devolucaoValePallet.Data.ToString("dd/MM/yyyy"),
                AprovacoesNecessarias = aprovacoesNecessarias,
                Aprovacoes = aprovacoes,
                Reprovacoes = reprovacoes,
                Situacao = devolucaoValePallet.Situacao.ObterDescricao(),
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

        private dynamic Pesquisar(ref int totalRegistros, string propriedadeOrdenar, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            var repositorio = new Repositorio.Embarcador.Pallets.ValePallet(unitOfWork);
            var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Pallets.FiltroPesquisaDevolucaoValePallet()
            {
                CodigoFilial = Request.GetIntParam("Filial"),
                CpfCnpjCliente = Request.GetDoubleParam("Cliente"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Nfe = Request.GetIntParam("Nfe"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoValePallet>("Situacao")
            };
            var listaValePallet = repositorio.ConsultarDevolucaoValePallet(filtrosPesquisa, propriedadeOrdenar, direcaoOrdenacao, inicioRegistros, maximoRegistros);

            totalRegistros = repositorio.ContarConsultaControleValePallet(filtrosPesquisa);

            return (
                from valePallet in listaValePallet
                select new
                {
                    valePallet.Codigo,
                    Data = valePallet.DataLancamento.ToString("dd/MM/yyyy"),
                    valePallet.Numero,
                    NFe = valePallet.Devolucao.XMLNotaFiscal?.Numero.ToString() ?? string.Empty,
                    Transportador = valePallet.Devolucao?.Transportador?.Descricao ?? string.Empty,
                    Motorista = valePallet.Devolucao.CargaPedido?.Carga?.NomeMotoristas ?? string.Empty,
                    Cliente = valePallet.Devolucao.XMLNotaFiscal?.Destinatario?.Descricao ?? string.Empty,
                    valePallet.Quantidade,
                    Cidade = valePallet.Devolucao.Filial?.Localidade?.Descricao ?? string.Empty,
                    Representante = valePallet.Representante?.Descricao ?? string.Empty,
                    Situacao = valePallet.Situacao.ObterDescricao(),
                }
            )
            .ToList();
        }

        #endregion
    }
}
