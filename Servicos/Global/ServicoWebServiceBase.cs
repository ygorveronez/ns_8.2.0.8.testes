using AdminMultisoftware.Dominio.Entidades.Pessoas;
using Dominio.ObjetosDeValor.Enumerador;
using Repositorio;
using System;
using System.Threading;

namespace Servicos
{
    public class ServicoWebServiceBase : ServicoBase
    {
        #region Construtores
        public ServicoWebServiceBase() : base() { }        
        public ServicoWebServiceBase(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }
        public ServicoWebServiceBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware) : base(unitOfWork, tipoServicoMultisoftware) { }
        public ServicoWebServiceBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware, cancelationToken) { }

        public ServicoWebServiceBase(UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, ClienteURLAcesso clienteURLAcesso, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, CancellationToken cancelationToken = default) : base(unitOfWork, tipoServicoMultisoftware, clienteURLAcesso, clienteMultisoftware, cancelationToken) { }

        #endregion

        #region Métodos Privados

        private void AuditarIntegracao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, string codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao statusRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string nomeMetodo = "")
        {

            try
            {
                Dominio.Entidades.Auditoria.HistoricoIntegracao historicoIntegracao = new Dominio.Entidades.Auditoria.HistoricoIntegracao()
                {
                    CodigoIntegracao = codigoIntegracao,
                    Data = DateTime.Now,
                    Integradora = auditado.Integradora,
                    NomeMetodo = nomeMetodo,
                    Origem = auditado.OrigemAuditado,
                    Retorno = mensagemRetorno,
                    StatusRetorno = statusRetorno,
                };

                new Repositorio.Auditoria.HistoricoIntegracao(unitOfWork).Inserir(historicoIntegracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro($"Falha ao auditar integração do método {nomeMetodo} do WS {auditado.OrigemAuditado.ObterDescricao()}: " + excecao);
            }
        }

        #endregion


        #region Metodo Protegidos
        protected void AuditarRetornoDadosInvalidos(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.DadosInvalidos, auditado, nomeMetodo);
        }

        protected void AuditarRetornoDadosInvalidosCNPJTransportador(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.CNPNaoCadastrado, auditado, nomeMetodo);
        }

        protected void AuditarRetornoDuplicidadeDaRequisicao(Repositorio.UnitOfWork unitOfWork, string mensagemRetorno, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, string codigoIntegracao = "", [System.Runtime.CompilerServices.CallerMemberName] string nomeMetodo = "")
        {
            AuditarIntegracao(unitOfWork, mensagemRetorno, codigoIntegracao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusRetornoRequisicao.DuplicidadeDaRequisicao, auditado, nomeMetodo);
        }
        protected void ArmazenarLogIntegracao(object entidade, Repositorio.UnitOfWork unitOfWork)
        {
            try
            {
                string entidadeJson = Newtonsoft.Json.JsonConvert.SerializeObject(entidade);

                Utilidades.IO.FileStorageService.Storage.WriteAllText(ObterCaminhoArquivoLog(unitOfWork), entidadeJson);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("Falha ao serializar os dados para gerar o log: " + excecao);
            }
        }
        protected virtual string ObterCaminhoArquivoLog(Repositorio.UnitOfWork unitOfWork)
        {
            return Utilidades.IO.FileStorageService.Storage.Combine(Servicos.Embarcador.Configuracoes.ConfigurationInstance.GetInstance(unitOfWork).ObterConfiguracaoArquivo().CaminhoTempArquivosImportacao, Guid.NewGuid() + "_integracaoPedidos.txt");
        }
        #endregion



    }
}
