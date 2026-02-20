using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;
namespace SGT.WebAdmin.Controllers.Fretes
{
    [CustomAuthorize("Fretes/ComponenteFrete")]
    public class ComponenteFreteController : BaseController
    {
        #region Construtores

        public ComponenteFreteController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                string descricao = Request.Params("Descricao");

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete)int.Parse(Request.Params("TipoComponenteFrete"));

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                grid.header = new List<Models.Grid.Head>();
                grid.AdicionarCabecalho("Codigo", false);
                grid.AdicionarCabecalho("TipoComponenteFrete", false);
                grid.AdicionarCabecalho("TipoComponenteFreteDOCCOB", false);
                grid.AdicionarCabecalho("TipoValor", false);
                grid.AdicionarCabecalho("AcrescentaValorTotalAReceber", false);
                grid.AdicionarCabecalho("DescontarValorTotalAReceber", false);
                grid.AdicionarCabecalho("SomarComponenteFreteLiquido", false);
                grid.AdicionarCabecalho("DescontarComponenteFreteLiquido", false);
                grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Descricao, "Descricao", 70, Models.Grid.Align.left, true);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ComponenteFrete.DescontaValor, "DescontaValor", 10, Models.Grid.Align.left, false);
                grid.AdicionarCabecalho(Localization.Resources.Consultas.ComponenteFrete.TipoDoComponente, "DescricaoComponente", 15, Models.Grid.Align.left, false);

                if (ativo == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
                    grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Situacao, "DescricaoAtivo", 8, Models.Grid.Align.center, false);

                List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> listaComponenteFrete = repComponenteFrete.Consultar(descricao, tipoComponenteFrete, ativo, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repComponenteFrete.ContarConsulta(descricao, tipoComponenteFrete, ativo));
                var lista = (from obj in listaComponenteFrete
                             select new
                             {
                                 obj.Codigo,
                                 TipoComponenteFrete = obj.TipoComponenteFrete,
                                 TipoComponenteFreteDOCCOB = obj.TipoComponenteFreteDOCCOB,
                                 obj.Descricao,
                                 obj.DescricaoComponente,
                                 obj.SomarComponenteFreteLiquido,
                                 obj.DescontarComponenteFreteLiquido,
                                 obj.AcrescentaValorTotalAReceber,
                                 obj.NaoSomarValorTotalAReceber,
                                 obj.NaoSomarValorTotalPrestacao,
                                 obj.TipoValor,
                                 obj.DescontarValorTotalAReceber,
                                 DescontaValor = obj.DescontarValorTotalAReceber ? Localization.Resources.Gerais.Geral.Sim : Localization.Resources.Gerais.Geral.Nao,
                                 obj.DescricaoAtivo
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

        [AllowAuthenticate]
        public async Task<IActionResult> ObterTodos()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa ativo = !string.IsNullOrEmpty(Request.Params("Ativo")) ? (Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa)int.Parse(Request.Params("Ativo")) : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo;

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete = (Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete)int.Parse(Request.Params("Tipo"));

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unidadeDeTrabalho);

                List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> listaComponenteFrete = repComponenteFrete.Consultar(null, tipoComponenteFrete, ativo, "Descricao", "asc", 0, 999);

                return new JsonpResult((from obj in listaComponenteFrete select new { value = obj.Codigo, text = obj.Descricao }).ToList());
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public async Task<IActionResult> Adicionar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                bool descontarValorTotalAReceber, acrescentaValorTotalAReceber, somarComponenteFreteLiquido, descontarComponenteFreteLiquido, ativo, imprimirOutraDescricaoCTe, gerarMovimentoAutomatico, imprimirDescricaoComponenteEmComplementos;
                bool.TryParse(Request.Params("DescontarValorTotalAReceber"), out descontarValorTotalAReceber);
                bool.TryParse(Request.Params("AcrescentaValorTotalAReceber"), out acrescentaValorTotalAReceber);
                bool.TryParse(Request.Params("SomarComponenteFreteLiquido"), out somarComponenteFreteLiquido);
                bool.TryParse(Request.Params("DescontarComponenteFreteLiquido"), out descontarComponenteFreteLiquido);
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("ImprimirOutraDescricaoCTe"), out imprimirOutraDescricaoCTe);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);
                bool.TryParse(Request.Params("ImprimirDescricaoComponenteEmComplementos"), out imprimirDescricaoComponenteEmComplementos);
                bool naoSomarValorTotalAReceber = Request.GetBoolParam("NaoSomarValorTotalAReceber");
                bool naoSomarValorTotalPrestacao = Request.GetBoolParam("NaoSomarValorTotalPrestacao");
                bool naoIncluirBaseCalculoImpostos = Request.GetBoolParam("NaoIncluirBaseCalculoImpostos");
                bool naoDeveIncidirSobreNotasFiscaisPateles = Request.GetBoolParam("NaoDeveIncidirSobreNotasFiscaisPateles");
                bool somenteParaCargaPerigosa = Request.GetBoolParam("SomenteParaCargaPerigosa");
                bool descontarComponenteNotaFiscalServico = Request.GetBoolParam("DescontarComponenteNotaFiscalServico");
                bool calcularPISComponente = Request.GetBoolParam("CalcularPISComponente");
                bool calcularICMSComponente = Request.GetBoolParam("CalcularICMSComponente");
                bool enviarComponenteNFTP = Request.GetBoolParam("EnviarComponenteNFTP");
                bool possuiBundle = Request.GetBoolParam("PossuiBundle");

                string codigoEmbarcador = Request.Params("CodigoEmbarcador");
                string descricao = Request.Params("Descricao");
                string descricaoCTe = Request.Params("DescricaoCTe");
                string chargeId = Request.Params("ChargeId");
                string chargeCodeNet = Request.Params("ChargeCodeNet");
                string chargeCodeGross = Request.Params("ChargeCodeGross");


                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete;
                Enum.TryParse(Request.Params("TipoComponenteFrete"), out tipoComponenteFrete);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFreteDOCCOB;
                Enum.TryParse(Request.Params("TipoComponenteFreteDOCCOB"), out tipoComponenteFreteDOCCOB);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor;
                Enum.TryParse(Request.Params("TipoValor"), out tipoValor);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem tipoCampoAcertoViagem;
                Enum.TryParse(Request.Params("TipoCampoAcertoViagem"), out tipoCampoAcertoViagem);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao definicaoDataEnvioIntegracao;
                Enum.TryParse(Request.Params("DefinicaoDataEnvioIntegracao"), out definicaoDataEnvioIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao definicaoDataEnvioIntegracaoEmbarque;
                Enum.TryParse(Request.Params("DefinicaoDataEnvioIntegracaoEmbarque"), out definicaoDataEnvioIntegracaoEmbarque);

                int codigoTipoMovimentoEmissao, codigoTipoMovimentoCancelamento, codigoTipoMovimentoAnulacao, codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador;
                int.TryParse(Request.Params("TipoMovimentoEmissao"), out codigoTipoMovimentoEmissao);
                int.TryParse(Request.Params("TipoMovimentoCancelamento"), out codigoTipoMovimentoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoAnulacao"), out codigoTipoMovimentoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador"), out codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);

                int codigoComponenteFreteBundle;
                int.TryParse(Request.Params("ComponenteFreteBundle"), out codigoComponenteFreteBundle);

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                if (descricao.ToLower().Trim() == "frete valor")
                    return new JsonpResult(false, true, "Não é possível cadastrar um componente com a nomenclatura FRETE VALOR. Este é um componente de uso restrito do sistema.");

                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = new Dominio.Entidades.Embarcador.Frete.ComponenteFrete()
                {
                    Ativo = ativo,
                    CodigoIntegracao = codigoEmbarcador,
                    Descricao = descricao,
                    TipoComponenteFrete = tipoComponenteFrete,
                    TipoComponenteFreteDOCCOB = tipoComponenteFreteDOCCOB,
                    DescontarValorTotalAReceber = descontarValorTotalAReceber,
                    AcrescentaValorTotalAReceber = acrescentaValorTotalAReceber,
                    NaoSomarValorTotalAReceber = naoSomarValorTotalAReceber,
                    NaoSomarValorTotalPrestacao = naoSomarValorTotalPrestacao,
                    NaoIncluirBaseCalculoImpostos = naoIncluirBaseCalculoImpostos,
                    NaoDeveIncidirSobreNotasFiscaisPateles = naoDeveIncidirSobreNotasFiscaisPateles,
                    SomarComponenteFreteLiquido = somarComponenteFreteLiquido,
                    DescontarComponenteFreteLiquido = descontarComponenteFreteLiquido,
                    DescontarComponenteNotaFiscalServico = descontarComponenteNotaFiscalServico,
                    TipoValor = tipoValor,
                    ImprimirOutraDescricaoCTe = imprimirOutraDescricaoCTe,
                    ImprimirDescricaoComponenteEmComplementos = imprimirDescricaoComponenteEmComplementos,
                    SomenteParaCargaPerigosa = somenteParaCargaPerigosa,
                    DescricaoCTe = imprimirOutraDescricaoCTe ? descricaoCTe : string.Empty,
                    GerarMovimentoAutomatico = gerarMovimentoAutomatico,
                    TipoMovimentoAnulacao = codigoTipoMovimentoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacao) : null,
                    TipoMovimentoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador) : null,
                    TipoMovimentoCancelamento = codigoTipoMovimentoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCancelamento) : null,
                    TipoMovimentoEmissao = codigoTipoMovimentoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoEmissao) : null,
                    TipoCampoAcertoViagem = tipoCampoAcertoViagem,
                    ChargeId = chargeId,
                    ChargeCodeNet = chargeCodeNet,
                    ChargeCodeGross = chargeCodeGross,
                    CalcularPISComponente = calcularPISComponente,
                    CalcularICMSComponente = calcularICMSComponente,
                    EnviarComponenteNFTP = enviarComponenteNFTP,
                    DefinicaoDataEnvioIntegracao = definicaoDataEnvioIntegracao,
                    DefinicaoDataEnvioIntegracaoEmbarque = definicaoDataEnvioIntegracaoEmbarque,
                    PossuiBundle = possuiBundle,
                    ComponenteFreteBundle = codigoComponenteFreteBundle > 0 ? repComponenteFrete.BuscarPorCodigo(codigoComponenteFreteBundle) : null
                };

                if (componenteFrete.Ativo && componenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteExiste = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteFrete.TipoComponenteFrete);

                    if (componenteFreteExiste != null)
                        return new JsonpResult(false, true, "Já existe um componente de frete do tipo " + componenteFreteExiste.DescricaoComponente + " cadastrado");

                    componenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                }

                if (gerarMovimentoAutomatico)
                {
                    if (componenteFrete.TipoMovimentoAnulacao == null || componenteFrete.TipoMovimentoCancelamento == null || componenteFrete.TipoMovimentoEmissao == null || componenteFrete.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador == null)
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                }

                if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                    componenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                repComponenteFrete.Inserir(componenteFrete, Auditado);

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
                int codigo, codigoTipoMovimentoEmissao, codigoTipoMovimentoCancelamento, codigoTipoMovimentoAnulacao, codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador;
                int.TryParse(Request.Params("TipoMovimentoEmissao"), out codigoTipoMovimentoEmissao);
                int.TryParse(Request.Params("TipoMovimentoCancelamento"), out codigoTipoMovimentoCancelamento);
                int.TryParse(Request.Params("TipoMovimentoAnulacao"), out codigoTipoMovimentoAnulacao);
                int.TryParse(Request.Params("TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador"), out codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador);
                int.TryParse(Request.Params("Codigo"), out codigo);

                int codigoComponenteFreteBundle;
                int.TryParse(Request.Params("ComponenteFreteBundle"), out codigoComponenteFreteBundle);

                bool descontarValorTotalAReceber, acrescentaValorTotalAReceber, somarComponenteFreteLiquido, descontarComponenteFreteLiquido, ativo, imprimirOutraDescricaoCTe, gerarMovimentoAutomatico, imprimirDescricaoComponenteEmComplementos;
                bool.TryParse(Request.Params("DescontarValorTotalAReceber"), out descontarValorTotalAReceber);
                bool.TryParse(Request.Params("AcrescentaValorTotalAReceber"), out acrescentaValorTotalAReceber);
                bool.TryParse(Request.Params("SomarComponenteFreteLiquido"), out somarComponenteFreteLiquido);
                bool.TryParse(Request.Params("DescontarComponenteFreteLiquido"), out descontarComponenteFreteLiquido);
                bool.TryParse(Request.Params("Ativo"), out ativo);
                bool.TryParse(Request.Params("ImprimirOutraDescricaoCTe"), out imprimirOutraDescricaoCTe);
                bool.TryParse(Request.Params("ImprimirDescricaoComponenteEmComplementos"), out imprimirDescricaoComponenteEmComplementos);
                bool.TryParse(Request.Params("GerarMovimentoAutomatico"), out gerarMovimentoAutomatico);
                bool naoSomarValorTotalAReceber = Request.GetBoolParam("NaoSomarValorTotalAReceber");
                bool naoSomarValorTotaPrestacao = Request.GetBoolParam("NaoSomarValorTotalPrestacao");
                bool naoIncluirBaseCalculoImpostos = Request.GetBoolParam("NaoIncluirBaseCalculoImpostos");
                bool naoDeveIncidirSobreNotasFiscaisPateles = Request.GetBoolParam("NaoDeveIncidirSobreNotasFiscaisPateles");
                bool somenteParaCargaPerigosa = Request.GetBoolParam("SomenteParaCargaPerigosa");
                bool descontarComponenteNotaFiscalServico = Request.GetBoolParam("DescontarComponenteNotaFiscalServico");
                bool calcularPISComponente = Request.GetBoolParam("CalcularPISComponente");
                bool calcularICMSComponente = Request.GetBoolParam("CalcularICMSComponente");
                bool enviarComponenteNFTP = Request.GetBoolParam("EnviarComponenteNFTP");
                bool possuiBundle = Request.GetBoolParam("PossuiBundle");

                string codigoEmbarcador = Request.Params("CodigoEmbarcador");
                string descricao = Request.Params("Descricao");
                string descricaoCTe = Request.Params("DescricaoCTe");
                string chargeId = Request.Params("ChargeId");
                string chargeCodeNet = Request.Params("ChargeCodeNet");
                string chargeCodeGross = Request.Params("ChargeCodeGross");

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFrete;
                Enum.TryParse(Request.Params("TipoComponenteFrete"), out tipoComponenteFrete);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete tipoComponenteFreteDOCCOB;
                Enum.TryParse(Request.Params("TipoComponenteFreteDOCCOB"), out tipoComponenteFreteDOCCOB);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete tipoValor;
                Enum.TryParse(Request.Params("TipoValor"), out tipoValor);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoAcertoViagem tipoCampoAcertoViagem;
                Enum.TryParse(Request.Params("TipoCampoAcertoViagem"), out tipoCampoAcertoViagem);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao definicaoDataEnvioIntegracao;
                Enum.TryParse(Request.Params("DefinicaoDataEnvioIntegracao"), out definicaoDataEnvioIntegracao);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.DefinicaoDataEnvioIntegracao definicaoDataEnvioIntegracaoEmbarque;
                Enum.TryParse(Request.Params("DefinicaoDataEnvioIntegracaoEmbarque"), out definicaoDataEnvioIntegracaoEmbarque);

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);
                Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(unitOfWork);

                if (descricao.ToLower().Trim() == "frete valor")
                    return new JsonpResult(false, true, "Não é possível cadastrar um componente com a nomenclatura FRETE VALOR. Este é um componente de uso restrito do sistema.");

                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigo, true);

                componenteFrete.Ativo = ativo;
                componenteFrete.CodigoIntegracao = codigoEmbarcador;
                componenteFrete.Descricao = descricao;
                componenteFrete.TipoComponenteFrete = tipoComponenteFrete;
                componenteFrete.TipoComponenteFreteDOCCOB = tipoComponenteFreteDOCCOB;
                componenteFrete.DescontarValorTotalAReceber = descontarValorTotalAReceber;
                componenteFrete.AcrescentaValorTotalAReceber = acrescentaValorTotalAReceber;
                componenteFrete.NaoSomarValorTotalAReceber = naoSomarValorTotalAReceber;
                componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles = naoDeveIncidirSobreNotasFiscaisPateles;
                componenteFrete.NaoSomarValorTotalPrestacao = naoSomarValorTotaPrestacao;
                componenteFrete.NaoIncluirBaseCalculoImpostos = naoIncluirBaseCalculoImpostos;
                componenteFrete.SomarComponenteFreteLiquido = somarComponenteFreteLiquido;
                componenteFrete.DescontarComponenteFreteLiquido = descontarComponenteFreteLiquido;
                componenteFrete.DescontarComponenteNotaFiscalServico = descontarComponenteNotaFiscalServico;
                componenteFrete.TipoValor = tipoValor;
                componenteFrete.ImprimirOutraDescricaoCTe = imprimirOutraDescricaoCTe;
                componenteFrete.ImprimirDescricaoComponenteEmComplementos = imprimirDescricaoComponenteEmComplementos;
                componenteFrete.SomenteParaCargaPerigosa = somenteParaCargaPerigosa;
                componenteFrete.DescricaoCTe = imprimirOutraDescricaoCTe ? descricaoCTe : string.Empty;
                componenteFrete.GerarMovimentoAutomatico = gerarMovimentoAutomatico;
                componenteFrete.TipoMovimentoAnulacao = codigoTipoMovimentoAnulacao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacao) : null;
                componenteFrete.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador = codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoAnulacaoComNotaAnulacaoEmbarcador) : null;
                componenteFrete.TipoMovimentoCancelamento = codigoTipoMovimentoCancelamento > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoCancelamento) : null;
                componenteFrete.TipoMovimentoEmissao = codigoTipoMovimentoEmissao > 0 ? repTipoMovimento.BuscarPorCodigo(codigoTipoMovimentoEmissao) : null;
                componenteFrete.TipoCampoAcertoViagem = tipoCampoAcertoViagem;
                componenteFrete.ChargeId = chargeId;
                componenteFrete.ChargeCodeNet = chargeCodeNet;
                componenteFrete.ChargeCodeGross = chargeCodeGross;
                componenteFrete.CalcularPISComponente = calcularPISComponente;
                componenteFrete.CalcularICMSComponente = calcularICMSComponente;
                componenteFrete.EnviarComponenteNFTP = enviarComponenteNFTP;
                componenteFrete.DefinicaoDataEnvioIntegracao = definicaoDataEnvioIntegracao;
                componenteFrete.DefinicaoDataEnvioIntegracaoEmbarque = definicaoDataEnvioIntegracaoEmbarque;
                componenteFrete.PossuiBundle = possuiBundle;
                componenteFrete.ComponenteFreteBundle = codigoComponenteFreteBundle > 0 ? repComponenteFrete.BuscarPorCodigo(codigoComponenteFreteBundle) : null;

                if (componenteFrete.Ativo && componenteFrete.TipoComponenteFrete != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.OUTROS)
                {
                    Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFreteExiste = repComponenteFrete.BuscarPorTipoComponenteFrete(componenteFrete.TipoComponenteFrete);

                    if (componenteFreteExiste != null && componenteFreteExiste.Codigo != componenteFrete.Codigo)
                        return new JsonpResult(false, true, "Já existe um componente de frete do tipo " + componenteFreteExiste.DescricaoComponente + " cadastrado");

                    componenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.AumentoValor;
                }

                if (gerarMovimentoAutomatico)
                {
                    if (componenteFrete.TipoMovimentoAnulacao == null || componenteFrete.TipoMovimentoCancelamento == null || componenteFrete.TipoMovimentoEmissao == null || componenteFrete.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador == null)
                        return new JsonpResult(false, true, "É necessário selecionar os tipos de movimento para gerar os movimentos automaticamente.");
                }

                if (componenteFrete.TipoComponenteFrete == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoComponenteFrete.ADVALOREM)
                    componenteFrete.TipoValor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete.PercentualSobreValorNotaFiscal;

                repComponenteFrete.Atualizar(componenteFrete, Auditado);

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
                int codigo;
                int.TryParse(Request.Params("Codigo"), out codigo);

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigo);

                return new JsonpResult(new
                {
                    componenteFrete.Descricao,
                    componenteFrete.Codigo,
                    componenteFrete.TipoValor,
                    componenteFrete.DescontarValorTotalAReceber,
                    componenteFrete.CodigoIntegracao,
                    CodigoEmbarcador = componenteFrete.CodigoIntegracao,
                    componenteFrete.SomarComponenteFreteLiquido,
                    componenteFrete.DescontarComponenteFreteLiquido,
                    componenteFrete.DescontarComponenteNotaFiscalServico,
                    componenteFrete.AcrescentaValorTotalAReceber,
                    componenteFrete.NaoSomarValorTotalAReceber,
                    componenteFrete.NaoDeveIncidirSobreNotasFiscaisPateles,
                    componenteFrete.NaoSomarValorTotalPrestacao,
                    componenteFrete.NaoIncluirBaseCalculoImpostos,
                    componenteFrete.TipoComponenteFrete,
                    componenteFrete.TipoComponenteFreteDOCCOB,
                    componenteFrete.Ativo,
                    componenteFrete.ImprimirOutraDescricaoCTe,
                    componenteFrete.ImprimirDescricaoComponenteEmComplementos,
                    componenteFrete.SomenteParaCargaPerigosa,
                    componenteFrete.DescricaoCTe,
                    componenteFrete.GerarMovimentoAutomatico,
                    TipoMovimentoAnulacao = new
                    {
                        Descricao = componenteFrete.TipoMovimentoAnulacao?.Descricao ?? string.Empty,
                        Codigo = componenteFrete.TipoMovimentoAnulacao?.Codigo ?? 0
                    },
                    TipoMovimentoAnulacaoComNotaAnulacaoEmbarcador = new
                    {
                        Descricao = componenteFrete.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador?.Descricao ?? string.Empty,
                        Codigo = componenteFrete.TipoMovimentoAnulacaoNotaAnulacaoEmbarcador?.Codigo ?? 0
                    },
                    TipoMovimentoCancelamento = new
                    {
                        Descricao = componenteFrete.TipoMovimentoCancelamento?.Descricao ?? string.Empty,
                        Codigo = componenteFrete.TipoMovimentoCancelamento?.Codigo ?? 0
                    },
                    TipoMovimentoEmissao = new
                    {
                        Descricao = componenteFrete.TipoMovimentoEmissao?.Descricao ?? string.Empty,
                        Codigo = componenteFrete.TipoMovimentoEmissao?.Codigo ?? 0
                    },
                    componenteFrete.TipoCampoAcertoViagem,
                    componenteFrete.ChargeId,
                    componenteFrete.ChargeCodeNet,
                    componenteFrete.ChargeCodeGross,
                    componenteFrete.CalcularPISComponente,
                    componenteFrete.CalcularICMSComponente,
                    componenteFrete.EnviarComponenteNFTP,
                    componenteFrete.DefinicaoDataEnvioIntegracao,
                    componenteFrete.DefinicaoDataEnvioIntegracaoEmbarque,
                    componenteFrete.PossuiBundle,
                    ComponenteFreteBundle = new {
                        Descricao = componenteFrete.ComponenteFreteBundle?.Descricao ?? string.Empty,
                        Codigo = componenteFrete.ComponenteFreteBundle?.Codigo ?? 0  
                    }
            });
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
                int codigo;
                int.TryParse(Request.Params("codigo"), out codigo);

                Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork);

                Dominio.Entidades.Embarcador.Frete.ComponenteFrete componenteFrete = repComponenteFrete.BuscarPorCodigo(codigo, true);

                repComponenteFrete.Deletar(componenteFrete, Auditado);

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
