using System;
using Dominio.ObjetosDeValor.Embarcador.Terceiros;
using System.Collections.Generic;
using CoreWCF;
using Microsoft.AspNetCore.Http;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class ContratoFreteTerceiro(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IContratoFreteTerceiro
    {
        #region Métodos Públicos

        public Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>> BuscarContratosFretePendentesIntegracao(string dataInicial, string dataFinal, int? inicio, int? quantidadeRegistros)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Terceiros.ContratoFreteTerceiro>>.CreateFrom(new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarContratosFretePendentesIntegracao(dataInicial, dataFinal, inicio ?? 0, quantidadeRegistros ?? 0));
            });
        }
        
        public Retorno<bool> ConfirmarIntegracaoContratoFrete(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoContratoFrete(protocolos));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoAutorizacaoPagamento> AutorizarPagamento(AutorizacaoPagamento autorizacaoPagamento)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<RetornoAutorizacaoPagamento>.CreateFrom(new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).AutorizarPagamento(autorizacaoPagamento));
            });
        }

        public Retorno<Dominio.ObjetosDeValor.Embarcador.Terceiros.RetornoEncerramentoCIOT> EncerrarCIOTPeloContratoTerfeito(int? protocoloContratoTerceiro)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<RetornoEncerramentoCIOT>.CreateFrom(new Servicos.Embarcador.Terceiros.ContratoFrete(unitOfWork, Auditado, TipoServicoMultisoftware, Cliente, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).EncerrarCIOTPeloContratoTerfeito(protocoloContratoTerceiro ?? 0));
            });
        }

        #endregion

        #region Métodos Protegidos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebSContratoFreteTerceiro;
        }

        #endregion
    }
}
