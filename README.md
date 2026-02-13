# Sales Lab – Dynamics 365 CRM

Projeto de CRM de vendas desenvolvido no Microsoft Dynamics 365, utilizando Dataverse e Model-Driven App.  
O objetivo do projeto é simular um cenário real de gestão de clientes, produtos e pedidos, aplicando boas práticas de modelagem de dados, customização client-side e implementação de lógica server-side com plugins C# no Dynamics 365.

---

## 🎯 Objetivo do Projeto

Criar um sistema de CRM funcional para controle de vendas, permitindo:

- Cadastro de clientes  
- Cadastro de produtos  
- Criação e gerenciamento de pedidos  
- Relacionamento entre dados de forma estruturada  
- Validação de regras de negócio (client-side e server-side)

Este projeto está sendo desenvolvido com foco em aprendizado prático e construção de portfólio para vagas de estágio e nível júnior em Dynamics 365.

---

## 🧱 Modelagem de Dados (Dataverse)

### Tabela Cliente

Responsável por armazenar as informações dos clientes.

Campos principais:

- Nome  
- Email  
- Telefone  

![Tabela Cliente](images/cliente.png)

---

### Tabela Produto

Armazena os produtos disponíveis para venda.

Campos principais:

- Nome do Produto  
- Preço  
- Estoque  
- Status  

![Tabela Produto](images/produto%20(1).png)

---

### Tabela Pedido

Representa os pedidos realizados pelos clientes.

Campos principais:

- Cliente  
- Produto  
- Quantidade  
- Valor Total  
- Status do Pedido  

![Tabela Pedido](images/pedido.png)

---

## 🔗 Relacionamentos

- **Cliente (1:N) Pedido**  
  Um cliente pode possuir vários pedidos.

- **Produto (1:N) Pedido**  
  Um produto pode estar associado a vários pedidos.

---

## 🧩 Model-Driven App

Foi criado um Model-Driven App baseado nas tabelas do Dataverse, permitindo a navegação e o gerenciamento dos dados de forma padrão do Dynamics 365.

Funcionalidades disponíveis:

- Cadastro e edição de Clientes  
- Cadastro e edição de Produtos  
- Criação e acompanhamento de Pedidos  

---

## ⚙️ Business Rules

Foram implementadas regras de negócio para validação e controle dos dados diretamente no formulário do Pedido.

---

### ✔️ Regra 1 – Validação da Quantidade do Pedido

Garante que a quantidade informada seja maior que zero.

Se a quantidade for menor ou igual a zero:

- O sistema exibe mensagem de erro  
- O salvamento do registro é impedido  

📷 Estrutura da regra:

![BR Quantidade Designer](images/business-rules/BR-validar-quantidade-designer.png)

📷 Funcionamento no formulário:

![BR Quantidade Erro](images/business-rules/BR-validar-quantidade-erro-form.png)

---

### ✔️ Regra 2 – Bloqueio de Campos ao Confirmar Pedido

Quando o status do pedido é alterado para **Confirmado**:

- Cliente, Produto e Quantidade são bloqueados para edição  
- Caso o pedido volte para Rascunho, os campos são liberados novamente  

📷 Estrutura da regra:

![BR Bloqueio Designer](images/business-rules/BR-bloquear-campos-confirmado-designer.png)

📷 Funcionamento com Pedido Confirmado:

![BR Campos Bloqueados](images/business-rules/BR-bloquear-campos-confirmado-form.png)

📷 Funcionamento com Pedido em Rascunho:

![BR Campos Liberados](images/business-rules/BR-bloquear-campos-rascunho-form.png)

---

## 💻 JavaScript (Formulário de Pedido)

Foi implementado um módulo JavaScript personalizado para melhorar a experiência do usuário e adicionar lógica dinâmica ao formulário de Pedido.

### 🎯 Objetivos da implementação

- Calcular automaticamente o **Valor Total** (Preço × Quantidade)  
- Utilizar Web API para buscar o preço do produto  
- Manipular campos dinamicamente via `formContext`  
- Garantir melhor usabilidade no preenchimento do Pedido  

