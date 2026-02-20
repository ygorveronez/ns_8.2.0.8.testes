using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class Titulo : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo, Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.Titulo _repositorioTitulo;

        #endregion

        #region Construtores

        public Titulo(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.Titulo> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioTitulo.ConsultarRelatorioTitulos(propriedadesAgrupamento, filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, List<PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioTitulo.ContarConsultaRelatorioTitulos(propriedadesAgrupamento, filtrosPesquisa, "", "", "", "", 0, 0);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/Titulo";
        }

        protected override int ObterLimiteRegistrosRelatorio()
        {
            int limiteRegistrosConfigurado = base.ObterLimiteRegistrosRelatorio();
            int limiteRegistrosPadrao = 20000;

            return Math.Min(limiteRegistrosConfigurado, limiteRegistrosPadrao);
        }

        protected override List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioTitulo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Titulo repTitulo = new Repositorio.Embarcador.Financeiro.Titulo(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(_unitOfWork);
            Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS repDocumentoEntradaTMS = new Repositorio.Embarcador.Financeiro.DocumentoEntradaTMS(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Bordero repBordero = new Repositorio.Embarcador.Financeiro.Bordero(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento repTipoPagamentoRecebimento = new Repositorio.Embarcador.Financeiro.TipoPagamentoRecebimento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.PagamentoEletronico repPagamentoEletronico = new Repositorio.Embarcador.Financeiro.PagamentoEletronico(_unitOfWork);
            Repositorio.Embarcador.Financeiro.TipoMovimento repTipoMovimento = new Repositorio.Embarcador.Financeiro.TipoMovimento(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoRemessa repBoletoRemessa = new Repositorio.Embarcador.Financeiro.BoletoRemessa(_unitOfWork);
            Repositorio.Embarcador.Financeiro.Cheque repCheque = new Repositorio.Embarcador.Financeiro.Cheque(_unitOfWork);
            Repositorio.Embarcador.Pessoas.CategoriaPessoa repCategoriaPessoa = new Repositorio.Embarcador.Pessoas.CategoriaPessoa(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo repPagamentoMotoristaTipo = new Repositorio.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo(_unitOfWork);
            Repositorio.Embarcador.Financeiro.BoletoRetornoComando repComandoBanco = new Repositorio.Embarcador.Financeiro.BoletoRetornoComando(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);

            List<Dominio.Entidades.Cliente> clientes = filtrosPesquisa.CnpjPessoas.Count > 0 ? repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPessoas) : new List<Dominio.Entidades.Cliente>();
            Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = filtrosPesquisa.CodigoCTe > 0 ? repCTe.BuscarPorCodigo(filtrosPesquisa.CodigoCTe) : null;
            Dominio.Entidades.Embarcador.Financeiro.Titulo titulo = filtrosPesquisa.CodigoTitulo > 0 ? repTitulo.BuscarPorCodigo(filtrosPesquisa.CodigoTitulo) : null;
            List<Dominio.Entidades.Empresa> empresas = filtrosPesquisa.CodigosEmpresa.Count > 0 ? repEmpresa.BuscarPorCodigos(filtrosPesquisa.CodigosEmpresa) : new List<Dominio.Entidades.Empresa>();
            Dominio.Entidades.Embarcador.Financeiro.BoletoRemessa remessa = filtrosPesquisa.CodigoRemessa > 0 ? repBoletoRemessa.BuscarPorCodigo(filtrosPesquisa.CodigoRemessa) : null;
            Dominio.Entidades.Embarcador.Financeiro.Cheque cheque = filtrosPesquisa.CodigoCheque > 0 ? repCheque.BuscarPorCodigo(filtrosPesquisa.CodigoCheque) : null;
            Dominio.Entidades.Embarcador.Pessoas.CategoriaPessoa categoriaPessoa = filtrosPesquisa.CodigoCategoria > 0 ? repCategoriaPessoa.BuscarPorCodigo(filtrosPesquisa.CodigoCategoria) : null;
            List<Dominio.Entidades.ModeloDocumentoFiscal> modelosDocumentoFiscal = filtrosPesquisa.ModelosDocumento.Count > 0 ? repModeloDocumentoFiscal.BuscarPorCodigos(filtrosPesquisa.ModelosDocumento) : new List<Dominio.Entidades.ModeloDocumentoFiscal>();
            Dominio.Entidades.Embarcador.PagamentoMotorista.PagamentoMotoristaTipo pagamentoMotoristaTipo = filtrosPesquisa.CodigoPagamentoMotoristaTipo > 0 ? repPagamentoMotoristaTipo.BuscarPorCodigo(filtrosPesquisa.CodigoPagamentoMotoristaTipo) : null;
            List<string> gruposPessoas = filtrosPesquisa.GruposPessoas.Count > 0 ? repGrupoPessoas.BuscarDescricaoPorCodigo(filtrosPesquisa.GruposPessoas) : new List<string>();
            Dominio.Entidades.Embarcador.Fatura.Fatura fatura = filtrosPesquisa.CodigoFatura > 0 ? repFatura.BuscarPorCodigo(filtrosPesquisa.CodigoFatura) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;


            parametros.Add(new Parametro("ModeloFatura", filtrosPesquisa.NovoModeloFatura.HasValue ? filtrosPesquisa.NovoModeloFatura.Value ? "Novo" : "Antigo" : string.Empty));
            parametros.Add(new Parametro("Pessoa", clientes.Select(o => o.Descricao)));
            parametros.Add(new Parametro("Tipo", filtrosPesquisa.Tipo?.ObterDescricao()));
            parametros.Add(new Parametro("Status", string.Join(", ", filtrosPesquisa.Status.Select(o => o.ObterDescricao()))));
            parametros.Add(new Parametro("DataEmissao", filtrosPesquisa.DataInicialEmissao, filtrosPesquisa.DataFinalEmissao));
            parametros.Add(new Parametro("DataVencimento", filtrosPesquisa.DataInicialVencimento, filtrosPesquisa.DataFinalVencimento));
            parametros.Add(new Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Parametro("CTe", cte != null ? "(" + cte.Numero + ") " + cte.Chave : string.Empty));
            parametros.Add(new Parametro("Titulo", titulo?.Codigo.ToString()));
            parametros.Add(new Parametro("Empresa", string.Join(", ", empresas.Select(o => o.RazaoSocial))));
            parametros.Add(new Parametro("GrupoPessoa", string.Join(", ", gruposPessoas)));
            parametros.Add(new Parametro("Fatura", fatura?.Numero));
            parametros.Add(new Parametro("DataQuitacao", filtrosPesquisa.DataInicialQuitacao, filtrosPesquisa.DataFinalQuitacao));
            parametros.Add(new Parametro("DataEmissaoDocumentoEntrada", filtrosPesquisa.DataInicialEmissaoDocumentoEntrada, filtrosPesquisa.DataFinalEmissaoDocumentoEntrada));
            parametros.Add(new Parametro("Veiculo", veiculo?.Placa_Formatada));


            if (filtrosPesquisa.CodigoDocumentoEntrada > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.DocumentoEntradaTMS documento = repDocumentoEntradaTMS.BuscarPorCodigo(filtrosPesquisa.CodigoDocumentoEntrada);
                parametros.Add(new Parametro("DocumentoEntrada", documento.Numero.ToString(), true));
            }
            else
                parametros.Add(new Parametro("DocumentoEntrada", false));

            if (filtrosPesquisa.DataPosicaoFinal > DateTime.MinValue)
                parametros.Add(new Parametro("DataPosicao", "Até " + filtrosPesquisa.DataPosicaoFinal.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Parametro("DataPosicao", false));

            parametros.Add(new Parametro("ValorTitulo", filtrosPesquisa.ValorInicial, filtrosPesquisa.ValorFinal));
            parametros.Add(new Parametro("DataCancelamento", filtrosPesquisa.DataInicialCancelamento, filtrosPesquisa.DataFinalCancelamento));
            parametros.Add(new Parametro("DataBase", filtrosPesquisa.DataBaseInicial, filtrosPesquisa.DataBaseFinal));
            parametros.Add(new Parametro("DocumentoOriginal", filtrosPesquisa.DocumentoOriginal));

            if (filtrosPesquisa.CodigoBordero > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.Bordero bordero = repBordero.BuscarPorCodigo(filtrosPesquisa.CodigoBordero);

                parametros.Add(new Parametro("Bordero", bordero.Numero.ToString(), true));
            }
            else
                parametros.Add(new Parametro("Bordero", false));

            if (filtrosPesquisa.Autorizados == OpcaoSimNao.Sim || filtrosPesquisa.Autorizados == OpcaoSimNao.Nao)
                parametros.Add(new Parametro("Autorizados", filtrosPesquisa.Autorizados.ObterDescricao()));
            else
                parametros.Add(new Parametro("Autorizados", false));

            parametros.Add(new Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente));
            parametros.Add(new Parametro("NumeroOcorrenciaCliente", filtrosPesquisa.NumeroOcorrenciaCliente));
            parametros.Add(new Parametro("NumeroOcorrencia", filtrosPesquisa.NumeroOcorrencia));
            parametros.Add(new Parametro("NumeroDocumentoOriginario", filtrosPesquisa.NumeroDocumentoOriginario));
            if (filtrosPesquisa.TipoProposta.Count > 0)
                parametros.Add(new Parametro("TipoProposta", string.Join(", ", filtrosPesquisa.TipoProposta.Select(o => o.ObterDescricao()))));
            else
                parametros.Add(new Parametro("TipoProposta", false));

            if (filtrosPesquisa.Adiantado == 0)
                parametros.Add(new Parametro("Adiantado", "Não", true));
            else if (filtrosPesquisa.Adiantado == 1)
                parametros.Add(new Parametro("Adiantado", "Sim", true));
            else
                parametros.Add(new Parametro("Adiantado", false));

            if (filtrosPesquisa.TipoBoleto != TipoBoletoPesquisaTitulo.Todos)
                parametros.Add(new Parametro("TipoBoleto", filtrosPesquisa.TipoBoleto.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoBoleto", false));

            if (filtrosPesquisa.TipoDocumento.HasValue)
                parametros.Add(new Parametro("TipoDocumento", filtrosPesquisa.TipoDocumento.ObterDescricao(), true));
            else
                parametros.Add(new Parametro("TipoDocumento", false));

            if (filtrosPesquisa.CnpjPortador > 0)
            {
                Dominio.Entidades.Cliente portador = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CnpjPortador);
                parametros.Add(new Parametro("Portador", "(" + portador.CPF_CNPJ_Formatado + ") " + portador.Nome, true));
            }
            else
                parametros.Add(new Parametro("Portador", false));

            if (filtrosPesquisa.FormaTitulo.Count > 0)
                parametros.Add(new Parametro("FormaTitulo", string.Join(", ", filtrosPesquisa.FormaTitulo.Select(o => o.ObterDescricao()))));
            else
                parametros.Add(new Parametro("FormaTitulo", false));

            if (filtrosPesquisa.CodigoTipoOperacao > 0)
            {
                var repositorioTipoOperacao = new Repositorio.Embarcador.Pedidos.TipoOperacao(_unitOfWork);
                var tipoOperacao = repositorioTipoOperacao.BuscarPorCodigo(filtrosPesquisa.CodigoTipoOperacao);

                parametros.Add(new Parametro("TipoOperacao", tipoOperacao.Descricao, true));
            }
            else
                parametros.Add(new Parametro("TipoOperacao", false));

            if (filtrosPesquisa.ProvisaoPesquisaTitulo != ProvisaoPesquisaTitulo.ComProvisao)
            {
                if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SemProvisao)
                    parametros.Add(new Parametro("Provisao", "Sem Provisão", true));
                else if (filtrosPesquisa.ProvisaoPesquisaTitulo == ProvisaoPesquisaTitulo.SomenteProvisao)
                    parametros.Add(new Parametro("Provisao", "Somente Provisão", true));
            }
            else
                parametros.Add(new Parametro("Provisao", false));

            if (filtrosPesquisa.CodigoTipoPagamentoRecebimento > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento tipoPagamentoRecebimento = repTipoPagamentoRecebimento.BuscarPorCodigo(filtrosPesquisa.CodigoTipoPagamentoRecebimento);
                parametros.Add(new Parametro("TipoPagamentoRecebimento", tipoPagamentoRecebimento.Descricao, true));
            }
            else
                parametros.Add(new Parametro("TipoPagamentoRecebimento", false));

            if (filtrosPesquisa.CodigoPagamentoEletronico > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.PagamentoEletronico pagamentoEletronico = repPagamentoEletronico.BuscarPorCodigo(filtrosPesquisa.CodigoPagamentoEletronico);
                parametros.Add(new Parametro("PagamentoEletronico", pagamentoEletronico.Numero.ToString(), true));
            }
            else
                parametros.Add(new Parametro("PagamentoEletronico", false));

            if (filtrosPesquisa.CodigosTipoMovimento.Count > 0)
            {
                List<Dominio.Entidades.Embarcador.Financeiro.TipoMovimento> tipoMovimentos = repTipoMovimento.BuscarPorCodigo(filtrosPesquisa.CodigosTipoMovimento.ToArray());
                parametros.Add(new Parametro("TipoMovimento", string.Join(", ", tipoMovimentos.Select(o => o.Descricao)), true));
            }
            else
                parametros.Add(new Parametro("TipoMovimento", false));

            parametros.Add(new Parametro("Remessa", remessa?.Descricao));
            parametros.Add(new Parametro("Cheque", cheque?.Descricao));
            parametros.Add(new Parametro("DataEntradaDocumentoEntrada", filtrosPesquisa.DataInicialEntradaDocumentoEntrada, filtrosPesquisa.DataFinalEntradaDocumentoEntrada));
            parametros.Add(new Parametro("DataAutorizacao", filtrosPesquisa.DataAutorizacaoInicial, filtrosPesquisa.DataAutorizacaoFinal));
            parametros.Add(new Parametro("DataProgramacao", filtrosPesquisa.DataProgramacaoPagamentoInicial, filtrosPesquisa.DataProgramacaoPagamentoFinal));
            parametros.Add(new Parametro("Categoria", categoriaPessoa?.Descricao));
            parametros.Add(new Parametro("Moeda", filtrosPesquisa.Moeda.ObterDescricao()));
            parametros.Add(new Parametro("ModeloDocumento", string.Join(", ", modelosDocumentoFiscal.Select(o => o.Descricao))));
            parametros.Add(new Parametro("TipoSistema", filtrosPesquisa.TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? "6" : "1", false));
            parametros.Add(new Parametro("PagamentoMotoristaTipo", pagamentoMotoristaTipo?.Descricao));

            if (filtrosPesquisa.CodigoComandoBanco > 0)
            {
                Dominio.Entidades.Embarcador.Financeiro.BoletoRetornoComando ComandoBanco = repComandoBanco.BuscarPorCodigo(filtrosPesquisa.CodigoComandoBanco);

                parametros.Add(new Parametro("ComandoBanco", ComandoBanco.Descricao, true));
            }
            else
                parametros.Add(new Parametro("ComandoBanco", false));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar.Contains("Formatado"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatado", "");

            if (propriedadeOrdenarOuAgrupar.Contains("Formatada"))
                return propriedadeOrdenarOuAgrupar.Replace("Formatada", "");

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}