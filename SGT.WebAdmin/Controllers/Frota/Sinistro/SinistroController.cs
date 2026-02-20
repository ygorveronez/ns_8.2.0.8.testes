using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Frota.Sinistro
{
    [CustomAuthorize("Frota/Sinistro")]
    public class SinistroController : BaseController
    {
		#region Construtores

		public SinistroController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Públicos

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

                Models.Grid.Grid grid = ObterGridPesquisa();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro filtrosPesquisa = ObterFiltrosPesquisa();

                int totalRegistros = repositorioSinistro.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Frota.SinistroDados> sinistros = totalRegistros > 0 ? repositorioSinistro.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Frota.SinistroDados>();

                var listaRegistros = (from obj in sinistros
                                      select new
                                      {
                                          obj.Codigo,
                                          obj.Numero,
                                          obj.NumeroBoletimOcorrencia,
                                          DataSinistro = obj.DataSinistro.ToDateTimeString(),
                                          Veiculo = obj.Veiculo.Placa_Formatada,
                                          VeiculoReboque = obj.VeiculoReboque?.Placa_Formatada ?? "",
                                          Cidade = obj.Cidade.DescricaoCidadeEstado,
                                          Motorista = obj.Motorista.Nome,
                                          Etapa = obj.Etapa.ObterDescricao(),
                                          Situacao = obj.Situacao.ObterDescricao(),
                                          Frota = obj.Veiculo.NumeroFrota,
                                          TipoSinistro = obj.TipoSinistro?.Descricao ?? "",
                                          GravidadeSinistro = obj.GravidadeSinistro?.Descricao ?? ""
                                      });

                grid.AdicionaRows(listaRegistros);
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

        public async Task<IActionResult> SalvarNumeroBoletimOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);

                Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork).BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                servicoSinistroDados.SalvarNumeroBoletimOcorrencia(Request.GetStringParam("NumeroBoletimOcorrencia"), dadosSinistro);

                return new JsonpResult(true, true, "Número do boletim de ocorrência salvo com sucesso.");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o número do boletim de ocorrência.");
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
                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos repositorioSinistroEnvolvidos = new Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroParcela repositorioSinistroParcela = new Repositorio.Embarcador.Frota.SinistroParcela(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroHistorico repositorioSinistroHistorico = new Repositorio.Embarcador.Frota.SinistroHistorico(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroNota repositorioSinistroNota = new Repositorio.Embarcador.Frota.SinistroNota(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroOrdemServico repositorioSinistroOrdemServico = new Repositorio.Embarcador.Frota.SinistroOrdemServico(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = repositorioSinistroDados.BuscarPorCodigo(Request.GetIntParam("Codigo"), true);

                if (dadosSinistro == null)
                    return new JsonpResult(false, true, "O registro não foi encontrado");

                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados> repositorioAnexo = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados>(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroIndicacaoPagadorAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados> repositorioAnexoIndicacaoPagador = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroIndicacaoPagadorAnexo, Dominio.Entidades.Embarcador.Frota.SinistroDados>(unitOfWork);
                Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroHistorico> repositorioAnexoHistorico = new Repositorio.Embarcador.Anexo.Anexo<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo, Dominio.Entidades.Embarcador.Frota.SinistroHistorico>(unitOfWork);

                List<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoAnexo> anexos = repositorioAnexo.BuscarPorEntidade(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroIndicacaoPagadorAnexo> anexosIndicacaoPagador = repositorioAnexoIndicacaoPagador.BuscarPorEntidade(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos> envolvidos = repositorioSinistroEnvolvidos.BuscarPorFluxoSinistro(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> parcelas = repositorioSinistroParcela.BuscarPorFluxoSinistro(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroNota> notas = repositorioSinistroNota.BuscarPorFluxoSinistro(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroOrdemServico> ordensServico = repositorioSinistroOrdemServico.BuscarPorFluxoSinistro(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroHistorico> historicos = repositorioSinistroHistorico.BuscarPorFluxoSinistro(dadosSinistro.Codigo);
                List<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo> anexosHistorico = historicos.Count > 0 ? repositorioAnexoHistorico.BuscarPorEntidades(historicos.Select(x => x.Codigo).ToList()) : new List<Dominio.Entidades.Embarcador.Frota.SinistroHistoricoAnexo>();

                return new JsonpResult(new
                {
                    dadosSinistro.Etapa,
                    dadosSinistro.Situacao,
                    DadosSinistro = new
                    {
                        dadosSinistro.Codigo,
                        dadosSinistro.Etapa,
                        dadosSinistro.Situacao,
                        dadosSinistro.Numero,
                        dadosSinistro.NumeroBoletimOcorrencia,
                        dadosSinistro.CausadorSinistro,
                        dadosSinistro.Local,
                        dadosSinistro.Endereco,
                        dadosSinistro.Observacao,
                        DataSinistro = dadosSinistro.DataSinistro.ToDateTimeString(),
                        DataEmissao = dadosSinistro.DataEmissao?.ToDateString() ?? string.Empty,
                        Cidade = new { dadosSinistro.Cidade.Codigo, dadosSinistro.Cidade.Descricao },
                        Veiculo = new { dadosSinistro.Veiculo.Codigo, dadosSinistro.Veiculo.Descricao },
                        VeiculoReboque = new { dadosSinistro.VeiculoReboque?.Codigo, dadosSinistro.VeiculoReboque?.Descricao },
                        Motorista = new { dadosSinistro.Motorista.Codigo, dadosSinistro.Motorista.Descricao },
                        TipoSinistro = new { dadosSinistro.TipoSinistro?.Codigo, dadosSinistro.TipoSinistro?.Descricao },
                        PossuiReboque = dadosSinistro.VeiculoReboque != null,
                        GravidadeSinistro = new { dadosSinistro.GravidadeSinistro?.Codigo, dadosSinistro.GravidadeSinistro?.Descricao }
                    },
                    Documentacao = new
                    {
                        Envolvidos = (from obj in envolvidos
                                      select new
                                      {
                                          obj.Codigo,
                                          TipoDescricao = obj.TipoEnvolvido.ObterDescricao(),
                                          Tipo = obj.TipoEnvolvido,
                                          obj.Nome,
                                          obj.CPF,
                                          obj.RG,
                                          obj.CNH,
                                          obj.TelefonePrincipal,
                                          obj.TelefoneSecundario,
                                          obj.Veiculo,
                                          obj.Observacao
                                      }).ToList(),
                        Anexos = (from obj in anexos
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Descricao,
                                      obj.NomeArquivo
                                  }).ToList()
                    },
                    Manutencao = new
                    {
                        Sinistro = dadosSinistro.Codigo,
                        DataProgramada = dadosSinistro.DataProgramada?.ToDateString() ?? string.Empty,
                        dadosSinistro.ObservacaoOS,
                        TipoOrdemServico = new { Codigo = dadosSinistro.TipoOrdemServico?.Codigo ?? 0, Descricao = dadosSinistro.TipoOrdemServico?.Descricao ?? string.Empty },
                        LocalManutencao = new { Codigo = dadosSinistro.LocalManutencao?.Codigo ?? 0, Descricao = dadosSinistro.LocalManutencao?.Descricao ?? string.Empty },

                        CodigoOrdemServico = dadosSinistro.OrdemServico?.Codigo ?? 0,
                        NumeroOrdemServico = dadosSinistro.OrdemServico?.Numero ?? 0,
                        SituacaoOrdemServico = dadosSinistro.OrdemServico?.DescricaoSituacao ?? string.Empty,
                        DataProgramacaoOrdemServico = dadosSinistro.OrdemServico?.DataProgramada.ToDateString() ?? string.Empty,
                        InformacaoTipoOrdemServico = dadosSinistro.OrdemServico?.TipoOrdemServico?.Descricao ?? string.Empty,
                        LocalOrdemServico = dadosSinistro.OrdemServico?.LocalManutencao?.Nome ?? string.Empty,

                        OrdensServico = (from obj in ordensServico
                                         select new
                                         {
                                             obj.OrdemServico.Codigo,
                                             obj.OrdemServico.Numero,
                                             DataProgramada = obj.OrdemServico.DataProgramada.ToString("dd/MM/yyyy"),
                                             Veiculo = obj.OrdemServico.Veiculo?.Placa ?? string.Empty,
                                             Equipamento = obj.OrdemServico.Equipamento?.Descricao ?? string.Empty,
                                             NumeroFrota = obj.OrdemServico.Veiculo?.NumeroFrota ?? string.Empty,
                                             Motorista = obj.OrdemServico.Motorista?.Nome ?? string.Empty,
                                             LocalManutencao = obj.OrdemServico.LocalManutencao?.Nome ?? string.Empty,
                                             Operador = obj.OrdemServico.Operador.Nome,
                                             TipoManutencao = obj.OrdemServico.DescricaoTipoManutencao,
                                             Situacao = obj.OrdemServico.DescricaoSituacao,
                                             ValorOS = obj.OrdemServico.Orcamento?.ValorTotalOrcado.ToString("n2") ?? 0.ToString("n2"),
                                         }).ToList()
                    },
                    IndicacaoPagador = new
                    {
                        Sinistro = dadosSinistro.Codigo,
                        BloqueiaEdicaoAoVoltarEtapa = dadosSinistro.PossuiTitulo || dadosSinistro.FolhaLancamento != null,
                        dadosSinistro.IndicadorPagador,
                        TipoMovimento = new { Codigo = dadosSinistro.TipoMovimento?.Codigo ?? 0, Descricao = dadosSinistro.TipoMovimento?.Descricao ?? string.Empty },
                        Pessoa = new { Codigo = dadosSinistro.PessoaTitulo?.Codigo ?? 0, Descricao = dadosSinistro.PessoaTitulo?.Descricao ?? string.Empty },
                        DataEmissao = dadosSinistro.DataEmissaoTitulo?.ToDateString() ?? string.Empty,
                        DataVencimento = dadosSinistro.DataVencimentoTitulo?.ToDateString() ?? string.Empty,
                        ValorOriginal = dadosSinistro.ValorOriginalTitulo > 0 ? dadosSinistro.ValorOriginalTitulo.ToString("n2") : string.Empty,
                        dadosSinistro.FormaTitulo,
                        TipoDocumento = dadosSinistro.TipoDocumentoTitulo,
                        NumeroDocumento = dadosSinistro.NumeroDocumentoTitulo,
                        LinhaDigitavel = dadosSinistro.LinhaDigitavelBoleto,
                        CodigoDeBarras = dadosSinistro.NossoNumeroBoleto,
                        Observacao = dadosSinistro.ObservacaoTitulo,
                        Parcelas = (from obj in parcelas
                                    select new
                                    {
                                        obj.Codigo,
                                        Parcela = obj.Sequencia,
                                        Valor = obj.Valor.ToString("n2"),
                                        DataVencimento = obj.DataVencimento.ToDateString()
                                    }).ToList(),
                        Anexos = (from obj in anexosIndicacaoPagador
                                  select new
                                  {
                                      obj.Codigo,
                                      obj.Descricao,
                                      obj.NomeArquivo
                                  }).ToList(),
                        Notas = (from obj in notas
                                 select new
                                 {
                                     obj.DocumentoEntrada.Codigo,
                                     obj.DocumentoEntrada.Numero,
                                     Fornecedor = obj.DocumentoEntrada.Fornecedor.NomeCNPJ,
                                     DataEmissao = obj.DocumentoEntrada.DataEmissao.ToDateTimeString(),
                                     ValorTotal = obj.DocumentoEntrada.ValorTotal.ToString("n2")
                                 }).ToList(),
                        Descricao = dadosSinistro.DescricaoFolhaLancamento,
                        NumeroEvento = dadosSinistro.NumeroEventoFolhaLancamento > 0 ? dadosSinistro.NumeroEventoFolhaLancamento.ToString() : string.Empty,
                        NumeroContrato = dadosSinistro.NumeroContratoFolhaLancamento > 0 ? dadosSinistro.NumeroContratoFolhaLancamento.ToString() : string.Empty,
                        DataInicial = dadosSinistro.DataInicialFolhaLancamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataFinal = dadosSinistro.DataFinalFolhaLancamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        DataCompetencia = dadosSinistro.DataCompetenciaFolhaLancamento?.ToString("dd/MM/yyyy") ?? string.Empty,
                        Base = dadosSinistro.BaseFolhaLancamento > 0 ? dadosSinistro.BaseFolhaLancamento.ToString("n2") : string.Empty,
                        Referencia = dadosSinistro.ReferenciaFolhaLancamento > 0 ? dadosSinistro.ReferenciaFolhaLancamento.ToString("n2") : string.Empty,
                        Valor = dadosSinistro.ValorFolhaLancamento > 0 ? dadosSinistro.ValorFolhaLancamento.ToString("n2") : string.Empty,
                        Funcionario = dadosSinistro.FuncionarioFolhaLancamento != null ? new { dadosSinistro.FuncionarioFolhaLancamento.Codigo, dadosSinistro.FuncionarioFolhaLancamento.Descricao } : null,
                        FolhaInformacao = dadosSinistro.FolhaInformacao != null ? new { dadosSinistro.FolhaInformacao.Codigo, dadosSinistro.FolhaInformacao.Descricao } : null
                    },
                    Acompanhamento = new
                    {
                        Historicos = (from historico in historicos
                                      select new
                                      {
                                          historico.Codigo,
                                          Data = historico.Data.ToString("dd/MM/yyyy HH:mm"),
                                          historico.Tipo,
                                          historico.Observacao,
                                          TipoDescricao = historico.Tipo.ObterDescricao()
                                      }).ToList(),
                        Anexos = (from anexo in anexosHistorico
                                  select new
                                  {
                                      anexo.Codigo,
                                      anexo.NomeArquivo,
                                      anexo.Descricao,
                                      CodigoHistorico = anexo.EntidadeAnexo.Codigo
                                  }).ToList()
                    }
                });
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar o registro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> IniciarFluxoSinistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.SinistroDadosDTO dadosSinistroDTO = PreencherDadosSinistro(unitOfWork);
                int codigo = servicoSinistroDados.IniciarFluxo(dadosSinistroDTO);

                return new JsonpResult(new { Codigo = codigo }, true, "Sucesso");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao iniciar o fluxo de sinistro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AtualizarFluxoSinistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Frota.SinistroDadosDTO dadosSinistroDTO = PreencherDadosSinistro(unitOfWork);
                servicoSinistroDados.AtualizarFluxo(dadosSinistroDTO);

                return new JsonpResult(new { dadosSinistroDTO.Codigo }, true, "Sucesso");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar o fluxo de sinistro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AvancarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = repositorioSinistroDados.BuscarPorCodigo(Request.GetIntParam("Codigo"), false);
                EtapaSinistro etapa = Request.GetEnumParam<EtapaSinistro>("Etapa");

                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);
                servicoSinistroDados.AvancarEtapa(dadosSinistro, etapa);

                return new JsonpResult(new { dadosSinistro.Codigo }, true, "Sucesso");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao avançar o fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> VoltarEtapa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = repositorioSinistroDados.BuscarPorCodigo(Request.GetIntParam("Codigo"), false);
                EtapaSinistro etapa = Request.GetEnumParam<EtapaSinistro>("Etapa");

                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);
                servicoSinistroDados.VoltarEtapa(dadosSinistro, etapa);

                return new JsonpResult(new { dadosSinistro.Codigo }, true, "Sucesso");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao voltar etapa do fluxo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> SalvarEnvolvido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Frota.SinistroDados servicoSinistroDados = new Servicos.Embarcador.Frota.SinistroDados(Auditado, unitOfWork);
                Repositorio.Embarcador.Frota.SinistroDados repositorioSinistroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

                Dominio.Entidades.Embarcador.Frota.SinistroDados dadosSinistro = repositorioSinistroDados.BuscarPorCodigo(Request.GetIntParam("CodigoSinistro"), false);
                Dominio.ObjetosDeValor.Embarcador.Frota.SinistroEnvolvidoDTO envolvidoDTO = PreencherDadosEnvolvido();

                Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos envolvido = servicoSinistroDados.SalvarEnvolvido(dadosSinistro, envolvidoDTO);

                return new JsonpResult(new { CodigoEnvolvido = envolvido.Codigo }, true, "Envolvido salvo com sucesso");
            }
            catch (ServicoException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar o envolvido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ExcluirEnvolvido()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoEnvolvido = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos repositorioEnvolvido = new Repositorio.Embarcador.Frota.SinistroDocumentacaoEnvolvidos(unitOfWork);
                Dominio.Entidades.Embarcador.Frota.SinistroDocumentacaoEnvolvidos envolvido = repositorioEnvolvido.BuscarPorCodigo(codigoEnvolvido, false);

                if (envolvido == null)
                    return new JsonpResult(false, true, "O registro não foi encontrado");

                repositorioEnvolvido.Deletar(envolvido);

                return new JsonpResult(true, true, "Envolvido excluido com sucesso");
            }
            catch (Exception excecao)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao excluir o envolvido.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> CancelarSinistro()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Frota.SinistroParcela repSinistroParcela = new Repositorio.Embarcador.Frota.SinistroParcela(unitOfWork);
                Repositorio.Embarcador.Frota.SinistroDados repSinitroDados = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                int codigoSinistro = Request.GetIntParam("Codigo");
                Dominio.Entidades.Embarcador.Frota.SinistroDados sinistro = repSinitroDados.BuscarPorCodigo(codigoSinistro);
                List<Dominio.Entidades.Embarcador.Frota.SinistroParcela> sinistroParcelas = repSinistroParcela.BuscarPorFluxoSinistro(codigoSinistro);

                if (sinistro == null)
                    return new JsonpResult(false, true, "Sinistro não encontrado!");

                if (sinistroParcelas.Count > 0)
                {
                    bool ExisteTituloQuitado = false;
                    List<int> codigosSinistroParcela = new List<int>();

                    foreach (Dominio.Entidades.Embarcador.Frota.SinistroParcela sinistroParcela in sinistroParcelas)
                    {
                        ExisteTituloQuitado = repTitulo.BuscarTitulosQuitadosPorSinistroParcela(sinistroParcela.Codigo);
                        if (ExisteTituloQuitado)
                            return new JsonpResult(false, true, "Existem parcelas em aberto para o Sinistro! Reverta a baixa dos títulos e tente novamente!");

                        codigosSinistroParcela.Add(sinistroParcela.Codigo);
                    }

                    Servicos.Embarcador.Financeiro.Titulo servicoTitulo = new Servicos.Embarcador.Financeiro.Titulo(unitOfWork, TipoServicoMultisoftware, Auditado);
                    List<Dominio.Entidades.Embarcador.Financeiro.Titulo> titulos = repTitulo.BuscarTitulosPorSinistroParcela(codigosSinistroParcela);

                    foreach (var titulo in titulos)
                    {
                        unitOfWork.Start();
                        servicoTitulo.CancelarTitulo(titulo.Codigo);
                        unitOfWork.CommitChanges();
                    }
                }

                sinistro.Situacao = SituacaoEtapaFluxoSinistro.Cancelado;
                repSinitroDados.Atualizar(sinistro, Auditado, null, "Sinistro cancelado pelo usuário!");

                return new JsonpResult(true);
            }
            catch (ServicoException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (ControllerException ex)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, ex.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao cancelar títulos para cancelar o Sinistro.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaSinistro()
            {
                DataSinistroInicial = Request.GetDateTimeParam("DataSinistroInicial"),
                DataSinistroFinal = Request.GetDateTimeParam("DataSinistroFinal"),
                Numero = Request.GetIntParam("Numero"),
                NumeroBoletimOcorrencia = Request.GetStringParam("NumeroBoletimOcorrencia"),
                CodigoCidade = Request.GetIntParam("Cidade"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoVeiculoReboque = Request.GetIntParam("VeiculoReboque"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                Situacao = Request.GetNullableEnumParam<SituacaoEtapaFluxoSinistro>("Situacao"),
                Etapa = Request.GetNullableEnumParam<EtapaSinistro>("Etapa"),
                CodigoTipoSinistro = Request.GetIntParam("TipoSinistro"),
                CodigoGravidadeSinistro = Request.GetIntParam("GravidadeSinistro")
            };
        }

        private Models.Grid.Grid ObterGridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número", "Numero", 5, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Número B.O.", "NumeroBoletimOcorrencia", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Data Sinistro", "DataSinistro", 10, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Tração", "Veiculo", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Reboque", "VeiculoReboque", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Frota", "Frota", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Cidade", "Cidade", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Etapa", "Etapa", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Situação", "Situacao", 5, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Tipo Sinistro", "TipoSinistro", 7, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho("Gravidade Sinistro", "GravidadeSinistro", 7, Models.Grid.Align.center, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.SinistroEnvolvidoDTO PreencherDadosEnvolvido()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.SinistroEnvolvidoDTO()
            {
                Codigo = Request.GetIntParam("Codigo"),
                Nome = Request.GetStringParam("Nome"),
                CPF = Request.GetStringParam("CPF"),
                RG = Request.GetStringParam("RG"),
                CNH = Request.GetStringParam("CNH"),
                TelefonePrincipal = Request.GetStringParam("TelefonePrincipal"),
                TelefoneSecundario = Request.GetStringParam("TelefoneSecundario"),
                Veiculo = Request.GetStringParam("Veiculo"),
                Tipo = Request.GetEnumParam<TipoEnvolvidoSinistro>("Tipo"),
                Observacao = Request.GetStringParam("Observacao")
            };
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.SinistroDadosDTO PreencherDadosSinistro(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Frota.SinistroDados repositorioSinistro = new Repositorio.Embarcador.Frota.SinistroDados(unitOfWork);

            int codigo = Request.GetIntParam("Codigo");

            return new Dominio.ObjetosDeValor.Embarcador.Frota.SinistroDadosDTO()
            {
                Codigo = codigo,
                Numero = codigo == 0 ? repositorioSinistro.BuscarProximoNumero() : 0,
                CausadorSinistro = Request.GetEnumParam<CausadorSinistro>("CausadorSinistro"),
                CodigoCidade = Request.GetIntParam("Cidade"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoVeiculoReboque = Request.GetIntParam("VeiculoReboque"),
                DataEmissao = Request.GetNullableDateTimeParam("DataEmissao"),
                DataSinistro = Request.GetDateTimeParam("DataSinistro"),
                Endereco = Request.GetStringParam("Endereco"),
                Local = Request.GetStringParam("Local"),
                Observacao = Request.GetStringParam("Observacao"),
                NumeroBoletimOcorrencia = Request.GetStringParam("NumeroBoletimOcorrencia"),
                CodigoTipoSinistro = Request.GetIntParam("TipoSinistro"),
                CodigoGravidadeSinistro = Request.GetIntParam("GravidadeSinistro")
            };
        }

        #endregion Métodos Privados
    }
}
