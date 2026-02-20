using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
namespace SGT.WebAdmin.Controllers.FaturamentoMensal
{
    [CustomAuthorize("FaturamentosMensais/PlanoEmissaoFaturamento")]
    public class PlanoEmissaoFaturamentoController : BaseController
    {
		#region Construtores

		public PlanoEmissaoFaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                string descricao = Request.Params("Descricao");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
                List<Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe> listaPlanoEmissaoNFe = repPlanoEmissaoNFe.Consulta(descricao, ativo, this.Usuario.Empresa.Codigo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repPlanoEmissaoNFe.ContaConsulta(descricao, ativo, this.Usuario.Empresa.Codigo));
                var lista = (from p in listaPlanoEmissaoNFe
                             select new
                             {
                                 p.Codigo,
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

                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor repPlanoEmissaoNFeValor = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor(unitOfWork);

                bool ativo, cobrancaNFe, cobrancaNFSe, cobrancaBoleto, cobrancaTitulo;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("CobrancaNFe"), out cobrancaNFe);
                bool.TryParse(Request.Params("CobrancaNFSe"), out cobrancaNFSe);
                bool.TryParse(Request.Params("CobrancaBoleto"), out cobrancaBoleto);
                bool.TryParse(Request.Params("CobrancaTitulo"), out cobrancaTitulo);

                decimal valorAdesao = 0;
                decimal.TryParse(Request.Params("ValorAdesao"), out valorAdesao);

                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);

                Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe planoEmissaoNFe = new Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe();
                planoEmissaoNFe.Ativo = ativo;
                planoEmissaoNFe.CobrancaBoleto = cobrancaBoleto;
                planoEmissaoNFe.CobrancaNFe = cobrancaNFe;
                planoEmissaoNFe.CobrancaNFSe = cobrancaNFSe;
                planoEmissaoNFe.CobrancaTitulo = cobrancaTitulo;
                planoEmissaoNFe.Descricao = descricao;
                planoEmissaoNFe.Empresa = this.Usuario.Empresa;
                planoEmissaoNFe.Observacao = observacao;
                planoEmissaoNFe.TipoObservacaoFaturamentoMensal = tipoObservacao;
                planoEmissaoNFe.ValorAdesao = valorAdesao;

                repPlanoEmissaoNFe.Inserir(planoEmissaoNFe, Auditado);

                dynamic dynValores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ValoresPlano"));
                foreach (var dynValor in dynValores)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacaoServicoExtra = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                    Enum.TryParse((string)dynValor.TipoObservacaoValor, out tipoObservacaoServicoExtra);

                    int qtdInicial = 0, qtdFinal = 0;
                    int.TryParse(((string)dynValor.QuantidadeInicial).Replace(".", ""), out qtdInicial);
                    int.TryParse(((string)dynValor.QuantidadeFinal).Replace(".", ""), out qtdFinal);

                    decimal valorPlano = 0;
                    decimal.TryParse(((string)dynValor.Valor).Replace(".", ""), out valorPlano);

                    Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor valor = new Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor
                    {
                        Descricao = (string)dynValor.DescricaoValor,
                        Observacao = (string)dynValor.ObservacaoValor,
                        TipoObservacaoFaturamentoMensal = tipoObservacaoServicoExtra,
                        PlanoEmissaoNFe = planoEmissaoNFe,
                        QuantidadeFinal = qtdFinal,
                        QuantidadeInicial = qtdInicial,
                        Valor = valorPlano
                    };

                    repPlanoEmissaoNFeValor.Inserir(valor, Auditado);
                }

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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

                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor repPlanoEmissaoNFeValor = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor(unitOfWork);

