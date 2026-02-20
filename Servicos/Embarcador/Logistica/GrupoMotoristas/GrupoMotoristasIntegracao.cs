using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Logistica.GrupoMotoristas
{
    public class GrupoMotoristasIntegracao : Abstrato.ServicoAbstratoGrupoMotoristas
    {
        #region Construtores
        public GrupoMotoristasIntegracao(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado Auditado = null)
            : base(unitOfWork, cancelationToken, Auditado) { }

        #endregion

        public async Task CriarGrupoMotoristasIntegracoes(List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasTipoIntegracao> grupoMotoristasTipoIntegracoes, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracaoGrupoMotorista tipo)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repGrupoMotoristasIntegracao = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new(_unitOfWork, _cancellationToken);

            List<Dominio.Entidades.Embarcador.Cargas.TipoIntegracao> entidadesTipoIntegracao = await repTipoIntegracao.BuscarPorTiposAsync(
                grupoMotoristasTipoIntegracoes.Select(x => x.TipoIntegracao).ToList()
            );

            List<Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao> grupoMotoristasIntegracoes = new();

            for (int i = 0; i < grupoMotoristasTipoIntegracoes.Count; i++)
            {
                grupoMotoristasIntegracoes.Add(new()
                {
                    GrupoMotoristas = grupoMotoristasTipoIntegracoes[i].GrupoMotoristas,
                    TipoIntegracao = entidadesTipoIntegracao[i],
                    DataIntegracao = DateTime.Now,
                    SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao,
                    NumeroTentativas = 0,
                    ProblemaIntegracao = string.Empty,
                    Tipo = tipo
                });
            }

            await repGrupoMotoristasIntegracao.InserirMuitosAsync(grupoMotoristasIntegracoes);
        }

        public async Task AplicarReenvio(Dominio.Entidades.Embarcador.Logistica.GrupoMotoristasIntegracao integracao)
        {
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristas repGrupoMotoristas = new(_unitOfWork, _cancellationToken);
            Repositorio.Embarcador.Logistica.GrupoMotoristas.GrupoMotoristasIntegracao repGrupoMotoristasIntegracao = new(_unitOfWork, _cancellationToken);

            integracao.GrupoMotoristas.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracaoGrupoMotoristas.AguardandoIntegracoes;
            integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.AgIntegracao;

            await Auditoria.Auditoria.AuditarAsync(_auditado, integracao, null, "Reenviou a Integração.", _unitOfWork);
            await Auditoria.Auditoria.AuditarAsync(_auditado, integracao.GrupoMotoristas, null, "Reenviou a Integração.", _unitOfWork);
            await repGrupoMotoristasIntegracao.AtualizarAsync(integracao);
            await repGrupoMotoristas.AtualizarAsync(integracao.GrupoMotoristas);
        }
    }
}
