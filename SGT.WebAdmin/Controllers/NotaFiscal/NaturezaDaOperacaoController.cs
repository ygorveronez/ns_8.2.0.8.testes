using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.NotaFiscal
{
    [CustomAuthorize("NotasFiscais/NaturezaDaOperacao")]
    public class NaturezaDaOperacaoController : BaseController
    {
		#region Construtores

		public NaturezaDaOperacaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

                string descricao = Request.Params("Descricao");
                string status = Request.Params("Status");
                string dentroForaEstado = Request.Params("DentroForaEstado");

                int.TryParse(Request.Params("IndicadorPresenca"), out int indicadorPresenca);
                int.TryParse(Request.Params("Empresa"), out int codigoEmpresa);
                int codigoLocalidadeTerminoPrestacao = Request.GetIntParam("LocalidadeTerminoPrestacao");

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    codigoEmpresa = this.Usuario.Empresa.Codigo;

                double.TryParse(Request.Params("Pessoa"), out double codigoPessoa);

                Dominio.Entidades.Cliente pessoa = codigoPessoa > 0d ? repCliente.BuscarPorCPFCNPJ(codigoPessoa) : null;
                Dominio.Entidades.Empresa empresa = codigoEmpresa > 0 ? repEmpresa.BuscarPorCodigo(codigoEmpresa) : null;
                Dominio.Entidades.Localidade localidadeTerminoPrestacao = codigoLocalidadeTerminoPrestacao > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidadeTerminoPrestacao) : null;

                TipoEntradaSaida tipoEntradaSaida = TipoEntradaSaida.Todos;
                string tipo = Request.Params("Tipo");
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    int.TryParse(tipo, out int tipoES);
                    if (tipoES >= 0)
                    {
                        tipoES = tipoES + 1;
                        tipo = tipoES.ToString();
                        Enum.TryParse(tipo, out tipoEntradaSaida);
                    }
                }

                bool? dentroEstado = null;
                if (localidadeTerminoPrestacao != null)
                    dentroEstado = localidadeTerminoPrestacao.Estado.Sigla == (empresa?.Localidade.Estado.Sigla ?? Usuario.Empresa.Localidade.Estado.Sigla) ? true : false;
                else if (pessoa != null)
                {
                    if (indicadorPresenca == 1 && pessoa.IndicadorIE == IndicadorIE.NaoContribuinte)
                        dentroEstado = true;
                    else
                        dentroEstado = pessoa.Localidade.Estado.Sigla == (empresa?.Localidade.Estado.Sigla ?? Usuario.Empresa.Localidade.Estado.Sigla) ? true : false;
                }
                else if (!string.IsNullOrWhiteSpace(dentroForaEstado))
                {
                    if (dentroForaEstado == "S")
                        dentroEstado = true;
                    else
                        dentroEstado = false;
                }

                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    codigoEmpresa = 0;
                else
                    codigoEmpresa = this.Usuario?.Empresa?.Codigo ?? 0;

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Codigo, "Codigo", 8, Models.Grid.Align.right, true);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);
                if (string.IsNullOrWhiteSpace(status))
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Status, "DescricaoStatus", 15, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho("CFOP", false);
                grid.AdicionarCabecalho("CodigoCFOP", false);

                List<Dominio.Entidades.NaturezaDaOperacao> listaNaturezaDaOperacao = repNaturezaDaOperacao.Consultar(descricao, codigoEmpresa, status, tipoEntradaSaida, dentroEstado, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repNaturezaDaOperacao.ContarConsulta(descricao, codigoEmpresa, status, tipoEntradaSaida, dentroEstado));
                var lista = (from p in listaNaturezaDaOperacao
                             select new
                             {
                                 p.Codigo,
                                 p.Descricao,
                                 p.DescricaoStatus,
                                 CFOP = p.CFOP?.CodigoCFOP ?? 0,
                                 CodigoCFOP = p.CFOP?.Codigo ?? 0
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

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = new Dominio.Entidades.NaturezaDaOperacao();

                PreencherNaturezaDaOperacao(naturezaDaOperacao, unitOfWork);

                repNaturezaDaOperacao.Inserir(naturezaDaOperacao, Auditado);

                SalvarCFOPs(ref naturezaDaOperacao, unitOfWork);

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

                int codigo = Request.GetIntParam("Codigo");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigo);
                naturezaDaOperacao.Initialize();

                PreencherNaturezaDaOperacao(naturezaDaOperacao, unitOfWork);

                repNaturezaDaOperacao.Atualizar(naturezaDaOperacao, Auditado);

                SalvarCFOPs(ref naturezaDaOperacao, unitOfWork);

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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigo);

                if (naturezaDaOperacao == null)
                    return new JsonpResult(false, true, "Não foi possível encontrar o registro.");

                var dynNaturezaDaOperacao = new
                {
                    naturezaDaOperacao.Codigo,
                    naturezaDaOperacao.Descricao,
                    naturezaDaOperacao.Tipo,
                    naturezaDaOperacao.Status,
                    naturezaDaOperacao.DentroEstado,
                    naturezaDaOperacao.GeraTitulo,
                    naturezaDaOperacao.Garantia,
                    naturezaDaOperacao.Demonstracao,
                    naturezaDaOperacao.Bonificacao,
                    naturezaDaOperacao.ControlaEstoque,
                    naturezaDaOperacao.Outras,
                    naturezaDaOperacao.DesconsideraICMSEfetivo,
                    naturezaDaOperacao.Devolucao,
                    CFOP = naturezaDaOperacao.CFOP != null ? new { Codigo = naturezaDaOperacao.CFOP.Codigo, Descricao = !string.IsNullOrWhiteSpace(naturezaDaOperacao.CFOP.Extensao) ? naturezaDaOperacao.CFOP.CodigoCFOP.ToString() + " (" + naturezaDaOperacao.CFOP.Extensao + ") " + naturezaDaOperacao.CFOP.Descricao : naturezaDaOperacao.CFOP.CodigoCFOP.ToString() + " " + naturezaDaOperacao.CFOP.Descricao, } : null,
                    Empresa = naturezaDaOperacao.Empresa != null ? new { Codigo = naturezaDaOperacao.Empresa.Codigo, Descricao = naturezaDaOperacao.Empresa.RazaoSocial } : null,
                    TipoMovimento = naturezaDaOperacao.TipoMovimento != null ? new { Codigo = naturezaDaOperacao.TipoMovimento.Codigo, Descricao = naturezaDaOperacao.TipoMovimento.Descricao } : null,
                    naturezaDaOperacao.Numero,
                    Localidade = naturezaDaOperacao.Localidade != null ? new { Codigo = naturezaDaOperacao.Localidade.Codigo, Descricao = naturezaDaOperacao.Localidade.Descricao } : null,
                    CFOPs = (from obj in naturezaDaOperacao.CFOPs
                             orderby obj.CodigoCFOP
                             select new
                             {
                                 CFOP = new
                                 {
                                     Codigo = obj.Codigo,
                                     CFOP = obj.CodigoCFOP,
                                     Extensao = obj.Extensao,
                                     Descricao = obj.Descricao
                                 }
                             }).ToList(),
                    naturezaDaOperacao.GerarMovimentoAutomaticoEntrada,
                    TipoMovimentoUsoEntrada = new
                    {
                        Descricao = naturezaDaOperacao.TipoMovimentoUsoEntrada?.Descricao ?? string.Empty,
                        Codigo = naturezaDaOperacao.TipoMovimentoUsoEntrada?.Codigo ?? 0
                    },
                    TipoMovimentoReversaoEntrada = new
                    {
                        Descricao = naturezaDaOperacao.TipoMovimentoReversaoEntrada?.Descricao ?? string.Empty,
                        Codigo = naturezaDaOperacao.TipoMovimentoReversaoEntrada?.Codigo ?? 0
                    }
                };

                return new JsonpResult(dynNaturezaDaOperacao);
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
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigo);

                naturezaDaOperacao.CFOPs.Clear();

                repNaturezaDaOperacao.Deletar(naturezaDaOperacao, Auditado);

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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                int codigo = Request.GetIntParam("Codigo");

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = repNaturezaDaOperacao.BuscarPorId(codigo);

                if (naturezaDaOperacao == null)
                    return new JsonpResult(false, true, "Natureza da Operação não encontrada.");

                List<Dominio.Entidades.CFOP> cfops = naturezaDaOperacao.CFOPs.ToList();
                Dominio.Entidades.CFOP cfop = cfops.Count == 1 ? cfops.FirstOrDefault() : null;

                var retorno = new
                {
                    naturezaDaOperacao.Codigo,
                    naturezaDaOperacao.Descricao,
                    QuantidadeCFOPs = cfops.Count,
                    CodigoPrimeiraCFOP = cfop?.Codigo ?? 0,
                    PrimeiraCFOP = cfop?.Descricao ?? string.Empty,
                };

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao obter os detalhes da natureza de operação.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AllowAuthenticate]
        public async Task<IActionResult> AdicionarNaturezaOperacaoSimplificada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                unitOfWork.Start();

                Repositorio.NaturezaDaOperacao repNaturezaDaOperacao = new Repositorio.NaturezaDaOperacao(unitOfWork);
                Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao = new Dominio.Entidades.NaturezaDaOperacao();

                PreencherNaturezaDaOperacaoSimplificada(naturezaDaOperacao, unitOfWork);

                repNaturezaDaOperacao.Inserir(naturezaDaOperacao, Auditado);

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

        #endregion

        #region Métodos Privados

        private void PreencherNaturezaDaOperacao(Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.CFOP repCFOP = new Repositorio.CFOP(unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            int.TryParse(Request.Params("TipoMovimento"), out int codigoTipoMovimento);
            int.TryParse(Request.Params("CFOP"), out int codigocfop);
            int.TryParse(Request.Params("Localidade"), out int codigoLocalidade);
            int.TryParse(Request.Params("Numero"), out int numero);

            string descricao = Request.Params("Descricao");
            string status = Request.Params("Status");

            bool.TryParse(Request.Params("DentroEstado"), out bool dentroEstado);
            bool.TryParse(Request.Params("GeraTitulo"), out bool geraTitulo);
            bool.TryParse(Request.Params("Garantia"), out bool garantia);
            bool.TryParse(Request.Params("Demonstracao"), out bool demonstracao);
            bool.TryParse(Request.Params("Bonificacao"), out bool bonificacao);
            bool.TryParse(Request.Params("Outras"), out bool outras);
            bool.TryParse(Request.Params("ControlaEstoque"), out bool controlaEstoque);
            bool.TryParse(Request.Params("DesconsideraICMSEfetivo"), out bool desconsideraICMSEfetivo);
            bool.TryParse(Request.Params("Devolucao"), out bool devolucao);

            Enum.TryParse(Request.Params("Tipo"), out TipoEntradaSaida tipoEntradaSaida);

            if (naturezaDaOperacao.Codigo == 0)
                naturezaDaOperacao.Empresa = Empresa;
            naturezaDaOperacao.TipoMovimento = codigoTipoMovimento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento) : null;
            naturezaDaOperacao.CFOP = codigocfop > 0 ? repCFOP.BuscarPorCodigo(codigocfop) : null;

            naturezaDaOperacao.Bonificacao = bonificacao;
            naturezaDaOperacao.Demonstracao = demonstracao;
            naturezaDaOperacao.DentroEstado = dentroEstado;
            naturezaDaOperacao.Garantia = garantia;
            naturezaDaOperacao.GeraTitulo = geraTitulo;
            naturezaDaOperacao.Outras = outras;
            naturezaDaOperacao.Descricao = descricao;
            naturezaDaOperacao.Status = status;
            naturezaDaOperacao.ControlaEstoque = controlaEstoque;
            naturezaDaOperacao.Tipo = tipoEntradaSaida;
            naturezaDaOperacao.DesconsideraICMSEfetivo = desconsideraICMSEfetivo;
            naturezaDaOperacao.Devolucao = devolucao;

            SalvarMovimentoAutomaticoEntrada(naturezaDaOperacao, unitOfWork);

            naturezaDaOperacao.Numero = numero;
            naturezaDaOperacao.Localidade = codigoLocalidade > 0 ? repLocalidade.BuscarPorCodigo(codigoLocalidade) : null;
            if (codigoLocalidade > 0)
                naturezaDaOperacao.NaturezaNFSe = AdicionarAtualizarNaturezaNFSe(unitOfWork, naturezaDaOperacao);
        }

        private void SalvarCFOPs(ref Dominio.Entidades.NaturezaDaOperacao naturezaOperacao, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unidadeTrabalho);

            dynamic cfops = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("CFOPs"));

            if (naturezaOperacao.CFOPs == null)
            {
                naturezaOperacao.CFOPs = new List<Dominio.Entidades.CFOP>();
            }
            else
            {
                List<int> codigos = new List<int>();

                foreach (dynamic cfop in cfops)
                    codigos.Add((int)cfop.CFOP.Codigo);

                List<Dominio.Entidades.CFOP> cfopsDeletar = naturezaOperacao.CFOPs.Where(o => !codigos.Contains(o.Codigo)).ToList();

                foreach (Dominio.Entidades.CFOP cfopDeletar in cfopsDeletar)
                    naturezaOperacao.CFOPs.Remove(cfopDeletar);
            }

            foreach (var cfop in cfops)
            {
                int codigoCFOP = (int)cfop.CFOP.Codigo;
                if (!naturezaOperacao.CFOPs.Any(o => o.Codigo == codigoCFOP))
                    naturezaOperacao.CFOPs.Add(new Dominio.Entidades.CFOP() { Codigo = codigoCFOP });
            }
        }

        private void SalvarMovimentoAutomaticoEntrada(Dominio.Entidades.NaturezaDaOperacao naturezaOperacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unidadeDeTrabalho);
            dynamic configuracoes = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("MovimentoAutomaticoEntrada"));
            if (configuracoes != null)
            {
                foreach (var configuracao in configuracoes)
                {
                    naturezaOperacao.GerarMovimentoAutomaticoEntrada = bool.Parse((string)configuracao.GerarMovimentoAutomaticoEntrada);
                    int codigoTipoMovimento = 0, codigoTipoMovimentoReversao = 0;
                    int.TryParse((string)configuracao.TipoMovimentoUsoEntrada, out codigoTipoMovimento);
                    int.TryParse((string)configuracao.TipoMovimentoReversaoEntrada, out codigoTipoMovimentoReversao);
                    if (codigoTipoMovimento > 0)
                        naturezaOperacao.TipoMovimentoUsoEntrada = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimento);
                    else
                        naturezaOperacao.TipoMovimentoUsoEntrada = null;
                    if (codigoTipoMovimentoReversao > 0)
                        naturezaOperacao.TipoMovimentoReversaoEntrada = repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoReversao);
                    else
                        naturezaOperacao.TipoMovimentoReversaoEntrada = null;
                }
            }
        }

        private Dominio.Entidades.NaturezaNFSe AdicionarAtualizarNaturezaNFSe(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao)
        {
            try
            {
                Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unitOfWork);

                Dominio.Entidades.NaturezaNFSe naturezaNFSe = naturezaDaOperacao.NaturezaNFSe;
                if (naturezaNFSe == null)
                    naturezaNFSe = new Dominio.Entidades.NaturezaNFSe();

                naturezaNFSe.Status = "A";
                naturezaNFSe.Descricao = naturezaDaOperacao.Descricao;
                naturezaNFSe.Numero = naturezaDaOperacao.Numero;
                naturezaNFSe.Localidade = naturezaDaOperacao.Localidade;
                if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                    naturezaNFSe.Empresa = this.Usuario.Empresa;

                if (naturezaNFSe.Codigo == 0)
                    repNaturezaNFSe.Inserir(naturezaNFSe);
                else
                    repNaturezaNFSe.Atualizar(naturezaNFSe);

                return naturezaNFSe;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                throw ex;
            }
        }

        private void PreencherNaturezaDaOperacaoSimplificada(Dominio.Entidades.NaturezaDaOperacao naturezaDaOperacao, Repositorio.UnitOfWork unitOfWork)
        {
            naturezaDaOperacao.Descricao = Request.GetStringParam("Descricao");
            naturezaDaOperacao.Tipo = Request.GetEnumParam<TipoEntradaSaida>("Tipo");
            naturezaDaOperacao.Status = Request.GetStringParam("Status");
            naturezaDaOperacao.DentroEstado = Request.GetBoolParam("DentroEstado");
            naturezaDaOperacao.GeraTitulo = Request.GetBoolParam("GeraTitulo");
            naturezaDaOperacao.ControlaEstoque = Request.GetBoolParam("ControlaEstoque");
            naturezaDaOperacao.Garantia = Request.GetBoolParam("Garantia");
            naturezaDaOperacao.Demonstracao = Request.GetBoolParam("Demonstracao");
            naturezaDaOperacao.Bonificacao = Request.GetBoolParam("Bonificacao");
            naturezaDaOperacao.Outras = Request.GetBoolParam("Outras");
            naturezaDaOperacao.DesconsideraICMSEfetivo = Request.GetBoolParam("DesconsideraICMSEfetivo");
            naturezaDaOperacao.Devolucao = Request.GetBoolParam("Devolucao");
        }

        #endregion
    }
}
