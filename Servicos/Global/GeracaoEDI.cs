using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Servicos
{
    public class GeracaoEDI
    {
        #region Construtores

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout)
        {
            this.UnitOfWork = unitOfWork;
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.IndiceLinhaArquivo = 1;
            this.Layout = layout;
            this.Ocorrencias = new List<Dominio.Entidades.OcorrenciaDeCTe>();
            this.RegistroEDI = new StringBuilder();
            this.Series = null;
            this.StatusCTe = new string[] { "A" };
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.MDFe.EDIMDFe edimdfe, Dominio.Entidades.LayoutEDI layout)
        {
            this.UnitOfWork = unitOfWork;
            this.EDIMDFe = edimdfe;
            this.Layout = layout;
            this.RegistroEDI = new StringBuilder();
            this.Series = null;
            this.StatusCTe = new string[] { "A" };
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Empresa empresa, string cpfCnpjRemetente, string cpfCnpjDestinatario, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, bool somenteCTesDasOcorrencias = false, List<int> codigosCTes = null, int codigoDuplicata = 0, string[] statusCTes = null, int[] series = null)
        {
            this.UnitOfWork = unitOfWork;
            this.Series = series;
            this.StatusCTe = statusCTes ?? new string[] { "A" };
            this.RegistroEDI = new StringBuilder();

            this.Empresa = empresa;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.CodigoVeiculo = codigoVeiculo;
            this.Layout = layout;
            this.CpfCnpjRemetente = cpfCnpjRemetente;
            this.CpfCnpjDestinatario = cpfCnpjDestinatario;
            this.SomenteCTesDasOcorrencias = somenteCTesDasOcorrencias;
            this.CodigosCTes = codigosCTes;
            this.IndiceLinhaArquivo = 1;
            this.CodigoDuplicata = codigoDuplicata;
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Empresa empresa, DateTime dataInicial, DateTime dataFinal, int codigoVeiculo, bool somenteCTesDasOcorrencias = false, List<int> codigosCTes = null, int codigoDuplicata = 0, string[] statusCTes = null, int[] series = null, string cpfCnpjRemetente = "", string cpfCnpjDestinatario = "")
        {
            this.UnitOfWork = unitOfWork;
            this.Series = series;
            this.StatusCTe = statusCTes ?? new string[] { "A" };
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.Ocorrencias = new List<Dominio.Entidades.OcorrenciaDeCTe>();
            this.RegistroEDI = new StringBuilder();

            this.Empresa = empresa;
            this.Layout = layout;
            this.DataInicial = dataInicial;
            this.DataFinal = dataFinal;
            this.CodigoVeiculo = codigoVeiculo;
            this.SomenteCTesDasOcorrencias = somenteCTesDasOcorrencias;
            this.CodigosCTes = codigosCTes;
            this.CodigoDuplicata = codigoDuplicata;
            this.CpfCnpjRemetente = cpfCnpjRemetente;
            this.CpfCnpjDestinatario = cpfCnpjDestinatario;
            this.IndiceLinhaArquivo = 1;
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Empresa empresa, string[] statusCTes, List<Dominio.Entidades.OcorrenciaDeCTe> listaOcorrencias)
        {
            this.UnitOfWork = unitOfWork;
            this.StatusCTe = statusCTes ?? new string[] { "A" };
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.Ocorrencias = listaOcorrencias;
            this.RegistroEDI = new StringBuilder();

            this.Empresa = empresa;
            this.Layout = layout;

            this.IndiceLinhaArquivo = 1;
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Empresa empresa, string[] statusCTes = null, int[] series = null)
        {
            this.UnitOfWork = unitOfWork;
            this.Series = series;
            this.StatusCTe = statusCTes ?? new string[] { "A" };
            this.CTes = new List<Dominio.Entidades.ConhecimentoDeTransporteEletronico>();
            this.Ocorrencias = new List<Dominio.Entidades.OcorrenciaDeCTe>();
            this.RegistroEDI = new StringBuilder();

            this.Empresa = empresa;
            this.Layout = layout;

            this.IndiceLinhaArquivo = 1;
        }

        public GeracaoEDI(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.LayoutEDI layout, Dominio.Entidades.Empresa empresa, List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> ctes, string[] statusCTes = null, int[] series = null)
        {
            this.UnitOfWork = unitOfWork;
            this.Series = series;
            this.StatusCTe = statusCTes ?? new string[] { "A" };
            this.Empresa = empresa;
            this.Layout = layout;
            this.CTes = ctes;

            this.RegistroEDI = new StringBuilder();

            this.IndiceLinhaArquivo = 1;
        }

        #endregion

        #region Propriedades

        private Repositorio.UnitOfWork UnitOfWork { get; set; }

        private string[] StatusCTe { get; set; }

        private int[] Series { get; set; }

        private Dominio.Entidades.Empresa Empresa { get; set; }

        private Dominio.Entidades.LayoutEDI Layout { get; set; }

        private Dominio.ObjetosDeValor.MDFe.EDIMDFe EDIMDFe { get; set; }

        private DateTime DataInicial { get; set; }

        private DateTime DataFinal { get; set; }

        private List<int> CodigosCTes { get; set; }

        private int CodigoVeiculo { get; set; }

        private int CodigoDuplicata { get; set; }

        private List<Dominio.Entidades.ConhecimentoDeTransporteEletronico> CTes { get; set; }

        private List<Dominio.Entidades.OcorrenciaDeCTe> Ocorrencias { get; set; }

        private StringBuilder RegistroEDI { get; set; }

        private string CpfCnpjRemetente { get; set; }

        private string CpfCnpjDestinatario { get; set; }

        private bool SomenteCTesDasOcorrencias { get; set; }

        private int IndiceLinhaArquivo { get; set; }

        private dynamic ObjetoDinamico { get; set; }

        #endregion

        #region Métodos Públicos

        public MemoryStream GerarArquivoRecursivo(dynamic objeto, System.Text.Encoding encoding = null)
        {
            this.ObjetoDinamico = objeto;

            string arquivo = this.GerarEDIRecursivo();

            if (this.Layout.RemoverDiacriticos)
                arquivo = Utilidades.String.RemoveDiacritics(arquivo);

            return Utilidades.String.ToStream(arquivo, encoding);
        }

        public MemoryStream GerarArquivo(dynamic objeto)
        {
            this.ObjetoDinamico = objeto;

            MemoryStream memoStream = new MemoryStream();

            string arquivo = this.GerarEDI();

            if (this.Layout.RemoverDiacriticos)
                arquivo = Utilidades.String.RemoveDiacritics(arquivo);

            memoStream.Write(System.Text.Encoding.UTF8.GetBytes(arquivo), 0, arquivo.Length);

            memoStream.Position = 0;

            return memoStream;
        }

        public MemoryStream GerarArquivo(bool mudarRegistroNFQuandoSubContratacao = false)
        {
            if (this.Ocorrencias != null && this.Ocorrencias.Count > 0)
            {
                this.CTes = (from o in Ocorrencias select o.CTe).ToList();

                MemoryStream memoStream = new MemoryStream();

                string arquivo = this.GerarEDI(mudarRegistroNFQuandoSubContratacao);

                if (this.Layout.RemoverDiacriticos)
                    arquivo = Utilidades.String.RemoveDiacritics(arquivo);

                memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo), 0, arquivo.Length);

                memoStream.Position = 0;

                return memoStream;
            }
            else
            {
                if (this.SomenteCTesDasOcorrencias)
                {
                    Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(UnitOfWork);
                    this.CTes = repOcorrencia.BuscarCTesPorRemetente(this.Empresa.Codigo, this.CpfCnpjRemetente, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);
                }
                else if (this.CTes == null || this.CTes.Count <= 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);

                    if (CodigoDuplicata > 0)
                        this.CTes = repCTe.BuscarPorDuplicata(this.Empresa.Codigo, this.CodigoDuplicata);
                    else
                        this.CTes = repCTe.BuscarPorRemetente(this.Empresa.Codigo, this.CpfCnpjRemetente, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo, this.Series, CpfCnpjDestinatario);
                }

                MemoryStream memoStream = new MemoryStream();

                if (this.CTes.Count > 0)
                {
                    string arquivo = this.GerarEDI(mudarRegistroNFQuandoSubContratacao);

                    if (this.Layout.RemoverDiacriticos)
                        arquivo = Utilidades.String.RemoveDiacritics(arquivo);

                    memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo), 0, arquivo.Length);

                    memoStream.Position = 0;
                }

                return memoStream;
            }
        }

        public MemoryStream GerarLote(bool gerarPorTomador = false)
        {
            MemoryStream fZip = new MemoryStream();

            ZipOutputStream zipOStream = new ZipOutputStream(fZip);

            zipOStream.SetLevel(9);

            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(UnitOfWork);
            Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(UnitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(UnitOfWork);

            List<string> cpfCnpjClientes = new List<string>();

            if (this.SomenteCTesDasOcorrencias)
                cpfCnpjClientes = repOcorrencia.BuscarRemetentesDosCTes(this.Empresa.Codigo, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);
            else if (!gerarPorTomador)
                cpfCnpjClientes = repCTe.BuscarRemetentes(this.Empresa.Codigo, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);
            else
                cpfCnpjClientes = repCTe.BuscarTomadores(this.Empresa.Codigo, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo, this.CpfCnpjRemetente, this.CpfCnpjDestinatario);

            foreach (string cpfCnpjCliente in cpfCnpjClientes)
            {
                this.RegistroEDI = new StringBuilder();

                if (this.SomenteCTesDasOcorrencias)
                    this.CTes = repOcorrencia.BuscarCTesPorRemetente(this.Empresa.Codigo, cpfCnpjCliente, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);
                else if (!gerarPorTomador)
                    this.CTes = repCTe.BuscarPorRemetente(this.Empresa.Codigo, cpfCnpjCliente, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);
                else
                    this.CTes = repCTe.BuscarPorTomador(this.Empresa.Codigo, cpfCnpjCliente, this.DataInicial, this.DataFinal, this.StatusCTe, this.Empresa.TipoAmbiente, this.CodigosCTes, this.CodigoVeiculo);

                byte[] arquivo = System.Text.Encoding.Default.GetBytes(this.GerarEDI());

                ZipEntry entry = new ZipEntry(!gerarPorTomador ? string.Concat(Utilidades.String.RemoveAllSpecialCharacters(this.CTes[0].Remetente.Nome), " - ", this.CTes[0].Remetente.CPF_CNPJ, ".txt") : string.Concat(Utilidades.String.RemoveAllSpecialCharacters(this.CTes[0].Tomador.Nome), " - ", this.CTes[0].Tomador.CPF_CNPJ, ".txt"));

                entry.DateTime = DateTime.Now;

                zipOStream.PutNextEntry(entry);

                zipOStream.Write(arquivo, 0, arquivo.Length);

                zipOStream.CloseEntry();

                this.Ocorrencias = null;

            }

            zipOStream.IsStreamOwner = false;

            zipOStream.Close();

            fZip.Position = 0;

            return fZip;
        }

        public MemoryStream GerarArquivoMDFe()
        {
            string arquivo = this.GerarEDIMDFe();

            if (this.Layout.RemoverDiacriticos)
                arquivo = Utilidades.String.RemoveDiacritics(arquivo);

            MemoryStream memoStream = new MemoryStream();

            memoStream.Write(System.Text.Encoding.Default.GetBytes(arquivo), 0, arquivo.Length);

            memoStream.Position = 0;

            return memoStream;
        }

        public string ObterNomenclaturaLayoutEDI(string nomenclatura, Dominio.Entidades.Empresa transportador, Dominio.Entidades.Cliente cliente, string numero, DateTime dataHora)
        {

            if (!string.IsNullOrWhiteSpace(nomenclatura))
            {
                return nomenclatura.Replace("#CNPJTransportadora#", transportador != null ? transportador.CNPJ : string.Empty)
                                   .Replace("#RazaoTransportadora#", transportador != null ? transportador.RazaoSocial.Replace(" ", "") : string.Empty)
                                   .Replace("#CNPJCliente#", cliente != null ? cliente.CPF_CNPJ_SemFormato : string.Empty)
                                   .Replace("#RazaoCliente#", cliente != null ? cliente.Nome.Replace(" ", "") : string.Empty)
                                   .Replace("#Numero#", numero)
                                   .Replace("#Ano#", dataHora.ToString("yyyy"))
                                   .Replace("#AnoAbreviado#", dataHora.ToString("yy"))
                                   .Replace("#Mes#", dataHora.ToString("MM"))
                                   .Replace("#Dia#", dataHora.ToString("dd"))
                                   .Replace("#Hora#", dataHora.ToString("HH"))
                                   .Replace("#Minutos#", dataHora.ToString("mm"))
                                   .Replace("#Segundos#", dataHora.ToString("ss"));
            }
            else
                return "";
        }

        #endregion

        #region Métodos Privados

        private string GerarEDIRecursivo()
        {
            var registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

            foreach (string registro in registrosPai)
            {
                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                this.EscreverRegistroRecursivo(campos, this.ObjetoDinamico);
            }

            return this.RegistroEDI.ToString();
        }

        private string GerarEDI(bool mudarRegistroNFQuandoSubContratacao = false)
        {
            var codigosCTes = (from obj in this.CTes select obj.Codigo).ToArray();

            if ((from obj in this.Layout.Campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia select obj).Any())
            {
                if (this.Ocorrencias == null || this.Ocorrencias.Count == 0)
                {
                    Repositorio.OcorrenciaDeCTe repOcorrencia = new Repositorio.OcorrenciaDeCTe(UnitOfWork);
                    this.Ocorrencias = repOcorrencia.BuscarPorCTe(codigosCTes);
                }
            }

            var registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

            foreach (string registro in registrosPai)
            {
                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                if (SomenteCTesDasOcorrencias)
                    this.EscreverRegistroOcorrencias(campos);
                else
                    this.EscreverRegistro(campos, null, mudarRegistroNFQuandoSubContratacao);
            }

            return this.RegistroEDI.ToString();
        }

        private void IniciarRegistro()
        {
            if (this.Layout.SeparadorInicialFinal && !string.IsNullOrEmpty(this.Layout.Separador))
                this.RegistroEDI.Append(this.Layout.Separador);
        }

        private void FinalizarRegistro()
        {
            if (!this.Layout.SeparadorInicialFinal && !string.IsNullOrEmpty(this.Layout.Separador))
                this.RegistroEDI.Remove(this.RegistroEDI.Length - 1, 1);

            this.RegistroEDI.AppendLine();

            this.IndiceLinhaArquivo += 1;
        }

        private void EscreverRegistroRecursivo(List<Dominio.Entidades.CampoEDI> campos, dynamic data)
        {
            if (data != null)
            {
                var registrosFilhos = (from obj in this.Layout.Campos where obj.IdentificadorRegistroPai == campos[0].IdentificadorRegistro orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

                if (campos[0].Repetir)
                {
                    foreach (var filho in data)
                    {
                        if (!campos.FirstOrDefault().NaoEscreverRegistro)
                        {
                            this.IniciarRegistro();

                            foreach (Dominio.Entidades.CampoEDI campo in campos)
                            {
                                if (campo.NaoEscreverRegistro)
                                    continue;

                                if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                                    this.EscreverCampoEDI(campo, filho);
                                else
                                    this.EscreverCampoFixo(campo);
                            }

                            this.FinalizarRegistro();
                        }

                        if (registrosFilhos.Count() > 0)
                        {
                            foreach (string registro in registrosFilhos)
                            {
                                var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                                this.EscreverRegistroRecursivo(camposFilho, this.GetNestedPropertyValue(filho, camposFilho[0].PropriedadeObjetoPai));
                            }
                        }
                    }
                }
                else
                {

                    this.IniciarRegistro();
                    bool naoEscreverRegistro = false;

                    foreach (Dominio.Entidades.CampoEDI campo in campos)
                    {
                        if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            this.EscreverCampoEDI(campo, data);
                        else
                            this.EscreverCampoFixo(campo);
                        naoEscreverRegistro = campo.NaoEscreverRegistro;
                    }

                    if (!naoEscreverRegistro)
                        this.FinalizarRegistro();

                    if (registrosFilhos.Count() > 0)
                    {
                        foreach (string registro in registrosFilhos)
                        {
                            var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                            this.EscreverRegistroRecursivo(camposFilho, this.GetNestedPropertyValue(data, camposFilho[0].PropriedadeObjetoPai));
                        }
                    }
                }
            }
        }

        private void PreecherRegistroCTe(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, List<Dominio.Entidades.CampoEDI> campos, List<string> registrosFilhos, bool mudarRegistroNFQuandoSubContratacao = false, int indiceNotas = 0)
        {
            this.IniciarRegistro();

            for (var i = 0; i < campos.Count(); i++)
            {
                if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe)
                {
                    this.EscreverCampoEDI(campos[i], cte);
                }
                else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                {
                    if ((cte.TipoServico != Dominio.Enumeradores.TipoServico.SubContratacao
                        && cte.TipoServico != Dominio.Enumeradores.TipoServico.Redespacho
                        && cte.TipoServico != Dominio.Enumeradores.TipoServico.RedIntermediario
                        ) || !mudarRegistroNFQuandoSubContratacao)
                    {
                        //int indiceNotas = 0;
                        var primeiraPropriedade = campos[i].PropriedadeObjeto;
                        var notasFiscaisDoCTe = cte.Documentos;

                        int quantidadesNotas = notasFiscaisDoCTe.Count();

                        cte.ContinuacaoNotasConemb = "U";

                        if (Layout.QuantidadeNotasSequencia > 0 && ((quantidadesNotas - indiceNotas) > Layout.QuantidadeNotasSequencia))
                            cte.ContinuacaoNotasConemb = "C";


                        while (campos[i] != null && campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                        {
                            if (quantidadesNotas > indiceNotas)
                            {
                                if (!string.IsNullOrWhiteSpace(campos[i].PropriedadeObjeto))
                                    this.EscreverCampoEDI(campos[i], notasFiscaisDoCTe[indiceNotas]);
                                else
                                    this.EscreverCampoFixo(campos[i]);
                            }
                            else if (campos[i].Condicao == Dominio.Enumeradores.CondicaoCampoEDI.First && !string.IsNullOrWhiteSpace(campos[i].PropriedadeObjeto))
                            {
                                this.EscreverCampoEDI(campos[i], notasFiscaisDoCTe.FirstOrDefault());
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(campos[i].ValorFixo))
                                {
                                    if (campos[i].Tipo == Dominio.Enumeradores.TipoCampoEDI.Numerico)
                                    {
                                        long valorCampo = 0L;
                                        long.TryParse(campos[i].ValorFixo, out valorCampo);
                                        this.EscreverDado(valorCampo, campos[i].QuantidadeInteiros);
                                    }
                                    else
                                        this.EscreverDado("", (campos[i].QuantidadeCaracteres + campos[i].QuantidadeDecimais + campos[i].QuantidadeInteiros));
                                }
                                else
                                    this.EscreverDado("", (campos[i].QuantidadeCaracteres + campos[i].QuantidadeDecimais + campos[i].QuantidadeInteiros));
                            }

                            i++;

                            if (campos[i].PropriedadeObjeto == primeiraPropriedade)
                                indiceNotas++;
                        }
                        i--;
                    }
                }
                else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTeAnterior)
                {
                    if ((cte.TipoServico == Dominio.Enumeradores.TipoServico.SubContratacao ||
                        cte.TipoServico == Dominio.Enumeradores.TipoServico.Redespacho ||
                        cte.TipoServico == Dominio.Enumeradores.TipoServico.RedIntermediario)
                        && mudarRegistroNFQuandoSubContratacao)
                    {

                        var primeiraPropriedade = campos[i].PropriedadeObjeto;
                        var documentosCteAnteriorCTe = cte.DocumentosTransporteAnterior;
                        int quantidadeCTes = documentosCteAnteriorCTe.Count();
                        cte.ContinuacaoNotasConemb = "U";
                        if (Layout.QuantidadeNotasSequencia > 0 && ((quantidadeCTes - indiceNotas) > Layout.QuantidadeNotasSequencia))
                            cte.ContinuacaoNotasConemb = "C";

                        while (campos[i] != null && campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTeAnterior)
                        {
                            if (quantidadeCTes > indiceNotas)
                            {
                                if (!string.IsNullOrWhiteSpace(campos[i].PropriedadeObjeto))
                                    this.EscreverCampoEDI(campos[i], documentosCteAnteriorCTe[indiceNotas]);
                                else
                                    this.EscreverCampoFixo(campos[i]);
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(campos[i].ValorFixo))
                                {
                                    if (campos[i].Tipo == Dominio.Enumeradores.TipoCampoEDI.Numerico)
                                    {
                                        long valorCampo = 0L;
                                        long.TryParse(campos[i].ValorFixo, out valorCampo);
                                        this.EscreverDado(valorCampo, campos[i].QuantidadeInteiros);
                                    }
                                    else
                                        this.EscreverDado("", (campos[i].QuantidadeCaracteres + campos[i].QuantidadeDecimais + campos[i].QuantidadeInteiros));
                                }
                                else
                                    this.EscreverDado("", (campos[i].QuantidadeCaracteres + campos[i].QuantidadeDecimais + campos[i].QuantidadeInteiros));
                            }

                            i++;

                            if (campos[i].PropriedadeObjeto == primeiraPropriedade)
                                indiceNotas++;
                        }
                        i--;
                    }
                }
                else
                {
                    this.EscreverCampoEDICondicional(campos[i], cte);
                }
            }

            this.FinalizarRegistro();
            if (cte.ContinuacaoNotasConemb == "C")
                PreecherRegistroCTe(cte, campos, new List<string>(), mudarRegistroNFQuandoSubContratacao, (indiceNotas + 1));

            if (registrosFilhos.Count() > 0)
            {
                foreach (string registro in registrosFilhos)
                {
                    var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                    this.EscreverRegistro(camposFilho, cte);
                }
            }
        }

        private void EscreverRegistro(List<Dominio.Entidades.CampoEDI> campos, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOrigem = null, bool mudarRegistroNFQuandoSubContratacao = false)
        {
            List<string> registrosFilhos = (from obj in this.Layout.Campos where obj.IdentificadorRegistroPai == campos[0].IdentificadorRegistro orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct().ToList();

            if (campos[0].Repetir)
            {
                if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe select obj).Any() && !this.Layout.GerarEDIPorNotaFiscal)//&& !(from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
                    {
                        PreecherRegistroCTe(cte, campos, registrosFilhos, mudarRegistroNFQuandoSubContratacao);
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                {
                    var notasFiscaisDoCTe = cteOrigem != null ? cteOrigem.Documentos : (from obj in this.CTes select obj.Documentos).SelectMany(x => x).ToList();

                    foreach (Dominio.Entidades.DocumentosCTE notaFiscal in notasFiscaisDoCTe)
                    {
                        this.IniciarRegistro();

                        for (var i = 0; i < campos.Count(); i++)
                        {
                            if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                            {
                                this.EscreverCampoEDI(campos[i], notaFiscal);
                            }
                            else
                            {
                                cteOrigem = notaFiscal.CTE;
                                this.EscreverCampoEDICondicional(campos[i], cteOrigem, notaFiscal);
                            }
                        }

                        this.FinalizarRegistro();

                        if (registrosFilhos.Count() > 0)
                        {
                            foreach (string registro in registrosFilhos)
                            {
                                var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();
                                this.EscreverRegistro(camposFilho, notaFiscal.CTE);
                            }
                        }
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico select obj).Any())
                {
                    List<Dominio.Entidades.CampoEDI> camposEDIDinimicos = (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico select obj).ToList();
                    foreach (Dominio.Entidades.CampoEDI campo in camposEDIDinimicos)
                    {
                        if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            this.EscreverCampoEDI(campo, this.ObjetoDinamico);
                        else
                            this.EscreverCampoFixo(campo);

                    }

                }
            }
            else
            {
                this.IniciarRegistro();

                foreach (Dominio.Entidades.CampoEDI campo in campos)
                {
                    if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico)
                    {
                        if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            this.EscreverCampoEDI(campo, this.ObjetoDinamico);
                        else
                            this.EscreverCampoFixo(campo);
                    }
                    else if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Duplicata)
                    {
                        Repositorio.DuplicataCtes repDuplicataCTe = new Repositorio.DuplicataCtes(UnitOfWork);
                        Dominio.Entidades.DuplicataCtes duplicataCTe = repDuplicataCTe.BuscarPorCodigoCTe(this.CTes.FirstOrDefault().Codigo);

                        if (duplicataCTe != null)
                            this.EscreverCampoEDI(campo, duplicataCTe.Duplicata);
                        else
                            this.EscreverDado("", (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros));
                    }
                    else if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.DuplicataParcela)
                    {
                        Repositorio.DuplicataCtes repDuplicataCTe = new Repositorio.DuplicataCtes(UnitOfWork);
                        Repositorio.DuplicataParcelas repDuplicataParcela = new Repositorio.DuplicataParcelas(UnitOfWork);
                        Dominio.Entidades.DuplicataCtes duplicataCTe = repDuplicataCTe.BuscarPorCodigoCTe(this.CTes.FirstOrDefault().Codigo);

                        if (duplicataCTe != null)
                        {
                            List<Dominio.Entidades.DuplicataParcelas> parcelas = repDuplicataParcela.BuscarPorDuplicata(duplicataCTe.Duplicata.Codigo);
                            if (parcelas.Count > 0)
                                this.EscreverCampoEDI(campo, parcelas.FirstOrDefault());
                            else
                                this.EscreverDado("", (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros));
                        }
                        else
                            this.EscreverDado("", (campo.QuantidadeCaracteres + campo.QuantidadeDecimais + campo.QuantidadeInteiros));
                    }
                    else
                    {
                        if (cteOrigem == null)
                        {
                            this.EscreverCampoEDICondicional(campo);
                        }
                        else
                        {
                            if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe)
                                this.EscreverCampoEDI(campo, cteOrigem);
                            else
                                this.EscreverCampoEDICondicional(campo, cteOrigem);
                        }
                    }
                }

                this.FinalizarRegistro();

                if (registrosFilhos.Count() > 0)
                {
                    foreach (string registro in registrosFilhos)
                    {
                        var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                        this.EscreverRegistro(camposFilho, cteOrigem);
                    }
                }
            }
        }

        private void EscreverRegistroOcorrencias(List<Dominio.Entidades.CampoEDI> campos, Dominio.Entidades.ConhecimentoDeTransporteEletronico cteOrigem = null)
        {
            var registrosFilhos = (from obj in this.Layout.Campos where obj.IdentificadorRegistroPai == campos[0].IdentificadorRegistro orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

            if (campos[0].Repetir)
            {
                if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                {
                    var notasFiscaisDoCTe = cteOrigem != null ? cteOrigem.Documentos : (from obj in this.CTes select obj.Documentos).SelectMany(x => x).ToList();

                    foreach (Dominio.Entidades.DocumentosCTE notaFiscal in notasFiscaisDoCTe)
                    {
                        this.IniciarRegistro();

                        for (var i = 0; i < campos.Count(); i++)
                        {
                            if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                            {
                                this.EscreverCampoEDI(campos[i], notaFiscal);
                            }
                            else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe)
                            {
                                this.EscreverCampoEDI(campos[i], notaFiscal.CTE);
                            }
                            else
                            {
                                this.EscreverCampoEDICondicional(campos[i], cteOrigem, notaFiscal);
                            }
                        }

                        this.FinalizarRegistro();

                        if (registrosFilhos.Count() > 0)
                        {
                            foreach (string registro in registrosFilhos)
                            {
                                var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();
                                this.EscreverRegistroOcorrencias(camposFilho, notaFiscal.CTE);
                            }
                        }
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe select obj).Any())
                {
                    foreach (Dominio.Entidades.ConhecimentoDeTransporteEletronico cte in this.CTes)
                    {
                        this.IniciarRegistro();

                        for (var i = 0; i < campos.Count(); i++)
                        {
                            if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe)
                            {
                                this.EscreverCampoEDI(campos[i], cte);
                            }
                            else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                            {
                                int indiceNotas = 0;
                                var primeiraPropriedade = campos[i].PropriedadeObjeto;
                                var notasFiscaisDoCTe = cte.Documentos;

                                while (campos[i] != null && campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                                {
                                    if (notasFiscaisDoCTe.Count() > indiceNotas)
                                    {
                                        if (!string.IsNullOrWhiteSpace(campos[i].PropriedadeObjeto))
                                            this.EscreverCampoEDI(campos[i], notasFiscaisDoCTe[indiceNotas]);
                                        else
                                            this.EscreverCampoFixo(campos[i]);
                                    }
                                    else
                                    {
                                        this.EscreverDado("", (campos[i].QuantidadeCaracteres + campos[i].QuantidadeDecimais + campos[i].QuantidadeInteiros));
                                    }

                                    i++;

                                    if (campos[i].PropriedadeObjeto == primeiraPropriedade)
                                        indiceNotas++;
                                }

                                i--;
                            }
                            else
                            {
                                this.EscreverCampoEDICondicional(campos[i], cte);
                            }
                        }

                        this.FinalizarRegistro();

                        if (registrosFilhos.Count() > 0)
                        {
                            foreach (string registro in registrosFilhos)
                            {
                                var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                                this.EscreverRegistroOcorrencias(camposFilho, cte);
                            }
                        }
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico select obj).Any())
                {
                    List<Dominio.Entidades.CampoEDI> camposEDIDinimicos = (from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico select obj).ToList();
                    foreach (Dominio.Entidades.CampoEDI campo in camposEDIDinimicos)
                    {
                        if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            this.EscreverCampoEDI(campo, this.ObjetoDinamico);
                        else
                            this.EscreverCampoFixo(campo);

                    }

                }
            }
            else
            {
                this.IniciarRegistro();

                foreach (Dominio.Entidades.CampoEDI campo in campos)
                {
                    if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Dinamico)
                    {
                        if (!string.IsNullOrWhiteSpace(campo.PropriedadeObjeto))
                            this.EscreverCampoEDI(campo, this.ObjetoDinamico);
                        else
                            this.EscreverCampoFixo(campo);
                    }
                    else
                    {
                        if (cteOrigem == null)
                        {
                            this.EscreverCampoEDICondicional(campo);
                        }
                        else
                        {
                            if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.CTe)
                                this.EscreverCampoEDI(campo, cteOrigem);
                            else
                                this.EscreverCampoEDICondicional(campo, cteOrigem);
                        }
                    }
                }

                this.FinalizarRegistro();

                if (registrosFilhos.Count() > 0)
                {
                    foreach (string registro in registrosFilhos)
                    {
                        var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                        this.EscreverRegistroOcorrencias(camposFilho, cteOrigem);
                    }
                }
            }
        }

        private void EscreverCampoEDI(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            this.Escrever(campo, this.GetNestedPropertyValue(cte, campo.PropriedadeObjeto));
        }

        private void EscreverCampoEDI(Dominio.Entidades.CampoEDI campo, dynamic objeto)
        {
            if (campo.PropriedadeObjeto == "IndiceLinhaArquivo")
                this.EscreverDado(this.IndiceLinhaArquivo, campo.QuantidadeInteiros);
            else
                this.Escrever(campo, this.GetNestedPropertyValue(objeto, campo.PropriedadeObjeto));

        }

        private void EscreverCampoEDI(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.DocumentosCTE documento)
        {
            this.Escrever(campo, this.GetNestedPropertyValue(documento, campo.PropriedadeObjeto));
        }

        private void EscreverCampoEDI(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.Duplicata duplicata)
        {
            this.Escrever(campo, this.GetNestedPropertyValue(duplicata, campo.PropriedadeObjeto));
        }

        private void EscreverCampoEDI(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.DuplicataParcelas parcela)
        {
            this.Escrever(campo, this.GetNestedPropertyValue(parcela, campo.PropriedadeObjeto));
        }

        private void EscreverCampoEDICondicional(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null, Dominio.Entidades.DocumentosCTE notaFiscal = null)
        {
            switch (campo.Objeto)
            {
                case Dominio.Enumeradores.ObjetoCampoEDI.ComponentePrestacao:

                    this.EscreverCampoComponentePrestacao(campo, cte);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.CTe:

                    this.EscreverCampoCTe(campo, cte);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.InformacaoCarga:

                    this.EscreverCampoInformacaoCarga(campo, cte);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal:

                    this.EscreverCampoNotaFiscal(campo, cte);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.Ocorrencia:

                    this.EscreverCampoOcorrencia(campo, (cte != null ? cte : (notaFiscal != null ? notaFiscal.CTE : null)));

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.Veiculo:

                    this.EscreverCampoVeiculo(campo, cte);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.Nenhum:

                    this.EscreverCampoFixo(campo);

                    break;

                case Dominio.Enumeradores.ObjetoCampoEDI.Global:

                    this.EscreverCampoGlobal(campo);

                    break;
            }
        }

        private void EscreverCampoGlobal(Dominio.Entidades.CampoEDI campo)
        {
            if (campo.PropriedadeObjeto == "IndiceLinhaArquivo")
            {
                this.EscreverDado(this.IndiceLinhaArquivo, campo.QuantidadeInteiros);
            }
        }

        private void EscreverCampoComponentePrestacao(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.CTes select obj.ComponentesPrestacao.Count()).Sum());
                    else
                        this.Escrever(campo, cte.ComponentesPrestacao.Count());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue(this.CTes.First().ComponentesPrestacao.First(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue(cte.ComponentesPrestacao.First(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.ComponentesPrestacao) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());
                    else
                        this.Escrever(campo, (from obj in cte.ComponentesPrestacao select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.ComponentesPrestacao) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    else
                        this.Escrever(campo, (from obj in cte.ComponentesPrestacao select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.ComponentesPrestacao) select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());
                    else
                        this.Escrever(campo, (from obj in cte.ComponentesPrestacao select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoCTe(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (SomenteCTesDasOcorrencias)
                        this.Escrever(campo, (from obj in this.CTes select obj.Documentos.Count()).Sum());
                    else
                        this.Escrever(campo, (from obj in this.CTes select obj).Count());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    this.Escrever(campo, cte != null ? this.GetNestedPropertyValue(cte, campo.PropriedadeObjeto) : this.GetNestedPropertyValue((from obj in this.CTes select obj).First(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    this.Escrever(campo, (from obj in this.CTes select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    this.Escrever(campo, (from obj in this.CTes select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    this.Escrever(campo, (from obj in this.CTes select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoNotaFiscal(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.CTes select obj.Documentos.Count()).Sum());
                    else
                        this.Escrever(campo, cte.Documentos);

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue(this.CTes.First().Documentos.First(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue(cte.Documentos.First(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Documentos) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());
                    else
                        this.Escrever(campo, (from obj in cte.Documentos select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Documentos) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    else
                        this.Escrever(campo, (from obj in cte.Documentos select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Documentos) select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());
                    else
                        this.Escrever(campo, (from obj in cte.Documentos select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoOcorrencia(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select obj).Count());
                    else
                        this.Escrever(campo, (from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia select obj).Count());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue((from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select obj).First(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue((from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia descending select obj).First(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Last:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue((from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select obj).LastOrDefault(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue((from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia select obj).LastOrDefault(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());
                    else
                        this.Escrever(campo, (from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    else
                        this.Escrever(campo, (from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.Ocorrencias orderby obj.DataDaOcorrencia select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());
                    else
                        this.Escrever(campo, (from obj in this.Ocorrencias where obj.CTe.Codigo == cte.Codigo orderby obj.DataDaOcorrencia select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoInformacaoCarga(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.CTes select obj.QuantidadesCarga.Count()).Sum());
                    else
                        this.Escrever(campo, cte.QuantidadesCarga.Count());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue(this.CTes.First().QuantidadesCarga.FirstOrDefault(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue(cte.QuantidadesCarga.FirstOrDefault(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.QuantidadesCarga) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());
                    else
                        this.Escrever(campo, (from obj in cte.QuantidadesCarga select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.QuantidadesCarga) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    else
                        this.Escrever(campo, (from obj in cte.QuantidadesCarga select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.QuantidadesCarga) select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());
                    else
                        this.Escrever(campo, (from obj in cte.QuantidadesCarga select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoVeiculo(Dominio.Entidades.CampoEDI campo, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = null)
        {
            switch (campo.Condicao)
            {
                case Dominio.Enumeradores.CondicaoCampoEDI.Count:

                    if (cte == null)
                        this.Escrever(campo, (from obj in this.CTes select obj.Veiculos.Count()).Sum());
                    else
                        this.Escrever(campo, cte.Veiculos.Count());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.First:
                case Dominio.Enumeradores.CondicaoCampoEDI.Nenhum:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue(this.CTes.First().Veiculos.FirstOrDefault(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue(cte.Veiculos.FirstOrDefault(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Last:

                    if (cte == null)
                        this.Escrever(campo, this.GetNestedPropertyValue(this.CTes.Last().Veiculos.OrderBy(o => o.Codigo).LastOrDefault(), campo.PropriedadeObjeto));
                    else
                        this.Escrever(campo, this.GetNestedPropertyValue(cte.Veiculos.OrderBy(o => o.Codigo).LastOrDefault(), campo.PropriedadeObjeto));

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Max:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Veiculos) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());
                    else
                        this.Escrever(campo, (from obj in cte.Veiculos orderby obj.Codigo select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Max());

                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Min:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Veiculos) select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    else
                        this.Escrever(campo, (from obj in cte.Veiculos orderby obj.Codigo select this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Min());
                    break;

                case Dominio.Enumeradores.CondicaoCampoEDI.Sum:

                    if (cte == null)
                        this.Escrever(campo, (from obj in (from x in this.CTes select x.Veiculos) select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());
                    else
                        this.Escrever(campo, (from obj in cte.Veiculos orderby obj.Codigo select (decimal)this.GetNestedPropertyValue(obj, campo.PropriedadeObjeto)).Sum());

                    break;
            }
        }

        private void EscreverCampoFixo(Dominio.Entidades.CampoEDI campo)
        {
            if (campo.Tipo == Dominio.Enumeradores.TipoCampoEDI.DataEHora)
            {
                this.EscreverDado(DateTime.Now, campo.Mascara);
            }
            else
            {
                if (campo.Tipo == Dominio.Enumeradores.TipoCampoEDI.Numerico)
                    this.EscreverDado(campo.ValorFixo, campo.QuantidadeInteiros);
                else
                    this.EscreverDado(campo.ValorFixo, campo.QuantidadeCaracteres);
            }

            if (!campo.NaoEscreverRegistro && !string.IsNullOrEmpty(this.Layout.Separador) && string.IsNullOrWhiteSpace(campo.PropriedadeObjetoPai))
                this.RegistroEDI.Append(this.Layout.Separador);
        }

        private void Escrever(Dominio.Entidades.CampoEDI campo, object valor)
        {
            if (!string.IsNullOrEmpty(campo.Expressao))
            {
                valor = processarExpressaoEDI(campo, valor?.ToString() ?? "");
            }
            switch (campo.Tipo)
            {
                case Dominio.Enumeradores.TipoCampoEDI.Alfanumerico:

                    string dado = valor == null ? "" : valor.ToString();

                    if (campo.RemoverCaracteresEspeciais)
                        dado = Utilidades.String.RemoveSpecialCharacters(dado);

                    this.EscreverDado(dado, campo.QuantidadeCaracteres);

                    break;

                case Dominio.Enumeradores.TipoCampoEDI.DataEHora:

                    if (valor == null)
                        this.EscreverDado("", campo.Mascara.Length);
                    else if (valor.GetType() == typeof(DateTime))
                        this.EscreverDado((DateTime)valor, campo.Mascara);
                    else if (valor.GetType() == typeof(DateTime?))
                        this.EscreverDado((DateTime?)valor, campo.Mascara);

                    break;

                case Dominio.Enumeradores.TipoCampoEDI.Decimal:

                    decimal.TryParse(valor != null ? valor.ToString() : "0", out decimal decimalConvertido);

                    if (campo.QuantidadeCaracteres > 0)
                        this.EscreverDado(decimalConvertido, campo.QuantidadeInteiros, campo.QuantidadeDecimais, campo.QuantidadeCaracteres);
                    else
                        this.EscreverDado(decimalConvertido, campo.QuantidadeInteiros, campo.QuantidadeDecimais);

                    break;

                case Dominio.Enumeradores.TipoCampoEDI.Numerico:

                    long.TryParse(valor != null ? valor.ToString() : "0", out long longConvertido);

                    this.EscreverDado(longConvertido, campo.QuantidadeInteiros);

                    break;
            }

            if (!string.IsNullOrEmpty(this.Layout.Separador))
                this.RegistroEDI.Append(this.Layout.Separador);
        }

        private object GetNestedPropertyValue(object obj, string propertyName)
        {
            string[] properties = propertyName.Split('.');

            foreach (string part in properties)
            {
                if (obj == null)
                    return null;

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);

                if (info == null)
                    return null;

                obj = info.GetValue(obj, null);
            }

            return obj;
        }

        private string processarExpressaoEDI(Dominio.Entidades.CampoEDI campo, string valor)
        {
            string expressao = campo.Expressao;

            if (!expressao.Contains("remove"))
            {
                List<string> condicoes = new List<string>();
                List<string> retornos = new List<string>();

                string retorno = "";
                string conteudo = "";
                bool inicio = false;
                for (int i = 0; i < expressao.Length; i++)
                {
                    if (expressao[i] != ')' && !inicio)
                    {
                        conteudo += expressao[i];
                    }
                    else
                    {
                        if (!inicio)
                        {
                            conteudo += expressao[i];
                            inicio = true;
                        }
                        else
                        {
                            if (expressao[i] != '(')
                            {
                                retorno += expressao[i];
                                if (i == expressao.Length - 1)
                                {
                                    condicoes.Add(conteudo);
                                    retornos.Add(retorno);
                                }
                            }
                            else
                            {
                                condicoes.Add(conteudo);
                                retornos.Add(retorno);
                                conteudo = "";
                                retorno = "";
                                inicio = false;

                                conteudo += expressao[i];
                            }
                        }
                    }
                }

                for (int iCondi = 0; iCondi <= condicoes.Count - 1; iCondi++)
                {
                    string condicao = condicoes[iCondi];

                    string campoEDI = "";
                    string operacao = "";
                    string valorCompara = "";
                    string tipocomparacao = "";

                    bool comparacao = false;
                    for (int i = 0; i < condicao.Length; i++)
                    {
                        if (campoEDI.Trim() == "#campo")
                        {
                            if (condicao[i] != '"' && valorCompara == "")
                            {
                                operacao += condicao[i];
                            }
                            else
                            {
                                if (condicao[i] == '#' || condicao[i] == '&' || condicao[i] == '|' || condicao[i] == ')')
                                {
                                    valorCompara = valorCompara.Replace("\"", "").TrimEnd().TrimStart();
                                    if (operacao.Trim() == "==")
                                    {
                                        if (valor == valorCompara)
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    if (operacao.Trim() == "!=")
                                    {
                                        if (valor != valorCompara)
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    if (operacao.Trim() == ">=")
                                    {
                                        if (decimal.Parse(valor) >= decimal.Parse(valorCompara))
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    if (operacao.Trim() == "<=")
                                    {
                                        if (decimal.Parse(valor) <= decimal.Parse(valorCompara))
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    if (operacao.Trim() == ">")
                                    {
                                        if (decimal.Parse(valor) > decimal.Parse(valorCompara))
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    if (operacao.Trim() == "<")
                                    {
                                        if (decimal.Parse(valor) < decimal.Parse(valorCompara))
                                        {
                                            if ((tipocomparacao != "&&" || comparacao == true))
                                                comparacao = true;
                                        }
                                    }
                                    operacao = "";
                                    valorCompara = "";
                                    campoEDI = "";
                                    campoEDI += condicao[i];
                                }
                                else
                                {
                                    valorCompara += condicao[i];
                                }
                            }
                        }
                        else
                        {
                            if (campoEDI.Trim() == "&&" || campoEDI.Trim() == "||")
                            {
                                tipocomparacao = campoEDI.Trim();
                                campoEDI = "";
                            }
                            else
                            {
                                if (condicao[i] != '(')
                                    campoEDI += condicao[i];
                            }

                        }
                    }
                    if (comparacao || condicao.Trim() == "()")
                    {
                        if (retornos[iCondi] != "#campo")
                            valor = retornos[iCondi];
                        break;
                    }
                }
            }
            else
            {
                string removeText = expressao.Split('-')[1];
                valor = valor.Replace(removeText, "");
            }

            return valor;

        }

        #endregion

        #region Métodos Privados de Manipulação de Valores

        private void EscreverDado(string dado, int numeroCaracteres)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                dado = Utilidades.String.ReplaceInvalidCharacters(dado);

                if (numeroCaracteres == 0 && !string.IsNullOrEmpty(this.Layout.Separador))
                {
                    this.RegistroEDI.Append(dado);
                }
                else if (dado.Length > numeroCaracteres)
                {
                    this.RegistroEDI.Append(dado.Remove(numeroCaracteres, (dado.Length - numeroCaracteres)));
                }
                else
                {
                    this.RegistroEDI.Append(dado);

                    if (dado.Length != numeroCaracteres)
                    {
                        if (!Layout.CamposPorIndices)
                            this.RegistroEDI.Append(new string(' ', numeroCaracteres - dado.Length));
                    }
                }
            }
            else
            {
                if (!Layout.CamposPorIndices)
                    this.RegistroEDI.Append(new string(' ', numeroCaracteres));
            }
        }

        private void EscreverDado(DateTime data, string formato = null)
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.RegistroEDI.Append(data.ToString("ddMMyyyy"));
            else
                this.RegistroEDI.Append(data.ToString(formato));
        }

        private void EscreverDado(DateTime? data, string formato = null)
        {
            if (string.IsNullOrWhiteSpace(formato))
                this.RegistroEDI.Append(data.Value.ToString("ddMMyyyy"));
            else
                this.RegistroEDI.Append(data.Value.ToString(formato));
        }

        private void EscreverDado(decimal valor, int numeroInteiros, int numeroDecimais)
        {
            string formato = "{0:";

            for (var i = 0; i < numeroInteiros; i++)
                formato += "0";

            formato += ".";

            for (var i = 0; i < numeroDecimais; i++)
                formato += "0";

            formato += "}";

            string valorFormatado = "";

            if (!Layout.CamposPorIndices)
                valorFormatado = string.Format(formato, valor).Replace(".", "").Replace(",", "");
            else
                valorFormatado = valor.ToString("n" + numeroDecimais).Replace(".", "").Replace(",", "");

            if (!string.IsNullOrEmpty(this.Layout.SeparadorDecimal))
            {
                if (!Layout.CamposPorIndices)
                    valorFormatado = valorFormatado.Insert(numeroInteiros, this.Layout.SeparadorDecimal);
                else if (numeroDecimais > 0)
                    valorFormatado = valorFormatado.Insert(valorFormatado.Length - numeroDecimais, this.Layout.SeparadorDecimal);
            }

            this.RegistroEDI.Append(valorFormatado);
        }

        private void EscreverDado(decimal valor, int numeroInteiros, int numeroDecimais, int numeroCaracteres)
        {
            string formato = "{0:";

            for (var i = 0; i < numeroInteiros; i++)
                formato += "0";

            formato += ".";

            for (var i = 0; i < numeroDecimais; i++)
                formato += "0";

            formato += "}";

            string valorFormatado = "";

            if (!Layout.CamposPorIndices)
                valorFormatado = string.Format(formato, valor).Replace(".", "").Replace(",", "");
            else
                valorFormatado = valor.ToString("n" + numeroDecimais).Replace(".", "").Replace(",", "");

            if (!string.IsNullOrEmpty(this.Layout.SeparadorDecimal))
            {
                if (!Layout.CamposPorIndices)
                    valorFormatado = valorFormatado.Insert(numeroInteiros, this.Layout.SeparadorDecimal);
                else if (numeroDecimais > 0)
                    valorFormatado = valorFormatado.Insert(valorFormatado.Length - numeroDecimais, this.Layout.SeparadorDecimal);
            }

            if (valorFormatado.Length > numeroCaracteres)
                this.RegistroEDI.Append(valorFormatado.Remove(numeroCaracteres, (valorFormatado.Length - numeroCaracteres)));
            else
                this.RegistroEDI.Append(valorFormatado);
        }

        private void EscreverDado(long valor, int numeroDigitos)
        {
            if (!Layout.CamposPorIndices)
                this.RegistroEDI.Append(valor.ToString("D" + numeroDigitos.ToString()));
            else
                this.RegistroEDI.Append(valor.ToString());
        }

        #endregion

        #region Métodos Privados de MDFe

        private string GerarEDIMDFe()
        {
            var registrosPai = (from obj in this.Layout.Campos where string.IsNullOrWhiteSpace(obj.IdentificadorRegistroPai) orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

            foreach (string registro in registrosPai)
            {
                var campos = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                this.EscreverRegistroMDFe(campos);
            }

            return this.RegistroEDI.ToString();
        }

        private void EscreverRegistroMDFe(List<Dominio.Entidades.CampoEDI> campos)
        {
            var registrosFilhos = (from obj in this.Layout.Campos where obj.IdentificadorRegistroPai == campos[0].IdentificadorRegistro orderby obj.Ordem ascending select obj.IdentificadorRegistro).Distinct();

            if (campos[0].Repetir)
            {
                if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal select obj).Any())
                {
                    var notasFiscais = this.EDIMDFe.NotasFiscais;

                    for (var nIndex = 0; nIndex < notasFiscais.Count; nIndex++)
                    {
                        this.IniciarRegistro();

                        for (var i = 0; i < campos.Count(); i++)
                        {
                            if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.NotaFiscal)
                            {
                                this.EscreverCampoEDI(campos[i], notasFiscais[nIndex]);
                            }
                            else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.MDFe)
                            {
                                this.EscreverCampoEDI(campos[i], this.EDIMDFe);
                            }
                            else if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Global)
                            {
                                if (campos[i].PropriedadeObjeto == "IndiceNotaFiscal")
                                    this.EscreverDado(nIndex + 1, campos[i].QuantidadeInteiros);
                            }
                            else
                            {
                                this.EscreverCampoFixo(campos[i]);
                            }
                        }

                        this.FinalizarRegistro();
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.MDFe select obj).Any())
                {
                    this.IniciarRegistro();

                    for (var i = 0; i < campos.Count(); i++)
                    {
                        if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.MDFe)
                        {
                            this.EscreverCampoEDI(campos[i], this.EDIMDFe);
                        }
                    }

                    this.FinalizarRegistro();

                    if (registrosFilhos.Count() > 0)
                    {
                        foreach (string registro in registrosFilhos)
                        {
                            var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                            this.EscreverRegistroMDFe(camposFilho);
                        }
                    }
                }
                else if ((from obj in campos where obj.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Lacre select obj).Any())
                {
                    var lacres = this.EDIMDFe.MDFe.Lacres;

                    for (var nIndex = 0; nIndex < lacres.Count; nIndex++)
                    {
                        this.IniciarRegistro();

                        for (var i = 0; i < campos.Count(); i++)
                        {
                            if (campos[i].Objeto == Dominio.Enumeradores.ObjetoCampoEDI.Lacre)
                            {
                                this.EscreverCampoEDI(campos[i], lacres[nIndex]);
                            }
                            else
                            {
                                this.EscreverCampoFixo(campos[i]);
                            }
                        }

                        this.FinalizarRegistro();
                    }
                }
            }
            else
            {
                this.IniciarRegistro();

                foreach (Dominio.Entidades.CampoEDI campo in campos)
                {
                    if (campo.Objeto == Dominio.Enumeradores.ObjetoCampoEDI.MDFe)
                        this.EscreverCampoEDI(campo, this.EDIMDFe);
                    else
                        this.EscreverCampoFixo(campo);
                }

                this.FinalizarRegistro();

                if (registrosFilhos.Count() > 0)
                {
                    foreach (string registro in registrosFilhos)
                    {
                        var camposFilho = (from obj in this.Layout.Campos where obj.IdentificadorRegistro == registro orderby obj.Ordem ascending select obj).ToList();

                        this.EscreverRegistroMDFe(camposFilho);
                    }
                }
            }
        }

        #endregion

    }
}
