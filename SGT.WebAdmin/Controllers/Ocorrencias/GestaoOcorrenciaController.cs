using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Ocorrencias
{
    [CustomAuthorize("Ocorrencias/GestaoOcorrencia")]
    public class GestaoOcorrenciaController : BaseController
    {
        #region Construtores

        public GestaoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(GridPesquisa());
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repositorioOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega ocorrencia = repositorioOcorrenciaColetaEntrega.BuscarPorCodigo(codigo, false);

                Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras repOcorrenciaSobras = new Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras(unitOfWork);
                List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> sobras = repOcorrenciaSobras.BuscarPorCargaEntrega(ocorrencia.CargaEntrega.Codigo);

                if (ocorrencia == null)
                    return new JsonpResult(false, "O registro não foi encontrado");

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Repositorio.Embarcador.Chamados.ChamadoAnexo repositorioChamadoAnexo = new Repositorio.Embarcador.Chamados.ChamadoAnexo(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Chamados.Chamado chamado = repositorioChamado.BuscarPorCargaEntrega(ocorrencia.CargaEntrega.Codigo);
                List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo> anexos = chamado != null ? repositorioChamadoAnexo.BuscarPorChamado(chamado.Codigo) : new List<Dominio.Entidades.Embarcador.Chamados.ChamadoAnexo>();

                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal repositorioOcorrenciaColetaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntregaNotaFiscal> ocorrenciaColetaEntregaNotaFiscal = repositorioOcorrenciaColetaEntregaNotaFiscal.BuscarPorOcorrenciaColetaEntrega(ocorrencia.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(ocorrencia.CargaEntrega.Codigo);

                return new JsonpResult(
                    new
                    {
                        Ocorrencia = new
                        {
                            ocorrencia.Codigo,
                            CodigoChamado = chamado?.Codigo ?? 0,
                            SituacaoChamado = chamado?.Situacao,
                            Carga = new { Codigo = ocorrencia.CargaEntrega.Carga.Codigo, Descricao = ocorrencia.CargaEntrega.Carga.CodigoCargaEmbarcador },
                            CargaEntrega = new { Codigo = ocorrencia.CargaEntrega.Codigo, Descricao = ocorrencia.CargaEntrega.Codigo.ToString() },
                            TipoOcorrencia = new { Codigo = ocorrencia.TipoDeOcorrencia.Codigo, Descricao = ocorrencia.TipoDeOcorrencia.Descricao },
                            ocorrencia.TipoDeOcorrencia.OcorrenciaPorNotaFiscal,
                            TiposCausadoresOcorrencia = new { Codigo = ocorrencia.TiposCausadoresOcorrencia?.Codigo, Descricao = ocorrencia.TiposCausadoresOcorrencia?.Descricao },
                            PermiteSobras = ocorrencia.TipoDeOcorrencia.PermiteInformarSobras,
                            TipoDevolucao = ocorrencia.CargaEntrega.TipoDevolucao,
                            Observacoes = ocorrencia.CargaEntrega.Observacao,
                            Sobras = (from sobra in sobras
                                      select new
                                      {
                                          sobra.Codigo,
                                          sobra.CodigoSobra,
                                          sobra.QuantidadeSobra
                                      }).ToList(),
                            Anexos = (from anexo in anexos
                                      select new
                                      {
                                          anexo.Codigo,
                                          anexo.Descricao,
                                          anexo.NomeArquivo
                                      }).ToList()
                        },
                        Atendimento = new
                        {
                            Codigo = chamado?.Codigo ?? 0,
                            Numero = chamado?.Numero ?? 0,
                            Empresa = chamado?.Empresa?.Descricao ?? "",
                            Carga = chamado?.Carga?.CodigoCargaEmbarcador ?? "",
                            Observacoes = chamado?.Observacao ?? "",
                            SituacaoDescricao = chamado?.DescricaoSituacao ?? "",
                            Situacao = chamado?.Situacao ?? null,
                            NotasFiscais = string.Join(", ",
                                (from obj in cargaEntregaNotasFiscais
                                 where (obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial ||
                                 obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida)
                                 select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero
                                ).ToList()
                            )
                        },
                        NotasFiscais = (from obj in ocorrenciaColetaEntregaNotaFiscal
                                        select new
                                        {
                                            obj.XMLNotaFiscal.Codigo,
                                            obj.XMLNotaFiscal.Numero,
                                            Emitente = obj.XMLNotaFiscal.Emitente.Descricao,
                                            DataEmissao = obj.XMLNotaFiscal.DataEmissao.ToDateString(),
                                        }).ToList()
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

        public async Task<IActionResult> AdicionarOcorrencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                DateTime data = DateTime.Now;
                int codigoTipoCausadorOcorrencia = Request.GetIntParam("TiposCausadoresOcorrencia");

                Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);
                Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia repositorioTiposCausadoresOcorrencia = new Repositorio.Embarcador.Ocorrencias.TiposCausadoresOcorrencia(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros parametros = ObterParametrosOcorrencia(unitOfWork);
                Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(parametros.codigoCargaEntrega);
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repositorioTipoOcorrencia.BuscarPorCodigo(parametros.codigoMotivo);
                Dominio.Entidades.Embarcador.Ocorrencias.TiposCausadoresOcorrencia tiposCausadoresOcorrencia = codigoTipoCausadorOcorrencia > 0 ? repositorioTiposCausadoresOcorrencia.BuscarPorCodigo(codigoTipoCausadorOcorrencia, false) : null;

                OrigemSituacaoEntrega origem = OrigemSituacaoEntrega.UsuarioMultiEmbarcador;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                    origem = OrigemSituacaoEntrega.UsuarioPortalTransportador;

                if (tipoOcorrencia.UsadoParaMotivoRejeicaoColetaEntrega)
                {
                    Dominio.Entidades.Embarcador.Chamados.Chamado chamado;

                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.RejeitarEntrega(parametros, Auditado, unitOfWork, out chamado, TipoServicoMultisoftware, tiposCausadoresOcorrencia);

                    cargaEntrega.FinalizadaManualmente = true;
                    cargaEntrega.ResponsavelFinalizacaoManual = Usuario;

                    repositorioCargaEntrega.Atualizar(cargaEntrega);
                    Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaEntrega, $"Operador {Usuario.Descricao} rejeitou manualmente a entrega", unitOfWork);

                    if (chamado != null)
                    {
                        if (chamado.MotivoChamado.GerarCargaDevolucaoSeAprovado)
                        {
                            int codigoChamado = chamado.Codigo;
                            string stringConexao = unitOfWork.StringConexao;
                            Task t = Task.Factory.StartNew(() => { Servicos.Embarcador.Chamado.Chamado.EnviarEmailCargaDevolucao(codigoChamado, stringConexao); });
                        }

                        Servicos.Embarcador.Chamado.Chamado.NotificarChamadoAdicionadoOuAtualizado(chamado, unitOfWork);
                        new Servicos.Embarcador.Chamado.Chamado(unitOfWork).EnviarEmailChamadoAberto(chamado, unitOfWork);
                    }
                }
                else
                {
                    List<int> notasFiscais = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(Request.Params("NotasFiscais"));
                    Servicos.Embarcador.Carga.ControleEntrega.OcorrenciaEntrega.GerarOcorrenciaRejeicao(cargaEntrega, data, tipoOcorrencia, null, null, string.Empty, 0m, ConfiguracaoEmbarcador, TipoServicoMultisoftware, this.Cliente, origem, unitOfWork, Auditado, null, null, EventoColetaEntrega.Todos, null, null, null, tiposCausadoresOcorrencia, notasFiscais);
                }

                SalvarSobras(cargaEntrega, unitOfWork);

                cargaEntrega.Observacao = parametros.observacao;

                if (parametros.dataPrevisaoEntrega != null && parametros.dataPrevisaoEntrega != DateTime.MinValue)
                    cargaEntrega.DataPrevista = parametros.dataPrevisaoEntrega;

                repositorioCargaEntrega.Atualizar(cargaEntrega);
                Servicos.Embarcador.Carga.ControleEntrega.ControleEntrega.SincronizarEntregaOrigem(cargaEntrega, repositorioCargaEntrega, unitOfWork);

                Repositorio.Embarcador.Chamados.Chamado repositorioChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);
                Dominio.Entidades.Embarcador.Chamados.Chamado chamadoRetornar = repositorioChamado.BuscarPorCargaEntrega(cargaEntrega.Codigo);

                unitOfWork.CommitChanges();

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargaEntregaNotasFiscais = new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor)
                    cargaEntregaNotasFiscais = repositorioCargaEntregaNotaFiscal.BuscarPorCargaEntrega(cargaEntrega.Codigo);


                return new JsonpResult(new
                {
                    Codigo = chamadoRetornar?.Codigo ?? 0,
                    Atendimento = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.Fornecedor ?
                    new
                    {
                        Codigo = chamadoRetornar?.Codigo ?? 0,
                        Numero = chamadoRetornar?.Numero ?? 0,
                        Empresa = chamadoRetornar?.Empresa?.Descricao ?? "",
                        Carga = chamadoRetornar?.Carga?.CodigoCargaEmbarcador ?? "",
                        Observacoes = chamadoRetornar?.Observacao ?? "",
                        SituacaoDescricao = chamadoRetornar?.DescricaoSituacao ?? "",
                        Situacao = chamadoRetornar?.Situacao ?? null,
                        NotasFiscais = string.Join(", ",
                                (from obj in cargaEntregaNotasFiscais
                                 where (obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.DevolvidaParcial ||
                                 obj.PedidoXMLNotaFiscal.XMLNotaFiscal.SituacaoEntregaNotaFiscal == SituacaoNotaFiscal.Devolvida)
                                 select obj.PedidoXMLNotaFiscal.XMLNotaFiscal.Numero
                                ).ToList()
                            )
                    } : null,
                });
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar a ocorrência.");
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
                //if (exportarPesquisa && totalRegistros > 10000)
                //    return new JsonpResult(false, true, "A quantidade de registros para exportação não pode ser superior a 5000.");

                Models.Grid.Grid grid = GridPesquisa(true);
                byte[] arquivoBinario = grid.GerarExcel();

                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{grid.tituloExportacao}.{grid.extensaoCSV}");

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

        public async Task<IActionResult> SalvarDadosSobraConferencia()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras repGestaoOcorrenciaSobras = new Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras(unitOfWork);

                int codigoGestaoOcorrenciaSobra = Request.GetIntParam("CodigoGestaoOcorrenciaSobra");
                int quantidadeConferencia = Request.GetIntParam("QuantidadeConferencia");
                string observacao = Request.GetStringParam("Observacao");

                Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras gestaoSobra = repGestaoOcorrenciaSobras.BuscarPorCodigo(codigoGestaoOcorrenciaSobra, false);

                if (gestaoSobra == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                if (quantidadeConferencia < gestaoSobra.QuantidadeSobra && string.IsNullOrWhiteSpace(observacao))
                    return new JsonpResult(false, true, "Necessário informar a observação para sobras com divergência na quantidade.");

                gestaoSobra.ObservacaoConferencia = observacao;
                gestaoSobra.QuantidadeConferencia = quantidadeConferencia;

                repGestaoOcorrenciaSobras.Atualizar(gestaoSobra, Auditado);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao salvar os dados das sobras.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ObterPermissaoAlterarDataPrevisaoEntrega()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoTipoOcorrencia = Request.GetIntParam("CodigoTipoOcorrencia");
                int codigoEntrega = Request.GetIntParam("CodigoEntrega");

                dynamic retorno = null;

                if (codigoTipoOcorrencia > 0)
                {
                    Repositorio.TipoDeOcorrenciaDeCTe repositorioTipoOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                    Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repositorioCargaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork);

                    Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repositorioTipoOcorrencia.BuscarPorCodigo(codigoTipoOcorrencia);
                    Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega = repositorioCargaEntrega.BuscarPorCodigo(codigoEntrega);

                    if (tipoOcorrencia != null && tipoOcorrencia.PermitirAlterarDataPrevisaoEntrega)
                    {
                        retorno = new
                        {
                            PermiteAlterarDataEntrega = true,
                            DataPrevisaoEntregaAtual = cargaEntrega?.DataPrevista?.ToString("dd/MM/yyyy HH:mm") ?? ""
                        };
                    }

                }

                return new JsonpResult(retorno);
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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia()
            {
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe ? Empresa.Codigo : 0,
                CodigoCarga = Request.GetStringParam("NumeroCarga"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataLimite = Request.GetNullableDateTimeParam("DataFinal"),
                CodigoNotaFiscal = Request.GetIntParam("NotaFiscal")
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> ObterNotasFiscaisDevolver(dynamic itensDevolver, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega repositorioMotivoDevolucaoEntrega = new Repositorio.Embarcador.GestaoEntregas.MotivoDevolucaoEntrega(unitOfWork);
            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal> notasFiscais = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal>();

            foreach (dynamic notaFiscal in itensDevolver.NotasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal cargaEntregaNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.CargaEntregaNotaFiscal()
                {
                    Codigo = ((string)notaFiscal.Codigo).ToInt(),
                    DevolucaoParcial = ((string)notaFiscal.DevolucaoParcial).ToBool(),
                    DevolucaoTotal = ((string)notaFiscal.DevolucaoTotal).ToBool(),
                    Numero = ((string)notaFiscal.Numero).ToInt(),
                    MotivoDevolucaoEntrega = repositorioMotivoDevolucaoEntrega.BuscarPorCodigo(((string)notaFiscal.MotivoDevolucaoEntrega).ToInt()),
                    Produtos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>()
                };

                foreach (dynamic produto in notaFiscal.Produtos)
                    cargaEntregaNotaFiscal.Produtos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                    {
                        Protocolo = ((string)produto.Codigo).ToInt(),
                        QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal(),
                        ValorDevolucao = ((string)produto.ValorDevolucao).ToDecimal(),
                        NFDevolucao = ((string)produto.NFDevolucao).ToInt()
                    });

                notasFiscais.Add(cargaEntregaNotaFiscal);
            }

            return notasFiscais;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros ObterParametrosOcorrencia(Repositorio.UnitOfWork unitOfWork)
        {
            int codigoTipoOcorrencia = Request.GetIntParam("TipoOcorrencia");
            int codigoCargaEntrega = Request.GetIntParam("CargaEntrega");
            DateTime dataPrevisaoEntrega = Request.GetDateTimeParam("DataPrevisaoEntrega");

            dynamic itensDevolver = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ItensDevolver"));
            OrigemSituacaoEntrega origemSituacaoEntrega = (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador) ?
                                OrigemSituacaoEntrega.UsuarioMultiEmbarcador : OrigemSituacaoEntrega.UsuarioPortalTransportador;

            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.RejeitarEntregaParametros
            {
                codigoCargaEntrega = codigoCargaEntrega,
                codigoMotivo = codigoTipoOcorrencia,
                configuracao = ConfiguracaoEmbarcador,
                data = DateTime.Now,
                devolucaoParcial = Request.GetEnumParam("TipoDevolucao", TipoColetaEntregaDevolucao.Total) == TipoColetaEntregaDevolucao.Parcial,
                notasFiscais = ObterNotasFiscaisDevolver(itensDevolver, unitOfWork),
                observacao = Request.GetStringParam("Observacoes"),
                produtos = ObterProdutosDevolver(itensDevolver),
                tipoServicoMultisoftware = TipoServicoMultisoftware,
                usuario = this.Usuario,
                OrigemSituacaoEntrega = origemSituacaoEntrega,
                clienteMultisoftware = this.Cliente,
                dataPrevisaoEntrega = dataPrevisaoEntrega,
            };
        }

        private List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> ObterProdutosDevolver(dynamic itensDevolver)
        {

            List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto> listaProdutos = new List<Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto>();

            foreach (dynamic produto in itensDevolver.Produtos)
                listaProdutos.Add(new Dominio.ObjetosDeValor.Embarcador.Mobile.ControleEntrega.Produto()
                {
                    Protocolo = ((string)produto.Codigo).ToInt(),
                    QuantidadeDevolucao = ((string)produto.QuantidadeDevolucao).ToDecimal()
                });

            return listaProdutos;
        }

        private Models.Grid.Grid GridPesquisa(bool exportarPesquisa = false)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Sinalização Ocorrência", "SinalizacaoOcorrencia", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Carga", "NumeroCarga", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Entrega", "NumeroEntrega", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Nº Atendimento", "NumeroAtendimento", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação da Entrega", "Situacao", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Causador da Ocorrência", "TiposCausadoresOcorrencia", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data", "Data", 15, Models.Grid.Align.center, true);

                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Chamados.Chamado repChamado = new Repositorio.Embarcador.Chamados.Chamado(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                int totalRegistros = repOcorrenciaColetaEntrega.ContarConsulta(filtrosPesquisa);
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega> listaGrid = totalRegistros > 0 ? repOcorrenciaColetaEntrega.Consultar(filtrosPesquisa, parametrosConsulta) : new List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega>();
                List<Dominio.Entidades.Embarcador.Chamados.Chamado> chamados = listaGrid.Count > 0 ? repChamado.BuscarPorCargas(listaGrid.Select(obj => obj.CargaEntrega.Carga.Codigo).Distinct().ToList()) : new List<Dominio.Entidades.Embarcador.Chamados.Chamado>();

                var lista = (from obj in listaGrid
                             select new
                             {
                                 Codigo = obj.Codigo,
                                 SinalizacaoOcorrencia = obj.Codigo.ToString(),
                                 NumeroCarga = obj.CargaEntrega.Carga.CodigoCargaEmbarcador,
                                 NumeroEntrega = obj.CargaEntrega.Descricao,
                                 NumeroAtendimento = (from chamado in chamados where chamado.CargaEntrega != null && chamado.CargaEntrega.Codigo == obj.CargaEntrega.Codigo select chamado.Numero.ToString()).FirstOrDefault(),
                                 Situacao = obj.CargaEntrega.DescricaoSituacao,
                                 TiposCausadoresOcorrencia = obj.TiposCausadoresOcorrencia?.Descricao,
                                 Data = obj.DataOcorrencia.ToString("dd/MM/yyyy HH:mm")
                             }).ToList();

                grid.AdicionaRows(lista);
                grid.setarQuantidadeTotal(totalRegistros);

                if (ContarRegistrosExportacao(exportarPesquisa, totalRegistros))
                    throw new ControllerException("A quantidade de registros para exportação não pode ser superior a 10000.");
                else
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

        private static bool ContarRegistrosExportacao(bool exportarPesquisa, int totalRegistros)
        {
            if (exportarPesquisa && totalRegistros > 10000)
                return true;
            else
                return false;
        }

        private void SalvarSobras(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras repSobras = new Repositorio.Embarcador.Ocorrencias.GestaoOcorrenciaSobras(unidadeDeTrabalho);
            Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unidadeDeTrabalho);

            dynamic dynSobras = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Sobras"));

            List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> listaSobras = repSobras.BuscarPorCargaEntrega(cargaEntrega.Codigo);

            if (listaSobras.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (dynamic retorno in dynSobras)
                    if (retorno.Codigo != null)
                        codigos.Add(((string)retorno.Codigo).ToInt());

                List<Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras> listaDeletar = (from obj in listaSobras where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (int i = 0; i < listaDeletar.Count; i++)
                    repSobras.Deletar(listaDeletar[i], Auditado);
            }

            foreach (dynamic retorno in dynSobras)
            {
                int.TryParse((string)retorno.Codigo, out int codigoSobras);
                Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras sobras = codigoSobras > 0 ? repSobras.BuscarPorCodigo(codigoSobras, true) : null;

                if (sobras == null)
                {
                    sobras = new Dominio.Entidades.Embarcador.Ocorrencias.GestaoOcorrenciaSobras();
                    sobras.CargaEntrega = cargaEntrega;
                }

                sobras.CodigoSobra = retorno.CodigoSobra;
                sobras.QuantidadeSobra = int.Parse(retorno.QuantidadeSobra.ToString().Replace(".", ""));

                if (sobras.Codigo > 0)
                    repSobras.Atualizar(sobras, Auditado);
                else
                    repSobras.Inserir(sobras, Auditado);
            }
        }

        #endregion
    }
}
