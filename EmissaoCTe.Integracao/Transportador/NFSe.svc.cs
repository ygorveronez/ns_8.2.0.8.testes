using System;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.Integracao.Transportador
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "NFSe" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select NFSe.svc or NFSe.svc.cs at the Solution Explorer and start debugging.
    public class NFSe : INFSe
    {
        public Retorno<List<int>> ObterProtocolos(string cnpj, string token, string dataInicial, string dataFinal)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa == null)
                    return new Retorno<List<int>>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<int>>() { Mensagem = "Token de acesso inválido.", Status = false };

                DateTime dtInicial, dtFinal;
                DateTime.TryParseExact(dataInicial, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtInicial);
                DateTime.TryParseExact(dataFinal, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dtFinal);

                if ((dtInicial == DateTime.MinValue || dtFinal == DateTime.MinValue))
                    return new Retorno<List<int>>() { Mensagem = "Deve ser enviado uma Data Inicial e Final ou um número de série.", Status = false };

                if (dtInicial == DateTime.MinValue)
                    return new Retorno<List<int>>() { Mensagem = "Data inicial inválida (formato: dd/MM/yyyy). Quando não enviado um período deve ser enviado a série.", Status = false };

                if (dtFinal == DateTime.MinValue)
                    return new Retorno<List<int>>() { Mensagem = "Data final inválida (formato: dd/MM/yyyy). Quando não enviado um período deve ser enviado a série", Status = false };

                if (dtInicial > DateTime.MinValue && dtFinal > DateTime.MinValue && (dtFinal - dtInicial).TotalDays > 45)
                    return new Retorno<List<int>>() { Mensagem = "Período inválido (máximo permitido: 45 dias).", Status = false };

                List<int> protocolos = repNFSe.BuscarListaProtocolos(empresa.Codigo, dtInicial, dtFinal);

                return new Retorno<List<int>>() { Status = true, Mensagem = "Protocolos obtidos com sucesso.", Objeto = protocolos.Distinct().ToList() };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterProtocolosNFSe");                

                return new Retorno<List<int>>() { Mensagem = "Ocorreu uma falha ao obter os protocolos dos CT-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoNFSe>> ObterXML(string cnpj, string token, List<int> protocolos)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.NFSe repNFSe = new Repositorio.NFSe(unidadeDeTrabalho);
                Repositorio.XMLNFSe repXML = new Repositorio.XMLNFSe(unidadeDeTrabalho);                

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpj);

                if (empresa == null)
                    return new Retorno<List<RetornoNFSe>>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<List<RetornoNFSe>>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (protocolos == null || protocolos.Count <= 0 || protocolos.Count > 100)
                    return new Retorno<List<RetornoNFSe>>() { Mensagem = "Total de protocolos inválido (mínimo 1, máximo 100).", Status = false };

                List<Dominio.Entidades.NFSe> listaNFSe = repNFSe.BuscarPorCodigosNFSe(protocolos);
                
                List<RetornoNFSe> retorno = (from obj in listaNFSe
                                             select new RetornoNFSe()
                                             {
                                                 CNPJEmitente = obj.Empresa.CNPJ,
                                                 CodigoNFSe = obj.Codigo,
                                                 DataEmissao = obj.DataEmissao.ToString("dd/MM/yyyy HH:mm:ss"),
                                                 CodigoVerificacao = obj.CodigoVerificacao,
                                                 JustificativaCancelamento = obj.Status == Dominio.Enumeradores.StatusNFSe.Cancelado ? obj.JustificativaCancelamento : string.Empty,
                                                 MensagemRetorno = obj.RPS.MensagemRetorno,
                                                 NumeroNFSe = obj.Numero,
                                                 NumeroProtocolo = obj.RPS.Protocolo,
                                                 SerieNFSe = obj.Serie.Numero,
                                                 NumeroRPS = obj.RPS.Numero,
                                                 SerieRPS = obj.RPS.Serie,
                                                 StatusNFSe = obj.Status,
                                                 OutrasInformacoes = obj.OutrasInformacoes,
                                                 Tomador = new Dominio.ObjetosDeValor.NFSe.Tomador()
                                                 {
                                                     CNPJ = obj.Tomador.CPF_CNPJ,
                                                     Razao = obj.Tomador.Nome,
                                                     IBGE = obj.Tomador.Localidade.CodigoIBGE.ToString(),
                                                     Cidade = obj.Tomador.Localidade.Descricao,
                                                     Estado = obj.Tomador.Localidade.Estado.Sigla
                                                 },
                                                 ValoresNFSe = new Dominio.ObjetosDeValor.NFSe.ValoresNFSe()
                                                 {
                                                     BaseCalculoISS = Math.Round(obj.BaseCalculoISS, 2, MidpointRounding.ToEven),
                                                     AliquotaISS = obj.AliquotaISS,
                                                     ValorISS = Math.Round(obj.ValorISS, 2, MidpointRounding.ToEven),
                                                     ValorNFSe = Math.Round(obj.ValorServicos, 2, MidpointRounding.ToEven),
                                                     ISSRetido = obj.ISSRetido ? "Sim" : "Não"
                                                 },
                                                 XML = repXML.BuscarPorNFSe(obj.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao) != null ? repXML.BuscarPorNFSe(obj.Codigo, Dominio.Enumeradores.TipoXMLNFSe.Autorizacao).XML : string.Empty
                                                
                                             }).ToList();

                return new Retorno<List<RetornoNFSe>>() { Status = true, Mensagem = "XML obtido com sucesso.", Objeto = retorno };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ObterXML");                

                return new Retorno<List<RetornoNFSe>>() { Mensagem = "Ocorreu uma falha ao obter o XML dos CT-es.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
