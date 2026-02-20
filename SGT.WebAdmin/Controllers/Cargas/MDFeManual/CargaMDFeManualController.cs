using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System.Globalization;

namespace SGT.WebAdmin.Controllers.Cargas.MDFeManual
{
    [CustomAuthorize("Cargas/CargaMDFeManual", "Cargas/CargaMDFeAquaviarioManual")]
    public class CargaMDFeManualController : BaseController
    {
        #region Construtores

        public CargaMDFeManualController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracaoMagalogMDFe = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.MagalogMDFe);

                int codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, codigoCarga, numeroMDFe, codigoTransportador;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("CTe"), out numeroCTe);
                int.TryParse(Request.Params("MDFe"), out numeroMDFe);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);
                int codigoVeiculo = Request.GetIntParam("Veiculo");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoTransportador = this.Empresa.Codigo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual? situacao = null;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual situacaoAux;
                if (Enum.TryParse(Request.Params("Situacao"), out situacaoAux))
                    situacao = situacaoAux;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                if (tipoIntegracaoMagalogMDFe != null)
                    grid.AdicionarCabecalho("Protocolo", "Codigo", 10, Models.Grid.Align.left, true);
                else
                    grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 12, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> listaCargaMDFeManual = repCargaMDFeManual.Consultar(0, 0, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Rodoviario, codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countCargaMDFeManual = repCargaMDFeManual.ContarConsulta(0, 0, 0, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Rodoviario, codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, situacao);

                grid.setarQuantidadeTotal(countCargaMDFeManual);

                grid.AdicionaRows((from p in listaCargaMDFeManual
                                   select new
                                   {
                                       p.Codigo,
                                       Transportador = p.Empresa.Descricao,
                                       MDFe = string.Join(", ", p.MDFeManualMDFes.Select(o => o.MDFe.Numero)),
                                       Origem = p.Origem.DescricaoCidadeEstado,
                                       Destino = !p.UsarListaDestinos() ? p.Destino?.DescricaoCidadeEstado ?? "" : string.Join(",", (from obj in p.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList()),
                                       Motorista = p.Motoristas != null && p.Motoristas.Count > 0 ? string.Join(", ", p.Motoristas.Select(o => o.Descricao)) : "",
                                       Veiculo = p.Veiculo != null ? p.Veiculo.Placa + (p.Reboques.Count > 0 ? (", " + string.Join(", ", p.Reboques.Select(o => o.Placa))) : string.Empty) : "",
                                       Situacao = p.DescricaoSituacao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> PesquisaParaCancelamento()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, codigoCarga, numeroMDFe, codigoTransportador;
                int.TryParse(Request.Params("Origem"), out codigoOrigem);
                int.TryParse(Request.Params("Destino"), out codigoDestino);
                int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
                int.TryParse(Request.Params("Motorista"), out codigoMotorista);
                int.TryParse(Request.Params("Carga"), out codigoCarga);
                int.TryParse(Request.Params("CTe"), out numeroCTe);
                int.TryParse(Request.Params("MDFe"), out numeroMDFe);
                int.TryParse(Request.Params("Empresa"), out codigoTransportador);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoTransportador = this.Empresa.Codigo;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descricao", false);
                grid.AdicionarCabecalho("MDF-e", "MDFe", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Origem", "Origem", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Destino", "Destino", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Motorista", "Motorista", 12, Models.Grid.Align.left, true);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                    grid.AdicionarCabecalho("Transportador", "Transportador", 20, Models.Grid.Align.left, false);

                grid.AdicionarCabecalho("Veículo", "Veiculo", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Situação", "Situacao", 10, Models.Grid.Align.left, true);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual> listaCargaMDFeManual = repCargaMDFeManual.ConsultarCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Todos, codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                int countCargaMDFeManual = repCargaMDFeManual.ContarConsultaCancelamento(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Todos, codigoVeiculo, codigoMotorista, codigoOrigem, codigoDestino, numeroCTe, numeroMDFe, codigoCarga, codigoTransportador);

                grid.setarQuantidadeTotal(countCargaMDFeManual);

                grid.AdicionaRows((from p in listaCargaMDFeManual
                                   select new
                                   {
                                       p.Codigo,
                                       Transportador = p.Empresa.Descricao,
                                       Descricao = string.Join(", ", p.MDFeManualMDFes.Select(o => o.MDFe.Numero)) + " - " + p.Origem.DescricaoCidadeEstado + " até " + (!p.UsarListaDestinos() ? p.Destino?.DescricaoCidadeEstado ?? "" : string.Join(",", (from obj in p.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList())),
                                       MDFe = string.Join(", ", p.MDFeManualMDFes.Select(o => o.MDFe.Numero)),
                                       Origem = p.Origem.DescricaoCidadeEstado,
                                       Destino = !p.UsarListaDestinos() ? p.Destino?.DescricaoCidadeEstado ?? "" : string.Join(",", (from obj in p.Destinos orderby obj.Ordem select obj.Localidade.DescricaoCidadeEstado).ToList()),
                                       Motorista = p.Motoristas != null && p.Motoristas.Count > 0 ? string.Join(", ", p.Motoristas.Select(o => o.Descricao)) : "",
                                       Veiculo = p.Veiculo != null ? p.Veiculo.Placa + (p.Reboques.Count > 0 ? (", " + string.Join(", ", p.Reboques.Select(o => o.Placa))) : string.Empty) : "",
                                       Situacao = p.DescricaoSituacao
                                   }).ToList());

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Salvar()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string erro = string.Empty;

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = null;

                if (!SalvarMDFeManual(out erro, ref cargaMDFeManual, false, unidadeTrabalho))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, null, "Salvou dados do MDF-e", unidadeTrabalho);

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao salvar o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Emitir()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = codigo > 0 ? repCargaMDFeManual.BuscarPorCodigo(codigo) : null;

                string erro = string.Empty;
                if (!SalvarMDFeManual(out erro, ref cargaMDFeManual, true, unidadeTrabalho))
                    return new JsonpResult(false, true, erro);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaMDFeManual, null, "Solicitou emissão do MDF-e", unidadeTrabalho);

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                unidadeTrabalho.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao emitir o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unidadeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigo);

                if (cargaMDFeManual == null)
                    return new JsonpResult(false, true, "MDF-e manual não encontrado.");

                return new JsonpResult(ObterDetalhesMDFeManual(cargaMDFeManual, unidadeTrabalho));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, false, "Ocorreu uma falha ao buscar o MDF-e manual.");
            }
            finally
            {
                unidadeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private dynamic ObterDetalhesMDFeManual(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Servicos.Embarcador.Carga.CargaDadosSumarizados serCargaDadosSumarizados = new Servicos.Embarcador.Carga.CargaDadosSumarizados(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro repCargaMDFeManualSeguro = new Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT repCargaMDFeManualCIOT = new Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualNFe repCargaMDFeManualNFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualNFe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio repCargaMDFeManualValePedagio = new Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista reCargaMDFeManualMotorista = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursos = repPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista> PercentualMotoristas = reCargaMDFeManualMotorista.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> lacres = repLacre.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> cargaMDFeManualDestinos = repDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro> seguros = repCargaMDFeManualSeguro.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT> ciots = repCargaMDFeManualCIOT.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfes = repCargaMDFeManualNFe.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio> valesPedagios = repCargaMDFeManualValePedagio.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();



            if (cargaMDFeManual.UsarListaDestinos())
            {
                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino cargaMDFeManualDestino in cargaMDFeManualDestinos)
                    destinos.Add(cargaMDFeManualDestino.Localidade);
            }
            else if (cargaMDFeManual.Destino != null)
                destinos.Add(cargaMDFeManual.Destino);


            List<Dominio.Entidades.Localidade> localidadesMDFe = new List<Dominio.Entidades.Localidade>();
            localidadesMDFe.Add(cargaMDFeManual.Origem);

            foreach (Dominio.Entidades.Localidade destino in destinos)
                localidadesMDFe.Add(destino);

            var retorno = new
            {
                cargaMDFeManual.Codigo,
                cargaMDFeManual.Situacao,
                cargaMDFeManual.SituacaoCancelamento,
                cargaMDFeManual.UsarDadosCTe,
                cargaMDFeManual.UsarSeguroCTe,
                cargaMDFeManual.AdicionarMotoristaNaCarga,
                cargaMDFeManual.Agencia,
                cargaMDFeManual.Conta,
                cargaMDFeManual.ChavePIX,
                cargaMDFeManual.TipoPagamento,
                cargaMDFeManual.TipoChavePIX,
                cargaMDFeManual.CNPJInstituicaoPagamento,
                Empresa = new
                {
                    cargaMDFeManual.Empresa.Codigo,
                    cargaMDFeManual.Empresa.Descricao,
                    cargaMDFeManual.Empresa.EmpresaPropria,
                },
                CTes = (from obj in cargaMDFeManual.CTes
                        select new
                        {
                            obj.Codigo,
                            CodigoCTE = obj.CTe.Codigo,
                            Carga = obj.Carga.CodigoCargaEmbarcador,
                            Numero = obj.CTe.Numero + " - " + obj.CTe.Serie.Numero,
                            CodigoEmpresa = obj.CTe.Empresa.Codigo,
                            Serie = obj.CTe.Serie.Numero,
                            Notas = string.Join(", ", obj.CTe.XMLNotaFiscais.Select(o => o.Numero.ToString())),
                            Remetente = obj.CTe.Remetente != null ? obj.CTe.Remetente.Nome + "(" + obj.CTe.Remetente.CPF_CNPJ_Formatado + ")" : string.Empty,
                            Destinatario = obj.CTe.Destinatario != null ? obj.CTe.Destinatario.Nome + "(" + obj.CTe.Destinatario.CPF_CNPJ_Formatado + ")" : string.Empty,
                            Destino = obj.CTe.LocalidadeTerminoPrestacao.DescricaoCidadeEstado,
                            ValorFrete = obj.CTe.ValorAReceber.ToString("n2"),
                        }).ToList(),
                Cargas = (from obj in cargaMDFeManual.Cargas
                          select new
                          {
                              Codigo = obj.Codigo,
                              CodigoCargaEmbarcador = obj.CodigoCargaEmbarcador,
                              ExigeConfirmacaoTracao = obj.TipoOperacao?.ExigePlacaTracao ?? false,
                              NumeroReboques = obj.ModeloVeicularCarga?.NumeroReboques ?? 0,
                              Filial = obj.Filial != null ? obj.Filial.Descricao : "",
                              OrigemDestino = serCargaDadosSumarizados.ObterOrigemDestinos(obj, false, TipoServicoMultisoftware),
                              Transportador = obj.Empresa != null ? obj.Empresa.RazaoSocial + " (" + obj.Empresa.Localidade.DescricaoCidadeEstado + " )" : string.Empty,
                              Veiculo = obj.Veiculo != null ? obj.Veiculo.Placa : "",
                              DataCarregamento = obj.DataCarregamentoCarga.HasValue ? obj.DataCarregamentoCarga.Value.ToString("dd/MM/yyyy") : "",
                              NumeroCTes = obj.NumerosCTes
                          }).ToList(),
                NFes = (from obj in nfes
                        select new
                        {
                            Codigo = obj.Codigo,
                            obj.Chave
                        }).ToList(),
                ExigeConfirmacaoTracao = cargaMDFeManual.Cargas.Any(c => c.TipoOperacao?.ExigePlacaTracao ?? false),
                Destino = cargaMDFeManual.Destino != null ? new
                {
                    Codigo = cargaMDFeManual.Destino.Codigo,
                    Descricao = cargaMDFeManual.Destino.DescricaoCidadeEstado
                } : new { Codigo = 0, Descricao = "" },
                Origem = new
                {
                    Codigo = cargaMDFeManual.Origem.Codigo,
                    Descricao = cargaMDFeManual.Origem.DescricaoCidadeEstado
                },
                Veiculo = new
                {
                    Codigo = cargaMDFeManual.Veiculo.Codigo,
                    Descricao = cargaMDFeManual.Veiculo.Placa,
                    ModeloVeicularCarga = cargaMDFeManual.Veiculo.ModeloVeicularCarga?.Descricao ?? string.Empty,
                    NumeroFrota = cargaMDFeManual.Veiculo.NumeroFrota ?? string.Empty,
                    Reboques = (from obj in cargaMDFeManual.Reboques
                                select new
                                {
                                    Codigo = obj.Codigo,
                                    Descricao = obj.Placa,
                                    ModeloVeicularCarga = obj.ModeloVeicularCarga?.Descricao ?? "",
                                    NumeroFrota = obj.NumeroFrota
                                }).ToList()
                },
                Motoristas = (from obj in cargaMDFeManual.Motoristas
                              select new
                              {
                                  obj.Codigo,
                                  obj.Descricao,
                                  PercentualExecucao = (PercentualMotoristas.Where(o => o.Motorista.Codigo == obj.Codigo).FirstOrDefault())?.PercentualExecucao?.ToString("n2") ?? null

                              }).ToList(),
                Lacres = (from obj in lacres
                          select new
                          {
                              Codigo = obj.Codigo,
                              Numero = obj.Numero
                          }).ToList(),
                CIOT = (from obj in ciots
                        select new
                        {
                            Codigo = obj.Codigo,
                            NumeroCIOT = obj.NumeroCIOT,
                            ResponsavelCIOT = obj.Responsavel,
                            obj.ValorAdiantamento,
                            obj.ValorFrete,
                            obj.FormaPagamento,
                            obj.DataVencimento
                        }
                        ).ToList(),
                Seguro = (from obj in seguros
                          select new
                          {
                              Codigo = obj.Codigo,
                              TipoSeguro = obj.TipoResponsavel,
                              CNPJSeguradoraSeguro = obj.CNPJSeguradora,
                              NomeSeguradoraSeguro = obj.NomeSeguradora,
                              NomeResponsavelSeguro = obj.Responsavel,
                              ApoliceSeguro = obj.NumeroApolice,
                              AverbacaoSeguro = obj.NumeroAverbacao
                          }
                        ).ToList(),
                ValePedagio = (from obj in valesPedagios
                               select new
                               {
                                   Codigo = obj.Codigo,
                                   FornecedorValePedagio = obj.CNPJFornecedor,
                                   ResponsavelValePedagio = obj.CNPJResponsavel,
                                   ComprovanteValePedagio = obj.NumeroComprovante,
                                   ValorValePedagio = obj.ValorValePedagio.ToString("n2")
                               }
                        ).ToList(),
                Observacao = new
                {
                    ObservacaoFisco = cargaMDFeManual.ObservacaoFisco,
                    ObservacaoContribuinte = cargaMDFeManual.ObservacaoContribuinte
                },
                Percurso = new
                {
                    EstadoOrigem = cargaMDFeManual.Origem.Estado.Sigla,
                    EstadoDestino = destinos.LastOrDefault()?.Estado?.Sigla ?? cargaMDFeManual.Origem.Estado.Sigla,
                    EstadosOrigensDestinos = (from obj in destinos
                                              select new
                                              {
                                                  Codigo = 0,
                                                  Origem = new { Estado = cargaMDFeManual.Origem.Estado.Sigla, Cidade = cargaMDFeManual.Origem.Descricao },
                                                  Destino = new { Estado = obj.Estado.Sigla, Cidade = obj.Descricao }
                                              }).ToList(),
                    passagens = percursos.Select(o => new Dominio.ObjetosDeValor.Embarcador.Logistica.Passagem() { Posicao = o.Ordem, Sigla = o.Estado.Sigla }).OrderBy(o => o.Posicao).ToList(),
                    rotasParaMDFe = (from obj in localidadesMDFe
                                     select new
                                     {
                                         Estado = obj.Estado.Sigla,
                                         Cidade = obj.Descricao,
                                         IBGE = obj.CodigoIBGE
                                     }).ToList()
                },
                Destinos = (from obj in cargaMDFeManualDestinos
                            select new
                            {
                                Codigo = obj.Localidade.Codigo,
                                Descricao = obj.Localidade.DescricaoCidadeEstado,
                                Posicao = obj.Ordem
                            }).ToList()
            };

            return retorno;
        }

        private bool SalvarMDFeManual(out string erro, ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, bool emitirMDFe, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            int codigo, codigoOrigem, codigoDestino, codigoMotorista, codigoVeiculo, codigoEmpresa;
            int.TryParse(Request.Params("Codigo"), out codigo);
            int.TryParse(Request.Params("Origem"), out codigoOrigem);
            int.TryParse(Request.Params("Destino"), out codigoDestino);
            int.TryParse(Request.Params("Veiculo"), out codigoVeiculo);
            int.TryParse(Request.Params("Motorista"), out codigoMotorista);
            int.TryParse(Request.Params("Empresa"), out codigoEmpresa);

            dynamic informacoesBancarias = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("InformacoesBancarias"));

            TipoPagamentoMDFe? tipoPagamentoMDFe = null;
            TipoChavePix? tipoChavePIX = null;

            if (!string.IsNullOrWhiteSpace((string)informacoesBancarias.TipoPagamento))
                tipoPagamentoMDFe = (TipoPagamentoMDFe)Enum.Parse(typeof(TipoPagamentoMDFe), (string)informacoesBancarias.TipoPagamento);

            if (!string.IsNullOrWhiteSpace((string)informacoesBancarias.TipoChavePIX))
                tipoChavePIX = (TipoChavePix)Enum.Parse(typeof(TipoChavePix), (string)informacoesBancarias.TipoChavePIX);

            string conta = (string)informacoesBancarias.Conta;
            string agencia = (string)informacoesBancarias.Agencia;
            string chavePIX = (string)informacoesBancarias.ChavePIX;
            string cnpjInstituicaoPagamento = (string)informacoesBancarias.CNPJInstituicaoPagamento;

            bool usarDadosCTe = false;
            bool.TryParse(Request.Params("UsarDadosCTe"), out usarDadosCTe);

            bool usarSeguroCTe = false;
            bool.TryParse(Request.Params("UsarSeguroCTe"), out usarSeguroCTe);

            bool adicionarMotoristaNaCarga = false;
            bool.TryParse(Request.Params("AdicionarMotoristaNaCarga"), out adicionarMotoristaNaCarga);


            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                codigoEmpresa = this.Empresa.Codigo;

            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualNFe repCargaMDFeManualNFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualNFe(unidadeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista repositorioConfiguracaoComissaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista(unidadeTrabalho);

            unidadeTrabalho.Start();

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoComissaoMotorista configuracaoComissaoMotorista = repositorioConfiguracaoComissaoMotorista.BuscarConfiguracaoPadrao();
            cargaMDFeManual = codigo > 0 ? repCargaMDFeManual.BuscarPorCodigo(codigo) : null;

            if (cargaMDFeManual == null)
            {
                cargaMDFeManual = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual();
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmDigitacao;
            }

            if (cargaMDFeManual.MDFeManualMDFes != null && cargaMDFeManual.MDFeManualMDFes.Count > 0)
            {
                erro = "Não é possível salvar as informações pois o MDF-e já foi gerado.";
                unidadeTrabalho.Rollback();
                return false;
            }

            cargaMDFeManual.TipoModalMDFe = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModalMDFe.Rodoviario;
            cargaMDFeManual.Origem = repLocalidade.BuscarPorCodigo(codigoOrigem);
            cargaMDFeManual.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            cargaMDFeManual.UsarDadosCTe = !(cargaMDFeManual.Empresa?.EmpresaPropria ?? false) && usarDadosCTe;
            cargaMDFeManual.UsarSeguroCTe = usarSeguroCTe;
            cargaMDFeManual.AdicionarMotoristaNaCarga = (configuracaoGeral?.PermitirAdicionarMotoristaCargaMDFeManual ?? false) ? adicionarMotoristaNaCarga : false;
            cargaMDFeManual.TipoPagamento = tipoPagamentoMDFe;
            cargaMDFeManual.TipoChavePIX = tipoChavePIX;
            cargaMDFeManual.Conta = conta;
            cargaMDFeManual.Agencia = agencia;
            cargaMDFeManual.ChavePIX = chavePIX;
            cargaMDFeManual.CNPJInstituicaoPagamento = cnpjInstituicaoPagamento;

            if (!cargaMDFeManual.UsarListaDestinos())
                cargaMDFeManual.Destino = repLocalidade.BuscarPorCodigo(codigoDestino);
            else
                cargaMDFeManual.Destino = null;

            SetarMotoristas(cargaMDFeManual, unidadeTrabalho);


            SalvarObservacao(ref cargaMDFeManual, unidadeTrabalho);

            if (cargaMDFeManual.Codigo > 0)
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
            else
                repCargaMDFeManual.Inserir(cargaMDFeManual);

            erro = SalvarCargas(ref cargaMDFeManual, unidadeTrabalho);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                unidadeTrabalho.Rollback();
                return false;
            }

            erro = SalvarCTes(ref cargaMDFeManual, unidadeTrabalho);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                unidadeTrabalho.Rollback();
                return false;
            }

            erro = SalvarNFes(ref cargaMDFeManual, unidadeTrabalho);
            if (!string.IsNullOrWhiteSpace(erro))
            {
                unidadeTrabalho.Rollback();
                return false;
            }



            SalvarVeiculos(ref cargaMDFeManual, unidadeTrabalho);
            SalvarDestinos(ref cargaMDFeManual, unidadeTrabalho);
            SalvarPercurso(ref cargaMDFeManual, unidadeTrabalho);
            SalvarLacres(ref cargaMDFeManual, unidadeTrabalho);
            SalvarCIOT(ref cargaMDFeManual, unidadeTrabalho);
            SalvarSeguro(ref cargaMDFeManual, unidadeTrabalho);
            SalvarValePedagio(ref cargaMDFeManual, unidadeTrabalho);

            if (configuracaoComissaoMotorista.UtilizaControlePercentualExecucao)
            {
                SetarPercentualMotoristas(cargaMDFeManual, unidadeTrabalho);
            }

            //cargaMDFeManual.Empresa = cargaMDFeManual.CTes.Select(o => o.CTe.Empresa).FirstOrDefault();
            repCargaMDFeManual.Atualizar(cargaMDFeManual);

            if ((cargaMDFeManual.CTes == null || cargaMDFeManual.CTes.Count <= 0) && (cargaMDFeManual.Cargas == null || cargaMDFeManual.Cargas.Count <= 0) && (!repCargaMDFeManualNFe.ContemPorCargaMDFeManual(cargaMDFeManual.Codigo)))
            {
                if (cargaMDFeManual.Empresa.EmpresaPropria)
                    erro = "É necessário adicionar ao menos uma Carga para salvar o MDF-e manual.";
                else
                    erro = "É necessário adicionar ao menos um CT-e, Carga ou NF-e para salvar o MDF-e manual.";
                unidadeTrabalho.Rollback();
                return false;
            }

            bool cteEmpresasDiferentes = false;
            if (cargaMDFeManual.CTes.Any(o => o.CTe.Empresa.Codigo != codigoEmpresa))
            {
                string raiz = (cargaMDFeManual.Empresa.CNPJ_SemFormato).Remove(8, 6);
                if (cargaMDFeManual.CTes.Any(o => !o.CTe.Empresa.CNPJ.Contains(raiz)))
                    cteEmpresasDiferentes = true;
            }

            if (cteEmpresasDiferentes)
            {
                erro = "É necessário selecionar os CT-es de ";

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    erro += " uma única Empresa/Filial ";
                else
                    erro += " um único Transportador ";

                erro += "para realizar a geração do MDF-e manual.";

                unidadeTrabalho.Rollback();

                return false;
            }

            Dominio.Entidades.Embarcador.Cargas.Carga carga = cargaMDFeManual.Cargas?.FirstOrDefault();

            if (carga == null && cargaMDFeManual.CTes?.Count > 0)
            {
                carga = cargaMDFeManual.CTes.First().Carga;
            }

            if ((carga?.TipoOperacao?.ValidarMotoristaTeleriscoAoConfirmarTransportador ?? false) && (carga?.Empresa?.ValidarMotoristaTeleriscoAoConfirmarTransportador ?? false))
            {
                string retornoTelerisco = string.Empty;

                bool falhaIntegracaoTelerisco = false;
                string protocolo = string.Empty;
                foreach (Dominio.Entidades.Usuario motorista in cargaMDFeManual.Motoristas)
                {
                    retornoTelerisco = Servicos.Embarcador.Transportadores.Motorista.ConsultarMotoristaTelerisco(motorista, carga.Filial != null ? carga.Filial : null, carga.DataCarregamentoCarga.HasValue ? carga.DataCarregamentoCarga.Value : DateTime.MinValue, ref falhaIntegracaoTelerisco, ref protocolo, TipoServicoMultisoftware, unidadeTrabalho, carga?.Veiculo?.Placa);
                    if (falhaIntegracaoTelerisco)
                        break;
                }


                if (falhaIntegracaoTelerisco)
                {
                    unidadeTrabalho.Rollback();

                    erro = "Consulta motorista Telerisco: " + retornoTelerisco;
                    return false;
                }
            }

            if (cargaMDFeManual.AdicionarMotoristaNaCarga && carga != null)
                AdicionarMotoristaNaCarga(cargaMDFeManual, carga, unidadeTrabalho);

            unidadeTrabalho.CommitChanges();

            if (emitirMDFe)
            {
                if (!GerarMDFe(out erro, cargaMDFeManual.Codigo, unidadeTrabalho))
                    return false;
            }

            erro = string.Empty;
            return true;
        }

        private void SalvarVeiculos(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unidadeTrabalho);

            cargaMDFeManual.Veiculo = repVeiculo.BuscarPorCodigo(Request.GetIntParam("Veiculo"));
            bool especificarReboques = cargaMDFeManual.Cargas != null ? cargaMDFeManual.Cargas.Any(c => c.TipoOperacao?.ExigePlacaTracao ?? false) : false;

            if (cargaMDFeManual.Reboques == null)
                cargaMDFeManual.Reboques = new List<Dominio.Entidades.Veiculo>();

            cargaMDFeManual.Reboques.Clear();

            if (!especificarReboques)
            {
                if (cargaMDFeManual.Veiculo.TipoVeiculo == "1")
                {
                    if (cargaMDFeManual.Veiculo.VeiculosTracao != null && cargaMDFeManual.Veiculo.VeiculosTracao.Count > 0)
                    {
                        Dominio.Entidades.Veiculo tracao = (from obj in cargaMDFeManual.Veiculo.VeiculosTracao where obj.Ativo select obj).FirstOrDefault();

                        if (tracao != null)
                            cargaMDFeManual.Veiculo = tracao;
                    }
                }

                foreach (Dominio.Entidades.Veiculo veiculoVinculado in cargaMDFeManual.Veiculo.VeiculosVinculados)
                    cargaMDFeManual.Reboques.Add(veiculoVinculado);
            }
            else
            {
                Dominio.Entidades.Veiculo reboque = repVeiculo.BuscarPorCodigo(Request.GetIntParam("Reboque"));
                if (reboque != null)
                {
                    cargaMDFeManual.Reboques.Add(reboque);

                    Dominio.Entidades.Veiculo segundoReboque = repVeiculo.BuscarPorCodigo(Request.GetIntParam("SegundoReboque"));
                    if (segundoReboque != null)
                        cargaMDFeManual.Reboques.Add(segundoReboque);
                }
            }
        }

        private void SalvarDestinos(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeTrabalho);

            dynamic dynDestinos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Destinos"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> destinosExistentes = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (destinosExistentes.Count > 0)
            {
                List<int> codigosDestinos = new List<int>();

                foreach (var dynDestino in dynDestinos)
                    codigosDestinos.Add((int)dynDestino.Codigo);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> destinoDeletar = (from obj in destinosExistentes where !codigosDestinos.Contains(obj.Localidade.Codigo) select obj).ToList();

                for (var i = 0; i < destinoDeletar.Count; i++)
                    repCargaMDFeManualDestino.Deletar(destinoDeletar[i]);
            }

            foreach (var dynDestino in dynDestinos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino destino = repCargaMDFeManualDestino.BuscarPorLocalidadeECargaMDFeManual(cargaMDFeManual.Codigo, (int)dynDestino.Codigo);

                if (destino == null)
                    destino = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino();

                destino.CargaMDFeManual = cargaMDFeManual;
                destino.Localidade = repLocalidade.BuscarPorCodigo((int)dynDestino.Codigo);
                destino.Ordem = (int)dynDestino.Posicao;

                if (destino.Codigo > 0)
                    repCargaMDFeManualDestino.Atualizar(destino);
                else
                    repCargaMDFeManualDestino.Inserir(destino);
            }
        }

        private void SalvarPercurso(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Estado repEstado = new Repositorio.Estado(unidadeTrabalho);

            dynamic percursos = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Percurso"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursosExistentes = repPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (percursosExistentes.Count > 0)
            {
                List<string> estados = new List<string>();

                foreach (var percurso in percursos)
                    estados.Add((string)percurso.Estado);

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursosDeletar = (from obj in percursosExistentes where !estados.Contains(obj.Estado.Sigla) select obj).ToList();

                for (var i = 0; i < percursosDeletar.Count; i++)
                    repPercurso.Deletar(percursosDeletar[i]);
            }

            foreach (var percurso in percursos)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso perc = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso
                {
                    CargaMDFeManual = cargaMDFeManual,
                    Estado = repEstado.BuscarPorSigla((string)percurso.Sigla),
                    Ordem = (int)percurso.Posicao
                };

                if (perc.Codigo > 0)
                    repPercurso.Atualizar(perc);
                else
                    repPercurso.Inserir(perc);
            }
        }

        private void SalvarLacres(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);

            dynamic lacres = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Lacres"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> lacresExistentes = repLacre.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (lacresExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var lacre in lacres)
                {
                    int codigo;
                    int.TryParse((string)lacre.Codigo, out codigo);

                    codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> lacresDeletar = (from obj in lacresExistentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < lacresDeletar.Count; i++)
                    repLacre.Deletar(lacresDeletar[i]);
            }

            foreach (var lacre in lacres)
            {
                int codigo;
                int.TryParse((string)lacre.Codigo, out codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre lac = repLacre.BuscarPorCodigo(codigo);

                if (lac == null)
                    lac = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre();

                lac.CargaMDFeManual = cargaMDFeManual;
                lac.Numero = (string)lacre.Numero;

                if (lac.Codigo > 0)
                    repLacre.Atualizar(lac);
                else
                    repLacre.Inserir(lac);
            }
        }

        private void SetarMotoristas(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

            dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));

            cargaMDFeManual.Motoristas = new List<Dominio.Entidades.Usuario>();

            foreach (var motorista in motoristas)
            {
                Dominio.Entidades.Usuario motora = repMotorista.BuscarPorCodigo((int)motorista.Codigo);

                cargaMDFeManual.Motoristas.Add(motora);
            }
        }

        private void SetarPercentualMotoristas(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista reCargaMDFeManualMotorista = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista(unitOfWork);
            Repositorio.Usuario repMotorista = new Repositorio.Usuario(unitOfWork);

            dynamic motoristas = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Motoristas"));
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista> PercentualMotoristasExistentes = reCargaMDFeManualMotorista.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (motoristas.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var motorista in motoristas)
                {
                    int codigo;
                    int.TryParse((string)motorista.Codigo, out codigo);

                    codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista> motoristasDeletar = (from obj in PercentualMotoristasExistentes where !codigos.Contains(obj.Motorista.Codigo) select obj).ToList();

                for (var i = 0; i < motoristasDeletar.Count; i++)
                    reCargaMDFeManualMotorista.Deletar(motoristasDeletar[i]);
            }



            foreach (var motorista in motoristas)
            {
                int codigo;
                int.TryParse((string)motorista.Codigo, out codigo);
                decimal PercentualExecucao;
                decimal.TryParse((string)motorista.PercentualExecucao, out PercentualExecucao);

                Dominio.Entidades.Usuario motora = repMotorista.BuscarPorCodigo(codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista mot = reCargaMDFeManualMotorista.BuscarPorCargaMDFeManualMotorista(cargaMDFeManual.Codigo, motora.Codigo);

                if (mot == null)
                    mot = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista();

                mot.CargaMDFeManual = cargaMDFeManual;
                mot.Motorista = motora;
                mot.PercentualExecucao = PercentualExecucao;

                if (mot.Codigo > 0)
                    reCargaMDFeManualMotorista.Atualizar(mot);
                else
                    reCargaMDFeManualMotorista.Inserir(mot);
            }
        }

        //private void SalvarSeguro(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        //{
        //    Repositorio.MDFeSeguro repMDFeSeguro = new Repositorio.MDFeSeguro(unidadeTrabalho);

        //    dynamic seguros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Seguro"));

        //    List<Dominio.Entidades.MDFeSeguro> segurosExistentes = repMDFeSeguro.BuscarPorMDFe(cargaMDFeManual.MDFe.Codigo);

        //    if (segurosExistentes.Count > 0)
        //    {
        //        List<int> codigos = new List<int>();

        //        foreach (var seguro in seguros)
        //        {
        //            int codigo;
        //            int.TryParse((string)seguro.Codigo, out codigo);

        //            codigos.Add(codigo);
        //        }

        //        List<Dominio.Entidades.MDFeSeguro> segurosDeletar = (from obj in segurosExistentes where !codigos.Contains(obj.Codigo) select obj).ToList();

        //        for (var i = 0; i < segurosDeletar.Count; i++)
        //            repMDFeSeguro.Deletar(segurosDeletar[i]);
        //    }

        //    if (!cargaMDFeManual.UsarSeguroCTe)
        //    {
        //        if (seguros != null && seguros.Count > 0)
        //        {
        //            foreach (var seguro in seguros)
        //            {
        //                int codigo;
        //                int.TryParse((string)seguro.Codigo, out codigo);

        //                Dominio.Entidades.MDFeSeguro seg = repMDFeSeguro.BuscarPorCodigo(codigo, cargaMDFeManual.MDFe.Codigo);

        //                if (seg == null)
        //                    seg = new Dominio.Entidades.MDFeSeguro();

        //                seg.MDFe = cargaMDFeManual.MDFe;
        //                seg.CNPJSeguradora = Utilidades.String.OnlyNumbers((string)seguro.CNPJSeguradoraSeguro);
        //                seg.NomeSeguradora = (string)seguro.NomeSeguradoraSeguro;
        //                if (!string.IsNullOrWhiteSpace(seg.NomeSeguradora) && seg.NomeSeguradora.Length > 30)
        //                    seg.NomeSeguradora = seg.NomeSeguradora.Substring(0, 29);

        //                seg.Responsavel = Utilidades.String.OnlyNumbers((string)seguro.NomeResponsavelSeguro);
        //                seg.NumeroApolice = (string)seguro.ApoliceSeguro;
        //                seg.NumeroAverbacao = (string)seguro.AverbacaoSeguro;
        //                Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoSeguro = Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;
        //                Enum.TryParse((string)seguro.TipoSeguro, out tipoSeguro);
        //                seg.TipoResponsavel = tipoSeguro;

        //                if (seg.Codigo > 0)
        //                    repMDFeSeguro.Atualizar(seg);
        //                else
        //                    repMDFeSeguro.Inserir(seg);
        //            }
        //        }
        //        else
        //        {
        //            Dominio.Entidades.MDFeSeguro mdfeSeguro = new Dominio.Entidades.MDFeSeguro();

        //            mdfeSeguro.MDFe = cargaMDFeManual.MDFe;
        //            mdfeSeguro.TipoResponsavel = cargaMDFeManual.MDFe.Empresa.Configuracao != null && cargaMDFeManual.MDFe.Empresa.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
        //                                         cargaMDFeManual.MDFe.Empresa.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante :
        //                                         cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro != Dominio.Enumeradores.TipoSeguro.Remetente ?
        //                                         cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.ResponsavelSeguro == Dominio.Enumeradores.TipoSeguro.Emitente_CTE ? Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Contratante : Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;

        //            mdfeSeguro.Responsavel = cargaMDFeManual.MDFe.Empresa.CNPJ;

        //            mdfeSeguro.CNPJSeguradora = cargaMDFeManual.MDFe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.Configuracao.CNPJSeguro) ? cargaMDFeManual.MDFe.Empresa.Configuracao.CNPJSeguro :
        //                                        cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.CNPJSeguro) ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.CNPJSeguro :
        //                                        cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? cargaMDFeManual.MDFe.Empresa.CNPJ : string.Empty;

        //            mdfeSeguro.NomeSeguradora = cargaMDFeManual.MDFe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.Configuracao.NomeSeguro) ? cargaMDFeManual.MDFe.Empresa.Configuracao.NomeSeguro.Length > 30 ? cargaMDFeManual.MDFe.Empresa.Configuracao.NomeSeguro.Substring(0, 30) : cargaMDFeManual.MDFe.Empresa.Configuracao.NomeSeguro :
        //                                        cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NomeSeguro) ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Length > 30 ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NomeSeguro.Substring(0, 30) : cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NomeSeguro :
        //                                        cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim ? cargaMDFeManual.MDFe.Empresa.RazaoSocial.Length > 30 ? cargaMDFeManual.MDFe.Empresa.RazaoSocial.Substring(0, 30) : cargaMDFeManual.MDFe.Empresa.RazaoSocial : string.Empty;

        //            mdfeSeguro.NumeroApolice = cargaMDFeManual.MDFe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.Configuracao.NumeroApoliceSeguro) ? cargaMDFeManual.MDFe.Empresa.Configuracao.NumeroApoliceSeguro.Length > 20 ? cargaMDFeManual.MDFe.Empresa.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : cargaMDFeManual.MDFe.Empresa.Configuracao.NumeroApoliceSeguro :
        //                                       cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro) ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Length > 20 ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro.Substring(0, 20) : cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NumeroApoliceSeguro : string.Empty;

        //            mdfeSeguro.NumeroAverbacao = cargaMDFeManual.MDFe.Empresa.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.Configuracao.AverbacaoSeguro) ? cargaMDFeManual.MDFe.Empresa.Configuracao.AverbacaoSeguro.Length > 40 ? cargaMDFeManual.MDFe.Empresa.Configuracao.AverbacaoSeguro.Substring(0, 40) : cargaMDFeManual.MDFe.Empresa.Configuracao.AverbacaoSeguro :
        //                                         cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && !string.IsNullOrWhiteSpace(cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro) ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Length > 40 ? cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro.Substring(0, 40) : cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.AverbacaoSeguro :
        //                                         cargaMDFeManual.MDFe.Empresa.Configuracao != null && !cargaMDFeManual.MDFe.Empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao != null && cargaMDFeManual.MDFe.Empresa.EmpresaPai.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim && !string.IsNullOrWhiteSpace(mdfeSeguro.NumeroApolice) ? mdfeSeguro.NumeroApolice : string.Empty;

        //            repMDFeSeguro.Inserir(mdfeSeguro);
        //        }
        //    }
        //}


        private void SalvarCIOT(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT repMDFeCIOT = new Repositorio.Embarcador.Cargas.CargaMDFeManualCIOT(unidadeTrabalho);

            dynamic ciots = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CIOT"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT> ciotsExistentes = repMDFeCIOT.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (ciotsExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var ciot in ciots)
                {
                    int codigo;
                    int.TryParse((string)ciot.Codigo, out codigo);

                    codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT> ciotsDeletar = (from obj in ciotsExistentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < ciotsDeletar.Count; i++)
                    repMDFeCIOT.Deletar(ciotsDeletar[i]);
            }

            foreach (var ciot in ciots)
            {
                int codigo;
                int.TryParse((string)ciot.Codigo, out codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT cio = repMDFeCIOT.BuscarPorCodigo(codigo);

                if (cio == null)
                    cio = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualCIOT();

                cio.CargaMDFeManual = cargaMDFeManual;
                cio.NumeroCIOT = (string)ciot.NumeroCIOT;
                cio.Responsavel = Utilidades.String.OnlyNumbers((string)ciot.ResponsavelCIOT);
                cio.ValorFrete = ciot.ValorFrete == null ? null : decimal.Parse(ciot.ValorFrete.ToString(), new CultureInfo("pt-BR"));
                cio.ValorAdiantamento = ciot.ValorAdiantamento == null ? null : decimal.Parse(ciot.ValorAdiantamento.ToString(), new CultureInfo("pt-BR"));
                cio.DataVencimento = string.IsNullOrWhiteSpace(ciot.DataVencimento.ToString()) ? null : DateTime.ParseExact(ciot.DataVencimento.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
                int formaPagamentoInt = 0;
                cio.FormaPagamento = (ciot.FormaPagamento != null && int.TryParse(ciot.FormaPagamento.ToString(), out formaPagamentoInt) && (formaPagamentoInt == 0 || formaPagamentoInt == 1))
                    ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento)formaPagamentoInt
                    : null;

                if (cio.Codigo > 0)
                    repMDFeCIOT.Atualizar(cio);
                else
                    repMDFeCIOT.Inserir(cio);
            }
        }

        private void SalvarSeguro(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro repCargaMDFeManualSeguro = new Repositorio.Embarcador.Cargas.CargaMDFeManualSeguro(unidadeTrabalho);

            dynamic seguros = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Seguro"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro> segurosExistentes = repCargaMDFeManualSeguro.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (segurosExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var seguro in seguros)
                {
                    int codigo;
                    int.TryParse((string)seguro.Codigo, out codigo);

                    codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro> segurosDeletar = (from obj in segurosExistentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < segurosDeletar.Count; i++)
                    repCargaMDFeManualSeguro.Deletar(segurosDeletar[i]);
            }

            if (!cargaMDFeManual.UsarSeguroCTe)
            {
                if (seguros != null && seguros.Count > 0)
                {
                    foreach (var seguro in seguros)
                    {
                        int codigo;
                        int.TryParse((string)seguro.Codigo, out codigo);

                        Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro seg = repCargaMDFeManualSeguro.BuscarPorCodigo(codigo);

                        if (seg == null)
                            seg = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualSeguro();

                        seg.CargaMDFeManual = cargaMDFeManual;
                        seg.CNPJSeguradora = Utilidades.String.OnlyNumbers((string)seguro.CNPJSeguradoraSeguro);
                        seg.NomeSeguradora = (string)seguro.NomeSeguradoraSeguro;
                        if (!string.IsNullOrWhiteSpace(seg.NomeSeguradora) && seg.NomeSeguradora.Length > 30)
                            seg.NomeSeguradora = seg.NomeSeguradora.Substring(0, 29);

                        seg.Responsavel = Utilidades.String.OnlyNumbers((string)seguro.NomeResponsavelSeguro);
                        seg.NumeroApolice = (string)seguro.ApoliceSeguro;
                        seg.NumeroAverbacao = (string)seguro.AverbacaoSeguro;
                        Dominio.Enumeradores.TipoResponsavelSeguroMDFe tipoSeguro = Dominio.Enumeradores.TipoResponsavelSeguroMDFe.Emitente;
                        Enum.TryParse((string)seguro.TipoSeguro, out tipoSeguro);
                        seg.TipoResponsavel = tipoSeguro;

                        if (seg.Codigo > 0)
                            repCargaMDFeManualSeguro.Atualizar(seg);
                        else
                            repCargaMDFeManualSeguro.Inserir(seg);
                    }
                }
            }
        }

        private void SalvarValePedagio(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio repValePedagioMDFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualValePedagio(unidadeTrabalho);

            dynamic vales = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ValePedagio"));

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio> valesExistentes = repValePedagioMDFe.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);

            if (valesExistentes.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var vale in vales)
                {
                    int codigo;
                    int.TryParse((string)vale.Codigo, out codigo);

                    codigos.Add(codigo);
                }

                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio> valesDeletar = (from obj in valesExistentes where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < valesDeletar.Count; i++)
                    repValePedagioMDFe.Deletar(valesDeletar[i]);
            }

            foreach (var vale in vales)
            {
                int codigo;
                int.TryParse((string)vale.Codigo, out codigo);

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio val = repValePedagioMDFe.BuscarPorCodigo(codigo);

                if (val == null)
                    val = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualValePedagio();

                val.CargaMDFeManual = cargaMDFeManual;
                val.CNPJFornecedor = Utilidades.String.OnlyNumbers((string)vale.FornecedorValePedagio);
                val.CNPJResponsavel = Utilidades.String.OnlyNumbers((string)vale.ResponsavelValePedagio);
                val.NumeroComprovante = (string)vale.ComprovanteValePedagio;
                decimal valor = 0;
                decimal.TryParse((string)vale.ValorValePedagio, out valor);
                val.ValorValePedagio = valor;

                if (val.Codigo > 0)
                    repValePedagioMDFe.Atualizar(val);
                else
                    repValePedagioMDFe.Inserir(val);
            }
        }

        private void SalvarObservacao(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            dynamic dynObservacao = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("Observacao"));
            if (dynObservacao == null)
                return;

            cargaMDFeManual.ObservacaoFisco = (string)dynObservacao.ObservacaoFisco;
            cargaMDFeManual.ObservacaoContribuinte = (string)dynObservacao.ObservacaoContribuinte;
        }

        private string SalvarNFes(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMDFeManualNFe repCargaMDFeManualNFe = new Repositorio.Embarcador.Cargas.CargaMDFeManualNFe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

            List<string> chaves = JsonConvert.DeserializeObject<List<string>>(Request.Params("NFes"));

            if (cargaMDFeManual.NFes != null)
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe> nfesDeletar = cargaMDFeManual.NFes.Where(o => !chaves.Contains(o.Chave)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe nfeDeletar in nfesDeletar)
                    repCargaMDFeManualNFe.Deletar(nfeDeletar);
            }

            foreach (string chave in chaves)
            {
                if (cargaMDFeManual.NFes == null || !cargaMDFeManual.NFes.Any(o => o.Chave == chave))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe nfe = new Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualNFe()
                    {
                        Chave = chave,
                        CargaMDFeManual = cargaMDFeManual
                    };
                    repCargaMDFeManualNFe.Inserir(nfe);
                }
            }

            return "";
        }

        private string SalvarCargas(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

            List<int> codigosCargas = JsonConvert.DeserializeObject<List<int>>(Request.Params("Cargas"));

            if (cargaMDFeManual.Cargas == null)
            {
                cargaMDFeManual.Cargas = new List<Dominio.Entidades.Embarcador.Cargas.Carga>();
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.Carga> cargasDeletar = cargaMDFeManual.Cargas.Where(o => !codigosCargas.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.Carga cargaDeletar in cargasDeletar)
                    cargaMDFeManual.Cargas.Remove(cargaDeletar);
            }

            foreach (int codigoCarga in codigosCargas)
            {
                if (!cargaMDFeManual.Cargas.Any(o => o.Codigo == codigoCarga))
                {
                    Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);
                    cargaMDFeManual.Cargas.Add(carga);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualExiste = repCargaMDFeManual.VerificarCargaMDFeManualComCarga(codigoCarga);

                if (cargaMDFeManualExiste != null && cargaMDFeManualExiste.Codigo != cargaMDFeManual.Codigo && cargaMDFeManualExiste.Origem.Codigo == cargaMDFeManual.Origem.Codigo)
                    return "Existem Cargas que estão alocadas em outro MDF-e manual";
            }

            if (codigosCargas.Count > 0)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMunicipioDescarregamentoMDFe = repMDFe.BuscarPorCargaEStatus(codigosCargas.ToArray(), new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Autorizado, Dominio.Enumeradores.StatusMDFe.Enviado });

                if (documentoMunicipioDescarregamentoMDFe != null)
                    return $"Existe um MDF-e [{documentoMunicipioDescarregamentoMDFe.MunicipioDescarregamento.MDFe.Chave}] autorizado para o CT-e [{documentoMunicipioDescarregamentoMDFe.CTe.Chave}].";
            }

            return "";
        }

        private string SalvarCTes(ref Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeTrabalho);

            List<int> codigosCargaCTe = JsonConvert.DeserializeObject<List<int>>(Request.Params("CTes"));

            if (cargaMDFeManual.CTes == null)
            {
                cargaMDFeManual.CTes = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
            }
            else
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> cargaCTesDeletar = cargaMDFeManual.CTes.Where(o => !codigosCargaCTe.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTeDeletar in cargaCTesDeletar)
                    cargaMDFeManual.CTes.Remove(cargaCTeDeletar);
            }

            foreach (int codigoCargaCTe in codigosCargaCTe)
            {
                if (!cargaMDFeManual.CTes.Any(o => o.Codigo == codigoCargaCTe))
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = repCargaCTe.BuscarPorCodigo(codigoCargaCTe);
                    cargaMDFeManual.CTes.Add(cargaCTe);
                }

                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManualExiste = repCargaMDFeManual.VerificarCargaMDFeManualComCte(codigoCargaCTe);
                if (cargaMDFeManualExiste != null && cargaMDFeManualExiste.Codigo != cargaMDFeManual.Codigo)
                    return "Existem CT-es que estão alocados em outro MDF-e manual";
            }

            if (codigosCargaCTe.Count > 0)
            {
                Dominio.Entidades.DocumentoMunicipioDescarregamentoMDFe documentoMunicipioDescarregamentoMDFe = repMDFe.BuscarPorCargaCTeEStatus(codigosCargaCTe.ToArray(), new Dominio.Enumeradores.StatusMDFe[] { Dominio.Enumeradores.StatusMDFe.Autorizado, Dominio.Enumeradores.StatusMDFe.Enviado });

                if (documentoMunicipioDescarregamentoMDFe != null)
                    return $"Existe um MDF-e [{documentoMunicipioDescarregamentoMDFe.MunicipioDescarregamento.MDFe.Chave}] autorizado para o CT-e [{(documentoMunicipioDescarregamentoMDFe.CTe?.Chave ?? documentoMunicipioDescarregamentoMDFe.Chave)}].";
            }

            return "";
        }

        private bool GerarMDFe(out string erro, int codigoMDFeManual, Repositorio.UnitOfWork unidadeTrabalho)
        {

            Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso repPercurso = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercurso(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualLacre repLacre = new Repositorio.Embarcador.Cargas.CargaMDFeManualLacre(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManual repCargaMDFeManual = new Repositorio.Embarcador.Cargas.CargaMDFeManual(unidadeTrabalho);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unidadeTrabalho);

            Repositorio.Embarcador.Cargas.CargaMDFeManualDestino repCargaMDFeManualDestino = new Repositorio.Embarcador.Cargas.CargaMDFeManualDestino(unidadeTrabalho);

            Servicos.Embarcador.Carga.MDFe svcMDFe = new Servicos.Embarcador.Carga.MDFe(unidadeTrabalho);

            Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual = repCargaMDFeManual.BuscarPorCodigo(codigoMDFeManual);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualLacre> lacres = repLacre.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercurso> percursos = repPercurso.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Localidade> destinos = new List<Dominio.Entidades.Localidade>();

            if (cargaMDFeManual.UsarListaDestinos())
            {
                List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualDestino> destinosMDF = repCargaMDFeManualDestino.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
                destinos = destinosMDF.Select(o => o.Localidade).ToList();
            }
            else
            {
                destinos.Add(cargaMDFeManual.Destino);
            }

            bool exigeEDIFiscalMT = cargaMDFeManual.Empresa?.Configuracao?.ExigeEDIFiscalMT ?? false;

            if (exigeEDIFiscalMT && (percursos.Any(o => o.Estado.Sigla == "MT") || (cargaMDFeManual.Origem.Estado.Sigla != "MT" && destinos.Any(obj => obj.Estado.Sigla == "MT"))) && lacres.Count <= 0)
            {
                erro = "O MDF-e possui passagem pelo MT, sendo necessário informar um lacre.";
                return false;
            }

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            bool gerouMdfe = false;
            string retorno = svcMDFe.EmitirMDFe(cargaMDFeManual, configuracaoTMS, TipoServicoMultisoftware, WebServiceConsultaCTe, unidadeTrabalho, out gerouMdfe);

            if (string.IsNullOrWhiteSpace(retorno))
            {
                cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                repCargaMDFeManual.Atualizar(cargaMDFeManual);
                erro = string.Empty;
                return true;
            }
            else
            {
                if (gerouMdfe)
                {
                    cargaMDFeManual.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoMDFeManual.EmEmissao;
                    repCargaMDFeManual.Atualizar(cargaMDFeManual);
                }
                erro = retorno;
                return false;
            }
        }

        private void AdicionarMotoristaNaCarga(Dominio.Entidades.Embarcador.Cargas.CargaMDFeManual cargaMDFeManual, Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Cargas.CargaMotorista repCargaMotorista = new Repositorio.Embarcador.Cargas.CargaMotorista(unidadeTrabalho);
            Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista repCargaMDFeManualPercentualMotorista = new Repositorio.Embarcador.Cargas.CargaMDFeManualPercentualMotorista(unidadeTrabalho);

            List<Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista> PercentualMotoristasExistentes = repCargaMDFeManualPercentualMotorista.BuscarPorCargaMDFeManual(cargaMDFeManual.Codigo);
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCarga = repCargaMotorista.BuscarPorCarga(carga.Codigo);

            ////Remover da carga os motorista removido do MDFe
            //List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCargaExcuir = motoristasCarga.Where(q => q.InseridoPelaCargaMDFeManual == true && !cargaMDFeManual.Motoristas.ToList().ConvertAll<int>(q => q.Codigo).Contains(q.Motorista.Codigo)).ToList();
            //foreach (var motoristaExcluir in motoristasCargaExcuir) {
            //    repCargaMotorista.Deletar(motoristaExcluir);
            //    motoristasCarga.Remove(motoristaExcluir);
            //}

            decimal percentualRestante = 100;
            foreach (var motorista in cargaMDFeManual?.Motoristas)
            {
                Dominio.Entidades.Embarcador.Cargas.CargaMDFeManualPercentualMotorista PercentualMotorista = PercentualMotoristasExistentes.Where(q => q.Motorista.Codigo == motorista.Codigo).FirstOrDefault();
                Dominio.Entidades.Embarcador.Cargas.CargaMotorista motoristaCarga = motoristasCarga.Where(q => q.Motorista.Codigo == motorista.Codigo).FirstOrDefault();
                percentualRestante = percentualRestante - (PercentualMotorista?.PercentualExecucao ?? 0);
                if (motoristaCarga != null)
                {
                    motoristaCarga.PercentualExecucao = PercentualMotorista?.PercentualExecucao ?? null;
                    repCargaMotorista.Atualizar(motoristaCarga);
                }
                else
                {
                    motoristaCarga = new Dominio.Entidades.Embarcador.Cargas.CargaMotorista();
                    motoristaCarga.Motorista = motorista;
                    motoristaCarga.Carga = carga;
                    motoristaCarga.PercentualExecucao = PercentualMotorista?.PercentualExecucao ?? null;
                    motoristaCarga.InseridoPelaCargaMDFeManual = true;
                    repCargaMotorista.Inserir(motoristaCarga);
                }
            }

            //rateio o restante para os outros motorista da carga
            List<Dominio.Entidades.Embarcador.Cargas.CargaMotorista> motoristasCargaAjustar = motoristasCarga.Where(q => !cargaMDFeManual.Motoristas.ToList().ConvertAll<int>(q => q.Codigo).Contains(q.Motorista.Codigo)).ToList();

            if (motoristasCargaAjustar.Count > 0)
            {
                decimal valorRateio = percentualRestante / motoristasCargaAjustar.Count;
                decimal somaRateioAplicado = 0;
                for (int i = 0; i < motoristasCargaAjustar.Count; i++)
                {
                    somaRateioAplicado = somaRateioAplicado + valorRateio;
                    var motoristaCarga = motoristasCargaAjustar[i];
                    motoristaCarga.PercentualExecucao = ((i == motoristasCargaAjustar.Count - 1) ? percentualRestante - somaRateioAplicado : 0) + valorRateio;
                    repCargaMotorista.Atualizar(motoristaCarga);
                }
            }
        }

        #endregion
    }
}
