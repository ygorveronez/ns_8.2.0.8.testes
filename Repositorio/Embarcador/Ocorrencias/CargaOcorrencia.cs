using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Ocorrencias
{
    public class CargaOcorrencia : RepositorioBase<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>
    {
        #region Construtores

        public CargaOcorrencia(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaOcorrencia(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa)
        {
            var consultaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>()
                .Where(obj => obj.Inativa != true);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOcorrenciaCliente))
                consultaOcorrencia = consultaOcorrencia.Where(o => o.NumeroOcorrenciaCliente == filtrosPesquisa.NumeroOcorrenciaCliente);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CnpjTransportador) && string.IsNullOrWhiteSpace(filtrosPesquisa.CnpjTransportadorExterior))
            {
                consultaOcorrencia = consultaOcorrencia.Where(obj =>
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga
                        && (obj.Carga.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador) || obj.Carga.Empresa.CNPJ == filtrosPesquisa.CnpjTransportador)
                    ) ||
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo
                        && (obj.Cargas.Any(c => c.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador) || c.Empresa.CNPJ == filtrosPesquisa.CnpjTransportador))
                    ) ||
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato
                        && (obj.VeiculosContrato.Any(c => c.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador)) || obj.ContratoFrete.Transportador.CNPJ == filtrosPesquisa.CnpjTransportador)
                    ) ||
                    (obj.Usuario.ClienteTerceiro.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador))
                );

                consultaOcorrencia = consultaOcorrencia.Where(obj => (obj.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador == null || obj.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador.Value == false));
            }
            else if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CnpjTransportador) && !string.IsNullOrWhiteSpace(filtrosPesquisa.CnpjTransportadorExterior))
            {
                consultaOcorrencia = consultaOcorrencia.Where(obj =>
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga
                        && (obj.Carga.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador) || obj.Carga.Empresa.CNPJ == filtrosPesquisa.CnpjTransportador || obj.Carga.Empresa.CNPJ == filtrosPesquisa.CnpjTransportadorExterior)
                    ) ||
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo
                        && (obj.Cargas.Any(c => c.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador) || c.Empresa.CNPJ == filtrosPesquisa.CnpjTransportador || obj.Carga.Empresa.CNPJ == filtrosPesquisa.CnpjTransportadorExterior))
                    ) ||
                    (
                        obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato
                        && (obj.VeiculosContrato.Any(c => c.Veiculo.Proprietario.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador)) || obj.ContratoFrete.Transportador.CNPJ == filtrosPesquisa.CnpjTransportador || obj.Carga.Empresa.CNPJ == filtrosPesquisa.CnpjTransportadorExterior)
                    ) ||
                    (obj.Usuario.ClienteTerceiro.CPF_CNPJ == double.Parse(filtrosPesquisa.CnpjTransportador))
                );

                consultaOcorrencia = consultaOcorrencia.Where(obj => (obj.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador == null || obj.TipoOcorrencia.BloquearVisualizacaoTipoOcorrenciaTransportador.Value == false));
            }

            if (filtrosPesquisa.CodigoChamado > 0)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                    .Where(o => o.Chamado.Codigo == filtrosPesquisa.CodigoChamado);

                consultaOcorrencia = consultaOcorrencia.Where(o => consultaChamadoOcorrencia.Any(c => c.CargaOcorrencia.Codigo == o.Codigo));
            }

            if (filtrosPesquisa.Codigo > 0)
            {
                // Codigo é maior que zero quando o filtro ocorre com notificação global
                // Não remover esse filtro
                consultaOcorrencia = consultaOcorrencia.Where(obj => obj.Codigo == filtrosPesquisa.Codigo);
            }
            else
            {
                if (filtrosPesquisa.CodigosEmpresa?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => filtrosPesquisa.CodigosEmpresa.Contains(obj.Carga.Empresa.Codigo) || filtrosPesquisa.CodigosEmpresa.Contains(obj.Emitente.Codigo));

                if (filtrosPesquisa.CodigosFilial.Any(codigo => codigo == -1))
                {

                    var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                    queryCargaPedido = queryCargaPedido.Where(obj => obj.Carga.Pedidos.Any(ped => ped.Recebedor != null && filtrosPesquisa.CodigosRecebedor.Contains(ped.Recebedor.CPF_CNPJ)));
                    consultaOcorrencia = consultaOcorrencia.Where(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Carga.Filial.Codigo) || queryCargaPedido.Any(p => p.Carga == obj.Carga));

                }
                else if (filtrosPesquisa.CodigosFilial?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => filtrosPesquisa.CodigosFilial.Contains(obj.Carga.Filial.Codigo));

                if (filtrosPesquisa.TipoPessoa.HasValue)
                {
                    if (filtrosPesquisa.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.GrupoPessoa && filtrosPesquisa.CodigoGrupoPessoa > 0)
                        consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa));
                    else if (filtrosPesquisa.TipoPessoa == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa.Pessoa && filtrosPesquisa.CpfCnpjPessoa > 0d)
                        consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.Destinatario.CPF_CNPJ == filtrosPesquisa.CpfCnpjPessoa));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoCarga))
                {
                    if (filtrosPesquisa.FiltrarCargasPorParteDoNumero)
                        consultaOcorrencia = consultaOcorrencia.Where(obj => obj.Carga.CodigoCargaEmbarcador.Contains(filtrosPesquisa.CodigoCarga));
                    else
                        consultaOcorrencia = consultaOcorrencia.Where(obj => obj.Carga.CodigoCargaEmbarcador.Equals(filtrosPesquisa.CodigoCarga));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ObservacaoCTe))
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.ObservacaoCTe.Contains(filtrosPesquisa.ObservacaoCTe));

                if (filtrosPesquisa.NumeroOcorrencia > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.NumeroOcorrencia == filtrosPesquisa.NumeroOcorrencia);

                if (filtrosPesquisa.DataInicial.HasValue)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.DataOcorrencia.Date >= filtrosPesquisa.DataInicial.Value.Date);

                if (filtrosPesquisa.DataLimite.HasValue)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.DataOcorrencia.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                if (filtrosPesquisa.Situacoes?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => filtrosPesquisa.Situacoes.Contains(obj.SituacaoOcorrencia));

                if (filtrosPesquisa.CodigoUsuario > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.Usuario.Codigo == filtrosPesquisa.CodigoUsuario || obj.Usuario == null);

                if (filtrosPesquisa.CodigosTipoOcorrencia?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => filtrosPesquisa.CodigosTipoOcorrencia.Contains(obj.TipoOcorrencia.Codigo));

                if (filtrosPesquisa.CodigoCteOrigem > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CargaCTeComplementado.CTe.Numero == filtrosPesquisa.CodigoCteOrigem select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (filtrosPesquisa.NumeroDocumentoOriginario > 0)
                {
                    IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> queryCargaCTeComplementoInfo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();

                    consultaOcorrencia = consultaOcorrencia.Where(o => queryCargaCTeComplementoInfo.Any(cct => cct.CargaOcorrencia.Codigo == o.Codigo && cct.CargaCTeComplementado.CTe.DocumentosOriginarios.Any(doc => doc.Numero == filtrosPesquisa.NumeroDocumentoOriginario)));
                }

                if (filtrosPesquisa.CodigoCteComplementar > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CTe.Numero == filtrosPesquisa.CodigoCteComplementar select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroControle))
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CargaCTeComplementado.CTe.NumeroControle == filtrosPesquisa.NumeroControle select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroBooking))
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CargaCTeComplementado.CTe.NumeroBooking == filtrosPesquisa.NumeroBooking select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroOS))
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CargaCTeComplementado.CTe.NumeroOS == filtrosPesquisa.NumeroOS select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (filtrosPesquisa.CpfCnpjTomadorCTeComplementar > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe
                                                                        where obj.CargaOcorrencia.Codigo == o.Codigo
                                                   &&
                                                   (
                                                   (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && obj.CTe.Remetente.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && obj.CTe.Destinatario.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && obj.CTe.Expedidor.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && obj.CTe.Recebedor.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && obj.CTe.OutrosTomador.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeComplementar)
                                                   )
                                                                        select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe
                                                                        where obj.CargaOcorrencia.Codigo == o.Codigo
                                                   &&
                                                   (
                                                   (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && obj.CTe.Remetente.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && obj.CTe.Destinatario.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && obj.CTe.Expedidor.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && obj.CTe.Recebedor.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && obj.CTe.OutrosTomador.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteComplementar)
                                                   )
                                                                        select obj.CargaOcorrencia.Codigo).Contains(o.Codigo));
                }

                if (filtrosPesquisa.NumeroNFe > 0)
                {
                    var queryCargaOcorrenciaDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

                    queryCargaOcorrenciaDocumento = queryCargaOcorrenciaDocumento.Where(c => c.CargaCTe.CTe.Documentos.Any(nf => nf.Numero.Equals(filtrosPesquisa.NumeroNFe.ToString())) || c.CargaCTe.PreCTe.Documentos.Any(nf => nf.Numero.Equals(filtrosPesquisa.NumeroNFe.ToString())));

                    consultaOcorrencia = consultaOcorrencia.Where(obj => queryCargaOcorrenciaDocumento.Select(o => o.CargaOcorrencia).Contains(obj));
                }

                if (filtrosPesquisa.TipoDocumentoCreditoDebito != Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoCreditoDebito.Todos)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.ModeloDocumentoFiscal.TipoDocumentoCreditoDebito == filtrosPesquisa.TipoDocumentoCreditoDebito);

                if (filtrosPesquisa.OcultarOcorrenciasAutomaticas)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.GeradaPorGatilho == false);

                if (filtrosPesquisa.CodigosFilialVenda?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosFilialVenda.Contains(p.Pedido.FilialVenda.Codigo)));

                if (filtrosPesquisa.CodigosTipoCarga?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => filtrosPesquisa.CodigosTipoCarga.Contains(o.Carga.TipoDeCarga.Codigo));

                if (filtrosPesquisa.CodigosTipoOperacao?.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => filtrosPesquisa.CodigosTipoOperacao.Contains(o.Carga.TipoOperacao.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedido))
                {
                    var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();

                    consultaOcorrencia = consultaOcorrencia.Where(o => queryCargaPedido.Where(a => a.Carga.Codigo == o.Carga.Codigo && filtrosPesquisa.NumeroPedido.Contains(a.Pedido.NumeroPedidoEmbarcador)).Any());
                }

                if (filtrosPesquisa.DataInicialEmissaoDocumento.HasValue || filtrosPesquisa.DataLimiteEmissaoDocumento.HasValue)
                {
                    var consultaCargaCTeComplementoInfo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                        .Where(o => o.CTe != null);

                    if (filtrosPesquisa.DataInicialEmissaoDocumento.HasValue)
                        consultaCargaCTeComplementoInfo = consultaCargaCTeComplementoInfo.Where(o => o.CTe.DataEmissao >= filtrosPesquisa.DataInicialEmissaoDocumento.Value.Date);

                    if (filtrosPesquisa.DataLimiteEmissaoDocumento.HasValue)
                        consultaCargaCTeComplementoInfo = consultaCargaCTeComplementoInfo.Where(o => o.CTe.DataEmissao <= filtrosPesquisa.DataLimiteEmissaoDocumento.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

                    consultaOcorrencia = consultaOcorrencia.Where(ocorrencia => consultaCargaCTeComplementoInfo.Any(documento => documento.CargaOcorrencia.Codigo == ocorrencia.Codigo));
                }

                if (filtrosPesquisa.DataInicialAprovacao.HasValue)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.DataAprovacao >= filtrosPesquisa.DataInicialAprovacao.Value.Date);

                if (filtrosPesquisa.DataLimiteAprovacao.HasValue)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.DataAprovacao <= filtrosPesquisa.DataLimiteAprovacao.Value.Date);

                if (filtrosPesquisa.CodigoLoteAvaria > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.LoteAvaria.Codigo == filtrosPesquisa.CodigoLoteAvaria);

                if (filtrosPesquisa.CodigosCentroResultado.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(cpe => filtrosPesquisa.CodigosCentroResultado.Contains(cpe.Pedido.CentroResultado.Codigo)));

                if (filtrosPesquisa.CodigoMotorista > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Motoristas.Any(m => m.Codigo == filtrosPesquisa.CodigoMotorista));

                if (filtrosPesquisa.CpfCnpjTomadorCTeOriginal > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe
                                                                        where obj.Carga.Codigo == o.Carga.Codigo
                                                   &&
                                                   (
                                                   (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && obj.CTe.Remetente.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && obj.CTe.Destinatario.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && obj.CTe.Expedidor.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && obj.CTe.Recebedor.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && obj.CTe.OutrosTomador.Cliente.CPF_CNPJ == filtrosPesquisa.CpfCnpjTomadorCTeOriginal)
                                                   )
                                                                        select obj.Carga.Codigo).Contains(o.Carga.Codigo));
                }

                if (filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal > 0)
                {
                    var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();
                    consultaOcorrencia = consultaOcorrencia.Where(o => (from obj in queryCTe
                                                                        where obj.Carga.Codigo == o.Carga.Codigo
                                                   &&
                                                   (
                                                   (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Remetente && obj.CTe.Remetente.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Destinatario && obj.CTe.Destinatario.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Expedidor && obj.CTe.Expedidor.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Recebedor && obj.CTe.Recebedor.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal)
                                                   || (obj.CTe.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros && obj.CTe.OutrosTomador.Cliente.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoasTomadorCteOriginal)
                                                   )
                                                                        select obj.Carga.Codigo).Contains(o.Carga.Codigo));
                }

                if (filtrosPesquisa.AguardandoImportacaoCTe.HasValue)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.AgImportacaoCTe == filtrosPesquisa.AguardandoImportacaoCTe.Value);

                if (filtrosPesquisa.CodigoTiposCausadoresOcorrencia > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.TiposCausadoresOcorrencia.Codigo == filtrosPesquisa.CodigoTiposCausadoresOcorrencia);

                if (filtrosPesquisa.CodigoCausasTipoOcorrencia > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.CausasTipoOcorrencia.Codigo == filtrosPesquisa.CodigoCausasTipoOcorrencia);

                if (filtrosPesquisa.CodigosGrupoOcorrencia.Count > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(o => filtrosPesquisa.CodigosGrupoOcorrencia.Contains(o.GrupoOcorrencia.Codigo));

                if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPedidoCliente))
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => p.Pedido.CodigoPedidoCliente == filtrosPesquisa.NumeroPedidoCliente));

                if (filtrosPesquisa.CodigosClienteComplementar.Any())
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosClienteComplementar.Contains((int)p.Pedido.Destinatario.Codigo)));

                if (filtrosPesquisa.CodigosVendedor.Any())
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosVendedor.Contains(p.Pedido.FuncionarioVendedor.Codigo)));

                if (filtrosPesquisa.CodigosSupervisor.Any())
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosSupervisor.Contains(p.Pedido.FuncionarioSupervisor.Codigo)));

                if (filtrosPesquisa.CodigosGerente.Any())
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosGerente.Contains(p.Pedido.FuncionarioGerente.Codigo)));

                if (filtrosPesquisa.CodigosUFDestino.Any())
                    consultaOcorrencia = consultaOcorrencia.Where(o => o.Carga.Pedidos.Any(p => filtrosPesquisa.CodigosUFDestino.Contains(p.Pedido.Destino.Estado.Sigla)));

                if (filtrosPesquisa.NumeroNF > 0)
                    consultaOcorrencia = consultaOcorrencia.Where(obj => obj.Carga.Pedidos.Any(ped => ped.NotasFiscais.Any(nf => nf.XMLNotaFiscal.Numero == filtrosPesquisa.NumeroNF) || ped.CargaPedidoXMLNotasFiscaisParcial.Any(pa => pa.Numero == filtrosPesquisa.NumeroNF)));

                if (filtrosPesquisa.NumeroAtendimento > 0)
                {
                    var queryChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>();
                    consultaOcorrencia = (IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>)consultaOcorrencia.Where(o => (from obj in queryChamadoOcorrencia where obj.CargaOcorrencia.Codigo == o.Codigo && obj.Chamado.Numero == filtrosPesquisa.NumeroAtendimento select obj.CargaOcorrencia.Codigo).Contains(o.Codigo)).AnyAsync();
                }
            }

            return consultaOcorrencia;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarPorCodigoComFetch(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Usuario)
                .ThenFetch(obj => obj.ClienteTerceiro)
                .Fetch(obj => obj.ContratoFrete)
                .ThenFetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.UsuarioResponsavelAprovacao)
                .Fetch(obj => obj.DTNatura)
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCodigoComFetchAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.ModeloDocumentoFiscal)
                .Fetch(obj => obj.Tomador)
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Usuario)
                .ThenFetch(obj => obj.ClienteTerceiro)
                .Fetch(obj => obj.ContratoFrete)
                .ThenFetch(obj => obj.Transportador)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.UsuarioResponsavelAprovacao)
                .Fetch(obj => obj.DTNatura)
                .Fetch(obj => obj.ComponenteFrete)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Filial)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.TipoOcorrencia)
                .FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCodigoAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.TipoOcorrencia)
                .FirstOrDefaultAsync(CancellationToken);
        }
        public Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> BuscarPorCodigoAsync(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result
                .Fetch(obj => obj.TipoOcorrencia)
                .ToListAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarPorNumero(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.NumeroOcorrencia == numero select obj;
            return result.FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorNumeroAsync(int numero)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.NumeroOcorrencia == numero select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarPorCodigoSolicitacaoCredito(int solicitacaoCredito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.SolicitacaoCredito.Codigo == solicitacaoCredito select obj;
            return result.FirstOrDefault();
        }
        public Task<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCodigoSolicitacaoCreditoAsync(int solicitacaoCredito)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.SolicitacaoCredito.Codigo == solicitacaoCredito select obj;
            return result.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasParaFechamento(int codigoContrato, List<int> codigosCargas, DateTime dataInicio, DateTime dataFim, List<int> codigosTipoDeOcorrenciaDeCTe)
        {
            var consultaFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia>()
                .Where(fechamento => fechamento.Fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Cancelado);

            var consultaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>()
                .Where(ocorrencia =>
                    ocorrencia.DataOcorrencia.Date >= dataInicio &&
                    ocorrencia.DataOcorrencia.Date <= dataFim && (
                        ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada ||
                        ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao
                    ) &&
                    (codigosCargas.Contains(ocorrencia.Carga.Codigo) || ocorrencia.ContratoFrete.Codigo == codigoContrato) &&
                    !consultaFechamento.Select(fechamento => fechamento.Ocorrencia.Codigo).Contains(ocorrencia.Codigo)
                );

            if (codigosTipoDeOcorrenciaDeCTe.Count > 0)
                consultaOcorrencia = consultaOcorrencia.Where(ocorrencia => codigosTipoDeOcorrenciaDeCTe.Contains(ocorrencia.TipoOcorrencia.Codigo));

            return consultaOcorrencia.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> BuscarOcorrenciasParaFechamentoAsync(int codigoContrato, List<int> codigosCargas, DateTime dataInicio, DateTime dataFim, List<int> codigosTipoDeOcorrenciaDeCTe)
        {
            var consultaFechamento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Fechamento.FechamentoFreteOcorrencia>()
                .Where(fechamento => fechamento.Fechamento.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFechamentoFrete.Cancelado);

            var consultaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>()
                .Where(ocorrencia =>
                    ocorrencia.DataOcorrencia.Date >= dataInicio &&
                    ocorrencia.DataOcorrencia.Date <= dataFim && (
                        ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada ||
                        ocorrencia.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao
                    ) &&
                    (codigosCargas.Contains(ocorrencia.Carga.Codigo) || ocorrencia.ContratoFrete.Codigo == codigoContrato) &&
                    !consultaFechamento.Select(fechamento => fechamento.Ocorrencia.Codigo).Contains(ocorrencia.Codigo)
                );

            if (codigosTipoDeOcorrenciaDeCTe.Count > 0)
                consultaOcorrencia = consultaOcorrencia.Where(ocorrencia => codigosTipoDeOcorrenciaDeCTe.Contains(ocorrencia.TipoOcorrencia.Codigo));

            return await consultaOcorrencia.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Carga.Codigo == carga || obj.Cargas.Any(c => c.Codigo == carga) select obj;
            return result.ToList();
        }

        public async Task<List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Carga.Codigo == carga || obj.Cargas.Any(c => c.Codigo == carga) select obj;
            return await result.ToListAsync();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCargaEntregaCodigoCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.CargaEntrega.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarOcorrenciaConhecimento(int codigoConhecimento, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = query.Where(o => o.CTeImportado.Codigo == codigoConhecimento || o.CargaCTe.CTe.Codigo == codigoConhecimento);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.Select(o => o.CargaOcorrencia).ToList();
        }

        public int ContarConsultarOcorrenciaConhecimento(int codigoConhecimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var result = query.Where(o => o.CTeImportado.Codigo == codigoConhecimento || o.CargaCTe.CTe.Codigo == codigoConhecimento);

            return result.Select(o => o.CargaOcorrencia).Count();
        }

        public bool ContemOcorrenciaCargaCTeTipoOcorrencia(int cargaCTe, int tipoDeOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from o in query where o.CargaCTe.Codigo == cargaCTe && o.CargaOcorrencia.TipoOcorrencia.Codigo == tipoDeOcorrencia select o.CargaOcorrencia;

            return result.Count() > 0;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCargaCTeEMotivoChamado(int cargaCTe, int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from o in query where o.CargaCTe.Codigo == cargaCTe select o.CargaOcorrencia;

            if (codigoMotivoChamado > 0)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                    .Where(o => o.Chamado.MotivoChamado.Codigo == codigoMotivoChamado);

                result = result.Where(o => consultaChamadoOcorrencia.Any(c => c.CargaOcorrencia.Codigo == o.Codigo));
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorDocParaNFSManualEMotivoChamado(int codigoCocParaNFSManual, int codigoMotivoChamado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaDocumentoParaEmissaoNFSManual>();
            var result = from o in query where o.Codigo == codigoCocParaNFSManual && o.CargaOcorrencia != null select o.CargaOcorrencia;

            if (codigoMotivoChamado > 0)
            {
                var consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                    .Where(o => o.Chamado.MotivoChamado.Codigo == codigoMotivoChamado);

                result = result.Where(o => consultaChamadoOcorrencia.Any(c => c.CargaOcorrencia.Codigo == o.Codigo));
            }

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaCTe> BuscarCargaCTe(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaOcorrencia.Codigo == codigoOcorrencia select obj;

            return result.Select(o => o.CargaCTe).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCTe(int cargaCte)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();
            var result = from obj in query where obj.CargaCTe.Codigo == cargaCte select obj;

            return result.Select(obj => obj.CargaOcorrencia).Distinct().ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciaCanhoto(int codigoNFX, int codigoCte, int carga)
        {
            var queryDocumento = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            var resultTudo = from obj in queryDocumento
                             where obj.XMLNotaFiscais.Any(nf => nf.Codigo == codigoNFX)
                                && obj.CargaCTe.CTe.Codigo == codigoCte
                             select obj.CargaOcorrencia;

            var listaOcorrencias = resultTudo.Distinct().ToList();
            if (listaOcorrencias.Any())
                return listaOcorrencias;

            var resultNFX = from obj in queryDocumento
                            where obj.XMLNotaFiscais.Any(nf => nf.Codigo == codigoNFX)
                            select obj.CargaOcorrencia;

            var listaOcorrenciasNFX = resultNFX.Distinct().ToList();
            if (listaOcorrenciasNFX.Any())
                return listaOcorrenciasNFX;

            var resultCargaCTe = from obj in queryDocumento
                                 where obj.CargaCTe.CTe.Codigo == codigoCte
                                 select obj.CargaOcorrencia;

            var listaOcorrenciasCargaCTe = resultCargaCTe.Distinct().ToList();
            if (listaOcorrenciasCargaCTe.Any())
                return listaOcorrenciasCargaCTe;

            var queryOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var resultCarga = from obj in queryOcorrencia
                              where obj.Carga.Codigo == carga || obj.Cargas.Any(c => c.Codigo == carga)
                              select obj;

            var listaOcorrenciasCarga = resultCarga.ToList();
            if (listaOcorrenciasCarga.Any())
                return listaOcorrenciasCarga;

            return new List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCargaESituacao(int carga, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia situacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.SituacaoOcorrencia == situacaoOcorrencia select obj;
            return result.ToList();
        }

        public int BuscarProximoCodigo()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            int? retorno = query.Max(o => (int?)o.NumeroOcorrencia);

            return retorno.HasValue ? retorno.Value + 1 : 1;
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasParaComissaoMotorista(Dominio.Entidades.Usuario motorista, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query where obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada && obj.ValorOcorrenciaLiquida > 0 select obj;

            if (motorista != null)
                result = result.Where(obj => obj.Carga.Motoristas.Contains(motorista));

            result = result.Where(obj => obj.DataOcorrencia <= dataFinal.AddDays(1) && obj.DataOcorrencia >= dataInicial);

            return result.ToList();

        }

        public int ContarCargaPreCTePorNaoEnviados(int codigoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTe>();

            var result = from obj in query where obj.CargaCTeComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia && obj.PreCTe != null && obj.CTe == null select obj.Codigo;

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia situacaoOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query select obj;

            if (situacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Todas)
                result = result.Where(obj => obj.SituacaoOcorrencia == situacaoOcorrencia);

            return result.ToList();

        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasPorSituacao(Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia situacaoOcorrencia, bool gerandoIntegracoes, int quantidadeRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query where obj.GerandoIntegracoes == gerandoIntegracoes select obj;

            if (situacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Todas)
                result = result.Where(obj => obj.SituacaoOcorrencia == situacaoOcorrencia);

            return result.Take(quantidadeRegistros).ToList();

        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia> BuscarDadosParaRateio(int ocorrencia)
        {
            string sql = @"SELECT
	                            DEST.PCT_CPF_CNPJ CNPJ,
                                CLI.CLI_CODIGO_INTEGRACAO CentroCusto,
                                SUM(CON_VALOR_TOTAL_MERC) ValorMercadoria
                            FROM 
	                            T_CARGA_OCORRENCIA_CARGAS CA
                            JOIN 
	                            T_CARGA_CTE CT ON CA.CAR_CODIGO = CT.CAR_CODIGO AND CCC_CODIGO IS NULL
                            JOIN 
	                            T_CTE CTE ON CT.CON_CODIGO = CTE.CON_CODIGO
                            JOIN 
	                            T_CTE_PARTICIPANTE DEST ON CTE.CON_DESTINATARIO_CTE = DEST.PCT_CODIGO
                            JOIN 
	                            T_CLIENTE CLI ON DEST.CLI_CODIGO = CLI.CLI_CGCCPF
                            WHERE 
	                            COC_CODIGO = " + ocorrencia.ToString() + @"
                            GROUP BY
	                            DEST.PCT_CPF_CNPJ, CLI.CLI_CODIGO_INTEGRACAO";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>();
        }

        public async Task<List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>> BuscarDadosParaRateioAsync(int ocorrencia)
        {
            string sql = @"SELECT
	                            DEST.PCT_CPF_CNPJ CNPJ,
                                CLI.CLI_CODIGO_INTEGRACAO CentroCusto,
                                SUM(CON_VALOR_TOTAL_MERC) ValorMercadoria
                            FROM 
	                            T_CARGA_OCORRENCIA_CARGAS CA
                            JOIN 
	                            T_CARGA_CTE CT ON CA.CAR_CODIGO = CT.CAR_CODIGO AND CCC_CODIGO IS NULL
                            JOIN 
	                            T_CTE CTE ON CT.CON_CODIGO = CTE.CON_CODIGO
                            JOIN 
	                            T_CTE_PARTICIPANTE DEST ON CTE.CON_DESTINATARIO_CTE = DEST.PCT_CODIGO
                            JOIN 
	                            T_CLIENTE CLI ON DEST.CLI_CODIGO = CLI.CLI_CGCCPF
                            WHERE 
	                            COC_CODIGO = " + ocorrencia.ToString() + @"
                            GROUP BY
	                            DEST.PCT_CPF_CNPJ, CLI.CLI_CODIGO_INTEGRACAO";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia)));

            return (List<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>)
                await query.ListAsync<Dominio.ObjetosDeValor.Embarcador.Carga.RateioOcorrencia>();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasEmEmissaoSituacao(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && !obj.AgImportacaoCTe && obj.ValorOcorrencia == 0);

            return result.Take(limite).OrderBy(o => o.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasEmEmissaoSituacaoDocumentosNaoGeradosNaoAptosParaAvancarEtapaOcorrencia(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && !obj.AgImportacaoCTe && !obj.GerouTodosDocumentos && obj.ValorOcorrencia > 0);
            return result.Take(limite).ToList();
        }

        public List<int> BuscarCodigosOcorrenciasEmEmissaoSituacaoDocumentosNaoGeradosNaoAptosParaAvancarEtapaOcorrencia(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && !obj.AgImportacaoCTe && !obj.GerouTodosDocumentos && obj.ValorOcorrencia > 0);
            return result.Select(x=>x.Codigo).Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasEmEmissaoSituacaoDocumentosGeradosAptosParaAvancarEtapaOcorrencia(int limite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;
            result = result.Where(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar && !obj.AgImportacaoCTe && obj.GerouTodosDocumentos && obj.ValorOcorrencia > 0);
            return result.Take(limite).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> Consultar(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOcorrencia = Consultar(filtrosPesquisa);

            consultaOcorrencia = consultaOcorrencia
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.Empresa)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.Carga)
                .ThenFetch(obj => obj.DadosSumarizados)
                .Fetch(obj => obj.Emitente)
                .ThenFetch(obj => obj.Localidade)
                .Fetch(obj => obj.TipoOcorrencia)
                .Fetch(obj => obj.ComponenteFrete)
                .WithOptions(o => o.SetTimeout(600));

            return ObterLista(consultaOcorrencia, parametrosConsulta);
        }

        public List<int> ConsultarCodigosOcorrencia(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa)
        {
            var consultaOcorrencia = Consultar(filtrosPesquisa);

            return consultaOcorrencia.Select(o => o.Codigo).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa)
        {
            var consultaOcorrencia = Consultar(filtrosPesquisa);

            return consultaOcorrencia.Count();
        }

        public List<int> ObterCodigosOcorrenciasSelecionadas(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaOcorrencia filtrosPesquisa, bool selecionarTodos, List<int> codigosOcorrencias)
        {
            var consultaOcorrencia = Consultar(filtrosPesquisa);

            if (selecionarTodos)
                consultaOcorrencia = consultaOcorrencia.Where(o => !codigosOcorrencias.Contains(o.Codigo));
            else
                consultaOcorrencia = consultaOcorrencia.Where(o => codigosOcorrencias.Contains(o.Codigo));

            return consultaOcorrencia.Select(o => o.Codigo).ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.Ocorrencia.Ocorrencia> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrencia().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaFilaCarregamentoHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.Ocorrencia.Ocorrencia)));

            return consultaFilaCarregamentoHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.Ocorrencia.Ocorrencia>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega> ConsultarRelatorioEntrega(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrenciaEntrega().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaFilaCarregamentoHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega)));

            return consultaFilaCarregamentoHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaEntrega>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto> ConsultarRelatorioPorCentroCusto(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrenciaCentroCusto().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consultaFilaCarregamentoHistorico.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto)));

            return consultaFilaCarregamentoHistorico.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.OcorrenciaCentroCusto>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrencia filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrencia().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaFilaCarregamentoHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorioEntrega(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaEntrega filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrenciaEntrega().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaFilaCarregamentoHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public int ContarConsultaRelatorioPorCentroCusto(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioOcorrenciaCentroCusto filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consultaFilaCarregamentoHistorico = new ConsultaOcorrenciaCentroCusto().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consultaFilaCarregamentoHistorico.SetTimeout(600).UniqueResult<int>();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ConsultarOcorrenciaSemAcerto(DateTime dataInicio, DateTime dataFim, Dominio.Entidades.Usuario motorista, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query select obj;

            //if (veiculo > 0)
            //    result = result.Where(obj => (obj.Carga.Veiculo.Codigo == veiculo));

            if (motorista != null)
                result = result.Where(obj => (obj.Carga.Motoristas.Contains(motorista)));

            if (dataInicio != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(obj => obj.DataOcorrencia.Date <= dataFim);

            var queryAcerto = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Acerto.AcertoOcorrencia>();
            var resultAcerto = from obj in queryAcerto where obj.AcertoViagem.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAcertoViagem.Cancelado select obj;

            result = result.Where(obj => !resultAcerto.Select(a => a.CargaOcorrencia).Contains(obj));

            return result.ToList();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia ExisteOcorrenciaPorPeriodoETipoVeiculo(DateTime periodoInico, DateTime periodoFim, int tipoOcorrencia, int usuario, int codigoFilial, List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacos, int codigoVeiculo, int codigoEmitente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query
                         where
                         obj.PeriodoInicio == periodoInico
                         && obj.PeriodoFim == periodoFim
                         && obj.TipoOcorrencia.Codigo == tipoOcorrencia
                         && obj.Usuario.Codigo == usuario
                         && !situacos.Contains(obj.SituacaoOcorrencia)
                         select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoEmitente > 0)
                result = result.Where(obj => obj.Emitente.Codigo == codigoEmitente);

            if (codigoFilial > 0)
                result = result.Where(obj => obj.Filial.Codigo == codigoFilial);

            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasComplementoValorFreteCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            query = query.Where(o =>
                                    o.ComplementoValorFreteCarga &&
                                    o.Carga.Codigo == codigoCarga &&
                                    (o.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.FalhaIntegracao ||
                                     o.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada ||
                                     o.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgIntegracao));

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarParaIntegracaoGPA()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query
                         where
                            obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.EmEmissaoCTeComplementar
                            && obj.AgImportacaoCTe == true
                            && obj.IntegradoComGPA == false
                            && obj.ErroIntegracaoComGPA == false
                            && (((obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorPeriodo || obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorContrato) && obj.Emitente.EmiteNFSeOcorrenciaForaEmbarcador && obj.Emitente.EndpointIntegracaoGPA != null && obj.Emitente.EndpointIntegracaoGPA != "")
                                ||
                                (obj.OrigemOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.OrigemOcorrencia.PorCarga && obj.Carga.Empresa.EmiteNFSeOcorrenciaForaEmbarcador && obj.Carga.Empresa.EndpointIntegracaoGPA != null && obj.Carga.Empresa.EndpointIntegracaoGPA != ""))

                         select obj;
            return result.ToList();
        }

        public bool ExisteOcorrenciaInvalidaParaCancelamento(int codigoCarga)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoesPermitidas = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>()
            {
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.RejeitadaEtapaEmissao,
                 Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Rejeitada
            };

            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            query = query.Where(o => o.Carga.Codigo == codigoCarga &&
                                     !situacoesPermitidas.Contains(o.SituacaoOcorrencia)
                                     && !o.TipoOcorrencia.TipoOcorrenciaControleEntrega
                                     && o.ValorOcorrencia != 0);

            return query.Any();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento> ConsultarOcorrenciasDocumento(double codigoRecebedor, string numeroPedidoCliente, string numeroSolicitacao,
            string numeroNota, int serieNota, int numeroCTe, int serieCTe,
            int numeroPedido, double codigoCliente, double codigoRemetente, double codigoDestinatario, int inicio, int limite, string numeroPedidoNoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            query = query.Where(o => o.CargaCTe != null);

            if (codigoCliente > 0)
                query = query.Where(o => o.CargaCTe.CTe.XMLNotaFiscais.Any(nf => nf.Destinatario.CPF_CNPJ == codigoCliente || nf.Recebedor.CPF_CNPJ == codigoCliente || nf.Emitente.CPF_CNPJ == codigoCliente));

            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNota)));
            if (serieNota > 0)
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTe > 0)
                query = query.Where(o => o.CargaCTe.CTe.Numero == numeroCTe);
            if (serieCTe > 0)
                query = query.Where(o => o.CargaCTe.CTe.Serie.Numero == serieCTe);


            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.CargaCTe.CTe.XMLNotaFiscais.Any(nf => nf.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCliente))
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCliente));

            if (numeroPedido > 0)
                query = query.Where(o => o.CargaCTe.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (codigoRemetente > 0)
                query = query.Where(o => o.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CargaCTe.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            if (!string.IsNullOrWhiteSpace(numeroPedidoNoCliente))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                queryCargaPedido = queryCargaPedido.Where(c => c.Pedido.CodigoPedidoCliente == numeroPedidoNoCliente);

                query = query.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.CargaCTe.Carga.Codigo));
            }

            return query
                .Fetch(o => o.CargaOcorrencia)
                .Fetch(o => o.OcorrenciaDeCTe)
                .Skip(inicio).Take(limite)
                .ToList();
        }

        public int ContarOcorrenciasDocumento(double codigoRecebedor, string numeroPedidoCliente, string numeroSolicitacao,
            string numeroNota, int serieNota, int numeroCTe, int serieCTe,
            int numeroPedido, double codigoCliente, double codigoRemetente, double codigoDestinatario, string numeroPedidoNoCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrenciaDocumento>();

            query = query.Where(o => o.CargaCTe != null);

            if (codigoCliente > 0)
                query = query.Where(o => o.CargaCTe.CTe.XMLNotaFiscais.Any(nf => nf.Destinatario.CPF_CNPJ == codigoCliente || nf.Recebedor.CPF_CNPJ == codigoCliente || nf.Emitente.CPF_CNPJ == codigoCliente));

            if (!string.IsNullOrWhiteSpace(numeroNota))
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(nf => nf.Numero.Equals(numeroNota)));
            if (serieNota > 0)
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(nf => nf.Serie.Equals(serieNota.ToString())));

            if (numeroCTe > 0)
                query = query.Where(o => o.CargaCTe.CTe.Numero == numeroCTe);
            if (serieCTe > 0)
                query = query.Where(o => o.CargaCTe.CTe.Serie.Numero == serieCTe);


            if (!string.IsNullOrWhiteSpace(numeroSolicitacao))
                query = query.Where(o => o.CargaCTe.CTe.XMLNotaFiscais.Any(nf => nf.NumeroSolicitacao == numeroSolicitacao));

            if (!string.IsNullOrWhiteSpace(numeroPedidoCliente))
                query = query.Where(o => o.CargaCTe.CTe.Documentos.Any(r => r.NumeroPedido == numeroPedidoCliente));

            if (numeroPedido > 0)
                query = query.Where(o => o.CargaCTe.Carga.Pedidos.Any(ped => ped.Pedido.Numero == numeroPedido));

            if (codigoRemetente > 0)
                query = query.Where(o => o.CargaCTe.CTe.Remetente.Cliente.CPF_CNPJ == codigoRemetente);
            if (codigoDestinatario > 0)
                query = query.Where(o => o.CargaCTe.CTe.Destinatario.Cliente.CPF_CNPJ == codigoDestinatario);
            if (codigoRecebedor > 0)
                query = query.Where(o => o.CargaCTe.CTe.Recebedor.Cliente.CPF_CNPJ == codigoRecebedor);

            if (!string.IsNullOrWhiteSpace(numeroPedidoNoCliente))
            {
                var queryCargaPedido = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedido>();
                queryCargaPedido = queryCargaPedido.Where(c => c.Pedido.CodigoPedidoCliente == numeroPedidoNoCliente);

                query = query.Where(o => queryCargaPedido.Any(c => c.Carga.Codigo == o.CargaCTe.Carga.Codigo));
            }

            return query.Count();
        }

        public bool ExisteOcorrenciaValidaPorCTeTerceiro(int codigoCTeTerceiro)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia> situacoes = new List<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia>() {
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada,
                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Anulada
            };

            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            query = query.Where(o => o.CTeTerceiro.Codigo == codigoCTeTerceiro && !situacoes.Contains(o.SituacaoOcorrencia));

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasParaAprovacaoAutomatica()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAprovacao || obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.AgAutorizacaoEmissao);
            result = result.Where(obj => obj.TipoOcorrencia.DiasAprovacaoAutomatica > 0);

            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarNaoIntegradasPorTransportador(List<int> codigosTransportadores)
        {

            var queryCTe = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>();
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            query = query.Where(o => o.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada &&
                                codigosTransportadores.Contains(o.Carga.Empresa.Codigo) &&
                                !o.IntegrouTransportador && (from obj in queryCTe where obj.CargaOcorrencia.Codigo == o.Codigo && obj.CargaCTeComplementado.CTe != null select obj.CargaOcorrencia).Contains(o));

            return query.ToList();

        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarOcorrenciaPorProtocoloETransportador(int protocoloOcorrencia, List<int> codigosTransportadores)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            query = query.Where(o => o.Codigo == protocoloOcorrencia &&
                                     codigosTransportadores.Contains(o.Carga.Empresa.Codigo));

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarOcorrenciaPorProtocolo(int protocoloOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            query = query.Where(o => o.Protocolo == protocoloOcorrencia);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarProvisionadasPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where (obj.Carga.Codigo == carga || obj.Cargas.Any(c => c.Codigo == carga)) && obj.TipoOcorrencia.OcorrenciaProvisionada select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntrega select obj;
            return result.ToList();
        }

        public List<(int, int)> BuscarPorCodigoCargas(List<int> codigoCargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where codigoCargas.Contains(obj.Carga.Codigo) select obj;
            return result.Select(x => ValueTuple.Create(x.Codigo, x.Carga.Codigo)).ToList();
        }

        public void DefinirSituacaoOcorrenciaPorCodigo(int codigoOcorrencia, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia situacaoOcorrencia)
        {
            string sql = @"
                UPDATE T_CARGA_OCORRENCIA 
                   SET COC_SITUACAO_OCORRENCIA = :situacaoOcorrencia
                 WHERE COC_CODIGO = :codigoOcorrencia";

            var query = this.SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameter("codigoOcorrencia", codigoOcorrencia)
                .SetParameter("situacaoOcorrencia", situacaoOcorrencia);

            query.ExecuteUpdate();
        }

        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarOcorrenciasDeEstadiasPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.Carga.Codigo == carga && obj.OcorrenciaDeEstadia && obj.SituacaoOcorrencia != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Cancelada select obj;
            return result.ToList();
        }


        public List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> BuscarRegistrosPendentesIntegracao(int quantidade, bool apenasOcorrenciasFinalizadas)
        {
            int totalRegistros = quantidade > 0 ? quantidade : 50;

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query
                         where !obj.IntegradoERP
                            && (!apenasOcorrenciasFinalizadas || obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada)
                         select obj;

            return result.Take(totalRegistros).ToList();
        }

        public int ContarRegistroPendenteIntegracao(bool apenasOcorrenciasFinalizadas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query
                         where !obj.IntegradoERP
                            && (!apenasOcorrenciasFinalizadas || obj.SituacaoOcorrencia == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoOcorrencia.Finalizada)
                         select obj.Codigo;

            return result.Count();
        }

        public bool ContemOcorrenciaCargaTipoOcorrencia(int carga, int tipoDeOcorrencia)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from o in query where o.Carga.Codigo == carga && o.TipoOcorrencia.Codigo == tipoDeOcorrencia select o;

            return result.Count() > 0;
        }

        public Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia BuscarOcorrenciaPorCodigoGestaoDevolucao(long codigoGestaoDevolucao)
        {

            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            query = query.Where(obj => obj.GestaoDevolucao.Codigo == codigoGestaoDevolucao);


            return query.FirstOrDefault();
        }

        public List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia> ObterModeloDadosOcorrencias(DateTime dataInicioCriacao, DateTime dataFimCriacao, int numeroOcorrencia)
        {
            int limiteRegistros = 1000;
            IQueryable<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> consultaCargaOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();

            if (dataInicioCriacao != DateTime.MinValue)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(cargaOcorrencia => cargaOcorrencia.DataOcorrencia >= dataInicioCriacao);

            if (dataFimCriacao != DateTime.MinValue)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(cargaOcorrencia => cargaOcorrencia.DataOcorrencia <= dataFimCriacao);

            if (numeroOcorrencia > 0)
                consultaCargaOcorrencia = consultaCargaOcorrencia.Where(cargaOcorrencia => cargaOcorrencia.NumeroOcorrencia == numeroOcorrencia);

            List<Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia> cargaOcorrencias = consultaCargaOcorrencia
                .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                .Select(cargaOcorrencia => new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia()
                {
                    Protocolo = cargaOcorrencia.Codigo,
                    DataCriacao = cargaOcorrencia.DataOcorrencia,
                    NumeroOcorrencia = cargaOcorrencia.NumeroOcorrencia,
                    Valor = cargaOcorrencia.ValorOcorrencia,
                    ProtocoloCarga = (cargaOcorrencia.Carga == null) ? 0 : cargaOcorrencia.Carga.Protocolo,
                    Tipo = (cargaOcorrencia.TipoOcorrencia == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.TipoOcorrencia()
                    {
                        CodigoIntegracao = cargaOcorrencia.TipoOcorrencia.CodigoIntegracao,
                        Descricao = cargaOcorrencia.TipoOcorrencia.Descricao
                    }
                })
                .ToList();

            if (cargaOcorrencias.Count > 0)
            {
                List<(int ProtocoloOcorrencia, int ProtocoloChamado)> chamadoOcorrencias = new List<(int ProtocoloOcorrencia, int ProtocoloChamado)>();
                List<(int ProtocoloOcorrencia, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico Cte, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico CteComplemento)> dadosCtesComplemento = new List<(int ProtocoloOcorrencia, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico Cte, Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico CteComplemento)>();

                for (int registroInicial = 0; registroInicial < cargaOcorrencias.Count; registroInicial += limiteRegistros)
                {
                    List<int> protocolosCargaOcorrencias = cargaOcorrencias.Select(cargaOcorrencia => cargaOcorrencia.Protocolo).Skip(registroInicial).Take(limiteRegistros).ToList();

                    IQueryable<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia> consultaChamadoOcorrencia = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.ChamadoOcorrencia>()
                        .Where(chamadoOcorrencia => protocolosCargaOcorrencias.Contains(chamadoOcorrencia.CargaOcorrencia.Codigo));

                    chamadoOcorrencias.AddRange(consultaChamadoOcorrencia
                       .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(chamadoOcorrencia => ValueTuple.Create(
                            chamadoOcorrencia.CargaOcorrencia.Codigo,
                            chamadoOcorrencia.Chamado.Codigo
                        ))
                        .ToList()
                    );

                    IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> consultaCargaCTeComplementoInfo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                        .Where(cteComplemento => protocolosCargaOcorrencias.Contains(cteComplemento.CargaOcorrencia.Codigo) && cteComplemento.CargaCTeComplementado != null);

                    dadosCtesComplemento.AddRange(consultaCargaCTeComplementoInfo
                       .WithOptions(opcoes => { opcoes.SetTimeout(600); })
                        .Select(cteComplemento => ValueTuple.Create(
                            cteComplemento.CargaOcorrencia.Codigo,
                            new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico()
                            {
                                Chave = cteComplemento.CargaCTeComplementado.CTe.Chave,
                                Numero = cteComplemento.CargaCTeComplementado.CTe.Numero,
                                DataEmissao = cteComplemento.CargaCTeComplementado.CTe.DataEmissao,
                                DataAutorizacao = cteComplemento.CargaCTeComplementado.CTe.DataAutorizacao
                            },
                            (cteComplemento.CTe == null) ? null : new Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.ConhecimentoTransporteEletronico()
                            {
                                Chave = cteComplemento.CTe.Chave,
                                Numero = cteComplemento.CTe.Numero,
                                DataEmissao = cteComplemento.CTe.DataEmissao,
                                DataAutorizacao = cteComplemento.CTe.DataAutorizacao
                            }
                        ))
                        .ToList()
                    );
                }

                foreach (Dominio.ObjetosDeValor.WebService.Rest.ModeloDados.Ocorrencia cargaOcorrencia in cargaOcorrencias)
                {
                    cargaOcorrencia.ProtocolosChamados = chamadoOcorrencias
                        .Where(chamadoOcorrencia => chamadoOcorrencia.ProtocoloOcorrencia == cargaOcorrencia.Protocolo)
                        .Select(chamadoOcorrencia => chamadoOcorrencia.ProtocoloChamado)
                        .ToList();

                    cargaOcorrencia.Ctes = dadosCtesComplemento
                        .Where(cteComplemento => cteComplemento.ProtocoloOcorrencia == cargaOcorrencia.Protocolo)
                        .Select(cteComplemento => cteComplemento.Cte)
                        .ToList();

                    cargaOcorrencia.CtesComplemento = dadosCtesComplemento
                        .Where(cteComplemento => (cteComplemento.ProtocoloOcorrencia == cargaOcorrencia.Protocolo) && (cteComplemento.CteComplemento != null))
                        .Select(cteComplemento => cteComplemento.CteComplemento)
                        .ToList();
                }
            }

            return cargaOcorrencias;
        }

        public bool ExisteOcorrenciaParaTipoOcorrenciaECarga(int codigoTipoOcorrencia, int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia>();
            var result = from obj in query where obj.TipoOcorrencia.Codigo == codigoTipoOcorrencia && obj.Carga.Codigo == codigoCarga select obj;
            return result.Any();
        }

        public void AtualizarPagamento(int codigoPagamento, AutorizacaoOcorrenciaPagamento autorizacaoPagamento)
        {
            string sql = $"UPDATE T_CARGA_OCORRENCIA SET COC_PAGAMENTO = :autorizacaoPagamento WHERE COC_CODIGO = :codigo ";
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);
            query.SetParameter("codigo", codigoPagamento)
                 .SetEnum("autorizacaoPagamento", autorizacaoPagamento);
            query.ExecuteUpdate();
        }

        public List<Dominio.ObjetosDeValor.Embarcador.Integracao.RetornoOcorrenciaCargaEnvioProgramado> BuscarOcorrenciasPorCodigosCarga(List<int> codigosCarga)
        {
            var sql = @"
                SELECT 
                    oc.CAR_CODIGO AS CodigoCarga,
                    SUM(oc.COC_VALOR_OCORRENCIA) AS ValorTotalOcorrencia,
                    STRING_AGG(CAST(oc.COC_NUMERO_CONTRATO AS varchar), ', ') AS NumerosOcorrencia
                FROM T_CARGA_OCORRENCIA oc
                WHERE oc.CAR_CODIGO IN (:codigosCarga)
                GROUP BY oc.CAR_CODIGO";

            return SessionNHiBernate.CreateSQLQuery(sql)
                .SetParameterList("codigosCarga", codigosCarga)
                .SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<RetornoOcorrenciaCargaEnvioProgramado>())
                .List<RetornoOcorrenciaCargaEnvioProgramado>()
                .ToList();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia> BuscarOcorrenciasPorCodigoNotaFiscaECarga(int codigoCarga, int CodigoNotaFiscal)
        {
            string sql = @$" SELECT DISTINCT Ocorrencia.COC_CODIGO CodigoOcorrencia,
                                    Ocorrencia.COC_NUMERO_CONTRATO NumeroOcorrencia,
                                    NFe.NFX_CODIGO CodigoNotaFiscal,
                                    NFe.NF_NUMERO  NumeroNotaFiscal
                               FROM T_CARGA_OCORRENCIA Ocorrencia
                               JOIN T_CTE_OCORRENCIA CTeOCorrencia ON Ocorrencia.OCO_CODIGO  = CTeOCorrencia.OCO_CODIGO
                               JOIN T_CTE_XML_NOTAS_FISCAIS CTeNotas ON CTeNotas.CON_CODIGO = CTeOCorrencia.CON_CODIGO
                               JOIN T_XML_NOTA_FISCAL NFe ON NFe.NFX_CODIGO = CTeNotas.NFX_CODIGO
                              WHERE NFe.NFX_CODIGO = {CodigoNotaFiscal} AND Ocorrencia.CAR_CODIGO = {codigoCarga}"; // SQL-INJECTION-SAFE

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia)));

            return query.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Ocorrencia.Ocorrencia>();
        }

        public bool VerificarSeExisteNFSManualPendenteGeracao(int codigoOcorrencia)
        {
            IQueryable<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaCTeComplementoInfo>()
                               .Where(cargaCteComplementoInfo => cargaCteComplementoInfo.CargaOcorrencia.Codigo == codigoOcorrencia
                                        && cargaCteComplementoInfo.CargaDocumentoParaEmissaoNFSManualGerado != null
                                        && cargaCteComplementoInfo.CTe == null);
            return query.Any();
        }

        #endregion
    }
}
