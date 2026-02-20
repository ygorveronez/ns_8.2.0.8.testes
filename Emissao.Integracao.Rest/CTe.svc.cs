using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using Emissao.Integracao.Rest.Base;
using Emissao.Integracao.Rest.Class;

namespace Emissao.Integracao.Rest
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CTe" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CTe.svc or CTe.svc.cs at the Solution Explorer and start debugging.
    public class CTe : ICTe
    {
        public Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>> BuscarCTePorPeriodo(string DataInicial, string DataFinal, string Status, int Inicio, int Limite)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora == null)
                    return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Acesso não permitido, contate a empresa administradora.", Status = false };

                if (Limite > 500)
                    return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Limte máximo é de 500 registros.", Status = false };

                DateTime.TryParseExact(DataInicial, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataInicial);
                DateTime.TryParseExact(DataFinal, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime dataFinal);

                if (dataInicial <= DateTime.MinValue)
                    return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Obrigatorio uma data inicial no formato dd/MM/yyyy HH:mm", Status = false };

                if (dataFinal <= DateTime.MinValue)
                    return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Obrigatorio uma data inicial no formato dd/MM/yyyy HH:mm", Status = false };

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                if (Limite == 0)
                    Limite = 50;

                int totalRegistros = repCTe.ContarConsultaCTesPorPeriodo(dataInicial, dataFinal, true, Status);
                if (Inicio + Limite > totalRegistros)
                    Limite = totalRegistros - Inicio;

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCTesPorPeriodo(dataInicial, dataFinal, true, Inicio, Limite, Status);

                List<Dominio.ObjetosDeValor.WebService.Rest.CTe> listaCteRetorno = ConverterRetornoCTe(listaCTes, unitOfWork);

                Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe> retorno = new Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>()
                {
                    Itens = listaCteRetorno,
                    NumeroTotalDeRegistro = totalRegistros
                };

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Sucesso", Status = true, Objeto = retorno };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Falha genérica ao consultar dados", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

        public Retorno<string> ConsultarXMLCTe(string Protocolo, string Status)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                if (integradora == null)
                    return new Retorno<string> { Mensagem = "Acesso não permitido, contate a empresa administradora.", Status = false };

                int.TryParse(Protocolo, out int protocoloCTe);

                if (protocoloCTe <= 0)
                    return new Retorno<string> { Mensagem = "Protocolo CTe inválido.", Status = false };

                Repositorio.XMLCTe repXMLCTe = new Repositorio.XMLCTe(unitOfWork);

                Dominio.Enumeradores.TipoXMLCTe tipoXML = Dominio.Enumeradores.TipoXMLCTe.Autorizacao;
                if (!string.IsNullOrWhiteSpace(Status) && (Status == "Cancelado" || Status == "Inutilizado"))
                    tipoXML = Dominio.Enumeradores.TipoXMLCTe.Cancelamento;

                Dominio.Entidades.XMLCTe xmlCTe = repXMLCTe.BuscarPorCTe(protocoloCTe, tipoXML);

                if (xmlCTe == null)
                    return new Retorno<string> { Mensagem = "Não localizado XML para este CTe.", Status = false };

                string xml = string.Empty;
                if (!xmlCTe.XMLArmazenadoEmArquivo)
                    xml = xmlCTe.XML;
                else
                {
                    Servicos.CTe serCTe = new Servicos.CTe(unitOfWork);

                    string caminho = serCTe.ObterCaminhoArmazenamentoXMLCTeArquivo(xmlCTe.CTe, xmlCTe.CTe.Status, unitOfWork);

                    if (Utilidades.IO.FileStorageService.Storage.Exists(caminho))
                        xml = Utilidades.IO.FileStorageService.Storage.ReadAllText(caminho);
                }

                return new Retorno<string>() { Mensagem = "Sucesso", Status = true, Objeto = xml };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<string>() { Mensagem = "Falha genérica ao consultar XML", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        private List<Dominio.ObjetosDeValor.WebService.Rest.CTe> ConverterRetornoCTe(List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes, Repositorio.UnitOfWork unitOfWork)
        {
            List<Dominio.ObjetosDeValor.WebService.Rest.CTe> CTesIntegracao = new List<Dominio.ObjetosDeValor.WebService.Rest.CTe>();

            if (listaCTes.Count > 0)
            {
                foreach (var cte in listaCTes)
                {
                    Dominio.ObjetosDeValor.WebService.Rest.CTe cteIntegracao = new Dominio.ObjetosDeValor.WebService.Rest.CTe();
                    cteIntegracao.ProtocoloCTe = cte.Codigo;
                    cteIntegracao.ChaveCTe = cte.Chave;
                    cteIntegracao.CNPJEmissor = cte.Empresa.CNPJ;
                    cteIntegracao.DataEmissao = cte.DataEmissao.Value.ToString("dd/MM/yyyy HH:mm");
                    cteIntegracao.DataEvento = cte.DataRetornoSefaz.HasValue ? cte.DataRetornoSefaz.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty;
                    cteIntegracao.MensagemSefaz = cte.Status != "A" ? cte.MensagemStatus?.MensagemDoErro ?? cte.MensagemRetornoSefaz : string.Empty;
                    cteIntegracao.SituacaoCTe = cte.DescricaoStatus;
                    cteIntegracao.ValorCTe = cte.ValorAReceber;
                    cteIntegracao.NumeroCarga = cte.CargaCTes?.FirstOrDefault().Carga.CodigoCargaEmbarcador ?? string.Empty;

                    CTesIntegracao.Add(cteIntegracao);
                }
            }

            return CTesIntegracao;

        }

        private Dominio.Entidades.WebService.Integradora ValidarToken()
        {
            using (Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(
                Conexao.StringConexao,
                Dominio.ObjetosDeValor.Enumerador.TipoSessaoBancoDados.Nova))
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection headers = request.Headers;
                string tokenBaerer = headers["Authorization"];
                if (string.IsNullOrWhiteSpace(tokenBaerer))
                    return null;

                Repositorio.WebService.Integradora repIntegracadora = new Repositorio.WebService.Integradora(unitOfWork);
                Dominio.Entidades.WebService.Integradora integradora = repIntegracadora.BuscarPorTokenIntegracao(tokenBaerer);

                return integradora;
            }
        }

        public Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>> BuscarCTePeriodoAnterior()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                //Servicos.Log.TratarErro(AvroConvert.GenerateSchema(typeof(Dominio.ObjetosDeValor.WebService.Rest.CTe)), "CTe");

                //Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

                //if (integradora == null)
                    //return new Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Acesso não permitido, contate a empresa administradora.", Status = false };

                DateTime dataFinal = DateTime.Now.Date.AddDays(-1);
                DateTime dataInicial = DateTime.Now.Date.AddDays(-1);

                if (dataFinal <= DateTime.MinValue)
                    return new Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Obrigatorio uma data inicial no formato dd/MM/yyyy HH:mm", Status = false };

                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

                int limite = 6000;
                int inicio = 0;

                int totalRegistros = repCTe.ContarConsultaCTesPorPeriodo(dataInicial, dataFinal, true, string.Empty);

                List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> listaCTes = repCTe.ConsultarCTesPorPeriodo(dataInicial, dataFinal, true, inicio, limite, string.Empty);

                List<Dominio.ObjetosDeValor.WebService.Rest.CTe> listaCteRetorno = ConverterRetornoCTe(listaCTes, unitOfWork);

                return new Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Sucesso", Status = true, Objeto = listaCteRetorno };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<List<Dominio.ObjetosDeValor.WebService.Rest.CTe>>() { Mensagem = "Falha genérica ao consultar dados", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }

    }
}
