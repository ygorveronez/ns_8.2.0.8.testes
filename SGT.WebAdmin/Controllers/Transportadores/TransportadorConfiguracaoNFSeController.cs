using Dominio.ObjetosDeValor.NFSe;
using Microsoft.AspNetCore.Mvc;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize("Transportadores/Transportador")]
    public class TransportadorConfiguracaoNFSeController : BaseController
    {
        #region Construtores

        public TransportadorConfiguracaoNFSeController(Conexao conexao) : base(conexao) { }

        #endregion


        #region Métodos Globais
        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                return new JsonpResult(ObterDadosGrid(unitOfWork));
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoConsultar);
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

                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Repositorio.NaturezaNFSe repNaturezaOperacao = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = new Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe();

                int.TryParse(Request.Params("LocalidadeTomador"), out int localidadeTomador);
                int.TryParse(Request.Params("LocalidadePrestacao"), out int localidadePrestacao);
                int.TryParse(Request.Params("ServicoNFSe"), out int servico);
                int.TryParse(Request.Params("NaturezaNFSe"), out int natureza);
                int.TryParse(Request.Params("Empresa"), out int codEmpresa);
                int.TryParse(Request.Params("SerieNFSe"), out int serie);
                int.TryParse(Request.Params("PrazoCancelamento"), out int prazoCancelamento);
                int.TryParse(Request.Params("GrupoTomador"), out int grupoTomador);
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int TipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

                double.TryParse(Request.Params("ClienteTomador"), out double clienteTomador);

                string ufTomador = Request.Params("UFTomador");

                decimal.TryParse(Request.Params("AliquotaISS"), out decimal aliquotaISS);
                decimal.TryParse(Request.Params("RetencaoISS"), out decimal retencaoISS);

                bool.TryParse(Request.Params("PermiteAnular"), out bool permiteAnular);
                bool.TryParse(Request.Params("ConfiguracaoParaProvisaoDeISS"), out bool configuracaoParaProvisaoDeISS);

                bool.TryParse(Request.Params("ReterIReDestacarNFs"), out bool reterIReDestacarNFs);
                decimal.TryParse(Request.Params("AliquotaIR"), out decimal aliquotaIR);
                decimal.TryParse(Request.Params("BaseCalculoIR"), out decimal baseCalculoIR);

                Dominio.Entidades.NaturezaNFSe naturezaNFSe = repNaturezaOperacao.BuscarPorCodigo(natureza);
                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorCodigo(servico);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codEmpresa);

                if (naturezaNFSe.Localidade.Codigo == empresa.Localidade.Codigo)
                {
                    if (servicoNFSe.Localidade.Codigo == empresa.Localidade.Codigo)
                    {
                        Servicos.Embarcador.ISS.RegrasCalculoISS regrasCalculoImpostos = Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork);
                        IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> queryListaRegras = regrasCalculoImpostos.ObterRegrasISS().AsQueryable();

                        if (repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(empresa.Codigo, localidadePrestacao, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, TipoOcorrencia, queryListaRegras) == null)
                        {
                            transportadorConfiguracaoNFSe.AliquotaISS = aliquotaISS;
                            transportadorConfiguracaoNFSe.Empresa = empresa;
                            transportadorConfiguracaoNFSe.FraseSecreta = Request.Params("FraseSecreta");
                            if (localidadePrestacao > 0)
                                transportadorConfiguracaoNFSe.LocalidadePrestacao = new Dominio.Entidades.Localidade() { Codigo = localidadePrestacao };
                            if (localidadeTomador > 0)
                                transportadorConfiguracaoNFSe.LocalidadeTomador = new Dominio.Entidades.Localidade() { Codigo = localidadeTomador };
                            else
                                transportadorConfiguracaoNFSe.LocalidadeTomador = null;

                            transportadorConfiguracaoNFSe.NBS = Request.GetStringParam("NBS");
                            transportadorConfiguracaoNFSe.LoginSitePrefeitura = Request.Params("LoginSitePrefeitura");
                            transportadorConfiguracaoNFSe.NaturezaNFSe = naturezaNFSe;
                            transportadorConfiguracaoNFSe.ObservacaoIntegracao = Request.Params("ObservacaoIntegracao");
                            transportadorConfiguracaoNFSe.DiscriminacaoNFSe = Request.GetStringParam("DiscriminacaoNFSe");
                            transportadorConfiguracaoNFSe.RetencaoISS = retencaoISS;
                            transportadorConfiguracaoNFSe.SenhaSitePrefeitura = Request.Params("SenhaSitePrefeitura");
                            transportadorConfiguracaoNFSe.SerieNFSe = new Dominio.Entidades.EmpresaSerie() { Codigo = serie };
                            transportadorConfiguracaoNFSe.SerieRPS = Request.Params("SerieRPS");
                            transportadorConfiguracaoNFSe.ServicoNFSe = servicoNFSe;
                            transportadorConfiguracaoNFSe.URLPrefeitura = Request.Params("URLPrefeitura");
                            transportadorConfiguracaoNFSe.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)Request.GetIntParam("ExigibilidadeISS");
                            transportadorConfiguracaoNFSe.IncluirISSBaseCalculo = bool.Parse(Request.Params("IncluirISSBaseCalculo"));
                            transportadorConfiguracaoNFSe.RealizarArredondamentoCalculoIss = bool.Parse(Request.Params("RealizarArredondamentoCalculoIss"));
                            transportadorConfiguracaoNFSe.IncidenciaISSLocalidadePrestador = bool.Parse(Request.Params("IncidenciaISSLocalidadePrestador"));
                            transportadorConfiguracaoNFSe.PermiteAnular = permiteAnular;
                            transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS = configuracaoParaProvisaoDeISS;
                            transportadorConfiguracaoNFSe.PrazoCancelamento = prazoCancelamento;
                            transportadorConfiguracaoNFSe.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
                            transportadorConfiguracaoNFSe.TipoOcorrencia = TipoOcorrencia > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(TipoOcorrencia) : null;
                            transportadorConfiguracaoNFSe.ReterIReDestacarNFs = reterIReDestacarNFs;
                            transportadorConfiguracaoNFSe.AliquotaIR = aliquotaIR;
                            transportadorConfiguracaoNFSe.BaseCalculoIR = baseCalculoIR;
                            transportadorConfiguracaoNFSe.NaoEnviarAliquotaEValorISS = bool.Parse(Request.Params("NaoEnviarAliquotaEValorISS"));

                            if (!string.IsNullOrWhiteSpace(ufTomador))
                                transportadorConfiguracaoNFSe.UFTomador = repEstado.BuscarPorSigla(ufTomador);
                            else
                                transportadorConfiguracaoNFSe.UFTomador = null;
                            if (grupoTomador > 0)
                                transportadorConfiguracaoNFSe.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
                            else
                                transportadorConfiguracaoNFSe.GrupoTomador = null;

                            if (clienteTomador > 0)
                                transportadorConfiguracaoNFSe.ClienteTomador = clienteTomador > 0 ? repCliente.BuscarPorCPFCNPJ(clienteTomador) : null;
                            else
                                transportadorConfiguracaoNFSe.ClienteTomador = null;

                            repTransportadorConfiguracaoNFSe.Inserir(transportadorConfiguracaoNFSe, Auditado);

                            Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork).AtualizarRegrasISS(unitOfWork);

                            Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, null, Localization.Resources.Transportadores.Transportador.AdicionadoConfiguracaoRPS, unitOfWork);

                            unitOfWork.CommitChanges();
                            return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.JaExisteUmaConfiguracaoCadastradaParaEssaLocalidadeDePrestacao);
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.ServicoPrecisaSerDaCidadeDaEmpresa);
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaturezaPrecisaSerDaCidadeDaEmpresa);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAdicionar);
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
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Repositorio.NaturezaNFSe repNaturezaOperacao = new Repositorio.NaturezaNFSe(unitOfWork);
                Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unitOfWork);
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Estado repEstado = new Repositorio.Estado(unitOfWork);
                Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                Repositorio.Embarcador.Pedidos.TipoOperacao repTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(unitOfWork);
                Repositorio.TipoDeOcorrenciaDeCTe repTipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(unitOfWork);
                Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);

                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorCodigo(int.Parse(Request.Params("Codigo")), true);

                int.TryParse(Request.Params("LocalidadePrestacao"), out int localidadePrestacao);
                int.TryParse(Request.Params("LocalidadeTomador"), out int localidadeTomador);
                int.TryParse(Request.Params("ServicoNFSe"), out int servico);
                int.TryParse(Request.Params("NaturezaNFSe"), out int natureza);
                int.TryParse(Request.Params("Empresa"), out int codEmpresa);
                int.TryParse(Request.Params("SerieNFSe"), out int serie);
                int.TryParse(Request.Params("PrazoCancelamento"), out int prazoCancelamento);
                int.TryParse(Request.Params("GrupoTomador"), out int grupoTomador);
                int codigoTipoOperacao = Request.GetIntParam("TipoOperacao");
                int TipoOcorrencia = Request.GetIntParam("TipoOcorrencia");

                double.TryParse(Request.Params("ClienteTomador"), out double clienteTomador);

                string ufTomador = Request.Params("UFTomador");

                decimal.TryParse(Request.Params("AliquotaISS"), out decimal aliquotaISS);
                decimal.TryParse(Request.Params("RetencaoISS"), out decimal retencaoISS);

                bool.TryParse(Request.Params("PermiteAnular"), out bool permiteAnular);
                bool.TryParse(Request.Params("ConfiguracaoParaProvisaoDeISS"), out bool configuracaoParaProvisaoDeISS);

                bool.TryParse(Request.Params("ReterIReDestacarNFs"), out bool reterIReDestacarNFs);
                decimal.TryParse(Request.Params("AliquotaIR"), out decimal aliquotaIR);
                decimal.TryParse(Request.Params("BaseCalculoIR"), out decimal baseCalculoIR);

                Dominio.Entidades.NaturezaNFSe naturezaNFSe = repNaturezaOperacao.BuscarPorCodigo(natureza);
                Dominio.Entidades.ServicoNFSe servicoNFSe = repServicoNFSe.BuscarPorCodigo(servico);
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codEmpresa, true);

                if (naturezaNFSe.Localidade.Codigo == empresa.Localidade.Codigo)
                {
                    if (servicoNFSe.Localidade.Codigo == empresa.Localidade.Codigo)
                    {
                        Servicos.Embarcador.ISS.RegrasCalculoISS regrasCalculoImpostos = Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork);
                        IQueryable<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> queryListaRegras = regrasCalculoImpostos.ObterRegrasISS().AsQueryable();

                        Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSeExiste = repTransportadorConfiguracaoNFSe.BuscarPorEmpresaELocalidade(empresa.Codigo, localidadePrestacao, ufTomador, grupoTomador, localidadeTomador, codigoTipoOperacao, clienteTomador, TipoOcorrencia, queryListaRegras);
                        if (transportadorConfiguracaoNFSeExiste == null || transportadorConfiguracaoNFSeExiste.Codigo == transportadorConfiguracaoNFSe.Codigo)
                        {
                            transportadorConfiguracaoNFSe.AliquotaISS = aliquotaISS;
                            transportadorConfiguracaoNFSe.Empresa = empresa;
                            transportadorConfiguracaoNFSe.FraseSecreta = Request.Params("FraseSecreta");

                            if (localidadePrestacao > 0)
                                transportadorConfiguracaoNFSe.LocalidadePrestacao = new Dominio.Entidades.Localidade() { Codigo = localidadePrestacao };
                            else
                                transportadorConfiguracaoNFSe.LocalidadePrestacao = null;

                            if (localidadeTomador > 0)
                                transportadorConfiguracaoNFSe.LocalidadeTomador = new Dominio.Entidades.Localidade() { Codigo = localidadeTomador };
                            else
                                transportadorConfiguracaoNFSe.LocalidadeTomador = null;

                            transportadorConfiguracaoNFSe.NBS = Request.GetStringParam("NBS");
                            transportadorConfiguracaoNFSe.LoginSitePrefeitura = Request.Params("LoginSitePrefeitura");
                            transportadorConfiguracaoNFSe.NaturezaNFSe = naturezaNFSe;
                            transportadorConfiguracaoNFSe.ObservacaoIntegracao = Request.Params("ObservacaoIntegracao");
                            transportadorConfiguracaoNFSe.DiscriminacaoNFSe = Request.GetStringParam("DiscriminacaoNFSe");
                            transportadorConfiguracaoNFSe.RetencaoISS = retencaoISS;
                            transportadorConfiguracaoNFSe.SenhaSitePrefeitura = Request.Params("SenhaSitePrefeitura");
                            transportadorConfiguracaoNFSe.SerieNFSe = new Dominio.Entidades.EmpresaSerie() { Codigo = serie };
                            transportadorConfiguracaoNFSe.SerieRPS = Request.Params("SerieRPS");
                            transportadorConfiguracaoNFSe.ServicoNFSe = servicoNFSe;
                            transportadorConfiguracaoNFSe.URLPrefeitura = Request.Params("URLPrefeitura");
                            transportadorConfiguracaoNFSe.ExigibilidadeISS = (Dominio.Enumeradores.ExigibilidadeISS)Request.GetIntParam("ExigibilidadeISS");
                            transportadorConfiguracaoNFSe.IncluirISSBaseCalculo = bool.Parse(Request.Params("IncluirISSBaseCalculo"));
                            transportadorConfiguracaoNFSe.RealizarArredondamentoCalculoIss = bool.Parse(Request.Params("RealizarArredondamentoCalculoIss"));
                            transportadorConfiguracaoNFSe.IncidenciaISSLocalidadePrestador = bool.Parse(Request.Params("IncidenciaISSLocalidadePrestador"));
                            transportadorConfiguracaoNFSe.PermiteAnular = permiteAnular;
                            transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS = configuracaoParaProvisaoDeISS;
                            transportadorConfiguracaoNFSe.PrazoCancelamento = prazoCancelamento;
                            transportadorConfiguracaoNFSe.TipoOperacao = codigoTipoOperacao > 0 ? repTipoOperacao.BuscarPorCodigo(codigoTipoOperacao) : null;
                            transportadorConfiguracaoNFSe.TipoOcorrencia = TipoOcorrencia > 0 ? repTipoDeOcorrenciaDeCTe.BuscarPorCodigo(TipoOcorrencia) : null;
                            transportadorConfiguracaoNFSe.ReterIReDestacarNFs = reterIReDestacarNFs;
                            transportadorConfiguracaoNFSe.AliquotaIR = aliquotaIR;
                            transportadorConfiguracaoNFSe.BaseCalculoIR = baseCalculoIR;
                            transportadorConfiguracaoNFSe.NaoEnviarAliquotaEValorISS = bool.Parse(Request.Params("NaoEnviarAliquotaEValorISS"));

                            if (!string.IsNullOrWhiteSpace(ufTomador))
                                transportadorConfiguracaoNFSe.UFTomador = repEstado.BuscarPorSigla(ufTomador);
                            else
                                transportadorConfiguracaoNFSe.UFTomador = null;
                            if (grupoTomador > 0)
                                transportadorConfiguracaoNFSe.GrupoTomador = repGrupoPessoas.BuscarPorCodigo(grupoTomador);
                            else
                                transportadorConfiguracaoNFSe.GrupoTomador = null;

                            if (clienteTomador > 0)
                                transportadorConfiguracaoNFSe.ClienteTomador = repCliente.BuscarPorCPFCNPJ(clienteTomador);
                            else
                                transportadorConfiguracaoNFSe.ClienteTomador = null;

                            repTransportadorConfiguracaoNFSe.Atualizar(transportadorConfiguracaoNFSe, Auditado);
                            empresa.SetExternalChanges(transportadorConfiguracaoNFSe.GetCurrentChanges());
                            Servicos.Auditoria.Auditoria.AuditarComAlteracoesRealizadas(Auditado, empresa, empresa.GetChanges(), Localization.Resources.Transportadores.Transportador.AtualizadoConfiguracaoRPS, unitOfWork);
                            unitOfWork.CommitChanges();
                            Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork).AtualizarRegrasISS(unitOfWork);
                            return new JsonpResult(true);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.JaExisteUmaConfiguracaoCadastradaParaEssaLocalidadeDePrestacao);
                        }
                    }
                    else
                    {
                        unitOfWork.Rollback();
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.ServicoPrecisaSerDaCidadeDaEmpresa);
                    }
                }
                else
                {
                    unitOfWork.Rollback();
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaturezaPrecisaSerDaCidadeDaEmpresa);
                }
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoAtualizar);
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
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorCodigo(codigo);

                var dynTransportadorConfiguracaoNFSe = new
                {
                    transportadorConfiguracaoNFSe.Codigo,
                    AliquotaISS = transportadorConfiguracaoNFSe.AliquotaISS.ToString("n4"),
                    transportadorConfiguracaoNFSe.FraseSecreta,
                    LocalidadePrestacao = transportadorConfiguracaoNFSe.LocalidadePrestacao != null ? new { transportadorConfiguracaoNFSe.LocalidadePrestacao.Codigo, Descricao = transportadorConfiguracaoNFSe.LocalidadePrestacao.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                    LocalidadeTomador = transportadorConfiguracaoNFSe.LocalidadeTomador != null ? new { transportadorConfiguracaoNFSe.LocalidadeTomador.Codigo, Descricao = transportadorConfiguracaoNFSe.LocalidadeTomador.DescricaoCidadeEstado } : new { Codigo = 0, Descricao = "" },
                    transportadorConfiguracaoNFSe.LoginSitePrefeitura,
                    NaturezaNFSe = transportadorConfiguracaoNFSe.NaturezaNFSe != null ? new { transportadorConfiguracaoNFSe.NaturezaNFSe.Codigo, transportadorConfiguracaoNFSe.NaturezaNFSe.Descricao } : new { Codigo = 0, Descricao = "" },
                    transportadorConfiguracaoNFSe.ObservacaoIntegracao,
                    transportadorConfiguracaoNFSe.DiscriminacaoNFSe,
                    transportadorConfiguracaoNFSe.ExigibilidadeISS,
                    transportadorConfiguracaoNFSe.IncluirISSBaseCalculo,
                    transportadorConfiguracaoNFSe.RealizarArredondamentoCalculoIss,
                    transportadorConfiguracaoNFSe.IncidenciaISSLocalidadePrestador,
                    RetencaoISS = transportadorConfiguracaoNFSe.RetencaoISS.ToString("n2"),
                    transportadorConfiguracaoNFSe.SenhaSitePrefeitura,
                    SerieNFSe = transportadorConfiguracaoNFSe.SerieNFSe != null ? new { transportadorConfiguracaoNFSe.SerieNFSe.Codigo, Descricao = transportadorConfiguracaoNFSe.SerieNFSe.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    transportadorConfiguracaoNFSe.SerieRPS,
                    ServicoNFSe = transportadorConfiguracaoNFSe.ServicoNFSe != null ? new { transportadorConfiguracaoNFSe.ServicoNFSe.Codigo, transportadorConfiguracaoNFSe.ServicoNFSe.Descricao } : new { Codigo = 0, Descricao = "" },
                    transportadorConfiguracaoNFSe.URLPrefeitura,
                    transportadorConfiguracaoNFSe.PermiteAnular,
                    transportadorConfiguracaoNFSe.ConfiguracaoParaProvisaoDeISS,
                    transportadorConfiguracaoNFSe.PrazoCancelamento,
                    transportadorConfiguracaoNFSe.NBS,
                    UFTomador = new { Codigo = transportadorConfiguracaoNFSe.UFTomador?.Sigla ?? "", Descricao = transportadorConfiguracaoNFSe.UFTomador?.Descricao ?? "" },
                    GrupoTomador = new { Codigo = transportadorConfiguracaoNFSe.GrupoTomador?.Codigo ?? 0, Descricao = transportadorConfiguracaoNFSe.GrupoTomador?.Descricao ?? "" },
                    TipoOperacao = new { Codigo = transportadorConfiguracaoNFSe.TipoOperacao?.Codigo ?? 0, Descricao = transportadorConfiguracaoNFSe.TipoOperacao?.Descricao ?? string.Empty },
                    TipoOcorrencia = new { Codigo = transportadorConfiguracaoNFSe.TipoOcorrencia?.Codigo ?? 0, Descricao = transportadorConfiguracaoNFSe.TipoOcorrencia?.Descricao ?? string.Empty },
                    ClienteTomador = new { Codigo = transportadorConfiguracaoNFSe.ClienteTomador?.CPF_CNPJ ?? 0, Descricao = transportadorConfiguracaoNFSe.ClienteTomador?.Descricao ?? "" },
                    transportadorConfiguracaoNFSe.ReterIReDestacarNFs,
                    AliquotaIR = transportadorConfiguracaoNFSe.AliquotaIR.ToString("n2"),
                    BaseCalculoIR = transportadorConfiguracaoNFSe.BaseCalculoIR.ToString("n2"),
                    transportadorConfiguracaoNFSe.NaoEnviarAliquotaEValorISS,
                };

                return new JsonpResult(dynTransportadorConfiguracaoNFSe);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
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
                Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
                Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorCodigo(codigo);
                Servicos.Auditoria.Auditoria.Auditar(Auditado, transportadorConfiguracaoNFSe.Empresa, null, Localization.Resources.Transportadores.Transportador.RemovidoConfiguracaoRPS, unitOfWork);
                repTransportadorConfiguracaoNFSe.Deletar(transportadorConfiguracaoNFSe, Auditado);
                Servicos.Embarcador.ISS.RegrasCalculoISS.GetInstance(unitOfWork).AtualizarRegrasISS(unitOfWork);
                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                if (ExcessaoPorPossuirDependeciasNoBanco(ex))
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.NaoFoiPossivelExcluirRegistroPoisMesmoJaPossuiVinculoEmOutrosRecursosDoSistemaRecomendamosQueVoceInativeRegistroCasoNaoDesejaMaisUtilizaLo);
                else
                {
                    Servicos.Log.TratarErro(ex);
                    return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoExcluir);
                }
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public async Task<IActionResult> ValidarLocalidadeSelecionada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                int codigoLocalidade = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.ISS.AliquotaISS repositorioAliquotaISS = new Repositorio.Embarcador.ISS.AliquotaISS(unitOfWork);

                Dominio.Entidades.Embarcador.ISS.AliquotaISS aliquotaISS = null;
                if (repositorioAliquotaISS.ExisteAtivo())
                {
                    aliquotaISS = repositorioAliquotaISS.BuscarPorLocalidadeEAtivo(codigoLocalidade);
                    if (aliquotaISS == null)
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.LocalidadeNaoPossuiAliquotaISSConfigurada);
                }

                var dynLocalidade = new
                {
                    Aliquota = aliquotaISS?.Aliquota.ToString("n4") ?? string.Empty,
                    CodigoLocalidadeNFSe = aliquotaISS?.Localidade?.Codigo ?? 0,
                    RetemISS = aliquotaISS?.RetemISS ?? false,
                    Existe = aliquotaISS != null ? true : false,
                    TipoUnilever = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork).ExistePorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Unilever),
                };

                return new JsonpResult(dynLocalidade);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Transportadores.Transportador.OcorreuUmaFalhaAoBuscarPorCodigo);
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaConfiguracaoNFSe ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaConfiguracaoNFSe()
            {
                Empresa = Request.GetIntParam("Empresa"),
                Servico = Request.GetIntParam("Servico"),
                LocalidadePrestacao = Request.GetIntParam("LocalidadePrestacao"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                NBS = Request.GetStringParam("NBS"),
            };
        }

        private Models.Grid.Grid ObterDadosGrid(Repositorio.UnitOfWork unitOfWork, bool exportar = false)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);

            Dominio.ObjetosDeValor.Embarcador.Transportadores.FiltroPesquisaConfiguracaoNFSe filtrosPesquisa = ObterFiltrosPesquisa();

            Models.Grid.Grid grid = null;
            string propOrdena = "Empresa";
            string dirOrdenacao = "desc";
            int inicioRegistros = 0;
            int maximoRegistros = 50;
            if (exportar)
            {
                grid = new Models.Grid.Grid();
            }
            else
            {
                grid = new Models.Grid.Grid(Request);
                inicioRegistros = grid.inicio;
                maximoRegistros = grid.limite;
                dirOrdenacao = grid.dirOrdena;
                propOrdena = grid.header.Any() ? grid.header[grid.indiceColunaOrdena].data : "Empresa";

                if (propOrdena == "Localidade" ||
                    propOrdena == "TipoOperacao")
                    propOrdena += "Descricao";
            }

            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Serie, "Serie", 15, Models.Grid.Align.center, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Servico, "Servico", 25, Models.Grid.Align.left, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.LocalidadeDaPrestacao, "Localidade", 25, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.TipoOperacao, "TipoOperacao", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.TipoOcorrencia, "TipoOcorrencia", 20, Models.Grid.Align.left, true);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.AliquotaISS, "AliquotaISS", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.UFTomador, "UF", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.SerieRPS, "SerieRPS", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Numero, "NumeroServico", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Codigo, "CodigoServico", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.Natureza, "Natureza", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.RetencaoISS, "RetencaoISS", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho(Localization.Resources.Transportadores.Transportador.IncluirValorISSBaseCalculo, "InclusaoISS", 15, Models.Grid.Align.right, false);
            grid.AdicionarCabecalho("NBS", "NBS", 15, Models.Grid.Align.right, false);




            List<Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe> listaTransportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.Consultar(filtrosPesquisa, propOrdena, dirOrdenacao, inicioRegistros, maximoRegistros);
            grid.setarQuantidadeTotal(repTransportadorConfiguracaoNFSe.ContarConsulta(filtrosPesquisa));
            var lista = (from p in listaTransportadorConfiguracaoNFSe
                         select new
                         {
                             p.Codigo,
                             Serie = p.SerieNFSe.Numero,
                             Servico = p.ServicoNFSe != null ? p.ServicoNFSe.Numero + " - " + p.ServicoNFSe.Descricao : "",
                             Localidade = p.LocalidadePrestacao != null ? p.LocalidadePrestacao.DescricaoCidadeEstado : "",
                             TipoOperacao = p.TipoOperacao != null ? p.TipoOperacao?.Descricao : "",
                             TipoOcorrencia = p.TipoOcorrencia != null ? p.TipoOcorrencia?.Descricao : "",
                             AliquotaISS = p.AliquotaISS != null ? p.AliquotaISS.ToString("n4") : "",
                             UF = p.LocalidadePrestacao != null ? (p.LocalidadePrestacao?.Estado != null ? p.LocalidadePrestacao?.Estado?.Sigla : "") : "",
                             SerieRPS = p.SerieRPS != null ? p.SerieRPS : "",
                             NumeroServico = p.ServicoNFSe != null ? p.ServicoNFSe.Numero : "",
                             CodigoServico = p.ServicoNFSe != null ? p.ServicoNFSe.Codigo : 0,
                             Natureza = p.NaturezaNFSe.Numero != null ? p.NaturezaNFSe.Numero : 0,
                             RetencaoISS = p.RetencaoISS.ToString("n4"),
                             p.NBS,
                             InclusaoISS = p.IncluirISSBaseCalculo ? Localization.Resources.Enumeradores.SimNao.Sim : Localization.Resources.Enumeradores.SimNao.Nao
                         }).ToList();
           
            grid.AdicionaRows(lista);
            return grid;
        }
        #endregion

        #region Exportação

        public async Task<IActionResult> ExportarPlanilhaISS()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var grid = ObterDadosGrid(unitOfWork, true);
                byte[] arquivoBinario = grid.GerarExcel();
                var nomeArquivo = string.IsNullOrEmpty(grid.tituloExportacao) ? "Configuração_NFS" : grid.tituloExportacao;
                if (arquivoBinario != null)
                    return Arquivo(arquivoBinario, "application/octet-stream", $"{nomeArquivo}.{grid.extensaoCSV}");
                return new JsonResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuUmaFalhaAoGerarArquivo);
            }
            finally
            {
                unitOfWork.Dispose();
            }

        }
        #endregion
    }
}

