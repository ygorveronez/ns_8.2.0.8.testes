using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DocumentFormat.OpenXml.InkML;

namespace SGT.WebAdmin.Controllers.Frota
{
    [CustomAuthorize("Frota/SugestaoMensal")]
    public class SugestaoMensalController : BaseController
    {
		#region Construtores

		public SugestaoMensalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                var rows = ObterDadosGridPesquisa(unidadeTrabalho, grid.inicio, grid.limite);
                //Repositorio.Embarcador.Frota.ServicoVeiculoFrota repServico = new Repositorio.Embarcador.Frota.ServicoVeiculoFrota(unidadeTrabalho);
                grid.setarQuantidadeTotal(rows.Item2);
                grid.AdicionaRows(rows.Item1);
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

        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = ObterGridPesquisa();
                var rows = ObterDadosGridPesquisa(unitOfWork, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(rows.Item2);
                grid.AdicionaRows(rows.Item1);
                // Gera excel
                byte[] bArquivo = grid.GerarExcel();
                if (bArquivo != null)
                    return Arquivo(bArquivo, "application/octet-stream", grid.tituloExportacao + "." + grid.extensaoCSV);
                else
                    return new JsonpResult(false, "Ocorreu uma falha ao gerar arquivo.");
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

        public async Task<IActionResult> ObterVeiculos()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtros = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = Request.GetIntParam("CodigoFilial"),
                    CodigoTransportador = EhEmbarcador() ? Request.GetIntParam("Transportador") : Empresa.Codigo,
                    CodigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga"),
                    CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamentoFrotaMes")
                };

                var repoPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unidadeTrabalho);
                var lista = repoPlanejamentoFrotaVeiculos.ObterPlanejamentosDoMes(filtros);

                if (!lista.Any())
                    return new JsonpResult(new List<dynamic>());

                Func<Dominio.Entidades.Veiculo, string> obterTesteDeFrio = veiculo =>
                {
                    if (veiculo.LicencasVeiculo.IsNullOrEmpty())
                    {
                        return "Sem cadastro";
                    }
                    else
                    {
                        var testeDeFrio = veiculo.LicencasVeiculo.OrderByDescending(x => x.DataVencimento).FirstOrDefault();
                        if (testeDeFrio.DataVencimento.IsNullOrMinValue())
                            return veiculo.LicencasVeiculo.OrderByDescending(x => x.Codigo).Select(x => x.Status).FirstOrDefault().ObterDescricao();
                        else
                            return testeDeFrio.Status.ObterDescricao();
                    }
                };
                var result = lista.Select(x => new
                {
                    Codigo = x.Codigo,
                    Placa = x.PlacaVeiculo,
                    EnumSituacao = (int)x.Situacao,
                    ModeloVeicular = x.ModeloVeicular.Descricao,
                    Capacidade = x.Veiculo.CapacidadeKG.ToString(),
                    TesteFrio = obterTesteDeFrio(x.Veiculo),
                    ObservacaoTransp = x.ObservacaoTransportador,
                    ObservacaoMarfrig = x.Observacao,
                    CodigoVeiculo = x.Veiculo.Codigo,
                    RespostaTransportador = x.RespostaDoTransportador.ObterDescricao(),
                    Rejeitado = x.RespostaDoTransportador == RespostaTransportadorPlanejamentoFrota.Rejeitado,
                    Filial = x.PlanejamentoFrotaMes.Filial.Descricao,
                    Transportador = x.Veiculo.Empresa.RazaoSocial,
                    DT_FontColor = x.RespostaDoTransportador.ObterCorFonte(),
                    DT_RowColor = x.RespostaDoTransportador.ObterCorLinha(),
                    CNPJTransportador = x.Veiculo.Empresa.CNPJ_Formatado,
                    ListaDiariaGerada = x.Situacao == SituacaoPlanejamentoFrota.ListaDiariaGerada ? "SIM" : "NÃO"
                }).ToList();

