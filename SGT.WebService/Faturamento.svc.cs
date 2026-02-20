using System;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Faturamento(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IFaturamento
    {
        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>> BuscarDocumentosPagamentoLiberado(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno? tipoDocumentoRetorno, int? inicio, int? limite)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>>.CreateFrom(new Servicos.WebService.Faturamento.Faturamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarDocumentosPagamentoLiberado(tipoDocumentoRetorno ?? Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.Nenhum, inicio ?? 0, limite ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Financeiro.DocumentoFaturamento>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> ConfirmarIntegracaoDocumentoFaturamento(int? protocolo)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Faturamento.Faturamento(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoDocumentoFaturamento(protocolo ?? 0));
            });
        }

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceFaturamento;
        }

        #endregion

    }
}
