using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.MDFe
{
    public class MDFe
    {
        #region Métodos Globais

        public static Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe ConverterProcMDFeParaMDFePorObjeto(object mdfeProc)
        {
            if (mdfeProc.GetType() == typeof(MultiSoftware.MDFe.v100a.mdfeProc))
                return ConverterProcMDFeParaMDFe((MultiSoftware.MDFe.v100a.mdfeProc)mdfeProc);
            else if (mdfeProc.GetType() == typeof(MultiSoftware.MDFe.v300.mdfeProc))
                return ConverterProcMDFeParaMDFe((MultiSoftware.MDFe.v300.mdfeProc)mdfeProc);

            return null;
        }

        public static Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe ConverterProcMDFeParaMDFe(MultiSoftware.MDFe.v100a.mdfeProc mdfeProc)
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe = new Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe();

            SetarEmitente(ref mdfe, mdfeProc.MDFe.infMDFe.emit);

            return mdfe;
        }

        public static Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe ConverterProcMDFeParaMDFe(MultiSoftware.MDFe.v300.mdfeProc mdfeProc)
        {
            Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe = new Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe();

            SetarEmitente(ref mdfe, mdfeProc.MDFe.infMDFe.emit);

            return mdfe;
        }

        public static void ConsultarSituacaoMDFesAutorizados(Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.ManifestoEletronicoDeDocumentosFiscais repMDFe = new Repositorio.ManifestoEletronicoDeDocumentosFiscais(unitOfWork);

            List<int> codigosMDFes = repMDFe.BuscarMDFesAutorizadosParaConsultaSituacao(300);

            foreach (int codigoMDFe in codigosMDFes)
            {
                Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe = repMDFe.BuscarPorCodigoComFetch(codigoMDFe);
                mdfe.DataUltimaConsultaSituacaoMDFe = DateTime.Now;

                if (mdfe == null || (mdfe != null && mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado))
                    continue;

                if (!ConsultarSituacaoMDFe(out string erro, mdfe, unitOfWork))
                    Servicos.Log.TratarErro(erro);

                unitOfWork.FlushAndClear();
            }
        }

        public static bool ConsultarSituacaoMDFe(out string erro, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            erro = string.Empty;

            if (mdfe.Status != Dominio.Enumeradores.StatusMDFe.Autorizado)
            {
                erro = "É necessário que o MDF-e esteja autorizado para a consulta da situação do mesmo.";
                return false;
            }

            try
            {
                if (MultiSoftware.MDFe.Servicos.ConsultaSituacaoMDFe.Consultar(out string xmlRetorno, out MultiSoftware.MDFe.v300.ConsultaSituacaoMDFe.TRetConsSitMDFe retornoConsulta, out erro, mdfe.Chave, (MultiSoftware.MDFe.v300.TAmb)mdfe.TipoAmbiente, (MultiSoftware.MDFe.v300.TCodUfIBGE)mdfe.Empresa.Localidade.Estado.CodigoIBGE, mdfe.Empresa.NomeCertificado, mdfe.Empresa.SenhaCertificado))
                    return ProcessarRetornoConsultaMDFe(out erro, xmlRetorno, retornoConsulta, mdfe, unitOfWork);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                erro = "Ocorreu uma falha ao consultar a situação do MDF-e.";
            }

            return false;
        }

        #endregion

        #region Métodos Privados

        private static bool ProcessarRetornoConsultaMDFe(out string erro, string xmlRetorno, MultiSoftware.MDFe.v300.ConsultaSituacaoMDFe.TRetConsSitMDFe retornoConsulta, Dominio.Entidades.ManifestoEletronicoDeDocumentosFiscais mdfe, Repositorio.UnitOfWork unitOfWork)
        {
            erro = string.Empty;

            if (retornoConsulta.cStat == "100")
                return true;

            if (retornoConsulta.cStat != "101" && retornoConsulta.cStat != "132")
            {
                erro = $"Consulta de situação do MDF-e [{mdfe.Chave}]: {retornoConsulta.cStat} - {retornoConsulta.xMotivo}";
                return false;
            }

            Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema,
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema
            };

            Servicos.MDFe servicoMDFe = new Servicos.MDFe(unitOfWork);

            for (int i = 0; i < retornoConsulta.procEventoMDFe.Length; i++)
            {
                MultiSoftware.MDFe.v300.TProcEvento procEventoMDFe = retornoConsulta.procEventoMDFe[i];

                if (procEventoMDFe.eventoMDFe.infEvento.tpEvento != "110112" && procEventoMDFe.eventoMDFe.infEvento.tpEvento != "110111")
                    continue;

                System.Xml.Linq.XDocument doc = System.Xml.Linq.XDocument.Parse(xmlRetorno);

                string xmlEvento = doc.Descendants(doc.Root.Name.Namespace + "procEventoMDFe").ElementAt(i).ToString(System.Xml.Linq.SaveOptions.DisableFormatting);

                if (procEventoMDFe.eventoMDFe.infEvento.tpEvento == "110112")
                    return string.IsNullOrEmpty(servicoMDFe.EncerrarMDFeImportadoAsync(mdfe, procEventoMDFe, null, xmlEvento, auditado).GetAwaiter().GetResult());
                else if (procEventoMDFe.eventoMDFe.infEvento.tpEvento == "110111")
                    return string.IsNullOrEmpty(servicoMDFe.CancelarMDFeImportadoAsync(mdfe, procEventoMDFe, null, xmlEvento, auditado).GetAwaiter().GetResult());
            }

            return true;
        }

        private static void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe, MultiSoftware.MDFe.v100a.TMDFeInfMDFeEmit infEmit)
        {
            if (infEmit != null)
            {
                mdfe.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                mdfe.Emitente.CNPJ = infEmit.CNPJ;
                mdfe.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                mdfe.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                mdfe.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                mdfe.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                mdfe.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                mdfe.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                mdfe.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                mdfe.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                mdfe.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                mdfe.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                mdfe.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                mdfe.Emitente.IE = infEmit.IE;
                mdfe.Emitente.NomeFantasia = infEmit.xFant;
                mdfe.Emitente.RazaoSocial = infEmit.xNome;
                mdfe.Emitente.RNTRC = "";
                mdfe.Emitente.SimplesNacional = true;
            }
        }

        private static void SetarEmitente(ref Dominio.ObjetosDeValor.Embarcador.MDFe.MDFe mdfe, MultiSoftware.MDFe.v300.TMDFeInfMDFeEmit infEmit)
        {
            if (infEmit != null)
            {
                mdfe.Emitente = new Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa();
                mdfe.Emitente.CNPJ = infEmit.Item;
                mdfe.Emitente.Endereco = new Dominio.ObjetosDeValor.Embarcador.Localidade.Endereco();
                mdfe.Emitente.Endereco.Bairro = infEmit.enderEmit.xBairro;
                mdfe.Emitente.Endereco.CEP = infEmit.enderEmit.CEP;
                mdfe.Emitente.Endereco.Cidade = new Dominio.ObjetosDeValor.Localidade();
                mdfe.Emitente.Endereco.Cidade.IBGE = int.Parse(infEmit.enderEmit.cMun);
                mdfe.Emitente.Endereco.Cidade.Descricao = infEmit.enderEmit.xMun;
                mdfe.Emitente.Endereco.Cidade.SiglaUF = infEmit.enderEmit.UF.ToString("g");
                mdfe.Emitente.Endereco.Complemento = infEmit.enderEmit.xCpl;
                mdfe.Emitente.Endereco.Logradouro = infEmit.enderEmit.xLgr;
                mdfe.Emitente.Endereco.Numero = infEmit.enderEmit.nro;
                mdfe.Emitente.Endereco.Telefone = infEmit.enderEmit.fone;
                mdfe.Emitente.IE = infEmit.IE;
                mdfe.Emitente.NomeFantasia = infEmit.xFant;
                mdfe.Emitente.RazaoSocial = infEmit.xNome;
                mdfe.Emitente.RNTRC = "";
                mdfe.Emitente.SimplesNacional = true;
            }
        }

        #endregion
    }
}
