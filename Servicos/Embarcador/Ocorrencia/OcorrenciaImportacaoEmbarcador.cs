using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Servicos.Embarcador.Ocorrencia
{
    public class OcorrenciaImportacaoEmbarcador
    {

        #region Métodos Globais

        //metodo chamado pela thred para buscar ocorrencias do Embarcador para o grupo de pessoas que esta habilitado a integrar
        public static bool ImportarOcorrenciasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            List<int> codigosGrupoPessoas = repGrupoPessoa.BuscarCodigosGrupoPessoaIntegrarEmbarcador();

            foreach (int codigoGrupoPessoa in codigosGrupoPessoas)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoa.BuscarPorCodigo(codigoGrupoPessoa);

                if (!ImportarOcorrenciasEmbarcador(out mensagemErro, grupoPessoa, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return false;
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        //metodo chamado pela thred para buscar os CTEs das ocorrencias do Embarcador que estao aguardando consulta ctes
        public static bool ImportarCTesOcorrenciasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repCargaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);

            List<long> codigosOcorrencia = repCargaImportacaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTes, 100);

            foreach (long codigoOcorrencia in codigosOcorrencia)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaIntegracao = repCargaImportacaoEmbarcador.BuscarPorCodigo(codigoOcorrencia, false);

                if (!ImportarCTesOcorrenciaEmbarcador(out mensagemErro, OcorrenciaIntegracao, tipoServicoMultisoftware, unitOfWork, configuracaoTMS))
                    Servicos.Log.TratarErro($"Falha ao importar CT-e da ocorrência (código {OcorrenciaIntegracao.Codigo} - número {OcorrenciaIntegracao.NumeroOcorrenciaEmbarcador}): {mensagemErro}");

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        //metodo chamado pela thred para gerar as ocorrencias do Embarcador que estao aguardando geracao
        public static bool GerarOcorrenciasImportadasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe repCargaPedidoDocumentoCTe = new Repositorio.Embarcador.Cargas.CargaPedidoDocumentoCTe(unitOfWork);

            List<long> codigosOcorrencias = repOcorrenciaIntegracaoEmbarcador.BuscarCodigosPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoOcorrencia, 100);

            foreach (long codigoOcorrencia in codigosOcorrencias)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaIntegracao = repOcorrenciaIntegracaoEmbarcador.BuscarPorCodigo(codigoOcorrencia, false);
                IEnumerable<int> codigosCTes = ocorrenciaIntegracao.CTes.Select(o => o.Codigo);

                if (repCargaCTe.ExisteAutorizadoPorCTe(codigosCTes) ||
                    repCargaPedidoDocumentoCTe.ExistePorCTe(codigosCTes) || repCargaOcorrenciaDocumento.ExisteAtivoPorListaCTe(codigosCTes))
                {
                    ocorrenciaIntegracao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Problemas;
                    ocorrenciaIntegracao.Mensagem = "Não é possível gerar a ocorrência pois os documentos já estão vinculados em outra ocorrência.";

                    repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaIntegracao);

                    mensagemErro = ocorrenciaIntegracao.Mensagem;
                    return false;
                }

                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = ocorrenciaIntegracao.CTes.FirstOrDefault();

                if (!GerarOcorrenciaImportadasEmbarcador(out mensagemErro, cte, ocorrenciaIntegracao, tipoServicoMultisoftware, unitOfWork))
                    Servicos.Log.TratarErro($"Falha ao gerar a importacao da ocorrencia (código {ocorrenciaIntegracao.Codigo} - número ocorrencia embarcador {ocorrenciaIntegracao.NumeroOcorrenciaEmbarcador}): {mensagemErro}");

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        //metodo chamado pela thred para buscar ocorrencias canceladas do Embarcador para o grupo de pessoas que esta habilitado a integrar
        public static bool ImportarOcorrenciasCanceladasEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);

            List<int> codigosGrupoPessoas = repGrupoPessoa.BuscarCodigosGrupoPessoaIntegrarEmbarcador();

            foreach (int codigoGrupoPessoa in codigosGrupoPessoas)
            {
                Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupoPessoa = repGrupoPessoa.BuscarPorCodigo(codigoGrupoPessoa);

                if (!ImportarOcorrenciasCanceladasEmbarcador(out mensagemErro, grupoPessoa, unitOfWork))
                {
                    unitOfWork.Rollback();
                    return false;
                }

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        //metodo chamado pela thred para buscar ctes de cancelamento do embarcador q estao aguardando consulta ctes cancelados
        public static bool ImportarCTesCanceladosEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);

            List<long> codigosOcorrencias = repOcorrenciaIntegracaoEmbarcador.BuscarCodigosPorSituacao(SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTesCancelados, 100);

            foreach (long codigoOcorrenciaIntegracao in codigosOcorrencias)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaIntegracao = repOcorrenciaIntegracaoEmbarcador.BuscarPorCodigo(codigoOcorrenciaIntegracao, false);

                if (ImportarCTesCanceladosOcorrenciaEmbarcador(out mensagemErro, OcorrenciaIntegracao, tipoServicoMultisoftware, unitOfWork) &&
                    ImportarNFSesCanceladosOcorrenciaEmbarcador(out mensagemErro, OcorrenciaIntegracao, tipoServicoMultisoftware, unitOfWork))
                {
                    OcorrenciaIntegracao.Situacao = SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoCancelamento;
                    repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);
                }
                else
                    Servicos.Log.TratarErro($"Falha ao importar documento cancelado da Ocorrência (código {OcorrenciaIntegracao.Codigo} - número {OcorrenciaIntegracao.NumeroOcorrenciaEmbarcador}): {mensagemErro}");

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        //metodo chamado pela thred para gerar cancelamentos de ocorrencias que estao como aguardando geracao cancelamento
        public static bool GerarCancelamentosOcorrenciaEmbarcador(out string mensagemErro, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            if (tipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                return true;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);

            List<long> codigosOcorrenciaIntegracao = repOcorrenciaIntegracaoEmbarcador.BuscarCodigosPorSituacao(SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoCancelamento, 100);

            foreach (long codigoOcorrenciaIntegracao in codigosOcorrenciaIntegracao)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaIntegracao = repOcorrenciaIntegracaoEmbarcador.BuscarPorCodigo(codigoOcorrenciaIntegracao, false);

                if (!GerarCancelamentoOcorrenciaEmbarcador(out mensagemErro, ocorrenciaIntegracao, tipoServicoMultisoftware, unitOfWork))
                    Servicos.Log.TratarErro($"Falha ao gerar o cancelamento da ocorrência (código {ocorrenciaIntegracao.Codigo} - número {ocorrenciaIntegracao.NumeroOcorrenciaEmbarcador}): {mensagemErro}");

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        #endregion


        #region Métodos Privados

        private static bool GerarCancelamentoOcorrenciaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento repOcorrenciaCancelamento = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCancelamento(unitOfWork);

            if (ocorrenciaIntegracao.CargaOcorrencia != null)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento OcorrenciaCancelamento = repOcorrenciaCancelamento.BuscarPorOcorrencia(ocorrenciaIntegracao.CargaOcorrencia.Codigo);

                if (OcorrenciaCancelamento == null)
                {
                    OcorrenciaCancelamento = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCancelamento()
                    {
                        Ocorrencia = ocorrenciaIntegracao.CargaOcorrencia,
                        DataCancelamento = ocorrenciaIntegracao.DataCancelamento,
                        MotivoCancelamento = $"Cancelamento importado do MultiEmbarcador ({ocorrenciaIntegracao.MotivoCancelamento})",
                        Situacao = SituacaoCancelamentoOcorrencia.EmCancelamento,
                        Tipo = TipoCancelamentoOcorrencia.Cancelamento,
                        SituacaoOcorrenciaNoCancelamento = ocorrenciaIntegracao.CargaOcorrencia?.SituacaoOcorrenciaNoCancelamento ?? SituacaoOcorrencia.Finalizada
                    };

                    repOcorrenciaCancelamento.Inserir(OcorrenciaCancelamento);

                    ocorrenciaIntegracao.OcorrenciaCancelamento = OcorrenciaCancelamento;
                }
            }

            ocorrenciaIntegracao.Situacao = SituacaoOcorrenciaIntegracaoEmbarcador.Cancelado;
            ocorrenciaIntegracao.Mensagem = "Cancelamento gerado com sucesso.";

            repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaIntegracao);

            return true;
        }

        private static bool ImportarCTesCanceladosOcorrenciaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            mensagemErro = null;

            Servicos.ServicoSGT.CTe.CTeClient svcWSCTe = ObterClientCTes(OcorrenciaIntegracao.GrupoPessoas.URLIntegracaoMultiEmbarcador, OcorrenciaIntegracao.GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            if (OcorrenciaIntegracao.CargaOcorrencia == null)
                return true;

            do
            {
                ServicoSGT.CTe.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcWSCTe.BuscarCTesPorOcorrencia(OcorrenciaIntegracao.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, 0, 50);

                if (!retorno.Status)
                {
                    OcorrenciaIntegracao.Mensagem = retorno.Mensagem;
                    repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in retorno.Objeto.Itens)
                {
                    if (cte.Modelo != "57")
                        continue;

                    if (string.IsNullOrWhiteSpace(cte.XMLCancelamento))
                    {
                        mensagemErro = $"O CT-e {cte.Numero} não possui um XML de cancelamento/inutilização.";

                        OcorrenciaIntegracao.Mensagem = mensagemErro;
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);
                        return false;
                    }

                    System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.XMLCancelamento.Replace("ProcInutCTe", "procInutCTe"));

                    object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, OcorrenciaIntegracao.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false, tipoServicoMultisoftware, true, cte);

                    if (retornoInserir == null)
                    {
                        OcorrenciaIntegracao.Mensagem = $"Não foi possível ler o XML de cancelamento/inutilização do CT-e {cte.Numero}.";
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                    else if (retornoInserir.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteInserido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;

                        if (cteInserido.Status == "I" && !repCargaCTe.ExistePorCargaECTe(OcorrenciaIntegracao.CargaOcorrencia.Carga.Codigo, cteInserido.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                            {
                                Carga = OcorrenciaIntegracao.CargaOcorrencia.Carga,
                                CargaOrigem = OcorrenciaIntegracao.CargaOcorrencia.Carga,
                                CTe = cteInserido,
                                DataVinculoCarga = DateTime.Now,
                                SistemaEmissor = SistemaEmissor.OutrosEmissores
                            };

                            repCargaCTe.Inserir(cargaCTe);

                            if (cargaCTe.CTe != null)
                            {
                                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).GeracaoControleDocumento(cargaCTe.CTe);
                            }
                        }
                    }
                    else if (retornoInserir.GetType() == typeof(string))
                    {
                        OcorrenciaIntegracao.Mensagem = retorno.Mensagem;
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            return true;
        }

        private static bool ImportarNFSesCanceladosOcorrenciaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaIntegracao, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTe repCargaCTe = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);

            mensagemErro = null;

            Servicos.ServicoSGT.CTe.CTeClient svcWSCTe = ObterClientCTes(OcorrenciaIntegracao.GrupoPessoas.URLIntegracaoMultiEmbarcador, OcorrenciaIntegracao.GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            int limite = 50, inicio = 0;

            if (OcorrenciaIntegracao.CargaOcorrencia == null)
                return true;

            do
            {
                ServicoSGT.CTe.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcWSCTe.BuscarCTesPorOcorrencia(OcorrenciaIntegracao.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, 0, 50);

                if (!retorno.Status)
                {
                    OcorrenciaIntegracao.Mensagem = retorno.Mensagem;
                    repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in retorno.Objeto.Itens)
                {
                    if (cte.Modelo != "57")
                        continue;

                    if (string.IsNullOrWhiteSpace(cte.XMLCancelamento))
                    {
                        mensagemErro = $"O CT-e {cte.Numero} não possui um XML de cancelamento/inutilização.";

                        OcorrenciaIntegracao.Mensagem = mensagemErro;
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);
                        return false;
                    }

                    System.IO.MemoryStream memoryStream = Utilidades.String.ToStream(cte.XMLCancelamento.Replace("ProcInutCTe", "procInutCTe"));

                    object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, OcorrenciaIntegracao.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false, tipoServicoMultisoftware, true, cte);

                    if (retornoInserir == null)
                    {
                        OcorrenciaIntegracao.Mensagem = $"Não foi possível ler o XML de cancelamento/inutilização do CT-e {cte.Numero}.";
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                    else if (retornoInserir.GetType() == typeof(Dominio.Entidades.ConhecimentoDeTransporteEletronico))
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico cteInserido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;

                        if (cteInserido.Status == "I" && !repCargaCTe.ExistePorCargaECTe(OcorrenciaIntegracao.CargaOcorrencia.Carga.Codigo, cteInserido.Codigo))
                        {
                            Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = new Dominio.Entidades.Embarcador.Cargas.CargaCTe()
                            {
                                Carga = OcorrenciaIntegracao.CargaOcorrencia.Carga,
                                CargaOrigem = OcorrenciaIntegracao.CargaOcorrencia.Carga,
                                CTe = cteInserido,
                                DataVinculoCarga = DateTime.Now,
                                SistemaEmissor = SistemaEmissor.OutrosEmissores
                            };

                            repCargaCTe.Inserir(cargaCTe);

                            if (cargaCTe.CTe != null)
                            {
                                new Servicos.Embarcador.Documentos.ControleDocumento(unitOfWork).GeracaoControleDocumento(cargaCTe.CTe);
                            }
                        }
                    }
                    else if (retornoInserir.GetType() == typeof(string))
                    {
                        OcorrenciaIntegracao.Mensagem = retorno.Mensagem;
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(OcorrenciaIntegracao);

                        mensagemErro = (string)retornoInserir;
                        return false;
                    }
                }

                limite = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += 50;
            }
            while (inicio < limite);

            return true;
        }

        private static bool ImportarOcorrenciasCanceladasEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            ServicoSGT.Ocorrencia.OcorrenciasClient svcOcorrencia = ObterClientOcorrencia(GrupoPessoas.URLIntegracaoMultiEmbarcador, GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            ServicoSGT.Ocorrencia.RetornoOfArrayOfOcorrenciaCancelamento0halQhHF retorno;
            retorno = svcOcorrencia.BuscarOcorrenciasCanceladasPorTransportador();

            if (!retorno.Status)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }

            foreach (Dominio.ObjetosDeValor.WebService.OcorrenciaCancelamento.OcorrenciaCancelamento ocorrenciaCancelamentoIntegracaoMulti in retorno.Objeto)
            {
                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaIntegracaoEmbarcador = repOcorrenciaImportacaoEmbarcador.BuscarPorProtocolo(ocorrenciaCancelamentoIntegracaoMulti.ProtocoloOcorrecia);

                if (ocorrenciaIntegracaoEmbarcador != null)
                {
                    if (!ValidarCancelamentoOcorrencia(out mensagemErro, ocorrenciaIntegracaoEmbarcador, unitOfWork))
                    {
                        if (ocorrenciaIntegracaoEmbarcador.CargaOcorrencia != null && (ocorrenciaIntegracaoEmbarcador.CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada || ocorrenciaIntegracaoEmbarcador.CargaOcorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada))
                        {
                            ocorrenciaIntegracaoEmbarcador.Mensagem = "A ocorrência já está cancelada/anulada, não sendo possível importar o cancelamento do embarcador.";
                            ocorrenciaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Cancelado;

                            repOcorrenciaImportacaoEmbarcador.Atualizar(ocorrenciaIntegracaoEmbarcador);
                        }
                        else
                        {
                            ocorrenciaIntegracaoEmbarcador.Mensagem = mensagemErro;
                            ocorrenciaIntegracaoEmbarcador.ProtocoloCancelamento = ocorrenciaCancelamentoIntegracaoMulti.ProtocoloCancelamento;
                            ocorrenciaIntegracaoEmbarcador.DataCancelamento = ocorrenciaCancelamentoIntegracaoMulti.DataCancelamento;
                            ocorrenciaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(ocorrenciaCancelamentoIntegracaoMulti.MotivoCancelamento, 250);

                            repOcorrenciaImportacaoEmbarcador.Atualizar(ocorrenciaIntegracaoEmbarcador);

                            continue;
                        }
                    }
                    else
                    {
                        ocorrenciaIntegracaoEmbarcador.ProtocoloCancelamento = ocorrenciaCancelamentoIntegracaoMulti.ProtocoloCancelamento;
                        ocorrenciaIntegracaoEmbarcador.DataCancelamento = ocorrenciaCancelamentoIntegracaoMulti.DataCancelamento;
                        ocorrenciaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(ocorrenciaCancelamentoIntegracaoMulti.MotivoCancelamento, 250);
                        ocorrenciaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTesCancelados;

                        repOcorrenciaImportacaoEmbarcador.Atualizar(ocorrenciaIntegracaoEmbarcador);
                    }
                }
                else
                {
                    if (ocorrenciaCancelamentoIntegracaoMulti.PossuiDocumentoCancelado)
                    {
                        ServicoSGT.Ocorrencia.RetornoOfOcorrenciaIntegracaoMultivXP_PntxX retornoConsultaCarga = svcOcorrencia.BuscarOcorrenciaPorProtocoloETransportador(ocorrenciaCancelamentoIntegracaoMulti.ProtocoloOcorrecia);

                        if (!retornoConsultaCarga.Status)
                        {
                            mensagemErro = retornoConsultaCarga.Mensagem;
                            return false;
                        }

                        DateTime dataConsulta = retorno.DataRetorno.ToDateTime();

                        Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti OcorrenciaIntegracao = retornoConsultaCarga.Objeto;

                        unitOfWork.Start();

                        ocorrenciaIntegracaoEmbarcador = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador();

                        if (SalvarInformacoesOcorrenciaIntegracaoEmbarcador(out StringBuilder mensagem, true, ocorrenciaIntegracaoEmbarcador, OcorrenciaIntegracao, GrupoPessoas, dataConsulta, unitOfWork))
                        {
                            ocorrenciaIntegracaoEmbarcador.Mensagem = "Integrado com sucesso à partir do cancelamento.";
                            ocorrenciaIntegracaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTes;
                            ocorrenciaIntegracaoEmbarcador.ProtocoloCancelamento = ocorrenciaCancelamentoIntegracaoMulti.ProtocoloCancelamento;
                            ocorrenciaIntegracaoEmbarcador.DataCancelamento = ocorrenciaCancelamentoIntegracaoMulti.DataCancelamento;
                            ocorrenciaIntegracaoEmbarcador.MotivoCancelamento = Utilidades.String.Left(ocorrenciaCancelamentoIntegracaoMulti.MotivoCancelamento, 250);

                            repOcorrenciaImportacaoEmbarcador.Atualizar(ocorrenciaIntegracaoEmbarcador);
                        }
                        else
                        {
                            unitOfWork.Rollback();
                            mensagemErro = mensagem.ToString();
                            return false;
                        }

                        unitOfWork.CommitChanges();
                    }
                }

                if (ocorrenciaIntegracaoEmbarcador != null)
                    svcOcorrencia.ConfirmarIntegracaoOcorrenciaCanceladaTransportador((int)ocorrenciaIntegracaoEmbarcador.ProtocoloCancelamento);
            }

            return true;
        }

        private static bool ImportarOcorrenciasEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            ServicoSGT.Ocorrencia.OcorrenciasClient svcOcorrencia = ObterClientOcorrencia(GrupoPessoas.URLIntegracaoMultiEmbarcador, GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            ServicoSGT.Ocorrencia.RetornoOfArrayOfOcorrenciaIntegracaoMultivXP_PntxX retorno;
            retorno = svcOcorrencia.BuscarOcorrenciasPorTransportador();

            if (!retorno.Status)
            {
                mensagemErro = retorno.Mensagem;
                return false;
            }

            DateTime dataConsulta = retorno.DataRetorno.ToNullableDateTime() ?? DateTime.Now;

            foreach (Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti ocorrenciaIntegracaoMulti in retorno.Objeto)
            {
                StringBuilder mensagem = new StringBuilder();
                bool sucesso = true;

                DateTime dataCriacaoOcorrencia = ocorrenciaIntegracaoMulti.DataOcorrencia;

                Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaImportacao = repOcorrenciaImportacaoEmbarcador.BuscarPorProtocoloEGrupoPessoas(ocorrenciaIntegracaoMulti.Protocolo, GrupoPessoas.Codigo);

                if ((GrupoPessoas.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador.HasValue && GrupoPessoas.DataInicialIntegracaoOcorrenciaTMSWSMultiEmbarcador > dataCriacaoOcorrencia) ||
                    (ocorrenciaImportacao != null &&
                    ocorrenciaImportacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Pendente &&
                    ocorrenciaImportacao.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Problemas))
                {
                    if (ocorrenciaImportacao != null)
                        svcOcorrencia.ConfirmarIntegracaoOcorrenciaTransportador(ocorrenciaImportacao.Protocolo);

                    continue;
                }

                unitOfWork.Start();

                if (ocorrenciaImportacao == null)
                    ocorrenciaImportacao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador();

                sucesso = SalvarInformacoesOcorrenciaIntegracaoEmbarcador(out mensagem, false, ocorrenciaImportacao, ocorrenciaIntegracaoMulti, GrupoPessoas, dataConsulta, unitOfWork);

                if (sucesso)
                {
                    ocorrenciaImportacao.Mensagem = "Integrado com sucesso.";
                    ocorrenciaImportacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTes;
                }
                else
                {
                    ocorrenciaImportacao.Mensagem = mensagem.ToString();
                    ocorrenciaImportacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Problemas;
                }

                unitOfWork.CommitChanges();

                if (sucesso)
                    svcOcorrencia.ConfirmarIntegracaoOcorrenciaTransportador(ocorrenciaImportacao.Protocolo);

                unitOfWork.FlushAndClear();
            }

            return true;
        }

        private static bool ImportarCTesOcorrenciaEmbarcador(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaImportacaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);

            mensagemErro = null;

            if (OcorrenciaImportacaoEmbarcador.CTes != null)
                OcorrenciaImportacaoEmbarcador.CTes.Clear();
            else
                OcorrenciaImportacaoEmbarcador.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();

            if (!ConsultarCTes(out mensagemErro, OcorrenciaImportacaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
                return false;

            if (configuracaoTMS.GerarNFSeImportacaoEmbarcador && !ConsultarNFSes(out mensagemErro, OcorrenciaImportacaoEmbarcador, unitOfWork, tipoServicoMultisoftware))
                return false;

            unitOfWork.Start();

            if (OcorrenciaImportacaoEmbarcador.GrupoPessoas?.NaoGerarOcorreciaApenasDocumentos ?? false)
                OcorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Finalizado;
            else
                OcorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoOcorrencia;

            repOcorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

            unitOfWork.CommitChanges();

            return true;
        }

        private static bool SalvarInformacoesOcorrenciaIntegracaoEmbarcador(out StringBuilder mensagem, bool cancelamento, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaImportacao, Dominio.ObjetosDeValor.WebService.Ocorrencia.OcorrenciaIntegracaoMulti OcorrenciaIntegracaoMulti, Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas GrupoPessoas, DateTime dataConsulta, Repositorio.UnitOfWork unitOfWork)
        {
            mensagem = new StringBuilder();
            bool sucesso = true;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            ocorrenciaImportacao.DataConsulta = dataConsulta;
            ocorrenciaImportacao.DataOcorrencia = OcorrenciaIntegracaoMulti.DataOcorrencia;
            ocorrenciaImportacao.Protocolo = OcorrenciaIntegracaoMulti.Protocolo;
            ocorrenciaImportacao.CodigoIntegracaoTipoOcorrencia = OcorrenciaIntegracaoMulti.TipoOcorrencia.CodigoIntegracao;
            ocorrenciaImportacao.ProtocoloCarga = OcorrenciaIntegracaoMulti.ProtocoloCarga;
            ocorrenciaImportacao.GrupoPessoas = GrupoPessoas;
            ocorrenciaImportacao.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Pendente;
            ocorrenciaImportacao.ObservacaoEmbarcador = OcorrenciaIntegracaoMulti.Observacao;
            ocorrenciaImportacao.Descricao = OcorrenciaIntegracaoMulti.Descricao;
            ocorrenciaImportacao.NumeroOcorrenciaEmbarcador = OcorrenciaIntegracaoMulti.Descricao.ToInt();
            ocorrenciaImportacao.Empresa = repEmpresa.BuscarPorCNPJ(OcorrenciaIntegracaoMulti.Empresa.CNPJ);
            ocorrenciaImportacao.Cancelamento = cancelamento;

            if (string.IsNullOrWhiteSpace(ocorrenciaImportacao.CodigoIntegracaoTipoOcorrencia) && !(ocorrenciaImportacao.GrupoPessoas.NaoGerarOcorreciaApenasDocumentos ?? false))
            {
                sucesso = false;
                mensagem.Append("O tipo de ocorrência do embarcador não possui um código de integração. ");
            }

            if (ocorrenciaImportacao.Empresa == null)
            {
                sucesso = false;
                mensagem.Append($"A empresa/filial {OcorrenciaIntegracaoMulti.Empresa.CNPJ} - {OcorrenciaIntegracaoMulti.Empresa.RazaoSocial} não está cadastrada. ");
            }

            if (ocorrenciaImportacao.Codigo > 0)
                repCargaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacao);
            else
                repCargaIntegracaoEmbarcador.Inserir(ocorrenciaImportacao);

            return sucesso;
        }

        private static bool ConsultarCTes(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaImportacaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOocorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);

            Servicos.CTe svcCTe = new Servicos.CTe(unitOfWork);
            Servicos.ServicoSGT.CTe.CTeClient svcWSCTe = ObterClientCTes(OcorrenciaImportacaoEmbarcador.GrupoPessoas.URLIntegracaoMultiEmbarcador, OcorrenciaImportacaoEmbarcador.GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            int limite = 25, inicio = 0, totalRegistros = 0;

            do
            {
                ServicoSGT.CTe.RetornoOfPaginacaoOfCTepIzbOyUQ51p1vPsU retorno = svcWSCTe.BuscarCTesPorOcorrencia(OcorrenciaImportacaoEmbarcador.Protocolo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML, inicio, limite);

                if (!retorno.Status)
                {
                    OcorrenciaImportacaoEmbarcador.Mensagem = retorno.Mensagem;
                    repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.CTe.CTe cte in retorno.Objeto.Itens)
                {
                    if (cte.Modelo != "57" || cte.SituacaoCTeSefaz == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCTeSefaz.Inutilizada)
                        continue;

                    if (string.IsNullOrWhiteSpace(cte.XMLAutorizacao) && string.IsNullOrWhiteSpace(cte.XML))
                    {
                        mensagemErro = $"O CT-e {cte.Numero} não possui um XML.";

                        OcorrenciaImportacaoEmbarcador.Mensagem = mensagemErro;
                        repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);
                        return false;
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cteInserido = repCTe.BuscarPorChave(cte.Chave, OcorrenciaImportacaoEmbarcador.Empresa.Codigo);

                    if (cteInserido == null)
                    {
                        System.IO.MemoryStream memoryStream = !string.IsNullOrWhiteSpace(cte.XMLAutorizacao) ? Utilidades.String.ToStream(cte.XMLAutorizacao) : Utilidades.String.ToStream(cte.XML);

                        object retornoInserir = svcCTe.GerarCTeAnterior(memoryStream, OcorrenciaImportacaoEmbarcador.Empresa.Codigo, string.Empty, string.Empty, unitOfWork, null, true, false, tipoServicoMultisoftware, true);

                        if (retornoInserir.GetType() == typeof(string))
                        {
                            OcorrenciaImportacaoEmbarcador.Mensagem = retorno.Mensagem;
                            repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

                            mensagemErro = (string)retornoInserir;
                            return false;
                        }

                        cteInserido = (Dominio.Entidades.ConhecimentoDeTransporteEletronico)retornoInserir;
                    }

                    OcorrenciaImportacaoEmbarcador.CTes.Add(cteInserido);
                }

                totalRegistros = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += limite;
            }
            while (inicio < totalRegistros);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool ConsultarNFSes(out string mensagemErro, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaImportacaoEmbarcador, Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOocorrenciaImportacaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);

            Servicos.WebService.NFS.NFS svcNFSe = new Servicos.WebService.NFS.NFS(unitOfWork);
            Servicos.ServicoSGT.NFS.NFSClient svcWSNFS = ObterClientNFS(OcorrenciaImportacaoEmbarcador.GrupoPessoas.URLIntegracaoMultiEmbarcador, OcorrenciaImportacaoEmbarcador.GrupoPessoas.TokenIntegracaoMultiEmbarcador);

            int limite = 25, inicio = 0, totalRegistros = 0;

            do
            {
                var request = new Dominio.ObjetosDeValor.WebService.NFS.RequestNFSOcorrencia()
                {
                    ProtocoloOcorrencia = OcorrenciaImportacaoEmbarcador.Protocolo,
                    TipoDocumentoRetorno = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoRetorno.XML,
                    Inicio = inicio,
                    Limite = limite
                };

                ServicoSGT.NFS.RetornoOfPaginacaoOfNFSYRsas_SFX51p1vPsU retorno = svcWSNFS.BuscarNFSsPorOcocorrencia(request);

                if (!retorno.Status)
                {
                    OcorrenciaImportacaoEmbarcador.Mensagem = retorno.Mensagem;
                    repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

                    mensagemErro = retorno.Mensagem;
                    return false;
                }

                foreach (Dominio.ObjetosDeValor.WebService.NFS.NFS nfse in retorno.Objeto.Itens)
                {
                    if (nfse?.NFSe == null || (nfse.NFSe.StatusNFSe != Dominio.Enumeradores.StatusNFSe.Autorizado && nfse.NFSe.StatusNFSe != Dominio.Enumeradores.StatusNFSe.Cancelado))
                        continue;

                    Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(nfse.TransportadoraEmitente.CNPJ));

                    if (empresa == null)
                    {
                        mensagemErro = $"Emitente da NFS-e ({nfse.TransportadoraEmitente.CNPJ} - {nfse.TransportadoraEmitente.RazaoSocial}) não encontrado.";
                        return false;
                    }

                    Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = repCTe.BuscarNFSe(nfse.NFSe.Numero, nfse.NFSe.Serie, empresa.Codigo, nfse.NFSe.CodigoVerificacao);

                    if (nfseGerada == null)
                    {
                        try
                        {
                            nfse.UltimoRetornoSEFAZ = "NFS-e importada automaticamente do embarcador";
                            nfseGerada = svcNFSe.GerarNFSe(nfse, empresa, unitOfWork);
                        }
                        catch (ServicoException ex)
                        {
                            OcorrenciaImportacaoEmbarcador.Mensagem = ex.Message;
                            repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

                            mensagemErro = ex.Message;
                            return false;
                        }
                        catch (Exception ex)
                        {
                            Servicos.Log.TratarErro(ex);

                            OcorrenciaImportacaoEmbarcador.Mensagem = "Ocorreu um erro ao importar a NFS-e do embarcador";
                            repOocorrenciaImportacaoEmbarcador.Atualizar(OcorrenciaImportacaoEmbarcador);

                            mensagemErro = ex.Message;
                            return false;
                        }
                    }

                    OcorrenciaImportacaoEmbarcador.CTes.Add(nfseGerada);
                }

                totalRegistros = retorno.Objeto.NumeroTotalDeRegistro;
                inicio += limite;
            }
            while (inicio < totalRegistros);

            mensagemErro = string.Empty;
            return true;
        }

        private static bool GerarOcorrenciaImportadasEmbarcador(out string mensagemErro, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador ocorrenciaImportacaoEmbarcador, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, Repositorio.UnitOfWork unitOfWork)
        {
            mensagemErro = null;

            Servicos.Embarcador.CargaOcorrencia.Ocorrencia srvOcorrencia = new Servicos.Embarcador.CargaOcorrencia.Ocorrencia(unitOfWork);
            Servicos.Embarcador.Carga.CTeComplementar serCargaCTeComplementar = new Servicos.Embarcador.Carga.CTeComplementar(unitOfWork);
            Servicos.Embarcador.Carga.Ocorrencia serOcorrencia = new Servicos.Embarcador.Carga.Ocorrencia(unitOfWork);

            Repositorio.Embarcador.Cargas.CargaCTe repCargaCte = new Repositorio.Embarcador.Cargas.CargaCTe(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento repCargaOcorrenciaDocumento = new Repositorio.Embarcador.Ocorrencias.CargaOcorrenciaDocumento(unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia repGrupoPessoaTipoOcorrencia = new Repositorio.Embarcador.Pessoas.GrupoPessoaTipoOcorrencia(unitOfWork);
            Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador repOcorrenciaIntegracaoEmbarcador = new Repositorio.Embarcador.Ocorrencias.OcorrenciaImportacaoEmbarcador(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador repCargaIntegracaoEmbarcador = new Repositorio.Embarcador.Cargas.CargaIntegracaoEmbarcador(unitOfWork);

            try
            {
                Dominio.Entidades.TipoDeOcorrenciaDeCTe tipoOcorrencia = repGrupoPessoaTipoOcorrencia.BuscarTipoOcorrenciaPorGrupoPessoasECodigoIntegracao(ocorrenciaImportacaoEmbarcador.GrupoPessoas, ocorrenciaImportacaoEmbarcador.CodigoIntegracaoTipoOcorrencia);

                unitOfWork.Start();

                if (tipoOcorrencia == null)
                {
                    mensagemErro = "O tipo de ocorrência com o código de integração '" + ocorrenciaImportacaoEmbarcador.CodigoIntegracaoTipoOcorrencia + "' não foi encontrado para este grupo de pessoas.";

                    ocorrenciaImportacaoEmbarcador.Mensagem = mensagemErro;

                    repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);

                    unitOfWork.CommitChanges();

                    return true;
                }

                Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoEmbarcador cargaIntegracaoGerada = repCargaIntegracaoEmbarcador.BuscarPorProtocoloEGrupoPessoas(ocorrenciaImportacaoEmbarcador.ProtocoloCarga, ocorrenciaImportacaoEmbarcador.GrupoPessoas.Codigo);
                if (cargaIntegracaoGerada == null || cargaIntegracaoGerada.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCargaIntegracaoEmbarcador.Finalizado)
                {
                    ocorrenciaImportacaoEmbarcador.Mensagem = "Carga com protocolo: " + ocorrenciaImportacaoEmbarcador.ProtocoloCarga + " não foi encontrada. ";
                    ocorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Problemas;

                    repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);

                    unitOfWork.CommitChanges();

                    return false;
                }

                Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia();
                Repositorio.Embarcador.CTe.CTeDocumentoOriginario repDocumentoOriginario = new Repositorio.Embarcador.CTe.CTeDocumentoOriginario(unitOfWork);

                if (cargaIntegracaoGerada.Carga != null)
                {
                    cargaOcorrencia.Carga = cargaIntegracaoGerada.Carga;
                }
                else
                {
                    string chaveCTESubComp = null;
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = null;
                    Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = null;

                    var lstNFSe = ocorrenciaImportacaoEmbarcador.CTes
                                        .Where(o => o.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe);

                    if (lstNFSe != null)
                    {
                        Dominio.Entidades.ConhecimentoDeTransporteEletronico nfseGerada = lstNFSe.FirstOrDefault();
                        documentoOriginario = repDocumentoOriginario.BuscarPrimeiroPorCTe(nfseGerada.Codigo);

                        if (documentoOriginario != null)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarNFSe(documentoOriginario.Numero, documentoOriginario.Serie.ToInt(), nfseGerada.Empresa.Codigo);

                            if (nfse != null)
                                cargaCTe = repCargaCte.BuscarPorCTe(nfse.Codigo);
                        }
                    }
                    else
                    {
                        chaveCTESubComp = ocorrenciaImportacaoEmbarcador.CTes.FirstOrDefault().ChaveCTESubComp;
                        cargaCTe = repCargaCte.BuscarPorChaveCTe(chaveCTESubComp);
                    }

                    if (cargaCTe?.Carga != null)
                    {
                        cargaOcorrencia.Carga = cargaCTe.Carga;
                    }
                    else
                    {
                        if (lstNFSe != null)
                        {
                            if (documentoOriginario != null)
                                ocorrenciaImportacaoEmbarcador.Mensagem = $"Carga originária do NFS-e nro {documentoOriginario.Numero} e série {documentoOriginario.Serie} não foi encontrada. ";
                            else
                                ocorrenciaImportacaoEmbarcador.Mensagem = $"Carga originária não foi encontrada. ";
                        }
                        else
                            ocorrenciaImportacaoEmbarcador.Mensagem = $"Carga originária do CT-e chave acesso {chaveCTESubComp} não foi encontrada. ";

                        ocorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Problemas;
                        repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);
                        unitOfWork.CommitChanges();
                        return false;
                    }
                }

                cargaOcorrencia.DataAlteracao = DateTime.Now;
                cargaOcorrencia.DataOcorrencia = ocorrenciaImportacaoEmbarcador.DataOcorrencia;
                cargaOcorrencia.DataFinalizacaoEmissaoOcorrencia = ocorrenciaImportacaoEmbarcador.DataOcorrencia;
                cargaOcorrencia.NumeroOcorrencia = Servicos.Embarcador.CargaOcorrencia.OcorrenciaSequencial.GetInstance().ObterProximoNumeroSequencial(unitOfWork);
                cargaOcorrencia.Observacao = "Ocorrência gerada automaticamente pelo Embarcador.";
                cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgInformacoes;
                cargaOcorrencia.ValorOcorrencia = ocorrenciaImportacaoEmbarcador.CTes.Sum(x => x.ValorAReceber);
                cargaOcorrencia.ValorOcorrenciaOriginal = ocorrenciaImportacaoEmbarcador.CTes.Sum(x => x.ValorAReceber);
                cargaOcorrencia.ObservacaoCTe = !string.IsNullOrWhiteSpace(cte?.ObservacoesGerais) ? cte.ObservacoesGerais : "";
                cargaOcorrencia.CTeEmitidoNoEmbarcador = true;
                cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;

                if (cargaOcorrencia.Codigo <= 0)
                    repOcorrencia.Inserir(cargaOcorrencia);
                else
                    repOcorrencia.Atualizar(cargaOcorrencia);

                foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOcorrencia in ocorrenciaImportacaoEmbarcador.CTes)
                {
                    Dominio.Entidades.Embarcador.Cargas.CargaCTe cargaCTe = null;
                    Dominio.Entidades.Embarcador.CTe.CTeDocumentoOriginario documentoOriginario = null;

                    if (cteOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                    {
                        documentoOriginario = repDocumentoOriginario.BuscarPrimeiroPorCTe(cteOcorrencia.Codigo);

                        if (documentoOriginario != null)
                        {
                            Dominio.Entidades.ConhecimentoDeTransporteEletronico nfse = repCTe.BuscarNFSe(documentoOriginario.Numero, documentoOriginario.Serie.ToInt(), cteOcorrencia.Empresa.Codigo);

                            if (nfse != null)
                                cargaCTe = repCargaCte.BuscarPorCTe(nfse.Codigo);
                        }
                    }
                    else
                        cargaCTe = repCargaCte.BuscarPorChaveCTe(cteOcorrencia.ChaveCTESubComp);

                    if (cargaCTe == null)
                    {
                        if (cteOcorrencia.ModeloDocumentoFiscal.TipoDocumentoEmissao == Dominio.Enumeradores.TipoDocumento.NFSe)
                        {
                            if (documentoOriginario != null)
                                ocorrenciaImportacaoEmbarcador.Mensagem = $"NFS-e original de nro {documentoOriginario.Numero} e série {documentoOriginario.Serie} não foi encontrado. ";
                            else
                                ocorrenciaImportacaoEmbarcador.Mensagem = $"NFS-e original não encontrado. ";
                        }
                        else
                            ocorrenciaImportacaoEmbarcador.Mensagem = "CT-e original não encontrado: " + cteOcorrencia.ChaveCTESubComp;

                        repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);

                        unitOfWork.CommitChanges();

                        return true;
                    }

                    Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento cargaOcorrenciaDocumento = new Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento
                    {
                        CargaCTe = cargaCTe,
                        CargaOcorrencia = cargaOcorrencia,
                        CTeImportado = cteOcorrencia
                    };

                    repCargaOcorrenciaDocumento.Inserir(cargaOcorrenciaDocumento);

                    cte.CTeSemCarga = false;

                    repCTe.Atualizar(cte);

                    Dominio.Entidades.OcorrenciaDeCTe ocorrenciaDeCTe = srvOcorrencia.GerarOcorrenciaCTe(cargaOcorrencia, cargaCTe, unitOfWork);

                    cargaOcorrenciaDocumento.OcorrenciaDeCTe = ocorrenciaDeCTe;

                    repCargaOcorrenciaDocumento.Atualizar(cargaOcorrenciaDocumento);

                    if (!srvOcorrencia.AjustarCTeImportado(out string erro, cte, cargaCTe.Carga, tipoOcorrencia?.ComponenteFrete, unitOfWork))
                        throw new Exception(erro);
                }

                if (tipoOcorrencia != null && tipoOcorrencia.ComponenteFrete != null)
                {
                    cargaOcorrencia.TipoOcorrencia = tipoOcorrencia;
                    cargaOcorrencia.OrigemOcorrencia = tipoOcorrencia.OrigemOcorrencia;
                    cargaOcorrencia.ComponenteFrete = tipoOcorrencia.ComponenteFrete;
                    cargaOcorrencia.SituacaoOcorrencia = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar;

                    if (tipoOcorrencia.ComponenteFrete.SomarComponenteFreteLiquido)
                        cargaOcorrencia.ValorOcorrenciaLiquida = cargaOcorrencia.ValorOcorrencia;
                    else
                        cargaOcorrencia.ValorOcorrenciaLiquida = 0;

                    repOcorrencia.Atualizar(cargaOcorrencia);

                    serCargaCTeComplementar.ImportarCTesComplementaresParaOcorrencia(cargaOcorrencia, unitOfWork, tipoServicoMultisoftware);

                    Servicos.Embarcador.Integracao.IntegracaoEDI.AdicionarEDIParaIntegracao(cargaOcorrencia, false, tipoServicoMultisoftware, unitOfWork);

                    serOcorrencia.ValidarEnviarEmissaoComplementosOcorrencia(cargaOcorrencia, unitOfWork);
                }
                else
                {
                    mensagemErro = "Tipo ocorrência não encontrada pelo codigo integração: " + ocorrenciaImportacaoEmbarcador.CodigoIntegracaoTipoOcorrencia;
                    ocorrenciaImportacaoEmbarcador.CargaOcorrencia = cargaOcorrencia;
                    ocorrenciaImportacaoEmbarcador.Mensagem = mensagemErro;
                    ocorrenciaImportacaoEmbarcador.Situacao = SituacaoOcorrenciaIntegracaoEmbarcador.Problemas;
                    repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);
                    unitOfWork.CommitChanges();
                    return true;
                }

                if (ocorrenciaImportacaoEmbarcador.Cancelamento)
                    ocorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTesCancelados;
                else
                    ocorrenciaImportacaoEmbarcador.Situacao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Finalizado;

                ocorrenciaImportacaoEmbarcador.Mensagem = "Ocorrencia gerada com sucesso.";
                ocorrenciaImportacaoEmbarcador.CargaOcorrencia = cargaOcorrencia;
                repOcorrenciaIntegracaoEmbarcador.Atualizar(ocorrenciaImportacaoEmbarcador);

                unitOfWork.CommitChanges();

                return true;

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro("Ocorrencia integração não gerada");
                Servicos.Log.TratarErro(ex);
                mensagemErro = ex.Message;
                if (unitOfWork != null)
                    unitOfWork.Rollback();

                return false;
            }

        }

        private static bool ValidarCancelamentoOcorrencia(out string mensagem, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaIntegracaoEmbarcador OcorrenciaIntegracaoEmbarcador, Repositorio.UnitOfWork unitOfWork)
        {
            //por enquanto apenas se estiver finalizada... (daiprai, Lule, Maio 2021)
            Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[] situacoesPermitirCancelamento = new Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia[]
            {
                    Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada
            };

            if (OcorrenciaIntegracaoEmbarcador.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrenciaIntegracaoEmbarcador.Finalizado)
            {
                mensagem = $"A situação da integração ({OcorrenciaIntegracaoEmbarcador.Situacao.ObterDescricao()}) não permite que seja integrado o cancelamento.";
                return false;
            }

            if (OcorrenciaIntegracaoEmbarcador.CargaOcorrencia != null && !situacoesPermitirCancelamento.Contains(OcorrenciaIntegracaoEmbarcador.CargaOcorrencia.SituacaoOcorrencia))
            {
                mensagem = $"A situação da Ocorrência {OcorrenciaIntegracaoEmbarcador.CargaOcorrencia.Codigo} ({OcorrenciaIntegracaoEmbarcador.CargaOcorrencia.SituacaoOcorrencia.ObterDescricao()}) não permite que a mesma seja cancelada.";
                return false;
            }

            mensagem = "";
            return true;
        }

        private static ServicoSGT.Ocorrencia.OcorrenciasClient ObterClientOcorrencia(string url, string token)
        {
            url = url.ToLower();
            if (!url.EndsWith("/"))
                url += "/";
            url += "Ocorrencias.svc";

            ServicoSGT.Ocorrencia.OcorrenciasClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.Ocorrencia.OcorrenciasClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.CTe.CTeClient ObterClientCTes(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "CTe.svc";

            ServicoSGT.CTe.CTeClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.CTe.CTeClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }

        private static ServicoSGT.NFS.NFSClient ObterClientNFS(string url, string token)
        {
            url = url.ToLower();

            if (!url.EndsWith("/"))
                url += "/";

            url += "NFS.svc";

            ServicoSGT.NFS.NFSClient client = null;
            System.ServiceModel.EndpointAddress endpointAddress = new System.ServiceModel.EndpointAddress(url);

            if (url.StartsWith("https"))
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;

                client = new ServicoSGT.NFS.NFSClient(binding, endpointAddress);
            }
            else
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();

                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
                binding.SendTimeout = new TimeSpan(0, 5, 0);

                client = new ServicoSGT.NFS.NFSClient(binding, endpointAddress);
            }

            System.ServiceModel.OperationContextScope scope = new System.ServiceModel.OperationContextScope(client.InnerChannel);
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("Token", "Token", token);
            System.ServiceModel.OperationContext.Current.OutgoingMessageHeaders.Add(header);

            return client;
        }


        #endregion
    }
}
