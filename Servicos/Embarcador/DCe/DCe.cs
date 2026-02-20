using Dominio.Excecoes.Embarcador;
using Dominio.Interfaces.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Servicos.Embarcador.DCe
{
    public class DCe : ServicoBase
    {
        #region Propiedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;
        private CancellationToken _cancellationToken { get; set; }
        #endregion

        #region Construtores
        
        public DCe(Repositorio.UnitOfWork unitOfWork, CancellationToken cancelationToken = default) : base(unitOfWork, cancelationToken) { }
        
        #endregion

        #region Métodos Públicos

        public bool BuscarDadosDCe(out string erro, out Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal, System.IO.StreamReader xml, Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.DCe.DadosDCe dce = null, bool armazenarXML = true, bool manterCNPJTransportadorXML = false, bool somarProdutosComoPallet = false, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware? tipoServicoMultisoftware = null, bool importarEmailCliente = false, bool importarFreteNota = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null, Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoGeralCarga configuracaoGeralCarga = null, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado = null, bool cadastroAutomaticoPessoaExterior = false)
        {
            xmlNotaFiscal = null;

            Repositorio.Embarcador.Pedidos.XMLNotaFiscal repXmlNotaFiscal = new Repositorio.Embarcador.Pedidos.XMLNotaFiscal(unitOfWork);
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
            Repositorio.Pais repPais = new Repositorio.Pais(unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(unitOfWork);
            Repositorio.Atividade repAtividade = new Repositorio.Atividade(unitOfWork);

            xml.BaseStream.Position = 0;

            if (dce == null)
            {
                xml.BaseStream.Position = 0;

                dce = this.ObterDocumentoPorXML(xml.BaseStream, unitOfWork, true, importarEmailCliente, cargaPedido);
            }

            xml.Dispose();
            xml.Close();

            Dominio.Entidades.Cliente destinatario = null;

            if (!string.IsNullOrWhiteSpace(dce.CNPJCPFDestinatario))
            {
                destinatario = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(dce.CNPJCPFDestinatario)));
                if (destinatario == null)
                {
                    destinatario = new Dominio.Entidades.Cliente();
                    destinatario.Nome = dce.NomeDestinatario;
                    destinatario.Endereco = dce.EnderecoDestinatario;
                    destinatario.Bairro = dce.BairroDestinatario.Left(40);
                    destinatario.Complemento = dce.ComplementoDestinatario.Left(60);
                    destinatario.Numero = dce.NumeroEnderecoDestinatario;
                    destinatario.Cidade = dce.CidadeDestinatario;
                    destinatario.Pais = repPais.BuscarPorSigla(!string.IsNullOrEmpty(dce.PaisDestinatario) ? dce.PaisDestinatario : "01058");
                    destinatario.Tipo = dce.TipoDestinatario;
                    destinatario.CPF_CNPJ = double.Parse(Utilidades.String.OnlyNumbers(dce.CNPJCPFDestinatario));
                    destinatario.Localidade = repLocalidade.BuscarPorCodigoIBGE(!string.IsNullOrEmpty(dce.CodigoCidadeDestinatario) ? int.Parse(dce.CodigoCidadeDestinatario) : 0);
                    destinatario.Atividade = repAtividade.BuscarPorCodigo(1);
                    destinatario.Ativo = true;
                    repCliente.Inserir(destinatario);
                }
            }

            Dominio.Entidades.Cliente emitente = null;

            if (!string.IsNullOrWhiteSpace(dce.CNPJCPFEmitente))
            {
                emitente = repCliente.BuscarPorCPFCNPJ(double.Parse(Utilidades.String.OnlyNumbers(dce.CNPJCPFEmitente)));

                if (emitente == null)
                {
                    emitente = new Dominio.Entidades.Cliente();
                    emitente.Nome = dce.NomeEmitente;
                    emitente.Endereco = dce.EnderecoEmitente;
                    emitente.Bairro = dce.BairroEmitente.Left(40);
                    emitente.Complemento = dce.ComplementoEmitente.Left(60);
                    emitente.Numero = dce.NumeroEnderecoEmitente;
                    emitente.Cidade = dce.CidadeEmitente;
                    emitente.Pais = repPais.BuscarPorSigla(!string.IsNullOrEmpty(dce.PaisEmitente) ? dce.PaisEmitente : "01058");
                    emitente.Tipo = dce.TipoEmitente;
                    emitente.CPF_CNPJ = double.Parse(Utilidades.String.OnlyNumbers(dce.CNPJCPFEmitente));
                    emitente.Localidade = repLocalidade.BuscarPorCodigoIBGE(!string.IsNullOrEmpty(dce.CodigoCidadeEmitente) ? int.Parse(dce.CodigoCidadeEmitente) : 0);
                    emitente.Atividade = repAtividade.BuscarPorCodigo(1);
                    emitente.Ativo = true;
                    repCliente.Inserir(emitente);
                }
            }

            List<string> cfops = new List<string>();
            List<string> ncms = new List<string>();
            int quantidadePallets = 0;

            foreach (var prod in dce.Produtos)
            {
                if (somarProdutosComoPallet)
                    quantidadePallets += (int)prod.Quantidade;

                if (!string.IsNullOrEmpty(prod.NCM) && prod.NCM != "00000000")
                    ncms.Add(prod.NCM);
            }

            string ncmPrincipal = "";

            if (ncms != null && ncms.Count > 0)
            {
                ncmPrincipal = RetornaRegistroComMaiorQuantidade(ncms);

                if (ncmPrincipal.Length > 4)
                    ncmPrincipal = ncmPrincipal.Substring(0, 4);
            }

            if (!string.IsNullOrWhiteSpace(dce.Chave))
                xmlNotaFiscal = repXmlNotaFiscal.BuscarPorChaveTipoDocumento(dce.Chave, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros);

            if (xmlNotaFiscal == null)
                xmlNotaFiscal = new Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal();
            else
                xmlNotaFiscal.NotaJaEstavaNaBase = true;

            if (!string.IsNullOrWhiteSpace(ncmPrincipal))
                xmlNotaFiscal.NCM = ncmPrincipal;

            xmlNotaFiscal.TipoDocumento = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumento.Outros;
            xmlNotaFiscal.XML = "";
            xmlNotaFiscal.Chave = dce.Chave;
            xmlNotaFiscal.TipoEmissao = Utilidades.Chave.ObterTipoEmissao(xmlNotaFiscal.Chave).ToString().ToEnum<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoEmissaoNotaFiscal>();
            xmlNotaFiscal.Numero = int.Parse(dce.Numero);
            xmlNotaFiscal.Modelo = dce.Modelo;
            xmlNotaFiscal.Serie = dce.Serie;
            xmlNotaFiscal.Valor = dce.Produtos?.Sum(x => x.ValorTotal) ?? 0;
           
            xmlNotaFiscal.Peso = (decimal)0.001;
            xmlNotaFiscal.PesoBaseParaCalculo = xmlNotaFiscal.Peso;
            xmlNotaFiscal.NomeDestinatario = Utilidades.String.Left(dce.NomeDestinatario, 150);
            xmlNotaFiscal.IEDestinatario = dce.IEDestinatario ?? "";
            xmlNotaFiscal.DataRecebimento = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(dce.Empresa))
            {
                Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
                xmlNotaFiscal.CNPJTranposrtador = dce.Empresa;
                xmlNotaFiscal.Empresa = repEmpresa.BuscarPorCNPJ(Utilidades.String.OnlyNumbers(dce.Empresa));
            }
            else
            {
                if (!manterCNPJTransportadorXML)
                    xmlNotaFiscal.CNPJTranposrtador = "";
                else
                    xmlNotaFiscal.CNPJTranposrtador = (string)dce.CNPJTransportador;
            }

            if (dce.Produtos?.Count > 1)
                xmlNotaFiscal.Produto = "Diversos";
            else
                xmlNotaFiscal.Produto = dce.Produtos?.FirstOrDefault()?.Descricao?.Left(150) ?? null;

            xmlNotaFiscal.QuantidadePallets = quantidadePallets;

            DateTime dataEmissao;
            if (!DateTime.TryParseExact(dce.DataEmissao, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
            {
                if (!DateTime.TryParseExact(dce.DataEmissao, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dataEmissao))
                    dataEmissao = DateTime.Now;
            }

            xmlNotaFiscal.DataEmissao = dataEmissao;
            xmlNotaFiscal.Descricao = string.Concat(xmlNotaFiscal.Numero, " - ", xmlNotaFiscal.DataEmissao.ToString("dd/MM/yyyy"));

            xmlNotaFiscal.nfAtiva = true;

            xmlNotaFiscal.Emitente = emitente;
            xmlNotaFiscal.Destinatario = destinatario;

            xmlNotaFiscal.PlacaVeiculoNotaFiscal = "";

            this.ValidarDadosDocumento(xmlNotaFiscal);

            erro = string.Empty;
            return true;
        }


        #endregion

        #region Métodos privados

        private void ValidarDadosDocumento(Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal xmlNotaFiscal)
        {
            if (xmlNotaFiscal.Destinatario == null)
                throw new ControllerException("O destinatário não foi informado no pedido, sendo necessário informar na nota.");

            if (xmlNotaFiscal.Emitente == null)
                throw new ControllerException("O emitente não foi informado no pedido, sendo necessário informar na nota.");
        }
        private string RetornaRegistroComMaiorQuantidade(List<string> lista)
        {
            var nameGroup = lista.GroupBy(x => x);
            var maxCount = nameGroup.Max(g => g.Count());
            return nameGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).FirstOrDefault();
        }
        public Dominio.ObjetosDeValor.Embarcador.DCe.DadosDCe ObterDocumentoPorXML(System.IO.Stream xml, Repositorio.UnitOfWork unitOfWork, bool buscarDocumentosAnterior = true, bool importarEmailCliente = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            object dce = Servicos.Embarcador.DCe.Leitura.Ler(xml);

            if (dce == null)
                throw new NullReferenceException("Não foi possível ler o XML do DC-e.");

            if (dce.GetType() == typeof(Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCe))
                return this.ObterDocumentoPorXML((Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCe)dce, unitOfWork, buscarDocumentosAnterior, importarEmailCliente, cargaPedido);
            else
                return null;
        }

        private Dominio.ObjetosDeValor.Embarcador.DCe.DadosDCe ObterDocumentoPorXML(Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCe dce, Repositorio.UnitOfWork unitOfWork = null, bool buscarDocumentosAnterior = true, bool importarEmailCliente = false, Dominio.Entidades.Embarcador.Cargas.CargaPedido cargaPedido = null)
        {
            if (unitOfWork == null)
                unitOfWork = new Repositorio.UnitOfWork(StringConexao);

            System.Globalization.CultureInfo cultura = new System.Globalization.CultureInfo("en-US");

            Repositorio.DocumentosCTE repDocumentosCTe = new Repositorio.DocumentosCTE(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(unitOfWork);

            Dominio.Entidades.Empresa empresa = null;

            string transportadorOrigem = "";
            if (dce.transp != null && dce.transp.CNPJTransp != null && !string.IsNullOrWhiteSpace(dce.transp.CNPJTransp))
            {
                transportadorOrigem = dce.transp.CNPJTransp;
                empresa = repEmpresa.BuscarPorCNPJ(dce.transp.CNPJTransp);
            }

            Dominio.ObjetosDeValor.Embarcador.DCe.DadosDCe notaRetorno = new Dominio.ObjetosDeValor.Embarcador.DCe.DadosDCe
            {
                Chave = dce.Id,
                CNPJCPFEmitente = dce.emit?.Item ?? "",
                NomeEmitente = dce.emit?.xNome ?? "",
                CNPJCPFDestinatario = dce.dest?.Item ?? "",
                NomeDestinatario = dce.dest?.xNome ?? "",
                EnderecoEmitente = dce.emit?.enderEmit?.xLgr ?? "",
                EnderecoDestinatario = dce.dest?.enderDest?.xLgr ?? "",
                BairroEmitente = dce.emit?.enderEmit?.xBairro ?? "",
                BairroDestinatario = dce.dest?.enderDest?.xBairro ?? "",
                CidadeEmitente = dce.emit?.enderEmit?.xMun ?? "",
                CidadeDestinatario = dce.dest?.enderDest?.xMun ?? "",
                CodigoCidadeEmitente = dce.emit?.enderEmit?.cMun ?? "",
                CodigoCidadeDestinatario = dce.dest?.enderDest?.cMun ?? "",
                CEPEmitente = dce.emit?.enderEmit?.CEP ?? "",
                CEPDestinatario = dce.dest?.enderDest?.CEP ?? "",
                ComplementoEmitente = dce.emit?.enderEmit?.xCpl ?? "",
                ComplementoDestinatario = dce.dest?.enderDest?.xCpl ?? "",
                NumeroEnderecoEmitente = dce.emit?.enderEmit?.nro ?? "",
                NumeroEnderecoDestinatario = dce.dest?.enderDest?.nro ?? "",
                PaisEmitente = dce.emit?.enderEmit?.cPais ?? "",
                PaisDestinatario = dce.dest?.enderDest?.cPais ?? "",
                TipoEmitente = dce.emit?.ItemElementName != null ? dce.emit?.ItemElementName == Dominio.ObjetosDeValor.Embarcador.DCe.ItemChoiceType.cnpj ? "J" : dce.emit?.ItemElementName == Dominio.ObjetosDeValor.Embarcador.DCe.ItemChoiceType.cpf ? "F" : "E" : null,
                TipoDestinatario = dce.dest?.ItemElementName != null ? dce.dest?.ItemElementName == Dominio.ObjetosDeValor.Embarcador.DCe.ItemChoiceType1.cnpj ? "J" : dce.dest?.ItemElementName == Dominio.ObjetosDeValor.Embarcador.DCe.ItemChoiceType1.cpf ? "F" : "E" : null,
                ValorTotal = dce.total?.vDC != null ? decimal.Parse(dce.total.vDC, cultura) : 0m,
                DataEmissao = dce.ide?.dhEmi != null ? DateTime.ParseExact(dce.ide.dhEmi.Substring(0, 19), "yyyy-MM-ddTHH:mm:ss", null, System.Globalization.DateTimeStyles.None).ToString("dd/MM/yyyy HH:mm:ss") : "",
                Modelo = dce.ide?.mod.ToString().Replace("Item", "") ?? "",
                Serie = dce.ide?.serie,
                Numero = dce.ide?.nDC,
                CNPJTransportador = transportadorOrigem,
                ModalidadeTransporte = ((int)dce.transp?.modTrans).ToString(),
                Produtos = this.ObterProdutos(dce.det?.ToList()),
                Empresa = empresa != null ? empresa.CNPJ_Formatado : ""
            };


            return notaRetorno;
        }
        private List<Dominio.ObjetosDeValor.Embarcador.DCe.Produto> ObterProdutos(List<Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCeDet> produtos)
        {
            List<Dominio.ObjetosDeValor.Embarcador.DCe.Produto> listaProdutos = new List<Dominio.ObjetosDeValor.Embarcador.DCe.Produto>();

            foreach (var produto in produtos)
            {
                Dominio.ObjetosDeValor.Embarcador.DCe.Produto produtoDCe = new Dominio.ObjetosDeValor.Embarcador.DCe.Produto
                {
                    Quantidade = produto.prod?.qCom != null ? decimal.Parse(produto.prod.qCom, System.Globalization.CultureInfo.InvariantCulture) : 0m,
                    ValorUnitario = produto.prod?.vUnCom != null ? decimal.Parse(produto.prod.vUnCom, System.Globalization.CultureInfo.InvariantCulture) : 0m,
                    ValorTotal = produto.prod?.vProd != null ? decimal.Parse(produto.prod.vProd, System.Globalization.CultureInfo.InvariantCulture) : 0m,
                    NCM = produto.prod?.NCM ?? "",
                    InfAdProd = produto.infAdProd,
                    Descricao = produto.prod?.xProd ?? "",
                };

                listaProdutos.Add(produtoDCe);
            }

            return listaProdutos;
        }


        #endregion
    }
}
