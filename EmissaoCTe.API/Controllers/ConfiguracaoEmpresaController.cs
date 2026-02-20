using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EmissaoCTe.API.Controllers
{
    public class ConfiguracaoEmpresaController : ApiController
    {
        private Dominio.ObjetosDeValor.PaginaUsuario Permissao()
        {
            return (from obj in this.BuscarPaginasUsuario() where obj.Pagina.Formulario.ToLower().Equals("configuracoesempresasemissoras.aspx") select obj).FirstOrDefault();
        }

        #region Métodos Públicos

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhes()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo = 0;
                bool editandoConfigEmpresa = true;
                if (string.IsNullOrEmpty(Request.Params["Codigo"]))
                {
                    if (this.EmpresaUsuario.Configuracao == null && this.EmpresaUsuario.EmpresaPai != null)
                        codigo = this.EmpresaUsuario.EmpresaPai.Codigo;
                    else
                        codigo = this.EmpresaUsuario.Codigo;

                    editandoConfigEmpresa = false;
                }
                else
                {
                    codigo = int.Parse(Servicos.Criptografia.Descriptografar(HttpUtility.UrlDecode(Request.Params["Codigo"]), "CT3##MULT1@#$S0FTW4R3"));
                }

                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.EstadosDeEmissaoSerie repEstadosDeEmissaoSerie = new Repositorio.EstadosDeEmissaoSerie(unitOfWork);
                Repositorio.ConfiguracaoEmpresaClienteSerie repConfiguracaoEmpresaClienteSerie = new Repositorio.ConfiguracaoEmpresaClienteSerie(unitOfWork);
                Repositorio.ConfiguracaoAverbacaoClientes repConfiguracaoAverbacaoClientes = new Repositorio.ConfiguracaoAverbacaoClientes(unitOfWork);
                Repositorio.ConfiguracaoAverbacaoSerie repConfiguracaoAverbacaoSerie = new Repositorio.ConfiguracaoAverbacaoSerie(unitOfWork);
                Repositorio.ConfiguracaoFTP repConfiguracaoFTP = new Repositorio.ConfiguracaoFTP(unitOfWork);
                Repositorio.ConfiguracaoAutDownloadXML repConfiguracaoAutDownloadXML = new Repositorio.ConfiguracaoAutDownloadXML(unitOfWork);
                Repositorio.ConfiguracaoArquivosDica repConfiguracaoArquivosDica = new Repositorio.ConfiguracaoArquivosDica(unitOfWork);
                Repositorio.ServicoNFsePorCidade repServicoNFsePorCidade = new Repositorio.ServicoNFsePorCidade(unitOfWork);
                Repositorio.CodigosServicoNFSe repCodigosServicoNFSe = new Repositorio.CodigosServicoNFSe(unitOfWork);
                Repositorio.EstadosBloqueadosEmissao repEstadosBloqueadosEmissao = new Repositorio.EstadosBloqueadosEmissao(unitOfWork);

                Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);
                int countMDFeEmpresa = repMDFe.ContarPorEmpresa(codigo);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigo);
                if (empresa != null)
                {
                    if (empresa.Configuracao != null)
                    {
                        List<Dominio.Entidades.EstadosDeEmissaoSerie> listaEstadosDeEmissaoSerie = repEstadosDeEmissaoSerie.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie> listaConfiguracaoEmpresaClienteSerie = repConfiguracaoEmpresaClienteSerie.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoAverbacaoClientes> listaAverbacoesCliente = repConfiguracaoAverbacaoClientes.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoAverbacaoSerie> listaAverbacoesSerie = repConfiguracaoAverbacaoSerie.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoFTP> listaConfiguracaoFTP = repConfiguracaoFTP.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoAutDownloadXML> listaCNPJCPFAutorizados = repConfiguracaoAutDownloadXML.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ConfiguracaoArquivosDica> listaArquivosDica = repConfiguracaoArquivosDica.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.ServicoNFsePorCidade> listaServicoNFsePorCidade = repServicoNFsePorCidade.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.CodigosServicoNFSe> listaCodigosServicoNFSe = repCodigosServicoNFSe.BuscarPorConfiguracao(empresa.Configuracao.Codigo);
                        List<Dominio.Entidades.EstadosBloqueadosEmissao> listaEstadosBloqueadosEmissao = repEstadosBloqueadosEmissao.BuscarPorConfiguracao(empresa.Configuracao.Codigo);

                        Dominio.Enumeradores.TipoSeguro? responsavelSeguro = null;

                        responsavelSeguro = editandoConfigEmpresa && empresa.Configuracao.ResponsavelSeguro != null ? empresa.Configuracao.ResponsavelSeguro :
                                            !editandoConfigEmpresa && empresa.Configuracao.ResponsavelSeguro == null ? Dominio.Enumeradores.TipoSeguro.Remetente : empresa.Configuracao.ResponsavelSeguro;

                        string cnpjMatrizCIOT = empresa.Configuracao.EmpresaMatrizCIOT != null ? empresa.Configuracao.EmpresaMatrizCIOT.CNPJ : string.Empty;

                        var retorno = new
                        {
                            CodigoEmpresa = empresa.Codigo,
                            CodigoAtividade = empresa.Configuracao.Atividade != null ? empresa.Configuracao.Atividade.Codigo : 0,
                            DescricaoAtividade = empresa.Configuracao.Atividade != null ? empresa.Configuracao.Atividade.Descricao : "",
                            empresa.Configuracao.ObservacaoCTeNormal,
                            empresa.Configuracao.ObservacaoCTeAnulacao,
                            empresa.Configuracao.ObservacaoCTeComplementar,
                            empresa.Configuracao.ObservacaoCTeSubstituicao,
                            empresa.Configuracao.PrazoCancelamentoCTe,
                            empresa.Configuracao.PrazoCancelamentoMDFe,
                            empresa.RazaoSocial,
                            empresa.Configuracao.DiasParaEntrega,
                            empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao,
                            empresa.Configuracao.DiasParaEmissaoDeCTeComplementar,
                            empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao,
                            empresa.Configuracao.IndicadorDeLotacao,
                            empresa.Configuracao.EmitirSemValorDaCarga,
                            empresa.Configuracao.DicasEmissaoCTe,
                            empresa.Configuracao.CadastrarItemDocumentoEntrada,
                            empresa.Configuracao.PermiteSelecionarCTeOutroTomador,
                            ArquivosDicas = (from obj in listaArquivosDica
                                             select new
                                             {
                                                 obj.Codigo,
                                                 Nome = obj.NomeArquivo
                                             }),
                            empresa.Configuracao.ProdutoPredominante,
                            OutrasCaracteristicas = !string.IsNullOrWhiteSpace(empresa.Configuracao.OutrasCaracteristicas) ? empresa.Configuracao.OutrasCaracteristicas : string.Empty,
                            empresa.Configuracao.TipoImpressao,
                            CodigoContaAbastecimento = empresa.Configuracao.PlanoAbastecimento != null ? empresa.Configuracao.PlanoAbastecimento.Codigo : 0,
                            DescricaoContaAbastecimento = empresa.Configuracao.PlanoAbastecimento != null ? string.Concat(empresa.Configuracao.PlanoAbastecimento.Conta, " - ", empresa.Configuracao.PlanoAbastecimento.Descricao) : string.Empty,
                            CodigoContaCTe = empresa.Configuracao.PlanoCTe != null ? empresa.Configuracao.PlanoCTe.Codigo : 0,
                            DescricaoContaCTe = empresa.Configuracao.PlanoCTe != null ? string.Concat(empresa.Configuracao.PlanoCTe.Conta, " - ", empresa.Configuracao.PlanoCTe.Descricao) : string.Empty,
                            CodigoContaPagamentoMotorista = empresa.Configuracao.PlanoPagamentoMotorista != null ? empresa.Configuracao.PlanoPagamentoMotorista.Codigo : 0,
                            DescricaoContaPagamentoMotorista = empresa.Configuracao.PlanoPagamentoMotorista != null ? string.Concat(empresa.Configuracao.PlanoPagamentoMotorista.Conta, " - ", empresa.Configuracao.PlanoPagamentoMotorista.Descricao) : string.Empty,
                            empresa.Configuracao.UtilizaTabelaDeFrete,
                            empresa.Configuracao.PermiteVincularMesmaPlacaOutrosVeiculos,
                            empresa.Configuracao.NaoCopiarImpostosCTeAnterior,
                            empresa.Configuracao.GeraDuplicatasAutomaticamente,
                            empresa.Configuracao.DiasParaVencimentoDasDuplicatas,
                            empresa.Configuracao.DiasParaAvisoVencimentos,
                            empresa.Configuracao.NumeroDeParcelasDasDuplicatas,
                            CodigoSerieInterestadual = empresa.Configuracao.SerieInterestadual != null ? empresa.Configuracao.SerieInterestadual.Codigo : 0,
                            NumeroSerieInterestadual = empresa.Configuracao.SerieInterestadual != null ? empresa.Configuracao.SerieInterestadual.Numero : 0,
                            CodigoSerieIntraestadual = empresa.Configuracao.SerieIntraestadual != null ? empresa.Configuracao.SerieIntraestadual.Codigo : 0,
                            NumeroSerieIntraestadual = empresa.Configuracao.SerieIntraestadual != null ? empresa.Configuracao.SerieIntraestadual.Numero : 0,
                            CodigoSerieComplementar = empresa.Configuracao.SerieCTeComplementar != null ? empresa.Configuracao.SerieCTeComplementar.Codigo : 0,
                            NumeroSerieComplementar = empresa.Configuracao.SerieCTeComplementar != null ? empresa.Configuracao.SerieCTeComplementar.Numero : 0,
                            CodigoSerieMDFe = empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe.Codigo : 0,
                            NumeroSerieMDFe = empresa.Configuracao.SerieMDFe != null ? empresa.Configuracao.SerieMDFe.Numero : 0,
                            TipoGeracaoCTeWS = empresa.Configuracao.TipoGeracaoCTeWS.ToString("d"),
                            Permissoes = this.ObterPermissoesDaEmpresa(empresa.Codigo, unitOfWork),
                            empresa.Configuracao.ICMSIsento,
                            IncluirICMS = empresa.Configuracao.IncluirICMSNoFrete,
                            empresa.Configuracao.Perfil,
                            empresa.Configuracao.CriterioEscrituracaoEApuracao,
                            empresa.Configuracao.IncidenciaTributariaNoPeriodo,
                            AliquotaCOFINS = empresa.Configuracao.AliquotaCOFINS.HasValue ? empresa.Configuracao.AliquotaCOFINS.Value.ToString("n2") : "",
                            CSTCOFINS = empresa.Configuracao.CSTCOFINS.HasValue ? empresa.Configuracao.CSTCOFINS.Value.ToString("D") : "",
                            AliquotaPIS = empresa.Configuracao.AliquotaPIS.HasValue ? empresa.Configuracao.AliquotaPIS.Value.ToString("n2") : "",
                            CSTPIS = empresa.Configuracao.CSTPIS.HasValue ? empresa.Configuracao.CSTPIS.Value.ToString("D") : "",
                            CodigoApoliceSeguro = empresa.Configuracao.ApoliceSeguro != null ? empresa.Configuracao.ApoliceSeguro.Codigo : 0,
                            DescricaoApoliceSeguro = empresa.Configuracao.ApoliceSeguro != null ? empresa.Configuracao.ApoliceSeguro.NumeroApolice + " - " + empresa.Configuracao.ApoliceSeguro.NomeSeguradora : string.Empty,
                            NomeSeguradora = empresa.Configuracao.ApoliceSeguro != null ? empresa.Configuracao.ApoliceSeguro.NomeSeguradora : string.Empty,
                            CNPJSeguradora = empresa.Configuracao.ApoliceSeguro != null ? empresa.Configuracao.ApoliceSeguro.CNPJSeguradora : string.Empty,
                            NumeroApoliceSeguro = empresa.Configuracao.ApoliceSeguro != null ? empresa.Configuracao.ApoliceSeguro.NumeroApolice : string.Empty,
                            ObservacaoCTeAvancadaProprio = empresa.Configuracao.ObservacaoCTeAvancadaProprio,
                            ObservacaoCTeAvancadaTerceiros = empresa.Configuracao.ObservacaoCTeAvancadaTerceiros,
                            PercentualImpostoSimplesNacional = (empresa.Configuracao.PercentualImpostoSimplesNacional.HasValue ? empresa.Configuracao.PercentualImpostoSimplesNacional.Value : 0m).ToString("n2"),
                            this.EmpresaUsuario.OptanteSimplesNacional,
                            CodigoATM = empresa.Configuracao.CodigoSeguroATM,
                            UsuarioATM = empresa.Configuracao.UsuarioSeguroATM,
                            SenhaATM = empresa.Configuracao.SenhaSeguroATM,
                            AverbaAutomaticoATM = empresa.Configuracao.AverbaAutomaticoATM,
                            ListaAverbacoes = (from obj in listaAverbacoesCliente
                                               select new
                                               {
                                                   Id = obj.Codigo,
                                                   CnpjCliente = obj.Cliente.CPF_CNPJ,
                                                   Nome = obj.Cliente.CPF_CNPJ_Formatado + " " + obj.Cliente.Nome,
                                                   Tipo = obj.TipoTomador,
                                                   IntegradoraAverbacao = obj.IntegradoraAverbacao != null ? obj.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : obj.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : obj.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : obj.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ELT ? "E" : obj.IntegradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido ? "" : "" : "",
                                                   obj.CodigoAverbacao,
                                                   obj.UsuarioAverbacao,
                                                   obj.SenhaAverbacao,
                                                   obj.TokenAverbacao,
                                                   obj.RaizCNPJ,
                                                   obj.NaoAverbar,
                                                   obj.TipoCTeAverbacao,
                                                   Excluir = false
                                               }).OrderBy(o => o.IntegradoraAverbacao).ToList(),
                            ListaAverbacoesSerie = (from obj in listaAverbacoesSerie
                                                    select new
                                                    {
                                                        Id = obj.Codigo,
                                                        CodigoSerie = obj.EmpresaSerie.Codigo,
                                                        NumeroSerie = obj.EmpresaSerie.Numero,
                                                        Excluir = false
                                                    }).OrderBy(o => o.NumeroSerie).ToList(),
                            ListaFTP = (from obj in listaConfiguracaoFTP
                                        select new Dominio.ObjetosDeValor.ConfiguracaoFTP()
                                        {
                                            Id = obj.Codigo,
                                            Cliente = obj.Cliente.CPF_CNPJ,
                                            DescricaoCliente = obj.Cliente != null ? obj.Cliente.Nome : string.Empty,
                                            LayoutEDI = obj.LayoutEDI != null ? obj.LayoutEDI.Codigo.ToString() : string.Empty,
                                            DescricaoLayoutEDI = obj.LayoutEDI != null ? obj.LayoutEDI.Descricao : string.Empty,
                                            Tipo = obj.Tipo,
                                            DescricaoTipo = obj.Tipo != null ? obj.Tipo.ToString().Replace("_", " ") : string.Empty,
                                            TipoArquivo = obj.TipoArquivo,
                                            DescricaoTipoArquivo = obj.DescricaoTipoArquivo,
                                            Rateio = obj.Rateio,
                                            DescricaoRateio = obj.DescricaoRateio,
                                            Host = obj.Host,
                                            Porta = obj.Porta,
                                            Usuario = obj.Usuario,
                                            Senha = obj.Senha,
                                            Diretorio = obj.Diretorio,
                                            SSL = obj.SSL,
                                            Seguro = obj.Seguro,
                                            GerarNFSe = obj.GerarNFSe,
                                            EmitirDocumento = obj.EmitirDocumento,
                                            UtilizarContratanteComoTomador = obj.UtilizarContratanteComoTomador,
                                            Excluir = false
                                        }).ToList(),
                            TipoEmpresaCIOT = empresa.Configuracao.TipoEmpresaCIOT,
                            TipoIntegradoraCIOT = empresa.Configuracao.TipoIntegradoraCIOT.HasValue ? empresa.Configuracao.TipoIntegradoraCIOT.Value.ToString("d") : string.Empty,
                            TipoPagamentoCIOT = empresa.Configuracao.TipoPagamentoCIOT.HasValue ? empresa.Configuracao.TipoPagamentoCIOT.Value.ToString("d") : string.Empty,
                            empresa.Configuracao.ChaveCriptograficaSigaFacil,
                            empresa.Configuracao.CodigoContratanteSigaFacil,
                            empresa.Configuracao.CodigoIntegradorEFrete,
                            empresa.Configuracao.SenhaEFrete,
                            empresa.Configuracao.UsuarioEFrete,
                            empresa.Configuracao.BloquearNaoEquiparadoEFrete,
                            empresa.Configuracao.EmissaoGratuitaEFrete,
                            empresa.Configuracao.TruckPadURL,
                            empresa.Configuracao.TruckPadUser,
                            empresa.Configuracao.TruckPadPassword,
                            CodigoSerieNFSe = empresa.Configuracao.SerieNFSe?.Codigo,
                            NumeroSerieNFSe = empresa.Configuracao.SerieNFSe?.Numero,
                            empresa.Configuracao.SenhaNFSe,
                            empresa.Configuracao.FraseSecretaNFSe,
                            empresa.Configuracao.SerieRPSNFSe,
                            CodigoServicoNFSe = empresa.Configuracao.ServicoNFSe?.Codigo,
                            DescricaoServicoNFSe = empresa.Configuracao.ServicoNFSe?.Descricao,
                            CodigoServicoNFSeFora = empresa.Configuracao.ServicoNFSeFora?.Codigo,
                            DescricaoServicoNFSeFora = empresa.Configuracao.ServicoNFSeFora?.Descricao,
                            CodigoNaturezaNFSe = empresa.Configuracao.NaturezaNFSe?.Codigo,
                            DescricaoNaturezaNFSe = empresa.Configuracao.NaturezaNFSe?.Descricao,
                            CodigoNaturezaForaNFSe = empresa.Configuracao.NaturezaNFSeFora?.Codigo,
                            DescricaoNaturezaForaNFSe = empresa.Configuracao.NaturezaNFSeFora?.Descricao,
                            NomePDFCTe = empresa.Configuracao.NomePDFCTe,

                            NaturaMatriz = empresa.Configuracao.CodigoMatrizNatura,
                            NaturaFilial = empresa.Configuracao.CodigoFilialNatura,
                            NaturaUsuario = empresa.Configuracao.UsuarioNatura,
                            NaturaSenha = empresa.Configuracao.SenhaNatura,
                            LayoutEDINatura = empresa.Configuracao.LayoutEDINatura != null ? empresa.Configuracao.LayoutEDINatura.Codigo : 0,
                            DescricaoNaturaLayoutEDI = empresa.Configuracao.LayoutEDINatura != null ? empresa.Configuracao.LayoutEDINatura.Descricao : string.Empty,

                            FTPNaturaHost = empresa.Configuracao.FTPNaturaHost,
                            FTPNaturaPorta = empresa.Configuracao.FTPNaturaPorta,
                            FTPNaturaUsuario = empresa.Configuracao.FTPNaturaUsuario,
                            FTPNaturaSenha = empresa.Configuracao.FTPNaturaSenha,
                            FTPNaturaDiretorio = empresa.Configuracao.FTPNaturaDiretorio,
                            FTPNaturaPassivo = empresa.Configuracao.FTPNaturaPassivo,
                            BloquearDuplicidadeCTeAcerto = empresa.Configuracao.BloquearDuplicidadeCTeAcerto,
                            FTPNaturaSeguro = empresa.Configuracao.FTPNaturaSeguro,
                            NaturaLayoutEDIOcoren = empresa.Configuracao.LayoutEDIOcoren != null ? empresa.Configuracao.LayoutEDIOcoren.Codigo : 0,
                            DescricaoNaturaLayoutEDIOcoren = empresa.Configuracao.LayoutEDIOcoren != null ? empresa.Configuracao.LayoutEDIOcoren.Descricao : string.Empty,

                            SeguradoraCNPJ = empresa.Configuracao.CNPJSeguro,
                            SeguradoraNome = empresa.Configuracao.NomeSeguro,
                            SeguradoraNApolice = empresa.Configuracao.NumeroApoliceSeguro,
                            SeguradoraNAverbacao = empresa.Configuracao.AverbacaoSeguro,
                            ResponsavelSeguro = responsavelSeguro,

                            ObservacaoCTeSimplesNacional = empresa.Configuracao.ObservacaoCTeSimplesNacional != null ? empresa.Configuracao.ObservacaoCTeSimplesNacional : string.Empty,
                            empresa.Configuracao.AtualizaVeiculoImpXMLCTe,
                            empresa.Configuracao.TokenIntegracaoCTe,
                            empresa.Configuracao.TokenIntegracaoEnvioCTe,
                            empresa.Configuracao.WsIntegracaoEnvioCTe,
                            empresa.Configuracao.URLPrefeituraNFSe,
                            empresa.Configuracao.LoginSitePrefeituraNFSe,
                            empresa.Configuracao.SenhaSitePrefeituraNFSe,
                            empresa.Configuracao.ObservacaoIntegracaoNFSe,
                            empresa.Configuracao.ObservacaoPadraoNFSe,
                            seriesUF = (from obj in listaEstadosDeEmissaoSerie
                                        select new Dominio.ObjetosDeValor.EstadosDeEmissaoSerie()
                                        {
                                            SiglaUF = obj.Estado.Sigla,
                                            CodigoSerie = obj.Serie != null ? obj.Serie.Codigo : 0,
                                            Serie = obj.Serie != null ? obj.Serie.Numero : 0,
                                            Excluir = false
                                        }).ToList(),
                            estadosBloqueados = (from obj in listaEstadosBloqueadosEmissao
                                        select new Dominio.ObjetosDeValor.EstadosBloqueados()
                                        {
                                            SiglaUF = obj.Estado.Sigla,
                                            Excluir = false
                                        }).ToList(),
                            seriesCliente = (from obj in listaConfiguracaoEmpresaClienteSerie
                                             select new Dominio.ObjetosDeValor.ConfiguracaoEmpresaClienteSerie()
                                             {
                                                 CnpjCliente = obj.Cliente.CPF_CNPJ_SemFormato,
                                                 CodigoSerie = obj.Serie != null ? obj.Serie.Codigo : 0,
                                                 Serie = obj.Serie != null ? obj.Serie.Numero : 0,
                                                 TipoCliente = ((int)obj.TipoCliente).ToString(),
                                                 DescricaoTipoCliente = obj.TipoCliente == Dominio.Enumeradores.TipoTomador.Destinatario ? "Destinatario" :
                                                                        obj.TipoCliente == Dominio.Enumeradores.TipoTomador.Expedidor ? "Expedidor" :
                                                                        obj.TipoCliente == Dominio.Enumeradores.TipoTomador.Recebedor ? "Recebedor" :
                                                                        obj.TipoCliente == Dominio.Enumeradores.TipoTomador.Remetente ? "Remetente" : "Tomador",
                                                 RaizCNPJ = obj.RaizCNPJ ? "Sim" : "Não",
                                                 Excluir = false
                                             }).ToList(),
                            PrimeiroNumeroMDFe = empresa.PrimeiroNumeroMDFe,
                            JaEmitiuMDFe = countMDFeEmpresa > 0 ? true : false,
                            DocumentosXML = (from obj in listaCNPJCPFAutorizados
                                             select new
                                             {
                                                 Id = obj.Codigo,
                                                 CnpjCpf = obj.CNPJCPF,
                                                 Excluir = false
                                             }).ToList(),
                            empresa.Configuracao.AssinaturaEmail,
                            empresa.Configuracao.UtilizaNovaImportacaoEDI,
                            ModeloPadrao = !string.IsNullOrWhiteSpace(empresa.Configuracao.ModeloPadrao) ? empresa.Configuracao.ModeloPadrao : "57",
                            NFSeIntegracaoENotas = empresa.Configuracao.NFSeIntegracaoENotas ? "1" : "0",
                            empresa.Configuracao.EmiteNFSeForaEmbarcador,
                            empresa.NFSeIDENotas,
                            empresa.Configuracao.NFSeCPF,
                            SeguradoraAverbacao = empresa.Configuracao.SeguradoraAverbacao != null ? empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ELT ? "E" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido ? "" : "" : "",
                            TokenAverbacao = empresa.Configuracao.TokenAverbacaoBradesco ?? string.Empty,
                            WsdlQuorum = empresa.Configuracao.WsdlAverbacaoQuorum ?? string.Empty,
                            empresa.Configuracao.EmailSemTexto,
                            empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai,
                            empresa.Configuracao.AguardarAverbacaoCTeParaEmitirMDFe,
                            VersaoCTe = !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoCTe) ? empresa.Configuracao.VersaoCTe : string.Empty,
                            TipoPesoNFe = empresa.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Bruto ? 0 : 1,
                            empresa.Configuracao.BloquearEmissaoMDFeWS,
                            empresa.Configuracao.GerarNFSeImportacoes,

                            empresa.Configuracao.AcertoViagemMovimentoReceitas,
                            empresa.Configuracao.AcertoViagemMovimentoDespesas,
                            empresa.Configuracao.AcertoViagemMovimentoDespesasAbastecimentos,
                            empresa.Configuracao.AcertoViagemMovimentoDespesasAdiantamentosMotorista,
                            empresa.Configuracao.AcertoViagemMovimentoReceitasDevolucoesMotorista,

                            AcertoViagemContaReceitas = empresa.Configuracao.AcertoViagemContaReceitas == null ? null : new { empresa.Configuracao.AcertoViagemContaReceitas.Codigo, empresa.Configuracao.AcertoViagemContaReceitas.Descricao },
                            AcertoViagemContaDespesas = empresa.Configuracao.AcertoViagemContaDespesas == null ? null : new { empresa.Configuracao.AcertoViagemContaDespesas.Codigo, empresa.Configuracao.AcertoViagemContaDespesas.Descricao },
                            AcertoViagemContaDespesasAbastecimentos = empresa.Configuracao.AcertoViagemContaDespesasAbastecimentos == null ? null : new { empresa.Configuracao.AcertoViagemContaDespesasAbastecimentos.Codigo, empresa.Configuracao.AcertoViagemContaDespesasAbastecimentos.Descricao },
                            AcertoViagemContaDespesasPagamentosMotorista = empresa.Configuracao.AcertoViagemContaDespesasPagamentosMotorista == null ? null : new { empresa.Configuracao.AcertoViagemContaDespesasPagamentosMotorista.Codigo, empresa.Configuracao.AcertoViagemContaDespesasPagamentosMotorista.Descricao },
                            AcertoViagemContaDespesasAdiantamentosMotorista = empresa.Configuracao.AcertoViagemContaDespesasAdiantamentosMotorista == null ? null : new { empresa.Configuracao.AcertoViagemContaDespesasAdiantamentosMotorista.Codigo, empresa.Configuracao.AcertoViagemContaDespesasAdiantamentosMotorista.Descricao },
                            AcertoViagemContaReceitasDevolucoesMotorista = empresa.Configuracao.AcertoViagemContaReceitasDevolucoesMotorista == null ? null : new { empresa.Configuracao.AcertoViagemContaReceitasDevolucoesMotorista.Codigo, empresa.Configuracao.AcertoViagemContaReceitasDevolucoesMotorista.Descricao },

                            empresa.Configuracao.FormatarPlacaComHifenNaObservacao,
                            empresa.Configuracao.TipoCTeAverbacao,
                            empresa.Configuracao.ImportacaoNaoRateiaPedagio,
                            empresa.Configuracao.TagParaImportarObservacaoNFe,
                            empresa.Configuracao.TamanhoTagObservacaoNFe,
                            empresa.Configuracao.GerarCTeIntegracaoDocumentosMunicipais,
                            empresa.Configuracao.DescricaoMedidaKgCTe,
                            empresa.Configuracao.NaturaEnviaOcorrenciaEntreguePadrao,
                            empresa.Configuracao.AverbarMDFe,
                            empresa.Configuracao.NaoCalcularDIFALCTeOS,
                            empresa.Configuracao.NaoPermitirDuplciataMesmoDocumento,
                            empresa.Configuracao.ExibirHomeVencimentoCertificado,
                            empresa.Configuracao.ExibirHomePendenciasEntrega,
                            empresa.Configuracao.ExibirHomeGraficosEmissoes,
                            empresa.Configuracao.ExibirHomeServicosVeiculos,
                            empresa.Configuracao.ExibirHomeParcelaDuplicatas,
                            empresa.Configuracao.ExibirHomePagamentosMotoristas,
                            empresa.Configuracao.ExibirHomeAcertoViagem,
                            empresa.Configuracao.ExibirHomeMDFesPendenteEncerramento,
                            empresa.Configuracao.NaoCopiarSeguroCTeAnterior,
                            BloquearEmissaoCTeCargaMunicipal = empresa.Configuracao.BloquearEmissaoCTeParaCargaMunicipal,
                            empresa.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca,
                            empresa.Configuracao.BloquearEmissaoCTeComUFDestinosDiferentes,
                            ValorLimiteFrete = empresa.Configuracao.ValorLimiteFrete.ToString("n2"),
                            ExibirCobrancaCancelamento = this.UsuarioAdministrativo != null ? empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null ? empresa.EmpresaPai.Configuracao.ExibirCobrancaCancelamento : false : false,
                            empresa.Configuracao.AverbarNFSe,
                            AliquotaIR = empresa.Configuracao.AliquotaIR.ToString("n2"),
                            AliquotaINSS = empresa.Configuracao.AliquotaINSS.ToString("n2"),
                            AliquotaCSLL = empresa.Configuracao.AliquotaCSLL.ToString("n2"),
                            PercentualBaseINSS = empresa.Configuracao.PercentualBaseINSS.ToString("n2"),
                            empresa.Configuracao.DescontarINSSValorReceber,
                            empresa.Configuracao.CopiarObservacaoFiscoContribuinteCTeAnterior,
                            empresa.Configuracao.NaoCopiarValoresCTeAnterior,
                            UtilizaResumoEmissaoCTe = this.UsuarioAdministrativo != null && empresa.EmpresaPai != null && empresa.EmpresaPai.Configuracao != null ? empresa.EmpresaPai.Configuracao.UtilizaResumoEmissaoCTe : empresa.Configuracao != null ? empresa.Configuracao.UtilizaResumoEmissaoCTe : false,
                            empresa.Configuracao.AdicionarResponsavelSeguroObsContribuinte,
                            CNPJMatrizCIOT = cnpjMatrizCIOT,
                            empresa.Configuracao.ExibirOpcaoEDICaterpillarDuplicata,
                            empresa.Configuracao.PermiteImportarXMLNFSe,
                            empresa.Configuracao.ArmazenaNotasParaGerarPorPeriodo,
                            empresa.Configuracao.NaoImportarValoresImportacaoCTe,
                            empresa.Configuracao.ExigirObservacaoContribuinteValorContainer,
                            empresa.Configuracao.UsarRegraICMSParaCteDeSubcontratacao,
                            empresa.Configuracao.NaoImportarNotaDuplicadaEDINovaImportacao,
                            empresa.Configuracao.NaoSmarCreditoICMSNoValorDaPrestacao,
                            servicosCidades = (from obj in listaServicoNFsePorCidade
                                               select new Dominio.ObjetosDeValor.ServicoNFsePorCidade()
                                               {
                                                   CodigoCidade = obj.Localidade.Codigo,
                                                   DescricaoCidade = obj.Localidade.Descricao,
                                                   CodigoNatureza = obj.NaturezaNFSe.Codigo,
                                                   DescricaoNatureza = obj.NaturezaNFSe.Descricao,
                                                   CodigoServico = obj.ServicoNFSe.Codigo,
                                                   DescricaoServico = obj.ServicoNFSe.Descricao,
                                                   Excluir = false
                                               }).ToList(),
                            codigosServicos = (from obj in listaCodigosServicoNFSe
                                               select new
                                               {
                                                   Codigo = obj.Codigo,
                                                   CodigoTributacao = obj.CodigoTributacao,
                                                   CodigoTributacaoPrefeitura = obj.CodigoTributacaoPrefeitura,
                                                   NumeroTributacaoPrefeitura = obj.NumeroTributacaoPrefeitura,
                                                   CNAE = obj?.CNAE ?? string.Empty,
                                                   Excluir = false
                                               }).ToList(),
                            empresa.Configuracao.TrafegusURL,
                            empresa.Configuracao.TrafegusUsuario,
                            empresa.Configuracao.TrafegusSenha,
                            empresa.Configuracao.BuonnyURL,
                            empresa.Configuracao.BuonnyToken,
                            empresa.Configuracao.BuonnyGerenciadora,
                            empresa.Configuracao.BuonnyCodigoTipoProduto,
                            IntegradoraSM = empresa.Configuracao.IntegradoraSM.HasValue ? empresa.Configuracao.IntegradoraSM.Value.ToString("d") : string.Empty,
                            empresa.Configuracao.URLMercadoLivre,
                            empresa.Configuracao.SecretKeyMercadoLivre,
                            empresa.Configuracao.IDMercadoLivre,
                            empresa.Configuracao.WsIntegracaoEnvioCTeEmbarcadorTMS,
                            empresa.Configuracao.WsIntegracaoEnvioNFSeEmbarcadorTMS,
                            empresa.Configuracao.WsIntegracaoEnvioMDFeEmbarcadorTMS,
                            empresa.Configuracao.TokenIntegracaoEmbarcadorTMS,
                            empresa.NFSeNacional
                        };
                        return Json(retorno, true);
                    }
                    else
                    {
                        var retorno = new
                        {
                            CodigoAtividade = 0,
                            DescricaoAtividade = "",
                            ObservacaoCTeNormal = "",
                            ObservacaoCTeAnulacao = "",
                            ObservacaoCTeComplementar = "",
                            ObservacaoCTeSubstituicao = "",
                            empresa.RazaoSocial,
                            DiasParaEntrega = 0,
                            DiasParaEmissaoDeCTeAnulacao = 0,
                            DiasParaEmissaoDeCTeComplementar = 0,
                            DiasParaEmissaoDeCTeSubstituicao = 0,
                            IndicadorDeLotacao = false,
                            EmitirSemValorDaCarga = false,
                            DicasEmissaoCTe = "",
                            ProdutoPredominante = "",
                            TipoImpressao = 0,
                            CodigoContaAbastecimento = 0,
                            DescricaoContaAbastecimento = "",
                            CodigoContaCTe = 0,
                            DescricaoContaCTe = "",
                            UtilizaTabelaDeFrete = false,
                            PermiteVincularMesmaPlacaOutrosVeiculos = false,
                            NaoCopiarImpostosCTeAnterior = false,
                            GeraDuplicatasAutomaticamente = 1,
                            DiasParaVencimentoDasDuplicatas = 0,
                            DiasParaAvisoVencimentos = 0,
                            NumeroDeParcelasDasDuplicatas = 0,
                            CodigoSerieInterestadual = 0,
                            NumeroSerieInterestadual = 0,
                            CodigoSerieIntraestadual = 0,
                            NumeroSerieIntraestadual = 0,
                            CodigoSerieComplementar = 0,
                            NumeroSerieComplementar = 0,
                            CodigoSerieMDFe = 0,
                            NumeroSerieMDFe = 0,
                            TipoGeracaoCTeWS = 0,
                            Permissoes = this.ObterPermissoesDaEmpresa(empresa.Codigo, unitOfWork),
                            ICMSIsento = false,
                            IncluirICMS = 0,
                            Perfil = 0,
                            AliquotaCOFINS = "",
                            CSTCOFINS = "",
                            AliquotaPIS = "",
                            CSTPIS = "",
                            CodigoApoliceSeguro = 0,
                            DescricaoApoliceSeguro = string.Empty,
                            ObservacaoCTeAvancadaProprio = string.Empty,
                            ObservacaoCTeAvancadaTerceiros = string.Empty,
                            PercentualImpostoSimplesNacional = string.Empty,
                            this.EmpresaUsuario.OptanteSimplesNacional,
                            CodigoATM = "",
                            UsuarioATM = "",
                            SenhaATM = "",
                            TipoIntegradoraCIOT = string.Empty,
                            TipoPagamentoCIOT = string.Empty,
                            ChaveCriptograficaSigaFacil = string.Empty,
                            CodigoContratanteSigaFacil = string.Empty,
                            CodigoIntegradorEFrete = string.Empty,
                            SenhaEFrete = string.Empty,
                            UsuarioEFrete = string.Empty,
                            BloquearNaoEquiparadoEFrete = false,
                            EmissaoGratuitaEFrete = false,
                            CodigoSerieNFSe = 0,
                            SenhaNFSe = string.Empty,
                            FraseSecretaNFSe = string.Empty,
                            SerieRPSNFSe = string.Empty,
                            CodigoServicoNFSe = 0,
                            DescricaoServicoNFSe = string.Empty,
                            CodigoServicoNFSeFora = 0,
                            DescricaoServicoNFSeFora = string.Empty,
                            CodigoNaturezaNFSe = 0,
                            DescricaoNaturezaNFSe = string.Empty,
                            PrimeiroNumeroMDFe = 0,
                            JaEmitiuMDFe = false,
                            AssinaturaEmail = string.Empty,
                            UtilizaNovaImportacaoEDI = false,
                            ModeloPadrao = "57",
                            NFSeIntegracaoENotas = "0",
                            NFSeIDENotas = string.Empty,
                            NFSeCPF = string.Empty,
                            SeguradoraAverbacao = string.Empty,
                            TokenAverbacao = string.Empty,
                            WsdlQuorum = string.Empty,
                            EmailSemTexto = false,
                            SeguradoraCNPJ = string.Empty,
                            SeguradoraNome = string.Empty,
                            SeguradoraNApolice = string.Empty,
                            SeguradoraNAverbacao = string.Empty,
                            ResponsavelSeguro = Dominio.Enumeradores.TipoSeguro.Emitente_CTE,
                            NaoUtilizarDadosSeguroEmpresaPai = false,
                            AguardarAverbacaoCTeParaEmitirMDFe = false,
                            CadastrarItemDocumentoEntrada = false,
                            PermiteSelecionarCTeOutroTomador = false,
                            VersaoCTe = string.Empty,
                            TipoPesoNFe = 0,
                            BloquearEmissaoMDFeWS = false,
                            GerarNFSeImportacoes = false,
                            FormatarPlacaComHifenNaObservacao = false,
                            TipoCTeAverbacao = 0,
                            ImportacaoNaoRateiaPedagio = false,
                            TagParaImportarObservacaoNFe = string.Empty,
                            TamanhoTagObservacaoNFe = 0,
                            GerarCTeIntegracaoDocumentosMunicipais = false,
                            DescricaoMedidaKgCTe = string.Empty,
                            NaturaEnviaOcorrenciaEntreguePadrao = false,
                            AverbarMDFe = false,
                            NaoCalcularDIFALCTeOS = false,
                            NaoPermitirDuplciataMesmoDocumento = false,
                            ExibirHomeVencimentoCertificado = true,
                            ExibirHomePendenciasEntrega = true,
                            ExibirHomeGraficosEmissoes = true,
                            ExibirHomeServicosVeiculos = false,
                            ExibirHomeParcelaDuplicatas = false,
                            ExibirHomePagamentosMotoristas = false,
                            ExibirHomeAcertoViagem = false,
                            ExibirHomeMDFesPendenteEncerramento = false,
                            NaoCopiarSeguroCTeAnterior = false,
                            BloquearEmissaoCTeCargaMunicipal = false,
                            BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca = false,
                            BloquearEmissaoCTeComUFDestinosDiferentes = false,
                            ValorLimiteFrete = "0,00",
                            ExibirCobrancaCancelamento = false,
                            AverbarNFSe = false,
                            AliquotaIR = "0,00",
                            AliquotaINSS = "0,00",
                            AliquotaCSLL = "0,00",
                            PercentualBaseINSS = "0,00",
                            DescontarINSSValorReceber = false,
                            CopiarObservacaoFiscoContribuinteCTeAnterior = false,
                            NaoCopiarValoresCTeAnterior = false,
                            UtilizaResumoEmissaoCTe = false,
                            AdicionarResponsavelSeguroObsContribuinte = false,
                            CNPJMatrizCIOT = string.Empty,
                            ExibirOpcaoEDICaterpillarDuplicata = false,
                            ArmazenaNotasParaGerarPorPeriodo = false,
                            PermiteImportarXMLNFSe = false,
                            NaoImportarValoresImportacaoCTe = false,
                            ExigirObservacaoContribuinteValorContainer = false,
                            UsarRegraICMSParaCteDeSubcontratacao = false,
                            NaoImportarNotaDuplicadaEDINovaImportacao = false,
                            NFSeNacional = false
                        };
                        return Json(retorno, true);
                    }
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult ObterDetalhesEmpresaAdmin()
        {
            try
            {
                Dominio.Entidades.Empresa empresa = this.EmpresaUsuario;

                if (empresa != null)
                {

                    if (empresa.Configuracao != null)
                    {
                        var retorno = new
                        {
                            empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao,
                            empresa.Configuracao.DiasParaEmissaoDeCTeComplementar,
                            empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao,
                            empresa.Configuracao.PrazoCancelamentoCTe,
                            empresa.Configuracao.PrazoCancelamentoMDFe,
                            empresa.Configuracao.ProdutoPredominante,
                            empresa.Configuracao.OutrasCaracteristicas,
                            empresa.Configuracao.TipoImpressao,
                            empresa.Configuracao.TokenIntegracaoCTe,
                            CNPJTransportadorComoCNPJSeguradora = empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora != null && empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora == Dominio.Enumeradores.OpcaoSimNao.Sim,
                            NumeroApoliceComoNumeroAverbacao = empresa.Configuracao.NumeroApoliceComoNumeroAverbacao != null && empresa.Configuracao.NumeroApoliceComoNumeroAverbacao == Dominio.Enumeradores.OpcaoSimNao.Sim,
                            VersaoCTe = !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoCTe) ? empresa.Configuracao.VersaoCTe : string.Empty,
                            VersaoMDFe = !string.IsNullOrWhiteSpace(empresa.Configuracao.VersaoMDFe) ? empresa.Configuracao.VersaoMDFe : string.Empty,
                            empresa.SerieCTeFora,
                            empresa.SerieCTeDentro,
                            empresa.SerieMDFe,
                            empresa.NomeFantasia,
                            ValorLimiteFrete = empresa.Configuracao.ValorLimiteFrete.ToString("n2"),
                            empresa.Configuracao.AssinaturaEmail,
                            empresa.Configuracao.NFSeKeyENotas,
                            empresa.Configuracao.NFSeURLENotas,

                            SeguradoraCNPJ = empresa.Configuracao.CNPJSeguro,
                            SeguradoraNome = empresa.Configuracao.NomeSeguro,
                            SeguradoraNApolice = empresa.Configuracao.NumeroApoliceSeguro,
                            SeguradoraNAverbacao = empresa.Configuracao.AverbacaoSeguro,
                            ResponsavelSeguro = empresa.Configuracao.ResponsavelSeguro,

                            AverbaAutomaticoATM = empresa.Configuracao.AverbaAutomaticoATM,
                            CodigoATM = empresa.Configuracao.CodigoSeguroATM,
                            UsuarioATM = empresa.Configuracao.UsuarioSeguroATM,
                            SenhaATM = empresa.Configuracao.SenhaSeguroATM,
                            SeguradoraAverbacao = empresa.Configuracao.SeguradoraAverbacao != null ? empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ELT ? "E" : empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido ? "" : "" : "",
                            TokenAverbacao = empresa.Configuracao.TokenAverbacaoBradesco ?? string.Empty,
                            AverbarComoEmbarcador = empresa.Configuracao.AverbarComoEmbarcador,

                            IntegrarAutomaticamenteValePedagio = empresa.Configuracao.ValePedagioIntegraAutomatico,
                            IntegradoraValePedagio = (int)empresa.Configuracao.ValePedagioIntegradora,
                            UrlIntegracaoRest = empresa.Configuracao.URLIntegracaoRest,
                            UsuarioValePedagio = empresa.Configuracao.ValePedagioUsuario,
                            SenhaValePedagio = empresa.Configuracao.ValePedagioSenha,
                            TokenValePedagio = empresa.Configuracao.ValePedagioToken,
                            FornecedorValePedagio = empresa.Configuracao.ValePedagioFornecedor,
                            ResponsavelValePedagio = empresa.Configuracao.ValePedagioResponsavel,
                            TipoPesoNFe = empresa.Configuracao.TipoPesoNFe == Dominio.Enumeradores.TipoPesoNFe.Bruto ? 0 : 1,
                            empresa.Configuracao.BloquearEmissaoMDFeWS,
                            empresa.Configuracao.GerarNFSeImportacoes,
                            empresa.Configuracao.AverbarMDFe,
                            empresa.Configuracao.AverbarNFSe,
                            empresa.Configuracao.TrafegusURL,
                            empresa.Configuracao.TrafegusUsuario,
                            empresa.Configuracao.TrafegusSenha,
                            empresa.Configuracao.BuonnyURL,
                            empresa.Configuracao.BuonnyToken,
                            empresa.Configuracao.BuonnyGerenciadora,
                            empresa.Configuracao.BuonnyCodigoTipoProduto,
                            IntegradoraSM = empresa.Configuracao.IntegradoraSM.HasValue ? empresa.Configuracao.IntegradoraSM.Value.ToString("d") : string.Empty
                        };

                        return Json(retorno, true);
                    }
                    else
                    {
                        var retorno = new
                        {
                            DiasParaEmissaoDeCTeAnulacao = 30,
                            DiasParaEmissaoDeCTeComplementar = 90,
                            DiasParaEmissaoDeCTeSubstituicao = 30,
                            PrazoCancelamentoCTe = 0,
                            PrazoCancelamentoMDFe = 0,
                            ProdutoPredominante = "DIVERSOS",
                            OutrasCaracteristicas = String.Empty,
                            TipoImpressao = Dominio.Enumeradores.TipoImpressao.Retrato,
                            TokenIntegracaoCTe = string.Empty,
                            VersaoCTe = "2.00",
                            VersaoMDFe = "1.00",
                            empresa.NomeFantasia,
                            ValorLimiteFrete = "0,00",
                            CNPJTransportadorComoCNPJSeguradora = false,
                            NumeroApoliceComoNumeroAverbacao = false,
                            AssinaturaEmail = string.Empty,
                            NFSeKeyENotas = string.Empty,
                            NFSeURLENotas = string.Empty,

                            AverbaAutomaticoATM = 0,
                            CodigoATM = string.Empty,
                            UsuarioATM = string.Empty,
                            SenhaATM = string.Empty,
                            SeguradoraAverbacao = string.Empty,
                            TokenAverbacao = string.Empty,

                            SeguradoraCNPJ = string.Empty,
                            SeguradoraNome = string.Empty,
                            SeguradoraNApolice = string.Empty,
                            SeguradoraNAverbacao = string.Empty,
                            ResponsavelSeguro = Dominio.Enumeradores.TipoSeguro.Emitente_CTE,
                            AverbarComoEmbarcador = false,

                            IntegrarAutomaticamenteValePedagio = 0,
                            IntegradoraValePedagio = Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma,
                            UsuarioValePedagio = string.Empty,
                            SenhaValePedagio = string.Empty,
                            TokenValePedagio = string.Empty,
                            FornecedorValePedagio = string.Empty,
                            ResponsavelValePedagio = string.Empty,
                            TipoPesoNFe = 0,
                            BloquearEmissaoMDFeWS = false,
                            GerarNFSeImportacoes = false,
                            AverbarMDFe = false,
                            AverbarNFSe = false
                        };

                        return Json(retorno, true);
                    }

                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações para a empresa.");
            }
        }

        [AcceptVerbs("POST")]
        [ValidateInput(false)]
        public ActionResult Salvar()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigo = int.Parse(Servicos.Criptografia.Descriptografar(HttpUtility.UrlDecode(Request.Params["Codigo"]), "CT3##MULT1@#$S0FTW4R3"));
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa;

                if (this.EmpresaUsuario.Codigo != codigo)
                    empresa = repEmpresa.BuscarPorCodigo(codigo);
                else
                    empresa = this.EmpresaUsuario;

                if (empresa != null)
                {
                    int codigoAtividade, diasParaEntrega, diasEmissaoCTeSubstituicao, diasEmissaoCTeComplementar, diasEmissaoCTeAnulacao,
                        codigoPlanoAbastecimento, prazoCancelamentoCTe, prazoCancelamentoMDFe, codigoPlanoCTe, diasParaVencimentoDuplicatas, numeroParcelasDuplicatas, codigoSerieIntraestadual,
                        codigoSerieInterestadual, codigoSerieComplementar, codigoSerieMDFe, codigoApoliceSeguro, codigoSerieNFSe, codigoPlanoPagamentoMotorista, codigoServicoNFSe, codigoServicoNFSeFora,
                        codigoNaturezaNFSe, codigoNaturezaForaNFSe, averbaAutomaticoATM, naturaLayoutEDI, naturaLayoutEDIOcoren, primeiroNumeroMDFe, diasParaAvisoVencimentos, nfseIntegracaoENotas = 0;

                    int.TryParse(Request.Params["CodigoAtividade"], out codigoAtividade);
                    int.TryParse(Request.Params["DiasParaEntrega"], out diasParaEntrega);
                    int.TryParse(Request.Params["DiasParaEmissaoCTeComplementar"], out diasEmissaoCTeComplementar);
                    int.TryParse(Request.Params["DiasParaEmissaoCTeAnulacao"], out diasEmissaoCTeAnulacao);
                    int.TryParse(Request.Params["DiasParaEmissaoCTeSubstituicao"], out diasEmissaoCTeSubstituicao);
                    int.TryParse(Request.Params["PrazoCancelamentoCTe"], out prazoCancelamentoCTe);
                    int.TryParse(Request.Params["PrazoCancelamentoMDFe"], out prazoCancelamentoMDFe);
                    int.TryParse(Request.Params["CodigoPlanoAbastecimento"], out codigoPlanoAbastecimento);
                    int.TryParse(Request.Params["CodigoPlanoCTe"], out codigoPlanoCTe);
                    int.TryParse(Request.Params["CodigoPlanoPagamentoMotorista"], out codigoPlanoPagamentoMotorista);
                    int.TryParse(Request.Params["DiasParaVencimentoDasDuplicatas"], out diasParaVencimentoDuplicatas);
                    int.TryParse(Request.Params["DiasParaAvisoVencimentos"], out diasParaAvisoVencimentos);
                    int.TryParse(Request.Params["NumeroDeParcelasDasDuplicatas"], out numeroParcelasDuplicatas);
                    int.TryParse(Request.Params["CodigoSerieIntraestadual"], out codigoSerieIntraestadual);
                    int.TryParse(Request.Params["CodigoSerieInterestadual"], out codigoSerieInterestadual);
                    int.TryParse(Request.Params["CodigoSerieComplementar"], out codigoSerieComplementar);
                    int.TryParse(Request.Params["CodigoSerieMDFe"], out codigoSerieMDFe);
                    int.TryParse(Request.Params["CodigoApoliceSeguro"], out codigoApoliceSeguro);
                    int.TryParse(Request.Params["CodigoSerieNFSe"], out codigoSerieNFSe);
                    int.TryParse(Request.Params["CodigoServicoNFSe"], out codigoServicoNFSe);
                    int.TryParse(Request.Params["CodigoServicoNFSeFora"], out codigoServicoNFSeFora);
                    int.TryParse(Request.Params["CodigoNaturezaNFSe"], out codigoNaturezaNFSe);
                    int.TryParse(Request.Params["CodigoNaturezaForaNFSe"], out codigoNaturezaForaNFSe);
                    int.TryParse(Request.Params["AverbaAutomaticoATM"], out averbaAutomaticoATM);
                    int.TryParse(Request.Params["PrimeiroNumeroMDFe"], out primeiroNumeroMDFe);
                    int.TryParse(Request.Params["NaturaLayoutEDI"], out naturaLayoutEDI);
                    int.TryParse(Request.Params["NaturaLayoutEDIOcoren"], out naturaLayoutEDIOcoren);
                    int.TryParse(Request.Params["NFSeIntegracaoENotas"], out nfseIntegracaoENotas);
                    int.TryParse(Request.Params["TamanhoTagObservacaoNFe"], out int tamanhoTagObservacaoNFe);

                    decimal percentualImpostoSimplesNacional;
                    decimal.TryParse(Request.Params["PercentualImpostoSimplesNacional"], out percentualImpostoSimplesNacional);

                    string obsAvancadaProprio = Request.Params["ObservacaoCTeAvancadaProprio"];
                    string obsAvancadaTerceiros = Request.Params["ObservacaoCTeAvancadaTerceiros"];
                    string obsNormal = Request.Params["ObservacaoCTeNormal"];
                    string obsComplementar = Request.Params["ObservacaoCTeComplementar"];
                    string obsAnulacao = Request.Params["ObservacaoCTeAnulacao"];
                    string obsSubstituicao = Request.Params["ObservacaoCTeSubstituicao"];
                    string obsCTeSimplesNacional = Request.Params["ObservacaoCTeSimplesNacional"];
                    string dicasEmissaoCTe = Request.Params["DicasEmissaoCTe"];
                    string produtoPredominante = Request.Params["ProdutoPredominante"];
                    string outrasCaracteristicas = Request.Params["OutrasCaracteristicas"];
                    string codigoATM = Request.Params["CodigoATM"];
                    string usuarioATM = Request.Params["UsuarioATM"];
                    string senhaATM = Request.Params["SenhaATM"];
                    string chaveCriptograficaSigaFacil = Request.Params["ChaveCriptograficaSigaFacil"];
                    string codigoContratanteSigaFacil = Request.Params["CodigoContratanteSigaFacil"];
                    string codigoIntegradorEFrete = Request.Params["CodigoIntegradorEFrete"];
                    string senhaEFrete = Request.Params["SenhaEFrete"];
                    string usuarioEFrete = Request.Params["UsuarioEFrete"];

                    string truckPadURL = Request.Params["TruckPadURL"];
                    string truckPadUser = Request.Params["TruckPadUser"];
                    string truckPadPassword = Request.Params["TruckPadPassword"];

                    string cpfNFSe = Request.Params["NFSeCPF"];
                    string senhaNFSe = Request.Params["SenhaNFSe"];
                    string fraseSecretaNFSe = Request.Params["FraseSecretaNFSe"];
                    string serieRPSNFSe = Request.Params["SerieRPSNFSe"];
                    string nomePDFCTe = Request.Params["NomePDFCTe"];

                    string tokenIntegracaoCTe = Request.Params["TokenIntegracaoCTe"];
                    string tokenIntegracaoEnvioCTe = Request.Params["TokenIntegracaoEnvioCTe"];
                    string wsIntegracaoCTe = Request.Params["WsIntegracaoCTe"];
                    string urlPrefeituraNFSe = Request.Params["URLPrefeituraNFSe"];
                    string loginSitePrefeituraNFSe = Request.Params["LoginSitePrefeituraNFSe"];
                    string senhaSitePrefeituraNFSe = Request.Params["SenhaSitePrefeituraNFSe"];
                    string observacaoIntegracaoNFSe = Request.Params["ObservacaoIntegracaoNFSe"];
                    string observacaoPadraoNFSe = Request.Params["ObservacaoPadraoNFSe"] ?? string.Empty;
                    string assinaturaEmail = Request.Params["AssinaturaEmail"];

                    string naturaMatriz = Request.Params["NaturaMatriz"];
                    string naturaFilial = Request.Params["NaturaFilial"];
                    string naturaUsuario = Request.Params["NaturaUsuario"];
                    string naturaSenha = Request.Params["NaturaSenha"];

                    string ftpNaturaHost = Request.Params["FTPNaturaHost"];
                    string ftpNaturaPorta = Request.Params["FTPNaturaPorta"];
                    string ftpNaturaUsuario = Request.Params["FTPNaturaUsuario"];
                    string ftpNaturaSenha = Request.Params["FTPNaturaSenha"];
                    string ftpNaturaDiretorio = Request.Params["FTPNaturaDiretorio"];
                    string modeloPadrao = Request.Params["ModeloPadrao"];
                    string nfseIDENotas = Request.Params["NFSeIDENotas"];
                    string versaoCTe = Request.Params["VersaoCTe"];

                    string seguradoraAverbacao = Request.Params["SeguradoraAverbacao"];
                    string tokenBradesco = Request.Params["TokenAverbacao"];
                    string wsdlQuorum = Request.Params["WsdlQuorum"];

                    string tagParaImportarObservacaoNFe = Request.Params["TagParaImportarObservacaoNFe"];
                    string descricaoMedidaKgCTe = Request.Params["DescricaoMedidaKgCTe"];
                    string cnpjMatrizCIOT = Request.Params["CNPJMatrizCIOT"];

                    string trafegusURL = Request.Params["TrafegusURL"];
                    string trafegusUsuario = Request.Params["TrafegusUsuario"];
                    string trafegusSenha = Request.Params["TrafegusSenha"];

                    string buonnyURL = Request.Params["BuonnyURL"];
                    string buonnyToken = Request.Params["BuonnyToken"];
                    string buonnyGerenciadora = Request.Params["BuonnyGerenciadora"];
                    string buonnyCodigoTipoProduto = Request.Params["BuonnyCodigoTipoProduto"];

                    string urlMercadoLivre = Request.Params["URLMercadoLivre"];
                    string idMercadoLivre = Request.Params["IDMercadoLivre"];
                    string secretKeyMercadoLivre = Request.Params["SecretKeyMercadoLivre"];

                    string wsIntegracaoEnvioCTeEmbarcadorTMS = Request.Params["WsIntegracaoEnvioCTeEmbarcadorTMS"];
                    string wsIntegracaoEnvioNFSeEmbarcadorTMS = Request.Params["WsIntegracaoEnvioNFSeEmbarcadorTMS"];
                    string wsIntegracaoEnvioMDFeEmbarcadorTMS = Request.Params["WsIntegracaoEnvioMDFeEmbarcadorTMS"];
                    string tokenIntegracaoEmbarcadorTMS = Request.Params["TokenIntegracaoEmbarcadorTMS"];

                    bool aguardarAverbacaoCTeParaEmitirMDFe, naoUtilizarDadosSeguroEmpresaPai, indicadorDeLotacao, emitirSemValorDaCarga, utilizaTabeladeFrete, icmsIsento, emissaoGratuitaEFrete, bloquearNaoEquiparadoEFrete,
                         permiteVincularMesmaPlacaOutrosVeiculos, atualizaVeiculoImpXMLCTe,
                         ftpNaturaPassivo, ftpNaturaSeguro, utilizaNovaImportacaoEDI, emailSemTexto = false;
                    bool.TryParse(Request.Params["IndicadorDeLotacao"], out indicadorDeLotacao);
                    bool.TryParse(Request.Params["EmitirSemValorDaCarga"], out emitirSemValorDaCarga);
                    bool.TryParse(Request.Params["UtilizaTabelaFrete"], out utilizaTabeladeFrete);
                    bool.TryParse(Request.Params["PermiteVincularMesmaPlacaOutrosVeiculos"], out permiteVincularMesmaPlacaOutrosVeiculos);
                    bool.TryParse(Request.Params["NaoCopiarImpostosCTeAnterior"], out bool naoCopiarImpostosCTeAnterior);
                    bool.TryParse(Request.Params["ICMSIsento"], out icmsIsento);
                    bool.TryParse(Request.Params["EmissaoGratuitaEFrete"], out emissaoGratuitaEFrete);
                    bool.TryParse(Request.Params["BloquearNaoEquiparadoEFrete"], out bloquearNaoEquiparadoEFrete);
                    bool.TryParse(Request.Params["AtualizaVeiculoImpXMLCTe"], out atualizaVeiculoImpXMLCTe);
                    bool.TryParse(Request.Params["FTPNaturaPassivo"], out ftpNaturaPassivo);
                    bool.TryParse(Request.Params["BloquearDuplicidadeCTeAcerto"], out bool bloquearDuplicidadeCTeAcerto);
                    bool.TryParse(Request.Params["FTPNaturaSeguro"], out ftpNaturaSeguro);
                    bool.TryParse(Request.Params["UtilizaNovaImportacaoEDI"], out utilizaNovaImportacaoEDI);
                    bool.TryParse(Request.Params["EmailSemTexto"], out emailSemTexto);
                    bool.TryParse(Request.Params["NaoUtilizarDadosSeguroEmpresaPai"], out naoUtilizarDadosSeguroEmpresaPai);
                    bool.TryParse(Request.Params["AguardarAverbacaoCTeParaEmitirMDFe"], out aguardarAverbacaoCTeParaEmitirMDFe);
                    bool.TryParse(Request.Params["CadastrarItemDocumentoEntrada"], out bool cadastrarItemDocumentoEntrada);
                    bool.TryParse(Request.Params["PermiteSelecionarCTeOutroTomador"], out bool permiteSelecionarCTeOutroTomador);
                    bool.TryParse(Request.Params["BloquearEmissaoMDFeWS"], out bool bloquearEmissaoMDFeWS);
                    bool.TryParse(Request.Params["GerarNFSeImportacoes"], out bool gerarNFSeImportacoes);
                    bool.TryParse(Request.Params["FormatarPlacaComHifenNaObservacao"], out bool formatarPlacaComHifenNaObservacao);
                    bool.TryParse(Request.Params["ImportacaoNaoRateiaPedagio"], out bool importacaoNaoRateiaPedagio);
                    bool.TryParse(Request.Params["GerarCTeIntegracaoDocumentosMunicipais"], out bool gerarCTeIntegracaoDocumentosMunicipais);
                    bool.TryParse(Request.Params["EmiteNFSeForaEmbarcador"], out bool emiteNFSeForaEmbarcador);
                    bool.TryParse(Request.Params["NaturaEnviaOcorrenciaEntreguePadrao"], out bool naturaEnviaOcorrenciaEntreguePadrao);
                    bool.TryParse(Request.Params["AverbarMDFe"], out bool averbarMDFe);
                    bool.TryParse(Request.Params["NaoCalcularDIFALCTeOS"], out bool naoCalcularDIFALCTeOS);
                    bool.TryParse(Request.Params["NaoPermitirDuplciataMesmoDocumento"], out bool naoPermitirDuplciataMesmoDocumento);
                    bool.TryParse(Request.Params["NaoCopiarSeguroCTeAnterior"], out bool naoCopiarSeguroCTeAnterior);
                    bool.TryParse(Request.Params["BloquearEmissaoCTeCargaMunicipal"], out bool bloquearEmissaoCTeCargaMunicipal);
                    bool.TryParse(Request.Params["BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca"], out bool bloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca);
                    bool.TryParse(Request.Params["BloquearEmissaoCTeComUFDestinosDiferentes"], out bool bloquearEmissaoCTeComUFDestinosDiferentes);
                    bool.TryParse(Request.Params["AverbarNFSe"], out bool averbarNFSe);
                    bool.TryParse(Request.Params["DescontarINSSValorReceber"], out bool descontarINSSValorReceber);
                    bool.TryParse(Request.Params["CopiarObservacaoFiscoContribuinteCTeAnterior"], out bool copiarObservacaoFiscoContribuinteCTeAnterior);
                    bool.TryParse(Request.Params["NaoCopiarValoresCTeAnterior"], out bool naoCopiarValoresCTeAnterior);
                    bool.TryParse(Request.Params["AdicionarResponsavelSeguroObsContribuinte"], out bool adicionarResponsavelSeguroObsContribuinte);
                    bool.TryParse(Request.Params["PermiteImportarXMLNFSe"], out bool permiteImportarXMLNFSe);
                    bool.TryParse(Request.Params["NaoImportarValoresImportacaoCTe"], out bool naoImportarValoresImportacaoCTe);
                    bool.TryParse(Request.Params["ExigirObservacaoContribuinteValorContainer"], out bool exigirObservacaoContribuinteValorContainer);
                    bool.TryParse(Request.Params["UsarRegraICMSParaCteDeSubcontratacao"], out bool usarRegraICMSParaCteDeSubcontratacao);
                    bool.TryParse(Request.Params["NaoImportarNotaDuplicadaEDINovaImportacao"], out bool naoImportarNotaDuplicadaEDINovaImportacao);
                    bool.TryParse(Request.Params["NaoSmarCreditoICMSNoValorDaPrestacao"], out bool naoSmarCreditoICMSNoValorDaPrestacao);                    

                    bool.TryParse(Request.Params["ExibirHomeVencimentoCertificado"], out bool exibirHomeVencimentoCertificado);
                    bool.TryParse(Request.Params["ExibirHomePendenciasEntrega"], out bool exibirHomePendenciasEntrega);
                    bool.TryParse(Request.Params["ExibirHomeGraficosEmissoes"], out bool exibirHomeGraficosEmissoes);
                    bool.TryParse(Request.Params["ExibirHomeServicosVeiculos"], out bool exibirHomeServicosVeiculos);
                    bool.TryParse(Request.Params["ExibirHomeParcelaDuplicatas"], out bool exibirHomeParcelaDuplicatas);
                    bool.TryParse(Request.Params["ExibirHomePagamentosMotoristas"], out bool exibirHomePagamentosMotoristas);
                    bool.TryParse(Request.Params["ExibirHomeAcertoViagem"], out bool exibirHomeAcertoViagem);
                    bool.TryParse(Request.Params["ExibirHomeMDFesPendenteEncerramento"], out bool exibirHomeMDFesPendenteEncerramento);
                    bool.TryParse(Request.Params["ArmazenaNotasParaGerarPorPeriodo"], out bool armazenaNotasParaGerarPorPeriodo);                    
                    bool.TryParse(Request.Params["NFSeNacional"], out bool nfseNacional);                    

                    string seguradoraCNPJ = Utilidades.String.OnlyNumbers(Request.Params["SeguradoraCNPJ"] ?? string.Empty);
                    string seguradoraNome = Request.Params["SeguradoraNome"];
                    string seguradoraNApolice = Request.Params["SeguradoraNApolice"];
                    string seguradoraNAverbacao = Request.Params["SeguradoraNAverbacao"];

                    decimal valorLimiteFrete = 0;
                    decimal.TryParse(Request.Params["ValorLimiteFrete"], out valorLimiteFrete);

                    Dominio.Enumeradores.TipoSeguro? responsavelSeguro = null;
                    Dominio.Enumeradores.TipoSeguro responsavelSeguroAux;
                    if (Enum.TryParse(Request.Params["ResponsavelSeguro"], out responsavelSeguroAux))
                        responsavelSeguro = responsavelSeguroAux;

                    Dominio.Enumeradores.TipoImpressao tipoImpressao;
                    Enum.TryParse<Dominio.Enumeradores.TipoImpressao>(Request.Params["TipoImpressao"], out tipoImpressao);

                    Dominio.Enumeradores.OpcaoSimNao geraDuplicatasAutomaticamente;
                    Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["GeraDuplicatasAutomaticamente"], out geraDuplicatasAutomaticamente);

                    Dominio.Enumeradores.TipoGeracaoCTeWS tipoGeracaoCTeWS;
                    Enum.TryParse<Dominio.Enumeradores.TipoGeracaoCTeWS>(Request.Params["TipoGeracaoCTeWS"], out tipoGeracaoCTeWS);

                    Dominio.Enumeradores.OpcaoSimNao incluirICMS;
                    Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["IncluirICMS"], out incluirICMS);

                    Dominio.Enumeradores.PerfilEmpresa perfil;
                    Enum.TryParse<Dominio.Enumeradores.PerfilEmpresa>(Request.Params["Perfil"], out perfil);

                    Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado criterioEscrituracaoEApuracao;
                    Enum.TryParse<Dominio.Enumeradores.IndicadorDoCriterioDeEscrituracaoEApuracaoAdotado>(Request.Params["CriterioEscrituracaoEApuracao"], out criterioEscrituracaoEApuracao);

                    Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo incidenciaTributariaNoPeriodo;
                    Enum.TryParse<Dominio.Enumeradores.IndicadorDaIncidenciaTributariaNoPeriodo>(Request.Params["IncidenciaTributariaNoPeriodo"], out incidenciaTributariaNoPeriodo);

                    Dominio.Enumeradores.TipoPesoNFe tipoPesoNFe;
                    Enum.TryParse<Dominio.Enumeradores.TipoPesoNFe>(Request.Params["TipoPesoNFe"], out tipoPesoNFe);

                    Dominio.Enumeradores.TipoCTEAverbacao tipoCTeAverbacao;
                    Enum.TryParse<Dominio.Enumeradores.TipoCTEAverbacao>(Request.Params["TipoCTeAverbacao"], out tipoCTeAverbacao);

                    Dominio.Entidades.Empresa empresaMatrizCIOT = null;
                    if (!string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(cnpjMatrizCIOT)))
                    {
                        if (!Utilidades.Validate.ValidarCNPJ(Utilidades.String.OnlyNumbers(cnpjMatrizCIOT)))
                            return Json<bool>(false, false, "CNPJ Matriz CIOT inválido.");
                        empresaMatrizCIOT = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(cnpjMatrizCIOT));
                        if (empresaMatrizCIOT == null)
                            return Json<bool>(false, false, "Não existe uma transportadora cadastrada para o CNPJ Matriz CIOT informado.");
                    }

                    if (empresa.Configuracao == null)
                    {
                        empresa.Configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();
                        if (this.Permissao() == null || this.Permissao().PermissaoDeInclusao != "A")
                            return Json<bool>(false, false, "Permissão para inclusão das configurações da empresa negada.");
                    }
                    else
                    {
                        if (this.Permissao() == null || this.Permissao().PermissaoDeAlteracao != "A")
                            return Json<bool>(false, false, "Permissão para alteração das configurações da empresa negada!");

                        empresa.Configuracao.Initialize();
                    }

                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);
                    Repositorio.Atividade repAtividade = new Repositorio.Atividade(unidadeDeTrabalho);
                    Repositorio.PlanoDeConta repPlanoConta = new Repositorio.PlanoDeConta(unidadeDeTrabalho);
                    Repositorio.ApoliceDeSeguro repApoliceSeguro = new Repositorio.ApoliceDeSeguro(unidadeDeTrabalho);
                    Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unidadeDeTrabalho);
                    Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);
                    Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);

                    empresa.Configuracao.Atividade = repAtividade.BuscarPorCodigo(codigoAtividade);
                    empresa.Configuracao.ObservacaoCTeAvancadaProprio = obsAvancadaProprio;
                    empresa.Configuracao.ObservacaoCTeAvancadaTerceiros = obsAvancadaTerceiros;
                    empresa.Configuracao.ObservacaoCTeNormal = obsNormal;
                    empresa.Configuracao.ObservacaoCTeAnulacao = obsAnulacao;
                    empresa.Configuracao.ObservacaoCTeComplementar = obsComplementar;
                    empresa.Configuracao.ObservacaoCTeSubstituicao = obsSubstituicao;
                    empresa.Configuracao.DiasParaEntrega = diasParaEntrega;
                    empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao = diasEmissaoCTeAnulacao;
                    empresa.Configuracao.DiasParaEmissaoDeCTeComplementar = diasEmissaoCTeComplementar;
                    empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao = diasEmissaoCTeSubstituicao;
                    empresa.Configuracao.PrazoCancelamentoCTe = prazoCancelamentoCTe;
                    empresa.Configuracao.PrazoCancelamentoMDFe = prazoCancelamentoMDFe;
                    empresa.Configuracao.IndicadorDeLotacao = indicadorDeLotacao;
                    empresa.Configuracao.EmitirSemValorDaCarga = emitirSemValorDaCarga;

                    if (empresa.Configuracao.DicasEmissaoCTe != dicasEmissaoCTe)
                        SalvarLogDicas(empresa, dicasEmissaoCTe, unidadeDeTrabalho);

                    empresa.Configuracao.DicasEmissaoCTe = dicasEmissaoCTe;
                    empresa.Configuracao.ProdutoPredominante = produtoPredominante;
                    empresa.Configuracao.OutrasCaracteristicas = outrasCaracteristicas;
                    empresa.Configuracao.TipoImpressao = tipoImpressao;
                    empresa.Configuracao.PlanoAbastecimento = codigoPlanoAbastecimento > 0 ? repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoAbastecimento) : null;
                    empresa.Configuracao.PlanoCTe = codigoPlanoCTe > 0 ? repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoCTe) : null;
                    empresa.Configuracao.PlanoPagamentoMotorista = codigoPlanoPagamentoMotorista > 0 ? repPlanoConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoPlanoPagamentoMotorista) : null;
                    empresa.Configuracao.UtilizaTabelaDeFrete = utilizaTabeladeFrete;
                    empresa.Configuracao.PermiteVincularMesmaPlacaOutrosVeiculos = permiteVincularMesmaPlacaOutrosVeiculos;
                    empresa.Configuracao.NaoCopiarImpostosCTeAnterior = naoCopiarImpostosCTeAnterior;
                    empresa.Configuracao.GeraDuplicatasAutomaticamente = geraDuplicatasAutomaticamente;
                    empresa.Configuracao.DiasParaVencimentoDasDuplicatas = diasParaVencimentoDuplicatas;
                    empresa.Configuracao.DiasParaAvisoVencimentos = diasParaAvisoVencimentos;
                    empresa.Configuracao.NumeroDeParcelasDasDuplicatas = numeroParcelasDuplicatas;
                    empresa.Configuracao.SerieInterestadual = codigoSerieInterestadual > 0 ? repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerieInterestadual) : null;
                    empresa.Configuracao.SerieIntraestadual = codigoSerieIntraestadual > 0 ? repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerieIntraestadual) : null;
                    empresa.Configuracao.SerieCTeComplementar = codigoSerieComplementar > 0 ? repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerieComplementar) : null;
                    empresa.Configuracao.SerieMDFe = codigoSerieMDFe > 0 ? repSerie.BuscarPorCodigo(this.EmpresaUsuario.Codigo, codigoSerieMDFe) : null;
                    empresa.Configuracao.ICMSIsento = icmsIsento;
                    empresa.Configuracao.TipoGeracaoCTeWS = tipoGeracaoCTeWS;
                    empresa.Configuracao.IncluirICMSNoFrete = incluirICMS;
                    empresa.Configuracao.Perfil = perfil;
                    empresa.Configuracao.CriterioEscrituracaoEApuracao = criterioEscrituracaoEApuracao;
                    empresa.Configuracao.IncidenciaTributariaNoPeriodo = incidenciaTributariaNoPeriodo;
                    empresa.Configuracao.ApoliceSeguro = repApoliceSeguro.BuscarPorCodigo(codigoApoliceSeguro, empresa.Codigo);
                    empresa.Configuracao.PercentualImpostoSimplesNacional = percentualImpostoSimplesNacional;
                    empresa.Configuracao.CodigoSeguroATM = codigoATM;
                    empresa.Configuracao.UsuarioSeguroATM = usuarioATM;
                    empresa.Configuracao.SenhaSeguroATM = senhaATM;
                    empresa.Configuracao.AverbaAutomaticoATM = averbaAutomaticoATM;
                    empresa.Configuracao.CodigoContratanteSigaFacil = codigoContratanteSigaFacil;
                    empresa.Configuracao.ChaveCriptograficaSigaFacil = chaveCriptograficaSigaFacil;
                    empresa.Configuracao.CodigoIntegradorEFrete = codigoIntegradorEFrete;
                    empresa.Configuracao.SenhaEFrete = senhaEFrete;
                    empresa.Configuracao.UsuarioEFrete = usuarioEFrete;
                    empresa.Configuracao.EmissaoGratuitaEFrete = emissaoGratuitaEFrete;
                    empresa.Configuracao.BloquearNaoEquiparadoEFrete = bloquearNaoEquiparadoEFrete;
                    empresa.Configuracao.TruckPadURL = truckPadURL;
                    empresa.Configuracao.TruckPadUser = truckPadUser;
                    empresa.Configuracao.TruckPadPassword = truckPadPassword;
                    empresa.Configuracao.FraseSecretaNFSe = fraseSecretaNFSe;
                    empresa.Configuracao.NFSeCPF = cpfNFSe;
                    empresa.Configuracao.SenhaNFSe = senhaNFSe;
                    empresa.Configuracao.SerieNFSe = codigoSerieNFSe > 0 ? repSerie.BuscarPorCodigo(codigoSerieNFSe) : null;
                    empresa.Configuracao.SerieRPSNFSe = serieRPSNFSe;
                    empresa.Configuracao.ServicoNFSe = codigoServicoNFSe > 0 ? repServicoNFSe.BuscarPorCodigo(codigoServicoNFSe) : null;
                    empresa.Configuracao.ServicoNFSeFora = codigoServicoNFSeFora > 0 ? repServicoNFSe.BuscarPorCodigo(codigoServicoNFSeFora) : null;
                    empresa.Configuracao.NaturezaNFSe = codigoNaturezaNFSe > 0 ? repNaturezaNFSe.BuscarPorCodigo(codigoNaturezaNFSe) : null;
                    empresa.Configuracao.NaturezaNFSeFora = codigoNaturezaForaNFSe > 0 ? repNaturezaNFSe.BuscarPorCodigo(codigoNaturezaForaNFSe) : null;
                    empresa.Configuracao.ObservacaoCTeSimplesNacional = obsCTeSimplesNacional;
                    empresa.Configuracao.AtualizaVeiculoImpXMLCTe = atualizaVeiculoImpXMLCTe;

                    empresa.Configuracao.TokenIntegracaoCTe = tokenIntegracaoCTe;
                    empresa.Configuracao.TokenIntegracaoEnvioCTe = tokenIntegracaoEnvioCTe;
                    empresa.Configuracao.WsIntegracaoEnvioCTe = wsIntegracaoCTe;
                    empresa.Configuracao.URLPrefeituraNFSe = urlPrefeituraNFSe;
                    empresa.Configuracao.LoginSitePrefeituraNFSe = loginSitePrefeituraNFSe;
                    empresa.Configuracao.SenhaSitePrefeituraNFSe = senhaSitePrefeituraNFSe;
                    empresa.Configuracao.ObservacaoIntegracaoNFSe = observacaoIntegracaoNFSe;
                    empresa.Configuracao.ObservacaoPadraoNFSe = observacaoPadraoNFSe.Trim();
                    empresa.Configuracao.AssinaturaEmail = assinaturaEmail;

                    empresa.Configuracao.CodigoMatrizNatura = naturaMatriz;
                    empresa.Configuracao.CodigoFilialNatura = naturaFilial;
                    empresa.Configuracao.UsuarioNatura = naturaUsuario;
                    empresa.Configuracao.SenhaNatura = naturaSenha;
                    empresa.Configuracao.LayoutEDINatura = repLayoutEDI.BuscarPorCodigo(naturaLayoutEDI);

                    empresa.Configuracao.FTPNaturaHost = ftpNaturaHost;
                    empresa.Configuracao.FTPNaturaPorta = ftpNaturaPorta;
                    empresa.Configuracao.FTPNaturaUsuario = ftpNaturaUsuario;
                    empresa.Configuracao.FTPNaturaSenha = ftpNaturaSenha;
                    empresa.Configuracao.FTPNaturaDiretorio = ftpNaturaDiretorio;
                    empresa.Configuracao.FTPNaturaPassivo = ftpNaturaPassivo;
                    empresa.Configuracao.BloquearDuplicidadeCTeAcerto = bloquearDuplicidadeCTeAcerto;
                    empresa.Configuracao.FTPNaturaSeguro = ftpNaturaSeguro;
                    empresa.Configuracao.LayoutEDIOcoren = repLayoutEDI.BuscarPorCodigo(naturaLayoutEDIOcoren);
                    empresa.Configuracao.UtilizaNovaImportacaoEDI = utilizaNovaImportacaoEDI;
                    empresa.Configuracao.ModeloPadrao = modeloPadrao;
                    empresa.Configuracao.NFSeIntegracaoENotas = nfseIntegracaoENotas == 0 ? false : true;
                    empresa.Configuracao.EmiteNFSeForaEmbarcador = emiteNFSeForaEmbarcador;
                    empresa.NFSeIDENotas = nfseIDENotas;

                    empresa.Configuracao.CNPJSeguro = seguradoraCNPJ;
                    empresa.Configuracao.NomeSeguro = seguradoraNome;
                    empresa.Configuracao.NumeroApoliceSeguro = seguradoraNApolice;
                    empresa.Configuracao.AverbacaoSeguro = seguradoraNAverbacao;
                    empresa.Configuracao.ResponsavelSeguro = responsavelSeguro;
                    empresa.Configuracao.NaoUtilizarDadosSeguroEmpresaPai = naoUtilizarDadosSeguroEmpresaPai;
                    empresa.Configuracao.AguardarAverbacaoCTeParaEmitirMDFe = aguardarAverbacaoCTeParaEmitirMDFe;
                    empresa.Configuracao.CadastrarItemDocumentoEntrada = cadastrarItemDocumentoEntrada;
                    empresa.Configuracao.PermiteSelecionarCTeOutroTomador = permiteSelecionarCTeOutroTomador;
                    empresa.Configuracao.TipoPesoNFe = tipoPesoNFe;
                    empresa.Configuracao.BloquearEmissaoMDFeWS = bloquearEmissaoMDFeWS;
                    empresa.Configuracao.GerarNFSeImportacoes = gerarNFSeImportacoes;
                    empresa.Configuracao.FormatarPlacaComHifenNaObservacao = formatarPlacaComHifenNaObservacao;
                    empresa.Configuracao.TipoCTeAverbacao = tipoCTeAverbacao;
                    empresa.Configuracao.ImportacaoNaoRateiaPedagio = importacaoNaoRateiaPedagio;
                    empresa.Configuracao.TagParaImportarObservacaoNFe = tagParaImportarObservacaoNFe;
                    empresa.Configuracao.TamanhoTagObservacaoNFe = tamanhoTagObservacaoNFe;
                    empresa.Configuracao.GerarCTeIntegracaoDocumentosMunicipais = gerarCTeIntegracaoDocumentosMunicipais;
                    empresa.Configuracao.DescricaoMedidaKgCTe = descricaoMedidaKgCTe;
                    empresa.Configuracao.NaturaEnviaOcorrenciaEntreguePadrao = naturaEnviaOcorrenciaEntreguePadrao;
                    empresa.Configuracao.AverbarMDFe = averbarMDFe;
                    empresa.Configuracao.NaoCalcularDIFALCTeOS = naoCalcularDIFALCTeOS;
                    empresa.Configuracao.NaoPermitirDuplciataMesmoDocumento = naoPermitirDuplciataMesmoDocumento;
                    empresa.Configuracao.ExibirHomeVencimentoCertificado = exibirHomeVencimentoCertificado;
                    empresa.Configuracao.ExibirHomePendenciasEntrega = exibirHomePendenciasEntrega;
                    empresa.Configuracao.ExibirHomeGraficosEmissoes = exibirHomeGraficosEmissoes;
                    empresa.Configuracao.ExibirHomeServicosVeiculos = exibirHomeServicosVeiculos;
                    empresa.Configuracao.ExibirHomeParcelaDuplicatas = exibirHomeParcelaDuplicatas;
                    empresa.Configuracao.ExibirHomePagamentosMotoristas = exibirHomePagamentosMotoristas;
                    empresa.Configuracao.ExibirHomeAcertoViagem = exibirHomeAcertoViagem;
                    empresa.Configuracao.ExibirHomeMDFesPendenteEncerramento = exibirHomeMDFesPendenteEncerramento;
                    empresa.Configuracao.NaoCopiarSeguroCTeAnterior = naoCopiarSeguroCTeAnterior;
                    empresa.Configuracao.BloquearEmissaoCTeParaCargaMunicipal = bloquearEmissaoCTeCargaMunicipal;
                    empresa.Configuracao.BloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca = bloquearEmissaoMDFeComMDFeAutorizadoParaMesmaPlaca;
                    empresa.Configuracao.BloquearEmissaoCTeComUFDestinosDiferentes = bloquearEmissaoCTeComUFDestinosDiferentes;
                    empresa.Configuracao.ValorLimiteFrete = valorLimiteFrete;
                    empresa.Configuracao.AverbarNFSe = averbarNFSe;
                    empresa.Configuracao.DescontarINSSValorReceber = descontarINSSValorReceber;
                    empresa.Configuracao.CopiarObservacaoFiscoContribuinteCTeAnterior = copiarObservacaoFiscoContribuinteCTeAnterior;
                    empresa.Configuracao.NaoCopiarValoresCTeAnterior = naoCopiarValoresCTeAnterior;
                    empresa.Configuracao.AdicionarResponsavelSeguroObsContribuinte = adicionarResponsavelSeguroObsContribuinte;
                    empresa.Configuracao.EmpresaMatrizCIOT = empresaMatrizCIOT;
                    empresa.Configuracao.ArmazenaNotasParaGerarPorPeriodo = armazenaNotasParaGerarPorPeriodo;
                    empresa.Configuracao.PermiteImportarXMLNFSe = permiteImportarXMLNFSe;
                    empresa.Configuracao.NaoImportarValoresImportacaoCTe = naoImportarValoresImportacaoCTe;
                    empresa.Configuracao.ExigirObservacaoContribuinteValorContainer = exigirObservacaoContribuinteValorContainer;
                    empresa.Configuracao.UsarRegraICMSParaCteDeSubcontratacao = usarRegraICMSParaCteDeSubcontratacao;
                    empresa.Configuracao.NaoImportarNotaDuplicadaEDINovaImportacao = naoImportarNotaDuplicadaEDINovaImportacao;
                    empresa.Configuracao.NaoSmarCreditoICMSNoValorDaPrestacao = naoSmarCreditoICMSNoValorDaPrestacao;

                    empresa.Configuracao.NumeroApoliceComoNumeroAverbacao = null;
                    empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora = null;

                    empresa.Configuracao.SeguradoraAverbacao = seguradoraAverbacao == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM : seguradoraAverbacao == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum : seguradoraAverbacao == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro : seguradoraAverbacao == "E" ? Dominio.Enumeradores.IntegradoraAverbacao.ELT : Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                    empresa.Configuracao.TokenAverbacaoBradesco = tokenBradesco;
                    empresa.Configuracao.WsdlAverbacaoQuorum = wsdlQuorum;

                    empresa.Configuracao.TrafegusURL = trafegusURL;
                    empresa.Configuracao.TrafegusUsuario = trafegusUsuario;
                    empresa.Configuracao.TrafegusSenha = trafegusSenha;

                    empresa.Configuracao.BuonnyURL = buonnyURL;
                    empresa.Configuracao.BuonnyToken = buonnyToken;
                    empresa.Configuracao.BuonnyGerenciadora = buonnyGerenciadora;
                    empresa.Configuracao.BuonnyCodigoTipoProduto = buonnyCodigoTipoProduto;

                    Dominio.Enumeradores.IntegradoraSM integradoraSM;
                    if (Enum.TryParse(Request.Params["IntegradoraSM"], out integradoraSM))
                        empresa.Configuracao.IntegradoraSM = integradoraSM;
                    else
                        empresa.Configuracao.TipoIntegradoraCIOT = null;

                    empresa.Configuracao.URLMercadoLivre = urlMercadoLivre;
                    empresa.Configuracao.IDMercadoLivre = idMercadoLivre;
                    empresa.Configuracao.SecretKeyMercadoLivre = secretKeyMercadoLivre;

                    empresa.Configuracao.WsIntegracaoEnvioCTeEmbarcadorTMS = wsIntegracaoEnvioCTeEmbarcadorTMS;
                    empresa.Configuracao.WsIntegracaoEnvioNFSeEmbarcadorTMS = wsIntegracaoEnvioNFSeEmbarcadorTMS;
                    empresa.Configuracao.WsIntegracaoEnvioMDFeEmbarcadorTMS = wsIntegracaoEnvioMDFeEmbarcadorTMS;
                    empresa.Configuracao.TokenIntegracaoEmbarcadorTMS = tokenIntegracaoEmbarcadorTMS;

                    empresa.Configuracao.EmailSemTexto = emailSemTexto;

                    empresa.Configuracao.NomePDFCTe = nomePDFCTe;

                    empresa.NFSeNacional = nfseNacional;

                    Dominio.Enumeradores.TipoCOFINS cstCOFINS;

                    if (Enum.TryParse<Dominio.Enumeradores.TipoCOFINS>(Request.Params["CSTCOFINS"], out cstCOFINS))
                        empresa.Configuracao.CSTCOFINS = cstCOFINS;
                    else
                        empresa.Configuracao.CSTCOFINS = null;

                    Dominio.Enumeradores.TipoPIS cstPIS;

                    if (Enum.TryParse<Dominio.Enumeradores.TipoPIS>(Request.Params["CSTPIS"], out cstPIS))
                        empresa.Configuracao.CSTPIS = cstPIS;
                    else
                        empresa.Configuracao.CSTPIS = null;

                    decimal aliquotaPIS, aliquotaCOFINS, aliquotaIR, aliquotaINSS, aliquotaCSLL, percentualBaseINSS;

                    if (decimal.TryParse(Request.Params["AliquotaPIS"], out aliquotaPIS))
                        empresa.Configuracao.AliquotaPIS = aliquotaPIS;
                    else
                        empresa.Configuracao.AliquotaPIS = null;

                    if (decimal.TryParse(Request.Params["AliquotaCOFINS"], out aliquotaCOFINS))
                        empresa.Configuracao.AliquotaCOFINS = aliquotaCOFINS;
                    else
                        empresa.Configuracao.AliquotaCOFINS = null;

                    decimal.TryParse(Request.Params["AliquotaIR"], out aliquotaIR);
                    empresa.Configuracao.AliquotaIR = aliquotaIR;

                    decimal.TryParse(Request.Params["AliquotaINSS"], out aliquotaINSS);
                    empresa.Configuracao.AliquotaINSS = aliquotaINSS;

                    decimal.TryParse(Request.Params["AliquotaCSLL"], out aliquotaCSLL);
                    empresa.Configuracao.AliquotaCSLL = aliquotaCSLL;

                    decimal.TryParse(Request.Params["PercentualBaseINSS"], out percentualBaseINSS);
                    empresa.Configuracao.PercentualBaseINSS = percentualBaseINSS;

                    Dominio.Enumeradores.TipoEmpresaCIOT tipoEmpresaCIOT;
                    Enum.TryParse<Dominio.Enumeradores.TipoEmpresaCIOT>(Request.Params["TipoEmpresaCIOT"], out tipoEmpresaCIOT);
                    empresa.Configuracao.TipoEmpresaCIOT = tipoEmpresaCIOT;

                    Dominio.ObjetosDeValor.Enumerador.TipoIntegradoraCIOT tipoIntegradoraCIOT;

                    if (Enum.TryParse(Request.Params["TipoIntegradoraCIOT"], out tipoIntegradoraCIOT))
                        empresa.Configuracao.TipoIntegradoraCIOT = tipoIntegradoraCIOT;
                    else
                        empresa.Configuracao.TipoIntegradoraCIOT = null;

                    Dominio.ObjetosDeValor.Enumerador.TipoPagamentoCIOT tipoPagamentoCIOT;

                    if (Enum.TryParse(Request.Params["TipoPagamentoCIOT"], out tipoPagamentoCIOT))
                        empresa.Configuracao.TipoPagamentoCIOT = tipoPagamentoCIOT;
                    else
                        empresa.Configuracao.TipoPagamentoCIOT = null;

                    if (!string.IsNullOrWhiteSpace(versaoCTe))
                        empresa.Configuracao.VersaoCTe = versaoCTe;

                    empresa.PrimeiroNumeroMDFe = primeiroNumeroMDFe;
                    SalvarMovimentoFinanceiro(ref empresa, unidadeDeTrabalho);

                    if (empresa.Configuracao.Codigo > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.Configuracao.GetChanges(), "Alterou as Configurações.", unidadeDeTrabalho);

                    repEmpresa.Atualizar(empresa);

                    SalvarSeriesPorUF(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarEstadosBloqueados(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarSeriesCliente(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarAverbacoes(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarAverbacaoSerie(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarConfiguracoesFTP(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarCNPJCPFAutorizados(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarServicoNFsePorCidade(empresa.Configuracao, unidadeDeTrabalho);
                    SalvarCodigosServicosNFse(empresa.Configuracao, unidadeDeTrabalho);

                    Servicos.AtualizacaoEmpresa svcAtualizaEmpresa = new Servicos.AtualizacaoEmpresa(unidadeDeTrabalho);
                    string retorno = svcAtualizaEmpresa.Atualizar(empresa, unidadeDeTrabalho);

                    string mensagemErro = string.Empty;
                    Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech integracaoNStech = new Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento.IntegracaoNSTech(unidadeDeTrabalho);
                    if (!integracaoNStech.IncluirAtualizarCertificado(out mensagemErro, empresa, unidadeDeTrabalho))
                        Servicos.Log.TratarErro($"Ocorreu um erro ao enviar o certificado para o emissor NStech: {mensagemErro}");

                    if (empresa.Configuracao != null && empresa.Configuracao.NFSeIntegracaoENotas)
                    {
                        Servicos.NFSeENotas svcNFSeENotas = new Servicos.NFSeENotas(unidadeDeTrabalho);
                        var retornoEmpresaNFse = svcNFSeENotas.SalvarEmpresa(empresa.Codigo, unidadeDeTrabalho);
                        if (retornoEmpresaNFse != null && !string.IsNullOrWhiteSpace(retornoEmpresaNFse.Result))
                            return Json<bool>(false, false, retornoEmpresaNFse.Result);
                    }

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar as configurações da empresa.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        [ValidateInput(false)]
        public ActionResult SalvarEmpresaAdmin()
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Dominio.Entidades.Empresa empresa;

                if (this.EmpresaUsuario == null)
                    return Json<bool>(false, false, "Empresa admin não encontradas.");
                else
                    empresa = repEmpresa.BuscarPorCodigo(this.EmpresaUsuario.Codigo);

                if (empresa != null)
                {
                    if (empresa.Configuracao == null)
                        empresa.Configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();
                    else
                        empresa.Configuracao.Initialize();

                    int diasEmissaoCTeSubstituicao, diasEmissaoCTeComplementar, diasEmissaoCTeAnulacao, prazoCancelamentoCTe, prazoCancelamentoMDFe, serieMDFe, serieCTeFora, serieCTeDentro,
                        averbaAutomaticoATM = 0;

                    int.TryParse(Request.Params["DiasParaEmissaoDeCTeComplementar"], out diasEmissaoCTeComplementar);
                    int.TryParse(Request.Params["DiasParaEmissaoDeCTeAnulacao"], out diasEmissaoCTeAnulacao);
                    int.TryParse(Request.Params["DiasParaEmissaoDeCTeSubstituicao"], out diasEmissaoCTeSubstituicao);
                    int.TryParse(Request.Params["PrazoCancelamentoCTe"], out prazoCancelamentoCTe);
                    int.TryParse(Request.Params["PrazoCancelamentoMDFe"], out prazoCancelamentoMDFe);
                    int.TryParse(Request.Params["SerieMDFe"], out serieMDFe);
                    int.TryParse(Request.Params["SerieCTeFora"], out serieCTeFora);
                    int.TryParse(Request.Params["SerieCTeDentro"], out serieCTeDentro);
                    int.TryParse(Request.Params["AverbaAutomaticoATM"], out averbaAutomaticoATM);

                    bool.TryParse(Request.Params["AverbarComoEmbarcador"], out bool averbarComoEmbarcador);
                    bool.TryParse(Request.Params["AverbarMDFe"], out bool averbarMDFe);

                    decimal valorLimiteFrete = 0;
                    decimal.TryParse(Request.Params["ValorLimiteFrete"], out valorLimiteFrete);

                    string produtoPredominante = Request.Params["ProdutoPredominante"];
                    string outrasCaracteristicas = Request.Params["OutrasCaracteristicas"];
                    string versaoCTe = Request.Params["VersaoCTe"];
                    string versaoMDFe = Request.Params["VersaoMDFe"];
                    string tokenIntegracaoCTe = Request.Params["TokenIntegracaoCTe"];
                    string assinaturaEmail = Request.Params["AssinaturaEmail"];

                    string nfseKeyENotas = Request.Params["NFSeKeyENotas"];
                    string nfseURLENotas = Request.Params["NFSeURLENotas"];

                    string codigoATM = Request.Params["CodigoATM"];
                    string usuarioATM = Request.Params["UsuarioATM"];
                    string senhaATM = Request.Params["SenhaATM"];
                    string seguradoraAverbacao = Request.Params["SeguradoraAverbacao"];
                    string tokenBradesco = Request.Params["TokenAverbacao"];
                    string wsdlQuorum = Request.Params["WsdlQuorum"];

                    string seguradoraCNPJ = Utilidades.String.OnlyNumbers(Request.Params["SeguradoraCNPJ"] ?? string.Empty);
                    string seguradoraNome = Request.Params["SeguradoraNome"];
                    string seguradoraNApolice = Request.Params["SeguradoraNApolice"];
                    string seguradoraNAverbacao = Request.Params["SeguradoraNAverbacao"];

                    string trafegusURL = Request.Params["TrafegusURL"];
                    string trafegusUsuario = Request.Params["TrafegusUsuario"];
                    string trafegusSenha = Request.Params["TrafegusSenha"];

                    Dominio.Enumeradores.TipoSeguro? responsavelSeguro = null;
                    Dominio.Enumeradores.TipoSeguro responsavelSeguroAux;
                    if (Enum.TryParse(Request.Params["ResponsavelSeguro"], out responsavelSeguroAux))
                        responsavelSeguro = responsavelSeguroAux;

                    Dominio.Enumeradores.TipoImpressao tipoImpressao;
                    Enum.TryParse<Dominio.Enumeradores.TipoImpressao>(Request.Params["TipoImpressao"], out tipoImpressao);

                    Dominio.Enumeradores.OpcaoSimNao CNPJTransportadorComoCNPJSeguradora;
                    Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["CNPJTransportadorComoCNPJSeguradora"], out CNPJTransportadorComoCNPJSeguradora);

                    Dominio.Enumeradores.OpcaoSimNao numeroApoliceComoNumeroAverbacao;
                    Enum.TryParse<Dominio.Enumeradores.OpcaoSimNao>(Request.Params["NumeroApoliceComoNumeroAverbacao"], out numeroApoliceComoNumeroAverbacao);

                    int.TryParse(Request.Params["IntegrarAutomaticamenteValePedagio"], out int integrarAutomaticamenteValePedagio);
                    Dominio.Enumeradores.IntegradoraValePedagio integradoraValePedagio = Dominio.Enumeradores.IntegradoraValePedagio.Nenhuma;
                    Dominio.Enumeradores.IntegradoraValePedagio integradoraValePedagioAux;
                    if (Enum.TryParse(Request.Params["IntegradoraValePedagio"], out integradoraValePedagioAux))
                        integradoraValePedagio = integradoraValePedagioAux;
                    string usuarioValePedagio = Request.Params["UsuarioValePedagio"];
                    string senhaValePedagio = Request.Params["SenhaValePedagio"];
                    string tokenValePedagio = Request.Params["TokenValePedagio"];
                    string fornecedorValePedagio = Request.Params["FornecedorValePedagio"];
                    string responsavelValePedagio = Request.Params["ResponsavelValePedagio"];
                    string urlIntegracaoRest = Request.Params["UrlIntegracaoRest"];

                    empresa.Configuracao.DiasParaEmissaoDeCTeAnulacao = diasEmissaoCTeAnulacao;
                    empresa.Configuracao.DiasParaEmissaoDeCTeComplementar = diasEmissaoCTeComplementar;
                    empresa.Configuracao.DiasParaEmissaoDeCTeSubstituicao = diasEmissaoCTeSubstituicao;
                    empresa.Configuracao.PrazoCancelamentoCTe = prazoCancelamentoCTe;
                    empresa.Configuracao.PrazoCancelamentoMDFe = prazoCancelamentoMDFe;
                    empresa.Configuracao.ProdutoPredominante = produtoPredominante;
                    empresa.Configuracao.OutrasCaracteristicas = outrasCaracteristicas;
                    empresa.Configuracao.TipoImpressao = tipoImpressao;
                    empresa.Configuracao.VersaoCTe = versaoCTe;
                    empresa.Configuracao.VersaoMDFe = versaoMDFe;
                    empresa.Configuracao.TokenIntegracaoCTe = !string.IsNullOrWhiteSpace(tokenIntegracaoCTe) ? tokenIntegracaoCTe : null;
                    empresa.Configuracao.ValorLimiteFrete = valorLimiteFrete;
                    empresa.Configuracao.AssinaturaEmail = assinaturaEmail;
                    empresa.Configuracao.CNPJTransportadorComoCNPJSeguradora = CNPJTransportadorComoCNPJSeguradora;
                    empresa.Configuracao.NumeroApoliceComoNumeroAverbacao = numeroApoliceComoNumeroAverbacao;
                    empresa.SerieMDFe = serieMDFe;
                    empresa.SerieCTeFora = serieCTeFora;
                    empresa.SerieCTeDentro = serieCTeDentro;
                    empresa.Configuracao.NFSeKeyENotas = nfseKeyENotas;
                    empresa.Configuracao.NFSeURLENotas = nfseURLENotas;

                    empresa.Configuracao.CNPJSeguro = seguradoraCNPJ;
                    empresa.Configuracao.NomeSeguro = seguradoraNome;
                    empresa.Configuracao.NumeroApoliceSeguro = seguradoraNApolice;
                    empresa.Configuracao.AverbacaoSeguro = seguradoraNAverbacao;
                    empresa.Configuracao.ResponsavelSeguro = responsavelSeguro;

                    empresa.Configuracao.AverbaAutomaticoATM = averbaAutomaticoATM;
                    empresa.Configuracao.UsuarioSeguroATM = usuarioATM;
                    empresa.Configuracao.SenhaSeguroATM = senhaATM;
                    empresa.Configuracao.CodigoSeguroATM = codigoATM;
                    empresa.Configuracao.SeguradoraAverbacao = seguradoraAverbacao == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM : seguradoraAverbacao == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum : seguradoraAverbacao == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro : seguradoraAverbacao == "E" ? Dominio.Enumeradores.IntegradoraAverbacao.ELT : Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                    empresa.Configuracao.TokenAverbacaoBradesco = tokenBradesco;
                    empresa.Configuracao.WsdlAverbacaoQuorum = wsdlQuorum;
                    empresa.Configuracao.AverbarComoEmbarcador = averbarComoEmbarcador;
                    empresa.Configuracao.AverbarMDFe = averbarMDFe;

                    empresa.Configuracao.ValePedagioIntegraAutomatico = integrarAutomaticamenteValePedagio;
                    empresa.Configuracao.ValePedagioIntegradora = integradoraValePedagio;
                    empresa.Configuracao.ValePedagioUsuario = usuarioValePedagio;
                    empresa.Configuracao.ValePedagioSenha = senhaValePedagio;
                    empresa.Configuracao.ValePedagioToken = tokenValePedagio;
                    empresa.Configuracao.ValePedagioFornecedor = fornecedorValePedagio;
                    empresa.Configuracao.ValePedagioResponsavel = responsavelValePedagio;
                    empresa.Configuracao.URLIntegracaoRest = urlIntegracaoRest;

                    empresa.Configuracao.TrafegusURL = trafegusURL;
                    empresa.Configuracao.TrafegusUsuario = trafegusUsuario;
                    empresa.Configuracao.TrafegusSenha = trafegusSenha;

                    if (empresa.Configuracao.Codigo > 0)
                        Servicos.Auditoria.Auditoria.Auditar(Auditado, empresa, empresa.Configuracao.GetChanges(), "Alterou as Configurações.", unidadeDeTrabalho);

                    repEmpresa.Atualizar(empresa);

                    return Json<bool>(true, true);
                }
                else
                {
                    return Json<bool>(false, false, "Empresa admin não encontrada.");
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao salvar as configurações da empresa admin.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult Buscar()
        {
            try
            {
                Dominio.Entidades.ConfiguracaoEmpresa configuracao;

                if (this.EmpresaUsuario.Configuracao != null)
                    configuracao = this.EmpresaUsuario.Configuracao;
                else if (this.EmpresaUsuario.EmpresaPai != null && this.EmpresaUsuario.EmpresaPai.Configuracao != null)
                    configuracao = this.EmpresaUsuario.EmpresaPai.Configuracao;
                else
                    configuracao = new Dominio.Entidades.ConfiguracaoEmpresa();

                var retorno = new
                {
                    CodigoAtividade = configuracao.Atividade != null ? configuracao.Atividade.Codigo : 0,
                    DescricaoAtividade = configuracao.Atividade != null ? configuracao.Atividade.Descricao : "",
                    configuracao.ObservacaoCTeNormal,
                    configuracao.ObservacaoCTeAnulacao,
                    configuracao.ObservacaoCTeComplementar,
                    configuracao.ObservacaoCTeSubstituicao,
                    configuracao.IndicadorDeLotacao,
                    configuracao.EmitirSemValorDaCarga,
                    configuracao.DicasEmissaoCTe,
                    configuracao.ProdutoPredominante,
                    ResponsavelSeguro = configuracao.ResponsavelSeguro != null ? configuracao.ResponsavelSeguro : null,
                    CodigoApoliceSeguro = configuracao.ApoliceSeguro != null ? configuracao.ApoliceSeguro.Codigo : 0,
                    NomeSeguradora = configuracao.ApoliceSeguro != null ? configuracao.ApoliceSeguro.NomeSeguradora : string.Empty,
                    NumeroApoliceSeguro = configuracao.ApoliceSeguro != null ? configuracao.ApoliceSeguro.NumeroApolice : string.Empty,
                    configuracao.TipoImpressao,
                    CodigoContaAbastecimento = configuracao.PlanoAbastecimento != null ? configuracao.PlanoAbastecimento.Codigo : 0,
                    DescricaoContaAbastecimento = configuracao.PlanoAbastecimento != null ? string.Concat(configuracao.PlanoAbastecimento.Conta, " - ", configuracao.PlanoAbastecimento.Descricao) : string.Empty,
                    CodigoContaCTe = configuracao.PlanoCTe != null ? configuracao.PlanoCTe.Codigo : 0,
                    DescricaoContaCTe = configuracao.PlanoCTe != null ? string.Concat(configuracao.PlanoCTe.Conta, " - ", configuracao.PlanoCTe.Descricao) : string.Empty,
                    configuracao.UtilizaTabelaDeFrete,
                    configuracao.PermiteVincularMesmaPlacaOutrosVeiculos,
                    configuracao.NaoCopiarImpostosCTeAnterior,
                    configuracao.GeraDuplicatasAutomaticamente,
                    configuracao.DiasParaVencimentoDasDuplicatas,
                    configuracao.DiasParaAvisoVencimentos,
                    configuracao.NumeroDeParcelasDasDuplicatas,
                    configuracao.ICMSIsento,
                    TipoGeracaoCTeWS = configuracao.TipoGeracaoCTeWS.ToString("d"),
                    IncluirICMS = configuracao.IncluirICMSNoFrete.ToString("d"),
                    PercentualImpostoSimplesNacional = (configuracao.PercentualImpostoSimplesNacional.HasValue ? configuracao.PercentualImpostoSimplesNacional.Value : 0m).ToString("n2"),
                    this.EmpresaUsuario.OptanteSimplesNacional,
                    configuracao.EmailSemTexto,
                    configuracao.NaoCopiarSeguroCTeAnterior,
                    configuracao.CopiarObservacaoFiscoContribuinteCTeAnterior,
                    configuracao.NaoCopiarValoresCTeAnterior,
                    configuracao.AdicionarResponsavelSeguroObsContribuinte
                };

                return Json(retorno, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações para a empresa.");
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarLogDicas()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoEmpresa;
                int.TryParse(Utilidades.String.OnlyNumbers(Request.Params["CodigoEmpresa"]), out codigoEmpresa);

                Repositorio.EmpresaLogDicas repEmpresaLogDicas = new Repositorio.EmpresaLogDicas(unitOfWork);

                List<Dominio.Entidades.EmpresaLogDicas> listaEmpresaLogDicas = repEmpresaLogDicas.BuscarPorEmpresa(codigoEmpresa);

                int countDocumentos = listaEmpresaLogDicas.Count();

                var retorno = (from log in listaEmpresaLogDicas
                               select new
                               {
                                   log.Codigo,
                                   Data = log.Data.ToString("dd/MM/yyyy HH:mm"),
                                   Usuario = log.Funcionario.Nome
                               }).ToList();

                return Json(retorno, true, null, new string[] { "Codigo", "Data|20", "Usuario|60" }, countDocumentos);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar Log das Dicas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult BuscarDicaDeLog()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                int codigoDica;
                int.TryParse(Request.Params["Codigo"], out codigoDica);

                Repositorio.EmpresaLogDicas repEmpresaLogDicas = new Repositorio.EmpresaLogDicas(unitOfWork);
                Dominio.Entidades.EmpresaLogDicas LogDica = repEmpresaLogDicas.BuscarPorCodigo(codigoDica);

                if (LogDica == null)
                    return Json<bool>(false, false, "Ocorreu uma falha ao consultar Log das Dicas.");

                return Json(LogDica.DicasEmissaoCTe, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao consultar Log das Dicas.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        [AcceptVerbs("POST")]
        public ActionResult InserirArquivosDicas()
        {
            Repositorio.UnitOfWork unidadeDeTarabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.ConfiguracaoArquivosDica repArquivosDica = new Repositorio.ConfiguracaoArquivosDica(unidadeDeTarabalho);
                Dominio.Entidades.ConfiguracaoArquivosDica arquivo = new Dominio.Entidades.ConfiguracaoArquivosDica();

                string[] extensoesValidas = { ".jpg", ".png", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".txt" };

                if (Request.Files.Count == 0)
                    return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");

                // Converte arquivo upado
                System.Web.HttpPostedFileBase file = Request.Files[0];

                // Valida
                string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!extensoesValidas.Contains(extensao))
                    return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");


                // Cria Entidade e insere
                arquivo.Configuracao = this.EmpresaUsuario.Configuracao;
                arquivo.Funcionario = this.Usuario;
                arquivo.DataHora = DateTime.Now;
                arquivo.NomeArquivo = file.FileName;
                arquivo.CaminhoArquivo = "";

                if (arquivo.NomeArquivo.Length > 45)
                {
                    string auxNome = arquivo.NomeArquivo.Replace(extensao, "");
                    auxNome = auxNome.Substring(0, (auxNome.Length - extensao.Length)) + extensao;
                    arquivo.NomeArquivo = auxNome;
                }

                repArquivosDica.Inserir(arquivo);

                // Salva na pasta configurada
                string caminho = Utilidades.IO.FileStorageService.Storage.Combine(System.Configuration.ConfigurationManager.AppSettings["CaminhoArquivsoDicas"]);
                string arquivoFisico = arquivo.Codigo.ToString() + extensao;
                arquivoFisico = Utilidades.IO.FileStorageService.Storage.Combine(caminho, arquivoFisico);

                Utilidades.IO.FileStorageService.Storage.SaveStream(arquivoFisico, file.InputStream);

                arquivo.CaminhoArquivo = arquivoFisico;
                repArquivosDica.Atualizar(arquivo);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, arquivo.Funcionario.Empresa, null, "Inseriu o arquivo " + arquivo.NomeArquivo + ".", unidadeDeTarabalho);

                // Retorna informacoes
                return Json(new
                {
                    Codigo = arquivo.Codigo,
                    Nome = arquivo.NomeArquivo,
                }, true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ExcluirArquivoDicas()
        {
            Repositorio.UnitOfWork unidadeDeTarabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ConfiguracaoArquivosDica repArquivosDica = new Repositorio.ConfiguracaoArquivosDica(unidadeDeTarabalho);
                Dominio.Entidades.ConfiguracaoArquivosDica arquivo = repArquivosDica.BuscarPorCodigo(this.EmpresaUsuario.Configuracao.Codigo, codigo);

                if (arquivo == null)
                    return Json<bool>(false, false, "Não foi possível encontrar o arquivo.");

                // Deleta o arquivo fisico
                Utilidades.IO.FileStorageService.Storage.Delete(arquivo.CaminhoArquivo);

                // Deleta registro
                Servicos.Auditoria.Auditoria.Auditar(Auditado, arquivo.Funcionario.Empresa, null, "Excluiu o arquivo " + arquivo.NomeArquivo + ".", unidadeDeTarabalho);
                repArquivosDica.Deletar(arquivo);

                // Retorna sucesso
                return Json("Arquivo excluído com sucesso.", true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao excluir o arquivo.");
            }

        }

        [AcceptVerbs("POST", "GET")]
        public ActionResult DownloadArquivoDicas()
        {
            Repositorio.UnitOfWork unidadeDeTarabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                int codigo;
                int.TryParse(Request.Params["Codigo"], out codigo);

                Repositorio.ConfiguracaoArquivosDica repArquivosDica = new Repositorio.ConfiguracaoArquivosDica(unidadeDeTarabalho);
                Dominio.Entidades.ConfiguracaoArquivosDica arquivo = repArquivosDica.BuscarPorCodigo(this.EmpresaUsuario.Configuracao.Codigo, codigo);

                if (arquivo == null)
                    return Json<bool>(false, false, "Não foi possível encontrar o arquivo.");

                if (!Utilidades.IO.FileStorageService.Storage.Exists(arquivo.CaminhoArquivo))
                    return Json<bool>(false, false, "O arquivo não existe.");

                return Arquivo(Utilidades.IO.FileStorageService.Storage.ReadAllBytes(arquivo.CaminhoArquivo), MimeMapping.GetMimeMapping(arquivo.CaminhoArquivo), arquivo.NomeArquivo);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return Json<bool>(false, false, "Ocorreu uma falha ao inserir o arquivo.");
            }

        }

        [AcceptVerbs("POST")]
        public ActionResult ImportarArquivoPreNFS()
        {
            string[] extensoesValidas = { ".xml", ".XML" };

            // Valida quantidade aruivos
            if (Request.Files.Count != 1)
                return Json<bool>(false, false, "Nenhum arquivo recebido.");

            // Converte arquivo upado
            HttpPostedFileBase file = Request.Files[0];

            // Valida extensao
            string extensao = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!extensoesValidas.Contains(extensao))
                return Json<bool>(false, false, "Extensão " + extensao.Substring(1) + " inválida.");

            string nomeArquivo = Path.GetFileName(Request.Files[0].FileName).ToUpper();
            string caminhoTemporario = Path.GetTempPath();
            if (Utilidades.IO.FileStorageService.Storage.Exists(caminhoTemporario + nomeArquivo))
                Utilidades.IO.FileStorageService.Storage.Delete(caminhoTemporario + nomeArquivo);

            file.SaveAs(caminhoTemporario + nomeArquivo);

            // Converte arquivo em json
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.NFSePreNFSItupeva repNFSePreNFSItupeva = new Repositorio.NFSePreNFSItupeva(unidadeDeTrabalho);
                unidadeDeTrabalho.Start();
                string vFile = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoTemporario + nomeArquivo);

                string contents = vFile.Replace("\n", "").Replace("\t", "");
                Dominio.ObjetosDeValor.Pre_NFS_E_XML raizXML;


                raizXML = Servicos.XML.ConvertXMLStringToObject<Dominio.ObjetosDeValor.Pre_NFS_E_XML>(contents);

                for (int i = 0; i < raizXML.Detalhes_Pre_NFS_e.Count(); i++)
                {
                    if (!repNFSePreNFSItupeva.RegistroJaLancado(raizXML.Detalhes_Pre_NFS_e[i].Pre_NFS_E_Det_Cod_Validacao.Trim(), raizXML.cabecalho.Cab_Bloco_Pre_NFS_e.Trim(), this.EmpresaUsuario.Codigo))
                    {
                        Dominio.Entidades.NFSePreNFSItupeva preNFS = new Dominio.Entidades.NFSePreNFSItupeva();
                        preNFS.CNPJ = raizXML.cabecalho.Cab_CNPJ.Trim();
                        preNFS.Empresa = this.EmpresaUsuario;
                        preNFS.NumeroBloco = raizXML.cabecalho.Cab_Bloco_Pre_NFS_e.Trim();
                        preNFS.NumeroSequencia = raizXML.Detalhes_Pre_NFS_e[i].Pre_NFS_E_Det_Seq.Trim();
                        preNFS.NumeroValidacao = raizXML.Detalhes_Pre_NFS_e[i].Pre_NFS_E_Det_Cod_Validacao.Trim();

                        repNFSePreNFSItupeva.Inserir(preNFS);
                    }
                }

                unidadeDeTrabalho.CommitChanges();
                Utilidades.IO.FileStorageService.Storage.Delete(caminhoTemporario + nomeArquivo);

                return Json("Importação feita com sucesso.", true);
            }
            catch (Exception e)
            {
                unidadeDeTrabalho.Rollback();
                Servicos.Log.TratarErro(e);
                return Json<bool>(false, false, "Houve um erro ao converter arquivo.");
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Métodos Privados

        private object ObterPermissoesDaEmpresa(int codigoEmpresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PermissaoEmpresa repPermissaoEmpresa = new Repositorio.PermissaoEmpresa(unitOfWork);
            List<Dominio.Entidades.PermissaoEmpresa> permissoesEmpresa = repPermissaoEmpresa.BuscarPorEmpresa(codigoEmpresa);
            var retorno = from obj in permissoesEmpresa
                          select new
                          {
                              Codigo = obj.Pagina.Codigo,
                              Acesso = obj.PermissaoDeAcesso == "A" ? true : false,
                              Incluir = obj.PermissaoDeInclusao == "A" ? true : false,
                              Alterar = obj.PermissaoDeAlteracao == "A" ? true : false,
                              Excluir = obj.PermissaoDeDelecao == "A" ? true : false
                          };
            return retorno;
        }

        private void SalvarSeriesPorUF(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["SeriesPorUF"]))
            {
                List<Dominio.ObjetosDeValor.EstadosDeEmissaoSerie> seriesPorUF = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.EstadosDeEmissaoSerie>>(Request.Params["SeriesPorUF"]);

                if (seriesPorUF != null)
                {
                    Repositorio.EstadosDeEmissaoSerie repEstadosDeEmissaoSerie = new Repositorio.EstadosDeEmissaoSerie(unidadeDeTrabalho);
                    Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);
                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                    List<Dominio.Entidades.EstadosDeEmissaoSerie> seriesPorUFExcluir = repEstadosDeEmissaoSerie.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < seriesPorUFExcluir.Count; i++)
                        repEstadosDeEmissaoSerie.Deletar(seriesPorUFExcluir[i]);

                    for (var i = 0; i < seriesPorUF.Count; i++)
                    {
                        if (!seriesPorUF[i].Excluir)
                        {
                            var estadoDeEmissaoSerie = new Dominio.Entidades.EstadosDeEmissaoSerie();

                            estadoDeEmissaoSerie.Codigo = 0;
                            estadoDeEmissaoSerie.ConfiguracaoEmpresa = configuracaoEmpresa;
                            estadoDeEmissaoSerie.Estado = repEstado.BuscarPorSigla(seriesPorUF[i].SiglaUF);
                            estadoDeEmissaoSerie.Serie = repSerie.BuscarPorCodigo(seriesPorUF[i].CodigoSerie);

                            repEstadosDeEmissaoSerie.Inserir(estadoDeEmissaoSerie);
                        }
                    }
                }
            }
        }

        private void SalvarEstadosBloqueados(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["EstadosBloqueados"]))
            {
                List<Dominio.ObjetosDeValor.EstadosBloqueados> estadosBloqueados = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.EstadosBloqueados>>(Request.Params["EstadosBloqueados"]);

                if (estadosBloqueados != null)
                {
                    Repositorio.EstadosBloqueadosEmissao repEstadosBloqueadosEmissao = new Repositorio.EstadosBloqueadosEmissao(unidadeDeTrabalho);
                    Repositorio.Estado repEstado = new Repositorio.Estado(unidadeDeTrabalho);

                    List<Dominio.Entidades.EstadosBloqueadosEmissao> estadosBloqueadosEmissao = repEstadosBloqueadosEmissao.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < estadosBloqueadosEmissao.Count; i++)
                        repEstadosBloqueadosEmissao.Deletar(estadosBloqueadosEmissao[i]);

                    for (var i = 0; i < estadosBloqueados.Count; i++)
                    {
                        if (!estadosBloqueados[i].Excluir)
                        {
                            var estadoBloqueadoEmissao = new Dominio.Entidades.EstadosBloqueadosEmissao();

                            estadoBloqueadoEmissao.Codigo = 0;
                            estadoBloqueadoEmissao.ConfiguracaoEmpresa = configuracaoEmpresa;
                            estadoBloqueadoEmissao.Estado = repEstado.BuscarPorSigla(estadosBloqueados[i].SiglaUF);

                            repEstadosBloqueadosEmissao.Inserir(estadoBloqueadoEmissao);
                        }
                    }
                }
            }
        }
        
        private void SalvarServicoNFsePorCidade(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ServicosCidades"]))
            {
                List<Dominio.ObjetosDeValor.ServicoNFsePorCidade> servicosCidades = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ServicoNFsePorCidade>>(Request.Params["ServicosCidades"]);

                if (servicosCidades != null)
                {
                    Repositorio.ServicoNFsePorCidade repServicoNFsePorCidade = new Repositorio.ServicoNFsePorCidade(unidadeDeTrabalho);
                    Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
                    Repositorio.ServicoNFSe repServicoNFSe = new Repositorio.ServicoNFSe(unidadeDeTrabalho);
                    Repositorio.NaturezaNFSe repNaturezaNFSe = new Repositorio.NaturezaNFSe(unidadeDeTrabalho);

                    List<Dominio.Entidades.ServicoNFsePorCidade> ServicoNFsePorCidadeExcluir = repServicoNFsePorCidade.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < ServicoNFsePorCidadeExcluir.Count; i++)
                        repServicoNFsePorCidade.Deletar(ServicoNFsePorCidadeExcluir[i]);

                    for (var i = 0; i < servicosCidades.Count; i++)
                    {
                        if (!servicosCidades[i].Excluir)
                        {
                            var servicoNFsePorCidade = new Dominio.Entidades.ServicoNFsePorCidade();

                            servicoNFsePorCidade.ConfiguracaoEmpresa = configuracaoEmpresa;
                            servicoNFsePorCidade.Localidade = repLocalidade.BuscarPorCodigo(servicosCidades[i].CodigoCidade);
                            servicoNFsePorCidade.NaturezaNFSe = repNaturezaNFSe.BuscarPorCodigo(servicosCidades[i].CodigoNatureza);
                            servicoNFsePorCidade.ServicoNFSe = repServicoNFSe.BuscarPorCodigo(servicosCidades[i].CodigoServico);

                            repServicoNFsePorCidade.Inserir(servicoNFsePorCidade);
                        }
                    }
                }
            }
        }

        private void SalvarCodigosServicosNFse(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["CodigosServicos"]))
            {
                List<dynamic> codigosServicosFront = JsonConvert.DeserializeObject<List<dynamic>>(Request.Params["CodigosServicos"]);

                if (codigosServicosFront != null)
                {
                    Repositorio.CodigosServicoNFSe repCodigosServico = new Repositorio.CodigosServicoNFSe(unidadeDeTrabalho);

                    List<Dominio.Entidades.CodigosServicoNFSe> CodigosServicosNFseExcluir = repCodigosServico.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < CodigosServicosNFseExcluir.Count; i++)
                        repCodigosServico.Deletar(CodigosServicosNFseExcluir[i]);

                    for (var i = 0; i < codigosServicosFront.Count; i++)
                    {
                        if (!((string)codigosServicosFront[i].Excluir).ToBool())
                        {
                            var codigosServicoNFse = new Dominio.Entidades.CodigosServicoNFSe()
                            {
                                ConfiguracaoEmpresa = configuracaoEmpresa,
                                CodigoTributacao = (string)codigosServicosFront[i].CodigoTributacao,
                                CodigoTributacaoPrefeitura = (string)codigosServicosFront[i].CodigoTributacaoPrefeitura,
                                NumeroTributacaoPrefeitura = (string)codigosServicosFront[i].NumeroTributacaoPrefeitura,
                                CNAE = string.IsNullOrWhiteSpace((string)codigosServicosFront[i].CNAE) ? null : (string)codigosServicosFront[i].CNAE,
                            };

                            repCodigosServico.Inserir(codigosServicoNFse);
                        }
                    }
                }
            }
        }

        private void SalvarSeriesCliente(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["SeriesPorCliente"]))
            {
                List<Dominio.ObjetosDeValor.ConfiguracaoEmpresaClienteSerie> seriesPorCliente = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ConfiguracaoEmpresaClienteSerie>>(Request.Params["SeriesPorCliente"]);

                if (seriesPorCliente != null)
                {
                    Repositorio.ConfiguracaoEmpresaClienteSerie repConfiguracaoEmpresaClienteSerie = new Repositorio.ConfiguracaoEmpresaClienteSerie(unidadeDeTrabalho);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                    Repositorio.EmpresaSerie repSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                    List<Dominio.Entidades.ConfiguracaoEmpresaClienteSerie> seriesExcluir = repConfiguracaoEmpresaClienteSerie.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < seriesExcluir.Count; i++)
                        repConfiguracaoEmpresaClienteSerie.Deletar(seriesExcluir[i]);

                    for (var i = 0; i < seriesPorCliente.Count; i++)
                    {
                        if (!seriesPorCliente[i].Excluir && !string.IsNullOrWhiteSpace(seriesPorCliente[i].CnpjCliente))
                        {
                            var clienteSerie = new Dominio.Entidades.ConfiguracaoEmpresaClienteSerie();

                            Dominio.Enumeradores.TipoTomador tipoCliente;
                            Enum.TryParse<Dominio.Enumeradores.TipoTomador>(seriesPorCliente[i].TipoCliente, out tipoCliente);

                            clienteSerie.Codigo = 0;
                            clienteSerie.ConfiguracaoEmpresa = configuracaoEmpresa;
                            clienteSerie.Cliente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(seriesPorCliente[i].CnpjCliente)));
                            clienteSerie.Serie = repSerie.BuscarPorCodigo(seriesPorCliente[i].CodigoSerie);
                            clienteSerie.TipoCliente = tipoCliente;
                            clienteSerie.RaizCNPJ = seriesPorCliente[i].RaizCNPJ == "Sim";

                            repConfiguracaoEmpresaClienteSerie.Inserir(clienteSerie);
                        }
                    }
                }
            }
        }

        private void SalvarAverbacoes(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ListaAverbacoes"]))
            {
                List<Dominio.ObjetosDeValor.ConfiguracaoAverbacaoClientes> listaAverabcoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ConfiguracaoAverbacaoClientes>>(Request.Params["ListaAverbacoes"]);

                if (listaAverabcoes != null)
                {
                    Repositorio.ConfiguracaoAverbacaoClientes repConfiguracaoAverbacaoClientes = new Repositorio.ConfiguracaoAverbacaoClientes(unidadeDeTrabalho);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);

                    List<Dominio.Entidades.ConfiguracaoAverbacaoClientes> averbacoesClienteExcluir = repConfiguracaoAverbacaoClientes.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < averbacoesClienteExcluir.Count; i++)
                        repConfiguracaoAverbacaoClientes.Deletar(averbacoesClienteExcluir[i]);

                    for (var i = 0; i < listaAverabcoes.Count; i++)
                    {
                        if (!listaAverabcoes[i].Excluir)
                        {
                            var averbacaoCliente = new Dominio.Entidades.ConfiguracaoAverbacaoClientes();

                            Dominio.Enumeradores.TipoTomador tipoDoTomador;
                            Enum.TryParse<Dominio.Enumeradores.TipoTomador>(listaAverabcoes[i].Tipo, out tipoDoTomador);

                            Dominio.Enumeradores.TipoCTEAverbacao tipoCTeAverbacao;
                            Enum.TryParse<Dominio.Enumeradores.TipoCTEAverbacao>(listaAverabcoes[i].TipoCTeAverbacao, out tipoCTeAverbacao);

                            Dominio.Enumeradores.IntegradoraAverbacao integradoraAverbacao = listaAverabcoes[i].IntegradoraAverbacao == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM : listaAverabcoes[i].IntegradoraAverbacao == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum : listaAverabcoes[i].IntegradoraAverbacao == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro : listaAverabcoes[i].IntegradoraAverbacao == "E" ? Dominio.Enumeradores.IntegradoraAverbacao.ELT : Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;

                            double cnpj;
                            double.TryParse(listaAverabcoes[i].CnpjCliente, out cnpj);

                            averbacaoCliente.Cliente = repCliente.BuscarPorCPFCNPJ(cnpj);
                            averbacaoCliente.TipoTomador = tipoDoTomador;
                            averbacaoCliente.Configuracao = configuracaoEmpresa;
                            averbacaoCliente.IntegradoraAverbacao = integradoraAverbacao;
                            averbacaoCliente.CodigoAverbacao = listaAverabcoes[i].CodigoAverbacao;
                            averbacaoCliente.UsuarioAverbacao = listaAverabcoes[i].UsuarioAverbacao;
                            averbacaoCliente.SenhaAverbacao = listaAverabcoes[i].SenhaAverbacao;
                            averbacaoCliente.TokenAverbacao = listaAverabcoes[i].TokenAverbacao;
                            averbacaoCliente.RaizCNPJ = listaAverabcoes[i].RaizCNPJ;
                            averbacaoCliente.NaoAverbar = listaAverabcoes[i].NaoAverbar;
                            averbacaoCliente.TipoCTeAverbacao = tipoCTeAverbacao;

                            repConfiguracaoAverbacaoClientes.Inserir(averbacaoCliente);
                        }
                    }
                }
            }
        }

        private void SalvarAverbacaoSerie(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ListaAverbacoesSerie"]))
            {
                List<Dominio.ObjetosDeValor.ConfiguracaoAverbacaoSerie> listaAverbacoes = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ConfiguracaoAverbacaoSerie>>(Request.Params["ListaAverbacoesSerie"]);

                if (listaAverbacoes != null)
                {
                    Repositorio.ConfiguracaoAverbacaoSerie repConfiguracaoAverbacaoSerie = new Repositorio.ConfiguracaoAverbacaoSerie(unidadeDeTrabalho);
                    Repositorio.EmpresaSerie repEmpresaSerie = new Repositorio.EmpresaSerie(unidadeDeTrabalho);

                    List<Dominio.Entidades.ConfiguracaoAverbacaoSerie> averbacoesSerieExcluir = repConfiguracaoAverbacaoSerie.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);
                    for (var i = 0; i < averbacoesSerieExcluir.Count; i++)
                        repConfiguracaoAverbacaoSerie.Deletar(averbacoesSerieExcluir[i]);

                    for (var i = 0; i < listaAverbacoes.Count; i++)
                    {
                        if (!listaAverbacoes[i].Excluir)
                        {
                            var averbacaoCliente = new Dominio.Entidades.ConfiguracaoAverbacaoSerie();

                            averbacaoCliente.EmpresaSerie = repEmpresaSerie.BuscarPorCodigo(listaAverbacoes[i].CodigoSerie);
                            averbacaoCliente.Configuracao = configuracaoEmpresa;
                            averbacaoCliente.NaoAverbar = true;

                            repConfiguracaoAverbacaoSerie.Inserir(averbacaoCliente);
                        }
                    }
                }
            }
        }

        private void SalvarConfiguracoesFTP(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["ListaFTP"]))
            {
                List<Dominio.ObjetosDeValor.ConfiguracaoFTP> listaConfiguracaoAverbacaoFTP = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ConfiguracaoFTP>>(Request.Params["ListaFTP"]);

                if (listaConfiguracaoAverbacaoFTP != null)
                {
                    Repositorio.ConfiguracaoFTP repConfiguracaoFTP = new Repositorio.ConfiguracaoFTP(unidadeDeTrabalho);
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unidadeDeTrabalho);
                    Repositorio.LayoutEDI repLayoutEDI = new Repositorio.LayoutEDI(unidadeDeTrabalho);

                    for (var i = 0; i < listaConfiguracaoAverbacaoFTP.Count; i++)
                    {
                        Dominio.Entidades.ConfiguracaoFTP configuracaoFTP = repConfiguracaoFTP.BuscarPorCodigo(listaConfiguracaoAverbacaoFTP[i].Id, configuracaoEmpresa.Codigo);

                        if (configuracaoFTP == null)
                            configuracaoFTP = new Dominio.Entidades.ConfiguracaoFTP();
                        else if (listaConfiguracaoAverbacaoFTP[i].Excluir)
                        {
                            repConfiguracaoFTP.Deletar(configuracaoFTP);
                            continue;
                        }

                        configuracaoFTP.Cliente = repCliente.BuscarPorCPFCNPJ(listaConfiguracaoAverbacaoFTP[i].Cliente);
                        if (!string.IsNullOrWhiteSpace(listaConfiguracaoAverbacaoFTP[i].LayoutEDI) && listaConfiguracaoAverbacaoFTP[i].LayoutEDI != "0")
                            configuracaoFTP.LayoutEDI = repLayoutEDI.BuscarPorCodigo(int.Parse(listaConfiguracaoAverbacaoFTP[i].LayoutEDI));
                        configuracaoFTP.Tipo = listaConfiguracaoAverbacaoFTP[i].Tipo;
                        configuracaoFTP.TipoArquivo = listaConfiguracaoAverbacaoFTP[i].TipoArquivo;
                        configuracaoFTP.Host = listaConfiguracaoAverbacaoFTP[i].Host;
                        configuracaoFTP.Porta = listaConfiguracaoAverbacaoFTP[i].Porta;
                        configuracaoFTP.Usuario = listaConfiguracaoAverbacaoFTP[i].Usuario;
                        configuracaoFTP.Senha = listaConfiguracaoAverbacaoFTP[i].Senha;
                        configuracaoFTP.Diretorio = listaConfiguracaoAverbacaoFTP[i].Diretorio;
                        configuracaoFTP.Passivo = listaConfiguracaoAverbacaoFTP[i].Passivo;
                        configuracaoFTP.Seguro = listaConfiguracaoAverbacaoFTP[i].Seguro;
                        configuracaoFTP.GerarNFSe = listaConfiguracaoAverbacaoFTP[i].GerarNFSe;
                        configuracaoFTP.EmitirDocumento = listaConfiguracaoAverbacaoFTP[i].EmitirDocumento;
                        configuracaoFTP.Rateio = listaConfiguracaoAverbacaoFTP[i].Rateio;
                        configuracaoFTP.SSL = listaConfiguracaoAverbacaoFTP[i].SSL;
                        configuracaoFTP.UtilizarContratanteComoTomador = listaConfiguracaoAverbacaoFTP[i].UtilizarContratanteComoTomador;
                        configuracaoFTP.Configuracao = configuracaoEmpresa;

                        //string erroConexao = string.Empty;
                        //if (!Servicos.FTP.TestarConexao(configuracaoFTP.Host, configuracaoFTP.Porta, configuracaoFTP.Diretorio, configuracaoFTP.Usuario, configuracaoFTP.Senha, configuracaoFTP.Passivo, configuracaoFTP.SSL, out erroConexao, configuracaoFTP.Seguro))
                        //    throw new Exception("Falha ao salvar FTP:" + configuracaoFTP.Host + " " + erroConexao);

                        //MemoryStream xml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("teste"));
                        //Servicos.FTP.EnviarArquivo(xml, "teste.txt", configuracaoFTP.Host, configuracaoFTP.Porta, configuracaoFTP.Diretorio, configuracaoFTP.Usuario, configuracaoFTP.Senha, configuracaoFTP.Passivo, configuracaoFTP.SSL, out erroConexao, configuracaoFTP.Seguro);

                        if (configuracaoFTP.Codigo > 0)
                            repConfiguracaoFTP.Atualizar(configuracaoFTP);
                        else
                            repConfiguracaoFTP.Inserir(configuracaoFTP);
                    }
                }
            }
        }

        private void SalvarCNPJCPFAutorizados(Dominio.Entidades.ConfiguracaoEmpresa configuracaoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (!string.IsNullOrWhiteSpace(Request.Params["DocumentosXML"]))
            {
                List<Dominio.ObjetosDeValor.ConfiguracaoAutDownloadXML> listaDocumentos = JsonConvert.DeserializeObject<List<Dominio.ObjetosDeValor.ConfiguracaoAutDownloadXML>>(Request.Params["DocumentosXML"]);

                if (listaDocumentos != null)
                {
                    Repositorio.ConfiguracaoAutDownloadXML repConfiguracaoAutDownloadXML = new Repositorio.ConfiguracaoAutDownloadXML(unidadeDeTrabalho);
                    List<Dominio.Entidades.ConfiguracaoAutDownloadXML> documentosXMLExcluir = repConfiguracaoAutDownloadXML.BuscarPorConfiguracao(configuracaoEmpresa.Codigo);

                    for (var i = 0; i < documentosXMLExcluir.Count; i++)
                        repConfiguracaoAutDownloadXML.Deletar(documentosXMLExcluir[i]);

                    for (var i = 0; i < listaDocumentos.Count; i++)
                    {
                        if (!listaDocumentos[i].Excluir)
                        {
                            var documentoAutorizado = new Dominio.Entidades.ConfiguracaoAutDownloadXML();

                            documentoAutorizado.CNPJCPF = listaDocumentos[i].CnpjCpf;
                            documentoAutorizado.Configuracao = configuracaoEmpresa;

                            repConfiguracaoAutDownloadXML.Inserir(documentoAutorizado);
                        }
                    }
                }
            }
        }

        private void SalvarMovimentoFinanceiro(ref Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.PlanoDeConta repPlanoDeConta = new Repositorio.PlanoDeConta(unitOfWork);

            Enum.TryParse(Request.Params["AcertoViagemMovimentoReceitas"], out Dominio.Enumeradores.TipoMovimentoAcerto movimentoReceitas);
            Enum.TryParse(Request.Params["AcertoViagemMovimentoDespesas"], out Dominio.Enumeradores.TipoMovimentoAcerto movimentoDespesas);
            Enum.TryParse(Request.Params["AcertoViagemMovimentoDespesasAbastecimentos"], out Dominio.Enumeradores.TipoMovimentoAcerto movimentoDespesasAbastecimentos);
            Enum.TryParse(Request.Params["AcertoViagemMovimentoDespesasAdiantamentosMotorista"], out Dominio.Enumeradores.TipoMovimentoAcerto movimentoDespesasAdiantamentosMotorista);
            Enum.TryParse(Request.Params["AcertoViagemMovimentoReceitasDevolucoesMotorista"], out Dominio.Enumeradores.TipoMovimentoAcerto movimentoReceitasDevolucoesMotorista);

            int.TryParse(Request.Params["AcertoViagemContaReceitas"], out int contaReceitas);
            int.TryParse(Request.Params["AcertoViagemContaDespesas"], out int contaDespesas);
            int.TryParse(Request.Params["AcertoViagemContaDespesasAbastecimentos"], out int contaDespesasAbastecimentos);
            int.TryParse(Request.Params["AcertoViagemContaDespesasPagamentosMotorista"], out int contaDespesasPagamentosMotorista);
            int.TryParse(Request.Params["AcertoViagemContaDespesasAdiantamentosMotorista"], out int contaDespesasAdiantamentosMotorista);
            int.TryParse(Request.Params["AcertoViagemContaReceitasDevolucoesMotorista"], out int contaReceitasDevolucoesMotorista);

            empresa.Configuracao.AcertoViagemMovimentoReceitas = movimentoReceitas;
            empresa.Configuracao.AcertoViagemMovimentoDespesas = movimentoDespesas;
            empresa.Configuracao.AcertoViagemMovimentoDespesasAbastecimentos = movimentoDespesasAbastecimentos;
            empresa.Configuracao.AcertoViagemMovimentoDespesasAdiantamentosMotorista = movimentoDespesasAdiantamentosMotorista;
            empresa.Configuracao.AcertoViagemMovimentoReceitasDevolucoesMotorista = movimentoReceitasDevolucoesMotorista;

            empresa.Configuracao.AcertoViagemContaReceitas = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaReceitas);
            empresa.Configuracao.AcertoViagemContaDespesas = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaDespesas);
            empresa.Configuracao.AcertoViagemContaDespesasAbastecimentos = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaDespesasAbastecimentos);
            empresa.Configuracao.AcertoViagemContaDespesasPagamentosMotorista = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaDespesasPagamentosMotorista);
            empresa.Configuracao.AcertoViagemContaDespesasAdiantamentosMotorista = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaDespesasAdiantamentosMotorista);
            empresa.Configuracao.AcertoViagemContaReceitasDevolucoesMotorista = repPlanoDeConta.BuscarPorCodigo(this.EmpresaUsuario.Codigo, contaReceitasDevolucoesMotorista);
        }

        private void SalvarLogDicas(Dominio.Entidades.Empresa empresa, string dicasEmissaoCTe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.EmpresaLogDicas repEmpresaLogDicas = new Repositorio.EmpresaLogDicas(unidadeDeTrabalho);
            Dominio.Entidades.EmpresaLogDicas empresaLogDicas = new Dominio.Entidades.EmpresaLogDicas();
            empresaLogDicas.Empresa = empresa;
            empresaLogDicas.Funcionario = this.UsuarioAdministrativo != null ? this.UsuarioAdministrativo : this.Usuario;
            empresaLogDicas.Data = DateTime.Now;
            empresaLogDicas.DicasEmissaoCTe = dicasEmissaoCTe;
            repEmpresaLogDicas.Inserir(empresaLogDicas);
        }

        #endregion

        #region CorrecaoDicas
        [AcceptVerbs("POST")]
        public ActionResult CDLoopCorrecao()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                List<Dominio.Entidades.Empresa> empresas = repEmpresa.BuscarTodas(Request.Params["Status"]);

                int i = 0;
                int s = empresas.Count();
                int c = 0;
                int j = 0;
                for (; i < s; i++)
                {
                    if (empresas[i].Configuracao != null && !string.IsNullOrWhiteSpace(empresas[i].Configuracao.DicasEmissaoCTe))
                    {
                        string nomeEmpresa = empresas[i].NomeFantasia;
#if DEBUG
                        string stripDicas = Regex.Replace(empresas[i].Configuracao.DicasEmissaoCTe, "<.*?>", String.Empty);
#else
                        string stripDicas = empresas[i].Configuracao.DicasEmissaoCTe;
#endif
                        string[] dicas = stripDicas.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                        for (j = 0; j < dicas.Length; j++)
                            dicas[j] = string.Concat("<p>", dicas[j], "</p>");

                        empresas[i].Configuracao.DicasEmissaoCTe = String.Join("\n", dicas);

                        repEmpresa.Atualizar(empresas[i]);
                        c++;
                    }
                }

                return Json("Sucesso ao atualizar as " + c.ToString() + " empresas de um total de " + s.ToString() + " empresas.", true);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return Json<bool>(false, false, "Ocorreu uma falha ao obter as configurações da empresa.");
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        #endregion
    }
}
