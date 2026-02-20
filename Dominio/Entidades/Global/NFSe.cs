using System;
using System.Collections.Generic;

namespace Dominio.Entidades
{
    [NHibernate.Mapping.Attributes.Class(0, Table = "T_NFSE", EntityName = "NFSe", Name = "Dominio.Entidades.NFSe", NameType = typeof(NFSe))]
    public class NFSe : EntidadeBase
    {
        [NHibernate.Mapping.Attributes.Id(0, Name = "Codigo", Type = "System.Int32", Column = "NFSE_CODIGO")]
        [NHibernate.Mapping.Attributes.Generator(1, Class = "native")]
        public virtual int Codigo { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Empresa", Column = "EMP_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Empresa Empresa { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NotaFiscalServico", Column = "NFS_CODIGO", NotNull = false, Cascade = "all", Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NotaFiscalServico NFS { get; set; }
        
        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "NaturezaNFSe", Column = "NAN_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual NaturezaNFSe Natureza { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "Localidade", Column = "LOC_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Localidade LocalidadePrestacaoServico { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "RPSNFSe", Column = "RPS_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual RPSNFSe RPS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteNFSe", Column = "PNF_CODIGO_TOMADOR", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual ParticipanteNFSe Tomador { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "ParticipanteNFSe", Column = "PNF_CODIGO_INTERMEDIARIO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy, Cascade = "all")]
        public virtual ParticipanteNFSe Intermediario { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "EmpresaSerie", Column = "ESE_CODIGO", NotNull = true, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual EmpresaSerie Serie { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Numero", Column = "NFSE_NUMERO", TypeType = typeof(int), NotNull = true)]
        public virtual int Numero { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroRPS", Column = "NFSE_NUMERO_RPS", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroRPS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "SerieSubstituicao", Column = "NFSE_SERIE_SUBSTITUICAO", TypeType = typeof(string), Length = 3, NotNull = false)]
        public virtual string SerieSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroSubstituicao", Column = "NFSE_NUMERO_SUBSTITUICAO", TypeType = typeof(int), NotNull = false)]
        public virtual int? NumeroSubstituicao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Ambiente", Column = "NFSE_AMBIENTE", TypeType = typeof(Dominio.Enumeradores.TipoPessoa), NotNull = true)]
        public virtual Dominio.Enumeradores.TipoAmbiente Ambiente { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "Status", Column = "NFSE_STATUS", TypeType = typeof(Dominio.Enumeradores.StatusNFSe), NotNull = true)]
        public virtual Dominio.Enumeradores.StatusNFSe Status { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataIntegracao", Column = "NFSE_DATA_INTEGRACAO", TypeType = typeof(DateTime), NotNull = false)]
        public virtual DateTime? DataIntegracao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "DataEmissao", Column = "NFSE_DATA_EMISSAO", TypeType = typeof(DateTime), NotNull = true)]
        public virtual DateTime DataEmissao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorServicos", Column = "NFSE_VALOR_SERVICOS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorServicos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDeducoes", Column = "NFSE_VALOR_DEDUCOES", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDeducoes { get; set; }


        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoPIS", Column = "NFSE_BASE_CALCULO_PIS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoCOFINS", Column = "NFSE_BASE_CALCULO_COFINS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaPIS", Column = "NFSE_ALIQUOTA_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCOFINS", Column = "NFSE_ALIQUOTA_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCOFINS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorPIS", Column = "NFSE_VALOR_PIS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorPIS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCOFINS", Column = "NFSE_VALOR_COFINS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorCOFINS { get; set; }
        
        
        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorINSS", Column = "NFSE_VALOR_INSS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorINSS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIR", Column = "NFSE_VALOR_IR", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorIR { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCSLL", Column = "NFSE_VALOR_CSLL", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorCSLL { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ISSRetido", Column = "NFSE_ISS_RETIDO", TypeType = typeof(bool), NotNull = true)]
        public virtual bool ISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISSRetido", Column = "NFSE_VALOR_ISS_RETIDO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorISSRetido { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorOutrasRetencoes", Column = "NFSE_VALOR_OUTRAS_RETENCOES", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorOutrasRetencoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoIncondicionado", Column = "NFSE_VALOR_DESC_INCONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDescontoIncondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorDescontoCondicionado", Column = "NFSE_VALOR_DESC_CONDICIONADO", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorDescontoCondicionado { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaISS", Column = "NFSE_ALIQUOTA_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 6, NotNull = true)]
        public virtual decimal AliquotaISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoISS", Column = "NFSE_BASE_CALCULO_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal BaseCalculoISS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorISS", Column = "NFSE_VALOR_ISS", TypeType = typeof(decimal), Scale = 4, Precision = 16, NotNull = true)]
        public virtual decimal ValorISS { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Class = "OutrasAliquotas", Column = "TOA_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Dominio.Entidades.Embarcador.Imposto.OutrasAliquotas OutrasAliquotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFSE_NBS", TypeType = typeof(string), Length = 9, NotNull = false)]
        public virtual string NBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFSE_CODIGO_INDICADOR_OPERACAO", TypeType = typeof(string), Length = 6, NotNull = false)]
        public virtual string CodigoIndicadorOperacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CSTIBSCBS", Column = "NFSE_CST_IBSCBS", TypeType = typeof(string), Length = 5, NotNull = false)]
        public virtual string CSTIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ClassificacaoTributariaIBSCBS", Column = "NFSE_CLASSIFICACAO_TRIBUTARIA_IBSCBS", TypeType = typeof(string), Length = 8, NotNull = false)]
        public virtual string ClassificacaoTributariaIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "BaseCalculoIBSCBS", Column = "NFSE_BASE_CALCULO_IBSCBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal BaseCalculoIBSCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSEstadual", Column = "NFSE_ALIQUOTA_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSEstadual", Column = "NFSE_PERCENTUAL_REDUCAO_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSEstadual", Column = "NFSE_VALOR_IBS_ESTADUAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSEstadual { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaIBSMunicipal", Column = "NFSE_ALIQUOTA_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoIBSMunicipal", Column = "NFSE_PERCENTUAL_REDUCAO_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorIBSMunicipal", Column = "NFSE_VALOR_IBS_MUNICIPAL", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorIBSMunicipal { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "AliquotaCBS", Column = "NFSE_ALIQUOTA_CBS", TypeType = typeof(decimal), Scale = 2, Precision = 18, NotNull = false)]
        public virtual decimal AliquotaCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PercentualReducaoCBS", Column = "NFSE_PERCENTUAL_REDUCAO_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal PercentualReducaoCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "ValorCBS", Column = "NFSE_VALOR_CBS", TypeType = typeof(decimal), Scale = 3, Precision = 18, NotNull = false)]
        public virtual decimal ValorCBS { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CodigoVerificacao", Column = "NFSE_CODIGO_VERIFICACAO", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string CodigoVerificacao { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "OutrasInformacoes", Column = "NFSE_OUTRAS_INFORMACOES", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string OutrasInformacoes { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "JustificativaCancelamento", Column = "NFSE_JUSTIFICATIVA_CANCELAMENTO", TypeType = typeof(string), Length = 255, NotNull = false)]
        public virtual string JustificativaCancelamento { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "PesoKG", Column = "NFSE_PESO_KG", TypeType = typeof(decimal), Scale = 4, Precision = 15, NotNull = false)]
        public virtual decimal PesoKG { get; set; }

        [NHibernate.Mapping.Attributes.ManyToOne(0, Name = "Veiculo", Class = "Veiculo", Column = "VEI_CODIGO", NotNull = false, Lazy = NHibernate.Mapping.Attributes.Laziness.Proxy)]
        public virtual Veiculo Veiculo { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "IDEnotas", Column = "NFSE_ID_ENOTAS", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string IDEnotas { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "CFOP", Column = "NFSE_CFOP", TypeType = typeof(string), Length = 20, NotNull = false)]
        public virtual string CFOP { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroPrefeitura", Column = "NFSE_NUMERO_PREFEITURA", TypeType = typeof(string), Length = 100, NotNull = false)]
        public virtual string NumeroPrefeitura { get; set; }

        [NHibernate.Mapping.Attributes.Bag(0, Name = "Documentos", Cascade = "none", Lazy = NHibernate.Mapping.Attributes.CollectionLazy.True, Table = "T_NFSE_DOCS")]
        [NHibernate.Mapping.Attributes.Key(1, Column = "NFSE_CODIGO")]
        [NHibernate.Mapping.Attributes.ManyToMany(2, Class = "DocumentosNFSe", Column = "DNS_CODIGO")]
        public virtual IList<Dominio.Entidades.DocumentosNFSe> Documentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Name = "NumeroDocumentos", Formula = @"SUBSTRING((SELECT ', ' + CAST(_documentos.DNS_NUMERO AS NVARCHAR(20))
                                                                                        FROM T_NFSE_DOCS _documentos
                                                                                        WHERE _documentos.NFSE_CODIGO = NFSE_CODIGO FOR XML PATH('')), 3, 1000)", TypeType = typeof(string), Lazy = true)]
        public virtual string NumeroDocumentos { get; set; }

        [NHibernate.Mapping.Attributes.Property(0, Column = "NFSE_LOG", Type = "StringClob", NotNull = false)]
        public virtual string Log { get; set; }

        public virtual void SetarParticipante(Dominio.Entidades.Cliente cliente, Enumeradores.TipoClienteNotaFiscalServico tipoParticipante, Dominio.ObjetosDeValor.Endereco endereco = null)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoClienteNotaFiscalServico.Intermediario:
                    this.Intermediario = this.ObterParticipante(this.Intermediario, cliente, endereco);
                    break;
                case Enumeradores.TipoClienteNotaFiscalServico.Tomador:
                    this.Tomador = this.ObterParticipante(this.Tomador, cliente, endereco);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.Cliente cliente, Enumeradores.TipoClienteNotaFiscalServico tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoClienteNotaFiscalServico.Intermediario:
                    this.Intermediario = this.ObterParticipante(this.Intermediario, cliente, pais);
                    break;
                case Enumeradores.TipoClienteNotaFiscalServico.Tomador:
                    this.Tomador = this.ObterParticipante(this.Tomador, cliente, pais);
                    break;
                default:
                    break;
            }
        }

