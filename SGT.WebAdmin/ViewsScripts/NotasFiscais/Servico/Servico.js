/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Consultas/CFOP.js" />
/// <reference path="../../Enumeradores/EnumTipoCFOP.js" />
/// <reference path="../../Enumeradores/EnumListaServico.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridServico;
var _servico;
var _pesquisaServico;
var _crudServico;

var _listaServico = [
    { text: "Selecione", value: 0 },
    { text: "1.01 Análise e desenvolvimento de sistemas", value: EnumListaServico.LS101 },
    { text: "1.02 Programação", value: EnumListaServico.LS102 },
    { text: "1.03 Processamento de dados e congêneres", value: EnumListaServico.LS103 },
    { text: "1.04 Elaboração de programas de computadores, inclusive de jogos eletrônicos", value: EnumListaServico.LS104 },
    { text: "1.05 Licenciamento ou cessão de direito de uso de programas de computação", value: EnumListaServico.LS105 },
    { text: "1.06 Assessoria e consultoria em informática", value: EnumListaServico.LS106 },
    { text: "1.07 Suporte técnico em informática, inclusive instalação, configuração e manutenção de programas de computação e bancos de dados", value: EnumListaServico.LS107 },
    { text: "1.08 Planejamento, confecção, manutenção e atualização de páginas eletrônicas", value: EnumListaServico.LS108 },
    { text: "2.01 Serviços de pesquisas e desenvolvimento de qualquer natureza", value: EnumListaServico.LS201 },
    { text: "3.01 (VETADO)", value: EnumListaServico.LS301 },
    { text: "3.02 Cessão de direito de uso de marcas e de sinais de propaganda", value: EnumListaServico.LS302 },
    { text: "3.03 Exploração de salões de festas, centro de convenções, escritórios virtuais, stands, quadras esportivas, estádios, ginásios, auditórios, casas de espetáculos, parques de diversões, canchas e congêneres, para realização de eventos ou negócios de qualquer natureza", value: EnumListaServico.LS303 },
    { text: "3.04 Locação, sublocação, arrendamento, direito de passagem ou permissão de uso, compartilhado ou não, de ferrovia, rodovia, postes, cabos, dutos e condutos de qualquer natureza", value: EnumListaServico.LS304 },
    { text: "3.05 Cessão de andaimes, palcos, coberturas e outras estruturas de uso temporário", value: EnumListaServico.LS305 },
    { text: "4.01 Medicina e biomedicina", value: EnumListaServico.LS401 },
    { text: "4.02 Análises clínicas, patologia, eletricidade médica, radioterapia, quimioterapia, ultra-sonografia, ressonância magnética, radiologia, tomografia e congêneres", value: EnumListaServico.LS402 },
    { text: "4.03 Hospitais, clínicas, laboratórios, sanatórios, manicômios, casas de saúde, prontos-socorros, ambulatórios e congêneres", value: EnumListaServico.LS403 },
    { text: "4.04 Instrumentação cirúrgica", value: EnumListaServico.LS404 },
    { text: "4.05 Acupuntura", value: EnumListaServico.LS405 },
    { text: "4.06 Enfermagem, inclusive serviços auxiliares", value: EnumListaServico.LS406 },
    { text: "4.07 Serviços farmacêuticos", value: EnumListaServico.LS407 },
    { text: "4.08 Terapia ocupacional, fisioterapia e fonoaudiologia", value: EnumListaServico.LS408 },
    { text: "4.09 Terapias de qualquer espécie destinadas ao tratamento físico, orgânico e mental", value: EnumListaServico.LS409 },
    { text: "4.10 Nutrição", value: EnumListaServico.LS410 },
    { text: "4.11 Obstetrícia", value: EnumListaServico.LS411 },
    { text: "4.12 Odontologia", value: EnumListaServico.LS412 },
    { text: "4.13 Ortóptica", value: EnumListaServico.LS413 },
    { text: "4.14 Próteses sob encomenda", value: EnumListaServico.LS414 },
    { text: "4.15 Psicanálise", value: EnumListaServico.LS415 },
    { text: "4.16 Psicologia", value: EnumListaServico.LS416 },
    { text: "4.17 Casas de repouso e de recuperação, creches, asilos e congêneres", value: EnumListaServico.LS417 },
    { text: "4.18 Inseminação artificial, fertilização in vitro e congêneres", value: EnumListaServico.LS418 },
    { text: "4.19 Bancos de sangue, leite, pele, olhos, óvulos, sêmen e congêneres", value: EnumListaServico.LS419 },
    { text: "4.20 Coleta de sangue, leite, tecidos, sêmen, órgãos e materiais biológicos de qualquer espécie", value: EnumListaServico.LS420 },
    { text: "4.21 Unidade de atendimento, assistência ou tratamento móvel e congêneres", value: EnumListaServico.LS421 },
    { text: "4.22 Planos de medicina de grupo ou individual e convênios para prestação de assistência médica, hospitalar, odontológica e congêneres", value: EnumListaServico.LS422 },
    { text: "4.23 Outros planos de saúde que se cumpram através de serviços de terceiros contratados, credenciados, cooperados ou apenas pagos pelo operador do plano mediante indicação do beneficiário", value: EnumListaServico.LS423 },
    { text: "5.01 Medicina veterinária e zootecnia", value: EnumListaServico.LS501 },
    { text: "5.02 Hospitais, clínicas, ambulatórios, prontos-socorros e congêneres, na área veterinária", value: EnumListaServico.LS502 },
    { text: "5.03 Laboratórios de análise na área veterinária", value: EnumListaServico.LS503 },
    { text: "5.04 Inseminação artificial, fertilização in vitro e congêneres", value: EnumListaServico.LS504 },
    { text: "5.05 Bancos de sangue e de órgãos e congêneres", value: EnumListaServico.LS505 },
    { text: "5.06 Coleta de sangue, leite, tecidos, sêmen, órgãos e materiais biológicos de qualquer espécie", value: EnumListaServico.LS506 },
    { text: "5.07 Unidade de atendimento, assistência ou tratamento móvel e congêneres", value: EnumListaServico.LS507 },
    { text: "5.08 Guarda, tratamento, amestramento, embelezamento, alojamento e congêneres", value: EnumListaServico.LS508 },
    { text: "5.09 Planos de atendimento e assistência médico-veterinária", value: EnumListaServico.LS509 },
    { text: "6.01 Barbearia, cabeleireiros, manicuros, pedicuros e congêneres", value: EnumListaServico.LS601 },
    { text: "6.02 Esteticistas, tratamento de pele, depilação e congêneres", value: EnumListaServico.LS602 },
    { text: "6.03 Banhos, duchas, sauna, massagens e congêneres", value: EnumListaServico.LS603 },
    { text: "6.04 Ginástica, dança, esportes, natação, artes marciais e demais atividades físicas", value: EnumListaServico.LS604 },
    { text: "6.05 Centros de emagrecimento, spa e congêneres", value: EnumListaServico.LS605 },
    { text: "7.01 Engenharia, agronomia, agrimensura, arquitetura, geologia, urbanismo, paisagismo e congêneres", value: EnumListaServico.LS701 },
    { text: "7.02 Execução, por administração, empreitada ou subempreitada, de obras de construção civil, hidráulica ou elétrica e de outras obras semelhantes, inclusive sondagem, perfuração de poços, escavação, drenagem e irrigação, terraplanagem, pavimentação, concretagem e a instalação e montagem de produtos, peças e equipamentos (exceto o fornecimento de mercadorias produzidas pelo prestador de serviços fora do local da prestação dos serviços, que fica sujeito ao ICMS)", value: EnumListaServico.LS702 },
    { text: "7.03 Elaboração de planos diretores, estudos de viabilidade, estudos organizacionais e outros, relacionados com obras e serviços de engenharia; elaboração de anteprojetos, projetos básicos e projetos executivos para trabalhos de engenharia", value: EnumListaServico.LS703 },
    { text: "7.04 Demolição", value: EnumListaServico.LS704 },
    { text: "7.05 Reparação, conservação e reforma de edifícios, estradas, pontes, portos e congêneres (exceto o fornecimento de mercadorias produzidas pelo prestador dos serviços, fora do local da prestação dos serviços, que fica sujeito ao ICMS)", value: EnumListaServico.LS705 },
    { text: "7.06 Colocação e instalação de tapetes, carpetes, assoalhos, cortinas, revestimentos de parede, vidros, divisórias, placas de gesso e congêneres, com material fornecido pelo tomador do serviço", value: EnumListaServico.LS706 },
    { text: "7.07 Recuperação, raspagem, polimento e lustração de pisos e congêneres", value: EnumListaServico.LS707 },
    { text: "7.08 Calafetação", value: EnumListaServico.LS708 },
    { text: "7.09 Varrição, coleta, remoção, incineração, tratamento, reciclagem, separação e destinação final de lixo, rejeitos e outros resíduos quaisquer", value: EnumListaServico.LS709 },
    { text: "7.10 Limpeza, manutenção e conservação de vias e logradouros públicos, imóveis, chaminés, piscinas, parques, jardins e congêneres", value: EnumListaServico.LS710 },
    { text: "7.11 Decoração e jardinagem, inclusive corte e poda de árvores", value: EnumListaServico.LS711 },
    { text: "7.12 Controle e tratamento de efluentes de qualquer natureza e de agentes físicos, químicos e biológicos", value: EnumListaServico.LS712 },
    { text: "7.13 Dedetização, desinfecção, desinsetização, imunização, higienização, desratização, pulverização e congêneres", value: EnumListaServico.LS713 },
    { text: "7.14 (VETADO)", value: EnumListaServico.LS714 },
    { text: "7.15 (VETADO)", value: EnumListaServico.LS715 },
    { text: "7.16Florestamento, reflorestamento, semeadura, adubação e congêneres", value: EnumListaServico.LS716 },
    { text: "7.17 Escoramento, contenção de encostas e serviços congêneres", value: EnumListaServico.LS717 },
    { text: "7.18 Limpeza e dragagem de rios, portos, canais, baías, lagos, lagoas, represas, açudes e congêneres", value: EnumListaServico.LS718 },
    { text: "7.19 Acompanhamento e fiscalização da execução de obras de engenharia, arquitetura e urbanismo", value: EnumListaServico.LS719 },
    { text: "7.20 Aerofotogrametria (inclusive interpretação), cartografia, mapeamento, levantamentos topográficos, batimétricos, geográficos, geodésicos, geológicos, geofísicos e congêneres", value: EnumListaServico.LS720 },
    { text: "7.21 Pesquisa, perfuração, cimentação, mergulho, perfilagem, concretação, testemunhagem, pescaria, estimulação e outros serviços relacionados com a exploração e explotação de petróleo, gás natural e de outros recursos minerais", value: EnumListaServico.LS721 },
    { text: "7.21 Nucleação e bombardeamento de nuvens e congêneres", value: EnumListaServico.LS722 },
    { text: "8.01 Ensino regular pré-escolar, fundamental, médio e superior", value: EnumListaServico.LS801 },
    { text: "8.02 Instrução, treinamento, orientação pedagógica e educacional, avaliação de conhecimentos de qualquer natureza", value: EnumListaServico.LS802 },
    { text: "9.01 Hospedagem de qualquer natureza em hotéis, apart-service condominiais, flat, apart-hotéis, hotéis residência, residence-service, suite service, hotelaria marítima, motéis, pensões e congêneres; ocupação por temporada com fornecimento de serviço (o valor da alimentação e gorjeta, quando incluído no preço da diária, fica sujeito ao Imposto Sobre Serviços)", value: EnumListaServico.LS901 },
    { text: "9.02 Agenciamento, organização, promoção, intermediação e execução de programas de turismo, passeios, viagens, excursões, hospedagens e congêneres", value: EnumListaServico.LS902 },
    { text: "9.03 Guias de turismo", value: EnumListaServico.LS903 },
    { text: "10.01 Agenciamento, corretagem ou intermediação de câmbio, de seguros, de cartões de crédito, de planos de saúde e de planos de previdência privada", value: EnumListaServico.LS1001 },
    { text: "10.02 Agenciamento, corretagem ou intermediação de títulos em geral, valores mobiliários e contratos quaisquer", value: EnumListaServico.LS1002 },
    { text: "10.03 Agenciamento, corretagem ou intermediação de direitos de propriedade industrial, artística ou literária", value: EnumListaServico.LS1003 },
    { text: "10.04 Agenciamento, corretagem ou intermediação de contratos de arrendamento mercantil (leasing), de franquia (franchising) e de faturização (factoring)", value: EnumListaServico.LS1004 },
    { text: "10.05 Agenciamento, corretagem ou intermediação de bens móveis ou imóveis, não abrangidos em outros itens ou subitens, inclusive aqueles realizados no âmbito de Bolsas de Mercadorias e Futuros, por quaisquer meios", value: EnumListaServico.LS1005 },
    { text: "10.06 Agenciamento marítimo", value: EnumListaServico.LS1006 },
    { text: "10.07 Agenciamento de notícias", value: EnumListaServico.LS1007 },
    { text: "10.08 Agenciamento de publicidade e propaganda, inclusive o agenciamento de veiculação por quaisquer meios", value: EnumListaServico.LS1008 },
    { text: "10.09 Representação de qualquer natureza, inclusive comercial", value: EnumListaServico.LS1009 },
    { text: "10.10 Distribuição de bens de terceiros", value: EnumListaServico.LS1010 },
    { text: "11.01 Guarda e estacionamento de veículos terrestres automotores, de aeronaves e de embarcações", value: EnumListaServico.LS1101 },
    { text: "11.02 Vigilância, segurança ou monitoramento de bens e pessoas", value: EnumListaServico.LS1102 },
    { text: "11.03 Escolta, inclusive de veículos e cargas", value: EnumListaServico.LS1103 },
    { text: "11.04 Armazenamento, depósito, carga, descarga, arrumação e guarda de bens de qualquer espécie", value: EnumListaServico.LS1104 },
    { text: "12.01 Espetáculos teatrais", value: EnumListaServico.LS1201 },
    { text: "12.02 Exibições cinematográficas", value: EnumListaServico.LS1202 },
    { text: "12.03 Espetáculos circenses", value: EnumListaServico.LS1203 },
    { text: "12.04 Programas de auditório", value: EnumListaServico.LS1204 },
    { text: "12.05 Parques de diversões, centros de lazer e congêneres", value: EnumListaServico.LS1205 },
    { text: "12.06 Boates, taxi-dancing e congêneres", value: EnumListaServico.LS1206 },
    { text: "12.07 Shows, ballet, danças, desfiles, bailes, óperas, concertos, recitais, festivais e congêneres", value: EnumListaServico.LS1207 },
    { text: "12.08 Feiras, exposições, congressos e congêneres", value: EnumListaServico.LS1208 },
    { text: "12.09 Bilhares, boliches e diversões eletrônicas ou não", value: EnumListaServico.LS1209 },
    { text: "12.10 Corridas e competições de animais", value: EnumListaServico.LS1210 },
    { text: "12.11 Competições esportivas ou de destreza física ou intelectual, com ou sem a participação do espectador", value: EnumListaServico.LS1211 },
    { text: "12.12 Execução de música", value: EnumListaServico.LS1212 },
    { text: "12.13 Produção, mediante ou sem encomenda prévia, de eventos, espetáculos, entrevistas, shows, ballet, danças, desfiles, bailes, teatros, óperas, concertos, recitais, festivais e congêneres", value: EnumListaServico.LS1213 },
    { text: "12.14 Fornecimento de música para ambientes fechados ou não, mediante transmissão por qualquer processo", value: EnumListaServico.LS1214 },
    { text: "12.15 Desfiles de blocos carnavalescos ou folclóricos, trios elétricos e congêneres", value: EnumListaServico.LS1215 },
    { text: "12.16 Exibição de filmes, entrevistas, musicais, espetáculos, shows, concertos, desfiles, óperas, competições esportivas, de destreza intelectual ou congêneres", value: EnumListaServico.LS1216 },
    { text: "12.17 Recreação e animação, inclusive em festas e eventos de qualquer natureza", value: EnumListaServico.LS1217 },
    { text: "13.01 (VETADO)", value: EnumListaServico.LS1301 },
    { text: "13.02 Fonografia ou gravação de sons, inclusive trucagem, dublagem, mixagem e congêneres", value: EnumListaServico.LS1302 },
    { text: "13.03 Fotografia e cinematografia, inclusive revelação, ampliação, cópia, reprodução, trucagem e congêneres", value: EnumListaServico.LS1303 },
    { text: "13.04 Reprografia, microfilmagem e digitalização", value: EnumListaServico.LS1304 },
    { text: "13.05 Composição gráfica, fotocomposição, clicheria, zincografia, litografia, fotolitografia", value: EnumListaServico.LS1305 },
    { text: "14.01 Lubrificação, limpeza, lustração, revisão, carga e recarga, conserto, restauração, blindagem, manutenção e conservação de máquinas, veículos, aparelhos, equipamentos, motores, elevadores ou de qualquer objeto (exceto peças e partes empregadas, que ficam sujeitas ao ICMS)", value: EnumListaServico.LS1401 },
    { text: "14.02 Assistência técnica", value: EnumListaServico.LS1402 },
    { text: "14.03 Recondicionamento de motores (exceto peças e partes empregadas, que ficam sujeitas ao ICMS)", value: EnumListaServico.LS1403 },
    { text: "14.04 Recauchutagem ou regeneração de pneus", value: EnumListaServico.LS1404 },
    { text: "14.05 Restauração, recondicionamento, acondicionamento, pintura, beneficiamento, lavagem, secagem, tingimento, galvanoplastia, anodização, corte, recorte, polimento, plastificação e congêneres, de objetos quaisquer", value: EnumListaServico.LS1405 },
    { text: "14.06 Instalação e montagem de aparelhos, máquinas e equipamentos, inclusive montagem industrial, prestados ao usuário final, exclusivamente com material por ele fornecido", value: EnumListaServico.LS1406 },
    { text: "14.07 Colocação de molduras e congêneres", value: EnumListaServico.LS1407 },
    { text: "14.08 Encadernação, gravação e douração de livros, revistas e congêneres", value: EnumListaServico.LS1408 },
    { text: "14.09 Alfaiataria e costura, quando o material for fornecido pelo usuário final, exceto aviamento", value: EnumListaServico.LS1409 },
    { text: "14.10 Tinturaria e lavanderia", value: EnumListaServico.LS1410 },
    { text: "14.11 Tapeçaria e reforma de estofamentos em geral", value: EnumListaServico.LS1411 },
    { text: "14.12 Funilaria e lanternagem", value: EnumListaServico.LS1412 },
    { text: "14.13 Carpintaria e serralheria", value: EnumListaServico.LS1413 },
    { text: "15.01 Administração de fundos quaisquer, de consórcio, de cartão de crédito ou débito e congêneres, de carteira de clientes, de cheques pré-datados e congêneres", value: EnumListaServico.LS1501 },
    { text: "15.02 Abertura de contas em geral, inclusive conta-corrente, conta de investimentos e aplicação e caderneta de poupança, no País e no exterior, bem como a manutenção das referidas contas ativas e inativas", value: EnumListaServico.LS1502 },
    { text: "15.03 Locação e manutenção de cofres particulares, de terminais eletrônicos, de terminais de atendimento e de bens e equipamentos em geral", value: EnumListaServico.LS1503 },
    { text: "15.04 Fornecimento ou emissão de atestados em geral, inclusive atestado de idoneidade, atestado de capacidade financeira e congêneres", value: EnumListaServico.LS1504 },
    { text: "15.05 Cadastro, elaboração de ficha cadastral, renovação cadastral e congêneres, inclusão ou exclusão no Cadastro de Emitentes de Cheques sem Fundos – CCF ou em quaisquer outros bancos cadastrais", value: EnumListaServico.LS1505 },
    { text: "15.06 Emissão, reemissão e fornecimento de avisos, comprovantes e documentos em geral; abono de firmas; coleta e entrega de documentos, bens e valores; comunicação com outra agência ou com a administração central; licenciamento eletrônico de veículos; transferência de veículos; agenciamento fiduciário ou depositário; devolução de bens em custódia", value: EnumListaServico.LS1506 },
    { text: "15.07 Acesso, movimentação, atendimento e consulta a contas em geral, por qualquer meio ou processo, inclusive por telefone, fac-símile, internet e telex, acesso a terminais de atendimento, inclusive vinte e quatro horas; acesso a outro banco e a rede compartilhada; fornecimento de saldo, extrato e demais informações relativas a contas em geral, por qualquer meio ou processo", value: EnumListaServico.LS1507 },
    { text: "15.08 Emissão, reemissão, alteração, cessão, substituição, cancelamento e registro de contrato de crédito; estudo, análise e avaliação de operações de crédito; emissão, concessão, alteração ou contratação de aval, fiança, anuência e congêneres; serviços relativos a abertura de crédito, para quaisquer fins", value: EnumListaServico.LS1508 },
    { text: "15.09 Arrendamento mercantil (leasing) de quaisquer bens, inclusive cessão de direitos e obrigações, substituição de garantia, alteração, cancelamento e registro de contrato, e demais serviços relacionados ao arrendamento mercantil (leasing)", value: EnumListaServico.LS1509 },
    { text: "15.10 Serviços relacionados a cobranças, recebimentos ou pagamentos em geral, de títulos quaisquer, de contas ou carnês, de câmbio, de tributos e por conta de terceiros, inclusive os efetuados por meio eletrônico, automático ou por máquinas de atendimento; fornecimento de posição de cobrança, recebimento ou pagamento; emissão de carnês, fichas de compensação, impressos e documentos em geral", value: EnumListaServico.LS1510 },
    { text: "15.11 Devolução de títulos, protesto de títulos, sustação de protesto, manutenção de títulos, reapresentação de títulos, e demais serviços a eles relacionados", value: EnumListaServico.LS1511 },
    { text: "15.12 Custódia em geral, inclusive de títulos e valores mobiliários", value: EnumListaServico.LS1512 },
    { text: "15.13 Serviços relacionados a operações de câmbio em geral, edição, alteração, prorrogação, cancelamento e baixa de contrato de câmbio; emissão de registro de exportação ou de crédito; cobrança ou depósito no exterior; emissão, fornecimento e cancelamento de cheques de viagem; fornecimento, transferência, cancelamento e demais serviços relativos a carta de crédito de importação, exportação e garantias recebidas; envio e recebimento de mensagens em geral relacionadas a operações de câmbio", value: EnumListaServico.LS1513 },
    { text: "15.14 Fornecimento, emissão, reemissão, renovação e manutenção de cartão magnético, cartão de crédito, cartão de débito, cartão salário e congêneres", value: EnumListaServico.LS1514 },
    { text: "15.15 Compensação de cheques e títulos quaisquer; serviços relacionados a depósito, inclusive depósito identificado, a saque de contas quaisquer, por qualquer meio ou processo, inclusive em terminais eletrônicos e de atendimento", value: EnumListaServico.LS1515 },
    { text: "15.16 Emissão, reemissão, liquidação, alteração, cancelamento e baixa de ordens de pagamento, ordens de crédito e similares, por qualquer meio ou processo; serviços relacionados à transferência de valores, dados, fundos, pagamentos e similares, inclusive entre contas em geral", value: EnumListaServico.LS1516 },
    { text: "15.17 Emissão, fornecimento, devolução, sustação, cancelamento e oposição de cheques quaisquer, avulso ou por talão", value: EnumListaServico.LS1517 },
    { text: "15.18 Serviços relacionados a crédito imobiliário, avaliação e vistoria de imóvel ou obra, análise técnica e jurídica, emissão, reemissão, alteração, transferência e renegociação de contrato, emissão e reemissão do termo de quitação e demais serviços relacionados a crédito imobiliário", value: EnumListaServico.LS1518 },
    { text: "16.01 Serviços de transporte de natureza municipal", value: EnumListaServico.LS1601 },
    { text: "17.01 Assessoria ou consultoria de qualquer natureza, não contida em outros itens desta lista; análise, exame, pesquisa, coleta, compilação e fornecimento de dados e informações de qualquer natureza, inclusive cadastro e similares", value: EnumListaServico.LS1701 },
    { text: "17.02 Datilografia, digitação, estenografia, expediente, secretaria em geral, resposta audível, redação, edição, interpretação, revisão, tradução, apoio e infra-estrutura administrativa e congêneres", value: EnumListaServico.LS1702 },
    { text: "17.03 Planejamento, coordenação, programação ou organização técnica, financeira ou administrativa", value: EnumListaServico.LS1703 },
    { text: "17.04 Recrutamento, agenciamento, seleção e colocação de mão-de-obra", value: EnumListaServico.LS1704 },
    { text: "17.05 Fornecimento de mão-de-obra, mesmo em caráter temporário, inclusive de empregados ou trabalhadores, avulsos ou temporários, contratados pelo prestador de serviço", value: EnumListaServico.LS1705 },
    { text: "17.06 Propaganda e publicidade, inclusive promoção de vendas, planejamento de campanhas ou sistemas de publicidade, elaboração de desenhos, textos e demais materiais publicitários", value: EnumListaServico.LS1706 },
    { text: "17.07 (VETADO)", value: EnumListaServico.LS1707 },
    { text: "17.08 Franquia (franchising)", value: EnumListaServico.LS1708 },
    { text: "17.09 Perícias, laudos, exames técnicos e análises técnicas", value: EnumListaServico.LS1709 },
    { text: "17.10 Planejamento, organização e administração de feiras, exposições, congressos e congêneres", value: EnumListaServico.LS1710 },
    { text: "17.11 Organização de festas e recepções; bufê (exceto o fornecimento de alimentação e bebidas, que fica sujeito ao ICMS)", value: EnumListaServico.LS1711 },
    { text: "17.12 Administração em geral, inclusive de bens e negócios de terceiros", value: EnumListaServico.LS1712 },
    { text: "17.13 Leilão e congêneres", value: EnumListaServico.LS1713 },
    { text: "17.14 Advocacia", value: EnumListaServico.LS1714 },
    { text: "17.15 Arbitragem de qualquer espécie, inclusive jurídica", value: EnumListaServico.LS1715 },
    { text: "17.16 Auditoria", value: EnumListaServico.LS1716 },
    { text: "17.17 Análise de Organização e Métodos", value: EnumListaServico.LS1717 },
    { text: "17.18 Atuária e cálculos técnicos de qualquer natureza", value: EnumListaServico.LS1718 },
    { text: "17.19 Contabilidade, inclusive serviços técnicos e auxiliares", value: EnumListaServico.LS1719 },
    { text: "17.20 Consultoria e assessoria econômica ou financeira", value: EnumListaServico.LS1720 },
    { text: "17.21 Estatística", value: EnumListaServico.LS1721 },
    { text: "17.22 Cobrança em geral", value: EnumListaServico.LS1722 },
    { text: "17.23 Assessoria, análise, avaliação, atendimento, consulta, cadastro, seleção, gerenciamento de informações, administração de contas a receber ou a pagar e em geral, relacionados a operações de faturização (factoring)", value: EnumListaServico.LS1723 },
    { text: "17.24 Apresentação de palestras, conferências, seminários e congêneres", value: EnumListaServico.LS1724 },
    { text: "18.01 Serviços de regulação de sinistros vinculados a contratos de seguros; inspeção e avaliação de riscos para cobertura de contratos de seguros; prevenção e gerência de riscos seguráveis e congêneres", value: EnumListaServico.LS1801 },
    { text: "19.02 Serviços de distribuição e venda de bilhetes e demais produtos de loteria, bingos, cartões, pules ou cupons de apostas, sorteios, prêmios, inclusive os decorrentes de títulos de capitalização e congêneres", value: EnumListaServico.LS1901 },
    { text: "20.01 Serviços portuários, ferroportuários, utilização de porto, movimentação de passageiros, reboque de embarcações, rebocador escoteiro, atracação, desatracação, serviços de praticagem, capatazia, armazenagem de qualquer natureza, serviços acessórios, movimentação de mercadorias, serviços de apoio marítimo, de movimentação ao largo, serviços de armadores, estiva, conferência, logística e congêneres", value: EnumListaServico.LS2001 },
    { text: "20.02 Serviços aeroportuários, utilização de aeroporto, movimentação de passageiros, armazenagem de qualquer natureza, capatazia, movimentação de aeronaves, serviços de apoio aeroportuários, serviços acessórios, movimentação de mercadorias, logística e congêneres", value: EnumListaServico.LS2002 },
    { text: "20.03 Serviços de terminais rodoviários, ferroviários, metroviários, movimentação de passageiros, mercadorias, inclusive suas operações, logística e congêneres", value: EnumListaServico.LS2003 },
    { text: "21.01 Serviços de registros públicos, cartorários e notariais", value: EnumListaServico.LS2101 },
    { text: "22.02 Serviços de exploração de rodovia mediante cobrança de preço ou pedágio dos usuários, envolvendo execução de serviços de conservação, manutenção, melhoramentos para adequação de capacidade e segurança de trânsito, operação, monitoração, assistência aos usuários e outros serviços definidos em contratos, atos de concessão ou de permissão ou em normas oficiais", value: EnumListaServico.LS2201 },
    { text: "23.01 Serviços de programação e comunicação visual, desenho industrial e congêneres", value: EnumListaServico.LS2301 },
    { text: "24.01 Serviços de chaveiros, confecção de carimbos, placas, sinalização visual, banners, adesivos e congêneres", value: EnumListaServico.LS2401 },
    { text: "25.01 Funerais, inclusive fornecimento de caixão, urna ou esquifes; aluguel de capela; transporte do corpo cadavérico; fornecimento de flores, coroas e outros paramentos; desembaraço de certidão de óbito; fornecimento de véu, essa e outros adornos; embalsamento, embelezamento, conservação ou restauração de cadáveres", value: EnumListaServico.LS2501 },
    { text: "25.02 Cremação de corpos e partes de corpos cadavéricos", value: EnumListaServico.LS2502 },
    { text: "25.03 Planos ou convênio funerários", value: EnumListaServico.LS2503 },
    { text: "25.04 Manutenção e conservação de jazigos e cemitérios", value: EnumListaServico.LS2504 },
    { text: "26.01 Serviços de coleta, remessa ou entrega de correspondências, documentos, objetos, bens ou valores, inclusive pelos correios e suas agências franqueadas; courrier e congêneres", value: EnumListaServico.LS2601 },
    { text: "27.01 Serviços de assistência social", value: EnumListaServico.LS2701 },
    { text: "28.01 Serviços de avaliação de bens e serviços de qualquer natureza", value: EnumListaServico.LS2801 },
    { text: "29.01 Serviços de biblioteconomia", value: EnumListaServico.LS2901 },
    { text: "30.01 Serviços de biologia, biotecnologia e química", value: EnumListaServico.LS3001 },
    { text: "31.01 Serviços técnicos em edificações, eletrônica, eletrotécnica, mecânica, telecomunicações e congêneres", value: EnumListaServico.LS3101 },
    { text: "32.01 Serviços de desenhos técnicos", value: EnumListaServico.LS3201 },
    { text: "33.01 Serviços de desembaraço aduaneiro, comissários, despachantes e congêneres", value: EnumListaServico.LS3301 },
    { text: "34.01 Serviços de investigações particulares, detetives e congêneres", value: EnumListaServico.LS3401 },
    { text: "35.01 Serviços de reportagem, assessoria de imprensa, jornalismo e relações públicas", value: EnumListaServico.LS3501 },
    { text: "36.01 Serviços de meteorologia", value: EnumListaServico.LS3601 },
    { text: "37.01 Serviços de artistas, atletas, modelos e manequins", value: EnumListaServico.LS3701 },
    { text: "38.01 Serviços de museologia", value: EnumListaServico.LS3801 },
    { text: "39.01 Serviços de ourivesaria e lapidação (quando o material for fornecido pelo tomador do serviço)", value: EnumListaServico.LS3901 },
    { text: "40.01 Obras de arte sob encomenda", value: EnumListaServico.LS4001 }
]

