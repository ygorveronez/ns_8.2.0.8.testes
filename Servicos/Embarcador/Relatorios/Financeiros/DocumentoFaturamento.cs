using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.Relatorios.Financeiros
{
    public class DocumentoFaturamento : RelatorioBase<Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento, Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento>
    {
        #region Atributos

        private readonly Repositorio.Embarcador.Financeiro.DocumentoFaturamento _repositorioDocumentoFinanceiro;

        #endregion

        #region Construtores

        public DocumentoFaturamento(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _repositorioDocumentoFinanceiro = new Repositorio.Embarcador.Financeiro.DocumentoFaturamento(_unitOfWork);
        }

        #endregion

        #region Métodos Protegidos Sobrescritos
        // TODO: ToList metodo override
        protected override List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DocumentoFaturamento> ConsultarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            return _repositorioDocumentoFinanceiro.ConsultarRelatorioDocumentoFaturamento(propriedadesAgrupamento, filtrosPesquisa, parametrosConsulta.PropriedadeAgrupar, parametrosConsulta.DirecaoAgrupar, parametrosConsulta.PropriedadeOrdenar, parametrosConsulta.DirecaoOrdenar, parametrosConsulta.InicioRegistros, parametrosConsulta.LimiteRegistros).ToList();
        }

        protected override int ContarRegistros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedadesAgrupamento)
        {
            return _repositorioDocumentoFinanceiro.ContarConsultaRelatorioDocumentoFaturamento(propriedadesAgrupamento, filtrosPesquisa, "", "", "", "", 0, 0);
        }

        protected override string ObterCaminhoPaginaRelatorio()
        {
            return "Relatorios/Financeiros/DocumentoFaturamento";
        }

        protected override List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioDocumentoFaturamento filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            Repositorio.Cliente repCliente = new Repositorio.Cliente(_unitOfWork);
            Repositorio.Localidade repLocalidade = new Repositorio.Localidade(_unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(_unitOfWork);
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoa = new Repositorio.Embarcador.Pessoas.GrupoPessoas(_unitOfWork);
            Repositorio.Estado repEstado = new Repositorio.Estado(_unitOfWork);
            Repositorio.ModeloDocumentoFiscal repModeloDocumentoFiscal = new Repositorio.ModeloDocumentoFiscal(_unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repFilial = new Repositorio.Embarcador.Filiais.Filial(_unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(_unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(_unitOfWork);
            Repositorio.TipoDeOcorrenciaDeCTe reptipoDeOcorrenciaDeCTe = new Repositorio.TipoDeOcorrenciaDeCTe(_unitOfWork);


            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            List<string> gruposPessoas = filtrosPesquisa.GruposPessoas.Count > 0 ? repGrupoPessoa.BuscarDescricaoPorCodigo(filtrosPesquisa.GruposPessoas) : new List<string>();
            List<string> gruposPessoasDiferente = filtrosPesquisa.GruposPessoasDiferente.Count > 0 ? repGrupoPessoa.BuscarDescricaoPorCodigo(filtrosPesquisa.GruposPessoasDiferente) : new List<string>();
            List<Dominio.Entidades.Cliente> tomadores = filtrosPesquisa.CpfCnpjTomador.Count > 0 ? repCliente.BuscarPorCPFCNPJs(filtrosPesquisa.CpfCnpjTomador) : new List<Dominio.Entidades.Cliente>();
            List<string> tipoDeOcorrenciaDeCTe = filtrosPesquisa.TipoOcorrencia.Count > 0 ? reptipoDeOcorrenciaDeCTe.BuscarDescricoesPorCodigos(filtrosPesquisa.TipoOcorrencia) : new List<string>();



            if (filtrosPesquisa.DataInicialEmissao != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", filtrosPesquisa.DataInicialEmissao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicialEmissao", false));

            if (filtrosPesquisa.DataFinalEmissao != DateTime.MinValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", filtrosPesquisa.DataFinalEmissao.ToString("dd/MM/yyyy"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinalEmissao", false));

            if (filtrosPesquisa.CodigoTransportador > 0)
            {
                Dominio.Entidades.Empresa empresa = repEmpresa.BuscarPorCodigo(filtrosPesquisa.CodigoTransportador);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", empresa.CNPJ_Formatado + " - " + empresa.RazaoSocial, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Transportador", false));

            if (filtrosPesquisa.CodigoFilial > 0)
            {
                Dominio.Entidades.Embarcador.Filiais.Filial filial = repFilial.BuscarPorCodigo(filtrosPesquisa.CodigoFilial);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", filial.Descricao, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Filial", false));

            if (filtrosPesquisa.CodigoCarga > 0)
            {
                Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(filtrosPesquisa.CodigoCarga);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", carga.CodigoCargaEmbarcador, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Carga", false));

            if (filtrosPesquisa.CodigoOrigem > 0)
            {
                Dominio.Entidades.Localidade origem = repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoOrigem);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", origem.DescricaoCidadeEstado, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Origem", false));

            if (filtrosPesquisa.CodigoDestino > 0)
            {
                Dominio.Entidades.Localidade destino = repLocalidade.BuscarPorCodigo(filtrosPesquisa.CodigoDestino);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", destino.DescricaoCidadeEstado, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destino", false));

            if (filtrosPesquisa.CpfCnpjRemetente > 0d)
            {
                Dominio.Entidades.Cliente remetente = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjRemetente);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", remetente.CPF_CNPJ_Formatado + " - " + remetente.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Remetente", false));

            if (filtrosPesquisa.CpfCnpjDestinatario > 0d)
            {
                Dominio.Entidades.Cliente destinatario = repCliente.BuscarPorCPFCNPJ(filtrosPesquisa.CpfCnpjDestinatario);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", destinatario.CPF_CNPJ_Formatado + " - " + destinatario.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Destinatario", false));

            if (filtrosPesquisa.CpfCnpjTomador.Count > 0d)
            {
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", tomadores.ToString(), true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Tomador", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GruposPessoas", gruposPessoas));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoOrigem) && filtrosPesquisa.EstadoOrigem != "0")
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(filtrosPesquisa.EstadoOrigem);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", estado.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoOrigem", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.EstadoDestino) && filtrosPesquisa.EstadoDestino != "0")
            {
                Dominio.Entidades.Estado estado = repEstado.BuscarPorSigla(filtrosPesquisa.EstadoDestino);

                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", estado.Nome, true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("EstadoDestino", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoPropriedadeVeiculo))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", filtrosPesquisa.TipoPropriedadeVeiculo == "T" ? "Terceiro" : "Próprio", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("PropriedadeVeiculo", false));

            if (filtrosPesquisa.ModeloDocumento > 0)
            {
                Dominio.Entidades.ModeloDocumentoFiscal modelo = repModeloDocumentoFiscal.BuscarPorId(filtrosPesquisa.ModeloDocumento);
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumento", modelo.Descricao + " (" + modelo.Abreviacao + ")", true));
            }
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloDocumento", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GruposPessoasDiferente", gruposPessoasDiferente));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoFaturamento", filtrosPesquisa.TipoFaturamento != null ? string.Join(", ", from obj in filtrosPesquisa.TipoFaturamento select obj.ObterDescricao()) : null));

            if (filtrosPesquisa.Situacao.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", filtrosPesquisa.Situacao == SituacaoDocumentoFaturamento.Anulado ? "Anulado" : filtrosPesquisa.Situacao == SituacaoDocumentoFaturamento.Cancelado ? "Cancelado" : "Autorizado", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Situacao", false));

            if (filtrosPesquisa.TipoLiquidacao.HasValue)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLiquidacao", filtrosPesquisa.TipoLiquidacao == TipoLiquidacaoRelatorioDocumentoFaturamento.Pendente ? "Pendente" : "Liquidado", true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoLiquidacao", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", filtrosPesquisa.NumeroPedidoCliente, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroPedidoCliente", false));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrenciaCliente))
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaCliente", filtrosPesquisa.NumeroOcorrenciaCliente, true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrenciaCliente", false));

            if (filtrosPesquisa.NumeroOcorrencia > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrencia", filtrosPesquisa.NumeroOcorrencia.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroOcorrencia", false));

            if (filtrosPesquisa.NumeroDocumentoOriginario > 0)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroDocumentoOriginario", filtrosPesquisa.NumeroDocumentoOriginario.ToString(), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroDocumentoOriginario", false));

            if (filtrosPesquisa.ValorInicial > 0m)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorInicial", filtrosPesquisa.ValorInicial.ToString("n2"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorInicial", false));

            if (filtrosPesquisa.ValorFinal > 0m)
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorFinal", filtrosPesquisa.ValorFinal.ToString("n2"), true));
            else
                parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ValorFinal", false));

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("NumeroFatura", filtrosPesquisa.NumeroFatura));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", parametrosConsulta != null ? parametrosConsulta.PropriedadeAgrupar : string.Empty));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Placa));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Numero", filtrosPesquisa.NumeroInicial, filtrosPesquisa.NumeroFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAutorizacao", filtrosPesquisa.DataAutorizacaoInicial, filtrosPesquisa.DataAutorizacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataCancelamento", filtrosPesquisa.DataCancelamentoInicial, filtrosPesquisa.DataCancelamentoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataAnulacao", filtrosPesquisa.DataAnulacaoInicial, filtrosPesquisa.DataAnulacaoFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DocumentoComCanhotosRecebidos", filtrosPesquisa.DocumentoComCanhotosRecebidos));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DocumentoComCanhotosDigitalizados", filtrosPesquisa.DocumentoComCanhotosDigitalizados));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TipoOcorrencia", filtrosPesquisa.TipoOcorrencia.Count>0 ?  string.Join(", ", from obj in tipoDeOcorrenciaDeCTe select obj) : null));

            return parametros;
        }

        protected override string ObterPropriedadeOrdenarOuAgrupar(string propriedadeOrdenarOuAgrupar)
        {
            if (propriedadeOrdenarOuAgrupar == "CPFCNPJTomadorFormatado")
                return "CNPJTomador";
            
            if (propriedadeOrdenarOuAgrupar == "CPFCNPJRemetenteFormatado")
                return "CNPJRemetente";
            
            if (propriedadeOrdenarOuAgrupar == "CPFCNPJExpedidorFormatado")
                return "CNPJExpedidor";
            
            if (propriedadeOrdenarOuAgrupar == "CPFCNPJRecebedorFormatado")
                return "CNPJRecebedor";
            
            if (propriedadeOrdenarOuAgrupar == "CPFCNPJDestinatarioFormatado")
                return "CNPJDestinatario";

            if (propriedadeOrdenarOuAgrupar == "CNPJEmpresaFormatado")
                return "CNPJEmpresa";

            return propriedadeOrdenarOuAgrupar;
        }

        #endregion 
    }
}