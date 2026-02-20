using System;
using System.Collections.Generic;
using System.Linq;

namespace EmissaoCTe.Integracao
{
    public class ImpressaoCTe : IImpressaoCTe
    {
        public Retorno<int> SolicitarImpressao(int[] codigosCTes, string cnpjEmpresaPai)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.IntegracaoCTe repIntegracaoCTe = new Repositorio.IntegracaoCTe(unidadeDeTrabalho);
                Repositorio.ImpressaoIntegracaoCTe repImpressaoCTe = new Repositorio.ImpressaoIntegracaoCTe(unidadeDeTrabalho);

                foreach (int codigoCTe in codigosCTes)
                {
                    List<Dominio.Entidades.IntegracaoCTe> integracoes = repIntegracaoCTe.BuscarPorCTeETipo(codigoCTe, Dominio.Enumeradores.TipoIntegracao.Emissao, Utilidades.String.OnlyNumbers(cnpjEmpresaPai), "A");

                    if (integracoes.Count() > 0)
                    {
                        Dominio.Entidades.ImpressaoIntegracaoCTe impressao = new Dominio.Entidades.ImpressaoIntegracaoCTe();

                        impressao.CTe = integracoes[0].CTe;
                        impressao.DataSolicitacao = DateTime.Now;
                        impressao.Status = Dominio.Enumeradores.StatusImpressaoCTe.Pendente;

                        repImpressaoCTe.Inserir(impressao);
                    }
                }

                return new Retorno<int>() { Mensagem = "Impressões solicitadas com sucesso.", Status = true };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao solicitar a impressão.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
             
        public Retorno<int> Alterar(int codigoImpressao, Dominio.Enumeradores.StatusImpressaoCTe status)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (codigoImpressao <= 0)
                    return new Retorno<int>() { Mensagem = "O protocolo de impressão informado é inválido.", Status = false };

                Repositorio.ImpressaoIntegracaoCTe repImpressao = new Repositorio.ImpressaoIntegracaoCTe(unidadeDeTrabalho);
                Dominio.Entidades.ImpressaoIntegracaoCTe impressao = repImpressao.Buscar(codigoImpressao);

                if (impressao == null)
                    return new Retorno<int>() { Mensagem = "Solicitação de impressão não encontrada.", Status = false };

                impressao.Status = Dominio.Enumeradores.StatusImpressaoCTe.Impresso;

                repImpressao.Atualizar(impressao);

                return new Retorno<int>() { Mensagem = "Alteração realizada com sucesso!", Status = true, Objeto = impressao.Codigo };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<int>() { Mensagem = "Ocorreu uma falha genérica ao tentar alterar a impressão.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<List<RetornoImpressao>> ObterImpressoesPendentes(int codigoEmpresaPai, int numeroUnidade)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                if (codigoEmpresaPai <= 0)
                    return new Retorno<List<RetornoImpressao>>() { Mensagem = "Código da empresa pai inválido.", Status = false };

                if (numeroUnidade <= 0)
                    return new Retorno<List<RetornoImpressao>>() { Mensagem = "Número da unidade informada inválido.", Status = false };
                
                Repositorio.ImpressaoIntegracaoCTe repImpressao = new Repositorio.ImpressaoIntegracaoCTe(unidadeDeTrabalho);

                List<Dominio.Entidades.ImpressaoIntegracaoCTe> impressoes = repImpressao.Buscar(codigoEmpresaPai, numeroUnidade, Dominio.Enumeradores.StatusImpressaoCTe.Pendente);

                Retorno<List<RetornoImpressao>> retorno = new Retorno<List<RetornoImpressao>>() { Objeto = new List<RetornoImpressao>(), Mensagem = "Impressões geradas com sucesso.", Status = true };

                Servicos.CTe servicoCTe = new Servicos.CTe(unidadeDeTrabalho);

                foreach (Dominio.Entidades.ImpressaoIntegracaoCTe impressao in impressoes)
                {
                    retorno.Objeto.Add(new RetornoImpressao()
                    {
                        CodigoImpressao = impressao.Codigo,
                        NumeroCTe = impressao.CTe.Numero,
                        SerieCTe = impressao.CTe.Serie.Numero,
                        PDF = servicoCTe.ObterDACTE(impressao.CTe.Codigo, impressao.CTe.Empresa.Codigo, unidadeDeTrabalho)
                    });
                }

                return retorno;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new Retorno<List<RetornoImpressao>>() { Mensagem = "Ocorreu uma falha ao obter as impressões pendentes.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
    }
}
