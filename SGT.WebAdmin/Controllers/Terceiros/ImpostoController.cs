using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Terceiros
{
    [CustomAuthorize("Terceiros/Imposto")]
    public class ImpostoController : BaseController
    {
        #region Construtores

        public ImpostoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                double cpfCnpjTerceiro = Request.GetDoubleParam("Terceiro");

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();

                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("Terceiro", "Terceiro", 90, Models.Grid.Align.left, true);

                Repositorio.ImpostoContratoFrete repImposto = new Repositorio.ImpostoContratoFrete(unitOfWork);

                List<Dominio.Entidades.ImpostoContratoFrete> impostos = repImposto.Consultar(cpfCnpjTerceiro, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);

                grid.setarQuantidadeTotal(repImposto.ContarConsulta(cpfCnpjTerceiro));

                var lista = (from p in impostos
                             select new
                             {
                                 p.Codigo,
                                 Terceiro = p.Terceiro?.Descricao ?? string.Empty
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
                decimal aliquotaCOFINS = Request.GetDecimalParam("AliquotaCOFINS");
                decimal aliquotaPIS = Request.GetDecimalParam("AliquotaPIS");
                decimal aliquotaSENAT = Request.GetDecimalParam("AliquotaSENAT");
                decimal aliquotaSEST = Request.GetDecimalParam("AliquotaSEST");
                decimal percentualBCINSS = Request.GetDecimalParam("PercentualBCINSS");
                decimal percentualBCIR = Request.GetDecimalParam("PercentualBCIR");
                decimal valorPorDependenteDescontoIRRF = Request.GetDecimalParam("ValorPorDependenteDescontoIRRF");
                decimal valorTetoRetencaoINSS = Request.GetDecimalParam("ValorTetoRetencaoINSS");
                decimal aliquotaINSSPatronal = Request.GetDecimalParam("AliquotaINSSPatronal");
                bool utilizarBaseCalculoAcumulada = Request.GetBoolParam("UtilizarBaseCalculoAcumulada");
                bool utilizarCalculoIrSobreFaixaValorTotal = Request.GetBoolParam("UtilizarCalculoIrSobreFaixaValorTotal");
                bool calcularPorRaizCNPJ = Request.GetBoolParam("CalcularPorRaizCNPJ");

                double cpfCnpjTerceiro = Request.GetDoubleParam("Terceiro");
                int codigoTipoTerceiro = Request.GetIntParam("TipoTerceiro");
                string codigoIntegracaoTributaria = Request.GetStringParam("CodigoIntegracaoTributaria");
                RegimeTributario? regimeTributario = Request.GetNullableEnumParam<RegimeTributario>("RegimeTributario");
                var tipoPessoa = Request.GetIntParam("TipoPessoa", valorPadrao: 99);
                string tipo = "";
                if (tipoPessoa == 0)
                    tipo = "F";
                else if (tipoPessoa == 1)
                    tipo = "J";
                else if (tipoPessoa == 2)
                    tipo = "E";
                else
                    tipo = "T";

                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

                if (repImpostoContratoFrete.BuscarPorTerceiro(cpfCnpjTerceiro, codigoTipoTerceiro, regimeTributario, tipo) != null)
                    return new JsonpResult(false, true, "Já existe uma tabela de imposto com esta configuração.");

                unitOfWork.Start();

                Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = new Dominio.Entidades.ImpostoContratoFrete()
                {
                    AliquotaSENAT = aliquotaSENAT,
                    AliquotaSEST = aliquotaSEST,
                    PercentualBCINSS = percentualBCINSS,
                    PercentualBCIR = percentualBCIR,
                    ValorPorDependenteDescontoIRRF = valorPorDependenteDescontoIRRF,
                    Terceiro = cpfCnpjTerceiro > 0D ? repCliente.BuscarPorCPFCNPJ(cpfCnpjTerceiro) : null,
                    ValorTetoRetencaoINSS = valorTetoRetencaoINSS,
                    AliquotaINSSPatronal = aliquotaINSSPatronal,
                    UtilizarBaseCalculoAcumulada = utilizarBaseCalculoAcumulada,
                    UtilizarCalculoIrSobreFaixaValorTotal = utilizarCalculoIrSobreFaixaValorTotal,
                    CalcularPorRaizCNPJ = calcularPorRaizCNPJ,
                    AliquotaCOFINS = aliquotaCOFINS,
                    AliquotaPIS = aliquotaPIS,
                    CodigoIntegracaoTributaria = codigoIntegracaoTributaria,
                    TipoTerceiro = codigoTipoTerceiro > 0 ? repTipoTerceiro.BuscarPorCodigo(codigoTipoTerceiro) : null
                };

                impostoContratoFrete.RegimeTributario = regimeTributario;
                impostoContratoFrete.TipoPessoa = tipo;

                repImpostoContratoFrete.Inserir(impostoContratoFrete, Auditado);

                SalvarINSS(impostoContratoFrete, unitOfWork);
                SalvarIRRF(impostoContratoFrete, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
                int codigo = Request.GetIntParam("Codigo");

                decimal aliquotaSENAT = Request.GetDecimalParam("AliquotaSENAT");
                decimal aliquotaSEST = Request.GetDecimalParam("AliquotaSEST");
                decimal percentualBCINSS = Request.GetDecimalParam("PercentualBCINSS");
                decimal percentualBCIR = Request.GetDecimalParam("PercentualBCIR");
                decimal valorPorDependenteDescontoIRRF = Request.GetDecimalParam("ValorPorDependenteDescontoIRRF");
                decimal valorTetoRetencaoINSS = Request.GetDecimalParam("ValorTetoRetencaoINSS");
                decimal aliquotaINSSPatronal = Request.GetDecimalParam("AliquotaINSSPatronal");
                bool utilizarBaseCalculoAcumulada = Request.GetBoolParam("UtilizarBaseCalculoAcumulada");
                bool utilizarCalculoIrSobreFaixaValorTotal = Request.GetBoolParam("UtilizarCalculoIrSobreFaixaValorTotal");
                bool calcularPorRaizCNPJ = Request.GetBoolParam("CalcularPorRaizCNPJ");
                int codigoTipoTerceiro = Request.GetIntParam("TipoTerceiro");

                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);
                Repositorio.Embarcador.Pessoas.TipoTerceiro repTipoTerceiro = new Repositorio.Embarcador.Pessoas.TipoTerceiro(unitOfWork);

                unitOfWork.Start();

                Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorCodigo(codigo, true);

                impostoContratoFrete.AliquotaSENAT = aliquotaSENAT;
                impostoContratoFrete.AliquotaSEST = aliquotaSEST;
                impostoContratoFrete.PercentualBCINSS = percentualBCINSS;
                impostoContratoFrete.PercentualBCIR = percentualBCIR;
                impostoContratoFrete.ValorPorDependenteDescontoIRRF = valorPorDependenteDescontoIRRF;
                impostoContratoFrete.ValorTetoRetencaoINSS = valorTetoRetencaoINSS;
                impostoContratoFrete.AliquotaINSSPatronal = aliquotaINSSPatronal;
                impostoContratoFrete.UtilizarBaseCalculoAcumulada = utilizarBaseCalculoAcumulada;
                impostoContratoFrete.UtilizarCalculoIrSobreFaixaValorTotal = utilizarCalculoIrSobreFaixaValorTotal;
                impostoContratoFrete.CalcularPorRaizCNPJ = calcularPorRaizCNPJ;
                impostoContratoFrete.AliquotaPIS = Request.GetDecimalParam("AliquotaPIS");
                impostoContratoFrete.AliquotaCOFINS = Request.GetDecimalParam("AliquotaCOFINS");
                impostoContratoFrete.CodigoIntegracaoTributaria = Request.GetStringParam("CodigoIntegracaoTributaria");
                impostoContratoFrete.TipoTerceiro = codigoTipoTerceiro > 0 ? repTipoTerceiro.BuscarPorCodigo(codigoTipoTerceiro) : null;

                RegimeTributario? regimeTributario = Request.GetNullableEnumParam<RegimeTributario>("RegimeTributario");
                impostoContratoFrete.RegimeTributario = regimeTributario;
                var tipoPessoa = Request.GetIntParam("TipoPessoa", valorPadrao: 99);
                if (tipoPessoa == 0)
                    impostoContratoFrete.TipoPessoa = "F";
                else if (tipoPessoa == 1)
                    impostoContratoFrete.TipoPessoa = "J";
                else if (tipoPessoa == 2)
                    impostoContratoFrete.TipoPessoa = "E";
                else
                    impostoContratoFrete.TipoPessoa = "T";

                repImpostoContratoFrete.Atualizar(impostoContratoFrete, Auditado);

                SalvarINSS(impostoContratoFrete, unitOfWork);
                SalvarIRRF(impostoContratoFrete, unitOfWork);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
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
                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorCodigo(codigo, false);

                int tipoPessoa = 0;
                if (string.IsNullOrWhiteSpace(impostoContratoFrete.TipoPessoa) || impostoContratoFrete.TipoPessoa.Equals("J"))
                    tipoPessoa = 1;
                else if (impostoContratoFrete.TipoPessoa.Equals("F"))
                    tipoPessoa = 0;
                else if (impostoContratoFrete.TipoPessoa.Equals("J"))
                    tipoPessoa = 2;
                else
                    tipoPessoa = 99;

                var retorno = new
                {
                    impostoContratoFrete.Codigo,
                    impostoContratoFrete.AliquotaSENAT,
                    impostoContratoFrete.AliquotaSEST,
                    AliquotaCOFINS = impostoContratoFrete.AliquotaCOFINS.ToString("n4"),
                    AliquotaPIS = impostoContratoFrete.AliquotaPIS.ToString("n4"),
                    impostoContratoFrete.CodigoIntegracaoTributaria,
                    TipoPessoa = tipoPessoa,
                    impostoContratoFrete.RegimeTributario,
                    Empresa = new
                    {
                        Codigo = impostoContratoFrete.Empresa?.Codigo ?? 0,
                        Descricao = impostoContratoFrete.Empresa?.Descricao ?? string.Empty
                    },
                    impostoContratoFrete.PercentualBCINSS,
                    impostoContratoFrete.AliquotaINSSPatronal,
                    impostoContratoFrete.PercentualBCIR,
                    impostoContratoFrete.ValorPorDependenteDescontoIRRF,
                    impostoContratoFrete.UtilizarBaseCalculoAcumulada,
                    impostoContratoFrete.UtilizarCalculoIrSobreFaixaValorTotal,
                    impostoContratoFrete.CalcularPorRaizCNPJ,
                    Terceiro = new
                    {
                        Codigo = impostoContratoFrete.Terceiro?.CPF_CNPJ_SemFormato,
                        Descricao = impostoContratoFrete.Terceiro?.Descricao
                    },
                    TipoTerceiro = new
                    {
                        Codigo = impostoContratoFrete.TipoTerceiro?.Codigo ?? 0,
                        Descricao = impostoContratoFrete.TipoTerceiro?.Descricao ?? ""
                    },
                    impostoContratoFrete.ValorTetoRetencaoINSS,
                    ListaINSS = impostoContratoFrete.INSS.Select(o => new
                    {
                        o.Codigo,
                        PercentualAplicar = new { val = o.PercentualAplicar, tipo = "decimal" },
                        ValorInicial = new { val = o.ValorInicial, tipo = "decimal" },
                        ValorFinal = new { val = o.ValorFinal, tipo = "decimal" }
                    }).ToList(),
                    ListaIRRF = impostoContratoFrete.IRRF.Select(o => new
                    {
                        o.Codigo,
                        PercentualAplicar = new { val = o.PercentualAplicar, tipo = "decimal" },
                        ValorInicial = new { val = o.ValorInicial, tipo = "decimal" },
                        ValorFinal = new { val = o.ValorFinal, tipo = "decimal" },
                        ValorDeduzir = new { val = o.ValorDeduzir, tipo = "decimal" }
                    }).ToList()
                };

                return new JsonpResult(retorno);
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
                Repositorio.ImpostoContratoFrete repImpostoContratoFrete = new Repositorio.ImpostoContratoFrete(unitOfWork);

                int codigo = Request.GetIntParam("Codigo");

                Dominio.Entidades.ImpostoContratoFrete impostoContratoFrete = repImpostoContratoFrete.BuscarPorCodigo(codigo, true);

                impostoContratoFrete.INSS = null;
                impostoContratoFrete.IRRF = null;

                repImpostoContratoFrete.Deletar(impostoContratoFrete, Auditado);

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

        private void SalvarINSS(Dominio.Entidades.ImpostoContratoFrete imposto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.INSSImpostoContratoFrete repINSS = new Repositorio.INSSImpostoContratoFrete(unitOfWork);

            dynamic listaINSS = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaINSS"));

            if (imposto.INSS != null && imposto.INSS.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var inss in listaINSS)
                {
                    if (int.TryParse((string)inss.Codigo, out int codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.INSSImpostoContratoFrete> inssDeletar = (from obj in imposto.INSS where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < inssDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, null, "Removeu a configuração do INSS de " + inssDeletar[i].DescricaoFaixa + ".", unitOfWork);

                    repINSS.Deletar(inssDeletar[i], Auditado);
                }
            }

            foreach (var inss in listaINSS)
            {
                Dominio.Entidades.INSSImpostoContratoFrete i = null;

                int codigo = 0;

                if (inss.Codigo != null && int.TryParse((string)inss.Codigo, out codigo))
                    i = repINSS.BuscarPorCodigo(codigo);

                if (i == null)
                    i = new Dominio.Entidades.INSSImpostoContratoFrete();
                else
                    i.Initialize();

                i.Imposto = imposto;
                i.ValorInicial = (decimal)inss.ValorInicial;
                i.ValorFinal = (decimal)inss.ValorFinal;
                i.PercentualAplicar = (decimal)inss.PercentualAplicar;

                if (i.Codigo > 0)
                {
                    repINSS.Atualizar(i, Auditado);

                    var alteracoes = i.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, alteracoes, "Alterou a faixa de INSS de " + i.DescricaoFaixa + ".", unitOfWork);
                }
                else
                {
                    repINSS.Inserir(i, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, null, "Adicionou a faixa de INSS de " + i.DescricaoFaixa + ".", unitOfWork);
                }
            }
        }

        private void SalvarIRRF(Dominio.Entidades.ImpostoContratoFrete imposto, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.IRImpostoContratoFrete repIRRF = new Repositorio.IRImpostoContratoFrete(unitOfWork);

            dynamic listaIRRF = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Request.Params("ListaIRRF"));

            if (imposto.IRRF != null && imposto.IRRF.Count > 0)
            {
                List<int> codigos = new List<int>();

                foreach (var irrf in listaIRRF)
                {
                    if (int.TryParse((string)irrf.Codigo, out int codigo))
                        codigos.Add(codigo);
                }

                List<Dominio.Entidades.IRImpostoContratoFrete> irrfDeletar = (from obj in imposto.IRRF where !codigos.Contains(obj.Codigo) select obj).ToList();

                for (var i = 0; i < irrfDeletar.Count; i++)
                {
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, null, "Removeu a configuração do IRRF de " + irrfDeletar[i].DescricaoFaixa + ".", unitOfWork);

                    repIRRF.Deletar(irrfDeletar[i], Auditado);
                }
            }

            foreach (var irrf in listaIRRF)
            {
                Dominio.Entidades.IRImpostoContratoFrete i = null;

                int codigo = 0;

                if (irrf.Codigo != null && int.TryParse((string)irrf.Codigo, out codigo))
                    i = repIRRF.BuscarPorCodigo(codigo);

                if (i == null)
                    i = new Dominio.Entidades.IRImpostoContratoFrete();
                else
                    i.Initialize();

                i.Imposto = imposto;
                i.ValorInicial = (decimal)irrf.ValorInicial;
                i.ValorFinal = (decimal)irrf.ValorFinal;
                i.PercentualAplicar = (decimal)irrf.PercentualAplicar;
                i.ValorDeduzir = (decimal)irrf.ValorDeduzir;

                if (i.Codigo > 0)
                {
                    repIRRF.Atualizar(i, Auditado);

                    var alteracoes = i.GetChanges();

                    if (alteracoes.Count > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, alteracoes, "Alterou a faixa de IRRF de " + i.DescricaoFaixa + ".", unitOfWork);
                }
                else
                {
                    repIRRF.Inserir(i, Auditado);
                    Servicos.Auditoria.Auditoria.Auditar(Auditado, imposto, null, "Adicionou a faixa de IRRF de " + i.DescricaoFaixa + ".", unitOfWork);
                }
            }
        }

        #endregion
    }
}
