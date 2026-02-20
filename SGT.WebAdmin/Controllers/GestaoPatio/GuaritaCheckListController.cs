using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Relatorios;
using Servicos.Extensions;

namespace SGT.WebAdmin.Controllers.GestaoPatio
{
    [CustomAuthorize(new string[] { "Imprimir" }, "GestaoPatio/GuaritaCheckList")]
    public class GuaritaCheckListController : BaseController
    {
		#region Construtores

		public GuaritaCheckListController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> AdicionarCheckList()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
                Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServicoFrota = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);

                unitOfWork.Start();

                int.TryParse(Request.Params("Carga"), out int codigoCarga);
                int.TryParse(Request.Params("OrdemServicoFrota"), out int codigoOrdemServicoFrota);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("KMAtual"), out int kmAtual);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida;
                Enum.TryParse(Request.Params("TipoEntradaSaida"), out tipoEntradaSaida);
                //Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCheckListGuarita tipoCheckListGuarita;
                //Enum.TryParse(Request.Params("TipoCheckListGuarita"), out tipoCheckListGuarita);
                int.TryParse(Request.Params("CheckListTipo"), out int codigoCheckListTipo);
                string observacao = Request.Params("Observacao");

                int codigoEmpresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                int codigo = Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarCheckList(null, unitOfWork, kmAtual, tipoEntradaSaida, repCarga.BuscarPorCodigo(codigoCarga), repOrdemServicoFrota.BuscarPorCodigo(codigoOrdemServicoFrota), repVeiculo.BuscarPorCodigo(codigoVeiculo), observacao, codigoCheckListTipo, codigoEmpresa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(new { Codigo = codigo });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
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
                // Instancia repositorios
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

                // Parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Operador"), out int codigoOperador);

                // Busca informacoes
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigo);

                // Valida
                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                guaritaCheckList.Initialize();

                DateTime? data = null;
                if (DateTime.TryParse(Request.Params("Data"), out DateTime dataAux))
                    data = dataAux;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList.Aberto;
                Enum.TryParse(Request.Params("Situacao"), out situacao);

                guaritaCheckList.Data = data;
                guaritaCheckList.Situacao = situacao;
                guaritaCheckList.Operador = repUsuario.BuscarPorCodigo(codigoOperador);

                unitOfWork.Start();
                SalvarCheckList(Request.Params("CheckList"), guaritaCheckList, unitOfWork);
                SalvarCheckList(Request.Params("Croquis"), guaritaCheckList, unitOfWork);
                SalvarManutencao(guaritaCheckList, unitOfWork);
                SalvarAbastecimento(guaritaCheckList, unitOfWork);
                SalvarManutencaoEquipamento(guaritaCheckList, unitOfWork);

                repGuaritaCheckList.Atualizar(guaritaCheckList, Auditado);

                if (guaritaCheckList.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList.Finalizado)
                {
                    if (guaritaCheckList.GerarOS)
                    {
                        InserirOrdemServico(guaritaCheckList, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Gerou Ordem de Serviço nº " + guaritaCheckList.OrdemServicoFrota.Numero.ToString() + " na finalização do check list", unitOfWork);
                    }

                    if (guaritaCheckList.GerarAbastecimento)
                    {
                        string erro = string.Empty;
                        if (!Servicos.Embarcador.GestaoPatio.GuaritaCheckList.GerarAbastecimentos(guaritaCheckList, unitOfWork, TipoServicoMultisoftware, ConfiguracaoEmbarcador, out erro))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Gerou Abastecimento na finalização do check list", unitOfWork);
                    }

                    if (guaritaCheckList.GerarOSEquipamento)
                    {
                        InserirOrdemServicoEquipamento(guaritaCheckList, unitOfWork);
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, guaritaCheckList, null, "Gerou Ordem de Serviço de Equipamento nº " + guaritaCheckList.OrdemServicoFrotaEquipamento.Numero.ToString() + " na finalização do check list", unitOfWork);
                    }
                }

                unitOfWork.CommitChanges();

                // Retorna informacoes
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa();

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Busca Dados
                int totalRegistros = 0;
                var lista = ExecutaPesquisa(ref totalRegistros, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                return new JsonpResult(grid);
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

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
                Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo repGuaritaCheckListAnexo = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListAnexo(unitOfWork);
                Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList = repGuaritaCheckList.BuscarPorCodigo(codigo);
                List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListAnexo> anexos = repGuaritaCheckListAnexo.BuscarPorCheckList(codigo);

                if (guaritaCheckList == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList> categorias = (from p in guaritaCheckList.Perguntas where p.Categoria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList.Motorista select p.Categoria).Distinct().ToList();
                List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList> categoriasCroqui = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList>()
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.CategoriaOpcaoCheckList.Motorista
                };

                var retorno = new
                {
                    guaritaCheckList.Codigo,
                    Operador = guaritaCheckList.Operador != null ? new { guaritaCheckList.Operador.Codigo, Descricao = guaritaCheckList.Operador.Nome } : null,
                    Veiculo = guaritaCheckList.Veiculo?.Placa ?? string.Empty,
                    Motorista = guaritaCheckList.Motorista != null ? guaritaCheckList.Motorista.Nome : guaritaCheckList.Carga?.NomeMotoristas ?? guaritaCheckList.OrdemServicoFrota?.Motorista?.Nome ?? string.Empty,
                    Tipo = guaritaCheckList.DescricaoTipoEntradaSaida,
                    CheckListTipo = guaritaCheckList.CheckListTipo != null ? new { guaritaCheckList.CheckListTipo.Codigo, Descricao = guaritaCheckList.CheckListTipo.Descricao } : null,
                    OrdemServico = guaritaCheckList.OrdemServicoFrota?.Numero.ToString("n0") ?? string.Empty,
                    Carga = guaritaCheckList.Carga?.CodigoCargaEmbarcador ?? string.Empty,
                    Data = guaritaCheckList.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty,
                    guaritaCheckList.Situacao,

                    CheckList = (from c in categorias select FormataCategoria(c, (from p in guaritaCheckList.Perguntas where p.Categoria == c select p).ToList())).ToList(),
                    Croquis = (from c in categoriasCroqui select FormataCategoria(c, (from p in guaritaCheckList.Perguntas where p.Categoria == c select p).ToList())).ToList(),
                    Anexos = (from obj in anexos
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  obj.NomeArquivo,
                              }).ToList(),
                    Manutencao = new
                    {
                        guaritaCheckList.GerarOS,
                        DataProgramada = guaritaCheckList.DataProgramada?.ToString("dd/MM/yyyy") ?? (configuracaoTMS.PreencherDataProgramadaComAtualCheckList ? DateTime.Now.ToString("dd/MM/yyyy") : string.Empty),
                        TipoOrdemServico = guaritaCheckList.TipoOrdemServico != null ? new { guaritaCheckList.TipoOrdemServico.Codigo, guaritaCheckList.TipoOrdemServico.Descricao } : null,
                        LocalManutencao = guaritaCheckList.LocalManutencao != null ? new { guaritaCheckList.LocalManutencao.Codigo, guaritaCheckList.LocalManutencao.Descricao } :
                                          configuracaoTMS.LocalManutencaoPadraoCheckList != null ? new { configuracaoTMS.LocalManutencaoPadraoCheckList.Codigo, configuracaoTMS.LocalManutencaoPadraoCheckList.Descricao } : null,
                        guaritaCheckList.ObservacaoOS
                    },
                    Abastecimento = new
                    {
                        guaritaCheckList.GerarAbastecimento,
                        guaritaCheckList.TipoAbastecimento,
                        Posto = guaritaCheckList.Posto != null ? new { Codigo = guaritaCheckList.Posto.CPF_CNPJ, Descricao = guaritaCheckList.Posto.Nome } : null,
                        Produto = guaritaCheckList.Produto != null ? new { guaritaCheckList.Produto.Codigo, guaritaCheckList.Produto.Descricao } : null,
                        Equipamento = guaritaCheckList.Equipamento != null ? new { guaritaCheckList.Equipamento.Codigo, guaritaCheckList.Equipamento.Descricao } : null,
                        Litros = guaritaCheckList.Litros > 0 ? guaritaCheckList.Litros.ToString("n4") : string.Empty,
                        ValorUnitario = guaritaCheckList.ValorUnitario > 0 ? guaritaCheckList.ValorUnitario.ToString("n4") : string.Empty,
                        ValorTotal = guaritaCheckList.ValorTotal > 0 ? guaritaCheckList.ValorTotal.ToString("n2") : string.Empty,
                        Horimetro = guaritaCheckList.Horimetro > 0 ? guaritaCheckList.Horimetro.ToString("D") : string.Empty
                    },
                    ManutencaoEquipamento = new
                    {
                        NumeroOrdemServico = guaritaCheckList.OrdemServicoFrotaEquipamento?.Numero.ToString("n0") ?? string.Empty,
                        GerarOS = guaritaCheckList.GerarOSEquipamento,
                        DataProgramada = guaritaCheckList.DataProgramadaEquipamento?.ToString("dd/MM/yyyy") ?? (configuracaoTMS.PreencherDataProgramadaComAtualCheckList ? DateTime.Now.ToString("dd/MM/yyyy") : string.Empty),
                        TipoOrdemServico = guaritaCheckList.TipoOrdemServicoEquipamento != null ? new { guaritaCheckList.TipoOrdemServicoEquipamento.Codigo, guaritaCheckList.TipoOrdemServicoEquipamento.Descricao } : null,
                        LocalManutencao = guaritaCheckList.LocalManutencaoEquipamento != null ? new { guaritaCheckList.LocalManutencaoEquipamento.Codigo, guaritaCheckList.LocalManutencaoEquipamento.Descricao } :
                                          configuracaoTMS.LocalManutencaoPadraoCheckList != null ? new { configuracaoTMS.LocalManutencaoPadraoCheckList.Codigo, configuracaoTMS.LocalManutencaoPadraoCheckList.Descricao } : null,
                        EquipamentoServico = guaritaCheckList.EquipamentoServico != null ? new { guaritaCheckList.EquipamentoServico.Codigo, guaritaCheckList.EquipamentoServico.Descricao } : null,
                        ObservacaoOS = guaritaCheckList.ObservacaoOSEquipamento
                    }
                };

                return new JsonpResult(retorno);
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

        public async Task<IActionResult> Imprimir()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigo);

                byte[] pdf = ReportRequest.WithType(ReportType.CheckListGuarita)
                    .WithExecutionType(ExecutionType.Sync)
                    .AddExtraData("codigo", codigo.ToString())
                    .CallReport()
                    .GetContentFile();

                return Arquivo(pdf, "application/pdf", "Check List Guarita.pdf");
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o arquivo.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void SalvarCheckList(string json, Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas repGuaritaCheckListPerguntas = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntas(unitOfWork);

            List<dynamic> dynCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);
            if (dynCategorias == null) return;

            foreach (dynamic dynCategoria in dynCategorias)
            {
                foreach (var dynPergunta in dynCategoria.Perguntas)
                {
                    int.TryParse((string)dynPergunta.Codigo, out int codigo);
                    Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas pergunta = repGuaritaCheckListPerguntas.BuscarPorCodigoEGuarita(guaritaCheckList.Codigo, codigo);

                    if (pergunta != null)
                    {
                        pergunta.Initialize();
                        Enum.TryParse((string)dynPergunta.Tipo, out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList tipo);

                        switch (tipo)
                        {
                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.SimNao:
                                bool? simNao = null;
                                if (bool.TryParse((string)dynPergunta.Opcao, out bool simNaoAux))
                                    simNao = simNaoAux;
                                pergunta.Opcao = simNao;
                                break;

                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Informativo:
                                pergunta.Resposta = (string)dynPergunta.Resposta;
                                break;

                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Opcoes:
                                SalvarAlternativasCheckList(pergunta, dynPergunta, unitOfWork);
                                break;

                            case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Selecoes:
                                SalvarSelecoesCheckList(pergunta, dynPergunta, unitOfWork);
                                break;
                        }

                        repGuaritaCheckListPerguntas.Atualizar(pergunta, Auditado);
                    }
                }
            }
        }

        private void SalvarSelecoesCheckList(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas pergunta, dynamic dynPergunta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa repGuaritaCheckListPerguntasAlternativa = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa(unitOfWork);

            foreach (dynamic dynOpcoao in dynPergunta.Opcoes)
            {
                int.TryParse((string)dynOpcoao.value, out int codigo);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa alternativa = repGuaritaCheckListPerguntasAlternativa.BuscarPorCodigoEPergunta(pergunta.Codigo, codigo);

                if (alternativa != null)
                {
                    alternativa.Initialize();

                    bool selecionado = (Utilidades.String.OnlyNumbers((string)dynPergunta.Resposta) == Utilidades.String.OnlyNumbers((string)dynOpcoao.value));
                    alternativa.Marcado = selecionado;

                    repGuaritaCheckListPerguntasAlternativa.Atualizar(alternativa, Auditado);
                }
            }
        }

        private void SalvarAlternativasCheckList(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas pergunta, dynamic dynPergunta, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa repGuaritaCheckListPerguntasAlternativa = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa(unitOfWork);

            foreach (dynamic dynAlternativa in dynPergunta.Alternativas)
            {
                int.TryParse((string)dynAlternativa.Codigo, out int codigo);
                Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntasAlternativa alternativa = repGuaritaCheckListPerguntasAlternativa.BuscarPorCodigoEPergunta(pergunta.Codigo, codigo);

                if (alternativa != null)
                {
                    alternativa.Initialize();

                    bool.TryParse((string)dynAlternativa.Marcado, out bool marcado);
                    alternativa.Marcado = marcado;

                    repGuaritaCheckListPerguntasAlternativa.Atualizar(alternativa, Auditado);
                }
            }
        }

        private void SalvarManutencao(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);

            dynamic manutencao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Manutencao"));

            int.TryParse((string)manutencao.TipoOrdemServico, out int tipoOrdemServico);
            double.TryParse((string)manutencao.LocalManutencao, out double localManutencao);

            bool.TryParse((string)manutencao.GerarOS, out bool gerarOS);

            DateTime.TryParse((string)manutencao.DataProgramada, out DateTime dataProgramada);

            guaritaCheckList.GerarOS = gerarOS;
            if (dataProgramada > DateTime.MinValue)
                guaritaCheckList.DataProgramada = dataProgramada;
            else
                guaritaCheckList.DataProgramada = null;

            guaritaCheckList.TipoOrdemServico = tipoOrdemServico > 0 ? repTipoOrdemServico.BuscarPorCodigo(tipoOrdemServico) : null;
            guaritaCheckList.LocalManutencao = localManutencao > 0 ? repCliente.BuscarPorCPFCNPJ(localManutencao) : null;
            guaritaCheckList.ObservacaoOS = (string)manutencao.ObservacaoOS;
        }

        private void SalvarAbastecimento(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Produto repProduto = new Repositorio.Produto(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

            dynamic abastecimento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Abastecimento"));

            int produto = ((string)abastecimento.Produto).ToInt();
            double posto = ((string)abastecimento.Posto).ToDouble();
            int equipamento = ((string)abastecimento.Equipamento).ToInt();


            guaritaCheckList.GerarAbastecimento = ((string)abastecimento.GerarAbastecimento).ToBool();
            guaritaCheckList.TipoAbastecimento = ((string)abastecimento.TipoAbastecimento).ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoAbastecimento>();
            guaritaCheckList.Litros = ((string)abastecimento.Litros).ToDecimal();
            guaritaCheckList.ValorUnitario = ((string)abastecimento.ValorUnitario).ToDecimal();
            guaritaCheckList.Horimetro = ((string)abastecimento.Horimetro).ToInt();

            guaritaCheckList.Equipamento = equipamento > 0 ? repEquipamento.BuscarPorCodigo(equipamento) : null;
            guaritaCheckList.Produto = produto > 0 ? repProduto.BuscarPorCodigo(produto) : null;
            guaritaCheckList.Posto = posto > 0 ? repCliente.BuscarPorCPFCNPJ(posto) : null;
        }

        private void SalvarManutencaoEquipamento(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo repTipoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaTipo(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

            dynamic manutencaoEquipamento = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ManutencaoEquipamento"));

            int.TryParse((string)manutencaoEquipamento.TipoOrdemServico, out int tipoOrdemServico);
            int.TryParse((string)manutencaoEquipamento.EquipamentoServico, out int codigoEquipamento);
            double.TryParse((string)manutencaoEquipamento.LocalManutencao, out double localManutencao);

            bool.TryParse((string)manutencaoEquipamento.GerarOS, out bool gerarOS);

            DateTime.TryParse((string)manutencaoEquipamento.DataProgramada, out DateTime dataProgramada);

            guaritaCheckList.GerarOSEquipamento = gerarOS;
            if (dataProgramada > DateTime.MinValue)
                guaritaCheckList.DataProgramadaEquipamento = dataProgramada;
            else
                guaritaCheckList.DataProgramadaEquipamento = null;

            guaritaCheckList.EquipamentoServico = codigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(codigoEquipamento) : null;
            guaritaCheckList.TipoOrdemServicoEquipamento = tipoOrdemServico > 0 ? repTipoOrdemServico.BuscarPorCodigo(tipoOrdemServico) : null;
            guaritaCheckList.LocalManutencaoEquipamento = localManutencao > 0 ? repCliente.BuscarPorCPFCNPJ(localManutencao) : null;
            guaritaCheckList.ObservacaoOSEquipamento = (string)manutencaoEquipamento.ObservacaoOS;
        }

        private Models.Grid.Grid GridPesquisa()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("Codigo");
            grid.Prop("Operador").Nome("Operador").Tamanho(15).Align(Models.Grid.Align.left).Visibilidade(true, false, false);

            grid.Prop("Veiculo").Nome("Veículo").Tamanho(8).Align(Models.Grid.Align.left).Ord(false);

            if(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.Prop("NumeroFrota").Nome("Nº Frota").Tamanho(8).Align(Models.Grid.Align.left).Ord(false).Visibilidade(true, true, true); ;

            grid.Prop("Motorista").Nome("Motorista").Tamanho(15).Align(Models.Grid.Align.left).Ord(false).Visibilidade(true, true, true);
            grid.Prop("Tipo").Nome("Tipo").Tamanho(10).Align(Models.Grid.Align.left).Visibilidade(true, false, false);
            grid.Prop("CheckListTipo").Nome("Tipo Check").Tamanho(10).Align(Models.Grid.Align.left).Visibilidade(true, false, false); ;
            
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                grid.Prop("OrdemServico").Nome("O.S.").Tamanho(10).Align(Models.Grid.Align.left).Visibilidade(true, false, false);
                grid.Prop("Carga").Nome("Carga").Tamanho(10).Align(Models.Grid.Align.left).Visibilidade(true, false, false);
            }

            grid.Prop("Data").Nome("Data").Tamanho(12).Align(Models.Grid.Align.center).Visibilidade(true, true, true); ;

            return grid;
        }

        private void PropOrdena(ref string propOrdenar)
        {
            if (propOrdenar == "Operador") propOrdenar = "Operador.Nome";
            else if (propOrdenar == "Carga") propOrdenar = "Guarita.Carga.CodigoCargaEmbarcador";
            else if (propOrdenar == "OrdemServico") propOrdenar = "Guarita.OrdemServicoFrota.Numero";
            else if (propOrdenar == "Tipo") propOrdenar = "Guarita.TipoEntradaSaida";
        }

        private dynamic ExecutaPesquisa(ref int totalRegistros, string propOrdenar, string dirOrdena, int inicio, int limite, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancia repositorios
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);

            // Dados do filtro
            int.TryParse(Request.Params("Carga"), out int carga);
            int.TryParse(Request.Params("OrdemServico"), out int ordemServico);
            int.TryParse(Request.Params("Operador"), out int operador);
            int.TryParse(Request.Params("Veiculo"), out int veiculo);
            Enum.TryParse(Request.Params("Tipo"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipo);
            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoGuaritaCheckList situacao);

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            // Consulta
            List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList> listaGrid = repGuaritaCheckList.Consultar(ordemServico, tipo, carga, operador, situacao, veiculo, codigoEmpresa, propOrdenar, dirOrdena, inicio, limite);
            totalRegistros = repGuaritaCheckList.ContarConsulta(ordemServico, tipo, carga, operador, situacao, veiculo, codigoEmpresa);

            var lista = (from obj in listaGrid
                         select new
                         {
                             Codigo = obj.Codigo,
                             Operador = obj.Operador?.Nome ?? string.Empty,
                             Veiculo = obj.Veiculo?.Placa ?? string.Empty,
                             NumeroFrota = obj.Veiculo?.NumeroFrota ?? string.Empty,
                             Motorista = obj.Motorista != null ? obj.Motorista.Nome : obj.Carga != null ? obj.Carga.NomeMotoristas : obj.OrdemServicoFrota?.Motorista?.Nome ?? string.Empty,
                             Tipo = obj.DescricaoTipoEntradaSaida,
                             CheckListTipo = obj.CheckListTipo?.Descricao ?? string.Empty,
                             OrdemServico = obj.OrdemServicoFrota != null ? obj.OrdemServicoFrota.Numero.ToString("n0") : string.Empty,
                             Carga = obj.Carga != null ? obj.Carga.CodigoCargaEmbarcador : string.Empty,
                             Data = obj.Data?.ToString("dd/MM/yyyy HH:mm") ?? string.Empty
                         }).ToList();

            return lista.ToList();
        }

        private dynamic FormataCategoria(CategoriaOpcaoCheckList categoria, List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas> perguntas)
        {
            return new
            {
                Descricao = perguntas.FirstOrDefault()?.Categoria.ObterDescricao() ?? string.Empty,
                Categoria = categoria,
                Perguntas = (from p in perguntas select FormataPergunta(p)).ToList()
            };
        }

        private dynamic FormataPergunta(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListPerguntas pergunta)
        {
            return new
            {
                Codigo = pergunta.Codigo,
                Descricao = pergunta.Descricao,
                Tipo = pergunta.Tipo,

                Resposta = pergunta.Tipo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOpcaoCheckList.Selecoes ? (from a in pergunta.Alternativas
                                                                                                                          where a.Marcado == true
                                                                                                                          select a.Codigo)?.FirstOrDefault().ToString() ?? "" : pergunta.Resposta,

                Opcao = pergunta.Opcao.HasValue ? (pergunta.Opcao.Value == true ? "true" : "false") : null,

                Alternativas = (from a in pergunta.Alternativas
                                select new
                                {
                                    Codigo = a.Codigo,
                                    Marcado = a.Marcado,
                                    Descricao = a.Descricao,
                                    Tipo = pergunta.Tipo
                                }).ToList(),
                Opcoes = (from a in pergunta.Alternativas
                          select new
                          {
                              value = a.Codigo,
                              text = a.Descricao
                          }).ToList()
            };
        }

        private void InserirOrdemServico(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Dominio.Entidades.Veiculo veiculo = guaritaCheckList.Veiculo;

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            var tipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.PreventivaECorretiva;

            if (guaritaCheckList.TipoOrdemServico.OSCorretiva)
                tipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.Corretiva;

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota()
            {
                DataProgramada = guaritaCheckList.DataProgramada?.Date ?? DateTime.Now.Date,
                LocalManutencao = guaritaCheckList.LocalManutencao,
                Motorista = guaritaCheckList.Motorista,
                Numero = repOrdemServico.BuscarUltimoNumero(codigoEmpresa) + 1,
                Observacao = "GERADO A PARTIR DO CHECK LIST" + (!string.IsNullOrWhiteSpace(guaritaCheckList.ObservacaoOS) ? " - " + guaritaCheckList.ObservacaoOS : ""),
                Operador = this.Usuario,
                TipoManutencao = tipoManutencao,
                Veiculo = veiculo,
                QuilometragemVeiculo = guaritaCheckList.KMAtual > 0 ? guaritaCheckList.KMAtual : veiculo?.KilometragemAtual ?? 0,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.EmManutencao,
                DataAlteracao = DateTime.Now,
                TipoOrdemServico = guaritaCheckList.TipoOrdemServico,
                Empresa = codigoEmpresa > 0 ? this.Usuario.Empresa : null
            };

            if (ordemServico.LocalManutencao != null)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(ordemServico.LocalManutencao.CPF_CNPJ);

                if (modalidadeFornecedor != null && modalidadeFornecedor.Oficina && modalidadeFornecedor.TipoOficina.HasValue)
                    ordemServico.TipoOficina = modalidadeFornecedor.TipoOficina;
            }

            repOrdemServico.Inserir(ordemServico, Auditado);

            List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoVeiculo> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(guaritaCheckList.Codigo);
            foreach (var servicoGuaritaCheckList in servicosGuaritaCheckList)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo()
                {
                    CustoEstimado = servicoGuaritaCheckList.CustoEstimado,
                    CustoMedio = servicoGuaritaCheckList.CustoMedio,
                    Observacao = servicoGuaritaCheckList.Observacao,
                    OrdemServico = ordemServico,
                    Servico = servicoGuaritaCheckList.Servico,
                    TipoManutencao = servicoGuaritaCheckList.TipoManutencao,
                    UltimaManutencao = servicoGuaritaCheckList.UltimaManutencao,
                    TempoEstimado = servicoGuaritaCheckList.TempoEstimado,
                    TempoExecutado = 0
                };

                repServicoOrdemServico.Inserir(servicoOrdemServico, Auditado);
            }

            if (veiculo == null)
                Servicos.Embarcador.Frota.OrdemServicoOrcamento.GerarOrcamentoInicial(ordemServico, null, unitOfWork);
            else
            {
                Servicos.Embarcador.Frota.OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unitOfWork);
                Servicos.Embarcador.Frota.OrdemServicoManutencao.IniciarManutencaoVeiculo(veiculo.Codigo, ordemServico, unitOfWork, Auditado);
            }

            Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unitOfWork);

            guaritaCheckList.OrdemServicoFrota = ordemServico;
            repGuaritaCheckList.Atualizar(guaritaCheckList);
        }

        private void InserirOrdemServicoEquipamento(Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckList guaritaCheckList, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckList repGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckList(unitOfWork);
            Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento repServicoGuaritaCheckList = new Repositorio.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrota repOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrota(unitOfWork);
            Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo repServicoOrdemServico = new Repositorio.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = guaritaCheckList.EquipamentoServico;

            int codigoEmpresa = 0;
            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Usuario.Empresa.Codigo;

            Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota ordemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrota()
            {
                DataProgramada = guaritaCheckList.DataProgramadaEquipamento?.Date ?? DateTime.Now.Date,
                LocalManutencao = guaritaCheckList.LocalManutencaoEquipamento,
                Motorista = guaritaCheckList.Motorista,
                Numero = repOrdemServico.BuscarUltimoNumero(codigoEmpresa) + 1,
                Observacao = "GERADO A PARTIR DO CHECK LIST DE EQUIPAMENTO" + (!string.IsNullOrWhiteSpace(guaritaCheckList.ObservacaoOSEquipamento) ? " - " + guaritaCheckList.ObservacaoOSEquipamento : ""),
                Operador = this.Usuario,
                TipoManutencao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManutencaoOrdemServicoFrota.PreventivaECorretiva,
                Equipamento = equipamento,
                Horimetro = equipamento.Horimetro,
                Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOrdemServicoFrota.EmManutencao,
                DataAlteracao = DateTime.Now,
                TipoOrdemServico = guaritaCheckList.TipoOrdemServicoEquipamento,
                Empresa = codigoEmpresa > 0 ? this.Usuario.Empresa : null,
                Veiculo = guaritaCheckList.Veiculo
            };

            if (ordemServico.LocalManutencao != null)
            {
                Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas repModalidadeFornecedor = new Repositorio.Embarcador.Pessoas.ModalidadeFornecedorPessoas(unitOfWork);

                Dominio.Entidades.Embarcador.Pessoas.ModalidadeFornecedorPessoas modalidadeFornecedor = repModalidadeFornecedor.BuscarPorCliente(ordemServico.LocalManutencao.CPF_CNPJ);

                if (modalidadeFornecedor != null && modalidadeFornecedor.Oficina && modalidadeFornecedor.TipoOficina.HasValue)
                    ordemServico.TipoOficina = modalidadeFornecedor.TipoOficina;
            }

            repOrdemServico.Inserir(ordemServico, Auditado);

            List<Dominio.Entidades.Embarcador.GestaoPatio.GuaritaCheckListServicoEquipamento> servicosGuaritaCheckList = repServicoGuaritaCheckList.BuscarPorGuaritaCheckList(guaritaCheckList.Codigo);
            foreach (var servicoGuaritaCheckList in servicosGuaritaCheckList)
            {
                Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo servicoOrdemServico = new Dominio.Entidades.Embarcador.Frota.OrdemServicoFrotaServicoVeiculo()
                {
                    CustoEstimado = servicoGuaritaCheckList.CustoEstimado,
                    CustoMedio = servicoGuaritaCheckList.CustoMedio,
                    Observacao = servicoGuaritaCheckList.Observacao,
                    OrdemServico = ordemServico,
                    Servico = servicoGuaritaCheckList.Servico,
                    TipoManutencao = servicoGuaritaCheckList.TipoManutencao,
                    UltimaManutencao = servicoGuaritaCheckList.UltimaManutencao,
                    TempoEstimado = servicoGuaritaCheckList.TempoEstimado,
                    TempoExecutado = 0
                };

                repServicoOrdemServico.Inserir(servicoOrdemServico, Auditado);
            }

            if (equipamento == null)
                Servicos.Embarcador.Frota.OrdemServicoOrcamento.GerarOrcamentoInicial(ordemServico, null, unitOfWork);
            else
                Servicos.Embarcador.Frota.OrdemServico.AtualizarTipoManutencaoOrdemServico(ref ordemServico, unitOfWork);

            Servicos.Embarcador.Frota.OrdemServico.SalvarLogAlteracao(ordemServico, Usuario, unitOfWork);

            guaritaCheckList.OrdemServicoFrotaEquipamento = ordemServico;
            repGuaritaCheckList.Atualizar(guaritaCheckList);
        }

        #endregion
    }
}
