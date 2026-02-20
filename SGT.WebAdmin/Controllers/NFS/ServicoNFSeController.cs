using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/ServicoNFSe")]
    public class ServicoNFSeController : BaseController
    {
		#region Construtores

		public ServicoNFSeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Repositorio.Embarcador.NotaFiscal.Servico repServico = new Repositorio.Embarcador.NotaFiscal.Servico(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Aliquota", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "Numero", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Localidade, "Localidade", 25, Models.Grid.Align.left, true); ;
                grid.AdicionarCabecalho("NBS", "NBS", 15, Models.Grid.Align.center, false);
               
                if (filtrosPesquisa.Situacao == SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho("ValorServico", false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Localidade")
                    propOrdena += ".Descricao";

                int totalRegistros = repServicoNFSe.ContarConsulta(filtrosPesquisa);
                if (totalRegistros == 0)
                {
                    grid.setarQuantidadeTotal(totalRegistros);
                    grid.AdicionaRows(new List<dynamic>());
                    return new JsonpResult(grid);
                }

                List<Dominio.Entidades.ServicoNFSe> listaServicoNFSe = repServicoNFSe.Consultar(filtrosPesquisa, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                List<int> codigosServicoNFSe = listaServicoNFSe.Select(s => s.Codigo).ToList();
                List<Dominio.Entidades.Embarcador.NotaFiscal.Servico> listaServicos = repServico.BuscarPorCodigosServicoNFSe(codigosServicoNFSe);

                var lista = (from p in listaServicoNFSe
                             select new
                             {
                                 p.Codigo,
                                 p.Aliquota,
                                 p.Descricao,
                                 p.Numero,
                                 p.NBS,
                                 Localidade = p.Localidade?.DescricaoCidadeEstado ?? string.Empty,
                                 p.DescricaoStatus,
                                 ValorServico = listaServicos.Where(s => s.ServicoNFSe?.Codigo == p.Codigo).FirstOrDefault()?.ValorVenda.ToString("n2") ?? 0.ToString("n2")
                             }).ToList();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
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

                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.ServicoNFSe servicoNFSe = new Dominio.Entidades.ServicoNFSe();

                int localidade = 0;
                int.TryParse(Request.Params("Localidade"), out localidade);

                string numero = Request.Params("Numero");

                servicoNFSe.Status = Request.Params("Status");
                servicoNFSe.CNAE = Request.Params("CNAE");
                servicoNFSe.Descricao = Request.Params("Descricao");
                servicoNFSe.NBS = Request.Params("NBS");
                servicoNFSe.CodigoTributacao = Request.Params("CodigoTributacao");
                servicoNFSe.Numero = numero;
                servicoNFSe.Localidade = new Dominio.Entidades.Localidade() { Codigo = localidade };
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    servicoNFSe.Empresa = this.Usuario.Empresa;

                if (repServicoNFSe.BuscarPorNumeroELocalidadeECodigoTributacao(servicoNFSe.Numero, localidade, servicoNFSe.CodigoTributacao) == null)
                {
                    repServicoNFSe.Inserir(servicoNFSe, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma natureza cadastrada para esse número");
                }
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
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);

                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int localidade = 0;
                int.TryParse(Request.Params("Localidade"), out localidade);
                string numero = Request.Params("Numero");

                servicoNFSe.Status = Request.Params("Status");
                servicoNFSe.Descricao = Request.Params("Descricao");
                servicoNFSe.CNAE = Request.Params("CNAE");
                servicoNFSe.NBS = Request.Params("NBS");
                servicoNFSe.CodigoTributacao = Request.Params("CodigoTributacao");
                servicoNFSe.Numero = numero;
                servicoNFSe.Localidade = new Dominio.Entidades.Localidade() { Codigo = localidade };
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    servicoNFSe.Empresa = this.Usuario.Empresa;

                Dominio.Entidades.ServicoNFSe servicoNFSeExiste = repServicoNFSe.BuscarPorNumeroELocalidadeECodigoTributacao(servicoNFSe.Numero, localidade, servicoNFSe.CodigoTributacao);

                if (servicoNFSeExiste == null || (servicoNFSeExiste.Codigo == servicoNFSe.Codigo))
                {
                    repServicoNFSe.Atualizar(servicoNFSe, Auditado);
                    unitOfWork.CommitChanges();
                    return new JsonpResult(true);
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, "Já existe uma natureza cadastrada para esse número");
                }
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
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorCodigo(codigo);
                var dynServicoNFSe = new
                {
                    servicoNFSe.Codigo,
                    servicoNFSe.Numero,
                    servicoNFSe.CNAE,
                    servicoNFSe.NBS,
                    servicoNFSe.CodigoTributacao,
                    Localidade = servicoNFSe.Localidade != null ? new { servicoNFSe.Localidade.Codigo, Descricao = servicoNFSe.Localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                    servicoNFSe.Descricao,
                    servicoNFSe.Status
                };
                return new JsonpResult(dynServicoNFSe);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorCodigo(codigo);
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

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NFS.FiltroPesquisaServicoNFSe()
            {
                Localidade = Request.GetIntParam("Localidade"),
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : SituacaoAtivoPesquisa.Ativo,
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.Empresa = this.Usuario.Empresa.Codigo;

            return filtrosPesquisa;
        }

        #endregion
    }
}
