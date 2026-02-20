using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Cargas.Carga.DocumentosEmissao
{
    public class CargaPedidoDocumentoMDFeController : BaseController
    {
		#region Construtores

		public CargaPedidoDocumentoMDFeController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Consultar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);

                int codigoCargaPedido = int.Parse(Request.Params("CargaPedido"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("CodigoMDFE", false);
                grid.AdicionarCabecalho("SituacaoMDFe", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 10, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Veículos", "Veiculos", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarregamento", 18, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDescarregamento", 18, Models.Grid.Align.left, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "UFCarregamento")
                    propOrdenacao = "MDFe.EstadoCarregamento.Nome";
                else if (propOrdenacao == "UFDescarregamento")
                    propOrdenacao = "MDFe.EstadoDescarregamento.Nome";
                else
                    propOrdenacao = "MDFe." + propOrdenacao;

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> cargaPedidoDocumentosMDFe = repCargaPedidoDocumentoMDFe.Consultar(codigoCargaPedido, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repCargaPedidoDocumentoMDFe.ContarConsulta(codigoCargaPedido));

                grid.AdicionaRows((from obj in cargaPedidoDocumentosMDFe
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoMDFE = obj.MDFe.Codigo,
                                       CodigoEmpresa = obj.MDFe.Empresa.Codigo,
                                       SituacaoMDFe = obj.MDFe.Status,
                                       obj.MDFe.Numero,
                                       DataEmissao = obj.MDFe.DataEmissao?.ToString("dd/MM/yyyy HH:mm"),
                                       Serie = obj.MDFe.Serie.Numero,
                                       Veiculos = Servicos.Embarcador.Carga.MDFe.ObterPlacas(obj.MDFe),
                                       UFCarregamento = obj.MDFe.EstadoCarregamento?.Nome ?? string.Empty,
                                       UFDescarregamento = obj.MDFe.EstadoDescarregamento?.Nome ?? string.Empty
                                   }).ToList());

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

        public async Task<IActionResult> ConsultarMDFesSemCarga()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Numero"), out int numero);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);

                string ufCarregamento = Request.Params("EstadoCarregamento");
                string ufDescarregamento = Request.Params("EstadoDescarregamento");

                string placaVeiculo = string.Empty;

                if (codigoVeiculo > 0)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                    placaVeiculo = veiculo?.Placa;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoMDFe", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Emissão", "DataEmissao", 11, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 18, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 19, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarregamento", 14, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDescarregamento", 14, Models.Grid.Align.left, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "UFCarregamento")
                    propOrdenacao = "EstadoCarregamento.Nome";
                else if (propOrdenacao == "UFDescarregamento")
                    propOrdenacao = "EstadoDescarregamento.Nome";

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.ConsultarMDFesSemCarga(numero, placaVeiculo, ufCarregamento, ufDescarregamento, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repMDFe.ContarConsultaMDFesSemCarga(numero, placaVeiculo, ufCarregamento, ufDescarregamento));

                grid.AdicionaRows((from obj in mdfes
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoEmpresa = obj.Empresa.Codigo,
                                       SituacaoMDFe = obj.Status,
                                       obj.Numero,
                                       Serie = obj.Serie.Numero,
                                       DataEmissao = obj.DataEmissao.HasValue ? obj.DataEmissao.Value.ToString("dd/MM/yyyy") : "",
                                       Motorista = Servicos.Embarcador.Carga.MDFe.ObterMotoristas(obj),
                                       Veiculo = Servicos.Embarcador.Carga.MDFe.ObterPlacas(obj),
                                       UFCarregamento = obj.EstadoCarregamento?.Nome,
                                       UFDescarregamento = obj.EstadoDescarregamento?.Nome
                                   }).ToList());

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

        public async Task<IActionResult> ConsultarMDFes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Numero"), out int numero);
                int.TryParse(Request.Params("Veiculo"), out int codigoVeiculo);
                int.TryParse(Request.Params("Carga"), out int codigoCarga);

                string ufCarregamento = Request.Params("EstadoCarregamento");
                string ufDescarregamento = Request.Params("EstadoDescarregamento");

                string placaVeiculo = string.Empty;

                if (codigoVeiculo > 0)
                {
                    Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                    Dominio.Entidades.Veiculo veiculo = repVeiculo.BuscarPorCodigo(codigoVeiculo);

                    placaVeiculo = veiculo?.Placa;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("SituacaoMDFe", false);
                grid.AdicionarCabecalho("CodigoEmpresa", false);
                grid.AdicionarCabecalho("Número", "Numero", 8, Models.Grid.Align.center, true);
                grid.AdicionarCabecalho("Veículo", "Veiculo", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("UF Carregamento", "UFCarregamento", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("UF Descarregamento", "UFDescarregamento", 15, Models.Grid.Align.left, true);

                string propOrdenacao = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdenacao == "UFCarregamento")
                    propOrdenacao = "EstadoCarregamento.Nome";
                else if (propOrdenacao == "UFDescarregamento")
                    propOrdenacao = "EstadoDescarregamento.Nome";

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais> mdfes = repMDFe.ConsultarMDFes(codigoCarga, numero, placaVeiculo, ufCarregamento, ufDescarregamento, null, propOrdenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repMDFe.ContarConsultaMDFes(codigoCarga, numero, placaVeiculo, ufCarregamento, ufDescarregamento, null));

                grid.AdicionaRows((from obj in mdfes
                                   select new
                                   {
                                       obj.Codigo,
                                       CodigoEmpresa = obj.Empresa.Codigo,
                                       SituacaoMDFe = obj.Status,
                                       obj.Numero,
                                       Serie = obj.Serie.Numero,
                                       Motorista = Servicos.Embarcador.Carga.MDFe.ObterMotoristas(obj),
                                       Veiculo = Servicos.Embarcador.Carga.MDFe.ObterPlacas(obj),
                                       UFCarregamento = obj.EstadoCarregamento?.Nome,
                                       UFDescarregamento = obj.EstadoDescarregamento?.Nome
                                   }).ToList());

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

        public async Task<IActionResult> BuscarMDFesCompativeis()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                unitOfWork.Start();

                if (!Servicos.Embarcador.MDFe.MDFeImportado.VerificarSeCargaPossuiAlgumMDFeCompativel(out string erro, codigoCargaPedido, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os MDF-es compatíveis.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                int.TryParse(Request.Params("CodigoMDFe"), out int codigoMDFe);
                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (mdfe == null)
                    return new JsonpResult(false, true, "MDF-e não encontrado.");

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                unitOfWork.Start();

                if (!Servicos.Embarcador.MDFe.MDFeImportado.VincularMDFeACargaPedido(out string erro, codigoCargaPedido, codigoMDFe, unitOfWork, Auditado, ConfiguracaoEmbarcador))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Adicionou ao Pedido " + cargaPedido.Pedido.Numero.ToString(), unitOfWork);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar o MDF-e.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> Remover()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("CodigoMDFe"), out int codigoMDFe);
                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);
                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                unitOfWork.Start();

                if (!Servicos.Embarcador.MDFe.MDFeImportado.RemoverMDFeCargaPedido(out string erro, codigoCargaPedido, codigoMDFe, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }

                Servicos.Auditoria.Auditoria.Auditar(Auditado, mdfe, null, "Removeu do Pedido " + cargaPedido.Pedido.Numero.ToString(), unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, "Removeu o MDF-e " + mdfe.Descricao + ".", unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> RemoverTodos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);
                Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe repCargaPedidoDocumentoMDFe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoMDFe(unitOfWork);

                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Cargas/Carga", "Logistica/JanelaCarregamento");
                if (!permissoesPersonalizadas.Contains(AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada.Carga_InformarDocumentosFiscais))
                    return new JsonpResult(false, true, "Você não possui permissões para executar esta ação.");

                int.TryParse(Request.Params("CargaPedido"), out int codigoCargaPedido);

                Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = repCargaPedido.BuscarPorCodigo(codigoCargaPedido);

                if (cargaPedido.Carga.ProcessandoDocumentosFiscais)
                    return new JsonpResult(false, true, "A carga está processando os documentos fiscais para avançar para a próxima etapa, aguarde.");

                List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> cargaPedidoDocumentoMDFes = repCargaPedidoDocumentoMDFe.BuscarPorCargaPedido(cargaPedido.Codigo);

                if (cargaPedidoDocumentoMDFes.Count > 0)
                {
                    unitOfWork.Start();

                    foreach (Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe cargaPedidoDocumentoMDFe in cargaPedidoDocumentoMDFes)
                    {
                        if (!Servicos.Embarcador.MDFe.MDFeImportado.RemoverMDFeCargaPedido(out string erro, 0, 0, unitOfWork, cargaPedidoDocumentoMDFe))
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, erro);
                        }

                        Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedidoDocumentoMDFe.MDFe, null, "Removeu do pedido " + cargaPedido.Pedido.Numero.ToString() + ".", unitOfWork);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, cargaPedido.Carga, null, "Removeu todos os MDF-es para vínculo.", unitOfWork);

                    unitOfWork.CommitChanges();
                }

                return new JsonpResult(true);

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao remover.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
