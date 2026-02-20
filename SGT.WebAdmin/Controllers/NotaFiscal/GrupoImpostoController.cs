using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/GrupoImposto")]
    public class GrupoImpostoController : BaseController
    {
		#region Construtores

		public GrupoImpostoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);

                string descricao = Request.Params("Descricao");
                string ncm = Request.Params("NCM");
                if (!string.IsNullOrWhiteSpace(ncm) && ncm.Length > 4)
                    ncm = ncm.Substring(0, 4);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo;
                Enum.TryParse(Request.Params("Ativo"), out ativo);

                int codigoEmpresa = 0;
                int empresaPai = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                {
                    codigoEmpresa = this.Usuario.Empresa.Codigo;
                    if (this.Usuario.Empresa.EmpresaPai != null)
                        empresaPai = this.Usuario.Empresa.EmpresaPai.Codigo;
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("NCM", "NCM", 15, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 55, Models.Grid.Align.left, false);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto> listaGrupoImposto = repGrupoImposto.Consultar(ncm, empresaPai, codigoEmpresa, descricao, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repGrupoImposto.ContarConsulta(ncm, empresaPai, codigoEmpresa, descricao, ativo));
                var lista = (from p in listaGrupoImposto
                             select new
                             {
                                 p.Codigo,
                                 p.NCM,
                                 p.Descricao,
                                 p.DescricaoAtivo
                             }).ToList();
                grid.AdicionaRows(lista);
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

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                unitOfWork.Start();

                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);

                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto();
                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                grupoImposto.Descricao = Request.Params("Descricao");
                grupoImposto.NCM = Request.Params("NCM");
                grupoImposto.Ativo = ativo;
                grupoImposto.Empresa = this.Usuario.Empresa;

                repGrupoImposto.Inserir(grupoImposto, Auditado);
                unitOfWork.CommitChanges();

                string retorno = SalvarItensImposto(grupoImposto, unitOfWork);
                if (retorno != "Sucesso")
                    throw new Exception(retorno);

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar: " + ex.Message);
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
                unitOfWork.Start();
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);

                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto = repGrupoImposto.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                bool ativo;
                bool.TryParse(Request.Params("Ativo"), out ativo);

                grupoImposto.Descricao = Request.Params("Descricao");
                grupoImposto.NCM = Request.Params("NCM");
                grupoImposto.Empresa = this.Usuario.Empresa;
                grupoImposto.Ativo = ativo;

                string retorno = SalvarItensImposto(grupoImposto, unitOfWork);
                if (retorno != "Sucesso")
                    throw new Exception(retorno);

                repGrupoImposto.Atualizar(grupoImposto, Auditado);
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar: " + ex.Message);
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
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));
                string estadoOrigem = Request.Params("EstadoOrigem");
                string estadoDestino = Request.Params("EstadoDestino");
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto = repGrupoImposto.BuscarPorCodigo(codigo);

                string retorno = SalvarItensImposto(grupoImposto, unitOfWork);
                if (retorno != "Sucesso")
                    throw new Exception(retorno);

                unitOfWork.CommitChanges();

                grupoImposto = repGrupoImposto.BuscarPorCodigo(codigo);

                var dynProcessoMovimento = new
                {
                    grupoImposto.Codigo,
                    grupoImposto.Descricao,
                    grupoImposto.NCM,
                    grupoImposto.Ativo,
                    ListaItensImposto = grupoImposto.ItensImposto != null ? (from obj in grupoImposto.ItensImposto
                                                                             where obj.UFOrigem == estadoOrigem
                                                                             && (string.IsNullOrWhiteSpace(estadoDestino) ? 1 == 1 : obj.UFDestino == estadoDestino)
                                                                             orderby obj.UFDestino, obj.Atividade.Descricao
                                                                             select new
                                                                             {
                                                                                 obj.Codigo,
                                                                                 CodigoGrupoImposto = obj.GrupoImposto.Codigo,
                                                                                 obj.UFOrigem,
                                                                                 obj.UFDestino,
                                                                                 AliquotaICMSInternaVenda = obj.DescricaoAliquotaICMSInternaVenda(),
                                                                                 AliquotaICMSInterestadualVenda = obj.DescricaoAliquotaICMSInterestadualVenda(),
                                                                                 MVAVenda = obj.DescricaoMVAVenda(),
                                                                                 CSTICMSVenda = obj.DescricaoCSTICMSVendaGrid(),
                                                                                 CFOPVenda = obj.DescricaoCFOPVenda(),
                                                                                 ReducaoMVAVenda = obj.DescricaoReducaoMVAVenda(),
                                                                                 DiferencialAliquotaVenda = obj.DescricaoDiferencialAliquotaVendaGrid(),
                                                                                 Atividade = obj.Atividade.Descricao,
                                                                                 ReducaoBCICMSVenda = obj.DescricaoReducaoBCICMSVenda(),
                                                                                 CSTPISVenda = obj.DescricaoCSTPISVendaGrid(),
                                                                                 ReducaoBCPISVenda = obj.DescricaoReducaoBCPISVenda(),
                                                                                 AliquotaPISVenda = obj.DescricaoAliquotaPISVenda(),
                                                                                 CSTCOFINSVenda = obj.DescricaoCSTCOFINSVendaGrid(),
                                                                                 ReducaoBCCOFINSVenda = obj.DescricaoReducaoBCCOFINSVenda(),
                                                                                 AliquotaCOFINSVenda = obj.DescricaoAliquotaCOFINSVenda(),
                                                                                 AliquotaFCPVenda = obj.DescricaoAliquotaFCPVenda(),
                                                                                 AliquotaDifalVenda = obj.DescricaoAliquotaDifalVenda(),
                                                                                 AliquotaICMSCompra = obj.DescricaoAliquotaICMSCompra(),
                                                                                 MVACompra = obj.DescricaoMVACompra(),
                                                                                 CSTICMSCompra = obj.DescricaoCSTICMSCompraGrid(),
                                                                                 CFOPCompra = obj.DescricaoCFOPCompra(),
                                                                                 ReducaoBCICMSCompra = obj.DescricaoReducaoBCICMSCompra(),
                                                                                 ReducaoMVACompra = obj.DescricaoReducaoMVACompra(),
                                                                                 CSTPISCompra = obj.DescricaoCSTPISCompraGrid(),
                                                                                 ReducaoBCPISCompra = obj.DescricaoReducaoBCPISCompra(),
                                                                                 AliquotaPISCompra = obj.DescricaoAliquotaPISCompra(),
                                                                                 CSTCOFINSCompra = obj.DescricaoCSTCOFINSCompraGrid(),
                                                                                 ReducaoBCCOFINSCompra = obj.DescricaoReducaoBCCOFINSCompra(),
                                                                                 AliquotaCOFINSCompra = obj.DescricaoAliquotaCOFINSCompra(),
                                                                                 ObservacaoFiscal = obj.DescricaoObservacaoFiscal()
                                                                             }).ToList() : null
                };
                return new JsonpResult(dynProcessoMovimento);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto = repGrupoImposto.BuscarPorCodigo(codigo);
                repGrupoImposto.Deletar(grupoImposto, Auditado);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, "Não foi possível excluir o registro pois o mesmo já possui vínculo com outros recursos do sistema, recomendamos que você inative o registro caso não deseja mais utilizá-lo.");
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, "Ocorreu uma falha ao excluir.");
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarTributacaoAutomaticoItem()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoProduto, codigoServico, codigoCFOP, codigoAtividade = 0;
                int.TryParse(Request.Params("CodigoPoduto"), out codigoProduto);
                int.TryParse(Request.Params("CodigoServico"), out codigoServico);
                int.TryParse(Request.Params("CodigoCFOP"), out codigoCFOP);
                int.TryParse(Request.Params("CodigoAtividade"), out codigoAtividade);
                double codigoCliente = 0;
                double.TryParse(Request.Params("CodigoCliente"), out codigoCliente);
                decimal valorTotalItem, valorIPI, valorOutras, valorDesconto, valorFrete, valorSeguro = 0;
                decimal.TryParse(Request.Params("ValorTotalItem"), out valorTotalItem);
                decimal.TryParse(Request.Params("ValorOutras"), out valorOutras);
                decimal.TryParse(Request.Params("ValorDesconto"), out valorDesconto);
                decimal.TryParse(Request.Params("ValorFrete"), out valorFrete);
                decimal.TryParse(Request.Params("ValorSeguro"), out valorSeguro);
                decimal.TryParse(Request.Params("ValorIPI"), out valorIPI);
                decimal.TryParse(Request.Params("Quantidade"), out decimal quantidade);

                Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica serNotaFiscalEletronica = new Servicos.Embarcador.NotaFiscal.NotaFiscalEletronica(unitOfWork);
                dynamic dynTributacao = serNotaFiscalEletronica.BuscarTributacaoItem(unitOfWork, codigoProduto, codigoServico, codigoCFOP, codigoAtividade, codigoCliente, valorTotalItem, valorIPI, valorOutras, valorDesconto, valorFrete, valorSeguro, this.Empresa.Localidade.Estado.Sigla, this.Usuario.Empresa.Localidade.Estado.Sigla, quantidade, TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario?.Empresa?.Codigo ?? 0 : 0);

                if (dynTributacao == null)
                    return new JsonpResult(false, "Ocorreu uma falha ao carregar os impostos");
                else
                    return new JsonpResult(dynTributacao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao carregar os impostos: " + ex.Message);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> LancarNovosDadosTabelaDeImposto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));
                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto;
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                if (codigo > 0)
                    grupoImposto = repGrupoImposto.BuscarPorCodigo(codigo);
                else
                {
                    bool ativo;
                    bool.TryParse(Request.Params("Ativo"), out ativo);

                    grupoImposto = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto();
                    grupoImposto.Ativo = ativo;
                    grupoImposto.Descricao = Request.Params("Descricao");
                    grupoImposto.NCM = Request.Params("NCM");

                    repGrupoImposto.Inserir(grupoImposto);
                }

                if (grupoImposto.ItensImposto != null && grupoImposto.ItensImposto.Count() > 0)
                    for (int i = 0; i < grupoImposto.ItensImposto.Count(); i++)
                        repGrupoImpostoItens.Deletar(grupoImposto.ItensImposto[i]);

                IList<Dominio.Entidades.Atividade> listaAtividades = repAtividade.BuscarTodos(0, 100, null);
                List<Dominio.Entidades.Estado> listaEstadoDestino = repEstado.BuscarTodos();
                List<Dominio.Entidades.Estado> listaEstadoOrigem = repEstado.BuscarTodos();

                for (int i = 0; i < listaEstadoDestino.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens item = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens();
                    item.UFDestino = listaEstadoDestino[i].Sigla;
                    for (int a = 0; a < listaEstadoOrigem.Count(); a++)
                    {
                        if (a > 0)
                        {
                            item = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens();
                            item.UFDestino = listaEstadoDestino[i].Sigla;
                        }
                        item.UFOrigem = listaEstadoOrigem[a].Sigla;
                        for (int j = 0; j < listaAtividades.Count; j++)
                        {
                            if (j > 0)
                            {
                                item = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens();
                                item.UFDestino = listaEstadoDestino[i].Sigla;
                                item.UFOrigem = listaEstadoOrigem[a].Sigla;
                            }

                            item.Atividade = listaAtividades[j];
                            item.AliquotaCOFINSCompra = 0;
                            item.AliquotaCOFINSVenda = 0;
                            item.AliquotaDifalVenda = 0;
                            item.AliquotaFCPVenda = 0;
                            item.AliquotaICMSCompra = 0;
                            item.AliquotaICMSInterestadualVenda = 0;
                            item.AliquotaICMSInternaVenda = 0;
                            item.AliquotaPISCompra = 0;
                            item.AliquotaPISVenda = 0;
                            item.CFOPCompra = string.Empty;
                            item.CFOPVenda = string.Empty;
                            item.DiferencialAliquotaVenda = false;
                            item.GrupoImposto = grupoImposto;
                            item.MVACompra = 0;
                            item.MVAVenda = 0;
                            item.ReducaoBCCOFINSCompra = 0;
                            item.ReducaoBCCOFINSVenda = 0;
                            item.ReducaoBCICMSCompra = 0;
                            item.ReducaoBCICMSVenda = 0;
                            item.ReducaoBCPISCompra = 0;
                            item.ReducaoBCPISVenda = 0;
                            item.ReducaoMVACompra = 0;
                            item.ReducaoMVAVenda = 0;

                            repGrupoImpostoItens.Inserir(item);
                        }
                    }
                }

                unitOfWork.CommitChanges();
                var dynProcessoMovimento = new { grupoImposto.Codigo };
                return new JsonpResult(dynProcessoMovimento, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao lançar os novos registros para a tabela de imposto, por favor verifique no log.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> DuplicarTabelaDeImposto()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();
                int codigo = int.Parse(Request.Params("Codigo"));
                int codigoGrupoImposto = int.Parse(Request.Params("CodigoGrupoImposto"));
                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto;
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto repGrupoImposto = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImposto(unitOfWork);
                Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unitOfWork);
                Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);

                if (codigo > 0)
                    grupoImposto = repGrupoImposto.BuscarPorCodigo(codigo);
                else
                {
                    bool ativo;
                    bool.TryParse(Request.Params("Ativo"), out ativo);

                    grupoImposto = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto();
                    grupoImposto.Ativo = ativo;
                    grupoImposto.Descricao = Request.Params("Descricao");
                    grupoImposto.NCM = Request.Params("NCM");

                    repGrupoImposto.Inserir(grupoImposto);
                }

                if (grupoImposto.ItensImposto != null && grupoImposto.ItensImposto.Count() > 0)
                    for (int i = 0; i < grupoImposto.ItensImposto.Count(); i++)
                        repGrupoImpostoItens.Deletar(grupoImposto.ItensImposto[i]);

                List<Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens> listaItensImpostoGrupo = repGrupoImpostoItens.BuscarPorGrupo(codigoGrupoImposto);
                for (int i = 0; i < listaItensImpostoGrupo.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens item = new Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens();
                    item.UFDestino = listaItensImpostoGrupo[i].UFDestino;
                    item.UFOrigem = listaItensImpostoGrupo[i].UFOrigem;
                    item.Atividade = listaItensImpostoGrupo[i].Atividade;
                    item.AliquotaCOFINSCompra = listaItensImpostoGrupo[i].AliquotaCOFINSCompra;
                    item.AliquotaCOFINSVenda = listaItensImpostoGrupo[i].AliquotaCOFINSVenda;
                    item.AliquotaDifalVenda = listaItensImpostoGrupo[i].AliquotaDifalVenda;
                    item.AliquotaFCPVenda = listaItensImpostoGrupo[i].AliquotaFCPVenda;
                    item.AliquotaICMSCompra = listaItensImpostoGrupo[i].AliquotaICMSCompra;
                    item.AliquotaICMSInterestadualVenda = listaItensImpostoGrupo[i].AliquotaICMSInterestadualVenda;
                    item.AliquotaICMSInternaVenda = listaItensImpostoGrupo[i].AliquotaICMSInternaVenda;
                    item.AliquotaPISCompra = listaItensImpostoGrupo[i].AliquotaPISCompra;
                    item.AliquotaPISVenda = listaItensImpostoGrupo[i].AliquotaPISVenda;

                    item.CSTICMSVenda = listaItensImpostoGrupo[i].CSTICMSVenda;
                    item.CSTPISVenda = listaItensImpostoGrupo[i].CSTPISVenda;
                    item.CSTCOFINSVenda = listaItensImpostoGrupo[i].CSTCOFINSVenda;
                    item.CSTICMSCompra = listaItensImpostoGrupo[i].CSTICMSCompra;
                    item.CSTPISCompra = listaItensImpostoGrupo[i].CSTPISCompra;
                    item.CSTCOFINSCompra = listaItensImpostoGrupo[i].CSTCOFINSCompra;

                    item.CFOPCompra = listaItensImpostoGrupo[i].CFOPCompra;
                    item.CFOPVenda = listaItensImpostoGrupo[i].CFOPVenda;
                    item.DiferencialAliquotaVenda = listaItensImpostoGrupo[i].DiferencialAliquotaVenda;
                    item.GrupoImposto = grupoImposto;
                    item.MVACompra = listaItensImpostoGrupo[i].MVACompra;
                    item.MVAVenda = listaItensImpostoGrupo[i].MVAVenda;
                    item.ReducaoBCCOFINSCompra = listaItensImpostoGrupo[i].ReducaoBCCOFINSCompra;
                    item.ReducaoBCCOFINSVenda = listaItensImpostoGrupo[i].ReducaoBCCOFINSVenda;
                    item.ReducaoBCICMSCompra = listaItensImpostoGrupo[i].ReducaoBCICMSCompra;
                    item.ReducaoBCICMSVenda = listaItensImpostoGrupo[i].ReducaoBCICMSVenda;
                    item.ReducaoBCPISCompra = listaItensImpostoGrupo[i].ReducaoBCPISCompra;
                    item.ReducaoBCPISVenda = listaItensImpostoGrupo[i].ReducaoBCPISVenda;
                    item.ReducaoMVACompra = listaItensImpostoGrupo[i].ReducaoMVACompra;
                    item.ReducaoMVAVenda = listaItensImpostoGrupo[i].ReducaoMVAVenda;

                    repGrupoImpostoItens.Inserir(item);
                }

                unitOfWork.CommitChanges();
                var dynProcessoMovimento = new { grupoImposto.Codigo };
                return new JsonpResult(dynProcessoMovimento, true, "Sucesso");
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao lançar os novos registros para a tabela de imposto, por favor verifique no log.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private string SalvarItensImposto(Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImposto grupoImposto, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens repGrupoImpostoItens = new Repositorio.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens(unidadeDeTrabalho);
            Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal repObservacaoFiscal = new Repositorio.Embarcador.NotaFiscal.ObservacaoFiscal(unidadeDeTrabalho);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unidadeDeTrabalho);
            if (string.IsNullOrWhiteSpace(Request.Params("ListaItensImposto")))
                return "Sucesso";
            dynamic listaItensImposto = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaItensImposto"));

            foreach (var item in listaItensImposto)
            {
                Dominio.Entidades.Embarcador.ImpostoNotaFiscal.GrupoImpostoItens impostoItem = repGrupoImpostoItens.BuscarPorCodigo(int.Parse((string)item.ItemImposto.Codigo));

                impostoItem.AliquotaCOFINSCompra = decimal.Parse(((string)item.ItemImposto.AliquotaCOFINSCompra).Replace(".", ","));
                impostoItem.AliquotaCOFINSVenda = decimal.Parse(((string)item.ItemImposto.AliquotaCOFINSVenda).Replace(".", ","));
                impostoItem.AliquotaDifalVenda = decimal.Parse(((string)item.ItemImposto.AliquotaDifalVenda).Replace(".", ","));
                impostoItem.AliquotaFCPVenda = decimal.Parse(((string)item.ItemImposto.AliquotaFCPVenda).Replace(".", ","));
                impostoItem.AliquotaICMSCompra = decimal.Parse(((string)item.ItemImposto.AliquotaICMSCompra).Replace(".", ","));
                impostoItem.AliquotaICMSInterestadualVenda = decimal.Parse(((string)item.ItemImposto.AliquotaICMSInterestadualVenda).Replace(".", ","));
                impostoItem.AliquotaICMSInternaVenda = decimal.Parse(((string)item.ItemImposto.AliquotaICMSInternaVenda).Replace(".", ","));
                impostoItem.AliquotaPISCompra = decimal.Parse(((string)item.ItemImposto.AliquotaPISCompra).Replace(".", ","));
                impostoItem.AliquotaPISVenda = decimal.Parse(((string)item.ItemImposto.AliquotaPISVenda).Replace(".", ","));

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMSVenda = impostoItem.EnumeradorCSTICMS((string)item.ItemImposto.CSTICMSVenda);
                if (CSTICMSVenda != null)
                {
                    if ((int)CSTICMSVenda > 0)
                        impostoItem.CSTICMSVenda = CSTICMSVenda;
                    else
                        impostoItem.CSTICMSVenda = null;
                }
                else
                    return "CST de ICMS de Venda " + (string)item.ItemImposto.CSTICMSVenda + " inválido!";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTPISVenda = impostoItem.EnumeradorCSTPISCOFINS((string)item.ItemImposto.CSTPISVenda);
                if (CSTPISVenda != null)
                {
                    if ((int)CSTPISVenda > 0)
                        impostoItem.CSTPISVenda = CSTPISVenda;
                    else
                        impostoItem.CSTPISVenda = null;
                }
                else
                    return "CST de PIS de Venda " + (string)item.ItemImposto.CSTPISVenda + " inválido!";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTCOFINSVenda = impostoItem.EnumeradorCSTPISCOFINS((string)item.ItemImposto.CSTCOFINSVenda);
                if (CSTCOFINSVenda != null)
                {
                    if ((int)CSTCOFINSVenda > 0)
                        impostoItem.CSTCOFINSVenda = CSTCOFINSVenda;
                    else
                        impostoItem.CSTCOFINSVenda = null;
                }
                else
                    return "CST de COFINS de Venda " + (string)item.ItemImposto.CSTCOFINSVenda + " inválido!";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? CSTICMSCompra = impostoItem.EnumeradorCSTICMS((string)item.ItemImposto.CSTICMSCompra);
                if (CSTICMSCompra != null)
                {
                    if ((int)CSTICMSCompra > 0)
                        impostoItem.CSTICMSCompra = CSTICMSCompra;
                    else
                        impostoItem.CSTICMSCompra = null;
                }
                else
                    return "CST de ICMS de Compra " + (string)item.ItemImposto.CSTICMSCompra + " inválido!";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTPISCompra = impostoItem.EnumeradorCSTPISCOFINS((string)item.ItemImposto.CSTPISCompra);
                if (CSTPISCompra != null)
                {
                    if ((int)CSTPISCompra > 0)
                        impostoItem.CSTPISCompra = CSTPISCompra;
                    else
                        impostoItem.CSTPISCompra = null;
                }
                else
                    return "CST de PIS de Compra " + (string)item.ItemImposto.CSTPISCompra + " inválido!";

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTPISCOFINS? CSTCOFINSCompra = impostoItem.EnumeradorCSTPISCOFINS((string)item.ItemImposto.CSTCOFINSCompra);
                if (CSTCOFINSCompra != null)
                {
                    if ((int)CSTCOFINSCompra > 0)
                        impostoItem.CSTCOFINSCompra = CSTCOFINSCompra;
                    else
                        impostoItem.CSTCOFINSCompra = null;
                }
                else
                    return "CST de COFINS de Compra " + (string)item.ItemImposto.CSTCOFINSCompra + " inválido!";

                if (!string.IsNullOrWhiteSpace((string)item.ItemImposto.CFOPCompra) && repCFOP.BuscarPorNumero(int.Parse((string)item.ItemImposto.CFOPCompra)) != null)
                    impostoItem.CFOPCompra = (string)item.ItemImposto.CFOPCompra;
                else if (!string.IsNullOrWhiteSpace((string)item.ItemImposto.CFOPCompra))
                    return "CFOP de Compra " + (string)item.ItemImposto.CFOPCompra + " inválido!";
                else
                    impostoItem.CFOPCompra = null;

                if (!string.IsNullOrWhiteSpace((string)item.ItemImposto.CFOPVenda) && repCFOP.BuscarPorNumero(int.Parse((string)item.ItemImposto.CFOPVenda)) != null)
                    impostoItem.CFOPVenda = (string)item.ItemImposto.CFOPVenda;
                else if (!string.IsNullOrWhiteSpace((string)item.ItemImposto.CFOPVenda))
                    return "CFOP de Venda " + (string)item.ItemImposto.CFOPVenda + " inválido!";
                else
                    impostoItem.CFOPVenda = null;

                if (!string.IsNullOrWhiteSpace((string)item.ItemImposto.ObservacaoFiscal) && repObservacaoFiscal.BuscarPorCodigo(int.Parse((string)item.ItemImposto.ObservacaoFiscal)) != null)
                    impostoItem.ObservacaoFiscal = repObservacaoFiscal.BuscarPorCodigo(int.Parse((string)item.ItemImposto.ObservacaoFiscal));
                else
                    impostoItem.ObservacaoFiscal = null;

                impostoItem.DiferencialAliquotaVenda = (string)item.ItemImposto.DiferencialAliquotaVenda == "S";
                impostoItem.MVACompra = decimal.Parse(((string)item.ItemImposto.MVACompra).Replace(".", ","));
                impostoItem.MVAVenda = decimal.Parse(((string)item.ItemImposto.MVAVenda).Replace(".", ","));
                impostoItem.ReducaoBCCOFINSCompra = decimal.Parse(((string)item.ItemImposto.ReducaoBCCOFINSCompra).Replace(".", ","));
                impostoItem.ReducaoBCCOFINSVenda = decimal.Parse(((string)item.ItemImposto.ReducaoBCCOFINSVenda).Replace(".", ","));
                impostoItem.ReducaoBCICMSCompra = decimal.Parse(((string)item.ItemImposto.ReducaoBCICMSCompra).Replace(".", ","));
                impostoItem.ReducaoBCICMSVenda = decimal.Parse(((string)item.ItemImposto.ReducaoBCICMSVenda).Replace(".", ","));
                impostoItem.ReducaoBCPISCompra = decimal.Parse(((string)item.ItemImposto.ReducaoBCPISCompra).Replace(".", ","));
                impostoItem.ReducaoBCPISVenda = decimal.Parse(((string)item.ItemImposto.ReducaoBCPISVenda).Replace(".", ","));
                impostoItem.ReducaoMVACompra = decimal.Parse(((string)item.ItemImposto.ReducaoMVACompra).Replace(".", ","));
                impostoItem.ReducaoMVAVenda = decimal.Parse(((string)item.ItemImposto.ReducaoMVAVenda).Replace(".", ","));

                repGrupoImpostoItens.Atualizar(impostoItem);


            }
            return "Sucesso";
        }

        private bool CSTCalculaICMS(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS)
        {
            if ((int)cstICMS == 0)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return true;
            else
                return false;
        }

        private bool CSTCalculaICMSST(Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS? cstICMS)
        {
            if ((int)cstICMS == 0)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST00)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST10)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST20)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST30)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST40 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST41 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST50)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST51)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST60)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST70)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CST90)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN101)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN102 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN103 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN300 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN400)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN201)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN202 || cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN203)
                return true;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN500)
                return false;
            else if (cstICMS == Dominio.ObjetosDeValor.Embarcador.Enumeradores.CSTICMS.CSOSN900)
                return true;
            else
                return false;
        }

        #endregion
    }
}
