using System;

namespace EmissaoCTe.Integracao.Transportador
{
    public class OcorrenciaCTe : IOcorrenciaCTe
    {
        #region Metodos Publicos
        public Retorno<RetornoOcorrenciaCTe> ConsultarOcorrencia(string cnpjTransportador, string cnpjEmbarcador, string token, string chaveNFe, int numeroCTe)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);

                if (empresa == null)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(chaveNFe) && numeroCTe <= 0)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Informe a chave da NF-e ou o número do CT-e a ser consultado.", Status = false };

                Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrencia.Buscar(empresa.Codigo, numeroCTe, chaveNFe, Utilidades.String.OnlyNumbers(cnpjEmbarcador));

                if (ocorrencia == null)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Nenhuma ocorrência encontrada.", Status = true };

                return new Retorno<RetornoOcorrenciaCTe>()
                {
                    Mensagem = "Retorno realizado com sucesso.",
                    Objeto = new RetornoOcorrenciaCTe()
                    {
                        NumeroCTe = ocorrencia.CTe.Numero,
                        SerieCTe = ocorrencia.CTe.Serie.Numero,
                        ChaveCTe = ocorrencia.CTe.Chave,
                        CodigoOcorrencia = ocorrencia.Ocorrencia.CodigoProceda,
                        DescricaoOcorrencia = ocorrencia.Ocorrencia.Descricao,
                        ObservacaoOcorrencia = ocorrencia.Observacao
                    },
                    Status = true
                };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Ocorreu uma falha ao consultar a ocorrência.", Status = false };
            }
        }

        public Retorno<RetornoOcorrenciaCTe> ConsultarOcorrenciaChaveCTe(string chaveCTe, string cnpjTransportador, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);

                if (empresa == null)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(chaveCTe))
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Informe a chave do CT-e a ser consultado.", Status = false };

                if (!Utilidades.Validate.ValidarChave(Utilidades.String.OnlyNumbers(chaveCTe)))
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Informe a chave do CT-e (" + chaveCTe + ") está invalida.", Status = false };

                Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);

                Dominio.Entidades.OcorrenciaDeCTe ocorrencia = repOcorrencia.BuscarPorChaveCTe(Utilidades.String.OnlyNumbers(chaveCTe));

                if (ocorrencia == null)
                    return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Nenhuma ocorrência encontrada.", Status = true };

                return new Retorno<RetornoOcorrenciaCTe>()
                {
                    Mensagem = "Retorno realizado com sucesso.",
                    Objeto = new RetornoOcorrenciaCTe()
                    {
                        NumeroCTe = ocorrencia.CTe.Numero,
                        SerieCTe = ocorrencia.CTe.Serie.Numero,
                        ChaveCTe = ocorrencia.CTe.Chave,
                        CodigoOcorrencia = ocorrencia.Ocorrencia.CodigoProceda,
                        DescricaoOcorrencia = ocorrencia.Ocorrencia.Descricao,
                        ObservacaoOcorrencia = ocorrencia.Observacao
                    },
                    Status = true
                };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<RetornoOcorrenciaCTe>() { Mensagem = "Ocorreu uma falha ao consultar a ocorrência.", Status = false };
            }
        }

        public Retorno<int> IntegrarOcorrenciaCTe(string cnpjTransportador, string chaveCTe, string codigoOcorrencia, string dataOcorrencia, string observacao, string token)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
                Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);
                Repositorio.TipoDeOcorrenciaDeCTe repOcorrencia = new Repositorio.TipoDeOcorrenciaDeCTe(unidadeDeTrabalho);

                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(cnpjTransportador);
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null;
                Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrencia = null;

                if (empresa == null)
                    return new Retorno<int>() { Mensagem = "Transportador não encontrado.", Status = false };

                if (empresa.Configuracao == null || string.IsNullOrWhiteSpace(empresa.Configuracao.TokenIntegracaoCTe) || empresa.Configuracao.TokenIntegracaoCTe != token)
                    return new Retorno<int>() { Mensagem = "Token de acesso inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(chaveCTe))
                    return new Retorno<int>() { Mensagem = "Informe a chave do CT-e a ser consultado.", Status = false };

                if (!Utilidades.Validate.ValidarChave(Utilidades.String.OnlyNumbers(chaveCTe)))
                    return new Retorno<int>() { Mensagem = "Informe a chave do CT-e (" + chaveCTe + ") está invalida.", Status = false };

                cte = repCTe.BuscarPorChave(Utilidades.String.OnlyNumbers(chaveCTe));
                if (cte == null)
                    return new Retorno<int>() { Mensagem = "CT-e não localizado pela chave " + chaveCTe, Status = false };

                if (string.IsNullOrWhiteSpace(codigoOcorrencia))
                    return new Retorno<int>() { Mensagem = "Informe o código da ocorrência conforme manual.", Status = false };

                ocorrencia = repOcorrencia.BuscarPorCodigoProceda(codigoOcorrencia);

                if (ocorrencia == null && codigoOcorrencia.Length == 2 && codigoOcorrencia.Substring(0, 1) == "0")
                    ocorrencia = repOcorrencia.BuscarPorCodigoProceda(codigoOcorrencia.Substring(1, 1));

                if (ocorrencia == null)
                    return new Retorno<int>() { Mensagem = "Ocorrência não localizada, informe o código da ocorrência conforme manual.", Status = false };

                return this.SalvarOcorrenciaCTe(cnpjTransportador, cte, ocorrencia, dataOcorrencia, observacao, unidadeDeTrabalho);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha ao integrar ocorrência do CT-e.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }
        #endregion

        #region Metodos Privados

        private Retorno<int> SalvarOcorrenciaCTe(string cnpjTransportador, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.TipoDeOcorrenciaDeCTe ocorrencia, string dataOcorrencia, string observacao, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Repositorio.OcorrenciaDeCTe repOcorrenciaCTe = new Repositorio.OcorrenciaDeCTe(unidadeDeTrabalho);
            Dominio.Entidades.OcorrenciaDeCTe ocorrenciaCTe = new Dominio.Entidades.OcorrenciaDeCTe();

            DateTime data = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(dataOcorrencia))
                DateTime.TryParseExact(dataOcorrencia, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out data);

            ocorrenciaCTe.CTe = cte;
            ocorrenciaCTe.DataDaOcorrencia = data;
            ocorrenciaCTe.DataDeCadastro = DateTime.Today;
            ocorrenciaCTe.Observacao = observacao;
            ocorrenciaCTe.Ocorrencia = ocorrencia;

            repOcorrenciaCTe.Inserir(ocorrenciaCTe);

            return new Retorno<int>() { Mensagem = "Integração da ocorrência realizada com sucesso.", Status = true, Objeto = ocorrenciaCTe.Codigo };
        }

        #endregion
    }
}
