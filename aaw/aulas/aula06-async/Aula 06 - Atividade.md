#Aula 06 - Atividade.md

## Síncrono ou Assíncrono?

*Análise de fluxos de comunicação entre serviços — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês são os arquitetos dos 4 fluxos abaixo. Para CADA cenário:

- Decidam o estilo de comunicação: síncrono (request/response), assíncrono (fila/evento) ou API Gateway/BFF
- Desenhem o fluxo com caixas (serviços) e setas (chamadas/mensagens) no espaço indicado
- Justifiquem com pelo menos 2 fatores (urgência da resposta, tolerância a atraso, picos, falhas...)
- Apontem o principal risco da escolha de vocês

*⏱️ Tempo: 25 minutos  |  👥 Formato: em duplas  |  Não existe resposta única — o que vale é a justificativa.*

> **Nomes:** vinicius gasparini sander brettas   **Turma:** ____________________   **Data:** 11 / 09 / 2926

## CENÁRIO 01 — PagFácil — aprovar ou negar AGORA

No checkout do PagFácil, ao clicar em “Pagar”, o serviço de Pagamentos precisa consultar o saldo/limite do cliente no serviço de Contas — e a resposta define se a venda acontece neste exato momento.

- O cliente está na tela, esperando o resultado da compra
- Sem a resposta de Contas, não há decisão possível: aprovar às cegas é proibido
- Tempo de resposta do serviço de Contas: ~80 ms em condições normais

**Sua análise:**

1. Estilo recomendado:   X Síncrono      ☐ Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

[ Cliente / Tela ]
       |
       | 1. Pagar (Requisição HTTP)
       v
[ Serviço de Pagamentos ]
       |
       | 2. POST/GET /saldo
       v
[ Serviço de Contas ]
       |
       | 3. Resposta com status do saldo/limite
       v
[ Serviço de Pagamentos ]
       |
       | 4. Retorna Status (Aprovado / Negado)
       v
[ Cliente / Tela ]

3. Justificativa (mínimo 2 fatores):

A regra de negócio exige validação prévia obrigatória enquanto o cliente aguarda na tela. A comunicação síncrona bloqueia o processamento até obter a resposta necessária para aprovar ou recusar a transação.
O tempo de resposta em condições normais é baixo para ser executado dentro de uma requisição HTTP sem estourar o limite aceitável de UX do checkout.

4. Principal risco da escolha:
O serviço de Pagamentos fica totalmente dependente da disponibilidade de Contas.

## CENÁRIO 02 — CadastraJá — o e-mail de boas-vindas

Após criar a conta no CadastraJá, o sistema envia um e-mail de boas-vindas. O provedor de e-mail às vezes demora 8 segundos para responder e falha em 2% das tentativas.

- O usuário quer começar a usar o app imediatamente após o cadastro
- O e-mail chegar 1 minuto depois não incomoda ninguém
- Se o provedor falhar, o envio deve ser tentado de novo — sem o usuário perceber

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      X Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

[ Cliente / App ]
       |
       | 1. Requisição de cadastro (HTTP POST)
       v
[ Serviço de Cadastro ]
       |
       | 2a. Salva usuário no Banco de Dados
       | 2b. Publica mensagem "UsuarioCriado" na Fila
       | 2c. Responde ao Cliente ("Cadastro Realizado com Sucesso")
       v
[ Fila / Broker ]
       |
       | 3. Consome evento de cadastro
       v
[ Serviço de Notificações / E-mail ]
       |
       | 4. Requisição de envio de e-mail (com Retry automático)
       v
[ Provedor de E-mail ]

3. Justificativa (mínimo 2 fatores):

O envio de e-mail pode demorar até 8 segundos. A arquitetura assíncrona libera o serviço para responder ao cliente instantaneamente, permitindo o uso imediato do app.
Para lidar com os 2% de falha do provedor externo, o consumo assíncrono permite aplicar retry em segundo plano sem impactar a requisição do usuário.

4. Principal risco da escolha:

O processamento em segundo plano oculta falhas da interface do usuário.

## CENÁRIO 03 — MegaMarket — baixa de estoque nos picos

