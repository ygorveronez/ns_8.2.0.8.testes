using Dominio.Excecoes.Embarcador;
using Servicos;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Controllers.Transportadores
{
    [CustomAuthorize(new string[] { "BuscarPorEmpresaLogada" }, "Transportadores/GestaoTransportador", "Transportadores/Transportador")]
    public class GestaoTransportadorController : BaseController
    {
		#region Construtores

		public GestaoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Métodos Globais

        public async Task<IActionResult> Atualizar()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador repConfiguracaoTransportador = new Repositorio.Embarcador.Configuracoes.ConfiguracaoTransportador(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoTransportador configuracaoTransportador = repConfiguracaoTransportador.BuscarConfiguracaoPadrao();
                List<AdminMultisoftware.Dominio.Enumeradores.PermissaoPersonalizada> permissoesPersonalizadas = ObterPermissoesPersonalizadas("Transportadores/Transportador");

                unitOfWork.Start();

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Transportadores.TransportadorFormulario repTransportadorFormulario = new Repositorio.Embarcador.Transportadores.TransportadorFormulario(unitOfWork);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(Request.GetIntParam("Codigo"));

                //PreencherTransportador(empresa, unitOfWork, permissoesPersonalizadas);

                empresa.DataAtualizacao = DateTime.Now;
                empresa.UsuarioAtualizacao = Auditado.Usuario;

                empresa.Email = Request.Params("Email").ToLower();
                empresa.EmailAdministrativo = Request.Params("EmailAdministrativo").ToLower();
                empresa.EmailContador = Request.Params("EmailContador").ToLower();

                bool enviarEmail = Request.GetBoolParam("EnviarEmail");
                bool enviarEmailAdministrativo = Request.GetBoolParam("EnviarEmailAdministrativo");
                bool enviarEmailContador = Request.GetBoolParam("EnviarEmailContador");
                empresa.OptanteSimplesNacionalComExcessoReceitaBruta = Request.GetBoolParam("OptanteSimplesNacionalComExcessoReceitaBruta");
                empresa.StatusEmail = enviarEmail ? "A" : "I";
                empresa.StatusEmailAdministrativo = enviarEmailAdministrativo ? "A" : "I";
                empresa.StatusEmailContador = enviarEmailContador ? "A" : "I";
                empresa.StatusEmissao = "S";

                empresa.Integrado = false;

                if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe && TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFeAdmin)
                    if (empresa.EmpresaPai != null && empresa.TipoAmbiente != empresa.EmpresaPai.TipoAmbiente)
                        return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.TipoDeAmbienteInformadoDiferenteDoAmbienteGeral);

                if (string.IsNullOrWhiteSpace(empresa.CEP) && !configuracaoTransportador.PermitirCadastrarTransportadorInformacoesMinimas)
                    return new JsonpResult(false, true, Localization.Resources.Transportadores.Transportador.FavorInformeCEPDaEmpresa);

                Dominio.Entidades.Empresa empresaExiste = empresa.Tipo == "E" ? repEmpresa.BuscarPorCodigoIntegracao(empresa.CodigoIntegracao) : repEmpresa.BuscarPorCNPJ(empresa.CNPJ);

                repEmpresa.Atualizar(empresa, Auditado);

                unitOfWork.CommitChanges();

                return new JsonpResult(true);
            }
            catch (ControllerException excecao)
            {
                unitOfWork.Rollback();
                return new JsonpResult(false, true, excecao.Message);
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

        public async Task<IActionResult> BuscarPorEmpresaLogada()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                var dynEmpresa = BuscarDynEmpresaCNPJ(Empresa, unitOfWork);

                return new JsonpResult(dynEmpresa);
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

        private dynamic BuscarDynEmpresaCNPJ(Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe repTransportadorConfiguracaoNFSe = new Repositorio.Embarcador.Transportadores.TransportadorConfiguracaoNFSe(unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.TransportadorConfiguracaoNFSe transportadorConfiguracaoNFSe = repTransportadorConfiguracaoNFSe.BuscarPorCodigoEmpresa(empresa.Codigo);

            Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unitOfWork);
            List<Dominio.Entidades.EmpresaSerie> series = repSerie.BuscarTodosPorEmpresa(empresa.Codigo);

            var dynEmpresa = new
            {
                empresa.Codigo,
                empresa.RazaoSocial,
                empresa.CodigoIntegracao,
                empresa.NomeFantasia,
                empresa.Tipo,
                empresa.CNPJ,
                empresa.RegistroANTT,
                empresa.InscricaoEstadual,
                empresa.InscricaoMunicipal,
                empresa.CNAE,
                empresa.CEP,
                Localidade = new { empresa.Localidade.Codigo, Descricao = empresa.Localidade.DescricaoCidadeEstado },
                empresa.Endereco,
                empresa.Numero,
                empresa.Bairro,
                empresa.Complemento,
                empresa.Telefone,
                empresa.Status,
                EnviarEmail = empresa.StatusEmail == "A",
                EnviarEmailAdministrativo = empresa.StatusEmailAdministrativo == "A",
                EnviarEmailContador = empresa.StatusEmailContador == "A",
                empresa.Email,
                empresa.EmailAdministrativo,
                empresa.EmailContador,
                UsarTipoOperacaoApolice = empresa.UsarTipoOperacaoApolice,
                Certificado = new
                {
                    DataInicial = empresa.DataInicialCertificado.ToDateString(),
                    DataFinal = empresa.DataFinalCertificado.ToDateString(),
                    SerieCertificado = empresa.SerieCertificado,
                    Senha = empresa.SenhaCertificado,
                    PossuiCertificado = !string.IsNullOrWhiteSpace(empresa.NomeCertificado) ? Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) : false,
                },
                CTEMDF = new
                {
                    SerieInterestadual = empresa.Configuracao.SerieInterestadual != null ? new { Codigo = empresa.Configuracao.SerieInterestadual.Codigo, Descricao = empresa.Configuracao.SerieInterestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    SerieIntraestadual = empresa.Configuracao.SerieIntraestadual != null ? new { Codigo = empresa.Configuracao.SerieIntraestadual.Codigo, Descricao = empresa.Configuracao.SerieIntraestadual.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    SerieMDFe = empresa.Configuracao.SerieMDFe != null ? new { Codigo = empresa.Configuracao.SerieMDFe.Codigo, Descricao = empresa.Configuracao.SerieMDFe.Numero.ToString() } : new { Codigo = 0, Descricao = "" },
                    documentoFretes = empresa.CodigoDocumento,
                    tempoEmissao = empresa.TempoDelayHorasParaIniciarEmissao,
                    aliquotaICMS = empresa.AliquotaICMSSimples,
                    aliquotaPIS = (empresa.Configuracao.AliquotaPIS ?? 0m).ToString("n2"),
                    aliquotaCOFINS = (empresa.Configuracao.AliquotaCOFINS ?? 0m).ToString("n2"),
                    fraseNFS = (empresa.Configuracao.FraseSecretaNFSe ?? ""),
                    senhaPrefeitura = (empresa.Configuracao.SenhaNFSe ?? ""),
                    quantidadeEmailRPS = empresa.QuantidadeMaximaEmailRPS,
                    observacaoCTe = empresa.ObservacaoCTe,
                },
                TransportadorConfiguracaoNFSe = new
                {
                    Codigo = transportadorConfiguracaoNFSe?.Codigo ?? 0,
                    AliquotaISS = transportadorConfiguracaoNFSe?.AliquotaISS ?? 0,
                    FraseSecreta = transportadorConfiguracaoNFSe?.FraseSecreta ?? string.Empty,
                    LocalidadePrestacao = transportadorConfiguracaoNFSe?.LocalidadePrestacao != null ?
                                new { transportadorConfiguracaoNFSe.LocalidadePrestacao.Codigo, Descricao = transportadorConfiguracaoNFSe.LocalidadePrestacao.DescricaoCidadeEstado } :
                                new { Codigo = 0, Descricao = string.Empty },
                    LoginSitePrefeitura = transportadorConfiguracaoNFSe?.LoginSitePrefeitura ?? string.Empty,
                    NaturezaNFSe = transportadorConfiguracaoNFSe?.NaturezaNFSe != null ?
                                new { transportadorConfiguracaoNFSe.NaturezaNFSe.Codigo, transportadorConfiguracaoNFSe.NaturezaNFSe.Descricao } :
                                new { Codigo = 0, Descricao = string.Empty },
                    DiscriminacaoNFSe = transportadorConfiguracaoNFSe?.DiscriminacaoNFSe ?? string.Empty,
                    RetencaoISS = transportadorConfiguracaoNFSe?.RetencaoISS ?? 0,
                    SenhaSitePrefeitura = transportadorConfiguracaoNFSe?.SenhaSitePrefeitura ?? string.Empty,
                    SerieNFSe = transportadorConfiguracaoNFSe?.SerieNFSe != null ?
                                new { transportadorConfiguracaoNFSe.SerieNFSe.Codigo, Descricao = transportadorConfiguracaoNFSe.SerieNFSe.Numero } :
                                new { Codigo = 0, Descricao = 0 },
                    SerieRPS = transportadorConfiguracaoNFSe?.SerieRPS ?? string.Empty,
                    ServicoNFSe = transportadorConfiguracaoNFSe?.ServicoNFSe != null ?
                                new { transportadorConfiguracaoNFSe.ServicoNFSe.Codigo, transportadorConfiguracaoNFSe.ServicoNFSe.Descricao } :
                                new { Codigo = 0, Descricao = string.Empty },
                    URLPrefeitura = transportadorConfiguracaoNFSe?.URLPrefeitura ?? string.Empty,
                    PermiteAnular = transportadorConfiguracaoNFSe?.PermiteAnular ?? false,
                    PrazoCancelamento = transportadorConfiguracaoNFSe?.PrazoCancelamento ?? 0,
                    UFTomador = new { Codigo = transportadorConfiguracaoNFSe?.UFTomador?.Sigla ?? "", Descricao = transportadorConfiguracaoNFSe?.UFTomador?.Descricao ?? "" },
                    GrupoTomador = new { Codigo = transportadorConfiguracaoNFSe?.GrupoTomador?.Codigo ?? 0, Descricao = transportadorConfiguracaoNFSe?.GrupoTomador?.Descricao ?? "" },
                    TipoOperacao = new { Codigo = transportadorConfiguracaoNFSe?.TipoOperacao?.Codigo ?? 0, Descricao = transportadorConfiguracaoNFSe?.TipoOperacao?.Descricao ?? string.Empty },
                    ClienteTomador = new { Codigo = transportadorConfiguracaoNFSe?.ClienteTomador?.CPF_CNPJ ?? 0, Descricao = transportadorConfiguracaoNFSe?.ClienteTomador?.Descricao ?? "" }
                },
                Series = (
                    from obj in series
                    orderby obj.Numero, obj.Tipo
                    select new
                    {
                        Codigo = obj.Codigo,
                        Numero = obj.Numero,
                        Status = obj.Status,
                        Tipo = obj.Tipo,
                        obj.ProximoNumeroDocumento,
                        obj.NaoGerarCargaAutomaticamente,

                    }
                ).ToList()
            };
            return dynEmpresa;
        }

        #endregion
    }
}