                return new JsonpResult(result);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Erro ao processar a requisição. Por favor, tente novamente.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> AdicionarListaVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Servicos.Embarcador.Frotas.PlanejamentoFrotaMes servicoSugestaoMensal = new Servicos.Embarcador.Frotas.PlanejamentoFrotaMes(unitOfWork, Auditado);
                Dominio.ObjetosDeValor.Embarcador.Frotas.PlanejamentoFrotaMesAdicionar planejamentoFrotaMesAdicionar = new Dominio.ObjetosDeValor.Embarcador.Frotas.PlanejamentoFrotaMesAdicionar()
                {
                    Ano = Request.GetIntParam("Ano"),
                    Mes = Request.GetIntParam("Mes"),
                    CodigoFilial = Request.GetIntParam("Filial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicularCarga"),
                    CodigosVeiculos = Request.GetListParam<int>("Veiculos")
                };
                List<(string Placa, string MensagemRetorno, bool Sucesso)> retornos = servicoSugestaoMensal.AdicionarSugestaoFrota(planejamentoFrotaMesAdicionar, TipoServicoMultisoftware);

                unitOfWork.CommitChanges();

                return new JsonpResult((
                    from retorno in retornos
                    select new
                    {
                        Placa = retorno.Placa,
                        MensagemRetorno = retorno.MensagemRetorno,
                        DT_RowColor = retorno.Sucesso ? "#99ff99" : "#ffb3b3"
                    }
                ).ToList());
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar os veículos");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarListaDeVeiculos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtros = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = Request.GetIntParam("CodigoFilial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga"),
                    CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamentoFrotaMes")
                };

                if (filtros.CodigoPlanejamentoFrotaMes == 0)
                    return new JsonpResult(false, "Selecione um Planejamento de Frota Mensal.");

                if (filtros.CodigoModeloVeicularCarga == 0)
                    return new JsonpResult(false, "Modelo Veicular não especificado.");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
                Repositorio.Veiculo repositorioVeiculo = new Repositorio.Veiculo(unitOfWork);
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes repositorioPlanejamentoFrota = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes planejamento = repositorioPlanejamentoFrota.BuscarPorCodigo(filtros.CodigoPlanejamentoFrotaMes, false);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosExistentes = repositorioPlanejamentoFrotaVeiculos.ObterPlanejamentosDoMes(filtros);
                bool ehTransportador = !EhEmbarcador();
                List<dynamic> veiculos = Request.GetListParam<dynamic>("Veiculos");
                List<string> placasVeiculos = veiculos.Select(veiculo => (string)veiculo.Placa).ToList();

                List<Dominio.Entidades.Empresa> empresaJaGeradas = veiculosExistentes.Where(x => x.Situacao == SituacaoPlanejamentoFrota.ListaDiariaGerada).Select(x => x.Veiculo.Empresa).Distinct().ToList();

                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosExistentesPorTransportador = ehTransportador ? veiculosExistentes.Where(planejamentoVeiculo => planejamentoVeiculo.Veiculo.Empresa.Codigo == Empresa.Codigo).ToList() : veiculosExistentes.ToList();
                HashSet<int> transportadorasDosVeiculosExcluidos = new HashSet<int>();

                if (veiculos.IsNullOrEmpty()) //se enviou a lista vazia, quer dizer que está tentando exluir a lista inteira
                {
                    List<string> naoPuderamSerExcluidos = new List<string>();

                    if (ehTransportador)
                    {
                        naoPuderamSerExcluidos = veiculosExistentesPorTransportador.Where(x => !x.TransportadorPodeExcluir()).Select(x => x.PlacaVeiculo).ToList();
                        List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosQuePodemSerExcluidos = veiculosExistentesPorTransportador.Where(x => x.TransportadorPodeExcluir()).ToList();

                        foreach (var item in veiculosQuePodemSerExcluidos)
                        {
                            repositorioPlanejamentoFrotaVeiculos.Deletar(item);
                            Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: item.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: item.DescricaoPorModelo, descricaoAcao: $"Veículo {item.PlacaVeiculo} removido", unitOfWork);
                        }
                    }
                    else
                    {
                        naoPuderamSerExcluidos = veiculosExistentesPorTransportador.Where(x => !x.EmbarcadorPodeExcluir()).Select(x => x.PlacaVeiculo).ToList();
                        List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosQuePodemSerExcluidos = veiculosExistentesPorTransportador.Where(x => x.EmbarcadorPodeExcluir()).ToList();

                        foreach (var item in veiculosQuePodemSerExcluidos)
                        {
                            repositorioPlanejamentoFrotaVeiculos.Deletar(item);
                            transportadorasDosVeiculosExcluidos.Add(item.Veiculo.Empresa.Codigo);
                            Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: item.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: item.DescricaoPorModelo, descricaoAcao: $"Veículo {item.PlacaVeiculo} removido", unitOfWork);
                        }
                    }

                    unitOfWork.CommitChanges();

                    unitOfWork.Start();
                    RetornarListaParaTransportador(unitOfWork, planejamento, transportadorasDosVeiculosExcluidos.ToList());
                    unitOfWork.CommitChanges();

                    if (naoPuderamSerExcluidos.Any())
                        return new JsonpResult(false, "Os veículos " + string.Join(", ", naoPuderamSerExcluidos) + " não podem ser excluídos.");
                    else
                        return new JsonpResult(true);
                }

                List<string> mensagensDeErro = new List<string>();
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosParaExcluir = veiculosExistentesPorTransportador.Where(x => !placasVeiculos.Contains(x.PlacaVeiculo)).ToList();

                foreach (var v in veiculosParaExcluir)
                {
                    if (ehTransportador)
                    {
                        if (v.TransportadorPodeExcluir())
                        {
                            repositorioPlanejamentoFrotaVeiculos.Deletar(v);
                            Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: v.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: v.DescricaoPorModelo, descricaoAcao: $"Veículo {v.PlacaVeiculo} removido", unitOfWork);
                        }
                        else
                            mensagensDeErro.Add($"O status atual não permite alteração do veículo {v.PlacaVeiculo}: {v.Situacao.ObterDescricao()}");
                    }
                    else
                    {
                        repositorioPlanejamentoFrotaVeiculos.Deletar(v);
                        transportadorasDosVeiculosExcluidos.Add(v.Veiculo.Empresa.Codigo);
                        Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: v.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: v.DescricaoPorModelo, descricaoAcao: $"Veículo {v.PlacaVeiculo} removido", unitOfWork);
                    }
                }

                List<string> placasExistentes = veiculosExistentesPorTransportador.Select(x => x.PlacaVeiculo).ToList();
                List<dynamic> veiculosParaInserir = veiculos.Where(veiculo => !placasExistentes.Contains(veiculo.Placa)).AsQueryable().DistinctBy(veiculo => veiculo).ToList();
                List<string> placasExistentesEmOutrasFiliais = repositorioPlanejamentoFrotaVeiculos.VerificarPlacasJaPlanejadasEmOutraFilial(veiculosParaInserir.Select(veiculo => (string)veiculo.Placa).ToList(), filtros.CodigoFilial, planejamento.Data.FirstDayOfMonth());

                if (placasExistentesEmOutrasFiliais.Any())
                    mensagensDeErro.Add($"Os veículos {string.Join(", ", placasExistentesEmOutrasFiliais)} já estão cadastrados em outra filial.");

                foreach (dynamic veiculoParaInserir in veiculosParaInserir)
                {
                    int codigoVeiculo = ((string)veiculoParaInserir.CodigoVeiculo).ToInt();
                    string placaVeiculo = (string)veiculoParaInserir.Placa;

                    if (placasExistentesEmOutrasFiliais.Contains(placaVeiculo))
                        continue;

                    Dominio.Entidades.Veiculo veiculo = repositorioVeiculo.BuscarPorCodigo(codigoVeiculo);

                    if (veiculo.ModeloVeicularCarga.Codigo != filtros.CodigoModeloVeicularCarga)
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, $"O veículo {placaVeiculo} não pôde ser inserido por ser de um Modelo Veicular diferente.");
                    }

                    if(empresaJaGeradas.Contains(veiculo.Empresa))
                        return new JsonpResult(false, $"Transportador({veiculo.Empresa.Descricao}) ja tem lista diaria gerada não podendo adicionar novos veiculos");


                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo planejamentoFrotaMesVeiculoAdicionar = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo()
                    {
                        IncluidoPeloEmbarcador = EhEmbarcador(),
                        IncluidoPeloTransportador = ehTransportador,
                        GeradoPeloSistema = false,
                        Veiculo = veiculo,
                        ModeloVeicular = veiculo.ModeloVeicularCarga,
                        PlanejamentoFrotaMes = planejamento,
                        TipoOperacao = null,
                        Observacao = (string)veiculoParaInserir.ObservacaoMarfrig,
                        PlacaVeiculo = placaVeiculo,
                        Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador,
                        RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Pendente
                    };

                    repositorioPlanejamentoFrotaVeiculos.Inserir(planejamentoFrotaMesVeiculoAdicionar);
                    Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: planejamentoFrotaMesVeiculoAdicionar.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: planejamentoFrotaMesVeiculoAdicionar.DescricaoPorModelo, descricaoAcao: $"Veículo {planejamentoFrotaMesVeiculoAdicionar.PlacaVeiculo} adicionado", unitOfWork);
                }

                unitOfWork.CommitChanges();

                if (mensagensDeErro.Any())
                    return new JsonpResult(false, string.Join("; ", mensagensDeErro));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Erro ao processar a requisição. Por favor, tente novamente.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarVeiculo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                int codigoPlanejamentoFrotaMesVeiculo = Request.GetIntParam("CodigoPlanejamentoFrotaMesVeiculo");
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo veiculo = repositorioPlanejamentoFrotaVeiculos.BuscarPorCodigo(codigoPlanejamentoFrotaMesVeiculo, true);

                if (veiculo == null)
                    throw new ControllerException("Não foi possível encontrar o registro.");

                if (veiculo.Situacao == SituacaoPlanejamentoFrota.ListaDiariaGerada)
                    throw new ControllerException("Não é possível fazer alterações. A lista diária já foi gerada.");

                if (EhEmbarcador())
                {
                    if (veiculo.Situacao != SituacaoPlanejamentoFrota.EmAnaliseEmbarcador)
                        throw new ControllerException("O status atual não permite alteração: " + veiculo.Situacao.ObterDescricao());

                    veiculo.Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador;
                    veiculo.Observacao = Request.GetStringParam("ObservacaoMarfrig");
                }
                else
                {
                    if (veiculo.Situacao == SituacaoPlanejamentoFrota.EmAnaliseTransportador)
                        throw new ControllerException("O status atual não permite alteração: " + veiculo.Situacao.ObterDescricao());

                    veiculo.Situacao = SituacaoPlanejamentoFrota.EmAnaliseEmbarcador;
                    veiculo.ObservacaoTransportador = Request.GetStringParam("ObservacaoTransportador");
                }

                veiculo.RespostaDoTransportador = Request.GetBoolParam("Rejeitar") ? RespostaTransportadorPlanejamentoFrota.Rejeitado : RespostaTransportadorPlanejamentoFrota.Pendente;

                repositorioPlanejamentoFrotaVeiculos.Atualizar(veiculo);
                Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, veiculo.GetChanges(), descricaoAcao: $"Atualizadas as informações do veículo {veiculo.PlacaVeiculo}", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao alterar as informações do veículo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PublicarParaTransportador()
        {
            if (!EhEmbarcador())
                return new JsonpResult(false, false, "A aprovação da lista deve ser feita pelo Embarcador.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtros = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = Request.GetIntParam("CodigoFilial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga"),
                    CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamentoFrotaMes")
                };

                if (filtros.CodigoPlanejamentoFrotaMes == 0)
                    return new JsonpResult(false, "Selecione um Planejamento de Frota Mensal.");

                if (filtros.CodigoModeloVeicularCarga == 0)
                    return new JsonpResult(false, "Modelo Veicular não especificado.");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculos = repositorioPlanejamentoFrotaVeiculos.ObterPlanejamentosDoMes(filtros);

                if (!(veiculos.Any(x => x.Situacao != SituacaoPlanejamentoFrota.ListaDiariaGerada)))
                    return new JsonpResult(false, "Não é possivel fazer alterações. A lista diária já foi criada.");

                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosEmAnaliseEmbarcador = veiculos.Where(planejamentoVeiculo => planejamentoVeiculo.Situacao == SituacaoPlanejamentoFrota.EmAnaliseEmbarcador).ToList();

                foreach (Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo veiculo in veiculosEmAnaliseEmbarcador)
                {
                    veiculo.Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador;

                    if (veiculo.RespostaDoTransportador == RespostaTransportadorPlanejamentoFrota.Confirmado)
                        veiculo.RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Pendente;

                    repositorioPlanejamentoFrotaVeiculos.Atualizar(veiculo);
                    Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, descricaoAcao: $"Veículo {veiculo.PlacaVeiculo} enviado para análise do transportador", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar veículos para análise do transportador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> PublicarParaEmbarcador()
        {
            if (EhEmbarcador())
                return new JsonpResult(false, false, "A aprovação da lista deve ser feita pelo Transportador.");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                unitOfWork.Start();

                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtros = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = Request.GetIntParam("CodigoFilial"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga"),
                    CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamentoFrotaMes")
                };

                if (filtros.CodigoPlanejamentoFrotaMes == 0)
                    return new JsonpResult(false, "Selecione um Planejamento de Frota Mensal.");

                if (filtros.CodigoModeloVeicularCarga == 0)
                    return new JsonpResult(false, "Modelo Veicular não especificado.");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculos = repositorioPlanejamentoFrotaVeiculos.ObterPlanejamentosDoMes(filtros);

                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculosEmAnaliseTransportador = veiculos.Where(planejamentoVeiculo => planejamentoVeiculo.Situacao == SituacaoPlanejamentoFrota.EmAnaliseTransportador && planejamentoVeiculo.Veiculo.Empresa.Codigo == Empresa.Codigo).ToList();

                foreach (var veiculo in veiculosEmAnaliseTransportador)
                {
                    veiculo.Situacao = SituacaoPlanejamentoFrota.EmAnaliseEmbarcador;

                    if (veiculo.RespostaDoTransportador == RespostaTransportadorPlanejamentoFrota.Pendente)
                        veiculo.RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Confirmado;

                    repositorioPlanejamentoFrotaVeiculos.Atualizar(veiculo);
                    Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, descricaoAcao: $"Veículo {veiculo.PlacaVeiculo} enviado para análise do embarcador", unitOfWork);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao enviar veículos para análise do embarcador.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> GerarListaDiaria()
        {
            if (!EhEmbarcador())
                return new JsonpResult(false, false, "");

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtros = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
                {
                    CodigoFilial = Request.GetIntParam("CodigoFilial"),
                    CodigoTransportador = Request.GetIntParam("Transportador"),
                    CodigoModeloVeicularCarga = Request.GetIntParam("CodigoModeloVeicularCarga"),
                    CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamentoFrotaMes"),
                    RespostaTransportador = RespostaTransportadorPlanejamentoFrota.Confirmado
                };

                unitOfWork.Start();

                if (filtros.CodigoPlanejamentoFrotaMes == 0)
                    return new JsonpResult(false, "Selecione um Planejamento de Frota Mensal.");

                if (filtros.CodigoModeloVeicularCarga == 0)
                    return new JsonpResult(false, "Modelo Veicular não especificado.");

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Frota/SugestaoMensal");
                if (!(this.Usuario.UsuarioAdministrador || permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.SugestaoMensal_PermiteGerarListaDiariaApartirListaMensal)))
                    return new JsonpResult(false, "Seu usuário não possui permissão para gerar lista diária.");

                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaMesVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes repositorioPlanejamentoFrotaMes = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMes(unitOfWork);
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo repositorioPlanejamentoFrotaDiaVeiculo = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo(unitOfWork);
                Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia repositorioPlanejamentoFrotaDia = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaDia(unitOfWork);

                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes planejamentoMes = repositorioPlanejamentoFrotaMes.BuscarPorCodigo(filtros.CodigoPlanejamentoFrotaMes, true);
                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> listaPlanejamentosMes = repositorioPlanejamentoFrotaMesVeiculos.ObterPlanejamentosDoMes(filtros);

                List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> listaPlanejamentosMesParaGeracaoLista = listaPlanejamentosMes.Where(x => x.Situacao == SituacaoPlanejamentoFrota.EmAnaliseEmbarcador && x.RespostaDoTransportador == RespostaTransportadorPlanejamentoFrota.Confirmado).ToList();

                if (!listaPlanejamentosMesParaGeracaoLista.Any())
                    throw new ControllerException("Nenhum registro disponível para geração da lista foi encontrado.");

                Repositorio.Embarcador.Frotas.RodizioPlacas repositorioRodizio = new Repositorio.Embarcador.Frotas.RodizioPlacas(unitOfWork);
                Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo primeiroPlanejamentosMesParaGeracao = listaPlanejamentosMesParaGeracaoLista.FirstOrDefault();
                List<Dominio.Entidades.Embarcador.Frotas.RodizioPlacas> configuracoesRodizio = repositorioRodizio.BuscarTodos();
                List<int> codigosVeiculos = listaPlanejamentosMesParaGeracaoLista.Select(x => x.Veiculo.Codigo).Distinct().ToList();
                var ultimosCarregamentos = repositorioPlanejamentoFrotaDiaVeiculo.ObterDataDoUltimoCarregamento(codigosVeiculos);
                DateTime dataInicialGerarListaDiaria = planejamentoMes.Data.FirstDayOfMonth();
                int diasNoMes = DateTime.DaysInMonth(dataInicialGerarListaDiaria.Year, dataInicialGerarListaDiaria.Month);

                for (int dia = 1; dia <= diasNoMes; dia++)
                {
                    DateTime diaDoPlanejamentoDeFrota = new DateTime(dataInicialGerarListaDiaria.Year, dataInicialGerarListaDiaria.Month, dia);

                    if ((diaDoPlanejamentoDeFrota.DayOfWeek == DayOfWeek.Sunday) || (diaDoPlanejamentoDeFrota.DayOfWeek == DayOfWeek.Saturday))
                        continue;

                    DateTime dataAtual = DateTime.Now;

                    if (diaDoPlanejamentoDeFrota.Month < dataAtual.Month || diaDoPlanejamentoDeFrota.Day <= dataAtual.Day)
                        throw new ControllerException("Não é possivel gerar a lista por que não pertence ao mes vigente");

                    Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia planoFrotaDia = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDia()
                    {
                        Data = diaDoPlanejamentoDeFrota,
                        Filial = listaPlanejamentosMesParaGeracaoLista[0].PlanejamentoFrotaMes.Filial,
                        PlanejamentoFrotaMes = listaPlanejamentosMesParaGeracaoLista[0].PlanejamentoFrotaMes
                    };

                    List<Dominio.Entidades.Embarcador.Frotas.RodizioPlacas> rodiziosFilial = configuracoesRodizio.Where(x => x.Filial.Codigo == planoFrotaDia.Filial.Codigo).ToList();
                    bool fazRodizio = rodiziosFilial?.Count > 0;
                    List<int> finaisPlacas = new List<int>();

                    if (fazRodizio)
                    {
                        Dominio.Entidades.Embarcador.Frotas.RodizioPlacas rodizioDoDia = rodiziosFilial.Where(x => ((int)x.DiaSemana - 1) == (int)diaDoPlanejamentoDeFrota.DayOfWeek).FirstOrDefault();

                        if (rodizioDoDia != null)
                            finaisPlacas = rodizioDoDia.ObterFinaisDePlacas();
                    }

                    Func<string, bool> estaEmRodizio = placa =>
                    {
                        if (!fazRodizio)
                            return false;

                        int finalPlaca = placa.Substring(placa.Length - 1).ToInt();

                        return finaisPlacas?.Contains(finalPlaca) ?? false;
                    };

                    repositorioPlanejamentoFrotaDia.Inserir(planoFrotaDia);
                   
                    foreach (var item in listaPlanejamentosMesParaGeracaoLista.AsQueryable().DistinctBy(x => x.PlacaVeiculo))
                    {
                        item.Situacao = SituacaoPlanejamentoFrota.ListaDiariaGerada;

                        repositorioPlanejamentoFrotaMesVeiculos.Atualizar(item, Auditado);

                        if (item.ModeloVeicular == null)
                            continue;

                        Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo planoDiaVeiculo = new Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaDiaVeiculo()
                        {
                            GeradoPeloSistema = item.GeradoPeloSistema,
                            Indisponivel = false,
                            JustificativaIndisponibilidade = null,
                            Rodizio = estaEmRodizio(item.PlacaVeiculo),
                            ModeloVeicular = item.ModeloVeicular,
                            ObservacaoMarfrig = item.Observacao,
                            ObservacaoTransportador = item.ObservacaoTransportador,
                            PlacaVeiculo = item.PlacaVeiculo,
                            Roteirizado = false,
                            UltimoEmbarque = ultimosCarregamentos.FirstOrDefault(x => x.CodigoVeiculo == item.Veiculo.Codigo)?.DataCarregamento,
                            Veiculo = item.Veiculo,
                            PlanejamentoFrotaDia = planoFrotaDia,
                            RotaDeConhecimento = string.Empty
                        };

                        repositorioPlanejamentoFrotaDiaVeiculo.Inserir(planoDiaVeiculo, Auditado);
                    }
                }

                Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: primeiroPlanejamentosMesParaGeracao.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: primeiroPlanejamentosMesParaGeracao.DescricaoPorModelo, descricaoAcao: "Gerada a lista diária", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true, "Listas diárias geradas com Suceso.");
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar as listas diárias.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frotas.FiltroPesquisaPlanejamentoFrotaMesVeiculo()
            {
                Ano = Request.GetIntParam("Ano"),
                CodigoFilial = Request.GetIntParam("Filial"),
                CodigoPlanejamentoFrotaMes = Request.GetIntParam("CodigoPlanejamento"),
                CodigoModeloVeicularCarga = Request.GetIntParam("ModeloVeicular"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador ? Request.GetIntParam("Transportador") : Empresa.Codigo,
                Mes = Request.GetIntParam("Mes"),
                Situacao = Request.GetNullableEnumParam<SituacaoPlanejamentoFrota>("Situacao")
            };

            if (!EhEmbarcador())
                filtrosPesquisa.CodigoTransportador = Empresa.Codigo;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("CodigoPlanejamentoFrotaMes", false);
            grid.AdicionarCabecalho("CodigoModeloVeicular", false);
            grid.AdicionarCabecalho("CodigoPorModelo", false);
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Datetime", false);
            grid.AdicionarCabecalho("EnumSituacao", false);
            grid.AdicionarCabecalho("CodigoFilial", false);
            grid.AdicionarCabecalho("CodigoTransportadora", false);
            grid.AdicionarCabecalho("Transportador", false);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Data", "Data", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Quantidade", "Quantidade", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Obs Marfrig", "Obs", 10, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
            return grid;
        }

        private Tuple<IList, int> ObterDadosGridPesquisa(Repositorio.UnitOfWork unidadeTrabalho, int start, int limit)
        {
            var filtros = ObterFiltrosPesquisa();
            var repoPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unidadeTrabalho);
            var lista = repoPlanejamentoFrotaVeiculos.ObterPlanejamentosDoMes(filtros);
            int codigo = 1;

            Func<IEnumerable<string>, string> obterSituacao = situacoes =>
            {
                var situacoesDistinct = situacoes.Distinct().ToList();
                if (situacoesDistinct.Count == 1)
                    return situacoesDistinct[0];

                return string.Join(" / ", situacoesDistinct);
            };

            if (limit <= 0)
                limit = 9999;

            var rows = lista.GroupBy(x => new
            {
                Modelo = x.ModeloVeicular.Codigo,
                Filial = x.PlanejamentoFrotaMes.Filial.Codigo,
                Data = x.PlanejamentoFrotaMes.Data.Date
            }).Select(x => new
            {
                CodigoPlanejamentoFrotaMes = x.FirstOrDefault()?.PlanejamentoFrotaMes.Codigo,
                CodigoModeloVeicular = x.Key.Modelo,
                CodigoPorModelo = x.FirstOrDefault()?.CodigoPorModelo,
                Codigo = codigo++,
                Datetime = x.Key.Data,
                EnumSituacao = -1,
                CodigoFilial = x.Key.Filial,
                CodigoTransportadora = -1,
                Transportador = x.FirstOrDefault()?.Veiculo?.Empresa?.Codigo ?? 0,
                Filial = x.FirstOrDefault()?.PlanejamentoFrotaMes.Filial.Descricao,
                ModeloVeicular = x.FirstOrDefault()?.ModeloVeicular.Descricao,
                Data = x.Key.Data.ToString("MM/yyyy"),
                Quantidade = x.Count(),
                Obs = string.Join(" ", x.Select(y => y.Observacao)).Trim(),
                Situacao = obterSituacao(x.Select(y => y.Situacao.ObterDescricao()))
            }).OrderByDescending(x => x.Datetime)
                .ThenBy(x => x.Filial)
                .ThenBy(x => x.ModeloVeicular);

            return new Tuple<IList, int>(rows.Skip(start).Take(limit).ToList(), rows.Count());
        }

        private bool EhEmbarcador()
        {
            return TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador;
        }

        private void RetornarListaParaTransportador(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMes planejamento, List<int> transportadores)
        {
            if (planejamento?.Codigo <= 0 || transportadores?.Count == 0)
                return;

            Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo repositorioPlanejamentoFrotaVeiculos = new Repositorio.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo(unitOfWork);
            List<Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo> veiculos = repositorioPlanejamentoFrotaVeiculos.ObterPlanejamentoVeiculosPorTransportador(transportadores, planejamento.Codigo);

            foreach (Dominio.Entidades.Embarcador.Frotas.PlanejamentoFrotaMesVeiculo veiculo in veiculos)
            {
                veiculo.Situacao = SituacaoPlanejamentoFrota.EmAnaliseTransportador;

                if (veiculo.RespostaDoTransportador == RespostaTransportadorPlanejamentoFrota.Confirmado)
                    veiculo.RespostaDoTransportador = RespostaTransportadorPlanejamentoFrota.Pendente;

                repositorioPlanejamentoFrotaVeiculos.Atualizar(veiculo);
                Servicos.Auditoria.Auditoria.AuditarSemEntidade(Auditado, codigoEntidade: veiculo.CodigoPorModelo, nomeEntidade: "PlanejamentoFrotaMesModelo", descricaoEntidade: veiculo.DescricaoPorModelo, descricaoAcao: $"Veículo {veiculo.PlacaVeiculo} enviado para análise do transportador", unitOfWork);
            }
        }

        #endregion Métodos Privados
    }
}

