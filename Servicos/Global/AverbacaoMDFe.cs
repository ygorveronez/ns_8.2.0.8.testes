using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Servicos
{
    public class AverbacaoMDFe : ServicoBase
    {        
        public AverbacaoMDFe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public bool ConsultarAverbacoes(int codigoMDFe, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unidadeDeTrabalho);

            Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigo(codigoMDFe);

            if (mdfe == null)
                throw new Exception("MDF-e é nulo para a consulta de averbações.");

            Repositorio.AverbacaoMDFe repAverbacao = new Repositorio.AverbacaoMDFe(unidadeDeTrabalho);

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            ServicoMDFe.RetornoAverbacaoMDFeBradesco retorno = svcMDFe.ConsultarAverbacoesMDFeBradesco(mdfe.CodigoIntegradorAutorizacao);

            if (retorno.Info.Tipo == "ERRO")
                return false;
                
            List<Dominio.Entidades.AverbacaoMDFe> averbacoesExistentes = repAverbacao.BuscarPorMDFe(mdfe.Empresa.Codigo, mdfe.Codigo);

            foreach (ServicoMDFe.AverbacaoMDFeBradesco averbacaoIntegrada in retorno.Averbacoes)
            {
                var averbacao = (from obj in averbacoesExistentes where obj.CodigoIntegracao == averbacaoIntegrada.CodigoAverbacao select obj).FirstOrDefault();

                if (averbacao == null)
                    averbacao = new Dominio.Entidades.AverbacaoMDFe();

                averbacao.CodigoIntegracao = averbacaoIntegrada.CodigoAverbacao;
                averbacao.CodigoRetorno = averbacaoIntegrada.CodigoRetorno;
                averbacao.MDFe = mdfe;
                averbacao.DataRetorno = averbacaoIntegrada.DataProtocolo;
                averbacao.MensagemRetorno = averbacaoIntegrada.MensagemRetorno;
                averbacao.Protocolo = averbacaoIntegrada.NumeroProtocolo;
                averbacao.Status = averbacaoIntegrada.Status == "A" ? Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso : averbacaoIntegrada.Status == "I" ? Dominio.Enumeradores.StatusAverbacaoMDFe.Pendente : Dominio.Enumeradores.StatusAverbacaoMDFe.Rejeicao;
                averbacao.Tipo = averbacaoIntegrada.Tipo == "A" ? Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao : averbacaoIntegrada.Tipo == "E" ? Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento: Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento;
                averbacao.SeguradoraAverbacao = averbacaoIntegrada.Seguradora == "A" ? Dominio.Enumeradores.IntegradoraAverbacao.ATM : averbacaoIntegrada.Seguradora == "B" ? Dominio.Enumeradores.IntegradoraAverbacao.Quorum : averbacaoIntegrada.Seguradora == "P" ? Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro : Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                averbacao.Averbacao = averbacaoIntegrada.Averbacao;
                if (string.IsNullOrWhiteSpace(averbacao.Averbacao) && averbacao.Status == Dominio.Enumeradores.StatusAverbacaoMDFe.Sucesso && averbacao.Tipo == Dominio.Enumeradores.TipoAverbacaoMDFe.Autorizacao)
                    averbacao.Averbacao = averbacaoIntegrada.NumeroProtocolo;

                if (averbacao.Codigo > 0)
                    repAverbacao.Atualizar(averbacao);
                else
                    repAverbacao.Inserir(averbacao);
            }              

            if ((from obj in retorno.Averbacoes where obj.Status == "I" select obj).Any())
                return false;
            else
                return true;
        }

        public bool Emitir(Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Dominio.Enumeradores.TipoAverbacaoMDFe tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            if (mdfe == null)
                throw new Exception("O MDF-e não pode ser nulo para emitir uma averbação.");

            ServicoMDFe.uMDFeServiceTSSoapClient svcMDFe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoMDFe.uMDFeServiceTSSoapClient, ServicoMDFe.uMDFeServiceTSSoap>(Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoWebServiceIntegracao.Oracle_uMDFeServiceTS);

            string seguradora = mdfe.Empresa.Configuracao != null && mdfe.Empresa.Configuracao.SeguradoraAverbacao != null ?
                                mdfe.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" :
                                mdfe.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" :
                                mdfe.Empresa.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : string.Empty : string.Empty;
            if (string.IsNullOrWhiteSpace(seguradora))
                seguradora = mdfe.Empresa.EmpresaPai.Configuracao != null && mdfe.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao != null ?
                             mdfe.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.Quorum ? "B" :
                             mdfe.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.PortoSeguro ? "P" :
                             mdfe.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao == Dominio.Enumeradores.IntegradoraAverbacao.ATM ? "A" : "" : "";

            string usuarioATM = !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.UsuarioSeguroATM) ? mdfe.Empresa.Configuracao.UsuarioSeguroATM : mdfe.Empresa.EmpresaPai?.Configuracao?.UsuarioSeguroATM;
            string senhaATM = !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.SenhaSeguroATM) ? mdfe.Empresa.Configuracao.SenhaSeguroATM : mdfe.Empresa.EmpresaPai?.Configuracao?.SenhaSeguroATM;
            string codigoATM = !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.CodigoSeguroATM) ? mdfe.Empresa.Configuracao.CodigoSeguroATM : mdfe.Empresa.EmpresaPai?.Configuracao?.CodigoSeguroATM;
            string tokenBradesco = !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.TokenAverbacaoBradesco) ? mdfe.Empresa.Configuracao.TokenAverbacaoBradesco : mdfe.Empresa.EmpresaPai?.Configuracao?.TokenAverbacaoBradesco;
            string wsdlQuorum = !string.IsNullOrWhiteSpace(mdfe.Empresa.Configuracao.WsdlAverbacaoQuorum) ? mdfe.Empresa.Configuracao.WsdlAverbacaoQuorum : mdfe.Empresa.EmpresaPai?.Configuracao?.WsdlAverbacaoQuorum;

            if (tipo == Dominio.Enumeradores.TipoAverbacaoMDFe.Cancelamento)
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarAverbacaoMDFeBradesco(mdfe.CodigoIntegradorAutorizacao, ServicoMDFe.TipoAverbacaoMDFe.Cancelamento, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, wsdlQuorum);

                if (retorno.Valor > 0)
                    return true;
                else
                    return false;
            }
            else if (tipo == Dominio.Enumeradores.TipoAverbacaoMDFe.Encerramento)
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarAverbacaoMDFeBradesco(mdfe.CodigoIntegradorAutorizacao, ServicoMDFe.TipoAverbacaoMDFe.Encerramento, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, wsdlQuorum);

                if (retorno.Valor > 0)
                    return true;
                else
                    return false;
            } else
            {
                ServicoMDFe.ResultadoInteger retorno = svcMDFe.ImportarAverbacaoMDFeBradesco(mdfe.CodigoIntegradorAutorizacao, ServicoMDFe.TipoAverbacaoMDFe.Autorizacao, seguradora, codigoATM, usuarioATM, senhaATM, tokenBradesco, wsdlQuorum);

                if (retorno.Valor > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}
