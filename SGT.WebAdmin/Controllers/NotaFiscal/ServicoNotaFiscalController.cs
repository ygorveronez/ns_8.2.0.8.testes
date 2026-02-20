using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/Servico")]
    public class ServicoNotaFiscalController : BaseController
    {
		#region Construtores

		public ServicoNotaFiscalController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao natureza = null;

                string dentroEstado = "";
                double codigoPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out codigoPessoa);
                int codigoNatureza = 0;
                int.TryParse(Request.Params("NaturezaOperacao"), out codigoNatureza);
                if (codigoNatureza > 0)
                    natureza = repNaturezaDaOperacao.BuscarPorId(codigoNatureza);
                Dominio.Entidades.Cliente pessoa = null;
                if (codigoPessoa > 0)
                    pessoa = repCliente.BuscarPorCPFCNPJ(codigoPessoa);
                if (pessoa != null)
                    dentroEstado = pessoa.Localidade.Estado.Sigla == this.Usuario.Empresa.Localidade.Estado.Sigla ? "S" : "N";

                Dominio.Enumeradores.TipoEmissaoNFe? tipoEmissaoNFe = null;
                Dominio.Enumeradores.TipoEmissaoNFe tipoEmissaoNFeAux;
                if (!string.IsNullOrWhiteSpace(Request.Params("TipoEmissao")))
                {
                    if (Enum.TryParse(Request.Params("TipoEmissao"), out tipoEmissaoNFeAux))
                        tipoEmissaoNFe = tipoEmissaoNFeAux;
                }

                string descricao = Request.Params("Descricao");
                int empresa = 0;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    empresa = this.Usuario.Empresa.Codigo;
                int codigo = 0;
                int.TryParse(descricao, out codigo);
                if (codigo > 0)
                    descricao = "";
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status = Request.GetEnumParam("Status", Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Código", "Codigo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho("Descrição", "Descricao", 55, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Lista Serviço", "DescricaoCodigoServico", 25, Models.Grid.Align.left, false);
                if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Status", "DescricaoStatus", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("CodigoCFOP", false);
                grid.AdicionarCabecalho("CFOP", false);
                grid.AdicionarCabecalho("AliquotaISS", false);
                grid.AdicionarCabecalho("ValorVenda", false);
                grid.AdicionarCabecalho("DescricaoSemNFE", false);
                grid.AdicionarCabecalho("DescricaoNFE", false);

                List<Dominio.Entidades.Embarcador.NotaFiscal.Servico> listaServico = repServico.Consultar(codigo, descricao, empresa, status, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repServico.ContarConsulta(codigo, descricao, empresa, status));
                var lista = (from p in listaServico
                             select new
                             {
                                 p.Codigo,
                                 Descricao = p.Descricao + " (" + p.DescricaoNFE + ")",
                                 DescricaoCodigoServico = p.NumeroCodigoServico + " - " + p.DescricaoCodigoServico,
                                 p.DescricaoStatus,
                                 CodigoCFOP = dentroEstado == "S" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Saida && p.CFOPVendaDentroEstado != null ? p.CFOPVendaDentroEstado.Codigo :
                                     dentroEstado == "S" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Entrada && p.CFOPCompraDentroEstado != null ? p.CFOPCompraDentroEstado.Codigo :
                                     dentroEstado == "N" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Saida && p.CFOPVendaForaEstado != null ? p.CFOPVendaForaEstado.Codigo :
                                     dentroEstado == "N" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Entrada && p.CFOPCompraForaEstado != null ? p.CFOPCompraForaEstado.Codigo : 0,
                                 CFOP = dentroEstado == "S" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Saida && p.CFOPVendaDentroEstado != null ? p.CFOPVendaDentroEstado.CodigoCFOP :
                                     dentroEstado == "S" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Entrada && p.CFOPCompraDentroEstado != null ? p.CFOPCompraDentroEstado.CodigoCFOP :
                                     dentroEstado == "N" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Saida && p.CFOPVendaForaEstado != null ? p.CFOPVendaForaEstado.CodigoCFOP :
                                     dentroEstado == "N" && tipoEmissaoNFe == Dominio.Enumeradores.TipoEmissaoNFe.Entrada && p.CFOPCompraForaEstado != null ? p.CFOPCompraForaEstado.CodigoCFOP : 0,
                                 p.AliquotaISS,
                                 ValorVenda = p.ValorVenda.ToString("n" + this.Usuario.Empresa.CasasValorProdutoNFe.ToString()),
                                 DescricaoSemNFE = p.Descricao,
                                 p.DescricaoNFE
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

                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = new Dominio.Entidades.Embarcador.NotaFiscal.Servico();

                PreencherServico(servico, unitOfWork);

                if (!string.IsNullOrWhiteSpace(servico.CodigoIntegracao))
                {
                    if (repServico.ContemServicoMesmoCodigoIntegracao(servico.CodigoIntegracao, 0, servico.Empresa?.Codigo ?? 0))
                        throw new ControllerException("Já existe um serviço com o mesmo Código de Integração informado.");
                }

                repServico.Inserir(servico, Auditado);

                if (string.IsNullOrWhiteSpace(servico.CodigoIntegracao))
                {
                    servico.CodigoIntegracao = servico.Codigo.ToString();
                    repServico.Atualizar(servico);
                }

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao adicionar.");
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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigo(codigo, true);

                if (servico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                PreencherServico(servico, unitOfWork);

                if (!string.IsNullOrWhiteSpace(servico.CodigoIntegracao))
                {
                    if (repServico.ContemServicoMesmoCodigoIntegracao(servico.CodigoIntegracao, servico.Codigo, servico.Empresa?.Codigo ?? 0))
                        throw new ControllerException("Já existe um serviço com o mesmo Código de Integração informado.");
                }

                if (string.IsNullOrWhiteSpace(servico.CodigoIntegracao))
                    servico.CodigoIntegracao = servico.Codigo.ToString();

                repServico.Atualizar(servico, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
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
                return new JsonpResult(false, "Ocorreu uma falha ao atualizar.");
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

                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigo(codigo);

                if (servico == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynServico = new
                {
                    servico.Codigo,
                    servico.Descricao,
                    servico.DescricaoNFE,
                    servico.CodigoTributacao,
                    servico.ValorVenda,
                    servico.AliquotaISS,
                    servico.CodigoServico,
                    CFOPVendaDentroEstado = servico.CFOPVendaDentroEstado != null ? new { Codigo = servico.CFOPVendaDentroEstado.Codigo, Descricao = servico.CFOPVendaDentroEstado.CodigoCFOP } : null,
                    CFOPVendaForaEstado = servico.CFOPVendaForaEstado != null ? new { Codigo = servico.CFOPVendaForaEstado.Codigo, Descricao = servico.CFOPVendaForaEstado.CodigoCFOP } : null,
                    CFOPCompraDentroEstado = servico.CFOPCompraDentroEstado != null ? new { Codigo = servico.CFOPCompraDentroEstado.Codigo, Descricao = servico.CFOPCompraDentroEstado.CodigoCFOP } : null,
                    CFOPCompraForaEstado = servico.CFOPCompraForaEstado != null ? new { Codigo = servico.CFOPCompraForaEstado.Codigo, Descricao = servico.CFOPCompraForaEstado.CodigoCFOP } : null,
                    Empresa = servico.Empresa != null ? new { Codigo = servico.Empresa.Codigo, Descricao = servico.Empresa.RazaoSocial } : null,
                    servico.Numero,
                    servico.CNAE,
                    servico.NBS,
                    Localidade = servico.Localidade != null ? new { Codigo = servico.Localidade.Codigo, Descricao = servico.Localidade.Descricao } : null,
                    servico.Status,
                    servico.CodigoIntegracao
                };

                return new JsonpResult(dynServico);
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

        public async Task<IActionResult> ExcluirPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.Embarcador.NotaFiscal.Servico servico = repServico.BuscarPorCodigo(codigo, true);

                if (servico == null)
                    return new JsonpResult(false, true, "Serviço não encontrado.");

                Dominio.Entidades.ServicoNFSe servicoNFSe = servico.ServicoNFSe;

                repServico.Deletar(servico, Auditado);
                if (servicoNFSe != null)
                    repServicoNFSe.Deletar(servicoNFSe, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
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

        #endregion

        #region Metodos Privados

        private void PreencherServico(Dominio.Entidades.Embarcador.NotaFiscal.Servico servico, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int.TryParse(Request.Params("CFOPVendaDentroEstado"), out int codigoCfopVendaDentroEstado);
            int.TryParse(Request.Params("CFOPVendaForaEstado"), out int codigoCfopVendaForaEstado);
            int.TryParse(Request.Params("CFOPCompraDentroEstado"), out int codigoCfopCompraDentroEstado);
            int.TryParse(Request.Params("CFOPCompraForaEstado"), out int codigoCfopCompraForaEstado);
            int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);

            Dominio.Enumeradores.ListaServico listaServico;

            string descricao = Request.Params("Descricao");
            string descricaoNFE = Request.Params("DescricaoNFE");
            string codigoTributacao = Request.Params("CodigoTributacao");
            string numero = Request.Params("Numero");
            string cnae = Request.Params("CNAE");
            string nbs = Request.Params("NBS");

            decimal.TryParse(Request.Params("ValorVenda"), out decimal valorVenda);
            decimal.TryParse(Request.Params("AliquotaISS"), out decimal aliquotaISS);

            Enum.TryParse(Request.Params("CodigoServico"), out listaServico);
            bool.TryParse(Request.Params("Status"), out bool status);

            servico.CFOPVendaDentroEstado = codigoCfopVendaDentroEstado > 0 ? repCFOP.BuscarPorCodigo(codigoCfopVendaDentroEstado) : null;
            servico.CFOPVendaForaEstado = codigoCfopVendaForaEstado > 0 ? repCFOP.BuscarPorCodigo(codigoCfopVendaForaEstado) : null;
            servico.CFOPCompraDentroEstado = codigoCfopCompraDentroEstado > 0 ? repCFOP.BuscarPorCodigo(codigoCfopCompraDentroEstado) : null;
            servico.CFOPCompraForaEstado = codigoCfopCompraForaEstado > 0 ? repCFOP.BuscarPorCodigo(codigoCfopCompraForaEstado) : null;

            if (servico.Codigo == 0)
                servico.Empresa = Usuario.Empresa;

            servico.CodigoServico = listaServico;
            servico.Descricao = descricao;
            servico.DescricaoNFE = descricaoNFE;
            servico.CodigoTributacao = codigoTributacao;
            servico.ValorVenda = valorVenda;
            servico.AliquotaISS = aliquotaISS;
            servico.Numero = !string.IsNullOrWhiteSpace(numero) ? numero : null;
            servico.Localidade = codigoLocalidade > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidade) : null;
            servico.CNAE = cnae;
            servico.NBS = nbs;
            servico.Status = status;
            servico.CodigoIntegracao = Request.GetStringParam("CodigoIntegracao");

            if ((!string.IsNullOrWhiteSpace(numero) && numero != "0") || codigoLocalidade > 0)
                servico.ServicoNFSe = AdicionarAtualizarServicoNFSe(unitOfWork, servico);
        }

        private Dominio.Entidades.ServicoNFSe AdicionarAtualizarServicoNFSe(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.NotaFiscal.Servico servico)
        {
            try
            {
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.ServicoNFSe servicoNFSe = servico.ServicoNFSe;
                if (servicoNFSe == null)
                    servicoNFSe = new Dominio.Entidades.ServicoNFSe();

                servicoNFSe.Status = !servico.Status ? "I" : "A";
                servicoNFSe.CNAE = servico.CNAE;
                servicoNFSe.Descricao = servico.DescricaoNFE;
                servicoNFSe.CodigoTributacao = servico.CodigoTributacao;
                servicoNFSe.NBS = servico.NBS;
                servicoNFSe.Numero = servico.Numero;
                servicoNFSe.Localidade = servico.Localidade;
                servicoNFSe.Aliquota = servico.AliquotaISS;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    servicoNFSe.Empresa = this.Usuario.Empresa;

                if (servicoNFSe.Codigo == 0)
                    repServicoNFSe.Inserir(servicoNFSe);
                else
                    repServicoNFSe.Atualizar(servicoNFSe);

                return servicoNFSe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw;
            }
        }

        #endregion
    }
}
