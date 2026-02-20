using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Servicos.Embarcador.GestaoPatio
{
    public sealed class FluxoGestaoPatioConfiguracaoEtapa
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio _configuracaoGestaoPatio;

        #endregion Atributos Privados

        #region Construtores

        public FluxoGestaoPatioConfiguracaoEtapa(Repositorio.UnitOfWork unitOfWork) : this(unitOfWork, configuracaoGestaoPatio: null) { }

        public FluxoGestaoPatioConfiguracaoEtapa(Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio)
        {
            _configuracaoGestaoPatio = configuracaoGestaoPatio;
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio ObterConfiguracaoGestaoPatio()
        {
            if (_configuracaoGestaoPatio == null)
            {
                FluxoGestaoPatioConfiguracao servicoConfiguracaoGestaoPatio = new FluxoGestaoPatioConfiguracao(_unitOfWork);
                _configuracaoGestaoPatio = servicoConfiguracaoGestaoPatio.ObterConfiguracao();
            }

            return _configuracaoGestaoPatio;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> ObterDescricoesEtapas(TipoFluxoGestaoPatio tipo, List<int> codigosFilial, int codigoTipoOperacao)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> descricoesEtapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao>();

            if (codigosFilial?.Count > 0)
            {
                foreach (int codigoFilial in codigosFilial)
                {
                    List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = ObterEtapas(tipo, codigoFilial, codigoTipoOperacao);

                    descricoesEtapas.AddRange((
                        from etapa in etapas
                        select new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao()
                        {
                            CodigoFilial = codigoFilial,
                            Descricao = etapa.Descricao,
                            Enumerador = etapa.Etapa,
                            Tipo = tipo
                        }
                    ).ToList());
                }
            }

            return descricoesEtapas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> ObterDescricoesEtapasAgrupadas(List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> descricoesEtapas)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapasAgrupadas = descricoesEtapas
                .GroupBy(obj => new
                {
                    obj.Enumerador,
                })
                .Select(obj => new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada()
                {
                    Enumerador = obj.Key.Enumerador,
                    Descricao = string.Join(" / ", obj.Select(d => d.Descricao.Trim()).Distinct())
                }).ToList();

            return descricoesEtapasAgrupadas;
        }

        private List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> ObterEtapas(Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio, bool retornarTodasEtapas)
        {
            Dominio.Entidades.Embarcador.Filiais.ConfiguracaoGestaoPatio configuracaoGestaoPatio = ObterConfiguracaoGestaoPatio();
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa>();

            if (!retornarTodasEtapas && (sequenciaGestaoPatio == null))
                return etapas;

            if (retornarTodasEtapas || sequenciaGestaoPatio.AvaliacaoDescarga)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.AvaliacaoDescargaDescricao, configuracaoGestaoPatio.AvaliacaoDescargaDescricao, EtapaFluxoGestaoPatio.AvaliacaoDescarga.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.AvaliacaoDescarga,
                    CodigoIntegracao = sequenciaGestaoPatio?.AvaliacaoDescargaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemAvaliacaoDescarga ?? 0,
                    CheckListPermiteSalvarSemPreencher = configuracaoGestaoPatio.CheckListPermiteSalvarSemPreencher,
                    PermiteQRCode = configuracaoGestaoPatio.CheckListPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.InformarDocaCarregamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.InformarDocaCarregamentoDescricao, configuracaoGestaoPatio.InformarDocaCarregamentoDescricao, EtapaFluxoGestaoPatio.InformarDoca.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.InformarDoca,
                    CodigoIntegracao = sequenciaGestaoPatio?.InformarDocaCarregamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemInformarDocaCarregamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.InformarDocaCarregamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.ChegadaVeiculo)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.ChegadaVeiculoDescricao, configuracaoGestaoPatio.ChegadaVeiculoDescricao, EtapaFluxoGestaoPatio.ChegadaVeiculo.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.ChegadaVeiculo,
                    CodigoIntegracao = sequenciaGestaoPatio?.ChegadaVeiculoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemChegadaVeiculo ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.ChegadaVeiculoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.GuaritaEntrada)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.GuaritaEntradaDescricao, configuracaoGestaoPatio.GuaritaEntradaDescricao, EtapaFluxoGestaoPatio.Guarita.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.Guarita,
                    CodigoIntegracao = sequenciaGestaoPatio?.GuaritaEntradaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemGuaritaEntrada ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.GuaritaEntradaPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.CheckList)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.CheckListDescricao, configuracaoGestaoPatio.CheckListDescricao, EtapaFluxoGestaoPatio.CheckList.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.CheckList,
                    CodigoIntegracao = sequenciaGestaoPatio?.CheckListCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemCheckList ?? 0,
                    CheckListPermiteSalvarSemPreencher = configuracaoGestaoPatio.CheckListPermiteSalvarSemPreencher,
                    PermiteQRCode = configuracaoGestaoPatio.CheckListPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.TravaChave)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.TravaChaveDescricao, configuracaoGestaoPatio.TravaChaveDescricao, EtapaFluxoGestaoPatio.TravamentoChave.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.TravamentoChave,
                    CodigoIntegracao = sequenciaGestaoPatio?.TravaChaveCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemTravaChave ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.TravaChavePermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.Expedicao)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.ExpedicaoDescricao, configuracaoGestaoPatio.ExpedicaoDescricao, EtapaFluxoGestaoPatio.Expedicao.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.Expedicao,
                    CodigoIntegracao = sequenciaGestaoPatio?.ExpedicaoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemExpedicao ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.ExpedicaoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.LiberaChave)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.LiberaChaveDescricao, configuracaoGestaoPatio.LiberaChaveDescricao, EtapaFluxoGestaoPatio.LiberacaoChave.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.LiberacaoChave,
                    CodigoIntegracao = sequenciaGestaoPatio?.LiberaChaveCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemLiberaChave ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.LiberaChavePermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.Faturamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FaturamentoDescricao, configuracaoGestaoPatio.FaturamentoDescricao, EtapaFluxoGestaoPatio.Faturamento.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.Faturamento,
                    CodigoIntegracao = sequenciaGestaoPatio?.FaturamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemFaturamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.FaturamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.GuaritaSaida)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.GuaritaSaidaDescricao, configuracaoGestaoPatio.GuaritaSaidaDescricao, EtapaFluxoGestaoPatio.InicioViagem.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.InicioViagem,
                    CodigoIntegracao = sequenciaGestaoPatio?.GuaritaSaidaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemGuaritaSaida ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.GuaritaSaidaPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.Posicao)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.PosicaoDescricao, configuracaoGestaoPatio.PosicaoDescricao, EtapaFluxoGestaoPatio.Posicao.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.Posicao,
                    CodigoIntegracao = sequenciaGestaoPatio?.PosicaoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemPosicao ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.PosicaoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.ChegadaLoja)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.ChegadaLojaDescricao, configuracaoGestaoPatio.ChegadaLojaDescricao, EtapaFluxoGestaoPatio.ChegadaLoja.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.ChegadaLoja,
                    CodigoIntegracao = sequenciaGestaoPatio?.ChegadaLojaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemChegadaLoja ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.ChegadaLojaPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.DeslocamentoPatio)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.DeslocamentoPatioDescricao, configuracaoGestaoPatio.DeslocamentoPatioDescricao, EtapaFluxoGestaoPatio.DeslocamentoPatio.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.DeslocamentoPatio,
                    CodigoIntegracao = sequenciaGestaoPatio?.DeslocamentoPatioCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemDeslocamentoPatio ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.DeslocamentoPatioPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.SaidaLoja)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.SaidaLojaDescricao, configuracaoGestaoPatio.SaidaLojaDescricao, EtapaFluxoGestaoPatio.SaidaLoja.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.SaidaLoja,
                    CodigoIntegracao = sequenciaGestaoPatio?.SaidaLojaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemSaidaLoja ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.SaidaLojaPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.FimViagem)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FimViagemDescricao, configuracaoGestaoPatio.FimViagemDescricao, EtapaFluxoGestaoPatio.FimViagem.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.FimViagem,
                    CodigoIntegracao = sequenciaGestaoPatio?.FimViagemCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemFimViagem ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.FimViagemPermiteQRCode
                });
            }

            if (retornarTodasEtapas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FimViagemDescricao, configuracaoGestaoPatio.FimViagemDescricao, EtapaFluxoGestaoPatio.Entregas.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.Entregas,
                    CodigoIntegracao = "",
                    Ordem = 0,
                    PermiteQRCode = false
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.InicioHigienizacao)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.InicioHigienizacaoDescricao, configuracaoGestaoPatio.InicioHigienizacaoDescricao, EtapaFluxoGestaoPatio.InicioHigienizacao.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.InicioHigienizacao,
                    CodigoIntegracao = sequenciaGestaoPatio?.InicioHigienizacaoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemInicioHigienizacao ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.InicioHigienizacaoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.FimHigienizacao)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FimHigienizacaoDescricao, configuracaoGestaoPatio.FimHigienizacaoDescricao, EtapaFluxoGestaoPatio.FimHigienizacao.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.FimHigienizacao,
                    CodigoIntegracao = sequenciaGestaoPatio?.FimHigienizacaoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemFimHigienizacao ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.FimHigienizacaoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.InicioCarregamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.InicioCarregamentoDescricao, configuracaoGestaoPatio.InicioCarregamentoDescricao, EtapaFluxoGestaoPatio.InicioCarregamento.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.InicioCarregamento,
                    CodigoIntegracao = sequenciaGestaoPatio?.InicioCarregamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemInicioCarregamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.InicioCarregamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.FimCarregamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FimCarregamentoDescricao, configuracaoGestaoPatio.FimCarregamentoDescricao, EtapaFluxoGestaoPatio.FimCarregamento.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.FimCarregamento,
                    CodigoIntegracao = sequenciaGestaoPatio?.FimCarregamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemFimCarregamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.FimCarregamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.SolicitacaoVeiculo)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.SolicitacaoVeiculoDescricao, configuracaoGestaoPatio.SolicitacaoVeiculoDescricao, EtapaFluxoGestaoPatio.SolicitacaoVeiculo.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.SolicitacaoVeiculo,
                    CodigoIntegracao = sequenciaGestaoPatio?.SolicitacaoVeiculoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemSolicitacaoVeiculo ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.SolicitacaoVeiculoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.InicioDescarregamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.InicioDescarregamentoDescricao, configuracaoGestaoPatio.InicioDescarregamentoDescricao, EtapaFluxoGestaoPatio.InicioDescarregamento.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.InicioDescarregamento,
                    CodigoIntegracao = sequenciaGestaoPatio?.InicioDescarregamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemInicioDescarregamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.InicioDescarregamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.FimDescarregamento)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.FimDescarregamentoDescricao, configuracaoGestaoPatio.FimDescarregamentoDescricao, EtapaFluxoGestaoPatio.FimDescarregamento.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.FimDescarregamento,
                    CodigoIntegracao = sequenciaGestaoPatio?.FimDescarregamentoCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemFimDescarregamento ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.FimDescarregamentoPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.DocumentoFiscal)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.DocumentoFiscalDescricao, configuracaoGestaoPatio.DocumentoFiscalDescricao, EtapaFluxoGestaoPatio.DocumentoFiscal.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.DocumentoFiscal,
                    CodigoIntegracao = sequenciaGestaoPatio?.DocumentoFiscalCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemDocumentoFiscal ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.DocumentoFiscalPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.DocumentosTransporte)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.DocumentosTransporteDescricao, configuracaoGestaoPatio.DocumentosTransporteDescricao, EtapaFluxoGestaoPatio.DocumentosTransporte.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.DocumentosTransporte,
                    CodigoIntegracao = sequenciaGestaoPatio?.DocumentosTransporteCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemDocumentosTransporte ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.DocumentosTransportePermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.MontagemCarga)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.MontagemCargaDescricao, configuracaoGestaoPatio.MontagemCargaDescricao, EtapaFluxoGestaoPatio.MontagemCarga.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.MontagemCarga,
                    CodigoIntegracao = sequenciaGestaoPatio?.MontagemCargaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemMontagemCarga ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.MontagemCargaPermiteQRCode
                });
            }

            if (retornarTodasEtapas || sequenciaGestaoPatio.SeparacaoMercadoria)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa
                {
                    Descricao = ObterNomeEtapa(sequenciaGestaoPatio?.SeparacaoMercadoriaDescricao, configuracaoGestaoPatio.SeparacaoMercadoriaDescricao, EtapaFluxoGestaoPatio.SeparacaoMercadoria.ObterDescricao()),
                    Etapa = EtapaFluxoGestaoPatio.SeparacaoMercadoria,
                    CodigoIntegracao = sequenciaGestaoPatio?.SeparacaoMercadoriaCodigoIntegracao,
                    Ordem = sequenciaGestaoPatio?.OrdemSeparacaoMercadoria ?? 0,
                    PermiteQRCode = configuracaoGestaoPatio.SeparacaoMercadoriaPermiteQRCode
                });
            }

            return etapas.OrderBy(o => o.Ordem).ToList();
        }

        private string ObterNomeEtapa(string nomeConfiguracaoFilial, string nomeConfiguracaoGeral, string nomePadrao)
        {
            if (!string.IsNullOrEmpty(nomeConfiguracaoFilial))
                return nomeConfiguracaoFilial;

            if (!string.IsNullOrEmpty(nomeConfiguracaoGeral))
                return nomeConfiguracaoGeral;

            return nomePadrao;
        }

        #endregion Métodos Privados

        #region Métodos Públicos

        public bool ExisteSequenciaGestaoPatio(int codigoFilial, TipoFluxoGestaoPatio tipo)
        {
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(_unitOfWork);

            return repositorioSequenciaGestaoPatio.ExistePorFilialETipo(codigoFilial, tipo);
        }

        public List<int> ObterCodigosFiliaisComSequenciaGestaoPatio(TipoFluxoGestaoPatio tipo)
        {
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(_unitOfWork);

            return repositorioSequenciaGestaoPatio.BuscarCodigosFiliaisPorTipo(tipo);
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapaConfiguracao ObterConfiguracoesPorCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Filiais.GestaoPatioConfiguracaoEtapa repositorioGestaoPatioConfiguracaoEtapa = new Repositorio.Embarcador.Filiais.GestaoPatioConfiguracaoEtapa(_unitOfWork);
            IList<Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa> configuracoesPorEtapa = repositorioGestaoPatioConfiguracaoEtapa.BuscarPorCarga(carga.Codigo);
            Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa configuracaoBloquearEdicaoDadosTransporte = configuracoesPorEtapa.Where(o => o.BloquearEdicaoDadosTransporteJanelaTransportador.HasValue).OrderByDescending(o => o.PossuiFiliaisExclusivas).ThenByDescending(o => o.SituacaoConfirmacao).ThenByDescending(o => o.Ordem).FirstOrDefault();
            Dominio.ObjetosDeValor.Embarcador.Filial.GestaoPatioConfiguracaoEtapa configuracaoBloquearEdicaoVeiculos = configuracoesPorEtapa.Where(o => o.BloquearEdicaoVeiculosCarga.HasValue).OrderByDescending(o => o.PossuiFiliaisExclusivas).ThenByDescending(o => o.SituacaoConfirmacao).ThenByDescending(o => o.Ordem).FirstOrDefault();

            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapaConfiguracao()
            {
                BloquearEdicaoDadosTransporteJanelaTransportador = (configuracaoBloquearEdicaoDadosTransporte?.BloquearEdicaoDadosTransporteJanelaTransportador ?? false),
                BloquearEdicaoVeiculosCarga = (configuracaoBloquearEdicaoVeiculos?.BloquearEdicaoVeiculosCarga ?? false)
            };
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao ObterDescricaoEtapa(EtapaFluxoGestaoPatio etapa, TipoFluxoGestaoPatio tipo)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> descricoesEtapas = ObterDescricoesEtapas(tipo, codigosFilial: new List<int>() { 0 }, codigoTipoOperacao: 0);

            return descricoesEtapas.Where(descricaoEtapa => descricaoEtapa.Enumerador == etapa).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada ObterDescricaoEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            return ObterDescricaoEtapa(fluxoGestaoPatio, fluxoGestaoPatio.EtapaFluxoGestaoPatioAtual);
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada ObterDescricaoEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, EtapaFluxoGestaoPatio etapa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapas = ObterDescricoesEtapas(fluxoGestaoPatio);

            return descricoesEtapas.Where(descricaoEtapa => descricaoEtapa.Enumerador == etapa).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada ObterDescricaoEtapa(EtapaFluxoGestaoPatio etapa)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = ObterEtapas(sequenciaGestaoPatio: null, retornarTodasEtapas: true);
            Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa gestaoPatioEtapa = etapas.Where(o => o.Etapa == etapa).First();

            return new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada()
            {
                Descricao = gestaoPatioEtapa.Descricao,
                Enumerador = gestaoPatioEtapa.Etapa
            };
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> ObterDescricoesEtapas(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> descricoesEtapas = ObterDescricoesEtapas(fluxoGestaoPatio.Tipo, new List<int>() { fluxoGestaoPatio.Filial?.Codigo ?? 0 }, fluxoGestaoPatio.CargaBase.TipoOperacao?.Codigo ?? 0);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> descricoesEtapasAgrupadas = ObterDescricoesEtapasAgrupadas(descricoesEtapas);

            return descricoesEtapasAgrupadas;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricaoSimplificada> ObterDescricoesEtapasAgrupadas(List<int> codigosFilial, TipoFluxoGestaoPatio? tipo)
        {
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFiliaisETipo(codigosFilial, tipo);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao> descricoesEtapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao>();

            foreach (Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio in sequenciasGestaoPatio)
            {
                List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = ObterEtapas(sequenciaGestaoPatio, retornarTodasEtapas: false);

                descricoesEtapas.AddRange((
                    from etapa in etapas
                    select new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.EtapaDescricao()
                    {
                        CodigoFilial = sequenciaGestaoPatio.Filial.Codigo,
                        Descricao = etapa.Descricao,
                        Enumerador = etapa.Etapa,
                        Tipo = sequenciaGestaoPatio.Tipo
                    }
                ).ToList());
            }

            return ObterDescricoesEtapasAgrupadas(descricoesEtapas);
        }

        public Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa ObterEtapa(EtapaFluxoGestaoPatio etapa, TipoFluxoGestaoPatio tipo, int codigoFilial)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapas = ObterEtapas(tipo, codigoFilial, codigoTipoOperacao: 0);

            return (from o in etapas where o.Etapa == etapa select o).FirstOrDefault();
        }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.EtapaFluxoGestaoPatio ObterEtapa(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio, string codigoIntegracao)
        {
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(fluxoGestaoPatio);
            if (sequenciaGestaoPatio.MontagemCargaCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.MontagemCarga;

            if (sequenciaGestaoPatio.InformarDocaCarregamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.InformarDoca;

            if (sequenciaGestaoPatio.ChegadaVeiculoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.ChegadaVeiculo;

            if (sequenciaGestaoPatio.GuaritaEntradaCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.Guarita;

            if (sequenciaGestaoPatio.CheckListCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.CheckList;

            if (sequenciaGestaoPatio.TravaChaveCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.TravamentoChave;

            if (sequenciaGestaoPatio.ExpedicaoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.Expedicao;

            if (sequenciaGestaoPatio.LiberaChaveCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.LiberacaoChave;

            if (sequenciaGestaoPatio.FaturamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.Faturamento;

            if (sequenciaGestaoPatio.GuaritaSaidaCodigoIntegracao== codigoIntegracao)
                return EtapaFluxoGestaoPatio.Guarita;

            if (sequenciaGestaoPatio.PosicaoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.Posicao;

            if (sequenciaGestaoPatio.ChegadaVeiculoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.ChegadaLoja;

            if (sequenciaGestaoPatio.DeslocamentoPatioCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.DeslocamentoPatio;

            if (sequenciaGestaoPatio.SaidaLojaCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.SaidaLoja;

            if (sequenciaGestaoPatio.FimViagemCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.FimViagem;

            if (sequenciaGestaoPatio.InicioCarregamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.InicioCarregamento;

            if (sequenciaGestaoPatio.FimCarregamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.FimCarregamento;

            if (sequenciaGestaoPatio.InicioHigienizacaoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.InicioHigienizacao;

            if (sequenciaGestaoPatio.FimHigienizacaoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.FimHigienizacao;

            if (sequenciaGestaoPatio.SolicitacaoVeiculoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.SolicitacaoVeiculo;

            if (sequenciaGestaoPatio.InicioDescarregamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.InicioDescarregamento;
            
            if (sequenciaGestaoPatio.FimDescarregamentoCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.FimDescarregamento;

            if (sequenciaGestaoPatio.DocumentoFiscalCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.DocumentoFiscal;

            if (sequenciaGestaoPatio.DocumentosTransporteCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.DocumentosTransporte;

            if (sequenciaGestaoPatio.SeparacaoMercadoriaCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.SeparacaoMercadoria;

            if (sequenciaGestaoPatio.AvaliacaoDescargaCodigoIntegracao == codigoIntegracao)
                return EtapaFluxoGestaoPatio.AvaliacaoDescarga;

            return EtapaFluxoGestaoPatio.Todas;
        
        }




        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> ObterEtapas(TipoFluxoGestaoPatio tipo)
        {
            return ObterEtapas(tipo, codigoFilial: 0, codigoTipoOperacao: 0);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> ObterEtapas(TipoFluxoGestaoPatio tipo, int codigoFilial, int codigoTipoOperacao)
        {
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(tipo, codigoFilial, codigoTipoOperacao);

            return ObterEtapas(sequenciaGestaoPatio, retornarTodasEtapas: (codigoFilial == 0));
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio> ObterEtapasAtualizacao(TipoFluxoGestaoPatio tipo, int codigoFilial, int codigoTipoOperacao, List<Dominio.ObjetosDeValor.WebService.GestaoPatio.AtualizacaoStatusEtapa> etapasWebService)
        {
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa> etapasPatio = ObterEtapas(tipo, codigoFilial, codigoTipoOperacao);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio> etapasAtualizacao = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio>();

            foreach (Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioEtapa etapaPatio in etapasPatio)
            {
                Dominio.ObjetosDeValor.WebService.GestaoPatio.AtualizacaoStatusEtapa etapaWebService = (from o in etapasWebService where o.Status == etapaPatio.CodigoIntegracao select o).FirstOrDefault();

                if (etapaWebService == null)
                    continue;

                Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio etapaAtualizacao = new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.AtualizacaoEtapaPatio()
                {
                    Etapa = etapaPatio,
                    DataHora = $"{etapaWebService.Data} {etapaWebService.Hora}".ToNullableDateTime()
                };

                etapasAtualizacao.Add(etapaAtualizacao);

                if (!etapaAtualizacao.DataHora.HasValue)
                    break;
            }

            return etapasAtualizacao;
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> ObterEtapasOrdenadas(TipoFluxoGestaoPatio tipo, int codigoFilial, int codigoTipoOperacao)
        {
            return ObterEtapasOrdenadas(tipo, codigoFilial, codigoTipoOperacao, retornarEtapasDesabilitadas: false);
        }

        public List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> ObterEtapasOrdenadas(TipoFluxoGestaoPatio tipo, int codigoFilial, int codigoTipoOperacao, bool retornarEtapasDesabilitadas)
        {
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = ObterSequenciaGestaoPatio(tipo, codigoFilial, codigoTipoOperacao);
            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapas = new List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao>();

            if (sequenciaGestaoPatio == null)
                return etapas;

            if (sequenciaGestaoPatio.AvaliacaoDescarga || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemAvaliacaoDescarga,
                    Tempo = sequenciaGestaoPatio.AvaliacaoDescargaTempo,
                    Etapa = EtapaFluxoGestaoPatio.AvaliacaoDescarga
                });
            }

            if (sequenciaGestaoPatio.CheckList || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemCheckList,
                    Tempo = sequenciaGestaoPatio.CheckListTempo,
                    Etapa = EtapaFluxoGestaoPatio.CheckList
                });
            }

            if (sequenciaGestaoPatio.ChegadaLoja || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemChegadaLoja,
                    Tempo = sequenciaGestaoPatio.ChegadaLojaTempo,
                    Etapa = EtapaFluxoGestaoPatio.ChegadaLoja
                });
            }

            if (sequenciaGestaoPatio.ChegadaVeiculo || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemChegadaVeiculo,
                    Tempo = sequenciaGestaoPatio.ChegadaVeiculoTempo,
                    Etapa = EtapaFluxoGestaoPatio.ChegadaVeiculo
                });
            }

            if (sequenciaGestaoPatio.DeslocamentoPatio || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemDeslocamentoPatio,
                    Tempo = sequenciaGestaoPatio.DeslocamentoPatioTempo,
                    Etapa = EtapaFluxoGestaoPatio.DeslocamentoPatio
                });
            }

            if (sequenciaGestaoPatio.DocumentoFiscal || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemDocumentoFiscal,
                    Tempo = sequenciaGestaoPatio.DocumentoFiscalTempo,
                    Etapa = EtapaFluxoGestaoPatio.DocumentoFiscal
                });
            }

            if (sequenciaGestaoPatio.DocumentosTransporte || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemDocumentosTransporte,
                    Tempo = sequenciaGestaoPatio.DocumentosTransporteTempo,
                    Etapa = EtapaFluxoGestaoPatio.DocumentosTransporte
                });
            }

            if (sequenciaGestaoPatio.Expedicao || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemExpedicao,
                    Tempo = sequenciaGestaoPatio.ExpedicaoTempo,
                    Etapa = EtapaFluxoGestaoPatio.Expedicao
                });
            }

            if (sequenciaGestaoPatio.Faturamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemFaturamento,
                    Tempo = sequenciaGestaoPatio.FaturamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.Faturamento
                });
            }

            if (sequenciaGestaoPatio.FimCarregamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemFimCarregamento,
                    Tempo = sequenciaGestaoPatio.FimCarregamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.FimCarregamento
                });
            }

            if (sequenciaGestaoPatio.FimDescarregamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemFimDescarregamento,
                    Tempo = sequenciaGestaoPatio.FimDescarregamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.FimDescarregamento
                });
            }

            if (sequenciaGestaoPatio.FimHigienizacao || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemFimHigienizacao,
                    Tempo = sequenciaGestaoPatio.FimHigienizacaoTempo,
                    Etapa = EtapaFluxoGestaoPatio.FimHigienizacao
                });
            }

            if (sequenciaGestaoPatio.FimViagem || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemFimViagem,
                    Tempo = sequenciaGestaoPatio.FimViagemTempo,
                    Etapa = EtapaFluxoGestaoPatio.FimViagem
                });
            }

            if (sequenciaGestaoPatio.GuaritaEntrada || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemGuaritaEntrada,
                    Tempo = sequenciaGestaoPatio.GuaritaEntradaTempo,
                    Etapa = EtapaFluxoGestaoPatio.Guarita
                });
            }

            if (sequenciaGestaoPatio.GuaritaSaida || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemGuaritaSaida,
                    Tempo = sequenciaGestaoPatio.GuaritaSaidaTempo,
                    Etapa = EtapaFluxoGestaoPatio.InicioViagem
                });
            }

            if (sequenciaGestaoPatio.InformarDocaCarregamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemInformarDocaCarregamento,
                    Tempo = sequenciaGestaoPatio.InformarDocaCarregamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.InformarDoca
                });
            }

            if (sequenciaGestaoPatio.InicioCarregamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemInicioCarregamento,
                    Tempo = sequenciaGestaoPatio.InicioCarregamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.InicioCarregamento
                });
            }

            if (sequenciaGestaoPatio.InicioDescarregamento || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemInicioDescarregamento,
                    Tempo = sequenciaGestaoPatio.InicioDescarregamentoTempo,
                    Etapa = EtapaFluxoGestaoPatio.InicioDescarregamento
                });
            }

            if (sequenciaGestaoPatio.InicioHigienizacao || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemInicioHigienizacao,
                    Tempo = sequenciaGestaoPatio.InicioHigienizacaoTempo,
                    Etapa = EtapaFluxoGestaoPatio.InicioHigienizacao
                });
            }

            if (sequenciaGestaoPatio.LiberaChave || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemLiberaChave,
                    Tempo = sequenciaGestaoPatio.LiberaChaveTempo,
                    Etapa = EtapaFluxoGestaoPatio.LiberacaoChave
                });
            }

            if (sequenciaGestaoPatio.MontagemCarga || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemMontagemCarga,
                    Tempo = sequenciaGestaoPatio.MontagemCargaTempo,
                    Etapa = EtapaFluxoGestaoPatio.MontagemCarga
                });
            }

            if (sequenciaGestaoPatio.Posicao || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemPosicao,
                    Tempo = sequenciaGestaoPatio.PosicaoTempo,
                    Etapa = EtapaFluxoGestaoPatio.Posicao
                });
            }

            if (sequenciaGestaoPatio.SaidaLoja || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemSaidaLoja,
                    Tempo = sequenciaGestaoPatio.SaidaLojaTempo,
                    Etapa = EtapaFluxoGestaoPatio.SaidaLoja
                });
            }

            if (sequenciaGestaoPatio.SeparacaoMercadoria || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemSeparacaoMercadoria,
                    Tempo = sequenciaGestaoPatio.SeparacaoMercadoriaTempo,
                    Etapa = EtapaFluxoGestaoPatio.SeparacaoMercadoria
                });
            }

            if (sequenciaGestaoPatio.SolicitacaoVeiculo || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemSolicitacaoVeiculo,
                    Tempo = sequenciaGestaoPatio.SolicitacaoVeiculoTempo,
                    Etapa = EtapaFluxoGestaoPatio.SolicitacaoVeiculo
                });
            }

            if (sequenciaGestaoPatio.TravaChave || retornarEtapasDesabilitadas)
            {
                etapas.Add(new Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao()
                {
                    Ordem = sequenciaGestaoPatio.OrdemTravaChave,
                    Tempo = sequenciaGestaoPatio.TravaChaveTempo,
                    Etapa = EtapaFluxoGestaoPatio.TravamentoChave
                });
            }

            List<Dominio.ObjetosDeValor.Embarcador.GestaoPatio.GestaoPatioOrdenacao> etapasOrdenadas = (from etapa in etapas orderby etapa.Ordem ascending select etapa).ToList();

            for (int i = 0; i < etapasOrdenadas.Count; i++)
                etapasOrdenadas[i].Ordem = i;

            return etapasOrdenadas;
        }

        public Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio ObterSequenciaGestaoPatio(Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (fluxoGestaoPatio != null)
                return ObterSequenciaGestaoPatio(fluxoGestaoPatio.Tipo, fluxoGestaoPatio.Filial?.Codigo ?? 0, fluxoGestaoPatio.CargaBase?.TipoOperacao?.Codigo ?? 0);
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio ObterSequenciaGestaoPatio(TipoFluxoGestaoPatio tipo, int codigoFilial)
        {
            return ObterSequenciaGestaoPatio(tipo, codigoFilial, codigoTipoOperacao: 0);
        }

        public Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio ObterSequenciaGestaoPatio(TipoFluxoGestaoPatio tipo, int codigoFilial, int codigoTipoOperacao)
        {
            Repositorio.Embarcador.Filiais.SequenciaGestaoPatio repositorioSequenciaGestaoPatio = new Repositorio.Embarcador.Filiais.SequenciaGestaoPatio(_unitOfWork);
            List<Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio> sequenciasGestaoPatio = repositorioSequenciaGestaoPatio.BuscarTodosPorFilialETipo(codigoFilial, tipo);

            return sequenciasGestaoPatio
                .Where(sequencia => sequencia.TipoOperacao == null || sequencia.TipoOperacao.Codigo == codigoTipoOperacao)
                .OrderBy(sequencia => sequencia.TipoOperacao == null)
                .FirstOrDefault();
        }

        #endregion Métodos Públicos
    }
}
