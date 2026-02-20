using Dominio.Excecoes.Embarcador;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Financeiros
{
    [CustomAuthorize("Financeiros/RegraEntradaDocumento")]
    public class RegraEntradaDocumentoController : BaseController
    {
		#region Construtores

		public RegraEntradaDocumentoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos;
                Enum.TryParse(Request.Params("Ativo"), out ativo);
                string ncm = Request.Params("NCM");
                int codigoEmpresa, codigoNaturezaDaOperacao, codigoCFOPDentro, codigoCFOPFora = 0;
                int.TryParse(Request.Params("Empresa"), out codigoEmpresa);
                int.TryParse(Request.Params("NaturezaDaOperacao"), out codigoNaturezaDaOperacao);
                int.TryParse(Request.Params("CFOPDentro"), out codigoCFOPDentro);
                int.TryParse(Request.Params("CFOPFora"), out codigoCFOPFora);
                string descricao = Request.Params("Descricao");

                double codigoPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out codigoPessoa);

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Descrição", "Descricao", 30, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Empresa", "Empresa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Pessoa / Fornecedor", "Pessoa", 20, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho("Natureza da Operação", "NaturezaDaOperacao", 20, Models.Grid.Align.left, true);
                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho("Situação", "DescricaoAtivo", 10, Models.Grid.Align.center, false);

                string propOrdenar = grid.header[grid.indiceColunaOrdena].data;
                if (propOrdenar == "NaturezaDaOperacao")
                    propOrdenar = "NaturezaOperacao.Descricao";

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento> listaRegraEntradaDocumento = repRegraEntradaDocumento.Consultar(ativo, ncm, codigoEmpresa, codigoPessoa, codigoNaturezaDaOperacao, codigoCFOPDentro, codigoCFOPFora, descricao, propOrdenar, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repRegraEntradaDocumento.ContarConsulta(ativo, ncm, codigoEmpresa, codigoPessoa, codigoNaturezaDaOperacao, codigoCFOPDentro, codigoCFOPFora, descricao));
                var lista = (from p in listaRegraEntradaDocumento
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 Empresa = p.Empresa.RazaoSocial + " (" + p.Empresa.CNPJ_Formatado + ")",
                                 Pessoa = p.Pessoa != null ? p.Pessoa.Nome + " (" + p.Pessoa.CPF_CNPJ_Formatado + ")" : "Multiplos Fornecedores",
                                 NaturezaDaOperacao = p.NaturezaOperacao != null ? p.NaturezaOperacao.Descricao : string.Empty,
                                 p.DescricaoAtivo
                             }).ToList();
                grid.AdicionaRows(lista);
                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                unitOfWork.Dispose();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = new Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento();

                PreencherRegraEntradaDocumento(regraEntradaDocumento, unitOfWork);

                repRegraEntradaDocumento.Inserir(regraEntradaDocumento, Auditado);

                List<string> listaNCM = SalvarListaNCM(regraEntradaDocumento, unitOfWork);
                List<double> listaFornecedor = SalvarListaFornecedor(regraEntradaDocumento, unitOfWork);

                string mensagem = "";

                VerificarDuplicidade(regraEntradaDocumento, listaNCM, listaFornecedor, out mensagem, unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    throw new ControllerException(mensagem);

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

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = repRegraEntradaDocumento.BuscarPorCodigo(codigo, true);

                PreencherRegraEntradaDocumento(regraEntradaDocumento, unitOfWork);

                repRegraEntradaDocumento.Atualizar(regraEntradaDocumento, Auditado);

                List<string> listaNCM = SalvarListaNCM(regraEntradaDocumento, unitOfWork);
                List<double> listaFornecedor = SalvarListaFornecedor(regraEntradaDocumento, unitOfWork);

                string mensagem = "";

                VerificarDuplicidade(regraEntradaDocumento, listaNCM, listaFornecedor, out mensagem, unitOfWork);

                if (!string.IsNullOrWhiteSpace(mensagem))
                    throw new ControllerException(mensagem);

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

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = repRegraEntradaDocumento.BuscarPorCodigo(codigo);

                var dynRegra = new
                {
                    regraEntradaDocumento.Codigo,
                    regraEntradaDocumento.Descricao,
                    Empresa = new { Codigo = regraEntradaDocumento.Empresa != null ? regraEntradaDocumento.Empresa.Codigo : 0, Descricao = regraEntradaDocumento.Empresa != null ? "(" + regraEntradaDocumento.Empresa.RazaoSocial + ") " + regraEntradaDocumento.Empresa.CNPJ_Formatado : "" },
                    Pessoa = new { Codigo = regraEntradaDocumento.Pessoa != null ? regraEntradaDocumento.Pessoa.CPF_CNPJ : 0, Descricao = regraEntradaDocumento.Pessoa != null ? "(" + regraEntradaDocumento.Pessoa.Nome + ") " + regraEntradaDocumento.Pessoa.CPF_CNPJ_Formatado : "" },
                    NaturezaDaOperacao = new { Codigo = regraEntradaDocumento.NaturezaOperacao != null ? regraEntradaDocumento.NaturezaOperacao.Codigo : 0, Descricao = regraEntradaDocumento.NaturezaOperacao != null ? "(" + regraEntradaDocumento.NaturezaOperacao.Descricao + ") " : "" },
                    CFOPDentro = new { Codigo = regraEntradaDocumento.CFOPDentro != null ? regraEntradaDocumento.CFOPDentro.Codigo : 0, Descricao = regraEntradaDocumento.CFOPDentro != null ? "(" + regraEntradaDocumento.CFOPDentro.CodigoCFOP + ") " + regraEntradaDocumento.CFOPDentro.Extensao : "" },
                    CFOPFora = new { Codigo = regraEntradaDocumento.CFOPFora != null ? regraEntradaDocumento.CFOPFora.Codigo : 0, Descricao = regraEntradaDocumento.CFOPFora != null ? "(" + regraEntradaDocumento.CFOPFora.CodigoCFOP + ") " + regraEntradaDocumento.CFOPFora.Extensao : "" },
                    ListaNCM = (from p in regraEntradaDocumento.NCMs
                                select new
                                {
                                    p.Codigo,
                                    p.NCM
                                }).ToList(),
                    ListaFornecedor = (from p in regraEntradaDocumento.Fornecedores
                                       select new
                                       {
                                           p.Codigo,
                                           CodigoFornecedor = p.Pessoa.CPF_CNPJ,
                                           Fornecedor = p.Pessoa.Descricao
                                       }).ToList(),
                    regraEntradaDocumento.Ativo,
                    regraEntradaDocumento.ObrigarInformarVeiculo,
                    regraEntradaDocumento.FinalizarFaturarNotaAutomaticamente,
                    TributarICMS = regraEntradaDocumento.NaoTributarICMS,
                    regraEntradaDocumento.MultiplosFornecedores,
                    regraEntradaDocumento.IndicadorPagamento,
                    regraEntradaDocumento.NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento,
                    regraEntradaDocumento.NaoFinalizarQuandoArlaTiverQuantidadeSuperior,
                    regraEntradaDocumento.NaoFinalizarDocumentoSemProdutoPreCadastrado,
                    QuantidadeSuperiorArla = regraEntradaDocumento.QuantidadeSuperiorArla > 0 ? regraEntradaDocumento.QuantidadeSuperiorArla.ToString("n4") : string.Empty
                };

                return new JsonpResult(dynRegra);
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
                Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
                Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento = repRegraEntradaDocumento.BuscarPorCodigo(codigo);

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM repRegraEntradaDocumentoNCM = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM> listaRegraEntradaDocumentoNCM = repRegraEntradaDocumentoNCM.BuscarPorRegra(regraEntradaDocumento.Codigo);
                for (int i = 0; i < listaRegraEntradaDocumentoNCM.Count(); i++)
                    repRegraEntradaDocumentoNCM.Deletar(listaRegraEntradaDocumentoNCM[i]);

                Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor repRegraEntradaDocumentoFornecedor = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor(unitOfWork);
                List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor> listaRegraEntradaDocumentoFornecedor = repRegraEntradaDocumentoFornecedor.BuscarPorRegra(regraEntradaDocumento.Codigo);
                for (int i = 0; i < listaRegraEntradaDocumentoFornecedor.Count(); i++)
                    repRegraEntradaDocumentoFornecedor.Deletar(listaRegraEntradaDocumentoFornecedor[i]);

                repRegraEntradaDocumento.Deletar(regraEntradaDocumento);
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

        #region Métodos Privados

        private void VerificarDuplicidade(Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, List<string> listaNCM, List<double> listaFornecedor, out string mensagem, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumento repositorioRegraEntradaDocumento = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumento(unitOfWork);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor repositorioRegraEntradaDocumentoFornecedor = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor(unitOfWork);
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM repositorioRegraEntradaDocumentoNCM = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM(unitOfWork);

            List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento> regras = repositorioRegraEntradaDocumento.BuscarAtivas();
            List<(int CodigoRegra, double CpfCnpjFornecedor)> fornecedores = repositorioRegraEntradaDocumentoFornecedor.BuscarAtivos();
            List<(int CodigoRegra, string Ncm)> ncms = repositorioRegraEntradaDocumentoNCM.BuscarAtivos();

            int contador = 0;
            mensagem = "";

            foreach (Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regra in regras)
            {
                if (regra.Codigo == regraEntradaDocumento.Codigo) continue;

                contador = 0;

                if (regraEntradaDocumento.Empresa != null && regra.Empresa != null)
                {
                    if (regraEntradaDocumento.Empresa.Codigo == regra.Empresa.Codigo)
                        contador += 1;
                }

                if (regraEntradaDocumento.Pessoa != null && regra.Pessoa != null)
                {
                    if (regraEntradaDocumento.Pessoa.Codigo == regra.Pessoa.Codigo && !regraEntradaDocumento.MultiplosFornecedores)
                        contador += 1;
                }

                if (regraEntradaDocumento.MultiplosFornecedores)
                {
                    if ((from obj in fornecedores where obj.CodigoRegra == regra.Codigo && listaFornecedor.Contains(obj.CpfCnpjFornecedor) select obj).Count() > 0)
                        contador++;
                }

                if ((from obj in ncms where obj.CodigoRegra == regra.Codigo && listaNCM.Contains(obj.Ncm) select obj).Count() > 0)
                    contador++;

                if (contador >= 3)
                {
                    mensagem = $"Foi detectado um conflito com a regra: {regra.Descricao}.";
                    break;
                }
            }
        }

        private void PreencherRegraEntradaDocumento(Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
            int.TryParse(Request.Params("NaturezaDaOperacao"), out int codigoNaturezaDaOperacao);
            int.TryParse(Request.Params("CFOPDentro"), out int codigoCFOPDentro);
            int.TryParse(Request.Params("CFOPFora"), out int codigoCFOPFora);

            double.TryParse(Request.Params("Pessoa"), out double codigoPessoa);

            regraEntradaDocumento.Descricao = Request.GetStringParam("Descricao");
            regraEntradaDocumento.IndicadorPagamento = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.IndicadorPagamentoDocumentoEntrada>("IndicadorPagamento");
            regraEntradaDocumento.MultiplosFornecedores = Request.GetBoolParam("MultiplosFornecedores");
            regraEntradaDocumento.Ativo = Request.GetBoolParam("Ativo");
            regraEntradaDocumento.ObrigarInformarVeiculo = Request.GetBoolParam("ObrigarInformarVeiculo");
            regraEntradaDocumento.FinalizarFaturarNotaAutomaticamente = Request.GetBoolParam("FinalizarFaturarNotaAutomaticamente");
            regraEntradaDocumento.NaoTributarICMS = Request.GetBoolParam("TributarICMS");

            regraEntradaDocumento.Empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);
            regraEntradaDocumento.Pessoa = codigoPessoa > 0 ? repCliente.BuscarPorCPFCNPJ(codigoPessoa) : null;
            regraEntradaDocumento.NaturezaOperacao = codigoNaturezaDaOperacao > 0 ? repNaturezaDaOperacao.BuscarPorId(codigoNaturezaDaOperacao) : null;
            regraEntradaDocumento.CFOPDentro = codigoCFOPDentro > 0 ? repCFOP.BuscarPorId(codigoCFOPDentro) : null;
            regraEntradaDocumento.CFOPFora = codigoCFOPFora > 0 ? repCFOP.BuscarPorId(codigoCFOPFora) : null;

            regraEntradaDocumento.NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento = Request.GetBoolParam("NaoFinalizarQuandoArlaEstiverAssociadaReboqueEquipamento");
            regraEntradaDocumento.NaoFinalizarDocumentoSemProdutoPreCadastrado = Request.GetBoolParam("NaoFinalizarDocumentoSemProdutoPreCadastrado");
            regraEntradaDocumento.NaoFinalizarQuandoArlaTiverQuantidadeSuperior = Request.GetBoolParam("NaoFinalizarQuandoArlaTiverQuantidadeSuperior");
            regraEntradaDocumento.QuantidadeSuperiorArla = Request.GetDecimalParam("QuantidadeSuperiorArla");
            if (regraEntradaDocumento.NaoFinalizarQuandoArlaTiverQuantidadeSuperior && regraEntradaDocumento.QuantidadeSuperiorArla == 0)
                throw new ControllerException("Obrigatório preencher a quantidade da ARLA");
        }

        private List<string> SalvarListaNCM(Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM repRegraEntradaDocumentoNCM = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoNCM(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM> listaRegraEntradaDocumentoNCM = repRegraEntradaDocumentoNCM.BuscarPorRegra(regraEntradaDocumento.Codigo);
            for (int i = 0; i < listaRegraEntradaDocumentoNCM.Count(); i++)
                repRegraEntradaDocumentoNCM.Deletar(listaRegraEntradaDocumentoNCM[i], Auditado);

            List<string> listaRegraEntradaDocumentoNCMRetornar = new List<string>();

            dynamic listaNCM = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaNCM"));
            if (listaNCM != null)
            {
                foreach (var ncm in listaNCM)
                {
                    Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM regraEntradaDocumentoNCM = new Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoNCM();
                    regraEntradaDocumentoNCM.NCM = (string)ncm.NCM;
                    regraEntradaDocumentoNCM.RegraEntradaDocumento = regraEntradaDocumento;
                    repRegraEntradaDocumentoNCM.Inserir(regraEntradaDocumentoNCM, Auditado);
                    listaRegraEntradaDocumentoNCMRetornar.Add(regraEntradaDocumentoNCM.NCM);
                }
            }

            return listaRegraEntradaDocumentoNCMRetornar;
        }

        private List<double> SalvarListaFornecedor(Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumento regraEntradaDocumento, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor repRegraEntradaDocumentoFornecedor = new Repositorio.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor(unidadeDeTrabalho);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

            List<Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor> listaRegraEntradaDocumentoFornecedor = repRegraEntradaDocumentoFornecedor.BuscarPorRegra(regraEntradaDocumento.Codigo);
            for (int i = 0; i < listaRegraEntradaDocumentoFornecedor.Count(); i++)
                repRegraEntradaDocumentoFornecedor.Deletar(listaRegraEntradaDocumentoFornecedor[i], Auditado);

            List<double> listaRegraEntradaDocumentoFornecedorRetornar = new List<double>();

            if (regraEntradaDocumento.MultiplosFornecedores)
            {
                dynamic listaFornecedor = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaFornecedor"));
                if (listaFornecedor != null)
                {
                    foreach (var fornecedor in listaFornecedor)
                    {
                        double codigoFornecedor = 0;
                        double.TryParse((string)fornecedor.CodigoFornecedor, out codigoFornecedor);
                        if (codigoFornecedor > 0)
                        {
                            Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor regraEntradaDocumentoFornecedor = new Dominio.Entidades.Embarcador.Financeiro.RegraEntradaDocumentoFornecedor();
                            regraEntradaDocumentoFornecedor.Pessoa = repCliente.BuscarPorCPFCNPJ(codigoFornecedor);
                            regraEntradaDocumentoFornecedor.RegraEntradaDocumento = regraEntradaDocumento;
                            repRegraEntradaDocumentoFornecedor.Inserir(regraEntradaDocumentoFornecedor, Auditado);
                            listaRegraEntradaDocumentoFornecedorRetornar.Add(regraEntradaDocumentoFornecedor.Pessoa.CPF_CNPJ);
                        }
                    }
                }
            }

            return listaRegraEntradaDocumentoFornecedorRetornar;
        }

        #endregion
    }
}
