# Proposta – Trabalho Semestral · Arquitetura de Aplicações Web 2026.2

- **Aluno:** vinicius gasparini sander brettas · **Curso/Turma:** ADS/AAW - GNP0547
- **Repositório:** https://github.com/ViniciusGSB/ra-202451085701/tree/main

## 1. Domínio e problema
A aplicação resolve a dificuldade de gestão, planeamento e acompanhamento de ordens de serviço de arboricultura urbana para equipas técnicas e cidadãos. O sistema centraliza pedidos de poda, abate e vistoria de árvores, permitindo que a administração ou empresas do setor otimizem o fluxo operacional e o histórico de intervenções em zonas urbanas.

## 2. Entidades
- **Cliente:**
  - Atributos: `id`, `nome`, `telefone`, `endereco`
- **OrdemServico:**
  - Atributos: `id`, `descricao`, `grauRisco`, `estado`, `clienteId`
- **Relacionamento:** Relacionamento de 1-N (Um Cliente possui várias Ordens de Serviço). No MongoDB (NoSQL), será modelado utilizando **referência** (o documento da Ordem de Serviço guardará o `clienteId`), pois as ordens de serviço crescem continuamente em volume de forma independente do cadastro do cliente, evitando problemas com o limite de tamanho de documentos embutidos.

## 3. Endpoints previstos
- **Clientes:**
  - `GET /clientes` — Lista todos os clientes registados.
  - `GET /clientes/{id}` — Mostra os detalhes de um cliente específico.
  - `POST /clientes` — Regista um novo cliente.
  - `PUT /clientes/{id}` — Atualiza os dados de um cliente existente.
  - `DELETE /clientes/{id}` — Remove um cliente do sistema.
- **Ordens de Serviço:**
  - `GET /ordens-servico` — Lista todas as ordens de serviço.
  - `GET /ordens-servico/{id}` — Detalha uma ordem de serviço específica.
  - `POST /ordens-servico` — Cria uma nova ordem de serviço.
  - `PATCH /ordens-servico/{id}/estado` — Altera o estado de uma ordem de serviço.
  - `DELETE /ordens-servico/{id}` — Elimina uma ordem de serviço.

## 4. Regra de negócio
A camada de serviço validará estritamente a transição de estados da ordem de serviço (`Pendente` -> `Aprovado` -> `Em Execução` -> `Concluído`), impedindo saltos inválidos no fluxo de trabalho. Adicionalmente, ordens de serviço com grau de risco `Alto` exigem uma verificação prévia de segurança antes de passarem para o estado de execução.

## 5. Casos de erro
- **ID Inexistente (`404 Not Found`):** Devolvido quando é feita uma tentativa de consultar, atualizar ou apagar um cliente ou ordem de serviço cujo ID não existe na base de dados.
- **Transição de Estado Inválida (`400 Bad Request`):** Devolvido quando a camada de serviço deteta uma tentativa de alterar o estado de uma ordem de serviço violando a sequência lógica permitida pelas regras de negócio.

## 6. Stack escolhida
- **Backend:** .NET 10 (C#)
- **Banco de dados:** MongoDB (NoSQL)
- **Frontend:** HTML + JavaScript (fetch)

## 7. Segurança e testes
- **Segurança:** Autenticação baseada em tokens JWT gerados no endpoint de login. O sistema implementará RBAC com dois perfis: `utilizador` (permissão para criar e consultar pedidos) e `admin` (permissão total para alterar estados críticos, gerir dados e apagar registos).
- **Testes Unitários:** Implementação de testes na camada de serviços utilizando o framework `xUnit`, cobrindo 2 cenários de sucesso e 2 cenários de erro, executáveis por um único comando (`dotnet test`) documentado no `README.md`.
