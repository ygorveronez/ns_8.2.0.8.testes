using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores.Frete;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ApuracaoBonificacao")]
    public class ApuracaoBonificacaoController : BaseController
    {
		#region Construtores

		public ApuracaoBonificacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Processar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int ano = Request.GetIntParam("Ano");
                Mes mes = Request.GetEnumParam<Mes>("Mes");
                List<int> codigodRegraApuracao = Request.GetListParam<int>("RegraApuracao");

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.BonificacaoTransportador repositorioBonificacaoTransportador = new Repositorio.Embarcador.Frete.BonificacaoTransportador(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador> bonificacaoTransportador = repositorioBonificacaoTransportador.BuscarPorCodigos(codigodRegraApuracao);

                Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacaoDuplicado = repositorioApuracaoBonificacao.BuscarApuracaoDuplicada(ano, mes, codigodRegraApuracao);

                if (apuracaoBonificacaoDuplicado != null)
                    throw new ControllerException($"O período informado já possui um fechamento (Número: {apuracaoBonificacaoDuplicado.Numero}).");

                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao = new Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao()
                {
                    Ano = ano,
                    DataCriacao = DateTime.Now,
                    Mes = mes,
                    Numero = repositorioApuracaoBonificacao.BuscarProximoNumero(),
                    Situacao = SituacaoApuracaoBonificacao.AguardandoGeracaoOcorrencia,
                    Usuario = Usuario,
                    RegrasApuracao = bonificacaoTransportador
                };

                repositorioApuracaoBonificacao.Inserir(apuracaoBonificacao);

                ProcessarFechamento(apuracaoBonificacao, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { apuracaoBonificacao.Codigo });
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o fechamento.");
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
                Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao = repositorioApuracaoBonificacao.BuscarPorCodigo(codigo, false);

                if (apuracaoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var fechamentoPontuacaoRetornar = new
                {
                    apuracaoBonificacao.Ano,
                    apuracaoBonificacao.Codigo,
                    apuracaoBonificacao.Mes,
                    apuracaoBonificacao.Numero,
                    apuracaoBonificacao.Situacao,
                    RegraApuracao = (from o in apuracaoBonificacao.RegrasApuracao
                                     select new
                                     {
                                         o.Codigo,
                                         o.Descricao
                                     }).ToList(),
                    TotalAcrescimo = repositorioApuracaoBonificacao.BuscarTotalAcrescimoOuDesconto(apuracaoBonificacao.Codigo, TipoAjusteValor.Acrescimo).ToString("n2"),
                    TotalDesconto = repositorioApuracaoBonificacao.BuscarTotalAcrescimoOuDesconto(apuracaoBonificacao.Codigo, TipoAjusteValor.Desconto).ToString("n2")
                };

                return new JsonpResult(fechamentoPontuacaoRetornar);
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

        public async Task<IActionResult> Cancelar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");
                Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);
                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao = repositorioApuracaoBonificacao.BuscarPorCodigo(codigo, false);

                if (apuracaoBonificacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (apuracaoBonificacao.Situacao == SituacaoApuracaoBonificacao.Cancelado)
                    return new JsonpResult(false, true, "O fechamento já está cancelado.");

                if (apuracaoBonificacao.Situacao == SituacaoApuracaoBonificacao.Finalizado)
                    return new JsonpResult(false, true, "O fechamento já está finalizado.");

                unitOfWork.Start();

                apuracaoBonificacao.Situacao = SituacaoApuracaoBonificacao.Cancelado;

                repositorioApuracaoBonificacao.Atualizar(apuracaoBonificacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar o fechamento.");
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
                Models.Grid.Grid grid = ObterGridPesquisa();
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
        public async Task<IActionResult> ExportarPesquisaFechamentoApuracao()
        {
            try
            {
                Models.Grid.Grid grid = ObterGridFechamentoApuracao();
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
        public async Task<IActionResult> PesquisaFechamentoApuracao()
        {
            try
            {
                return new JsonpResult(ObterGridFechamentoApuracao());
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> GerarOcorrenciasPGT()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                unitOfWork.Start();

                Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);
                Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento repositorioApuracaoBonificacaoFechamento = new Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento(unitOfWork);

                Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao = repositorioApuracaoBonificacao.BuscarPorCodigo(codigo, false);
                List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento> apuracaoBonificacaoFechamentos = repositorioApuracaoBonificacaoFechamento.BuscarPorApuracaoBonificacao(apuracaoBonificacao.Codigo);

                foreach (Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento fechamento in apuracaoBonificacaoFechamentos)
                {
                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ocorrencia = srvOcorrencia.GerarOcorrenciaPGT(fechamento.CargaMaiorValor, fechamento.ValorOcorrencia, fechamento.RegraApuracao.TipoOcorrencia, unitOfWork, TipoServicoMultisoftware, Cliente);
                    if (ocorrencia == null)
                        throw new ControllerException($"Ocorreu uma falha ao gerar uma ocorrência do transportador {fechamento.Empresa.NomeCNPJ}. A apuração está incompleta.");

                    fechamento.Ocorrencia = ocorrencia;
                    repositorioApuracaoBonificacaoFechamento.Atualizar(fechamento);
                }

                apuracaoBonificacao.Situacao = SituacaoApuracaoBonificacao.Finalizado;
                repositorioApuracaoBonificacao.Atualizar(apuracaoBonificacao);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as ocorrências.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void DefinirPropriedadeOrdenar(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            if (parametrosConsulta.PropriedadeOrdenar == "Descricao")
                parametrosConsulta.PropriedadeOrdenar = $"Ano {parametrosConsulta.DirecaoOrdenar}, Mes";
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao()
            {
                Ano = Request.GetIntParam("Ano"),
                Mes = Request.GetNullableEnumParam<Mes>("Mes"),
                Numero = Request.GetIntParam("Numero"),
                Situacao = Request.GetNullableEnumParam<SituacaoApuracaoBonificacao>("Situacao"),
                CodigosRegraApuracao = Request.GetNullableListParam<int>("RegraApuracao")
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
                grid.AdicionarCabecalho("Número", "Numero", 15, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Mês", "Mes", 25, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Ano", "Ano", 25, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação", "Situacao", 25, Models.Grid.Align.center, false);

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaApuracaoBonificacao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                DefinirPropriedadeOrdenar(parametrosConsulta);
                Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);
                int totalRegistros = repositorioApuracaoBonificacao.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao> listaApuracaoBonificacao = totalRegistros > 0 ? repositorioApuracaoBonificacao.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao>();

                var listaApuracaoBonificacaoRetornar = (
                    from apuracaoBonificacao in listaApuracaoBonificacao
                    select new
                    {
                        apuracaoBonificacao.Codigo,
                        apuracaoBonificacao.Numero,
                        Situacao = apuracaoBonificacao.Situacao.ObterDescricao(),
                        apuracaoBonificacao.Ano,
                        Mes = apuracaoBonificacao.Mes.ObterDescricao()
                    }
                ).ToList();

                grid.AdicionaRows(listaApuracaoBonificacaoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private Models.Grid.Grid ObterGridFechamentoApuracao()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao))
            {
                int codigoApuracaoBonificacao = Request.GetIntParam("Codigo");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request)
                {
                    header = new List<Models.Grid.Head>()
                };

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoPercentual", false);
                grid.AdicionarCabecalho("Transportador", "Transportador", 40, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Valor Faturamento Frete e Ocorrência", "ValorFaturamentoFreteOcorrencia", 20, Models.Grid.Align.right, false);
                grid.AdicionarCabecalho("% Apuração PGT", "ApuracaoPGT", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Valor Ocorrência PGT", "ValorOcorrenciaPGT", 15, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Ocorrência", "Ocorrencia", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Situação Ocorrência", "SituacaoOcorrencia", 10, Models.Grid.Align.left, false);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                DefinirPropriedadeOrdenar(parametrosConsulta);
                Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento repositorioApuracaoBonificacaoFechamento = new Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento(unitOfWork);
                int totalRegistros = repositorioApuracaoBonificacaoFechamento.ContarConsulta(codigoApuracaoBonificacao);
                List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento> listaApuracaoBonificacaoFechamento = totalRegistros > 0 ? repositorioApuracaoBonificacaoFechamento.Consultar(codigoApuracaoBonificacao, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento>();

                var listaApuracaoBonificacaoFechamentoRetornar = (
                    from apuracaoBonificacaoFechamento in listaApuracaoBonificacaoFechamento
                    select new
                    {
                        apuracaoBonificacaoFechamento.Codigo,
                        Transportador = apuracaoBonificacaoFechamento.Empresa.NomeCNPJ,
                        ValorFaturamentoFreteOcorrencia = apuracaoBonificacaoFechamento.ValorFaturamento.ToString("n2"),
                        ApuracaoPGT = apuracaoBonificacaoFechamento.PercentualApuracao.ToString("n2"),
                        ValorOcorrenciaPGT = apuracaoBonificacaoFechamento.ValorOcorrencia.ToString("n2"),
                        Ocorrencia = apuracaoBonificacaoFechamento.Ocorrencia?.NumeroOcorrencia,
                        SituacaoOcorrencia = apuracaoBonificacaoFechamento.Ocorrencia?.SituacaoOcorrencia.ObterDescricao(),
                        TipoPercentual = apuracaoBonificacaoFechamento.RegraApuracao.Tipo
                    }
                ).ToList();

                grid.AdicionaRows(listaApuracaoBonificacaoFechamentoRetornar);
                grid.setarQuantidadeTotal(totalRegistros);

                return grid;
            }
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> ObterListaApuracaoFechamentoValorFretEValorOcorrencia(Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ApuracaoBonificacao repositorioApuracaoBonificacao = new Repositorio.Embarcador.Frete.ApuracaoBonificacao(unitOfWork);

            IList<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorFrete = repositorioApuracaoBonificacao.BuscarFechamentoValorFrete(apuracaoBonificacao);
            IList<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorOcorrencia = repositorioApuracaoBonificacao.BuscarFechamentoValorOcorrencia(apuracaoBonificacao);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorFretEValorOcorrencia = new List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao>();
            apuracaoFechamentoValorFretEValorOcorrencia = apuracaoFechamentoValorFretEValorOcorrencia.Concat(apuracaoFechamentoValorFrete).ToList();
            apuracaoFechamentoValorFretEValorOcorrencia = apuracaoFechamentoValorFretEValorOcorrencia.Concat(apuracaoFechamentoValorOcorrencia).ToList();

            return apuracaoFechamentoValorFretEValorOcorrencia;
        }

        private Dominio.Entidades.Embarcador.Cargas.Carga ObterCargaMaiorValor(List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorFretEValorOcorrencia, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            int codigoCarga = apuracaoFechamentoValorFretEValorOcorrencia.OrderByDescending(o => o.Valor).Select(o => o.CodigoCarga).First();

            return repositorioCarga.BuscarPorCodigo(codigoCarga);
        }

        private void ProcessarFechamento(Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacao apuracaoBonificacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento repositorioApuracaoBonificacaoFechamento = new Repositorio.Embarcador.Frete.ApuracaoBonificacaoFechamento(unitOfWork);

            List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorFretEValorOcorrencia = ObterListaApuracaoFechamentoValorFretEValorOcorrencia(apuracaoBonificacao, unitOfWork);

            foreach (Dominio.Entidades.Embarcador.Frete.BonificacaoTransportador regra in apuracaoBonificacao.RegrasApuracao)
            {
                List<Dominio.ObjetosDeValor.Embarcador.Frete.ConsultaApuracaoBonificacao> apuracaoFechamentoValorFretEValorOcorrenciaCopia = apuracaoFechamentoValorFretEValorOcorrencia;

                apuracaoFechamentoValorFretEValorOcorrenciaCopia = apuracaoFechamentoValorFretEValorOcorrenciaCopia.Where(o => o.CodigoTransportador == regra.Empresa.Codigo
                                                                                       || regra.FiliasTransportador.Select(x => x.Codigo).Contains(o.CodigoTransportador)).ToList();

                if (apuracaoFechamentoValorFretEValorOcorrenciaCopia.Count == 0) continue;

                Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento apuracaoBonificacaoFechamento = new Dominio.Entidades.Embarcador.Frete.ApuracaoBonificacaoFechamento()
                {
                    Empresa = regra.Empresa,
                    ValorFaturamento = apuracaoFechamentoValorFretEValorOcorrenciaCopia.Sum(o => o.Valor),
                    PercentualApuracao = regra.Percentual,
                    ApuracaoBonificacao = apuracaoBonificacao,
                    CargaMaiorValor = ObterCargaMaiorValor(apuracaoFechamentoValorFretEValorOcorrenciaCopia, unitOfWork),
                    RegraApuracao = regra
                };

                apuracaoBonificacaoFechamento.ValorOcorrencia = apuracaoBonificacaoFechamento.ValorFaturamento * (regra.PercentualAplicar / 100);

                repositorioApuracaoBonificacaoFechamento.Inserir(apuracaoBonificacaoFechamento);
            }

        }

        #endregion
    }
}
