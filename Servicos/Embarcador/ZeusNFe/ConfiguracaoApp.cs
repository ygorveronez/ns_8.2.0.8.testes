using System;
using System.IO;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using NFe.Utils;
using NFe.Utils.Email;
using NFe.Danfe.Base;

namespace Zeus.Embarcador.ZeusNFe
{
    public class ConfiguracaoApp
    {
        private ConfiguracaoServico _cfgServico;

        public ConfiguracaoApp()
        {
            CfgServico = ConfiguracaoServico.Instancia;
            CfgServico.tpAmb = DFe.Classes.Flags.TipoAmbiente.Homologacao;
            CfgServico.tpEmis = TipoEmissao.teNormal;
            Emitente = new emit { CPF = "", CRT = CRT.SimplesNacional };
            EnderecoEmitente = new enderEmit();
            ConfiguracaoEmail = new ConfiguracaoEmail("nfe@commerce.inf.br", "cesaoexp18", "Envio de NF-e", "Mensagem do e-mail", "smtp.commerce.inf.br", 587, false, true);
            ConfiguracaoCsc = new ConfiguracaoCsc("000001", "");
            ConfiguracaoDanfeNfe = new NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe();
            ConfiguracaoDanfeNfce = new NFe.Danfe.Base.NFCe.ConfiguracaoDanfeNfce(NfceDetalheVendaNormal.UmaLinha, NfceDetalheVendaContigencia.UmaLinha);
        }

        public ConfiguracaoServico CfgServico
        {
            get
            {
                Funcoes.CopiarPropriedades(_cfgServico, ConfiguracaoServico.Instancia);
                return _cfgServico;
            }
            set
            {
                _cfgServico = value;
                Funcoes.CopiarPropriedades(value, ConfiguracaoServico.Instancia);
            }
        }

        public emit Emitente { get; set; }
        public enderEmit EnderecoEmitente { get; set; }
        public ConfiguracaoEmail ConfiguracaoEmail { get; set; }
        public ConfiguracaoCsc ConfiguracaoCsc { get; set; }
        public NFe.Danfe.Base.NFe.ConfiguracaoDanfeNfe ConfiguracaoDanfeNfe { get; set; }
        public NFe.Danfe.Base.NFCe.ConfiguracaoDanfeNfce ConfiguracaoDanfeNfce { get; set; }

        /// <summary>
        ///     Salva os dados de CfgServico em um arquivo XML
        /// </summary>
        /// <param name="arquivo">Arquivo XML onde será salvo os dados</param>
        public void SalvarParaAqruivo(string arquivo)
        {
            var camposEmBranco = Funcoes.ObterPropriedadesEmBranco(CfgServico);

            var propinfo = Funcoes.ObterPropriedadeInfo(_cfgServico, c => c.DiretorioSalvarXml);
            camposEmBranco.Remove(propinfo.Name);

            if (camposEmBranco.Count > 0)
                throw new Exception("Informe os dados abaixo antes de salvar as Configurações:" + Environment.NewLine + string.Join(", ", camposEmBranco.ToArray()));

            var dir = Path.GetDirectoryName(arquivo);
            if (dir != null && !Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException("Diretório " + dir + " não encontrado!");
            }

            DFe.Utils.FuncoesXml.ClasseParaArquivoXml(this, arquivo);
        }
    }
}