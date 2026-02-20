using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Servicos
{
    public class AverbacaoNFSe : ServicoBase
    {        
        public AverbacaoNFSe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        public void Averbar(Dominio.Entidades.NFSe nfse, Dominio.Enumeradores.TipoAverbacaoCTe tipo, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.AverbacaoNFSe repAverbacaoNFSe = new Repositorio.AverbacaoNFSe(unidadeDeTrabalho);
            Repositorio.ConfiguracaoAverbacaoClientes repAverbacao = new Repositorio.ConfiguracaoAverbacaoClientes(unidadeDeTrabalho);
            List<Dominio.Entidades.ConfiguracaoAverbacaoClientes> configAverbacao = repAverbacao.BuscarPorConfiguracao(nfse.Empresa.Configuracao.Codigo);

            if (nfse == null)
                throw new Exception("NFSe não pode ser nula para emitir uma averbação.");

            if (nfse.Empresa.Configuracao == null)
                throw new Exception("Empresa sem configuração para averbação");

            Dominio.Enumeradores.IntegradoraAverbacao? seguradora = nfse.Empresa.Configuracao.SeguradoraAverbacao != null ? nfse.Empresa.Configuracao.SeguradoraAverbacao : null;

            if (seguradora == null)
                seguradora = nfse.Empresa.EmpresaPai.Configuracao != null && nfse.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao != null ? nfse.Empresa.EmpresaPai.Configuracao.SeguradoraAverbacao : null;

            string usuarioATM = !string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.UsuarioSeguroATM) ? nfse.Empresa.Configuracao.UsuarioSeguroATM : nfse.Empresa.EmpresaPai?.Configuracao?.UsuarioSeguroATM;
            string senhaATM = !string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.SenhaSeguroATM) ? nfse.Empresa.Configuracao.SenhaSeguroATM : nfse.Empresa.EmpresaPai?.Configuracao?.SenhaSeguroATM;
            string codigoATM = !string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.CodigoSeguroATM) ? nfse.Empresa.Configuracao.CodigoSeguroATM : nfse.Empresa.EmpresaPai?.Configuracao?.CodigoSeguroATM;
            string tokenBradesco = !string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.TokenAverbacaoBradesco) ? nfse.Empresa.Configuracao.TokenAverbacaoBradesco : nfse.Empresa.EmpresaPai?.Configuracao?.TokenAverbacaoBradesco;
            string wsdlQuorum = !string.IsNullOrWhiteSpace(nfse.Empresa.Configuracao.WsdlAverbacaoQuorum) ? nfse.Empresa.Configuracao.WsdlAverbacaoQuorum : nfse.Empresa.EmpresaPai?.Configuracao?.WsdlAverbacaoQuorum;

            if (configAverbacao != null && configAverbacao.Count > 0)
            {
                for (var i = 0; i < configAverbacao.Count; i++)
                {
                    if (!configAverbacao[i].NaoAverbar) //Somente configuradas para Averbar
                    {
                        if ((nfse.Tomador != null && !string.IsNullOrWhiteSpace(nfse.Tomador.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && nfse.Tomador.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (nfse.Tomador != null && !string.IsNullOrWhiteSpace(nfse.Tomador.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && nfse.Tomador.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                        {
                            seguradora = configAverbacao[i].IntegradoraAverbacao;
                            usuarioATM = configAverbacao[i].UsuarioAverbacao;
                            senhaATM = configAverbacao[i].SenhaAverbacao;
                            codigoATM = configAverbacao[i].CodigoAverbacao;
                            tokenBradesco = configAverbacao[i].TokenAverbacao;
                        }
                    }
                }
            }

            if (configAverbacao != null && configAverbacao.Count > 0)
            {
                for (var i = 0; i < configAverbacao.Count; i++)
                {
                    if (configAverbacao[i].NaoAverbar) 
                    {
                        if ((nfse.Tomador != null && !string.IsNullOrWhiteSpace(nfse.Tomador.CPF_CNPJ) && !configAverbacao[i].RaizCNPJ && nfse.Tomador.CPF_CNPJ.Equals(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato)) || (nfse.Tomador != null && !string.IsNullOrWhiteSpace(nfse.Tomador.CPF_CNPJ) && configAverbacao[i].RaizCNPJ && nfse.Tomador.CPF_CNPJ.Contains(Utilidades.String.OnlyNumbers(configAverbacao[i].Cliente.CPF_CNPJ_SemFormato).Remove(8, 6))))
                        {
                            Dominio.Entidades.AverbacaoNFSe averbacaoNFSe = new Dominio.Entidades.AverbacaoNFSe();
                            averbacaoNFSe.NFSe = nfse;
                            averbacaoNFSe.IntegradoraAverbacao = Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido;
                            averbacaoNFSe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                            averbacaoNFSe.MensagemRetorno = "Tomador da NFSe configurado para não averbar.";
                            repAverbacaoNFSe.Atualizar(averbacaoNFSe);

                            return;
                        }
                    }
                }
            }

            if (seguradora != null && seguradora != Dominio.Enumeradores.IntegradoraAverbacao.NaoDefinido)
            {
                Dominio.Entidades.AverbacaoNFSe averbacaoNFSe = new Dominio.Entidades.AverbacaoNFSe();
                averbacaoNFSe.NFSe = nfse;
                averbacaoNFSe.IntegradoraAverbacao = seguradora.Value;
                averbacaoNFSe.CodigoUsuario = codigoATM;
                averbacaoNFSe.Usuario = usuarioATM;
                averbacaoNFSe.Senha = senhaATM;
                averbacaoNFSe.Token = tokenBradesco;
                averbacaoNFSe.Tipo = tipo;

                averbacaoNFSe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Pendente;
                repAverbacaoNFSe.Inserir(averbacaoNFSe);

                if (seguradora != Dominio.Enumeradores.IntegradoraAverbacao.ATM)
                {
                    averbacaoNFSe.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacaoNFSe.MensagemRetorno = "Integradora não disponível para averbar NFSe";                    
                    repAverbacaoNFSe.Atualizar(averbacaoNFSe);
                }
                else if (averbacaoNFSe.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao)
                {
                    Servicos.Embarcador.Integracao.ATM.ATMIntegracao.AverbarNFSe(averbacaoNFSe, unidadeDeTrabalho);
                }
                else if (averbacaoNFSe.Tipo == Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento)
                {
                    Servicos.Embarcador.Integracao.ATM.ATMIntegracao.CancelarNFSe(averbacaoNFSe, unidadeDeTrabalho);
                }
            }
        }
    }
}
