using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Frete
{
    public class ConfiguracaoDescargaCliente : RegraAutorizacao.AprovacaoAlcada
    <
        Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente,
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga,
        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga
    >
    {
        #region Construtores

        public ConfiguracaoDescargaCliente(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Privados       

        private void CriarRegrasAprovacao(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga, List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> regras, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            bool existeRegraSemAprovacao = false;
            Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga repositorio = new Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga(_unitOfWork);
            Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral repConfiguracaoGeral = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeral(_unitOfWork);

            int menorPrioridadeAprovacao = regras.Where(regra => regra.NumeroAprovadores > 0).Select(regra => (int?)regra.PrioridadeAprovacao).Min() ?? 0;

            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeral configuracaoGeral = repConfiguracaoGeral.BuscarConfiguracaoPadrao();

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga regra in regras)
            {
                if (regra.NumeroAprovadores > 0)
                {
                    existeRegraSemAprovacao = true;

                    foreach (Dominio.Entidades.Usuario aprovador in regra.Aprovadores)
                    {
                        Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga()
                        {
                            OrigemAprovacao = taxaDescarga,
                            Bloqueada = regra.PrioridadeAprovacao > menorPrioridadeAprovacao,
                            Usuario = aprovador,
                            RegraAutorizacao = regra,
                            Situacao = SituacaoAlcadaRegra.Pendente,
                            DataCriacao = taxaDescarga.FimVigencia,
                            NumeroAprovadores = regra.NumeroAprovadores
                        };

                        repositorio.Inserir(aprovacao);               

                        if (!aprovacao.Bloqueada && !(configuracaoGeral?.EnviarApenasEmailDiarioTaxasDescargaPendenteAprovacao ?? false))
                            NotificarAprovador(taxaDescarga, aprovacao, tipoServicoMultisoftware);
                    }
                }
                else
                {
                    Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga aprovacao = new Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga()
                    {
                        OrigemAprovacao = taxaDescarga,
                        Usuario = null,
                        RegraAutorizacao = regra,
                        Situacao = SituacaoAlcadaRegra.Aprovada,
                        Data = DateTime.Now,
                        Motivo = $"Alçada aprovada pela Regra {regra.Descricao}",
                        DataCriacao = taxaDescarga.FimVigencia
                    };

                    repositorio.Inserir(aprovacao);
                }
            }

            taxaDescarga.Situacao = existeRegraSemAprovacao ? SituacaoAjusteConfiguracaoDescargaCliente.AgAprovacao : SituacaoAjusteConfiguracaoDescargaCliente.Aprovada;
        }

        private List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> ObterRegrasAutorizacao(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga)
        {
            Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> repositorioRegraAutorizacao = new Repositorio.Embarcador.RegraAutorizacao.RegraAutorizacaoPadrao<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> listaRegras = repositorioRegraAutorizacao.BuscarPorAtiva();
            List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> listaRegrasFiltradas = new List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga>();
            decimal valorDescarga = taxaDescarga.Valor;

            foreach (Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga regra in listaRegras)
            {
                //if (regra.RegraPorTransportador && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTransportador, Dominio.Entidades.Empresa>(regra.AlcadasTransportador, taxaDescarga.Emp)) //O campo transportador não existe na tabela de ConfiguracaoDescarcaCliente..;               

                if (regra.RegraPorTipoOperacao && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoOperacao, Dominio.Entidades.Embarcador.Pedidos.TipoOperacao>(regra.AlcadasTipoOperacao, taxaDescarga?.TiposOperacoes?.Select(o => o.Codigo).ToList()))
                    continue;

                if (regra.RegraPorFilial && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaFilial, Dominio.Entidades.Embarcador.Filiais.Filial>(regra.AlcadasFilial, taxaDescarga.Filial?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorValor && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaValor, decimal>(regra.AlcadasValor, valorDescarga))
                    continue;

                if (regra.RegraPorTipoDeCarga && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaTipoDeCarga, Dominio.Entidades.Embarcador.Cargas.TipoDeCarga>(regra.AlcadasTipoDeCarga, taxaDescarga.TipoCarga?.Codigo ?? 0))
                    continue;

                if (regra.RegraPorCliente && !ValidarAlcadas<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AlcadaCliente, Dominio.Entidades.Cliente>(regra.AlcadasCliente, taxaDescarga.Clientes.Select(o => o.CPF_CNPJ).ToList()))
                    continue;

                listaRegrasFiltradas.Add(regra);
            }

            return listaRegrasFiltradas;
        }

        private void EnviarEmailAprovadores(List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> taxasDescargasPendentes)
        {
            Servicos.Email servicoEmail = new Servicos.Email(_unitOfWork);

            Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga repAprovacaoAlcadaTaxaDescarga = new Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga(_unitOfWork);
            Repositorio.Embarcador.Email.ConfigEmailDocTransporte repositorioConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(_unitOfWork);

            Dominio.Entidades.Embarcador.Email.ConfigEmailDocTransporte configuracaoEmail = repositorioConfigEmailDocTransporte.BuscarEmailEnviaDocumentoAtivo();

            List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga> aprovacoesPendentes = repAprovacaoAlcadaTaxaDescarga.BuscarAprovacoesPendentesPorTaxasDescarga(taxasDescargasPendentes.Select(o => o.Codigo).ToList());
            List<Dominio.Entidades.Usuario> aprovadores = aprovacoesPendentes.Select(o => o?.Usuario).Distinct().ToList();

            foreach (Dominio.Entidades.Usuario aprovador in aprovadores)
            {
                List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> taxasDoAprovador = aprovacoesPendentes.Where(o => o.Usuario.Codigo == aprovador.Codigo).Select(o => o.OrigemAprovacao).ToList();

                if (taxasDoAprovador.Count == 0)
                    continue;

                string titulo = "Tarifas de descarga para aprovar";

                StringBuilder mensagem = new StringBuilder();
                mensagem.AppendLine($"Olá, {aprovador.Nome}");
                mensagem.AppendLine();
                mensagem.AppendLine("Você possui taxa(s) de descarga pendente(s) de aprovação: ");
                mensagem.AppendLine();
                if (taxasDoAprovador.Count == 1)
                    mensagem.AppendLine($"Existe uma tarifa de descarga pendente de aprovação.");
                else
                    mensagem.AppendLine($"Existem {taxasDoAprovador.Count} tarifas de descarga pendentes de aprovação.");
                mensagem.AppendLine();
                mensagem.AppendLine("Envio de e-mail automático. Favor não responder");

                servicoEmail.EnviarEmail(configuracaoEmail.Email, configuracaoEmail.Email, configuracaoEmail.Senha, aprovador.Email, null, null, titulo, mensagem.ToString(), configuracaoEmail.Smtp, null, null, configuracaoEmail.RequerAutenticacaoSmtp, "", configuracaoEmail.PortaSmtp, _unitOfWork);
            }
        }

        #endregion Métodos Privados 

        #region Métodos Protegidos Sobrescritos

        protected override void NotificarAprovador(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga, Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga aprovacao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Notificacao.Notificacao servicoNotificacao = new Notificacao.Notificacao(_unitOfWork.StringConexao, cliente: null, tipoServicoMultisoftware: tipoServicoMultisoftware, adminStringConexao: string.Empty);

            servicoNotificacao.GerarNotificacaoEmail(
                usuario: aprovacao.Usuario,
                usuarioGerouNotificacao: null,
                codigoObjeto: taxaDescarga.Codigo,
                URLPagina: "Frete/ConfiguracaoDescargaCliente",
                titulo: Localization.Resources.Fretes.ConfiguracaoDescargaCliente.TituloConfiguracaoDescargaCliente,
                nota: string.Format(Localization.Resources.Fretes.ConfiguracaoDescargaCliente.CriadaSolicitacaoAprovacaoTaxaDescarga, taxaDescarga.Descricao),
                icone: IconesNotificacao.cifra,
                tipoNotificacao: TipoNotificacao.credito,
                tipoServicoMultisoftwareNotificar: tipoServicoMultisoftware,
                unitOfWork: _unitOfWork
            );
        }

        #endregion

        #region Métodos Públicos

        public void CriarAprovacao(Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente taxaDescarga, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador)
                return;

            RemoverAprovacao(taxaDescarga);

            List<Dominio.Entidades.Embarcador.Frete.AlcadasTaxaDescarga.RegraAutorizacaoTaxaDescarga> regras = ObterRegrasAutorizacao(taxaDescarga);

            if (regras.Count > 0)
                CriarRegrasAprovacao(taxaDescarga, regras, tipoServicoMultisoftware);
            else
                taxaDescarga.Situacao = SituacaoAjusteConfiguracaoDescargaCliente.SemRegraAprovacao;
        }

        public void VerificarTarifasPendentesAprovacao()
        {
            Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga repAprovacaoAlcadaTaxaDescarga = new Repositorio.Embarcador.Frete.AlcadasTaxaDescarga.AprovacaoAlcadaTaxaDescarga(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Frete.ConfiguracaoDescargaCliente> taxasDescargasPendentes = repAprovacaoAlcadaTaxaDescarga.ObterPendentes();

            if (taxasDescargasPendentes.Count > 0)
                EnviarEmailAprovadores(taxasDescargasPendentes);
        }

        #endregion  Métodos Públicos
    }
}
