using System.Collections.Generic;
using System.Linq;
using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos.Embarcador.Carga
{
    public sealed class MensagemAlertaCarga : Alertas.MensagemAlerta<Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga, Dominio.Entidades.Embarcador.Cargas.Carga>
    {
        #region Atributos

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga _configuracaoGeralCarga;

        #endregion Atributos

        #region Construtores

        public MensagemAlertaCarga(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, auditado: null, configuracaoGeralCarga: null) { }

        public MensagemAlertaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : this(unitOfWork, auditado, configuracaoGeralCarga: null) { }

        public MensagemAlertaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga) : this(unitOfWork, auditado: null, configuracaoGeralCarga) { }

        public MensagemAlertaCarga(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga) : base(unitOfWork, auditado)
        {
            _configuracaoGeralCarga = configuracaoGeralCarga;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga ObterConfiguracaoGeralCarga()
        {
            if (_configuracaoGeralCarga == null)
                _configuracaoGeralCarga = new Repositorio.Embarcador.Configuracoes.ConfiguracaoGeralCarga(_unitOfWork).BuscarPrimeiroRegistro();

            return _configuracaoGeralCarga;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public override List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> ObterMensagensPorEntidades(List<int> codigosCargas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta> mensagensAlertaRetornar = base.ObterMensagensPorEntidades(codigosCargas);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = ObterConfiguracaoGeralCarga();

            if (configuracaoGeralCarga.ExibirMensagemAlertaPrevisaoEntregaNaMesmaData)
            {
                if (codigosCargas.Count == 0)
                    return mensagensAlertaRetornar;

                Repositorio.Embarcador.Cargas.Carga repositorioCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
                IList<Dominio.ObjetosDeValor.Embarcador.Carga.MensagemAlertaAdicional> mensagensPedidosComPrevisaoEntregaEDestinatarioIguais = repositorioCarga.BuscarMensagensPedidosComPrevisaoEntregaEDestinatarioIguais(codigosCargas);
                List<int> codigosCargasPedidosComPrevisaoEntregaEDestinatarioIguais = mensagensPedidosComPrevisaoEntregaEDestinatarioIguais.Select(o => o.CodigoCarga).Distinct().ToList();

                foreach (int codigoCarga in codigosCargasPedidosComPrevisaoEntregaEDestinatarioIguais)
                {
                    List<string> mensagensAlertaAdicionaisPorCarga = mensagensPedidosComPrevisaoEntregaEDestinatarioIguais.Where(o => o.CodigoCarga == codigoCarga).Select(o => o.Mensagem).ToList();

                    mensagensAlertaRetornar.Add(new Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta()
                    {
                        Codigo = 0,
                        CodigoEntidade = codigoCarga,
                        Mensagens = mensagensAlertaAdicionaisPorCarga,
                        Tipo = 0,
                        Titulo = "Previsão de Entrega na Mesma Data"
                    });
                }
            }

            return mensagensAlertaRetornar;
        }

        public Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta ObterMensagemAlertaBase(Dominio.Entidades.Embarcador.Cargas.MensagemAlertaCarga alerta)
        {
            if (alerta == null) return null;

            Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta mensagemAlerta = new Dominio.ObjetosDeValor.Embarcador.Alertas.MensagemAlerta()
            {
                Bloquear = alerta.Bloquear,
                Codigo = alerta.Codigo,
                CodigoEntidade = alerta.Entidade.Codigo,
                Mensagens = alerta.Mensagens.ToList(),
                Tipo = alerta.Tipo,
                Titulo = alerta.Titulo,
                UtilizarConfirmacao = alerta.UtilizarConfirmacao
            };

            return mensagemAlerta;
        }

        #endregion Métodos Públicos
    }
}