using System;

namespace EmissaoCTe.Integracao
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Clientes" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Clientes.svc or Clientes.svc.cs at the Solution Explorer and start debugging.
    public class Clientes : IClientes
    {
        #region Metodos Publicos

        public Retorno<int> EnviarCNPJConsulta(string cnpj, string estado)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ConsultaCNPJ repConsultaCNPJ = new Repositorio.ConsultaCNPJ(unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(cnpj))
                    return new Retorno<int>() { Mensagem = "CNPJ inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(estado))
                    return new Retorno<int>() { Mensagem = "Sigla estado inválido.", Status = false };

                Dominio.Entidades.ConsultaCNPJ consultaCNPJ = repConsultaCNPJ.Consultar(cnpj, estado, Dominio.Enumeradores.StatusConsultaCNPJ.Pendente);
                if (consultaCNPJ == null)
                {
                    consultaCNPJ = repConsultaCNPJ.Consultar(cnpj, estado, Dominio.Enumeradores.StatusConsultaCNPJ.EmProcessamento);
                    if (consultaCNPJ == null)
                    {
                        consultaCNPJ = new Dominio.Entidades.ConsultaCNPJ();
                        consultaCNPJ.CNPJ = cnpj;
                        consultaCNPJ.Estado = estado;
                        consultaCNPJ.Status = Dominio.Enumeradores.StatusConsultaCNPJ.Pendente;

                        repConsultaCNPJ.Inserir(consultaCNPJ);
                    }
                }

                return new Retorno<int>() { Status = true, Mensagem = "Consulta inserida com sucesso.", Objeto = consultaCNPJ.Codigo };
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ConsultaCNPJ");

                return new Retorno<int>() { Mensagem = "Ocorreu uma falha inserir consulta de CNPJ.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        public Retorno<RetornoConsultaCNPJ> ConsultarCNPJConsulta(string cnpj, string estado)
        {
            Repositorio.UnitOfWork unidadeDeTrabalho = new Repositorio.UnitOfWork(Conexao.StringConexao);

            try
            {
                Repositorio.ConsultaCNPJ repConsultaCNPJ = new Repositorio.ConsultaCNPJ(unidadeDeTrabalho);

                if (string.IsNullOrWhiteSpace(cnpj))
                    return new Retorno<RetornoConsultaCNPJ>() { Mensagem = "CNPJ inválido.", Status = false };

                if (string.IsNullOrWhiteSpace(estado))
                    return new Retorno<RetornoConsultaCNPJ>() { Mensagem = "Sigla estado inválido.", Status = false };

                Dominio.Entidades.ConsultaCNPJ consultaCNPJ = repConsultaCNPJ.Consultar(cnpj, estado, null);

                if (consultaCNPJ != null)
                {
                    RetornoConsultaCNPJ retornoConsultaCNPJ = new RetornoConsultaCNPJ();
                    retornoConsultaCNPJ.CNPJ = consultaCNPJ.CNPJ;
                    retornoConsultaCNPJ.Estado = consultaCNPJ.Estado;
                    retornoConsultaCNPJ.IE = consultaCNPJ.InscricaoEstadual;
                    retornoConsultaCNPJ.StatusIE = consultaCNPJ.StatusIE;
                    retornoConsultaCNPJ.Regime = consultaCNPJ.RegimeTributario;
                    if (consultaCNPJ.Status == Dominio.Enumeradores.StatusConsultaCNPJ.Sucesso && (string.IsNullOrWhiteSpace(consultaCNPJ.RegimeTributario) || string.IsNullOrWhiteSpace(consultaCNPJ.InscricaoEstadual)))
                        retornoConsultaCNPJ.StatusConsulta = Dominio.Enumeradores.StatusConsultaCNPJ.EmProcessamento;
                    else
                        retornoConsultaCNPJ.StatusConsulta = consultaCNPJ.Status;
                    retornoConsultaCNPJ.ErroConsulta = consultaCNPJ.ErroConsulta;

                    return new Retorno<RetornoConsultaCNPJ>() { Status = true, Mensagem = "Consulta retornada com sucesso.", Objeto = retornoConsultaCNPJ };
                }

                return new Retorno<RetornoConsultaCNPJ>() { Mensagem = "Nenhuma consulta localizada para este CNPJ.", Status = false };

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ConsultaCNPJ");

                return new Retorno<RetornoConsultaCNPJ>() { Mensagem = "Ocorreu uma falha obter retorno consulta CNPJ.", Status = false };
            }
            finally
            {
                unidadeDeTrabalho.Dispose();
            }
        }

        #endregion

        #region Metodos Privados


        #endregion
    }
}
