using System;
using System.Collections.Generic;
using Servicos.Extensions;

namespace Servicos.Embarcador.Integracao.Nstech.IntegracaoEmissorDocumento
{
    public partial class IntegracaoNSTech
    {
        #region Métodos Globais

        public bool EmitirCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, int codigoEmpresa, Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            bool sucesso = false;
            string id = string.Empty;
            string mensagemErro = string.Empty;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeDeTrabalho);
            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(unidadeDeTrabalho);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unidadeDeTrabalho);

            try
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento envioWS = this.obterCteCCe(cce, empresa, unidadeDeTrabalho);

                //Transmite
                var retornoWS = this.TransmitirEmissor(Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumTipoWS.POST, envioWS, "cte-v4/events", _configuracaoIntegracaoEmissorDocumento.NSTechUrlAPICte);

                if (retornoWS.erro)
                {
                    mensagemErro = string.Concat("Emissor NSTech: Ocorreu uma falha ao consumir o webservice: ", retornoWS.mensagem);
                    sucesso = false;
                }
                else
                {
                    dynamic retorno = null;

                    try
                    {
                        retorno = retornoWS.jsonRetorno.FromJson<dynamic>();
                    }
                    catch (Exception ex)
                    {
                        Servicos.Log.TratarErro($"[Arquitetura-CatchNoAction] Erro ao desserializar resposta de carta de correção CTe Nstech: {ex.ToString()}", "CatchNoAction");
                    }

                    if (retorno == null)
                    {
                        mensagemErro = string.Format("Emissor NSTech: Ocorreu uma falha ao solicitar o cancelamento do cte; RetornoWS {0}.", retornoWS.jsonRetorno);
                        sucesso = false;
                    }
                    else
                    {
                        id = (string)retorno.id;
                        sucesso = true;
                    }
                }

                if (sucesso)
                {
                    cce.CodigoIntegrador = 0;
                    cce.MensagemRetornoSefaz = "CT-e em processamento.";
                    cce.SistemaEmissor = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissorDocumento.NSTech;
                    cce.Status = Dominio.Enumeradores.StatusCCe.Enviado;
                    repCCe.Atualizar(cce);

                    return true;
                }
                else
                {
                    cce.MensagemRetornoSefaz = Utilidades.String.ReplaceInvalidCharacters(mensagemErro);
                    cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                    repCCe.Atualizar(cce);

                    Servicos.Log.TratarErro(mensagemErro);
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                cce.Status = Dominio.Enumeradores.StatusCCe.Rejeicao;
                cce.MensagemRetornoSefaz = string.Concat("ERRO: Sefaz indisponível no momento. Tente novamente.");
                repCCe.Atualizar(cce);

                sucesso = false;
            }

            return sucesso;
        }

        #endregion Métodos Globais

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento obterCteCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce, Dominio.Entidades.Empresa empresa, Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEvento();

            TimeZoneInfo fusoHorarioEmpresa = TimeZoneInfo.FindSystemTimeZoneById(empresa.FusoHorario);
            bool horarioVerao = fusoHorarioEmpresa.IsDaylightSavingTime(cce.DataEmissao.Value);
            string fusoHorario = horarioVerao ? AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours + 1, fusoHorarioEmpresa.BaseUtcOffset.Minutes) : AjustarFuso(fusoHorarioEmpresa.BaseUtcOffset.Hours, fusoHorarioEmpresa.BaseUtcOffset.Minutes);

            retorno.data = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoData();
            retorno.data.externalId = cce.Codigo.ToString();
            retorno.data.eventDate = cce.DataEmissao.HasValue ? cce.DataEmissao.Value.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "T") + fusoHorario : null;
            retorno.data.cteKey = cce.CTe.Chave;
            retorno.data.eventSequence = cce.NumeroSequencialEvento;
            retorno.data.issueType = Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssueType.default_;

            retorno.data.issuer = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuer();
            retorno.data.issuer.type = empresa.Tipo == "F" ? Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.individual : Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.enumIssuerType.legal;
            retorno.data.issuer.document = Utilidades.String.OnlyNumbers(empresa.CNPJ);
            retorno.data.issuer.state = empresa.Localidade.Estado.Sigla;

            Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCorrectionLetter eventoCartaCorrecao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCorrectionLetter();

            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(unitOfWork);
            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cce.Codigo);

            foreach (Dominio.Entidades.ItemCCe item in itensCCe)
            {
                if (eventoCartaCorrecao.fields == null)
                    eventoCartaCorrecao.fields = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCorrectionLetterFields>();

                Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCorrectionLetterFields correcao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteEventoIssuerCorrectionLetterFields();
                correcao.group = item.CampoAlterado.GrupoCampo;
                correcao.field = item.CampoAlterado.NomeCampo;
                correcao.value = item.ValorAlterado;

                if (item.NumeroItemAlterado != 0)
                    correcao.itemIndex = item.NumeroItemAlterado;

                eventoCartaCorrecao.fields.Add(correcao);
            }

            retorno.data.evento = eventoCartaCorrecao;

            retorno.options = new Dominio.ObjetosDeValor.Embarcador.Integracao.Nstech.EmissorDocumento.cteOptions();
            retorno.options.removeSpecialsChars = true;

            return retorno;
        }

        #endregion Métodos Privados
    }
}
