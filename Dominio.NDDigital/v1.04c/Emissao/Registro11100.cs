using System;

namespace Dominio.NDDigital.v104.Emissao
{
    public class Registro11100 : Registro
    {
        #region Construtores

        public Registro11100(string registro)
            : base(registro)
        {
            this.GerarRegistro();
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Utilizar tabela do IBGE de Código de Unidades da Federação
        /// </summary>
        public int cUF { get; set; }

        /// <summary>
        /// Número aleatório, gerado pelo emitente para cada CT-e para evitar acessos indevidos do CT-e
        /// </summary>
        public int cCT { get; set; }

        public int cfop { get; set; }

        public string natOp { get; set; }

        /// <summary>
        /// 0 - Pago;
        /// 1 - A pagar;
        /// 2 - Outros.
        /// </summary>
        public int forPag { get; set; }

        /// <summary>
        /// Utilizar o código 57 para identificação do CT-e, emitida em substituição aos modelos de conhecimentos atuais
        /// </summary>
        public int mod { get; set; }

        /// <summary>
        /// Preencher com "0" no caso de série única. A série de contingência com autorização pela SRF deve ser "900" ou superior
        /// </summary>
        public int serie { get; set; }

        public int nCT { get; set; }

        /// <summary>
        /// Data e hora de emissão
        /// Formato = AAAA-MM-DDTHH:MM:SS
        /// Preenchido com data e hora de emissão.
        /// </summary>
        public DateTime dhEmi { get; set; }

        /// <summary>
        /// 1 - Retrato
        /// 2 - Paisagem
        /// </summary>
        public int tpImp { get; set; }

        /// <summary>
        /// 1 - Normal;
        /// 4 - Contingência EPEC;
        /// 5 - Contingência FSDA;
        /// 7 - Contingência SVC-RS;
        /// 8 - Contingência SVC-SP.
        /// </summary>
        public int tpEmis { get; set; }

        /// <summary>
        /// Informar o dígito da chave de acesso do CT-e. Será calculado com a aplicação do algoritmo módulo 11 (base 2,9) da chave de acesso
        /// </summary>
        public int cDV { get; set; }

        /// <summary>
        /// 1 - Produção
        ///2 - Homologação
        /// </summary>
        public int tpAmb { get; set; }

        /// <summary>
        /// 0 - CT-e Normal
        /// 1 - CT-e de Complemento de Valores
        /// 2 - CT-e de anulação
        /// 3 - CT-e substituto
        /// </summary>
        public int tpCTe { get; set; }

        /// <summary>
        /// Identificador do processo de emissão do CT -e:
        /// 0 - emissão de CT-e com aplicativo do contribuinte;
        /// 1 - emissão de CT-e avulsa pelo Fisco;
        /// 2 - emissão de CT-e avulsa, pelo contribuinte com seu certificado digital, através do site do Fisco;
        /// 3 - emissão CT-e pelo contribuinte com aplicativo fornecido pelo Fisco.
        /// </summary>
        public int procEmi { get; set; }

        /// <summary>
        /// Identificador da versão do processo de emissão (informar a versão do aplicativo emissor de CT-e). Formato: “NDDigital CTe X.X.X”, onde: X.X.X se refere a versão. Ex: “NDDigital CTe 2.5.7”
        /// </summary>
        public string verProc { get; set; }

        /// <summary>
        /// CT-e cujo débito fora anulado, informado no Tipo do Conhecimento = 2
        /// </summary>
        public string refCTe { get; set; }

        /// <summary>
        /// Código do Município, onde o CT-e está sendo emitido
        /// </summary>
        public int cMunEnv { get; set; }

        /// <summary>
        /// Nome do Município, onde o CT-e está sendo emitido
        /// </summary>
        public string xMunEnv { get; set; }

        /// <summary>
        /// Sigla da UF do local onde o CT-e está sendo emitido
        /// </summary>
        public string UFEnv { get; set; }

        /// <summary>
        /// 01-Rodoviário;
        /// 02-Aéreo;
        /// 03-Aquaviário;
        /// 04-Ferroviário;
        /// 05-Dutoviário.
        /// </summary>
        public int modal { get; set; }

        /// <summary>
        /// 0 - Normal;
        /// 1 - Subcontratação;
        /// 2 – Redespacho;
        /// 3 - Redespacho intermediário
        /// </summary>
        public int tpServ { get; set; }

        /// <summary>
        /// Código do Município do inicio da prestação
        /// </summary>
        public int cMunIni { get; set; }

        /// <summary>
        /// Nome do Município do inicio da prestação
        /// </summary>
        public string xMunIni { get; set; }

        /// <summary>
        /// UF do inicio da prestação
        /// </summary>
        public string UFIni { get; set; }

        /// <summary>
        /// Código do Município do término da prestação
        /// </summary>
        public int cMunFim { get; set; }

        /// <summary>
        /// Nome do Município do término da prestação
        /// </summary>
        public string xMunFim { get; set; }

        /// <summary>
        /// UF do término da prestação
        /// </summary>
        public string UFFim { get; set; }

        /// <summary>
        /// Recebedor retira no Aeroporto, Filial, Porto ou Estação de Destino?
        /// 0-Sim
        /// 1-Não
        /// </summary>
        public int retira { get; set; }

        /// <summary>
        /// Detalhes do retira
        /// </summary>
        public string xDetRetira { get; set; }

        public Registro11110 toma03 { get; set; }

        public Registro11120 toma4 { get; set; }

        #endregion

        #region Métodos

        protected override void GerarRegistro()
        {
            string[] dados = this.StringRegistro.Split(';');

            this.Identificador = this.ObterString(dados[0]);
            this.cUF = this.ObterNumero(dados[1]);
            this.cCT = this.ObterNumero(dados[2]);
            this.cfop = this.ObterNumero(dados[3]);
            this.natOp = this.ObterString(dados[4]);
            this.forPag = this.ObterNumero(dados[5]);
            this.mod = this.ObterNumero(dados[6]);
            this.serie = this.ObterNumero(dados[7]);
            this.nCT = this.ObterNumero(dados[8]);
            this.dhEmi = this.ObterData(dados[9], "yyyy-MM-ddTHH:mm:ss");
            this.tpImp = this.ObterNumero(dados[10]);
            this.tpEmis = this.ObterNumero(dados[11]);
            this.cDV = this.ObterNumero(dados[12]);
            this.tpAmb = this.ObterNumero(dados[13]);
            this.tpCTe = this.ObterNumero(dados[14]);
            this.procEmi = this.ObterNumero(dados[15]);
            this.verProc = this.ObterString(dados[16]);
            this.refCTe = this.ObterString(dados[17]);
            this.cMunEnv = this.ObterNumero(dados[18]);
            this.xMunEnv = this.ObterString(dados[19]);
            this.UFEnv = this.ObterString(dados[20]);
            this.modal = this.ObterNumero(dados[21]);
            this.tpServ = this.ObterNumero(dados[22]);
            this.cMunIni = this.ObterNumero(dados[23]);
            this.xMunIni = this.ObterString(dados[24]);
            this.UFIni = this.ObterString(dados[25]);
            this.cMunFim = this.ObterNumero(dados[26]);
            this.xMunFim = this.ObterString(dados[27]);
            this.UFFim = this.ObterString(dados[28]);
            this.retira = this.ObterNumero(dados[29]);
            this.xDetRetira = this.ObterString(dados[30]);
        }

        #endregion
    }
}