var PesquisaServico = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridServico.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var Servico = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, maxlength: 300 });
    this.DescricaoNFE = PropertyEntity({ text: "*Descrição na Nota Fiscal: ", required: true, maxlength: 60 });
    this.CodigoTributacao = PropertyEntity({ text: "Código Tributação: ", required: false, maxlength: 20 });
    this.ValorVenda = PropertyEntity({ text: "*Valor Venda: ", required: true, getType: typesKnockout.decimal, maxlength: 18 });
    this.AliquotaISS = PropertyEntity({ text: "Alíquota ISS: ", required: false, getType: typesKnockout.decimal, maxlength: 18 });
    this.CodigoServico = PropertyEntity({ options: _listaServico, val: ko.observable(0), def: ko.observable(0), text: "*Lista Serviço: ", required: true });
    this.Numero = PropertyEntity({ text: "Número: ", maxlength: 20, getType: typesKnockout.int, configInt: { precision: 0, allowZero: true } });
    this.CNAE = PropertyEntity({ text: "CNAE: ", maxlength: 100 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", maxlength: 50 });

    this.CFOPVendaDentroEstado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Venda para Dentro do Estado:", idBtnSearch: guid(), required: false, visible: true });
    this.CFOPVendaForaEstado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Venda para Fora do Estado:", idBtnSearch: guid(), required: false, visible: true });
    this.CFOPCompraDentroEstado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Compra para Dentro do Estado:", idBtnSearch: guid(), required: false, visible: false });
    this.CFOPCompraForaEstado = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "CFOP de Compra para Fora do Estado:", idBtnSearch: guid(), required: false, visible: false });
    this.Empresa = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Empresa:", idBtnSearch: guid(), required: false, visible: false });
    this.Localidade = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Localidade:", idBtnSearch: guid() });
};

