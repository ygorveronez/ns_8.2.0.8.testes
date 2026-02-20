using System;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.ICMS
{
    public class RegraICMS
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware; 
        private readonly string _razaoSocial; 

        #endregion

        #region Construtores

        public RegraICMS(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, string razaoSocial)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _razaoSocial = razaoSocial;
        }

        #endregion

        #region Métodos Públicos

        public void AlertarRegraForaDeVigencia()
        {
            Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta repositorioConfiguracaoAlerta = new Repositorio.Embarcador.Notificacoes.ConfiguracaoAlerta(_unitOfWork);
            Repositorio.Embarcador.ICMS.RegraICMS repositorioRegraICMS = new Repositorio.Embarcador.ICMS.RegraICMS(_unitOfWork);

            Dominio.Entidades.Embarcador.Notificacoes.ConfiguracaoAlerta configuracaoAlerta = repositorioConfiguracaoAlerta.BuscarAtivaPorTipo(TipoConfiguracaoAlerta.RegraICMS);

            if ((configuracaoAlerta == null) || (configuracaoAlerta.Usuarios == null) || (configuracaoAlerta.Usuarios.Count == 0))
                return;

            DateTime dataUltimoAlerta = DateTime.Now.Date;
            Embarcador.Notificacao.Notificacao servicoNotificacao = new Embarcador.Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: _tipoServicoMultisoftware, adminStringConexao: string.Empty);
            Embarcador.Notificacao.NotificacaoEmpresa servicoNotificacaoEmpresa = new Embarcador.Notificacao.NotificacaoEmpresa(_unitOfWork);
            Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.FiltroPesquisaAlerta()
            {
                AlertarAposVencimento = configuracaoAlerta.AlertarAposVencimento,
                DiasAlertarAntesVencimento = configuracaoAlerta.DiasAlertarAntesVencimento,
                DiasRepetirAlerta = configuracaoAlerta.DiasRepetirAlerta
            };

            List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> listaRegraICMS = repositorioRegraICMS.BuscarPorVencimentoAlertar(filtrosPesquisa);

            foreach (Dominio.Entidades.Embarcador.ICMS.RegraICMS regraICMS in listaRegraICMS)
            {
                int diasParaVencer = (int)regraICMS.VigenciaFim.Value.Subtract(dataUltimoAlerta).TotalDays;
                string mensagem = string.Empty;

                mensagem = string.Format(Localization.Resources.ICMS.RegraICMS.Embarcador, _razaoSocial) + Environment.NewLine;

                if (diasParaVencer > 0)
                    mensagem += string.Format(Localization.Resources.ICMS.RegraICMS.RegraICMSVence, regraICMS.Descricao, diasParaVencer, (diasParaVencer == 1 ? "" : "s"));
                else if (diasParaVencer == 0)
                    mensagem += string.Format(Localization.Resources.ICMS.RegraICMS.RegraICMSVenceHoje, regraICMS.Descricao);
                else
                    mensagem += string.Format(Localization.Resources.ICMS.RegraICMS.RegraICMSVenceuA, regraICMS.Descricao, (-diasParaVencer), (diasParaVencer == -1 ? "" : "s"));

                foreach (Dominio.Entidades.Usuario usuarioNotificar in configuracaoAlerta.Usuarios)
                {
                    servicoNotificacao.GerarNotificacaoEmail(
                        usuario: usuarioNotificar,
                        usuarioGerouNotificacao: null,
                        codigoObjeto: regraICMS.Codigo,
                        URLPagina: "ICMS/RegraICMS",
                        titulo: Localization.Resources.ICMS.RegraICMS.VencimentoRegraICMS,
                        nota: mensagem,
                        icone: IconesNotificacao.atencao,
                        tipoNotificacao: TipoNotificacao.alerta,
                        tipoServicoMultisoftwareNotificar: _tipoServicoMultisoftware,
                        unitOfWork: _unitOfWork
                    );
                }

                if (configuracaoAlerta.AlertarTransportador)
                {
                    Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa notificacaoEmailEmpresa = new Dominio.ObjetosDeValor.Embarcador.Notificacao.NotificacaoEmpresa()
                    {
                        AssuntoEmail = "Vencimento de Regra de ICMS",
                        CabecalhoMensagem = "Alerta de Regra de ICMS",
                        Empresa = regraICMS.Empresa,
                        Mensagem = mensagem
                    };

                    servicoNotificacaoEmpresa.GerarNotificacaoEmail(notificacaoEmailEmpresa);
                }

                regraICMS.DataUltimoAlertaVencimento = dataUltimoAlerta;

                repositorioRegraICMS.Atualizar(regraICMS);
            }

        }

        #endregion

        #region Métodos Privados

        #endregion
    }
}