using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos
{
    public class Empresa
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public Empresa(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public void GerarIntegracoes()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.EmpresaIntegracao repEmpresaIntegracao = new Repositorio.EmpresaIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Sintegra);
            if (tipo == null)
                return;

            List<Dominio.Entidades.Empresa> empresas = repEmpresaIntegracao.BuscarEmpresasSemIntegracao();

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                Dominio.Entidades.EmpresaIntegracao empresaIntegracao = new Dominio.Entidades.EmpresaIntegracao();
                empresaIntegracao.TipoIntegracao = tipo;
                empresaIntegracao.DataIntegracao = DateTime.Now;
                empresaIntegracao.ProblemaIntegracao = "";
                empresaIntegracao.Empresa = empresa;
                empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                repEmpresaIntegracao.Inserir(empresaIntegracao);
            }
        }

        public void AtualizarIntegracoes()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(_unitOfWork);
            Repositorio.EmpresaIntegracao repEmpresaIntegracao = new Repositorio.EmpresaIntegracao(_unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipo = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Sintegra);
            if (tipo == null)
                return;

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();
            if (configuracaoIntegracao == null)
                return;

            List<Dominio.Entidades.EmpresaIntegracao> empresaIntegracoes = repEmpresaIntegracao.BuscarEmpresasParaNovaIntegracao(configuracaoIntegracao.IntervaloConsultaSintegra);

            foreach (Dominio.Entidades.EmpresaIntegracao empresaIntegracao in empresaIntegracoes)
            {
                empresaIntegracao.ProblemaIntegracao = "";
                empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                repEmpresaIntegracao.Atualizar(empresaIntegracao);
            }
        }

        public void VerificarIntegracoesPendentes(Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            Repositorio.EmpresaIntegracao repEmpresaIntegracao = new Repositorio.EmpresaIntegracao(_unitOfWork);

            Servicos.Embarcador.Integracao.Sintegra.IntegracaoSintegra svcIntegracaoSintegra = new Servicos.Embarcador.Integracao.Sintegra.IntegracaoSintegra(_unitOfWork);

            List<Dominio.Entidades.EmpresaIntegracao> empresaIntegracoes = repEmpresaIntegracao.BuscarIntegracaoPendente();

            foreach (Dominio.Entidades.EmpresaIntegracao empresaIntegracao in empresaIntegracoes)
            {
                if (empresaIntegracao.TipoIntegracao.Tipo == TipoIntegracao.Sintegra)
                    svcIntegracaoSintegra.ConsultarSimplesNacional(empresaIntegracao, auditado);
                else
                {
                    empresaIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    empresaIntegracao.ProblemaIntegracao = "Tipo de integração não implementada";
                    empresaIntegracao.NumeroTentativas++;

                    repEmpresaIntegracao.Atualizar(empresaIntegracao);
                }
            }
        }

        #endregion

        #region Métodos Públicos Estáticos

        public static Dominio.ObjetosDeValor.CTe.Empresa ObterEmpresaCTE(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.CTe.Empresa empresaCTE = new Dominio.ObjetosDeValor.CTe.Empresa();

            empresaCTE.CNPJ = empresa.CNPJ;
            empresaCTE.Bairro = empresa.Bairro;
            empresaCTE.CEP = empresa.CEP;
            empresaCTE.Complemento = empresa.Complemento;
            empresaCTE.Email = empresa.Email;
            empresaCTE.StatusEmail = empresa.StatusEmail == "A" ? true : false;
            empresaCTE.EmailContador = empresa.EmailContador;
            empresaCTE.TelefoneContador = empresa.TelefoneContador;
            empresaCTE.StatusEmailContador = empresa.StatusEmailContador == "A" ? true : false;
            empresaCTE.Contato = empresa.Contato;
            empresaCTE.TelefoneContato = empresa.TelefoneContato;
            empresaCTE.Endereco = empresa.Endereco;
            empresaCTE.InscricaoEstadual = empresa.InscricaoEstadual;
            empresaCTE.CodigoIBGECidade = empresa.Localidade.CodigoIBGE;
            empresaCTE.RazaoSocial = empresa.RazaoSocial;
            empresaCTE.NomeFantasia = empresa.NomeFantasia;
            empresaCTE.Numero = empresa.Numero;
            empresaCTE.Telefone1 = Utilidades.String.OnlyNumbers(empresa.Telefone);
            empresaCTE.Endereco = empresa.Endereco;
            empresaCTE.EmailAdministrativo = empresa.EmailAdministrativo;
            empresaCTE.NomeContador = empresa.NomeContador;
            empresaCTE.OptanteSimplesNacional = empresa.OptanteSimplesNacional;
            empresaCTE.Responsavel = empresa.Responsavel;

            return empresaCTE;
        }

        public static Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa ObterEmpresa(Dominio.Entidades.Cliente empresa)
        {
            Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa empresaRetorno = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
            empresaRetorno.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
            empresaRetorno.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();

            empresaRetorno.CNPJ = empresa.CPF_CNPJ_Formatado;
            empresaRetorno.Endereco.Bairro = empresa.Bairro;
            empresaRetorno.Endereco.CEP = empresa.CEP;
            empresaRetorno.Endereco.Complemento = empresa.Complemento;
            empresaRetorno.Endereco.Logradouro = empresa.Endereco;
            empresaRetorno.IE = empresa.IE_RG;
            empresaRetorno.Endereco.Cidade.IBGE = empresa.Localidade.CodigoIBGE;
            empresaRetorno.RazaoSocial = empresa.Nome;
            empresaRetorno.NomeFantasia = empresa.NomeFantasia;
            empresaRetorno.Endereco.Numero = empresa.Numero;
            empresaRetorno.Endereco.Telefone = empresa.Telefone1;
            empresaRetorno.Endereco.Telefone2 = empresa.Telefone2;

            return empresaRetorno;
        }

        public static void AlertarVencimentoAntt(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(TipoConfiguracaoAlerta.Antt);

            if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                return;

            DateTime dataUltimoAlerta = DateTime.Now.Date;
            Embarcador.Notificacao.Notificacao servicoNotificacao = new Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Embarcador.Notificacao.NotificacaoEmpresa(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta()
            {
                AlertarAposVencimento = configuracaoAlerta.AlertarAposVencimento,
                DiasAlertarAntesVencimento = configuracaoAlerta.DiasAlertarAntesVencimento,
                DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta
            };
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            List<Dominio.Entidades.Empresa> empresas = repositorioEmpresa.BuscarPorVencimentoAnttAlertar(filtrosPesquisa);

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                int diasParaVencer = (int)empresa.DataValidadeANTT.Value.Subtract(dataUltimoAlerta).TotalDays;
                string mensagem = string.Empty;

                mensagem = string.Format(Localization.Resources.Global.Empresa.Embarcador, razaoSocial) + Environment.NewLine;

                if (diasParaVencer > 0)
                    mensagem += string.Format(Localization.Resources.Global.Empresa.ANTTTransportadorVenceEm, empresa.Descricao, diasParaVencer, (diasParaVencer == 1 ? "" : "s"));
                else if (diasParaVencer == 0)
                    mensagem += string.Format(Localization.Resources.Global.Empresa.ANTTTrasnportadorVenceHoje, empresa.Descricao);
                else
                    mensagem += string.Format(Localization.Resources.Global.Empresa.ANTTTransportadorVenceuA, empresa.Descricao, (-diasParaVencer), (diasParaVencer == -1 ? "" : "s"));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: empresa.Codigo,
                        URLPagina: "Transportadores/Transportador",
                        titulo: Localization.Resources.Global.Empresa.VencimentoANTT,
                        nota: mensagem,
                        icone: IconesNotificacao.atencao,
                        tipoNotificacao: TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                        unitOfWork: unitOfWork
                    );
                }

                if (configuracaoAlerta.AlertarTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = Localization.Resources.Global.Empresa.VencimentoANTT,
                        CabecalhoMensagem = Localization.Resources.Global.Empresa.AlertaVencimentoANTT,
                        Empresa = empresa,
                        Mensagem = mensagem
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
                }

                empresa.DataUltimoAlertaVencimentoAntt = dataUltimoAlerta;

                repositorioEmpresa.Atualizar(empresa);
            }
        }

        public static void AlertarVencimentoCertificadoDigital(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(TipoConfiguracaoAlerta.CertificadoDigital);

            if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                return;

            DateTime dataUltimoAlerta = DateTime.Now.Date;
            Embarcador.Notificacao.Notificacao servicoNotificacao = new Embarcador.Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Embarcador.Notificacao.NotificacaoEmpresa(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta()
            {
                AlertarAposVencimento = configuracaoAlerta.AlertarAposVencimento,
                DiasAlertarAntesVencimento = configuracaoAlerta.DiasAlertarAntesVencimento,
                DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta
            };
            Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
            List<Dominio.Entidades.Empresa> empresas = repositorioEmpresa.BuscarPorVencimentoCertificadoDigitalAlertar(filtrosPesquisa);

            foreach (Dominio.Entidades.Empresa empresa in empresas)
            {
                int diasParaVencer = (int)empresa.DataFinalCertificado.Value.Subtract(dataUltimoAlerta).TotalDays;
                string mensagem = string.Empty;

                mensagem = string.Format(Localization.Resources.Global.Empresa.Embarcador, razaoSocial) + Environment.NewLine;

                if (diasParaVencer > 0)
                    mensagem += string.Format(Localization.Resources.Global.Empresa.CertificadoDigitalTransportadorVenceEm, empresa.Descricao, diasParaVencer, (diasParaVencer == 1 ? "" : "s"));
                else if (diasParaVencer == 0)
                    mensagem += string.Format(Localization.Resources.Global.Empresa.CertificadoDigitalTransportadorVenceHoje, empresa.Descricao);
                else
                    mensagem += string.Format(Localization.Resources.Global.Empresa.CertificadoDigitalTransportadorVenceuA, empresa.Descricao, (-diasParaVencer), (diasParaVencer == -1 ? "" : "s"));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: empresa.Codigo,
                        URLPagina: "Transportadores/Transportador",
                        titulo: Localization.Resources.Global.Empresa.VencimentoCertificadoDigital,
                        nota: mensagem,
                        icone: IconesNotificacao.atencao,
                        tipoNotificacao: TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                        unitOfWork: unitOfWork
                    );
                }

                if (configuracaoAlerta.AlertarTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = "Vencimento de Certificado Digital",
                        CabecalhoMensagem = "Alerta de Vencimento de Certificado Digital",
                        Empresa = empresa,
                        Mensagem = mensagem
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
                }

                empresa.DataUltimoAlertaVencimentoCertificado = dataUltimoAlerta;

                repositorioEmpresa.Atualizar(empresa);
            }
        }

        public static Dominio.Entidades.Empresa AtualizarEmpresa(int codigoEmpresa, Dominio.ObjetosDeValor.CTe.Empresa emitente, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unidadeDeTrabalho);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null)
                return null;

            empresa.Bairro = emitente.Bairro;
            empresa.CEP = Utilidades.String.OnlyNumbers(emitente.CEP);
            empresa.Complemento = emitente.Complemento;
            empresa.Contato = emitente.Contato;
            empresa.DataAtualizacao = DateTime.Now;
            empresa.Email = emitente.Email;
            empresa.EmailAdministrativo = emitente.EmailAdministrativo;
            empresa.EmailContador = emitente.EmailContador;
            empresa.Endereco = emitente.Endereco;
            empresa.Fax = Utilidades.String.OnlyNumbers(emitente.Telefone2);
            empresa.InscricaoEstadual = string.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(emitente.InscricaoEstadual)) ? "ISENTO" : Utilidades.String.OnlyNumbers(emitente.InscricaoEstadual);
            empresa.InscricaoMunicipal = emitente.InscricaoMunicipal;
            empresa.Localidade = repLocalidade.BuscarPorCodigoIBGE(emitente.CodigoIBGECidade);
            empresa.NomeContador = emitente.NomeContador;
            empresa.NomeFantasia = emitente.NomeFantasia;
            empresa.Numero = emitente.Numero;
            if (emitente.OptanteSimplesNacional.HasValue)
                empresa.OptanteSimplesNacional = emitente.OptanteSimplesNacional.HasValue ? emitente.OptanteSimplesNacional.Value : false;
            empresa.RazaoSocial = emitente.RazaoSocial;
            empresa.RegistroANTT = emitente.RNTRC;
            empresa.Responsavel = emitente.Responsavel;
            empresa.StatusEmail = emitente.StatusEmail ? "A" : "I";
            empresa.StatusEmailAdministrativo = emitente.StatusEmailAdministrativo ? "A" : "I";
            empresa.StatusEmailContador = emitente.StatusEmailContador ? "A" : "I";
            empresa.Telefone = Utilidades.String.OnlyNumbers(emitente.Telefone2);
            empresa.TelefoneContador = Utilidades.String.OnlyNumbers(emitente.TelefoneContador);
            empresa.TelefoneContato = Utilidades.String.OnlyNumbers(emitente.TelefoneContato);

            repEmpresa.Atualizar(empresa);

            return empresa;
        }

        public static void EnviarCertificados(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            string ambiente = Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unidadeDeTrabalho).ObterConfiguracaoAmbiente().IdentificacaoAmbiente;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            List<Dominio.Entidades.Empresa> listaEmpresas = repEmpresa.BuscarAtivas();

            Servicos.Log.TratarErro("Inicio envio vencimentos Certificados", "EnvioVencimentoCertificados");
            if (listaEmpresas != null && listaEmpresas.Count() > 0)
            {
                for (var i = 0; i < listaEmpresas.Count; i++)
                {
                    try
                    {
                        if (Utilidades.Validate.ValidarCNPJ(listaEmpresas[i].CNPJ) && listaEmpresas[i].DataFinalCertificado.HasValue && !string.IsNullOrWhiteSpace(listaEmpresas[i].NomeCertificado)) //Não envia as empresas cadastradas com CNPJ Inválido e sem Certificado configurado
                        {
                            if (listaEmpresas[i].EmpresaPai != null && listaEmpresas[i].EmpresaPai.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao)
                            {
                                Servicos.Log.TratarErro(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - Selecionando", "EnvioVencimentoCertificados");

                                WSEmpresa.EmpresaClient wsEmpresa = new WSEmpresa.EmpresaClient();
                                string emails = !string.IsNullOrWhiteSpace(listaEmpresas[i].Email) ? listaEmpresas[i].Email : string.Empty;
                                if (!string.IsNullOrWhiteSpace(listaEmpresas[i].EmailAdministrativo))
                                    emails = !string.IsNullOrWhiteSpace(emails) ? string.Concat(emails, ',', listaEmpresas[i].EmailAdministrativo) : listaEmpresas[i].EmailAdministrativo;
                                if (!string.IsNullOrWhiteSpace(listaEmpresas[i].EmailContador))
                                    emails = !string.IsNullOrWhiteSpace(emails) ? string.Concat(emails, ',', listaEmpresas[i].EmailContador) : listaEmpresas[i].EmailContador;

                                string telefone = !string.IsNullOrWhiteSpace(listaEmpresas[i].Telefone) ? listaEmpresas[i].Telefone : string.Empty;
                                if (!string.IsNullOrWhiteSpace(listaEmpresas[i].TelefoneContato))
                                    telefone = !string.IsNullOrWhiteSpace(telefone) ? string.Concat(telefone, ',', listaEmpresas[i].TelefoneContato) : listaEmpresas[i].TelefoneContato;
                                if (!string.IsNullOrWhiteSpace(listaEmpresas[i].TelefoneContador))
                                    telefone = !string.IsNullOrWhiteSpace(telefone) ? string.Concat(telefone, ',', listaEmpresas[i].TelefoneContador) : listaEmpresas[i].TelefoneContador;

                                Servicos.Log.TratarErro(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - Buscou Empresa", "EnvioVencimentoCertificados");
                                Dominio.ObjetosDeValor.Empresa empresa = GerarObjetoEmpresa(listaEmpresas[i]);

                                Servicos.Log.TratarErro(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - Inicio envou WS", "EnvioVencimentoCertificados");
                                WSEmpresa.RetornoOfstring retorno = wsEmpresa.EnviarVencimentoCertificado(listaEmpresas[i].CNPJ, listaEmpresas[i].RazaoSocial, listaEmpresas[i].DataFinalCertificado.Value.ToString("dd/MM/yyyy"), ambiente, listaEmpresas[i].TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Homologacao ? "S" : "N", emails, telefone, empresa);
                                Servicos.Log.TratarErro(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") + " - Finalizou envou WS", "EnvioVencimentoCertificados");

                                if (!retorno.Status)
                                    Servicos.Log.TratarErro(listaEmpresas[i].CNPJ + " Erro: " + retorno.Mensagem, "EnvioVencimentoCertificados");
                                else
                                    Servicos.Log.TratarErro(listaEmpresas[i].CNPJ + " " + retorno.Mensagem, "EnvioVencimentoCertificados");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro(listaEmpresas[i].CNPJ + " Erro: " + ex, "EnvioVencimentoCertificados");
                    }
                }
            }
            Servicos.Log.TratarErro("Termino envio vencimentos Certificados", "EnvioVencimentoCertificados");
        }

        public Dominio.ObjetosDeValor.Empresa ConverterObjetoEmpresa(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Empresa empresaRetorno = null;

            empresaRetorno = new Dominio.ObjetosDeValor.Empresa();

            empresaRetorno.CNPJ = empresa.CNPJ;
            empresaRetorno.NomeFantasia = empresa.NomeFantasia;
            empresaRetorno.RazaoSocial = empresa.RazaoSocial;
            empresaRetorno.InscricaoEstadual = empresa.InscricaoEstadual;
            empresaRetorno.ANTT = empresa.RegistroANTT;
            empresaRetorno.CNAE = empresa.CNAE;
            empresaRetorno.OptanteSimplesNacional = empresa.OptanteSimplesNacional;

            empresaRetorno.CodigoIBGECidade = empresa.Localidade.CodigoIBGE;
            empresaRetorno.Endereco = empresa.Endereco;
            empresaRetorno.Complemento = empresa.Complemento;
            empresaRetorno.CEP = empresa.CEP;
            empresaRetorno.Bairro = empresa.Bairro;
            empresaRetorno.Numero = empresa.Numero;

            empresaRetorno.Contato = empresa.Contato;
            empresaRetorno.NomeContador = empresa.NomeContador;
            empresaRetorno.Responsavel = empresa.Responsavel;

            empresaRetorno.Fax = empresa.Fax;
            empresaRetorno.Telefone = empresa.Telefone;
            empresaRetorno.TelefoneContador = empresa.TelefoneContador;
            empresaRetorno.TelefoneContato = empresa.TelefoneContato;

            empresaRetorno.Email = empresa.Email;
            empresaRetorno.EmailAdministrativo = empresa.EmailAdministrativo;
            empresaRetorno.EmailContador = empresa.EmailContador;

            if (empresa.DataInicialCertificado.HasValue)
                empresaRetorno.DataInicialCertificado = empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy");

            if (empresa.DataFinalCertificado.HasValue)
                empresaRetorno.DataFinalCertificado = empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy");

            empresaRetorno.StatusEmail = !string.IsNullOrWhiteSpace(empresa.StatusEmail) && empresa.StatusEmail == "A";
            empresaRetorno.StatusEmailAdministrativo = !string.IsNullOrWhiteSpace(empresa.StatusEmailAdministrativo) && empresa.StatusEmailAdministrativo == "A";
            empresaRetorno.StatusEmailContador = !string.IsNullOrWhiteSpace(empresa.StatusEmailContador) && empresa.StatusEmailContador == "A";

            empresaRetorno.Status = "A";

            return empresaRetorno;
        }

        #endregion

        #region Métodos Privados

        private static Dominio.ObjetosDeValor.Empresa GerarObjetoEmpresa(Dominio.Entidades.Empresa empresa)
        {
            Dominio.ObjetosDeValor.Empresa empresaRetorno = null;

            try
            {
                empresaRetorno = new Dominio.ObjetosDeValor.Empresa();

                empresaRetorno.CNPJ = empresa.CNPJ;
                empresaRetorno.NomeFantasia = empresa.NomeFantasia;
                empresaRetorno.RazaoSocial = empresa.RazaoSocial;
                empresaRetorno.InscricaoEstadual = empresa.InscricaoEstadual;
                empresaRetorno.ANTT = empresa.RegistroANTT;
                empresaRetorno.CNAE = empresa.CNAE;
                empresaRetorno.OptanteSimplesNacional = empresa.OptanteSimplesNacional;

                empresaRetorno.CodigoIBGECidade = empresa.Localidade.CodigoIBGE;
                empresaRetorno.Endereco = empresa.Endereco;
                empresaRetorno.Complemento = empresa.Complemento;
                empresaRetorno.CEP = empresa.CEP;
                empresaRetorno.Bairro = empresa.Bairro;
                empresaRetorno.Numero = empresa.Numero;

                empresaRetorno.Contato = empresa.Contato;
                empresaRetorno.NomeContador = empresa.NomeContador;
                empresaRetorno.Responsavel = empresa.Responsavel;

                empresaRetorno.Fax = empresa.Fax;
                empresaRetorno.Telefone = empresa.Telefone;
                empresaRetorno.TelefoneContador = empresa.TelefoneContador;
                empresaRetorno.TelefoneContato = empresa.TelefoneContato;

                empresaRetorno.Email = empresa.Email;
                empresaRetorno.EmailAdministrativo = empresa.EmailAdministrativo;
                empresaRetorno.EmailContador = empresa.EmailContador;

                //if (empresa.DataInicialCertificado.HasValue)
                //    empresaRetorno.DataInicialCertificado = empresa.DataInicialCertificado.Value.ToString("dd/MM/yyyy");
                //if (empresa.DataFinalCertificado.HasValue)
                //    empresaRetorno.DataFinalCertificado = empresa.DataFinalCertificado.Value.ToString("dd/MM/yyyy");

                empresaRetorno.StatusEmail = !string.IsNullOrWhiteSpace(empresa.StatusEmail) && empresa.StatusEmail == "A";
                empresaRetorno.StatusEmailAdministrativo = !string.IsNullOrWhiteSpace(empresa.StatusEmailAdministrativo) && empresa.StatusEmailAdministrativo == "A";
                empresaRetorno.StatusEmailContador = !string.IsNullOrWhiteSpace(empresa.StatusEmailContador) && empresa.StatusEmailContador == "A";

                empresaRetorno.Status = "A";
            }
            catch (Exception e)
            {
                Servicos.Log.TratarErro("GerarObjetoEmpresa: " + e.Message);
            }

            return empresaRetorno;
        }

        #endregion
    }
}