        public virtual void SetarParticipanteExportacao(Dominio.ObjetosDeValor.CTe.Cliente cliente, Enumeradores.TipoClienteNotaFiscalServico tipoParticipante, Dominio.Entidades.Pais pais)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoClienteNotaFiscalServico.Intermediario:
                    this.Intermediario = this.ObterParticipante(this.Intermediario, cliente, pais);
                    break;
                case Enumeradores.TipoClienteNotaFiscalServico.Tomador:
                    this.Tomador = this.ObterParticipante(this.Tomador, cliente, pais);
                    break;
                default:
                    break;
            }
        }

        public virtual ParticipanteNFSe ObterParticipante(Dominio.Enumeradores.TipoClienteNotaFiscalServico tipoParticipante)
        {
            switch (tipoParticipante)
            {
                case Enumeradores.TipoClienteNotaFiscalServico.Intermediario:
                    return this.Intermediario;
                case Enumeradores.TipoClienteNotaFiscalServico.Tomador:
                    return this.Tomador;
                default:
                    return null;
            }
        }

        private ParticipanteNFSe ObterParticipante(ParticipanteNFSe participante, Cliente cliente, Dominio.ObjetosDeValor.Endereco endereco)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteNFSe();

                participante.Atividade = cliente.Atividade;
                participante.Cidade = null;

                participante.CPF_CNPJ = cliente.CPF_CNPJ_SemFormato;
                participante.Email = cliente.Email;
                participante.EmailContador = cliente.EmailContador;
                participante.EmailContadorStatus = cliente.EmailContadorStatus == "A" ? true : false;
                participante.EmailContato = cliente.EmailContato;
                participante.EmailContatoStatus = cliente.EmailContatoStatus == "A" ? true : false;
                participante.EmailStatus = cliente.EmailStatus == "A" ? true : false;
                participante.Exterior = false;
                participante.IE_RG = cliente.IE_RG;
                participante.InscricaoMunicipal = cliente.InscricaoMunicipal;
                participante.InscricaoSuframa = cliente.InscricaoSuframa;
                participante.Nome = cliente.Nome;
                participante.NomeFantasia = cliente.NomeFantasia;
                participante.Pais = null;
                participante.Telefone2 = cliente.Telefone2;
                participante.Tipo = cliente.Tipo == "J" ? Enumeradores.TipoPessoa.Juridica : Enumeradores.TipoPessoa.Fisica;
                //participante.CodigoIntegracao = cliente.CodigoIntegracao;

                if (endereco == null)
                {
                    participante.Bairro = cliente.Bairro;
                    participante.CEP = cliente.CEP;
                    participante.Complemento = cliente.Complemento;
                    participante.Endereco = cliente.Endereco;
                    participante.Localidade = cliente.Localidade;
                    participante.Numero = cliente.Numero;
                    participante.Telefone1 = cliente.Telefone1;
                    participante.SalvarEndereco = true;
                }
                else
                {
                    participante.Bairro = endereco.Bairro;
                    participante.CEP = endereco.CEP;
                    participante.Complemento = endereco.Complemento;
                    participante.Endereco = endereco.Logradouro;
                    participante.Localidade = endereco.Cidade;
                    participante.Numero = endereco.Numero;
                    participante.Telefone1 = endereco.Telefone;
                    participante.SalvarEndereco = false;
                }
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteNFSe ObterParticipante(ParticipanteNFSe participante, ObjetosDeValor.Cliente cliente, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteNFSe();

                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.NumeroDocumentoExterior = cliente.NumeroDocumentoExportacao;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        private ParticipanteNFSe ObterParticipante(ParticipanteNFSe participante, ObjetosDeValor.CTe.Cliente cliente, Dominio.Entidades.Pais pais)
        {
            if (cliente != null)
            {
                if (participante == null)
                    participante = new ParticipanteNFSe();

                participante.Atividade = null;
                participante.Bairro = cliente.Bairro;
                participante.CEP = null;
                participante.Cidade = cliente.Cidade;
                participante.Complemento = cliente.Complemento;
                participante.CPF_CNPJ = null;
                participante.Email = cliente.Emails;
                participante.EmailContador = null;
                participante.EmailContadorStatus = false;
                participante.EmailContato = null;
                participante.EmailContatoStatus = false;
                participante.EmailStatus = true;
                participante.Endereco = cliente.Endereco;
                participante.Exterior = true;
                participante.IE_RG = null;
                participante.InscricaoMunicipal = null;
                participante.InscricaoSuframa = null;
                participante.Localidade = null;
                participante.Nome = cliente.RazaoSocial;
                participante.NomeFantasia = null;
                participante.Numero = cliente.Numero;
                participante.NumeroDocumentoExterior = cliente.NumeroDocumentoExportacao;
                participante.Pais = pais;
                participante.Telefone1 = null;
                participante.Telefone2 = null;
                participante.Tipo = Enumeradores.TipoPessoa.Juridica;
                participante.CodigoIntegracao = cliente.CodigoCliente;
            }
            else
            {
                participante = null;
            }

            return participante;
        }

        public virtual string DescricaoStatus
        {
            get
            {
                switch (this.Status)
                {
                    case Enumeradores.StatusNFSe.Autorizado:
                        return "Autorizado";
                    case Enumeradores.StatusNFSe.Cancelado:
                        return "Cancelado";
                    case Enumeradores.StatusNFSe.EmCancelamento:
                        return "Em Cancelamento";
                    case Enumeradores.StatusNFSe.EmDigitacao:
                        return "Em Digitação";
                    case Enumeradores.StatusNFSe.Enviado:
                        return "Enviado";
                    case Enumeradores.StatusNFSe.Pendente:
                        return "Pendente";
                    case Enumeradores.StatusNFSe.Rejeicao:
                        return "Rejeição";
                    case Enumeradores.StatusNFSe.AgAprovacaoNFSeManual:
                        return "Ag. Aprovação NFS-e Manual";
                    case Enumeradores.StatusNFSe.AgDadosNFSeManual:
                        return "Ag. Dados NFS-e Manual";
                    case Enumeradores.StatusNFSe.AgGeracaoNFSeManual:
                        return "Ag. Geração NFS-e Manual";
                    case Enumeradores.StatusNFSe.NFSeManualGerada:
                        return "NFS-e Manual Gerada";
                    case Enumeradores.StatusNFSe.AguardandoAutorizacaoRPS:
                        return "Aguardando Autorização RPS";
                    case Enumeradores.StatusNFSe.Inutilizada:
                        return "Inutilizada";
                    default:
                        return string.Empty;
                }
            }
        }

        public virtual string Descricao
        {
            get
            {
                return this.Numero.ToString() + " - " + (this.Serie?.Numero ?? 0).ToString();
            }
        }
    }
}
