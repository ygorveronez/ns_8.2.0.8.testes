using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Transportadores
{
    public class Motorista
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Privados

        #region Construtores

        public Motorista(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public static void AlertarVencimentoCnh(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(unitOfWork);
            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoConfiguracaoAlerta.Cnh);

            if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                return;

            DateTime dataUltimoAlerta = DateTime.Now.Date;
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Notificacao.NotificacaoEmpresa(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta()
            {
                AlertarAposVencimento = configuracaoAlerta.AlertarAposVencimento,
                DiasAlertarAntesVencimento = configuracaoAlerta.DiasAlertarAntesVencimento,
                DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta
            };
            Repositorio.Usuario repositorioMotorista = new Repositorio.Usuario(unitOfWork);
            List<Dominio.Entidades.Usuario> motoristasAlertarVencimentoCnh = repositorioMotorista.BuscarMotoristasAlertarVencimentoCnh(filtrosPesquisa);

            foreach (Dominio.Entidades.Usuario motorista in motoristasAlertarVencimentoCnh)
            {
                int diasParaVencer = (int)motorista.DataVencimentoHabilitacao.Value.Subtract(dataUltimoAlerta).TotalDays;
                string mensagem = string.Empty;

                mensagem = string.Format(Localization.Resources.Transportadores.Motorista.Embarcador, razaoSocial) + Environment.NewLine;

                if (diasParaVencer > 0)
                    mensagem += string.Format(Localization.Resources.Transportadores.Motorista.CNHMotoristaVenceEm, motorista.Descricao, diasParaVencer, (diasParaVencer == 1 ? "" : "s"));
                else if (diasParaVencer == 0)
                    mensagem += string.Format(Localization.Resources.Transportadores.Motorista.CNHMotoristaVenceHoje, motorista.Descricao);
                else
                    mensagem += string.Format(Localization.Resources.Transportadores.Motorista.CNHMotoristaVenceuA, motorista.Descricao, (-diasParaVencer), (diasParaVencer == -1 ? "" : "s"));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: motorista.Codigo,
                        URLPagina: "Transportadores/Motorista",
                        titulo: Localization.Resources.Transportadores.Motorista.VencimentoCNH,
                        nota: mensagem,
                        icone: Dominio.ObjetosDeValor.Embarcador.Enumeradores.IconesNotificacao.atencao,
                        tipoNotificacao: Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                        unitOfWork: unitOfWork
                    );
                }

                if (configuracaoAlerta.AlertarTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = Localization.Resources.Transportadores.Motorista.VencimentoCNH,
                        CabecalhoMensagem = Localization.Resources.Transportadores.Motorista.AlertaVencimentoCNH,
                        Empresa = motorista.Empresa,
                        Mensagem = mensagem
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
                }

                motorista.DataUltimoAlertaVencimentoCnh = dataUltimoAlerta;

                repositorioMotorista.Atualizar(motorista);
            }
        }

        public static Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT IntegrarComCIOT(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = Servicos.Embarcador.CIOT.CIOT.ObterConfiguracaoCIOT(motorista.ClienteTerceiro, motorista.Empresa, unitOfWork);

            if (configuracaoCIOT != null && configuracaoCIOT.IntegrarMotoristaNoCadastro)
                return configuracaoCIOT;

            return null;
        }

        public static void AtualizarIntegracoes(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracao = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            IList<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoes = repositorioMotoristaIntegracao.BuscarPorMotorista(motorista.Codigo);

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracaoMotorista in integracoes)
            {
                if (!integracaoMotorista.TipoIntegracao.Ativo)
                    continue;

                TipoIntegracao tipoIntegracao = integracaoMotorista.TipoIntegracao.Tipo;

                if (tipoIntegracao == TipoIntegracao.Frota162 && motorista.TipoMotorista == TipoMotorista.Terceiro)
                    continue;

                if (tipoIntegracao == TipoIntegracao.BrasilRiskVeiculoMotorista)
                    continue;

                if (tipoIntegracao != TipoIntegracao.CIOT && !configuracao.TiposIntegracaoValidarMotorista.Any(o => o.Equals(integracaoMotorista.TipoIntegracao)))
                    repositorioMotoristaIntegracao.Deletar(integracaoMotorista);
                else if (tipoIntegracao == TipoIntegracao.Buonny || tipoIntegracao == TipoIntegracao.BuonnyRNTRC)
                {
                    if (integracaoMotorista.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    {
                        integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                        integracaoMotorista.DataIntegracao = DateTime.Now;
                        integracaoMotorista.NumeroTentativas = 0;
                        integracaoMotorista.ProblemaIntegracao = "";
                        repositorioMotoristaIntegracao.Atualizar(integracaoMotorista);
                    }
                }
                else if (tipoIntegracao == TipoIntegracao.Telerisco || tipoIntegracao == TipoIntegracao.BrasilRiskGestao || tipoIntegracao == TipoIntegracao.Frota162 || tipoIntegracao == TipoIntegracao.KMM)
                {
                    integracaoMotorista.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    integracaoMotorista.DataIntegracao = DateTime.Now;
                    integracaoMotorista.NumeroTentativas = 0;
                    integracaoMotorista.ProblemaIntegracao = "";
                    repositorioMotoristaIntegracao.Atualizar(integracaoMotorista);
                }
            }

            foreach (Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao in configuracao.TiposIntegracaoValidarMotorista)
            {
                if (tipoIntegracao.Ativo && !integracoes.Any(o => o.TipoIntegracao.Equals(tipoIntegracao)))
                {
                    if (tipoIntegracao.Tipo == TipoIntegracao.Frota162 && motorista.TipoMotorista == TipoMotorista.Terceiro)
                        continue;

                    Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Motorista = motorista,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                        TipoIntegracao = tipoIntegracao
                    };

                    repositorioMotoristaIntegracao.Inserir(integracao);
                }
            }

            GerarOuAtualizarIntegracaoCIOT(motorista, integracoes, unitOfWork);
            GerarIntegracaoMotoristaBRK(motorista, integracoes, unitOfWork);
        }

        public static string ConsultarMotoristaTelerisco(Dominio.Entidades.Usuario motorista, Dominio.Entidades.Embarcador.Filiais.Filial filial, DateTime dataHoraEmbarque, ref bool falhaIntegracao, ref string protocolo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, string placa)
        {
            falhaIntegracao = false;
            string mensagemErro = string.Empty;
            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            var repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);
            var repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco);

            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = repMotoristaIntegracao.BuscarPorMotoristaETipo(motorista.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco);

                if (integracao == null)
                {
                    integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Motorista = motorista,
                        ProblemaIntegracao = "",
                        TipoIntegracao = tipoIntegracao
                    };

                    repMotoristaIntegracao.Inserir(integracao);
                }

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas = 1;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.Protocolo = string.Empty;
                integracao.Mensagem = string.Empty;
                integracao.DescricaoTipo = string.Empty;

                Dominio.ObjetosDeValor.Embarcador.Integracao.Telerisco.ConsultaMotoristaResponse retorno = Servicos.Embarcador.Integracao.Telerisco.IntegracaoTelerisco.ConsultaMotorista(motorista, filial, dataHoraEmbarque, ref mensagemErro, ref jsonRequest, ref jsonResponse, tipoServicoMultisoftware, unitOfWork, placa);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno == null)
                {
                    integracao.ProblemaIntegracao = "Integração não teve retorno.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.retornoWs == "1")
                {
                    if (configuracaoIntegracao != null && !string.IsNullOrWhiteSpace(configuracaoIntegracao.CodigosAceitosRetornoTelerisco) && retorno.retornoWs == "1" && string.IsNullOrWhiteSpace(retorno.consulta))
                    {
                        if (configuracaoIntegracao.CodigosAceitosRetornoTelerisco.Contains(retorno.categoriaResultado))
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta retornada com sucesso";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            integracao.Protocolo = retorno.protocolo;
                            integracao.Mensagem = retorno.consultaMensagem;
                            integracao.DescricaoTipo = retorno.tipoMotorista?.ToString() ?? retorno.mensagemRetorno;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : !string.IsNullOrWhiteSpace(retorno.resultado) ? retorno.resultado : "Consulta sem mensagem de retorno";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else if ((retorno.retornoWs == "1" && string.IsNullOrWhiteSpace(retorno.consulta)) || retorno.consulta == "1" || retorno.categoriaResultado == "350" || retorno.categoriaResultado == "350" || retorno.categoriaResultado == "500" || retorno.categoriaResultado == "400" || retorno.categoriaResultado == "280" || retorno.categoriaResultado == "200" || retorno.categoriaResultado == "100")
                    {
                        if (retorno.categoriaResultado == "250" || retorno.categoriaResultado == "350" || retorno.categoriaResultado == "500" || retorno.categoriaResultado == "400" || retorno.categoriaResultado == "280" || retorno.categoriaResultado == "200" || retorno.categoriaResultado == "100")
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Consulta retornada com sucesso";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                            integracao.Protocolo = retorno.protocolo;
                            integracao.Mensagem = retorno.consultaMensagem;
                            integracao.DescricaoTipo = retorno.tipoMotorista?.ToString() ?? retorno.mensagemRetorno;
                        }
                        else
                        {
                            integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : !string.IsNullOrWhiteSpace(retorno.resultado) ? retorno.resultado : "Consulta sem mensagem de retorno";
                            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        }
                    }
                    else if (retorno.consulta == "2")
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Telerisco retornou Consulta não Localizada, verifique o histórico da integração no cadastro do motorista";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno.consulta == "3")
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Perfil atual do motorista diferente da última consulta, necessário nova consulta";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (retorno.consulta == "4")
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : "Motorista com restrição com a empresa Transportador/Embarcador";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else if (string.IsNullOrWhiteSpace(retorno.consulta))
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consultaMensagem : !string.IsNullOrWhiteSpace(retorno.resultado) ? retorno.resultado : "Motorista com restrição com a empresa Transportador/Embarcador";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        integracao.ProblemaIntegracao = !string.IsNullOrWhiteSpace(retorno.consultaMensagem) ? retorno.consulta + " - " + retorno.consultaMensagem : "Código consulta " + retorno.consulta + " não previsto no manual da Telerisco";
                        integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    }
                }
                else
                {
                    integracao.ProblemaIntegracao = retorno.mensagemRetorno;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                mensagemErro = integracao.ProblemaIntegracao;
                falhaIntegracao = integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                string stringRetorno = string.Empty;
                if (retorno != null)
                    stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(integracaoArquivo);
                protocolo = integracao.Protocolo;

                repMotoristaIntegracao.Inserir(integracao);
            }


            return mensagemErro;
        }

        public static string ConsultarMotoristaBrasilRisk(Dominio.Entidades.Usuario motorista, ref bool falhaIntegracao, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            falhaIntegracao = false;
            string mensagemErro = string.Empty;
            string xmlRequest = string.Empty;
            string xmlResponse = string.Empty;

            var repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);
            var repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao);

            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = repMotoristaIntegracao.BuscarPorMotoristaETipo(motorista.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.BrasilRiskGestao);

                if (integracao == null)
                {
                    integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Motorista = motorista,
                        ProblemaIntegracao = "",
                        TipoIntegracao = tipoIntegracao
                    };

                    repMotoristaIntegracao.Inserir(integracao);
                }

                integracao.DataIntegracao = DateTime.Now;
                integracao.NumeroTentativas = 1;
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;
                integracao.ProblemaIntegracao = string.Empty;
                integracao.Protocolo = string.Empty;
                integracao.Mensagem = string.Empty;
                integracao.DescricaoTipo = string.Empty;


                Servicos.ServicoBrasilRisk.GestaoAnaliseDePerfil.RetornoConsulta retorno = Servicos.Embarcador.Integracao.BrasilRisk.IntegracaoBrasilRisk.ConsultaMotorista(motorista.CPF, ref mensagemErro, ref xmlRequest, ref xmlResponse, unitOfWork);
                if (!string.IsNullOrWhiteSpace(mensagemErro))
                {
                    integracao.ProblemaIntegracao = mensagemErro.Length > 300 ? mensagemErro.Substring(0, 300) : mensagemErro;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno == null)
                {
                    integracao.ProblemaIntegracao = "Integração não teve retorno.";
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }
                else if (retorno.Status)
                {
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    integracao.Mensagem = retorno.Mensagem;
                }
                else
                {
                    integracao.ProblemaIntegracao = retorno.Mensagem;
                    integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                }

                mensagemErro = integracao.ProblemaIntegracao.Replace("\n", "");
                falhaIntegracao = integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

                string stringRetorno = string.Empty;
                if (retorno != null)
                    stringRetorno = Newtonsoft.Json.JsonConvert.SerializeObject(retorno);

                Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo integracaoArquivo = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                integracaoArquivo.Mensagem = !string.IsNullOrWhiteSpace(stringRetorno) ? Utilidades.String.Left(stringRetorno, 400) : integracao.ProblemaIntegracao;
                integracaoArquivo.Data = DateTime.Now;
                integracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.RetornoDoProcessamento;

                integracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlRequest, "xml", unitOfWork);
                integracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(xmlResponse, "xml", unitOfWork);

                repCargaCTeIntegracaoArquivo.Inserir(integracaoArquivo);

                if (integracao.ArquivosTransacao == null)
                    integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>();

                integracao.ArquivosTransacao.Add(integracaoArquivo);

                repMotoristaIntegracao.Inserir(integracao);

                if (integracao.SituacaoIntegracao == SituacaoIntegracao.ProblemaIntegracao)
                    GerarAtendimentoParaIntegracoesPendentesMotorista(integracao, codigoCarga, unitOfWork);
            }

            return mensagemErro;
        }

        public static string ConsultarMotoristaAdagio(Dominio.Entidades.Usuario motorista, ref bool falhaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            Servicos.Embarcador.Integracao.Adagio.IntegracaoAdagio integracaoAdagio = Servicos.Embarcador.Integracao.Adagio.IntegracaoAdagio.GetInstance(unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Integracao.Adagio.API.BuscarDpaCartaCpfResponse retorno = integracaoAdagio.ConsultaCartorialMotorista(motorista.CPF);
            if (retorno != null)
            {
                falhaIntegracao = !integracaoAdagio.VerificarStatusAceito(retorno.status);
                return integracaoAdagio.AlterarMensagem(retorno.status);
            }
            falhaIntegracao = true;
            return "Falha";
        }

        public static string ConsultarMotoristaBuonny(Dominio.Entidades.Embarcador.Cargas.Carga carga, Dominio.Entidades.Usuario motorista, ref bool falhaIntegracao, ref string protocolo, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            var repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            var repMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Buonny);

            if (tipoIntegracao != null)
            {
                Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = repMotoristaIntegracao.BuscarPorMotoristaETipo(motorista.Codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao.Telerisco);

                if (integracao == null)
                {
                    integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
                    {
                        DataIntegracao = DateTime.Now,
                        Motorista = motorista,
                        ProblemaIntegracao = "",
                        TipoIntegracao = tipoIntegracao,
                    };

                    repMotoristaIntegracao.Inserir(integracao);
                }


                Servicos.Embarcador.Integracao.Buonny.IntegracaoBuonny.StatusMotorista(ref integracao, tipoServicoMultisoftware, unitOfWork);

                falhaIntegracao = integracao.SituacaoIntegracao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                protocolo = integracao.Protocolo;

                return integracao.ProblemaIntegracao;
            }

            return "";
        }

        public static void AtualizarIntegracaoSituacaoColaborador(Repositorio.UnitOfWork unitOfWork, int codigoSituacaoColaborador)
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoA52 repIntegracaoA52 = new Repositorio.Embarcador.Configuracoes.IntegracaoA52(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipointegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.A52);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoA52 integracao = repIntegracaoA52.BuscarPrimeiroRegistro();

            if ((integracao?.IntegrarSituacaoMotorista ?? false) && tipointegracao != null)
            {
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao repColaboradorSituacaoLancamentoIntegracao = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao(unitOfWork);
                Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);

                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao colaboradorSituacao = repColaboradorSituacaoLancamentoIntegracao.BuscarPorCodigoSituacao(codigoSituacaoColaborador, SituacaoIntegracao.AgIntegracao);
                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamento = repColaboradorLancamento.BuscarPorCodigo(codigoSituacaoColaborador);

                if (colaboradorSituacao == null)
                {
                    colaboradorSituacao = new Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorSituacaoLancamentoIntegracao()
                    {
                        ColaboradorLancamento = lancamento,
                        Mensagem = "",
                        NumeroTentativas = 0,
                        ProblemaIntegracao = "",
                        SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                        DataIntegracao = DateTime.Now,
                        TipoIntegracao = tipointegracao
                    };
                    repColaboradorSituacaoLancamentoIntegracao.Inserir(colaboradorSituacao);
                }
                else
                {
                    colaboradorSituacao.Mensagem = "";
                    colaboradorSituacao.NumeroTentativas = 0;
                    colaboradorSituacao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                    colaboradorSituacao.DataIntegracao = DateTime.Now;
                    colaboradorSituacao.TipoIntegracao = tipointegracao;

                    repColaboradorSituacaoLancamentoIntegracao.Atualizar(colaboradorSituacao);
                }
            }
        }

        public static void AtualizarStatusColaborador(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, bool controlarTransacao = false, int codigoColaborador = 0)
        {
            Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento repColaboradorLancamento = new Repositorio.Embarcador.Usuarios.Colaborador.ColaboradorLancamento(unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repConfiguracaoVeiculo.BuscarConfiguracaoPadrao();
            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);

            List<Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento> lancamentos = repColaboradorLancamento.BuscarLancamentosPendentes(codigoColaborador);

            for (int i = 0; i < lancamentos.Count; i++)
            {
                if (controlarTransacao)
                    unitOfWork.Start();

                Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamento = repColaboradorLancamento.BuscarPorCodigo(lancamentos[i].Codigo);
                Dominio.Entidades.Usuario usuario = repUsuario.BuscarPorCodigo(lancamento.Colaborador.Codigo);

                if (lancamento.SituacaoLancamento == SituacaoLancamentoColaborador.Agendado && lancamento.DataInicial.Date <= DateTime.Now.Date)
                {
                    lancamento.SituacaoLancamento = SituacaoLancamentoColaborador.Execucao;

                    if (!configuracaoVeiculo.ManterVinculoMotoristaEmFolga)
                        RemoverVinculoVeiculosColaborador(lancamento, unitOfWork, auditado);

                    usuario.SituacaoColaborador = lancamento.ColaboradorSituacao.SituacaoColaborador;

                    if (lancamento.ColaboradorSituacao.SituacaoColaborador == SituacaoColaborador.Folga)
                        usuario.DiasFolgaRetirado += (int)(lancamento.DataFinal - lancamento.DataInicial).TotalDays;

                    repUsuario.Atualizar(usuario);
                    repColaboradorLancamento.Atualizar(lancamento);
                    AtualizarIntegracaoSituacaoColaborador(unitOfWork, lancamento.Codigo);
                }
                else if (lancamento.DataFinal.Date <= DateTime.Now.Date)
                {
                    lancamento.SituacaoLancamento = SituacaoLancamentoColaborador.Finalizado;
                    usuario.SituacaoColaborador = SituacaoColaborador.Trabalhando;

                    if (!configuracaoVeiculo.ManterVinculoMotoristaEmFolga)
                        RemoverVinculoVeiculosColaborador(lancamento, unitOfWork, auditado);

                    repUsuario.Atualizar(usuario);
                    repColaboradorLancamento.Atualizar(lancamento);
                    AtualizarIntegracaoSituacaoColaborador(unitOfWork, lancamento.Codigo);

                    Servicos.Auditoria.Auditoria.Auditar(auditado, usuario, null, "Alterou a situação para Trabalhando.", unitOfWork);
                }

                if (controlarTransacao)
                {
                    unitOfWork.CommitChanges();

                    unitOfWork.FlushAndClear();
                }
            }
        }

        public static bool GetHabilitarFichaMotorista(Dominio.Entidades.Usuario motorista, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            var configuacaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            return motorista.AtivarFichaMotorista || configuacaoTMS.HabilitarFichaMotoristaTodos;
        }

        public static void RemoverVinculoVeiculosColaborador(Dominio.Entidades.Embarcador.Usuarios.Colaborador.ColaboradorLancamento lancamento, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (lancamento?.ColaboradorSituacao?.SituacaoColaborador != SituacaoColaborador.Trabalhando && lancamento?.Colaborador?.Tipo == "M")
            {
                Repositorio.Embarcador.Veiculos.VeiculoMotorista repVeiculoMotorista = new Repositorio.Embarcador.Veiculos.VeiculoMotorista(unitOfWork);
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
                List<Dominio.Entidades.Veiculo> veiculos = lancamento.Colaborador != null ? repVeiculo.BuscarVeiculosPorMotorista(lancamento.Colaborador.Codigo) : null;

                if (veiculos.Count > 0)
                {
                    foreach (var veiculo in veiculos)
                    {
                        Servicos.Auditoria.Auditoria.Auditar(auditado, veiculo, $"Removido motorista principal.", unitOfWork);
                        repVeiculoMotorista.DeletarMotoristaPrincipal(veiculo.Codigo);
                    }
                }
            }
        }

        public void NotificarVencimentoLicencasMobile(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return;

            Repositorio.Embarcador.Transportadores.MotoristaLicenca repositorioMotoristaLicenca = new Repositorio.Embarcador.Transportadores.MotoristaLicenca(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista repositorioConfiguracaoMotorista = new Repositorio.Embarcador.Configuracoes.ConfiguracaoMotorista(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoMotorista configuracaoMotorista = repositorioConfiguracaoMotorista.BuscarConfiguracaoPadrao();
            if (configuracaoMotorista.DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca == 0)
                return;

            Notificacao.NotificacaoMTrack servicoNotificacaoMTrack = new Notificacao.NotificacaoMTrack(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca> motoristaLicencas = repositorioMotoristaLicenca.BuscarLicencasPorDataVencimentoParaMobile(DateTime.Now.Date.AddDays(configuracaoMotorista.DiasAntecidenciaParaComunicarMotoristaVencimentoLicenca));

            foreach (Dominio.Entidades.Embarcador.Transportadores.MotoristaLicenca motoristaLicenca in motoristaLicencas)
            {
                servicoNotificacaoMTrack.NotificarPushGenerica(motoristaLicenca.Motorista, "Uma licença vai vencer em breve. Confira!");
            }
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private static void GerarAtendimentoParaIntegracoesPendentesMotorista(Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao, int codigoCarga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Chamados.MotivoChamado repositorioChamado = new Repositorio.Embarcador.Chamados.MotivoChamado(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracao = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repositorioConfiguracao.BuscarConfiguracaoPadrao();
            Dominio.Entidades.Embarcador.Chamados.MotivoChamado motivoChamado = repositorioChamado.BuscarPorIntegracao(integracao.TipoIntegracao.Codigo);
            Dominio.Entidades.Embarcador.Cargas.Carga carga = repositorioCarga.BuscarPorCodigo(codigoCarga);

            if (motivoChamado == null)
                return;

            if (carga == null) return;

            Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado objetoChamado = new Dominio.ObjetosDeValor.Embarcador.Chamado.ObjetoChamado()
            {
                Observacao = $"Atendimento gerado a partir da rejeição da integração com {integracao.TipoIntegracao?.Descricao} do motorista '{integracao.Motorista?.Nome ?? string.Empty}'",
                MotivoChamado = motivoChamado,
                Carga = carga,
                Empresa = carga.Empresa,
                Cliente = carga.Pedidos?.FirstOrDefault()?.Pedido?.Remetente,
                TipoCliente = configuracaoTMS.ChamadoOcorrenciaUsaRemetente ? Dominio.Enumeradores.TipoTomador.Remetente : Dominio.Enumeradores.TipoTomador.Destinatario
            };

            Dominio.Entidades.Usuario usuario = carga.Operador;

            if (usuario == null)
                return;

            Dominio.Entidades.Embarcador.Chamados.Chamado chamado = Servicos.Embarcador.Chamado.Chamado.AbrirChamado(objetoChamado, usuario, 0, null, unitOfWork);
        }

        private static void GerarOuAtualizarIntegracaoCIOT(Dominio.Entidades.Usuario motorista, IList<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoes, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT = IntegrarComCIOT(motorista, unitOfWork);

            if (configuracaoCIOT == null)
                return;

            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);
            Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = integracoes.FirstOrDefault(o => o.ConfiguracaoCIOT != null && o.ConfiguracaoCIOT.Codigo == configuracaoCIOT.Codigo);

            if (integracao != null)
            {
                // Atualizando Motorista
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.NumeroTentativas = 0;
                integracao.ProblemaIntegracao = "";

                repositorioMotoristaIntegracao.Atualizar(integracao);

                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.CIOT);

            if (tipoIntegracao == null)
                return;

            // Inserindo Motorista
            integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
            {
                DataIntegracao = DateTime.Now,
                Motorista = motorista,
                ProblemaIntegracao = "",
                ConfiguracaoCIOT = configuracaoCIOT,
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracao
            };

            repositorioMotoristaIntegracao.Inserir(integracao);
        }

        private static void GerarIntegracaoMotoristaBRK(Dominio.Entidades.Usuario motorista, IList<Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao> integracoes, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo repositorioConfiguracaoVeiculo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoVeiculo(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoVeiculo configuracaoVeiculo = repositorioConfiguracaoVeiculo.BuscarConfiguracaoPadrao();

            if (!configuracaoVeiculo.CadastrarVeiculoMotoristaBRK)
                return;

            Repositorio.Embarcador.Transportadores.MotoristaIntegracao repositorioMotoristaIntegracao = new Repositorio.Embarcador.Transportadores.MotoristaIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao integracao = integracoes.FirstOrDefault(o => o.TipoIntegracao.Tipo == TipoIntegracao.BrasilRiskVeiculoMotorista);

            if (integracao != null)
            {
                // Atualizando Motorista
                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.NumeroTentativas = 0;
                integracao.ProblemaIntegracao = "";

                repositorioMotoristaIntegracao.Atualizar(integracao);

                return;
            }

            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repositorioTipoIntegracao.BuscarPorTipo(TipoIntegracao.BrasilRiskVeiculoMotorista);

            if (tipoIntegracao == null)
                return;

            // Inserindo Motorista
            integracao = new Dominio.Entidades.Embarcador.Transportadores.MotoristaIntegracao()
            {
                DataIntegracao = DateTime.Now,
                Motorista = motorista,
                ProblemaIntegracao = "",
                SituacaoIntegracao = SituacaoIntegracao.AgIntegracao,
                TipoIntegracao = tipoIntegracao
            };

            repositorioMotoristaIntegracao.Inserir(integracao);
        }

        #endregion Métodos Privados
    }
}
