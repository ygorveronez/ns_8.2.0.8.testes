using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using CoreWCF;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class Filial(IServiceProvider serviceProvider) : WebServiceBase(serviceProvider), IFilial
    {
        #region Métodos Globais

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliais(int? inicio, int? limite)
        {
            ValidarToken();

            inicio ??= 0;
            limite ??= 0;

            Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> retorno = new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            try
            {
                if (limite <= 100)
                {
                    retorno.Objeto = new Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>();
                    Servicos.WebService.Filial.Filial serWSFilial = new Servicos.WebService.Filial.Filial(unitOfWork);
                    Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
                    var filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Filial.FiltroPesquisaFilial()
                    {
                        Ativo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo
                    };
                    List<Dominio.Entidades.Embarcador.Filiais.Filial> filiais = repFilial.Consultar(filtrosPesquisa, "Codigo", "desc", (int)inicio, (int)limite);
                    retorno.Objeto.NumeroTotalDeRegistro = repFilial.ContarConsulta(filtrosPesquisa);
                    retorno.Objeto.Itens = serWSFilial.RetornarFiliais(filiais);
                    retorno.Status = true;

                    Servicos.Auditoria.Auditoria.AuditarConsulta(Auditado, "Consultou filiais", unitOfWork);
                }
                else
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "O limite não pode ser maior que 100";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar as empresas filiais";
            }
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }

        public Retorno<bool> SalvarFilial(Dominio.ObjetosDeValor.Embarcador.Filial.Filial filialIntegracao)
        {
            Dominio.Entidades.WebService.Integradora integradora = ValidarToken();

            Retorno<bool> retorno = new Retorno<bool>();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            retorno.Mensagem = "";

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);

            try
            {
                Servicos.Log.TratarErro("SalvarFilial: " + Newtonsoft.Json.JsonConvert.SerializeObject(filialIntegracao));

                string mensagem = "";

                if (filialIntegracao == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Dados da filial não enviados;";
                    return retorno;
                }

                if (string.IsNullOrWhiteSpace(filialIntegracao.CodigoIntegracao))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Código Integração obrigatório;";
                    return retorno;
                }

                if (string.IsNullOrWhiteSpace(filialIntegracao.Descricao))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Descrição obrigatório;";
                    return retorno;
                }

                if (filialIntegracao.CodigoAtividade <= 0 || filialIntegracao.CodigoAtividade > 7)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Código Atividade obrigatório;";
                    return retorno;
                }

                if (string.IsNullOrWhiteSpace(filialIntegracao.CNPJ))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "CNPJ Filial obrigatório;";
                    return retorno;
                }
                else if (!Utilidades.Validate.ValidarCNPJ(Utilidades.String.OnlyNumbers(filialIntegracao.CNPJ)))
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "CNPJ Filial inválido;";
                    return retorno;
                }

                if (filialIntegracao.Endereco == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Endereço é obrigatório;";
                    return retorno;
                }
                else if (filialIntegracao.Endereco.Cidade == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Cidade é obrigatório;";
                    return retorno;
                }
                else if (filialIntegracao.Endereco.Cidade.IBGE <= 0)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "IBGE da cidade é obrigatório;";
                    return retorno;
                }

                bool inserir = false;
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.buscarPorCodigoEmbarcador(filialIntegracao.CodigoIntegracao);
                if (filial == null)
                {
                    inserir = true;
                    filial = new Dominio.Entidades.Embarcador.Filiais.Filial();
                }

                unitOfWork.Start();
                filial.Descricao = filialIntegracao.Descricao;
                filial.CodigoFilialEmbarcador = filialIntegracao.CodigoIntegracao;
                filial.CNPJ = Utilidades.String.OnlyNumbers(filialIntegracao.CNPJ);
                filial.Atividade = repAtividade.BuscarPorCodigo(filialIntegracao.CodigoAtividade);
                filial.Localidade = repLocalidade.BuscarPorCodigoIBGE(filialIntegracao.Endereco.Cidade.IBGE);
                filial.Ativo = filialIntegracao.Ativo;

                if (filial.Atividade == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Atividade com código " + filialIntegracao.CodigoAtividade.ToString() + " não localizada;";
                    return retorno;
                }

                if (filial.Localidade == null)
                {
                    retorno.Status = false;
                    retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.DadosInvalidos;
                    retorno.Mensagem = "Localidade com IBGE "+ filialIntegracao.Endereco.Cidade.IBGE.ToString()+ " não localizada;";
                    return retorno;
                }

                if (inserir)
                    repFilial.Inserir(filial);
                else
                    repFilial.Atualizar(filial);

                Servicos.Auditoria.Auditoria.Auditar(Auditado, filial, "Salvou filial por integração", unitOfWork);

                unitOfWork.CommitChanges();

                retorno.Objeto = true;
                retorno.Status = true;
                return retorno;

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                Servicos.Log.TratarErro("SalvarCliente: " + ex);
                return new Retorno<bool>() { Mensagem = "Ocorreu uma falha genérica ao realizar a integração.", Status = false };
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
     
        public Retorno<bool> ConfirmarIntegracaoFilial(List<int> protocolos)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).ConfirmarIntegracaoFilial(protocolos));
            });
        }

        public Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> BuscarFiliaisPendentesIntegracao(int? quantidade)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>> retorno = Retorno<Dominio.ObjetosDeValor.WebService.Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>>.CreateFrom(new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).BuscarFiliaisPendentesIntegracao(quantidade ?? 0));

                return new Retorno<Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>>()
                {
                    CodigoMensagem = retorno.CodigoMensagem,
                    DataRetorno = retorno.DataRetorno,
                    Mensagem = retorno.Mensagem,
                    Status = retorno.Status,
                    Objeto = Paginacao<Dominio.ObjetosDeValor.Embarcador.Filial.Filial>.CreateFrom(retorno.Objeto)
                };
            });
        }

        public Retorno<bool> InformarVolumesTanques(Dominio.ObjetosDeValor.WebService.Filial.FilialTanque filialTanque)
        {
            return ProcessarRequisicao((Dominio.Entidades.WebService.Integradora integradora, Repositorio.UnitOfWork unitOfWork) =>
            {
                return Retorno<bool>.CreateFrom(new Servicos.WebService.Filial.Filial(unitOfWork, TipoServicoMultisoftware, Cliente, Auditado, ClienteAcesso, Conexao.createInstance(_serviceProvider).AdminStringConexao).InformarVolumesTanques(filialTanque));
            });
        }
        
        #endregion

        #region Métodos Protegidos Sobrescritos

        protected override Dominio.ObjetosDeValor.Enumerador.OrigemAuditado ObterOrigemAuditado()
        {
            return Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.WebServiceFilial;
        }

        #endregion
    }
}
