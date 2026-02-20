using AdminMultisoftware.Dominio.Enumeradores;
using Dominio.ObjetosDeValor.WebService;
using System;
using System.Collections.Generic;

namespace Servicos.WebService.CTe
{
    public class CCe : ServicoWebServiceBase
    {
        #region Atributos Globais

        Repositorio.UnitOfWork _unitOfWork;
        Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;
        TipoServicoMultisoftware _tipoServicoMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente _clienteMultisoftware;
        AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso _clienteAcesso;
        protected string _adminStringConexao;

        #endregion

        #region Construtores

        public CCe(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { _unitOfWork = unitOfWork; }
        public CCe(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
        }
        public CCe(Repositorio.UnitOfWork unitOfWork, TipoServicoMultisoftware tipoServicoMultisoftware, AdminMultisoftware.Dominio.Entidades.Pessoas.Cliente clienteMultisoftware, Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado auditado, AdminMultisoftware.Dominio.Entidades.Pessoas.ClienteURLAcesso clienteAcesso, string adminStringConexao) : base(unitOfWork, tipoServicoMultisoftware, clienteMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
            _clienteMultisoftware = clienteMultisoftware;
            _auditado = auditado;
            _clienteAcesso = clienteAcesso;
            _adminStringConexao = adminStringConexao;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.ObjetosDeValor.WebService.CTe.CCe ConverterObjetoCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce)
        {
            Dominio.ObjetosDeValor.WebService.CTe.CCe objetoCce = new Dominio.ObjetosDeValor.WebService.CTe.CCe()
            {
                ChaveCTe = cce.CTe.Chave,
                NumeroSequencialEvento = cce.NumeroSequencialEvento,
                Protocolo = cce.Protocolo,
                DataEmissao = cce.DataEmissao,
                DataRetornoSefaz = cce.DataRetornoSefaz,
                CodigoIntegrador = cce.CodigoIntegrador,
                MensagemRetornoSefaz = cce.MensagemRetornoSefaz,
                CodigoErroSefaz = cce.MensagemStatus?.Codigo ?? 0,
                Log = cce.Log,
                Status = cce.Status,
                CampoCCe = ConverterObjetoCamposCCe(cce)
            };

            return objetoCce;
        }

        public Retorno<bool> EnviarCCe(Dominio.ObjetosDeValor.WebService.CTe.CCe cce)
        {
            Servicos.Log.TratarErro($"EnviarCCe: {(cce != null ? Newtonsoft.Json.JsonConvert.SerializeObject(cce) : string.Empty)}", "Request");

            Repositorio.CartaDeCorrecaoEletronica repCCe = new Repositorio.CartaDeCorrecaoEletronica(_unitOfWork);
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);
            Repositorio.CampoCCe repCampoCCe = new Repositorio.CampoCCe(_unitOfWork);
            Repositorio.ConhecimentoDeTransporteEletronico repCTE = new Repositorio.ConhecimentoDeTransporteEletronico(_unitOfWork);
            Repositorio.ErroSefaz repErroSefaz = new Repositorio.ErroSefaz(_unitOfWork);

            try
            {
                Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTE.BuscarPorChave(cce.ChaveCTe);

                if (cte == null)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Não foi localizado nenhum CT-e para recebimento da CCe " + cce.ChaveCTe, false);

                if (cce.CampoCCe == null || cce.CampoCCe.Count == 0)
                    return Retorno<bool>.CriarRetornoDadosInvalidos("Favor informe os campos que deseja realizar a carta de correção.", false);

                _unitOfWork.Start();

                Dominio.Entidades.CartaDeCorrecaoEletronica carta = repCCe.BuscarPorProtocoloAutorizacao(cce.CodigoIntegrador, cce.ChaveCTe);
                if (carta != null)
                {
                    carta.Status = cce.Status;
                    carta.DataRetornoSefaz = cce.DataRetornoSefaz;
                    carta.MensagemRetornoSefaz = cce.MensagemRetornoSefaz;
                    carta.Log += "-Atualização recebida via integração";
                    carta.Importado = true;

                    repCCe.Atualizar(carta);

                    _unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
                else
                {
                    carta = new Dominio.Entidades.CartaDeCorrecaoEletronica()
                    {
                        CTe = cte,
                        DataEmissao = cce.DataEmissao,
                        Log = "CCe recebida via integração",
                        Status = cce.Status,
                        NumeroSequencialEvento = cce.NumeroSequencialEvento,
                        Importado = true,
                        Protocolo = cce.Protocolo,
                        DataRetornoSefaz = cce.DataRetornoSefaz,
                        CodigoIntegrador = cce.CodigoIntegrador,
                        MensagemRetornoSefaz = cce.MensagemRetornoSefaz,
                        MensagemStatus = cce.CodigoErroSefaz > 0 ? repErroSefaz.BuscarPorCodigo(cce.CodigoErroSefaz) : null
                    };

                    repCCe.Inserir(carta);

                    foreach (Dominio.ObjetosDeValor.WebService.CTe.CampoCCe item in cce.CampoCCe)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Descricao) || !string.IsNullOrWhiteSpace(item.GrupoCampo) || !string.IsNullOrWhiteSpace(item.NomeCampo))
                        {
                            Dominio.Entidades.CampoCCe campoCCe = repCampoCCe.BuscarPorNomeCampoGrupo(item.NomeCampo, item.GrupoCampo);
                            if (campoCCe == null)
                            {
                                campoCCe = new Dominio.Entidades.CampoCCe()
                                {
                                    Descricao = item.Descricao,
                                    GrupoCampo = item.GrupoCampo,
                                    IndicadorRepeticao = item.IndicadorRepeticao,
                                    NomeCampo = item.NomeCampo,
                                    Status = "A",
                                    TipoCampo = item.TipoCampo,
                                    QuantidadeCaracteres = item.QuantidadeCaracteres,
                                    QuantidadeDecimais = item.QuantidadeDecimais,
                                    QuantidadeInteiros = item.QuantidadeInteiros
                                };
                                repCampoCCe.Inserir(campoCCe);

                            }
                            Dominio.Entidades.ItemCCe itemCCe = new Dominio.Entidades.ItemCCe();

                            itemCCe.CCe = carta;
                            itemCCe.CampoAlterado = campoCCe;
                            itemCCe.NumeroItemAlterado = item.NumeroItemAlterado;
                            itemCCe.ValorAlterado = item.ValorAlterado;

                            repItemCCe.Inserir(itemCCe);
                        }
                        else
                        {
                            _unitOfWork.Rollback();
                            return Retorno<bool>.CriarRetornoDadosInvalidos("Dados faltantes para a geração da carta de correção, favor verifique os campos obrigatórios.", false);
                        }
                    }

                    //List<Dominio.Entidades.ItemCCe> itensAnteriores = repItemCCe.BuscarPorCTe(carta.CTe.Codigo);
                    //foreach (var item in itensAnteriores)
                    //{
                    //    Dominio.Entidades.ItemCCe itemCCe = new Dominio.Entidades.ItemCCe();

                    //    itemCCe.CCe = carta;
                    //    itemCCe.CampoAlterado = item.CampoAlterado;
                    //    itemCCe.NumeroItemAlterado = item.NumeroItemAlterado;
                    //    itemCCe.ValorAlterado = item.ValorAlterado;

                    //    repItemCCe.Inserir(itemCCe);
                    //}

                    cte.PossuiCartaCorrecao = true;
                    repCTE.Atualizar(cte);

                    _unitOfWork.CommitChanges();

                    return Retorno<bool>.CriarRetornoSucesso(true);
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                Servicos.Log.TratarErro(ex);
                ArmazenarLogIntegracao(cce, _unitOfWork);
                return Retorno<bool>.CriarRetornoDadosInvalidos(ex.Message);
            }
        }

