using Dominio.Entidades.Embarcador.Pedidos;
using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Pedidos
{
    [CustomAuthorize(new string[] { "RegrasAprovacao" }, "Pedidos/AutorizacaoPedido")]
    public class AutorizacaoPedidoController : BaseController
    {
		#region Construtores

		public AutorizacaoPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdena = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdena);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPedidos, ref totalRegistro, propOrdena, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPedidos, unitOfWork);

                // Retorna Grid
                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistro);

                // Retorna Dados
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

        [AllowAuthenticate]
        public async Task<IActionResult> ExportarPesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Manipula grids
                Models.Grid.Grid grid = GridPesquisa(unitOfWork);

                // Ordenacao da grid
                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                PropOrdena(ref propOrdenar);

                // Lista de ocorrencias que vai receber consulta
                List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                // Variavel com o numero total de resultados
                int totalRegistro = 0;

                // Executa metodo de consutla
                ExecutaPesquisa(ref listaPedidos, ref totalRegistro, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite, unitOfWork);

                // Converte os dados recebidos
                var lista = RetornaDyn(listaPedidos, unitOfWork);

                // Seta valores na grid
                grid.AdicionaRows(lista);

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
                return new JsonpResult(false, "Ocorreu uma falha ao exportar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RegrasAprovacao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia repositorio
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

                // Converte parametros
                int.TryParse(Request.Params("Codigo"), out int codigo);
                int.TryParse(Request.Params("Usuario"), out int usuario);

                // Manipula grids
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Regra", "Regra", 30, Models.Grid.Align.left, false);

                if (usuario > 0)
                    grid.AdicionarCabecalho("Usuario", false);
                else
                    grid.AdicionarCabecalho("Usuário", "Usuario", 15, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("PodeAprovar", false);
                grid.AdicionarCabecalho("Observacao", false);
                grid.AdicionarCabecalho("MotivoRejeicao", false);

                // Buscas regras do usuario para essa ocorrencia
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> regras = repPedidoAutorizacao.BuscarPorPedidoUsuario(codigo, usuario);

                // Converte as regras em dados apresentaveis
                var lista = from ocorrenciaAutorizacao in regras
                            select new
                            {
                                ocorrenciaAutorizacao.Codigo,
                                Regra = TituloRegra(ocorrenciaAutorizacao),
                                MotivoRejeicao = ocorrenciaAutorizacao.Motivo,
                                Observacao = ocorrenciaAutorizacao.Pedido.Observacao,
                                Situacao = ocorrenciaAutorizacao.DescricaoSituacao,
                                Usuario = ocorrenciaAutorizacao.Usuario.Nome,
                                Etapa = ocorrenciaAutorizacao.DescricaoEtapaAutorizacaoOcorrencia,
                                // Verifica se o usuario ja motificou essa autorizacao
                                PodeAprovar = repPedidoAutorizacao.VerificarSePodeAprovar(codigo, ocorrenciaAutorizacao.Codigo, this.Usuario.Codigo),
                                // Busca a cor de acordo com a situacao da autorizacao
                                DT_RowColor = this.CoresRegras(ocorrenciaAutorizacao)
                            };

                // Retorna Grid
                grid.setarQuantidadeTotal(regras.Count());
                grid.AdicionaRows(lista.ToList());
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
            // Busca a ocorrencia
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Repositorios
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                // Codigo requisicao
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Pedidos.Pedido pedido = repPedido.BuscarPorCodigo(codigo);

                if (pedido == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar por código.");

                Dominio.Entidades.Veiculo veiculo = pedido.VeiculoTracao ?? pedido.Veiculos.FirstOrDefault();

                var dynOcorrencia = new
                {
                    pedido.Codigo,
                    ValorFrete = pedido.ValorFreteNegociado.ToString("n2"),
                    NumeroPedido = pedido.Numero.ToString("n0"),
                    DataPedido = pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value.ToString("dd/MM/yyyy") : string.Empty,

                    Situacao = pedido.DescricaoSituacaoPedido,
                    NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                    TipoCarga = pedido.TipoDeCarga?.Descricao ?? string.Empty,

                    TipoOperacao = pedido.TipoOperacao?.Descricao ?? string.Empty,
                    Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente != null ? pedido.Remetente.Nome : string.Empty,
                    Destinatario = pedido.Destinatario?.Nome ?? string.Empty,

                    CidadeUfRemetente = pedido.Remetente?.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                    CidadeUfDestinatario = pedido.Destinatario?.Localidade?.DescricaoCidadeEstado ?? string.Empty,

                    Motorista = pedido.NomeMotoristas,
                    Solicitante = pedido.Usuario?.Nome ?? string.Empty,

                    pedido.Observacao,
                    MotivoCancelamento = string.Empty,
                    Veiculo = veiculo != null ? BuscarPlacas(pedido) : null,

                    ModeloVeicularCarga = pedido.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    ModeloVeiculo = pedido.VeiculoTracao?.Modelo?.Descricao ?? string.Empty,
                    TipoCarroceria = ConverterTipoCarroceria(pedido.VeiculoTracao?.TipoCarroceria ?? string.Empty),
                    ModeloCarroceria = veiculo?.ModeloCarroceria?.Descricao ?? (pedido.Veiculos.Any(v => v.ModeloCarroceria?.Descricao != null)
                       ? pedido.Veiculos.First(v => v.ModeloCarroceria?.Descricao != null).ModeloCarroceria.Descricao
                       : string.Empty),

                    EnumSituacao = pedido.SituacaoPedido,

                    PermiteSelecionarTomador = false,

                    Tomador = 0,
                };

                return new JsonpResult(dynOcorrencia);
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

        public async Task<IActionResult> AprovarMultiplasRegras()
        {
            /* Busca todas as regras da ocorrencia
             * Aprova todas as regras
             * Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
             */
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                // Instancia
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);

                // Converte parametros
                int codigoPedido = 0;
                int.TryParse(Request.Params("Codigo"), out codigoPedido);

                // Busca todas as regras das ocorrencias selecionadas
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> pedidoAutorizacao = repPedidoAutorizacao.BuscarPendentesPorPedidoEUsuario(codigoPedido, this.Usuario.Codigo);

                // Inicia transacao
                unitOfWork.Start();

                // Aprova todas as regras
                for (int i = 0; i < pedidoAutorizacao.Count(); i++)
                    EfetuarAprovacao(pedidoAutorizacao[i], unitOfWork, null);

                // Atualiza informacoes das ocorrencias (verifica se esta aprovada ou rejeitada)
                this.VerificarSituacaoPedido(repPedido.BuscarPorCodigo(codigoPedido), unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(new
                {
                    RegrasModificadas = pedidoAutorizacao.Count()
                });
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Aprovar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

                // Codigo requisicao
                int.TryParse(Request.Params("Codigo"), out int codigo);

                // Entidades
                Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao pedidoAutorizacao = repPedidoAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (pedidoAutorizacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida a situacao
                if (pedidoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                if (!ValidarNotasParciais(pedidoAutorizacao.Pedido.Codigo, unitOfWork))
                    return new JsonpResult(false, "Os dados da nota devem ser informados no pedido antes da aprovação.");

                // Inicia transacao
                unitOfWork.Start();

                // Chama metodo de aprovacao
                EfetuarAprovacao(pedidoAutorizacao, unitOfWork, null);

                // Faz verificacao se a carga esta aprovada
                this.VerificarSituacaoPedido(pedidoAutorizacao.Pedido, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Rejeitar()
        {
            // Recebe o codigo da regra especifica aprovada
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                // Repositorios
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

                // Codigo da regra
                int codigo = 0;
                int.TryParse(Request.Params("Codigo"), out codigo);

                string motivo = !string.IsNullOrWhiteSpace(Request.Params("Motivo")) ? Request.Params("Motivo") : string.Empty;

                // Entidades
                Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao pedidoAutorizacao = repPedidoAutorizacao.BuscarPorCodigo(codigo);

                // Valida se é o usuario da regra
                if (pedidoAutorizacao == null || pedidoAutorizacao.Usuario.Codigo != this.Usuario.Codigo)
                    return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(motivo))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                // Valida a situacao
                if (pedidoAutorizacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                    return new JsonpResult(false, "A situação da aprovação não permite alterações da mesma.");

                // Inicia transacao
                unitOfWork.Start();

                // Seta com aprovado e coloca informacoes do evento
                pedidoAutorizacao.Data = DateTime.Now;
                pedidoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                pedidoAutorizacao.Motivo = motivo;

                // Atualiza banco
                repPedidoAutorizacao.Atualizar(pedidoAutorizacao);

                // Verifica status gerais
                this.NotificarAlteracao(false, pedidoAutorizacao.Pedido, unitOfWork);
                this.VerificarSituacaoPedido(pedidoAutorizacao.Pedido, unitOfWork);

                // Finaliza transacao
                unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (BaseException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ReprovarMultiplasPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                var repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
                var repMotivoPedido = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);

                var pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

                // Codigo da regra
                int codigoJustificativa = 0;
                int codigoMotivoPedido = 0;

                int.TryParse(Request.Params("Justificativa"),  out codigoJustificativa);
                int.TryParse(Request.Params("CodigoMotivoPedido"), out codigoMotivoPedido);
                

                // Valida motivo  (obrigatorio)
                if (string.IsNullOrWhiteSpace(Request.Params("Motivo")))
                    return new JsonpResult(false, "Motivo é obrigatório.");

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    pedidos = ObterPedidosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                var pedidoAutorizacao = BuscarRegrasPorpedidos(pedidos, Usuario.Codigo, unitOfWork);
                var motivoPedido = repMotivoPedido.BuscarPorCodigo(codigoMotivoPedido);


                // Guarda os valores das ocorrencias para fazer a checagem geral
                var codigosOcorrenciasVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < pedidoAutorizacao.Count(); i++)
                {

                    // Inicia transacao
                    unitOfWork.Start();
                    try
                    {
                        int codigo = pedidoAutorizacao[i].Pedido.Codigo;

                        // Metodo de rejeitar avaria
                        pedidoAutorizacao[i].Data = DateTime.Now;
                        pedidoAutorizacao[i].Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada;
                        pedidoAutorizacao[i].Motivo = Request.Params("Motivo");
                        pedidoAutorizacao[i].MotivoPedido = motivoPedido;

                        // Atualiza banco
                        repPedidoAutorizacao.Atualizar(pedidoAutorizacao[i]);
                        VerificarSituacaoPedido(repPedido.BuscarPorCodigo(codigo), unitOfWork);

                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                    }
                }

                return new JsonpResult(new
                {
                    RegrasModificadas = pedidoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> AprovarMultiplasPedidos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var pedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();
                var repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                var repMotivoPedido = new Repositorio.Embarcador.Pedidos.MotivoPedido(unitOfWork);

                try
                {
                    // Busca todas as ocorrencias selecionadas
                    pedidos = ObterPedidosSelecionadas(unitOfWork);
                }
                catch (Exception ex)
                {
                    return new JsonpResult(false, ex.Message);
                }

                // Busca todas as regras das ocorrencias selecionadas
                var pedidoAutorizacao = BuscarRegrasPorpedidos(pedidos, Usuario.Codigo, unitOfWork);
                
                var codigoMotivoPedido = int.TryParse(Request.Params("CodigoMotivoPedido"), out int cod) ? cod : 0;
                var motivoPedido = repMotivoPedido.BuscarPorCodigo(codigoMotivoPedido);

                // Inicia transacao


                // Guarda os valores das ocorrencias para fazer a checagem geral
                var codigosPedidosVerificados = new List<int>();

                // Aprova todas as regras
                for (int i = 0; i < pedidoAutorizacao.Count(); i++)
                {
                    unitOfWork.Start();
                    try
                    {
                        int codigo = pedidoAutorizacao[i].Pedido.Codigo;

                        // Metodo de aprovar a ocorrencia
                        EfetuarAprovacao(pedidoAutorizacao[i], unitOfWork, motivoPedido);
                        VerificarSituacaoPedido(repPedido.BuscarPorCodigo(codigo), unitOfWork);
                        unitOfWork.CommitChanges();
                    }
                    catch (Exception ex2)
                    {
                        unitOfWork.Rollback();
                        Servicos.Log.TratarErro(ex2);
                    }
                }
                return new JsonpResult(new
                {
                    RegrasModificadas = pedidoAutorizacao.Count()
                });
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao aprovar os pedidos.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private void PropOrdena(ref string propOrdena)
        {
            if (propOrdena == "CodigoCargaEmbarcador")
                propOrdena = "Carga.CodigoCargaEmbarcador";
        }

        private void ExecutaPesquisa(ref List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, ref int totalRegistros, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, Repositorio.UnitOfWork unitOfWork)
        {
            // Instancias
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            // Converte parametros
            int.TryParse(Request.Params("NumeroPedido"), out int numeroPedido);
            int.TryParse(Request.Params("TipoCarga"), out int codigoTipoCarga);
            int.TryParse(Request.Params("GrupoPessoa"), out int codigoGrupoPessoa);
            int.TryParse(Request.Params("Usuario"), out int usuario);
            int.TryParse(Request.Params("TipoOperacao"), out int codigoTipoOperacao);
            int.TryParse(Request.Params("ModeloVeicularCarga"), out int modeloVeicularCarga);
            int.TryParse(Request.Params("ModeloVeiculo"), out int ModeloVeiculo);
            string TipoCarroceria = Request.Params("TipoCarroceria")?.PadLeft(2, '0') ?? "00";
            int.TryParse(Request.Params("ModeloCarroceria"), out int ModeloCarroceria);

            Enum.TryParse(Request.Params("Situacao"), out Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao);

            DateTime.TryParseExact(Request.Params("DataInicial"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
            DateTime.TryParseExact(Request.Params("DataFinal"), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

            listaPedidos = repPedidoAutorizacao.Consultar(usuario, dataInicial, dataFinal, situacao, numeroPedido, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao, propOrdenacao, dirOrdenacao, inicioRegistros, maximoRegistros, modeloVeicularCarga, ModeloVeiculo, TipoCarroceria, ModeloCarroceria);
            totalRegistros = repPedidoAutorizacao.ContarConsulta(usuario, dataInicial, dataFinal, situacao, numeroPedido, codigoGrupoPessoa, codigoTipoCarga, codigoTipoOperacao, modeloVeicularCarga, ModeloVeiculo, TipoCarroceria, ModeloCarroceria);
        }

        private Models.Grid.Grid GridPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            // Manipula grids
            Models.Grid.Grid grid = new Models.Grid.Grid(Request);
            grid.header = new List<Models.Grid.Head>();

            // Cabecalhos grid
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "DataCarregamentoPedido", 6, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Nº Pedido", "Numero", 7, Models.Grid.Align.center, true);
            grid.AdicionarCabecalho("Motorista(s)", "NomeMotoristas", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Tipo Carga", "TipoDeCarga", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("KM Rota", "KMRota", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Nº Pedido Embarcador", "NumeroPedidoEmbarcador", 7, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Remetente", "Remetente", 18, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho("Valor", "Valor", 6, Models.Grid.Align.right, true);
            grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho("Aprovadores", "Aprovadores", 10, Models.Grid.Align.left, true);

            return grid;
        }

        private dynamic RetornaDyn(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            var lista = from pedido in listaPedidos
                        select new
                        {
                            pedido.Codigo,
                            DataCarregamentoPedido = pedido.DataCarregamentoPedido.HasValue ? pedido.DataCarregamentoPedido.Value.ToString("dd/MM/yyyy") : string.Empty,
                            Numero = pedido.Numero.ToString("n0"),
                            NomeMotoristas = pedido.NomeMotoristas,
                            TipoDeCarga = pedido.TipoDeCarga?.Descricao ?? string.Empty,
                            KMRota = pedido?.RotaFrete?.Quilometros.ToString("n2") ?? string.Empty,
                            TipoOperacao = pedido.TipoOperacao?.Descricao ?? string.Empty,
                            NumeroPedidoEmbarcador = pedido.NumeroPedidoEmbarcador,
                            Remetente = pedido.GrupoPessoas != null ? pedido.GrupoPessoas.Descricao : pedido.Remetente != null ? pedido.Remetente.Nome : string.Empty,
                            Valor = pedido.ValorFreteNegociado.ToString("n2"),
                            Situacao = pedido.DescricaoSituacaoPedido,
                            Aprovadores = string.Join(", ", pedido.PedidoAutorizacao.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada).Select(o => o.Usuario.Nome))
                        };

            return lista.ToList();
        }

        private string TituloRegra(Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao regra)
        {
            return regra.RegrasPedido?.Descricao;
        }

        private string CoresRegras(Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao regra)
        {
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Success;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Rejeitada)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Danger;
            if (regra.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente)
                return Dominio.ObjetosDeValor.Embarcador.Enumeradores.CorGrid.Info;
            else
                return "";
        }

        private void EfetuarAprovacao(Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao pedidoAutorizacao, Repositorio.UnitOfWork unitOfWork, MotivoPedido motivoPedido)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);

            // So modifica a autorizacao quando ela for pendente
            if (pedidoAutorizacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Pendente && pedidoAutorizacao.Usuario.Codigo == Usuario.Codigo)
            {
                // Seta com aprovado e adiciona a hora do evento
                pedidoAutorizacao.Data = DateTime.Now;
                pedidoAutorizacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaAutorizacao.Aprovada;
                pedidoAutorizacao.Motivo = Request.Params("Motivo");
                pedidoAutorizacao.MotivoPedido = motivoPedido;

                // Atualiza os dados
                repPedidoAutorizacao.Atualizar(pedidoAutorizacao);

                // Notifica usuario que criou a ocorrencia
                NotificarAlteracao(true, pedidoAutorizacao.Pedido, unitOfWork);
            }
        }

        private void NotificarAlteracao(bool aprovada, Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Define icone
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                if (aprovada)
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;
                else
                    icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;

                // Emite notificação
                string titulo = Localization.Resources.Pedidos.Pedido.TituloPedido;
                string mensagem = string.Format(Localization.Resources.Pedidos.AutorizacaoPedido.UsuarioPedidoValorMotorista, (aprovada ? Localization.Resources.Gerais.Geral.Aprovou : Localization.Resources.Gerais.Geral.Rejeitou, pedido.Numero.ToString("n0"), pedido.ValorFreteNegociado.ToString("n2"), pedido.NomeMotoristas));
                serNotificacao.GerarNotificacaoEmail(pedido.Usuario, this.Usuario, pedido.Codigo, "Pedidos/Pedido", titulo, mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
        }

        private void VerificarSituacaoPedido(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido, Repositorio.UnitOfWork unitOfWork)
        {

            // Se a ocorencia nao esta com sitacao pendente, nao faz verificacao
            if (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente)
            {
                // Soma o numero de Interacoes, Aprovacoes e quantidade minima para proxima etapa
                Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
                Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
                Servicos.Embarcador.Notificacao.Notificacao serNotificacao = new Servicos.Embarcador.Notificacao.Notificacao(_conexao.StringConexao, null, TipoServicoMultisoftware, string.Empty);

                // Busca todas regras da ocorrencia (Distintas)
                List<Dominio.Entidades.Embarcador.Pedidos.RegrasPedido> regras = repPedidoAutorizacao.BuscarRegrasPedido(pedido.Codigo);

                // Flag de rejeicao
                bool rejeitada = false;
                bool aprovada = true;

                foreach (Dominio.Entidades.Embarcador.Pedidos.RegrasPedido regra in regras)
                {
                    // Quantidade de usuarios que marcaram como aprovado ou rejeitado
                    int pendentes = repPedidoAutorizacao.ContarPendentes(pedido.Codigo, regra.Codigo); // P

                    // Quantidade de aprovacoes
                    int aprovacoes = repPedidoAutorizacao.ContarAprovacoesOcorrencia(pedido.Codigo, regra.Codigo); // A

                    int rejeitadas = repPedidoAutorizacao.ContarRejeitadas(pedido.Codigo, regra.Codigo); // R

                    // Numero de aprovacoes minimas
                    int necessariosParaAprovar = regra.NumeroAprovadores; // N

                    if (rejeitadas > 0)
                        rejeitada = true; // Se uma regra foi reprovada, a carga ocorrencia é reprovada
                    else if (aprovacoes < necessariosParaAprovar) // A >= N -> Aprovacoes > NumeroMinimo
                        aprovada = false; // Se nao esta rejeitada e nem reprovada, esta pendente (nao faz nada)
                }

                // Define situacao da ocorrencia
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.AgAutorizacao;

                // Rejeicao na autorizacao
                if (rejeitada && (pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente))
                    situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Rejeitado;

                // Se houve alteracao de status, atualiza etapa da ocorencia
                if (rejeitada || aprovada)
                {
                    // Verifica se a situacao e ag aprovacao para testar a regra de etapa ag emissao
                    if ((pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AgAprovacao || pedido.SituacaoPedido == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.AutorizacaoPendente) && !rejeitada)
                    {
                        etapa = Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaPedido.Finalizada;
                        situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPedido.Aberto;
                    }

                    // Seta a nova situacao
                    pedido.SituacaoPedido = situacao;
                    pedido.EtapaPedido = etapa;

                    string retorno = Servicos.Embarcador.Pedido.Pedido.CriarCarga(pedido, unitOfWork, TipoServicoMultisoftware, Cliente, ConfiguracaoEmbarcador, cadastroPedido: true);
                    if (string.IsNullOrWhiteSpace(retorno))
                        repPedido.Atualizar(pedido);
                    else
                    {
                        throw new Exception(retorno);
                    }

                    // Define icone
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao icone;
                    if (rejeitada)
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.rejeitado;
                    else
                        icone = Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.confirmado;

                    // Emite notificação
                    string mensagem = string.Format(Localization.Resources.Pedidos.AutorizacaoPedido.PedidoValorMotoristaFoi, pedido.Numero.ToString("n0"), pedido.ValorFreteNegociado.ToString("n2"), pedido.NomeMotoristas, (rejeitada ? Localization.Resources.Gerais.Geral.Rejeitado : Localization.Resources.Gerais.Geral.Aprovado));
                    serNotificacao.GerarNotificacao(pedido.Usuario, this.Usuario, pedido.Codigo, "Pedidos/Pedido", mensagem, icone, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.credito, TipoServicoMultisoftware, unitOfWork);
                }
            }
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.Pedido> ObterPedidosSelecionadas(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
            Repositorio.Embarcador.Pedidos.Pedido repPedido = new Repositorio.Embarcador.Pedidos.Pedido(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.Pedido> listaPedidos = new List<Dominio.Entidades.Embarcador.Pedidos.Pedido>();

            bool todosSelecionados = false;
            bool.TryParse(Request.Params("SelecionarTodos"), out todosSelecionados);

            if (todosSelecionados)
            {
                try
                {
                    int totalRegistros = 0;
                    ExecutaPesquisa(ref listaPedidos, ref totalRegistros, "Codigo", "", 0, 0, unitOfWork);
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                    new Exception("Erro ao converte dados.");
                }

                dynamic listaPedidosNaoSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PedidosNaoSelecionadas"));
                foreach (var dybPedidoNaoSelecionada in listaPedidosNaoSelecionadas)
                    listaPedidos.Remove(new Dominio.Entidades.Embarcador.Pedidos.Pedido() { Codigo = (int)dybPedidoNaoSelecionada.Codigo });
            }
            else
            {
                dynamic listaPedidosSelecionadas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("PedidosSelecionadas"));
                foreach (var dynPedidoSelecionada in listaPedidosSelecionadas)
                    listaPedidos.Add(repPedido.BuscarPorCodigo((int)dynPedidoSelecionada.Codigo));
            }

            return listaPedidos;
        }

        private List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> BuscarRegrasPorpedidos(List<Dominio.Entidades.Embarcador.Pedidos.Pedido> pedidos, int usuario, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Pedidos.PedidoAutorizacao repPedidoAutorizacao = new Repositorio.Embarcador.Pedidos.PedidoAutorizacao(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> pedidoAutorizacao = new List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao>();

            // Itera todas as ocorrencias
            foreach (Dominio.Entidades.Embarcador.Pedidos.Pedido pedido in pedidos)
            {
                // Busca as autorizacoes da ocorrencias                
                List<Dominio.Entidades.Embarcador.Pedidos.PedidoAutorizacao> regras = repPedidoAutorizacao.BuscarPendentesPorPedidoEUsuario(pedido.Codigo, usuario);

                // Adiciona na lista
                pedidoAutorizacao.AddRange(regras);
            }

            // Retornas a lista com todas as autorizacao das ocorrencias
            return pedidoAutorizacao;
        }

        private bool ValidarNotasParciais(int codigoPedido, Repositorio.UnitOfWork unitOfWork)
        {
            if (!ConfiguracaoEmbarcador.ExigirInformarNotasFiscaisNoPedido || TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return true;

            Repositorio.Embarcador.Pedidos.PedidoNotaParcial repositorioPedidoNotaParcial = new Repositorio.Embarcador.Pedidos.PedidoNotaParcial(unitOfWork);
            return repositorioPedidoNotaParcial.BuscarExistePorPedido(codigoPedido);

        }

        private string BuscarPlacas(Dominio.Entidades.Embarcador.Pedidos.Pedido pedido)
        {
            string placas = "";

            if (ConfiguracaoEmbarcador.PermitirSelecionarReboquePedido)
            {
                if (pedido.VeiculoTracao != null)
                {
                    placas = pedido.VeiculoTracao.Placa;

                    if (pedido.VeiculoTracao.ModeloVeicularCarga != null)
                        placas += " (" + pedido.VeiculoTracao.ModeloVeicularCarga.Descricao + ")";

                    if (ConfiguracaoEmbarcador.ConcatenarFrotaPlaca)
                        placas += $" ({pedido.VeiculoTracao.NumeroFrota})";
                }

                if (pedido.VeiculoTracao != null && !pedido.VeiculoTracao.VeiculosVinculados.IsNullOrEmpty())
                {
                    placas += ", ";

                    var placasVinculadas = new List<string>();

                    foreach (var veiculo in pedido.VeiculoTracao.VeiculosVinculados)
                    {
                        string placaVeiculo = veiculo.Placa;

                        if (veiculo.ModeloVeicularCarga != null)
                            placaVeiculo += " (" + veiculo.ModeloVeicularCarga.Descricao + ")";

                        if (ConfiguracaoEmbarcador.ConcatenarFrotaPlaca)
                            placaVeiculo += $" ({veiculo.NumeroFrota})";

                        placasVinculadas.Add(placaVeiculo);
                    }

                    placas += string.Join(", ", placasVinculadas);
                }
            }
            else
            {
                Dominio.Entidades.Veiculo ultimoVeiculo = pedido.Veiculos.LastOrDefault();
                foreach (Dominio.Entidades.Veiculo veiculo in pedido.Veiculos)
                {
                    placas += veiculo.Placa;

                    if (veiculo.ModeloVeicularCarga != null)
                        placas += " (" + veiculo.ModeloVeicularCarga.Descricao + ")";

                    if (ultimoVeiculo.Codigo != veiculo.Codigo)
                        placas += ", ";

                }
            }

            return placas;
        }
        private string ConverterTipoCarroceria(string codigo)
        {
            switch (codigo)
            {
                case "00":
                    return "Não Aplicável";
                case "01":
                    return "Aberta";
                case "02":
                    return "Fechada/Baú";
                case "03":
                    return "Graneleira";
                case "04":
                    return "Porta Container";
                case "05":
                    return "Utilitário";
                case "06":
                    return "Sider";
                default:
                    return string.Empty;
            }
        }

        #endregion
    }
}
