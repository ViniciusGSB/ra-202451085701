# HANDOUT — AULA 02

## Dissecando o HTTP

*6 requisições sob o microscópio — Arquitetura de Aplicações Web*

## 🎯 MISSÃO

Vocês interceptaram 6 conversas entre um app e a API de uma biblioteca. Para CADA card:

- Descrevam o que o cliente pediu (verbo + recurso na URI)
- Expliquem o que o status code da resposta informa
- Respondam: repetindo a MESMA requisição 3 vezes seguidas, o estado do servidor muda?

Ao final, preencham juntos a TABELA-SÍNTESE dos verbos na última página.

*⏱️ Tempo: 30 minutos  |  👥 Formato: em duplas  |  Dica: o card 6 esconde uma pegadinha de quem é a culpa.*

> **Nomes:** ____________________   **Turma:** ____________________   **Data:** ___ / ___ / ______

## REQUISIÇÃO 01 — A prateleira inteira

```text
→ REQUISIÇÃO
GET /api/livros HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json
```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

[ { "id": 1, "titulo": "Clean Code", "autor": "Robert C. Martin" },
  { "id": 7, "titulo": "O Programador Pragmático", "autor": "Hunt & Thomas" } ]
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)? 
GET livros

2. O que o status code informa? Deu certo? Culpa de quem se não deu? 
"200 OK". Deu certo. Caso não, seria erro do servidor.

3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
o estado não muda, mas a resposta sim pois o recurso já terá sido requisitado.

## REQUISIÇÃO 02 — O livro fantasma

```text
→ REQUISIÇÃO
GET /api/livros/99 HTTP/1.1
Host: biblioteca.newton.br
Accept: application/json
```

```text
← RESPOSTA
HTTP/1.1 404 Not Found
Content-Type: application/problem+json

{ "title": "Not Found", "status": 404 }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
GET livros/99

2. O que o status code informa? Deu certo? Culpa de quem se não deu?
Não foi encontrado, é um erro de requisição do cliente.

3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
o estado não muda nem a resposta.

## REQUISIÇÃO 03 — Livro novo na estante

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "titulo": "Domain-Driven Design", "autor": "Eric Evans" }
```

```text
← RESPOSTA
HTTP/1.1 201 Created
Location: /api/livros/8
Content-Type: application/json

{ "id": 8, "titulo": "Domain-Driven Design", "autor": "Eric Evans" }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
POST livros

2. O que o status code informa? Deu certo? Culpa de quem se não deu?
201 Created, deu certo.

3. Enviando este POST 3 vezes seguidas, o que acontece na estante? Para que serve o header Location?
o POST ocorre 3 vezes, criando 3 recursos iguais. header location organiza onde estão os recursos.

## REQUISIÇÃO 04 — Corrigindo a ficha completa

```text
→ REQUISIÇÃO
PUT /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }
```

```text
← RESPOSTA
HTTP/1.1 200 OK
Content-Type: application/json

{ "id": 7, "titulo": "O Programador Pragmático", "autor": "D. Hunt; D. Thomas" }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
PUT livros/7

2. O que o status code informa? Deu certo? Culpa de quem se não deu?
200 OK, requisição sucedida.

3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
estado do servidor e resposta não mudam.

## REQUISIÇÃO 05 — Fora do catálogo

```text
→ REQUISIÇÃO
DELETE /api/livros/7 HTTP/1.1
Host: biblioteca.newton.br
```

```text
← RESPOSTA
HTTP/1.1 204 No Content
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
DELETE livros/7

2. O que o status code informa? Deu certo? Culpa de quem se não deu?
204 No Content, requisição sucedida, não precisa sair da página atual.

3. Repetindo o DELETE, o estado do servidor muda? Que resposta você ESPERA na segunda vez?
não muda, retornará um erro pois não haverá o deletado.

## REQUISIÇÃO 06 — O cadastro capenga

```text
→ REQUISIÇÃO
POST /api/livros HTTP/1.1
Host: biblioteca.newton.br
Content-Type: application/json

{ "autor": "Anônimo" }
```

```text
← RESPOSTA
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json

{ "title": "Bad Request", "status": 400,
  "errors": { "Titulo": [ "O campo Titulo é obrigatório" ] } }
```

**Sua análise:**

1. O que o cliente pediu (verbo + recurso)?
POST livros.

2. O que o status code informa? Deu certo? Culpa de quem se não deu?
400 Bad Request, erro, cliente fez requisição ruim.

3. Repetindo esta requisição 3 vezes seguidas, o estado do servidor muda? E a resposta?
o estado não muda nem a resposta, pois o erro será o mesmo.

## TABELA-SÍNTESE — Os verbos do HTTP

*Preencham com base nos 6 cards. “Seguro” = não altera nada no servidor. “Idempotente” = repetir N vezes deixa o servidor no mesmo estado que 1 vez.*

| **Verbo** | **Para que serve** | **Seguro?** | **Idempotente?** | **Status típicos** |
| --- | --- | --- | --- | --- |
| **`GET`** |      	Buscar/consultar um recurso              |      s       |         s         |200, 304, 404      |
| **`POST`**|Criar recurso ou executar uma operação          |      n       |         n         |201, 200, 202      |
| **`PUT`** |	Criar/substituir completamente um recurso    |      n       |         s         |200, 201, 204      |
| **`PATCH`**|    	Alterar parcialmente um recurso          |      n       |         n         |200, 204, 400      |
| **`DELETE`** |  	Remover um recurso                       |      n       |         s         |204, 200, 404      |

## DESAFIO

1. O verbo PATCH não apareceu em nenhum card. Qual a diferença entre PATCH e PUT? Um app de banco quer alterar SÓ o apelido do usuário, entre dezenas de campos do perfil — qual dos dois você usaria e por quê?