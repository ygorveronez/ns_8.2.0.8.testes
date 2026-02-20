using Repositorio;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zen.Barcode;

namespace EmissaoCTe.Relatorio
{
    static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                CriptografarAppConfig("connectionStrings");
                Servicos.Http.HttpClientRegistration.RegisterClients();

                if (args.Count() < 4)
                    throw new Exception("Quantidade de argumentos inválidos para a geração do relatório.");

                switch (args[0])
                {
                    case "DACTE":
                        GerarDACTE(args);
                        break;
                    default:
                        return 0;
                }

                return 1;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                return 0;
            }
            finally
            {
                Application.Exit();
            }
        }

        private static void CriptografarAppConfig(string section, string provider = "RSAProtectedConfigurationProvider")
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ConfigurationSection confStrSect = config.GetSection(section);

            if (confStrSect != null && !confStrSect.SectionInformation.IsProtected)
            {
                confStrSect.SectionInformation.ProtectSection(provider);
                config.Save();
            }
        }

        private static void GerarDACTE(string[] args)
        {
            int codigoCTe = 0;
            int.TryParse(args[1], out codigoCTe);

            string folderPath = args[2];
            string fileName = args[3];

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ControleCTe"].ConnectionString);

            Dominio.ObjetosDeValor.Relatorios.DACTE dacte = null;
            List<Dominio.Entidades.DocumentosCTE> documentos = null;
            List<Dominio.Entidades.VeiculoCTE> veiculos = null;
            List<Dominio.Entidades.MotoristaCTE> motoristas = null;

            try
            {
                connection.Open();

                dacte = ObterDadosCTe(codigoCTe, connection);
                veiculos = ObterVeiculos(codigoCTe, connection);
                motoristas = ObterMotoristas(codigoCTe, connection);

                documentos = new List<Dominio.Entidades.DocumentosCTE>();

                if (dacte.TipoCTe == Dominio.Enumeradores.TipoCTE.Complemento && !string.IsNullOrWhiteSpace(dacte.ChaveCTeSubcontratacaoComplementar) && dacte.ChaveCTeSubcontratacaoComplementar.Length == 44)
                {
                    Dominio.Entidades.DocumentosCTE documentoCTe = new Dominio.Entidades.DocumentosCTE()
                    {
                        ChaveNFE = dacte.ChaveCTeSubcontratacaoComplementar
                    };

                    documentoCTe.Numero = documentoCTe.NumeroOuNumeroDaChave;
                    documentoCTe.Serie = documentoCTe.SerieOuSerieDaChave;
                    documentoCTe.ModeloDocumentoFiscal = new Dominio.Entidades.ModeloDocumentoFiscal() { Numero = "57" };

                    documentos.Add(documentoCTe);
                }

                documentos.AddRange(ObterDocumentos(codigoCTe, connection));

                dacte.TotalDocumentos = documentos != null ? documentos.Count : 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
            }
            finally
            {
                connection.Close();
            }

            UnitOfWork unitOfWork = new UnitOfWork(Conexao.StringConexao);
            try
            {
                Servicos.DACTE svcDACTE = new Servicos.DACTE(unitOfWork);

                byte[] bytes = svcDACTE.Gerar(codigoCTe, dacte, documentos, veiculos, motoristas);

                string filePath = Utilidades.IO.FileStorageService.Storage.Combine(folderPath, fileName);

                using (System.IO.Stream fs = Utilidades.IO.FileStorageService.Storage.OpenWrite(filePath))
                    fs.Write(bytes, 0, bytes.Length);
            }
            finally 
            {
                unitOfWork.Dispose();
            }
        }

        public static List<Dominio.Entidades.DocumentosCTE> ObterDocumentos(int codigoCTe, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = "SELECT documento.NFC_BCICMS, documento.NFC_BCICMSST, documento.NFC_CFOP, documento.NFC_CHAVENFE, documento.NFC_CNPJ_REMETENTE, documento.NFC_CODIGO, documento.NFC_DATAEMISSAO, documento.NFC_DESCRICAO, documento.NFC_ITEMPRINCIPAL, documento.NFC_NUMERO, documento.NFC_PESO, documento.NFC_PIN_SUFRAMA, documento.NFC_SERIE, documento.NFC_SUB_SERIE, documento.NFC_VALOR, documento.NFC_VALORICMS, documento.NFC_VALORICMSST, documento.NFC_VALORPRODUTOS, documento.NFC_VOLUME, modelo.MOD_NUM FROM T_CTE_DOCS documento left outer join T_MODDOCFISCAL modelo on documento.MOD_CODIGO = modelo.MOD_CODIGO WHERE documento.CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            List<Dominio.Entidades.DocumentosCTE> documentos = new List<Dominio.Entidades.DocumentosCTE>();

            while (reader.Read())
            {
                documentos.Add(new Dominio.Entidades.DocumentosCTE()
                {
                    BaseCalculoICMS = reader["NFC_BCICMS"] != DBNull.Value ? (decimal)reader["NFC_BCICMS"] : 0m,
                    BaseCalculoICMSST = reader["NFC_BCICMSST"] != DBNull.Value ? (decimal)reader["NFC_BCICMSST"] : 0m,
                    CFOP = reader["NFC_CFOP"] != DBNull.Value ? (string)reader["NFC_CFOP"] : string.Empty,
                    ChaveNFE = reader["NFC_CHAVENFE"] != DBNull.Value ? (string)reader["NFC_CHAVENFE"] : string.Empty,
                    CNPJRemetente = reader["NFC_CNPJ_REMETENTE"] != DBNull.Value ? (string)reader["NFC_CNPJ_REMETENTE"] : string.Empty,
                    DataEmissao = reader["NFC_DATAEMISSAO"] != DBNull.Value ? (DateTime)reader["NFC_DATAEMISSAO"] : DateTime.MinValue,
                    Descricao = reader["NFC_DESCRICAO"] != DBNull.Value ? (string)reader["NFC_DESCRICAO"] : string.Empty,
                    ItemPrincipal = reader["NFC_ITEMPRINCIPAL"] != DBNull.Value ? (string)reader["NFC_ITEMPRINCIPAL"] : string.Empty,
                    ModeloDocumentoFiscal = reader["MOD_NUM"] != DBNull.Value ? new Dominio.Entidades.ModeloDocumentoFiscal() { Numero = (string)reader["MOD_NUM"] } : null,
                    Numero = reader["NFC_NUMERO"] != DBNull.Value ? (string)reader["NFC_NUMERO"] : string.Empty,
                    Peso = reader["NFC_PESO"] != DBNull.Value ? (decimal)reader["NFC_PESO"] : 0m,
                    PINSuframa = reader["NFC_PIN_SUFRAMA"] != DBNull.Value ? (string)reader["NFC_PIN_SUFRAMA"] : string.Empty,
                    Serie = reader["NFC_SERIE"] != DBNull.Value ? (string)reader["NFC_SERIE"] : string.Empty,
                    SubSerie = reader["NFC_SUB_SERIE"] != DBNull.Value ? (string)reader["NFC_SUB_SERIE"] : string.Empty,
                    Valor = reader["NFC_VALOR"] != DBNull.Value ? (decimal)reader["NFC_VALOR"] : 0m,
                    ValorICMS = reader["NFC_VALORICMS"] != DBNull.Value ? (decimal)reader["NFC_VALORICMS"] : 0m,
                    ValorICMSST = reader["NFC_VALORICMSST"] != DBNull.Value ? (decimal)reader["NFC_VALORICMSST"] : 0m,
                    ValorProdutos = reader["NFC_VALORPRODUTOS"] != DBNull.Value ? (decimal)reader["NFC_VALORPRODUTOS"] : 0m,
                    Volume = reader["NFC_VOLUME"] != DBNull.Value ? (int)reader["NFC_VOLUME"] : 0
                });
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();

            return documentos;
        }

        public static List<Dominio.Entidades.MotoristaCTE> ObterMotoristas(int codigoCTe, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = "SELECT CMO_CPF_MOTORISTA, CMO_NOME_MOTORISTA FROM T_CTE_MOTORISTA WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            List<Dominio.Entidades.MotoristaCTE> motoristas = new List<Dominio.Entidades.MotoristaCTE>();

            while (reader.Read())
            {
                motoristas.Add(new Dominio.Entidades.MotoristaCTE()
                {
                    CPFMotorista = reader["CMO_CPF_MOTORISTA"] != DBNull.Value ? (string)reader["CMO_CPF_MOTORISTA"] : string.Empty,
                    NomeMotorista = reader["CMO_NOME_MOTORISTA"] != DBNull.Value ? (string)reader["CMO_NOME_MOTORISTA"] : string.Empty
                });
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();

            return motoristas;
        }

        public static List<Dominio.Entidades.VeiculoCTE> ObterVeiculos(int codigoCTe, SqlConnection connection)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = "SELECT veiculo.CVE_CAPACIDADE_KG, veiculo.CVE_CAPACIDADE_M3, veiculo.CVE_PLACA, veiculo.CVE_RENAVAM, veiculo.CVE_TARA, veiculo.CVE_TIPO_CARROCERIA, veiculo.CVE_TIPO_PROPRIEDADE, veiculo.CVE_TIPO_RODADO, veiculo.CVE_TIPO_VEICULO, veiculo.UF_SIGLA, proprietario.PVE_CODIGO, proprietario.PVE_CPF_CNPJ, proprietario.PVE_IE, proprietario.PVE_NOME, proprietario.PVE_RNTRC, proprietario.PVE_TIPO  FROM T_CTE_VEICULO veiculo left outer join T_CTE_VEICULO_PROPRIETARIO proprietario on veiculo.PVE_CODIGO = proprietario.PVE_CODIGO WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            List<Dominio.Entidades.VeiculoCTE> veiculos = new List<Dominio.Entidades.VeiculoCTE>();

            while (reader.Read())
            {
                Dominio.Entidades.VeiculoCTE veiculo = new Dominio.Entidades.VeiculoCTE()
                {
                    CapacidadeKG = reader["CVE_CAPACIDADE_KG"] != DBNull.Value ? (int)reader["CVE_CAPACIDADE_KG"] : 0,
                    CapacidadeM3 = reader["CVE_CAPACIDADE_M3"] != DBNull.Value ? (int)reader["CVE_CAPACIDADE_M3"] : 0,
                    Estado = new Dominio.Entidades.Estado() { Sigla = (string)reader["UF_SIGLA"] },
                    Placa = reader["CVE_PLACA"] != DBNull.Value ? (string)reader["CVE_PLACA"] : string.Empty,
                    RENAVAM = reader["CVE_RENAVAM"] != DBNull.Value ? (string)reader["CVE_RENAVAM"] : string.Empty,
                    Tara = reader["CVE_TARA"] != DBNull.Value ? (int)reader["CVE_TARA"] : 0,
                    TipoCarroceria = reader["CVE_TIPO_CARROCERIA"] != DBNull.Value ? (string)reader["CVE_TIPO_CARROCERIA"] : string.Empty,
                    TipoPropriedade = reader["CVE_TIPO_PROPRIEDADE"] != DBNull.Value ? (string)reader["CVE_TIPO_PROPRIEDADE"] : string.Empty,
                    TipoRodado = reader["CVE_TIPO_RODADO"] != DBNull.Value ? (string)reader["CVE_TIPO_RODADO"] : string.Empty,
                    TipoVeiculo = reader["CVE_TIPO_VEICULO"] != DBNull.Value ? (string)reader["CVE_TIPO_VEICULO"] : string.Empty
                };

                if (reader["PVE_CODIGO"] != DBNull.Value)
                {
                    veiculo.Proprietario = new Dominio.Entidades.ProprietarioVeiculoCTe()
                    {
                        CPF_CNPJ = reader["PVE_CPF_CNPJ"] != DBNull.Value ? (string)reader["PVE_CPF_CNPJ"] : string.Empty,
                        IE = reader["PVE_IE"] != DBNull.Value ? (string)reader["PVE_IE"] : string.Empty,
                        Nome = reader["PVE_NOME"] != DBNull.Value ? (string)reader["PVE_NOME"] : string.Empty,
                        RNTRC = reader["PVE_RNTRC"] != DBNull.Value ? (string)reader["PVE_RNTRC"] : string.Empty,
                        Tipo = reader["PVE_TIPO"] != DBNull.Value ? (Dominio.Enumeradores.TipoProprietarioVeiculo)reader["PVE_TIPO"] : Dominio.Enumeradores.TipoProprietarioVeiculo.Outros
                    };
                }

                veiculos.Add(veiculo);
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();

            return veiculos;
        }

        private static Dominio.ObjetosDeValor.Relatorios.DACTE ObterDadosCTe(int codigoCTe, SqlConnection connection)
        {
            Dominio.ObjetosDeValor.Relatorios.DACTE dacte = null;

            SqlCommand command = new SqlCommand();

            command.CommandText = @"SELECT CON_CODIGO, MOA_CODIGO, cte.NAT_CODIGO, cte.CFO_CODIGO, cte.EMP_CODIGO, CON_NUM, CON_SERIE, CON_CHAVECTE, CON_PAGOAPAGAR, CON_DATAHORAEMISSAO, CON_IMPRESSAO, CON_TOMADOR,
                                    CON_TPEMIS, CON_CARAC_TRANSP, CON_CARAC_SERV, CON_TIPO_AMBIENTE, CON_TIPO_CTE, CON_TIPO_SERVICO, CON_RETIRA, CON_DETALHES_RETIRA, CON_VALOR_PREST_SERVICO, 
                                    CON_VALOR_RECEBER, CON_CST, CON_BC_ICMS, CON_ALIQ_ICMS, CON_VAL_ICMS, CON_PER_RED_BC_ICMS, CON_VAL_PRESUMIDO, CON_VAL_ICMS_DEVIDO, CON_SIMPLES_NAC, CON_VALOR_TOTAL_MERC, 
                                    CON_PRODUTO_PRED, CON_OBS_CARGA, CON_RNTRC, CON_LOTACAO, CON_CIOT, CON_OBSGERAIS, CON_LOCEMISSAO, CON_LOCINICIOPRESTACAO, CON_LOCTERMINOPRESTACAO, CON_PROTOCOLO, 
                                    CON_RETORNOCTEDATA, CON_STATUS, CON_OBS_FISCO_IMPOSTOS, CON_INCLUIR_ICMS, CON_PERCENTUAL_INCLUIR, CON_VALOR_FRETE, CON_EXIBE_ICMS_DACTE, CON_OUTRAS_CARAC_CARGA, 
                                    CON_END_DESTINATARIO, CON_END_RECEBEDOR, CON_END_TOMADOR, CON_IMPORTACAO_PRE_CTE, CON_STATUS_IMPORTACAO_PRE_CTE, CON_OBSDIGITACAO, CON_INFORMACAO_ADICIONAL_FISCO, 
                                    CON_NUMERO_RECIBO, CON_REMETENTE_CTE, CON_EXPEDIDOR_CTE, CON_DESTINATARIO_CTE, CON_RECEBEDOR_CTE, CON_TOMADOR_CTE, CON_DATA_INTEGRACAO, CON_OBS_AVANCADAS, 
                                    CON_VALOR_ICMS_UF_DESTINO, empresa.EMP_RAZAO EMPRESA_NOME, CON_CHAVE_CTE_SUB_COMP,
                                    empresa.EMP_ENDERECO EMPRESA_ENDERECO, empresa.EMP_NUMERO EMPRESA_NUMERO, empresa.EMP_BAIRRO EMPRESA_BAIRRO, empresa.EMP_CEP EMPRESA_CEP, empresa.EMP_CNPJ EMPRESA_CNPJ, 
                                    empresa.EMP_INSCRICAO EMPRESA_IE, empresa.EMP_REGISTROANTT EMPRESA_RNTRC, empresa.EMP_FONE EMPRESA_TELEFONE, empresa.EMP_CAMINHOLOGO EMPRESA_CAMINHO_LOGO_DACTE, 
                                    localidadeEmpresa.LOC_DESCRICAO EMPRESA_CIDADE, localidadeEmpresa.UF_SIGLA EMPRESA_UF, localidadeInicioPrestacao.LOC_DESCRICAO LOCALIDADE_INICIO_PRESTACAO, 
                                    localidadeInicioPrestacao.UF_SIGLA UF_INICIO_PRESTACAO, localidadeTerminoPrestacao.LOC_DESCRICAO LOCALIDADE_TERMINO_PRESTACAO, 
                                    localidadeTerminoPrestacao.UF_SIGLA UF_TERMINO_PRESTACAO, modelo.MOD_NUM NUMERO_MODELO, modelo.MOD_DESCRICAO as MODELO_DESCRICAO, modelo.MOD_ABREVIACAO AS MODELO_ABREVIACAO, cfop.CFO_CFOP NUMERO_CFOP,natureza.NAT_DESCRICAO NATUREZA_OPERACAO, serie.ESE_NUMERO NUMERO_SERIE, 
                                    REMETENTE_CPF_CNPJ = CASE remetente.PCT_EXTERIOR WHEN 0 THEN remetente.PCT_CPF_CNPJ WHEN 1 THEN '00.000.000/0000-00' ELSE NULL END, 
                                    REMETENTE_IE_RG = CASE remetente.PCT_EXTERIOR WHEN 0 THEN remetente.PCT_IERG WHEN 1 THEN 'ISENTO' ELSE NULL END, 
                                    remetente.PCT_ENDERECO REMETENTE_ENDERECO,remetente.PCT_BAIRRO REMETENTE_BAIRRO, remetente.PCT_NUMERO REMETENTE_NUMERO, remetente.PCT_FONE REMETENTE_TELEFONE, remetente.PCT_CEP REMETENTE_CEP, 
                                    REMETENTE_NOME = CASE WHEN cte.CON_TIPO_AMBIENTE = 1 THEN remetente.PCT_NOME WHEN cte.CON_TIPO_AMBIENTE = 2 AND remetente.PCT_CPF_CNPJ is not null THEN 'CT-e EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO' END, 
                                    remetente.PCT_EXTERIOR REMETENTE_EXTERIOR, 
                                    REMETENTE_CIDADE = CASE remetente.PCT_EXTERIOR WHEN 0 THEN localidadeRemetente.LOC_DESCRICAO WHEN 1 THEN remetente.PCT_CIDADE ELSE NULL END, 
                                    REMETENTE_SIGLA_ESTADO = CASE remetente.PCT_EXTERIOR WHEN 0 THEN ufRemetente.UF_SIGLA WHEN 1 THEN 'EX' ELSE NULL END, 
                                    REMETENTE_PAIS = CASE remetente.PCT_EXTERIOR WHEN 0 THEN paisRemetente.PAI_NOME WHEN 1 THEN paisExteriorRemetente.PAI_NOME ELSE NULL END, 
                                    RECEBEDOR_CPF_CNPJ = CASE recebedor.PCT_EXTERIOR WHEN 0 THEN recebedor.PCT_CPF_CNPJ WHEN 1 THEN '00.000.000/0000-00' ELSE NULL END, 
                                    RECEBEDOR_IE_RG = CASE recebedor.PCT_EXTERIOR WHEN 0 THEN recebedor.PCT_IERG WHEN 1 THEN 'ISENTO' ELSE NULL END, 
                                    recebedor.PCT_ENDERECO RECEBEDOR_ENDERECO, recebedor.PCT_BAIRRO RECEBEDOR_BAIRRO, recebedor.PCT_NUMERO RECEBEDOR_NUMERO, recebedor.PCT_FONE RECEBEDOR_TELEFONE, recebedor.PCT_CEP RECEBEDOR_CEP, 
                                    RECEBEDOR_NOME = CASE WHEN cte.CON_TIPO_AMBIENTE = 1 THEN recebedor.PCT_NOME WHEN cte.CON_TIPO_AMBIENTE = 2 AND recebedor.PCT_CPF_CNPJ is not null THEN 'CT-e EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO' END, 
                                    recebedor.PCT_EXTERIOR RECEBEDOR_EXTERIOR, 
                                    RECEBEDOR_CIDADE = CASE recebedor.PCT_EXTERIOR WHEN 0 THEN localidadeRecebedor.LOC_DESCRICAO WHEN 1 THEN recebedor.PCT_CIDADE ELSE NULL END, 
                                    RECEBEDOR_SIGLA_ESTADO = CASE recebedor.PCT_EXTERIOR WHEN 0 THEN ufRecebedor.UF_SIGLA WHEN 1 THEN 'EX' ELSE NULL END, 
                                    RECEBEDOR_PAIS = CASE recebedor.PCT_EXTERIOR WHEN 0 THEN paisRecebedor.PAI_NOME WHEN 1 THEN paisExteriorRecebedor.PAI_NOME ELSE NULL END, 
                                    EXPEDIDOR_CPF_CNPJ = CASE expedidor.PCT_EXTERIOR WHEN 0 THEN expedidor.PCT_CPF_CNPJ WHEN 1 THEN '00.000.000/0000-00' ELSE NULL END, 
                                    EXPEDIDOR_IE_RG = CASE expedidor.PCT_EXTERIOR WHEN 0 THEN expedidor.PCT_IERG WHEN 1 THEN 'ISENTO' ELSE NULL END, 
                                    expedidor.PCT_ENDERECO EXPEDIDOR_ENDERECO, expedidor.PCT_BAIRRO EXPEDIDOR_BAIRRO, expedidor.PCT_NUMERO EXPEDIDOR_NUMERO, expedidor.PCT_FONE EXPEDIDOR_TELEFONE, expedidor.PCT_CEP EXPEDIDOR_CEP, 
                                    EXPEDIDOR_NOME = CASE WHEN cte.CON_TIPO_AMBIENTE = 1 THEN expedidor.PCT_NOME WHEN cte.CON_TIPO_AMBIENTE = 2 AND expedidor.PCT_CPF_CNPJ is not null THEN 'CT-e EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO' END, 
                                    expedidor.PCT_EXTERIOR EXPEDIDOR_EXTERIOR, 
                                    EXPEDIDOR_CIDADE = CASE expedidor.PCT_EXTERIOR WHEN 0 THEN localidadeExpedidor.LOC_DESCRICAO WHEN 1 THEN expedidor.PCT_CIDADE ELSE NULL END,
                                    EXPEDIDOR_SIGLA_ESTADO = CASE expedidor.PCT_EXTERIOR WHEN 0 THEN ufExpedidor.UF_SIGLA WHEN 1 THEN 'EX' ELSE NULL END, 
                                    EXPEDIDOR_PAIS = CASE expedidor.PCT_EXTERIOR WHEN 0 THEN paisExpedidor.PAI_NOME WHEN 1 THEN paisExteriorExpedidor.PAI_NOME ELSE NULL END, 
                                    DESTINATARIO_CPF_CNPJ = CASE destinatario.PCT_EXTERIOR WHEN 0 THEN destinatario.PCT_CPF_CNPJ WHEN 1 THEN '00.000.000/0000-00' ELSE NULL END, 
                                    DESTINATARIO_IE_RG = CASE destinatario.PCT_EXTERIOR WHEN 0 THEN destinatario.PCT_IERG WHEN 1 THEN 'ISENTO' ELSE NULL END, 
                                    destinatario.PCT_ENDERECO DESTINATARIO_ENDERECO, destinatario.PCT_BAIRRO DESTINATARIO_BAIRRO, destinatario.PCT_NUMERO DESTINATARIO_NUMERO, destinatario.PCT_FONE DESTINATARIO_TELEFONE, destinatario.PCT_CEP DESTINATARIO_CEP,
                                    DESTINATARIO_NOME = CASE WHEN cte.CON_TIPO_AMBIENTE = 1 THEN destinatario.PCT_NOME WHEN cte.CON_TIPO_AMBIENTE = 2 AND destinatario.PCT_CPF_CNPJ is not null THEN 'CT-e EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO' END,
                                    destinatario.PCT_EXTERIOR DESTINATARIO_EXTERIOR, 
                                    DESTINATARIO_CIDADE = CASE destinatario.PCT_EXTERIOR WHEN 0 THEN localidadeDestinatario.LOC_DESCRICAO WHEN 1 THEN destinatario.PCT_CIDADE ELSE NULL END, 
                                    DESTINATARIO_SIGLA_ESTADO = CASE destinatario.PCT_EXTERIOR WHEN 0 THEN ufDestinatario.UF_SIGLA WHEN 1 THEN 'EX' ELSE NULL END, 
                                    DESTINATARIO_PAIS = CASE destinatario.PCT_EXTERIOR WHEN 0 THEN paisDestinatario.PAI_NOME WHEN 1 THEN paisExteriorDestinatario.PAI_NOME ELSE NULL END, 
                                    TOMADOR_CPF_CNPJ = CASE tomador.PCT_EXTERIOR WHEN 0 THEN tomador.PCT_CPF_CNPJ WHEN 1 THEN '00.000.000/0000-00' ELSE NULL END, 
                                    TOMADOR_IE_RG = CASE tomador.PCT_EXTERIOR WHEN 0 THEN tomador.PCT_IERG WHEN 1 THEN 'ISENTO' ELSE NULL END, 
                                    tomador.PCT_ENDERECO TOMADOR_ENDERECO, tomador.PCT_BAIRRO TOMADOR_BAIRRO, tomador.PCT_NUMERO TOMADOR_NUMERO, tomador.PCT_FONE TOMADOR_TELEFONE, tomador.PCT_CEP TOMADOR_CEP, 
                                    TOMADOR_NOME = CASE WHEN cte.CON_TIPO_AMBIENTE = 1 THEN tomador.PCT_NOME WHEN cte.CON_TIPO_AMBIENTE = 2 AND tomador.PCT_CPF_CNPJ is not null THEN 'CT-e EMITIDO EM AMBIENTE DE HOMOLOGAÇÃO' END,
                                    tomador.PCT_EXTERIOR TOMADOR_EXTERIOR,  
                                    TOMADOR_CIDADE = CASE tomador.PCT_EXTERIOR WHEN 0 THEN localidadeTomador.LOC_DESCRICAO WHEN 1 THEN tomador.PCT_CIDADE ELSE NULL END, 
                                    TOMADOR_SIGLA_ESTADO = CASE tomador.PCT_EXTERIOR WHEN 0 THEN ufTomador.UF_SIGLA WHEN 1 THEN 'EX' ELSE NULL END, 
                                    TOMADOR_PAIS = CASE tomador.PCT_EXTERIOR WHEN 0 THEN paisTomador.PAI_NOME WHEN 1 THEN paisExteriorTomador.PAI_NOME ELSE NULL END, 
                                    MOTORISTA_CPF = (SELECT TOP(1) CMO_CPF_MOTORISTA FROM T_CTE_MOTORISTA WHERE CON_CODIGO = cte.CON_CODIGO), 
                                    MOTORISTA_NOME = (SELECT TOP(1) CMO_NOME_MOTORISTA FROM T_CTE_MOTORISTA WHERE CON_CODIGO = cte.CON_CODIGO), 
                                    SEGURO_SEGURADORA = (SELECT TOP(1) SEG_NOMESEGURADORA FROM T_CTE_SEGURO WHERE CON_CODIGO = cte.CON_CODIGO), 
                                    SEGURO_APOLICE = (SELECT TOP(1) SEG_NUMAPOLICE FROM T_CTE_SEGURO WHERE CON_CODIGO = cte.CON_CODIGO), 
                                    SEGURO_AVERBACAO = (SELECT TOP(1) SEG_NUMAVERBACAO FROM T_CTE_SEGURO WHERE CON_CODIGO = cte.CON_CODIGO), 
                                    SEGURO_RESPONSAVEL = (SELECT TOP(1) SEG_TIPO FROM T_CTE_SEGURO WHERE CON_CODIGO = cte.CON_CODIGO),
                                    ibpt.IBP_PERCENTUAL_MUNICIPAL, ibpt.IBP_PERCENTUAL_ESTADUAL, ibpt.IBP_PERCENTUAL_FEDERAL_INTERNACIONAL, ibpt.IBP_PERCENTUAL_FEDERAL_NACIONAL, cte.CON_DATAPREVISTAENTREGA, cte.CON_QRCODE, cte.CON_IND_GLOBALIZADO
                                    FROM T_CTE cte 
                                    inner join T_EMPRESA empresa on empresa.EMP_CODIGO = cte.EMP_CODIGO 
                                    inner join T_LOCALIDADES localidadeEmpresa on localidadeEmpresa.LOC_CODIGO = empresa.LOC_CODIGO 
                                    left outer join T_LOCALIDADES localidadeInicioPrestacao on cte.CON_LOCINICIOPRESTACAO = localidadeInicioPrestacao.LOC_CODIGO 
                                    left outer join T_LOCALIDADES localidadeTerminoPrestacao on cte.CON_LOCTERMINOPRESTACAO = localidadeTerminoPrestacao.LOC_CODIGO 
                                    left outer join T_MODDOCFISCAL modelo on modelo.MOD_CODIGO = cte.CON_MODELODOC 
                                    left outer join T_CFOP cfop on cfop.CFO_CODIGO = cte.CFO_CODIGO 
                                    left outer join T_NATUREZAOPERACAO natureza on natureza.NAT_CODIGO = cte.NAT_CODIGO 
                                    left outer join T_EMPRESA_SERIE serie on serie.ESE_CODIGO = cte.CON_SERIE 
                                    left outer join T_CTE_PARTICIPANTE remetente on remetente.PCT_CODIGO = cte.CON_REMETENTE_CTE
                                    left outer join T_LOCALIDADES localidadeRemetente on localidadeRemetente.LOC_CODIGO = remetente.LOC_CODIGO 
                                    left outer join T_UF ufRemetente on localidadeRemetente.UF_SIGLA = ufRemetente.UF_SIGLA 
                                    left outer join T_PAIS paisRemetente on ufRemetente.PAI_CODIGO = paisRemetente.PAI_CODIGO 
                                    left outer join T_PAIS paisExteriorRemetente on remetente.PAI_CODIGO = paisExteriorRemetente.PAI_CODIGO
                                    left outer join T_CTE_PARTICIPANTE recebedor on recebedor.PCT_CODIGO = cte.CON_RECEBEDOR_CTE 
                                    left outer join T_LOCALIDADES localidadeRecebedor on localidadeRecebedor.LOC_CODIGO = recebedor.LOC_CODIGO 
                                    left outer join T_UF ufRecebedor on localidadeRecebedor.UF_SIGLA = ufRecebedor.UF_SIGLA 
                                    left outer join T_PAIS paisRecebedor on ufRecebedor.PAI_CODIGO = paisRecebedor.PAI_CODIGO 
                                    left outer join T_PAIS paisExteriorRecebedor on recebedor.PAI_CODIGO = paisExteriorRecebedor.PAI_CODIGO 
                                    left outer join T_CTE_PARTICIPANTE expedidor on expedidor.PCT_CODIGO = cte.CON_EXPEDIDOR_CTE 
                                    left outer join T_LOCALIDADES localidadeExpedidor on localidadeExpedidor.LOC_CODIGO = expedidor.LOC_CODIGO
                                    left outer join T_UF ufExpedidor on localidadeExpedidor.UF_SIGLA = ufExpedidor.UF_SIGLA
                                    left outer join T_PAIS paisExpedidor on ufExpedidor.PAI_CODIGO = paisExpedidor.PAI_CODIGO 
                                    left outer join T_PAIS paisExteriorExpedidor on expedidor.PAI_CODIGO = paisExteriorExpedidor.PAI_CODIGO 
                                    left outer join T_CTE_PARTICIPANTE destinatario on destinatario.PCT_CODIGO = cte.CON_DESTINATARIO_CTE 
                                    left outer join T_LOCALIDADES localidadeDestinatario on localidadeDestinatario.LOC_CODIGO = destinatario.LOC_CODIGO
                                    left outer join T_UF ufDestinatario on localidadeDestinatario.UF_SIGLA = ufDestinatario.UF_SIGLA 
                                    left outer join T_PAIS paisDestinatario on ufDestinatario.PAI_CODIGO = paisDestinatario.PAI_CODIGO 
                                    left outer join T_PAIS paisExteriorDestinatario on destinatario.PAI_CODIGO = paisExteriorDestinatario.PAI_CODIGO 
                                    left outer join T_CTE_PARTICIPANTE tomador on tomador.PCT_CODIGO = cte.CON_TOMADOR_CTE
                                    left outer join T_LOCALIDADES localidadeTomador on localidadeTomador.LOC_CODIGO = tomador.LOC_CODIGO 
                                    left outer join T_UF ufTomador on localidadeTomador.UF_SIGLA = ufTomador.UF_SIGLA 
                                    left outer join T_PAIS paisTomador on ufTomador.PAI_CODIGO = paisTomador.PAI_CODIGO 
                                    left outer join T_PAIS paisExteriorTomador on tomador.PAI_CODIGO = paisExteriorTomador.PAI_CODIGO 
                                    left outer join T_IMPOSTO_IBPT ibpt on localidadeEmpresa.UF_SIGLA = ibpt.UF_SIGLA 
                                    WHERE CON_CODIGO = " + codigoCTe.ToString();

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows && reader.Read())
            {
                dacte = new Dominio.ObjetosDeValor.Relatorios.DACTE();
                dacte.ChaveCTeSubcontratacaoComplementar = reader["CON_CHAVE_CTE_SUB_COMP"] != DBNull.Value ? (string)reader["CON_CHAVE_CTE_SUB_COMP"] : string.Empty;
                dacte.AliquotaICMS = reader["CON_ALIQ_ICMS"] != DBNull.Value ? (decimal)reader["CON_ALIQ_ICMS"] : 0m;
                dacte.Ambiente = reader["CON_TIPO_AMBIENTE"] != DBNull.Value ? (Dominio.Enumeradores.TipoAmbiente)reader["CON_TIPO_AMBIENTE"] : Dominio.Enumeradores.TipoAmbiente.Homologacao;
                dacte.BairroEmitente = reader["EMPRESA_BAIRRO"] != DBNull.Value ? (string)reader["EMPRESA_BAIRRO"] : string.Empty;
                dacte.BaseCalculoICMS = reader["CON_BC_ICMS"] != DBNull.Value ? (decimal)reader["CON_BC_ICMS"] : 0m;
                dacte.CEPDestinatario = reader["DESTINATARIO_CEP"] != DBNull.Value ? (string)reader["DESTINATARIO_CEP"] : string.Empty;
                dacte.CEPEmitente = reader["EMPRESA_CEP"] != DBNull.Value ? (string)reader["EMPRESA_CEP"] : string.Empty;
                dacte.CEPExpedidor = reader["EXPEDIDOR_CEP"] != DBNull.Value ? (string)reader["EXPEDIDOR_CEP"] : string.Empty;
                dacte.CEPRecebedor = reader["RECEBEDOR_CEP"] != DBNull.Value ? (string)reader["RECEBEDOR_CEP"] : string.Empty;
                dacte.CEPRemetente = reader["REMETENTE_CEP"] != DBNull.Value ? (string)reader["REMETENTE_CEP"] : string.Empty;
                dacte.CFOP = reader["NUMERO_CFOP"] != DBNull.Value ? ((int)reader["NUMERO_CFOP"]).ToString() : string.Empty;
                dacte.Chave = reader["CON_CHAVECTE"] != DBNull.Value ? (string)reader["CON_CHAVECTE"] : string.Empty;
                dacte.CidadeDestinatario = reader["DESTINATARIO_CIDADE"] != DBNull.Value ? (string)reader["DESTINATARIO_CIDADE"] : string.Empty;
                dacte.CidadeDestinoPrestacao = reader["LOCALIDADE_TERMINO_PRESTACAO"] != DBNull.Value ? (string)reader["LOCALIDADE_TERMINO_PRESTACAO"] : string.Empty;
                dacte.CidadeEmitente = reader["EMPRESA_CIDADE"] != DBNull.Value ? (string)reader["EMPRESA_CIDADE"] : string.Empty;
                dacte.CidadeExpedidor = reader["EXPEDIDOR_CIDADE"] != DBNull.Value ? (string)reader["EXPEDIDOR_CIDADE"] : string.Empty;
                dacte.CidadeOrigemPrestacao = reader["LOCALIDADE_INICIO_PRESTACAO"] != DBNull.Value ? (string)reader["LOCALIDADE_INICIO_PRESTACAO"] : string.Empty;
                dacte.CidadeRecebedor = reader["RECEBEDOR_CIDADE"] != DBNull.Value ? (string)reader["RECEBEDOR_CIDADE"] : string.Empty;
                dacte.CidadeRemetente = reader["REMETENTE_CIDADE"] != DBNull.Value ? (string)reader["REMETENTE_CIDADE"] : string.Empty;
                dacte.CNPJEmitente = reader["EMPRESA_CNPJ"] != DBNull.Value ? (string)reader["EMPRESA_CNPJ"] : string.Empty;
                dacte.Codigo = reader["CON_CODIGO"] != DBNull.Value ? (int)reader["CON_CODIGO"] : 0;
                dacte.CodigoDeBarras = !string.IsNullOrWhiteSpace(dacte.Chave) ? Utilidades.Barcode.Gerar(dacte.Chave, ZXing.BarcodeFormat.CODE_128, new BarcodeMetrics1d(1, 30), System.Drawing.Imaging.ImageFormat.Bmp) : null;
                dacte.CPFCNPJDestinatario = reader["DESTINATARIO_CPF_CNPJ"] != DBNull.Value ? (string)reader["DESTINATARIO_CPF_CNPJ"] : string.Empty;
                dacte.CPFCNPJExpedidor = reader["EXPEDIDOR_CPF_CNPJ"] != DBNull.Value ? (string)reader["EXPEDIDOR_CPF_CNPJ"] : string.Empty;
                dacte.CPFCNPJRecebedor = reader["RECEBEDOR_CPF_CNPJ"] != DBNull.Value ? (string)reader["RECEBEDOR_CPF_CNPJ"] : string.Empty;
                dacte.CPFCNPJRemetente = reader["REMETENTE_CPF_CNPJ"] != DBNull.Value ? (string)reader["REMETENTE_CPF_CNPJ"] : string.Empty;
                dacte.CPFMotorista = reader["MOTORISTA_CPF"] != DBNull.Value ? (string)reader["MOTORISTA_CPF"] : string.Empty;
                dacte.CSTICMS = reader["CON_CST"] != DBNull.Value ? (string)reader["CON_CST"] : string.Empty;
                dacte.DataAutorizacao = reader["CON_RETORNOCTEDATA"] != DBNull.Value ? (DateTime?)reader["CON_RETORNOCTEDATA"] : null;
                dacte.DataEmissao = reader["CON_DATAHORAEMISSAO"] != DBNull.Value ? (DateTime?)reader["CON_DATAHORAEMISSAO"] : null;
                dacte.IEEmitente = reader["EMPRESA_IE"] != DBNull.Value ? (string)reader["EMPRESA_IE"] : string.Empty;
                dacte.IEDestinatario = reader["DESTINATARIO_IE_RG"] != DBNull.Value ? (string)reader["DESTINATARIO_IE_RG"] : string.Empty;
                dacte.IEExpedidor = reader["EXPEDIDOR_IE_RG"] != DBNull.Value ? (string)reader["EXPEDIDOR_IE_RG"] : string.Empty;
                dacte.IERecebedor = reader["RECEBEDOR_IE_RG"] != DBNull.Value ? (string)reader["RECEBEDOR_IE_RG"] : string.Empty;
                dacte.IERemetente = reader["REMETENTE_IE_RG"] != DBNull.Value ? (string)reader["REMETENTE_IE_RG"] : string.Empty;
                dacte.Logo = Utilidades.Image.GetFromPath(reader["EMPRESA_CAMINHO_LOGO_DACTE"] != DBNull.Value ? (string)reader["EMPRESA_CAMINHO_LOGO_DACTE"] : string.Empty, System.Drawing.Imaging.ImageFormat.Bmp);
                dacte.LogradouroDestinatario = reader["DESTINATARIO_ENDERECO"] != DBNull.Value ? (string)reader["DESTINATARIO_ENDERECO"] : string.Empty;
                dacte.LogradouroEmitente = reader["EMPRESA_ENDERECO"] != DBNull.Value ? (string)reader["EMPRESA_ENDERECO"] : string.Empty;
                dacte.LogradouroExpedidor = reader["EXPEDIDOR_ENDERECO"] != DBNull.Value ? (string)reader["EXPEDIDOR_ENDERECO"] : string.Empty;
                dacte.LogradouroRecebedor = reader["RECEBEDOR_ENDERECO"] != DBNull.Value ? (string)reader["RECEBEDOR_ENDERECO"] : string.Empty;
                dacte.LogradouroRemetente = reader["REMETENTE_ENDERECO"] != DBNull.Value ? (string)reader["REMETENTE_ENDERECO"] : string.Empty;
                dacte.Lotacao = reader["CON_LOTACAO"] != DBNull.Value ? ((Dominio.Enumeradores.OpcaoSimNao)reader["CON_LOTACAO"]) == Dominio.Enumeradores.OpcaoSimNao.Sim ? "SIM" : "NÃO" : "NÃO";
                dacte.Modelo = reader["NUMERO_MODELO"] != DBNull.Value ? (string)reader["NUMERO_MODELO"] : string.Empty;
                dacte.DescricaoModeloFiscal = reader["MODELO_DESCRICAO"] != DBNull.Value ? (string)reader["MODELO_DESCRICAO"] : string.Empty;
                dacte.AbreviacaoModeloFiscal = reader["MODELO_ABREVIACAO"] != DBNull.Value ? (string)reader["MODELO_ABREVIACAO"] : string.Empty;
                dacte.NaturezaOperacao = reader["NATUREZA_OPERACAO"] != DBNull.Value ? (string)reader["NATUREZA_OPERACAO"] : string.Empty;
                dacte.NomeDestinatario = reader["DESTINATARIO_NOME"] != DBNull.Value ? (string)reader["DESTINATARIO_NOME"] : string.Empty;
                dacte.NomeEmitente = reader["EMPRESA_NOME"] != DBNull.Value ? (string)reader["EMPRESA_NOME"] : string.Empty;
                dacte.NomeExpedidor = reader["EXPEDIDOR_NOME"] != DBNull.Value ? (string)reader["EXPEDIDOR_NOME"] : string.Empty;
                dacte.NomeMotorista = reader["MOTORISTA_NOME"] != DBNull.Value ? (string)reader["MOTORISTA_NOME"] : string.Empty;
                dacte.NomeRecebedor = reader["RECEBEDOR_NOME"] != DBNull.Value ? (string)reader["RECEBEDOR_NOME"] : string.Empty;
                dacte.NomeRemetente = reader["REMETENTE_NOME"] != DBNull.Value ? (string)reader["REMETENTE_NOME"] : string.Empty;
                dacte.NomeSeguradora = reader["SEGURO_SEGURADORA"] != DBNull.Value ? (string)reader["SEGURO_SEGURADORA"] : string.Empty;
                dacte.Numero = reader["CON_NUM"] != DBNull.Value ? (int)reader["CON_NUM"] : 0;
                dacte.NumeroApoliceSeguro = reader["SEGURO_APOLICE"] != DBNull.Value ? (string)reader["SEGURO_APOLICE"] : string.Empty;
                dacte.NumeroAverbacaoSeguro = reader["SEGURO_AVERBACAO"] != DBNull.Value ? (string)reader["SEGURO_AVERBACAO"] : string.Empty;
                dacte.NumeroDestinatario = reader["DESTINATARIO_NUMERO"] != DBNull.Value ? (string)reader["DESTINATARIO_NUMERO"] + " - " + (reader["DESTINATARIO_BAIRRO"] != DBNull.Value ? (string)reader["DESTINATARIO_BAIRRO"] : string.Empty) : string.Empty;
                dacte.NumeroEmitente = reader["EMPRESA_NUMERO"] != DBNull.Value ? (string)reader["EMPRESA_NUMERO"] : string.Empty;
                dacte.NumeroExpedidor = reader["EXPEDIDOR_NUMERO"] != DBNull.Value ? (string)reader["EXPEDIDOR_NUMERO"] + " - " + (reader["EXPEDIDOR_BAIRRO"] != DBNull.Value ? (string)reader["EXPEDIDOR_BAIRRO"] : string.Empty) : string.Empty;
                dacte.NumeroRecebedor = reader["RECEBEDOR_NUMERO"] != DBNull.Value ? (string)reader["RECEBEDOR_NUMERO"] + " - " + (reader["RECEBEDOR_BAIRRO"] != DBNull.Value ? (string)reader["RECEBEDOR_BAIRRO"] : string.Empty) : string.Empty;
                dacte.NumeroRemetente = reader["REMETENTE_NUMERO"] != DBNull.Value ? (string)reader["REMETENTE_NUMERO"] + " - " + (reader["REMETENTE_BAIRRO"] != DBNull.Value ? (string)reader["REMETENTE_BAIRRO"] : string.Empty) : string.Empty;
                dacte.Observacoes = reader["CON_OBSGERAIS"] != DBNull.Value ? (string)reader["CON_OBSGERAIS"] : string.Empty;
                dacte.Observacoes += " " + (reader["CON_OBS_AVANCADAS"] != DBNull.Value ? (string)reader["CON_OBS_AVANCADAS"] : string.Empty);
                dacte.OutrasCaracteristicasCarga = reader["CON_OUTRAS_CARAC_CARGA"] != DBNull.Value ? (string)reader["CON_OUTRAS_CARAC_CARGA"] : string.Empty;
                dacte.PaisDestinatario = reader["DESTINATARIO_PAIS"] != DBNull.Value ? (string)reader["DESTINATARIO_PAIS"] : string.Empty;
                dacte.PaisExpedidor = reader["EXPEDIDOR_PAIS"] != DBNull.Value ? (string)reader["EXPEDIDOR_PAIS"] : string.Empty;
                dacte.PaisRecebedor = reader["RECEBEDOR_PAIS"] != DBNull.Value ? (string)reader["RECEBEDOR_PAIS"] : string.Empty;
                dacte.PaisRemetente = reader["REMETENTE_PAIS"] != DBNull.Value ? (string)reader["REMETENTE_PAIS"] : string.Empty;
                dacte.PercentualReducaoBaseCalculoICMS = reader["CON_PER_RED_BC_ICMS"] != DBNull.Value ? (decimal)reader["CON_PER_RED_BC_ICMS"] : 0m;
                dacte.ProdutoPredominante = reader["CON_PRODUTO_PRED"] != DBNull.Value ? (string)reader["CON_PRODUTO_PRED"] : string.Empty;
                dacte.ProtocoloAutorizacao = reader["CON_PROTOCOLO"] != DBNull.Value ? (string)reader["CON_PROTOCOLO"] : string.Empty;
                dacte.ResponsavelSeguro = reader["SEGURO_RESPONSAVEL"] != DBNull.Value ? (Dominio.Enumeradores.TipoSeguro)reader["SEGURO_RESPONSAVEL"] : Dominio.Enumeradores.TipoSeguro.Remetente;
                dacte.RNTRCEmitente = reader["EMPRESA_RNTRC"] != DBNull.Value ? (string)reader["EMPRESA_RNTRC"] : string.Empty;
                dacte.Serie = reader["NUMERO_SERIE"] != DBNull.Value ? (int)reader["NUMERO_SERIE"] : 0;
                dacte.Status = reader["CON_STATUS"] != DBNull.Value ? (string)reader["CON_STATUS"] : string.Empty;
                dacte.SuprimirImpostos = reader["CON_EXIBE_ICMS_DACTE"] != DBNull.Value ? (bool)reader["CON_EXIBE_ICMS_DACTE"] : false;
                dacte.TelefoneDestinatario = reader["DESTINATARIO_TELEFONE"] != DBNull.Value ? (string)reader["DESTINATARIO_TELEFONE"] : string.Empty;
                dacte.TelefoneEmitente = reader["EMPRESA_TELEFONE"] != DBNull.Value ? (string)reader["EMPRESA_TELEFONE"] : string.Empty;
                dacte.TelefoneExpedidor = reader["EXPEDIDOR_TELEFONE"] != DBNull.Value ? (string)reader["EXPEDIDOR_TELEFONE"] : string.Empty;
                dacte.TelefoneRecebedor = reader["RECEBEDOR_TELEFONE"] != DBNull.Value ? (string)reader["RECEBEDOR_TELEFONE"] : string.Empty;
                dacte.TelefoneRemetente = reader["REMETENTE_TELEFONE"] != DBNull.Value ? (string)reader["REMETENTE_TELEFONE"] : string.Empty;
                dacte.TipoCTe = reader["CON_TIPO_CTE"] != DBNull.Value ? (Dominio.Enumeradores.TipoCTE)reader["CON_TIPO_CTE"] : Dominio.Enumeradores.TipoCTE.Normal;
                dacte.TipoImpressao = reader["CON_IMPRESSAO"] != DBNull.Value ? (Dominio.Enumeradores.TipoImpressao)reader["CON_IMPRESSAO"] : Dominio.Enumeradores.TipoImpressao.Retrato;
                dacte.TipoPagamento = reader["CON_PAGOAPAGAR"] != DBNull.Value ? (Dominio.Enumeradores.TipoPagamento)reader["CON_PAGOAPAGAR"] : Dominio.Enumeradores.TipoPagamento.Pago;
                dacte.TipoServico = reader["CON_TIPO_SERVICO"] != DBNull.Value ? (Dominio.Enumeradores.TipoServico)reader["CON_TIPO_SERVICO"] : Dominio.Enumeradores.TipoServico.Normal;
                dacte.TipoTomador = reader["CON_TOMADOR"] != DBNull.Value ? (Dominio.Enumeradores.TipoTomador)reader["CON_TOMADOR"] : Dominio.Enumeradores.TipoTomador.Remetente;
                dacte.TotalDocumentos = 0;
                dacte.UFDestinatario = reader["DESTINATARIO_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["DESTINATARIO_SIGLA_ESTADO"] : string.Empty;
                dacte.UFDestinoPrestacao = reader["UF_TERMINO_PRESTACAO"] != DBNull.Value ? (string)reader["UF_TERMINO_PRESTACAO"] : string.Empty;
                dacte.UFEmitente = reader["EMPRESA_UF"] != DBNull.Value ? (string)reader["EMPRESA_UF"] : string.Empty;
                dacte.UFExpedidor = reader["EXPEDIDOR_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["EXPEDIDOR_SIGLA_ESTADO"] : string.Empty;
                dacte.UFOrigemPrestacao = reader["UF_INICIO_PRESTACAO"] != DBNull.Value ? (string)reader["UF_INICIO_PRESTACAO"] : string.Empty;
                dacte.UFRecebedor = reader["RECEBEDOR_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["RECEBEDOR_SIGLA_ESTADO"] : string.Empty;
                dacte.UFRemetente = reader["REMETENTE_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["REMETENTE_SIGLA_ESTADO"] : string.Empty;
                dacte.ValorICMS = reader["CON_VAL_ICMS"] != DBNull.Value ? (decimal)reader["CON_VAL_ICMS"] : 0m;
                dacte.ValorTotalMercadoria = reader["CON_VALOR_TOTAL_MERC"] != DBNull.Value ? (decimal)reader["CON_VALOR_TOTAL_MERC"] : 0m;
                dacte.ValorTotalReceber = reader["CON_VALOR_RECEBER"] != DBNull.Value ? (decimal)reader["CON_VALOR_RECEBER"] : 0m;
                dacte.ValorTotalServico = reader["CON_VALOR_PREST_SERVICO"] != DBNull.Value ? (decimal)reader["CON_VALOR_PREST_SERVICO"] : 0m;
                dacte.PercentualTributosEstadual = reader["IBP_PERCENTUAL_ESTADUAL"] != DBNull.Value ? (decimal)reader["IBP_PERCENTUAL_ESTADUAL"] : 0m;
                dacte.PercentualTributosInternacional = reader["IBP_PERCENTUAL_FEDERAL_INTERNACIONAL"] != DBNull.Value ? (decimal)reader["IBP_PERCENTUAL_FEDERAL_INTERNACIONAL"] : 0m;
                dacte.PercentualTributosMunicipal = reader["IBP_PERCENTUAL_MUNICIPAL"] != DBNull.Value ? (decimal)reader["IBP_PERCENTUAL_MUNICIPAL"] : 0m;
                dacte.PercentualTributosNacional = reader["IBP_PERCENTUAL_FEDERAL_NACIONAL"] != DBNull.Value ? (decimal)reader["IBP_PERCENTUAL_FEDERAL_NACIONAL"] : 0m;
                dacte.ValorICMSUFDestino = reader["CON_VALOR_ICMS_UF_DESTINO"] != DBNull.Value ? (decimal)reader["CON_VALOR_ICMS_UF_DESTINO"] : 0m;

                if (reader["TOMADOR_CPF_CNPJ"] != DBNull.Value && (string)reader["TOMADOR_CPF_CNPJ"] != "")
                {
                    dacte.CPFCNPJTomador = reader["TOMADOR_CPF_CNPJ"] != DBNull.Value ? (string)reader["TOMADOR_CPF_CNPJ"] : string.Empty;
                    dacte.IETomador = reader["TOMADOR_IE_RG"] != DBNull.Value ? (string)reader["TOMADOR_IE_RG"] : string.Empty;
                    dacte.NomeTomador = reader["TOMADOR_NOME"] != DBNull.Value ? (string)reader["TOMADOR_NOME"] : string.Empty;
                    dacte.CEPTomador = reader["TOMADOR_CEP"] != DBNull.Value ? (string)reader["TOMADOR_CEP"] : string.Empty;
                    dacte.CidadeTomador = reader["TOMADOR_CIDADE"] != DBNull.Value ? (string)reader["TOMADOR_CIDADE"] : string.Empty;
                    dacte.UFTomador = reader["TOMADOR_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["TOMADOR_SIGLA_ESTADO"] : string.Empty;
                    dacte.PaisTomador = reader["TOMADOR_PAIS"] != DBNull.Value ? (string)reader["TOMADOR_PAIS"] : string.Empty;
                    dacte.LogradouroTomador = reader["TOMADOR_ENDERECO"] != DBNull.Value ? (string)reader["TOMADOR_ENDERECO"] : string.Empty;
                    dacte.NumeroTomador = reader["TOMADOR_NUMERO"] != DBNull.Value ? (string)reader["TOMADOR_NUMERO"] + " - " + (reader["TOMADOR_BAIRRO"] != DBNull.Value ? (string)reader["TOMADOR_BAIRRO"] : string.Empty) : string.Empty;
                    dacte.TelefoneTomador = reader["TOMADOR_TELEFONE"] != DBNull.Value ? (string)reader["TOMADOR_TELEFONE"] : string.Empty;
                }
                else if ((Dominio.Enumeradores.TipoTomador)reader["CON_TOMADOR"] == Dominio.Enumeradores.TipoTomador.Remetente && reader["REMETENTE_CPF_CNPJ"] != DBNull.Value && (string)reader["REMETENTE_CPF_CNPJ"] != "")
                {
                    dacte.CPFCNPJTomador = reader["REMETENTE_CPF_CNPJ"] != DBNull.Value ? (string)reader["REMETENTE_CPF_CNPJ"] : string.Empty;
                    dacte.IETomador = reader["REMETENTE_IE_RG"] != DBNull.Value ? (string)reader["REMETENTE_IE_RG"] : string.Empty;
                    dacte.NomeTomador = reader["REMETENTE_NOME"] != DBNull.Value ? (string)reader["REMETENTE_NOME"] : string.Empty;
                    dacte.CEPTomador = reader["REMETENTE_CEP"] != DBNull.Value ? (string)reader["REMETENTE_CEP"] : string.Empty;
                    dacte.CidadeTomador = reader["REMETENTE_CIDADE"] != DBNull.Value ? (string)reader["REMETENTE_CIDADE"] : string.Empty;
                    dacte.UFTomador = reader["REMETENTE_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["REMETENTE_SIGLA_ESTADO"] : string.Empty;
                    dacte.PaisTomador = reader["REMETENTE_PAIS"] != DBNull.Value ? (string)reader["REMETENTE_PAIS"] : string.Empty;
                    dacte.LogradouroTomador = reader["REMETENTE_ENDERECO"] != DBNull.Value ? (string)reader["REMETENTE_ENDERECO"] : string.Empty;
                    dacte.NumeroTomador = reader["REMETENTE_NUMERO"] != DBNull.Value ? (string)reader["REMETENTE_NUMERO"] + " - " + (reader["REMETENTE_BAIRRO"] != DBNull.Value ? (string)reader["REMETENTE_BAIRRO"] : string.Empty) : string.Empty;
                    dacte.TelefoneTomador = reader["REMETENTE_TELEFONE"] != DBNull.Value ? (string)reader["REMETENTE_TELEFONE"] : string.Empty;
                }
                else if ((Dominio.Enumeradores.TipoTomador)reader["CON_TOMADOR"] == Dominio.Enumeradores.TipoTomador.Destinatario && reader["DESTINATARIO_CPF_CNPJ"] != DBNull.Value && (string)reader["DESTINATARIO_CPF_CNPJ"] != "")
                {
                    dacte.CPFCNPJTomador = reader["DESTINATARIO_CPF_CNPJ"] != DBNull.Value ? (string)reader["DESTINATARIO_CPF_CNPJ"] : string.Empty;
                    dacte.IETomador = reader["DESTINATARIO_IE_RG"] != DBNull.Value ? (string)reader["DESTINATARIO_IE_RG"] : string.Empty;
                    dacte.NomeTomador = reader["DESTINATARIO_NOME"] != DBNull.Value ? (string)reader["DESTINATARIO_NOME"] : string.Empty;
                    dacte.CEPTomador = reader["DESTINATARIO_CEP"] != DBNull.Value ? (string)reader["DESTINATARIO_CEP"] : string.Empty;
                    dacte.CidadeTomador = reader["DESTINATARIO_CIDADE"] != DBNull.Value ? (string)reader["DESTINATARIO_CIDADE"] : string.Empty;
                    dacte.UFTomador = reader["DESTINATARIO_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["DESTINATARIO_SIGLA_ESTADO"] : string.Empty;
                    dacte.PaisTomador = reader["DESTINATARIO_PAIS"] != DBNull.Value ? (string)reader["DESTINATARIO_PAIS"] : string.Empty;
                    dacte.LogradouroTomador = reader["DESTINATARIO_ENDERECO"] != DBNull.Value ? (string)reader["DESTINATARIO_ENDERECO"] : string.Empty;
                    dacte.NumeroTomador = reader["DESTINATARIO_NUMERO"] != DBNull.Value ? (string)reader["DESTINATARIO_NUMERO"] + " - " + (reader["DESTINATARIO_BAIRRO"] != DBNull.Value ? (string)reader["DESTINATARIO_BAIRRO"] : string.Empty) : string.Empty;
                    dacte.TelefoneTomador = reader["DESTINATARIO_TELEFONE"] != DBNull.Value ? (string)reader["DESTINATARIO_TELEFONE"] : string.Empty;
                }
                else if ((Dominio.Enumeradores.TipoTomador)reader["CON_TOMADOR"] == Dominio.Enumeradores.TipoTomador.Expedidor && reader["EXPEDIDOR_CPF_CNPJ"] != DBNull.Value && (string)reader["EXPEDIDOR_CPF_CNPJ"] != "")
                {
                    dacte.CPFCNPJTomador = reader["EXPEDIDOR_CPF_CNPJ"] != DBNull.Value ? (string)reader["EXPEDIDOR_CPF_CNPJ"] : string.Empty;
                    dacte.IETomador = reader["EXPEDIDOR_IE_RG"] != DBNull.Value ? (string)reader["EXPEDIDOR_IE_RG"] : string.Empty;
                    dacte.NomeTomador = reader["EXPEDIDOR_NOME"] != DBNull.Value ? (string)reader["EXPEDIDOR_NOME"] : string.Empty;
                    dacte.CEPTomador = reader["EXPEDIDOR_CEP"] != DBNull.Value ? (string)reader["EXPEDIDOR_CEP"] : string.Empty;
                    dacte.CidadeTomador = reader["EXPEDIDOR_CIDADE"] != DBNull.Value ? (string)reader["EXPEDIDOR_CIDADE"] : string.Empty;
                    dacte.UFTomador = reader["EXPEDIDOR_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["EXPEDIDOR_SIGLA_ESTADO"] : string.Empty;
                    dacte.PaisTomador = reader["EXPEDIDOR_PAIS"] != DBNull.Value ? (string)reader["EXPEDIDOR_PAIS"] : string.Empty;
                    dacte.LogradouroTomador = reader["EXPEDIDOR_ENDERECO"] != DBNull.Value ? (string)reader["EXPEDIDOR_ENDERECO"] : string.Empty;
                    dacte.NumeroTomador = reader["EXPEDIDOR_NUMERO"] != DBNull.Value ? (string)reader["EXPEDIDOR_NUMERO"] + " - " + (reader["EXPEDIDOR_BAIRRO"] != DBNull.Value ? (string)reader["EXPEDIDOR_BAIRRO"] : string.Empty) : string.Empty;
                    dacte.TelefoneTomador = reader["EXPEDIDOR_TELEFONE"] != DBNull.Value ? (string)reader["EXPEDIDOR_TELEFONE"] : string.Empty;
                }
                else if ((Dominio.Enumeradores.TipoTomador)reader["CON_TOMADOR"] == Dominio.Enumeradores.TipoTomador.Recebedor && reader["RECEBEDOR_CPF_CNPJ"] != DBNull.Value && (string)reader["RECEBEDOR_CPF_CNPJ"] != "")
                {
                    dacte.CPFCNPJTomador = reader["RECEBEDOR_CPF_CNPJ"] != DBNull.Value ? (string)reader["RECEBEDOR_CPF_CNPJ"] : string.Empty;
                    dacte.IETomador = reader["RECEBEDOR_IE_RG"] != DBNull.Value ? (string)reader["RECEBEDOR_IE_RG"] : string.Empty;
                    dacte.NomeTomador = reader["RECEBEDOR_NOME"] != DBNull.Value ? (string)reader["RECEBEDOR_NOME"] : string.Empty;
                    dacte.CEPTomador = reader["RECEBEDOR_CEP"] != DBNull.Value ? (string)reader["RECEBEDOR_CEP"] : string.Empty;
                    dacte.CidadeTomador = reader["RECEBEDOR_CIDADE"] != DBNull.Value ? (string)reader["RECEBEDOR_CIDADE"] : string.Empty;
                    dacte.UFTomador = reader["RECEBEDOR_SIGLA_ESTADO"] != DBNull.Value ? (string)reader["RECEBEDOR_SIGLA_ESTADO"] : string.Empty;
                    dacte.PaisTomador = reader["RECEBEDOR_PAIS"] != DBNull.Value ? (string)reader["RECEBEDOR_PAIS"] : string.Empty;
                    dacte.LogradouroTomador = reader["RECEBEDOR_ENDERECO"] != DBNull.Value ? (string)reader["RECEBEDOR_ENDERECO"] : string.Empty;
                    dacte.NumeroTomador = reader["RECEBEDOR_NUMERO"] != DBNull.Value ? (string)reader["RECEBEDOR_NUMERO"] + " - " + (reader["RECEBEDOR_BAIRRO"] != DBNull.Value ? (string)reader["RECEBEDOR_BAIRRO"] : string.Empty) : string.Empty;
                    dacte.TelefoneTomador = reader["RECEBEDOR_TELEFONE"] != DBNull.Value ? (string)reader["RECEBEDOR_TELEFONE"] : string.Empty;
                }

                if (reader["CON_IND_GLOBALIZADO"] != DBNull.Value && (Dominio.Enumeradores.OpcaoSimNao)reader["CON_IND_GLOBALIZADO"] == Dominio.Enumeradores.OpcaoSimNao.Sim)
                    dacte.IndicadorCTeGlobalizado = "SIM";
                else
                    dacte.IndicadorCTeGlobalizado = "NÃO";

                string qrCode = reader["CON_QRCODE"] != DBNull.Value ? (string)reader["CON_QRCODE"] : string.Empty;

                if (!string.IsNullOrWhiteSpace(qrCode))
                    dacte.QRCode = Utilidades.QRcode.Gerar(qrCode);

                if (dacte.Ambiente == Dominio.Enumeradores.TipoAmbiente.Homologacao)
                    dacte.MarcaDagua = Utilidades.Image.DrawText("SEM VALOR FISCAL - AMBIENTE DE HOMOLOGAÇÃO", new Font("Arial", 20f, FontStyle.Bold), Color.Gray, Color.White, 45);
                else if (dacte.Status == "C")
                    dacte.MarcaDagua = Utilidades.Image.DrawText("SEM VALOR FISCAL - CT-e CANCELADO NA SEFAZ", new Font("Arial", 20f, FontStyle.Bold), Color.Gray, Color.White, 45);

                DateTime dataPrevisao = reader["CON_DATAPREVISTAENTREGA"] != DBNull.Value ? (DateTime)reader["CON_DATAPREVISTAENTREGA"] : DateTime.Today;
                dacte.FilialEntrega = dataPrevisao.ToString("dd/MM/yyyy");
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();

            if (dacte != null)
            {
                SetarComponentesDaPrestacao(codigoCTe, connection, ref dacte);
                SetarInformacoesCarga(codigoCTe, connection, ref dacte);
                SetarObservacoes(codigoCTe, connection, ref dacte);
            }

            return dacte;
        }

        public static void SetarComponentesDaPrestacao(int codigoCTe, SqlConnection connection, ref Dominio.ObjetosDeValor.Relatorios.DACTE dacte)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = "SELECT CPT_NOME, CPT_VALOR from T_CTE_COMP_PREST WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            int index = 1;

            while (reader.Read())
            {
                string nome = reader["CPT_NOME"] != DBNull.Value ? (string)reader["CPT_NOME"] : string.Empty;
                decimal valor = reader["CPT_VALOR"] != DBNull.Value ? (decimal)reader["CPT_VALOR"] : 0m;

                switch (index)
                {
                    case 1:
                        dacte.DescricaoComponentePrestacao1 = nome;
                        dacte.ValorComponentePrestacao1 = valor;
                        break;
                    case 2:
                        dacte.DescricaoComponentePrestacao2 = nome;
                        dacte.ValorComponentePrestacao2 = valor;
                        break;
                    case 3:
                        dacte.DescricaoComponentePrestacao3 = nome;
                        dacte.ValorComponentePrestacao3 = valor;
                        break;
                    case 4:
                        dacte.DescricaoComponentePrestacao4 = nome;
                        dacte.ValorComponentePrestacao4 = valor;
                        break;
                    case 5:
                        dacte.DescricaoComponentePrestacao5 = nome;
                        dacte.ValorComponentePrestacao5 = valor;
                        break;
                    case 6:
                        dacte.DescricaoComponentePrestacao6 = nome;
                        dacte.ValorComponentePrestacao6 = valor;
                        break;
                    case 7:
                        dacte.DescricaoComponentePrestacao7 = nome;
                        dacte.ValorComponentePrestacao7 = valor;
                        break;
                    case 8:
                        dacte.DescricaoComponentePrestacao8 = nome;
                        dacte.ValorComponentePrestacao8 = valor;
                        break;
                    case 9:
                        dacte.DescricaoComponentePrestacao9 = nome;
                        dacte.ValorComponentePrestacao9 = valor;
                        break;
                }

                index++;
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();
        }

        public static void SetarInformacoesCarga(int codigoCTe, SqlConnection connection, ref Dominio.ObjetosDeValor.Relatorios.DACTE dacte)
        {
            SqlCommand command = new SqlCommand();
             
            command.CommandText = "SELECT ICA_UN, ICA_QTD FROM T_CTE_INF_CARGA WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            int index = 1;

            while (reader.Read())
            {
                string descricao = reader["ICA_UN"] != DBNull.Value ? ((Dominio.Enumeradores.UnidadeMedida)int.Parse((string)reader["ICA_UN"])).ToString("G") : string.Empty;
                decimal quantidade = reader["ICA_QTD"] != DBNull.Value ? (decimal)reader["ICA_QTD"] : 0m;

                switch (index)
                {
                    case 1:
                        dacte.UnidadeMedida1 = descricao;
                        dacte.QuantidadeCarga1 = quantidade;
                        break;
                    case 2:
                        dacte.UnidadeMedida2 = descricao;
                        dacte.QuantidadeCarga2 = quantidade;
                        break;
                    case 3:
                        dacte.UnidadeMedida3 = descricao;
                        dacte.QuantidadeCarga3 = quantidade;
                        break;
                    case 4:
                        dacte.UnidadeMedida4 = descricao;
                        dacte.QuantidadeCarga4 = quantidade;
                        break;
                }

                index++;
            }

            reader.Close();
            reader.Dispose();
            command.Dispose();
        }

        public static void SetarObservacoes(int codigoCTe, SqlConnection connection, ref Dominio.ObjetosDeValor.Relatorios.DACTE dacte)
        {
            SqlCommand command = new SqlCommand();

            command.CommandText = "SELECT SUM(CPT_VALOR) VALOR_TOTAL from T_CTE_COMP_PREST WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            command.CommandType = System.Data.CommandType.Text;
            command.Connection = connection;

            SqlDataReader reader = command.ExecuteReader();

            decimal valorTotalComponentes = 0;

            if (reader.HasRows && reader.Read())
                valorTotalComponentes = reader["VALOR_TOTAL"] != DBNull.Value ? (decimal)reader["VALOR_TOTAL"] : 0m;

            string mensagens = "O valor aproximado de tributos incidentes sobre o valor deste serviço é de R$ " + (valorTotalComponentes * (dacte.PercentualTributosMunicipal / 100)).ToString("n2") + " (" + dacte.PercentualTributosMunicipal.ToString("n2") + "% Municipal), R$ "
                                                                                                                + (valorTotalComponentes * (dacte.PercentualTributosEstadual / 100)).ToString("n2") + " (" + dacte.PercentualTributosEstadual.ToString("n2") + "% Estadual) e R$ "
                                                                                                                + (valorTotalComponentes * (dacte.PercentualTributosNacional / 100)).ToString("n2") + " (" + dacte.PercentualTributosNacional.ToString("n2") + "% Federal). Fonte: IBPT.\n";

            string observacoes = "";

            reader.Close();
            reader.Dispose();
            command.Dispose();

            if (dacte.ValorICMSUFDestino > 0)
                mensagens += "O Valor do ICMS UF Fim é R$ " + dacte.ValorICMSUFDestino.ToString("n2") + ".\n";

            command = new SqlCommand();

            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT 'F' TIPO, OBF_DESCRICAO OBSERVACAO FROM T_OBS_FISCO WHERE CON_CODIGO = " + codigoCTe.ToString() + " UNION SELECT 'C' TIPO, OBC_DESCRICAO OBSERVACAO FROM T_OBS_CONTRIBUINTE WHERE CON_CODIGO = " + codigoCTe.ToString(); // SQL-INJECTION-SAFE

            reader = command.ExecuteReader();

            while (reader.Read())
            {
                if ((string)reader["TIPO"] == "F" && reader["OBSERVACAO"] != DBNull.Value)
                {
                    observacoes += (string)reader["OBSERVACAO"] + "\n";
                }
                else if (reader["OBSERVACAO"] != DBNull.Value)
                {
                    mensagens += (string)reader["OBSERVACAO"] + "\n";
                }
            }

            dacte.Mensagem = mensagens;
            dacte.Observacoes += string.IsNullOrWhiteSpace(dacte.Observacoes) ? observacoes : " - " + observacoes;

            reader.Close();
            reader.Dispose();
            command.Dispose();
        }
    }
}
