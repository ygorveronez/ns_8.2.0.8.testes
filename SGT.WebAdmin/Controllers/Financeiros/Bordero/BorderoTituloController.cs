using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros.Bordero
{
    [CustomAuthorize("Financeiros/Bordero")]
    public class BorderoTituloController : BaseController
    {
		#region Construtores

		public BorderoTituloController(Conexao conexao) : base(conexao) { }

		#endregion

        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoBordero);
                int.TryParse(Request.Params("Titulo"), out int codigoTitulo);
                int.TryParse(Request.Params("CTe"), out int numeroCTe);

                string numeroCarga = Request.Params("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Título", "Numero", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Documento", "Documento", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Vencto.", "DataVencimento", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 20, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Vl. a Cobrar", "ValorACobrar", 10, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Acréscimo", "ValorTotalAcrescimo", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Desconto", "ValorTotalDesconto", 8, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Vl. Total", "ValorTotalACobrar", 10, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                if (propOrdena == "Numero")
                    propOrdena = "Titulo.Codigo";
                else if (propOrdena == "DataEmissao" || propOrdena == "DataVencimento")
                    propOrdena = "Titulo." + propOrdena;

                Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo> listaTitulosBordero = repBorderoTitulo.Consultar(codigoBordero, codigoTitulo, numeroCTe, numeroCarga, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                int countTitulosBordero = repBorderoTitulo.ContarConsulta(codigoBordero, codigoTitulo, numeroCTe, numeroCarga);

                var lista = (from p in listaTitulosBordero
                             select new
                             {
                                 p.Codigo,
                                 Numero = p.Titulo.Codigo,
                                 Documento = p.Titulo.NumeroDocumentos,
                                 DataEmissao = p.Titulo.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 DataVencimento = p.Titulo.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 Tomador = p.Titulo.Pessoa != null ? p.Titulo.Pessoa.Nome + " (" + p.Titulo.Pessoa.CPF_CNPJ_Formatado + ")" : p.Titulo.GrupoPessoas?.Descricao ?? string.Empty,
                                 ValorACobrar = p.ValorACobrar.ToString("n2"),
                                 ValorTotalAcrescimo = p.ValorTotalAcrescimo.ToString("n2"),
                                 ValorTotalDesconto = p.ValorTotalDesconto.ToString("n2"),
                                 ValorTotalACobrar = p.ValorTotalACobrar.ToString("n2")
                             }).ToList();

                grid.setarQuantidadeTotal(countTitulosBordero);
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

        public async Task<IActionResult> PesquisaTitulosParaBordero()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Bordero"), out int codigoBordero);
                int.TryParse(Request.Params("NumeroTitulo"), out int codigoTitulo);
                int.TryParse(Request.Params("CTe"), out int numeroCTe);

                string numeroCarga = Request.Params("Carga");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Número", "Codigo", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Documento", "Documento", 12, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Data Vencto.", "DataVencimento", 12, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Tomador", "Tomador", 30, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("Valor", "ValorOriginal", 12, Models.Grid.Align.left, true);

                string propOrdena = grid.header[grid.indiceColunaOrdena].data;

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigoBordero);

                if (bordero == null)
                    return new JsonpResult(false, true, "Borderô não encontrado.");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                List<Dominio.Entidades.Embarcador.Financeiro.Titulo> listaTitulos = repTitulo.ConsultarTitulosAReceberPendentesParaBordero(bordero.Cliente?.CPF_CNPJ ?? 0d, bordero.GrupoPessoas?.Codigo ?? 0, numeroCTe, numeroCarga, codigoTitulo, propOrdena, grid.dirOrdena, grid.inicio, grid.limite);
                int countTitulos = repTitulo.ContarConsultaTitulosAReceberPendentesParaBordero(bordero.Cliente?.CPF_CNPJ ?? 0d, bordero.GrupoPessoas?.Codigo ?? 0, numeroCTe, numeroCarga, codigoTitulo);

                var lista = (from p in listaTitulos
                             select new
                             {
                                 p.Codigo,
                                 Documento = p.NumeroDocumentos,
                                 DataEmissao = p.DataEmissao?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 DataVencimento = p.DataVencimento?.ToString("dd/MM/yyyy") ?? string.Empty,
                                 Tomador = p.Pessoa != null ? p.Pessoa.Nome + " (" + p.Pessoa.CPF_CNPJ_Formatado + ")" : p.GrupoPessoas?.Descricao ?? string.Empty,
                                 ValorOriginal = p.ValorOriginal.ToString("n2"),
                             }).ToList();

                grid.setarQuantidadeTotal(countTitulos);
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

        public async Task<IActionResult> AdicionarTitulos()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Bordero"), out int codigoBordero);

                List<int> codigosTitulos = JsonConvert.DeserializeObject<List<int>>(Request.Params("Titulos"));

                Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(codigoBordero);

                if (bordero == null)
                    return new JsonpResult(false, true, "Borderô não encontrado.");

                if (bordero.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoBordero.EmAndamento)
                    return new JsonpResult(false, true, "A situação atual do borderô não permite adicionar títulos.");

                Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(unitOfWork);

                unitOfWork.Start();

                foreach (int codigoTitulo in codigosTitulos)
                {
                    Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = repTitulo.BuscarPorCodigo(codigoTitulo);

                    if (!Servicos.Embarcador.Financeiro.Bordero.AdicionarTituloAoBordero(out string erro, bordero, titulo, unitOfWork))
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, erro);
                    }

                    Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Adicionou ao Borderô " + bordero.Descricao + ".", unitOfWork);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Adicionou o Título " + titulo.Descricao + ".", unitOfWork);
                }

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
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

        public async Task<IActionResult> RemoverTitulo()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int.TryParse(Request.Params("Codigo"), out int codigoBorderoTitulo);

                Repositorio.Embarcador.Financeiro.BorderoTitulo repBorderoTitulo = new Repositorio.Embarcador.Financeiro.BorderoTitulo(unitOfWork);

                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBorderoTitulo.BuscarBorderoPorTitulo(codigoBorderoTitulo);
                Dominio.Entidades.Embarcador.Financeiro.BorderoTitulo titulo = repBorderoTitulo.BuscarPorCodigo(codigoBorderoTitulo);
                
                unitOfWork.Start();

                if (!Servicos.Embarcador.Financeiro.Bordero.RemoverTituloDoBordero(out string erro, codigoBorderoTitulo, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, erro);
                }
                Servicos.Auditoria.Auditoria.Auditar(Auditado, titulo, null, "Removeu do Borderô " + bordero.Descricao + ".", unitOfWork);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, bordero, null, "Removeu o Título " + titulo.Descricao + ".", unitOfWork);

                Servicos.Embarcador.Financeiro.Bordero.AtualizarTotaisBordero(ref bordero, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(Servicos.Embarcador.Financeiro.Bordero.ObterDetalhesBordero(bordero));
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
    }
}