var CRUDServico = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadServico() {

    _pesquisaServico = new PesquisaServico();
    KoBindings(_pesquisaServico, "knockoutPesquisaServico", false, _pesquisaServico.Pesquisar.id);

    _servico = new Servico();
    KoBindings(_servico, "knockoutCadastroServico");

    HeaderAuditoria("Servico", _servico);

    _crudServico = new CRUDServico();
    KoBindings(_crudServico, "knockoutCRUDServico");

    new BuscarCFOPNotaFiscal(_servico.CFOPVendaDentroEstado, null, EnumTipoCFOP.Saida, "S");
    new BuscarCFOPNotaFiscal(_servico.CFOPVendaForaEstado, null, EnumTipoCFOP.Saida, "N");
    new BuscarCFOPNotaFiscal(_servico.CFOPCompraDentroEstado, null, EnumTipoCFOP.Entrada, "S");
    new BuscarCFOPNotaFiscal(_servico.CFOPCompraForaEstado, null, EnumTipoCFOP.Entrada, "N");
    new BuscarLocalidades(_servico.Localidade);

    buscarServicos();
}

function adicionarClick(e, sender) {
    if (!ValidarListaServico())
        return;

    Salvar(_servico, "ServicoNotaFiscal/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridServico.CarregarGrid();
                limparCamposServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    if (!ValidarListaServico())
        return;

    Salvar(_servico, "ServicoNotaFiscal/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridServico.CarregarGrid();
                limparCamposServico();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o serviço " + _servico.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_servico, "ServicoNotaFiscal/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridServico.CarregarGrid();
                    limparCamposServico();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposServico();
}

//*******MÉTODOS*******

function buscarServicos() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarServico, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridServico = new GridView(_pesquisaServico.Pesquisar.idGrid, "ServicoNotaFiscal/Pesquisa", _pesquisaServico, menuOpcoes, null);
    _gridServico.CarregarGrid();
}

function editarServico(servicoGrid) {
    limparCamposServico();
    _servico.Codigo.val(servicoGrid.Codigo);
    BuscarPorCodigo(_servico, "ServicoNotaFiscal/BuscarPorCodigo", function (arg) {
        _pesquisaServico.ExibirFiltros.visibleFade(false);
        _crudServico.Atualizar.visible(true);
        _crudServico.Cancelar.visible(true);
        _crudServico.Excluir.visible(true);
        _crudServico.Adicionar.visible(false);
    }, null);
}

function limparCamposServico() {
    _crudServico.Atualizar.visible(false);
    _crudServico.Cancelar.visible(false);
    _crudServico.Excluir.visible(false);
    _crudServico.Adicionar.visible(true);
    LimparCampos(_servico);
    _servico.CodigoServico.val(0);
}

function ValidarListaServico() {
    if (_servico.CodigoServico.val() != 0) {
        return true;
    } else {
        exibirMensagem("atencao", "Campo Obrigatório", "Por Favor, Informe os campos obrigatórios");
        return false;
    }
}