---

### ⚙️ Lógica Implementada

O script é executado nos eventos:

- `OnChange` do campo Produto  
- `OnChange` do campo Quantidade  

Fluxo da lógica:

1. Quando Produto e Quantidade são preenchidos  
2. O sistema consulta o Preço do Produto via `Xrm.WebApi.retrieveRecord`  
3. O Valor Total é calculado automaticamente  
4. O campo Valor Total é atualizado no formulário  

---

### 🧠 Conceitos Utilizados (Client-Side)

- `formContext`  
- `getAttribute()` e `setValue()`  
- `setSubmitMode("always")`  
- `Xrm.WebApi.retrieveRecord`  
- Manipulação de Lookup  
- JavaScript modular (namespace pattern)  

---

# 🔐 Plugins (Server-Side – C#)

Foram implementados plugins em C# para garantir validações críticas no servidor, assegurando que as regras de negócio sejam aplicadas mesmo que o JavaScript ou Business Rules sejam burlados.

Os plugins foram desenvolvidos utilizando:

- `IPlugin`
- Pipeline de execução (Pre-Operation)
- `Target` e `PreEntityImages`
- `IOrganizationService`
- `InvalidPluginExecutionException`
- Strong Name Assembly

---

## ✔️ Plugin 1 – Cálculo Automático do Total

**Evento:** Create e Update da entidade `Pedido`  
**Estágio:** Pre-Operation (Stage 20)  
**Execução:** Synchronous  

### Regras implementadas:

- Impede salvar pedido com Quantidade ≤ 0  
- Busca o Preço do Produto via `service.Retrieve()`  
- Calcula o Total no servidor  
- Atualiza o campo `Valor Total` antes do registro ser persistido  

🔒 Garante integridade mesmo que o JavaScript seja removido.

---

## ✔️ Plugin 2 – Validação e Baixa de Estoque ao Confirmar Pedido

**Evento:**  
- Create (caso o pedido já seja criado como Confirmado)  
- Update (quando Status muda para Confirmado)

**Estágio:** Pre-Operation  
**Execução:** Synchronous  
**Image:** PreImage configurada  

### Regras implementadas:

- Detecta transição de Status para **Confirmado**  
- Verifica se há estoque suficiente  
- Impede confirmação caso estoque seja insuficiente  
- Atualiza automaticamente o estoque do Produto

---

## ✔️ Plugin 3 – Processamento Pós-Confirmação do Pedido

**Evento:** Update da entidade `Pedido`  
**Estágio:** Post-Operation (Stage 40)  
**Execução:** Synchronous  

### Regras implementadas:

- Detecta alteração do Status para **Confirmado**
- Executa lógica complementar após persistência do registro
- Pode ser utilizado para:
  - Criar logs de confirmação
  - Atualizar campos auxiliares (ex: Data de Confirmação)
  - Integrar com processos externos
  - Disparar automações futuras

---

### 📷 Demonstração – Validação Server-Side

#### Erro ao tentar confirmar pedido com estoque insuficiente

![Plugin Estoque Insuficiente](images/plugins/plugin-estoque-insuficiente-bloqueio.png)

---

## ⚙️ Tecnologias Utilizadas

- Microsoft Dynamics 365  
- Power Apps  
- Dataverse  
- Model-Driven App  
- Business Rules  
- JavaScript (Client-side customization)  
- C# Plugins (Server-side logic)  

---

## 📌 Status do Projeto

✔️ Modelagem de dados concluída  
✔️ Relacionamentos configurados  
✔️ Model-Driven App funcional  
✔️ Business Rules implementadas  
✔️ JavaScript implementado  
✔️ Plugins server-side implementados (C#)  

🔄 Próximos passos:

- Automações com Power Automate  
- Controle de permissões com Security Roles  
- Melhorias arquiteturais e versionamento de solução  

---

## 👤 Autor

Projeto desenvolvido por **Vinicius Ferreira** com foco em aprendizado prático e desenvolvimento profissional na área de Dynamics 365.
