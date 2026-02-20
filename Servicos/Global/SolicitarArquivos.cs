using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Servicos
{
    public class SolicitarArquivos : ServicoBase
    {        
        public SolicitarArquivos(Repositorio.UnitOfWork unitOfWork): base(unitOfWork) { }

        public bool Solicitar(string cnpjEmpresa, int mes, int ano, int tipoArquivo, string emails, Repositorio.UnitOfWork unidadeDeTrabalho)
        {

            ServicoCTe.uCteServiceTSSoapClient svcCTe = new Servicos.Embarcador.Integracao.ConfiguracaoWebService(unidadeDeTrabalho).ObterClient<ServicoCTe.uCteServiceTSSoapClient, ServicoCTe.uCteServiceTSSoap>(TipoWebServiceIntegracao.Oracle_uCTeServiceTS);

            ServicoCTe.ResultadoString retorno = svcCTe.SolicitarArquivos(cnpjEmpresa, mes, ano, tipoArquivo == 1 ? ServicoCTe.TipoSolicitacao.RelCTePDF : 
                                                                                                  tipoArquivo == 2 ? ServicoCTe.TipoSolicitacao.RelCTeExcel : 
                                                                                                  tipoArquivo == 3 ? ServicoCTe.TipoSolicitacao.XMLCTe :
                                                                                                  tipoArquivo == 5 ? ServicoCTe.TipoSolicitacao.XMLMDFe:
                                                                                                  ServicoCTe.TipoSolicitacao.RelCTePDF,
                                                                                                  emails);

            if (retorno.Valor != "")
                return true;
            else
            {
                Servicos.Log.TratarErro(retorno.Info.MensagemOriginal);
                return false;
            }
        }
    }
}