        #endregion

        #region Métodos Privados

        private List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> ConverterObjetoCamposCCe(Dominio.Entidades.CartaDeCorrecaoEletronica cce)
        {
            Repositorio.ItemCCe repItemCCe = new Repositorio.ItemCCe(_unitOfWork);

            List<Dominio.Entidades.ItemCCe> itensCCe = repItemCCe.BuscarPorCCe(cce.Codigo);

            List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe> camposCCe = new List<Dominio.ObjetosDeValor.WebService.CTe.CampoCCe>();

            foreach (Dominio.Entidades.ItemCCe item in itensCCe)
            {
                Dominio.Entidades.CampoCCe campoCCe = item.CampoAlterado;

                Dominio.ObjetosDeValor.WebService.CTe.CampoCCe objetoCampoCce = new Dominio.ObjetosDeValor.WebService.CTe.CampoCCe()
                {
                    ValorAlterado = item.ValorAlterado,
                    NumeroItemAlterado = item.NumeroItemAlterado,
                    Descricao = campoCCe.Descricao,
                    NomeCampo = campoCCe.NomeCampo,
                    GrupoCampo = campoCCe.GrupoCampo,
                    IndicadorRepeticao = campoCCe.IndicadorRepeticao,
                    TipoCampo = campoCCe.TipoCampo,
                    QuantidadeInteiros = campoCCe.QuantidadeInteiros,
                    QuantidadeDecimais = campoCCe.QuantidadeDecimais,
                    QuantidadeCaracteres = campoCCe.QuantidadeCaracteres
                };

                camposCCe.Add(objetoCampoCce);
            }

            return camposCCe;
        }

        #endregion
    }
}
