using System;
using System.Collections.Generic;

namespace Servicos.Embarcador.Documentos
{
    public class ManifestacaoDestinatario
    {
        public static Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario EmitirManifestacaoDestinatario(int codigoEmpresa, string chaveNFe, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacao, string justificativa, Repositorio.UnitOfWork unidadeTrabalho)
        {
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unidadeTrabalho);
            Repositorio.Embarcador.Documentos.ManifestacaoDestinatario repManifestacaoDestinatario = new Repositorio.Embarcador.Documentos.ManifestacaoDestinatario(unidadeTrabalho);

            Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(codigoEmpresa);

            if (empresa == null || string.IsNullOrWhiteSpace(empresa.NomeCertificado) || !Utilidades.IO.FileStorageService.Storage.Exists(empresa.NomeCertificado) || string.IsNullOrWhiteSpace(empresa.SenhaCertificado))
                throw new Exception("Certificado da empresa é inválido ou inexistente.");

            MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento tipoEvento;
            MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb tipoAmbiente = empresa.TipoAmbiente == Dominio.Enumeradores.TipoAmbiente.Producao ? MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item1 : MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TAmb.Item2;

            switch (tipoManifestacao)
            {
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao:
                    tipoEvento = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento.Item210200;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao:
                    tipoEvento = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento.Item210210;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.Desconhecida:
                    tipoEvento = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento.Item210220;
                    break;
                case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.OperacaoNaoRealizada:
                    tipoEvento = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Envio.TEventoInfEventoTpEvento.Item210240;
                    break;
                default:
                    throw new Exception("Tipo de manifestação não implementada para a manifestação do destinatário.");
            }

            DateTime dataEmissao = DateTime.Now;

            MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Retorno.TRetEnvEvento retorno = MultiSoftware.NFe.Evento.ManifestacaoDestinatario.Servicos.ManifestacaoDestinatario.EnviarManifestacaoDestinatario4(empresa.CNPJ, tipoAmbiente, tipoEvento, chaveNFe, dataEmissao, repManifestacaoDestinatario.BuscarUltimoIdLote(empresa.Codigo) + 1, justificativa, empresa.NomeCertificado, empresa.SenhaCertificado);

            Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario manifestacaoDestinatario = new Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario();

            manifestacaoDestinatario.Empresa = empresa;
            manifestacaoDestinatario.Ambiente = empresa.TipoAmbiente;
            manifestacaoDestinatario.ChaveNFe = chaveNFe;
            manifestacaoDestinatario.CodigoStatusResposta = retorno.retEvento[0].infEvento.cStat;
            manifestacaoDestinatario.DataAutorizacao = DateTime.ParseExact(retorno.retEvento[0].infEvento.dhRegEvento, "yyyy-MM-ddTHH:mm:sszzz", null, System.Globalization.DateTimeStyles.None);
            manifestacaoDestinatario.DataEmissao = dataEmissao;
            manifestacaoDestinatario.DescricaoStatusResposta = retorno.retEvento[0].infEvento.xMotivo;
            manifestacaoDestinatario.IdLote = long.Parse(retorno.idLote);
            manifestacaoDestinatario.Justificativa = justificativa;
            manifestacaoDestinatario.NumeroSequencialEvento = int.Parse(retorno.retEvento[0].infEvento.nSeqEvento);
            manifestacaoDestinatario.Protocolo = retorno.retEvento[0].infEvento.nProt;
            manifestacaoDestinatario.Tipo = tipoManifestacao;
            manifestacaoDestinatario.VersaoAplicacao = retorno.retEvento[0].infEvento.verAplic;

            if (manifestacaoDestinatario.CodigoStatusResposta == "135" || manifestacaoDestinatario.CodigoStatusResposta == "136")
                manifestacaoDestinatario.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Autorizado;
            else
                manifestacaoDestinatario.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Rejeitado;

            repManifestacaoDestinatario.Inserir(manifestacaoDestinatario);

            return manifestacaoDestinatario;
        }

        public static bool EmitirManifestacaoDestinatario(out string erro, Dominio.Entidades.Embarcador.Documentos.DocumentoDestinadoEmpresa documentoDestinado, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario tipoManifestacaoDestinatario, string justificativa, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado)
        {
            if (documentoDestinado == null)
            {
                erro = "Documento não encontrado.";
                return false;
            }

            if (documentoDestinado.TipoDocumento != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoDestinadoEmpresa.NFeDestinada)
            {
                erro = "Somente é permitida a manifestação para o tipo de documento NF-e Destinada.";
                return false;
            }

            Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa repDocumentoDestinadoEmpresa = new Repositorio.Embarcador.Documentos.DocumentoDestinadoEmpresa(unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario manifestacao = Servicos.Embarcador.Documentos.ManifestacaoDestinatario.EmitirManifestacaoDestinatario(documentoDestinado.Empresa.Codigo, documentoDestinado.Chave, tipoManifestacaoDestinatario, justificativa, unitOfWork);

            if (documentoDestinado.Manifestacoes == null)
                documentoDestinado.Manifestacoes = new List<Dominio.Entidades.Embarcador.Documentos.ManifestacaoDestinatario>();

            documentoDestinado.Manifestacoes.Add(manifestacao);

            if (manifestacao.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusManifestacaoDestinatario.Autorizado)
            {
                switch (manifestacao.Tipo)
                {
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.CienciaOperacao:
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.CienciaOperacao;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.ConfirmadaOperacao:
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.ConfirmadaOperacao;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.Desconhecida:
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.Desconhecida;
                        break;
                    case Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoManifestacaoDestinatario.OperacaoNaoRealizada:
                        documentoDestinado.SituacaoManifestacaoDestinatario = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoManifestacaoDestinatario.OperacaoNaoRealizada;
                        break;
                }
            }

            repDocumentoDestinadoEmpresa.Atualizar(documentoDestinado);

            Servicos.Auditoria.Auditoria.Auditar(auditado, documentoDestinado, null, "Emitiu manisfestação do destinatário.", unitOfWork);

            erro = string.Empty;
            return true;
        }
    }
}