                bool ativo, cobrancaNFe, cobrancaNFSe, cobrancaBoleto, cobrancaTitulo;
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("CobrancaNFe"), out cobrancaNFe);
                bool.TryParse(Request.Params("CobrancaNFSe"), out cobrancaNFSe);
                bool.TryParse(Request.Params("CobrancaBoleto"), out cobrancaBoleto);
                bool.TryParse(Request.Params("CobrancaTitulo"), out cobrancaTitulo);

                decimal valorAdesao = 0;
                decimal.TryParse(Request.Params("ValorAdesao"), out valorAdesao);

                string descricao = Request.Params("Descricao");
                string observacao = Request.Params("Observacao");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                Enum.TryParse(Request.Params("TipoObservacao"), out tipoObservacao);

                Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe planoEmissaoNFe = repPlanoEmissaoNFe.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                planoEmissaoNFe.Ativo = ativo;
                planoEmissaoNFe.CobrancaBoleto = cobrancaBoleto;
                planoEmissaoNFe.CobrancaNFe = cobrancaNFe;
                planoEmissaoNFe.CobrancaNFSe = cobrancaNFSe;
                planoEmissaoNFe.CobrancaTitulo = cobrancaTitulo;
                planoEmissaoNFe.Descricao = descricao;
                planoEmissaoNFe.Empresa = this.Usuario.Empresa;
                planoEmissaoNFe.Observacao = observacao;
                planoEmissaoNFe.TipoObservacaoFaturamentoMensal = tipoObservacao;
                planoEmissaoNFe.ValorAdesao = valorAdesao;

                repPlanoEmissaoNFeValor.DeletarPorPlanoEmissao(planoEmissaoNFe.Codigo);

                dynamic dynValores = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>((string)Request.Params("ValoresPlano"));
                foreach (var dynValor in dynValores)
                {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal tipoObservacaoServicoExtra = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoObservacaoFaturamentoMensal.Nenhum;
                    Enum.TryParse((string)dynValor.TipoObservacaoValor, out tipoObservacaoServicoExtra);

                    int qtdInicial = 0, qtdFinal = 0;
                    int.TryParse(((string)dynValor.QuantidadeInicial).Replace(".", ""), out qtdInicial);
                    int.TryParse(((string)dynValor.QuantidadeFinal).Replace(".", ""), out qtdFinal);

                    decimal valorPlano = 0;
                    decimal.TryParse(((string)dynValor.Valor).Replace(".", ""), out valorPlano);

                    Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor valor = new Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFeValor
                    {
                        Descricao = (string)dynValor.DescricaoValor,
                        Observacao = (string)dynValor.ObservacaoValor,
                        TipoObservacaoFaturamentoMensal = tipoObservacaoServicoExtra,
                        PlanoEmissaoNFe = planoEmissaoNFe,
                        QuantidadeFinal = qtdFinal,
                        QuantidadeInicial = qtdInicial,
                        Valor = valorPlano
                    };

                    repPlanoEmissaoNFeValor.Inserir(valor, Auditado);
                }

                repPlanoEmissaoNFe.Atualizar(planoEmissaoNFe, Auditado);

                unitOfWork.CommitChanges();
                return new JsonpResult(true);
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

                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe planoEmissaoNFe = repPlanoEmissaoNFe.BuscarPorCodigo(codigo);

                var dynPlanoEmissaoNFe = new
                {
                    planoEmissaoNFe.Ativo,
                    planoEmissaoNFe.CobrancaBoleto,
                    planoEmissaoNFe.CobrancaNFe,
                    planoEmissaoNFe.CobrancaNFSe,
                    planoEmissaoNFe.CobrancaTitulo,
                    planoEmissaoNFe.Codigo,
                    planoEmissaoNFe.Descricao,
                    planoEmissaoNFe.Observacao,
                    TipoObservacaoValor = planoEmissaoNFe.TipoObservacaoFaturamentoMensal,
                    planoEmissaoNFe.ValorAdesao,
                    ValoresPlano = planoEmissaoNFe.Valores != null && planoEmissaoNFe.Valores.Count > 0 ? (from obj in planoEmissaoNFe.Valores
                                                                                                           select new
                                                                                                           {
                                                                                                               obj.Codigo,
                                                                                                               QuantidadeInicial = obj.QuantidadeInicial.ToString("n0"),
                                                                                                               QuantidadeFinal = obj.QuantidadeFinal.ToString("n0"),
                                                                                                               Valor = obj.Valor.ToString("n2"),
                                                                                                               DescricaoValor = obj.Descricao,
                                                                                                               TipoObservacaoValor = obj.TipoObservacaoFaturamentoMensal,
                                                                                                               ObservacaoValor = obj.Observacao
                                                                                                           }).ToList() : null
                };
                return new JsonpResult(dynPlanoEmissaoNFe);
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

                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe repPlanoEmissaoNFe = new Repositorio.Embarcador.FaturamentoMensal.PlanoEmissaoNFe(unitOfWork);
                Dominio.Entidades.Embarcador.FaturamentoMensal.PlanoEmissaoNFe planoEmissaoNFe = repPlanoEmissaoNFe.BuscarPorCodigo(codigo);

                repPlanoEmissaoNFe.Deletar(planoEmissaoNFe, Auditado);

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
    }
}
