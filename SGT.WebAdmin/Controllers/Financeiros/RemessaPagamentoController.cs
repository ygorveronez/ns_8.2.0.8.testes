using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/RemessaPagamento")]
    public class RemessaPagamentoController : BaseController
    {
		#region Construtores

		public RemessaPagamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> BuscarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigo = int.Parse(Request.Params("Codigo"));
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);
                var dynTipoMovimento = new
                {
                    pagamentoEletronico.Codigo,
                    pagamentoEletronico.Numero,
                    DataPagamento = pagamentoEletronico.DataPagamento.Value.ToString("dd/MM/yyyy"),
                    DataGeracao = pagamentoEletronico.DataGeracao.Value.ToString("dd/MM/yyyy"),
                    BoletoConfiguracao = new { Codigo = pagamentoEletronico.BoletoConfiguracao != null ? pagamentoEletronico.BoletoConfiguracao.Codigo : 0, Descricao = pagamentoEletronico.BoletoConfiguracao != null ? pagamentoEletronico.BoletoConfiguracao.BoletoBanco.ObterDescricao() : "" },
                    Empresa = new { Codigo = pagamentoEletronico.Empresa != null ? pagamentoEletronico.Empresa.Codigo : 0, Descricao = pagamentoEletronico.Empresa != null ? pagamentoEletronico.Empresa.RazaoSocial : "" },
                    LayoutEDI = new { Codigo = pagamentoEletronico.LayoutEDI != null ? pagamentoEletronico.LayoutEDI.Codigo : 0, Descricao = pagamentoEletronico.LayoutEDI != null ? pagamentoEletronico.LayoutEDI.Descricao : "" },
                    pagamentoEletronico.ModalidadePagamentoEletronico,
                    pagamentoEletronico.TipoContaPagamentoEletronico,
                    pagamentoEletronico.FinalidadePagamentoEletronico,
                    pagamentoEletronico.DescricaoUsoEmpresaPagamentoEletronico,
                    Usuario = new { Codigo = pagamentoEletronico.Usuario != null ? pagamentoEletronico.Usuario.Codigo : 0, Descricao = pagamentoEletronico.Usuario != null ? pagamentoEletronico.Usuario.Nome : "" },
                    ValorTotal = pagamentoEletronico.ValorTotal.ToString("n2"),
                    QuantidadeTitulos = pagamentoEletronico.QuantidadeTitulos.ToString("n0"),
                    pagamentoEletronico.SituacaoAutorizacaoPagamentoEletronico
                };
                return new JsonpResult(dynTipoMovimento);
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
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico> repositorioAprovacao = new Repositorio.Embarcador.RegraAutorizacao.AprovacaoAlcada<Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.AprovacaoAlcadaPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.AlcadasPagamentoEletronico.RegraAutorizacaoPagamentoEletronico, Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico>(unitOfWork);

                if (repositorioAprovacao.ContemAlcadaPendente(codigo))
                    return new JsonpResult(false, false, "Existe uma alçada pendente de aprovação, deve-se rejeitar a mesma para excluir.");

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repPagamentoEletronicoTitulo.BuscarPorTitulosRemessa(pagamentoEletronico.Codigo);
                for (int i = 0; i < listaTitulo.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];
                    titulo.Historico += " REMOVIDO DA REMESSA DE PAGAMENTO " + this.Usuario.Nome + " " + DateTime.Now.ToString("dd/MM/yyyy");
                    titulo.Historico += " Nº ANTERIOR: " + pagamentoEletronico.Numero.ToString();

                    repTitulo.Atualizar(titulo);
                }

                //unitOfWork.Start();

                List<Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronicoTitulo> listaPagamentoEletronicoTitulo = repPagamentoEletronicoTitulo.BuscarPorPagamento(pagamentoEletronico.Codigo);
                foreach (var pagTitulo in listaPagamentoEletronicoTitulo)
                    repPagamentoEletronicoTitulo.Deletar(pagTitulo);

                repPagamentoEletronico.Deletar(pagamentoEletronico, Auditado);
                //unitOfWork.CommitChanges();
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                //unitOfWork.Rollback();
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

        public async Task<IActionResult> ReenviarPorCodigo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {

                int codigo = int.Parse(Request.Params("codigo"));
                Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(unitOfWork);
                Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo repPagamentoEletronicoTitulo = new Repositorio.Embarcador.Financeiro.PagamentoEletronicoTitulo(unitOfWork);
                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(codigo);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulo = repPagamentoEletronicoTitulo.BuscarPorTitulosRemessa(pagamentoEletronico.Codigo);

                unitOfWork.Start();

                for (int i = 0; i < listaTitulo.Count(); i++)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = listaTitulo[i];
                    titulo.Historico += " SOLICITADO NOVO REENVIO DE REMESSA " + this.Usuario.Nome + " " + DateTime.Now.ToString("dd/MM/yyyy");
                    titulo.BoletoStatusTitulo = BoletoStatusTitulo.AguardandoRemessa;

                    repTitulo.Atualizar(titulo);
                }
                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao reenviar.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