No marketplace MegaMarket, cada venda gera uma baixa no serviço de Estoque. Nas grandes promoções o tráfego sobe 10x e o Estoque não dá conta de responder na velocidade das vendas.

- Atraso de alguns segundos na baixa é aceitável
- PERDER uma baixa de estoque não é aceitável (gera venda sem produto)
- O checkout não pode ficar lento nem cair porque o Estoque está sobrecarregado

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      X Assíncrono (fila/evento)      ☐ API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

[ Cliente / Checkout ]
       |
       | 1. Confirma Compra (HTTP POST)
       v
[ Serviço de Pedidos / Vendas ]
       |
       | 2a. Persiste o Pedido
       | 2b. Publica evento "PedidoRealizado" na Fila
       | 2c. Retorna confirmação de compra ao Cliente
       v
[ Fila / Broker ]
       |
       | 3. Consumo cadenciado
       v
[ Serviço de Estoque ]
       |
       | 4. Processa e efetiva a baixa no Banco de Estoque
       v
[ Banco de Dados do Estoque ]

3. Justificativa (mínimo 2 fatores):

A fila atua como um buffer amortecedor nos picos de tráfego de 10x. O checkout publica a mensagem instantaneamente e responde ao cliente, enquanto o Serviço de Estoque consome as mensagens em seu próprio ritmo, sem sobrecarregar o banco de dados.
Garantia de Durabilidade e Não Perda de Dados: Mensagens gravadas de forma persistente garantem processamento. Caso o Serviço de Estoque falhe ou caia, as mensagens permanecem retidas na fila para serem reprocessadas assim que ele restabelecer a operação.

4. Principal risco da escolha:

Produtos em estoque podem ser vendidos simultaneamente para múltiplos clientes durante o pico de tráfego antes da baixa efetivada.

## CENÁRIO 04 — AppBanco — uma tela, cinco serviços

A tela inicial do AppBanco mostra saldo, fatura do cartão, investimentos, empréstimos e cashback — dados de 5 serviços diferentes. O time mobile reclama: são 5 chamadas, 5 formatos de resposta e 5 pontos de falha em cada abertura do app.

- A tela precisa abrir rápido, inclusive em redes móveis ruins
- Cada serviço tem equipe, formato e autenticação próprios
- Amanhã nasce a versão web, que precisa de MAIS dados que a mobile

**Sua análise:**

1. Estilo recomendado:   ☐ Síncrono      ☐ Assíncrono (fila/evento)      X API Gateway/BFF

2. Desenhe o fluxo (caixas = serviços, setas = chamadas/mensagens):

[ Cliente Mobile ]                [ Cliente Web ]
        |                                |
        | 1. GET /home-data              | 1. GET /dashboard-data
        v                                v
[ BFF Mobile ]                      [ BFF Web ]
        |                                |
        +----------------+---------------+
                         |
                         | (Chamadas internas paralelas)
                         v
       +-----------------+-----------------+----------------------+------------------------+
       |                 |                 |                      |                        |
       v                 v                 v
[ Serv. Saldo ]   [ Serv. Fatura ]   [ Serv. Invest. ]   [ Serv. Empréstimos ]   [ Serv. Cashback ]

3. Justificativa (mínimo 2 fatores):

Reduz as 5 chamadas de rede móvel (alta latência e instabilidade) para 1 única requisição entre o aplicativo e o BFF. Na rede interna do banco o BFF dispara chamadas paralelas para os 5 serviços e consolida a resposta.
O padrão BFF permite criar camadas sob medida. O BFF Mobile entrega uma resposta enxuta, enquanto o BFF Web pode consultar e entregar dados adicionais sem afetar o contrato do app.

4. Principal risco da escolha:

A resposta final do BFF para o app será tão lenta quanto o serviço interno mais lento entre os cinco.

## DESAFIO

1. Escolha um cenário em que vocês indicaram ASSÍNCRONO. Os brokers de mensagens costumam garantir entrega “pelo menos uma vez” — ou seja, a MESMA mensagem pode chegar duas vezes. O que aconteceria no seu fluxo? Como o consumidor deveria se proteger?