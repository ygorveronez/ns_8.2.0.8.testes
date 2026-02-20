using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NFS
{
    [CustomAuthorize("NFS/NaturezaNFSe")]
    public class NaturezaNFSeController : BaseController
    {
		#region Construtores

		public NaturezaNFSeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.Embarcador.Operacional.OperadorLogistica repOperadorLogistica = new Repositorio.Embarcador.Operacional.OperadorLogistica(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                string descricao = Request.Params("Descricao");
                string dentroForaEstado = Request.Params("DentroForaEstado");
                string tipo = Request.Params("Tipo");
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                int localidade = 0;
                int codigoEmpresa = 0;
                int.TryParse(Request.Params("Localidade"), out localidade);
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                double codigoPessoa = 0d;
                double.TryParse(Request.Params("Pessoa"), out codigoPessoa);

                Dominio.Entidades.Cliente pessoa = codigoPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(codigoPessoa) : null;

                bool? dentroEstado = null;
                if (pessoa != null)
                    dentroEstado = pessoa.Localidade.Estado.Sigla == this.Usuario.Empresa.Localidade.Estado.Sigla;
                else if (!string.IsNullOrWhiteSpace(dentroForaEstado))
                {
                    if (dentroForaEstado == "D")
                        dentroEstado = true;
                    else
                        dentroEstado = false;
                }

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida tipoEntradaSaida = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEntradaSaida.Todos;
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    int tipoES = 0;
                    if (tipo == "S")
                        tipoES = 2;
                    else if (tipo == "E")
                        tipoES = 1;

                    if (tipoES > 0)
                        Enum.TryParse(tipoES.ToString(), out tipoEntradaSaida);
                }

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "Numero", 15, Models.Grid.Align.center, false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 45, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Localidade, "Localidade", 25, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoStatus", 10, Models.Grid.Align.center, false);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Localidade")
                    propOrdena += ".Descricao";

                List<Dominio.Entidades.NaturezaNFSe> listaNaturezaNFSe = repNaturezaNFSe.Consultar(descricao, localidade, ativo, codigoEmpresa, tipoEntradaSaida, dentroEstado, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNaturezaNFSe.ContarConsulta(descricao, localidade, ativo, codigoEmpresa, tipoEntradaSaida, dentroEstado));
                var lista = (from p in listaNaturezaNFSe
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.Numero,
                                 Localidade = p.Localidade != null ? p.Localidade.DescricaoCidadeEstado : "",
                                 p.DescricaoStatus
                             }).ToList();
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

                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoMigrateNFSeNatureza repIntegracaoMigrateNFSeNatureza = new Repositorio.Embarcador.Integracao.IntegracaoMigrateNFSeNatureza(unitOfWork);

                Dominio.Entidades.NaturezaNFSe naturezaNFSe = new Dominio.Entidades.NaturezaNFSe();

                int localidade = 0, numero = 0, codigoIntegracaoMigrateNFSeNatureza = 0;
                int.TryParse(Request.Params("Localidade"), out localidade);
                int.TryParse(Request.Params("Numero"), out numero);
                int.TryParse(Request.Params("IntegracaoMigrateNFSeNatureza"), out codigoIntegracaoMigrateNFSeNatureza);

                naturezaNFSe.Status = Request.Params("Status");
                naturezaNFSe.Descricao = Request.Params("Descricao");
                naturezaNFSe.Numero = numero;
                naturezaNFSe.Localidade = new Dominio.Entidades.Localidade() { Codigo = localidade };
                naturezaNFSe.IntegracaoMigrateNFSeNatureza = repIntegracaoMigrateNFSeNatureza.BuscarPorCodigo(codigoIntegracaoMigrateNFSeNatureza, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    naturezaNFSe.Empresa = this.Usuario.Empresa;

                if (repNaturezaNFSe.BuscarPorNumeroELocalidade(naturezaNFSe.Numero, localidade) == null)
                {
                    repNaturezaNFSe.Inserir(naturezaNFSe, Auditado);
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
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.Embarcador.Integracao.IntegracaoMigrateNFSeNatureza repIntegracaoMigrateNFSeNatureza = new Repositorio.Embarcador.Integracao.IntegracaoMigrateNFSeNatureza(unitOfWork);

                Dominio.Entidades.NaturezaNFSe naturezaNFSe = repNaturezaNFSe.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int localidade = 0, numero = 0, codigoIntegracaoMigrateNFSeNatureza = 0;
                int.TryParse(Request.Params("Localidade"), out localidade);
                int.TryParse(Request.Params("Numero"), out numero);
                int.TryParse(Request.Params("IntegracaoMigrateNFSeNatureza"), out codigoIntegracaoMigrateNFSeNatureza);

                naturezaNFSe.Status = Request.Params("Status");
                naturezaNFSe.Descricao = Request.Params("Descricao");
                naturezaNFSe.Numero = numero;
                naturezaNFSe.Localidade = new Dominio.Entidades.Localidade() { Codigo = localidade };
                naturezaNFSe.IntegracaoMigrateNFSeNatureza = repIntegracaoMigrateNFSeNatureza.BuscarPorCodigo(codigoIntegracaoMigrateNFSeNatureza, false);

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    naturezaNFSe.Empresa = this.Usuario.Empresa;

                Dominio.Entidades.NaturezaNFSe naturezaNFSeExiste = repNaturezaNFSe.BuscarPorNumeroELocalidade(naturezaNFSe.Numero, localidade);

                if (naturezaNFSeExiste == null || (naturezaNFSeExiste.Codigo == naturezaNFSe.Codigo))
                {
                    repNaturezaNFSe.Atualizar(naturezaNFSe, Auditado);
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
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                Dominio.Entidades.NaturezaNFSe naturezaNFSe = repNaturezaNFSe.BuscarPorCodigo(codigo);
                var dynNaturezaNFSe = new
                {
                    naturezaNFSe.Codigo,
                    naturezaNFSe.Numero,
                    Localidade = naturezaNFSe.Localidade != null ? new { naturezaNFSe.Localidade.Codigo, Descricao = naturezaNFSe.Localidade.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                    naturezaNFSe.Descricao,
                    naturezaNFSe.Status,
                    IntegracaoMigrateNFSeNatureza = naturezaNFSe.IntegracaoMigrateNFSeNatureza != null ? new { naturezaNFSe.IntegracaoMigrateNFSeNatureza.Codigo, Descricao = naturezaNFSe.IntegracaoMigrateNFSeNatureza.Descricao } : new { Codigo = 0, Descricao = "" },

                };
                return new JsonpResult(dynNaturezaNFSe);
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
                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);
                Dominio.Entidades.NaturezaNFSe naturezaNFSe = repNaturezaNFSe.BuscarPorCodigo(codigo);
                repNaturezaNFSe.Deletar(naturezaNFSe, Auditado);
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

        #endregion
    }
}
