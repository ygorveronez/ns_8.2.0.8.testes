using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Chamados
{
    [CustomAuthorize(new string[] { "ObterDetalhes", "RegrasAprovacao" }, "Chamados/LoteChamadoOcorrencia")]
    public class LoteChamadoOcorrenciaController : BaseController
    {
		#region Construtores

		public LoteChamadoOcorrenciaController(Conexao conexao) : base(conexao) { }

		#endregion


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
        public async Task<IActionResult> PesquisarAtendimentos()
        {
            try
            {
                return new JsonpResult(ObterGridPesquisaAtendimentosPendentes());
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisaAtendimentosPendentes()
        {
            try
            {
                var grid = ObterGridPesquisaAtendimentosPendentes();

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
                int codigoLoteChamadoOcorrencia = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia lote = repLoteChamadoOcorrencia.BuscarPorCodigo(codigoLoteChamadoOcorrencia);

                if (lote == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                var chamados = repChamado.BuscarPorLote(lote.Codigo);


                var retorno = new
                {
                    Codigo = lote.Codigo,
                    Situacao = (int)lote.Situacao,
                    AtendimentosSelecionados = chamados != null && lote.Situacao == SituacaoLoteChamadoOcorrencia.EmEdicao ? (
                        from obj in chamados
                        select new
                        {
                            DT_RowId = obj.Codigo.ToString(),
                            Codigo = obj.Codigo
                        }
                    ).ToList() : null,
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

        public async Task<IActionResult> GerarLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosAtendimento = Request.GetListParam<int>("Atendimentos");
                int codigoLote = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> atendimentos = repChamado.BuscarPorCodigos(codigosAtendimento);
                if (atendimentos == null || atendimentos.Count == 0)
                    throw new ControllerException("Não foi possível encontrar os registros");

                unitOfWork.Start();
                Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia lote = codigoLote > 0 ? repLoteChamadoOcorrencia.BuscarPorCodigo(codigoLote) : null;

                if (lote == null)
                {
                    lote = new Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia()
                    {
                        DataCriacao = DateTime.Now,
                        NumeroLote = repLoteChamadoOcorrencia.ObterProximoNumero(),
                        Situacao = SituacaoLoteChamadoOcorrencia.AgAprovacao
                    };
                    repLoteChamadoOcorrencia.Inserir(lote);
                }
                else
                {
                    lote.Situacao = SituacaoLoteChamadoOcorrencia.AgAprovacao;
                    repLoteChamadoOcorrencia.Atualizar(lote);
                }

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = codigoLote > 0 ? repChamado.BuscarPorLote(lote.Codigo) : new List<Dominio.Entidades.Embarcador.Chamados.Chamado>(); ;

                foreach (var atendimento in atendimentos)
                {
                    if (chamados.Contains(atendimento))
                        continue;

                    atendimento.Situacao = SituacaoChamado.AgAprovacaoLote;
                    atendimento.LoteChamadoOcorrencia = lote;
                    repChamado.Atualizar(atendimento);
                }
                foreach (var chamado in chamados)
                {
                    if (!atendimentos.Contains(chamado))
                    {
                        chamado.LoteChamadoOcorrencia = null;
                        repChamado.Atualizar(chamado);
                    }
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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

        public async Task<IActionResult> SalvarEdicao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                List<int> codigosAtendimento = Request.GetListParam<int>("Atendimentos");
                int codigoLote = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia lote = codigoLote > 0 ? repLoteChamadoOcorrencia.BuscarPorCodigo(codigoLote) : null;


                List<Dominio.Entidades.Embarcador.Chamados.Chamado> atendimentos = repChamado.BuscarPorCodigos(codigosAtendimento);
                if (atendimentos == null || atendimentos.Count == 0)
                    throw new ControllerException("Não foi possível encontrar os registros");

                unitOfWork.Start();

                if (lote == null)
                {
                    lote = new Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia()
                    {
                        DataCriacao = DateTime.Now,
                        NumeroLote = repLoteChamadoOcorrencia.ObterProximoNumero(),
                        Situacao = SituacaoLoteChamadoOcorrencia.EmEdicao
                    };
                    repLoteChamadoOcorrencia.Inserir(lote);
                }

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = codigoLote > 0 ? repChamado.BuscarPorLote(lote.Codigo) : new List<Dominio.Entidades.Embarcador.Chamados.Chamado>(); ;

                foreach (var atendimento in atendimentos)
                {
                    if (chamados.Contains(atendimento))
                        continue;
                    
                    atendimento.LoteChamadoOcorrencia = lote;
                    repChamado.Atualizar(atendimento);
                }
                foreach(var chamado in chamados)
                {
                    if (!atendimentos.Contains(chamado))
                    {
                        chamado.LoteChamadoOcorrencia = null;
                        repChamado.Atualizar(chamado);
                    }
                }
                

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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

        public async Task<IActionResult> ExcluirLote()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoLote = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.LoteChamadoOcorrencia lote = codigoLote > 0 ? repLoteChamadoOcorrencia.BuscarPorCodigo(codigoLote) : null;

                unitOfWork.Start();

                if (lote == null)
                    throw new ControllerException("Não foi possível encontrar o registro");

                if (lote.Situacao != SituacaoLoteChamadoOcorrencia.EmEdicao)
                    throw new ControllerException("O lote não está em uma situação válida para exclusão");

                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = repChamado.BuscarPorLote(lote.Codigo);

                foreach (var atendimento in chamados)
                {
                    atendimento.LoteChamadoOcorrencia = null;
                    repChamado.Atualizar(atendimento);
                }

                repLoteChamadoOcorrencia.Deletar(lote);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, excecao.Message);
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

        public async Task<IActionResult> ObterDetalhesAtendimento()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoAtendimento = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                var chamado = repChamado.BuscarPorCodigo(codigoAtendimento);

                if (chamado == null)
                    return new JsonpResult(false, "Não foi possível encontrar o registro.");

                var retorno = new
                {
                    Codigo = chamado.Codigo,
                    Situacao = chamado.Situacao.ObterDescricao(),
                    Carga = chamado.Carga.CodigoCargaEmbarcador,
                    Numero = chamado.Numero,
                    Transportador = chamado.Empresa?.Descricao ?? string.Empty,
                    Responsavel = chamado.Responsavel?.Nome ?? string.Empty,
                    DataCriacao = chamado.DataCriacao.ToString("g"),
                    MotivoChamado = chamado.MotivoChamado?.Descricao ?? string.Empty,
                    Veiculo = chamado.Carga?.Veiculo?.Placa ?? string.Empty
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


        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia()
            {
                NumeroLote = Request.GetIntParam("NumeroLote"),
                DataCriacaoInicial = Request.GetDateTimeParam("DataCriacaoInicial"),
                DataCriacaoFinal = Request.GetDateTimeParam("DataCriacaoFinal"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Transportador"),
                Situacao = Request.GetListEnumParam<SituacaoLoteChamadoOcorrencia>("Situacao"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número Lote", "NumeroLote", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Criação", "DataCriacao", 5, Models.Grid.Align.left, false);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Transportadores", false);
                else
                    grid.AdicionarCabecalho("Transportador", "Transportadores", 9, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 9, Models.Grid.Align.left, false);

                    

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "LoteChamadoOcorrencia/Pesquisa", "grid-lote-chamado-ocorrencia");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));


                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaLoteChamadoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repLoteChamadoOcorrencia.Contar(filtrosPesquisa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                IList<Dominio.ObjetosDeValor.Embarcador.Chamado.LoteChamadoOcorrencia> lotes = (totalRegistros > 0) ? repLoteChamadoOcorrencia.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Chamado.LoteChamadoOcorrencia>();

                grid.AdicionaRows(lotes);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes ObterFiltrosPesquisaAtendimentosPendentes()
        {
            Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes()
            {
                NumeroInicial = Request.GetIntParam("NumeroInicial"),
                NumeroFinal = Request.GetIntParam("NumeroFinal"),
                DataCriacaoInicial = Request.GetDateTimeParam("DataInicial"),
                DataCriacaoFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigoCliente = Request.GetDoubleParam("Cliente"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoGrupoMotivoAtendimento = Request.GetIntParam("GrupoMotivoAtendimento"),
                CodigoMotivoChamado = Request.GetIntParam("MotivoChamado"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? this.Usuario.Empresa.Codigo : Request.GetIntParam("Transportador"),
                CodigoLote = Request.GetIntParam("Codigo"),
                SituacaoLote = Request.GetNullableEnumParam<SituacaoLoteChamadoOcorrencia>("SituacaoLote")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisaAtendimentosPendentes()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request) { header = new List<Models.Grid.Head>() };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Carga", "NumeroCarga", 5, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Grupo Motivo Atendimento", "GrupoMotivoAtendimento", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motivo do Chamado", "MotivoChamado", 9, Models.Grid.Align.left, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    grid.AdicionarCabecalho("Transportador", false);
                else
                    grid.AdicionarCabecalho("Transportador", "Transportador", 9, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Cliente", "Cliente", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data da Criação", "DataCriacao", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Notas Fiscais", "NotasFiscais", 9, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Filial", "Filial", 9, Models.Grid.Align.left, false);

                Models.Grid.GridPreferencias gridPreferencias = new Models.Grid.GridPreferencias(unitOfWork, "LoteChamadoOcorrencia/Pesquisa", "grid-lote-chamado-ocorrencia");
                grid.AplicarPreferenciasGrid(gridPreferencias.ObterPreferenciaGrid(this.Usuario.Codigo, grid.modelo));


                Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia repLoteChamadoOcorrencia = new Repositorio.Embarcador.Chamados.LoteChamadoOcorrencia(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaAtendimentosPendentes filtrosPesquisa = ObterFiltrosPesquisaAtendimentosPendentes();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                int totalRegistros = repLoteChamadoOcorrencia.ContarAtendimentosPendentes(filtrosPesquisa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                IList<Dominio.ObjetosDeValor.Embarcador.Chamado.AtendimentoPendente> atendimentos = (totalRegistros > 0) ? repLoteChamadoOcorrencia.ConsultarAtendimentosPendentes(filtrosPesquisa, parametrosConsulta) : new List<Dominio.ObjetosDeValor.Embarcador.Chamado.AtendimentoPendente>();

                grid.AdicionaRows(atendimentos);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Equals("DataGeracaoIrregularidadeFormatada"))
                return propriedadeOrdenar.Replace("Formatada", "");

            if (propriedadeOrdenar.Equals("SituacaoLoteChamadoOcorrencia"))
                return "Situacao";

            if (propriedadeOrdenar.Equals("ResponsavelPelaIrregularidade"))
                return "ServicoResponsavel";

            return propriedadeOrdenar;
        }

        #endregion Métodos Privados
    }
}
